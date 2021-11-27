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
    public class AutomatedTask_Pipeline : BasePlugin
    {
        Entity Target = null;
        Entity preImage = null;
        EntityReference userSys = null;
        Common objCommon = new Common();
        EntityCollection ecMappings = null;
        string loanStatusName = string.Empty;
        EntityReference primaryLOA_User = null;
        EntityReference loa2 = null;
        EntityReference loa3 = null;
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            if (context.PreEntityImages.Contains("PreImage"))
            {
                preImage = (Entity)context.PreEntityImages["PreImage"];
            }
            if (context == null)
            {
                throw new NotImplementedException();
            }
            if (context.MessageName.ToLower() == "update")
            {
                Target = context.GetTargetEntity<Entity>();
                #region Auto Creation of Task on Update of Loan Status and User Reference whether auto should be Created or Not
                autoTaskCreateOnUpdateOfLoanStatus(context, Target);
                #endregion
            }
            if (context.MessageName.ToLower() == "create")
            {
                Target = context.GetTargetEntity<Entity>();
                autoTaskCreateOnCreateOfLoan(context, Target);
            }
        }
        public void autoTaskCreateOnUpdateOfLoanStatus(IExtendedExecutionContext context, Entity Target)
        {
            int appraisalResults = 0;
            int pre_appraisalResults = 0;
            context.Trace("Auto Task Creation Begins Here");

            Entity loanety = context.Retrieve(Target.LogicalName, Target.Id, new ColumnSet(Loan.Owner, Loan.PrimaryLOA_PIP, Loan.LOA2_PIP, Loan.LOA3_PIP, Loan.PIW_PIP, Loan.AppraisalResults_PIP, Loan.Attorney, Loan.BuyersAgent, Loan.SettlementAgent, Loan.SellersAgent, Loan.Borrower, Loan.Co_Borrower, Loan.LoanStatus, Loan.ClosingDate, Loan.AppraisalExpectedDate, Loan.CDSentDate, Loan.LoanNumber));
            #region Create Tasks Based on Loan status
            if (loanety.Attributes.Contains(Loan.LoanStatus))
            {
                context.Trace("The Loan Contains Loan Status");
                string piw = loanety.GetAttributeValue<bool>(Loan.PIW_PIP).ToString();
                context.Trace("The PIW value is" + " " + piw);
                EntityReference loanStatus = loanety.GetAttributeValue<EntityReference>(Loan.LoanStatus);
                loanStatusName = loanStatus.Name;
                context.Trace("The Status of the Loan is" + " " + loanStatusName);

                #region Update Application Status Date, Submitted Status Date, Funded Status Date based on Loan Status
                Entity updateLoan = new Entity(Loan.EntityName, loanety.Id);
                if (loanStatusName == "Application")
                {
                    updateLoan[Loan.ApplicationStatusDate_PIP] = DateTime.UtcNow;
                    context.Update(updateLoan);
                }
                else if (loanStatusName == "Submitted")
                {
                    updateLoan[Loan.submittedStatusDate_PIP] = DateTime.UtcNow;
                    context.Update(updateLoan);
                }
                else if (loanStatusName == "Funded")
                {
                    updateLoan[Loan.fundedstatusDate_PIP] = DateTime.UtcNow;
                    context.Update(updateLoan);
                }
                #endregion



                #region if the Loan Officer  is Team Cancel all the Automatted Tasks
                userSys = loanety.GetAttributeValue<EntityReference>(Loan.Owner);
                if (userSys.LogicalName == Team.EntityName)
                {
                    string getAutomatedTasks = @"<fetch top='5000' >
              <entity name='task' >
                <attribute name='subject' />
                <attribute name='description' />
                <attribute name='ims_automatedtask' />
                <filter>
                  <condition attribute='ims_automatedtask' operator='eq' value='1' />
                  <condition attribute='regardingobjectid' operator='eq' value='" + loanety.Id + @"' />
                </filter>
              </entity>
            </fetch>";

                    EntityCollection autoamtedTask = context.RetrieveMultiple(new FetchExpression(getAutomatedTasks));
                    foreach (var tasks in autoamtedTask.Entities)
                    {
                        Entity taskety = new Entity(Task.EntityName, tasks.Id);
                        taskety[Task.StateCode] = new OptionSetValue(2);
                        context.Update(taskety);
                        context.Trace("update sucessful");
                    }
                }
                #endregion

                #region Create Task for loan only if loan officer is a User
                if (userSys.LogicalName == SystemUser.EntityName)
                {
                    Entity systemUser = context.Retrieve(userSys.LogicalName, userSys.Id, new ColumnSet(SystemUser.IsAutomationRequired));

                    //If Automation Required to yes Create Automatic Tasks
                    if (systemUser.Attributes.Contains(SystemUser.IsAutomationRequired))
                    {
                        bool isautomated = systemUser.GetAttributeValue<bool>(SystemUser.IsAutomationRequired);
                        context.Trace("Task Automation is set to Yes for the User " + " " + isautomated);
                        if (isautomated == true)
                        {
                            string preImage_loanStatus = string.Empty;
                            if (preImage.Attributes.Contains(Loan.LoanStatus))
                            {
                                preImage_loanStatus = preImage.GetAttributeValue<EntityReference>(Loan.LoanStatus).Name;
                            }
                            context.Trace("Automated is set to True and Fetch the Import Details Mappings and Creating the Task Record Here");

                            #region Create Task when loan status is Application and alos based on PIW value.
                            if (loanStatusName.ToLower() == "application")
                            {
                                if (piw == "False")
                                {
                                    if (loanStatusName != preImage_loanStatus)
                                    {
                                        string TaskName = "Order Appraisal";
                                        ecMappings = fetchXml(TaskName, context);
                                        FetchImportDetailsMappings(ecMappings, context, loanety);
                                    }
                                }
                            }
                            if (loanStatusName.ToLower() == "application")
                            {
                                if (loanStatusName != preImage_loanStatus)
                                {
                                    ecMappings = fetchXml("Docs Out Follow-up", context);
                                    FetchImportDetailsMappings(ecMappings, context, loanety);
                                }
                            }
                            #endregion

                            #region Create Tasks when the Loan status is equl to Appraisal ORdered
                            if (loanStatusName.ToLower() == "appraisal ordered")
                            {
                                if (piw == "False" || piw == null)
                                {
                                    context.Trace(piw);
                                    if (loanStatusName != preImage_loanStatus)
                                    {
                                        ecMappings = fetchXml("Check on Update on Appraisal", context);
                                        FetchImportDetailsMappings(ecMappings, context, loanety);
                                        context.Trace(loanety.Attributes.Contains(Loan.AppraisalExpectedDate).ToString());
                                        if (!loanety.Attributes.Contains(Loan.AppraisalExpectedDate))
                                        {
                                            ecMappings = fetchXml("Appraisal Follow-up", context);
                                            FetchImportDetailsMappings(ecMappings, context, loanety);
                                        }
                                    }
                                }
                            }
                            #endregion

                            #region Create Tasks when the Loan status is Approved With Condtitions
                            if (loanStatusName.ToLower() == "approved with conditions")
                            {
                                if (loanStatusName != preImage_loanStatus)
                                {
                                    ecMappings = fetchXml("Request HOI Declarations", context);
                                    FetchImportDetailsMappings(ecMappings, context, loanety);
                                    ecMappings = fetchXml("Send Remaining Conditions", context);
                                    FetchImportDetailsMappings(ecMappings, context, loanety);
                                }
                            }
                            #endregion
                            #region Create Tasks when the loan status is Approved
                            if (loanStatusName.ToLower() == "approved")
                            {
                                if (loanety.Attributes.Contains(Loan.ClosingDate))
                                {
                                    string taskfetchxml = @"<fetch top='50' >
                                                  <entity name='task' >
                                                    <attribute name='statecode' />
                                                    <attribute name='ims_automatedtaskname' />
                                                    <attribute name='description' />
                                                    <attribute name='activityid' />
                                                    <attribute name='scheduledend' />
                                                    <attribute name='actualdurationminutes' />
                                                    <attribute name='subject' />
                                                    <filter>
                                                      <condition attribute='regardingobjectid' operator='eq' value='" + loanety.Id + @"' uiname='8429356' uitype='opportunity' />
                                                      <condition attribute='subject' operator='like' value='%Instructions out%' />
                                                      <condition attribute='statecode' operator='neq' value='1' />
                                                    </filter>
                                                  </entity>
                                                </fetch>";

                                    EntityCollection ec = context.RetrieveMultiple(new FetchExpression(taskfetchxml));
                                    if (ec.Entities.Count > 0 && ec != null)
                                    {
                                        foreach (var task in ec.Entities)
                                        {
                                            if (task.Attributes.Contains(Task.ScheduledEnd))
                                            {
                                                Entity taskety = new Entity(Task.EntityName, task.Id);
                                                DateTime closingDate = checkBusinessclosuer_ClosingDate(context, "-1", loanety.GetAttributeValue<DateTime>(Loan.ClosingDate));

                                                DateTime today = DateTime.UtcNow.Date;
                                                int d = (closingDate.Date - today).Days;
                                                var dateAndTime1 = DateTime.UtcNow.AddDays(d);
                                                //int? getTimeZoneCode = RetrieveCurrentUsersSettings(context.SystemOrganizationService,context);
                                                //DateTime localDateTime = RetrieveLocalTimeFromUTCTime(dateAndTime1, getTimeZoneCode, context.SystemOrganizationService,context);
                                                // context.Trace(localDateTime.ToString());
                                                taskety[Task.ScheduledEnd] = dateAndTime1;// closingDate.AddHours(12).AddMinutes(00); // loanety.GetAttributeValue<DateTime>(Loan.ClosingDate);
                                                context.Update(taskety);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (loanety.Attributes.Contains(Loan.ClosingDate))
                                        {
                                            if (loanety.GetAttributeValue<DateTime>(Loan.ClosingDate) != null)
                                            {
                                                ecMappings = fetchXml("Confirm with Closer", context);
                                                FetchImportDetailsMappings(ecMappings, context, loanety);
                                            }
                                        }
                                    }
                                }
                            }
                            #endregion

                            #region Create Tasks when the Loan status is Package Sent
                            if (loanStatusName.ToLower() == "package sent")
                            {
                                if (loanStatusName != preImage_loanStatus)
                                {
                                    ecMappings = fetchXml("Send final numbers", context);
                                    FetchImportDetailsMappings(ecMappings, context, loanety);
                                }
                            }
                            #endregion

                            #region Create tasks based on the corresponding APpraisal Results field Value here and PIW field Values.
                            if (loanStatusName.ToLower() != "funded")
                            {
                                if (loanStatusName.ToLower() != "dead lead /loan")
                                {
                                    if (loanety.Attributes.Contains(Loan.AppraisalResults_PIP))
                                    {
                                        if (piw == "False")
                                        {
                                            context.Trace("The Loan has Appraisal Results Value");
                                            appraisalResults = loanety.GetAttributeValue<OptionSetValue>(Loan.AppraisalResults_PIP).Value;
                                            context.Trace("Appraisal results Value is" + " " + appraisalResults.ToString());
                                            if (preImage.Attributes.Contains(Loan.AppraisalResults_PIP))
                                            {
                                                pre_appraisalResults = preImage.GetAttributeValue<OptionSetValue>(Loan.AppraisalResults_PIP).Value;
                                                context.Trace("The PreAppraisal results value is" + " " + pre_appraisalResults.ToString());
                                            }
                                            if (appraisalResults != 0)
                                            {
                                                context.Trace("Appraisal results contains data");
                                                if (appraisalResults != pre_appraisalResults)
                                                {
                                                    context.Trace("The pre appraisalresults value is" + " " + pre_appraisalResults.ToString());
                                                    string results = loanety.FormattedValues[Loan.AppraisalResults_PIP];
                                                    if (results == "Undervalued")
                                                    {
                                                        context.Trace(results);
                                                        ecMappings = fetchXml("Undervalued", context);
                                                        FetchImportDetailsMappings(ecMappings, context, loanety);
                                                    }
                                                    else if (results == "As Is")
                                                    {
                                                        context.Trace(results);
                                                        ecMappings = fetchXml("As Is", context);
                                                        FetchImportDetailsMappings(ecMappings, context, loanety);
                                                    }
                                                    else if (results == "Subject To")
                                                    {
                                                        context.Trace(results);
                                                        ecMappings = fetchXml("Subject To", context);
                                                        FetchImportDetailsMappings(ecMappings, context, loanety);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    #region Cancel the incomplete/open Tasks when the Appraisal Results is Set to NULL and PreImage Appraisal Results Contains Data.
                                    else if (!loanety.Attributes.Contains(Loan.AppraisalResults_PIP))
                                    {
                                        context.Trace("APpraisalResults does not contains any data");
                                        if (piw == "False")
                                        {
                                            if (preImage.Attributes.Contains(Loan.AppraisalResults_PIP))
                                            {
                                                pre_appraisalResults = preImage.GetAttributeValue<OptionSetValue>(Loan.AppraisalResults_PIP).Value;
                                                context.Trace("The PreAppraisal results value is" + " " + pre_appraisalResults.ToString());
                                            }
                                            if ((appraisalResults == 0) && (pre_appraisalResults != 0))
                                            {
                                                context.Trace("When the Preimage contains data and appraisal results value is empty");
                                                string aprraisalResultTask = @"<fetch top='50' >
                                              <entity name='task' >
                                                <attribute name='subject' />
                                                <attribute name='description' />
                                                <filter>
                                                  <condition attribute='subject' operator='like' value='%Auto-Follow-up on Appraisal Results-%' />
                                                  <condition attribute='regardingobjectid' operator='eq' value='" + loanety.Id + @"' />
                                                  <condition attribute='statecode' operator='eq' value='0' />
                                                </filter>
                                              </entity>
                                            </fetch>";

                                                EntityCollection getTasks = context.RetrieveMultiple(new FetchExpression(aprraisalResultTask));
                                                foreach (var task in getTasks.Entities)
                                                {
                                                    Entity taskety = new Entity(Task.EntityName, task.Id);
                                                    taskety[Task.StateCode] = new OptionSetValue(2);
                                                    context.Update(taskety);
                                                    context.Trace("update sucessful");
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                }
                            }
                            #endregion

                            #region Create Tasks when Loan record is populated with CDSentDate
                            if (loanStatusName.ToLower() != "funded")
                            {
                                if (loanStatusName.ToLower() != "dead lead /loan")
                                {
                                    if (loanety.Attributes.Contains(Loan.CDSentDate))
                                    {
                                        context.Trace("CDsentDate contains data");
                                        if (preImage.Attributes.Contains(Loan.CDSentDate))
                                        {
                                            context.Trace("preimage cdsent date doesbt contain data");
                                            if (loanety.GetAttributeValue<DateTime>(Loan.CDSentDate) != preImage.GetAttributeValue<DateTime>(Loan.CDSentDate))
                                            {
                                                context.Trace("pre and current cdsent date are not equal");
                                                ecMappings = fetchXml("Initial CD Follow-up", context);
                                                FetchImportDetailsMappings(ecMappings, context, loanety);
                                            }
                                        }
                                        else
                                        {
                                            context.Trace("there is no pre image value for cd sent date");
                                            ecMappings = fetchXml("Initial CD Follow-up", context);
                                            FetchImportDetailsMappings(ecMappings, context, loanety);
                                        }
                                    }
                                }
                            }
                            #endregion

                            #region here close the incompleted/pending for these task, Order Appraisal task, Check on Update on Appraisal, Appraisal Follow-up, Follow-up on Appraisal Results when PIW is set to TRUE.
                            if (piw == "True")
                            {
                                context.Trace("PIW Value is" + " " + piw);
                                context.Trace("the Fetch XMl executes below to update the open tasks here");
                                string fetchTask = @"<fetch top='50' >
                                      <entity name='task' >
                                        <attribute name='prioritycode' />
                                        <attribute name='statecode' />
                                        <attribute name='description' />
                                        <attribute name='statuscode' />
                                        <attribute name='ims_automatedtask' />
                                        <attribute name='subject' />
                                        <filter type='or' >
                                          <condition attribute='subject' operator='like' value='%Order Appraisal%' />
                                          <condition attribute='subject' operator='like' value='%Check on Update on Appraisal%' />
                                          <condition attribute='subject' operator='like' value='%Appraisal Follow-up%' />
                                          <condition attribute='subject' operator='like' value='%Follow-up on Appraisal Results%' />
                                        </filter>
                                        <filter>
                                          <condition attribute='regardingobjectid' operator='eq' value='" + loanety.Id + @"' />
                                          <condition attribute='statecode' operator='neq' value='1' />
                                        </filter>
                                      </entity>
                                    </fetch>";
                                EntityCollection tasks = context.RetrieveMultiple(new FetchExpression(fetchTask));
                                foreach (var task in tasks.Entities)
                                {
                                    Entity taskety = new Entity(Task.EntityName, task.Id);
                                    taskety[Task.StateCode] = new OptionSetValue(2);
                                    context.Update(taskety);
                                    context.Trace("update sucessful");
                                }
                            }
                            #endregion

                            #region Cancel tasks if loan status is "Dead Lead /Loan"
                            if (loanStatusName.ToLower() == "dead lead /loan")
                            {
                                context.Trace("PIP2.0-Loan Status Value is" + " " + loanStatusName.ToLower());
                                context.Trace("PIP2.0-the Fetch XMl executes below to update the open tasks here");
                                string getAutomatedTasks = @"<fetch top='5000' >
                                                              <entity name='task' >
                                                                <attribute name='subject' />
                                                                <attribute name='description' />
                                                                <attribute name='ims_automatedtask' />
                                                                <filter>
                                                                  <condition attribute='ims_automatedtask' operator='eq' value='1' />
                                                                  <condition attribute='statecode' operator='eq' value='0' />
                                                                 <condition attribute='regardingobjectid' operator='eq' value='" + loanety.Id + @"' />
                                                                </filter>
                                                              </entity>
                                                            </fetch>";
                                EntityCollection tasks = context.RetrieveMultiple(new FetchExpression(getAutomatedTasks));
                                foreach (var task in tasks.Entities)
                                {
                                    Entity taskety = new Entity(Task.EntityName, task.Id);
                                    taskety[Task.StateCode] = new OptionSetValue(2);
                                    context.Update(taskety);
                                    context.Trace("PIP2.0 -update sucessful");
                                }
                            }
                            #endregion
                        }
                    }
                }
            }
            #endregion
            #endregion
        }
        public void autoTaskCreateOnCreateOfLoan(IExtendedExecutionContext context, Entity Target)
        {
            int appraisalResults = 0;
            context.Trace("Auto Task Creation Begins Here");

            Entity loanety = context.Retrieve(Target.LogicalName, Target.Id, new ColumnSet(Loan.Owner, Loan.PrimaryLOA_PIP, Loan.LOA2_PIP, Loan.LOA3_PIP, Loan.PIW_PIP, Loan.AppraisalResults_PIP, Loan.Attorney, Loan.BuyersAgent, Loan.SettlementAgent, Loan.SellersAgent, Loan.Borrower, Loan.Co_Borrower, Loan.LoanStatus, Loan.ClosingDate, Loan.AppraisalExpectedDate, Loan.CDSentDate, Loan.LoanNumber));
            #region Create Tasks Based on Loan Status
            if (loanety.Attributes.Contains(Loan.LoanStatus))
            {
                context.Trace("The Loan Contains Loan Status");
                string piw = loanety.GetAttributeValue<bool>(Loan.PIW_PIP).ToString();
                context.Trace("The PIW value is" + " " + piw);
                EntityReference loanStatus = loanety.GetAttributeValue<EntityReference>(Loan.LoanStatus);
                loanStatusName = loanStatus.Name;
                context.Trace("The Status of the Loan is" + " " + loanStatusName);

                #region Update Application Status Date,Submitted Status Date,Funded Status Date based on Loan Status
                Entity updateLoan = new Entity(Loan.EntityName, loanety.Id);
                if (loanStatusName == "Application")
                {
                    updateLoan[Loan.ApplicationStatusDate_PIP] = DateTime.UtcNow;
                    context.Update(updateLoan);
                }
                else if (loanStatusName == "Submitted")
                {
                    updateLoan[Loan.submittedStatusDate_PIP] = DateTime.UtcNow;
                    context.Update(updateLoan);
                }
                else if (loanStatusName == "Funded")
                {
                    updateLoan[Loan.fundedstatusDate_PIP] = DateTime.UtcNow;
                    context.Update(updateLoan);
                }
                #endregion

                #region if the Loan Officer  is Team Cancel all the Automatted Tasks
                userSys = loanety.GetAttributeValue<EntityReference>(Loan.Owner);
                if (userSys.LogicalName == Team.EntityName)
                {
                    string getAutomatedTasks = @"<fetch top='5000' >
              <entity name='task' >
                <attribute name='subject' />
                <attribute name='description' />
                <attribute name='ims_automatedtask' />
                <filter>
                  <condition attribute='ims_automatedtask' operator='eq' value='1' />
                  <condition attribute='regardingobjectid' operator='eq' value='" + loanety.Id + @"' />
                </filter>
              </entity>
            </fetch>";

                    EntityCollection autoamtedTask = context.RetrieveMultiple(new FetchExpression(getAutomatedTasks));
                    foreach (var tasks in autoamtedTask.Entities)
                    {
                        Entity taskety = new Entity(Task.EntityName, tasks.Id);
                        taskety[Task.StateCode] = new OptionSetValue(2);
                        context.Update(taskety);
                        context.Trace("update sucessful");
                    }
                }
                #endregion

                #region Create Task for loan only if loan officer is a User
                if (userSys.LogicalName == SystemUser.EntityName)
                {
                    Entity systemUser = context.Retrieve(userSys.LogicalName, userSys.Id, new ColumnSet(SystemUser.IsAutomationRequired));
                    #region Create Tasks only if User Automation is to Yes.
                    if (systemUser.Attributes.Contains(SystemUser.IsAutomationRequired))
                    {
                        bool isautomated = systemUser.GetAttributeValue<bool>(SystemUser.IsAutomationRequired);
                        context.Trace("Task Automation is set to Yes for the User " + " " + isautomated);
                        if (isautomated == true)
                        {
                            context.Trace("Automated is set to True and Fetch the Import Details Mappings and Creating the Task Record Here");

                            #region Create Task when loan status is Application and alos based on PIW value.
                            if (loanStatusName.ToLower() == "application")
                            {
                                if (piw == "False")
                                {
                                    string TaskName = "Order Appraisal";
                                    ecMappings = fetchXml(TaskName, context);
                                    FetchImportDetailsMappings(ecMappings, context, loanety);
                                }
                            }
                            if (loanStatusName.ToLower() == "application")
                            {
                                ecMappings = fetchXml("Docs Out Follow-up", context);
                                FetchImportDetailsMappings(ecMappings, context, loanety);
                            }
                            #endregion

                            #region Create task when the loan status is equal to Appraisal Ordered
                            if (loanStatusName.ToLower() == "appraisal ordered")
                            {
                                if (piw == "False" || piw == null)
                                {
                                    context.Trace(piw);
                                    ecMappings = fetchXml("Check on Update on Appraisal", context);
                                    FetchImportDetailsMappings(ecMappings, context, loanety);
                                    context.Trace(loanety.Attributes.Contains(Loan.AppraisalExpectedDate).ToString());
                                    if (!loanety.Attributes.Contains(Loan.AppraisalExpectedDate))
                                    {
                                        ecMappings = fetchXml("Appraisal Follow-up", context);
                                        FetchImportDetailsMappings(ecMappings, context, loanety);
                                    }
                                }
                            }
                            #endregion

                            #region Create task when the loan status is equal to Approved with conditions
                            if (loanStatusName.ToLower() == "approved with conditions")
                            {
                                ecMappings = fetchXml("Request HOI Declarations", context);
                                FetchImportDetailsMappings(ecMappings, context, loanety);
                                ecMappings = fetchXml("Send Remaining Conditions", context);
                                FetchImportDetailsMappings(ecMappings, context, loanety);
                            }
                            #endregion

                            #region Create  task when the loan status is Equal To Approved.
                            if (loanStatusName.ToLower() == "approved")
                            {
                                if (loanety.Attributes.Contains(Loan.ClosingDate))
                                {
                                    string taskfetchxml = @"<fetch top='50' >
                                                  <entity name='task' >
                                                    <attribute name='statecode' />
                                                    <attribute name='ims_automatedtaskname' />
                                                    <attribute name='description' />
                                                    <attribute name='activityid' />
                                                    <attribute name='scheduledend' />
                                                    <attribute name='actualdurationminutes' />
                                                    <attribute name='subject' />
                                                    <filter>
                                                      <condition attribute='regardingobjectid' operator='eq' value='" + loanety.Id + @"' uiname='8429356' uitype='opportunity' />
                                                      <condition attribute='subject' operator='like' value='%instructions out%' />
                                                      <condition attribute='statecode' operator='neq' value='1' />
                                                    </filter>
                                                  </entity>
                                                </fetch>";

                                    EntityCollection ec = context.RetrieveMultiple(new FetchExpression(taskfetchxml));
                                    if (ec.Entities.Count > 0 && ec != null)
                                    {
                                        foreach (var task in ec.Entities)
                                        {
                                            if (task.Attributes.Contains(Task.ScheduledEnd))
                                            {
                                                Entity taskety = new Entity(Task.EntityName, task.Id);
                                                DateTime closingDate = checkBusinessclosuer_ClosingDate(context, "-1", loanety.GetAttributeValue<DateTime>(Loan.ClosingDate));

                                                DateTime today = DateTime.UtcNow.Date;
                                                int d = (closingDate.Date - today).Days;
                                                var dateAndTime1 = DateTime.UtcNow.AddDays(d);
                                                taskety[Task.ScheduledEnd] = dateAndTime1;
                                                context.Update(taskety);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (loanety.Attributes.Contains(Loan.ClosingDate))
                                        {
                                            if (loanety.GetAttributeValue<DateTime>(Loan.ClosingDate) != null)
                                            {
                                                ecMappings = fetchXml("Confirm with Closer", context);
                                                FetchImportDetailsMappings(ecMappings, context, loanety);
                                            }
                                        }
                                    }
                                }
                            }
                            #endregion

                            #region Create task when Loan Status is Equal to Package Sent
                            if (loanStatusName.ToLower() == "package sent")
                            {
                                ecMappings = fetchXml("Send final numbers", context);
                                FetchImportDetailsMappings(ecMappings, context, loanety);
                            }
                            #endregion

                            #region When Appraisal results is populated then create tasks for the corresponding appraisal results.
                            if (loanStatusName.ToLower() != "funded")
                            {
                                if (loanStatusName.ToLower() != "dead lead /loan")
                                {
                                    if (loanety.Attributes.Contains(Loan.AppraisalResults_PIP))
                                    {
                                        if (piw == "False")
                                        {
                                            context.Trace("The Loan has Appraisal Results Value");
                                            appraisalResults = loanety.GetAttributeValue<OptionSetValue>(Loan.AppraisalResults_PIP).Value;

                                            if (appraisalResults != 0)
                                            {
                                                context.Trace("Appraisal results contains data");

                                                string results = loanety.FormattedValues[Loan.AppraisalResults_PIP];
                                                if (results == "Undervalued")
                                                {
                                                    context.Trace(results);
                                                    ecMappings = fetchXml("Undervalued", context);
                                                    FetchImportDetailsMappings(ecMappings, context, loanety);
                                                }
                                                else if (results == "As Is")
                                                {
                                                    context.Trace(results);
                                                    ecMappings = fetchXml("As Is", context);
                                                    FetchImportDetailsMappings(ecMappings, context, loanety);
                                                }
                                                else if (results == "Subject To")
                                                {
                                                    context.Trace(results);
                                                    ecMappings = fetchXml("Subject To", context);
                                                    FetchImportDetailsMappings(ecMappings, context, loanety);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            #endregion

                            #region When CDSent Date Contains Data then Create Initial CD Follow-up Task.

                            if (loanStatusName.ToLower() != "dead lead /loan")
                            {
                                context.Trace("LoanStatus not equal to dead lead /loan" + loanStatusName);
                                if (loanStatusName.ToLower() != "funded")
                                {
                                    context.Trace("LoanStatus not equal to funded" + loanStatusName);
                                    if (loanety.Attributes.Contains(Loan.CDSentDate))
                                    {
                                        context.Trace("CDsentDate contains data");
                                        ecMappings = fetchXml("Initial CD Follow-up", context);
                                        FetchImportDetailsMappings(ecMappings, context, loanety);
                                    }
                                }
                            }

                            #endregion

                            #region Here close the incompleted/pending for these task, Order Appraisal task, Check on Update on Appraisal, Appraisal Follow-up, Follow-up on Appraisal Results)
                            if (piw == "True")
                            {
                                context.Trace("PIW Value is" + " " + piw);
                                context.Trace("the Fetch XMl executes below to update the open tasks here");

                                string fetchTask = @"<fetch top='50' >
                                      <entity name='task' >
                                        <attribute name='prioritycode' />
                                        <attribute name='statecode' />
                                        <attribute name='description' />
                                        <attribute name='statuscode' />
                                        <attribute name='ims_automatedtask' />
                                        <attribute name='subject' />
                                        <filter type='or' >
                                          <condition attribute='subject' operator='like' value='%Order Appraisal%' />
                                          <condition attribute='subject' operator='like' value='%Check on Update on Appraisal%' />
                                          <condition attribute='subject' operator='like' value='%Appraisal Follow-up%' />
                                          <condition attribute='subject' operator='like' value='%Follow-up on Appraisal Results%' />
                                        </filter>
                                        <filter>
                                          <condition attribute='regardingobjectid' operator='eq' value='" + loanety.Id + @"' />
                                          <condition attribute='statecode' operator='neq' value='1' />
                                        </filter>
                                      </entity>
                                    </fetch>";
                                EntityCollection tasks = context.RetrieveMultiple(new FetchExpression(fetchTask));

                                foreach (var task in tasks.Entities)
                                {
                                    Entity taskety = new Entity(Task.EntityName, task.Id);
                                    taskety[Task.StateCode] = new OptionSetValue(2);
                                    context.Update(taskety);
                                    context.Trace("update sucessful");
                                }
                            }
                            #endregion
                        }
                    }
                    #endregion
                }
                #endregion
            }
            #endregion
        }
        public static DateTime checkBusinessClosures(IExtendedExecutionContext context, string datetimeVal, DateTime? closingDate)
        {
            //Check Business closure Dates and WeekEnd Dates here.
            QueryExpression calenderQuery = new QueryExpression
            {
                EntityName = "calendar",
                ColumnSet = new ColumnSet(true),
                Criteria =
                                       {
                                           Conditions =
                                           {
                                               new ConditionExpression("name", ConditionOperator.Equal, "Business Closure Calendar")
                                           }
                                       }
            };

            EntityCollection calendar = context.RetrieveMultiple(calenderQuery);

            DateTime requiredDateTime = DateTime.UtcNow;
            context.Trace("Inside Business closure", requiredDateTime.ToString());
            /*if (closingDate != null)
            {
                context.Trace("The Loan Status is APproved and the closing date contains value is " + " " + closingDate.ToString());
                DateTime closeDate = Convert.ToDateTime(closingDate);
                int daysToAdd1 = int.Parse(datetimeVal);
                if (daysToAdd1 < 0)
                {
                    while (daysToAdd1 < 0)
                    {
                        closeDate = closeDate.AddDays(-1);
                        if (isWorkingDay(calendar, closeDate))
                        {
                            daysToAdd1++;
                        }
                    }
                }

                context.Trace("The CloseDate value is" + " " + closeDate.ToString());
                return closeDate;
            }*/
            //else
            //{
            context.Trace("Inside Business closure function" + " " + datetimeVal);
            int daysToAdd = int.Parse(datetimeVal);
            //if (daysToAdd < 0)
            //{
            //    while (daysToAdd < 0)
            //    {
            //        requiredDateTime = requiredDateTime.AddDays(-1);
            //        if (isWorkingDay(calendar, requiredDateTime))
            //        {
            //            daysToAdd++;
            //        }
            //    }
            //}
            //else 
            if (daysToAdd > 0)
            {
                while (daysToAdd > 0)
                {
                    requiredDateTime = requiredDateTime.AddDays(1);
                    if (isWorkingDay(calendar, requiredDateTime))
                    {
                        daysToAdd--;
                    }
                }
            }
            context.Trace("The Required Datetime is" + " " + requiredDateTime);
            return requiredDateTime;
            //}
        }
        public void FetchImportDetailsMappings(EntityCollection ecMappings, IExtendedExecutionContext context, Entity loanety)
        {
            context.Trace("Inside FetchImportDetailsMappings");

            context.Trace("The Import Details Mappings Count Record is" + ecMappings.Entities.Count.ToString());

            List<Common.Mapping> mappings = new List<Common.Mapping>();
            if (ecMappings != null && ecMappings.Entities.Count > 0)
            {
                foreach (Entity objMapping in ecMappings.Entities)
                {
                    mappings.Add(new Common.Mapping
                    {
                        Source = objMapping.GetAttributeValue<string>(ImportDetailsMapping.SourceField),
                        Target = objMapping.GetAttributeValue<string>(ImportDetailsMapping.TargetField),
                        TargetEntity = objMapping.GetAttributeValue<string>(ImportDetailsMapping.PrimaryName),
                        DataType = objMapping.Contains(ImportDetailsMapping.TargetDataType) ? objMapping.GetAttributeValue<OptionSetValue>
                            (ImportDetailsMapping.TargetDataType).Value : (int)ImportDetailsMapping.TargetDataType_OptionSet.SingleLineOfText,
                        SourceDatatype = objMapping.Contains(ImportDetailsMapping.SourceDataType) ? objMapping.GetAttributeValue<OptionSetValue>
                            (ImportDetailsMapping.SourceDataType).Value : (int)ImportDetailsMapping.SourceDataType_OptionSet.SingleLineOfText,
                        LookupEntityAttribute = objMapping.GetAttributeValue<string>(ImportDetailsMapping.LookupentityAttributeFilterCondition),
                        LookupEntityName = objMapping.GetAttributeValue<string>(ImportDetailsMapping.LookupEntityName),
                        Mandatory = objMapping.Contains(ImportDetailsMapping.IsDataMandatoryforallrecords) ?
                            objMapping.GetAttributeValue<bool>(ImportDetailsMapping.IsDataMandatoryforallrecords) : false,
                        CrmDisplayName = objMapping.GetAttributeValue<string>(ImportDetailsMapping.CrmDisplayName),
                        MaxLengthAllowed = objMapping.Contains(ImportDetailsMapping.MaximumLengthAllowed) ?
                            objMapping.GetAttributeValue<int>(ImportDetailsMapping.MaximumLengthAllowed) : 0,
                        CreatelookupRecord = objMapping.Contains(ImportDetailsMapping.CreateLookupRecordUnresolved) ?
                            objMapping.GetAttributeValue<bool>(ImportDetailsMapping.CreateLookupRecordUnresolved) : false,
                        DataMaster = objMapping.GetAttributeValue<string>(ImportDetailsMapping.DataMasterName),
                        DefaultValue = objMapping.GetAttributeValue<string>(ImportDetailsMapping.DefaultValue),
                        RevertResult = objMapping.Contains(ImportDetailsMapping.RevertResult) ? objMapping.GetAttributeValue<bool>(ImportDetailsMapping.RevertResult) : false
                    });
                }
                Entity taskObject = new Entity(Task.EntityName);
                foreach (Common.Mapping objMapping in mappings)
                {
                    try
                    {
                        Common.Mapping mapping = objMapping;
                        if (loanety.Attributes.Contains(Loan.PrimaryLOA_PIP))
                        {
                            primaryLOA_User = loanety.GetAttributeValue<EntityReference>(Loan.PrimaryLOA_PIP);
                            GetValueFromSourceEntity(mapping, taskObject, context, primaryLOA_User, loanety);
                        }
                        else if (loanety.Attributes.Contains(Loan.Owner))
                        {
                            GetValueFromSourceEntity(mapping, taskObject, context, userSys, loanety);
                        }
                    }
                    catch (Exception ex)
                    {
                        context.Trace(ex.Message);
                    }
                }
                if (taskObject != null && taskObject.Attributes.Count > 0)
                {
                    bool isDuplicateTask = true;
                    context.Trace("Creating the Auto Task Record here");
                    if (taskObject.Attributes.Contains(Task.RegardingObjectId))
                    {
                        Guid loanId = taskObject.GetAttributeValue<EntityReference>(Task.RegardingObjectId).Id;
                        string taskSub = taskObject.GetAttributeValue<string>(Task.Subject);
                        isDuplicateTask = checkIfDuplicateTaskExist(context, loanId, taskSub);
                    }
                    if (isDuplicateTask == true)
                    {
                        return;
                    }
                    else if (isDuplicateTask == false)
                    {
                        context.Create(taskObject);
                    }
                }
            }
        }
        public EntityCollection fetchXml(string taskName, IExtendedExecutionContext context)
        {

            string fetchImportDetailsMapping = @"<fetch top='50' >
              <entity name='ims_importdetailsmapping' >
                <attribute name='owningteam' />
                <attribute name='statecode' />
                <attribute name='owneridname' />
                <attribute name='statuscode' />
                <attribute name='statecodename' />
                <attribute name='owninguser' />
                <attribute name='overriddencreatedon' />
                <attribute name='ims_revertresult' />
                <attribute name='ims_sourceentity' />
                <attribute name='ims_importdetailsmappingid' />
                <attribute name='ims_name' />
                <attribute name='ims_targetdatatypename' />
                <attribute name='ims_targetfield' />
                <attribute name='ims_importdatamaster' />
                <attribute name='ims_crmdisplayname' />
                <attribute name='ims_mandatory' />
                <attribute name='owneridtype' />
                <attribute name='statuscodename' />
                <attribute name='ims_lookupentityattribute' />
                <attribute name='ims_defaultvalue' />
                <attribute name='ims_revertresultname' />
                <attribute name='ims_datamastername' />
                <attribute name='owneridyominame' />
                <attribute name='ims_targetdatatype' />
                <attribute name='ims_mandatoryname' />
                <attribute name='ims_lookupentityname' />
                <attribute name='owningbusinessunit' />
                <attribute name='ims_maxlength' />
                <attribute name='ims_sourcedatatype' />
                <attribute name='ims_sourcedatatypename' />
                <attribute name='ims_sourcefield' />
                <attribute name='ownerid' />
                <attribute name='ims_createlookuprecord' />
                <attribute name='ims_importdatamastername' />
                <filter>
                  <condition attribute='ims_datamastername' operator='like' value='%" + taskName + @"%' />
                                            </filter>
                                          </entity>
                                        </fetch>";
            EntityCollection ecMappings = context.RetrieveMultiple(new FetchExpression(fetchImportDetailsMapping));

            return ecMappings;


        }
        public void GetValueFromSourceEntity(Common.Mapping mappings, Entity taskObject, IExtendedExecutionContext context, EntityReference userRec, Entity loanEntity)
        {
            context.Trace("Inside GetValueFromSourceEntity");
            string owner = null;
            try
            {
                if (mappings.Source != null)
                {
                    if (mappings.SourceDatatype == (int)ImportDetailsMapping.SourceDataType_OptionSet.Lookup)
                    {
                        owner = GetValueFromSourceLookUpField(mappings.Source, context, Target);
                        mappings.value = owner;
                        string id = GetId(mappings.LookupEntityName, mappings.LookupEntityAttribute, mappings, context);
                        taskObject[mappings.Target] = new EntityReference(mappings.LookupEntityName, new Guid(id));
                    }
                    else if (mappings.DataType == (int)ImportDetailsMapping.TargetDataType_OptionSet.SingleLineOfText)
                    {
                        string singlelinetxt = GetValueFromSingleLineText(mappings.Source, context);
                        taskObject[mappings.Target] = singlelinetxt;
                    }
                    else if (mappings.DataType == (int)ImportDetailsMapping.TargetDataType_OptionSet.Optonset)
                    {
                        int val = FetchOptionSetLookupValue(mappings, int.Parse(mappings.Source), context);
                        taskObject[mappings.Target] = new OptionSetValue(val);
                    }
                    else if (mappings.SourceDatatype == (int)ImportDetailsMapping.SourceDataType_OptionSet.DateTime)
                    {
                        DateTime dateVal = GetValueFromDateTime(mappings, mappings.Source, context, Target);
                        taskObject[mappings.Target] = dateVal;
                    }
                    else if (mappings.SourceDatatype == (int)ImportDetailsMapping.SourceDataType_OptionSet.WholeNumber)
                    {
                        int wholeVal = GetValueFromWholenumber(mappings.Source, context, Target);
                        taskObject[mappings.Target] = wholeVal;
                    }
                }
                else if (mappings.Source == null)
                {
                    if (mappings.DataType == (int)ImportDetailsMapping.TargetDataType_OptionSet.Optonset)
                    {
                        int val = FetchOptionSetLookupValue(mappings, int.Parse(mappings.DefaultValue), context);
                        taskObject[mappings.Target] = new OptionSetValue(val);
                    }
                    else if (mappings.DataType == (int)ImportDetailsMapping.TargetDataType_OptionSet.SingleLineOfText)
                    {
                        //mappings.value = mappings.DefaultValue;
                        string firstName = null;
                        string loanNumber = string.Empty;
                        string lastName = null;
                        EntityReference borrowerEty = null;
                        Entity borrowerVal = null;
                        if (loanEntity.Attributes.Contains(Loan.LoanNumber))
                        {
                            loanNumber = loanEntity.GetAttributeValue<string>(Loan.LoanNumber);
                        }
                        if (loanEntity.Attributes.Contains(Loan.Borrower))
                        {
                            borrowerEty = loanEntity.GetAttributeValue<EntityReference>(Loan.Borrower);
                            borrowerVal = context.Retrieve(borrowerEty.LogicalName, borrowerEty.Id, new ColumnSet(Lead.FirstName, Lead.LastName));
                            if (borrowerVal.Attributes.Contains(Lead.FirstName))
                            {
                                firstName = borrowerVal.GetAttributeValue<string>(Lead.FirstName);
                            }
                            if (borrowerVal.Attributes.Contains(Lead.LastName))
                            {
                                lastName = borrowerVal.GetAttributeValue<string>(Lead.LastName);
                            }
                        }

                        mappings.value = mappings.DefaultValue;
                        if (mappings.DefaultValue.Contains("{0}"))
                        {
                            if (firstName != null && lastName != null)
                            {
                                mappings.value = mappings.DefaultValue.Replace("{0}", "-" + " " + firstName + " " + lastName + " " + "(" + loanNumber + ")");
                            }
                            if (firstName == null)
                            {
                                mappings.value = mappings.DefaultValue.Replace("{0}", "-" + " " + lastName + " " + "(" + loanNumber + ")");
                                taskObject[mappings.Target] = mappings.value;
                            }
                            if (lastName == null)
                            {
                                mappings.value = mappings.DefaultValue.Replace("{0}", "-" + " " + firstName + " " + "(" + loanNumber + ")");
                                taskObject[mappings.Target] = mappings.value;
                            }
                        }
                        taskObject[mappings.Target] = mappings.value;
                    }
                    else if (mappings.DataType == (int)ImportDetailsMapping.TargetDataType_OptionSet.Lookup)
                    {
                        owner = GetValueFromSourceLookUpField(mappings.Target, context, Target);
                        mappings.value = owner;
                        string id = GetId(mappings.LookupEntityName, mappings.LookupEntityAttribute, mappings, context);
                        taskObject[mappings.Target] = new EntityReference(mappings.LookupEntityName, new Guid(id));
                    }
                    else if (mappings.DataType == (int)ImportDetailsMapping.SourceDataType_OptionSet.DateTime)
                    {
                        DateTime? dateAndTime1 = null;
                        mappings.value = mappings.DefaultValue;

                        context.Trace("Inside Approved and default value" + " " + mappings.value);
                        if (loanEntity.GetAttributeValue<EntityReference>(Loan.LoanStatus).Name.ToLower() == "approved")
                        {
                            if (context.MessageName.ToLower() == "create")
                            {
                                DateTime? dateVal = null;
                                context.Trace("The Loan status values is approvedand inside create context");
                                if (Target.Attributes.Contains(Loan.ClosingDate))
                                {
                                    DateTime? closingDate = Target.GetAttributeValue<DateTime>(Loan.ClosingDate);
                                    context.Trace("The CLosing Date values is" + closingDate.ToString());
                                    if (mappings.value != "0")
                                    {
                                        dateVal = checkBusinessclosuer_ClosingDate(context, mappings.value, closingDate);
                                    }
                                    else if (mappings.value == "0")
                                    {
                                        dateVal = checkBusinessClosures(context, mappings.value, null);
                                    }
                                    DateTime date = Convert.ToDateTime(dateVal);
                                    DateTime today = DateTime.UtcNow.Date;
                                    int d = (date.Date - today).Days;
                                    dateAndTime1 = DateTime.UtcNow.AddDays(d);
                                    //dateVal = dateVal.AddHours(12).AddMinutes(00);
                                    context.Trace(mappings.value);
                                    mappings.value = dateAndTime1.ToString();
                                }
                            }
                            if (context.MessageName.ToLower() == "update")
                            {
                                if (loanEntity.GetAttributeValue<EntityReference>(Loan.LoanStatus).Name.ToLower() == "approved")
                                {
                                    context.Trace("The Loan status values is not approved");
                                    if (Target.Attributes.Contains(Loan.ClosingDate))
                                    {
                                        DateTime? dateVal = null;
                                        context.Trace("Issue is here only");
                                        DateTime closingDate = Target.GetAttributeValue<DateTime>(Loan.ClosingDate);
                                        context.Trace("The CLosing Date values is" + closingDate.ToString());
                                        if (mappings.value != "0")
                                        {
                                            context.Trace("The Confirm with confirm default value is" + mappings.value);
                                            dateVal = checkBusinessclosuer_ClosingDate(context, mappings.value, closingDate);
                                        }
                                        else if (mappings.value == "0")
                                        {
                                            context.Trace("The CD Signed Task Value is" + mappings.value);
                                            dateVal = checkBusinessClosures(context, mappings.value, null);
                                        }
                                        //DateTime dateVal = checkBusinessclosuer_ClosingDate(context, mappings.value, closingDate);
                                        DateTime today = DateTime.UtcNow.Date;
                                        DateTime date = Convert.ToDateTime(dateVal);
                                        int d = (date.Date - today).Days;
                                        dateAndTime1 = DateTime.UtcNow.AddDays(d);
                                        //int? getTimeZoneCode = RetrieveCurrentUsersSettings(context.SystemOrganizationService,context);
                                        //DateTime localDateTime = RetrieveLocalTimeFromUTCTime(dateAndTime1, getTimeZoneCode, context.SystemOrganizationService,context);
                                        context.Trace(mappings.value.ToString());
                                        mappings.value = dateAndTime1.ToString();
                                    }
                                    //
                                    else
                                    {
                                        context.Trace("The Loan Status is Approved and Context doesn;t contains ClosingDate");
                                        if (mappings.value == "0")
                                        {
                                            context.Trace(mappings.value);
                                            DateTime dateVal = checkBusinessClosures(context, mappings.value, null);
                                            DateTime today = DateTime.UtcNow.Date;
                                            int d = (dateVal.Date - today).Days;
                                            dateAndTime1 = DateTime.UtcNow.AddDays(d);
                                            //int? getTimeZoneCode = RetrieveCurrentUsersSettings(context.SystemOrganizationService,context);
                                            //DateTime localDateTime = RetrieveLocalTimeFromUTCTime(dateAndTime1, getTimeZoneCode, context.SystemOrganizationService,context);
                                            //context.Trace("UtcTimeNow" + " " + localDateTime.ToString());
                                            //dateVal = dateVal.AddHours(0).AddMinutes(00);
                                            mappings.value = dateAndTime1.ToString();
                                        }
                                        else
                                        {
                                            context.Trace(mappings.value);
                                            DateTime dateVal = checkBusinessClosures(context, mappings.value, null);
                                            DateTime today = DateTime.UtcNow.Date;
                                            int d = (dateVal.Date - today).Days;
                                            dateAndTime1 = DateTime.UtcNow.AddDays(d);
                                            //int? getTimeZoneCode = RetrieveCurrentUsersSettings(context.SystemOrganizationService,context);
                                            //DateTime localDateTime = RetrieveLocalTimeFromUTCTime(dateAndTime1, getTimeZoneCode, context.SystemOrganizationService,context);
                                            //context.Trace("UtcTimeNow" + " " + localDateTime.ToString());
                                            //dateVal = dateVal.AddHours(0).AddMinutes(00);
                                            context.Trace(mappings.value);
                                            mappings.value = dateAndTime1.ToString();
                                        }
                                    }
                                }
                                else if (loanEntity.GetAttributeValue<EntityReference>(Loan.LoanStatus).Name.ToLower() != "approved")
                                {
                                    if (mappings.value == "0")
                                    {
                                        context.Trace("Loan Status is Approved and creating tasks for appraisal results here=1");
                                        DateTime dateVal = checkBusinessClosures(context, mappings.value, null);
                                        DateTime today = DateTime.UtcNow.Date;
                                        int d = (dateVal.Date - today).Days;
                                        dateAndTime1 = DateTime.UtcNow.AddDays(d);
                                        //int? getTimeZoneCode = RetrieveCurrentUsersSettings(context.SystemOrganizationService,context);
                                        //DateTime localDateTime = RetrieveLocalTimeFromUTCTime(dateAndTime1, getTimeZoneCode, context.SystemOrganizationService,context);
                                        //context.Trace("UtcTimeNow" + " " + localDateTime.ToString());
                                        //dateVal = dateVal.AddHours(0).AddMinutes(00);
                                        mappings.value = dateAndTime1.ToString();
                                    }
                                    else
                                    {
                                        context.Trace("Loan Status is Approved and creating tasks for appraisal results here=3");
                                        DateTime dateVal = checkBusinessClosures(context, mappings.value, null);
                                        DateTime today = DateTime.UtcNow.Date;
                                        int d = (dateVal.Date - today).Days;
                                        dateAndTime1 = DateTime.UtcNow.AddDays(d);
                                        //int? getTimeZoneCode = RetrieveCurrentUsersSettings(context.SystemOrganizationService,context);
                                        //DateTime localDateTime = RetrieveLocalTimeFromUTCTime(dateAndTime1, getTimeZoneCode, context.SystemOrganizationService,context);
                                        //context.Trace("UtcTimeNow" + " " + localDateTime.ToString());
                                        //dateVal = dateVal.AddHours(0).AddMinutes(00);
                                        context.Trace(mappings.value);
                                        mappings.value = dateAndTime1.ToString();
                                    }
                                }

                                //}
                            }
                            /*else
                            {
                                if (mappings.value == "0")
                                {
                                    DateTime dateVal = checkBusinessClosures(context, mappings.value, null);
                                    dateVal = dateVal.AddHours(5).AddMinutes(30);
                                    mappings.value = dateVal.ToString();
                                }
                                else
                                {
                                    DateTime dateVal = checkBusinessClosures(context, mappings.value, null);
                                    dateVal = dateVal.AddHours(5).AddMinutes(30);
                                    context.Trace(mappings.value);
                                    mappings.value = dateVal.ToString();
                                }
                            }*/
                        }
                        else
                        {
                            if (mappings.value == "0")
                            {
                                DateTime dateVal = checkBusinessClosures(context, mappings.value, null);
                                DateTime today = DateTime.UtcNow.Date;
                                int d = (dateVal.Date - today).Days;
                                dateAndTime1 = DateTime.UtcNow.AddDays(d);
                                //int? getTimeZoneCode = RetrieveCurrentUsersSettings(context.SystemOrganizationService,context);
                                //DateTime localDateTime = RetrieveLocalTimeFromUTCTime(dateAndTime1, getTimeZoneCode, context.SystemOrganizationService,context);
                                //context.Trace("UtcTimeNow" + " " + localDateTime.ToString());
                                //dateVal = dateVal.AddHours(0).AddMinutes(00);
                                mappings.value = dateAndTime1.ToString();
                            }
                            else
                            {
                                DateTime dateVal = checkBusinessClosures(context, mappings.value, null);
                                DateTime today = DateTime.UtcNow.Date;
                                int d = (dateVal.Date - today).Days;
                                dateAndTime1 = DateTime.UtcNow.AddDays(d);
                                //int? getTimeZoneCode = RetrieveCurrentUsersSettings(context.SystemOrganizationService,context);
                                //DateTime localDateTime = RetrieveLocalTimeFromUTCTime(dateAndTime1, getTimeZoneCode, context.SystemOrganizationService,context);
                                //context.Trace("UtcTimeNow" + " " + localDateTime.ToString());
                                //dateVal = dateVal.AddHours(0).AddMinutes(00);
                                context.Trace(mappings.value);
                                mappings.value = dateAndTime1.ToString();
                            }
                            context.Trace(mappings.value);
                        }

                        /* if ((context.MessageName.ToLower() == "update") && (Target.Attributes.Contains(Loan.AppraisalResults_PIP)))
                         {
                             context.Trace("Context Message is Update" + " " + Target.GetAttributeValue<OptionSetValue>(Loan.AppraisalResults_PIP).Value.ToString());
                             if (mappings.value == "0")
                             {
                                 DateTime dateVal = checkBusinessClosures(context, mappings.value, null);
                                 dateVal = dateVal.AddHours(5).AddMinutes(30);
                                 mappings.value = dateVal.ToString();
                                 taskObject[mappings.Target] = Convert.ToDateTime(mappings.value);
                             }
                             else
                             {
                                 DateTime dateVal = checkBusinessClosures(context, mappings.value, null);
                                 dateVal = dateVal.AddHours(5).AddMinutes(30);
                                 context.Trace(mappings.value);
                                 mappings.value = dateVal.ToString();
                                 taskObject[mappings.Target] = Convert.ToDateTime(mappings.value);
                             }
                             context.Trace("AppraisalResults Test");
                         }
                         if (Target.Attributes.Contains(Loan.CDSentDate))
                         {
                             context.Trace("Context Message is either Create/Update" + " " + Target.GetAttributeValue<DateTime>(Loan.CDSentDate).ToString());
                             if (mappings.value == "0")
                             {
                                 DateTime dateVal = checkBusinessClosures(context, mappings.value, null);
                                 dateVal = dateVal.AddHours(5).AddMinutes(30);
                                 mappings.value = dateVal.ToString();
                                 taskObject[mappings.Target] = Convert.ToDateTime(mappings.value);
                             }
                             else
                             {
                                 DateTime dateVal = checkBusinessClosures(context, mappings.value, null);
                                 dateVal = dateVal.AddHours(5).AddMinutes(30);
                                 context.Trace(mappings.value);
                                 mappings.value = dateVal.ToString();
                                 taskObject[mappings.Target] = Convert.ToDateTime(mappings.value);
                             }
                             context.Trace("TestCDSentDate");
                         }*/

                        taskObject[mappings.Target] = dateAndTime1;// Convert.ToDateTime(mappings.value);
                    }
                    else if (mappings.DataType == (int)ImportDetailsMapping.SourceDataType_OptionSet.WholeNumber)
                    {
                        int wholeval = GetValueFromWholenumber(mappings.Target, context, Target);
                        taskObject[mappings.Target] = mappings.value;
                    }
                }

                //for lookup values we are setting value here
                taskObject[Task.AutomatedTask] = true;
                taskObject[Task.RegardingObjectId] = new EntityReference(Loan.EntityName, Target.Id);
                taskObject[Task.OwnerId] = new EntityReference(userRec.LogicalName, userRec.Id);
            }
            catch (Exception ex)
            {
                context.Trace(ex.Message);
            }

        }
        public static int GetValueFromWholenumber(string field, IExtendedExecutionContext context, Entity Target)
        {
            context.Trace("Inside GetValueFromWholenumber");
            int wholenumber;
            Entity loan = context.Retrieve(Loan.EntityName, Target.Id, new ColumnSet(field));
            wholenumber = loan.GetAttributeValue<int>(field);
            return wholenumber;
        }
        public static string GetValueFromSingleLineText(string field, IExtendedExecutionContext context)
        {
            context.Trace("Inside GetValueFromSingleLineText");
            Entity loan = context.Retrieve(Loan.EntityName, new Guid("DE1F885C-4D85-EA11-A811-000D3A30F195"), new ColumnSet(field));
            string value = loan.GetAttributeValue<string>(field);
            return value;
        }
        public static string GetValueFromSourceLookUpField(string field, IExtendedExecutionContext context, Entity Target)
        {
            context.Trace("Inside GetValueFromSourceLookUpField");
            Entity loanety = context.Retrieve(Loan.EntityName, Target.Id, new ColumnSet(field));
            string ownerName = loanety.GetAttributeValue<EntityReference>(field).Name;
            return ownerName;
        }
        public static int FetchOptionSetLookupValue(Common.Mapping objMapping, int optionsetVal, IExtendedExecutionContext context)
        {
            context.Trace("Inside FetchOptionSetLookupValue");
            int val = 11;
            try
            {
                if (optionsetVal == (int)Task.PriorityCode_OptionSet.Low)
                {
                    val = (int)Task.PriorityCode_OptionSet.Low;
                }
                else if (optionsetVal == (int)Task.PriorityCode_OptionSet.High)
                {
                    val = (int)Task.PriorityCode_OptionSet.High;
                }
                else if (optionsetVal == (int)Task.PriorityCode_OptionSet.Normal)
                {
                    val = (int)Task.PriorityCode_OptionSet.Normal;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching lookupOption: " + ex.Message);
            }
            return val;
        }
        public static DateTime GetValueFromDateTime(Common.Mapping mappings, string field, IExtendedExecutionContext context, Entity Target)
        {
            context.Trace("Inside GetValueFromDateTime");
            DateTime value;
            Entity loanety = context.Retrieve(Loan.EntityName, Target.Id, new ColumnSet(field));
            value = loanety.GetAttributeValue<DateTime>(field);
            context.Trace(value.ToString());
            value = checkBusinessClosures(context, value.ToString(), null);
            return value;
        }
        public static string GetId(string lookupEntityName, string filterAttributeName, Common.Mapping mapping, IExtendedExecutionContext context)
        {
            context.Trace("Inside GetId");
            string filterAttributeValue = mapping.value;
            lookupEntityName = lookupEntityName.Trim();
            Guid entityId = Guid.Empty;
            try
            {
                var fetchXml = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
               "<entity name='" + lookupEntityName + "'>" +
               "<attribute name='" + lookupEntityName + "id" + "' />" +
               "<filter type='and'>" +
                 "<condition attribute='" + filterAttributeName + "' operator='eq' value='" + filterAttributeValue + "'/>"
                 +
                 (lookupEntityName == "systemuser" ? "<condition attribute='isdisabled' operator='eq' value='0'/>" : (lookupEntityName != "lead") ?
                 "<condition attribute='statecode' operator='eq' value='0'/>" : string.Empty) +
               "</filter></entity></fetch>";

                EntityCollection ecEntities = context.RetrieveMultiple(new FetchExpression(fetchXml));
                if (ecEntities != null && ecEntities.Entities.Count > 0)
                {
                    Entity objEntity = ecEntities.Entities[0];
                    if (objEntity != null)
                    {
                        entityId = objEntity.Id;
                    }
                }
            }
            catch (Exception ex)
            {
                context.Trace(ex.Message);
                entityId = Guid.Empty;
            }
            return (entityId == Guid.Empty ? string.Empty : entityId.ToString());
        }
        public static bool isWorkingDay(EntityCollection calendarrules, DateTime time)
        {
            if (time.DayOfWeek == DayOfWeek.Saturday || time.DayOfWeek == DayOfWeek.Sunday)
            {
                return false;
            }
            if (calendarrules.Entities.Count > 0)
            {
                var entity = calendarrules.Entities[0];
                EntityCollection businessClosureRules = (EntityCollection)calendarrules.Entities[0].Attributes["calendarrules"];

                foreach (Entity rule in businessClosureRules.Entities)
                {
                    if ((DateTime)rule.Attributes["effectiveintervalstart"] <= time && (DateTime)rule.Attributes["effectiveintervalend"] > time)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public static bool checkIfDuplicateTaskExist(IExtendedExecutionContext context, Guid loanId, string taskSubj)
        {
            string CheckIfTasksExists = @"<fetch top='5000' >
                          <entity name='task' >
                            <attribute name='subject' />
                            <attribute name='description' />
                            <filter>
                              <condition attribute='subject' operator='like' value='%" + taskSubj + @"%' />
                              <condition attribute='regardingobjectid' operator='eq' value='" + loanId + @"'/>
                                <condition attribute='statecode' operator='neq' value='2' />
                                </filter>
                          </entity>
                        </fetch>";
            EntityCollection ifTaskExists = context.RetrieveMultiple(new FetchExpression(CheckIfTasksExists));
            if (ifTaskExists.Entities.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /*private static int? RetrieveCurrentUsersSettings(IOrganizationService service,IExtendedExecutionContext context)
        {
            context.Trace("Inside RetriveCurrentUserSettings");
            var currentUserSettings = service.RetrieveMultiple(
            new QueryExpression("usersettings")
            {
                ColumnSet = new ColumnSet("timezonecode"),
                Criteria = new FilterExpression
                {
                    Conditions =
                        {
                        new ConditionExpression("systemuserid", ConditionOperator.Equal,context.UserId)
                        }
                }
            }).Entities[0].ToEntity<Entity>();

            //return time zone code
            context.Trace(currentUserSettings.Attributes["timezonecode"].ToString());
            return (int?)currentUserSettings.Attributes["timezonecode"];
        }
        private static DateTime RetrieveLocalTimeFromUTCTime(DateTime utcTime, int? timeZoneCode, IOrganizationService service, IExtendedExecutionContext context)*
        {
            context.Trace("Inside RetreiveLocalTimeFromUTCTime");
            if (!timeZoneCode.HasValue)
                return DateTime.Now;
            var request = new LocalTimeFromUtcTimeRequest
            {
                TimeZoneCode = timeZoneCode.Value,
                UtcTime = utcTime.ToUniversalTime()
            };
            var response = (LocalTimeFromUtcTimeResponse)service.Execute(request);
            context.Trace(response.ToString());
            return response.LocalTime;
        }*/
        public static DateTime checkBusinessclosuer_ClosingDate(IExtendedExecutionContext context, string datetimeVal, DateTime? closingDate)
        {
            QueryExpression calenderQuery = new QueryExpression
            {
                EntityName = "calendar",
                ColumnSet = new ColumnSet(true),
                Criteria =
                                       {
                                           Conditions =
                                           {
                                               new ConditionExpression("name", ConditionOperator.Equal, "Business Closure Calendar")
                                           }
                                       }
            };

            EntityCollection calendar = context.RetrieveMultiple(calenderQuery);
            context.Trace("Inside CheckBusinessClosuer_ClosingDate" + closingDate);
            DateTime closeDate = Convert.ToDateTime(closingDate);
            if (closingDate != null)
            {
                context.Trace("The Loan Status is APproved and the closing date contains value is " + " " + closingDate.ToString());
                int daysToAdd1 = int.Parse(datetimeVal);
                if (daysToAdd1 < 0)
                {
                    while (daysToAdd1 < 0)
                    {
                        closeDate = closeDate.AddDays(-1);
                        if (isWorkingDay(calendar, closeDate))
                        {
                            daysToAdd1++;
                        }
                    }
                }
                context.Trace("The CloseDate value is" + " " + closeDate.ToString());
            }
            return closeDate;
        }
        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Asynchronous, "Update");
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "Create");
        }
    }
}