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
    public class LeadStagingPreCreate : BasePlugin
    {

        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            try
            {
                if (context == null)
                {
                    throw new InvalidPluginExecutionException("Context not found");
                }
                var postImage = context.GetFirstPostImage<Entity>();
                string spouceRecord = CheckExistingContactRecord(postImage, context);

                AttributeCollection attColleciton = new AttributeCollection();
                attColleciton.Add(LeadStaging.CoBorrowerFullName, spouceRecord);
                context.SystemOrganizationService.Update(new Entity
                {
                    LogicalName = postImage.LogicalName,
                    Attributes = attColleciton,
                    Id = postImage.Id
                });

            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }


        /// <summary>
        /// Checking for the existing Account record with Spouse info from TE Lead Import
        /// </summary>
        public string CheckExistingContactRecord(Entity postImage,IExtendedPluginContext pluginContext)
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
                queryEntity.ColumnSet = new ColumnSet(Account.PrimaryName,Account.FirstName, Account.LastName, Account.Email, Account.MainPhone);
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
                EntityCollection queryEntityRecords = pluginContext.RetrieveMultiple(queryEntity);
                if (queryEntityRecords.Entities.Count > 1)
                {
                    return queryEntityRecords.Entities.FirstOrDefault().Attributes[Account.PrimaryName].ToString();
                }
                else

                {
                    string name=CreateContactRecord(postImage, pluginContext);
                    if (name != null)
                        return name;
                }
            }
            return null;
        }

        public string CreateContactRecord(Entity postImage, IExtendedPluginContext pluginContext)
        {
            Entity entityAccount = new Entity(Account.EntityName);
            if (postImage.Contains(LeadStaging.SpouseFirstName))
                entityAccount[Account.FirstName] = postImage.GetAttributeValue<string>(LeadStaging.SpouseFirstName);
            if (postImage.Contains(LeadStaging.SpouseLastName))
                entityAccount[Account.LastName] = postImage.GetAttributeValue<string>(LeadStaging.SpouseLastName);
            if (postImage.Contains(LeadStaging.SpouseEmail))
                entityAccount[Account.Email] = postImage.GetAttributeValue<string>(LeadStaging.SpouseEmail);
            if (postImage.Contains(LeadStaging.SpousePhoneCell))
                entityAccount[Account.MainPhone] = postImage.GetAttributeValue<string>(LeadStaging.SpousePhoneCell);
            if (postImage.Contains(LeadStaging.SpouseBirthday))
            {
                DateTime dt;
                string birthDate= postImage.GetAttributeValue<string>(LeadStaging.SpouseBirthday);
                if (DateTime.TryParse(birthDate, out dt))
                {
                    entityAccount[Account.BirthDate] = dt;
                }
            }
            if (postImage.Contains(LeadStaging.SpousePhoneOffice))
                entityAccount[Account.BusinessPhone] = postImage.GetAttributeValue<string>(LeadStaging.SpousePhoneOffice);
            pluginContext.SystemOrganizationService.Create(entityAccount);
            if (entityAccount.Contains(Account.FirstName) && entityAccount.Contains(Account.LastName))
                return string.Join(" ", entityAccount.GetAttributeValue<string>(Account.FirstName), entityAccount.GetAttributeValue<string>(Account.LastName));
            return null;
        }

        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "Create");
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "Update");
        }
    }
}
