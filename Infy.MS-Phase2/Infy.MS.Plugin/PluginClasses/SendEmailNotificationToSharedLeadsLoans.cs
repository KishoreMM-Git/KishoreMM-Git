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
using Microsoft.Xrm.Sdk.Messages;

namespace Infy.MS.Plugins
{
    public class SendEmailNotificationToSharedLeadsLoans : BasePlugin
    {
        public string leadRecordEmailTemplateTitle = string.Empty;
        public string loanRecordEmailTemplateTitle = string.Empty;
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            if (context == null)
            {
                throw new InvalidPluginExecutionException("Context not found");
            }
            if (context.MessageName == "GrantAccess")
            {
                var leadEmailTemplateConfig = context.GetConfigValue<Entity>(Constants.LeadEmailTemplateTitle, Constants.SendEmailNotificationToSharedLeadsLoans);
                var loanEmailTemplateConfig = context.GetConfigValue<Entity>(Constants.LoanEmailTemplateTitle, Constants.SendEmailNotificationToSharedLeadsLoans);
                if(leadEmailTemplateConfig.Contains(Configuration.Value))
                {
                    leadRecordEmailTemplateTitle = leadEmailTemplateConfig.GetAttributeValue<string>(Configuration.Value);
                }
                if (loanEmailTemplateConfig.Contains(Configuration.Value))
                {
                    loanRecordEmailTemplateTitle = loanEmailTemplateConfig.GetAttributeValue<string>(Configuration.Value);
                }
                // var config = context.GetConfigValue<Entity>("", Constants.SendEmailNotificationToSharedLeadsLoans);
                //this GrantAccess is for sharing
                //for Unshare, messagename = "RevokeAccess" and will do the same passing parameters


                

                // Obtain the target entity from the input parameter.
                EntityReference EntityRef = (EntityReference)context.InputParameters["Target"];
                string recordUrl = BuildRecordUrl(context, EntityRef);
                context.Trace(recordUrl);
                //after get this EntityRef then will be easy to continue the logic
                PrincipalAccess PrincipalAccess = (PrincipalAccess)context.InputParameters["PrincipalAccess"];
                var userOrTeam = PrincipalAccess.Principal;
                var userOrTeamId = userOrTeam.Id;
                var userOrTeamName = userOrTeam.Name;
                //this userOrTeam.Name will be blank since entityReference only will give you ID
                var userOrTeamLogicalName = userOrTeam.LogicalName;

                //use the logical Name to know whether this is User or Team!
                if (userOrTeamLogicalName == "team")
                {
                    //what you are going to do if shared to Team?
                }
                if (userOrTeamLogicalName == "systemuser")
                {
                    SendEmailNotification(context,context.OrganizationService, context.InitiatingUserId, userOrTeam.Id, EntityRef,recordUrl);
                    //what you are going to do if shared to Team?
                }
            }
                //throw new NotImplementedException();
        }

