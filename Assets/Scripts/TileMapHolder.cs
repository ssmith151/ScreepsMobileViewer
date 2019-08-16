using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapHolder : MonoBehaviour
{

    public TileBase tile; // Assign a tile asset to this.
    //public TileBase tile2;
    TilemapRenderer tRenderer;
    RoomTerrain ter;
    string[,] terrainTypes;

    GameObject tileMapGO;

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
    }

    void GenerateTileMap()
    {
        ter = LoadTerrainFromFile();
        Debug.Log(ter.result[0].type);
        terrainTypes = SetTerrain(ter);
        int terrainLength = terrainTypes.GetLength(0) * terrainTypes.GetLength(1);
        var grid = this.gameObject.AddComponent<Grid>();
        grid.cellSize = new Vector3(0.16f, 0.16f, 1);
        tileMapGO = new GameObject("Tilemap");
        Tilemap tilemap = tileMapGO.AddComponent<Tilemap>();
        tilemap.transform.SetParent(grid.transform);
        tRenderer = tilemap.gameObject.AddComponent<TilemapRenderer>();
        tilemap.GetComponent<TilemapRenderer>().sortOrder = TilemapRenderer.SortOrder.TopLeft;
        TileBase[,] tileArray = new TileBase[terrainTypes.GetLength(0), terrainTypes.GetLength(1)];
        for (int x = 0; x < 50; x++)
        {
            for (int y = 0; y < 50; y++)
            {
                Vector3Int currentPos = new Vector3Int(x, y, 0);
                tilemap.SetTile(currentPos, tile);
                tilemap.SetTileFlags(currentPos, TileFlags.None);
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
                }
            }
        }
        //tilemap.SetTiles(positions, tileArray);
    }
    public void UpdateTerrainFromConnection(object terrainIn)
    {
        ter = LoadTerrainFromConnection(terrainIn);
        //Debug.Log(ter.result[0].type);
        terrainTypes = SetTerrain(ter);
        int terrainLength = terrainTypes.GetLength(0) * terrainTypes.GetLength(1);
         
        Tilemap tilemap = tileMapGO.GetComponent<Tilemap>();
        tRenderer = tilemap.gameObject.GetComponent<TilemapRenderer>();
        TileBase[,] tileArray = new TileBase[terrainTypes.GetLength(0), terrainTypes.GetLength(1)];
        for (int x = 0; x < 50; x++)
        {
            for (int y = 0; y < 50; y++)
            {
                Vector3Int currentPos = new Vector3Int(x, y, 0);
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
                }
            }
        }
    }
    RoomTerrain LoadTerrainFromFile()
    {
        // get the file
        // var savePath = Application.dataPath + "\\Terrain1.JSON";
        string savePath = "C:\\Users\\Stephen\\ScreepsMobileViewer\\Assets\\Terrain1.json";
        string terrainFile = File.ReadAllText(savePath);
        terrainFile = "{\"result\":" + terrainFile.ToString() + "}";
        RoomTerrain t = JsonUtility.FromJson<RoomTerrain>(terrainFile);
        return t;
    }
    RoomTerrain LoadTerrainFromConnection(object terrainIn)
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
        ConnectRecievedTerrain t = JsonUtility.FromJson<ConnectRecievedTerrain>(terrainIn.ToString());
        char[] c = t.terrain[0].terrain.ToCharArray();
        for (int i = 0; i < 2500; i++) {
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
            else {
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
        public int ok;
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
        public Badge badge;
    }
    [System.Serializable]
    public class Badge
    {
        public int type; // 2,
        public string color1; // #000000,
        public string color2; // #028300,
        public string color3; // #8b5c00,
        public int param; // 0,
        public bool flip; // false
    }
}

