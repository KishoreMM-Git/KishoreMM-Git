using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using XRMExtensions;
using System.Linq;

namespace Xrm
{
    public class BasePluginContext : IExtendedPluginContext
    {
        #region Private members for lazy loading

        private readonly IServiceProvider _provider;
        private readonly IOrganizationServiceFactory _factory;
        private IPluginExecutionContext _pluginContext;
        private IServiceEndpointNotificationService _notification;
        private IOrganizationService _organizationService;
        private IOrganizationService _systemOrganizationService;
        private IOrganizationService _initiatedOrganizationService;
        private ITracingService _tracing;
        private List<Entity> _executionConfig;

        #endregion

        #region IPluginExecutionContext Properties

        public int Stage => PluginExecutionContext.Stage;
        public IPluginExecutionContext ParentContext => PluginExecutionContext.ParentContext;
        public int Mode => PluginExecutionContext.Mode;
        public int IsolationMode => PluginExecutionContext.IsolationMode;
        public int Depth => PluginExecutionContext.Depth;
        public string MessageName => PluginExecutionContext.MessageName;
        public string PrimaryEntityName => PluginExecutionContext.PrimaryEntityName;
        public Guid? RequestId => PluginExecutionContext.RequestId;
        public string SecondaryEntityName => PluginExecutionContext.SecondaryEntityName;
        public ParameterCollection InputParameters => PluginExecutionContext.InputParameters;
        public ParameterCollection OutputParameters => PluginExecutionContext.OutputParameters;
        public ParameterCollection SharedVariables => PluginExecutionContext.SharedVariables;
        public Guid UserId => PluginExecutionContext.UserId;
        public Guid InitiatingUserId => PluginExecutionContext.InitiatingUserId;
        public Guid BusinessUnitId => PluginExecutionContext.BusinessUnitId;
        public Guid OrganizationId => PluginExecutionContext.OrganizationId;
        public string OrganizationName => PluginExecutionContext.OrganizationName;
        public Guid PrimaryEntityId => PluginExecutionContext.PrimaryEntityId;
        public EntityImageCollection PreEntityImages => PluginExecutionContext.PreEntityImages;
        public EntityImageCollection PostEntityImages => PluginExecutionContext.PostEntityImages;
        public EntityReference OwningExtension => PluginExecutionContext.OwningExtension;
        public Guid CorrelationId => PluginExecutionContext.CorrelationId;
        public bool IsExecutingOffline => PluginExecutionContext.IsExecutingOffline;
        public bool IsOfflinePlayback => PluginExecutionContext.IsOfflinePlayback;
        public bool IsInTransaction => PluginExecutionContext.IsInTransaction;
        public Guid OperationId => PluginExecutionContext.OperationId;
        public DateTime OperationCreatedOn => PluginExecutionContext.OperationCreatedOn;

        #endregion

        #region IExtendedPluginContext Properties

        /// <summary>
        /// Fullname of the plugin the context is running against
        /// </summary>
        public string PluginTypeName { get; }

        /// <summary>
        /// Name of the plugin the context is running against
        /// </summary>
        public string PluginName { get; }

        /// <summary>
        /// Event the current plugin is executing for
        /// </summary>
        public RegisteredEvent Event { get; private set; }

        /// <summary>
        /// Pre Image alias name
        /// </summary>
        public string PreImageAlias => "PreImage";

        /// <summary>
        /// Post Image alias name
        /// </summary>
        public string PostImageAlias => "PostImage";

        /// <summary>
        /// Pipeline stage for the context
        /// </summary>
        public PipelineStage PipelineStage => (PipelineStage)Stage;

        /// <summary>
        /// IPluginExecutionContext contains information that describes the run-time environment in which the plug-in executes, 
        /// information related to the execution pipeline, and entity business information
        /// </summary>
        public IPluginExecutionContext PluginExecutionContext =>
            _pluginContext ?? (_pluginContext = (IPluginExecutionContext)_provider.GetService(typeof(IPluginExecutionContext)));

        /// <summary>
        /// Synchronous registered plug-ins can post the execution context to the Microsoft Azure Service Bus. <br/> 
        /// It is through this notification service that synchronous plug-ins can send brokered messages to the Microsoft Azure Service Bus
        /// </summary>
        public IServiceEndpointNotificationService NotificationService =>
            _notification ?? (_notification = (IServiceEndpointNotificationService)_provider.GetService(typeof(IServiceEndpointNotificationService)));

        /// <summary>
        /// Get a <see href="OrganizationRequest" /> object for the current plugin execution
        /// </summary>
        public T GetRequest<T>() where T : OrganizationRequest, new() => new T { Parameters = PluginExecutionContext.InputParameters };

        /// <summary>
        /// Get a <see href="OrganizationResponse" /> object for the current plugin execution
        /// </summary>
        public T GetResponse<T>() where T : OrganizationResponse, new() => new T { Results = PluginExecutionContext.OutputParameters };

        #endregion

