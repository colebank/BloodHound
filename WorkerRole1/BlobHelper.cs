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



    public class BlobHelper
    {
        public CloudStorageAccount Account;
        public CloudBlobClient BlobClient;

        // Constructor - get settings from a hosted service configuration or .NET configuration file.

        public BlobHelper(string configurationSettingName, bool hostedService)
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

            BlobClient = Account.CreateCloudBlobClient();
            BlobClient.RetryPolicy = RetryPolicies.Retry(4, TimeSpan.Zero);
        }

        // Constructor - pass in a storage connection string.

        public BlobHelper(string connectionString)
        {
            Account = CloudStorageAccount.Parse(connectionString);

            BlobClient = Account.CreateCloudBlobClient();
            BlobClient.RetryPolicy = RetryPolicies.Retry(4, TimeSpan.Zero);
        }

        // Enumerate the containers in a storage account.
        // Return true on success, false if already exists, throw exception on error.

        public bool ListContainers(out List<CloudBlobContainer> containerList)
        {
            containerList = new List<CloudBlobContainer>();

            try
            {
                IEnumerable<CloudBlobContainer> containers = BlobClient.ListContainers();
                if (containers != null)
                {
                    containerList.AddRange(containers);
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


        // Create a blob container. 
        // Return true on success, false if already exists, throw exception on error.

        public bool CreateContainer(string containerName)
        {
            try
            {
                CloudBlobContainer container = BlobClient.GetContainerReference(containerName);
                container.Create();
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


        // Get container access control.
        // Return true on success, false if not found, throw exception on error. 
        // Access level set to container|blob|private.

        public bool GetContainerACL(string containerName, out string accessLevel)
        {
            accessLevel = null;

            try
            {
                CloudBlobContainer container = BlobClient.GetContainerReference(containerName);
                BlobContainerPermissions permissions = container.GetPermissions();
                switch (permissions.PublicAccess)
                {
                    case BlobContainerPublicAccessType.Container:
                        accessLevel = "container";
                        break;
                    case BlobContainerPublicAccessType.Blob:
                        accessLevel = "blob";
                        break;
                    case BlobContainerPublicAccessType.Off:
                        accessLevel = "private";
                        break;
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

        // Set container access control to container|blob|private.
        // Return true on success, false if not found, throw exception on error.
        // Set access level to container|blob|private.

        public bool SetContainerACL(string containerName, string accessLevel)
        {
            try
            {
                CloudBlobContainer container = BlobClient.GetContainerReference(containerName);
                BlobContainerPermissions permissions = new BlobContainerPermissions();
                switch (accessLevel)
                {
                    case "container":
                        permissions.PublicAccess = BlobContainerPublicAccessType.Container;
                        break;
                    case "blob":
                        permissions.PublicAccess = BlobContainerPublicAccessType.Blob;
                        break;
                    case "private":
                    default:
                        permissions.PublicAccess = BlobContainerPublicAccessType.Off;
                        break;
                }

                container.SetPermissions(permissions);
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


        // Get container access policies.
        // Return true on success, false if not found, throw exception on error. 

        public bool GetContainerAccessPolicy(string containerName, out SortedList<string, SharedAccessPolicy> policies)
        {
            policies = new SortedList<string, SharedAccessPolicy>();

            try
            {
                CloudBlobContainer container = BlobClient.GetContainerReference(containerName);
                BlobContainerPermissions permissions = container.GetPermissions();

                if (permissions.SharedAccessPolicies != null)
                {
                    foreach (KeyValuePair<string, SharedAccessPolicy> policy in permissions.SharedAccessPolicies)
                    {
                        policies.Add(policy.Key, policy.Value);
                    }
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

        // Set container access policy.
        // Return true on success, false if not found, throw exception on error.

        public bool SetContainerAccessPolicy(string containerName, SortedList<string, SharedAccessPolicy> policies)
        {
            try
            {
                CloudBlobContainer container = BlobClient.GetContainerReference(containerName);
                BlobContainerPermissions permissions = container.GetPermissions();

                permissions.SharedAccessPolicies.Clear();

                if (policies != null)
                {
                    foreach (KeyValuePair<string, SharedAccessPolicy> policy in policies)
                    {
                        permissions.SharedAccessPolicies.Add(policy.Key, policy.Value);
                    }
                }

                container.SetPermissions(permissions);

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


        // Generate a shared access signature for a policy.
        // Return true on success, false if not found, throw exception on error.

        public bool GenerateSharedAccessSignature(string containerName, SharedAccessPolicy policy, out string signature)
        {
            signature = null;

            try
            {
                CloudBlobContainer container = BlobClient.GetContainerReference(containerName);
                signature = container.GetSharedAccessSignature(policy);
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


        // Generate a shared access signature for a saved container policy.
        // Return true on success, false if not found, throw exception on error.

        public bool GenerateSharedAccessSignature(string containerName, string policyName, out string signature)
        {
            signature = null;

            try
            {
                CloudBlobContainer container = BlobClient.GetContainerReference(containerName);
                signature = container.GetSharedAccessSignature(new SharedAccessPolicy(), policyName);
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


        // Get container properties.
        // Return true on success, false if not found, throw exception on error.

        public bool GetContainerProperties(string containerName, out BlobContainerProperties properties)
        {
            properties = null;

            try
            {
                CloudBlobContainer container = BlobClient.GetContainerReference(containerName);
                container.FetchAttributes();
                properties = container.Properties;
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



        // Get container metadata.
        // Return true on success, false if not found, throw exception on error.

        public bool GetContainerMetadata(string containerName, out NameValueCollection metadata)
        {
            metadata = null;

            try
            {
                CloudBlobContainer container = BlobClient.GetContainerReference(containerName);
                container.FetchAttributes();
                metadata = container.Metadata;
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


        // Set container metadata.
        // Return true on success, false if not found, throw exception on error.

        public bool SetContainerMetadata(string containerName, NameValueCollection metadata)
        {
            try
            {
                CloudBlobContainer container = BlobClient.GetContainerReference(containerName);
                container.Metadata.Clear();
                container.Metadata.Add(metadata);
                container.SetMetadata();
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



        // Delete a blob container. 
        // Return true on success, false if not found, throw exception on error.

        public bool DeleteContainer(string containerName)
        {
            try
            {
                CloudBlobContainer container = BlobClient.GetContainerReference(containerName);
                container.Delete();
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


        // Enumerate the blobs in a container. 
        // Return true on success, false if already exists, throw exception on error.

        public bool ListBlobs(string containerName, out List<CloudBlob> blobList)
        {
            blobList = new List<CloudBlob>();

            try
            {
                CloudBlobContainer container = BlobClient.GetContainerReference(containerName);
                IEnumerable<IListBlobItem> blobs = container.ListBlobs();
                if (blobs != null)
                {
                    foreach (CloudBlob blob in blobs)
                    {
                        blobList.Add(blob);
                    }
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


        // Put (create or update) a blob.
        // Return true on success, false if unable to create, throw exception on error.

        public bool PutBlob(string containerName, string blobName, string content)
        {
            try
            {
                CloudBlobContainer container = BlobClient.GetContainerReference(containerName);
                CloudBlob blob = container.GetBlobReference(blobName);
                blob.UploadText(content);
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


        // Put (create or update) a page blob.
        // Return true on success, false if unable to create, throw exception on error.

        public bool PutBlob(string containerName, string blobName, int pageBlobSize)
        {
            try
            {
                CloudBlobContainer container = BlobClient.GetContainerReference(containerName);
                CloudPageBlob blob = container.GetPageBlobReference(blobName);
                blob.Create(pageBlobSize);
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


        // Put (create or update) a blob conditionally based on expected ETag value.
        // Return true on success, false if unable to create, throw exception on error.

        public bool PutBlobIfUnchanged(string containerName, string blobName, string content, string ExpectedETag)
        {
            try
            {
                CloudBlobContainer container = BlobClient.GetContainerReference(containerName);
                CloudBlob blob = container.GetBlobReference(blobName);
                BlobRequestOptions options = new BlobRequestOptions();
                options.AccessCondition = AccessCondition.IfMatch(ExpectedETag);
                blob.UploadText(content, new UTF8Encoding(), options);
                return true;
            }
            catch (StorageClientException ex)
            {
                if ((int)ex.StatusCode == 404 || (int)ex.StatusCode == 412)
                {
                    return false;
                }

                throw;
            }
        }


        // Put (create or update) a blob with an MD5 hash.
        // Return true on success, false if unable to create, throw exception on error.

        public bool PutBlobWithMD5(string containerName, string blobName, string content)
        {
            try
            {
                CloudBlobContainer container = BlobClient.GetContainerReference(containerName);
                CloudBlob blob = container.GetBlobReference(blobName);
                string md5 = Convert.ToBase64String(new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(System.Text.Encoding.Default.GetBytes(content)));
                blob.Properties.ContentMD5 = md5;
                blob.UploadText(content);
                return true;
            }
            catch (StorageClientException ex)
            {
                if ((int)ex.StatusCode == 404 || (int)ex.StatusCode == 412)
                {
                    return false;
                }

                throw;
            }
        }


        // Get a page of a page blob.
        // Return true on success, false if unable to create, throw exception on error.

        public bool GetPage(string containerName, string blobName, int pageOffset, int pageSize, out string content)
        {
            content = null;

            try
            {
                CloudBlobContainer container = BlobClient.GetContainerReference(containerName);
                CloudPageBlob blob = container.GetPageBlobReference(blobName);
                BlobStream stream = blob.OpenRead();
                byte[] data = new byte[pageSize];
                stream.Seek(pageOffset, SeekOrigin.Begin);
                stream.Read(data, 0, pageSize);
                content = new UTF8Encoding().GetString(data);
                stream.Close();

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


        // Put a page of a page blob.
        // Return true on success, false if unable to create, throw exception on error.

        public bool PutPage(string containerName, string blobName, string content, int pageOffset, int pageSize)
        {
            try
            {
                CloudBlobContainer container = BlobClient.GetContainerReference(containerName);
                CloudPageBlob blob = container.GetPageBlobReference(blobName);
                MemoryStream stream = new MemoryStream(new UTF8Encoding().GetBytes(content));
                blob.WritePages(stream, pageOffset);
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


        // Retrieve the list of regions in use for a page blob. 
        // Return true on success, false if not found, throw exception on error.

        public bool GetPageRegions(string containerName, string blobName, out PageRange[] regions)
        {
            regions = null;

            try
            {
                CloudBlobContainer container = BlobClient.GetContainerReference(containerName);
                CloudPageBlob blob = container.GetPageBlobReference(blobName);

                IEnumerable<PageRange> regionList = blob.GetPageRanges();
                regions = new PageRange[regionList.Count()];
                int i = 0;
                foreach (PageRange region in regionList)
                {
                    regions[i++] = region;
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


        // Retrieve the list of uploaded blocks for a blob. 
        // Return true on success, false if already exists, throw exception on error.

        public bool GetBlockList(string containerName, string blobName, out string[] blockIds)
        {
            blockIds = null;

            try
            {
                CloudBlobContainer container = BlobClient.GetContainerReference(containerName);
                CloudBlockBlob blob = container.GetBlockBlobReference(blobName);

                IEnumerable<ListBlockItem> blobs = blob.DownloadBlockList();
                blockIds = new string[blobs.Count()];
                int i = 0;
                foreach (ListBlockItem block in blobs)
                {
                    blockIds[i++] = block.Name;
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


        // Put block - upload a block (portion) of a blob. 
        // Return true on success, false if already exists, throw exception on error.

        public bool PutBlock(string containerName, string blobName, int blockId, string[] blockIds, string content)
        {
            try
            {
                CloudBlobContainer container = BlobClient.GetContainerReference(containerName);
                CloudBlockBlob blob = container.GetBlockBlobReference(blobName);

                string blockIdBase64 = Convert.ToBase64String(System.BitConverter.GetBytes(blockId));

                UTF8Encoding utf8Encoding = new UTF8Encoding();
                using (MemoryStream memoryStream = new MemoryStream(utf8Encoding.GetBytes(content)))
                {
                    blob.PutBlock(blockIdBase64, memoryStream, null);
                }

                blockIds[blockId] = blockIdBase64;

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


        // Put block list - complete creation of blob based on uploaded content.
        // Return true on success, false if already exists, throw exception on error.

        public bool PutBlockList(string containerName, string blobName, string[] blockIds)
        {
            try
            {
                CloudBlobContainer container = BlobClient.GetContainerReference(containerName);
                CloudBlockBlob blob = container.GetBlockBlobReference(blobName);

                blob.PutBlockList(blockIds);

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


        // Get (retrieve) a blob and return its content.
        // Return true on success, false if unable to create, throw exception on error.

        public bool GetBlob(string containerName, string blobName, out string content)
        {
            content = null;

            try
            {
                CloudBlobContainer container = BlobClient.GetContainerReference(containerName);
                CloudBlob blob = container.GetBlobReference(blobName);
                content = blob.DownloadText();
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


        // Copy a blob.
        // Return true on success, false if unable to create, throw exception on error.

        public bool CopyBlob(string sourceContainerName, string sourceBlobName, string destContainerName, string destBlobName)
        {
            try
            {
                CloudBlobContainer sourceContainer = BlobClient.GetContainerReference(sourceContainerName);
                CloudBlob sourceBlob = sourceContainer.GetBlobReference(sourceBlobName);
                CloudBlobContainer destContainer = BlobClient.GetContainerReference(destContainerName);
                CloudBlob destBlob = destContainer.GetBlobReference(destBlobName);
                destBlob.CopyFromBlob(sourceBlob);
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


        // Create a snapshot of a blob.
        // Return true on success, false if unable to create, throw exception on error.

        public bool SnapshotBlob(string containerName, string blobName)
        {
            try
            {
                CloudBlobContainer container = BlobClient.GetContainerReference(containerName);
                CloudBlob blob = container.GetBlobReference(blobName);
                blob.CreateSnapshot();
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


        // Delete a blob.
        // Return true on success, false if unable to create, throw exception on error.

        public bool DeleteBlob(string containerName, string blobName)
        {
            try
            {
                CloudBlobContainer container = BlobClient.GetContainerReference(containerName);
                CloudBlob blob = container.GetBlobReference(blobName);
                blob.Delete();
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


        // Lease a blob.
        // Lease action: acquire|renew|break|release.
        // Lease Id: set on acquire action; must be specifie for all other actions.
        // Return true on success, false if already exists, throw exception on error.

        public bool LeaseBlob(string containerName, string blobName, string leaseAction, ref string leaseId)
        {
            const int timeout = 90;

            try
            {
                CloudBlobContainer container = BlobClient.GetContainerReference(containerName);
                CloudBlob blob = container.GetBlobReference(blobName);

                switch (leaseAction)
                {
                    case "acquire":
                        leaseId = AcquireLease(blob, timeout);
                        break;
                    case "release":
                        LeaseOperation(blob, leaseId, LeaseAction.Release, timeout);
                        break;
                    case "renew":
                        LeaseOperation(blob, leaseId, LeaseAction.Renew, timeout);
                        break;
                    case "break":
                        LeaseOperation(blob, null, LeaseAction.Break, timeout);
                        break;
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

        public string AcquireLease(CloudBlob blob, int timeout)
        {
            var creds = blob.ServiceClient.Credentials;
            var transformedUri = new Uri(creds.TransformUri(blob.Uri.ToString()));
            var request = BlobRequest.Lease(transformedUri, timeout, LeaseAction.Acquire, null);
            blob.ServiceClient.Credentials.SignRequest(request);
            using (var response = request.GetResponse())
            {
                return response.Headers["x-ms-lease-id"];
            }
        }

        public void LeaseOperation(CloudBlob blob, string leaseId, LeaseAction action, int timeout)
        {
            var creds = blob.ServiceClient.Credentials;
            var transformedUri = new Uri(creds.TransformUri(blob.Uri.ToString()));
            var request = BlobRequest.Lease(transformedUri, timeout, action, leaseId);
            creds.SignRequest(request);
            request.GetResponse().Close();
        }


        // Get blob metadata.
        // Return true on success, false if not found, throw exception on error.

        public bool GetBlobMetadata(string containerName, string blobName, out NameValueCollection metadata)
        {
            metadata = null;

            try
            {
                CloudBlobContainer container = BlobClient.GetContainerReference(containerName);
                CloudBlob blob = container.GetBlobReference(blobName);
                blob.FetchAttributes();

                metadata = blob.Metadata;

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


        // Set blob metadata.
        // Return true on success, false if not found, throw exception on error.

        public bool SetBlobMetadata(string containerName, string blobName, NameValueCollection metadata)
        {
            try
            {
                CloudBlobContainer container = BlobClient.GetContainerReference(containerName);
                CloudBlob blob = container.GetBlobReference(blobName);
                blob.Metadata.Clear();
                blob.Metadata.Add(metadata);
                blob.SetMetadata();

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



        // Get blob properties.
        // Return true on success, false if not found, throw exception on error.

        public bool GetBlobProperties(string containerName, string blobName, out SortedList<string, string> properties)
        {
            properties = new SortedList<string, string>();

            try
            {
                CloudBlobContainer container = BlobClient.GetContainerReference(containerName);
                CloudBlob blob = container.GetBlobReference(blobName);
                blob.FetchAttributes();

                properties.Add("BlobType", blob.Properties.BlobType.ToString());
                properties.Add("CacheControl", blob.Properties.CacheControl);
                properties.Add("ContentEncoding", blob.Properties.ContentEncoding);
                properties.Add("ContentLanguage", blob.Properties.ContentLanguage);
                properties.Add("ContentMD5", blob.Properties.ContentMD5);
                properties.Add("ContentType", blob.Properties.ContentType);
                properties.Add("ETag", blob.Properties.ETag);
                properties.Add("LastModified", blob.Properties.LastModifiedUtc.ToShortDateString());

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


        // Set blob properties.
        // Return true on success, false if not found, throw exception on error.

        public bool SetBlobProperties(string containerName, string blobName, SortedList<string, string> properties)
        {
            try
            {
                CloudBlobContainer container = BlobClient.GetContainerReference(containerName);
                CloudBlob blob = container.GetBlobReference(blobName);
                foreach (KeyValuePair<string, string> property in properties)
                {
                    switch (property.Key)
                    {
                        case "CacheControl":
                            blob.Properties.CacheControl = property.Value;
                            break;
                        case "ContentEncoding":
                            blob.Properties.ContentEncoding = property.Value;
                            break;
                        case "ContentLanguage":
                            blob.Properties.ContentLanguage = property.Value;
                            break;
                        case "ContentMD5":
                            blob.Properties.ContentMD5 = property.Value;
                            break;
                        case "ContentType":
                            blob.Properties.ContentType = property.Value;
                            break;
                    }
                }
                blob.SetProperties();

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

