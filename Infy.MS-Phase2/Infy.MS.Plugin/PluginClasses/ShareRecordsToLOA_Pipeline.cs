using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xrm;
using Microsoft.Xrm.Sdk.Query;
using XRMExtensions;
using Microsoft.Crm.Sdk.Messages;
using Task = XRMExtensions.Task;

namespace Infy.MS.Plugins.PluginClasses
{
    public class LoanPrimaryLOA_Pipeline : BasePlugin
    {

        Entity Target = null;
        Entity preImage = null;
        Entity postImage = null;
        EntityReference userSys = null;
        EntityReference pre_primaryLOA = null;
        EntityReference pre_LOA2 = null;
        EntityReference pre_LOA3 = null;
        EntityReference primaryLOA = null;
        EntityReference LOA2 = null;
        EntityReference LOA3 = null;
        Common objCommon = new Common();
        EntityCollection ecMappings = null;
        string loanStatusName = string.Empty;
        public override void ExecutePlugin(IExtendedPluginContext context)
        {

            if (context.PreEntityImages.Contains("PreImage"))
            {
                preImage = (Entity)context.PreEntityImages["PreImage"];
            }
            if (context.PostEntityImages.Contains("PostImage"))
            {
                postImage = (Entity)context.PostEntityImages["PostImage"];
            }
            if (context == null)
            {
                throw new NotImplementedException();
            }
            if (context.MessageName.ToLower() == "update")
            {
                try
                {
                    context.Trace("Inside the Update context");

                    Target = context.GetTargetEntity<Entity>();
                    context.Trace("Target contains value");
                    Entity loanety = context.Retrieve(Target.LogicalName, Target.Id, new ColumnSet(Loan.Owner, Loan.PrimaryLOA_PIP, Loan.LOA2_PIP, Loan.LOA3_PIP, Loan.LoanStatus));
                    context.Trace("the loanety contains values");

                    if (preImage.Attributes.Contains(Loan.PrimaryLOA_PIP))
                    {
                        context.Trace("This contains pre Image Primary LOA");
                        pre_primaryLOA = preImage.GetAttributeValue<EntityReference>(Loan.PrimaryLOA_PIP);
                        context.Trace("pre image primary loa" + "  " + pre_primaryLOA.Name);
                    }
                    //if (preImage.Attributes.Contains(Loan.LOA2_PIP))
                    //{
                    //    context.Trace("This contains LOA2");
                    //    pre_LOA2 = preImage.GetAttributeValue<EntityReference>(Loan.LOA2_PIP);
                    //    context.Trace("pre image loa2" + "  " + pre_LOA2.Name);
                    //}
                    //if (preImage.Attributes.Contains(Loan.LOA3_PIP))
                    //{
                    //    context.Trace("This Contains LOA3");
                    //    pre_LOA3 = preImage.GetAttributeValue<EntityReference>(Loan.LOA3_PIP);
                    //    context.Trace("pre image loa3" + "  " + pre_LOA3.Name);
                    //}

                    if (loanety.Attributes.Contains(Loan.PrimaryLOA_PIP))
                    {
                        context.Trace("The current context contains Primary LOA");
                        primaryLOA = loanety.GetAttributeValue<EntityReference>(Loan.PrimaryLOA_PIP);
                        context.Trace("primary loa" + "  " + primaryLOA.Name);
                    }
                    //if (loanety.Attributes.Contains(Loan.LOA2_PIP))
                    //{
                    //    context.Trace("The current context contains   LOA2");
                    //    LOA2 = loanety.GetAttributeValue<EntityReference>(Loan.LOA2_PIP);
                    //    context.Trace("loa2" + "  " + LOA2.Name);
                    //}
                    //if (loanety.Attributes.Contains(Loan.LOA3_PIP))
                    //{
                    //    context.Trace("The current context contains   LOA3");
                    //    LOA3 = loanety.GetAttributeValue<EntityReference>(Loan.LOA3_PIP);
                    //    context.Trace("loa3" + "  " + LOA3.Name);
                    //}


                    //For Primay LOA
                    if (pre_primaryLOA != null)
                    {
                        if ((pre_primaryLOA != null) && (primaryLOA != null))
                        {
                            context.Trace("Checks for Null Validations for Primary LOA");
                            if (pre_primaryLOA.Name != primaryLOA.Name)
                            {
                                context.Trace("Inside primaryLOA if statement");
                                //getRelatedRecords(loanety, primaryLOA, context, "grantAccess");
                                //getRelatedRecords(loanety, pre_primaryLOA, context, "revokeAccess");

                                //Assign all the Automated Tasks to Primary LOA
                                getAllTask(loanety, primaryLOA, context, "assign");
                                //context.Trace("Removing the Access for the pre_primaryLOA");
                                //getAllTask(loanety, pre_primaryLOA, context, "revokeAccess");
                            }
                        }
                        else if ((pre_primaryLOA != null) && (primaryLOA == null))
                        {
                            context.Trace("After removing the Primary LOA Value here");
                            if (primaryLOA == null)
                            {
                                context.Trace("The Primary LOA value is NULL");
                                EntityReference loanownerid = loanety.GetAttributeValue<EntityReference>(Loan.Owner);
                                context.Trace("LoanOwnerId" + " " + loanownerid.Name);

                                //getRelatedRecords(loanety, pre_primaryLOA, context, "revokeAccess");
                                ///getRelatedRecords(loanety, loanownerid, context, "grantAccess");
                                getAllTask(loanety, loanownerid, context, "assign");
                                //getAllTask(loanety, pre_primaryLOA, context, "revokeAccess");
                            }
                        }
                    }
                    if ((pre_primaryLOA == null) && (primaryLOA != null))
                    {
                        EntityReference loanownerid = loanety.GetAttributeValue<EntityReference>(Loan.Owner);
                        context.Trace("PRe Image Primary LOA doesn't contain data");
                        //getRelatedRecords(loanety, primaryLOA, context, "grantAccess");
                        //getAllTask(loanety, loanownerid, context, "revokeAccess");
                        getAllTask(loanety, primaryLOA, context, "assign");
                    }

                    //For LOA2
                    /*  if (pre_LOA2 != null)
                      {
                          if ((pre_LOA2 != null) && (LOA2 != null))
                          {
                              context.Trace("Checks for Null Validations for LOA2");
                              if (pre_LOA2.Name != LOA2.Name)
                              {
                                  context.Trace("Inside LOA2 if statement");
                                  //getRelatedRecords(loanety, LOA2, context, "grantAccess");
                                  //getRelatedRecords(loanety, pre_LOA2, context, "revokeAccess");

                                  //Assign all the Automated Tasks to Primary LOA
                                  //getAllTask(loanety, LOA2, context, "grantAccess");
                                  //getAllTask(loanety, pre_LOA2, context, "revokeAccess");
                              }
                          }
                          else if ((pre_LOA2 != null) && (LOA2 == null))
                          {
                              //getRelatedRecords(loanety, pre_LOA2, context, "revokeAccess");
                              //getAllTask(loanety, pre_LOA2, context, "revokeAccess");
                          }
                      }
                      if ((pre_LOA2 == null) && (LOA2 != null))
                      {
                          context.Trace("PRe Image LOA2  doesn't contain data");
                          //getRelatedRecords(loanety, LOA2, context, "grantAccess");
                          //getAllTask(loanety, LOA2, context, "grantAccess");
                      }*/

                    //For LOA3
                    /*  if (pre_LOA3 != null)
                      {
                          if ((pre_LOA3 != null) && (LOA3 != null))
                          {
                              context.Trace("Checks for Null Validations for  LOA3");
                              if (pre_LOA3.Name != LOA3.Name)
                              {
                                  context.Trace("Inside LOA3 if statement");
                                  //getRelatedRecords(loanety, LOA3, context, "grantAccess");
                                  //getRelatedRecords(loanety, pre_LOA3, context, "revokeAccess");

                                  //Assign all the Automated Tasks to Primary LOA
                                  //getAllTask(loanety, LOA3, context, "grantAccess");
                                  //getAllTask(loanety, pre_LOA3, context, "revokeAccess");
                              }
                          }
                          else if ((pre_LOA3 != null) && (LOA3 == null))
                          {
                              //getRelatedRecords(loanety, pre_LOA3, context, "revokeAccess");
                              //getAllTask(loanety, pre_LOA3, context, "revokeAccess");
                          }
                      }
                      if ((pre_LOA3 == null) && (LOA3 != null))
                      {
                          context.Trace("PRe Image   LOA3 doesn't contain data");
                          //getRelatedRecords(loanety, LOA3, context, "grantAccess");
                          //getAllTask(loanety, LOA3, context, "grantAccess");
                      }*/

                    //if (((pre_LOA2 != null) && (LOA2 != null)) || ((pre_LOA2 == null) && (LOA2 != null)))
                    //{
                    //    context.Trace("Checks for Null Validations for   LOA2");
                    //    if (pre_LOA2.Name != LOA2.Name)
                    //    {
                    //        context.Trace("Inside LOA2 if statement");
                    //        getRelatedRecords(loanety, LOA2, context, "grantAccess");
                    //        getRelatedRecords(loanety, pre_LOA2, context, "revokeAccess");

                    //        //Share All Automated Tasks to LOA2,LOA3
                    //        getAllTask(loanety, LOA2, context, "grantAccess");
                    //        getAllTask(loanety, pre_LOA2, context, "revokeAccess");
                    //    }
                    //}
                    //if (((pre_LOA3 != null) && (LOA3 != null)) || ((pre_LOA3 == null) && (LOA3 != null)))
                    //{
                    //    context.Trace("Checks for Null Validations for   LOA3");
                    //    if (pre_LOA3.Name != LOA3.Name)
                    //    {
                    //        context.Trace("Inside LOA3 if statement");
                    //        getRelatedRecords(loanety, LOA3, context, "grantAccess");
                    //        getRelatedRecords(loanety, pre_LOA3, context, "revokeAccess");

                    //        //Share All Automated Tasks to LOA2,LOA3
                    //        getAllTask(loanety, LOA3, context, "grantAccess");
                    //        getAllTask(loanety, pre_LOA3, context, "revokeAccess");
                    //    }
                    //}
                }
                catch (Exception ex)
                {
                    context.Trace(ex.Message);
                }
            }

            if (context.MessageName.ToLower() == "create" || context.MessageName.ToLower() == "update")
            {
                Target = context.GetTargetEntity<Entity>();
                if (Target.Attributes.Contains(Loan.LoanStatus))
                {
                    context.Trace("Target contains Loan Status and inside Pizza Tracker Function");
                    updatePizzaTracker(context, Target);
                }
            }

        }

