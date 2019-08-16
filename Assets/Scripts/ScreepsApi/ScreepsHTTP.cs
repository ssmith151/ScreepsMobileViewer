using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

namespace ScreepsViewer {
    public class ScreepsHTTP : MonoBehaviour {

        public string Token { get; private set; }
        public Action<string> PrintConnectionStatus;

        private ScreepsAPI api;
        private string path;

        public void Init(ScreepsAPI api) {
            this.api = api;
        }

        private void Request(string requestMethod, string path, RequestBody body = null,
            Action<JSONObject> onSuccess = null, Action onError = null) {

            Debug.Log(string.Format("HTTP: attempting {0} to {1}", requestMethod, path));
            PrintConnectionStatus.Invoke("HTTP: attempting : " + requestMethod.ToString() + "@" + path.ToString());
            UnityWebRequest www;
            var fullPath = api.Address.Http(path);
            if (requestMethod == UnityWebRequest.kHttpVerbGET) {
                if (body != null) {
                    fullPath = fullPath + body.ToQueryString();
                }
                www = UnityWebRequest.Get(fullPath);
            } else if (requestMethod == UnityWebRequest.kHttpVerbPOST) {
                www = new UnityWebRequest(fullPath, "POST");
                byte[] bodyRaw = Encoding.UTF8.GetBytes(body.ToString());
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                www.downloadHandler = new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");
            } else {
                Debug.Log(string.Format("HTTP: request method {0} unrecognized", requestMethod));
                PrintConnectionStatus.Invoke("HTTP: request method unrecognized : " + requestMethod.ToString());
                return;
            }

            Action<UnityWebRequest> onComplete = (UnityWebRequest outcome) => {
                if (outcome.isNetworkError || outcome.isHttpError) {
                    string err = outcome.error != null ? outcome.error.ToString() : "null";
                    string response = outcome.responseCode > 0 ? outcome.responseCode.ToString() : "null";
                    Debug.Log(string.Format("HTTP: error ({1}), reason: {0}", err, outcome.responseCode));
                    PrintConnectionStatus.Invoke("HTTP: error : " + err + " reason " + response);
                    if (onError != null) {
                        onError();
                    } else {
                        Auth((reply) => {
                            Request(requestMethod, path, body, onSuccess);
                        }, () => {
                            if (api.OnConnectionStatusChange != null) api.OnConnectionStatusChange.Invoke(false);
                        });
                    }
                    if (response == "502")
                    {
                        BadGateway();
                    }
                } else {
                    Debug.Log(string.Format("HTTP: success, data: \n{0}", outcome.downloadHandler.text));
                    //PrintConnectionStatus.Invoke("HTTP: success, data: " + outcome.downloadHandler.text.ToString());
                    PrintConnectionStatus.Invoke("HTTP: success, data: data currently masked in ScreepsHTTP.cs");
                    var reply = new JSONObject(outcome.downloadHandler.text);
                    var token = reply["token"];
                    if (token != null) {
                        Token = token.str;
                        Debug.Log(string.Format("HTTP: found a token! {0}", Token));
                        //PrintConnectionStatus.Invoke("HTTP: found token : " + Token.ToString());
                        PrintConnectionStatus.Invoke("HTTP: found token : ********************************");
                    }
                    var status = reply["ok"];
                    if (status != null && status.n == 1) {
                        if (onSuccess != null) onSuccess.Invoke(reply);
                    }
                }
            };

            StartCoroutine(SendRequest(www, onComplete));
        }
        private void BadGateway()
        {
            PrintConnectionStatus.Invoke("HTTP: Screeps server may be down...");
            if (api.Address.hostName.ToLowerInvariant() == "screeps.com")
                BadGateRequest();
        }
        private void BadGateRequest(Action<JSONObject> onSuccess = null, Action onError = null)
        {
            //Debug.Log(string.Format("HTTP: attempting {0} to {1}", requestMethod, path));
            //PrintConnectionStatus.Invoke("HTTP: attempting : " + requestMethod.ToString() + "@" + path.ToString());
            UnityWebRequest www;
            var fullPath = "https://status.screeps.com/";
            www = UnityWebRequest.Get(fullPath);
            Action<UnityWebRequest> bgOnComplete = (UnityWebRequest outcome) => {
                if (outcome.isNetworkError || outcome.isHttpError)
                {
                    string err = outcome.error != null ? outcome.error.ToString() : "null";
                    string response = outcome.responseCode > 0 ? outcome.responseCode.ToString() : "null";
                    Debug.Log(string.Format("HTTP: error ({1}), reason: {0}", err, outcome.responseCode));
                    PrintConnectionStatus.Invoke("HTTP: error : " + err + " reason " + response);
                    if (onError != null)
                    {
                        onError();
                    }
                }
                else
                {
                    string downloaded = outcome.downloadHandler.text.ToString();
                    string htmlString = downloaded.Substring(downloaded.IndexOf("status-value"));
                    htmlString = htmlString.Substring(htmlString.IndexOf(">"));
                    htmlString = htmlString.Replace(">", "");
                    htmlString = htmlString.Substring(0,htmlString.IndexOf("</div"));
                    PrintConnectionStatus.Invoke("HTTP: Found server site successfully");
                    PrintConnectionStatus.Invoke("Server Status : "+htmlString);
                    var reply = new JSONObject(downloaded);
                    var token = reply["token"];
                    if (token != null)
                    {
                        Token = token.str;
                        Debug.Log(string.Format("HTTP: found a token! {0}", Token));
                        //PrintConnectionStatus.Invoke("HTTP: found token : " + Token.ToString());
                        PrintConnectionStatus.Invoke("HTTP: found token : ************************");
                    }
                    var status = reply["ok"];
                    if (status != null && status.n == 1)
                    {
                        if (onSuccess != null) onSuccess.Invoke(reply);
                    }
                }
            };
            StartCoroutine(SendRequest(www, bgOnComplete));
        }
        private IEnumerator SendRequest(UnityWebRequest www, Action<UnityWebRequest> onComplete) {
            if (Token != null) {
                www.SetRequestHeader("X-Token", Token);
                www.SetRequestHeader("X-Username", Token);
            }
            yield return www.SendWebRequest();
            onComplete(www);
        }

