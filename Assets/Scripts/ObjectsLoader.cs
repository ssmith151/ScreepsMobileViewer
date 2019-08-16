using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using ScreepsViewer;

namespace ScreepsViewer
{
    public class ObjectsLoader : MonoBehaviour
    {
        [SerializeField]public static int gameTime;
        private bool controllerBadgeAssigned = false;

        public TileBase storageTile;
        public TileBase storageTileHL;
        public TileBase extensionTile;
        public TileBase extensionTileHL;
        public TileBase extensionFull;
        public TileBase extension50;
        public TileBase roadTile;
        public TileBase containerTile;
        public TileBase towerTile;
        public TileBase nukerTile;
        public TileBase nukerTileHL;
        public TileBase observerTile;
        public TileBase sourceTile;
        public TileBase sourceTileRec;
        public TileBase sourceTileRec50;
        public TileBase controllerTile;
        public TileBase spawnTile;
        public TileBase terminalTile;
        public TileBase terminalTileHL;
        public TileBase labTile;
        public TileBase labTileHL;
        public TileBase linkTile;
        public TileBase linkTileHL;
        public TileBase rampartTile;
        public TileBase constructedWallTile;
        public TileBase constructedWallTileHL;
        public TileBase portalTile;
        public TileBase flagTile;
        public TileBase flag2Tile;
        public TileBase labResourcesTile0;
        public TileBase labResourcesTile1;
        public TileBase labResourcesTile2;
        public TileBase labResourcesTile3;
        public TileBase labResourcesTile4;
        public TileBase labResourcesTile5;
        public TileBase labResourcesTile6;
        public TileBase labResourcesTile7;
        public TileBase labResourcesTile8;
        public TileBase extractorTile;
        public TileBase HminTile;
        public TileBase OminTile;
        public TileBase LminTile;
        public TileBase KminTile;
        public TileBase UminTile;
        public TileBase XminTile;
        public TileBase ZminTile;
        public TileBase tombStoneTile;
        public TileBase tombStoneTileHL;
        public TileBase tombStoneTileRec;
        public TileBase keeperLair;
        public TileBase keeperLairHL;
        public TileBase powerBankTile;
        public TileBase powerBankRec;
        public TileBase resourceTile;
        public TileBase nuke;
        public TileBase nukeGlow;
        public TileBase disabledTile;
        public Sprite invaderSprite;
        public GameObject creepPrefab;
        public GameObject roadPrefab;
        public GameObject towerPrefab;
        public GameObject nukerPrefab;
        public GameObject storagePrefab;
        public GameObject terminalPrefab;
        public GameObject constructionPrefab;
        public GameObject spawnPrefab;
        public GameObject controllerPrefab;
        public GameObject powerSpawnPrefab;
        public GameObject powerCreepPrefab;
        public GameObject factoryPrefab;
        public GameObject laserPrefab;

        private Tilemap tilemap;
        private Tilemap tilemapHL;
        private Tilemap tilemapResources;
        private Tilemap tilemapRamp;
        private Tilemap tilemapFlags;
        private Tilemap tilemapDis;

        TilemapRenderer tRenderer;
        TilemapRenderer tRendererHL;
        TilemapRenderer tRendererResources;
        TilemapRenderer tRendererRamp;
        TilemapRenderer tRendererFlags;
        TilemapRenderer tRendererDis;

        GameObject tileMapGO;
        GameObject tileMapGOHL;
        GameObject tileMapGOResources;
        GameObject tileMapGORamp;
        GameObject tileMapGOFlags;
        GameObject tileMapGODis;

        GameObject creepLayer;
        GameObject roadsLayer;

        public Material disabledMaterial;
        public Material safeModeMaterial;

        public Grid grid;

        private RoomObject ro;
        ConnectRecievedRoomObjects recTer;
        [SerializeField]
        private ConnectionController connectController;
        private ScreepsAPI api;
        private BadgeStorage bs;

        private JSONObject currentData;

        private Dictionary<string, RoomObject> objects = new Dictionary<string, RoomObject>();
        private Dictionary<string, GameObject> selectableGOs = new Dictionary<string, GameObject>();
        private Dictionary<string, GameObject> creeps = new Dictionary<string, GameObject>();
        private Dictionary<string, GameObject> sites = new Dictionary<string, GameObject>();
        private Dictionary<string, GameObject> lasers = new Dictionary<string, GameObject>();
        private Dictionary<string, Flag> flags = new Dictionary<string, Flag>();
        [SerializeField]
        public Dictionary<string, User> users = new Dictionary<string, User>();
        public Dictionary<string, List<string>> selectables;

        //colors of players
        [SerializeField]
        private Color playerColor = new Color32(30, 200, 30, 100);
        [SerializeField]
        private Color opponentColor = new Color32(255, 80, 80, 100);
        [SerializeField]
        private Color energyColor = new Color32(255, 255, 127, 255);


        // Start is called before the first frame update
        void Awake()
        {
            selectables = new Dictionary<string, List<string>>();
            grid = !!this.gameObject.GetComponent<Grid>() ? this.gameObject.GetComponent<Grid>() : this.gameObject.AddComponent<Grid>();
            grid.cellSize = new Vector3(0.16f, 0.16f, 1);
            GenerateTileMap();
            GenerateCreepLayer();
            GenerateRoadsLayer();
            connectController = GameObject.Find("ConnectionController").GetComponent<ConnectionController>();
            //http = connectController.GetComponent<ScreepsHTTP>();
            api = GameObject.Find("ConnectionController").GetComponent<ScreepsAPI>();
        }
        void GenerateCreepLayer()
        {
            creepLayer = new GameObject("Creepmap");
            creepLayer.transform.SetParent(grid.transform);
        }
        void GenerateRoadsLayer()
        {
            roadsLayer = new GameObject("roadmap");
            roadsLayer.transform.SetParent(grid.transform);
        }
        void GenerateTileMap()
        {
            // shadow layer
            tileMapGO = new GameObject("Tilemap2");
            tilemap = tileMapGO.AddComponent<Tilemap>();
            tRenderer = tilemap.gameObject.AddComponent<TilemapRenderer>();
            tRenderer.sortingOrder = 2;
            tilemap.GetComponent<TilemapRenderer>().sortOrder = TilemapRenderer.SortOrder.TopLeft;
            tilemap.transform.SetParent(grid.transform);
            // highlights layer
            tileMapGOHL = new GameObject("Tilemap3");
            tilemapHL = tileMapGOHL.AddComponent<Tilemap>();
            tilemapHL.transform.SetParent(grid.transform);
            tRendererHL = tilemapHL.gameObject.AddComponent<TilemapRenderer>();
            tRendererHL.sortingOrder = 3;
            tilemapHL.GetComponent<TilemapRenderer>().sortOrder = TilemapRenderer.SortOrder.TopLeft;
            // resources Layer
            tileMapGOResources = new GameObject("Tilemap4");
            tilemapResources = tileMapGOResources.AddComponent<Tilemap>();
            tilemapResources.transform.SetParent(grid.transform);
            tRendererResources = tilemapResources.gameObject.AddComponent<TilemapRenderer>();
            tRendererResources.sortingOrder = 4;
            tilemapResources.GetComponent<TilemapRenderer>().sortOrder = TilemapRenderer.SortOrder.TopLeft;
            // ramparts layer
            tileMapGORamp = new GameObject("Tilemap5");
            tilemapRamp = tileMapGORamp.AddComponent<Tilemap>();
            tilemapRamp.transform.SetParent(grid.transform);
            tRendererRamp = tilemapRamp.gameObject.AddComponent<TilemapRenderer>();
            tRendererRamp.sortingLayerName = "Ramparts";
            tRendererRamp.sortingOrder = 5;
            tilemapRamp.GetComponent<TilemapRenderer>().sortOrder = TilemapRenderer.SortOrder.TopLeft;
            // flags layer tilemapFlags
            tileMapGOFlags = new GameObject("Tilemap6");
            tilemapFlags = tileMapGOFlags.AddComponent<Tilemap>();
            tilemapFlags.transform.SetParent(grid.transform);
            tRendererFlags = tilemapFlags.gameObject.AddComponent<TilemapRenderer>();
            tRendererFlags.sortingOrder = 6;
            tilemapFlags.GetComponent<TilemapRenderer>().sortOrder = TilemapRenderer.SortOrder.TopLeft;
            // flags layer tilemapDisabled/nuke/powermine
            tileMapGODis = new GameObject("Tilemap7");
            tilemapDis = tileMapGODis.AddComponent<Tilemap>();
            tilemapDis.transform.SetParent(grid.transform);
            tRendererDis = tilemapDis.gameObject.AddComponent<TilemapRenderer>();
            tRendererDis.sortingOrder = 7;
            tilemapDis.GetComponent<TilemapRenderer>().sortOrder = TilemapRenderer.SortOrder.TopLeft;
            tRendererDis.material = disabledMaterial;
            //for (int x = 0; x < 50; x++)
            //{
            //    for (int y = 0; y < 50; y++)
            //    {
            //        //Vector3Int currentPos = new Vector3Int(x, -y+50, -1); // the z here is one up from terrain
            //        ////tilemap.SetTile(currentPos, tile);
            //        ////tilemap.SetTileFlags(currentPos, TileFlags.None);
            //        //if (x == 25 && y == 25)
            //        //{
            //        //    tilemap.SetTile(currentPos, storageTile);
            //        //}
            //        //if (x < 30 && y < 30 && (x + y) % 4 == 0)
            //        //{
            //        //    tilemap.SetTile(currentPos, extensionTile);
            //        //}
            //    }
            //}
            //tilemap.SetTiles(positions, tileArray);
        }
        //private void ClearTileMap()
        //{
        //    for (int x = 0; x< 50; x++)
        //    {
        //        for (int y = 0; y< 50; y++)
        //        {
        //            Vector3Int currentPos = new Vector3Int(x, y, -1); // the z here is one up from terrain
        //            tilemap.SetTile(currentPos, null);
        //        }
        //    }
        //}
        private void ClearCreepLayer()
        {
            for (int i = creepLayer.transform.childCount - 1; i >= 0; i--)
            {
                Transform child = creepLayer.transform.GetChild(i);
                Destroy(child.gameObject);
            }
        }
        private void ClearRoadsLayer()
        {
            for (int i = roadsLayer.transform.childCount - 1; i >= 0; i--)
            {
                Transform child = roadsLayer.transform.GetChild(i);
                Destroy(child.gameObject);
            }
        }
        //private void AssignRoomObjects(JSONObject obj)
        //{
        //    ro = JsonUtility.FromJson<RoomObject>(obj["objects"].ToString());
        //    Debug.Log("Assign Room : " + ro);
        //    //RoomParsedObjects[] oo = { o };
        //    //recTer = new ConnectRecievedRoomObjects
        //    //{
        //    //    ok = obj["ok"].str,
        //    //    terrain = oo
        //    //};
        //    UpdateRoomObjectsFromConnection();
        //}
        public void GetObjectsFromConnection()
        {
            ClearCreepLayer();
            ClearRoadsLayer();
            tilemap.ClearAllTiles();
            tilemapHL.ClearAllTiles();
            tilemapResources.ClearAllTiles();
            tilemapRamp.ClearAllTiles();
            tilemapFlags.ClearAllTiles();
            tilemapDis.ClearAllTiles();
            objects = new Dictionary<string, RoomObject>();
            creeps = new Dictionary<string, GameObject>();
            sites = new Dictionary<string, GameObject>();
            flags = new Dictionary<string, Flag>();
            users = new Dictionary<string, User>();
            lasers = new Dictionary<string, GameObject>();
            controllerBadgeAssigned = false;
            selectableGOs = new Dictionary<string, GameObject>();
            selectables = new Dictionary<string, List<string>>();
            Action<JSONObject> time = new Action<JSONObject>(GetTime);
            api.Http.GetTime(ConnectionController.shardName, time);
            Action<JSONObject> thisBlows = new Action<JSONObject>(OnRoomData);
            Debug.Log("get objects: " + thisBlows);
            api.Http.GetRoomObjects(ConnectionController.roomName, ConnectionController.shardName, thisBlows);
        }

