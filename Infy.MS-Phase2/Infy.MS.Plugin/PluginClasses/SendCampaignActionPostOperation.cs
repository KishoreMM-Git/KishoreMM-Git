using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xrm;
using XRMExtensions;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;

namespace Infy.MS.Plugins.PluginClasses
{
    public class SendCampaignActionPostOperation : BasePlugin
    {
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            if (context == null)
            {
                throw new InvalidPluginExecutionException("Context not found");
            }

            if (context.MessageName.ToLower() != "ims_sendcampaign") { return; }

            #region Variable Declaration

            EntityReference marketingList = null;
            EntityReference campaign = null;
            EntityReference email = null;
            Entity marketingListObj = null;
            Entity campaignObj = null;
            Entity emailObj = null;

            #endregion

            // get Marketing List
            if (context.InputParameters.Contains("MarketingList") && context.InputParameters["MarketingList"] is EntityReference)
            {
                marketingList = (EntityReference)context.InputParameters["MarketingList"];
                //Retrieve Marketing List Name and the value of IsML Updated 
                marketingListObj = context.Retrieve(Marketinglist.EntityName, marketingList.Id, new ColumnSet(Marketinglist.IsMLUpdated, Marketinglist.PrimaryName));
            }
            // get Campaign
            if (context.InputParameters.Contains("Campaign") && context.InputParameters["Campaign"] is EntityReference)
            {
                campaign = (EntityReference)context.InputParameters["Campaign"];
                //Retrieve Campaign Name 
                campaignObj = context.Retrieve(Campaign.EntityName, campaign.Id, new ColumnSet(Campaign.PrimaryName));
            }

            //get Email
            if (context.InputParameters.Contains("Email") && context.InputParameters["Email"] is EntityReference)
            {
                email = (EntityReference)context.InputParameters["Email"];
                //Retrieve Email Name and Template ID
                emailObj = context.Retrieve(AEMEmail.EntityName, email.Id, new ColumnSet(AEMEmail.PrimaryName, AEMEmail.TemplateId));
            }

            Guid callingUserId = context.UserId;
            var userName = context.Retrieve(SystemUser.EntityName, callingUserId, new ColumnSet(SystemUser.FullName));

            if (marketingList != null && email != null)
            {
                //create ims_ACSCampaignRequest record
                Entity objACSCampaignRequest = new Entity(ACSCampaignRequest.EntityName);
                string acsCampaignName = string.Empty;

                if (userName != null && userName.Attributes.Contains(SystemUser.FullName))
                {
                    acsCampaignName += userName.GetAttributeValue<string>(SystemUser.FullName);
                }

                if (marketingListObj != null && marketingListObj.Attributes.Contains(Marketinglist.PrimaryName))
                {
                    acsCampaignName += "-" + marketingListObj.GetAttributeValue<string>(Marketinglist.PrimaryName);
                }

                if (campaignObj != null && campaignObj.Attributes.Contains(Campaign.PrimaryName))
                {
                    acsCampaignName += "-" + campaignObj.GetAttributeValue<string>(Campaign.PrimaryName);
                }

                if (emailObj != null && emailObj.Attributes.Contains(AEMEmail.PrimaryName))
                {
                    acsCampaignName += "-" + emailObj.GetAttributeValue<string>(AEMEmail.PrimaryName);
                }

                objACSCampaignRequest[ACSCampaignRequest.PrimaryName] = acsCampaignName;
                objACSCampaignRequest[ACSCampaignRequest.MarketingList] = new EntityReference(marketingList.LogicalName, marketingList.Id);
                if (campaign != null)
                    objACSCampaignRequest[ACSCampaignRequest.Campaign] = new EntityReference(campaign.LogicalName, campaign.Id);
                objACSCampaignRequest[ACSCampaignRequest.Email] = new EntityReference(email.LogicalName, email.Id);
                objACSCampaignRequest[ACSCampaignRequest.Owner] = new EntityReference("systemuser", callingUserId);
                objACSCampaignRequest[ACSCampaignRequest.IsSynced] = false;

                if (marketingListObj != null && marketingListObj.Attributes.Contains(ACSCampaignRequest.IsMLUpdated))
                {
                    objACSCampaignRequest[ACSCampaignRequest.IsMLUpdated] = marketingListObj.GetAttributeValue<Boolean>(Marketinglist.IsMLUpdated);
                }
                if (emailObj != null && emailObj.Attributes.Contains(AEMEmail.TemplateId))
                {
                    objACSCampaignRequest[ACSCampaignRequest.TemplateId] = emailObj.GetAttributeValue<string>(AEMEmail.TemplateId);
                }

                //Impersonation using Admin ID
                context.SystemOrganizationService.Create(objACSCampaignRequest);
            }
        }

        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "ims_SendCampaign");
        }
    }
}
