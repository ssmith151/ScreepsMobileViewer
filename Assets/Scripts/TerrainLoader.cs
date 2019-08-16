using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using ScreepsViewer;

public class TerrainLoader : MonoBehaviour
{
    public TileBase tile; // Assign a tile asset to this.
    public TileBase northExit;
    public TileBase eastExit;
    public TileBase southExit;
    public TileBase westExit;
    //public TileBase tile2;
    TilemapRenderer tRenderer;
    TilemapRenderer tileMapExitsRenderer;
    RoomTerrain ter;
    ConnectRecievedTerrain recTer;
    [SerializeField]
    private ConnectionController connectController;
    private ScreepsHTTP http;
    string[,] terrainTypes;

    GameObject tileMapGO;
    GameObject tileMapExitsGO;

    //colors of terrain
    [SerializeField]
    private Color wallColor = new Color32(16, 16, 16, 255);
    [SerializeField]
    private Color plainsColor = new Color32(128, 128, 128, 255);
    [SerializeField]
    private Color swampColor = new Color32(150, 190, 110, 255);


    // Start is called before the first frame update
    void Awake()
    {
        GenerateTileMap();
        connectController = GameObject.Find("ConnectionController").GetComponent<ConnectionController>();
        http = connectController.GetComponent<ScreepsHTTP>();
    }

