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
using Microsoft.Xrm.Sdk.Messages;

namespace Infy.MS.Plugins
{
    public class LoanPostUpdate : BasePlugin
    {

        #region Variable Declaration

        Guid leadId = Guid.Empty;
        string leadStatusName = string.Empty;
        Guid leadStatusId = Guid.Empty;
        string contactTypeName = string.Empty;
        Guid contactTypeId = Guid.Empty;
        string crmLoanStatus = string.Empty;
        string crmLoanStatusReason = string.Empty;
        string crmLeadStatus = string.Empty;
        string crmLeadStatusReason = string.Empty;
        Entity loanStatusName = null;
        Entity leadToUpdate = new Entity(Lead.EntityName);
        Entity property = new Entity(Property.PropertyType);
        Entity postImage = null;
        Entity preImage = null;
        #endregion
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            if (context == null)
            {
                throw new InvalidPluginExecutionException("Context not found");
            }

            Entity loan = context.GetTargetEntity<Entity>();
            Entity preLoan = context.GetTargetEntity<Entity>();
            Entity objLoan = new Entity(Loan.EntityName);
            if (context.MessageName.ToLower() != "update") { return; }

            if (context.PrimaryEntityName.ToLower() != Loan.EntityName) { return; }

            postImage = context.GetFirstPostImage<Entity>();
            preImage = context.GetFirstPreImage<Entity>();
            loan = postImage;
            preLoan = preImage;


            Entity customer = null;
            Guid customerId = Guid.Empty;
            if (postImage.Attributes.Contains(Loan.Borrower))
            {

                leadId = ((EntityReference)postImage.Attributes[Loan.Borrower]).Id;
                leadToUpdate.Id = leadId;
                customer = context.Retrieve(Lead.EntityName, leadToUpdate.Id, new ColumnSet(Lead.ParentAccountforlead));
                if (customer.Attributes.Contains(Lead.ParentAccountforlead))
                {
                    customer.Id = ((EntityReference)customer.Attributes[Lead.ParentAccountforlead]).Id;
                }
                context.Trace("Found Lead ID and the Customer ID. " + leadToUpdate.Id.ToString() + " :::: " + customer.Id.ToString());
            }



            if (loan != null && loan.Attributes.Contains(Loan.LoanStatus) && loan.Attributes[Loan.LoanStatus] != null)
            {
                retriveLoanStatusMapping(loan, context);

                #region Update the related Customer Contact Type

                if (customer != null && customer.Id != Guid.Empty)
                {
                    customerId = customer.Id;

                }
                //Fix- Entity ID must be specified for Update
                if (customerId != Guid.Empty)
                {
                    Entity customerToUpdate = new Entity(Account.EntityName);
                    if (contactTypeId != Guid.Empty)
                    {
                        customerToUpdate[Account.ContactType] = new EntityReference(ContactType.EntityName, contactTypeId);
                    }
                    customerToUpdate.Id = customerId;
                    context.Update(customerToUpdate);
                    context.Trace("Updated the Customer Record for Contact Type");
                }
                #endregion
            }


            //Updating the loan status optionset value based on selected loan status lookup
            if (loan.Attributes.Contains(Loan.LoanStatus) && loan.Attributes[Loan.LoanStatus] != null)
            {
                Guid loanStatusId = ((EntityReference)loan.Attributes[Loan.LoanStatus]).Id;
                Common objCommon = new Common();
                objLoan.Id = loan.Id;
                var optionset = objCommon.FetchLookupOptionSet(context, loanStatusId, LoanStatus.LoanStatusOptions, LoanStatus.EntityName);
                if (optionset != null)
                {
                    objLoan.Attributes.Add(Loan.LoanStatusOptionSet, optionset);
                    objLoan.Attributes.Add(Loan.LoanStatusDate, DateTime.UtcNow);
                    context.Update(objLoan);
                    context.Trace("Updated the Loan Status and Loan Status Date");
                }
            }

