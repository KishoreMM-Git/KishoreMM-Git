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
    public class CreateConactionRecordePostCreate : BasePlugin
    {
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            Entity Target = null;
            Guid Leadid = Guid.Empty;
            Guid Contactid = Guid.Empty;
            string importDataMasterCoborrowerName = string.Empty;

            if (context == null)
            {
                throw new InvalidPluginExecutionException("Context not Found");
            }
            importDataMasterCoborrowerName = Constants.CoBorrower;

            Target = context.GetTargetEntity<Entity>();
            if (Target.Attributes.Contains(Lead.Co_Borrower))
            {
                if (Target[Lead.Co_Borrower] != null)
                {
                    Contactid = Target.GetAttributeValue<EntityReference>(Lead.Co_Borrower).Id;
                    if (Contactid != null)
                    {
                        Leadid = Target.Id;
                        SetconnectionRecorde(Target, context, Leadid, Contactid, importDataMasterCoborrowerName);
                    }
                }
            }
        }

        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "Update");
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Asynchronous, "Create");

        }
        public void SetconnectionRecorde(Entity entity, IExtendedPluginContext context, Guid id, Guid Contactid, string Cobrrower)
        {
            if (id != null)
            {
                Entity objLead = context.Retrieve(Lead.EntityName, id, new ColumnSet(Lead.ParentAccountforlead, Lead.Owner));
                if (objLead != null)
                {
                    if (objLead.Attributes.Contains(Lead.Owner))
                    {
                        if (objLead.Attributes.Contains(Lead.ParentAccountforlead))
                        {
                            Guid leadOwnerid = ((EntityReference)objLead.Attributes[Lead.Owner]).Id;
                            Guid ParentAccountforlead = ((EntityReference)objLead.Attributes[Lead.ParentAccountforlead]).Id;
                            if (ParentAccountforlead != Guid.Empty && leadOwnerid != Guid.Empty)
                            {
                                QueryExpression query = new QueryExpression(ConnectionRole.EntityName);
                                query.ColumnSet = new ColumnSet(true);
                                query.Criteria.AddCondition(new ConditionExpression(ConnectionRole.PrimaryName, ConditionOperator.Equal, Cobrrower));
                                EntityCollection results = context.RetrieveMultiple(query);
                                if (results != null && results.Entities.Count > 0)
                                {
                                    Guid Roleid = results.Entities[0].Id;
                                    Entity ConnectionEntity = new Entity(Connection.EntityName);
                                    ConnectionEntity.Attributes[Connection.Connectedfrom] = new EntityReference(Account.EntityName, ParentAccountforlead);//main
                                    ConnectionEntity.Attributes[Connection.Connectedto] = new EntityReference(Account.EntityName, Contactid);//cobrower
                                    ConnectionEntity.Attributes[Connection.RoleTo] = new EntityReference(ConnectionRole.EntityName, Roleid);
                                    ConnectionEntity.Attributes[Connection.Owner] = objLead.Attributes[Lead.Owner];
                                    context.Create(ConnectionEntity);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
