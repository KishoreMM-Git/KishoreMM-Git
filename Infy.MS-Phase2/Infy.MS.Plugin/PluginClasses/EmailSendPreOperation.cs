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

namespace Infy.MS.Plugins.PluginClasses
{
    public class EmailSendPreOperation : BasePlugin
    {
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            if (context == null)
            {
                throw new InvalidPluginExecutionException("Context not found");
            }

            if (context.MessageName.ToLower() != "send") { return; }

            if (context.PrimaryEntityName.ToLower() != Email.EntityName) { return; }

            Entity email = null;
            bool isValidToParty = true;
            if (context.InputParameters.Contains("EmailId"))
            {
                Guid emailId = (Guid)context.InputParameters["EmailId"];
                if (emailId != Guid.Empty)
                    email = context.SystemOrganizationService.Retrieve(Email.EntityName, emailId, new ColumnSet(Email.To));
            }

            if (email != null)
            {
                if (email.Attributes.Contains(Email.To) && email[Email.To] != null)
                {
                    EntityCollection to = email.GetAttributeValue<EntityCollection>(Email.To);
                    foreach (Entity toParty in to.Entities)
                    {
                        if (toParty.Attributes.Contains("partyid") && toParty["partyid"] != null)
                        {
                            EntityReference partyId = toParty.GetAttributeValue<EntityReference>("partyid");
                            isValidToParty = IsValidParty(context.SystemOrganizationService, partyId);
                            if (!isValidToParty) break;
                        }
                    }

                    if (!isValidToParty)
                    {
                        throw new InvalidPluginExecutionException("At least one recipient does not have an email address or is marked as 'Do Not Allow' email.");
                    }
                }

                if (email.Attributes.Contains(Email.Bcc) && email[Email.Bcc] != null)
                {
                    EntityCollection bcc = email.GetAttributeValue<EntityCollection>(Email.Bcc);
                    foreach (Entity toParty in bcc.Entities)
                    {
                        if (toParty.Attributes.Contains("partyid") && toParty["partyid"] != null)
                        {
                            EntityReference partyId = toParty.GetAttributeValue<EntityReference>("partyid");
                            isValidToParty = IsValidParty(context.SystemOrganizationService, partyId);
                            if (!isValidToParty) break;
                        }
                    }

                    if (!isValidToParty)
                    {
                        throw new InvalidPluginExecutionException("At least one recipient does not have an email address or is marked as 'Do Not Allow' email.");
                    }
                }
                if (email.Attributes.Contains(Email.Cc) && email[Email.Cc] != null)
                {
                    EntityCollection cc = email.GetAttributeValue<EntityCollection>(Email.Cc);
                    foreach (Entity toParty in cc.Entities)
                    {
                        if (toParty.Attributes.Contains("partyid") && toParty["partyid"] != null)
                        {
                            EntityReference partyId = toParty.GetAttributeValue<EntityReference>("partyid");
                            isValidToParty = IsValidParty(context.SystemOrganizationService, partyId);
                            if (!isValidToParty) break;
                        }
                    }

                    if (!isValidToParty)
                    {
                        throw new InvalidPluginExecutionException("At least one recipient does not have an email address or is marked as 'Do Not Allow' email.");
                    }
                }


            }
        }

        public bool IsValidParty(IOrganizationService service, EntityReference partyId)
        {
            bool isValidToParty = true;
            if (partyId != null && partyId.Id != Guid.Empty)
            {
                if (partyId.LogicalName != SystemUser.EntityName && partyId.LogicalName != "queue")
                {
                    Entity recipient = service.Retrieve(partyId.LogicalName, partyId.Id, new ColumnSet("emailaddress1", "emailaddress2", "donotemail"));
                    if (recipient != null && recipient.Id != null)
                    {
                        string emailAddress1 = string.Empty;
                        string emailAddress2 = string.Empty;
                        bool doNotEmail = false;
                        if (recipient.Attributes.Contains("emailaddress1") && recipient["emailaddress1"] != null)
                        {
                            emailAddress1 = recipient.GetAttributeValue<string>("emailaddress1");
                        }
                        if (recipient.Attributes.Contains("emailaddress2") && recipient["emailaddress2"] != null)
                        {
                            emailAddress2 = recipient.GetAttributeValue<string>("emailaddress2");
                        }
                        if (recipient.Attributes.Contains("donotemail") && recipient["donotemail"] != null)
                        {
                            doNotEmail = recipient.GetAttributeValue<bool>("donotemail");
                        }
                        //Recipient does not have an email address
                        if (string.IsNullOrEmpty(emailAddress1) && string.IsNullOrEmpty(emailAddress2))
                        {
                            isValidToParty = false;
                        }
                        //Recipient is marked as "Do Not Allow" email
                        if (doNotEmail)
                        {
                            isValidToParty = false;
                        }
                    }
                }
            }
            return isValidToParty;
        }
        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PreValidation, SdkMessageProcessingStepMode.Synchronous, "Send");
        }
    }
}
