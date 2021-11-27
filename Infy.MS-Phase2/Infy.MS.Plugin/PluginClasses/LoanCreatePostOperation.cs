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
    public class LoanCreatePostOperation : BasePlugin
    {
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            if (context == null)
            {
                throw new InvalidPluginExecutionException("Context not found");
            }
            context.Trace($"Entered Plugin");
            Entity loan = null;


            loan = context.GetTargetEntity<Entity>();
            Entity objLoan = new Entity(Loan.EntityName);
            context.Trace($"loanID {loan.Id}");

            if (context.MessageName.ToLower() != "create") { return; }

            if (context.PrimaryEntityName.ToLower() != Loan.EntityName) { return; }

            Entity leadToUpdate = new Entity(Lead.EntityName);


            Entity postImage = context.GetFirstPostImage<Entity>();

            Guid leadId = Guid.Empty;
            Guid loanStatus = Guid.Empty;
            if (postImage.Attributes.Contains(Loan.Borrower))
            {
                leadId = ((EntityReference)postImage.Attributes[Loan.Borrower]).Id;
            }
            if (postImage.Contains(Loan.Co_Borrower))
            {
                leadToUpdate[Lead.Co_Borrower] = postImage.GetAttributeValue<EntityReference>(Loan.Co_Borrower);
            }
            leadToUpdate.Id = leadId;


            #region Update the Lead Status and the Contact Type of the Account if Loan Status has value else default it to Active

            if (postImage.Attributes.Contains(Loan.LoanStatus) && postImage.Attributes[Loan.LoanStatus] != null)
            {
                context.Trace($"Loan Status is not NUll");
                loanStatus = ((EntityReference)postImage.Attributes[Loan.LoanStatus]).Id;
                context.Trace($"loanStatus {loanStatus}");

                //Retrieving LoanStatus Name to update it in Lead entity
                var loanStatusName = context.Retrieve(LoanStatus.EntityName, loanStatus, new ColumnSet(LoanStatus.PrimaryName));
                context.Trace($"loanStatus {loanStatusName}");

                #region Query LoanStatus Mapping entity to get corresponding Lead Status, CRM LoanStatus , Crm Loan StatusReason and Contact Type

                QueryExpression query = new QueryExpression(LoanStatusMapping.EntityName);
                ColumnSet cols = new ColumnSet(LoanStatusMapping.LeadStatus, LoanStatusMapping.CRMLoanStatus, LoanStatusMapping.CRMLoanStatusReason, LoanStatusMapping.CRMLeadStatus, LoanStatusMapping.CRMLeadStatusReason, LoanStatusMapping.ContactType);
                query.ColumnSet = cols;
                query.Criteria.AddCondition(new ConditionExpression(LoanStatusMapping.LoanStatus, ConditionOperator.Equal, loanStatus));

                EntityCollection result = context.RetrieveMultiple(query);

                #endregion

                #region Variable Declaration

                string leadStatusName = string.Empty;
                Guid leadStatusId = Guid.Empty;
                string contactTypeName = string.Empty;
                Guid contactTypeId = Guid.Empty;
                string crmLoanStatus = string.Empty;
                string crmLoanStatusReason = string.Empty;
                string crmLeadStatus = string.Empty;
                string crmLeadStatusReason = string.Empty;

                #endregion

                if (result != null && result.Entities.Count > 0)
                {
                    context.Trace($"Contains Mapping");
                    foreach (Entity loanMapping in result.Entities)
                    {
                        if (loanMapping.Attributes.Contains(LoanStatusMapping.LeadStatus))
                        {
                            leadStatusName = loanMapping.GetAttributeValue<EntityReference>(LoanStatusMapping.LeadStatus).Name;
                            leadStatusId = loanMapping.GetAttributeValue<EntityReference>(LoanStatusMapping.LeadStatus).Id;
                        }
                        if (loanMapping.Attributes.Contains(LoanStatusMapping.ContactType))
                        {
                            contactTypeName = loanMapping.GetAttributeValue<EntityReference>(LoanStatusMapping.ContactType).Name;
                            contactTypeId = loanMapping.GetAttributeValue<EntityReference>(LoanStatusMapping.ContactType).Id;
                        }
                        if (loanMapping.Attributes.Contains(LoanStatusMapping.CRMLoanStatus))
                        {
                            crmLoanStatus = loanMapping.GetAttributeValue<OptionSetValue>(LoanStatusMapping.CRMLoanStatus).Value.ToString();
                        }
                        if (loanMapping.Attributes.Contains(LoanStatusMapping.CRMLoanStatusReason))
                        {
                            crmLoanStatusReason = loanMapping.GetAttributeValue<OptionSetValue>(LoanStatusMapping.CRMLoanStatusReason).Value.ToString();
                        }
                        if (loanMapping.Attributes.Contains(LoanStatusMapping.CRMLeadStatus))
                        {
                            crmLeadStatus = loanMapping.GetAttributeValue<OptionSetValue>(LoanStatusMapping.CRMLeadStatus).Value.ToString();
                        }
                        if (loanMapping.Attributes.Contains(LoanStatusMapping.CRMLeadStatusReason))
                        {
                            crmLeadStatusReason = loanMapping.GetAttributeValue<OptionSetValue>(LoanStatusMapping.CRMLeadStatusReason).Value.ToString();
                        }
                        #region Update the related Customer Contact Type

                        Guid customerId = Guid.Empty;
                        if (postImage.Attributes.Contains(Loan.PotentialCustomer))
                        {
                            customerId = ((EntityReference)postImage.Attributes[Loan.PotentialCustomer]).Id;
                        }
                        //Fix- Entity ID must be specified for Update
                        if (customerId != Guid.Empty)
                        {
                            Entity customerToUpdate = new Entity(Account.EntityName);
                            if (contactTypeId != Guid.Empty)
                                customerToUpdate[Account.ContactType] = new EntityReference(ContactType.EntityName, contactTypeId);
                            customerToUpdate.Id = customerId;
                            context.Update(customerToUpdate);
                        }

                        #endregion
                        context.Trace($"Updated Contact Type of Customer");

                        // Update Lead Status
                        if (leadStatusId != Guid.Empty)
                        {
                            leadToUpdate[Lead.LeadStatus] = new EntityReference(LeadStatus.EntityName, leadStatusId);
                        }
                        context.Trace($"Updated Lead Status");


                        #region Update Loan Status and StatusReason retrieved from Mapping Entity
                        SetStateRequest loansetStateReq = new SetStateRequest();
                        loansetStateReq.EntityMoniker = new EntityReference(Loan.EntityName, loan.Id);
                        loansetStateReq.State = new OptionSetValue(Convert.ToInt32(crmLoanStatus));
                        loansetStateReq.Status = new OptionSetValue(Convert.ToInt32(crmLoanStatusReason));
                        context.Execute(loansetStateReq);

                        #endregion
                        context.Trace($"Updated Loan Status");

                        #region Update Lead Status and StatusReason retrieved from Mapping Entity
                        SetStateRequest leadsetStateReq = new SetStateRequest();
                        leadsetStateReq.EntityMoniker = new EntityReference(Lead.EntityName, leadId);
                        leadsetStateReq.State = new OptionSetValue(Convert.ToInt32(crmLeadStatus));
                        leadsetStateReq.Status = new OptionSetValue(Convert.ToInt32(crmLeadStatusReason));
                        context.Execute(leadsetStateReq);

                        #endregion
                        context.Trace($"Updated Lead Status");

                        #region Update the Lead to Loan BPF stage when the Loan status gets changed

                        if (loanStatusName != null && loanStatusName.Attributes.Contains(LoanStatus.PrimaryName))
                        {
                            context.Trace($"Entered to Update BPF");
                            var loanStatusUpdateOnBpf = loanStatusName.GetAttributeValue<string>(LoanStatus.PrimaryName);

                            #region Query the Mortagage Loan Process entity and update the Loan id and the BPF Active Stage accordingly

                            QueryExpression bpfQuery = new QueryExpression(MortgageLoanProcess.EntityName);
                            ColumnSet bpfCols = new ColumnSet(MortgageLoanProcess.PrimaryKey);
                            bpfQuery.ColumnSet = bpfCols;
                            bpfQuery.Criteria.AddCondition(new ConditionExpression(MortgageLoanProcess.Lead, ConditionOperator.Equal, leadId));

                            EntityCollection bpfResult = context.RetrieveMultiple(bpfQuery);

                            if (bpfResult.Entities.Count > 0)
                            {
                                foreach (Entity bpfRecord in bpfResult.Entities)
                                {
                                    // Have to discuss on the design
                                    if (bpfRecord.Attributes.Contains(MortgageLoanProcess.PrimaryKey))
                                    {
                                        Guid bpfRecordId = (Guid)bpfRecord.Attributes[MortgageLoanProcess.PrimaryKey];
                                        context.Trace($"BPF Record Id {bpfRecordId}");

                                        Entity bpfRecordToUpdate = new Entity(MortgageLoanProcess.EntityName);
                                        bpfRecordToUpdate.Id = bpfRecordId;
                                        bpfRecordToUpdate[MortgageLoanProcess.Opportunity] = new EntityReference(Loan.EntityName, loan.Id);
                                        if (loanStatusUpdateOnBpf.ToLower() == "submitted" || loanStatusUpdateOnBpf.ToLower() == "broker submitted" || loanStatusUpdateOnBpf.ToLower() == "prequalified" || loanStatusUpdateOnBpf.ToLower() == "prequal submitted")
                                        {
                                            bpfRecordToUpdate[MortgageLoanProcess.ActiveStage] = new EntityReference("processstage", new Guid("36f13886-17aa-481a-880e-b92a13f4ac6a"));
                                        }
                                        else if (loanStatusUpdateOnBpf.ToLower() == "contruction perm" || loanStatusUpdateOnBpf.ToLower() == "submitted and appraisal approved" || loanStatusUpdateOnBpf.ToLower() == "submitted and appraisal ordered" || loanStatusUpdateOnBpf.ToLower() == "submitted and appraisal received" || loanStatusUpdateOnBpf.ToLower() == "appraisal approved" || loanStatusUpdateOnBpf.ToLower() == "appraisal ordered" || loanStatusUpdateOnBpf.ToLower() == "appraisal received" || loanStatusUpdateOnBpf.ToLower() == "approved with conditions and appraisal submitted" || loanStatusUpdateOnBpf.ToLower() == "approved with conditions and appraisal approved" || loanStatusUpdateOnBpf.ToLower() == "approved with conditions and appraisal received" || loanStatusUpdateOnBpf.ToLower() == "approved with conditions and appraisal ordered" || loanStatusUpdateOnBpf.ToLower() == "broker approved with conditions" || loanStatusUpdateOnBpf.ToLower() == "wire sent" || loanStatusUpdateOnBpf.ToLower() == "lead" || loanStatusUpdateOnBpf.ToLower() == "application"
                                                || loanStatusUpdateOnBpf.ToLower() == "approved" || loanStatusUpdateOnBpf.ToLower() == "approved with conditions")
                                        {
                                            bpfRecordToUpdate[MortgageLoanProcess.ActiveStage] = new EntityReference("processstage", new Guid("b8f606f0-db57-4ac7-b460-ff7a25201055"));
                                        }
                                        else if (loanStatusUpdateOnBpf.ToLower() == "package sent" || loanStatusUpdateOnBpf.ToLower() == "broker funded" || loanStatusUpdateOnBpf.ToLower() == "funded" || loanStatusUpdateOnBpf.ToLower() == "rejected" || loanStatusUpdateOnBpf.ToLower() == "dead lead/loan" || loanStatusUpdateOnBpf.ToLower() == "in closing")
                                        {
                                            bpfRecordToUpdate[MortgageLoanProcess.ActiveStage] = new EntityReference("processstage", new Guid("2806a244-18b3-4c43-9205-7b2ef8eafe1b"));
                                        }
                                        else
                                        {
                                            bpfRecordToUpdate[MortgageLoanProcess.ActiveStage] = new EntityReference("processstage", new Guid("36f13886-17aa-481a-880e-b92a13f4ac6a"));
                                        }

                                        context.Trace($"Updating Start Yes- Started");
                                        updateLoanBPFRestrictToYes(context, loan.Id);
                                        context.Trace($"Updating Start Yes- End");

                                        context.Trace($"Updating BPF- Started");
                                        context.SystemOrganizationService.Update(bpfRecordToUpdate);
                                        context.Trace($"Updating BPF- Started");

                                        context.Trace($"Updating Start No- Started");
                                        updateLoanBPFRestrictToNo(context, loan.Id);
                                        context.Trace($"Updating Start NO- Started");
                                    }
                                }
                            }


                            #endregion

                        }

                        #endregion

                        #region  Should update the Lead status field on Lead which reflects the Loan status
                        if (loanStatusName != null && loanStatusName.Attributes.Contains(LoanStatus.PrimaryName))
                        {
                            leadToUpdate[Lead.LoanStatus] = loanStatusName.GetAttributeValue<string>(LoanStatus.PrimaryName);
                        }

                        #endregion

                        context.Update(leadToUpdate);
                    }
                }
                else
                {
                    context.Trace($"NO Mapping Found");
                    #region  Query the LoanStatus entity to get the guid of the Default LoanStatus

                    QueryExpression queryLoanStatusMappingDefault = new QueryExpression(LoanStatusMapping.EntityName);
                    ColumnSet loanStatusMappingCols = new ColumnSet(LoanStatusMapping.LeadStatus, LoanStatusMapping.CRMLoanStatus, LoanStatusMapping.CRMLoanStatusReason, LoanStatusMapping.CRMLeadStatus, LoanStatusMapping.CRMLeadStatusReason, LoanStatusMapping.ContactType);
                    queryLoanStatusMappingDefault.ColumnSet = loanStatusMappingCols;
                    queryLoanStatusMappingDefault.Criteria.AddCondition(new ConditionExpression(LoanStatusMapping.PrimaryName, ConditionOperator.Equal, Constants.LoanStausMappingDefaultStatus));

                    EntityCollection resultsForNoMapping = context.RetrieveMultiple(queryLoanStatusMappingDefault);
                    context.Trace($"Retrieved Results");


                    #endregion

                    #region Variable Declaration

                    string leadStatusNameForNoMapping = string.Empty;
                    Guid leadStatusIdForNoMapping = Guid.Empty;
                    string crmLoanStatusForNoMapping = string.Empty;
                    string crmLoanStatusReasonForNoMapping = string.Empty;
                    string contactTypeNameForNoMapping = string.Empty;
                    Guid contactTypeIdForNoMapping = Guid.Empty;
                    string crmLeadStatusForNoMapping = string.Empty;
                    string crmLeadStatusReasonForNoMapping = string.Empty;

                    #endregion



                    if (resultsForNoMapping != null && resultsForNoMapping.Entities.Count > 0)
                    {
                        context.Trace($"Mapping Values");
                        foreach (Entity loanStatusMapping in resultsForNoMapping.Entities)
                        {
                            if (loanStatusMapping.Attributes.Contains(LoanStatusMapping.LeadStatus))
                            {
                                leadStatusNameForNoMapping = loanStatusMapping.GetAttributeValue<EntityReference>(LoanStatusMapping.LeadStatus).Name;
                                leadStatusIdForNoMapping = loanStatusMapping.GetAttributeValue<EntityReference>(LoanStatusMapping.LeadStatus).Id;
                            }
                            if (loanStatusMapping.Attributes.Contains(LoanStatusMapping.ContactType))
                            {
                                contactTypeNameForNoMapping = loanStatusMapping.GetAttributeValue<EntityReference>(LoanStatusMapping.ContactType).Name;
                                contactTypeIdForNoMapping = loanStatusMapping.GetAttributeValue<EntityReference>(LoanStatusMapping.ContactType).Id;
                            }
                            if (loanStatusMapping.Attributes.Contains(LoanStatusMapping.CRMLoanStatus))
                            {
                                crmLoanStatusForNoMapping = loanStatusMapping.GetAttributeValue<OptionSetValue>(LoanStatusMapping.CRMLoanStatus).Value.ToString();
                            }
                            if (loanStatusMapping.Attributes.Contains(LoanStatusMapping.CRMLoanStatusReason))
                            {
                                crmLoanStatusReasonForNoMapping = loanStatusMapping.GetAttributeValue<OptionSetValue>(LoanStatusMapping.CRMLoanStatusReason).Value.ToString();
                            }
                            if (loanStatusMapping.Attributes.Contains(LoanStatusMapping.CRMLeadStatus))
                            {
                                crmLeadStatusForNoMapping = loanStatusMapping.GetAttributeValue<OptionSetValue>(LoanStatusMapping.CRMLeadStatus).Value.ToString();
                            }
                            if (loanStatusMapping.Attributes.Contains(LoanStatusMapping.CRMLeadStatusReason))
                            {
                                crmLeadStatusReasonForNoMapping = loanStatusMapping.GetAttributeValue<OptionSetValue>(LoanStatusMapping.CRMLeadStatusReason).Value.ToString();
                            }

                            #region Update the related Customer Contact Type

                            Guid customerIdForNoMapping = Guid.Empty;
                            if (postImage.Attributes.Contains(Loan.PotentialCustomer))
                            {
                                customerIdForNoMapping = ((EntityReference)postImage.Attributes[Loan.PotentialCustomer]).Id;
                            }
                            //Fix- Entity ID must be specified for update
                            if (customerIdForNoMapping != Guid.Empty)
                            {
                                Entity customerToUpdateForNoMapping = new Entity(Account.EntityName);
                                if (contactTypeIdForNoMapping != Guid.Empty)
                                    customerToUpdateForNoMapping[Account.ContactType] = new EntityReference(ContactType.EntityName, contactTypeIdForNoMapping);
                                customerToUpdateForNoMapping.Id = customerIdForNoMapping;
                                context.Update(customerToUpdateForNoMapping);
                            }

                            #endregion
                            context.Trace($"Updated Customer Contact Type");

                            // Update Lead Status
                            if (leadStatusIdForNoMapping != Guid.Empty)
                            {
                                leadToUpdate[Lead.LeadStatus] = new EntityReference(LeadStatus.EntityName, leadStatusIdForNoMapping);
                                leadToUpdate[Lead.LeadStatusDate] = DateTime.UtcNow;
                            }
                            context.Trace($"Updated Lead Status");

                            context.Trace($"Updated Loan Status -Started");
                            #region Update Loan Status and StatusReason retrieved from Mapping Entity
                            SetStateRequest loansetStateReqForNoMapping = new SetStateRequest();
                            loansetStateReqForNoMapping.EntityMoniker = new EntityReference(Loan.EntityName, loan.Id);
                            loansetStateReqForNoMapping.State = new OptionSetValue(Convert.ToInt32(crmLoanStatusForNoMapping));
                            loansetStateReqForNoMapping.Status = new OptionSetValue(Convert.ToInt32(crmLoanStatusReasonForNoMapping));
                            context.Execute(loansetStateReqForNoMapping);

                            #endregion
                            context.Trace($"Updated Loan Status");

                            #region Update Lead Status and StatusReason retrieved from Mapping Entity
                            SetStateRequest leadsetStateReqForNoMapping = new SetStateRequest();
                            leadsetStateReqForNoMapping.EntityMoniker = new EntityReference(Lead.EntityName, leadId);
                            leadsetStateReqForNoMapping.State = new OptionSetValue(Convert.ToInt32(crmLeadStatusForNoMapping));
                            leadsetStateReqForNoMapping.Status = new OptionSetValue(Convert.ToInt32(crmLeadStatusReasonForNoMapping));
                            context.Execute(leadsetStateReqForNoMapping);

                            #endregion
                            context.Trace($"Updated Lead Status");

                            context.Trace($"Updated BPF - Started");
                            #region Update the Lead to Loan BPF stage when the Loan status gets changed

                            if (loanStatusName != null && loanStatusName.Attributes.Contains(LoanStatus.PrimaryName))
                            {
                                var loanStatusUpdateOnBpf = loanStatusName.GetAttributeValue<string>(LoanStatus.PrimaryName);

                                #region Query the Mortagage Loan Process entity and update the Loan id and the BPF Active Stage accordingly

                                QueryExpression bpfQueryForNoMapping = new QueryExpression(MortgageLoanProcess.EntityName);
                                ColumnSet bpfColsForNoMapping = new ColumnSet(MortgageLoanProcess.PrimaryKey);
                                bpfQueryForNoMapping.ColumnSet = bpfColsForNoMapping;
                                bpfQueryForNoMapping.Criteria.AddCondition(new ConditionExpression(MortgageLoanProcess.Lead, ConditionOperator.Equal, leadId));

                                EntityCollection bpfResultForNoMapping = context.RetrieveMultiple(bpfQueryForNoMapping);

                                if (bpfResultForNoMapping.Entities.Count > 0)
                                {
                                    foreach (Entity bpfRecordForNoMapping in bpfResultForNoMapping.Entities)
                                    {
                                        // Have to discuss on the design
                                        if (bpfRecordForNoMapping.Attributes.Contains(MortgageLoanProcess.PrimaryKey))
                                        {
                                            context.Trace($"Updating Start");
                                            Guid bpfRecordIdForNoMapping = (Guid)bpfRecordForNoMapping.Attributes[MortgageLoanProcess.PrimaryKey];
                                            context.Trace($"BPF Record Id {bpfRecordIdForNoMapping}");

                                            Entity bpfRecordToUpdateForNoMapping = new Entity(MortgageLoanProcess.EntityName);
                                            bpfRecordToUpdateForNoMapping.Id = bpfRecordIdForNoMapping;
                                            bpfRecordToUpdateForNoMapping[MortgageLoanProcess.Opportunity] = new EntityReference(Loan.EntityName, loan.Id);
                                            if (loanStatusUpdateOnBpf.ToLower() == "submitted" || loanStatusUpdateOnBpf.ToLower() == "broker submitted" || loanStatusUpdateOnBpf.ToLower() == "prequalified" || loanStatusUpdateOnBpf.ToLower() == "prequal submitted")
                                            {
                                                bpfRecordToUpdateForNoMapping[MortgageLoanProcess.ActiveStage] = new EntityReference("processstage", new Guid("36f13886-17aa-481a-880e-b92a13f4ac6a"));
                                            }
                                            else if (loanStatusUpdateOnBpf.ToLower() == "contruction perm" || loanStatusUpdateOnBpf.ToLower() == "submitted and appraisal approved" || loanStatusUpdateOnBpf.ToLower() == "submitted and appraisal ordered" || loanStatusUpdateOnBpf.ToLower() == "submitted and appraisal received" || loanStatusUpdateOnBpf.ToLower() == "appraisal approved" || loanStatusUpdateOnBpf.ToLower() == "appraisal ordered" || loanStatusUpdateOnBpf.ToLower() == "appraisal received" || loanStatusUpdateOnBpf.ToLower() == "approved with conditions and appraisal submitted" || loanStatusUpdateOnBpf.ToLower() == "approved with conditions and appraisal approved" || loanStatusUpdateOnBpf.ToLower() == "approved with conditions and appraisal received" || loanStatusUpdateOnBpf.ToLower() == "approved with conditions and appraisal ordered" || loanStatusUpdateOnBpf.ToLower() == "broker approved with conditions" || loanStatusUpdateOnBpf.ToLower() == "wire sent" || loanStatusUpdateOnBpf.ToLower() == "lead" || loanStatusUpdateOnBpf.ToLower() == "application"
                                                    || loanStatusUpdateOnBpf.ToLower() == "approved" || loanStatusUpdateOnBpf.ToLower() == "approved with conditions")
                                            {
                                                bpfRecordToUpdateForNoMapping[MortgageLoanProcess.ActiveStage] = new EntityReference("processstage", new Guid("b8f606f0-db57-4ac7-b460-ff7a25201055"));
                                            }
                                            else if (loanStatusUpdateOnBpf.ToLower() == "package sent" || loanStatusUpdateOnBpf.ToLower() == "broker funded" || loanStatusUpdateOnBpf.ToLower() == "funded" || loanStatusUpdateOnBpf.ToLower() == "rejected" || loanStatusUpdateOnBpf.ToLower() == "dead lead/loan" || loanStatusUpdateOnBpf.ToLower() == "in closing")
                                            {
                                                bpfRecordToUpdateForNoMapping[MortgageLoanProcess.ActiveStage] = new EntityReference("processstage", new Guid("2806a244-18b3-4c43-9205-7b2ef8eafe1b"));
                                            }
                                            else
                                            {
                                                bpfRecordToUpdateForNoMapping[MortgageLoanProcess.ActiveStage] = new EntityReference("processstage", new Guid("36f13886-17aa-481a-880e-b92a13f4ac6a"));
                                            }
                                            context.Trace($"Updating Start Yes- Started");
                                            updateLoanBPFRestrictToYes(context, loan.Id);
                                            context.Trace($"Updating Start Yes- End");

                                            context.Trace($"Updating Start BPF REcord- Started");
                                            context.SystemOrganizationService.Update(bpfRecordToUpdateForNoMapping);
                                            context.Trace($"Updating Start BPF REcord- End");

                                            context.Trace($"Updating Start No- End");
                                            updateLoanBPFRestrictToNo(context, loan.Id);
                                            context.Trace($"Updating Start No- End");
                                        }
                                    }
                                }


                                #endregion

                            }

                            #endregion

                            #region  Should update the Lead status field on Lead which reflects the Loan status
                            if (loanStatusName != null && loanStatusName.Attributes.Contains(LoanStatus.PrimaryName))
                            {
                                leadToUpdate[Lead.LoanStatus] = loanStatusName.GetAttributeValue<string>(LoanStatus.PrimaryName);
                                leadToUpdate[Lead.LoanStatusDate] = DateTime.Now;
                            }
                            #endregion

                            context.Update(leadToUpdate);

                        }
                    }

                }
            }

            // If the Loan Status value is null
            else
            {
                #region  Query the LoanStatus entity to get the guid of the Default LoanStatus

                QueryExpression queryLoanStatus = new QueryExpression(LoanStatus.EntityName);
                ColumnSet loanStatusCols = new ColumnSet(LoanStatus.PrimaryKey);
                queryLoanStatus.ColumnSet = loanStatusCols;
                queryLoanStatus.Criteria.AddCondition(new ConditionExpression(LoanStatus.PrimaryName, ConditionOperator.Equal, Constants.LoanDefaultStatus));

                EntityCollection results = context.RetrieveMultiple(queryLoanStatus);

                #endregion

                Entity loanToUpdate = new Entity(Loan.EntityName);
                loanToUpdate.Id = loan.Id;

                if (results != null && results.Entities.Count > 0)
                {
                    foreach (Entity loanRecord in results.Entities)
                    {
                        if (loanRecord.Attributes.Contains(LoanStatus.PrimaryKey))
                        {
                            loanToUpdate[Loan.LoanStatus] = new EntityReference(LoanStatus.EntityName, new Guid(loanRecord[LoanStatus.PrimaryKey].ToString()));

                        }
                    }

                    context.Update(loanToUpdate);
                }
            }

            #endregion

            #region If Loan Rate and Loan Amount changes the corresponding lead should reflect the values.

            if ((postImage.Attributes.Contains(Loan.LoanRate) || postImage.Attributes.Contains(Loan.LoanAmount) ||
                postImage.Attributes.Contains(Loan.LoanProgram) || postImage.Attributes.Contains(Loan.LoanPurpose) ||
                postImage.Attributes.Contains(Loan.Investor) || postImage.Attributes.Contains(Loan.Property) || postImage.Attributes.Contains(Loan.OccupancyOptionSet) ||
                postImage.Attributes.Contains(Loan.MinLoanAmount) || postImage.Attributes.Contains(Loan.MaxLoanAmount) || postImage.Attributes.Contains(Loan.LoanType) ||
                postImage.Attributes.Contains(Loan.PurchaseDate) || postImage.Attributes.Contains(Loan.PurchaseTimeframe) || postImage.Attributes.Contains(Loan.LoanNumber) ||
                postImage.Attributes.Contains(Loan.ExternalID) || postImage.Attributes.Contains(Loan.LoanTerm) || postImage.Attributes.Contains(Loan.Loan_to_Value) ||
                postImage.Attributes.Contains(Loan.LockStatus) || postImage.Attributes.Contains(Loan.LoanArmExpirationDate) || postImage.Attributes.Contains(Loan.ClosingDate) ||
                postImage.Attributes.Contains(Loan.Pre_ApprovalIssuedDate) || postImage.Attributes.Contains(Loan.Pre_ApprovedExpirationDate) ||
                postImage.Attributes.Contains(Loan.ApplicationReceivedDate)))
            {
                if (postImage.Attributes.Contains(Loan.LoanNumber))
                {
                    leadToUpdate[Lead.LoanNumber] = postImage.Attributes[Loan.LoanNumber];
                }

                if (postImage.Attributes.Contains(Loan.LoanRate))
                {
                    leadToUpdate[Lead.LoanRate] = postImage.Attributes[Loan.LoanRate];
                }
                if (postImage.Attributes.Contains(Loan.LoanAmount))
                {
                    leadToUpdate[Lead.LoanAmount] = postImage.Attributes[Loan.LoanAmount];
                }
                if (postImage.Attributes.Contains(Loan.LoanProgram))
                {
                    leadToUpdate[Lead.LoanProgram] = postImage.Attributes[Loan.LoanProgram];
                }
                if (postImage.Attributes.Contains(Loan.LoanPurpose))
                {
                    leadToUpdate[Lead.LoanPurpose] = postImage.Attributes[Loan.LoanPurpose];
                }
                if (postImage.Attributes.Contains(Loan.Investor))
                {
                    leadToUpdate[Lead.Investor] = postImage.Attributes[Loan.Investor];
                }
                if (postImage.Attributes.Contains(Loan.Property))
                {
                    leadToUpdate[Lead.Property] = loan.Attributes[Loan.Property];
                    Guid propertyId = Guid.Empty;
                    propertyId = ((EntityReference)loan.Attributes[Loan.Property]).Id;
                    QueryExpression queries = new QueryExpression("ims_property");
                    queries.ColumnSet.AddColumn("ims_propertytype");
                    queries.Criteria.AddCondition("ims_propertyid", ConditionOperator.Equal, propertyId);
                    var results = context.RetrieveMultiple(queries);
                    if (results != null && results.Entities.Count > 0)
                    {
                        foreach (Entity propertType in results.Entities)
                        {
                            leadToUpdate[Lead.PropertyType] = propertType.GetAttributeValue<EntityReference>(Property.PropertyType);

                        }

                    }
                }
                if (postImage.Attributes.Contains(Loan.OccupancyOptionSet))
                {
                    leadToUpdate[Lead.OccupancyOptionSet] = postImage.Attributes[Loan.OccupancyOptionSet];
                }
                if (postImage.Attributes.Contains(Loan.MinLoanAmount))
                {
                    leadToUpdate[Lead.MinLoanAmountRequested] = postImage.Attributes[Loan.MinLoanAmount];
                }
                if (postImage.Attributes.Contains(Loan.MaxLoanAmount))
                {
                    leadToUpdate[Lead.LoanAmount] = postImage.Attributes[Loan.MaxLoanAmount];
                }
                if (postImage.Attributes.Contains(Loan.LoanType))
                {
                    leadToUpdate[Lead.LoanType] = postImage.Attributes[Loan.LoanType];
                }
                if (postImage.Attributes.Contains(Loan.PurchaseDate))
                {
                    leadToUpdate[Lead.PurchaseYear] = postImage.Attributes[Loan.PurchaseDate];
                }
                if (postImage.Attributes.Contains(Loan.PurchaseTimeframe))
                {
                    leadToUpdate[Lead.PurchaseTimeframe] = postImage.Attributes[Loan.PurchaseTimeframe];
                }
                if (postImage.Attributes.Contains(Loan.FundedDate))
                {
                    leadToUpdate[Lead.FundedDate] = postImage.Attributes[Loan.FundedDate];
                }
                if (postImage.Attributes.Contains(Loan.LoanTerm))
                {
                    leadToUpdate[Lead.Loanterm] = postImage.Attributes[Loan.LoanTerm];
                }
                if (postImage.Attributes.Contains(Loan.Loan_to_Value))
                {
                    leadToUpdate[Lead.Loantovalue] = postImage.Attributes[Loan.Loan_to_Value];
                }
                if (postImage.Attributes.Contains(Loan.LockStatus))
                {
                    leadToUpdate[Lead.Lockstatus] = postImage.Attributes[Loan.LockStatus];
                }
                if (postImage.Attributes.Contains(Loan.LoanArmExpirationDate))
                {
                    leadToUpdate[Lead.LoanARMExpirationDate] = postImage.Attributes[Loan.LoanArmExpirationDate];
                }
                if (postImage.Attributes.Contains(Loan.ClosingDate))
                {
                    leadToUpdate[Lead.ClosingDate] = postImage.Attributes[Loan.ClosingDate];
                }
                if (postImage.Attributes.Contains(Loan.Pre_ApprovalIssuedDate))
                {
                    leadToUpdate[Lead.PreApprovalIssuedDate] = postImage.Attributes[Loan.Pre_ApprovalIssuedDate];
                }
                if (postImage.Attributes.Contains(Loan.Pre_ApprovedExpirationDate))
                {
                    leadToUpdate[Lead.PreApprovalExpirationDate] = postImage.Attributes[Loan.Pre_ApprovedExpirationDate];
                }
                if (postImage.Attributes.Contains(Loan.ApplicationReceivedDate))
                {
                    leadToUpdate[Lead.ApplicationReceivedDate] = postImage.Attributes[Loan.ApplicationReceivedDate];
                }
            }

            context.Update(leadToUpdate);
            #endregion

            //Updating the loan status optionset value based on selected loan status lookup
            if (loan.Attributes.Contains(Loan.LoanStatus) && loan.Attributes[Loan.LoanStatus] != null)
            {
                Guid loanStatusId = ((EntityReference)loan.Attributes[Loan.LoanStatus]).Id;
                Common objCommon = new Common();
                objLoan.Id = loan.Id;
                var optionset = objCommon.FetchLookupOptionSet(context, loanStatusId, LoanStatus.LoanStatusOptions, LoanStatus.EntityName);
                if (optionset != null)
                {
                    objLoan.Attributes.Add(Loan.LoanStatusDate, DateTime.UtcNow);
                    objLoan.Attributes.Add(Loan.LoanStatusOptionSet, optionset);
                    context.Update(objLoan);
                }
            }

            if (loan.Attributes.Contains(Loan.LoanOfficerAssistant) && loan.Attributes[Loan.LoanOfficerAssistant] != null)
            {
                EntityReference LOA = (EntityReference)loan.Attributes[Loan.LoanOfficerAssistant];
                context.Trace("Loan Officer Assistant Name " + LOA.Name);
                Common commonClass = new Common();
                commonClass.shareRecordWithLOA(context, LOA, loan.ToEntityReference());
                if (loan.Attributes.Contains(Loan.Borrower) && loan.Attributes[Loan.Borrower] != null)
                {
                    EntityReference borrower = (EntityReference)loan.Attributes[Loan.Borrower];
                    context.Trace("borrower Name " + borrower.Name);
                    commonClass.shareRecordWithLOA(context, LOA, borrower);
                }
                if (loan.Attributes.Contains(Loan.Co_Borrower) && loan.Attributes[Loan.Co_Borrower] != null)
                {
                    EntityReference coBorrower = (EntityReference)loan.Attributes[Loan.Co_Borrower];
                    context.Trace("coborrower Name " + coBorrower.Name);
                    commonClass.shareRecordWithLOA(context, LOA, coBorrower);
                }
            }

            //Updating Loan At Risk Status field based on Conditions
            updateLoanAtRiskStatus(loan, context, loan.Id);
        }
        private void updateLoanBPFRestrictToYes(IExtendedPluginContext context, Guid loanId)
        {
            Entity loanToUpdate = new Entity(Loan.EntityName);
            loanToUpdate[Loan.RestrictBPFMovement] = true;
            loanToUpdate.Id = loanId;
            context.Update(loanToUpdate);
        }

        private void updateLoanBPFRestrictToNo(IExtendedPluginContext context, Guid loanId)
        {
            Entity loanToUpdate = new Entity(Loan.EntityName);
            loanToUpdate[Loan.RestrictBPFMovement] = false;
            loanToUpdate.Id = loanId;
            context.Update(loanToUpdate);
        }

        private void updateLoanAtRiskStatus(Entity loan, IExtendedPluginContext context, Guid loanId)
        {
            Entity loanToUpdate = new Entity(Loan.EntityName);
            loanToUpdate.Id = loanId;
            var query = new QueryExpression(Loan.EntityName);
            query.ColumnSet.AddColumns(Loan.ClosingDate);
            query.ColumnSet.AddColumns(Loan.LockDate);
            query.ColumnSet.AddColumns(Loan.CDSentDate);
            query.ColumnSet.AddColumns(Loan.CDReceivedDate);
            query.ColumnSet.AddColumns(Loan.AppraisalOrderedDate);
            query.ColumnSet.AddColumns(Loan.LoanStatus);
            query.Criteria.AddCondition(Loan.PrimaryKey, ConditionOperator.Equal, loanId);

            var response = context.RetrieveMultiple(query);

            bool flag = false;
            Dictionary<string, string> dcConfigDetails = Common.FetchConfigDetails(Constants.AppConfigLoanAtRiskStatus, context);
            foreach (string value in dcConfigDetails.Values)
            {
                if (value.Contains(response.Entities[0].GetAttributeValue<EntityReference>(Loan.LoanStatus).Name))
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }
            }


            if (response.Entities[0].Attributes.Contains(Loan.LoanStatus) && flag == false)
            {
                if (response.Entities[0].Attributes.Contains(Loan.ClosingDate))
                {

                    DateTime ClosingDate = response.Entities[0].GetAttributeValue<DateTime>(Loan.ClosingDate);
                    if (ClosingDate >= DateTime.Today && ClosingDate <= DateTime.Today.AddDays(4) && !(response.Entities[0].Attributes.Contains(Loan.CDReceivedDate)))

                    {
                        loanToUpdate[Loan.LoanAtRiskStatus] = "CD not signed";
                    }
                    if (ClosingDate >= DateTime.Today && ClosingDate <= DateTime.Today.AddDays(7) && !(response.Entities[0].Attributes.Contains(Loan.CDSentDate)))

                    {
                        loanToUpdate[Loan.LoanAtRiskStatus] = "CD not sent";
                    }
                    if (ClosingDate >= DateTime.Today && ClosingDate <= DateTime.Today.AddDays(7) && !(response.Entities[0].Attributes.Contains(Loan.LockDate)))

                    {
                        loanToUpdate[Loan.LoanAtRiskStatus] = "Rate not locked";
                    }
                    //loanToUpdate.Id = loanId;
                    //context.Update(loanToUpdate);
                }
                if (response.Entities[0].Attributes.Contains(Loan.CDSentDate))
                {
                    if (response.Entities[0].GetAttributeValue<DateTime>(Loan.CDSentDate) <= DateTime.Today.AddDays(-2) && !(response.Entities[0].Attributes.Contains(Loan.CDReceivedDate)))
                    {
                        loanToUpdate[Loan.LoanAtRiskStatus] = "CD not signed";
                    }
                    //loanToUpdate.Id = loanId;
                    //context.Update(loanToUpdate);
                }
                if (response.Entities[0].Attributes.Contains(Loan.ClosingDate))
                {
                    if (response.Entities[0].GetAttributeValue<DateTime>(Loan.ClosingDate) >= DateTime.Today && response.Entities[0].GetAttributeValue<DateTime>(Loan.ClosingDate) <= DateTime.Today.AddDays(15) && !(response.Entities[0].Attributes.Contains(Loan.AppraisalOrderedDate)))

                    {
                        loanToUpdate[Loan.LoanAtRiskStatus] = "Appraisal not ordered";
                    }
                    //loanToUpdate.Id = loanId;
                    //context.Update(loanToUpdate);
                }

                if (loanToUpdate.Attributes.Count > 0)
                    context.Update(loanToUpdate);

            }
            else
            {
                loanToUpdate[Loan.LoanAtRiskStatus] = "";
                context.Update(loanToUpdate);
            }




        }

        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Asynchronous, "Create");
        }
    }
}
