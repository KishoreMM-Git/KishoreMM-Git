using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRMExtensions
{
    public static class Task
    {
        public const string EntityName = "task";
        public const string OwnerId = "ownerid";
        public const string RegardingObjectId = "regardingobjectid";
        public const string Subject = "subject";
        public const string ScheduledStart = "scheduledstart";
        public const string Description = "description";
        public const string ScheduledEnd = "scheduledend";
        public const string PriorityCode = "prioritycode";
        public const string ActualDurationMinutes = "actualdurationminutes";
        public const string PrimaryKey = "activityid";
        public const string AutomatedTask = "ims_automatedtask";
        public const string StateCode = "statecode";
        public const string BlendApplicationGuid = "ims_blendapplicationguid";
        public const string BlendLoanApplicationID = "ims_blendloanapplicationid";
        public const string Category = "ims_category";

        public enum PriorityCode_OptionSet
        {
            Low = 0,
            Normal = 1,
            High = 2
        }
    }
}
