using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Xrm;
using XRMExtensions;
using System.Text.RegularExpressions;
using System.ServiceModel;
using Microsoft.Crm.Sdk.Messages;

namespace Infy.MS.Plugins
{
    public class LoanStagingPostCreate : BasePlugin
    {
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            if (context == null)
            {
                throw new InvalidPluginExecutionException("Context not found");
            }
            Program objProgram = new Program();
            objProgram.ProcessImportRecord(context);
        }

        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Asynchronous, "Create");
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Asynchronous, "Update");
        }
        private class Program
        {
            List<Common.Mapping> mappings = new List<Common.Mapping>();
            //string validationMessage = string.Empty;
            //string infoMessage = string.Empty;
            string errorMessage = string.Empty;
            //string defaultMessage = string.Empty;
            Dictionary<string, string> dcConfigDetails = new Dictionary<string, string>();
            bool validationStatus = true;
            bool canReturn = false;
            //string errorLog = string.Empty;
            bool errorLogConfig = false;
            EntityReference manualImport = null;
            bool isMDLoanTransfer = false;
            bool isMDLoan = false;
            bool LOExternalIsMD = false;
            public void ProcessImportRecord(IExtendedPluginContext context)
            {
                IOrganizationService service = context.SystemOrganizationService;
                Entity loanStaging = null;
                string loanNumber = string.Empty;
                string loanExtId = string.Empty;
                Common objCommon = new Common();
                string sharedVariableFromLeadStaging = string.Empty;
                bool retryExists = false;

                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    loanStaging = (Entity)context.InputParameters["Target"];
                    if (loanStaging.Contains("ims_retry"))
                    {
                        retryExists = loanStaging.GetAttributeValue<bool>("ims_retry");
                    }
                }
                

                //manualImport = new EntityReference(SystemUser.EntityName, context.UserId);
                if (context.MessageName.ToLower() == "update" && context.Depth > 1 && retryExists!=true)
                    return;
                try
                {
                    if (context.PostEntityImages.Contains("PostImage") && context.PostEntityImages["PostImage"] is Entity)
                    {
                        loanStaging = (Entity)context.PostEntityImages["PostImage"];
                        if (loanStaging.Contains(LoanStaging.Owner))
                        {
                            objCommon.stagingRecordOwner = context.InitiatingUserId;
                        }
                    }
                    else if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                    {
                        loanStaging = (Entity)context.InputParameters["Target"];
                    }
                    //return if plugin primary entity is not Loan
                    if (loanStaging.LogicalName != LoanStaging.EntityName )
                        return;
                    try
                    {
                        //Aquire Borrower Info
                        //objCommon.GetBorrower(service, loanStaging,dcConfigDetails);

                        loanNumber = loanStaging.GetAttributeValue<string>(LoanStaging.PrimaryName);
                        loanExtId = loanStaging.GetAttributeValue<string>(LoanStaging.LoanExternalID);
                        //fetch all configurations related to StagingPostCreate
                        dcConfigDetails = Common.FetchConfigDetails(Constants.AppConfigSetup, service);
                        //Identify the MD Lead with security role of LO External Id with Movement Direct Role assigned or Not, from LDW Job
                        //Stage 1
                        if (loanStaging.Contains(LoanStaging.LOExternalID) && !loanStaging.Contains(LoanStaging.MDLOExternalId))
                        {
                            isMDLoan = objCommon.IdentifyMovementDirectLead(loanStaging.GetAttributeValue<string>(LeadStaging.LOExternalID), service, dcConfigDetails);
                            if (isMDLoan)
                            {
                                //isMDLoanTransfer = true;
                                LOExternalIsMD = true;
                                errorMessage += "Info:MD Lead, Identified with Security Role at LO Externnal Id\n";
                            }
                        }

                        //Stage 2 Transfer Case from MD to Retail
                        if (loanStaging.Contains(LoanStaging.MDLOExternalId) && loanStaging.Contains(LoanStaging.LOExternalID))
                        {
                            isMDLoan = true;
                            isMDLoanTransfer = true;
                            errorMessage += "Info:MD Lead Transfer Processing\n";
                        }
                       

                        //create/update Loan record
                        UpsertLoanDetails(context,loanStaging, service, ref errorMessage);
                    }
                    catch (Exception ex)
                    {
                        errorMessage += ex.Message;
                    }
                    errorLogConfig = dcConfigDetails.ContainsKey(Constants.CreateErrorLog) ? Convert.ToBoolean(dcConfigDetails[Constants.CreateErrorLog].ToString()) : false;
                    if (errorLogConfig)
                    {
                        if (!string.IsNullOrEmpty(errorMessage))
                            objCommon.CreateErrorLog(loanNumber + " " + loanExtId, errorMessage, service);
                    }
                }
                //catch (FaultException<OrganizationServiceFault> ex)
                //{
                //    //throw new InvalidPluginExecutionException(ex.Message);
                //    objCommon.CreateErrorLog(loanNumber + " " + loanExtId, errorMessage + " " + ex.Message, service);
                //}
                catch (Exception ex)
                {
                    //throw new InvalidPluginExecutionException(ex.Message);
                    objCommon.CreateErrorLog(loanNumber + " " + loanExtId, errorMessage + " " + ex.Message, service);
                }
            }

            public void UpsertLoanDetails(IExtendedPluginContext context,Entity loanStaging, IOrganizationService service, ref string errorMessage)
            {
                //Fetch Mapping Details [ImportDetailsMapping]
                Guid importMasterDataId = Guid.Empty;
                Entity objLoan = null;
                Entity leadEntity = null;
                var borrowerReactivated = false;
                var loanReactivated = false;
                var isLoanStatusChanged = false;
                Guid loanId = Guid.Empty;
                //Guid leadId = Guid.Empty;
                EntityReference defaultTeam = null;
                string importProcessName = string.Empty;
                Common objCommon = new Common();

                //Check Import Process Name field value
                if (loanStaging.Attributes.Contains(LoanStaging.ImportProcessName) && loanStaging.Attributes[LoanStaging.ImportProcessName] != null)
                {
                    importProcessName = loanStaging.GetAttributeValue<string>(LoanStaging.ImportProcessName);
                    if(importProcessName.Equals(Constants.IDMEDWLoan,StringComparison.OrdinalIgnoreCase))
                    {
                        if (!loanStaging.Contains(Lead.LOExternalId))
                        {
                            manualImport = new EntityReference(SystemUser.EntityName, context.UserId);
                            if (manualImport != null)
                                objCommon.manualImportReference = manualImport;
                            if (!loanStaging.Contains(LoanStaging.LOExternalID))
                            {
                                var loExternalId=objCommon.GetLOExternalId(manualImport, service);
                                if (loExternalId != null)
                                    loanStaging.Attributes[LoanStaging.LOExternalID] = loExternalId;
                            }
                        }
                        importProcessName = Constants.IDMEDWLoan;
                    }
                    //Manual Import
                    else
                    {
                        string integrationUserConfigValue = context.GetConfigValue<string>(Constants.D365CRM_EDW_INTEGRATION_Id, Constants.AppConfigSetup);
                        objCommon.GetCrmUserGuid(ref integrationUserConfigValue, service);
                        if (!context.UserId.Equals(new Guid(integrationUserConfigValue)))
                        {
                            importProcessName = Constants.IDMEDWLoan;
                            if (!loanStaging.Contains(Lead.LOExternalId))
                            {
                                manualImport = new EntityReference(SystemUser.EntityName, context.UserId);
                                if (manualImport != null)
                                    objCommon.manualImportReference = manualImport;
                                if (!loanStaging.Contains(LoanStaging.LOExternalID))
                                {
                                    var loExternalId = objCommon.GetLOExternalId(manualImport, service);
                                    if (loExternalId != null)
                                        loanStaging.Attributes[LoanStaging.LOExternalID] = loExternalId;
                                }
                            }
                            //manualImport = new EntityReference(SystemUser.EntityName, context.UserId);
                            objCommon.manualImportReference = manualImport;
                        }
                    }
                }
                else if (!loanStaging.Contains(LoanStaging.ImportProcessName))
                {
                    string integrationUserConfigValue = context.GetConfigValue<string>(Constants.D365CRM_EDW_INTEGRATION_Id, Constants.AppConfigSetup);
                    objCommon.GetCrmUserGuid(ref integrationUserConfigValue, service);
                    if (!context.UserId.Equals(new Guid(integrationUserConfigValue)))
                    {
                        importProcessName = Constants.IDMEDWLoan;
                        if (!loanStaging.Contains(Lead.LOExternalId))
                        {
                            manualImport = new EntityReference(SystemUser.EntityName, context.UserId);
                            if (manualImport != null)
                                objCommon.manualImportReference = manualImport;
                            if (!loanStaging.Contains(LoanStaging.LOExternalID))
                            {
                                var loExternalId = objCommon.GetLOExternalId(manualImport, service);
                                if (loExternalId != null)
                                    loanStaging.Attributes[LoanStaging.LOExternalID] = loExternalId;
                            }
                        }
                       // manualImport = new EntityReference(SystemUser.EntityName, context.UserId);
                    }
                }

                if (importProcessName != string.Empty)
                {
                    //Get ImportDataMaster GUID based on Import Process Name
                    importMasterDataId = Common.GetImportDataMasterAndDefaultTeam(importProcessName, ref defaultTeam, service);
                    if (defaultTeam != null)
                        objCommon.defaultTeamReference = defaultTeam;
                }
                //overwrite the Default Team from Marketing Team to Movement Direct Team in case of LO External Id is MD Loan Officer
                if (!string.IsNullOrEmpty(Constants.VeloLDW) && LOExternalIsMD)
                {
                    Common.GetImportDataMasterAndDefaultTeam(Constants.VeloLDW, ref  defaultTeam, service);
                    if (defaultTeam != null)
                        objCommon.defaultTeamReference = defaultTeam;
                }

                //return if import data Master ID is null
                if (importMasterDataId == Guid.Empty)
                {
                    validationStatus = false;
                    objCommon.UpdateStagingLog(loanStaging.Id, Guid.Empty, LoanStaging.EntityName, "Import Process Name is Invalid or Import Data Master Configuration is missing.", validationStatus, service);
                    return;
                }
                //Check the Loan status is existed in Master data or not, if not set the default value as Lead
                if (loanStaging.Contains(LoanStaging.LoanStatusExternalID))
                {
                    var isLoanStatusExisted = CheckMasterLoanStatus(service, loanStaging.GetAttributeValue<string>(LoanStaging.LoanStatusExternalID));
                    if (!isLoanStatusExisted.Item1)
                    {
                        var defaultLoanStatus = objCommon.GetMessage(Constants.DefaultLoanStatus, dcConfigDetails);
                        if (!string.IsNullOrEmpty(defaultLoanStatus))
                        {
                            loanStaging.Attributes[LoanStaging.LoanStatusExternalID] = defaultLoanStatus;
                            errorMessage += "Info:Loan Status not found in the master data, setting it to default loan status as Lead\n";
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(isLoanStatusExisted.Item2))
                            loanStaging.Attributes[LoanStaging.LoanStatusExternalID] = isLoanStatusExisted.Item2;
                    }
                }
                //Check the Status of the Borrower ,if it is read only reactivate it
                //if (loanStaging.Contains(LoanStaging.BorrowerExternalID))
                //{
                //    var en = CheckBorrowerStatus(context.InitiatingUserOrganizationService, loanStaging.GetAttributeValue<string>(LoanStaging.BorrowerExternalID));
                //    if (en != null)
                //    {
                //        if (en.Contains(Lead.Status))
                //        {
                //            if (en.GetAttributeValue<OptionSetValue>(Lead.Status).Value != 0)
                //            {
                //                //Reactivating Lead and Making it Open and Qualuified
                //                SetStateRequest setStateRequest = new SetStateRequest()
                //                {
                //                    EntityMoniker = new EntityReference
                //                    {
                //                        Id = en.Id,
                //                        LogicalName = en.LogicalName,
                //                    },
                //                    State = new OptionSetValue(0),
                //                    Status = new OptionSetValue(1)
                //                };
                //                service.Execute(setStateRequest);
                //                errorMessage += "Info:Borrower is reactivated to create Loan\n";
                //            }
                //        }
                //    }
                //}
                if (!objCommon.FetchMappings(importMasterDataId, ref mappings, service, ref errorMessage))
                {
                    validationStatus = false;
                    objCommon.UpdateStagingLog(loanStaging.Id, Guid.Empty, LoanStaging.EntityName, errorMessage, validationStatus, service);
                    return;
                }
                //Check for Mandatory fields and Max Length Allowed
                objCommon.MandatoryValidation(loanStaging, mappings, ref validationStatus, ref canReturn, ref errorMessage, dcConfigDetails);

                //Form Loan Object based on mappings which is require for Create/Update
                objLoan = objCommon.FormTargetEntityObject(context.SystemOrganizationService, Loan.EntityName, loanStaging, mappings, service, ref validationStatus, ref canReturn, ref errorMessage, dcConfigDetails);

                if (canReturn)
                {
                    if (leadEntity != null && borrowerReactivated == true)
                        ChangeLeadStatus(leadEntity, service, errorMessage);
                    objCommon.UpdateStagingLog(loanStaging.Id, Guid.Empty, LoanStaging.EntityName, errorMessage, validationStatus, service);
                    return;
                }

                //If Borrower external ID not found, DO NOT Create Loan. Show error message in staging table: BORROWER NOT FOUND
                if (!objLoan.Attributes.Contains(Loan.Borrower))
                {
                    if (leadEntity != null && borrowerReactivated == true)
                        ChangeLeadStatus(leadEntity, service, errorMessage);
                    validationStatus = false;
                    objCommon.UpdateStagingLog(loanStaging.Id, Guid.Empty, LoanStaging.EntityName, errorMessage + "BORROWER NOT FOUND", validationStatus, service);
                    return;
                }

                //Get Lead Account
                if (objLoan.Attributes.Contains(Loan.Borrower))
                {
                    EntityReference erfLead = objLoan.GetAttributeValue<EntityReference>(Loan.Borrower);
                    Entity account = service.Retrieve(Lead.EntityName, erfLead.Id, new ColumnSet(Lead.ParentAccountforlead));
                    if (account != null)
                    {
                        //Set Loan Account Field
                        if (account.Attributes.Contains(Lead.ParentAccountforlead))
                        {
                            objLoan[Loan.PotentialCustomer] = account[Lead.ParentAccountforlead];
                            objLoan[Loan.Account] = account[Lead.ParentAccountforlead];
                        }
                    }
                }

                //Check for Existing Active Loan record
                bool existingLoan = false;
                Entity objExistingLoan = GetPreviousLoan(loanStaging.GetAttributeValue<string>(LoanStaging.PrimaryName), loanStaging.GetAttributeValue<string>(LoanStaging.LoanExternalID), context.InitiatingUserOrganizationService);
                if (objExistingLoan != null && objExistingLoan.Id != Guid.Empty)
                {
                    existingLoan = true;
                    loanId = objExistingLoan.Id;
                }
                //Existing Active Loan record present in SYSTEM. update dirty fields
                if (existingLoan)
                {
                    if (objExistingLoan.Contains(Loan.LoanStatus) && objLoan.Contains(Loan.LoanStatus))
                    {
                        if (objLoan.GetAttributeValue<EntityReference>(Loan.LoanStatus).Id.ToString().ToLower() != objExistingLoan.GetAttributeValue<EntityReference>(Loan.LoanStatus).Id.ToString().ToLower())
                        {
                            isLoanStatusChanged = true;
                        }
                    }

                    //Owner update will not happen in Update. Once Set Can not be override
                    //if (objLoan.Attributes.Contains(Loan.Owner))
                    //    objLoan.Attributes.Remove(Loan.Owner);
                    //if (objLoan.Contains(Loan.Owner) && defaultTeam!=null)
                    //{
                    //    if(objLoan.GetAttributeValue<EntityReference>(Loan.Owner).Id.ToString().ToLower().Equals(defaultTeam.Id.ToString().ToLower()))
                    //    {
                    //        if(objLoan.Contains(Loan.LOExternalID))
                    //        {
                    //            objLoan.Attributes.Remove(Loan.LOExternalID);
                    //        }
                    //    }
                    //}
                    if (objExistingLoan.Contains(Loan.Status) && objExistingLoan.GetAttributeValue<OptionSetValue>(Loan.Status).Value == 1)
                    {
                        SetStateRequest setStateRequest = new SetStateRequest()
                        {
                            EntityMoniker = new EntityReference
                            {
                                Id = objExistingLoan.Id,
                                LogicalName = objExistingLoan.LogicalName,
                            },
                            State = new OptionSetValue(0),
                            Status = new OptionSetValue(1)
                        };
                        service.Execute(setStateRequest);
                        errorMessage += "Info: Existing Loan is reactivated to update the information\n";
                        loanReactivated = true;
                    }

                    objCommon.UpdateValidationMessage(string.Format(objCommon.GetMessage(Constants.DupliRecordFound, dcConfigDetails), Loan.EntityName), ref errorMessage);
                    if (loanStaging.Contains(LoanStaging.OneTimeDataMigration) && loanStaging.GetAttributeValue<bool>(LoanStaging.OneTimeDataMigration))
                    {
                        //return the record if the data is one data migration
                        return;
                    }
                    objCommon.UpdateRecordIfDirty(objExistingLoan, objLoan, Loan.EntityName, mappings, service, ref validationStatus, ref canReturn, ref errorMessage);
                    if (canReturn)
                    {
                        if (objExistingLoan != null && loanReactivated == true)
                            ChangeLoanStatus(objExistingLoan, service);

                        if (leadEntity != null && borrowerReactivated == true)
                            ChangeLeadStatus(leadEntity, service, errorMessage);
                        objCommon.UpdateStagingLog(loanStaging.Id, Guid.Empty, LoanStaging.EntityName, errorMessage, validationStatus, service);
                        return;
                    }
                    //Asscociate,Disasscosiate Other Contacts ans Loan Officer
                    objCommon.AssociateDisasscociateContacts(service, objLoan, ref errorMessage, ref validationStatus);
                    if (isMDLoan && isMDLoanTransfer)
                    {
                        //To Share the Lead to MD LO External ID( to MD LO)
                        if (objExistingLoan.LogicalName == Loan.EntityName && loanStaging.Contains(Loan.MDLOExternalId) && loanStaging.Contains(Loan.LOExternalID))
                        {
                            string errorMessageFromShare = string.Empty;
                            objCommon.ShareRecordsMDLoanOfficerExternalId(ref errorMessageFromShare, objExistingLoan.ToEntityReference(), loanStaging.GetAttributeValue<string>(Loan.MDLOExternalId), service, dcConfigDetails);
                            if (!string.IsNullOrEmpty(errorMessageFromShare))
                                errorMessage += errorMessageFromShare;
                            else
                                errorMessage += "Info:MD Loan is Shared to MD EEID \n";
                        }

                    }
                }
                //No Existing Active Loan Present in SYSTEM. Created new Lead with all details
                else
                {
                    try
                    {
                        //Check the Status of the Borrower ,if it is read only reactivate it
                        if (loanStaging.Contains(LoanStaging.BorrowerExternalID))
                        {

                            var en = CheckBorrowerStatus(context.InitiatingUserOrganizationService, loanStaging.GetAttributeValue<string>(LoanStaging.BorrowerExternalID));
                            if (en != null)
                            {
                                leadEntity = en;
                                if (en.Contains(Lead.Status))
                                {
                                    if (en.GetAttributeValue<OptionSetValue>(Lead.Status).Value == 1 || en.GetAttributeValue<OptionSetValue>(Lead.Status).Value == 2)
                                    {
                                        //Reactivating Lead and Making it Open and Qualuified
                                        SetStateRequest setStateRequest = new SetStateRequest()
                                        {
                                            EntityMoniker = new EntityReference
                                            {
                                                Id = en.Id,
                                                LogicalName = en.LogicalName,
                                            },
                                            State = new OptionSetValue(0),
                                            Status = new OptionSetValue(1)
                                        };
                                        service.Execute(setStateRequest);
                                        errorMessage += "Info:Borrower is reactivated for Loan\n";
                                        borrowerReactivated = true;
                                    }//If Lead is Qualified
                                    //else if (en.GetAttributeValue<OptionSetValue>(Lead.Status).Value == 2)//If Lead is Disqualified
                                    //{
                                    //    validationStatus = false;
                                    //    objCommon.UpdateStagingLog(loanStaging.Id, Guid.Empty, LoanStaging.EntityName, errorMessage + "Borrower is Disqualified", validationStatus, service);
                                    //    return;
                                    //}
                                }
                            }
                        }
                        //If Owner not found in CRM System. Assign Loan to default Team Configured in Import Process
                        if (!objLoan.Attributes.Contains(Loan.Owner) && manualImport == null)
                        {
                            if (defaultTeam != null)
                            {
                                //if (objLoan.Contains(Loan.LOExternalID))
                                //    objLoan.Attributes[Loan.LOExternalID] = null;
                                objLoan[Loan.Owner] = defaultTeam;
                                objCommon.UpdateValidationMessage("Loan Officer/Owner not found in CRM System. Assigning Lead to Default Team", ref errorMessage);
                            }
                        }
                        else if (!objLoan.Attributes.Contains(Loan.Owner) && manualImport != null)
                        {
                            objLoan[Loan.Owner] = manualImport;
                        }

                        loanId = service.Create(objLoan);
                        //Asscociate,Disasscosiate Other Contacts ans Loan Officer
                        objCommon.AssociateDisasscociateContacts(service, objLoan, ref errorMessage, ref validationStatus);
                    }
                    catch (Exception ex)
                    {
                        objCommon.UpdateValidationMessage("Error while create Loan record " + ex.Message, ref errorMessage);
                        validationStatus = false;
                        canReturn = true;
                    }
                }
                if (canReturn)
                {
                    if (leadEntity != null && borrowerReactivated == true)
                        ChangeLeadStatus(leadEntity, service, errorMessage);
                    objCommon.UpdateStagingLog(loanStaging.Id, Guid.Empty, LoanStaging.EntityName, errorMessage, validationStatus, service);
                    return;
                }
                if (loanReactivated == true && isLoanStatusChanged == false)
                {
                    ChangeLoanStatus(objExistingLoan, service);
                }
                if (loanId != Guid.Empty)
                    objCommon.UpdateStagingLog(loanStaging.Id, loanId, LoanStaging.EntityName, errorMessage, validationStatus, service);
            }

            public Entity GetPreviousLoan(string loanNumber, string externalId, IOrganizationService service)
            {
                Entity objExistingLoan = null;
                if (!string.IsNullOrEmpty(externalId) || !string.IsNullOrEmpty(loanNumber))
                {
                    //// Define Condition Values
                    var QEims_loan_ims_loannumber = loanNumber;
                    // Instantiate QueryExpression QEloan
                    var QEloan = new QueryExpression(Loan.EntityName);
                    // Add all columns to QEims_loanStaging.ColumnSet
                    QEloan.ColumnSet.AllColumns = true;
                    // Define filter QEims_loanStaging.Criteria
                    //QEloan.Criteria.AddCondition(Loan.Status, ConditionOperator.Equal, (int)Loan.Status_OptionSet.Open);
                    var QEopportunity_Criteria_0 = new FilterExpression();
                    QEloan.Criteria.AddFilter(QEopportunity_Criteria_0);

                    // Define filter QEopportunity_Criteria_0
                    QEopportunity_Criteria_0.FilterOperator = LogicalOperator.Or;
                    if (!string.IsNullOrEmpty(externalId))
                        QEopportunity_Criteria_0.AddCondition(Loan.ExternalID, ConditionOperator.Equal, externalId);
                    if (!string.IsNullOrEmpty(loanNumber))
                        QEopportunity_Criteria_0.AddCondition(Loan.PrimaryName, ConditionOperator.Equal, loanNumber);

                    EntityCollection ecloan = service.RetrieveMultiple(QEloan);
                    if (ecloan != null && ecloan.Entities.Count > 0)
                    {
                        objExistingLoan = ecloan.Entities[0];
                    }
                }
                return objExistingLoan;
            }
            public void ReactivateBorrower(IOrganizationService service)
            {

            }
            public Entity CheckBorrowerStatus(IOrganizationService service,string borrowerId)
            {
                string xml = string.Format(@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
  "<entity name='lead'>" +
  "<attribute name='statecode' />"+
  "<attribute name='statuscode' />" +
  "<order attribute='createdon' descending='true' />" +
    "<filter type='and'>" +
      "<condition attribute='ims_externalid' operator='eq' value='"+ borrowerId +"' />" +
    "</filter>" +
  "</entity>" +
"</fetch>");
                var enCollection = service.RetrieveMultiple(new FetchExpression(xml));
                if(enCollection.Entities.Count>0)
                {
                   return enCollection.Entities.FirstOrDefault();
                }
                return null;
            }

            public Tuple<bool,string> CheckMasterLoanStatus(IOrganizationService service,string loanstatus)
            {
                EntityCollection ec = new EntityCollection();
                string xmlName = string.Format(@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
                                                "<entity name='ims_loanstatus'>" +
                                                  "<attribute name='ims_loanstatusid' />" +
                                                  "<attribute name='ims_name' />" +
                                                  "<attribute name='createdon' />" +
                                                  "<order attribute='ims_name' descending='false' />" +
                                                  "<filter type='and'>" +
                                                    "<filter type='or'>" +
                                                      "<condition attribute='ims_name' operator='eq' value='" + loanstatus + "' />" +
                                                    "</filter>" +
                                                    "<condition attribute='statecode' operator='eq' value='0' />" +
                                                  "</filter>" +
                                                "</entity>" +
                                              "</fetch>");
                string xmlPossibleValues = string.Format(@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
                                                          "<entity name='ims_loanstatus'>" +
                                                            "<attribute name='ims_loanstatusid' />" +
                                                            "<attribute name='ims_name' />" +
                                                            "<attribute name='createdon' />" +
                                                            "<order attribute='ims_name' descending='false' />" +
                                                            "<filter type='and'>" +
                                                              "<filter type='or'>" +
                                                                "<condition attribute='ims_possiblevalues' operator='like' value='%" + loanstatus + "%' />" +
                                                              "</filter>" +
                                                              "<condition attribute='statecode' operator='eq' value='0' />" +
                                                            "</filter>" +
                                                          "</entity>" +
                                                        "</fetch>");

                ec = service.RetrieveMultiple(new FetchExpression(xmlName));
                if(ec.Entities.Count>0)
                {
                    if (ec.Entities.FirstOrDefault().Contains(LoanStatus.PrimaryName))
                        return new Tuple<bool, string>(true, ec.Entities.FirstOrDefault().GetAttributeValue<string>(LoanStatus.PrimaryName));
                }
                else
                {
                    ec = service.RetrieveMultiple(new FetchExpression(xmlPossibleValues));
                    if (ec.Entities.Count > 0)
                    {
                        if (ec.Entities.FirstOrDefault().Contains(LoanStatus.PrimaryName))
                            return new Tuple<bool, string>(true, ec.Entities.FirstOrDefault().GetAttributeValue<string>(LoanStatus.PrimaryName));
                    }
                }
                return new Tuple<bool, string>(false, string.Empty);
            }

            public void ChangeLeadStatus(Entity lead, IOrganizationService service, string errorMessage)
            {
                //Reactivating Lead and Making it Open and Qualuified
                SetStateRequest setStateRequest = new SetStateRequest()
                {
                    EntityMoniker = new EntityReference
                    {
                        Id = lead.Id,
                        LogicalName = lead.LogicalName,
                    },
                    State = new OptionSetValue(lead.GetAttributeValue<OptionSetValue>(Lead.Status).Value),
                    Status = new OptionSetValue(lead.GetAttributeValue<OptionSetValue>(Lead.StatusReason).Value)
                };
                service.Execute(setStateRequest);
                errorMessage += "Info: Borrower is set to orignal state\n";
            }
            public void ChangeLeadQualified(Entity lead, IOrganizationService service, string errorMessage)
            {
                //Reactivating Lead and Making it Open and Qualuified
                SetStateRequest setStateRequest = new SetStateRequest()
                {
                    EntityMoniker = new EntityReference
                    {
                        Id = lead.Id,
                        LogicalName = lead.LogicalName,
                    },
                    State = new OptionSetValue(1),
                    Status = new OptionSetValue(1)
                };
                service.Execute(setStateRequest);
                errorMessage += "Info: Borrower is set to orignal state\n";
            }
            public void ChangeLoanStatus(Entity loan, IOrganizationService service)
            {
                //Reactivating Lead and Making it Open and Qualuified
                SetStateRequest setStateRequest = new SetStateRequest()
                {
                    EntityMoniker = new EntityReference
                    {
                        Id = loan.Id,
                        LogicalName = loan.LogicalName,
                    },
                    State = new OptionSetValue(loan.GetAttributeValue<OptionSetValue>(Loan.Status).Value),
                    Status = new OptionSetValue(loan.GetAttributeValue<OptionSetValue>(Loan.StatusReason).Value)
                };
                service.Execute(setStateRequest);
                errorMessage += "Info: Loan is set to orignal state\n";
            }
        }
    }
}
