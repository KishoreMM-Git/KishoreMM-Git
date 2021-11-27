// *********************************************************************
// Created by: Latebound Constant Generator 1.2019.6.2 for XrmToolBox
// Author    : Jonas Rapp http://twitter.com/rappen
// Repo      : https://github.com/rappen/LateboundConstantGenerator
// Source Org: https://mmdev.crm.dynamics.com/
// Filename  : C:\Users\user\Desktop\Display Name\LoanStatusMapping.cs
// Created   : 2019-11-05 15:36:56
// *********************************************************************

namespace XRMExtensions
{
    public static class LoanStatusMapping
    {
        public const string EntityName = "ims_loanstatusmapping";
        public const string EntityCollectionName = "ims_loanstatusmappings";
        public const string PrimaryKey = "ims_loanstatusmappingid";
        public const string PrimaryName = "ims_name";
        public const string CreatedBy = "createdby";
        public const string CreatedByDelegate = "createdonbehalfby";
        public const string CreatedOn = "createdon";
        public const string CRMLoanStatus = "ims_crmloanstatus";
        public const string CRMLoanStatusReason = "ims_crmloanstatusreason";
        public const string CRMLeadStatus = "ims_crmleadstatus";
        public const string CRMLeadStatusReason = "ims_crmleadstatusreason";
        public const string ContactType = "ims_contacttype";
        public const string ImportSequenceNumber = "importsequencenumber";
        public const string LeadStatus = "ims_leadstatus";
        public const string LoanStatus = "ims_loanstatus";
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
        public enum CRMLoanStatus_OptionSet
        {
            Open = 0,
            Won = 1,
            Lost = 2
        }
        public enum CRMLoanStatusReason_OptionSet
        {
            InProgress = 1,
            OnHold = 2,
            Won = 3,
            Canceled = 4,
            Out_Sold = 5
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
