using Microsoft.Xrm.Sdk;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Infy.MS.Plugins.PluginClasses
{
    public class Contacts
    {
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
    }

    public class RetrieveLead : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {

            // Obtain the execution context from the service provider.
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            ITracingService tracingService =
               (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the organization service reference.
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                // The InputParameters collection contains all the data passed in the message request.
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is EntityReference)
                {
                    // Obtain the target entity from the input parameters.
                    EntityReference entity = (EntityReference)context.InputParameters["Target"];

                    using (WebClient client = new WebClient())
                    {
                        var myContact = new Contacts();
                        myContact.firstname = "Kennedy";
                        myContact.lastname = "charles";
                        myContact.email = "kennedy.charles@gm.com";


                        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Contacts));

                        MemoryStream memoryStream = new MemoryStream();

                        serializer.WriteObject(memoryStream, myContact);

                        var jsonObject = Encoding.Default.GetString(memoryStream.ToArray());

                        var webClient = new WebClient();
                        webClient.Headers[HttpRequestHeader.ContentType] = "application/json";

                        // our function key
                        var code = "6AgwXqDOnlxav2aKHDvRr008APLmDI6br7TE3gNyNyA5ntEc04CjjA==";

                        // the url for our Azure Function
                        var serviceUrl = "https://advanceleadscore.azurewebsites.net/api/ALS?code=" + code;

                        // upload the data using Post mehtod
                        string response = webClient.UploadString(serviceUrl, jsonObject);
                        Entity taskObj = new Entity("lead");
                        var objects = JArray.Parse(response);
                        foreach (JObject root in objects)
                        {

                            var LeadId = (Guid)root["m_Item1"];
                            var LeadScore = (String)root["m_Item2"];
                            var Factors = (String)root["m_Item3"];
                            taskObj.Id = LeadId;
                            taskObj["ims_advancedleadscore"] = 120;
                            taskObj["ims_leadscorecontributionfactors"] = Factors;
                            service.Update(taskObj);
                        }

                            // Create Task with response from Azure function                          

                        //taskObj["subject"] = "Azure Function Called Successfully..";
                        //taskObj["description"] = "Azure Response: " + response;
                        //taskObj["regardingobjectid"] = new EntityReference("lead", entity.Id);

                        //service.Create(taskObj);
                    }

                }


            }  // try end
            catch (Exception ex)
            {

                throw new InvalidPluginExecutionException(ex.Message);

            }
        }
    }
}