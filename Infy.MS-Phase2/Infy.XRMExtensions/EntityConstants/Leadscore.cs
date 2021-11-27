// *********************************************************************
// Created by: Latebound Constant Generator 1.2018.9.2 for XrmToolBox
// Author    : Jonas Rapp http://twitter.com/rappen
// Repo      : https://github.com/rappen/LateboundConstantGenerator
// Source Org: https://mmdev.crm.dynamics.com/
// Filename  : C:\Users\amit.vaidya01\Downloads\Contants\Leadscore.cs
// Created   : 2019-10-23 14:12:31
// *********************************************************************

namespace XRMExtensions
{
    public static class Leadscore
    {
        public const string EntityName = "msdyncrm_leadscore_v2";
        public const string PrimaryKey = "msdyncrm_leadscore_v2id";
        public const string PrimaryName = "msdyncrm_name";
        public const string Createdby = "createdby";
        public const string Createdbydelegate = "createdonbehalfby";
        public const string Createdon = "createdon";
        public const string Grade = "msdyncrm_grade";
        public const string Importsequencenumber = "importsequencenumber";
        public const string Lead = "msdyncrm_lead";
        public const string Leadscoringmodel = "msdyncrm_leadscoremodel";
        public const string Modelversion = "msdyncrm_modelversion";
        public const string Modifiedbydelegate = "modifiedonbehalfby";
        public const string Modifiedby = "modifiedby";
        public const string Modifiedon = "modifiedon";
        public const string Owner = "ownerid";
        public const string owneridtype = "owneridtype";
        public const string Owningbusinessunit = "owningbusinessunit";
        public const string Recordcreatedon = "overriddencreatedon";
        public const string Score = "msdyncrm_score";
        public const string Scorestatus = "msdyncrm_scorestatus";
        public const string Status = "statecode";
        public const string Statusreason = "statuscode";
        public const string Timezoneruleversionnumber = "timezoneruleversionnumber";
        public const string UTCconversiontimezonecode = "utcconversiontimezonecode";
        public const string VersionNumber = "versionnumber";
        public enum owneridtype_OptionSet
        {
        }
        public enum Scorestatus_OptionSet
        {
            Obsolete = 533240000,
            Inprogress = 533240001,
            Uptodate = 533240002,
            Error = 533240003
        }
        public enum Status_OptionSet
        {
            Active = 0,
            Inactive = 1
        }
        public enum Statusreason_OptionSet
        {
            Active = 1,
            Inactive = 2
        }
    }
}