            if (preLoan.Attributes.Contains(Loan.LoanOfficerAssistant) && preLoan.Attributes[Loan.LoanOfficerAssistant] != null)
            {
                EntityReference preLOA = (EntityReference)preLoan.Attributes[Loan.LoanOfficerAssistant];
                context.Trace("Pre Loan Officer Assistant Name " + preLOA.Name);
                Common commonClassObj = new Common();
                commonClassObj.removeAccess(context, preLOA, preLoan.ToEntityReference());
                if (preLoan.Attributes.Contains(Loan.Borrower) && preLoan.Attributes[Loan.Borrower] != null)
                {
                    EntityReference borrower = (EntityReference)preLoan.Attributes[Loan.Borrower];
                    context.Trace("Pre borrower Name " + borrower.Name);
                    commonClassObj.removeAccess(context, preLOA, borrower);
                }
                if (preLoan.Attributes.Contains(Loan.Co_Borrower) && preLoan.Attributes[Loan.Co_Borrower] != null)
                {
                    EntityReference coBorrower = (EntityReference)preLoan.Attributes[Loan.Co_Borrower];
                    context.Trace("Pre coborrower Name " + coBorrower.Name);
                    commonClassObj.removeAccess(context, preLOA, coBorrower);
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

            #region Update Loan Status and StatusReason retrieved from Mapping Entity
            if (!string.IsNullOrEmpty(crmLoanStatus) && !string.IsNullOrEmpty(crmLoanStatusReason))
            {
                SetStateRequest loansetStateReq = new SetStateRequest();
                loansetStateReq.EntityMoniker = new EntityReference(Loan.EntityName, loan.Id);
                loansetStateReq.State = new OptionSetValue(Convert.ToInt32(crmLoanStatus));
                loansetStateReq.Status = new OptionSetValue(Convert.ToInt32(crmLoanStatusReason));
                context.Execute(loansetStateReq);
                context.Trace("Updated the Loan Status and status reasons");
            }

            #endregion

            ////Updating Loan At Risk Status field based on Conditions 
            updateLoanAtRiskStatus(loan, context, loan.Id);
            context.Trace("Updated the Lead Status Date");


            var loanCollection = GetBorrowerLoans(context, leadId);
            context.Trace("Retrived loans for the borrower :" + loanCollection.Entities.Count.ToString());
            string loanStatusNameValue = string.Empty;
            if (loanCollection != null && loanCollection.Entities.Count > 1)
            {
                if (loanStatusName != null && loanStatusName.Attributes.Contains(LoanStatus.PrimaryName))
                {
                    loanStatusNameValue = loanStatusName.GetAttributeValue<string>(LoanStatus.PrimaryName);
                    if (loanStatusNameValue == "Funded" || loanStatusNameValue == "Broker Funded" || loanStatusNameValue == "In Closing" || loanStatusNameValue == "Dead Lead /Loan" || loanStatusNameValue == "Submitted")
                    {
                        List<Entity> loans = loanCollection.Entities.OrderByDescending(r => r.Attributes["modifiedon"]).ToList<Entity>();
                        foreach (Entity qItem in loans)
                        {
                            string qItemStatus = string.Empty;
                            if (qItem.GetAttributeValue<EntityReference>(Loan.LoanStatus).Name == null)
                            {
                                //Retrieving LoanStatus Name to update it in Lead entity
                                Entity newloanStatusName = context.Retrieve(LoanStatus.EntityName, qItem.Id, new ColumnSet(LoanStatus.PrimaryName));
                                if (newloanStatusName != null && newloanStatusName.Attributes.Contains(LoanStatus.PrimaryName))
                                {
                                    qItemStatus = newloanStatusName.GetAttributeValue<string>(LoanStatus.PrimaryName);
                                }
                            }
                            else
                            {
                                qItemStatus = qItem.GetAttributeValue<EntityReference>(Loan.LoanStatus).Name;
                            }

                            if (qItemStatus != "Funded" && qItemStatus != "Broker Funded" && qItemStatus != "In Closing" && qItemStatus != "Dead Lead /Loan" && qItemStatus != "Submitted")
                            {
                                updateDataOnLeadScreen(qItem, context);
                                break;

                            }


                            if (loans.Last<Entity>().Id == qItem.Id)
                            {
                                updateDataOnLeadScreen(loan, context);
                                break;
                            }
                        }
                    }
                    else
                    {
                        updateDataOnLeadScreen(loan, context);
                    }
                }
            }
            else
            {
                updateDataOnLeadScreen(loan, context);
            }

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
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "Update");
        }

