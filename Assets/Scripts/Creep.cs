using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ScreepsViewer;

[Serializable]
public class CreepBody
{
    //"body":[{"type":"work","hits":100},{"type":"work","hits":100},{"type":"carry","hits":100},{"type":"carry","hits":100},{"type":"move","hits":100},{"type":"move","hits":100}]
    public string[] parts;
    public bool boosted;
    public bool[] boosts;
    public bool[] active;

    public CreepBody(JSONObject bodyIn)
    {
        if (bodyIn.type != JSONObject.Type.ARRAY) { Debug.Log("Creep Body is not an array"); parts = null; active = null; boosts = null; return; }
        int totalBody = bodyIn.list.Count;
        parts = new string[totalBody];
        boosts = new bool[totalBody];
        active = new bool[totalBody];
        int counter = 0;
        foreach (JSONObject j in bodyIn.list)
        {
            if (j.HasField("hits") && j.HasField("type"))
            {
                active[counter] = Int32.Parse(j["hits"].ToString()) > 0 ? true : false;
                parts[counter] = active[counter] ? j["type"].ToString().Replace("\"", "") : "empty";
            }
            if (j["boost"] != null)
            {
                boosts[counter] = true;
                parts[counter] = parts[counter] + "Boosted";
            } else
            {
                boosts[counter] = false;
            }
            counter++;
        }
    }
    public CreepBody UpdateBody(JSONObject objIn, CreepBody bodyIn)
    {
        // this does not come in a list form like the orginal array
        //"body":{ "0":{ "type":"tough","hits":100,"boost":"XGHO2"},"1":{ "type":"tough","hits":100,"boost":"XGHO2"} }
        int totalBody = bodyIn.parts.Length;
        for (int i = 0; i<totalBody; i++)
        {
            if (objIn.HasField(i.ToString()))
            {
                if (objIn[i.ToString()].HasField("hits"))
                {
                    int newHits = 0;
                    objIn[i.ToString()].GetField(ref newHits, "hits");
                    active[i] = newHits > 0 ? true : false;
                    parts[i] = active[i] ? objIn[i.ToString()]["type"].ToString().Replace("\"", "") : "empty";
                }
            }
        }
        return bodyIn;
    }
}
public struct Dir
{
    public int dir;
    public int dirX;
    public int dirY;
    public Dir(Vector2Int oldPos, Vector2Int newPos)
    {
        dirX = Mathf.RoundToInt(Mathf.Sign(oldPos.x - newPos.x));
        dirY = Mathf.RoundToInt(Mathf.Sign(oldPos.y - newPos.y));

        if (oldPos.y - newPos.y > 0) // U
        {
            if (oldPos.x - newPos.x > 0)
                dir = 4;
            else if (oldPos.x - newPos.x < 0)
                dir = 6;
            else
                dir = 5;
        } else if (oldPos.y - newPos.y < 0) // D
        {
            if (oldPos.x - newPos.x > 0)
                dir = 2;
            else if (oldPos.x - newPos.x < 0)
                dir = 8;
            else
                dir = 1;
        } else // mid
        {
            if (oldPos.x - newPos.x > 0)
                dir = 3;
            else if (oldPos.x - newPos.x < 0)
                dir = 7;
            else
                dir = 0;
        }
    }
}
public class Creep : MonoBehaviour, ObjectsLoader.IRoomObject, ObjectsLoader.IHitsObject
{
    public string _id;
    public string user;
    public string userName;
    public string name;
    public int hits;
    public int maxHits;
    public CreepBody creepBody;
    
    public int energy;
    public int energyCapacity;
    public GameObject creepEnergy;
    public GameObject creepBadge;
    public GameObject creepParts;
    private GameObject creepTough;
    public PartMaker pM;
    public GameObject myLaser;
    public GameObject laser;
    public GameObject myRMA;
    public GameObject RMA;
    public float energyWidth = 2f;
    public float energyHeight = 2f;

    public Vector2Int lastPosition;
    public Vector2Int newPosition;
    private Vector3 v3lastPosition;
    private float lastRotation;
    private float newRotation;
    private Vector3 v3newPosition;
    public bool moving = false;
    public float moveStartTime;
    public float moveDistance;
    public float speed;
    public float turnSpeed;
    public float bumpSpeed;
    public bool bumping;
    public bool badgeAttached = false;
    public int lastGameTime;

