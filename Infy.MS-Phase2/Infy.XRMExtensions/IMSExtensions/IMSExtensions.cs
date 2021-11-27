using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class IMSExtensions
    {
        /// <summary>
        /// Calculate LTV i.e. Loan to Value ratio
        /// </summary>
        /// <param name="loanAmount"></param>
        /// <param name="propertyAppraisedValue"></param>
        /// <param name="convertToPercent"></param>
        /// <returns></returns>
        public static decimal CalculateLTV(decimal loanAmount, decimal propertyAppraisedValue, bool convertToPercent = false)
        {
            /////////////////////////////////////////////////////
            // LTV = (Loan Amount / Property Appraised Value
            /////////////////////////////////////////////////////

            if (propertyAppraisedValue == 0) return 0m;

            var ltv = (loanAmount / propertyAppraisedValue);
            return (convertToPercent ? (ltv * 100) : ltv);
        }
    }
}
