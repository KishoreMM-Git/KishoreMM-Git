using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infy.MS.AutoCloseNBAActivities
{
    public class AutoCloseNextBestActionActivity
    {
        static void Main(string[] args)
        {
            //fetch Azure configurations from App.config to GetAccessToken
            string batchJobLog = string.Empty;
            string authority = ConfigurationManager.AppSettings[Constants.Authority].ToString();
            string clientId =  ConfigurationManager.AppSettings[Constants.ClientId].ToString();
            string secret =  ConfigurationManager.AppSettings[Constants.Secret].ToString();
            string serviceUrl = ConfigurationManager.AppSettings[Constants.ServiceUrl].ToString();
           // string jobFrequency = ConfigurationManager.AppSettings[Constants.jobFreqency].ToString();
            string keyVaultUrl = ConfigurationManager.AppSettings["KeyVaultUrl"].ToString();
            string clientIdSecretName = ConfigurationManager.AppSettings["clientIdSecretName"].ToString();
            string cientSecretName = ConfigurationManager.AppSettings["ClientSecretName"].ToString();
            int totalActivitiesFound = 0;
            int totalFailedActivities = 0;
            bool batchJobStatus = true;
            int batchJobStatusReason = 176390000;
            Common objCommon = new Common();
           // clientId = objCommon.GetSecretValue(keyVaultUrl, clientIdSecretName).Result;
            Console.WriteLine("Azure key Vault ClientId secret Value:" + clientId);
            //secret = objCommon.GetSecretValue(keyVaultUrl, cientSecretName).Result;
            Console.WriteLine("Azure key Vault Client secret Value:" + secret);
            batchJobLog += "---Batch Job Started---" + DateTime.Now + Environment.NewLine;
            batchJobLog += "<<=================================================>>" + Environment.NewLine;
            Guid batchJobRecordId = Common.CreateBatchJobRecord(authority, clientId, secret, serviceUrl, Constants.BatchJobName).Result;
            batchJobLog += "Batch Job Record Created" + Environment.NewLine;

            var accessToken = objCommon.GetAccessToken(authority, clientId, secret, serviceUrl);
            Dictionary<string,string> dcConfigs = objCommon.FetchConfigurations(accessToken, serviceUrl, Constants.ConfigSetupName);
            if (dcConfigs != null && dcConfigs.Count > 0)
            {
                if (dcConfigs.ContainsKey(Constants.OpenNextBestActionActivities))
                {
                    var fetchXmlOpenNextBestActions = dcConfigs[Constants.OpenNextBestActionActivities];
                    var nextBestActions = objCommon.GetOpenNextBestActions(serviceUrl, accessToken, fetchXmlOpenNextBestActions);
                    if(nextBestActions.Count>0)
                    {
                        totalActivitiesFound = nextBestActions.Count;
                        foreach(var nba in nextBestActions)
                        {
                            try
                            {
                                JObject nbaObject = new JObject();
                                nbaObject["statuscode"] = Convert.ToInt32(2);
                                nbaObject["statecode"] = Convert.ToInt32(1);
                                if (dcConfigs.ContainsKey(Constants.AutoCloseNBAMessage))
                                    nbaObject["ims_reasonforclosure"] = dcConfigs[Constants.AutoCloseNBAMessage];
                                Task.WaitAll(Task.Run(async () => await new Common().CloseNextBestAction(nba.ActivityId, nbaObject, authority, serviceUrl, clientId, secret)));
                                batchJobLog += "Auto Closed Activity" + Environment.NewLine;
                            }
                            catch(Exception ex)
                            {
                                totalFailedActivities = totalFailedActivities + 0;
                            }
                        }
                    }
                    batchJobLog += "Total NBA Activities Found to Process  in CRM:" + totalActivitiesFound.ToString() + Environment.NewLine;
                    batchJobLog += "Total NBA Activities Processed  in CRM:" + (totalActivitiesFound - totalFailedActivities) + Environment.NewLine;
                    batchJobLog += "Total NBA Activities Failed to Process  in CRM:" + (totalFailedActivities) + Environment.NewLine;
                    batchJobLog += "Batch Job Ended---" + DateTime.Now + Environment.NewLine;
                    batchJobLog += "<<================Batch Job Ende================>>" + Environment.NewLine;
                    if (totalFailedActivities > 0)
                    {
                        batchJobStatusReason = 176390002;
                    }
                    Task.WaitAll(Task.Run(async () => await Common.UpdateBatchJobStatusAsync(authority, clientId, secret, serviceUrl, batchJobRecordId, batchJobStatus, batchJobStatusReason, batchJobLog, Constants.BatchJobName)));
                }
            }
                    
        }
    }
}
