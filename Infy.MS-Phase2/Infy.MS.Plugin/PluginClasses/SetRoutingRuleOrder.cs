using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xrm;
using XRMExtensions;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk.Messages;
using System.Xml.Linq;

namespace Infy.MS.Plugins
{
    public class SetRoutingRuleOrder : BasePlugin
    {
        int status;
        int ruleType;
        int priorityNumber;
        int BasePriorityNumber;
        int EntityMonikerStatus;
        Guid ObjrecordID = Guid.Empty;
        int ObjPriorityNumber;
        int ObjBasePriority;
        Guid ID = Guid.Empty;
        Entity Target;
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            if (context == null)
            {
                throw new InvalidPluginExecutionException("Context not Found");
            }
            // Active Deactive of Lead Routing Record 
            if (context.MessageName.ToLower() == "setstatedynamicentity")
            {
                string Entityname = ((EntityReference)context.InputParameters["EntityMoniker"]).LogicalName;
                {
                    ID = ((EntityReference)context.InputParameters["EntityMoniker"]).Id;
                    EntityMonikerStatus = ((OptionSetValue)context.InputParameters["State"]).Value;

                    if (ID != Guid.Empty && ID != null)
                    {
                        Entity objRoutingRule = context.PreEntityImages["PreImage"];

                        if (objRoutingRule != null)
                        {
                            status = objRoutingRule.GetAttributeValue<OptionSetValue>(RoutingRule.Status).Value;
                            ruleType= objRoutingRule.GetAttributeValue<OptionSetValue>(RoutingRule.RuleType).Value;

                            if (EntityMonikerStatus != status)
                            {
                                if (status == 0)
                                {
                                    if (objRoutingRule.Contains(RoutingRule.Priority) && objRoutingRule.Contains(RoutingRule.BasePriorityNumber))
                                    {
                                        priorityNumber = objRoutingRule.GetAttributeValue<int>(RoutingRule.Priority);
                                        BasePriorityNumber = objRoutingRule.GetAttributeValue<int>(RoutingRule.BasePriorityNumber);
                                        {
                                            EntityCollection Results = GetRoutingRuleReocrds(context, status);
                                            if (Results.Entities.Count > 0)
                                            {
                                                updateroutingruleStatusequaltoone(Results, context, priorityNumber);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (status == 1)
                                    {
                                        EntityCollection Results1 = GetRoutingRuleReocrds(context, status);
                                        if (Results1.Entities.Count > 0)
                                        {

                                            updateroutingruleStatusequaltozero(Results1, context, ID);

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //delete Record of routing Rule
            if (context.MessageName.ToLower() == "delete")
            {
                Target = context.GetTargetEntity<Entity>();               
                Entity ObjPreImage = context.PreEntityImages["PreImage"];
                ruleType = ObjPreImage.GetAttributeValue<OptionSetValue>(RoutingRule.RuleType).Value;
                if (ObjPreImage != null)
                {
                    if (ObjPreImage.Contains(RoutingRule.Priority) && ObjPreImage.Contains(RoutingRule.BasePriorityNumber))
                    {
                        int delPriorityNumber = (int)ObjPreImage.GetAttributeValue<int>(RoutingRule.Priority);
                        int delBasePriority = (int)ObjPreImage.GetAttributeValue<int>(RoutingRule.BasePriorityNumber);
                        status = (int)ObjPreImage.GetAttributeValue<OptionSetValue>(RoutingRule.Status).Value;
                        if (status == 0)
                        {
                            EntityCollection Results3 = GetRoutingRuleReocrds(context, 1);
                            if (Results3.Entities.Count > 0)
                            {
                                updateroutingruleStatusequaltoone(Results3, context, delPriorityNumber);
                            }
                        }
                    }
                }
            }

            //create     Record of routing Rule
            if (context.MessageName.ToLower() == "create")
            {
                Target = context.GetTargetEntity<Entity>();
                //Guid TargetId = Target.Id;
                ruleType = Target.GetAttributeValue<OptionSetValue>(RoutingRule.RuleType).Value;
                EntityCollection Results5 = GetRoutingRuleReocrds(context, 1);
                if (Results5.Entities.Count > 0)
                {
                    priorityNumber = (int)Results5.Entities[0].Attributes[RoutingRule.Priority];
                    BasePriorityNumber = (int)Results5.Entities[0].Attributes[RoutingRule.BasePriorityNumber];

                    priorityNumber = priorityNumber + 1;
                    BasePriorityNumber = BasePriorityNumber + 1;
                    if(priorityNumber != 0 && BasePriorityNumber !=0)
                    {
                        Target.Attributes.Add(RoutingRule.Priority, priorityNumber);
                        Target.Attributes.Add(RoutingRule.BasePriorityNumber, BasePriorityNumber);
                    }
                }
                else
                {
                    Target.Attributes.Add(RoutingRule.Priority, 1);
                    Target.Attributes.Add(RoutingRule.BasePriorityNumber, 1);
                }
            }

        }

        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PreOperation, SdkMessageProcessingStepMode.Synchronous, "SetStateDynamicEntity");
            yield return new Xrm.RegisteredEvent(PipelineStage.PreValidation, SdkMessageProcessingStepMode.Synchronous, "Create");
            yield return new Xrm.RegisteredEvent(PipelineStage.PreOperation, SdkMessageProcessingStepMode.Synchronous, "Delete");
        }

        public void UpdateRoutingrule(IOrganizationService service, int ObjPriorityNumber, int ObjBasePriority, Guid ObjrecordID)
        {
            Entity updateRoutingRule = new Entity(RoutingRule.EntityName);
            updateRoutingRule[RoutingRule.Priority] = ObjPriorityNumber;
            updateRoutingRule[RoutingRule.BasePriorityNumber] = ObjBasePriority;
            updateRoutingRule.Id = ObjrecordID;
            service.Update(updateRoutingRule);
        }

        public EntityCollection GetRoutingRuleReocrds(IOrganizationService service, int status)
        {
            EntityCollection objRoutingOutput = null;
            QueryExpression objRoutingRule = new QueryExpression(RoutingRule.EntityName);
            ColumnSet cols = new ColumnSet(RoutingRule.Priority, RoutingRule.BasePriorityNumber, RoutingRule.PrimaryName);
            objRoutingRule.ColumnSet = cols;
            objRoutingRule.Criteria.AddCondition(new ConditionExpression(RoutingRule.Status, ConditionOperator.Equal, 0));
            objRoutingRule.Criteria.AddCondition(new ConditionExpression(RoutingRule.RuleType, ConditionOperator.Equal, ruleType));
            objRoutingRule.Criteria.AddCondition(RoutingRule.Priority, ConditionOperator.NotNull);
            if (status == 0)
            {
                objRoutingRule.Orders.Add(new OrderExpression(RoutingRule.Priority, OrderType.Ascending));
            }
            if (status == 1)
            {
                objRoutingRule.Orders.Add(new OrderExpression(RoutingRule.Priority, OrderType.Descending));
            }
            objRoutingOutput = service.RetrieveMultiple(objRoutingRule);

            return objRoutingOutput;
        }

        public void updateroutingruleStatusequaltozero(EntityCollection Results1, IOrganizationService service,Guid recordid)
        {
            if (Results1.Entities[0].Contains(RoutingRule.Priority) && Results1.Entities[0].Contains(RoutingRule.BasePriorityNumber))
            {
                ObjPriorityNumber = (int)Results1.Entities[0].Attributes[RoutingRule.Priority];
                ObjBasePriority = (int)Results1.Entities[0].Attributes[RoutingRule.BasePriorityNumber];
                ObjPriorityNumber = ObjPriorityNumber + 1;
                ObjBasePriority = ObjBasePriority + 1;
                UpdateRoutingrule(service, ObjPriorityNumber, ObjBasePriority, recordid);
            }
        }

        public void updateroutingruleStatusequaltoone(EntityCollection Results, IOrganizationService service,int Recordprioritynumber)
        {
            if (Results.Entities.Count > 0)
            {
                foreach (Entity objrecode in Results.Entities)
                {
                    ObjrecordID = objrecode.Id;
                    ObjPriorityNumber = (int)objrecode.Attributes[RoutingRule.Priority];
                    ObjBasePriority = (int)objrecode.Attributes[RoutingRule.BasePriorityNumber];                   
                        if (Recordprioritynumber < ObjPriorityNumber)
                        {
                            ObjPriorityNumber = ObjPriorityNumber - 1;
                            ObjBasePriority = ObjBasePriority - 1;
                        UpdateRoutingrule(service, ObjPriorityNumber, ObjBasePriority, ObjrecordID);
                    }                     
                 }               
            }
        }
    }
}
