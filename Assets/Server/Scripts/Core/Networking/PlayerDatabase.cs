using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public struct PlayerStruct
{
    public string name;
    public List<string> coordinates;
}

namespace MonsterWorld.Unity.Network.Server
{
    public class PlayerDatabase : MonoBehaviour
    {
        static Dictionary<Guid, PlayerStruct> playersDict = new Dictionary<Guid, PlayerStruct>();

        /// <summary>
        /// Function to call to initialize a player, will retrieve the data of a player and return false if this data doesn't exists.
        /// </summary>
        public async static Task<bool> LoadUser(Guid uid)
        {
            Document p = await ServerDatabase.GetUser(uid.ToString());
            if(p == null)
            {
                return false;
            }
            playersDict[uid] = DocumentToPlayerStruct(p);
            return true;
        }

        ///<summary>
        /// Update a user without update the database
        ///</summary>
        public static void UpdateUserNoDatabase(Guid uid, PlayerStruct p)
        {
            playersDict[uid] = p;
        }

        public async static Task<bool> UpdateUser(Guid uid, PlayerStruct p)
        {
            playersDict[uid] = p;
            return await UpdateOrCreateUser(uid, p);
        }

        public async static Task<bool> UpdateUser(Guid uid, string name)
        {
            if (playersDict.ContainsKey(uid))
            {
                PlayerStruct p = playersDict[uid];
                p.name = name;
                return await UpdateOrCreateUser(uid, p);
            } else
            {
                Document doc = await ServerDatabase.GetUser(uid.ToString());
                if(doc == null)
                {
                    return await InitializeNewUser(uid, name);
                } else
                {
                    PlayerStruct p = DocumentToPlayerStruct(doc);
                    p.name = name;
                    return await UpdateOrCreateUser(uid, p);
                }
            }
        }

        public static PlayerStruct GetPlayerData(Guid uid)
        {
            return playersDict[uid];
        }

        public static void PlayerDisconnect(Guid uid)
        {
            if(playersDict.ContainsKey(uid))
            {
                playersDict.Remove(uid);
            }
        }


        // Utility

        /// <summary>
        /// Initialize a non existing user in database
        /// </summary>
        private async static Task<bool> InitializeNewUser(Guid uid, string name)
        {
            PlayerStruct p = new PlayerStruct()
            {
                name = name,
                coordinates  = new List<string>() { "1" }
            };
            return await UpdateOrCreateUser(uid, p);
        }

        private async static Task<bool> UpdateOrCreateUser(Guid uid, PlayerStruct p)
        {
            return await ServerDatabase.SetUser(uid.ToString(), PlayerStructToDocument(p));
        }

        private static Document PlayerStructToDocument(PlayerStruct data)
        {
            Document input = Document.FromJson(JsonUtility.ToJson(data));
            return input;
        }

        private static PlayerStruct DocumentToPlayerStruct(Document doc)
        {
            return JsonUtility.FromJson<PlayerStruct>(doc.ToJson());
        }


    }
}