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
using System.ServiceModel;
using System.Text.RegularExpressions;

namespace Infy.MS.Plugins
{
    public class ContactCreateUpdatePreOpertaion : BasePlugin
    {
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            Guid ownerId = Guid.Empty;
            Guid modifiedBy = Guid.Empty;


            if (context == null)
            {
                throw new InvalidPluginExecutionException("localContext");
            }

            if (context.PrimaryEntityName.ToLower() != Contact.EntityName) { return; }
            var contact = context.GetTargetEntity<Entity>();

            if (contact != null)
            {
                context.Trace("Target Found");
                modifiedBy = context.InitiatingUserId;
                //if (contact.Attributes.Contains(Contact.ModifiedBy) && contact[Contact.ModifiedBy] != null)
                //{
                //    modifiedBy = contact.GetAttributeValue<EntityReference>(Contact.ModifiedBy).Id;
                //}

                if (context.MessageName.ToLower() == "create")
                {
                    if (contact.Attributes.Contains(Contact.ContactType))
                    {
                        Guid contactTypeGuid = ((EntityReference)contact.Attributes[Contact.ContactType]).Id;
                        Common objCommon = new Common();
                        var optionset = objCommon.FetchLookupOptionSet(context, contactTypeGuid, ContactType.ContactTypeLookup, ContactType.EntityName);
                        if (optionset != null)
                        {
                            contact[Contact.ContactTypeLookup] = new OptionSetValue(optionset.Value);
                        }


                    }
                    if (contact.Attributes.Contains(Contact.LoanOfficer) && contact[Contact.LoanOfficer] != null)
                    {
                        context.Trace("Owner Present in Context");
                        ownerId = contact.GetAttributeValue<EntityReference>(Contact.LoanOfficer).Id;
                    }
                    //Formatted Phone Number Logic here
                    if (contact.Attributes.Contains(Contact.MobilePhone))
                    {
                        contact.Attributes[Contact.UnformattedMobilePhone] = formatPhoneNumber(contact.GetAttributeValue<string>(Lead.MobilePhone));
                    }
                    if (contact.Attributes.Contains("telephone1")) //Work Phone for Contact
                    {
                        contact.Attributes["ims_unformattedbusinessphone"] = formatPhoneNumber(contact.GetAttributeValue<string>("telephone1"));
                    }

                }
                else if (context.MessageName.ToLower() == "update")
                {
                    context.Trace("Message is Update");
                    if (contact.Attributes.Contains(Contact.LoanOfficer) && contact[Contact.LoanOfficer] != null)
                    {
                        context.Trace("Owner is present in Context");
                        ownerId = contact.GetAttributeValue<EntityReference>(Contact.LoanOfficer).Id;
                    }
                    else
                    {
                        var preImage = context.GetFirstPreImage<Entity>();
                        if (preImage.Attributes.Contains(Contact.LoanOfficer) && preImage[Contact.LoanOfficer] != null)
                        {
                            context.Trace("Owner is present in Preimage");
                            ownerId = preImage.GetAttributeValue<EntityReference>(Contact.LoanOfficer).Id;
                        }
                    }
                    //Formatted Phone Number Logic here
                    if (contact.Attributes.Contains(Contact.MobilePhone))
                    {
                        contact.Attributes[Contact.UnformattedMobilePhone] = formatPhoneNumber(contact.GetAttributeValue<string>(Lead.MobilePhone));
                    }
                    if (contact.Attributes.Contains("telephone1")) //Work Phone for Contact
                    {
                        contact.Attributes["ims_unformattedbusinessphone"] = formatPhoneNumber(contact.GetAttributeValue<string>("telephone1"));
                    }

                }
                var xmlModifiedBy = context.GetConfigValue<string>(Constants.ModifiedBy_RoleCheck, Constants.LeadContactPreOperationPlugin);
                var xmlOwner = context.GetConfigValue<string>(Constants.Owner_CheckRole, Constants.LeadContactPreOperationPlugin);
                //Checl Modify By  & UserRole
                if (modifiedBy != Guid.Empty && ownerId != Guid.Empty)
                {
                    context.Trace("Modified By And Owner found");
                    if (!string.IsNullOrEmpty(xmlModifiedBy) && !string.IsNullOrEmpty(xmlOwner))
                    {
                        context.Trace("FetchXML  configuration Found");
                        var isModifiedByValidRole = new Common().CheckMovementDirectBusinessUnit(xmlModifiedBy, modifiedBy, context.SystemOrganizationService);
                        if (isModifiedByValidRole)
                        {
                            context.Trace("Modified By Role Check Success");
                            var isOwnerValidRole = new Common().CheckMovementDirectBusinessUnit(xmlOwner, ownerId, context.SystemOrganizationService);
                            if (isOwnerValidRole)
                            {
                                context.Trace("Owner Role Check Success");
                                contact[Contact.ToSync] = true;
                            }
                        }
                    }
                }

            }
        }
        public string formatPhoneNumber(string number)
        {
            number = number.Replace("+1", "");
            number = number.Replace("+91", "");
            number = number.Replace("(", "");
            number = number.Replace(")", "");
            number = number.Replace("-", "");
            number = number.Replace(" ", "");
            return number;
        }

        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PreOperation, SdkMessageProcessingStepMode.Synchronous, "Create");
            yield return new Xrm.RegisteredEvent(PipelineStage.PreOperation, SdkMessageProcessingStepMode.Synchronous, "Update");
        }

    }
}
