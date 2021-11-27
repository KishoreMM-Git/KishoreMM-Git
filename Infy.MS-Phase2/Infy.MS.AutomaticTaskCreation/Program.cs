using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Crm.Sdk.Messages;
using System.Net;
using System.ServiceModel.Description;
using Microsoft.Xrm.Sdk.Query;
using System.Net.Mail;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Data;


//using Microsoft.Azure.KeyVault;
//using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Xrm.Sdk.WebServiceClient;
//using Microsoft.Azure.KeyVault;
//using Microsoft.Azure.WebJobs;
using System.Net.Http;
using System.Net.Http.Headers;
//using AutoShareLeadLoanContacts;
//using Newtonsoft.Json.Linq;
using System.Threading;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault;

namespace AutomaticTaskCreation
{
    class Program
    {
        static Entity Target = null;
        static IOrganizationService organizationService = null;
        static DateTime closingDate;
        static Guid loanId = Guid.Empty;
        static double csdDate;
        static double apdDate;
        static Guid loanOwnerId = Guid.Empty;
        static void Main(string[] args)
        {
            try
            {
                organizationService = getProxy();
                string datetimeVal = string.Empty;
                #region FOR APPLICATION LOAN STATUS TASK CREATION
                string applicationXml = @"<fetch {0} >
  <entity name='opportunity' >
    <attribute name='ims_closingdate' />
    <attribute name='name' />
    <attribute name='opportunityid' />
    <attribute name='ims_apd' />
    <attribute name='ims_loanstatus' />
    <attribute name='ims_submittedstatusdate' />
    <attribute name='ims_applicationstatusdate' />
    <attribute name='ims_csddate' />
    <attribute name='ims_cdsent' />
    <attribute name='ims_primaryloa' />
    <attribute name='ownerid' />
    <attribute name='originatingleadid'/>
    <filter>
      <condition attribute='ims_applicationstatusdate' operator='not-null' />
      <condition attribute='ims_loanstatus' operator='eq' uiname='Application' uitype='ims_loanstatus' value='8A655E22-C006-EA11-A811-000D3A4E6DC6' />
    </filter>
    <link-entity name='systemuser' from='systemuserid' to='owninguser' >
      <attribute name='systemuserid' />
      <attribute name='ims_automationrequired' />
      <filter>
        <condition attribute='systemuserid' operator='not-null' />
        <condition attribute='ims_automationrequired' operator='eq' value='1' />
      </filter>
    </link-entity>
  </entity>
</fetch>";

                List<Entity> loans = pagingmethod(applicationXml);
                int appProxycounter = 0;
                int appCount = 0;
                //EntityCollection loans = organizationService.RetrieveMultiple(new FetchExpression(applicationXml));
                foreach (var loanety in loans)
                {
                    loanId = loanety.Id;
                    appProxycounter++;
                    if (appProxycounter == 300)
                    {
                        appCount++;
                        getProxy();
                        int totalCount = appCount * appProxycounter;
                        Console.WriteLine("The total count for Application Loan Status is" + "-" + totalCount.ToString());
                        appProxycounter = 0;
                    }
                    if (loanety.Attributes.Contains("ims_primaryloa"))
                    {
                        loanOwnerId = loanety.GetAttributeValue<EntityReference>("ims_primaryloa").Id;
                    }
                    if (!loanety.Attributes.Contains("ims_primaryloa"))
                    {
                        loanOwnerId = loanety.GetAttributeValue<EntityReference>("ownerid").Id;
                    }
                    if (loanety.Attributes.Contains("ims_loanstatus"))
                    {
                        if (loanety.GetAttributeValue<EntityReference>("ims_loanstatus").Name.ToLower() == "application")
                        {
                            if (loanety.Attributes.Contains("ims_applicationstatusdate"))
                            {
                                #region Check Whether Tasks Exist and Prevent Duplicate Task Creation here
                                string CheckIfTasksExists = @"<fetch top='5000' >
                    <entity name='task' >
                      <attribute name='subject' />
                      <attribute name='description' />
                      <filter>
                        <condition attribute='subject' operator='like' value='Submit to UW' />
                      <condition attribute='regardingobjectid' operator='eq' value='" + loanId + @"'/>
                       </filter >
                      </entity >
                     </fetch > ";
                                EntityCollection ifTaskExists = organizationService.RetrieveMultiple(new FetchExpression(CheckIfTasksExists));
                                if (ifTaskExists.Entities.Count > 0)
                                {
                                    //return;
                                }
                                else
                                {
                                    DateTime statusDate = loanety.GetAttributeValue<DateTime>("ims_applicationstatusdate");
                                    DateTime d1 = checkBusinessClosures_loanStatusDate(organizationService, "5", statusDate);
                                    if (d1.Date <= DateTime.Now.Date)
                                    {
                                        //Create task here
                                        fetchXml("Application-Dead in Water", organizationService, loanety);
                                    }
                                }
                                #endregion
                            }
                        }
                    }

                }
                #endregion*/

                #region FOR SUBMITTED LOAN STATUS TASK CREATION
                string submittedXml = @"
                    <fetch {0}>
                      <entity name='opportunity'>
                        <attribute name='ims_closingdate' />
                        <attribute name='name' />
                        <attribute name='opportunityid' />
                        <attribute name='ims_apd' />
                        <attribute name='ims_loanstatus' />
                        <attribute name='ims_submittedstatusdate' />
                        <attribute name='ims_applicationstatusdate' />
                        <attribute name='ims_csddate' />
                        <attribute name='ims_cdsent' />
                        <attribute name='ims_primaryloa' />
                        <attribute name='originatingleadid'/>
                        <attribute name='ownerid' />
                        <filter>
                          <condition attribute='ims_submittedstatusdate' operator='not-null' />
                          <condition attribute='ims_loanstatus' operator='eq' uiname='Submitted' uitype='ims_loanstatus' value='6B373F6C-9B07-EA11-A811-000D3A4F62E7' />
                        </filter>
                        <link-entity name='systemuser' from='systemuserid' to='owninguser'>
                          <attribute name='systemuserid' />
                          <filter>
                            <condition attribute='systemuserid' operator='not-null' />
                            <condition attribute='ims_automationrequired' operator='eq' value='1' />
                          </filter>
                        </link-entity>
                      </entity>
                    </fetch>";
                List<Entity> submittedLoans = pagingmethod(submittedXml);
                int subProxycounter = 0;
                int subCount = 0;
                foreach (var loanety in submittedLoans)
                {
                    subProxycounter++;
                    if (subProxycounter == 300)
                    {
                        subCount++;
                        getProxy();
                        int totalCount = subCount * subProxycounter;
                        Console.WriteLine("The Total count for Submitted Loan is" + "-" + totalCount.ToString());
                        subProxycounter = 0;
                    }
                    if (loanety.Attributes.Contains("ims_loanstatus"))
                    {
                        if (loanety.GetAttributeValue<EntityReference>("ims_loanstatus").Name.ToLower() == "submitted")
                        {
                            if (loanety.Attributes.Contains("ims_submittedstatusdate"))
                            {
                                loanId = loanety.Id;
                                if (loanety.Attributes.Contains("ims_primaryloa"))
                                {
                                    loanOwnerId = loanety.GetAttributeValue<EntityReference>("ims_primaryloa").Id;
                                }
                                if (!loanety.Attributes.Contains("ims_primaryloa"))
                                {
                                    loanOwnerId = loanety.GetAttributeValue<EntityReference>("ownerid").Id;
                                }
                                string CheckIfTasksExists = @"<fetch top='5000' >
                      <entity name='task' >
                        <attribute name='subject' />
                        <attribute name='description' />
                        <filter>
                          <condition attribute='subject' operator='like' value='%Loan Approved?%' />
                          <condition attribute='regardingobjectid' operator='eq' value='" + loanId + @"'/>
                        </filter>
                      </entity>
                    </fetch>";
                                EntityCollection ifTaskExists = organizationService.RetrieveMultiple(new FetchExpression(CheckIfTasksExists));
                                if (ifTaskExists.Entities.Count > 0)
                                {
                                    //return;
                                }
                                else
                                {
                                    DateTime statusDate = loanety.GetAttributeValue<DateTime>("ims_submittedstatusdate");
                                    DateTime d1 = checkBusinessClosures_loanStatusDate(organizationService, "4", statusDate);
                                    //int daysDiff = Convert.ToInt32((statusDate.Date - DateTime.Now.Date).TotalDays);
                                    if (d1.Date <= DateTime.Now.Date)
                                    {
                                        //Create task here
                                        fetchXml("Submitted-Approved with Conditions Follow", organizationService, loanety);
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion

                #region FOR CSD DATE STATUS TASK CREATION

                var Today = DateTime.Now.Date.ToString("yyyy/MM/dd");//ToShortDateString();
                var date = checkBusinessClosures(organizationService, "10").ToString("yyyy/MM/dd");
                string csdXml = @"
<fetch {0}>
<entity name='opportunity'>
 <attribute name='ims_closingdate' />
 <attribute name='name' />
 <attribute name='opportunityid' />
 <attribute name='ims_apd' />
 <attribute name='ims_loanstatus' />
 <attribute name='ims_submittedstatusdate' />
 <attribute name='ims_applicationstatusdate' />
 <attribute name='ims_csddate' />
 <attribute name='ims_cdsent' />
<attribute name='originatingleadid'/>
 <attribute name='ims_primaryloa' />
 <attribute name='ownerid' />
 <filter>
         <condition attribute='ims_closingdate' operator='lt' value='" + date + @"' />
      <condition attribute='ims_closingdate' operator='gt' value='" + Today + @"' />
   <condition attribute='ims_csddate' operator='null' />
 <condition attribute='ims_loanstatus' operator='not-in'>
        <value>C6655E22-C006-EA11-A811-000D3A4E6DC6</value>
        <value>766DA378-2117-EA11-A815-000D3A4ED4E6</value>
      </condition>
 </filter>
 <link-entity name='systemuser' from='systemuserid' to='owninguser'>
   <attribute name='systemuserid' />
   <filter>
     <condition attribute='systemuserid' operator='not-null' />
    <condition attribute='ims_automationrequired' operator='eq' value='1' />
   </filter>
 </link-entity>
</entity>
</fetch>";
                List<Entity> csdLoans = pagingmethod(csdXml);
                int csdProxycounter = 0;
                int csdCount = 0;
                //EntityCollection csdLoans = organizationService.RetrieveMultiple(new FetchExpression(csdXml));
                foreach (var loanety in csdLoans)
                {
                    csdProxycounter++;
                    if (csdProxycounter == 300)
                    {
                        csdCount++;
                        getProxy();
                        int totalCount = csdCount * csdProxycounter;
                        Console.WriteLine("The Total record cound for CSD date is" + "-" + totalCount.ToString());
                        csdProxycounter = 0;
                    }
                    loanId = loanety.Id;
                    if (loanety.Attributes.Contains("ims_primaryloa"))
                    {
                        loanOwnerId = loanety.GetAttributeValue<EntityReference>("ims_primaryloa").Id;
                    }
                    if (!loanety.Attributes.Contains("ims_primaryloa"))
                    {
                        loanOwnerId = loanety.GetAttributeValue<EntityReference>("ownerid").Id;
                    }

                    //if (loanety.Attributes.Contains("ims_closingdate"))
                    //{
                    //    closingDate = loanety.GetAttributeValue<DateTime>("ims_closingdate");
                    //    var currentDate = DateTime.UtcNow;
                    //    csdDate = (closingDate.Date - currentDate.Date).TotalDays;
                    //    DateTime csddt =checkBusinessClosures(organizationService, csdDate.ToString());

                    //}
                    //if (!loanety.Attributes.Contains("ims_csddate") && csd <= 10)
                    //{
                    //CHECK THE CLOSING DATE VALUE HERE

                    string CheckIfTasksExists = @"<fetch top='5000' >
                       <entity name='task' >
                         <attribute name='subject' />
                         <attribute name='description' />
                         <filter>
                           <condition attribute='subject' operator='like' value='%Check on CSD Status%' />
                           <condition attribute='regardingobjectid' operator='eq' value='" + loanId + @"'/>
                         </filter>
                       </entity>
                     </fetch>";
                    EntityCollection ifTaskExists = organizationService.RetrieveMultiple(new FetchExpression(CheckIfTasksExists));
                    if (ifTaskExists.Entities.Count > 0)
                    {
                        //return;
                    }
                    else
                    {
                        fetchXml("Check on CSD Status", organizationService, loanety);
                    }
                }
                //  }
                #endregion

                #region FOR APD DATE STATUS TASK CREATION
                var Todaydt = DateTime.Now.Date.ToString("yyyy/MM/dd");
                var dtVal = checkBusinessClosures(organizationService, "4");
                dtVal = DateTime.Parse(dtVal.Date.ToString("yyyy/MM/dd"));

                var apdXml = @"
                    <fetch {0}>
                      <entity name='opportunity'>
                        <attribute name='ims_closingdate' />
                        <attribute name='name' />
                        <attribute name='opportunityid' />
                        <attribute name='originatingleadid'/>
                        <attribute name='ims_apd' />
                        <attribute name='ims_loanstatus' />
                        <attribute name='ims_submittedstatusdate' />
                        <attribute name='ims_applicationstatusdate' />
                        <attribute name='ims_csddate' />
                        <attribute name='ims_cdsent' />
                        <attribute name='ims_primaryloa' />
                        <attribute name='ownerid' />
                        <filter>
                          <condition attribute='ims_apd' operator='null' />
                          <condition attribute='ims_closingdate' operator='lt' value='" + dtVal + @"' />
                          <condition attribute='ims_closingdate' operator='gt' value='" + Todaydt + @"' />
                          <condition attribute='ims_loanstatus' operator='not-in'>
        <value>C6655E22-C006-EA11-A811-000D3A4E6DC6</value>
        <value>766DA378-2117-EA11-A815-000D3A4ED4E6</value>
      </condition>
                        </filter>
                        <link-entity name='systemuser' from='systemuserid' to='owninguser'>
                          <attribute name='systemuserid' />
                          <filter>
                            <condition attribute='systemuserid' operator='not-null' />
                            <condition attribute='ims_automationrequired' operator='eq' value='1' />
                          </filter>
                        </link-entity>
                      </entity>
                    </fetch>";
                List<Entity> apdLoans = pagingmethod(apdXml);
                int apdProxycounter = 0;
                int apdCount = 0;
                foreach (var loanety in apdLoans)
                {
                    apdProxycounter++;
                    if (apdProxycounter == 300)
                    {
                        apdCount++;
                        getProxy();
                        int totalCount = apdCount * apdProxycounter;
                        Console.WriteLine("The Total Record count for APD date is " + "-" + totalCount.ToString());
                        apdProxycounter = 0;
                    }
                    loanId = loanety.Id;
                    if (loanety.Attributes.Contains("ims_primaryloa"))
                    {
                        loanOwnerId = loanety.GetAttributeValue<EntityReference>("ims_primaryloa").Id;
                    }
                    if (!loanety.Attributes.Contains("ims_primaryloa"))
                    {
                        loanOwnerId = loanety.GetAttributeValue<EntityReference>("ownerid").Id;
                    }
                    //CHECK THE CLOSING DATE VALUE HERE
                    //if (loanety.Attributes.Contains("ims_closingdate"))
                    //{
                    //    closingDate = loanety.GetAttributeValue<DateTime>("ims_closingdate");
                    //    var currentDate = DateTime.UtcNow;
                    //    apdDate = (closingDate.Date - currentDate.Date).TotalDays;
                    //    apd = Convert.ToInt32(checkBusinessClosures(organizationService, apdDate.ToString()));
                    //}
                    //if (!loanety.Attributes.Contains("ims_apd") && apd <= 4)
                    //{
                    string CheckIfTasksExists = @"<fetch top='5000' >
                          <entity name='task' >
                            <attribute name='subject' />
                            <attribute name='description' />
                            <filter>
                              <condition attribute='subject' operator='like' value='%Check on APD Status%' />
                              <condition attribute='regardingobjectid' operator='eq' value='" + loanId + @"'/>
                            </filter>
                          </entity>
                        </fetch>";
                    EntityCollection ifTaskExists = organizationService.RetrieveMultiple(new FetchExpression(CheckIfTasksExists));
                    if (ifTaskExists.Entities.Count > 0)
                    {
                        // return;
                    }
                    else
                    {
                        fetchXml("Check on APD Status", organizationService, loanety);
                    }
                }
                //}

                #endregion

                #region FORLOAN STATUS APPROVED & CD SEND DATE IS BLANK AND REQUESTED CLOSE DATE WITHIN 10 DAYS
                var todayDate = DateTime.Now.Date.ToString("yyyy/MM/dd");//ToShortDateString();
                var greaterDate = checkBusinessClosures(organizationService, "10").ToString("yyyy/MM/dd");
                string approvedXml = @"
                      <fetch {0}>
                        <entity name='opportunity'>
                          <attribute name='ims_closingdate' />
                          <attribute name='originatingleadid'/>
                          <attribute name='name' />
                          <attribute name='opportunityid' />
                          <attribute name='ims_apd' />
                          <attribute name='ims_loanstatus' />
                          <attribute name='ims_submittedstatusdate' />
                          <attribute name='ims_applicationstatusdate' />
                          <attribute name='ims_csddate' />
                          <attribute name='ims_cdsent' />
                          <attribute name='ims_primaryloa' />
                          <attribute name='ownerid' />
                          <filter>
                            <condition attribute='ims_loanstatus' operator='eq' uiname='Approved' uitype='ims_loanstatus' value='9f59a541-9a07-ea11-a811-000d3a4f62e7' />
                            <condition attribute='ims_cdsent' operator='null' />
                            <condition attribute='ims_closingdate' operator='lt' value='" + greaterDate + @"' />
                            <condition attribute='ims_closingdate' operator='gt' value='" + todayDate + @"' />
                          </filter>
                          <link-entity name='systemuser' from='systemuserid' to='owninguser'>
                            <attribute name='systemuserid' />
                            <filter>
                              <condition attribute='systemuserid' operator='not-null' />
                             <condition attribute='ims_automationrequired' operator='eq' value='1' />
                            </filter>
                          </link-entity>
                        </entity>
                      </fetch>";
                List<Entity> approvedLoans = pagingmethod(approvedXml);
                foreach (var loanety in approvedLoans)
                {
                    int CDProxycounter = 0;
                    int CDCount = 0;
                    CDProxycounter++;
                    if (CDProxycounter == 300)
                    {
                        CDCount++;
                        getProxy();
                        int totalCount = CDCount * CDProxycounter;
                        Console.WriteLine("The Total Count for Approved and CD sent date Blank is " + "-" + totalCount.ToString());
                        CDProxycounter = 0;
                    }
                    loanId = loanety.Id;
                    if (loanety.Attributes.Contains("ims_primaryloa"))
                    {
                        loanOwnerId = loanety.GetAttributeValue<EntityReference>("ims_primaryloa").Id;
                    }
                    if (!loanety.Attributes.Contains("ims_primaryloa"))
                    {
                        loanOwnerId = loanety.GetAttributeValue<EntityReference>("ownerid").Id;
                    }
                    if (!loanety.Attributes.Contains("ims_cdsent"))
                    {
                        //string loanStatus = loanety.GetAttributeValue<EntityReference>("ims_loanstatus").Name.ToLower();
                        //DateTime cdSent = loanety.GetAttributeValue<DateTime>("ims_cdsent");
                        //if (loanStatus.Equals("approved") && cdSent == null)
                        //{
                        string CheckIfTasksExists = @"<fetch top='5000' >
                        <entity name='task' >
                          <attribute name='subject' />
                          <attribute name='description' />
                          <filter>
                            <condition attribute='subject' operator='like' value='%Is ICD out?%' />
                            <condition attribute='regardingobjectid' operator='eq' value='" + loanety.Id + @"'/>
                          </filter>
                        </entity>
                      </fetch>";
                        EntityCollection ifTaskExists = organizationService.RetrieveMultiple(new FetchExpression(CheckIfTasksExists));
                        if (ifTaskExists.Entities.Count > 0)
                        {
                            //return;
                        }
                        else if (ifTaskExists.Entities.Count == 0)
                        {
                            fetchXml("Approved-Follow-up with Pre-Closer", organizationService, loanety);
                        }
                    }
                }
                // }
                #endregion


                #region Completed all the Tasks after 2 Days when the Loan status is Moved to Funded
                string fundloans = @"<fetch {0}>
  <entity name='opportunity'>
    <attribute name='name' />
    <attribute name='customerid' />
    <attribute name='estimatedvalue' />
    <attribute name='statuscode' />
    <attribute name='statecode' />
    <attribute name='originatingleadid'/>
    <attribute name='opportunityid' />
    <attribute name='ims_loanstatus' />
    <attribute name='ims_fundedstatusdate' />
    <order attribute='name' descending='false' />
    <filter type='and'>
      <condition attribute='ims_loanstatus' operator='eq' uiname='Funded' uitype='ims_loanstatus' value='C6655E22-C006-EA11-A811-000D3A4E6DC6' />
      <condition attribute='ims_fundedstatusdate' operator='not-null' />
    </filter>
  </entity>
</fetch>";
                int proxycounter = 0;
                int count = 0;
                List<Entity> loan = pagingmethod(fundloans);
                foreach (var loanRec in loan)
                {
                    proxycounter++;

                    if (proxycounter == 300)
                    {
                        count++;
                        getProxy();
                        int totalCount = count * proxycounter;
                        Console.WriteLine("The total Count for Funded Status Loan is " + "-" + totalCount.ToString());
                        proxycounter = 0;
                    }
                    loanId = loanRec.Id;
                    if (loanRec.Attributes.Contains("ims_fundedstatusdate"))
                    {
                        DateTime fundedDate = loanRec.GetAttributeValue<DateTime>("ims_fundedstatusdate");
                        fundedDate = checkBusinessClosures_loanStatusDate(organizationService, "2", fundedDate);
                        if (fundedDate.Date <= DateTime.Now.Date)
                        {
                            string tasks = @"<fetch top='5000' >
                                  <entity name='task' >
                                    <attribute name='subject' />
                                    <attribute name='description' />
                                    <attribute name='ims_automatedtask' />
                                    <filter>
                                      <condition attribute='ims_automatedtask' operator='eq' value='1' />
                                      <condition attribute='regardingobjectid' operator='eq' value='" + loanId + @"'/>
                                      <condition attribute='statecode' operator='eq' value='0' />
                                    </filter>
                                  </entity>
                                </fetch>";
                            EntityCollection getTasks = organizationService.RetrieveMultiple(new FetchExpression(tasks));
                            foreach (var task in getTasks.Entities)
                            {
                                Entity taskRec = new Entity("task", task.Id);
                                taskRec["statecode"] = new OptionSetValue(1);
                                organizationService.Update(taskRec);
                            }
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public static void fetchXml(string taskName, IOrganizationService service, Entity loanety)
        {
            string fetchImportDetailsMapping = @"<fetch top='50' >
          <entity name='ims_importdetailsmapping' >
            <attribute name='owningteam' />
            <attribute name='statecode' />
            <attribute name='owneridname' />
            <attribute name='statuscode' />
            <attribute name='statecodename' />
            <attribute name='owninguser' />
            <attribute name='overriddencreatedon' />
            <attribute name='ims_revertresult' />
            <attribute name='ims_sourceentity' />
            <attribute name='ims_importdetailsmappingid' />
            <attribute name='ims_name' />
            <attribute name='ims_targetdatatypename' />
            <attribute name='ims_targetfield' />
            <attribute name='ims_importdatamaster' />
            <attribute name='ims_crmdisplayname' />
            <attribute name='ims_mandatory' />
            <attribute name='owneridtype' />
            <attribute name='statuscodename' />
            <attribute name='ims_lookupentityattribute' />
            <attribute name='ims_defaultvalue' />
            <attribute name='ims_revertresultname' />
            <attribute name='ims_datamastername' />
            <attribute name='owneridyominame' />
            <attribute name='ims_targetdatatype' />
            <attribute name='ims_mandatoryname' />
            <attribute name='ims_lookupentityname' />
            <attribute name='owningbusinessunit' />
            <attribute name='ims_maxlength' />
            <attribute name='ims_sourcedatatype' />
            <attribute name='ims_sourcedatatypename' />
            <attribute name='ims_sourcefield' />
            <attribute name='ownerid' />
            <attribute name='ims_createlookuprecord' />
            <attribute name='ims_importdatamastername' />
            <filter>
              <condition attribute='ims_datamastername' operator='like' value='%" + taskName + @"%' />
                                        </filter>
                                      </entity>
                                    </fetch>";
            EntityCollection ecMappings = service.RetrieveMultiple(new FetchExpression(fetchImportDetailsMapping));
            FetchImportDetailsMappings(ecMappings, service, taskName, loanety);
            //  return ecMappings;
        }
        public static void FetchImportDetailsMappings(EntityCollection ecMappings, IOrganizationService service, string taskName, Entity loanety)
        {
            Console.WriteLine("Inside FetchImportDetailsMappings");

            Console.WriteLine("The Import Details Mappings Count Record is" + ecMappings.Entities.Count.ToString());

            List<Mapping> mappings = new List<Mapping>();
            if (ecMappings != null && ecMappings.Entities.Count > 0)
            {
                foreach (Entity objMapping in ecMappings.Entities)
                {
                    mappings.Add(new Mapping
                    {
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
                Entity taskObject = new Entity("task");
                foreach (Mapping objMapping in mappings)
                {
                    try
                    {
                        Mapping mapping = objMapping;
                        GetValueFromSourceEntity(mapping, taskObject, service, taskName, loanety);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                if (taskObject != null && taskObject.Attributes.Count > 0)
                {
                    Console.WriteLine("Creating the Auto Task Record here");
                    service.Create(taskObject);
                }

            }
        }
        public static void GetValueFromSourceEntity(Mapping mappings, Entity taskObject, IOrganizationService service, string taskName, Entity loanety)
        {
            Console.WriteLine("Inside GetValueFromSourceEntity");
            string owner = null;
            try
            {
                if (mappings.Source != null)
                {
                    if (mappings.SourceDatatype == (int)ImportDetailsMapping.SourceDataType_OptionSet.Lookup)
                    {
                        owner = GetValueFromSourceLookUpField(mappings.Source, service, Target);
                        mappings.value = owner;
                        string id = GetId(mappings.LookupEntityName, mappings.LookupEntityAttribute, mappings, service);
                        taskObject[mappings.Target] = new EntityReference(mappings.LookupEntityName, new Guid(id));
                    }
                    else if (mappings.DataType == (int)ImportDetailsMapping.TargetDataType_OptionSet.SingleLineOfText)
                    {
                        string singlelinetxt = GetValueFromSingleLineText(mappings.Source, service);
                        taskObject[mappings.Target] = singlelinetxt;
                    }
                    else if (mappings.DataType == (int)ImportDetailsMapping.TargetDataType_OptionSet.Optonset)
                    {
                        int val = FetchOptionSetLookupValue(mappings, int.Parse(mappings.Source), service);
                        taskObject[mappings.Target] = new OptionSetValue(val);
                    }
                    else if (mappings.SourceDatatype == (int)ImportDetailsMapping.SourceDataType_OptionSet.DateTime)
                    {
                        DateTime dateVal = GetValueFromDateTime(mappings, mappings.Source, service, Target);
                        taskObject[mappings.Target] = dateVal;
                    }
                    else if (mappings.SourceDatatype == (int)ImportDetailsMapping.SourceDataType_OptionSet.WholeNumber)
                    {
                        int wholeVal = GetValueFromWholenumber(mappings.Source, service, Target);
                        taskObject[mappings.Target] = wholeVal;
                    }
                }
                else if (mappings.Source == null)
                {
                    if (mappings.DataType == (int)ImportDetailsMapping.TargetDataType_OptionSet.Optonset)
                    {
                        int val = FetchOptionSetLookupValue(mappings, int.Parse(mappings.DefaultValue), service);
                        taskObject[mappings.Target] = new OptionSetValue(val);
                    }
                    else if (mappings.DataType == (int)ImportDetailsMapping.TargetDataType_OptionSet.SingleLineOfText)
                    {
                        string firstName = null;
                        string lastName = null;
                        string loanNumber = loanety.GetAttributeValue<string>("name");
                        EntityReference borrowerEty = loanety.GetAttributeValue<EntityReference>("originatingleadid");
                        Entity borrowerVal = organizationService.Retrieve(borrowerEty.LogicalName, borrowerEty.Id, new ColumnSet("firstname", "lastname"));
                        if (borrowerVal.Attributes.Contains("firstname"))
                        {
                            firstName = borrowerVal.GetAttributeValue<string>("firstname");

                        }
                        if (borrowerVal.Attributes.Contains("lastname"))
                        {
                            lastName = borrowerVal.GetAttributeValue<string>("lastname");
                        }
                        mappings.value = mappings.DefaultValue;
                        if (mappings.DefaultValue.Contains("{0}"))
                        {
                            if (firstName != null && lastName != null)
                            {
                                mappings.value = mappings.DefaultValue.Replace("{0}", "-" + " " + firstName + " " + lastName + " " + "(" + loanNumber + ")");
                            }
                            if (firstName == null)
                            {
                                mappings.value = mappings.DefaultValue.Replace("{0}", "-" + " " + lastName + " " + "(" + loanNumber + ")");
                                taskObject[mappings.Target] = mappings.value;
                            }
                            if (lastName == null)
                            {
                                mappings.value = mappings.DefaultValue.Replace("{0}", "-" + " " + firstName + " " + "(" + loanNumber + ")");
                                taskObject[mappings.Target] = mappings.value;
                            }
                        }
                        taskObject[mappings.Target] = mappings.value;
                    }
                    else if (mappings.DataType == (int)ImportDetailsMapping.TargetDataType_OptionSet.Lookup)
                    {
                        owner = GetValueFromSourceLookUpField(mappings.Target, service, Target);
                        mappings.value = owner;
                        string id = GetId(mappings.LookupEntityName, mappings.LookupEntityAttribute, mappings, service);
                        taskObject[mappings.Target] = new EntityReference(mappings.LookupEntityName, new Guid(id));
                    }
                    else if (mappings.DataType == (int)ImportDetailsMapping.SourceDataType_OptionSet.DateTime)
                    {
                        DateTime? dateAndTime1 = null;
                        mappings.value = mappings.DefaultValue;
                        if (mappings.value == "0")
                        {
                            DateTime dateVal = checkBusinessClosures(service, mappings.value);
                            DateTime today = DateTime.UtcNow.Date;
                            int d = (dateVal.Date - today).Days;
                            dateAndTime1 = DateTime.UtcNow.AddDays(d);
                            mappings.value = dateAndTime1.ToString();
                        }
                        else
                        {
                            DateTime dateVal = checkBusinessClosures(service, mappings.value);
                            Console.WriteLine(dateVal.ToUniversalTime().ToString());
                            DateTime today = DateTime.UtcNow.Date;
                            int d = (dateVal.Date - today).Days;
                            dateAndTime1 = DateTime.UtcNow.AddDays(d);
                            Console.WriteLine(mappings.value);
                            mappings.value = dateAndTime1.ToString();
                        }
                        Console.WriteLine(mappings.value);

                        taskObject[mappings.Target] = dateAndTime1; //Convert.ToDateTime(dateAndTime1);

                    }
                    else if (mappings.DataType == (int)ImportDetailsMapping.SourceDataType_OptionSet.WholeNumber)
                    {
                        int wholeval = GetValueFromWholenumber(mappings.Target, service, Target);
                        taskObject[mappings.Target] = mappings.value;
                    }
                }

                //for lookup values we are setting value here
                taskObject[Task.AutomatedTask] = true;
                taskObject[Task.RegardingObjectId] = new EntityReference("opportunity", loanId);
                taskObject[Task.OwnerId] = new EntityReference("systemuser", loanOwnerId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        public static int GetValueFromWholenumber(string field, IOrganizationService service, Entity Target)
        {
            Console.WriteLine("Inside GetValueFromWholenumber");
            int wholenumber;
            Entity loan = service.Retrieve("opportunity", Target.Id, new ColumnSet(field));
            wholenumber = loan.GetAttributeValue<int>(field);
            return wholenumber;
        }
        public static string GetValueFromSingleLineText(string field, IOrganizationService service)
        {
            Console.WriteLine("Inside GetValueFromSingleLineText");
            Entity loan = service.Retrieve("opportunity", new Guid("DE1F885C-4D85-EA11-A811-000D3A30F195"), new ColumnSet(field));
            string value = loan.GetAttributeValue<string>(field);
            return value;
        }
        public static string GetValueFromSourceLookUpField(string field, IOrganizationService service, Entity Target)
        {
            Console.WriteLine("Inside GetValueFromSourceLookUpField");
            Entity loanety = service.Retrieve("opportunity", Target.Id, new ColumnSet(field));
            string ownerName = loanety.GetAttributeValue<EntityReference>(field).Name;
            return ownerName;
        }
        public static int FetchOptionSetLookupValue(Mapping objMapping, int optionsetVal, IOrganizationService service)
        {
            Console.WriteLine("Inside FetchOptionSetLookupValue");
            int val = 11;
            try
            {
                if (optionsetVal == (int)Task.PriorityCode_OptionSet.Low)
                {
                    val = (int)Task.PriorityCode_OptionSet.Low;
                }
                else if (optionsetVal == (int)Task.PriorityCode_OptionSet.High)
                {
                    val = (int)Task.PriorityCode_OptionSet.High;
                }
                else if (optionsetVal == (int)Task.PriorityCode_OptionSet.Normal)
                {
                    val = (int)Task.PriorityCode_OptionSet.Normal;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching lookupOption: " + ex.Message);
            }
            return val;
        }
        public static DateTime GetValueFromDateTime(Mapping mappings, string field, IOrganizationService service, Entity Target)
        {
            Console.WriteLine("Inside GetValueFromDateTime");
            DateTime value;
            Entity loanety = service.Retrieve("opportunity", Target.Id, new ColumnSet(field));
            value = loanety.GetAttributeValue<DateTime>(field);
            Console.WriteLine(value.ToString());
            value = checkBusinessClosures(service, value.ToString());
            return value;
        }
        public static string GetId(string lookupEntityName, string filterAttributeName, Mapping mapping, IOrganizationService service)
        {
            Console.WriteLine("Inside GetId");
            string filterAttributeValue = mapping.value;
            lookupEntityName = lookupEntityName.Trim();
            Guid entityId = Guid.Empty;
            try
            {
                var fetchXml = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
               "<entity name='" + lookupEntityName + "'>" +
               "<attribute name='" + lookupEntityName + "id" + "' />" +
               "<filter type='and'>" +
                 "<condition attribute='" + filterAttributeName + "' operator='eq' value='" + filterAttributeValue + "'/>"
                 +
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                entityId = Guid.Empty;
            }
            return (entityId == Guid.Empty ? string.Empty : entityId.ToString());
        }
        public static DateTime checkBusinessClosures(IOrganizationService organizationService, string datetimeVal)
        {
            //Check Business closure Dates and WeekEnd Dates here.
            QueryExpression calenderQuery = new QueryExpression
            {
                EntityName = "calendar",
                ColumnSet = new ColumnSet(true),
                Criteria =
                                    {
                                        Conditions =
                                        {
                                            new ConditionExpression("name", ConditionOperator.Equal, "Business Closure Calendar")
                                        }
                                    }
            };

            EntityCollection calendar = organizationService.RetrieveMultiple(calenderQuery);

            DateTime requiredDateTime = DateTime.UtcNow;
            int daysToAdd = int.Parse(datetimeVal);
            while (daysToAdd > 0)
            {
                requiredDateTime = requiredDateTime.AddDays(1);
                if (isWorkingDay(calendar, requiredDateTime))
                {
                    daysToAdd--;
                }
            }
            Console.WriteLine("The Required Datetime is" + " " + requiredDateTime);
            return requiredDateTime;
        }

       
        public static bool isWorkingDay(EntityCollection calendarrules, DateTime time)
        {
            if (time.DayOfWeek == DayOfWeek.Saturday || time.DayOfWeek == DayOfWeek.Sunday)
            {
                return false;
            }
            if (calendarrules.Entities.Count > 0)
            {
                var entity = calendarrules.Entities[0];
                EntityCollection businessClosureRules = (EntityCollection)calendarrules.Entities[0].Attributes["calendarrules"];

                foreach (Entity rule in businessClosureRules.Entities)
                {
                    if ((DateTime)rule.Attributes["effectiveintervalstart"] <= time && (DateTime)rule.Attributes["effectiveintervalend"] > time)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        #region Share Records to LOA2 and LOA3 here
        public static void shareRecords(EntityReference userRec, Entity relatedRec)
        {
            /*try
            {
                GrantAccessRequest grant = new GrantAccessRequest();
                grant.Target = new EntityReference(relatedRec.LogicalName, relatedRec.Id);
                PrincipalAccess principal = new PrincipalAccess();
                principal.Principal = new EntityReference(userRec.LogicalName, userRec.Id);
                principal.AccessMask = AccessRights.ReadAccess | AccessRights.WriteAccess | AccessRights.AppendAccess;
                grant.PrincipalAccess = principal;
                GrantAccessResponse granted = (GrantAccessResponse)organizationService.Execute(grant);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }*/
        }
        #endregion
        public static IOrganizationService getProxy()
        {
            Console.WriteLine("Connection using proxy method succesful for Dev Instance");
            IOrganizationService _orgService = null;

            try
            {
                string organizationUrl = ConfigurationManager.AppSettings["organizationUrl"].ToString();
                string resourceURL = ConfigurationManager.AppSettings["resourceURL"].ToString();
                string serviceUrl = ConfigurationManager.AppSettings["organizationurl"].ToString();
                Console.WriteLine(organizationUrl + " - " + resourceURL);
                string clientId = GetSecretValue(ConfigurationManager.AppSettings["KeyVaultUrl"].ToString(), "clientId").Result;
                Console.WriteLine("clientId : " + clientId);
                string appKey = GetSecretValue(ConfigurationManager.AppSettings["KeyVaultUrl"].ToString(), "secret").Result;
                Console.WriteLine("appKey : " + appKey);

                string connectionString = "AuthType=ClientSecret;Url='" + serviceUrl + "' ; clientid='" + clientId + "';     clientsecret= '" + appKey + "'; ";
                Console.WriteLine("The ConnectionString value is " + connectionString);
                CrmServiceClient crmConn = new CrmServiceClient(connectionString);
                IOrganizationService crmService = crmConn.OrganizationServiceProxy;

                CrmServiceClient.MaxConnectionTimeout = new TimeSpan(4, 0, 0);

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;


                if (crmConn != null)
                {
                    Guid userid = ((WhoAmIResponse)crmConn.Execute(new WhoAmIRequest())).UserId;
                    Console.WriteLine("UserId is " + userid);
                    if (userid != null)
                    {
                        Console.WriteLine("The Connection is successfull");
                        return crmConn;
                    }
                }
                else
                {
                    Console.WriteLine("Failed to establish connection");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught - " + ex.Message);
            }
            return _orgService;

        }
        public static IOrganizationService getProxy_Static()
        {
            IOrganizationService _orgService = null;
            OrganizationWebProxyClient sdkService;

            try
            {
                //string organizationUrl = "https://mmsitphase2.crm.dynamics.com";
                //string resourceURL = "https://mmsitphase2.api.crm.dynamics.com/api/data/v9.1/";
                //string clientId = "156d5a4b-0618-46cd-9b8c-a70e17d0eba1"; //"dbbfed03-5023-410b-b588-8db70a61c8af";
                //string appKey = "wa1L:.]7=NnBR7l0?sTYn6ZytngidJPs";//"rFzxuRtO20n@BywqSIn_i7Y]5Q]nnAMQ";

                string organizationUrl = "https://mmdevphase2.crm.dynamics.com";
                string resourceURL = "https://mmdevphase2.api.crm.dynamics.com/api/data/v9.1/";
                string clientId = "dbbfed03-5023-410b-b588-8db70a61c8af";///"39bb7c8c-c5c2-46c1-8829-a78d8742822b";
                string appKey = "rFzxuRtO20n@BywqSIn_i7Y]5Q]nnAMQ";// "z4 @b2Y00iXzAbxsmxxTi@OekDP:saZf=";



                string connectionString = "AuthType=ClientSecret;Url='" + organizationUrl + "' ; clientid='" + clientId + "';     clientsecret= '" + appKey + "'; ";
                Console.WriteLine("The ConnectionString value is " + connectionString);
                CrmServiceClient crmConn = new CrmServiceClient(connectionString); //ConfigurationManager.ConnectionStrings["CRM"].ConnectionString
                IOrganizationService crmService = crmConn.OrganizationServiceProxy;

                CrmServiceClient.MaxConnectionTimeout = new TimeSpan(4, 0, 0);

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;


                if (crmConn != null)
                {
                    Guid userid = ((WhoAmIResponse)crmConn.Execute(new WhoAmIRequest())).UserId;
                    Console.WriteLine("UserId is " + userid);
                    if (userid != null)
                    {
                        Console.WriteLine("The Connection is successfull");
                        return crmConn;
                    }
                }
                else
                {
                    Console.WriteLine("Failed to establish connection");
                }

                //string clientId = "156d5a4b-0618-46cd-9b8c-a70e17d0eba1";
                //string appKey = "wa1L:.]7=NnBR7l0?sTYn6ZytngidJPs";


                //Create the Client credentials to pass for authentication
                ClientCredential clientcred = new ClientCredential(clientId, appKey);

                //get the authentication parameters
                AuthenticationParameters authParam = AuthenticationParameters.CreateFromUrlAsync(new Uri(resourceURL)).Result;

                //Generate the authentication context - this is the azure login url specific to the tenant
                // string authority = authParam.Authority;
                string authority = "https://login.microsoftonline.com/095f0976-bf66-4d76-bf29-0fbab0884ecb";

                //request token
                AuthenticationResult authenticationResult = new AuthenticationContext(authority).AcquireTokenAsync(organizationUrl, clientcred).Result;

                //get the token              
                string token = authenticationResult.AccessToken;
                // accessToken = token;

                Uri serviceUrl = new Uri(organizationUrl + @"/xrmservices/2011/organization.svc/web?SdkClientVersion=8.2");
                //HttpWebRequest req = (HttpWebRequest)WebRequest.Create(serviceUrl);
                //req.KeepAlive = false;

                using (sdkService = new OrganizationWebProxyClient(serviceUrl, false))
                {
                    sdkService.HeaderToken = token;

                    _orgService = (IOrganizationService)sdkService != null ? (IOrganizationService)sdkService : null;
                }


    



                // For Dynamics 365 Customer Engagement V9.X, set Security Protocol as TLS12
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
                // Get the URL from CRM, Navigate to Settings -> Customizations -> Developer Resources
                // Copy and Paste Organization Service Endpoint Address URL
                //organizationService = (IOrganizationService)new OrganizationServiceProxy(new Uri("https://mmdevphase2.api.crm.dynamics.com/XRMServices/2011/Organization.svc"),
                // null, clientCredentials, null);

                if (_orgService != null)
                {
                    Guid userid = ((WhoAmIResponse)_orgService.Execute(new WhoAmIRequest())).UserId;

                    if (userid != Guid.Empty)
                    {

                        return _orgService;

                    }
                }
                else
                {
                    Console.WriteLine("Failed to Established Connection!!!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught - " + ex.Message);
            }
            return _orgService;

        }
        public static async Task<string> GetSecretValue(string keyVaultUrl, string secretName)
        {
            string secret = "";
            try
            {
                AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
                var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
                //slow without ConfigureAwait(false)    
                //keyvault should be keyvault DNS Name    
                var secretBundle = await keyVaultClient.GetSecretAsync(keyVaultUrl + secretName).ConfigureAwait(false);
                secret = secretBundle.Value;
                Console.WriteLine("Secret Name:" + secretName + " Value:" + secret);
                return secret;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return secret;
        }

        public static List<Entity> pagingmethod(string leadXml)
        {
            var moreRecords = false;
            int page = 1;
            var cookie = string.Empty;
            EntityCollection allleads = null;
            List<Entity> Entities = new List<Entity>();
            do
            {
                var xml = string.Format(leadXml, cookie);
                allleads = organizationService.RetrieveMultiple(new FetchExpression(xml));

                if (allleads.Entities.Count >= 0) Entities.AddRange(allleads.Entities);

                moreRecords = allleads.MoreRecords;
                if (moreRecords)
                {
                    page++;
                    cookie = string.Format("paging-cookie='{0}' page='{1}'", System.Security.SecurityElement.Escape(allleads.PagingCookie), page);
                }
            } while (moreRecords);

            return Entities;
        }

        public static DateTime checkBusinessClosures_loanStatusDate(IOrganizationService organizationService, string datetimeVal, DateTime loanStatusDate)
        {
            //Check Business closure Dates and WeekEnd Dates here.
            QueryExpression calenderQuery = new QueryExpression
            {
                EntityName = "calendar",
                ColumnSet = new ColumnSet(true),
                Criteria =
                                    {
                                        Conditions =
                                        {
                                            new ConditionExpression("name", ConditionOperator.Equal, "Business Closure Calendar")
                                        }
                                    }
            };

            EntityCollection calendar = organizationService.RetrieveMultiple(calenderQuery);

            DateTime requiredDateTime = loanStatusDate;
            int daysToAdd = int.Parse(datetimeVal);
            while (daysToAdd > 0)
            {
                requiredDateTime = requiredDateTime.AddDays(1);
                if (isWorkingDay(calendar, requiredDateTime))
                {
                    daysToAdd--;
                }
            }
            Console.WriteLine("The Required Datetime is" + " " + requiredDateTime);
            return requiredDateTime;
        }
    }

    internal class Mapping
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
    public static class ImportDetailsMapping
    {
        public const string EntityName = "ims_importdetailsmapping";
        public const string EntityCollectionName = "ims_importdetailsmappings";
        public const string PrimaryKey = "ims_importdetailsmappingid";
        public const string PrimaryName = "ims_name";
        public const string CreateLookupRecordUnresolved = "ims_createlookuprecord";
        public const string CreatedBy = "createdby";
        public const string CreatedByDelegate = "createdonbehalfby";
        public const string CreatedOn = "createdon";
        public const string CrmDisplayName = "ims_crmdisplayname";
        public const string DataMasterName = "ims_datamastername";
        public const string DefaultValue = "ims_defaultvalue";
        public const string ImportDataMaster = "ims_importdatamaster";
        public const string ImportSequenceNumber = "importsequencenumber";
        public const string IsDataMandatoryforallrecords = "ims_mandatory";
        public const string LookupentityAttributeFilterCondition = "ims_lookupentityattribute";
        public const string LookupEntityName = "ims_lookupentityname";
        public const string MaximumLengthAllowed = "ims_maxlength";
        public const string ModifiedByDelegate = "modifiedonbehalfby";
        public const string ModifiedBy = "modifiedby";
        public const string ModifiedOn = "modifiedon";
        public const string Owner = "ownerid";
        public const string owneridtype = "owneridtype";
        public const string OwningBusinessUnit = "owningbusinessunit";
        public const string RecordCreatedOn = "overriddencreatedon";
        public const string RevertResult = "ims_revertresult";
        public const string SourceDataType = "ims_sourcedatatype";
        public const string SourceEntity = "ims_sourceentity";
        public const string SourceField = "ims_sourcefield";
        public const string Status = "statecode";
        public const string StatusReason = "statuscode";
        public const string TargetDataType = "ims_targetdatatype";
        public const string TargetField = "ims_targetfield";
        public const string TimeZoneRuleVersionNumber = "timezoneruleversionnumber";
        public const string UTCConversionTimeZoneCode = "utcconversiontimezonecode";
        public const string VersionNumber = "versionnumber";
        public enum owneridtype_OptionSet
        {
        }
        public enum SourceDataType_OptionSet
        {
            Currency = 8,
            DateTime = 4,
            Decimal = 9,
            Lookup = 5,
            MultipleLineOfText = 1,
            Optonset = 6,
            SingleLineOfText = 2,
            TwoOptions = 7,
            WholeNumber = 3
        }
        public enum Status_OptionSet
        {
            Active = 0,
            Inactive = 1
        }
        public enum StatusReason_OptionSet
        {
            Active = 1,
            Inactive = 2
        }
        public enum TargetDataType_OptionSet
        {
            Currency = 8,
            DateTime = 4,
            Decimal = 9,
            Lookup = 5,
            MultipleLineOfText = 1,
            Optonset = 6,
            SingleLineOfText = 2,
            TwoOptions = 7,
            WholeNumber = 3
        }
    }
    public static class Task
    {
        public const string EntityName = "task";
        public const string OwnerId = "ownerid";
        public const string RegardingObjectId = "regardingobjectid";
        public const string Subject = "subject";
        public const string ScheduledStart = "scheduledstart";
        public const string Description = "description";
        public const string ScheduledEnd = "scheduledend";
        public const string PriorityCode = "prioritycode";
        public const string ActualDurationMinutes = "actualdurationminutes";
        public const string PrimaryKey = "activityid";
        public const string AutomatedTask = "ims_automatedtask";
        public const string StateCode = "statecode";

        public enum PriorityCode_OptionSet
        {
            Low = 0,
            Normal = 1,
            High = 2
        }
    }
}
