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

namespace Infy.MS.Plugins.PluginClasses
{
    public class DeleteDraftEmail : BasePlugin
    {
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            if (context == null)
            {
                throw new InvalidPluginExecutionException("Context not Found");
            }
            context.Trace("Before Update Block");
            Entity email = context.GetTargetEntity<Entity>();
            if (context.MessageName == "Update")
            {
                context.Trace("Inside Update Block");
                if (email.Contains(Email.ActivityStatus) && email.Contains(Email.StatusReason) && email.Contains(Email.IsDeleteEligible))
                {
                    context.Trace("Check Block");
                    OptionSetValue activityStatus = (OptionSetValue)email.Attributes[Email.ActivityStatus];
                    OptionSetValue statusReason = (OptionSetValue)email.Attributes[Email.StatusReason];
                    bool isDeleteEligible = (bool)email.Attributes[Email.IsDeleteEligible]; 
                    if (activityStatus.Value == 0 && statusReason.Value == 1 && isDeleteEligible == true)
                    {
                        context.Trace("Before Delete");
                        context.SystemOrganizationService.Delete(email);
                        context.Trace("After Delete");
                    }
                }
                    
            }
        }

        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PreOperation, SdkMessageProcessingStepMode.Synchronous, "Update");
        }
    }
}
