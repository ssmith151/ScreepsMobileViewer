using ScreepsViewer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ScreepsViewer
{
    [Serializable]
    public class MapObject
    {
        string sub;
        public string sectorPath;
        public Texture2D roomTerrain;
        public Vector2Int roomStart;
        // parsedRoom
        public int ewNum;
        public string ew;
        public int nsNum;
        public string ns;
        public int[][] walls;
        public int[][] roads;
        public int[][] power;
        public int[][] portals;
        int[][] sources; // built in to texture
        int[][] minerals; // built in to texture
        int[][] controllers; // built in to texture
        int[][] keeperLairs; // built in to texture
        public int[][] invaders;
        public int[][] keepers;
        public Dictionary<string, int[][]> playerItems = new Dictionary<string, int[][]>();

        string parseRoomToSector(string room)
        {
            int nsPos = room.IndexOf('N');
            nsPos = nsPos > 0 ? nsPos : room.IndexOf('S');
            string[] splitroom = room.Split(room[nsPos]);
            splitroom[1] = room[nsPos] + splitroom[1];
            ew = splitroom[0].Substring(0, 1);
            ewNum = Int32.Parse(splitroom[0].Substring(1));
            string ewNumS = ewNum >= 10 ? ewNum.ToString()[0] + "0" : "0";
            ns = splitroom[1].Substring(0, 1);
            nsNum = Int32.Parse(splitroom[1].Substring(1));
            string nsNumS = nsNum >= 10 ? nsNum.ToString()[0] + "0" : "0";
            string sectorName = ew + ewNumS + ns + nsNumS;
            return sectorName;
        }
        Vector2Int roomTerrainStart()
        {
            //Todo:this needs a solution to the furthest out room
            int xStart = (ewNum % 10) * 50;
            int yStart = (nsNum % 10) * 50;
            int xMod = ew == "E" ? xStart : 500 - xStart;
            int yMod = ns == "N" ? yStart : 500 - yStart;
            return new Vector2Int(xMod, yMod);
        }
        public MapObject(string subIn, JSONObject objIn)
        {
            /*roomMap2:{}/{}
            roomMap2:{}
                {
                    "w": [...],     # walls
                    "r": [...],     # roads
                    "pb": [...],    # power banks or power
                    "p": [...],     # portals
                    "s": [...],     # sources
                    "m": [...],     # minerals
                    "c": [...],     # controllers
                    "k": [...],     # keeper lairs
                    "<user id>": [...]  # structures and creeps belonging to <user id> user
                    # 0-inf user id entries allowed
                    // addding in this one
                    "2": []         # invader
                    "3": []         # source keeper
                }
                # each [...] is a list of positions, like `[[0, 2], [40,30]]` to represent the thing being at
                # (x=0, y=2) and (x=40, y=30). The lists are empty if there were no things of that type present
                # last tick. "user" entries only appear if a user is present in the room, and are distinguished
                # from the others by not being one of the 8 known keys.*/
            sub = subIn;
            if (subIn.Contains("_"))
            {
                string shard = subIn.Split('_')[0];
                string room = subIn.Split('_')[1];
                // parse room in
                string sectorName = parseRoomToSector(room);
                //sectorPath = "Resources" + Path.DirectorySeparatorChar +"Maps"+ Path.DirectorySeparatorChar + shard + Path.DirectorySeparatorChar + sectorName;
                sectorPath = "Maps" + Path.DirectorySeparatorChar + shard + Path.DirectorySeparatorChar + sectorName;
                roomStart = roomTerrainStart();
                Texture2D sectorMap = (Texture2D)Resources.Load(sectorPath);
                Debug.Log(sectorPath + "  " +sectorMap.width + "  " + sectorMap.height);
                roomTerrain = new Texture2D(50, 50);
                for (int x = 1; x <= 50; x++)
                {
                    for (int y = 1; y <= 50; y++)
                    {
                        roomTerrain.SetPixel(x, y, sectorMap.GetPixel(roomStart.x + x, roomStart.y + y));
                    }
                }
                roomTerrain.Apply();
            }
            else
            {
                // in order to add private server support there needs to be a standard naming convention.
                // this better left as todo until it becomes clear what private seervers need naming wise.
                string sectorName = parseRoomToSector(subIn); // + http.privateServerName
                sectorPath = "Resources" + Path.DirectorySeparatorChar + "Maps" + Path.DirectorySeparatorChar + sectorName;
            }
            Debug.Log("sector name : " + sectorPath);
            if (objIn.type == JSONObject.Type.NULL)
            {
                Debug.Log("could not find input to create MapObject");
                return;
            }
            if (objIn.HasField("w"))
            {
                walls = NewList("w");
            }
            if (objIn.HasField("r"))
            {
                roads = NewList("r");
            }
            if (objIn.HasField("pb"))
            {
                power = NewList("pb");
            }
            if (objIn.HasField("p"))
            {
                portals = NewList("p");
            }
            if (objIn.HasField("2"))
            {
                invaders = NewList("2");
            }
            if (objIn.HasField("3"))
            {
                keepers = NewList("3");
            }
            List<string> dataKeys = objIn.keys;
            foreach (string key in dataKeys)
            {
                if (checkKey(key))
                {
                    continue;
                }
                if (playerItems.Count <= 0)
                {
                    playerItems = new Dictionary<string, int[][]>();
                }

                if (!playerItems.ContainsKey(key))
                {
                    playerItems.Add(key, NewList(key));
                }
                else
                {
                    playerItems[key] = NewList(key);
                }
            }
            bool checkKey(string inCheck)
            {
                foreach (string defaultType in Constants.defaultMapTypes)
                {
                    if (defaultType.Contains(inCheck))
                    {
                        return true;
                    }
                }
                return false;
            }
            int[][] NewList(string typeName)
            {
                int typeLength = objIn[typeName].Count;
                if (typeLength <= 0)
                    return null;
                int[][] arrayType = new int[typeLength][];
                int _x = 0;
                int _y = 0;
                List<JSONObject> typeList = objIn[typeName].list;
                for (int i = 0; i < typeLength; i++)
                {
                    _x = Int32.Parse(typeList[i][0].ToString());
                    _y = Int32.Parse(typeList[i][1].ToString());
                    arrayType[i] = new int[2] { _x, _y };
                }
                return arrayType;
            }
        }
    }
}
