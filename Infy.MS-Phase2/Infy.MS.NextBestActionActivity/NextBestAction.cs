using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infy.MS.NextBestActionActivity
{
    public class NextBestAction
    {
        static void Main(string[] args)
        {
            string marketingListType = string.Empty;
            if (args.Count() > 0)
            {
                var argument = args[0];
                if (argument.Contains("-"))
                {
                    var splitArg = argument.Split('-');
                    if (splitArg.Count() > 0)
                    {
                        
                    }
                }
                marketingListType = argument;
            }
            //fetch Azure configurations from App.config to GetAccessToken
            string authority = ConfigurationManager.AppSettings[Constants.Authority].ToString();
            string clientId = ConfigurationManager.AppSettings[Constants.ClientId].ToString();
            string secret = ConfigurationManager.AppSettings[Constants.Secret].ToString();
            string serviceUrl = ConfigurationManager.AppSettings[Constants.ServiceUrl].ToString();
            //string jobFrequency = ConfigurationManager.AppSettings[Constants.jobFreqency].ToString();
            string keyVaultUrl = ConfigurationManager.AppSettings["KeyVaultUrl"].ToString();
            string clientIdSecretName = ConfigurationManager.AppSettings["clientIdSecretName"].ToString();
            string cientSecretName = ConfigurationManager.AppSettings["ClientSecretName"].ToString();

            Common objCommon = new Common();
           // clientId = objCommon.GetSecretValue(keyVaultUrl, clientIdSecretName).Result;
            Console.WriteLine("Azure key Vault ClientId secret Value:" + clientId);
            //secret = objCommon.GetSecretValue(keyVaultUrl, cientSecretName).Result;
            Console.WriteLine("Azure key Vault Client secret Value:" + secret);
            string batchJobLog = string.Empty;
            bool batchJobStatus = true;
            int batchJobStatusReason = 176390000;
            int totalActivityRequests = 0;
            int totalFailedRequests = 0;
            Dictionary<string, string> dcConfigs = null;
            Dictionary<string, string> dcConfigs_ActivityScheduler = null;
            string accessToken = string.Empty;
            batchJobLog += "---Batch Job Started---" + DateTime.Now + Environment.NewLine;
            batchJobLog += "<<=================================================>>" + Environment.NewLine;
            //batchJobLog += "Job Frequency " + jobFrequency + Environment.NewLine;
            Guid batchJobRecordId = Common.CreateBatchJobRecord(authority, clientId, secret, serviceUrl, Constants.batchJobName).Result;
            batchJobLog += "Batch Job Record Created" + Environment.NewLine;
            try
            {
                
                accessToken = objCommon.GetAccessToken(authority, clientId, secret, serviceUrl);
                if (!string.IsNullOrEmpty(accessToken))
                {
                    batchJobLog += "Access Token retrieved" + Environment.NewLine;
                    dcConfigs = objCommon.FetchConfigurations(accessToken, serviceUrl, Constants.configSetUpName);
                    dcConfigs_ActivityScheduler = objCommon.FetchConfigurations(accessToken, serviceUrl, Constants.configSetUpName_ActivityScheduler);
                    if (dcConfigs != null && dcConfigs.Count > 0)
                    {
                        batchJobLog += "Configuration exist in CRM for NBA Activity" + Environment.NewLine;
                        if (dcConfigs.ContainsKey(Constants.ActivityRequestNBATypeFetchXml))
                        {
                            var fetchXmlActivityRequests = dcConfigs[Constants.ActivityRequestNBATypeFetchXml];
                            if (fetchXmlActivityRequests.Contains("{createdfromcode}") && marketingListType != string.Empty)
                            {
                                fetchXmlActivityRequests = fetchXmlActivityRequests.Replace("{createdfromcode}", marketingListType);
                                var activityRequestResponses = objCommon.GetActivityRequests(objCommon.GetAccessToken(authority, clientId, secret, serviceUrl), serviceUrl, clientId, secret, fetchXmlActivityRequests);
                                if (activityRequestResponses.Count > 0)
                                {
                                    totalActivityRequests = activityRequestResponses.Count;
                                    foreach (var activityRequest in activityRequestResponses)
                                    {
                                        string activityRequestBatchJobLog = string.Empty;
                                        int failedNbaActivities = 0;
                                        int successNbaActivities = 0;
                                        int totalLeadsUnderMarketingList = 0;
                                        activityRequestBatchJobLog += "-------------" + dcConfigs[Constants.ActivityRequestJob_Started_Message] + "-----------" + DateTime.Now + Environment.NewLine;
                                        activityRequestBatchJobLog += "<<=================================================>>" + Environment.NewLine;
                                        Guid activityRequestBatchJobId = Common.CreateBatchJobRecord(authority, clientId, secret, serviceUrl, activityRequest.activityRequestName).Result;
                                        activityRequestBatchJobLog += dcConfigs[Constants.ActivityRequestJob_Created_Message] + Environment.NewLine;
                                        try
                                        {
                                            if (!string.IsNullOrEmpty(activityRequest.marketingListType))
                                            {
                                                //Dynamic ML
                                                if (Convert.ToBoolean(activityRequest.marketingListType))
                                                {
                                                    activityRequestBatchJobLog += dcConfigs[Constants.ActivityRequestMarketingListType_Message_Dynamic] + Environment.NewLine;
                                                    var dynamicMarketingListQueryRequest = objCommon.GetDynamicMarketingListQuery(accessToken, serviceUrl, clientId, secret, activityRequest.marketingListGuid);
                                                    if (dynamicMarketingListQueryRequest != null)
                                                    {
                                                        if (dynamicMarketingListQueryRequest.query != null)
                                                        {
                                                            string str = "logical";
                                                            string s = "mapping=" + str + "";
                                                            if (dynamicMarketingListQueryRequest.query.Contains(s))
                                                            {
                                                                dynamicMarketingListQueryRequest.query = dynamicMarketingListQueryRequest.query.Replace(s, "mapping='logical' page='{0}' count='5000'");
                                                            }
                                                            int pageNumber = 1;
                                                            string pagingCookie = string.Empty;
                                                            List<ContactRequestResponse> marketingListContactRecords = new List<ContactRequestResponse>();
                                                            List<LeadRequestResponse> marketingListLeadRecords = new List<LeadRequestResponse>();
                                                            if (marketingListType == "4")
                                                                marketingListLeadRecords = objCommon.GetLeads(pageNumber, pagingCookie, objCommon.GetAccessToken(authority, clientId, secret, serviceUrl), serviceUrl, clientId, secret, dynamicMarketingListQueryRequest.query);
                                                            else if(marketingListType=="2")
                                                                marketingListContactRecords = objCommon.GetContacts(pageNumber, pagingCookie, objCommon.GetAccessToken(authority, clientId, secret, serviceUrl), serviceUrl, clientId, secret, dynamicMarketingListQueryRequest.query);
                                                            if (marketingListLeadRecords.Count > 0)
                                                            {
                                                                totalLeadsUnderMarketingList = marketingListLeadRecords.Count;
                                                                activityRequestBatchJobLog += "<<*************************************************>>" + Environment.NewLine;
                                                                activityRequestBatchJobLog += string.Format(dcConfigs[Constants.ActivityRequestJob_MarketingListLeadCount_Message], totalLeadsUnderMarketingList) + Environment.NewLine;

                                                                foreach (var lead in marketingListLeadRecords)
                                                                {

                                                                    try
                                                                    {
                                                                        var leadDetails = objCommon.GetLeadDetails(objCommon.GetAccessToken(authority, clientId, secret, serviceUrl), serviceUrl, clientId, secret, lead.leadId);
                                                                        if (leadDetails.stateCode == "0")
                                                                        {
                                                                            JObject nbaObject = new JObject();
                                                                            if (!string.IsNullOrEmpty(activityRequest.nbaText))
                                                                            {
                                                                                string nbaText = BuildNbaText(leadDetails.leadName, activityRequest.nbaText, dcConfigs_ActivityScheduler,Constants.ActivityScheduler_LeadName);
                                                                                nbaObject["subject"] = nbaText;
                                                                            }
                                                                            if (!string.IsNullOrEmpty(activityRequest.nbaSla))
                                                                                nbaObject["ims_activetillxdays"] = Convert.ToInt32(activityRequest.nbaSla);
                                                                            if (leadDetails.owningUser != null && leadDetails.owningUser != "")
                                                                                nbaObject["ownerid@odata.bind"] = "/systemusers(" + Guid.Parse(leadDetails.owningUser) + ")";
                                                                            else if (leadDetails.owningTeam != null && leadDetails.owningTeam != "")
                                                                                nbaObject["ownerid@odata.bind"] = "/teams(" + Guid.Parse(leadDetails.owningTeam) + ")";
                                                                            nbaObject["regardingobjectid_lead@odata.bind"] = "/leads(" + Guid.Parse(lead.leadId) + ")";
                                                                            if (!string.IsNullOrEmpty(activityRequest.nbaSla))
                                                                                nbaObject["scheduledend"] = objCommon.AddBusinessDays(DateTime.Now, (Convert.ToInt32(activityRequest.nbaSla) - 1));// DateTime.Now.ToUniversalTime().AddDays(Convert.ToInt32(activityRequest.nbaSla)-1);
                                                                            else
                                                                                nbaObject["scheduledend"] = DateTime.Now;
                                                                            if (nbaObject.ContainsKey("subject") && !string.IsNullOrEmpty(lead.leadId))
                                                                            {
                                                                                var isNbaActivityExisits = objCommon.CheckNbaActivityExistance(authority, serviceUrl, clientId, secret, "?$filter=_regardingobjectid_value eq " + lead.leadId + " and subject eq '" + nbaObject["subject"] + "' and statuscode eq 1");
                                                                                if (!isNbaActivityExisits.recordExisted)
                                                                                {
                                                                                    var isNbaExistWithOpen = objCommon.CheckNbaActivityExistance(authority, serviceUrl, clientId, secret, "?$filter=_regardingobjectid_value eq " + lead.leadId + " and subject ne '" + nbaObject["subject"] + "' and statuscode eq 1");
                                                                                    if (isNbaExistWithOpen.recordExisted == true && !string.IsNullOrEmpty(isNbaExistWithOpen.nbaActivityId))
                                                                                    {
                                                                                        JObject closeNbaObject = new JObject();
                                                                                        nbaObject["statuscode"] = Convert.ToInt32(2);
                                                                                        nbaObject["statecode"] = Convert.ToInt32(1);
                                                                                        if (dcConfigs.ContainsKey(Constants.NBAPreClosingReason))
                                                                                            nbaObject["ims_reasonforclosure"] = dcConfigs[Constants.NBAPreClosingReason];
                                                                                        Task.WaitAll(Task.Run(async () => await new Common().CloseNextBestAction(isNbaExistWithOpen.nbaActivityId, nbaObject, authority, serviceUrl, clientId, secret)));
                                                                                        batchJobLog += "Pre Closed Activity" + Environment.NewLine;
                                                                                    }
                                                                                    var reult = objCommon.CreateNbaAsync(nbaObject, authority, accessToken, serviceUrl, clientId, secret);
                                                                                    if (!string.IsNullOrEmpty(lead.leadId) && nbaObject.ContainsKey("subject"))
                                                                                    {
                                                                                        JObject leadEntity = new JObject();
                                                                                        leadEntity["ims_nextbestactionmessage"] = nbaObject["subject"];
                                                                                        var response = objCommon.UpdateLeadNextBestActionAttribute(lead.leadId, leadEntity, authority, serviceUrl, clientId, secret);
                                                                                        //Task.WaitAll(Task.Run(async () => await new Common().UpdateLeadNextBestActionAttribute(lead.leadId, leadEntity, authority, serviceUrl, clientId, secret)));

                                                                                    }
                                                                                    successNbaActivities = successNbaActivities + 1;

                                                                                }
                                                                                else if (isNbaActivityExisits.recordExisted)
                                                                                {
                                                                                    activityRequestBatchJobLog += string.Format(dcConfigs[Constants.ActivityRequestJob_NBAActivityDuplicate_Message], leadDetails.leadName) + Environment.NewLine;
                                                                                    failedNbaActivities = failedNbaActivities + 1;
                                                                                }
                                                                            }
                                                                        }
                                                                        else if (leadDetails.stateCode != "0")
                                                                        {
                                                                            activityRequestBatchJobLog += string.Format(dcConfigs[Constants.ActivityRequestJob_LeadInactive_Message], leadDetails.leadName) + Environment.NewLine;
                                                                            failedNbaActivities = failedNbaActivities + 1;
                                                                            //JObject annotationObject = new JObject();
                                                                            //annotationObject["subject"] = "Failed to Create NBA Activity: Lead Is Inactive:" + leadDetails.leadName + "";
                                                                            //annotationObject["objectid_ims_acscampaignrequest@odata.bind"] = "/ims_acscampaignrequests(" + Guid.Parse(activityRequest.activityRequestId.ToString()) + ")";
                                                                            //var noteResponse = objCommon.CreateFailedRecordAsync(annotationObject, authority, accessToken, serviceUrl, clientId, secret);
                                                                        }

                                                                    }
                                                                    catch (Exception ex)
                                                                    {
                                                                        activityRequestBatchJobLog += string.Format(dcConfigs[Constants.ActivityRequest_NBAActivityCreationFailed_ErrorMessage], lead.leadName) + Environment.NewLine;
                                                                        failedNbaActivities = failedNbaActivities + 1;
                                                                        //JObject annotationObject = new JObject();
                                                                        //annotationObject["subject"] = "Failed to Create NBA Activity: " + ex.Message + ":" + lead.leadName + "";
                                                                        //annotationObject["objectid_ims_acscampaignrequest@odata.bind"] = "/ims_acscampaignrequests(" + Guid.Parse(activityRequest.activityRequestId.ToString()) + ")";
                                                                        //var noteResponse = objCommon.CreateFailedRecordAsync(annotationObject, authority, accessToken, serviceUrl, clientId, secret);
                                                                    }
                                                                }

                                                            }
                                                            else if(marketingListContactRecords.Count > 0)
                                                            {
                                                                foreach (var contact in marketingListContactRecords)
                                                                {

                                                                    try
                                                                    {
                                                                        var contactDetails = objCommon.GetContactDetails(objCommon.GetAccessToken(authority, clientId, secret, serviceUrl), serviceUrl, clientId, secret, contact.contactId);
                                                                        if (contactDetails.stateCode == "0")
                                                                        {
                                                                            JObject nbaObject = new JObject();
                                                                            if (!string.IsNullOrEmpty(activityRequest.nbaText))
                                                                            {
                                                                                string nbaText = BuildNbaText(contactDetails.contactName, activityRequest.nbaText, dcConfigs_ActivityScheduler,Constants.ActivityScheduler_ContactName);
                                                                                nbaObject["subject"] = nbaText;
                                                                            }
                                                                            if (!string.IsNullOrEmpty(activityRequest.nbaSla))
                                                                                nbaObject["ims_activetillxdays"] = Convert.ToInt32(activityRequest.nbaSla);
                                                                            if (contactDetails.owningUser != null && contactDetails.owningUser != "")
                                                                                nbaObject["ownerid@odata.bind"] = "/systemusers(" + Guid.Parse(contactDetails.owningUser) + ")";
                                                                            else if (contactDetails.owningTeam != null && contactDetails.owningTeam != "")
                                                                                nbaObject["ownerid@odata.bind"] = "/teams(" + Guid.Parse(contactDetails.owningTeam) + ")";
                                                                            nbaObject["regardingobjectid_contact@odata.bind"] = "/contacts(" + Guid.Parse(contact.contactId) + ")";
                                                                            if (!string.IsNullOrEmpty(activityRequest.nbaSla))
                                                                                nbaObject["scheduledend"] = objCommon.AddBusinessDays(DateTime.Now, (Convert.ToInt32(activityRequest.nbaSla) - 1));// DateTime.Now.ToUniversalTime().AddDays(Convert.ToInt32(activityRequest.nbaSla)-1);
                                                                            else
                                                                                nbaObject["scheduledend"] = DateTime.Now;
                                                                            if (nbaObject.ContainsKey("subject") && !string.IsNullOrEmpty(contact.contactId))
                                                                            {
                                                                                var isNbaActivityExisits = objCommon.CheckNbaActivityExistance(authority, serviceUrl, clientId, secret, "?$filter=_regardingobjectid_value eq " + contact.contactId + " and subject eq '" + nbaObject["subject"] + "' and statuscode eq 1");
                                                                                if (!isNbaActivityExisits.recordExisted)
                                                                                {
                                                                                    var isNbaExistWithOpen = objCommon.CheckNbaActivityExistance(authority, serviceUrl, clientId, secret, "?$filter=_regardingobjectid_value eq " + contact.contactId + " and subject ne '" + nbaObject["subject"] + "' and statuscode eq 1");
                                                                                    if (isNbaExistWithOpen.recordExisted == true && !string.IsNullOrEmpty(isNbaExistWithOpen.nbaActivityId))
                                                                                    {
                                                                                        JObject closeNbaObject = new JObject();
                                                                                        nbaObject["statuscode"] = Convert.ToInt32(2);
                                                                                        nbaObject["statecode"] = Convert.ToInt32(1);
                                                                                        if (dcConfigs.ContainsKey(Constants.NBAPreClosingReason))
                                                                                            nbaObject["ims_reasonforclosure"] = dcConfigs[Constants.NBAPreClosingReason];
                                                                                        Task.WaitAll(Task.Run(async () => await new Common().CloseNextBestAction(isNbaExistWithOpen.nbaActivityId, nbaObject, authority, serviceUrl, clientId, secret)));
                                                                                        batchJobLog += "Pre Closed Activity" + Environment.NewLine;
                                                                                    }
                                                                                    var reult = objCommon.CreateNbaAsync(nbaObject, authority, accessToken, serviceUrl, clientId, secret);
                                                                                    if (!string.IsNullOrEmpty(contact.contactId) && nbaObject.ContainsKey("subject"))
                                                                                    {
                                                                                        JObject leadEntity = new JObject();
                                                                                        leadEntity["ims_nextbestactionmessage"] = nbaObject["subject"];
                                                                                        var response = objCommon.UpdateLeadNextBestActionAttribute(contact.contactId, leadEntity, authority, serviceUrl, clientId, secret);
                                                                                        //Task.WaitAll(Task.Run(async () => await new Common().UpdateLeadNextBestActionAttribute(lead.leadId, leadEntity, authority, serviceUrl, clientId, secret)));

                                                                                    }
                                                                                    successNbaActivities = successNbaActivities + 1;

                                                                                }
                                                                                else if (isNbaActivityExisits.recordExisted)
                                                                                {
                                                                                    activityRequestBatchJobLog += string.Format(dcConfigs[Constants.ActivityRequestJob_NBAActivityDuplicate_Message], contact.contactName) + Environment.NewLine;
                                                                                    failedNbaActivities = failedNbaActivities + 1;
                                                                                }
                                                                            }
                                                                        }
                                                                        else if (contactDetails.stateCode != "0")
                                                                        {
                                                                            activityRequestBatchJobLog += string.Format(dcConfigs[Constants.ActivityRequestJob_LeadInactive_Message], contact.contactName) + Environment.NewLine;
                                                                            failedNbaActivities = failedNbaActivities + 1;
                                                                            //JObject annotationObject = new JObject();
                                                                            //annotationObject["subject"] = "Failed to Create NBA Activity: Lead Is Inactive:" + leadDetails.leadName + "";
                                                                            //annotationObject["objectid_ims_acscampaignrequest@odata.bind"] = "/ims_acscampaignrequests(" + Guid.Parse(activityRequest.activityRequestId.ToString()) + ")";
                                                                            //var noteResponse = objCommon.CreateFailedRecordAsync(annotationObject, authority, accessToken, serviceUrl, clientId, secret);
                                                                        }

                                                                    }
                                                                    catch (Exception ex)
                                                                    {
                                                                        activityRequestBatchJobLog += string.Format(dcConfigs[Constants.ActivityRequest_NBAActivityCreationFailed_ErrorMessage], contact.contactName) + Environment.NewLine;
                                                                        failedNbaActivities = failedNbaActivities + 1;
                                                                        //JObject annotationObject = new JObject();
                                                                        //annotationObject["subject"] = "Failed to Create NBA Activity: " + ex.Message + ":" + lead.leadName + "";
                                                                        //annotationObject["objectid_ims_acscampaignrequest@odata.bind"] = "/ims_acscampaignrequests(" + Guid.Parse(activityRequest.activityRequestId.ToString()) + ")";
                                                                        //var noteResponse = objCommon.CreateFailedRecordAsync(annotationObject, authority, accessToken, serviceUrl, clientId, secret);
                                                                    }
                                                                }
                                                            }
                                                            else if(marketingListContactRecords.Count==0 && marketingListLeadRecords.Count==0)
                                                            {
                                                                activityRequestBatchJobLog += string.Format(dcConfigs[Constants.ActivityRequest_ZeroMarketingMembers_InfoMessage], activityRequest.activityRequestName) + Environment.NewLine;
                                                                failedNbaActivities = failedNbaActivities + 1;

                                                                //JObject annotationObject = new JObject();
                                                                //annotationObject["subject"] = "Marketing List Contains 0 Active Lead Memebers";
                                                                //annotationObject["objectid_ims_acscampaignrequest@odata.bind"] = "/ims_acscampaignrequests(" + Guid.Parse(activityRequest.activityRequestId) + ")";
                                                                //var noteResponse = objCommon.CreateFailedRecordAsync(annotationObject, authority, accessToken, serviceUrl, clientId, secret);
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
                                                            fetchXmlStaticMLSynchronization = fetchXmlStaticMLSynchronization.Replace("{activityType}", "176390002");
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
                                                                            activityRequestBatchJobLog += "Info:Total Marketing Leads Found for the Marketing List are: " + totalLeadsUnderMarketingList + "" + Environment.NewLine;
                                                                            foreach (var lead in marketingListLeads)
                                                                            {
                                                                                try
                                                                                {
                                                                                    if (lead.ownerId != null || lead.owningTeam != null)
                                                                                    {
                                                                                        JObject nbaObject = new JObject();
                                                                                        if (activityRequest.nbaText.Length > 400)
                                                                                            nbaObject["subject"] = activityRequest.nbaText.Substring(0, 399);
                                                                                        else
                                                                                            nbaObject["subject"] = activityRequest.nbaText;
                                                                                        if (lead.owningUser != null && lead.owningUser != "")
                                                                                            nbaObject["ownerid@odata.bind"] = "/systemusers(" + Guid.Parse(lead.owningUser) + ")";
                                                                                        else if (lead.owningTeam != null && lead.owningTeam != "")
                                                                                            nbaObject["ownerid@odata.bind"] = "/teams(" + Guid.Parse(lead.owningTeam) + ")";
                                                                                        nbaObject["regardingobjectid_lead@odata.bind"] = "/leads(" + Guid.Parse(lead.leadId) + ")";
                                                                                        if (!string.IsNullOrEmpty(activityRequest.nbaSla))
                                                                                            nbaObject["scheduledend"] = objCommon.AddBusinessDays(DateTime.Now, (Convert.ToInt32(activityRequest.nbaSla) - 1));// DateTime.Now.ToUniversalTime().AddDays(Convert.ToInt32(activityRequest.nbaSla)-1);
                                                                                        else
                                                                                            nbaObject["scheduledend"] = DateTime.Now;
                                                                                        // var isNbaActivityExisits = objCommon.CheckNbaActivityExistance(authority, serviceUrl, clientId, secret, "?$filter=_regardingobjectid_value eq " + lead.leadId + " and subject eq '" + activityRequest.nbaText + "' and statuscode eq 1");
                                                                                        // if (!isNbaActivityExisits)
                                                                                        // {
                                                                                        var reult = objCommon.CreateNbaAsync(nbaObject, authority, accessToken, serviceUrl, clientId, secret);
                                                                                        if (!string.IsNullOrEmpty(lead.leadId) && nbaObject.ContainsKey("subject"))
                                                                                        {
                                                                                            JObject leadEntity = new JObject();
                                                                                            leadEntity["ims_nextbestactionmessage"] = nbaObject["subject"];
                                                                                            Task.WaitAll(Task.Run(async () => await new Common().UpdateLeadNextBestActionAttribute(lead.leadId, leadEntity, authority, serviceUrl, clientId, secret)));
                                                                                            // var leadResult = objCommon.
                                                                                        }
                                                                                        successNbaActivities = successNbaActivities + 1;
                                                                                        //}
                                                                                        // else if (isNbaActivityExisits)
                                                                                        // {
                                                                                        //  activityRequestBatchJobLog += "Info:Duplicate NBA Activity is exists for the lead:" + lead.leadName + "" + Environment.NewLine;
                                                                                        //  failedNbaActivities = failedNbaActivities + 1;
                                                                                        // }
                                                                                    }
                                                                                }
                                                                                catch (Exception ex)
                                                                                {
                                                                                    activityRequestBatchJobLog += "Error:NBA Activity Request Creation Failed for   " + lead.leadName + "" + Environment.NewLine;
                                                                                    failedNbaActivities = failedNbaActivities + 1;
                                                                                    //JObject annotationObject = new JObject();
                                                                                    //annotationObject["subject"] = "Failed to Create NBA Activity: " + ex.Message + "" + lead.leadName + "";
                                                                                    //annotationObject["objectid_ims_acscampaignrequest@odata.bind"] = "/ims_acscampaignrequests(" + Guid.Parse(activityRequest.activityRequestId) + ")";
                                                                                    //var noteResponse = objCommon.CreateFailedRecordAsync(annotationObject, authority, accessToken, serviceUrl, clientId, secret);
                                                                                }
                                                                            }
                                                                            //activityRequestBatchJobLog += "<<******************Completed**************>>" + Environment.NewLine;
                                                                            //activityRequestBatchJobLog += "Info:Total Failed Leads to Create NBA Activities:" + failedNbaActivities + "" + Environment.NewLine;
                                                                            //JObject activityRequestObj = new JObject();
                                                                            //activityRequestObj["ims_issynced"] = true;
                                                                            //activityRequestObj["ims_batchjob@odata.bind"] = "/ims_batchjobs(" + activityRequestBatchJobId + ")";
                                                                            //var activityRequestIsSyncUpdate = objCommon.UpdateActivityRequest(new Guid(activityRequest.activityRequestId), activityRequestObj, authority, objCommon.GetAccessToken(authority, clientId, secret, serviceUrl), serviceUrl, clientId, secret);
                                                                        }
                                                                        else
                                                                        {
                                                                            activityRequestBatchJobLog += "Info:Activity Request Marketing List Contains 0 Active Lead Memebers :" + activityRequest.activityRequestName + "" + Environment.NewLine;
                                                                            failedNbaActivities = failedNbaActivities + 1;

                                                                            //JObject annotationObject = new JObject();
                                                                            //annotationObject["subject"] = "Marketing List Contains 0 Active Lead Memebers";
                                                                            //annotationObject["objectid_ims_acscampaignrequest@odata.bind"] = "/ims_acscampaignrequests(" + Guid.Parse(activityRequest.activityRequestId) + ")";
                                                                            //var noteResponse = objCommon.CreateFailedRecordAsync(annotationObject, authority, accessToken, serviceUrl, clientId, secret);
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
                                                        activityRequestBatchJobLog += "Please check the Static ML Synchronization configuration" + Environment.NewLine;
                                                }
                                            }
                                        }

                                        catch (Exception ex)
                                        {
                                            activityRequestBatchJobLog += "Activity Request Failed to Process" + ex.Message + "" + Environment.NewLine;
                                            totalFailedRequests = totalFailedRequests + 1;
                                            //JObject annotationObject = new JObject();
                                            //annotationObject["subject"] = "Error in Processing Activity Request:" + ex.Message + ":" + activityRequest.marketingListName + "";
                                            //annotationObject["objectid_ims_batchjob@odata.bind"] = "/ims_batchjobs(" + Guid.Parse(batchJobRecordId.ToString()) + ")";
                                            //var noteResponse = objCommon.CreateFailedRecordAsync(annotationObject, authority, accessToken, serviceUrl, clientId, secret);
                                        }

                                        activityRequestBatchJobLog += "<<******************Completed**************>>" + Environment.NewLine;
                                        activityRequestBatchJobLog += string.Format(dcConfigs[Constants.ActivityRequestJob_TotalFailedActivities_Message], failedNbaActivities) + Environment.NewLine;
                                        JObject activityRequestObj = new JObject();
                                        activityRequestObj["ims_issynced"] = true;
                                        activityRequestObj["ims_batchjob@odata.bind"] = "/ims_batchjobs(" + activityRequestBatchJobId + ")";
                                        var activityRequestIsSyncUpdate = objCommon.UpdateActivityRequest(new Guid(activityRequest.activityRequestId), activityRequestObj, authority, objCommon.GetAccessToken(authority, clientId, secret, serviceUrl), serviceUrl, clientId, secret);

                                        //Logging Infomation in Note pad as attachment in activity scheduer job
                                        if (failedNbaActivities > 0 && successNbaActivities == 0)
                                        {
                                            //Failed
                                            Task.WaitAll(Task.Run(async () => await Common.UpdateBatchJobStatusAsync(authority, clientId, secret, serviceUrl, activityRequestBatchJobId, batchJobStatus, 176390001, activityRequestBatchJobLog, activityRequest.activityRequestName + "Log-" + DateTime.Now + "")));
                                        }
                                        else if (failedNbaActivities == 0 && successNbaActivities == totalLeadsUnderMarketingList)
                                        {
                                            //Completed
                                            Task.WaitAll(Task.Run(async () => await Common.UpdateBatchJobStatusAsync(authority, clientId, secret, serviceUrl, activityRequestBatchJobId, batchJobStatus, 176390000, activityRequestBatchJobLog, activityRequest.activityRequestName + "Log-" + DateTime.Now + "")));
                                        }
                                        else if (failedNbaActivities > 0 && successNbaActivities != totalLeadsUnderMarketingList)
                                        {
                                            //Partially Completed
                                            Task.WaitAll(Task.Run(async () => await Common.UpdateBatchJobStatusAsync(authority, clientId, secret, serviceUrl, activityRequestBatchJobId, batchJobStatus, 176390002, activityRequestBatchJobLog, activityRequest.activityRequestName + "Log-" + DateTime.Now + "")));
                                        }
                                    }
                                }
                            }
                            else
                                batchJobLog += "Please check the configuration in Logic App, Marketing List Target Entity is NULL" + Environment.NewLine;
                        }
                        else
                        {
                            batchJobLog += "No configuration found for SMS Activity , Please check the configuration" + Environment.NewLine;
                        }
                    }
                    else
                    {
                        batchJobLog += "No configuration found for NBA Activity Job" + Environment.NewLine;
                    }
                }
            }
            catch (Exception ex)
            {
                batchJobStatus = false;
                batchJobStatusReason = 176390001;
                batchJobLog += Environment.NewLine + "Error @ Main Method " + ex.Message + Environment.NewLine;
            }
            batchJobLog += "Total Activity Requests Found to Process  in CRM:" + totalActivityRequests.ToString() + Environment.NewLine;
            batchJobLog += "Total Activity Requests Processed  in CRM:" + (totalActivityRequests - totalFailedRequests) + Environment.NewLine;
            batchJobLog += "Total Activity Requests Failed to Process  in CRM:" + (totalFailedRequests) + Environment.NewLine;
            batchJobLog += "---Batch Job Ended---" + DateTime.Now + Environment.NewLine;
            batchJobLog += "<<=================================================>>" + Environment.NewLine;
            //Update Log record
            if (totalFailedRequests > 0)
            {
                batchJobStatusReason = 176390002;
            }
            Task.WaitAll(Task.Run(async () => await Common.UpdateBatchJobStatusAsync(authority, clientId, secret, serviceUrl, batchJobRecordId, batchJobStatus, batchJobStatusReason,batchJobLog, Constants.batchJobName)));
        }
        static void ActivityBatchJobLog(ref string activityRequestBatchJobLog)
        {
            activityRequestBatchJobLog += "---Activity Batch Job Started---" + DateTime.Now + Environment.NewLine;
            activityRequestBatchJobLog += "<<=================================================>>" + Environment.NewLine;
        }
        static string BuildNbaText(string fullName,string nbaText,Dictionary<string,string> config_activityScheduler,string subject)
        {
            string nbaTextDescription = string.Empty;
            if (nbaText.Contains(subject))
            {
                if (config_activityScheduler.ContainsKey(subject))
                {
                    var value = config_activityScheduler[subject];
                    if (value == "fullname")
                    {
                        nbaTextDescription = nbaText.Replace(subject, fullName);
                        return nbaTextDescription;
                    }
                }
            }
            return nbaTextDescription;
        }

        
    }

    public class ActivityRequestResponse
    {
        public string activityRequestId { get; set; }
        public string activityRequestName { get; set; }
        public string marketingListName { get; set; }
        public string marketingListGuid { get; set; }
        public string nbaText { get; set; }
        public string nbaSla { get; set; }
        public string marketingListType { get; set; }

        public string marketingMemberType { get; set; }
        public string activitySchedulerName { get; set; }
    }
    public class NBAActivity
    {
        public string nbaActivityId { get; set; } = "";
        public bool recordExisted { get; set; }
    }
    public class LeadRequestResponse
    {
        public string leadId { get; set; }
        public string ownerId { get; set; }

        public string owningUser { get; set; }
        public string owningTeam { get; set; }
        public string stateCode { get; set; }
        public string cellPhone { get; set; }
        public string otherPhone { get; set; }
        public string leadName { get; set; }
    }
    public class ContactRequestResponse
    {
        public string contactId { get; set; }
        public string ownerId { get; set; }

        public string owningUser { get; set; }
        public string owningTeam { get; set; }
        public string stateCode { get; set; }
        public string cellPhone { get; set; }
        public string otherPhone { get; set; }
        public string contactName { get; set; }
    }

    public class DynamicMarketingListQueryRequest
    {
        public string query { get; set; }

    }
    public class GenericDto
    {
        [JsonProperty("@Microsoft.Dynamics.CRM.fetchxmlpagingcookie")]
        public string Cookie { get; set; }

    }
}
