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
    public class DuplicateDetectionOnLeadPreValidation : BasePlugin
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

            if (Target.Attributes.Contains(Lead.LastName) && Target.Attributes.Contains(Lead.FirstName))
            {
                Common objCommon = new Common();
                errormeesage = Common.FetchConfigDetails(Constants.errormessage, context);
                if (errormeesage != null)
                {
                    message = objCommon.checkerrorMessageforOtherContact(context, errormeesage);

                }
                dcConfigDetails = Common.FetchConfigDetails(Constants.AppConfigCustomer, context);
                if (dcConfigDetails != null)
                {

                    EntityCollection queryEntityRecords = checkDuplicateRecordesinLead(Target, context);
                    if (queryEntityRecords != null)
                    {
                        if (queryEntityRecords.Entities.Count > 0)
                        {
                            //throw new InvalidPluginExecutionException(message);
                        }
                    }
                }
            }
        }

        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PreValidation, SdkMessageProcessingStepMode.Synchronous, "Create");
        }

        public EntityCollection checkDuplicateRecordesinLead(Entity Target, IExtendedPluginContext context)
        {
            string MobilePhone = string.Empty;
            string email = string.Empty;
            string firstname = string.Empty;
            string Lastname = string.Empty;
            if (Target.Contains(Lead.FirstName) && Target.Contains(Lead.FirstName))
            {
                firstname = Target.GetAttributeValue<string>(Lead.FirstName);
                Lastname = Target.GetAttributeValue<string>(Lead.LastName);

                QueryExpression queryEntity = new QueryExpression(Lead.EntityName);
                queryEntity.ColumnSet = new ColumnSet(Lead.FirstName, Lead.LastName, Lead.PersonalEmail, Lead.MobilePhone);
                queryEntity.AddOrder(Lead.FirstName, OrderType.Descending);
                FilterExpression Filter1 = new FilterExpression(LogicalOperator.And);
                Filter1.AddCondition(Lead.FirstName, ConditionOperator.Equal, firstname);
                Filter1.AddCondition(Lead.LastName, ConditionOperator.Equal, Lastname);

                FilterExpression Filter2 = new FilterExpression(LogicalOperator.Or);
                if (Target.Contains(Lead.PersonalEmail))
                {
                    email = Target.GetAttributeValue<string>(Lead.PersonalEmail);
                    if(email != null)
                    Filter2.AddCondition(Lead.PersonalEmail, ConditionOperator.Equal, email);
                }
                if (Target.Contains(Lead.MobilePhone))
                {
                    MobilePhone = Target.GetAttributeValue<string>(Lead.MobilePhone);
                    if (MobilePhone != null)
                    Filter2.AddCondition(Lead.MobilePhone, ConditionOperator.Equal, MobilePhone);
                }
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
    }
}
