
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xrm;
using XRMExtensions;
namespace Infy.MS.WorkflowActivities
{
    public class SendBirthdayReminderEmail : BaseWorkflowActivity
    {
        public override void ExecuteWorkflowActivity(IExtendedWorkflowContext context)
        {
            try
            {
                //To Update the Next year Birthday
                var confiValueYesterday = context.GetConfigValue<string>(ConfigurationKeys.Key_UpcomingBirthday_Yesterday, AppConfigSetupKeys.AppKey_UpcomingbirhdayReminderNotofication);
                if (confiValueYesterday != string.Empty)
                {
                    var entityCollection = context.RetrieveMultiple(new FetchExpression(string.Format(@"" + confiValueYesterday + "")));
                    if (entityCollection.Entities.Count > 0)
                    {
                        foreach (Entity en in entityCollection.Entities)
                        {
                            if (en.Contains(Lead.UpcomingBirthday))
                            {
                                Entity leadEntity = new Entity(Lead.EntityName);
                                leadEntity.Id = en.Id;
                                leadEntity.Attributes[Lead.UpcomingBirthday] = en.GetAttributeValue<DateTime>(Lead.UpcomingBirthday).ToLocalTime().AddYears(1);
                                context.Update(leadEntity);
                                context.Trace("Updated the next birthday date");
                            }
                        }
                    }
                }

                //To Get the Next XDays Upcoming Birthday Lead Records
                var confiValueNextXDays = context.GetConfigValue<string>(ConfigurationKeys.Key_UpcomingBirthday_NextXDays, AppConfigSetupKeys.AppKey_UpcomingbirhdayReminderNotofication);
                if (confiValueNextXDays != string.Empty)
                {
                    var entityCollection = context.RetrieveMultiple(new FetchExpression(string.Format(@"" + confiValueNextXDays + "")));
                    if (entityCollection.Entities.Count > 0)
                    {
                        var configValueEmailActivity = context.GetConfigValue<string>(ConfigurationKeys.Key_UpcomingBirthday_EmailActivity, AppConfigSetupKeys.AppKey_UpcomingbirhdayReminderNotofication);
                        foreach (Entity en in entityCollection.Entities)
                        {
                            if (!CheckRelatedTaskActivity(en, context))
                            {
                                CreateTaskActvity(context, en);
                            }
                        }
                    }
                }
                UpdateBirthDayReminder(context);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }

        }

        /// <summary>
        /// Creating a Task 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="targetLead"></param>
        public void CreateTaskActvity(IExtendedExecutionContext context,Entity targetLead)
        {
            Entity task = new Entity(XRMExtensions.Task.EntityName);
            if (targetLead.Contains(Lead.OwningUser))
                task.Attributes[XRMExtensions.Task.OwnerId] = new EntityReference(SystemUser.EntityName, ((EntityReference)targetLead[Lead.Owner]).Id);
            else if(targetLead.Contains(Lead.OwningTeam))
                task.Attributes[XRMExtensions.Task.OwnerId] = new EntityReference(Team.EntityName, ((EntityReference)targetLead[Lead.Owner]).Id);
            task.Attributes[XRMExtensions.Task.RegardingObjectId] = new EntityReference(Lead.EntityName,targetLead.Id);
            task.Attributes[XRMExtensions.Task.Subject] = targetLead.Contains(Lead.PrimaryName) ? string.Join("-",targetLead.GetAttributeValue<string>(Lead.PrimaryName), targetLead.Contains(Lead.UpcomingBirthday) ? targetLead.GetAttributeValue<DateTime>(Lead.UpcomingBirthday).Date.ToShortDateString():string.Empty) : "Lead Birthday Wishes";
            task.Attributes[XRMExtensions.Task.ScheduledStart] = targetLead.Contains(Lead.UpcomingBirthday) ? targetLead.GetAttributeValue<DateTime>(Lead.UpcomingBirthday).Date : DateTime.Now.Date;
            task.Attributes[XRMExtensions.Task.ScheduledEnd] = targetLead.Contains(Lead.UpcomingBirthday) ? DateTime.Parse(targetLead.GetAttributeValue<DateTime>(Lead.UpcomingBirthday).Date.ToShortDateString() + " " + "11:59 PM",System.Globalization.CultureInfo.InvariantCulture,System.Globalization.DateTimeStyles.None) : DateTime.Now.Date;
            task.Attributes[XRMExtensions.Task.ActualDurationMinutes] = Convert.ToInt32(30);
            context.Create(task);
        }

