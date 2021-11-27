using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xrm;
using XRMExtensions;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;

namespace Infy.MS.Plugins.PluginClasses
{
    public class LeadScoreCalculation : BasePlugin
    {
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            //Variable Declarations
            Entity LeadScore = null;
            Guid leadId = Guid.Empty;
            int score = 0;
            if (context == null)
            {
                throw new InvalidPluginExecutionException("Context not found");
            }
            else
            {
                if (context.MessageName.ToLower() != "create") { return; }
                LeadScore = context.GetTargetEntity<Entity>();

                if (context.PrimaryEntityName.ToLower() == Leadscore.EntityName) { return; }
                if (LeadScore.Attributes.Contains(Leadscore.Lead))
                {
                    leadId = LeadScore.GetAttributeValue<EntityReference>(Leadscore.Lead).Id;
                    var query = new QueryExpression(Leadscore.EntityName);
                    query.ColumnSet.AddColumns(Leadscore.Score);
                    query.Criteria.AddCondition(Leadscore.Lead, ConditionOperator.Equal, leadId);
                    var response = context.RetrieveMultiple(query);
                    if (response.Entities.Count > 0 && response == null)
                    {
                        foreach (var res in response.Entities)
                        {
                            if (res.Attributes.Contains(Leadscore.Lead))
                            {
                                score = score + Convert.ToInt32(res.Attributes[Leadscore.Score]);
                            }
                        }
                    }
                    Entity lead = new Entity(Lead.EntityName);
                    lead.Id = leadId;
                    lead[Lead.LeadScore] = score;
                    context.Update(lead);
                }
            }
        }
        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "Create");
        }
    }
}
