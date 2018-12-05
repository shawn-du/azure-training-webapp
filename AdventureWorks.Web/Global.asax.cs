using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using log4net;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Threading.Tasks;
using log4net.Appender;
using log4net.Core;
namespace AdventureWorks.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        ILog appLog = LogManager.GetLogger("appLog.Logging");
        ILog azurelog = LogManager.GetLogger("appLog.Logging");
        private AzureTableAppender _appender;
        protected void Application_Start()
        {
            ProcessAsync().GetAwaiter().GetResult();

            AreaRegistration.RegisterAllAreas();
            Azure_Table_Appender();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            azurelog.Info("test1");
            appLog.Info("test1");

        }
        public void Azure_Table_Appender()
        {
            _appender = new AzureTableAppender()
            {
                ConnectionString = CloudConfigurationManager.GetSetting("AzureStorageLogTableConnectionString"),
                TableName = "testAzureLoggingTable"
            };
            _appender.ActivateOptions();
            const string message = "Exception to follow on other line";
            var ex = new Exception("This is the exception message");

            var @event = new LoggingEvent(null, null, "testLoggerName", Level.Critical, message, ex);

            _appender.DoAppend(@event);
        }
        protected async Task ProcessAsync()
        {
            CloudStorageAccount storageAccount = null;
            CloudBlobContainer cloudBlobContainer = null;
            string sourceFile = null;
            string destinationFile = null;

            // Retrieve the connection string for use with the application. The storage connection string is stored
            // in an environment variable on the machine running the application called storageconnectionstring.
            // If the environment variable is created after the application is launched in a console or with Visual
            // Studio, the shell needs to be closed and reloaded to take the environment variable into account.
            //string storageConnectionString = Environment.GetEnvironmentVariable("storageconnectionstring");
            string storageConnectionString = CloudConfigurationManager.GetSetting("AzureStorageLogTableConnectionString");

            // Check whether the connection string can be parsed.
            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                try
                {
                    // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
                    CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                    cloudBlobContainer = cloudBlobClient.GetContainerReference("helloworld" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
                    await cloudBlobContainer.CreateAsync();
                    azurelog.Info("Created container "+ cloudBlobContainer.Name);

                    // Set the permissions so the blobs are public. 
                    BlobContainerPermissions permissions = new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    };
                    await cloudBlobContainer.SetPermissionsAsync(permissions);

                    // Create a file in your local MyDocuments folder to upload to a blob.
                    string localPath = Environment.CurrentDirectory+"temp";
                    string localFileName = System.Net.Dns.GetHostName() + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".txt";
                    sourceFile = Path.Combine(localPath, localFileName);
                    // Write text to the file.
                    File.WriteAllText(sourceFile, localFileName);

                    azurelog.Info("Temp file ="+sourceFile);
                    azurelog.Info("Uploading to Blob storage as blob "+localFileName);

                    // Get a reference to the blob address, then upload the file to the blob.
                    // Use the value of localFileName for the blob name.
                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(localFileName);
                    await cloudBlockBlob.UploadFromFileAsync(sourceFile);

                    // List the blobs in the container.
                    azurelog.Info("Listing blobs in container.");
                    BlobContinuationToken blobContinuationToken = null;
                    do
                    {
                        var results = await cloudBlobContainer.ListBlobsSegmentedAsync(null, blobContinuationToken);
                        // Get the value of the continuation token returned by the listing call.
                        blobContinuationToken = results.ContinuationToken;
                        foreach (IListBlobItem item in results.Results)
                        {
                            azurelog.Info(item.Uri);
                        }
                    } while (blobContinuationToken != null); // Loop while the continuation token is not null.

                    // Download the blob to a local file, using the reference created earlier. 
                    // Append the string "_DOWNLOADED" before the .txt extension so that you can see both files in MyDocuments.
                    destinationFile = sourceFile.Replace(".txt", "_download.txt");
                    await cloudBlockBlob.DownloadToFileAsync(destinationFile, FileMode.Create);
                }
                catch (StorageException ex)
                {
                    azurelog.Info("Error returned from the service:"+ex.Message);
                }
                finally
                {
                    // Clean up resources. This includes the container and the two temp files.
                    //azurelog.Info("Deleting the container and any blobs it contains");
                    //if (cloudBlobContainer != null)
                    //{
                    //    await cloudBlobContainer.DeleteIfExistsAsync();
                    //}
                    //azurelog.Info("Deleting the local source file and local downloaded files");
                    //File.Delete(sourceFile);
                    //File.Delete(destinationFile);
                }
            }
            else
            {
                azurelog.Info(
                    "A connection string has not been defined in the system environment variables. " +
                    "Add a environment variable named 'storageconnectionstring' with your storage " +
                    "connection string as a value.");
            }
        }
    }
}
