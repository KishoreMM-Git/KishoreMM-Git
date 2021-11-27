using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Crm.Sdk.Messages;
using Newtonsoft.Json;


namespace XRMExtensions
{
    public class Common
    {
        public class Mapping
        {
            public string Source;
            public string Target;
            public string TargetEntity;
            public int DataType;
            public int SourceDatatype;
            public string LookupEntityAttribute;
            public string LookupEntityName;
            public bool Mandatory;
            public string CrmDisplayName;
            public int MaxLengthAllowed;
            public bool CreatelookupRecord;
            public string value;
            public string ImportDataMaster;
            public string DataMaster;
            public string DefaultValue;
            public bool RevertResult;
        }
        public EntityReference manualImportReference = null;
        public EntityReference defaultTeamReference = null;
        public Guid contextUserId = Guid.Empty;
        public Guid stagingRecordOwner = Guid.Empty;
        //public bool isLoanOfficer = false;
        public IOrganizationService organizationServiceStagingOwner = null;
        public static Dictionary<string, string> FetchConfigDetails(string appConfigSetupName, IOrganizationService service)
        {
            Dictionary<string, string> dcConfig = null;
            string key = string.Empty;
            string value = string.Empty;
            try
            {
                List<Entity> lstConfigs = ExecutionConfig.ReadExecutionConfig(service, appConfigSetupName);
                if (lstConfigs != null && lstConfigs.Count > 0)
                {
                    dcConfig = new Dictionary<string, string>();
                    foreach (Entity config in lstConfigs)
                    {
                        key = config.Attributes.Contains(Configuration.PrimaryName) ? config.GetAttributeValue<string>(Configuration.PrimaryName) : string.Empty;
                        if (config.Attributes.Contains(Configuration.Value))
                        {
                            value = config.GetAttributeValue<string>(Configuration.Value);
                        }
                        else
                        {
                            value = config.Attributes.Contains(Configuration.ValueMultiline) ? config.GetAttributeValue<string>(Configuration.ValueMultiline) : string.Empty;
                        }
                        dcConfig.Add(key, value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching configurations: " + ex.Message);
            }
            return dcConfig;
        }

        public static Dictionary<String, String> FetchOptionSetList(IOrganizationService service, String entityName, String fieldName)
        {
            Dictionary<String, String> dcOptionDic = new Dictionary<String, String>();

            try
            {
                if (String.Equals(entityName, "GlobalOptionSet", StringComparison.OrdinalIgnoreCase))
                {
                    #region "--- Global OptionSet ---"
                    RetrieveOptionSetRequest retrieveOptionSetRequest = new RetrieveOptionSetRequest
                    {
                        Name = fieldName
                    };

                    // Execute the request.
                    RetrieveOptionSetResponse retrieveOptionSetResponse = (RetrieveOptionSetResponse)service.Execute(retrieveOptionSetRequest);

                    // Access the retrieved OptionSetMetadata.
                    OptionSetMetadata retrievedOptionSetMetadata = (OptionSetMetadata)retrieveOptionSetResponse.OptionSetMetadata;

                    // Get the current options list for the retrieved attribute.
                    OptionMetadata[] optionList = retrievedOptionSetMetadata.Options.ToArray();

                    for (int optionCount = 0; optionCount < optionList.Length; optionCount++)
                    {
                        dcOptionDic.Add(optionList[optionCount].Label.UserLocalizedLabel.Label, optionList[optionCount].Value.ToString());
                    }
                    return dcOptionDic;
                    #endregion
                }
                else
                {
                    #region "--- Entity OptionSet ---"
                    RetrieveAttributeRequest attributeRequest = new RetrieveAttributeRequest
                    {
                        EntityLogicalName = entityName,
                        LogicalName = fieldName,
                        RetrieveAsIfPublished = true
                    };
                    // Execute the request
                    RetrieveAttributeResponse attributeResponse = (RetrieveAttributeResponse)service.Execute(attributeRequest);
                    OptionMetadata[] optionList = (((Microsoft.Xrm.Sdk.Metadata.EnumAttributeMetadata)(attributeResponse.AttributeMetadata)).OptionSet.Options).ToArray();
                    for (int optionCount = 0; optionCount < optionList.Length; optionCount++)
                    {
                        dcOptionDic.Add(optionList[optionCount].Label.UserLocalizedLabel.Label, optionList[optionCount].Value.ToString());
                    }
                    return dcOptionDic;
                    #endregion
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetId(string lookupEntityName, string filterAttributeName, Mapping mapping, IOrganizationService service, ref string errorMessage)
        {
            string filterAttributeValue = WebUtility.HtmlEncode(mapping.value);
            lookupEntityName = lookupEntityName.Trim();
            Guid entityId = Guid.Empty;
            try
            {
                var fetchXml = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
               "<entity name='" + lookupEntityName + "'>" +
               "<attribute name='" + lookupEntityName + "id" + "' />" +
               "<filter type='and'>" +
                 "<condition attribute='" + filterAttributeName + "' operator='eq' value='" + filterAttributeValue + "'/>" +
                 (lookupEntityName == "systemuser" ? "<condition attribute='isdisabled' operator='eq' value='0'/>" : (lookupEntityName != "lead") ?
                 "<condition attribute='statecode' operator='eq' value='0'/>" : string.Empty) +
               "</filter></entity></fetch>";

                EntityCollection ecEntities = service.RetrieveMultiple(new FetchExpression(fetchXml));
                if (ecEntities != null && ecEntities.Entities.Count > 0)
                {
                    Entity objEntity = ecEntities.Entities[0];
                    if (objEntity != null)
                    {
                        entityId = objEntity.Id;
                    }
                }
                //Create new lookup record if flag is set to true in mappings
                if (mapping.CreatelookupRecord && entityId == Guid.Empty)
                {
                    entityId = CreateEntityRecord(mapping, service, ref errorMessage);
                }
            }
            catch (Exception ex)
            {
                entityId = Guid.Empty;
            }
            return (entityId == Guid.Empty ? string.Empty : entityId.ToString());
        }

        public static Guid GetImportDataMasterIdBasedOnName(string idmName, IOrganizationService service)
        {
            Guid idmId = Guid.Empty;
            try
            {
                var queryIDM = new QueryExpression(ImportDataMaster.EntityName);
                queryIDM.TopCount = 1;
                queryIDM.ColumnSet.AddColumns(ImportDataMaster.PrimaryName, ImportDataMaster.PrimaryKey);
                queryIDM.Criteria.AddCondition(ImportDataMaster.PrimaryName, ConditionOperator.Equal, idmName);
                queryIDM.Criteria.AddCondition(ImportDataMaster.Status, ConditionOperator.Equal, (int)ImportDataMaster.Status_OptionSet.Active);
                EntityCollection ecIDM = service.RetrieveMultiple(queryIDM);
                if (ecIDM != null && ecIDM.Entities.Count > 0)
                {
                    idmId = ecIDM.Entities[0].Id;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return idmId;
        }

        public static Guid GetImportDataMasterAndDefaultTeam(string importProcessName, ref EntityReference defaultTeam, IOrganizationService service)
        {
            Guid idmId = Guid.Empty;
            try
            {
                var queryIDM = new QueryExpression(ImportProcess.EntityName);
                queryIDM.TopCount = 1;
                queryIDM.ColumnSet.AddColumns(ImportProcess.ImportDataMaster, ImportProcess.DefaultTeam);
                queryIDM.Criteria.AddCondition(ImportProcess.PrimaryName, ConditionOperator.Equal, importProcessName);
                queryIDM.Criteria.AddCondition(ImportProcess.Status, ConditionOperator.Equal, (int)ImportProcess.Status_OptionSet.Active);
                EntityCollection ecIDM = service.RetrieveMultiple(queryIDM);
                if (ecIDM != null && ecIDM.Entities.Count > 0)
                {
                    Entity importProcess = ecIDM.Entities[0];
                    if (importProcess != null)
                    {
                        if (importProcess.Attributes.Contains(ImportProcess.ImportDataMaster) && importProcess.Attributes[ImportProcess.ImportDataMaster] != null)
                        {
                            idmId = importProcess.GetAttributeValue<EntityReference>(ImportProcess.ImportDataMaster).Id;
                        }

                        if (importProcess.Attributes.Contains(ImportProcess.DefaultTeam) && importProcess.Attributes[ImportProcess.DefaultTeam] != null)
                        {
                            defaultTeam = importProcess.GetAttributeValue<EntityReference>(ImportProcess.DefaultTeam);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return idmId;
        }

        public void MandatoryValidation(Entity sourceEntity, List<Mapping> mapings, ref bool ValidationStatus, ref bool canReturn, ref string errorMessage, Dictionary<string, string> dcConfigDetails)
        {
            foreach (Mapping objMapping in mapings)
            {
                if (objMapping.Mandatory && objMapping.Source != null)//mandatory data check
                {
                    if (!sourceEntity.Contains(objMapping.Source))
                    {
                        //update validationmessage, validationstatus->false, canreturn->true
                        UpdateValidationMessage(string.Format(GetMessage(Constants.ValidationError_MandatoryCheck, dcConfigDetails), objMapping.CrmDisplayName), ref errorMessage);
                        //ValidationStatus = false;
                        //canReturn = true;
                    }
                }
                if (objMapping.MaxLengthAllowed != 0 && sourceEntity.Contains(objMapping.Source) &&
                    (objMapping.SourceDatatype == (int)ImportDetailsMapping.SourceDataType_OptionSet.SingleLineOfText) || objMapping.SourceDatatype == (int)ImportDetailsMapping.SourceDataType_OptionSet.MultipleLineOfText)
                {
                    string value = sourceEntity.GetAttributeValue<string>(objMapping.Source);
                    if (!string.IsNullOrEmpty(value) && value.Length > objMapping.MaxLengthAllowed)//maximum length check
                    {
                        //update validationmessage, validationstatus->false, canreturn->true
                        UpdateValidationMessage(string.Format(GetMessage(Constants.ValidationError_MaxLengthCheck, dcConfigDetails), objMapping.CrmDisplayName, objMapping.MaxLengthAllowed), ref errorMessage);
                        //ValidationStatus = false;
                        //canReturn = true;
                    }
                }
            }
        }

        public bool FetchMappings(Guid importDataMasterId, ref List<Common.Mapping> mappings, IOrganizationService service, ref string errorMessage)
        {
            try
            {
                var queryMappings = new QueryExpression(ImportDetailsMapping.EntityName);
                queryMappings.ColumnSet = new ColumnSet(true);
                queryMappings.Criteria.AddCondition(ImportDetailsMapping.ImportDataMaster, ConditionOperator.Equal, importDataMasterId);
                queryMappings.Criteria.AddCondition(ImportDetailsMapping.Status, ConditionOperator.Equal, (int)ImportDetailsMapping.Status_OptionSet.Active);
                EntityCollection ecMappings = service.RetrieveMultiple(queryMappings);
                if (ecMappings != null && ecMappings.Entities.Count > 0)
                {
                    foreach (Entity objMapping in ecMappings.Entities)
                    {
                        mappings.Add(new Common.Mapping
                        {
                            //int x=(int)Enum.Parse(typeof(ImportDetailsMapping.SourceDataType_OptionSet),ImportDetailsMapping.SourceDataType_OptionSet.SingleLineOfText.ToString())
                            Source = objMapping.GetAttributeValue<string>(ImportDetailsMapping.SourceField),
                            Target = objMapping.GetAttributeValue<string>(ImportDetailsMapping.TargetField),
                            TargetEntity = objMapping.GetAttributeValue<string>(ImportDetailsMapping.PrimaryName),
                            DataType = objMapping.Contains(ImportDetailsMapping.TargetDataType) ? objMapping.GetAttributeValue<OptionSetValue>
                            (ImportDetailsMapping.TargetDataType).Value : (int)ImportDetailsMapping.TargetDataType_OptionSet.SingleLineOfText,
                            SourceDatatype = objMapping.Contains(ImportDetailsMapping.SourceDataType) ? objMapping.GetAttributeValue<OptionSetValue>
                            (ImportDetailsMapping.SourceDataType).Value : (int)ImportDetailsMapping.SourceDataType_OptionSet.SingleLineOfText,
                            LookupEntityAttribute = objMapping.GetAttributeValue<string>(ImportDetailsMapping.LookupentityAttributeFilterCondition),
                            LookupEntityName = objMapping.GetAttributeValue<string>(ImportDetailsMapping.LookupEntityName),
                            Mandatory = objMapping.Contains(ImportDetailsMapping.IsDataMandatoryforallrecords) ?
                            objMapping.GetAttributeValue<bool>(ImportDetailsMapping.IsDataMandatoryforallrecords) : false,
                            CrmDisplayName = objMapping.GetAttributeValue<string>(ImportDetailsMapping.CrmDisplayName),
                            MaxLengthAllowed = objMapping.Contains(ImportDetailsMapping.MaximumLengthAllowed) ?
                            objMapping.GetAttributeValue<int>(ImportDetailsMapping.MaximumLengthAllowed) : 0,
                            CreatelookupRecord = objMapping.Contains(ImportDetailsMapping.CreateLookupRecordUnresolved) ?
                            objMapping.GetAttributeValue<bool>(ImportDetailsMapping.CreateLookupRecordUnresolved) : false,
                            DataMaster = objMapping.GetAttributeValue<string>(ImportDetailsMapping.DataMasterName),
                            DefaultValue = objMapping.GetAttributeValue<string>(ImportDetailsMapping.DefaultValue),
                            RevertResult = objMapping.Contains(ImportDetailsMapping.RevertResult) ? objMapping.GetAttributeValue<bool>(ImportDetailsMapping.RevertResult) : false
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                UpdateValidationMessage("Error while fetching mappings: " + ex.Message, ref errorMessage);
                return false;
            }
            return true;
        }

        public void SetValueToTargetEntity(ref Entity entity, Common.Mapping mapping, IOrganizationService service, ref bool ValidationStatus, ref bool canReturn, ref string errorMessage, Dictionary<string, string> dcConfigDetails, Entity staging)
        {
            try
            {
                if (!string.IsNullOrEmpty(mapping.value))
                {
                    if (mapping.DataType == (int)ImportDetailsMapping.TargetDataType_OptionSet.SingleLineOfText ||
                        mapping.DataType == (int)ImportDetailsMapping.TargetDataType_OptionSet.MultipleLineOfText)
                    {
                        if (mapping.Source.Contains("phone") && mapping.value != null && (mapping.value.IndexOf("-") == -1 || (mapping.Target.Contains("unformatted"))))
                        {
                            string phoneNumber = string.Empty;
                            if (!(mapping.Target.Contains("unformatted")))
                            {
                                phoneNumber = PhoneNumberFormatting(mapping.value);
                                mapping.value = phoneNumber;
                            }
                            else
                            {
                                phoneNumber = PhoneNumberUnformatting(mapping.value);
                                mapping.value = phoneNumber;

                            }
                            entity[mapping.Target] = mapping.value;
                        }
                        else
                        {
                            entity[mapping.Target] = mapping.value;
                        }
                    }
                    else if (mapping.DataType == (int)ImportDetailsMapping.TargetDataType_OptionSet.WholeNumber)
                    {
                        //mapping.value = mapping.value.Replace('%', ' ').Trim();
                        int value = -1;
                        var values = Regex.Split(mapping.value, @"\D+").Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                        if (values.Count > 0)
                        {
                            int.TryParse(values[0], out value);
                        }
                        if (value != -1)
                            entity[mapping.Target] = Convert.ToInt32(value);
                    }
                    else if (mapping.DataType == (int)ImportDetailsMapping.TargetDataType_OptionSet.DateTime)
                    {
                        if (!mapping.value.Contains("/") && !mapping.value.Contains("-"))
                        {
                            entity[mapping.Target] = DateTime.FromOADate(Convert.ToDouble(mapping.value));
                        }
                        else if (mapping.value != "1/1/0001")
                        {
                            //Purchase year data coming as MM-YY. Making this as DD-MM-YY
                            if (mapping.TargetEntity == Lead.EntityName && mapping.Source == LeadStaging.PurchaseDate && mapping.Source.Length == 5)
                                mapping.value = "01-" + mapping.value;
                            DateTime dt;
                            if (DateTime.TryParse(mapping.value, out dt))
                            {
                                //dt = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                                entity[mapping.Target] = dt;
                            }
                        }
                    }
                    else if (mapping.DataType == (int)ImportDetailsMapping.TargetDataType_OptionSet.Decimal)
                    {
                        decimal d;
                        d = Convert.ToDecimal(mapping.value);
                        entity[mapping.Target] = d;
                    }

                    else if (mapping.DataType == (int)ImportDetailsMapping.TargetDataType_OptionSet.Optonset)
                    {
                        bool matchFound = false;
                        if (mapping.TargetEntity == Lead.EntityName && mapping.Target == Lead.PurchaseTimeframe)
                        {
                            //Map Purchase Time Frame fields based on LO Website Data
                            int value = -1;
                            value = GetPurchaseTimeFrameMapping(service, mapping.value);
                            if (value != -1)
                            {
                                entity[mapping.Target] = new OptionSetValue(value);
                                matchFound = true;
                            }
                        }
                        else
                        {
                            if (mapping.TargetEntity == Lead.EntityName && mapping.Target == Lead.Title)
                            {
                                string fetchxmlTitle = GetMessage(Constants.Lead_Title_PossibleValues, dcConfigDetails);
                                if (fetchxmlTitle != null)
                                {
                                    if (fetchxmlTitle.Contains("{appname}") && fetchxmlTitle.Contains("{title}"))
                                    {
                                        fetchxmlTitle = fetchxmlTitle.Replace("{appname}", Constants.AppConfigSetup);
                                        fetchxmlTitle = fetchxmlTitle.Replace("{title}", mapping.value);
                                        EntityCollection ec = service.RetrieveMultiple(new FetchExpression(string.Format(fetchxmlTitle)));
                                        if (ec.Entities.Count > 0)
                                        {
                                            Entity en = ec.Entities.FirstOrDefault();
                                            if (en.Contains(Configuration.Value))
                                            {
                                                mapping.value = en.GetAttributeValue<string>(Configuration.Value);
                                            }
                                        }
                                    }
                                }
                            }
                            else if (mapping.TargetEntity == Lead.EntityName && mapping.Target == Lead.MaritalStatus)
                            {

                                if(mapping.value!=null && !string.IsNullOrEmpty(mapping.value) && mapping.value.Equals("Unmarried",StringComparison.OrdinalIgnoreCase))
                                {
                                    string fetchXmlConfig = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
                                                              "<entity name='ims_configuration'>" +
                                                                "<attribute name='ims_configurationid' />" +
                                                                "<attribute name='ims_name' />" +
                                                                "<attribute name='ims_value' />" +
                                                                "<attribute name='createdon' />" +
                                                                "<order attribute='ims_name' descending='false' />" +
                                                                "<filter type='and'>" +
                                                                  "<condition attribute='ims_valuemultiline' operator='like' value='%" + mapping.value + "%' />" +
                                                                "</filter>" +
                                                                "<link-entity name='ims_appconfigsetup' from='ims_appconfigsetupid' to='ims_appconfigsetup' link-type='inner' alias='aa'>" +
                                                                  "<filter type='and'>" +
                                                                    "<condition attribute='ims_name' operator='eq' value='" + Constants.AppConfigSetup + "' />" +
                                                                  "</filter>" +
                                                                "</link-entity>" +
                                                              "</entity>" +
                                                            "</fetch>";
                                    EntityCollection ec = service.RetrieveMultiple(new FetchExpression(string.Format(fetchXmlConfig)));
                                    if (ec.Entities.Count > 0)
                                    {
                                        Entity en = ec.Entities.FirstOrDefault();
                                        if (en.Contains(Configuration.Value))
                                        {
                                            mapping.value = en.GetAttributeValue<string>(Configuration.Value);
                                        }
                                    }
                                }
                                //string fetchxmlTitle = GetMessage(Constants.Lead_MaritalStatus_Unmarried, dcConfigDetails);
                                //if (fetchxmlTitle != null)
                                //{
                                //    if (fetchxmlTitle.Contains("{appname}") && fetchxmlTitle.Contains("{title}"))
                                //    {
                                //        fetchxmlTitle = fetchxmlTitle.Replace("{appname}", Constants.AppConfigSetup);
                                //        fetchxmlTitle = fetchxmlTitle.Replace("{title}", mapping.value);
                                //        EntityCollection ec = service.RetrieveMultiple(new FetchExpression(string.Format(fetchxmlTitle)));
                                //        if (ec.Entities.Count > 0)
                                //        {
                                //            Entity en = ec.Entities.FirstOrDefault();
                                //            if (en.Contains(Configuration.Value))
                                //            {
                                //                mapping.value = en.GetAttributeValue<string>(Configuration.Value);
                                //            }
                                //        }
                                //    }
                                //}
                            }
                            Dictionary<string, string> dcOptionSet = Common.FetchOptionSetList(service, mapping.TargetEntity, mapping.Target);
                            KeyValuePair<string, string> optionSetVal = dcOptionSet.Where(x => x.Key.ToUpper() == mapping.value.ToUpper()).FirstOrDefault();
                            if (optionSetVal.Value != null)
                            {
                                entity[mapping.Target] = new OptionSetValue(Convert.ToInt32(optionSetVal.Value));
                                matchFound = true;
                            }
                        }
                        if (!matchFound)
                        {
                            entity[mapping.Target] = null;
                            //Add Log To UpdateValidationMessage();
                            UpdateValidationMessage(string.Format(GetMessage(Constants.ValidationError_OptionsetUnresolved, dcConfigDetails), mapping.TargetEntity, mapping.CrmDisplayName), ref errorMessage);
                        }
                    }
                    else if (mapping.DataType == (int)ImportDetailsMapping.TargetDataType_OptionSet.Lookup)
                    {
                        string sourceValue = mapping.value;

                        if (mapping.TargetEntity.Equals(Loan.EntityName) && mapping.value.Equals(Constants.LoanBorrower, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(mapping.DataMaster))
                        {
                            if ((staging.Contains(LoanStaging.BorrowerFirstName) || staging.Contains(LoanStaging.BorrowerLastName)) || staging.Contains(LoanStaging.BorrowerExternalID))
                            {
                                mapping.value = CreateBorrower(mapping, organizationServiceStagingOwner, ref ValidationStatus, ref canReturn, ref errorMessage, dcConfigDetails, staging, service);
                            }
                            else
                            {
                                //to overwite the master data name if it is failed in the above condition
                                mapping.value = string.Empty;
                            }
                        }
                        //Setting up for Co-Borrower in Loan 
                        else if (mapping.TargetEntity.Equals(Loan.EntityName) && staging.LogicalName == LoanStaging.EntityName && mapping.LookupEntityName.Equals(Account.EntityName) && mapping.LookupEntityAttribute.Equals(Account.ExternalID)
                            && mapping.Target.Equals(Loan.Co_Borrower))
                        {
                            if (!string.IsNullOrEmpty(mapping.value) && !string.IsNullOrEmpty(staging.GetAttributeValue<string>(LoanStaging.LOExternalID)))
                            {
                                Entity accountEntity = new Entity(Account.EntityName);
                                accountEntity[Account.LOExternalId] = staging.GetAttributeValue<string>(LoanStaging.LOExternalID);
                                accountEntity[Account.ExternalID] = WebUtility.HtmlEncode(mapping.value);
                                var accountGuid = new Common().CheckCoBorrower(organizationServiceStagingOwner, accountEntity, dcConfigDetails);
                                if (accountGuid != Guid.Empty)
                                {
                                    mapping.value = accountGuid.ToString();
                                }
                                else
                                {
                                    mapping.value = string.Empty;
                                }
                            }
                            else if (!string.IsNullOrEmpty(mapping.value))
                            {
                                mapping.value = string.Empty;
                            }
                        }
                        //Setting up for Buyer`s Agent, Seller`s Agent,etc
                        else if (mapping.TargetEntity.Equals(Loan.EntityName) && staging.LogicalName == LoanStaging.EntityName && mapping.LookupEntityName.Equals(Contact.EntityName) && mapping.LookupEntityAttribute.Equals(Contact.ExternalID)
                            && (mapping.Target.Equals(Loan.SellersAgent) || mapping.Target.Equals(Loan.BuyersAgent) || mapping.Target.Equals(Loan.SettlementAgent) || mapping.Target.Equals(Loan.Attorney)))
                        {
                            if (!string.IsNullOrEmpty(mapping.value) && !string.IsNullOrEmpty(staging.GetAttributeValue<string>(LoanStaging.LOExternalID)))
                            {
                                Entity contactEntity = new Entity(Contact.EntityName);
                                contactEntity[Contact.LOExternalId] = staging.GetAttributeValue<string>(LoanStaging.LOExternalID);
                                contactEntity[Contact.ExternalID] = WebUtility.HtmlEncode(mapping.value);
                                var contactGuid = new Common().CheckOtherContact(organizationServiceStagingOwner, contactEntity, dcConfigDetails);
                                if (contactGuid != Guid.Empty)
                                {
                                    mapping.value = contactGuid.ToString();
                                }
                                else
                                {
                                    mapping.value = string.Empty;
                                }
                            }
                            else if (!string.IsNullOrEmpty(mapping.value))
                            {
                                mapping.value = string.Empty;
                            }
                        }
                        else if (mapping.TargetEntity.Equals(Lead.EntityName) && mapping.value.Equals(Constants.LeadCoborrower, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(mapping.DataMaster))
                        {
                            if (staging.Contains(LeadStaging.SpouseFirstName) && staging.Contains(LeadStaging.SpouseLastName))
                            {
                                mapping.value = CreateCoborrower(mapping, organizationServiceStagingOwner, ref ValidationStatus, ref canReturn, ref errorMessage, dcConfigDetails, staging, service);
                            }
                            else
                            {
                                //to overwite the master data name if it is failed in the above condition
                                mapping.value = string.Empty;
                            }
                        }
                        else if ((mapping.value.Equals(Constants.Property) || mapping.value.Equals(Constants.LeadProperty)) && !string.IsNullOrEmpty(mapping.DataMaster))
                            mapping.value = CreateProperty(mapping, service, ref ValidationStatus, ref canReturn, ref errorMessage, dcConfigDetails, staging,mapping.DataMaster);
                        else if (mapping.TargetEntity.Equals(Lead.EntityName) && mapping.value.Equals(Constants.MovementInternalLeadProperty) && !string.IsNullOrEmpty(mapping.DataMaster))
                        {
                            mapping.value = CreateProperty(mapping, service, ref ValidationStatus, ref canReturn, ref errorMessage, dcConfigDetails, staging, mapping.DataMaster);
                        }
                        else if (mapping.LookupEntityName == PropertyType.EntityName)
                        {
                            mapping.value = GetPropertyType(mapping, service, ref errorMessage);
                        }
                        //Zillow Sellers Agent mapping
                        else if (mapping.value.Equals(Constants.ZillowSellersAgent, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(mapping.DataMaster))
                        {
                            //Create/update Zillow Sellers Agent as Contact Record
                            mapping.value = CreateZillowSellersAgent(mapping, ref ValidationStatus, ref canReturn, ref errorMessage, dcConfigDetails, staging, service);
                        }
                        else if (staging.LogicalName == LoanStaging.EntityName && mapping.TargetEntity.Equals(Lead.EntityName) && mapping.LookupEntityAttribute.Equals(Loan.Borrower))
                        {
                            mapping.value = Common.GetId(mapping.LookupEntityName, mapping.LookupEntityAttribute, mapping, organizationServiceStagingOwner, ref errorMessage);
                        }
                        else
                            mapping.value = Common.GetId(mapping.LookupEntityName, mapping.LookupEntityAttribute, mapping, service, ref errorMessage);
                        if (!string.IsNullOrEmpty(mapping.value) && new Guid(mapping.value) != Guid.Empty)
                        {
                            entity[mapping.Target] = new EntityReference(mapping.LookupEntityName, new Guid(mapping.value));
                        }
                        else
                        {
                            //Add Log To UpdateValidationMessage();
                            UpdateValidationMessage(string.Format(GetMessage(Constants.ValidationError_LookupUnresolved, dcConfigDetails), mapping.LookupEntityName, mapping.CrmDisplayName), ref errorMessage);
                        }
                    }
                    else if (mapping.DataType == (int)ImportDetailsMapping.TargetDataType_OptionSet.TwoOptions)
                    {
                        var configValues = GetMessage(Constants.Validate_Two_Options, dcConfigDetails);
                        bool revertResultCompareIsTrue = GetRevertResultCompareIsTrue(mapping.RevertResult);
                        bool revertResultCompareIsFalse = GetRevertResultCompareIsFalse(mapping.RevertResult);
                        if (configValues.Contains(";"))
                        {
                            var valueSplit = configValues.Split(';');
                            var isMappingValueFound = false;
                            foreach (var v in valueSplit)
                            {
                                if (CompareMappingValue(v, mapping.value))
                                {
                                    isMappingValueFound = true;
                                    entity[mapping.Target] = revertResultCompareIsTrue;
                                    break;
                                }
                            }
                            if (!isMappingValueFound)
                            {
                                entity[mapping.Target] = revertResultCompareIsFalse;
                            }
                        }
                        else
                        {
                            entity[mapping.Target] = CompareMappingValue(configValues, mapping.value) ? revertResultCompareIsTrue : revertResultCompareIsFalse;

                        }

                    }
                    else if (mapping.DataType == (int)ImportDetailsMapping.TargetDataType_OptionSet.Currency)
                    {
                        //Replace $ and , with Space
                        mapping.value = mapping.value.Replace('$', ' ').Replace(',', ' ');
                        //Remove Space from string
                        mapping.value = Regex.Replace(mapping.value, @"\s+", "");

                        if (mapping.TargetEntity == Lead.EntityName && mapping.Source == LeadStaging.DownPaymentAmount)
                        {
                            if (mapping.value.Contains("%"))
                            {
                                string downPaymentPercentage = Regex.Replace(mapping.value, "[^0-9]+", string.Empty);
                                string loanAmount = string.Empty;
                                bool calculateFromLoanAmount = false;
                                if (!string.IsNullOrEmpty(staging.GetAttributeValue<string>(LeadStaging.PurchasePrice)))
                                {
                                    loanAmount = staging.GetAttributeValue<string>(LeadStaging.PurchasePrice);
                                }
                                else if (!string.IsNullOrEmpty(staging.GetAttributeValue<string>(LeadStaging.LoanAmountMax)))
                                {
                                    calculateFromLoanAmount = true;
                                    loanAmount = staging.GetAttributeValue<string>(LeadStaging.LoanAmountMax);
                                }
                                if (!string.IsNullOrEmpty(loanAmount))
                                {
                                    if (!calculateFromLoanAmount)
                                        mapping.value = CalculateDownPaymentAmount(Convert.ToDecimal(downPaymentPercentage), Convert.ToDecimal(loanAmount)).ToString();
                                    else if (calculateFromLoanAmount)
                                        mapping.value = CalculateDownPaymentAmountFromLoanAmount(Convert.ToDecimal(downPaymentPercentage), Convert.ToDecimal(loanAmount)).ToString();
                                }
                            }
                        }

                        Money currencyField = new Money();
                        currencyField.Value = Convert.ToDecimal(mapping.value);
                        entity[mapping.Target] = currencyField;
                    }
                }
            }
            catch (Exception ex)
            {
                //validationStatus-> False canReturn->true  Add log UpdateValidationMessage
                UpdateValidationMessage(string.Format(GetMessage(Constants.ErrorInSettingValue, dcConfigDetails), mapping.Target) + ex.Message, ref errorMessage);
                ValidationStatus = false;
                //canReturn = true;
            }
        }

       

        public Entity FormTargetEntityObject(IOrganizationService ServiceStagingOwner, string targetEntity, Entity entityStaging, List<Common.Mapping> mappings, IOrganizationService service, ref bool ValidationStatus, ref bool canReturn, ref string errorMessage, Dictionary<string, string> dcConfigdetails)
        {
            organizationServiceStagingOwner = ServiceStagingOwner;
            Entity objTargetEntity = new Entity(targetEntity);

            foreach (Common.Mapping objMapping in mappings)
            {
                try
                {
                    Common.Mapping mapping = objMapping;
                    GetValueFromSourceEntity(entityStaging, ref mapping, ref ValidationStatus, ref canReturn, ref errorMessage);
                    if (!string.IsNullOrEmpty(mapping.value))
                    {
                        //Handling Annual Income
                        SetValueToTargetEntity(ref objTargetEntity, mapping, service, ref ValidationStatus, ref canReturn, ref errorMessage, dcConfigdetails, entityStaging);
                    }
                }
                catch (Exception ex)
                {
                    //validationStatus-> False canReturn->true  Add log UpdateValidationMessage
                    UpdateValidationMessage("Error While FormTargetEntityObject " + ex.Message, ref errorMessage);
                    ValidationStatus = false;
                    //canReturn = true;
                }
            }
            return objTargetEntity;
        }

        public Tuple<Entity, Entity> FormTargetEntity(string targetEntity, Entity entityStaging, List<Common.Mapping> mappings, IOrganizationService service, ref bool ValidationStatus, ref bool canReturn, ref string errorMessage, Dictionary<string, string> dcConfigdetails)
        {
            Entity objTargetEntity = new Entity(targetEntity);
            Entity objLeadEntity = new Entity(Lead.EntityName);

            foreach (Common.Mapping objMapping in mappings)
            {
                try
                {
                    Common.Mapping mapping = objMapping;
                    GetValueFromSourceEntity(entityStaging, ref mapping, ref ValidationStatus, ref canReturn, ref errorMessage);
                    if (!string.IsNullOrEmpty(mapping.value))
                    {
                        if (mapping.TargetEntity.Equals(Lead.EntityName))
                        {
                            //Form Lead Entity Object
                            // SetValueToTargetEntity(ref objLeadEntity, mapping, service, ref ValidationStatus, ref canReturn, ref errorMessage, dcConfigdetails, entityStaging);
                        }
                        else if (mapping.TargetEntity.Equals(Loan.EntityName))
                        {
                            //Form Loan Entity object
                            // SetValueToTargetEntity(ref objTargetEntity, mapping, service, ref ValidationStatus, ref canReturn, ref errorMessage, dcConfigdetails, entityStaging);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //validationStatus-> False canReturn->true  Add log UpdateValidationMessage
                    UpdateValidationMessage("Error While FormTargetEntityObject " + ex.Message, ref errorMessage);
                    ValidationStatus = false;
                    //canReturn = true;
                }
            }
            return Tuple.Create(objLeadEntity, objTargetEntity);
        }

        public void GetValueFromSourceEntity(Entity entityStaging, ref Common.Mapping mapping, ref bool ValidationStatus, ref bool canReturn, ref string errorMessage)
        {
            try
            {
                if (mapping.SourceDatatype == (int)ImportDetailsMapping.SourceDataType_OptionSet.SingleLineOfText ||
                        mapping.SourceDatatype == (int)ImportDetailsMapping.SourceDataType_OptionSet.MultipleLineOfText)
                {
                    if (!string.IsNullOrEmpty(mapping.DataMaster))
                        mapping.value = mapping.DataMaster;
                    else if (entityStaging.Attributes.Contains(mapping.Source) && entityStaging[mapping.Source] != null)
                        mapping.value = entityStaging.GetAttributeValue<string>(mapping.Source);
                    else
                        mapping.value = string.Empty;

                    //If Source is not having value and Default value is set in Import Details Mapping then Use that value
                    if (string.IsNullOrEmpty(mapping.value))
                    {
                        if (!string.IsNullOrEmpty(mapping.DefaultValue))
                        {
                            mapping.value = mapping.DefaultValue;
                        }
                    }

                    //Split Loan Amount & Credit Score to Min/Max Range
                    if (!string.IsNullOrEmpty(mapping.value))
                    {
                        //Split Loan Amount
                        if (mapping.TargetEntity == Lead.EntityName && (mapping.Source == LeadStaging.LoanAmountMax || mapping.Source == LeadStaging.LoanAmountMin))
                        {
                            Tuple<Decimal, Decimal> loanMinMaxRange = GetLoanAmountMinMaxRange(mapping.value);
                            decimal loanAmountMin = loanMinMaxRange.Item1;
                            decimal loanAmountMax = loanMinMaxRange.Item2;

                            if (mapping.Target == Lead.MinLoanAmountRequested)
                            {
                                mapping.value = loanAmountMin.ToString();
                            }
                            else if (mapping.Target == Lead.LoanAmount)
                            {
                                mapping.value = loanAmountMax.ToString();
                            }
                        }


                        //Split Credit Score
                        if (mapping.TargetEntity == Lead.EntityName && (mapping.Source == LeadStaging.LeadCreditScoreMax || mapping.Source == LeadStaging.LeadCreditScoreMin))
                        {
                            Tuple<int, int> CreditScoreMinMaxRange = GetCreditScoreMinMaxRange(mapping.value);
                            int creditScoreMin = CreditScoreMinMaxRange.Item1;
                            int creditScoreMax = CreditScoreMinMaxRange.Item2;

                            if (mapping.Target == Lead.MinCreditScore)
                            {
                                mapping.value = creditScoreMin.ToString();
                            }
                            else if (mapping.Target == Lead.CreditScore)
                            {
                                mapping.value = creditScoreMax.ToString();
                            }
                        }

                    }

                    //Trim Data if length Exceeded
                    if (!string.IsNullOrEmpty(mapping.value))
                    {
                        if (mapping.MaxLengthAllowed != 0)
                        {
                            if (mapping.value.Length > mapping.MaxLengthAllowed)
                            {
                                mapping.value = mapping.value.Substring(0, Math.Min(mapping.MaxLengthAllowed, mapping.value.Length));
                            }
                        }
                    }
                }
                else if (mapping.SourceDatatype == (int)ImportDetailsMapping.SourceDataType_OptionSet.WholeNumber)
                {
                    if (entityStaging.Attributes.Contains(mapping.Source) && entityStaging[mapping.Source] != null)
                        mapping.value = entityStaging.GetAttributeValue<int>(mapping.Source).ToString();
                    else
                        mapping.value = string.Empty;
                }
                else if (mapping.SourceDatatype == (int)ImportDetailsMapping.SourceDataType_OptionSet.DateTime)
                {
                    if (entityStaging.Attributes.Contains(mapping.Source) && entityStaging[mapping.Source] != null)
                        mapping.value = entityStaging.GetAttributeValue<DateTime>(mapping.Source).ToShortDateString();
                    else
                        mapping.value = string.Empty;
                }
                else
                {
                    mapping.value = string.Empty;
                }
            }
            catch (Exception ex)
            {
                //validationStatus-> False canReturn->true  Add log UpdateValidationMessage
                UpdateValidationMessage("Error While GetValueFromSourceEntity " + ex.Message, ref errorMessage);
                ValidationStatus = false;
                //canReturn = true;
            }
        }

       

        public void UpdateRecordIfDirty(Entity existingRec, Entity objNewRec, string targetEntity, List<Common.Mapping> mappings, IOrganizationService service, ref bool ValidationStatus, ref bool canReturn, ref string errorMessage, string importProcessName = null, EntityReference manualImport = null, bool oneTimeMigration = false)
        {
            int dirtyFields = 0;// flag to know the dirty fields count
            int updateFlag = 0;
            Entity recordTobBeUpdated = new Entity(targetEntity);
            recordTobBeUpdated.Id = existingRec.Id;
            foreach (Common.Mapping mapping in mappings)
            {
                if (mapping.TargetEntity == targetEntity)
                {
                    try
                    {
                        IsDirty(existingRec, objNewRec, ref recordTobBeUpdated, mapping.Target, ref updateFlag, ref dirtyFields, ref ValidationStatus, ref canReturn, ref errorMessage);
                        if (updateFlag != (int)Constants.NewRecord.ValueIsNull)
                        {
                            if (mapping.DataType == (int)ImportDetailsMapping.TargetDataType_OptionSet.SingleLineOfText ||
                                    mapping.DataType == (int)ImportDetailsMapping.TargetDataType_OptionSet.MultipleLineOfText)
                            {
                                if (updateFlag == (int)Constants.NewRecord.ValueIsNotNull || (updateFlag == (int)Constants.NewRecord.ValueIsDiffFromOldValue &&
                                    !existingRec.GetAttributeValue<string>(mapping.Target).Equals(objNewRec.GetAttributeValue<string>(mapping.Target))))
                                {
                                    recordTobBeUpdated[mapping.Target] = objNewRec.GetAttributeValue<string>(mapping.Target);
                                    dirtyFields++;
                                }
                            }
                            else if (mapping.DataType == (int)ImportDetailsMapping.TargetDataType_OptionSet.DateTime)
                            {
                                if (updateFlag == (int)Constants.NewRecord.ValueIsNotNull || (updateFlag == (int)Constants.NewRecord.ValueIsDiffFromOldValue &&
                                    !existingRec.GetAttributeValue<DateTime>(mapping.Target).ToShortDateString().Equals(objNewRec.GetAttributeValue<DateTime>(mapping.Target).ToShortDateString())))
                                {
                                    recordTobBeUpdated[mapping.Target] = objNewRec.GetAttributeValue<DateTime>(mapping.Target);
                                    dirtyFields++;
                                }
                            }
                            else if (mapping.DataType == (int)ImportDetailsMapping.TargetDataType_OptionSet.Decimal)
                            {
                                if (updateFlag == (int)Constants.NewRecord.ValueIsNotNull || (updateFlag == (int)Constants.NewRecord.ValueIsDiffFromOldValue &&
                                    !existingRec.GetAttributeValue<decimal>(mapping.Target).ToString().Equals(objNewRec.GetAttributeValue<decimal>(mapping.Target).ToString())))
                                {
                                    recordTobBeUpdated[mapping.Target] = objNewRec.GetAttributeValue<decimal>(mapping.Target);
                                    dirtyFields++;
                                }
                            }
                            else if (mapping.DataType == (int)ImportDetailsMapping.TargetDataType_OptionSet.WholeNumber)
                            {
                                if (updateFlag == (int)Constants.NewRecord.ValueIsNotNull || (updateFlag == (int)Constants.NewRecord.ValueIsDiffFromOldValue &&
                                    !existingRec.GetAttributeValue<int>(mapping.Target).ToString().Equals(objNewRec.GetAttributeValue<int>(mapping.Target).ToString())))
                                {
                                    recordTobBeUpdated[mapping.Target] = objNewRec.GetAttributeValue<int>(mapping.Target);
                                    dirtyFields++;
                                }
                            }
                            else if (mapping.DataType == (int)ImportDetailsMapping.TargetDataType_OptionSet.TwoOptions)
                            {
                                if (updateFlag == (int)Constants.NewRecord.ValueIsNotNull || (updateFlag == (int)Constants.NewRecord.ValueIsDiffFromOldValue &&
                                    !existingRec.GetAttributeValue<bool>(mapping.Target).ToString().Equals(objNewRec.GetAttributeValue<bool>(mapping.Target).ToString())))
                                {
                                    recordTobBeUpdated[mapping.Target] = objNewRec.GetAttributeValue<bool>(mapping.Target);
                                    dirtyFields++;
                                }
                            }
                            else if (mapping.DataType == (int)ImportDetailsMapping.TargetDataType_OptionSet.Currency)
                            {
                                if (updateFlag == (int)Constants.NewRecord.ValueIsNotNull || (updateFlag == (int)Constants.NewRecord.ValueIsDiffFromOldValue &&
                                    !existingRec.GetAttributeValue<Money>(mapping.Target).Value.ToString().Equals(objNewRec.GetAttributeValue<Money>(mapping.Target).Value.ToString())))
                                {
                                    recordTobBeUpdated[mapping.Target] = objNewRec.GetAttributeValue<Money>(mapping.Target);
                                    dirtyFields++;
                                }
                            }
                            else if (mapping.DataType == (int)ImportDetailsMapping.TargetDataType_OptionSet.Optonset)
                            {
                                if (updateFlag == (int)Constants.NewRecord.ValueIsNotNull || (updateFlag == (int)Constants.NewRecord.ValueIsDiffFromOldValue &&
                                    !existingRec.GetAttributeValue<OptionSetValue>(mapping.Target).Value.ToString().Equals(objNewRec.GetAttributeValue<OptionSetValue>(mapping.Target).Value.ToString())))
                                {
                                    recordTobBeUpdated[mapping.Target] = new OptionSetValue(objNewRec.GetAttributeValue<OptionSetValue>(mapping.Target).Value);
                                    dirtyFields++;
                                }
                            }
                            else if (mapping.DataType == (int)ImportDetailsMapping.TargetDataType_OptionSet.Lookup)
                            {
                                if (updateFlag == (int)Constants.NewRecord.ValueIsNotNull || (updateFlag == (int)Constants.NewRecord.ValueIsDiffFromOldValue &&
                                    !existingRec.GetAttributeValue<EntityReference>(mapping.Target).Id.ToString().Equals(objNewRec.GetAttributeValue<EntityReference>(mapping.Target).Id.ToString())))
                                {
                                    if (existingRec.Contains(Loan.Borrower) && ((mapping.Target.Equals(Loan.BuyersAgent) || mapping.Target.Equals(Loan.SellersAgent) || mapping.Target.Equals(Loan.SettlementAgent) || mapping.Target.Equals(Loan.Attorney))))
                                    {
                                        Disasscociate(service, existingRec.GetAttributeValue<EntityReference>(mapping.Target), existingRec.GetAttributeValue<EntityReference>(Loan.Borrower));
                                    }
                                    recordTobBeUpdated[mapping.Target] = new EntityReference(objNewRec.GetAttributeValue<EntityReference>(mapping.Target).LogicalName,
                                        objNewRec.GetAttributeValue<EntityReference>(mapping.Target).Id);
                                    dirtyFields++;
                                }
                            }
                        }
                        //Update Null value for Lead through Integration, Manual Import, Loan to Lead update, Customer etc.
                        else if (updateFlag == (int)Constants.NewRecord.ValueIsNull)
                        {
                            if (mapping.Target != Lead.Owner && !(mapping.Target.Contains("unformatted")) && !(mapping.Target.Contains("externalid"))
                                && string.IsNullOrEmpty(mapping.DataMaster))
                            {
                                if (!SkipTargetFieldUpdateWithNull(mapping))
                                {
                                    recordTobBeUpdated[mapping.Target] = null;
                                    dirtyFields++;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //validationStatus-> False canReturn->true  Add log UpdateValidationMessage
                        UpdateValidationMessage("Error While UpdateRecordIfDirty " + ex.Message, ref errorMessage);
                        ValidationStatus = false;
                        canReturn = true;
                    }
                }
            }
            if (dirtyFields > 0)
            {
                try
                {
                    //If Record imported from Manual Import and If It is not One Time Migration record then Set ims_tosync to True
                    if (!oneTimeMigration)
                    {
                        if (manualImport == null && importProcessName != null)
                        {
                            if (importProcessName.Equals(Constants.IDMEDWLead, StringComparison.OrdinalIgnoreCase) || importProcessName.Equals(Constants.OtherContact, StringComparison.OrdinalIgnoreCase))
                            {
                                if (targetEntity == Lead.EntityName || targetEntity == Contact.EntityName)
                                {
                                    recordTobBeUpdated.Attributes[Lead.ToSync] = false;
                                }
                            }
                        }
                        else if (manualImport != null && importProcessName != null)
                        {
                            if (importProcessName.Equals(Constants.IDMEDWLead, StringComparison.OrdinalIgnoreCase) || importProcessName.Equals(Constants.OtherContact, StringComparison.OrdinalIgnoreCase))
                            {
                                if (targetEntity == Lead.EntityName || targetEntity == Contact.EntityName)
                                {
                                    recordTobBeUpdated.Attributes[Lead.ToSync] = true;
                                }
                            }
                        }
                    }
                    else if (oneTimeMigration && importProcessName != null)
                    {
                        if (importProcessName.Equals(Constants.IDMEDWLead, StringComparison.OrdinalIgnoreCase) || importProcessName.Equals(Constants.OtherContact, StringComparison.OrdinalIgnoreCase))
                        {
                            if (targetEntity == Lead.EntityName || targetEntity == Contact.EntityName)
                            {
                                recordTobBeUpdated.Attributes[Lead.ToSync] = false;
                            }
                        }
                    }

                    if(targetEntity==Lead.EntityName &&  objNewRec.Contains(Lead.BorrowerType))
                    {
                        recordTobBeUpdated[Lead.BorrowerType]= new OptionSetValue(Convert.ToInt32(Lead.BorrowerType_OptionSet.PrimaryBorrower));
                    }
                    service.Update(recordTobBeUpdated);
                }
                catch (Exception ex)
                {
                    //validationStatus-> False canReturn->true  Add log UpdateValidationMessage
                    UpdateValidationMessage("Error While UPDATING " + targetEntity + " " + ex.Message, ref errorMessage);
                    ValidationStatus = false;
                    canReturn = true;
                }
            }
        }

        public bool SkipTargetFieldUpdateWithNull(Mapping mapping)
        {
            bool skip = false;
            if (mapping.TargetEntity == Lead.EntityName)
            {
                if (mapping.Target.Equals(Lead.LoanRate) || mapping.Target.Equals(Lead.LoanAmount) ||
                    mapping.Target.Equals(Lead.LoanProgram) || mapping.Target.Equals(Lead.LoanPurpose) ||
                    mapping.Target.Equals(Lead.Investor) || mapping.Target.Equals(Lead.Property) || mapping.Target.Equals(Lead.Occupancy) ||
                    mapping.Target.Equals(Lead.MinLoanAmountRequested) || mapping.Target.Equals(Lead.MaxLoanAmountRequested) || mapping.Target.Equals(Lead.LoanType)
                    || mapping.Target.Equals(Lead.PurchaseYear) || mapping.Target.Equals(Lead.PurchaseTimeframe) || mapping.Target.Equals(Lead.LoanStatus) ||
                    mapping.Target.Equals(Lead.LoanNumber) || mapping.Target.Equals(Lead.LeadSource) || mapping.Target.Equals(Lead.LeadStatus) || mapping.Target.Equals(Lead.ReferredBy) || mapping.Target.Equals(Lead.ApplicationCompletedDate) || mapping.Target.Equals(Lead.PreApproved)||
                    mapping.Target.Equals(Lead.DonotallowEmails) || mapping.Target.Equals(Lead.DonotallowPhoneCalls)||mapping.Target.Equals(Lead.DonotallowSMS)||mapping.Target.Equals(Lead.DonotallowMails))
                {
                    skip = true;
                }
            }
            else if(mapping.TargetEntity == Contact.EntityName)
            {
                if(mapping.Target.Equals(Contact.ContactSource)||mapping.Target.Equals(Contact.DonotallowEmails)||
                    mapping.Target.Equals(Contact.DonotallowPhoneCalls)|| mapping.Target.Equals(Contact.DonotallowMails))
                {
                    skip = true;
                }
            }
            else if (mapping.TargetEntity == Account.EntityName)
            {
                if (mapping.Target.Equals(Account.ContactSource))
                {
                    skip = true;
                }
            }
            return skip;
        }

        public void UpdateStagingLog(Guid stagingId, Guid targetEntityId, string staginEntityName, string validationMessage, bool validationStatus, IOrganizationService service, string entityName = "",bool contactGroupCo_Borrower=false)
        {
            try
            {
                Entity objStaging = new Entity(staginEntityName);
                objStaging.Id = stagingId;
                validationMessage = string.IsNullOrEmpty(validationMessage) ? string.Empty : validationMessage.Substring(0, Math.Min(2000, validationMessage.ToString().Length));
                if (!Guid.Empty.Equals(targetEntityId))
                {
                    if (staginEntityName == LeadStaging.EntityName)
                    {
                        if (entityName == Lead.EntityName)
                            objStaging[LeadStaging.Lead] = new EntityReference(Lead.EntityName, targetEntityId);
                        else if (entityName == Contact.EntityName)
                            objStaging[LeadStaging.OtherContact] = new EntityReference(Contact.EntityName, targetEntityId);
                        else if (entityName == Account.EntityName)
                            objStaging[LeadStaging.Coborrower] = new EntityReference(Account.EntityName, targetEntityId);
                    }
                    else if (staginEntityName == LoanStaging.EntityName)
                    {
                        objStaging[LoanStaging.CRMLoan] = new EntityReference(Loan.EntityName, targetEntityId);
                    }
                }
                objStaging[LeadStaging.ErrorLog] = validationMessage;
                objStaging[LeadStaging.ValidationStatus] = validationStatus;
                
                service.Update(objStaging);
            }
            catch (InvalidOperationException ex)
            {
                validationMessage += ex.Message;
                CreateLog(stagingId.ToString(), validationMessage, service);
            }
            catch (Exception e)
            {
                validationMessage += e.Message;
                CreateLog(stagingId.ToString(), validationMessage, service);
            }
        }

        public void UpdateValidationMessage(string message, ref string errorMessage)
        {
            if (!string.IsNullOrEmpty(message))
            {
                errorMessage = errorMessage + (errorMessage.Contains(message) ? string.Empty : (message + Environment.NewLine));
            }
        }

        public string GetMessage(string key, Dictionary<string, string> dcConfigDetails)
        {
            if (dcConfigDetails.ContainsKey(key))
            {
                return dcConfigDetails[key];
            }
            else
            {
                return "They Key '" + key + "' is not present in the configurations. Please check the configurations.";
            }
        }

        public void IsDirty(Entity existingRec, Entity objNewRec, ref Entity recordToBeUpdated, string targetField, ref int updateFlag, ref int dirtyFields, ref bool ValidationStatus, ref bool canReturn, ref string errorMessage)
        {
            try
            {
                updateFlag = 0;
                if ((existingRec.Contains(targetField) && !objNewRec.Contains(targetField)) ||
                    ((existingRec.Contains(targetField) || !existingRec.Contains(targetField)) && objNewRec.Contains(targetField) && objNewRec[targetField] == null))
                    updateFlag = (int)Constants.NewRecord.ValueIsNull;
                else if (!existingRec.Contains(targetField) && objNewRec.Contains(targetField))
                    updateFlag = (int)Constants.NewRecord.ValueIsNotNull;
                else if (existingRec.Contains(targetField) && objNewRec.Contains(targetField))
                    updateFlag = (int)Constants.NewRecord.ValueIsDiffFromOldValue;
            }
            catch (Exception ex)
            {
                UpdateValidationMessage("Error While checking if fields are changed " + ex.Message, ref errorMessage);
                ValidationStatus = false;
                //canReturn = true;
            }
        }

        public static Guid CreateEntityRecord(Common.Mapping mapping, IOrganizationService service, ref string errorMessage)
        {
            Guid entityid = Guid.Empty;
            try
            {
                Entity objEntity = new Entity(mapping.LookupEntityName);
                objEntity[mapping.LookupEntityAttribute] = mapping.value;
                entityid = service.Create(objEntity);
                Common objCommon = new Common();
                objCommon.UpdateValidationMessage("Master Data record not found in CRM System for Entity " + mapping.LookupEntityName + ".Creating New record with value " + mapping.value, ref errorMessage);
            }
            catch (Exception ex)
            {

                throw new Exception("Error while creating record for lookup Master Data: " + ex.Message);
            }
            return entityid;
        }

        public void CreateErrorLog(string externalId, string errorMessage, IOrganizationService service)
        {
            try
            {
                externalId = string.IsNullOrEmpty(externalId) ? string.Empty : externalId;
                Entity errorLog = new Entity(ErrorLog.EntityName);
                errorMessage = errorMessage.Substring(0, Math.Min(4900, errorMessage.Length));
                errorLog[ErrorLog.PrimaryName] = this.GetType().Name + " " + externalId;
                errorLog[ErrorLog.ErrorDetails] = errorMessage;
                service.Create(errorLog);
            }
            catch (Exception ex)
            {
                CreateLog(externalId, ex.Message, service);
            }
        }

        public void CreateLog(string externalId, string errorMessage, IOrganizationService service)
        {
            try
            {
                externalId = string.IsNullOrEmpty(externalId) ? string.Empty : externalId;
                Entity errorLog = new Entity(ErrorLog.EntityName);
                errorMessage = errorMessage.Substring(0, Math.Min(4900, errorMessage.Length));
                errorLog[ErrorLog.PrimaryName] = this.GetType().Name + " " + externalId;
                errorLog[ErrorLog.ErrorDetails] = errorMessage;
                service.Create(errorLog);
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidPluginExecutionException("Error while creating Error Log " + errorMessage + " " + e.Message);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("Error while creating Error Log " + errorMessage + " " + ex.Message);
            }
        }

        public string UpsertContactOROtherContact(Common.Mapping mapping, IOrganizationService service, ref bool ValidationStatus, ref bool canReturn, ref string errorMessage, Dictionary<string, string> dcConfigDetails, Entity staging)
        {
            Guid entityId = Guid.Empty;
            string importDataMaster = string.Empty;
            Guid importDataMasterId = Guid.Empty;
            List<Mapping> mappings = new List<Mapping>();
            Entity existingRecord = null;
            //Get Import Data Master to fetch mappings
            if (!string.IsNullOrEmpty(mapping.DataMaster)) importDataMaster = mapping.DataMaster;
            //Get Import Data Master GUID based on Name
            if (importDataMaster != string.Empty) importDataMasterId = GetImportDataMasterIdBasedOnName(importDataMaster, service);
            //return if import data Master ID is null
            if (importDataMasterId == Guid.Empty)
            {
                //UpdateStagingLog(staging.Id, Guid.Empty, staging.LogicalName, "Import Data Master Configuration is missing for Lookup field " + mapping.CrmDisplayName + " is missing.", service);
                return (entityId == Guid.Empty ? string.Empty : entityId.ToString());
            }
            //Fetch Mappings
            if (!FetchMappings(importDataMasterId, ref mappings, service, ref errorMessage))
            {
                //UpdateStagingLog(staging.Id, Guid.Empty, staging.LogicalName, errorMessage, service);
                return (entityId == Guid.Empty ? string.Empty : entityId.ToString());
            }
            Entity objContactOROtherContact = new Entity(mappings[0].TargetEntity);

            foreach (Common.Mapping objMapping in mappings)
            {
                try
                {
                    Common.Mapping mappingObject = objMapping;
                    GetValueFromSourceEntity(staging, ref mappingObject, ref ValidationStatus, ref canReturn, ref errorMessage);
                    if (!string.IsNullOrEmpty(mappingObject.value))
                    {
                        //SetValueToTargetEntity(ref objContactOROtherContact, mappingObject, service, ref ValidationStatus, ref canReturn, ref errorMessage, dcConfigDetails, staging);
                    }
                }
                catch (Exception ex)
                {
                    //validationStatus-> False canReturn->true  Add log UpdateValidationMessage
                    UpdateValidationMessage("Error While FormTargetEntityObject for Contact OR Other Contact " + ex.Message, ref errorMessage);
                }
            }

            //Mapping Of Contact Type lookup field
            //if (!mapping.DataMaster.Equals(Constants.CoBorrower))
            //    objContactOROtherContact[OtherContact.ContactType] = new EntityReference(ContactType.EntityName, GetContactTypeID(importDataMaster, service));

            //mapping of Is Cobborwer?
            if (mapping.DataMaster.Equals(Constants.CoBorrower)) objContactOROtherContact[Contact.IsCoBorrower] = true;

            //Run Duplicate Request to check for existing record
            var request = new RetrieveDuplicatesRequest
            {
                //Entity Object to be searched with the values filled for the attributes to check
                BusinessEntity = objContactOROtherContact,
                //Logical Name of the Entity to check Matching Entity
                MatchingEntityName = mappings[0].TargetEntity,
                PagingInfo = new PagingInfo() { PageNumber = 1, Count = 50 }
            };

            var response = (RetrieveDuplicatesResponse)service.Execute(request);

            if (response.DuplicateCollection.Entities.Count >= 1)
            {

                existingRecord = response.DuplicateCollection.Entities[0];
                UpdateRecordIfDirty(existingRecord, objContactOROtherContact, mappings[0].TargetEntity, mappings, service, ref ValidationStatus, ref canReturn, ref errorMessage);
                entityId = existingRecord.Id;
            }
            else
            {
                entityId = service.Create(objContactOROtherContact);
            }

            return (entityId == Guid.Empty ? string.Empty : entityId.ToString());
        }

        public Entity GetContactType(string name, IOrganizationService service)
        {
            Guid contactTypeId = Guid.Empty;
            try
            {
                var queryContactType = new QueryExpression(ContactType.EntityName);
                queryContactType.TopCount = 1;
                queryContactType.ColumnSet.AddColumns(ContactType.PrimaryName, ContactType.PrimaryKey, ContactType.CreateContact, ContactType.CreateCustomer, ContactType.CreateLead);
                queryContactType.Criteria.AddCondition(ContactType.PrimaryName, ConditionOperator.Equal, name);
                queryContactType.Criteria.AddCondition(ContactType.Status, ConditionOperator.Equal, (int)ContactType.Status_OptionSet.Active);
                EntityCollection ecContactType = service.RetrieveMultiple(queryContactType);
                if (ecContactType != null && ecContactType.Entities.Count > 0)
                {
                    return ecContactType.Entities.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public string CreateProperty(Common.Mapping mapping, IOrganizationService service, ref bool ValidationStatus, ref bool canReturn, ref string errorMessage, Dictionary<string, string> dcConfigDetails, Entity staging,string dataMasterName)
        {
            string propertyName = string.Empty;
            Guid entityId = Guid.Empty;
            string importDataMaster = string.Empty;
            Guid importDataMasterId = Guid.Empty;
            EntityReference defaultTeam = null;
           Entity entityProperty = null;
            List<Mapping> mappings = new List<Mapping>();
            //Get Import Data Master to fetch mappings
            if (!string.IsNullOrEmpty(mapping.DataMaster)) importDataMaster = mapping.DataMaster;
            //Get Import Data Master GUID based on Name
            if (importDataMaster != string.Empty) importDataMasterId = GetImportDataMasterAndDefaultTeam(importDataMaster, ref defaultTeam, service);
            //return if import data Master ID is null
            if (importDataMasterId == Guid.Empty)
            {
                UpdateStagingLog(staging.Id, Guid.Empty, staging.LogicalName, "Import Data Master Configuration is missing for Lookup field " + mapping.CrmDisplayName + " is missing.", true, service);
                return (entityId == Guid.Empty ? string.Empty : entityId.ToString());
            }
            //Fetch Mappings
            if (!FetchMappings(importDataMasterId, ref mappings, service, ref errorMessage))
            {
                UpdateStagingLog(staging.Id, Guid.Empty, staging.LogicalName, errorMessage, true, service);
                return (entityId == Guid.Empty ? string.Empty : entityId.ToString());
            }
            Entity objProperty = new Entity(mappings[0].TargetEntity);

            foreach (Common.Mapping objMapping in mappings)
            {
                try
                {
                    Common.Mapping mappingObject = objMapping;
                    GetValueFromSourceEntity(staging, ref mappingObject, ref ValidationStatus, ref canReturn, ref errorMessage);
                    if (!string.IsNullOrEmpty(mappingObject.value))
                    {
                        SetValueToTargetEntity(ref objProperty, mappingObject, service, ref ValidationStatus, ref canReturn, ref errorMessage, dcConfigDetails, staging);
                    }
                }
                catch (Exception ex)
                {
                    //validationStatus-> False canReturn->true  Add log UpdateValidationMessage
                    UpdateValidationMessage("Error While FormTargetEntityObject for Contact OR Other Contact " + ex.Message, ref errorMessage);
                }
            }

            //Check for existing property
            propertyName = BuildPropertyName(objProperty);
            if (!string.IsNullOrEmpty(propertyName))
                objProperty.Attributes[Property.PrimaryName] = propertyName;
            entityProperty = CheckPropertyExistance(service, dcConfigDetails, objProperty, dataMasterName);
            //UpdateValidationMessage(dataMasterName, ref errorMessage);
            if (entityProperty != null)
            {
                objProperty.Id = entityProperty.Id;
                service.Update(objProperty);
                entityId = entityProperty.Id;
            }
            else if (entityProperty == null)
            {
                if (objProperty.Attributes.Count > 0)
                {
                    if (defaultTeam != null)
                        objProperty[Property.Owner] = defaultTeam;
                    entityId = service.Create(objProperty);
                }

            }
            return (entityId == Guid.Empty ? string.Empty : entityId.ToString());
        }

        public Entity GetPreviousLead(Entity objEntity, IOrganizationService service)
        {
            Entity objExistingLead = null;
            string MobilePhone = string.Empty;
            string email = string.Empty;

            #region Query to find if any existing leads -To check existing, External Id OR First Name – Last Name OR email OR Phone No. In case of multiple records (rare but possible), external id will get precedence, followed by First Name,Last Name, email and then Phone no

            QueryExpression existingLeadQuery = new QueryExpression(Lead.EntityName);
            existingLeadQuery.ColumnSet = new ColumnSet(true);

            existingLeadQuery.Criteria.AddCondition(new ConditionExpression(Lead.Status, ConditionOperator.Equal, 0));
            FilterExpression finalFilter = new FilterExpression();
            finalFilter.FilterOperator = LogicalOperator.Or;


            if (objEntity.Attributes.Contains(Lead.ExternalID) && objEntity[Lead.ExternalID] != null)
            {

                ConditionExpression externalIdCondition = new ConditionExpression();
                externalIdCondition = new ConditionExpression(Lead.ExternalID, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Lead.ExternalID));
                finalFilter.Conditions.Add(externalIdCondition);
            }
            if ((objEntity.Attributes.Contains(Lead.FirstName) && objEntity[Lead.FirstName] != null) || (objEntity.Attributes.Contains(Lead.LastName) && objEntity[Lead.LastName] != null))
            {
                ConditionExpression firstNameCondition = new ConditionExpression();
                ConditionExpression lastNameCondition = new ConditionExpression();
                ConditionExpression Email = new ConditionExpression();
                if (objEntity[Lead.FirstName] != null)
                {
                    firstNameCondition = new ConditionExpression(Lead.FirstName, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Lead.FirstName));
                }
                if (objEntity[Lead.LastName] != null)
                {
                    lastNameCondition = new ConditionExpression(Lead.LastName, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Lead.LastName));
                }

                if (objEntity.Attributes.Contains(Lead.PersonalEmail) && objEntity[Lead.PersonalEmail] != null)
                {

                    Email = new ConditionExpression(Lead.PersonalEmail, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Lead.PersonalEmail));

                    //ConditionExpression emailCondition = new ConditionExpression();
                    //emailCondition = new ConditionExpression(Lead.PersonalEmail, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Lead.PersonalEmail));
                    //finalFilter.Conditions.Add(emailCondition);
                }
                if (objEntity[Lead.FirstName] != null && objEntity[Lead.LastName] != null && objEntity.Contains(Lead.PersonalEmail) && objEntity[Lead.PersonalEmail] != null)
                {
                    FilterExpression filterExpression = new FilterExpression();
                    filterExpression.FilterOperator = LogicalOperator.And;
                    filterExpression.Conditions.Add(firstNameCondition);
                    filterExpression.Conditions.Add(lastNameCondition);
                    filterExpression.Conditions.Add(Email);
                    finalFilter.AddFilter(filterExpression);
                }

            }

            if (objEntity.Attributes.Contains(Lead.MobilePhone) && objEntity[Lead.MobilePhone] != null)
            {
                //ConditionExpression phoneCondition = new ConditionExpression();
                //phoneCondition = new ConditionExpression(Lead.MobilePhone, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Lead.MobilePhone));
                //finalFilter.Conditions.Add(phoneCondition);
                ConditionExpression firstNameCondition = new ConditionExpression();
                ConditionExpression lastNameCondition = new ConditionExpression();
                ConditionExpression phone = new ConditionExpression();
                if (objEntity[Lead.FirstName] != null)
                {
                    firstNameCondition = new ConditionExpression(Lead.FirstName, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Lead.FirstName));
                }
                if (objEntity[Lead.LastName] != null)
                {
                    lastNameCondition = new ConditionExpression(Lead.LastName, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Lead.LastName));
                }

                if (objEntity.Attributes.Contains(Lead.MobilePhone) && objEntity[Lead.MobilePhone] != null)
                {

                    phone = new ConditionExpression(Lead.MobilePhone, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Lead.MobilePhone));

                    //ConditionExpression emailCondition = new ConditionExpression();
                    //emailCondition = new ConditionExpression(Lead.PersonalEmail, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Lead.PersonalEmail));
                    //finalFilter.Conditions.Add(emailCondition);
                }
                if (objEntity[Lead.FirstName] != null && objEntity[Lead.LastName] != null && objEntity.Contains(Lead.MobilePhone) && objEntity[Lead.MobilePhone] != null)
                {
                    FilterExpression filterExpression = new FilterExpression();
                    filterExpression.FilterOperator = LogicalOperator.And;
                    filterExpression.Conditions.Add(firstNameCondition);
                    filterExpression.Conditions.Add(lastNameCondition);
                    filterExpression.Conditions.Add(phone);
                    finalFilter.AddFilter(filterExpression);
                }

            }

            existingLeadQuery.Criteria.Filters.Add(finalFilter);

            EntityCollection result = service.RetrieveMultiple(existingLeadQuery);

            if (result.Entities.Count > 0)
            {
                objExistingLead = result.Entities[0];
            }
            #endregion
            return objExistingLead;
        }

        public Entity GetDuplicateRecord(Entity objEntity, string attributes, StringBuilder formattedCondition, string xml, IOrganizationService service, EntityReference defaultTeamReference = null)
        {
            if (xml.Contains("{entityName}"))
                xml = xml.Replace("{entityName}", objEntity.LogicalName);
            xml = xml.Replace("{condition}", GetConditionString(ref attributes, ref formattedCondition, objEntity, defaultTeamReference));
            EntityCollection ec = service.RetrieveMultiple(new FetchExpression(string.Format(@"" + xml + "")));
            if (ec.Entities.Count > 0)
            {
                return ec.Entities.FirstOrDefault();
            }
            return null;
        }
        public Entity CheckLeadExistiance(bool directCreateBorrower, bool isLoanOfficer, Entity objEntity, IOrganizationService service, Entity staging, Dictionary<string, string> dcConfigDetails,bool isMovementDirectLead=false,bool LOExternalIdIsMD=false,bool isMovementDirectTransfer=false)
        {
            Entity lead = new Entity();


            if (staging.Contains(LeadStaging.BlendApplicationSource) && staging.GetAttributeValue<string>(LeadStaging.BlendApplicationSource) == "MovehomeCRM")
            {
                lead = CheckBlendLeadExistance(staging, service);
                if (lead != null)
                    return lead;
                else if (lead == null)
                    return null;
            }
           
            string xml = GetMessage(Constants.DuplicateDetectionRule_Lead_Contact_Account, dcConfigDetails);
            if(objEntity.Contains(Lead.FromBlend) &&objEntity.GetAttributeValue<bool>(Lead.FromBlend)==true)
            {
                //This logic is to find out the duplicate record from Blend Integration
                StringBuilder formattedCondtion = new StringBuilder();
                string attributes = string.Empty;
                if (objEntity.Attributes.Contains(Lead.FirstName) && objEntity.Contains(Lead.LastName) && objEntity.Contains(Lead.PersonalEmail))
                {
                    attributes = "" + Lead.FromBlend + "," + Lead.PersonalEmail + "," + Lead.FirstName + "," + Lead.LastName;
                    lead = GetDuplicateRecord(objEntity, attributes, formattedCondtion, xml, service);
                    if (lead != null)
                        return lead;
                }

               if (objEntity.Attributes.Contains(Lead.FirstName) && objEntity.Contains(Lead.LastName) && objEntity.Contains(Lead.MobilePhone))
                {
                    formattedCondtion = new StringBuilder();
                    attributes = string.Empty;
                    attributes = "" + Lead.FromBlend + "," + Lead.MobilePhone + "," + Lead.FirstName + "," + Lead.LastName;
                    lead = GetDuplicateRecord(objEntity, attributes, formattedCondtion, xml, service);
                    if (lead != null)
                        return lead;
                }
               return null;
            }
        
            //This is to check when MD to RLO transfer happens,have to check the duplicate record with mergeexternal Id
            if(isMovementDirectLead && staging.Contains(LeadStaging.MergeContactExternalId) && isMovementDirectTransfer)
            {
                objEntity[Lead.ExternalID] = staging.GetAttributeValue<string>(LeadStaging.MergeContactExternalId);
            }
            
            //Stage1
            if (objEntity.Attributes.Contains(Lead.ExternalID))
            {
                StringBuilder formattedCondtion = new StringBuilder();
                string attributes = string.Empty;
                if (staging.LogicalName.Equals(LoanStaging.EntityName, StringComparison.OrdinalIgnoreCase) && objEntity.Contains(Lead.LOExternalId))
                    attributes = "" + Lead.ExternalID + "," + Lead.LOExternalId;
                else
                    attributes = "" + Lead.ExternalID + "";
                lead = GetDuplicateRecord(objEntity, attributes, formattedCondtion, xml, service);
                if (lead != null)
                {
                    //if it is a loan staging borrower mapping return the record
                    if (staging.LogicalName.Equals(LoanStaging.EntityName, StringComparison.OrdinalIgnoreCase))
                    {
                        return lead;
                    }
                    if (lead.Contains(Lead.LOExternalId) && objEntity.Contains(Lead.LOExternalId) && lead.GetAttributeValue<string>(Lead.LOExternalId).Equals(objEntity.GetAttributeValue<string>(Lead.LOExternalId)))
                    {
                        return lead;
                    }
                    else if (lead.Contains(Lead.LOExternalId) && objEntity.Contains(Lead.LOExternalId) && !staging.LogicalName.Equals(LoanStaging.EntityName, StringComparison.OrdinalIgnoreCase))
                    {
                        return lead;
                    }
                }
            }

            //Stage2
            if (objEntity.Contains(Lead.LOExternalId))
            {
                if (objEntity.Attributes.Contains(Lead.FirstName) && objEntity.Contains(Lead.LastName) && objEntity.Contains(Lead.PersonalEmail))
                {
                    StringBuilder formattedCondtion = new StringBuilder();
                    string attributes = "" + Lead.LOExternalId + "," + Lead.FirstName + "," + Lead.LastName + "," + Lead.PersonalEmail + "";
                    lead = GetDuplicateRecord(objEntity, attributes, formattedCondtion, xml, service);
                    if (lead != null)
                    {
                        if (objEntity.Contains(Lead.ExternalID) && !lead.Contains(Lead.ExternalID))
                        {
                            return lead;
                        }
                        //To make sure duplicate should not create from LO website we are adding this condition after Go-Lives
                        if (!objEntity.Contains(Lead.ExternalID) && !lead.Contains(Lead.ExternalID))
                        {
                            return lead;
                        }

                    }
                }
                if (objEntity.Attributes.Contains(Lead.FirstName) && objEntity.Contains(Lead.LastName) && objEntity.Contains(Lead.MobilePhone))
                {
                    StringBuilder formattedCondtion = new StringBuilder();
                    string attributes = "" + Lead.LOExternalId + "," + Lead.FirstName + "," + Lead.LastName + "," + Lead.MobilePhone + "";
                    lead = GetDuplicateRecord(objEntity, attributes, formattedCondtion, xml, service);
                    if (lead != null)
                    {

                        if (!lead.Contains(Lead.ExternalID) && objEntity.Contains(Lead.ExternalID))
                        {
                            return lead;
                        }
                        //To make sure duplicate should not create from LO website we are adding this condition after Go-Lives
                        if (!objEntity.Contains(Lead.ExternalID) && !lead.Contains(Lead.ExternalID))
                        {
                            return lead;
                        }
                    }
                }
            }

            //Stage3
            if (objEntity.Attributes.Contains(Lead.FirstName) && objEntity.Contains(Lead.LastName) && objEntity.Contains(Lead.PersonalEmail))
            {
                StringBuilder formattedCondtion = new StringBuilder();
                string attributes = string.Empty;
                if (!isLoanOfficer && defaultTeamReference != null && !isMovementDirectLead)
                    attributes = "" + Lead.FirstName + "," + Lead.LastName + "," + Lead.PersonalEmail + "," + Lead.Owner;
                else if (isMovementDirectLead && LOExternalIdIsMD && defaultTeamReference != null)//To find the duplicate records of MD, when pushes from LDW
                    attributes = "" + Lead.FirstName + "," + Lead.LastName + "," + Lead.PersonalEmail + "," + Lead.Owner;
                else if (isLoanOfficer || !directCreateBorrower || isMovementDirectLead)
                    attributes = "" + Lead.FirstName + "," + Lead.LastName + "," + Lead.PersonalEmail + "";
                lead = GetDuplicateRecord(objEntity, attributes, formattedCondtion, xml, service, defaultTeamReference);
                ////if it is a loan staging borrower mapping return the record
                //if (staging.LogicalName.Equals(LoanStaging.EntityName, StringComparison.OrdinalIgnoreCase))
                //{
                //    return lead;
                //}
                if (isMovementDirectLead && defaultTeamReference != null && lead != null)
                {
                    return lead;
                }
                if (!isLoanOfficer && defaultTeamReference != null && lead != null)
                {
                    //if (lead.GetAttributeValue<EntityReference>(Lead.Owner).Id.ToString().ToLower().Equals(defaultTeamReference.Id.ToString().ToLower()))
                    //{
                    return lead;
                    //}
                }
                if ((isLoanOfficer && lead != null) || (!directCreateBorrower && lead != null))
                {
                    return lead;
                }
                //if (!directCreateBorrower)
                //{
                //    return lead;
                //}
            }
            if (objEntity.Attributes.Contains(Lead.FirstName) && objEntity.Contains(Lead.LastName) && objEntity.Contains(Lead.MobilePhone))
            {
                StringBuilder formattedCondtion = new StringBuilder();
                string attributes = string.Empty;
                if (!isLoanOfficer && defaultTeamReference != null && !isMovementDirectLead)
                    attributes = "" + Lead.FirstName + "," + Lead.LastName + "," + Lead.MobilePhone + "," + Lead.Owner;
                else if (isMovementDirectLead && LOExternalIdIsMD && defaultTeamReference != null)//To find the duplicate records of MD, when pushes from LDW
                    attributes = "" + Lead.FirstName + "," + Lead.LastName + "," + Lead.MobilePhone + "," + Lead.Owner;
                else if (isLoanOfficer || !directCreateBorrower || isMovementDirectLead)
                    attributes = "" + Lead.FirstName + "," + Lead.LastName + "," + Lead.MobilePhone + "";
                lead = GetDuplicateRecord(objEntity, attributes, formattedCondtion, xml, service, defaultTeamReference);
                ////if it is a loan staging borrower mapping return the record
                //if (staging.LogicalName.Equals(LoanStaging.EntityName, StringComparison.OrdinalIgnoreCase))
                //{
                //    return lead;
                //}
                if(isMovementDirectLead && defaultTeamReference != null && lead != null)
                {
                    return lead;
                }
                if (!isLoanOfficer && defaultTeamReference != null && lead != null)
                {
                    //if (lead.GetAttributeValue<EntityReference>(Lead.Owner).Id.ToString().ToLower().Equals(defaultTeamReference.Id.ToString().ToLower()))
                    //{
                    return lead;
                    //}
                }
                if ((isLoanOfficer && lead != null) || (!directCreateBorrower && lead != null))
                {
                    return lead;
                }
                //if (!directCreateBorrower)
                //{
                //    return lead;
                //}
            }
            if (isMovementDirectLead && staging.Contains(LeadStaging.ContactExternalId) && isMovementDirectTransfer)
            {
                objEntity[Lead.ExternalID] = staging.GetAttributeValue<string>(LeadStaging.ContactExternalId);
            }
            return null;
        }
        public Entity CheckBlendLeadExistance(Entity leadStaging,IOrganizationService service)
        {
            if(leadStaging.Contains(LeadStaging.BlendApplicationGuid))
            {
                var blendApplicationGuid = leadStaging.GetAttributeValue<string>(LeadStaging.BlendApplicationGuid);
                QueryExpression queryExpression = new QueryExpression(XRMExtensions.Task.EntityName);
                queryExpression.ColumnSet = new ColumnSet(true);
                queryExpression.Criteria.AddCondition(new ConditionExpression(XRMExtensions.Task.BlendApplicationGuid, ConditionOperator.Equal, blendApplicationGuid));
                queryExpression.Criteria.AddCondition(new ConditionExpression(XRMExtensions.Task.Subject, ConditionOperator.Equal, "Sent app - successful"));
                EntityCollection enColleciton = service.RetrieveMultiple(queryExpression);
                if(enColleciton.Entities.Count>0)
                {
                    if(enColleciton.Entities.FirstOrDefault().Contains(XRMExtensions.Task.RegardingObjectId))
                    {
                        var leadGuid = enColleciton.Entities.FirstOrDefault().GetAttributeValue<EntityReference>(XRMExtensions.Task.RegardingObjectId).Id;
                        Entity leadEntity = service.Retrieve(Lead.EntityName, leadGuid, new ColumnSet(true));
                        return leadEntity;
                        //return enColleciton.Entities.FirstOrDefault().GetAttributeValue<EntityReference>(XRMExtensions.Task.RegardingObjectId);
                    }
                }
                return null;
            }
            return null;
        }
        public bool GetRecordCount(QueryExpression expression, IOrganizationService service, ref Entity lead)
        {
            EntityCollection ec = service.RetrieveMultiple(expression);
            if (ec.Entities.Count > 0)
            {
                lead = ec.Entities.FirstOrDefault();
                return true;
            }
            return false;
        }

        public Entity GetPreviousCoborrowerAccount(Entity objEntity, IOrganizationService service)
        {
            Entity objExistingContact = null;

            #region Query to find if any existing Coborrower Account -To check existing, External Id OR First Name – Last Name OR email OR Phone No. In case of multiple records (rare but possible), external id will get precedence, followed by First Name,Last Name, email and then Phone no

            QueryExpression existingAccountQuery = new QueryExpression(Account.EntityName);
            existingAccountQuery.ColumnSet = new ColumnSet(true);

            FilterExpression finalFilter = new FilterExpression();
            finalFilter.FilterOperator = LogicalOperator.Or;


            if (objEntity.Attributes.Contains(Account.ExternalID) && objEntity[Account.ExternalID] != null)
            {
                ConditionExpression externalIdCondition = new ConditionExpression();
                externalIdCondition = new ConditionExpression(Account.ExternalID, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Account.ExternalID));
                finalFilter.Conditions.Add(externalIdCondition);
            }
            if ((objEntity.Attributes.Contains(Account.FirstName) && objEntity[Account.FirstName] != null) || (objEntity.Attributes.Contains(Account.LastName) && objEntity[Account.LastName] != null))
            {
                ConditionExpression firstNameCondition = new ConditionExpression();
                ConditionExpression lastNameCondition = new ConditionExpression();
                ConditionExpression phone = new ConditionExpression();

                FilterExpression filterExpression = new FilterExpression();
                if (objEntity.Attributes.Contains(Account.FirstName) && objEntity[Account.FirstName] != null)
                {
                    firstNameCondition = new ConditionExpression(Account.FirstName, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Account.FirstName));
                    // filterExpression.Conditions.Add(firstNameCondition);
                }

                if (objEntity.Attributes.Contains(Account.MainPhone) && objEntity[Account.MainPhone] != null)
                {

                    phone = new ConditionExpression(Account.Mobilephone, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Account.Mobilephone));

                    //ConditionExpression emailCondition = new ConditionExpression();
                    //emailCondition = new ConditionExpression(Lead.PersonalEmail, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Lead.PersonalEmail));
                    //finalFilter.Conditions.Add(emailCondition);
                }


                if (objEntity.Attributes.Contains(Account.LastName) && objEntity[Account.LastName] != null)
                {
                    lastNameCondition = new ConditionExpression(Account.LastName, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Account.LastName));
                    //filterExpression.FilterOperator = LogicalOperator.And;
                    // filterExpression.Conditions.Add(lastNameCondition);
                    // finalFilter.AddFilter(filterExpression);
                }

                if (objEntity[Account.PrimaryName] != null && objEntity[Account.LastName] != null && objEntity.Contains(Account.Mobilephone) && objEntity[Account.Mobilephone] != null)
                {
                    FilterExpression filterExpressionfordata = new FilterExpression();
                    filterExpressionfordata.FilterOperator = LogicalOperator.And;
                    filterExpressionfordata.Conditions.Add(firstNameCondition);
                    filterExpressionfordata.Conditions.Add(lastNameCondition);
                    filterExpressionfordata.Conditions.Add(phone);
                    finalFilter.AddFilter(filterExpressionfordata);
                }

            }
            //if (objEntity.Attributes.Contains(Account.Email) && objEntity[Account.Email] != null)
            //{
            //    ConditionExpression emailCondition = new ConditionExpression();
            //    emailCondition = new ConditionExpression(Account.Email, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Account.Email));
            //    finalFilter.Conditions.Add(emailCondition);
            //}
            //if (objEntity.Attributes.Contains(Account.MainPhone) && objEntity[Account.MainPhone] != null)
            //{
            //    ConditionExpression phoneCondition = new ConditionExpression();
            //    phoneCondition = new ConditionExpression(Account.MainPhone, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Account.MainPhone));
            //    finalFilter.Conditions.Add(phoneCondition);
            //}

            if ((objEntity.Attributes.Contains(Account.FirstName) && objEntity[Account.FirstName] != null) || (objEntity.Attributes.Contains(Account.LastName) && objEntity[Account.LastName] != null))
            {
                ConditionExpression firstNameCondition = new ConditionExpression();
                ConditionExpression lastNameCondition = new ConditionExpression();
                ConditionExpression Email = new ConditionExpression();

                FilterExpression filterExpression = new FilterExpression();
                if (objEntity.Attributes.Contains(Account.FirstName) && objEntity[Account.FirstName] != null)
                {
                    firstNameCondition = new ConditionExpression(Account.FirstName, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Account.FirstName));
                    filterExpression.Conditions.Add(firstNameCondition);
                }

                if (objEntity.Attributes.Contains(Account.Email) && objEntity[Account.Email] != null)
                {

                    Email = new ConditionExpression(Account.Email, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Account.Email));

                    //ConditionExpression emailCondition = new ConditionExpression();
                    //emailCondition = new ConditionExpression(Lead.PersonalEmail, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Lead.PersonalEmail));
                    //finalFilter.Conditions.Add(emailCondition);
                }


                if (objEntity.Attributes.Contains(Account.LastName) && objEntity[Account.LastName] != null)
                {
                    lastNameCondition = new ConditionExpression(Account.LastName, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Account.LastName));
                    // filterExpression.FilterOperator = LogicalOperator.And;
                    // filterExpression.Conditions.Add(lastNameCondition);
                    //finalFilter.AddFilter(filterExpression);
                }

                if (objEntity[Account.PrimaryName] != null && objEntity[Account.LastName] != null && objEntity.Contains(Account.Email) && objEntity[Account.Email] != null)
                {
                    FilterExpression filterExpressionfordata = new FilterExpression();
                    filterExpressionfordata.FilterOperator = LogicalOperator.And;
                    filterExpressionfordata.Conditions.Add(firstNameCondition);
                    filterExpressionfordata.Conditions.Add(lastNameCondition);
                    filterExpressionfordata.Conditions.Add(Email);
                    finalFilter.AddFilter(filterExpressionfordata);
                }

            }

            existingAccountQuery.Criteria.Filters.Add(finalFilter);
            EntityCollection result = service.RetrieveMultiple(existingAccountQuery);

            if (result.Entities.Count > 0)
            {
                objExistingContact = result.Entities[0];
            }


            #endregion
            return objExistingContact;
        }

        public Entity CheckAccountExistance(bool directCreateCoborrower, bool isLoanOfficer, Entity staging, Entity objEntity, IOrganizationService service, Dictionary<string, string> dcConfigDetails, bool isMovementDirectLead = false, bool LOExternalIdIsMD = false, bool isMovementDirectTransfer = false)
        {
            Entity CoborrowerAccount = new Entity();
            string xml = GetMessage(Constants.DuplicateDetectionRule_Lead_Contact_Account, dcConfigDetails);
            //This is to check when MD to RLO transfer happens,have to check the duplicate record with mergeexternal Id
            if (isMovementDirectLead && staging.Contains(LeadStaging.MergeContactExternalId) && isMovementDirectTransfer)
            {
                objEntity[Lead.ExternalID] = staging.GetAttributeValue<string>(LeadStaging.MergeContactExternalId);
            }

            if (objEntity.Attributes.Contains(Account.ExternalID))
            {
                StringBuilder formattedCondtion = new StringBuilder();
                string attributes = string.Empty;
                if (staging.LogicalName.Equals(LoanStaging.EntityName, StringComparison.OrdinalIgnoreCase) && objEntity.Contains(Account.LOExternalId))
                    attributes = "" + Account.ExternalID + "," + Account.LOExternalId;
                else
                    attributes = "" + Account.ExternalID + "";
                CoborrowerAccount = GetDuplicateRecord(objEntity, attributes, formattedCondtion, xml, service);
                if (CoborrowerAccount != null)
                {
                    if (staging.LogicalName.Equals(LoanStaging.EntityName, StringComparison.OrdinalIgnoreCase))
                    {
                        return CoborrowerAccount;
                    }
                    if (CoborrowerAccount.Contains(Account.LOExternalId) && objEntity.Contains(Account.LOExternalId) && CoborrowerAccount.GetAttributeValue<string>(Account.LOExternalId).Equals(objEntity.GetAttributeValue<string>(Account.LOExternalId)))
                    {
                        return CoborrowerAccount;
                    }
                    else if (CoborrowerAccount.Contains(Account.LOExternalId) && objEntity.Contains(Account.LOExternalId))
                    {
                        return CoborrowerAccount;
                    }
                }
            }
            if (objEntity.Contains(Account.LOExternalId))
            {
                if (objEntity.Attributes.Contains(Account.FirstName) && objEntity.Contains(Account.LastName) && objEntity.Contains(Account.Email))
                {
                    StringBuilder formattedCondtion = new StringBuilder();
                    string attributes = "" + Account.LOExternalId + "," + Account.FirstName + "," + Account.LastName + "," + Account.Email + "";
                    CoborrowerAccount = GetDuplicateRecord(objEntity, attributes, formattedCondtion, xml, service);
                    if (CoborrowerAccount != null)
                    {
                        if (objEntity.Contains(Account.ExternalID) && !CoborrowerAccount.Contains(Account.ExternalID))
                        {
                            return CoborrowerAccount;
                        }
                        //if co-borower is trying from Co-borrower section
                        else if (LeadStaging.EntityName.Equals(staging.LogicalName) && !directCreateCoborrower)
                        {
                            return CoborrowerAccount;
                        }
                    }
                }
                if (objEntity.Attributes.Contains(Account.FirstName) && objEntity.Contains(Account.LastName) && objEntity.Contains(Account.MainPhone))
                {
                    StringBuilder formattedCondtion = new StringBuilder();
                    string attributes = "" + Account.LOExternalId + "," + Account.FirstName + "," + Account.LastName + "," + Account.Mobilephone + "";
                    CoborrowerAccount = GetDuplicateRecord(objEntity, attributes, formattedCondtion, xml, service);
                    if (CoborrowerAccount != null)
                    {
                        if (!CoborrowerAccount.Contains(Account.ExternalID) && objEntity.Contains(Account.ExternalID))
                        {
                            return CoborrowerAccount;
                        }
                        //if co-borower is trying from Co-borrower section
                        else if (LeadStaging.EntityName.Equals(staging.LogicalName) && !directCreateCoborrower)
                        {
                            return CoborrowerAccount;
                        }
                    }
                }
            }
            if (objEntity.Attributes.Contains(Account.FirstName) && objEntity.Contains(Account.LastName) && objEntity.Contains(Account.Email))
            {
                StringBuilder formattedCondtion = new StringBuilder();
                string attributes = string.Empty;
                if (!isLoanOfficer && defaultTeamReference != null && !isMovementDirectLead)
                    attributes = "" + Account.FirstName + "," + Account.LastName + "," + Account.Email + "," + Account.Owner;
                else if (isMovementDirectLead && LOExternalIdIsMD && defaultTeamReference != null)
                    attributes = "" + Account.FirstName + "," + Account.LastName + "," + Account.Email + "," + Account.Owner;
                else if (isLoanOfficer || !directCreateCoborrower || isMovementDirectLead)
                    attributes = "" + Account.FirstName + "," + Account.LastName + "," + Account.Email + "";
                CoborrowerAccount = GetDuplicateRecord(objEntity, attributes, formattedCondtion, xml, service, defaultTeamReference);
                if (isMovementDirectLead && defaultTeamReference != null && CoborrowerAccount != null)
                {
                    return CoborrowerAccount;
                }
                if (!isLoanOfficer && defaultTeamReference != null && CoborrowerAccount != null)
                {
                    return CoborrowerAccount;
                }
                if ((isLoanOfficer && CoborrowerAccount != null) || (!directCreateCoborrower && CoborrowerAccount != null))
                {
                    return CoborrowerAccount;
                }
            }
            if (objEntity.Attributes.Contains(Account.FirstName) && objEntity.Contains(Account.LastName) && objEntity.Contains(Account.MainPhone))
            {
                StringBuilder formattedCondtion = new StringBuilder();
                string attributes = string.Empty;
                if (!isLoanOfficer && defaultTeamReference != null && !isMovementDirectLead)
                    attributes = "" + Account.FirstName + "," + Account.LastName + "," + Account.Mobilephone + "," + Account.Owner;
                else if (isMovementDirectLead && LOExternalIdIsMD && defaultTeamReference != null)
                    attributes = "" + Account.FirstName + "," + Account.LastName + "," + Account.Mobilephone + "," + Account.Owner;
                else if (isLoanOfficer || !directCreateCoborrower || isMovementDirectLead)
                    attributes = "" + Account.FirstName + "," + Account.LastName + "," + Account.Mobilephone + "";
                CoborrowerAccount = GetDuplicateRecord(objEntity, attributes, formattedCondtion, xml, service, defaultTeamReference);
                if (isMovementDirectLead && defaultTeamReference != null && CoborrowerAccount != null)
                {
                    return CoborrowerAccount;
                }
                if (!isLoanOfficer && defaultTeamReference != null && CoborrowerAccount != null)
                {
                    return CoborrowerAccount;
                }
                if ((isLoanOfficer && CoborrowerAccount != null) || (!directCreateCoborrower && CoborrowerAccount != null))
                {

                    return CoborrowerAccount;
                }
            }
            if (isMovementDirectLead && staging.Contains(LeadStaging.ContactExternalId) && isMovementDirectTransfer)
            {
                objEntity[Lead.ExternalID] = staging.GetAttributeValue<string>(LeadStaging.ContactExternalId);
            }
            return null;
        }

        public Entity GetPreviousOtherContact(Entity objEntity, IOrganizationService service)
        {
            Entity objExistingOtherContact = null;

            #region Query to find if any existing Other Conatct -To check existing, External Id OR First Name – Last Name OR email OR Phone No. In case of multiple records (rare but possible), external id will get precedence, followed by First Name,Last Name, email and then Phone no

            ConditionExpression firstNameCondition = new ConditionExpression();
            ConditionExpression lastNameCondition = new ConditionExpression();
            FilterExpression filterExpression = new FilterExpression();
            ConditionExpression Email = new ConditionExpression();
            ConditionExpression phoneCondition = new ConditionExpression();
            QueryExpression existingContactQuery = new QueryExpression(Contact.EntityName);
            existingContactQuery.ColumnSet = new ColumnSet(true);

            FilterExpression finalFilter = new FilterExpression();
            finalFilter.FilterOperator = LogicalOperator.Or;


            if (objEntity.Attributes.Contains(Contact.ExternalID) && objEntity[Contact.ExternalID] != null)
            {
                ConditionExpression externalIdCondition = new ConditionExpression();
                externalIdCondition = new ConditionExpression(Contact.ExternalID, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Contact.ExternalID));
                finalFilter.Conditions.Add(externalIdCondition);
            }
            if ((objEntity.Attributes.Contains(Contact.FirstName) && objEntity[Contact.FirstName] != null) || (objEntity.Attributes.Contains(Contact.LastName) && objEntity[Contact.LastName] != null))
            {
                if (objEntity[Contact.FirstName] != null)
                {
                    firstNameCondition = new ConditionExpression(Contact.FirstName, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Contact.FirstName));
                    //filterExpression.Conditions.Add(firstNameCondition);
                }
            }
            if (objEntity.Attributes.Contains(Contact.LastName))
            {
                if (objEntity[Contact.LastName] != null)
                {
                    lastNameCondition = new ConditionExpression(Contact.LastName, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Contact.LastName));
                    //filterExpression.FilterOperator = LogicalOperator.And;
                    //filterExpression.Conditions.Add(lastNameCondition);
                    //finalFilter.AddFilter(filterExpression);
                }
            }

            if (objEntity.Attributes.Contains(Contact.PersonalEmail) && objEntity[Contact.PersonalEmail] != null)
            {
                //ConditionExpression emailCondition = new ConditionExpression();
                Email = new ConditionExpression(Contact.PersonalEmail, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Contact.PersonalEmail));
                //finalFilter.Conditions.Add(emailCondition);
            }
            if (objEntity[Contact.FirstName] != null && objEntity[Contact.LastName] != null && (objEntity.Attributes.Contains(Contact.PersonalEmail) && objEntity[Contact.PersonalEmail] != null))
            {
                FilterExpression filterExpressionfordata = new FilterExpression();
                filterExpressionfordata.FilterOperator = LogicalOperator.And;
                filterExpressionfordata.Conditions.Add(firstNameCondition);
                filterExpressionfordata.Conditions.Add(lastNameCondition);
                filterExpressionfordata.Conditions.Add(Email);
                finalFilter.AddFilter(filterExpressionfordata);
            }
            if (objEntity.Attributes.Contains(Contact.MobilePhone) && objEntity[Contact.MobilePhone] != null)
            {

                phoneCondition = new ConditionExpression(Contact.MobilePhone, ConditionOperator.Equal, objEntity.GetAttributeValue<string>(Contact.MobilePhone));
                //finalFilter.Conditions.Add(phoneCondition);

                if (objEntity[Contact.FirstName] != null && objEntity[Contact.LastName] != null && objEntity.Attributes.Contains(Contact.MobilePhone) && objEntity[Contact.MobilePhone] != null)
                {
                    FilterExpression filterExpressionfordata = new FilterExpression();
                    filterExpressionfordata.FilterOperator = LogicalOperator.And;
                    filterExpressionfordata.Conditions.Add(firstNameCondition);
                    filterExpressionfordata.Conditions.Add(lastNameCondition);
                    filterExpressionfordata.Conditions.Add(phoneCondition);
                    finalFilter.AddFilter(filterExpressionfordata);
                }
            }

            existingContactQuery.Criteria.Filters.Add(finalFilter);
            EntityCollection result = service.RetrieveMultiple(existingContactQuery);

            if (result.Entities.Count > 0)
            {
                objExistingOtherContact = result.Entities[0];
            }
            #endregion

            return objExistingOtherContact;
        }

        public Entity CheckPreviousOtherContact(bool isLoanOfficer, Entity staging, Entity objEntity, IOrganizationService service, Dictionary<string, string> dcConfigDetails,bool isMovementDirectLead = false, bool LOExternalIdIsMD = false, bool isMovementDirectTransfer = false)
        {
            Entity contact = new Entity();
            string xml = GetMessage(Constants.DuplicateDetectionRule_Lead_Contact_Account, dcConfigDetails);
            //This is to check when MD to RLO transfer happens,have to check the duplicate record with mergeexternal Id
            if (isMovementDirectLead && staging.Contains(LeadStaging.MergeContactExternalId) && isMovementDirectTransfer)
            {
                objEntity[Lead.ExternalID] = staging.GetAttributeValue<string>(LeadStaging.MergeContactExternalId);
            }
            if (objEntity.Attributes.Contains(Contact.ExternalID) && objEntity.Contains(Contact.LOExternalId))
            {
                StringBuilder formattedCondtion = new StringBuilder();
                string attributes = string.Empty;
                if (staging.LogicalName.Equals(LoanStaging.EntityName, StringComparison.OrdinalIgnoreCase) && objEntity.Contains(Contact.LOExternalId))
                    attributes = "" + Contact.ExternalID + "," + Contact.LOExternalId;
                else 
                    attributes = "" + Contact.ExternalID + "," + Contact.LOExternalId;
                contact = GetDuplicateRecord(objEntity, attributes, formattedCondtion, xml, service);
                if (contact != null)
                {
                    if (staging.LogicalName.Equals(LoanStaging.EntityName, StringComparison.OrdinalIgnoreCase))
                    {
                        return contact;
                    }
                    if (contact.Contains(Contact.LOExternalId) && objEntity.Contains(Contact.LOExternalId) && contact.GetAttributeValue<string>(Contact.LOExternalId).Equals(objEntity.GetAttributeValue<string>(Contact.LOExternalId)))
                    {
                        return contact;
                    }
                    //else if (contact.Contains(Contact.LOExternalId) && objEntity.Contains(Contact.LOExternalId))
                    //{
                    //    return contact;
                    //}
                }
            }
            if (objEntity.Contains(Contact.LOExternalId))
            {
                if (objEntity.Attributes.Contains(Contact.FirstName) && objEntity.Contains(Contact.LastName) && objEntity.Contains(Contact.PersonalEmail))
                {
                    StringBuilder formattedCondtion = new StringBuilder();
                    string attributes = "" + Contact.LOExternalId + "," + Contact.FirstName + "," + Contact.LastName + "," + Contact.PersonalEmail + "";
                    contact = GetDuplicateRecord(objEntity, attributes, formattedCondtion, xml, service);
                    if (contact != null)
                    {
                        if (!contact.Contains(Contact.ExternalID) && objEntity.Contains(Contact.ExternalID))
                        {
                            return contact;
                        }
                        //To make sure duplicate should not create from LO website we are adding this condition after Go-Live
                        if (!contact.Contains(Contact.ExternalID) && !objEntity.Contains(Contact.ExternalID))
                        {
                            return contact;
                        }
                    }
                }
                if (objEntity.Attributes.Contains(Contact.FirstName) && objEntity.Contains(Contact.LastName) && objEntity.Contains(Contact.MobilePhone))
                {
                    StringBuilder formattedCondtion = new StringBuilder();
                    string attributes = "" + Contact.LOExternalId + "," + Contact.FirstName + "," + Contact.LastName + "," + Contact.MobilePhone + "";
                    contact = GetDuplicateRecord(objEntity, attributes, formattedCondtion, xml, service);
                    if (contact != null)
                    {
                        if (!contact.Contains(Contact.ExternalID) && objEntity.Contains(Contact.ExternalID))
                        {
                            return contact;
                        }
                        //To make sure duplicate should not create from LO website we are adding this condition after Go-Live
                        if (!contact.Contains(Contact.ExternalID) && !objEntity.Contains(Contact.ExternalID))
                        {
                            return contact;
                        }
                    }
                }
            }
            if (objEntity.Attributes.Contains(Contact.FirstName) && objEntity.Contains(Contact.LastName) && objEntity.Contains(Contact.PersonalEmail))
            {
                StringBuilder formattedCondtion = new StringBuilder();
                string attributes = string.Empty;
                if (!isLoanOfficer && defaultTeamReference != null && !isMovementDirectLead)
                    attributes = "" + Contact.FirstName + "," + Contact.LastName + "," + Contact.PersonalEmail + "," + Contact.Owner;
                else if (isMovementDirectLead && LOExternalIdIsMD && defaultTeamReference != null)
                    attributes = "" + Contact.FirstName + "," + Contact.LastName + "," + Contact.PersonalEmail + "," + Contact.Owner;
                else if (isLoanOfficer || isMovementDirectLead)
                    attributes = "" + Contact.FirstName + "," + Contact.LastName + "," + Contact.PersonalEmail + "";
                contact = GetDuplicateRecord(objEntity, attributes, formattedCondtion, xml, service, defaultTeamReference);
                if (isMovementDirectLead && defaultTeamReference != null && contact != null)
                {
                    return contact;
                }
                if (!isLoanOfficer && defaultTeamReference != null && contact != null)
                {
                    return contact;
                }
                if (isLoanOfficer && contact != null)
                {

                    return contact;
                }
            }
            if (objEntity.Attributes.Contains(Contact.FirstName) && objEntity.Contains(Contact.LastName) && objEntity.Contains(Contact.MobilePhone))
            {
                StringBuilder formattedCondtion = new StringBuilder();
                string attributes = string.Empty;
                if (!isLoanOfficer && defaultTeamReference != null && !isMovementDirectLead)
                    attributes = "" + Contact.FirstName + "," + Contact.LastName + "," + Contact.MobilePhone + "," + Contact.Owner;
                else if (isMovementDirectLead && LOExternalIdIsMD && defaultTeamReference != null)
                    attributes = "" + Contact.FirstName + "," + Contact.LastName + "," + Contact.MobilePhone + "," + Contact.Owner;
                else if (isLoanOfficer || isMovementDirectLead)
                    attributes = "" + Contact.FirstName + "," + Contact.LastName + "," + Contact.MobilePhone + "";
                contact = GetDuplicateRecord(objEntity, attributes, formattedCondtion, xml, service, defaultTeamReference);
                if (isMovementDirectLead && defaultTeamReference != null && contact != null)
                {
                    return contact;
                }
                if (!isLoanOfficer && defaultTeamReference != null && contact != null)
                {
                    return contact;
                }
                if (isLoanOfficer && contact != null)
                {
                    return contact;
                }
            }
            if (isMovementDirectLead && staging.Contains(LeadStaging.ContactExternalId) && isMovementDirectTransfer)
            {
                objEntity[Lead.ExternalID] = staging.GetAttributeValue<string>(LeadStaging.ContactExternalId);
            }
            return null;
        }

        //To check the Other contact from Loan Staging with LO External and External Ids
        public Guid CheckOtherContact(IOrganizationService service,Entity objEntity,Dictionary<string,string> dcConfigDetails)
        {
            Entity contact = new Entity();
            string xml = GetMessage(Constants.DuplicateDetectionRule_Lead_Contact_Account, dcConfigDetails);

            if (objEntity.Attributes.Contains(Contact.ExternalID))
            {
                StringBuilder formattedCondtion = new StringBuilder();
                string attributes = "" + Contact.ExternalID + "," + Contact.LOExternalId;
                contact = GetDuplicateRecord(objEntity, attributes, formattedCondtion, xml, service);
                if (contact != null)
                {
                    return contact.Id;
                }
            }
            return Guid.Empty;
        }
        //To check the Other Customer from Loan Staging with LO External and External Ids
        public Guid CheckCoBorrower(IOrganizationService service, Entity objEntity, Dictionary<string, string> dcConfigDetails)
        {
            Entity account = new Entity();
            string xml = GetMessage(Constants.DuplicateDetectionRule_Lead_Contact_Account, dcConfigDetails);

            if (objEntity.Attributes.Contains(Account.ExternalID) && objEntity.Contains(Account.LOExternalId))
            {
                StringBuilder formattedCondtion = new StringBuilder();
                string attributes = "" + Account.ExternalID + "," + Account.LOExternalId;
                account = GetDuplicateRecord(objEntity, attributes, formattedCondtion, xml, service);
                if (account != null && account.Contains(Account.IsCoborrower))
                {
                    if (account.GetAttributeValue<bool>(Account.IsCoborrower))
                    {
                        return account.Id;
                    }
                    else if (!account.GetAttributeValue<bool>(Account.IsCoborrower))
                    {
                        Entity accountEntity = new Entity(Account.EntityName);
                        accountEntity.Id = account.Id;
                        accountEntity[Account.IsCoborrower] = true;
                        service.Update(accountEntity);
                    }
                }
            }
            return Guid.Empty;
        }
        public Entity GetPreviousProperty(Entity objProperty, IOrganizationService service)
        {
            Entity objExistingProperty = null;
            return objExistingProperty;
        }

        public bool CompareMappingValue(string configValue, string mappingValue)
        {
            if (mappingValue.Equals(configValue, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }
        public bool GetRevertResultCompareIsTrue(bool revertResult)
        {
            //if the revert result is true we will tranform to CRM value as reverted from source data
            if (revertResult == true)
                return false;
            else
                return true;
        }
        public bool GetRevertResultCompareIsFalse(bool revertResult)
        {
            //if the revert result is true we will tranform to CRM value as reverted from source data
            if (revertResult == true)
                return true;
            else
                return false;
        }

        public Entity CheckPropertyExistance(IOrganizationService service, Dictionary<string, string> dcConfigDetails, Entity propertyEntity,string dataMasterName)
        {
            string xml = GetMessage(Constants.PropertyDuplicateCheck, dcConfigDetails);

            if (BuildPropertyConditions(propertyEntity, ref xml,dataMasterName)!= string.Empty)
            {
                // FormatPropertyXml(ref xml, ref propertyEntity);
                EntityCollection entityCollectionProperty = service.RetrieveMultiple(new FetchExpression(string.Format(@"" + xml + "")));
                if (entityCollectionProperty.Entities.Count > 0)
                {
                    if (!dataMasterName.Equals(Constants.MovementInternalLeadProperty) || (dataMasterName.Equals(Constants.MovementInternalLeadProperty) && !entityCollectionProperty.Entities.FirstOrDefault().Contains(Property.AddressLine1) && !entityCollectionProperty.Entities.FirstOrDefault().Contains(Property.AddressLine2) && !entityCollectionProperty.Entities.FirstOrDefault().Contains(Property.PropertyType)))
                    {
                        return entityCollectionProperty.Entities.FirstOrDefault();
                    }
                }
            }
            return null;
        }

        public void FormatPropertyXml(ref string xml, ref Entity property)
        {

            if (property.Contains(Property.AddressLine1))
            {
                if (xml.Contains("{" + Property.AddressLine1 + "}"))
                {
                    AddConditionToXmlProperty(ref xml, Property.AddressLine1, WebUtility.HtmlEncode(property.GetAttributeValue<string>(Property.AddressLine1)));
                }
            }
            else
            {
                if (xml.Contains("{" + Property.AddressLine1 + "}"))
                {
                    RemoveCondtionsFromXmlProperty(ref xml, Property.AddressLine1);
                    property.Attributes.Add(Property.AddressLine1, string.Empty);
                }
            }
            if (property.Contains(Property.AddressLine2))
            {
                if (xml.Contains("{" + Property.AddressLine2 + "}"))
                {
                    AddConditionToXmlProperty(ref xml, Property.AddressLine2, WebUtility.HtmlEncode(property.GetAttributeValue<string>(Property.AddressLine2)));
                }
            }
            else
            {
                if (xml.Contains("{" + Property.AddressLine2 + "}"))
                {
                    RemoveCondtionsFromXmlProperty(ref xml, Property.AddressLine2);
                    property.Attributes.Add(Property.AddressLine2, string.Empty);
                }
            }
            if (property.Contains(Property.State))
            {
                if (xml.Contains("{" + Property.State + "}"))
                {
                    AddConditionToXmlProperty(ref xml, Property.State, property.GetAttributeValue<string>(Property.State));
                }
            }
            else
            {
                if (xml.Contains("{" + Property.State + "}"))
                {
                    RemoveCondtionsFromXmlProperty(ref xml, Property.State);
                    property.Attributes.Add(Property.State, string.Empty);
                }
            }
            if (property.Contains(Property.City))
            {
                if (xml.Contains("{" + Property.City + "}"))
                {
                    AddConditionToXmlProperty(ref xml, Property.City, property.GetAttributeValue<string>(Property.City));

                }
            }
            else
            {
                if (xml.Contains("{" + Property.City + "}"))
                {
                    RemoveCondtionsFromXmlProperty(ref xml, Property.City);
                    property.Attributes.Add(Property.City, string.Empty);
                }
            }
            if (property.Contains(Property.Zip))
            {
                if (xml.Contains("{" + Property.Zip + "}"))
                {
                    AddConditionToXmlProperty(ref xml, Property.Zip, property.GetAttributeValue<string>(Property.Zip));

                }
            }
            else
            {
                if (xml.Contains("{" + Property.Zip + "}"))
                {
                    RemoveCondtionsFromXmlProperty(ref xml, Property.Zip);
                    property.Attributes.Add(Property.Zip, string.Empty);
                }
            }
            if (property.Contains(Property.PropertyType))
            {
                if (xml.Contains("{" + Property.PropertyType + "}"))
                {
                    AddConditionToXmlProperty(ref xml, Property.PropertyType, property.GetAttributeValue<EntityReference>(Property.PropertyType).Id.ToString());
                }
            }
            else
            {
                if (xml.Contains("{" + Property.PropertyType + "}"))
                {
                    RemoveCondtionsFromXmlProperty(ref xml, Property.PropertyType);
                    property.Attributes.Add(Property.PropertyType, null);
                }
            }

        }

        public void RemoveCondtionsFromXmlProperty(ref string xml, string key)
        {
            xml = xml.Replace("{" + key + "}", " ");
            //if (key == Property.PropertyType)
            //    xml = xml.Replace("{" + key + "}", "<condition attribute='" + key + "' uitype='ims_propertytype' uiname='' operator='null' />");
            //else
            //    xml = xml.Replace("{" + key + "}", "<condition attribute='" + key + "' operator='null' />");
        }
        public void AddConditionToXmlProperty(ref string xml, string key, string value)
        {
            if (key == Property.PropertyType)
                xml = xml.Replace("{" + key + "}", "<condition attribute='" + key + "' operator='eq' uiname='' uitype='ims_propertytype' value='" + value + "' />");
            else
                xml = xml.Replace("{" + key + "}", "<condition attribute='" + key + "' operator='eq' value='" + value + "' />");
        }
        public string BuildPropertyName(Entity entity)
        {
            string propertyName = string.Empty;
            string address1 = string.Empty;
            string address2 = string.Empty;
            string state = string.Empty;
            string city = string.Empty;
            string zip = string.Empty;
            if (entity.Contains(Property.AddressLine1))
                address1 = entity.GetAttributeValue<string>(Property.AddressLine1);
            if (entity.Contains(Property.AddressLine2))
                address2 = entity.GetAttributeValue<string>(Property.AddressLine2);
            if (entity.Contains(Property.State))
                state = entity.GetAttributeValue<string>(Property.State);
            if (entity.Contains(Property.City))
                city = entity.GetAttributeValue<string>(Property.City);
            if (entity.Contains(Property.Zip))
                zip = entity.GetAttributeValue<string>(Property.Zip);
            if (!string.IsNullOrEmpty(address1) && !string.IsNullOrEmpty(address2))
            {
                propertyName = String.Concat(address1, " ", address2);
            }
            else if (!string.IsNullOrEmpty(state) && !string.IsNullOrEmpty(city))
            {
                propertyName = String.Concat(state, " ", city);
            }
            else if (!string.IsNullOrEmpty(state) && !string.IsNullOrEmpty(zip))
            {
                propertyName = String.Concat(state, " ", zip);
            }
            else if (!string.IsNullOrEmpty(city) && !string.IsNullOrEmpty(zip))
            {
                propertyName = String.Concat(city, " ", zip);
            }
            else if (!string.IsNullOrEmpty(address1) && !string.IsNullOrEmpty(city))
            {
                propertyName = String.Concat(address1, " ", city);
            }
            else if (!string.IsNullOrEmpty(address2) && !string.IsNullOrEmpty(city))
            {
                propertyName = String.Concat(address2, " ", city);
            }
            return propertyName;
        }

        public void MarketingListAssociation(Dictionary<string, string> dcConfigValues, string contactGroup, Entity targetEntity, IOrganizationService service)
        {
            Guid marketingListGuid = Guid.Empty;
            Entity marketingList = CheckmarketingList(contactGroup, service, ref marketingListGuid);
            if (marketingList != null)
            {
                if (marketingList.Contains(Marketinglist.Type) && (bool)marketingList[Marketinglist.Type] == false)
                {
                    if (!CheckExistingMarketListContactGroupAssoc(service, dcConfigValues, targetEntity, contactGroup))
                    {
                        AddMemberToList(marketingListGuid, targetEntity.Id, service);
                    }
                }
            }
            else
            {
                CreateMarketingList(contactGroup, service, ref marketingListGuid, targetEntity);
                AddMemberToList(marketingListGuid, targetEntity.Id, service);
            }
        }
        public Entity CheckmarketingList(string contactGroup, IOrganizationService service, ref Guid marketingListGuid)
        {
            var queryML = new QueryExpression(Marketinglist.EntityName);
            queryML.TopCount = 1;
            queryML.ColumnSet.AddColumns(Marketinglist.PrimaryKey, Marketinglist.Type);
            queryML.Criteria.AddCondition(Marketinglist.PrimaryName, ConditionOperator.Equal, contactGroup);
            EntityCollection ecIDM = service.RetrieveMultiple(queryML);
            if (ecIDM != null && ecIDM.Entities.Count > 0)
            {
                marketingListGuid = ecIDM.Entities[0].Id;
                return ecIDM.Entities[0];
            }
            return null;
        }

        public void CreateMarketingList(string contactGroupName, IOrganizationService service, ref Guid marketingListGuid, Entity targetEntity)
        {
            int memberType = 0;
            AttributeCollection attrCollection = new AttributeCollection();
            attrCollection.Add(Marketinglist.Type, false);
            attrCollection.Add(Marketinglist.MarketingListMemberType, new OptionSetValue(GetMarketingListMemberType(targetEntity, ref memberType)));
            attrCollection.Add(Marketinglist.PrimaryName, contactGroupName);

            marketingListGuid = service.Create(new Entity()
            {
                LogicalName = Marketinglist.EntityName,
                Attributes = attrCollection
            });
        }
        public bool CheckExistingMarketListContactGroupAssoc(IOrganizationService service, Dictionary<string, string> dcConfigDetails, Entity targetEntity, string contactGroup)
        {
            var fetchXml = GetMessage(Constants.MarketingListContactGroupAsscoCheck, dcConfigDetails);
            if (fetchXml.Contains("{targetEntity}"))
            {
                fetchXml = fetchXml.Replace("{targetEntity}", targetEntity.LogicalName);
            }
            if (fetchXml.Contains("{targetEntityId}"))
            {
                fetchXml = fetchXml.Replace("{targetEntityId}", targetEntity.LogicalName + "id");
            }
            if (fetchXml.Contains("{targetEntityGuid}"))
            {
                fetchXml = fetchXml.Replace("{targetEntityGuid}", targetEntity.Id.ToString());
            }
            if (fetchXml.Contains("{listname}"))
            {
                fetchXml = fetchXml.Replace("{listname}",WebUtility.HtmlDecode(contactGroup));
            }
            EntityCollection entityCollection = service.RetrieveMultiple(new FetchExpression(string.Format(@"" + fetchXml + "")));
            if (entityCollection.Entities.Count > 0)
            {
                return true;
            }
            return false;
        }
        public Guid GetmarketingList(string contactGroupName, int marketingListMemberType, IOrganizationService service)
        {
            Guid marketingListId = Guid.Empty;
            //Check existing Marketing List
            var queryML = new QueryExpression(Marketinglist.EntityName);
            queryML.TopCount = 1;
            queryML.ColumnSet.AddColumns(Marketinglist.PrimaryKey);
            queryML.Criteria.AddCondition(Marketinglist.PrimaryName, ConditionOperator.Equal, contactGroupName);
            queryML.Criteria.AddCondition(Marketinglist.MarketingListMemberType, ConditionOperator.Equal, marketingListMemberType);
            queryML.Criteria.AddCondition(Marketinglist.Type, ConditionOperator.Equal, Marketinglist.Type_OptionSet.Static);
            queryML.Criteria.AddCondition(Marketinglist.Status, ConditionOperator.Equal, (int)Marketinglist.Status_OptionSet.Active);
            EntityCollection ecIDM = service.RetrieveMultiple(queryML);
            if (ecIDM != null && ecIDM.Entities.Count > 0)
            {
                marketingListId = ecIDM.Entities[0].Id;
            }

            //Create New Marketing List If not exist in System
            if (marketingListId == Guid.Empty)
            {
                // Create the marketing list 
                Entity CurrentList = new Entity(Marketinglist.EntityName);
                CurrentList[Marketinglist.Type] = false;
                CurrentList[Marketinglist.PrimaryName] = contactGroupName;
                CurrentList[Marketinglist.MarketingListMemberType] = new OptionSetValue(marketingListMemberType);
                // Actually create the list 
                marketingListId = service.Create(CurrentList);
            }

            return marketingListId;
        }
        public void AddMemberToList(Guid marketingListId, Guid memberId, IOrganizationService service)
        {
            try
            {
                // Use the AddListMembersListRequest to add the members to the list 
                List<Guid> MemberListIds = new List<Guid>();
                // Now you'll need to add the Guids for each member to the list  
                // I'm leaving that part out as adding values to a list is very basic. 
                AddMemberListRequest AddMemberRequest = new AddMemberListRequest();
                AddMemberRequest.ListId = marketingListId;
                AddMemberRequest.EntityId = memberId;
                // Use AddListMembersListReponse to get information about the request execution 
                AddMemberListResponse AddMemberResponse = service.Execute(AddMemberRequest) as AddMemberListResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int GetMarketingListMemberType(Entity targetEntity, ref int memberType)
        {
            if (targetEntity.LogicalName.Equals(Contact.EntityName))
                memberType = (int)Marketinglist.MarketingListMemberType_OptionSet.Contact;
            else if (targetEntity.LogicalName.Equals(Lead.EntityName))
                memberType = (int)Marketinglist.MarketingListMemberType_OptionSet.Lead;
            else if (targetEntity.LogicalName.Equals(Account.EntityName))
                memberType = (int)Marketinglist.MarketingListMemberType_OptionSet.Account;

            return memberType;
        }

        public EntityCollection CheckExistingContactlist(IOrganizationService context, Dictionary<string, string> dcConfigDetails, Entity targetEntity)
        {
            var fetchXml = GetMessage(Constants.AppConfigCustomerDuplicatecheck, dcConfigDetails);

            if (targetEntity.Attributes.Contains(Account.FirstName))
            {
                if (fetchXml.Contains("{tagetentityfirstname}"))
                    fetchXml = fetchXml.Replace("{tagetentityfirstname}", targetEntity.GetAttributeValue<string>(Account.FirstName));
            }
            else
            {
                Replacecondtion(ref fetchXml, "tagetentityfirstname", Account.FirstName);
            }

            if (targetEntity.Attributes.Contains(Account.LastName))

            {
                if (fetchXml.Contains("{tagetentitylastname}"))
                    fetchXml = fetchXml.Replace("{tagetentitylastname}", targetEntity.GetAttributeValue<string>(Account.LastName));
            }
            else
            {
                Replacecondtion(ref fetchXml, "tagetentitylastname", Account.LastName);
            }
            if (targetEntity.Attributes.Contains(Account.Email))

            {
                if (fetchXml.Contains("{tagetentityemail}"))
                    fetchXml = fetchXml.Replace("{tagetentityemail}", targetEntity.GetAttributeValue<string>(Account.Email));
            }
            else
            {
                Replacecondtion(ref fetchXml, "tagetentityemail", Account.Email);
            }
            if (targetEntity.Attributes.Contains(Account.Mobilephone))

            {
                if (fetchXml.Contains("{tagetentityphonenumber}"))
                    fetchXml = fetchXml = fetchXml.Replace("{tagetentityphonenumber}", targetEntity.GetAttributeValue<string>(Account.Mobilephone));
            }
            else
            {
                Replacecondtion(ref fetchXml, "tagetentityphonenumber", Account.Mobilephone);
            }
            EntityCollection entityCollection = context.RetrieveMultiple(new FetchExpression(string.Format(@"" + fetchXml + "")));
            if (entityCollection.Entities.Count > 0)
            {
                return entityCollection;
            }
            return null;
        }

        public void Replacecondtion(ref string fetchXml, string key, string attribute)
        {
            //var fetchdata = "<condition attribute='{" + attribute + "}'value='{" + key + "}' operator='eq'/>";
            fetchXml = fetchXml.Replace("value='{" + key + "}' operator='eq'/>", "operator='null'/>");
        }

        public string checkerrorMessageforCoBrrower(IExecutionContext context, Dictionary<string, string> erorMessage)
        {
            var fetchXml = GetMessage(Constants.errromessageforCoBrrower, erorMessage);

            if (fetchXml != null)
            {
                return fetchXml;
            }
            return null;
        }
        public string checkerrorMessageforOtherContact(IExecutionContext context, Dictionary<string, string> erorMessage)
        {
            var fetchXml = GetMessage(Constants.errromessageforOtherContact, erorMessage);

            if (fetchXml != null)
            {
                return fetchXml;
            }
            return null;
        }

        public string Getid(IExecutionContext context, Dictionary<string, string> getTeamName)
        {
            var fetchXml = GetMessage(Constants.GetTeamId, getTeamName);

            if (fetchXml != null)
            {
                return fetchXml;
            }
            return null;
        }



        public int GetPurchaseTimeFrameMapping(IOrganizationService service, string purchaseTimeFrameName)
        {
            int value = -1;

            //Check existing Purchase TimeFrame Mappings
            var queryPTFM = new QueryExpression(PurchaseTimeframeMapping.EntityName);
            queryPTFM.TopCount = 1;
            queryPTFM.ColumnSet.AddColumns(PurchaseTimeframeMapping.LeadPurchaseTimeFrame);
            queryPTFM.Criteria.AddCondition(PurchaseTimeframeMapping.PrimaryName, ConditionOperator.Equal, purchaseTimeFrameName);
            queryPTFM.Criteria.AddCondition(PurchaseTimeframeMapping.Status, ConditionOperator.Equal, (int)PurchaseTimeframeMapping.Status_OptionSet.Active);
            EntityCollection ecPTFM = service.RetrieveMultiple(queryPTFM);
            if (ecPTFM != null && ecPTFM.Entities.Count > 0)
            {
                Entity objPTFM = ecPTFM.Entities[0];
                if (objPTFM.Attributes.Contains(PurchaseTimeframeMapping.LeadPurchaseTimeFrame))
                {
                    value = objPTFM.GetAttributeValue<OptionSetValue>(PurchaseTimeframeMapping.LeadPurchaseTimeFrame).Value;
                }
            }
            return value;
        }

        public Tuple<Decimal, Decimal> GetLoanAmountMinMaxRange(string loanAmount)
        {
            var loanAmtRange = Regex.Split(loanAmount.Replace(",", string.Empty), @"\D+").Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            var loanAmoutMin = 0m;
            var loanAmtMax = 0m;
            if (loanAmtRange.Count > 0)
            {
                Decimal.TryParse(loanAmtRange.Min(), out loanAmoutMin);
                Decimal.TryParse(loanAmtRange.Max(), out loanAmtMax);
            }
            return Tuple.Create(loanAmoutMin, loanAmtMax);
        }

        public Tuple<int, int> GetCreditScoreMinMaxRange(string creditScore)
        {
            var creditScoreRange = Regex.Split(creditScore, @"\D+").Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            int creditScoreMin = 0;
            int creditScoreMax = 0;
            if (creditScoreRange.Count > 0)
            {
                int.TryParse(creditScoreRange.Min(), out creditScoreMin);
                int.TryParse(creditScoreRange.Max(), out creditScoreMax);
            }
            return Tuple.Create(creditScoreMin, creditScoreMax);
        }
        public Tuple<decimal, decimal> GetAnnualIncomeMinMaxRange(string annualIncome)
        {
            //var c
            var anunualIncomeRange = Regex.Split(annualIncome, @"\D+").Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            var anunualIncomeRangeInts = anunualIncomeRange.Select<string, int>(q => Convert.ToInt32(q));
            decimal anunualIncomeMin = 0;
            decimal anunualIncomeMax = 0;
            if (anunualIncomeRange.Count > 0)
            {
                anunualIncomeMax = anunualIncomeRangeInts.Max();
                anunualIncomeMin = anunualIncomeRangeInts.Min();
                //decimal.TryParse(anunualIncomeRangeInts.Min(), out anunualIncomeMin);
                // decimal.TryParse(anunualIncomeRange.Max(), out anunualIncomeMax);
            }
            return Tuple.Create(anunualIncomeMin, anunualIncomeMax);
        }


        public EntityCollection CheckExistingContacttypelist(IOrganizationService context, Dictionary<string, string> dcConfigDetails, Entity targetEntity)
        {
            var fetchXml = GetMessage(Constants.AppConfigContactType, dcConfigDetails);

            if (targetEntity.Attributes.Contains(Contact.FirstName))
            {
                if (fetchXml.Contains("{tagetentityfirstname}"))
                    fetchXml = fetchXml.Replace("{tagetentityfirstname}", targetEntity.GetAttributeValue<string>(Contact.FirstName));
            }
            else
            {
                Replacecondtion(ref fetchXml, "tagetentityfirstname", Contact.FirstName);
            }

            if (targetEntity.Attributes.Contains(Contact.LastName))

            {
                if (fetchXml.Contains("{tagetentitylastname}"))
                    fetchXml = fetchXml.Replace("{tagetentitylastname}", targetEntity.GetAttributeValue<string>(Contact.LastName));
            }
            else
            {
                Replacecondtion(ref fetchXml, "tagetentitylastname", Contact.LastName);
            }
            if (targetEntity.Attributes.Contains(Contact.PersonalEmail))

            {
                if (fetchXml.Contains("{tagetentityemail}"))
                    fetchXml = fetchXml.Replace("{tagetentityemail}", targetEntity.GetAttributeValue<string>(Contact.PersonalEmail));
            }
            else
            {
                Replacecondtion(ref fetchXml, "tagetentityemail", Contact.PersonalEmail);
            }
            if (targetEntity.Attributes.Contains(Contact.MobilePhone))

            {
                if (fetchXml.Contains("{tagetentityphonenumber}"))
                    fetchXml = fetchXml = fetchXml.Replace("{tagetentityphonenumber}", targetEntity.GetAttributeValue<string>(Contact.MobilePhone));
            }
            else
            {
                Replacecondtion(ref fetchXml, "tagetentityphonenumber", Contact.MobilePhone);
            }
            EntityCollection entityCollection = context.RetrieveMultiple(new FetchExpression(string.Format(@"" + fetchXml + "")));
            if (entityCollection.Entities.Count > 0)
            {
                return entityCollection;
            }
            return null;
        }

        public string BuildPropertyConditions(Entity property, ref string xml,string dataMasterName)
        {
            StringBuilder formattedCondtion = new StringBuilder();
            if (!string.IsNullOrEmpty(dataMasterName) && dataMasterName.Equals(Constants.MovementInternalLeadProperty))
            {
                if (property.Contains(Property.City) && property.Contains(Property.State) && property.Contains(Property.Zip))
                {
                    string attributes = "" + Property.State + "," + Property.City + "," + Property.Zip + "";
                    xml = xml.Replace("{Condition}", GetConditionString(ref attributes, ref formattedCondtion, property, null, dataMasterName));

                    return xml;
                }
            }
            else
            {
                string attributes = "" + Property.AddressLine1 + "," + Property.AddressLine2 + "," + Property.State + "," + Property.City + "," + Property.Zip + "," + Property.PropertyType;
                string[] splitAttributes = attributes.Split(',');
                if (splitAttributes.Count() > 0)
                {
                    foreach (var attr in splitAttributes)
                    {

                        if (property.Contains(attr))
                        {
                            if (attr == Property.PropertyType)
                            {
                                formattedCondtion = formattedCondtion.Append("<condition attribute ='" + attr + "' operator='eq' uitype='ims_propertytype' value ='" + property.GetAttributeValue<EntityReference>(attr).Id.ToString() + "' />");
                            }
                            else
                            {
                                formattedCondtion = formattedCondtion.Append("<condition attribute ='" + attr + "' operator='eq' value ='" + WebUtility.HtmlEncode(property.GetAttributeValue<string>(attr).ToString()) + "' />");
                            }
                        }
                        else
                        {
                            formattedCondtion = formattedCondtion.Append("<condition attribute ='" + attr + "' operator='null'/>");
                        }
                    }
                    xml = xml.Replace("{Condition}", formattedCondtion.ToString());
                    return xml;
                }
            }
            //if (property.Contains(Property.AddressLine1) && property.Contains(Property.AddressLine2) && property.Contains(Property.City) && property.Contains(Property.State) && property.Contains(Property.Zip))
            //{
            //    string attributes = "" + Property.AddressLine1 + "," + Property.AddressLine2 + "," + Property.State + "," + Property.City + "," + Property.Zip + "";
            //    xml = xml.Replace("{Condition}", GetConditionString(ref attributes, ref formattedCondtion, property, null));
            //    return xml;
            //}
            //else if (property.Contains(Property.AddressLine1) && property.Contains(Property.AddressLine2) && property.Contains(Property.City) && property.Contains(Property.State))
            //{
            //    string attributes = "" + Property.AddressLine1 + "," + Property.AddressLine2 + "," + Property.State + "," + Property.City + "";
            //    xml = xml.Replace("{Condition}", GetConditionString(ref attributes, ref formattedCondtion, property, null));
            //    return xml;
            //}
            //else if (property.Contains(Property.AddressLine1) && property.Contains(Property.AddressLine2) && property.Contains(Property.City))
            //{
            //    string attributes = "" + Property.AddressLine1 + "," + Property.AddressLine2 + "," + Property.City + "";
            //    xml = xml.Replace("{Condition}", GetConditionString(ref attributes, ref formattedCondtion, property, null));
            //    return xml;
            //}
            //else if (property.Contains(Property.AddressLine1) && property.Contains(Property.AddressLine2))
            //{
            //    string attributes = "" + Property.AddressLine1 + "," + Property.AddressLine2 + "";
            //    xml = xml.Replace("{Condition}", GetConditionString(ref attributes, ref formattedCondtion, property, null));
            //    return xml;
            //}
            //else if (property.Contains(Property.PropertyType) && property.Contains(Property.City) && property.Contains(Property.State) && property.Contains(Property.Zip))
            //{
            //    string attributes = "" + Property.PropertyType + "," + Property.City + "," + Property.State + "," + Property.Zip + "";
            //    xml = xml.Replace("{Condition}", GetConditionString(ref attributes, ref formattedCondtion, property, null));
            //    return xml;
            //}
            //else if (property.Contains(Property.PropertyType) && property.Contains(Property.City) && property.Contains(Property.State))
            //{
            //    string attributes = "" + Property.PropertyType + "," + Property.City + "," + Property.State + "";
            //    xml = xml.Replace("{Condition}", GetConditionString(ref attributes, ref formattedCondtion, property, null));
            //    return xml;
            //}
            //else if (property.Contains(Property.PropertyType) && property.Contains(Property.City))
            //{
            //    string attributes = "" + Property.PropertyType + "," + Property.City + "";
            //    xml = xml.Replace("{Condition}", GetConditionString(ref attributes, ref formattedCondtion, property, null));
            //    return xml;
            //}
            return string.Empty;
        }

        public string GetConditionString(ref string attributes, ref StringBuilder formattedCondtion, Entity property,EntityReference defaultTeamReference,string dataMaster=null)
        {
            string[] splitAttributes = attributes.Split(',');
            if (splitAttributes.Count() > 0)
            {
                if (dataMaster != null && dataMaster.Equals(Constants.MovementInternalLeadProperty))
                {
                    formattedCondtion = formattedCondtion.Append("<condition attribute ='" + Property.AddressLine1 + "' operator='null'/>");
                    formattedCondtion = formattedCondtion.Append("<condition attribute ='" + Property.AddressLine2 + "' operator='null'/>");
                    formattedCondtion = formattedCondtion.Append("<condition attribute ='" + Property.PropertyType + "' operator='null'/>");
                }
                foreach (var attr in splitAttributes)
                {

                    if (attr != Property.PropertyType && attr != Lead.Owner && attr!=Lead.FromBlend)
                    {
                        if (property.Contains(attr))
                        {
                            formattedCondtion = formattedCondtion.Append("<condition attribute ='" + attr + "' operator='eq' value ='" + WebUtility.HtmlEncode(property.GetAttributeValue<string>(attr).ToString()) + "' />");
                        }
                    }
                    else if(attr==Lead.FromBlend)
                    {
                        if(property.Contains(attr))
                        {
                            if (property.GetAttributeValue<bool>(attr) == true)
                            {
                                formattedCondtion = formattedCondtion.Append("<condition attribute ='" + attr + "' operator='eq' value ='1' />");
                            }
                            else
                                formattedCondtion = formattedCondtion.Append("<condition attribute ='" + attr + "' operator='eq' value ='0' />");
                        }
                    }
                    else if (attr == Property.PropertyType)
                    {
                        if (property.Contains(attr))
                        {
                            formattedCondtion = formattedCondtion.Append("<condition attribute ='" + attr + "' operator='eq' uitype='ims_propertytype' value ='" + property.GetAttributeValue<EntityReference>(attr).Id.ToString() + "' />");
                        }
                    }
                    else if (attr == Lead.Owner && defaultTeamReference != null)
                    {
                        formattedCondtion = formattedCondtion.Append("<condition attribute ='" + attr + "' operator='eq' uitype='teams' value ='" + defaultTeamReference.Id.ToString() + "' />");
                    }
                }
                return formattedCondtion.ToString();
            }

            return null;
        }

        public string BuildConditionString(ref string attributes, ref StringBuilder formattedCondtion, Entity property)
        {
            string[] splitAttributes = attributes.Split(',');
            if (splitAttributes.Count() > 0)
            {
                foreach (var attr in splitAttributes)
                {
                    formattedCondtion = formattedCondtion.Append("<condition attribute ='" + attr + "' operator='eq' value ='" + WebUtility.HtmlEncode(property.GetAttributeValue<string>(attr).ToString()) + "' />");
                }
                return formattedCondtion.ToString();
            }

            return null;
        }
        public string GetPropertyType(Mapping mapping, IOrganizationService service, ref string errorMessage)
        {
            Guid propertytypeId = Guid.Empty;
            string fetchxml = string.Format(@"
                <fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
                  "<entity name='ims_propertytype'>" +
                    "<attribute name='ims_propertytypeid' />" +
                    "<attribute name='ims_name' />" +
                    "<attribute name='createdon' />" +
                    "<order attribute='ims_name' descending='false' />" +
                    "<filter type='and'>" +
                      "<condition attribute='statecode' operator='eq' value='0' />" +
                      "<filter type='or'>" +
                        "<condition attribute='ims_name' operator='eq' value='" + mapping.value + "' />" +
                        "<condition attribute='ims_possiblepropertyvalues' operator='like' value='%" + mapping.value + "%' />" +
                      "</filter>" +
                    "</filter>" +
                  "</entity>" +
                "</fetch>");
            EntityCollection ecPropertyType = service.RetrieveMultiple(new FetchExpression(fetchxml));
            if (ecPropertyType != null && ecPropertyType.Entities.Count > 0)
            {
                propertytypeId = ecPropertyType.Entities[0].Id;
            }

            //Create new lookup record if flag is set to true in mappings
            if (mapping.CreatelookupRecord && propertytypeId == Guid.Empty)
            {
                propertytypeId = CreateEntityRecord(mapping, service, ref errorMessage);
            }

            return (propertytypeId != Guid.Empty) ? propertytypeId.ToString() : string.Empty;
        }

        /// <summary>
        /// Associate, DisAssociate Other contacts from Loan to Lead
        /// </summary>
        public void AssociateDisasscociateContacts(IOrganizationService service, Entity loanEntity, ref string errorMessage, ref bool ValidationStatus)
        {
            try
            {
                if (loanEntity.Contains(Loan.Borrower))
                {
                    if (loanEntity.Contains(Loan.BuyersAgent))
                    {
                        if (!CheckAsscocationExists(service, loanEntity.GetAttributeValue<EntityReference>(Loan.BuyersAgent).Id, loanEntity.GetAttributeValue<EntityReference>(Loan.Borrower).Id))
                        {
                            Asscociate(service, loanEntity.GetAttributeValue<EntityReference>(Loan.BuyersAgent), loanEntity.GetAttributeValue<EntityReference>(Loan.Borrower));
                        }
                    }
                    if (loanEntity.Contains(Loan.Attorney))
                    {
                        if (!CheckAsscocationExists(service, loanEntity.GetAttributeValue<EntityReference>(Loan.Attorney).Id, loanEntity.GetAttributeValue<EntityReference>(Loan.Borrower).Id))
                        {
                            Asscociate(service, loanEntity.GetAttributeValue<EntityReference>(Loan.Attorney), loanEntity.GetAttributeValue<EntityReference>(Loan.Borrower));
                        }
                    }
                    if (loanEntity.Contains(Loan.SettlementAgent))
                    {
                        if (!CheckAsscocationExists(service, loanEntity.GetAttributeValue<EntityReference>(Loan.SettlementAgent).Id, loanEntity.GetAttributeValue<EntityReference>(Loan.Borrower).Id))
                        {
                            Asscociate(service, loanEntity.GetAttributeValue<EntityReference>(Loan.SettlementAgent), loanEntity.GetAttributeValue<EntityReference>(Loan.Borrower));
                        }
                    }
                    if (loanEntity.Contains(Loan.SellersAgent))
                    {
                        if (!CheckAsscocationExists(service, loanEntity.GetAttributeValue<EntityReference>(Loan.SellersAgent).Id, loanEntity.GetAttributeValue<EntityReference>(Loan.Borrower).Id))
                        {
                            Asscociate(service, loanEntity.GetAttributeValue<EntityReference>(Loan.SellersAgent), loanEntity.GetAttributeValue<EntityReference>(Loan.Borrower));
                        }
                    }
                    if (loanEntity.Contains(Loan.LoanOfficerAssistant))
                    {
                        Guid templateId = GetAccessTeamTemplate(service);
                        if (templateId != Guid.Empty)
                        {
                            AddUserToAccessTeam(service, templateId, loanEntity.GetAttributeValue<EntityReference>(Loan.LoanOfficerAssistant).Id, loanEntity.GetAttributeValue<EntityReference>(Loan.Borrower));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.Message);
                UpdateValidationMessage(ex.Message, ref errorMessage);
                ValidationStatus = false;
            }
        }
        public void Asscociate(IOrganizationService service, EntityReference entityReference, EntityReference targetReference)
        {
            var contactReferences = new EntityReferenceCollection();
            contactReferences.Add(entityReference);
            var contactleads_association = new Relationship(Loan.contactleads_association);
            AssociateRequest request = new AssociateRequest()
            {
                RelatedEntities = contactReferences,
                Relationship = contactleads_association,
                Target = targetReference
            };
            service.Execute(request);
        }
        public void Disasscociate(IOrganizationService service, EntityReference existingRecordObject, EntityReference targetRecordReference)
        {
            if (existingRecordObject != null && targetRecordReference != null)
            {
                if (CheckAsscocationExists(service, existingRecordObject.Id, targetRecordReference.Id))
                {
                    var contactReferences = new EntityReferenceCollection();
                    contactReferences.Add(existingRecordObject);
                    var contactleads_association = new Relationship(Loan.contactleads_association);
                    DisassociateRequest request = new DisassociateRequest()
                    {
                        RelatedEntities = contactReferences,
                        Relationship = contactleads_association,
                        Target = targetRecordReference
                    };
                    service.Execute(request);
                }
            }
        }
        public void AddUserToAccessTeam(IOrganizationService service, Guid templateId, Guid systemUserGuid, EntityReference entityReference)
        {
            if (CheckUserHasRole(service, entityReference.Id))
            {
                AddUserToRecordTeamRequest adduser = new AddUserToRecordTeamRequest()
                {
                    Record = new EntityReference(entityReference.LogicalName, entityReference.Id),
                    SystemUserId = systemUserGuid,
                    TeamTemplateId = templateId
                };
                service.Execute(adduser);
            }
        }
        public bool CheckUserHasRole(IOrganizationService service, Guid userId)
        {
            // Create a QueryExpression.
            QueryExpression qe = new QueryExpression();
            qe.EntityName = "role";
            qe.ColumnSet = new ColumnSet("roleid");

            // Set up the join between the role entity
            // and the intersect table systemuserroles.
            LinkEntity le = new LinkEntity();
            le.LinkFromEntityName = "role";
            le.LinkFromAttributeName = "roleid";
            le.LinkToEntityName = "systemuserroles";
            le.LinkToAttributeName = "roleid";

            // Set up the join between the intersect table
            // systemuserroles and the systemuser entity.
            LinkEntity le2 = new LinkEntity();
            le2.LinkFromEntityName = "systemuserroles";
            le2.LinkFromAttributeName = "systemuserid";
            le2.LinkToEntityName = "systemuser";
            le2.LinkToAttributeName = "systemuserid";

            // The condition is to find the user ID.
            ConditionExpression ce = new ConditionExpression("systemuserid", ConditionOperator.Equal, userId);

            le2.LinkCriteria = new FilterExpression();
            le2.LinkCriteria.AddCondition(ce);

            le.LinkEntities.Add(le2);
            qe.LinkEntities.Add(le);

            // Execute the query.
            EntityCollection bec = service.RetrieveMultiple(qe);
            if (bec.Entities.Count > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checking the Contact is already associated or not
        /// </summary>
        /// <param name="service"></param>
        /// <param name="contactId"></param>
        /// <param name="leadId"></param>
        /// <returns></returns>
        public bool CheckAsscocationExists(IOrganizationService service, Guid contactId, Guid leadId)
        {
            // Execute the query.
            EntityCollection bec = service.RetrieveMultiple(new FetchExpression(string.Format(@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>" +
  "<entity name='contact'>" +
    "<attribute name='fullname' />" +
    "<attribute name='telephone1' />" +
    "<attribute name='contactid' />" +
    "<order attribute='fullname' descending='false' />" +
    "<filter type='and'>" +
      "<condition attribute='contactid' operator='eq' uiname='Aaron Donnachie' uitype='contact' value='" + contactId.ToString() + "' />" +
    "</filter>" +
    "<link-entity name='contactleads' from='contactid' to='contactid' visible='false' intersect='true'>" +
      "<link-entity name='lead' from='leadid' to='leadid' alias='ag'>" +
        "<filter type='and'>" +
          "<condition attribute='leadid' operator='eq' uiname='Jacob Beaver' uitype='lead' value='" + leadId.ToString() + "' />" +
        "</filter>" +
      "</link-entity>" +
    "</link-entity>" +
  "</entity>" +
"</fetch>")));
            if (bec.Entities.Count > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get LOA Team Template
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public Guid GetAccessTeamTemplate(IOrganizationService service)
        {
            //  Query using ConditionExpression and FilterExpression
            ConditionExpression condition = new ConditionExpression("teamtemplatename", ConditionOperator.Equal, Constants.LeadLOAAccessTeam_Template);
            // filter creation
            FilterExpression filter = new FilterExpression();
            //condition added
            filter.Conditions.Add(condition);
            //   create query expression
            QueryExpression query = new QueryExpression("teamtemplate");
            //filter added to query
            query.Criteria.AddFilter(filter);
            //retrieve all columns
            query.ColumnSet = new ColumnSet("teamtemplatename");
            // execute query which will retrieve the Access team teamplate
            EntityCollection ec = service.RetrieveMultiple(query);
            if (ec.Entities.Count > 0)
            {
                return ec.Entities.FirstOrDefault().Id;
            }
            return Guid.Empty;
        }
        /// <summary>
        /// Create Note for Lead
        /// 
        /// 
        /// </summary>

        public Guid GetAccessTeamTemplateLoan(IOrganizationService service)
        {
            //  Query using ConditionExpression and FilterExpression
            ConditionExpression condition = new ConditionExpression("teamtemplatename", ConditionOperator.Equal, Constants.GetAccessTeamTemplateLoan);
            // filter creation
            FilterExpression filter = new FilterExpression();
            //condition added
            filter.Conditions.Add(condition);
            //   create query expression
            QueryExpression query = new QueryExpression("teamtemplate");
            //filter added to query
            query.Criteria.AddFilter(filter);
            //retrieve all columns
            query.ColumnSet = new ColumnSet("teamtemplatename");
            // execute query which will retrieve the Access team teamplate
            EntityCollection ec = service.RetrieveMultiple(query);
            if (ec.Entities.Count > 0)
            {
                return ec.Entities.FirstOrDefault().Id;
            }
            return Guid.Empty;
        }


        public Guid GetAccessTeamTemplatecontact(IOrganizationService service)
        {
            //  Query using ConditionExpression and FilterExpression
            ConditionExpression condition = new ConditionExpression("teamtemplatename", ConditionOperator.Equal, Constants.GetAccessTeamTemplatecontact);
            // filter creation
            FilterExpression filter = new FilterExpression();
            //condition added
            filter.Conditions.Add(condition);
            //   create query expression
            QueryExpression query = new QueryExpression("teamtemplate");
            //filter added to query
            query.Criteria.AddFilter(filter);
            //retrieve all columns
            query.ColumnSet = new ColumnSet("teamtemplatename");
            // execute query which will retrieve the Access team teamplate
            EntityCollection ec = service.RetrieveMultiple(query);
            if (ec.Entities.Count > 0)
            {
                return ec.Entities.FirstOrDefault().Id;
            }
            return Guid.Empty;
        }


        public void CreateNote(IOrganizationService service, Entity objEntity, string noteInfo)
        {

            Entity Note = new Entity(Annotation.EntityName);
            Note[Annotation.ObjectId] = new EntityReference(objEntity.LogicalName, objEntity.Id);
            Note[Annotation.ObjectTypeCode] = objEntity.LogicalName;
            Note[Annotation.Subject] = "General Note";
            Note[Annotation.NoteText] = noteInfo;
            service.Create(Note);

        }

        ///<summary>
        ///Phone Number Formatting
        ///</summary>
        public string PhoneNumberFormatting(string phoneNumber)
        {
            string formattedPhoneNumber = string.Empty;
            string phoneFormat = string.Empty;
            try
            {
                if (phoneNumber != null)
                {
                    phoneNumber = phoneNumber.Replace(@"/\D / g", "");
                    Regex reg = new Regex(@"^[0-9+]*$", RegexOptions.Multiline);

                    if (reg.IsMatch(phoneNumber))
                    {
                        if (phoneNumber.Length == 10)
                        {
                            phoneFormat = "(###) ###-####";
                            formattedPhoneNumber = Convert.ToInt64(phoneNumber).ToString(phoneFormat);

                        }
                        else if (phoneNumber.Length > 10 && phoneNumber.Length <= 13)
                        {
                            if (phoneNumber.IndexOf('+') == 0 || phoneNumber.IndexOf('1') == 0 || phoneNumber.IndexOf('0') == 0)
                            {
                                phoneFormat = "+#-###-###-####";
                            }
                            //else
                            //{
                            //    phoneFormat = "00#-###-###-####";
                            //}
                            formattedPhoneNumber = Convert.ToInt64(phoneNumber).ToString(phoneFormat);
                        }
                        else
                        {
                            //throw new Exception("Invalid due to exceeded number of digits  ");

                        }
                    }
                    else
                    {
                        //throw new Exception("Invalid due to alphanumeric characters ");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return formattedPhoneNumber;
        }

        public string PhoneNumberUnformatting(string phoneNumber)
        {
            if (phoneNumber != null)
            {
                string pattern = @"[^a-zA-Z0-9]";
                phoneNumber = Regex.Replace(phoneNumber, pattern, string.Empty);
                if (phoneNumber.IndexOf('1') == 0 && phoneNumber.Length == 11)
                {
                    phoneNumber = phoneNumber.Remove(0, 1);
                }
                else if (phoneNumber.IndexOf('0') == 0 && phoneNumber.Length == 13)
                {
                    phoneNumber = phoneNumber.Remove(0, 3);
                }
            }

            return phoneNumber;

        }

        public void UpdateContactCategory(ref Entity objEntity, EntityReference attr)
        {
            //if (attr.Name != null && (attr.Name.Equals("Buyers Agent", StringComparison.OrdinalIgnoreCase) || attr.Name.Equals("Sellers Agent", StringComparison.OrdinalIgnoreCase)))
            //{
            //    //Updating to Realtor if the contact type is Buyers Agent Sellers Agent
            //    if (!objEntity.Contains(Contact.ContactCategory))
            //        objEntity.Attributes.Add(Contact.ContactCategory, new OptionSetValue(176390000));
            //}
            //else
            //{
            //    //Updating to Loan Participants if the contact type is neither Buyers Agent nor Sellers Agent
            //    if (!objEntity.Contains(Contact.ContactCategory))
            //        objEntity.Attributes.Add(Contact.ContactCategory, new OptionSetValue(176390001));
            //}
        }
        public EntityReference GetBorrower(IOrganizationService service, Entity postImage, Dictionary<string, string> dcConfigDetails)
        {
            var fetchXml = GetMessage(Constants.GetBorrower_ExternalIDDoesNotExists, dcConfigDetails);
            bool recordFound = false;
            if (!postImage.Contains(LoanStaging.BorrowerExternalID) && postImage.Contains(LoanStaging.LOExternalID))
            {
                StringBuilder formattedCondtion = new StringBuilder();

                if (postImage.Contains(LoanStaging.BorrowerFirstName) && postImage.Contains(LoanStaging.BorrowerLastName) && postImage.Contains(LoanStaging.BorrowerEmail))
                {

                    if (fetchXml != null)
                    {
                        Entity lead = new Entity(Lead.EntityName);
                        lead.Attributes.Add(Lead.FirstName, postImage.GetAttributeValue<string>(LoanStaging.BorrowerFirstName));
                        lead.Attributes.Add(Lead.LastName, postImage.GetAttributeValue<string>(LoanStaging.BorrowerLastName));
                        lead.Attributes.Add(Lead.PersonalEmail, postImage.GetAttributeValue<string>(LoanStaging.BorrowerEmail));
                        lead.Attributes.Add(Lead.LOExternalId, postImage.GetAttributeValue<string>(LoanStaging.LOExternalID));
                        string attributes = "" + Lead.FirstName + "," + Lead.LastName + "," + Lead.PersonalEmail;
                        fetchXml = fetchXml.Replace("{Condition}", GetConditionString(ref attributes, ref formattedCondtion, lead,null));
                        EntityCollection ec = service.RetrieveMultiple(new FetchExpression(fetchXml));
                        if (ec.Entities.Count > 0)
                        {
                            recordFound = true;
                            return ec.Entities.FirstOrDefault().ToEntityReference();
                        }
                    }
                }
                if (!recordFound && postImage.Contains(LoanStaging.BorrowerFirstName) && postImage.Contains(LoanStaging.BorrowerLastName) && postImage.Contains(LoanStaging.BorrowerCellPhone))
                {
                    //var fetchXml = GetMessage(Constants.GetBorrower_ExternalIDDoesNotExists, dcConfigDetails);
                    if (fetchXml != null)
                    {
                        Entity lead = new Entity(Lead.EntityName);
                        lead.Attributes.Add(Lead.FirstName, postImage.GetAttributeValue<string>(LoanStaging.BorrowerFirstName));
                        lead.Attributes.Add(Lead.LastName, postImage.GetAttributeValue<string>(LoanStaging.BorrowerLastName));
                        lead.Attributes.Add(Lead.MobilePhone, postImage.GetAttributeValue<string>(LoanStaging.BorrowerCellPhone));
                        lead.Attributes.Add(Lead.LOExternalId, postImage.GetAttributeValue<string>(LoanStaging.LOExternalID));
                        string attributes = "" + Lead.FirstName + "," + Lead.LastName + "," + Lead.PersonalEmail;
                        fetchXml = fetchXml.Replace("{Condition}", GetConditionString(ref attributes, ref formattedCondtion, lead,null));
                        EntityCollection ec = service.RetrieveMultiple(new FetchExpression(fetchXml));
                        if (ec.Entities.Count > 0)
                        {
                            recordFound = true;
                            return ec.Entities.FirstOrDefault().ToEntityReference();
                        }
                    }
                }
                if (!recordFound && postImage.Contains(LoanStaging.BorrowerFirstName) && postImage.Contains(LoanStaging.BorrowerLastName))
                {
                    //var fetchXml = GetMessage(Constants.GetBorrower_ExternalIDDoesNotExists, dcConfigDetails);
                    if (fetchXml != null)
                    {
                        Entity lead = new Entity(Lead.EntityName);
                        lead.Attributes.Add(Lead.FirstName, postImage.GetAttributeValue<string>(LoanStaging.BorrowerFirstName));
                        lead.Attributes.Add(Lead.LastName, postImage.GetAttributeValue<string>(LoanStaging.BorrowerLastName));
                        lead.Attributes.Add(Lead.MobilePhone, postImage.GetAttributeValue<string>(LoanStaging.BorrowerCellPhone));
                        lead.Attributes.Add(Lead.LOExternalId, postImage.GetAttributeValue<string>(LoanStaging.LOExternalID));
                        string attributes = "" + Lead.FirstName + "," + Lead.LastName + "," + Lead.PersonalEmail;
                        fetchXml = fetchXml.Replace("{Condition}", GetConditionString(ref attributes, ref formattedCondtion, lead,null));
                        EntityCollection ec = service.RetrieveMultiple(new FetchExpression(fetchXml));
                        if (ec.Entities.Count > 0)
                        {
                            recordFound = true;
                            return ec.Entities.FirstOrDefault().ToEntityReference();
                        }
                    }
                }
            }
            else if (postImage.Contains(LoanStaging.BorrowerExternalID) && postImage.Contains(LoanStaging.LOExternalID) && postImage.Contains(LoanStaging.BorrowerEmail))
            {
                var xml = GetMessage(Constants.GetBorrower_ExternalIDExists, dcConfigDetails);
                if (xml != null)
                {
                    xml = xml.Replace("{" + LoanStaging.BorrowerExternalID + "}", postImage.GetAttributeValue<string>(LoanStaging.BorrowerExternalID));
                    //xml = xml.Replace("{" + LoanStaging.LOExternalID + "}", postImage.GetAttributeValue<string>(LoanStaging.LOExternalID));
                    xml = xml.Replace("{" + LoanStaging.BorrowerEmail + "}", postImage.GetAttributeValue<string>(LoanStaging.BorrowerEmail));
                    EntityCollection ec = service.RetrieveMultiple(new FetchExpression(xml));
                    if (ec.Entities.Count > 0)
                    {
                        Entity en = ec.Entities.FirstOrDefault();
                        if (en.Contains(Lead.LOExternalId))
                        {
                            if (postImage.GetAttributeValue<string>(LoanStaging.LoanExternalID).Equals(en.GetAttributeValue<string>(Lead.LOExternalId)))
                            {
                                return en.ToEntityReference();
                            }
                            else if (!postImage.GetAttributeValue<string>(LoanStaging.LoanExternalID).Equals(en.GetAttributeValue<string>(Lead.LOExternalId)))
                            {
                                Entity entity = new Entity(Lead.EntityName);
                                if (postImage.Contains(LoanStaging.BorrowerFirstName))
                                    en.Attributes.Add(Lead.FirstName, postImage.GetAttributeValue<string>(LoanStaging.BorrowerFirstName));
                                if (postImage.Contains(LoanStaging.BorrowerLastName))
                                    en.Attributes.Add(Lead.LastName, postImage.GetAttributeValue<string>(LoanStaging.BorrowerLastName));
                                if (postImage.Contains(LoanStaging.BorrowerCellPhone))
                                    en.Attributes.Add(Lead.MobilePhone, postImage.GetAttributeValue<string>(LoanStaging.BorrowerCellPhone));
                                if (postImage.Contains(LoanStaging.BorrowerEmail))
                                    en.Attributes.Add(Lead.PersonalEmail, postImage.GetAttributeValue<string>(LoanStaging.BorrowerEmail));
                                if (postImage.Contains(LoanStaging.LOExternalID))
                                    en.Attributes.Add(Lead.LOExternalId, postImage.GetAttributeValue<string>(LoanStaging.LOExternalID));
                                Guid leadId = service.Create(entity);
                                return new EntityReference(Lead.EntityName, leadId);
                            }
                        }
                    }
                }
            }
            return null;
        }

        public string CreateCoborrower(Mapping mapping, IOrganizationService service, ref bool ValidationStatus, ref bool canReturn, ref string errorMessage, Dictionary<string, string> dcConfigDetails, Entity staging, IOrganizationService systemService)
        {
            Entity objAccount = new Entity(Account.EntityName);
            string propertyName = string.Empty;
            Guid entityId = Guid.Empty;
            string importDataMaster = string.Empty;
            Guid importDataMasterId = Guid.Empty;
            EntityReference defaultTeam = null;
            List<Mapping> mappings = new List<Mapping>();
            //bool isLoanOfficer = CheckIsLoanOfficer(service, contextUserId, dcConfigDetails);
            //Get Import Data Master to fetch mappings
            if (!string.IsNullOrEmpty(mapping.DataMaster))
            {

                importDataMasterId = GetImportDataMasterAndDefaultTeam(mapping.DataMaster, ref defaultTeam, service);
                if (importDataMasterId == Guid.Empty)
                {
                    UpdateStagingLog(staging.Id, Guid.Empty, staging.LogicalName, "Import Data Master Configuration is missing for Lookup field " + mapping.CrmDisplayName + " is missing.", true, service);
                    return (entityId == Guid.Empty ? string.Empty : entityId.ToString());
                }
                else if (importDataMasterId != Guid.Empty)
                {
                    if (!FetchMappings(importDataMasterId, ref mappings, service, ref errorMessage))
                    {
                        UpdateStagingLog(staging.Id, Guid.Empty, staging.LogicalName, errorMessage, true, service);
                        return (entityId == Guid.Empty ? string.Empty : entityId.ToString());
                    }
                    else
                    {
                        foreach (Common.Mapping objMapping in mappings)
                        {
                            try
                            {
                                Common.Mapping mappingObject = objMapping;
                                GetValueFromSourceEntity(staging, ref mappingObject, ref ValidationStatus, ref canReturn, ref errorMessage);
                                if (!string.IsNullOrEmpty(mappingObject.value))
                                {
                                    SetValueToTargetEntity(ref objAccount, mappingObject, service, ref ValidationStatus, ref canReturn, ref errorMessage, dcConfigDetails, staging);
                                }
                            }
                            catch (Exception ex)
                            {
                                //validationStatus-> False canReturn->true  Add log UpdateValidationMessage
                                UpdateValidationMessage("Error While FormTargetEntityObject for Contact OR Other Contact " + ex.Message, ref errorMessage);
                            }
                        }
                        if (objAccount.Attributes.Count > 0)
                        {
                            //Checking object Contains LO External ID or not
                            if (!objAccount.Contains(Lead.LOExternalId))
                            {
                                string LOExternalId = GetLOExternalId(staging.GetAttributeValue<EntityReference>(LeadStaging.Owner), service);
                                if (LOExternalId != null)
                                    objAccount[SystemUser.LOExternalId] = LOExternalId;
                            }
                            bool isLoanOfficer = CheckIsLoanOfficer(systemService, contextUserId, dcConfigDetails);
                            Entity account = CheckAccountExistance(false, isLoanOfficer, staging, objAccount, service, dcConfigDetails);
                            if (account != null)
                            {
                                objAccount[Account.IsCoborrower]= true;
                                if (staging.Contains(LoanStaging.LOExternalID))
                                {
                                    OwnershipAssignment(ref objAccount, service, defaultTeamReference, manualImportReference, staging);
                                }
                                objAccount.Id = account.Id;
                                service.Update(objAccount);
                                entityId = objAccount.Id;

                            }
                            else if (account == null)
                            {
                                objAccount.Attributes.Add(Account.IsCoborrower, true);
                                OwnershipAssignment(ref objAccount, service, defaultTeamReference, manualImportReference, staging);
                                if (objAccount.Contains(Account.FirstName) || objAccount.Contains(Account.LastName))
                                    entityId = service.Create(objAccount);
                            }
                        }
                    }
                }
            }
            return (entityId == Guid.Empty ? string.Empty : entityId.ToString());
        }
        public string CreateBorrower(Mapping mapping, IOrganizationService service, ref bool ValidationStatus, ref bool canReturn, ref string errorMessage, Dictionary<string, string> dcConfigDetails, Entity staging, IOrganizationService systemService)
        {
            Guid entityId = Guid.Empty;
            EntityReference defaultTeam = null;
            Entity en = CreateEntityObject(ref defaultTeam, Lead.EntityName, mapping, service, ref ValidationStatus, ref canReturn, ref errorMessage, dcConfigDetails, staging);
            if (en.Attributes.Count > 0)
            {
                bool isLoanOfficer = CheckIsLoanOfficer(systemService, contextUserId, dcConfigDetails);
                Entity lead = CheckLeadExistiance(false, isLoanOfficer, en, service, staging, dcConfigDetails);
                if (lead != null)
                {
                    OwnershipAssignment(ref en, service, defaultTeamReference, manualImportReference, staging);
                    en.Id = lead.Id;
                    service.Update(en);
                    entityId = en.Id;

                }
                else if (lead == null)
                {
                    OwnershipAssignment(ref en, service, defaultTeamReference, manualImportReference, staging);
                    if (en.Contains(Lead.FirstName) || en.Contains(Lead.LastName))
                        entityId = service.Create(en);
                }
            }
            return (entityId == Guid.Empty ? string.Empty : entityId.ToString());
        }

        public Guid GetUserByQueryAttribute(string loexternalId, IOrganizationService service)
        {
            QueryByAttribute queryByAttribute = new QueryByAttribute(SystemUser.EntityName);
            queryByAttribute.AddAttributeValue(SystemUser.LOExternalId, loexternalId);
            queryByAttribute.AddAttributeValue(SystemUser.IsDisabled, false);
            queryByAttribute.ColumnSet = new ColumnSet(SystemUser.FullName);
            EntityCollection ec = service.RetrieveMultiple(queryByAttribute);
            if (ec.Entities.Count > 0)
            {
                return ec.Entities.FirstOrDefault().Id;
            }
            return Guid.Empty;
        }

        public Entity CreateEntityObject(ref EntityReference defaultTeam, string entityName, Mapping mapping, IOrganizationService service, ref bool ValidationStatus, ref bool canReturn, ref string errorMessage, Dictionary<string, string> dcConfigDetails, Entity staging)
        {
            Entity objEntity = new Entity(entityName);
            string propertyName = string.Empty;
            Guid entityId = Guid.Empty;
            string importDataMaster = string.Empty;
            Guid importDataMasterId = Guid.Empty;
            //EntityReference defaultTeam = null;
            List<Mapping> mappings = new List<Mapping>();
            //Get Import Data Master to fetch mappings
            if (!string.IsNullOrEmpty(mapping.DataMaster))
            {

                importDataMasterId = GetImportDataMasterAndDefaultTeam(mapping.DataMaster, ref defaultTeam, service);
                if (importDataMasterId == Guid.Empty)
                {
                    UpdateStagingLog(staging.Id, Guid.Empty, staging.LogicalName, "Import Data Master Configuration is missing for Lookup field " + mapping.CrmDisplayName + " is missing.", true, service);
                    return objEntity;
                }
                else if (importDataMasterId != Guid.Empty)
                {
                    if (!FetchMappings(importDataMasterId, ref mappings, service, ref errorMessage))
                    {
                        UpdateStagingLog(staging.Id, Guid.Empty, staging.LogicalName, errorMessage, true, service);
                        return objEntity;
                    }
                    else
                    {
                        foreach (Common.Mapping objMapping in mappings)
                        {
                            try
                            {
                                Common.Mapping mappingObject = objMapping;
                                GetValueFromSourceEntity(staging, ref mappingObject, ref ValidationStatus, ref canReturn, ref errorMessage);
                                if (!string.IsNullOrEmpty(mappingObject.value))
                                {
                                    SetValueToTargetEntity(ref objEntity, mappingObject, service, ref ValidationStatus, ref canReturn, ref errorMessage, dcConfigDetails, staging);
                                }
                            }
                            catch (Exception ex)
                            {
                                //validationStatus-> False canReturn->true  Add log UpdateValidationMessage
                                UpdateValidationMessage("Error While FormTargetEntityObject for Contact OR Other Contact " + ex.Message, ref errorMessage);
                            }
                        }
                    }
                }

                //Checking object Contains LO External ID or not
                if (!objEntity.Contains(Lead.LOExternalId))
                {
                    string LOExternalId = GetLOExternalId(staging.GetAttributeValue<EntityReference>(LeadStaging.Owner), service);
                    if (LOExternalId != null)
                        objEntity[SystemUser.LOExternalId] = LOExternalId;
                }
            }
            return objEntity;
        }

        public decimal CalculateDownPaymentPercentage(decimal downPaymentAmount, decimal loanAmount)
        {
            decimal downPaymentPercentage = 0;
            downPaymentPercentage = (downPaymentAmount / loanAmount) * 100;
            return Math.Round(downPaymentPercentage, 2);
        }
        public decimal CalculateDownPaymentAmount(decimal downPaymentPercentage, decimal loanAmount)
        {
            decimal downPaymentAmount = 0;
            downPaymentAmount = (downPaymentPercentage * loanAmount) / 100;
            return Math.Round(downPaymentAmount, 2);
        }

        public decimal CalculateDownPaymentAmountFromLoanAmount(decimal downPaymentPercentage, decimal loanAmount)
        {
            decimal downPaymentAmount = 0;
            downPaymentAmount = (downPaymentPercentage * loanAmount) / (100 - downPaymentPercentage);
            return downPaymentAmount;
        }

        public void GetCrmUserGuid(ref string attrValue, IOrganizationService service)
        {
            QueryByAttribute byAttribute = new QueryByAttribute(SystemUser.EntityName);
            byAttribute.ColumnSet = new ColumnSet(SystemUser.DomainName);
            byAttribute.AddAttributeValue(SystemUser.InternalEmail, attrValue);
            EntityCollection entityCollection = service.RetrieveMultiple(byAttribute);
            if (entityCollection.Entities.Count > 0)
            {
                foreach (Entity en in entityCollection.Entities)
                {
                    attrValue = en.Id.ToString();
                }
            }
        }

        public string GetLOExternalId(EntityReference userId, IOrganizationService service)
        {
            QueryByAttribute byAttribute = new QueryByAttribute(SystemUser.EntityName);
            byAttribute.ColumnSet = new ColumnSet(SystemUser.LOExternalId);
            byAttribute.AddAttributeValue(SystemUser.EntityId, userId.Id);
            EntityCollection entityCollection = service.RetrieveMultiple(byAttribute);
            if (entityCollection.Entities.Count > 0)
            {
                foreach (Entity en in entityCollection.Entities)
                {
                    if (en.Contains(SystemUser.LOExternalId))
                        return en.GetAttributeValue<string>(SystemUser.LOExternalId);
                }
            }
            return null;
        }

        /// <summary>
        /// This method is to assign the ownership of Coborrower and Borrower in Lead and Loan Staging Respectivley
        /// </summary>
        /// <param name="targetEntity"></param>
        /// <param name="service"></param>
        /// <param name="defaultTeamReference"></param>
        /// <param name="manualImportReference"></param>
        /// <param name="staging"></param>
        public void OwnershipAssignment(ref Entity targetEntity, IOrganizationService service, EntityReference defaultTeamReference, EntityReference manualImportReference, Entity staging)
        {
            if (staging.Contains(LoanStaging.LOExternalID))
            {
                Guid userGuid = GetUserByQueryAttribute(staging.GetAttributeValue<string>(LoanStaging.LOExternalID), service);
                if (userGuid != Guid.Empty)
                    targetEntity[Account.Owner] = new EntityReference(SystemUser.EntityName, userGuid);
                else
                {
                    if (manualImportReference != null && !targetEntity.Contains(Lead.Owner))
                    {
                        targetEntity[Lead.Owner] = manualImportReference;
                    }
                    else if (defaultTeamReference != null && !targetEntity.Contains(Lead.Owner))
                    {
                        //if (targetEntity.Contains(Lead.LOExternalId))
                        //    targetEntity.Attributes[Lead.LOExternalId] = null;
                        targetEntity[Lead.Owner] = defaultTeamReference;
                    }

                }
            }
            else if (!staging.Contains(LoanStaging.LOExternalID) && manualImportReference != null && !targetEntity.Contains(Lead.Owner))
            {
                targetEntity[Lead.Owner] = manualImportReference;
            }
            else if (!staging.Contains(LoanStaging.LOExternalID) && defaultTeamReference != null && !targetEntity.Contains(Lead.Owner))
            {
                //if (targetEntity.Contains(Lead.LOExternalId))
                //    targetEntity.Attributes[Lead.LOExternalId] = null;
                targetEntity[Lead.Owner] = defaultTeamReference;
            }
        }

        //This is to check the user is Loan Officer or not
        public bool CheckIsLoanOfficer(IOrganizationService service, Guid contextUserId, Dictionary<string, string> configDetails)
        {
            var xml = GetMessage(Constants.IsLoanOfficerCheck, configDetails);
            if (xml != null && xml.Contains("userGuid"))
            {
                xml = xml.Replace("userGuid", contextUserId.ToString());
                var result = service.RetrieveMultiple(new FetchExpression(string.Format(@"" + xml + "")));
                if (result.Entities.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }
        public bool CheckIsLoanOfficerAssistant(IOrganizationService service, Guid contextUserId, Dictionary<string, string> configDetails)
        {
            var xml = GetMessage(Constants.IsLoanOfficerAssistantCheck, configDetails);
            if (xml != null && xml.Contains("userGuid"))
            {
                xml = xml.Replace("userGuid", contextUserId.ToString());
                var result = service.RetrieveMultiple(new FetchExpression(string.Format(@"" + xml + "")));
                if (result.Entities.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }
        public bool CheckUserSecuirtyRoles(IOrganizationService service, Guid contextUserId, Dictionary<string, string> configDetails)
        {
            var xml = GetMessage(Constants.IsUserHaveSecuirtyRole, configDetails);
            if (xml != null && xml.Contains("{userGuid}"))
            {
                xml = xml.Replace("{userGuid}", contextUserId.ToString());
                var result = service.RetrieveMultiple(new FetchExpression(string.Format(@"" + xml + "")));
                if (result.Entities.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public OptionSetValue FetchLookupOptionSet(IOrganizationService service, Guid masterdataRecordGuid, string optionsetSchema, string entityName)
        {
            try
            {
                QueryExpression queryExpression = new QueryExpression(entityName);
                queryExpression.ColumnSet.AddColumn(optionsetSchema);
                queryExpression.Criteria.AddCondition(new ConditionExpression(entityName + "id", ConditionOperator.Equal, masterdataRecordGuid));
                var ec = service.RetrieveMultiple(queryExpression);
                if (ec.Entities.Count > 0)
                {
                    if (ec.Entities.FirstOrDefault().Contains(optionsetSchema))
                    {
                        return ec.Entities.FirstOrDefault().GetAttributeValue<OptionSetValue>(optionsetSchema);
                    }
                }

            }

            catch (Exception ex)
            {
                throw new Exception("Error while fetching lookupOption: " + ex.Message);
            }


            return null;
        }

        public bool CheckMovementDirectLeadUsingLeadSource(IOrganizationService service, EntityReference leadSource, Dictionary<string, string> configDetails)
        {
            var ec = new EntityCollection();
            var xmlLeadSource = GetMessage(Constants.MovementDirectLead_Identification_LeadSource, configDetails);
            //var xmlParentLeadSource = GetMessage(Constants.MovementDirectLead_Identification_ParentLeadSource, configDetails);
            if (!string.IsNullOrEmpty(xmlLeadSource) && xmlLeadSource.Contains("{ims_leadsourceguid}"))
            {
                xmlLeadSource = xmlLeadSource.Replace("ims_leadsourceguid", leadSource.Id.ToString());
                ec = service.RetrieveMultiple(new FetchExpression(xmlLeadSource));
                if (ec.Entities.Count > 0)
                    return true;
            }
            return false;
        }

        public bool CheckMovementDirectBusinessUnit(string xml, Guid userId, IOrganizationService service)
        {
            if (xml != null && xml.Contains("{userGuid}"))
            {
                xml = xml.Replace("{userGuid}", userId.ToString());
                var result = service.RetrieveMultiple(new FetchExpression(string.Format(@"" + xml + "")));
                if (result.Entities.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }
        public string GetMovementDirectDuplicateCheckLeadStatus(EntityReference leadStatusRef, IOrganizationService service, Dictionary<string, string> dcConfigDetails)
        {
            string statusGuids = string.Empty;
            var xml = GetMessage(Constants.LeadStatus_Active_MBAPreapproved, dcConfigDetails);
            EntityCollection ec = service.RetrieveMultiple(new FetchExpression(xml));
            if (ec.Entities.Count > 0)
            {
                foreach (var en in ec.Entities)
                {
                    statusGuids += ";" + en.Id.ToString();
                }
            }
            return statusGuids;
        }

        public Entity CheckMovementDirectDuplicateLead(Entity objEntity, IOrganizationService service, Dictionary<string, string> dcConfigDetails)
        {
            if (objEntity.Contains(Lead.OrginatedByMovementDirect) && objEntity.GetAttributeValue<bool>(Lead.OrginatedByMovementDirect))
            {
                if (objEntity.Contains(Lead.FirstName) && objEntity.Contains(Lead.LastName))
                {
                    //var xmlDuplicate = GetMessage(Constants.MovementDirectLeadDUplicateCheck, dcConfigDetails);
                    //if(xmlDuplicate.Contains("{firstname}") && xmlDuplicate.Contains("{lastname}") && xmlDuplicate.Contains("{mobilephone}") && xmlDuplicate.Contains("{emailaddress1}")){
                    //    xmlDuplicate= xmlDuplicate.Replace("{firstname}", objEntity.GetAttributeValue<string>(Lead.FirstName));
                    //    xmlDuplicate = xmlDuplicate.Replace("{lastname}", objEntity.GetAttributeValue<string>(Lead.LastName));
                    //    if (objEntity.Contains(Lead.MobilePhone))
                    //    {
                    //        xmlDuplicate = xmlDuplicate.Replace("{mobilephone}", objEntity.GetAttributeValue<string>(Lead.MobilePhone));
                    //    }
                    //    if (objEntity.Contains(Lead.PersonalEmail))
                    //    {
                    //        xmlDuplicate = xmlDuplicate.Replace("{emailaddress1}", objEntity.GetAttributeValue<string>(Lead.PersonalEmail));
                    //    }
                    //    EntityCollection ec = service.RetrieveMultiple(new FetchExpression(xmlDuplicate));
                    //    if (ec.Entities.Count > 0)
                    //    {
                    //        return ec.Entities.FirstOrDefault();
                    //    }
                    //}
                    if (objEntity.Contains(Lead.MobilePhone))
                    {
                        var xmlMobileNumber = GetMessage(Constants.MovementDirectLeadDuplicateCheck_MobileNumber, dcConfigDetails);
                        if (xmlMobileNumber.Contains("{firstname}") && xmlMobileNumber.Contains("{lastname}") && xmlMobileNumber.Contains("{mobilephone}"))
                        {
                            xmlMobileNumber = xmlMobileNumber.Replace("{firstname}", objEntity.GetAttributeValue<string>(Lead.FirstName));
                            xmlMobileNumber = xmlMobileNumber.Replace("{lastname}", objEntity.GetAttributeValue<string>(Lead.LastName));
                            xmlMobileNumber = xmlMobileNumber.Replace("{mobilephone}", objEntity.GetAttributeValue<string>(Lead.MobilePhone));
                            EntityCollection ec = service.RetrieveMultiple(new FetchExpression(xmlMobileNumber));
                            if (ec.Entities.Count > 0)
                            {
                                return ec.Entities.FirstOrDefault();
                            }
                        }

                    }
                    if (objEntity.Contains(Lead.PersonalEmail))
                    {
                        var xmlEMailAddress = GetMessage(Constants.MovementDirectLeadDuplicateCheck_EmailAddress, dcConfigDetails);
                        if (xmlEMailAddress.Contains("{firstname}") && xmlEMailAddress.Contains("{lastname}") && xmlEMailAddress.Contains("{emailaddress1}"))
                        {
                            xmlEMailAddress = xmlEMailAddress.Replace("{firstname}", objEntity.GetAttributeValue<string>(Lead.FirstName));
                            xmlEMailAddress = xmlEMailAddress.Replace("{lastname}", objEntity.GetAttributeValue<string>(Lead.LastName));
                            xmlEMailAddress = xmlEMailAddress.Replace("{emailaddress1}", objEntity.GetAttributeValue<string>(Lead.PersonalEmail));
                            EntityCollection ec = service.RetrieveMultiple(new FetchExpression(xmlEMailAddress));
                            if (ec.Entities.Count > 0)
                            {
                                return ec.Entities.FirstOrDefault();
                            }
                        }
                    }

                }
            }
            return null;
        }

        //Mix Image and Target Entity
        public Entity GetEntityMix(Entity targetEntity, Entity ImageEntity)
        {
            Entity entityMix = new Entity();
            foreach (var attr in targetEntity.Attributes)
            {
                if (ImageEntity.Attributes.Contains(attr.Key))
                {
                    ImageEntity.Attributes[attr.Key] = targetEntity.Attributes[attr.Key];
                }
            }
            return entityMix = ImageEntity;
        }

        public void SplitAnnualIncome(string annualIncome, Entity leadStaging)
        {
            string[] annualSplitValue = annualIncome.Split('-');
            string[] annualSplitValueOrder = annualSplitValue.OrderByDescending(val => val).ToArray();
            if (annualSplitValue.Count() > 0)
            {
                leadStaging.Attributes[LeadStaging.LeadAnnualIncome] = annualSplitValue[0].Replace("$", "");
                leadStaging.Attributes[LeadStaging.LeadAnnualIncome] = annualSplitValue[1].Replace("$", "");
                foreach (var val in annualSplitValue)
                {
                    if (val.Contains("$"))
                    {
                        leadStaging.Attributes[LeadStaging.LeadAnnualIncome] = val.Replace("$", "");
                    }
                }
            }
        }

        public string CreateZillowSellersAgent(Mapping mapping, ref bool ValidationStatus, ref bool canReturn, ref string errorMessage, Dictionary<string, string> dcConfigDetails, Entity staging, IOrganizationService systemService)
        {
            Entity objContact = new Entity(Contact.EntityName);
            string propertyName = string.Empty;
            Guid entityId = Guid.Empty;
            string importDataMaster = string.Empty;
            Guid importDataMasterId = Guid.Empty;
            EntityReference defaultTeam = null;
            List<Mapping> mappings = new List<Mapping>();
            //bool isLoanOfficer = CheckIsLoanOfficer(service, contextUserId, dcConfigDetails);
            //Get Import Data Master to fetch mappings
            if (!string.IsNullOrEmpty(mapping.DataMaster))
            {

                importDataMasterId = GetImportDataMasterAndDefaultTeam(mapping.DataMaster, ref defaultTeam, systemService);
                if (importDataMasterId == Guid.Empty)
                {
                    UpdateStagingLog(staging.Id, Guid.Empty, staging.LogicalName, "Import Data Master Configuration is missing for Lookup field " + mapping.CrmDisplayName + " is missing.", true, systemService);
                    return (entityId == Guid.Empty ? string.Empty : entityId.ToString());
                }
                else if (importDataMasterId != Guid.Empty)
                {
                    if (!FetchMappings(importDataMasterId, ref mappings, systemService, ref errorMessage))
                    {
                        UpdateStagingLog(staging.Id, Guid.Empty, staging.LogicalName, errorMessage, true, systemService);
                        return (entityId == Guid.Empty ? string.Empty : entityId.ToString());
                    }
                    else
                    {
                        foreach (Common.Mapping objMapping in mappings)
                        {
                            try
                            {
                                Common.Mapping mappingObject = objMapping;
                                GetValueFromSourceEntity(staging, ref mappingObject, ref ValidationStatus, ref canReturn, ref errorMessage);
                                if (!string.IsNullOrEmpty(mappingObject.value))
                                {
                                    SetValueToTargetEntity(ref objContact, mappingObject, systemService, ref ValidationStatus, ref canReturn, ref errorMessage, dcConfigDetails, staging);
                                }
                            }
                            catch (Exception ex)
                            {
                                //validationStatus-> False canReturn->true  Add log UpdateValidationMessage
                                UpdateValidationMessage("Error While FormTargetEntityObject for Contact " + ex.Message, ref errorMessage);
                            }
                        }
                        if (objContact.Attributes.Count > 0)
                        {
                            Entity contact = CheckPreviousOtherContact(false, staging, objContact, systemService, dcConfigDetails);
                            if (contact != null)
                            {
                               
                                objContact.Id = contact.Id;
                                systemService.Update(objContact);
                                entityId = objContact.Id;

                            }
                            else if (contact == null)
                            {
                                if (defaultTeam != null) objContact[Contact.LoanOfficer] = defaultTeam;
                                if (objContact.Contains(Contact.FirstName) || objContact.Contains(Contact.LastName))
                                    entityId = systemService.Create(objContact);
                            }
                        }
                    }
                }
            }
            return (entityId == Guid.Empty ? string.Empty : entityId.ToString());
        }

        public string GetLOExternalIdUsingEmailId(string loEmail, IOrganizationService service)
        {
            QueryByAttribute byAttribute = new QueryByAttribute(SystemUser.EntityName);
            byAttribute.ColumnSet = new ColumnSet(SystemUser.LOExternalId);
            byAttribute.AddAttributeValue(SystemUser.DomainName, loEmail);
            EntityCollection entityCollection = service.RetrieveMultiple(byAttribute);
            if (entityCollection.Entities.Count > 0)
            {
                foreach (Entity en in entityCollection.Entities)
                {
                    if (en.Contains(SystemUser.LOExternalId))
                        return en.GetAttributeValue<string>(SystemUser.LOExternalId);
                }
            }
            return null;
        }

        public void shareRecordWithLOA(IOrganizationService service, EntityReference user, EntityReference record)
        {
            var grantAccessRequest = new GrantAccessRequest
            {
                PrincipalAccess = new PrincipalAccess
                {
                    AccessMask = AccessRights.ReadAccess | AccessRights.WriteAccess,
                    Principal = user
                },
                Target = record
            };
            service.Execute(grantAccessRequest);
        }

        public void removeAccess(IOrganizationService service, EntityReference user, EntityReference record)
        {
            var revokeTeamAccessReq = new RevokeAccessRequest
            {
                Revokee = user,
                Target = record
            };
            service.Execute(revokeTeamAccessReq);
        }

        /// <summary>
        /// System User Role Check
        /// </summary>
        /// <param name="service"></param>
        /// <param name="dcConfigDetails"></param>
        public bool SystemuserMovementDirectRoleCheck(string userGuid, IOrganizationService service, Dictionary<string, string> dcConfigDetails)
        {
            var mdSecurityRole = GetMessage(Constants.MovementDirectSecurityRole, dcConfigDetails);
            if (!string.IsNullOrEmpty(mdSecurityRole))
            {
                var matchRole = MatchMovementDirectSecurityRole(service, userGuid, mdSecurityRole);
                return matchRole;
            }
            return false;
        }
        public bool SystemuserValidation(string userGuid, IOrganizationService service, Dictionary<string, string> dcConfigDetails)
        {
            //var mdSecurityRole = GetMessage(Constants.MovementDirectSecurityRole, dcConfigDetails);
            //if (!string.IsNullOrEmpty(mdSecurityRole))
            //{
            //    var matchRole = MatchMovementDirectSecurityRole(service, userGuid, mdSecurityRole);
            //    return matchRole;
            //}
            //return false;

            var userIsValid = service.Retrieve(SystemUser.EntityName, new Guid(userGuid), new ColumnSet(SystemUser.LOExternalId));
            if (userIsValid != null)
            {
                if (userIsValid.Contains(SystemUser.LOExternalId))
                {
                    return true;
                }
            }
            return false;
        }
        public bool IdentifyMovementDirectLead(string loExternalId, IOrganizationService service, Dictionary<string, string> dcConfigDetails)
        {
            var systemUserGuid = GetSystemuserGuid(loExternalId, service);
            if(!string.IsNullOrEmpty(systemUserGuid))
            {
                return SystemuserMovementDirectRoleCheck(systemUserGuid, service, dcConfigDetails);
            }
            return false;
        }
        public void ShareRecordsMDLoanOfficerExternalId(ref string errorMessage, EntityReference entityReference, string loExternalId, IOrganizationService service, Dictionary<string, string> dcConfigDetails)
        {
            if (!loExternalId.Equals("0", StringComparison.OrdinalIgnoreCase))
            {
                var systemUserGuid = GetSystemuserGuid(loExternalId, service);
                if (!string.IsNullOrEmpty(systemUserGuid))
                {
                    shareRecordWithLOA(service, new EntityReference(SystemUser.EntityName, new Guid(systemUserGuid)), entityReference);
                }
                else
                    errorMessage = "Info:MD Loan Officer is Not valid, Not able to share the record \n";
            }
            else
                errorMessage = "Info:MD Loan Officer is Not valid, Not able to share the record \n";
        }
        public string GetSystemuserGuid(string loExternalId, IOrganizationService service)
        {
            QueryByAttribute byAttribute = new QueryByAttribute(SystemUser.EntityName);
            byAttribute.ColumnSet = new ColumnSet(SystemUser.EntityId);
            byAttribute.AddAttributeValue(SystemUser.LOExternalId, loExternalId);
            EntityCollection entityCollection = service.RetrieveMultiple(byAttribute);
            if (entityCollection.Entities.Count > 0)
            {

                return entityCollection.Entities.FirstOrDefault().Id.ToString();
                
            }
            return null;
        }
        public bool MatchMovementDirectSecurityRole(IOrganizationService service, string userGuid, string securityRoleName)
        {
            if (!string.IsNullOrEmpty(userGuid))
            {
                string xml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>" +
                              "<entity name='role'>" +
                                "<attribute name='name' />" +
                                "<attribute name='businessunitid' />" +
                                "<attribute name='roleid' />" +
                                "<order attribute='name' descending='false' />" +
                                "<link-entity name='systemuserroles' from='roleid' to='roleid' visible='false' intersect='true'>" +
                                  "<link-entity name='systemuser' from='systemuserid' to='systemuserid' alias='ab'>" +
                                    "<filter type='and'>" +
                                      "<condition attribute='systemuserid' operator='eq' uitype='systemuser' value='" + userGuid + "' />" +
                                    "</filter>" +
                                  "</link-entity>" +
                                "</link-entity>" +
                              "</entity>" +
                            "</fetch>";
                FetchExpression fetchExpression = new FetchExpression(xml);
                EntityCollection ec = service.RetrieveMultiple(fetchExpression);
                //List<Entity> ec = service.RetrieveMultipleByFetchXml(xml);
                if (ec.Entities.Count > 0)
                {
                    var role = ec.Entities.Where(securityRole => securityRole["name"].Equals(securityRoleName));

                    if (role != null && role.Count() > 0)
                    {
                        return true;
                    }
                    else
                    {
                        var adminRole = ec.Entities.Where(securityRole => securityRole["name"].Equals("Movement Direct Admin"));
                        if (adminRole != null && adminRole.Count() > 0)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            return false;
        }

        public void BuildTags(IOrganizationService service,string tags,Guid stagingOwnerGuid, Guid recordGuid,string targetEntity,ref string errorMessage)
        {
            if (!string.IsNullOrEmpty(tags))
            {
                List<Tags> tagsList = new List<Tags>();
                var tagSplit = tags.Split('$');
                var consoldatedTagConst = "tagname" + ':' + "~{tagname}~" + ',' + "tagid" + ':' + "{tagid}" + ',' + "userId" + ':' + "{userId}";
                var tagsJson = string.Empty;
                foreach (var tag in tagSplit)
                {
                    Tags tagsObj = new Tags();
                    var tagReference = CheckTagExistance(service, tag, stagingOwnerGuid);
                    if (targetEntity == Contact.EntityName)
                        AssociateTags(service, new Relationship("ims_contact_ims_tags"), new EntityReference(targetEntity, recordGuid), tagReference);
                    else if (targetEntity == Lead.EntityName)
                        AssociateTags(service, new Relationship("ims_lead_ims_tags"), new EntityReference(targetEntity, recordGuid), tagReference);
                    tagsObj.tagname = "~" + tag + "~";
                    tagsObj.tagid = tagReference.Id.ToString();
                    tagsObj.userId = stagingOwnerGuid.ToString();
                    tagsList.Add(tagsObj);
                    //var json = JsonConvert.SerializeObject(tagsObj);
                    //var tagConst ="{"+consoldatedTagConst+"}";
                    //var tagJson = tagConst.Replace("{tagname}", tag).Replace("{tagid}", tagReference.Id.ToString()).Replace("{userId}", stagingOwnerGuid.ToString());
                    //tagsJson += tagJson;
                }
                Entity consolidatedTags = new Entity(targetEntity);
                consolidatedTags.Id = recordGuid;
                consolidatedTags.Attributes["ims_consolidatedtags"] = JsonConvert.SerializeObject(tagsList);//  "[" + tagsJson + "]";
                service.Update(consolidatedTags);
                errorMessage += "Info:Tags has been associated for the lead/contacts \n";
            }
        }
        public EntityReference CheckTagExistance(IOrganizationService service,string tag,Guid stagingOwnerGuid)
        {
            QueryExpression qryExp = new QueryExpression("ims_tags");
            qryExp.Criteria.AddCondition(new ConditionExpression("ims_name", ConditionOperator.Equal,tag));
            qryExp.Criteria.AddCondition(new ConditionExpression("ims_user", ConditionOperator.Equal, stagingOwnerGuid));
            var tagQryResult = service.RetrieveMultiple(qryExp);
            if (tagQryResult.Entities.Count > 0)
                return tagQryResult.Entities.FirstOrDefault().ToEntityReference();
            else
            {
                KeyAttributeCollection keyValues = new KeyAttributeCollection();
                keyValues.Add("ims_name", tag);
                keyValues.Add("ims_user", new EntityReference(SystemUser.EntityName, stagingOwnerGuid));
                Entity tagEntity = new Entity("ims_tags");//,keyValues);
                tagEntity.Attributes["ims_name"] = tag;
                tagEntity.Attributes["ims_user"] = new EntityReference(SystemUser.EntityName, stagingOwnerGuid);
                var tagGuid=service.Create(tagEntity);
                return new EntityReference("ims_tags", tagGuid);
            }

        }
        public void AssociateTags(IOrganizationService service,Relationship relationship,EntityReference entityReference,EntityReference tagReferance)
        {
            var entityReferences = new EntityReferenceCollection();
            entityReferences.Add(entityReference);
            AssociateRequest request = new AssociateRequest()
            {
                RelatedEntities = entityReferences,
                Relationship = relationship,
                Target = tagReferance
            };
            service.Execute(request);
        }
    }

    public class Tags
    {
        public string tagname { get; set; }
        public string tagid { get; set; }

        public string userId { get; set; }
    }
}