        private void OnDestroy()
        {
            if (api.Socket != null)
            {
                string path = string.Format("room:{0}/{1}", ConnectionController.shardName, ConnectionController.shardName);
                api.Socket.Unsub(path);
            }
        }

        private void OnRoomData(JSONObject data)
        {
            currentData = data;
        }
        private void GetTime(JSONObject data)
        {
            Debug.Log("asking for time");
            if (data["time"] != null)
                data.GetField(ref gameTime, "time");
        }

        private void Update()
        {
            if (currentData != null)
            {
                if (currentData.HasField("gameTime"))
                {
                    int newTime = 0;//Int64.Parse(currentData["gameTime"].ToString());
                    currentData.GetField(ref newTime, "gameTime");
                    Debug.Log("GameTime : " + newTime);
                    gameTime = newTime;
                }
                RenderEntities();
                currentData = null;
            }
        }

        private void RenderEntities()
        {
            if (gameTime == 0)
            {
                //gameTime = int.Parse(currentData["gameTime"].ToString());
            }
            //tilemap.SetTile(new Vector3Int(25, 25, -1), roadTile);
            Debug.Log("currentData"+currentData);
            SetRoomUsers(currentData["users"]);
            var objects = currentData["objects"];
            List<RoomObject> roads = new List<RoomObject>();
            foreach (var id in objects.keys)
            {
                var obj = objects[id];
                if (!this.objects.ContainsKey(id) && obj)
                {
                    RoomObject newRo = new RoomObject(obj);
                    //int[] key = { newRo.x, newRo.y };
                    string xOut = newRo.x < 10 ? "0" + newRo.x.ToString() : newRo.x.ToString();
                    string yOut = newRo.y < 10 ? "0" + newRo.y.ToString() : newRo.y.ToString();
                    int yInverted = 50 - newRo.y;
                    string key = xOut + yOut;
                    if (!selectables.ContainsKey(key))
                    {
                        selectables[key] = new List<string>();
                    }
                    selectables[key].Add(id);
                    this.objects[id] = newRo;
                    Vector3Int currentPos = new Vector3Int(newRo.x, yInverted, -1);
                    if (newRo.type == null)
                    {
                        continue;
                    }
                    //if ()
                    if (obj.HasField("off"))
                    {
                        bool _off = false;
                        obj.GetField(ref _off, "off");
                        if (_off)
                        {
                            Vector3Int currentPosDis = new Vector3Int(newRo.x, yInverted, -7);
                            tilemapDis.SetTile(currentPosDis, disabledTile);
                        }
                    }
                    if (String.Equals(newRo.type, "extension".ToLowerInvariant()))
                    {
                        tilemap.SetTile(currentPos, extensionTile);
                        tilemap.SetTileFlags(currentPos, TileFlags.None);
                        tilemap.SetColor(currentPos, api.User.id == newRo._user ? playerColor : opponentColor);
                        Vector3Int currentPosHL = new Vector3Int(newRo.x, yInverted, -2);
                        tilemapHL.SetTile(currentPosHL, extensionTileHL);
                        Vector3Int currentPosRes = new Vector3Int(newRo.x, yInverted, -3);
                        int e = Int32.Parse(obj["energy"].ToString());
                        int eM = Int32.Parse(obj["energyCapacity"].ToString());
                        if (e > 0 && e < eM)
                        {
                            tilemapResources.SetTile(currentPosRes, extension50);
                        }
                        else if (e == eM)
                        {
                            tilemapResources.SetTile(currentPosRes, extensionFull);
                        }
                        //Debug.Log("extension is owned by player : "+(api.User._id == newRo._user));
                        //tilemap.SetColor(currentPos, api.User._id == newRo._user ? playerColor : opponentColor);
                    }
                    if (String.Equals(newRo.type, "storage".ToLowerInvariant()))
                    {
                        //tilemap.SetTile(currentPos, storageTile);
                        //Vector3Int currentPosHL = new Vector3Int(newRo.x, newRo.y, -2);
                        //tilemapHL.SetTile(currentPosHL, storageTileHL);
                        //tilemap.SetTileFlags(currentPos, TileFlags.None);
                        //tilemap.SetColor(currentPos, api.User._id == newRo._user ? playerColor : opponentColor);
                        Vector3 storagePos = new Vector3(currentPos.x * 0.16f + 0.08f,
                            currentPos.y * 0.16f + 0.08f, -1);
                        GameObject storage = GameObject.Instantiate(storagePrefab, storagePos, new Quaternion());
                        storage.name = newRo._id;
                        storage.transform.SetParent(creepLayer.transform);
                        Storage sCode = storage.GetComponent<Storage>();
                        storage.GetComponent<SpriteRenderer>().color = api.User.id == newRo._user ? playerColor : opponentColor;
                        sCode.StorageData(obj);
                        selectableGOs.Add(id, storage);
                        //Debug.Log(obj);
                    }
                    if (String.Equals(newRo.type, "road".ToLowerInvariant()))
                    {
                        //tilemap.SetTile(currentPos, roadTile);
                        roads.Add(newRo);
                    }
                    if (String.Equals(newRo.type, "mineral".ToLowerInvariant()))
                    {
                        string minType = obj["mineralType"].ToString().Replace("\"", "");
                        switch (minType)
                        {
                            case "H":
                                tilemap.SetTile(currentPos, HminTile);
                                break;
                            case "O":
                                tilemap.SetTile(currentPos, OminTile);
                                break;
                            case "L":
                                tilemap.SetTile(currentPos, LminTile);
                                break;
                            case "K":
                                tilemap.SetTile(currentPos, KminTile);
                                break;
                            case "U":
                                tilemap.SetTile(currentPos, UminTile);
                                break;
                            case "X":
                                tilemap.SetTile(currentPos, XminTile);
                                break;
                            case "Z":
                                tilemap.SetTile(currentPos, ZminTile);
                                break;
                            default:
                                tilemap.SetTile(currentPos, HminTile);
                                break;
                        }
                    }
                    if (String.Equals(newRo.type, "tower".ToLowerInvariant()))
                    {
                        tilemap.SetTile(currentPos, towerTile);
                        tilemap.SetTileFlags(currentPos, TileFlags.None);
                        tilemap.SetColor(currentPos, api.User.id == newRo._user ? playerColor : opponentColor);
                        Vector3 towerPos = new Vector3(currentPos.x * 0.16f + 0.08f,
                            currentPos.y * 0.16f + 0.08f, -2);
                        GameObject tower = GameObject.Instantiate(towerPrefab, towerPos, new Quaternion());
                        tower.name = newRo._id;
                        tower.transform.SetParent(creepLayer.transform);
                        Tower tCode = tower.GetComponent<Tower>();
                        tCode.TowerData(obj);
                        selectableGOs.Add(id, tower);
                    }
                    if (String.Equals(newRo.type, "nuker".ToLowerInvariant()))
                    {
                        Vector3 nukerPos = new Vector3(currentPos.x * 0.16f + 0.08f,
                            currentPos.y * 0.16f + 0.08f, currentPos.z);
                        GameObject nuker = GameObject.Instantiate(nukerPrefab, nukerPos, new Quaternion());
                        nuker.name = newRo._id;
                        nuker.transform.SetParent(creepLayer.transform);
                        nuker.GetComponent<Nuker>().SetEnergy(Int32.Parse(obj["energy"].ToString()));
                        nuker.GetComponent<SpriteRenderer>().color = api.User.id == newRo._user ? playerColor : opponentColor;
                        selectableGOs.Add(id, nuker);
                    }
                    if (String.Equals(newRo.type, "observer".ToLowerInvariant()))
                    {
                        tilemap.SetTile(currentPos, observerTile);
                    }
                    if (String.Equals(newRo.type, "source".ToLowerInvariant()))
                    {
                        tilemap.SetTile(currentPos, sourceTile);
                        int maxE = Int32.Parse(obj["energyCapacity"].ToString());
                        int energy = Int32.Parse(obj["energy"].ToString());
                        maxE = maxE > 0 ? maxE : 1500;
                        energy = energy > 0 ? energy : 0;
                        Vector3Int currentPosRes = new Vector3Int(newRo.x, yInverted, -3);
                        if (energy < maxE && energy != 0)
                        {
                            tilemapResources.SetTile(currentPosRes, sourceTileRec50);
                        }
                        if (energy == maxE)
                        {
                            tilemapResources.SetTile(currentPosRes, sourceTileRec);
                        }
                    }
                    if (newRo.type == "powerBank")
                    {
                        tilemap.SetTile(currentPos, powerBankTile);
                        Vector3Int currentPosRes = new Vector3Int(newRo.x, yInverted, -3);
                        tilemapResources.SetTile(currentPosRes, powerBankRec);
                    }
                    if (newRo.type == "constructedWall")
                    {
                        tilemap.SetTile(currentPos, constructedWallTile);
                        Vector3Int currentPosHL = new Vector3Int(newRo.x, yInverted, -2);
                        tilemapHL.SetTile(currentPosHL, constructedWallTileHL);
                    }
                    if (String.Equals(newRo.type, "container".ToLowerInvariant()))
                    {
                        tilemap.SetTile(currentPos, containerTile);
                        int e = Int32.Parse(obj["energy"].ToString());
                        if (e > 0 && e < 2000)
                        {
                            Vector3Int currentPosRes = new Vector3Int(newRo.x, yInverted, -3);
                            tilemapHL.SetTile(currentPosRes, extension50);
                        }
                        else if (e >= 2000)
                        {
                            Vector3Int currentPosRes = new Vector3Int(newRo.x, yInverted, -3);
                            tilemapHL.SetTile(currentPosRes, sourceTileRec);
                        }

                    }
                    if (String.Equals(newRo.type, "tombstone".ToLowerInvariant()))
                    {
                        tilemap.SetTile(currentPos, tombStoneTile);
                        Vector3Int currentPosHL = new Vector3Int(newRo.x, yInverted, -2);
                        tilemapHL.SetTile(currentPosHL, tombStoneTileHL);
                        if (obj["energy"] != null && Int32.Parse(obj["energy"].ToString()) > 0)
                        {
                            Vector3Int currentPosRes = new Vector3Int(newRo.x, yInverted, -3);
                            tilemapResources.SetTile(currentPosRes, tombStoneTileRec);
                            tilemapResources.SetTileFlags(currentPosRes, TileFlags.None);
                            tilemapResources.SetColor(currentPosRes, api.User.id == newRo._user ? playerColor : opponentColor);
                            // todo : update with all resources
                            // todo : remove this tile when the resources are gone;
                        }
                        tilemap.SetTileFlags(currentPos, TileFlags.None);
                        tilemap.SetColor(currentPos, api.User.id == newRo._user ? playerColor : opponentColor);
                    }
                    if (String.Equals(newRo.type, "resource".ToLowerInvariant()))
                    {
                        Vector3Int currentPosHL = new Vector3Int(newRo.x, yInverted, -2);
                        tilemapHL.SetTile(currentPosHL, resourceTile);
                        if (obj["energy"] == null) { continue;  }
                        int energy = Int32.Parse(obj["energy"].ToString());
                        //Vector3Int currentPosRes = new Vector3Int(newRo.x, yInverted, -3);
                        //tilemapResources.SetTile(currentPosRes, tombStoneTileRec);
                        tilemapHL.SetTileFlags(currentPosHL, TileFlags.None);
                        tilemapHL.SetColor(currentPosHL, energy > 0 ? energyColor : Color.white);
                            // todo : update with all resources
                            // todo : remove this tile when the resources are gone;
                    }
                    if (String.Equals(newRo.type, "nuke".ToLowerInvariant()))
                    {
                        tilemap.SetTile(currentPos, nuke);
                        Vector3Int currentPosDis = new Vector3Int(newRo.x, yInverted, -6);
                        tilemapDis.SetTile(currentPosDis, nukeGlow);
                    }
                    if (newRo.type == "keeperLair")
                    {
                        tilemap.SetTile(currentPos, keeperLair);
                        Vector3Int currentPosHL = new Vector3Int(newRo.x, yInverted, -2);
                        tilemapHL.SetTile(currentPosHL, keeperLairHL);
                    }
                    if (String.Equals(newRo.type, "extractor".ToLowerInvariant()))
                    {
                        Vector3Int currentPosHL = new Vector3Int(newRo.x, yInverted, -2);
                        tilemapHL.SetTile(currentPosHL, extractorTile);
                    }
                    if (String.Equals(newRo.type, "controller".ToLowerInvariant()))
                    {
                        //tilemap.SetTile(currentPos, controllerTile);
                        //tilemap.SetTileFlags(currentPos, TileFlags.None);
                        //tilemap.SetColor(currentPos, api.User._id == newRo._user ? playerColor : opponentColor);
                        //Vector3Int currentPosHL = new Vector3Int(newRo.x, yInverted, -2);
                        //Vector3Int currentPosRec = new Vector3Int(newRo.x, yInverted, -3);
                        //tilemapResources.SetTile(currentPosRec, controllerBadgeTile);
                        Vector3 spawnPos = new Vector3(currentPos.x * 0.16f + 0.08f,
                            currentPos.y * 0.16f + 0.08f, -1);
                        GameObject controller = GameObject.Instantiate(controllerPrefab, spawnPos, new Quaternion());
                        controller.transform.SetParent(creepLayer.transform);
                        Controller sCode = controller.GetComponent<Controller>();
                        controller.GetComponent<SpriteRenderer>().color = api.User.id == newRo._user ? playerColor : opponentColor;
                        sCode.ControllerData(obj);
                        selectableGOs.Add(id, controller);
                    }
                    if (String.Equals(newRo.type, "factory".ToLowerInvariant()))
                    {
                        Vector3 spawnPos = new Vector3(currentPos.x * 0.16f + 0.08f,
                            currentPos.y * 0.16f + 0.08f, -1);
                        GameObject factory = GameObject.Instantiate(factoryPrefab, spawnPos, new Quaternion());
                        factory.name = newRo._id;
                        factory.transform.SetParent(creepLayer.transform);
                        Factory fCode = factory.GetComponent<Factory>();
                        factory.GetComponent<SpriteRenderer>().color = api.User.id == newRo._user ? playerColor : opponentColor;
                        fCode.FactoryData(obj);
                        selectableGOs.Add(id, factory);
                    }
                    if (String.Equals(newRo.type, "spawn".ToLowerInvariant()))
                    {
                        Vector3 spawnPos = new Vector3(currentPos.x * 0.16f + 0.08f,
                            currentPos.y * 0.16f + 0.08f, -1);
                        GameObject spawn = GameObject.Instantiate(spawnPrefab, spawnPos, new Quaternion());
                        spawn.name = newRo._id;
                        spawn.transform.SetParent(creepLayer.transform);
                        Spawn sCode = spawn.GetComponent<Spawn>();
                        spawn.GetComponent<SpriteRenderer>().color = api.User.id == newRo._user ? playerColor : opponentColor;
                        sCode.SpawnData(obj);
                        selectableGOs.Add(id, spawn);
                    }
                    if (newRo.type == "powerSpawn")
                    {
                        Vector3 spawnPos = new Vector3(currentPos.x * 0.16f + 0.08f,
                            currentPos.y * 0.16f + 0.08f, -1);
                        GameObject spawn = GameObject.Instantiate(powerSpawnPrefab, spawnPos, new Quaternion());
                        spawn.name = newRo._id;
                        spawn.transform.SetParent(creepLayer.transform);
                        PowerSpawn sCode = spawn.GetComponent<PowerSpawn>();
                        spawn.GetComponent<SpriteRenderer>().color = api.User.id == newRo._user ? playerColor : opponentColor;
                        sCode.SpawnData(obj);
                        selectableGOs.Add(id, spawn);
                    }
                    if (String.Equals(newRo.type, "terminal".ToLowerInvariant()))
                    {
                        //tilemap.SetTile(currentPos, terminalTile);
                        //Vector3Int currentPosHL = new Vector3Int(newRo.x, yInverted, -2);
                        //tilemapHL.SetTile(currentPosHL, terminalTileHL);
                        //tilemap.SetTileFlags(currentPos, TileFlags.None);
                        //tilemap.SetColor(currentPos, api.User._id == newRo.user ? playerColor : opponentColor);
                        Vector3 terminalPos = new Vector3(currentPos.x * 0.16f + 0.08f,
                            currentPos.y * 0.16f + 0.08f, -1);
                        GameObject terminal = GameObject.Instantiate(terminalPrefab, terminalPos, new Quaternion());
                        terminal.name = newRo._id;
                        terminal.transform.SetParent(creepLayer.transform);
                        Terminal sCode = terminal.GetComponent<Terminal>();
                        terminal.GetComponent<SpriteRenderer>().color = api.User.id == newRo._user ? playerColor : opponentColor;
                        sCode.StorageData(obj);
                        selectableGOs.Add(id, terminal);
                    }
                    if (String.Equals(newRo.type, "lab".ToLowerInvariant()))
                    {
                        tilemap.SetTile(currentPos, labTile);
                        Vector3Int currentPosHL = new Vector3Int(newRo.x, yInverted, -2);
                        tilemapHL.SetTile(currentPosHL, labTileHL);
                        tilemap.SetTileFlags(currentPos, TileFlags.None);
                        tilemap.SetColor(currentPos, api.User.id == newRo._user ? playerColor : opponentColor);
                        int mineralAmount = Int32.Parse(obj["mineralAmount"].ToString());
                        int energy = Int32.Parse(obj["energy"].ToString());
                        Vector3Int currentPosRec = new Vector3Int(newRo.x, yInverted, -3);
                        if (mineralAmount == 0 && energy == 0)
                        {
                            tilemapResources.SetTile(currentPosRec, labResourcesTile0);
                        }
                        else if (mineralAmount > 0 && mineralAmount < 3000 && energy == 0)
                        {
                            tilemapResources.SetTile(currentPosRec, labResourcesTile1);
                        }
                        else if (mineralAmount == 3000 && energy == 0)
                        {
                            tilemapResources.SetTile(currentPosRec, labResourcesTile2);
                        }
                        else if (mineralAmount == 0 && energy > 0 && energy < 2000)
                        {
                            tilemapResources.SetTile(currentPosRec, labResourcesTile3);
                        }
                        else if (mineralAmount > 0 && mineralAmount < 3000 && energy > 0 && energy < 2000)
                        {
                            tilemapResources.SetTile(currentPosRec, labResourcesTile4);
                        }
                        else if (mineralAmount == 3000 && energy > 0 && energy < 2000)
                        {
                            tilemapResources.SetTile(currentPosRec, labResourcesTile5);
                        }
                        else if (mineralAmount == 0 && energy == 2000)
                        {
                            tilemapResources.SetTile(currentPosRec, labResourcesTile6);
                        }
                        else if (mineralAmount > 0 && mineralAmount < 3000 && energy == 2000)
                        {
                            tilemapResources.SetTile(currentPosRec, labResourcesTile7);
                        }
                        else if (mineralAmount == 3000 && energy == 2000)
                        {
                            tilemapResources.SetTile(currentPosRec, labResourcesTile8);
                        }
                    }
                    if (String.Equals(newRo.type, "portal".ToLowerInvariant()))
                    {
                        tilemap.SetTile(currentPos, portalTile);
                    }
                    if (String.Equals(newRo.type, "link".ToLowerInvariant()))
                    {
                        tilemap.SetTile(currentPos, linkTile);
                        Vector3Int currentPosHL = new Vector3Int(newRo.x, yInverted, -2);
                        tilemapHL.SetTile(currentPosHL, linkTileHL);
                        tilemap.SetTileFlags(currentPos, TileFlags.None);
                        tilemap.SetColor(currentPos, api.User.id == newRo._user ? playerColor : opponentColor);
                    }
                    if (String.Equals(newRo.type, "creep".ToLowerInvariant()))
                    {
                        Vector3 creepPos = new Vector3(currentPos.x * 0.16f + 0.08f,
                            currentPos.y * 0.16f + 0.08f, currentPos.z);
                        GameObject creep = GameObject.Instantiate(creepPrefab, creepPos, new Quaternion());
                        creep.transform.SetParent(creepLayer.transform);
                        Creep creepSc = creep.GetComponent<Creep>();
                        creepSc.CreepData(obj);
                        creep.name = newRo._id;
                        float bodySize = creepSc.creepBody.parts.Length > 0 ? creepSc.creepBody.parts.Length : 10;
                        if (bodySize > 0)
                        {
                            creep.transform.localScale = new Vector3(Mathf.Pow(0.5f + (bodySize / 50.0f), ((1.0f / bodySize) + 0.5f) * 1.5f), Mathf.Pow(0.5f + (bodySize / 50.0f), ((1.0f / bodySize) + 0.5f) * 1.5f), 1);
                        }
                        SpriteRenderer creepSr = creep.GetComponent<SpriteRenderer>();
                        creepSr.color = api.User.id == newRo._user ? playerColor : opponentColor;
                        if (newRo._user == "2" || newRo._user == "3")
                        {
                            creepSr.sprite = invaderSprite;
                            creepSc.creepEnergy.SetActive(false);
                        }
                        else
                        {
                            if (users.ContainsKey(newRo._user) && users[newRo._user].badgePNG != null)
                            {
                                creepSc.SetBadge(users[newRo._user].badgePNG);
                                Debug.Log("setting badge in loader...");
                            }
                        }
                        creeps[id] = creep;
                    }
                    if (newRo.type == "powerCreep")
                    {
                        Vector3 creepPos = new Vector3(currentPos.x * 0.16f + 0.08f,
                            currentPos.y * 0.16f + 0.08f, currentPos.z);
                        GameObject creep = GameObject.Instantiate(powerCreepPrefab , creepPos, new Quaternion());
                        creep.transform.SetParent(creepLayer.transform);
                        creep.name = newRo._id;
                        PowerCreep creepSc = creep.GetComponent<PowerCreep>();
                        creepSc.CreepData(obj);
                        SpriteRenderer creepSr = creep.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
                        creepSr.color = api.User.id == newRo._user ? playerColor : opponentColor;
                        if (users.ContainsKey(newRo._user) && users[newRo._user].badgePNG != null)
                        {
                            creepSc.SetBadge(users[newRo._user].badgePNG);
                            Debug.Log("setting badge in loader...");
                        }
                        creeps[id] = creep;
                    }
                    if (String.Equals(newRo.type.ToLowerInvariant(), "constructionsite".ToLowerInvariant()))
                    {
                        Vector3 sitePos = new Vector3(currentPos.x * 0.16f + 0.08f,
                            currentPos.y * 0.16f + 0.08f, currentPos.z);
                        GameObject site = GameObject.Instantiate(constructionPrefab, sitePos, new Quaternion());
                        site.name = newRo._id;
                        site.transform.SetParent(creepLayer.transform);
                        site.GetComponent<constructionSite>().SiteData(obj);
                        sites[id] = site;
                    }
                    if (String.Equals(newRo.type, "rampart".ToLowerInvariant()))
                    {
                        Vector3Int currentPosRamp = new Vector3Int(newRo.x, yInverted, -4);//ramps on own layer
                        tilemapRamp.SetTile(currentPosRamp, rampartTile);
                        tilemapRamp.SetTileFlags(currentPosRamp, TileFlags.None);
                        tilemapRamp.SetColor(currentPosRamp, api.User.id == newRo._user ? playerColor : opponentColor);
                    }
                }
                else if (this.objects.ContainsKey(id) && (this.objects[id].type == "creep" || this.objects[id].type == "invader" || this.objects[id].type == "powercreep") )
                {
                    if (objects[id] == null || objects[id].type == JSONObject.Type.NULL)
                    {
                        //Creep _creep = creeps[id].GetComponent<Creep>();
                        //_creep.DeleteCreep();
                        selectableGOs.Remove(id);
                        this.objects.Remove(id);
                        if (creeps.ContainsKey(id))
                        {
                            GameObject _creep = creeps[id];
                            creeps.Remove(id);
                            Destroy(_creep, 2f);
                        }
                        continue;
                    }
                    if (obj != null && (obj["x"] != null || obj["y"] != null))
                    {
                        var oldRo = this.objects[id];
                        string oldxOut = oldRo.x < 10 ? "0" + oldRo.x.ToString() : oldRo.x.ToString();
                        string oldyOut = oldRo.y < 10 ? "0" + oldRo.y.ToString() : oldRo.y.ToString();
                        string oldkey = oldxOut + oldyOut;
                        if (selectables[oldkey].Contains(id))
                        {
                            int oldIndex = selectables[oldkey].IndexOf(id);
                            selectables[oldkey].RemoveAt(oldIndex);
                        }
                        if (obj["x"] != null)
                        {
                            oldRo.x = Int32.Parse(obj["x"].ToString());
                        }
                        if (obj["y"] != null)
                        {
                            oldRo.y = Int32.Parse(obj["y"].ToString());
                        }
                        string xOut = oldRo.x < 10 ? "0" + oldRo.x.ToString() : oldRo.x.ToString();
                        string yOut = oldRo.y < 10 ? "0" + oldRo.y.ToString() : oldRo.y.ToString();
                        string key = xOut + yOut;
                        if (!selectables.ContainsKey(key))
                        {
                            selectables[key] = new List<string>();
                        }
                        if (selectables.ContainsKey(key))
                        {
                            selectables[key].Add(id);
                            //Debug.Log("found new creep pos : " + key);
                        }
                        else
                        {
                            Debug.Log("did not find new creep pos : " + key);
                        }
                        this.objects[id] = oldRo;
                    }
                    if (creeps.ContainsKey(id))
                    {
                        if (creeps[id] == null)
                        {
                            creeps.Remove(id);
                            continue;
                        }
                        Creep _creep = creeps[id].GetComponent<Creep>();
                        if (_creep != null)
                        {
                            UpdateObject(id, obj);
                            _creep.CreepData(obj);
                        } else
                        {
                            try
                            {
                                PowerCreep _pCreep = creeps[id].GetComponent<PowerCreep>();
                                _pCreep.CreepData(obj);
                            } catch
                            {
                                Debug.Log("could not find class of creep in creeps");
                            }
                        }
                    }
                }
                else if (this.objects.ContainsKey(id))
                {
                    if (this.objects[id].type == "constructionSite" && sites[id])
                        sites[id].GetComponent<constructionSite>().SiteData(obj);
                    UpdateObject(id, obj);
                }
                if (!this.objects.ContainsKey(id)) { continue;  }
                if (this.objects[id].type == "spawn" || this.objects[id].type == "powerSpawn" || this.objects[id].type == "terminal" || this.objects[id].type == "storage" || this.objects[id].type == "tower" || this.objects[id].type == "nuker" || this.objects[id].type == "controller")
                {
                    if (obj == null || obj.type == JSONObject.Type.NULL)
                    {
                        //Creep _creep = creeps[id].GetComponent<Creep>();
                        //_creep.DeleteCreep();
                        this.objects.Remove(id);
                        Destroy(selectableGOs[id], 2f);
                        selectableGOs.Remove(id);
                        continue;
                    }
                    if (selectableGOs[id])
                    {
                        //Debug.Log("found an object : " + this.objects[id].type);
                        switch (this.objects[id].type)
                        {
                            case "nuker":
                                break;
                            case "spawn":
                                selectableGOs[id].GetComponent<Spawn>().SpawnData(obj);
                                break;
                            case "powerSpawn":
                                selectableGOs[id].GetComponent<PowerSpawn>().SpawnData(obj);
                                break;
                            case "storage":
                                selectableGOs[id].GetComponent<Storage>().StorageData(obj);
                                break;
                            case "controller":
                                selectableGOs[id].GetComponent<Controller>().ControllerData(obj);
                                break;
                            case "terminal":
                                selectableGOs[id].GetComponent<Terminal>().StorageData(obj);
                                break;
                            case "tower":
                                selectableGOs[id].GetComponent<Tower>().TowerData(obj);
                                break;
                        }
                    }
                }
                if (this.objects[id].type == "link")
                {
                    if (obj.HasField("actionLog"))
                    {
                        LinkLaser(id, obj["actionLog"]);
                    }
                }
            }
            if (currentData["flags"] != null)
            {
                //"flags":"Source10Spawn1~6~6~40~31"
                string[] _flags = currentData["flags"].ToString().Replace("\"", "").Split('|');
                foreach (string f in _flags)
                {
                    Flag flag = new Flag(f, api.User.id);
                    // if the position and the key are the same, do next item
                    if (flags.ContainsKey(flag.name) && (flags[flag.name].x == flag.x || flags[flag.name].y == flag.y))
                    {
                        continue;
                    }
                    if (!this.objects.ContainsKey(flag.name))
                    {
                        this.objects[flag.name] = flag;
                    }
                    string xOut = flag.x < 10 ? "0" + flag.x.ToString() : flag.x.ToString();
                    string yOut = flag.y < 10 ? "0" + flag.y.ToString() : flag.y.ToString();
                    int yInverted = 50 - flag.y;
                    string key = xOut + yOut;
                    if (!selectables.ContainsKey(key))
                    {
                        selectables[key] = new List<string>();
                    }
                    // if the position of the flag changes after it was placed, then it has to be removed from flags and selectables
                    if (flags.ContainsKey(flag.name) && (flags[flag.name].x != flag.x || flags[flag.name].y != flag.y))
                    {
                        Flag oldFlag = flags[flag.name];
                        string xOldOut = flag.x < 10 ? "0" + flag.x.ToString() : flag.x.ToString();
                        string yOldOut = flag.y < 10 ? "0" + flag.y.ToString() : flag.y.ToString();
                        string oldKey = xOut + yOut;
                        if (selectables[oldKey].Contains(flag.name))
                        {
                            selectables[oldKey].Remove(flag.name);
                        }
                        flags.Remove(flag.name);
                        this.objects.Remove(flag.name);
                    }
                    flags.Add(flag.name,flag);
                    if (!selectables[key].Contains(flag.name))
                    {
                        selectables[key].Add(flag.name);
                        Vector3Int currentPos = new Vector3Int(flag.x, yInverted, -5);
                        tilemapFlags.SetTile(currentPos, flagTile);
                        tilemapFlags.SetTileFlags(currentPos, TileFlags.None);
                        tilemapFlags.SetColor(currentPos, flag.primary);
                        Vector3Int currentPosHL = new Vector3Int(flag.x, yInverted, -6);
                        tilemapFlags.SetTile(currentPosHL, flag2Tile);
                        tilemapFlags.SetTileFlags(currentPosHL, TileFlags.None);
                        tilemapFlags.SetColor(currentPosHL, flag.secondary);
                    }
                }
            }
            // for each road
            if (roads.Count <= 0) { return; }
            for (var i = 0; i < roads.Count; i++)
            {
                int rx = roads[i].x;
                int ry = roads[i].y;
                // see if there are roads within one space
                List<RoomObject> filteredRoads = roads.Where(o => o.x == rx - 1 && o.y == ry ||
                o.x == rx + 1 && o.y == ry || o.x == rx - 1 && o.y == ry - 1 || o.x == rx + 1 && o.y == ry - 1 ||
                o.x == rx - 1 && o.y == ry + 1 || o.x == rx + 1 && o.y == ry + 1 || o.x == rx && o.y == ry + 1 ||
                o.x == rx && o.y == ry - 1).ToList<RoomObject>();
                if (filteredRoads.Count > 0)
                {
                    // draw a link between them
                    foreach (RoomObject filteredRoad in filteredRoads)
                    {
                        Vector3[] positions = { new Vector3(rx * 0.16f + 0.08f, (50-ry) * 0.16f + 0.08f, -0.5f),
                        new Vector3(filteredRoad.x * 0.16f + 0.08f, (50-filteredRoad.y) * 0.16f + 0.08f, -0.5f) };
                        GameObject road = GameObject.Instantiate(roadPrefab);
                        road.transform.SetParent(roadsLayer.transform);
                        LineRenderer line = road.GetComponent<LineRenderer>();
                        line.SetPositions(positions);
                    }
                }
            }
        }
        private void ClearTilesAtLocation(string keyIn)
        {
            int x = Int32.Parse(keyIn.Substring(0, 2));
            int y = Int32.Parse(keyIn.Substring(2, 2));
            ClearTilesAtLocation(x, y);
        }
        private void ClearTilesAtLocation(int xIn, int yIn)
        {
            yIn = 50 - yIn;
            Vector3Int currentPos = new Vector3Int(xIn, yIn, -1);
            tilemap.SetTile(currentPos,null);
            Vector3Int currentPosHL = new Vector3Int(xIn, yIn, -2);
            tilemapHL.SetTile(currentPosHL, null);
            Vector3Int currentPosRes = new Vector3Int(xIn, yIn, -3);
            tilemapResources.SetTile(currentPosRes, null);
            Vector3Int currentPosRamp = new Vector3Int(xIn, yIn, -4);
            tilemapRamp.SetTile(currentPosRamp, null);
            Vector3Int currentPosFlag = new Vector3Int(xIn, yIn, -5);
            tilemapFlags.SetTile(currentPos, null);
            Vector3Int currentPosFlagHL = new Vector3Int(xIn, yIn, -6);
            tilemapFlags.SetTile(currentPosHL, null);
            Vector3Int currentPosDis = new Vector3Int(xIn, yIn, -7);
            tilemapDis.SetTile(currentPosDis, null);
        }
        private void LinkLaser(string idIn, JSONObject objIn)
        {
            if (!objIn.HasField("transferEnergy") || objIn["transferEnergy"].type == JSONObject.Type.NULL)
                return;
            RoomObject thisLink = GetObjectByID(idIn);
            int tarX = 0;
            int tarY = 0;
            if (objIn["transferEnergy"].HasField("x"))
                objIn["transferEnergy"].GetField(ref tarX, "x");
            if (objIn["transferEnergy"].HasField("y"))
                objIn["transferEnergy"].GetField(ref tarY, "y");
            tarY = 50 - tarY;
            Vector2Int linkPos = new Vector2Int(thisLink.x, 50 - thisLink.y);
            Vector2Int targetPos = new Vector2Int(tarX, tarY);
            //Vector2Int creepPos = new Vector2Int(newPosition.x, newPosition.y);
            if (!lasers.ContainsKey(idIn))
            {
                lasers[idIn] = Instantiate(laserPrefab, creepLayer.transform);
                lasers[idIn].GetComponent<Laser>().Load(linkPos, targetPos, "repair");
            }
            else
            {
                lasers[idIn].SetActive(true);
                lasers[idIn].GetComponent<Laser>().Load(linkPos, targetPos, "repair");
            }
        }
        private void UpdateObject(string idIn, JSONObject obj)
        {
            RoomObject oldRo = GetObjectByID(idIn);
            //remove an item that was destroyed
            if (obj.type == JSONObject.Type.NULL)
            {
                if (oldRo.x > 0 && oldRo.y > 0)
                {
                    string xOut = oldRo.x < 10 ? "0" + oldRo.x.ToString() : oldRo.x.ToString();
                    string yOut = oldRo.y < 10 ? "0" + oldRo.y.ToString() : oldRo.y.ToString();
                    string key = xOut + yOut;
                    if (selectables.ContainsKey(key)) {
                        if (selectables[key].Contains(idIn))
                            selectables[key].Remove(idIn);
                        if (selectables[key].Count <= 0)
                            selectables[key] = null;
                    };
                    try
                    {
                        GameObject removeObj = GameObject.Find(oldRo._id);
                        Destroy(removeObj);
                    }
                    catch
                    {
                        if (selectables[key].Count <= 0)
                            ClearTilesAtLocation(oldRo.x, oldRo.y);
                        // else 
                            // determine layer to be removed by type
                    }
                    objects.Remove(idIn);
                    return;
                }
            }
            //int yInverted = 50 - oldRo.y;
            //Vector3Int currentPos = new Vector3Int(oldRo.x, yInverted, -1);
            if (oldRo.type == null)
            {
                return;
            }
            oldRo.UpdateRoomObject(obj);
            objects[idIn] = oldRo;
        }
        private void SetRoomUsers(JSONObject userData)
        {
            if (users.Count > 0 && !controllerBadgeAssigned)
            {
                var controller = objects.Where(kvp => kvp.Value.type == "controller");
                if (controller.Count() > 0 )
                {

                }
                TileData td = new TileData();
                td.sprite = users[users.Keys.First()].badgePNG;
                if (users[users.Keys.First()].badgePNG != null)
                {
                    controllerBadgeAssigned = true;
                }
            }
            if (userData == null)
            {
                return;
            }
            foreach (var _user in userData.keys)
            {
                string _userId = userData[_user]["_id"].ToString().ToLowerInvariant().Replace("\"", "");
                if (users.ContainsKey(_userId))
                {
                    continue;
                }
                string _userName = "";
                userData[_user].GetField(ref _userName, "username");//.ToString().ToLowerInvariant().Replace("\"", "");
                JSONObject _badge = userData[_user]["badge"];
                User roomUser = new User(_userId, _userName, _badge);
                if (roomUser != null)
                {
                    Debug.Log("setting new room user");
                    users[_user] = roomUser;
                }
            }
        }
        public RoomObject GetObjectByID(string idIn)
        {
            if (objects.ContainsKey(idIn))
                return objects[idIn];
            else
                return null;
        }
        public RoomObject[] GetSelectablesAtTile(string tileIn)
        {
            int xIn = Int32.Parse(tileIn.Substring(0,2));
            int yIn = Int32.Parse(tileIn.Substring(2,2));
            if (xIn < 0 || xIn > 49 || yIn < 0 || yIn > 49)
            {
                Debug.Log("pos : " + xIn + " " + yIn + " Out of bounds for sub room" );
                return null;
            }
            List<RoomObject> selectableRos = new List<RoomObject>();
            if (selectables.Count <=0  || !selectables.ContainsKey(tileIn))
            {
                Debug.Log("pos : " + xIn + " " + yIn + " key? "+selectables.ContainsKey(tileIn) +" count"+ selectables.Count);
                return null;
            }
            foreach (string index in selectables[tileIn])
            {
                Debug.Log("index of seletable Clicked : " + index);
                if (objects.ContainsKey(index))
                    selectableRos.Add(objects[index]);
                // else
                    // remove object
            }
            Debug.Log("pos : "+ xIn + " " + yIn + " ros "+selectableRos);
            return selectableRos.ToArray();
        }
        public Creep GetSelectedCreep(string idIn)
        {
            Creep selectedCreep = null;
            if (idIn != null && creeps[idIn] != null)
            {
                selectedCreep = creeps[idIn].GetComponent<Creep>();
                if (selectedCreep != null)
                {
                    return selectedCreep;
                }
            }
            return selectedCreep;
        }
        public GameObject GetSelectedGameObject(string idIn)
        {
            GameObject selectedGO = null;
            if (idIn != null && selectableGOs[idIn] != null)
            {
                return selectableGOs[idIn];
            }
            return selectedGO;
        }

