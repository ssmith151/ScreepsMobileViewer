using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ScreepsViewer;

public class PowerCreep : MonoBehaviour
{
    public string _id;
    public string user;
    public string userName;
    public string name;
    public int hits;
    public int maxHits;
    public int level;
    public CreepBody creepBody;

    public int energy;
    public int energyCapacity;
    public GameObject creepEnergy;
    public GameObject creepBadge;
    public GameObject creepLevel;
    public float energyWidth = 2f;
    public float energyHeight = 2f;

    public Vector2Int lastPosition;
    public Vector2Int newPosition;
    private Vector3 v3lastPosition;
    private Vector3 v3newPosition;
    public bool moving = false;
    public float moveStartTime;
    public float moveDistance;
    public float speed;
    public bool badgeAttached = false;

    public Sprite operatorLvl1;
    public Sprite operatorLvl2;
    public Sprite operatorLvl3;
    public Sprite operatorLvl4;

    [SerializeField]
    private bool deleting = false;
    private float deleteStart;

    private List<SpriteRenderer> fadeoutSprites;

    private void Awake()
    {
        hits = -1;
        energy = -1;
        creepEnergy = transform.GetChild(3).gameObject;
        creepBadge = transform.GetChild(2).gameObject;
        creepLevel = transform.GetChild(1).gameObject;
        moving = false;
        lastPosition = new Vector2Int(-100, -100);
        if (fadeoutSprites == null || fadeoutSprites.Count == 0)
        {
            fadeoutSprites = new List<SpriteRenderer>();
            if (gameObject.GetComponent<SpriteRenderer>() != null)
            {
                fadeoutSprites.Add(gameObject.GetComponent<SpriteRenderer>());
            }
            fadeoutSprites.AddRange(gameObject.GetComponentsInChildren<SpriteRenderer>());
        }
    }
    private void Update()
    {
        if (moving)
        {
            Movement();
        }
        if (deleting)
        {
            DeleteCreep();
        }
        if (!badgeAttached)
        {
            GetBadge();
        }
    }
    private void Movement()
    {
        float movementTiming = (Time.time - moveStartTime) * speed;
        float movePoint = movementTiming / moveDistance;
        transform.position = Vector3.Lerp(
            new Vector3(lastPosition.x * 0.16f + 0.08f, lastPosition.y * 0.16f + 0.08f, -1f),
            new Vector3(newPosition.x * 0.16f + 0.08f, newPosition.y * 0.16f + 0.08f, -1f),
            movementTiming);
        if (movementTiming >= 1)
        {
            moving = false;
            lastPosition = newPosition;
            if ((lastPosition.x <= 0 || lastPosition.y <= 0 || lastPosition.x >= 49 || lastPosition.y >= 49) && (lastPosition.x != -100 || lastPosition.y != -100))
            {
                // start deleting the creep if it moved out of the room;
                DeleteCreep();
            }
        }
    }
    public void DeleteCreep()
    {
        if (!deleting)
        {
            deleting = true;
            deleteStart = Time.time;
            Destroy(gameObject, 2f);
        }
        if (fadeoutSprites == null || fadeoutSprites.Count == 0)
        {
            fadeoutSprites = new List<SpriteRenderer>();
            if (gameObject.GetComponent<SpriteRenderer>() != null)
            {
                fadeoutSprites.Add(gameObject.GetComponent<SpriteRenderer>());
            }
            fadeoutSprites.AddRange(gameObject.GetComponentsInChildren<SpriteRenderer>());
        }
        foreach (SpriteRenderer sr in fadeoutSprites)
        {
            float percent = Time.time - deleteStart;
            sr.color = Color.Lerp(sr.color, Color.clear, percent);
        }
    }
    public void SetEnergy(int energyIn)
    {
        if (energy == energyIn) { return; }
        if (energyCapacity == 0 || energyIn <= 0)
        {
            creepEnergy.transform.localScale = new Vector2(energyWidth, 0.05f * energyHeight);
        }
        else
        {
            float percent = energyIn / (energyCapacity * 1f);
            creepEnergy.transform.localScale = new Vector2(energyWidth, percent * energyHeight);
        }
        energy = energyIn;
    }
    public void CreepData(JSONObject dataIn)
    {
        if (dataIn == null) { DeleteCreep(); return; }
        _id = dataIn["_id"] != null ? dataIn["_id"].ToString().Replace("\"", "") : _id;
        user = dataIn["user"] != null ? dataIn["user"].ToString().Replace("\"", "") : _id;
        int x = dataIn["x"] != null ? Int32.Parse(dataIn["x"].ToString()) : lastPosition.x;
        int y = dataIn["y"] != null ? Int32.Parse(dataIn["y"].ToString()) : lastPosition.y;
        if (lastPosition == new Vector2Int(-100, -100))
        {
            lastPosition = new Vector2Int(x, -y + 50);
        }
        if (lastPosition.x != x || lastPosition.y != y)
        {
            moving = true;
            newPosition = lastPosition.y != y ? new Vector2Int(x, -y + 50) : new Vector2Int(x, y);
            moveStartTime = Time.time;
            v3lastPosition = new Vector3(lastPosition.x * 0.16f + 0.08f, lastPosition.y * 0.16f + 0.08f, -1f);
            v3newPosition = new Vector3(newPosition.x * 0.16f + 0.08f, newPosition.y * 0.16f + 0.08f, -1f);
            moveDistance = Vector3.Distance(v3lastPosition, v3newPosition);
        }
        if (dataIn["energy"] != null)
        {
            int e = Int32.Parse(dataIn["energy"].ToString());
            if ((energy == -1 && e == 0) || e != energy)
            {
                if (energyCapacity == 0 && dataIn["energyCapacity"])
                {
                    energyCapacity = Int32.Parse(dataIn["energyCapacity"].ToString());
                }
                SetEnergy(e);
            }
        }
        if (dataIn["name"] != null)
        {
            name = dataIn["name"].ToString().Replace("\"", "");
        }
        if (dataIn["level"] != null)
        {
            level = Int32.Parse(dataIn["level"].ToString());
            SetLevel();
        }
        if (dataIn["hits"] != null)
        {
            hits = Int32.Parse(dataIn["hits"].ToString());
        }
    }
    public void SetBadge(Sprite badgeSpriteIn)
    {
        creepBadge.GetComponent<SpriteRenderer>().sprite = badgeSpriteIn;
        badgeAttached = true;
    }
    private void SetLevel()
    {
        SpriteRenderer pcR = creepLevel.GetComponent<SpriteRenderer>();
        if (level > 1 && level < 7)
        {
            pcR.sprite = operatorLvl1;
        }
        if (level > 6 && level < 13)
        {
            pcR.sprite = operatorLvl2;
        }
        if (level > 12 && level < 19)
        {
            pcR.sprite = operatorLvl3;
        }
        if (level > 18)
        {
            pcR.sprite = operatorLvl4;
        }
        pcR.color = new Color(0.95f, 0.2f, 0.2f);
    }
    private void GetBadge()
    {
        if (user == null)
        {
            return;
        }
        if (user == "2" || user == "3")
        {
            badgeAttached = true;
            return;
        }
        Sprite badgeSprite = transform.parent.parent.GetComponent<ObjectsLoader>().users[user].badgePNG;
        if (badgeSprite != null)
        {
            creepBadge.GetComponent<SpriteRenderer>().sprite = badgeSprite;
            badgeAttached = true;
        }
    }
}
