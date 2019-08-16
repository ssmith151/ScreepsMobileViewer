using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nuker : MonoBehaviour
{
    public GameObject nukerEnergy;
    public GameObject nukerGhodium;
    public int energy;
    public int ghodium;
    public float energyWidth = 3.25f;
    public float energyHeight = 4f;
    public float ghodiumWidth = 0.95f;
    public float ghodiumHeight = 3.35f;

    private void Awake()
    {
        nukerEnergy = transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;
        nukerGhodium = transform.GetChild(0).gameObject.transform.GetChild(1).gameObject;
    }
    public void SetEnergy(int energyIn)
    {
        float percent = energyIn / 300000f;
        nukerEnergy.transform.localScale = new Vector2(energyWidth, percent * energyHeight);
    }
    public void SetGhodium(int ghodiumIn)
    {
        float percent = ghodiumIn / 5000f;
        nukerEnergy.transform.localScale = new Vector2(ghodiumWidth, percent * ghodiumHeight);
    }
}