using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xrm;
using Microsoft.Xrm.Sdk.Query;
using XRMExtensions;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Task = XRMExtensions.Task;
using Microsoft.Xrm.Sdk.Client;

namespace Infy.MS.Plugins.PluginClasses
{
    public class AutoTaskCreation_Pipeline : IPlugin
    {
        Entity Target = null;
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            try
            {
                if (context == null)
                {
                    throw new NotImplementedException();
                }
                if (context.MessageName.ToLower() == "update" || context.MessageName.ToLower() == "create")
                {
                    if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                    {
                        Target = (Entity)context.InputParameters["Target"];
                        IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                        IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
                        Entity taskety = service.Retrieve(Target.LogicalName, Target.Id, new ColumnSet(Task.RegardingObjectId));
                        tracingService.Trace("Target contains value");

                        /*tracingService.Trace("Impersonate User begins here");
                        #region Impersonate User
                        EntityReference loan = null;
                        tracingService.Trace("after the current user id values here");
                        
                        if (taskety.Attributes.Contains(Task.RegardingObjectId))
                        {
                            loan = taskety.GetAttributeValue<EntityReference>(Task.RegardingObjectId);
                        }
                        Entity loanOwnerId = service.Retrieve(loan.LogicalName, loan.Id, new ColumnSet(Loan.Owner));
                        Guid ownerid = loanOwnerId.GetAttributeValue<EntityReference>(Loan.Owner).Id;
                        tracingService.Trace("the Current owner after impersonation is " + " " + ownerid + " " + loanOwnerId.GetAttributeValue<EntityReference>(Loan.Owner).Name);
                        IOrganizationService impersonatedUser = serviceFactory.CreateOrganizationService(ownerid);
                        #endregion
                        tracingService.Trace("IMpersonating completed");*/

                        if (taskety.Attributes.Contains(Task.RegardingObjectId))
                        {
                            string entityName = taskety.GetAttributeValue<EntityReference>(Task.RegardingObjectId).LogicalName;
                            Guid regardingId = taskety.GetAttributeValue<EntityReference>(Task.RegardingObjectId).Id;

                            if (entityName == Lead.EntityName) { return; }
                            else if (entityName == Loan.EntityName)
                            {
                                refreshRollUpFields(taskety, service, tracingService);
                                Entity loanObj = service.Retrieve(Loan.EntityName, regardingId, new ColumnSet(Loan.LOA2_PIP, Loan.LOA3_PIP));
                                EntityReference userRec = null;
                                //if (loanObj.Attributes.Contains(Loan.LOA2_PIP))
                                //{
                                //    userRec = loanObj.GetAttributeValue<EntityReference>(Loan.LOA2_PIP);
                                //    shareRecords(taskety, service, tracingService, userRec);
                                //}
                                //if (loanObj.Attributes.Contains(Loan.LOA3_PIP))
                                //{
                                //    userRec = loanObj.GetAttributeValue<EntityReference>(Loan.LOA3_PIP);
                                //    shareRecords(taskety, service, tracingService, userRec);
                                //}
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                tracingService.Trace(ex.Message);
            }
        }
        public void refreshRollUpFields(Entity taskRec, IOrganizationService context, ITracingService tracingservice)
        {
            tracingservice.Trace("Refreshing rollUp Fields now");
            try
            {
                Int32 highcount = 0;
                Int32 lowcount = 0;
                Int32 normalcount = 0;
                if (taskRec.Attributes.Contains(Task.RegardingObjectId))
                {
                    EntityReference loan = taskRec.GetAttributeValue<EntityReference>(Task.RegardingObjectId);

                    if (loan != null)
                    {
                        if (loan.LogicalName == Loan.EntityName)
                        {
                            Entity loanety = context.Retrieve(loan.LogicalName, loan.Id, new ColumnSet(Loan.HighPriorityCount_PIP, Loan.LowPriorityCount_PIP, Loan.NormalPriorityCount_PIP));

                            #region HighPriorityRollupFieldAutoRefresh
                            if (loanety.Attributes.Contains(Loan.HighPriorityCount_PIP))
                            {
                                highcount = Convert.ToInt32(loanety.Attributes[Loan.HighPriorityCount_PIP].ToString());
                                if (highcount != 0)
                                {
                                    highcount = Convert.ToInt32(loanety.Attributes[Loan.HighPriorityCount_PIP].ToString());
                                    tracingservice.Trace("The Old RollUp Field Value is" + " " + highcount);

                                }
                                CalculateRollupFieldRequest crfrRequested = new CalculateRollupFieldRequest
                                {
                                    Target = new EntityReference(loan.LogicalName, loan.Id),
                                    FieldName = Loan.HighPriorityCount_PIP
                                };
                                context.Execute(crfrRequested);
                            }
                            #endregion

                            #region NormalPriorityRollupFieldAutoRefresh

                            if (loanety.Attributes.Contains(Loan.NormalPriorityCount_PIP))
                            {
                                normalcount = Convert.ToInt32(loanety.Attributes[Loan.NormalPriorityCount_PIP].ToString());
                                if (normalcount != 0)
                                {
                                    normalcount = Convert.ToInt32(loanety.Attributes[Loan.NormalPriorityCount_PIP].ToString());
                                    tracingservice.Trace("The Old RollUp Field Value is" + " " + normalcount);
                                }
                                CalculateRollupFieldRequest crfrRequested = new CalculateRollupFieldRequest
                                {
                                    Target = new EntityReference(loan.LogicalName, loan.Id),
                                    FieldName = Loan.NormalPriorityCount_PIP
                                };
                                context.Execute(crfrRequested);
                            }
                            #endregion

                            #region LowPriorityRollupFieldAutoRefresh 
                            if (loanety.Attributes.Contains(Loan.LowPriorityCount_PIP))
                            {
                                lowcount = Convert.ToInt32(loanety.Attributes[Loan.LowPriorityCount_PIP].ToString());
                                if (lowcount != 0)
                                {
                                    lowcount = Convert.ToInt32(loanety.Attributes[Loan.LowPriorityCount_PIP].ToString());
                                    tracingservice.Trace("The Old RollUp Field Value is" + " " + lowcount);
                                }
                                CalculateRollupFieldRequest crfrRequested = new CalculateRollupFieldRequest
                                {
                                    Target = new EntityReference(loan.LogicalName, loan.Id),
                                    FieldName = Loan.LowPriorityCount_PIP
                                };

                                context.Execute(crfrRequested);
                            }
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                tracingservice.Trace(ex.Message);
            }

        }

        public void shareRecords(Entity taskRec, IOrganizationService context, ITracingService tracingService, EntityReference userRec)
        {
            /*  try
              {
                  GrantAccessRequest grant = new GrantAccessRequest();
                  grant.Target = new EntityReference(taskRec.LogicalName, taskRec.Id);
                  PrincipalAccess principal = new PrincipalAccess();
                  principal.Principal = new EntityReference(userRec.LogicalName, userRec.Id);
                  principal.AccessMask = AccessRights.ReadAccess | AccessRights.WriteAccess | AccessRights.AppendToAccess | AccessRights.AppendAccess;
                  grant.PrincipalAccess = principal;
                  GrantAccessResponse granted = (GrantAccessResponse)context.Execute(grant);
              }
              catch (Exception ex)
              {
                  tracingService.Trace(ex.Message);
              }*/
        }
    }
}

//public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
//{
//    yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "Create");
//    yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "Update");
//}