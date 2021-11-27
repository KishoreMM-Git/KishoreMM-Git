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
using System.Xml.Linq;

namespace Infy.MS.Plugins
{
    public class AddUserToAccessTeam : BasePlugin
    {
        Entity Target;
        Guid PrimaryLoaId = Guid.Empty;
        Guid Tempid = Guid.Empty;
        Guid CustmorTempid = Guid.Empty;
        Entity Preimage = null;
        Guid RecordID = Guid.Empty;
        Common Maping = new Common();
        Guid UserId = Guid.Empty;
        Guid Leadid = Guid.Empty;
        Guid AccountID = Guid.Empty;
        string TempleteName = Constants.GetEmailTempNameForLOA;
        string username = null;
        Guid Leadownerid = Guid.Empty;
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            if (context == null)
            {
                throw new InvalidPluginExecutionException("Context not Found");
            }


            Tempid = Maping.GetAccessTeamTemplate(context);
            CustmorTempid = Maping.GetAccessTeamTemplatecontact(context);

            Target = context.GetTargetEntity<Entity>();

            if (context.MessageName == "Create" || context.MessageName == "Update")
            {

                if (Target.Contains(Lead.PrimaryLoa))
                {
                    if (Target.GetAttributeValue<EntityReference>(Lead.PrimaryLoa) != null)
                    {
                        PrimaryLoaId = (Guid)(Target.GetAttributeValue<EntityReference>(Lead.PrimaryLoa)).Id;
                        if (PrimaryLoaId != Guid.Empty && PrimaryLoaId != null)
                        {

                            addUsertoTeam(context, Lead.EntityName, PrimaryLoaId, Target.Id, Tempid);
                            //SendEmail(context, Target.Id,);

                        }
                    }
                }

                if (context.PreEntityImages.Contains("Preimage"))
                {
                    Preimage = context.PreEntityImages["Preimage"];
                    if (Preimage != null)
                    {
                        if (Preimage.Contains(Lead.PrimaryLoa))
                        {
                            if (Preimage.GetAttributeValue<EntityReference>(Lead.PrimaryLoa) != null)
                            {
                                PrimaryLoaId = (Guid)(Preimage.GetAttributeValue<EntityReference>(Lead.PrimaryLoa)).Id;
                                if (PrimaryLoaId != null && PrimaryLoaId != Guid.Empty)
                                {
                                    RemoveUserFromRecordTeamRequest removeUser = new RemoveUserFromRecordTeamRequest()
                                    {
                                        Record = new EntityReference(Lead.EntityName, Preimage.Id),
                                        SystemUserId = PrimaryLoaId,
                                        TeamTemplateId = Tempid
                                    };
                                    context.SystemOrganizationService.Execute(removeUser);
                                }

                            }
                        }
                    }
                }
            }

