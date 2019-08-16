using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ScreepsViewer;

public class PowerSpawn : MonoBehaviour
{
    public GameObject spawnEnergy;
    public GameObject spawnPower;
    public GameObject spawnBadge;
    public int energy;
    public int energyCapacity;
    public int power;
    public int powerCapacity;
    private int lastPower;
    public bool processing;
    private int lastGameTime;
    private float moveStartTime;
    private float[] targetSizes = { 0.6f, 0.7f, 0.8f, 0.9f, 1.0f, 1.1f, 1.0f, 0.9f, 0.8f, 0.7f };
    public float speed = 0.1f;
    public float energyWidth = 2f;
    public float energyHeight = 4.1f;
    public float powerWidth = 2f;
    public float powerHeight = 4.1f;
    [SerializeField]
    private bool deleting = false;
    private float deleteStart;
    private List<SpriteRenderer> fadeoutSprites;
    private bool badgeAttached = false;
    public string _id;
    public string user;

    private void Awake()
    {
        processing = false;
        spawnEnergy = transform.GetChild(0).gameObject;
        spawnPower = transform.GetChild(1).gameObject;
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
        if (processing)
        {
            AnimateSpawn();
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
        Sprite badgeSprite = transform.parent.parent.GetComponent<ObjectsLoader>().users[user].badgePNG;
        if (badgeSprite != null)
        {
            spawnBadge.GetComponent<SpriteRenderer>().sprite = badgeSprite;
            badgeAttached = true;
        }
    }
    private int ModTimeToInt(int x, int m)
    {
        int r = x % m;
        return r < 0 ? r + m : r;
    }
    private void AnimateSpawn()
    {
        if (lastGameTime < ObjectsLoader.gameTime)
        {
            processing = false;
            transform.localScale = Vector3.one;
            return;
        }
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
    public void SetPower(int powerIn)
    {
        if (power == powerIn || powerCapacity == 0) { return; }
        float percent = power / (powerCapacity * 1f);
        spawnPower.transform.localScale = new Vector2(powerWidth, percent * powerWidth);
        power = powerIn;
        if (power < lastPower)
        {
            processing = true;
            moveStartTime = Time.time;
            lastGameTime = ObjectsLoader.gameTime;
        }
        lastPower = power;
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
        if (dataIn.HasField("power"))
        {
            Debug.Log("powerchange");
            int p = 0;
            dataIn.GetField(ref p, "power");
            if (powerCapacity == 0 && dataIn["powerCapacity"])
            {
                powerCapacity = Int32.Parse(dataIn["powerCapacity"].ToString());
            }
            SetPower(p);
        }
    }
}
