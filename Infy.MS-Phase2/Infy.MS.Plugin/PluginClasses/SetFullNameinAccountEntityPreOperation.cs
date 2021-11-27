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

namespace Infy.MS.Plugins
{
    public class SetFullNameinAccountEntityPreOperation : BasePlugin

    {
        Entity Target = null;
        string Fullname = string.Empty;
        string firstname = string.Empty;
        string lastname = string.Empty;
        public override void ExecutePlugin(IExtendedPluginContext context)
        {

            if (context == null)
            {
                throw new InvalidPluginExecutionException("Context not Found");
            }
            if (context.MessageName.ToLower() == "create")
            {
                Target = context.GetTargetEntity<Entity>();
                Setfullname(Target, context);
            }
            if (context.MessageName.ToLower() == "update")
            {
                Entity PostImage = (Entity)context.PostEntityImages["PostImage"];
                Setfullname(PostImage, context);
            }
        }

        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Asynchronous, "Create");
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Asynchronous, "Update");
        }


        public void Setfullname(Entity Target, IExtendedPluginContext context)
        {
            if (Target.Contains(Account.FirstName) || Target.Contains(Account.LastName))
            {

                if (Target.Contains(Account.FirstName))
                {
                    firstname = Target.GetAttributeValue<string>(Account.FirstName);
                }
                if (Target.Contains(Account.LastName))
                {
                    lastname = Target.GetAttributeValue<string>(Account.LastName);
                }
                Fullname = string.Join(" ", firstname, lastname);

                if (context.MessageName.ToLower() == "create")
                {
                    Entity accountEntity = new Entity(Account.EntityName);
                    accountEntity.Id = Target.Id;
                    accountEntity[Account.PrimaryName] = Fullname;
                    context.Update(accountEntity);
                }
                if (context.MessageName.ToLower() == "update")
                {
                    if (Target.LogicalName == Account.EntityName)
                    {
                        Entity Leadentity = new Entity(Account.EntityName);
                        Leadentity.Id = Target.Id;
                        Leadentity[Account.PrimaryName] = Fullname;
                        context.Update(Leadentity);
                    }
                }
            }
        }
    }
}
