// *********************************************************************
// Created by: Latebound Constant Generator 1.2018.9.2 for XrmToolBox
// Author    : Jonas Rapp http://twitter.com/rappen
// Repo      : https://github.com/rappen/LateboundConstantGenerator
// Source Org: https://mmdev.crm.dynamics.com/
// Filename  : C:\Users\amit.vaidya01\Downloads\Contants\LeadScoringModel.cs
// Created   : 2019-10-23 14:12:31
// *********************************************************************

namespace XRMExtensions
{
    public static class LeadScoringModel
    {
        public const string EntityName = "msdyncrm_leadscoremodel";
        public const string PrimaryKey = "msdyncrm_leadscoremodelid";
        public const string PrimaryName = "msdyncrm_name";
        public const string Createdby = "createdby";
        public const string Createdbydelegate = "createdonbehalfby";
        public const string Createdon = "createdon";
        public const string Description = "msdyncrm_description";
        public const string Entitytarget = "msdyncrm_entitytarget";
        public const string Importsequencenumber = "importsequencenumber";
        public const string Insights = "msdyncrm_insights_placeholder";
        public const string Minimumconsent = "msgdpr_requiredconsent";
        public const string Modeldefinition = "msdyncrm_modeldefinition";
        public const string Modifiedbydelegate = "modifiedonbehalfby";
        public const string Modifiedby = "modifiedby";
        public const string Modifiedon = "modifiedon";
        public const string Owner = "ownerid";
        public const string owneridtype = "owneridtype";
        public const string Owningbusinessunit = "owningbusinessunit";
        public const string Recordcreatedon = "overriddencreatedon";
        public const string Status = "statecode";
        public const string Statusreason = "statuscode";
        public const string Time_zoneruleversionnumber = "timezoneruleversionnumber";
        public const string UTCconversiontime_zonecode = "utcconversiontimezonecode";
        public const string VersionNumber = "versionnumber";
        public enum Entitytarget_OptionSet
        {
            Contact = 0,
            Account = 1
        }
        public enum Minimumconsent_OptionSet
        {
            _1Consent = 587030001,
            _2Transactional = 587030002,
            _3Subscriptions = 587030003,
            _4Marketing = 587030004,
            _5Profiling = 587030005
        }
        public enum owneridtype_OptionSet
        {
        }
        public enum Status_OptionSet
        {
            Active = 0,
            Inactive = 1
        }
        public enum Statusreason_OptionSet
        {
            Draft = 192350000,
            Error = 192350010,
            Goinglive = 192350011,
            Live = 192350001,
            Stopping = 192350012,
            Expired = 192350004
        }
    }
}
