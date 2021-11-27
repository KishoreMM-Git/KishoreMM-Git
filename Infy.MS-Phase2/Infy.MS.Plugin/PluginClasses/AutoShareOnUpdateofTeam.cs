using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xrm;
using XRMExtensions;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk.Messages;
using System.Xml.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Crm.Sdk;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net;


namespace Infy.MS.Plugins.PluginClasses
{
    public class AutoShareOnUpdateofTeam : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingservice = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.  
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            if (context == null)
            {
                throw new InvalidPluginExecutionException("Context Not Found");
            }
            if (context.MessageName.ToLower() != "update")
            {
                return;
            }
            tracingservice.Trace("PLugis executes");
            var team = (Entity)context.InputParameters["Target"];
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                if (context.MessageName.ToLower() == "update")
                {
                    Entity ety = service.Retrieve(team.LogicalName, team.Id, new ColumnSet(true)); //"administratorid", "ims_autosharemyleadsandloans", "ims_autosharemycontacts"
                    tracingservice.Trace("Web job gettting triggered here");
                    triggerWebJob(ety.Id, tracingservice, service, context);
                    tracingservice.Trace("Web Job triggered Successfully");
                }
            }
        }
        public static void triggerWebJob(Guid teamid, ITracingService tracingService, IOrganizationService service, IPluginExecutionContext context)
        {
            tracingService.Trace("Inside triggerWebJob Function & the Team Id value is " + teamid.ToString());

            QueryExpression organizationquery = new QueryExpression("organization")
            {
                ColumnSet = new ColumnSet("name")
            };

            Entity organization = service.RetrieveMultiple(organizationquery).Entities.FirstOrDefault();

            string organizationname = organization.GetAttributeValue<string>("name");

            tracingService.Trace("the Instance name is " + organizationname);
            try
            {
                string url = string.Empty;
                string user = string.Empty;
                string cred = string.Empty;
                if (organizationname.ToLower() == "mmdevphase2" || organizationname.ToLower() == "org14c342d3")
                {

                    try
                    {
                        #region AutoShareLeadsLoans
                        tracingService.Trace("the Instance name is " + "Into the instace" + organizationname);
                        Dictionary<string, string> AutoShareUrl = Common.FetchConfigDetails(Constants.AutoShareUrl, service);
                        foreach (var value in AutoShareUrl.Values)
                        {
                            url = value.ToString();
                        }
                        string uri = "" + url + "" + teamid;   //E4CECEA5-F959-EA11-A811-000D3A33F637
                        tracingService.Trace(uri);
                        Dictionary<string, string> AutoShareUserName = Common.FetchConfigDetails(Constants.AutoShareUserName, service);
                        foreach (var value in AutoShareUserName.Values)
                        {
                            user = value.ToString();
                        }
                        Dictionary<string, string> AutoShareCred = Common.FetchConfigDetails(Constants.AutoShareCred, service);
                        foreach (var value in AutoShareCred.Values)
                        {
                            cred = value.ToString();
                        }
                        WebClient webclient = new WebClient();
                        string userName = user;
                        string passWord = cred;
                        string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(userName + ":" + passWord));
                        webclient.Headers[HttpRequestHeader.Authorization] = "Basic " + credentials;
                        var resposnestring = webclient.UploadValues(uri, "POST", webclient.QueryString);
                        var responseString = UnicodeEncoding.UTF8.GetString(resposnestring);
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        setFlag(teamid, context, service, tracingService);
                    }
                }
                if (organizationname.ToLower() == "mmsitphase2" || organizationname.ToLower() == "org49bf57be")
                {
                    try
                    {
                        Dictionary<string, string> AutoShareUrl = Common.FetchConfigDetails(Constants.AutoShareUrl, service);
                        foreach (var value in AutoShareUrl.Values)
                        {
                            url = value.ToString();
                        }
                        string uri = "" + url + "" + teamid;   //E4CECEA5-F959-EA11-A811-000D3A33F637
                        tracingService.Trace(uri);
                        Dictionary<string, string> AutoShareUserName = Common.FetchConfigDetails(Constants.AutoShareUserName, service);
                        foreach (var value in AutoShareUserName.Values)
                        {
                            user = value.ToString();
                        }
                        Dictionary<string, string> AutoShareCred = Common.FetchConfigDetails(Constants.AutoShareCred, service);
                        foreach (var value in AutoShareCred.Values)
                        {
                            cred = value.ToString();
                        }
                        tracingService.Trace("the Instance name is " + "Into the instace" + organizationname);
                        WebClient webclient = new WebClient();
                        string userName = user;
                        string passWord = cred;
                        string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(userName + ":" + passWord));
                        webclient.Headers[HttpRequestHeader.Authorization] = "Basic " + credentials;
                        var resposnestring = webclient.UploadValues(uri, "POST", webclient.QueryString);
                        var responseString = UnicodeEncoding.UTF8.GetString(resposnestring);
                    }
                    catch (Exception ex)
                    {
                        setFlag(teamid, context, service, tracingService);
                    }
                }
                if (organizationname.ToLower() == "org3fd83431" || organizationname.ToLower() == "movehome") //Prod App Name
                {
                    try
                    {
                        Dictionary<string, string> AutoShareUrl = Common.FetchConfigDetails(Constants.AutoShareUrl, service);
                        foreach (var value in AutoShareUrl.Values)
                        {
                            url = value.ToString();
                        }
                        string uri = "" + url + "" + teamid;   //E4CECEA5-F959-EA11-A811-000D3A33F637
                        tracingService.Trace(uri);
                        Dictionary<string, string> AutoShareUserName = Common.FetchConfigDetails(Constants.AutoShareUserName, service);
                        foreach (var value in AutoShareUserName.Values)
                        {
                            user = value.ToString();
                        }
                        Dictionary<string, string> AutoShareCred = Common.FetchConfigDetails(Constants.AutoShareCred, service);
                        foreach (var value in AutoShareCred.Values)
                        {
                            cred = value.ToString();
                        }
                        tracingService.Trace("the Instance name is " + "Into the instace" + organizationname);
                        WebClient webclient = new WebClient();
                        string userName = user;
                        string passWord = cred;
                        string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(userName + ":" + passWord));
                        webclient.Headers[HttpRequestHeader.Authorization] = "Basic " + credentials;
                        var resposnestring = webclient.UploadValues(uri, "POST", webclient.QueryString);
                        var responseString = UnicodeEncoding.UTF8.GetString(resposnestring);
                        //prod need to deploy azure function yet.
                    }
                    catch (Exception ex)
                    {
                        setFlag(teamid, context, service, tracingService);
                    }
                }
                if (organizationname == "mortgageapp")
                {
                    // mortgageapp need to deploy azure function yet.
                }
                if (organizationname == "org64873616" || organizationname.ToLower() == "mmdev")
                {
                    Dictionary<string, string> AutoShareUrl = Common.FetchConfigDetails(Constants.AutoShareUrl, service);
                    foreach (var value in AutoShareUrl.Values)
                    {
                        url = value.ToString();
                    }
                    string uri = "" + url + "" + teamid;   //E4CECEA5-F959-EA11-A811-000D3A33F637
                    tracingService.Trace(uri);
                    Dictionary<string, string> AutoShareUserName = Common.FetchConfigDetails(Constants.AutoShareUserName, service);
                    foreach (var value in AutoShareUserName.Values)
                    {
                        user = value.ToString();
                    }
                    Dictionary<string, string> AutoShareCred = Common.FetchConfigDetails(Constants.AutoShareCred, service);
                    foreach (var value in AutoShareCred.Values)
                    {
                        cred = value.ToString();
                    }
                    tracingService.Trace("the Instance name is " + "Into the instace" + organizationname);
                    WebClient webclient = new WebClient();
                    string userName = user;
                    string passWord = cred;
                    string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(userName + ":" + passWord));
                    webclient.Headers[HttpRequestHeader.Authorization] = "Basic " + credentials;
                    var resposnestring = webclient.UploadValues(uri, "POST", webclient.QueryString);
                    var responseString = UnicodeEncoding.UTF8.GetString(resposnestring);
                }
            }
            catch (Exception ex)
            {
                tracingService.Trace(ex.Message);
            }
        }
        public static void setFlag(Guid teamid, IPluginExecutionContext context, IOrganizationService service, ITracingService trace)
        {
            trace.Trace("Inside Set Flag");
            bool? syncLeads = null;
            bool? syncContacts = null;
            //bool? syncMarketingList = null;

            Entity team = service.Retrieve(Team.EntityName, teamid, new ColumnSet(Team.SyncContacts, Team.SyncLeadLoans, Team.SyncMarketingList, Team.AutoshareContacts, Team.AutoShareleadLoans, Team.AutoShareMarketingList));
            if (team.Attributes.Contains(Team.SyncLeadLoans)) { syncLeads = team.GetAttributeValue<bool>(Team.SyncLeadLoans); }
            if (team.Attributes.Contains(Team.SyncContacts)) { syncContacts = team.GetAttributeValue<bool>(Team.SyncContacts); }
            // if (team.Attributes.Contains(Team.SyncMarketingList)) { syncMarketingList = team.GetAttributeValue<bool>(Team.SyncMarketingList); }

            Entity updateTeam = new Entity(Team.EntityName, teamid);
            var teamContext = (Entity)context.InputParameters["Target"];
            if (teamContext.Attributes.Contains(Team.AutoShareleadLoans))
            {
                bool autoshareLeads = teamContext.GetAttributeValue<bool>(Team.AutoShareleadLoans);

                if (autoshareLeads == true)
                {
                    trace.Trace("The AutoShareLeads" + " __" + autoshareLeads);
                    updateTeam[Team.SyncLeadLoans] = true;
                    service.Update(updateTeam);
                }
                if (autoshareLeads == false)
                {
                    trace.Trace("The AutoShareLeads" + " __" + autoshareLeads);
                    //if (syncLeads == true)
                    //{
                    trace.Trace("The SyncLeads is" + "___" + syncLeads);
                    updateTeam[Team.SyncLeadLoans] = true;
                    service.Update(updateTeam);
                    //}
                    //else
                    //{
                    //    trace.Trace("The SyncLeads is" + "___" + syncLeads);
                    //    updateTeam[Team.SyncLeadLoans] = true;
                    //    service.Update(updateTeam);
                    //}
                }
            }
            if (teamContext.Attributes.Contains(Team.AutoshareContacts))
            {
                bool autosharecontacts = teamContext.GetAttributeValue<bool>(Team.AutoshareContacts);
                if (autosharecontacts == true)
                {
                    trace.Trace("The AutoShareContacts is" + "____" + autosharecontacts);
                    updateTeam[Team.SyncContacts] = true;
                    service.Update(updateTeam);
                }
                if (autosharecontacts == false)
                {
                    trace.Trace("The AutoShareContacts is" + "____" + autosharecontacts);
                    //if (syncContacts == true)
                    //{
                    trace.Trace("The syncContacts is" + "___" + syncContacts);
                    updateTeam[Team.SyncContacts] = true;
                    service.Update(updateTeam);
                    //}
                    //else
                    //{
                    //    trace.Trace("The syncContacts is" + "___" + syncContacts);
                    //    updateTeam[Team.SyncContacts] = true;
                    //    service.Update(updateTeam);
                    //}
                }
            }
            /*if (team.Attributes.ContainsKey(Team.AutoShareMarketingList))
            {
                bool autoShareList = team.GetAttributeValue<bool>(Team.AutoShareMarketingList);
                if (autoShareList != true)
                {
                    trace.Trace("The AutoShareContacts is" + "____" + autoShareList);
                    updateTeam[Team.SyncMarketingList] = true;
                    service.Update(updateTeam);
                }
                else if (autoShareList == false)
                {
                    trace.Trace("The AutoShareContacts is" + "____" + autoShareList);
                    //if (syncMarketingList == true)
                    //{
                    trace.Trace("The syncContacts is" + "___" + syncMarketingList);
                    updateTeam[Team.SyncMarketingList] = true;
                    service.Update(updateTeam);
                    // }
                    //else
                    //{
                    //    trace.Trace("The syncContacts is" + "___" + syncMarketingList);
                    //    updateTeam[Team.SyncMarketingList] = true;
                    //    service.Update(updateTeam);
                    //}
                }
            }*/
        }
    }
}