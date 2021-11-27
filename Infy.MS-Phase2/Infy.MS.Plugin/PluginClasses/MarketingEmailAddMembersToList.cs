using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xrm;
using XRMExtensions;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;

namespace Infy.MS.Plugins
{
    public class MarketingEmailAddMembersToList : BasePlugin
    {
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            if (context == null)
            {
                throw new InvalidPluginExecutionException("Context not found");
            }

            if (context.MessageName != "ims_MarketingEmailAddMemberstoList") { return; }

            #region Variable Declaration
            string primaryEntityName = string.Empty;
            string fetchXml = string.Empty;
            int marketingEmailMemberType = -1;
            string selectedMembers = string.Empty;
            string listId = string.Empty;
            string viewId = string.Empty;
            string viewEntityType = string.Empty;
            Common objCommon = new Common();
            #endregion

            // get primaryEntityName
            if (context.InputParameters.Contains("PrimaryEntityName") && context.InputParameters["PrimaryEntityName"] is string)
            {
                primaryEntityName = (string)context.InputParameters["PrimaryEntityName"];
            }
            // get fetchXml
            if (context.InputParameters.Contains("FetchXml") && context.InputParameters["FetchXml"] is string)
            {
                fetchXml = (string)context.InputParameters["FetchXml"];
            }

            // get marketingEmailMemberType
            if (context.InputParameters.Contains("MarketingEmailMemberType") && context.InputParameters["MarketingEmailMemberType"] is Int32)
            {
                marketingEmailMemberType = (Int32)context.InputParameters["MarketingEmailMemberType"];
            }

            // get selectedMembers
            if (context.InputParameters.Contains("SelectedMembers") && context.InputParameters["SelectedMembers"] is string)
            {
                selectedMembers = (string)context.InputParameters["SelectedMembers"];
            }

            // get selectedMembers
            if (context.InputParameters.Contains("ListId") && context.InputParameters["ListId"] is string)
            {
                listId = (string)context.InputParameters["ListId"];
            }
            // get viewId
            if (context.InputParameters.Contains("ViewId") && context.InputParameters["ViewId"] is string)
            {
                viewId = (string)context.InputParameters["ViewId"];
            }
            // get viewEntityType
            if (context.InputParameters.Contains("ViewEntityType") && context.InputParameters["ViewEntityType"] is string)
            {
                viewEntityType = (string)context.InputParameters["ViewEntityType"];
            }

