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
using System.Runtime.Serialization;
using Microsoft.Xrm.Sdk.Messages;
using System.IO;
using System.Runtime.Serialization.Json;

namespace Infy.MS.Plugins.LeadPostCreateUpdateGenerateScore
{
    public class LeadPostCreateUpdateGenerateScore : BasePlugin
    {
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            if (context == null)
            {
                throw new InvalidPluginExecutionException("Context not found");
            }

            IOrganizationService service = context.SystemOrganizationService;
            Microsoft.Xrm.Sdk.Entity lead = null;
            Microsoft.Xrm.Sdk.Entity Preimage = null;
            int existingLeadScore = -1;
            int score = 0;

            //if (context.MessageName.ToLower() == "update" && context.Depth > 1)
            //    return;

            if (context.PostEntityImages.Contains("PostImage") && context.PostEntityImages["PostImage"] is Microsoft.Xrm.Sdk.Entity)
            {
                lead = (Microsoft.Xrm.Sdk.Entity)context.PostEntityImages["PostImage"];
            }

            if (context.PreEntityImages.Contains("Preimage"))
            {
                Preimage = context.PreEntityImages["Preimage"];
                if (Preimage != null)
                {
                    if (Preimage.Contains(Lead.LeadScore))
                    {
                        existingLeadScore = Preimage.GetAttributeValue<int>(Lead.LeadScore);
                    }
                }
            }

            if (existingLeadScore == -1) existingLeadScore = 0;


            EntityCollection ecLeadScoreModel = GetLeadScoringModelDefinition(service);
            if (lead != null)
            {
                if (ecLeadScoreModel != null && ecLeadScoreModel.Entities.Count > 0)
                {
                    int leadScore = 0;
                    foreach (Microsoft.Xrm.Sdk.Entity leadScoreModel in ecLeadScoreModel.Entities)
                    {
                        string modelDef = string.Empty;
                        if (leadScoreModel != null)
                        {
                            if (leadScoreModel.Attributes.Contains("msdyncrm_modeldefinition"))
                            {
                                modelDef = leadScoreModel.GetAttributeValue<string>("msdyncrm_modeldefinition");
                            }
                        }

                        if (!string.IsNullOrEmpty(modelDef))
                        {
                            Rootobject objRoot = DeserializeLeadScoreModelDef(modelDef);
                            leadScore = GetLeadScore(lead, objRoot, service);
                            score += leadScore;
                        }
                    }
                }
            }

            if (context.MessageName.ToLower() == "update")
            {
                if (existingLeadScore != -1)
                {
                    if (existingLeadScore != score)
                    {
                        Microsoft.Xrm.Sdk.Entity leadEntity = new Microsoft.Xrm.Sdk.Entity(Lead.EntityName);
                        leadEntity.Id = lead.Id;
                        leadEntity[Lead.LeadScore] = score;
                        service.Update(leadEntity);
                    }
                }
            }
            else if (context.MessageName.ToLower() == "create")
            {
                Microsoft.Xrm.Sdk.Entity leadEntity = new Microsoft.Xrm.Sdk.Entity(Lead.EntityName);
                leadEntity.Id = lead.Id;
                leadEntity[Lead.LeadScore] = score;
                service.Update(leadEntity);
            }
        }

        public Rootobject DeserializeLeadScoreModelDef(string modelDef)
        {
            Rootobject objRoot = null;
            using (MemoryStream DeSerializememoryStream = new MemoryStream())
            {
                //initialize DataContractJsonSerializer object and pass Rootobject class type to it
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Rootobject));

                //user stream writer to write JSON string data to memory stream
                StreamWriter writer = new StreamWriter(DeSerializememoryStream);
                writer.Write(modelDef);
                writer.Flush();

