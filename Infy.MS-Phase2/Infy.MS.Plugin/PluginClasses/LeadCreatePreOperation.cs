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


namespace Infy.MS.Plugins
{
    public class LeadCreatePreOperation : BasePlugin
    {
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            if (context == null)
            {
                throw new InvalidPluginExecutionException("Context not found");
            }

            //if (context.Depth > 1) { return; }

            if (context.MessageName.ToLower() != "create") { return; }

            if (context.PrimaryEntityName.ToLower() != Lead.EntityName) { return; }

            var lead = context.GetTargetEntity<Entity>();
            Common objCommon = new Common();

            if (lead != null)
            {
                #region  Query the LeadStatus entity to get the guid of the Default LeadStatus - Have to Retrieve from Config entity -Pending
                QueryExpression query = new QueryExpression(LeadStatus.EntityName);
                ColumnSet cols = new ColumnSet(LeadStatus.PrimaryKey);
                query.ColumnSet = cols;
                query.Criteria.AddCondition(new ConditionExpression(LoanStatusMapping.PrimaryName.ToLower(), ConditionOperator.Equal, Constants.LeadDefaultStatus));

                EntityCollection result = context.RetrieveMultiple(query);

                #endregion

                if (result != null && result.Entities.Count > 0)
                {
                    foreach (Entity leadStatus in result.Entities)
                    {
                        if (leadStatus.Attributes.Contains(LeadStatus.PrimaryKey))
                        {
                            if (!lead.Contains(Lead.LeadStatus))//Added as part of Go-Live Change, it should set the value as per the lead staging, if not set the value to default
                                lead[Lead.LeadStatus] = new EntityReference(LeadStatus.EntityName, new Guid(leadStatus[LeadStatus.PrimaryKey].ToString()));
                        }
                    }
                }

                //If Prequalified is Yes Set Lead Status to LO Preapproved
                if (lead.Attributes.Contains(Lead.Prequalified))
                {
                    if (lead.GetAttributeValue<bool>(Lead.Prequalified))
                    {
                        QueryExpression queryLeadStatus = new QueryExpression(LeadStatus.EntityName);
                        ColumnSet columns = new ColumnSet(LeadStatus.PrimaryKey);
                        queryLeadStatus.ColumnSet = columns;
                        queryLeadStatus.Criteria.AddCondition(new ConditionExpression(LoanStatusMapping.PrimaryName.ToLower(), ConditionOperator.Equal, Constants.LOPreapproved));

                        EntityCollection resultLeadStatus = context.RetrieveMultiple(queryLeadStatus);

                        if (resultLeadStatus != null && resultLeadStatus.Entities.Count > 0)
                        {
                            foreach (Entity leadStatus in resultLeadStatus.Entities)
                            {
                                if (leadStatus.Attributes.Contains(LeadStatus.PrimaryKey))
                                {
                                    lead[Lead.LeadStatus] = new EntityReference(LeadStatus.EntityName, new Guid(leadStatus[LeadStatus.PrimaryKey].ToString()));
                                }
                            }
                        }
                    }
                }

                #region When ‘Down Payment (%)’ value X is updated calculate ‘Down Payment ($)’ as ‘x% of Loan Amount’ && When ‘Down Payment ($)’ value X is updated calculate ‘Down Payment (%)’ as ‘ (Down Payment($) / Loan Amount) *100’ 
                Money loanAmountValue = null;
                bool calculateFromLoanAmount = false;
                if (lead.Attributes.Contains(Lead.PurchasePrice) && lead.GetAttributeValue<Money>(Lead.PurchasePrice) != null)
                {
                    loanAmountValue = lead.GetAttributeValue<Money>(Lead.PurchasePrice);
                }
                else if (lead.Attributes.Contains(Lead.LoanAmount) && lead.GetAttributeValue<Money>(Lead.LoanAmount) != null)
                {
                    loanAmountValue = lead.GetAttributeValue<Money>(Lead.LoanAmount);
                    calculateFromLoanAmount = true;
                }
                if (loanAmountValue != null)
                {
                    try
                    {
                        Money loanAmount = loanAmountValue;
                        bool isCalculated = false;
                        int loanAmountVal = 0;
                        bool downPaymentOpt = false;
                        if (lead.Attributes.Contains(Lead.DownPaymentOpt))
                        {
                            downPaymentOpt = lead.GetAttributeValue<bool>(Lead.DownPaymentOpt);
                        }

                        if (loanAmount != null)
                        {
                            loanAmountVal = Decimal.ToInt32(loanAmount.Value);
                        }
                        if (loanAmountVal != 0)
                        {
                            if (lead.Attributes.Contains(Lead.DownPaymentPercentage) && !downPaymentOpt)
                            {
                                decimal downPaymentPercentage = lead.GetAttributeValue<decimal>(Lead.DownPaymentPercentage);
                                if (downPaymentPercentage != 0)
                                {
                                    //When ‘Down Payment (%)’ value X is updated calculate ‘Down Payment ($)’ as ‘x% of Loan Amount’
                                    Money downPaymentField = new Money();
                                    if (!calculateFromLoanAmount)
                                        downPaymentField.Value = objCommon.CalculateDownPaymentAmount(downPaymentPercentage, loanAmount.Value);
                                    else if (calculateFromLoanAmount)
                                        downPaymentField.Value = objCommon.CalculateDownPaymentAmountFromLoanAmount(downPaymentPercentage, loanAmount.Value);
                                    lead[Lead.DownPaymentAmount] = downPaymentField;
                                    isCalculated = true;
                                }
                                else
                                {
                                    //Set Down Payment ($) valut to zero
                                    Money currencyField = new Money();
                                    currencyField.Value = Convert.ToDecimal(0);
                                    lead[Lead.DownPaymentAmount] = currencyField;
                                    isCalculated = true;
                                }
                            }

                            if (lead.Attributes.Contains(Lead.DownPaymentAmount) && downPaymentOpt)
                            {
                                if (!isCalculated)
                                {
                                    Money downPaymentAmount = lead.GetAttributeValue<Money>(Lead.DownPaymentAmount);
                                    int downPaymentAmountVal = 0;
                                    if (downPaymentAmount != null)
                                    {
                                        downPaymentAmountVal = Decimal.ToInt32(downPaymentAmount.Value);
                                    }
                                    if (downPaymentAmountVal != 0)
                                    {
                                        //When ‘Down Payment($)’ value X is updated calculate ‘Down Payment(%)’ as ‘ (Down Payment($) / Loan Amount) *100’
                                        if (!calculateFromLoanAmount)
                                            lead[Lead.DownPaymentPercentage] = objCommon.CalculateDownPaymentPercentage(downPaymentAmount.Value, loanAmount.Value);
                                        else if (calculateFromLoanAmount)
                                            lead[Lead.DownPaymentPercentage] = objCommon.CalculateDownPaymentPercentage(downPaymentAmount.Value, downPaymentAmount.Value + loanAmount.Value);
                                    }
                                    else
                                    {
                                        //Set Down Payment (%) valut to zero
                                        lead[Lead.DownPaymentPercentage] = Convert.ToDecimal(0);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                #endregion
            }

        }

        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PreOperation, SdkMessageProcessingStepMode.Synchronous, "Create");
        }
    }
}
