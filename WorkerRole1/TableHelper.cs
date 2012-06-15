using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System.Data.Services.Client;


public class TableHelper
{
    public CloudStorageAccount Account;
    public CloudTableClient TableClient;

    // Constructor - get settings from a hosted service configuration or .NET configuration file.

    public TableHelper(string configurationSettingName, bool hostedService)
    {
        if (hostedService)
        {
            CloudStorageAccount.SetConfigurationSettingPublisher(
                (configName, configSettingPublisher) =>
                {
                    var connectionString = RoleEnvironment.GetConfigurationSettingValue(configName);
                    configSettingPublisher(connectionString);
                }
            );
        }
        else
        {
            CloudStorageAccount.SetConfigurationSettingPublisher(
                (configName, configSettingPublisher) =>
                {
                    var connectionString = ConfigurationManager.ConnectionStrings[configName].ConnectionString;
                    configSettingPublisher(connectionString);
                }
            );
        }

        Account = CloudStorageAccount.FromConfigurationSetting(configurationSettingName);

        TableClient = Account.CreateCloudTableClient();
        TableClient.RetryPolicy = RetryPolicies.Retry(4, TimeSpan.Zero);
    }

    // Constructor - pass in a storage connection string.

    public TableHelper(string connectionString)
    {
        Account = CloudStorageAccount.Parse(connectionString);

        TableClient = Account.CreateCloudTableClient();
        TableClient.RetryPolicy = RetryPolicies.Retry(4, TimeSpan.Zero);
    }


    // List Tables.
    // Return true on success, false if not found, throw exception on error.

    public bool ListTables(out List<string> tableList)
    {
        tableList = new List<string>();

        try
        {
            IEnumerable<string> tables = TableClient.ListTables();
            if (tables != null)
            {
                tableList.AddRange(tables);
            }
            return true;
        }
        catch (StorageClientException ex)
        {
            if ((int)ex.StatusCode == 404)
            {
                return false;
            }

            throw;
        }
    }


    // Create Table.
    // Return true on success, false if already exists, throw exception on error.

    public bool CreateTable(string tableName)
    {
        try
        {
            TableClient.CreateTable(tableName);
            return true;
        }
        catch (StorageClientException ex)
        {
            if ((int)ex.StatusCode == 409)
            {
                return false;
            }

            throw;
        }
    }


    // Delete Table.
    // Return true on success, false if not found, throw exception on error.

    public bool DeleteTable(string tableName)
    {
        try
        {
            TableClient.DeleteTable(tableName);
            return true;
        }
        catch (StorageClientException ex)
        {
            if ((int)ex.StatusCode == 404)
            {
                return false;
            }

            throw;
        }
    }

    // Insert entity.
    // Return true on success, false if not found, throw exception on error.

    public bool InsertEntity(string tableName, object obj)
    {
        try
        {
            TableServiceContext tableServiceContext = TableClient.GetDataServiceContext();

            tableServiceContext.AddObject(tableName, obj);
            tableServiceContext.SaveChanges();

            return true;
        }
        catch (DataServiceRequestException)
        {
            return false;
        }
        catch (StorageClientException ex)
        {
            if ((int)ex.StatusCode == 404)
            {
                return false;
            }

            throw;
        }
    }


    // Retrieve an entity.
    // Return true on success, false if not found, throw exception on error.

    public bool GetEntity<T>(string tableName, string partitionKey, string rowKey, out T entity) where T : TableServiceEntity
    {
        entity = null;

        try
        {
            TableServiceContext tableServiceContext = TableClient.GetDataServiceContext();
            IQueryable<T> entities = (from e in tableServiceContext.CreateQuery<T>(tableName)
                                        where e.PartitionKey == partitionKey && e.RowKey == rowKey
                                        select e);

            entity = entities.FirstOrDefault();

            return true;
        }
        catch (DataServiceRequestException)
        {
            return false;
        }
        catch (StorageClientException ex)
        {
            if ((int)ex.StatusCode == 404)
            {
                return false;
            }

            throw;
        }
    }


    // Query entities. Use LINQ clauses to filter data.
    // Return true on success, false if not found, throw exception on error.

    public DataServiceQuery<T> QueryEntities<T>(string tableName) where T : TableServiceEntity
    {
        TableServiceContext tableServiceContext = TableClient.GetDataServiceContext();
        return tableServiceContext.CreateQuery<T>(tableName);
    }


    // Replace Update entity. Completely replace previous entity with new entity.
    // Return true on success, false if not found, throw exception on error.

    public bool ReplaceUpdateEntity<T>(string tableName, string partitionKey, string rowKey, T obj) where T : TableServiceEntity
    {
        try
        {
            TableServiceContext tableServiceContext = TableClient.GetDataServiceContext();
            IQueryable<T> entities = (from e in tableServiceContext.CreateQuery<T>(tableName)
                                        where e.PartitionKey == partitionKey && e.RowKey == rowKey
                                        select e);

            T entity = entities.FirstOrDefault();

            Type t = obj.GetType();
            PropertyInfo[] pi = t.GetProperties();

            foreach (PropertyInfo p in pi)
            {
                p.SetValue(entity, p.GetValue(obj, null), null);
            }

            tableServiceContext.UpdateObject(entity);
            tableServiceContext.SaveChanges(SaveChangesOptions.ReplaceOnUpdate);

            return true;
        }
        catch (DataServiceRequestException)
        {
            return false;
        }
        catch (StorageClientException ex)
        {
            if ((int)ex.StatusCode == 404)
            {
                return false;
            }

            throw;
        }
    }


    // Merge update an entity (preserve previous properties not overwritten).
    // Return true on success, false if not found, throw exception on error.

    public bool MergeUpdateEntity<T>(string tableName, string partitionKey, string rowKey, T obj) where T : TableServiceEntity
    {
        try
        {
            TableServiceContext tableServiceContext = TableClient.GetDataServiceContext();
            IQueryable<T> entities = (from e in tableServiceContext.CreateQuery<T>(tableName)
                                        where e.PartitionKey == partitionKey && e.RowKey == rowKey
                                        select e);

            T entity = entities.FirstOrDefault();

            Type t = obj.GetType();
            PropertyInfo[] pi = t.GetProperties();

            foreach (PropertyInfo p in pi)
            {
                p.SetValue(entity, p.GetValue(obj, null), null);
            }

            tableServiceContext.UpdateObject(entity);
            tableServiceContext.SaveChanges();

            return true;
        }
        catch (DataServiceRequestException)
        {
            return false;
        }
        catch (StorageClientException ex)
        {
            if ((int)ex.StatusCode == 404)
            {
                return false;
            }

            throw;
        }
    }


    // Delete entity.
    // Return true on success, false if not found, throw exception on error.

    public bool DeleteEntity<T>(string tableName, string partitionKey, string rowKey) where T : TableServiceEntity
    {
        try
        {
            TableServiceContext tableServiceContext = TableClient.GetDataServiceContext();
            IQueryable<T> entities = (from e in tableServiceContext.CreateQuery<T>(tableName)
                                        where e.PartitionKey == partitionKey && e.RowKey == rowKey
                                        select e);

            T entity = entities.FirstOrDefault();

            if (entities != null)
            {
                tableServiceContext.DeleteObject(entity);
                tableServiceContext.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (DataServiceRequestException)
        {
            return false;
        }
        catch (StorageClientException ex)
        {
            if ((int)ex.StatusCode == 404)
            {
                return false;
            }

            throw;
        }
    }

}



