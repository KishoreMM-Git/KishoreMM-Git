using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRMExtensions
{
    public static class MortgageLoanProcess
    {
        public const string EntityName = "ims_mortgageloanprocess";
        public const string PrimaryKey = "businessprocessflowinstanceid";
        public const string Name = "bpf_name";
        public const string Lead = "bpf_leadid";
        public const string TraversedPath = "traversedpath";
        public const string Process = "processid";
        public const string Opportunity = "bpf_opportunityid";
        public const string ActiveStage = "activestageid";
    }
}