            if (context.MessageName == "RemoveUserFromRecordTeam" || context.MessageName == "AddUserToRecordTeam")
            {

                string Entityname = ((EntityReference)context.InputParameters["Record"]).LogicalName;
                if (((EntityReference)context.InputParameters["Record"]).LogicalName == Lead.EntityName)
                {

                    UserId = (Guid)context.InputParameters["SystemUserId"];
                    Leadid = ((EntityReference)context.InputParameters["Record"]).Id;
                    Tempid = Maping.GetAccessTeamTemplateLoan(context);
                    if (Tempid != null && Tempid != Guid.Empty)
                    {

                        QueryExpression query = new QueryExpression(Loan.EntityName);
                        ColumnSet cols = new ColumnSet(Loan.PrimaryKey, Loan.LoanName);
                        query.ColumnSet = cols;
                        FilterExpression Filter1 = new FilterExpression(LogicalOperator.And);
                        Filter1.AddCondition(Loan.Borrower, ConditionOperator.Equal, Leadid);
                        Filter1.AddCondition(Loan.Status, ConditionOperator.Equal, 0);
                        query.Criteria.AddFilter(Filter1);
                        EntityCollection result = context.RetrieveMultiple(query);// get Related Loan records 

                        ColumnSet attributes = new ColumnSet(Lead.ParentAccountforlead, Lead.PrimaryLoa);
                        Entity entityObj = context.Retrieve(Lead.EntityName, Leadid, attributes);
                        if (context.MessageName == "RemoveUserFromRecordTeam")
                        {
                            if (result.Entities.Count > 0)
                            {
                                RecordID = result.Entities[0].Id;
                                RemoveUserFromRecordTeamRequest(context, Loan.EntityName, UserId, RecordID, Tempid);

                            }
                            if (Leadid != Guid.Empty && UserId != Guid.Empty)
                            {
                                //ColumnSet attributes = new ColumnSet(Lead.PrimaryLoa);
                                //Entity entityObj = context.Retrieve(Lead.EntityName, Leadid, attributes);
                                if (entityObj.Contains(Lead.PrimaryLoa))
                                {

                                    Guid PrimaryLoaGuidId = (Guid)(entityObj.GetAttributeValue<EntityReference>(Lead.PrimaryLoa).Id);
                                    if (PrimaryLoaGuidId != Guid.Empty && PrimaryLoaGuidId != null)
                                    {
                                        if (PrimaryLoaGuidId == UserId)
                                        {
                                            Entity leadobj = new Entity(Lead.EntityName);
                                            leadobj[Lead.PrimaryKey] = Leadid;
                                            leadobj[Lead.PrimaryLoa] = null;
                                            context.Update(leadobj);
                                        }
                                    }
                                }
                            }
                            if (entityObj.Contains(Lead.ParentAccountforlead))
                            {
                                AccountID = (Guid)(entityObj.GetAttributeValue<EntityReference>(Lead.ParentAccountforlead)).Id;
                                RemoveUserFromRecordTeamRequest(context, Customer.EntityName, UserId, AccountID, CustmorTempid);//Loan   
                            }
                        }
                        if (context.MessageName == "AddUserToRecordTeam")
                        {
                            if (result.Entities.Count > 0 && Tempid != Guid.Empty)
                            {
                                RecordID = result.Entities[0].Id;
                                addUsertoTeam(context, Loan.EntityName, UserId, RecordID, Tempid);//Loan                             
                            }
                            if (entityObj.Contains(Lead.ParentAccountforlead) && CustmorTempid != null)
                            {
                                AccountID = (Guid)(entityObj.GetAttributeValue<EntityReference>(Lead.ParentAccountforlead)).Id;
                                addUsertoTeam(context, Customer.EntityName, UserId, AccountID, CustmorTempid);//Loan  

                            }
                            if (Leadid != null)
                            {

                                Entity systemuserobj = context.Retrieve(SystemUser.EntityName, UserId, new ColumnSet(SystemUser.FullName));
                                username = systemuserobj.GetAttributeValue<string>(SystemUser.FullName);
                                Entity leadobjdata = context.Retrieve(Lead.EntityName, Leadid, new ColumnSet(Lead.Owner));
                                Leadownerid = (Guid)(leadobjdata.GetAttributeValue<EntityReference>(Lead.Owner)).Id;
                                if (Leadid != Guid.Empty && username != null && Leadownerid != Guid.Empty && UserId != Guid.Empty)
                                {
                                    SendEmail(context, Leadid, username, Leadownerid, UserId,context.SystemOrganizationService);
                                }
                            }
                        }
                    }
                }
            }
            if(context.MessageName== "GrantAccess")
            {
                // Obtain the target entity from the input parameter.
                EntityReference EntityRef = (EntityReference)context.InputParameters["Target"];
                var leadGuid = EntityRef.Id;

                //after get this EntityRef then will be easy to continue the logic

                //Obtain the principal access object from the input parameter
                Microsoft.Crm.Sdk.Messages.PrincipalAccess PrincipalAccess = (Microsoft.Crm.Sdk.Messages.PrincipalAccess)context.InputParameters["PrincipalAccess"];
                //***to Get User/Team that being Shared With
                var userOrTeam = PrincipalAccess.Principal;
                var userOrTeamId = userOrTeam.Id;
                var userOrTeamName = userOrTeam.Name;
                //this userOrTeam.Name will be blank since entityReference only will give you ID
                var userOrTeamLogicalName = userOrTeam.LogicalName;

                //use the logical Name to know whether this is User or Team!
                //if (userOrTeamLogicalName == "team"|| userOrTeamLogicalName == "systemuser")
                if (userOrTeamLogicalName == "systemuser")
                {
                    //AddUserToRecordTeamRequest adduser = new AddUserToRecordTeamRequest()
                    //{
                    //    Record = new EntityReference(Lead.EntityName, leadGuid),
                    //    SystemUserId = userOrTeamId,
                    //    TeamTemplateId = Tempid
                    //};
                    //context.Execute(adduser);
                 
                        addUsertoTeam(context, Lead.EntityName, userOrTeamId, leadGuid, Tempid);
                 }
               
            }
            if(context.MessageName=="RevokeAccess")
            {
                // Obtain the target entity from the input parameter.
                EntityReference EntityRef = (EntityReference)context.InputParameters["Target"];
                //after get this EntityRef then will be easy to continue the logic
                var leadGuid = EntityRef.Id;

                //Unshare does not have PrincipalAccess because it removes all, only can get the revokee
                //Obtain the principal access object from the input parameter
                var Revokee = (EntityReference)context.InputParameters["Revokee"];
                var RevokeeId = Revokee.Id;
                var RevokeeLogicalName = Revokee.LogicalName; //this one Team or User
                if (RevokeeLogicalName == "systemuser")
                {
                    RemoveUserFromRecordTeamRequest(context, Lead.EntityName, RevokeeId, leadGuid, Tempid);
                }
            }
        }
        public void addUsertoTeam(IExtendedPluginContext context, string EntityName, Guid PrimaryLoaId, Guid RecordID, Guid Tempid)
        {
            AddUserToRecordTeamRequest adduser = new AddUserToRecordTeamRequest()
            {
                Record = new EntityReference(EntityName, RecordID),
                SystemUserId = PrimaryLoaId,
                TeamTemplateId = Tempid
            };
            context.Execute(adduser);
        }

