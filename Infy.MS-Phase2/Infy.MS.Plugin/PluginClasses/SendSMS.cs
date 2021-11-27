using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Infy.MS.Plugins
{
    public class SendSMS : IPlugin
    {

        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the tracing service
            ITracingService tracingService =
            (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.  
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            // The InputParameters collection contains all the data passed in the message request.  
            if (context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is Entity)
            {
                // Obtain the target entity from the input parameters.  
                Entity entity = (Entity)context.InputParameters["Target"];

                // Obtain the organization service reference which you will need for   ownerid ims_messagetext
                // web service calls.  
                IOrganizationServiceFactory serviceFactory =
                    (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                try
                {
                    //check for Incoming SMS
                    if (entity.Attributes.Contains("ims_direction") && entity.GetAttributeValue<Boolean>("ims_direction") == false)
                    {
                        //return if it is an incoming SMS.
                        return;
                    }
                    if (context.MessageName == "Create")///applies on SMS with auto send enabled.
                    {
                        if (entity.Attributes.Contains("ims_autosend") && entity.GetAttributeValue<Boolean>("ims_autosend") == true)
                        {
                            if (entity.Attributes.Contains("statuscode"))
                            {
                                //Ideally the value of statecode at this point wont be '176390006'.
                                entity.Attributes["statuscode"] = new OptionSetValue(176390006); /// Updating the value to make the below if statement to True.

                            }
                        }
                    }
                    // throw new InvalidPluginExecutionException(entity.GetAttributeValue<OptionSetValue>("statuscode").Value.ToString());
                    if (entity.Attributes.Contains("statuscode") && entity.GetAttributeValue<OptionSetValue>("statuscode").Value == 176390006)
                    {
                        RetrieveRequest request = new RetrieveRequest();
                        request.ColumnSet = new ColumnSet(new string[] { "ims_to", "ims_messagetext", "ownerid" });
                        request.Target = new EntityReference(entity.LogicalName, entity.Id);

                        //Retrieve the entity 
                        Entity smsActivity = (Entity)((RetrieveResponse)service.Execute(request)).Entity;
                        if (smsActivity.Attributes.Contains("ims_to") && smsActivity.Attributes.Contains("ims_messagetext"))
                        {
                            string ownerMobileNumber = null;
                            if (!smsActivity.Attributes.Contains("ims_from"))
                            {
                                //checking owner/user record for the business phone number for sending SMS
                                EntityReference owner = smsActivity.GetAttributeValue<EntityReference>("ownerid");
                                RetrieveRequest retriveOwner = new RetrieveRequest();
                                retriveOwner.ColumnSet = new ColumnSet(new string[] { "ims_businessphonenumber" });
                                retriveOwner.Target = new EntityReference(owner.LogicalName, owner.Id);
                                Entity ownerEntity = (Entity)((RetrieveResponse)service.Execute(retriveOwner)).Entity;
                             
                                if (ownerEntity.Attributes.Contains("ims_businessphonenumber") && ownerEntity.GetAttributeValue<string>("ims_businessphonenumber") != null)
                                {
                                    ownerMobileNumber = ownerEntity.GetAttributeValue<string>("ims_businessphonenumber");
                                }
                            }
                            else
                            {
                                ownerMobileNumber = smsActivity.GetAttributeValue<string>("ims_from");
                            }
                            try
                            {

                                MessageResource messageResponse = send(service, ownerMobileNumber, smsActivity.GetAttributeValue<string>("ims_to"), smsActivity.GetAttributeValue<string>("ims_messagetext"));
                                if (messageResponse != null)
                                {
                                    //Immediate status after sending SMS to twilio will be 'queued'.Updating the same to the SMS record in CRM
                                    if (messageResponse.Status.ToString() == "queued")
                                    {
                                        updateSMS(service, smsActivity, 1, 176390003, "ims_sid", messageResponse.Sid, messageResponse.From.ToString());
                                    }
                                    //If status after sending SMS to twilio is 'sent'.Updating the same to the SMS record in CRM
                                    else if (messageResponse.Status.ToString() == "sent")// 
                                    {
                                        updateSMS(service, smsActivity, 1, 2, "ims_sid", messageResponse.Sid, messageResponse.From.ToString());
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                //0 Open  
                                //1 Completed 
                                //2 Canceled  
                                //3 Scheduled 

                                //Console.WriteLine(ex.Message);
                                //throw new InvalidPluginExecutionException(ex.Source+ex.Message);
                                if (ex.Source == "Twilio" || ex.Source == "Infy.MS.SMS")
                                {

                                    //If sending SMS is failed , the status and reason will be reported by twilio in error object.Updating the same to the SMS record in CRM
                                    updateSMS(service, smsActivity, 2, 3, "ims_twilionotes", ex.Message, null);
                                }
                                else if (ex.Source == "Infy.MS.SMS")
                                {
                                    string errormessage = "Some error occured." + ex.InnerException.ToString();
                                    updateSMS(service, smsActivity, 2, 3, "ims_twilionotes", errormessage, null);
                                }
                            }
                        }

                    }




                }
                catch (Exception ex)
                {
                    throw new InvalidPluginExecutionException(ex.Message);
                }
            }
        }


        public static void send_MW(IOrganizationService service, string fromNumber, string toNumber, string messageText)
        {

            ////using (var client = new HttpClient())
            ////{
            ////    //specify to use TLS 1.2 as default connection
            ////    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            ////    //Define Headers
            ////    client.DefaultRequestHeaders.Accept.Clear();
            ////    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            ////    // client.Timeout = -1;

            ////    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials)));
            ////    ///TE Creds 
            ////    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "Zjg4YmY2ZTc4N2E2OGY3YWJhNzhmNTBkZGIzYTRiM2Y6YzlhZjczNTg3ZGMzMGE1NTZkODU5YzNhYjNlNGY0ZGJjMzI3YmMwNmVlMDkwZWFmNDA0MjkxYmI2YzQyYjA2Mw==");

            ////    //TE Creds 
            ////    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "MGQ1MGQyN2RmZGNlNTZhNmEyMmFlNTYyOGVmM2U3ZDg6ZmYxNjU5ZjQ3NzFiYTZiZWQ2YjBkZDNhNzE5ZWU5NGRkOTg4YzkzYWEyZDE5ZTBiZDA4ZDhjYTllNGRiM2Q4Yw==");
            ////    //Prepare Request Body
            ////    List<KeyValuePair<string, string>> requestData = new List<KeyValuePair<string, string>>();
            ////    requestData.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));

            ////    FormUrlEncodedContent requestBody = new FormUrlEncodedContent(requestData);


            ////    //Request Token
            ////    var request = await client.PostAsync("https://public.totalexpert.net/v1/token", requestBody);
            ////    var response = await request.Content.ReadAsStringAsync();
            ////    return JsonConvert.DeserializeObject<AccessToken>(response);

           // }
        }

        public string GetAccessToken()
        {
            //fetch Azure configurations from App.config to GetAccessToken
            string authority = ConfigurationManager.AppSettings["authority"].ToString();
            string clientId = ConfigurationManager.AppSettings["clientId"].ToString();
            string secret = ConfigurationManager.AppSettings["clientSecret"].ToString();
            string serviceUrl = ConfigurationManager.AppSettings["serviceUrl"].ToString();

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

        /// <summary>
        /// Send SMS and return the messageresource object for status update
        /// </summary>
        /// <param name="service"></param>
        /// <param name="fromNumber"></param>
        /// <param name="toNumber"></param>
        /// <param name="messageText"></param>
        public static MessageResource send(IOrganizationService service, string fromNumber, string toNumber, string messageText)
        {
            MessageResource message = null;
            var xml = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
                     "<entity name='ims_configuration'>" +
                     "<attribute name='ims_configurationid' />" +
                     "<attribute name='ims_name' />" +
                     "<attribute name='ims_value' />" +
                     "<order attribute='ims_name' descending='false' />" +
                     "<link-entity name='ims_appconfigsetup' from='ims_appconfigsetupid' to='ims_appconfigsetup' link-type='inner' alias='ac'>" +
                     "<filter type='and'>" +
                     "<condition attribute='ims_name' operator='eq' value='TwilioIntegration' />" +
                     "</filter>" +
                     "</link-entity>" +
                     "</entity>" +
                     "</fetch>";

            EntityCollection configSet = loadTwilioSettings(xml, service);
            // Find your Account Sid and Token at twilio.com/console
            string accountSid = "AC8dd5ddb2a7ca9ec57897d7623ef86e7c"; //Eg value
            string authToken = "d5a6eef2db17357a7d83326e51f94fdc"; //Eg value
            string from = fromNumber;
            string countryCode = "";
            string callbackURL = "http://infymstwilio.azurewebsites.net/OutboundSMSstatus";// Eg value
            foreach (Entity config in configSet.Entities)
            {
                var name = config.GetAttributeValue<string>("ims_name").ToLower();
                var value = config.GetAttributeValue<string>("ims_value");
                if (name == "accountsid")
                {
                    accountSid = value;
                }
                else if (name == "authtoken")
                {
                    authToken = value;
                }
                else if (name == "phonenumber")
                {
                    if (fromNumber == null || fromNumber == "")
                    {
                        from = value;
                    }

                }
                else if (name == "defaultcountrycode")
                {
                    countryCode = value;
                }
                else if (name == "callbackurl")
                {
                    callbackURL = value;
                }
            }

            TwilioClient.Init(accountSid, authToken);

            message = MessageResource.Create(
            body: messageText,
            from: new Twilio.Types.PhoneNumber(formatMobileNumber(from, countryCode)),
            statusCallback: new Uri(callbackURL),//callback url on which twilio will post back the status of the SMS , when it gets updated in twilio corresponding to the generated message SID
            to: new Twilio.Types.PhoneNumber(formatMobileNumber(toNumber, countryCode))
        );

            //Console.WriteLine(message.Sid);
            return message;
        }

        /// <summary>
        /// format phone number by removing etra/special char.
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="countryCode"></param>
        public static string formatMobileNumber(string phoneNumber, string countryCode)
        {
            List<char> charsToRemove = new List<char>() { '(', ')', ' ', '-' };

            foreach (char c in charsToRemove)
            {
                phoneNumber = phoneNumber.Replace(c.ToString(), String.Empty);
            }

            return setCountryCode(phoneNumber, countryCode);
        }

        /// <summary>
        /// Check for country code in the input phone number
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="countryCode"></param>

        public static string setCountryCode(string phoneNumber, string countryCode)
        {
            if (!phoneNumber.StartsWith("+"))
            {
                phoneNumber = countryCode + phoneNumber;
            }
            return phoneNumber;
        }


        /// <summary>
        /// load the configuration settings for Twilio
        /// </summary>
        /// <param name="fXml"></param>
        /// <param name="service"></param>

        public static EntityCollection loadTwilioSettings(string fXml, IOrganizationService service)
        {
            FetchExpression fe = new FetchExpression(fXml);

            EntityCollection results = service.RetrieveMultiple(fe);

            return results;
        }

        /// <summary>
        /// Update SMS activity record in CRM
        /// </summary>
        /// <param name="service"></param>
        /// <param name="sms"></param>
        /// <param name="statecode"></param>
        /// <param name="statuscode"></param>
        /// <param name="fieldname"></param>
        /// <param name="fieldvalue"></param>
        /// <param name="fromNumber"></param>
        public static void updateSMS(IOrganizationService organizationService, Entity sms, int statecode, int statuscode, string fieldname, string fieldvalue, string fromNumber)
        {
            var smsObj = new Entity(sms.LogicalName);
            smsObj.Id = sms.Id;
            if (fromNumber != null && fromNumber != "")
            {
                smsObj["ims_from"] = fromNumber;
            }
            smsObj[fieldname] = fieldvalue;
            smsObj["statecode"] = new OptionSetValue(statecode); //Status
            smsObj["statuscode"] = new OptionSetValue(statuscode); //Status reason
            UpdateRequest updatesmsObj = new UpdateRequest()
            {
                Target = smsObj
            };
            organizationService.Execute(updatesmsObj);
        }
    }
}
