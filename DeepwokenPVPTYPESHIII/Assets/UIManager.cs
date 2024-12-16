using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject tempUI;
    
    void Start()
    {
        tempUI.SetActive(false);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            if(!tempUI.activeSelf)
            {
            tempUI.SetActive(true);
            }
            else
            {
                tempUI.SetActive(false);
            }
        }
    }

   
}
