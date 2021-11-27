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
    public class LeadStatusUpdate : BasePlugin
    {
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            if (context == null)
            {
                throw new InvalidPluginExecutionException("Context not found");
            }

            //if (context.Depth > 1) { return; }

            Entity lead = null;

            lead = context.GetTargetEntity<Entity>();

            if (context.MessageName.ToLower() != "update") { return; }

            if (context.PrimaryEntityName.ToLower() != Lead.EntityName) { return; }

            Entity postImage = context.GetFirstPostImage<Entity>();

            Guid leadStatusId = Guid.Empty;
            Guid customerId = Guid.Empty;
            if (postImage.Attributes.Contains(Lead.LeadStatus))
            {
                leadStatusId = ((EntityReference)postImage.Attributes[Lead.LeadStatus]).Id;
            }
            
            if (postImage.Attributes.Contains(Lead.ParentAccountforlead))
            {
                customerId = ((EntityReference)postImage.Attributes[Lead.ParentAccountforlead]).Id;
            }

            #region Query the Lead Status entity to get the corresponding Contact Type to update it on the customer

            QueryExpression query = new QueryExpression(LeadStatus.EntityName);
            ColumnSet cols = new ColumnSet(LeadStatus.ContactType);
            query.ColumnSet = cols;
            query.Criteria.AddCondition(new ConditionExpression(LeadStatus.PrimaryKey, ConditionOperator.Equal, leadStatusId));

            EntityCollection result = context.RetrieveMultiple(query);

            #endregion

            string contactTypeName = string.Empty;
            Guid contactTypeId = Guid.Empty;

            if (result != null && result.Entities.Count > 0)
            {
                foreach (Entity contactType in result.Entities)
                {
                    if (contactType.Attributes.Contains(LeadStatus.ContactType) && contactType.Attributes[LeadStatus.ContactType] != null)
                    {
                        contactTypeName = contactType.GetAttributeValue<EntityReference>(LeadStatus.ContactType).Name;
                        contactTypeId = contactType.GetAttributeValue<EntityReference>(LeadStatus.ContactType).Id;
                        if (customerId != Guid.Empty)
                        {
                            Entity contactToUpdate = new Entity(Account.EntityName);
                            contactToUpdate[Account.ContactType] = new EntityReference(ContactType.EntityName, contactTypeId);
                            contactToUpdate.Id = customerId;
                            context.Update(contactToUpdate);
                        }
                    }
                }
            }

            }

        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "Update");
        }
    }
}
