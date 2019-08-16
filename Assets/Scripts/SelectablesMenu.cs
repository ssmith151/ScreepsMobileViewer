using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ScreepsViewer;
using TMPro;

public class SelectablesMenu : MonoBehaviour
{
    //Create a new Dropdown GameObject by going to the Hierarchy and clicking Create>UI>Dropdown. Attach this script to the Dropdown GameObject.

    //Create a List of new Dropdown options
    private List<string> SelectablesDropOptions;
    private ObjectsLoader.RoomObject[] selectableObjects;
    //This is the Dropdown
    private TMP_Dropdown selectablesDropdown;
    private TMP_Text selectablesText;

    private void Awake()
    {
        SelectablesDropOptions = new List<string>();
        selectablesText = this.transform.GetChild(0).GetComponent<TMP_Text>();
        this.gameObject.SetActive(false);
    }

    public void UpdateSelectablesMenu(string position, ObjectsLoader.RoomObject[] optionsIn, Action<int> callback)
    {
        //Initialise the Text to say the value of position
        //selectablesText.text = position;
        selectableObjects = optionsIn;
        SelectablesDropOptions.Clear();
        foreach (ObjectsLoader.RoomObject s in optionsIn)
        {
            Debug.Log(s.type);
            SelectablesDropOptions.Add(s.type);
        }
        //Fetch the Dropdown GameObject the script is attached to
        selectablesDropdown = GetComponent<TMP_Dropdown>();
        //Clear the old options of the Dropdown menu
        selectablesDropdown.ClearOptions();
        //Add the options created in the List above
        selectablesDropdown.AddOptions(SelectablesDropOptions);
        // makes fake option
        selectablesDropdown.options.Add(new TMP_Dropdown.OptionData() { text = "" });
        // selects fake option
        selectablesDropdown.value = selectablesDropdown.GetComponent<TMP_Dropdown>().options.Count - 1;
        // removes fake option
        selectablesDropdown.options.RemoveAt(selectablesDropdown.options.Count - 1);
        //Add listener for when the value of the Dropdown changes, to take action
        selectablesDropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(selectablesDropdown, callback);
        });
        selectablesDropdown.Show();
    }

    //Ouput the new value of the Dropdown into Text
    void DropdownValueChanged(TMP_Dropdown change, Action<int> callback)
    {
        SelectablesDropOptions = new List<string>();
        selectablesDropdown.onValueChanged.RemoveAllListeners();
        //m_Text.text = "New Value : " + change.value;
        if (callback != null) callback.Invoke(change.value);
    }
}
