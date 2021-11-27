using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System.Net;
using System.ServiceModel.Description;
using System.Xml;
using System.IO;
using System.Configuration;
namespace OccupancyUpdateConsole
{
    class Program
    {
        static IOrganizationService service;
        static void Main(string[] args)
        {

            string Filename = "ErrorLog" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm") + ".txt";
            ConnectToMSCRM(ConfigurationManager.AppSettings["Environment"], Filename);
            //UpdateLoanOccupancy();
            //UpdateLeadOccupancy();
            getGuid(Filename);

        }
        public static void ConnectToMSCRM(string OrgUri, string Filename)
        {
            try
            {
                Console.WriteLine("****************************************************");
                Console.WriteLine("INFY MM CONSOLE APP TO UPDATE OCCUPANCY");
                Console.WriteLine("****************************************************");
                Console.WriteLine("Connecting to Movehome DevPhase2... Please Wait");
                ClientCredentials credentials = new ClientCredentials();
                credentials.UserName.UserName = ConfigurationManager.AppSettings["Username"];
                credentials.UserName.Password = ConfigurationManager.AppSettings["Password"];
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
                        Console.WriteLine("Connection Established Successfully... Enter to Proceed");
                        Console.ReadLine();
                        WriteLog(Filename, "Connection Established Successfully");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while connecting to CRM " + ex.Message);
                WriteLog(Filename, "Error while connecting to CRM");
                Console.ReadLine();
            }
        }
        public static void UpdateLoanOccupancy()
        {

            string fetchXml = @"<fetch distinct='false' mapping='logical' output-format='xml-platform' version='1.0' >" +
                                  "<entity name='opportunity' >" +
                                    "<attribute name='name' />" +
                                    "<attribute name='opportunityid' />" +
                                    "<order descending='false' attribute='name' />" +
                                    "<filter type='and' >" +
                                    //"<condition attribute ='name' value= '33265279' operator='eq'/>" +
                                    //33265229
                                    //33265279
                                    "<condition attribute='ims_occupancy' operator='not-null'/>" +
                                   //"<condition attribute='ims_occupancy' value='Investment' operator='eq' />"+
                                   "<condition attribute ='ownerid' operator='eq' value='{6D6D9D8F-2652-EA11-A815-000D3A33E825}'/>" +

                         "</filter>" +
                                  "</entity>" +
                                "</fetch>";
            //1680783
            //"<condition attribute='" + attributetype + "' value='{1C56C27C-0D6A-EA11-A813-000D3A1AB3C8}' uitype='contact' uiname='William Brown' operator='eq' />" +
            FetchExpression fetchExpression = new FetchExpression(fetchXml);
            EntityCollection ec = service.RetrieveMultiple(fetchExpression);
            Console.WriteLine(ec.Entities.Count);
            //Console.ReadLine();
            if (ec.Entities.Count > 0)
            {

                foreach (var en in ec.Entities)
                {
                    Entity loan = new Entity(en.LogicalName);
                    loan.Id = en.Id;
                    //loan.Attributes["ims_loanatriskstatus"] = "";
                    loan.Attributes["ims_occupancy"] = "Second Home";
                    //loan.Attributes["ims_occupancyoptionset"] = null;
                    service.Update(loan);
                    Console.WriteLine("Updated Record");
                    Console.ReadLine();
                }
            }

        }

