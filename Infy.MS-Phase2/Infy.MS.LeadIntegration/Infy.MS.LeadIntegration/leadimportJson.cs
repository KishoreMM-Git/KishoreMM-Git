using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Infy.MS.LeadIntegration
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
    public static class leadimportJson
    {
        [FunctionName("leadimportJson")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            string client = string.Empty;
            string leadProvider = string.Empty;
            string campaignId = string.Empty;
            bool isException = false;
            Dictionary<string, string> dcConfig = null;
            string requestBodyJson = string.Empty;
            HttpClient HttpClient = null;
            Guid recordId = Guid.Empty;
            try
            {
                log.Info("C# HTTP trigger function processed a request.");

                //Get Client, Provider & Campaign ID from Request Query String
                client = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "Client", true) == 0).Value;
                leadProvider = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "Provider", true) == 0).Value;
                campaignId = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "CampaignId", true) == 0).Value;

                //Get Http Client
                HttpClient = GetHttpClientforCRM(log);
                log.Info("HTTP Client acquired");

                //Fetch Configurations for Lead Integration from CRM
                dcConfig = FetchConfigurations(HttpClient, Constants.configSetUpName);
                log.Info("Configurations fetched from CRM");


                //Check If Client, Provider And Campaign ID has valid data
                if (string.IsNullOrEmpty(client) && string.IsNullOrEmpty(leadProvider) && string.IsNullOrEmpty(campaignId))
                {
                    //Write Error in Error Log Entity
                    CreateErrorLog(HttpClient, client, leadProvider, campaignId, requestBodyJson, GetMessage(Constants.invalidQuerystring, dcConfig), log);
                    return req.CreateResponse(HttpStatusCode.BadRequest, "One or more error Occurred");
                }
                log.Info("Client:" + client + " Provider:" + leadProvider + " Campaign Id:" + campaignId);


                //Fetch Import Data Master
                Guid importDataMasterId = Guid.Empty;
                bool reuestContent = false;
                importDataMasterId = GetImportDataMaster(HttpClient, leadProvider, ref reuestContent);

                //Read Request Body 
                string content = await req.Content.ReadAsStringAsync();
                log.Info("Request Content" + content);
                content = HttpUtility.HtmlDecode(content);
                NameValueCollection requestBodyQueryString = HttpUtility.ParseQueryString(content);

                log.Info("Import Data Master Request Content:" + reuestContent);
                //Deserialize Request Body to Lead Object
                JObject requestBodyObject = null;
                if (!reuestContent && !leadProvider.Equals("Leadpops", StringComparison.OrdinalIgnoreCase) && !leadProvider.Equals("BoomTown", StringComparison.OrdinalIgnoreCase))
                {
                    log.Info("Before Request Body Parse");
                    requestBodyObject = JsonConvert.DeserializeObject<JObject>(await req.Content.ReadAsStringAsync());
                    log.Info("Request Body:" + requestBodyObject);

                    //Convert Request body to JSON string
                    if (requestBodyObject != null)
                    {
                        requestBodyJson = JsonConvert.SerializeObject(requestBodyObject, Formatting.Indented);
                        log.Info("Request Body in JSON:" + requestBodyJson);
						//----------Fix for Lending Tree- Nested JSON Issue-----------------//
						if (leadProvider.Equals("LendingTree", StringComparison.OrdinalIgnoreCase))
						{
							requestBodyObject = flattenNestedJSON(requestBodyObject);
							log.Info("Flattened Request Body in JSON:" + requestBodyObject);
							requestBodyJson += "\n" +"------Flattened JSON-------" +"\n" + JsonConvert.SerializeObject(requestBodyObject, Formatting.Indented);
						}
                        //--------------------------------------------------------------------//
                    }
                }
                var queryStringJson = "";
                if (req.GetQueryNameValuePairs() != null)
                {
                    //Convert request Query String to JSON String
                    queryStringJson = JsonConvert.SerializeObject(req.GetQueryNameValuePairs(), Formatting.Indented);
                }
                if (leadProvider.Equals("Leadpops", StringComparison.OrdinalIgnoreCase) || leadProvider.Equals("BoomTown", StringComparison.OrdinalIgnoreCase))
                {
                    requestBodyJson += content + "\n" + queryStringJson;
                }
                else
                {
                    requestBodyJson = requestBodyJson + "\n" + queryStringJson;
                }

                //Validate Lead Source               
                bool isValidLeadSource = false;
                string leadSource = string.Empty;
                int dailyWarningLimit = -1;
                int dailyThreshold = -1;
                log.Info("Validate Lead SOurce.");
                isValidLeadSource = ValidateLeadSource(HttpClient, campaignId, ref leadSource, ref dailyWarningLimit, ref dailyThreshold);
                if (!isValidLeadSource)
                {
                    log.Info(GetMessage(Constants.LeadSourceNotFound, dcConfig));
                    CreateErrorLog(HttpClient, client, leadProvider, campaignId, requestBodyJson, GetMessage(Constants.LeadSourceNotFound, dcConfig), log);
                    return req.CreateResponse(HttpStatusCode.BadRequest, "One or more error Occurred");
                }

                //Get Lead Staging Record Count Created On Today for requested Lead Source
                int recordCount = GetLeadStagingRecordCount(HttpClient, leadSource);

                //Validate Daily Threshold
                if (dailyThreshold != -1 && dailyThreshold != 0)
                {
                    if (recordCount > 0)
                    {
                        if (recordCount >= dailyThreshold)
                        {
                            CreateErrorLog(HttpClient, client, leadProvider, campaignId, requestBodyJson, GetMessage(Constants.DailyThreshold, dcConfig), log);
                            return req.CreateResponse(HttpStatusCode.BadRequest, "One or more error Occurred");
                        }
                    }
                }

                // Daily Warning Limit
                if (dailyWarningLimit != -1 && dailyWarningLimit != 0)
                {
                    if (recordCount > 0)
                    {
                        if (recordCount >= dailyWarningLimit)
                        {
                            CreateErrorLog(HttpClient, client, leadProvider, campaignId, requestBodyJson, GetMessage(Constants.DailyWarningLimit, dcConfig), log, true);
                        }
                    }
                }

                //Fetch Import details mappings
                List<Mapping> mappings = null;
                if (importDataMasterId != Guid.Empty)
                    mappings = FetchMappings(HttpClient, importDataMasterId.ToString());


                //Create Lead Staging Record
                recordId = CreateLeadStaging(HttpClient, log, leadProvider, leadSource, dcConfig, requestBodyObject, reuestContent, mappings, req.GetQueryNameValuePairs(), requestBodyJson, requestBodyQueryString);
                log.Info("Record ID:" + recordId);
            }
            catch (Exception ex)
            {
                isException = true;
                log.Info("Error:" + ex.Message + ex.InnerException );
                //Write Error in Error Log Entity
                CreateErrorLog(HttpClient, client, leadProvider, campaignId, requestBodyJson, ex.Message + ex.InnerException, log);
            }

            if (isException)
            {
            
                return req.CreateResponse(HttpStatusCode.BadRequest, "One or more error Occurred");
            }
            else
            {
             
                if (leadProvider.Equals("BoomTown", StringComparison.OrdinalIgnoreCase))
                {
                    var leadid = recordId.ToString().Replace("-", "");
                    var xml = "<ImportResults><ImportResult refId ='" + recordId + "' leadId ='4' result = 'Success' message = 'Lead Created' ></ImportResult></ImportResults>";
                    var response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new StringContent(xml);
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
                    return response;
                }
                else
                {
                    return req.CreateResponse(HttpStatusCode.OK, "Success");
                }
            }
        }
        public static HttpClient GetHttpClientforCRM(TraceWriter log)
        {
            string clientID = string.Empty;
            string clientsecret = string.Empty;
            string apiUrl = string.Empty;
            string authority = string.Empty;
            HttpClient httpClientforCRM = new HttpClient();
            try
            {

                log.Info("Creating instance of 'HttpClient' for CRM.");
                clientID = Environment.GetEnvironmentVariable("crmClientId");
                clientsecret = Environment.GetEnvironmentVariable("crmclientSecret");
                apiUrl = Environment.GetEnvironmentVariable("crmApiUrl");
                authority = Environment.GetEnvironmentVariable("authority");
                if (string.IsNullOrEmpty(clientID) && string.IsNullOrEmpty(clientsecret) && string.IsNullOrEmpty(apiUrl) && string.IsNullOrEmpty(authority))
                {
                    throw new Exception("Error: Client ID || Client Secret || API URL || Authority value is Null or Missing in Function Application Settings!");
                }
                var creds = new ClientCredential(clientID, clientsecret);
                AuthenticationContext authContext = new AuthenticationContext(authority);
                var token = authContext.AcquireTokenAsync(apiUrl, creds).Result.AccessToken;
                httpClientforCRM.BaseAddress = new Uri(apiUrl);
                httpClientforCRM.Timeout = new TimeSpan(0, 2, 0);
                httpClientforCRM.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                httpClientforCRM.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                httpClientforCRM.DefaultRequestHeaders.Add("OData-Version", "4.0");
                httpClientforCRM.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
            catch (Exception httpcdserror)
            {
                log.Info("Error while Connecting to CRM API." + httpcdserror.InnerException.Message);
                throw new Exception("CDS_API_Error", new Exception(httpcdserror.InnerException.Message));
            }
            return httpClientforCRM;
        }

        public static Dictionary<string, string> FetchConfigurations(HttpClient httpClient, string configSetupName)
        {
            Dictionary<string, string> dcConfigs = null;
            JObject collection;
            try
            {
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
                            JToken name = entity.GetValue("ims_name");
                            JToken value = entity.GetValue("ims_value");
                            if (name.Type != JTokenType.Null && value.Type != JTokenType.Null)
                                dcConfigs.Add(name.ToString(), value.ToString());
                        }
                    }
                }
                else
                {
                    dcConfigs = null;
                    throw new Exception(response.ReasonPhrase + "Status Code " + response.StatusCode + " Content" + response.Content + leadimportJson.ExtractErrorMessage(response) + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                //log Error
                dcConfigs = null;
                throw new Exception("Error while fetching Configurations: " + ex.Message + ex.InnerException + Environment.NewLine);
            }

            return dcConfigs;
        }

        public static int GetLeadStagingRecordCount(HttpClient client, string leadSource)
        {
            int recordCount = -1;
            JObject collection;
            try
            {
                string fetchXmlQuery = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' aggregate='true'>" +
                                    "<entity name='ims_leadstaging'>" +
                                    "<attribute name='ims_leadstagingid' aggregate='count' alias='Record_Count'/>" +
                                    "<filter type='and'>" +
                                    "<condition attribute='createdon' operator='today' />" +
                                    "<condition attribute='ims_source' operator='eq' value='" + leadSource + "' />" +
                                    "</filter></entity></fetch>";
                HttpResponseMessage response = client.GetAsync("api/data/v9.1/ims_leadstagings?fetchXml=" + WebUtility.UrlEncode(fetchXmlQuery), HttpCompletionOption.ResponseHeadersRead).Result;

                if (response.IsSuccessStatusCode) //200
                {
                    collection = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                    JToken valArray;
                    if (collection.TryGetValue("value", out valArray))
                    {
                        JArray entities = (JArray)valArray;
                        foreach (JObject entity in entities)
                        {
                            JToken tokenRecordCount = entity.GetValue("Record_Count");
                            if (tokenRecordCount.Type != JTokenType.Null)
                                recordCount = (int)tokenRecordCount;
                        }
                    }
                }
                else
                {
                    recordCount = -1;
                    throw new Exception(response.ReasonPhrase + "Status Code " + response.StatusCode + " Content" + response.Content + leadimportJson.ExtractErrorMessage(response) + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                recordCount = -1;
                throw new Exception("Error while Getting Lead Staging Record Count: " + ex.Message + ex.InnerException + Environment.NewLine);
            }

            return recordCount;
        }

        public static bool ValidateLeadSource(HttpClient client, string campaignId, ref string leadSource, ref int dailyWarningLimit, ref int dailyThreshold)
        {
            bool isValidLeadSource = false;
            try
            {
                HttpResponseMessage response = client.GetAsync("/api/data/v9.1/ims_leadsources?$select=ims_name,ims_dailywarninglimit,ims_dailythreshold&$filter=ims_id eq '" + campaignId + "' and  statecode eq 0").Result;
                if (response.IsSuccessStatusCode)
                {
                    JObject collection = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                    JToken valArray;
                    if (collection.TryGetValue("value", out valArray))
                    {
                        JArray entities = (JArray)valArray;
                        foreach (JObject entity in entities)
                        {
                            JToken name = entity.GetValue("ims_name");
                            JToken dailyWarLim = entity.GetValue("ims_dailywarninglimit");
                            JToken dailyThres = entity.GetValue("ims_dailythreshold");
                            if (name.Type != JTokenType.Null)
                            {
                                isValidLeadSource = true;
                                leadSource = (string)name;
                            }
                            if (dailyWarLim.Type != JTokenType.Null)
                            {
                                dailyWarningLimit = (int)dailyWarLim;
                            }
                            if (dailyThres.Type != JTokenType.Null)
                            {
                                dailyThreshold = (int)dailyThres;
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception(response.ReasonPhrase + "Status Code " + response.StatusCode + "Content" + response.Content + leadimportJson.ExtractErrorMessage(response) + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error While Fetching Lead Source: " + ex.Message + ex.InnerException );
            }
            return isValidLeadSource;
        }

        public static Guid CreateLeadStaging(HttpClient client, TraceWriter log, string leadProvider, string leadSource, Dictionary<string, string> dcConfig, JObject requestBody, bool content, List<Mapping> mappings, IEnumerable<KeyValuePair<string, string>> queryStringData, string requestBodyJson, NameValueCollection requestBodyQueryString)
        {
            bool dataFound = false;
            Guid leadStagingId = Guid.Empty;

            //Check Request body is valid??
            if (!content && !leadProvider.Equals("Leadpops", StringComparison.OrdinalIgnoreCase) && !leadProvider.Equals("BoomTown", StringComparison.OrdinalIgnoreCase))
            {
                if (requestBody == null)
                {
                    throw new Exception(GetMessage(Constants.invalidRequestBody, dcConfig));
                }
            }

            if (mappings != null)
            {

                //Create Lead Staging Object
                JObject newLeadStaging = new JObject();
                foreach (Mapping mapping in mappings)
                {
                    string value = GetValueFromSource(requestBody, content, mapping, queryStringData, leadProvider, requestBodyQueryString);
                    if (!string.IsNullOrEmpty(value))
                    {
                        JToken keyExist = newLeadStaging[mapping.Target];
                        if (keyExist == null)
                        {
                            dataFound = true;
                            newLeadStaging.Add(mapping.Target, JToken.FromObject(value));
                        }
                    }
                }
                //Lead Source, Request Content && Import Process Mapping
                newLeadStaging.Add(LeadStaging.ImportProcessName, JToken.FromObject(leadProvider));
                newLeadStaging.Add(LeadStaging.LeadSource, JToken.FromObject(leadSource));
                newLeadStaging.Add(LeadStaging.RequestContent, JToken.FromObject(requestBodyJson));

                if (!dataFound)
                {
                    throw new Exception(GetMessage(Constants.invalidRequestBody, dcConfig));
                }
                HttpResponseMessage createResponse =
                    client.SendAsJsonAsync(HttpMethod.Post, "api/data/v9.1/ims_leadstagings", newLeadStaging).Result;

                if (createResponse.IsSuccessStatusCode)
                {
                    string leadUri = createResponse.Headers.GetValues("OData-EntityId").FirstOrDefault();
                    if (leadUri != null)
                        leadStagingId = Guid.Parse(leadUri.Split('(', ')')[1]);
                    log.Info("Lead Stating Record '{0}' created.", leadStagingId.ToString());
                }
                else
                    throw new Exception("Error while creating Lead Staging Record: " + createResponse.ReasonPhrase + " " + createResponse.StatusCode + " Content" + createResponse.Content + leadimportJson.ExtractErrorMessage(createResponse));
            }
            return leadStagingId;
        }

        public static string GetValueFromSource(JObject requestBody, bool content, Mapping mapping, IEnumerable<KeyValuePair<string, string>> queryStringData, string leadProvider, NameValueCollection requestBodyQueryString)
        {
            string value = string.Empty;
            //Get Value from Request Body
            if (!content)
            {
                if (!string.IsNullOrEmpty(mapping.className))
                {
                    if (requestBody.Value<JObject>(mapping.className) != null) //ClassName would come from mapping which is sender[mapping.ClassName]
                    {
                        var nestedObject = requestBody.Value<JObject>(mapping.className).Properties(); //[mapping.ClassName]
                        var nestedObjectDict = nestedObject.ToDictionary(k => k.Name, v => v.Value.ToString());
                        if (nestedObjectDict.ContainsKey(mapping.Source))
                        {
                            if (!string.IsNullOrEmpty(nestedObjectDict[mapping.Source].ToString())) //lastName[mapping.sourcefield]
                                value = nestedObjectDict[mapping.Source].ToString();
                        }
                    }
                }
                else if (leadProvider.Equals("Leadpops", StringComparison.OrdinalIgnoreCase) || leadProvider.Equals("BoomTown", StringComparison.OrdinalIgnoreCase))
                {
                    if (requestBodyQueryString[mapping.Source] != null)
                    {
                        value = requestBodyQueryString[mapping.Source].ToString();
                    }
                }
                else if (requestBody[mapping.Source] != null)
                {
                    value = requestBody[mapping.Source].ToString();
                }
            }
            //Get Value from Request Query String
            else if (content)
            {
                value = queryStringData.FirstOrDefault(q => string.Compare(q.Key, mapping.Source, true) == 0).Value;
            }

            //Trim Data if length is exceeded
            if (!string.IsNullOrEmpty(value))
            {
                if (mapping.MaxLengthAllowed != 0)
                {
                    if (value.Length > mapping.MaxLengthAllowed)
                    {
                        value = value.Substring(0, Math.Min(mapping.MaxLengthAllowed, value.Length));
                    }
                }
            }
            return value;
        }

        public static void CreateErrorLog(HttpClient httpClient, string client, string leadProvider, string campaignId, string reqBodyJson, string errorMessage, TraceWriter log, bool dailyWarningLimit = false)
        {
            JObject errorLog = new JObject();
            errorMessage = errorMessage.Substring(0, Math.Min(4900, errorMessage.Length));
            errorLog.Add(ErrorLog.PrimaryName, JToken.FromObject("Client:" + client + " Provider:" + leadProvider + " Campaign Id:" + campaignId));
            errorLog.Add(ErrorLog.ErrorDetails, JToken.FromObject(errorMessage));
            if (!string.IsNullOrEmpty(leadProvider)) errorLog.Add(ErrorLog.LeadProvider, JToken.FromObject(leadProvider));
            if (!string.IsNullOrEmpty(reqBodyJson)) errorLog.Add(ErrorLog.LeadJSONData, JToken.FromObject(reqBodyJson));
            if (dailyWarningLimit) errorLog.Add(ErrorLog.DailyWarningLimitReached, JToken.FromObject(dailyWarningLimit));

            HttpResponseMessage createResponse =
                httpClient.SendAsJsonAsync(HttpMethod.Post, "api/data/v9.1/ims_errorlogs", errorLog).Result;

            if (!createResponse.IsSuccessStatusCode)
            {
                log.Info("Error While Creating Error Log record:" + createResponse.ReasonPhrase + " Status Code:" + createResponse.StatusCode + " Content" + createResponse.Content + leadimportJson.ExtractErrorMessage(createResponse));
            }
        }

        public static string GetMessage(string key, Dictionary<string, string> dcConfigDetails)
        {
            if (dcConfigDetails != null)
            {
                if (dcConfigDetails.ContainsKey(key))
                {
                    return dcConfigDetails[key];
                }
                else
                {
                    return "They Key '" + key + "' is not present in the configurations. Please check the configurations.";
                }
            }
            else
            {
                return "App Config Setup OR Configuration is missing";
            }
        }

        public static Guid GetImportDataMaster(HttpClient client, string importDataMasterName, ref bool content)
        {
            Guid importDataMasterId = Guid.Empty;
            content = true;
            try
            {
                if (!string.IsNullOrEmpty(importDataMasterName))
                {
                    HttpResponseMessage response = client.GetAsync("/api/data/v9.1/ims_importdatamasters?$select=ims_content,ims_importdatamasterid,ims_name&$filter=ims_name eq '" + importDataMasterName + "' and  statecode eq 0 and ims_contenttype eq 1").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        JObject collection = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                        JToken valArray;
                        if (collection.TryGetValue("value", out valArray))
                        {
                            JArray entities = (JArray)valArray;
                            foreach (JObject entity in entities)
                            {
                                JToken tokenContent = entity.GetValue("ims_content");
                                JToken tokenIDMId = entity.GetValue("ims_importdatamasterid");
                                if (tokenContent.Type != JTokenType.Null)
                                {
                                    content = (bool)tokenContent;
                                }
                                if (tokenIDMId.Type != JTokenType.Null)
                                {
                                    importDataMasterId = (Guid)tokenIDMId;
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new Exception(response.ReasonPhrase + "Status Code " + response.StatusCode + "Content" + response.Content + leadimportJson.ExtractErrorMessage(response) + Environment.NewLine);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching Import Data Master: " + ex.Message + ex.InnerException + Environment.NewLine);
            }
            return importDataMasterId;
        }

        public static List<Mapping> FetchMappings(HttpClient client, string importDataMasterId)
        {
            List<Mapping> mappings = null;
            try
            {
                if (!string.IsNullOrEmpty(importDataMasterId))
                {
                    importDataMasterId = importDataMasterId.Replace('{', ' ').Replace('}', ' ').Trim();
                    HttpResponseMessage response = client.GetAsync("/api/data/v9.1/ims_importdetailsmappings?$select=ims_maxlength,ims_sourcefield,ims_targetfield,ims_classname&$filter=_ims_importdatamaster_value eq " + importDataMasterId + " and  statecode eq 0").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        JObject collection = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                        JToken valArray;
                        if (collection.TryGetValue("value", out valArray))
                        {
                            mappings = new List<Mapping>();
                            JArray entities = (JArray)valArray;
                            foreach (JObject entity in entities)
                            {
                                Mapping objMapping = new Mapping();
                                JToken sourceField = entity.GetValue("ims_sourcefield");
                                JToken targetField = entity.GetValue("ims_targetfield");
                                JToken maxLength = entity.GetValue("ims_maxlength");
                                JToken className = entity.GetValue("ims_classname");
                                if (sourceField.Type != JTokenType.Null)
                                {
                                    objMapping.Source = sourceField.ToString();
                                }
                                if (targetField.Type != JTokenType.Null)
                                {
                                    objMapping.Target = targetField.ToString();
                                }
                                if (className.Type != JTokenType.Null)
                                {
                                    objMapping.className = className.ToString();
                                }
                                if (maxLength.Type != JTokenType.Null)
                                {
                                    objMapping.MaxLengthAllowed = (int)maxLength;
                                }
                                else
                                {
                                    objMapping.MaxLengthAllowed = 0;
                                }
                                mappings.Add(objMapping);
                            }
                        }
                    }
                    else
                    {
                        throw new Exception(response.ReasonPhrase + "Status Code " + response.StatusCode + "Content" + response.Content + "Error:" + leadimportJson.ExtractErrorMessage(response) + Environment.NewLine);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching Mappings: " + ex.Message + ex.InnerException + Environment.NewLine);
            }

            return mappings;
        }
        public static string ExtractErrorMessage(HttpResponseMessage response)
        {
            string errorMsg = string.Empty;
            JObject errorObject = JObject.Parse(response.Content.ReadAsStringAsync().Result);
            errorMsg = errorObject["error"]["message"].ToString();
            return errorMsg;
        }

		public static JObject flattenNestedJSON(JObject nestedJSON)
		{
			JObject jsonObject = nestedJSON;
			Dictionary<string, string> myDict = new Dictionary<string, string>();
			IEnumerable<JToken> jTokens = jsonObject.Descendants().Where(p => p.Count() == 0);
			Dictionary<string, string> results = jTokens.Aggregate(new Dictionary<string, string>(), (properties, jToken) =>
			{
				properties.Add(jToken.Path, jToken.ToString());
				return properties;

			});
			if (results.Count > 0)
			{
				foreach (var entry in results)
				{
					// do something with entry.Value or entry.Key'
					string key = string.Empty;
					string val = string.Empty;
					val = entry.Value;
					key = entry.Key;
					if (key.Contains('.'))
					{
						key = key.Replace('.', '_');
					}

					myDict.Add(key, val);

				}
			}
			JObject jobj = JObject.Parse(JsonConvert.SerializeObject(myDict));
			return jobj;
			//Console.WriteLine(myDict);
			////var data = (results.Values);
			//Console.ReadKey();
		}
    }

    public class Constants
    {
        public const string configSetUpName = "LeadPops";
        public const string invalidQuerystring = "Invalid Querystring";
        public const string invalidRequestBody = "Invalid Request Body";
        public const string LeadSourceNotFound = "LeadSourceNotFound";
        public const string DailyThreshold = "DailyThreshold";
        public const string DailyWarningLimit = "DailyWarningLimit";
    }

    public static class LeadStaging
    {
        public const string EntityName = "ims_leadstaging";
        public const string EntityCollectionName = "ims_leadstagings";

        #region Attributes

        /// <summary>Type: Uniqueidentifier, RequiredLevel: SystemRequired</summary>
        public const string PrimaryKey = "ims_leadstagingid";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string PrimaryName = "ims_lastname";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string ActiveDutyMilitary = "ims_activedutymilitary";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string AnnualIncome = "ims_annualincome";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string BirthDay = "ims_birthday";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string BusinessPhone = "ims_officephone";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string City = "ims_city";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: account</summary>
        public const string Co_Borrower = "ims_coborrower";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string Co_BorrowerBirthday = "ims_spousebirthday";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string Co_BorrowerEmail = "ims_spouseemail";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string Co_BorrowerFirstName = "ims_spousefirstname";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string Co_BorrowerFullName = "ims_coborrowerfullname";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string Co_BorrowerLastName = "ims_spouselastname";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string Co_BorrowerPhoneCell = "ims_spousephonecell";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string Co_BorrowerPhoneOffice = "ims_spousephoneoffice";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string ContactExternalId = "ims_contactexternalid";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 418, Format: Text</summary>
        public const string ContactGroupExternalID = "ims_contactgroupexternalid";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: systemuser</summary>
        public const string CreatedBy = "createdby";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateAndTime, DateTimeBehavior: UserLocal</summary>
        public const string CreatedOn = "createdon";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string CreditRating = "ims_creditrating";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string CreditScore = "ims_creditscore";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string CreditScoreDate = "ims_creditscoredate";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string CreditScoreExpiryDate = "ims_creditscoreexpirydate";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string CreditScoreInfo = "ims_creditscoreinfo";
        /// <summary>Type: Memo, RequiredLevel: None, MaxLength: 2000</summary>
        public const string Description = "ims_message";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string DisabilityIncome = "ims_disabilityincome";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string DoNotCallFlag = "ims_oktocall";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string DoNotEmailFlag = "ims_oktoemail";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string DoNotMailFlag = "ims_oktomail";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string DoNotSMSFlag = "ims_oktosms";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string DownPayment = "ims_downpayment";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string DownPaymentAmount = "ims_downpaymentamount";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string Employer = "ims_employer";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 250, Format: Text</summary>
        public const string EmployerAddress = "ims_employeraddress";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 250, Format: Text</summary>
        public const string EmployerAddress2 = "ims_employeraddress2";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string EmployerCity = "ims_employercity";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string EmployerLicenseNumber = "ims_employerlicensenumber";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string EmployerState = "ims_employerstate";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string EmployerZIP_PostalCode = "ims_employerzip";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string EmploymentStatus = "ims_employmentstatus";
        /// <summary>Type: Memo, RequiredLevel: None, MaxLength: 2000</summary>
        public const string ErrorLog = "ims_errorlog";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 300, Format: Text</summary>
        public const string FiledBankruptcy = "ims_filedbankruptcy";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string FirstName = "ims_firstname";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string FirstPropertyPurchase = "ims_firstpropertypurchase";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string FundedDate = "ims_fundeddate";
        /// <summary>Type: Memo, RequiredLevel: None, MaxLength: 15089</summary>
        public const string GeneralNoteInfo = "ims_generalnoteinfo";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string HasAgent = "ims_hasagent";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string HasContract = "ims_hascontract";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string HomePhone = "ims_homephone";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string ImportProcessName = "ims_importprocessname";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string IsMajorRenovation = "ims_ismajorrenovation";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string IsPropertyNearby = "ims_ispropertynearby";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: lead</summary>
        public const string Lead = "ims_lead";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string LeadCreationDate = "ims_creationdate";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string LeadCreditScoreMin = "ims_creditscoremin";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string LeadLastModifiedDate = "ims_lastmodifieddate";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string LeadSource = "ims_source";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string LeadStatus = "ims_leadstatus";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string LicenseNumber = "ims_licensenumber";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string LOExternalID = "ims_loexternalid";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string LoanAmountMin = "ims_loanamtmin";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string LoanNumber = "ims_loannumber";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string LoanOfficerEmail = "ims_loemail";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string LoanOfficerFirstName = "ims_lofirstname";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string LoanOfficerLastName = "ims_lolastname";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string LoanPurpose = "ims_loanpurpose";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string LoanRate = "ims_loanrate";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string LoanStatus = "ims_loanstatus";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string LoanType = "ims_renovationloantype";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string Married = "ims_married";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string MaxLoanAmount = "ims_loanamtmax";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string MID = "ims_leadmid";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string MobilePhone = "ims_phone";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: systemuser</summary>
        public const string ModifiedBy = "modifiedby";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateAndTime, DateTimeBehavior: UserLocal</summary>
        public const string ModifiedOn = "modifiedon";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string NickName = "ims_nickname";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 200, Format: Text</summary>
        public const string Occupation = "ims_occupation";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string OfficialEmail = "ims_workemail";
        /// <summary>Type: Boolean, RequiredLevel: None, True: 1, False: 0, DefaultValue: False</summary>
        public const string OneTimeDataMigration = "ims_onetimedatamigration";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: contact</summary>
        public const string OtherContact = "ims_othercontact";
        /// <summary>Type: Owner, RequiredLevel: SystemRequired, Targets: systemuser,team</summary>
        public const string Owner = "ownerid";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string PersonalEmail = "ims_email";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 300, Format: Text</summary>
        public const string ProofOfIncome = "ims_proofofincome";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string PropertyAddress1 = "ims_propertyaddress1";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string PropertyAddress2 = "ims_propertyaddress2";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string PropertyCity = "ims_propertycity";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string PropertyState = "ims_propertystate";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string PropertyType = "ims_propertytype";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string PropertyUse = "ims_propertyuse";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string PropertyZip = "ims_propertyzip";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string PurchaseIntent = "ims_purchaseintent";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string PurchasePrice = "ims_purchaseprice";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string PurchaseTimeframe = "ims_renovationdate";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string PurchaseYear = "ims_purchasedate";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string RateDesired = "ims_ratedesired";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string ReferredBy = "ims_referredby";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string ReferredTo = "ims_referredto";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string RequestedAPR = "ims_requestedapr";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string RequestedProduct = "ims_requestedproduct";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string RequestedRate = "ims_requestedrate";
        /// <summary>Type: Boolean, RequiredLevel: None, True: 1, False: 0, DefaultValue: False</summary>
        public const string Retry = "ims_retry";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string Salutation = "ims_salutation";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string ShowProof = "ims_showproof";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string State = "ims_state";
        /// <summary>Type: State, RequiredLevel: SystemRequired, DisplayName: Status, OptionSetType: State</summary>
        public const string Status = "statecode";
        /// <summary>Type: Status, RequiredLevel: None, DisplayName: Status Reason, OptionSetType: Status</summary>
        public const string StatusReason = "statuscode";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 250, Format: Text</summary>
        public const string Street1 = "ims_address";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 250, Format: Text</summary>
        public const string Street2_UnitNo = "ims_address2";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string Suffix = "ims_suffix";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string Title = "ims_title";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string User = "ims_user";
        /// <summary>Type: Boolean, RequiredLevel: None, True: 1, False: 0, DefaultValue: True</summary>
        public const string ValidationStatus = "ims_validationstatus";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string WantsLoan = "ims_wantsloan";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string WeddingAnniversaryDate = "ims_weddinganniversarydate";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string WorkingWith = "ims_workingwith";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string ZIP_PostalCode = "ims_zipcode";
        public const string RequestContent = "ims_requestcontent";

        #endregion Attributes

        #region OptionSets

        public enum Status_OptionSet
        {
            Active = 0,
            Inactive = 1
        }
        public enum StatusReason_OptionSet
        {
            Active = 1,
            Inactive = 2
        }

        #endregion OptionSets
    }

    public static class ErrorLog
    {
        public const string EntityName = "ims_errorlog";
        public const string EntityCollectionName = "ims_errorlogs";

        #region Attributes

        /// <summary>Type: Uniqueidentifier, RequiredLevel: SystemRequired</summary>
        public const string PrimaryKey = "ims_errorlogid";
        /// <summary>Type: String, RequiredLevel: ApplicationRequired, MaxLength: 100, Format: Text</summary>
        public const string PrimaryName = "ims_name";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: systemuser</summary>
        public const string CreatedBy = "createdby";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateAndTime, DateTimeBehavior: UserLocal</summary>
        public const string CreatedOn = "createdon";
        /// <summary>Type: Memo, RequiredLevel: None, MaxLength: 5000</summary>
        public const string ErrorDetails = "ims_errordetails";
        /// <summary>Type: Memo, RequiredLevel: None, MaxLength: 10000</summary>
        public const string LeadJSONData = "ims_leadjsondata";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string LeadProvider = "ims_leadprovider";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: systemuser</summary>
        public const string ModifiedBy = "modifiedby";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateAndTime, DateTimeBehavior: UserLocal</summary>
        public const string ModifiedOn = "modifiedon";
        /// <summary>Type: Owner, RequiredLevel: SystemRequired, Targets: systemuser,team</summary>
        public const string Owner = "ownerid";
        /// <summary>Type: State, RequiredLevel: SystemRequired, DisplayName: Status, OptionSetType: State</summary>
        public const string Status = "statecode";
        /// <summary>Type: Status, RequiredLevel: None, DisplayName: Status Reason, OptionSetType: Status</summary>
        public const string StatusReason = "statuscode";
        public const string DailyWarningLimitReached = "ims_dailywarninglimitreached";

        #endregion Attributes

        #region OptionSets

        public enum Status_OptionSet
        {
            Active = 0,
            Inactive = 1
        }
        public enum StatusReason_OptionSet
        {
            Active = 1,
            Inactive = 2
        }

        #endregion OptionSets
    }

    public class Mapping
    {
        public string Source;
        public string Target;
        public int MaxLengthAllowed;
        public string value;
        public string className;
    }
}