    void GenerateTileMap()
    {
        //ter = LoadTerrainFromFile();
        //Debug.Log(ter.result[0].type);
        //terrainTypes = SetTerrain(ter);
        //int terrainLength = terrainTypes.GetLength(0) * terrainTypes.GetLength(1);
        var grid = !!this.gameObject.GetComponent<Grid>() ? this.gameObject.GetComponent<Grid>() : this.gameObject.AddComponent<Grid>();
        //var grid = this.gameObject.AddComponent<Grid>();
        grid.cellSize = new Vector3(0.16f, 0.16f, 1);
        tileMapGO = new GameObject("Tilemap");
        Tilemap tilemap = tileMapGO.AddComponent<Tilemap>();
        tilemap.transform.SetParent(grid.transform);
        tRenderer = tilemap.gameObject.AddComponent<TilemapRenderer>();
        tilemap.GetComponent<TilemapRenderer>().sortOrder = TilemapRenderer.SortOrder.TopLeft;
        //exits
        tileMapExitsGO = new GameObject("TilemapE");
        Tilemap tileMapExits = tileMapExitsGO.AddComponent<Tilemap>();
        tileMapExits.transform.SetParent(grid.transform);
        tileMapExitsRenderer = tileMapExits.gameObject.AddComponent<TilemapRenderer>();
        tileMapExits.GetComponent<TilemapRenderer>().sortOrder = TilemapRenderer.SortOrder.TopLeft;
        //TileBase[,] tileArray = new TileBase[terrainTypes.GetLength(0), terrainTypes.GetLength(1)];
        for (int x = 0; x < 50; x++)
        {
            for (int y = 0; y < 50; y++)
            {
                Vector3Int currentPos = new Vector3Int(x, -y+50, 1);
                tilemap.SetTile(currentPos, tile);
                tilemap.SetTileFlags(currentPos, TileFlags.None);
                if (false)//(terrainTypes[x, y] == "wall")
                {
                    tilemap.SetColor(currentPos, wallColor);
                }
                else if (false)//(terrainTypes[x, y] == "swamp")
                {
                    tilemap.SetColor(currentPos, swampColor);
                }
                else
                {
                    tilemap.SetColor(currentPos, plainsColor);
                    if (x == 0)
                    {
                        Vector3Int currentPosExit = new Vector3Int(x, -y + 50, 0);
                        tileMapExits.SetTile(currentPos, westExit);
                    }
                    if (x == 49)
                    {
                        Vector3Int currentPosExit = new Vector3Int(x, -y + 50, 0);
                        tileMapExits.SetTile(currentPos, eastExit);
                    }
                    if ( y == 0)
                    {
                        Vector3Int currentPosExit = new Vector3Int(x, -y + 50, 0);
                        tileMapExits.SetTile(currentPos, northExit);
                    }
                    if (y == 49)
                    {
                        Vector3Int currentPosExit = new Vector3Int(x, -y + 50, 0);
                        tileMapExits.SetTile(currentPos, southExit);
                    }
                }
            }
        }
        //tilemap.SetTiles(positions, tileArray);
    }
    private void AssignRoomTerrain(JSONObject obj)
    {
        ConnectParsedTerrain t = JsonUtility.FromJson<ConnectParsedTerrain>(obj["terrain"].list[0].ToString());
        Debug.Log("Assign Room : " + t);
        ConnectParsedTerrain[] tt = { t };
        recTer = new ConnectRecievedTerrain {
            ok = obj["ok"].str,
            terrain = tt
        };
        UpdateTerrainFromConnection();
    }
    public void GetTerrainFromConnection()
    {
        Action<JSONObject> thisBlows = new Action<JSONObject>(AssignRoomTerrain);
        Debug.Log(thisBlows);
        http.GetRoomTerrain(ConnectionController.roomName, ConnectionController.shardName, thisBlows);
    }
    public string GetTerrainAtTile(int xIn, int yIn)
    {
        if (xIn < 0 || xIn > 49 || yIn < 0 || yIn > 49)
        {
            return null;
        }
        if (terrainTypes != null)
            return terrainTypes[xIn,yIn];
        return "plains";
    }
    //api.Http.GetRoom(coord.roomName, coord.shardName, serverCallback);
    public void UpdateTerrainFromConnection()
    {
        //Action<JSONObject> serverCallback = obj => {
        //    var terrainData = obj["terrain"].list[0]["terrain"].str;
        //    this.terrain[coord.key] = terrainData;
        //    callback(terrainData);
        //};
        
        Debug.Log("recTer : " + recTer );
        ter = LoadTerrainFromConnection(recTer);
        Debug.Log(ter.result[0].type);
        terrainTypes = SetTerrain(ter);
        int terrainLength = terrainTypes.GetLength(0) * terrainTypes.GetLength(1);

        Tilemap tilemap = tileMapGO.GetComponent<Tilemap>();
        tilemap.ClearAllTiles();
        tRenderer = tilemap.gameObject.GetComponent<TilemapRenderer>();
        TileBase[,] tileArray = new TileBase[terrainTypes.GetLength(0), terrainTypes.GetLength(1)];
        //exits
        Tilemap tileMapExits = tileMapExitsGO.GetComponent<Tilemap>();
        tileMapExits.ClearAllTiles();
        tileMapExitsRenderer = tileMapExits.gameObject.GetComponent<TilemapRenderer>();
        //tileMapExits.GetComponent<TilemapRenderer>().sortOrder = TilemapRenderer.SortOrder.TopLeft;
        for (int x = 0; x < 50; x++)
        {
            for (int y = 0; y < 50; y++)
            {
                Vector3Int currentPos = new Vector3Int(x, -y+50, -1);
                tilemap.SetTile(currentPos, tile);
                tilemap.SetTileFlags(currentPos, TileFlags.None);
                //Debug.Log(terrainTypes[x, y] + " " + x + " "+ y);
                if (terrainTypes[x, y] == "wall")
                {
                    tilemap.SetColor(currentPos, wallColor);
                }
                else if (terrainTypes[x, y] == "swamp")
                {
                    tilemap.SetColor(currentPos, swampColor);
                }
                else
                {
                    tilemap.SetColor(currentPos, plainsColor);
                    if (x == 0)
                    {
                        Vector3Int currentPosExit = new Vector3Int(x, -y + 50, 0);
                        tileMapExits.SetTile(currentPos, westExit);
                    }
                    if (x == 49)
                    {
                        Vector3Int currentPosExit = new Vector3Int(x, -y + 50, 0);
                        tileMapExits.SetTile(currentPos, eastExit);
                    }
                    if (y == 0)
                    {
                        Vector3Int currentPosExit = new Vector3Int(x, -y + 50, 0);
                        tileMapExits.SetTile(currentPos, northExit);
                    }
                    if (y == 49)
                    {
                        Vector3Int currentPosExit = new Vector3Int(x, -y + 50, 0);
                        tileMapExits.SetTile(currentPos, southExit);
                    }
                }
            }
        }
    }
    RoomTerrain LoadTerrainFromFile()
    {
        // get the file
        string savePath = Application.persistentDataPath + Path.DirectorySeparatorChar + "Terrain1.JSON";
        //string savePath = "C:\\Users\\Stephen\\ScreepsMobileViewer\\Assets\\Terrain1.json";
        string terrainFile = File.ReadAllText(savePath);
        terrainFile = "{\"result\":" + terrainFile.ToString() + "}";
        RoomTerrain t = JsonUtility.FromJson<RoomTerrain>(terrainFile);
        return t;
    }
    RoomTerrain LoadTerrainFromConnection(ConnectRecievedTerrain t)
    {
        //{ "ok":1,"terrain":[
        //         {"_id":"5982ff33b097071b4adc23cb",
        //         "room":"E20N6",
        //         "terrain":"1111111111111111110000000000000000000000000111111111111111111111111000000000000000000000000001111111111111111111111100000000000000000000000000011111111111111111111110000000000000000000000000000111111111111111111111000000000000000000000000000001111111111111111111100000000000000000000000000000011111111111111111100000000000000000000000000000000111111111111111110000000000000000000000000000000001111111111111111000000000000000000000000000000000011111111111111100000000000000000000000000000000000111111111111110000000000000000000000000000000000001111111111111000000000000000000000000000000000000011111111111110000000000000000000000000000000000000111111111111000000000000000000000000000000000000001111111111110000000000000000000000000000000000000011111111111000000000000000000000000000000000000000011111111110000000000000000000000000000000000000000111111111000000000000000000000000000000000000000001111111100000000000000000000000000000000000000000011111110000000000000000000000000000000000000000000111111000000000000000000000000000000000000000000001111110000000000000000000000000011100000000000000001111100000000000000000000000000111000000000000000011111000000000000000000000000000000000000000000000011110000000000000000000000000000000000000000000000111100000000000000000000000000000000000000000000000111100000000000000000000011100000000000000000000001111000000000000000000001111100000000000000000000011110000011100000000000111111000000000000000000000111100000111000000000001111110000000000000000100001111000000000000000000001111000000000000000011100011110000000000000000000001100000000000000000110001111100000000000000000000000000000000000000000000011111000000000000000000000000000011000000000000001111110000000000000000000000000000110000000000000011111100000000000000000000000000000000000000000001111110000000000000000000000000000000000011000000011111100000000000000000000000000000000000111000001111111000000000000000000000000000000000001111000011111110000000000000000000000000000000000011100001111111100000000000000000000000000000000000000000011111111000000000111111111000000000000000000000001111111110000000011111111110000000000000000000000011111111110000001111111111000000000000000000000001111111111110000001111111100000100000000000000000011111111111110000001111110000011100000000000000000111111111111110000000000000000111100000000000000001111111111111100000000000000000110000000000000000001111111111111000000000000000000000000000000000000001111111111110000000000000000000000000000000000000011111111",
        //         "type":"terrain"}
        //    ]
        //}
        //string terrainFile = "{\"result\": null}";
        RoomTerrain terr = new RoomTerrain();
        Terrain[] result = new Terrain[2500];
        //terr.result = result;
        //ConnectRecievedTerrain t = JsonUtility.FromJson<ConnectRecievedTerrain>(terrainIn.ToString());
        Debug.Log("load terrain : "+t);
        char[] c = t.terrain[0].terrain.ToCharArray();
        for (int i = 0; i < 2500; i++)
        {
            result[i] = new Terrain();
            //Debug.Log(result[i]);
            int x = i % 50;
            int y = Mathf.FloorToInt(i / 50);
            result[i].x = x;
            result[i].y = y;
            result[i].roomName = t.terrain[0].room;
            if (c[i] == '1' || c[i] == '3')
            {
                result[i].type = "wall";
            }
            else if (c[i] == '2')
            {
                result[i].type = "swamp";
            }
            else
            {
                result[i].type = "plains";
            }
        }
        terr.result = result;
        return terr;
    }
    string[,] SetTerrain(RoomTerrain terrainIn)
    {
        string[,] terrainTypes = new string[50, 50];
        foreach (Terrain t in terrainIn.result)
        {
            if (terrainTypes[t.x, t.y] != "wall")
            {
                terrainTypes[t.x, t.y] = t.type;
            }
        }
        return terrainTypes;
    }
    [System.Serializable]
    public class ConnectRecievedTerrain
    {
        public string ok;
        public ConnectParsedTerrain[] terrain;
    }
    [System.Serializable]
    public class ConnectParsedTerrain
    {
        public string _id;
        public string room;
        public string terrain;
        public string type;
    }
    [System.Serializable]
    public class Terrain
    {
        public int x;
        public int y;
        public string roomName;
        public string type;
    }
    [System.Serializable]
    public class RoomTerrain
    {
        public Terrain[] result;
    }

    [System.Serializable]
    public class RoomData
    {
        public RoomObjects[] objects;
        public User[] users;
    }
    [System.Serializable]
    public class RoomObjects
    {

    }
    [System.Serializable]
    public class User
    {
        public string id;
        public string username;
        public BadgeController.Badge badge;
    }
}
