using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ScreepsViewer;

public class Tower : MonoBehaviour
{
    public float speed = 5;
    public bool pewing;
    public string target;
    public int hits;
    public int maxHits;
    public GameObject targetGO;
    public GameObject towerEnergy;
    public GameObject laser;
    private Vector3Int lastTarget;
    public int energy;
    public int x;
    public int y;

    private void Awake()
    {
        pewing = false;
        target = null;
        targetGO = null;
        towerEnergy = transform.GetChild(0).gameObject;
    }

    private void Update()
    {
        if (!pewing)
        {
            transform.Rotate(0, 0, Time.deltaTime * speed, Space.Self);
        }
        else
        {
            //if (target != null)
            //{
            //    transform.LookAt(lastTarget);
            //}
        }
    }
    public void SetEnergy(int energyIn)
    {
        float percent = energyIn / 1000f;
        towerEnergy.transform.localScale = new Vector2(2.2f, percent * 1.8f);
    }
    private void ShootTarget(Vector2Int targetIn, string type)
    {
        Vector2Int towerPos = new Vector2Int(x, 50 - y);
        //lastTarget = new Vector3Int(targetIn.x, targetIn.y, 0);
        Vector2Int dir = targetIn - towerPos;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);
        if (targetGO == null)
        {
            GameObject _laser = Instantiate(laser, gameObject.transform.parent.transform);
            _laser.GetComponent<Laser>().Load(towerPos, targetIn, type);
            _laser.GetComponent<Laser>().SetWidth(0.04f);
            targetGO = _laser;
        } else
        {
            targetGO.SetActive(true);
            targetGO.GetComponent<Laser>().Load(towerPos, targetIn, type);
        }
    }
    public void TowerData(JSONObject dataIn)
    {
        pewing = false;
        if (dataIn.HasField("x"))
            dataIn.GetField(ref x, "x");
        if (dataIn.HasField("y"))
            dataIn.GetField(ref y, "y");
        if (dataIn["hits"] != null)
        {
            hits = Int32.Parse(dataIn["hits"].ToString());
        }
        if (dataIn.HasField("energy"))
        {
            SetEnergy(Int32.Parse(dataIn["energy"].ToString()));
        }
        if (dataIn.HasField("actionLog"))
        {
            var actionLog = dataIn.GetField("actionLog");
            foreach (var action in actionLog.keys)
            {
                if (actionLog[action].type == JSONObject.Type.NULL)
                    continue;
                pewing = true;
                int tarX = 0;
                int tarY = 0;
                if (actionLog[action].HasField("x"))
                    actionLog[action].GetField(ref tarX, "x");
                if (actionLog[action].HasField("y"))
                    actionLog[action].GetField(ref tarY, "y");
                tarY = 50 - tarY;
                Vector2Int targetPos = new Vector2Int(tarX, tarY);
                ShootTarget(targetPos, action.ToString());
            }
        }
    }
}
