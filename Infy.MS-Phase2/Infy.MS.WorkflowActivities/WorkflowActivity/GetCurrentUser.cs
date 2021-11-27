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
using System.Activities;
using Microsoft.Xrm.Sdk.Workflow;

namespace Infy.MS.WorkflowActivities
{
   public class GetCurrentUser : BaseWorkflowActivity
    {
        [Output("Current User")]
        [ReferenceTarget("systemuser")]
        public OutArgument<EntityReference> CurrentUser { get; set; }

        public override void ExecuteWorkflowActivity(IExtendedWorkflowContext context)
        {
            IWorkflowContext workflowContext = context.ActivityContext.GetExtension<IWorkflowContext>();
            CheckUser(workflowContext.InitiatingUserId, context);
            CurrentUser.Set(context.ActivityContext, new EntityReference("systemuser", workflowContext.InitiatingUserId));
        }

        public void CheckUser(Guid userId,IExtendedWorkflowContext extendedWorkflowContext)
        {
            var user = extendedWorkflowContext.Retrieve("systemuser", userId, new ColumnSet("firstname", "lastname"));
            if(user!=null)
            {
                if(!user.Contains("firstname") && user.Contains("lastname"))
                {
                    if(user.GetAttributeValue<string>("lastname").Equals("System",StringComparison.OrdinalIgnoreCase))
                    {
                        return;
                    }
                }
            }
        }
    }
}
