using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ScreepsViewer;
using UnityEngine.UI;

namespace ScreepsViewer
{
    public class PartMaker : MonoBehaviour
    {
        public Color attackColor = Color.red;
        public Color rangedAttackColor = Color.blue;
        public Color healColor = Color.green;
        public Color claimColor = new Color(0.9f,0.2f,0.7f);
        public Color workColor = Color.yellow;
        public Color toughColor = Color.white;
        public Color carryColor = new Color(0.4f,0.4f,0.4f);
        public Color moveColor = new Color(0.6f,0.6f,0.6f);
        public GameObject bodyPart;
        private int moveParts;

        private Dictionary<string, int> partNumbers = new Dictionary<string, int>();
        private Dictionary<string, GameObject> partGOs = new Dictionary<string, GameObject>();

        private GameObject ringOut;

        private CreepBody body;

        private void CountParts()
        {
            foreach (string p in body.parts)
            {
                if (!partNumbers.ContainsKey(p))
                {
                    partNumbers.Add(p, 0);
                }
                if (p == "move")
                    moveParts++;
                partNumbers[p]++;
            }
        }
        private string MostParts()
        {
            int highestInt = 0;
            string highestString = "";
            foreach (KeyValuePair<string, int> kvp in partNumbers)
            {
                if (kvp.Value > highestInt)
                {
                    highestString = kvp.Key;
                    highestInt = kvp.Value;
                }
            }
            return highestString;
        }
        private Color PartColor(string partIn)
        {
            switch (partIn)
            {
                case "work":
                    return workColor;
                case "heal":
                    return healColor;
                case "ranged_attack":
                    return rangedAttackColor;
                case "attack":
                    return attackColor;
                case "carry":
                    return carryColor;
                case "claim":
                    return claimColor;
                case "tough":
                    return toughColor;
                case "move":
                    return moveColor;
                default:
                    return Color.black;
            }
        }
        public void CreateParts(CreepBody bodyIn, GameObject lastRing)
        {
            // initialize variables
            if (bodyPart == null)
                bodyPart = Resources.Load<GameObject>("bodyPart");
            int totalParts = bodyIn.parts.Length;
            moveParts = 0;
            ringOut = lastRing;
            body = bodyIn;
            if (partGOs.Count <= 0)
                partGOs = new Dictionary<string, GameObject>();
            if (partNumbers.Count <= 0)
                partNumbers = new Dictionary<string, int>();
            // get total number of each part
            CountParts();
            int bottomParts = totalParts - moveParts;
            if (moveParts > 0)
            {
                float currentPercent = moveParts * 0.02f;
                string nextPart = "move";
                if (!partGOs.ContainsKey(nextPart))
                {
                    GameObject nextGO = Instantiate(bodyPart, ringOut.transform);
                    partGOs.Add(nextPart, nextGO);
                }
                Image _image = partGOs[nextPart].GetComponent<Image>();
                _image.fillAmount = currentPercent;
                _image.fillOrigin = 0;
                _image.color = PartColor(nextPart);
                RectTransform _rect = partGOs[nextPart].GetComponent<RectTransform>();
                _rect.localEulerAngles = new Vector3(0, 0, 3.6f * moveParts);
                totalParts -= partNumbers[nextPart];
                partNumbers.Remove(nextPart);
            }
            while (partNumbers.Count > 0)
            {
                float currentPercent = bottomParts * 0.02f;
                // rank largest to smallest
                string nextPart = MostParts();
                // add GO for parts largest to smallest
                if (!partGOs.ContainsKey(nextPart))
                {
                    GameObject nextGO = Instantiate(bodyPart, ringOut.transform);
                    partGOs.Add(nextPart, nextGO);
                }
                // largest is base, add smaller parts to outside
                Image _image = partGOs[nextPart].GetComponent<Image>();
                _image.fillAmount = currentPercent;
                _image.color = PartColor(nextPart);
                _image.fillOrigin = 2;
                RectTransform _rect = partGOs[nextPart].GetComponent<RectTransform>();
                _rect.localEulerAngles = new Vector3(0, 0, 3.6f * bottomParts);
                totalParts -= partNumbers[nextPart];
                bottomParts = totalParts;
                partNumbers.Remove(nextPart);
            }
            // return GameObject
        }
    }
}