        //void OnDrawGizmosSelected()
        //{
        //    // Draw a semitransparent blue cube at the transforms position
        //    Gizmos.color = new Color(1, 0, 0, 0.5f);
        //    foreach (KeyValuePair<string, List<string>> s in selectables)
        //    {
        //        int xIn = Int32.Parse(s.Key.Substring(0, 2));
        //        int yIn = Int32.Parse(s.Key.Substring(2, 2));

        //        Vector3 cubePos = new Vector3(xIn * 0.16f + 0.08f,
        //                    yIn * 0.16f + 0.08f, 0f);
        //        Gizmos.DrawCube(cubePos, new Vector3(0.16f, 0.16f, 0.16f));
        //    }

        //}
        //api.Http.GetRoom(coord.roomName, coord.shardName, serverCallback);
        //public void UpdateRoomObjectsFromConnection()
        //{
        //    //Action<JSONObject> serverCallback = obj => {
        //    //    var terrainData = obj["terrain"].list[0]["terrain"].str;
        //    //    this.terrain[coord.key] = terrainData;
        //    //    callback(terrainData);
        //    //};

        //    Debug.Log("recTer : " + recTer);
        //    ter = LoadTerrainFromConnection(recTer);
        //    Debug.Log(ter.result[0].type);
        //    terrainTypes = SetTerrain(ter);
        //    int terrainLength = terrainTypes.GetLength(0) * terrainTypes.GetLength(1);

