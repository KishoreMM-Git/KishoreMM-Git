using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace XRMExtensions
{
    public static partial class EntityReferenceExtensions
    {
        /// <summary>
        /// Retrieve entity from EntityReference
        /// </summary>
        /// <param name="reference">EntityReference</param>
        /// <param name="columns">Columns to retrieve</param>
        /// <param name="service">IOrganizationService</param>
        /// <typeparam name="T">Early bound entity type</typeparam>
        /// <returns></returns>
        public static T ToEntity<T>(this EntityReference reference, ColumnSet columns, IOrganizationService service) where T : Entity
        {
            if (reference == null)
            {
                return null;
            }
            
            return (T)service.Retrieve(reference.LogicalName, reference.Id, columns);
        }
    }
}
