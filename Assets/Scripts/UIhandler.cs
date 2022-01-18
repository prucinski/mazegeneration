using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//frankly speaking it has been a while since I worked on UI, so I am making it relatively basic and code-based.
public class UIhandler : MonoBehaviour
{
    public GameObject canvas;
    public GameObject contButton;
    public GameObject titleText;
    public GameObject signature;
    public GameObject firstScreen;
    public GameObject secondScreen;
    public GameObject mazeCanvas;
    private int choice = 1;
    public void Start()
    {
        firstScreen.SetActive(true);
        secondScreen.SetActive(false);
    }

    public void clickedContinue()
    {
        Debug.Log("Continue button clicked");
        firstScreen.SetActive(false);
        secondScreen.SetActive(true);
    }
    public void clickedGenerate()
    {
        secondScreen.SetActive(false);
        MazeGenerator mg = mazeCanvas.GetComponent<MazeGenerator>();
        mg.generateMaze(choice);
    }
    public void clickedRecursive()
    {
        choice = 1;
    }
    public void clickedIterative()
    {
        choice = 2;
    }

}
