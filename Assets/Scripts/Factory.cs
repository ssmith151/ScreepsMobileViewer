using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ScreepsViewer;

public class Factory : MonoBehaviour
{
    public GameObject factoryEnergy;
    public GameObject factoryMineral;
    public GameObject factoryLevel;
    public int energy;
    public int energyCapacity;
    public int mineral;
    public int level;
    public Dictionary<string, int> store;
    public float energyWidth = 2.5f;
    public float energyHeight = 2.5f;
    public float mineralWidth = 2.5f;
    public float mineralHeight = 2.5f;

    public Sprite level1;
    public Sprite level2;
    public Sprite level3;
    public Sprite level4;
    public Sprite level5;

    private void Awake()
    {
        store = new Dictionary<string, int>();
        mineral = 0;
        factoryLevel = transform.GetChild(2).gameObject;
        factoryEnergy = transform.GetChild(3).gameObject;
        factoryMineral = transform.GetChild(4).gameObject;
    }
    public void SetEnergy(int energyIn)
    {
        energy = energyIn;
        float percent = 0.001f;
        percent = energyIn > 0 ? (energyIn + mineral) / (energyCapacity * 1f) : percent;
        factoryEnergy.transform.localScale = new Vector2(energyWidth, percent * energyHeight);
    }
    public void SetMineral(int mineralIn)
    {
        mineral = mineralIn;
        float percent = 0.001f;
        percent = mineralIn > 0 ? mineralIn / (energyCapacity * 1f) : percent;
        factoryMineral.transform.localScale = new Vector2(mineralWidth, percent * mineralHeight);
    }
    public void SetLevel(int levelIn)
    {
        SpriteRenderer levelSr = factoryLevel.GetComponent<SpriteRenderer>();
        switch (levelIn)
        {
            case 1:
                levelSr.sprite = level1;
                break;
            case 2:
                levelSr.sprite = level2;
                break;
            case 3:
                levelSr.sprite = level3;
                break;
            case 4:
                levelSr.sprite = level4;
                break;
            case 5:
                levelSr.sprite = level5;
                break;
        }
    }
    public void FactoryData(JSONObject dataIn)
    {
        int mineralIn = 0;
        if (energyCapacity == 0 && dataIn["energyCapacity"])
        {
            energyCapacity = Int32.Parse(dataIn["energyCapacity"].ToString());
        }
        foreach (string r in Constants.RESOURCES_ALL)
        {
            if (r == "energy")
            {
                continue;
            }
            if (dataIn[r])
            {
                string min = dataIn[r].ToString();
                int value = Int32.Parse(min) > 0 ? Int32.Parse(min) : 0;
                if (value > 0 && !store.ContainsKey(r))
                {
                    store.Add(r, value);
                }
                if (store.ContainsKey(r))
                {
                    if (store[r] != value)
                    {
                        store[r] = value;
                    }
                    mineralIn += value;
                }

            }
        }
        SetMineral(mineralIn);
        if (dataIn["energy"])
        {
            int e = Int32.Parse(dataIn["energy"].ToString());
            if (e != 0 || e != energy)
            {
                SetEnergy(e);
            }
        }
        if (dataIn.HasField("level"))
        {
            dataIn.GetField(ref level, "level");
            SetLevel(level);
        }
    }
}
