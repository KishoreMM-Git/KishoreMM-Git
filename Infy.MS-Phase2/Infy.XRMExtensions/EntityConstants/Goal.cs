// *********************************************************************
// Created by: Latebound Constant Generator 1.2018.9.2 for XrmToolBox
// Author    : Jonas Rapp http://twitter.com/rappen
// Repo      : https://github.com/rappen/LateboundConstantGenerator
// Source Org: https://mmdev.crm.dynamics.com/
// Filename  : C:\Users\amit.vaidya01\Downloads\Contants\Goal.cs
// Created   : 2019-10-23 14:12:31
// *********************************************************************

namespace XRMExtensions
{
    public static class Goal
    {
        public const string EntityName = "goal";
        public const string PrimaryKey = "goalid";
        public const string PrimaryName = "title";
        public const string Actual = "actualstring";
        public const string ActualDecimal = "actualdecimal";
        public const string ActualInteger = "actualinteger";
        public const string ActualMoney = "actualmoney";
        public const string ActualMoneyBase = "actualmoney_base";
        public const string CreatedBy = "createdby";
        public const string CreatedByDelegate = "createdonbehalfby";
        public const string CreatedOn = "createdon";
        public const string Currency = "transactioncurrencyid";
        public const string CustomRollupField = "customrollupfieldstring";
        public const string CustomRollupFieldDecimal = "customrollupfielddecimal";
        public const string CustomRollupFieldInteger = "customrollupfieldinteger";
        public const string CustomRollupFieldMoneyBase = "customrollupfieldmoney_base";
        public const string CustomRollupFieldMoney = "customrollupfieldmoney";
        public const string Depth = "depth";
        public const string EntityImageId = "entityimageid";
        public const string ExchangeRate = "exchangerate";
        public const string FiscalPeriod = "fiscalperiod";
        public const string FiscalYear = "fiscalyear";
        public const string From = "goalstartdate";
        public const string GoalMetric = "metricid";
        public const string GoalOwner = "goalownerid";
        public const string GoalOwnerType = "goalowneridtype";
        public const string GoalPeriodType = "isfiscalperiodgoal";
        public const string GoalWithError = "goalwitherrorid";
        public const string goalowneridname = "goalowneridname";
        public const string goalowneridyominame = "goalowneridyominame";
        public const string ImportSequenceNumber = "importsequencenumber";
        public const string In_progressDecimal = "inprogressdecimal";
        public const string In_Progress = "inprogressstring";
        public const string In_progressInteger = "inprogressinteger";
        public const string In_progressMoneyBase = "inprogressmoney_base";
        public const string In_progressMoney = "inprogressmoney";
        public const string LastRolledUpDate = "lastrolledupdate";
        public const string Manager = "ownerid";
        public const string ModifiedByDelegate = "modifiedonbehalfby";
        public const string ModifiedBy = "modifiedby";
        public const string ModifiedOn = "modifiedon";
        public const string Overridden = "isoverridden";
        public const string Override = "isoverride";
        public const string owneridtype = "owneridtype";
        public const string OwningBusinessUnit = "owningbusinessunit";
        public const string ParentGoal = "parentgoalid";
        public const string PercentageAchieved = "percentage";
        public const string RecordCreatedOn = "overriddencreatedon";
        public const string RecordSetforRollup = "consideronlygoalownersrecords";
        public const string RollUpOnlyfromChildGoals = "rolluponlyfromchildgoals";
        public const string RollupErrorCode = "rolluperrorcode";
        public const string RollupQuery_ActualDecimal = "rollupqueryactualdecimalid";
        public const string RollupQuery_ActualInteger = "rollupqueryactualintegerid";
        public const string RollupQuery_ActualMoney = "rollupqueryactualmoneyid";
        public const string RollupQuery_CustomRollupFieldDecimal = "rollupquerycustomdecimalid";
        public const string RollupQuery_CustomRollupFieldInteger = "rollupquerycustomintegerid";
        public const string RollupQuery_CustomRollupFieldMoney = "rollupquerycustommoneyid";
        public const string RollupQuery_In_progressDecimal = "rollupqueryinprogressdecimalid";
        public const string RollupQuery_In_progressInteger = "rollupqueryinprogressintegerid";
        public const string RollupQuery_In_progressMoney = "rollupqueryinprogressmoneyid";
        public const string Status = "statecode";
        public const string StatusReason = "statuscode";
        public const string StretchTargetDecimal = "stretchtargetdecimal";
        public const string StretchTargetInteger = "stretchtargetinteger";
        public const string StretchTargetMoneyBase = "stretchtargetmoney_base";
        public const string StretchTargetMoney = "stretchtargetmoney";
        public const string StretchedTarget = "stretchtargetstring";
        public const string TargetDecimal = "targetdecimal";
        public const string TargetInteger = "targetinteger";
        public const string TargetMoneyBase = "targetmoney_base";
        public const string TargetMoney = "targetmoney";
        public const string Target = "targetstring";
        public const string TimeZoneRuleVersionNumber = "timezoneruleversionnumber";
        public const string To = "goalenddate";
        public const string TodaysTargetDecimal = "computedtargetasoftodaydecimal";
        public const string TodaysTargetInteger = "computedtargetasoftodayinteger";
        public const string TodaysTargetMoneyBase = "computedtargetasoftodaymoney_base";
        public const string TodaysTargetMoney = "computedtargetasoftodaymoney";
        public const string TodaysTargetPercentageAchieved = "computedtargetasoftodaypercentageachieved";
        public const string TreeID = "treeid";
        public const string UTCConversionTimeZoneCode = "utcconversiontimezonecode";
        public const string VersionNumber = "versionnumber";
        public enum FiscalPeriod_OptionSet
        {
            Quarter1 = 1,
            Quarter2 = 2,
            Quarter3 = 3,
            Quarter4 = 4,
            January = 101,
            February = 102,
            March = 103,
            April = 104,
            May = 105,
            June = 106,
            July = 107,
            August = 108,
            September = 109,
            October = 110,
            November = 111,
            December = 112,
            Semester1 = 201,
            Semester2 = 202,
            Annual = 301,
            P1 = 401,
            P2 = 402,
            P3 = 403,
            P4 = 404,
            P5 = 405,
            P6 = 406,
            P7 = 407,
            P8 = 408,
            P9 = 409,
            P10 = 410,
            P11 = 411,
            P12 = 412,
            P13 = 413
        }
        public enum FiscalYear_OptionSet
        {
            FY2038 = 2038,
            FY2037 = 2037,
            FY2036 = 2036,
            FY2035 = 2035,
            FY2034 = 2034,
            FY2033 = 2033,
            FY2032 = 2032,
            FY2031 = 2031,
            FY2030 = 2030,
            FY2029 = 2029,
            FY2028 = 2028,
            FY2027 = 2027,
            FY2026 = 2026,
            FY2025 = 2025,
            FY2024 = 2024,
            FY2023 = 2023,
            FY2022 = 2022,
            FY2021 = 2021,
            FY2020 = 2020,
            FY2019 = 2019,
            FY2018 = 2018,
            FY2017 = 2017,
            FY2016 = 2016,
            FY2015 = 2015,
            FY2014 = 2014,
            FY2013 = 2013,
            FY2012 = 2012,
            FY2011 = 2011,
            FY2010 = 2010,
            FY2009 = 2009,
            FY2008 = 2008,
            FY2007 = 2007,
            FY2006 = 2006,
            FY2005 = 2005,
            FY2004 = 2004,
            FY2003 = 2003,
            FY2002 = 2002,
            FY2001 = 2001,
            FY2000 = 2000,
            FY1999 = 1999,
            FY1998 = 1998,
            FY1997 = 1997,
            FY1996 = 1996,
            FY1995 = 1995,
            FY1994 = 1994,
            FY1993 = 1993,
            FY1992 = 1992,
            FY1991 = 1991,
            FY1990 = 1990,
            FY1989 = 1989,
            FY1988 = 1988,
            FY1987 = 1987,
            FY1986 = 1986,
            FY1985 = 1985,
            FY1984 = 1984,
            FY1983 = 1983,
            FY1982 = 1982,
            FY1981 = 1981,
            FY1980 = 1980,
            FY1979 = 1979,
            FY1978 = 1978,
            FY1977 = 1977,
            FY1976 = 1976,
            FY1975 = 1975,
            FY1974 = 1974,
            FY1973 = 1973,
            FY1972 = 1972,
            FY1971 = 1971,
            FY1970 = 1970
        }
        public enum GoalOwnerType_OptionSet
        {
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
            Open = 0,
            Closed = 1,
            Discarded = 2
        }
    }
}
