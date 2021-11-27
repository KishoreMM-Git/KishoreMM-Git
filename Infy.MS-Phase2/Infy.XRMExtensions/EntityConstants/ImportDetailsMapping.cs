// *********************************************************************
// Created by: Latebound Constant Generator 1.2019.6.2 for XrmToolBox
// Author    : Jonas Rapp http://twitter.com/rappen
// Repo      : https://github.com/rappen/LateboundConstantGenerator
// Source Org: https://mmdev.crm.dynamics.com/
// Filename  : C:\Users\venkata.mungamuri\Desktop\Constants\ImportDetailsMapping.cs
// Created   : 2019-11-18 12:39:40
// *********************************************************************

namespace XRMExtensions
{
    public static class ImportDetailsMapping
    {
        public const string EntityName = "ims_importdetailsmapping";
        public const string EntityCollectionName = "ims_importdetailsmappings";
        public const string PrimaryKey = "ims_importdetailsmappingid";
        public const string PrimaryName = "ims_name";
        public const string CreateLookupRecordUnresolved = "ims_createlookuprecord";
        public const string CreatedBy = "createdby";
        public const string CreatedByDelegate = "createdonbehalfby";
        public const string CreatedOn = "createdon";
        public const string CrmDisplayName = "ims_crmdisplayname";
        public const string DataMasterName = "ims_datamastername";
        public const string DefaultValue = "ims_defaultvalue";
        public const string ImportDataMaster = "ims_importdatamaster";
        public const string ImportSequenceNumber = "importsequencenumber";
        public const string IsDataMandatoryforallrecords = "ims_mandatory";
        public const string LookupentityAttributeFilterCondition = "ims_lookupentityattribute";
        public const string LookupEntityName = "ims_lookupentityname";
        public const string MaximumLengthAllowed = "ims_maxlength";
        public const string ModifiedByDelegate = "modifiedonbehalfby";
        public const string ModifiedBy = "modifiedby";
        public const string ModifiedOn = "modifiedon";
        public const string Owner = "ownerid";
        public const string owneridtype = "owneridtype";
        public const string OwningBusinessUnit = "owningbusinessunit";
        public const string RecordCreatedOn = "overriddencreatedon";
        public const string RevertResult = "ims_revertresult";
        public const string SourceDataType = "ims_sourcedatatype";
        public const string SourceEntity = "ims_sourceentity";
        public const string SourceField = "ims_sourcefield";
        public const string Status = "statecode";
        public const string StatusReason = "statuscode";
        public const string TargetDataType = "ims_targetdatatype";
        public const string TargetField = "ims_targetfield";
        public const string TimeZoneRuleVersionNumber = "timezoneruleversionnumber";
        public const string UTCConversionTimeZoneCode = "utcconversiontimezonecode";
        public const string VersionNumber = "versionnumber";
        public enum owneridtype_OptionSet
        {
        }
        public enum SourceDataType_OptionSet
        {
            Currency = 8,
            DateTime = 4,
            Decimal = 9,
            Lookup = 5,
            MultipleLineOfText = 1,
            Optonset = 6,
            SingleLineOfText = 2,
            TwoOptions = 7,
            WholeNumber = 3
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
        public enum TargetDataType_OptionSet
        {
            Currency = 8,
            DateTime = 4,
            Decimal = 9,
            Lookup = 5,
            MultipleLineOfText = 1,
            Optonset = 6,
            SingleLineOfText = 2,
            TwoOptions = 7,
            WholeNumber = 3
        }
    }
}
