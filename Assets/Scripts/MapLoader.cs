using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScreepsViewer;
using UnityEngine.UI;
using System.IO;

public class MapLoader : MonoBehaviour
{
    // roomName, objects in room
    Dictionary<string, MapObject> mapObjects;
    Dictionary<string, User> users;
    string[] mapRooms;
    JSONObject currentData;
    ScreepsAPI api;
    float updateMapTimer = 5.0f;
    float lastMapUpdate;
    public MapObject mo;

    public Texture2D mapTexture;

    private void Awake()
    {
        lastMapUpdate = 0;
        api = GameObject.Find("ConnectionController").GetComponent<ScreepsAPI>();
    }

    private void OnMapData(JSONObject data)
    {
        currentData = data;
    }
    private void Update()
    {
        // Todo : create a check here for all 9  rooms being recieved.
        if (currentData != null)
        {
            SetMapData();
            currentData = null;
        }
    }
    public void GetMapFromConnection()
    {
        mapTexture = new Texture2D(152, 152);
        DrawGrid();
        mapObjects = new Dictionary<string, MapObject>();
        users = new Dictionary<string, User>();
        Action<JSONObject> thisBlows = new Action<JSONObject>(OnMapData);
        Debug.Log("get map: " + thisBlows);
        api.Http.GetSurroundingRooms(ConnectionController.roomName, ConnectionController.shardName, thisBlows);
        mapRooms = api.Http.GetSurroundingRoomNames(ConnectionController.roomName, ConnectionController.shardName);
        int len = mapRooms.Length;
        for (int i = 0; i < len; i++)
        {
            mapRooms[i] = ConnectionController.shardName + "_" + mapRooms[i];
        }
    }
    private void SetMapData()
    {
        string sub = "";
        currentData.GetField(ref sub, "subscription");
        sub = sub.Split(':')[1];
        sub = sub.Replace('/', '_');
        if (sub != "" && !mapObjects.ContainsKey(sub))
        {
            MapObject newMap = new MapObject(sub, currentData);
            mapObjects.Add(sub, newMap);
            Debug.Log("new map data : " + sub);
            Debug.Log("roomNames : " + mapRooms[1]);
        }
        if (mapObjects.Count > 0 && (lastMapUpdate + updateMapTimer) < Time.time)
        {
            lastMapUpdate = Time.time;
            CreateMapTexture();
        }
    }
    private void DrawGrid()
    {
        int xy = 51;
        int xyy = 102;
        for (int x = 0; x < 152; x++)
        {
            mapTexture.SetPixel(x, xy, Color.black);
            mapTexture.SetPixel(x, xyy, Color.black);
        }
        int yx = 51;
        int yxx = 102;
        for (int y = 0; y < 152; y++)
        {
            mapTexture.SetPixel(yx, y, Color.black);
            mapTexture.SetPixel(yxx, y, Color.black);
        }
        mapTexture.Apply();
    }
    private void CreateMapTexture()
    {
        mapTexture = new Texture2D(152, 152);
        DrawGrid();
        int xCounter = 0;
        int yCounter = 0;
        MapObject currentObject = null;
        foreach (string room in mapRooms)
        {
            if (!mapObjects.ContainsKey(room)) { Debug.Log("room not found in mapObjects" + room);  continue; }
            Debug.Log("room found in mapRooms: " + room);
            currentObject = mapObjects[room];
            for (int x = 1; x <= 50; x++)
            {
                for (int y = 1; y <= 50; y++)
                {
                    mapTexture.SetPixel(x + xCounter, y + yCounter, currentObject.roomTerrain.GetPixel(x,y));
                }
            }
            if (currentObject.walls != null) {
                foreach (int[] wall in currentObject.walls)
                {
                    //Debug.Log("wall here:"+wall[0] + wall[1]);
                    // set pixels for walls and roads and items
                    mapTexture.SetPixel(wall[0] + xCounter, 152 - (wall[1] + yCounter), Color.black);
                }
            }
            if (currentObject.roads != null)
            {
                foreach (int[] road in currentObject.roads)
                {
                    //Debug.Log("wall here:"+wall[0] + wall[1]);
                    // set pixels for walls and roads and items
                    mapTexture.SetPixel(road[0] + xCounter, 152 - (road[1] + yCounter), Color.gray);
                }
            }
            if (currentObject.playerItems != null)
            {
                foreach (string key in currentObject.playerItems.Keys)
                {
                    Color color = key == api.User.id ? Color.green : Color.red;
                    int[][] items = currentObject.playerItems[key];
                    Debug.Log(items + "  " + key);
                    foreach (int[] playerItem in items)
                    {
                        mapTexture.SetPixel(playerItem[0] + xCounter, 152 - (playerItem[1] + yCounter), color);
                    }
                }
            }
            if (xCounter > 100)
            {
                xCounter = 0;
                yCounter += 51;
            } else
            {
                xCounter += 51;
            }
            mo = currentObject;
        }
        mapTexture.Apply();
        Sprite s = Sprite.Create(mapTexture, new Rect(0, 0, 152, 152), new Vector2(0.5f, 0.5f));
        GameObject.Find("MapImage").GetComponent<Image>().sprite = s;
    }
}
