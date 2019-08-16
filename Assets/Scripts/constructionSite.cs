using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class constructionSite : MonoBehaviour
{
    public Image constructionInternals;
    public int progress;
    public int progressTotal;
    public float energyWidth = 2f;
    public float energyHeight = 2f;
    [SerializeField]
    private bool deleting = false;
    private float deleteStart;
    private List<SpriteRenderer> fadeoutSprites;
    
    private void Awake()
    {
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
    private void DeleteSite()
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
    public void SetProgress(int progressIn)
    {
        if (progress == progressIn || progressTotal == 0) { return; }
        float percent = progressIn / (progressTotal * 1f);
        constructionInternals.fillAmount = percent;
        progress = progressIn;
        if (percent >= 1) { DeleteSite(); }
    }
    public void SiteData(JSONObject dataIn)
    {
        if (dataIn == null) { DeleteSite(); return; }
        if (dataIn["progressTotal"])
        {
            progressTotal = Int32.Parse(dataIn["progressTotal"].ToString());
        }
        if (dataIn["progress"])
        {
            int e = Int32.Parse(dataIn["progress"].ToString());
            if (e != 0 || e != progress)
            {
                SetProgress(e);
            }
        }
    }
}
