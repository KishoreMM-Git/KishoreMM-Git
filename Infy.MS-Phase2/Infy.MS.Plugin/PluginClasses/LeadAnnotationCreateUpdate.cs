using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xrm;
using XRMExtensions;
using Microsoft.Xrm.Sdk.Query;
namespace Infy.MS.Plugins
{
   public class LeadAnnotationCreateUpdate:BasePlugin
    {
        public override void ExecutePlugin(IExtendedPluginContext context)
        {

            if (context == null)
            {
                throw new InvalidPluginExecutionException("Context not Found");
            }

           Entity Target = context.GetTargetEntity<Entity>();
            if (context.MessageName.ToLower() == "create" || context.MessageName.ToLower() == "update" && Target.LogicalName == Annotation.EntityName)
            {
                var recordId = Target.Id;
                Entity annotationEntity = context.Retrieve(Annotation.EntityName, recordId, new ColumnSet(Annotation.ObjectId, Annotation.ModifiedOn));
                if (annotationEntity != null && annotationEntity.Contains(Annotation.ObjectId))
                {
                    EntityReference entityReference = annotationEntity.GetAttributeValue<EntityReference>(Annotation.ObjectId);
                    if (entityReference.LogicalName.Equals(Lead.EntityName))
                    {
                        Entity leadEntity = new Entity(Lead.EntityName);
                        leadEntity.Id = entityReference.Id;
                        leadEntity[Lead.LastNotesDate] = annotationEntity.GetAttributeValue<DateTime>(Annotation.ModifiedOn).ToUniversalTime();
                        leadEntity[Lead.Lastinteractiondate] = annotationEntity.GetAttributeValue<DateTime>(Annotation.ModifiedOn).ToUniversalTime();
                        context.Update(leadEntity);
                    }
                }
            }
            // throw new NotImplementedException();
        }

        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "Update");
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "Create");
        }
    }
}
