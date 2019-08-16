using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using ScreepsViewer;

public class ConnectionController : MonoBehaviour
{

    public static string roomName;
    public static string shardName = "shard1";
    private string baseUrl;
    private string roomUrl;
    //private string shardRoomName;
    public object pageResult;
    public string currentRoom;
    private bool isError;
    public TileMapHolder TileMapObject;

    public void Start()
    {
        roomName = "E0S0";
    }
    public void UpdateRoomName(string roomIn)
    {
        roomName = roomIn;
    }
    public void ChangeRoom()
    {
        if (TileMapObject == null)
        {
            GameObject tMap = GameObject.Find("TileMapHolder");
            TileMapObject = tMap.GetComponent<TileMapHolder>();
        }
        //roomName = ControlsPanel.roomName;
        GetRequest();
    }
    public void GetRequest()
    {
        //Debug.Log("starting getRequest...");
        baseUrl = "https://screeps.com/api/game/room-terrain?room=ROOM_NAME&encoded=1&shard=SHARD_NAME";
        // A correct website page.
        roomUrl = baseUrl.Replace("ROOM_NAME", roomName);
        roomUrl = roomUrl.Replace("SHARD_NAME", shardName);
        StartCoroutine(GetRequestWrapper(roomUrl));
        //pageResult = StartCoroutine(GetRequestCo(roomUrl));
        //Debug.Log(pageResult);
    }
    IEnumerator GetRequestWrapper(string uri, params object[] list)
    {
        //Debug.Log("starting getRequest wrapper...");
        CoroutineWithData cd = new CoroutineWithData(this, GetRequestCo(uri));
        yield return cd.Routine;
        yield return cd.result;
        pageResult = cd.result;
        currentRoom = roomName;
        if (!isError) {
            Debug.Log("sending to tilemap...");
            TileMapObject.UpdateTerrainFromConnection(pageResult);
        }
    }
    IEnumerator GetRequestCo(string uri, params object[] list)
    {
        //Debug.Log("starting getRequest coroutine...");
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            //webRequest.SetRequestHeader("/api/game/room-terrain",list);
            //webRequest.SetRequestHeader("shard", shardName);
            //webRequest.SetRequestHeader("room", roomName);
            //webRequest.SetRequestHeader("encoded", "1");
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
                isError = true;
                yield return webRequest.error;
            }
            else
            {
                Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                isError = false;
                yield return webRequest.downloadHandler.text;
            }
        }
    }
}