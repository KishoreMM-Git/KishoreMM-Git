using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infy.MS.ActivitySchedulerCreateActivityRequest
{
    class Program
    {
        static void Main(string[] args)
        {
            string jobFrequency = "176390000";
            string activityType = string.Empty;
            string marketingListType = string.Empty;
            if (args.Count() > 0)
            {
                var argument = args[0];
                if(argument.Contains("-"))
                {
                    var splitArg = argument.Split('-');
                    if(splitArg.Count()>0)
                    {
                        jobFrequency = splitArg[0];
                        activityType = splitArg[1];
                        marketingListType = splitArg[2];
                    }
                }
            }
            //fetch Azure configurations from App.config to GetAccessToken
            string authority = ConfigurationManager.AppSettings[Constants.Authority].ToString();
            string clientId =  ConfigurationManager.AppSettings[Constants.ClientId].ToString();
            string secret =  ConfigurationManager.AppSettings[Constants.Secret].ToString();
            string serviceUrl = ConfigurationManager.AppSettings[Constants.ServiceUrl].ToString();
            string keyVaultUrl = ConfigurationManager.AppSettings["KeyVaultUrl"].ToString();
            string clientIdSecretName = ConfigurationManager.AppSettings["clientIdSecretName"].ToString();
            string cientSecretName = ConfigurationManager.AppSettings["ClientSecretName"].ToString();

            string accessToken = string.Empty;
            Dictionary<string, string> dcConfigs = null;
            List<ActiveActivityScheduler> lstActiveActivitySchedulers = null;
            bool batcjJobStatus = true;
            string batchJobLog = string.Empty;
            Guid batchJobRecordId = Guid.Empty;
            string fetchXml = string.Empty;
            Common objCommon = new Common();
           //clientId = objCommon.GetSecretValue(keyVaultUrl, clientIdSecretName).Result;
            Console.WriteLine("Azure key Vault ClientId secret Value:" + clientId);
           //secret = objCommon.GetSecretValue(keyVaultUrl, cientSecretName).Result;
            Console.WriteLine("Azure key Vault Client secret Value:" + secret);
            int activityRequestCount = 0;
            batchJobLog += "---Batch Job Started---" + DateTime.Now + Environment.NewLine;
            batchJobLog += "---This is configured on ---" + jobFrequency + " basis" +Environment.NewLine;
            batchJobLog += "---This is configured on ---" + activityType + " basis" + Environment.NewLine;
            batchJobLog += "---This is configured on ---" + marketingListType + " basis" + Environment.NewLine;
            batchJobLog += "<<=================================================>>" + Environment.NewLine;

            try
            {
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
                        batchJobLog += "Configuration exist in CRM for Activity Scheduler" + Environment.NewLine;
                        if (activityType!=string.Empty && jobFrequency!=string.Empty&& marketingListType!=string.Empty)
                        {
                            if (dcConfigs.ContainsKey("GetActiveActivityScheduler_TimeInterval_ActivityType_MLType")) fetchXml = dcConfigs["GetActiveActivityScheduler_TimeInterval_ActivityType_MLType"].ToString();
                        }
                        else if (jobFrequency == "176390000" && activityType=="Null" && marketingListType=="Null")
                        {
                            if (dcConfigs.ContainsKey("GetActiveActivityScheduler_TimeInterval")) fetchXml = dcConfigs["GetActiveActivityScheduler_TimeInterval"].ToString();
                        }
                        else if (jobFrequency == "176390000" && activityType == "Null" && marketingListType == "Null")
                        {
                            if (dcConfigs.ContainsKey("GetActiveActivityScheduler_TimeInterval")) fetchXml = dcConfigs["GetActiveActivityScheduler_TimeInterval"].ToString();
                        }
                        if (!string.IsNullOrEmpty(fetchXml))
                        {
                            fetchXml = fetchXml.Replace("{TODAY}", String.Format("{0:yyyy-mm-dd}", DateTime.Now.ToShortDateString()));
                            fetchXml = fetchXml.Replace("{timeInterval}", jobFrequency);
                            if (fetchXml.Contains("{activityType}"))
                                fetchXml = fetchXml.Replace("{activityType}", activityType);
                            if(fetchXml.Contains("{createdfromcode}"))
                                fetchXml = fetchXml.Replace("{createdfromcode}", marketingListType);
                            // batchJobLog += fetchXml + Environment.NewLine;
                            lstActiveActivitySchedulers = objCommon.FetchActiveActivitySchedlers(authority, clientId, secret, serviceUrl, accessToken, dcConfigs, fetchXml);
                            if (lstActiveActivitySchedulers != null && lstActiveActivitySchedulers.Count > 0)
                            {
                                foreach (ActiveActivityScheduler objActiveActivityScheduler in lstActiveActivitySchedulers)
                                {
                                    try
                                    {
                                        Task.WaitAll(Task.Run(async () => await Common.CreateActivityRequest(authority, clientId, secret, serviceUrl, batchJobRecordId, Guid.Parse(objActiveActivityScheduler.ActivitySchedulerRecordId), Guid.Parse(objActiveActivityScheduler.MarketingListId), objActiveActivityScheduler.ActivityType)));
                                        activityRequestCount++;
                                        Console.WriteLine("Activity Request Record Created :"+ activityRequestCount);
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
                                batchJobLog += "No Activity Scheduler record found in CRM:" + Environment.NewLine;
                            }
                        }
                        else
                        {
                            batchJobLog += "Configuration GetActiveActivityScheduler missing. Please check configuration record in CRM." + Environment.NewLine;
                        }
                    }
                    else
                    {
                        batchJobLog += "No configuration found for Activity Scheduler. Please check configuration record in CRM." + Environment.NewLine;
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
            }
            finally
            {
                batchJobLog += "Total Activity Request Created in CRM:" + activityRequestCount.ToString() + Environment.NewLine;
                batchJobLog += "---Batch Job Ended---" + DateTime.Now + Environment.NewLine;
                batchJobLog += "<<=================================================>>" + Environment.NewLine;
                //Update Log record
                Task.WaitAll(Task.Run(async () => await Common.UpdateBatchJobStatus(authority, clientId, secret, serviceUrl, batchJobRecordId, batcjJobStatus, batchJobLog, Constants.batchJobName)));
            }
        }
    }
}
