using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ScreepsViewer
{
    [System.Serializable]
    public class Badge
    {
        public int type; // 2,
        public string color1; // #000000,
        public string color2; // #028300,
        public string color3; // #8b5c00,
        public int param; // 0,
        public bool flip; // false
        public Badge(JSONObject objIn)
        {
            if (objIn == null) { return; }
            if (objIn["type"] != null)
            {
                type = Int32.Parse(objIn["type"].ToString());
            }
            if (objIn["color1"] != null)
            {
                color1 = !!objIn["color1"] ? objIn["color1"].ToString().ToLowerInvariant().Replace("\"", "") : "#773333";
            }
            if (objIn["color2"] != null)
            {
                color2 = !!objIn["color2"] ? objIn["color2"].ToString().ToLowerInvariant().Replace("\"", "") : "#773333";
            }
            if (objIn["color3"] != null)
            {
                color3 = !!objIn["color3"] ? objIn["color3"].ToString().ToLowerInvariant().Replace("\"", "") : "#773333";
            }
            if (objIn["param"] != null)
            {
                param = Int32.Parse(objIn["param"].ToString());
            }
            if (objIn["flip"] != null)
            {
                flip = Boolean.TryParse(objIn["flip"].ToString(), out flip);
            }
        }
        public Badge(int typeIn, string color1In, string color2In, string color3In, int paramIn, bool flipIn)
        {
            type = typeIn;
            color1 = color1In;
            color2 = color2In;
            color3 = color3In;
            param = paramIn;
            flip = flipIn;
        }
    }
}