            if (!string.IsNullOrEmpty(listId))
            {
                Guid marketingListId = Guid.Parse(listId);

                if (marketingEmailMemberType != -1)
                {
                    //Selected Members
                    if (marketingEmailMemberType == 1)
                    {
                        if (selectedMembers.Contains(";"))
                        {
                            List<string> selectedmembersids = selectedMembers.Split(';').Select(s => s.Trim()).ToList();
                            selectedmembersids = selectedmembersids.Distinct().ToList();
                            if (selectedmembersids != null && selectedmembersids.Count > 0)
                            {
                                Guid[] membersx = new Guid[selectedmembersids.Count - 1];
                                int ix = 0;
                                foreach (string item in selectedmembersids)
                                {
                                    if (!string.IsNullOrEmpty(item))
                                    {
                                        if (primaryEntityName == Lead.EntityName || primaryEntityName == Contact.EntityName)
                                        {
                                            membersx[ix] = Guid.Parse(item);
                                            //objCommon.AddMemberToList(marketingListId, Guid.Parse(item), context.OrganizationService);
                                        }
                                        else if (primaryEntityName == Loan.EntityName)
                                        {
                                            Guid borrowerId = GetBorrower(context.OrganizationService, Guid.Parse(item));
                                            if (borrowerId != Guid.Empty)
                                            {
                                                membersx[ix] = borrowerId;
                                                //objCommon.AddMemberToList(marketingListId, borrowerId, context.OrganizationService);
                                            }
                                        }
                                        ix++;
                                    }
                                }
                                if (membersx != null && membersx.Length > 0)
                                    AddMemberToStaticList(context.SystemOrganizationService, membersx, marketingListId);
                            }
                        }
                    }
                    //Members (First Page Only)
                    else if (marketingEmailMemberType == 2)
                    {
                        if (!string.IsNullOrEmpty(fetchXml))
                        {
                            EntityCollection ecmembers = context.RetrieveMultiple(new FetchExpression(fetchXml));
                            if (ecmembers != null && ecmembers.Entities.Count > 0)
                            {
                                Guid[] membersy = new Guid[ecmembers.Entities.Count];
                                int iy = 0;
                                foreach (Entity member in ecmembers.Entities)
                                {
                                    if (member != null)
                                    {
                                        if (primaryEntityName == Lead.EntityName || primaryEntityName == Contact.EntityName)
                                        {
                                            membersy[iy] = member.Id;
                                            //objCommon.AddMemberToList(marketingListId, member.Id, context.OrganizationService);
                                        }
                                        else if (primaryEntityName == Loan.EntityName)
                                        {
                                            Guid borrowerId = GetBorrower(context.OrganizationService, member.Id);
                                            if (borrowerId != Guid.Empty)
                                            {
                                                membersy[iy] = borrowerId;
                                                //objCommon.AddMemberToList(marketingListId, borrowerId, context.OrganizationService);
                                            }
                                        }
                                        iy++;
                                    }
                                }
                                if (membersy != null && membersy.Length > 0)
                                {
                                    AddMemberToStaticList(context.SystemOrganizationService, membersy, marketingListId);
                                }
                            }
                        }

                    }
                    //Entire Members from view
                    else if (marketingEmailMemberType == 3)
                    {
                        string memberFetchXml = string.Empty;
                        if (!string.IsNullOrEmpty(viewId) && !string.IsNullOrEmpty(viewEntityType))
                        {
                            Entity view = context.SystemOrganizationService.Retrieve(viewEntityType, Guid.Parse(viewId), new ColumnSet("fetchxml"));
                            if (view != null)
                            {
                                if (view.Attributes.Contains("fetchxml") && view["fetchxml"] != null)
                                {
                                    memberFetchXml = view["fetchxml"].ToString();
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(memberFetchXml))
                        {
                            EntityCollection ecmembers = context.RetrieveMultiple(new FetchExpression(memberFetchXml));
                            if (ecmembers != null && ecmembers.Entities.Count > 0)
                            {
                                Guid[] membersz = new Guid[ecmembers.Entities.Count];
                                int iz = 0;
                                foreach (Entity member in ecmembers.Entities)
                                {
                                    if (member != null)
                                    {
                                        if (primaryEntityName == Lead.EntityName || primaryEntityName == Contact.EntityName)
                                        {
                                            membersz[iz] = member.Id;
                                            //objCommon.AddMemberToList(marketingListId, member.Id, context.OrganizationService);
                                        }
                                        else if (primaryEntityName == Loan.EntityName)
                                        {
                                            Guid borrowerId = GetBorrower(context.OrganizationService, member.Id);
                                            if (borrowerId != Guid.Empty)
                                            {
                                                membersz[iz] = borrowerId;
                                                //objCommon.AddMemberToList(marketingListId, borrowerId, context.OrganizationService);
                                            }
                                        }
                                        iz++;
                                    }
                                }
                                if (membersz != null && membersz.Length > 0)
                                    AddMemberToStaticList(context.SystemOrganizationService, membersz, marketingListId);
                            }
                        }
                    }
                }
            }
        }

        public Guid GetBorrower(IOrganizationService service, Guid loanId)
        {
            Guid borrowerId = Guid.Empty;
            if (loanId != null && loanId != Guid.Empty)
            {
                Entity lead = service.Retrieve(Loan.EntityName, loanId, new ColumnSet(Loan.Borrower));
                if (lead != null)
                {
                    if (lead.Attributes.Contains(Loan.Borrower) && lead[Loan.Borrower] != null)
                    {
                        borrowerId = ((EntityReference)lead[Loan.Borrower]).Id;
                    }
                }
            }
            return borrowerId;
        }

        public void AddMemberToStaticList(IOrganizationService service, Guid[] members, Guid listId)
        {
            members = members.Distinct().ToArray();
            members = members.Where(x => x != Guid.Empty).ToArray();
            // Add a list of contacts to the marketing list.
            var addMemberListReq = new AddListMembersListRequest
            {
                MemberIds = members,//new[] { _contactIdList[0], _contactIdList[2] },
                ListId = listId
            };

            service.Execute(addMemberListReq);
        }
        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PostOperation, SdkMessageProcessingStepMode.Asynchronous, "ims_MarketingEmailAddMemberstoList");
        }
    }
}
