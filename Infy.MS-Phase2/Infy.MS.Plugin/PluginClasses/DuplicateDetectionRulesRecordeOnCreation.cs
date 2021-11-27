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
    public class DuplicateDetectionRulesRecordeOnCreation : BasePlugin
    {
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            Entity Target = null;
            string errorMessage = string.Empty;
            string importDataMasterName = string.Empty;
            string importDataMasterCoborrowerName = string.Empty;
            Guid importMasterDataId = Guid.Empty;
            Common objectCommon = new Common();
            Guid ContactId = Guid.Empty;
            Dictionary<string, string> dcConfigDetails = new Dictionary<string, string>();
            Dictionary<string, string> errormeesage = new Dictionary<string, string>();
            Guid leadid = Guid.Empty;
            Guid Contactid = Guid.Empty;
            bool Iscoborrower = false;
            string personalmobile = string.Empty;
            string Email = string.Empty;
            List<Common.Mapping> mappings = new List<Common.Mapping>();
            if (context == null)
            {
                throw new InvalidPluginExecutionException("Context not Found");
            }
            if (context.MessageName.ToLower() != "create") { return; }
            Target = context.GetTargetEntity<Entity>();
            if (Target.Attributes.Contains(Account.LastName) && Target.Attributes.Contains(Account.FirstName))
            {

                string Lastname = Target.GetAttributeValue<string>(Account.LastName);
                string Firstname = Target.GetAttributeValue<string>(Account.FirstName);
                if (Target.Attributes.Contains(Account.Email))
                {
                    Email = Target.GetAttributeValue<string>(Account.Email);
                }
                if (Target.Attributes.Contains(Account.MainPhone))
                {
                    personalmobile = Target.GetAttributeValue<string>(Account.MainPhone);
                }
                Iscoborrower = Target.GetAttributeValue<bool>(Account.IsCoborrower);
                Common objCommon = new Common();
                if (Iscoborrower == false)
                {
                    return;
                }
                errormeesage = Common.FetchConfigDetails(Constants.errormessage, context);
                var message = objCommon.checkerrorMessageforCoBrrower(context, errormeesage);
                importDataMasterCoborrowerName = Constants.CoBorrower;
                dcConfigDetails = Common.FetchConfigDetails(Constants.AppConfigCustomer, context);
                if (dcConfigDetails.Count > 0)
                {
                    if (Target.Attributes.Contains(Account.OriginatingLead))
                    {
                        leadid = Target.GetAttributeValue<EntityReference>(Account.OriginatingLead).Id;
                        if (leadid != Guid.Empty)
                        {
                            // EntityCollection queryEntityRecords = objCommon.CheckExistingContactlist(context, dcConfigDetails, Target);

                            EntityCollection queryEntityRecords = checkDuplicateRecordesinAccount(Target, context);
                            if (queryEntityRecords != null)
                            {
                                if (queryEntityRecords.Entities.Count > 0)
                                {
                                    ContactId = queryEntityRecords.Entities[0].Id;
                                    Entity Leadentityobj = new Entity(Lead.EntityName);
                                    Leadentityobj.Id = leadid;
                                    Leadentityobj[Lead.Co_Borrower] = new EntityReference(Account.EntityName, ContactId);
                                    context.SystemOrganizationService.Update(Leadentityobj);
                                    throw new InvalidPluginExecutionException(message);
                                }                                
                                    
                            }
                           
                            QueryExpression getType = new QueryExpression(ContactType.EntityName);
                            getType.ColumnSet = new ColumnSet(true);
                            getType.Criteria.AddCondition(new ConditionExpression(ContactType.PrimaryName, ConditionOperator.Equal, importDataMasterCoborrowerName));
                            EntityCollection ContacttypeResults = context.RetrieveMultiple(getType);
                            if (ContacttypeResults != null)
                            {
                                context.Trace($"frist {ContacttypeResults}");
                                if (ContacttypeResults.Entities.Count > 0)
                                {
                                    context.Trace($"secound {ContacttypeResults.Entities.Count}");
                                    Guid Coborrowerid = ContacttypeResults.Entities[0].Id;
                                    Entity RetriveaLead = context.Retrieve(Lead.EntityName, leadid, new ColumnSet(true));
                                    context.Trace($"thrird {RetriveaLead}");
                                    if (RetriveaLead.Attributes.Contains(Lead.LeadSource))
                                    {
                                        if (RetriveaLead[Lead.LeadSource] != null)
                                        {
                                            Guid Leadsource = RetriveaLead.GetAttributeValue<EntityReference>(Lead.LeadSource).Id;
                                            context.Trace($"thrird {Leadsource}");
                                            Target[Account.ContactType] = new EntityReference(ContactType.EntityName, Coborrowerid);
                                            if (Leadsource != null)
                                            {
                                                Target[Account.ContactSource] = new EntityReference(LeadSource.EntityName, Leadsource);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PreValidation, SdkMessageProcessingStepMode.Synchronous, "Create");
        }

        public EntityCollection checkDuplicateRecordesinAccount(Entity Target, IExtendedPluginContext context)
        {
            string MobilePhone = string.Empty;
            string email = string.Empty;
            string firstname = string.Empty;
            string Lastname = string.Empty;
            if (Target.Contains(Account.FirstName) && Target.Contains(Account.FirstName))
            {
                firstname = Target.GetAttributeValue<string>(Account.FirstName);
                Lastname = Target.GetAttributeValue<string>(Account.LastName);

                QueryExpression queryEntity = new QueryExpression(Account.EntityName);
                queryEntity.ColumnSet = new ColumnSet(Account.FirstName, Account.LastName, Account.Email, Account.Mobilephone,Account.CreatedOn);
                queryEntity.AddOrder(Account.CreatedOn, OrderType.Descending);
                FilterExpression Filter1 = new FilterExpression(LogicalOperator.And);
                if (Target.Contains(Account.Email))
                {
                    email = Target.GetAttributeValue<string>(Account.Email);
                    if (email != null)
                    {
                        Filter1.AddCondition(Account.FirstName, ConditionOperator.Equal, firstname);
                        Filter1.AddCondition(Account.LastName, ConditionOperator.Equal, Lastname);
                        Filter1.AddCondition(Account.Email, ConditionOperator.Equal, email);

                    }
                }
                FilterExpression Filter2 = new FilterExpression(LogicalOperator.And);

                if (Target.Contains(Account.Mobilephone))
                {
                    MobilePhone = Target.GetAttributeValue<string>(Account.Mobilephone);
                    if (MobilePhone != null)
                    {
                        Filter2.AddCondition(Account.FirstName, ConditionOperator.Equal, firstname);
                        Filter2.AddCondition(Account.LastName, ConditionOperator.Equal, Lastname);
                        Filter2.AddCondition(Account.Mobilephone, ConditionOperator.Equal, MobilePhone);
                    }
                }
                FilterExpression mainFilter = new FilterExpression(LogicalOperator.Or);
                if (Filter1.Conditions.Count > 0)
                {
                    mainFilter.AddFilter(Filter1);
                }

                if (Filter2.Conditions.Count > 0)
                {
                    mainFilter.AddFilter(Filter2);
                }
                if (mainFilter.Filters.Count > 0)
                {
                    queryEntity.Criteria = mainFilter;
                    EntityCollection queryEntityRecords = context.RetrieveMultiple(queryEntity);
                    if (queryEntityRecords.Entities.Count > 0)
                    {
                        return queryEntityRecords;

                    }
                }
                
            }
            return null;
        }

    }

}




