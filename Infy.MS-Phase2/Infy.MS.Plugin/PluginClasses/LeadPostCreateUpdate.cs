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
    public class LeadPostCreateUpdate : BasePlugin
    {
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            var targetEntity = context.GetTargetEntity<Entity>();
            if (targetEntity.Contains(Lead.BirthDay))
            {
                DateTime dateTimeNow = DateTime.Now.Date;
                Entity leadEntity = new Entity(targetEntity.LogicalName);
                leadEntity.Id = targetEntity.Id;
                DateTime leadBirthday = targetEntity.GetAttributeValue<DateTime>(Lead.BirthDay).ToLocalTime();
                if ((leadBirthday.Month < dateTimeNow.Month) || (leadBirthday.Month == dateTimeNow.Month && leadBirthday.Day < dateTimeNow.Day))
                    leadEntity.Attributes[Lead.UpcomingBirthday] = new DateTime(dateTimeNow.Year + 1, leadBirthday.Month, leadBirthday.Day).ToUniversalTime();
                else
                    leadEntity.Attributes[Lead.UpcomingBirthday] = new DateTime(dateTimeNow.Year, leadBirthday.Month, leadBirthday.Day).ToUniversalTime();
                context.Update(leadEntity);
            }

        }
        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Asynchronous, "Create");
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Asynchronous, "Update");
        }
    }
}

