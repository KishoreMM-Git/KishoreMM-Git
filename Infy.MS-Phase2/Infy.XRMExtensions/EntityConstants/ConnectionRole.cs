// *********************************************************************
// Created by: Latebound Constant Generator 1.2018.9.2 for XrmToolBox
// Author    : Jonas Rapp http://twitter.com/rappen
// Repo      : https://github.com/rappen/LateboundConstantGenerator
// Source Org: https://mmdev.crm.dynamics.com/
// Filename  : C:\Users\amit.vaidya01\Downloads\Contants\ConnectionRole.cs
// Created   : 2019-10-23 14:12:31
// *********************************************************************

namespace XRMExtensions
{
    public static class ConnectionRole
    {
        public const string EntityName = "connectionrole";
        public const string PrimaryKey = "connectionroleid";
        public const string PrimaryName = "name";
        public const string ComponentState = "componentstate";
        public const string ConnectionRoleCategory = "category";
        public const string CreatedBy = "createdby";
        public const string CreatedByDelegate = "createdonbehalfby";
        public const string CreatedOn = "createdon";
        public const string Customizable = "iscustomizable";
        public const string Description = "description";
        public const string ImportSequenceNumber = "importsequencenumber";
        public const string IntroducedVersion = "introducedversion";
        public const string ModifiedByDelegate = "modifiedonbehalfby";
        public const string ModifiedBy = "modifiedby";
        public const string ModifiedOn = "modifiedon";
        public const string Organization = "organizationid";
        public const string OverwrittenOn = "overwritetime";
        public const string Solution = "solutionid";
        public const string Solution1 = "supportingsolutionid";
        public const string State = "ismanaged";
        public const string Status = "statecode";
        public const string StatusReason = "statuscode";
        public const string UniqueID = "connectionroleidunique";
        public const string VersionNumber = "versionnumber";
        public enum ComponentState_OptionSet
        {
            Published = 0,
            Unpublished = 1,
            Deleted = 2,
            DeletedUnpublished = 3
        }
        public enum ConnectionRoleCategory_OptionSet
        {
            Business = 1,
            Family = 2,
            Social = 3,
            Sales = 4,
            Other = 5,
            Stakeholder = 1000,
            SalesTeam = 1001,
            Service = 1002
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
