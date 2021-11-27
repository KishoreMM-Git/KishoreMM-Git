using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xrm;
using XRMExtensions;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using System.Activities;

namespace Infy.MS.Plugins.PluginClasses
{
    public class EmailSignature : BasePlugin
    {
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            //IServiceProvider serviceProvider = null;
            
           
            if (context == null)
            {
                throw new InvalidPluginExecutionException("Context not found");
            }

            if (context.MessageName.ToLower() != "new_appendsignature") { return; }
            Entity email = null;
            
            string body = "Thanks";
           //email["ims_desaction"] = body;
            //context.Update(email);
            context.OutputParameters["OutputParameter"] = body;
            
            //OrganizationRequest actionRequest = new OrganizationRequest();
            //actionRequest.Parameters.Add("OutputParameter", context.OutputParameters);
            //var actionResponse = (OrganizationResponse)context.Execute(actionRequest);
            //if (actionResponse.Results.Contains("OutputParameter"))


            //    return actionResponse.Results["OutputParameter"];

            //context.OutputParameters(email);


        }
        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "new_AppendSignature");
        }
    }
}
