// *********************************************************************
// Created by : Latebound Constants Generator 1.2020.2.1 for XrmToolBox
// Author     : Jonas Rapp https://twitter.com/rappen
// GitHub     : https://github.com/rappen/LCG-UDG
// Source Org : https://mmdevphase2.crm.dynamics.com/
// Filename   : C:\Users\bipin.kumar\Desktop\RoutingRule.cs
// Created    : 2020-04-15 15:29:06
// *********************************************************************

namespace XRMExtensions
{
    /// <summary>DisplayName: Routing Rule, OwnershipType: UserOwned, IntroducedVersion: 1.0.0.0</summary>
    public static class RoutingRule
    {
        public const string EntityName = "ims_routingrule";
        public const string EntityCollectionName = "ims_routingrules";

        #region Attributes

        /// <summary>Type: Uniqueidentifier, RequiredLevel: SystemRequired</summary>
        public const string PrimaryKey = "ims_routingruleid";
        /// <summary>Type: String, RequiredLevel: ApplicationRequired, MaxLength: 100, Format: Text</summary>
        public const string PrimaryName = "ims_name";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: ims_leadstatus</summary>
        public const string AssignedLeadStatus = "ims_assignedleadstatus";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: 1, MaxValue: 2147483647</summary>
        public const string BasePriorityNumber = "ims_baseprioritynumber";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: systemuser</summary>
        public const string CreatedBy = "createdby";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateAndTime, DateTimeBehavior: UserLocal</summary>
        public const string CreatedOn = "createdon";
        /// <summary>Type: Picklist, RequiredLevel: None, DisplayName: Distribution Logic, OptionSetType: Picklist, DefaultFormValue: 176390000</summary>
        public const string DistributionLogic = "ims_distributionlogic";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateOnly, DateTimeBehavior: UserLocal</summary>
        public const string EndDate = "ims_enddate";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string EndTime = "ims_endtime_lable";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string entityList = "ims_entitylist";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string entitylistbase = "ims_entitylist_base";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string Friday = "ims_friday_lable";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string FridayEndTime = "ims_fridayendtime";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string FridayStartTime = "ims_fridaystarttime";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateAndTime, DateTimeBehavior: UserLocal</summary>
        public const string LastDistributedOn = "ims_lastdistributedon";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateAndTime, DateTimeBehavior: UserLocal</summary>
        public const string LastRunOn = "ims_lastrunon";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: list</summary>
        public const string ListMember = "ims_listmemberid";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: systemuser</summary>
        public const string ModifiedBy = "modifiedby";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateAndTime, DateTimeBehavior: UserLocal</summary>
        public const string ModifiedOn = "modifiedon";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string Monday = "ims_monday_lable";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string MondayEndTime = "ims_mondayendtime";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string MondayStartTime = "ims_mondaystarttime";
        /// <summary>Type: Owner, RequiredLevel: SystemRequired, Targets: systemuser,team</summary>
        public const string Owner = "ownerid";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: 1, MaxValue: 2147483647</summary>
        public const string Priority = "ims_priority";
        /// <summary>Type: Memo, RequiredLevel: None, MaxLength: 1048576</summary>
        public const string QueryXML = "ims_queryxml";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: queue</summary>
        public const string Queue = "ims_queue";
        /// <summary>Type: Picklist, RequiredLevel: None, DisplayName: Routing Method, OptionSetType: Picklist, DefaultFormValue: 176390000</summary>
        public const string RoutingMethod = "ims_routingmethod";
        /// <summary>Type: Boolean, RequiredLevel: None, True: 1, False: 0, DefaultValue: False</summary>
        public const string RuleStatus = "ims_rulestatus";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string RuleType = "ims_ruletype";
        /// <summary>Type: Picklist</summary>
        public const string Saturday = "ims_saturday_lable";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string SaturdayEndTime = "ims_saturdayendtime";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string SaturdayStartTime = "ims_saturdaystarttime";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateOnly, DateTimeBehavior: UserLocal</summary>
        public const string StartDate = "ims_startdate";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string StartTime = "ims_starttime_lable";
        /// <summary>Type: State, RequiredLevel: SystemRequired, DisplayName: Status, OptionSetType: State</summary>
        public const string Status = "statecode";
        /// <summary>Type: Status, RequiredLevel: None, DisplayName: Status Reason, OptionSetType: Status</summary>
        public const string StatusReason = "statuscode";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string Sunday = "ims_sunday_lable";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string SundayEndTime = "ims_sundayendtime";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string SundayStartTime = "ims_sundaystarttime";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string Thursday = "ims_thursday_lable";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string ThursdayEndTime = "ims_thursdayendtime";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string ThursdayStartTime = "ims_thursdaystarttime";
        /// <summary>Type: Picklist, RequiredLevel: None, DisplayName: Time Zone, OptionSetType: Picklist, DefaultFormValue: 176390000</summary>
        public const string TimeZone = "ims_timezone";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string Tuesday = "ims_tuesday_lable";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string TuesdayEndTime = "ims_tuesdayendtime";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string TuesdayStartTime = "ims_tuesdaystarttime";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string Wednesday = "ims_wednesday_lable";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string WednesdayEndTime = "ims_wednesdayendtime";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 100, Format: Text</summary>
        public const string WednesdayStartTime = "ims_wednesdaystarttime";

        #endregion Attributes

        #region OptionSets

        public enum RuleType_OptionSet
        {
            Distribution = 176390000,
            Prioritization = 176390001
        }
        public enum DistributionLogic_OptionSet
        {
            RoundRobin = 176390000,
            MaxCapacity = 176390001
        }
        public enum RoutingMethod_OptionSet
        {
            Push = 176390000,
            Pull = 176390001,
            OnScreenNotification = 176390002
        }
        public enum Status_OptionSet
        {
            Active = 0,
            Inactive = 1
        }
        public enum StatusReason_OptionSet
        {
            Active = 1,
            Inactive = 2
        }
        public enum TimeZone_OptionSet
        {
            USA = 176390000
        }

        #endregion OptionSets
    }
}
