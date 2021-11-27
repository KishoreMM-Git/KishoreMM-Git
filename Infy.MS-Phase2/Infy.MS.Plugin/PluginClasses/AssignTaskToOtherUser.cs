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
    public class AssignTaskToOtherUser : BasePlugin
    {
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            if (context == null)
            {
                throw new InvalidPluginExecutionException("Context not Found");
            }
            if (context.InputParameters.Contains("Target"))
            {
                if (context.MessageName == "Assign")
                {
                    EntityReference TaskReference = (EntityReference)context.InputParameters["Target"];
                    var currentUserReference = new EntityReference(SystemUser.EntityName, context.InitiatingUserId);
                    var grantAccessRequest = new GrantAccessRequest
                    {
                        PrincipalAccess = new PrincipalAccess
                        {
                            AccessMask = AccessRights.ReadAccess | AccessRights.WriteAccess | AccessRights.AssignAccess,
                            Principal = currentUserReference
                        },
                        Target = TaskReference
                    };
                    context.Execute(grantAccessRequest);
                }
            }
        }

        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "Assign");
        }
    }
}
