// *********************************************************************
// Created by: Latebound Constant Generator 1.2018.9.2 for XrmToolBox
// Author    : Jonas Rapp http://twitter.com/rappen
// Repo      : https://github.com/rappen/LateboundConstantGenerator
// Source Org: https://mmdev.crm.dynamics.com/
// Filename  : C:\Users\amit.vaidya01\Downloads\Contants\Connection.cs
// Created   : 2019-10-23 14:12:31
// *********************************************************************

namespace XRMExtensions
{
    public static class Connection
    {
        public const string EntityName = "connection";
        public const string PrimaryKey = "connectionid";
        public const string Connectedfrom = "record1id";
        public const string Connectedto = "record2id";
        public const string CreatedBy = "createdby";
        public const string CreatedByDelegate = "createdonbehalfby";
        public const string CreatedOn = "createdon";
        public const string Currency = "transactioncurrencyid";
        public const string Description = "description";
        public const string Ending = "effectiveend";
        public const string EntityImageId = "entityimageid";
        public const string ExchangeRate = "exchangerate";
        public const string ImportSequenceNumber = "importsequencenumber";
        public const string IsMaster = "ismaster";
        public const string ModifiedByDelegate = "modifiedonbehalfby";
        public const string ModifiedBy = "modifiedby";
        public const string ModifiedOn = "modifiedon";
        public const string NameFrom = "record1idname";
        public const string NameTo = "record2idname";
        public const string Owner = "ownerid";
        public const string owneridtype = "owneridtype";
        public const string OwningBusinessUnit = "owningbusinessunit";
        public const string ReciprocalConnection = "relatedconnectionid";
        public const string RecordCreatedOn = "overriddencreatedon";
        public const string record1idobjecttypecode = "record1idobjecttypecode";
        public const string record2idobjecttypecode = "record2idobjecttypecode";
        public const string RoleFrom = "record1roleid";
        public const string RoleTo = "record2roleid";
        public const string Starting = "effectivestart";
        public const string Status = "statecode";
        public const string StatusReason = "statuscode";
        public const string TypeFrom = "record1objecttypecode";
        public const string TypeTo = "record2objecttypecode";
        public const string VersionNumber = "versionnumber";
        public enum owneridtype_OptionSet
        {
        }
        public enum record1idobjecttypecode_OptionSet
        {
        }
        public enum record2idobjecttypecode_OptionSet
        {
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
        public enum TypeFrom_OptionSet
        {
            MarketingEMailDynamicContentmetadata = 10181,
            ResourceGroup = 4007,
            Loan = 3,
            Quote = 1084,
            LinkedInleadmatchingstrategy = 10229,
            LeadScoringModel = 10215,
            PriceList = 1022,
            Entitlement = 9700,
            EntitlementTemplateChannel = 9703,
            KnowledgeArticle = 9953,
            Account = 1,
            Order = 1088,
            FacilityEquipment = 4000,
            User = 8,
            KnowledgeBaseRecord = 9930,
            Activity = 4200,
            PhoneCall = 4210,
            LinkedInLeadGenform = 10225,
            Lead = 4,
            LinkedInformsubmissionanswer = 10226,
            LinkedInLeadGenformsubmission = 10228,
            Product = 1024,
            ProcessSession = 4710,
            UICconfig = 10200,
            ResponseError = 10089,
            LinkedInUserProfile = 10230,
            SurveyActivity = 10094,
            SchedulingGroup = 4005,
            LinkedInAccount = 10220,
            Invoice = 1090,
            Campaign = 4400,
            ChannelAccessProfileRule = 9400,
            DynamicContentmetadata = 10179,
            FormsProsurveyresponse = 10050,
            MarketingEMailTestsend = 10183,
            SocialActivity = 4216,
            SocialProfile = 99,
            Territory = 2013,
            EMail = 4202,
            Contentsettings = 10165,
            SurveyResponse = 10096,
            Appointment = 4201,
            Competitor = 123,
            MarketingList = 4300,
            ServiceActivity = 4214,
            Case = 112,
            Task = 4212,
            FormsProsurveyinvite = 10049,
            AppointmentactivityMarketingTemplate = 10162,
            LinkedInactivity = 10221,
            Position = 50,
            LinkedInformquestion = 10227,
            TaskactivityMarketingTemplate = 10199,
            MarketingActivity = 10193,
            EntitlementChannel = 9701,
            Fax = 4204,
            LinkedInfieldmapping = 10224,
            Letter = 4207,
            ResponseAction = 10086,
            CampaignActivity = 4402,
            Contract = 1010,
            Goal = 9600,
            PhoneCallactivityMarketingTemplate = 10194,
            Contact = 2,
            RecurringAppointment = 4251,
            Team = 9,
            ProfileAlbum = 10040
        }
        public enum TypeTo_OptionSet
        {
            PhoneCall = 4210,
            SurveyActivity = 10094,
            DynamicContentmetadata = 10179,
            Quote = 1084,
            ResourceGroup = 4007,
            SurveyResponse = 10096,
            SocialProfile = 99,
            Loan = 3,
            Contract = 1010,
            ChannelAccessProfileRule = 9400,
            Activity = 4200,
            Letter = 4207,
            LinkedInLeadGenformsubmission = 10228,
            TaskactivityMarketingTemplate = 10199,
            FormsProsurveyresponse = 10050,
            ProfileAlbum = 10040,
            LinkedInleadmatchingstrategy = 10229,
            Case = 112,
            MarketingActivity = 10193,
            PhoneCallactivityMarketingTemplate = 10194,
            LinkedInLeadGenform = 10225,
            MarketingEMailDynamicContentmetadata = 10181,
            User = 8,
            UICconfig = 10200,
            FacilityEquipment = 4000,
            RecurringAppointment = 4251,
            Order = 1088,
            Invoice = 1090,
            LinkedInAccount = 10220,
            MarketingList = 4300,
            LinkedInfieldmapping = 10224,
            LinkedInactivity = 10221,
            EntitlementTemplateChannel = 9703,
            SchedulingGroup = 4005,
            AppointmentactivityMarketingTemplate = 10162,
            PriceList = 1022,
            EMail = 4202,
            LeadScoringModel = 10215,
            KnowledgeBaseRecord = 9930,
            KnowledgeArticle = 9953,
            Fax = 4204,
            FormsProsurveyinvite = 10049,
            Entitlement = 9700,
            Product = 1024,
            ProcessSession = 4710,
            Goal = 9600,
            Contact = 2,
            Competitor = 123,
            Lead = 4,
            Account = 1,
            Team = 9,
            SocialActivity = 4216,
            ResponseError = 10089,
            LinkedInformsubmissionanswer = 10226,
            Campaign = 4400,
            Contentsettings = 10165,
            ServiceActivity = 4214,
            MarketingEMailTestsend = 10183,
            CampaignActivity = 4402,
            EntitlementChannel = 9701,
            Appointment = 4201,
            ResponseAction = 10086,
            Territory = 2013,
            Position = 50,
            LinkedInformquestion = 10227,
            Task = 4212,
            LinkedInUserProfile = 10230
        }
    }
}
