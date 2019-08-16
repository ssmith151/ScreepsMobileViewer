using System;
using System.Collections;
using System.Collections.Generic;
using SocketIO;
using UnityEngine;
using WebSocketSharp;

namespace ScreepsViewer {
    [RequireComponent(typeof(ScreepsHTTP))]
	[RequireComponent(typeof(ScreepsSocket))]
	public class ScreepsAPI : MonoBehaviour {
	
		public Address Address { get; private set; }
		public Credentials Credentials { get; private set; }
		public ScreepsHTTP Http { get; private set; }
		public ScreepsSocket Socket { get; private set; }
		public User User { get; private set; }
	    public Action<bool> OnConnectionStatusChange;
        public Action<string> PrintConnectionStatus;

        private string token;
	
		public void Awake() {
			Http = GetComponent<ScreepsHTTP>();
			Http.Init(this);
			Socket = GetComponent<ScreepsSocket>();
			Socket.Init(this);
		}

        // Use this for initialization
        // TODO: this should be an IEnumerator with a coroutine that waits for connection
        public void Connect (Credentials credentials, Address address) {
			Credentials = credentials;
			Address = address;
			// configure HTTP
			Http.Auth(o => {
                PrintConnectionStatus.Invoke("login successful");
                Debug.Log("login successful");
				Socket.Connect();
				Http.GetUser(AssignUser);
			}, () => {
                PrintConnectionStatus.Invoke("login Failed");
                Debug.Log("login failed");
			});
		}

	    private void AssignUser(JSONObject obj) {
		    User = new User(obj["_id"].str);
            PrintConnectionStatus.Invoke("logged in as : " + obj["_id"].str);
		    if (OnConnectionStatusChange != null) OnConnectionStatusChange.Invoke(true);
	    }
	}
}