        public void ValidateLoan(IExtendedPluginContext context, Guid borrowerId, Guid loanGuid)
        {
            var loanCollection = GetBorrowerLoans(context, borrowerId);
            if (loanCollection != null)
            {

            }
        }
        public EntityCollection GetBorrowerLoans(IExtendedPluginContext context, Guid borrowerId)
        {
            QueryExpression queryExpression = new QueryExpression();
            queryExpression.EntityName = Loan.EntityName;
            queryExpression.ColumnSet = new ColumnSet(Loan.Borrower, Loan.LoanStatus, Loan.ModifiedOn);
            queryExpression.Criteria.AddCondition(new ConditionExpression(Loan.Borrower, ConditionOperator.Equal, borrowerId));
            EntityCollection loanEntity = context.RetrieveMultiple(queryExpression);
            if (loanEntity != null)
            {
                return loanEntity;
            }
            return null;
        }


        public void retriveLoanStatusMapping(Entity loanObj, IExtendedPluginContext context)
        {
            // When Loan status gets changed the corrresponding lead status and the Contact Type of the related contact should change accordingly and even the BPF stage change should also happen.
            if (loanObj != null && loanObj.Attributes.Contains(Loan.LoanStatus) && loanObj.Attributes[Loan.LoanStatus] != null)
            {

                EntityReference loanStatus = ((EntityReference)loanObj.Attributes[Loan.LoanStatus]);

                //Retrieving LoanStatus Name to update it in Lead entity
                loanStatusName = context.Retrieve(LoanStatus.EntityName, loanStatus.Id, new ColumnSet(LoanStatus.PrimaryName));

                #region Query LoanStatus Mapping entity to get corresponding Lead Status, CRM LoanStatus , Crm Loan StatusReason and Contact Type

                QueryExpression query = new QueryExpression(LoanStatusMapping.EntityName);
                ColumnSet cols = new ColumnSet(LoanStatusMapping.LeadStatus, LoanStatusMapping.CRMLoanStatus, LoanStatusMapping.CRMLoanStatusReason, LoanStatusMapping.CRMLeadStatus, LoanStatusMapping.CRMLeadStatusReason, LoanStatusMapping.ContactType);
                query.ColumnSet = cols;
                query.Criteria.AddCondition(new ConditionExpression(LoanStatusMapping.LoanStatus, ConditionOperator.Equal, loanStatus.Id));

                EntityCollection result = context.RetrieveMultiple(query);

                #endregion

                if (result != null && result.Entities.Count > 0)
                {
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
                        context.Trace("Loan Status Mapping retrived successfully");
                    }

                }

            }

        }