        /// <summary>
        /// Composing an Email with User and Target Entity(Lead)
        /// </summary>
        /// <param name="service">Crm Org Service</param>
        /// <param name="userId">user Context From Party</param>
        /// <param name="target">Lead entity object To Party</param>
        /// <returns></returns>
        public void ComposeEmail(IExtendedWorkflowContext context, Guid userId, Entity target, string emailActivityXml)
        {
            //Compising an email

            //XML to check Email Activity is exists or not for the Lead
            if (emailActivityXml != string.Empty && emailActivityXml.Contains(ConfigurationKeys.DynamicValue_Key_UpcomingBirthday_EmailActivity))
            {
                if (CheckRelatedEmailActivity(emailActivityXml.Replace(ConfigurationKeys.DynamicValue_Key_UpcomingBirthday_EmailActivity, target.Id.ToString()), context))
                {
                    Entity fromParty = new Entity(ActivityParty.EntityName);
                    fromParty.Attributes[ActivityParty.PartyId] = new EntityReference(SystemUser.EntityName, userId);
                    Entity toParty = new Entity(ActivityParty.EntityName);
                    toParty.Attributes[ActivityParty.PartyId] = new EntityReference(Lead.EntityName, target.Id);

                    Entity email = new Entity(Email.EntityName);
                    email.Attributes[Email.From] = new Entity[] { fromParty };
                    email.Attributes[Email.To] = new Entity[] { toParty };
                    email.Attributes[Email.Subject] = "Birthday Wishes";
                    email.Attributes[Email.RegardingObjectId] = new EntityReference(Lead.EntityName, target.Id);
                    email.Attributes[Email.Description] = "Happy Birthday";
                    Guid emailId = context.Create(email);
                }
            }
        }

        /// <summary>
        /// Updting the Birth Daty Reminder Record(for the next trigger point) which is configured in Configuration Entity as Record
        /// </summary>
        /// <param name="service">Crm Org Service</param>
        public void UpdateBirthDayReminder(IExtendedWorkflowContext context)
        {
            // getting entire config Object "Upcoming_Birthday_Reccouring" record
            var confiObject = context.GetConfigValue<Entity>(ConfigurationKeys.Key_UpcomingBirthday_RecurringTrigger, AppConfigSetupKeys.AppKey_UpcomingbirhdayReminderNotofication);
            if(confiObject!=null)
            {
                AttributeCollection keyValuePairs = new AttributeCollection();
                keyValuePairs.Add(Configuration.Value, DateTime.UtcNow.AddDays(1).ToString());
                context.Update(new Entity
                {
                    LogicalName = Configuration.EntityName,
                    Id = confiObject.Id,
                    Attributes = keyValuePairs
                });
            }
        }
        /// <summary>
        /// Getting the Upcoming Birthday Record from the Configuration Entity, here "UP_COMING_BIRTHDAY_REMINDER" is case sensitive
        /// </summary>
        /// <param name="service">Crm Org Service</param>
        /// <returns></returns>
        public Entity GetBirthDayReminderRecordAppConfig(IExtendedWorkflowContext context)
        {
            QueryExpression expression = new QueryExpression(Configuration.EntityName);
            expression.ColumnSet.AddColumn(Configuration.PrimaryName);
            expression.Criteria.AddCondition(Configuration.PrimaryName, ConditionOperator.Equal, ConfigurationKeys.Key_UpcomingBirthday_RecurringTrigger);
            Entity appConfig = context.RetrieveMultiple(expression).Entities.FirstOrDefault();
            if (appConfig.Id != Guid.Empty)
                return appConfig;
            return new Entity();

        }
        /// <summary>
        /// Check the Email Activity already created for the lead
        /// </summary>
        /// <param name="xml">Fetch Xml</param>
        /// <param name="context">Work Flow Context</param>
        /// <returns></returns>
        public bool CheckRelatedEmailActivity(string xml, IExtendedExecutionContext context)
        {
            var entityCollection = context.RetrieveMultiple(new FetchExpression(string.Format(@"" + xml + "")));
            if (entityCollection.Entities.Count > 0)
                return false;
            return true;
        }
        /// <summary>
        /// Check the Task Activity is existed for the Lead with Birthday 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool CheckRelatedTaskActivity(Entity entity,IExtendedWorkflowContext context)
        {
            if(entity.Contains(Lead.UpcomingBirthday))
            {
                QueryByAttribute queryByAttribute = new QueryByAttribute(XRMExtensions.Task.EntityName);
                queryByAttribute.ColumnSet = new ColumnSet(XRMExtensions.Task.Subject);
                queryByAttribute.AddAttributeValue(XRMExtensions.Task.RegardingObjectId, entity.Id);
                queryByAttribute.AddAttributeValue(XRMExtensions.Task.ScheduledStart, entity.GetAttributeValue<DateTime>(Lead.UpcomingBirthday));
                EntityCollection entityCollection = context.RetrieveMultiple(queryByAttribute);
                if (entityCollection.Entities.Count > 0)
                    return true;
            }
            return false;
        }
    }
}
