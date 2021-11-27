using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRMExtensions
{
    public static class Email
    {
        public const string EntityName = "email";
        public const string From = "from";
        public const string To = "to";
        public const string Subject = "subject";
        public const string RegardingObjectId = "regardingobjectid";
        public const string Description = "description";
        public const string ImportSequenceNumber = "importsequencenumber";
        public const string ModifiedByDelegate = "modifiedonbehalfby";
        public const string ModifiedBy = "modifiedby";
        public const string Bcc = "bcc";
        public const string Cc = "cc";
        public const string modifiedon = "modifiedon";
        public const string ActivityStatus = "statecode";
        public const string partyid = "partyid";
        public const string StatusReason = "statuscode";
        public const string IsDeleteEligible = "ims_isdeleteeligible";

        // Default Values for Subject and Description for Sample Email Activity Creation
        public const string EmailSubject = "Birthday Wishes";
        public const string EmailDescription = "Wish you Many Happy Returns of the Day!";

    }
}