        public string  GetUserDetails(IExtendedPluginContext context,Guid userOrTeamId)
        {
            var entity = context.SystemOrganizationService.Retrieve(SystemUser.EntityName, userOrTeamId, new ColumnSet("firstname"));
            if(entity!=null)
            {
                if(entity.Contains("firstname"))
                {
                    return entity.GetAttributeValue<string>("firstname");
                }
            }
            return string.Empty;
        }
        public void SendEmailNotification(IExtendedPluginContext context,IOrganizationService service, Guid fromUser, Guid toUser, EntityReference targetEntityReference,string recordUrl)
        {

            string destinationUrl = string.Empty;
            string emailTemplateBody = string.Empty;
            Entity emailTemplate = null;
            string emailDescription = string.Empty;
            string emailSubject = string.Empty;

            string fromUserName = GetUserDetails(context, fromUser);
            string toUserName = GetUserDetails(context, toUser);
            emailTemplate = GetTemplateByName(leadRecordEmailTemplateTitle, service);
            //if (targetEntityReference.LogicalName == Lead.EntityName)
            //{
            //    var leadEmailDescriptionConfig = context.GetConfigValue<Entity>(Constants.LeadEmailTemplateDescription, Constants.SendEmailNotificationToSharedLeadsLoans);
            //    if (leadEmailDescriptionConfig.Contains(Configuration.ValueMultiline))
            //        emailDescription = leadEmailDescriptionConfig.GetAttributeValue<string>(Configuration.ValueMultiline);

            //    emailSubject = "Lead Record Shared";
            //}
            
            //else if (targetEntityReference.LogicalName == Loan.EntityName)
            //{
            //    var loanEmailDescriptionConfig = context.GetConfigValue<Entity>(Constants.LoanEmailTemplateDescription, Constants.SendEmailNotificationToSharedLeadsLoans);
            //    if (loanEmailDescriptionConfig.Contains(Configuration.ValueMultiline))
            //        emailDescription = loanEmailDescriptionConfig.GetAttributeValue<string>(Configuration.ValueMultiline);

            //    emailSubject = "Shared Loan Record URL";
            //}

            Entity Fromparty = new Entity(ActivityParty.EntityName);
            Entity Toparty = new Entity(ActivityParty.EntityName);
            Toparty[Email.partyid] = new EntityReference(SystemUser.EntityName, toUser);
            Fromparty[Email.partyid] = new EntityReference(SystemUser.EntityName, fromUser);
            Entity email = new Entity(Email.EntityName);

            var instTemplateReq = new InstantiateTemplateRequest
            {
                TemplateId = emailTemplate.Id,
                ObjectId = targetEntityReference.Id,
                ObjectType = Lead.EntityName
            };
            var instTemplateResp = (InstantiateTemplateResponse)service.Execute(instTemplateReq);

            if (instTemplateResp != null)
            {
                email = instTemplateResp.EntityCollection.Entities[0];
                email[Email.Description] = email[Email.Description].ToString().Replace("{ToAddress}", toUserName);
            }
            email[Email.From] = new Entity[] { Fromparty };
            email[Email.To] = new Entity[] { Toparty };
            email.Attributes[Email.Subject] = "Lead Record Shared";
            email.Attributes[Email.RegardingObjectId] = new EntityReference(Lead.EntityName, targetEntityReference.Id);
            Guid _emailId = service.Create(email);
            SendEmailRequest sendEmailreq = new SendEmailRequest
            {
                EmailId = _emailId,
                TrackingToken = "",
                IssueSend = true
            };
            SendEmailResponse sendEmailresp = (SendEmailResponse)service.Execute(sendEmailreq);

            //emailTemplate = GetTemplateByName(loanRecordEmailTemplateTitle, service);
            //Entity emailObject = GetEmailObject(context,fromUserName,toUserName,service, fromUser, toUser, targetEntityReference,recordUrl,emailDescription,emailSubject);
            //if (targetEntityReference.LogicalName == Lead.EntityName && leadRecordEmailTemplateTitle!=string.Empty)
            //     emailTemplate = GetTemplateByName(leadRecordEmailTemplateTitle, service);
            //else if(targetEntityReference.LogicalName==Loan.EntityName && loanRecordEmailTemplateTitle!=string.Empty)
            //    emailTemplate = GetTemplateByName(loanRecordEmailTemplateTitle, service);

            //if(emailTemplate.Contains("body"))
            //{
            //    emailTemplateBody = emailTemplate.GetAttributeValue<string>("body");
            //    if (emailTemplateBody.Contains("URL:Record URL"))
            //    {
            //        context.Trace("found");
            //        context.Trace(recordUrl);
            //        destinationUrl = "<a href=" + recordUrl + ">Click Here</a>";
            //        var emailTemplateBodyWithDestinationUrl = emailTemplateBody.Replace("URL:Record URL", destinationUrl);
            //        emailTemplate["body"] = emailTemplateBodyWithDestinationUrl;
                    
            //        Entity template = new Entity(Template.EntityName);
            //        template.Id = emailTemplate.Id;
            //        template["body"] = emailTemplateBodyWithDestinationUrl;
            //        context.SystemOrganizationService.Update(template);
            //    }
               
            //}
          
            //SendEmailFromTemplateRequest emailUsingTemplateReq = new SendEmailFromTemplateRequest
            //{
            //    Target = emailObject,
            //    TemplateId = emailTemplate.Id,
            //    RegardingId = targetEntityReference.Id,
            //    RegardingType = targetEntityReference.LogicalName
            //};
            ////var emailUsingTemplateResp = (SendEmailFromTemplateResponse)service.Execute(emailUsingTemplateReq);


            //Entity emailtemplate = new Entity(Template.EntityName);
            //emailtemplate.Id = emailTemplate.Id;
            //emailtemplate["body"] = emailTemplateBody;
            //context.SystemOrganizationService.Update(emailtemplate);
        }
        public Entity  GetEmailObject(IExtendedPluginContext context,string fromUserName,string toUserName,IOrganizationService service, Guid fromUser, Guid toUser, EntityReference targetEntityReference,string recordUrl,string emailDescription,string emailSubject)
        {
            Dictionary<string, string> dcConfigDetails = Common.FetchConfigDetails(Constants.AppConfigSetup, service);
            if (!new Common().CheckIsLoanOfficerAssistant(context.SystemOrganizationService, toUser, dcConfigDetails))
            {
                Entity Fromparty = new Entity(ActivityParty.EntityName);
                Entity Toparty = new Entity(ActivityParty.EntityName);
                Toparty[Email.partyid] = new EntityReference(SystemUser.EntityName, toUser);
                Fromparty[Email.partyid] = new EntityReference(SystemUser.EntityName, fromUser);
                Entity email = new Entity(Email.EntityName);
                email[Email.From] = new Entity[] { Fromparty };
                email[Email.To] = new Entity[] { Toparty };
                email[Email.RegardingObjectId] = new EntityReference(targetEntityReference.LogicalName, targetEntityReference.Id);
                email[Email.Subject] = emailSubject;
                var leadDetails = GetLeadDetails(context, targetEntityReference);
                if (emailDescription.Contains("{LeadName}") && leadDetails.Contains("fullname"))
                {
                    emailDescription = emailDescription.Replace("{LeadName}", leadDetails.GetAttributeValue<string>("fullname"));
                }
                if (emailDescription.Contains("{LeadPhoneNumber}") && leadDetails.Contains("mobilephone"))
                {
                    emailDescription = emailDescription.Replace("{LeadPhoneNumber}", leadDetails.GetAttributeValue<string>("mobilephone"));
                }
                if (emailDescription.Contains("{LeadEmailAddress}") && leadDetails.Contains("emailaddress1"))
                {
                    emailDescription = emailDescription.Replace("{LeadEmailAddress}", leadDetails.GetAttributeValue<string>("LeadEmailAddress"));
                }
                if (emailDescription.Contains("{recordUrl}"))
                {
                    //email[Email.Description] = emailDescription.Replace("{recordUrl}", recordUrl);
                    emailDescription = emailDescription.Replace("{recordUrl}", recordUrl);
                }
                if (emailDescription.Contains("{FromAddress}"))
                {
                    emailDescription = emailDescription.Replace("{FromAddress}", fromUserName);
                    //email[Email.Description] = emailDescription.Replace("{FromAddress}", fromUserName);
                }
                if (emailDescription.Contains("{ToAddress}"))
                {
                    emailDescription = emailDescription.Replace("{ToAddress}", toUserName);
                    // email[Email.Description] = emailDescription.Replace("{ToAddress}", toUserName);
                }
                email[Email.Description] = emailDescription;
                var emailId = service.Create(email);

                SendEmailRequest se = new SendEmailRequest
                {
                    EmailId = emailId,
                    IssueSend = true,
                    TrackingToken = ""
                };
                SendEmailResponse sendEmailResponse = (SendEmailResponse)service.Execute(se);
                return email;
            } return null;
        }
        public Entity GetTemplateByName(string title, IOrganizationService crmService)

