using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//I just wanted to put something together very quickly, so this is independent of screen sizes.
public class MoveBackground : MonoBehaviour
{
    private float speedOfBackground = 0.005f;
    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Translate(Vector3.left*speedOfBackground);
        if(gameObject.transform.position.x < -35.5f)
        {
            gameObject.transform.position = new Vector3(71, 0, 0);
        }
    }
}
