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
    public class ShareMarketingListOnCreate : IPlugin
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
            Entity list = (Entity)context.InputParameters["Target"];
            if (context.MessageName.ToLower() == "create")
            {
              /*  try
                {
                    EntityReference listOwner = null;
                    Entity user = null;
                    Entity mainTeam = null;
                    if (list.Attributes.Contains(Marketinglist.Owner))
                    {
                        listOwner = list.GetAttributeValue<EntityReference>(Marketinglist.Owner);
                    }
                    if (listOwner.LogicalName != Team.EntityName)
                    {
                        user = service.Retrieve(listOwner.LogicalName, listOwner.Id, new ColumnSet(SystemUser.FirstName, SystemUser.LastName, SystemUser.LOA));
                    }

                    if (user.Attributes.Contains(SystemUser.LOA))
                    {
                        EntityReference team = user.GetAttributeValue<EntityReference>(SystemUser.LOA);
                        mainTeam = service.Retrieve(team.LogicalName, team.Id, new ColumnSet(Team.Name, Team.AutoShareleadLoans, Team.AutoShareMarketingList));
                        if (mainTeam.Attributes.Contains(Team.AutoShareMarketingList))
                        {
                            bool IsAutoShare = mainTeam.GetAttributeValue<bool>(Team.AutoShareMarketingList);
                            if (IsAutoShare == true)
                            {
                                //shareRecordOnCreate(mainTeam, list, service, tracingservice);
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    tracingservice.Trace(ex.Message.ToString());
                    throw new InvalidPluginExecutionException("Context Not Found");
                }*/
            }
        }
       /* public static void shareRecordOnCreate(Entity team, Entity list, IOrganizationService service, ITracingService tracingService)
        {
            tracingService.Trace("Sharing Records here");
            EntityReference sharewithTeam = new EntityReference();
            GrantAccessRequest grant = new GrantAccessRequest();
            grant.Target = new EntityReference(list.LogicalName, list.Id);

            PrincipalAccess principal = new PrincipalAccess();
            principal.Principal = new EntityReference(team.LogicalName, team.Id);
            principal.AccessMask = AccessRights.ReadAccess | AccessRights.WriteAccess;
            grant.PrincipalAccess = principal;

            GrantAccessResponse granted = (GrantAccessResponse)service.Execute(grant);
            tracingService.Trace("The Records are shared with the Team members");
        }*/
    }
}
