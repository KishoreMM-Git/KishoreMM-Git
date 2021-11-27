using System;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xrm;
using XRMExtensions;
using Microsoft.Xrm.Sdk.Query;

namespace Infy.MS.Plugins
{
    public class ContactPostCreateUpdate : BasePlugin
    {
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            //Variable Declarations
            string webResourceName = string.Empty;
            Entity contactType = null;
            string content = string.Empty;
            Entity objContact = new Entity(Contact.EntityName);

            if (context == null)
            {
                throw new InvalidPluginExecutionException("Context not found");
            }
            if (context.Depth > 1) { return; }
            Entity contact = null;
            if (context.MessageName.ToLower() != "create" && context.MessageName.ToLower() != "update")
            {
                return;
            }
            contact = context.GetTargetEntity<Entity>();
            objContact.Id = contact.Id;
            if (context.PrimaryEntityName.ToLower() != Contact.EntityName) { return; }
            if (contact.Attributes.Contains(Contact.ContactGroups))
            {
                EntityReference erfContactGroups = contact.GetAttributeValue<EntityReference>(Contact.ContactGroups);
                if (erfContactGroups.Id != Guid.Empty)
                {
                    //Get Web Resource Name from Contact type record
                    contactType = context.Retrieve(ContactType.EntityName, erfContactGroups.Id, new ColumnSet(ContactType.WebResourceName));
                    if (contactType != null && contactType.Attributes.Contains(ContactType.WebResourceName))
                    {
                        webResourceName = contactType.GetAttributeValue<string>(ContactType.WebResourceName);
                        if (webResourceName != string.Empty)
                        {
                            //Get Web Resource content based on name
                            var query = new QueryExpression(WebResource.EntityName);
                            query.TopCount = 1;
                            query.ColumnSet.AddColumns(WebResource.PrimaryName, WebResource.Content);
                            query.Criteria.AddCondition(WebResource.PrimaryName, ConditionOperator.Equal, webResourceName.Trim());
                            var webResource = context.RetrieveMultiple(query);

                            if (webResource.Entities.Count > 0)
                            {
                                if (webResource.Entities[0].Attributes.Contains(WebResource.Content))
                                {
                                    content = webResource.Entities[0].GetAttributeValue<string>(WebResource.Content);
                                    //Update Contact record Image 
                                    //Entity objContact = new Entity(Contact.EntityName);
                                    objContact.Id = contact.Id;
                                    objContact[Contact.EntityImage] = Convert.FromBase64String(content);
                                    context.Update(objContact);
                                }
                            }
                        }
                    }
                }
            }
            if (contact.Attributes.Contains(Contact.ContactType))
            {
                Guid contactTypeGuid = ((EntityReference)contact.Attributes[Contact.ContactType]).Id;
                Common objCommon = new Common();
                objContact.Id = contact.Id;
                var optionset = objCommon.FetchLookupOptionSet(context, contactTypeGuid, ContactType.ContactTypeLookup, ContactType.EntityName);
                if (optionset != null)
                {
                    objContact[Contact.ContactTypeLookup] = new OptionSetValue(optionset.Value);
                    //objContact.Attributes.Add(Contact.ContactTypeLookup, optionset);
                    context.Update(objContact);
                }
                

            }
        }

        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Asynchronous, "Create");
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "Update");
        }
    }
}
