using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.WebServiceClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Infy.MS.UpcomingBirthdayTaskCreation
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
        public string GetCrmAccessToken()
        {
            string authority = ConfigurationManager.AppSettings[Constants.Authority].ToString();
            string clientId = ConfigurationManager.AppSettings[Constants.ClientId].ToString();
            string secret = ConfigurationManager.AppSettings[Constants.Secret].ToString();
            string serviceUrl = ConfigurationManager.AppSettings[Constants.ServiceUrl].ToString();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext authContext = new Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext(authority);
            ClientCredential credential = new ClientCredential(clientId, secret);
            AuthenticationResult result = authContext.AcquireTokenAsync(serviceUrl, credential).Result;
            return result.AccessToken;
           
        }
        public IOrganizationService GetCrmService(string accessToken)
        {
            Uri serviceUrl = new Uri(ConfigurationManager.AppSettings[Constants.ServiceUrl].ToString() + ConfigurationManager.AppSettings["orgUrl"].ToString());
            using (var sdkService = new OrganizationWebProxyClient(serviceUrl, false))
            {
                sdkService.HeaderToken = accessToken;
                var orgService = (IOrganizationService)sdkService;
                return orgService;
            }
        }
        public static Guid CreateBatchJobRecord(IOrganizationService service,string batchJobName)
        {
            Guid batchJobId = Guid.Empty;
            try
            {
                Entity en = new Entity("ims_batchjob");
                en.Attributes["ims_name"] = batchJobName + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Year;
                batchJobId = service.Create(en);
            }
            //    string accessToken = objCommon.GetCrmAccessToken();
            //    //Create
            //    JObject newBatchJob = new JObject
            //    {
            //        {"ims_name", batchJobName+"-"+DateTime.Now.Month+"-"+DateTime.Now.Day+"-"+DateTime.Now.Year}
            //    };

            //    using (HttpClient httpClient = new HttpClient())
            //    {
            //        httpClient.BaseAddress = new Uri(serviceUrl);
            //        httpClient.Timeout = new TimeSpan(0, 2, 0);
            //        httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
            //        httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
            //        httpClient.DefaultRequestHeaders.Accept.Add(
            //            new MediaTypeWithQualityHeaderValue("application/json"));
            //        httpClient.DefaultRequestHeaders.Authorization =
            //            new AuthenticationHeaderValue("Bearer", accessToken);
            //        HttpResponseMessage createResponse =
            //            await httpClient.SendAsJsonAsync(HttpMethod.Post, "api/data/v9.1/ims_batchjobs", newBatchJob);
            //        if (createResponse.IsSuccessStatusCode)
            //        {
            //            string batchJobUri = createResponse.Headers.GetValues("OData-EntityId").FirstOrDefault();
            //            if (batchJobUri != null)
            //                batchJobId = Guid.Parse(batchJobUri.Split('(', ')')[1]);
            //        }
            //        else
            //        {
            //            batchJobId = Guid.Empty;
            //            throw new Exception(createResponse.ReasonPhrase + "Status Code " + createResponse.StatusCode);
            //        }
            //    }
            //}
            catch (Exception ex)
            {
                batchJobId = Guid.Empty;
                throw new Exception("Error while Creating Batch Job record: " + ex.Message + Environment.NewLine);
            }
            return batchJobId;
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

        public class GenericDto
        {
            [JsonProperty("@Microsoft.Dynamics.CRM.fetchxmlpagingcookie")]
            public string Cookie { get; set; }

        }
    }
}