    public string lastAction;
    public Vector2Int lastTarget;

    [SerializeField]
    private bool deleting = false;
    private float deleteStart;

    private List<SpriteRenderer> fadeoutSprites;

    private void Awake()
    {
        lastGameTime = ObjectsLoader.gameTime;
        lastAction = null;
        myLaser = null;
        hits = -1;
        energy = -1;
        pM = null;
        creepEnergy = transform.GetChild(0).gameObject;
        creepBadge = transform.GetChild(1).gameObject;
        creepTough = transform.GetChild(3).gameObject;
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
        if (moving) {
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
        if (lastAction != null && ObjectsLoader.gameTime > lastGameTime)
        {
            lastGameTime = ObjectsLoader.gameTime;
            if (lastAction == "upgradeController" || lastAction == "rangedHeal" || lastAction == "rangedAttack" || lastAction == "repair" || lastAction == "build")
            {
                ShootTarget(lastTarget, lastAction);
            }
        }
    }
    private void RangedMassAttack()
    {
        //Vector2Int creepPos = new Vector2Int(newPosition.x, newPosition.y);
        if (myRMA == null)
        {
            GameObject _RMA = Instantiate(RMA, gameObject.transform.parent.transform);
            _RMA.GetComponent<RMA>().Load(newPosition);
            myRMA = _RMA;
        }
        else
        {
            myRMA.SetActive(true);
            myRMA.GetComponent<RMA>().Load(newPosition);
        }
    }
    private IEnumerator TurnTo(int dirIn)
    {
        newRotation = (dirIn - 1) * 45;
        lastRotation = transform.eulerAngles.z;
        int turnDirection = Mathf.Abs((newRotation - lastRotation)) > 180 ? -1 : 1;
        while ((lastRotation < newRotation && turnDirection > 0) || (lastRotation > newRotation && turnDirection < 0))
        {
            yield return new WaitForEndOfFrame();
            transform.eulerAngles += Vector3.forward * turnDirection * turnSpeed;
            lastRotation = transform.eulerAngles.z;
        }
        transform.eulerAngles = Vector3.forward * newRotation;
    }
    private IEnumerator StartBump(Dir dirIn)
    {
        while (lastAction != null)
        {
            //Debug.Log("Starting Bumpto");
            bumping = true;
            v3lastPosition = transform.position;
            Vector3 bumpToPosition = new Vector3(transform.position.x - (dirIn.dirX * 0.04f), transform.position.y - (dirIn.dirY * 0.04f), 0.0f);
            yield return BumpTo(v3lastPosition, bumpToPosition, 1.0f);
            //Debug.Log("Middle Bumpto");
            yield return BumpTo(bumpToPosition, v3lastPosition, 1.5f);
            transform.localPosition = v3lastPosition;
            bumping = false;
            //Debug.Log("Finished Bumpto");
        }
    }
    IEnumerator BumpTo(Vector3 A, Vector3 B, float bumpTime)
    {
        var i = 0.0f;
        var rate = 1.0f / bumpTime * bumpSpeed;
        while (i < 1.0f)
        {
            i += Time.deltaTime * rate;
            transform.position = Vector3.Lerp(A, B, i);
            yield return null;
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
        //user = dataIn["user"] != null ? dataIn["user"].ToString().Replace("\"", "") : _id;
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
            int newDir = new Dir(lastPosition, newPosition).dir;
            StartCoroutine(TurnTo(newDir));
            v3lastPosition = new Vector3(lastPosition.x * 0.16f + 0.08f, lastPosition.y * 0.16f + 0.08f, -1f);
            v3newPosition = new Vector3(newPosition.x * 0.16f + 0.08f, newPosition.y * 0.16f + 0.08f, -1f);
            moveDistance = Vector3.Distance( v3lastPosition, v3newPosition );
        }
        if (dataIn["energy"] != null)
        {
            int e = Int32.Parse(dataIn["energy"].ToString());
            if ((energy == -1 && e == 0)|| e != energy) {
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
        if (dataIn["hits"] != null)
        {
            hits = Int32.Parse(dataIn["hits"].ToString());
        }
        if (dataIn.HasField("user"))
        {
            dataIn.GetField(ref user, "user");
        }
        if ((user == "2" || user == "3") && creepTough != null){
            Destroy(creepTough);
            creepTough = null;
        }
        if (dataIn.HasField("body"))
        {
            if (creepBody.parts.Length <= 0)
            {
                creepBody = new CreepBody(dataIn["body"]);
            }
            if (creepBody.parts.Length > 0)
            {
                maxHits = creepBody.parts.Length * 100;
            }
            if (creepParts == null)
            {
                creepParts = transform.GetChild(2).gameObject;
            }
            
            //creepParts.TryGetComponent<PartMaker>(out pM);
            if (pM == null && !(user == "2" || user == "3"))
            {
                pM = creepParts.AddComponent<PartMaker>();
                pM.CreateParts(creepBody, creepParts);
            }
        }
        if (dataIn.HasField("actionLog"))
        {
            //"attacked":null,"healed":null,"attack":null,"rangedAttack":null,"rangedMassAttack":null,"rangedHeal":null,"harvest":{"x":33,"y":14},"heal":null,"repair":null,"build":null,"say":null,"upgradeController":null,"reserveController":null
            var actionLog = dataIn.GetField("actionLog");
            foreach (var action in actionLog.keys)
            {
                if (actionLog[action].type == JSONObject.Type.NULL)
                {
                    if (lastAction == action)
                    {
                        lastAction = null;
                    }
                    continue;
                }
                string actionType = Convert.ToString(action);
                int tarX = 0;
                int tarY = 0;
                if (actionLog[action].HasField("x"))
                    actionLog[action].GetField(ref tarX, "x");
                if (actionLog[action].HasField("y"))
                    actionLog[action].GetField(ref tarY, "y");
                tarY = 50 - tarY;
                Vector2Int targetPos = new Vector2Int(tarX, tarY);
                lastTarget = targetPos;
                if (actionType == "upgradeController" || actionType == "rangedHeal" || actionType == "rangedAttack" || actionType == "repair" || actionType == "build")
                {
                    lastAction = actionType;
                    ShootTarget(targetPos, actionType);
                }
                if (actionType == "attack" || actionType == "harvest" || actionType == "heal" || actionType == "reserveController")
                {
                    lastAction = actionType;
                    Dir newDir = new Dir(lastPosition, targetPos);
                    //StartCoroutine(TurnTo(newDir.dir));
                    //if (!moving)
                    //{
                        //Debug.Log("Starting Bumpto");
                        StartCoroutine(StartBump(newDir));
                    //}
                }
            }
        } 
    }
    private void ShootTarget(Vector2Int targetIn, string type)
    {
        //Vector2Int creepPos = new Vector2Int(newPosition.x, newPosition.y);
        if (myLaser == null)
        {
            GameObject _laser = Instantiate(laser, gameObject.transform.parent.transform);
            _laser.GetComponent<Laser>().Load(newPosition, targetIn, type);
            myLaser = _laser;
        }
        else
        {
            myLaser.SetActive(true);
            myLaser.GetComponent<Laser>().Load(newPosition, targetIn, type);
        }
    }
    public void SetBadge(Sprite badgeSpriteIn)
    {
        creepBadge.GetComponent<SpriteRenderer>().sprite = badgeSpriteIn;
        badgeAttached = true;
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
        if (transform.parent.parent.GetComponent<ObjectsLoader>().users.ContainsKey(user))
        {
            Sprite badgeSprite = transform.parent.parent.GetComponent<ObjectsLoader>().users[user].badgePNG;
            if (badgeSprite != null)
            {
                creepBadge.GetComponent<SpriteRenderer>().sprite = badgeSprite;
                badgeAttached = true;
            }
        }
    }
    public string Id()
    {
        return _id;
    }
    public int[] Pos()
    {
        int[] _pos = { lastPosition.x, lastPosition.y };
        return _pos;
    }
    public string Type()
    {
        return "creep";
    }
    public int Hits()
    {
        return hits;
    }
    public int MaxHits()
    {
        return maxHits;
    }
}
