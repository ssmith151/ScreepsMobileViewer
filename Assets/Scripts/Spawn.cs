using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ScreepsViewer;

public class Spawn : MonoBehaviour
{
    public GameObject spawnEnergy;
    public GameObject spawnFinished;
    public GameObject spawnBadge;
    public int energy;
    public int energyCapacity;
    public int finished;
    private int finishedFinal;
    private int lastGameTime;
    private float moveStartTime;
    private float[] targetSizes = { 0.6f, 1.3f, 0.6f, 1.2f, 0.6f, 0.7f, 0.8f, 0.9f, 1.0f, 1.1f };
    public int finishedCapacity;
    public float speed = 0.1f;
    public float energyWidth = 2f;
    public float energyHeight = 4.1f;
    public float finishedWidth = 2f;
    public float finishedHeight = 4.1f;
    [SerializeField]
    private bool deleting = false;
    private float deleteStart;
    private List<SpriteRenderer> fadeoutSprites;
    private bool badgeAttached = false;
    public string _id;
    public string user;

    private void Awake()
    {
        finishedFinal = -1;
        spawnEnergy = transform.GetChild(0).gameObject;
        spawnFinished = transform.GetChild(1).gameObject;
        spawnBadge = transform.GetChild(2).gameObject;
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
        if (deleting)
        {
            DeleteSpawn();
        }
        if (finishedFinal != -1)
        {
            AnimateSpawn();
            if (ObjectsLoader.gameTime != lastGameTime)
            {
                SetFinished(finishedCapacity - (finishedFinal - ObjectsLoader.gameTime));
            }
        }
        if (!badgeAttached)
        {
            GetBadge();
        }
    }
    private void GetBadge()
    {
        if (user == null)
        {
            return;
        }
        if (transform.parent.parent.GetComponent<ObjectsLoader>().users.ContainsKey(user))
        {
            Sprite badgeSprite = transform.parent.parent.GetComponent<ObjectsLoader>().users[user].badgePNG;
            if (badgeSprite != null)
            {
                spawnBadge.GetComponent<SpriteRenderer>().sprite = badgeSprite;
                badgeAttached = true;
            }
        }
    }
    private int ModTimeToInt(int x, int m)
    {
        int r = x % m;
        return r < 0 ? r + m : r;
    }
    private void AnimateSpawn()
    {
        float lastSize = targetSizes[ModTimeToInt((ObjectsLoader.gameTime - 1), 10)];
        float currentTargetSize = targetSizes[ObjectsLoader.gameTime % 10];
        float movementTiming = (Time.time - moveStartTime) * speed / Math.Abs(currentTargetSize - lastSize);
        transform.localScale = Vector3.Lerp(
            new Vector3(lastSize, lastSize),
            new Vector3(currentTargetSize, currentTargetSize),
            movementTiming);
    }
    private void DeleteSpawn()
    {
        if (!deleting)
        {
            deleting = true;
            deleteStart = Time.time;
            Destroy(this.gameObject, 1f);
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
        if (energy == energyIn || energyCapacity == 0) { return; }
        float percent = energyIn / (energyCapacity * 1f);
        spawnEnergy.transform.localScale = new Vector2(energyWidth, percent * energyHeight);
        energy = energyIn;
    }
    public void SetFinished(int finishedIn)
    {
        if (finished == finishedIn || finishedCapacity == 0) { return; }
        if (finishedFinal == -1 && ObjectsLoader.gameTime != 0)
            finishedFinal = ObjectsLoader.gameTime + finishedCapacity - finishedIn;
        float percent = finishedIn / (finishedCapacity * 1f);
        spawnFinished.transform.localScale = new Vector2(finishedWidth, percent * finishedHeight);
        finished = finishedIn;
        moveStartTime = Time.time;
        lastGameTime = ObjectsLoader.gameTime;
        if (ObjectsLoader.gameTime >= finishedFinal && ObjectsLoader.gameTime != 0)
        {
            finishedFinal = -1;
            gameObject.transform.localScale = Vector3.one;
        }
    }
    public void SpawnData(JSONObject dataIn)
    {
        if (dataIn == null) { DeleteSpawn(); return; }
        _id = dataIn["_id"] != null ? dataIn["_id"].ToString().Replace("\"", "") : _id;
        user = dataIn["user"] != null ? dataIn["user"].ToString().Replace("\"", "") : _id;
        if (dataIn["energy"])
        {
            int e = Int32.Parse(dataIn["energy"].ToString());
            if (e != 0 || e != energy)
            {
                if (energyCapacity == 0 && dataIn["energyCapacity"])
                {
                    energyCapacity = Int32.Parse(dataIn["energyCapacity"].ToString());
                }
                SetEnergy(e);
            }
        }
        if (dataIn["spawning"] && dataIn["spawning"] != null)
        {
            if (dataIn["spawning"]["needTime"])
            {
                finishedCapacity = Int32.Parse(dataIn["spawning"]["needTime"].ToString());
            }
            if (dataIn["spawning"]["remainingTime"])
            {
                int e = Int32.Parse(dataIn["spawning"]["remainingTime"].ToString());
                SetFinished(e);
            }
            //if (e != 0 || e != energy)
            //{
            //    if (energyCapacity == 0 && dataIn["energyCapacity"])
            //    {
            //        energyCapacity = Int32.Parse(dataIn["energyCapacity"].ToString());
            //    }
            //    SetEnergy(e);
            //}
            //Debug.Log(dataIn["spawning"]);
        }
    }
}