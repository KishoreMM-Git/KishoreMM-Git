using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRMExtensions
{
    public static class ExecutionConfig
    {
        public static List<Entity> ReadExecutionConfig(IOrganizationService service, string configSetupName)
        {
            if (!string.IsNullOrWhiteSpace(configSetupName))
            {
                // Instantiate QueryExpression
                var query = new QueryExpression(AppConfigSetup.EntityName);
                query.TopCount = 1;
                query.ColumnSet.AddColumns(AppConfigSetup.PrimaryName, AppConfigSetup.PrimaryKey);
                query.Criteria.AddCondition(AppConfigSetup.PrimaryName, ConditionOperator.Equal, configSetupName.Trim());
                var appConfigSetup = service.RetrieveMultiple(query);

                // read the configuration for give configuration setup
                if (appConfigSetup.Entities.Count > 0)
                {
                    // Instantiate QueryExpression
                    query = new QueryExpression(Configuration.EntityName);
                    query.ColumnSet.AddColumns(Configuration.PrimaryName, Configuration.AppConfigSetup, Configuration.Description, 
                                               Configuration.Enabled, Configuration.Value, Configuration.ValueMultiline, 
                                               Configuration.ValueType);
                    query.Criteria.AddCondition(Configuration.AppConfigSetup, ConditionOperator.Equal, appConfigSetup.Entities[0].Id);
                    var config = service.RetrieveMultiple(query);

                    // return configuration
                    return config.Entities.ToList();
                }
            }
            return new List<Entity>();
        }

        public static T ReadConfigValue<T>(List<Entity> executionConfig, string configKey, string configSetupName) 
        {
            // retrieve config value for the given key
            if (executionConfig.Count > 0)
            {
                var config = executionConfig.Where(x => Convert.ToString(x[Configuration.PrimaryName]).Equals(configKey, StringComparison.InvariantCultureIgnoreCase))
                                           .FirstOrDefault();

                if (config != null)
                {
                    // return entire config object if requested else return config value
                    if (typeof(T).Name.Equals(typeof(Entity).Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return (T)(config as object);
                    }
                    else
                    {

                        return ((config.Contains(Configuration.Value) ? (T)config[Configuration.Value] :
                               (config.Contains(Configuration.ValueMultiline) ? (T)config[Configuration.ValueMultiline] : default(T))));
                    }
                }
            }

            return default(T);
        }
    }
}
