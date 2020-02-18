using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DropDownMenuUtility : MonoBehaviour
{
    public Dropdown dropdownMenuButton;
    public int menuIndex;
    void Start() 
    {
        dropdownMenuButton.onValueChanged.AddListener(delegate {
            DropdownMenuButtonValueChangedHandler(dropdownMenuButton);
        });
    }
    void Destroy() 
    {
        dropdownMenuButton.onValueChanged.RemoveAllListeners();
    }
    
    private void DropdownMenuButtonValueChangedHandler(Dropdown target) 
    {
        Debug.Log("selected: "+target.value);

        // TODO Make this a switch case if we are supposed to load a scene on any choice
        if (target.value == menuIndex)
        {
            SceneManager.LoadScene(sceneBuildIndex: 0);
        }
    }
    
    public void SetDropdownIndex(int index) 
    {
        dropdownMenuButton.value = index;
    }
}

