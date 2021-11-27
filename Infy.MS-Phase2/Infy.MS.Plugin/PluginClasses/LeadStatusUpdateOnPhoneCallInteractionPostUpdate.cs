using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk.Messages;

namespace Infy.MS.Plugins
{
    public class LeadStatusUpdateOnActivityCreation : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.  
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // The InputParameters collection contains all the data passed in the message request.  
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                if ((context.MessageName == "Update") || (context.MessageName == "Create"))
                {
                    //Get PhoneCall Details.
                    var phoneCall = (Entity)context.InputParameters["Target"];
                    tracingService.Trace("The Entity Name is" + phoneCall.LogicalName + phoneCall.Id);
                    tracingService.Trace(phoneCall.Attributes.Count.ToString());

                    Guid objId = Guid.Empty;
                    string objName = string.Empty;
                    string interactionTxt = string.Empty;
                    int interaction = 0;

                    IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                    IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                    Entity entity = service.Retrieve(phoneCall.LogicalName, phoneCall.Id, new ColumnSet(true)); //PhoneCall
                    if (entity.GetAttributeValue<EntityReference>("regardingobjectid") != null)
                    {
                        objId = entity.GetAttributeValue<EntityReference>("regardingobjectid").Id;  //Lead Id
                        objName = entity.GetAttributeValue<EntityReference>("regardingobjectid").LogicalName; //EntityName Lead or Contact
                    }
                    try
                    {
                        tracingService.Trace("Exceution Begin");
                        if (entity.Contains("ims_interactionresult"))
                        {
                            tracingService.Trace("The Record Contains Interactionresult");
                            interaction = entity.GetAttributeValue<OptionSetValue>("ims_interactionresult").Value;
                        }
                        if ((objId != null) && (objName != null))
                        {
                            //Retreive Related Lead/Contact from the PhoneCallEntity
                            tracingService.Trace("About to update the lead records" + "" + objName + "" + objId);
                            updateRecords(objId, objName, interaction, service, tracingService, entity);
                        }
                        else if ((objId == Guid.Empty) && (objName == string.Empty))
                        {
                            tracingService.Trace("Just create a phone Call here no need to update the lead");
                        }

                    }
                    catch (Exception ex)
                    {
                        tracingService.Trace("FollowUpPlugin: {0}", ex.ToString());

                    }
                }
            }
        }

        public void updateRecords(Guid objId, string objName, int interaction, IOrganizationService service, ITracingService tracingService, Entity phoneCall)
        {
            tracingService.Trace("The entity Name is " + "" + objName);
            if (objName == "contact")
            {
                Entity entity = service.Retrieve(objName, objId, new ColumnSet("ims_interactwithcontact", "ims_lastinteractiondate"));//lead or contact

                if (entity.Attributes.Contains("ims_interactwithcontact"))
                {
                    tracingService.Trace(interaction.ToString());

                    switch (interaction)
                    {
                        case phoneConstants.appointment:
                            ((OptionSetValue)entity["ims_interactwithcontact"]).Value = (176390002); //Scheduled Appointment
                            entity.Attributes["ims_lastinteractiondate"] = DateTime.Now;
                            break;
                        case phoneConstants.apponphone:
                            ((OptionSetValue)entity["ims_interactwithcontact"]).Value = (176390011); //Spoke To
                            entity.Attributes["ims_lastinteractiondate"] = DateTime.Now;
                            break;
                        case phoneConstants.financed:
                            ((OptionSetValue)entity["ims_interactwithcontact"]).Value = (176390011); //Spoke To
                            entity.Attributes["ims_lastinteractiondate"] = DateTime.Now;
                            break;
                        case phoneConstants.loanApp:
                            ((OptionSetValue)entity["ims_interactwithcontact"]).Value = (176390011); //Spoke To
                            entity.Attributes["ims_lastinteractiondate"] = DateTime.Now;
                            break;
                        case phoneConstants.noContact:
                            ((OptionSetValue)entity["ims_interactwithcontact"]).Value = (176390011); //Spoke To
                            entity.Attributes["ims_lastinteractiondate"] = DateTime.Now;
                            break;
                        case phoneConstants.notIntersted:
                            ((OptionSetValue)entity["ims_interactwithcontact"]).Value = (176390004); //Not Interested
                            entity.Attributes["ims_lastinteractiondate"] = DateTime.Now;
                            break;
                        case phoneConstants.notLooking:
                            ((OptionSetValue)entity["ims_interactwithcontact"]).Value = (176390011); //Spoke To
                            entity.Attributes["ims_lastinteractiondate"] = DateTime.Now;
                            break;
                        case phoneConstants.Property:
                            ((OptionSetValue)entity["ims_interactwithcontact"]).Value = (176390011); //Spoke To
                            entity.Attributes["ims_lastinteractiondate"] = DateTime.Now;
                            break;
                        case phoneConstants.sendMail:
                            ((OptionSetValue)entity["ims_interactwithcontact"]).Value = (176390012); //Sent Email
                            entity.Attributes["ims_lastinteractiondate"] = DateTime.Now;
                            break;
                        case phoneConstants.sendText:
                            ((OptionSetValue)entity["ims_interactwithcontact"]).Value = (176390010); //Sent Text
                            entity.Attributes["ims_lastinteractiondate"] = DateTime.Now;
                            break;
                        case phoneConstants.spokeTo:
                            ((OptionSetValue)entity["ims_interactwithcontact"]).Value = (176390011); //Spoke To
                            entity.Attributes["ims_lastinteractiondate"] = DateTime.Now;
                            break;
                        case phoneConstants.voicecall:
                            ((OptionSetValue)entity["ims_interactwithcontact"]).Value = 176390000;//Left VOiceMail
                            entity.Attributes["ims_lastinteractiondate"] = DateTime.Now;
                            break;
                        case phoneConstants.wrongNum:
                            ((OptionSetValue)entity["ims_interactwithcontact"]).Value = (176390001); //Wrong Number
                            entity.Attributes["ims_lastinteractiondate"] = DateTime.Now;
                            break;
                        default:
                            if (phoneCall.Attributes.Contains("scheduledstart"))
                            {
                                ((OptionSetValue)entity["ims_interactwithcontact"]).Value = (176390002);
                                entity.Attributes["ims_lastinteractiondate"] = DateTime.Now;
                            }
                            if (phoneCall.Attributes.Contains("actualstart")) //176390000
                            {
                                var statuscode = phoneCall.GetAttributeValue<OptionSetValue>("statuscode").Value;
                                if (statuscode != 176390000)
                                {
                                    tracingService.Trace(statuscode.ToString());
                                    entity.Attributes["ims_interactwithcontact"] = new OptionSetValue(176390011);
                                    entity.Attributes["ims_lastinteractiondate"] = DateTime.Now;
                                }
                            }
                            //if ((phoneCall.Attributes.Contains("directioncode")) && (phoneCall.Attributes.Contains("statuscode")))
                            //{
                            //    if((phoneCall.GetAttributeValue<bool>("directioncode") == false)&&(phoneCall.GetAttributeValue<OptionSetValue>("statuscode") == new OptionSetValue(176390000)))
                            //    {
                            //        entity["ims_interactwithcontact"] = null;
                            //        entity.Attributes["ims_lastinteractiondate"] = DateTime.Now;
                            //    }
                            //}
                            break;
                    }
                    service.Update(entity);
                    tracingService.Trace("The Record Updated succesfully" + objName);
                }
                else
                {
                    tracingService.Trace("The Contact doesn't contain the last interaction date value");
                    Entity ety = new Entity(objName, objId);
                    switch (interaction)
                    {
                        case phoneConstants.appointment:
                            ety.Attributes["ims_interactwithcontact"] = new OptionSetValue(176390002); //Scheduled Appointment
                            ety.Attributes["ims_lastinteractiondate"] = DateTime.Now;
                            break;
                        case phoneConstants.apponphone:
                            ety.Attributes["ims_interactwithcontact"] = new OptionSetValue(176390011); //Spoke To
                            ety.Attributes["ims_lastinteractiondate"] = DateTime.Now;
                            break;
                        case phoneConstants.financed:
                            ety.Attributes["ims_interactwithcontact"] = new OptionSetValue(176390011); //Spoke To
                            ety.Attributes["ims_lastinteractiondate"] = DateTime.Now;
                            break;
                        case phoneConstants.loanApp:
                            ety.Attributes["ims_interactwithcontact"] = new OptionSetValue(176390011); //Spoke 
                            ety.Attributes["ims_lastinteractiondate"] = DateTime.Now;
                            break;
                        case phoneConstants.noContact:
                            tracingService.Trace(phoneConstants.noContact.ToString());
                            ety.Attributes["ims_interactwithcontact"] = new OptionSetValue(176390011); //Spoke To
                            ety.Attributes["ims_lastinteractiondate"] = DateTime.Now;
                            break;
                        case phoneConstants.notIntersted:
                            ety.Attributes["ims_interactwithcontact"] = new OptionSetValue(176390004); //Not Interested.
                            ety.Attributes["ims_lastinteractiondate"] = DateTime.Now;
                            break;
                        case phoneConstants.notLooking:
                            ety.Attributes["ims_interactwithcontact"] = new OptionSetValue(176390011); //Spoke To
                            ety.Attributes["ims_lastinteractiondate"] = DateTime.Now;
                            break;
                        case phoneConstants.Property:
                            ety.Attributes["ims_interactwithcontact"] = new OptionSetValue(176390011); //Spoke To
                            ety.Attributes["ims_lastinteractiondate"] = DateTime.Now;
                            break;
                        case phoneConstants.sendMail:
                            ety.Attributes["ims_interactwithcontact"] = new OptionSetValue(176390012); //Sent Email
                            ety.Attributes["ims_lastinteractiondate"] = DateTime.Now;
                            break;
                        case phoneConstants.sendText:
                            ety.Attributes["ims_interactwithcontact"] = new OptionSetValue(176390010); //Send Text
                            ety.Attributes["ims_lastinteractiondate"] = DateTime.Now;
                            break;
                        case phoneConstants.spokeTo:
                            ety.Attributes["ims_interactwithcontact"] = new OptionSetValue(176390011); //Spoke To
                            ety.Attributes["ims_lastinteractiondate"] = DateTime.Now;
                            break;
                        case phoneConstants.voicecall:
                            ety.Attributes["ims_interactwithcontact"] = new OptionSetValue(176390000); //Left VoiceMail
                            ety.Attributes["ims_lastinteractiondate"] = DateTime.Now;
                            break;
                        case phoneConstants.wrongNum:
                            ety.Attributes["ims_interactwithcontact"] = new OptionSetValue(176390001); //Wrong Number
                            ety.Attributes["ims_lastinteractiondate"] = DateTime.Now;
                            break;
                        default:
                            if (phoneCall.Attributes.Contains("scheduledstart"))
                            {
                                ety.Attributes["ims_interactwithcontact"] = new OptionSetValue(176390002);
                                ety.Attributes["ims_lastinteractiondate"] = DateTime.Now;
                            }
                            if (phoneCall.Attributes.Contains("actualstart")) //176390000
                            {
                                var statuscode = phoneCall.GetAttributeValue<OptionSetValue>("statuscode").Value;
                                if (statuscode != 176390000)
                                {
                                    tracingService.Trace(statuscode.ToString());
                                    ety.Attributes["ims_interactwithcontact"] = new OptionSetValue(176390011);
                                    ety.Attributes["ims_lastinteractiondate"] = DateTime.Now;
                                }
                            }
                            //if ((phoneCall.Attributes.Contains("directioncode")) && (phoneCall.Attributes.Contains("statuscode")))
                            //{
                            //    if ((phoneCall.GetAttributeValue<bool>("directioncode") == false) && (phoneCall.GetAttributeValue<OptionSetValue>("statuscode") == new OptionSetValue(176390000)))
                            //    {
                            //        entity["ims_interactwithcontact"] = null;
                            //        entity.Attributes["ims_lastinteractiondate"] = DateTime.Now;
                            //    }
                            //}

                            break;
                    }
                    service.Update(ety);
                    tracingService.Trace("the Contact record without last interaction date updated successfully");
                }
            }
            if (objName == "lead")
            {
                Entity entity = service.Retrieve(objName, objId, new ColumnSet("ims_interactwithlead", "ims_lastcalldate", "ims_lastinteractiondate"));//lead or contact 
                tracingService.Trace("The Interaction Field Value from the PhoneCall Entity" + " " + interaction.ToString());
                if (entity.Attributes.Contains("ims_interactwithlead"))
                {
                    int optionSet = entity.GetAttributeValue<OptionSetValue>("ims_interactwithlead").Value;
                    string optionsetVal = entity.FormattedValues.ContainsKey("ims_interactwithlead") ? entity.FormattedValues["ims_interactwithlead"] : null;
                    tracingService.Trace("The Last Interaction Value in the lead Optionset is " + " " + optionSet);
                    switch (interaction)
                    {
                        case phoneConstants.appointment:
                            ((OptionSetValue)entity["ims_interactwithlead"]).Value = (176390002); //Scheduled Appointment
                            break;
                        case phoneConstants.apponphone:
                            ((OptionSetValue)entity["ims_interactwithlead"]).Value = (176390008); //Took App On Phone
                            break;
                        case phoneConstants.financed:
                            ((OptionSetValue)entity["ims_interactwithlead"]).Value = (176390005); //Already Found Financing.
                            break;
                        case phoneConstants.loanApp:
                            ((OptionSetValue)entity["ims_interactwithlead"]).Value = 176390011;
                            break;
                        case phoneConstants.noContact:
                            updateinteraction(optionsetVal, tracingService, entity, optionSet);
                            break;
                        case phoneConstants.notIntersted:
                            ((OptionSetValue)entity["ims_interactwithlead"]).Value = (176390004);//Not Interested.
                            break;
                        case phoneConstants.notLooking:
                            ((OptionSetValue)entity["ims_interactwithlead"]).Value = (176390006); //No Longer Looking
                            break;
                        case phoneConstants.Property:
                            ((OptionSetValue)entity["ims_interactwithlead"]).Value = (176390009); //Found Property
                            break;
                        case phoneConstants.sendMail:
                            ((OptionSetValue)entity["ims_interactwithlead"]).Value = (176390012); //Sent Email
                            break;
                        case phoneConstants.sendText:
                            ((OptionSetValue)entity["ims_interactwithlead"]).Value = (176390010); //Send Text
                            break;
                        case phoneConstants.spokeTo:
                            ((OptionSetValue)entity["ims_interactwithlead"]).Value = (176390011); //Spoke To
                            break;
                        case phoneConstants.voicecall:
                            ((OptionSetValue)entity["ims_interactwithlead"]).Value = (176390000); //Left VoiceMail
                            break;
                        case phoneConstants.wrongNum:
                            ((OptionSetValue)entity["ims_interactwithlead"]).Value = (176390003); //Wrong Number
                            break;
                        default:
                            if (phoneCall.Attributes.Contains("scheduledstart"))
                            {
                                ((OptionSetValue)entity["ims_interactwithlead"]).Value = (176390002);
                            }
                            if ((phoneCall.Attributes.Contains("actualstart"))) //&& (  != new OptionSetValue(176390000)
                            {
                                var statuscode = phoneCall.GetAttributeValue<OptionSetValue>("statuscode").Value;
                                tracingService.Trace(statuscode.ToString());
                                if (statuscode != 176390000)
                                {
                                    tracingService.Trace("the phone Call entity Contains ACtualStart");
                                    entity.Attributes["ims_interactwithlead"] = new OptionSetValue(176390011);
                                }
                            }
                            break;
                    }
                    service.Update(entity);
                    tracingService.Trace("the lead record with last interaction date updated successfully");
                }
                else
                {
                    tracingService.Trace("The lead doesn't contain the last interaction date value");
                    Entity ety = new Entity(objName, objId);
                    switch (interaction)
                    {
                        case phoneConstants.appointment:
                            ety.Attributes["ims_interactwithlead"] = new OptionSetValue(176390002); //Scheduled Appointment
                            break;
                        case phoneConstants.apponphone:
                            ety.Attributes["ims_interactwithlead"] = new OptionSetValue(176390008); //Took App On Phone
                            break;
                        case phoneConstants.financed:
                            ety.Attributes["ims_interactwithlead"] = new OptionSetValue(176390005); //Already Found Financing.
                            break;
                        case phoneConstants.loanApp:
                            ety.Attributes["ims_interactwithlead"] = new OptionSetValue(176390011);
                            break;
                        case phoneConstants.noContact:
                            ety.Attributes["ims_interactwithlead"] = new OptionSetValue(176390001);
                            tracingService.Trace(ety.Attributes["ims_interactwithlead"].ToString());
                            break;
                        case phoneConstants.notIntersted:
                            ety.Attributes["ims_interactwithlead"] = new OptionSetValue(176390004); //Not Interested.
                            break;
                        case phoneConstants.notLooking:
                            ety.Attributes["ims_interactwithlead"] = new OptionSetValue(176390006); //No Longer Looking
                            break;
                        case phoneConstants.Property:
                            ety.Attributes["ims_interactwithlead"] = new OptionSetValue(176390009); //Found Property
                            break;
                        case phoneConstants.sendMail:
                            ety.Attributes["ims_interactwithlead"] = new OptionSetValue(176390012); //Sent Email
                            break;
                        case phoneConstants.sendText:
                            ety.Attributes["ims_interactwithlead"] = new OptionSetValue(176390010); //Send Text
                            break;
                        case phoneConstants.spokeTo:
                            ety.Attributes["ims_interactwithlead"] = new OptionSetValue(176390011); //Spoke To
                            break;
                        case phoneConstants.voicecall:
                            ety.Attributes["ims_interactwithlead"] = new OptionSetValue(176390000); //Left VoiceMail
                            break;
                        case phoneConstants.wrongNum:
                            ety.Attributes["ims_interactwithlead"] = new OptionSetValue(176390003); //Wrong Number
                            break;
                        default:
                            if (phoneCall.Attributes.Contains("scheduledstart"))
                            {
                                tracingService.Trace("the Phone Call entity Contains ScheduleStart");
                                ety.Attributes["ims_interactwithlead"] = new OptionSetValue(176390002);
                            }
                            if ((phoneCall.Attributes.Contains("actualstart"))) //&& (  != new OptionSetValue(176390000)
                            {
                                var statuscode = phoneCall.GetAttributeValue<OptionSetValue>("statuscode").Value;
                                tracingService.Trace(statuscode.ToString());
                                if(statuscode != 176390000)
                                {
                                    tracingService.Trace("the phone Call entity Contains ACtualStart");
                                    ety.Attributes["ims_interactwithlead"] = new OptionSetValue(176390011);
                                } 
                            }
                            break;
                    }
                    service.Update(ety);
                    tracingService.Trace("the lead record without last interaction date updated successfully");
                }
                tracingService.Trace("The Record Updated succesfully" + objName);
            }

        }
        public void updateinteraction(string optionsetVal, ITracingService tracingService, Entity entity, int OptionSet)
        {
            if (optionsetVal == ContactAttptContansts.contactAttpt1)
            {
                tracingService.Trace("Set Value Contact Attempt 2");
                ((OptionSetValue)entity["ims_interactwithlead"]).Value = (176390014); //Contact Attempt 2
                tracingService.Trace(((OptionSetValue)entity["ims_interactwithlead"]).Value.ToString());
            }
            else if (optionsetVal == ContactAttptContansts.contactAttpt2)
            {
                tracingService.Trace("Set Value Contact Attempt 3");
                ((OptionSetValue)entity["ims_interactwithlead"]).Value = (176390015); //Contact Attempt 3
                tracingService.Trace(((OptionSetValue)entity["ims_interactwithlead"]).Value.ToString());
            }
            else if (optionsetVal == ContactAttptContansts.contactAttpt3)
            {
                tracingService.Trace("Set Value Contact Attempt 4");
                ((OptionSetValue)entity["ims_interactwithlead"]).Value = (176390016);//Contact Attempt 4
                tracingService.Trace(((OptionSetValue)entity["ims_interactwithlead"]).Value.ToString());
            }
            else if (optionsetVal == ContactAttptContansts.contactAttpt4)
            {
                tracingService.Trace("Set Value Contact Attempt 5");
                ((OptionSetValue)entity["ims_interactwithlead"]).Value = (176390017);//Contact Attempt 5
                tracingService.Trace(((OptionSetValue)entity["ims_interactwithlead"]).Value.ToString());
            }
            else if (optionsetVal == ContactAttptContansts.contactAttpt5)
            {
                tracingService.Trace("Set Value Contact Attempt 6");
                ((OptionSetValue)entity["ims_interactwithlead"]).Value = (176390018);//Contact Attempt 6
                tracingService.Trace(((OptionSetValue)entity["ims_interactwithlead"]).Value.ToString());
            }
            else if (optionsetVal == ContactAttptContansts.contactAttpt6)
            {
                tracingService.Trace("Set Value Contact Attempt 7");
                ((OptionSetValue)entity["ims_interactwithlead"]).Value = (176390019);//Contact Attempt 7
                tracingService.Trace(((OptionSetValue)entity["ims_interactwithlead"]).Value.ToString());
            }
            else if (optionsetVal == ContactAttptContansts.contactAttpt7)
            {
                tracingService.Trace("Set Value Contact Attempt 8");
                ((OptionSetValue)entity["ims_interactwithlead"]).Value = (176390020);//Contact Attempt 8
                tracingService.Trace(((OptionSetValue)entity["ims_interactwithlead"]).Value.ToString());
            }
            else if (optionsetVal == ContactAttptContansts.contactAttpt8)
            {
            }
            else
            {
                tracingService.Trace("Set Value Contact Attempt 1");
                ((OptionSetValue)entity["ims_interactwithlead"]).Value = (176390001);//Contact Attempt 1
                tracingService.Trace(((OptionSetValue)entity["ims_interactwithlead"]).Value.ToString());
            }
        }
    }
    public class phoneConstants
    {
        public const int voicecall = 176390000;     // "Left Voicemail"
        public const int spokeTo = 176390001;       //"Spoke To"
        public const int loanApp = 176390002;       // "Start Loan App"
        public const int appointment = 176390003;   //"Schedule Appointment
        public const int wrongNum = 176390004;      //" Wrong Number"
        public const int notIntersted = 176390005;  //"Not Interested"
        public const int notLooking = 176390006;    //" No Longer Looking"
        public const int financed = 176390007;      //"Already Found Financing"
        public const int apponphone = 176390008;    //"Took App on Phone"
        public const int Property = 176390009;      //"Found Property"
        public const int sendMail = 176390010;      //"Send Email"
        public const int sendText = 176390011;      //"Send Text"
        public const int noContact = 176390012;     //"No Contact/Contact Attempts"
    }
    public class ContactAttptContansts
    {
        public const string contactAttpt1 = "Contact Attempt 1"; // 176390001; //Contact Attempt 1 
        public const string contactAttpt2 = "Contact Attempt 2"; //176390014;  //Contact Attempt 2
        public const string contactAttpt3 = "Contact Attempt 3"; //176390015;  //Contact Attempt 3
        public const string contactAttpt4 = "Contact Attempt 4"; //176390016;//Contact Attempt 4
        public const string contactAttpt5 = "Contact Attempt 5"; //176390017;//Contact Attempt 5
        public const string contactAttpt6 = "Contact Attempt 6"; //176390018;//Contact Attempt 6
        public const string contactAttpt7 = "Contact Attempt 7"; //176390019;//Contact Attempt 7
        public const string contactAttpt8 = "Contact Attempt 8"; // 176390020;//Contact Attempt 8
    }
}
