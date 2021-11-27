using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Infy.MS.BlendIntegration
{
    public static class BlendMovehome
    {
        [FunctionName("BlendMovehome")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous,"post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");


            string basicEndPoint = "basicEndPoint;https://api.beta.blendlabs.com/home-lending/applications/{blendApplicationId}";
            string partiesEndPoint = "partiesEndPoint;https://api.beta.blendlabs.com/home-lending/applications/{blendApplicationId}/parties";
            string employersEndPoint = "employersEndPoint;https://api.beta.blendlabs.com/parties/{blendApplicationId}/employers";
            string assigneeEndPoint = "assigneeEndPoint;https://api.beta.blendlabs.com/lenders/?ids={blendApplicationId}";
            string followupEndPoint = "followupEndPoint;https://api.beta.blendlabs.com/follow-ups?applicationId={blendApplicationId}";

            //string basicEndPoint = "basicEndPoint;https://api.blendlabs.com/home-lending/applications/{blendApplicationId}";
            //string partiesEndPoint = "partiesEndPoint;https://api.blendlabs.com/home-lending/applications/{blendApplicationId}/parties";
            //string employersEndPoint = "employersEndPoint;https://api.blendlabs.com/parties/{blendApplicationId}/employers";
            //string assigneeEndPoint = "assigneeEndPoint;https://api.blendlabs.com/lenders/?ids={blendApplicationId}";
            //string followupEndPoint = "followupEndPoint;https://api.blendlabs.com/follow-ups?applicationId={blendApplicationId}";
            HttpClient HttpClient = GetHttpClientforCRM(log);
            string[] endPoints = { basicEndPoint ,partiesEndPoint,employersEndPoint, assigneeEndPoint, followupEndPoint };
            dynamic requestBody = await req.Content.ReadAsAsync<object>();
           
            string blendAction = requestBody?.data.action;
            string blendApplicationId = string.Empty;
            bool blendResult = false;
            string partyId = string.Empty;
            string assigneeId = string.Empty;
            string mainBlendApplicationId = string.Empty;
            bool followUpsExists = false;

            if (blendAction == "healthcheck")
            {
                log.Info("Blend Health Check Request");
                blendResult = true;
            }
            else if (blendAction == "updated" || blendAction == "created")
            {
                log.Info("Blend action is from " + blendAction + " Request");
                //log.Info(JsonConvert.SerializeObject(requestBody, Formatting.Indented));
                if (blendAction == "created")
                {
                    blendApplicationId = requestBody?.data.fields.application.id;
                    mainBlendApplicationId = blendApplicationId;
                }
                else if (blendAction == "updated")
                {
                    blendApplicationId = requestBody?.data.id;
                    mainBlendApplicationId = blendApplicationId;
                }
                if (blendAction != string.Empty)
                {
                    if (endPoints.Count() > 0)
                    {
                        JObject jObjectLeadStaging = new JObject();
                        foreach (var endPoint in endPoints)
                        {
                            var endPointSplit = endPoint.Split(';');
                            if (endPointSplit[0] == "employersEndPoint" && partyId != string.Empty)
                            {
                                blendApplicationId = partyId;
                            }
                            if (endPointSplit[0] == "assigneeEndPoint" && assigneeId != string.Empty)
                            {
                                blendApplicationId = assigneeId;
                            }
                            if (endPointSplit[0] == "followupEndPoint" && mainBlendApplicationId != string.Empty)
                            {
                                blendApplicationId = mainBlendApplicationId;
                            }
                            JObject blendLeadData = await getLead(blendApplicationId, "", log, endPointSplit[1]);
                            JObject falttenedJObject = flattenNestedJSON(blendLeadData);
                            if (endPointSplit[0] == "followupEndPoint")
                            {
                                if (falttenedJObject.GetValue("followUps") == null)
                                {
                                    followUpsExists = true;
                                }
                            }
                            if (endPointSplit[0] == "basicEndPoint")
                            {
                                if(falttenedJObject["applicationSource.name"]!=null && falttenedJObject.GetValue("applicationSource.name").ToString()=="MoveHome")
                                {
                                    if (blendAction == "created")
                                    {
                                        blendResult = false;
                                        return blendResult != true ? req.CreateResponse(HttpStatusCode.BadRequest, "Blend Request is failed to process, because this application is triggered from Movehome no need to create") : req.CreateResponse(HttpStatusCode.OK, "Blend Request Sucessfully Processed");
                                    }
                                    else if(blendAction == "updated")
                                    {
                                        jObjectLeadStaging["ims_blendapplicationsource"] = "MovehomeCRM";
                                    }
                                }
                                
                                if (falttenedJObject["parties[0].id"] != null)
                                    partyId = falttenedJObject.GetValue("parties[0].id").ToString();//where we can get the Party Id to get the Employers Endpoint;
                                if (falttenedJObject["assignees[0].userId"] != null)
                                    assigneeId = falttenedJObject.GetValue("assignees[0].userId").ToString();
                                if (endPointSplit[0] == "basicEndPoint" && string.IsNullOrEmpty(falttenedJObject["parties[0].name.firstName"].ToString())&& string.IsNullOrEmpty( falttenedJObject["parties[0].name.lastName"].ToString()))
                                {
                                    blendResult = false;
                                    log.Info("Blend Request is failed to process, First Name and Last Name having NULL values");
                                    return blendResult != true ? req.CreateResponse(HttpStatusCode.BadRequest, "Blend Request is failed to process, First Name and Last Name having NULL values") : req.CreateResponse(HttpStatusCode.OK, "Blend Request Sucessfully Processed");
                                }
                                jObjectLeadStaging["ims_blendapplicationguid"] = blendApplicationId;
                            }
                            blendResult = processInput(HttpClient,falttenedJObject, log, endPointSplit[0], ref jObjectLeadStaging);
                        }
                        
                        if (jObjectLeadStaging.GetValue("ims_blendapplicationstatus") != null && jObjectLeadStaging.GetValue("ims_blendapplicationstatus").ToString() == "TAKEN_OVER" || jObjectLeadStaging.GetValue("ims_blendapplicationstatus").ToString() == "SUBMITTED")
                        {
                            jObjectLeadStaging["ims_leadstatus"] = "SUBMITTED";
                        }
                        if(jObjectLeadStaging.GetValue("ims_applicationcompleteddate")!=null && jObjectLeadStaging.GetValue("ims_leadstatus").ToString() == "SUBMITTED")
                        {
                            jObjectLeadStaging["ims_leadstatus"] = "SUBMITTED_App Exported";
                        }
                        else if(jObjectLeadStaging.GetValue("ims_applicationpreapprovedate")!=null && jObjectLeadStaging.GetValue("ims_leadstatus").ToString() == "SUBMITTED")
                        {
                            jObjectLeadStaging["ims_leadstatus"] = "SUBMITTED_Pre-approved";
                        }
                        else if (jObjectLeadStaging.GetValue("ims_applicationcompleteddate") == null&& followUpsExists && jObjectLeadStaging.GetValue("ims_leadstatus").ToString() == "SUBMITTED")
                        {
                            jObjectLeadStaging["ims_leadstatus"] = "SUBMITTED_Items Needed";
                        }
                       
                        if (jObjectLeadStaging["ims_loemail"] != null && !string.IsNullOrEmpty(jObjectLeadStaging.GetValue("ims_loemail").ToString()))
                        {
                            var loEmail = jObjectLeadStaging.GetValue("ims_loemail").ToString();
                          var roleCheck=  ValidateLO(HttpClient, loEmail);
                            if(!roleCheck)
                            {
                                log.Info("Loan Officer does not have any security roles assigned in movehome");
                                jObjectLeadStaging.Add("ims_loexternalid", JToken.FromObject(loEmail));
                                jObjectLeadStaging.Remove("ims_loemail");
                            }
                        }
                        Guid recordId = CreateLeadStaging(HttpClient, log, null, null, jObjectLeadStaging);
                    }
                }
            }
            
            return blendResult != true? req.CreateResponse(HttpStatusCode.BadRequest, "Blend Request is failed to process"): req.CreateResponse(HttpStatusCode.OK, "Blend Request Sucessfully Processed");
        }
        private static async Task<JObject> getLead(string leadId, string access_token, TraceWriter log,string reqURL)
        {
            log.Info("Get Blend Lead Record Triggered.");
            log.Info("leadId  :" + leadId);
            log.Info(reqURL);
            using (var client = new HttpClient())
            {
                //specify to use TLS 1.2 as default connection
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("blend-api-version", "4.3.0");
                client.DefaultRequestHeaders.Add("blend-target-instance", "movement");
                //client.DefaultRequestHeaders.Add("Authorization","Basic " + Convert.ToBase64String(Encoding.Default.GetBytes("e45858b01d17461fbca5c1bb871ba93f:gdf+EVKtalCMhlPTT+9RpDkMqxRjmNnJ")));
                client.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes("7129007bb6944a67aa3f8ca93c691e1c:ClAjl54Taq/xZP40zTtlo06lU4bc+k7J")));
                //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + "YXBpLXNlY3JldC10b2tlbjphYTZhMGUyNjc5ODA0NWMwOTkxNDMzNjdmZTNkZmU3ZDpabDduNFlaZFNmak9kVmZ4QlU5eEFqZmtPNEZXeWlsaQ");
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "7129007bb6944a67aa3f8ca93c691e1c:ClAjl54Taq/xZP40zTtlo06lU4bc+k7J");
                if (reqURL.Contains("{blendApplicationId}"))
                    reqURL = reqURL.Replace("{blendApplicationId}", leadId);
                HttpResponseMessage response = await client.GetAsync(reqURL);
                //response.EnsureSuccessStatusCode();
                var resp = await response.Content.ReadAsStringAsync();
                JObject resultObject = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                return resultObject;


            }

        }

        private static async Task<AccessToken> GetToken(TraceWriter log)
        {
            log.Info("Get TE Access Token Triggered.");
            string clientId = "YOUR CLIENT ID";
            string clientSecret = "YOUR CLIENT SECRET";
            string credentials = String.Format("{0}:{1}", clientId, clientSecret);

            using (var client = new HttpClient())
            {
                //specify to use TLS 1.2 as default connection
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                //Define Headers
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                // client.Timeout = -1;

                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials)));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "Zjg4YmY2ZTc4N2E2OGY3YWJhNzhmNTBkZGIzYTRiM2Y6YzlhZjczNTg3ZGMzMGE1NTZkODU5YzNhYjNlNGY0ZGJjMzI3YmMwNmVlMDkwZWFmNDA0MjkxYmI2YzQyYjA2Mw==");
                //Prepare Request Body
                List<KeyValuePair<string, string>> requestData = new List<KeyValuePair<string, string>>();
                requestData.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));

                FormUrlEncodedContent requestBody = new FormUrlEncodedContent(requestData);


                //Request Token
                var request = await client.PostAsync("https://public.move-stg.totalexpert.net/v1/token", requestBody);
                var response = await request.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<AccessToken>(response);
            }
        }

        public static bool processInput(HttpClient HttpClient, JObject inputData, TraceWriter log,string endPointSourceName, ref JObject leadStaging)
        {
            string client = string.Empty;
            bool isException = false;
            string requestBodyJson = string.Empty;
            try
            {

                Guid importDataMasterId = Guid.Empty;
                bool reuestContent = false;
                importDataMasterId = GetImportDataMaster(HttpClient, "BlendMovehome", ref reuestContent);
                if (inputData != null)
                {
                    requestBodyJson = JsonConvert.SerializeObject(inputData, Formatting.Indented);
                }
                //Fetch Import details mappings
                List<Mapping> mappings = null;
                if (importDataMasterId != Guid.Empty)
                {
                    mappings = FetchMappings(HttpClient, importDataMasterId.ToString(), endPointSourceName);
                    FormLeadStaging(inputData, mappings, ref leadStaging);
                }
                
            }
            catch (Exception ex)
            {
                isException = true;
                log.Info("Error:" + ex.Message + ex.InnerException);
                //Write Error in Error Log Entity
                CreateErrorLog(HttpClient, client, "BlendMovehome", null, requestBodyJson, ex.Message + ex.InnerException, log);
            }

            if (isException)
            {
                return false;
            }
            else
            {
                return true;
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
                authority = Environment.GetEnvironmentVariable("authority");
                apiUrl = Environment.GetEnvironmentVariable("crmApiUrl");
                //clientID = "dbbfed03-5023-410b-b588-8db70a61c8af";

                //clientsecret = "rFzxuRtO20n@BywqSIn_i7Y]5Q]nnAMQ";

                //apiUrl = "https://mmdevphase2.crm.dynamics.com";

                //authority = "https://login.microsoftonline.com/095f0976-bf66-4d76-bf29-0fbab0884ecb";
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
        public static Guid CreateLeadStaging(HttpClient client, TraceWriter log, JObject requestBody, List<Mapping> mappings, JObject newLeadStaging)
        {
            // bool dataFound = false;
            Guid leadStagingId = Guid.Empty;
            //Lead Source, Request Content && Import Process Mapping
            newLeadStaging.Add(LeadStaging.ImportProcessName, JToken.FromObject("BlendMovehome"));
            newLeadStaging.Add(LeadStaging.FromBlend, JToken.FromObject("True"));
            //newLeadStaging.Add(LeadStaging.RequestContent, JToken.FromObject(JsonConvert.SerializeObject(requestBody, Formatting.Indented)));
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
                throw new Exception("Error while creating Lead Staging Record: " + createResponse.ReasonPhrase + " " + createResponse.StatusCode + " Content" + createResponse.Content + BlendMovehome.ExtractErrorMessage(createResponse));
            //}
            return leadStagingId;
        }

        public static string GetValueFromSource(JObject requestBody, Mapping mapping)
        {
            string value = string.Empty;
            //Get Value from Request Body

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
            else if (requestBody[mapping.Source] != null)
            {
                value = requestBody[mapping.Source].ToString();
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
            log.Info(reqBodyJson);
            if (dailyWarningLimit) errorLog.Add(ErrorLog.DailyWarningLimitReached, JToken.FromObject(dailyWarningLimit));

            HttpResponseMessage createResponse =
                httpClient.SendAsJsonAsync(HttpMethod.Post, "api/data/v9.1/ims_errorlogs", errorLog).Result;

            if (!createResponse.IsSuccessStatusCode)
            {
                log.Info("Error While Creating Error Log record:" + createResponse.ReasonPhrase + " Status Code:" + createResponse.StatusCode + " Content" + createResponse.Content + BlendMovehome.ExtractErrorMessage(createResponse));
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
                        throw new Exception(response.ReasonPhrase + "Status Code " + response.StatusCode + "Content" + response.Content + BlendMovehome.ExtractErrorMessage(response) + Environment.NewLine);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching Import Data Master: " + ex.Message + ex.InnerException + Environment.NewLine);
            }
            return importDataMasterId;
        }

        public static List<Mapping> FetchMappings(HttpClient client, string importDataMasterId,string endpointSourceName=null)
        {
            List<Mapping> mappings = null;
            try
            {
                if (!string.IsNullOrEmpty(importDataMasterId))
                {
                    importDataMasterId = importDataMasterId.Replace('{', ' ').Replace('}', ' ').Trim();
                    HttpResponseMessage response = client.GetAsync("/api/data/v9.1/ims_importdetailsmappings?$select=ims_maxlength,ims_sourcefield,ims_targetfield,ims_classname&$filter=_ims_importdatamaster_value eq " + importDataMasterId + " and ims_sourceentity eq '"+endpointSourceName +"' and statecode eq 0").Result;
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
                        throw new Exception(response.ReasonPhrase + "Status Code " + response.StatusCode + "Content" + response.Content + "Error:" + BlendMovehome.ExtractErrorMessage(response) + Environment.NewLine);
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

                    //key=Regex.Replace(key, @"\d", "");
                    //key = key.Replace("[", "").Replace("]","");
                    if (key.Contains('['))
                    {
                        // key = key.Replace('.', '_');
                    }

                    myDict.Add(key, val);

                }
            }
            JObject jobj = JObject.Parse(JsonConvert.SerializeObject(myDict));
            return jobj;
        }

        

        public static JObject FormLeadStaging(JObject requestBody, List<Mapping> mappings,ref JObject leadStaging)
        {
            foreach (Mapping mapping in mappings)
            {
                string value = GetValueFromSource(requestBody, mapping);
                if (!string.IsNullOrEmpty(value))
                {
                    JToken keyExist = leadStaging[mapping.Target];
                    if (keyExist == null)
                    {
                        leadStaging.Add(mapping.Target, JToken.FromObject(value));
                    }
                }
            }
            return leadStaging;
        }

        public static bool ValidateLO(HttpClient client, string loemail)
        {
            string loSecurityRoles = string.Empty;
            if (!string.IsNullOrEmpty(loemail))
            {
                string fetchXml = "<fetch mapping='logical'>" +
  "<entity name='role'>" +
    "<attribute name='name' />" +
    "<attribute name='businessunitid' />" +
    "<attribute name='roleid' />" +
    "<order attribute='name' descending='false' />" +
    "<link-entity name='systemuserroles' from='roleid' to='roleid' visible='false' intersect='true'>" +
      "<link-entity name='systemuser' from='systemuserid' to='systemuserid' alias='ad'>" +
        "<filter type='and'>" +
          "<condition attribute='domainname' operator='eq' value='" + loemail + "' />" +
        "</filter>" +
      "</link-entity>" +
    "</link-entity>" +
  "</entity>" +
"</fetch>";

                string configFetchXml = "<fetch mapping='logical'>" +
  "<entity name='ims_configuration'>" +
    "<attribute name='ims_configurationid' />" +
    "<attribute name='ims_name' />" +
    "<attribute name='createdon' />" +
    "<attribute name='ims_valuemultiline' />" +
    "<order attribute='ims_name' descending='false' />" +
    "<filter type='and'>" +
      "<condition attribute='ims_name' operator='eq' value='LOSecurityRoles' />" +
    "</filter>" +
  "</entity>" +
"</fetch>"; HttpResponseMessage configResponseMessage =
                        client.GetAsync("api/data/v9.1/ims_configurations?fetchXml=" + WebUtility.UrlEncode(configFetchXml), HttpCompletionOption.ResponseHeadersRead).Result;
                if (configResponseMessage.IsSuccessStatusCode) //200
                {
                    JObject collection = JObject.Parse(configResponseMessage.Content.ReadAsStringAsync().Result);
                    JToken valArray;
                    if (collection.TryGetValue("value", out valArray))
                    {
                        JArray entities = (JArray)valArray;
                        foreach (JObject entity in entities.ToList())
                        {
                            if (entity["ims_valuemultiline"] != null && !string.IsNullOrEmpty(entity["ims_valuemultiline"].ToString()))
                            {
                                loSecurityRoles = entity.GetValue("ims_valuemultiline").ToString();
                            }
                        }
                    }
                }
                HttpResponseMessage responseMessage =
           client.GetAsync("api/data/v9.1/roles?fetchXml=" + WebUtility.UrlEncode(fetchXml), HttpCompletionOption.ResponseHeadersRead).Result;
                if (responseMessage.IsSuccessStatusCode) //200
                {
                    JObject collection = JObject.Parse(responseMessage.Content.ReadAsStringAsync().Result);
                    JToken valArray;
                    if (collection.TryGetValue("value", out valArray))
                    {
                        JArray entities = (JArray)valArray;
                        foreach (JObject entity in entities.ToList())
                        {
                            if (entity["name"] != null && !string.IsNullOrEmpty(entity["name"].ToString()))
                            {
                                if (!string.IsNullOrEmpty(loSecurityRoles) && loSecurityRoles.Contains(entity.GetValue("name").ToString()))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
                return false;
            }
            return false;
        }

        //public IOrganizationService GetCrmService(string accessToken)
        //{
        //    Uri serviceUrl = new Uri(ConfigurationManager.AppSettings[Constants.ServiceUrl].ToString() + ConfigurationManager.AppSettings["orgUrl"].ToString());
        //    using (var sdkService = new OrganizationWebProxyClient(serviceUrl, false))
        //    {
        //        sdkService.HeaderToken = accessToken;
        //        var orgService = (IOrganizationService)sdkService;
        //        return orgService;
        //    }
        //}
    }
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
        public const string FromTE = "ims_fromte";
        public const string FromBlend = "ims_fromblend";

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

    public class AccessToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public long expires_in { get; set; }
    }
}
