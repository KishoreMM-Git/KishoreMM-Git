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
using Microsoft.Xrm.Sdk.Client;

namespace Infy.MS.Plugins.PluginClasses
{
    public class TaskTemplateCreateUpdate : IPlugin
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
                        Entity taskety = service.Retrieve(Target.LogicalName, Target.Id, new ColumnSet("ims_tasklist"));
                        tracingService.Trace("Target contains value");

                       if (taskety.Attributes.Contains("ims_tasklist"))
                        {
                            string entityName = taskety.GetAttributeValue<EntityReference>("ims_tasklist").LogicalName;
                            Guid tasklistid = taskety.GetAttributeValue<EntityReference>("ims_tasklist").Id;

                                                            
                                CalculateRollupFieldRequest crfrRequested = new CalculateRollupFieldRequest
                                {
                                    Target = new EntityReference(entityName, tasklistid),
                                    FieldName = "ims_countoftasks"
                                };
                                service.Execute(crfrRequested);
                                                           
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                tracingService.Trace(ex.Message);
            }
        }
    }
}

//public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
//{
//    yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "Create");
//    yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "Update");
//}