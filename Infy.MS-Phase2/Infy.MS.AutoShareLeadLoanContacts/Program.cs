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
using System.Net.Mail;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Data;


//using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Xrm.Sdk.WebServiceClient;
using Microsoft.Azure.KeyVault;
//using Microsoft.Azure.WebJobs;
using System.Net.Http;
using System.Net.Http.Headers;
//using AutoShareLeadLoanContacts;
//using Newtonsoft.Json.Linq;
using System.Threading;
using Microsoft.Xrm.Tooling.Connector;

namespace Infy.MS.AutoShareLeadLoanContacts
{
    public class Program
    {
        static IOrganizationService organizationService = null;

        static void Main(string[] args)
        {
            Guid teamid = new Guid(args[0]);  //new Guid("E710ABB8-BB54-EB11-BB23-000D3A30F81D"); //    // new Guid("B587A656-255A-EB11-A812-002248047EDB");
            try
            {

                organizationService = getProxy();   // getProxy_Static(); // getProxy_Static();
                shareleadContacts(teamid, organizationService);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        /*public static void ConnectToMSCRM(string UserName, string Password, string OrgUri)
        {
            try
            {
                ClientCredentials credentials = new ClientCredentials();
                credentials.UserName.UserName = UserName;
                credentials.UserName.Password = Password;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                Uri serviceUri = new Uri(OrgUri);
                OrganizationServiceProxy proxy = new OrganizationServiceProxy(serviceUri, null, credentials, null);
                proxy.EnableProxyTypes();
                Console.WriteLine("Connection Created Successfully");
                organizationService = (IOrganizationService)proxy;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while connecting to CRM " + ex.Message);
                Console.ReadKey();
            }
        }*/
        public static void shareleadContacts(Guid teamid, IOrganizationService organizationService)
        {
            Entity team = organizationService.Retrieve("team", teamid, new ColumnSet("administratorid", "ims_autosharemyleadsandloans", "ims_autosharemycontacts"));
            bool autoshareleadloans = team.GetAttributeValue<bool>("ims_autosharemyleadsandloans");
            bool autosharecontacts = team.GetAttributeValue<bool>("ims_autosharemycontacts");
            Console.WriteLine("The Ety Name is" + team.LogicalName);
            Console.WriteLine("The Team Name is Name is" + team.GetAttributeValue<string>("name"));
            var adminid = team.GetAttributeValue<EntityReference>("administratorid").Id;

            if (autosharecontacts == true)
            {
                string contactXml = @"<fetch {0}>
                  <entity name='contact' >
                    <attribute name='ownerid' />
                    <attribute name='firstname' />
                    <attribute name='lastname' />
                    <filter>
                      <condition attribute='ownerid' operator='eq' value='" + adminid + @"'/>
                    </filter>
                  </entity>
                </fetch>";
                List<Entity> allcontacts = pagingmethod(contactXml);
                grantaccess(allcontacts, teamid);
            }
            if (autoshareleadloans == true)
            {
                string leadXml = @"<fetch {0}>
  <entity name='lead' >
    <attribute name='firstname' />
    <attribute name='ownerid' />
    <attribute name='lastname' />
    <filter>
      <condition attribute='ownerid' operator='eq' value='" + adminid + @"' />
    </filter>
  </entity>
</fetch>";
                List<Entity> allleads = pagingmethod(leadXml);
                grantaccess(allleads, teamid);

                string loanXml = @"<fetch {0}>
  <entity name='opportunity'>
    <attribute name='ownerid' />
    <attribute name='ims_loanname' />
    <attribute name='ims_lastname' />
    <filter>
      <condition attribute='ownerid' operator='eq' value='" + adminid + @"' />
    </filter>
  </entity>
</fetch>";
                List<Entity> allloans = pagingmethod(loanXml);
                grantaccess(allloans, teamid);
            }

            if (autosharecontacts == false)
            {
                //string contactXml = @"<fetch {0}>
                //  <entity name='contact' >
                //    <attribute name='ownerid' />
                //    <attribute name='firstname' />
                //    <attribute name='lastname' />
                //    <filter>
                //      <condition attribute='ownerid' operator='eq' value='" + adminid + @"'/>
                //    </filter>
                //  </entity>
                //</fetch>";

                string contactxml = @"<fetch {0} >
  <entity name='contact' >
    <attribute name='fullname' />
    <link-entity name='principalobjectaccess' from='objectid' to='contactid' alias='bb' >
      <filter type='and' >
        <condition attribute='principalid' operator='eq' uitype='team' value='" + teamid + @"' />
        <condition attribute='accessrightsmask' operator='eq' value='3' />
      </filter>
    </link-entity>
  </entity>
</fetch>";


                List<Entity> allcontacts = pagingmethod(contactxml);
                revokeAccess(allcontacts, teamid);
            }
            if (autoshareleadloans == false)
            {
                string leadxml = @"<fetch {0} >
  <entity name='lead' >
    <attribute name='fullname' />
    <link-entity name='principalobjectaccess' from='objectid' to='leadid' alias='bb' >
      <filter type='and' >
        <condition attribute='principalid' operator='eq' uitype='team' value='" + teamid + @"' />
<condition attribute='accessrightsmask' operator='eq' value='3' />
      </filter>
    </link-entity>
  </entity>
</fetch>";

                List<Entity> allleads = pagingmethod(leadxml);
                revokeAccess(allleads, teamid);


                string loanxml = @"<fetch {0} >
  <entity name='opportunity' >
    <attribute name='name' />
    <link-entity name='principalobjectaccess' from='objectid' to='opportunityid' alias='bb' >
      <filter type='and' >
        <condition attribute='principalid' operator='eq' uitype='team' value='" + teamid + @"' />
        <condition attribute='accessrightsmask' operator='eq' value='3' />
      </filter>
    </link-entity>
  </entity>
</fetch>";

                List<Entity> allloans = pagingmethod(loanxml);
                revokeAccess(allloans, teamid);
            }
        }

        public static void revokeAccess(List<Entity> records, Guid teamid)
        {
            try
            {
                int proxycounter = 0;
                int count = 0;
                var systemUser2Ref = new EntityReference("team", teamid);
                foreach (var ety in records)
                {
                    proxycounter++;
                    var leadreference = new EntityReference(ety.LogicalName, ety.Id);
                    var revokeUserAccessReq = new RevokeAccessRequest
                    {
                        Revokee = systemUser2Ref,
                        Target = leadreference
                    };
                    organizationService.Execute(revokeUserAccessReq);
                    Console.WriteLine("The revokeAccess ProxyCounter Value is" + proxycounter);
                    if (proxycounter == 300)
                    {
                        count++;
                        int totalrecordcount = proxycounter * count;
                        Console.WriteLine("The total record count is" + totalrecordcount);
                        proxycounter = 0;
                        Console.WriteLine("the proxy counter value is " + proxycounter);

                        //getProxy_Static();
                        getProxy();
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }

        }
        public static void grantaccess(List<Entity> records, Guid teamid)
        {
            try
            {
                int proxycounter = 0;
                int count = 0;
                foreach (var ety in records)
                {
                    proxycounter++;
                    GrantAccessRequest grant = new GrantAccessRequest();
                    grant.Target = new EntityReference(ety.LogicalName, ety.Id);

                    PrincipalAccess principal = new PrincipalAccess();
                    principal.Principal = new EntityReference("team", teamid);
                    principal.AccessMask = AccessRights.ReadAccess | AccessRights.WriteAccess;
                    grant.PrincipalAccess = principal;

                    GrantAccessResponse granted = (GrantAccessResponse)organizationService.Execute(grant);
                    Console.WriteLine("The ProxyCounter Value is" + proxycounter);
                    if (proxycounter == 300)
                    {
                        count++;
                        int totalrecordcount = proxycounter * count;
                        Console.WriteLine("The total record count is" + totalrecordcount);
                        proxycounter = 0;
                        Console.WriteLine("the proxy counter value is " + proxycounter);

                        getProxy();  //getProxy_Static();

                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }

        }

        public static List<Entity> pagingmethod(string leadXml)
        {
            var moreRecords = false;
            int page = 1;
            var cookie = string.Empty;
            EntityCollection allleads = null;
            List<Entity> Entities = new List<Entity>();
            do
            {
                //organizationService = getProxy();


                var xml = string.Format(leadXml, cookie);
                allleads = organizationService.RetrieveMultiple(new FetchExpression(xml));

                if (allleads.Entities.Count >= 0) Entities.AddRange(allleads.Entities);

                moreRecords = allleads.MoreRecords;
                if (moreRecords)
                {
                    page++;
                    cookie = string.Format("paging-cookie='{0}' page='{1}'", System.Security.SecurityElement.Escape(allleads.PagingCookie), page);
                }
            } while (moreRecords);

            return Entities;
        }
        public static IOrganizationService getProxy()
        {
            Console.WriteLine("Connection using proxy method succesful for Dev Instance");
            IOrganizationService _orgService = null;

            try
            {
                string organizationUrl = ConfigurationManager.AppSettings["organizationUrl"].ToString();
                string resourceURL = ConfigurationManager.AppSettings["resourceURL"].ToString();
                string serviceUrl = ConfigurationManager.AppSettings["organizationurl"].ToString();
                Console.WriteLine(organizationUrl + " - " + resourceURL);
                string clientId = GetSecretValue(ConfigurationManager.AppSettings["KeyVaultUrl"].ToString(), "clientId").Result;
                Console.WriteLine("clientId : " + clientId);
                string appKey = GetSecretValue(ConfigurationManager.AppSettings["KeyVaultUrl"].ToString(), "secret").Result;
                Console.WriteLine("appKey : " + appKey);

                string connectionString = "AuthType=ClientSecret;Url='" + serviceUrl + "' ; clientid='" + clientId + "';     clientsecret= '" + appKey + "'; ";
                Console.WriteLine("The ConnectionString value is " + connectionString);
                CrmServiceClient crmConn = new CrmServiceClient(connectionString);
                IOrganizationService crmService = crmConn.OrganizationServiceProxy;

                CrmServiceClient.MaxConnectionTimeout = new TimeSpan(4, 0, 0);

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;


                if (crmConn != null)
                {
                    Guid userid = ((WhoAmIResponse)crmConn.Execute(new WhoAmIRequest())).UserId;
                    Console.WriteLine("UserId is " + userid);
                    if (userid != null)
                    {
                        Console.WriteLine("The Connection is successfull");
                        return crmConn;
                    }
                }
                else
                {
                    Console.WriteLine("Failed to establish connection");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught - " + ex.Message);
            }
            return _orgService;

        }

        public static IOrganizationService getProxy_Static()
        {
            IOrganizationService _orgService = null;
            OrganizationWebProxyClient sdkService;

            try
            {
                //string organizationUrl = "https://mmsitphase2.crm.dynamics.com";
                //string resourceURL = "https://mmsitphase2.api.crm.dynamics.com/api/data/v9.1/";
                //string clientId = "156d5a4b-0618-46cd-9b8c-a70e17d0eba1"; //"dbbfed03-5023-410b-b588-8db70a61c8af";
                //string appKey = "wa1L:.]7=NnBR7l0?sTYn6ZytngidJPs";//"rFzxuRtO20n@BywqSIn_i7Y]5Q]nnAMQ";

                string organizationUrl = "https://mmdevphase2.crm.dynamics.com";
                string resourceURL = "https://mmdevphase2.api.crm.dynamics.com/api/data/v9.1/";
                string clientId = "dbbfed03-5023-410b-b588-8db70a61c8af";///"39bb7c8c-c5c2-46c1-8829-a78d8742822b";
                string appKey = "rFzxuRtO20n@BywqSIn_i7Y]5Q]nnAMQ";// "z4 @b2Y00iXzAbxsmxxTi@OekDP:saZf=";



                string connectionString = "AuthType=ClientSecret;Url='" + organizationUrl + "' ; clientid='" + clientId + "';     clientsecret= '" + appKey + "'; ";
                Console.WriteLine("The ConnectionString value is " + connectionString);
                CrmServiceClient crmConn = new CrmServiceClient(connectionString); //ConfigurationManager.ConnectionStrings["CRM"].ConnectionString
                IOrganizationService crmService = crmConn.OrganizationServiceProxy;

                CrmServiceClient.MaxConnectionTimeout = new TimeSpan(4, 0, 0);

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;


                if (crmConn != null)
                {
                    Guid userid = ((WhoAmIResponse)crmConn.Execute(new WhoAmIRequest())).UserId;
                    Console.WriteLine("UserId is " + userid);
                    if (userid != null)
                    {
                        Console.WriteLine("The Connection is successfull");
                        return crmConn;
                    }
                }
                else
                {
                    Console.WriteLine("Failed to establish connection");
                }

                //string clientId = "156d5a4b-0618-46cd-9b8c-a70e17d0eba1";
                //string appKey = "wa1L:.]7=NnBR7l0?sTYn6ZytngidJPs";


                //Create the Client credentials to pass for authentication
                ClientCredential clientcred = new ClientCredential(clientId, appKey);

                //get the authentication parameters
                AuthenticationParameters authParam = AuthenticationParameters.CreateFromUrlAsync(new Uri(resourceURL)).Result;

                //Generate the authentication context - this is the azure login url specific to the tenant
                // string authority = authParam.Authority;
                string authority = "https://login.microsoftonline.com/095f0976-bf66-4d76-bf29-0fbab0884ecb";

                //request token
                AuthenticationResult authenticationResult = new AuthenticationContext(authority).AcquireTokenAsync(organizationUrl, clientcred).Result;

                //get the token              
                string token = authenticationResult.AccessToken;
                // accessToken = token;

                Uri serviceUrl = new Uri(organizationUrl + @"/xrmservices/2011/organization.svc/web?SdkClientVersion=8.2");
                //HttpWebRequest req = (HttpWebRequest)WebRequest.Create(serviceUrl);
                //req.KeepAlive = false;

                using (sdkService = new OrganizationWebProxyClient(serviceUrl, false))
                {
                    sdkService.HeaderToken = token;

                    _orgService = (IOrganizationService)sdkService != null ? (IOrganizationService)sdkService : null;
                }





                // For Dynamics 365 Customer Engagement V9.X, set Security Protocol as TLS12
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
                // Get the URL from CRM, Navigate to Settings -> Customizations -> Developer Resources
                // Copy and Paste Organization Service Endpoint Address URL
                //organizationService = (IOrganizationService)new OrganizationServiceProxy(new Uri("https://mmdevphase2.api.crm.dynamics.com/XRMServices/2011/Organization.svc"),
                // null, clientCredentials, null);

                if (_orgService != null)
                {
                    Guid userid = ((WhoAmIResponse)_orgService.Execute(new WhoAmIRequest())).UserId;

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

        /* public static IOrganizationService getProxy()
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

         }*/
        public static async Task<string> GetSecretValue(string keyVaultUrl, string secretName)
        {
            string secret = "";
            try
            {
                AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
                var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
                //slow without ConfigureAwait(false)    
                //keyvault should be keyvault DNS Name    
                var secretBundle = await keyVaultClient.GetSecretAsync(keyVaultUrl + secretName).ConfigureAwait(false);
                secret = secretBundle.Value;
                Console.WriteLine("Secret Name:" + secretName + " Value:" + secret);
                return secret;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return secret;
        }
    }
}