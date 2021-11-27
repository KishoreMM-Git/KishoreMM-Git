using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using OfficeOpenXml.Core.ExcelPackage;

namespace ImportExcelFunctionality
{
    class ImportToCRM
    {
        static IOrganizationService service;
        public static void ConnectToMSCRM(string OrgUri)
        {
            try
            {
                ClientCredentials credentials = new ClientCredentials();
                credentials.UserName.UserName = "nehasingh.maurya@movement.com";
                credentials.UserName.Password = "Password";
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                Uri serviceUri = new Uri(OrgUri);
                OrganizationServiceProxy proxy = new OrganizationServiceProxy(serviceUri, null, credentials, null);
                proxy.EnableProxyTypes();
                service = (IOrganizationService)proxy;

                if (service != null)
                {
                    Guid userid = ((WhoAmIResponse)service.Execute(new WhoAmIRequest())).UserId;

                    if (userid != Guid.Empty)
                    {
                        Console.WriteLine("Connection Established Successfully...");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while connecting to CRM " + ex.Message);
                Console.ReadKey();
            }
        }



        static void Main(string[] args)
        {
            ConnectToMSCRM("https://mmdevphase2.api.crm.dynamics.com/XRMServices/2011/Organization.svc");
            ImportRecordFromExcelToCRM();
        }


        public static void ImportRecordFromExcelToCRM()
        {
            string sourceEntity = string.Empty;
            string targetCrmEntity = string.Empty;
            var importMap = new ImportMap()
            {
                Name = "Import Map " + DateTime.Now.Ticks.ToString(),
                Source = "import " + ims_loanstaging.EntityLogicalName,
                Description = "Description of data being imported",
                EntitiesPerFile = new OptionSetValue(1),
                EntityState = EntityState.Created
            };
            Guid importMapId = service.Create(importMap);

            QueryExpression GetConfig = new QueryExpression()
            {
                EntityName = ims_configuration.EntityLogicalName,
                ColumnSet = new ColumnSet("ims_valuemultiline", "ims_value", "ims_description"),
                Criteria =
                        {
                        FilterOperator = LogicalOperator.And,
                        Conditions =
                                    {
                                        new ConditionExpression("ims_name", ConditionOperator.Equal, "Loan Staging Data Import"),
                                    }
                        }
            };
            EntityCollection RetrieveLoanStatagingDetails = service.RetrieveMultiple(GetConfig);

            foreach (var configDetails in RetrieveLoanStatagingDetails.Entities)
            {
                string jsonObj = string.Empty;
                if (configDetails.Contains("ims_value"))
                {
                    sourceEntity = configDetails["ims_value"].ToString();
                }
                if (configDetails.Contains("ims_description"))
                {
                    targetCrmEntity = configDetails["ims_description"].ToString();
                }
                if (configDetails.Contains("ims_valuemultiline"))
                {
                    jsonObj = configDetails["ims_valuemultiline"].ToString();
                }
                Dictionary<string, string> mappingAattributes = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonObj);

                foreach (var item in mappingAattributes)
                {
                    FieldMapping(importMapId, item.Key, sourceEntity, item.Value, targetCrmEntity);
                }

            }
            var import = new Import()
            {
                ModeCode = new OptionSetValue(0),
                Name = "Importing data"
            };
            Guid importId = service.Create(import);

            var systemUserRequest = new WhoAmIRequest();
            var systemUserResponse =
                (WhoAmIResponse)service.Execute(systemUserRequest);

            var importFile = new ImportFile()
            {
                Content = ReadXlsxFile(@"C:\Users\nehasingh.maurya\Desktop\T\LDW Lead Import.xlsx"),
                Name = "Import " + sourceEntity,
                IsFirstRowHeader = true,
                //ImportMapId = new EntityReference(ImportMap.EntityLogicalName, importMapId),
                UseSystemMap = false,
                Source = sourceEntity + ".xlsx",
                SourceEntityName = sourceEntity,
                TargetEntityName = "ims_leadstaging",
                ImportId = new EntityReference(Import.EntityLogicalName, importId),
                EnableDuplicateDetection = false,
                FieldDelimiterCode = new OptionSetValue(2),
                DataDelimiterCode = new OptionSetValue(1),
                ProcessCode = new OptionSetValue(1),
                FileTypeCode = new OptionSetValue(3),
                RecordsOwnerId = new EntityReference(SystemUser.EntityLogicalName, systemUserResponse.UserId)
            };

            Guid importFileId = service.Create(importFile);
            Console.WriteLine("ImportFileId : " + importFileId);

            //ParseImportRequest parseRequest = new ParseImportRequest();
            //parseRequest.ImportId = importId;
            //ParseImportResponse parseImportResponse = (ParseImportResponse)service.Execute(parseRequest);
            //Console.WriteLine("Waiting for Parse async job to complete");

            //TransformImportRequest transformRequest = new TransformImportRequest();
            //transformRequest.ImportId = importId;
            //TransformImportResponse transResponse = (TransformImportResponse)service.Execute(transformRequest);
            //Console.WriteLine("Waiting for Transform async job to complete");

            //ImportRecordsImportRequest importRequest = new ImportRecordsImportRequest();
            //importRequest.ImportId = importId;
            //ImportRecordsImportResponse importResponse = (ImportRecordsImportResponse)service.Execute(importRequest);
            //Console.WriteLine("Waiting for ImportRecords async job to complete");

        }

        public static string ReadCsvFile(string filePath)
        {
            string data = string.Empty;
            using (StreamReader reader = new StreamReader(filePath))
            {
                string value = reader.ReadLine();
                while (value != null)
                {
                    data += value;
                    data += "\n";
                    value = reader.ReadLine();
                }
            }
            return data;
        }

        public static string ReadXmlFile(string filePath)
        {
            string data = string.Empty;
            using (StreamReader reader = new StreamReader(filePath))
            {
                data = reader.ReadToEnd();
            }
            return data;
        }

        public static void FieldMapping(Guid importMapId, string sourceAttributeName, string sourceEntityName, string targetAttributeName, string targetEntityName)
        {
            var colMapping = new ColumnMapping()
            {
                SourceAttributeName = sourceAttributeName,
                SourceEntityName = sourceEntityName,
                TargetAttributeName = targetAttributeName,
                TargetEntityName = targetEntityName,
                ImportMapId = new EntityReference(ImportMap.EntityLogicalName, importMapId),
                ProcessCode = new OptionSetValue(1)
            };
            Guid colMappingId = service.Create(colMapping);
        }

        public static void ReportErrors(IOrganizationService service, Guid importFileId)
        {
            QueryByAttribute importLogQuery = new QueryByAttribute();
            importLogQuery.EntityName = ImportLog.EntityLogicalName;
            importLogQuery.ColumnSet = new ColumnSet(true);
            importLogQuery.Attributes.Add("importfileid");
            importLogQuery.Values.Add(new object[1]);
            importLogQuery.Values[0] = importFileId;

            EntityCollection importLogs = service.RetrieveMultiple(importLogQuery);

            if (importLogs.Entities.Count > 0)
            {
                Console.WriteLine("Number of Failures: " + importLogs.Entities.Count.ToString());
                Console.WriteLine("Sequence Number    Error Number    Description    Column Header    Column Value   Line Number");

                // Display errors.
                foreach (ImportLog log in importLogs.Entities)
                {
                    Console.WriteLine(
                        string.Format("Sequence Number: {0}\nError Number: {1}\nDescription: {2}\nColumn Header: {3}\nColumn Value: {4}\nLine Number: {5}",
                            log.SequenceNumber.Value,
                            log.ErrorNumber.Value,
                            log.ErrorDescription,
                            log.HeaderColumn,
                            log.ColumnValue,
                            log.LineNumber.Value));
                }
            }
        }

        public static void WaitForAsyncJobCompletion(IOrganizationService service, Guid asyncJobId)
        {
            ColumnSet cs = new ColumnSet("statecode", "statuscode");
            AsyncOperation asyncjob = (AsyncOperation)service.Retrieve("asyncoperation", asyncJobId, cs);

            int retryCount = 1000;

            while (asyncjob.StateCode.Value != AsyncOperationState.Completed && retryCount > 0)
            {
                asyncjob = (AsyncOperation)service.Retrieve("asyncoperation", asyncJobId, cs);
                System.Threading.Thread.Sleep(5000);
                retryCount--;
                Console.WriteLine("Async operation state is " + asyncjob.StateCode.Value.ToString());
            }

        }

    }

}
