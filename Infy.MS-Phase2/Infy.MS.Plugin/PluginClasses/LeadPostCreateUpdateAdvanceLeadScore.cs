using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Xrm;
using XRMExtensions;
using System.Text.RegularExpressions;
using System.ServiceModel;
using Microsoft.Crm.Sdk.Messages;
using System.Net;
using System.Runtime.Serialization.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System.Web;

namespace Infy.MS.Plugins
{
    public class LeadPostCreateUpdateAdvanceLeadScore : BasePlugin
    {
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            if (context == null)
            {
                throw new InvalidPluginExecutionException("Context not found");
            }

            //if (context.MessageName.ToLower() == "update" && context.Depth > 1)
            //    return;

            List<Common.Mapping> mappings = new List<Common.Mapping>();
            string errorMessage = string.Empty;
            Dictionary<string, string> dcConfigDetails = new Dictionary<string, string>();
            Common objCommon = new Common();
            Entity leadContext = null;
            Entity leadPostImage = null;
            Guid importDataMasterId = Guid.Empty;
            bool dataChanged = false;
            string errorFromASL = string.Empty;
            try
            {
                IOrganizationService service = context.SystemOrganizationService;
                string serviceUrl = string.Empty;
                string response = string.Empty;
                try
                {
                    if (context.PostEntityImages.Contains("PostImage") && context.PostEntityImages["PostImage"] is Entity)
                    {
                        leadPostImage = (Entity)context.PostEntityImages["PostImage"];
                    }

                    if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                    {
                        leadContext = (Entity)context.InputParameters["Target"];
                    }

                    //return if plugin primary entity is not Lead
                    if (leadContext.LogicalName != Lead.EntityName)
                        return;

                    importDataMasterId = Common.GetImportDataMasterIdBasedOnName(Constants.ALSImportDataMaster, service);
                    //fetch all configurations related to StagingPostCreate
                    dcConfigDetails = Common.FetchConfigDetails(Constants.ALSAppConfigSetupName, service);
                    if (importDataMasterId != Guid.Empty)
                    {
                        if (objCommon.FetchMappings(importDataMasterId, ref mappings, service, ref errorMessage))
                        {
                            string QueryStringParameters = string.Empty;
                            QueryStringParameters += "&lead_id=" + leadContext.Id;
                            foreach (Common.Mapping objMapping in mappings)
                            {
                                object value = LeadPostCreateUpdateAdvanceLeadScore.GetValueFromLeadEntity(leadContext, leadPostImage, objMapping, ref errorMessage, ref dataChanged, service);

                                //Convert DOB to Age [Year.Month format]
                                if (objMapping.Target == "age")
                                {
                                    if (value.ToString() != string.Empty)
                                        value = LeadPostCreateUpdateAdvanceLeadScore.CalculateYourAge(Convert.ToDateTime(value));
                                }

                                //Convert Marital Status field to 1.0 – Married 0.0 – All other Empty string if marital status is not known
                                if (objMapping.Target == "married")
                                {
                                    if (value.ToString() != string.Empty)
                                    {
                                        if (value.ToString().Equals("Married", StringComparison.OrdinalIgnoreCase))
                                        {
                                            value = "1.0";
                                        }
                                        else
                                        {
                                            value = "0.0";
                                        }
                                    }
                                }
                                QueryStringParameters += "&" + objMapping.Target + "=" + value;
                            }

                            //Call ALS Azure function To Get ALS
                            //Check if any of the input parameters changed
                            if (dataChanged)
                            {
                                string alsFunctionURL = objCommon.GetMessage(Constants.ALSFunctionURL, dcConfigDetails);
                                int leadScore = 0;
                                string contributingFactor = string.Empty;
                                if (!string.IsNullOrEmpty(alsFunctionURL))
                                {
                                    using (WebClient client = new WebClient())
                                    {
                                        //string jsonObject = string.Empty;
                                        //var webClient = new WebClient();
                                        //webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
                                        //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                                        //WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultNetworkCredentials;

                                        // the url for our Azure Function
                                        serviceUrl = alsFunctionURL;
                                        //QueryStringParameters = HttpUtility.HtmlEncode(QueryStringParameters);
                                        serviceUrl += QueryStringParameters;
                                        // upload the data using Post mehtod
                                        try
                                        {
                                            byte[] data = client.DownloadData(serviceUrl);
                                            response = Encoding.UTF8.GetString(data);
                                        }
                                        catch (WebException ex)
                                        {
                                            errorFromASL = ex.Message;
                                            throw new Exception(ex.Message);
                                        }
                                        try
                                        {
                                            var objects = JArray.Parse(response);
                                            foreach (JObject root in objects)
                                            {
                                                leadScore = (int)root["lead_score"];
                                                contributingFactor = (string)root["significant_variables"];
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            errorFromASL = response;
                                            throw new Exception(ex.Message);
                                        }
                                    }

                                    //Update ALS, factor Contribution to Lead
                                    Entity leadUpdate = new Entity(Lead.EntityName);
                                    leadUpdate.Id = leadContext.Id;
                                    if (!string.IsNullOrEmpty(contributingFactor))
                                        leadUpdate["ims_leadscorecontributionfactors"] = contributingFactor;
                                    //if (leadScore != 0)
                                    leadUpdate["ims_advancedleadscore"] = leadScore;
                                    leadUpdate["ims_alserror"] = null;
                                    service.Update(leadUpdate);
                                }
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    errorMessage += ex.Message + Environment.NewLine;
                    errorMessage += ex.InnerException + Environment.NewLine;
                    errorMessage += "Request URL:" + serviceUrl + Environment.NewLine;
                    errorMessage += "Response from ASL:" + response + Environment.NewLine;
                    if (!string.IsNullOrEmpty(errorMessage))
                        objCommon.CreateErrorLog("ALS ERROR:" + leadContext.Id.ToString(), errorMessage, service);
                    Entity leadUpdate = new Entity(Lead.EntityName);
                    leadUpdate.Id = leadContext.Id;
                    leadUpdate["ims_alserror"] = errorFromASL;//ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + "Response from ASL:" + response;
                    service.Update(leadUpdate);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }

        static string CalculateYourAge(DateTime Dob)
        {
            try
            {
                DateTime Now = DateTime.Now;
                int Years = new DateTime(DateTime.Now.Subtract(Dob).Ticks).Year - 1;
                DateTime PastYearDate = Dob.AddYears(Years);
                int Months = 0;
                for (int i = 1; i <= 12; i++)
                {
                    if (PastYearDate.AddMonths(i) == Now)
                    {
                        Months = i;
                        break;
                    }
                    else if (PastYearDate.AddMonths(i) >= Now)
                    {
                        Months = i - 1;
                        break;
                    }
                }
                return Years + "." + Months;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
            //return Years + "." + Months;
        }

        static object GetValueFromLeadEntity(Entity leadContext, Entity leadPostImage, Common.Mapping mapping, ref string errorMessage, ref bool dataChanged, IOrganizationService service)
        {
            object value = string.Empty;
            if (mapping.SourceDatatype == (int)ImportDetailsMapping.SourceDataType_OptionSet.SingleLineOfText ||
                        mapping.SourceDatatype == (int)ImportDetailsMapping.SourceDataType_OptionSet.MultipleLineOfText)
            {
                if (leadContext.Attributes.Contains(mapping.Source) && leadContext[mapping.Source] != null)
                {
                    value = leadContext.GetAttributeValue<string>(mapping.Source);
                    dataChanged = true;
                }
                else if (leadPostImage.Attributes.Contains(mapping.Source) && leadPostImage[mapping.Source] != null)
                {
                    value = leadPostImage.GetAttributeValue<string>(mapping.Source);
                }
                else
                {
                    value = string.Empty;
                }
            }
            else if (mapping.SourceDatatype == (int)ImportDetailsMapping.SourceDataType_OptionSet.WholeNumber)
            {
                if (leadContext.Attributes.Contains(mapping.Source) && leadContext[mapping.Source] != null)
                {
                    value = leadContext.GetAttributeValue<int>(mapping.Source).ToString();
                    dataChanged = true;
                }
                else if (leadPostImage.Attributes.Contains(mapping.Source) && leadPostImage[mapping.Source] != null)
                    value = leadPostImage.GetAttributeValue<int>(mapping.Source).ToString();
                else
                    value = string.Empty;
            }
            else if (mapping.SourceDatatype == (int)ImportDetailsMapping.SourceDataType_OptionSet.DateTime)
            {
                if (leadContext.Attributes.Contains(mapping.Source) && leadContext[mapping.Source] != null)
                {
                    value = leadContext.GetAttributeValue<DateTime>(mapping.Source).ToShortDateString();
                    dataChanged = true;
                }
                else if (leadPostImage.Attributes.Contains(mapping.Source) && leadPostImage[mapping.Source] != null)
                    value = leadPostImage.GetAttributeValue<DateTime>(mapping.Source).ToShortDateString();
                else
                    value = string.Empty;
            }
            else if (mapping.SourceDatatype == (int)ImportDetailsMapping.SourceDataType_OptionSet.Currency)
            {
                if (leadContext.Attributes.Contains(mapping.Source) && leadContext[mapping.Source] != null)
                {
                    value = leadContext.GetAttributeValue<Money>(mapping.Source).Value;
                    dataChanged = true;
                }
                else if (leadPostImage.Attributes.Contains(mapping.Source) && leadPostImage[mapping.Source] != null)
                    value = leadPostImage.GetAttributeValue<Money>(mapping.Source).Value;
                else
                    value = string.Empty;
            }
            else if (mapping.SourceDatatype == (int)ImportDetailsMapping.SourceDataType_OptionSet.Decimal)
            {
                if (leadContext.Attributes.Contains(mapping.Source) && leadContext[mapping.Source] != null)
                {
                    value = leadContext.GetAttributeValue<decimal>(mapping.Source);
                    dataChanged = true;
                }
                else if (leadPostImage.Attributes.Contains(mapping.Source) && leadPostImage[mapping.Source] != null)
                    value = leadPostImage.GetAttributeValue<decimal>(mapping.Source);
                else
                    value = string.Empty;
            }
            else if (mapping.SourceDatatype == (int)ImportDetailsMapping.SourceDataType_OptionSet.TwoOptions)
            {
                if (leadContext.Attributes.Contains(mapping.Source) && leadContext[mapping.Source] != null)
                {
                    value = leadContext.GetAttributeValue<bool>(mapping.Source) == true ? "True" : "False";
                    dataChanged = true;
                }
                else if (leadPostImage.Attributes.Contains(mapping.Source) && leadPostImage[mapping.Source] != null)
                    value = leadPostImage.GetAttributeValue<bool>(mapping.Source) == true ? "True" : "False";
                else
                    value = string.Empty;
            }
            else if (mapping.SourceDatatype == (int)ImportDetailsMapping.SourceDataType_OptionSet.Optonset)
            {
                int optionSetValue = 0;
                if (leadContext.Attributes.Contains(mapping.Source) && leadContext[mapping.Source] != null)
                {
                    optionSetValue = leadContext.GetAttributeValue<OptionSetValue>(mapping.Source).Value;
                    value = LeadPostCreateUpdateAdvanceLeadScore.GetoptionsetTextOnValue(service, Lead.EntityName, mapping.Source, optionSetValue);
                    dataChanged = true;
                }
                else if (leadPostImage.Attributes.Contains(mapping.Source) && leadPostImage[mapping.Source] != null)
                {
                    optionSetValue = leadPostImage.GetAttributeValue<OptionSetValue>(mapping.Source).Value;
                    value = LeadPostCreateUpdateAdvanceLeadScore.GetoptionsetTextOnValue(service, Lead.EntityName, mapping.Source, optionSetValue);
                }
                else
                    value = string.Empty;
            }
            else if (mapping.SourceDatatype == (int)ImportDetailsMapping.SourceDataType_OptionSet.Lookup)
            {
                Guid lookupRecordId = Guid.Empty;
                if (leadContext.Attributes.Contains(mapping.Source) && leadContext[mapping.Source] != null)
                {
                    lookupRecordId = leadContext.GetAttributeValue<EntityReference>(mapping.Source).Id;
                    value = LeadPostCreateUpdateAdvanceLeadScore.GetLookUpPrimaryKeyName(mapping, lookupRecordId, service);
                    dataChanged = true;
                }
                else if (leadPostImage.Attributes.Contains(mapping.Source) && leadPostImage[mapping.Source] != null)
                {
                    lookupRecordId = leadPostImage.GetAttributeValue<EntityReference>(mapping.Source).Id;
                    value = LeadPostCreateUpdateAdvanceLeadScore.GetLookUpPrimaryKeyName(mapping, lookupRecordId, service);
                }
                else
                    value = string.Empty;
            }
            return value;
        }
        static string GetLookUpPrimaryKeyName(Common.Mapping mapping, Guid lookupId, IOrganizationService service)
        {
            string primaryKeyValue = string.Empty;
            var lookupEntityName = service.Retrieve(mapping.LookupEntityName, lookupId, new ColumnSet(mapping.LookupEntityAttribute));
            if (lookupEntityName != null && lookupEntityName.Attributes.Contains(mapping.LookupEntityAttribute))
            {
                primaryKeyValue = lookupEntityName.GetAttributeValue<string>(mapping.LookupEntityAttribute);
            }
            return primaryKeyValue;
        }

        public static String GetoptionsetTextOnValue(IOrganizationService service, string entityName, string attributeName, int selectedValue)
        {
            RetrieveEntityRequest retrieveDetails = new RetrieveEntityRequest
            {
                EntityFilters = EntityFilters.All,
                LogicalName = entityName
            };
            RetrieveEntityResponse retrieveEntityResponseObj = (RetrieveEntityResponse)service.Execute(retrieveDetails);
            Microsoft.Xrm.Sdk.Metadata.EntityMetadata metadata = retrieveEntityResponseObj.EntityMetadata;
            Microsoft.Xrm.Sdk.Metadata.PicklistAttributeMetadata picklistMetadata = metadata.Attributes.FirstOrDefault(attribute => String.Equals(attribute.LogicalName, attributeName, StringComparison.OrdinalIgnoreCase)) as Microsoft.Xrm.Sdk.Metadata.PicklistAttributeMetadata;
            Microsoft.Xrm.Sdk.Metadata.OptionSetMetadata options = picklistMetadata.OptionSet;

            IList<OptionMetadata> OptionsList = (from o in options.Options
                                                 where o.Value.Value == selectedValue
                                                 select o).ToList();
            string optionsetLabel = (OptionsList.First()).Label.UserLocalizedLabel.Label;
            return optionsetLabel;
        }
        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "Create");
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "Update");
        }
    }
}
