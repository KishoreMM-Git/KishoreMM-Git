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

namespace UpdateUserProfile
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
        public List<USer> FetchActiveUser(string accessToken, string serviceUrl)
        {
            List<USer> lstactiveuser = null;
           
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

                    string fetchXmlQuery = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
                                            "  <entity name='systemuser'>" +
                                            "    <attribute name='fullname' />" +
                                            "    <attribute name='domainname' />" +
                                            "    <attribute name='firstname' />" +
                                            "    <attribute name='lastname' />" +
                                            "    <attribute name='businessunitid' />" +
                                            "    <attribute name='title' />" +
                                            "    <attribute name='address1_telephone1' />" +
                                            "    <attribute name='positionid' />" +
                                            "    <attribute name='systemuserid' />" +
                                            "    <attribute name='ims_nmlsnumber' />" +
                                            "    <attribute name='ims_slug' />" +
                                            "    <order attribute='fullname' descending='false' />" +
                                            "    <filter type='and'>" +
                                            //"      <condition attribute='ims_nmlsnumber' operator='not-null' />" +
                                            "      <condition attribute='firstname' operator='not-null' />" +
                                            "      <condition attribute='lastname' operator='not-null' />" +
                                            "      <condition attribute='isdisabled' operator='eq' value='0' />" +
                                            "      <condition attribute='accessmode' operator='ne' value='4' />" +
      //                                      "<condition attribute='systemuserid' operator='in'>" +
      //  "<value uiname='Christopher Haynes' uitype='systemuser'>{DAAA0056-2C6F-EA11-A811-000D3A30F12C}</value>" +
      //  "<value uiname='Cam Lawler' uitype='systemuser'>{A72A29CC-2A6F-EA11-A811-000D3A30F12C}</value>" +
      //  "<value uiname='David Perras' uitype='systemuser'>{FB3B6874-C26A-EB11-B0B0-000D3A33F6CA}</value>" +
      //"</condition>" +
                                            "    </filter>" +
                                            "  </entity>" +
                                            "</fetch>";

                    //Must encode the FetchXML query because it's a part of the request (GET) string .
                    HttpResponseMessage response = httpClient.GetAsync("api/data/v9.1/systemusers?fetchXml=" + WebUtility.UrlEncode(fetchXmlQuery), HttpCompletionOption.ResponseHeadersRead).Result;
                    if (response.IsSuccessStatusCode) //200
                    {
                        collection = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                        JToken valArray;
                        if (collection.TryGetValue("value", out valArray))
                        {
                            JArray entities = (JArray)valArray;

                            lstactiveuser = new List<USer>();
                             foreach (JObject entity in entities)
                            {
                                USer objActiveUser = new USer();
                                if (entity.ContainsKey("ims_nmlsnumber"))
                                {
                                    objActiveUser.NMLS = entity["ims_nmlsnumber"].ToString();
                                }
                                if (entity.ContainsKey("systemuserid"))
                                {
                                    objActiveUser.USERID = entity["systemuserid"].ToString();
                                }
                                if (entity.ContainsKey("firstname"))
                                {
                                    objActiveUser.FirstName = entity["firstname"].ToString();
                                }
                                if (entity.ContainsKey("lastname"))
                                {
                                    objActiveUser.LastName = entity["lastname"].ToString();
                                }
                                if (entity.ContainsKey("domainname"))
                                {
                                    objActiveUser.DomainName = entity["domainname"].ToString();
                                }
                                if (entity.ContainsKey("ims_slug"))
                                {
                                    objActiveUser.Slug = entity["ims_slug"].ToString();
                                }

                                lstactiveuser.Add(objActiveUser);
                            }
                        }
                        

                    }
                    else
                    {
                        lstactiveuser = null;
                        throw new Exception("FetchActiveUser "+ response.ReasonPhrase + "Status Code " + response.StatusCode + "|Error:" + ExtractErrorMessage(response) + Environment.NewLine  );
                    }

                }
            }
            catch (Exception ex)
            {
                //log Error
                lstactiveuser = null;
                throw new Exception("Error while FetchActiveUser : " + ex.Message + Environment.NewLine);
            }

            return lstactiveuser;
        }
        public bool fetchtestimonial(string accessToken, string serviceUrl, string testimonialname,Guid Userid)
        {
            bool recdexist = false;
            JObject collection;
            try {
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
                    var fetchtestimonial = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
                                    "  <entity name='ims_testimonial'>" +
                                    "    <attribute name='ims_testimonialid' />" +
                                    "    <attribute name='ims_name' />" +
                                    "    <attribute name='createdon' />" +
                                    "    <order attribute='ims_name' descending='false' />" +
                                    "   <filter type='and'>" +
                                    "   <condition attribute='ims_user' operator='eq' uitype='systemuser' value='" + Userid + "'/>" +
                                    "      <condition attribute='ims_name' operator='eq' value='" + testimonialname + "' />" +
                                    "    </filter>" +
                                    "    </entity>" +
                                    "     </fetch>";

                    HttpResponseMessage response = httpClient.GetAsync("api/data/v9.1/ims_testimonials?fetchXml=" + WebUtility.UrlEncode(fetchtestimonial), HttpCompletionOption.ResponseHeadersRead).Result;
                    if (response.IsSuccessStatusCode) //200
                    {
                        collection = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                        if(collection.Count >0)
                        {
                            JToken valArray;
                            if (collection.TryGetValue("value", out valArray))
                            {
                                JArray entities = (JArray)valArray;
                                if(entities.Count >0)
                                {
                                    recdexist = true;
                                }                                
                            }
                        }
                    }
                    else
                    {
                        //lstactiveuser = null;
                        throw new Exception("fetchtestimonial" + response.ReasonPhrase + "Status Code " + response.StatusCode  +"|Error:" + ExtractErrorMessage(response) + Environment.NewLine);
                    }
                }
              }
            catch(Exception ex)
            {
                throw new Exception("Error while fetchtestimonial : " + ex.Message + Environment.NewLine);
            }
             return recdexist;
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
                        throw new Exception(response.ReasonPhrase + "Status Code " + response.StatusCode + "|Error:" + ExtractErrorMessage(response) + Environment.NewLine);
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
                        throw new Exception(createResponse.ReasonPhrase + "Status Code " + createResponse.StatusCode + "|Error:" + objCommon.ExtractErrorMessage(createResponse));
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
        public async static Task<List<JObject>> callnmlswebservice(string url)
        {
            List<JObject> data = null;
            try
            {
                using (HttpClient client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true }))
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {

                        string resonseData = await response.Content.ReadAsStringAsync();
                        data = JsonConvert.DeserializeObject<List<JObject>>(resonseData);
                    }
                    else
                    {
                        //batchJobId = Guid.Empty;
                        throw new Exception("callnmlswebservice" + response.ReasonPhrase + "Status Code " + response.StatusCode);
                    }
                }

            }
            catch(Exception ex)
            {
                throw new Exception("callnmlswebservice" + ex.Message + " " + ex.InnerException);
            }

           return data;
            
        }
        public string ExtractErrorMessage(HttpResponseMessage response)
        {
            string errorMsg = string.Empty;
            JObject errorObject = JObject.Parse(response.Content.ReadAsStringAsync().Result);
            errorMsg = errorObject["error"]["message"].ToString();
            return errorMsg;
        }
        public async static Task<string> GetNMLSData(string url)
        {
            using (HttpClient client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true }))
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpResponseMessage response = await client.GetAsync(url);
                return await response.Content.ReadAsStringAsync();
            }
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
    }

    public class USer
    {
        string  nmls;
        string userid;
        public string NMLS
        {
            get
            {
                return nmls;
            }
            set
            {
                nmls = value;
            }
        }
       
        public string USERID
        {
            get
            {
                return userid;
            }
            set
            {
                userid = value;
            }
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DomainName { get; set; }
        public string Slug { get; set; }
    }
    public class GenericDto
    {
        [JsonProperty("@Microsoft.Dynamics.CRM.fetchxmlpagingcookie")]
        public string Cookie { get; set; }

    }

    namespace nmlswebservice
    {
        public class Rootobject
        {
            public Class1[] Property1 { get; set; }
        }

        public class Class1
        {
            public int id { get; set; }
            public DateTime date { get; set; }
            public DateTime date_gmt { get; set; }
            public Guid guid { get; set; }
            public DateTime modified { get; set; }
            public DateTime modified_gmt { get; set; }
            public string slug { get; set; }
            public string status { get; set; }
            public string type { get; set; }
            public string link { get; set; }
            public Title title { get; set; }
            public Content content { get; set; }
            public Excerpt excerpt { get; set; }
            public int author { get; set; }
            public int featured_media { get; set; }
            public int menu_order { get; set; }
            public string template { get; set; }
            public object[] meta { get; set; }
            public Acf acf { get; set; }
            public _Links _links { get; set; }
        }

        public class Guid
        {
            public string rendered { get; set; }
        }

        public class Title
        {
            public string rendered { get; set; }
        }

        public class Content
        {
            public string rendered { get; set; }
            public bool _protected { get; set; }
        }

        public class Excerpt
        {
            public string rendered { get; set; }
            public bool _protected { get; set; }
        }

        public class Acf
        {
            public string lo_crm { get; set; }
            public string point_of_sale { get; set; }
            public string leadpops { get; set; }
            public string personal_greeting { get; set; }
            public string cover_bg { get; set; }
            public string lotitle { get; set; }
            public string email_address { get; set; }
            public object loan_officer_image { get; set; }
            public string office_phone_number { get; set; }
            public string cell_phone_number { get; set; }
            public string fax_number { get; set; }
            public string nmls_id { get; set; }
            public string lic_info { get; set; }
            public string state_disclosure { get; set; }
            public string apply_online_url_blend { get; set; }
            public string apply_video_url { get; set; }
            public string facebook_url { get; set; }
            public string linkedin_url { get; set; }
            public string twitter_url { get; set; }
            public string zillow_url { get; set; }
            public string testimonial_1 { get; set; }
            public string testimonial_1_name { get; set; }
            public string testimonial_2 { get; set; }
            public string testimonial_2_name { get; set; }
            public string testimonial_3 { get; set; }
            public string testimonial_3_name { get; set; }
            public string facebook_pixel { get; set; }
            public string personal_slogan { get; set; }
            public string custom_bio { get; set; }
            public string meta_keywords { get; set; }
            public bool fluent_in_spanish { get; set; }
            public string about_me_video_url { get; set; }
            public string about_me_bio { get; set; }
        }

        public class _Links
        {
            public Self[] self { get; set; }
            public Collection[] collection { get; set; }
            public About[] about { get; set; }
            public Author[] author { get; set; }
            public VersionHistory[] versionhistory { get; set; }
            public PredecessorVersion[] predecessorversion { get; set; }
            public WpAttachment[] wpattachment { get; set; }
            public Cury[] curies { get; set; }
        }

        public class Self
        {
            public string href { get; set; }
        }

        public class Collection
        {
            public string href { get; set; }
        }

        public class About
        {
            public string href { get; set; }
        }

        public class Author
        {
            public bool embeddable { get; set; }
            public string href { get; set; }
        }

        public class VersionHistory
        {
            public int count { get; set; }
            public string href { get; set; }
        }

        public class PredecessorVersion
        {
            public int id { get; set; }
            public string href { get; set; }
        }

        public class WpAttachment
        {
            public string href { get; set; }
        }

        public class Cury
        {
            public string name { get; set; }
            public string href { get; set; }
            public bool templated { get; set; }
        }
    }
}