                DeSerializememoryStream.Position = 0;
                //get the Desrialized data in object of type Rootobject
                objRoot = (Rootobject)serializer.ReadObject(DeSerializememoryStream);
            }

            return objRoot;
        }

        public EntityCollection GetLeadScoringModelDefinition(IOrganizationService service)
        {
            EntityCollection ecLeadScoreModel = null;

            string fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
                                "<entity name='msdyncrm_leadscoremodel'>" +
                                "<attribute name='msdyncrm_name' />" +
                                "<attribute name='msdyncrm_leadscoremodelid' />" +
                                "<attribute name='msdyncrm_modeldefinition' />" +
                                "<order descending='false' attribute='msdyncrm_name' />" +
                                "<filter type='and'>" +
                                "<condition attribute='msdyncrm_modeldefinition' operator='not-null' />" +
                                "<condition attribute='statuscode' operator='eq' value='192350000' />" +
                                "</filter>" +
                                "</entity></fetch>";

            ecLeadScoreModel = service.RetrieveMultiple(new FetchExpression(fetchXml));

            return ecLeadScoreModel;
        }

        public static int GetLeadScore(Microsoft.Xrm.Sdk.Entity lead, Rootobject modelDef, IOrganizationService service)
        {
            int leadScore = 0;

            foreach (Rule rule in modelDef.Rules)
            {
                bool condSatisfied = false;
                bool firstCondition = false;
                foreach (Condition cond in rule.Conditions)
                {
                    int i = 0;
                    foreach (Expression exp in cond.Expressions)
                    {
                        i++;
                        if (lead.Attributes.Contains(exp.Field) && lead[exp.Field] != null)
                        {
                            if (GetOperatorfromName(exp.Operator, GetEntityAttributeValue(lead, exp, service), exp.Value, exp.FieldType, exp.Field, service))
                            {
                                if (i == 1)
                                {
                                    firstCondition = true;
                                    condSatisfied = true;
                                }
                                else
                                {
                                    if (firstCondition)
                                        condSatisfied = true;
                                }
                            }
                            else
                            {
                                if (i == 1)
                                {
                                    firstCondition = false;
                                    condSatisfied = false;
                                }
                                else
                                    condSatisfied = false;
                            }
                        }
                    }
                }
                if (condSatisfied)
                {
                    //Update Lead Score
                    leadScore += rule.Action.Value;
                }
            }
            return leadScore;
        }

        public static bool GetOperatorfromName(string opName, object source, object target, string dataType, string fieldName, IOrganizationService service)
        {
            bool condSatisfied = false;
            switch (opName)
            {
                case "GreaterThanOrEqualTo":
                    switch (dataType)
                    {
                        case "Edm.Int32":
                            if (Convert.ToInt32(source) >= Convert.ToInt32(target)) condSatisfied = true;
                            break;
                        case "Edm.Decimal":
                            if (Convert.ToDecimal(source) >= Convert.ToDecimal(target)) condSatisfied = true;
                            break;
                    }
                    break;
                case "LowerThanOrEqualTo":
                    switch (dataType)
                    {
                        case "Edm.Int32":
                            if (Convert.ToInt32(source) <= Convert.ToInt32(target)) condSatisfied = true;
                            break;
                        case "Edm.Decimal":
                            if (Convert.ToDecimal(source) <= Convert.ToDecimal(target)) condSatisfied = true;
                            break;
                    }
                    break;
                case "LowerThan":
                    switch (dataType)
                    {
                        case "Edm.Int32":
                            if (Convert.ToInt32(source) < Convert.ToInt32(target)) condSatisfied = true;
                            break;
                        case "Edm.Decimal":
                            if (Convert.ToDecimal(source) < Convert.ToDecimal(target)) condSatisfied = true;
                            break;
                    }
                    break;
                case "GreaterThan":
                    switch (dataType)
                    {
                        case "Edm.Int32":
                            if (Convert.ToInt32(source) > Convert.ToInt32(target)) condSatisfied = true;
                            break;
                        case "Edm.Decimal":
                            if (Convert.ToDecimal(source) > Convert.ToDecimal(target)) condSatisfied = true;
                            break;
                    }
                    break;
                case "Equals":
                    switch (dataType)
                    {
                        case "Edm.Int32":
                            if (Convert.ToInt32(source) == Convert.ToInt32(target)) condSatisfied = true;
                            break;
                        case "Edm.Decimal":
                            if (Convert.ToDecimal(source) == Convert.ToDecimal(target)) condSatisfied = true;
                            break;
                        case "Edm.Boolean":
                            if (Convert.ToBoolean(source) == Convert.ToBoolean(target)) condSatisfied = true;
                            break;
                    }
                    break;
            }

            return condSatisfied;
        }

        public static object GetDataTypeFromFieldSchemaName(string fieldName, IOrganizationService service)
        {
            // Create the request
            RetrieveAttributeRequest attributeRequest = new RetrieveAttributeRequest
            {
                EntityLogicalName = "lead",
                LogicalName = fieldName,
                RetrieveAsIfPublished = false
            };

            // Execute the request
            RetrieveAttributeResponse attributeResponse =
                (RetrieveAttributeResponse)service.Execute(attributeRequest);

            return attributeResponse.AttributeMetadata.AttributeType.Value;
        }

        public static object GetEntityAttributeValue(Microsoft.Xrm.Sdk.Entity lead, Expression exp, IOrganizationService service)
        {
            object value = null;

            switch (exp.FieldType)
            {
                case "Edm.Int32":
                    switch (GetDataTypeFromFieldSchemaName(exp.Field, service).ToString())
                    {
                        case "Picklist":
                            value = lead.GetAttributeValue<OptionSetValue>(exp.Field).Value;
                            break;
                        case "Integer":
                            value = lead.GetAttributeValue<Int32>(exp.Field);
                            break;
                    }
                    break;
                case "Edm.Decimal":
                    switch (GetDataTypeFromFieldSchemaName(exp.Field, service).ToString())
                    {
                        case "Money":
                            value = lead.GetAttributeValue<Money>(exp.Field).Value;
                            break;
                        case "Decimal":
                            value = lead.GetAttributeValue<decimal>(exp.Field);
                            break;
                    }
                    break;
                case "Edm.Boolean":
                    value = lead.GetAttributeValue<bool>(exp.Field);
                    break;
            }
            return value;
        }
        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Asynchronous, "Create");
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Asynchronous, "Update");
        }
    }

    [DataContract]
    public class Rootobject
    {
        [DataMember]
        public Rule[] Rules { get; set; }
        [DataMember]
        public object[] GradeRules { get; set; }
        [DataMember]
        public object Id { get; set; }
        [DataMember]
        public object Name { get; set; }
        [DataMember]
        public object SalesReadyScore { get; set; }
    }
    [DataContract]
    public class Rule
    {
        [DataMember]
        public string RuleName { get; set; }
        [DataMember]
        public Condition[] Conditions { get; set; }
        [DataMember]
        public Action Action { get; set; }
    }
    [DataContract]
    public class Action
    {
        [DataMember]
        public int Value { get; set; }
    }
    [DataContract]
    public class Condition
    {
        [DataMember]
        public Expression[] Expressions { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public Entity Entity { get; set; }
    }
    [DataContract]
    public class Entity
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string DisplayName { get; set; }
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public object[] EntityRelations { get; set; }
    }
    [DataContract]
    public class Expression
    {
        [DataMember]
        public object Value { get; set; }
        [DataMember]
        public string Field { get; set; }
        [DataMember]
        public string FieldType { get; set; }
        [DataMember]
        public string Operator { get; set; }
    }
}
