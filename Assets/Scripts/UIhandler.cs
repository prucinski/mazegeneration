using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//frankly speaking it has been a while since I worked on UI, so I am making it relatively basic and code-based.
public class UIhandler : MonoBehaviour
{
    public GameObject contButton;
    public GameObject titleText;
    public GameObject signature;

    public void clickedContinue()
    {
        Debug.Log("Continue button clicked");
        contButton.SetActive(false);
        titleText.SetActive(false);
        signature.SetActive(false);
    }

}
