// *********************************************************************
// Created by: Latebound Constant Generator 1.2018.9.2 for XrmToolBox
// Author    : Jonas Rapp http://twitter.com/rappen
// Repo      : https://github.com/rappen/LateboundConstantGenerator
// Source Org: https://mmdev.crm.dynamics.com/
// Filename  : C:\Users\amit.vaidya01\Downloads\Marketinglist.cs
// Created   : 2019-11-14 22:28:03
// *********************************************************************

namespace XRMExtensions
{
    public static class Marketinglist
    {
        public const string EntityName = "list";
        public const string PrimaryKey = "listid";
        public const string PrimaryName = "listname";
        public const string DeprecatedStageId = "stageid";
        public const string DeprecatedTraversedPath = "traversedpath";
        public const string CostBase = "cost_base";
        public const string Cost = "cost";
        public const string CreatedBy = "createdby";
        public const string CreatedByDelegate = "createdonbehalfby";
        public const string CreatedOn = "createdon";
        public const string Currency = "transactioncurrencyid";
        public const string Description = "description";
        public const string ExchangeRate = "exchangerate";
        public const string ExcludeMembersWhoOptOut = "donotsendonoptout";
        public const string IgnoreInactiveListMembers = "ignoreinactivelistmembers";
        public const string ImportSequenceNumber = "importsequencenumber";
        public const string IsMLUpdated = "ims_ismlupdated";
        public const string LastUsedOn = "lastusedon";
        public const string Locked = "lockstatus";
        public const string MarketingListMemberType = "createdfromcode";
        public const string MemberType = "membertype";
        public const string MembersCount = "membercount";
        public const string ModifiedByDelegate = "modifiedonbehalfby";
        public const string ModifiedBy = "modifiedby";
        public const string ModifiedOn = "modifiedon";
        public const string Owner = "ownerid";
        public const string owneridtype = "owneridtype";
        public const string OwningBusinessUnit = "owningbusinessunit";
        public const string ProcessId = "processid";
        public const string processedMemberCount = "processedmembercount";
        public const string processFetchXML = "processfetchxml";
        public const string Purpose = "purpose";
        public const string Query = "query";
        public const string RecordCreatedOn = "overriddencreatedon";
        public const string Source = "source";
        public const string Status = "statecode";
        public const string StatusReason = "statuscode";
        public const string Subscription = "msdyncrm_issubscription";
        public const string TimeZoneRuleVersionNumber = "timezoneruleversionnumber";
        public const string Type = "type";
        public const string UTCConversionTimeZoneCode = "utcconversiontimezonecode";
        public const string VersionNumber = "versionnumber";
        public const string Configuration = "ims_configuration";
        public enum MarketingListMemberType_OptionSet
        {
            Account = 1,
            Contact = 2,
            Lead = 4
        }
        public enum Type_OptionSet
        {
            Static = 0,
            Dynamic = 1
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
            Active = 0,
            Inactive = 1
        }
    }
}
