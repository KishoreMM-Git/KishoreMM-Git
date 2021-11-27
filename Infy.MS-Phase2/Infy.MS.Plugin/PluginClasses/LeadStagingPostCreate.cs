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
using System.Net;

namespace Infy.MS.Plugins
{
    public class LeadStagingPostCreate : BasePlugin
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
            //yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Asynchronous, "Create");
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
            string generalNoteInfo = string.Empty;
            IExtendedPluginContext extendedPluginContext = null;

            IOrganizationService InitiatingUserOrganizationService = null;
            bool isLoanOfficer = false;
            bool ContactGroupCoBorrower = false;
            public void ProcessImportRecord(IExtendedPluginContext context)
            {
                IOrganizationService service = context.SystemOrganizationService;
                InitiatingUserOrganizationService = context.InitiatingUserOrganizationService;
                Entity leadStaging = null;
                Common objCommon = new Common();
                objCommon.contextUserId = context.InitiatingUserId;
                if (context.MessageName.ToLower() == "update" && context.Depth > 1)
                    return;
                try
                {
                    if (context.PostEntityImages.Contains("PostImage") && context.PostEntityImages["PostImage"] is Entity)
                    {
                        leadStaging = (Entity)context.PostEntityImages["PostImage"];
                        if (leadStaging.Contains(LeadStaging.Owner))
                        {
                            objCommon.stagingRecordOwner = context.InitiatingUserId;// leadStaging.GetAttributeValue<EntityReference>(LeadStaging.Owner).Id;
                        }
                        //if(leadStaging.Contains(LeadStaging.LeadSource))
                        //{
                        //    if(!leadStaging.GetAttributeValue<string>(LeadStaging.LeadSource).Equals("PCL Automated Import",StringComparison.OrdinalIgnoreCase) && context.Event.Mode.ToString()== "Synchronous")
                        //    {
                        //        return;
                        //    }
                        //    else if (leadStaging.GetAttributeValue<string>(LeadStaging.LeadSource).Equals("PCL Automated Import", StringComparison.OrdinalIgnoreCase) && context.Event.Mode.ToString() == "Asynchronous")
                        //    {
                        //        return;
                        //    }
                        //}
                        //else
                        //{
                        //    if (context.Event.Mode.ToString() == "Synchronous")
                        //    {
                        //        return;
                        //    }
                        //}
                    }
                    else if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                    {
                        leadStaging = (Entity)context.InputParameters["Target"];
                    }
                    //return if plugin primary entity is not Lead
                    if (leadStaging.LogicalName != LeadStaging.EntityName)
                        return;

                    try
                    {

                        //Handling Annual Income Range Value
                        //Split AnnualIncome
                        if (leadStaging.Contains(LeadStaging.LeadAnnualIncome))
                        {
                            var mappingValue = leadStaging.GetAttributeValue<string>(LeadStaging.LeadAnnualIncome);
                            Tuple<decimal, decimal> anunualImcome = objCommon.GetAnnualIncomeMinMaxRange(mappingValue.Replace("$", "").ToString().Replace(",", ""));
                            decimal annualIncomeMin = anunualImcome.Item1;
                            decimal annualIncomeMax = anunualImcome.Item2;
                            leadStaging[LeadStaging.LeadAnnualIncome] = annualIncomeMax.ToString();
                        }
                        //Getting Notes info
                        
                        if (leadStaging.Contains(LeadStaging.GeneralNoteInfo))
                            generalNoteInfo = leadStaging.GetAttributeValue<string>(LeadStaging.GeneralNoteInfo);
                        if(leadStaging.Contains(LeadStaging.ContactGroupExternalID))
                        {
                            if(leadStaging.GetAttributeValue<string>(LeadStaging.ContactGroupExternalID).Equals(Constants.CoBorrower,StringComparison.OrdinalIgnoreCase))
                            {
                                leadStaging[LeadStaging.ContactGroupExternalID] = ""+ Constants.CoBorrower+",Lead";
                                ContactGroupCoBorrower = true;
                                errorMessage += "Info: Co-Borrower Treated as Co-Borrower,Lead In Contact Group External Id\n";
                            }
                        }

                        //fetch all configurations related to StagingPostCreate
                        dcConfigDetails = Common.FetchConfigDetails(Constants.AppConfigSetup, service);
                        isLoanOfficer = objCommon.CheckIsLoanOfficer(service, objCommon.stagingRecordOwner, dcConfigDetails);
                        //create/update Lead record
                        UpsertLeadDetails(leadStaging, service, ref errorMessage, context);
                    }
                    catch (Exception ex)
                    {
                        errorMessage += ex.Message;
                    }
                    errorLogConfig = dcConfigDetails.ContainsKey(Constants.CreateErrorLog) ? Convert.ToBoolean(dcConfigDetails[Constants.CreateErrorLog].ToString()) : false;
                    if (errorLogConfig)
                    {
                        if (!string.IsNullOrEmpty(errorMessage))
                            objCommon.CreateErrorLog(leadStaging.Id.ToString(), errorMessage, service);
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidPluginExecutionException(ex.Message);
                }
            }

            public void UpsertLeadDetails(Entity leadStaging, IOrganizationService service, ref string errorMessage, IExtendedPluginContext context)
            {
                //Fetch Mapping Details [ImportDetailsMapping]
                extendedPluginContext = context;
                Guid importMasterDataId = Guid.Empty;
                Entity objEntity = null;
                string importProcessName = string.Empty;
                EntityReference defaultTeam = null;
                EntityReference manualImport = null;
                string contactGroup = string.Empty;
                string entityName = string.Empty;
                Common objCommon = new Common();

                //Contact US Web Site and LO Web Site //LDW LEAD and Data Migration
                if (leadStaging.Attributes.Contains(LeadStaging.ImportProcessName))
                {
                    importProcessName = leadStaging.GetAttributeValue<string>(LeadStaging.ImportProcessName);
                    // if (importProcessName.Equals(Constants.BankRate, StringComparison.OrdinalIgnoreCase) || importProcessName.Equals(Constants.Kunversion,StringComparison.OrdinalIgnoreCase)|| importProcessName.Equals(Constants.LeadPops,StringComparison.OrdinalIgnoreCase)||importProcessName.Equals(Constants.IDMContactUSWebsite, StringComparison.OrdinalIgnoreCase) || importProcessName.Equals(Constants.IDMLOWebsite, StringComparison.OrdinalIgnoreCase) || importProcessName.Equals(Constants.IDMEDWLead, StringComparison.OrdinalIgnoreCase))
                    if (importProcessName.Equals(Constants.MovementInternal, StringComparison.OrdinalIgnoreCase) || importProcessName.Equals(Constants.BankRate, StringComparison.OrdinalIgnoreCase) || importProcessName.Equals(Constants.BlendMovehome, StringComparison.OrdinalIgnoreCase) || importProcessName.Equals(Constants.Kunversion, StringComparison.OrdinalIgnoreCase) || importProcessName.Equals(Constants.LendingTree, StringComparison.OrdinalIgnoreCase) || importProcessName.Equals(Constants.ZillowLeadSource, StringComparison.OrdinalIgnoreCase) || importProcessName.Equals(Constants.BoomTown, StringComparison.OrdinalIgnoreCase) || importProcessName.Equals(Constants.CINC, StringComparison.OrdinalIgnoreCase) || importProcessName.Equals(Constants.LeadPops, StringComparison.OrdinalIgnoreCase) || importProcessName.Equals(Constants.IDMContactUSWebsite, StringComparison.OrdinalIgnoreCase) || importProcessName.Equals(Constants.IDMLOWebsite, StringComparison.OrdinalIgnoreCase) || importProcessName.Equals(Constants.IDMEDWLead, StringComparison.OrdinalIgnoreCase))
                    {
                        if (importProcessName.Equals(Constants.MovementInternal, StringComparison.OrdinalIgnoreCase))
                        {
                            var stateName = FormatStateValue(service, leadStaging);
                            if (!string.IsNullOrEmpty(stateName))
                                leadStaging[LeadStaging.LeadState] = stateName;
                        }
                        entityName = Lead.EntityName;
                        if (!leadStaging.Contains(Lead.LOExternalId) && importProcessName.Equals(Constants.IDMEDWLead, StringComparison.OrdinalIgnoreCase))
                        {
                            manualImport = new EntityReference(SystemUser.EntityName, context.UserId);
                            if (manualImport != null)
                                objCommon.manualImportReference = manualImport;
                            if (!leadStaging.Contains(LeadStaging.LOExternalID))
                                leadStaging.Attributes[LeadStaging.LOExternalID] = objCommon.GetLOExternalId(manualImport, service);
                        }
                        if (leadStaging.Contains(LeadStaging.ContactGroupExternalID))
                            contactGroup = leadStaging.GetAttributeValue<string>(LeadStaging.ContactGroupExternalID);
                        ProcessContactGroupImport(manualImport, ref defaultTeam, contactGroup, service, ref importProcessName, ref entityName, objCommon, ref leadStaging);
                    }
                    //Manual Import
                    else
                    {
                        string integrationUserConfigValue = context.GetConfigValue<string>(Constants.D365CRM_EDW_INTEGRATION_Id, Constants.AppConfigSetup);
                        GetCrmUserGuid(ref integrationUserConfigValue, service);
                        if (!context.UserId.Equals(new Guid(integrationUserConfigValue)))
                        {
                            importProcessName = Constants.IDMEDWLead;
                            entityName = Lead.EntityName;
                            if (!leadStaging.Contains(Lead.LOExternalId))
                            {
                                manualImport = new EntityReference(SystemUser.EntityName, context.UserId);
                                if (manualImport != null)
                                    objCommon.manualImportReference = manualImport;
                                if (!leadStaging.Contains(LeadStaging.LOExternalID))
                                    leadStaging.Attributes[LeadStaging.LOExternalID] = objCommon.GetLOExternalId(manualImport, service);
                            }
                            objCommon.manualImportReference = manualImport;
                            if (leadStaging.Contains(LeadStaging.ContactGroupExternalID))
                                contactGroup = leadStaging.GetAttributeValue<string>(LeadStaging.ContactGroupExternalID);
                            ProcessContactGroupImport(manualImport, ref defaultTeam, contactGroup, service, ref importProcessName, ref entityName, objCommon, ref leadStaging);
                        }
                    }
                }
                //Manual Import
                else if (!leadStaging.Attributes.Contains(LeadStaging.ImportProcessName))
                {
                    string integrationUserConfigValue = context.GetConfigValue<string>(Constants.D365CRM_EDW_INTEGRATION_Id, Constants.AppConfigSetup);
                    GetCrmUserGuid(ref integrationUserConfigValue, service);
                    if (!context.UserId.Equals(new Guid(integrationUserConfigValue)))
                    {
                        importProcessName = Constants.IDMEDWLead;
                        entityName = Lead.EntityName;
                        if (!leadStaging.Contains(Lead.LOExternalId))
                        {
                            manualImport = new EntityReference(SystemUser.EntityName, context.UserId);
                            if (manualImport != null)
                                objCommon.manualImportReference = manualImport;
                            if (!leadStaging.Contains(LeadStaging.LOExternalID))
                                leadStaging.Attributes[LeadStaging.LOExternalID] = objCommon.GetLOExternalId(manualImport, service);
                        }
                        if (leadStaging.Contains(LeadStaging.ContactGroupExternalID))
                            contactGroup = leadStaging.GetAttributeValue<string>(LeadStaging.ContactGroupExternalID);
                        ProcessContactGroupImport(manualImport, ref defaultTeam, contactGroup, service, ref importProcessName, ref entityName, objCommon, ref leadStaging);
                    }
                }
            }
            /// <summary>
            /// Splitting the Contact Group if it comes with ',' seperation
            /// </summary>
            /// <param name="contactGroup"></param>
            /// <returns></returns>
            public List<string> ContactGroupSplit(string contactGroup)
            {
                List<string> contactGroupList = new List<string>();
                if (contactGroup.Contains(","))
                {
                    IEnumerable<string> trimmed = contactGroup.Split(',').Select(s => s.Trim());
                    contactGroupList = trimmed.Distinct().ToList();
                }
                else
                    contactGroupList.Add(contactGroup);
                return contactGroupList;
            }
            /// <summary>
            /// Get the process Name and Entity Name based on the Contact Group
            /// </summary>
            /// <param name="contactGroup"></param>
            /// <param name="service"></param>
            /// <param name="importProcessName"></param>
            /// <param name="entityName"></param>
            /// <param name="objCommon"></param>
            public void ProcessContactGroupImport(EntityReference manualImport, ref EntityReference defaultTeam, string contactGroup, IOrganizationService service, ref string importProcessName, ref string entityName, Common objCommon, ref Entity leadStaging)
            {

                string formattedContactGroup = string.Empty;
                Guid importMasterDataId = Guid.Empty;
                List<string> contactGroupList = ContactGroupSplit(contactGroup);
                Entity contactTypeEntity = null;
                if (!string.IsNullOrEmpty(contactGroup))
                {
                    if (contactGroupList != null)
                    {
                        foreach (string groupName in contactGroupList)//Handling (,) seperated contact groups
                        {
                            formattedContactGroup = groupName;
                            FormatContactGroup(ref formattedContactGroup, service);
                            //Making the Lead Status based on Contact Group, only for Dead Lead
                            if (formattedContactGroup.Contains("Dead Lead"))
                            {
                                leadStaging[LeadStaging.LeadStatus] = "Dead Lead";
                            }
                            //defaultTeam = null;
                            if (formattedContactGroup != string.Empty)
                            {
                                contactTypeEntity = objCommon.GetContactType(formattedContactGroup, service);
                                if (contactTypeEntity != null)
                                {
                                    if (contactTypeEntity.Contains(ContactType.CreateContact) && contactTypeEntity.GetAttributeValue<bool>(ContactType.CreateContact))
                                    {
                                        importProcessName = Constants.OtherContact;
                                        entityName = Contact.EntityName;
                                        ProcessRecordUpsert(manualImport, contactTypeEntity, formattedContactGroup, ref entityName, ref importProcessName, ref importMasterDataId, ref defaultTeam, service, ref objCommon, ref leadStaging);
                                    }
                                    else if (contactTypeEntity.Contains(ContactType.CreateCustomer) && contactTypeEntity.GetAttributeValue<bool>(ContactType.CreateCustomer))
                                    {
                                        importProcessName = Constants.CoBorrower;
                                        entityName = Account.EntityName;
                                        ProcessRecordUpsert(manualImport, contactTypeEntity, formattedContactGroup, ref entityName, ref importProcessName, ref importMasterDataId, ref defaultTeam, service, ref objCommon, ref leadStaging);
                                    }
                                    else if (contactTypeEntity.Contains(ContactType.CreateLead) && contactTypeEntity.GetAttributeValue<bool>(ContactType.CreateLead))
                                    {
                                        importProcessName = Constants.IDMEDWLead;
                                        entityName = Lead.EntityName;
                                        ProcessRecordUpsert(manualImport, contactTypeEntity, formattedContactGroup, ref entityName, ref importProcessName, ref importMasterDataId, ref defaultTeam, service, ref objCommon, ref leadStaging);
                                    }
                                    else
                                    {
                                        //Exclduing the Contact Types which are having the Create lead , Create Customer and Create Contact flags are 'NO'
                                        validationStatus = false;
                                        objCommon.UpdateStagingLog(leadStaging.Id, Guid.Empty, LeadStaging.EntityName, "Info:Contact Groups is excluded for Record creation", validationStatus, service);
                                        if (contactGroupList.Count == 1)
                                            return;
                                    }
                                }
                                else if (contactTypeEntity == null)
                                {
                                    importProcessName = Constants.IDMEDWLead;
                                    entityName = Lead.EntityName;
                                    ProcessRecordUpsert(manualImport, contactTypeEntity, formattedContactGroup, ref entityName, ref importProcessName, ref importMasterDataId, ref defaultTeam, service, ref objCommon, ref leadStaging);
                                }
                            }
                        }
                    }
                }
                else if (importProcessName.Equals(Constants.IDMEDWLead, StringComparison.OrdinalIgnoreCase))
                {
                    importProcessName = Constants.IDMEDWLead;
                    entityName = Lead.EntityName;
                    ProcessRecordUpsert(manualImport, contactTypeEntity, formattedContactGroup, ref entityName, ref importProcessName, ref importMasterDataId, ref defaultTeam, service, ref objCommon, ref leadStaging);
                }
                else if (importProcessName.Equals(Constants.MovementInternal, StringComparison.OrdinalIgnoreCase) || importProcessName.Equals(Constants.Kunversion, StringComparison.OrdinalIgnoreCase) || importProcessName.Equals(Constants.LendingTree, StringComparison.OrdinalIgnoreCase) || importProcessName.Equals(Constants.BankRate, StringComparison.OrdinalIgnoreCase) || importProcessName.Equals(Constants.ZillowLeadSource, StringComparison.OrdinalIgnoreCase) || importProcessName.Equals(Constants.BoomTown, StringComparison.OrdinalIgnoreCase) || importProcessName.Equals(Constants.CINC, StringComparison.OrdinalIgnoreCase) || importProcessName.Equals(Constants.BlendMovehome, StringComparison.OrdinalIgnoreCase) || importProcessName.Equals(Constants.LeadPops, StringComparison.OrdinalIgnoreCase) || (importProcessName.Equals(Constants.IDMLOWebsite, StringComparison.OrdinalIgnoreCase)) || importProcessName.Equals(Constants.IDMContactUSWebsite, StringComparison.OrdinalIgnoreCase))
                {
                    ProcessRecordUpsert(manualImport, contactTypeEntity, formattedContactGroup, ref entityName, ref importProcessName, ref importMasterDataId, ref defaultTeam, service, ref objCommon, ref leadStaging);
                }
            }
            /// <summary>
            /// Transforming the file data into CRM Data format
            /// </summary>
            /// <param name="contactGroup">contact group from import</param>
            public void FormatContactGroup(ref string contactGroup, IOrganizationService service)
            {
                EntityCollection ec = new EntityCollection();
                //with COntact Type Name
                string fetchXml = string.Format(@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
                                                  "<entity name='ims_contacttype'>" +
                                                    "<attribute name='ims_contacttypeid' />" +
                                                    "<attribute name='ims_name' />" +
                                                    "<attribute name='createdon' />" +
                                                    "<order attribute='ims_name' descending='false' />" +
                                                    "<filter type='and'>" +
                                                      "<condition attribute='ims_name' operator='eq' value='" + WebUtility.HtmlEncode(contactGroup) + "' />" +
                                                      "<condition attribute='statecode' operator='eq' value='0' />" +
                                                    "</filter>" +
                                                  "</entity>" +
                                                "</fetch>");
                //Wirh Contact Type Possible Values
                string fetchXmlPossibleValues = string.Format(@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
                                                  "<entity name='ims_contacttype'>" +
                                                    "<attribute name='ims_contacttypeid' />" +
                                                    "<attribute name='ims_name' />" +
                                                    "<attribute name='createdon' />" +
                                                    "<order attribute='ims_name' descending='false' />" +
                                                    "<filter type='and'>" +
                                                      "<condition attribute='statecode' operator='eq' value='0' />" +
                                                      "<condition attribute='ims_contacttypepossiblevalues' operator='like' value='%" + WebUtility.HtmlEncode(contactGroup) + "%' />" +
                                                    "</filter>" +
                                                  "</entity>" +
                                                "</fetch>");
                ec = service.RetrieveMultiple(new FetchExpression(fetchXml));
                if (ec.Entities.Count > 0)
                {
                    if (ec.Entities.FirstOrDefault().Contains(ContactType.PrimaryName))
                    {
                        contactGroup = ec.Entities.FirstOrDefault().GetAttributeValue<string>(ContactType.PrimaryName);
                    }
                }
                else
                {
                    ec = service.RetrieveMultiple(new FetchExpression(fetchXmlPossibleValues));
                    if (ec.Entities.Count > 0)
                    {
                        if (ec.Entities.FirstOrDefault().Contains(ContactType.PrimaryName))
                        {
                            contactGroup = ec.Entities.FirstOrDefault().GetAttributeValue<string>(ContactType.PrimaryName);
                        }
                    }
                }
            }

            /// <summary>
            /// Process the Record Update or Insert
            /// </summary>
            /// <param name="contactTypeEntity"></param>
            /// <param name="contactGroup"></param>
            /// <param name="entityName"></param>
            /// <param name="importProcessName"></param>
            /// <param name="importMasterDataId"></param>
            /// <param name="defaultTeam"></param>
            /// <param name="service"></param>
            /// <param name="objCommon"></param>
            /// <param name="leadStaging"></param>
            public void ProcessRecordUpsert(EntityReference manualImport, Entity contactTypeEntity, string contactGroup, ref string entityName, ref string importProcessName, ref Guid importMasterDataId, ref EntityReference defaultTeam, IOrganizationService service, ref Common objCommon, ref Entity leadStaging)
            {
                mappings.Clear();
                Entity objEntity = new Entity();
                if (importProcessName != string.Empty)
                {
                    //Get ImportDataMaster GUID & Default Team based on Import Process Name
                    importMasterDataId = Common.GetImportDataMasterAndDefaultTeam(importProcessName, ref defaultTeam, service);
                    if (defaultTeam != null)
                        objCommon.defaultTeamReference = defaultTeam;
                }

                //return if import data Master ID is null
                if (importMasterDataId == Guid.Empty)
                {
                    validationStatus = false;
                    objCommon.UpdateStagingLog(leadStaging.Id, Guid.Empty, LeadStaging.EntityName, "Import Process Name is Invalid or Import Data Master Configuration is missing.", validationStatus, service);
                    return;
                }
                if (!objCommon.FetchMappings(importMasterDataId, ref mappings, service, ref errorMessage))
                {
                    validationStatus = false;
                    objCommon.UpdateStagingLog(leadStaging.Id, Guid.Empty, LeadStaging.EntityName, errorMessage, validationStatus, service);
                    return;
                }
                //Check for Mandatory fields and Max Length Allowed
                objCommon.MandatoryValidation(leadStaging, mappings, ref validationStatus, ref canReturn, ref errorMessage, dcConfigDetails);

                //Form Lead/Contact/OtherContact Object based on mappings which is require for Create/Update
                objEntity = objCommon.FormTargetEntityObject(InitiatingUserOrganizationService, entityName, leadStaging, mappings, service, ref validationStatus, ref canReturn, ref errorMessage, dcConfigDetails);

                if (canReturn)
                {
                    objCommon.UpdateStagingLog(leadStaging.Id, Guid.Empty, LeadStaging.EntityName, errorMessage, validationStatus, service);
                    return;
                }
                //Mapping Of Contact Type lookup field for Other Contact
                if (entityName == Contact.EntityName)
                {
                    if (!string.IsNullOrEmpty(contactGroup))
                    {
                        //Entity contacttypeId = objCommon.GetContactType(contactGroup, service);
                        if (contactTypeEntity != null)
                        {
                            if (contactTypeEntity.Id != Guid.Empty)
                                objEntity[Contact.ContactType] = new EntityReference(ContactType.EntityName, contactTypeEntity.Id);
                        }
                    }
                }
                CheckRecordCreateUpdate(manualImport, contactGroup, ref objEntity, contactTypeEntity, ref entityName, ref importProcessName, ref importMasterDataId, ref defaultTeam, service, ref objCommon, ref leadStaging);
            }
            /// <summary>
            /// Check record is existed or not if not create, else update record with available info
            /// </summary>
            /// <param name="objEntity"></param>
            /// <param name="contactTypeEntity"></param>
            /// <param name="entityName"></param>
            /// <param name="importProcessName"></param>
            /// <param name="importMasterDataId"></param>
            /// <param name="defaultTeam"></param>
            /// <param name="service"></param>
            /// <param name="objCommon"></param>
            /// <param name="leadStaging"></param>
            public void CheckRecordCreateUpdate(EntityReference manualImport, string contactGroup, ref Entity objEntity, Entity contactTypeEntity, ref string entityName, ref string importProcessName, ref Guid importMasterDataId, ref EntityReference defaultTeam, IOrganizationService service, ref Common objCommon, ref Entity leadStaging)
            {


                //This service is used to make the duplicate detection rules need to run in user context
                IOrganizationService userOrgService = extendedPluginContext.InitiatingUserOrganizationService;
                Guid entityId = Guid.Empty;
                var exsitingleadStatus = -1;
                //Check for Existing Active Lead/Contact/Other Contact record
                bool existingEntity = false;
                bool isMovementDirectLead = false;
                Entity objExistingEntity = null;
                Guid zillowSellersAgentContactId = Guid.Empty;
                bool LOExternalIdIsMD = false;
                bool isMDLeadTransfer = false;
                bool vendorDupLead = false;
                //This block of code handles the MD Lead Source Integration..
                if (!leadStaging.Contains(LeadStaging.ContactExternalId) && objEntity.LogicalName == Lead.EntityName && objEntity.Contains(Lead.LeadSource))
                {
                    // objCommon.IdentifyMovementDirectLead(leadStaging,service);
                    isMovementDirectLead = objCommon.CheckMovementDirectLeadUsingLeadSource(service, (EntityReference)objEntity.GetAttributeValue<EntityReference>(Lead.LeadSource), dcConfigDetails);
                    if (isMovementDirectLead)
                    {
                        objEntity.Attributes[Lead.OrginatedByMovementDirect] = isMovementDirectLead;
                        if (objEntity.LogicalName == Lead.EntityName)
                            objEntity.Attributes[Lead.IsFromLeadStaging] = true;
                        var leadSource = objCommon.GetMessage(Constants.DefaultLeadStatus_MovementDirect, dcConfigDetails);
                        if (!string.IsNullOrEmpty(leadSource))
                        {
                            var defaultMovementDirectLeadStatus = GetMovementDirectDefaultLeadSource(service, leadSource);
                            if (defaultMovementDirectLeadStatus != Guid.Empty)
                                objEntity[Lead.LeadStatus] = new EntityReference(LeadStatus.EntityName, defaultMovementDirectLeadStatus);
                        }
                        objExistingEntity = objCommon.CheckMovementDirectDuplicateLead(objEntity, service, dcConfigDetails);
                        if (objExistingEntity != null && objExistingEntity.Id != Guid.Empty)
                        {
                            vendorDupLead = true;
                            if (objEntity.Contains(Lead.LeadStatus))
                            {
                                objEntity.Attributes.Remove(Lead.LeadStatus);
                            }
                        }
                    }
                }

                //Identify the MD Lead with security role of LO External Id with Movement Direct Role assigned or Not, from LDW Job
                //Stage 1
                if (leadStaging.Contains(LeadStaging.LOExternalID) && (objEntity.LogicalName == Lead.EntityName || objEntity.LogicalName == Contact.EntityName || objEntity.LogicalName == Account.EntityName) && leadStaging.Contains(LeadStaging.ContactExternalId) && !leadStaging.Contains(LeadStaging.MDLOExternalId))
                {
                    isMovementDirectLead = objCommon.IdentifyMovementDirectLead(leadStaging.GetAttributeValue<string>(LeadStaging.LOExternalID), service, dcConfigDetails);
                    if (isMovementDirectLead)
                    {
                        LOExternalIdIsMD = true;
                        isMovementDirectLead = true;
                        objEntity.Attributes[Lead.OrginatedByMovementDirect] = isMovementDirectLead;
                        if (objEntity.LogicalName == Lead.EntityName)
                            objEntity.Attributes[Lead.IsFromLeadStaging] = true;
                        errorMessage += "Info:MD Record Identified with Security Role at LO Externnal Id\n";
                    }
                }
                //Stage 2 Transfer Case from MD to Retail
                if (!isMovementDirectLead && (objEntity.LogicalName == Lead.EntityName || objEntity.LogicalName == Contact.EntityName || objEntity.LogicalName == Account.EntityName) && leadStaging.Contains(LeadStaging.MDLOExternalId) && leadStaging.Contains(LeadStaging.LOExternalID))
                {
                    isMDLeadTransfer = true;
                    isMovementDirectLead = true;
                    objEntity.Attributes[Lead.OrginatedByMovementDirect] = isMovementDirectLead;
                    if (objEntity.LogicalName == Lead.EntityName)
                        objEntity.Attributes[Lead.IsFromLeadStaging] = true;
                    errorMessage += "Info:MD Record Transfer Processing\n";
                }
                ////Stage 3 with Provider and MD Lead Source 
                //if (!isMovementDirectLead && (leadStaging.Contains(LeadStaging.Provider) || leadStaging.Contains(LeadStaging.MovementDirectLeadSource)))
                //{
                //    isMovementDirectLead = true;
                //    objEntity.Attributes[Lead.OrginatedByMovementDirect] = isMovementDirectLead;
                //    objEntity.Attributes[Lead.IsFromLeadStaging] = true;

                //}
                //If Movement Direct Lead from LDW Job, 
                //overwrite the Default Team from Marketing Team to Movement Direct Team in case of LO External Id is MD Loan Officer
                if (!string.IsNullOrEmpty(Constants.VeloLDW) && isMovementDirectLead && LOExternalIdIsMD)
                {
                    Common.GetImportDataMasterAndDefaultTeam(Constants.VeloLDW, ref defaultTeam, service);
                    if (defaultTeam != null)
                        objCommon.defaultTeamReference = defaultTeam;
                }

                // Pre-Popultre LO External Id from Contact and LO Website, because LO External Id would not be presnt,this would help in finding the duplicate
                if (objEntity.Contains(Lead.Owner) && leadStaging.Contains(LeadStaging.LoanOfficerEmail)
                    && !leadStaging.Contains(LeadStaging.LOExternalID) &&
                    (importProcessName.Equals(Constants.IDMContactUSWebsite, StringComparison.OrdinalIgnoreCase)
                    || importProcessName.Equals(Constants.IDMLOWebsite, StringComparison.OrdinalIgnoreCase)))
                {
                    objEntity[Lead.LOExternalId] = objCommon.GetLOExternalIdUsingEmailId(
                                                   leadStaging.GetAttributeValue<string>(LeadStaging.LoanOfficerEmail), service);
                }
                //if (entityName == Lead.EntityName && leadStaging.Contains(LeadStaging.BlendApplicationSource) && leadStaging.GetAttributeValue<string>(LeadStaging.BlendApplicationSource) == "MovehomeCRM")
                //    objExistingEntity = objCommon.CheckBlendLeadExistance(leadStaging, service);
                if (entityName == Lead.EntityName && !isMovementDirectLead)
                    objExistingEntity = objCommon.CheckLeadExistiance(true, isLoanOfficer, objEntity, userOrgService, leadStaging, dcConfigDetails);
                if (entityName == Lead.EntityName && isMovementDirectLead && (isMDLeadTransfer || LOExternalIdIsMD))
                    objExistingEntity = objCommon.CheckLeadExistiance(true, isLoanOfficer, objEntity, userOrgService, leadStaging, dcConfigDetails, isMovementDirectLead, LOExternalIdIsMD, isMDLeadTransfer);
                if (objExistingEntity == null && entityName == Lead.EntityName && isMovementDirectLead && (isMDLeadTransfer || LOExternalIdIsMD))
                    objExistingEntity = objCommon.CheckLeadExistiance(true, isLoanOfficer, objEntity, userOrgService, leadStaging, dcConfigDetails, false, LOExternalIdIsMD, false);

                if (entityName == Account.EntityName && !isMovementDirectLead)
                    objExistingEntity = objCommon.CheckAccountExistance(true, isLoanOfficer, leadStaging, objEntity, userOrgService, dcConfigDetails);
                if (entityName == Account.EntityName && isMovementDirectLead && (isMDLeadTransfer || LOExternalIdIsMD))
                    objExistingEntity = objCommon.CheckAccountExistance(true, isLoanOfficer, leadStaging, objEntity, userOrgService, dcConfigDetails, isMovementDirectLead, LOExternalIdIsMD, isMDLeadTransfer);
                if (objExistingEntity == null && entityName == Account.EntityName && isMovementDirectLead && (isMDLeadTransfer || LOExternalIdIsMD))
                    objExistingEntity = objCommon.CheckAccountExistance(true, isLoanOfficer, leadStaging, objEntity, userOrgService, dcConfigDetails, false, LOExternalIdIsMD, false);

                if (entityName == Contact.EntityName && !isMovementDirectLead)
                    objExistingEntity = objCommon.CheckPreviousOtherContact(isLoanOfficer, leadStaging, objEntity, userOrgService, dcConfigDetails);
                if (entityName == Contact.EntityName && isMovementDirectLead && (isMDLeadTransfer || LOExternalIdIsMD))
                    objExistingEntity = objCommon.CheckPreviousOtherContact(isLoanOfficer, leadStaging, objEntity, userOrgService, dcConfigDetails, isMovementDirectLead, LOExternalIdIsMD, isMDLeadTransfer);
                if (objExistingEntity == null && entityName == Contact.EntityName && isMovementDirectLead && (isMDLeadTransfer || LOExternalIdIsMD))
                    objExistingEntity = objCommon.CheckPreviousOtherContact(isLoanOfficer, leadStaging, objEntity, userOrgService, dcConfigDetails, false, LOExternalIdIsMD, false);

                if (objExistingEntity != null && objExistingEntity.Id != Guid.Empty)
                {
                    if (defaultTeam != null && !objEntity.Contains(Lead.Owner))
                        objEntity[Lead.Owner] = defaultTeam;

                    existingEntity = true;
                    entityId = objExistingEntity.Id;
                    if (objExistingEntity.Contains(Lead.Status))
                    {
                        exsitingleadStatus = objExistingEntity.GetAttributeValue<OptionSetValue>(Lead.Status).Value;
                    }
                }

                //Existing Active Lead/Contact/OtherContact record present in SYSTEM. update dirty fields
                if (existingEntity)
                {
                    if(objExistingEntity.LogicalName==Lead.EntityName && ContactGroupCoBorrower==false)
                    {
                        objEntity[Lead.BorrowerType] = new OptionSetValue(Convert.ToInt32(Lead.BorrowerType_OptionSet.PrimaryBorrower));
                    }
                    if (leadStaging.Contains(LeadStaging.BulkLeadContactTags) && (objEntity.LogicalName == Lead.EntityName || objEntity.LogicalName == Contact.EntityName))
                    {
                        errorMessage += "Info: Tags cannot be associated for existing leads/contacts";
                    }
                    if (!leadStaging.Contains(Lead.LeadStatus) && objEntity.Contains(Lead.LeadStatus) && objExistingEntity.Contains(Lead.LeadStatus))
                    {
                        objEntity.Attributes.Remove(Lead.LeadStatus);
                    }
                    if (objEntity.Contains(Lead.LeadSource) && objExistingEntity.Contains(Lead.LeadSource))
                    {
                        objEntity.Attributes.Remove(Lead.LeadSource); // Should not overwrite this for existing lead records
                    }
                    else if (objEntity.Contains(Account.ContactSource) && objExistingEntity.Contains(Account.ContactSource))
                    {
                        objEntity.Attributes.Remove(Account.ContactSource); // Should not overwrite this for existing Co-Borrowers records
                    }
                    else if (objEntity.Contains(Contact.ContactSource) && objExistingEntity.Contains(Contact.ContactSource))
                    {
                        objEntity.Attributes.Remove(Contact.ContactSource); // Should not overwrite this for existing Contact Records records
                    }
                    bool leadReactivated = false;
                    bool leadReactivatedDisqualified = false;
                    if (objExistingEntity.Contains(Lead.Status))
                    {
                        // var leadStatus = objExistingEntity.GetAttributeValue<OptionSetValue>(Lead.Status).Value;
                        if (exsitingleadStatus == 1)
                        {
                            SetStateRequest setStateRequest = new SetStateRequest()
                            {
                                EntityMoniker = new EntityReference
                                {
                                    Id = objExistingEntity.Id,
                                    LogicalName = objExistingEntity.LogicalName,
                                },
                                State = new OptionSetValue(0),
                                Status = new OptionSetValue(1)
                            };
                            service.Execute(setStateRequest);
                            leadReactivated = true;
                            errorMessage += "Info:Qualified Borrower is reactivated to Update\n";
                        }
                        else if (exsitingleadStatus == 2)
                        {
                            SetStateRequest setStateRequest = new SetStateRequest()
                            {
                                EntityMoniker = new EntityReference
                                {
                                    Id = objExistingEntity.Id,
                                    LogicalName = objExistingEntity.LogicalName,
                                },
                                State = new OptionSetValue(0),
                                Status = new OptionSetValue(1)
                            };
                            service.Execute(setStateRequest);
                            leadReactivatedDisqualified = true;
                            errorMessage += "Info:Disqulified Borrower is reactivated to Update\n";
                        }
                    }
                    objCommon.UpdateValidationMessage(string.Format(objCommon.GetMessage(Constants.DupliRecordFound, dcConfigDetails), entityName), ref errorMessage);
                    if (leadStaging.Contains(LeadStaging.OneTimeDataMigration) && leadStaging.GetAttributeValue<bool>(LeadStaging.OneTimeDataMigration))
                    {
                        if (entityId != Guid.Empty)
                            objCommon.UpdateStagingLog(leadStaging.Id, entityId, LeadStaging.EntityName, errorMessage, validationStatus, service, entityName);
                        //return the record if the data is one data migration
                        // return;
                    }
                    //Movement Direct Lead 
                    if (isMovementDirectLead)
                    {
                        //if(objEntity.Contains(Lead.LeadSource))
                        //{
                        //    objEntity.Attributes.Remove(Lead.LeadSource); // Should not overwrite this
                        //    //objExistingEntity.Attributes.Remove(Lead.LeadSource);
                        //}
                        if (isMDLeadTransfer)
                        {
                            //To Share the Lead to MD LO External ID( to MD LO)
                            if ((objEntity.LogicalName == Lead.EntityName || objEntity.LogicalName == Account.EntityName || objEntity.LogicalName == Contact.EntityName) && leadStaging.Contains(Lead.MDLOExternalId) && leadStaging.Contains(Lead.LOExternalId))
                            {
                                string errorMessageFromShare = string.Empty;
                                if (objEntity.Contains(Lead.Owner))
                                    ShareActivities(objEntity.GetAttributeValue<EntityReference>(Lead.Owner), service, objExistingEntity.ToEntityReference());

                                objCommon.ShareRecordsMDLoanOfficerExternalId(ref errorMessageFromShare, objExistingEntity.ToEntityReference(), leadStaging.GetAttributeValue<string>(Lead.MDLOExternalId), service, dcConfigDetails);
                                if (!string.IsNullOrEmpty(errorMessageFromShare))
                                    errorMessage += errorMessageFromShare;
                                else
                                    errorMessage += "Info:MD Record is Shared to MD EEID \n";
                                if (leadStaging.Contains(LeadStaging.ContactExternalId))
                                {
                                    objEntity[Lead.ExternalID] = leadStaging.GetAttributeValue<string>(LeadStaging.ContactExternalId);
                                }
                            }
                        }

                        if (entityId != Guid.Empty && !isMDLeadTransfer)
                        {
                            if (objEntity.LogicalName == Lead.EntityName)
                                objCommon.UpdateValidationMessage(string.Format(objCommon.GetMessage(Constants.MDDupliRecordFound, dcConfigDetails), entityName), ref errorMessage);
                            objCommon.UpdateStagingLog(leadStaging.Id, entityId, LeadStaging.EntityName, errorMessage, validationStatus, service, entityName);
                            if (vendorDupLead)//Return only if it is from Lead Source Integration
                                return;
                        }

                    }

                    //Remove ims_zillowselleresagent Attribute from Lead Object
                    if (importProcessName.Equals(Constants.ZillowLeadSource, StringComparison.OrdinalIgnoreCase) && entityName == Lead.EntityName)
                    {
                        if (objEntity.Attributes.Contains(Lead.ZillowSellersAgent))
                        {
                            if (objEntity.GetAttributeValue<EntityReference>(Lead.ZillowSellersAgent) != null)
                                zillowSellersAgentContactId = objEntity.GetAttributeValue<EntityReference>(Lead.ZillowSellersAgent).Id;
                            objEntity.Attributes.Remove(Lead.ZillowSellersAgent);
                        }
                    }
                    //if (objExistingEntity.LogicalName == Lead.EntityName && objExistingEntity.Contains(Lead.FromBlend) && objExistingEntity.GetAttributeValue<bool>(Lead.FromBlend) && leadStaging.Contains(LeadStaging.LeadSource) && leadStaging.GetAttributeValue<string>(LeadStaging.LeadSource)!="PCL Automated Import")
                    //    objEntity.Attributes.Remove(Lead.Owner);
                    objCommon.UpdateRecordIfDirty(objExistingEntity, objEntity, entityName, mappings, service, ref validationStatus, ref canReturn, ref errorMessage, importProcessName, manualImport, leadStaging.GetAttributeValue<bool>(LeadStaging.OneTimeDataMigration));
                    //setting ims_iscoborrower TRUE, if we get Contact as created from Lead, and again it gets from Co-Borrower we need to overwrite the existing contact 
                    if (objExistingEntity != null && objExistingEntity.LogicalName == Account.EntityName && !canReturn)
                    {
                        Entity updateAccount = new Entity(Account.EntityName);
                        updateAccount.Id = objExistingEntity.Id;
                        updateAccount.Attributes["ims_iscoborrower"] = true;
                        service.Update(updateAccount);
                    }
                    if (objExistingEntity != null && objExistingEntity.LogicalName == Contact.EntityName && !canReturn)
                    {

                        if (!string.IsNullOrEmpty(contactGroup))
                        {
                            //Entity contacttypeId = objCommon.GetContactType(contactGroup, service);
                            if (contactTypeEntity != null)
                            {
                                if (contactTypeEntity.Id != Guid.Empty)
                                {
                                    Entity updateContact = new Entity(Contact.EntityName);
                                    updateContact.Id = objExistingEntity.Id;
                                    if (contactTypeEntity != null)
                                    {
                                        updateContact[Contact.ContactType] = new EntityReference(ContactType.EntityName, contactTypeEntity.Id);
                                        service.Update(updateContact);
                                    }
                                }
                            }
                        }

                    }
                    if (!string.IsNullOrEmpty(contactGroup))
                        objCommon.MarketingListAssociation(dcConfigDetails, contactGroup, objExistingEntity, service);
                    if (canReturn)
                    {
                        objCommon.UpdateStagingLog(leadStaging.Id, Guid.Empty, LeadStaging.EntityName, errorMessage, validationStatus, service);
                        return;
                    }

                    //Associate Zillow Sellers Agent Contact to Lead
                    if (importProcessName.Equals(Constants.ZillowLeadSource, StringComparison.OrdinalIgnoreCase) && entityName == Lead.EntityName)
                    {
                        if (zillowSellersAgentContactId != Guid.Empty)
                        {
                            if (!objCommon.CheckAsscocationExists(service, zillowSellersAgentContactId, objEntity.Id))
                            {
                                EntityReference contactReference = new EntityReference(Contact.EntityName, zillowSellersAgentContactId);
                                EntityReference leadReference = new EntityReference(Lead.EntityName, objExistingEntity.Id);
                                objCommon.Asscociate(service, contactReference, leadReference);
                            }
                        }
                    }
                    //Qualifying the Lead Again, if it is reactivated
                    if (leadReactivated)
                    {
                        SetStateRequest setStateRequest = new SetStateRequest()
                        {
                            EntityMoniker = new EntityReference
                            {
                                Id = objExistingEntity.Id,
                                LogicalName = objExistingEntity.LogicalName,
                            },
                            State = new OptionSetValue(1),//Qualified
                            Status = new OptionSetValue(3)//Qualified
                        };
                        service.Execute(setStateRequest);
                        errorMessage += "Info:Borrower is Qualified\n";
                    }
                    ////Disqualifying the Lead Again, if it is reactivated
                    //if (leadReactivatedDisqualified)
                    //{
                    //    SetStateRequest setStateRequest = new SetStateRequest()
                    //    {
                    //        EntityMoniker = new EntityReference
                    //        {
                    //            Id = objExistingEntity.Id,
                    //            LogicalName = objExistingEntity.LogicalName,
                    //        },
                    //        State = new OptionSetValue(2),//Disqualified
                    //        Status = new OptionSetValue(4)//Lost
                    //    };
                    //    service.Execute(setStateRequest);
                    //    errorMessage += "Info:Borrower is Disqualifed\n";
                    //}
                }
                else
                {
                    try
                    {
                        //Assigning the Owner for the Lead Record
                        if (manualImport != null && !objEntity.Attributes.Contains(Lead.Owner))
                        {

                            objEntity[Lead.Owner] = manualImport;
                        }
                        else if (defaultTeam != null && !objEntity.Attributes.Contains(Lead.Owner))
                        {
                            objEntity[Lead.Owner] = defaultTeam;
                            objCommon.UpdateValidationMessage("Loan Officer/Owner not found in CRM System. Assigning" + entityName + "to Default Team", ref errorMessage);
                        }

                        //setting ims_iscoborrower true if we create the Account Record
                        if (entityName == Account.EntityName)
                            objEntity.Attributes["ims_iscoborrower"] = true;
                        if (entityName == Lead.EntityName)
                            objEntity.Attributes[Lead.OrginatedByMovementDirect] = isMovementDirectLead;
                        if (entityName == Lead.EntityName)
                        {
                            objEntity.Attributes[Lead.BorrowerType] = ContactGroupCoBorrower == true ? new OptionSetValue(Convert.ToInt32(Lead.BorrowerType_OptionSet.Co_Borrower)) : new OptionSetValue(Convert.ToInt32(Lead.BorrowerType_OptionSet.Co_Borrower));
                        }

                        if (entityName == Lead.EntityName && objEntity.Contains(Lead.FromBlend) && objEntity.GetAttributeValue<bool>(Lead.FromBlend))
                        {

                        }
                        //Update Contact Category

                        //if(objEntity.Contains(Contact.ContactType))
                        //{
                        //    objCommon.UpdateContactCategory(ref objEntity, (EntityReference)objEntity.Attributes[Contact.ContactType]);
                        //}

                        //If Record imported from Manual Import and If It is not One Time Migration record then Set ims_tosync to True
                        if (leadStaging.Contains(LeadStaging.OneTimeDataMigration) && !leadStaging.GetAttributeValue<bool>(LeadStaging.OneTimeDataMigration))
                        {
                            if (manualImport != null)
                            {
                                if (importProcessName.Equals(Constants.IDMEDWLead, StringComparison.OrdinalIgnoreCase) || importProcessName.Equals(Constants.OtherContact, StringComparison.OrdinalIgnoreCase))
                                {
                                    if (entityName == Lead.EntityName || entityName == Contact.EntityName)
                                    {
                                        objEntity.Attributes[Lead.ToSync] = true;
                                    }
                                }
                            }
                        }
                        //If One Time Migrated Record Update Lead to (ims_isonetimemigrationrecord) to true
                        if (leadStaging.Contains(LeadStaging.OneTimeDataMigration) && leadStaging.GetAttributeValue<bool>(LeadStaging.OneTimeDataMigration))
                        {

                            if (importProcessName.Equals(Constants.IDMEDWLead, StringComparison.OrdinalIgnoreCase))
                            {
                                if (entityName == Lead.EntityName)
                                {
                                    objEntity.Attributes[Lead.IsOneTimeMigratedRecord] = true;
                                }
                            }

                        }
                        //Remove ims_zillowselleresagent Attribute from Lead Object
                        if (importProcessName.Equals(Constants.ZillowLeadSource, StringComparison.OrdinalIgnoreCase) && entityName == Lead.EntityName)
                        {
                            if (objEntity.Attributes.Contains(Lead.ZillowSellersAgent))
                            {
                                if (objEntity.GetAttributeValue<EntityReference>(Lead.ZillowSellersAgent) != null)
                                    zillowSellersAgentContactId = objEntity.GetAttributeValue<EntityReference>(Lead.ZillowSellersAgent).Id;
                                objEntity.Attributes.Remove(Lead.ZillowSellersAgent);
                            }
                        }

                        objEntity.Id = (entityId = service.Create(objEntity));
                        //Associate tags in bulk action

                        if (leadStaging.Contains(LeadStaging.BulkLeadContactTags) && (objEntity.LogicalName == Lead.EntityName || objEntity.LogicalName == Contact.EntityName));
                        {
                            //objCommon.BuildTags(service, leadStaging.GetAttributeValue<string>(LeadStaging.BulkLeadContactTags),extendedPluginContext.InitiatingUserId, objEntity.Id, objEntity.LogicalName,ref errorMessage);
                        }
                        //To Process Failed Loans from Loan Staging,
                        if (objEntity.LogicalName == Lead.EntityName && objEntity.Contains(Lead.ExternalID))
                        {
                            ProcessFailedLoans(service, objEntity.Id);
                        }
                        //To Share the Lead to MD LO External ID( to MD LO)
                        if (objEntity.LogicalName == Lead.EntityName && leadStaging.Contains(Lead.MDLOExternalId) && leadStaging.Contains(Lead.LOExternalId))
                        {
                            string errorMessageFromShare = string.Empty;
                            objCommon.ShareRecordsMDLoanOfficerExternalId(ref errorMessageFromShare, objEntity.ToEntityReference(), leadStaging.GetAttributeValue<string>(Lead.MDLOExternalId), service, dcConfigDetails);
                            if (!string.IsNullOrEmpty(errorMessageFromShare))
                                errorMessage += errorMessageFromShare;
                            else
                                errorMessage += "Info:MD Lead is Shared to MD EEID \n";
                        }

                        if (generalNoteInfo != string.Empty)
                            objCommon.CreateNote(service, objEntity, generalNoteInfo);
                        if (!string.IsNullOrEmpty(contactGroup))
                            objCommon.MarketingListAssociation(dcConfigDetails, contactGroup, objEntity, service);

                        //Associate Zillow Sellers Agent Contact to Lead
                        if (importProcessName.Equals(Constants.ZillowLeadSource, StringComparison.OrdinalIgnoreCase) && entityName == Lead.EntityName)
                        {
                            if (zillowSellersAgentContactId != Guid.Empty)
                            {
                                if (!objCommon.CheckAsscocationExists(service, zillowSellersAgentContactId, objEntity.Id))
                                {
                                    EntityReference contactReference = new EntityReference(Contact.EntityName, zillowSellersAgentContactId);
                                    EntityReference leadReference = new EntityReference(Lead.EntityName, objEntity.Id);
                                    objCommon.Asscociate(service, contactReference, leadReference);
                                }
                            }
                        }


                        //Create Realtor(Contact) from BoomTown and Associate to Lead

                        if (importProcessName.Equals(Constants.BoomTown, StringComparison.OrdinalIgnoreCase) && entityName == Lead.EntityName)
                        {
                            Entity contactEntity = new Entity(Contact.EntityName);
                            if (leadStaging.Contains(LeadStaging.SpouseFirstName))
                            {
                                contactEntity[Contact.FirstName] = leadStaging.GetAttributeValue<string>(LeadStaging.SpouseFirstName);
                            }
                            if (leadStaging.Contains(LeadStaging.SpousePhoneCell))
                            {
                                contactEntity[Contact.MobilePhone] = leadStaging.GetAttributeValue<string>(LeadStaging.SpousePhoneCell);
                            }

                            if (contactEntity.Attributes.Count > 0)
                            {
                                contactEntity[Contact.ContactCategory] = new OptionSetValue(Convert.ToInt32(Contact.ContactCategory_OptionSet.Realtor));
                                if (objEntity.Contains(Contact.LoanOfficer))
                                    contactEntity[Contact.LoanOfficer] = objEntity.GetAttributeValue<EntityReference>(Contact.LoanOfficer);
                                var contacctGuid = service.Create(contactEntity);
                                if (!objCommon.CheckAsscocationExists(service, contacctGuid, objEntity.Id))
                                {
                                    EntityReference contactReference = new EntityReference(Contact.EntityName, contacctGuid);
                                    EntityReference leadReference = new EntityReference(Lead.EntityName, objEntity.Id);
                                    objCommon.Asscociate(service, contactReference, leadReference);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        objCommon.UpdateValidationMessage("Error while creating " + entityName + " record " + ex.Message, ref errorMessage);
                        validationStatus = false;
                        canReturn = true;
                    }
                }
                if (canReturn)
                {
                    objCommon.UpdateStagingLog(leadStaging.Id, Guid.Empty, LeadStaging.EntityName, errorMessage, validationStatus, service);
                    return;
                }
                if ((existingEntity &&objExistingEntity.Contains(Lead.FromBlend) && objExistingEntity.GetAttributeValue<bool>(Lead.FromBlend)) || (objEntity.GetAttributeValue<bool>(Lead.FromBlend) && objEntity.Contains(Lead.FromBlend)))
                {
                    if (leadStaging.Contains(LeadStaging.LeadStatus))
                    {
                        var leadStatus = leadStaging.GetAttributeValue<string>(LeadStaging.LeadStatus);
                        QueryExpression qryExpression = new QueryExpression(LoanStatusMapping.EntityName);
                        qryExpression.ColumnSet = new ColumnSet(true);
                        qryExpression.Criteria.AddCondition(new ConditionExpression(LoanStatusMapping.PrimaryName, ConditionOperator.Equal, leadStatus));
                        EntityCollection ec = service.RetrieveMultiple(qryExpression);
                        if (ec.Entities.Count > 0)
                        {
                            var leadStatusFromMapping = ec.Entities.FirstOrDefault().GetAttributeValue<EntityReference>(LoanStatusMapping.LeadStatus);
                            Entity leadEntity = new Entity(Lead.EntityName);
                            if (existingEntity)
                                leadEntity.Id = objExistingEntity.Id;
                            else
                                leadEntity.Id = objEntity.Id;
                            leadEntity[Lead.LeadStatus] = leadStatusFromMapping;
                            service.Update(leadEntity);
                        }
                    }
                }
                if (importProcessName.Equals(Constants.BlendMovehome))
                    CreateBlendApplicationAsTask(service, leadStaging, Constants.BlendApplicationAsTask, ref defaultTeam, existingEntity, entityId, ref errorMessage);
                if (entityId != Guid.Empty)
                    objCommon.UpdateStagingLog(leadStaging.Id, entityId, LeadStaging.EntityName, errorMessage, validationStatus, service, entityName);
            }

            /// <summary>
            /// Get the CRM User Guid Based on the Email
            /// </summary>
            /// <param name="attrValue">config Value</param>
            /// <param name="service">Crm Service</param>
            public void GetCrmUserGuid(ref string attrValue, IOrganizationService service)
            {
                QueryByAttribute byAttribute = new QueryByAttribute(SystemUser.EntityName);
                byAttribute.ColumnSet = new ColumnSet(SystemUser.DomainName);
                byAttribute.AddAttributeValue(SystemUser.InternalEmail, attrValue);
                EntityCollection entityCollection = service.RetrieveMultiple(byAttribute);
                if (entityCollection.Entities.Count > 0)
                {
                    foreach (Entity en in entityCollection.Entities)
                    {
                        attrValue = en.Id.ToString();
                    }
                }
            }

            public string CheckExistingContactRecord(Entity postImage, IOrganizationService service)
            {
                //spouse info
                string MobilePhone = string.Empty;
                string email = string.Empty;
                string firstname = string.Empty;
                string Lastname = string.Empty;
                if (postImage.Contains(LeadStaging.SpouseFirstName) && postImage.Contains(LeadStaging.SpouseLastName))
                {
                    firstname = postImage.GetAttributeValue<string>(LeadStaging.SpouseFirstName);
                    Lastname = postImage.GetAttributeValue<string>(LeadStaging.SpouseLastName);

                    QueryExpression queryEntity = new QueryExpression(Account.EntityName);
                    queryEntity.ColumnSet = new ColumnSet(Account.PrimaryName, Account.FirstName, Account.LastName, Account.Email, Account.MainPhone);
                    queryEntity.AddOrder(Account.FirstName, OrderType.Descending);
                    FilterExpression Filter1 = new FilterExpression(LogicalOperator.And);
                    Filter1.AddCondition(Account.FirstName, ConditionOperator.Equal, firstname);
                    Filter1.AddCondition(Account.LastName, ConditionOperator.Equal, Lastname);

                    FilterExpression Filter2 = new FilterExpression(LogicalOperator.Or);
                    if (postImage.Contains(LeadStaging.SpouseEmail))
                    {
                        Filter2.AddCondition(Account.Email, ConditionOperator.Equal, postImage.GetAttributeValue<string>(LeadStaging.SpouseEmail));
                    }
                    if (postImage.Contains(LeadStaging.SpousePhoneCell))
                    {
                        Filter2.AddCondition(Account.MainPhone, ConditionOperator.Equal, postImage.GetAttributeValue<string>(LeadStaging.SpousePhoneCell));
                    }
                    FilterExpression mainFilter = new FilterExpression(LogicalOperator.And);
                    mainFilter.AddFilter(Filter1);
                    if (Filter1.Conditions.Count > 0)
                        mainFilter.AddFilter(Filter2);
                    queryEntity.Criteria = mainFilter;
                    EntityCollection queryEntityRecords = service.RetrieveMultiple(queryEntity);
                    if (queryEntityRecords.Entities.Count > 0)
                    {
                        return queryEntityRecords.Entities.FirstOrDefault().Attributes[Account.PrimaryName].ToString();
                    }
                    else

                    {
                        string name = CreateContactRecord(postImage, service);
                        if (name != null)
                            return name;
                    }
                }
                return null;
            }

            public string CreateContactRecord(Entity postImage, IOrganizationService service)
            {
                Common objCommon = new Common();
                Entity entityAccount = new Entity(Account.EntityName);
                if (postImage.Contains(LeadStaging.SpouseFirstName))
                    entityAccount[Account.FirstName] = postImage.GetAttributeValue<string>(LeadStaging.SpouseFirstName);
                if (postImage.Contains(LeadStaging.SpouseLastName))
                    entityAccount[Account.LastName] = postImage.GetAttributeValue<string>(LeadStaging.SpouseLastName);
                if (postImage.Contains(LeadStaging.SpouseEmail))
                    entityAccount[Account.Email] = postImage.GetAttributeValue<string>(LeadStaging.SpouseEmail);
                if (postImage.Contains(LeadStaging.SpousePhoneCell))
                {
                    string phoneNumber = postImage.GetAttributeValue<string>(LeadStaging.SpousePhoneCell);
                    if (phoneNumber != null && (phoneNumber.IndexOf("-") == -1))
                    {
                        phoneNumber = objCommon.PhoneNumberFormatting(phoneNumber);
                        entityAccount[Account.MainPhone] = phoneNumber;
                        entityAccount[Account.unformattedMobilePhone] = postImage.GetAttributeValue<string>(LeadStaging.SpousePhoneCell);

                    }
                    else
                    {
                        phoneNumber = objCommon.PhoneNumberUnformatting(phoneNumber);
                        entityAccount[Account.unformattedMobilePhone] = phoneNumber;
                        entityAccount[Account.MainPhone] = postImage.GetAttributeValue<string>(LeadStaging.SpousePhoneCell);

                    }

                }
                if (postImage.Contains(LeadStaging.SpouseBirthday))
                {
                    DateTime dt;
                    string birthDate = postImage.GetAttributeValue<string>(LeadStaging.SpouseBirthday);
                    if (DateTime.TryParse(birthDate, out dt))
                    {
                        entityAccount[Account.BirthDate] = dt;
                    }
                }

                if (postImage.Contains(LeadStaging.SpousePhoneOffice))
                {
                    string phoneNumber = postImage.GetAttributeValue<string>(LeadStaging.SpousePhoneOffice);
                    if (phoneNumber != null && (phoneNumber.IndexOf("-") == -1))
                    {
                        phoneNumber = objCommon.PhoneNumberFormatting(phoneNumber);
                        entityAccount[Account.BusinessPhone] = phoneNumber;
                        entityAccount[Account.unformattedBusinessPhone] = postImage.GetAttributeValue<string>(LeadStaging.SpousePhoneOffice);
                    }
                    else
                    {
                        phoneNumber = objCommon.PhoneNumberUnformatting(phoneNumber);
                        entityAccount[Account.BusinessPhone] = postImage.GetAttributeValue<string>(LeadStaging.SpousePhoneOffice);
                        entityAccount[Account.unformattedBusinessPhone] = phoneNumber;

                    }

                }
                service.Create(entityAccount);
                if (entityAccount.Contains(Account.FirstName) && entityAccount.Contains(Account.LastName))
                    return string.Join(" ", entityAccount.GetAttributeValue<string>(Account.FirstName), entityAccount.GetAttributeValue<string>(Account.LastName));
                return null;
            }

            public void OneTimeDataMigrationCheck(IOrganizationService service, Entity staging, string entityName)
            {
                //if(staging.Contains(LeadStaging.ContactExternalId) && staging.Contains(LeadStaging.LOExternalID))
                //{
                //    QueryExpression queryExpression = new QueryExpression(entityName);
                //    queryExpression.ColumnSet = new ColumnSet(entityName + "id");
                //    queryExpression.Criteria.AddCondition()
                //}
            }

            public void ProcessFailedLoans(IOrganizationService service, Guid leadGuid)
            {
                var leadEntity = service.Retrieve(Lead.EntityName, leadGuid, new ColumnSet(Lead.LOExternalId, Lead.ExternalID));
                if (leadEntity != null && leadEntity.Contains(Lead.LOExternalId) && leadEntity.Contains(Lead.ExternalID))
                {
                    Common common = new Common();
                    string fetchXml = common.GetMessage(Constants.GetFailedLoans, dcConfigDetails);
                    string externalId = leadEntity.GetAttributeValue<string>(Lead.ExternalID);
                    string loExternalId = leadEntity.GetAttributeValue<string>(Lead.LOExternalId);
                    if (!string.IsNullOrEmpty(externalId) && !string.IsNullOrEmpty(loExternalId) && !string.IsNullOrEmpty(fetchXml))
                    {
                        if (fetchXml.Contains("{externalId}") && fetchXml.Contains("{loExternalId}"))
                        {
                            fetchXml = fetchXml.Replace("{externalId}", externalId);
                            fetchXml = fetchXml.Replace("{loExternalId}", loExternalId);
                            FetchExpression fetchExpression = new FetchExpression(fetchXml);
                            EntityCollection ec = service.RetrieveMultiple(fetchExpression);
                            if (ec.Entities.Count > 0)
                            {
                                var distinctLoanNumbersList = ec.Entities.GroupBy(loanExternalId => loanExternalId.Attributes["ims_loannumber"]).Select(DistinctLoan => DistinctLoan.First()).OrderByDescending(createdOn => createdOn["createdon"]);
                                if (distinctLoanNumbersList != null && distinctLoanNumbersList.Count() > 0)
                                {
                                    foreach (var loanStaging in distinctLoanNumbersList)
                                    {
                                        Entity loanStagingEntity = new Entity(LoanStaging.EntityName);
                                        loanStagingEntity.Id = loanStaging.Id;
                                        loanStagingEntity["ims_retry"] = true;
                                        extendedPluginContext.SharedVariables["RetryFromLeadStaging"] = "true";
                                        service.Update(loanStagingEntity);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            public Guid GetMovementDirectDefaultLeadSource(IOrganizationService service, string leadSource)
            {
                QueryExpression queryExpression = new QueryExpression(LeadStatus.EntityName);
                queryExpression.ColumnSet = new ColumnSet(true);
                queryExpression.Criteria.AddCondition(new ConditionExpression(LeadStatus.Scope, ConditionOperator.Equal, (int)LeadStatus.Scope_OptionSet.MDOnly));
                queryExpression.Criteria.AddCondition(new ConditionExpression(LeadStatus.PrimaryName, ConditionOperator.Equal, leadSource));

                EntityCollection entityCollection = service.RetrieveMultiple(queryExpression);
                if (entityCollection.Entities.Count > 0)
                {
                    return entityCollection.Entities.FirstOrDefault().Id;
                }
                return Guid.Empty;
            }

            public string FormatStateValue(IOrganizationService service, Entity leadStaging)
            {
                if (leadStaging.Contains(LeadStaging.LeadState))
                {
                    var stateName = GetValue(service, new ConditionExpression(State.Name, ConditionOperator.Equal, leadStaging.GetAttributeValue<string>(LeadStaging.LeadState)));
                    if (string.IsNullOrEmpty(stateName))
                    {
                        stateName = GetValue(service, new ConditionExpression(State.Code, ConditionOperator.Equal, leadStaging.GetAttributeValue<string>(LeadStaging.LeadState)));
                        if (!string.IsNullOrEmpty(stateName))
                        {
                            return stateName;
                        }
                    }
                    else if (!string.IsNullOrEmpty(stateName))
                    {
                        return stateName;
                    }
                }
                return null;
            }
            public string GetValue(IOrganizationService service, ConditionExpression condition)
            {
                QueryExpression expression = new QueryExpression(State.EntityName);
                expression.Criteria.AddCondition(condition);
                expression.ColumnSet = new ColumnSet(true);
                EntityCollection ec = service.RetrieveMultiple(expression);
                if (ec.Entities.Count > 0)
                {
                    if (ec.Entities.FirstOrDefault().Contains(State.Name))
                    {
                        return ec.Entities.FirstOrDefault().GetAttributeValue<string>(State.Name);
                    }
                }
                return null;
            }

            public void ShareActivities(EntityReference loanOfficer, IOrganizationService service, EntityReference entityReference)
            {
                GetActivities(loanOfficer, service, entityReference, Annotation.EntityName, Annotation.ObjectId);
                GetActivities(loanOfficer, service, entityReference, PhoneCall.EntityName, PhoneCall.Regarding);
                GetActivities(loanOfficer, service, entityReference, SMS.EntityName, SMS.RegardingObjectId);
            }
            public EntityCollection GetActivities(EntityReference loanOfficer, IOrganizationService service, EntityReference entityReference, string activityEntity, string regardingObj)
            {
                QueryExpression queryExpression = new QueryExpression(activityEntity);
                queryExpression.ColumnSet = new ColumnSet(true);
                queryExpression.Criteria.AddCondition(new ConditionExpression(regardingObj, ConditionOperator.Equal, entityReference.Id));
                EntityCollection ec = service.RetrieveMultiple(queryExpression);
                if (ec.Entities.Count > 0)
                {
                    Common objectCommon = new Common();
                    foreach (var en in ec.Entities)
                    {
                        objectCommon.shareRecordWithLOA(service, loanOfficer, en.ToEntityReference());
                    }
                }

                return null;
            }

            public void CreateBlendApplicationAsTask(IOrganizationService service, Entity leadStaging, string importProcessName, ref EntityReference defaultTeam, bool isLeadExists, Guid leadGuid, ref string errorMsg)
            {
                Common objCommon = new Common();
                Guid importMasterDataId = new Guid();
                mappings.Clear();
                if (importProcessName != string.Empty)
                {
                    //Get ImportDataMaster GUID & Default Team based on Import Process Name
                    importMasterDataId = Common.GetImportDataMasterAndDefaultTeam(importProcessName, ref defaultTeam, service);
                    if (defaultTeam != null)
                        objCommon.defaultTeamReference = defaultTeam;
                }
                if (!objCommon.FetchMappings(importMasterDataId, ref mappings, service, ref errorMessage))
                {
                    validationStatus = false;
                    objCommon.UpdateStagingLog(leadStaging.Id, Guid.Empty, LeadStaging.EntityName, errorMessage, validationStatus, service);
                    return;
                }
                Entity objEntityFormed = objCommon.FormTargetEntityObject(service, XRMExtensions.Task.EntityName, leadStaging, mappings, service, ref validationStatus, ref canReturn, ref errorMessage, dcConfigDetails);
                if (objEntityFormed != null)
                {
                    if (objEntityFormed.Contains(XRMExtensions.Task.BlendApplicationGuid) && leadGuid!=Guid.Empty)
                    {
                        bool taskStatus = false;

                        objEntityFormed[XRMExtensions.Task.RegardingObjectId] = new EntityReference(Lead.EntityName, leadGuid);
                        objEntityFormed[XRMExtensions.Task.Category] = new OptionSetValue(176390000);
                        
                        if (!objEntityFormed.Contains(XRMExtensions.Task.OwnerId) && defaultTeam != null)
                            objEntityFormed[XRMExtensions.Task.OwnerId] = defaultTeam;
                        //objEntityFormed[XRMExtensions.Task.OwnerId] = new OptionSetValue(176390000);
                        QueryExpression qryExpression = new QueryExpression(XRMExtensions.Task.EntityName);
                        qryExpression.ColumnSet = new ColumnSet(true);
                        qryExpression.Criteria.AddCondition(new ConditionExpression(XRMExtensions.Task.BlendApplicationGuid, ConditionOperator.Equal, objEntityFormed.GetAttributeValue<string>(XRMExtensions.Task.BlendApplicationGuid)));
                        qryExpression.Criteria.AddCondition(new ConditionExpression(XRMExtensions.Task.RegardingObjectId, ConditionOperator.Equal, leadGuid));
                        EntityCollection ec = service.RetrieveMultiple(qryExpression);
                        if (ec.Entities.Count > 0)
                        {
                            objEntityFormed.Id = ec.Entities.FirstOrDefault().Id;
                            if(ec.Entities.FirstOrDefault().Contains(XRMExtensions.Task.StateCode))
                            {
                                if(ec.Entities.FirstOrDefault().GetAttributeValue<OptionSetValue>(XRMExtensions.Task.StateCode).Value==1 || ec.Entities.FirstOrDefault().GetAttributeValue<OptionSetValue>(XRMExtensions.Task.StateCode).Value == 2)
                                {
                                    taskStatus= ActivateReactiateTaskRecord(service,ec.Entities.FirstOrDefault().ToEntityReference(), 0, 3);
                                    errorMessage += "Info:Application Task has been Updated to In-Progress\n";
                                    //Opena and Marking as In Progress
                                }
                            }
                            service.Update(objEntityFormed);
                            errorMessage += "Info:Application Task has been updated\n";
                            if(taskStatus)
                            {
                                ActivateReactiateTaskRecord(service, ec.Entities.FirstOrDefault().ToEntityReference(), 1, 5);
                            }
                        }
                        else
                        {
                            objEntityFormed[XRMExtensions.Task.Subject] = "Blend Application";
                            service.Create(objEntityFormed);
                            errorMessage += "Info:Application Task has been Created\n";
                        }
                    }
                }
            }

            public bool ActivateReactiateTaskRecord(IOrganizationService service,EntityReference task,int stateCode,int statusCode)
            {
                SetStateRequest setStateRequest = new SetStateRequest()
                {
                    EntityMoniker = new EntityReference
                    {
                        Id = task.Id,
                        LogicalName = task.LogicalName,
                    },
                    State = new OptionSetValue(stateCode),
                    Status = new OptionSetValue(statusCode)
                };
                service.Execute(setStateRequest);
                return true;
            }
        }
    }
}