        public static void UpdateLoanOccupancyOptionset(String userId, string Filename)
        {
            try
            {
                int totalRecord = 0;
                int updatedRecordCount = 0;
                int fetchCount = 5000;
                //int fetchCount1 = ConfigurationManager.AppSettings["FetchCount"].To;
                // Initialize the page number.
                int pageNumber = 1;
                // Initialize the number of records.
                int recordCount = 0;
                //progressBar1.Minimum = 0;
                //progressBar1.Step = 1;
                // Specify the current paging cookie. For retrieving the first page, 
                // pagingCookie should be null.
                string pagingCookie = null;
                string fetchXml = @"<fetch distinct='false' mapping='logical' output-format='xml-platform' version='1.0' >" +
                                    "<entity name='opportunity' >" +
                                      "<attribute name='name' />" +
                                      "<attribute name='opportunityid' />" +
                                       "<attribute name='ims_occupancy' />" +
                                      "<order descending='false' attribute='name' />" +
                                      "<filter type='and' >" +
                                      "<condition attribute='ims_occupancy' operator='not-null'/>" +
                                      "<condition attribute ='ownerid' operator='eq' value='" + userId + "'/>" +
                                      "</filter>" +
                                      "</entity>" +
                                      "</fetch>";

                //FetchExpression fetchExpression = new FetchExpression(fetchXml);
                //var collection = service.RetrieveMultiple(fetchExpression);
                while (true)
                {
                    // Build fetchXml string with the placeholders.
                    string xml = CreateXml(fetchXml, pagingCookie, pageNumber, fetchCount);

                    // Excute the fetch query and get the xml result.
                    RetrieveMultipleRequest fetchRequest1 = new RetrieveMultipleRequest
                    {
                        Query = new FetchExpression(xml)
                    };

                    EntityCollection returnCollection = ((RetrieveMultipleResponse)service.Execute(fetchRequest1)).EntityCollection;
                    if (returnCollection.Entities.Count > 0)
                    {


                        totalRecord = totalRecord + returnCollection.Entities.Count;
                        //progressBar1.Maximum = totalRecord;
                        Console.WriteLine("Total Record" + totalRecord);
                        var message = "Total Record:" + totalRecord;
                        WriteLog(Filename, message);
                        //Console.ReadLine();
                        foreach (var c in returnCollection.Entities)
                        {
                            string occupancyVal = string.Empty;
                            occupancyVal = c.GetAttributeValue<string>("ims_occupancy");
                            occupancyVal = occupancyVal.Trim();
                            int occupancyOptVal = 0;
                            switch (occupancyVal)
                            {
                                case "Owner":
                                    occupancyOptVal = 176390000;
                                    break;
                                case "Second Home":
                                    occupancyOptVal = 176390001;
                                    break;
                                case "Investment":
                                    occupancyOptVal = 176390002;
                                    break;
                                default:
                                    occupancyOptVal = 100;
                                    break;
                            }
                            //176390000 = "Primary Residence";
                            //176390001-Second Home
                            //176390002-Investment Property


                            Entity loan = new Entity(c.LogicalName);
                            loan.Id = c.Id;
                            //loan.Attributes["ims_loanatriskstatus"] = "Test Occupancy1";
                            if (occupancyOptVal != 100)
                            {
                                Console.WriteLine("Updating For Loan Record: " + loan.Id);
                                var printmsg = "For Loan Record:" + loan.Id;
                                WriteLog(Filename, printmsg);
                                loan.Attributes["ims_occupancyoptionset"] = new OptionSetValue(occupancyOptVal);
                                service.Update(loan);
                                updatedRecordCount = updatedRecordCount + 1;
                                Console.WriteLine("Updated Record-" + updatedRecordCount + " for " + loan.Id);
                                var message1 = "Updated Record-" + updatedRecordCount + " for " + loan.Id;
                                WriteLog(Filename, message1);
                            }
                            //Console.ReadLine();
                        }
                    }

                    // Check for morerecords, if it returns 1.
                    if (returnCollection.MoreRecords)
                    {
                        //Console.WriteLine("\n****************\nPage number {0}\n****************", pageNumber);
                        //Console.WriteLine("#\tAccount Name\t\t\tEmail Address");

                        // Increment the page number to retrieve the next page.
                        pageNumber++;

                        // Set the paging cookie to the paging cookie returned from current results.                            
                        pagingCookie = returnCollection.PagingCookie;
                    }
                    else
                    {
                        // If no more records in the result nodes, exit the loop.
                        break;
                    }
                }
                //return new Tuple<int, int>(totalRecord, updatedRecordCount);
                Console.WriteLine("Updated Record Count" + updatedRecordCount);
                var message2 = "Updated Record Count" + updatedRecordCount;
                WriteLog(Filename, message2);
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occured in UpdateLoanOccupancyOptionset:" + ex.Message);
                Console.ReadLine();
                WriteLog(Filename, ex.Message);
            }
        }

        public static string CreateXml(string xml, string cookie, int page, int count)
        {
            StringReader stringReader = new StringReader(xml);
            XmlTextReader reader = new XmlTextReader(stringReader);

            // Load document
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);

            return CreateXml(doc, cookie, page, count);
        }

