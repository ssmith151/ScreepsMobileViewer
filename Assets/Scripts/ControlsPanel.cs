using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ScreepsViewer;
using UnityEngine.SceneManagement;
using TMPro;

public class ControlsPanel : MonoBehaviour
{
    public TextMeshProUGUI cursorText;
    public TextMeshProUGUI roomText;
    public TextMeshProUGUI SelectedDetailsText;
    public GameObject DetailsScrollView;
    public Scrollbar DetailsScrollViewBar;
    public GameObject SelectablesDropdown;
    public Slider EWslider;
    public Slider NSslider;
    public Button Nbutton;
    public Button Sbutton;
    public Button Ebutton;
    public Button Wbutton;
    public Button S0button;
    public Button S1button;
    public Button S2button;
    public Button S3button;
    public Button GetRoomButton;
    //public string roomName;
    public string shardName;
    private string EW;
    private string NS;
    private string EWnum;
    private string NSnum;
    private string lastRoomName;
    private string shard;
    public TerrainLoader terrLoader;
    public ObjectsLoader objectLoader;
    public MapLoader mapLoader;
    public Color ButtonActive;
    public Color ButtonInactive;

    public GameObject selector;
    private Grid grid;
    public int xPos;
    public int yPos;
    public ObjectsLoader.RoomObject currentSelection;
    private Vector3 touchStart;
    private Vector3 distance;
    public ObjectsLoader.RoomObject[] selectableObs;
    private Vector2Int touchStartTile;
    public GameObject selectMenu;
    private Vector2 selectMenuMin;
    private Vector2 selectMenuMax;
    private int lastTime;
    private float lastMenuPos;

