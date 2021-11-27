// *********************************************************************
// Created by : Latebound Constants Generator 1.2020.2.1 for XrmToolBox
// Author     : Jonas Rapp https://twitter.com/rappen
// GitHub     : https://github.com/rappen/LCG-UDG
// Source Org : https://mmdev.crm.dynamics.com/
// Filename   : C:\Users\bipin.kumar\Desktop\PhoneCall.cs
// Created    : 2020-02-21 11:57:24
// *********************************************************************

namespace XRMExtensions
{
    /// <summary>DisplayName: Phone Call, OwnershipType: UserOwned, IntroducedVersion: 5.0.0.0</summary>
    public static class PhoneCall
    {
        public const string EntityName = "phonecall";
        public const string EntityCollectionName = "phonecalls";
        public const string ReasonToCall= "ims_reasontocall";

        #region Attributes

        /// <summary>Type: Uniqueidentifier, RequiredLevel: SystemRequired</summary>
        public const string PrimaryKey = "activityid";
        /// <summary>Type: String, RequiredLevel: ApplicationRequired, MaxLength: 200, Format: Text</summary>
        public const string PrimaryName = "subject";
        /// <summary>Type: Uniqueidentifier, RequiredLevel: None</summary>
        public const string DeprecatedProcessStage = "stageid";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 1250, Format: Text</summary>
        public const string DeprecatedTraversedPath = "traversedpath";
        /// <summary>Type: State, RequiredLevel: SystemRequired, DisplayName: Activity Status, OptionSetType: State</summary>
        public const string ActivityStatus = "statecode";
        /// <summary>Type: EntityName, RequiredLevel: SystemRequired, DisplayName: Activity Type, OptionSetType: Picklist</summary>
        public const string ActivityType = "activitytypecode";
        /// <summary>Type: Virtual (Logical), RequiredLevel: None</summary>
        public const string activitytypecodename = "activitytypecodename";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateOnly, DateTimeBehavior: UserLocal</summary>
        public const string ActualEnd = "actualend";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateOnly, DateTimeBehavior: UserLocal</summary>
        public const string ActualStart = "actualstart";
        /// <summary>Type: Memo, RequiredLevel: None, MaxLength: 8192</summary>
        public const string AdditionalParameters = "activityadditionalparams";
        /// <summary>Type: PartyList (Logical), RequiredLevel: ApplicationRequired, Targets: account,contact,lead,systemuser</summary>
        public const string CallFrom = "from";
        /// <summary>Type: PartyList (Logical), RequiredLevel: ApplicationRequired, Targets: account,contact,lead,systemuser</summary>
        public const string CallTo = "to";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 250, Format: Text</summary>
        public const string Category = "category";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: systemuser</summary>
        public const string CreatedBy = "createdby";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateAndTime, DateTimeBehavior: UserLocal</summary>
        public const string CreatedOn = "createdon";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: transactioncurrency</summary>
        public const string Currency = "transactioncurrencyid";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: msdyncrm_customerjourneyiteration</summary>
        public const string Customerjourneyiteration = "msdyncrm_associatedcustomerjourneyiteration";
        /// <summary>Type: Memo, RequiredLevel: None, MaxLength: 2000</summary>
        public const string Description = "description";
        /// <summary>Type: Boolean, RequiredLevel: None, True: 1, False: 0, DefaultValue: True</summary>
        public const string Direction = "directioncode";
        /// <summary>Type: Virtual (Logical), RequiredLevel: None</summary>
        public const string directioncodename = "directioncodename";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateAndTime, DateTimeBehavior: UserLocal</summary>
        public const string Due = "scheduledend";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: 0, MaxValue: 2147483647</summary>
        public const string Duration = "actualdurationminutes";
        /// <summary>Type: Decimal, RequiredLevel: None, MinValue: 0.0000000001, MaxValue: 100000000000, Precision: 10</summary>
        public const string ExchangeRate = "exchangerate";
        /// <summary>Type: Boolean, RequiredLevel: None, True: 1, False: 0, DefaultValue: False</summary>
        public const string IsBilled = "isbilled";
        /// <summary>Type: Boolean, RequiredLevel: SystemRequired, True: 1, False: 0, DefaultValue: False</summary>
        public const string IsRegularActivity = "isregularactivity";
        /// <summary>Type: Boolean, RequiredLevel: None, True: 1, False: 0, DefaultValue: False</summary>
        public const string IsWorkflowCreated = "isworkflowcreated";
        /// <summary>Type: Virtual (Logical), RequiredLevel: None</summary>
        public const string isbilledname = "isbilledname";
        /// <summary>Type: Virtual (Logical), RequiredLevel: None</summary>
        public const string isregularactivityname = "isregularactivityname";
        /// <summary>Type: Virtual (Logical), RequiredLevel: None</summary>
        public const string isworkflowcreatedname = "isworkflowcreatedname";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateAndTime, DateTimeBehavior: UserLocal</summary>
        public const string LastOnHoldTime = "lastonholdtime";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: sla</summary>
        public const string LastSLAapplied = "slainvokedid";
        /// <summary>Type: Boolean, RequiredLevel: None, True: 1, False: 0, DefaultValue: False</summary>
        public const string LeftVoiceMail = "leftvoicemail";
        /// <summary>Type: Virtual (Logical), RequiredLevel: None</summary>
        public const string leftvoicemailname = "leftvoicemailname";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: systemuser</summary>
        public const string ModifiedBy = "modifiedby";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateAndTime, DateTimeBehavior: UserLocal</summary>
        public const string ModifiedOn = "modifiedon";
        /// <summary>Type: String (Logical), RequiredLevel: None, MaxLength: 256, Format: Text</summary>
        public const string msdyncrm_associatedcustomerjourneyiterationname = "msdyncrm_associatedcustomerjourneyiterationname";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: -2147483648, MaxValue: 2147483647</summary>
        public const string OnHoldTimeMinutes = "onholdtime";
        /// <summary>Type: Owner, RequiredLevel: SystemRequired, Targets: systemuser,team</summary>
        public const string Owner = "ownerid";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 200, Format: Text</summary>
        public const string PhoneNumber = "phonenumber";
        /// <summary>Type: Picklist, RequiredLevel: None, DisplayName: Priority, OptionSetType: Picklist, DefaultFormValue: 1</summary>
        public const string Priority = "prioritycode";
        /// <summary>Type: Virtual (Logical), RequiredLevel: None</summary>
        public const string prioritycodename = "prioritycodename";
        /// <summary>Type: Uniqueidentifier, RequiredLevel: None</summary>
        public const string Process = "processid";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: account,bookableresourcebooking,bookableresourcebookingheader,bulkoperation,campaign,campaignactivity,contact,contract,entitlement,entitlementtemplate,incident,invoice,knowledgearticle,knowledgebaserecord,lead,msdyncrm_contentsettings,msdyncrm_customerjourney,msdyncrm_leadscoremodel,msdyncrm_linkedinaccount,msdyncrm_linkedinactivity,msdyncrm_linkedinfieldmapping,msdyncrm_linkedinform,msdyncrm_linkedinformanswer,msdyncrm_linkedinformquestion,msdyncrm_linkedinformsubmission,msdyncrm_linkedinleadmatchingstrategy,msdyncrm_linkedinuserprofile,msdyncrm_marketingdynamiccontentmetadata,msdyncrm_marketingemaildynamiccontentmetadata,msdyncrm_marketingemailtestsend,msdyncrm_migration,msdyncrm_uicconfig,msdyn_customerasset,msdyn_playbookinstance,msdyn_postalbum,msdyn_question,msdyn_responseaction,msdyn_responseerror,msdyn_survey,msdyn_surveyresponse,msevtmgt_checkin,msevtmgt_event,msevtmgt_eventpurchase,msevtmgt_eventpurchaseattendee,msevtmgt_eventpurchasepass,msevtmgt_eventregistration,msevtmgt_hotel,msevtmgt_hotelroomallocation,msevtmgt_hotelroomreservation,msevtmgt_layout,msevtmgt_room,msevtmgt_session,msevtmgt_sessionregistration,msevtmgt_sessiontrack,msevtmgt_speaker,msevtmgt_speakerengagement,msevtmgt_sponsorablearticle,msevtmgt_sponsorship,msevtmgt_venue,msevtmgt_webinarconfiguration,msevtmgt_webinarprovider,opportunity,quote,salesorder,site</summary>
        public const string Regarding = "regardingobjectid";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: 0, MaxValue: 2147483647</summary>
        public const string ScheduledDuration = "scheduleddurationminutes";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: service</summary>
        public const string Service = "serviceid";
        /// <summary>Type: String (Logical), RequiredLevel: None, MaxLength: 160, Format: Text</summary>
        public const string serviceidname = "serviceidname";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: sla</summary>
        public const string SLA = "slaid";
        /// <summary>Type: String (Logical), RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string slainvokedidname = "slainvokedidname";
        /// <summary>Type: String (Logical), RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string slaname = "slaname";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateAndTime, DateTimeBehavior: UserLocal</summary>
        public const string SortDate = "sortdate";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateAndTime, DateTimeBehavior: UserLocal</summary>
        public const string StartDate = "scheduledstart";
        /// <summary>Type: Virtual (Logical), RequiredLevel: None</summary>
        public const string statecodename = "statecodename";
        /// <summary>Type: Status, RequiredLevel: None, DisplayName: Status Reason, OptionSetType: Status</summary>
        public const string StatusReason = "statuscode";
        /// <summary>Type: Virtual (Logical), RequiredLevel: None</summary>
        public const string statuscodename = "statuscodename";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 250, Format: Text</summary>
        public const string Sub_Category = "subcategory";
        /// <summary>Type: Uniqueidentifier, RequiredLevel: None</summary>
        public const string Subscription = "subscriptionid";

