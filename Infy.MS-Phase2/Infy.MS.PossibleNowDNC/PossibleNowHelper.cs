using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Infy.MS.PossibleNowDNC
{
    public class PossibleNowHelper
    {
        public static DNC GetPossibleNowDNC(string baseUrl, string requestUri, string phoneNumber)
        {
            DNC dncObject = null;
            try
            {
                HttpClient httpClientforDNC = new HttpClient();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultNetworkCredentials;
                httpClientforDNC.BaseAddress = new Uri(baseUrl);
                HttpResponseMessage getResponse =
                httpClientforDNC.GetAsync(requestUri + phoneNumber).Result;

                if (getResponse.IsSuccessStatusCode)
                {
                    dncObject = JsonConvert.DeserializeObject<DNC>(getResponse.Content.ReadAsStringAsync().Result);
                }
                else
                {
                    string invalidResponse = getResponse.Content.ReadAsStringAsync().Result;
                    throw new Exception("Error from DNC API:" + invalidResponse);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dncObject;
        }
    }
    public class DNC
    {
        public Ebrstatus EBRStatus { get; set; }
        public string PhoneNumber { get; set; }
        public string Status { get; set; }
        public string TimeZone { get; set; }
        public Filter[] Filters { get; set; }
        public Callcurfewwindow CallCurfewWindow { get; set; }
    }

    public class Ebrstatus
    {
        public string EBRExemption { get; set; }
        public string GoodThruDate { get; set; }
    }

    public class Callcurfewwindow
    {
        public string Start { get; set; }
        public string End { get; set; }
        public string WithinCallCurfewWindow { get; set; }
        public string WithinCallWindow { get; set; }
    }

    public class Filter
    {
        public string FilterName { get; set; }
        public string Flag { get; set; }
    }
}
