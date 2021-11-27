using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xrm;
using System.Threading.Tasks;
//using XRMExtensions;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk.Messages;
using XRMExtensions;

namespace Infy.MS.Plugins
{
  public class CreatemultipleTasksSelectedRecords : BasePlugin
    {
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            EntityReference TaskEntity = (EntityReference)context.InputParameters["Target"];

            if (TaskEntity != null && TaskEntity.Id != Guid.Empty)
            {
                try
                {
                    if (context.InputParameters.Contains("InputParameters"))
                    {
                        QueryExpression Taskquery = new QueryExpression(XRMExtensions.Task.EntityName);
                        ColumnSet cols = new ColumnSet(true);
                        Taskquery.ColumnSet = cols;
                        Taskquery.Criteria.AddCondition(new ConditionExpression(XRMExtensions.Task.PrimaryKey, ConditionOperator.Equal, TaskEntity.Id));
                        EntityCollection taskrec = context.RetrieveMultiple(Taskquery);
                        Entity TaskRecorde = null;
                        if (taskrec != null && taskrec.Entities.Count > 0)
                        {
                            TaskRecorde = taskrec[0];
                        }
                        string leads = (string)context.InputParameters["InputParameters"];
                        if (leads.Contains(";"))
                        {
                            List<string> leadids = leads.Split(';').Select(s => s.Trim()).ToList();
                            leadids = leadids.Distinct().ToList();
                            if (leadids != null && leadids.Count > 0)
                            {
                                foreach (string item in leadids.Skip(1))
                                {
                                    createtask(context.OrganizationService, item, TaskRecorde);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in FollowUpPlugin.", ex);
                }
            }
        }
        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "ims_CreateMultipleTasks");
        }

        public void createtask(IOrganizationService service, string leadid, Entity task)
        {
            if (!string.IsNullOrEmpty(leadid))
            {
                Entity objtask = new Entity(XRMExtensions.Task.EntityName);

                if (task.Attributes.Contains(XRMExtensions.Task.OwnerId))
                {
                    objtask[XRMExtensions.Task.OwnerId] = task[XRMExtensions.Task.OwnerId];
                }

                if (task.Attributes.Contains(XRMExtensions.Task.Subject))
                {
                    objtask[XRMExtensions.Task.Subject] = task[XRMExtensions.Task.Subject];
                }

                if (task.Attributes.Contains(XRMExtensions.Task.Description))
                {
                    objtask[XRMExtensions.Task.Description] = task[XRMExtensions.Task.Description];
                }
                if (task.Attributes.Contains(XRMExtensions.Task.ScheduledEnd))
                {
                    objtask[XRMExtensions.Task.ScheduledEnd] = task[XRMExtensions.Task.ScheduledEnd];
                }

                if (task.Attributes.Contains(XRMExtensions.Task.ScheduledStart))
                {
                    objtask[XRMExtensions.Task.ScheduledStart] = task[XRMExtensions.Task.ScheduledStart];
                }

                if (task.Attributes.Contains(XRMExtensions.Task.RegardingObjectId))
                {
                    var EntitylogicalName = ((EntityReference)task[XRMExtensions.Task.RegardingObjectId]).LogicalName;
                    objtask[XRMExtensions.Task.RegardingObjectId] = new EntityReference(EntitylogicalName, Guid.Parse(leadid));
                }

                if (task.Attributes.Contains(XRMExtensions.Task.ActualDurationMinutes))
                {
                    objtask[XRMExtensions.Task.ActualDurationMinutes] = task[XRMExtensions.Task.ActualDurationMinutes];
                }
                if (task.Attributes.Contains(XRMExtensions.Task.PriorityCode))
                {
                    objtask[XRMExtensions.Task.PriorityCode] = task[XRMExtensions.Task.PriorityCode];
                }
                service.Create(objtask);

            }
        }
    }
}

