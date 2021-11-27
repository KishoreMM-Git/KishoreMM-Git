using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRMExtensions
{
    public static class SMS
    {
        public const string EntityName = "ims_sms";
        public const string PrimaryKey = "activityid";
        public const string PrimaryName = "subject";
        public const string ActivityStatus = "statecode";
        public const string RegardingObjectId = "regardingobjectid";
        public const string ModifiedOn = "modifiedon";
        public const string direction = "ims_direction";
        public const string isAutomatedSMS = "ims_isautomatedsms";
        public const string ReasonToText = "ims_reasontotext";
    }
}
