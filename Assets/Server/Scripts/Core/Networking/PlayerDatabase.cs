using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class PlayerData
{
    [NonSerialized]
    public string uid;
    [NonSerialized]
    public bool dirty = false;

    public string name;
    public List<string> coordinates;

    public PlayerData(string uid, string name, List<string> coordinates)
    {
        this.name = name;
        this.coordinates = coordinates;
    }
}

namespace MonsterWorld.Unity.Network.Server
{
    public static class PlayerDatabase
    {

        static Dictionary<Guid, PlayerData> playersDict = new Dictionary<Guid, PlayerData>();

        public static void MarkForUpdate(PlayerData p)
        {
            p.dirty = true;
        }


        public async static Task<bool> PlayerConnection(Guid uid)
        {
            if (!playersDict.ContainsKey(uid))
            {
                return await LoadUser(uid);
            }
            return true;
        }

        public static PlayerData GetPlayerData(Guid uid)
        {
            return playersDict[uid];
        }

        /// <summary>
        /// /!\ BULK UPDATE THE DATABASE /!\
        /// </summary>
        public static void UpdateUsers()
        {
            foreach (var player in playersDict)
            {
                Guid uid = player.Key;
                PlayerData playerState = player.Value;
                if(playerState.dirty)
                {
                    UpdateOrCreateUser(uid, playerState);
                    playerState.dirty = false;
                }
                
            }
        }

        /// <summary>
        /// /!\ UPDATE THE DATABASE /!\
        /// </summary>
        public static void CreateUser(Guid uid, PlayerData p)
        {
            UpdateOrCreateUser(uid, p);
        }


        // Utility

        /// <summary>
        /// Function to call to initialize a player, will retrieve the data of a player and return false if this data doesn't exists.
        /// </summary>
        private async static Task<bool> LoadUser(Guid uid)
        {
            Document p = await ServerDatabase.GetUser(uid.ToString());
            if (p == null)
            {
                playersDict[uid] = null;
                return false;
            }
            playersDict[uid] = DocumentToPlayerStruct(p);

            return true;
        }

        /// <summary>
        /// Utility function to get a sample User
        /// </summary>
        public static PlayerData GetEmptyPlayer(Guid uid, string name)
        {
            return new PlayerData(uid.ToString(), name, new List<string>() { "1" });
        }

        private static void UpdateOrCreateUser(Guid uid, PlayerData p)
        {
            ServerDatabase.SetUser(uid.ToString(), PlayerStructToDocument(p));
        }

        private static Document PlayerStructToDocument(PlayerData data)
        {
            Document input = Document.FromJson(JsonUtility.ToJson(data));
            return input;
        }

        private static PlayerData DocumentToPlayerStruct(Document doc)
        {
            return JsonUtility.FromJson<PlayerData>(doc.ToJson());
        }


    }
}