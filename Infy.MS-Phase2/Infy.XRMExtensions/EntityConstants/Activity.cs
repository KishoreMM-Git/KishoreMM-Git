// *********************************************************************
// Created by: Latebound Constant Generator 1.2018.9.2 for XrmToolBox
// Author    : Jonas Rapp http://twitter.com/rappen
// Repo      : https://github.com/rappen/LateboundConstantGenerator
// Source Org: https://mmdev.crm.dynamics.com/
// Filename  : C:\Users\amit.vaidya01\Downloads\Contants\Activity.cs
// Created   : 2019-10-23 14:12:31
// *********************************************************************

namespace XRMExtensions
{
    public static class Activity
    {
        public const string EntityName = "activitypointer";
        public const string PrimaryKey = "activityid";
        public const string PrimaryName = "subject";
        public const string DeprecatedProcessStage = "stageid";
        public const string DeprecatedTraversedPath = "traversedpath";
        public const string ActivityAdditionalParameters = "activityadditionalparams";
        public const string ActivityStatus = "statecode";
        public const string ActivityType = "activitytypecode";
        public const string ActualDuration = "actualdurationminutes";
        public const string ActualEnd = "actualend";
        public const string ActualStart = "actualstart";
        public const string CreatedBy = "createdby";
        public const string CreatedByDelegate = "createdonbehalfby";
        public const string Currency = "transactioncurrencyid";
        public const string DateCreated = "createdon";
        public const string DateDeliveryLastAttempted = "deliverylastattemptedon";
        public const string DateSent = "senton";
        public const string Delayactivityprocessinguntil = "postponeactivityprocessinguntil";
        public const string DeliveryPriority = "deliveryprioritycode";
        public const string Description = "description";
        public const string DueDate = "scheduledend";
        public const string ExchangeItemID = "exchangeitemid";
        public const string ExchangeRate = "exchangerate";
        public const string ExchangeWebLink = "exchangeweblink";
        public const string IsBilled = "isbilled";
        public const string IsPrivate = "ismapiprivate";
        public const string IsRegularActivity = "isregularactivity";
        public const string IsWorkflowCreated = "isworkflowcreated";
        public const string LastOnHoldTime = "lastonholdtime";
        public const string LastSLAapplied = "slainvokedid";
        public const string LastUpdated = "modifiedon";
        public const string LeftVoiceMail = "leftvoicemail";
        public const string ModifiedByDelegate = "modifiedonbehalfby";
        public const string ModifiedBy = "modifiedby";
        public const string OnHoldTimeMinutes = "onholdtime";
        public const string Owner = "ownerid";
        public const string owneridtype = "owneridtype";
        public const string OwningBusinessUnit = "owningbusinessunit";
        public const string Priority = "prioritycode";
        public const string Process = "processid";
        public const string RecurringInstanceType = "instancetypecode";
        public const string Regarding = "regardingobjectid";
        public const string regardingobjectidname = "regardingobjectidname";
        public const string regardingobjectidyominame = "regardingobjectidyominame";
        public const string regardingobjecttypecode = "regardingobjecttypecode";
        public const string ScheduledDuration = "scheduleddurationminutes";
        public const string SendersMailbox = "sendermailboxid";
        public const string SeriesId = "seriesid";
        public const string Service = "serviceid";
        public const string SLA = "slaid";
        public const string SocialChannel = "community";
        public const string SortDate = "sortdate";
        public const string StartDate = "scheduledstart";
        public const string StatusReason = "statuscode";
        public const string TimeZoneRuleVersionNumber = "timezoneruleversionnumber";
        public const string UTCConversionTimeZoneCode = "utcconversiontimezonecode";
        public const string VersionNumber = "versionnumber";
        public enum ActivityStatus_OptionSet
        {
            Open = 0,
            Completed = 1,
            Canceled = 2,
            Scheduled = 3
        }
        public enum ActivityType_OptionSet
        {
            Fax = 4204,
            PhoneCall = 4210,
            EMail = 4202,
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
            SurveyActivity = 10094
        }
        public enum DeliveryPriority_OptionSet
        {
            Low = 0,
            Normal = 1,
            High = 2
        }
        public enum owneridtype_OptionSet
        {
        }
        public enum Priority_OptionSet
        {
            Low = 0,
            Normal = 1,
            High = 2
        }
        public enum RecurringInstanceType_OptionSet
        {
            NotRecurring = 0,
            RecurringMaster = 1,
            RecurringInstance = 2,
            RecurringException = 3,
            RecurringFutureException = 4
        }
        public enum regardingobjecttypecode_OptionSet
        {
        }
        public enum SocialChannel_OptionSet
        {
            Facebook = 1,
            Twitter = 2,
            Other = 0
        }
        public enum StatusReason_OptionSet
        {
            Open = 1,
            Completed = 2,
            Canceled = 3,
            Scheduled = 4
        }
    }
}
