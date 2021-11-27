// *********************************************************************
// Created by: Latebound Constant Generator 1.2018.9.2 for XrmToolBox
// Author    : Jonas Rapp http://twitter.com/rappen
// Repo      : https://github.com/rappen/LateboundConstantGenerator
// Source Org: https://mmdev.crm.dynamics.com/
// Filename  : C:\Users\amit.vaidya01\Downloads\Contants\Product.cs
// Created   : 2019-10-23 14:12:31
// *********************************************************************

namespace XRMExtensions
{
    public static class Product
    {
        public const string EntityName = "product";
        public const string PrimaryKey = "productid";
        public const string PrimaryName = "name";
        public const string DeprecatedStageId = "stageid";
        public const string DeprecatedTraversedPath = "traversedpath";
        public const string CreatedBy = "createdby";
        public const string CreatedByDelegate = "createdonbehalfby";
        public const string CreatedByExternalParty = "createdbyexternalparty";
        public const string CreatedOn = "createdon";
        public const string Currency = "transactioncurrencyid";
        public const string CurrentCostBase = "currentcost_base";
        public const string CurrentCost = "currentcost";
        public const string DecimalsSupported = "quantitydecimal";
        public const string DefaultPriceList = "pricelevelid";
        public const string DefaultUnit = "defaultuomid";
        public const string Description = "description";
        public const string entityimageid = "entityimageid";
        public const string ExchangeRate = "exchangerate";
        public const string HierarchyPath = "hierarchypath";
        public const string ImportSequenceNumber = "importsequencenumber";
        public const string IsKit = "iskit";
        public const string IsReparented = "isreparented";
        public const string ListPriceBase = "price_base";
        public const string ListPrice = "price";
        public const string ModifiedByDelegate = "modifiedonbehalfby";
        public const string ModifiedByExternalParty = "modifiedbyexternalparty";
        public const string ModifiedBy = "modifiedby";
        public const string ModifiedOn = "modifiedon";
        public const string OrganizationId = "organizationid";
        public const string Parent = "parentproductid";
        public const string ProcessId = "processid";
        public const string ProductID = "productnumber";
        public const string ProductStructure = "productstructure";
        public const string ProductType = "producttypecode";
        public const string QuantityOnHand = "quantityonhand";
        public const string RecordCreatedOn = "overriddencreatedon";
        public const string Size = "size";
        public const string StandardCostBase = "standardcost_base";
        public const string StandardCost = "standardcost";
        public const string Status = "statecode";
        public const string StatusReason = "statuscode";
        public const string StockItem = "isstockitem";
        public const string StockVolume = "stockvolume";
        public const string StockWeight = "stockweight";
        public const string Subject = "subjectid";
        public const string SupplierName = "suppliername";
        public const string TimeZoneRuleVersionNumber = "timezoneruleversionnumber";
        public const string UnitGroup = "defaultuomscheduleid";
        public const string URL = "producturl";
        public const string UTCConversionTimeZoneCode = "utcconversiontimezonecode";
        public const string ValidFrom = "validfromdate";
        public const string ValidTo = "validtodate";
        public const string Vendor = "vendorname";
        public const string VendorID = "vendorid";
        public const string VendorName = "vendorpartnumber";
        public const string VersionNumber = "versionnumber";
        public enum ProductStructure_OptionSet
        {
            Product = 1,
            ProductFamily = 2,
            ProductBundle = 3
        }
        public enum ProductType_OptionSet
        {
            SalesInventory = 1,
            MiscellaneousCharges = 2,
            Services = 3,
            FlatFees = 4
        }
        public enum Status_OptionSet
        {
            Active = 0,
            Retired = 1,
            Draft = 2,
            UnderRevision = 3
        }
        public enum StatusReason_OptionSet
        {
            Active = 1,
            Retired = 2,
            Draft = 0,
            UnderRevision = 3
        }
    }
}