        public void Auth(Action<JSONObject> onSuccess, Action onError = null) {
            var body = new RequestBody();
            body.AddField("email", api.Credentials.email);
            body.AddField("password", api.Credentials.password);

            Request("POST", "/api/auth/signin", body, onSuccess, onError);
        }

        public void GetUser(Action<JSONObject> onSuccess) {
            Request("GET", "/api/auth/me", null, onSuccess);
        }

        public void ConsoleInput(string message, string shard) {
            var body = new RequestBody();
            body.AddField("expression", message);
            body.AddField("shard", shard);
            Request("POST", "/api/user/console", body);
        }

        public void GetRoomTerrain(string roomName, string shard, Action<JSONObject> callback) {
            var body = new RequestBody();
            body.AddField("room", roomName);
            body.AddField("encoded", "0");
            body.AddField("shard", shard);

            Request("GET", "/api/game/room-terrain", body, callback);
        }

        public void GetBadge(string userName, Action<JSONObject> callback){
            var body = new RequestBody();
            body.AddField("username", userName);
            Request("POST", "/api/user/badge", body, callback);
        }

        public void GetTime(string shard, Action<JSONObject> callback)
        {
            var body = new RequestBody();
            body.AddField("shard", shard);
            Request("GET", "/api/game/time", body, callback);
        }

        public void GetRoomObjects(string roomName, string shard, Action<JSONObject> callback)
        {
            path = string.Format("room:{0}/{1}", shard, roomName);
            api.Socket.Subscribe(path, callback);
            //GetSurroundingRooms(roomName, shard, callback);
        }
        public void GetSurroundingRooms(string roomName, string shard, Action<JSONObject> callback)
        {
            string[] surroundingRooms = GetSurroundingRoomNames(roomName, shard);
            foreach (string newRoomName in surroundingRooms)
            {
                //Debug.Log(newRoomName);
                if (api.Address.hostName.ToLowerInvariant() == "screeps.com")
                    path = string.Format("roomMap2:{0}/{1}", shard, newRoomName);
                else
                    path = string.Format("roomMap2:{0}", newRoomName);
                api.Socket.Subscribe(path, callback);
            }
        }
        public string[] GetSurroundingRoomNames(string roomName, string shard = null)
        {
            // parse room in
            int nsPos = roomName.IndexOf('N');
            nsPos = nsPos > 0 ? nsPos : roomName.IndexOf('S');
            string[] splitRoomName = roomName.Split(roomName[nsPos]);
            splitRoomName[1] = roomName[nsPos] + splitRoomName[1];
            string ew = splitRoomName[0].Substring(0, 1);
            int ewNum = Int32.Parse(splitRoomName[0].Substring(1));
            string ns = splitRoomName[1].Substring(0, 1);
            int nsNum = Int32.Parse(splitRoomName[1].Substring(1));
            int nsMod = ns == "N" ? 1 : -1;
            int ewMod = ns == "E" ? 1 : -1;
            // store -1 and +1
            int ewMinNum = ewNum == 0 && ew == "E" ? 0 : ewNum - ewMod;
            int ewMaxNum = ewNum == 0 && ew == "W" ? 0 : ewNum + ewMod;
            int nsMinNum = nsNum == 0 && ns == "N" ? 0 : nsNum - nsMod;
            int nsMaxNum = nsNum == 0 && ns == "S" ? 0 : nsNum + nsMod;
            // store ew ns mins maxs
            string ewMinChr = ewNum == 0 && ew == "E" ? "W" : ew;
            string ewMaxChr = ewNum == 0 && ew == "W" ? "E" : ew;
            string nsMinChr = nsNum == 0 && ns == "N" ? "S" : ns;
            string nsMaxChr = nsNum == 0 && ns == "S" ? "N" : ns;
            // give new room names
            string[] roomsOut = new string[] {
                ewMaxChr + ewMaxNum.ToString() + nsMinChr + nsMinNum.ToString(),    //1,3
                splitRoomName[0] + nsMinChr + nsMinNum.ToString(),                  //1,2
                ewMinChr + ewMinNum.ToString() + nsMinChr + nsMinNum.ToString(),    //1,1
                ewMaxChr + ewMaxNum.ToString() + splitRoomName[1],                  //2,3
                roomName,                                                           //2,2
                ewMinChr + ewMinNum.ToString() + splitRoomName[1],                  //2,1
                ewMaxChr + ewMaxNum.ToString() + nsMaxChr + nsMaxNum.ToString(),    //3,3
                splitRoomName[0] + nsMaxChr + nsMaxNum.ToString(),                  //3,2
                ewMinChr + ewMinNum.ToString() + nsMaxChr + nsMaxNum.ToString()     //3,1
            };
            return roomsOut;
        }
    }
}
