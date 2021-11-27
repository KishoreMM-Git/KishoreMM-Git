using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infy.MS.SMSActivity
{
    public class SMSActivity
    {
        static void Main(string[] args)
        {
            //fetch Azure configurations from App.config to GetAccessToken
            string authority = ConfigurationManager.AppSettings[Constants.Authority].ToString();
            string clientId = ConfigurationManager.AppSettings[Constants.ClientId].ToString();
            string secret = ConfigurationManager.AppSettings[Constants.Secret].ToString();
            string serviceUrl = ConfigurationManager.AppSettings[Constants.ServiceUrl].ToString();
            string jobFrequency= ConfigurationManager.AppSettings[Constants.jobFreqency].ToString();
            string keyVaultUrl = ConfigurationManager.AppSettings["KeyVaultUrl"].ToString();
            string clientIdSecretName = ConfigurationManager.AppSettings["clientIdSecretName"].ToString();
            string cientSecretName = ConfigurationManager.AppSettings["ClientSecretName"].ToString();

            Common objCommon = new Common();
            clientId = objCommon.GetSecretValue(keyVaultUrl, clientIdSecretName).Result;
            Console.WriteLine("Azure key Vault ClientId secret Value:" + clientId);
            secret = objCommon.GetSecretValue(keyVaultUrl, cientSecretName).Result;
            Console.WriteLine("Azure key Vault Client secret Value:" + secret);
            string batchJobLog = string.Empty;
            bool batcjJobStatus = true;
            int batchJobStatusReason = 176390000;
            int totalActivityRequests = 0;
            int totalFailedRequests = 0;
            Dictionary<string, string> dcConfigs = null;
            //int totalActivityRequests = 0;
            //int activitiyRequestsProcessed = 0;
            //int activitiyRequestsFailed = 0;
            string accessToken = string.Empty;
            batchJobLog += "---Batch Job Started---" + DateTime.Now + Environment.NewLine;
            batchJobLog += "<<=================================================>>" + Environment.NewLine;
            batchJobLog += "Job Frequency " + jobFrequency + Environment.NewLine;
            Guid batchJobRecordId = Common.CreateBatchJobRecord(authority, clientId, secret, serviceUrl, Constants.batchJobName).Result;
            batchJobLog += "Batch Job Record Created" + Environment.NewLine;
            try
            {
                //Common objCommon = new Common();
                accessToken = objCommon.GetAccessToken(authority, clientId, secret, serviceUrl);
                if (!string.IsNullOrEmpty(accessToken))
                {
                    batchJobLog += "Access Token retrieved" + Environment.NewLine;
                    dcConfigs = objCommon.FetchConfigurations(accessToken, serviceUrl, Constants.configSetUpName);
                    if (dcConfigs != null && dcConfigs.Count > 0)
                    {
                        batchJobLog += "Configuration exist in CRM for SMS Activity" + Environment.NewLine;
                        if (dcConfigs.ContainsKey(Constants.ActivityRequestSMSTypeFetchXml))
                        {
                            var fetchXmlActivityRequests = dcConfigs[Constants.ActivityRequestSMSTypeFetchXml];
                            var activityRequestResponses = objCommon.GetActivityRequests(objCommon.GetAccessToken(authority, clientId, secret, serviceUrl), serviceUrl, clientId, secret, fetchXmlActivityRequests);
                            if (activityRequestResponses.Count > 0)
                            {
                                totalActivityRequests = activityRequestResponses.Count;
                                foreach (var activityRequest in activityRequestResponses)
                                {
                                    string activityRequestBatchJobLog = string.Empty;
                                    int failedSmsActivities = 0;
                                    int successSmsActivities = 0;
                                    int totalLeadsUnderMarketingList = 0;
                                    activityRequestBatchJobLog += "-------------Activity Request Batch Job Started-----------" + DateTime.Now + Environment.NewLine;
                                    activityRequestBatchJobLog += "<<=================================================>>" + Environment.NewLine;
                                    Guid activityRequestBatchJobId = Common.CreateBatchJobRecord(authority, clientId, secret, serviceUrl, activityRequest.activityRequestName).Result;
                                    activityRequestBatchJobLog += "Activity Request Batch Job Record Created" + Environment.NewLine;
                                    try
                                    {
                                        if (!string.IsNullOrEmpty(activityRequest.marketingListType))
                                        {
                                            //Dynamic ML
                                            if (Convert.ToBoolean(activityRequest.marketingListType))
                                            {
                                                activityRequestBatchJobLog += "Activity Request has Dynamic Marketing List" + Environment.NewLine;
                                                var dynamicMarketingListQueryRequest = objCommon.GetDynamicMarketingListQuery(accessToken, serviceUrl, clientId, secret, activityRequest.marketingListGuid);
                                                if (dynamicMarketingListQueryRequest != null)
                                                {
                                                    if (dynamicMarketingListQueryRequest.query != null)
                                                    {
                                                        int pageNumber = 1;
                                                        string pagingCookie = string.Empty;
                                                        var marketingListLeads = objCommon.GetLeads(pageNumber, pagingCookie, objCommon.GetAccessToken(authority, clientId, secret, serviceUrl), serviceUrl, clientId, secret, dynamicMarketingListQueryRequest.query);
                                                        if (marketingListLeads.Count > 0)
                                                        {
                                                            totalLeadsUnderMarketingList = marketingListLeads.Count;
                                                            activityRequestBatchJobLog += "<<*************************************************>>" + Environment.NewLine;
                                                            activityRequestBatchJobLog += "Info: Activity Request contains " + marketingListLeads.Count + " leads " + Environment.NewLine;
                                                            foreach (var lead in marketingListLeads)
                                                            {
                                                                try
                                                                {
                                                                    var leadDetails = objCommon.GetLeadDetails(objCommon.GetAccessToken(authority, clientId, secret, serviceUrl), serviceUrl, clientId, secret, lead.leadId);
                                                                    if (leadDetails.stateCode == "0")
                                                                    {

                                                                        if ((leadDetails.otherPhone == "" && leadDetails.cellPhone == null) || (leadDetails.otherPhone == null && leadDetails.cellPhone == "") || (leadDetails.otherPhone == null && leadDetails.cellPhone == null) || (leadDetails.cellPhone == "" && leadDetails.otherPhone == ""))
                                                                        {
                                                                            activityRequestBatchJobLog += "Info:Failed to Create SMS Activity, Other Phone and Mobile Phone are null for :" + lead.leadName + "" + Environment.NewLine;
                                                                            //JObject annotationObject = new JObject();
                                                                            //annotationObject["subject"] = "Failed to Create SMS Activity: Lead Other Phone and Mobile Phone are NULL for Lead:" + leadDetails.leadName + "";
                                                                            //annotationObject["objectid_ims_acscampaignrequest@odata.bind"] = "/ims_acscampaignrequests(" + Guid.Parse(activityRequest.activityRequestId) + ")";
                                                                            //var noteResponse = objCommon.CreateFailedRecord(annotationObject, authority, accessToken, serviceUrl, clientId, secret);
                                                                        }
                                                                        else if (leadDetails.otherPhone != null || leadDetails.cellPhone != null)
                                                                        {
                                                                            JObject smsObject = new JObject();
                                                                            if (activityRequest.smsText.Length > 400)
                                                                                smsObject["ims_messagetext"] = activityRequest.smsText.Substring(0, 399);
                                                                            else
                                                                                smsObject["ims_messagetext"] = activityRequest.smsText;
                                                                            smsObject["subject"] = activityRequest.activitySchedulerName;
                                                                            if (leadDetails.cellPhone != null && leadDetails.cellPhone != "")
                                                                                smsObject["ims_to"] = leadDetails.cellPhone;
                                                                            else if (leadDetails.otherPhone != null && leadDetails.otherPhone != "")
                                                                                smsObject["ims_to"] = leadDetails.otherPhone;
                                                                            smsObject["ims_autosend"] = true;
                                                                            // smsObject["ims_from"] = "9666226227";
                                                                            if (leadDetails.owningUser != null && leadDetails.owningUser != "")
                                                                                smsObject["ownerid@odata.bind"] = "/systemusers(" + Guid.Parse(leadDetails.owningUser) + ")";
                                                                            else if (leadDetails.owningTeam != null && leadDetails.owningTeam != "")
                                                                                smsObject["ownerid@odata.bind"] = "/teams(" + Guid.Parse(leadDetails.owningTeam) + ")";
                                                                            smsObject["regardingobjectid_lead@odata.bind"] = "/leads(" + Guid.Parse(lead.leadId) + ")";
                                                                            smsObject["scheduledend"] = DateTime.Now.ToUniversalTime().AddHours(12);
                                                                            //var isSmsActivityExisits = objCommon.CheckSmsctivityExistance(authority, serviceUrl, clientId, secret, "?$filter=_regardingobjectid_value eq " + lead.leadId + " and ims_smstext eq '" + activityRequest.smsText + "' and statuscode eq 2");

                                                                            // if (isSmsActivityExisits)
                                                                            // {
                                                                            // activityRequestBatchJobLog += "Info:Duplicate SMS Activity is exists for the lead:" + leadDetails.leadName + "" + Environment.NewLine;
                                                                            // failedSmsActivities = failedSmsActivities + 1;
                                                                            //}
                                                                            //if(!isSmsActivityExisits)
                                                                            // {
                                                                            var reult = objCommon.CreateSmsAsync(smsObject, authority, accessToken, serviceUrl, clientId, secret);
                                                                            successSmsActivities = successSmsActivities + 1;
                                                                            //}
                                                                        }
                                                                    }

                                                                    else if (leadDetails.stateCode != "0")
                                                                    {
                                                                        activityRequestBatchJobLog += "Info:" + leadDetails.leadName + " is Inactive to Create SMS Activity" + Environment.NewLine;
                                                                        failedSmsActivities = failedSmsActivities + 1;
                                                                        //JObject annotationObject = new JObject();
                                                                        //annotationObject["subject"] = "Failed to Create SMS Activity: Lead Is Inactive:" + leadDetails.leadName + "";
                                                                        //annotationObject["objectid_ims_acscampaignrequest@odata.bind"] = "/ims_acscampaignrequests(" + activityRequest.activityRequestId + ")";
                                                                        //var noteResponse = objCommon.CreateFailedRecord(annotationObject, authority, accessToken, serviceUrl, clientId, secret);
                                                                    }
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                    activityRequestBatchJobLog += "Error:SMS Activity Request Creation Failed for   " + lead.leadName + "" + Environment.NewLine;
                                                                    failedSmsActivities = failedSmsActivities + 1;
                                                                    //JObject annotationObject = new JObject();
                                                                    //annotationObject["subject"] = "Failed to Create SMS Activity: " + ex.Message + "" + lead.leadName + "";
                                                                    //annotationObject["objectid_ims_acscampaignrequest@odata.bind"] = "/ims_acscampaignrequests(" + Guid.Parse(activityRequest.activityRequestId.ToString()) + ")";
                                                                    //var noteResponse = objCommon.CreateFailedRecord(annotationObject, authority, accessToken, serviceUrl, clientId, secret);
                                                                }
                                                            }

                                                        }
                                                        else
                                                        {
                                                            activityRequestBatchJobLog += "Activity Request Marketing List Contains 0  Lead Members :" + activityRequest.activityRequestName + "" + Environment.NewLine;
                                                            failedSmsActivities = failedSmsActivities + 1;
                                                            //JObject annotationObject = new JObject();
                                                            //annotationObject["subject"] = "Marketing List Contains 0 Active Lead Memebers";
                                                            //annotationObject["objectid_ims_acscampaignrequest@odata.bind"] = "/ims_acscampaignrequests(" + Guid.Parse(activityRequest.activityRequestId) + ")";
                                                            //var noteResponse = objCommon.CreateFailedRecord(annotationObject, authority, accessToken, serviceUrl, clientId, secret);
                                                        }
                                                    }
                                                }

                                            }
                                            //Static ML
                                            else if (!Convert.ToBoolean(activityRequest.marketingListType))
                                            {
                                                if (dcConfigs.ContainsKey(Constants.CheckStaticMLSynchronization))
                                                {
                                                    var fetchXmlStaticMLSynchronization = dcConfigs[Constants.CheckStaticMLSynchronization];
                                                    if (fetchXmlStaticMLSynchronization.Contains("{marketingListId}") && fetchXmlStaticMLSynchronization.Contains("{activityType}"))
                                                    {
                                                        fetchXmlStaticMLSynchronization = fetchXmlStaticMLSynchronization.Replace("{activityType}", "176390001");
                                                        fetchXmlStaticMLSynchronization = fetchXmlStaticMLSynchronization.Replace("{marketingListId}", activityRequest.marketingListGuid);
                                                        var checkStaticMLSynchronization = objCommon.CheckStaticMLSynchronization(objCommon.GetAccessToken(authority, clientId, secret, serviceUrl), serviceUrl, clientId, secret, fetchXmlStaticMLSynchronization);
                                                        if (checkStaticMLSynchronization == 0)
                                                        {
                                                            if (dcConfigs.ContainsKey(Constants.StaticMarketingList_LeadsFetchXml))
                                                            {
                                                                activityRequestBatchJobLog += "Activity Request has Static Marketing List" + Environment.NewLine;
                                                                var fetchXmlActiveLeads = dcConfigs[Constants.StaticMarketingList_LeadsFetchXml];
                                                                if (fetchXmlActiveLeads.Contains("{marketingListGuid}"))
                                                                {
                                                                    int pageNumber = 1;
                                                                    string pagingCookie = string.Empty;
                                                                    fetchXmlActiveLeads = fetchXmlActiveLeads.Replace("{marketingListGuid}", activityRequest.marketingListGuid);
                                                                    var marketingListLeads = objCommon.GetLeads(pageNumber, pagingCookie, objCommon.GetAccessToken(authority, clientId, secret, serviceUrl), serviceUrl, clientId, secret, fetchXmlActiveLeads);
                                                                    if (marketingListLeads.Count > 0)
                                                                    {
                                                                        totalLeadsUnderMarketingList = marketingListLeads.Count;
                                                                        activityRequestBatchJobLog += "<<*************************************************>>" + Environment.NewLine;
                                                                        activityRequestBatchJobLog += "Info: Activity Request contains " + marketingListLeads.Count + " leads " + Environment.NewLine;
                                                                        foreach (var lead in marketingListLeads)
                                                                        {
                                                                            try
                                                                            {
                                                                                if (lead.otherPhone == null && lead.cellPhone == null)
                                                                                {
                                                                                    activityRequestBatchJobLog += "Info:Failed to Create SMS Activity,Other Phone and Mobile Phone are null for :" + lead.leadName + "" + Environment.NewLine;
                                                                                    //JObject annotationObject = new JObject();
                                                                                    //annotationObject["subject"] = "Failed to Create SMS Activity: Lead Other Phone and Mobile Phone are NULL for Lead:" + lead.leadName + "";
                                                                                    //annotationObject["objectid_ims_acscampaignrequest@odata.bind"] = "/ims_acscampaignrequests(" + Guid.Parse(activityRequest.activityRequestId) + ")";
                                                                                    //var noteResponse = objCommon.CreateFailedRecord(annotationObject, authority, accessToken, serviceUrl, clientId, secret);
                                                                                }

                                                                                else if (lead.otherPhone != null || lead.cellPhone != null)
                                                                                {
                                                                                    JObject smsObject = new JObject();
                                                                                    if (activityRequest.smsText.Length > 400)
                                                                                        smsObject["ims_messagetext"] = activityRequest.smsText.Substring(0, 399);
                                                                                    else
                                                                                        smsObject["ims_messagetext"] = activityRequest.smsText;
                                                                                    smsObject["subject"] = activityRequest.activitySchedulerName;
                                                                                    if (lead.cellPhone != null)
                                                                                        smsObject["ims_to"] = lead.cellPhone;
                                                                                    else if (lead.otherPhone != null)
                                                                                        smsObject["ims_to"] = lead.otherPhone;
                                                                                    smsObject["ims_autosend"] = true;
                                                                                    //smsObject["ims_from"] = "9666226227";
                                                                                    if (lead.owningUser != null && lead.owningUser != "")
                                                                                        smsObject["ownerid@odata.bind"] = "/systemusers(" + Guid.Parse(lead.owningUser) + ")";
                                                                                    else if (lead.owningTeam != null && lead.owningTeam != "")
                                                                                        smsObject["ownerid@odata.bind"] = "/teams(" + Guid.Parse(lead.owningTeam) + ")";
                                                                                    smsObject["regardingobjectid_lead@odata.bind"] = "/leads(" + Guid.Parse(lead.leadId) + ")";
                                                                                    smsObject["scheduledend"] = DateTime.Now.ToUniversalTime().AddHours(12);
                                                                                    //var isSmsActivityExisits = objCommon.CheckSmsctivityExistance(authority, serviceUrl, clientId, secret, "?$filter=_regardingobjectid_value eq " + lead.leadId + " and ims_smstext eq '" + activityRequest.smsText + "' and statuscode eq 2");
                                                                                    // if (isSmsActivityExisits)
                                                                                    // {
                                                                                    //     activityRequestBatchJobLog += "Info:Duplicate SMS Activity exists for the lead :" + lead.leadName + "" + Environment.NewLine;
                                                                                    //     failedSmsActivities = failedSmsActivities + 1;
                                                                                    // }
                                                                                    // if (!isSmsActivityExisits)
                                                                                    //{
                                                                                    var reult = objCommon.CreateSmsAsync(smsObject, authority, accessToken, serviceUrl, clientId, secret);
                                                                                    successSmsActivities = successSmsActivities + 1;
                                                                                    //}
                                                                                }
                                                                            }
                                                                            catch (Exception ex)
                                                                            {
                                                                                activityRequestBatchJobLog += "Error:SMS Activity Request Creation Failed for   " + lead.leadName + "" + Environment.NewLine;
                                                                                failedSmsActivities = failedSmsActivities + 1;
                                                                                //JObject annotationObject = new JObject();
                                                                                //annotationObject["subject"] = "Failed to Create SMS Activity: " + ex.Message + "" + lead.leadName + "";
                                                                                //annotationObject["objectid_ims_acscampaignrequest@odata.bind"] = "/ims_acscampaignrequests(" + Guid.Parse(activityRequest.activityRequestId.ToString()) + ")";
                                                                                //var noteResponse = objCommon.CreateFailedRecord(annotationObject, authority, accessToken, serviceUrl, clientId, secret);
                                                                            }
                                                                        }

                                                                        //activityRequestBatchJobLog += "<<******************Completed**************>>" + Environment.NewLine;
                                                                        //activityRequestBatchJobLog += "Total Failed Leads to Create SMS Activities:" + failedSmsActivities + "" + Environment.NewLine;
                                                                        //JObject activityRequestObj = new JObject();
                                                                        //activityRequestObj["ims_issynced"] = true;
                                                                        //activityRequestObj["ims_batchjob@odata.bind"] = "/ims_batchjobs(" + activityRequestBatchJobId + ")";
                                                                        //var activityRequestIsSyncUpdate = objCommon.UpdateActivityRequest(new Guid(activityRequest.activityRequestId), activityRequestObj, authority, objCommon.GetAccessToken(authority, clientId, secret, serviceUrl), serviceUrl, clientId, secret);
                                                                    }
                                                                    else
                                                                    {
                                                                        activityRequestBatchJobLog += "Info:Activity Request Marketing List Contains 0 Active Leads :" + activityRequest.activityRequestName + "" + Environment.NewLine;
                                                                        failedSmsActivities = failedSmsActivities + 1;
                                                                        //JObject annotationObject = new JObject();
                                                                        //annotationObject["subject"] = "Marketing List Contains 0 Active Lead Memebers";
                                                                        //annotationObject["objectid_ims_acscampaignrequest@odata.bind"] = "/ims_acscampaignrequests(" + Guid.Parse(activityRequest.activityRequestId) + ")";
                                                                        //var noteResponse = objCommon.CreateFailedRecord(annotationObject, authority, accessToken, serviceUrl, clientId, secret);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            activityRequestBatchJobLog += "Activity Request has Static Marketing List and it is been already process from previous batch job sessions" + Environment.NewLine;
                                                        }
                                                    }
                                                    else
                                                        activityRequestBatchJobLog += "Please check the Static ML Synchronization configuration" + Environment.NewLine;
                                                }
                                                else
                                                {
                                                    activityRequestBatchJobLog += "Please check the Static ML Synchronization configuration" + Environment.NewLine;
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        activityRequestBatchJobLog += "Activity Request Failed to Process" + ex.Message + "" + Environment.NewLine;
                                        totalFailedRequests = totalFailedRequests + 1;
                                        //activitiyRequestsFailed = activitiyRequestsFailed + 1;
                                        //JObject annotationObject = new JObject();
                                        //annotationObject["subject"] = "Error in Processing Activity Request:"+ ex.Message +":" + activityRequest.marketingListName + "";
                                        //annotationObject["objectid_ims_batchjob@odata.bind"] = "/ims_batchjobs(" + batchJobRecordId + ")";
                                        //var noteResponse = objCommon.CreateFailedRecord(annotationObject, authority, accessToken, serviceUrl, clientId, secret);
                                    }
                                    activityRequestBatchJobLog += "<<******************Completed**************>>" + Environment.NewLine;
                                    activityRequestBatchJobLog += "Total Number of leads for which the creation of SMS activity failed : " + failedSmsActivities + "" + Environment.NewLine;
                                    JObject activityRequestObj = new JObject();
                                    activityRequestObj["ims_issynced"] = true;
                                    activityRequestObj["ims_batchjob@odata.bind"] = "/ims_batchjobs(" + activityRequestBatchJobId + ")";
                                    var activityRequestIsSyncUpdate = objCommon.UpdateActivityRequest(new Guid(activityRequest.activityRequestId), activityRequestObj, authority, objCommon.GetAccessToken(authority, clientId, secret, serviceUrl), serviceUrl, clientId, secret);
                                    //Logging Infomation in Note pad as attachment in activity scheduer job
                                    if (failedSmsActivities > 0 && successSmsActivities == 0)
                                    {
                                        //Failed
                                        Task.WaitAll(Task.Run(async () => await Common.UpdateBatchJobStatusAsync(authority, clientId, secret, serviceUrl, activityRequestBatchJobId, batcjJobStatus, 176390001, activityRequestBatchJobLog, activityRequest.activityRequestName + "Log-" + DateTime.Now + "")));
                                    }
                                    else if (failedSmsActivities == 0 && successSmsActivities == totalLeadsUnderMarketingList)
                                    {
                                        //Completed
                                        Task.WaitAll(Task.Run(async () => await Common.UpdateBatchJobStatusAsync(authority, clientId, secret, serviceUrl, activityRequestBatchJobId, batcjJobStatus, 176390000, activityRequestBatchJobLog, activityRequest.activityRequestName + "Log-" + DateTime.Now + "")));
                                    }
                                    else if (failedSmsActivities > 0 && successSmsActivities != totalLeadsUnderMarketingList)
                                    {
                                        //Partially Completed
                                        Task.WaitAll(Task.Run(async () => await Common.UpdateBatchJobStatusAsync(authority, clientId, secret, serviceUrl, activityRequestBatchJobId, batcjJobStatus, 176390002, activityRequestBatchJobLog, activityRequest.activityRequestName + "Log-" + DateTime.Now + "")));
                                    }
                                }
                            }   
                        }
                        else
                        {
                            batchJobLog += "No configuration found for SMS Activity , Please check the configuration" + Environment.NewLine;
                        }
                    }
                    else
                    {
                        batchJobLog += "No configuration found for SMS Activity Job" + Environment.NewLine;
                    }
                }
            }
            catch (Exception ex)
            {
                batcjJobStatus = false;
                batchJobStatusReason = 176390001;
                batchJobLog += Environment.NewLine + "Error @ Main Method " + ex.Message + Environment.NewLine;
            }
            finally
            {
                batchJobLog += "Total Activity Requests Found to Process  in CRM:" + totalActivityRequests.ToString() + Environment.NewLine;
                batchJobLog += "Total Activity Requests Processed  in CRM:" + (totalActivityRequests-totalFailedRequests) + Environment.NewLine;
                batchJobLog += "Total Activity Requests Failed to Process  in CRM:" + (totalFailedRequests) + Environment.NewLine;
                batchJobLog += "---Batch Job Ended---" + DateTime.Now + Environment.NewLine;
                batchJobLog += "<<=================================================>>" + Environment.NewLine;
                //Update Log record
                if(totalFailedRequests>0)
                {
                    batchJobStatusReason = 176390002;
                }
                Task.WaitAll(Task.Run(async () => await Common.UpdateBatchJobStatusAsync(authority, clientId, secret, serviceUrl, batchJobRecordId, batcjJobStatus, batchJobStatusReason,batchJobLog, Constants.batchJobName)));
            }
        }

        
    }
    public class ActivityRequestResponse
    {
        public string activityRequestName { get; set; }
        public string activityRequestId { get; set; }
        public string marketingListName { get; set; }
        public string marketingListGuid { get; set; }
        public string smsText { get; set; }
        public string marketingListType { get; set; }

        public string marketingMemberType { get; set; }

        public string activitySchedulerName { get; set; }
    }
    public class GenericDto
    {
        [JsonProperty("@Microsoft.Dynamics.CRM.fetchxmlpagingcookie")]
        public string Cookie { get; set; }

    }
    public class LeadRequestResponse
    {
        public string leadId { get; set; }
        public string ownerId { get; set; }

        public string owningUser { get; set; }
        public string owningTeam { get; set; }
        public string cellPhone { get; set; }
        public string otherPhone { get; set; }
        public string leadName { get; set; }
        public string stateCode { get; set; }
    }
    public class DynamicMarketingListQueryRequest
    {
        public string query { get; set; }

    }
}
