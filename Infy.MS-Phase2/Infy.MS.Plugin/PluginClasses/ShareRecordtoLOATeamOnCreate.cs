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

namespace Infy.MS.Plugins.PluginClasses
{
    public class ShareRecordtoLOATeamOnCreate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            Guid ownerId = Guid.Empty;
            string etyName = string.Empty;
            Guid teamId = Guid.Empty;
            string teamName = string.Empty;

            ITracingService tracingservice = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.  
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            if (context == null)
            {
                throw new InvalidPluginExecutionException("Context Not Found");
            }
            if (context.MessageName.ToLower() != "create")
            {
                return;
            }
            tracingservice.Trace("PLugis executes");
            Entity lead = (Entity)context.InputParameters["Target"];
            tracingservice.Trace("Errorthrows here" + lead.LogicalName,lead.Id); ;

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                if (context.MessageName.ToLower() == "create")
                {
                    tracingservice.Trace("Create");
                    try
                    {
                        tracingservice.Trace("Inside the try statement");
                        if ((lead.LogicalName == Lead.EntityName) || (lead.LogicalName == Contact.EntityName) || (lead.LogicalName == Loan.EntityName))
                        {
                            tracingservice.Trace("The Target Entity Name is ", lead.LogicalName);
                            Entity ety = service.Retrieve(lead.LogicalName, lead.Id, new ColumnSet("ownerid"));
                            if (ety.Attributes.Contains("ownerid"))
                            {
                                EntityReference currentUser = ety.GetAttributeValue<EntityReference>("ownerid"); //for contactss,lead,loans
                                ownerId = currentUser.Id;
                                etyName = currentUser.LogicalName;
                                tracingservice.Trace("The System User Entity Name is " + currentUser.LogicalName);
                            }

                            //Fetching the Default LOA from the User Entity.
                            Entity userEty = service.Retrieve(etyName, ownerId, new ColumnSet(SystemUser.LOA));
                            tracingservice.Trace("retreving the LOA value from the user ety");
                            if (userEty.Attributes.Contains(SystemUser.LOA))
                            {
                                EntityReference teamref = userEty.GetAttributeValue<EntityReference>(SystemUser.LOA);
                                teamId = teamref.Id;
                                teamName = teamref.LogicalName;
                                tracingservice.Trace("The Team name is " + teamref.LogicalName);
                            }


                            //Using the Default LOA Team From the user Entity get the Team Members values.
                            Entity teamEty = service.Retrieve(teamName, teamId, new ColumnSet(true));

                            if ((teamEty.GetAttributeValue<bool>(Team.AutoShareleadLoans) == true) && ((lead.LogicalName == Lead.EntityName) || (lead.LogicalName == Loan.EntityName)))
                            {
                                tracingservice.Trace("Executing the share records functionality here for leads and loans");
                                shareRecords(teamEty, lead, tracingservice, service);
                            }
                            else if ((teamEty.GetAttributeValue<bool>(Team.AutoshareContacts) == true) && (lead.LogicalName == Contact.EntityName))
                            {
                                tracingservice.Trace("Executing the share records functionality here for  contacts");
                                shareRecords(teamEty, lead, tracingservice, service);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        tracingservice.Trace(ex.Message);
                        //throw new InvalidPluginExecutionException();
                    }
                }
            }

        }

        public static void shareRecords(Entity teamEty, Entity lead, ITracingService tracingservice, IOrganizationService service)
        {
            tracingservice.Trace("Sharing Records here");
            EntityReference sharewithTeam = new EntityReference();
            GrantAccessRequest grant = new GrantAccessRequest();
            grant.Target = new EntityReference(lead.LogicalName, lead.Id); 

            PrincipalAccess principal = new PrincipalAccess();
            principal.Principal = new EntityReference(teamEty.LogicalName, teamEty.Id);
            principal.AccessMask = AccessRights.ReadAccess | AccessRights.WriteAccess;
            grant.PrincipalAccess = principal;

            GrantAccessResponse granted = (GrantAccessResponse)service.Execute(grant);
            //Recods are shared with the teams
            tracingservice.Trace("The Records are shared with the Team members");
        }


        //public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        //{
        //    yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "Create");
        //}
    }
}