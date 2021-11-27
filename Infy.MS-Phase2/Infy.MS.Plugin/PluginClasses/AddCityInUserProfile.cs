using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xrm;
using XRMExtensions;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System.Net.Http;
using System.Net;
using System.Web;

namespace Infy.MS.Plugins
{
    public class AddCityInUserProfile : BasePlugin
    {

        string stateLicence = string.Empty;
        string RelationshipName = Constants.UsertoStateRelationshipName;
        Guid recordexistornot = Guid.Empty;
        Guid bookableresourceid = Guid.Empty;
        string GetStateLicense = Constants.GetStateLicense;
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            if (context == null)
            {
                throw new InvalidPluginExecutionException("Context not Found");
            }

            if (context.MessageName.ToLower() == "update" || context.MessageName.ToLower() == "create")
            {

                Entity Target = context.GetTargetEntity<Entity>();
                string Citycode = string.Empty;
                EntityCollection result = getUsers(context.SystemOrganizationService,Target.Id);
                //QueryExpression query = new QueryExpression(SystemUser.EntityName);
                //ColumnSet cols = new ColumnSet(true);
                //query.ColumnSet = cols;
                //FilterExpression Filter1 = new FilterExpression(LogicalOperator.And);
                //Filter1.AddCondition(SystemUser.DisclaimerText, ConditionOperator.NotNull);
                //Filter1.AddCondition(SystemUser.Nmlsnumber, ConditionOperator.NotNull);
                //Filter1.AddCondition(SystemUser.EntityId, ConditionOperator.Equal, Target.Id);
                //query.Criteria.AddFilter(Filter1);
                //EntityCollection result = context.RetrieveMultiple(query);
                if (result.Entities.Count > 0)
                {
                    if (result.Entities[0].Contains(SystemUser.DisclaimerText))
                    {
                        Guid userGuid = result.Entities[0].Id;
                        //string ImageUrl = result.Entities[0].Attributes[SystemUser.photourl].ToString();
                        EntityReferenceCollection relatedEntities = new EntityReferenceCollection();
                        EntityReference secondaryEntity = new EntityReference(SystemUser.EntityName, userGuid);
                        relatedEntities.Add(secondaryEntity);
                        //DisassociateStateRecords(context, userGuid, relatedEntities);
                        var Sates = result.Entities[0].Attributes[SystemUser.DisclaimerText].ToString();
                        if (Sates != null && Sates != string.Empty)
                        {
                            Sates = Sates.Trim();
                            var data = Sates.Split('|');
                            var state1 = data[0];
                            data = state1.Split(',');
                            //int i = 0;
                            foreach (var city in data)
                            {
                                Console.WriteLine(city);
                                stateLicence = city.Trim();
                                Citycode = stateLicence.Substring(0, 2);
                                //AssociateRecord(context, Citycode, relatedEntities);
                                //---------------------------------------------------------------//
                                ///Below validation for user security role was implemented to avoid the creation of bookable resource for retail LO's
                                /// to solve call forwarding to retail LO's
                                //---------------------------------------------------------------//
                                if (userHasRole(context.SystemOrganizationService, "Movement Direct Admin", userGuid)|| userHasRole(context.SystemOrganizationService, "Movement Direct LO", userGuid))
                                {
                                    recordexistornot = CheckBookableResourceExistorNOt(context, userGuid);
                                    if (recordexistornot != Guid.Empty && recordexistornot != null)
                                    {

                                        Createbookableresourcecharacteristic(context.SystemOrganizationService, recordexistornot, Citycode, stateLicence);
                                    }
                                    else
                                    {
                                        bookableresourceid = CreateBookableResource(context.SystemOrganizationService, userGuid);
                                        if (bookableresourceid != Guid.Empty && bookableresourceid != null)
                                        {
                                            Createbookableresourcecharacteristic(context.SystemOrganizationService, bookableresourceid, Citycode, stateLicence);
                                        }
                                    }
                                }
                            }

                        }

                    }
                }
            }
        }

