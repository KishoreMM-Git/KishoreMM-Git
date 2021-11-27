using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xrm;
using XRMExtensions;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk.Messages;

namespace Infy.MS.Plugins
{
    public class ContactDuplicateDetectionPostCreate : BasePlugin
    {
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            Entity Target = null;
            Entity Accountentity = null;
            string errorMessage = string.Empty;
            string importDataMasterName = string.Empty;
            string importDataMasterCoborrowerName = string.Empty;
            Dictionary<string, string> dcConfigDetails = new Dictionary<string, string>();
            Guid importMasterDataId = Guid.Empty;
            bool validationStatus = true;
            Guid AccountId = Guid.Empty;
            bool canReturn = false;
            string externalid= string.Empty;          
            string MarketingTeam = string.Empty;
            
            string fetchxmlTeam = string.Empty;
           

        // Dictionary<string, string> dcConfigDetailsdata = new Dictionary<string, string>();

            List<Common.Mapping> mappings = new List<Common.Mapping>();
            if (context == null)
            {
                throw new InvalidPluginExecutionException("Context not Found");
            }

            importDataMasterName = Constants.Contactfields;
            importDataMasterCoborrowerName = Constants.CoBorrower;
            MarketingTeam = Constants.GetteamName;

            if (importDataMasterName != string.Empty)
            {
                //Get ImportDataMaster GUID based on Import Master Data Name
                importMasterDataId = Common.GetImportDataMasterIdBasedOnName(importDataMasterName, context.SystemOrganizationService);
                dcConfigDetails = Common.FetchConfigDetails(Constants.AppConfigCustomer, context.SystemOrganizationService);
            }

