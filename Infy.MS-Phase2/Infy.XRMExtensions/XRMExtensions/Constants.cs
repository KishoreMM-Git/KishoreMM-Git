using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRMExtensions
{
    public class Constants
    {
        public enum NewRecord { ValueIsNull, ValueIsNotNull, ValueIsDiffFromOldValue };

        public const string IDMLOWebsite = "LO Website";
        public const string IDMContactUSWebsite = "Contact US Website";
        public const string IDMEDWLead = "LDW Lead";
        public const string IDMEDWLoan = "LDW Loan";
        public const string LeadPops = "Leadpops";
        public const string CINC = "CINC";
        public const string BoomTown = "BoomTown";
        public const string ZillowLeadSource = "Zillow";
        public const string AppConfigSetup = "StagingPostCreate";
        public const string LeadSourcePostCreateUpdate = "LeadSourcePostCreateUpdate";
        public const string Contactfields = "Lead to Contact";
        public const string OtherContact = "Other Contact";
        public const string CoBorrower = "Co-borrowers";
        public const string Property = "Property";
        public const string LeadProperty = "Lead Property";
        public const string MovementInternalLeadProperty = "Movement Internal Lead Property";
        public const string CurrentClient = "Current Client";
        public const string Lead = "Lead";
        public const string Borrower = "Borrower";
        public const string AppConfigCustomer = "ContactDuplicateDetectionPostCreate";
        public const string errormessage = "Error Messages";
        public const string Zillow = "ZILLOW - CORPORATE";
        public const string ZillowSellersAgent = "Zillow Sellers Agent";
        public const string EmployeeLoan = "Employee Loans";
		public const string BankRate = "BankRate";
        public const string MovementInternal = "Movement Internal";
        public const string Kunversion = "Kunversion";
        public const string LendingTree = "LendingTree";
        public const string Realtor = "Realtor.com";
        public const string BlendMovehome = "BlendMovehome";
        public const string LeadContactPreOperationPlugin = "LeadContactPreOperationPlugin";
        public const string MovementDirectSecurityRole = "MovementDirectSecurityRole";
        



        public const string LeadCoborrower = "Lead Co-Borrower";
        public const string LoanBorrower = "Loan Borrower";

        //Connection Role Name
        public const string BuyersAgentConnectionRole = "Buyer's Agent";
        public const string SellersAgentConnectionRole = "Seller's Agent";
        public const string SettlementAgentConnectionRole = "Settlement Agent";
        public const string AttorneyConnectionRole = "Attorney";

        #region Configurations
        public const string DefaultLoanStatus = "DefaultLoanStatus";
        public const string ValidationError_MandatoryCheck = "StagingValError_MandatoryCheck";
        public const string ValidationError_MaxLengthCheck = "StagingValError_MaxLengthCheck";
        public const string ValidationError_LookupUnresolved = "StagingValError_LookupUnresolved";
        public const string ValidationError_OptionsetUnresolved = "StagingValError_OptionsetUnresolved";
        public const string ValidationError_TwooptionsUnresolved = "StagingValError_TwooptionsUnresolved";
        public const string ErrorInSettingValue = "Staging_ErrorInSettingValue";
        public const string DupliRecordFound = "Staging_DupliRecordFound";
        public const string MDDupliRecordFound = "Staging_MDDupliRecordFound";
        public const string CreateErrorLog = "Staging_CreateErrorLog";
        public const string LeadDefaultStatus = "lead";
        public const string DefaultLeadStatus_MovementDirect = "DefaultLeadStatus_MovementDirect";
        public const string LOPreapproved = "LO Preapproved";
        public const string LoanDefaultStatus = "active";
        public const string LoanStausMappingDefaultStatus = "default";
        public const string Validate_Two_Options = "Validate_Two_Options";
        public const string PropertyDuplicateCheck = "Property_Duplicate_Check";
        public const string MarketingListContactGroupAsscoCheck = "MarketingListContactGroupAsscoCheck";
        public const string D365CRM_EDW_INTEGRATION_Id = "D365CRM_EDW_INTEGRATION";
        public const string AppConfigCustomerDuplicatecheck = "Duplicatecheck";
        public const string errromessageforCoBrrower = "Co-Borrower Error Message";
        public const string errromessageforOtherContact = "Other Contact Error Message";
        public const string AppConfigContactType = "DuplicatecheckforContactType";
        public const string GeneralNote = "General Note";
        public const string GetBorrower_ExternalIDExists = "GetBorrower_ExternalIDExists";
        public const string GetBorrower_ExternalIDDoesNotExists = "GetBorrower_ExternalIDDoesNotExists";
        public const string Lead_Title_PossibleValues = "Lead_Title_PossibleValues";
        public const string Lead_MaritalStatus_Unmarried = "Lead_MaritalStatus_Unmarried";
        public const string DuplicateDetectionRule_Lead_Contact_Account = "DuplicateDetectionRule_Lead_Contact_Account";
        public const string GetteamName = "Get Team";
        public const string GetTeamId = "Get Team ID";
        public const string IsLoanOfficerCheck = "IsLoanOfficerCheck";
        public const string IsLoanOfficerAssistantCheck = "IsLoanOfficerAssistantCheck";
        public const string IsUserHaveSecuirtyRole = "IsUserHaveSecuirtyRole";
        public const string MovementDirectBusinessUnit = "MovementDirect_BusinessUnit";
        public const string MovementDirectLead_Identification_LeadSource = "MovementDirectLead_Identification_LeadSource";
        public const string MovementDirectLead_Identification_ParentLeadSource = "MovementDirectLead_Identification_ParentLeadSource";
        public const string MovementDirectLeadDUplicateCheck = "MovementDirectLeadDuplicateCheck";
        public const string MovementDirectLeadDuplicateCheck_MobileNumber = "MovementDirectLeadDuplicateCheck_MobileNumber";
        public const string MovementDirectLeadDuplicateCheck_EmailAddress = "MovementDirectLeadDuplicateCheck_EmailAddress";
        public const string LeadStatus_Active_MBAPreapproved = "LeadStatus_Active_MBAPreapproved";
        public const string PostingURL_ClientId = "PostingURL_ClientId";
        public const string PostingURL_BaseApi = "PostingURL_BaseApi";
        public const string ModifiedBy_RoleCheck = "ModifiedBy_RoleCheck";
        public const string Owner_CheckRole = "Owner_RoleCheck";
        public const string GetFailedLoans = "GetFailedLoans";
        public const string VeloLDW = "Velo-LDW";
        public const string BlendApplicationAsTask = "BlendApplicationAsTask";
        #endregion

        //Access Team Templates
        public const string LeadLOAAccessTeam_Template = "Lead LOA Access Team Template";
        public const string GetAccessTeamTemplatecontact = "Customer LOA Access Team Template";
        public const string GetAccessTeamTemplateLoan = "Loan LOA Access Team Template";
        public const string GetEmailTempNameForLOA = "Send Notification To LOA";

        //Relationship
        public const string UsertoStateRelationshipName = "ims_systemuser_ims_state";
        public const string GetStateLicense = "State License";

        //Advance Lead Score
        public const string ALSImportDataMaster = "Advance Lead Score";
        public const string ALSAppConfigSetupName = "Advance Lead Score";
        public const string ALSFunctionURL = "ALSFunctionURL";

        //SendEmailNotificationToSharedLeadsLoans
        public const string SendEmailNotificationToSharedLeadsLoans = "SendEmailNotificationToSharedLeadsLoans";
        public const string LeadEmailTemplateTitle = "LeadEmailTemplateTitle";
        public const string LoanEmailTemplateTitle = "LoanEmailTemplateTitle";
        public const string LeadEmailTemplateDescription = "LeadEmailTemplateDescription";
        public const string LoanEmailTemplateDescription = "LoanEmailTemplateDescription";
        public const string EnvUrl = "EnvUrl";
        public const string LeadUrl = "LeadURL";
        public const string LoanUrl = "LoanURL";

        //LoanAtRisk
        public const string AppConfigLoanAtRiskStatus = "Loanatriskstatus";

        //Azure WebJobs Configuration List
        public const string AutoShareCred = "AutoSharePassword";
        public const string AutoShareUrl = "AutoShareUrl";
        public const string AutoShareUserName = "AutoShareUserName";
    }
}