        public void RemoveUserFromRecordTeamRequest(IExtendedPluginContext context, string EntityName, Guid PrimaryLoaId, Guid RecordID, Guid Tempid)
        {
            RemoveUserFromRecordTeamRequest removeUser = new RemoveUserFromRecordTeamRequest()
            {
                Record = new EntityReference(EntityName, RecordID),
                SystemUserId = PrimaryLoaId,
                TeamTemplateId = Tempid
            };
            context.Execute(removeUser);
        }

        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "Create");
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "Update");
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "RemoveUserFromRecordTeam");
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "AddUserToRecordTeam");
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "GrantAccess");
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "RevokeAccess");
        }

        public void SendEmail(IOrganizationService service, Guid leadid, String UserName, Guid FromUSer, Guid Touser,IOrganizationService systemService)
        {
            Dictionary<string, string> dcConfigDetails = Common.FetchConfigDetails(Constants.AppConfigSetup, service);
            if (new Common().CheckIsLoanOfficerAssistant(systemService, Touser, dcConfigDetails))
            {
                Entity fromActivityParty = new Entity(ActivityParty.EntityName);
                Entity toActivityParty = new Entity(ActivityParty.EntityName);
                fromActivityParty[ActivityParty.PartyId] = new EntityReference(SystemUser.EntityName, FromUSer);
                toActivityParty[ActivityParty.PartyId] = new EntityReference(SystemUser.EntityName, Touser);
                QueryExpression emailTemplete = new QueryExpression(Template.EntityName);
                ColumnSet cols = new ColumnSet(true);
                emailTemplete.ColumnSet = cols;
                FilterExpression Filter1 = new FilterExpression(LogicalOperator.And);
                Filter1.AddCondition(Template.title, ConditionOperator.Equal, TempleteName);
                emailTemplete.Criteria.AddFilter(Filter1);
                EntityCollection result = service.RetrieveMultiple(emailTemplete);

                if (result.Entities.Count > 0)
                {
                    Entity templete = result.Entities[0];
                    //Entity template = service.Retrieve("template", new Guid("05E8421A-0661-EA11-A811-000D3A4F6FCE"), new ColumnSet(true));


                    Entity email = new Entity(Email.EntityName);

                    var instTemplateReq = new InstantiateTemplateRequest
                    {
                        TemplateId = templete.Id,
                        ObjectId = leadid,
                        ObjectType = Lead.EntityName
                    };
                    var instTemplateResp = (InstantiateTemplateResponse)service.Execute(instTemplateReq);

                    if (instTemplateResp != null)
                    {
                        email = instTemplateResp.EntityCollection.Entities[0];
                        email[Email.Description] = email[Email.Description].ToString().Replace("LOA", UserName);
                    }



                    //email.Attributes[Email.Description] = GetDataFromXml(templete.Attributes["body"].ToString(), "match");
                    //email.Attributes[Email.Description] = email.Attributes[Email.Description].ToString().Replace("LOA", UserName);
                    email.Attributes[Email.To] = new Entity[] { toActivityParty };
                    email.Attributes[Email.From] = new Entity[] { fromActivityParty };
                    email.Attributes[Email.RegardingObjectId] = new EntityReference(Lead.EntityName, leadid);
                    Guid _emailId = service.Create(email);
                    SendEmailRequest sendEmailreq = new SendEmailRequest
                    {
                        EmailId = _emailId,
                        TrackingToken = "",
                        IssueSend = true
                    };
                    SendEmailResponse sendEmailresp = (SendEmailResponse)service.Execute(sendEmailreq);

                }
            }
        }
        private static string GetDataFromXml(string value, string attributeName)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            XDocument document = XDocument.Parse(value);
            // get the Element with the attribute name specified  
            XElement element = document.Descendants().Where(ele => ele.Attributes().Any(attr => attr.Name == attributeName)).FirstOrDefault();
            return element == null ? string.Empty : element.Value;
        }
        
    }
}