        #region IExtendedExecutionContext Properties

        /// <summary>
        /// Provides logging run-time trace information for plug-ins
        /// </summary>
        public ITracingService TracingService => _tracing ?? (_tracing = (ITracingService)_provider.GetService(typeof(ITracingService)));

        /// <summary>
        /// <see cref="IOrganizationService"/> using the user from the plugin context
        /// </summary>
        public IOrganizationService OrganizationService => _organizationService ?? (_organizationService = CreateOrganizationService(UserId));

        /// <summary>
        /// <see cref="IOrganizationService"/> using the SYSTEM user
        /// </summary>
        public IOrganizationService SystemOrganizationService =>
            _systemOrganizationService ?? (_systemOrganizationService = CreateOrganizationService(null));

        /// <summary>
        /// <see cref="IOrganizationService"/> using the initiating user from the plugin context
        /// </summary>
        public IOrganizationService InitiatingUserOrganizationService =>
            _initiatedOrganizationService ?? (_initiatedOrganizationService = CreateOrganizationService(InitiatingUserId));

        /// <summary>
        /// Primary entity from the context as an entity reference
        /// </summary>
        public EntityReference PrimaryEntity => new EntityReference(PrimaryEntityName, PrimaryEntityId);

        #endregion

        /// <summary>
        /// Helper object that stores the services available in this plug-in
        /// </summary>
        /// <param name="serviceProvider">IServiceProvider</param>
        /// <param name="events">List of <see href="RegisteredEvents" /> for the plugin</param>
        /// <param name="plugin">Plugin handler</param>
        public BasePluginContext(IServiceProvider serviceProvider, IEnumerable<RegisteredEvent> events, IBasePlugin plugin)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            _provider = serviceProvider;

            // Obtain the organization factory service from the service provider
            _factory = (IOrganizationServiceFactory)_provider.GetService(typeof(IOrganizationServiceFactory));

            // Set Event
            Event = PluginExecutionContext.GetEvent(events);

            var pluginType = plugin.GetType();
            PluginTypeName = pluginType.FullName;
            PluginName = pluginType.Name;
        }

        #region IOrganizationService methods

        public Guid Create(Entity entity)
        {
            return OrganizationService.Create(entity);
        }

        public Entity Retrieve(string entityName, Guid id, ColumnSet columnSet)
        {
            return OrganizationService.Retrieve(entityName, id, columnSet);
        }

        public void Update(Entity entity)
        {
            OrganizationService.Update(entity);
        }

        public void Delete(string entityName, Guid id)
        {
            OrganizationService.Delete(entityName, id);
        }

        public OrganizationResponse Execute(OrganizationRequest request)
        {
            return OrganizationService.Execute(request);
        }

        public void Associate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            OrganizationService.Associate(entityName, entityId, relationship, relatedEntities);
        }

        public void Disassociate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            OrganizationService.Disassociate(entityName, entityId, relationship, relatedEntities);
        }

        public EntityCollection RetrieveMultiple(QueryBase query)
        {
            return OrganizationService.RetrieveMultiple(query);
        }

        #endregion

        /// <summary>
        /// Prevent plugin from running multiple times for the same context
        /// </summary>
        public bool IsDuplicatePluginExecution()
        {
            // Delete message can't be called twice so can ignore
            if (Event.MessageName == "Delete")
            {
                return false;
            }

            var key = $"{PluginTypeName}|{MessageName}|{PipelineStage.ToString()}|{PrimaryEntityId}";

            // Check if key exists in shared variables
            if (this.GetSharedVariable<bool>(key) == true)
            {
                return true;
            }

            // Add key to shared variables
            SharedVariables.Add(key, true);

            return false;
        }

        /// <summary>
        /// Create an instance of <see cref="IOrganizationService"/> with provided user id
        /// </summary>
        /// <param name="userId">User id to use</param>
        /// <returns>IOrganizationService</returns>
        public IOrganizationService CreateOrganizationService(Guid? userId)
        {
            return _factory.CreateOrganizationService(userId);
        }

        /// <summary>
        /// Writes a trace message to the CRM trace log.
        /// </summary>
        /// <param name="format">Message format</param>
        /// <param name="args">Message format arguments</param>
        public void Trace(string format, params object[] args)
        {
            TracingService.Trace(format, args);
        }

        /// <summary>
        /// Read config value from executing plugin configuration
        /// </summary>
        /// <param name="configKey"></param>
        /// <param name="configSetupName"></param>
        /// <returns></returns>
        public T GetConfigValue<T>(string configKey, string configSetupName = "")
        {
            if (string.IsNullOrWhiteSpace(configSetupName))
            {
                configSetupName = PluginName;
            }

            // read plugin configuration
            if(_executionConfig == null)
            {
                _executionConfig = ExecutionConfig.ReadExecutionConfig(SystemOrganizationService, configSetupName);
            }

            return ExecutionConfig.ReadConfigValue<T>(_executionConfig, configKey, configSetupName);
        }
    }
}
