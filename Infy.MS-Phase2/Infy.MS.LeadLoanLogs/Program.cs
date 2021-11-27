using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Crm.Sdk.Messages;
using System.Net;
using System.ServiceModel.Description;
using Microsoft.Xrm.Sdk.Query;
using ClosedXML.Excel;
using System.Net.Mail;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Data;


using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Xrm.Sdk.WebServiceClient;

namespace Infy.MS.LeadLoanLogs
{
    class Program
    {
        static void Main(string[] args)
        {

            IOrganizationService organizationService = null;
            try
            {

                // For Dynamics 365 Customer Engagement V9.X, set Security Protocol as TLS12
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                organizationService = getProxy();

                //organizationService = (IOrganizationService)new OrganizationServiceProxy(new Uri("https://mortgageapp.api.crm.dynamics.com/XRMServices/2011/Organization.svc"),
                // null, clientCredentials, null);

                if (organizationService != null)
                {
                    Guid userid = ((WhoAmIResponse)organizationService.Execute(new WhoAmIRequest())).UserId;

                    if (userid != Guid.Empty)
                    {
                        Console.WriteLine("Connection Established Successfully...");
                        EntityCollection leadEntities = leadStaging(organizationService);
                        IEnumerable<Entity> LoanEntities = loanstaging(organizationService);
                        SendMail(leadEntities, LoanEntities);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static EntityCollection leadStaging(IOrganizationService organizationService)
        {
            List<string> passLead = new List<string>();
            QueryExpression qe = new QueryExpression("ims_leadstaging");
            qe.ColumnSet = new ColumnSet(true);

            ConditionExpression Conditio2 = new ConditionExpression("createdon", ConditionOperator.LastXHours, "24"); //09 / 21 / 2020
            ConditionExpression Condition3 = new ConditionExpression("ims_source", ConditionOperator.Equal, "PCL Automated Import");
            ConditionExpression Condition4 = new ConditionExpression("ims_validationstatus", ConditionOperator.Equal, false);
            FilterExpression filter1 = new FilterExpression();
            filter1.Conditions.Add(Conditio2);
            filter1.Conditions.Add(Condition3);
            filter1.Conditions.Add(Condition4);
            qe.Criteria.AddFilter(filter1);
            EntityCollection entityCollection = organizationService.RetrieveMultiple(qe);

            List<KeyValuePair<string, DateTime>> mylistFail = new List<KeyValuePair<string, DateTime>>();
            List<string> externalId = new List<string>();
            foreach (var entity in entityCollection.Entities)
            {
                if (entity.Attributes.Contains("ims_contactexternalid"))
                {
                    string failId = entity.Attributes["ims_contactexternalid"].ToString();
                    DateTime createOn = (DateTime)entity.Attributes["createdon"];
                    mylistFail.Add(new KeyValuePair<string, DateTime>(failId, createOn));
                }
                else
                {
                    Console.WriteLine("The REcord does not contain Contact External ID");
                }
            }

            //Validation Status Passed Records
            QueryExpression querypass = new QueryExpression("ims_leadstaging");
            querypass.ColumnSet = new ColumnSet(true);

            ConditionExpression createdon = new ConditionExpression("createdon", ConditionOperator.LastXHours, "24");
            ConditionExpression leadsource = new ConditionExpression("ims_source", ConditionOperator.Equal, "PCL Automated Import");
            ConditionExpression validationsta = new ConditionExpression("ims_validationstatus", ConditionOperator.Equal, true);
            FilterExpression filter2 = new FilterExpression();
            filter2.Conditions.Add(createdon);
            filter2.Conditions.Add(leadsource);
            filter2.Conditions.Add(validationsta);
            querypass.Criteria.AddFilter(filter2);
            EntityCollection entityCollectionpass = organizationService.RetrieveMultiple(querypass);

            List<KeyValuePair<string, DateTime>> mylistPass = new List<KeyValuePair<string, DateTime>>();
            List<string> myexternalId = new List<string>();
            foreach (var entity in entityCollectionpass.Entities)
            {
                if (entity.Attributes.Contains("ims_contactexternalid"))
                {
                    string passId = entity.Attributes["ims_contactexternalid"].ToString();
                    DateTime createOn = (DateTime)entity.Attributes["createdon"];
                    myexternalId.Add(passId);
                    mylistPass.Add(new KeyValuePair<string, DateTime>(passId, createOn));
                }
                else
                {
                    Console.WriteLine("The REcord does not contain Contact External ID");
                }
            }

            // Check for the ID from validation Failed and Passed Records and Checks th CreatedOn Time Here
            foreach (KeyValuePair<string, DateTime> faillist in mylistFail)
            {
                foreach (KeyValuePair<string, DateTime> passlist in mylistPass)
                {
                    if (passlist.Key == faillist.Key)
                    {
                        int datetime = DateTime.Compare(passlist.Value, faillist.Value);
                        if (datetime > 0)
                        {
                            passLead.Add(faillist.Key);
                        }
                        else if ((datetime < 0) || (datetime == 0))
                        {
                            passLead.Remove(faillist.Key);
                        }
                    }
                }
            }
            List<string> passlistlead = passLead.Distinct().ToList();


            //Final Query to get only Validation Failed Records 
            QueryExpression finalqe = new QueryExpression("ims_leadstaging");
            finalqe.ColumnSet = new ColumnSet(true);
            ConditionExpression cond5 = null;
            ConditionExpression cond2 = new ConditionExpression("createdon", ConditionOperator.LastXHours, "24");
            ConditionExpression cond3 = new ConditionExpression("ims_source", ConditionOperator.Equal, "PCL Automated Import");
            ConditionExpression cond4 = new ConditionExpression("ims_validationstatus", ConditionOperator.Equal, false);
            if (passlistlead.Count > 0) { cond5 = new ConditionExpression("ims_contactexternalid", ConditionOperator.NotIn, passlistlead); }
            FilterExpression fil1 = new FilterExpression();
            fil1.Conditions.Add(cond2);
            fil1.Conditions.Add(cond2);
            fil1.Conditions.Add(cond3);
            fil1.Conditions.Add(cond4);
            fil1.Conditions.Add(cond5);
            finalqe.Criteria.AddFilter(fil1);
            EntityCollection finalcollection = organizationService.RetrieveMultiple(finalqe);
            return finalcollection;
        }
        static IEnumerable<Entity> loanstaging(IOrganizationService organizationService)
        {
            //Validation Failed Records Query
            QueryExpression failqe = new QueryExpression("ims_loanstaging");
            failqe.ColumnSet = new ColumnSet(true);

            ConditionExpression fConditio2 = new ConditionExpression("createdon", ConditionOperator.LastXHours, "24");//"09/21/2020"
            ConditionExpression fCondition = new ConditionExpression("ims_validationstatus", ConditionOperator.Equal, false);

            FilterExpression failfilter1 = new FilterExpression();
            failfilter1.Conditions.Add(fConditio2);
            failfilter1.Conditions.Add(fCondition);
            failqe.Criteria.AddFilter(failfilter1);
            EntityCollection failEntityCollection = organizationService.RetrieveMultiple(failqe);

            List<KeyValuePair<string, DateTime>> mylistFail = new List<KeyValuePair<string, DateTime>>();
            List<string> externalId = new List<string>();
            foreach (var entity in failEntityCollection.Entities)
            {
                if (entity.Attributes.Contains("ims_loanexternalid"))
                {
                    string failId = entity.Attributes["ims_loanexternalid"].ToString();
                    DateTime createOn = (DateTime)entity.Attributes["createdon"];
                    mylistFail.Add(new KeyValuePair<string, DateTime>(failId, createOn));
                }
                else
                {
                    Console.WriteLine("The Record doesn't contain Loan External ID");
                }
            }

            //Validation Passed Records
            QueryExpression passQe = new QueryExpression("ims_loanstaging");
            passQe.ColumnSet = new ColumnSet(true);

            ConditionExpression passConditio2 = new ConditionExpression("createdon", ConditionOperator.LastXHours, "24");//"2020 -03-01"
            ConditionExpression passCondition4 = new ConditionExpression("ims_validationstatus", ConditionOperator.Equal, true);

            FilterExpression filter1 = new FilterExpression();
            filter1.Conditions.Add(passConditio2);
            filter1.Conditions.Add(passCondition4);
            passQe.Criteria.AddFilter(filter1);
            EntityCollection passEntityCollection = organizationService.RetrieveMultiple(passQe);

            List<KeyValuePair<string, DateTime>> mylistPass = new List<KeyValuePair<string, DateTime>>();
            foreach (var entity in passEntityCollection.Entities)
            {
                if (entity.Attributes.Contains("ims_loanexternalid"))
                {
                    string passId = entity.Attributes["ims_loanexternalid"].ToString();
                    DateTime createOn = (DateTime)entity.Attributes["createdon"];
                    mylistPass.Add(new KeyValuePair<string, DateTime>(passId, createOn));
                }
                else
                {
                    Console.WriteLine("The Record doesn't contain Loan External ID");
                }
            }
            List<string> passloan = new List<string>();
            List<string> failLoan = new List<string>();


            foreach (KeyValuePair<string, DateTime> faillist in mylistFail)
            {
                foreach (KeyValuePair<string, DateTime> passlist in mylistPass)
                {
                    if (passlist.Key == faillist.Key)
                    {
                        int datetime = DateTime.Compare(passlist.Value, faillist.Value);
                        if ((datetime > 0))  //|| (datetime == 0)
                        {
                            Console.WriteLine(datetime);
                            passloan.Add(faillist.Key);
                        }
                        else if ((datetime < 0) || (datetime == 0))
                        {
                            failLoan.Add(faillist.Key);
                        }
                    }
                    else
                    {
                        failLoan.Add(faillist.Key);
                    }

                }
            }


            //List<string> loanpass
            passloan = passloan.Distinct().ToList();
            failLoan = failLoan.Distinct().ToList();
            failLoan = failLoan.Except(passloan).ToList();

            QueryExpression finalqe = new QueryExpression("ims_loanstaging");
            finalqe.ColumnSet = new ColumnSet(true);
            ConditionExpression cond5 = null;
            ConditionExpression cond2 = new ConditionExpression("createdon", ConditionOperator.LastXHours, "24"); //09 / 21 / 2020
            ConditionExpression cond4 = new ConditionExpression("ims_validationstatus", ConditionOperator.Equal, false);
            if (passloan.Count > 0) { cond5 = new ConditionExpression("ims_loanexternalid", ConditionOperator.NotIn, passloan.ToArray()); }
            finalqe.Orders.Add(new OrderExpression("createdon", OrderType.Descending));
            FilterExpression fil1 = new FilterExpression();
            fil1.Conditions.Add(cond2);
            fil1.Conditions.Add(cond4);
            fil1.Conditions.Add(cond5);
            finalqe.Criteria.AddFilter(fil1);
            EntityCollection finalcollection = organizationService.RetrieveMultiple(finalqe);
            var lastPlayerControlCommand = finalcollection.Entities
                    .GroupBy(s => s.Attributes["ims_loanexternalid"])
                    .Select(s => s.OrderByDescending(x => x.Attributes["createdon"]).FirstOrDefault());
            return lastPlayerControlCommand;

        }
        static void SendMail(EntityCollection leadentities, IEnumerable<Entity> loanentities)
        {
            List<string> leadlist = new List<string>();
            List<string> leadfields = new List<string>();

            List<string> loanlist = new List<string>();
            List<string> loanfields = new List<string>();

            string textBody = "<table border=" + 1 + " cellpadding=" + 0 + " cellspacing=" + 0 + " width = " + 400 + "><tr bgcolor='#4da6ff'><td><b>DistinctError Lead</b></td> <td> <b>Total Record Count</b> </td></tr>";
            string textBody1 = "<table border=" + 1 + " cellpadding=" + 0 + " cellspacing=" + 0 + " width = " + 400 + "><tr bgcolor='#4da6ff'><td><b>DistinctError Loan</b></td> <td> <b>Total Record Count</b> </td></tr>";

            foreach (var lead in leadentities.Entities)
            {
                string toBeSearched = "Error";
                var error = string.Empty;
                var lastName = string.Empty;
                var firstName = string.Empty;
                var email = string.Empty;
                var phone = string.Empty;
                var leadSource = string.Empty;
                var LOexternalId = string.Empty;
                var contactGrpexternalId = string.Empty;
                var createdon = string.Empty;
                var contactgrpid = string.Empty;

                if (lead.Contains("ims_errorlog")) { error = lead.Attributes["ims_errorlog"].ToString(); }
                if (lead.Contains("ims_lastname")) { lastName = lead.Attributes["ims_lastname"].ToString(); }
                if (lead.Contains("ims_firstname")) { firstName = lead.Attributes["ims_firstname"].ToString(); }
                if (lead.Contains("ims_email")) { email = lead.Attributes["ims_email"].ToString(); }
                if (lead.Contains("ims_phone")) { phone = lead.Attributes["ims_phone"].ToString(); }
                if (lead.Contains("ims_source")) { leadSource = lead.Attributes["ims_source"].ToString(); }
                if (lead.Contains("ims_loexternalid")) { LOexternalId = lead.Attributes["ims_loexternalid"].ToString(); }
                if (lead.Contains("ims_contactexternalid")) { contactgrpid = lead.Attributes["ims_contactexternalid"].ToString(); }
                if (lead.Contains("ims_contactgroupexternalid")) { contactGrpexternalId = lead.Attributes["ims_contactgroupexternalid"].ToString(); }
                if (lead.Contains("createdon")) { createdon = lead.Attributes["createdon"].ToString(); }

                string[] lines = error.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                foreach (string line in lines)
                {
                    if (line.Contains(toBeSearched))
                    {
                        leadfields.Add(line);
                        string text = line + "," + firstName + "," + lastName + "," + email + "," + phone + "," + LOexternalId + "," + contactGrpexternalId + "," + leadSource + "," + createdon + "," + contactgrpid;
                        leadlist.Add(text);
                    }
                }
            }
            foreach (var loan in loanentities)
            {
                string toBeSearched = "Error";
                string toBeSearched1 = "Error:";
                var error = string.Empty;
                var LoanNumber = string.Empty;
                var LOExternalID = string.Empty;
                var CoBorrowerExternalId = string.Empty;
                var BorrowerExternalId = string.Empty;
                var LoanExternalId = string.Empty;

                if (loan.Contains("ims_errorlog")) { error = loan.Attributes["ims_errorlog"].ToString(); }
                if (loan.Contains("ims_loannumber")) { LoanNumber = loan.Attributes["ims_loannumber"].ToString(); }
                if (loan.Contains("ims_loexternalid")) { LOExternalID = loan.Attributes["ims_loexternalid"].ToString(); }
                if (loan.Contains("ims_coborrowerexternalid")) { CoBorrowerExternalId = loan.Attributes["ims_coborrowerexternalid"].ToString(); }
                if (loan.Contains("ims_borrowerexternalid")) { BorrowerExternalId = loan.Attributes["ims_borrowerexternalid"].ToString(); }
                if (loan.Contains("ims_loanexternalid")) { LoanExternalId = loan.Attributes["ims_loanexternalid"].ToString(); }

                string[] lines = error.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                foreach (string line in lines)
                {
                    if (line.Contains(toBeSearched) || line.Contains("BORROWER NOT FOUND") || line.Contains(toBeSearched1))
                    {
                        loanfields.Add(line);
                        string text = line + "&" + LoanNumber + "&" + LOExternalID + "&" + CoBorrowerExternalId + "&" + BorrowerExternalId + "&" + LoanExternalId;
                        loanlist.Add(text);
                    }
                }
            }
            loanlist = loanlist.Distinct().ToList();
            //Distinct Lead Errors Records
            List<string> leaddistinctLog = leadfields.Distinct().ToList();
            foreach (var item in leaddistinctLog)
            {
                var cnt = leadfields.Count(i => i.Equals(item, StringComparison.InvariantCultureIgnoreCase));
                textBody += "<tr><td>" + item + "</td><td> " + cnt + "</td> </tr>";
            }
            textBody += "</table>";

            //Distinct Loan Errors Records
            List<string> loandistinctLog = loanfields.Distinct().ToList();
            foreach (var item in loandistinctLog)
            {
                var cnt = loanfields.Count(i => i.Equals(item, StringComparison.InvariantCultureIgnoreCase));
                textBody1 += "<tr><td>" + item + "</td><td> " + cnt + "</td> </tr>";
            }
            textBody1 += "</table>";

            //Creating Excel Sheet
            XLWorkbook workbook = new XLWorkbook();
            DataTable leadErrorLog = new DataTable("Lead Errors");
            leadErrorLog.Columns.AddRange(new DataColumn[10]{
                             new DataColumn("Lead Error Log"),
                             new DataColumn("FirstName"),
                             new DataColumn("LastName"),
                             new DataColumn("Email"),
                             new DataColumn("MobilePhone"),
                             new DataColumn("LO External Id"),
                             new DataColumn("Contact Group External Id"),
                             new DataColumn("Lead Source"),
                             new DataColumn("Created On"),
                             new DataColumn("Contact Group ID"),
                       });
            foreach (var items in leadlist)
            {
                string val = items.ToString();
                string[] values = val.Split(',');
                leadErrorLog.Rows.Add(values);
            }

            DataTable loanErrorLog = new DataTable("Loan Errors");
            loanErrorLog.Columns.AddRange(new DataColumn[6]{
                new DataColumn("Loan Error Log"),
                new DataColumn("Loan Number"),
                new DataColumn("LO External ID"),
                new DataColumn("Co-Borrower External Id"),
                new DataColumn("Borrower External Id"),
                new DataColumn("Loan External Id"),
            });
            foreach (var items in loanlist)
            {
                string val = items.ToString();
                string[] values = val.Split('&');
                loanErrorLog.Rows.Add(values);
            }

            workbook.Worksheets.Add(leadErrorLog);
            workbook.Worksheets.Add(loanErrorLog);
            MemoryStream stream = new MemoryStream();
            workbook.SaveAs(stream);

            //Attachment of Excel Sheet in the Mail.
            stream.Position = 0;
            Attachment ExcelAttch = new Attachment(stream, "ErrorLog.xlsx", "application/vnd.ms-excel");
            string _sender = ConfigurationManager.AppSettings["_senderMailId"];
            string _password = ConfigurationManager.AppSettings["_senderPass"];
            var senderName = string.Empty;
            int index = _sender.IndexOf('@');
            if (index > 0)
            {
                senderName = _sender.Substring(0, index);
            }

            //Mail Config.
            SmtpClient client = new SmtpClient("smtp-mail.outlook.com");
            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(_sender, _password);
            client.EnableSsl = true;
            client.Credentials = credentials;

            string[] fromAdd = ConfigurationManager.AppSettings["_receiverMailId"].Split(';');
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(_sender, "Svc-MSDCRM-Prod");
           // mailMessage.To.Add(new MailAddress(ConfigurationManager.AppSettings["_receiverMailId"]));
            foreach (string toemail in fromAdd)
            {
                mailMessage.To.Add(new MailAddress(toemail)); //adding multiple TO Email Id
            }
            mailMessage.Subject = "Error Log for Lead and Load Entity ";
            mailMessage.Body = "Please find the attached Document for the list of error in LeadStaging and LoanStaging entity" + "</br></br>" + "<b>Lead Error:</b></br></br>" + textBody + "</br></br><b>Loan Error</b></br></br>" + textBody1 + "</br></br>Thanks,</br>" + senderName;
            mailMessage.IsBodyHtml = true;
            mailMessage.Attachments.Add(ExcelAttch);
            client.Send(mailMessage);
        }

        public static IOrganizationService getProxy()
        {
            IOrganizationService _orgService = null;
            OrganizationWebProxyClient sdkService;


            try
            {
                string organizationUrl = ConfigurationManager.AppSettings["organizationUrl"].ToString();
                string resourceURL = ConfigurationManager.AppSettings["resourceURL"].ToString();
                Console.WriteLine(organizationUrl + " - " + resourceURL);
                string clientId = GetSecretValue(ConfigurationManager.AppSettings["KeyVaultUrl"].ToString(), "clientId").Result;
                Console.WriteLine("clientId : " + clientId);
                string appKey = GetSecretValue(ConfigurationManager.AppSettings["KeyVaultUrl"].ToString(), "secret").Result;
                Console.WriteLine("appKey : " + appKey);


                //Create the Client credentials to pass for authentication
                ClientCredential clientcred = new ClientCredential(clientId, appKey);

                //get the authentication parameters
                AuthenticationParameters authParam = AuthenticationParameters.CreateFromUrlAsync(new Uri(resourceURL)).Result;

                //Generate the authentication context - this is the azure login url specific to the tenant
                // string authority = authParam.Authority;
                string authority = ConfigurationManager.AppSettings["authority"].ToString();
                Console.WriteLine("authority : " + authority);
                //request token
                AuthenticationResult authenticationResult = new AuthenticationContext(authority).AcquireTokenAsync(organizationUrl, clientcred).Result;
                Console.WriteLine("authenticationResult : " + authenticationResult);
                //get the token              
                string token = authenticationResult.AccessToken;
                Console.WriteLine("token : " + token);
                Uri serviceUrl = new Uri(organizationUrl + @"/xrmservices/2011/organization.svc/web?SdkClientVersion=9.1");
                Console.WriteLine("token : " + token);
                using (sdkService = new OrganizationWebProxyClient(serviceUrl, false))
                {
                    sdkService.HeaderToken = token;

                    _orgService = (IOrganizationService)sdkService != null ? (IOrganizationService)sdkService : null;
                    Console.WriteLine("_orgService : " + _orgService);
                }


                //ClientCredentials clientCredentials = new ClientCredentials();
                


                // For Dynamics 365 Customer Engagement V9.X, set Security Protocol as TLS12
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                // Get the URL from CRM, Navigate to Settings -> Customizations -> Developer Resources
                // Copy and Paste Organization Service Endpoint Address URL
                //organizationService = (IOrganizationService)new OrganizationServiceProxy(new Uri("https://mmdevphase2.api.crm.dynamics.com/XRMServices/2011/Organization.svc"),
                // null, clientCredentials, null);

                if (_orgService != null)
                {
                    Guid userid = ((WhoAmIResponse)_orgService.Execute(new WhoAmIRequest())).UserId;
                    Console.WriteLine("userid : " + userid);
                    if (userid != Guid.Empty)
                    {

                        return _orgService;

                    }
                }
                else
                {
                    Console.WriteLine("Failed to Established Connection!!!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught - " + ex.Message);
            }
            return _orgService;

        }

        public static async Task<string> GetSecretValue(string keyVaultUrl, string secretName)
        {
            string secret = "";
            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
            var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            //slow without ConfigureAwait(false)    
            //keyvault should be keyvault DNS Name    
            var secretBundle = await keyVaultClient.GetSecretAsync(keyVaultUrl + secretName).ConfigureAwait(false);
            secret = secretBundle.Value;
            Console.WriteLine("Secret Name:" + secretName + " Value:" + secret);
            return secret;
        }
    }
}
