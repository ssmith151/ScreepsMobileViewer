using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

namespace ScreepsViewer {
    public class ScreepsSocket : MonoBehaviour {
        private WebSocket Socket { get; set; }
        private ScreepsAPI api;
        private Dictionary<string, Action<JSONObject>> subscriptions = new Dictionary<string, Action<JSONObject>>();
        
        public Action<EventArgs> OnOpen;
        public Action<CloseEventArgs> OnClose;
        public Action<MessageEventArgs> OnMessage;
        public Action<ErrorEventArgs> OnError;
        public Action<string, bool> OnConsoleMessage;
        public Action<string> PrintConnectionStatus;

        public void Init(ScreepsAPI screepsApi) {
            api = screepsApi;
        }
	
        private void OnDestroy() {
            UnsubAll();
            if (Socket != null) Socket.Close();
        }

        public void Connect() {
            if (Socket != null) {
                Socket.Close();
            }
            
            var protocol = api.Address.ssl ? "wss" : "ws";
            var port = api.Address.ssl ? "443" : api.Address.port;
            Socket = new WebSocket(string.Format("{0}://{1}:{2}/socket/websocket", protocol, api.Address.hostName, port));
            Socket.OnOpen += Open;
            Socket.OnError += Error;
            Socket.OnMessage += Message;
            Socket.OnClose += Close;
            Socket.Connect();
        }

        private void Open(object sender, EventArgs e) {
            try {
                Debug.Log("Socket Open");
                //PrintConnectionStatus.Invoke("Socket Open");
                Socket.Send(string.Format("auth {0}", api.Http.Token));
                if (OnOpen != null) OnOpen.Invoke(e);
            } catch (Exception exception) {
                Debug.Log(string.Format("Exception in ScreepSocket.OnOpen\n{0}", exception));
                PrintConnectionStatus.Invoke("Exception in ScreepSocket.OnOpen : "+ exception.ToString());
            }
        }

        private void Close(object sender, CloseEventArgs e) {
            try {
                Debug.Log(string.Format("Socket Closing: {0}", e.Reason));
                //PrintConnectionStatus.Invoke("Socket Closing: "+e.Reason.ToString());
                if (OnClose != null) OnClose.Invoke(e);
            } catch (Exception exception) {
                Debug.Log(string.Format("Exception in ScreepSocket.OnClose\n{0}", exception));
                PrintConnectionStatus.Invoke("Exception in ScreepSocket.OnClose: " + exception.ToString());
            }
            
        }

        private void Message(object sender, MessageEventArgs e) {
            try {
                Debug.Log(string.Format("Socket Message: {0}", e.Data));
                //PrintConnectionStatus.Invoke("Socket Message: " + e.Data.ToString());
                var parse = e.Data.Split(' ');
                if (parse.Length == 3 && parse[0] == "auth" && parse[1] == "ok") {
                    Debug.Log("socket auth success");
                    //PrintConnectionStatus.Invoke("socket auth success");
                }
                var json = new JSONObject(e.Data);
                FindSubscription(json);
                if (OnMessage != null) OnMessage.Invoke(e);
            } catch (Exception exception) {
                Debug.Log("Exception in ScreepSocket.OnMessage :"+ exception);
                PrintConnectionStatus.Invoke("Exception in ScreepSocket.OnMessage :" + exception.ToString());
            }
        }

        private void Error(object sender, ErrorEventArgs e) {
            try {
                Debug.Log(string.Format("Socket Error: {0}", e.Message));
                //PrintConnectionStatus.Invoke("Socket Error: " + e.Message.ToString());
                if (OnError != null) OnError.Invoke(e);
            } catch (Exception exception) {
                Debug.Log(string.Format("Exception in ScreepSocket.OnError\n{0}", exception));
                PrintConnectionStatus.Invoke("Exception in ScreepSocket.OnError :" + exception);
            }
        }

        private void FindSubscription(JSONObject json) {
            var list = json.list;
            if (list == null || list.Count < 2 || !list[0].IsString || !subscriptions.ContainsKey(list[0].str))
            {
                Debug.Log(string.Format("Error in ScreepSocket.FindSubscription"));
                //PrintConnectionStatus.Invoke("Error in ScreepSocket.FindSubscription");
                return;
            }
            list[1].AddField("subscription", list[0].str);
            subscriptions[list[0].str].Invoke(list[1]);
        }
	
        public void Subscribe(string path, Action<JSONObject> callback) {
            string subPath = path.Substring(0, 5);
            if (subPath == "room:")
                UnsubAll();
            Socket.Send(string.Format("subscribe {0}", path));
            subscriptions[path] = callback;
        }
        
        public void Unsub(string path) {
            Socket.Send(string.Format("unsubscribe {0}", path));
        }

        private void UnsubAll() {
            if (Socket == null) return;
            foreach (var kvp in subscriptions) {
                Unsub(kvp.Key);
            }
        }
    }
}
