using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Autism_Video_API.Models
{
    public class PathfinderVideo
    {
        public string PatientID { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }

        internal void AMSPublish(string storageConnectionString, string blobConnectionString)
        {
            //ToDo Connect to blob to generate uri
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["BlobConnectionString"]);

            //Create the blob client object.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            //Get a reference to a container to use for the sample code, and create it if it does not exist.
            CloudBlobContainer container = blobClient.GetContainerReference("testpublic");
            container.CreateIfNotExists();
            
            //Get a reference to a blob within the container.
            CloudBlockBlob blob = container.GetBlockBlobReference(this.FileName);

            // Mocking the call to Media Service

            //UpdateBlobUrl the media service url in database
            Update(this.PatientID, this.StartTime, blob.Uri.ToString(), storageConnectionString);
        }

        internal void RegisterMediaServiceUrl(string storageConnectionString, string mediaserviceUrl)
        {
            UpdateMediaServiceUrl(this.PatientID, this.StartTime, mediaserviceUrl, storageConnectionString);
        }


        public string FileName { get; set; }
        public string MediaServiceUrl { get; set; }

        public PathfinderVideo(){ }

        public PathfinderVideo(string patientID, string startTime, string endTime, string fileName, string mediaServiceUrl) 
        {
            this.PatientID = patientID;
            this.StartTime = startTime;
            this.EndTime = endTime;
            this.FileName = fileName;
            this.MediaServiceUrl = mediaServiceUrl;
        }

        public string Save(string StorageConnectionString)
        {
            var ve = new VideoEntity(PatientID, StartTime, EndTime, FileName, StorageConnectionString);

            //Parse the connection string and return a reference to the storage account.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["BlobConnectionString"]);

            //Create the blob client object.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            //Get a reference to a container to use for the sample code, and create it if it does not exist.
            CloudBlobContainer container = blobClient.GetContainerReference("testpublic");
            container.CreateIfNotExists();



            //Get a reference to a blob within the container.
            CloudBlockBlob blob = container.GetBlockBlobReference(FileName);

            //Set the expiry time and permissions for the blob.
            //In this case, the start time is specified as a few minutes in the past, to mitigate clock skew.
            //The shared access signature will be valid immediately.
            SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy();
            sasConstraints.SharedAccessStartTime = DateTimeOffset.UtcNow.AddMinutes(-5);
            sasConstraints.SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddHours(2);
            sasConstraints.Permissions = SharedAccessBlobPermissions.Write;

            //Generate the shared access signature on the blob, setting the constraints directly on the signature.
            string sasBlobToken = blob.GetSharedAccessSignature(sasConstraints);

            //Return the URI string for the container, including the SAS token.
            return blob.Uri + sasBlobToken;

        }

        public void Update(string PatientID, string StartTime, string Url, string StorageConnectionString)
        {
            var ve = new VideoEntity();
            ve.UpdateBlobUrl(PatientID, StartTime, Url, StorageConnectionString);
        }

        public void UpdateMediaServiceUrl(string PatientID, string StartTime, string Url, string StorageConnectionString)
        {
            var ve = new VideoEntity();
            ve.UpdateMediaServiceUrl(PatientID, StartTime, Url, StorageConnectionString);
        }


    }

}
