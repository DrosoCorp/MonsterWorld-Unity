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

        public async static Task<string> GetName(string uid)
        {
            Document res = await userTable.GetItemAsync(new Primitive(uid));
            if(res == null)
            {
                return null;
            }
            return res["Name"].AsString();
        }

        /// <summary>
        /// Change or set the name of an user, return false if name already taken
        /// </summary>
        public async static Task<bool> SetUsername(string uid, string name)
        {
            // Protection against malicious attack
            if (!lastChangeNameRequestDict.ContainsKey(uid))
            {
                lastChangeNameRequestDict.Add(uid, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds());
            }
            else
            {
                if(new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()  - lastChangeNameRequestDict[uid] > 3600000)
                {
                    lastChangeNameRequestDict[uid] = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                }
                else
                {
                    return false;
                }
            }

            if(name == "")
            {
                return false;
            }
            // Check availability
            Primitive primitiveName = new Primitive(name);
            DynamoDBEntry[] values = new DynamoDBEntry[1] { primitiveName };
            ScanFilter scanFilter = new ScanFilter();
            scanFilter.AddCondition("Name", ScanOperator.Equal, name);
            var response = userTable.Scan(scanFilter);

            List<Document> documentSet = new List<Document>();
            documentSet = await response.GetNextSetAsync();

            if (documentSet.Count > 0)
            {
                return false;
            }
            Document res = await userTable.GetItemAsync(new Primitive(uid));
            Document input = new Document();
            input.Add("UID", new Primitive(uid));
            input.Add("Name", new Primitive(name));
            if (res == null) {
                await userTable.PutItemAsync(input);
            }
            else
            {
                await userTable.UpdateItemAsync(input);
            }
            return true;
        }
    }
}
