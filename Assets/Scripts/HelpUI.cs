using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpUI : MonoBehaviour
{



    public GameObject UIHELP;
    private bool isShowing = true;
    // Use this for initialization
    void Start()
    {



    }

    // Update is called once per frame
    void Update()
    {

      
        if (Input.GetButtonDown("Help"))
        {
            isShowing = !isShowing;

        }

            if (isShowing == true)
            {
                UIHELP.SetActive(true);
             //   print("UI TRUE");
            }
            if (isShowing == false)
            {
                UIHELP.SetActive(false);
              //  print("UI FALSE");
            }
        
    }
    
}
