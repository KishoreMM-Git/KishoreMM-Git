using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XRMExtensions;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk.Messages;
using Xrm;


namespace Infy.MS.Plugins
{
    public class UpdateLastInteractiondateonLead : BasePlugin
    {
        Entity Target = null;
        Guid Recordid = Guid.Empty;
        public override void ExecutePlugin(IExtendedPluginContext context)
        {

            if (context == null)
            {
                throw new InvalidPluginExecutionException("Context not Found");
            }

            Target = context.GetTargetEntity<Entity>();

            Recordid = Target.Id;
            if (Target.LogicalName == Email.EntityName)
            {
                if (Recordid != null)
                {
                    ColumnSet attributes = new ColumnSet(true);
                    Entity entityObj = context.Retrieve(Email.EntityName, Recordid, attributes);
                    DateTime modifiedOn = DateTime.UtcNow;//entityObj.GetAttributeValue<DateTime>(Email.modifiedon).ToUniversalTime();
                    //Update Lead/Contact Last Interaction,Last Interaction Date On Creation of Email /Appointment
                    //  if (entityObj.GetAttributeValue<OptionSetValue>(Email.ActivityStatus).Value == 1)
                    // {
                    EntityCollection tocollection = entityObj.GetAttributeValue<EntityCollection>("to");
                    EntityCollection cccollection = entityObj.GetAttributeValue<EntityCollection>("cc");
                    EntityCollection bcccollection = entityObj.GetAttributeValue<EntityCollection>("bcc");
                    if (tocollection != null && tocollection.Entities.Count > 0)
                    {
                        context.Trace("the Tocollection is present");
                        getRecipientcollection(tocollection, modifiedOn, context, context);
                    }
                    if (cccollection != null && cccollection.Entities.Count > 0)
                    {
                        context.Trace("the cccollection is present");
                        getRecipientcollection(cccollection, modifiedOn, context, context);
                    }
                    if (bcccollection != null)
                    {
                        if (bcccollection.Entities.Count > 0)
                        {
                            context.Trace("the bcccollection is present");
                            getRecipientcollection(bcccollection, modifiedOn, context, context);
                        }
                    }
                }
                // }
            }
            if (Target.LogicalName == Appointment.EntityName)
            {
                ColumnSet attributes = new ColumnSet(true);
                Entity entityObj = context.Retrieve(Appointment.EntityName, Recordid, attributes);
                DateTime modifiedOn = DateTime.UtcNow;//entityObj.GetAttributeValue<DateTime>(Appointment.ModifiedOn).ToUniversalTime();
                //Update Lead/Contact Last Interaction,Last Interaction Date On Creation of Email /Appointment
                //if (entityObj.GetAttributeValue<OptionSetValue>(Appointment.Status).Value == 1)
                // {
                if (entityObj.Attributes.Contains(Appointment.Requiredattendees))
                {
                    EntityCollection attendeesCollection = entityObj.GetAttributeValue<EntityCollection>(Appointment.Requiredattendees);
                    if (attendeesCollection != null && attendeesCollection.Entities.Count > 0)
                    {
                        foreach (Entity attendee in attendeesCollection.Entities)
                        {
                            if (attendee != null)
                            {
                                EntityReference erfparty = (attendee.GetAttributeValue<EntityReference>(Appointment.partyid));
                                if (erfparty != null)
                                {
                                    Guid attendeeid = erfparty.Id;
                                    if (attendeeid != null)
                                    {
                                        if (erfparty.LogicalName == Lead.EntityName)
                                        {
                                            Entity lead = new Entity(Lead.EntityName);
                                            lead.Id = attendeeid;
                                            //lead[Lead.LastAppointmentDate] = modifiedOn;
                                            //lead[Lead.Lastinteractiondate] = modifiedOn;
                                            lead[Lead.InteractedwithLead] = new OptionSetValue((int)(Lead.LastInteraction_OptionSet.ScheduledAppointment));
                                            context.Update(lead);
                                        }
                                        else if (erfparty.LogicalName == Contact.EntityName)
                                        {
                                            Entity contact = new Entity(Contact.EntityName);
                                            contact.Id = attendeeid;
                                            contact[Contact.Lastinteractiondate] = modifiedOn;
                                            contact[Contact.InteractedwithContact] = new OptionSetValue((int)(Contact.LastInteraction_OptionSet.ScheduledAppointment));
                                            context.Update(contact);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                // }
            }
            if (Target.LogicalName == PhoneCall.EntityName)
            {
                context.Trace("Phonecall");
                ColumnSet attributes = new ColumnSet(true);
                Entity entityObj = context.Retrieve(PhoneCall.EntityName, Recordid, attributes);
                DateTime modifiedOn = entityObj.GetAttributeValue<DateTime>(PhoneCall.ModifiedOn).ToUniversalTime();

                //if (entityObj.GetAttributeValue<OptionSetValue>(PhoneCall.ActivityStatus).Value == 1)
                //{
                //    context.Trace("Completed");
                //    if (entityObj.Attributes.Contains(PhoneCall.Regarding))
                //    {
                //        if (regardingObjectId != null)
                //        {
                //            context.Trace("regardingObjectId : " + regardingObjectId);
                //            if (regardingObjEntityType == Lead.EntityName)
                //            {
                //                Entity lead = new Entity(Lead.EntityName);
                //                lead.Id = regardingObjectId;
                //                lead[Lead.LastCallDate] = modifiedOn;
                //                lead[Lead.Lastinteractiondate] = modifiedOn;
                //                lead[Lead.InteractedwithLead] = new OptionSetValue((int)(Lead.LastInteraction_OptionSet.SpokeTo));
                //                context.Update(lead);
                //            }
                //            else if (regardingObjEntityType == Contact.EntityName)
                //            {
                //                Entity contact = new Entity(Contact.EntityName);
                //                contact.Id = regardingObjectId;
                //                contact[Contact.Lastinteractiondate] = modifiedOn;
                //                contact[Contact.InteractedwithContact] = new OptionSetValue((int)(Contact.LastInteraction_OptionSet.SpokeTo));
                //                context.Update(contact);
                //            }
                //        }
                //    }
                //}

                if (context.MessageName == "Create" || context.MessageName == "Update")
                {
                    if (entityObj.Contains(PhoneCall.ReasonToCall))
                    {
                        //context.Trace(entityObj.GetAttributeValue<OptionSetValue>(PhoneCall.ReasonToCall).Value.ToString());
                        if (entityObj.Contains(PhoneCall.Regarding))
                        {
                            Guid regardingObjectId = (Guid)(entityObj.GetAttributeValue<EntityReference>(PhoneCall.Regarding)).Id;
                            string regardingObjEntityType = (entityObj.GetAttributeValue<EntityReference>(PhoneCall.Regarding)).LogicalName;
                            if (regardingObjectId != null && regardingObjEntityType != null)
                            {
                                if (regardingObjEntityType == Lead.EntityName)
                                {
                                    Entity lead = context.Retrieve(Lead.EntityName, regardingObjectId, new ColumnSet(Lead.EnableWarnings));// new Entity(Lead.EntityName);
                                    lead.Id = regardingObjectId;
                                    lead[Lead.EnableWarnings] = false;
                                    context.Update(lead);
                                    context.Trace("The Lead Record is Updated for PhoenCall Activity");
                                }
                                else if (regardingObjEntityType == Contact.EntityName)
                                {
                                    Entity contact = context.Retrieve(Contact.EntityName, regardingObjectId, new ColumnSet(Contact.EnableWarnings));//new Entity(Contact.EntityName);
                                    contact.Id = regardingObjectId;
                                    contact[Lead.EnableWarnings] = false;
                                    context.Update(contact);
                                    context.Trace("The Contact Record is Updated for PhoneCall ACtivity");
                                }
                            }
                        }
                    }
                }
            }
            if (Target.LogicalName == SMS.EntityName)
            {
                if (Recordid != null && Recordid != Guid.Empty)
                {
                    Entity entityObj = context.Retrieve(SMS.EntityName, Recordid, new ColumnSet(true));
                    if (entityObj.Contains(SMS.RegardingObjectId))
                    {
                        var regardingObject = entityObj.GetAttributeValue<EntityReference>(SMS.RegardingObjectId);
                        Guid regardingObjectId = entityObj.GetAttributeValue<EntityReference>(SMS.RegardingObjectId).Id;
                        string regardingObjectEntityName = entityObj.GetAttributeValue<EntityReference>(SMS.RegardingObjectId).LogicalName;
                        if ((entityObj.GetAttributeValue<bool>(SMS.direction) == true) && (entityObj.GetAttributeValue<bool>(SMS.isAutomatedSMS) == false))
                        {
                            //if (entityObj.GetAttributeValue<OptionSetValue>(SMS.ActivityStatus).Value == 1)
                            //{

                            DateTime modifiedOn = DateTime.UtcNow;//entityObj.GetAttributeValue<DateTime>(PhoneCall.ModifiedOn).ToUniversalTime();
                            if (regardingObjectId != null && regardingObjectEntityName != null)
                            {
                                if (regardingObjectEntityName == Lead.EntityName)
                                {
                                    Entity lead1 = context.Retrieve(regardingObjectEntityName, regardingObjectId, new ColumnSet(Lead.InteractedwithLead));
                                    if (lead1.Attributes.Contains(Lead.InteractedwithLead))
                                    {
                                        if (lead1.Attributes[Lead.InteractedwithLead] != new OptionSetValue(176390010)) //Send Text
                                        {
                                            Entity lead = new Entity(Lead.EntityName);
                                            lead.Id = regardingObjectId;
                                            lead[Lead.InteractedwithLead] = new OptionSetValue((int)(Lead.LastInteraction_OptionSet.SentText));
                                            context.Update(lead);
                                        }
                                    }
                                    else
                                    {
                                        Entity lead = new Entity(Lead.EntityName);
                                        lead.Id = regardingObjectId;
                                        lead[Lead.InteractedwithLead] = new OptionSetValue((int)(Lead.LastInteraction_OptionSet.SentText));
                                        context.Update(lead);
                                    }
                                }
                                else if (regardingObjectEntityName == Contact.EntityName)
                                {
                                    Entity Contact1 = context.Retrieve(regardingObjectEntityName, regardingObjectId, new ColumnSet(Contact.InteractedwithContact));
                                    if (Contact1.Attributes.Contains(Contact.InteractedwithContact))
                                    {
                                        if (Contact1.Attributes[Contact.InteractedwithContact] != new OptionSetValue(176390010)) //Send Text
                                        {
                                            Entity contact = new Entity(Contact.EntityName);
                                            contact.Id = regardingObjectId;
                                            contact[Contact.Lastinteractiondate] = modifiedOn;
                                            contact[Contact.InteractedwithContact] = new OptionSetValue((int)(Contact.LastInteraction_OptionSet.SentText));
                                            context.Update(contact);
                                        }
                                    }
                                    else
                                    {
                                        Entity contact = new Entity(Contact.EntityName);
                                        contact.Id = regardingObjectId;
                                        contact[Contact.Lastinteractiondate] = modifiedOn;
                                        contact[Contact.InteractedwithContact] = new OptionSetValue((int)(Contact.LastInteraction_OptionSet.SentText));
                                        context.Update(contact);
                                    }
                                }
                            }
                        }

                        if ((context.MessageName == "Create") || context.MessageName == "Update")
                        {
                            context.Trace("Reason to Text Update here");
                            if (entityObj.Contains(SMS.ReasonToText))
                            {
                                context.Trace(entityObj.GetAttributeValue<OptionSetValue>(SMS.ReasonToText).Value.ToString());
                                if (entityObj.Contains(SMS.RegardingObjectId))
                                {
                                    if (regardingObjectId != null && regardingObjectEntityName != null)
                                    {
                                        if (regardingObjectEntityName == Lead.EntityName)
                                        {
                                            Entity lead = new Entity(Lead.EntityName, regardingObjectId); //context.Retrieve(Lead.EntityName, regardingObjectId, new ColumnSet(Lead.EnableWarnings)); //new Entity(Lead.EntityName);
                                            //lead.Id = regardingObjectId;
                                            lead[Lead.EnableWarnings] = false;
                                            context.Update(lead);
                                            context.Trace("Lead Record is updated");
                                        }
                                        else if (regardingObjectEntityName == Contact.EntityName)
                                        {
                                            Entity contact = new Entity(Contact.EntityName, regardingObjectId);//context.Retrieve(Contact.EntityName, regardingObjectId, new ColumnSet(Contact.EnableWarnings)); //new Entity(Contact.EntityName);
                                            //contact.Id = regardingObjectId;
                                            contact[Contact.EnableWarnings] = false;
                                            context.Update(contact);
                                            context.Trace("Contact Record is updated");
                                        }
                                    }
                                }
                            }
                        }

                    }
                }

            }
        }
        public void updaterecord(Guid id, string entityname, DateTime modifiedOn, IOrganizationService service, IExtendedPluginContext context)
        {
            if (id != null)
            {
                if (entityname == Lead.EntityName)
                {
                    Entity lead = new Entity(entityname);
                    lead.Id = id;
                    //lead[Lead.LastPersonalEmailSentDate] = modifiedOn;
                    //lead[Lead.Lastinteractiondate] = modifiedOn;
                    lead[Lead.InteractedwithLead] = new OptionSetValue((int)(Lead.LastInteraction_OptionSet.SentEmail));
                    service.Update(lead);
                }
                else if (entityname == Contact.EntityName)
                {
                    Entity contact = new Entity(entityname);
                    contact.Id = id;
                    contact[Contact.Lastinteractiondate] = modifiedOn;
                    contact[Contact.InteractedwithContact] = new OptionSetValue((int)(Contact.LastInteraction_OptionSet.SentEmail));
                    service.Update(contact);
                }
            }
        }

        public void getRecipientcollection(EntityCollection bcccollection, DateTime modifiedOn, IOrganizationService service, IExtendedPluginContext context)
        {
            foreach (Entity obj in bcccollection.Entities)
            {
                if (obj != null)
                {
                    EntityReference erfparty = (obj.GetAttributeValue<EntityReference>(Email.partyid));
                    if (erfparty.LogicalName == Lead.EntityName || erfparty.LogicalName == Contact.EntityName)
                    {
                        string entityname = erfparty.LogicalName;
                        Guid recordid = erfparty.Id;
                        updaterecord(recordid, entityname, modifiedOn, service, context);
                    }
                }
            }
        }

        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "Create");
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "Update");
        }
    }
}