        //    Tilemap tilemap = tileMapGO.GetComponent<Tilemap>();
        //    tRenderer = tilemap.gameObject.GetComponent<TilemapRenderer>();
        //    TileBase[,] tileArray = new TileBase[terrainTypes.GetLength(0), terrainTypes.GetLength(1)];
        //    for (int x = 0; x < 50; x++)
        //    {
        //        for (int y = 0; y < 50; y++)
        //        {
        //            Vector3Int currentPos = new Vector3Int(x, y, 0);
        //            tilemap.SetTile(currentPos, storageTile);
        //            tilemap.SetTileFlags(currentPos, TileFlags.None);
        //            //Debug.Log(terrainTypes[x, y] + " " + x + " "+ y);
        //            if (terrainTypes[x, y] == "wall")
        //            {
        //                //tilemap.SetColor(currentPos, wallColor);
        //            }
        //            else if (terrainTypes[x, y] == "swamp")
        //            {
        //                //tilemap.SetColor(currentPos, swampColor);
        //            }
        //            else
        //            {
        //                //tilemap.SetColor(currentPos, plainsColor);
        //            }
        //        }
        //    }
        //}
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
        RoomTerrain LoadTerrainFromConnection(ConnectRecievedRoomObjects t)
        {
            //string terrainFile = "{\"result\": null}";
            RoomTerrain terr = new RoomTerrain();
            Terrain[] result = new Terrain[2500];
            //terr.result = result;
            //ConnectRecievedTerrain t = JsonUtility.FromJson<ConnectRecievedTerrain>(terrainIn.ToString());
            Debug.Log("load terrain : " + t);
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
        public class ConnectRecievedRoomObjects
        {
            public string ok;
            public RoomParsedObjects[] terrain;
        }
        [System.Serializable]
        public class RoomParsedObjects
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
            public RoomObject[] objects;
            //public User[] users;
        }
        public interface IRoomObject
        {
            int[] Pos();
            string Id();
            string Type();
        }
        [System.Serializable]
        public class ActionLog
        {
            public JSONObject actionsObj;
            public Vector2Int runReaction;
            public Vector2Int transferEnergy;
            public Vector2Int attack;
            public Vector2Int heal;
            public Vector2Int repair;
            public ActionLog(JSONObject objIn)
            {
                foreach (var key in objIn.keys)
                {
                    if (key == "runReaction")
                    {
                        int x = 0;
                        int y = 0;
                        objIn[key].GetField(ref x, "x");
                        objIn[key].GetField(ref y, "y");
                        runReaction = new Vector2Int(x, y);
                    }
                    if (key == "transferEnergy")
                    {
                        int x = 0;
                        int y = 0;
                        objIn[key].GetField(ref x, "x");
                        objIn[key].GetField(ref y, "y");
                        transferEnergy = new Vector2Int(x, y);
                    }
                    if (key == "attack")
                    {
                        int x = 0;
                        int y = 0;
                        objIn[key].GetField(ref x, "x");
                        objIn[key].GetField(ref y, "y");
                        attack = new Vector2Int(x, y);
                    }
                    if (key == "heal")
                    {
                        int x = 0;
                        int y = 0;
                        objIn[key].GetField(ref x, "x");
                        objIn[key].GetField(ref y, "y");
                        heal = new Vector2Int(x, y);
                    }
                    if (key == "repair")
                    {
                        int x = 0;
                        int y = 0;
                        objIn[key].GetField(ref x, "x");
                        objIn[key].GetField(ref y, "y");
                        repair = new Vector2Int(x, y);
                    }
                }
            }
        }
        [System.Serializable]
        public class RoomObject : IRoomObject, IHitsObject, IStore
        {
            public string _id;
            public string type;
            public int x;
            public int y;
            public string _user;
            public JSONObject _obj;
            //hits object
            public int hits;
            public int hitsMax;
            //store object
            public int storeTotal;
            public int storeCapacity;
            public Dictionary<string, int> _store = new Dictionary<string, int>();
            //lab object
            public int mineralAmount;
            public int mineralCapacity;
            //decaying object
            public int nextDecayTime;
            //disabled objecct
            public bool off;
            public bool _isDisabled;
            //keeper lair
            public int nextSpawnTime;
            //cooldown object
            public int cooldown;
            //regeneration object
            public int ticksToRegeneration;
            public int nextRegenerationTime;
            public int invaderHarvested;
            //progress object
            public int progress;
            public int progressTotal;
            //controller
            public int downgradeTime;
            public string reservationUser;
            public int reservationTime;
            public int safeModeAvailable;
            public int level;
            public Sign sign;

