using System.Collections;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System.Security;
using System.Linq;
using Microsoft.Xrm.Sdk.Messages;
using System;

namespace XRMExtensions
{
    public static partial class IOrganizationServiceExtensions
    {
        /// <summary>
        /// Creates a list of records.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="entities">A list of entity instances that contains the properties to set in the newly created records.</param>
        public static IEnumerable<Entity> Create(this IOrganizationService service, IEnumerable<Entity> entities)
        {
            foreach (var entity in entities)
            {
                entity.Id = service.Create(entity);
            }

            return entities;
        }

        /// <summary>
        /// Updates a list of existing records.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="entities">A list of entity instances that have one or more properties set to be updated in the records.</param>
        public static void Update(this IOrganizationService service, IEnumerable<Entity> entities)
        {
            foreach (var entity in entities)
            {
                service.Update(entity);
            }
        }

        /// <summary>
        /// Deletes a record.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="reference">Entity record to delete.</param>
        public static void Delete(this IOrganizationService service, Entity entity)
        {
            service.Delete(entity.LogicalName, entity.Id);
        }

        /// <summary>
        /// Deletes a record.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="reference">EntityReference record to delete.</param>
        public static void Delete(this IOrganizationService service, EntityReference reference)
        {
            service.Delete(reference.LogicalName, reference.Id);
        }

        /// <summary>
        /// Deletes a list of records.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="entities">A list of entity reference records to delete.</param>
        public static void Delete(this IOrganizationService service, IEnumerable<Entity> entities)
        {
            foreach (var entity in entities)
            {
                service.Delete(entity);
            }
        }

        /// <summary>
        /// Deletes a list of records.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="entities">A list of entity reference records to delete.</param>
        public static void Delete(this IOrganizationService service, IEnumerable<EntityReference> references)
        {
            foreach (var reference in references)
            {
                service.Delete(reference);
            }
        }

        /// <summary>
        /// Creates a link between records.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="reference">EntityReference to disassociate.</param>
        /// <param name="relationship">The name of the relationship to be used to create the link.</param>
        /// <param name="relatedEntities">A collection of entity references (references to records) to be associated.</param>
        public static void Associate(this IOrganizationService service, EntityReference reference, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            service.Associate(reference.LogicalName, reference.Id, relationship, relatedEntities);
        }

        /// <summary>
        /// Deletes a link between records.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="reference">EntityReference to disassociate.</param>
        /// <param name="relationship">The name of the relationship to be used to remove the link.</param>
        /// <param name="relatedEntities">A collection of entity references (references to records) to be disassociated.</param>
        public static void Disassociate(this IOrganizationService service, EntityReference reference, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            service.Disassociate(reference.LogicalName, reference.Id, relationship, relatedEntities);
        }

        /// <summary>
        /// create set state request for an entity
        /// </summary>
        /// <param name="service"></param>
        /// <param name="record"></param>
        /// <param name="state"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static OrganizationRequest ToSetStateRequest(this IOrganizationService service, Entity record, int state, int status)
        {
            var targetEntity = new Entity(record.LogicalName, record.Id);
            return new SetStateRequest()
            {
                EntityMoniker = new EntityReference(record.LogicalName, record.Id),
                State = new OptionSetValue(state),
                Status = new OptionSetValue(status),
            };
        }

        /// <summary>
        /// retrieve multiple records (all pages) using fetch xml. Use <fetch {0}> to replace {0} with page number
        /// </summary>
        /// <param name="service"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static List<Entity> RetrieveMultipleByFetchXml(this IOrganizationService service, string fetxmlQuery)
        {
            var moreRecords = false;
            int page = 1;
            var pageCookie = string.Empty;
            var records = new List<Entity>();
            do
            {
                var fetchXml = string.Format(fetxmlQuery, pageCookie);
                var results = service.RetrieveMultiple(new FetchExpression(fetchXml));
                if (results.Entities.Count > 0)
                {
                    records.AddRange(results.Entities);
                }

                moreRecords = results.MoreRecords;
                if (moreRecords)
                {
                    page++;
                    pageCookie = string.Format(" paging-cookie='{0}' page='{1}'", SecurityElement.Escape(results.PagingCookie), page);
                }
            } while (moreRecords);

            return records;
        }

