using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Infy.MS.NextBestActionActivity
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
                        JObject errorJobject = JObject.Parse(createResponse.Content.ReadAsStringAsync().Result);
                        throw new Exception("Error in creating Notes:" + errorJobject);
                        //var erroMsg = ExtractErrorMessage(createResponse);
                        //throw new Exception("Error in Retrieving Activity Requests :" + erroMsg + "");

                       // throw new Exception(createResponse.ReasonPhrase + "Status Code " + createResponse.StatusCode);
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
        public string GetAccessToken(string authority, string clientId, string secret, string serviceUrl)
        {
            string accessToken = string.Empty;
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultNetworkCredentials;
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
                        throw new Exception(response.ReasonPhrase + "Status Code " + response.StatusCode + Environment.NewLine);
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

        internal async Task CreateFailedRecordAsync(JObject annotationObject, string authority, string accessToken, string serviceUrl, string clientId, string secret)
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
                    new AuthenticationHeaderValue("Bearer", GetAccessToken(authority, clientId, secret, serviceUrl));
                HttpResponseMessage createResponse =
                    await httpClient.SendAsJsonAsync(HttpMethod.Post, "api/data/v9.1/annotations", annotationObject);
                if (createResponse.IsSuccessStatusCode)
                {
                    string batchJobUri = createResponse.Headers.GetValues("OData-EntityId").FirstOrDefault();
                }
                else
                {
                    JObject errorJobject = JObject.Parse(createResponse.Content.ReadAsStringAsync().Result);
                    throw new Exception("Error in creating Notes:"+ errorJobject);
                }
            }
        }

        public List<ActivityRequestResponse> GetActivityRequests(string accessToken, string serviceUrl, string clientId, string secret, string fetchXml)
        {
            List<ActivityRequestResponse> activityRequestResponses = new List<ActivityRequestResponse>();

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(serviceUrl);
                //httpClient.Timeout = new TimeSpan(0, 2, 0);
                httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                //Must encode the FetchXML query because it's a part of the request (GET) string .
                HttpResponseMessage response = httpClient.GetAsync("api/data/v9.1/ims_acscampaignrequests?fetchXml=" + WebUtility.UrlEncode(fetchXml), HttpCompletionOption.ResponseHeadersRead).Result;
                if (response.IsSuccessStatusCode) //200
                {
                    JObject collection = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                    JToken valArray;
                    if (collection.TryGetValue("value", out valArray))
                    {

                        JArray entities = (JArray)valArray;
                        foreach (JObject entity in entities)
                        {
                            ActivityRequestResponse activityRequestResponse = new ActivityRequestResponse();

                            if (entity.ContainsKey("ims_acscampaignrequestid"))
                            {
                                activityRequestResponse.activityRequestId = entity["ims_acscampaignrequestid"].ToString();
                            }
                            if (entity.ContainsKey("ims_name"))
                            {
                                activityRequestResponse.activityRequestName = entity["ims_name"].ToString();
                            }
                            if (entity.ContainsKey("a_list.listid"))
                            {
                                activityRequestResponse.marketingListGuid = entity["a_list.listid"].ToString();
                            }
                            if (entity.ContainsKey("a_list.listname"))
                            {
                                activityRequestResponse.marketingListName = entity["a_list.listname"].ToString();
                            }
                            if (entity.ContainsKey("a_list.type"))
                            {
                                activityRequestResponse.marketingListType = entity["a_list.type"].ToString();
                            }
                            if (entity.ContainsKey("a_list.createdfromcode"))
                            {
                                activityRequestResponse.marketingMemberType = entity["a_list.createdfromcode"].ToString();
                            }
                            if (entity.ContainsKey("a_activityscheduler.ims_nbatext"))
                            {
                                activityRequestResponse.nbaText = entity["a_activityscheduler.ims_nbatext"].ToString();
                            }
                            if (entity.ContainsKey("a_activityscheduler.ims_nbasla"))
                            {
                                activityRequestResponse.nbaSla = entity["a_activityscheduler.ims_nbasla"].ToString();
                            }
                            if (entity.ContainsKey("a_activityscheduler.ims_name"))
                            {
                                activityRequestResponse.activitySchedulerName = entity["a_activityscheduler.ims_name"].ToString();
                            }
                            activityRequestResponses.Add(activityRequestResponse);
                        }
                    }
                    return activityRequestResponses;
                }
                else
                {
                    var erroMsg = ExtractErrorMessage(response);
                    throw new Exception("Error in Retrieving Activity Requests :" + erroMsg + "");
                }
            }
        }
        public int CheckStaticMLSynchronization(string accessToken, string serviceUrl, string clientId, string secret, string fetchXml)
        {
            int recordCount = 0;
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(serviceUrl);
                    //httpClient.Timeout = new TimeSpan(0, 2, 0);
                    httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                    httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    //Must encode the FetchXML query because it's a part of the request (GET) string .
                    HttpResponseMessage response = httpClient.GetAsync("api/data/v9.1/ims_acscampaignrequests?fetchXml=" + WebUtility.UrlEncode(fetchXml), HttpCompletionOption.ResponseHeadersRead).Result;
                    if (response.IsSuccessStatusCode) //200
                    {
                        JObject collection = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                        JToken valArray;
                        if (collection.TryGetValue("value", out valArray))
                        {
                            JArray entities = (JArray)valArray;
                            foreach (JObject entity in entities)
                            {
                                if (entities.Count > 0)
                                {
                                    return recordCount + 1;
                                }
                            }
                        }
                        return recordCount;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return recordCount;
        }
        public List<LeadRequestResponse> GetLeads(int pageNumber,string pagingCookie,string accessToken, string serviceUrl, string clientId, string secret, string fetchXml)
        {
            List<LeadRequestResponse> leadsRequestResponses = new List<LeadRequestResponse>();
            HttpResponseMessage response = new HttpResponseMessage();
            //if (pageNumber == 1)
            //{
            //    leadsRequestResponses = new List<LeadRequestResponse>();
            //}

            GetDynamicLeads(ref leadsRequestResponses, pageNumber, pagingCookie, accessToken, serviceUrl, clientId, secret, fetchXml,fetchXml);
            
           
            //if (!string.IsNullOrWhiteSpace(pagingCookie))
            //{
            //    fetchXml = string.Format(fetchXml, pageNumber, "paging-cookie='" + pagingCookie + "'");
            //}
            //else
            //{
            //    fetchXml = string.Format(fetchXml, pageNumber, "");
            //}


            //using (HttpClient httpClient = new HttpClient())
            //{

            //    httpClient.BaseAddress = new Uri(serviceUrl);
            //    //httpClient.Timeout = new TimeSpan(0, 2, 0);
            //    httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
            //    httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
            //    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            //    httpClient.DefaultRequestHeaders.Add("Prefer", "odata.include-annotations=\"*\",odata.maxpagesize=5000");
            //    //httpClient.DefaultRequestHeaders.Add("pagingcookie", pagingCookie);
            //    //httpClient.DefaultRequestHeaders.Add("page", "2");

            //    //Must encode the FetchXML query because it's a part of the request (GET) string .

            //    response = httpClient.GetAsync("api/data/v9.1/leads?fetchXml=" + WebUtility.UrlEncode(fetchXml), HttpCompletionOption.ResponseHeadersRead).Result;
            //    if (response.IsSuccessStatusCode) //200
            //    {
            //        JObject collection = JObject.Parse(response.Content.ReadAsStringAsync().Result);
            //        JToken valArray;
            //        if (collection.TryGetValue("value", out valArray))
            //        {
            //            JArray entities = (JArray)valArray;
            //            foreach (JObject entity in entities)
            //            {
            //                LeadRequestResponse leadRequestResponse = new LeadRequestResponse();
            //                if (entity.ContainsKey("leadid"))
            //                {
            //                    leadRequestResponse.leadId = entity["leadid"].ToString();
            //                }
            //                if (entity.ContainsKey("_owninguser_value"))
            //                {
            //                    leadRequestResponse.owningUser = entity["_owninguser_value"].ToString();
            //                }
            //                if (entity.ContainsKey("_ownerid_value"))
            //                {
            //                    leadRequestResponse.ownerId = entity["_ownerid_value"].ToString();
            //                }
            //                if (entity.ContainsKey("_owningteam_value"))
            //                {
            //                    leadRequestResponse.owningTeam = entity["_owningteam_value"].ToString();
            //                }
            //                leadsRequestResponses.Add(leadRequestResponse);
            //            }
            //        }
            //        else
            //        {
            //            var erroMsg = ExtractErrorMessage(response);
            //            throw new Exception("Error in Retrieving Leads :" + erroMsg + "");
            //        }
            //    }

            //}
            //string httpResopnse = GetResponseString(response);
            //if (httpResopnse.Contains("@Microsoft.Dynamics.CRM.fetchxmlpagingcookie"))
            //{
            //    var myDtoObject = JsonConvert.DeserializeObject<GenericDto>(httpResopnse);
            //    XmlDocument doc = new XmlDocument();
            //    doc.LoadXml(myDtoObject.Cookie);
            //    XmlElement root = doc.DocumentElement;
            //    string s = root.Attributes["pagingcookie"].Value;
            //    pagingCookie = GetDecodedCookie(s);
            //    pageNumber = pageNumber + 1;
            //    this.GetLeads(pageNumber,pagingCookie, accessToken, serviceUrl,secret,clientId, fetchXml);
            //}
            return leadsRequestResponses;
        }

        public List<ContactRequestResponse> GetContacts(int pageNumber, string pagingCookie, string accessToken, string serviceUrl, string clientId, string secret, string fetchXml)
        {
            List<ContactRequestResponse> contactsRequestResponses = new List<ContactRequestResponse>();
            HttpResponseMessage response = new HttpResponseMessage();
            GetDynamicContacts(ref contactsRequestResponses, pageNumber, pagingCookie, accessToken, serviceUrl, clientId, secret, fetchXml, fetchXml);
            return contactsRequestResponses;
        }
        public void GetDynamicLeads(ref List<LeadRequestResponse> leadsRequestResponses, int pageNumber, string pagingCookie, string accessToken, string serviceUrl, string clientId, string secret, string fetchXml,string masterFetchXml)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            using (HttpClient httpClient = new HttpClient())
            {
                string xml = string.Empty;
                if (pageNumber == 1)
                {
                    xml = CreateXml(fetchXml, pagingCookie, pageNumber, 5000);
                }
                else
                    xml = fetchXml;

                if (!string.IsNullOrWhiteSpace(pagingCookie))
                {
                    fetchXml = fetchXml.Replace("paging-cookie", "paging-cookie='" + pagingCookie + "'");
                }
                //else
                //{
                //    fetchXml = string.Format(fetchXml, pageNumber, "");
                //}

                httpClient.BaseAddress = new Uri(serviceUrl);
                //httpClient.Timeout = new TimeSpan(0, 2, 0);
                httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                //httpClient.DefaultRequestHeaders.Add("Prefer", "odata.include-annotations=\"*\",odata.maxpagesize=5000");
                //httpClient.DefaultRequestHeaders.Add("pagingcookie", pagingCookie);
                //httpClient.DefaultRequestHeaders.Add("page", "2");

                //Must encode the FetchXML query because it's a part of the request (GET) string .

                response = httpClient.GetAsync("api/data/v9.1/leads?fetchXml=" + WebUtility.UrlEncode(xml), HttpCompletionOption.ResponseHeadersRead).Result;
                if (response.IsSuccessStatusCode) //200
                {
                    JObject collection = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                    JToken valArray;
                    if (collection.TryGetValue("value", out valArray))
                    {
                        JArray entities = (JArray)valArray;
                        foreach (JObject entity in entities)
                        {
                            LeadRequestResponse leadRequestResponse = new LeadRequestResponse();
                            if (entity.ContainsKey("leadid"))
                            {
                                leadRequestResponse.leadId = entity["leadid"].ToString();
                            }
                            if (entity.ContainsKey("_owninguser_value"))
                            {
                                leadRequestResponse.owningUser = entity["_owninguser_value"].ToString();
                            }
                            if (entity.ContainsKey("_ownerid_value"))
                            {
                                leadRequestResponse.ownerId = entity["_ownerid_value"].ToString();
                            }
                            if (entity.ContainsKey("_owningteam_value"))
                            {
                                leadRequestResponse.owningTeam = entity["_owningteam_value"].ToString();
                            }
                            leadsRequestResponses.Add(leadRequestResponse);
                        }
                    }
                    else
                    {
                        var erroMsg = ExtractErrorMessage(response);
                        throw new Exception("Error in Retrieving Leads :" + erroMsg + "");
                    }
                }

                else
                {
                    var erroMsg = ExtractErrorMessage(response);
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

                StringReader stringReader = new StringReader(masterFetchXml);
                XmlTextReader reader = new XmlTextReader(stringReader);
                XmlDocument doc1 = new XmlDocument();
                doc1.Load(reader);

                pagingCookie = GetDecodedCookie(s);
                pageNumber = pageNumber + 1;
                string xml = CreateXml(doc1, pagingCookie, pageNumber, 5000);
                this.GetDynamicLeads(ref leadsRequestResponses, pageNumber, pagingCookie, accessToken, serviceUrl, secret, clientId, xml,masterFetchXml);//,CreateXml(fetchXml,pagingCookie,pageNumber,5000));
            }
        }
        public void GetDynamicContacts(ref List<ContactRequestResponse> contactRequestResponses, int pageNumber, string pagingCookie, string accessToken, string serviceUrl, string clientId, string secret, string fetchXml, string masterFetchXml)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            using (HttpClient httpClient = new HttpClient())
            {
                string xml = string.Empty;
                if (pageNumber == 1)
                {
                    xml = CreateXml(fetchXml, pagingCookie, pageNumber, 5000);
                }
                else
                    xml = fetchXml;

                if (!string.IsNullOrWhiteSpace(pagingCookie))
                {
                    fetchXml = fetchXml.Replace("paging-cookie", "paging-cookie='" + pagingCookie + "'");
                }

                httpClient.BaseAddress = new Uri(serviceUrl);
                httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                response = httpClient.GetAsync("api/data/v9.1/contacts?fetchXml=" + WebUtility.UrlEncode(xml), HttpCompletionOption.ResponseHeadersRead).Result;
                if (response.IsSuccessStatusCode) //200
                {
                    JObject collection = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                    JToken valArray;
                    if (collection.TryGetValue("value", out valArray))
                    {
                        JArray entities = (JArray)valArray;
                        foreach (JObject entity in entities)
                        {
                            ContactRequestResponse contactRequestResponse = new ContactRequestResponse();
                            if (entity.ContainsKey("contactid"))
                            {
                                contactRequestResponse.contactId = entity["contactid"].ToString();
                            }
                            if (entity.ContainsKey("_owninguser_value"))
                            {
                                contactRequestResponse.owningUser = entity["_owninguser_value"].ToString();
                            }
                            if (entity.ContainsKey("_ownerid_value"))
                            {
                                contactRequestResponse.ownerId = entity["_ownerid_value"].ToString();
                            }
                            if (entity.ContainsKey("_owningteam_value"))
                            {
                                contactRequestResponse.owningTeam = entity["_owningteam_value"].ToString();
                            }
                            contactRequestResponses.Add(contactRequestResponse);
                        }
                    }
                    else
                    {
                        var erroMsg = ExtractErrorMessage(response);
                        throw new Exception("Error in Retrieving Contacts :" + erroMsg + "");
                    }
                }

                else
                {
                    var erroMsg = ExtractErrorMessage(response);
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

                StringReader stringReader = new StringReader(masterFetchXml);
                XmlTextReader reader = new XmlTextReader(stringReader);
                XmlDocument doc1 = new XmlDocument();
                doc1.Load(reader);

                pagingCookie = GetDecodedCookie(s);
                pageNumber = pageNumber + 1;
                string xml = CreateXml(doc1, pagingCookie, pageNumber, 5000);
                this.GetDynamicContacts(ref contactRequestResponses, pageNumber, pagingCookie, accessToken, serviceUrl, secret, clientId, xml, masterFetchXml);//,CreateXml(fetchXml,pagingCookie,pageNumber,5000));
            }
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
            b.Replace("%2520", "");
            b.Replace("%257b", "{");
            b.Replace("%257d", "}");
            b.Replace("%252f", "/");
            b.Replace("/ &amp;/ g", "&amp;amp;");
            return b.ToString();
        }
        public string GetDecodedCookies(string cookie)
        {
            StringBuilder b = new StringBuilder(cookie);
            b.Replace("%253c", "&lt;");
            b.Replace("%2520", " ");
            b.Replace("%253d", "=");
            b.Replace("%2522", "&quot;");
            b.Replace("%253e", "&gt;");
            b.Replace("%253c", "&lt;");
            b.Replace("%2520", " ");
            b.Replace("%257b", "{");
            b.Replace("%257d", "}");
            b.Replace("%252f", "/");
            b.Replace("/&amp;/g", "&amp;amp;");
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
        public DynamicMarketingListQueryRequest GetDynamicMarketingListQuery(string accessToken, string serviceUrl, string clientId, string secret, string dynamicMarketingListGuid)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            DynamicMarketingListQueryRequest dynamicMarketingListQueryRequest = new DynamicMarketingListQueryRequest();
            using (HttpClient httpClient = new HttpClient())
            {
                
                    httpClient.BaseAddress = new Uri(serviceUrl);
                    //httpClient.Timeout = new TimeSpan(0, 2, 0);
                    httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                    httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    //Must encode the FetchXML query because it's a part of the request (GET) string .
                    response = httpClient.GetAsync("api/data/v9.1/lists?$filter=listid eq " + dynamicMarketingListGuid, HttpCompletionOption.ResponseHeadersRead).Result;
                    if (response.IsSuccessStatusCode) //200
                    {
                        JObject collection = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                        JToken valArray;
                        if (collection.TryGetValue("value", out valArray))
                        {
                            JArray entities = (JArray)valArray;
                            foreach (JObject entity in entities)
                            {
                                if (entity.ContainsKey("query"))
                                {
                                    dynamicMarketingListQueryRequest.query = entity["query"].ToString();
                                }
                            }
                        }
                    }
                    else
                    {
                        var erroMsg = ExtractErrorMessage(response);
                        throw new Exception("Error in Retrieving Marketing Lists :" + erroMsg + "");
                    }
                
            }
            return dynamicMarketingListQueryRequest;
        }

        public LeadRequestResponse GetLeadDetails(string accessToken, string serviceUrl, string clientId, string secret, string leadId)
        {
            LeadRequestResponse leadRequestResponse = new LeadRequestResponse();
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(serviceUrl);
                //httpClient.Timeout = new TimeSpan(0, 2, 0);
                httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                //Must encode the FetchXML query because it's a part of the request (GET) string .
                HttpResponseMessage response = httpClient.GetAsync("api/data/v9.1/leads?$select=fullname,mobilephone,telephone1,statecode,_ownerid_value,_owninguser_value,_owningteam_value&$filter=leadid eq " + leadId, HttpCompletionOption.ResponseHeadersRead).Result;
                if (response.IsSuccessStatusCode) //200
                {
                    JObject collection = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                    JToken valArray;
                    if (collection.TryGetValue("value", out valArray))
                    {
                        JArray entities = (JArray)valArray;
                        foreach (JObject entity in entities)
                        {
                            if (entity.ContainsKey("mobilephone"))
                            {
                                leadRequestResponse.cellPhone = entity["mobilephone"].ToString();
                            }
                            if (entity.ContainsKey("fullname"))
                            {
                                leadRequestResponse.leadName = entity["fullname"].ToString();
                            }
                            if (entity.ContainsKey("otherphone1"))
                            {
                                leadRequestResponse.otherPhone = entity["otherphone1"].ToString();
                            }
                            if (entity.ContainsKey("statecode"))
                            {
                                leadRequestResponse.stateCode = entity["statecode"].ToString();
                            }
                            if (entity.ContainsKey("_ownerid_value"))
                            {
                                leadRequestResponse.ownerId = entity["_ownerid_value"].ToString();
                            }
                            if (entity.ContainsKey("_owninguser_value"))
                            {
                                leadRequestResponse.owningUser = entity["_owninguser_value"].ToString();
                            }
                            if (entity.ContainsKey("_owningteam_value"))
                            {
                                leadRequestResponse.owningTeam = entity["_owningteam_value"].ToString();
                            }
                        }
                    }
                }
                else
                {
                    var erroMsg = ExtractErrorMessage(response);
                    throw new Exception("Error in Retrieving Leads :" + erroMsg + "");
                }
            }
            return leadRequestResponse;
        }
        public ContactRequestResponse GetContactDetails(string accessToken, string serviceUrl, string clientId, string secret, string contactId)
        {
            ContactRequestResponse contactRequestResponse = new ContactRequestResponse();
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(serviceUrl);
                //httpClient.Timeout = new TimeSpan(0, 2, 0);
                httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                //Must encode the FetchXML query because it's a part of the request (GET) string .
                HttpResponseMessage response = httpClient.GetAsync("api/data/v9.1/contacts?$select=fullname,mobilephone,telephone1,statecode,_ownerid_value,_owninguser_value,_owningteam_value&$filter=contactid eq " + contactId, HttpCompletionOption.ResponseHeadersRead).Result;
                if (response.IsSuccessStatusCode) //200
                {
                    JObject collection = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                    JToken valArray;
                    if (collection.TryGetValue("value", out valArray))
                    {
                        JArray entities = (JArray)valArray;
                        foreach (JObject entity in entities)
                        {
                            if (entity.ContainsKey("mobilephone"))
                            {
                                contactRequestResponse.cellPhone = entity["mobilephone"].ToString();
                            }
                            if (entity.ContainsKey("fullname"))
                            {
                                contactRequestResponse.contactName = entity["fullname"].ToString();
                            }
                            if (entity.ContainsKey("otherphone1"))
                            {
                                contactRequestResponse.otherPhone = entity["otherphone1"].ToString();
                            }
                            if (entity.ContainsKey("statecode"))
                            {
                                contactRequestResponse.stateCode = entity["statecode"].ToString();
                            }
                            if (entity.ContainsKey("_ownerid_value"))
                            {
                                contactRequestResponse.ownerId = entity["_ownerid_value"].ToString();
                            }
                            if (entity.ContainsKey("_owninguser_value"))
                            {
                                contactRequestResponse.owningUser = entity["_owninguser_value"].ToString();
                            }
                            if (entity.ContainsKey("_owningteam_value"))
                            {
                                contactRequestResponse.owningTeam = entity["_owningteam_value"].ToString();
                            }
                        }
                    }
                }
                else
                {
                    var erroMsg = ExtractErrorMessage(response);
                    throw new Exception("Error in Retrieving Contacts :" + erroMsg + "");
                }
            }
            return contactRequestResponse;
        }
        public async Task CreateNbaAsync(JObject nbaObject, string authority, string accessToken, string serviceUrl, string clientId, string secret)
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
                    new AuthenticationHeaderValue("Bearer", GetAccessToken(authority, clientId, secret, serviceUrl));
                HttpResponseMessage createResponse =
                    await httpClient.SendAsJsonAsync(HttpMethod.Post, "api/data/v9.1/ims_nextbestactions", nbaObject);
                if (createResponse.IsSuccessStatusCode)
                {
                    string batchJobUri = createResponse.Headers.GetValues("OData-EntityId").FirstOrDefault();
                }
                else
                {
                    var erroMsg = ExtractErrorMessage(createResponse);
                    throw new Exception("Error in Checking NBA Activity:" + erroMsg + "");

                }
            }
        }

        public NBAActivity CheckNbaActivityExistance(string authority,string serviceUrl, string clientId, string secret,string api)
        {
            NBAActivity nBAActivity = new NBAActivity();
            bool isActivityExisted = false;
            using (HttpClient httpClient = new HttpClient())
            {

                httpClient.BaseAddress = new Uri(serviceUrl);
                httpClient.Timeout = new TimeSpan(0, 2, 0);
                httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
                httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", GetAccessToken(authority, clientId, secret, serviceUrl));
                HttpResponseMessage createResponse =
                     httpClient.GetAsync("api/data/v9.1/ims_nextbestactions"+ api +"",HttpCompletionOption.ResponseHeadersRead).Result;
                if (createResponse.IsSuccessStatusCode)
                {
                    JObject collection = JObject.Parse(createResponse.Content.ReadAsStringAsync().Result);
                    JToken valArray;
                    if (collection.TryGetValue("value", out valArray))
                    {
                        JArray entities = (JArray)valArray;
                        foreach (JObject entity in entities)
                        {
                            if(entity.ContainsKey("activityid"))
                            {
                                nBAActivity.nbaActivityId = entity["activityid"].ToString();
                                nBAActivity.recordExisted = true;
                            }
                        }
                        
                    }
                    return nBAActivity;
                }
                else
                {
                    var erroMsg = ExtractErrorMessage(createResponse);
                    throw new Exception("Error in Checking NBA Activity:" + erroMsg + "");
                }

            }
        }
        public static async Task UpdateBatchJobStatusAsync(string authority, string clientId, string secret, string serviceUrl, Guid batchJobRecordId, bool batcjJobStatus, int statuReasonValue,string batchJobLog, string batchJobName)
        {
            Common objCommon = new Common();
            try
            {
                if (batchJobRecordId != Guid.Empty)
                {
                    string accessToken = objCommon.GetAccessToken(authority, clientId, secret, serviceUrl);
                    int statusReason = -1;
                    if (statuReasonValue!=-1)
                    {
                        statusReason = statuReasonValue;
                    }
                   
                    //Create Notes
                    JObject createNotes = new JObject
                {
                    {"filename", batchJobName},
                    {"documentbody",Convert.ToBase64String(new UnicodeEncoding().GetBytes(batchJobLog)) },
                    {"objectid_ims_batchjob@odata.bind","/ims_batchjobs("+batchJobRecordId+")"}
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
                        HttpResponseMessage updateResponse = await httpClient.SendAsJsonAsync(new HttpMethod("PATCH"), "api/data/v9.1/ims_batchjobs(" + batchJobRecordId + ")", updateBatchJob);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error while updating batch job status:" + ex.Message);
            }
        }
        public async Task UpdateActivityRequest(Guid activityRequestId,JObject activityRequestObject, string authority, string accessToken, string serviceUrl, string clientId, string secret)
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
                    new AuthenticationHeaderValue("Bearer", GetAccessToken(authority, clientId, secret, serviceUrl));
                HttpResponseMessage createResponse =
                    await httpClient.SendAsJsonAsync(new HttpMethod("PATCH"), "api/data/v9.1/ims_acscampaignrequests("+ activityRequestId +")", activityRequestObject);
                if (createResponse.IsSuccessStatusCode)
                {
                }
                else
                {
                    var erroMsg = ExtractErrorMessage(createResponse);
                    throw new Exception("Error in Updating Activity Request:" + erroMsg + "");
                }

            }
        }

        public async Task UpdateLeadNextBestActionAttribute(string leadId, JObject nbaObject, string authority, string serviceUrl, string clientId, string secret)
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
                    new AuthenticationHeaderValue("Bearer", GetAccessToken(authority, clientId, secret, serviceUrl));
                HttpResponseMessage createResponse =
                    await httpClient.SendAsJsonAsync(new HttpMethod("PATCH"), "api/data/v9.1/leads(" + new Guid(leadId) + ")", nbaObject);
                if (createResponse.IsSuccessStatusCode)
                {

                }
                else
                {
                    var erroMsg = ExtractErrorMessage(createResponse);
                    throw new Exception("Error in Updating Activity Request:" + erroMsg + "");
                }

            }
        }
        public string ExtractErrorMessage(HttpResponseMessage response)
        {
            string errorMsg = string.Empty;
            JObject errorObject = JObject.Parse(response.Content.ReadAsStringAsync().Result);
            errorMsg = errorObject["error"]["message"].ToString();
            return errorMsg;
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
        public async void CreateBatchJobRecordAsync(string authority,string serviceUrl, string clientId, string secret,JObject batchJobObject)
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
                    new AuthenticationHeaderValue("Bearer", GetAccessToken(authority, clientId, secret, serviceUrl));
                HttpResponseMessage createResponse =
                    await httpClient.SendAsJsonAsync(new HttpMethod("POST"), "api/data/v9.1/ims_batchjobs", batchJobObject);
                if (createResponse.IsSuccessStatusCode)
                {
                }
                else
                {
                    JObject errorJobject = JObject.Parse(createResponse.Content.ReadAsStringAsync().Result);
                    throw new Exception("Error in Creating NBA Activity:" + errorJobject + "");
                }

            }
        }

        public async Task CloseNextBestAction(string nbaActivityId, JObject nbaObject, string authority, string serviceUrl, string clientId, string secret)
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
                    new AuthenticationHeaderValue("Bearer", GetAccessToken(authority, clientId, secret, serviceUrl));
                HttpResponseMessage createResponse =
                    await httpClient.SendAsJsonAsync(new HttpMethod("PATCH"), "api/data/v9.1/ims_nextbestactions(" + nbaActivityId + ")", nbaObject);
                if (createResponse.IsSuccessStatusCode)
                {

                }
                else
                {
                    var erroMsg = ExtractErrorMessage(createResponse);
                    throw new Exception("Error in Updating Activity Request:" + erroMsg + "");
                }

            }
        }

        public  DateTime AddBusinessDays(DateTime date, int days)
        {
            if (days < 0)
            {
                throw new ArgumentException("days cannot be negative", "days");
            }

            if (days == 0) return date;

            if (date.DayOfWeek == DayOfWeek.Saturday)
            {
                date = date.AddDays(2);
                days -= 1;
            }
            else if (date.DayOfWeek == DayOfWeek.Sunday)
            {
                date = date.AddDays(1);
                days -= 1;
            }

            date = date.AddDays(days / 5 * 7);
            int extraDays = days % 5;

            if ((int)date.DayOfWeek + extraDays > 5)
            {
                extraDays += 2;
            }

            return date.AddDays(extraDays);

        }

        public string CreateXml(string xml, string cookie, int page, int count)
        {
            StringReader stringReader = new StringReader(xml);
            XmlTextReader reader = new XmlTextReader(stringReader);

            // Load document
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);

            return CreateXml(doc, cookie, page, count);
        }

        public string CreateXml(XmlDocument doc, string cookie, int page, int count)
        {
            XmlAttributeCollection attrs = doc.DocumentElement.Attributes;

            //if (cookie != null)
            //{
                XmlAttribute pagingAttr = doc.CreateAttribute("paging-cookie");
               // pagingAttr.Value = cookie;
                attrs.Append(pagingAttr);
            //}

            XmlAttribute pageAttr = doc.CreateAttribute("page");
            pageAttr.Value = System.Convert.ToString(page);
            attrs.Append(pageAttr);

            XmlAttribute countAttr = doc.CreateAttribute("count");
            countAttr.Value = System.Convert.ToString(count);
            attrs.Append(countAttr);

            StringBuilder sb = new StringBuilder(1024);
            StringWriter stringWriter = new StringWriter(sb);

            XmlTextWriter writer = new XmlTextWriter(stringWriter);
            doc.WriteTo(writer);
            writer.Close();

            return sb.ToString();
        }
    }
}
