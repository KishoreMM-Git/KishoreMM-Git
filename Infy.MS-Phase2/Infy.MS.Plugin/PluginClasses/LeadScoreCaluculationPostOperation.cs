using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xrm;
using XRMExtensions;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;

namespace Infy.MS.Plugins
{
    public class LeadScoreCaluculationPostOperation : BasePlugin
    {
        public override void ExecutePlugin(IExtendedPluginContext context)
        {

            //Variable Declarations
            Entity LeadScore = null;
            Guid leadId = Guid.Empty;
            if (context == null)
            {
                throw new InvalidPluginExecutionException("Context not found");
            }
            else
            {
                if (context.MessageName.ToLower() == "create")
                {

                    context.Trace($"one {context.MessageName.ToLower()}");
                     LeadScore = context.GetTargetEntity<Entity>();
                    UpdateLeadScore(LeadScore, context);
               
                }
                else if(context.MessageName.ToLower() == "update")
                {
                    Entity preImage = (Entity)context.PreEntityImages["preImage"];
                    UpdateLeadScore(preImage, context);
                }
            }
        }
        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Asynchronous, "Create");
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Asynchronous, "Update");
        }

        public void UpdateLeadScore(Entity target, IExtendedPluginContext context)
        {
            int leadScore = 0;
            Guid leadGuid = target.GetAttributeValue<EntityReference>(Leadscore.Lead).Id;
            context.Trace($"Two {leadGuid}");

            Entity leadentity = context.Retrieve(Lead.EntityName, leadGuid, new ColumnSet(Lead.Status));
            {
                if ((leadentity.GetAttributeValue<OptionSetValue>(Lead.Status)).Value == (int)Lead.Status_OptionSet.Open)

                {
                    context.Trace($"Three {leadGuid}");
                    var query = new QueryExpression(Leadscore.EntityName);
                    query.ColumnSet.AddColumns(Leadscore.Score);
                    query.Criteria.AddCondition(Leadscore.Lead, ConditionOperator.Equal, leadGuid);
                    var response = context.RetrieveMultiple(query);
                    if (response.Entities.Count > 0)
                    {
                        context.Trace($"four {response.Entities.Count}");
                        foreach (var res in response.Entities)
                        {
                            if (res.Attributes.Contains(Leadscore.Score))
                            {
                                leadScore = leadScore + Convert.ToInt32(res.Attributes[Leadscore.Score]);
                            }
                        }
                    }
                }
            }
            Entity lead = new Entity(Lead.EntityName);
            lead.Id = leadGuid;
            lead[Lead.LeadScore] = leadScore;
            context.Trace($"five {lead[Lead.LeadScore]}");
            context.Update(lead);
        }
    }
}
