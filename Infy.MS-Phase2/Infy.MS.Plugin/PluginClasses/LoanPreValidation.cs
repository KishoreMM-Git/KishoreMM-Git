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
    class LoanPreValidation : BasePlugin
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

            if(loan.Attributes.Contains(Loan.OriginatingLead))
            {
                Guid leadId = ((EntityReference)loan.Attributes[Loan.OriginatingLead]).Id;

                int count = getLoanRecords(context, leadId);

                if(count > 0)
                {
                    throw new InvalidPluginExecutionException("There is already an associated Loan.Please close that to add a new loan");
                }
            }
            
        }

        private int getLoanRecords(IExtendedPluginContext context, Guid leadId)
        {
            int count = 0;

            QueryExpression query = new QueryExpression(Loan.EntityName);

            query.ColumnSet = new ColumnSet(Loan.LoanType);
            query.Criteria.AddCondition(new ConditionExpression(Loan.OriginatingLead, ConditionOperator.Equal, leadId));

            EntityCollection results = context.RetrieveMultiple(query);

            if (results.Entities.Count > 0)
            {
                return count++;
            }
            else
            {
                return count;
            }
                
        }

        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PreValidation, SdkMessageProcessingStepMode.Synchronous, "Create");
        }
    }
}
