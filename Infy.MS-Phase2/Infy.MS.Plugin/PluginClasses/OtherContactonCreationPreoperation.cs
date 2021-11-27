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

namespace Infy.MS.Plugins.PluginClasses
{
    public class OtherContactonCreationPreoperation : BasePlugin
    {
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            Entity Target = null;
            string errorMessage = string.Empty;
            string importDataMasterName = string.Empty;
            string importDataMasterCoborrowerName = string.Empty;
            Guid importMasterDataId = Guid.Empty;
            Guid ContactId = Guid.Empty;
            List<Common.Mapping> mappings = new List<Common.Mapping>();
            Dictionary<string, string> errormeesage = new Dictionary<string, string>();
            Dictionary<string, string> dcConfigDetails = new Dictionary<string, string>();
            string RelatedConfig = string.Empty;
            string message = string.Empty;

            if (context == null)
            {
                throw new InvalidPluginExecutionException("Context not Found");
            }
            if (context.MessageName.ToLower() != "create") { return; }
            Target = context.GetTargetEntity<Entity>();

            if (Target.Attributes.Contains(Contact.LastName) && Target.Attributes.Contains(Contact.FirstName))
            {

                string Lastname = Target.GetAttributeValue<string>(Contact.LastName);
                string Firstname = Target.GetAttributeValue<string>(Contact.FirstName);
                if (Target.Attributes.Contains(Contact.PersonalEmail))
                {
                    string Email = Target.GetAttributeValue<string>(Contact.PersonalEmail);
                }
                if (Target.Attributes.Contains(Contact.MobilePhone))
                {
                    string personalmobile = Target.GetAttributeValue<string>(Contact.MobilePhone);
                }
                   Common objCommon = new Common();
                   errormeesage = Common.FetchConfigDetails(Constants.errormessage, context);
                    if (errormeesage != null)
                    {
                        message = objCommon.checkerrorMessageforOtherContact(context, errormeesage);

                    }
                    dcConfigDetails = Common.FetchConfigDetails(Constants.AppConfigCustomer, context);
                    if (dcConfigDetails != null)
                    {
                        
                        Entity queryEntityRecords = CheckPreviousOtherContact(Target, context);
                        if (queryEntityRecords != null)
                        {
                            //if (queryEntityRecords.Entities.Count > 0)
                            
                         //throw new InvalidPluginExecutionException(message);
                            
                        }
                    }
                }
            }
        
        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PreValidation, SdkMessageProcessingStepMode.Synchronous, "Create");
        }

        public EntityCollection checkDuplicateRecordesinOtherContact(Entity Target, IOrganizationService context)
        {
            string MobilePhone = string.Empty;
            string email = string.Empty;
            string firstname = string.Empty;
            string Lastname = string.Empty;
            if (Target.Contains(Contact.FirstName) && Target.Contains(Contact.LastName))
            {
                firstname = Target.GetAttributeValue<string>(Contact.FirstName);
                Lastname = Target.GetAttributeValue<string>(Contact.LastName);

                QueryExpression queryEntity = new QueryExpression(Contact.EntityName);
                queryEntity.ColumnSet = new ColumnSet(Contact.FirstName, Contact.LastName, Contact.PersonalEmail, Contact.MobilePhone);
                queryEntity.AddOrder(Contact.CreatedOn, OrderType.Descending);
                FilterExpression Filter1 = new FilterExpression(LogicalOperator.And);
                Filter1.AddCondition(Contact.FirstName, ConditionOperator.Equal, firstname);
                Filter1.AddCondition(Contact.LastName, ConditionOperator.Equal, Lastname);

                FilterExpression Filter2 = new FilterExpression(LogicalOperator.Or);
                if (Target.Contains(Contact.PersonalEmail))
                {
                    email = Target.GetAttributeValue<string>(Contact.PersonalEmail);
                    if (email != null)
                    {
                        Filter1.AddCondition(Contact.FirstName, ConditionOperator.Equal, firstname);
                        Filter1.AddCondition(Contact.LastName, ConditionOperator.Equal, Lastname);
                        Filter1.AddCondition(Contact.PersonalEmail, ConditionOperator.Equal, email);
                    }
                }
                if (Target.Contains(Contact.MobilePhone))
                {
                    MobilePhone = Target.GetAttributeValue<string>(Contact.MobilePhone);
                    if (MobilePhone != null)
                    {
                        Filter2.AddCondition(Contact.FirstName, ConditionOperator.Equal, firstname);
                        Filter2.AddCondition(Contact.LastName, ConditionOperator.Equal, Lastname);
                        Filter2.AddCondition(Contact.MobilePhone, ConditionOperator.Equal, MobilePhone);
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

        public Entity CheckPreviousOtherContact(Entity objEntity, IOrganizationService service)
        {
            Entity contact = new Entity();
            QueryExpression existingLeadQuery = new QueryExpression(Contact.EntityName);
            existingLeadQuery.ColumnSet = new ColumnSet(true);
            existingLeadQuery.Criteria.AddCondition(new ConditionExpression(Contact.Status, ConditionOperator.Equal, 0));
            if (objEntity.Attributes.Contains(Contact.ExternalID) && objEntity[Contact.ExternalID] != null)
            {
                ConditionExpression externalIdCondition = new ConditionExpression();
                externalIdCondition = new ConditionExpression(Contact.ExternalID, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Contact.ExternalID));
                existingLeadQuery.Criteria.AddCondition(externalIdCondition);
                if (GetRecordCount(existingLeadQuery, service, ref contact))
                {
                    return contact;
                }
            }
            if (objEntity.Attributes.Contains(Contact.FirstName) && objEntity.Contains(Contact.LastName) && objEntity.Contains(Contact.PersonalEmail))
            {
                //fe1: FN && LN && Email
                FilterExpression fe1 = new FilterExpression(LogicalOperator.And);
                fe1.Conditions.Add(new ConditionExpression(Contact.FirstName, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Contact.FirstName)));
                fe1.Conditions.Add(new ConditionExpression(Contact.LastName, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Contact.LastName)));
                fe1.Conditions.Add(new ConditionExpression(Contact.PersonalEmail, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Contact.PersonalEmail)));
                existingLeadQuery.Criteria.AddFilter(fe1);
                if (GetRecordCount(existingLeadQuery, service, ref contact))
                {
                    return contact;
                }
            }

            if (objEntity.Attributes.Contains(Contact.FirstName) && objEntity.Contains(Contact.LastName) && objEntity.Contains(Contact.MobilePhone))
            {
                //fe2: FN && LN && Mobile Phone
                FilterExpression fe2 = new FilterExpression(LogicalOperator.And);
                fe2.Conditions.Add(new ConditionExpression(Contact.FirstName, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Contact.FirstName)));
                fe2.Conditions.Add(new ConditionExpression(Contact.LastName, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Contact.LastName)));
                fe2.Conditions.Add(new ConditionExpression(Contact.MobilePhone, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Contact.MobilePhone)));
                existingLeadQuery.Criteria.AddFilter(fe2);
                if (GetRecordCount(existingLeadQuery, service, ref contact))
                {
                    return contact;
                }
            }
            return null;
        }


        public bool GetRecordCount(QueryExpression expression, IOrganizationService service, ref Entity lead)
        {
            EntityCollection ec = service.RetrieveMultiple(expression);
            if (ec.Entities.Count > 0)
            {
                lead = ec.Entities.FirstOrDefault();
                return true;
            }
            return false;
        }



    }
}