        public static string CreateXml(XmlDocument doc, string cookie, int page, int count)
        {
            XmlAttributeCollection attrs = doc.DocumentElement.Attributes;

            if (cookie != null)
            {
                XmlAttribute pagingAttr = doc.CreateAttribute("paging-cookie");
                pagingAttr.Value = cookie;
                attrs.Append(pagingAttr);
            }

            XmlAttribute pageAttr = doc.CreateAttribute("page");
            pageAttr.Value = System.Convert.ToString(page);
            attrs.Append(pageAttr);

            XmlAttribute countAttr = doc.CreateAttribute("count");
            countAttr.Value = System.Convert.ToString(count);
            attrs.Append(countAttr);

            StringBuilder sb = new StringBuilder(1024);
            StringWriter stringWriter = new StringWriter(sb);

            XmlTextWriter writer = new XmlTextWriter(stringWriter);
            doc.WriteTo(writer);
            writer.Close();

            return sb.ToString();
        }
        public static void getGuid(string Filename)
        {
            try
            {
                QueryByAttribute userEntity = new QueryByAttribute("systemuser");
                userEntity.AddAttributeValue("domainname", ConfigurationManager.AppSettings["domainname"]);
                userEntity.ColumnSet = new ColumnSet("systemuserid");
                EntityCollection entityCollection = service.RetrieveMultiple(userEntity);
                if (entityCollection.Entities.Count > 0)
                {
                    var systemUser = entityCollection.Entities[0];
                    var message = "System user found with id - " + systemUser["systemuserid"].ToString();
                    Console.WriteLine("System user found with id - " + systemUser["systemuserid"].ToString());
                    WriteLog(Filename, message);
                    //Console.ReadLine();
                    //UpdateLoanOccupancyOptionset(systemUser["systemuserid"].ToString(), Filename);
                    UpdateLeadOccupancyOptionset(systemUser["systemuserid"].ToString(), Filename);
                }
                else
                {
                    Console.WriteLine($"System User doesn't exists");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occured in getGuid:" + ex.Message);
                Console.ReadLine();
                WriteLog(Filename, ex.Message);
            }
        }

        public static bool WriteLog(string strFileName, string strMessage)
        {
            try
            {

                //FileStream objFilestream = new FileStream(string.Format("{0}\\{1}", Path.GetTempPath(), strFileName), FileMode.Append, FileAccess.Write);
                FileStream objFilestream = new FileStream(string.Format("{0}\\{1}", ConfigurationManager.AppSettings["FilePath"], strFileName), FileMode.Append, FileAccess.Write);
                StreamWriter objStreamWriter = new StreamWriter((Stream)objFilestream);
                objStreamWriter.WriteLine(strMessage);
                objStreamWriter.Close();
                objFilestream.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static void UpdateLeadOccupancyOptionset(String userId, string Filename)
        {
            try
            {
                int totalRecord = 0;
                int updatedRecordCount = 0;
                int fetchCount = 5000;
                //int fetchCount1 = ConfigurationManager.AppSettings["FetchCount"].To;
                // Initialize the page number.
                int pageNumber = 1;
                // Initialize the number of records.
                int recordCount = 0;
                //progressBar1.Minimum = 0;
                //progressBar1.Step = 1;
                // Specify the current paging cookie. For retrieving the first page, 
                // pagingCookie should be null.
                string pagingCookie = null;
                string fetchXml = @"<fetch distinct='false' mapping='logical' output-format='xml-platform' version='1.0' >" +
                                        "<entity name='lead' >" +
                                        "<attribute name='fullname' />" +
                                        "<attribute name='leadid' />" +
                                        "<attribute name='ims_occupancy' />" +
                                        "<order descending='false' attribute='fullname' />" +
                                        "<filter type='and' >" +
                                        "<condition attribute='ims_occupancy' operator='not-null'/>" +
                                        "<condition attribute ='ownerid' operator='eq' value='" + userId + "'/>" +
                                        "</filter>" +
                                        "<link-entity name='opportunity' alias='ag' link-type='outer' to='leadid' from='originatingleadid'/>" +
                                        "<filter type='and'>" +
                                        "<condition attribute='originatingleadid' operator='null' entityname='ag'/>" +
                                        "</filter>" +
                                        "</entity>" +
                                        "</fetch>";

                //FetchExpression fetchExpression = new FetchExpression(fetchXml);
                //var collection = service.RetrieveMultiple(fetchExpression);
                while (true)
                {
                    // Build fetchXml string with the placeholders.
                    string xml = CreateXml(fetchXml, pagingCookie, pageNumber, fetchCount);

                    // Excute the fetch query and get the xml result.
                    RetrieveMultipleRequest fetchRequest1 = new RetrieveMultipleRequest
                    {
                        Query = new FetchExpression(xml)
                    };

                    EntityCollection returnCollection = ((RetrieveMultipleResponse)service.Execute(fetchRequest1)).EntityCollection;
                    if (returnCollection.Entities.Count > 0)
                    {


                        totalRecord = totalRecord + returnCollection.Entities.Count;
                        //progressBar1.Maximum = totalRecord;
                        Console.WriteLine("Total Record" + totalRecord);
                        var message = "Total Record:" + totalRecord;
                        WriteLog(Filename, message);
                        //Console.ReadLine();
                        foreach (var c in returnCollection.Entities)
                        {
                            string occupancyVal = string.Empty;
                            occupancyVal = c.GetAttributeValue<string>("ims_occupancy");
                            occupancyVal = occupancyVal.Trim();

                            int occupancyOptVal = 0;

                            switch (occupancyVal)
                            {
                                case "Owner":
                                    occupancyOptVal = 176390000;
                                    break;
                                case "Second Home":
                                    occupancyOptVal = 176390001;
                                    break;
                                case "Investment":
                                    occupancyOptVal = 176390002;
                                    break;
                                default:
                                    occupancyOptVal = 100;
                                    break;
                            }
                            //176390000 = "Primary Residence";
                            //176390001-Second Home
                            //176390002-Investment Property
                            Entity lead = new Entity(c.LogicalName);
                            lead.Id = c.Id;
                            if (occupancyOptVal != 100)
                            {
                                Console.WriteLine("Updating For Lead Record: " + lead.Id);
                                var printmsg = "For Lead Record:" + lead.Id;
                                WriteLog(Filename, printmsg);
                                lead.Attributes["ims_occupancyoptionset"] = new OptionSetValue(occupancyOptVal);
                                service.Update(lead);
                                updatedRecordCount = updatedRecordCount + 1;
                                Console.WriteLine("Updated Record-" + updatedRecordCount + " for " + lead.Id);
                                var message1 = "Updated Record-" + updatedRecordCount + " for " + lead.Id;
                                WriteLog(Filename, message1);
                            }
                            //Console.ReadLine();
                        }
                    }

                    // Check for morerecords, if it returns 1.
                    if (returnCollection.MoreRecords)
                    {
                        //Console.WriteLine("\n****************\nPage number {0}\n****************", pageNumber);
                        //Console.WriteLine("#\tAccount Name\t\t\tEmail Address");

                        // Increment the page number to retrieve the next page.
                        pageNumber++;

                        // Set the paging cookie to the paging cookie returned from current results.                            
                        pagingCookie = returnCollection.PagingCookie;
                    }
                    else
                    {
                        // If no more records in the result nodes, exit the loop.
                        break;
                    }
                }
                //return new Tuple<int, int>(totalRecord, updatedRecordCount);
                Console.WriteLine("Updated Record Count" + updatedRecordCount);
                var message2 = "Updated Record Count" + updatedRecordCount;
                WriteLog(Filename, message2);
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occured in UpdateLeadOccupancyOptionset:" + ex.Message);
                Console.ReadLine();
                WriteLog(Filename, ex.Message);
            }
        }
        public static void UpdateLeadOccupancy()
        {

            string fetchXml = @"<fetch distinct='false' mapping='logical' output-format='xml-platform' version='1.0' >" +
                                  "<entity name='lead' >" +
                                    "<attribute name='fullname' />" +
                                    "<attribute name='leadid' />" +
                                    "<order descending='false' attribute='fullname' />" +
                                    "<filter type='and' >" +
                                    //"<condition attribute ='name' value= '33265279' operator='eq'/>" +
                                    //33265229
                                    //33265279
                                    "<condition attribute='ims_occupancy' operator='not-null'/>" +
                                   //"<condition attribute='ims_occupancy' value='Investment' operator='eq' />"+
                                   "<condition attribute ='ownerid' operator='eq' value='{6D6D9D8F-2652-EA11-A815-000D3A33E825}'/>" +

                         "</filter>" +
                          "<link-entity name='opportunity' alias='ag' link-type='outer' to='leadid' from='originatingleadid'/>" +
                        "<filter type='and'>" +
                        "<condition attribute='originatingleadid' operator='null' entityname='ag'/>" +
                        "</filter>" +
                                  "</entity>" +
                                "</fetch>";
            //1680783
            //"<condition attribute='" + attributetype + "' value='{1C56C27C-0D6A-EA11-A813-000D3A1AB3C8}' uitype='contact' uiname='William Brown' operator='eq' />" +
            FetchExpression fetchExpression = new FetchExpression(fetchXml);
            EntityCollection ec = service.RetrieveMultiple(fetchExpression);
            Console.WriteLine(ec.Entities.Count);
            //Console.ReadLine();
            if (ec.Entities.Count > 0)
            {

                foreach (var en in ec.Entities)
                {
                    Entity lead = new Entity(en.LogicalName);
                    lead.Id = en.Id;
                    lead.Attributes["ims_occupancy"] = "Owner";
                    //loan.Attributes["ims_occupancyoptionset"]= new OptionSetValue(176390002);
                    service.Update(lead);
                    Console.WriteLine("Updated Record");
                    Console.ReadLine();
                }
            }

        }
    }
}