        #endregion Attributes

        #region OptionSets

        public enum ActivityStatus_OptionSet
        {
            Open = 0,
            Completed = 1,
            Canceled = 2
        }
        public enum ActivityType_OptionSet
        {
            Fax = 4204,
            PhoneCall = 4210,
            Email = 4202,
            Letter = 4207,
            Appointment = 4201,
            Task = 4212,
            RecurringAppointment = 4251,
            QuickCampaign = 4406,
            CampaignActivity = 4402,
            CampaignResponse = 4401,
            CaseResolution = 4206,
            ServiceActivity = 4214,
            OpportunityClose = 4208,
            OrderClose = 4209,
            QuoteClose = 4211,
            FormsProsurveyinvite = 10049,
            FormsProsurveyresponse = 10050,
            SurveyActivity = 10094,
            AdobeCampaignEmailBounce = 10264,
            AdobeCampaignEmailURLClick = 10265,
            AdobeCampaignEmailOpen = 10266,
            AdobeCampaignEmailSend = 10267
        }
        public enum Priority_OptionSet
        {
            Low = 0,
            Normal = 1,
            High = 2
        }
        public enum StatusReason_OptionSet
        {
            Open = 1,
            Made = 2,
            Canceled = 3,
            Received = 4
        }

        #endregion OptionSets
    }
}
