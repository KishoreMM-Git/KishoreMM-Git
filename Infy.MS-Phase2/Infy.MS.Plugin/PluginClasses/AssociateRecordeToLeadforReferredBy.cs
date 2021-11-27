using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xrm;
using XRMExtensions;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;


namespace Infy.MS.Plugins
{
    public class AssociateRecordeToLeadforReferredBy : BasePlugin
    {
        Entity Target = null;
        Guid Referredby = Guid.Empty;
        Entity preImage = null;
        Common objComman = new Common();
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            if (context == null)
            {
                throw new InvalidPluginExecutionException("Context not Found");
            }
            if (context.MessageName.ToLower() == "update" || context.MessageName.ToLower() == "create")
            {
                Target = context.GetTargetEntity<Entity>();
                if (context.PreEntityImages.Contains("PreImage"))
                {
                    preImage = (Entity)context.PreEntityImages["PreImage"];
                }
                if (Target.Contains(Lead.ReferredBy))
                {
                    EntityReference Referredbyreferrence = Target.GetAttributeValue<EntityReference>(Lead.ReferredBy);
                    if (Target.GetAttributeValue<EntityReference>(Lead.ReferredBy) != null)
                    {
                        Referredby = (Guid)Target.GetAttributeValue<EntityReference>(Lead.ReferredBy).Id;
                    }
                    EntityReference leadRefernce = new EntityReference(Lead.EntityName,
                     Target.Id);

                    if (Referredby != null && Referredby != Guid.Empty)
                    {
                        if (!objComman.CheckAsscocationExists(context.OrganizationService, Referredby, Target.Id))
                            objComman.Asscociate(context.OrganizationService, Referredbyreferrence, leadRefernce);
                    }
                    if (preImage != null)
                    {
                        if (preImage.Contains(Lead.ReferredBy))
                        {
                            EntityReference Referredbyentityrefernce = preImage.GetAttributeValue<EntityReference>(Lead.ReferredBy);
                            if (Referredbyentityrefernce != null)
                            {
                                objComman.Disasscociate(context.OrganizationService, Referredbyentityrefernce, leadRefernce);
                            }
                        }
                    }
                }
            }

            #region  Update the Lead Status on Assign of Record From MD LO to Retail LO.
            if ((context.MessageName.ToLower() == "update") && (Target.Attributes.Contains(Lead.Owner)))
            {

                //get the old owner from the lead record
                context.Trace("The Plugin triggered on the update of lead record");
                Guid oldOwner = preImage.GetAttributeValue<EntityReference>(Lead.Owner).Id;
                string entityName = preImage.GetAttributeValue<EntityReference>(Lead.Owner).LogicalName;
                string leadStatusName = preImage.GetAttributeValue<EntityReference>(Lead.LeadStatus).Name;
                string statusEntity = preImage.GetAttributeValue<EntityReference>(Lead.LeadStatus).LogicalName;

                context.Trace(oldOwner.ToString() + " Old owner entity Name " + entityName);
                //fetch the   SystemUser/team for the oldUser
                Entity oldety = context.Retrieve(entityName, oldOwner, new ColumnSet("businessunitid"));
                string oldBuName = oldety.GetAttributeValue<EntityReference>("businessunitid").Name;

                //New Owner value from the Context
                Guid newOwner = Target.GetAttributeValue<EntityReference>(Lead.Owner).Id;
                string newEntity = Target.GetAttributeValue<EntityReference>(Lead.Owner).LogicalName;
                context.Trace(newOwner.ToString() + "New entity Name" + newEntity);


                //Fetch for SystemUser/Team here for the new user
                Entity entity1 = context.Retrieve(newEntity, newOwner, new ColumnSet("businessunitid"));
                string newbuName = entity1.GetAttributeValue<EntityReference>("businessunitid").Name;

                context.Trace("The new user Bussiness UNit is" + " " + newbuName);
                if (newbuName == "Retail")
                {
                    if (oldBuName == "Movement Direct")
                    {
                        Entity target = new Entity(Target.LogicalName, Target.Id);
                        target.Attributes["ims_interactwithlead"] = new OptionSetValue(176390022); //Assigned by Movement Direct
                        if (leadStatusName == "New" || leadStatusName == "AI Path" || leadStatusName == "Rate Watch")
                        {
                            target.Attributes[Lead.LeadStatus] = new EntityReference(statusEntity, new Guid("db0e70db-9807-ea11-a811-000d3a4f62e7")); //Lead Status --- Lead.
                        }
                        context.Update(target);
                        context.Trace("The Lead Record is updated succesfully");
                    }
                }
            }
            #endregion

            #region Updated  Last Interaction to RefferedBy in Contact on update/Create of reffered by field in Lead
            if (Referredby != null && Referredby != Guid.Empty)
            {
                context.Trace("Updated Contact Last interaction");
                //Entity ety = context.Retrieve(Target.LogicalName, Target.Id, new ColumnSet("ims_referredby"));
                //string etyName = Target.GetAttributeValue<EntityReference>(Lead.ReferredBy).LogicalName;

                Entity contact = new Entity(Contact.EntityName, Referredby);
                contact.Attributes[Contact.InteractedwithContact] = new OptionSetValue(176390013); // Referred Lead
                context.Update(contact);
                context.Trace("Updated Contact Last interaction sucessfully");
            }
            #endregion
        }

        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "Create");
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "Update");
        }

    }

}
