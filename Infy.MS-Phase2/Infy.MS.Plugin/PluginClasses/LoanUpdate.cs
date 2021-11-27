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
    class LoanUpdate : BasePlugin
    {
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            if (context == null)
            {
                throw new InvalidPluginExecutionException("Context not found");
            }

            if (context.Depth > 1) { return; }

            Entity loan = null;

            loan = context.GetTargetEntity<Entity>();

            if (context.MessageName.ToLower() != "update") { return; }

                if (context.PrimaryEntityName.ToLower() != Loan.EntityName) { return; }

                if (loan != null && loan.Attributes.Contains(Loan.LoanStatus))
                {
                    EntityReference loanStatus = ((EntityReference)loan.Attributes[Loan.LoanStatus]);

                    Entity loanStatusEntity = context.Retrieve(loanStatus.LogicalName, loanStatus.Id, new ColumnSet(LoanStatus.PrimaryName));

                    var loanStatusName = loanStatusEntity.GetAttributeValue<string>(LoanStatus.PrimaryName);

                    if (loanStatusName.ToLower() == "submitted" || loanStatusName.ToLower() == "funded")
                    {
                    try
                    {
                        Entity preImage = context.GetFirstPreImage<Entity>();

                        Guid leadId = Guid.Empty;
                        if (preImage.Attributes.Contains(Loan.OriginatingLead))
                        {
                            leadId = ((EntityReference)loan.Attributes[Loan.OriginatingLead]).Id;
                        }

                        Entity leadToUpdate = new Entity(Lead.EntityName);
                        leadToUpdate.Id = leadId;

                        if (loanStatusName.ToLower() == "submitted")
                        {
                            leadToUpdate[Lead.LeadStatus] = new OptionSetValue(Convert.ToInt32(Lead.LeadStatus_OptionSet.CurrentClient));
                        }

                        if (loanStatusName.ToLower() == "funded")
                        {
                            leadToUpdate[Lead.LeadStatus] = new OptionSetValue(Convert.ToInt32(Lead.LeadStatus_OptionSet.FundedBorrower));
                        }
                        context.Update(leadToUpdate);
                    }
                    catch(Exception ex)
                    {
                        throw new InvalidPluginExecutionException(ex.Message);
                    }
                        
                    }
                }
        }

        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "Update");
        }
    }
}