            if (context.MessageName.ToLower() == "create")
            {
                Target = context.GetTargetEntity<Entity>();

                if(Target.Attributes.Contains(Lead.Owner))
                {
                    Entity lead = context.Retrieve(Lead.EntityName, Target.Id, new ColumnSet(Lead.LOExternalId));
                    if (lead.Contains(Lead.LOExternalId))
                    {
                        if (Lead.LOExternalId != null && Lead.LOExternalId != String.Empty)
                        {
                            externalid = lead.GetAttributeValue<String>(Lead.LOExternalId);
                        }
                   }
                }
                if (externalid != null && externalid != String.Empty)
                {
                    Target.Attributes[Lead.LOExternalId] = externalid;
                }
                mappingdata(Target, context, importMasterDataId, mappings, errorMessage, Accountentity, AccountId, validationStatus, canReturn, dcConfigDetails, MarketingTeam);
            }
            else if (context.MessageName.ToLower() == "update")
            {
                Target = context.GetTargetEntity<Entity>();
                Entity PostImage = (Entity)context.PostEntityImages["PostImage"];
                mappingdata(PostImage, context, importMasterDataId, mappings, errorMessage, Accountentity, AccountId, validationStatus, canReturn, dcConfigDetails, MarketingTeam);
            }
        }
        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "Create");
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "Update");
        }
        public EntityCollection checkDuplicateRecordesinLead(Entity Target, IExtendedPluginContext context)
        {
            string MobilePhone = string.Empty;
            string email = string.Empty;
            string firstname = string.Empty;
            string Lastname = string.Empty;
            if (Target.Contains(Lead.FirstName) && Target.Contains(Lead.LastName))
            {
                firstname = Target.GetAttributeValue<string>(Lead.FirstName);
                Lastname = Target.GetAttributeValue<string>(Lead.LastName);

                if (Target.Contains(Lead.PersonalEmail))
                {
                    email = Target.GetAttributeValue<string>(Lead.PersonalEmail);
                    //Filter2.AddCondition(Lead.PersonalEmail, ConditionOperator.Equal, email);
                }

                if (Target.Contains(Lead.MobilePhone))
                {
                    MobilePhone = Target.GetAttributeValue<string>(Lead.MobilePhone);
                    //Filter2.AddCondition(Lead.MobilePhone, ConditionOperator.Equal, MobilePhone);
                }


                QueryExpression queryEntity = new QueryExpression(Lead.EntityName);
                queryEntity.ColumnSet = new ColumnSet(Lead.FirstName, Lead.LastName, Lead.PersonalEmail, Lead.MobilePhone);
                queryEntity.AddOrder(Lead.FirstName, OrderType.Descending);
                FilterExpression Filter1 = new FilterExpression(LogicalOperator.And);
                Filter1.AddCondition(Lead.FirstName, ConditionOperator.Equal, firstname);
                Filter1.AddCondition(Lead.LastName, ConditionOperator.Equal, Lastname);

                FilterExpression Filter2 = new FilterExpression(LogicalOperator.Or);
                //if (Target.Contains(Lead.PersonalEmail))
                //{
                //    email = Target.GetAttributeValue<string>(Lead.PersonalEmail);
                //    Filter2.AddCondition(Lead.PersonalEmail, ConditionOperator.Equal, email);
                //}
                //if (Target.Contains(Lead.MobilePhone))
                //{
                //    MobilePhone = Target.GetAttributeValue<string>(Lead.MobilePhone);
                //    Filter2.AddCondition(Lead.MobilePhone, ConditionOperator.Equal, MobilePhone);
                //}
                FilterExpression mainFilter = new FilterExpression(LogicalOperator.And);
                mainFilter.AddFilter(Filter1);
                if (Filter2.Conditions.Count > 1)
                {
                    mainFilter.AddFilter(Filter2);
                }
                queryEntity.Criteria = mainFilter;
                EntityCollection queryEntityRecords = context.RetrieveMultiple(queryEntity);
                if (queryEntityRecords.Entities.Count > 0)
                {
                    return queryEntityRecords;

                }
            }
            return null;
        }
        public Entity checkDuplicateRecordinAccount(Entity objEntity, IExtendedPluginContext context, Entity PostImage,string MarketingTeam)
        {
            string MobilePhone = string.Empty;
            string email = string.Empty;
            string firstname = string.Empty;
            string Lastname = string.Empty;
            Entity CoborrowerAccount = new Entity();
            EntityCollection result = new EntityCollection();          
            Common objCommon = new Common();
            string FetchXml = string.Empty;
            Guid Teamid = Guid.Empty;
            Dictionary<string, string> defaultTeamReference = new Dictionary<string, string>();
            Dictionary<string, string> dcConfigDetails = new Dictionary<string, string>();
            dcConfigDetails = Common.FetchConfigDetails(Constants.AppConfigSetup, context.SystemOrganizationService);
            bool isLoanOfficer = objCommon.CheckIsLoanOfficer(context.SystemOrganizationService, context.UserId, dcConfigDetails);
            if (MarketingTeam != String.Empty)
            {
                defaultTeamReference = Common.FetchConfigDetails(MarketingTeam, context.SystemOrganizationService);
            }
            if (defaultTeamReference != null)
            {
                FetchXml = objCommon.Getid(context, defaultTeamReference);
                if (FetchXml != null)
                {
                    EntityCollection TeamEntity = context.RetrieveMultiple(new FetchExpression(@"" + FetchXml + ""));
                    if(TeamEntity.Entities.Count > 0 )
                    {
                        Teamid =(Guid)TeamEntity.Entities[0].Attributes[Team.teamid];                       
                    }
                }
            }

            if (context.MessageName == "Create")
            {
                
                //existingLeadQuery.Criteria.AddCondition(new ConditionExpression(Account.Status, ConditionOperator.Equal, 0));
                if (objEntity.Attributes.Contains(Account.ExternalID))
                {
                    QueryExpression existingLeadQuery = new QueryExpression(Account.EntityName);
                    existingLeadQuery.ColumnSet = new ColumnSet(true);
                    FilterExpression fe = new FilterExpression();
                    fe.Conditions.Add(new ConditionExpression(Account.ExternalID, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Account.ExternalID)));
                    existingLeadQuery.Criteria.AddFilter(fe);
                    if (objCommon.GetRecordCount(existingLeadQuery, context.OrganizationService, ref CoborrowerAccount))
                    {
                        if (CoborrowerAccount.Contains(Account.LOExternalId) && objEntity.Contains(Account.LOExternalId) && CoborrowerAccount.GetAttributeValue<string>(Account.LOExternalId).Equals(objEntity.GetAttributeValue<string>(Account.LOExternalId)))
                        {
                            return CoborrowerAccount;
                        }
                        else if (CoborrowerAccount.Contains(Account.LOExternalId) && objEntity.Contains(Account.LOExternalId))
                        {
                            return CoborrowerAccount;
                        }
                    }

                }
                if (objEntity.Attributes.Contains(Account.LOExternalId))
                {
                   
                    if (objEntity.Contains(Account.LOExternalId) && objEntity.Attributes.Contains(Account.FirstName) && objEntity.Contains(Account.LastName) && objEntity.Contains(Account.Email))
                    {
                        QueryExpression existingLeadQuery = new QueryExpression(Account.EntityName);
                        existingLeadQuery.ColumnSet = new ColumnSet(true);
                        //fe1: FN && LN && Email && LO External Id
                        FilterExpression fe1 = new FilterExpression(LogicalOperator.And);
                        fe1.Conditions.Add(new ConditionExpression(Account.FirstName, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Account.FirstName)));
                        fe1.Conditions.Add(new ConditionExpression(Account.LastName, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Account.LastName)));
                        fe1.Conditions.Add(new ConditionExpression(Account.Email, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Account.Email)));
                        fe1.Conditions.Add(new ConditionExpression(Account.LOExternalId, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Account.LOExternalId)));
                        //fe1.Conditions.Add(new ConditionExpression(Account.IsCoborrower, ConditionOperator.Equal, true));
                        existingLeadQuery.Criteria.AddFilter(fe1);
                        if (objCommon.GetRecordCount(existingLeadQuery, context.OrganizationService, ref CoborrowerAccount))
                        {
                            if (objEntity.Contains(Account.ExternalID) && !CoborrowerAccount.Contains(Account.ExternalID))
                            {
                                return CoborrowerAccount;
                            }
                        }
                    }

                    if (objEntity.Attributes.Contains(Account.LOExternalId) && objEntity.Attributes.Contains(Account.FirstName) && objEntity.Attributes.Contains(Account.LastName) && objEntity.Contains(Account.unformattedMobilePhone))
                    {
                        QueryExpression existingLeadQuery = new QueryExpression(Account.EntityName);
                        existingLeadQuery.ColumnSet = new ColumnSet(true);
                        //fe2: FN && LN && Mobile Phone  && LO External Id
                        FilterExpression fe2 = new FilterExpression(LogicalOperator.And);
                        fe2.Conditions.Add(new ConditionExpression(Account.FirstName, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Account.FirstName)));
                        fe2.Conditions.Add(new ConditionExpression(Account.LastName, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Account.LastName)));
                        fe2.Conditions.Add(new ConditionExpression(Account.unformattedMobilePhone, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Account.unformattedMobilePhone)));
                        fe2.Conditions.Add(new ConditionExpression(Account.LOExternalId, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Account.LOExternalId)));
                        existingLeadQuery.Criteria.AddFilter(fe2);
                        if (objCommon.GetRecordCount(existingLeadQuery, context.InitiatingUserOrganizationService, ref CoborrowerAccount))
                        {
                            if (objEntity.Contains(Account.ExternalID) && !CoborrowerAccount.Contains(Account.ExternalID))
                            {
                                return CoborrowerAccount;
                            }
                        }
                    }
                }

                if (objEntity.Attributes.Contains(Account.FirstName) && objEntity.Attributes.Contains(Account.LastName) && objEntity.Contains(Account.Email))
                {
                    if (objEntity.Attributes[Account.Email] != null)
                    {
                        QueryExpression existingLeadQuery = new QueryExpression(Account.EntityName);
                        existingLeadQuery.ColumnSet = new ColumnSet(true);
                        //fe1: FN && LN && Email && LO External Id
                        FilterExpression fe1 = new FilterExpression(LogicalOperator.And);
                        fe1.Conditions.Add(new ConditionExpression(Account.FirstName, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Account.FirstName)));
                        fe1.Conditions.Add(new ConditionExpression(Account.LastName, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Account.LastName)));
                        fe1.Conditions.Add(new ConditionExpression(Account.Email, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Account.Email)));
                        if (Teamid != null && !isLoanOfficer)
                        {
                            fe1.Conditions.Add(new ConditionExpression(Account.Owner, ConditionOperator.Equal, Teamid));
                        }
                        existingLeadQuery.Criteria.AddFilter(fe1);
                        if (objCommon.GetRecordCount(existingLeadQuery, context.InitiatingUserOrganizationService, ref CoborrowerAccount))
                        {
                            return CoborrowerAccount;
                        }
                    }
                }

                if (objEntity.Attributes.Contains(Account.FirstName) && objEntity.Attributes.Contains(Account.LastName) && objEntity.Attributes.Contains(Account.unformattedMobilePhone))
                {
                    QueryExpression existingLeadQuery = new QueryExpression(Account.EntityName);
                    existingLeadQuery.ColumnSet = new ColumnSet(true);
                    //fe2: FN && LN && Mobile Phone  && LO External Id
                    if (objEntity.Attributes[Account.unformattedMobilePhone] != null)
                    {
                        FilterExpression fe3 = new FilterExpression(LogicalOperator.And);
                        fe3.Conditions.Add(new ConditionExpression(Account.FirstName, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Account.FirstName)));
                        fe3.Conditions.Add(new ConditionExpression(Account.LastName, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Account.LastName)));
                        fe3.Conditions.Add(new ConditionExpression(Account.unformattedMobilePhone, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Account.unformattedMobilePhone)));
                        if (Teamid != null && !isLoanOfficer)
                        {
                            fe3.Conditions.Add(new ConditionExpression(Account.Owner, ConditionOperator.Equal, Teamid));
                        }
                        existingLeadQuery.Criteria.AddFilter(fe3);
                        if (objCommon.GetRecordCount(existingLeadQuery, context.InitiatingUserOrganizationService, ref CoborrowerAccount))
                        {
                            return CoborrowerAccount;
                        }
                    }
                }
                return null;
            }

            else if (context.MessageName == "Update")
            {
                if (PostImage.Contains(Account.ParentAccount))
                {
                    Guid ParentAccount = PostImage.GetAttributeValue<EntityReference>(Account.ParentAccount).Id;
                    if (ParentAccount != null)
                    {
                        QueryExpression Accountdata = new QueryExpression(Account.EntityName);
                        ColumnSet cols = new ColumnSet(Account.FirstName, Account.LastName, Account.Email, Account.Mobilephone, Account.CreatedOn);
                        Accountdata.ColumnSet = cols;
                        Accountdata.Criteria.AddCondition(new ConditionExpression(Account.PrimaryKey, ConditionOperator.Equal, ParentAccount));
                        EntityCollection results = context.RetrieveMultiple(Accountdata);
                        if (objCommon.GetRecordCount(Accountdata, context.SystemOrganizationService, ref CoborrowerAccount))
                        {
                            return CoborrowerAccount;
                        }

                    }
                }
            }
            return null;
        }
        public void mappingdata(Entity entity, IExtendedPluginContext context, Guid id, List<Common.Mapping> mappings, string errorMessage, Entity Accountentity, Guid AccountId, bool validationStatus, bool canReturn, Dictionary<string, string> dcConfigDetails,string Teamname)
        {
            Common Maping = new Common();
            if (Maping.FetchMappings(id, ref mappings, context.SystemOrganizationService, ref errorMessage))
            {
                Accountentity = new Entity(Account.EntityName);

                #region Updating the Contact Type of Customer to match Lead Status
                if (context.MessageName.ToLower() == "Create")
                {
                    if (entity.Attributes.Contains(Lead.LeadStatus) && entity.Attributes[Lead.LeadStatus] != null)
                    {
                        Guid leadStatusId = entity.GetAttributeValue<EntityReference>(Lead.LeadStatus).Id;

                        #region Query the Lead Status entity to get the Corresponsding Contact Type status to update it on customer

                        QueryExpression contactTypeQuery = new QueryExpression(LeadStatus.EntityName);
                        ColumnSet cols = new ColumnSet(LeadStatus.ContactType);
                        contactTypeQuery.ColumnSet = cols;
                        contactTypeQuery.Criteria.AddCondition(new ConditionExpression(LeadStatus.PrimaryKey, ConditionOperator.Equal, leadStatusId));

                        EntityCollection result = context.RetrieveMultiple(contactTypeQuery);


                        if (result.Entities.Count > 0)
                        {
                            foreach (Entity leadStatus in result.Entities)
                            {
                                if (leadStatus.Attributes.Contains(LeadStatus.ContactType) && leadStatus.Attributes[LeadStatus.ContactType] != null)
                                {
                                    Guid contactTypeId = leadStatus.GetAttributeValue<EntityReference>(LeadStatus.ContactType).Id;
                                    Accountentity[Account.ContactType] = new EntityReference(ContactType.EntityName, contactTypeId);
                                }
                            }
                        }

                        #endregion
                    }
                }

                #endregion

                foreach (Common.Mapping mapping in mappings)
                {
                    Common.Mapping objmapping = mapping;
                    if (entity.Attributes.Contains(mapping.Source) && entity[mapping.Source] != null)
                        Accountentity[mapping.Target] = entity[mapping.Source];
                }
                if (dcConfigDetails.Count > 0)
                {
                    bool existingEntity = false;
                    Guid entityId = Guid.Empty;
                    Entity objExistingEntity = checkDuplicateRecordinAccount(Accountentity, context, entity, Teamname);
                    if (objExistingEntity != null && objExistingEntity.Id != Guid.Empty)
                    {
                        existingEntity = true;
                        entityId = objExistingEntity.Id;
                    }
                    if (existingEntity)
                    {
                        AccountId = entityId;
                        Entity Retriveaccount = context.SystemOrganizationService.Retrieve(Account.EntityName, AccountId, new ColumnSet(true));
                        Maping.UpdateRecordIfDirty(Retriveaccount, Accountentity, Account.EntityName, mappings, context.SystemOrganizationService, ref validationStatus, ref canReturn, ref errorMessage);
                        UpdateLEad(entity, context, AccountId);
                    }
                    else
                    {
                        Accountentity.Attributes[Account.OriginatingLead] = new EntityReference(Lead.EntityName, entity.Id);
                        AccountId = context.Create(Accountentity);
                        UpdateLEad(entity, context, AccountId);
                    }

                }
            }
        }
        public void UpdateLEad(Entity entity, IExtendedPluginContext context, Guid AccountId)
          {
            if (entity.LogicalName == Lead.EntityName && entity.Id!=Guid.Empty)
            {
                Entity Leadentity = new Entity(Lead.EntityName);
                Leadentity.Id = entity.Id;
                Leadentity.Attributes[Lead.ParentAccountforlead] = new EntityReference(Account.EntityName, AccountId);
                if (AccountId != Guid.Empty)
                    context.SystemOrganizationService.Update(Leadentity);
            }
        }

    }
    
}

