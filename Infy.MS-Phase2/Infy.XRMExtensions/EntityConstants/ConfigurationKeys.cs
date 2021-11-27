using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRMExtensions
{
    public static class ConfigurationKeys
    {
        //Configuration Upcoming Birthday
        public static string Key_UpcomingBirthday_RecurringTrigger = "UpcomingBirthday_Recurring_Trigger";
        public static string Key_UpcomingBirthday_EmailActivity="UpcomingBirthday_EmailActivity";
        public static string DynamicValue_Key_UpcomingBirthday_EmailActivity = "{&&LeadId&&}";
        public static string Key_UpcomingBirthday_NextXDays= "UpcomingBirthday_NextXDays";
        public static string Key_UpcomingBirthday_Yesterday= "UpcomingBirthday_Yestrday";
    }
}
