using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infy.MS.NextBestActionActivity
{
    public class Constants
    {
        public const string Authority = "authority";
        public const string ClientId = "clientId";
        public const string Secret = "secret";
        public const string ServiceUrl = "serviceUrl";
        public const string jobFreqency = "JobFrequency";
        public const string configSetUpName = "Activity Requests";
        public const string NBAPreClosingReason = "NBAPreClosingReason";
        public const string configSetUpName_ActivityScheduler = "Activity Scheduler";
        public const string batchJobName = "Infy.MS.NextBestAction";
        public const string ActivityRequestNBATypeFetchXml = "ActivityRequest_NBATypeFetchXml";
        public const string StaticMarketingList_LeadsFetchXml = "StaticMarketingList_LeadsFetchXml";
        public const string CheckStaticMLSynchronization = "CheckStaticMLSynchronization";

        public const string ActivityRequestJob_Started_Message = "ActivityRequestJob_Started_Message";
        public const string ActivityRequestJob_Created_Message = "ActivityRequestJob_Created_Message";
        public const string ActivityRequestMarketingListType_Message_Dynamic = "ActivityRequestMarketingListType_Message_Dynamic";
        public const string ActivityRequestJob_MarketingListLeadCount_Message = "ActivityRequestJob_MarketingListLeadCount_Message";
        public const string ActivityRequestJob_NBAActivityDuplicate_Message = "ActivityRequestJob_NBAActivityDuplicate_Message";
        public const string ActivityRequestJob_LeadInactive_Message = "ActivityRequestJob_LeadInactive_Message";
        public const string ActivityRequest_NBAActivityCreationFailed_ErrorMessage = "ActivityRequest_NBAActivityCreationFailed_ErrorMessage";
        public const string ActivityRequestJob_TotalFailedActivities_Message = "ActivityRequestJob_TotalFailedActivities_Message";
        public const string ActivityRequest_ZeroMarketingMembers_InfoMessage = "ActivityRequest_ZeroMarketingMembers_InfoMessage";


        public const string ActivityScheduler_LeadName = "{LeadName}";
        public const string ActivityScheduler_ContactName = "{ContactName}";










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
    }
}
