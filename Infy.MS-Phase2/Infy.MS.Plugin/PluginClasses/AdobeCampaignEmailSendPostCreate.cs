using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xrm;
using XRMExtensions;
using Microsoft.Xrm.Sdk.Query;
namespace Infy.MS.Plugins
{
    public class AdobeCampaignEmailSendPostCreate: BasePlugin
    {
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            if (context == null)
            {
                throw new InvalidPluginExecutionException("Context not found");
            }
            if (context.MessageName.ToLower() == "create" && context.PrimaryEntityName.ToLower() == AdobeCampaignEmailSend.EntityName)
            {
                var adobeEmailSend = context.Retrieve(context.PrimaryEntityName, context.PrimaryEntityId, new ColumnSet(AdobeCampaignEmailSend.RegardingObjectId,AdobeCampaignEmailSend.ModifiedOn));
                if(adobeEmailSend!=null && adobeEmailSend.Contains(AdobeCampaignEmailSend.RegardingObjectId))
                {
                    var regardingObject = adobeEmailSend.GetAttributeValue<EntityReference>(AdobeCampaignEmailSend.RegardingObjectId);
                   if(regardingObject.LogicalName==Lead.EntityName)
                    {
                        if (adobeEmailSend.Contains(AdobeCampaignEmailSend.ModifiedOn))
                        {
                            Entity leadEntity = new Entity(Lead.EntityName);
                            leadEntity.Id = regardingObject.Id;
                            leadEntity[Lead.Lastinteractiondate] = adobeEmailSend.GetAttributeValue<DateTime>(AdobeCampaignEmailSend.ModifiedOn).ToUniversalTime();
                            leadEntity[Lead.LastMaketingEmailSentDate] = adobeEmailSend.GetAttributeValue<DateTime>(AdobeCampaignEmailSend.ModifiedOn).ToUniversalTime();
                            context.Update(leadEntity);
                        }
                    }
                }
             //   return;
            }
            //throw new NotImplementedException();
        }

        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "Create");
        }
    }
}
