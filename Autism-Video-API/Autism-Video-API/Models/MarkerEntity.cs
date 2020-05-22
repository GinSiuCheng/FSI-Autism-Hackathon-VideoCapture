using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Azure.Storage;
using Microsoft.Azure.CosmosDB.Table;

namespace Autism_Video_API.Models
{
    public class MarkerEntity : TableEntity
    {
        public string markerTime { get; set; }
        public string tag { get; set; }

        public MarkerEntity()
        {
        }

        public MarkerEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {
        }

        public MarkerEntity(string partitionKey, string rowKey, string markerTime, string tag, string StorageConnectionString)
        {
            string TableName = "Markers";
            base.PartitionKey = partitionKey;
            base.RowKey = rowKey;
            this.markerTime = markerTime;
            this.tag = tag;

            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table. 
            // Persist this http connection in the class
            var table = tableClient.GetTableReference(TableName);

            // Create the table if it doesn't exist.
            table.CreateIfNotExists();

            var io = TableOperation.Insert(this);

            table.Execute(io);

        }
    }
}