        public void getRelatedRecords(Entity loanety, EntityReference userRec, IExtendedPluginContext context, string grant)
        {
            /* try
             {
                 if (grant == "grantAccess")
                 {
                     //Assign all the record to the owner if there is no primary LOA Value here
                     context.Trace("For the own loan record here");

                     EntityReference loan = new EntityReference(Target.LogicalName, Target.Id);
                     context.Trace(loan.Id.ToString());
                     context.Trace(loan.LogicalName);
                     context.Trace(loan.Name);
                     grantaccess(loan, userRec, context);


                     //for Leads,Contacts,Accounts sharing records Here. with Primary LOA field Value here

                     if (loanety.Attributes.Contains(Loan.Attorney))
                     {
                         context.Trace("Grant Access for Attorney");
                         EntityReference attorney = loanety.GetAttributeValue<EntityReference>(Loan.Attorney);
                         grantaccess(attorney, userRec, context);
                     }
                     if (loanety.Attributes.Contains(Loan.Borrower))
                     {
                         context.Trace("Grant Access for Borrower");
                         EntityReference Borrower = loanety.GetAttributeValue<EntityReference>(Loan.Borrower);
                         grantaccess(Borrower, userRec, context);
                     }
                     if (loanety.Attributes.Contains(Loan.Co_Borrower))
                     {
                         context.Trace("Grant Access for Co_Borrower");
                         EntityReference Coborrower = loanety.GetAttributeValue<EntityReference>(Loan.Co_Borrower);
                         grantaccess(Coborrower, userRec, context);
                     }
                     if (loanety.Attributes.Contains(Loan.SettlementAgent))
                     {
                         context.Trace("Grant Access for SettlementAgent");
                         EntityReference SettlementAgent = loanety.GetAttributeValue<EntityReference>(Loan.SettlementAgent);
                         grantaccess(SettlementAgent, userRec, context);
                     }
                     if (loanety.Attributes.Contains(Loan.BuyersAgent))
                     {
                         context.Trace("Grant Access for BuyersAgent");
                         EntityReference BuyerAgent = loanety.GetAttributeValue<EntityReference>(Loan.BuyersAgent);
                         grantaccess(BuyerAgent, userRec, context);
                     }
                     if (loanety.Attributes.Contains(Loan.SellersAgent))
                     {
                         context.Trace("Grant Access for SellersAgent");
                         EntityReference SellerAgenet = loanety.GetAttributeValue<EntityReference>(Loan.SellersAgent);
                         grantaccess(SellerAgenet, userRec, context);
                     }

                 }
                 else if (grant == "revokeAccess")
                 {
                     //for The Loan Record here.
                     context.Trace("For the own record revoke access");
                     EntityReference loan = new EntityReference(Target.LogicalName, Target.Id);
                     revokeAccess(loan, userRec, context);

                     //for Leads,Contacts,Accounts sharing records Here.
                     if (loanety.Attributes.Contains(Loan.Attorney))
                     {
                         context.Trace("Revoke Access for Attorney");
                         EntityReference attorney = loanety.GetAttributeValue<EntityReference>(Loan.Attorney);
                         revokeAccess(attorney, userRec, context);
                     }
                     if (loanety.Attributes.Contains(Loan.Borrower))
                     {
                         context.Trace("Revoke Access for Borrower");
                         EntityReference Borrower = loanety.GetAttributeValue<EntityReference>(Loan.Borrower);
                         revokeAccess(Borrower, userRec, context);
                     }
                     if (loanety.Attributes.Contains(Loan.Co_Borrower))
                     {
                         context.Trace("Revoke Access for Co_Borrower");
                         EntityReference Coborrower = loanety.GetAttributeValue<EntityReference>(Loan.Co_Borrower);
                         revokeAccess(Coborrower, userRec, context);
                     }
                     if (loanety.Attributes.Contains(Loan.SettlementAgent))
                     {
                         context.Trace("Revoke Access for SettlementAgent");
                         EntityReference SettlementAgent = loanety.GetAttributeValue<EntityReference>(Loan.SettlementAgent);
                         revokeAccess(SettlementAgent, userRec, context);
                     }
                     if (loanety.Attributes.Contains(Loan.BuyersAgent))
                     {
                         context.Trace("Revoke Access for BuyersAgent");
                         EntityReference BuyerAgent = loanety.GetAttributeValue<EntityReference>(Loan.BuyersAgent);
                         revokeAccess(BuyerAgent, userRec, context);
                     }
                     if (loanety.Attributes.Contains(Loan.SellersAgent))
                     {
                         context.Trace("Revoke Access for SellersAgent");
                         EntityReference SellerAgenet = loanety.GetAttributeValue<EntityReference>(Loan.SellersAgent);
                         revokeAccess(SellerAgenet, userRec, context);
                     }
                 }
             }
             catch (Exception ex)
             {
                 context.Trace(ex.Message);
             }*/
        }
        public void getAllTask(Entity loanety, EntityReference userRec, IExtendedPluginContext context, string access)
        {
            try
            {
                context.Trace("Getting all Tasks for the Loan Records");
                string fetchxml = @"<fetch top='5000' >
                      <entity name='task' >
                        <attribute name='statecode' />
                        <attribute name='description' />
                        <attribute name='regardingobjectid' />
                        <attribute name='ims_automatedtask' />
                        <filter>
                          <condition attribute='regardingobjectid' operator='eq' value='" + loanety.Id + @" ' uiname='644081122' uitype='opportunity' />
                          <condition attribute='statecode' operator='eq' value='0' />
                          <condition attribute='ims_automatedtask' operator='eq' value='1'/>
                        </filter>
                      </entity>
                    </fetch>";

                //string Openfetchxml = @"<fetch top='5000' >
                //      <entity name='task' >
                //        <attribute name='statecode' />
                //        <attribute name='description' />
                //        <attribute name='regardingobjectid' />
                //        <attribute name='ims_automatedtask' />
                //        <filter>
                //          <condition attribute='regardingobjectid' operator='eq' value='" + loanety.Id + @" ' uiname='644081122' uitype='opportunity' />
                //          <condition attribute='statecode' operator='eq' value='0' />
                //          <condition attribute='ims_automatedtask' operator='eq' value='0'/>
                //        </filter>
                //      </entity>
                //    </fetch>";

                ////For Closed Task we sharing the records.
                //string closedTask =
                //     @"<fetch top='5000' >
                //      <entity name='task' >
                //        <attribute name='statecode' />
                //        <attribute name='description' />
                //        <attribute name='regardingobjectid' />
                //        <attribute name='ims_automatedtask' />
                //        <filter>
                //          <condition attribute='regardingobjectid' operator='eq' value='" + loanety.Id + @" ' uiname='644081122' uitype='opportunity' />
                //          <condition attribute='statecode' operator='eq' value='1' />
                //        </filter>
                //      </entity>
                //    </fetch>";

                //EntityCollection openCollection = context.RetrieveMultiple(new FetchExpression(Openfetchxml));
                //EntityCollection entityCollection = context.RetrieveMultiple(new FetchExpression(closedTask));
                EntityCollection etycollection = context.RetrieveMultiple(new FetchExpression(fetchxml));
                if (access == "assign")
                {
                    EntityReference loanOwnerId = loanety.GetAttributeValue<EntityReference>(Loan.Owner);

                    context.Trace("For Primary LOA alone assign the task ");
                    context.Trace("For Primary LOA we are sharing automated and open tasks here");
                    foreach (var ety in etycollection.Entities)
                    {
                        var assign = new AssignRequest
                        {
                            Assignee = new EntityReference(userRec.LogicalName, userRec.Id),
                            Target = new EntityReference(ety.LogicalName, ety.Id)
                        };
                        // Execute the Request
                        context.Execute(assign);
                    }
                    /*  if ((loanOwnerId != null && primaryLOA != null))
                      {
                          foreach (var entity in entityCollection.Entities)
                          {
                              context.Trace("For Primary LOA we are sharing Closed Tasks here both atuometed and manually");
                              GrantAccessRequest grant = new GrantAccessRequest();
                              grant.Target = new EntityReference(entity.LogicalName, entity.Id);

                              PrincipalAccess principal = new PrincipalAccess();
                              principal.Principal = new EntityReference(userRec.LogicalName, userRec.Id);
                              principal.AccessMask = AccessRights.ReadAccess | AccessRights.WriteAccess | AccessRights.AppendAccess | AccessRights.AppendToAccess;
                              grant.PrincipalAccess = principal;

                              GrantAccessResponse granted = (GrantAccessResponse)context.Execute(grant);
                          }
                          foreach (var entity in openCollection.Entities)
                          {
                              context.Trace("For Primary LOA we are sharing open and not automated tasks here");
                              GrantAccessRequest grant = new GrantAccessRequest();
                              grant.Target = new EntityReference(entity.LogicalName, entity.Id);

                              PrincipalAccess principal = new PrincipalAccess();
                              principal.Principal = new EntityReference(userRec.LogicalName, userRec.Id);
                              principal.AccessMask = AccessRights.ReadAccess | AccessRights.WriteAccess | AccessRights.AppendAccess | AccessRights.AppendToAccess;
                              grant.PrincipalAccess = principal;

                              GrantAccessResponse granted = (GrantAccessResponse)context.Execute(grant);
                          }
                      }*/
                }


                if (access == "grantAccess")
                {
                    /*  context.Trace("Granting access to all the Tasks");
                      //for open task grant access
                      foreach (var ety in etycollection.Entities)
                      {
                          GrantAccessRequest grant = new GrantAccessRequest();
                          grant.Target = new EntityReference(ety.LogicalName, ety.Id);

                          PrincipalAccess principal = new PrincipalAccess();
                          principal.Principal = new EntityReference(userRec.LogicalName, userRec.Id);
                          principal.AccessMask = AccessRights.ReadAccess | AccessRights.WriteAccess | AccessRights.AppendAccess | AccessRights.AppendToAccess;
                          grant.PrincipalAccess = principal;

                          GrantAccessResponse granted = (GrantAccessResponse)context.Execute(grant);
                      }
                      foreach (var entity in entityCollection.Entities)
                      {
                          GrantAccessRequest grant = new GrantAccessRequest();
                          grant.Target = new EntityReference(entity.LogicalName, entity.Id);

                          PrincipalAccess principal = new PrincipalAccess();
                          principal.Principal = new EntityReference(userRec.LogicalName, userRec.Id);
                          principal.AccessMask = AccessRights.ReadAccess | AccessRights.WriteAccess | AccessRights.AppendAccess | AccessRights.AppendToAccess;
                          grant.PrincipalAccess = principal;

                          GrantAccessResponse granted = (GrantAccessResponse)context.Execute(grant);
                      }
                      foreach (var entity in openCollection.Entities)
                      {
                          GrantAccessRequest grant = new GrantAccessRequest();
                          grant.Target = new EntityReference(entity.LogicalName, entity.Id);

                          PrincipalAccess principal = new PrincipalAccess();
                          principal.Principal = new EntityReference(userRec.LogicalName, userRec.Id);
                          principal.AccessMask = AccessRights.ReadAccess | AccessRights.WriteAccess | AccessRights.AppendAccess | AccessRights.AppendToAccess;
                          grant.PrincipalAccess = principal;

                          GrantAccessResponse granted = (GrantAccessResponse)context.Execute(grant);
                      }
                  }

                  if (access == "revokeAccess")
                  {
                      context.Trace("Revoking all the access for the Tasks");
                      //revoke access for open task
                      var systemUser2Ref = new EntityReference(userRec.LogicalName, userRec.Id);
                      foreach (var ety in etycollection.Entities)
                      {
                          var leadreference = new EntityReference(ety.LogicalName, ety.Id);
                          var revokeUserAccessReq = new RevokeAccessRequest
                          {
                              Revokee = systemUser2Ref,
                              Target = leadreference
                          };
                          context.Execute(revokeUserAccessReq);
                      }

                      foreach (var entity in entityCollection.Entities)
                      {
                          var leadreference = new EntityReference(entity.LogicalName, entity.Id);
                          var revokeUserAccessReq = new RevokeAccessRequest
                          {
                              Revokee = systemUser2Ref,
                              Target = leadreference
                          };
                          context.Execute(revokeUserAccessReq);
                      }
                      foreach (var entity in openCollection.Entities)
                      {
                          var leadreference = new EntityReference(entity.LogicalName, entity.Id);
                          var revokeUserAccessReq = new RevokeAccessRequest
                          {
                              Revokee = systemUser2Ref,
                              Target = leadreference
                          };
                          context.Execute(revokeUserAccessReq);
                      }*/
                }
            }
            catch (Exception ex)
            {
                context.Trace(ex.Message);
            }
        }
        public void grantaccess(EntityReference relatedRec, EntityReference userRec, IExtendedExecutionContext context)
        {

            /*  try
              {
                  GrantAccessRequest grant = new GrantAccessRequest();
                  grant.Target = new EntityReference(relatedRec.LogicalName, relatedRec.Id);
                  PrincipalAccess principal = new PrincipalAccess();
                  principal.Principal = new EntityReference(userRec.LogicalName, userRec.Id);
                  principal.AccessMask = AccessRights.ReadAccess | AccessRights.WriteAccess | AccessRights.AppendAccess | AccessRights.AppendToAccess;
                  grant.PrincipalAccess = principal;
                  GrantAccessResponse granted = (GrantAccessResponse)context.Execute(grant);
              }
              catch (Exception ex)
              {
                  context.Trace(ex.Message);
              }*/
        }
        public void revokeAccess(EntityReference relatedRec, EntityReference userRec, IExtendedExecutionContext context)
        {
            /* try
             {
                 EntityReference reference = new EntityReference(relatedRec.LogicalName, relatedRec.Id);
                 var systemUser2Ref = new EntityReference(userRec.LogicalName, userRec.Id);

                 var revokeUserAccessReq = new RevokeAccessRequest
                 {
                     Revokee = systemUser2Ref,
                     Target = reference
                 };
                 context.Execute(revokeUserAccessReq);
             }
             catch (Exception ex)
             {
                 context.Trace(ex.Message);
             }*/
        }