        public void reActivateBorrower(Guid borID, IExtendedPluginContext context)
        {
            context.Trace("Entered Re activation method");
            RetrieveRequest request = new RetrieveRequest();
            request.ColumnSet = new ColumnSet(new string[] { "statecode", "statuscode" });
            request.Target = new EntityReference(Lead.EntityName, borID);

            //Retrieve the entity 
            Entity leadToReactivate = (Entity)((RetrieveResponse)context.Execute(request)).Entity;

            if (leadToReactivate != null)
            {
                if (leadToReactivate.Contains(Lead.Status))
                {
                    if (leadToReactivate.GetAttributeValue<OptionSetValue>(Lead.Status).Value == 1 || leadToReactivate.GetAttributeValue<OptionSetValue>(Lead.Status).Value == 2)
                    {
                        //Reactivating Lead and Making it Open and Qualuified
                        SetStateRequest setStateRequest = new SetStateRequest()
                        {
                            EntityMoniker = new EntityReference
                            {
                                Id = leadToReactivate.Id,
                                LogicalName = leadToReactivate.LogicalName,
                            },
                            State = new OptionSetValue(0),
                            Status = new OptionSetValue(1)
                        };
                        context.Execute(setStateRequest);
                        context.Trace("Borrower is reactivated .");

                    }
                }
            }

        }
        public void updateDataOnLeadScreen(Entity loanObj, IExtendedPluginContext context)
        {

            try
            {
                context.Trace("Entered Lead Update Method");
                context.Trace(loanObj.Id.ToString() + " :::: " + postImage.Id);
                if (loanObj.Id != postImage.Id)
                {
                    ////This will over write the global variables and post Image with the 
                    ///data related to current/new loan about to be associated with lead
                    Entity newLoanToUpdateOnLead = context.Retrieve(Loan.EntityName, loanObj.Id, new ColumnSet(true));
                    postImage = newLoanToUpdateOnLead;
                    retriveLoanStatusMapping(loanObj, context);
                }

                // Should update the Lead status field on Lead which reflects the Loan status 
                // loan status and date  should get updated on lead  only for the following loan statuses : Funded, Broker Funded,
                // In Closing, Dead Lead/Loan, Submitted
                if (loanStatusName != null && loanStatusName.Attributes.Contains(LoanStatus.PrimaryName))
                {
                    leadToUpdate[Lead.LoanStatus] = loanStatusName.GetAttributeValue<string>(LoanStatus.PrimaryName);
                    leadToUpdate[Lead.LoanStatusDate] = DateTime.Now;
                }

                //Update Lead Status
                if (leadStatusId != Guid.Empty)
                {
                    leadToUpdate[Lead.LeadStatus] = new EntityReference(LeadStatus.EntityName, leadStatusId);
                    leadToUpdate[Lead.LeadStatusDate] = DateTime.UtcNow;
                }
                context.Trace(loanObj.Id.ToString() + " :::: " + postImage.Id);
                //	If Loan Rate and Loan Amount changes the corresponding lead should reflect the values.
                if (postImage != null /* && (postImage.Attributes.Contains(Loan.LoanRate) || postImage.Attributes.Contains(Loan.LoanAmount) ||
				postImage.Attributes.Contains(Loan.LoanProgram) || postImage.Attributes.Contains(Loan.LoanPurpose) ||
				postImage.Attributes.Contains(Loan.Investor) || postImage.Attributes.Contains(Loan.Property) || postImage.Attributes.Contains(Loan.OccupancyOptionSet) ||
				postImage.Attributes.Contains(Loan.MinLoanAmount) || postImage.Attributes.Contains(Loan.MaxLoanAmount) || postImage.Attributes.Contains(Loan.LoanType)
				|| postImage.Attributes.Contains(Loan.PurchaseDate) || postImage.Attributes.Contains(Loan.PurchaseTimeframe) || postImage.Attributes.Contains(Loan.LoanNumber) ||
				postImage.Attributes.Contains(Loan.ExternalID) || postImage.Attributes.Contains(Loan.LoanTerm) || postImage.Attributes.Contains(Loan.Loan_to_Value) ||
				postImage.Attributes.Contains(Loan.LockStatus) || postImage.Attributes.Contains(Loan.LoanArmExpirationDate) || postImage.Attributes.Contains(Loan.ClosingDate) ||
				postImage.Attributes.Contains(Loan.Pre_ApprovalIssuedDate) || postImage.Attributes.Contains(Loan.Pre_ApprovedExpirationDate) ||
				postImage.Attributes.Contains(Loan.ApplicationReceivedDate))*/)
                {
                    if (postImage.Attributes.Contains(Loan.LoanNumber))
                    {
                        leadToUpdate[Lead.LoanNumber] = postImage.Attributes[Loan.LoanNumber];
                    }
                    if (postImage.Attributes.Contains(Loan.Co_Borrower))
                    {
                        leadToUpdate[Lead.Co_Borrower] = postImage.Attributes[Loan.Co_Borrower];
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
                        leadToUpdate[Lead.Property] = postImage.Attributes[Loan.Property];
                        Guid propertyId = Guid.Empty;
                        propertyId = ((EntityReference)postImage.Attributes[Loan.Property]).Id;
                        QueryExpression queries = new QueryExpression("ims_property");
                        queries.ColumnSet.AddColumn("ims_propertytype");
                        queries.Criteria.AddCondition("ims_propertyid", ConditionOperator.Equal, propertyId);
                        var results = context.RetrieveMultiple(queries);
                        if (results != null && results.Entities.Count > 0)
                        {
                            foreach (Entity propertType in results.Entities)
                            {
                                if (propertType.Contains(Property.PropertyType))
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
                    if (postImage.Attributes.Contains(Loan.LoanStatusDate))
                    {
                        leadToUpdate[Lead.LoanStatusDate] = postImage.Attributes[Loan.LoanStatusDate];
                    }
                    context.Trace("Lead Fields added");
                }


                #region Update Lead Status and StatusReason retrieved from Mapping Entity
                if (!string.IsNullOrEmpty(crmLeadStatus) && !string.IsNullOrEmpty(crmLeadStatusReason))
                {
                    //SetStateRequest leadsetStateReq = new SetStateRequest();
                    //leadsetStateReq.EntityMoniker = new EntityReference(Lead.EntityName, leadId);
                    //leadsetStateReq.State = new OptionSetValue(Convert.ToInt32(crmLeadStatus));
                    //leadsetStateReq.Status = new OptionSetValue(Convert.ToInt32(crmLeadStatusReason));
                    //context.Execute(leadsetStateReq);

                    leadToUpdate["statecode"] = new OptionSetValue(Convert.ToInt32(crmLeadStatus)); //Status
                    leadToUpdate["statuscode"] = new OptionSetValue(Convert.ToInt32(crmLeadStatusReason)); //Status reason
                }
                #endregion


                if (leadToUpdate.Id != Guid.Empty)
                {
                    reActivateBorrower(leadToUpdate.Id, context);//Re-Activate Borrower if needed.
                    context.Update(leadToUpdate);
                }

                context.Trace("Updated Lead Attributes :" + leadToUpdate.Id);
                //throw new InvalidPluginExecutionException("Reached till BPF Update");

                #region Update the Lead to Loan BPF stage when the Loan status gets changed

                if (loanStatusName != null && loanStatusName.Attributes.Contains(LoanStatus.PrimaryName))
                {
                    context.Trace("Entered BPF Update region");
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
                            //Have to discuss on the design
                            if (bpfRecord.Attributes.Contains(MortgageLoanProcess.PrimaryKey))
                            {
                                Guid bpfRecordId = (Guid)bpfRecord.Attributes[MortgageLoanProcess.PrimaryKey];

                                Entity bpfRecordToUpdate = new Entity(MortgageLoanProcess.EntityName);
                                bpfRecordToUpdate.Id = bpfRecordId;
                                bpfRecordToUpdate[MortgageLoanProcess.Opportunity] = new EntityReference(Loan.EntityName, postImage.Id);
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

                                updateLoanBPFRestrictToYes(context, postImage.Id);

                                context.SystemOrganizationService.Update(bpfRecordToUpdate);

                                updateLoanBPFRestrictToNo(context, postImage.Id);
                            }
                        }
                    }


                    #endregion

                }
                #endregion
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message + ex.InnerException);
            }
        }
    }
}
