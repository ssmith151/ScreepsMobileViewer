using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMap : MonoBehaviour
{

    public TileBase tile; // Assign a tile asset to this.
    //public TileBase tile2;
    TilemapRenderer tRenderer;
    RoomTerrain ter;
    string[,] terrainTypes;

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

    void GenerateTileMap() {
        ter = LoadTerrainFromFile();
        terrainTypes = SetTerrain(ter);
        int terrainLength = terrainTypes.GetLength(0) * terrainTypes.GetLength(1);
        var grid = this.gameObject.AddComponent<Grid>();
        grid.cellSize = new Vector3(0.16f, 0.16f, 1);
        Tilemap tilemap = new GameObject("Tilemap").AddComponent<Tilemap>();
        tilemap.transform.SetParent(grid.transform);
        tRenderer = tilemap.gameObject.AddComponent<TilemapRenderer>();
        tilemap.GetComponent<TilemapRenderer>().sortOrder = TilemapRenderer.SortOrder.TopLeft;
        TileBase[,] tileArray = new TileBase[terrainTypes.GetLength(0), terrainTypes.GetLength(1)];
        for (int x = 0; x < 50; x++) {
            for (int y = 0; y < 50; y++) {
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
        terrainTypes = SetTerrain(ter);
        int terrainLength = terrainTypes.GetLength(0) * terrainTypes.GetLength(1);
        Tilemap tilemap = this.gameObject.GetComponent<Tilemap>();
        tRenderer = tilemap.gameObject.GetComponent<TilemapRenderer>();
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
        //string savePath = "C:\\Users\\Ssmit\\Documents\\Unity\\ScreepsMobileViewer\\Assets\\Terrain1.JSON";
        //string terrainFile = File.ReadAllText(savePath);
        // terrainFile = "{\"result\":" + terrainFile.ToString() + "}";
        string terrainFile = "{\"result\": null}";
        RoomTerrain t = JsonUtility.FromJson<RoomTerrain>(terrainFile);
        return t;
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
