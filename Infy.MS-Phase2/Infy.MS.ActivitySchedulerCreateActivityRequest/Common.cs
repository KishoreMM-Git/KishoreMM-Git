using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Infy.MS.ActivitySchedulerCreateActivityRequest
{
    public static class HttpClientExtensions
    {
        public static Task<HttpResponseMessage> SendAsJsonAsync<T>(this HttpClient client, HttpMethod method, string requestUri, T value)
        {
            var content = value.GetType().Name.Equals("JObject") ?
                value.ToString() :
                JsonConvert.SerializeObject(value, new JsonSerializerSettings() { DefaultValueHandling = DefaultValueHandling.Ignore });

            HttpRequestMessage request = new HttpRequestMessage(method, requestUri) { Content = new StringContent(content) };
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            return client.SendAsync(request);
        }
    }
    public class Common
    {
        public string GetAccessToken(string authority, string clientId, string secret, string serviceUrl)
        {
            string accessToken = string.Empty;
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                AuthenticationContext authContext = new AuthenticationContext(authority);
                ClientCredential credential = new ClientCredential(clientId, secret);
                AuthenticationResult result = authContext.AcquireTokenAsync(serviceUrl, credential).Result;
                accessToken = result.AccessToken;
            }
            catch (Exception ex)
            {
                accessToken = string.Empty;
                throw new Exception("Error while getting Access Token: " + ex.Message + Environment.NewLine);
            }
            return accessToken;
        }

        public async Task<string> GetSecretValue(string keyVaultUrl, string secretName)
        {
            string secret = "";
            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
            var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            //slow without ConfigureAwait(false)    
            //keyvault should be keyvault DNS Name    
            var secretBundle = await keyVaultClient.GetSecretAsync(keyVaultUrl + secretName).ConfigureAwait(false);
            secret = secretBundle.Value;
            Console.WriteLine("Secret Name:" + secretName + " Value:" + secret);
            return secret;
        }
        public Dictionary<string, string> FetchConfigurations(string accessToken, string serviceUrl, string configSetupName)
        {
            Dictionary<string, string> dcConfigs = null;
            JObject collection;
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(serviceUrl);
                    //httpClient.Timeout = new TimeSpan(0, 2, 0);
                    httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                    httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
                    httpClient.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", accessToken);

                    string fetchXmlQuery =
                                         "<fetch mapping='logical'>" +
                                         "  <entity name='ims_configuration' >" +
                                         "   <attribute name='ims_valuemultiline' />" +
                                         "    <attribute name='ims_description' />" +
                                         "    <attribute name='ims_name' />" +
                                         "    <attribute name='ims_value' />" +
                                         "    <attribute name='ims_appconfigsetup' />" +
                                         "    <attribute name='ims_valuetype' />" +
                                         "    <link-entity name='ims_appconfigsetup' from='ims_appconfigsetupid' to='ims_appconfigsetup' link-type='inner' >" +
                                         "      <filter type='and' >" +
                                         "        <condition attribute='ims_name' operator='eq' value='" + configSetupName + "' />" +
                                         "      </filter>" +
                                         "    </link-entity>" +
                                         "  </entity>" +
                                         "</fetch>";
                    //Must encode the FetchXML query because it's a part of the request (GET) string .
                    HttpResponseMessage response = httpClient.GetAsync("api/data/v9.1/ims_configurations?fetchXml=" + WebUtility.UrlEncode(fetchXmlQuery), HttpCompletionOption.ResponseHeadersRead).Result;
                    if (response.IsSuccessStatusCode) //200
                    {
                        collection = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                        JToken valArray;
                        if (collection.TryGetValue("value", out valArray))
                        {
                            JArray entities = (JArray)valArray;
                            dcConfigs = new Dictionary<string, string>();
                            foreach (JObject entity in entities)
                            {
                                string value = string.Empty;
                                if (entity.ContainsKey("ims_value"))
                                    value = entity["ims_value"].ToString();
                                if (string.IsNullOrEmpty(value) && entity.ContainsKey("ims_valuemultiline"))
                                    value = entity["ims_valuemultiline"].ToString();
                                if (entity.ContainsKey("ims_name"))
                                    dcConfigs.Add(entity["ims_name"].ToString(), value);

                            }
                        }
                    }
                    else
                    {
                        dcConfigs = null;
                        throw new Exception(response.ReasonPhrase + "|Status Code " + response.StatusCode + "|Error:" + ExtractErrorMessage(response) + Environment.NewLine);
                    }

                }
            }
            catch (Exception ex)
            {
                //log Error
                dcConfigs = null;
                throw new Exception("Error while fetching Configurations: " + ex.Message + Environment.NewLine);
            }

            return dcConfigs;
        }

        public List<ActiveActivityScheduler> FetchActiveActivitySchedlers(string authority, string clientId, string secret, string serviceUrl, string accessToken, Dictionary<string, string> dcConfigs, string fetchXml)
        {
            int pageNumber = 1;
            List<ActiveActivityScheduler> lstActiveActivitySchedlers = null;
            try
            {
                lstActiveActivitySchedlers = EnrichDetailsFromCRM(authority, clientId, secret, serviceUrl, accessToken, pageNumber, string.Empty, null, dcConfigs, fetchXml);
            }
            catch (Exception ex)
            {
                lstActiveActivitySchedlers = null;
                throw new Exception("Error while fetching Active Activity Schedlers: " + ex.Message + Environment.NewLine);
            }
            return lstActiveActivitySchedlers;
        }

        public List<ActiveActivityScheduler> EnrichDetailsFromCRM(string authority, string clientId, string secret, string serviceUrl, string accessToken, int pageNumber, string pagingCookie, List<ActiveActivityScheduler> lstActiveActivitySchedulers, Dictionary<string, string> dcConfigs, string fetchXml)
        {
            JObject collection = null;
            HttpResponseMessage response = null;
            if (lstActiveActivitySchedulers == null) lstActiveActivitySchedulers = new List<ActiveActivityScheduler>();
            accessToken = GetAccessToken(authority, clientId, serviceUrl, serviceUrl);
            var query = string.Empty;
            if (!string.IsNullOrWhiteSpace(pagingCookie))
            {
                query = string.Format(fetchXml, pageNumber, "paging-cookie='" + pagingCookie + "'");
            }
            else
            {
                query = string.Format(fetchXml, pageNumber, " ");
            }

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(serviceUrl);
                //httpClient.Timeout = new TimeSpan(0, 2, 0);
                httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
                httpClient.DefaultRequestHeaders.Add("Prefer", "odata.include-annotations = *");
                httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);

                //Must encode the FetchXML query because it's a part of the request (GET) string .
                response = httpClient.GetAsync("api/data/v9.1/ims_activityschedulers?fetchXml=" + query, HttpCompletionOption.ResponseHeadersRead).Result;
                if (response.IsSuccessStatusCode) //200
                {
                    collection = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                    JToken valArray;
                    if (collection.TryGetValue("value", out valArray))
                    {
                        JArray entities = (JArray)valArray;
                        foreach (JObject entity in entities)
                        {
                            ActiveActivityScheduler objActiveActivityScheduler = new ActiveActivityScheduler();
                            if (entity.ContainsKey("ims_activityschedulerid"))
                            {
                                objActiveActivityScheduler.ActivitySchedulerRecordId = entity["ims_activityschedulerid"].ToString();
                            }
                            if (entity.ContainsKey("_ims_marketinglist_value"))
                            {
                                objActiveActivityScheduler.MarketingListId = entity["_ims_marketinglist_value"].ToString();
                            }
                            if (entity.ContainsKey("ims_activitytype"))
                            {
                                objActiveActivityScheduler.ActivityType = (int)entity["ims_activitytype"];
                            }
                            lstActiveActivitySchedulers.Add(objActiveActivityScheduler);
                        }
                    }
                }
                else
                {
                    throw new Exception(response.ReasonPhrase + "|Status Code " + response.StatusCode + "|Error:" + ExtractErrorMessage(response));
                }
            }
            string httpResopnse = GetResponseString(response);
            if (httpResopnse.Contains("@Microsoft.Dynamics.CRM.fetchxmlpagingcookie"))
            {
                var myDtoObject = JsonConvert.DeserializeObject<GenericDto>(httpResopnse);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(myDtoObject.Cookie);
                XmlElement root = doc.DocumentElement;
                string s = root.Attributes["pagingcookie"].Value;
                pagingCookie = GetDecodedCookie(s);
                this.EnrichDetailsFromCRM(authority, clientId, secret, serviceUrl, accessToken, ++pageNumber, pagingCookie, lstActiveActivitySchedulers, dcConfigs, fetchXml);
            }
            else
                return lstActiveActivitySchedulers;

            return lstActiveActivitySchedulers;
        }

        public string GetDecodedCookie(string cookie)
        {
            StringBuilder b = new StringBuilder(cookie);
            b.Replace("%253c", "%26lt;");
            b.Replace("%2520", " ");
            b.Replace("%253d", "=");
            b.Replace("%2522", "%26quot;");
            b.Replace("%253e", "%26gt;");
            b.Replace("%253c", "%26lt;");
            b.Replace("%2520", " ");
            b.Replace("%257b", "{");
            b.Replace("%257d", "}");
            b.Replace("%252f", "/");
            return b.ToString();
        }
        public string GetResponseString(HttpResponseMessage responseMessage)
        {
            string responseString = null;

            if (responseMessage.IsSuccessStatusCode)
            {
                responseString = responseMessage.Content.ReadAsStringAsync().Result;
            }

            return responseString;
        }

        public static async Task<Guid> CreateBatchJobRecord(string authority, string clientId, string secret, string serviceUrl, string batchJobName)
        {
            Guid batchJobId = Guid.Empty;
            Common objCommon = new Common();
            try
            {
                string accessToken = objCommon.GetAccessToken(authority, clientId, secret, serviceUrl);
                //Create
                JObject newBatchJob = new JObject
                {
                    {"ims_name", batchJobName+"-"+DateTime.Now.Month+"-"+DateTime.Now.Day+"-"+DateTime.Now.Year}
                };

                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(serviceUrl);
                    httpClient.Timeout = new TimeSpan(0, 2, 0);
                    httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                    httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
                    httpClient.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", accessToken);
                    HttpResponseMessage createResponse =
                        await httpClient.SendAsJsonAsync(HttpMethod.Post, "api/data/v9.1/ims_batchjobs", newBatchJob);
                    if (createResponse.IsSuccessStatusCode)
                    {
                        string batchJobUri = createResponse.Headers.GetValues("OData-EntityId").FirstOrDefault();
                        if (batchJobUri != null)
                            batchJobId = Guid.Parse(batchJobUri.Split('(', ')')[1]);
                    }
                    else
                    {
                        batchJobId = Guid.Empty;
                        throw new Exception(createResponse.ReasonPhrase + "|Status Code " + createResponse.StatusCode + "|Error:" + objCommon.ExtractErrorMessage(createResponse));
                    }
                }
            }
            catch (Exception ex)
            {
                batchJobId = Guid.Empty;
                throw new Exception("Error while Creating Batch Job record: " + ex.Message + Environment.NewLine);
            }
            return batchJobId;
        }

        public static async Task UpdateBatchJobStatus(string authority, string clientId, string secret, string serviceUrl, Guid batchJobId, bool batchJobStatus, string batchJobContent, string batchJobName)
        {
            Common objCommon = new Common();
            try
            {
                if (batchJobId != Guid.Empty)
                {
                    string accessToken = objCommon.GetAccessToken(authority, clientId, secret, serviceUrl);
                    int statusReason = -1;
                    if (batchJobStatus)
                    {
                        statusReason = 176390000;
                    }
                    else
                    {
                        statusReason = 176390001;
                    }
                    //Create Notes
                    JObject createNotes = new JObject
                {
                    {"filename", batchJobName},
                    {"documentbody",Convert.ToBase64String(new UnicodeEncoding().GetBytes(batchJobContent)) },
                    {"objectid_ims_batchjob@odata.bind","/ims_batchjobs("+batchJobId+")"}
                };

                    //Update Batch Job Status
                    JObject updateBatchJob = new JObject
                {
                    {"statuscode", statusReason},
                    {"ims_enddate",DateTime.Now.ToUniversalTime() }
                };

                    using (HttpClient httpClient = new HttpClient())
                    {
                        httpClient.BaseAddress = new Uri(serviceUrl);
                        httpClient.Timeout = new TimeSpan(0, 2, 0);
                        httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                        httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
                        httpClient.DefaultRequestHeaders.Accept.Add(
                            new MediaTypeWithQualityHeaderValue("application/json"));
                        httpClient.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Bearer", accessToken);
                        //Create Notes
                        HttpResponseMessage createResponse =
                            await httpClient.SendAsJsonAsync(HttpMethod.Post, "api/data/v9.1/annotations", createNotes);
                        //Update Batch Job Status
                        HttpResponseMessage updateResponse = await httpClient.SendAsJsonAsync(new HttpMethod("PATCH"), "api/data/v9.1/ims_batchjobs(" + batchJobId + ")", updateBatchJob);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error while updating batch job status:" + ex.Message);
            }
        }

        public static async Task CreateActivityRequest(string authority, string clientId, string secret, string serviceUrl, Guid batchJobId, Guid activityScheduleId, Guid marketingListId, int activityType)
        {
            Common objCommon = new Common();
            try
            {
                string accessToken = objCommon.GetAccessToken(authority, clientId, secret, serviceUrl);
                JObject createActivityRequest = new JObject();
                //Create Activity Request
                string activityTypeName = string.Empty;
                string name = string.Empty;
                string marketingListName = string.Empty;

                //Fetch Category, Email, Template ID, List Name from Marketing List
                JObject mlObject = await objCommon.GetMarketingListDetails(marketingListId, accessToken, serviceUrl);

                //Get List Name
                if (mlObject != null)
                {
                    marketingListName = mlObject["listname"].ToString();
                }
                //Email
                if (activityType == 176390000)
                {
                    activityTypeName = "Email";
                    // Add Email , Category & Template ID to Activity request
                    if (mlObject != null)
                    {
                        Guid emailId = Guid.Empty;
                        Guid categoryId = Guid.Empty;
                        emailId = (Guid)mlObject["_ims_email_value"];
                        categoryId = (Guid)mlObject["_ims_category_value"];

                        if (emailId != Guid.Empty)
                        {
                            createActivityRequest.Add("ims_email@odata.bind", JToken.FromObject("/ims_aememails(" + emailId + ")"));
                        }

                        if (categoryId != Guid.Empty)
                        {
                            createActivityRequest.Add("ims_campaign@odata.bind", JToken.FromObject("/campaigns(" + categoryId + ")"));
                        }

                        createActivityRequest.Add("ims_templateid", JToken.FromObject(mlObject["ims_templateid"]));
                    }
                }
                else if (activityType == 176390001)
                {
                    activityTypeName = "SMS";
                }
                else if (activityType == 176390002)
                {
                    activityTypeName = "NBA";
                }

                name = name + Constants.activityRequest + "-" + activityTypeName + "-" + marketingListName;

                //createActivityRequest.Add("ims_batchjob@odata.bind", JToken.FromObject("/ims_batchjobs(" + batchJobId + ")"));
                createActivityRequest.Add("ims_activityscheduler@odata.bind", JToken.FromObject("/ims_activityschedulers(" + activityScheduleId + ")"));
                createActivityRequest.Add("ims_marketinglist@odata.bind", JToken.FromObject("/lists(" + marketingListId + ")"));
                createActivityRequest.Add("ims_name", JToken.FromObject(name));
                createActivityRequest.Add("ims_activitytype", JToken.FromObject(activityType));
                createActivityRequest.Add("ims_issynced", JToken.FromObject(false));

                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(serviceUrl);
                    httpClient.Timeout = new TimeSpan(0, 2, 0);
                    httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                    httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
                    httpClient.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", accessToken);
                    HttpResponseMessage createResponse =
                        await httpClient.SendAsJsonAsync(HttpMethod.Post, "api/data/v9.1/ims_acscampaignrequests", createActivityRequest);
                    if (!createResponse.IsSuccessStatusCode)
                        throw new Exception(createResponse.ReasonPhrase + "|Status Code " + createResponse.StatusCode + "|Error:" + objCommon.ExtractErrorMessage(createResponse));

                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error while Creating Activity Request record:" + ex.Message);
            }
        }

        public async Task<JObject> GetMarketingListDetails(Guid marketingListId, string accessToken, string serviceUrl)
        {
            JObject marketingListObject = null;
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(serviceUrl);
                    httpClient.Timeout = new TimeSpan(0, 2, 0);
                    httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                    httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
                    httpClient.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", accessToken);
                    HttpResponseMessage response = await httpClient.GetAsync("/api/data/v9.1/lists(" + marketingListId + ")?$select=_ims_category_value,_ims_email_value,ims_ismlupdated,ims_templateid,listname");
                    if (response.IsSuccessStatusCode) //200
                    {
                        marketingListObject = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                    }
                    else
                    {
                        throw new Exception(response.ReasonPhrase + "|Status Code " + response.StatusCode + "|Error:" + ExtractErrorMessage(response));
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error While Fetching Marketing List Details:" + ex.Message);
            }

            return marketingListObject;
        }

        public string ExtractErrorMessage(HttpResponseMessage response)
        {
            string errorMsg = string.Empty;
            JObject errorObject = JObject.Parse(response.Content.ReadAsStringAsync().Result);
            errorMsg = errorObject["error"]["message"].ToString();
            return errorMsg;
        }
    }

    public class ActiveActivityScheduler
    {
        string activitySchedulerRecordId;
        string marketingListId;
        int activityType;
        public string ActivitySchedulerRecordId
        {
            get
            {
                return activitySchedulerRecordId;
            }
            set
            {
                activitySchedulerRecordId = value;
            }
        }
        public string MarketingListId
        {
            get
            {
                return marketingListId;
            }
            set
            {
                marketingListId = value;
            }
        }
        public int ActivityType
        {
            get
            {
                return activityType;
            }
            set
            {
                activityType = value;
            }
        }
    }
    public class GenericDto
    {
        [JsonProperty("@Microsoft.Dynamics.CRM.fetchxmlpagingcookie")]
        public string Cookie { get; set; }

    }
}