    void Start() {
        Nbutton.onClick.AddListener(() => updateNSbutton("N"));
        Sbutton.onClick.AddListener(() => updateNSbutton("S"));
        Ebutton.onClick.AddListener(() => updateEWbutton("E"));
        Wbutton.onClick.AddListener(() => updateEWbutton("W"));
        S0button.onClick.AddListener(() => updateShardButton("shard0"));
        S1button.onClick.AddListener(() => updateShardButton("shard1"));
        S2button.onClick.AddListener(() => updateShardButton("shard2"));
        S3button.onClick.AddListener(() => updateShardButton("shard3"));
        EWslider.onValueChanged.AddListener(delegate { updateEWslider(); });
        NSslider.onValueChanged.AddListener(delegate { updateNSslider(); });
        GetRoomButton.onClick.AddListener(() => GoToRoom());
        //EW = "E";
        //NS = "N";
        EWnum = "0";
        NSnum = "0";
        Ebutton.onClick.Invoke();
        Sbutton.onClick.Invoke();
        S1button.onClick.Invoke();
        //shard = "shard1";
        //updateRoomName();
        //updateShardName();
        //roomName = ConnectionController.roomName;
        lastRoomName = ConnectionController.roomName;
        terrLoader = GameObject.Find("TileMapHolder").GetComponent<TerrainLoader>();
        objectLoader = GameObject.Find("TileMapHolder").GetComponent<ObjectsLoader>();
        mapLoader = gameObject.GetComponent<MapLoader>();
        grid = objectLoader.grid;
        touchStart = Vector3.zero;
        DetailsScrollView.SetActive(false);
        lastTime = 0;
    }
    private void Update()
    {
        Vector3 currentPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        if (currentPos.y < 0.75f && currentPos.y > 0.25f && !(currentPos.x > selectMenuMin.x && currentPos.x < selectMenuMax.x && currentPos.y > selectMenuMin.y && currentPos.y < selectMenuMax.y))
        {
            Vector3Int cellPosition = grid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            Vector2Int worldPosition = new Vector2Int(cellPosition.x, -cellPosition.y + 50);
            if (xPos != worldPosition.x || yPos != worldPosition.y)
            {
                if (worldPosition.x < 0 || worldPosition.x > 49 || worldPosition.y < 0 || worldPosition.y > 49)
                {
                    if (selector.activeSelf)
                        selector.SetActive(false);
                } else
                {
                    if (!selector.activeSelf)
                        selector.SetActive(true);
                    Vector3 newCursorPos = new Vector3((cellPosition.x * 0.16f)+0.08f, (cellPosition.y * 0.16f) + 0.08f, -3.0f);
                    selector.transform.position = newCursorPos;
                }
                xPos = worldPosition.x;
                yPos = worldPosition.y;
                updateCursorText(worldPosition);
            }
            if (Input.GetMouseButtonDown(0))
            {
                distance = Vector3.zero;
                touchStartTile = worldPosition;
                if (touchStart == Vector3.zero)
                {
                    touchStart = Camera.main.ViewportToScreenPoint(Input.mousePosition);
                }
                if (selectMenu.activeSelf)
                {
                    //selectMenu.GetComponent<SelectablesMenu>().UpdateSelectablesMenu(sendingPos, selectableObs, callback);
                    selectMenu.SetActive(false);
                }
            }
            if (Input.GetMouseButton(0))
            {
                distance += touchStart - Camera.main.ViewportToScreenPoint(Input.mousePosition);
            }
            if (Input.GetMouseButtonUp(0) && touchStartTile == worldPosition && touchStart != Vector3.zero)
            {
                
                //Debug.Log(distance);
                if (Mathf.Abs(distance.x) < 1000 && Mathf.Abs(distance.y) < 1000)
                {
                    // this is the selected tile
                    string xOut = worldPosition.x < 10 ? "0"+worldPosition.x.ToString() : worldPosition.x.ToString();
                    string yOut = worldPosition.y < 10 ? "0" + worldPosition.y.ToString() : worldPosition.y.ToString();
                    string sendingPos = xOut + yOut;
                    selectableObs = objectLoader.GetSelectablesAtTile(sendingPos);
                    if (selectableObs == null)
                    {
                        updateCurrentSelection(-1);
                        touchStart = Vector3.zero;
                        selectMenuMin = Vector2.zero;
                        selectMenuMax = Vector2.zero;
                        return;
                    }
                    if (selectableObs.Length > 1)
                    {
                        int maxStringLength = 0;
                        foreach (ObjectsLoader.RoomObject t in selectableObs)
                        {
                            maxStringLength = maxStringLength > t.type.Length ? maxStringLength : t.type.Length;
                        }
                        float stringScaler = maxStringLength * 0.03f;
                        float countScaler = selectableObs.Length * 0.08f;
                        Action<int> callback = new Action<int>(updateCurrentSelection);
                        float maxX = (currentPos.x + stringScaler) > 1 ? 1.0f : currentPos.x + stringScaler;
                        float minY = (currentPos.y - countScaler) < 0.25f ? 0.25f : currentPos.y - countScaler;
                        Vector2 maxMenuPos = new Vector2(maxX, minY+ countScaler);
                        Vector2 minMenuPos = new Vector2(maxMenuPos.x - stringScaler, minY + countScaler);
                        selectMenu.SetActive(true);
                        selectMenu.GetComponent<RectTransform>().anchorMin = minMenuPos;
                        selectMenu.GetComponent<RectTransform>().anchorMax = maxMenuPos;
                        selectMenu.GetComponent<SelectablesMenu>().UpdateSelectablesMenu(sendingPos, selectableObs, callback);
                        GameObject dropdownMenu;
                        try
                        {
                            dropdownMenu = selectMenu.transform.Find("Dropdown List").gameObject;
                        }
                        catch
                        {
                            // TODO: instantiate a backup, this is a bug when clicking out of menu and it becomes permanantly destroyed
                            Destroy(selectMenu);
                            Debug.Log("fix this ControlsPanel.cs error");
                            selectMenu = Instantiate(SelectablesDropdown, this.gameObject.transform);
                            selectMenu.SetActive(true);
                            selectMenu.GetComponent<RectTransform>().anchorMin = minMenuPos;
                            selectMenu.GetComponent<RectTransform>().anchorMax = maxMenuPos;
                            selectMenu.GetComponent<SelectablesMenu>().UpdateSelectablesMenu(sendingPos, selectableObs, callback);
                            dropdownMenu = selectMenu.transform.Find("Dropdown List").gameObject;
                        }
                        GameObject selectMenuViewport = dropdownMenu.transform.Find("SelectablesViewport").gameObject;
                        RectTransform openMenu = selectMenuViewport.GetComponent<RectTransform>();
                        Vector2 openMenuMin = Camera.main.ScreenToViewportPoint(openMenu.rect.min);
                        //Vector2 openMenuMax = Camera.main.ScreenToViewportPoint(openMenu.rect.max);
                        selectMenuMin = minMenuPos + openMenuMin;
                        selectMenuMax = maxMenuPos;
                    }
                    if (selectableObs.Length == 1)
                    {
                        selectMenuMin = Vector2.zero;
                        selectMenuMax = Vector2.zero;
                        updateCurrentSelection(0);
                    }
                }
                touchStart = Vector3.zero;
            }
        }
        if (ObjectsLoader.gameTime > lastTime)
        {
            if (currentSelection == null)
            {

            } else if (currentSelection._id != null)
            {
                if (currentSelection._id.Length > 0)
                {
                    currentSelection = objectLoader.GetObjectByID(currentSelection._id);
                    selectableObs = new ObjectsLoader.RoomObject[] { currentSelection };
                    updateCurrentSelection(0);
                }
            }
            lastTime = ObjectsLoader.gameTime;
            updateTimeText();
        }
    }
    public void UpdateLastMenuPos()
    {
        if (DetailsScrollViewBar.gameObject.activeSelf)
            lastMenuPos = DetailsScrollViewBar.value;
    }
    public void LogOut()
    {
        ScreepsLogin.cancelingLogin = true;
        GameObject[] objs = GameObject.FindGameObjectsWithTag("ConnectionController");
        foreach (GameObject obj in objs)
        {
            Destroy(obj);
        }
        SceneManager.LoadScene(0);
    }
    private string GetUserName(ObjectsLoader.RoomObject currentSelectionIn)
    {
        string selectedUser = "";
        if (objectLoader.users.Count > 0 && currentSelectionIn._user != null && objectLoader.users.ContainsKey(currentSelectionIn._user))
        {
            selectedUser = objectLoader.users[currentSelectionIn._user].username == null ? currentSelectionIn._user : objectLoader.users[currentSelectionIn._user].username;
        }
        return selectedUser;
    }
    private string GetUserName(string userIdIn)
    {
        string selectedUser = "";
        if (objectLoader.users.Count > 0 && userIdIn != null && objectLoader.users.ContainsKey(userIdIn))
        {
            selectedUser = objectLoader.users[userIdIn].username == null ? userIdIn : objectLoader.users[userIdIn].username;
        }
        return selectedUser;
    }
    private void updateCurrentSelection(int menuInput)
    {
        // need to test if this is null somehow
        selectMenu.SetActive(false);
        if (menuInput == -1 || menuInput >= selectableObs.Length)
        {
            DetailsScrollView.SetActive(false);
            currentSelection = null;
            return;
        }
        DetailsScrollView.SetActive(true);
        SelectedDetailsText.text = "";
        currentSelection = selectableObs[menuInput];
        string selectedUser = GetUserName(currentSelection);
        if (selectedUser == "")
            Debug.Log("no user found for : " + objectLoader.users.Count);
        SelectedDetailsText.text =
            "Type : " + selectableObs[menuInput].Type() + System.Environment.NewLine +
            "Id : " + selectableObs[menuInput].Id() + System.Environment.NewLine;
        
        if (selectedUser != "")
            SelectedDetailsText.text += "Owner: " + selectedUser + System.Environment.NewLine;

        if (currentSelection.hits > 0)
        {
            SelectedDetailsText.text += "Hits: " + PrintShorterInt(currentSelection.Hits()) + "/" + PrintShorterInt(currentSelection.MaxHits()) + System.Environment.NewLine;
        }

        if (selectableObs[menuInput].Type() == "creep"){
            Creep selectedCreep = objectLoader.GetSelectedCreep(currentSelection._id);
            if (selectedCreep.name != null)
                SelectedDetailsText.text += "Name: "+selectedCreep.name + System.Environment.NewLine;
            //if (selectedCreep.hits != 0)
            //    SelectedDetailsText.text += "Hits: "+selectedCreep.hits + "/" + selectedCreep.creepBody.parts.Length * 100 + System.Environment.NewLine;
            int counter = 1;
            SelectedDetailsText.text += "Body:" + "<indent=65%>" + selectedCreep.creepBody.parts.Length + " Parts" + System.Environment.NewLine;
            SelectedDetailsText.text += "<indent=15%>";
            foreach (string b in selectedCreep.creepBody.parts)
            {
                SelectedDetailsText.text += "<sprite name=\""+b+"\"> ";
                counter++;
                if (counter > 10)
                {
                    counter = 1;
                    SelectedDetailsText.text += System.Environment.NewLine + "<indent=15%>";
                }
            }
            SelectedDetailsText.text += System.Environment.NewLine;
            //SelectedDetailsText.text = selectableObs[menuInput].type + "<sprite index=2><sprite index=2><sprite index=2><sprite index=2><sprite index=2><sprite index=2><sprite index=2><sprite index=2>";
        }
        if (selectableObs[menuInput].Type() == "controller")
        {
            if (currentSelection.downgradeTime > 0)
                SelectedDetailsText.text += "Downgrade Time: " + currentSelection.downgradeTime + System.Environment.NewLine;
            if (currentSelection.reservationUser != null)
                SelectedDetailsText.text += "Reserved by: " + GetUserName(currentSelection.reservationUser) + System.Environment.NewLine;
            if (currentSelection.reservationTime > 0)
                SelectedDetailsText.text += "Reservation: " + Mathf.Abs(ObjectsLoader.gameTime - currentSelection.reservationTime).ToString() + System.Environment.NewLine;
            if (currentSelection.sign != null)
            {
                SelectedDetailsText.text += "Sign: User: " + GetUserName(currentSelection.sign.user) + System.Environment.NewLine;
                SelectedDetailsText.text += "      Time: " + currentSelection.sign.time + System.Environment.NewLine;
                SelectedDetailsText.text += "      Sign: " + currentSelection.sign.text + System.Environment.NewLine;
            }
        }
        if (currentSelection._store != null && currentSelection.storeCapacity > 0)
        {
            SelectedDetailsText.text += "<indent=0%>Store: " + PrintShorterInt(currentSelection.storeTotal) + "/" + PrintShorterInt(currentSelection.storeCapacity) + System.Environment.NewLine;
            //GameObject selectedObject = objectLoader.GetSelectedGameObject(currentSelection);
            int counter = 1;
            foreach (KeyValuePair<string,int> kvp in currentSelection.Store())
            {
                SelectedDetailsText.text += "<indent=5%><sprite name=\"" + kvp.Key + "\"><indent=20%>" + kvp.Value+ System.Environment.NewLine;
                counter++;
            }
        }
        if (DetailsScrollViewBar.gameObject.activeSelf)
            DetailsScrollViewBar.value = lastMenuPos;
    }
    private string PrintShorterInt(int intIn)
    {
        if (intIn <= 10000)
        {
            return intIn.ToString();
        }
        if (intIn > 10000 && intIn < 1000000)
        {
            int prefix = (intIn / 1000);
            int suffix = intIn % 1000;
            string suffixFixed = suffix == 0 ? "" : "." + suffix.ToString()[0].ToString();
            return prefix.ToString() + suffixFixed + "k";
        }
        if (intIn >= 1000000)
        {
            int prefix = intIn / 1000000;
            int suffix = intIn % 1000000;
            string suffixFixed = suffix == 0 ? "" : "." + suffix.ToString()[0].ToString() + suffix.ToString()[1].ToString();
            return prefix.ToString() + suffixFixed + "M";
        }
        return null;
    }
    private void updateCursorText(Vector2Int posIn) {
        string terrain = terrLoader.GetTerrainAtTile(posIn.x, posIn.y);
        cursorText.text = "Tick: "+ObjectsLoader.gameTime+ System.Environment.NewLine+"X : " +xPos.ToString()+ System.Environment.NewLine + 
            "Y : " +yPos.ToString()+ System.Environment.NewLine + "Terrain : " + terrain;
    }
    private void updateTimeText()
    {
        int startIndex = cursorText.text.IndexOf(System.Environment.NewLine);
        if (startIndex > 0)
        {
            string oldText = cursorText.text.Substring(startIndex);
            cursorText.text = "Tick: " + ObjectsLoader.gameTime + oldText;
        }
    }
    // TODO create a reverse direction for the EW slider if the w button is active
    public void updateEWslider()
    {
        EWnum = Mathf.Floor(EWslider.value).ToString();
        updateRoomName();
    }
    public void updateNSslider()
    {
        NSnum = Mathf.Floor(NSslider.value).ToString();
        updateRoomName();
    }
    public void updateNSbutton(string valueIn)
    {
        NS = valueIn;
        if (valueIn == "S") {
            Nbutton.GetComponentInChildren<Text>().color = ButtonInactive;
            Sbutton.GetComponentInChildren<Text>().color = ButtonActive;
        }
        if (valueIn == "N")
        {
            Sbutton.GetComponentInChildren<Text>().color = ButtonInactive;
            Nbutton.GetComponentInChildren<Text>().color = ButtonActive;
        }
        updateRoomName();
    }
    public void updateEWbutton(string valueIn)
    {
        EW = valueIn;
        if (valueIn == "E")
        {
            Wbutton.GetComponentInChildren<Text>().color = ButtonInactive;
            Ebutton.GetComponentInChildren<Text>().color = ButtonActive;
        }
        if (valueIn == "W")
        {
            Ebutton.GetComponentInChildren<Text>().color = ButtonInactive;
            Wbutton.GetComponentInChildren<Text>().color = ButtonActive;
        }
        updateRoomName();
    }
    private void updateRoomName()
    {
        ConnectionController.roomName = EW + EWnum + NS + NSnum;
        //roomName = ConnectionController.roomName;
        roomText.text = ConnectionController.roomName;
    }
    private void updateShardButton(string valueIn)
    {
        shard = valueIn;
        if (valueIn == "shard0")
        {
            S3button.GetComponentInChildren<Text>().color = ButtonInactive;
            S2button.GetComponentInChildren<Text>().color = ButtonInactive;
            S1button.GetComponentInChildren<Text>().color = ButtonInactive;
            S0button.GetComponentInChildren<Text>().color = ButtonActive;
        }
        if (valueIn == "shard1")
        {
            S3button.GetComponentInChildren<Text>().color = ButtonInactive;
            S2button.GetComponentInChildren<Text>().color = ButtonInactive;
            S1button.GetComponentInChildren<Text>().color = ButtonActive;
            S0button.GetComponentInChildren<Text>().color = ButtonInactive;
        }
        if (valueIn == "shard2")
        {
            S3button.GetComponentInChildren<Text>().color = ButtonInactive;
            S2button.GetComponentInChildren<Text>().color = ButtonActive;
            S1button.GetComponentInChildren<Text>().color = ButtonInactive;
            S0button.GetComponentInChildren<Text>().color = ButtonInactive;
        }
        if (valueIn == "shard3")
        {
            S3button.GetComponentInChildren<Text>().color = ButtonActive;
            S2button.GetComponentInChildren<Text>().color = ButtonInactive;
            S1button.GetComponentInChildren<Text>().color = ButtonInactive;
            S0button.GetComponentInChildren<Text>().color = ButtonInactive;
        }
        updateShardName();
    }
    private void updateShardName()
    {
        ConnectionController.shardName = shard;
        //shardText.text = shardName;
    }
    public void GoToRoom()
    {
        if (ConnectionController.roomName != lastRoomName)
        {
            currentSelection = null;
            updateCurrentSelection(-1);
            terrLoader.GetTerrainFromConnection();
            objectLoader.GetObjectsFromConnection();
            mapLoader.GetMapFromConnection();
        }
    }
}
