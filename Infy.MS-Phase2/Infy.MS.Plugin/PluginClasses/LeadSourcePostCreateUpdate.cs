using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xrm;
using XRMExtensions;

namespace Infy.MS.Plugins
{
    public class LeadSourcePostCreateUpdate : BasePlugin
    {
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            var targetEntity = context.GetTargetEntity<Entity>();
            var postImageEntity = context.GetFirstPostImage<Entity>();
            var postingURLBaseApi = context.GetConfigValue<string>(Constants.PostingURL_BaseApi, Constants.LeadSourcePostCreateUpdate);
            var postingURLClientId = context.GetConfigValue<string>(Constants.PostingURL_ClientId, Constants.LeadSourcePostCreateUpdate);
            if (postImageEntity != null)
            {
                if(!string.IsNullOrEmpty(postingURLClientId) &&postImageEntity.Contains(LeadSource.CampaignId) && postImageEntity.Contains(LeadSource.LeadSourceGroup))
                {
                    if (postingURLBaseApi.Contains("{provider}") && postingURLBaseApi.Contains("{clientid}") && postingURLBaseApi.Contains("{campaignid}"))
                    {
                        var campaignId = postImageEntity.GetAttributeValue<string>(LeadSource.CampaignId);
                        var leadsourceGroup = postImageEntity.GetAttributeValue<EntityReference>(LeadSource.LeadSourceGroup).Name;
                        postingURLBaseApi = postingURLBaseApi.Replace("{provider}", leadsourceGroup);
                        postingURLBaseApi = postingURLBaseApi.Replace("{clientid}", postingURLClientId);
                        postingURLBaseApi = postingURLBaseApi.Replace("{campaignid}", campaignId);
                        Entity leadSourceEntity = new Entity(postImageEntity.LogicalName);
                        leadSourceEntity.Id = postImageEntity.Id;
                        leadSourceEntity[LeadSource.Notes] = postingURLBaseApi;
                        context.Update(leadSourceEntity);
                    }

                }

            }
        }

        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Asynchronous, "Create");
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Asynchronous, "Update");
        }
    }
}
