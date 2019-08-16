using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ScreepsViewer;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    public GameObject controllerBG;
    public GameObject controllerLevel;
    private SpriteRenderer controllerLevelSR;
    public Image controllerProgress;
    public GameObject controllerBadge;
    public int progress;
    public int progressTotal;
    public int level;
    private bool badgeAttached = false;
    public string _id;
    public string user;

    public Sprite controllerLevel1;
    public Sprite controllerLevel2;
    public Sprite controllerLevel3;
    public Sprite controllerLevel4;
    public Sprite controllerLevel5;
    public Sprite controllerLevel6;
    public Sprite controllerLevel7;
    public Sprite controllerLevel8;
    public Sprite controllerLevel0;

    private void Update()
    {
        if (!badgeAttached)
        {
            GetBadge();
        }
    }
    private void Awake()
    {
        controllerBG = transform.GetChild(0).gameObject;
        controllerLevel = transform.GetChild(1).gameObject;
        controllerLevelSR = controllerLevel.GetComponent<SpriteRenderer>();
        controllerBadge = transform.GetChild(2).gameObject;
        controllerProgress = transform.GetChild(3).gameObject.GetComponent<Image>();
    }

    private void SetProgress(int progressIn)
    {
        if (progress == progressIn || progressTotal == 0) { return; }
        float percent = progressIn / (progressTotal * 1f);
        controllerProgress.fillAmount = percent;
        progress = progressIn;
    }
    private void SetLevel(int levelIn)
    {
        controllerLevelSR.color = level > 0 ? Color.white : Color.black;
        switch (levelIn)
        {
            case 1:
                controllerLevelSR.sprite = controllerLevel1;
                break;
            case 2:
                controllerLevelSR.sprite = controllerLevel2;
                break;
            case 3:
                controllerLevelSR.sprite = controllerLevel3;
                break;
            case 4:
                controllerLevelSR.sprite = controllerLevel4;
                break;
            case 5:
                controllerLevelSR.sprite = controllerLevel5;
                break;
            case 6:
                controllerLevelSR.sprite = controllerLevel6;
                break;
            case 7:
                controllerLevelSR.sprite = controllerLevel7;
                break;
            case 8:
                controllerLevelSR.sprite = controllerLevel8;
                break;
            default:
                controllerLevelSR.sprite = controllerLevel0;
                break;
        }
    }
    private void GetBadge()
    {
        if (user == null || !transform.parent.parent.GetComponent<ObjectsLoader>().users.ContainsKey(user))
        {
            return;
        }
        Sprite badgeSprite = transform.parent.parent.GetComponent<ObjectsLoader>().users[user].badgePNG;
        if (badgeSprite != null)
        {
            controllerBadge.GetComponent<SpriteRenderer>().sprite = badgeSprite;
            badgeAttached = true;
        }
    }
    public void SetBadge(Sprite badgeSpriteIn)
    {
        controllerBadge.GetComponent<SpriteRenderer>().sprite = badgeSpriteIn;
        badgeAttached = true;
    }
    public void ControllerData(JSONObject dataIn)
    {
        _id = dataIn["_id"] != null ? dataIn["_id"].ToString().Replace("\"", "") : _id;
        user = dataIn["user"] != null ? dataIn["user"].ToString().Replace("\"", "") : _id;
        int x = dataIn["x"] != null ? Int32.Parse(dataIn["x"].ToString()) : -1;
        int y = dataIn["y"] != null ? Int32.Parse(dataIn["y"].ToString()) : -1;

        if (dataIn["progress"] != null)
        {
            int p = Int32.Parse(dataIn["progress"].ToString());
            if ((progress == -1 && p == 0) || p != progress)
            {
                if (progressTotal == 0 && dataIn["progressTotal"])
                {
                    progressTotal = Int32.Parse(dataIn["progressTotal"].ToString());
                }
                SetProgress(p);
            }
        }
        if (dataIn["level"] != null)
        {
            dataIn.GetField(ref level, "level");
            SetLevel(level);
        }
            //{ "_id":"5982fef6b097071b4adc1d4f","room":"E15S19","type":"controller","x":28,"y":42,"level":0,"user":null,"progress":0,"downgradeTime":null,"safeMode":null,"upgradeBlocked":null,"safeModeCooldown":15999586,"safeModeAvailable":0,"reservation":{ "user":"588d50e64b9e9fe43a40ebb5","endTime":18687540},"hits":0,"hitsMax":0,"progressTotal":0,"sign":{ "user":"588d50e64b9e9fe43a40ebb5","text":"[Ypsilon Pact] Reserved by admon","time":18479861,"datetime":1560864457706} }
            // downgradeTime
            // safeMode
            // upgradeBlocked
            // safeModeCooldown
            // safeModeAvailable
            // reservation
        }
}
