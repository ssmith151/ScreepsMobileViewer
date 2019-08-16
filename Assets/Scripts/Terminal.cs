using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ScreepsViewer;

public class Terminal : MonoBehaviour
{
    public GameObject storageEnergy;
    public GameObject storageMineral;
    public int energy;
    public int energyCapacity;
    public int mineral;
    public float energyWidth = 2.5f;
    public float energyHeight = 2.5f;
    public float mineralWidth = 2.5f;
    public float mineralHeight = 2.5f;

    private void Awake()
    {
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
        float percent = 0.1f;
        percent = mineralIn > 0 ? mineralIn / (energyCapacity * 1f) : percent;
        storageMineral.transform.localScale = new Vector2(mineralWidth, percent * mineralHeight);
    }
    public void StorageData(JSONObject dataIn)
    {
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
            if (dataIn[r] && dataIn[r] != null)
            {
                int nextMin = 0;
                dataIn.GetField(ref nextMin, r);
                mineral += nextMin > 0 ? nextMin : 0;
            }
        }
        SetMineral(mineral);
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
