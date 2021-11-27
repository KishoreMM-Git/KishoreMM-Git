// *********************************************************************
// Created by: Latebound Constant Generator 1.2018.9.2 for XrmToolBox
// Author    : Jonas Rapp http://twitter.com/rappen
// Repo      : https://github.com/rappen/LateboundConstantGenerator
// Source Org: https://mmdev.crm.dynamics.com/
// Filename  : C:\Users\amit.vaidya01\Downloads\Contants\Configuration.cs
// Created   : 2019-10-23 14:12:31
// *********************************************************************

namespace XRMExtensions
{
    public static class Configuration
    {
        public const string EntityName = "ims_configuration";
        public const string PrimaryKey = "ims_configurationid";
        public const string PrimaryName = "ims_name";
        public const string AppConfigSetup = "ims_appconfigsetup";
        public const string CreatedBy = "createdby";
        public const string CreatedByDelegate = "createdonbehalfby";
        public const string CreatedOn = "createdon";
        public const string Description = "ims_description";
        public const string Enabled = "ims_enabled";
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
        public const string Value = "ims_value";
        public const string ValueMultiline = "ims_valuemultiline";
        public const string ValueType = "ims_valuetype";
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
        public enum ValueType_OptionSet
        {
            Static = 176390000,
            Dynamic = 176390001
        }
    }
}
