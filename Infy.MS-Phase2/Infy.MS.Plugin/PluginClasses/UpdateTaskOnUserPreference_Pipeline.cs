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
    public class UpdateTaskOnUserPreference_Pipeline : BasePlugin
    {
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            Entity Target = null;
            if (context == null)
            {
                throw new NotImplementedException();
            }
            if (context.MessageName.ToLower() == "update")
            {
                Target = context.GetTargetEntity<Entity>();
                Guid userId = Target.Id;
                completeTask(context, Target, userId);
            }
        }
        public void completeTask(IExtendedExecutionContext context, Entity user, Guid userId)
        {
            Entity systemUser = null;
            if (user.LogicalName == SystemUser.EntityName)
            {
                systemUser = context.Retrieve(SystemUser.EntityName, user.Id, new ColumnSet(SystemUser.IsAutomationRequired, SystemUser.FirstName, SystemUser.FullName));
            }
            else if (user.LogicalName == Team.EntityName)
            {
                return;
            }
            bool isAutomation = systemUser.GetAttributeValue<bool>(SystemUser.IsAutomationRequired);
            if (isAutomation == false)
            {
                string automatedTasksForAllLoans = @"<fetch top='5000' >
                      <entity name='task' >
                        <attribute name='subject' />
                        <attribute name='description' />
                        <attribute name='ims_automatedtask' />
                        <filter>
                          <condition attribute='ims_automatedtask' operator='eq' value='1' />
                          <condition attribute='statecode' operator='eq' value='0' />
                        </filter>
                        <link-entity name='opportunity' from='opportunityid' to='regardingobjectid' >
                          <attribute name='ownerid' />
                          <attribute name='name' />
                          <filter>
                            <condition attribute='ownerid' operator='eq' value='" + user.Id + @"' />
                          </filter>
                        </link-entity>
                      </entity>
                    </fetch>";

                EntityCollection autoamtedTaskforLoans = context.RetrieveMultiple(new FetchExpression(automatedTasksForAllLoans));
                foreach (var tasks in autoamtedTaskforLoans.Entities)
                {
                    Entity taskety = new Entity(Task.EntityName, tasks.Id);
                    taskety[Task.StateCode] = new OptionSetValue(2);
                    context.Update(taskety);
                    context.Trace("update sucessful");
                }
            }
        }
        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "Create");
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "Update");
        }
    }
}
