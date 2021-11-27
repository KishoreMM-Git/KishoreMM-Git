using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infy.MS.PossibleNowDNC
{
    class Program
    {
        static void Main(string[] args)
        {
            //fetch Azure configurations from App.config to GetAccessToken
            string authority = ConfigurationManager.AppSettings[Constants.Authority].ToString();
            string clientId = string.Empty;//ConfigurationManager.AppSettings[Constants.ClientId].ToString();
            string secret =  string.Empty;//ConfigurationManager.AppSettings[Constants.Secret].ToString();
            string serviceUrl = ConfigurationManager.AppSettings[Constants.ServiceUrl].ToString();
            string keyVaultUrl = ConfigurationManager.AppSettings["KeyVaultUrl"].ToString();
            string clientIdSecretName = ConfigurationManager.AppSettings["clientIdSecretName"].ToString();
            string cientSecretName = ConfigurationManager.AppSettings["ClientSecretName"].ToString();
            int jobFrequency = 1;// Convert.ToInt32(ConfigurationManager.AppSettings[Constants.jobFreqency].ToString());

            if (args.Length > 0)
            {
                if (!string.IsNullOrEmpty(args[0]))
                    jobFrequency = int.Parse(args[0]);
                
            }
           // jobFrequency = 1;

            string possibleNowBaseUrl = string.Empty;
            string possibleNOWRequestUri = string.Empty;
            string dncFilterNotFoundConfig = string.Empty;

            int DNCFoundCounter = 0;
            int DNCNotFoundCounter = 0;

            int contactDNCFoundCounter = 0;
            int contactDNCNotFoundCounter = 0;

            string accessToken = string.Empty;
            Dictionary<string, string> dcConfigs = null;
            List<ActiveLead> lstActiveLeads = null;
            List<ActiveContact> lstActiveContacts = null;
            bool batcjJobStatus = true;
            string batchJobLog = string.Empty;
            Guid batchJobRecordId = Guid.Empty;
            Common objCommon = new Common();
            batchJobLog += "---Batch Job Started---" + DateTime.Now + Environment.NewLine;
            batchJobLog += "<<=================================================>>" + Environment.NewLine;
            batchJobLog += "Job Frequency " + jobFrequency + Environment.NewLine;
            Console.WriteLine("Job Frequency:" + jobFrequency);
            try
            {
				clientId = objCommon.GetSecretValue(keyVaultUrl, clientIdSecretName).Result;
                //clientId = "dbbfed03-5023-410b-b588-8db70a61c8af";
                //clientId = "156d5a4b-0618-46cd-9b8c-a70e17d0eba1";

                Console.WriteLine("Azure key Vault ClientId secret Value:"+clientId);
				secret = objCommon.GetSecretValue(keyVaultUrl, cientSecretName).Result;
                //secret = "rFzxuRtO20n@BywqSIn_i7Y]5Q]nnAMQ";
                //secret = "wa1L:.]7=NnBR7l0?sTYn6ZytngidJPs";

                Console.WriteLine("Azure key Vault Client secret Value:" + secret);
                //Create Batch Job Record in CRM
                batchJobRecordId = Common.CreateBatchJobRecord(authority, clientId, secret, serviceUrl, Constants.batchJobName).Result;
                batchJobLog += "Batch Job Record Created" + Environment.NewLine;
                accessToken = objCommon.GetAccessToken(authority, clientId, secret, serviceUrl);
                //Fetch possible Now Configuration
                if (!string.IsNullOrEmpty(accessToken))
                {
                    batchJobLog += "Access Token retrieved" + Environment.NewLine;
                    dcConfigs = objCommon.FetchConfigurations(accessToken, serviceUrl, Constants.configSetUpName);
                    if (dcConfigs != null && dcConfigs.Count > 0)
                    {
                        batchJobLog += "Configuration exist in CRM for Possible Now" + Environment.NewLine;
                        if (dcConfigs.ContainsKey("BaseUrl")) possibleNowBaseUrl = dcConfigs["BaseUrl"].ToString();
                        if (dcConfigs.ContainsKey("RequestUri")) possibleNOWRequestUri = dcConfigs["RequestUri"].ToString();
                        if (dcConfigs.ContainsKey(Constants.DNCFilterNotFound)) dncFilterNotFoundConfig = dcConfigs[Constants.DNCFilterNotFound].ToString();

                        if (!string.IsNullOrEmpty(possibleNowBaseUrl) && !string.IsNullOrEmpty(possibleNOWRequestUri))
                        {
                            //fetch Active leads which has either home phone, cell phone or work phone
                            lstActiveLeads = objCommon.FetchActiveLeads(authority, clientId, secret, serviceUrl, accessToken, dcConfigs, jobFrequency);

                            //Process Each Lead and Get DNC value from Possible Now API
                            if (lstActiveLeads != null && lstActiveLeads.Count > 0)
                            {
                                batchJobLog += "Total Active Leads which has either Mobile or work phone in CRM:" + lstActiveLeads.Count + Environment.NewLine;
                                foreach (ActiveLead objLead in lstActiveLeads)
                                {
                                    try
                                    {
                                        if (objLead != null)
                                        {
                                            bool dncFound = false;
                                            bool isInternalDNC = false;
                                            DNC dncObject = null;
                                            //check Mobile phone in DNC registry 
                                            if (!string.IsNullOrEmpty(objLead.CellPhone))
                                            {
                                                try
                                                {
                                                    //Get Last 10 Digit if Phone length>10
                                                    if (objLead.CellPhone.Length > 10) objLead.CellPhone = objLead.CellPhone.Substring(objLead.CellPhone.Length - 10);
                                                    //Cal Possible Now API to get DNC
                                                    dncObject = PossibleNowHelper.GetPossibleNowDNC(possibleNowBaseUrl, possibleNOWRequestUri, objLead.CellPhone);
                                                    if (dncObject != null)
                                                    {
                                                        if (dncObject.Status.ToUpper() == "DNC")
                                                        {
                                                            if (dncObject.Filters != null)
                                                            {
                                                                Filter[] objDNCFilters = dncObject.Filters;
                                                                foreach (Filter filter in objDNCFilters)
                                                                {
                                                                    if (!string.IsNullOrEmpty(filter.FilterName) || !string.IsNullOrEmpty(filter.Flag))
                                                                    {
                                                                        dncFound = true;
                                                                        //Create Communication Preference record with Filter Name & Flag
                                                                        //Create Communication Preference record with Filter Name & Flag
                                                                        Task.WaitAll(Task.Run(async () => await Common.CreateCommunicationPreference
                                                                        (authority, clientId, secret, serviceUrl, batchJobRecordId, Guid.Parse(objLead.LeadRecordId),
                                                                        objLead.CellPhone, filter.FilterName, filter.Flag)));
                                                                    }
                                                                    if (filter.FilterName.ToLower() == "movement mortgage" || filter.Flag.ToLower() == "mst")
                                                                    {
                                                                        isInternalDNC = true;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            //Create Communication Preference record with Filter Name & Flag as Do not Found
                                                            Task.WaitAll(Task.Run(async () => await Common.CreateCommunicationPreference
                                                            (authority, clientId, secret, serviceUrl, batchJobRecordId, Guid.Parse(objLead.LeadRecordId),
                                                            objLead.CellPhone, dncFilterNotFoundConfig, dncFilterNotFoundConfig)));
                                                        }
                                                    }
                                                    dncObject = null;
                                                }
                                                catch (Exception ex)
                                                {
                                                    batcjJobStatus = false;
                                                    batchJobLog += ex.Message + Environment.NewLine;
                                                }
                                            }
                                            //check Other phone in DNC registry 
                                            if (!string.IsNullOrEmpty(objLead.WorkPhone))
                                            {
                                                try
                                                {
                                                    //Get Last 10 Digit if Phone length>10
                                                    if (objLead.WorkPhone.Length > 10) objLead.WorkPhone = objLead.WorkPhone.Substring(objLead.WorkPhone.Length - 10);
                                                    //Cal Possible Now API to get DNC
                                                    dncObject = PossibleNowHelper.GetPossibleNowDNC(possibleNowBaseUrl, possibleNOWRequestUri, objLead.WorkPhone);
                                                    if (dncObject != null)
                                                    {
                                                        if (dncObject.Status.ToUpper() == "DNC")
                                                        {
                                                            if (dncObject.Filters != null)
                                                            {
                                                                Filter[] objDNCFilters = dncObject.Filters;
                                                                foreach (Filter filter in objDNCFilters)
                                                                {
                                                                    if (!string.IsNullOrEmpty(filter.FilterName) || !string.IsNullOrEmpty(filter.Flag))
                                                                    {
                                                                        dncFound = true;
                                                                        //Create Communication Preference record with Filter Name & Flag
                                                                        Task.WaitAll(Task.Run(async () => await Common.CreateCommunicationPreference
                                                                        (authority, clientId, secret, serviceUrl, batchJobRecordId, Guid.Parse(objLead.LeadRecordId),
                                                                        objLead.WorkPhone, filter.FilterName, filter.Flag)));
                                                                    }
                                                                    if (filter.FilterName.ToLower() == "movement mortgage" || filter.Flag.ToLower() == "mst")
                                                                    {
                                                                        isInternalDNC = true;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            //Create Communication Preference record with Filter Name & Flag as Do not Found
                                                            Task.WaitAll(Task.Run(async () => await Common.CreateCommunicationPreference
                                                            (authority, clientId, secret, serviceUrl, batchJobRecordId, Guid.Parse(objLead.LeadRecordId),
                                                            objLead.WorkPhone, dncFilterNotFoundConfig, dncFilterNotFoundConfig)));
                                                        }
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    batcjJobStatus = false;
                                                    batchJobLog += ex.Message + Environment.NewLine;
                                                }
                                            }

                                            //Update Do not allow phone on Lead
                                            if (dncFound)
                                            {
                                                DNCFoundCounter++;
                                                if (!string.IsNullOrEmpty(objLead.LeadRecordId))
                                                {
                                                    try
                                                    {
                                                        //Update Do Not Allow Call in CRM to Do Not Allow
                                                       // if (!objLead.Donotcall)
                                                            Task.WaitAll(Task.Run(async () => await Common.UpdateLeadDNC(authority, clientId, secret, serviceUrl, objLead, true,isInternalDNC,dcConfigs)));
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        batcjJobStatus = false;
                                                        batchJobLog += ex.Message + Environment.NewLine;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //Set do not allow phone to Allow
                                                DNCNotFoundCounter++;
                                                try
                                                {
                                                    //Update Do Not Allow Call in CRM to Allow
                                                    if (!string.IsNullOrEmpty(objLead.LeadId))
                                                    {
                                                        //if (objLead.Donotcall)
                                                            Task.WaitAll(Task.Run(async () => await Common.UpdateLeadDNC(authority, clientId, secret, serviceUrl, objLead, false,isInternalDNC,dcConfigs)));
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    batcjJobStatus = false;
                                                    batchJobLog += ex.Message + Environment.NewLine;
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        batcjJobStatus = false;
                                        batchJobLog += ex.Message + Environment.NewLine;
                                    }
                                }
                            }
                            else
                            {
                                batchJobLog += "No Active Leads which has either Mobile,cell or work phone found in CRM:" + Environment.NewLine;
                            }


                            //fetch Active Contacts which has either home phone, cell phone or work phone
                            lstActiveContacts = objCommon.FetchActiveContacts(authority, clientId, secret, serviceUrl, accessToken, dcConfigs, jobFrequency);

                            if (lstActiveContacts != null && lstActiveContacts.Count > 0)
                            {
                                batchJobLog += "Total Active Contacts which has either Mobile or work phone in CRM:" + lstActiveContacts.Count + Environment.NewLine;
                                foreach (ActiveContact objContact in lstActiveContacts)
                                {
                                    try
                                    {
                                        if (objContact != null)
                                        {
                                            bool dncFound = false;
                                            bool isInternalDNC = false;
                                            DNC dncObject = null;
                                            //check Mobile phone in DNC registry 
                                            if (!string.IsNullOrEmpty(objContact.CellPhone))
                                            {
                                                try
                                                {
                                                    //Get Last 10 Digit if Phone length>10
                                                    if (objContact.CellPhone.Length > 10) objContact.CellPhone = objContact.CellPhone.Substring(objContact.CellPhone.Length - 10);
                                                    //Cal Possible Now API to get DNC
                                                    dncObject = PossibleNowHelper.GetPossibleNowDNC(possibleNowBaseUrl, possibleNOWRequestUri, objContact.CellPhone);
                                                    if (dncObject != null)
                                                    {
                                                        if (dncObject.Status.ToUpper() == "DNC")
                                                        {
                                                            if (dncObject.Filters != null)
                                                            {
                                                                Filter[] objDNCFilters = dncObject.Filters;
                                                                foreach (Filter filter in objDNCFilters)
                                                                {
                                                                    if (!string.IsNullOrEmpty(filter.FilterName) || !string.IsNullOrEmpty(filter.Flag))
                                                                    {
                                                                        dncFound = true;
                                                                        //Create Communication Preference record with Filter Name & Flag
                                                                        //Create Communication Preference record with Filter Name & Flag
                                                                        //Task.WaitAll(Task.Run(async () => await Common.CreateCommunicationPreference
                                                                        //(authority, clientId, secret, serviceUrl, batchJobRecordId, Guid.Parse(objContact.ContactRecordId),
                                                                        //objContact.CellPhone, filter.FilterName, filter.Flag)));
                                                                    }
                                                                    if (filter.FilterName.ToLower() == "movement mortgage" || filter.Flag.ToLower() == "mst")
                                                                    {
                                                                        isInternalDNC = true;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            //Create Communication Preference record with Filter Name & Flag as Do not Found
                                                            //Task.WaitAll(Task.Run(async () => await Common.CreateCommunicationPreference
                                                            //(authority, clientId, secret, serviceUrl, batchJobRecordId, Guid.Parse(objContact.ContactRecordId),
                                                            //objContact.CellPhone, dncFilterNotFoundConfig, dncFilterNotFoundConfig)));
                                                        }
                                                    }
                                                    dncObject = null;
                                                }
                                                catch (Exception ex)
                                                {
                                                    batcjJobStatus = false;
                                                    batchJobLog += ex.Message + Environment.NewLine;
                                                }
                                            }
                                            //check Other phone in DNC registry 
                                            if (!string.IsNullOrEmpty(objContact.WorkPhone))
                                            {
                                                try
                                                {
                                                    //Get Last 10 Digit if Phone length>10
                                                    if (objContact.WorkPhone.Length > 10) objContact.WorkPhone = objContact.WorkPhone.Substring(objContact.WorkPhone.Length - 10);
                                                    //Cal Possible Now API to get DNC
                                                    dncObject = PossibleNowHelper.GetPossibleNowDNC(possibleNowBaseUrl, possibleNOWRequestUri, objContact.WorkPhone);
                                                    if (dncObject != null)
                                                    {
                                                        if (dncObject.Status.ToUpper() == "DNC")
                                                        {
                                                            if (dncObject.Filters != null)
                                                            {
                                                                Filter[] objDNCFilters = dncObject.Filters;
                                                                foreach (Filter filter in objDNCFilters)
                                                                {
                                                                    if (!string.IsNullOrEmpty(filter.FilterName) || !string.IsNullOrEmpty(filter.Flag))
                                                                    {
                                                                        dncFound = true;
                                                                        //Create Communication Preference record with Filter Name & Flag
                                                                        //Task.WaitAll(Task.Run(async () => await Common.CreateCommunicationPreference
                                                                        //(authority, clientId, secret, serviceUrl, batchJobRecordId, Guid.Parse(objContact.ContactRecordId),
                                                                        //objContact.WorkPhone, filter.FilterName, filter.Flag)));

                                                                    }
                                                                    if (filter.FilterName.ToLower() == "movement mortgage" || filter.Flag.ToLower() == "mst")
                                                                    {
                                                                        isInternalDNC = true;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            //Create Communication Preference record with Filter Name & Flag as Do not Found
                                                            //Task.WaitAll(Task.Run(async () => await Common.CreateCommunicationPreference
                                                            //(authority, clientId, secret, serviceUrl, batchJobRecordId, Guid.Parse(objContact.ContactRecordId),
                                                            //objContact.WorkPhone, dncFilterNotFoundConfig, dncFilterNotFoundConfig)));
                                                        }
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    batcjJobStatus = false;
                                                    batchJobLog += ex.Message + Environment.NewLine;
                                                }
                                            }

                                            //Update Do not allow phone on Lead
                                            if (dncFound)
                                            {
                                                contactDNCFoundCounter++;
                                                if (!string.IsNullOrEmpty(objContact.ContactRecordId))
                                                {
                                                    try
                                                    {
                                                        //Update Do Not Allow Call in CRM to Do Not Allow
                                                       // if (!objContact.Donotcall)
                                                            Task.WaitAll(Task.Run(async () => await Common.UpdateContactDNC(authority, clientId, secret, serviceUrl, objContact, true,isInternalDNC,dcConfigs)));
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        batcjJobStatus = false;
                                                        batchJobLog += ex.Message + Environment.NewLine;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //Set do not allow phone to Allow
                                                contactDNCNotFoundCounter++;
                                                try
                                                {
                                                    //Update Do Not Allow Call in CRM to Allow
                                                    if (!string.IsNullOrEmpty(objContact.ContactId))
                                                    {
                                                       // if (objContact.Donotcall)
                                                            Task.WaitAll(Task.Run(async () => await Common.UpdateContactDNC(authority, clientId, secret, serviceUrl, objContact, false,isInternalDNC, dcConfigs)));
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    batcjJobStatus = false;
                                                    batchJobLog += ex.Message + Environment.NewLine;
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        batcjJobStatus = false;
                                        batchJobLog += ex.Message + Environment.NewLine;
                                    }
                                }
                            }
                            else
                            {
                                batchJobLog += "No Active Contacts which has either Mobile,cell or work phone found in CRM:" + Environment.NewLine;
                            }
                        }
                        else
                        {
                            batchJobLog += "Possible Now API Base Url & Request URI value is null!" + Environment.NewLine;
                        }
                    }
                    else
                    {
                        batchJobLog += "No configuration found for possible now API. Please check configuration record in CRM." + Environment.NewLine;
                    }
                }
                else
                {
                    batchJobLog += "Access Token retrieval failed!" + Environment.NewLine;
                }
            }
            catch (Exception ex)
            {
                batcjJobStatus = false;
                batchJobLog += Environment.NewLine + "Error @ Main Method " + ex.Message + Environment.NewLine;
                Console.WriteLine(ex.InnerException);
            }
            finally
            {
                batchJobLog += "Total Leads Phone Number found in DNC:" + DNCFoundCounter.ToString() + Environment.NewLine;
                batchJobLog += "Total Leads Phone Number Not found in DNC:" + DNCNotFoundCounter.ToString() + Environment.NewLine;
                batchJobLog += "Total Contacts Phone Number found in DNC:" + contactDNCFoundCounter.ToString() + Environment.NewLine;
                batchJobLog += "Total Contacts Phone Number Not found in DNC:" + contactDNCNotFoundCounter.ToString() + Environment.NewLine;
                batchJobLog += "---Batch Job Ended---" + DateTime.Now + Environment.NewLine;
                batchJobLog += "<<=================================================>>" + Environment.NewLine;
                //Update Log record
                Task.WaitAll(Task.Run(async () => await Common.UpdateBatchJobStatus(authority, clientId, secret, serviceUrl, batchJobRecordId, batcjJobStatus, batchJobLog, Constants.batchJobName)));
            }
        }
    }
}
