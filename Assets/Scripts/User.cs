using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ScreepsViewer;

namespace ScreepsViewer
{
    [System.Serializable]
    public class User
    {
        public string id;
        public string username;
        public Badge badge;
        public Sprite badgePNG;
        public User(string idIn, string usernameIn, JSONObject badgeIn)
        {
            id = idIn;
            username = usernameIn;
            if (badgeIn != null)
            {
                badge = new Badge(badgeIn);
                BadgeStorage bs = GameObject.Find("BadgeGenerator").GetComponent<BadgeStorage>();
                Action<Sprite> getBadge = new Action<Sprite>(GetBadgePNG);
                bs.GetBadge(username, badge, getBadge);
            }
        }
        public User(string idIn)
        {
            id = idIn;
        }
        private void GetBadgePNG(Sprite spriteIn)
        {
            Debug.Log("recieved badge from action");
            badgePNG = spriteIn;

        }
    }
}
