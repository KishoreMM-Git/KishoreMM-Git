﻿// *********************************************************************
// Created by: Latebound Constant Generator 1.2019.6.2 for XrmToolBox
// Author    : Jonas Rapp http://twitter.com/rappen
// Repo      : https://github.com/rappen/LateboundConstantGenerator
// Source Org: https://mmdev.crm.dynamics.com/
// Filename  : C:\Users\user\Desktop\Display Name\LeadStatus.cs
// Created   : 2019-11-05 16:14:17
// *********************************************************************

namespace XRMExtensions
{
    public static class LeadStatus
    {
        public const string EntityName = "ims_leadstatus";
        public const string EntityCollectionName = "ims_leadstatuses";
        public const string PrimaryKey = "ims_leadstatusid";
        public const string PrimaryName = "ims_name";
        public const string CreatedBy = "createdby";
        public const string ContactType="ims_contacttype";
        public const string CreatedByDelegate = "createdonbehalfby";
        public const string CreatedOn = "createdon";
        public const string createdbyname = "createdbyname";
        public const string createdbyyominame = "createdbyyominame";
        public const string createdonbehalfbyname = "createdonbehalfbyname";
        public const string createdonbehalfbyyominame = "createdonbehalfbyyominame";
        public const string ImportSequenceNumber = "importsequencenumber";
        public const string ModifiedByDelegate = "modifiedonbehalfby";
        public const string ModifiedBy = "modifiedby";
        public const string ModifiedOn = "modifiedon";
        public const string modifiedbyname = "modifiedbyname";
        public const string modifiedbyyominame = "modifiedbyyominame";
        public const string modifiedonbehalfbyname = "modifiedonbehalfbyname";
        public const string modifiedonbehalfbyyominame = "modifiedonbehalfbyyominame";
        public const string Owner = "ownerid";
        public const string owneridname = "owneridname";
        public const string owneridtype = "owneridtype";
        public const string owneridyominame = "owneridyominame";
        public const string OwningBusinessUnit = "owningbusinessunit";
        public const string OwningTeam = "owningteam";
        public const string OwningUser = "owninguser";
        public const string RecordCreatedOn = "overriddencreatedon";
        public const string statecodename = "statecodename";
        public const string Status = "statecode";
        public const string StatusReason = "statuscode";
        public const string Scope = "ims_scope";
        public const string statuscodename = "statuscodename";
        public const string TimeZoneRuleVersionNumber = "timezoneruleversionnumber";
        public const string UTCConversionTimeZoneCode = "utcconversiontimezonecode";
        public const string VersionNumber = "versionnumber";
        public const string LeadStatusOptions = "ims_leadstatusoptions";
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
        public enum Scope_OptionSet
        {
            All = 176390000,
            MDOnly = 176390001
        }
    }
}