        {

            var query = new QueryExpression();

            query.EntityName = Template.EntityName;
            query.ColumnSet = new ColumnSet(true);
            var filter = new FilterExpression();

            var condition1 = new ConditionExpression(Template.title, ConditionOperator.Equal, new object[] { title });

            filter.AddCondition(condition1);

            query.Criteria = filter;

            EntityCollection allTemplates = crmService.RetrieveMultiple(query);

            Entity emailTemplate = null;

            if (allTemplates.Entities.Count > 0)
            {
                emailTemplate = allTemplates.Entities[0];
            }

            return emailTemplate;

        }
        public string BuildRecordUrl(IExtendedPluginContext context,EntityReference entityReference)
        {
            var envUrl = string.Empty;
            var leadUrl = string.Empty;
            var loanUrl = string.Empty;
            var envUrlConfig = context.GetConfigValue<Entity>(Constants.EnvUrl, Constants.SendEmailNotificationToSharedLeadsLoans);
            if(envUrlConfig.Contains(Configuration.Value))
            {
                envUrl = envUrlConfig.GetAttributeValue<string>(Configuration.Value);
            }
            if(entityReference.LogicalName==Lead.EntityName)
            {
                var leadUrlConfig = context.GetConfigValue<Entity>(Constants.LeadUrl, Constants.SendEmailNotificationToSharedLeadsLoans);
                if (leadUrlConfig.Contains(Configuration.Value))
                    leadUrl = leadUrlConfig.GetAttributeValue<string>(Configuration.Value);

                context.Trace(leadUrl);
            }
            else if (entityReference.LogicalName == Loan.EntityName)
            {
                var loanUrlConfig = context.GetConfigValue<Entity>(Constants.LoanUrl, Constants.SendEmailNotificationToSharedLeadsLoans);
                if (loanUrlConfig.Contains(Configuration.Value))
                    loanUrl = loanUrlConfig.GetAttributeValue<string>(Configuration.Value);
            }
            //lead url
            if (envUrl != string.Empty && leadUrl != string.Empty)
            {
                if (leadUrl.Contains("{"+ Constants.EnvUrl +"}") && leadUrl.Contains("{"+Constants.LeadUrl+"}"))
                {
                    leadUrl = leadUrl.Replace("{" + Constants.EnvUrl + "}", envUrl);//.Replace("{" + Constants.LeadUrl + "}", entityReference.Id.ToString());
                    leadUrl = leadUrl.Replace("{LeadURL}", entityReference.Id.ToString());
                }
            }
            //loan url
            else if (envUrl != string.Empty && loanUrl != string.Empty)
            {
                if (loanUrl.Contains("{" + Constants.EnvUrl + "}") && loanUrl.Contains("{"+Constants.LoanUrl+"}"))
                {
                    loanUrl = loanUrl.Replace("{" + Constants.EnvUrl + "}", envUrl);//.Replace("{" + Constants.LoanUrl + "}", entityReference.Id.ToString());
                    loanUrl = loanUrl.Replace("{LoanURL}", entityReference.Id.ToString());
                }
            }

            if (entityReference.LogicalName == Lead.EntityName)
                return leadUrl;
            if (entityReference.LogicalName == Loan.EntityName)
                return loanUrl;
            return string.Empty;
        }

        public Entity GetLeadDetails(IExtendedPluginContext context,EntityReference entityReference)
        {
            var result = context.Retrieve(entityReference.LogicalName, entityReference.Id, new ColumnSet("fullname", "mobilephone", "emailaddress1"));
            return result;
        }
        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Asynchronous, "GrantAccess");
        }
    }
}