        /// <summary>
        /// Execute organization request 
        /// </summary>
        /// <param name="requestsToProcess"></param>
        /// <param name="service"></param>
        /// <param name="pageSize"></param>
        public static void ProcessRequests(this IOrganizationService service, IEnumerable<OrganizationRequest> requestsToProcess, int pageSize = 200)
        {
            var pageNo = 0;
            var recordsToSkip = 0;

            var processRequests = requestsToProcess.Skip(recordsToSkip).Take(pageSize);
            while (processRequests.Count() != 0)
            {
                var exMultipleRequest = new ExecuteMultipleRequest()
                {
                    Settings = new ExecuteMultipleSettings()
                    {
                        ContinueOnError = true,
                        ReturnResponses = false
                    },
                    Requests = new OrganizationRequestCollection()
                };

                exMultipleRequest.Requests.AddRange(processRequests);
                service.Execute(exMultipleRequest);

                pageNo += 1;
                recordsToSkip = (pageNo * pageSize);
                processRequests = requestsToProcess.Skip(recordsToSkip).Take(pageSize);
            }
        }

        /// <summary>
        /// Execute organization request in transaction
        /// </summary>
        /// <param name="requestsToProcess"></param>
        /// <param name="service"></param>
        /// <param name="pageSize"></param>
        public static void ProcessRequestsInTransaction(this IOrganizationService service, List<OrganizationRequest> requestsToProcess, int pageSize = 200)
        {
            var pageNo = 0;
            var recordsToSkip = 0;

            var processRequests = requestsToProcess.Skip(recordsToSkip).Take(pageSize);
            while (processRequests.Count() != 0)
            {
                ExecuteTransactionRequest multipleRequest = new ExecuteTransactionRequest()
                {
                    ReturnResponses = false,
                };
                multipleRequest.Requests = new OrganizationRequestCollection();
                multipleRequest.Requests.AddRange(processRequests);

                // Transaction call
                var responseForCreateRecords = (ExecuteTransactionResponse)service.Execute(multipleRequest);
                pageNo += 1;
                recordsToSkip = (pageNo * pageSize);
                processRequests = requestsToProcess.Skip(recordsToSkip).Take(pageSize);
            }
        }

        /// <summary>
        ///  Get organization service user id
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public static Guid UserId(this IOrganizationService service)
        {
            return ((WhoAmIResponse)service.Execute(new WhoAmIRequest())).UserId;
        }

        /// <summary>
        /// Submit bulk delete job 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="requestName"></param>
        /// <param name="userID"></param>
        /// <param name="queryExpressions"></param>
        public static void SubmitBulkDeleteRequest(this IOrganizationService service, string requestName, Guid userID, params QueryExpression[] queryExpressions)
        {
            // construct query
            BulkDeleteRequest bulkDeleteRequest = new BulkDeleteRequest()
            {
                JobName = requestName,
                QuerySet = queryExpressions,
                StartDateTime = DateTime.Now,
                RecurrencePattern = string.Empty,
                SendEmailNotification = false,
                ToRecipients = new Guid[] { userID },
                CCRecipients = new Guid[] { userID }
            };
            // submit bulk delete request
            service.Execute(bulkDeleteRequest);
        }

        /// <summary>
        /// Convert Query Expression to fetch xml
        /// </summary>
        /// <param name="queryExpression"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public static string ToFetchXMl(this QueryExpression queryExpression, IOrganizationService service)
        {
            var queryExpressionToFetchXmlRequest = new QueryExpressionToFetchXmlRequest() { Query = queryExpression };
            var response = (QueryExpressionToFetchXmlResponse)service.Execute(queryExpressionToFetchXmlRequest);
            // return fetch xml
            return response.FetchXml;
        }
    }
}