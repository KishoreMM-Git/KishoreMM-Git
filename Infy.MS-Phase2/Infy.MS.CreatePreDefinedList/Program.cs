using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Description;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.WebServiceClient;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.IdentityModel.Clients.ActiveDirectory;


namespace CreateMarketingList
{
    class Program
    {
        static IOrganizationService service = null;
        static void Main(string[] args)
        {
            service = getProxy_Static();
            retrieveUser(service);
        }
        public static void ConnectToMSCRM(string UserName, string Password, string OrgUri)
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
                service = (IOrganizationService)proxy;
                Console.WriteLine("The Connection is Successful");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while connecting to CRM " + ex.Message);
                Console.ReadKey();
            }
        }
        public static void retrieveUser(IOrganizationService service)
        {
            try
            {
                string fetchxml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' >
  <entity name='systemuser' >
    <attribute name='fullname' />
    <attribute name='businessunitid' />
    <attribute name='title' />
    <attribute name='address1_telephone1' />
    <attribute name='positionid' />
    <attribute name='systemuserid' />
    <order attribute='fullname' descending='false' />
    <filter type='and' >
      <condition attribute='createdon' operator='on-or-after' value='2021-07-31' />
      <condition attribute='businessunitid' operator='eq' uiname='Retail' uitype='businessunit' value='{8706AB32-F11F-EA11-A810-000D3A1AB78A}' />
    </filter>
    <link-entity name='systemuserroles' alias='a1' link-type='inner' to='systemuserid' from='systemuserid' >
      <link-entity name='role' alias='a2' link-type='inner' to='roleid' from='roleid' >
        <attribute name='name' />
        <filter type='or' >
          <condition attribute='name' value='Movement Direct LO' operator='eq' />
	      <condition attribute='name' value='Movement Direct Admin' operator='eq' />
          <condition attribute='name' value='Retail Loan Officer' operator='eq' />
        </filter>
      </link-entity>
    </link-entity>
  </entity>
</fetch>";

                EntityCollection etycoll = service.RetrieveMultiple(new FetchExpression(fetchxml));
                Console.WriteLine("The total Retail/MD Admin/MD LO " + " " + etycoll.Entities.Count);
                int i = 0;
                foreach (var users in etycoll.Entities)
                {
                    Guid systemUserId = users.GetAttributeValue<Guid>("systemuserid");
                    int count = checkIfListexist(systemUserId);
                    if (count > 0)
                    {

                    }
                    else
                    {
                        i = i + 1;
                        Console.WriteLine("Inside the foreach Statement for creatingsmartlist" + " " + i);
                        createMarketingList(systemUserId);
                    }
                }

                string allUsers = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' >
  <entity name='systemuser' >
    <attribute name='fullname' />
    <attribute name='businessunitid' />
    <attribute name='title' />
    <attribute name='address1_telephone1' />
    <attribute name='positionid' />
    <attribute name='systemuserid' />
    <order attribute='fullname' descending='false' />
    <filter type='and' >
      <condition attribute='createdon' operator='on-or-after' value='2021-07-31' />
      <condition attribute='businessunitid' operator='eq' uiname='Retail' uitype='businessunit' value='{8706AB32-F11F-EA11-A810-000D3A1AB78A}' />
    </filter>
    <link-entity name='systemuserroles' alias='a1' link-type='inner' to='systemuserid' from='systemuserid' >
      <link-entity name='role' alias='a2' link-type='inner' to='roleid' from='roleid' >
        <attribute name='name' />
        <filter type='or' >
          <condition attribute='name' value='Movement Direct LO' operator='eq' />
          <condition attribute='name' value='Retail Loan Officer' operator='eq' />
        </filter>
      </link-entity>
    </link-entity>
  </entity>
</fetch>";

                EntityCollection etyCollect = service.RetrieveMultiple(new FetchExpression(allUsers));
                int k = 0;
                foreach (var users in etyCollect.Entities)
                {
                    Guid systemUserId = users.GetAttributeValue<Guid>("systemuserid");
                    int count = checkListExist(systemUserId);
                    if (count > 0) { }
                    else
                    {
                        k = k + 1;
                        Console.WriteLine("Inside the foreach Statement for allleadsandcontacts" + " " + k);
                        allleadsContacts(systemUserId);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        public static int checkIfListexist(Guid userId)
        {
            string checkIfListExist = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='list'>
    <attribute name='listname' />
    <attribute name='type' />
    <attribute name='createdfromcode' />
    <attribute name='lastusedon' />
    <attribute name='purpose' />
    <attribute name='listid' />
    <attribute name='ownerid' />
    <order attribute='listname' descending='true' />
    <filter type='and'>
      <filter type='or'>
        <condition attribute='listname' operator='like' value='%Hot Leads%' />
        <condition attribute='listname' operator='like' value='%LO Website%' />
        <condition attribute='listname' operator='like' value='%Closed Loans%' />
      </filter>
      <condition attribute='ownerid' operator='eq'  value='" + userId + @"' />
    </filter>
  </entity>
</fetch>";
            EntityCollection list = service.RetrieveMultiple(new FetchExpression(checkIfListExist));
            int listCount = list.Entities.Count;
            return listCount;
        }
        public static int checkListExist(Guid userId)
        {
            string list = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='list'>
    <attribute name='listname' />
    <attribute name='type' />
    <attribute name='createdfromcode' />
    <attribute name='lastusedon' />
    <attribute name='purpose' />
    <attribute name='listid' />
    <attribute name='ownerid' />
    <order attribute='listname' descending='true' />
    <filter type='and'>
      <filter type='or'>
        <condition attribute='listname' operator='like' value='%All Leads%' />
        <condition attribute='listname' operator='like' value='%All Contacts%' />
      </filter>
      <condition attribute='ownerid' operator='eq'  value='" + userId + @"' />
    </filter>
  </entity>
</fetch>";
            EntityCollection list1 = service.RetrieveMultiple(new FetchExpression(list));
            int listCount = list1.Entities.Count;
            return listCount;
        }


        public static void createMarketingList(Guid userid)
        {
            //Guid userid = new Guid("28A37FF8-2552-EA11-A815-000D3A33E825");
            #region HotLeads
            //            string fetchXml = @"<fetch version='1.0' output-format='xml - platform' mapping='logical' distinct='false'>
            //    < entity name='lead'>
            //    <attribute name='fullname' />
            //    <attribute name='emailaddress1' />
            //    <attribute name='mobilephone' />
            //    <attribute name='lastname' />
            //    <attribute name='firstname' />
            //    <attribute name='leadqualitycode' />
            //    <attribute name='ims_leadstatus' />
            //    <attribute name='ims_leadscore' />
            //    <attribute name='ims_leadsource' />
            //    <attribute name='modifiedon' />
            //    <attribute name='ims_coborrower' />
            //    <attribute name='ims_loantype' />
            //    <attribute name='ims_loanpurpose' />
            //    <attribute name='ims_loanstatus' />
            //    <attribute name='ims_loanamount' />
            //    <attribute name='ims_propertytype' />
            //    <attribute name='ownerid' />
            //    <attribute name='ims_loanrate' />
            //    <attribute name='leadid' />
            //    <order descending='true' attribute='modifiedon' />
            //    <order descending='true' attribute='ims_leadscore' />
            //    <filter type='and'>
            //      <condition attribute='leadqualitycode' operator='eq' value='1' />
            //      <condition attribute='ownerid' operator='eq' uiname='Kathirvel Balasubramanian' uitype='systemuser' value='" + userid + @"' />
            //    </filter>
            //  </entity>
            //</fetch>";
            string fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='lead'>
    <attribute name='fullname' />
    <attribute name='emailaddress1' />
    <attribute name='mobilephone' />
    <attribute name='lastname' />
    <attribute name='firstname' />
    <attribute name='leadqualitycode' />
    <attribute name='ims_leadstatus' />
    <attribute name='ims_leadscore' />
    <attribute name='ims_leadsource' />
    <attribute name='modifiedon' />
    <attribute name='ims_coborrower' />
    <attribute name='ims_loantype' />
    <attribute name='ims_loanpurpose' />
    <attribute name='ims_loanstatus' />
    <attribute name='ims_loanamount' />
    <attribute name='ims_propertytype' />
    <attribute name='ownerid' />
    <attribute name='ims_loanrate' />
    <attribute name='leadid' />
    <order descending='true' attribute='modifiedon' />
    <order descending='true' attribute='ims_leadscore' />
 <filter type='and'>
      <condition attribute='leadqualitycode' operator='eq' value='1' />
      <condition attribute='ownerid' operator='eq'  value='" + userid + @"' />
    </filter>
  </entity>
</fetch>";
            //List<Entity> hotLeads = pagingmethod(fetchXml);

            Entity marketingList = new Entity("list");
            marketingList["listname"] = "Hot Leads";
            marketingList["type"] = true;
            marketingList["createdfromcode"] = new OptionSetValue(4);
            marketingList["query"] = fetchXml;
            marketingList["ownerid"] = new EntityReference("systemuser", userid);
            marketingList["ims_listtype"] = new OptionSetValue(176390000);
            service.Create(marketingList);
            Console.WriteLine("Marketing List for Hot Leads is created");
            #endregion

            #region LO WebSite Leads
            string LOLeads = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='lead'>
    <attribute name='fullname' />
    <attribute name='emailaddress1' />
    <attribute name='mobilephone' />
    <attribute name='lastname' />
    <attribute name='firstname' />
    <attribute name='leadqualitycode' />
    <attribute name='ims_leadstatus' />
    <attribute name='ims_leadscore' />
    <attribute name='ims_leadsource' />
    <attribute name='modifiedon' />
    <attribute name='ims_coborrower' />
    <attribute name='ims_loantype' />
    <attribute name='ims_loanpurpose' />
    <attribute name='ims_loanstatus' />
    <attribute name='ims_loanamount' />
    <attribute name='ims_propertytype' />
    <attribute name='ownerid' />
    <attribute name='ims_loanrate' />
    <attribute name='leadid' />
    <order descending='true' attribute='modifiedon' />
    <order descending='true' attribute='ims_leadscore' />
    <filter type='and'>
      <condition attribute='ims_leadsource' operator='eq' uiname='LO Website' uitype='ims_leadsource' value='{485E3DB7-A336-EA11-A812-000D3A4E6589}' />
      <condition attribute='ownerid' operator='eq' value='" + userid + @"' />
    </filter>
  </entity>
</fetch>";
            //List<Entity> LOwebsiteLeads = pagingmethod(LOLeads);

            Entity LOWebsiteLeads = new Entity("list");
            LOWebsiteLeads["listname"] = "LO Website Leads";
            LOWebsiteLeads["type"] = true;
            LOWebsiteLeads["createdfromcode"] = new OptionSetValue(4);
            LOWebsiteLeads["query"] = LOLeads;
            LOWebsiteLeads["ownerid"] = new EntityReference("systemuser", userid);
            LOWebsiteLeads["ims_listtype"] = new OptionSetValue(176390000);
            service.Create(LOWebsiteLeads);
            Console.WriteLine("Marketing List for Lo WebSite is created");
            #endregion

            #region Closed Loans
            string closedLoans = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='lead'>
    <attribute name='fullname' />
    <attribute name='emailaddress1' />
    <attribute name='mobilephone' />
    <attribute name='lastname' />
    <attribute name='firstname' />
    <attribute name='leadqualitycode' />
    <attribute name='ims_leadstatus' />
    <attribute name='ims_leadscore' />
    <attribute name='ims_leadsource' />
    <attribute name='modifiedon' />
    <attribute name='ims_coborrower' />
    <attribute name='ims_loantype' />
    <attribute name='ims_loanpurpose' />
    <attribute name='ims_loanstatus' />
    <attribute name='ims_loanamount' />
    <attribute name='ims_propertytype' />
    <attribute name='ownerid' />
    <attribute name='ims_loanrate' />
    <attribute name='leadid' />
    <order descending='true' attribute='modifiedon' />
    <order descending='true' attribute='ims_leadscore' />
    <filter type='and'>
      <condition attribute='ownerid' operator='eq'  value='" + userid + @"' />

      <condition attribute='ims_loanstatus' operator='eq' value='In Closing' />
    </filter>
  </entity>
</fetch>";
            //List<Entity> closedlOans = pagingmethod(LOLeads);
            Entity ClosedLoans = new Entity("list");
            ClosedLoans["listname"] = "Closed Loans";
            ClosedLoans["type"] = true;
            ClosedLoans["createdfromcode"] = new OptionSetValue(4);
            ClosedLoans["query"] = closedLoans;
            ClosedLoans["ownerid"] = new EntityReference("systemuser", userid);
            ClosedLoans["ims_listtype"] = new OptionSetValue(176390000);
            service.Create(ClosedLoans);
            Console.WriteLine("Marketing List for Closed Loans is created");
            //Console.ReadKey();
            #endregion


        }
        public static void allleadsContacts(Guid userid)
        {
            string allleads = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' >
  <entity name='lead' >
    <attribute name='fullname' />
    <attribute name='emailaddress1' />
    <attribute name='mobilephone' />
    <attribute name='lastname' />
    <attribute name='firstname' />
    <attribute name='leadqualitycode' />
    <attribute name='ims_leadstatus' />
    <attribute name='ims_leadscore' />
    <attribute name='ims_leadsource' />
    <attribute name='modifiedon' />
    <attribute name='ims_coborrower' />
    <attribute name='ims_loantype' />
    <attribute name='ims_loanpurpose' />
    <attribute name='ims_loanstatus' />
    <attribute name='ims_loanamount' />
    <attribute name='ims_propertytype' />
    <attribute name='ownerid' />
    <attribute name='ims_loanrate' />
    <attribute name='leadid' />
    <filter>
      <condition attribute='ownerid' operator='eq' value='" + userid + @"' />
    </filter>
    <order descending='true' attribute='modifiedon' />
    <order descending='true' attribute='ims_leadscore' />
  </entity>
</fetch>";
            //List<Entity> leadXml = pagingmethod(allleads);
            Entity leadList = new Entity("list");
            leadList["listname"] = "All Leads";
            leadList["type"] = true;
            leadList["createdfromcode"] = new OptionSetValue(4);
            leadList["query"] = allleads;
            leadList["ownerid"] = new EntityReference("systemuser", userid);
            leadList["ims_listtype"] = new OptionSetValue(176390000);
            service.Create(leadList);

            var fetchData = new
            {
                ownerid = userid
            };
            var fetchXml = $@"
<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='contact'>
    <attribute name='fullname' />
    <attribute name='telephone1' />
    <attribute name='ims_contactcategory' />
    <attribute name='contactid' />
    <attribute name='ims_company' />
    <attribute name='emailaddress1' />
    <attribute name='mobilephone' />
    <attribute name='ims_contacttype' />
    <attribute name='ownerid' />
    <order attribute='fullname' descending='false' />
    <filter type='and'>
      <condition attribute='ownerid' operator='eq' value='{fetchData.ownerid/*37723880-35b2-eb11-8236-00224805d85e*/}'/>
    </filter>
  </entity>
</fetch>";

            Entity contactList = new Entity("list");
            contactList["listname"] = "All Contacts";
            contactList["type"] = true;
            contactList["createdfromcode"] = new OptionSetValue(2);
            contactList["query"] = fetchXml;
            contactList["ownerid"] = new EntityReference("systemuser", userid);
            contactList["ims_listtype"] = new OptionSetValue(176390000);
            service.Create(contactList);
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
                allleads = service.RetrieveMultiple(new FetchExpression(xml));

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

                //string organizationUrl = "https://mmdevphase2.crm.dynamics.com";
                //string resourceURL = "https://mmdevphase2.api.crm.dynamics.com/api/data/v9.1/";
                //string clientId = "dbbfed03-5023-410b-b588-8db70a61c8af";///"39bb7c8c-c5c2-46c1-8829-a78d8742822b";
                //string appKey = "rFzxuRtO20n@BywqSIn_i7Y]5Q]nnAMQ";// "z4 @b2Y00iXzAbxsmxxTi@OekDP:saZf=";

                //string organizationUrl = "https://mmdev.crm.dynamics.com";
                //string resourceURL = "https://mmdev.api.crm.dynamics.com/api/data/v9.1/";
                //string clientId = "dbbfed03-5023-410b-b588-8db70a61c8af";///"39bb7c8c-c5c2-46c1-8829-a78d8742822b";
                //string appKey = "rFzxuRtO20n@BywqSIn_i7Y]5Q]nnAMQ";// "z4 @b2Y00iXzAbxsmxxTi@OekDP:saZf=";

                string organizationUrl = "https://movehome.crm.dynamics.com";
                string resourceURL = "https://movehome.api.crm.dynamics.com/api/data/v9.1/";
                string clientId = "39bb7c8c-c5c2-46c1-8829-a78d8742822b";
                string appKey = "z4@b2Y00iXzAbxsmxxTi@OekDP:saZf=";


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
             //   AuthenticationParameters authParam = AuthenticationParameters.CreateFromUrlAsync(new Uri(resourceURL)).Result;

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


                //ClientCredentials clientCredentials = new ClientCredentials();
                 



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
    }
}
