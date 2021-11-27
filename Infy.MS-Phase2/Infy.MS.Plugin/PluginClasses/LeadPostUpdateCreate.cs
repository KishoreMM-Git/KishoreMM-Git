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
namespace Infy.MS.Plugins
{
    public class LeadPostUpdateCreate : BasePlugin
    {
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            //if (context.Depth > 1) return;
            var targetEntity = context.GetTargetEntity<Entity>();
            Entity leadEntity = new Entity(targetEntity.LogicalName);
            leadEntity.Id = targetEntity.Id;

            if(context.MessageName.ToLower() == "create")
            {
                leadEntity[Lead.LastAssingedOn] = targetEntity.GetAttributeValue<DateTime>(Lead.CreatedOn);
            }
            if(targetEntity.Contains(Lead.LeadStatus))
            {
                leadEntity[Lead.LeadStatusDate] = DateTime.UtcNow;
                context.Trace("Updated the Lead Status Date");
            }
            if (targetEntity.Contains(Lead.BirthDay))
            {
                //Getting the Current Date
                DateTime dateTimeNow = DateTime.Now.Date;
                //Getting the Lead Birthday Date
                DateTime leadBirthday = targetEntity.GetAttributeValue<DateTime>(Lead.BirthDay).Date;
                //If the Lead Birthday is already crossed for the current year, update Upcoming birthday to the Next year with same month and date
                if ((leadBirthday.Month < dateTimeNow.Month) || (leadBirthday.Month == dateTimeNow.Month && leadBirthday.Day <= dateTimeNow.Day))
                    leadEntity.Attributes[Lead.UpcomingBirthday] = new DateTime(dateTimeNow.Year + 1, leadBirthday.Month, leadBirthday.Day);
                else
                    leadEntity.Attributes[Lead.UpcomingBirthday] = new DateTime(dateTimeNow.Year, leadBirthday.Month, leadBirthday.Day);
                
                context.Trace("Updated the Upcoming date of birth");
            }
            context.Update(leadEntity);

         
                //Update Originated by Movement Direct attribute
                //if (context.MessageName.ToLower().Equals("create"))
                //{
                //    var xmlBusinessUnit = context.GetConfigValue<string>(Constants.MovementDirectBusinessUnit, Constants.AppConfigSetup);
                //    if (xmlBusinessUnit != string.Empty)
                //    {
                //        var isMoverDirectAppCreation = new Common().CheckMovementDirectBusinessUnit(xmlBusinessUnit, context.InitiatingUserId, context.SystemOrganizationService);
                //        if (isMoverDirectAppCreation)
                //        {
                //            Entity lead = new Entity(targetEntity.LogicalName);
                //            lead.Attributes.Add(Lead.OrginatedByMovementDirect, true);
                //            lead.Id = targetEntity.Id;
                //            context.Update(lead);
                //        }
                //    }
                //}
                //if (targetEntity.Attributes.Contains(Lead.LeadStatus) && targetEntity[Lead.LeadStatus] != null)
                //{
                //    Guid leadStatusGuid = ((EntityReference)targetEntity.Attributes[Lead.LeadStatus]).Id;
                //    Common objCommon = new Common();
                //    var optionset = objCommon.FetchLookupOptionSet(context, leadStatusGuid, LeadStatus.LeadStatusOptions, LeadStatus.EntityName);
                //    if (optionset != null)
                //    {
                //        leadEntity.Attributes.Add(Lead.LeadStatusOptions, optionset);
                //    }
                //}

                //if (targetEntity.Attributes.Contains(Lead.LeadStatusOptions) && targetEntity[Lead.LeadStatusOptions] != null)
                //{
                //    int leadStatusoptions = targetEntity.GetAttributeValue<OptionSetValue>(Lead.LeadStatusOptions).Value;
                //    EntityReference leadStaus = null;
                //    QueryExpression queryExpression = new QueryExpression(LeadStatus.EntityName);
                //    queryExpression.ColumnSet.AddColumn(LeadStatus.PrimaryKey);
                //    queryExpression.Criteria.AddCondition(new ConditionExpression(LeadStatus.LeadStatusOptions, ConditionOperator.Equal, leadStatusoptions));
                //    var ec = context.RetrieveMultiple(queryExpression);
                //    if (ec.Entities.Count > 0)
                //    {
                //        if (ec.Entities.FirstOrDefault().Contains(LeadStatus.PrimaryKey))
                //        {
                //            leadStaus = new EntityReference(LeadStatus.EntityName, ec.Entities.FirstOrDefault().Id);
                //        }
                //    }

                //    if (leadStaus != null)
                //        leadEntity.Attributes.Add(Lead.LeadStatus, leadStaus);
                //}

            }
        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Asynchronous, "Create");
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Asynchronous, "Update");
        }
    }
}

