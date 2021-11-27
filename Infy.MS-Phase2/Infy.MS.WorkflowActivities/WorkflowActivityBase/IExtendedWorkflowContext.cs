
using Microsoft.Xrm.Sdk.Workflow;
using System.Activities;

namespace Xrm
{
    public interface IExtendedWorkflowContext: IWorkflowContext, IExtendedExecutionContext
    {
        /// <summary>
        /// Fullname of the workflow activity
        /// </summary>
        string WorkflowActivityTypeName { get; }

        /// <summary>
        /// Name of the workflow activity
        /// </summary>
        string WorkflowActivityName { get; }

        /// <summary>
        /// Extends ActivityContext and provides additional functionality for CodeActivity
        /// </summary>
        CodeActivityContext ActivityContext { get; }

        /// <summary>
        /// Read config value from executing plugin configuration
        /// </summary>
        /// <param name="configKey"></param>
        /// <param name="configSetupName"></param>
        /// <returns></returns>
        T GetConfigValue<T>(string configKey, string configSetupName = "");
    }
}