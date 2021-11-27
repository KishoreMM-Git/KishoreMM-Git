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
   public class LeadCreateUpdatePreOperation : BasePlugin 
    {
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            Guid ownerId = Guid.Empty;
            Guid modifiedBy = Guid.Empty;


            if (context == null)
            {
                throw new InvalidPluginExecutionException("localContext");
            }

            if (context.PrimaryEntityName.ToLower() != Lead.EntityName) { return; }
            var lead = context.GetTargetEntity<Entity>();

            if (lead != null)
            {
                context.Trace("Target Found");
                modifiedBy = context.InitiatingUserId;
                //if (lead.Attributes.Contains(Lead.ModifiedBy) && lead[Lead.ModifiedBy] != null)
                //{
                //    modifiedBy = lead.GetAttributeValue<EntityReference>(Lead.ModifiedBy).Id;
                //}

                if (context.MessageName.ToLower() == "create")
                {
                    context.Trace("Message is Create");
                    if (lead.Attributes.Contains(Lead.Owner) && lead[Lead.Owner] != null)
                    {
                        context.Trace("Owner Present in Context");
                        ownerId = lead.GetAttributeValue<EntityReference>(Lead.Owner).Id;
                    }
                }
                else if (context.MessageName.ToLower() == "update")
                {
                    context.Trace("Message is Update");
                    if (lead.Attributes.Contains(Lead.Owner) && lead[Lead.Owner] != null)
                    {
                        context.Trace("Owner is present in Context");
                        ownerId = lead.GetAttributeValue<EntityReference>(Lead.Owner).Id;
                    }
                    else
                    {
                        var preImage = context.GetFirstPreImage<Entity>();
                        if (preImage.Attributes.Contains(Lead.Owner) && preImage[Lead.Owner] != null)
                        {
                            context.Trace("Owner is present in Preimage");
                            ownerId = preImage.GetAttributeValue<EntityReference>(Lead.Owner).Id;
                        }
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
                                lead[Lead.ToSync] = true;
                            }
                        }
                    }
                }

            }
        }
        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PreOperation, SdkMessageProcessingStepMode.Synchronous, "Create");
            yield return new Xrm.RegisteredEvent(PipelineStage.PreOperation, SdkMessageProcessingStepMode.Synchronous, "Update");
        }

    }
}
