// *********************************************************************
// Created by: Latebound Constant Generator 1.2018.9.2 for XrmToolBox
// Author    : Jonas Rapp http://twitter.com/rappen
// Repo      : https://github.com/rappen/LateboundConstantGenerator
// Source Org: https://mmdev.crm.dynamics.com/
// Filename  : C:\Users\amit.vaidya01\Downloads\Contants\LeadScoringConfiguration.cs
// Created   : 2019-10-23 14:12:31
// *********************************************************************

namespace XRMExtensions
{
    public static class LeadScoringConfiguration
    {
        public const string EntityName = "msdyncrm_leadscoringconfiguration";
        public const string PrimaryKey = "msdyncrm_leadscoringconfigurationid";
        public const string PrimaryName = "msdyncrm_name";
        public const string AutomatedMarketingQualification = "msdyncrm_automaticqualification_enabled";
        public const string AutomaticLeadScoresCleanup = "msdyncrm_automaticleadscorescleanup_enabled";
        public const string AutomaticSalesReady = "msdyncrm_automaticsalesready_enabled";
        public const string CreatedBy = "createdby";
        public const string CreatedByDelegate = "createdonbehalfby";
        public const string CreatedOn = "createdon";
        public const string ImportSequenceNumber = "importsequencenumber";
        public const string ModifiedByDelegate = "modifiedonbehalfby";
        public const string ModifiedBy = "modifiedby";
        public const string ModifiedOn = "modifiedon";
        public const string OrganizationId = "organizationid";
        public const string RecordCreatedOn = "overriddencreatedon";
        public const string Status = "statecode";
        public const string StatusReason = "statuscode";
        public const string TimeZoneRuleVersionNumber = "timezoneruleversionnumber";
        public const string UTCConversionTimeZoneCode = "utcconversiontimezonecode";
        public const string VersionNumber = "versionnumber";
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
    }
}
