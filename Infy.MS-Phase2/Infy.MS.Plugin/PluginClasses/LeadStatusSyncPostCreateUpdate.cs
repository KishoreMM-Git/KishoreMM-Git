using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xrm;
using XRMExtensions;

namespace Infy.MS.Plugins
{
    public class LeadStatusSyncPostCreateUpdate : BasePlugin
    {
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            //if (context.Depth > 2) return;
            var targetEntity = context.GetTargetEntity<Entity>();
            Entity leadEntity = new Entity(targetEntity.LogicalName);
            leadEntity.Id = targetEntity.Id;
            int leastStatusOptionPostImage = -1;
            Entity postImage = null;
            Guid leadStatusPostImageId = Guid.Empty;
            if (context.PostEntityImages.Contains("PostImage") && context.PostEntityImages["PostImage"] is Entity)
            {
                postImage = (Entity)context.PostEntityImages["PostImage"];
                if (postImage.Attributes.Contains(Lead.LeadStatusOptions) && postImage[Lead.LeadStatusOptions] != null)
                {
                    leastStatusOptionPostImage = postImage.GetAttributeValue<OptionSetValue>(Lead.LeadStatusOptions).Value;
                }
                if (postImage.Attributes.Contains(Lead.LeadStatus) && postImage[Lead.LeadStatus] != null)
                {
                    leadStatusPostImageId = postImage.GetAttributeValue<EntityReference>(Lead.LeadStatus).Id;
                }
            }

            if (targetEntity.Attributes.Contains(Lead.LeadStatus) && targetEntity[Lead.LeadStatus] != null)
            {
                Guid leadStatusGuid = ((EntityReference)targetEntity.Attributes[Lead.LeadStatus]).Id;
                Common objCommon = new Common();
                var optionset = objCommon.FetchLookupOptionSet(context, leadStatusGuid, LeadStatus.LeadStatusOptions, LeadStatus.EntityName);
                if (optionset != null)
                {
                    if (leastStatusOptionPostImage != optionset.Value)
                        leadEntity.Attributes.Add(Lead.LeadStatusOptions, optionset);
                }
            }

            if (targetEntity.Attributes.Contains(Lead.LeadStatusOptions) && targetEntity[Lead.LeadStatusOptions] != null)
            {
                int leadStatusoptions = targetEntity.GetAttributeValue<OptionSetValue>(Lead.LeadStatusOptions).Value;
                EntityReference leadStaus = null;
                QueryExpression queryExpression = new QueryExpression(LeadStatus.EntityName);
                queryExpression.ColumnSet.AddColumn(LeadStatus.PrimaryKey);
                queryExpression.Criteria.AddCondition(new ConditionExpression(LeadStatus.LeadStatusOptions, ConditionOperator.Equal, leadStatusoptions));
                var ec = context.RetrieveMultiple(queryExpression);
                if (ec.Entities.Count > 0)
                {
                    if (ec.Entities.FirstOrDefault().Contains(LeadStatus.PrimaryKey))
                    {
                        if (leadStatusPostImageId != ec.Entities.FirstOrDefault().Id)
                            leadStaus = new EntityReference(LeadStatus.EntityName, ec.Entities.FirstOrDefault().Id);
                    }
                }

                if (leadStaus != null)
                    leadEntity.Attributes.Add(Lead.LeadStatus, leadStaus);
            }

            context.Update(leadEntity);
        }
        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "Create");
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "Update");
        }
    }
}
