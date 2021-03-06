﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Azure.Storage;
using Microsoft.Azure.CosmosDB.Table;


namespace Autism_Video_API.Models
{
    public class PathFinderEvents
    {
        List<PathfinderEvent> events;
        public List<PathfinderEvent> Events
        {
            get
            {
                return this.events;
            }
        }

        private void ExecuteQuery(TableQuery<EventEntity> query, string StorageConnectionString)
        {
            string TableName = "Events";
            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table. 
            // Persist this http connection in the class
            var table = tableClient.GetTableReference(TableName);

            // Create the table if it doesn't exist.
            table.CreateIfNotExists();

            var data = table.ExecuteQuery<EventEntity>(query);
            foreach(var d in data)
            {
                events.Add(new PathfinderEvent(d.PartitionKey, d.RowKey, d.Skill, d.Target, d.Result, d.Comments));
            }
        }

        public PathFinderEvents(string PatientID, string StartDate, string EndDate, string StorageConnectionString)
        {
            events = new List<PathfinderEvent>();
            TableQuery<EventEntity> query = new TableQuery<EventEntity>()
            .Where(
                TableQuery.CombineFilters(
                    (TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, PatientID),
                        TableOperators.And,
                        TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual, StartDate)
                    )),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThanOrEqual, EndDate)
                )
            );
            ExecuteQuery(query, StorageConnectionString);
        }

        public PathFinderEvents(string PatientID, string StartDate, string EndDate, string Skill, string StorageConnectionString)
        {
            events = new List<PathfinderEvent>();
            TableQuery<EventEntity> query = new TableQuery<EventEntity>()
            .Where(
                TableQuery.CombineFilters(
                    (TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, PatientID),
                        TableOperators.And,
                        TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual, StartDate)
                    )),
                    TableOperators.And,
                    (TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThanOrEqual, EndDate),
                        TableOperators.And,
                        TableQuery.GenerateFilterCondition("Skill", QueryComparisons.GreaterThanOrEqual, Skill)
                    ))
                )
            );
            ExecuteQuery(query, StorageConnectionString);
        }

        public PathFinderEvents(string PatientID, string StartDate, string EndDate, string Skill, string Target, string StorageConnectionString)
        {
            events = new List<PathfinderEvent>();
            TableQuery<EventEntity> query = new TableQuery<EventEntity>()
            .Where(
                TableQuery.CombineFilters(
                    TableQuery.CombineFilters(
                        (TableQuery.CombineFilters(
                            TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, PatientID),
                            TableOperators.And,
                            TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual, StartDate)
                        )),
                        TableOperators.And,
                        (TableQuery.CombineFilters(
                            TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThanOrEqual, EndDate),
                            TableOperators.And,
                            TableQuery.GenerateFilterCondition("Skill", QueryComparisons.GreaterThanOrEqual, Skill)
                        ))
                    ),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("Target", QueryComparisons.GreaterThanOrEqual, Target)
                )
            );
            ExecuteQuery(query, StorageConnectionString);
        }
    }
}