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

namespace Infy.MS.PossibleNowDNC
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
        public List<ActiveContact> FetchActiveContacts(string authority, string clientId, string secret, string serviceUrl, string accessToken, Dictionary<string, string> dcConfigs, int jobFrequency)
        {
            int pageNumber = 1;
            List<ActiveContact> lstActiveContacts = null;
            try
            {
                lstActiveContacts = EnrichContactDetailsFromCRM(authority, clientId, secret, serviceUrl, accessToken, pageNumber, string.Empty, null, dcConfigs, jobFrequency);
            }
            catch (Exception ex)
            {
                lstActiveContacts = null;
                throw new Exception("Error while fetching Active Leads: " + ex.Message + Environment.NewLine);
            }
            return lstActiveContacts;
        }
        public List<ActiveLead> FetchActiveLeads(string authority, string clientId, string secret, string serviceUrl, string accessToken, Dictionary<string, string> dcConfigs, int jobFrequency)
        {
            int pageNumber = 1;
            List<ActiveLead> lstActiveLeads = null;
            try
            {
                lstActiveLeads = EnrichDetailsFromCRM(authority, clientId, secret, serviceUrl, accessToken, pageNumber, string.Empty, null, dcConfigs, jobFrequency);
            }
            catch (Exception ex)
            {
                lstActiveLeads = null;
                throw new Exception("Error while fetching Active Leads: " + ex.Message + Environment.NewLine);
            }
            return lstActiveLeads;
        }

        public List<ActiveLead> EnrichDetailsFromCRM(string authority, string clientId, string secret, string serviceUrl, string accessToken, int pageNumber, string pagingCookie, List<ActiveLead> lstActiveLeads, Dictionary<string, string> dcConfigs, int jobFrequency)
        {
            JObject collection = null;
            HttpResponseMessage response = null;
            if (lstActiveLeads == null) lstActiveLeads = new List<ActiveLead>();
            accessToken = GetAccessToken(authority, clientId, serviceUrl, serviceUrl);
            string activeLeadsXmlQuery = string.Empty;
            //Daily Job 
            if (jobFrequency == 1)
            {
                if (dcConfigs.ContainsKey(Constants.DailyJobLeadFetchXml)) activeLeadsXmlQuery = dcConfigs[Constants.DailyJobLeadFetchXml].ToString();

            }
            //Monthly Job
            else if (jobFrequency == 2)
            {
                if (dcConfigs.ContainsKey(Constants.MonthlyJobLeadFetchXml)) activeLeadsXmlQuery = dcConfigs[Constants.MonthlyJobLeadFetchXml].ToString();

            }
            //Default to Monthly Job
            else
            {
                if (dcConfigs.ContainsKey(Constants.MonthlyJobLeadFetchXml)) activeLeadsXmlQuery = dcConfigs[Constants.MonthlyJobLeadFetchXml].ToString();
            }
            //string activeLeadsXmlQuery = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' page='{0}' count='5000' {1}>" +
            //                            "<entity name='lead'>" +
            //                              "<attribute name='leadid' />" +
            //                              "<attribute name='ims_unformattedworkphone' />" +
            //                              "<attribute name='ims_unformattedcellphone' />" +
            //                              "<attribute name='donotphone' />" +
            //                              "<attribute name='ims_leadid' />" +
            //                              "<filter type='and'>" +
            //                                "<condition attribute='statecode' operator='eq' value='0' />" +
            //                                "<filter type='or'>" +
            //                                  "<condition attribute='ims_unformattedcellphone' operator='not-null' />" +
            //                                  "<condition attribute='ims_unformattedworkphone' operator='not-null' />" +
            //                                "</filter></filter></entity></fetch>";
            var query = string.Empty;
            if (!string.IsNullOrWhiteSpace(pagingCookie))
            {
                query = string.Format(activeLeadsXmlQuery, pageNumber, "paging-cookie='" + pagingCookie + "'");
            }
            else
            {
                query = string.Format(activeLeadsXmlQuery, pageNumber, " ");
            }

            using (HttpClient httpClient = new HttpClient())
            {

          
                httpClient.BaseAddress = new Uri(serviceUrl);
                //httpClient.Timeout = new TimeSpan(0, 2, 0);
                httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
               // httpClient.DefaultRequestHeaders.Add("Prefer", "odata.include-annotations=\"*\"");
                httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);
                ////Header required to include formatted values
                //var formattedValueHeaders = new Dictionary<string, List<string>> {
                //        { "Prefer", new List<string>
                //            { "odata.include-annotations=\"OData.Community.Display.V1.FormattedValue\"" }
                //        }
                //    };
                httpClient.DefaultRequestHeaders.Add("Prefer", new List<string>
                            { "odata.include-annotations=\"OData.Community.Display.V1.FormattedValue\"" });
               //Must encode the FetchXML query because it's a part of the request (GET) string .
               response = httpClient.GetAsync("api/data/v9.1/leads?fetchXml=" + query, HttpCompletionOption.ResponseHeadersRead).Result;

                if (response.IsSuccessStatusCode) //200
                {
                    collection = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                    JToken valArray;
                    if (collection.TryGetValue("value", out valArray))
                    {
                        JArray entities = (JArray)valArray;
                        foreach (JObject entity in entities)
                        {
                            ActiveLead objActiveLead = new ActiveLead();
                            if (entity.ContainsKey("leadid"))
                            {
                                objActiveLead.LeadRecordId = entity["leadid"].ToString();
                            }
                            if (entity.ContainsKey("ims_unformattedworkphone"))
                            {
                                objActiveLead.WorkPhone = entity["ims_unformattedworkphone"].ToString();
                            }
                            if (entity.ContainsKey("ims_unformattedcellphone"))
                            {
                                objActiveLead.CellPhone = entity["ims_unformattedcellphone"].ToString();
                            }
                            if (entity.ContainsKey("address1_telephone1"))
                            {
                                objActiveLead.HomePhone = entity["address1_telephone1"].ToString();
                            }
                            if (entity.ContainsKey("donotphone"))
                            {
                                objActiveLead.Donotcall = (bool)entity["donotphone"];
                            }
                            if (entity.ContainsKey("ims_leadid"))
                            {
                                objActiveLead.LeadId = entity["ims_leadid"].ToString();
                            }
                            if (entity.ContainsKey("_ims_leadsource_value"))
                            {
                                objActiveLead.LeadSource = entity["_ims_leadsource_value@OData.Community.Display.V1.FormattedValue"].ToString();
                            }
                            if (entity.ContainsKey("_ims_referredby_value"))
                            {
                                objActiveLead.ReferedBy = entity["_ims_referredby_value@OData.Community.Display.V1.FormattedValue"].ToString();
                            }
                            if (entity.ContainsKey("_ims_leadstatus_value"))
                            {
                                objActiveLead.LeadStatus = entity["_ims_leadstatus_value@OData.Community.Display.V1.FormattedValue"].ToString();
                            }
                            if (entity.ContainsKey("aa.ims_leadsourcesgroup"))
                            {
                                objActiveLead.SourceGroup = entity["aa.ims_leadsourcesgroup@OData.Community.Display.V1.FormattedValue"].ToString();
                            }



                            lstActiveLeads.Add(objActiveLead);
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
                this.EnrichDetailsFromCRM(authority, clientId, secret, serviceUrl, accessToken, ++pageNumber, pagingCookie, lstActiveLeads, dcConfigs, jobFrequency);
            }
            else
                return lstActiveLeads;

            return lstActiveLeads;
        }

        public List<ActiveContact> EnrichContactDetailsFromCRM(string authority, string clientId, string secret, string serviceUrl, string accessToken, int pageNumber, string pagingCookie, List<ActiveContact> lstActiveContacts, Dictionary<string, string> dcConfigs, int jobFrequency)
        {
            JObject collection = null;
            HttpResponseMessage response = null;
            if (lstActiveContacts == null) lstActiveContacts = new List<ActiveContact>();
            accessToken = GetAccessToken(authority, clientId, serviceUrl, serviceUrl);
            string activeContactsXmlQuery = string.Empty;
            //Daily Job 
            if (jobFrequency == 1)
            {
                if (dcConfigs.ContainsKey(Constants.DailyJobContactFetchXml)) activeContactsXmlQuery = dcConfigs[Constants.DailyJobContactFetchXml].ToString();

            }
            //Monthly Job
            else if (jobFrequency == 2)
            {
                if (dcConfigs.ContainsKey(Constants.MonthlyJobContactFetchXml)) activeContactsXmlQuery = dcConfigs[Constants.MonthlyJobContactFetchXml].ToString();

            }
            //Default to Monthly Job
            else
            {
                if (dcConfigs.ContainsKey(Constants.MonthlyJobContactFetchXml)) activeContactsXmlQuery = dcConfigs[Constants.MonthlyJobContactFetchXml].ToString();
            }
            //string activeLeadsXmlQuery = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' page='{0}' count='5000' {1}>" +
            //                            "<entity name='lead'>" +
            //                              "<attribute name='leadid' />" +
            //                              "<attribute name='ims_unformattedworkphone' />" +
            //                              "<attribute name='ims_unformattedcellphone' />" +
            //                              "<attribute name='donotphone' />" +
            //                              "<attribute name='ims_leadid' />" +
            //                              "<filter type='and'>" +
            //                                "<condition attribute='statecode' operator='eq' value='0' />" +
            //                                "<filter type='or'>" +
            //                                  "<condition attribute='ims_unformattedcellphone' operator='not-null' />" +
            //                                  "<condition attribute='ims_unformattedworkphone' operator='not-null' />" +
            //                                "</filter></filter></entity></fetch>";
            var query = string.Empty;
            if (!string.IsNullOrWhiteSpace(pagingCookie))
            {
                query = string.Format(activeContactsXmlQuery, pageNumber, "paging-cookie='" + pagingCookie + "'");
            }
            else
            {
                query = string.Format(activeContactsXmlQuery, pageNumber, " ");
            }

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(serviceUrl);
                //httpClient.Timeout = new TimeSpan(0, 2, 0);
                httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
               // httpClient.DefaultRequestHeaders.Add("Prefer", "odata.include-annotations = *");
                httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);
                httpClient.DefaultRequestHeaders.Add("Prefer", new List<string>
                            { "odata.include-annotations=\"OData.Community.Display.V1.FormattedValue\"" });
                //Must encode the FetchXML query because it's a part of the request (GET) string .
                response = httpClient.GetAsync("api/data/v9.1/contacts?fetchXml=" + query, HttpCompletionOption.ResponseHeadersRead).Result;
                if (response.IsSuccessStatusCode) //200
                {
                    collection = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                    JToken valArray;
                    if (collection.TryGetValue("value", out valArray))
                    {
                        JArray entities = (JArray)valArray;
                        foreach (JObject entity in entities)
                        {
                            ActiveContact objActiveContact = new ActiveContact();
                            if (entity.ContainsKey("contactid"))
                            {
                                objActiveContact.ContactRecordId = entity["contactid"].ToString();
                            }
                            if (entity.ContainsKey("ims_unformattedbusinessphone"))
                            {
                                objActiveContact.WorkPhone = entity["ims_unformattedbusinessphone"].ToString();
                            }
                            if (entity.ContainsKey("ims_unformattedmobilephone"))
                            {
                                objActiveContact.CellPhone = entity["ims_unformattedmobilephone"].ToString();
                            }
                            if (entity.ContainsKey("donotphone")) 
                            {
                                objActiveContact.Donotcall = (bool)entity["donotphone"];
                            }
                            if (entity.ContainsKey("_ims_contactsource_value"))
                            {
                                objActiveContact.ContactSource = entity["_ims_contactsource_value@OData.Community.Display.V1.FormattedValue"].ToString(); 
                            }
                            if (entity.ContainsKey("aa.ims_leadsourcesgroup"))
                            {
                                objActiveContact.SourceGroup = entity["aa.ims_leadsourcesgroup@OData.Community.Display.V1.FormattedValue"].ToString();
                            }

                            lstActiveContacts.Add(objActiveContact);
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
                this.EnrichContactDetailsFromCRM(authority, clientId, secret, serviceUrl, accessToken, ++pageNumber, pagingCookie, lstActiveContacts, dcConfigs, jobFrequency);
            }
            else
                return lstActiveContacts;

            return lstActiveContacts;
        }

        public static async Task UpdateLeadDNC(string authority, string clientId, string secret, string serviceUrl, ActiveLead leadObj, bool doNotCall, bool isInternal, Dictionary<string, string> dcConfigs)
        {
            Common objCommon = new Common();
            try
            {
                string accessToken = objCommon.GetAccessToken(authority, clientId, serviceUrl, serviceUrl);

                bool warning = false;
                if (doNotCall && leadObj.LeadStatus.ToLower()!= "active" && leadObj.LeadStatus.ToLower() != "applied" && leadObj.LeadStatus.ToLower() != "mba preapproved") {
                    warning = checkWarningStatus(dcConfigs, doNotCall, isInternal, leadObj.LeadSource, leadObj.ReferedBy,leadObj.SourceGroup);
                }
                //Update Lead
                JObject updateLead = new JObject
                {
                    { "donotphone",doNotCall},
                    { "ims_donotallowsms",doNotCall},
                    { "ims_isinternaldnc",isInternal},
                    { "ims_enablewarnings",warning}
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
                    HttpResponseMessage updateResponse = await httpClient.SendAsJsonAsync(new HttpMethod("PATCH"), "api/data/v9.1/leads(" + leadObj.LeadRecordId + ")", updateLead);
                    if (!updateResponse.IsSuccessStatusCode) throw new Exception(updateResponse.ReasonPhrase + "|Status Code " + updateResponse.StatusCode + "|Error:" + objCommon.ExtractErrorMessage(updateResponse));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error while Updating Lead DNC: " + ex.Message + Environment.NewLine);
            }
        }

        public static async Task UpdateContactDNC(string authority, string clientId, string secret, string serviceUrl, ActiveContact contactObj, bool doNotCall,bool isInternal, Dictionary<string, string> dcConfigs)
        {
            Common objCommon = new Common();
            try
            {
                string accessToken = objCommon.GetAccessToken(authority, clientId, serviceUrl, serviceUrl);

                bool warning = false;
                if (doNotCall)
                {
                   warning = checkWarningStatus(dcConfigs, doNotCall, isInternal, contactObj.ContactSource, null,contactObj.SourceGroup);
                }
                //Update Lead
                JObject updateContact = new JObject
                {
                    { "donotphone",doNotCall},
                    { "ims_donotallowsms",doNotCall},
                    { "ims_isinternaldnc",isInternal},
                    { "ims_enablewarnings",warning}
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
                    HttpResponseMessage updateResponse = await httpClient.SendAsJsonAsync(new HttpMethod("PATCH"), "api/data/v9.1/contacts(" + contactObj.ContactRecordId + ")", updateContact);
                    if (!updateResponse.IsSuccessStatusCode) throw new Exception(updateResponse.ReasonPhrase + "|Status Code " + updateResponse.StatusCode + "|Error:" + objCommon.ExtractErrorMessage(updateResponse));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error while Updating Contact DNC: " + ex.Message + Environment.NewLine);
            }
        }

        public static bool checkWarningStatus( Dictionary<string, string> dcConfigs, bool doNotCall,bool isInternal, string source, string refferedBy,string sourceGroup)
        {
            if (!isInternal) {

                if (refferedBy != null&& refferedBy!="")
                {
                    return false;
                }

                if (source==null || source=="")
                {
                    return true;
                }
                string approvedSources = string.Empty;
                if (dcConfigs.ContainsKey(Constants.ApprovedSources))
                    approvedSources = dcConfigs[Constants.ApprovedSources].ToString();
                List<string> approvedSourceList = approvedSources.Split(',').ToList<string>();
                approvedSourceList = approvedSourceList.ConvertAll(d => d.ToLower());
                if (approvedSourceList.Contains(source.ToLower()))
                {
                    return false;
                }
                else if (sourceGroup != null && sourceGroup != "" && approvedSourceList.Contains(sourceGroup.ToLower()))
                {
                    return false;
                }
                else 
                {
                    foreach (string sourceName in approvedSourceList)
                    {
                        if (Regex.IsMatch(source.ToLower(), sourceName, RegexOptions.IgnoreCase))
                        {
                            return false;
                        }
                        if (sourceGroup!= null && sourceGroup!="" && Regex.IsMatch(sourceGroup.ToLower(), sourceName, RegexOptions.IgnoreCase))
                        {
                            return false;
                        }
                    }
                    return true;
                }

                // ims_contactsource // ims_isinternaldnc
            }
            else
            {
               
                return true;
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

        public static string PhoneNumberUnformatting(string phoneNumber)
        {
            if (phoneNumber != null)
            {
                string pattern = @"[^a-zA-Z0-9]";
                phoneNumber = Regex.Replace(phoneNumber, pattern, string.Empty);
                if (phoneNumber.IndexOf('1') == 0 && phoneNumber.Length == 11)
                {
                    phoneNumber = phoneNumber.Remove(0, 1);
                }
                else if (phoneNumber.IndexOf('0') == 0 && phoneNumber.Length == 13)
                {
                    phoneNumber = phoneNumber.Remove(0, 3);
                }
            }

            return phoneNumber;

        }

        public static async Task CreateCommunicationPreference(string authority, string clientId, string secret, string serviceUrl, Guid batchJobId, Guid leadId, string channel, string filterName, string flag)
        {
            Common objCommon = new Common();
            try
            {
                string accessToken = objCommon.GetAccessToken(authority, clientId, secret, serviceUrl);
                //Create Notes
                JObject createCommunicationPreference = new JObject
                {
                    {"ims_channel", channel},
                    {"ims_filtername",filterName },
                    {"ims_flag",flag },
                    {"ims_batchjob@odata.bind","/ims_batchjobs("+batchJobId+")"},
                    {"ims_lead@odata.bind","/leads("+leadId+")" }
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
                        await httpClient.SendAsJsonAsync(HttpMethod.Post, "api/data/v9.1/ims_communicationpreferences", createCommunicationPreference);
                    if (!createResponse.IsSuccessStatusCode)
                        throw new Exception(createResponse.ReasonPhrase + "|Status Code " + createResponse.StatusCode + "|Error:" + objCommon.ExtractErrorMessage(createResponse));

                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error while Creating Communication Preference record:" + ex.Message);
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
    }

    public class ActiveContact
    {
        string contactRecordId;
        string cellPhone;
        string workPhone;
        string homePhone;
        bool donotcall;
        string contactId;
        string contactsource;
        string sourcegroup;
        public string ContactRecordId
        {
            get
            {
                return contactRecordId;
            }
            set
            {
                contactRecordId = value;
            }
        }
        public string CellPhone
        {
            get
            {
                return cellPhone;
            }
            set
            {
                cellPhone = value;
            }
        }
        public string WorkPhone
        {
            get
            {
                return workPhone;
            }
            set
            {
                workPhone = value;
            }
        }
        public string HomePhone
        {
            get
            {
                return homePhone;
            }
            set
            {
                homePhone = value;
            }
        }
        public bool Donotcall
        {
            get
            {
                return donotcall;
            }
            set
            {
                donotcall = value;
            }
        }
        public string ContactId
        {
            get
            {
                return contactId;
            }
            set
            {
                contactId = value;
            }
        }
        public string ContactSource
        {
            get
            {
                return contactsource;
            }
            set
            {
                contactsource = value;
            }
        }

        public string SourceGroup
        {
            get
            {
              return  sourcegroup;
            }
            set
            {
                sourcegroup = value;
            }
        }
    }
    public class ActiveLead
    {
        string leadRecordId;
        string cellPhone;
        string workPhone;
        string homePhone;
        bool donotcall;
        string leadId;
        string leadSource;
        string referedBy;
        string leadStatus;
        string sourcegroup;
        public string LeadRecordId
        {
            get
            {
                return leadRecordId;
            }
            set
            {
                leadRecordId = value;
            }
        }
        public string CellPhone
        {
            get
            {
                return cellPhone;
            }
            set
            {
                cellPhone = value;
            }
        }
        public string WorkPhone
        {
            get
            {
                return workPhone;
            }
            set
            {
                workPhone = value;
            }
        }
        public string HomePhone
        {
            get
            {
                return homePhone;
            }
            set
            {
                homePhone = value;
            }
        }
        public bool Donotcall
        {
            get
            {
                return donotcall;
            }
            set
            {
                donotcall = value;
            }
        }
        public string LeadId
        {
            get
            {
                return leadId;
            }
            set
            {
                leadId = value; 
            }
        }
        public string LeadSource
        {
            get
            {
                return leadSource;
            }
            set
            {
                leadSource = value; ///ims_leadsource
            }
        }
        public string ReferedBy
        {
            get
            {
                return referedBy;
            }
            set
            {
                referedBy = value;
            }
        }
        public string LeadStatus
        {
            get
            {
                return leadStatus;
            }
            set
            {
                leadStatus = value;
            }
        }
        public string SourceGroup
        {
            get
            {
                return sourcegroup;
            }
            set
            {
                sourcegroup = value;
            }
        }
    }
    public class GenericDto
    {
        [JsonProperty("@Microsoft.Dynamics.CRM.fetchxmlpagingcookie")]
        public string Cookie { get; set; }

    }
}