        #region PizzaTracker Update
        public void updatePizzaTracker(IExtendedPluginContext context, Entity loan)
        {
            context.Trace("Entering Pizza Tracker Function" + loan.Id);
            EntityReference loanStatus = null;
            Entity loanStatusName = null;
            Entity loanObj = context.Retrieve(Loan.EntityName, loan.Id, new ColumnSet(Loan.LoanStatus));
            if (loanObj.Attributes.Contains(Loan.LoanStatus))
            {
                loanStatus = loanObj.GetAttributeValue<EntityReference>(Loan.LoanStatus);
                context.Trace("The LoanStatus Value is" + " " + loanStatus.Name);
                context.Trace("The Loan GUid is" + " " + loan.Id.ToString());
                loanStatusName = context.Retrieve(LoanStatus.EntityName, loanStatus.Id, new ColumnSet(LoanStatus.PrimaryName));
            }


            if (loanStatusName != null && loanStatusName.Attributes.Contains(LoanStatus.PrimaryName))
            {
                context.Trace("Entered Pizza Tracker BPF Update region");
                var loanStatusUpdateOnBpf = loanStatusName.GetAttributeValue<string>(LoanStatus.PrimaryName);

                #region Query the Mortagage Loan Process entity and update the Loan id and the BPF Active Stage accordingly

                QueryExpression bpfQuery = new QueryExpression(PizzaTrackerProcess.EntityName);
                ColumnSet bpfCols = new ColumnSet(PizzaTrackerProcess.PrimaryKey);
                bpfQuery.ColumnSet = bpfCols;
                bpfQuery.Criteria.AddCondition(new ConditionExpression(PizzaTrackerProcess.Opportunity, ConditionOperator.Equal, loan.Id));

                EntityCollection bpfResult = context.RetrieveMultiple(bpfQuery);

                if (bpfResult.Entities.Count > 0)
                {
                    Guid bpfid = bpfResult.Entities[0].Id;
                    context.Trace("The Pizzza Tracker record exists already" + " " + bpfid.ToString());
                    foreach (Entity bpfRecord in bpfResult.Entities)
                    {
                        //Have to discuss on the design
                        if (bpfRecord.Attributes.Contains(PizzaTrackerProcess.PrimaryKey))
                        {
                            Guid bpfRecordId = (Guid)bpfRecord.Attributes[PizzaTrackerProcess.PrimaryKey];
                            context.Trace("The Pizzza Tracker record bpf GUID is" + " " + bpfRecordId.ToString());

                            Entity bpfRecordToUpdate = new Entity(PizzaTrackerProcess.EntityName);
                            bpfRecordToUpdate.Id = bpfRecordId;
                            bpfRecordToUpdate[PizzaTrackerProcess.Opportunity] = new EntityReference(Loan.EntityName, loan.Id);

                            if (loanStatusUpdateOnBpf.ToLower() == "application")
                            {
                                context.Trace("Inside Application");
                                Entity lonstatus = context.Retrieve(loanStatusName.LogicalName, loanStatusName.Id, new ColumnSet(LoanStatus.StageId));
                                string stageId = lonstatus.GetAttributeValue<string>(LoanStatus.StageId);
                                bpfRecordToUpdate[PizzaTrackerProcess.ActiveStage] = new EntityReference("processstage", new Guid(stageId));
                                context.SystemOrganizationService.Update(bpfRecordToUpdate);
                            }
                            else if (loanStatusUpdateOnBpf.ToLower() == "approved")
                            {
                                context.Trace("Inside approved");
                                Entity lonstatus = context.Retrieve(loanStatusName.LogicalName, loanStatusName.Id, new ColumnSet(LoanStatus.StageId));
                                string stageId = lonstatus.GetAttributeValue<string>(LoanStatus.StageId);
                                bpfRecordToUpdate[PizzaTrackerProcess.ActiveStage] = new EntityReference("processstage", new Guid(stageId));
                                context.SystemOrganizationService.Update(bpfRecordToUpdate);
                            }
                            else if (loanStatusUpdateOnBpf.ToLower() == "approved with conditions and appraisal approved" || loanStatusUpdateOnBpf.ToLower() == "approved with conditions" || loanStatusUpdateOnBpf.ToLower() == "approved with conditions and appraisal ordered" ||
                                loanStatusUpdateOnBpf.ToLower() == "brokered approved with conditions" || loanStatusUpdateOnBpf.ToLower() == "approved with conditions and appraisal received")
                            {
                                context.Trace("Inside approved with conditiosn");
                                Entity lonstatus = context.Retrieve(loanStatusName.LogicalName, loanStatusName.Id, new ColumnSet(LoanStatus.StageId));
                                string stageId = lonstatus.GetAttributeValue<string>(LoanStatus.StageId);
                                bpfRecordToUpdate[PizzaTrackerProcess.ActiveStage] = new EntityReference("processstage", new Guid(stageId));
                                context.SystemOrganizationService.Update(bpfRecordToUpdate);
                            }
                            else if (loanStatusUpdateOnBpf.ToLower() == "funded" || loanStatusUpdateOnBpf.ToLower() == "broker funded")
                            {
                                context.Trace("Inside funded");
                                Entity lonstatus = context.Retrieve(loanStatusName.LogicalName, loanStatusName.Id, new ColumnSet(LoanStatus.StageId));
                                string stageId = lonstatus.GetAttributeValue<string>(LoanStatus.StageId);
                                bpfRecordToUpdate[PizzaTrackerProcess.ActiveStage] = new EntityReference("processstage", new Guid(stageId));
                                context.SystemOrganizationService.Update(bpfRecordToUpdate);
                            }
                            else if (loanStatusUpdateOnBpf.ToLower() == "in closing")
                            {
                                context.Trace("Inside In Closing");
                                Entity lonstatus = context.Retrieve(loanStatusName.LogicalName, loanStatusName.Id, new ColumnSet(LoanStatus.StageId));
                                string stageId = lonstatus.GetAttributeValue<string>(LoanStatus.StageId);
                                bpfRecordToUpdate[PizzaTrackerProcess.ActiveStage] = new EntityReference("processstage", new Guid(stageId));
                                context.SystemOrganizationService.Update(bpfRecordToUpdate);
                            }
                            else if (loanStatusUpdateOnBpf.ToLower() == "lead")
                            {
                                context.Trace("Inside lead");
                                Entity lonstatus = context.Retrieve(loanStatusName.LogicalName, loanStatusName.Id, new ColumnSet(LoanStatus.StageId));
                                string stageId = lonstatus.GetAttributeValue<string>(LoanStatus.StageId);
                                bpfRecordToUpdate[PizzaTrackerProcess.ActiveStage] = new EntityReference("processstage", new Guid(stageId));
                                context.SystemOrganizationService.Update(bpfRecordToUpdate);
                            }
                            else if (loanStatusUpdateOnBpf.ToLower() == "submitted" || loanStatusUpdateOnBpf.ToLower() == "broker submitted" || loanStatusUpdateOnBpf.ToLower() == "prequal submitted" || loanStatusUpdateOnBpf.ToLower() == "submitted and appraisal approved" || loanStatusUpdateOnBpf.ToLower() == "submitted and appraisal ordered" || loanStatusUpdateOnBpf.ToLower() == "submitted and appraisal received")
                            {
                                context.Trace("Inside submitted");
                                Entity lonstatus = context.Retrieve(loanStatusName.LogicalName, loanStatusName.Id, new ColumnSet(LoanStatus.StageId));
                                string stageId = lonstatus.GetAttributeValue<string>(LoanStatus.StageId);
                                bpfRecordToUpdate[PizzaTrackerProcess.ActiveStage] = new EntityReference("processstage", new Guid(stageId));
                                context.SystemOrganizationService.Update(bpfRecordToUpdate);
                            }
                            else if (loanStatusUpdateOnBpf.ToLower() == "package sent" || loanStatusUpdateOnBpf.ToLower() == "wire ordered" || loanStatusUpdateOnBpf.ToLower() == "wire sent")
                            {
                                context.Trace("Inside pavkage sent");
                                Entity lonstatus = context.Retrieve(loanStatusName.LogicalName, loanStatusName.Id, new ColumnSet(LoanStatus.StageId));
                                string stageId = lonstatus.GetAttributeValue<string>(LoanStatus.StageId);
                                bpfRecordToUpdate[PizzaTrackerProcess.ActiveStage] = new EntityReference("processstage", new Guid(stageId));
                                context.SystemOrganizationService.Update(bpfRecordToUpdate);
                            }
                            updateLoanBPFRestrictToYes(context, loan.Id);
                            updateLoanBPFRestrictToNo(context, loan.Id);
                        }
                    }
                }
                else
                {
                    Entity bpfrecord = new Entity(PizzaTrackerProcess.EntityName);
                    bpfrecord[PizzaTrackerProcess.Opportunity] = new EntityReference(Loan.EntityName, loan.Id);

                    if (loanStatusUpdateOnBpf.ToLower() == "application")
                    {
                        Entity lonstatus = context.Retrieve(loanStatusName.LogicalName, loanStatusName.Id, new ColumnSet(LoanStatus.StageId));
                        string stageId = lonstatus.GetAttributeValue<string>(LoanStatus.StageId);
                        bpfrecord[PizzaTrackerProcess.ActiveStage] = new EntityReference("processstage", new Guid(stageId));
                        context.SystemOrganizationService.Create(bpfrecord);
                    }
                    else if (loanStatusUpdateOnBpf.ToLower() == "approved")
                    {
                        Entity lonstatus = context.Retrieve(loanStatusName.LogicalName, loanStatusName.Id, new ColumnSet(LoanStatus.StageId));
                        string stageId = lonstatus.GetAttributeValue<string>(LoanStatus.StageId);
                        bpfrecord[PizzaTrackerProcess.ActiveStage] = new EntityReference("processstage", new Guid(stageId));
                        context.SystemOrganizationService.Create(bpfrecord);
                    }
                    else if (loanStatusUpdateOnBpf.ToLower() == "approved with conditions and appraisal approved" || loanStatusUpdateOnBpf.ToLower() == "approved with conditions" || loanStatusUpdateOnBpf.ToLower() == "approved with conditions and appraisal ordered" ||
                        loanStatusUpdateOnBpf.ToLower() == "brokered approved with conditions" || loanStatusUpdateOnBpf.ToLower() == "approved with conditions and appraisal received")
                    {
                        Entity lonstatus = context.Retrieve(loanStatusName.LogicalName, loanStatusName.Id, new ColumnSet(LoanStatus.StageId));
                        string stageId = lonstatus.GetAttributeValue<string>(LoanStatus.StageId);
                        bpfrecord[PizzaTrackerProcess.ActiveStage] = new EntityReference("processstage", new Guid(stageId));
                        context.SystemOrganizationService.Create(bpfrecord);
                    }
                    else if (loanStatusUpdateOnBpf.ToLower() == "funded" || loanStatusUpdateOnBpf.ToLower() == "broker funded")
                    {
                        Entity lonstatus = context.Retrieve(loanStatusName.LogicalName, loanStatusName.Id, new ColumnSet(LoanStatus.StageId));
                        string stageId = lonstatus.GetAttributeValue<string>(LoanStatus.StageId);
                        bpfrecord[PizzaTrackerProcess.ActiveStage] = new EntityReference("processstage", new Guid(stageId));
                        context.SystemOrganizationService.Create(bpfrecord);

                    }
                    else if (loanStatusUpdateOnBpf.ToLower() == "in closing")
                    {
                        Entity lonstatus = context.Retrieve(loanStatusName.LogicalName, loanStatusName.Id, new ColumnSet(LoanStatus.StageId));
                        string stageId = lonstatus.GetAttributeValue<string>(LoanStatus.StageId);
                        bpfrecord[PizzaTrackerProcess.ActiveStage] = new EntityReference("processstage", new Guid(stageId));
                        context.SystemOrganizationService.Create(bpfrecord);
                    }
                    else if (loanStatusUpdateOnBpf.ToLower() == "lead")
                    {
                        Entity lonstatus = context.Retrieve(loanStatusName.LogicalName, loanStatusName.Id, new ColumnSet(LoanStatus.StageId));
                        string stageId = lonstatus.GetAttributeValue<string>(LoanStatus.StageId);
                        bpfrecord[PizzaTrackerProcess.ActiveStage] = new EntityReference("processstage", new Guid(stageId));
                        context.SystemOrganizationService.Create(bpfrecord);
                    }
                    else if (loanStatusUpdateOnBpf.ToLower() == "submitted" || loanStatusUpdateOnBpf.ToLower() == "broker submitted" || loanStatusUpdateOnBpf.ToLower() == "prequal submitted" || loanStatusUpdateOnBpf.ToLower() == "submitted and appraisal approved" || loanStatusUpdateOnBpf.ToLower() == "submitted and appraisal ordered" || loanStatusUpdateOnBpf.ToLower() == "submitted and appraisal received")
                    {
                        Entity lonstatus = context.Retrieve(loanStatusName.LogicalName, loanStatusName.Id, new ColumnSet(LoanStatus.StageId));
                        string stageId = lonstatus.GetAttributeValue<string>(LoanStatus.StageId);
                        bpfrecord[PizzaTrackerProcess.ActiveStage] = new EntityReference("processstage", new Guid(stageId));
                        context.SystemOrganizationService.Create(bpfrecord);
                    }
                    else if (loanStatusUpdateOnBpf.ToLower() == "package sent" || loanStatusUpdateOnBpf.ToLower() == "wire ordered" || loanStatusUpdateOnBpf.ToLower() == "wire sent")
                    {
                        Entity lonstatus = context.Retrieve(loanStatusName.LogicalName, loanStatusName.Id, new ColumnSet(LoanStatus.StageId));
                        string stageId = lonstatus.GetAttributeValue<string>(LoanStatus.StageId);
                        bpfrecord[PizzaTrackerProcess.ActiveStage] = new EntityReference("processstage", new Guid(stageId));
                        context.SystemOrganizationService.Create(bpfrecord);
                    }
                    updateLoanBPFRestrictToYes(context, loan.Id);
                    updateLoanBPFRestrictToNo(context, loan.Id);
                }
                #endregion
            }

        }
        private void updateLoanBPFRestrictToYes(IExtendedPluginContext context, Guid loanId)
        {
            Entity loanToUpdate = new Entity(Loan.EntityName);
            loanToUpdate[Loan.RestrictBPFMovement] = true;
            loanToUpdate.Id = loanId;
            context.Update(loanToUpdate);
        }
        private void updateLoanBPFRestrictToNo(IExtendedPluginContext context, Guid loanId)
        {
            Entity loanToUpdate = new Entity(Loan.EntityName);
            loanToUpdate[Loan.RestrictBPFMovement] = false;
            loanToUpdate.Id = loanId;
            context.Update(loanToUpdate);
        }
        #endregion
        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "Create");
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "Update");
        }
    }
}