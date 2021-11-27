// *********************************************************************
// Created by: Latebound Constant Generator 1.2018.9.2 for XrmToolBox
// Author    : Jonas Rapp http://twitter.com/rappen
// Repo      : https://github.com/rappen/LateboundConstantGenerator
// Source Org: https://mmdev.crm.dynamics.com/
// Filename  : C:\Users\amit.vaidya01\Downloads\Contants\Campaign.cs
// Created   : 2019-10-23 14:12:31
// *********************************************************************

namespace XRMExtensions
{
    public static class Campaign
    {
        public const string EntityName = "campaign";
        public const string PrimaryKey = "campaignid";
        public const string PrimaryName = "name";
        public const string DeprecatedStageId = "stageid";
        public const string DeprecatedTraversedPath = "traversedpath";
        public const string ActualEndDate = "actualend";
        public const string ActualStartDate = "actualstart";
        public const string BudgetAllocatedBase = "budgetedcost_base";
        public const string BudgetAllocated = "budgetedcost";
        public const string CampaignCode = "codename";
        public const string CampaignType = "typecode";
        public const string CreatedBy = "createdby";
        public const string CreatedByDelegate = "createdonbehalfby";
        public const string CreatedOn = "createdon";
        public const string Currency = "transactioncurrencyid";
        public const string Description = "description";
        public const string EmailAddress = "emailaddress";
        public const string entityimageid = "entityimageid";
        public const string EstimatedRevenueBase = "expectedrevenue_base";
        public const string EstimatedRevenue = "expectedrevenue";
        public const string ExchangeRate = "exchangerate";
        public const string ExpectedResponsePercentage = "expectedresponse";
        public const string ImportSequenceNumber = "importsequencenumber";
        public const string Lead_Campaign = "ims_lead_campaignid";
        public const string Message = "message";
        public const string MiscellaneousCostsBase = "othercost_base";
        public const string MiscellaneousCosts = "othercost";
        public const string ModifiedByDelegate = "modifiedonbehalfby";
        public const string ModifiedBy = "modifiedby";
        public const string ModifiedOn = "modifiedon";
        public const string Offer = "objective";
        public const string Owner = "ownerid";
        public const string owneridtype = "owneridtype";
        public const string OwningBusinessUnit = "owningbusinessunit";
        public const string PriceList = "pricelistid";
        public const string ProcessId = "processid";
        public const string PromotionCode = "promotioncodename";
        public const string ProposedEndDate = "proposedend";
        public const string ProposedStartDate = "proposedstart";
        public const string RecordCreatedOn = "overriddencreatedon";
        public const string Status = "statecode";
        public const string StatusReason = "statuscode";
        public const string Template = "istemplate";
        public const string TimeZoneRuleVersionNumber = "timezoneruleversionnumber";
        public const string TmpRegardingObjectId = "tmpregardingobjectid";
        public const string TotalCostofCampaignBase = "totalactualcost_base";
        public const string TotalCostofCampaign = "totalactualcost";
        public const string TotalCostofCampaignActivitiesBase = "totalcampaignactivityactualcost_base";
        public const string TotalCostofCampaignActivities = "totalcampaignactivityactualcost";
        public const string UTCConversionTimeZoneCode = "utcconversiontimezonecode";
        public const string VersionNumber = "versionnumber";
        public enum CampaignType_OptionSet
        {
            Advertisement = 1,
            DirectMarketing = 2,
            Event = 3,
            CoBranding = 4,
            Other = 5
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
            Proposed = 0,
            ReadyToLaunch = 1,
            Launched = 2,
            Completed = 3,
            Canceled = 4,
            Suspended = 5,
            Inactive = 6
        }
    }
}
