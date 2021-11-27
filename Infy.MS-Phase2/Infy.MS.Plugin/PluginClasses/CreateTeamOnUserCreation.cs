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
    public class CreateTeamOnUserCreation : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingservice = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.  
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            if (context == null)
            {
                throw new InvalidPluginExecutionException("Context Not Found");
            }

            if ((context.MessageName.ToLower() == "create") || (context.MessageName.ToLower() == "update"))
            {
                try
                {
                    string fullname = string.Empty;
                    EntityReference businessUnit = null;

                    Entity systemUser = (Entity)context.InputParameters["Target"];
                    if (context.MessageName.ToLower() == "update")
                    {
                        tracingservice.Trace("Plugin Triggers on update of User");
                        //QueryExpression query = new QueryExpression
                        //{
                        //    EntityName = systemUser.LogicalName,
                        //    ColumnSet = new ColumnSet(SystemUser.EntityId, SystemUser.BU,SystemUser.FullName,SystemUser.FirstName),
                        //    Criteria = new FilterExpression
                        //    {
                        //        Conditions =
                        //        {
                        //            new ConditionExpression
                        //            {
                        //            AttributeName = SystemUser.EntityId,
                        //            Operator = ConditionOperator.Equal,
                        //            Values = { systemUser.Id }
                        //            }
                        //        }
                        //    }
                        //};

                        string fetchxml = @"<fetch top='50' >
                              <entity name='systemuser' >
                                <attribute name='lastname' />
                                <attribute name='firstname' />
                                <attribute name='systemuserid' />
                                <attribute name='businessunitid' />
                                <attribute name='fullname' />
                                <attribute name='ims_loa' />
                                <filter>
                                  <condition attribute='systemuserid' operator='eq' value='" + systemUser.Id + @"'  />
                                  <condition attribute='applicationid' operator='null' /> 
                                </filter>
                                    <link-entity name='systemuserroles' alias='a1' link-type='inner' to='systemuserid' from='systemuserid' >
                                          <link-entity name='role' alias='a2' link-type='inner' to='roleid' from='roleid' >
                                            <attribute name='name' />
                                            <filter type='or' >
                                              <condition attribute='name' value='Movement Direct Admin' operator='eq' />
                                              <condition attribute='name' value='Movement Direct LO' operator='eq' />
                                              <condition attribute='name' value='Retail Loan Officer' operator='eq' />
                                              <condition attribute='name' value='Marketing Team' operator='eq' />
                                            </filter>
                                          </link-entity>
                                 </link-entity>
                              </entity>
                            </fetch>";
                        EntityCollection etycoll = service.RetrieveMultiple(new FetchExpression(fetchxml));
                        //EntityCollection etyco = service.RetrieveMultiple(query);
                        tracingservice.Trace("The user record fetched on update of record");

                        if (etycoll.Entities.Count > 0)
                        {
                            fullname = etycoll.Entities[0].GetAttributeValue<string>(SystemUser.FullName);
                            tracingservice.Trace("The fulname from the queryexpression" + fullname);
                            businessUnit = etycoll.Entities[0].GetAttributeValue<EntityReference>(SystemUser.BU);
                            tracingservice.Trace("The businessUnit from the queryexpression" + businessUnit.Name);
                            Guid systemuser = etycoll.Entities[0].GetAttributeValue<Guid>(SystemUser.EntityId);
                            //Guid systemuserId = systemuser.Id;
                            tracingservice.Trace(systemuser.ToString());

                            Guid[] member = new[] { systemuser };

                            if (etycoll.Entities[0].Attributes.Contains(SystemUser.LOA))
                            {
                                EntityReference defaultLOA = etycoll.Entities[0].GetAttributeValue<EntityReference>(SystemUser.LOA);
                                Guid LOAId = defaultLOA.Id;
                                string etyName = defaultLOA.LogicalName;

                                Entity teamety = service.Retrieve(etyName, LOAId, new ColumnSet(Team.Name, Team.BU, Team.AdminId));
                                teamety[Team.Name] = fullname + " " + "Main Team";
                                teamety[Team.BU] = new EntityReference(businessUnit.LogicalName, businessUnit.Id);

                                AddMembersTeamRequest addrequest = new AddMembersTeamRequest();
                                addrequest.TeamId = teamety.Id;
                                addrequest.MemberIds = member;
                                service.Execute(addrequest);

                                service.Update(teamety);
                            }
                            else
                            {
                                createTeam(service, fullname, businessUnit, systemUser, tracingservice);
                            }
                        }
                    }

                    if (context.MessageName.ToLower() == "create")
                    {
                        //var createValues = 
                        EntityReference bu = null;
                        string name = systemUser.GetAttributeValue<string>(SystemUser.FullName);
                        if (systemUser.Attributes.Contains(SystemUser.BU))
                        {
                            bu = systemUser.GetAttributeValue<EntityReference>(SystemUser.BU);
                        }
                        createTeam(service, name, bu, systemUser, tracingservice);
                    }

                }
                catch (Exception ex)
                {
                    tracingservice.Trace(ex.Message);
                    throw;
                }
            }
        }


        public static void createTeam(IOrganizationService service, string fullname, EntityReference businessUnit, Entity systemUser, ITracingService tracingService)
        {
            tracingService.Trace("Enters the Create TEam function");
            if ((fullname != string.Empty) || (fullname != null))
            {
                Entity newteam = new Entity(Team.EntityName);
                newteam[Team.Name] = fullname + " " + "Main Team";
                newteam[Team.TeamType] = new OptionSetValue(1); //TeamType is set to Access
                newteam[Team.AdminId] = new EntityReference(systemUser.LogicalName, systemUser.Id);
                newteam[Team.BU] = new EntityReference(businessUnit.LogicalName, businessUnit.Id);

                Guid teamid = service.Create(newteam);

                Guid[] member = new[] { systemUser.Id };
                AddMembersTeamRequest addrequest = new AddMembersTeamRequest();
                addrequest.TeamId = teamid;
                addrequest.MemberIds = member;
                service.Execute(addrequest);
                tracingService.Trace("Team Created Succesfully" + teamid.ToString());

                Entity updateentity = new Entity(systemUser.LogicalName);
                //Console.WriteLine(systemUser.LogicalName, systemUser.Id);
                updateentity.Id = systemUser.Id;

                updateentity[SystemUser.LOA] = new EntityReference(newteam.LogicalName, teamid);

                service.Update(updateentity);
                tracingService.Trace("The user record updated succesfully");
            }
        }
    }
}
