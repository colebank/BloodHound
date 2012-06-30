using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure.StorageClient.Protocol;

    public class QueueHelper
    {
        public CloudStorageAccount Account;
        public CloudQueueClient QueueClient;

        // Constructor - get settings from a hosted service configuration or .NET configuration file.

        public QueueHelper(string configurationSettingName, bool hostedService)
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

            QueueClient = Account.CreateCloudQueueClient();
            QueueClient.RetryPolicy = RetryPolicies.Retry(4, TimeSpan.Zero);
        }

        // Constructor - pass in a storage connection string.

        public QueueHelper(string connectionString)
        {
            Account = CloudStorageAccount.Parse(connectionString);

            QueueClient = Account.CreateCloudQueueClient();
            QueueClient.RetryPolicy = RetryPolicies.Retry(4, TimeSpan.Zero);
        }


        // List queues.
        // Return true on success, false if not found, throw exception on error.

        public bool ListQueues(out List<CloudQueue> queueList)
        {
            queueList = new List<CloudQueue>();

            try
            {
                IEnumerable<CloudQueue> queues = QueueClient.ListQueues();
                if (queues != null)
                {
                    queueList.AddRange(queues);
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


        // Create a queue. 
        // Return true on success, false if already exists, throw exception on error.

        public bool CreateQueue(string queueName)
        {
            try
            {
                CloudQueue queue = QueueClient.GetQueueReference(queueName);
                queue.Create();
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


        // Delete a queue. 
        // Return true on success, false if not found, throw exception on error.

        public bool DeleteQueue(string queueName)
        {
            try
            {
                CloudQueue queue = QueueClient.GetQueueReference(queueName);
                queue.Delete();
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


        // Get queue metadata.
        // Return true on success, false if not found, throw exception on error.

        public bool GetQueueMetadata(string queueName, out NameValueCollection metadata)
        {
            metadata = null;

            try
            {
                CloudQueue queue = QueueClient.GetQueueReference(queueName);
                queue.FetchAttributes();

                metadata = queue.Metadata;

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


        // Set queue metadata.
        // Return true on success, false if not found, throw exception on error.

        public bool SetQueueMetadata(string queueName, NameValueCollection metadataList)
        {
            try
            {
                CloudQueue queue = QueueClient.GetQueueReference(queueName);
                queue.Metadata.Clear();
                queue.Metadata.Add(metadataList);
                queue.SetMetadata();
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


        // Peek the next message from a queue. 
        // Return true on success (message available), false if no message or no queue, throw exception on error.

        public bool PeekMessage(string queueName, out CloudQueueMessage message)
        {
            message = null;

            try
            {
                CloudQueue queue = QueueClient.GetQueueReference(queueName);
                message = queue.PeekMessage();
                return message != null;
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


        // Retrieve the next message from a queue. 
        // Return true on success (message available), false if no message or no queue, throw exception on error.

        public bool GetMessage(string queueName, out CloudQueueMessage message)
        {
            message = null;

            try
            {
                CloudQueue queue = QueueClient.GetQueueReference(queueName);
                message = queue.GetMessage();
                return message != null;
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


        // Create or update a blob. 
        // Return true on success, false if already exists, throw exception on error.

        public bool PutMessage(string queueName, CloudQueueMessage message)
        {
            try
            {
                CloudQueue queue = QueueClient.GetQueueReference(queueName);
                queue.AddMessage(message);
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


        // Clear all messages from a queue. 
        // Return true on success, false if already exists, throw exception on error.

        public bool ClearMessages(string queueName)
        {
            try
            {
                CloudQueue queue = QueueClient.GetQueueReference(queueName);
                queue.Clear();
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


        // Delete a previously read message. 
        // Return true on success, false if already exists, throw exception on error.

        public bool DeleteMessage(string queueName, CloudQueueMessage message)
        {
            try
            {
                CloudQueue queue = QueueClient.GetQueueReference(queueName);
                if (message != null)
                {
                    queue.DeleteMessage(message);
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

    }


