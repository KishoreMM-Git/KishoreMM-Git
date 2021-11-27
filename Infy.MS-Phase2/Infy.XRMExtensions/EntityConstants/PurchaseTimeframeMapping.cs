// *********************************************************************
// Created by: Latebound Constant Generator 1.2019.6.2 for XrmToolBox
// Author    : Jonas Rapp http://twitter.com/rappen
// Repo      : https://github.com/rappen/LateboundConstantGenerator
// Source Org: https://mmdev.crm.dynamics.com/
// Filename  : C:\Users\venkata.mungamuri\Desktop\Constants\PurchaseTimeframeMapping.cs
// Created   : 2019-11-19 13:56:21
// *********************************************************************

namespace XRMExtensions
{
    public static class PurchaseTimeframeMapping
    {
        public const string EntityName = "ims_purchasetimeframemapping";
        public const string EntityCollectionName = "ims_purchasetimeframemappings";
        public const string PrimaryKey = "ims_purchasetimeframemappingid";
        public const string PrimaryName = "ims_name";
        public const string CreatedBy = "createdby";
        public const string CreatedByDelegate = "createdonbehalfby";
        public const string CreatedOn = "createdon";
        public const string ImportSequenceNumber = "importsequencenumber";
        public const string LeadPurchaseTimeFrame = "ims_leadpurchasetimeframe";
        public const string ModifiedByDelegate = "modifiedonbehalfby";
        public const string ModifiedBy = "modifiedby";
        public const string ModifiedOn = "modifiedon";
        public const string Owner = "ownerid";
        public const string owneridtype = "owneridtype";
        public const string OwningBusinessUnit = "owningbusinessunit";
        public const string RecordCreatedOn = "overriddencreatedon";
        public const string Status = "statecode";
        public const string StatusReason = "statuscode";
        public const string TimeZoneRuleVersionNumber = "timezoneruleversionnumber";
        public const string UTCConversionTimeZoneCode = "utcconversiontimezonecode";
        public const string VersionNumber = "versionnumber";
        public enum LeadPurchaseTimeFrame_OptionSet
        {
            _0_3Months = 0,
            _4_6Months = 1,
            _7_12Months = 2,
            _12Months_ = 3,
            Unknown = 4
        }
        public enum owneridtype_OptionSet
        {
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
    }
}
