using System;
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
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using System.Xml;
using System.Text;
using System.Linq;

namespace Infy.MS.Plugins
{
    public class FetchXML2SQLDynamicML : BasePlugin
    {
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            Entity list = null;
            if (context == null)
            {
                throw new InvalidPluginExecutionException("Context not found");
            }
            try
            {
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    list = (Entity)context.InputParameters["Target"];
                }
                if (list != null)
                {
                    context.Trace("Target Found");
                    Entity objList = context.SystemOrganizationService.Retrieve(Marketinglist.EntityName, list.Id, new ColumnSet(Marketinglist.Query,Marketinglist.Type));
                    context.Trace("FetchXML :" + objList.GetAttributeValue<string>(Marketinglist.Query));
                    if (objList.Attributes.Contains(Marketinglist.Type) && objList[Marketinglist.Type] != null && (bool)objList[Marketinglist.Type] == true)
                    {
                        if (objList.Attributes.Contains(Marketinglist.Query) && objList[Marketinglist.Query] != null)
                        {
                            string fetchXMLData = objList.GetAttributeValue<string>(Marketinglist.Query);
                            if (!string.IsNullOrEmpty(fetchXMLData))
                            {
                                string sqlData = string.Empty;
                                sqlData = GetSQLFromFetchXML(fetchXMLData, context.SystemOrganizationService, context);
                                if (!string.IsNullOrEmpty(sqlData))
                                {
                                    context.Trace("Sql Data Query Generated" + sqlData);
                                    Entity objMList = new Entity(Marketinglist.EntityName);
                                    objMList.Id = list.Id;
                                    if (!sqlData.Contains("TBD"))
                                    {
                                        objMList[Marketinglist.Configuration] = sqlData;
                                        context.SystemOrganizationService.Update(objMList);
                                        context.Trace("SQLQuery Updated to ML");
                                    }
                                    else
                                    {
                                        throw new Exception("Fetch XML Operator not supported!");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Entity objMList = new Entity(Marketinglist.EntityName);
                objMList.Id = list.Id;
                objMList[Marketinglist.Configuration] = null;
                context.SystemOrganizationService.Update(objMList);
                Common objCommon = new Common();
                objCommon.CreateErrorLog("FetchXML2SQL ERROR:" + list.Id.ToString(), ex.Message, context.SystemOrganizationService);
            }
        }


        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Asynchronous, "Create");
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Asynchronous, "Update");
        }
        public string entityName;
        public string linkEntityName;
        public List<string> entityAttributes = new List<string>();

        public struct linkEntitiesAndAttributes
        {
            public string linkEntityData { get; private set; }
            public string linkEntityAttributeData { get; private set; }
            public linkEntitiesAndAttributes(string linkEntityValue, string linkEntityAttributeValue) : this()
            {
                linkEntityData = linkEntityValue;
                linkEntityAttributeData = linkEntityAttributeValue;
            }
        }

        public List<linkEntitiesAndAttributes> linkEntitiesAndAttributesList = new List<linkEntitiesAndAttributes>();
        public List<linkEntitiesFromTo> linkEntitiesFromToList = new List<linkEntitiesFromTo>();
        public List<linkEntitiesFilters> linkEntitiesFiltersList = new List<linkEntitiesFilters>();
        public List<mainFilters> mainFiltersList = new List<mainFilters>();
        public Dictionary<string, string> operatorReplacement = new Dictionary<string, string>();
        public string convertOperator;
        public struct linkEntitiesFromTo
        {
            public string linkEntityFromToName { get; private set; }
            public string linkEntityFrom { get; private set; }
            public string linkEntityTo { get; private set; }
            public string linkEntityJoinType { get; private set; }
            public linkEntitiesFromTo(string linkEntityValueforFromTo, string linkEntityFromValue, string linkEntityToValue, string linkEntityJoinTypeValue) : this()
            {
                linkEntityFromToName = linkEntityValueforFromTo;
                linkEntityFrom = linkEntityFromValue;
                linkEntityTo = linkEntityToValue;
                linkEntityJoinType = linkEntityJoinTypeValue;
            }
        }
        public struct linkEntitiesFilters
        {
            public string linkEntitiesFiltersLinkEntityName { get; private set; }
            public string linkEntitiesFiltersFilterType { get; private set; }
            public string linkEntitiesFiltersLinkEntityAttribute { get; private set; }
            public string linkEntitiesFiltersLinkEntityOperator { get; private set; }
            public string linkEntitiesFiltersLinkEntityValue { get; private set; }
            public linkEntitiesFilters(string linkEntitiesFiltersLinkEntityNameValue, string linkEntitiesFiltersFilterTypeValue, string linkEntitiesFiltersLinkEntityAttributeValue, string linkEntitiesFiltersLinkEntityOperatorValue, string linkEntitiesFiltersLinkEntityValueValue) : this()
            {
                linkEntitiesFiltersLinkEntityName = linkEntitiesFiltersLinkEntityNameValue;
                linkEntitiesFiltersFilterType = linkEntitiesFiltersFilterTypeValue;
                linkEntitiesFiltersLinkEntityAttribute = linkEntitiesFiltersLinkEntityAttributeValue;
                linkEntitiesFiltersLinkEntityOperator = linkEntitiesFiltersLinkEntityOperatorValue;
                linkEntitiesFiltersLinkEntityValue = linkEntitiesFiltersLinkEntityValueValue;
            }
        }
        public struct mainFilters
        {
            public string mainFiltersEntityName { get; private set; }
            public string mainFiltersFilterType { get; private set; }
            public string mainFiltersEntityAttribute { get; private set; }
            public string mainFiltersEntityOperator { get; private set; }
            public string mainFiltersEntityValue { get; private set; }
            public mainFilters(string mainFiltersEntityNameValue, string mainFiltersFilterTypeValue, string mainFiltersEntityAttributeValue, string mainFiltersEntityOperatorValue, string mainFiltersEntityValueValue) : this()
            {
                mainFiltersEntityName = mainFiltersEntityNameValue;
                mainFiltersFilterType = mainFiltersFilterTypeValue;
                mainFiltersEntityAttribute = mainFiltersEntityAttributeValue;
                mainFiltersEntityOperator = mainFiltersEntityOperatorValue;
                mainFiltersEntityValue = mainFiltersEntityValueValue;
            }
        }
        public string GetSQLFromFetchXML(string fetchXMLData, IOrganizationService service, IExtendedPluginContext context)
        {
            string sqlData = string.Empty;
            operatorReplacement.Add("eq", "= ");
            operatorReplacement.Add("neq", "!= ");
            operatorReplacement.Add("ne", "!= ");
            operatorReplacement.Add("gt", "> ");
            operatorReplacement.Add("ge", ">= ");
            operatorReplacement.Add("le", "<= ");
            operatorReplacement.Add("lt", "< ");
            operatorReplacement.Add("like", "LIKE ");
            operatorReplacement.Add("not-like", "NOT LIKE ");
            operatorReplacement.Add("null", "NULL ");
            operatorReplacement.Add("not-null", "IS NOT NULL ");
            operatorReplacement.Add("yesterday", "= CONVERT(DATE,DATEADD(DAY,-1,GETDATE()))");
            operatorReplacement.Add("today", "= CONVERT(DATE, GETDATE())");
            operatorReplacement.Add("tomorrow", "= CONVERT(DATE,DATEADD(DAY,1,GETDATE()))");
            operatorReplacement.Add("on", "= ");
            operatorReplacement.Add("on-or-before", "<= ");
            operatorReplacement.Add("on-or-after", ">= ");
            operatorReplacement.Add("eq-userid", "= ");
            operatorReplacement.Add("ne-userid", "!= ");
            operatorReplacement.Add("begins-with", "= %");
            operatorReplacement.Add("not-begin-with", "!= %");
            operatorReplacement.Add("ends-with", "= ");
            operatorReplacement.Add("not-end-with", "!= ");
            operatorReplacement.Add("in", "IN ");
            operatorReplacement.Add("not-in", "NOT IN ");


            operatorReplacement.Add("between", "TBD");
            operatorReplacement.Add("not-between", "TBD");
            operatorReplacement.Add("last-seven-days", "TBD");
            operatorReplacement.Add("next-seven-days", "TBD");
            operatorReplacement.Add("last-week", "TBD");
            operatorReplacement.Add("this-week", "TBD");
            operatorReplacement.Add("next-week", "TBD");
            operatorReplacement.Add("last-month", "TBD");
            operatorReplacement.Add("this-month", "TBD");
            operatorReplacement.Add("next-month", "TBD");
            operatorReplacement.Add("last-year", "TBD");
            operatorReplacement.Add("this-year", "TBD");
            operatorReplacement.Add("next-year", "TBD");
            operatorReplacement.Add("last-x-hours", "TBD");
            operatorReplacement.Add("next-x-hours", "TBD");
            operatorReplacement.Add("last-x-days", "TBD");
            operatorReplacement.Add("next-x-days", "TBD");
            operatorReplacement.Add("last-x-weeks", "TBD");
            operatorReplacement.Add("next-x-weeks", "TBD");
            operatorReplacement.Add("last-x-months", "TBD");
            operatorReplacement.Add("next-x-months", "TBD");
            operatorReplacement.Add("olderthan-x-months", "TBD");
            operatorReplacement.Add("olderthan-x-years", "TBD");
            operatorReplacement.Add("olderthan-x-weeks", "TBD");
            operatorReplacement.Add("olderthan-x-days", "TBD");
            operatorReplacement.Add("olderthan-x-hours", "TBD");
            operatorReplacement.Add("olderthan-x-minutes", "TBD");
            operatorReplacement.Add("last-x-years", "TBD");
            operatorReplacement.Add("next-x-years", "TBD");
            operatorReplacement.Add("eq-userteams", "TBD");
            operatorReplacement.Add("eq-useroruserteams", "TBD");
            operatorReplacement.Add("eq-useroruserhierarchy", "TBD");
            operatorReplacement.Add("eq-useroruserhierarchyandteams", "TBD");
            operatorReplacement.Add("eq-businessid", "TBD");
            operatorReplacement.Add("ne-businessid", "TBD");
            operatorReplacement.Add("eq-userlanguage", "TBD");
            operatorReplacement.Add("this-fiscal-year", "TBD");
            operatorReplacement.Add("this-fiscal-period", "TBD");
            operatorReplacement.Add("next-fiscal-year", "TBD");
            operatorReplacement.Add("next-fiscal-period", "TBD");
            operatorReplacement.Add("last-fiscal-year", "TBD");
            operatorReplacement.Add("last-fiscal-period", "TBD");
            operatorReplacement.Add("last-x-fiscal-years", "TBD");
            operatorReplacement.Add("last-x-fiscal-periods", "TBD");
            operatorReplacement.Add("next-x-fiscal-years", "TBD");
            operatorReplacement.Add("next-x-fiscal-periods", "TBD");
            operatorReplacement.Add("in-fiscal-year", "TBD");
            operatorReplacement.Add("in-fiscal-period", "TBD");
            operatorReplacement.Add("in-fiscal-period-and-year", "TBD");
            operatorReplacement.Add("in-or-before-fiscal-period-and-year", "TBD");
            operatorReplacement.Add("in-or-after-fiscal-period-and-year", "TBD");
            operatorReplacement.Add("under", "TBD");
            operatorReplacement.Add("eq-or-under", "TBD");
            operatorReplacement.Add("not-under", "TBD");
            operatorReplacement.Add("above", "TBD");
            operatorReplacement.Add("eq-or-above", "TBD");

            //Load the fetchXML
            if (fetchXMLData.TrimStart().StartsWith("<") == false)
            {
                throw new Exception("Please enter some fetchXML! Incorrect FetchXML");
            }

            fetchXMLData = fetchXMLData.Replace("\\n", "").Replace("\\", "");
            XmlDocument xmldoc = new XmlDocument();
            try
            {
                byte[] byteArray = Encoding.ASCII.GetBytes(fetchXMLData);
                MemoryStream stream = new MemoryStream(byteArray);
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    xmldoc.Load(reader);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Looks like this is not a valid fetchXML! Please Check " + "Error:" + ex.Message);
            }
            entityName = "";
            linkEntityName = "";
            entityAttributes.Clear();
            linkEntitiesAndAttributesList.Clear();
            linkEntitiesFromToList.Clear();
            linkEntitiesFiltersList.Clear();
            mainFiltersList.Clear();

            //Get the main entity from fetchXML
            foreach (XmlNode entityNode in xmldoc.GetElementsByTagName("entity"))
            {
                entityName = entityNode.Attributes["name"].InnerText;
            }

            //get all the attributes for the main entity and put them in a list.
            foreach (XmlNode entityAttributeNode in xmldoc.DocumentElement.SelectNodes("/fetch/entity/attribute"))
            {
                entityAttributes.Add(entityAttributeNode.Attributes["name"].InnerText);
            }

            //Get all link entities and their attributes
            foreach (XmlNode linkEntity in xmldoc.DocumentElement.SelectNodes("/fetch/entity/link-entity"))
            {
                //check if there are any attributes for the link entity
                if (linkEntity.HasChildNodes)
                {
                    for (int i = 0; i < linkEntity.ChildNodes.Count; i++)
                    {
                        if (linkEntity.ChildNodes[i].Attributes["name"] != null)
                        {
                            linkEntitiesAndAttributesList.Add(new linkEntitiesAndAttributes(linkEntity.Attributes["name"].InnerText, linkEntity.ChildNodes[i].Attributes["name"].InnerText));
                        }
                    }
                }
                //Get Link Entity Name
                if (linkEntity.Attributes["name"] != null)
                {
                    linkEntityName = linkEntity.Attributes["name"].InnerText;
                }

                //get from and to fields for the joins
                if (linkEntity.Attributes["from"] != null && linkEntity.Attributes["to"] != null)
                {
                    string getLinkType;
                    if (linkEntity.Attributes["link-type"] != null)
                    {
                        getLinkType = linkEntity.Attributes["link-type"].InnerText;
                        if (getLinkType.ToUpper() != "INNER")
                            getLinkType = "LEFT " + getLinkType.ToUpper();
                    }
                    else
                    {
                        getLinkType = "INNER";
                    }
                    linkEntitiesFromToList.Add(new linkEntitiesFromTo(linkEntity.Attributes["name"].InnerText, linkEntity.Attributes["from"].InnerText, linkEntity.Attributes["to"].InnerText, getLinkType));
                }
            }

            //get all link entities and their filters
            foreach (XmlNode linkEntityFilter in xmldoc.DocumentElement.SelectNodes("/fetch/entity/link-entity/filter"))
            {
                if (linkEntityFilter.HasChildNodes)
                {
                    for (int i = 0; i < linkEntityFilter.ChildNodes.Count; i++)
                    {
                        //check if there are multiple filters
                        if (linkEntityFilter.ChildNodes[i].Name == "filter")
                        {
                            throw new Exception("Filters within filters are currently not supported!");
                        }

                        if (linkEntityFilter.ChildNodes[i].Attributes["operator"] != null)
                        {
                            operatorReplacement.TryGetValue(linkEntityFilter.ChildNodes[i].Attributes["operator"].InnerText, out convertOperator);
                        }
                        if (linkEntityFilter.ChildNodes[i].Attributes["value"] != null)
                        {
                            string attributeType = GetDataTypeFromFieldSchemaName(linkEntityName, linkEntityFilter.ChildNodes[i].Attributes["attribute"].InnerText, service).ToString();
                            linkEntitiesFiltersList.Add(new linkEntitiesFilters(linkEntityFilter.ParentNode.Attributes["name"].InnerText, linkEntityFilter.Attributes["type"].InnerText.ToUpper(), linkEntityFilter.ChildNodes[i].Attributes["attribute"].InnerText, convertOperator, (attributeType == "Lookup" || attributeType == "Uniqueidentifier") ? linkEntityFilter.ChildNodes[i].Attributes["value"].InnerText.Replace("{", "").Replace("}", "") : linkEntityFilter.ChildNodes[i].Attributes["value"].InnerText));
                        }
                        else
                            linkEntitiesFiltersList.Add(new linkEntitiesFilters(linkEntityFilter.ParentNode.Attributes["name"].InnerText, linkEntityFilter.Attributes["type"].InnerText.ToUpper(), linkEntityFilter.ChildNodes[i].Attributes["attribute"].InnerText, convertOperator, ""));
                    }
                }
            }

            //get all filters for main entity
            foreach (XmlNode mainFilter in xmldoc.DocumentElement.SelectNodes("/fetch/entity/filter"))
            {
                if (mainFilter.HasChildNodes)
                {
                    for (int i = 0; i < mainFilter.ChildNodes.Count; i++)
                    {
                        if (mainFilter.ChildNodes[i].Attributes["operator"] != null)
                        {
                            operatorReplacement.TryGetValue(mainFilter.ChildNodes[i].Attributes["operator"].InnerText, out convertOperator);
                            context.Trace("Attributes[operator] " + convertOperator);
                        }
                        if (mainFilter.ChildNodes[i].Attributes["operator"].InnerText == "in" || mainFilter.ChildNodes[i].Attributes["operator"].InnerText == "not-in")
                        {
                            string InValue = string.Empty;
                            if (mainFilter.ChildNodes[i].InnerText.Contains("{") || mainFilter.ChildNodes[i].InnerText.Contains("}"))
                            {
                                InValue = mainFilter.ChildNodes[i].InnerText.Replace("}{", "','").Replace("{", "('").Replace("}", "')");
                            }
                            else
                            {
                                List<string> multipleValueList = new List<string>();
                                foreach (XmlElement val in mainFilter.ChildNodes[i].ChildNodes)
                                {
                                    multipleValueList.Add(val.InnerText);
                                }
                                string formattedMultipleValue = string.Join(",", multipleValueList.ToArray());
                                InValue = "(" + formattedMultipleValue + ")";
                            }
                            context.Trace("InValue" + InValue);
                            string attributeType = GetDataTypeFromFieldSchemaName(entityName, mainFilter.ChildNodes[i].Attributes["attribute"].InnerText, service).ToString();
                            mainFiltersList.Add(new mainFilters(mainFilter.ParentNode.Attributes["name"].InnerText,
                                mainFilter.Attributes["type"].InnerText.ToUpper(),
                                mainFilter.ChildNodes[i].Attributes["attribute"].InnerText,
                                convertOperator, InValue));
                        }
                        else if (mainFilter.ChildNodes[i].Attributes["value"] != null)
                        {
                            string attributeType = GetDataTypeFromFieldSchemaName(entityName, mainFilter.ChildNodes[i].Attributes["attribute"].InnerText, service).ToString();
                            mainFiltersList.Add(new mainFilters(mainFilter.ParentNode.Attributes["name"].InnerText, mainFilter.Attributes["type"].InnerText.ToUpper(), mainFilter.ChildNodes[i].Attributes["attribute"].InnerText, convertOperator, (attributeType == "Lookup" || attributeType == "Uniqueidentifier") ? mainFilter.ChildNodes[i].Attributes["value"].InnerText.Replace("{", "").Replace("}", "") : mainFilter.ChildNodes[i].Attributes["value"].InnerText));
                        }
                        else
                        {
                            mainFiltersList.Add(new mainFilters(mainFilter.ParentNode.Attributes["name"].InnerText, mainFilter.Attributes["type"].InnerText.ToUpper(), mainFilter.ChildNodes[i].Attributes["attribute"].InnerText, convertOperator, ""));
                        }
                    }
                }
            }
            sqlData += "FROM ";
            sqlData += entityName;

            foreach (linkEntitiesFromTo myStruct in linkEntitiesFromToList)
            {
                sqlData += Environment.NewLine;
                sqlData += myStruct.linkEntityJoinType + " JOIN ";
                sqlData += myStruct.linkEntityFromToName;
                sqlData += " ON ";
                string tempString = entityName + "." + myStruct.linkEntityTo + " = " + myStruct.linkEntityFromToName + "." + myStruct.linkEntityFrom;
                sqlData += tempString;
                bool writtenANDOnce;
                writtenANDOnce = false;
                foreach (linkEntitiesFilters myFilterstruct in linkEntitiesFiltersList)
                {
                    if (myStruct.linkEntityFromToName == myFilterstruct.linkEntitiesFiltersLinkEntityName)
                    {
                        if (writtenANDOnce == false)
                        {
                            sqlData += " AND ";
                            writtenANDOnce = true;
                        }
                        string attributeType = GetDataTypeFromFieldSchemaName(linkEntityName, myFilterstruct.linkEntitiesFiltersLinkEntityAttribute, service).ToString();
                        string value = string.Empty;
                        if (attributeType == "Picklist" ||
                            attributeType == "Integer" ||
                            attributeType == "Money" ||
                            attributeType == "Decimal" ||
                            attributeType == "Boolean" ||
                            attributeType == "State" ||
                            attributeType == "Status")
                        {
                            value = myFilterstruct.linkEntitiesFiltersLinkEntityValue;
                        }
                        else
                        {
                            value = "'" + myFilterstruct.linkEntitiesFiltersLinkEntityValue + "' ";
                        }
                        sqlData += myFilterstruct.linkEntitiesFiltersLinkEntityName + "." + myFilterstruct.linkEntitiesFiltersLinkEntityAttribute + " " + myFilterstruct.linkEntitiesFiltersLinkEntityOperator + value;
                        var lastItem = linkEntitiesFiltersList.Last();
                        if (writtenANDOnce == true && (myFilterstruct.linkEntitiesFiltersLinkEntityValue != lastItem.linkEntitiesFiltersLinkEntityValue || myFilterstruct.linkEntitiesFiltersLinkEntityAttribute != lastItem.linkEntitiesFiltersLinkEntityAttribute || myFilterstruct.linkEntitiesFiltersLinkEntityName != lastItem.linkEntitiesFiltersLinkEntityName))
                        {
                            sqlData += " " + myFilterstruct.linkEntitiesFiltersFilterType + " ";
                        }
                    }
                }
            }

            if (mainFiltersList.Count != 0)
            {
                sqlData += Environment.NewLine;
                sqlData += "WHERE ";
            }

            context.Trace(mainFiltersList.Count.ToString());
            foreach (mainFilters myMainFilter in mainFiltersList)
            {
                var lastItem = mainFiltersList.Last();
                if (myMainFilter.mainFiltersEntityValue == "")
                {
                    sqlData += myMainFilter.mainFiltersEntityName + "." + myMainFilter.mainFiltersEntityAttribute + " " + myMainFilter.mainFiltersEntityOperator;
                }
                else
                {
                    string attributeType = GetDataTypeFromFieldSchemaName(entityName, myMainFilter.mainFiltersEntityAttribute, service).ToString();
                    string value = string.Empty;
                    context.Trace("attributeType :" + attributeType);
                    context.Trace("myMainFilter.mainFiltersEntityValue :" + myMainFilter.mainFiltersEntityValue);
                    if (attributeType == "Picklist" ||
                        attributeType == "Integer" ||
                        attributeType == "Money" ||
                        attributeType == "Decimal" ||
                        attributeType == "Boolean" ||
                        attributeType == "State" ||
                        attributeType == "Status" ||
                        myMainFilter.mainFiltersEntityValue.Contains(","))
                    {
                        value = myMainFilter.mainFiltersEntityValue;
                    }
                    else
                    {
                        value = "'" + myMainFilter.mainFiltersEntityValue + "' ";
                    }
                    sqlData += myMainFilter.mainFiltersEntityName + "." + myMainFilter.mainFiltersEntityAttribute + " " + myMainFilter.mainFiltersEntityOperator + value;
                }
                if (myMainFilter.mainFiltersEntityAttribute != lastItem.mainFiltersEntityAttribute || myMainFilter.mainFiltersEntityOperator != lastItem.mainFiltersEntityOperator || myMainFilter.mainFiltersEntityValue != lastItem.mainFiltersEntityValue)
                {
                    sqlData += " " + myMainFilter.mainFiltersFilterType + " ";
                }
            }
            return sqlData;
        }

        public static object GetDataTypeFromFieldSchemaName(string entityName, string fieldName, IOrganizationService service)
        {
            RetrieveAttributeRequest attributeRequest = new RetrieveAttributeRequest
            {
                EntityLogicalName = entityName,
                LogicalName = fieldName,
                RetrieveAsIfPublished = false
            };
            RetrieveAttributeResponse attributeResponse =
                (RetrieveAttributeResponse)service.Execute(attributeRequest);
            return attributeResponse.AttributeMetadata.AttributeType.Value;
        }
    }
}
