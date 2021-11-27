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
    public  class LoanCreatePrevalidation : BasePlugin
    {
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            if (context == null)
            {
                throw new InvalidPluginExecutionException("Context not found");
            }

            if (context.Depth > 1) { return; }

            if (context.MessageName.ToLower() != "create") { return; }

            if (context.PrimaryEntityName.ToLower() != Loan.EntityName) { return; }

            var loan = context.GetTargetEntity<Entity>();

            if (loan.Attributes.Contains(Loan.Borrower))
            {
                Guid leadId = ((EntityReference)loan.Attributes[Loan.Borrower]).Id;

                QueryExpression query = new QueryExpression(Loan.EntityName);

                query.ColumnSet = new ColumnSet(Loan.LoanType,Loan.StatusReason);
                query.Criteria.AddCondition(new ConditionExpression(Loan.Borrower, ConditionOperator.Equal, leadId));
                query.Criteria.AddCondition(new ConditionExpression(Loan.Status, ConditionOperator.Equal, (Int32)Loan.Status_OptionSet.Open));
                query.Criteria.AddCondition(new ConditionExpression(Loan.StatusReason, ConditionOperator.Equal, (Int32)Loan.StatusReason_OptionSet.InProgress));
                query.Criteria.AddFilter(LogicalOperator.And);

                EntityCollection results = context.RetrieveMultiple(query);

                if (results.Entities.Count > 0)
                {
                    var configEntity = context.GetConfigValue<Entity>("LoanCreatePrevalidation-Plugin", "LoanCreatePrevalidation-Plugin");
                    if (configEntity != null && configEntity.Attributes.Contains(Configuration.Value))
                    {
                        throw new InvalidPluginExecutionException(configEntity.GetAttributeValue<string>(Configuration.Value));
                    }
                }

                
            }
        }

        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PreOperation, SdkMessageProcessingStepMode.Synchronous, "Create");
        }
    }
}
