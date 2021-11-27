using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xrm;
using XRMExtensions;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk.Messages;
using System.ServiceModel;
using System.Text.RegularExpressions;

namespace Infy.MS.Plugins
{
    public class LeadCreatePreValidation : BasePlugin
    {
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            
            if (context == null)
            {
                throw new InvalidPluginExecutionException("localContext");
            }
            //if (context.Depth > 1) { return; }
                        
            var lead = context.GetTargetEntity<Entity>();

            Dictionary<string, string> dcConfigDetails= Common.FetchConfigDetails(Constants.AppConfigSetup, context.SystemOrganizationService);
            
            if (lead.Attributes.Contains(Lead.MobilePhone))
            {
                lead.Attributes[Lead.unFormattedMobilePhone] = formatPhoneNumber(lead.GetAttributeValue<string>(Lead.MobilePhone));
            }
            if (lead.Attributes.Contains("address1_telephone1"))
            {
                lead.Attributes["ims_unformattedhomephone"] = formatPhoneNumber(lead.GetAttributeValue<string>("address1_telephone1"));
            }
            if (lead.Attributes.Contains("telephone1"))
            {
                lead.Attributes["ims_unformattedworkphone"] = formatPhoneNumber(lead.GetAttributeValue<string>("telephone1"));
            }
            if (lead.Attributes.Contains(Lead.MobilePhone) &&( lead.GetAttributeValue<string>(Lead.MobilePhone).Contains("+1")|| lead.GetAttributeValue<string>(Lead.MobilePhone).Contains("+91")))
            {
                lead.Attributes[Lead.MobilePhone] = formatPhoneNumber(lead.GetAttributeValue<string>(Lead.MobilePhone));
            }

