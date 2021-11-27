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

namespace Infy.MS.Plugins.PluginClasses
{
    public class LeadUpdatePostOperation : BasePlugin
    {
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            if (context == null)
            {
                throw new InvalidPluginExecutionException("Context not found");
            }
            if (context.MessageName.ToLower() == "update")
            {
                if ((context.InputParameters.Contains("Target")) && (context.InputParameters["Target"] is Entity))
                {
                    var entity = (Entity)context.InputParameters["Target"];
                    if (entity.LogicalName.ToLower() != "lead") return;
                    try
                    {
                        Entity preImage = (Entity)context.PreEntityImages["Image"];
                        Entity postImage = (Entity)context.PostEntityImages["PostImage"];
                        DeleteQueueItem(preImage, entity, context.SystemOrganizationService);
                        //Updating Related Loan Owner to Assigned Lead owner on update
                        context.Trace($"Entering Plugin");
                        if (context.PostEntityImages != null)
                        {
                            EntityCollection loanEntity = (ListRelatedEntities(entity, "opportunity_originating_lead", "opportunity", context));
                            if (loanEntity.Entities.Count > 0)
                            {
                                if (preImage.Contains(Lead.LoanNumber))
                                {
                                    for (var i = 0; i < loanEntity.Entities.Count; i++)
                                    {
                                        if (loanEntity.Entities[i].Contains(Loan.LoanNumber))
                                        {
                                            if (preImage.Attributes[Lead.LoanNumber].Equals(loanEntity.Entities[i].GetAttributeValue<String>(Loan.LoanNumber)))
                                            {
                                                Entity loanEnt = new Entity(loanEntity.EntityName);
                                                loanEnt.Id = loanEntity.Entities[i].Id;
                                                loanEnt[Loan.Owner] = ((EntityReference)postImage.Attributes[Lead.Owner]);
                                                context.Update(loanEnt);
                                                context.Trace($"Updating Loan");
                                            }
                                        }
                                    }
                                }

                            }
                        }

                        if (preImage.Contains(Lead.Owner))
                        {
                            Guid Id = ((EntityReference)preImage.Attributes[Lead.Owner]).Id;
                            if (context.PostEntityImages != null)
                            {

                                Guid postId = ((EntityReference)postImage.Attributes[Lead.Owner]).Id;
                                //Checking Previous Owner belongs to "Movement Direct LO" or "Movement Direct Admin"

                                if (userHasRole(Id, "Movement Direct LO", context) == true || userHasRole(Id, "Movement Direct Admin", context) == true)
                                {
                                    if (preImage.Attributes[Lead.Owner].Equals(postImage.Attributes[Lead.Owner]))
                                    {
                                      //  return;
                                    }
                                    else
                                    {

                                        Entity leadEntity = new Entity(entity.LogicalName);
                                        leadEntity.Id = entity.Id;

                                        //Updating Movement Direct LO Field in Lead Entity to Previous Owner
                                        leadEntity[Lead.MDLO] = ((EntityReference)preImage.Attributes[Lead.Owner]);
                                        context.Update(leadEntity);

                                        //Retrieving Related Loan Record & Checking Loan Number Equals Lean Loan Number Field & Add User to Loan Access Team
                                        EntityCollection entityToAdd = (ListRelatedEntities(entity, "opportunity_originating_lead", "opportunity", context));
                                        EntityCollection customerEntityToAdd = (ListRelatedEntities(entity, "account_originating_lead", "account", context));


                                        //Remove Existing Users from Access Team
                                        if (preImage.Contains(Lead.MDLO))
                                        {
                                            Guid removeUserId = ((EntityReference)preImage.Attributes[Lead.MDLO]).Id;
                                            removeUserFromAT("LeadMDLO", entity.ToEntityReference(), removeUserId, context);
                                            if (entityToAdd.Entities.Count > 0)
                                            {
                                                if (preImage.Contains(Lead.LoanNumber))
                                                {
                                                    for (var i = 0; i < entityToAdd.Entities.Count; i++)
                                                    {
                                                        if (entityToAdd.Entities[i].Contains(Loan.LoanNumber))
                                                        {
                                                            if (preImage.Attributes[Lead.LoanNumber].Equals(entityToAdd.Entities[i].GetAttributeValue<String>(Loan.LoanNumber)))
                                                            {
                                                                removeUserFromAT("LoanMDLO", entityToAdd.Entities[i].ToEntityReference(), removeUserId, context);
                                                            }

                                                        }
                                                    }
                                                }
                                            }
                                            if (customerEntityToAdd.Entities.Count > 0)
                                            {
                                                removeUserFromAT("CustomerMDLO", customerEntityToAdd.Entities[0].ToEntityReference(), removeUserId, context);

                                            }

                                        }

                                        //Adding Previous Owner to Lead Access Team
                                        addUserToAT("LeadMDLO", entity.ToEntityReference(), Id, context);


                                        if (entityToAdd.Entities.Count > 0)
                                        {
                                            if (preImage.Contains(Lead.LoanNumber))
                                            {
                                                for (var i = 0; i < entityToAdd.Entities.Count; i++)
                                                {
                                                    if (entityToAdd.Entities[i].Contains(Loan.LoanNumber))
                                                    {
                                                        if (preImage.Attributes[Lead.LoanNumber].Equals(entityToAdd.Entities[i].GetAttributeValue<String>(Loan.LoanNumber)))
                                                        {
                                                            addUserToAT("LoanMDLO", entityToAdd.Entities[i].ToEntityReference(), Id, context);
                                                        }
                                                    }
                                                }
                                            }

                                        }
                                        if (customerEntityToAdd.Entities.Count > 0)
                                        {
                                            addUserToAT("CustomerMDLO", customerEntityToAdd.Entities[0].ToEntityReference(), Id, context);

                                        }



                                    }
                                }
                                else
                                {
                                    Entity leadEntity = new Entity(entity.LogicalName);
                                    leadEntity.Id = entity.Id;
                                    //Updating Movement Direct LO Field in Lead Entity to Previous Owner
                                    if (preImage.Contains(Lead.MDLO))
                                    {
                                        leadEntity[Lead.MDLO] = null;
                                        context.Update(leadEntity);

                                        Guid removeUserId = ((EntityReference)preImage.Attributes[Lead.MDLO]).Id;
                                        removeUserFromAT("LeadMDLO", entity.ToEntityReference(), removeUserId, context);
                                        EntityCollection entityToAdd = (ListRelatedEntities(entity, "opportunity_originating_lead", "opportunity", context));
                                        if (entityToAdd.Entities.Count > 0)
                                        {
                                            if (preImage.Contains(Lead.LoanNumber))
                                            {
                                                for (var i = 0; i < entityToAdd.Entities.Count; i++)
                                                {
                                                    if (entityToAdd.Entities[i].Contains(Loan.LoanNumber))
                                                    {
                                                        if (preImage.Attributes[Lead.LoanNumber].Equals(entityToAdd.Entities[i].GetAttributeValue<String>(Loan.LoanNumber)))
                                                        {
                                                            removeUserFromAT("LoanMDLO", entityToAdd.Entities[i].ToEntityReference(), removeUserId, context);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        EntityCollection customerEntityToAdd = (ListRelatedEntities(entity, "account_originating_lead", "account", context));
                                        if (customerEntityToAdd.Entities.Count > 0)
                                        {
                                            removeUserFromAT("CustomerMDLO", customerEntityToAdd.Entities[0].ToEntityReference(), removeUserId, context);

                                        }

                                    }
                                }

                            }
                        }
                        //When Lead Owner changes Change the Owner to ParentAccountId record owner to Lead Owner
                        if (postImage.Contains(Lead.Owner))
                        {
                            UpdateParentAccountOwner(context, postImage.Id);
                        }

                        #region Close All Automated Tasks when LeadStatus Is Dead Lead /Loan or Disqulify       
                        if (postImage.Contains(Lead.Status))
                        {

                            context.Trace($"PIP2.0 Lead Contains State code");
                            /*  if (preImage.Attributes[Lead.Status].Equals(postImage.Attributes[Lead.Status]))
                              {
                                 // return;
                              }
                              else  */
                            if (!preImage.Attributes[Lead.Status].Equals(postImage.Attributes[Lead.Status]))
                            {
                                context.Trace($"PIP2.0 Lead Contains State code is different");
                                //int leadstatusVal = Convert.ToInt32(postImage.GetAttributeValue<OptionSetValue>(Lead.Status));
                                var leadstatusVal = postImage.GetAttributeValue<OptionSetValue>(Lead.Status).Value;
                                context.Trace($"PIP2.0 Lead Statecode" + leadstatusVal);
                                if (leadstatusVal.Equals(2))
                                {
                                    string getLoans = @"<fetch top='5000' >
                                                              <entity name='opportunity' >
                                                                <attribute name='opportunityid' />
                                                                <attribute name='name' />
                                                                <attribute name='originatingleadid' />
                                                                <filter>
                                                                  <condition attribute='originatingleadid' operator='eq' value='" + entity.Id + @"' />
                                                                </filter>
                                                              </entity>
                                                            </fetch>";

                                    EntityCollection loancoll = context.RetrieveMultiple(new FetchExpression(getLoans));
                                    context.Trace($"PIP2.0 loancoll" + loancoll.Entities.Count);
                                    foreach (var loanrecord in loancoll.Entities)
                                    {
                                        //                //Entity taskety = new Entity(Task.EntityName, tasks.Id);
                                        //                Entity taskety = new Entity("task", loanrecord.Id);
                                        //                // taskety[Task.StateCode] = new OptionSetValue(2);
                                        //                taskety["statecode"] = new OptionSetValue(2);
                                        //                context.Update(taskety);
                                        //                context.Trace("update sucessful");
                                        string getAutomatedTasks = @"<fetch top='5000' >
                                                              <entity name='task' >
                                                                <attribute name='subject' />
                                                                <attribute name='description' />
                                                                <attribute name='ims_automatedtask' />
                                                                <filter>
                                                                  <condition attribute='ims_automatedtask' operator='eq' value='1' />
                                                                  <condition attribute='statecode' operator='eq' value='0' />
                                                                  <condition attribute='regardingobjectid' operator='eq' value='" + loanrecord.Id + @"' />
                                                                </filter>
                                                              </entity>
                                                            </fetch>";

                                        EntityCollection autoamtedTask = context.RetrieveMultiple(new FetchExpression(getAutomatedTasks));
                                        context.Trace($"PIP2.0 task" + autoamtedTask.Entities.Count);
                                        foreach (var tasks in autoamtedTask.Entities)
                                        {
                                            Entity taskety = new Entity("task", tasks.Id);
                                            // taskety[Task.StateCode] = new OptionSetValue(2);
                                            taskety["statecode"] = new OptionSetValue(2);
                                            context.Update(taskety);
                                            context.Trace("PIP2.0 Task cancel sucessful");
                                        }
                                    }

                                }


                            }

                        }
                        if (postImage.Contains(Lead.LeadStatus))
                        {
                            context.Trace($"PIP2.0-Lead Contains Lead status ");
                            /* if (preImage.Attributes[Lead.LeadStatus].Equals(postImage.Attributes[Lead.LeadStatus]))
                             {
                                // return;
                             }
                             else*/
                            if (!preImage.Attributes[Lead.LeadStatus].Equals(postImage.Attributes[Lead.LeadStatus]))
                            {
                                context.Trace($"PIP2.0-Lead status is different ");
                                var leadstatusName = postImage.GetAttributeValue<EntityReference>(Lead.LeadStatus).Name;
                                context.Trace($"PIP2.0-Lead status-" + leadstatusName);
                                if (leadstatusName.ToLower() == "dead lead")
                                {
                                    string getLoans = @"<fetch top='5000' >
                                                              <entity name='opportunity' >
                                                                <attribute name='opportunityid' />
                                                                <attribute name='name' />
                                                                <attribute name='originatingleadid' />
                                                                <filter>
                                                                  <condition attribute='originatingleadid' operator='eq' value='" + entity.Id + @"' />
                                                                </filter>
                                                              </entity>
                                                            </fetch>";

                                    EntityCollection loancoll = context.RetrieveMultiple(new FetchExpression(getLoans));
                                    context.Trace($"PIP2.0-loancoll-" + loancoll.Entities.Count);
                                    foreach (var loanrecord in loancoll.Entities)
                                    {
                                        ////Entity taskety = new Entity(Task.EntityName, tasks.Id);
                                        //Entity taskety = new Entity("task", loanrecord.Id);
                                        //// taskety[Task.StateCode] = new OptionSetValue(2);
                                        //taskety["statecode"] = new OptionSetValue(2);
                                        //context.Update(taskety);
                                        //context.Trace("update sucessful");
                                        string getAutomatedTasks = @"<fetch top='5000' >
                                                              <entity name='task' >
                                                                <attribute name='subject' />
                                                                <attribute name='description' />
                                                                <attribute name='ims_automatedtask' />
                                                                <filter>
                                                                  <condition attribute='ims_automatedtask' operator='eq' value='1' />
                                                                  <condition attribute='statecode' operator='eq' value='0' />
                                                                  <condition attribute='regardingobjectid' operator='eq' value='" + loanrecord.Id + @"' />
                                                                </filter>
                                                              </entity>
                                                            </fetch>";

                                        EntityCollection autoamtedTask = context.RetrieveMultiple(new FetchExpression(getAutomatedTasks));
                                        foreach (var tasks in autoamtedTask.Entities)
                                        {
                                            //Entity taskety = new Entity(Task.EntityName, tasks.Id);
                                            Entity taskety = new Entity("task", tasks.Id);
                                            // taskety[Task.StateCode] = new OptionSetValue(2);
                                            taskety["statecode"] = new OptionSetValue(2);
                                            context.Update(taskety);
                                            context.Trace("update sucessful");
                                        }
                                    }

                                }


                            }

                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {

                        throw new InvalidPluginExecutionException("An error occurred in LeadUpdatePostOperationPlugin.", ex);
                    }
                }
            }

        }
        private static bool userHasRole(Guid userId, string roleName, IOrganizationService service)
        {
            bool hasRole = false;
            try
            {
                QueryExpression qe = new QueryExpression("systemuserroles");
                qe.Criteria.AddCondition("systemuserid", ConditionOperator.Equal, userId);
                LinkEntity link = qe.AddLink("role", "roleid", "roleid", JoinOperator.Inner);
                link.LinkCriteria.AddCondition("name", ConditionOperator.Equal, roleName);
                EntityCollection results = service.RetrieveMultiple(qe);
                hasRole = results.Entities.Count > 0;
                return hasRole;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while checking Role: " + ex.Message);
            }
        }
        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "Update");
        }
        public EntityCollection ListRelatedEntities(Entity parent, string relationshipLogicalName, string relatedEntity, IOrganizationService service)
        {
            try
            {
                Relationship relationship = new Relationship()
                {
                    SchemaName = relationshipLogicalName
                };
                RelationshipQueryCollection relatedEntityCollection = new RelationshipQueryCollection();
                relatedEntityCollection.Add(relationship,
                    new QueryExpression()
                    {
                        EntityName = relatedEntity,
                        ColumnSet = new ColumnSet(true)
                    });

                RetrieveRequest request = new RetrieveRequest()
                {
                    RelatedEntitiesQuery = relatedEntityCollection,
                    ColumnSet = new ColumnSet(true),
                    Target = new EntityReference
                    {
                        Id = parent.Id,
                        LogicalName = parent.LogicalName
                    }
                };

                RetrieveResponse response = (RetrieveResponse)service.Execute(request);
                if (response == null) return null;
                if (response.Entity == null) return null;
                return response.Entity.RelatedEntities[relationship];
            }
            catch (Exception ex)
            {
                throw new Exception("Error while retrieving Related Loan: " + ex.Message);
            }
        }

        public void addUserToAT(String Team, EntityReference entity, Guid Id, IOrganizationService service)
        {
            try
            {
                ConditionExpression condition = new ConditionExpression();
                condition.AttributeName = "teamtemplatename";
                condition.Operator = ConditionOperator.Equal;
                condition.Values.Add(Team);
                FilterExpression filter = new FilterExpression();
                filter.Conditions.Add(condition);
                QueryExpression query = new QueryExpression("teamtemplate");
                query.Criteria.AddFilter(filter);
                query.ColumnSet = new ColumnSet("teamtemplatename");
                var accessTeamColl = service.RetrieveMultiple(query);
                Guid teamTemplateId = (Guid)accessTeamColl[0].Attributes["teamtemplateid"];
                var entityToAdd = entity;
                var UserId = Id;
                AddUserToRecordTeamRequest adduser = new AddUserToRecordTeamRequest()
                {
                    Record = entityToAdd,
                    SystemUserId = UserId,
                    TeamTemplateId = teamTemplateId
                };
                AddUserToRecordTeamResponse response = (AddUserToRecordTeamResponse)service.Execute(adduser);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while adding User to Access Team: " + ex.Message);
            }
        }
        public void removeUserFromAT(String Team, EntityReference entity, Guid Id, IOrganizationService service)
        {
            try
            {
                ConditionExpression condition = new ConditionExpression();
                condition.AttributeName = "teamtemplatename";
                condition.Operator = ConditionOperator.Equal;
                condition.Values.Add(Team);
                FilterExpression filter = new FilterExpression();
                filter.Conditions.Add(condition);
                QueryExpression query = new QueryExpression("teamtemplate");
                query.Criteria.AddFilter(filter);
                query.ColumnSet = new ColumnSet("teamtemplatename");
                var accessTeamColl = service.RetrieveMultiple(query);
                Guid teamTemplateId = (Guid)accessTeamColl[0].Attributes["teamtemplateid"];
                var entityToAdd = entity;
                var UserId = Id;
                RemoveUserFromRecordTeamRequest removeUser = new RemoveUserFromRecordTeamRequest()
                {
                    Record = entityToAdd,
                    SystemUserId = UserId,
                    TeamTemplateId = teamTemplateId
                };
                RemoveUserFromRecordTeamResponse response = (RemoveUserFromRecordTeamResponse)service.Execute(removeUser);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while removing User from Access Team: " + ex.Message);
            }

        }

        public void UpdateParentAccountOwner(IExtendedPluginContext context, Guid leadGuid)
        {
            var leadEntity = context.Retrieve(Lead.EntityName, leadGuid, new ColumnSet(Lead.ParentAccountforlead, Lead.Owner));
            if (leadEntity != null && leadEntity.Contains(Lead.ParentAccountforlead))
            {
                Entity customerEntity = new Entity(Account.EntityName);
                customerEntity.Id = leadEntity.GetAttributeValue<EntityReference>(Lead.ParentAccountforlead).Id;
                customerEntity[Account.Owner] = leadEntity.GetAttributeValue<EntityReference>(Lead.Owner);
                context.SystemOrganizationService.Update(customerEntity);
            }
        }

        public void DeleteQueueItem(Entity PreImage, Entity entity, IOrganizationService sysAdminSvc)
        {
            if (PreImage.Contains(Lead.Owner) && entity.Contains(Lead.Owner))
            {
                string entityName = PreImage.GetAttributeValue<EntityReference>(Lead.Owner).LogicalName;
                string teamName = PreImage.GetAttributeValue<EntityReference>(Lead.Owner).Name;

                if ((entityName != "team") && (teamName != "Movement Direct Team"))
                {
                    if (PreImage.Attributes[Lead.Owner] != entity.Attributes[Lead.Owner])
                    {
                        QueryExpression query = new QueryExpression();
                        query.EntityName = "queueitem";
                        query.ColumnSet = new ColumnSet("objectid");
                        query.Criteria = new FilterExpression();
                        query.Criteria.AddCondition(new ConditionExpression("objectid", ConditionOperator.Equal, entity.Id));

                        RetrieveMultipleRequest request = new RetrieveMultipleRequest();
                        request.Query = query;
                        EntityCollection results = ((RetrieveMultipleResponse)sysAdminSvc.Execute(request)).EntityCollection;


                        DataCollection<Entity> queueItems = results.Entities;
                        if (queueItems.Count > 0)
                        {
                            foreach (Entity item in queueItems)
                            {
                                DeleteRequest deleteItem = new DeleteRequest()
                                {
                                    Target = new EntityReference(item.LogicalName, item.Id)
                                };

                                sysAdminSvc.Execute(deleteItem);
                            }
                        }
                    }
                }
            }
        }

    }
}
