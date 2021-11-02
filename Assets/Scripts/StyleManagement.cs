using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
Style Management script dynamically handles the number of saved styles,
and allows easy transitioning between styles during run time.
*/

/*
Resources used to help accomplish this script:

Dropdown scripting - https://www.youtube.com/watch?v=URS9A4V_yLc

*/
public class StyleManagement : MonoBehaviour
{
    //refers to the dropdown menu which this script is attached to. 
    public Dropdown styleDropdown;
    //styleDictionary is the most important data structure that this script is design to manipulate
    //the object is all the seralized data from other scripts which is then applied to those scripts variables
    //which allows us to easily change all the data at runtime when switching styles
    private Dictionary<string, object> styleDictionary = new Dictionary<string, object>();
    private string currentStyle = "base";
    // Start is called before the first frame update
    void Start()
    {
        //grab the dropdown UI element into this variable to improve clarity within the script.
        this.styleDropdown = this.GetComponent<Dropdown>();
        //this.styleDropdown.options.Clear(); //remove any options made


        //add default entries into the styleDictionary
        //NOTE: Dictionarys do nothing when trying to add an entry that already exists.
        //object defaultSerializedData {}; //TODO: populate some data structure with all the data types and data of serialized variables from scripts

        //TODO: see how to implement TryAdd or put all default additions into try catch exception handling logic
        // this.styleDictionary.Add("base", null);
        // this.styleDictionary.Add("polished", null);
        // this.styleDictionary.Add("distinct", null);
        // this.styleDictionary.Add("custom", null);

        //create the dropdown option list from the dictionary's key(string) names
        if (this.styleDictionary.Count > 0)
        {
            foreach (KeyValuePair<string, object> entry in this.styleDictionary)
            {
                this.styleDropdown.options.Add(new Dropdown.OptionData() { text = entry.Key });
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(this.styleDictionary);
        //Debug.Log(this.styleDropdown);
    }

    //New style was selected from the dropdown, so update to the current style
    public void DropdownItemSelected()
    {
        //change currentStyle to be the key of the selected dropdown item
        Debug.Log("before: " + this.currentStyle);
        int index = this.styleDropdown.value;
        string currentOption = this.styleDropdown.options[index].text;
        if (this.styleDictionary.ContainsKey(currentOption))
        {
            this.currentStyle = currentOption;
            //grab the value in styleDictionary to update all serialized data within the game's scripts
            populateSerializedVariables(this.styleDictionary[this.currentStyle]);
        }
        else 
        {
            Debug.LogWarning("current option selected: \"" + currentOption + "\" is not in styleDictionary");
        }
        Debug.Log("after: " + this.currentStyle);
    }

    //updates all the serialized variables in other scripts to change to the current style's saved properties
    void populateSerializedVariables(object serializedData)
    {
        //TODO: figure out how to do this?
        Debug.Log("populating data: " + serializedData);
    }
}