            public RoomObject(JSONObject obj)
            {
                if (obj["x"] != null)
                {
                    x = Int32.Parse(obj["x"].ToString());
                }
                if (obj["y"] != null)
                {
                    y = Int32.Parse(obj["y"].ToString());
                }
                if (obj["_id"] != null)
                {
                    _id = obj["_id"].ToString().ToLowerInvariant().Replace("\"", "");
                }
                storeCapacity = -1;
                _obj = obj;
                // store object initialization or let it always be null
                if (obj.HasField("energy"))
                {
                    SetStore(obj);
                }
                // hits object initialization or set to -1
                if (obj["hits"] != null)
                {
                    try
                    {
                        obj.GetField(ref hits, "hits");
                        //hits = Int32.Parse(obj["hits"].ToString());
                    }
                    catch
                    {
                        Debug.Log(type + " has an error with hits : " + obj["hits"]);
                    }

                }
                else
                {
                    hits = -1;
                }
                UpdateRoomObject(obj);
            }
                // renewables
                //  ticksToRegeneration invaderHarvested nextRegenerationTime 
                // cooldowns
                // complete
                // decay
                //  nextDecayTime
                // downgradeTime reservation safeModeAvailable reservation sign level
            public void UpdateRoomObject(JSONObject obj)
            {
                if (this._store != null)
                {
                    SetStore(obj);
                }
                // loop over everything and change it
                if (obj.HasField("type"))
                    obj.GetField(ref type, "type");
                if (obj.HasField("user"))
                    obj.GetField(ref _user, "user");
                if (obj.HasField("hits"))
                    obj.GetField(ref hits, "hits");
                if (obj.HasField("hitsMax"))
                    obj.GetField(ref hitsMax, "hitsMax");
                if (obj.HasField("energyCapacity"))
                    obj.GetField(ref storeCapacity, "energyCapacity");
                if (obj.HasField("mineralAmount"))
                    obj.GetField(ref mineralAmount, "mineralAmount");
                if (obj.HasField("mineralCapacity"))
                    obj.GetField(ref mineralCapacity, "mineralCapacity");
                if (obj.HasField("off"))
                    obj.GetField(ref off, "off");
                if (obj.HasField("_isDisabled"))
                    obj.GetField(ref _isDisabled, "_isDisabled");
                if (obj.HasField("nextSpawnTime"))
                    obj.GetField(ref nextSpawnTime, "nextSpawnTime");
                if (obj.HasField("cooldown"))
                    obj.GetField(ref cooldown, "cooldown");
                if (obj.HasField("ticksToRegeneration"))
                    obj.GetField(ref ticksToRegeneration, "ticksToRegeneration");
                if (obj.HasField("invaderHarvested"))
                    obj.GetField(ref invaderHarvested, "invaderHarvested");
                if (obj.HasField("nextRegenerationTime"))
                    obj.GetField(ref nextRegenerationTime, "nextRegenerationTime");
                if (obj.HasField("progress"))
                    obj.GetField(ref progress, "progress");
                if (obj.HasField("progressTotal"))
                    obj.GetField(ref progressTotal, "progressTotal");
                // downgradeTime reservation safeModeAvailable sign level
                if (obj.HasField("downgradeTime"))
                    obj.GetField(ref downgradeTime, "downgradeTime");
                if (obj.HasField("reservation"))
                {
                    if (obj["reservation"].type != JSONObject.Type.NULL)
                    {
                        obj["reservation"].GetField(ref reservationUser, "user");
                        obj["reservation"].GetField(ref reservationTime, "endTime");
                    } else
                    {
                        reservationUser = null;
                        reservationTime = 0;
                    }
                }
                if (obj.HasField("safeModeAvailable"))
                {
                    if (obj["safeModeAvailable"].type != JSONObject.Type.NULL)
                    {
                        obj.GetField(ref safeModeAvailable, "safeModeAvailable");
                    }
                    else
                    {
                        safeModeAvailable = 0;
                    }
                }
                if (obj.HasField("level"))
                    obj.GetField(ref level, "level");
                if (obj.HasField("sign"))
                {
                    sign = new Sign(obj["sign"]);
                }
            }
            public string Id()
            {
                return _id;
            }
            public int[] Pos()
            {
                int[] _pos = { x, y};
                return _pos;
            }
            public string Type()
            {
                return type;
            }
            public RoomObject (string flagStringIn)
            {
                // this is faked for pseudo-override for Flag
            }
            // for Ihits
            public int Hits()
            {
                return hits;
            }
            public int MaxHits()
            {
                return hitsMax;
            }
            // for IStore
            public Dictionary<string, int> Store()
            {
                return _store;
            }
            private void SetStore(JSONObject obj)
            {
                bool changed = false;
                if (_store.Count <= 0)
                {
                    _store = new Dictionary<string, int>();
                }
                if (storeCapacity == -1 && obj["energyCapacity"])
                {
                    storeCapacity = Int32.Parse(obj["energyCapacity"].ToString());
                }
                foreach (string r in Constants.RESOURCES_ALL)
                {
                    //if (r == "energy")
                    //{
                    //    continue;
                    //}
                    if (obj[r] != null)
                    {
                        if (!changed) changed = true;
                        string min = obj[r].ToString();
                        int value = 0;
                        obj.GetField(ref value,r);
                        if (value > 0 && !_store.ContainsKey(r))
                        {
                            _store.Add(r, value);
                        }
                        if (_store.ContainsKey(r))
                        {
                            if (_store[r] != value)
                            {
                                _store[r] = value;
                            }
                        }

                    }
                }
                if (obj.HasField("energy"))
                {
                    _store["energy"] = Int32.Parse(obj["energy"].ToString());
                } else
                {
                    if (_store.ContainsKey("energy"))
                        _store["energy"] = _store["energy"] > 0 ? _store["energy"] : 0;
                }
                if (changed)
                {
                    storeTotal = 0;
                    foreach (KeyValuePair<string,int> kvp in _store)
                    {
                        storeTotal += kvp.Value;
                    }
                }
            }
        }
        public class Sign
        {
            //"user": "5866db8ac04c074e4f1adf24",
            //"text": "Owned by Y Pact.",
            //"time": 24115,
            //"datetime": 1501825059180
            public string user;
            public string text;
            public int time;
            public int datetime;

