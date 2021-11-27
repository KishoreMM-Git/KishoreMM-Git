using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xrm;
using XRMExtensions;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk.Messages;
using System.Xml.Linq;
using System.Net;
using System.IO;
using System.Net.Http;

namespace Infy.MS.Plugins.PluginClasses
{
    public class AutoShareLeadLoansCont : BasePlugin
    {
        Entity Target;
        IServiceProvider serviceProvider;
        string queryStringParam = string.Empty;
        string url = "https://mmazsmp-d-crm-wa01.scm.azurewebsites.net/api/triggeredwebjobs/ErrorNotification--PreProd/run";
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            var tracingservice = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            if (context == null) { throw new InvalidPluginExecutionException("Context Not Found"); }

            Target = context.GetTargetEntity<Entity>();

            if (context.MessageName.ToLower() == "update")
            {
                tracingservice.Trace("update");
                try
                {
                    //get the team record values
                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage response = client.GetAsync(url + queryStringParam).Result;
                        //lead["new_status"] = response.StatusCode.ToString() + response.IsSuccessStatusCode + response.ReasonPhrase; ;
                        //service.Update(lead);
                    }
                }
                catch (Exception ex)
                {
                    tracingservice.Trace(ex.Message);
                    throw new NotImplementedException();
                }
            }

        }

        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Asynchronous, "Update");
        }
    }
}
