
namespace XRMExtensions.OptionSets
{
    public enum AccountAccountCategoryCode
    {
        PreferredCustomer = 1,
        Standard = 2,
    }
    public enum AccountAccountClassificationCode
    {
        DefaultValue = 1,
    }
    public enum AccountAccountRatingCode
    {
        DefaultValue = 1,
    }
    public enum AccountAddress1_AddressTypeCode
    {
        BillTo = 1,
        Other = 4,
        Primary = 3,
        ShipTo = 2,
    }
    public enum AccountAddress1_FreightTermsCode
    {
        FOB = 1,
        NoCharge = 2,
    }
    public enum AccountAddress1_ShippingMethodCode
    {
        Airborne = 1,
        DHL = 2,
        FedEx = 3,
        FullLoad = 6,
        PostalMail = 5,
        UPS = 4,
        WillCall = 7,
    }
    public enum AccountAddress2_AddressTypeCode
    {
        DefaultValue = 1,
    }
    public enum AccountAddress2_FreightTermsCode
    {
        DefaultValue = 1,
    }
    public enum AccountAddress2_ShippingMethodCode
    {
        DefaultValue = 1,
    }
    public enum AccountBusinessTypeCode
    {
        DefaultValue = 1,
    }
    public enum AccountCustomerSizeCode
    {
        DefaultValue = 1,
    }
    public enum AccountCustomerTypeCode
    {
        Competitor = 1,
        Consultant = 2,
        Customer = 3,
        Influencer = 6,
        Investor = 4,
        Other = 12,
        Partner = 5,
        Press = 7,
        Prospect = 8,
        Reseller = 9,
        Supplier = 10,
        Vendor = 11,
    }
    public enum AccountIndustryCode
    {
        Accounting = 1,
        AgricultureandNonpetrolNaturalResourceExtraction = 2,
        BroadcastingPrintingandPublishing = 3,
        Brokers = 4,
        BuildingSupplyRetail = 5,
        BusinessServices = 6,
        Consulting = 7,
        ConsumerServices = 8,
        DesignDirectionandCreativeManagement = 9,
        DistributorsDispatchersandProcessors = 10,
        DoctorsOfficesandClinics = 11,
        DurableManufacturing = 12,
        EatingandDrinkingPlaces = 13,
        EntertainmentRetail = 14,
        EquipmentRentalandLeasing = 15,
        Financial = 16,
        FoodandTobaccoProcessing = 17,
        InboundCapitalIntensiveProcessing = 18,
        InboundRepairandServices = 19,
        Insurance = 20,
        LegalServices = 21,
        NonDurableMerchandiseRetail = 22,
        OutboundConsumerService = 23,
        PetrochemicalExtractionandDistribution = 24,
        ServiceRetail = 25,
        SIGAffiliations = 26,
        SocialServices = 27,
        SpecialOutboundTradeContractors = 28,
        SpecialtyRealty = 29,
        Transportation = 30,
        UtilityCreationandDistribution = 31,
        VehicleRetail = 32,
        Wholesale = 33,
    }
    public enum AccountOwnershipCode
    {
        Other = 4,
        Private = 2,
        Public = 1,
        Subsidiary = 3,
    }
    public enum AccountPaymentTermsCode
    {
        _210Net30 = 2,
        Net30 = 1,
        Net45 = 3,
        Net60 = 4,
    }
    public enum AccountPreferredAppointmentDayCode
    {
        Friday = 5,
        Monday = 1,
        Saturday = 6,
        Sunday = 0,
        Thursday = 4,
        Tuesday = 2,
        Wednesday = 3,
    }
    public enum AccountPreferredAppointmentTimeCode
    {
        Afternoon = 2,
        Evening = 3,
        Morning = 1,
    }
    public enum AccountPreferredContactMethodCode
    {
        Any = 1,
        Email = 2,
        Fax = 4,
        Mail = 5,
        Phone = 3,
    }
    public enum AccountShippingMethodCode
    {
        DefaultValue = 1,
    }
    public enum AccountStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum AccountTerritoryCode
    {
        DefaultValue = 1,
    }
    public enum ACIViewMapperStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum ActionCardSource
    {
        CRM = 1,
        Exchange = 2,
    }
    public enum ActionCardState
    {
        Active = 0,
        Completed = 2,
        Dismissed = 1,
    }
    public enum ActionCardUserStateState
    {
        Active = 0,
        Completed = 2,
        Dismissed = 1,
    }
    public enum activity_mailmergetypecode
    {
        Appointment = 4201,
        Email = 4202,
        EmailviaMailMerge = 42020,
        Fax = 4204,
        FaxviaMailMerge = 42040,
        Letter = 4207,
        LetterviaMailMerge = 42070,
        PhoneCall = 4210,
    }
    public enum activity_typecode
    {
        Appointment = 4201,
        Email = 4202,
        Fax = 4204,
        Letter = 4207,
        PhoneCall = 4210,
    }
    public enum ActivityPartyInstanceTypeCode
    {
        NotRecurring = 0,
        RecurringException = 3,
        RecurringFutureException = 4,
        RecurringInstance = 2,
        RecurringMaster = 1,
    }
    public enum ActivityPartyParticipationTypeMask
    {
        BCCRecipient = 4,
        CCRecipient = 3,
        Customer = 11,
        Optionalattendee = 6,
        Organizer = 7,
        Owner = 9,
        Regarding = 8,
        Requiredattendee = 5,
        Resource = 10,
        Sender = 1,
        ToRecipient = 2,
    }
    public enum activitypointer_ActivityTypeCode
    {
        Appointment = 4201,
        CampaignActivity = 4402,
        CampaignResponse = 4401,
        CaseResolution = 4206,
        Email = 4202,
        Fax = 4204,
        FormsProsurveyinvite = 10049,
        FormsProsurveyresponse = 10050,
        Letter = 4207,
        OpportunityClose = 4208,
        OrderClose = 4209,
        PhoneCall = 4210,
        QuickCampaign = 4406,
        QuoteClose = 4211,
        RecurringAppointment = 4251,
        ServiceActivity = 4214,
        SurveyActivity = 10094,
        Task = 4212,
    }
    public enum activitypointer_DeliveryPriorityCode
    {
        High = 2,
        Low = 0,
        Normal = 1,
    }
    public enum ActivityPointerInstanceTypeCode
    {
        NotRecurring = 0,
        RecurringException = 3,
        RecurringFutureException = 4,
        RecurringInstance = 2,
        RecurringMaster = 1,
    }
    public enum ActivityPointerPriorityCode
    {
        High = 2,
        Low = 0,
        Normal = 1,
    }
    public enum ActivityPointerStatusCode
    {
        Canceled = 3,
        Completed = 2,
        Open = 1,
        Scheduled = 4,
    }
    public enum addlistcampaign
    {
        Tothecampaignandallundistributedcampaignactivities = 1,
        Tothecampaignonly = 0,
    }
    public enum adminsettingsentityStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum AdvancedSimilarityRuleFilterResultByStatus
    {
        Active = 0,
        Inactive = 1,
    }
    public enum AdvancedSimilarityRuleStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum AllocationType
    {
        Numberofcases = 0,
        Numberofhours = 1,
    }
    public enum AppConfigStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum AppModuleComponentComponentType
    {
        BusinessProcessFlows = 29,
        Charts = 59,
        Command_RibbonforFormsGridssubgrids = 48,
        Entities = 1,
        Forms = 60,
        Sitemap = 62,
        Views = 26,
    }
    public enum AppModuleComponentRootComponentBehavior
    {
        Donotincludesubcomponents = 1,
        IncludeAsShellOnly = 2,
        IncludeSubcomponents = 0,
    }
    public enum AppModuleNavigationType
    {
        Multisession = 1,
        Singlesession = 0,
    }
    public enum AppointmentAttachmentErrors
    {
        None = 0,
        TheappointmentwassavedasaMicrosoftDynamics365appointmentrecordbutnotalltheattachmentscouldbesavedwithitAnattachmentcannotbesavedifitisblockedorifitsfiletypeisinvalid = 1,
    }
    public enum AppointmentInstanceTypeCode
    {
        NotRecurring = 0,
        RecurringException = 3,
        RecurringFutureException = 4,
        RecurringInstance = 2,
        RecurringMaster = 1,
    }
    public enum AppointmentPriorityCode
    {
        High = 2,
        Low = 0,
        Normal = 1,
    }
    public enum AppointmentStatusCode
    {
        Busy = 5,
        Canceled = 4,
        Completed = 3,
        Free = 1,
        OutofOffice = 6,
        Tentative = 2,
    }
    public enum AsyncOperationOperationType
    {
        ActivityPropagation = 6,
        ALMAnomalyDetectionOperation = 73,
        AppModuleMetadataOperation = 72,
        AuditPartitionCreation = 41,
        BulkDelete = 13,
        BulkDeleteFileAttachment = 94,
        BulkDeleteSubprocess = 23,
        BulkDuplicateDetection = 8,
        BulkEmail = 2,
        CalculateOrganizationMaximumStorageSize = 22,
        CalculateOrganizationStorageSize = 18,
        CalculateRollupField = 57,
        CallbackRegistrationExpanderOperation = 79,
        CascadeAssign = 90,
        CascadeDelete = 91,
        CheckForLanguagePackUpdates = 42,
        Cleanupinactiveworkflowassemblies = 32,
        CleanupSolutionComponents = 71,
        CollectionOrganizationSizeStatistics = 20,
        CollectOrganizationDatabaseStatistics = 19,
        CollectOrganizationStatistics = 16,
        ConvertDateAndTimeBehavior = 62,
        Databaselogbackup = 26,
        DatabaseTuning = 21,
        DBCCSHRINKDATABASEmaintenancejob = 28,
        DBCCSHRINKFILEmaintenancejob = 29,
        DeletionService = 14,
        DuplicateDetectionRulePublish = 7,
        EncryptionHealthCheck = 53,
        EntityKeyIndexCreation = 63,
        EventExpanderOperation = 92,
        ExecuteAsyncRequest = 54,
        FlowNotification = 75,
        GoalRollUp = 40,
        Import = 5,
        ImportFileParse = 3,
        ImportSampleData = 38,
        ImportSolutionMetadata = 93,
        ImportSubprocess = 17,
        ImportTranslation = 59,
        IncomingEmailProcessing = 51,
        IndexManagement = 15,
        MailboxTestAccess = 52,
        MassCalculateRollupField = 58,
        MatchcodeUpdate = 12,
        OrganizationFullTextCatalogIndex = 25,
        OutgoingActivity = 50,
        PosttoYammer = 49,
        ProvisionLanguagePack = 43,
        QuickCampaign = 11,
        RecurringSeriesExpansion = 35,
        RefreshBusinessUnitforRecordsOwnedByPrincipal = 95,
        RegenerateEntityRowCountSnapshotData = 46,
        RegenerateReadShareSnapshotData = 47,
        Reindexallindicesmaintenancejob = 30,
        RelationshipAssistantCards = 69,
        ResourceBookingSync = 68,
        RevokeInheritedAccess = 96,
        RibbonClientMetadataOperation = 76,
        SQMDataCollection = 9,
        StorageLimitNotification = 31,
        SystemEvent = 1,
        TransformParseData = 4,
        UpdateContractStates = 27,
        UpdateEntitlementStates = 56,
        UpdateKnowledgeArticleStates = 65,
        UpdateOrganizationDatabase = 44,
        UpdateSolution = 45,
        UpdateStatisticIntervals = 24,
        Workflow = 10,
    }
    public enum AsyncOperationStatusCode
    {
        Canceled = 32,
        Canceling = 22,
        Failed = 31,
        InProgress = 20,
        Pausing = 21,
        Succeeded = 30,
        Waiting = 10,
        WaitingForResources = 0,
    }
    public enum AttributeImageConfigStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum AuditAction
    {
        Activate = 4,
        AddItem = 37,
        AddMember = 31,
        AddMembers = 35,
        AddPrivilegestoRole = 57,
        AddSubstitute = 39,
        AddToQueue = 52,
        Approve = 28,
        Assign = 13,
        AssignRoleToTeam = 53,
        AssignRoleToUser = 55,
        AssociateEntities = 33,
        AttributeAuditStarted = 106,
        AttributeAuditStopped = 109,
        AuditChangeatAttributeLevel = 103,
        AuditChangeatEntityLevel = 102,
        AuditChangeatOrgLevel = 104,
        AuditDisabled = 110,
        AuditEnabled = 107,
        AuditLogDeletion = 111,
        Book = 50,
        Cancel = 17,
        Cascade = 11,
        Clone = 61,
        Close = 16,
        Complete = 18,
        Create = 1,
        Deactivate = 5,
        Delete = 3,
        DeleteAttribute = 101,
        DeleteEntity = 100,
        DisassociateEntities = 34,
        Disqualify = 25,
        Enabledfororganization = 63,
        EntityAuditStarted = 105,
        EntityAuditStopped = 108,
        Fulfill = 22,
        GenerateQuoteFromOpportunity = 51,
        Hold = 30,
        ImportMappings = 60,
        InternalProcessing = 46,
        Invoice = 29,
        Lose = 45,
        Merge = 12,
        ModifyShare = 48,
        Paid = 23,
        Qualify = 24,
        Reject = 27,
        RemoveItem = 38,
        RemoveMember = 32,
        RemoveMembers = 36,
        RemovePrivilegesFromRole = 58,
        RemoveRoleFromTeam = 54,
        RemoveRoleFromUser = 56,
        RemoveSubstitute = 40,
        Renew = 42,
        Reopen = 21,
        ReplacePrivilegesInRole = 59,
        Reschedule = 47,
        Resolve = 20,
        Retrieve = 15,
        Revise = 43,
        SendDirectEmail = 62,
        SetState = 41,
        Share = 14,
        Submit = 26,
        Unknown = 0,
        Unshare = 49,
        Update = 2,
        UserAccessAuditStarted = 112,
        UserAccessAuditStopped = 113,
        UserAccessviaWeb = 64,
        UserAccessviaWebServices = 65,
        Win = 44,
    }
    public enum AuditOperation
    {
        Access = 4,
        Create = 1,
        Delete = 3,
        Update = 2,
    }
    public enum AzureServiceConnectionConnectionType
    {
        Recommendation = 1,
        TextAnalytics = 2,
    }
    public enum AzureServiceConnectionLastConnectionStatusCode
    {
        Failure = 2,
        Success = 1,
    }
    public enum AzureServiceConnectionStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum BookableResourceBookingBookingType
    {
        Liquid = 2,
        Solid = 1,
    }
    public enum BookableResourceBookingExchangeSyncIdMappingSyncStatus
    {
        Completed = 0,
        Pending = 2,
        Retry = 1,
    }
    public enum BookableResourceBookingHeaderStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum BookableResourceBookingStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum BookableResourceCategoryAssnStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum BookableResourceCategoryStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum BookableResourceCharacteristicStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum BookableResourceCharacteristicType
    {
        Certification = 2,
        Skill = 1,
    }
    public enum BookableResourceGroupStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum BookableResourceResourceType
    {
        Account = 5,
        Contact = 2,
        Crew = 6,
        Equipment = 4,
        Facility = 7,
        Generic = 1,
        Pool = 8,
        User = 3,
    }
    public enum BookableResourceStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum BookingStatusStatus
    {
        Canceled = 3,
        Committed = 2,
        Proposed = 1,
    }
    public enum BookingStatusStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum BudgetStatus
    {
        CanBuy = 2,
        MayBuy = 1,
        NoCommittedBudget = 0,
        WillBuy = 3,
    }
    public enum BulkDeleteOperationStatusCode
    {
        Canceled = 32,
        Canceling = 22,
        Failed = 31,
        InProgress = 20,
        Paused = 12,
        Pausing = 21,
        Retrying = 11,
        Succeeded = 30,
        Waiting = 10,
        WaitingForResources = 0,
    }
    public enum bulkemail_recipients
    {
        Allrecordsonallpages = 3,
        Allrecordsoncurrentpage = 2,
        Selectedrecordsoncurrentpage = 1,
    }
    public enum BulkOperationCreatedRecordTypeCode
    {
        Appointment = 5,
        Email = 4,
        Fax = 2,
        Letter = 3,
        PhoneCall = 1,
        SendDirectEmail = 6,
    }
    public enum BulkOperationInstanceTypeCode
    {
        NotRecurring = 0,
        RecurringException = 3,
        RecurringFutureException = 4,
        RecurringInstance = 2,
        RecurringMaster = 1,
    }
    public enum BulkOperationOperationTypeCode
    {
        CreateActivities = 2,
        CreateOpportunities = 1,
        Distribute = 4,
        Execute = 5,
        QuickCampaign = 7,
        SendDirectMail = 3,
    }
    public enum BulkOperationPriorityCode
    {
        High = 2,
        Low = 0,
        Normal = 1,
    }
    public enum BulkOperationStatusCode
    {
        Aborted = 3,
        Canceled = 5,
        Completed = 4,
        InProgress = 2,
        Pending = 1,
    }
    public enum BulkOperationTargetedRecordTypeCode
    {
        Account = 1,
        Contact = 2,
        Lead = 4,
    }
    public enum BusinessProcessFlowInstanceStatusCode
    {
        Aborted = 3,
        Active = 1,
        Finished = 2,
    }
    public enum BusinessUnitAddress1_AddressTypeCode
    {
        DefaultValue = 1,
    }
    public enum BusinessUnitAddress1_ShippingMethodCode
    {
        DefaultValue = 1,
    }
    public enum BusinessUnitAddress2_AddressTypeCode
    {
        DefaultValue = 1,
    }
    public enum BusinessUnitAddress2_ShippingMethodCode
    {
        DefaultValue = 1,
    }
    public enum BusinessUnitNewsArticleArticleTypeCode
    {
        AllUsers = 1,
        SalesUsers = 2,
        ServiceUsers = 3,
    }
    public enum CalendarType
    {
        CustomerService = 1,
        Default = 0,
        HolidaySchedule = 2,
        InnerCalendartype = -1,
    }
    public enum CallbackRegistrationMessage
    {
        Create = 1,
        CreateorDelete = 5,
        CreateorUpdate = 4,
        CreateorUpdateorDelete = 7,
        Delete = 2,
        Update = 3,
        UpdateorDelete = 6,
    }
    public enum CallbackRegistrationScope
    {
        BusinessUnit = 2,
        Organization = 4,
        ParentChildBusinessUnit = 3,
        User = 1,
    }
    public enum CallbackRegistrationVersion
    {
        V1 = 1,
    }
    public enum CampaignActivityChannelTypeCode
    {
        Appointment = 2,
        Email = 7,
        EmailviaMailMerge = 8,
        Fax = 5,
        FaxviaMailMerge = 6,
        Letter = 3,
        LetterviaMailMerge = 4,
        Other = 9,
        Phone = 1,
    }
    public enum CampaignActivityInstanceTypeCode
    {
        NotRecurring = 0,
        RecurringException = 3,
        RecurringFutureException = 4,
        RecurringInstance = 2,
        RecurringMaster = 1,
    }
    public enum CampaignActivityPriorityCode
    {
        High = 2,
        Low = 0,
        Normal = 1,
    }
    public enum CampaignActivityStatusCode
    {
        Canceled = 3,
        Closed = 2,
        Completed = 6,
        InProgress = 0,
        Pending = 4,
        Proposed = 1,
        SystemAborted = 5,
    }
    public enum CampaignActivityTypeCode
    {
        ContentDistribution = 5,
        ContentPreparation = 2,
        DirectFollowUpContact = 7,
        DirectInitialContact = 6,
        LeadQualification = 4,
        ReminderDistribution = 8,
        Research = 1,
        TargetMarketingListCreation = 3,
    }
    public enum CampaignResponseChannelTypeCode
    {
        Appointment = 5,
        Email = 1,
        Fax = 3,
        Letter = 4,
        Others = 6,
        Phone = 2,
    }
    public enum CampaignResponseInstanceTypeCode
    {
        NotRecurring = 0,
        RecurringException = 3,
        RecurringFutureException = 4,
        RecurringInstance = 2,
        RecurringMaster = 1,
    }
    public enum CampaignResponsePriorityCode
    {
        High = 2,
        Low = 0,
        Normal = 1,
    }
    public enum CampaignResponseResponseCode
    {
        DoNotSendMarketingMaterials = 3,
        Error = 4,
        Interested = 1,
        NotInterested = 2,
    }
    public enum CampaignResponseStatusCode
    {
        Canceled = 3,
        Closed = 2,
        Open = 1,
    }
    public enum CampaignStatusCode
    {
        Canceled = 4,
        Completed = 3,
        Inactive = 6,
        Launched = 2,
        Proposed = 0,
        ReadyToLaunch = 1,
        Suspended = 5,
    }
    public enum CampaignTypeCode
    {
        Advertisement = 1,
        Cobranding = 4,
        DirectMarketing = 2,
        Event = 3,
        Other = 5,
    }
    public enum CardTypeClientAvailability
    {
        MocaAndWeb = 3,
        MocaOnly = 2,
        WebClientOnly = 1,
    }
    public enum cascadeCaseClosurePreference
    {
        Closeallchildcaseswhenparentcaseisclosed = 0,
        Dontallowparentcaseclosureuntilallchildcasesareclosed = 1,
    }
    public enum ChannelAccessProfileRuleStatusCode
    {
        Active = 2,
        Draft = 1,
    }
    public enum ChannelAccessProfileStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum ChannelPropertyDataType
    {
        FloatingPointNumber = 0,
        SingleLineOfText = 1,
        WholeNumber = 2,
    }
    public enum ChannelPropertyGroupStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum ChannelPropertyStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum CharacteristicStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum ColumnMappingProcessCode
    {
        Ignore = 2,
        Internal = 3,
        Process = 1,
    }
    public enum ColumnMappingStatusCode
    {
        Active = 1,
    }
    public enum CommitmentStatusCode
    {
    }
    public enum CompetitorAddress1_AddressTypeCode
    {
        DefaultValue = 1,
    }
    public enum CompetitorAddress1_ShippingMethodCode
    {
        DefaultValue = 1,
    }
    public enum CompetitorAddress2_AddressTypeCode
    {
        DefaultValue = 1,
    }
    public enum CompetitorAddress2_ShippingMethodCode
    {
        DefaultValue = 1,
    }
    public enum ComponentState
    {
        Deleted = 2,
        DeletedUnpublished = 3,
        Published = 0,
        Unpublished = 1,
    }
    public enum ComponentType
    {
        AIConfiguration = 402,
        AIProject = 401,
        AIProjectType = 400,
        Attachment = 35,
        Attribute = 2,
        AttributeImageConfiguration = 431,
        AttributeLookupValue = 5,
        AttributeMap = 47,
        AttributePicklistValue = 4,
        CanvasApp = 300,
        ComplexControl = 64,
        ConnectionRole = 63,
        Connector_371 = 371,
        Connector_372 = 372,
        ContractTemplate = 37,
        ConvertRule = 154,
        ConvertRuleItem = 155,
        CustomControl = 66,
        CustomControlDefaultConfig = 68,
        DataSourceMapping = 166,
        DisplayString = 22,
        DisplayStringMap = 23,
        DuplicateRule = 44,
        DuplicateRuleCondition = 45,
        EmailTemplate = 36,
        Entity = 1,
        EntityAnalyticsConfiguration = 430,
        EntityImageConfiguration = 432,
        EntityKey = 14,
        EntityMap = 46,
        EntityRelationship = 10,
        EntityRelationshipRelationships = 12,
        EntityRelationshipRole = 11,
        EnvironmentVariableDefinition = 380,
        EnvironmentVariableValue = 381,
        FieldPermission = 71,
        FieldSecurityProfile = 70,
        Form = 24,
        HierarchyRule = 65,
        ImportMap = 208,
        Index = 18,
        KBArticleTemplate = 38,
        LocalizedLabel = 7,
        MailMergeTemplate = 39,
        ManagedProperty = 13,
        MobileOfflineProfile = 161,
        MobileOfflineProfileItem = 162,
        OptionSet = 9,
        Organization = 25,
        PluginAssembly = 91,
        PluginType = 90,
        Privilege = 16,
        PrivilegeObjectTypeCode = 17,
        Relationship = 3,
        RelationshipExtraCondition = 8,
        Report = 31,
        ReportCategory = 33,
        ReportEntity = 32,
        ReportVisibility = 34,
        RibbonCommand = 48,
        RibbonContextGroup = 49,
        RibbonCustomization = 50,
        RibbonDiff = 55,
        RibbonRule = 52,
        RibbonTabToCommandMap = 53,
        Role = 20,
        RolePrivilege = 21,
        RoutingRule = 150,
        RoutingRuleItem = 151,
        SavedQuery = 26,
        SavedQueryVisualization = 59,
        SDKMessage = 201,
        SDKMessageFilter = 202,
        SdkMessagePair = 203,
        SDKMessageProcessingStep = 92,
        SDKMessageProcessingStepImage = 93,
        SdkMessageRequest = 204,
        SdkMessageRequestField = 205,
        SdkMessageResponse = 206,
        SdkMessageResponseField = 207,
        ServiceEndpoint = 95,
        SimilarityRule = 165,
        SiteMap = 62,
        SLA = 152,
        SLAItem = 153,
        SystemForm = 60,
        ViewAttribute = 6,
        WebResource = 61,
        WebWizard = 210,
        Workflow = 29,
    }
    public enum ConnectionRecord1ObjectTypeCode
    {
        Account = 1,
        Activity = 4200,
        Appointment = 4201,
        Appointmentactivitymarketingtemplate = 10162,
        Campaign = 4400,
        CampaignActivity = 4402,
        Case = 112,
        ChannelAccessProfileRule = 9400,
        Competitor = 123,
        Contact = 2,
        Contentsettings = 10165,
        Contract = 1010,
        Dynamiccontentmetadata = 10179,
        Email = 4202,
        Entitlement = 9700,
        EntitlementChannel = 9701,
        EntitlementTemplateChannel = 9703,
        FacilityEquipment = 4000,
        Fax = 4204,
        FormsProsurveyinvite = 10049,
        FormsProsurveyresponse = 10050,
        Goal = 9600,
        Invoice = 1090,
        KnowledgeArticle = 9953,
        KnowledgeBaseRecord = 9930,
        Lead = 4,
        LeadScoringModel = 10215,
        Letter = 4207,
        LinkedInaccount = 10220,
        LinkedInactivity = 10221,
        LinkedInfieldmapping = 10224,
        LinkedInformquestion = 10227,
        LinkedInformsubmissionanswer = 10226,
        LinkedInLeadGenform = 10225,
        LinkedInLeadGenformsubmission = 10228,
        LinkedInleadmatchingstrategy = 10229,
        LinkedInuserprofile = 10230,
        Loan = 3,
        Marketingactivity = 10193,
        Marketingemaildynamiccontentmetadata = 10181,
        Marketingemailtestsend = 10183,
        Marketinglist = 4300,
        Order = 1088,
        PhoneCall = 4210,
        Phonecallactivitymarketingtemplate = 10194,
        Position = 50,
        PriceList = 1022,
        ProcessSession = 4710,
        Product = 1024,
        ProfileAlbum = 10040,
        Quote = 1084,
        RecurringAppointment = 4251,
        ResourceGroup = 4007,
        ResponseAction = 10086,
        ResponseError = 10089,
        SchedulingGroup = 4005,
        ServiceActivity = 4214,
        SocialActivity = 4216,
        SocialProfile = 99,
        SurveyActivity = 10094,
        SurveyResponse = 10096,
        Task = 4212,
        Taskactivitymarketingtemplate = 10199,
        Team = 9,
        Territory = 2013,
        UICconfig = 10200,
        User = 8,
    }
    public enum ConnectionRecord2ObjectTypeCode
    {
        Account = 1,
        Activity = 4200,
        Appointment = 4201,
        Appointmentactivitymarketingtemplate = 10162,
        Campaign = 4400,
        CampaignActivity = 4402,
        Case = 112,
        ChannelAccessProfileRule = 9400,
        Competitor = 123,
        Contact = 2,
        Contentsettings = 10165,
        Contract = 1010,
        Dynamiccontentmetadata = 10179,
        Email = 4202,
        Entitlement = 9700,
        EntitlementChannel = 9701,
        EntitlementTemplateChannel = 9703,
        FacilityEquipment = 4000,
        Fax = 4204,
        FormsProsurveyinvite = 10049,
        FormsProsurveyresponse = 10050,
        Goal = 9600,
        Invoice = 1090,
        KnowledgeArticle = 9953,
        KnowledgeBaseRecord = 9930,
        Lead = 4,
        LeadScoringModel = 10215,
        Letter = 4207,
        LinkedInaccount = 10220,
        LinkedInactivity = 10221,
        LinkedInfieldmapping = 10224,
        LinkedInformquestion = 10227,
        LinkedInformsubmissionanswer = 10226,
        LinkedInLeadGenform = 10225,
        LinkedInLeadGenformsubmission = 10228,
        LinkedInleadmatchingstrategy = 10229,
        LinkedInuserprofile = 10230,
        Loan = 3,
        Marketingactivity = 10193,
        Marketingemaildynamiccontentmetadata = 10181,
        Marketingemailtestsend = 10183,
        Marketinglist = 4300,
        Order = 1088,
        PhoneCall = 4210,
        Phonecallactivitymarketingtemplate = 10194,
        Position = 50,
        PriceList = 1022,
        ProcessSession = 4710,
        Product = 1024,
        ProfileAlbum = 10040,
        Quote = 1084,
        RecurringAppointment = 4251,
        ResourceGroup = 4007,
        ResponseAction = 10086,
        ResponseError = 10089,
        SchedulingGroup = 4005,
        ServiceActivity = 4214,
        SocialActivity = 4216,
        SocialProfile = 99,
        SurveyActivity = 10094,
        SurveyResponse = 10096,
        Task = 4212,
        Taskactivitymarketingtemplate = 10199,
        Team = 9,
        Territory = 2013,
        UICconfig = 10200,
        User = 8,
    }
    public enum ConnectionRole_Category
    {
        Business = 1,
        Family = 2,
        Other = 5,
        Sales = 4,
        SalesTeam = 1001,
        Service = 1002,
        Social = 3,
        Stakeholder = 1000,
    }
    public enum ConnectionRoleStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum ConnectionStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum connectorStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum ConnectorType
    {
        CustomConnector = 1,
        NotSpecified = 0,
    }
    public enum ConstraintBasedGroupGroupTypeCode
    {
        Dynamic = 1,
        Hidden = 2,
        Static = 0,
    }
    public enum ContactAccountRoleCode
    {
        DecisionMaker = 1,
        Employee = 2,
        Influencer = 3,
    }
    public enum ContactAddress1_AddressTypeCode
    {
        BillTo = 1,
        Other = 4,
        Primary = 3,
        ShipTo = 2,
    }
    public enum ContactAddress1_FreightTermsCode
    {
        FOB = 1,
        NoCharge = 2,
    }
    public enum ContactAddress1_ShippingMethodCode
    {
        Airborne = 1,
        DHL = 2,
        FedEx = 3,
        FullLoad = 6,
        PostalMail = 5,
        UPS = 4,
        WillCall = 7,
    }
    public enum ContactAddress2_AddressTypeCode
    {
        DefaultValue = 1,
    }
    public enum ContactAddress2_FreightTermsCode
    {
        DefaultValue = 1,
    }
    public enum ContactAddress2_ShippingMethodCode
    {
        DefaultValue = 1,
    }
    public enum ContactAddress3_AddressTypeCode
    {
        DefaultValue = 1,
    }
    public enum ContactAddress3_FreightTermsCode
    {
        DefaultValue = 1,
    }
    public enum ContactAddress3_ShippingMethodCode
    {
        DefaultValue = 1,
    }
    public enum ContactCustomerSizeCode
    {
        DefaultValue = 1,
    }
    public enum ContactCustomerTypeCode
    {
        DefaultValue = 1,
    }
    public enum ContactEducationCode
    {
        DefaultValue = 1,
    }
    public enum ContactFamilyStatusCode
    {
        Divorced = 3,
        Married = 2,
        Single = 1,
        Widowed = 4,
    }
    public enum ContactGenderCode
    {
        Female = 2,
        Male = 1,
    }
    public enum ContactHasChildrenCode
    {
        DefaultValue = 1,
    }
    public enum Contactims_jobstatus
    {
        Confirmed = 176390000,
        Probation = 176390001,
    }
    public enum Contactims_ownershiptype
    {
        Partnership = 176390001,
        Soleproprietorship = 176390000,
    }
    public enum Contactims_spousetitle
    {
        Mr = 176390000,
        Mrs = 176390001,
        Ms = 176390002,
        Mx = 176390003,
    }
    public enum Contactims_title
    {
        Mr = 176390000,
        Mrs = 176390001,
        Ms = 176390002,
        Mx = 176390003,
    }
    public enum ContactLeadSourceCode
    {
        DefaultValue = 1,
    }
    public enum Contactmsdyn_orgchangestatus
    {
        Ignore = 2,
        No = 0,
        Yes = 1,
    }
    public enum ContactPaymentTermsCode
    {
        _210Net30 = 2,
        Net30 = 1,
        Net45 = 3,
        Net60 = 4,
    }
    public enum ContactPreferredAppointmentDayCode
    {
        Friday = 5,
        Monday = 1,
        Saturday = 6,
        Sunday = 0,
        Thursday = 4,
        Tuesday = 2,
        Wednesday = 3,
    }
    public enum ContactPreferredAppointmentTimeCode
    {
        Afternoon = 2,
        Evening = 3,
        Morning = 1,
    }
    public enum ContactPreferredContactMethodCode
    {
        Any = 1,
        Email = 2,
        Fax = 4,
        Mail = 5,
        Phone = 3,
        Web = 6,
    }
    public enum ContactShippingMethodCode
    {
        DefaultValue = 1,
    }
    public enum ContactStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum ContactTerritoryCode
    {
        DefaultValue = 1,
    }
    public enum ContractAllotmentTypeCode
    {
        CoverageDates = 3,
        NumberofCases = 1,
        Time = 2,
    }
    public enum ContractBillingFrequencyCode
    {
        Annually = 5,
        Bimonthly = 2,
        Monthly = 1,
        Quarterly = 3,
        Semiannually = 4,
    }
    public enum ContractContractServiceLevelCode
    {
        Bronze = 3,
        Gold = 1,
        Silver = 2,
    }
    public enum ContractDetailContractStateCode
    {
    }
    public enum ContractDetailServiceContractUnitsCode
    {
        DefaultValue = 1,
    }
    public enum ContractDetailStatusCode
    {
        Canceled = 3,
        Expired = 4,
        New = 1,
        Renewed = 2,
    }
    public enum ContractStatusCode
    {
        Active = 3,
        Canceled = 5,
        Draft = 1,
        Expired = 6,
        Invoiced = 2,
        OnHold = 4,
    }
    public enum ContractTemplateAllotmentTypeCode
    {
        CoverageDates = 3,
        NumberofCases = 1,
        Time = 2,
    }
    public enum ContractTemplateBillingFrequencyCode
    {
        Annually = 5,
        Bimonthly = 2,
        Monthly = 1,
        Quarterly = 3,
        Semiannually = 4,
    }
    public enum ContractTemplateContractServiceLevelCode
    {
        Bronze = 3,
        Gold = 1,
        Silver = 2,
    }
    public enum convert_campaign_response_deactivate_status
    {
        Cancelled = 2,
        Closed = 1,
    }
    public enum convert_campaign_response_option
    {
        Closeresponse = 4,
        Converttoanexistinglead = 2,
        Createalead = 1,
        Createaquoteorderoropportunityforanaccountorcontact = 3,
    }
    public enum convert_campaign_response_options
    {
        Closeresponse = 3,
        Converttoanexistinglead = 1,
        Createalead = 0,
        Createaquoteorderoropportunityforanaccountorcontact = 2,
    }
    public enum convert_campaign_response_qualify_lead_options
    {
        Disqualifylead = 1,
        Qualifylead = 0,
    }
    public enum convert_campaign_response_sales_entity_type
    {
        Opportunity = 1,
        Order = 2,
        Quote = 3,
    }
    public enum convert_campaign_response_to_lead_disqualify_status
    {
        Canceled = 7,
        CannotContact = 5,
        Lost = 4,
        NoLongerInterested = 6,
    }
    public enum convert_campaign_response_to_lead_option
    {
        Disqualify = 2,
        Qualifyandconvertintothefollowingrecords = 1,
    }
    public enum convert_campaign_response_to_lead_qualify_status
    {
        Qualified = 3,
    }
    public enum ConvertRule_ChannelActivity
    {
        Appointment = 4201,
        Email = 4202,
        FormsProsurveyinvite = 10049,
        FormsProsurveyresponse = 10050,
        PhoneCall = 4210,
        ServiceActivity = 4214,
        SocialActivity = 4216,
        SurveyActivity = 10094,
        Task = 4212,
    }
    public enum ConvertRuleSourceTypeCode
    {
        Email = 2,
        SocialMonitoring = 1,
    }
    public enum ConvertRuleStatusCode
    {
        Active = 2,
        Draft = 1,
    }
    public enum CustomerAddressAddressTypeCode
    {
        BillTo = 1,
        Other = 4,
        Primary = 3,
        ShipTo = 2,
    }
    public enum CustomerAddressFreightTermsCode
    {
        FOB = 1,
        NoCharge = 2,
    }
    public enum CustomerAddressShippingMethodCode
    {
        Airborne = 1,
        DHL = 2,
        FedEx = 3,
        FullLoad = 6,
        PostalMail = 5,
        UPS = 4,
        WillCall = 7,
    }
    public enum delete_recurringappointmentmaster
    {
        Allinstances = 1,
        Theseriesleavepastappointments = 2,
    }
    public enum DelveActionHubCardType
    {
        Default = 0,
        MeetingRequest = 3,
        SendContentRequest = 1,
        YesNo = 2,
    }
    public enum DelveActionHubStatusCode
    {
        Completed = 2,
        Dismiss = 3,
        Pending = 1,
    }
    public enum DependencyType
    {
        None = 0,
        Published = 2,
        SolutionInternal = 1,
        Unpublished = 4,
    }
    public enum DiscountStatusCode
    {
    }
    public enum DiscountTypeStatusCode
    {
        Active = 100001,
        Inactive = 100002,
    }
    public enum DocumentIndexDocumentTypeCode
    {
        DefaultValue = 1,
    }
    public enum DocumentTemplateDocumentType
    {
        MicrosoftExcel = 1,
        MicrosoftWord = 2,
    }
    public enum DuplicateRuleBaseEntityTypeCode
    {
        Account = 1,
        AccountLeads = 16,
        ACIViewMapper = 8040,
        ActionCard = 9962,
        actioncardregarding = 10032,
        ActionCardRoleSetting = 10033,
        ActionCardType = 9983,
        ActionCardUserSettings = 9973,
        ActionCardUserState = 9968,
        Activity = 4200,
        ActivityParty = 135,
        Address = 1071,
        admin_settings_entity = 10025,
        AdvancedSimilarityRule = 9949,
        AIConfiguration = 402,
        AIFormProcessingDocument = 10008,
        AIModel = 401,
        AIObjectDetectionBoundingBox = 10011,
        AIObjectDetectionImage = 10009,
        AIObjectDetectionImageMapping = 10012,
        AIObjectDetectionLabel = 10010,
        AITemplate = 400,
        AnalysisComponent = 10065,
        AnalysisJob = 10066,
        AnalysisResult = 10067,
        AnalysisResultDetail = 10068,
        Announcement = 132,
        AnnualFiscalCalendar = 2000,
        Answer = 10073,
        AppConfigMaster = 9011,
        AppConfigSetup = 10055,
        AppConfiguration = 9012,
        AppConfigurationInstance = 9013,
        ApplicationFile = 4707,
        ApplicationRibbons = 1120,
        AppModuleComponent = 9007,
        AppModuleMetadata = 8700,
        AppModuleMetadataAsyncOperation = 8702,
        AppModuleMetadataDependency = 8701,
        AppModuleRoles = 9009,
        Appointment = 4201,
        Appointmentactivitymarketingtemplate = 10162,
        Article = 127,
        ArticleComment = 1082,
        ArticleTemplate = 1016,
        Attachment_1001 = 1001,
        Attachment_1002 = 1002,
        AttendeePass = 10110,
        Attribute = 9808,
        AttributeImageConfig = 431,
        AttributeMap = 4601,
        Auditing = 4567,
        AuthenticatedDomain = 10198,
        AuthenticationSettings = 10111,
        AuthorizationServer = 1094,
        AzureDeployment = 10074,
        AzureServiceConnection = 9936,
        BookableResource = 1150,
        BookableResourceBooking = 1145,
        BookableResourceBookingHeader = 1146,
        BookableResourceBookingtoExchangeIdMapping = 4421,
        BookableResourceCategory = 1147,
        BookableResourceCategoryAssn = 1149,
        BookableResourceCharacteristic = 1148,
        BookableResourceGroup = 1151,
        BookingStatus = 1152,
        Bucket = 10113,
        Building = 10114,
        BulkDeleteFailure = 4425,
        BulkDeleteOperation = 4424,
        BulkOperationLog = 4405,
        BusinessDataLocalizedLabel = 4232,
        BusinessProcessFlowInstance = 4725,
        BusinessUnit = 10,
        BusinessUnitMap = 6,
        Calendar = 4003,
        CalendarRule = 4004,
        CallbackRegistration = 301,
        Campaign = 4400,
        CampaignActivity = 4402,
        CampaignActivityItem = 4404,
        CampaignItem = 4403,
        CampaignResponse = 4401,
        CanvasApp = 300,
        Case = 112,
        CaseResolution = 4206,
        Category = 9959,
        CDNconfiguration = 10163,
        ChannelAccessProfile = 3005,
        ChannelAccessProfileRule = 9400,
        ChannelAccessProfileRuleItem = 9401,
        ChannelProperty = 1236,
        ChannelPropertyGroup = 1234,
        Characteristic = 1141,
        Checkin = 10115,
        ChildIncidentCount = 113,
        Clientupdate = 36,
        ColumnMapping = 4417,
        Comment = 8005,
        Commitment = 4215,
        Competitor = 123,
        CompetitorAddress = 1004,
        CompetitorProduct = 1006,
        CompetitorSalesLiterature = 26,
        ComponentLayer = 10006,
        ComponentLayerDataSource = 10007,
        Configuration = 10056,
        Connection = 3234,
        ConnectionRole = 3231,
        ConnectionRoleObjectTypeCode = 3233,
        Connector = 372,
        Contact = 2,
        ContactInvoices = 17,
        ContactLeads = 22,
        ContactOrders = 19,
        ContactQuotes = 18,
        ContactType = 10057,
        Contentblock = 10108,
        Contentsettings = 10165,
        Contract = 1010,
        ContractLine = 1011,
        ContractTemplate = 2011,
        Currency = 9105,
        Customchannelactivity = 10168,
        CustomControl = 9753,
        CustomControlDefaultConfig = 9755,
        CustomControlResource = 9754,
        Customerinsightsinformation = 10166,
        Customerjourney = 10167,
        Customerjourneyiteration = 10169,
        Customerjourneyruntimestate = 10170,
        Customerjourneytemplate = 10171,
        Customerjourneyworkflowlink = 10172,
        CustomerRelationship = 4502,
        CustomRegistrationField = 10116,
        DatabaseVersion = 10014,
        DataImport = 4410,
        DataMap = 4411,
        DataPerformanceDashboard = 4450,
        Defaultmarketingsettings = 10173,
        DelveActionHub = 9961,
        Dependency = 7105,
        DependencyFeature = 7108,
        DependencyNode = 7106,
        Designerfeatureprotection = 10109,
        Digitalassetsconfiguration = 10103,
        Discount = 1013,
        DiscountList = 1080,
        DisplayString = 4102,
        DisplayStringMap = 4101,
        DocumentLocation = 9508,
        DocumentSuggestions = 1189,
        DocumentTemplate = 9940,
        DuplicateDetectionRule = 4414,
        DuplicateRecord = 4415,
        DuplicateRuleCondition = 4416,
        Dynamiccontentmetadata = 10179,
        Email = 4202,
        EmailHash = 4023,
        EmailSearch = 4299,
        EmailServerProfile = 9605,
        EmailSignature = 9997,
        EmailTemplate = 2010,
        Entitlement = 9700,
        EntitlementChannel = 9701,
        EntitlementContact = 7272,
        EntitlementEntityAllocationTypeMapping = 9704,
        EntitlementProduct = 6363,
        EntitlementTemplate = 9702,
        EntitlementTemplateChannel = 9703,
        EntitlementTemplateProduct = 4545,
        Entity = 9800,
        EntityAnalyticsConfig = 430,
        EntityCounter = 10117,
        EntityImageConfig = 432,
        EntityMap = 4600,
        EntityRankingRule = 10034,
        EnvironmentVariableDefinition = 380,
        EnvironmentVariableValue = 381,
        Event = 10118,
        EventAdministration = 10119,
        EventCustomRegistrationField = 10120,
        EventMainBusinessProcessFlow = 10112,
        EventManagementActivity = 10121,
        EventManagementConfiguration = 10122,
        EventPurchase = 10123,
        EventPurchaseAttendee = 10124,
        EventPurchaseContact = 10125,
        EventPurchasePass = 10126,
        EventRegistration = 10127,
        EventTeamMember = 10128,
        EventVendor = 10129,
        ExchangeSyncIdMapping = 4120,
        ExpanderEvent = 4711,
        ExpiredProcess = 955,
        ExternalParty = 3008,
        ExternalPartyItem = 9987,
        FacilityEquipment = 4000,
        Fax = 4204,
        Feedback = 9958,
        FeedbackMapping = 10075,
        FeedbackRelatedSurvey = 10076,
        FieldPermission = 1201,
        FieldSecurityProfile = 1200,
        FieldSharing = 44,
        File = 10104,
        File_OBSOLETE = 10130,
        FileAttachment = 55,
        Filter = 10044,
        FilterTemplate = 30,
        FixedMonthlyFiscalCalendar = 2004,
        flowcardtype = 10035,
        Follow = 8003,
        Forecast = 10027,
        Forecastdefinition = 10026,
        Forecastrecurrence = 10028,
        Formpage = 10174,
        Formpagetemplate = 10175,
        FormsProsurvey = 10048,
        FormsProsurveyemailtemplate = 10045,
        FormsProsurveyinvite = 10049,
        FormsProsurveyquestion = 10046,
        FormsProsurveyquestionresponse = 10047,
        FormsProsurveyresponse = 10050,
        FormsProunsubscribedrecipient = 10051,
        Formwhitelistrule = 10187,
        GDPRconfiguration = 10235,
        GDPRconsentchangerecord = 10236,
        Geopin = 10176,
        GlobalSearchConfiguration = 54,
        Goal = 9600,
        GoalMetric = 9603,
        Grid = 10083,
        Gwennolfeatureconfiguration = 10240,
        HierarchyRule = 8840,
        HierarchySecurityConfiguration = 9919,
        HolidayWrapper = 9996,
        Hotel = 10131,
        HotelRoomAllocation = 10132,
        HotelRoomReservation = 10133,
        icebreakersconfig = 10039,
        Image = 10077,
        ImageDescriptor = 1007,
        ImageTokenCache = 10078,
        ImportData = 4413,
        ImportEntityMapping = 4428,
        ImportJob = 9107,
        ImportLog = 4423,
        ImportSourceFile = 4412,
        IncidentKnowledgeBaseRecord = 9931,
        IndexedArticle = 126,
        IntegrationStatus = 3000,
        InteractionforEmail = 9986,
        InternalAddress = 1003,
        InterProcessLock = 4011,
        InvalidDependency = 7107,
        Invoice = 1090,
        InvoiceProduct = 1091,
        ISVConfig = 4705,
        Keyword = 10105,
        KnowledgeArticle = 9953,
        KnowledgeArticleCategory = 9960,
        KnowledgeArticleIncident = 9954,
        KnowledgeArticleViews = 9955,
        KnowledgeBaseRecord = 9930,
        KnowledgeSearchModel = 9947,
        Language = 9957,
        LanguageProvisioningState = 9875,
        Layout = 10134,
        Lead = 4,
        LeadAddress = 1017,
        LeadCompetitors = 24,
        Leadentityfield = 10219,
        LeadProduct = 27,
        Leadscore = 10217,
        Leadscore_Deprecated = 10214,
        LeadScoringConfiguration = 10218,
        LeadScoringModel = 10215,
        LeadSource = 10058,
        Leadtoopportunity = 10216,
        LeadToOpportunitySalesProcess = 954,
        Letter = 4207,
        License = 2027,
        Like = 8006,
        LinkedAnswer = 10080,
        LinkedInaccount = 10220,
        LinkedInactivity = 10221,
        LinkedIncampaign = 10222,
        LinkedInfieldmapping = 10224,
        LinkedInformquestion = 10227,
        LinkedInformsubmissionanswer = 10226,
        LinkedInLeadGenform = 10225,
        LinkedInLeadGenformsubmission = 10228,
        LinkedInLeadGenintegrationconfiguration = 10223,
        LinkedInleadmatchingstrategy = 10229,
        LinkedInMatchedAudience = 10237,
        LinkedInuserprofile = 10230,
        LinkedQuestion = 10085,
        ListForm = 10177,
        ListValueMapping = 4418,
        Loan = 3,
        LoanProgram = 10059,
        LoanPurpose = 10060,
        LoanStatus = 10061,
        LoanType = 10062,
        LocalConfigStore = 9201,
        LookupMapping = 4419,
        Mailbox = 9606,
        MailboxAutoTrackingFolder = 9608,
        MailboxStatistics = 9607,
        MailboxTrackingCategory = 9609,
        MailMergeTemplate = 9106,
        Marketingactivity = 10193,
        Marketinganalyticsconfiguration = 10164,
        Marketingconfiguration = 10178,
        Marketingemail = 10180,
        Marketingemaildynamiccontentmetadata = 10181,
        Marketingemailtemplate = 10182,
        MarketingEmailTest = 10210,
        MarketingEmailTestAttribute = 10211,
        Marketingemailtestsend = 10183,
        Marketingfeatureconfiguration = 10234,
        Marketingfieldsubmission = 10212,
        Marketingform = 10184,
        Marketingformfield = 10185,
        Marketingformsubmission = 10213,
        Marketingformtemplate = 10186,
        Marketinglist = 4300,
        MarketingListMember = 4301,
        Marketingpage = 10188,
        Marketingpageconfiguration = 10189,
        Marketingpagetemplate = 10190,
        Marketingwebsite = 10202,
        Matchingstrategy = 10191,
        Matchingstrategyattributes = 10192,
        MetadataDifference = 4231,
        MicrosoftTeamsCollaborationentity = 10054,
        MicrosoftTeamsGraphresourceEntity = 10052,
        Migration = 10246,
        MobileOfflineProfile = 9866,
        MobileOfflineProfileItem = 9867,
        MobileOfflineProfileItemAssociation = 9868,
        ModeldrivenApp = 9006,
        MonthlyFiscalCalendar = 2003,
        MortgageLoanProcess = 10063,
        msdyn_msteamssetting = 10053,
        msdyn_relationshipinsightsunifiedconfig = 10029,
        MultiEntitySearch = 9910,
        MultiSelectOptionValue = 9912,
        NavigationSetting = 9900,
        NewProcess = 950,
        Note = 5,
        NotesanalysisConfig = 10038,
        Notification = 4110,
        ODatav4DataSource = 10002,
        OfficeDocument = 4490,
        OfficeGraphDocument = 9950,
        OfflineCommandDefinition = 9870,
        OpportunityClose = 4208,
        OpportunityCompetitors = 25,
        OpportunityProduct = 1083,
        OpportunityRelationship = 4503,
        OpportunitySalesProcess = 953,
        OptionSet = 9809,
        Order = 1088,
        OrderClose = 4209,
        OrderProduct = 1089,
        Organization = 1019,
        OrganizationInsightsMetric = 9699,
        OrganizationInsightsNotification = 9690,
        OrganizationStatistic = 4708,
        OrganizationUI = 1021,
        Owner = 7,
        OwnerMapping = 4420,
        Page = 10081,
        PartnerApplication = 1095,
        Pass = 10135,
        PersonalDocumentTemplate = 9941,
        Personalizedpage = 10244,
        Personalizedpagefield = 10245,
        PhoneCall = 4210,
        Phonecallactivitymarketingtemplate = 10194,
        PhoneToCaseProcess = 952,
        Playbook = 10022,
        Playbookactivity = 10019,
        Playbookactivityattribute = 10020,
        PlaybookCallableContext = 10018,
        Playbookcategory = 10021,
        Playbooktemplate = 10023,
        PluginAssembly = 4605,
        PluginProfile = 10072,
        PluginTraceLog = 4619,
        PluginType = 4602,
        PluginTypeStatistic = 4603,
        Portalsettings = 10195,
        Position = 50,
        Post = 8000,
        PostConfiguration = 10041,
        PostRegarding = 8002,
        PostRole = 8001,
        PostRuleConfiguration = 10042,
        PriceList = 1022,
        PriceListItem = 1026,
        PrincipalSyncAttributeMap = 1404,
        Privilege = 1023,
        PrivilegeObjectTypeCode = 31,
        Process = 4703,
        ProcessConfiguration = 9650,
        ProcessDependency = 4704,
        ProcessLog = 4706,
        ProcessSession = 4710,
        ProcessStage = 4724,
        ProcessTrigger = 4712,
        Product = 1024,
        ProductAssociation = 1025,
        ProductRelationship = 1028,
        ProductSalesLiterature = 21,
        ProfileAlbum = 10040,
        Property_10064 = 10064,
        Property_1048 = 1048,
        PropertyAssociation = 1235,
        PropertyInstance = 1333,
        PropertyOptionSetItem = 1049,
        Publisher = 7101,
        PublisherAddress = 7102,
        QuarterlyFiscalCalendar = 2002,
        Question = 10082,
        QuestionResponse = 10084,
        Queue = 2020,
        QueueItem = 2029,
        QueueItemCount = 2023,
        QueueMemberCount = 2024,
        QuickCampaign = 4406,
        Quotainfoentity = 10233,
        Quote = 1084,
        QuoteClose = 4211,
        QuoteProduct = 1085,
        RatingModel = 1144,
        RatingValue = 1142,
        RecordCreationandUpdateRule = 9300,
        RecordCreationandUpdateRuleItem = 9301,
        RecurrenceRule = 4250,
        RecurringAppointment = 4251,
        RedirectURL = 10196,
        RegistrationResponse = 10136,
        RelationshipRole = 4500,
        RelationshipRoleMap = 4501,
        ReplicationBacklog = 1140,
        Report = 9100,
        ReportLink = 9104,
        ReportRelatedCategory = 9102,
        ReportRelatedEntity = 9101,
        ReportVisibility = 9103,
        Resource = 4002,
        ResourceExpansion = 4010,
        ResourceGroup = 4007,
        ResourceSpecification = 4006,
        ResponseAction = 10086,
        ResponseCondition = 10088,
        ResponseError = 10089,
        ResponseOutcome = 10090,
        ResponseRouting = 10091,
        RibbonClientMetadata = 4579,
        RibbonCommand = 1116,
        RibbonContextGroup = 1115,
        RibbonDifference = 1130,
        RibbonMetadataToProcess = 9880,
        RibbonRule = 1117,
        RibbonTabToCommandMapping = 1113,
        RoleTemplate = 1037,
        RollupField = 9604,
        RollupJob = 9511,
        RollupProperties = 9510,
        RollupQuery = 9602,
        Room = 10137,
        RoomReservation = 10138,
        RoutingRuleSet = 8181,
        RuleItem = 8199,
        RuntimeDependency = 7200,
        SalesAttachment = 1070,
        salesinsightssettings = 10036,
        SalesLiterature = 1038,
        SalesProcessInstance = 32,
        SavedOrganizationInsightsConfiguration = 1309,
        SavedView = 4230,
        SchedulingGroup = 4005,
        SdkMessage = 4606,
        SdkMessageFilter = 4607,
        SdkMessagePair = 4613,
        SdkMessageProcessingStep = 4608,
        SdkMessageProcessingStepImage = 4615,
        SdkMessageProcessingStepSecureConfiguration = 4616,
        SdkMessageRequest = 4609,
        SdkMessageRequestField = 4614,
        SdkMessageResponse = 4610,
        SdkMessageResponseField = 4611,
        Section = 10092,
        SecurityRole = 1036,
        Segment = 10197,
        SemiannualFiscalCalendar = 2001,
        Service = 4001,
        ServiceActivity = 4214,
        ServiceContractContact = 20,
        ServiceEndpoint = 4618,
        Session = 10139,
        SessionRegistration = 10140,
        SessionTrack = 10141,
        SharePointData = 9509,
        SharepointDocument = 9507,
        SharePointSite = 9502,
        siconfig = 10030,
        SIKeyValueConfig = 10031,
        SimilarityRule = 9951,
        Site = 4009,
        SiteMap = 4709,
        SLA = 9750,
        SLAItem = 9751,
        SLAKPIInstance = 9752,
        SocialActivity = 4216,
        SocialInsightsConfiguration = 1300,
        Socialpost = 10241,
        SocialPostingConfiguration = 10242,
        SocialPostingConsent = 10243,
        SocialProfile = 99,
        Solution = 7100,
        SolutionComponent = 7103,
        SolutionComponentDataSource = 10001,
        SolutionComponentDefinition = 7104,
        SolutionComponentFileConfiguration = 10005,
        SolutionComponentSummary = 10000,
        SolutionHealthRule = 10069,
        SolutionHealthRuleArgument = 10070,
        SolutionHealthRuleSet = 10071,
        SolutionHistory = 10003,
        SolutionHistoryData = 9890,
        SolutionHistoryDataSource = 10004,
        SpamScoreActivity = 10238,
        SpamScoreRequest = 10239,
        Speaker = 10142,
        SpeakerEngagement = 10143,
        SponsorableArticle = 10144,
        Sponsorship = 10145,
        StatusMap = 1075,
        StringMap = 1043,
        Subject = 129,
        Subscription = 29,
        SubscriptionClients = 1072,
        SubscriptionManuallyTrackedObject = 37,
        SubscriptionStatisticOffline = 45,
        SubscriptionStatisticOutlook = 46,
        SubscriptionSyncEntryOffline = 47,
        SubscriptionSyncEntryOutlook = 48,
        SubscriptionSynchronizationInformation = 33,
        SuggestionCardTemplate = 1190,
        Survey = 10093,
        SurveyActivity = 10094,
        SurveyResponse = 10096,
        SyncAttributeMapping = 1401,
        SyncAttributeMappingProfile = 1400,
        SyncError = 9869,
        SystemApplicationMetadata = 7000,
        SystemChart = 1111,
        SystemForm = 1030,
        SystemJob = 4700,
        SystemUserBusinessUnitEntityMap = 42,
        SystemUserManagerMap = 51,
        SystemUserPrincipal = 14,
        Task = 4212,
        Taskactivitymarketingtemplate = 10199,
        Team = 9,
        TeamProfiles = 1203,
        TeamSyncAttributeMappingProfiles = 1403,
        Teamtemplate = 92,
        Territory = 2013,
        TextAnalyticsEntityMapping = 9945,
        TextAnalyticsTopic = 9948,
        Theme_10097 = 10097,
        Theme_2015 = 2015,
        TimeStampDateMapping = 9932,
        TimeZoneDefinition = 4810,
        TimeZoneLocalizedName = 4812,
        TimeZoneRule = 4811,
        TopicHistory = 9946,
        TopicModel = 9944,
        TopicModelConfiguration = 9942,
        TopicModelExecutionHistory = 9943,
        Trace = 8050,
        TraceAssociation = 8051,
        TraceRegarding = 8052,
        Trackinginformationfordeletedentities = 35,
        TransformationMapping = 4426,
        TransformationParameterMapping = 4427,
        TranslationProcess = 951,
        UICconfig = 10200,
        Unit = 1055,
        UnitGroup = 1056,
        UnresolvedAddress = 2012,
        UntrackedAppointment = 10037,
        UntrackedEmail = 4220,
        UpgradeRun = 10015,
        UpgradeStep = 10016,
        UpgradeVersion = 10017,
        User = 8,
        UserApplicationMetadata = 7001,
        UserChart = 1112,
        UserDashboard = 1031,
        UserEntityInstanceData = 2501,
        UserEntityUISettings = 2500,
        UserFiscalCalendar = 1086,
        Usergeoregion = 10201,
        UserMapping = 2016,
        UserSearchFacet = 52,
        UserSettings = 150,
        Venue = 10146,
        Video = 10106,
        View = 1039,
        VirtualEntityDataProvider = 78,
        VirtualEntityDataSource = 85,
        VoCImport = 10079,
        VoCResponseBlobStore = 10087,
        VoiceoftheCustomerConfiguration = 10098,
        VoiceoftheCustomerLog = 10095,
        WaitlistItem = 10147,
        WallView = 10043,
        WebApplication = 10148,
        WebinarConfiguration = 10149,
        WebinarConsent = 10150,
        WebinarProvider = 10151,
        WebinarType = 10152,
        WebResource = 9333,
        WebsiteEntityConfiguration = 10153,
        WebWizard = 4800,
        WebWizardAccessPrivilege = 4803,
        WizardPage = 4802,
        WorkflowWaitSubscription = 4702,
    }
    public enum DuplicateRuleConditionOperatorCode
    {
        ExactMatch = 0,
        ExactMatch_PickListLabel = 5,
        ExactMatch_PickListValue = 6,
        SameDate = 3,
        SameDateandTime = 4,
        SameFirstCharacters = 1,
        SameLastCharacters = 2,
    }
    public enum DuplicateRuleMatchingEntityTypeCode
    {
        Account = 1,
        AccountLeads = 16,
        ACIViewMapper = 8040,
        ActionCard = 9962,
        actioncardregarding = 10032,
        ActionCardRoleSetting = 10033,
        ActionCardType = 9983,
        ActionCardUserSettings = 9973,
        ActionCardUserState = 9968,
        Activity = 4200,
        ActivityParty = 135,
        Address = 1071,
        admin_settings_entity = 10025,
        AdvancedSimilarityRule = 9949,
        AIConfiguration = 402,
        AIFormProcessingDocument = 10008,
        AIModel = 401,
        AIObjectDetectionBoundingBox = 10011,
        AIObjectDetectionImage = 10009,
        AIObjectDetectionImageMapping = 10012,
        AIObjectDetectionLabel = 10010,
        AITemplate = 400,
        AnalysisComponent = 10065,
        AnalysisJob = 10066,
        AnalysisResult = 10067,
        AnalysisResultDetail = 10068,
        Announcement = 132,
        AnnualFiscalCalendar = 2000,
        Answer = 10073,
        AppConfigMaster = 9011,
        AppConfigSetup = 10055,
        AppConfiguration = 9012,
        AppConfigurationInstance = 9013,
        ApplicationFile = 4707,
        ApplicationRibbons = 1120,
        AppModuleComponent = 9007,
        AppModuleMetadata = 8700,
        AppModuleMetadataAsyncOperation = 8702,
        AppModuleMetadataDependency = 8701,
        AppModuleRoles = 9009,
        Appointment = 4201,
        Appointmentactivitymarketingtemplate = 10162,
        Article = 127,
        ArticleComment = 1082,
        ArticleTemplate = 1016,
        Attachment_1001 = 1001,
        Attachment_1002 = 1002,
        AttendeePass = 10110,
        Attribute = 9808,
        AttributeImageConfig = 431,
        AttributeMap = 4601,
        Auditing = 4567,
        AuthenticatedDomain = 10198,
        AuthenticationSettings = 10111,
        AuthorizationServer = 1094,
        AzureDeployment = 10074,
        AzureServiceConnection = 9936,
        BookableResource = 1150,
        BookableResourceBooking = 1145,
        BookableResourceBookingHeader = 1146,
        BookableResourceBookingtoExchangeIdMapping = 4421,
        BookableResourceCategory = 1147,
        BookableResourceCategoryAssn = 1149,
        BookableResourceCharacteristic = 1148,
        BookableResourceGroup = 1151,
        BookingStatus = 1152,
        Bucket = 10113,
        Building = 10114,
        BulkDeleteFailure = 4425,
        BulkDeleteOperation = 4424,
        BulkOperationLog = 4405,
        BusinessDataLocalizedLabel = 4232,
        BusinessProcessFlowInstance = 4725,
        BusinessUnit = 10,
        BusinessUnitMap = 6,
        Calendar = 4003,
        CalendarRule = 4004,
        CallbackRegistration = 301,
        Campaign = 4400,
        CampaignActivity = 4402,
        CampaignActivityItem = 4404,
        CampaignItem = 4403,
        CampaignResponse = 4401,
        CanvasApp = 300,
        Case = 112,
        CaseResolution = 4206,
        Category = 9959,
        CDNconfiguration = 10163,
        ChannelAccessProfile = 3005,
        ChannelAccessProfileRule = 9400,
        ChannelAccessProfileRuleItem = 9401,
        ChannelProperty = 1236,
        ChannelPropertyGroup = 1234,
        Characteristic = 1141,
        Checkin = 10115,
        ChildIncidentCount = 113,
        Clientupdate = 36,
        ColumnMapping = 4417,
        Comment = 8005,
        Commitment = 4215,
        Competitor = 123,
        CompetitorAddress = 1004,
        CompetitorProduct = 1006,
        CompetitorSalesLiterature = 26,
        ComponentLayer = 10006,
        ComponentLayerDataSource = 10007,
        Configuration = 10056,
        Connection = 3234,
        ConnectionRole = 3231,
        ConnectionRoleObjectTypeCode = 3233,
        Connector = 372,
        Contact = 2,
        ContactInvoices = 17,
        ContactLeads = 22,
        ContactOrders = 19,
        ContactQuotes = 18,
        ContactType = 10057,
        Contentblock = 10108,
        Contentsettings = 10165,
        Contract = 1010,
        ContractLine = 1011,
        ContractTemplate = 2011,
        Currency = 9105,
        Customchannelactivity = 10168,
        CustomControl = 9753,
        CustomControlDefaultConfig = 9755,
        CustomControlResource = 9754,
        Customerinsightsinformation = 10166,
        Customerjourney = 10167,
        Customerjourneyiteration = 10169,
        Customerjourneyruntimestate = 10170,
        Customerjourneytemplate = 10171,
        Customerjourneyworkflowlink = 10172,
        CustomerRelationship = 4502,
        CustomRegistrationField = 10116,
        DatabaseVersion = 10014,
        DataImport = 4410,
        DataMap = 4411,
        DataPerformanceDashboard = 4450,
        Defaultmarketingsettings = 10173,
        DelveActionHub = 9961,
        Dependency = 7105,
        DependencyFeature = 7108,
        DependencyNode = 7106,
        Designerfeatureprotection = 10109,
        Digitalassetsconfiguration = 10103,
        Discount = 1013,
        DiscountList = 1080,
        DisplayString = 4102,
        DisplayStringMap = 4101,
        DocumentLocation = 9508,
        DocumentSuggestions = 1189,
        DocumentTemplate = 9940,
        DuplicateDetectionRule = 4414,
        DuplicateRecord = 4415,
        DuplicateRuleCondition = 4416,
        Dynamiccontentmetadata = 10179,
        Email = 4202,
        EmailHash = 4023,
        EmailSearch = 4299,
        EmailServerProfile = 9605,
        EmailSignature = 9997,
        EmailTemplate = 2010,
        Entitlement = 9700,
        EntitlementChannel = 9701,
        EntitlementContact = 7272,
        EntitlementEntityAllocationTypeMapping = 9704,
        EntitlementProduct = 6363,
        EntitlementTemplate = 9702,
        EntitlementTemplateChannel = 9703,
        EntitlementTemplateProduct = 4545,
        Entity = 9800,
        EntityAnalyticsConfig = 430,
        EntityCounter = 10117,
        EntityImageConfig = 432,
        EntityMap = 4600,
        EntityRankingRule = 10034,
        EnvironmentVariableDefinition = 380,
        EnvironmentVariableValue = 381,
        Event = 10118,
        EventAdministration = 10119,
        EventCustomRegistrationField = 10120,
        EventMainBusinessProcessFlow = 10112,
        EventManagementActivity = 10121,
        EventManagementConfiguration = 10122,
        EventPurchase = 10123,
        EventPurchaseAttendee = 10124,
        EventPurchaseContact = 10125,
        EventPurchasePass = 10126,
        EventRegistration = 10127,
        EventTeamMember = 10128,
        EventVendor = 10129,
        ExchangeSyncIdMapping = 4120,
        ExpanderEvent = 4711,
        ExpiredProcess = 955,
        ExternalParty = 3008,
        ExternalPartyItem = 9987,
        FacilityEquipment = 4000,
        Fax = 4204,
        Feedback = 9958,
        FeedbackMapping = 10075,
        FeedbackRelatedSurvey = 10076,
        FieldPermission = 1201,
        FieldSecurityProfile = 1200,
        FieldSharing = 44,
        File = 10104,
        File_OBSOLETE = 10130,
        FileAttachment = 55,
        Filter = 10044,
        FilterTemplate = 30,
        FixedMonthlyFiscalCalendar = 2004,
        flowcardtype = 10035,
        Follow = 8003,
        Forecast = 10027,
        Forecastdefinition = 10026,
        Forecastrecurrence = 10028,
        Formpage = 10174,
        Formpagetemplate = 10175,
        FormsProsurvey = 10048,
        FormsProsurveyemailtemplate = 10045,
        FormsProsurveyinvite = 10049,
        FormsProsurveyquestion = 10046,
        FormsProsurveyquestionresponse = 10047,
        FormsProsurveyresponse = 10050,
        FormsProunsubscribedrecipient = 10051,
        Formwhitelistrule = 10187,
        GDPRconfiguration = 10235,
        GDPRconsentchangerecord = 10236,
        Geopin = 10176,
        GlobalSearchConfiguration = 54,
        Goal = 9600,
        GoalMetric = 9603,
        Grid = 10083,
        Gwennolfeatureconfiguration = 10240,
        HierarchyRule = 8840,
        HierarchySecurityConfiguration = 9919,
        HolidayWrapper = 9996,
        Hotel = 10131,
        HotelRoomAllocation = 10132,
        HotelRoomReservation = 10133,
        icebreakersconfig = 10039,
        Image = 10077,
        ImageDescriptor = 1007,
        ImageTokenCache = 10078,
        ImportData = 4413,
        ImportEntityMapping = 4428,
        ImportJob = 9107,
        ImportLog = 4423,
        ImportSourceFile = 4412,
        IncidentKnowledgeBaseRecord = 9931,
        IndexedArticle = 126,
        IntegrationStatus = 3000,
        InteractionforEmail = 9986,
        InternalAddress = 1003,
        InterProcessLock = 4011,
        InvalidDependency = 7107,
        Invoice = 1090,
        InvoiceProduct = 1091,
        ISVConfig = 4705,
        Keyword = 10105,
        KnowledgeArticle = 9953,
        KnowledgeArticleCategory = 9960,
        KnowledgeArticleIncident = 9954,
        KnowledgeArticleViews = 9955,
        KnowledgeBaseRecord = 9930,
        KnowledgeSearchModel = 9947,
        Language = 9957,
        LanguageProvisioningState = 9875,
        Layout = 10134,
        Lead = 4,
        LeadAddress = 1017,
        LeadCompetitors = 24,
        Leadentityfield = 10219,
        LeadProduct = 27,
        Leadscore = 10217,
        Leadscore_Deprecated = 10214,
        LeadScoringConfiguration = 10218,
        LeadScoringModel = 10215,
        LeadSource = 10058,
        Leadtoopportunity = 10216,
        LeadToOpportunitySalesProcess = 954,
        Letter = 4207,
        License = 2027,
        Like = 8006,
        LinkedAnswer = 10080,
        LinkedInaccount = 10220,
        LinkedInactivity = 10221,
        LinkedIncampaign = 10222,
        LinkedInfieldmapping = 10224,
        LinkedInformquestion = 10227,
        LinkedInformsubmissionanswer = 10226,
        LinkedInLeadGenform = 10225,
        LinkedInLeadGenformsubmission = 10228,
        LinkedInLeadGenintegrationconfiguration = 10223,
        LinkedInleadmatchingstrategy = 10229,
        LinkedInMatchedAudience = 10237,
        LinkedInuserprofile = 10230,
        LinkedQuestion = 10085,
        ListForm = 10177,
        ListValueMapping = 4418,
        Loan = 3,
        LoanProgram = 10059,
        LoanPurpose = 10060,
        LoanStatus = 10061,
        LoanType = 10062,
        LocalConfigStore = 9201,
        LookupMapping = 4419,
        Mailbox = 9606,
        MailboxAutoTrackingFolder = 9608,
        MailboxStatistics = 9607,
        MailboxTrackingCategory = 9609,
        MailMergeTemplate = 9106,
        Marketingactivity = 10193,
        Marketinganalyticsconfiguration = 10164,
        Marketingconfiguration = 10178,
        Marketingemail = 10180,
        Marketingemaildynamiccontentmetadata = 10181,
        Marketingemailtemplate = 10182,
        MarketingEmailTest = 10210,
        MarketingEmailTestAttribute = 10211,
        Marketingemailtestsend = 10183,
        Marketingfeatureconfiguration = 10234,
        Marketingfieldsubmission = 10212,
        Marketingform = 10184,
        Marketingformfield = 10185,
        Marketingformsubmission = 10213,
        Marketingformtemplate = 10186,
        Marketinglist = 4300,
        MarketingListMember = 4301,
        Marketingpage = 10188,
        Marketingpageconfiguration = 10189,
        Marketingpagetemplate = 10190,
        Marketingwebsite = 10202,
        Matchingstrategy = 10191,
        Matchingstrategyattributes = 10192,
        MetadataDifference = 4231,
        MicrosoftTeamsCollaborationentity = 10054,
        MicrosoftTeamsGraphresourceEntity = 10052,
        Migration = 10246,
        MobileOfflineProfile = 9866,
        MobileOfflineProfileItem = 9867,
        MobileOfflineProfileItemAssociation = 9868,
        ModeldrivenApp = 9006,
        MonthlyFiscalCalendar = 2003,
        MortgageLoanProcess = 10063,
        msdyn_msteamssetting = 10053,
        msdyn_relationshipinsightsunifiedconfig = 10029,
        MultiEntitySearch = 9910,
        MultiSelectOptionValue = 9912,
        NavigationSetting = 9900,
        NewProcess = 950,
        Note = 5,
        NotesanalysisConfig = 10038,
        Notification = 4110,
        ODatav4DataSource = 10002,
        OfficeDocument = 4490,
        OfficeGraphDocument = 9950,
        OfflineCommandDefinition = 9870,
        OpportunityClose = 4208,
        OpportunityCompetitors = 25,
        OpportunityProduct = 1083,
        OpportunityRelationship = 4503,
        OpportunitySalesProcess = 953,
        OptionSet = 9809,
        Order = 1088,
        OrderClose = 4209,
        OrderProduct = 1089,
        Organization = 1019,
        OrganizationInsightsMetric = 9699,
        OrganizationInsightsNotification = 9690,
        OrganizationStatistic = 4708,
        OrganizationUI = 1021,
        Owner = 7,
        OwnerMapping = 4420,
        Page = 10081,
        PartnerApplication = 1095,
        Pass = 10135,
        PersonalDocumentTemplate = 9941,
        Personalizedpage = 10244,
        Personalizedpagefield = 10245,
        PhoneCall = 4210,
        Phonecallactivitymarketingtemplate = 10194,
        PhoneToCaseProcess = 952,
        Playbook = 10022,
        Playbookactivity = 10019,
        Playbookactivityattribute = 10020,
        PlaybookCallableContext = 10018,
        Playbookcategory = 10021,
        Playbooktemplate = 10023,
        PluginAssembly = 4605,
        PluginProfile = 10072,
        PluginTraceLog = 4619,
        PluginType = 4602,
        PluginTypeStatistic = 4603,
        Portalsettings = 10195,
        Position = 50,
        Post = 8000,
        PostConfiguration = 10041,
        PostRegarding = 8002,
        PostRole = 8001,
        PostRuleConfiguration = 10042,
        PriceList = 1022,
        PriceListItem = 1026,
        PrincipalSyncAttributeMap = 1404,
        Privilege = 1023,
        PrivilegeObjectTypeCode = 31,
        Process = 4703,
        ProcessConfiguration = 9650,
        ProcessDependency = 4704,
        ProcessLog = 4706,
        ProcessSession = 4710,
        ProcessStage = 4724,
        ProcessTrigger = 4712,
        Product = 1024,
        ProductAssociation = 1025,
        ProductRelationship = 1028,
        ProductSalesLiterature = 21,
        ProfileAlbum = 10040,
        Property_10064 = 10064,
        Property_1048 = 1048,
        PropertyAssociation = 1235,
        PropertyInstance = 1333,
        PropertyOptionSetItem = 1049,
        Publisher = 7101,
        PublisherAddress = 7102,
        QuarterlyFiscalCalendar = 2002,
        Question = 10082,
        QuestionResponse = 10084,
        Queue = 2020,
        QueueItem = 2029,
        QueueItemCount = 2023,
        QueueMemberCount = 2024,
        QuickCampaign = 4406,
        Quotainfoentity = 10233,
        Quote = 1084,
        QuoteClose = 4211,
        QuoteProduct = 1085,
        RatingModel = 1144,
        RatingValue = 1142,
        RecordCreationandUpdateRule = 9300,
        RecordCreationandUpdateRuleItem = 9301,
        RecurrenceRule = 4250,
        RecurringAppointment = 4251,
        RedirectURL = 10196,
        RegistrationResponse = 10136,
        RelationshipRole = 4500,
        RelationshipRoleMap = 4501,
        ReplicationBacklog = 1140,
        Report = 9100,
        ReportLink = 9104,
        ReportRelatedCategory = 9102,
        ReportRelatedEntity = 9101,
        ReportVisibility = 9103,
        Resource = 4002,
        ResourceExpansion = 4010,
        ResourceGroup = 4007,
        ResourceSpecification = 4006,
        ResponseAction = 10086,
        ResponseCondition = 10088,
        ResponseError = 10089,
        ResponseOutcome = 10090,
        ResponseRouting = 10091,
        RibbonClientMetadata = 4579,
        RibbonCommand = 1116,
        RibbonContextGroup = 1115,
        RibbonDifference = 1130,
        RibbonMetadataToProcess = 9880,
        RibbonRule = 1117,
        RibbonTabToCommandMapping = 1113,
        RoleTemplate = 1037,
        RollupField = 9604,
        RollupJob = 9511,
        RollupProperties = 9510,
        RollupQuery = 9602,
        Room = 10137,
        RoomReservation = 10138,
        RoutingRuleSet = 8181,
        RuleItem = 8199,
        RuntimeDependency = 7200,
        SalesAttachment = 1070,
        salesinsightssettings = 10036,
        SalesLiterature = 1038,
        SalesProcessInstance = 32,
        SavedOrganizationInsightsConfiguration = 1309,
        SavedView = 4230,
        SchedulingGroup = 4005,
        SdkMessage = 4606,
        SdkMessageFilter = 4607,
        SdkMessagePair = 4613,
        SdkMessageProcessingStep = 4608,
        SdkMessageProcessingStepImage = 4615,
        SdkMessageProcessingStepSecureConfiguration = 4616,
        SdkMessageRequest = 4609,
        SdkMessageRequestField = 4614,
        SdkMessageResponse = 4610,
        SdkMessageResponseField = 4611,
        Section = 10092,
        SecurityRole = 1036,
        Segment = 10197,
        SemiannualFiscalCalendar = 2001,
        Service = 4001,
        ServiceActivity = 4214,
        ServiceContractContact = 20,
        ServiceEndpoint = 4618,
        Session = 10139,
        SessionRegistration = 10140,
        SessionTrack = 10141,
        SharePointData = 9509,
        SharepointDocument = 9507,
        SharePointSite = 9502,
        siconfig = 10030,
        SIKeyValueConfig = 10031,
        SimilarityRule = 9951,
        Site = 4009,
        SiteMap = 4709,
        SLA = 9750,
        SLAItem = 9751,
        SLAKPIInstance = 9752,
        SocialActivity = 4216,
        SocialInsightsConfiguration = 1300,
        Socialpost = 10241,
        SocialPostingConfiguration = 10242,
        SocialPostingConsent = 10243,
        SocialProfile = 99,
        Solution = 7100,
        SolutionComponent = 7103,
        SolutionComponentDataSource = 10001,
        SolutionComponentDefinition = 7104,
        SolutionComponentFileConfiguration = 10005,
        SolutionComponentSummary = 10000,
        SolutionHealthRule = 10069,
        SolutionHealthRuleArgument = 10070,
        SolutionHealthRuleSet = 10071,
        SolutionHistory = 10003,
        SolutionHistoryData = 9890,
        SolutionHistoryDataSource = 10004,
        SpamScoreActivity = 10238,
        SpamScoreRequest = 10239,
        Speaker = 10142,
        SpeakerEngagement = 10143,
        SponsorableArticle = 10144,
        Sponsorship = 10145,
        StatusMap = 1075,
        StringMap = 1043,
        Subject = 129,
        Subscription = 29,
        SubscriptionClients = 1072,
        SubscriptionManuallyTrackedObject = 37,
        SubscriptionStatisticOffline = 45,
        SubscriptionStatisticOutlook = 46,
        SubscriptionSyncEntryOffline = 47,
        SubscriptionSyncEntryOutlook = 48,
        SubscriptionSynchronizationInformation = 33,
        SuggestionCardTemplate = 1190,
        Survey = 10093,
        SurveyActivity = 10094,
        SurveyResponse = 10096,
        SyncAttributeMapping = 1401,
        SyncAttributeMappingProfile = 1400,
        SyncError = 9869,
        SystemApplicationMetadata = 7000,
        SystemChart = 1111,
        SystemForm = 1030,
        SystemJob = 4700,
        SystemUserBusinessUnitEntityMap = 42,
        SystemUserManagerMap = 51,
        SystemUserPrincipal = 14,
        Task = 4212,
        Taskactivitymarketingtemplate = 10199,
        Team = 9,
        TeamProfiles = 1203,
        TeamSyncAttributeMappingProfiles = 1403,
        Teamtemplate = 92,
        Territory = 2013,
        TextAnalyticsEntityMapping = 9945,
        TextAnalyticsTopic = 9948,
        Theme_10097 = 10097,
        Theme_2015 = 2015,
        TimeStampDateMapping = 9932,
        TimeZoneDefinition = 4810,
        TimeZoneLocalizedName = 4812,
        TimeZoneRule = 4811,
        TopicHistory = 9946,
        TopicModel = 9944,
        TopicModelConfiguration = 9942,
        TopicModelExecutionHistory = 9943,
        Trace = 8050,
        TraceAssociation = 8051,
        TraceRegarding = 8052,
        Trackinginformationfordeletedentities = 35,
        TransformationMapping = 4426,
        TransformationParameterMapping = 4427,
        TranslationProcess = 951,
        UICconfig = 10200,
        Unit = 1055,
        UnitGroup = 1056,
        UnresolvedAddress = 2012,
        UntrackedAppointment = 10037,
        UntrackedEmail = 4220,
        UpgradeRun = 10015,
        UpgradeStep = 10016,
        UpgradeVersion = 10017,
        User = 8,
        UserApplicationMetadata = 7001,
        UserChart = 1112,
        UserDashboard = 1031,
        UserEntityInstanceData = 2501,
        UserEntityUISettings = 2500,
        UserFiscalCalendar = 1086,
        Usergeoregion = 10201,
        UserMapping = 2016,
        UserSearchFacet = 52,
        UserSettings = 150,
        Venue = 10146,
        Video = 10106,
        View = 1039,
        VirtualEntityDataProvider = 78,
        VirtualEntityDataSource = 85,
        VoCImport = 10079,
        VoCResponseBlobStore = 10087,
        VoiceoftheCustomerConfiguration = 10098,
        VoiceoftheCustomerLog = 10095,
        WaitlistItem = 10147,
        WallView = 10043,
        WebApplication = 10148,
        WebinarConfiguration = 10149,
        WebinarConsent = 10150,
        WebinarProvider = 10151,
        WebinarType = 10152,
        WebResource = 9333,
        WebsiteEntityConfiguration = 10153,
        WebWizard = 4800,
        WebWizardAccessPrivilege = 4803,
        WizardPage = 4802,
        WorkflowWaitSubscription = 4702,
    }
    public enum DuplicateRuleStatusCode
    {
        Published = 2,
        Publishing = 1,
        Unpublished = 0,
    }
    public enum DynamicPropertyAssociationAssociationStatus
    {
        Active = 0,
        Deleted = 1,
        Draft = 2,
        DraftAdded = 3,
        DraftDeleted = 4,
    }
    public enum DynamicPropertyAssociationInheritanceState
    {
        Inherited = 0,
        Overridden = 1,
        Owned = 2,
    }
    public enum DynamicPropertyDataType
    {
        Decimal = 1,
        FloatingPointNumber = 2,
        OptionSet = 0,
        SingleLineOfText = 3,
        WholeNumber = 4,
    }
    public enum DynamicPropertyStatusCode
    {
        Active = 1,
        Draft = 0,
        Retired = 2,
    }
    public enum EmailCorrelationMethod
    {
        ConversationIndex = 5,
        CustomCorrelation = 7,
        InReplyTo = 3,
        None = 0,
        Skipped = 1,
        SmartMatching = 6,
        TrackingToken = 4,
        XHeader = 2,
    }
    public enum EmailEmailReminderStatus
    {
        NotSet = 0,
        ReminderExpired = 2,
        ReminderInvalid = 3,
        ReminderSet = 1,
    }
    public enum EmailEmailReminderType
    {
        IfIdonotreceiveareplyby = 0,
        Iftheemailisnotopenedby = 1,
        Remindmeanywaysat = 2,
    }
    public enum EmailNotifications
    {
        None = 0,
        ThemessagewassavedasaMicrosoftDynamics365emailrecordbutnotalltheattachmentscouldbesavedwithitAnattachmentcannotbesavedifitisblockedorifitsfiletypeisinvalid = 1,
        Truncatedbody = 2,
    }
    public enum EmailPriorityCode
    {
        High = 2,
        Low = 0,
        Normal = 1,
    }
    public enum EmailServerProfile_AuthenticationProtocol
    {
        AutoDetect = 0,
        Basic = 3,
        Negotiate = 1,
        NTLM = 2,
    }
    public enum EmailServerProfileExchangeVersion
    {
        Exchange2007 = 0,
        Exchange2007SP1 = 1,
        Exchange2010 = 2,
        Exchange2010SP1 = 3,
        Exchange2010SP2 = 4,
        Exchange2013 = 5,
    }
    public enum EmailServerProfileIncomingCredentialRetrieval
    {
        CredentialsSpecifiedbyaUserorQueue = 0,
        CredentialsSpecifiedinEmailServerProfile = 1,
        ServertoServerAuthentication = 2,
        WindowsIntegratedAuthentication = 3,
        WithoutCredentials_Anonymous = 4,
    }
    public enum EmailServerProfileLastAuthorizationStatus
    {
        Failure = 0,
        Success = 1,
    }
    public enum EmailServerProfileLastTestExecutionStatus
    {
        Failure = 0,
        Success = 1,
        Warning = 2,
    }
    public enum EmailServerProfileLastTestValidationStatus
    {
        Failure = 0,
        Success = 1,
    }
    public enum EmailServerProfileOutgoingCredentialRetrieval
    {
        CredentialsSpecifiedbyaUserorQueue = 0,
        CredentialsSpecifiedinEmailServerProfile = 1,
        ServertoServerAuthentication = 2,
        WindowsIntegratedAuthentication = 3,
        WithoutCredentials_Anonymous = 4,
    }
    public enum EmailServerProfileServerType
    {
        ExchangeOnline_Hybrid = 3,
        ExchangeServer = 0,
        ExchangeServer_Hybrid = 2,
        IMAPSMTP = 4,
        Other_POP3SMTP = 1,
    }
    public enum EmailServerProfileStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum EmailStatusCode
    {
        Canceled = 5,
        Completed = 2,
        Draft = 1,
        Failed = 8,
        PendingSend = 6,
        Received = 4,
        Sending = 7,
        Sent = 3,
    }
    public enum EntitlementAllocationTypeCode
    {
        Numberofcases = 0,
        Numberofhours = 1,
    }
    public enum EntitlementDecreaseRemainingOn
    {
        CaseCreation = 1,
        CaseResolution = 0,
    }
    public enum EntitlementEntityAllocationTypeMappingStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum EntitlementKbAccessLevel
    {
        None = 2,
        Premium = 1,
        Standard = 0,
    }
    public enum EntitlementStatusCode
    {
        Active = 1,
        Cancelled = 2,
        Draft = 0,
        Expired = 3,
        Waiting = 1200,
    }
    public enum EntitlementTemplateAllocationTypeCode
    {
        Numberofcases = 0,
        Numberofhours = 1,
    }
    public enum EntitlementTemplateDecreaseRemainingOn
    {
        CaseCreation = 1,
        CaseResolution = 0,
    }
    public enum EntitlementTemplateKbAccessLevel
    {
        None = 2,
        Premium = 1,
        Standard = 0,
    }
    public enum EntityImageConfigStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum EntityType
    {
        Case = 0,
    }
    public enum EnvironmentVariableDefinitionStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum EnvironmentVariableDefinitionType
    {
        Boolean = 100000002,
        JSON = 100000003,
        Number = 100000001,
        String = 100000000,
    }
    public enum EnvironmentVariableValueStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum ExpiredProcessStatusCode
    {
        Aborted = 3,
        Active = 1,
        Finished = 2,
    }
    public enum externalpartyitem_status_code
    {
        Disabled = 2,
        Enabled = 1,
    }
    public enum ExternalPartyStatusCode
    {
        Disabled = 2,
        Enabled = 1,
    }
    public enum FaxPriorityCode
    {
        High = 2,
        Low = 0,
        Normal = 1,
    }
    public enum FaxStatusCode
    {
        Canceled = 5,
        Completed = 2,
        Open = 1,
        Received = 4,
        Sent = 3,
    }
    public enum FeedbackSource
    {
        Internal = 0,
        Portal = 1,
    }
    public enum FeedbackStatusCode
    {
        Accepted = 2,
        Closed = 3,
        Proposed = 1,
        Rejected = 4,
    }
    public enum Field_Security_Permission_Type
    {
        Allowed = 4,
        NotAllowed = 0,
    }
    public enum flipswitch_options
    {
        Off = 0,
        On = 1,
    }
    public enum Goal_FiscalPeriod
    {
        Annual = 301,
        April = 104,
        August = 108,
        December = 112,
        February = 102,
        January = 101,
        July = 107,
        June = 106,
        March = 103,
        May = 105,
        November = 111,
        October = 110,
        P1 = 401,
        P10 = 410,
        P11 = 411,
        P12 = 412,
        P13 = 413,
        P2 = 402,
        P3 = 403,
        P4 = 404,
        P5 = 405,
        P6 = 406,
        P7 = 407,
        P8 = 408,
        P9 = 409,
        Quarter1 = 1,
        Quarter2 = 2,
        Quarter3 = 3,
        Quarter4 = 4,
        Semester1 = 201,
        Semester2 = 202,
        September = 109,
    }
    public enum Goal_FiscalYear
    {
        FY1970 = 1970,
        FY1971 = 1971,
        FY1972 = 1972,
        FY1973 = 1973,
        FY1974 = 1974,
        FY1975 = 1975,
        FY1976 = 1976,
        FY1977 = 1977,
        FY1978 = 1978,
        FY1979 = 1979,
        FY1980 = 1980,
        FY1981 = 1981,
        FY1982 = 1982,
        FY1983 = 1983,
        FY1984 = 1984,
        FY1985 = 1985,
        FY1986 = 1986,
        FY1987 = 1987,
        FY1988 = 1988,
        FY1989 = 1989,
        FY1990 = 1990,
        FY1991 = 1991,
        FY1992 = 1992,
        FY1993 = 1993,
        FY1994 = 1994,
        FY1995 = 1995,
        FY1996 = 1996,
        FY1997 = 1997,
        FY1998 = 1998,
        FY1999 = 1999,
        FY2000 = 2000,
        FY2001 = 2001,
        FY2002 = 2002,
        FY2003 = 2003,
        FY2004 = 2004,
        FY2005 = 2005,
        FY2006 = 2006,
        FY2007 = 2007,
        FY2008 = 2008,
        FY2009 = 2009,
        FY2010 = 2010,
        FY2011 = 2011,
        FY2012 = 2012,
        FY2013 = 2013,
        FY2014 = 2014,
        FY2015 = 2015,
        FY2016 = 2016,
        FY2017 = 2017,
        FY2018 = 2018,
        FY2019 = 2019,
        FY2020 = 2020,
        FY2021 = 2021,
        FY2022 = 2022,
        FY2023 = 2023,
        FY2024 = 2024,
        FY2025 = 2025,
        FY2026 = 2026,
        FY2027 = 2027,
        FY2028 = 2028,
        FY2029 = 2029,
        FY2030 = 2030,
        FY2031 = 2031,
        FY2032 = 2032,
        FY2033 = 2033,
        FY2034 = 2034,
        FY2035 = 2035,
        FY2036 = 2036,
        FY2037 = 2037,
        FY2038 = 2038,
    }
    public enum GoalRollupQueryStatusCode
    {
        Closed = 1,
        Open = 0,
    }
    public enum GoalStatusCode
    {
        Closed = 1,
        Discarded = 2,
        Open = 0,
    }
    public enum ImportEntityMappingDeDupe
    {
        Eliminate = 2,
        Ignore = 1,
    }
    public enum ImportEntityMappingProcessCode
    {
        Ignore = 2,
        Internal = 3,
        Process = 1,
    }
    public enum ImportEntityMappingStatusCode
    {
        Active = 1,
    }
    public enum ImportFileDataDelimiterCode
    {
        DoubleQuote = 1,
        None = 2,
        SingleQuote = 3,
    }
    public enum ImportFileFieldDelimiterCode
    {
        Colon = 1,
        Comma = 2,
        Semicolon = 4,
        Tab = 3,
    }
    public enum ImportFileFileTypeCode
    {
        Attachment = 2,
        CSV = 0,
        XLSX = 3,
        XMLSpreadsheet2003 = 1,
    }
    public enum ImportFileProcessCode
    {
        Ignore = 2,
        Internal = 3,
        Process = 1,
    }
    public enum ImportFileProcessingStatus
    {
        ComplexTransformation = 4,
        ImportComplete = 11,
        ImportPass1 = 9,
        ImportPass2 = 10,
        LookupTransformation = 5,
        NotStarted = 1,
        OwnerTransformation = 7,
        Parsing = 2,
        ParsingComplete = 3,
        PicklistTransformation = 6,
        PrimaryKeyTransformation = 12,
        TransformationComplete = 8,
    }
    public enum ImportFileStatusCode
    {
        Completed = 4,
        Failed = 5,
        Importing = 3,
        Parsing = 1,
        Submitted = 0,
        Transforming = 2,
    }
    public enum ImportFileUpsertModeCode
    {
        Create = 0,
        Ignore = 2,
        Update = 1,
    }
    public enum ImportLogLogPhaseCode
    {
        ImportCreate = 2,
        ImportUpdate = 3,
        Parse = 0,
        Transform = 1,
    }
    public enum ImportLogStatusCode
    {
        Active = 0,
    }
    public enum ImportMapEntitiesPerFile
    {
        MultipleEntitiesPerFile = 2,
        SingleEntityPerFile = 1,
    }
    public enum ImportMapImportMapType
    {
        InProcess = 3,
        OutofBox = 2,
        Standard = 1,
    }
    public enum ImportMapSourceType
    {
        GenericMapforContactandAccount = 5,
        MapForSalesForcecomContactandAccountReportExport = 3,
        MapForSalesForcecomFullDataExport = 1,
        MapForSalesForcecomReportExport = 2,
        MicrosoftOfficeOutlook2010withBusinessContactManager = 4,
    }
    public enum ImportMapStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum ImportMapTargetEntity
    {
        Account = 1,
        AccountLeads = 16,
        ACIViewMapper = 8040,
        ActionCard = 9962,
        actioncardregarding = 10032,
        ActionCardRoleSetting = 10033,
        ActionCardType = 9983,
        ActionCardUserSettings = 9973,
        ActionCardUserState = 9968,
        Activity = 4200,
        ActivityParty = 135,
        Address = 1071,
        admin_settings_entity = 10025,
        AdvancedSimilarityRule = 9949,
        AIConfiguration = 402,
        AIFormProcessingDocument = 10008,
        AIModel = 401,
        AIObjectDetectionBoundingBox = 10011,
        AIObjectDetectionImage = 10009,
        AIObjectDetectionImageMapping = 10012,
        AIObjectDetectionLabel = 10010,
        AITemplate = 400,
        AnalysisComponent = 10065,
        AnalysisJob = 10066,
        AnalysisResult = 10067,
        AnalysisResultDetail = 10068,
        Announcement = 132,
        AnnualFiscalCalendar = 2000,
        Answer = 10073,
        AppConfigMaster = 9011,
        AppConfigSetup = 10055,
        AppConfiguration = 9012,
        AppConfigurationInstance = 9013,
        ApplicationFile = 4707,
        ApplicationRibbons = 1120,
        AppModuleComponent = 9007,
        AppModuleMetadata = 8700,
        AppModuleMetadataAsyncOperation = 8702,
        AppModuleMetadataDependency = 8701,
        AppModuleRoles = 9009,
        Appointment = 4201,
        Appointmentactivitymarketingtemplate = 10162,
        Article = 127,
        ArticleComment = 1082,
        ArticleTemplate = 1016,
        Attachment_1001 = 1001,
        Attachment_1002 = 1002,
        AttendeePass = 10110,
        Attribute = 9808,
        AttributeImageConfig = 431,
        AttributeMap = 4601,
        Auditing = 4567,
        AuthenticatedDomain = 10198,
        AuthenticationSettings = 10111,
        AuthorizationServer = 1094,
        AzureDeployment = 10074,
        AzureServiceConnection = 9936,
        BookableResource = 1150,
        BookableResourceBooking = 1145,
        BookableResourceBookingHeader = 1146,
        BookableResourceBookingtoExchangeIdMapping = 4421,
        BookableResourceCategory = 1147,
        BookableResourceCategoryAssn = 1149,
        BookableResourceCharacteristic = 1148,
        BookableResourceGroup = 1151,
        BookingStatus = 1152,
        Bucket = 10113,
        Building = 10114,
        BulkDeleteFailure = 4425,
        BulkDeleteOperation = 4424,
        BulkOperationLog = 4405,
        BusinessDataLocalizedLabel = 4232,
        BusinessProcessFlowInstance = 4725,
        BusinessUnit = 10,
        BusinessUnitMap = 6,
        Calendar = 4003,
        CalendarRule = 4004,
        CallbackRegistration = 301,
        Campaign = 4400,
        CampaignActivity = 4402,
        CampaignActivityItem = 4404,
        CampaignItem = 4403,
        CampaignResponse = 4401,
        CanvasApp = 300,
        Case = 112,
        CaseResolution = 4206,
        Category = 9959,
        CDNconfiguration = 10163,
        ChannelAccessProfile = 3005,
        ChannelAccessProfileRule = 9400,
        ChannelAccessProfileRuleItem = 9401,
        ChannelProperty = 1236,
        ChannelPropertyGroup = 1234,
        Characteristic = 1141,
        Checkin = 10115,
        ChildIncidentCount = 113,
        Clientupdate = 36,
        ColumnMapping = 4417,
        Comment = 8005,
        Commitment = 4215,
        Competitor = 123,
        CompetitorAddress = 1004,
        CompetitorProduct = 1006,
        CompetitorSalesLiterature = 26,
        ComponentLayer = 10006,
        ComponentLayerDataSource = 10007,
        Configuration = 10056,
        Connection = 3234,
        ConnectionRole = 3231,
        ConnectionRoleObjectTypeCode = 3233,
        Connector = 372,
        Contact = 2,
        ContactInvoices = 17,
        ContactLeads = 22,
        ContactOrders = 19,
        ContactQuotes = 18,
        ContactType = 10057,
        Contentblock = 10108,
        Contentsettings = 10165,
        Contract = 1010,
        ContractLine = 1011,
        ContractTemplate = 2011,
        Currency = 9105,
        Customchannelactivity = 10168,
        CustomControl = 9753,
        CustomControlDefaultConfig = 9755,
        CustomControlResource = 9754,
        Customerinsightsinformation = 10166,
        Customerjourney = 10167,
        Customerjourneyiteration = 10169,
        Customerjourneyruntimestate = 10170,
        Customerjourneytemplate = 10171,
        Customerjourneyworkflowlink = 10172,
        CustomerRelationship = 4502,
        CustomRegistrationField = 10116,
        DatabaseVersion = 10014,
        DataImport = 4410,
        DataMap = 4411,
        DataPerformanceDashboard = 4450,
        Defaultmarketingsettings = 10173,
        DelveActionHub = 9961,
        Dependency = 7105,
        DependencyFeature = 7108,
        DependencyNode = 7106,
        Designerfeatureprotection = 10109,
        Digitalassetsconfiguration = 10103,
        Discount = 1013,
        DiscountList = 1080,
        DisplayString = 4102,
        DisplayStringMap = 4101,
        DocumentLocation = 9508,
        DocumentSuggestions = 1189,
        DocumentTemplate = 9940,
        DuplicateDetectionRule = 4414,
        DuplicateRecord = 4415,
        DuplicateRuleCondition = 4416,
        Dynamiccontentmetadata = 10179,
        Email = 4202,
        EmailHash = 4023,
        EmailSearch = 4299,
        EmailServerProfile = 9605,
        EmailSignature = 9997,
        EmailTemplate = 2010,
        Entitlement = 9700,
        EntitlementChannel = 9701,
        EntitlementContact = 7272,
        EntitlementEntityAllocationTypeMapping = 9704,
        EntitlementProduct = 6363,
        EntitlementTemplate = 9702,
        EntitlementTemplateChannel = 9703,
        EntitlementTemplateProduct = 4545,
        Entity = 9800,
        EntityAnalyticsConfig = 430,
        EntityCounter = 10117,
        EntityImageConfig = 432,
        EntityMap = 4600,
        EntityRankingRule = 10034,
        EnvironmentVariableDefinition = 380,
        EnvironmentVariableValue = 381,
        Event = 10118,
        EventAdministration = 10119,
        EventCustomRegistrationField = 10120,
        EventMainBusinessProcessFlow = 10112,
        EventManagementActivity = 10121,
        EventManagementConfiguration = 10122,
        EventPurchase = 10123,
        EventPurchaseAttendee = 10124,
        EventPurchaseContact = 10125,
        EventPurchasePass = 10126,
        EventRegistration = 10127,
        EventTeamMember = 10128,
        EventVendor = 10129,
        ExchangeSyncIdMapping = 4120,
        ExpanderEvent = 4711,
        ExpiredProcess = 955,
        ExternalParty = 3008,
        ExternalPartyItem = 9987,
        FacilityEquipment = 4000,
        Fax = 4204,
        Feedback = 9958,
        FeedbackMapping = 10075,
        FeedbackRelatedSurvey = 10076,
        FieldPermission = 1201,
        FieldSecurityProfile = 1200,
        FieldSharing = 44,
        File = 10104,
        File_OBSOLETE = 10130,
        FileAttachment = 55,
        Filter = 10044,
        FilterTemplate = 30,
        FixedMonthlyFiscalCalendar = 2004,
        flowcardtype = 10035,
        Follow = 8003,
        Forecast = 10027,
        Forecastdefinition = 10026,
        Forecastrecurrence = 10028,
        Formpage = 10174,
        Formpagetemplate = 10175,
        FormsProsurvey = 10048,
        FormsProsurveyemailtemplate = 10045,
        FormsProsurveyinvite = 10049,
        FormsProsurveyquestion = 10046,
        FormsProsurveyquestionresponse = 10047,
        FormsProsurveyresponse = 10050,
        FormsProunsubscribedrecipient = 10051,
        Formwhitelistrule = 10187,
        GDPRconfiguration = 10235,
        GDPRconsentchangerecord = 10236,
        Geopin = 10176,
        GlobalSearchConfiguration = 54,
        Goal = 9600,
        GoalMetric = 9603,
        Grid = 10083,
        Gwennolfeatureconfiguration = 10240,
        HierarchyRule = 8840,
        HierarchySecurityConfiguration = 9919,
        HolidayWrapper = 9996,
        Hotel = 10131,
        HotelRoomAllocation = 10132,
        HotelRoomReservation = 10133,
        icebreakersconfig = 10039,
        Image = 10077,
        ImageDescriptor = 1007,
        ImageTokenCache = 10078,
        ImportData = 4413,
        ImportEntityMapping = 4428,
        ImportJob = 9107,
        ImportLog = 4423,
        ImportSourceFile = 4412,
        IncidentKnowledgeBaseRecord = 9931,
        IndexedArticle = 126,
        IntegrationStatus = 3000,
        InteractionforEmail = 9986,
        InternalAddress = 1003,
        InterProcessLock = 4011,
        InvalidDependency = 7107,
        Invoice = 1090,
        InvoiceProduct = 1091,
        ISVConfig = 4705,
        Keyword = 10105,
        KnowledgeArticle = 9953,
        KnowledgeArticleCategory = 9960,
        KnowledgeArticleIncident = 9954,
        KnowledgeArticleViews = 9955,
        KnowledgeBaseRecord = 9930,
        KnowledgeSearchModel = 9947,
        Language = 9957,
        LanguageProvisioningState = 9875,
        Layout = 10134,
        Lead = 4,
        LeadAddress = 1017,
        LeadCompetitors = 24,
        Leadentityfield = 10219,
        LeadProduct = 27,
        Leadscore = 10217,
        Leadscore_Deprecated = 10214,
        LeadScoringConfiguration = 10218,
        LeadScoringModel = 10215,
        LeadSource = 10058,
        Leadtoopportunity = 10216,
        LeadToOpportunitySalesProcess = 954,
        Letter = 4207,
        License = 2027,
        Like = 8006,
        LinkedAnswer = 10080,
        LinkedInaccount = 10220,
        LinkedInactivity = 10221,
        LinkedIncampaign = 10222,
        LinkedInfieldmapping = 10224,
        LinkedInformquestion = 10227,
        LinkedInformsubmissionanswer = 10226,
        LinkedInLeadGenform = 10225,
        LinkedInLeadGenformsubmission = 10228,
        LinkedInLeadGenintegrationconfiguration = 10223,
        LinkedInleadmatchingstrategy = 10229,
        LinkedInMatchedAudience = 10237,
        LinkedInuserprofile = 10230,
        LinkedQuestion = 10085,
        ListForm = 10177,
        ListValueMapping = 4418,
        Loan = 3,
        LoanProgram = 10059,
        LoanPurpose = 10060,
        LoanStatus = 10061,
        LoanType = 10062,
        LocalConfigStore = 9201,
        LookupMapping = 4419,
        Mailbox = 9606,
        MailboxAutoTrackingFolder = 9608,
        MailboxStatistics = 9607,
        MailboxTrackingCategory = 9609,
        MailMergeTemplate = 9106,
        Marketingactivity = 10193,
        Marketinganalyticsconfiguration = 10164,
        Marketingconfiguration = 10178,
        Marketingemail = 10180,
        Marketingemaildynamiccontentmetadata = 10181,
        Marketingemailtemplate = 10182,
        MarketingEmailTest = 10210,
        MarketingEmailTestAttribute = 10211,
        Marketingemailtestsend = 10183,
        Marketingfeatureconfiguration = 10234,
        Marketingfieldsubmission = 10212,
        Marketingform = 10184,
        Marketingformfield = 10185,
        Marketingformsubmission = 10213,
        Marketingformtemplate = 10186,
        Marketinglist = 4300,
        MarketingListMember = 4301,
        Marketingpage = 10188,
        Marketingpageconfiguration = 10189,
        Marketingpagetemplate = 10190,
        Marketingwebsite = 10202,
        Matchingstrategy = 10191,
        Matchingstrategyattributes = 10192,
        MetadataDifference = 4231,
        MicrosoftTeamsCollaborationentity = 10054,
        MicrosoftTeamsGraphresourceEntity = 10052,
        Migration = 10246,
        MobileOfflineProfile = 9866,
        MobileOfflineProfileItem = 9867,
        MobileOfflineProfileItemAssociation = 9868,
        ModeldrivenApp = 9006,
        MonthlyFiscalCalendar = 2003,
        MortgageLoanProcess = 10063,
        msdyn_msteamssetting = 10053,
        msdyn_relationshipinsightsunifiedconfig = 10029,
        MultiEntitySearch = 9910,
        MultiSelectOptionValue = 9912,
        NavigationSetting = 9900,
        NewProcess = 950,
        Note = 5,
        NotesanalysisConfig = 10038,
        Notification = 4110,
        ODatav4DataSource = 10002,
        OfficeDocument = 4490,
        OfficeGraphDocument = 9950,
        OfflineCommandDefinition = 9870,
        OpportunityClose = 4208,
        OpportunityCompetitors = 25,
        OpportunityProduct = 1083,
        OpportunityRelationship = 4503,
        OpportunitySalesProcess = 953,
        OptionSet = 9809,
        Order = 1088,
        OrderClose = 4209,
        OrderProduct = 1089,
        Organization = 1019,
        OrganizationInsightsMetric = 9699,
        OrganizationInsightsNotification = 9690,
        OrganizationStatistic = 4708,
        OrganizationUI = 1021,
        Owner = 7,
        OwnerMapping = 4420,
        Page = 10081,
        PartnerApplication = 1095,
        Pass = 10135,
        PersonalDocumentTemplate = 9941,
        Personalizedpage = 10244,
        Personalizedpagefield = 10245,
        PhoneCall = 4210,
        Phonecallactivitymarketingtemplate = 10194,
        PhoneToCaseProcess = 952,
        Playbook = 10022,
        Playbookactivity = 10019,
        Playbookactivityattribute = 10020,
        PlaybookCallableContext = 10018,
        Playbookcategory = 10021,
        Playbooktemplate = 10023,
        PluginAssembly = 4605,
        PluginProfile = 10072,
        PluginTraceLog = 4619,
        PluginType = 4602,
        PluginTypeStatistic = 4603,
        Portalsettings = 10195,
        Position = 50,
        Post = 8000,
        PostConfiguration = 10041,
        PostRegarding = 8002,
        PostRole = 8001,
        PostRuleConfiguration = 10042,
        PriceList = 1022,
        PriceListItem = 1026,
        PrincipalSyncAttributeMap = 1404,
        Privilege = 1023,
        PrivilegeObjectTypeCode = 31,
        Process = 4703,
        ProcessConfiguration = 9650,
        ProcessDependency = 4704,
        ProcessLog = 4706,
        ProcessSession = 4710,
        ProcessStage = 4724,
        ProcessTrigger = 4712,
        Product = 1024,
        ProductAssociation = 1025,
        ProductRelationship = 1028,
        ProductSalesLiterature = 21,
        ProfileAlbum = 10040,
        Property_10064 = 10064,
        Property_1048 = 1048,
        PropertyAssociation = 1235,
        PropertyInstance = 1333,
        PropertyOptionSetItem = 1049,
        Publisher = 7101,
        PublisherAddress = 7102,
        QuarterlyFiscalCalendar = 2002,
        Question = 10082,
        QuestionResponse = 10084,
        Queue = 2020,
        QueueItem = 2029,
        QueueItemCount = 2023,
        QueueMemberCount = 2024,
        QuickCampaign = 4406,
        Quotainfoentity = 10233,
        Quote = 1084,
        QuoteClose = 4211,
        QuoteProduct = 1085,
        RatingModel = 1144,
        RatingValue = 1142,
        RecordCreationandUpdateRule = 9300,
        RecordCreationandUpdateRuleItem = 9301,
        RecurrenceRule = 4250,
        RecurringAppointment = 4251,
        RedirectURL = 10196,
        RegistrationResponse = 10136,
        RelationshipRole = 4500,
        RelationshipRoleMap = 4501,
        ReplicationBacklog = 1140,
        Report = 9100,
        ReportLink = 9104,
        ReportRelatedCategory = 9102,
        ReportRelatedEntity = 9101,
        ReportVisibility = 9103,
        Resource = 4002,
        ResourceExpansion = 4010,
        ResourceGroup = 4007,
        ResourceSpecification = 4006,
        ResponseAction = 10086,
        ResponseCondition = 10088,
        ResponseError = 10089,
        ResponseOutcome = 10090,
        ResponseRouting = 10091,
        RibbonClientMetadata = 4579,
        RibbonCommand = 1116,
        RibbonContextGroup = 1115,
        RibbonDifference = 1130,
        RibbonMetadataToProcess = 9880,
        RibbonRule = 1117,
        RibbonTabToCommandMapping = 1113,
        RoleTemplate = 1037,
        RollupField = 9604,
        RollupJob = 9511,
        RollupProperties = 9510,
        RollupQuery = 9602,
        Room = 10137,
        RoomReservation = 10138,
        RoutingRuleSet = 8181,
        RuleItem = 8199,
        RuntimeDependency = 7200,
        SalesAttachment = 1070,
        salesinsightssettings = 10036,
        SalesLiterature = 1038,
        SalesProcessInstance = 32,
        SavedOrganizationInsightsConfiguration = 1309,
        SavedView = 4230,
        SchedulingGroup = 4005,
        SdkMessage = 4606,
        SdkMessageFilter = 4607,
        SdkMessagePair = 4613,
        SdkMessageProcessingStep = 4608,
        SdkMessageProcessingStepImage = 4615,
        SdkMessageProcessingStepSecureConfiguration = 4616,
        SdkMessageRequest = 4609,
        SdkMessageRequestField = 4614,
        SdkMessageResponse = 4610,
        SdkMessageResponseField = 4611,
        Section = 10092,
        SecurityRole = 1036,
        Segment = 10197,
        SemiannualFiscalCalendar = 2001,
        Service = 4001,
        ServiceActivity = 4214,
        ServiceContractContact = 20,
        ServiceEndpoint = 4618,
        Session = 10139,
        SessionRegistration = 10140,
        SessionTrack = 10141,
        SharePointData = 9509,
        SharepointDocument = 9507,
        SharePointSite = 9502,
        siconfig = 10030,
        SIKeyValueConfig = 10031,
        SimilarityRule = 9951,
        Site = 4009,
        SiteMap = 4709,
        SLA = 9750,
        SLAItem = 9751,
        SLAKPIInstance = 9752,
        SocialActivity = 4216,
        SocialInsightsConfiguration = 1300,
        Socialpost = 10241,
        SocialPostingConfiguration = 10242,
        SocialPostingConsent = 10243,
        SocialProfile = 99,
        Solution = 7100,
        SolutionComponent = 7103,
        SolutionComponentDataSource = 10001,
        SolutionComponentDefinition = 7104,
        SolutionComponentFileConfiguration = 10005,
        SolutionComponentSummary = 10000,
        SolutionHealthRule = 10069,
        SolutionHealthRuleArgument = 10070,
        SolutionHealthRuleSet = 10071,
        SolutionHistory = 10003,
        SolutionHistoryData = 9890,
        SolutionHistoryDataSource = 10004,
        SpamScoreActivity = 10238,
        SpamScoreRequest = 10239,
        Speaker = 10142,
        SpeakerEngagement = 10143,
        SponsorableArticle = 10144,
        Sponsorship = 10145,
        StatusMap = 1075,
        StringMap = 1043,
        Subject = 129,
        Subscription = 29,
        SubscriptionClients = 1072,
        SubscriptionManuallyTrackedObject = 37,
        SubscriptionStatisticOffline = 45,
        SubscriptionStatisticOutlook = 46,
        SubscriptionSyncEntryOffline = 47,
        SubscriptionSyncEntryOutlook = 48,
        SubscriptionSynchronizationInformation = 33,
        SuggestionCardTemplate = 1190,
        Survey = 10093,
        SurveyActivity = 10094,
        SurveyResponse = 10096,
        SyncAttributeMapping = 1401,
        SyncAttributeMappingProfile = 1400,
        SyncError = 9869,
        SystemApplicationMetadata = 7000,
        SystemChart = 1111,
        SystemForm = 1030,
        SystemJob = 4700,
        SystemUserBusinessUnitEntityMap = 42,
        SystemUserManagerMap = 51,
        SystemUserPrincipal = 14,
        Task = 4212,
        Taskactivitymarketingtemplate = 10199,
        Team = 9,
        TeamProfiles = 1203,
        TeamSyncAttributeMappingProfiles = 1403,
        Teamtemplate = 92,
        Territory = 2013,
        TextAnalyticsEntityMapping = 9945,
        TextAnalyticsTopic = 9948,
        Theme_10097 = 10097,
        Theme_2015 = 2015,
        TimeStampDateMapping = 9932,
        TimeZoneDefinition = 4810,
        TimeZoneLocalizedName = 4812,
        TimeZoneRule = 4811,
        TopicHistory = 9946,
        TopicModel = 9944,
        TopicModelConfiguration = 9942,
        TopicModelExecutionHistory = 9943,
        Trace = 8050,
        TraceAssociation = 8051,
        TraceRegarding = 8052,
        Trackinginformationfordeletedentities = 35,
        TransformationMapping = 4426,
        TransformationParameterMapping = 4427,
        TranslationProcess = 951,
        UICconfig = 10200,
        Unit = 1055,
        UnitGroup = 1056,
        UnresolvedAddress = 2012,
        UntrackedAppointment = 10037,
        UntrackedEmail = 4220,
        UpgradeRun = 10015,
        UpgradeStep = 10016,
        UpgradeVersion = 10017,
        User = 8,
        UserApplicationMetadata = 7001,
        UserChart = 1112,
        UserDashboard = 1031,
        UserEntityInstanceData = 2501,
        UserEntityUISettings = 2500,
        UserFiscalCalendar = 1086,
        Usergeoregion = 10201,
        UserMapping = 2016,
        UserSearchFacet = 52,
        UserSettings = 150,
        Venue = 10146,
        Video = 10106,
        View = 1039,
        VirtualEntityDataProvider = 78,
        VirtualEntityDataSource = 85,
        VoCImport = 10079,
        VoCResponseBlobStore = 10087,
        VoiceoftheCustomerConfiguration = 10098,
        VoiceoftheCustomerLog = 10095,
        WaitlistItem = 10147,
        WallView = 10043,
        WebApplication = 10148,
        WebinarConfiguration = 10149,
        WebinarConsent = 10150,
        WebinarProvider = 10151,
        WebinarType = 10152,
        WebResource = 9333,
        WebsiteEntityConfiguration = 10153,
        WebWizard = 4800,
        WebWizardAccessPrivilege = 4803,
        WizardPage = 4802,
        WorkflowWaitSubscription = 4702,
    }
    public enum ImportModeCode
    {
        Create = 0,
        Update = 1,
    }
    public enum ImportStatusCode
    {
        Completed = 4,
        Failed = 5,
        Importing = 3,
        Parsing = 1,
        Submitted = 0,
        Transforming = 2,
    }
    public enum ims_appconfigsetupStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum ims_configurationims_valuetype
    {
        Dynamic = 176390001,
        Static = 176390000,
    }
    public enum ims_configurationStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum ims_contacttypeStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum ims_leadsourceStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum ims_loanprogramStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum ims_loanpurposeStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum ims_loanstatusStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum ims_loantypeStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum ims_mortgageloanprocessStatusCode
    {
        Aborted = 3,
        Active = 1,
        Finished = 2,
    }
    public enum ims_propertyims_mortgagestatus
    {
        AppliedforMortgage = 176390001,
        UnderMortgage = 176390000,
    }
    public enum ims_propertyStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum Incident_CaseOriginCode
    {
        Email = 2,
        Facebook = 2483,
        Phone = 1,
        Twitter = 3986,
        Web = 3,
    }
    public enum IncidentCaseTypeCode
    {
        Problem = 2,
        Question = 1,
        Request = 3,
    }
    public enum IncidentContractServiceLevelCode
    {
        Bronze = 3,
        Gold = 1,
        Silver = 2,
    }
    public enum IncidentCustomerSatisfactionCode
    {
        Dissatisfied = 2,
        Neutral = 3,
        Satisfied = 4,
        VeryDissatisfied = 1,
        VerySatisfied = 5,
    }
    public enum IncidentFirstResponseSLAStatus
    {
        InProgress = 1,
        NearingNoncompliance = 2,
        Noncompliant = 4,
        Succeeded = 3,
    }
    public enum IncidentIncidentStageCode
    {
        DefaultValue = 1,
    }
    public enum IncidentPriorityCode
    {
        High = 1,
        Low = 3,
        Normal = 2,
    }
    public enum IncidentResolutionInstanceTypeCode
    {
        NotRecurring = 0,
        RecurringException = 3,
        RecurringFutureException = 4,
        RecurringInstance = 2,
        RecurringMaster = 1,
    }
    public enum IncidentResolutionPriorityCode
    {
        High = 2,
        Low = 0,
        Normal = 1,
    }
    public enum IncidentResolutionStatusCode
    {
        Canceled = 3,
        Closed = 2,
        Open = 1,
    }
    public enum IncidentResolveBySLAStatus
    {
        InProgress = 1,
        NearingNoncompliance = 2,
        Noncompliant = 4,
        Succeeded = 3,
    }
    public enum IncidentSeverityCode
    {
        DefaultValue = 1,
    }
    public enum IncidentStatusCode
    {
        Cancelled = 6,
        InformationProvided = 1000,
        InProgress = 1,
        Merged = 2000,
        OnHold = 2,
        ProblemSolved = 5,
        Researching = 4,
        WaitingforDetails = 3,
    }
    public enum InitialCommunication
    {
        Contacted = 0,
        NotContacted = 1,
    }
    public enum InteractionForEmailInteractionType
    {
        AttachmentOpen = 2,
        EmailOpen = 0,
        EmailReply = 3,
        LinkOpen = 1,
    }
    public enum InteractionForEmailStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum InvoiceDetailInvoiceStateCode
    {
    }
    public enum InvoiceDetailShipTo_FreightTermsCode
    {
        FOB = 1,
        NoCharge = 2,
    }
    public enum InvoicePaymentTermsCode
    {
        _210Net30 = 2,
        Net30 = 1,
        Net45 = 3,
        Net60 = 4,
    }
    public enum InvoicePriorityCode
    {
        DefaultValue = 1,
    }
    public enum InvoiceShippingMethodCode
    {
        Airborne = 1,
        DHL = 2,
        FedEx = 3,
        FullLoad = 6,
        PostalMail = 5,
        UPS = 4,
        WillCall = 7,
    }
    public enum InvoiceShipTo_FreightTermsCode
    {
        DefaultValue = 1,
    }
    public enum InvoiceStatusCode
    {
        Billed = 4,
        Booked_appliestoservices = 5,
        Canceled = 100003,
        Canceled_deprecated = 3,
        Complete = 100001,
        Installed_appliestoservices = 6,
        New = 1,
        PaidinFull_deprecated = 7,
        Partial = 100002,
        PartiallyShipped = 2,
    }
    public enum IsInherited
    {
        DefaultTeamprivilegesonly = 0,
        DirectUser_BasicaccesslevelandTeamprivileges = 1,
    }
    public enum KbArticleStatusCode
    {
        Draft = 1,
        Published = 3,
        Unapproved = 2,
    }
    public enum knowledgearticle_ExpirationState
    {
        Archived = 5,
        Expired = 4,
        Published = 3,
    }
    public enum KnowledgeArticleExpiredReviewOptions
    {
        Archive = 2,
        NeedsUpdating = 0,
        Republish = 1,
    }
    public enum KnowledgeArticleIncidentKnowledgeUsage
    {
        Reference = 1,
        Solution = 2,
        Source = 3,
    }
    public enum KnowledgeArticleIncidentStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum KnowledgeArticleReview
    {
        Approved = 0,
        Rejected = 1,
    }
    public enum KnowledgeArticleStatusCode
    {
        Approved = 5,
        Archived = 12,
        Discarded = 13,
        Draft = 2,
        Expired = 10,
        Inreview = 4,
        Needsreview_Active = 3,
        Needsreview_Inactive = 8,
        Proposed = 1,
        Published = 7,
        Rejected_Inactive_11 = 11,
        Rejected_Inactive_14 = 14,
        Scheduled = 6,
        Updating = 9,
    }
    public enum KnowledgeArticleViewsLocation
    {
        Internal = 1,
        Web = 2,
    }
    public enum KnowledgeArticleViewsStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum KnowledgeSearchModelStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum LanguageLocaleStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum Lead_SalesStage
    {
        Qualify = 0,
    }
    public enum LeadAddress1_AddressTypeCode
    {
        DefaultValue = 1,
    }
    public enum LeadAddress1_ShippingMethodCode
    {
        DefaultValue = 1,
    }
    public enum LeadAddress2_AddressTypeCode
    {
        DefaultValue = 1,
    }
    public enum LeadAddress2_ShippingMethodCode
    {
        DefaultValue = 1,
    }
    public enum LeadAddressAddressTypeCode
    {
    }
    public enum LeadAddressShippingMethodCode
    {
    }
    public enum Leadims_coborrowertitle
    {
        Mr = 1,
        Mrs = 2,
        Ms = 3,
        Mx = 4,
    }
    public enum Leadims_gender
    {
        Female = 176390001,
        GenderDiverse = 176390002,
        Male = 176390000,
    }
    public enum Leadims_maritalstatus
    {
        Divorced = 176390003,
        Married = 176390001,
        RegisteredPartnership = 176390005,
        Seperated = 176390004,
        Single = 176390000,
        Widowed = 176390002,
    }
    public enum Leadims_ownershiptype
    {
        Partnership = 176390001,
        SoleProprietorship = 176390000,
    }
    public enum Leadims_preferredcommunicationchannel
    {
        Any = 176390004,
        Email = 176390002,
        Fax = 176390003,
        Phone = 176390001,
        Web = 176390000,
    }
    public enum Leadims_spousetitle
    {
        Mr = 176390000,
        Mrs = 176390001,
        Ms = 176390002,
        Mx = 176390003,
    }
    public enum Leadims_title
    {
        Mr = 176390000,
        Mrs = 176390001,
        Ms = 176390002,
        Mx = 176390003,
    }
    public enum LeadIndustryCode
    {
        Accounting = 1,
        AgricultureandNonpetrolNaturalResourceExtraction = 2,
        BroadcastingPrintingandPublishing = 3,
        Brokers = 4,
        BuildingSupplyRetail = 5,
        BusinessServices = 6,
        Consulting = 7,
        ConsumerServices = 8,
        DesignDirectionandCreativeManagement = 9,
        DistributorsDispatchersandProcessors = 10,
        DoctorsOfficesandClinics = 11,
        DurableManufacturing = 12,
        EatingandDrinkingPlaces = 13,
        EntertainmentRetail = 14,
        EquipmentRentalandLeasing = 15,
        Financial = 16,
        FoodandTobaccoProcessing = 17,
        InboundCapitalIntensiveProcessing = 18,
        InboundRepairandServices = 19,
        Insurance = 20,
        LegalServices = 21,
        NonDurableMerchandiseRetail = 22,
        OutboundConsumerService = 23,
        PetrochemicalExtractionandDistribution = 24,
        ServiceRetail = 25,
        SIGAffiliations = 26,
        SocialServices = 27,
        SpecialOutboundTradeContractors = 28,
        SpecialtyRealty = 29,
        Transportation = 30,
        UtilityCreationandDistribution = 31,
        VehicleRetail = 32,
        Wholesale = 33,
    }
    public enum LeadLeadQualityCode
    {
        Cold = 3,
        Hot = 1,
        Warm = 2,
    }
    public enum LeadLeadSourceCode
    {
        Advertisement = 1,
        EmployeeReferral = 2,
        ExternalReferral = 3,
        Landingpage = 192350100,
        LinkedInsponsoredform = 192350000,
        Other = 10,
        Partner = 4,
        PublicRelations = 5,
        Seminar = 6,
        TradeShow = 7,
        Web = 8,
        WordofMouth = 9,
    }
    public enum Leadmsdyncrm_leadsourcetype
    {
        Marketing = 192350000,
        Sales = 192350001,
        Teleprospect = 192350002,
    }
    public enum LeadPreferredContactMethodCode
    {
        Any = 1,
        Email = 2,
        Fax = 4,
        Mail = 5,
        Phone = 3,
    }
    public enum LeadPriorityCode
    {
        DefaultValue = 1,
    }
    public enum LeadSalesStageCode
    {
        DefaultValue = 1,
    }
    public enum LeadStatusCode
    {
        Canceled = 7,
        CannotContact = 5,
        CurrentClient = 176390002,
        DeadLeadLoan = 6,
        FundedBorrower = 176390001,
        Lead = 176390003,
        Lost = 4,
        Prequalified = 2,
        Prospect = 1,
        Qualified = 3,
        Suspect = 176390000,
    }
    public enum LeadToOpportunitySalesProcessStatusCode
    {
        Aborted = 3,
        Active = 1,
        Finished = 2,
    }
    public enum LetterPriorityCode
    {
        High = 2,
        Low = 0,
        Normal = 1,
    }
    public enum LetterStatusCode
    {
        Canceled = 5,
        Draft = 2,
        Open = 1,
        Received = 3,
        Sent = 4,
    }
    public enum ListCreatedFromCode
    {
        Account = 1,
        Contact = 2,
        Lead = 4,
    }
    public enum ListStatusCode
    {
        Active = 0,
        Inactive = 1,
    }
    public enum LookUpMappingLookUpSourceCode
    {
        Source = 0,
        System = 1,
    }
    public enum LookUpMappingProcessCode
    {
        Ignore = 2,
        Internal = 3,
        Process = 1,
    }
    public enum LookUpMappingStatusCode
    {
        Active = 0,
    }
    public enum MailboxACTDeliveryMethod
    {
        MicrosoftDynamics365forOutlook = 0,
        None = 2,
        ServerSideSynchronization = 1,
    }
    public enum MailboxACTStatus
    {
        Failure = 2,
        NotRun = 0,
        Success = 1,
    }
    public enum MailboxEmailRouterAccessApproval
    {
        Approved = 1,
        Empty = 0,
        PendingApproval = 2,
        Rejected = 3,
    }
    public enum MailboxExchangeContactsImportStatus
    {
        Imported = 1,
        ImportFailed = 2,
        NotImported = 0,
    }
    public enum MailboxIncomingEmailDeliveryMethod
    {
        ForwardMailbox = 3,
        MicrosoftDynamics365forOutlook = 1,
        None = 0,
        ServerSideSynchronizationorEmailRouter = 2,
    }
    public enum MailboxIncomingEmailStatus
    {
        Failure = 2,
        NotRun = 0,
        Success = 1,
    }
    public enum MailboxMailboxStatus
    {
        Failure = 2,
        NotRun = 0,
        Success = 1,
    }
    public enum MailboxOfficeAppsDeploymentStatus
    {
        Installed = 1,
        InstallFailed = 2,
        NotInstalled = 0,
        UninstallFailed = 3,
        UpgradeRequired = 4,
    }
    public enum MailboxOutgoingEmailDeliveryMethod
    {
        MicrosoftDynamics365forOutlook = 1,
        None = 0,
        ServerSideSynchronizationorEmailRouter = 2,
    }
    public enum MailboxOutgoingEmailStatus
    {
        Failure = 2,
        NotRun = 0,
        Success = 1,
    }
    public enum MailboxStatisticsOperationTypeId
    {
        ACT = 2,
        IncomingEmail = 0,
        OutgoingEmail = 1,
    }
    public enum MailboxStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum MailMergeTemplateDocumentFormat
    {
        _2003 = 1,
        _2007 = 2,
    }
    public enum MailMergeTemplateMailMergeType
    {
        EmailMessage = 2,
        Envelope = 3,
        Fax = 6,
        Labels = 4,
        Letter = 1,
        Quotes = 5,
    }
    public enum MailMergeTemplateStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum mbs_pluginprofilembs_mode
    {
        Asynchronous = 864340001,
        Synchronous = 864340000,
    }
    public enum mbs_pluginprofilembs_OperationType
    {
        Plugin = 864340000,
        Unknown = 0,
        WorkflowActivity = 864340001,
    }
    public enum mbs_pluginprofileStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum Metric_GoalType
    {
        Decimal = 1,
        Integer = 2,
        Money = 0,
    }
    public enum MetricStatusCode
    {
        Closed = 1,
        Open = 0,
    }
    public enum MobileOfflineEnabledEntities
    {
        Account = 1,
        AccountLeads = 16,
        Appointment = 4201,
        Attachment = 1001,
        Case = 112,
        Competitor = 123,
        CompetitorAddress = 1004,
        CompetitorProduct = 1006,
        Contact = 2,
        ContactLeads = 22,
        Email = 4202,
        Entitlement = 9700,
        EntitlementContact = 7272,
        EntitlementProduct = 6363,
        EntitlementTemplateProduct = 4545,
        EventMainBusinessProcessFlow = 10112,
        IncidentKnowledgeBaseRecord = 9931,
        Lead = 4,
        LeadCompetitors = 24,
        LeadProduct = 27,
        Leadtoopportunity = 10216,
        LeadToOpportunitySalesProcess = 954,
        Loan = 3,
        MortgageLoanProcess = 10063,
        Note = 5,
        OpportunityCompetitors = 25,
        OpportunityProduct = 1083,
        OpportunitySalesProcess = 953,
        PhoneToCaseProcess = 952,
        Product = 1024,
        Queue = 2020,
        QueueItem = 2029,
        SLAKPIInstance = 9752,
        Task = 4212,
        Team = 9,
        User = 8,
    }
    public enum MobileOfflineProfileItemRecordDistributionCriteria
    {
        Allrecords = 1,
        Customdatafilter = 3,
        Downloadrelateddataonly = 0,
        Otherdatafilter = 2,
    }
    public enum msdyn_actioncardregardingStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_actioncardrolesettingStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_AggregationType
    {
        Average = 986540000,
        Count = 986540001,
        None = 986540003,
        Text = 986540002,
    }
    public enum msdyn_AIConfigurationmsdyn_Type
    {
        RunConfiguration = 190690001,
        TrainingConfiguration = 190690000,
    }
    public enum msdyn_AIConfigurationStatusCode
    {
        CancelFailed = 12,
        Cancelling = 2,
        DeleteFailed = 13,
        Deleting = 5,
        Draft = 0,
        Published = 7,
        PublishFailed = 10,
        Publishing = 3,
        Scheduled = 8,
        Trained = 6,
        TrainFailed = 9,
        Training = 1,
        UnpublishFailed = 11,
        Unpublishing = 4,
        UnsuccessfulTraining = 14,
    }
    public enum msdyn_AIFpTrainingDocumentStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_AIModelStatusCode
    {
        Active = 1,
        Inactive = 0,
    }
    public enum msdyn_AIOdImageStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_AIOdLabelStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_AIOdTrainingBoundingBoxStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_AIOdTrainingImageStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_AITemplateStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_Alignment
    {
        Center = 986540001,
        Left = 986540000,
        Right = 986540002,
    }
    public enum msdyn_analysiscomponentmsdyn_AnalysisComponentType
    {
        ComponentHealth = 192350001,
        OrganizationHealth = 192350000,
    }
    public enum msdyn_analysiscomponentmsdyn_ComponentType
    {
        Configuration = 192350005,
        Entity = 192350001,
        Form = 192350003,
        Plugin = 192350004,
        Solution = 192350000,
        View = 192350002,
    }
    public enum msdyn_analysiscomponentStatusCode
    {
        Canceled = 2,
        Complete = 192350001,
        CompletedWithExceptions = 192350003,
        Exception = 192350002,
        Pending = 1,
        Running = 192350000,
    }
    public enum msdyn_analysisjobStatusCode
    {
        Canceled = 2,
        Complete = 192350001,
        CompletedWithExceptions = 192350003,
        Exception = 192350002,
        Pending = 1,
        Running = 192350000,
    }
    public enum msdyn_analysisresultdetailStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_analysisresultmsdyn_AnalysisComponentType
    {
        ComponentHealth = 192350001,
        OrganizationHealth = 192350000,
    }
    public enum msdyn_analysisresultmsdyn_Category
    {
        Accessibility = 192350008,
        Design = 192350004,
        Maintainability = 192350006,
        OnlineMigration = 192350005,
        Performance = 192350000,
        Security = 192350003,
        Supportability = 192350007,
        UpgradeReadiness = 192350001,
        Usage = 192350002,
    }
    public enum msdyn_analysisresultmsdyn_ComponentType
    {
        Configuration = 192350002,
        PlugIn = 192350001,
        WebResources = 192350000,
    }
    public enum msdyn_analysisresultmsdyn_Level
    {
        Error = 192350000,
        Warning = 192350001,
    }
    public enum msdyn_analysisresultmsdyn_ReturnStatus
    {
        ConfigError = 192350002,
        Fail = 192350001,
        Pass = 192350000,
        Resolved = 192350003,
        Warning = 192350004,
    }
    public enum msdyn_analysisresultmsdyn_Severity
    {
        Critical = 192350003,
        High = 192350002,
        Low = 192350000,
        Medium = 192350001,
    }
    public enum msdyn_analysisresultStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_answerStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_azuredeploymentStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_BookableResourceType
    {
        Account = 5,
        Contact = 2,
        Crew = 6,
        Equipment = 4,
        Facility = 7,
        Generic = 1,
        Pool = 8,
        User = 3,
    }
    public enum msdyn_CalculateSurveyScore
    {
        No = 986540001,
        Yes = 986540000,
    }
    public enum msdyn_callablecontextmsdyn_EntityOTC
    {
        Account = 1,
        AccountLeads = 16,
        ACIViewMapper = 8040,
        ActionCard = 9962,
        actioncardregarding = 10032,
        ActionCardRoleSetting = 10033,
        ActionCardType = 9983,
        ActionCardUserSettings = 9973,
        ActionCardUserState = 9968,
        Activity = 4200,
        ActivityParty = 135,
        Address = 1071,
        admin_settings_entity = 10025,
        AdvancedSimilarityRule = 9949,
        AIConfiguration = 402,
        AIFormProcessingDocument = 10008,
        AIModel = 401,
        AIObjectDetectionBoundingBox = 10011,
        AIObjectDetectionImage = 10009,
        AIObjectDetectionImageMapping = 10012,
        AIObjectDetectionLabel = 10010,
        AITemplate = 400,
        AnalysisComponent = 10065,
        AnalysisJob = 10066,
        AnalysisResult = 10067,
        AnalysisResultDetail = 10068,
        Announcement = 132,
        AnnualFiscalCalendar = 2000,
        Answer = 10073,
        AppConfigMaster = 9011,
        AppConfigSetup = 10055,
        AppConfiguration = 9012,
        AppConfigurationInstance = 9013,
        ApplicationFile = 4707,
        ApplicationRibbons = 1120,
        AppModuleComponent = 9007,
        AppModuleMetadata = 8700,
        AppModuleMetadataAsyncOperation = 8702,
        AppModuleMetadataDependency = 8701,
        AppModuleRoles = 9009,
        Appointment = 4201,
        Appointmentactivitymarketingtemplate = 10162,
        Article = 127,
        ArticleComment = 1082,
        ArticleTemplate = 1016,
        Attachment_1001 = 1001,
        Attachment_1002 = 1002,
        AttendeePass = 10110,
        Attribute = 9808,
        AttributeImageConfig = 431,
        AttributeMap = 4601,
        Auditing = 4567,
        AuthenticatedDomain = 10198,
        AuthenticationSettings = 10111,
        AuthorizationServer = 1094,
        AzureDeployment = 10074,
        AzureServiceConnection = 9936,
        BookableResource = 1150,
        BookableResourceBooking = 1145,
        BookableResourceBookingHeader = 1146,
        BookableResourceBookingtoExchangeIdMapping = 4421,
        BookableResourceCategory = 1147,
        BookableResourceCategoryAssn = 1149,
        BookableResourceCharacteristic = 1148,
        BookableResourceGroup = 1151,
        BookingStatus = 1152,
        Bucket = 10113,
        Building = 10114,
        BulkDeleteFailure = 4425,
        BulkDeleteOperation = 4424,
        BulkOperationLog = 4405,
        BusinessDataLocalizedLabel = 4232,
        BusinessProcessFlowInstance = 4725,
        BusinessUnit = 10,
        BusinessUnitMap = 6,
        Calendar = 4003,
        CalendarRule = 4004,
        CallbackRegistration = 301,
        Campaign = 4400,
        CampaignActivity = 4402,
        CampaignActivityItem = 4404,
        CampaignItem = 4403,
        CampaignResponse = 4401,
        CanvasApp = 300,
        Case = 112,
        CaseResolution = 4206,
        Category = 9959,
        CDNconfiguration = 10163,
        ChannelAccessProfile = 3005,
        ChannelAccessProfileRule = 9400,
        ChannelAccessProfileRuleItem = 9401,
        ChannelProperty = 1236,
        ChannelPropertyGroup = 1234,
        Characteristic = 1141,
        Checkin = 10115,
        ChildIncidentCount = 113,
        Clientupdate = 36,
        ColumnMapping = 4417,
        Comment = 8005,
        Commitment = 4215,
        Competitor = 123,
        CompetitorAddress = 1004,
        CompetitorProduct = 1006,
        CompetitorSalesLiterature = 26,
        ComponentLayer = 10006,
        ComponentLayerDataSource = 10007,
        Configuration = 10056,
        Connection = 3234,
        ConnectionRole = 3231,
        ConnectionRoleObjectTypeCode = 3233,
        Connector = 372,
        Contact = 2,
        ContactInvoices = 17,
        ContactLeads = 22,
        ContactOrders = 19,
        ContactQuotes = 18,
        ContactType = 10057,
        Contentblock = 10108,
        Contentsettings = 10165,
        Contract = 1010,
        ContractLine = 1011,
        ContractTemplate = 2011,
        Currency = 9105,
        Customchannelactivity = 10168,
        CustomControl = 9753,
        CustomControlDefaultConfig = 9755,
        CustomControlResource = 9754,
        Customerinsightsinformation = 10166,
        Customerjourney = 10167,
        Customerjourneyiteration = 10169,
        Customerjourneyruntimestate = 10170,
        Customerjourneytemplate = 10171,
        Customerjourneyworkflowlink = 10172,
        CustomerRelationship = 4502,
        CustomRegistrationField = 10116,
        DatabaseVersion = 10014,
        DataImport = 4410,
        DataMap = 4411,
        DataPerformanceDashboard = 4450,
        Defaultmarketingsettings = 10173,
        DelveActionHub = 9961,
        Dependency = 7105,
        DependencyFeature = 7108,
        DependencyNode = 7106,
        Designerfeatureprotection = 10109,
        Digitalassetsconfiguration = 10103,
        Discount = 1013,
        DiscountList = 1080,
        DisplayString = 4102,
        DisplayStringMap = 4101,
        DocumentLocation = 9508,
        DocumentSuggestions = 1189,
        DocumentTemplate = 9940,
        DuplicateDetectionRule = 4414,
        DuplicateRecord = 4415,
        DuplicateRuleCondition = 4416,
        Dynamiccontentmetadata = 10179,
        Email = 4202,
        EmailHash = 4023,
        EmailSearch = 4299,
        EmailServerProfile = 9605,
        EmailSignature = 9997,
        EmailTemplate = 2010,
        Entitlement = 9700,
        EntitlementChannel = 9701,
        EntitlementContact = 7272,
        EntitlementEntityAllocationTypeMapping = 9704,
        EntitlementProduct = 6363,
        EntitlementTemplate = 9702,
        EntitlementTemplateChannel = 9703,
        EntitlementTemplateProduct = 4545,
        Entity = 9800,
        EntityAnalyticsConfig = 430,
        EntityCounter = 10117,
        EntityImageConfig = 432,
        EntityMap = 4600,
        EntityRankingRule = 10034,
        EnvironmentVariableDefinition = 380,
        EnvironmentVariableValue = 381,
        Event = 10118,
        EventAdministration = 10119,
        EventCustomRegistrationField = 10120,
        EventMainBusinessProcessFlow = 10112,
        EventManagementActivity = 10121,
        EventManagementConfiguration = 10122,
        EventPurchase = 10123,
        EventPurchaseAttendee = 10124,
        EventPurchaseContact = 10125,
        EventPurchasePass = 10126,
        EventRegistration = 10127,
        EventTeamMember = 10128,
        EventVendor = 10129,
        ExchangeSyncIdMapping = 4120,
        ExpanderEvent = 4711,
        ExpiredProcess = 955,
        ExternalParty = 3008,
        ExternalPartyItem = 9987,
        FacilityEquipment = 4000,
        Fax = 4204,
        Feedback = 9958,
        FeedbackMapping = 10075,
        FeedbackRelatedSurvey = 10076,
        FieldPermission = 1201,
        FieldSecurityProfile = 1200,
        FieldSharing = 44,
        File = 10104,
        File_OBSOLETE = 10130,
        FileAttachment = 55,
        Filter = 10044,
        FilterTemplate = 30,
        FixedMonthlyFiscalCalendar = 2004,
        flowcardtype = 10035,
        Follow = 8003,
        Forecast = 10027,
        Forecastdefinition = 10026,
        Forecastrecurrence = 10028,
        Formpage = 10174,
        Formpagetemplate = 10175,
        FormsProsurvey = 10048,
        FormsProsurveyemailtemplate = 10045,
        FormsProsurveyinvite = 10049,
        FormsProsurveyquestion = 10046,
        FormsProsurveyquestionresponse = 10047,
        FormsProsurveyresponse = 10050,
        FormsProunsubscribedrecipient = 10051,
        Formwhitelistrule = 10187,
        GDPRconfiguration = 10235,
        GDPRconsentchangerecord = 10236,
        Geopin = 10176,
        GlobalSearchConfiguration = 54,
        Goal = 9600,
        GoalMetric = 9603,
        Grid = 10083,
        Gwennolfeatureconfiguration = 10240,
        HierarchyRule = 8840,
        HierarchySecurityConfiguration = 9919,
        HolidayWrapper = 9996,
        Hotel = 10131,
        HotelRoomAllocation = 10132,
        HotelRoomReservation = 10133,
        icebreakersconfig = 10039,
        Image = 10077,
        ImageDescriptor = 1007,
        ImageTokenCache = 10078,
        ImportData = 4413,
        ImportEntityMapping = 4428,
        ImportJob = 9107,
        ImportLog = 4423,
        ImportSourceFile = 4412,
        IncidentKnowledgeBaseRecord = 9931,
        IndexedArticle = 126,
        IntegrationStatus = 3000,
        InteractionforEmail = 9986,
        InternalAddress = 1003,
        InterProcessLock = 4011,
        InvalidDependency = 7107,
        Invoice = 1090,
        InvoiceProduct = 1091,
        ISVConfig = 4705,
        Keyword = 10105,
        KnowledgeArticle = 9953,
        KnowledgeArticleCategory = 9960,
        KnowledgeArticleIncident = 9954,
        KnowledgeArticleViews = 9955,
        KnowledgeBaseRecord = 9930,
        KnowledgeSearchModel = 9947,
        Language = 9957,
        LanguageProvisioningState = 9875,
        Layout = 10134,
        Lead = 4,
        LeadAddress = 1017,
        LeadCompetitors = 24,
        Leadentityfield = 10219,
        LeadProduct = 27,
        Leadscore = 10217,
        Leadscore_Deprecated = 10214,
        LeadScoringConfiguration = 10218,
        LeadScoringModel = 10215,
        LeadSource = 10058,
        Leadtoopportunity = 10216,
        LeadToOpportunitySalesProcess = 954,
        Letter = 4207,
        License = 2027,
        Like = 8006,
        LinkedAnswer = 10080,
        LinkedInaccount = 10220,
        LinkedInactivity = 10221,
        LinkedIncampaign = 10222,
        LinkedInfieldmapping = 10224,
        LinkedInformquestion = 10227,
        LinkedInformsubmissionanswer = 10226,
        LinkedInLeadGenform = 10225,
        LinkedInLeadGenformsubmission = 10228,
        LinkedInLeadGenintegrationconfiguration = 10223,
        LinkedInleadmatchingstrategy = 10229,
        LinkedInMatchedAudience = 10237,
        LinkedInuserprofile = 10230,
        LinkedQuestion = 10085,
        ListForm = 10177,
        ListValueMapping = 4418,
        Loan = 3,
        LoanProgram = 10059,
        LoanPurpose = 10060,
        LoanStatus = 10061,
        LoanType = 10062,
        LocalConfigStore = 9201,
        LookupMapping = 4419,
        Mailbox = 9606,
        MailboxAutoTrackingFolder = 9608,
        MailboxStatistics = 9607,
        MailboxTrackingCategory = 9609,
        MailMergeTemplate = 9106,
        Marketingactivity = 10193,
        Marketinganalyticsconfiguration = 10164,
        Marketingconfiguration = 10178,
        Marketingemail = 10180,
        Marketingemaildynamiccontentmetadata = 10181,
        Marketingemailtemplate = 10182,
        MarketingEmailTest = 10210,
        MarketingEmailTestAttribute = 10211,
        Marketingemailtestsend = 10183,
        Marketingfeatureconfiguration = 10234,
        Marketingfieldsubmission = 10212,
        Marketingform = 10184,
        Marketingformfield = 10185,
        Marketingformsubmission = 10213,
        Marketingformtemplate = 10186,
        Marketinglist = 4300,
        MarketingListMember = 4301,
        Marketingpage = 10188,
        Marketingpageconfiguration = 10189,
        Marketingpagetemplate = 10190,
        Marketingwebsite = 10202,
        Matchingstrategy = 10191,
        Matchingstrategyattributes = 10192,
        MetadataDifference = 4231,
        MicrosoftTeamsCollaborationentity = 10054,
        MicrosoftTeamsGraphresourceEntity = 10052,
        Migration = 10246,
        MobileOfflineProfile = 9866,
        MobileOfflineProfileItem = 9867,
        MobileOfflineProfileItemAssociation = 9868,
        ModeldrivenApp = 9006,
        MonthlyFiscalCalendar = 2003,
        MortgageLoanProcess = 10063,
        msdyn_msteamssetting = 10053,
        msdyn_relationshipinsightsunifiedconfig = 10029,
        MultiEntitySearch = 9910,
        MultiSelectOptionValue = 9912,
        NavigationSetting = 9900,
        NewProcess = 950,
        Note = 5,
        NotesanalysisConfig = 10038,
        Notification = 4110,
        ODatav4DataSource = 10002,
        OfficeDocument = 4490,
        OfficeGraphDocument = 9950,
        OfflineCommandDefinition = 9870,
        OpportunityClose = 4208,
        OpportunityCompetitors = 25,
        OpportunityProduct = 1083,
        OpportunityRelationship = 4503,
        OpportunitySalesProcess = 953,
        OptionSet = 9809,
        Order = 1088,
        OrderClose = 4209,
        OrderProduct = 1089,
        Organization = 1019,
        OrganizationInsightsMetric = 9699,
        OrganizationInsightsNotification = 9690,
        OrganizationStatistic = 4708,
        OrganizationUI = 1021,
        Owner = 7,
        OwnerMapping = 4420,
        Page = 10081,
        PartnerApplication = 1095,
        Pass = 10135,
        PersonalDocumentTemplate = 9941,
        Personalizedpage = 10244,
        Personalizedpagefield = 10245,
        PhoneCall = 4210,
        Phonecallactivitymarketingtemplate = 10194,
        PhoneToCaseProcess = 952,
        Playbook = 10022,
        Playbookactivity = 10019,
        Playbookactivityattribute = 10020,
        PlaybookCallableContext = 10018,
        Playbookcategory = 10021,
        Playbooktemplate = 10023,
        PluginAssembly = 4605,
        PluginProfile = 10072,
        PluginTraceLog = 4619,
        PluginType = 4602,
        PluginTypeStatistic = 4603,
        Portalsettings = 10195,
        Position = 50,
        Post = 8000,
        PostConfiguration = 10041,
        PostRegarding = 8002,
        PostRole = 8001,
        PostRuleConfiguration = 10042,
        PriceList = 1022,
        PriceListItem = 1026,
        PrincipalSyncAttributeMap = 1404,
        Privilege = 1023,
        PrivilegeObjectTypeCode = 31,
        Process = 4703,
        ProcessConfiguration = 9650,
        ProcessDependency = 4704,
        ProcessLog = 4706,
        ProcessSession = 4710,
        ProcessStage = 4724,
        ProcessTrigger = 4712,
        Product = 1024,
        ProductAssociation = 1025,
        ProductRelationship = 1028,
        ProductSalesLiterature = 21,
        ProfileAlbum = 10040,
        Property_10064 = 10064,
        Property_1048 = 1048,
        PropertyAssociation = 1235,
        PropertyInstance = 1333,
        PropertyOptionSetItem = 1049,
        Publisher = 7101,
        PublisherAddress = 7102,
        QuarterlyFiscalCalendar = 2002,
        Question = 10082,
        QuestionResponse = 10084,
        Queue = 2020,
        QueueItem = 2029,
        QueueItemCount = 2023,
        QueueMemberCount = 2024,
        QuickCampaign = 4406,
        Quotainfoentity = 10233,
        Quote = 1084,
        QuoteClose = 4211,
        QuoteProduct = 1085,
        RatingModel = 1144,
        RatingValue = 1142,
        RecordCreationandUpdateRule = 9300,
        RecordCreationandUpdateRuleItem = 9301,
        RecurrenceRule = 4250,
        RecurringAppointment = 4251,
        RedirectURL = 10196,
        RegistrationResponse = 10136,
        RelationshipRole = 4500,
        RelationshipRoleMap = 4501,
        ReplicationBacklog = 1140,
        Report = 9100,
        ReportLink = 9104,
        ReportRelatedCategory = 9102,
        ReportRelatedEntity = 9101,
        ReportVisibility = 9103,
        Resource = 4002,
        ResourceExpansion = 4010,
        ResourceGroup = 4007,
        ResourceSpecification = 4006,
        ResponseAction = 10086,
        ResponseCondition = 10088,
        ResponseError = 10089,
        ResponseOutcome = 10090,
        ResponseRouting = 10091,
        RibbonClientMetadata = 4579,
        RibbonCommand = 1116,
        RibbonContextGroup = 1115,
        RibbonDifference = 1130,
        RibbonMetadataToProcess = 9880,
        RibbonRule = 1117,
        RibbonTabToCommandMapping = 1113,
        RoleTemplate = 1037,
        RollupField = 9604,
        RollupJob = 9511,
        RollupProperties = 9510,
        RollupQuery = 9602,
        Room = 10137,
        RoomReservation = 10138,
        RoutingRuleSet = 8181,
        RuleItem = 8199,
        RuntimeDependency = 7200,
        SalesAttachment = 1070,
        salesinsightssettings = 10036,
        SalesLiterature = 1038,
        SalesProcessInstance = 32,
        SavedOrganizationInsightsConfiguration = 1309,
        SavedView = 4230,
        SchedulingGroup = 4005,
        SdkMessage = 4606,
        SdkMessageFilter = 4607,
        SdkMessagePair = 4613,
        SdkMessageProcessingStep = 4608,
        SdkMessageProcessingStepImage = 4615,
        SdkMessageProcessingStepSecureConfiguration = 4616,
        SdkMessageRequest = 4609,
        SdkMessageRequestField = 4614,
        SdkMessageResponse = 4610,
        SdkMessageResponseField = 4611,
        Section = 10092,
        SecurityRole = 1036,
        Segment = 10197,
        SemiannualFiscalCalendar = 2001,
        Service = 4001,
        ServiceActivity = 4214,
        ServiceContractContact = 20,
        ServiceEndpoint = 4618,
        Session = 10139,
        SessionRegistration = 10140,
        SessionTrack = 10141,
        SharePointData = 9509,
        SharepointDocument = 9507,
        SharePointSite = 9502,
        siconfig = 10030,
        SIKeyValueConfig = 10031,
        SimilarityRule = 9951,
        Site = 4009,
        SiteMap = 4709,
        SLA = 9750,
        SLAItem = 9751,
        SLAKPIInstance = 9752,
        SocialActivity = 4216,
        SocialInsightsConfiguration = 1300,
        Socialpost = 10241,
        SocialPostingConfiguration = 10242,
        SocialPostingConsent = 10243,
        SocialProfile = 99,
        Solution = 7100,
        SolutionComponent = 7103,
        SolutionComponentDataSource = 10001,
        SolutionComponentDefinition = 7104,
        SolutionComponentFileConfiguration = 10005,
        SolutionComponentSummary = 10000,
        SolutionHealthRule = 10069,
        SolutionHealthRuleArgument = 10070,
        SolutionHealthRuleSet = 10071,
        SolutionHistory = 10003,
        SolutionHistoryData = 9890,
        SolutionHistoryDataSource = 10004,
        SpamScoreActivity = 10238,
        SpamScoreRequest = 10239,
        Speaker = 10142,
        SpeakerEngagement = 10143,
        SponsorableArticle = 10144,
        Sponsorship = 10145,
        StatusMap = 1075,
        StringMap = 1043,
        Subject = 129,
        Subscription = 29,
        SubscriptionClients = 1072,
        SubscriptionManuallyTrackedObject = 37,
        SubscriptionStatisticOffline = 45,
        SubscriptionStatisticOutlook = 46,
        SubscriptionSyncEntryOffline = 47,
        SubscriptionSyncEntryOutlook = 48,
        SubscriptionSynchronizationInformation = 33,
        SuggestionCardTemplate = 1190,
        Survey = 10093,
        SurveyActivity = 10094,
        SurveyResponse = 10096,
        SyncAttributeMapping = 1401,
        SyncAttributeMappingProfile = 1400,
        SyncError = 9869,
        SystemApplicationMetadata = 7000,
        SystemChart = 1111,
        SystemForm = 1030,
        SystemJob = 4700,
        SystemUserBusinessUnitEntityMap = 42,
        SystemUserManagerMap = 51,
        SystemUserPrincipal = 14,
        Task = 4212,
        Taskactivitymarketingtemplate = 10199,
        Team = 9,
        TeamProfiles = 1203,
        TeamSyncAttributeMappingProfiles = 1403,
        Teamtemplate = 92,
        Territory = 2013,
        TextAnalyticsEntityMapping = 9945,
        TextAnalyticsTopic = 9948,
        Theme_10097 = 10097,
        Theme_2015 = 2015,
        TimeStampDateMapping = 9932,
        TimeZoneDefinition = 4810,
        TimeZoneLocalizedName = 4812,
        TimeZoneRule = 4811,
        TopicHistory = 9946,
        TopicModel = 9944,
        TopicModelConfiguration = 9942,
        TopicModelExecutionHistory = 9943,
        Trace = 8050,
        TraceAssociation = 8051,
        TraceRegarding = 8052,
        Trackinginformationfordeletedentities = 35,
        TransformationMapping = 4426,
        TransformationParameterMapping = 4427,
        TranslationProcess = 951,
        UICconfig = 10200,
        Unit = 1055,
        UnitGroup = 1056,
        UnresolvedAddress = 2012,
        UntrackedAppointment = 10037,
        UntrackedEmail = 4220,
        UpgradeRun = 10015,
        UpgradeStep = 10016,
        UpgradeVersion = 10017,
        User = 8,
        UserApplicationMetadata = 7001,
        UserChart = 1112,
        UserDashboard = 1031,
        UserEntityInstanceData = 2501,
        UserEntityUISettings = 2500,
        UserFiscalCalendar = 1086,
        Usergeoregion = 10201,
        UserMapping = 2016,
        UserSearchFacet = 52,
        UserSettings = 150,
        Venue = 10146,
        Video = 10106,
        View = 1039,
        VirtualEntityDataProvider = 78,
        VirtualEntityDataSource = 85,
        VoCImport = 10079,
        VoCResponseBlobStore = 10087,
        VoiceoftheCustomerConfiguration = 10098,
        VoiceoftheCustomerLog = 10095,
        WaitlistItem = 10147,
        WallView = 10043,
        WebApplication = 10148,
        WebinarConfiguration = 10149,
        WebinarConsent = 10150,
        WebinarProvider = 10151,
        WebinarType = 10152,
        WebResource = 9333,
        WebsiteEntityConfiguration = 10153,
        WebWizard = 4800,
        WebWizardAccessPrivilege = 4803,
        WizardPage = 4802,
        WorkflowWaitSubscription = 4702,
    }
    public enum msdyn_callablecontextStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_CollabGraphResourceStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_CreateSurveyResponseAlert
    {
        No = 986540000,
        Yes = 986540001,
    }
    public enum msdyn_databaseversionStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_DisplayLogic
    {
        Allaretrue_AND = 986540000,
        Anyaretrue_OR = 986540001,
    }
    public enum msdyn_entityrankingruleStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_FaceModelSet
    {
        Female1 = 986540000,
        Female2 = 986540003,
        Female3 = 986540005,
        Female4 = 986540007,
        Female5 = 986540009,
        Female7 = 986540011,
        Male1 = 986540001,
        Male2_986540002 = 986540002,
        Male2_986540004 = 986540004,
        Male3 = 986540006,
        Male4 = 986540008,
        Male6 = 986540010,
    }
    public enum msdyn_FacialExpressionSet
    {
        Angry = 986540011,
        Annoyed = 986540010,
        Bored = 986540006,
        Delighted = 986540003,
        Disappointed = 986540008,
        Noexpression = 986540000,
        Notengaged = 986540004,
        Notsuredontknow = 986540005,
        Sad = 986540009,
        Satisfied = 986540001,
        VerySatisfied = 986540002,
        Waiting = 986540007,
    }
    public enum msdyn_FacialExpressionType
    {
        Negative = 986540000,
        Neutral = 986540001,
        Positive = 986540002,
    }
    public enum msdyn_FeedbackAttributeType
    {
        Boolean = 0,
        DateTime = 2,
        Decimal = 3,
        File = 986540000,
        Integer = 5,
        Lookup = 6,
        Memo = 7,
        Picklist = 11,
        String = 14,
    }
    public enum msdyn_FeedbackGenerationStatus
    {
        GeneratedError = 986540002,
        GeneratedSuccess = 986540001,
        Generating = 986540000,
    }
    public enum msdyn_feedbackmappingmsdyn_AttributeStatus
    {
        AwaitingCreate = 986540000,
        AwaitingUpdate = 986540001,
        Complete = 986540002,
        Error = 986540003,
    }
    public enum msdyn_feedbackmappingmsdyn_MultipleResponseType
    {
        Bigbuttons = 986540001,
        Checkboxes = 986540000,
    }
    public enum msdyn_feedbackmappingmsdyn_ProcessedState
    {
        Processed = 192350001,
        Unprocessed = 192350000,
    }
    public enum msdyn_feedbackmappingmsdyn_SingleResponseType
    {
        Bigbuttons = 986540002,
        Dropdownlist = 986540001,
        Radiobuttons = 986540000,
        Scale = 986540003,
    }
    public enum msdyn_feedbackmappingStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_FeedbackRuntimeLookupType
    {
        Nonecustom = 986540003,
        SurveyInviteLookuponRegardingObject = 986540001,
        SurveyInviteOwningTeam = 986540005,
        SurveyInviteOwningUser = 986540002,
        SurveyInviteRegardingObject = 986540000,
        SurveyInviteService = 986540004,
    }
    public enum msdyn_feedbacksubsurveyStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_flowcardtypeStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_forecastdefinitionmsdyn_fiscalmonth
    {
        April = 3,
        August = 7,
        December = 11,
        February = 1,
        January = 0,
        July = 6,
        June = 5,
        March = 2,
        May = 4,
        November = 10,
        October = 9,
        September = 8,
    }
    public enum msdyn_forecastdefinitionmsdyn_fiscalquarter
    {
        Q1 = 0,
        Q2 = 1,
        Q3 = 2,
        Q4 = 3,
    }
    public enum msdyn_forecastdefinitionmsdyn_fiscalyear
    {
        FY2018 = 0,
        FY2019 = 1,
        FY2020 = 2,
        FY2021 = 3,
        FY2022 = 4,
        FY2023 = 5,
        FY2024 = 6,
        FY2025 = 7,
        FY2026 = 8,
        FY2027 = 9,
        FY2028 = 10,
        FY2029 = 11,
        FY2030 = 12,
        FY2031 = 13,
        FY2032 = 14,
        FY2033 = 15,
        FY2034 = 16,
        FY2035 = 17,
        FY2036 = 18,
        FY2037 = 19,
        FY2038 = 20,
        FY2039 = 21,
        FY2040 = 22,
    }
    public enum msdyn_forecastdefinitionmsdyn_forecastperiodtype
    {
        Custom = 2,
        Monthly = 0,
        Quarterly = 1,
    }
    public enum msdyn_forecastdefinitionmsdyn_quotasource
    {
        Goalbased = 192350000,
        Manual = 192350001,
    }
    public enum msdyn_forecastdefinitionStatusCode
    {
        Draft = 1,
        Failed = 4,
        Inprogress = 2,
        Success = 3,
    }
    public enum msdyn_forecastinstanceStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_forecastrecurrenceStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_GenerateResponseDataOptions
    {
        No = 986540000,
        Yes = 986540001,
    }
    public enum msdyn_icebreakersconfigStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_imagemsdyn_Extension
    {
        gif = 986540002,
        jpg = 986540001,
        png = 986540000,
    }
    public enum msdyn_imagemsdyn_ImageFormat
    {
        gif = 986540002,
        jpg = 986540000,
        png = 986540001,
    }
    public enum msdyn_imagemsdyn_Imagetype
    {
        Stockimage = 986540000,
        Uploadedcustomimage = 986540001,
    }
    public enum msdyn_imagemsdyn_Size
    {
        _128 = 128,
        _32 = 32,
        _48 = 48,
        _72 = 72,
    }
    public enum msdyn_imageStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_imagetokencacheStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_ImportActionType
    {
        Create = 986540001,
        Ignore = 986540002,
        Overwrite = 986540000,
    }
    public enum msdyn_importStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_linkedanswerStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_LinkQuestions
    {
        Donotlink = 986540001,
        Link = 986540000,
    }
    public enum msdyn_msteamssettingStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_NetPromoterScore
    {
        Detractor = 986540002,
        Passive = 986540001,
        Promoter = 986540000,
    }
    public enum msdyn_notesanalysisconfigStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_odatav4dsmsdyn_paginationtype
    {
        ClientsidePaging = 0,
        ServersidePaging = 1,
    }
    public enum msdyn_pagemsdyn_PageType
    {
        CompletionPage = 986540002,
        QuestionsPage = 986540001,
        WelcomePage = 986540000,
    }
    public enum msdyn_pageStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_PipeTypes
    {
        Customer = 4,
        Datetime = 6,
        Location = 5,
        Other1 = 7,
        Other2 = 8,
        Other3 = 9,
        Product = 2,
        Service = 3,
        User = 1,
    }
    public enum msdyn_playbookactivity_Priority
    {
        High = 2,
        Low = 0,
        Normal = 1,
    }
    public enum msdyn_playbookactivity_Time
    {
        _0100AM = 100,
        _0100PM = 1300,
        _0200AM = 200,
        _0200PM = 1400,
        _0300AM = 300,
        _0300PM = 1500,
        _0400AM = 400,
        _0400PM = 1600,
        _0500AM = 500,
        _0500PM = 1700,
        _0600AM = 600,
        _0600PM = 1800,
        _0700AM = 700,
        _0700PM = 1900,
        _0800AM = 800,
        _0800PM = 2000,
        _0900AM = 900,
        _0900PM = 2100,
        _1000AM = 1000,
        _1000PM = 2200,
        _1100AM = 1100,
        _1100PM = 2300,
        _1200AM = 0,
        _1200PM = 1200,
    }
    public enum msdyn_playbookactivityattributemsdyn_attributeType
    {
        boolean = 4,
        datetime = 3,
        integer = 2,
        optionset = 5,
        @string = 1,
    }
    public enum msdyn_playbookactivityattributeStatusCode
    {
        Draft = 1,
        Published = 2,
    }
    public enum msdyn_playbookactivitymsdyn_activityType
    {
        Account = 1,
        AccountLeads = 16,
        ACIViewMapper = 8040,
        ActionCard = 9962,
        actioncardregarding = 10032,
        ActionCardRoleSetting = 10033,
        ActionCardType = 9983,
        ActionCardUserSettings = 9973,
        ActionCardUserState = 9968,
        Activity = 4200,
        ActivityParty = 135,
        Address = 1071,
        admin_settings_entity = 10025,
        AdvancedSimilarityRule = 9949,
        AIConfiguration = 402,
        AIFormProcessingDocument = 10008,
        AIModel = 401,
        AIObjectDetectionBoundingBox = 10011,
        AIObjectDetectionImage = 10009,
        AIObjectDetectionImageMapping = 10012,
        AIObjectDetectionLabel = 10010,
        AITemplate = 400,
        AnalysisComponent = 10065,
        AnalysisJob = 10066,
        AnalysisResult = 10067,
        AnalysisResultDetail = 10068,
        Announcement = 132,
        AnnualFiscalCalendar = 2000,
        Answer = 10073,
        AppConfigMaster = 9011,
        AppConfigSetup = 10055,
        AppConfiguration = 9012,
        AppConfigurationInstance = 9013,
        ApplicationFile = 4707,
        ApplicationRibbons = 1120,
        AppModuleComponent = 9007,
        AppModuleMetadata = 8700,
        AppModuleMetadataAsyncOperation = 8702,
        AppModuleMetadataDependency = 8701,
        AppModuleRoles = 9009,
        Appointment = 4201,
        Appointmentactivitymarketingtemplate = 10162,
        Article = 127,
        ArticleComment = 1082,
        ArticleTemplate = 1016,
        Attachment_1001 = 1001,
        Attachment_1002 = 1002,
        AttendeePass = 10110,
        Attribute = 9808,
        AttributeImageConfig = 431,
        AttributeMap = 4601,
        Auditing = 4567,
        AuthenticatedDomain = 10198,
        AuthenticationSettings = 10111,
        AuthorizationServer = 1094,
        AzureDeployment = 10074,
        AzureServiceConnection = 9936,
        BookableResource = 1150,
        BookableResourceBooking = 1145,
        BookableResourceBookingHeader = 1146,
        BookableResourceBookingtoExchangeIdMapping = 4421,
        BookableResourceCategory = 1147,
        BookableResourceCategoryAssn = 1149,
        BookableResourceCharacteristic = 1148,
        BookableResourceGroup = 1151,
        BookingStatus = 1152,
        Bucket = 10113,
        Building = 10114,
        BulkDeleteFailure = 4425,
        BulkDeleteOperation = 4424,
        BulkOperationLog = 4405,
        BusinessDataLocalizedLabel = 4232,
        BusinessProcessFlowInstance = 4725,
        BusinessUnit = 10,
        BusinessUnitMap = 6,
        Calendar = 4003,
        CalendarRule = 4004,
        CallbackRegistration = 301,
        Campaign = 4400,
        CampaignActivity = 4402,
        CampaignActivityItem = 4404,
        CampaignItem = 4403,
        CampaignResponse = 4401,
        CanvasApp = 300,
        Case = 112,
        CaseResolution = 4206,
        Category = 9959,
        CDNconfiguration = 10163,
        ChannelAccessProfile = 3005,
        ChannelAccessProfileRule = 9400,
        ChannelAccessProfileRuleItem = 9401,
        ChannelProperty = 1236,
        ChannelPropertyGroup = 1234,
        Characteristic = 1141,
        Checkin = 10115,
        ChildIncidentCount = 113,
        Clientupdate = 36,
        ColumnMapping = 4417,
        Comment = 8005,
        Commitment = 4215,
        Competitor = 123,
        CompetitorAddress = 1004,
        CompetitorProduct = 1006,
        CompetitorSalesLiterature = 26,
        ComponentLayer = 10006,
        ComponentLayerDataSource = 10007,
        Configuration = 10056,
        Connection = 3234,
        ConnectionRole = 3231,
        ConnectionRoleObjectTypeCode = 3233,
        Connector = 372,
        Contact = 2,
        ContactInvoices = 17,
        ContactLeads = 22,
        ContactOrders = 19,
        ContactQuotes = 18,
        ContactType = 10057,
        Contentblock = 10108,
        Contentsettings = 10165,
        Contract = 1010,
        ContractLine = 1011,
        ContractTemplate = 2011,
        Currency = 9105,
        Customchannelactivity = 10168,
        CustomControl = 9753,
        CustomControlDefaultConfig = 9755,
        CustomControlResource = 9754,
        Customerinsightsinformation = 10166,
        Customerjourney = 10167,
        Customerjourneyiteration = 10169,
        Customerjourneyruntimestate = 10170,
        Customerjourneytemplate = 10171,
        Customerjourneyworkflowlink = 10172,
        CustomerRelationship = 4502,
        CustomRegistrationField = 10116,
        DatabaseVersion = 10014,
        DataImport = 4410,
        DataMap = 4411,
        DataPerformanceDashboard = 4450,
        Defaultmarketingsettings = 10173,
        DelveActionHub = 9961,
        Dependency = 7105,
        DependencyFeature = 7108,
        DependencyNode = 7106,
        Designerfeatureprotection = 10109,
        Digitalassetsconfiguration = 10103,
        Discount = 1013,
        DiscountList = 1080,
        DisplayString = 4102,
        DisplayStringMap = 4101,
        DocumentLocation = 9508,
        DocumentSuggestions = 1189,
        DocumentTemplate = 9940,
        DuplicateDetectionRule = 4414,
        DuplicateRecord = 4415,
        DuplicateRuleCondition = 4416,
        Dynamiccontentmetadata = 10179,
        Email = 4202,
        EmailHash = 4023,
        EmailSearch = 4299,
        EmailServerProfile = 9605,
        EmailSignature = 9997,
        EmailTemplate = 2010,
        Entitlement = 9700,
        EntitlementChannel = 9701,
        EntitlementContact = 7272,
        EntitlementEntityAllocationTypeMapping = 9704,
        EntitlementProduct = 6363,
        EntitlementTemplate = 9702,
        EntitlementTemplateChannel = 9703,
        EntitlementTemplateProduct = 4545,
        Entity = 9800,
        EntityAnalyticsConfig = 430,
        EntityCounter = 10117,
        EntityImageConfig = 432,
        EntityMap = 4600,
        EntityRankingRule = 10034,
        EnvironmentVariableDefinition = 380,
        EnvironmentVariableValue = 381,
        Event = 10118,
        EventAdministration = 10119,
        EventCustomRegistrationField = 10120,
        EventMainBusinessProcessFlow = 10112,
        EventManagementActivity = 10121,
        EventManagementConfiguration = 10122,
        EventPurchase = 10123,
        EventPurchaseAttendee = 10124,
        EventPurchaseContact = 10125,
        EventPurchasePass = 10126,
        EventRegistration = 10127,
        EventTeamMember = 10128,
        EventVendor = 10129,
        ExchangeSyncIdMapping = 4120,
        ExpanderEvent = 4711,
        ExpiredProcess = 955,
        ExternalParty = 3008,
        ExternalPartyItem = 9987,
        FacilityEquipment = 4000,
        Fax = 4204,
        Feedback = 9958,
        FeedbackMapping = 10075,
        FeedbackRelatedSurvey = 10076,
        FieldPermission = 1201,
        FieldSecurityProfile = 1200,
        FieldSharing = 44,
        File = 10104,
        File_OBSOLETE = 10130,
        FileAttachment = 55,
        Filter = 10044,
        FilterTemplate = 30,
        FixedMonthlyFiscalCalendar = 2004,
        flowcardtype = 10035,
        Follow = 8003,
        Forecast = 10027,
        Forecastdefinition = 10026,
        Forecastrecurrence = 10028,
        Formpage = 10174,
        Formpagetemplate = 10175,
        FormsProsurvey = 10048,
        FormsProsurveyemailtemplate = 10045,
        FormsProsurveyinvite = 10049,
        FormsProsurveyquestion = 10046,
        FormsProsurveyquestionresponse = 10047,
        FormsProsurveyresponse = 10050,
        FormsProunsubscribedrecipient = 10051,
        Formwhitelistrule = 10187,
        GDPRconfiguration = 10235,
        GDPRconsentchangerecord = 10236,
        Geopin = 10176,
        GlobalSearchConfiguration = 54,
        Goal = 9600,
        GoalMetric = 9603,
        Grid = 10083,
        Gwennolfeatureconfiguration = 10240,
        HierarchyRule = 8840,
        HierarchySecurityConfiguration = 9919,
        HolidayWrapper = 9996,
        Hotel = 10131,
        HotelRoomAllocation = 10132,
        HotelRoomReservation = 10133,
        icebreakersconfig = 10039,
        Image = 10077,
        ImageDescriptor = 1007,
        ImageTokenCache = 10078,
        ImportData = 4413,
        ImportEntityMapping = 4428,
        ImportJob = 9107,
        ImportLog = 4423,
        ImportSourceFile = 4412,
        IncidentKnowledgeBaseRecord = 9931,
        IndexedArticle = 126,
        IntegrationStatus = 3000,
        InteractionforEmail = 9986,
        InternalAddress = 1003,
        InterProcessLock = 4011,
        InvalidDependency = 7107,
        Invoice = 1090,
        InvoiceProduct = 1091,
        ISVConfig = 4705,
        Keyword = 10105,
        KnowledgeArticle = 9953,
        KnowledgeArticleCategory = 9960,
        KnowledgeArticleIncident = 9954,
        KnowledgeArticleViews = 9955,
        KnowledgeBaseRecord = 9930,
        KnowledgeSearchModel = 9947,
        Language = 9957,
        LanguageProvisioningState = 9875,
        Layout = 10134,
        Lead = 4,
        LeadAddress = 1017,
        LeadCompetitors = 24,
        Leadentityfield = 10219,
        LeadProduct = 27,
        Leadscore = 10217,
        Leadscore_Deprecated = 10214,
        LeadScoringConfiguration = 10218,
        LeadScoringModel = 10215,
        LeadSource = 10058,
        Leadtoopportunity = 10216,
        LeadToOpportunitySalesProcess = 954,
        Letter = 4207,
        License = 2027,
        Like = 8006,
        LinkedAnswer = 10080,
        LinkedInaccount = 10220,
        LinkedInactivity = 10221,
        LinkedIncampaign = 10222,
        LinkedInfieldmapping = 10224,
        LinkedInformquestion = 10227,
        LinkedInformsubmissionanswer = 10226,
        LinkedInLeadGenform = 10225,
        LinkedInLeadGenformsubmission = 10228,
        LinkedInLeadGenintegrationconfiguration = 10223,
        LinkedInleadmatchingstrategy = 10229,
        LinkedInMatchedAudience = 10237,
        LinkedInuserprofile = 10230,
        LinkedQuestion = 10085,
        ListForm = 10177,
        ListValueMapping = 4418,
        Loan = 3,
        LoanProgram = 10059,
        LoanPurpose = 10060,
        LoanStatus = 10061,
        LoanType = 10062,
        LocalConfigStore = 9201,
        LookupMapping = 4419,
        Mailbox = 9606,
        MailboxAutoTrackingFolder = 9608,
        MailboxStatistics = 9607,
        MailboxTrackingCategory = 9609,
        MailMergeTemplate = 9106,
        Marketingactivity = 10193,
        Marketinganalyticsconfiguration = 10164,
        Marketingconfiguration = 10178,
        Marketingemail = 10180,
        Marketingemaildynamiccontentmetadata = 10181,
        Marketingemailtemplate = 10182,
        MarketingEmailTest = 10210,
        MarketingEmailTestAttribute = 10211,
        Marketingemailtestsend = 10183,
        Marketingfeatureconfiguration = 10234,
        Marketingfieldsubmission = 10212,
        Marketingform = 10184,
        Marketingformfield = 10185,
        Marketingformsubmission = 10213,
        Marketingformtemplate = 10186,
        Marketinglist = 4300,
        MarketingListMember = 4301,
        Marketingpage = 10188,
        Marketingpageconfiguration = 10189,
        Marketingpagetemplate = 10190,
        Marketingwebsite = 10202,
        Matchingstrategy = 10191,
        Matchingstrategyattributes = 10192,
        MetadataDifference = 4231,
        MicrosoftTeamsCollaborationentity = 10054,
        MicrosoftTeamsGraphresourceEntity = 10052,
        Migration = 10246,
        MobileOfflineProfile = 9866,
        MobileOfflineProfileItem = 9867,
        MobileOfflineProfileItemAssociation = 9868,
        ModeldrivenApp = 9006,
        MonthlyFiscalCalendar = 2003,
        MortgageLoanProcess = 10063,
        msdyn_msteamssetting = 10053,
        msdyn_relationshipinsightsunifiedconfig = 10029,
        MultiEntitySearch = 9910,
        MultiSelectOptionValue = 9912,
        NavigationSetting = 9900,
        NewProcess = 950,
        Note = 5,
        NotesanalysisConfig = 10038,
        Notification = 4110,
        ODatav4DataSource = 10002,
        OfficeDocument = 4490,
        OfficeGraphDocument = 9950,
        OfflineCommandDefinition = 9870,
        OpportunityClose = 4208,
        OpportunityCompetitors = 25,
        OpportunityProduct = 1083,
        OpportunityRelationship = 4503,
        OpportunitySalesProcess = 953,
        OptionSet = 9809,
        Order = 1088,
        OrderClose = 4209,
        OrderProduct = 1089,
        Organization = 1019,
        OrganizationInsightsMetric = 9699,
        OrganizationInsightsNotification = 9690,
        OrganizationStatistic = 4708,
        OrganizationUI = 1021,
        Owner = 7,
        OwnerMapping = 4420,
        Page = 10081,
        PartnerApplication = 1095,
        Pass = 10135,
        PersonalDocumentTemplate = 9941,
        Personalizedpage = 10244,
        Personalizedpagefield = 10245,
        PhoneCall = 4210,
        Phonecallactivitymarketingtemplate = 10194,
        PhoneToCaseProcess = 952,
        Playbook = 10022,
        Playbookactivity = 10019,
        Playbookactivityattribute = 10020,
        PlaybookCallableContext = 10018,
        Playbookcategory = 10021,
        Playbooktemplate = 10023,
        PluginAssembly = 4605,
        PluginProfile = 10072,
        PluginTraceLog = 4619,
        PluginType = 4602,
        PluginTypeStatistic = 4603,
        Portalsettings = 10195,
        Position = 50,
        Post = 8000,
        PostConfiguration = 10041,
        PostRegarding = 8002,
        PostRole = 8001,
        PostRuleConfiguration = 10042,
        PriceList = 1022,
        PriceListItem = 1026,
        PrincipalSyncAttributeMap = 1404,
        Privilege = 1023,
        PrivilegeObjectTypeCode = 31,
        Process = 4703,
        ProcessConfiguration = 9650,
        ProcessDependency = 4704,
        ProcessLog = 4706,
        ProcessSession = 4710,
        ProcessStage = 4724,
        ProcessTrigger = 4712,
        Product = 1024,
        ProductAssociation = 1025,
        ProductRelationship = 1028,
        ProductSalesLiterature = 21,
        ProfileAlbum = 10040,
        Property_10064 = 10064,
        Property_1048 = 1048,
        PropertyAssociation = 1235,
        PropertyInstance = 1333,
        PropertyOptionSetItem = 1049,
        Publisher = 7101,
        PublisherAddress = 7102,
        QuarterlyFiscalCalendar = 2002,
        Question = 10082,
        QuestionResponse = 10084,
        Queue = 2020,
        QueueItem = 2029,
        QueueItemCount = 2023,
        QueueMemberCount = 2024,
        QuickCampaign = 4406,
        Quotainfoentity = 10233,
        Quote = 1084,
        QuoteClose = 4211,
        QuoteProduct = 1085,
        RatingModel = 1144,
        RatingValue = 1142,
        RecordCreationandUpdateRule = 9300,
        RecordCreationandUpdateRuleItem = 9301,
        RecurrenceRule = 4250,
        RecurringAppointment = 4251,
        RedirectURL = 10196,
        RegistrationResponse = 10136,
        RelationshipRole = 4500,
        RelationshipRoleMap = 4501,
        ReplicationBacklog = 1140,
        Report = 9100,
        ReportLink = 9104,
        ReportRelatedCategory = 9102,
        ReportRelatedEntity = 9101,
        ReportVisibility = 9103,
        Resource = 4002,
        ResourceExpansion = 4010,
        ResourceGroup = 4007,
        ResourceSpecification = 4006,
        ResponseAction = 10086,
        ResponseCondition = 10088,
        ResponseError = 10089,
        ResponseOutcome = 10090,
        ResponseRouting = 10091,
        RibbonClientMetadata = 4579,
        RibbonCommand = 1116,
        RibbonContextGroup = 1115,
        RibbonDifference = 1130,
        RibbonMetadataToProcess = 9880,
        RibbonRule = 1117,
        RibbonTabToCommandMapping = 1113,
        RoleTemplate = 1037,
        RollupField = 9604,
        RollupJob = 9511,
        RollupProperties = 9510,
        RollupQuery = 9602,
        Room = 10137,
        RoomReservation = 10138,
        RoutingRuleSet = 8181,
        RuleItem = 8199,
        RuntimeDependency = 7200,
        SalesAttachment = 1070,
        salesinsightssettings = 10036,
        SalesLiterature = 1038,
        SalesProcessInstance = 32,
        SavedOrganizationInsightsConfiguration = 1309,
        SavedView = 4230,
        SchedulingGroup = 4005,
        SdkMessage = 4606,
        SdkMessageFilter = 4607,
        SdkMessagePair = 4613,
        SdkMessageProcessingStep = 4608,
        SdkMessageProcessingStepImage = 4615,
        SdkMessageProcessingStepSecureConfiguration = 4616,
        SdkMessageRequest = 4609,
        SdkMessageRequestField = 4614,
        SdkMessageResponse = 4610,
        SdkMessageResponseField = 4611,
        Section = 10092,
        SecurityRole = 1036,
        Segment = 10197,
        SemiannualFiscalCalendar = 2001,
        Service = 4001,
        ServiceActivity = 4214,
        ServiceContractContact = 20,
        ServiceEndpoint = 4618,
        Session = 10139,
        SessionRegistration = 10140,
        SessionTrack = 10141,
        SharePointData = 9509,
        SharepointDocument = 9507,
        SharePointSite = 9502,
        siconfig = 10030,
        SIKeyValueConfig = 10031,
        SimilarityRule = 9951,
        Site = 4009,
        SiteMap = 4709,
        SLA = 9750,
        SLAItem = 9751,
        SLAKPIInstance = 9752,
        SocialActivity = 4216,
        SocialInsightsConfiguration = 1300,
        Socialpost = 10241,
        SocialPostingConfiguration = 10242,
        SocialPostingConsent = 10243,
        SocialProfile = 99,
        Solution = 7100,
        SolutionComponent = 7103,
        SolutionComponentDataSource = 10001,
        SolutionComponentDefinition = 7104,
        SolutionComponentFileConfiguration = 10005,
        SolutionComponentSummary = 10000,
        SolutionHealthRule = 10069,
        SolutionHealthRuleArgument = 10070,
        SolutionHealthRuleSet = 10071,
        SolutionHistory = 10003,
        SolutionHistoryData = 9890,
        SolutionHistoryDataSource = 10004,
        SpamScoreActivity = 10238,
        SpamScoreRequest = 10239,
        Speaker = 10142,
        SpeakerEngagement = 10143,
        SponsorableArticle = 10144,
        Sponsorship = 10145,
        StatusMap = 1075,
        StringMap = 1043,
        Subject = 129,
        Subscription = 29,
        SubscriptionClients = 1072,
        SubscriptionManuallyTrackedObject = 37,
        SubscriptionStatisticOffline = 45,
        SubscriptionStatisticOutlook = 46,
        SubscriptionSyncEntryOffline = 47,
        SubscriptionSyncEntryOutlook = 48,
        SubscriptionSynchronizationInformation = 33,
        SuggestionCardTemplate = 1190,
        Survey = 10093,
        SurveyActivity = 10094,
        SurveyResponse = 10096,
        SyncAttributeMapping = 1401,
        SyncAttributeMappingProfile = 1400,
        SyncError = 9869,
        SystemApplicationMetadata = 7000,
        SystemChart = 1111,
        SystemForm = 1030,
        SystemJob = 4700,
        SystemUserBusinessUnitEntityMap = 42,
        SystemUserManagerMap = 51,
        SystemUserPrincipal = 14,
        Task = 4212,
        Taskactivitymarketingtemplate = 10199,
        Team = 9,
        TeamProfiles = 1203,
        TeamSyncAttributeMappingProfiles = 1403,
        Teamtemplate = 92,
        Territory = 2013,
        TextAnalyticsEntityMapping = 9945,
        TextAnalyticsTopic = 9948,
        Theme_10097 = 10097,
        Theme_2015 = 2015,
        TimeStampDateMapping = 9932,
        TimeZoneDefinition = 4810,
        TimeZoneLocalizedName = 4812,
        TimeZoneRule = 4811,
        TopicHistory = 9946,
        TopicModel = 9944,
        TopicModelConfiguration = 9942,
        TopicModelExecutionHistory = 9943,
        Trace = 8050,
        TraceAssociation = 8051,
        TraceRegarding = 8052,
        Trackinginformationfordeletedentities = 35,
        TransformationMapping = 4426,
        TransformationParameterMapping = 4427,
        TranslationProcess = 951,
        UICconfig = 10200,
        Unit = 1055,
        UnitGroup = 1056,
        UnresolvedAddress = 2012,
        UntrackedAppointment = 10037,
        UntrackedEmail = 4220,
        UpgradeRun = 10015,
        UpgradeStep = 10016,
        UpgradeVersion = 10017,
        User = 8,
        UserApplicationMetadata = 7001,
        UserChart = 1112,
        UserDashboard = 1031,
        UserEntityInstanceData = 2501,
        UserEntityUISettings = 2500,
        UserFiscalCalendar = 1086,
        Usergeoregion = 10201,
        UserMapping = 2016,
        UserSearchFacet = 52,
        UserSettings = 150,
        Venue = 10146,
        Video = 10106,
        View = 1039,
        VirtualEntityDataProvider = 78,
        VirtualEntityDataSource = 85,
        VoCImport = 10079,
        VoCResponseBlobStore = 10087,
        VoiceoftheCustomerConfiguration = 10098,
        VoiceoftheCustomerLog = 10095,
        WaitlistItem = 10147,
        WallView = 10043,
        WebApplication = 10148,
        WebinarConfiguration = 10149,
        WebinarConsent = 10150,
        WebinarProvider = 10151,
        WebinarType = 10152,
        WebResource = 9333,
        WebsiteEntityConfiguration = 10153,
        WebWizard = 4800,
        WebWizardAccessPrivilege = 4803,
        WizardPage = 4802,
        WorkflowWaitSubscription = 4702,
    }
    public enum msdyn_playbookactivityStatusCode
    {
        Draft = 1,
        Published = 2,
    }
    public enum msdyn_playbookcategoryStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_playbookinstanceStatusCode
    {
        InProgress = 1,
        NotRequired = 5,
        NotSuccessful = 3,
        NotTracked = 6,
        PartiallySuccessful = 4,
        Successful = 2,
    }
    public enum msdyn_playbooktemplateStatusCode
    {
        Draft = 1,
        Published = 2,
    }
    public enum msdyn_PostAlbumStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_PostConfigStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_PostRuleConfigStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_QueryStringSet
    {
        a = 986540000,
        b = 986540001,
        c = 986540002,
    }
    public enum msdyn_questiongroupmsdyn_UseVerticalText
    {
        Norotationalignedbottom = 986540001,
        Norotationalignedtop = 986540002,
        Rotate90degrees = 986540000,
    }
    public enum msdyn_questiongroupStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_QuestionGroupType
    {
        Fixedsumquestions = 986540004,
        Gridofimagescales = 986540001,
        Gridofmultipleselectquestions = 986540003,
        Gridofsingleselectquestions = 986540002,
        Questionandanswer = 986540000,
    }
    public enum msdyn_questionmsdyn_CreateQuestionResponse
    {
        No = 986540001,
        Yes = 986540000,
    }
    public enum msdyn_questionmsdyn_FaceExpression
    {
        No = 986540001,
        Yes = 986540000,
    }
    public enum msdyn_questionmsdyn_FaceExpressionPicker
    {
        No = 986540001,
        Yes = 986540000,
    }
    public enum msdyn_questionmsdyn_FilterOperator
    {
        Equals = 986540000,
        Greaterthan = 986540002,
        Lessthan = 986540003,
        Notequals = 986540001,
    }
    public enum msdyn_questionmsdyn_Layouttype
    {
        Multiplequestionsperline_HTMLinline = 986540001,
        Singlequestionperline_HTMLBlock = 986540000,
    }
    public enum msdyn_questionmsdyn_multipleresponsetype
    {
        Bigbuttons = 986540001,
        Checkboxes = 986540000,
    }
    public enum msdyn_questionmsdyn_NumberType
    {
        Decimal = 986540001,
        Wholenumber = 986540000,
    }
    public enum msdyn_questionmsdyn_QuestionLayoutType
    {
        Questionatthetopanswerbelow = 986540000,
        Questionhalfspacequestionleftaligned = 986540002,
        Questionhalfspacequestionrightaligned = 986540003,
        Questiononethirdspacequestionleftaligned = 986540001,
        Questiononethirdspacequestionrightaligned = 986540008,
        Questiontwothirdsspacequestionleftaligned = 986540004,
        Questiontwothirdsspacequestionrightaligned = 986540005,
    }
    public enum msdyn_questionmsdyn_QuestionTextAlignment
    {
        Center = 986540001,
        Left = 986540000,
        Right = 986540002,
        Top = 986540003,
    }
    public enum msdyn_questionmsdyn_QuestionType
    {
        DateTime = 6,
        Descriptivetext = 10,
        Email = 11,
        File = 16,
        FixedSum = 15,
        Multiplelinesoftext = 2,
        Multipleresponseoptionset = 5,
        Numeric = 3,
        Ranking = 13,
        Rating = 9,
        Singlelineoftext = 1,
        Singleresponseoptionset = 4,
        WebSite = 12,
    }
    public enum msdyn_questionmsdyn_Required
    {
        No = 986540001,
        Yes = 986540000,
    }
    public enum msdyn_questionmsdyn_singleresponsetype
    {
        Bigbuttons = 986540002,
        Dropdownlist = 986540001,
        Radiobuttons = 986540000,
        Scale = 986540003,
    }
    public enum msdyn_questionresponseStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_questionStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_questiontypeStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_RatingImageSize
    {
        _24 = 986540000,
    }
    public enum msdyn_RatingImageType
    {
        Flags = 986540003,
        Smiliesneutraltohappy = 986540002,
        Smiliessadtohappy = 986540001,
        Star = 986540000,
    }
    public enum msdyn_relationshipinsightsunifiedconfigStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_responseactionmsdyn_Action
    {
        Chainsurvey = 986540004,
        Endsurvey = 986540003,
        Hide = 986540001,
        Show = 986540000,
        Skipto = 986540002,
        Togglevisibility = 986540005,
    }
    public enum msdyn_responseactionmsdyn_Scope
    {
        Client = 192350000,
        Server = 192350001,
    }
    public enum msdyn_responseactionStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_responseblobstoreStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_responseconditionmsdyn_Operator
    {
        Equals = 986540000,
        Greater = 986540001,
        LessThan = 986540002,
        Selected = 986540003,
    }
    public enum msdyn_responseconditionStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_responseerrorStatusCode
    {
        Active = 1,
        Closed = 2,
        Resolved = 3,
    }
    public enum msdyn_ResponseMappingSet
    {
        City = 986540010,
        CompanyName = 986540006,
        Country = 986540013,
        CustomDate1 = 986540032,
        CustomDate2 = 986540033,
        CustomerEffortScore = 986540066,
        CustomText1 = 986540022,
        CustomText2 = 986540023,
        CustomText3 = 986540024,
        CustomText4 = 986540025,
        CustomText5 = 986540026,
        Email = 986540002,
        FaceSelected = 986540068,
        FacialExpression = 986540067,
        FirstName = 986540000,
        Genericnumber1 = 986540048,
        Genericnumber10 = 986540037,
        Genericnumber11 = 986540038,
        Genericnumber12 = 986540039,
        Genericnumber13 = 986540040,
        Genericnumber14 = 986540041,
        Genericnumber15 = 986540042,
        Genericnumber16 = 986540043,
        Genericnumber17 = 986540044,
        Genericnumber18 = 986540046,
        Genericnumber19 = 986540045,
        Genericnumber2 = 986540028,
        Genericnumber20 = 986540047,
        Genericnumber3 = 986540029,
        Genericnumber4 = 986540030,
        Genericnumber5 = 986540031,
        Genericnumber6 = 986540027,
        Genericnumber7 = 986540035,
        Genericnumber8 = 986540036,
        Genericnumber9 = 986540034,
        Jobtitle = 986540005,
        LastName = 986540001,
        LinkedAnswer1 = 986540049,
        LinkedAnswer10 = 986540058,
        LinkedAnswer11 = 986540059,
        LinkedAnswer12 = 986540060,
        LinkedAnswer13 = 986540061,
        LinkedAnswer14 = 986540062,
        LinkedAnswer15 = 986540063,
        LinkedAnswer2 = 986540050,
        LinkedAnswer3 = 986540051,
        LinkedAnswer4 = 986540052,
        LinkedAnswer5 = 986540053,
        LinkedAnswer6 = 986540054,
        LinkedAnswer7 = 986540055,
        LinkedAnswer8 = 986540056,
        LinkedAnswer9 = 986540057,
        None_986540019 = 986540019,
        None_986540065 = 986540065,
        NPSComment = 986540064,
        Postalcode = 986540012,
        Salutation = 986540021,
        SatisfactionRating1 = 986540020,
        Slot1 = 986540014,
        Slot2 = 986540015,
        Slot3 = 986540016,
        Slot4 = 986540017,
        Slot5 = 986540018,
        State = 986540011,
        Street1 = 986540007,
        Street2 = 986540008,
        Street3 = 986540009,
        Telephone = 986540004,
        Website = 986540003,
    }
    public enum msdyn_responseoutcomeStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_ResponseRetrievalType
    {
        Pull = 986540001,
        Pull_WindowsService = 986540002,
        Push = 986540000,
    }
    public enum msdyn_responseroutingmsdyn_Group
    {
        GroupAND = 986540000,
        GroupOR = 986540001,
    }
    public enum msdyn_responseroutingStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_ResultType
    {
        Average = 986540001,
        None = 986540002,
        Percentage = 986540000,
    }
    public enum msdyn_salesinsightssettingsStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_ScoreDefinition
    {
        Auto_0100 = 986540000,
        Auto_1000 = 986540001,
        Ignore = 986540002,
        NPS = 986540003,
        UserSpecifiedRangeWeight = 986540004,
    }
    public enum msdyn_sectionStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_siconfigStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_sikeyvalueconfigStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_solutionhealthruleargumentStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_solutionhealthrulesetStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_solutionhealthruleStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_solutionhistorymsdyn_operation
    {
        Export = 2,
        Import = 0,
        Uninstall = 1,
    }
    public enum msdyn_solutionhistorymsdyn_status
    {
        Completed = 1,
        Started = 0,
    }
    public enum msdyn_solutionhistorymsdyn_suboperation
    {
        Delete = 4,
        New = 1,
        None = 0,
        Update = 3,
        Upgrade = 2,
    }
    public enum msdyn_SurveyActionType
    {
        Complaint = 986540000,
        Contactrequest = 986540005,
        Distress = 986540002,
        FacialExpressionDecrease = 986540010,
        FacialExpressionIncrease = 986540009,
        Followup = 986540006,
        HighScore = 986540004,
        Lowscore = 986540001,
        NPSDecrease = 986540008,
        NPSIncrease = 986540007,
        Unsubscribe = 986540003,
    }
    public enum msdyn_surveyinviteInstanceTypeCode
    {
        NotRecurring = 0,
        RecurringException = 3,
        RecurringFutureException = 4,
        RecurringInstance = 2,
        RecurringMaster = 1,
    }
    public enum msdyn_surveyinvitePriorityCode
    {
        High = 2,
        Low = 0,
        Normal = 1,
    }
    public enum msdyn_surveyinviteStatusCode
    {
        Canceled = 3,
        Completed = 2,
        NotResponded = 986540002,
        Open = 1,
        Scheduled = 4,
        Sent = 986540000,
    }
    public enum msdyn_surveylogmsdyn_LogLevel
    {
        _0Information = 986540000,
        _1Warning = 986540001,
        _2Error = 986540002,
    }
    public enum msdyn_surveylogStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_surveymsdyn_AddToSitemap
    {
        Marketing = 986540001,
        Sales = 986540000,
        Service = 986540002,
    }
    public enum msdyn_surveymsdyn_Assignresponses
    {
        Assigntoanotheruser = 986540001,
        Assigntome = 986540000,
        Assigntoteam = 986540002,
    }
    public enum msdyn_surveymsdyn_automaticallysendemailresponse
    {
        No = 986540001,
        Yes = 986540000,
    }
    public enum msdyn_surveymsdyn_ConvertResponsesToFeedback
    {
        Off = 986540000,
        Subsurveysonly = 986540001,
        Surveyandsubsurveys = 986540002,
    }
    public enum msdyn_surveymsdyn_GenerateFeedback
    {
        Automaticallywhensurveypublished = 986540000,
        Manually = 986540002,
        Off = 986540001,
    }
    public enum msdyn_surveymsdyn_RespondentDataActionType
    {
        Donotdownloadindividualresponsesaggregatethedata = 986540001,
        Downloadrespondentdataandexpandeachresponsetoarow = 986540002,
        Downloadrespondentdatatoasinglerow = 986540000,
    }
    public enum msdyn_surveymsdyn_Unsubscribe
    {
        No = 986540000,
        Yes = 986540001,
    }
    public enum msdyn_surveymsdyn_UseCaptcha
    {
        No = 986540000,
        Yes = 986540001,
    }
    public enum msdyn_surveyresponseStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_SurveyStage
    {
        During = 986540001,
        Post = 986540002,
        Prior = 986540000,
    }
    public enum msdyn_surveyStatusCode
    {
        Completed = 986540002,
        Draft = 1,
        Inactive = 2,
        Published = 986540001,
        Stopped = 986540000,
    }
    public enum msdyn_TeamsCollaborationStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_themeStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_untrackedappointmentStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_upgraderunStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_UpgradeStatus
    {
        Failure = 100000002,
        Started = 100000000,
        Success = 100000001,
    }
    public enum msdyn_upgradestepStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_upgradeversionStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_Visibility
    {
        Donotdisplay = 986540001,
        Visible = 986540000,
    }
    public enum msdyn_vocconfigurationStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_wallsavedqueryStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyn_wallsavedqueryusersettingsStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_appointmentactivitymarketingtemplatemsdyncrm_prioritycode
    {
        High = 2,
        Low = 0,
        Normal = 1,
    }
    public enum msdyncrm_appointmentactivitymarketingtemplateStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_cdnconfigurationStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_cdsaconnectorconfigurationStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_ClickmapKPIs
    {
        Clickthroughrate = 192350002,
        Totalclicks = 192350000,
        Uniqueclicks = 192350001,
    }
    public enum msdyncrm_Contentblockavailability
    {
        Email = 192350000,
        Forms = 192350001,
        Pages = 192350002,
    }
    public enum msdyncrm_contentblockStatusCode
    {
        Draft = 192350000,
        Expired = 192350004,
        Live = 192350001,
        Liveeditable = 192350003,
        Stopped = 192350002,
    }
    public enum msdyncrm_contentsettingsStatusCode
    {
        Draft = 192350000,
        Error = 192350005,
        Expired = 192350004,
        Goinglive = 192350006,
        Live = 192350001,
        Liveeditable = 192350003,
        Stopped = 192350002,
        Stopping = 192350007,
    }
    public enum msdyncrm_customerinsightsinfoStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_customerjourney_msdyncrm_entityTarget
    {
        Account = 1,
        Contact = 0,
    }
    public enum msdyncrm_customerjourney_Purpose
    {
        Announcement = 0,
        Emailmarketing = 6,
        Eventmarketing = 3,
        Largedeals = 4,
        Multipurpose = 1,
        Newsletters = 5,
        Onboarding = 2,
    }
    public enum msdyncrm_customerjourneycustomchannelactivityStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_customerjourneyiterationStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_customerjourneymsdyncrm_Type
    {
        Automated = 192350000,
        LinkedIn = 192350001,
    }
    public enum msdyncrm_customerjourneyruntimestatemsdyncrm_RuntimeState
    {
        Archived = 100230004,
        Inactive = 100230000,
        Inconsistentresume = 100230007,
        Inconsistentstart = 100230005,
        Inconsistentstop = 100230006,
        Running = 100230003,
        Scheduled = 100230001,
        Stopped = 100230002,
    }
    public enum msdyncrm_customerjourneyruntimestateStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_customerjourneyStatusCode
    {
        Draft = 192350000,
        Error = 192350005,
        Expired = 192350004,
        Goinglive = 192350006,
        Live = 192350001,
        Liveeditable = 192350003,
        Stopped = 192350002,
        Stopping = 192350007,
    }
    public enum msdyncrm_customerjourneytemplateStatusCode
    {
        Draft = 192350000,
        Inactive = 2,
        Live = 192350001,
    }
    public enum msdyncrm_customerjourneyworkflowlinkStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_defaultmarketingsettingmsdyncrm_thankyoupagesource
    {
        No = 100000001,
        Yes = 100000000,
    }
    public enum msdyncrm_defaultmarketingsettingStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_designerfeatureavailabilityStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_Designerfeatures
    {
        HTML = 192350000,
        Litmus = 192350001,
    }
    public enum msdyncrm_digitalassetsconfigurationStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_email_Contenttype
    {
        Confirmationrequest = 1,
        Default = 0,
    }
    public enum msdyncrm_email_template_market_type
    {
        B2B = 0,
        B2BB2C = 2,
        B2C = 1,
    }
    public enum msdyncrm_email_template_optimized_for
    {
        Desktop = 2,
        Mobile = 1,
        Widereach = 0,
    }
    public enum msdyncrm_email_template_Purpose
    {
        Abandonedshoppingcart = 15,
        Anniversary = 21,
        Announcement = 2,
        Birthday = 3,
        Blogpromotion = 4,
        Contentpromotion = 5,
        Doubleoptinemailbasedconfirmation = 23,
        Eventcountdown = 6,
        Eventorwebinarinvitation = 7,
        Feedbackrequest = 20,
        Followup = 8,
        Holidaygreetings = 9,
        Hospitality = 11,
        Information = 10,
        Leadnurturing = 22,
        Newsletter = 12,
        Postpurchase = 13,
        Promotional_upsellcrosssell = 14,
        Structural = 0,
        Thankyou = 16,
        Upcomingevent = 17,
        Welcome = 18,
        Winbackreengage = 19,
    }
    public enum msdyncrm_email_template_visual_style
    {
        Colorful = 2,
        Dark = 3,
        Light = 1,
        Other = 0,
    }
    public enum msdyncrm_entityupdatebehavioronsubmit
    {
        Contactsandleads = 0,
        Onlycontacts = 1,
        Onlyleads = 2,
    }
    public enum msdyncrm_feature_state_option_set
    {
        Disabled = 192350020,
        Enabled = 192350010,
    }
    public enum msdyncrm_featureconfigurationStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_fileStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_formpageStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_formpagetemplateStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_geokpis
    {
        Formsubmissions = 192350005,
        Redirectlinkclicks = 192350002,
        Totalclicks = 192350000,
        Totalopens = 192350001,
        Websiteclicks = 192350003,
        Websitevisits = 192350004,
    }
    public enum msdyncrm_geopinStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_Georegion
    {
        AR = 192350024,
        CN = 192350156,
        IN = 192350356,
        Other = 192350000,
        PK = 192350586,
    }
    public enum msdyncrm_gwennolfeatureconfigurationmsdyncrm_featurestate
    {
        Disabled = 192350020,
        Enabled = 192350010,
    }
    public enum msdyncrm_gwennolfeatureconfigurationStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_gwennolspamscoreactivityStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_gwennolspamscorerequestStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_keywordStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_leadentityfieldStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_leadscore_v2msdyncrm_Scorestatus
    {
        Error = 533240003,
        Inprogress = 533240001,
        Obsolete = 533240000,
        Uptodate = 533240002,
    }
    public enum msdyncrm_leadscore_v2StatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_leadscoremodelStatusCode
    {
        Draft = 192350000,
        Error = 192350010,
        Expired = 192350004,
        Goinglive = 192350011,
        Live = 192350001,
        Stopping = 192350012,
    }
    public enum msdyncrm_leadscoreStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_leadscoringconfigurationStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_leadtoopportunityStatusCode
    {
        Active = 1,
        Canceled = 3,
        Finished = 2,
    }
    public enum msdyncrm_linkedinaccountmsdyncrm_campaignsyncstatus
    {
        Active = 192350001,
        Forbidden = 192350003,
        Noactivecampaignsavailable = 192350002,
        Pendingsynchronization = 192350000,
    }
    public enum msdyncrm_linkedinaccountmsdyncrm_syncstatus
    {
        Active = 192350001,
        Forbidden = 192350004,
        Noformsavailable = 192350003,
        Notsyncing = 192350002,
        Pendingsynchronization = 192350000,
    }
    public enum msdyncrm_linkedinaccountStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_linkedinactivityStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_linkedinaudiencemsdyncrm_activestatus
    {
        Done = 192350001,
        Forbidden = 192350003,
        Processing = 192350000,
        Timeout = 192350004,
        Unauthorized = 192350002,
        Unknownstate = 192350005,
    }
    public enum msdyncrm_linkedinaudiencemsdyncrm_source
    {
        Accountlist = 192350002,
        Contactlist = 192350001,
        Segmentcontacts = 192350000,
    }
    public enum msdyncrm_linkedinaudienceStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_linkedincampaignmsdyncrm_CampaignType
    {
        LinkedIndynamicad = 192350003,
        LinkedInsponsoredcontent = 192350001,
        LinkedInsponsoredInMail = 192350002,
        Textadvertisement = 192350000,
    }
    public enum msdyncrm_linkedincampaignmsdyncrm_LinkedInStatus
    {
        Active = 192350000,
        Archived = 192350002,
        Canceled = 192350004,
        Completed = 192350003,
        Draft = 192350005,
        Paused = 192350001,
    }
    public enum msdyncrm_linkedincampaignStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_linkedinconfigurationmsdyncrm_authenticationtype
    {
        Basicauthentication = 192350001,
        Bearerauthentication = 192350000,
    }
    public enum msdyncrm_linkedinconfigurationmsdyncrm_OnMatchingFail
    {
        Createnewlead = 192350001,
        Ignore = 192350000,
    }
    public enum msdyncrm_linkedinconfigurationStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_linkedinfieldmappingStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_linkedinformanswerStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_linkedinformmsdyncrm_syncstatus
    {
        Active = 192350001,
        Forbidden = 192350003,
        Noformsubmissions = 192350002,
        Pendingsynchronization = 192350000,
    }
    public enum msdyncrm_linkedinformquestionStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_linkedinformStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_linkedinformsubmissionmsdyncrm_matchingstatus
    {
        Leadcreationfailed = 192350004,
        Leadmatchedbutnotupdated = 192350006,
        Leadupdatefailed = 192350005,
        Matchfailed = 192350003,
        Newleadcreated = 192350001,
        Pendingleadmatching = 192350000,
        Updatedexistinglead = 192350002,
    }
    public enum msdyncrm_linkedinformsubmissionmsdyncrm_processingstate
    {
        Processingfailed = 192350002,
        Processingsucceeded = 192350001,
        Unprocessed = 192350000,
    }
    public enum msdyncrm_linkedinformsubmissionStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_linkedinleadmatchingstrategyStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_linkedinuserprofilemsdyncrm_authorized
    {
        Authorized = 192350001,
        Expired = 192350002,
        Notauthorized = 192350000,
        Reauthorizing = 192350003,
    }
    public enum msdyncrm_linkedinuserprofilemsdyncrm_SyncStatus
    {
        Active = 192350002,
        Noaccountsavailable = 192350003,
        Notsyncing = 192350000,
        Syncingaccounts = 192350001,
    }
    public enum msdyncrm_linkedinuserprofileStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_listformStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_marketingconfigurationStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_marketingdynamiccontentmetadataStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_marketingemaildynamiccontentmetadataStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_marketingemailStatusCode
    {
        Draft = 192350000,
        Error = 192350005,
        Expired = 192350004,
        Goinglive = 192350006,
        Live = 192350001,
        Liveeditable = 192350003,
        Stopped = 192350002,
        Stopping = 192350007,
    }
    public enum msdyncrm_marketingemailtemplateStatusCode
    {
        Draft = 192350000,
        Inactive = 2,
        Live = 192350001,
    }
    public enum msdyncrm_marketingemailtest_status
    {
        Finished = 2,
        Published = 0,
        Started = 1,
    }
    public enum msdyncrm_marketingemailtestattributeStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_marketingemailtestsendStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_marketingemailtestStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_marketingfieldsubmissionStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_marketingform_Visualstyle
    {
        _1column = 0,
        _2column = 1,
        Mixed = 2,
    }
    public enum msdyncrm_marketingformfield_renderingcontrols
    {
        Checkbox = 7,
        Datepicker = 10,
        Datetimepicker = 11,
        Dropdown = 8,
        Emailinput = 2,
        Numberinput = 5,
        Phoneinput = 4,
        Radiobuttons = 9,
        Textarea = 6,
        Textbox = 1,
        URLinput = 3,
    }
    public enum msdyncrm_marketingformfield_supporteddataformats
    {
        Date = 6,
        Dateandtime = 7,
        Email = 3,
        Phone = 5,
        Text = 1,
        Textarea = 2,
        URL = 4,
    }
    public enum msdyncrm_marketingformfield_supporteddatatypes
    {
        Currency = 9,
        Dateandtime = 8,
        Decimalnumber = 7,
        Floatingpointnumber = 6,
        Multiplelinesoftext = 2,
        Optionset = 3,
        Singlelineoftext = 1,
        Twooptions = 4,
        Wholenumber = 5,
    }
    public enum msdyncrm_marketingformfieldStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_marketingformStatusCode
    {
        Draft = 192350000,
        Error = 192350005,
        Expired = 192350004,
        Live = 1,
        Liveeditable = 192350003,
        Stopped = 2,
    }
    public enum msdyncrm_marketingformsubmissionStatusCode
    {
        Failure = 192350001,
        Inactive = 2,
        Pending = 192350000,
        Success = 192350002,
    }
    public enum msdyncrm_marketingformtemplateStatusCode
    {
        Draft = 192350000,
        Inactive = 2,
        Live = 192350001,
    }
    public enum msdyncrm_marketingformwhitelistruleStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_marketingpage_Contenttype
    {
        Companybackground = 3,
        Confirmationrequest = 6,
        Eventinformation = 4,
        Offersanddiscounts = 5,
        Productinformation = 2,
        Productlaunch = 1,
        Structural = 0,
    }
    public enum msdyncrm_marketingpage_Markettype
    {
        All = 2,
        Consumer = 1,
        Enterprise = 0,
    }
    public enum msdyncrm_marketingpage_Optimizedfor
    {
        Desktop = 0,
        Mobile = 2,
        Tablet = 1,
    }
    public enum msdyncrm_marketingpage_Purpose
    {
        Collateraldownload = 3,
        Contactcapture = 0,
        DoubleOptInEmailbasedconfirmation = 7,
        Eventfeedback = 5,
        Eventregistration = 4,
        Leadgeneration = 2,
        Newslettersubscription = 1,
        Structural = 6,
    }
    public enum msdyncrm_marketingpage_Type
    {
        Forwardtoafriend = 2,
        Landingpage = 0,
        Subscriptioncenter = 1,
    }
    public enum msdyncrm_marketingpage_Visualstyle
    {
        Colorful = 2,
        Dark = 1,
        Light = 0,
        Other = 3,
    }
    public enum msdyncrm_marketingpageconfiguration_msdyncrm_Portalinstallationstatus
    {
        Notstarted = 1,
        Started = 2,
    }
    public enum msdyncrm_marketingpageconfiguration_msdyncrm_Portalintegrationtype
    {
        Forcelocalhosting = 1,
        Forceportalhosting = 2,
    }
    public enum msdyncrm_marketingpageconfigurationStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_marketingpageStatusCode
    {
        Draft = 192350000,
        Error = 192350005,
        Expired = 192350004,
        Goinglive = 192350006,
        Live = 192350001,
        Liveeditable = 192350003,
        Stopped = 192350002,
        Stopping = 192350007,
    }
    public enum msdyncrm_marketingpagetemplateStatusCode
    {
        Draft = 192350000,
        Inactive = 2,
        Live = 192350001,
    }
    public enum msdyncrm_matchingstrategy_Target
    {
        Contact = 192350000,
        Lead = 192350001,
    }
    public enum msdyncrm_matchingstrategyattributeStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_matchingstrategymsdyncrm_matchingstrategyfieldsstatus
    {
        Notvalidated = 100000000,
        Validated = 100000001,
    }
    public enum msdyncrm_matchingstrategyStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_messagedesignation
    {
        Commercial = 0,
        Transactional = 1,
    }
    public enum msdyncrm_migrationStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_mktactivityStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_personalizedpagefieldStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_personalizedpageStatusCode
    {
        Draft = 192350000,
        Error = 192350005,
        Expired = 192350004,
        Live = 192350001,
        Stopped = 192350002,
    }
    public enum msdyncrm_phonecallactivitymarketingtemplatemsdyncrm_prioritycode
    {
        High = 2,
        Low = 0,
        Normal = 1,
    }
    public enum msdyncrm_phonecallactivitymarketingtemplateStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_portalsettingsStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_quotainfoentityStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_redirecturlStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_Scheduletype
    {
        Delay_indays = 1,
        Fixeddate = 0,
    }
    public enum msdyncrm_segmentmsdyncrm_querytype
    {
        Interactionbased = 192350000,
        Profilebased = 192350001,
    }
    public enum msdyncrm_segmentmsdyncrm_segmentactivationstatus
    {
        Active = 192350000,
        Disabled = 192350001,
    }
    public enum msdyncrm_segmentmsdyncrm_segmenttype
    {
        Compoundsegment = 192350002,
        Dynamicsegment = 192350000,
        Staticsegment = 192350001,
    }
    public enum msdyncrm_segmentStatusCode
    {
        Draft = 192350000,
        Error = 192350005,
        Expired = 192350004,
        Goinglive = 192350006,
        Live = 192350001,
        Liveeditable = 192350003,
        Stopped = 192350002,
        Stopping = 192350007,
    }
    public enum msdyncrm_setupdomain_status
    {
        Cancelled = 2,
        Confirmed = 1,
        ConfirmingDNSregistration = 4,
        Internalerror = 32,
        Internalerror_recordnotfound = 31,
        KeysnotfoundonDNS = 30,
        Notrequested = 3,
        Waitingtoconfirm = 0,
    }
    public enum msdyncrm_setupdomainStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_socialchannellist
    {
        Facebook = 270100001,
        LinkedIn = 270100002,
        Twitter = 270100000,
    }
    public enum msdyncrm_socialpostingconfigurationmsdyncrm_SocialChannel
    {
        Facebook = 270100001,
        LinkedIn = 270100002,
        Twitter = 270100000,
    }
    public enum msdyncrm_socialpostingconfigurationStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_socialpostingconsentStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_socialpostmsdyncrm_linkedInvisibility
    {
        Connections = 270100000,
        Public = 270100001,
    }
    public enum msdyncrm_socialpostmsdyncrm_postingfrom
    {
        User0 = 270100000,
        User1 = 270100001,
        User10 = 270100010,
        User2 = 270100002,
        User3 = 270100003,
        User4 = 270100004,
        User5 = 270100005,
        User6 = 270100006,
        User7 = 270100007,
        User8 = 270100008,
        User9 = 270100009,
    }
    public enum msdyncrm_socialpostmsdyncrm_PostingPeriod
    {
        Postnow = 270100000,
        Schedule = 270100002,
        Schedulelater = 270100001,
    }
    public enum msdyncrm_socialpostmsdyncrm_poststate
    {
        Draft = 270100000,
        Failed = 270100004,
        Live = 270100002,
        New = 270100003,
        ReadyToGoLive = 270100005,
        Scheduled = 270100001,
    }
    public enum msdyncrm_socialpostmsdyncrm_socialchannel
    {
        Facebook = 270100001,
        LinkedIn = 270100002,
        Twitter = 270100000,
    }
    public enum msdyncrm_socialpostStatusCode
    {
        Draft = 2,
        New = 1,
        Published = 3,
    }
    public enum msdyncrm_starttimehour
    {
        _00 = 0,
        _01 = 1,
        _02 = 2,
        _03 = 3,
        _04 = 4,
        _05 = 5,
        _06 = 6,
        _07 = 7,
        _08 = 8,
        _09 = 9,
        _10 = 10,
        _11 = 11,
        _12 = 12,
        _13 = 13,
        _14 = 14,
        _15 = 15,
        _16 = 16,
        _17 = 17,
        _18 = 18,
        _19 = 19,
        _20 = 20,
        _21 = 21,
        _22 = 22,
        _23 = 23,
    }
    public enum msdyncrm_starttimeminute
    {
        _00 = 0,
        _15 = 15,
        _30 = 30,
        _45 = 45,
    }
    public enum msdyncrm_syncstatus
    {
        Error = 192350002,
        Notinitialized = 192350000,
        Synced = 192350001,
    }
    public enum msdyncrm_taskactivitymarketingtemplatemsdyncrm_prioritycode
    {
        High = 2,
        Low = 0,
        Normal = 1,
    }
    public enum msdyncrm_taskactivitymarketingtemplateStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_Templatesupportedlanguages
    {
        Arabic_SaudiArabia = 1025,
        Basque_Basque = 1069,
        Bulgarian_Bulgaria = 1026,
        Catalan_Catalan = 1027,
        Chinese_HongKongSAR = 3076,
        Chinese_Taiwan = 1028,
        Croatian_Croatia = 1050,
        Czech_CzechRepublic = 1029,
        Danish = 1030,
        Dutch = 1043,
        English = 1033,
        English_Australia = 3081,
        English_Canada = 4105,
        English_GreatBritain = 2057,
        Estonian_Estonia = 1061,
        Finnish_Finland = 1035,
        French = 1036,
        French_Canada = 3084,
        Galician_Galician = 1110,
        German = 1031,
        Greek_Greece = 1032,
        Hebrew_Israel = 1037,
        Hungarian_Hungary = 1038,
        Indonesian_Indonesia = 1057,
        Italian = 1040,
        Japanese = 1041,
        Korean_Korea = 1042,
        Latvian_Latvia = 1062,
        Lithuanian_Lithuania = 1063,
        NorwegianBokml_Norway = 1044,
        Polish_Poland = 1045,
        Portuguese_Brazil = 1046,
        Portuguese_Portugal = 2070,
        Romanian_Romania = 1048,
        Russian_Russia = 1049,
        Serbian_CyrillicSerbiaandMontenegro = 3098,
        Serbian_LatinSerbiaandMontenegro = 2074,
        Slovak_Slovakia = 1051,
        Slovenian_Slovenia = 1060,
        Spanish = 3082,
        Swedish_Sweden = 1053,
        Thai_Thailand = 1054,
        Turkish_Turkey = 1055,
        Ukrainian_Ukraine = 1058,
        Vietnamese_Vietnam = 1066,
    }
    public enum msdyncrm_uicconfigStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_usergeoregionStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_videoStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msdyncrm_website_purpose
    {
        General = 192350000,
        Marketingpage = 192350001,
    }
    public enum msdyncrm_websiteStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_Area
    {
        Administration = 100000000,
        Customerservice = 100000001,
        Executivemanagement = 100000002,
        Finance = 100000004,
        HR = 100000005,
        IT = 100000006,
        Legal = 100000007,
        Logistics = 100000003,
        Marketing = 100000008,
        Sales = 100000009,
    }
    public enum msevtmgt_AttendeePassStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_Audiencetype
    {
        Advanced = 100000003,
        General = 100000000,
        Intermediate = 100000002,
        Introductory = 100000001,
    }
    public enum msevtmgt_authenticationsettingsmsevtmgt_AuthenticationType
    {
        Basic = 100000000,
        s2s = 100000001,
    }
    public enum msevtmgt_authenticationsettingsStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_bpf_9623d46752ae497989f62ac0d11dad99StatusCode
    {
        Aborted = 3,
        Active = 1,
        Finished = 2,
    }
    public enum msevtmgt_bucketStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_BuildingStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_CheckInStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_checkintype
    {
        Eventcheckin = 100000001,
        Sessioncheckin = 100000000,
    }
    public enum msevtmgt_Companysize
    {
        _10001ormore = 100000008,
        _1001to2500 = 100000005,
        _101to250 = 100000002,
        _2501to5000 = 100000006,
        _251to500 = 100000003,
        _5001to10000 = 100000007,
        _501to1000 = 100000004,
        _50orless = 100000000,
        _51to100 = 100000001,
    }
    public enum msevtmgt_Configurationstate
    {
        Configuredsuccessfully = 832530001,
        Notconfigured = 832530000,
    }
    public enum msevtmgt_customregistrationfieldStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_Customregistrationfieldtype
    {
        Boolean_yesno = 100000001,
        Multiplechoice = 100000002,
        Simpletext = 100000000,
        Singlechoice = 100000003,
    }
    public enum msevtmgt_EntityCounterStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_eventadministrationmsevtmgt_contactmatchingstrategy
    {
        Email = 100000000,
        Emailfirstnameandlastname = 100000002,
        Firstnameandlastname = 100000001,
    }
    public enum msevtmgt_eventadministrationStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_eventcustomregistrationfieldStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_eventformat
    {
        Hybrid = 100000003,
        Onsite = 100000001,
        Webinar = 100000002,
    }
    public enum msevtmgt_eventmanagementactivityStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_eventmanagementconfigurationStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_eventpurchaseattendeeStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_eventpurchasecontactStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_eventpurchasepassStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_eventpurchaseStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_EventRegistrationmsevtmgt_internalregistrationstatus
    {
        New = 100000000,
        Ready = 100000001,
    }
    public enum msevtmgt_EventRegistrationmsevtmgt_publishingstate
    {
        Failedtopublish = 100000003,
        Parentwebinarfailed = 100000002,
        Pending = 100000000,
        Published = 100000001,
        Webinarprovidererror = 100000004,
    }
    public enum msevtmgt_EventRegistrationStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_EventStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_EventTeamMemberStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_eventtype
    {
        Conference = 100000002,
        Demonstration = 100000003,
        Executivebriefing = 100000001,
        Training = 100000004,
        Webcast = 100000005,
    }
    public enum msevtmgt_eventvendorStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_Eventvendortypeoptionset
    {
        Airline = 100000001,
        Foodcaterer = 100000003,
        Hotelgroup = 100000000,
        Rentalcarprovider = 100000002,
    }
    public enum msevtmgt_fileStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_Floorplan
    {
        Banquet = 100000007,
        Boardroom = 100000006,
        Booth = 100000010,
        Cabaret = 100000008,
        Classroom = 100000001,
        Cocktail = 100000009,
        Herringbone = 100000002,
        Hollowsquare = 100000005,
        Horseshoe = 100000004,
        Theater = 100000000,
        Ushape = 100000003,
    }
    public enum msevtmgt_Gender
    {
        Female = 100000001,
        Male = 100000000,
    }
    public enum msevtmgt_HotelRoomAllocationStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_HotelRoomReservationmsevtmgt_Guesttype
    {
        Attendee = 100000000,
        Eventteammember = 100000002,
        Speaker = 100000001,
    }
    public enum msevtmgt_HotelRoomReservationStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_HotelStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_Industry
    {
        Architectureandengineering = 100000000,
        Financialservices = 100000001,
        Manufacturing = 100000002,
        Mediaentertainment = 100000003,
        Other = 100000008,
        Professionalservices = 100000004,
        Publicsector = 100000005,
        Retail = 100000006,
        Wholesaleanddistribution = 100000007,
    }
    public enum msevtmgt_LayoutStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_Memberrole
    {
        Accommodationmanagement = 100000005,
        Cateringmanagement = 100000008,
        Equipmentmanagement = 100000009,
        Marketingeventpromotion = 100000006,
        Registrationmanagement = 100000003,
        Securitymanagement = 100000004,
        SessionManagement = 100000001,
        Speakermanagement = 100000002,
        Sponsorshipmanagement = 100000007,
        Venuemanagement = 100000000,
    }
    public enum msevtmgt_Membertype
    {
        Externalteammember = 100000001,
        Internalteammember = 100000000,
    }
    public enum msevtmgt_nooryes
    {
        No = 100000001,
        Yes = 100000002,
    }
    public enum msevtmgt_Notifyauthoritiesofeventoptionset
    {
        Complete = 100000003,
        Incomplete = 100000002,
        Notapplicable = 100000001,
    }
    public enum msevtmgt_passStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_primarygoal
    {
        Education = 100000003,
        Marketing = 100000001,
        Morale = 100000004,
        Sales = 100000002,
    }
    public enum msevtmgt_Primaryrole
    {
        Attendee = 100000000,
        Journalist = 100000001,
        Other = 100000002,
    }
    public enum msevtmgt_Providerservicestatus
    {
        Forbidden = 100000002,
        Healthy = 100000000,
        Notauthenticated = 100000001,
        Unhealthy = 100000003,
    }
    public enum msevtmgt_Publishstatus
    {
        Cancelled = 100000004,
        Draft = 100000000,
        Inprogress = 100000002,
        Live = 100000003,
        Readytogolive = 100000001,
    }
    public enum msevtmgt_registrationresponseStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_roomreservationStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_RoomStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_Roomtype
    {
        Doubleroom = 100000001,
        Executivesuite = 100000003,
        Juniorsuite = 100000002,
        Luxurysuite = 100000004,
        Singleroom = 100000000,
    }
    public enum msevtmgt_Seattype
    {
        Businessclass = 100000002,
        Economycoach = 100000000,
        Economyplus = 100000001,
    }
    public enum msevtmgt_SessionRegistrationmsevtmgt_publishingstate
    {
        Failedtopublish = 100000003,
        Parentwebinarfailed = 100000002,
        Pending = 100000000,
        Published = 100000001,
        Webinarprovidererror = 100000004,
    }
    public enum msevtmgt_SessionRegistrationStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_SessionStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_SessionTrackStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_Sessiontype
    {
        Brainstorming = 100000003,
        Breakout = 100000004,
        General = 100000002,
        Handsonlab = 100000000,
        Keynote = 100000001,
        Training = 100000005,
    }
    public enum msevtmgt_speakerengagementStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_SpeakerStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_Speakertype
    {
        Externalspeaker = 100000001,
        Internalspeaker = 100000000,
    }
    public enum msevtmgt_SponsorableArticleStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_Sponsorshipcategory
    {
        Bronze = 100000003,
        Gold = 100000001,
        Platinum = 100000000,
        Silver = 100000002,
    }
    public enum msevtmgt_SponsorshipStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_Sponsorshiptype
    {
        Equipment = 100000002,
        Monetary = 100000000,
        Services = 100000001,
    }
    public enum msevtmgt_Squareunits
    {
        Sqfeet = 100000001,
        Sqmeters = 100000000,
    }
    public enum msevtmgt_status
    {
        Completed = 100000003,
        Inprogress = 100000002,
        Notapplicable = 100000004,
        Notstarted = 100000001,
    }
    public enum msevtmgt_surveystatus
    {
        Completed = 100000003,
        Inprogress = 100000002,
        Notapplicable = 100000004,
        Notstarted = 100000001,
    }
    public enum msevtmgt_Tracktype
    {
        External = 100000001,
        Internal = 100000000,
    }
    public enum msevtmgt_Vehicletypepreference
    {
        Compact = 100000001,
        Economy = 100000000,
        Fullsize = 100000003,
        Luxury = 100000005,
        LuxurySUV = 100000007,
        Premium = 100000004,
        Premiumelite = 100000008,
        Standard = 100000002,
        SUV = 100000006,
    }
    public enum msevtmgt_VenueStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_waitlistitemStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_webapplicationmsevtmgt_userauthenticationtype
    {
        AzureActiveDirectoryB2C = 100000001,
    }
    public enum msevtmgt_webapplicationStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_webinarconfigurationmsevtmgt_authorized
    {
        No = 100000001,
        Tokenexpired = 100000002,
        Yes = 100000000,
    }
    public enum msevtmgt_webinarconfigurationStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_webinarconsentStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_Webinarinvitationoptions
    {
        Onlyinvitedattendeescanjointhewebinar = 100000001,
        Permitanyonewithalinktojointhewebinar = 100000000,
    }
    public enum msevtmgt_Webinarlanguage
    {
        Chinese_Simplified = 100000009,
        Chinese_Traditional = 100000013,
        Dutch = 100000006,
        English = 100000000,
        French = 100000001,
        German = 100000002,
        Hebrew = 100000012,
        Italian = 100000004,
        Japanese = 100000010,
        Korean = 100000011,
        Portuguese = 100000008,
        Russian = 100000005,
        Spanish = 100000003,
        Turkish = 100000007,
    }
    public enum msevtmgt_WebinarProviderStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_WebinarTypeStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_websiteentityconfigurationmsevtmgt_selectedentity
    {
        Building = 100000012,
        Customregistrationfield = 100000006,
        Event = 100000000,
        Eventregistration = 100000004,
        Layout = 100000014,
        Pass = 100000003,
        Registrationresponse = 100000007,
        Room = 100000013,
        Session = 100000001,
        Sessionregistration = 100000005,
        Sessiontrack = 100000002,
        Speaker = 10000009,
        Speakerengagement = 100000010,
        Sponsorship = 100000011,
        Waitlistitem = 100000008,
    }
    public enum msevtmgt_websiteentityconfigurationStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msevtmgt_Yearsinindustry
    {
        _1to5years = 100000001,
        _5to10years = 100000002,
        Over10years = 100000003,
        Underoneyear = 100000000,
    }
    public enum msevtmgt_yesorno
    {
        No = 100000002,
        Yes = 100000001,
    }
    public enum msfp_emailtemplateStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msfp_questionmsfp_choicetype
    {
        Multichoice = 647390001,
        none = 647390002,
        Singlechoice = 647390000,
    }
    public enum msfp_questionmsfp_questiontype
    {
        Choice = 647390000,
        Date = 647390003,
        FileUpload = 647390008,
        MatrixChoice = 647390006,
        MatrixChoiceGroup = 647390005,
        NPS = 647390007,
        Number = 647390009,
        Ranking = 647390004,
        Rating = 647390002,
        Text = 647390001,
    }
    public enum msfp_questionresponseStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msfp_questionStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msfp_surveyinviteInstanceTypeCode
    {
        NotRecurring = 0,
        RecurringException = 3,
        RecurringFutureException = 4,
        RecurringInstance = 2,
        RecurringMaster = 1,
    }
    public enum msfp_surveyinvitemsfp_channel
    {
        Email = 647390000,
        Flow = 647390001,
    }
    public enum msfp_surveyinvitemsfp_invitestatus
    {
        Created = 647390005,
        Failed = 647390004,
        Queued = 647390000,
        Responded = 647390003,
        Sent = 647390002,
        UnSubscribed = 647390001,
    }
    public enum msfp_surveyinvitePriorityCode
    {
        High = 2,
        Low = 0,
        Normal = 1,
    }
    public enum msfp_surveyinviteStatusCode
    {
        Canceled = 3,
        Completed = 2,
        Open = 1,
        Scheduled = 4,
    }
    public enum msfp_surveyresponseInstanceTypeCode
    {
        NotRecurring = 0,
        RecurringException = 3,
        RecurringFutureException = 4,
        RecurringInstance = 2,
        RecurringMaster = 1,
    }
    public enum msfp_surveyresponsemsfp_sentiment
    {
        Negative = 647390002,
        Neutral = 647390001,
        Positive = 647390000,
    }
    public enum msfp_surveyresponsePriorityCode
    {
        High = 2,
        Low = 0,
        Normal = 1,
    }
    public enum msfp_surveyresponseStatusCode
    {
        Canceled = 3,
        Completed = 2,
        Open = 1,
        Scheduled = 4,
    }
    public enum msfp_surveyStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msfp_unsubscribedrecipientStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msgdpr_gdpr_Consent_option_set
    {
        @__1Consent = 587030001,
        @__2Transactional = 587030002,
        @__3Subscriptions = 587030003,
        @__4Marketing = 587030004,
        @__5Profiling = 587030005,
    }
    public enum msgdpr_gdprconfigurationStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum msgdpr_gdprconsentchangerecordStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum NavigationSettingSettingType
    {
        AdvancedSetup = 0,
        AdvancedSetupSummary = 2,
        BasicSetup = 1,
        BasicSetupSummary = 3,
    }
    public enum Need
    {
        Goodtohave = 2,
        Musthave = 0,
        Noneed = 3,
        Shouldhave = 1,
    }
    public enum NewProcessStatusCode
    {
        Aborted = 3,
        Active = 1,
        Finished = 2,
    }
    public enum Opportunity_SalesStage
    {
        Close = 3,
        Develop = 1,
        Propose = 2,
        Qualify = 0,
    }
    public enum OpportunityCloseInstanceTypeCode
    {
        NotRecurring = 0,
        RecurringException = 3,
        RecurringFutureException = 4,
        RecurringInstance = 2,
        RecurringMaster = 1,
    }
    public enum OpportunityCloseOpportunityStateCode
    {
        Lost = 2,
        Open = 0,
        Won = 1,
    }
    public enum OpportunityCloseOpportunityStatusCode
    {
        Canceled = 4,
        InProgress = 1,
        OnHold = 2,
        OutSold = 5,
        Won = 3,
    }
    public enum OpportunityClosePriorityCode
    {
        High = 2,
        Low = 0,
        Normal = 1,
    }
    public enum OpportunityCloseStatusCode
    {
        Canceled = 3,
        Completed = 2,
        Open = 1,
    }
    public enum Opportunitymsdyn_forecastcategory
    {
        Bestcase_moderateconfidence = 100000002,
        Committed_highconfidence = 100000003,
        Omitted_excludefromforecast = 100000004,
        Pipeline_lowconfidence = 100000001,
    }
    public enum OpportunityOpportunityRatingCode
    {
        Cold = 3,
        Hot = 1,
        Warm = 2,
    }
    public enum OpportunityPriorityCode
    {
        DefaultValue = 1,
    }
    public enum OpportunityProductOpportunityStateCode
    {
    }
    public enum OpportunitySalesProcessStatusCode
    {
        Aborted = 3,
        Active = 1,
        Finished = 2,
    }
    public enum OpportunitySalesStageCode
    {
        DefaultValue = 1,
    }
    public enum OpportunityStatusCode
    {
        Canceled = 4,
        InProgress = 1,
        OnHold = 2,
        OutSold = 5,
        Won = 3,
    }
    public enum OpportunityTimeLine
    {
        Immediate = 0,
        NextQuarter = 2,
        Notknown = 4,
        ThisQuarter = 1,
        ThisYear = 3,
    }
    public enum OrderCloseInstanceTypeCode
    {
        NotRecurring = 0,
        RecurringException = 3,
        RecurringFutureException = 4,
        RecurringInstance = 2,
        RecurringMaster = 1,
    }
    public enum OrderClosePriorityCode
    {
        High = 2,
        Low = 0,
        Normal = 1,
    }
    public enum OrderCloseStatusCode
    {
        Canceled = 3,
        Completed = 2,
        Open = 1,
    }
    public enum OrganizationCurrencyDisplayOption
    {
        Currencycode = 1,
        Currencysymbol = 0,
    }
    public enum OrganizationDateFormatCode
    {
    }
    public enum OrganizationDefaultRecurrenceEndRangeType
    {
        EndByDate = 3,
        NoEndDate = 1,
        NumberofOccurrences = 2,
    }
    public enum OrganizationDiscountCalculationMethod
    {
        Lineitem = 0,
        Perunit = 1,
    }
    public enum OrganizationEmailConnectionChannel
    {
        MicrosoftDynamics365EmailRouter = 1,
        ServerSideSynchronization = 0,
    }
    public enum OrganizationFiscalPeriodFormatPeriod
    {
        M0 = 5,
        Month0 = 4,
        MonthName = 7,
        P0 = 3,
        Q0 = 2,
        Quarter0 = 1,
        Semester0 = 6,
    }
    public enum OrganizationFiscalYearFormatPrefix
    {
        _2 = 2,
        FY = 1,
    }
    public enum OrganizationFiscalYearFormatSuffix
    {
        _3 = 3,
        FiscalYear = 2,
        FY = 1,
    }
    public enum OrganizationFiscalYearFormatYear
    {
        GGYY = 3,
        YY = 2,
        YYYY = 1,
    }
    public enum OrganizationFullNameConventionCode
    {
        FirstName = 1,
        FirstNameMiddleInitialLastName = 3,
        FirstNameMiddleNameLastName = 5,
        LastNameFirstName = 0,
        LastNameFirstNameMiddleInitial = 2,
        LastNameFirstNameMiddleName = 4,
        LastNamenospaceFirstName = 7,
        LastNamespaceFirstName = 6,
    }
    public enum OrganizationISVIntegrationCode
    {
        All = 7,
        None = 0,
        Outlook = 6,
        OutlookLaptopClient = 4,
        OutlookWorkstationClient = 2,
        Web = 1,
        WebOutlookLaptopClient = 5,
        WebOutlookWorkstationClient = 3,
    }
    public enum OrganizationNegativeFormatCode
    {
        Brackets = 0,
        Dash = 1,
        DashplusSpace = 2,
        SpaceplusTrailingDash = 4,
        TrailingDash = 3,
    }
    public enum OrganizationOrganizationState
    {
        Active = 3,
        Creating = 0,
        Updating = 2,
        Upgrading = 1,
    }
    public enum OrganizationPluginTraceLogSetting
    {
        All = 2,
        Exception = 1,
        Off = 0,
    }
    public enum OrganizationReportScriptErrors
    {
        AskmeforpermissiontosendanerrorreporttoMicrosoft = 1,
        AutomaticallysendanerrorreporttoMicrosoftwithoutaskingmeforpermission = 2,
        NeversendanerrorreporttoMicrosoftaboutMicrosoftDynamics365 = 3,
        NopreferenceforsendinganerrorreporttoMicrosoftaboutMicrosoftDynamics365 = 0,
    }
    public enum OrganizationSchedulingEngine
    {
        LegacySchedulingEngine = 0,
    }
    public enum OrganizationSharePointDeploymentType
    {
        Online = 0,
        OnPremises = 1,
    }
    public enum OrganizationSyncOptInSelectionStatus
    {
        Failed = 3,
        Passed = 2,
        Processing = 1,
    }
    public enum OrganizationTimeFormatCode
    {
    }
    public enum OrganizationWeekStartDayCode
    {
    }
    public enum OrganizationYammerPostMethod
    {
        Private = 1,
        Public = 0,
    }
    public enum orginsightsconfiguration_Lookback
    {
        _2H = 1,
        _30D = 4,
        _48H = 2,
        _7D = 3,
    }
    public enum orginsightsconfiguration_PlotOption
    {
        Area = 3,
        Bar = 5,
        Bubble = 11,
        Column = 2,
        Donut = 6,
        DoubleDonut = 9,
        Infocard = 7,
        Line = 1,
        LinearGauge = 10,
        List = 8,
        Pie = 4,
    }
    public enum OrgInsightsMetricMetricType
    {
        Category = 2,
        TimeSeries = 1,
    }
    public enum OwnerMappingProcessCode
    {
        Ignore = 2,
        Internal = 3,
        Process = 1,
    }
    public enum OwnerMappingStatusCode
    {
        Active = 0,
    }
    public enum PersonalDocumentTemplateDocumentType
    {
        MicrosoftExcel = 1,
        MicrosoftWord = 2,
    }
    public enum PhoneCallPriorityCode
    {
        High = 2,
        Low = 0,
        Normal = 1,
    }
    public enum PhoneCallStatusCode
    {
        Canceled = 3,
        Made = 2,
        Open = 1,
        Received = 4,
    }
    public enum PhoneToCaseProcessStatusCode
    {
        Aborted = 3,
        Active = 1,
        Finished = 2,
    }
    public enum photo_resolution
    {
        _1024x768 = 2,
        _1600x1200 = 3,
        _2048x1536 = 4,
        _2592x1936 = 5,
        _640x480 = 1,
        DeviceDefault = 0,
    }
    public enum PickListMappingProcessCode
    {
        Ignore = 2,
        Internal = 3,
        Process = 1,
        Unmapped = 4,
    }
    public enum PickListMappingStatusCode
    {
        Active = 0,
    }
    public enum PluginAssemblyAuthType
    {
        BasicAuth = 0,
    }
    public enum PluginAssemblyIsolationMode
    {
        External = 3,
        None = 1,
        Sandbox = 2,
    }
    public enum PluginAssemblySourceType
    {
        AzureWebApp = 3,
        Database = 0,
        Disk = 1,
        Normal = 2,
    }
    public enum PluginTraceLogMode
    {
        Asynchronous = 1,
        Synchronous = 0,
    }
    public enum PluginTraceLogOperationType
    {
        Plugin = 1,
        Unknown = 0,
        WorkflowActivity = 2,
    }
    public enum PositionStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum PostSource
    {
        ActionHubPost = 3,
        AutoPost = 1,
        ManualPost = 2,
    }
    public enum PostType
    {
        Checkin = 1,
        Idea = 2,
        News = 3,
        PrivateMessage = 4,
        Question = 5,
        Repost = 6,
        Status = 7,
    }
    public enum PriceLevelFreightTermsCode
    {
        DefaultValue = 1,
    }
    public enum PriceLevelPaymentMethodCode
    {
        DefaultValue = 1,
    }
    public enum PriceLevelShippingMethodCode
    {
        DefaultValue = 1,
    }
    public enum PriceLevelStatusCode
    {
        Active = 100001,
        Inactive = 100002,
    }
    public enum PrincipalSyncAttributeMapping_SyncDirection
    {
        Bidirectional = 3,
        None = 0,
        ToCRM = 2,
        ToExchange = 1,
    }
    public enum ProcessSessionStatusCode
    {
        Canceled = 5,
        Completed = 4,
        Failed = 6,
        InProgress = 2,
        NotStarted = 1,
        Paused = 3,
    }
    public enum Processstage_Category
    {
        Approval = 7,
        Close = 3,
        Develop = 1,
        Identify = 4,
        Propose = 2,
        Qualify = 0,
        Research = 5,
        Resolve = 6,
    }
    public enum ProcessTriggerControlType
    {
        Attribute = 1,
        FormTab = 2,
    }
    public enum ProcessTriggerPipelineStage
    {
        AfterMainOperation = 40,
        BeforeMainOperation = 20,
        DefaultValue = 0,
    }
    public enum ProcessTriggerScope
    {
        Entity = 2,
        Form = 1,
    }
    public enum ProductAssociationProductIsRequired
    {
        Optional = 0,
        Required = 1,
    }
    public enum ProductAssociationPropertyCustomizationStatus
    {
        Available = 1,
        NotAvailable = 0,
    }
    public enum ProductAssociationStatusCode
    {
        Active = 1,
        Draft = 0,
        DraftActive = 3,
        Inactive = 2,
    }
    public enum ProductPriceLevelPricingMethodCode
    {
        CurrencyAmount = 1,
        PercentMarginCurrentCost = 4,
        PercentMarginStandardCost = 6,
        PercentMarkupCurrentCost = 3,
        PercentMarkupStandardCost = 5,
        PercentofList = 2,
    }
    public enum ProductPriceLevelQuantitySellingCode
    {
        NoControl = 1,
        Whole = 2,
        WholeandFractional = 3,
    }
    public enum ProductPriceLevelRoundingOptionCode
    {
        Endsin = 1,
        Multipleof = 2,
    }
    public enum ProductPriceLevelRoundingPolicyCode
    {
        Down = 3,
        None = 1,
        ToNearest = 4,
        Up = 2,
    }
    public enum ProductProductStructure
    {
        Product = 1,
        ProductBundle = 3,
        ProductFamily = 2,
    }
    public enum ProductProductTypeCode
    {
        FlatFees = 4,
        MiscellaneousCharges = 2,
        SalesInventory = 1,
        Services = 3,
    }
    public enum ProductStatusCode
    {
        Active = 1,
        Draft = 0,
        Retired = 2,
        UnderRevision = 3,
    }
    public enum ProductSubstituteDirection
    {
        BiDirectional = 1,
        UniDirectional = 0,
    }
    public enum ProductSubstituteSalesRelationshipType
    {
        Accessory = 2,
        Crosssell = 1,
        Substitute = 3,
        Upsell = 0,
    }
    public enum ProductSubstituteStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum PublisherAddress1_AddressTypeCode
    {
        DefaultValue = 1,
    }
    public enum PublisherAddress1_ShippingMethodCode
    {
        DefaultValue = 1,
    }
    public enum PublisherAddress2_AddressTypeCode
    {
        DefaultValue = 1,
    }
    public enum PublisherAddress2_ShippingMethodCode
    {
        DefaultValue = 1,
    }
    public enum PublisherAddressAddressTypeCode
    {
        BillTo = 1,
        Other = 4,
        Primary = 3,
        ShipTo = 2,
    }
    public enum PublisherAddressFreightTermsCode
    {
        FOB = 1,
        NoCharge = 2,
    }
    public enum PublisherAddressShippingMethodCode
    {
        Default = 1,
    }
    public enum PurchaseProcess
    {
        Committee = 1,
        Individual = 0,
        Unknown = 2,
    }
    public enum PurchaseTimeFrame
    {
        Immediate = 0,
        NextQuarter = 2,
        ThisQuarter = 1,
        ThisYear = 3,
        Unknown = 4,
    }
    public enum Qooi_PricingErrorCode
    {
        BaseCurrencyAttributeOverflow = 36,
        BaseCurrencyAttributeUnderflow = 37,
        DetailError = 1,
        DiscountTypeInvalidState = 27,
        InactiveDiscountType = 33,
        InactivePriceLevel = 3,
        InvalidCurrentCost = 20,
        InvalidDiscount = 28,
        InvalidDiscountType = 26,
        InvalidPrice = 19,
        InvalidPriceLevelAmount = 17,
        InvalidPriceLevelCurrency = 34,
        InvalidPriceLevelPercentage = 18,
        InvalidPricingCode = 9,
        InvalidPricingPrecision = 30,
        InvalidProduct = 7,
        InvalidQuantity = 29,
        InvalidRoundingAmount = 24,
        InvalidRoundingOption = 23,
        InvalidRoundingPolicy = 22,
        InvalidStandardCost = 21,
        MissingCurrentCost = 15,
        MissingPrice = 14,
        MissingPriceLevel = 2,
        MissingPriceLevelAmount = 12,
        MissingPriceLevelPercentage = 13,
        MissingPricingCode = 8,
        MissingProduct = 6,
        MissingProductDefaultUOM = 31,
        MissingProductUOMSchedule = 32,
        MissingQuantity = 4,
        MissingStandardCost = 16,
        MissingUnitPrice = 5,
        MissingUOM = 10,
        None = 0,
        PriceAttributeOutOfRange = 35,
        PriceCalculationError = 25,
        ProductNotInPriceLevel = 11,
        Transactioncurrencyisnotsetfortheproductpricelistitem = 38,
    }
    public enum qooi_skippricecalculation
    {
        DoPriceCalcAlways = 0,
        SkipPriceCalcOnRetrieve = 1,
    }
    public enum qooidetail_skippricecalculation
    {
        DoPriceCalcAlways = 0,
        SkipPriceCalcOnCreate = 1,
        SkipPriceCalcOnUpdate = 2,
    }
    public enum QooiProduct_ProductType
    {
        Bundle = 2,
        OptionalBundleProduct = 4,
        Product = 1,
        ProjectbasedService = 5,
        RequiredBundleProduct = 3,
    }
    public enum QooiProduct_PropertiesConfigurationStatus
    {
        Edit = 0,
        NotConfigured = 2,
        Rectify = 1,
    }
    public enum QueueEmailRouterAccessApproval
    {
        Approved = 1,
        Empty = 0,
        PendingApproval = 2,
        Rejected = 3,
    }
    public enum QueueIncomingEmailDeliveryMethod
    {
        ForwardMailbox = 3,
        None = 0,
        ServerSideSynchronizationorEmailRouter = 2,
    }
    public enum QueueIncomingEmailFilteringMethod
    {
        Allemailmessages = 0,
        EmailmessagesfromDynamics365LeadsContactsandAccounts = 2,
        EmailmessagesfromDynamics365recordsthatareemailenabled = 3,
        EmailmessagesinresponsetoDynamics365email = 1,
        Noemailmessages = 4,
    }
    public enum QueueItemObjectTypeCode
    {
        Activity = 4200,
        Appointment = 4201,
        Appointmentactivitymarketingtemplate = 10162,
        CampaignActivity = 4402,
        CampaignResponse = 4401,
        Case = 112,
        Email = 4202,
        Fax = 4204,
        FormsProsurveyinvite = 10049,
        FormsProsurveyresponse = 10050,
        KnowledgeArticle = 9953,
        Letter = 4207,
        Marketingactivity = 10193,
        PhoneCall = 4210,
        Phonecallactivitymarketingtemplate = 10194,
        QuickCampaign = 4406,
        RecurringAppointment = 4251,
        ResponseError = 10089,
        ServiceActivity = 4214,
        SocialActivity = 4216,
        SurveyActivity = 10094,
        SurveyResponse = 10096,
        Task = 4212,
        Taskactivitymarketingtemplate = 10199,
    }
    public enum QueueItemStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum QueueOutgoingEmailDeliveryMethod
    {
        None = 0,
        ServerSideSynchronizationorEmailRouter = 2,
    }
    public enum QueueQueueTypeCode
    {
        DefaultValue = 1,
    }
    public enum QueueQueueViewType
    {
        Private = 1,
        Public = 0,
    }
    public enum QueueStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum QuoteCloseInstanceTypeCode
    {
        NotRecurring = 0,
        RecurringException = 3,
        RecurringFutureException = 4,
        RecurringInstance = 2,
        RecurringMaster = 1,
    }
    public enum QuoteClosePriorityCode
    {
        High = 2,
        Low = 0,
        Normal = 1,
    }
    public enum QuoteCloseStatusCode
    {
        Canceled = 3,
        Completed = 2,
        Open = 1,
    }
    public enum QuoteDetailQuoteStateCode
    {
    }
    public enum QuoteDetailShipTo_FreightTermsCode
    {
        FOB = 1,
        NoCharge = 2,
    }
    public enum QuoteFreightTermsCode
    {
        FOB = 1,
        NoCharge = 2,
    }
    public enum QuotePaymentTermsCode
    {
        _210Net30 = 2,
        Net30 = 1,
        Net45 = 3,
        Net60 = 4,
    }
    public enum QuoteShippingMethodCode
    {
        Airborne = 1,
        DHL = 2,
        FedEx = 3,
        FullLoad = 6,
        PostalMail = 5,
        UPS = 4,
        WillCall = 7,
    }
    public enum QuoteShipTo_FreightTermsCode
    {
        DefaultValue = 1,
    }
    public enum RatingModelStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum RatingValueStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum RecurrenceRule_MonthOfYear
    {
        April = 4,
        August = 8,
        December = 12,
        February = 2,
        InvalidMonthOfYear = 0,
        January = 1,
        July = 7,
        June = 6,
        March = 3,
        May = 5,
        November = 11,
        October = 10,
        September = 9,
    }
    public enum RecurrenceRuleInstance
    {
        First = 1,
        Fourth = 4,
        Last = 5,
        Second = 2,
        Third = 3,
    }
    public enum RecurrenceRulePatternEndType
    {
        NoEndDate = 1,
        Occurrences = 2,
        PatternEndDate = 3,
    }
    public enum RecurrenceRuleRecurrencePatternType
    {
        Daily = 0,
        Monthly = 2,
        Weekly = 1,
        Yearly = 3,
    }
    public enum RecurringAppointmentMasterExpansionStateCode
    {
        Full = 2,
        Partial = 1,
        Unexpanded = 0,
    }
    public enum RecurringAppointmentMasterInstance
    {
        First = 1,
        Fourth = 4,
        Last = 5,
        Second = 2,
        Third = 3,
    }
    public enum RecurringAppointmentMasterInstanceTypeCode
    {
        NotRecurring = 0,
        RecurringException = 3,
        RecurringFutureException = 4,
        RecurringInstance = 2,
        RecurringMaster = 1,
    }
    public enum RecurringAppointmentMasterPatternEndType
    {
        NoEndDate = 1,
        Occurrences = 2,
        PatternEndDate = 3,
    }
    public enum RecurringAppointmentMasterPriorityCode
    {
        High = 2,
        Low = 0,
        Normal = 1,
    }
    public enum RecurringAppointmentMasterRecurrencePatternType
    {
        Daily = 0,
        Monthly = 2,
        Weekly = 1,
        Yearly = 3,
    }
    public enum RecurringAppointmentMasterStatusCode
    {
        Busy = 5,
        Canceled = 4,
        Completed = 3,
        Free = 1,
        OutofOffice = 6,
        Tentative = 2,
    }
    public enum RelationshipRoleStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum ReportCategoryCategoryCode
    {
        AdministrativeReports = 4,
        MarketingReports = 3,
        SalesReports = 1,
        ServiceReports = 2,
    }
    public enum ReportLinkLinkTypeCode
    {
        Drillthrough = 1,
        Drillthroughandsubreport = 3,
        Subreport = 2,
    }
    public enum ReportReportTypeCode
    {
        LinkedReport = 3,
        OtherReport = 2,
        ReportingServicesReport = 1,
    }
    public enum ReportVisibilityVisibilityCode
    {
        Formsforrelatedrecordtypes = 2,
        Listsforrelatedrecordtypes = 3,
        Reportsarea = 1,
    }
    public enum ResourceGroupGroupTypeCode
    {
        Dynamic = 1,
        Hidden = 2,
        Static = 0,
    }
    public enum RoutingRuleStatusCode
    {
        Active = 2,
        Draft = 1,
    }
    public enum SalesLiteratureItemFileTypeCode
    {
        DefaultValue = 1,
    }
    public enum SalesLiteratureLiteratureTypeCode
    {
        Bulletins = 6,
        CompanyBackground = 9,
        Manuals = 8,
        MarketingCollateral = 100001,
        News = 5,
        PoliciesAndProcedures = 2,
        Presentation = 0,
        PriceSheets = 7,
        ProductSheet = 1,
        SalesLiterature = 3,
        Spreadsheets = 4,
    }
    public enum SalesOrderDetailSalesOrderStateCode
    {
    }
    public enum SalesOrderDetailShipTo_FreightTermsCode
    {
        FOB = 1,
        NoCharge = 2,
    }
    public enum SalesOrderFreightTermsCode
    {
        FOB = 1,
        NoCharge = 2,
    }
    public enum SalesOrderPaymentTermsCode
    {
        _210Net30 = 2,
        Net30 = 1,
        Net45 = 3,
        Net60 = 4,
    }
    public enum SalesOrderPriorityCode
    {
        DefaultValue = 1,
    }
    public enum SalesOrderShippingMethodCode
    {
        Airborne = 1,
        DHL = 2,
        FedEx = 3,
        FullLoad = 6,
        PostalMail = 5,
        UPS = 4,
        WillCall = 7,
    }
    public enum SalesOrderShipTo_FreightTermsCode
    {
        DefaultValue = 1,
    }
    public enum SalesOrderStatusCode
    {
        Complete = 100001,
        InProgress = 3,
        Invoiced = 100003,
        New = 1,
        NoMoney = 4,
        Partial = 100002,
        Pending = 2,
    }
    public enum SavedOrgInsightsConfigurationMetricType
    {
        Category = 2,
        TimeSeries = 1,
    }
    public enum SavedQueryStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum SavedQueryVisualizationChartType
    {
        ASPNETCharts = 0,
        PowerBI = 1,
    }
    public enum SavedQueryVisualizationType
    {
        fordatacentricaswellasinteractioncentric = 0,
        justforinteractioncentric = 1,
    }
    public enum SdkMessageProcessingStepImageImageType
    {
        Both = 2,
        PostImage = 1,
        PreImage = 0,
    }
    public enum SdkMessageProcessingStepInvocationSource
    {
        Child = 1,
        Internal = -1,
        Parent = 0,
    }
    public enum SdkMessageProcessingStepMode
    {
        Asynchronous = 1,
        Synchronous = 0,
    }
    public enum SdkMessageProcessingStepStage
    {
        FinalPostoperation_Forinternaluseonly = 55,
        InitialPreoperation_Forinternaluseonly = 5,
        InternalPostoperationAfterExternalPlugins_Forinternaluseonly = 45,
        InternalPostoperationBeforeExternalPlugins_Forinternaluseonly = 35,
        InternalPreoperationAfterExternalPlugins_Forinternaluseonly = 25,
        InternalPreoperationBeforeExternalPlugins_Forinternaluseonly = 15,
        MainOperation_Forinternaluseonly = 30,
        Postoperation = 40,
        Postoperation_Deprecated = 50,
        Preoperation = 20,
        Prevalidation = 10,
    }
    public enum SdkMessageProcessingStepStatusCode
    {
        Disabled = 2,
        Enabled = 1,
    }
    public enum SdkMessageProcessingStepSupportedDeployment
    {
        Both = 2,
        MicrosoftDynamics365ClientforOutlookOnly = 1,
        ServerOnly = 0,
    }
    public enum selectedmobileofflineenabledentityrelationships
    {
    }
    public enum ServiceAppointmentInstanceTypeCode
    {
        NotRecurring = 0,
        RecurringException = 3,
        RecurringFutureException = 4,
        RecurringInstance = 2,
        RecurringMaster = 1,
    }
    public enum ServiceAppointmentPriorityCode
    {
        High = 2,
        Low = 0,
        Normal = 1,
    }
    public enum ServiceAppointmentStatusCode
    {
        Arrived = 7,
        Canceled = 9,
        Completed = 8,
        InProgress = 6,
        NoShow = 10,
        Pending = 3,
        Requested = 1,
        Reserved = 4,
        Tentative = 2,
    }
    public enum ServiceEndpointAuthType
    {
        ACS = 1,
        HttpHeader = 5,
        HttpQueryString = 6,
        SASKey = 2,
        SASToken = 3,
        WebhookKey = 4,
    }
    public enum ServiceEndpointConnectionMode
    {
        Federated = 2,
        Normal = 1,
    }
    public enum ServiceEndpointContract
    {
        EventHub = 7,
        OneWay = 1,
        Queue = 2,
        Queue_Persistent = 6,
        Rest = 3,
        Topic = 5,
        TwoWay = 4,
        Webhook = 8,
    }
    public enum ServiceEndpointMessageFormat
    {
        BinaryXML = 1,
        Json = 2,
        TextXML = 3,
    }
    public enum ServiceEndpointNamespaceFormat
    {
        NamespaceAddress = 2,
        NamespaceName = 1,
    }
    public enum ServiceEndpointUserClaim
    {
        None = 1,
        UserId = 2,
        UserInfo = 3,
    }
    public enum ServiceStage
    {
        Identify = 0,
        Research = 1,
        Resolve = 2,
    }
    public enum ServiceStatusCode
    {
        Arrived = 7,
        Canceled = 9,
        Completed = 8,
        InProgress = 6,
        NoShow = 10,
        Pending = 3,
        Requested = 1,
        Reserved = 4,
        Tentative = 2,
    }
    public enum SharePoint_ValidationStatus
    {
        Couldnotvalidate = 5,
        InProgress = 2,
        Invalid = 3,
        NotValidated = 1,
        Valid = 4,
    }
    public enum SharePoint_ValidationStatusReason
    {
        Authenticationfailure = 6,
        Invalidcertificates = 7,
        TheURLcouldnotbeaccessedbecauseofInternetExplorersecuritysettings = 5,
        TheURLschemesofMicrosoftDynamics365andSharePointaredifferent = 4,
        ThisrecordsURLhasnotbeenvalidated = 1,
        ThisrecordsURLisnotvalid = 3,
        ThisrecordsURLisvalid = 2,
    }
    public enum SharePointDocumentLocation_LocationType
    {
        DedicatedforOneNoteIntegration = 1,
        General = 0,
    }
    public enum SharePointDocumentLocationStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum sharepointsite_ServiceType
    {
        MSTeams = 3,
        OneDrive = 1,
        Sharedwithme = 2,
        SharePoint = 0,
    }
    public enum SharePointSiteStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum SiteAddress1_AddressTypeCode
    {
        DefaultValue = 1,
    }
    public enum SiteAddress1_ShippingMethodCode
    {
        DefaultValue = 1,
    }
    public enum SiteAddress2_AddressTypeCode
    {
        DefaultValue = 1,
    }
    public enum SiteAddress2_ShippingMethodCode
    {
        DefaultValue = 1,
    }
    public enum sla_slaenabledentities
    {
        Case = 112,
    }
    public enum SLAApplicableFromPickList
    {
        No = 1,
        Yes = 2,
    }
    public enum SLAKPIInstanceStatus
    {
        Canceled = 5,
        InProgress = 0,
        NearingNoncompliance = 2,
        Noncompliant = 1,
        Paused = 3,
        Succeeded = 4,
    }
    public enum SLAKPIInstanceWarningTimeReached
    {
        No = 0,
        Yes = 1,
    }
    public enum SLAObjectTypeCode
    {
        Account = 1,
        AccountLeads = 16,
        ACIViewMapper = 8040,
        ActionCard = 9962,
        actioncardregarding = 10032,
        ActionCardRoleSetting = 10033,
        ActionCardType = 9983,
        ActionCardUserSettings = 9973,
        ActionCardUserState = 9968,
        Activity = 4200,
        ActivityParty = 135,
        Address = 1071,
        admin_settings_entity = 10025,
        AdvancedSimilarityRule = 9949,
        AIConfiguration = 402,
        AIFormProcessingDocument = 10008,
        AIModel = 401,
        AIObjectDetectionBoundingBox = 10011,
        AIObjectDetectionImage = 10009,
        AIObjectDetectionImageMapping = 10012,
        AIObjectDetectionLabel = 10010,
        AITemplate = 400,
        AnalysisComponent = 10065,
        AnalysisJob = 10066,
        AnalysisResult = 10067,
        AnalysisResultDetail = 10068,
        Announcement = 132,
        AnnualFiscalCalendar = 2000,
        Answer = 10073,
        AppConfigMaster = 9011,
        AppConfigSetup = 10055,
        AppConfiguration = 9012,
        AppConfigurationInstance = 9013,
        ApplicationFile = 4707,
        ApplicationRibbons = 1120,
        AppModuleComponent = 9007,
        AppModuleMetadata = 8700,
        AppModuleMetadataAsyncOperation = 8702,
        AppModuleMetadataDependency = 8701,
        AppModuleRoles = 9009,
        Appointment = 4201,
        Appointmentactivitymarketingtemplate = 10162,
        Article = 127,
        ArticleComment = 1082,
        ArticleTemplate = 1016,
        Attachment_1001 = 1001,
        Attachment_1002 = 1002,
        AttendeePass = 10110,
        Attribute = 9808,
        AttributeImageConfig = 431,
        AttributeMap = 4601,
        Auditing = 4567,
        AuthenticatedDomain = 10198,
        AuthenticationSettings = 10111,
        AuthorizationServer = 1094,
        AzureDeployment = 10074,
        AzureServiceConnection = 9936,
        BookableResource = 1150,
        BookableResourceBooking = 1145,
        BookableResourceBookingHeader = 1146,
        BookableResourceBookingtoExchangeIdMapping = 4421,
        BookableResourceCategory = 1147,
        BookableResourceCategoryAssn = 1149,
        BookableResourceCharacteristic = 1148,
        BookableResourceGroup = 1151,
        BookingStatus = 1152,
        Bucket = 10113,
        Building = 10114,
        BulkDeleteFailure = 4425,
        BulkDeleteOperation = 4424,
        BulkOperationLog = 4405,
        BusinessDataLocalizedLabel = 4232,
        BusinessProcessFlowInstance = 4725,
        BusinessUnit = 10,
        BusinessUnitMap = 6,
        Calendar = 4003,
        CalendarRule = 4004,
        CallbackRegistration = 301,
        Campaign = 4400,
        CampaignActivity = 4402,
        CampaignActivityItem = 4404,
        CampaignItem = 4403,
        CampaignResponse = 4401,
        CanvasApp = 300,
        Case = 112,
        CaseResolution = 4206,
        Category = 9959,
        CDNconfiguration = 10163,
        ChannelAccessProfile = 3005,
        ChannelAccessProfileRule = 9400,
        ChannelAccessProfileRuleItem = 9401,
        ChannelProperty = 1236,
        ChannelPropertyGroup = 1234,
        Characteristic = 1141,
        Checkin = 10115,
        ChildIncidentCount = 113,
        Clientupdate = 36,
        ColumnMapping = 4417,
        Comment = 8005,
        Commitment = 4215,
        Competitor = 123,
        CompetitorAddress = 1004,
        CompetitorProduct = 1006,
        CompetitorSalesLiterature = 26,
        ComponentLayer = 10006,
        ComponentLayerDataSource = 10007,
        Configuration = 10056,
        Connection = 3234,
        ConnectionRole = 3231,
        ConnectionRoleObjectTypeCode = 3233,
        Connector = 372,
        Contact = 2,
        ContactInvoices = 17,
        ContactLeads = 22,
        ContactOrders = 19,
        ContactQuotes = 18,
        ContactType = 10057,
        Contentblock = 10108,
        Contentsettings = 10165,
        Contract = 1010,
        ContractLine = 1011,
        ContractTemplate = 2011,
        Currency = 9105,
        Customchannelactivity = 10168,
        CustomControl = 9753,
        CustomControlDefaultConfig = 9755,
        CustomControlResource = 9754,
        Customerinsightsinformation = 10166,
        Customerjourney = 10167,
        Customerjourneyiteration = 10169,
        Customerjourneyruntimestate = 10170,
        Customerjourneytemplate = 10171,
        Customerjourneyworkflowlink = 10172,
        CustomerRelationship = 4502,
        CustomRegistrationField = 10116,
        DatabaseVersion = 10014,
        DataImport = 4410,
        DataMap = 4411,
        DataPerformanceDashboard = 4450,
        Defaultmarketingsettings = 10173,
        DelveActionHub = 9961,
        Dependency = 7105,
        DependencyFeature = 7108,
        DependencyNode = 7106,
        Designerfeatureprotection = 10109,
        Digitalassetsconfiguration = 10103,
        Discount = 1013,
        DiscountList = 1080,
        DisplayString = 4102,
        DisplayStringMap = 4101,
        DocumentLocation = 9508,
        DocumentSuggestions = 1189,
        DocumentTemplate = 9940,
        DuplicateDetectionRule = 4414,
        DuplicateRecord = 4415,
        DuplicateRuleCondition = 4416,
        Dynamiccontentmetadata = 10179,
        Email = 4202,
        EmailHash = 4023,
        EmailSearch = 4299,
        EmailServerProfile = 9605,
        EmailSignature = 9997,
        EmailTemplate = 2010,
        Entitlement = 9700,
        EntitlementChannel = 9701,
        EntitlementContact = 7272,
        EntitlementEntityAllocationTypeMapping = 9704,
        EntitlementProduct = 6363,
        EntitlementTemplate = 9702,
        EntitlementTemplateChannel = 9703,
        EntitlementTemplateProduct = 4545,
        Entity = 9800,
        EntityAnalyticsConfig = 430,
        EntityCounter = 10117,
        EntityImageConfig = 432,
        EntityMap = 4600,
        EntityRankingRule = 10034,
        EnvironmentVariableDefinition = 380,
        EnvironmentVariableValue = 381,
        Event = 10118,
        EventAdministration = 10119,
        EventCustomRegistrationField = 10120,
        EventMainBusinessProcessFlow = 10112,
        EventManagementActivity = 10121,
        EventManagementConfiguration = 10122,
        EventPurchase = 10123,
        EventPurchaseAttendee = 10124,
        EventPurchaseContact = 10125,
        EventPurchasePass = 10126,
        EventRegistration = 10127,
        EventTeamMember = 10128,
        EventVendor = 10129,
        ExchangeSyncIdMapping = 4120,
        ExpanderEvent = 4711,
        ExpiredProcess = 955,
        ExternalParty = 3008,
        ExternalPartyItem = 9987,
        FacilityEquipment = 4000,
        Fax = 4204,
        Feedback = 9958,
        FeedbackMapping = 10075,
        FeedbackRelatedSurvey = 10076,
        FieldPermission = 1201,
        FieldSecurityProfile = 1200,
        FieldSharing = 44,
        File = 10104,
        File_OBSOLETE = 10130,
        FileAttachment = 55,
        Filter = 10044,
        FilterTemplate = 30,
        FixedMonthlyFiscalCalendar = 2004,
        flowcardtype = 10035,
        Follow = 8003,
        Forecast = 10027,
        Forecastdefinition = 10026,
        Forecastrecurrence = 10028,
        Formpage = 10174,
        Formpagetemplate = 10175,
        FormsProsurvey = 10048,
        FormsProsurveyemailtemplate = 10045,
        FormsProsurveyinvite = 10049,
        FormsProsurveyquestion = 10046,
        FormsProsurveyquestionresponse = 10047,
        FormsProsurveyresponse = 10050,
        FormsProunsubscribedrecipient = 10051,
        Formwhitelistrule = 10187,
        GDPRconfiguration = 10235,
        GDPRconsentchangerecord = 10236,
        Geopin = 10176,
        GlobalSearchConfiguration = 54,
        Goal = 9600,
        GoalMetric = 9603,
        Grid = 10083,
        Gwennolfeatureconfiguration = 10240,
        HierarchyRule = 8840,
        HierarchySecurityConfiguration = 9919,
        HolidayWrapper = 9996,
        Hotel = 10131,
        HotelRoomAllocation = 10132,
        HotelRoomReservation = 10133,
        icebreakersconfig = 10039,
        Image = 10077,
        ImageDescriptor = 1007,
        ImageTokenCache = 10078,
        ImportData = 4413,
        ImportEntityMapping = 4428,
        ImportJob = 9107,
        ImportLog = 4423,
        ImportSourceFile = 4412,
        IncidentKnowledgeBaseRecord = 9931,
        IndexedArticle = 126,
        IntegrationStatus = 3000,
        InteractionforEmail = 9986,
        InternalAddress = 1003,
        InterProcessLock = 4011,
        InvalidDependency = 7107,
        Invoice = 1090,
        InvoiceProduct = 1091,
        ISVConfig = 4705,
        Keyword = 10105,
        KnowledgeArticle = 9953,
        KnowledgeArticleCategory = 9960,
        KnowledgeArticleIncident = 9954,
        KnowledgeArticleViews = 9955,
        KnowledgeBaseRecord = 9930,
        KnowledgeSearchModel = 9947,
        Language = 9957,
        LanguageProvisioningState = 9875,
        Layout = 10134,
        Lead = 4,
        LeadAddress = 1017,
        LeadCompetitors = 24,
        Leadentityfield = 10219,
        LeadProduct = 27,
        Leadscore = 10217,
        Leadscore_Deprecated = 10214,
        LeadScoringConfiguration = 10218,
        LeadScoringModel = 10215,
        LeadSource = 10058,
        Leadtoopportunity = 10216,
        LeadToOpportunitySalesProcess = 954,
        Letter = 4207,
        License = 2027,
        Like = 8006,
        LinkedAnswer = 10080,
        LinkedInaccount = 10220,
        LinkedInactivity = 10221,
        LinkedIncampaign = 10222,
        LinkedInfieldmapping = 10224,
        LinkedInformquestion = 10227,
        LinkedInformsubmissionanswer = 10226,
        LinkedInLeadGenform = 10225,
        LinkedInLeadGenformsubmission = 10228,
        LinkedInLeadGenintegrationconfiguration = 10223,
        LinkedInleadmatchingstrategy = 10229,
        LinkedInMatchedAudience = 10237,
        LinkedInuserprofile = 10230,
        LinkedQuestion = 10085,
        ListForm = 10177,
        ListValueMapping = 4418,
        Loan = 3,
        LoanProgram = 10059,
        LoanPurpose = 10060,
        LoanStatus = 10061,
        LoanType = 10062,
        LocalConfigStore = 9201,
        LookupMapping = 4419,
        Mailbox = 9606,
        MailboxAutoTrackingFolder = 9608,
        MailboxStatistics = 9607,
        MailboxTrackingCategory = 9609,
        MailMergeTemplate = 9106,
        Marketingactivity = 10193,
        Marketinganalyticsconfiguration = 10164,
        Marketingconfiguration = 10178,
        Marketingemail = 10180,
        Marketingemaildynamiccontentmetadata = 10181,
        Marketingemailtemplate = 10182,
        MarketingEmailTest = 10210,
        MarketingEmailTestAttribute = 10211,
        Marketingemailtestsend = 10183,
        Marketingfeatureconfiguration = 10234,
        Marketingfieldsubmission = 10212,
        Marketingform = 10184,
        Marketingformfield = 10185,
        Marketingformsubmission = 10213,
        Marketingformtemplate = 10186,
        Marketinglist = 4300,
        MarketingListMember = 4301,
        Marketingpage = 10188,
        Marketingpageconfiguration = 10189,
        Marketingpagetemplate = 10190,
        Marketingwebsite = 10202,
        Matchingstrategy = 10191,
        Matchingstrategyattributes = 10192,
        MetadataDifference = 4231,
        MicrosoftTeamsCollaborationentity = 10054,
        MicrosoftTeamsGraphresourceEntity = 10052,
        Migration = 10246,
        MobileOfflineProfile = 9866,
        MobileOfflineProfileItem = 9867,
        MobileOfflineProfileItemAssociation = 9868,
        ModeldrivenApp = 9006,
        MonthlyFiscalCalendar = 2003,
        MortgageLoanProcess = 10063,
        msdyn_msteamssetting = 10053,
        msdyn_relationshipinsightsunifiedconfig = 10029,
        MultiEntitySearch = 9910,
        MultiSelectOptionValue = 9912,
        NavigationSetting = 9900,
        NewProcess = 950,
        Note = 5,
        NotesanalysisConfig = 10038,
        Notification = 4110,
        ODatav4DataSource = 10002,
        OfficeDocument = 4490,
        OfficeGraphDocument = 9950,
        OfflineCommandDefinition = 9870,
        OpportunityClose = 4208,
        OpportunityCompetitors = 25,
        OpportunityProduct = 1083,
        OpportunityRelationship = 4503,
        OpportunitySalesProcess = 953,
        OptionSet = 9809,
        Order = 1088,
        OrderClose = 4209,
        OrderProduct = 1089,
        Organization = 1019,
        OrganizationInsightsMetric = 9699,
        OrganizationInsightsNotification = 9690,
        OrganizationStatistic = 4708,
        OrganizationUI = 1021,
        Owner = 7,
        OwnerMapping = 4420,
        Page = 10081,
        PartnerApplication = 1095,
        Pass = 10135,
        PersonalDocumentTemplate = 9941,
        Personalizedpage = 10244,
        Personalizedpagefield = 10245,
        PhoneCall = 4210,
        Phonecallactivitymarketingtemplate = 10194,
        PhoneToCaseProcess = 952,
        Playbook = 10022,
        Playbookactivity = 10019,
        Playbookactivityattribute = 10020,
        PlaybookCallableContext = 10018,
        Playbookcategory = 10021,
        Playbooktemplate = 10023,
        PluginAssembly = 4605,
        PluginProfile = 10072,
        PluginTraceLog = 4619,
        PluginType = 4602,
        PluginTypeStatistic = 4603,
        Portalsettings = 10195,
        Position = 50,
        Post = 8000,
        PostConfiguration = 10041,
        PostRegarding = 8002,
        PostRole = 8001,
        PostRuleConfiguration = 10042,
        PriceList = 1022,
        PriceListItem = 1026,
        PrincipalSyncAttributeMap = 1404,
        Privilege = 1023,
        PrivilegeObjectTypeCode = 31,
        Process = 4703,
        ProcessConfiguration = 9650,
        ProcessDependency = 4704,
        ProcessLog = 4706,
        ProcessSession = 4710,
        ProcessStage = 4724,
        ProcessTrigger = 4712,
        Product = 1024,
        ProductAssociation = 1025,
        ProductRelationship = 1028,
        ProductSalesLiterature = 21,
        ProfileAlbum = 10040,
        Property_10064 = 10064,
        Property_1048 = 1048,
        PropertyAssociation = 1235,
        PropertyInstance = 1333,
        PropertyOptionSetItem = 1049,
        Publisher = 7101,
        PublisherAddress = 7102,
        QuarterlyFiscalCalendar = 2002,
        Question = 10082,
        QuestionResponse = 10084,
        Queue = 2020,
        QueueItem = 2029,
        QueueItemCount = 2023,
        QueueMemberCount = 2024,
        QuickCampaign = 4406,
        Quotainfoentity = 10233,
        Quote = 1084,
        QuoteClose = 4211,
        QuoteProduct = 1085,
        RatingModel = 1144,
        RatingValue = 1142,
        RecordCreationandUpdateRule = 9300,
        RecordCreationandUpdateRuleItem = 9301,
        RecurrenceRule = 4250,
        RecurringAppointment = 4251,
        RedirectURL = 10196,
        RegistrationResponse = 10136,
        RelationshipRole = 4500,
        RelationshipRoleMap = 4501,
        ReplicationBacklog = 1140,
        Report = 9100,
        ReportLink = 9104,
        ReportRelatedCategory = 9102,
        ReportRelatedEntity = 9101,
        ReportVisibility = 9103,
        Resource = 4002,
        ResourceExpansion = 4010,
        ResourceGroup = 4007,
        ResourceSpecification = 4006,
        ResponseAction = 10086,
        ResponseCondition = 10088,
        ResponseError = 10089,
        ResponseOutcome = 10090,
        ResponseRouting = 10091,
        RibbonClientMetadata = 4579,
        RibbonCommand = 1116,
        RibbonContextGroup = 1115,
        RibbonDifference = 1130,
        RibbonMetadataToProcess = 9880,
        RibbonRule = 1117,
        RibbonTabToCommandMapping = 1113,
        RoleTemplate = 1037,
        RollupField = 9604,
        RollupJob = 9511,
        RollupProperties = 9510,
        RollupQuery = 9602,
        Room = 10137,
        RoomReservation = 10138,
        RoutingRuleSet = 8181,
        RuleItem = 8199,
        RuntimeDependency = 7200,
        SalesAttachment = 1070,
        salesinsightssettings = 10036,
        SalesLiterature = 1038,
        SalesProcessInstance = 32,
        SavedOrganizationInsightsConfiguration = 1309,
        SavedView = 4230,
        SchedulingGroup = 4005,
        SdkMessage = 4606,
        SdkMessageFilter = 4607,
        SdkMessagePair = 4613,
        SdkMessageProcessingStep = 4608,
        SdkMessageProcessingStepImage = 4615,
        SdkMessageProcessingStepSecureConfiguration = 4616,
        SdkMessageRequest = 4609,
        SdkMessageRequestField = 4614,
        SdkMessageResponse = 4610,
        SdkMessageResponseField = 4611,
        Section = 10092,
        SecurityRole = 1036,
        Segment = 10197,
        SemiannualFiscalCalendar = 2001,
        Service = 4001,
        ServiceActivity = 4214,
        ServiceContractContact = 20,
        ServiceEndpoint = 4618,
        Session = 10139,
        SessionRegistration = 10140,
        SessionTrack = 10141,
        SharePointData = 9509,
        SharepointDocument = 9507,
        SharePointSite = 9502,
        siconfig = 10030,
        SIKeyValueConfig = 10031,
        SimilarityRule = 9951,
        Site = 4009,
        SiteMap = 4709,
        SLA = 9750,
        SLAItem = 9751,
        SLAKPIInstance = 9752,
        SocialActivity = 4216,
        SocialInsightsConfiguration = 1300,
        Socialpost = 10241,
        SocialPostingConfiguration = 10242,
        SocialPostingConsent = 10243,
        SocialProfile = 99,
        Solution = 7100,
        SolutionComponent = 7103,
        SolutionComponentDataSource = 10001,
        SolutionComponentDefinition = 7104,
        SolutionComponentFileConfiguration = 10005,
        SolutionComponentSummary = 10000,
        SolutionHealthRule = 10069,
        SolutionHealthRuleArgument = 10070,
        SolutionHealthRuleSet = 10071,
        SolutionHistory = 10003,
        SolutionHistoryData = 9890,
        SolutionHistoryDataSource = 10004,
        SpamScoreActivity = 10238,
        SpamScoreRequest = 10239,
        Speaker = 10142,
        SpeakerEngagement = 10143,
        SponsorableArticle = 10144,
        Sponsorship = 10145,
        StatusMap = 1075,
        StringMap = 1043,
        Subject = 129,
        Subscription = 29,
        SubscriptionClients = 1072,
        SubscriptionManuallyTrackedObject = 37,
        SubscriptionStatisticOffline = 45,
        SubscriptionStatisticOutlook = 46,
        SubscriptionSyncEntryOffline = 47,
        SubscriptionSyncEntryOutlook = 48,
        SubscriptionSynchronizationInformation = 33,
        SuggestionCardTemplate = 1190,
        Survey = 10093,
        SurveyActivity = 10094,
        SurveyResponse = 10096,
        SyncAttributeMapping = 1401,
        SyncAttributeMappingProfile = 1400,
        SyncError = 9869,
        SystemApplicationMetadata = 7000,
        SystemChart = 1111,
        SystemForm = 1030,
        SystemJob = 4700,
        SystemUserBusinessUnitEntityMap = 42,
        SystemUserManagerMap = 51,
        SystemUserPrincipal = 14,
        Task = 4212,
        Taskactivitymarketingtemplate = 10199,
        Team = 9,
        TeamProfiles = 1203,
        TeamSyncAttributeMappingProfiles = 1403,
        Teamtemplate = 92,
        Territory = 2013,
        TextAnalyticsEntityMapping = 9945,
        TextAnalyticsTopic = 9948,
        Theme_10097 = 10097,
        Theme_2015 = 2015,
        TimeStampDateMapping = 9932,
        TimeZoneDefinition = 4810,
        TimeZoneLocalizedName = 4812,
        TimeZoneRule = 4811,
        TopicHistory = 9946,
        TopicModel = 9944,
        TopicModelConfiguration = 9942,
        TopicModelExecutionHistory = 9943,
        Trace = 8050,
        TraceAssociation = 8051,
        TraceRegarding = 8052,
        Trackinginformationfordeletedentities = 35,
        TransformationMapping = 4426,
        TransformationParameterMapping = 4427,
        TranslationProcess = 951,
        UICconfig = 10200,
        Unit = 1055,
        UnitGroup = 1056,
        UnresolvedAddress = 2012,
        UntrackedAppointment = 10037,
        UntrackedEmail = 4220,
        UpgradeRun = 10015,
        UpgradeStep = 10016,
        UpgradeVersion = 10017,
        User = 8,
        UserApplicationMetadata = 7001,
        UserChart = 1112,
        UserDashboard = 1031,
        UserEntityInstanceData = 2501,
        UserEntityUISettings = 2500,
        UserFiscalCalendar = 1086,
        Usergeoregion = 10201,
        UserMapping = 2016,
        UserSearchFacet = 52,
        UserSettings = 150,
        Venue = 10146,
        Video = 10106,
        View = 1039,
        VirtualEntityDataProvider = 78,
        VirtualEntityDataSource = 85,
        VoCImport = 10079,
        VoCResponseBlobStore = 10087,
        VoiceoftheCustomerConfiguration = 10098,
        VoiceoftheCustomerLog = 10095,
        WaitlistItem = 10147,
        WallView = 10043,
        WebApplication = 10148,
        WebinarConfiguration = 10149,
        WebinarConsent = 10150,
        WebinarProvider = 10151,
        WebinarType = 10152,
        WebResource = 9333,
        WebsiteEntityConfiguration = 10153,
        WebWizard = 4800,
        WebWizardAccessPrivilege = 4803,
        WizardPage = 4802,
        WorkflowWaitSubscription = 4702,
    }
    public enum SLASLAType
    {
        Enhanced = 1,
        Standard = 0,
    }
    public enum SLAStatusCode
    {
        Active = 2,
        Draft = 1,
    }
    public enum SocialActivity_PostMessageType
    {
        PrivateMessage = 1,
        PublicMessage = 0,
    }
    public enum SocialActivityPriorityCode
    {
        High = 2,
        Low = 0,
        Normal = 1,
    }
    public enum SocialActivityStatusCode
    {
        Canceled = 5,
        Completed = 1,
        Failed = 2,
        Open = 4,
        Processing = 3,
    }
    public enum SocialInsightsConfigurationFormTypeCode
    {
        SystemForm = 1030,
        UserForm = 1031,
    }
    public enum SocialInsightsConfigurationSocialDataItemType
    {
        Class = 2,
        SearchItem = 1,
    }
    public enum SocialProfile_Community
    {
        Facebook = 1,
        Other = 0,
        Twitter = 2,
    }
    public enum SocialProfileStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum SolutionComponentDefinitionRemoveActiveCustomizationsBehavior
    {
        Cascade = 2,
        NoCascade = 1,
        None = 0,
    }
    public enum SolutionComponentFileConfigurationEncodingFormat
    {
        Base64 = 1,
        None = 0,
        UTF8 = 2,
    }
    public enum SolutionComponentFileConfigurationStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum SolutionComponentRootComponentBehavior
    {
        Donotincludesubcomponents = 1,
        IncludeAsShellOnly = 2,
        IncludeSubcomponents = 0,
    }
    public enum SolutionHistoryDataOperation
    {
        Export = 2,
        Import = 0,
        Uninstall = 1,
    }
    public enum SolutionHistoryDataStatus
    {
        End = 1,
        Start = 0,
    }
    public enum SolutionHistoryDataSubOperation
    {
        Delete = 4,
        New = 1,
        None = 0,
        Update = 3,
        Upgrade = 2,
    }
    public enum SolutionSolutionType
    {
        Internal = 2,
        None = 0,
        Snapshot = 1,
    }
    public enum SyncAttributeMapping_SyncDirection
    {
        Bidirectional = 3,
        None = 0,
        ToCRM = 2,
        ToExchange = 1,
    }
    public enum SyncErrorErrorType
    {
        Conflict = 0,
        Others = 3,
        Recordalreadyexists = 2,
        Recordnotfound = 1,
    }
    public enum SyncErrorStatusCode
    {
        Active = 0,
        Fixed = 1,
    }
    public enum SystemFormFormActivationState
    {
        Active = 1,
        Inactive = 0,
    }
    public enum SystemFormFormPresentation
    {
        AirForm = 1,
        ClassicForm = 0,
        ConvertedICForm = 2,
    }
    public enum SystemFormType
    {
        AppointmentBook = 1,
        AppointmentBookBackup = 102,
        Card = 11,
        Dashboard = 0,
        Dialog = 8,
        InteractionCentricDashboard = 10,
        Main = 2,
        MainBackup = 101,
        MainInteractiveexperience = 12,
        MiniCampaignBO = 3,
        MobileExpress = 5,
        Other = 100,
        PowerBIDashboard = 103,
        Preview = 4,
        QuickCreate = 7,
        QuickViewForm = 6,
        TaskFlowForm = 9,
    }
    public enum SystemUserAccessMode
    {
        Administrative = 1,
        DelegatedAdmin = 5,
        Noninteractive = 4,
        Read = 2,
        ReadWrite = 0,
        SupportUser = 3,
    }
    public enum SystemUserAddress1_AddressTypeCode
    {
        DefaultValue = 1,
    }
    public enum SystemUserAddress1_ShippingMethodCode
    {
        DefaultValue = 1,
    }
    public enum SystemUserAddress2_AddressTypeCode
    {
        DefaultValue = 1,
    }
    public enum SystemUserAddress2_ShippingMethodCode
    {
        DefaultValue = 1,
    }
    public enum SystemUserCALType
    {
        Administrative = 1,
        Basic = 2,
        DeviceBasic = 4,
        DeviceEnterprise = 8,
        DeviceEssential = 6,
        DeviceProfessional = 3,
        Enterprise = 7,
        Essential = 5,
        FieldService = 11,
        Professional = 0,
        ProjectService = 12,
        Sales = 9,
        Service = 10,
    }
    public enum SystemUserEmailRouterAccessApproval
    {
        Approved = 1,
        Empty = 0,
        PendingApproval = 2,
        Rejected = 3,
    }
    public enum SystemUserIncomingEmailDeliveryMethod
    {
        ForwardMailbox = 3,
        MicrosoftDynamics365forOutlook = 1,
        None = 0,
        ServerSideSynchronizationorEmailRouter = 2,
    }
    public enum SystemUserInviteStatusCode
    {
        InvitationAccepted = 4,
        InvitationExpired = 3,
        InvitationNearExpired = 2,
        InvitationNotSent = 0,
        InvitationRejected = 5,
        InvitationRevoked = 6,
        Invited = 1,
    }
    public enum SystemUserOutgoingEmailDeliveryMethod
    {
        MicrosoftDynamics365forOutlook = 1,
        None = 0,
        ServerSideSynchronizationorEmailRouter = 2,
    }
    public enum SystemUserPreferredAddressCode
    {
        MailingAddress = 1,
        OtherAddress = 2,
    }
    public enum SystemUserPreferredEmailCode
    {
        DefaultValue = 1,
    }
    public enum SystemUserPreferredPhoneCode
    {
        HomePhone = 3,
        MainPhone = 1,
        MobilePhone = 4,
        OtherPhone = 2,
    }
    public enum TaskPriorityCode
    {
        High = 2,
        Low = 0,
        Normal = 1,
    }
    public enum TaskStatusCode
    {
        Canceled = 6,
        Completed = 5,
        Deferred = 7,
        InProgress = 3,
        NotStarted = 2,
        Waitingonsomeoneelse = 4,
    }
    public enum TeamTeamType
    {
        AADOfficeGroup = 3,
        AADSecurityGroup = 2,
        Access = 1,
        Owner = 0,
    }
    public enum TextAnalyticsEntityMappingEntityPickList
    {
        No = 1,
        Yes = 2,
    }
    public enum TextAnalyticsEntityMappingFieldPickList
    {
        No = 1,
        Yes = 2,
    }
    public enum ThemeStatusCode
    {
        Custom = 1,
        System = 2,
    }
    public enum TopicModelConfigurationTimeFilter
    {
        LastNDays = 1,
        LastNWeeks = 2,
    }
    public enum TopicModelExecutionHistoryStatus
    {
        Failed = 4,
        Inprogress = 2,
        Queued = 1,
        Success = 3,
    }
    public enum TopicModelExecutionHistoryStatusReason
    {
        Analysisfailed = 6,
        Analyzingtopicanalysisexecution = 3,
        Connectionfailed = 7,
        Synchronizationfailed = 5,
        Topicanalysisexecutionisqueued = 1,
        Topicanalysisexecutionissynchronizing = 2,
        Topicanalysishasbuilt = 4,
    }
    public enum TopicModelStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum TraceLogLevel
    {
        Error = 3,
        Information = 1,
        Warning = 2,
    }
    public enum TransactionCurrencyStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum TransformationMappingProcessCode
    {
        Ignore = 2,
        Internal = 3,
        Process = 1,
    }
    public enum TransformationMappingStatusCode
    {
        Active = 0,
    }
    public enum TransformationParameterMappingDataTypeCode
    {
        Reference = 0,
        Value = 1,
    }
    public enum TransformationParameterMappingParameterTypeCode
    {
        Input = 0,
        Output = 1,
    }
    public enum TranslationProcessStatusCode
    {
        Aborted = 3,
        Active = 1,
        Finished = 2,
    }
    public enum UoMScheduleStatusCode
    {
        Active = 1,
        Inactive = 2,
    }
    public enum UserFormType
    {
        Dashboard = 0,
        PowerBIDashboard = 103,
    }
    public enum UserMappingPartnerApplicationType
    {
        Exchange = 1,
        SharePoint = 0,
    }
    public enum UserQueryStatusCode
    {
        Active = 1,
        All = 3,
        Inactive = 2,
    }
    public enum UserQueryVisualizationChartType
    {
        ASPNETCharts = 0,
        PowerBI = 1,
    }
    public enum UserSettingsDataValidationModeForExportToExcel
    {
        Full = 0,
        None = 1,
    }
    public enum UserSettingsDefaultSearchExperience
    {
        Categorizedsearch = 1,
        Customsearch = 3,
        Relevancesearch = 0,
        Uselastsearch = 2,
    }
    public enum UserSettingsEntityFormMode
    {
        Edit = 2,
        Organizationdefault = 0,
        Readoptimized = 1,
    }
    public enum UserSettingsIncomingEmailFilteringMethod
    {
        Allemailmessages = 0,
        EmailmessagesfromDynamics365LeadsContactsandAccounts = 2,
        EmailmessagesfromDynamics365recordsthatareemailenabled = 3,
        EmailmessagesinresponsetoDynamics365email = 1,
        Noemailmessages = 4,
    }
    public enum UserSettingsReportScriptErrors
    {
        AskmeforpermissiontosendanerrorreporttoMicrosoft = 1,
        AutomaticallysendanerrorreporttoMicrosoftwithoutaskingmeforpermission = 2,
        NeversendanerrorreporttoMicrosoftaboutMicrosoftDynamics365 = 3,
    }
    public enum UserSettingsVisualizationPaneLayout
    {
        Sidebyside = 1,
        Topbottom = 0,
    }
    public enum WebResourceWebResourceType
    {
        Data_XML = 4,
        GIFformat = 7,
        ICOformat = 10,
        JPGformat = 6,
        PNGformat = 5,
        Script_JScript = 3,
        Silverlight_XAP = 8,
        String_RESX = 12,
        StyleSheet_CSS = 2,
        StyleSheet_XSL = 9,
        Vectorformat_SVG = 11,
        Webpage_HTML = 1,
    }
    public enum Workflow_RunAs
    {
        CallingUser = 1,
        Owner = 0,
    }
    public enum Workflow_Stage
    {
        Postoperation = 40,
        Preoperation = 20,
    }
    public enum WorkflowBusinessProcessType
    {
        BusinessFlow = 0,
        TaskFlow = 1,
    }
    public enum WorkflowCategory
    {
        Action = 3,
        BusinessProcessFlow = 4,
        BusinessRule = 2,
        Dialog = 1,
        ModernFlow = 5,
        Reserved = 6,
        Workflow = 0,
    }
    public enum WorkflowDependencyType
    {
        ArgumentEntitythatworkflowdependson = 9,
        Attributedefinitionthatworkflowdependson = 8,
        Customentitydefinitionthatworkflowdependson = 7,
        Localparameter = 2,
        Primaryentity = 3,
        PrimaryentityafterSDKoperation = 5,
        PrimaryentitybeforeSDKoperation = 4,
        Relatedentity = 6,
        Sdkassociation = 1,
    }
    public enum WorkflowLog_ObjectTypeCode
    {
        SystemJob = 4700,
        WorkflowSession = 4710,
    }
    public enum WorkflowLogStatus
    {
        Canceled = 4,
        Failed = 3,
        InProgress = 1,
        Succeeded = 2,
        Waiting = 5,
    }
    public enum WorkflowMode
    {
        Background = 0,
        Realtime = 1,
    }
    public enum WorkflowScope
    {
        BusinessUnit = 2,
        Organization = 4,
        ParentChildBusinessUnits = 3,
        User = 1,
    }
    public enum WorkflowStatusCode
    {
        Activated = 2,
        Draft = 1,
    }
    public enum WorkflowType
    {
        Activation = 2,
        Definition = 1,
        Template = 3,
    }
    public enum WorkflowUIFlowType
    {
        Desktop = 0,
        PowerShell = 2,
        SeleniumIDE = 1,
    }
}