            public Sign (JSONObject objIn)
            {
                if (objIn.HasField("user"))
                    objIn.GetField(ref user, "user");
                if (objIn.HasField("text"))
                    objIn.GetField(ref text, "text");
                if (objIn.HasField("time"))
                    objIn.GetField(ref time, "time");
                if (objIn.HasField("datetime"))
                    objIn.GetField(ref datetime, "datetime");
            }
        }
        public interface IHitsObject
        {
            int Hits();
            int MaxHits();
            // notifyWhenAttacked
            // isActive
            // destroy / suicide
        }
        [System.Serializable]
        public class Flag : RoomObject
        {
            //"flags":"Source10Spawn1~6~6~40~31"
            public string name;
            //public int x;
            //public int y;
            public Color primary;
            public Color secondary;

            public Flag (string flagIn, string usrIn) : base (flagIn)
            {
                if (flagIn == null)
                    return;
                string[] pieces = flagIn.Split('~');
                type = "flag";
                name = pieces[0];
                _id = name;
                _user = usrIn;
                if (pieces.Length < 2)
                    return;
                primary = FlagColorsInt[Int32.Parse(pieces[1])];
                secondary = FlagColorsInt[Int32.Parse(pieces[2])];
                x = Int32.Parse(pieces[3]);
                y = Int32.Parse(pieces[4]);
            }
        }
        public interface IStore {
            Dictionary<string, int> Store();
        }
        [System.Serializable]
        public class Store { 
            public int storeCapacity;
            public Dictionary<string, int> _store;

