//-----------------------------------------------------------------
// File:         ServerDatabase.cs
// Description:  Utility class for database operations
// Module:       Network.Server
// Author:       Thomas Hervé
// Date:         30/03/2021
//-----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

namespace MonsterWorld.Unity.Network.Server
{
    public static class ServerDatabase
    {
        static readonly string accessKey = "AKIAV2ARZ5RDUFOW6KWR";
        static readonly string secretAccessKey = "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";
        static AmazonDynamoDBClient client = new AmazonDynamoDBClient(accessKey, secretAccessKey, new AmazonDynamoDBConfig {
            RegionEndpoint = RegionEndpoint.EUWest3
        });
        static readonly Table userTable = Table.LoadTable(client, "Users");
        // Protection against malicious attack
        static Dictionary<string, long> lastChangeNameRequestDict = new Dictionary<string, long>();

        public async static Task<int> UsernameAvailable(string uid, string name)
        {
            // Protection against malicious attack
            if (!lastChangeNameRequestDict.ContainsKey(uid))
            {
                lastChangeNameRequestDict.Add(uid, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds());
            }
            else
            {
                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() - lastChangeNameRequestDict[uid] > 5)
                {
                    lastChangeNameRequestDict[uid] = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                }
                else
                {
                    return 1; // timeout
                }
            }

            if (name == "")
            {
                return 3; // invalid
            }
            // Check availability
            Primitive primitiveName = new Primitive(name);
            DynamoDBEntry[] values = new DynamoDBEntry[1] { primitiveName };
            ScanFilter scanFilter = new ScanFilter();
            scanFilter.AddCondition("name", ScanOperator.Equal, name);
            var response = userTable.Scan(scanFilter);

            List<Document> documentSet = new List<Document>();
            documentSet = await response.GetNextSetAsync();

            if (documentSet.Count > 0)
            {
                return 2; // Already taken
            }
            return 0;
        }

        public async static Task<Document> GetUser(string uid)
        {
            Document res = await userTable.GetItemAsync(new Primitive(uid));
            if (res != null) { 
                res.Remove("UID");
            }
            return res;
        }

        /// <summary>
        /// Change or set the name of an user, return false if name already taken. You can't update the user if he already has been updated 
        /// in the last second
        /// </summary>
        public async static void SetUser(string uid, Document data)
        {
            string name = data["name"];
            data["UID"] = uid;
            Document res = await userTable.GetItemAsync(new Primitive(uid));
            if (res == null) {
                await userTable.PutItemAsync(data);
            }
            else
            {
                await userTable.UpdateItemAsync(data);
            }
        }
    }
}