            if (lead != null)
            {
                //if (context.MessageName.ToLower() != "create") { return; }
                if (context.MessageName.ToLower() == "create")
                {
                    //lead = (Entity)context.InputParameters["Target"];
                    if (context.PrimaryEntityName.ToLower() != Lead.EntityName) { return; }

                    if (context.PluginExecutionContext.Stage == 10) // pre-validation
                    {
                        try
                        {
                            CheckMovementDirectLeadUsingLeadSource(context, lead, dcConfigDetails);
                            if (lead.Contains(Lead.IsFromLeadStaging) && lead.GetAttributeValue<bool>(Lead.IsFromLeadStaging))
                                ValidateMovementDirectLead(context, lead,dcConfigDetails);
                            if (lead.Contains(Lead.LeadSource))
                            {
                                EntityReference leadsource = lead.GetAttributeValue<EntityReference>(Lead.LeadSource);
                                Entity leadsrc = context.Retrieve(Lead.LeadSource, leadsource.Id, new ColumnSet(true));
                                if (leadsrc != null)
                                {
                                    if (leadsrc.Attributes[LeadSource.PrimaryName].ToString().ToUpper() == Constants.Zillow)
                                    {
                                        if ((!lead.Contains(Lead.FirstName)) || lead.Attributes[Lead.FirstName] == null)
                                        {
                                            lead.Attributes[Lead.FirstName] = lead.Attributes[Lead.LastName].ToString();
                                            if (!lead.Contains(Lead.LastName))
                                                lead.Attributes[Lead.LastName] = null;
                                        }

                                        if (lead.Attributes.Contains(Lead.MobilePhone))
                                        {

                                            string phoneNumber = lead.GetAttributeValue<string>(Lead.MobilePhone);
                                            string unformattedMobilePhone = "";
                                            if (phoneNumber.Contains("(") & phoneNumber.Contains("-"))
                                            {
                                                unformattedMobilePhone = phoneNumber.Replace("(", "");
                                                unformattedMobilePhone = unformattedMobilePhone.Replace(")", "");
                                                unformattedMobilePhone = unformattedMobilePhone.Replace("-", "");
                                                unformattedMobilePhone = unformattedMobilePhone.Replace(" ", "");
                                            }
                                            lead.Attributes[Lead.unFormattedMobilePhone] = unformattedMobilePhone;
                                        }

                                        if (lead.Contains(Lead.Website))
                                        {
                                            QueryExpression query = new QueryExpression(SystemUser.EntityName);
                                            query.ColumnSet = new ColumnSet(SystemUser.ZillowAgent);
                                            query.Criteria.AddCondition(SystemUser.ZillowWebsite, ConditionOperator.Equal, lead.Attributes[Lead.Website].ToString());
                                            var users = context.RetrieveMultiple(query);

                                            if (users.Entities.Count != 0)
                                            {
                                                var user = users[0];
                                                user.Attributes[SystemUser.ZillowAgent] = lead.Attributes[Lead.ZillowContactAgentID].ToString();
                                                context.Update(user);
                                                lead.Attributes[Lead.Owner] = new EntityReference(SystemUser.EntityName, user.Id);
                                            }
                                        }


                                        ////Zillow Property 
                                        //Entity ZillowProperty = new Entity("ims_property");
                                        //ZillowProperty.Attributes["ims_name"] = (lead.Contains("ims_zillowpropertycity")) ? lead.Attributes["ims_zillowpropertycity"].ToString() : null;
                                        //ZillowProperty.Attributes["ims_city"] = (lead.Contains("ims_zillowpropertycity")) ? lead.Attributes["ims_zillowpropertycity"].ToString() : null;
                                        //ZillowProperty.Attributes["ims_state"] = (lead.Contains("ims_zillowpropertystate")) ? lead.Attributes["ims_zillowpropertystate"].ToString() : null;
                                        //ZillowProperty.Attributes["ims_zip"] = (lead.Contains("ims_zillowpropertyzip")) ? lead.Attributes["ims_zillowpropertyzip"].ToString() : null;

                                        //Guid PropertyID = service.Create(ZillowProperty);
                                        //lead.Attributes["ims_property"] = new EntityReference("ims_property", PropertyID);
                                    }
                                    else if (leadsrc.Attributes[LeadSource.PrimaryName].ToString().ToUpper() == Constants.EmployeeLoan.ToUpper())
                                    {
                                        String name = lead.GetAttributeValue<String>(Lead.FirstName);
                                        String[] names = name.Split(' ');
                                        String lastname = names[names.Length - 1];
                                        var restname = names.Take(names.Length - 1);
                                        String firstname = string.Join(" ", restname);
                                        lead.Attributes[Lead.FirstName] = firstname;
                                        lead.Attributes[Lead.LastName] = lastname;
                                        //Money money = lead.GetAttributeValue<Money>(Lead.DownPaymentAmount);
                                        if (lead.Contains(Lead.ZillowPropertyListingStatus))
                                        {
                                            String Employeeloanpurpose = lead.GetAttributeValue<String>(Lead.ZillowPropertyListingStatus);

                                            QueryExpression query = new QueryExpression(LoanPurpose.EntityName);
                                            query.ColumnSet = new ColumnSet(LoanPurpose.PrimaryName);
                                            query.Criteria.AddCondition(LoanPurpose.PrimaryName, ConditionOperator.Equal, Employeeloanpurpose);
                                            var loanpurposes = context.RetrieveMultiple(query);

                                            if (loanpurposes.Entities.Count != 0)
                                            {
                                                var loanpurpose = loanpurposes[0];
                                                lead.Attributes[Lead.LoanPurpose] = new EntityReference(LoanPurpose.EntityName, loanpurpose.Id);

                                            }
                                        }
                                        String zipcode = "";
                                        String city = "";
                                        String Addressline1 = "";
                                        String Addressline2 = "";
                                        String PropertyFound = "";
                                        if (lead.Contains(Lead.ZillowPropertyState))
                                        {
                                            PropertyFound=lead.GetAttributeValue<String>(Lead.ZillowPropertyState);
                                        }
                                        if (PropertyFound == "Yes")
                                        {
                                            if (lead.Contains(Lead.ZillowPropertyStreetAddress))
                                            {
                                                String stateName = "";
                                                String Address = lead.GetAttributeValue<String>(Lead.ZillowPropertyStreetAddress);
                                                String[] listOfAddress = Address.Split(',');
                                                int addresscount = listOfAddress.Length;
                                                if (addresscount > 0)
                                                {
                                                    String testZipCode = listOfAddress[listOfAddress.Length - 1];
                                                    Regex regex = new Regex(@"[0-9]+$");
                                                    bool zipcodeornot = regex.IsMatch(testZipCode);
                                                    if (zipcodeornot)
                                                    {
                                                        zipcode = testZipCode;

                                                    }

                                                    switch (addresscount)
                                                    {
                                                        case 1:
                                                            if (zipcodeornot)
                                                            {
                                                                zipcode = listOfAddress[listOfAddress.Length - 1];
                                                            }
                                                            else
                                                            {
                                                                Addressline1 = listOfAddress[0];
                                                            }
                                                            break;
                                                        case 2:
                                                            if (zipcodeornot)
                                                            {
                                                                zipcode = testZipCode;
                                                                Addressline1 = listOfAddress[listOfAddress.Length - 2];
                                                            }
                                                            else
                                                            {
                                                                Addressline1 = listOfAddress[listOfAddress.Length - 2];
                                                                Addressline2 = listOfAddress[listOfAddress.Length - 1];
                                                            }
                                                            break;

                                                        case 3:
                                                            if (zipcodeornot)
                                                            {
                                                                city = listOfAddress[1];
                                                                Addressline1 = listOfAddress[0];
                                                            }
                                                            else
                                                            {
                                                                city = listOfAddress[2];
                                                                Addressline2 = listOfAddress[1];
                                                                Addressline1 = listOfAddress[0];

                                                            }
                                                            break;
                                                        case 4:
                                                            city = listOfAddress[2];
                                                            Addressline2 = listOfAddress[1];
                                                            Addressline1 = listOfAddress[0];
                                                            break;
                                                        default:
                                                            city = listOfAddress[listOfAddress.Length - 2];
                                                            Addressline2 = listOfAddress[listOfAddress.Length - 3];
                                                            var restofAddress = listOfAddress.Take(listOfAddress.Length - 3);
                                                            Addressline1 = string.Join(",", restofAddress);
                                                            break;
                                                    }

                                                }
                                                if (lead.Contains(Lead.PropertyState))
                                                {
                                                    QueryExpression query1 = new QueryExpression(State.EntityName);
                                                    query1.ColumnSet = new ColumnSet(State.Name);
                                                    query1.Criteria.AddCondition(State.Code, ConditionOperator.Equal, lead.GetAttributeValue<String>(Lead.PropertyState));
                                                    var states = context.RetrieveMultiple(query1);
                                                   
                                                    if (states.Entities.Count != 0)
                                                    {
                                                        var state = states[0];
                                                        stateName = state.GetAttributeValue<String>(State.Name);


                                                    }
                                                }
                                                String propertyname = Addressline1 + " " + Addressline2;
                                                Entity property = new Entity(Property.EntityName);
                                                property[Property.AddressLine1] = Addressline1;
                                                property[Property.AddressLine2] = Addressline2;
                                                property[Property.City] = city;
                                                property[Property.State] = stateName;
                                                property[Property.Zip] = zipcode;
                                                property[Property.PrimaryName] = propertyname;

                                                Guid propertyId = context.Create(property);
                                                EntityReference propertyReference = lead.GetAttributeValue<EntityReference>(Lead.Property);

                                                lead.Attributes[Lead.Property] = new EntityReference(Property.EntityName, propertyId);
                                            }

                                            //lead.Attributes[Lead.PurchaseTimeframe] = Lead.PurchaseTimeframe_OptionSet._0_3Months;
                                            lead.Attributes[Lead.PurchaseTimeframe] = new OptionSetValue(0);

                                        }
                                        else
                                        {
                                            lead.Attributes[Lead.PurchaseTimeframe] = new OptionSetValue(4);
                                        }
                                    }
                                    else if (leadsrc.Attributes[LeadSource.PrimaryName].ToString().ToUpper() == Constants.Realtor.ToUpper())
                                    {
                                        String actualdayphone = lead.GetAttributeValue<String>(Lead.MobilePhone);
                                        if (actualdayphone.Contains("I need"))
                                        {
                                            int indexTo = actualdayphone.IndexOf("I");
                                            actualdayphone = actualdayphone.Substring(0, indexTo);
                                        }
                                        lead.Attributes[Lead.MobilePhone] = actualdayphone;
                                        String unformattedMobilePhone = "";
                                        if (actualdayphone.Contains("(") & actualdayphone.Contains("-"))
                                        {
                                            unformattedMobilePhone = actualdayphone.Replace("(", "");
                                            unformattedMobilePhone = unformattedMobilePhone.Replace(")", "");
                                            unformattedMobilePhone = unformattedMobilePhone.Replace("-", "");
                                            unformattedMobilePhone = unformattedMobilePhone.Replace(" ", "");
                                        }
                                        lead.Attributes[Lead.unFormattedMobilePhone] = unformattedMobilePhone;
                                        string stateName = string.Empty;
                                        if (lead.Contains(Lead.PropertyState))
                                        {
                                            QueryExpression query3 = new QueryExpression(State.EntityName);
                                            query3.ColumnSet = new ColumnSet(State.Name);
                                            query3.Criteria.AddCondition(State.Code, ConditionOperator.Equal, lead.GetAttributeValue<String>(Lead.PropertyState));
                                            var states = context.RetrieveMultiple(query3);
                                            if (states.Entities.Count > 0)
                                            {
                                                stateName = states.Entities.FirstOrDefault().GetAttributeValue<string>(State.Name);
                                            }
                                        }
                                        Entity property = new Entity(Property.EntityName);
                                        if (lead.Contains(Lead.ZillowPropertyStreetAddress))
                                        {
                                            property[Property.AddressLine1] = lead.GetAttributeValue<String>(Lead.ZillowPropertyStreetAddress);
                                            property[Property.PrimaryName] = lead.GetAttributeValue<String>(Lead.ZillowPropertyStreetAddress);
                                        }
                                        if (lead.Contains(Lead.ZillowPropertyCity))
                                        {
                                            property[Property.City] = lead.GetAttributeValue<String>(Lead.ZillowPropertyCity);
                                        }
                                        if (lead.Contains(Lead.ZillowPropertyZip))
                                        {
                                            property[Property.Zip] = lead.GetAttributeValue<String>(Lead.ZillowPropertyZip);
                                        }
                                        if (stateName != "" && !string.IsNullOrEmpty(stateName))
                                        {
                                            property[Property.State] = stateName;
                                        }

                                        //property[Property.AddressLine1] = AddressLine1;
                                        //property[Property.City] = city;
                                        //property[Property.State] = stateName;
                                        //property[Property.Zip] = zipcode;
                                        //property[Property.PrimaryName] = AddressLine1;
                                        if (property.Attributes.Count > 0)
                                        {
                                            Guid propertyId = context.Create(property);
                                            EntityReference propertyReference = lead.GetAttributeValue<EntityReference>(Lead.Property);
                                            lead.Attributes[Lead.Property] = new EntityReference(Property.EntityName, propertyId);
                                        }

                                    }
                                }
                            }
                            else
                            {
                                new Exception("Lead Source is null. Lead Creation failed.");
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new InvalidPluginExecutionException(ex.Message);
                        }
                        //catch (FaultException<OrganizationServiceFault> e)
                        //{ context.Trace(string.Format("Create Lead Plugin Fault Exception: {0}", new[] { e.ToString() })); }
                        //catch (Exception ex)
                        //{ context.Trace(string.Format("Create Lead Plugin Exception: {0}", new[] { ex.ToString() })); }

                    }
                }
                else if (context.MessageName.ToLower() == "update" && lead.Contains(Lead.FirstName) || lead.Contains(Lead.LastName) || lead.Contains(Lead.PersonalEmail) || lead.Contains(Lead.MobilePhone))
                {
                    try
                    {
                        var preImage = context.GetFirstPreImage<Entity>();
                        var entityMix = new Common().GetEntityMix(lead, preImage);
                        ValidateMovementDirectLead(context, entityMix,dcConfigDetails);
                    }
                    catch(Exception ex)
                    {
                        throw new InvalidPluginExecutionException(ex.Message);
                    }
                }

            }
        }