            public Store(JSONObject obj)
            {
                int mineralIn = 0;
                if (storeCapacity == 0 && obj["energyCapacity"])
                {
                    storeCapacity = Int32.Parse(obj["energyCapacity"].ToString());
                }
                foreach (string r in Constants.RESOURCES_ALL)
                {
                    //if (r == "energy")
                    //{
                    //    continue;
                    //}
                    if (obj[r])
                    {
                        string min = obj[r].ToString();
                        int value = Int32.Parse(min) > 0 ? Int32.Parse(min) : 0;
                        if (value > 0 && !_store.ContainsKey(r))
                        {
                            _store.Add(r, value);
                        }
                        if (_store.ContainsKey(r))
                        {
                            if (_store[r] != value)
                            {
                                _store[r] = value;
                            }
                            mineralIn += value;
                        }

                    }
                }
                if (obj["energy"])
                {
                    _store["energy"] = Int32.Parse(obj["energy"].ToString());
                }
            }
        }
        //public enum FlagColors
        //{
        //    COLOR_RED,
        //    COLOR_PURPLE,
        //    COLOR_BLUE,
        //    COLOR_CYAN,
        //    COLOR_GREEN,
        //    COLOR_YELLOW,
        //    COLOR_ORANGE,
        //    COLOR_BROWN,
        //    COLOR_GREY,
        //    COLOR_WHITE,
        //}
        public static IDictionary<string, Color> FlagColorsString = new Dictionary<string, Color>()
        {
            { "COLOR_RED", Color.red },
            { "COLOR_PURPLE", new Color(0.6f,0.0f,0.55f)},
            { "COLOR_BLUE", Color.blue},
            { "COLOR_CYAN", Color.cyan},
            { "COLOR_GREEN", Color.green},
            { "COLOR_YELLOW", Color.yellow},
            { "COLOR_ORANGE", new Color(1.0f,.66f,0.1f)},
            { "COLOR_BROWN", new Color(0.6f,0.3f,0.0f)},
            { "COLOR_GREY", Color.gray},
            { "COLOR_WHITE", Color.white}
        };
        public static IDictionary<int, Color> FlagColorsInt = new Dictionary<int, Color>()
        {
            { 1, Color.red },
            { 2, new Color(0.6f,0.0f,0.55f)},
            { 3, Color.blue},
            { 4, Color.cyan},
            { 5, Color.green},
            { 6, Color.yellow},
            { 7, new Color(1.0f,.66f,0.1f)},
            { 8, new Color(0.6f,0.3f,0.0f)},
            { 9, Color.gray},
            { 0, Color.white}
        };
        //[System.Serializable]
        //public class Badge
        //{
        //    public int type; // 2,
        //    public string color1; // #000000,
        //    public string color2; // #028300,
        //    public string color3; // #8b5c00,
        //    public int param; // 0,
        //    public bool flip; // false
        //    public Badge(JSONObject objIn)
        //    {
        //        if (objIn == null) { return; }
        //        if (objIn["type"] != null)
        //        {
        //            type = Int32.Parse(objIn["type"].ToString());
        //        }
        //        if (objIn["color1"] != null)
        //        {
        //            color1 = !!objIn["color1"] ? objIn["color1"].ToString().ToLowerInvariant().Replace("\"", "") : "#773333";
        //        }
        //        if (objIn["color2"] != null)
        //        {
        //            color2 = !!objIn["color2"] ? objIn["color2"].ToString().ToLowerInvariant().Replace("\"", "") : "#773333";
        //        }
        //        if (objIn["color3"] != null)
        //        {
        //            color3 = !!objIn["color3"] ? objIn["color3"].ToString().ToLowerInvariant().Replace("\"", "") : "#773333";
        //        }
        //        if (objIn["param"] != null)
        //        {
        //            param = Int32.Parse(objIn["param"].ToString());
        //        }
        //        if (objIn["flip"] != null)
        //        {
        //            flip = Boolean.TryParse(objIn["flip"].ToString(), out flip);
        //        }
        //    }
        //    public Badge(int typeIn, string color1In, string color2In, string color3In, int paramIn, bool flipIn)
        //    {
        //        type = typeIn;
        //        color1 = color1In;
        //        color2 = color2In;
        //        color3 = color3In;
        //        param = paramIn;
        //        flip = flipIn;
        //    }
        //}
    }
}