        public void AssociateRecord(IOrganizationService service, string statecode, EntityReferenceCollection systemuser)
        {
            QueryExpression Statecodedata = new QueryExpression(State.EntityName);
            ColumnSet cols = new ColumnSet(true);
            Statecodedata.ColumnSet = cols;
            FilterExpression Filter1 = new FilterExpression(LogicalOperator.And);
            Filter1.AddCondition(State.Code, ConditionOperator.Equal, statecode);
            Statecodedata.Criteria.AddFilter(Filter1);
            EntityCollection statecodeobj = service.RetrieveMultiple(Statecodedata);
            var relationship = new Relationship(RelationshipName);
            if (statecodeobj.Entities.Count > 0)
            {
                Guid stateGuid = statecodeobj.Entities[0].Id;
                service.Associate(State.EntityName, stateGuid, relationship, systemuser);

            }

        }
        public void DisassociateStateRecords(IOrganizationService service, Guid record, EntityReferenceCollection relatedEntities)
        {
            var fetchxml3 = @"<?xml version='1.0'?>" +
                            "<fetch distinct='false' mapping='logical' output-format='xml-platform' version='1.0'>" +
                            "<entity name='ims_state'>" +
                            "<attribute name='createdon' />" +
                            "<order attribute='createdon' descending='false'/>" +
                            "<link-entity name='ims_systemuser_ims_state' from='ims_stateid' to='ims_stateid'   visible='false' intersect='true'>" +
                            "<link-entity name='systemuser' from='systemuserid' to='systemuserid' alias='ab'>" +
                            "</link-entity>" +
                            "</link-entity>" +
                            "</entity>" +
                            "</fetch>";
            EntityCollection output4 = service.RetrieveMultiple(new FetchExpression(fetchxml3));

            if (output4.Entities.Count > 0)
            {
                foreach (Entity objrecode in output4.Entities)
                {
                    Guid Stateguid = objrecode.Id;
                    Disassociate(service, Stateguid, relatedEntities);
                }
            }

        }
        public void Disassociate(IOrganizationService service, Guid record, EntityReferenceCollection relatedEntities)
        {
            var relationship = new Relationship(RelationshipName);
            service.Disassociate(State.EntityName, record, relationship, relatedEntities);
        }

        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "Create");
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Synchronous, "Update");
        }
        public Guid CheckBookableResourceExistorNOt(IOrganizationService service, Guid userGuid)
        {
            Guid Resourceid = Guid.Empty;

            QueryExpression BookableResources = new QueryExpression(BookableResource.EntityName);
            ColumnSet cols = new ColumnSet(true);
            BookableResources.ColumnSet = cols;
            FilterExpression Filter1 = new FilterExpression(LogicalOperator.And);
            Filter1.AddCondition(BookableResource.statecode, ConditionOperator.Equal, 0);
            Filter1.AddCondition(BookableResource.userid, ConditionOperator.Equal, userGuid);
            BookableResources.Criteria.AddFilter(Filter1);
            EntityCollection fetchBookableResource = service.RetrieveMultiple(BookableResources);

            if (fetchBookableResource.Entities.Count > 0)
            {
                Resourceid = fetchBookableResource.Entities[0].Id;
                //recordexist = true;
            }
            return Resourceid;
        }
        public EntityCollection getUsers(IOrganizationService service,Guid userId)
        {
            var fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>" +
                        "  <entity name='systemuser'>" +
                        "    <attribute name='fullname' />" +
                        "    <attribute name='businessunitid' />" +
                        "    <attribute name='title' />" +
                        "    <attribute name='ims_disclaimertext' />" +
                        "    <attribute name='systemuserid' />" +
                        "    <order attribute='fullname' descending='false' />" +
                        "    <filter type='and'>" +
                        "      <condition attribute='ims_nmlsnumber' operator='not-null' />" +
                        "      <condition attribute='ims_disclaimertext' operator='not-null' />" +
                        "      <condition attribute='systemuserid' operator='eq' value='" + userId + "' /> " +
                        "    </filter>" +
                        "    <link-entity name='systemuserroles' from='systemuserid' to='systemuserid' visible='false' intersect='true'>" +
                        "      <link-entity name='role' from='roleid' to='roleid' alias='ae' />" +
                        "    </link-entity>" +
                        "  </entity>" +
                        "</fetch>";
        EntityCollection ecUsers= service.RetrieveMultiple(new FetchExpression(fetch));
            return ecUsers;
        }

        public Guid CreateBookableResource(IOrganizationService service, Guid userGuid)
        {
            Guid NewBookableResourceid = Guid.Empty;
            Entity CreateBookableResourceobj = new Entity(BookableResource.EntityName);
            CreateBookableResourceobj[BookableResource.resourcetype] = new OptionSetValue(3);
            CreateBookableResourceobj[BookableResource.userid] = new EntityReference(SystemUser.EntityName, userGuid);
            CreateBookableResourceobj[BookableResource.ownerid] = new EntityReference(SystemUser.EntityName, userGuid);
            NewBookableResourceid = service.Create(CreateBookableResourceobj);
            return NewBookableResourceid;
        }

        public void Createbookableresourcecharacteristic(IOrganizationService service, Guid userGuid, string Citycode, string state)

        {
            Guid getStateid = FetchStateId(service, Citycode);
            if (getStateid != null)
            {
                QueryExpression bookableresourcecharacteristic = new QueryExpression(BookableResourceCharacteristic.EntityName);
                ColumnSet cols = new ColumnSet(true);
                bookableresourcecharacteristic.ColumnSet = cols;
                FilterExpression Filter1 = new FilterExpression(LogicalOperator.And);
                Filter1.AddCondition(BookableResourceCharacteristic.statecode, ConditionOperator.Equal, 0);
                Filter1.AddCondition(BookableResourceCharacteristic.resource, ConditionOperator.Equal, userGuid);
                Filter1.AddCondition(BookableResourceCharacteristic.state, ConditionOperator.Equal, getStateid);
                bookableresourcecharacteristic.Criteria.AddFilter(Filter1);
                EntityCollection fetchbookableresourcecharacteristic = service.RetrieveMultiple(bookableresourcecharacteristic);
                if (fetchbookableresourcecharacteristic.Entities.Count > 0)
                {
                    return;
                }
                else
                {

                    if (getStateid != null && getStateid != Guid.Empty)
                    {
                        Guid getCharactersticId = characteristic(service, GetStateLicense);
                        Entity Createbookableresourcecharacteristic = new Entity(BookableResourceCharacteristic.EntityName);
                        Createbookableresourcecharacteristic[BookableResourceCharacteristic.characteristic] = new EntityReference(Characteristic.EntityName, getCharactersticId);
                        Createbookableresourcecharacteristic[BookableResourceCharacteristic.resource] = new EntityReference(BookableResource.EntityName, userGuid);
                        Createbookableresourcecharacteristic[BookableResourceCharacteristic.state] = new EntityReference(State.EntityName, getStateid);
                        Createbookableresourcecharacteristic[BookableResourceCharacteristic.ims_statelicense] = state;
                        service.Create(Createbookableresourcecharacteristic);

                    }
                }
            }
        }
        public Guid FetchStateId(IOrganizationService service, string citycode)
        {
            Guid stateId = Guid.Empty;
            QueryExpression state = new QueryExpression(State.EntityName);
            state.ColumnSet = new ColumnSet(true);
            FilterExpression filter1 = new FilterExpression(LogicalOperator.And);
            filter1.AddCondition(State.Code, ConditionOperator.Equal, citycode);
            state.Criteria.AddFilter(filter1);
            EntityCollection getStates = service.RetrieveMultiple(state);
            if (getStates.Entities.Count > 0)
            {
                stateId = getStates.Entities[0].Id;
            }


            return stateId;
        }

        public Guid characteristic(IOrganizationService service, string name)
        {
            Guid characteristicId = Guid.Empty;
            QueryExpression characteristic = new QueryExpression(Characteristic.EntityName);
            ColumnSet cols = new ColumnSet(true);
            characteristic.ColumnSet = cols;
            FilterExpression Filter1 = new FilterExpression(LogicalOperator.And);
            Filter1.AddCondition(BookableResourceCharacteristic.statecode, ConditionOperator.Equal, 0);
            Filter1.AddCondition(Characteristic.name, ConditionOperator.Equal, name);
            // Filter1.AddCondition(BookableResourceCharacteristic.state, ConditionOperator.Equal, getStateid);
            characteristic.Criteria.AddFilter(Filter1);
            EntityCollection fetchbookableresourcecharacteristic = service.RetrieveMultiple(characteristic);
            if (fetchbookableresourcecharacteristic.Entities.Count > 0)
            {
                characteristicId = fetchbookableresourcecharacteristic.Entities[0].Id;
            }

            return characteristicId;
        }


        private static bool userHasRole(IOrganizationService service,string roleName,Guid userId)
        {

            QueryExpression qe = new QueryExpression("systemuserroles");
            qe.Criteria.AddCondition("systemuserid", ConditionOperator.Equal, userId);
            LinkEntity link = qe.AddLink("role","roleid","roleid",JoinOperator.Inner);
            link.LinkCriteria.AddCondition("name",ConditionOperator.Equal,roleName);
            EntityCollection results = service.RetrieveMultiple(qe);
            if (results.Entities.Count >0)
            {
                return true;
            }
            return false;
        }
    }
    public class CustomWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
            if (request != null)
            {
                request.Timeout = 15000; //15 Seconds
                request.KeepAlive = false;

            }
            return request;
        }
    }
}

