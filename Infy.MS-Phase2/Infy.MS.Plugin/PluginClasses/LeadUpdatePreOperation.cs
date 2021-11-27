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
    public class LeadUpdatePreOperation : BasePlugin
    {
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            if (context == null)
            {
                throw new InvalidPluginExecutionException("Context not found");
            }

            //if (context.Depth > 1) { return; }

            if (context.MessageName.ToLower() != "update") { return; }

            if (context.PrimaryEntityName.ToLower() != Lead.EntityName) { return; }

            var lead = context.GetTargetEntity<Entity>();
            Entity PreImage = (Entity)context.PreEntityImages["PreImage"];
            Common objCommon = new Common();

            Money loanAmount = null;
            bool downPaymentOpt = false;
            bool calculateFromLoanAmount = false;

            if (lead.Attributes.Contains(Lead.PurchasePrice))
            {
                loanAmount = lead.GetAttributeValue<Money>(Lead.PurchasePrice);
            }
            else if (PreImage.Attributes.Contains(Lead.PurchasePrice))
            {
                loanAmount = PreImage.GetAttributeValue<Money>(Lead.PurchasePrice);
            }
            if (loanAmount == null)
            {
                if (lead.Attributes.Contains(Lead.LoanAmount))
                {
                    calculateFromLoanAmount = true;
                    loanAmount = lead.GetAttributeValue<Money>(Lead.LoanAmount);
                }
                else if (PreImage.Attributes.Contains(Lead.LoanAmount))
                {
                    calculateFromLoanAmount = true;
                    loanAmount = PreImage.GetAttributeValue<Money>(Lead.LoanAmount);
                }
            }
            if (lead.Attributes.Contains(Lead.DownPaymentOpt))
            {
                downPaymentOpt = lead.GetAttributeValue<bool>(Lead.DownPaymentOpt);
            }
            else if (PreImage.Attributes.Contains(Lead.DownPaymentOpt))
            {
                downPaymentOpt = PreImage.GetAttributeValue<bool>(Lead.DownPaymentOpt);
            }
            if ((lead.Attributes.Contains(Lead.LoanAmount) || lead.Attributes.Contains(Lead.PurchasePrice)) && !lead.Attributes.Contains(Lead.DownPaymentPercentage) && !lead.Attributes.Contains(Lead.DownPaymentAmount))
            {
                if (loanAmount != null)
                {
                    int loanAmountVal = 0;
                    if (loanAmount != null)
                    {
                        loanAmountVal = Decimal.ToInt32(loanAmount.Value);
                    }
                    if (loanAmountVal != 0)
                    {
                        //Amount Selected in Down payment
                        if (downPaymentOpt)
                        {
                            if (PreImage.Attributes.Contains(Lead.DownPaymentAmount))
                            {
                                Money downPaymentAmount = PreImage.GetAttributeValue<Money>(Lead.DownPaymentAmount);
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
                        //Percentage Selected in Down payment
                        else
                        {
                            if (PreImage.Attributes.Contains(Lead.DownPaymentPercentage))
                            {
                                decimal downPaymentPercentage = PreImage.GetAttributeValue<decimal>(Lead.DownPaymentPercentage);
                                if (downPaymentPercentage != 0)
                                {
                                    //When ‘Down Payment (%)’ value X is updated calculate ‘Down Payment ($)’ as ‘x% of Loan Amount’
                                    Money downPaymentField = new Money();
                                    if (!calculateFromLoanAmount)
                                        downPaymentField.Value = objCommon.CalculateDownPaymentAmount(downPaymentPercentage, loanAmount.Value);
                                    else if (calculateFromLoanAmount)
                                        downPaymentField.Value = objCommon.CalculateDownPaymentAmountFromLoanAmount(downPaymentPercentage, loanAmount.Value);
                                    lead[Lead.DownPaymentAmount] = downPaymentField;
                                }
                                else
                                {
                                    //Set Down Payment ($) valut to zero
                                    Money currencyField = new Money();
                                    currencyField.Value = Convert.ToDecimal(0);
                                    lead[Lead.DownPaymentAmount] = currencyField;
                                }
                            }
                        }
                    }
                }
            }
            else if (loanAmount != null)
            {
                try
                {
                    int loanAmountVal = 0;
                    bool isCalculated = false;
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
                catch (Exception ex)
                {
                    throw new InvalidPluginExecutionException(ex.Message);
                }
            }
        }
        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PreOperation, SdkMessageProcessingStepMode.Synchronous, "Update");
        }
    }
}
