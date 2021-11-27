namespace Xrm
{
    /// <summary>
    /// Pipeline stage of the Plugin
    /// </summary>
    public enum PipelineStage
    {
        /// <summary>
        /// Pre validation: 10
        /// </summary>
        PreValidation = 10,

        /// <summary>
        /// Pre operation: 20
        /// </summary>
        PreOperation = 20,

        /// <summary>
        /// Post operation: 40
        /// </summary>
        PostOperation = 40
    }

    /// <summary>
    /// Mode of the plugin execution
    /// </summary>
    public enum SdkMessageProcessingStepMode
    {
        /// <summary>
        /// Synchronous: 0
        /// </summary>
        Synchronous = 0,

        /// <summary>
        /// Asynchronous: 1
        /// </summary>
        Asynchronous = 1,
    }
}