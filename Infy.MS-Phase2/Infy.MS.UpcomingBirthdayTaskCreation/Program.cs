using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Infy.MS.UpcomingBirthdayTaskCreation
{
    class Program
    {
        static void Main(string[] args)
        {
            //string authority = ConfigurationManager.AppSettings[Constants.Authority].ToString();
            //string clientId = ConfigurationManager.AppSettings[Constants.ClientId].ToString();
            //string secret = ConfigurationManager.AppSettings[Constants.Secret].ToString();
            //string serviceUrl = ConfigurationManager.AppSettings[Constants.ServiceUrl].ToString();
            int jobFrequency = -1;// Convert.ToInt32(ConfigurationManager.AppSettings[Constants.jobFreqency].ToString());


			string authority = ConfigurationManager.AppSettings[Constants.Authority].ToString();
			string clientId = ConfigurationManager.AppSettings[Constants.ClientId].ToString();
			string secret = ConfigurationManager.AppSettings[Constants.Secret].ToString();
			string serviceUrl = ConfigurationManager.AppSettings[Constants.ServiceUrl].ToString();
			string keyVaultUrl = ConfigurationManager.AppSettings["KeyVaultUrl"].ToString();
			string clientIdSecretName = ConfigurationManager.AppSettings["clientIdSecretName"].ToString();
			string cientSecretName = ConfigurationManager.AppSettings["ClientSecretName"].ToString();

			string accessToken = string.Empty;
            Dictionary<string, string> dcConfigs = null;
            //List<ActiveLead> lstActiveLeads = null;
            bool batcjJobStatus = true;
            string batchJobLog = string.Empty;
            Guid batchJobRecordId = Guid.Empty;
            Common objCommon = new Common();
            batchJobLog += "---Batch Job Started---" + DateTime.Now + Environment.NewLine;
            batchJobLog += "<<=================================================>>" + Environment.NewLine;
            batchJobLog += "Job Frequency " + jobFrequency + Environment.NewLine;
            try
            {
				//clientId = GetSecretValue(keyVaultUrl, clientIdSecretName);
				//Console.WriteLine("Azure key Vault ClientId secret Value:" + clientId);
				//secret = GetSecretValue(keyVaultUrl, cientSecretName);
				Console.WriteLine("Azure key Vault Client secret Value:" + secret);
				//Create Batch Job Record in CRM
				accessToken = objCommon.GetCrmAccessToken();
                IOrganizationService service = objCommon.GetCrmService(accessToken);
                //batchJobRecordId = Common.CreateBatchJobRecord(service,Constants.batchJobName);
                batchJobLog += "Batch Job Record Created" + Environment.NewLine;
                
                if (!string.IsNullOrEmpty(accessToken))
                {
                    batchJobLog += "Access Token retrieved" + Environment.NewLine;
                    dcConfigs = objCommon.FetchConfigurations(accessToken, serviceUrl, Constants.configSetUpName);
                    if (dcConfigs != null && dcConfigs.Count > 0)
                    {
                        batchJobLog += "Configuration exist in CRM for Upcoming Birthday" + Environment.NewLine;
                        if(dcConfigs.ContainsKey("UpcomingBirthday_Yestrday"))
                        {
                            if (dcConfigs["UpcomingBirthday_Yestrday"] != string.Empty)
                            {
                                var entityCollection = service.RetrieveMultiple(new FetchExpression(dcConfigs["UpcomingBirthday_Yestrday"]));
                                if (entityCollection.Entities.Count > 0)
                                {
                                    foreach (Entity en in entityCollection.Entities)
                                    {
                                        try
                                        {
                                            if (en.Contains("ims_upcomingbirthday"))
                                            {
                                                Entity leadEntity = new Entity("lead");
                                                leadEntity.Id = en.Id;
                                                leadEntity.Attributes["ims_upcomingbirthday"] = en.GetAttributeValue<DateTime>("ims_upcomingbirthday").ToLocalTime().AddYears(1);
                                                service.Update(leadEntity);
                                                // context.Trace("Updated the next birthday date");
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            throw new Exception(ex.Message);
                                        }
                                    }
                                }
                            }
                        }
                        if (dcConfigs.ContainsKey("UpcomingBirthday_NextXDays"))
                        {
                            if (dcConfigs["UpcomingBirthday_NextXDays"] != string.Empty)
                            {
                                //To Get the Next XDays Upcoming Birthday Lead Records
                                    int fetchCount = 5000;
                                    // Initialize the page number.
                                    int pageNumber = 1;
                                    string pagingCookie = null;
                                    while (true)
                                    {
                                        string xml = CreateXml(dcConfigs["UpcomingBirthday_NextXDays"], pagingCookie, pageNumber, fetchCount);

                                        // Excute the fetch query and get the xml result.
                                        RetrieveMultipleRequest fetchRequest1 = new RetrieveMultipleRequest
                                        {
                                            Query = new FetchExpression(xml)
                                        };
                                        EntityCollection entityCollection = ((RetrieveMultipleResponse)service.Execute(fetchRequest1)).EntityCollection;
                                    //var entityCollection = context.RetrieveMultiple(new FetchExpression(string.Format(@"" + confiValueNextXDays + "")));
                                    if (entityCollection.Entities.Count > 0)
                                    {
                                        // var configValueEmailActivity = context.GetConfigValue<string>(ConfigurationKeys.Key_UpcomingBirthday_EmailActivity, AppConfigSetupKeys.AppKey_UpcomingbirhdayReminderNotofication);
                                        foreach (Entity en in entityCollection.Entities)
                                        {
                                            try
                                            {
                                                if (!CheckRelatedTaskActivity(en, service))
                                                {
                                                    CreateTaskActvity(service, en);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                throw new InvalidCastException(ex.Message);
                                            }
                                        }
                                    }
                                        // Check for morerecords, if it returns 1.
                                        if (entityCollection.MoreRecords)
                                        {
                                            //Console.WriteLine("\n****************\nPage number {0}\n****************", pageNumber);
                                            //Console.WriteLine("#\tAccount Name\t\t\tEmail Address");

                                            // Increment the page number to retrieve the next page.
                                            pageNumber++;

                                            // Set the paging cookie to the paging cookie returned from current results.                            
                                            pagingCookie = entityCollection.PagingCookie;
                                        }
                                        else
                                        {
                                            // If no more records in the result nodes, exit the loop.
                                            break;
                                        }
                                    }
                                }
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
        public static void CreateTaskActvity(IOrganizationService context, Entity targetLead)
        {
            Entity task = new Entity("task");
            if (targetLead.Contains("owninguser"))
                task.Attributes["ownerid"] = new EntityReference("systemuser", ((EntityReference)targetLead["ownerid"]).Id);
            else if (targetLead.Contains("owningteam"))
                task.Attributes["ownerid"] = new EntityReference("team", ((EntityReference)targetLead["ownerid"]).Id);
            task.Attributes["regardingobjectid"] = new EntityReference("lead", targetLead.Id);
            task.Attributes["subject"] = targetLead.Contains("fullname") ? string.Join("-", "Birthday Reminder : "+targetLead.GetAttributeValue<string>("fullname"), targetLead.Contains("ims_upcomingbirthday") ? targetLead.GetAttributeValue<DateTime>("ims_upcomingbirthday").Date.ToShortDateString() : string.Empty) : "Birthday Reminder";
            task.Attributes["scheduledstart"] = targetLead.Contains("ims_upcomingbirthday") ? targetLead.GetAttributeValue<DateTime>("ims_upcomingbirthday").Date.ToUniversalTime().AddHours(12) : DateTime.Now.Date;
            //task.Attributes["scheduledstart"] = targetLead.Contains("ims_upcomingbirthday") ? DateTime.Parse(targetLead.GetAttributeValue<DateTime>("ims_upcomingbirthday").Date.ToShortDateString() + " " + "12:00 AM", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None) : DateTime.Now.Date;
           // task.Attributes["scheduledend"] = targetLead.Contains("ims_upcomingbirthday") ? DateTime.Parse(targetLead.GetAttributeValue<DateTime>("ims_upcomingbirthday").Date.ToShortDateString() + " " + "11:59 PM", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None) : DateTime.Now.Date;
            task.Attributes["scheduledend"] = targetLead.Contains("ims_upcomingbirthday") ? targetLead.GetAttributeValue<DateTime>("ims_upcomingbirthday").Date.ToUniversalTime().AddHours(12): DateTime.Now.Date;
            task.Attributes["actualdurationminutes"] = Convert.ToInt32(30);
            context.Create(task);
        }
        private static string CreateXml(string xml, string cookie, int page, int count)
        {
            StringReader stringReader = new StringReader(xml);
            XmlTextReader reader = new XmlTextReader(stringReader);

            // Load document
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);

            return CreateXml(doc, cookie, page, count);
        }
        private static string CreateXml(XmlDocument doc, string cookie, int page, int count)
        {
            XmlAttributeCollection attrs = doc.DocumentElement.Attributes;

            if (cookie != null)
            {
                XmlAttribute pagingAttr = doc.CreateAttribute("paging-cookie");
                pagingAttr.Value = cookie;
                attrs.Append(pagingAttr);
            }

            XmlAttribute pageAttr = doc.CreateAttribute("page");
            pageAttr.Value = System.Convert.ToString(page);
            attrs.Append(pageAttr);

            XmlAttribute countAttr = doc.CreateAttribute("count");
            countAttr.Value = System.Convert.ToString(count);
            attrs.Append(countAttr);

            StringBuilder sb = new StringBuilder(1024);
            StringWriter stringWriter = new StringWriter(sb);

            XmlTextWriter writer = new XmlTextWriter(stringWriter);
            doc.WriteTo(writer);
            writer.Close();

            return sb.ToString();
        }
        public static bool CheckRelatedTaskActivity(Entity entity, IOrganizationService context)
        {
            if (entity.Contains("ims_upcomingbirthday"))
            {
                QueryByAttribute queryByAttribute = new QueryByAttribute("task");
                queryByAttribute.ColumnSet = new ColumnSet("subject");
                queryByAttribute.AddAttributeValue("regardingobjectid", entity.Id);
                queryByAttribute.AddAttributeValue("scheduledstart", entity.GetAttributeValue<DateTime>("ims_upcomingbirthday").Date.ToUniversalTime().AddHours(12));
                EntityCollection entityCollection = context.RetrieveMultiple(queryByAttribute);
                if (entityCollection.Entities.Count > 0)
                    return true;
            }
            return false;
        }

		public string GetSecretValue(string keyVaultUrl, string secretName)
		{
			string secret = "";
			AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
			var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
			//slow without ConfigureAwait(false)    
			//keyvault should be keyvault DNS Name    
			var secretBundle =  keyVaultClient.GetSecretAsync(keyVaultUrl + secretName).ConfigureAwait(false);
			secret = secretBundle.ToString();
			Console.WriteLine("Secret Name:" + secretName + " Value:" + secret);
			return secret;
		}

		public string GetAccessToken(string authority, string clientId, string secret, string serviceUrl)
		{
			string accessToken = string.Empty;
			try
			{
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
				Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext authContext = new Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext(authority);
				ClientCredential credential = new ClientCredential(clientId, secret);
				AuthenticationResult result = authContext.AcquireTokenAsync(serviceUrl, credential).Result;
				accessToken = result.AccessToken;
			}
			catch (Exception ex)
			{
				accessToken = string.Empty;
				throw new Exception("Error while getting Access Token: " + ex.Message + Environment.NewLine);
			}
			return accessToken;
		}
	}
}
