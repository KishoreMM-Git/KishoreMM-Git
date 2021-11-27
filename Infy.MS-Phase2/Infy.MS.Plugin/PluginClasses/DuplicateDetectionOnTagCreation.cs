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


namespace Infy.MS.Plugins.PluginClasses
{
    public class DuplicateDetectionOnTagCreation : BasePlugin
    {
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
             
            string tagName = string.Empty;
            Guid userId = Guid.Empty;

            if (context == null)
            {
                throw new InvalidPluginExecutionException("Context not Found");
            }
            if (context.MessageName.ToLower() != "create" || context.MessageName.ToLower() != "update") { return; }
            
            try
            {
                Entity Target = context.GetTargetEntity<Entity>();
                if (Target.Attributes.Contains("ims_user"))
                {
                    userId = Target.GetAttributeValue<EntityReference>("ims_user").Id;

                    string adminfetchXml = "@<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
  "<entity name='team'>" +
    "<attribute name='name' />" +
    "<attribute name='businessunitid' />" +
    "<attribute name='teamid' />" +
    "<attribute name='teamtype' />" +
    "<order attribute='name' descending='false' />" +
    "<filter type='and'>" +
      "<condition attribute='teamtype' operator='eq' value='1' />" +
      "<condition attribute='administratorid' operator='eq' uitype='systemuser' value='{"+ userId +"}' />" +
    "</filter>" +
  "</entity>" +
"</fetch>";
                    string fetchXmlUserTeams = "@<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>" +
                      "<entity name='team'>" +
                        "<attribute name='name' />" +
                        "<attribute name='businessunitid' />" +
                        "<attribute name='teamid' />" +
                        "<attribute name='teamtype' />" +
                        "<order attribute='name' descending='false' />" +
                        "<filter type='and'>" +
                          "<condition attribute='teamtype' operator='eq' value='1' />" +
                        "</filter>" +
                        "<link-entity name='teammembership' from='teamid' to='teamid' visible='false' intersect='true'>" +
                          "<link-entity name='systemuser' from='systemuserid' to='systemuserid' alias='ac'>" +
                            "<filter type='and'>" +
                              "<condition attribute='systemuserid' operator='eq' uitype='systemuser' value='{" + userId + "}' />" +
                            "</filter>" +
                          "</link-entity>" +
                        "</link-entity>" +
                      "</entity>" +
                    "</fetch>";
                    EntityCollection adminTeamCollection = context.RetrieveMultiple(new FetchExpression(adminfetchXml));
                    EntityCollection userTeamCollection = context.RetrieveMultiple(new FetchExpression(fetchXmlUserTeams));
                    if (adminTeamCollection.Entities.Count > 0)
                        ShareTag(context, Target.Id, adminTeamCollection);
                    if (userTeamCollection.Entities.Count > 0)
                        ShareTag(context, Target.Id, userTeamCollection);
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public void ShareTag(IExtendedPluginContext context,Guid tagGuid, EntityCollection teamCollection)
        {
            foreach (var team in teamCollection.Entities)
            {
                GrantAccessRequest grantAccessRequest = new GrantAccessRequest()
                {
                    PrincipalAccess = new PrincipalAccess
                    {
                        AccessMask = AccessRights.ReadAccess, // here i am giving Read , write and Assign access to selected team
                        Principal = new EntityReference("team", team.Id)
                    },
                    Target = new EntityReference("ims_tags", tagGuid)
                };
            }
        }
        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Asynchronous, "Create");
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Asynchronous, "Update");
        }
    }
}