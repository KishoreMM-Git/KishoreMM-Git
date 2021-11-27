using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Crm.Sdk.Messages;
//using Xrm;
//using XRMExtensions;
using System.Net;
//using System.ServiceModel.Description;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System.Xml.Linq;
using System.IO;
using System.Xml;
using Microsoft.Xrm.Sdk.Messages;

namespace Infy.MS.Plugins
{
    public class SendEmailAfterBookableResourceBookingRecordCreation : IPlugin
    {
        // private object SystemOrganizationService;

        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {


                //  IOrganizationService svc = (IOrganizationService)serviceProvider.GetService(typeof(IOrganizationService));

                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                IOrganizationServiceFactory serviceFactory =
                (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
                if (context == null)
                {
                    throw new InvalidPluginExecutionException("Context not found");
                }

                if (context.MessageName.ToLower() == "create")
                {

                    ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
                    Entity Target = null;
                    //Target = context.GetTargetEntity<Entity>();
                    if (context.InputParameters.Contains("Target"))
                    {
                        if (context.InputParameters["Target"] is Entity)
                        {
                            Target = (Entity)context.InputParameters["Target"];
                        }
                    }

                    if (Target != null)
                    {
                        if (Target.Contains("resource") && Target.Contains("ims_routingrule"))
                        {
                            Guid RoutingRuleId = Target.GetAttributeValue<EntityReference>("ims_routingrule").Id;
                            Guid ResourceId = Target.GetAttributeValue<EntityReference>("resource").Id;
                            

                          //  string entityname = Target.GetAttributeValue<EntityReference>("resource");
                            Guid leadid = Target.GetAttributeValue<EntityReference>("ims_lead").Id;

                            ColumnSet userAttributes = new ColumnSet("userid");
                            Entity sysUserObj = service.Retrieve("bookableresource", ResourceId, userAttributes);
                            Guid To = Guid.Empty;
                            string ResourceName = "";
                            if (sysUserObj.Contains("userid"))
                            {
                                 To = sysUserObj.GetAttributeValue<EntityReference>("userid").Id;
                                 ResourceName = sysUserObj.GetAttributeValue<EntityReference>("userid").Name;
                            }
                           // throw new InvalidPluginExecutionException(To.ToString());
                           // throw new InvalidPluginExecutionException( RoutingRuleId +" ---" +ResourceId+ " ---" + ResourceName+ " ---" + entityname+ " ---" + leadid +"---"+To);

                            if (RoutingRuleId != null && RoutingRuleId != Guid.Empty)
                            {
                                ColumnSet attributes = new ColumnSet("ims_triggernotification");
                                Entity entityObj = service.Retrieve("ims_routingrule", RoutingRuleId, attributes);
                                if (entityObj != null)
                                {
                                    //bool notificationValue = entityObj.GetAttributeValue<bool>("ims_triggernotification");
                                    if (entityObj.GetAttributeValue<bool>("ims_triggernotification") == true)
                                    {
                                       // tracingService.Trace(entityObj.GetAttributeValue<bool>("ims_triggernotification").ToString());
                                        Entity Fromparty = new Entity("activityparty");
                                        Entity Toparty = new Entity("activityparty");
                                        Toparty["partyid"] = new EntityReference("systemuser", To);
                                        Fromparty["partyid"] = new EntityReference("systemuser", new Guid("6dd1b8f9-c553-ea11-a814-000d3a33fcaa"));

                                        // Create an e-mail message.

                                        Entity email = new Entity("email");
                                        email["from"] = new Entity[] { Fromparty };
                                        email["to"] = new Entity[] { Toparty };
                                        //   email[Email.RegardingObjectId] = new EntityReference(SystemUser.EntityName, ResourceId);
                                        email["subject"] = "A Lead has been assigned to you.";
                                        email["description"] = "Hi" + ResourceName + ", A Lead has been assigned to you by a routing name.";
                                        var emailId = service.Create(email);

                                        SendEmailRequest se = new SendEmailRequest
                                        {
                                            EmailId = emailId,
                                            IssueSend = true,
                                            TrackingToken = ""
                                        };
                                        SendEmailResponse sendEmailResponse = (SendEmailResponse)service.Execute(se);
                                        //tracingService.Trace("Email Sent Succesfully");
                                    }
                                }
                            }

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }


        }

        //public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        //{
        //    yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Asynchronous, "Create");
        //}


    }
}
