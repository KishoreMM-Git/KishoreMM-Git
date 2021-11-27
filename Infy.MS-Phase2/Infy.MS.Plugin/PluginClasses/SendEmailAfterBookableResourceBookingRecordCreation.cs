using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xrm;
using XRMExtensions;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Messages;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Net;
using System.IdentityModel.Tokens;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using AuthenticationContext = Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext;

namespace Infy.MS.Plugins
{
    public class SendEmailAfterBookableResourceBookingRecordCreation : BasePlugin
    {
        public IOrganizationService serviceProxy;
        public string accessToken;
        //public string batchJobLog;
        public string commonNumber;
        public string postingURL;
        public string countryCode;
        IExtendedPluginContext TraceContext;
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            if (context == null)
            {
                throw new InvalidPluginExecutionException("Context not found");
            }
            TraceContext = context;
            if (context.MessageName == "Create")
            {
                serviceProxy = context;
                Entity Target;
                Target = context.GetTargetEntity<Entity>();

                if (Target != null)
                {
                    if (Target.Contains("resource") && Target.Contains("ims_routingrule"))
                    {
                        Guid RoutingRuleId = (Guid)(Target.GetAttributeValue<EntityReference>("ims_routingrule")).Id;
                        Guid ResourceId = (Guid)(Target.GetAttributeValue<EntityReference>("resource")).Id;

                        ColumnSet userattributes = new ColumnSet(new string[] { "userid" });
                        Entity bResource = context.Retrieve(BookableResource.EntityName, ResourceId, userattributes);

                        if (RoutingRuleId != null && RoutingRuleId != Guid.Empty)
                        {
                            ColumnSet attributes = new ColumnSet(new string[] { "ims_triggernotification", "ims_triggersmsnotification" });
                            Entity entityObj = context.Retrieve("ims_routingrule", RoutingRuleId, attributes);
                            if (entityObj != null)
                            {
                                if (entityObj.GetAttributeValue<bool>("ims_triggernotification") == true)
                                {
                                    Entity Fromparty = new Entity(ActivityParty.EntityName);
                                    Entity Toparty = new Entity(ActivityParty.EntityName);
                                    Toparty[Email.partyid] = bResource.GetAttributeValue<EntityReference>("userid");//new EntityReference(SystemUser.EntityName, bResource.GetAttributeValue<EntityReference>("userid").Id);
                                    // Fromparty[Email.partyid] = new EntityReference(SystemUser.EntityName, new Guid("6dd1b8f9-c553-ea11-a814-000d3a33fcaa"));

                                    QueryExpression query = new QueryExpression();
                                    query.EntityName = "systemuser";
                                    query.ColumnSet = new ColumnSet(new string[] { "firstname", "lastname" });
                                    query.Criteria = new FilterExpression();
                                    query.Criteria.AddCondition(new ConditionExpression("internalemailaddress", ConditionOperator.Equal, "Svc-MSDCRM-Test@movement.com"));
                                    query.Criteria.AddCondition(new ConditionExpression("isdisabled", ConditionOperator.Equal, false));
                                    RetrieveMultipleRequest request = new RetrieveMultipleRequest();
                                    request.Query = query;
                                    EntityCollection results = ((RetrieveMultipleResponse)serviceProxy.Execute(request)).EntityCollection;
                                    if (results.Entities.Count == 0)
                                    {
                                        throw new InvalidPluginExecutionException("Service Account with mail ID : Svc-MSDCRM-Test@movement.com not found..");
                                    }
                                    Fromparty[Email.partyid] = new EntityReference(SystemUser.EntityName, results.Entities[0].Id);

                                    ColumnSet leadattributes = new ColumnSet(new string[] { "firstname", "lastname", "mobilephone" });
                                    Entity leadDetails = context.Retrieve(Lead.EntityName, Target.GetAttributeValue<EntityReference>("ims_lead").Id, leadattributes);
                              
                                    Entity email = new Entity(Email.EntityName);
                                    email[Email.From] = new Entity[] { Fromparty };
                                    email[Email.To] = new Entity[] { Toparty };
                                    email[Email.RegardingObjectId] = Target.GetAttributeValue<EntityReference>("ims_lead");//new EntityReference(Lead.EntityName, Target.GetAttributeValue<EntityReference>("resource").Id);
                                    email[Email.Subject] = "A Lead has been assigned to you.";
                                    email[Email.Description] = "Hi,"+ "<br/>" +
                                                               "<br/>" +
                                                               "New Lead with below details have been assigned to you. Kindly login to review."+ "<br/>" +
                                                               "<b>First Name</b> : " +  leadDetails.GetAttributeValue<string>("firstname") + "<br/>" +
                                                               "<b>Last Name</b> : " + leadDetails.GetAttributeValue<string>("lastname") + "<br/>" +
                                                               "<br/>" +
                                                               "Regards,"+ "<br/>" + 
                                                               "CRM Admin" + "<br/>" +
                                                               "<br/>" +
                                                               "<br/>" +
                                                               "<< <i> This is an auto generated Email. Please do not reply.</i> >>";

                                  var emailId = context.SystemOrganizationService.Create(email);

                                    SendEmailRequest se = new SendEmailRequest
                                    {
                                        EmailId = emailId,
                                        IssueSend = true,
                                        TrackingToken = ""
                                    };
                                    SendEmailResponse sendEmailResponse = (SendEmailResponse)context.SystemOrganizationService.Execute(se);
                                }
                            }
                        }

                    }
                }
            }
        }
            
       
        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Asynchronous, "Create");
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "Create");
        }
    }
}
