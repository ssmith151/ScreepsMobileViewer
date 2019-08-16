using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ScreepsViewer;

public class Storage : MonoBehaviour
{
    public GameObject storageEnergy;
    public GameObject storageMineral;
    public int energy;
    public int energyCapacity;
    public int mineral;
    public Dictionary<string, int> store;
    public float energyWidth = 3.5f;
    public float energyHeight = 4.2f;
    public float mineralWidth = 3.5f;
    public float mineralHeight = 4.2f;

    private void Awake()
    {
        store = new Dictionary<string, int>();
        mineral = 0;
        storageEnergy = transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;
        storageMineral = transform.GetChild(0).gameObject.transform.GetChild(1).gameObject;
    }
    public void SetEnergy(int energyIn)
    {
        energy = energyIn;
        float percent = 0.001f;
        percent = energyIn > 0 ? (energyIn + mineral) / (energyCapacity * 1f) : percent;
        storageEnergy.transform.localScale = new Vector2(energyWidth, percent * energyHeight);
    }
    public void SetMineral(int mineralIn)
    {
        mineral = mineralIn;
        float percent = 0.001f;
        int eC = energyCapacity > 0 ? energyCapacity : 1;
        percent = mineralIn > 0 ? mineralIn / (eC * 1f) : percent;
        storageMineral.transform.localScale = new Vector2(mineralWidth, percent * mineralHeight);
    }
    public void StorageData(JSONObject dataIn)
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
    }
}