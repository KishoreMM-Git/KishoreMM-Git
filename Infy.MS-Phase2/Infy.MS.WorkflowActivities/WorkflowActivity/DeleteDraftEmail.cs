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

namespace Infy.MS.WorkflowActivities.WorkflowActivity
{
    public class DeleteDraftEmail : BaseWorkflowActivity
    {
        public override void ExecuteWorkflowActivity(IExtendedWorkflowContext workflowContext)
        {
            try
            {
                if (workflowContext == null)
                {
                    throw new InvalidPluginExecutionException("Context not Found");
                }
                Entity email = (Entity)workflowContext.InputParameters["Target"];
                workflowContext.Delete(email);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }

        }
    }
}