        public string formatPhoneNumber(string number)
        {
            number = number.Replace("+1", "");
            number = number.Replace("+91", "");
            number = number.Replace("(", "");
            number = number.Replace(")", "");
            number = number.Replace("-", "");
            number = number.Replace(" ", "");
            return number;
        }

        public void ValidateMovementDirectLead(IExtendedPluginContext context,Entity lead,Dictionary<string,string> dcConfigDetails)
        {

            var xmlBusinessUnit = context.GetConfigValue<string>(Constants.MovementDirectBusinessUnit, Constants.AppConfigSetup);
            if (xmlBusinessUnit != string.Empty)
            {
                var isMoverDirectAppCreationUsingBU = new Common().CheckMovementDirectBusinessUnit(xmlBusinessUnit, context.InitiatingUserId, context.SystemOrganizationService);
                if (isMoverDirectAppCreationUsingBU)
                {
                    CheckDuplicateLeadRecord(lead, context,dcConfigDetails);
                }
            }
        }
        public void CheckDuplicateLeadRecord(Entity lead,IExtendedPluginContext context,Dictionary<string,string> dcConfigDetails)
        {
            if (context.MessageName.ToLower().Equals("create"))
                lead.Attributes[Lead.OrginatedByMovementDirect] = true;
            dcConfigDetails = Common.FetchConfigDetails(Constants.AppConfigSetup, context.SystemOrganizationService);
            var duplicateMovementDirectLead = new Common().CheckMovementDirectDuplicateLead(lead, context.SystemOrganizationService, dcConfigDetails);
            if (duplicateMovementDirectLead != null)
            {
                throw new InvalidPluginExecutionException("Duplicate Movement Direct Lead exists with same details");
            }
        }
        public void CheckMovementDirectLeadUsingLeadSource(IExtendedPluginContext context,Entity lead,Dictionary<string,string> dcConfigDetails)
        {
            if (lead.Contains(Lead.LeadSource))
            {
                var ec = new EntityCollection();
                var xmlLeadSource = context.GetConfigValue<string>(Constants.MovementDirectLead_Identification_LeadSource, Constants.AppConfigSetup);
                var xmlParentLeadSource = context.GetConfigValue<string>(Constants.MovementDirectLead_Identification_ParentLeadSource, Constants.AppConfigSetup);
                if (!string.IsNullOrEmpty(xmlLeadSource) && xmlLeadSource.Contains("{ims_leadsourceguid}"))
                {
                    xmlLeadSource = xmlLeadSource.Replace("ims_leadsourceguid", (lead.GetAttributeValue<EntityReference>(Lead.LeadSource)).Id.ToString());
                    ec = context.RetrieveMultiple(new FetchExpression(xmlLeadSource));
                    if (ec.Entities.Count > 0)
                    {
                        if (ec.Entities.FirstOrDefault().Contains(LeadSource.LoanOfficer))
                        {
                            var loanOfficer = ec.Entities.FirstOrDefault().GetAttributeValue<EntityReference>(LeadSource.LoanOfficer);
                            var isValidUser = new Common().SystemuserValidation(loanOfficer.Id.ToString(), context, dcConfigDetails);
                            if(isValidUser)
                            {
                                var loExternalId = new Common().GetLOExternalId(loanOfficer, context.SystemOrganizationService);
                                if (!string.IsNullOrEmpty(loExternalId))
                                    lead[Lead.LOExternalId] = loExternalId;
                                lead[Lead.Owner] = loanOfficer;
                            }
                        }
                        //return true;
                    }
                }
                //if (!string.IsNullOrEmpty(xmlParentLeadSource) && xmlParentLeadSource.Contains("{ims_leadsourceidguid}"))
                //{
                //    xmlParentLeadSource = xmlParentLeadSource.Replace("ims_leadsourceidguid", (lead.GetAttributeValue<EntityReference>(Lead.LeadSource)).Id.ToString());
                //    ec = context.RetrieveMultiple(new FetchExpression(xmlParentLeadSource));
                //    if (ec.Entities.Count > 0)
                //        return true;
                //}
            }
            //return false;
        }
        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PreValidation, SdkMessageProcessingStepMode.Synchronous, "Create");
            yield return new Xrm.RegisteredEvent(PipelineStage.PreValidation, SdkMessageProcessingStepMode.Synchronous, "Update");
        }
    }
    
}


