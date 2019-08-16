using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScreepsViewer;
using System;

public class BadgeStorage : MonoBehaviour
{
    public Dictionary<string,Sprite> PlayerBadges;
    private BadgeGenerator bg;
    private float loadTime;

    private void Awake()
    {
        loadTime = -1f;
        bg = gameObject.GetComponent<BadgeGenerator>();
        PlayerBadges = new Dictionary<string, Sprite>();
    }
    public void StoreBadge(string playerIdIn, Badge badgeIn, Sprite spriteIn)
    {
        if (!PlayerBadges.ContainsKey(playerIdIn))
        {
            PlayerBadges.Add(playerIdIn, spriteIn);
        }
    }
    public void GetBadge(string playerIdIn, Badge badgeIn, Action<Sprite> callback) // wrapper
    {
        StartCoroutine(IRetrieveBadge(playerIdIn, badgeIn, callback));
    }
    public IEnumerator IRetrieveBadge(string playerIdIn, Badge badgeIn, Action<Sprite> callback)
    {
        Debug.Log("Attempting to get badge for: " + playerIdIn);
        Sprite s = RetrieveBadge(playerIdIn, badgeIn);
        yield return s;
        if (s == null)
        {
            Debug.Log("Could not retrieve badge for: " + playerIdIn);
            loadTime = -1f;
            callback.Invoke(s);
            yield return null;
        } else
        {
            callback.Invoke(s);
            loadTime = -1f;
            yield return s;
        }
    }
    private Sprite RetrieveBadge(string playerIdIn, Badge badgeIn)
    {
        if (loadTime < 0)
            loadTime = Time.time;
        if (PlayerBadges.ContainsKey(playerIdIn))
        {
            return PlayerBadges[playerIdIn];
        }
        else
        {
            Sprite loadedSprite = LoadBadgeSprite(playerIdIn);
            if (loadedSprite != null)
            {
                PlayerBadges[playerIdIn] = loadedSprite;
                return loadedSprite;
            }
            if (Time.time - loadTime >= 5.00f)
            {
                return null;
                //Texture2D tex = new Texture2D(1, 1);
                //Sprite sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
                //return sprite;
            }
            Debug.Log("creating new badge...");
            Sprite createdSprite = bg.CreateBadge(badgeIn, 64);
            //Debug.Log(createdSprite.border.x);
            return createdSprite;
        }
    }
    private void SaveBadgeSprite(Sprite spriteIn,string playerIdIn)
    {
        //Texture2D t = new Texture2D(1, 1);
        byte[] saveBytes = spriteIn.texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.persistentDataPath + "/badges/" + playerIdIn + "badge.png", saveBytes);
        Debug.Log("saved new badge : " + Application.persistentDataPath + "/badges/" + playerIdIn + "badge.png");
    }
    private Sprite LoadBadgeSprite(string playerIdIn)
    {
        string path = Application.persistentDataPath + "/badges/" + playerIdIn + "badge.png"; 
        if (string.IsNullOrEmpty(playerIdIn)) return null;
        if (System.IO.File.Exists(path))
        {
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(bytes);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            Debug.Log("loaded badge : " + Application.persistentDataPath + "/badges/" + playerIdIn + "badge.png");
            return sprite;
        }
        return null;
    }
}
