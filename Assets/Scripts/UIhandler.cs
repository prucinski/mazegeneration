using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//frankly speaking it has been a while since I worked on UI, so I am making it relatively basic and code-based.
public class UIhandler : MonoBehaviour
{
    public GameObject canvas;
    public GameObject contButton;
    public GameObject titleText;
    public GameObject signature;
    public GameObject firstScreen;
    public GameObject secondScreen;
    public GameObject thirdScreen;
    public GameObject mazeCanvas;
    public Slider widthSlider;
    public Slider heightSlider;
    private int choice = 1;
    private int width = 10;
    private int height = 10;
    public void Start()
    {
        firstScreen.SetActive(true);
        secondScreen.SetActive(false);
        thirdScreen.SetActive(false);
    }
    public void clickedGoAgain()
    {
        firstScreen.SetActive(true);
        secondScreen.SetActive(false);
        thirdScreen.SetActive(false);
        mazeCanvas.SetActive(false);
    }
    public void activateButtonAgain()
    {
        thirdScreen.SetActive(true);
    }

    public void clickedContinue()
    {
        Debug.Log("Continue button clicked");
        firstScreen.SetActive(false);
        secondScreen.SetActive(true);
    }
    public void clickedGenerate()
    {
        mazeCanvas.SetActive(true);
        Debug.Log("Generating with these parameters: " + choice + " " + width + " " + height);
        secondScreen.SetActive(false);
        MazeGenerator mg = mazeCanvas.GetComponent<MazeGenerator>();
        mg.generateMaze(choice, width, height);
        thirdScreen.SetActive(true);
    }
    public void clickedRecursive()
    {
        choice = 1;
    }
    public void clickedIterative()
    {
        choice = 2;
    }
    public void slidWidth()
    {
        width = (int)widthSlider.value;
    }
    public void slidHeight()
    {
        height = (int)heightSlider.value;
    }

}
