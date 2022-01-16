using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//The logic will be:
//Graphically, cells will be represented as 5x5 pixel blocks at minimum - multipliers of those
//for bigger mazes.
//I'll even it out for the smaller resolution - let's assume 1000 pixels.
//So for mazes up to 25x25 - cells are 25x25 pixel blocks
//So for mazes up to 50x50 - cells are 20x20 pixel blocks
//for mazes up to 66x66 - cells are 15x15 pixel blocks
//For mazes up to 100x100 - cells are 10x10 pixel blocks
//for mazes up to 200x200 - cells are 5x5 pixels.
//mazes up to 250x250 have to be 4x4 pixels to fill that requirement, and therefore will use a different pattern.
public class CreateTexture : MonoBehaviour
{
    private Texture2D texture;
    private Sprite sprite;
    private SpriteRenderer sr;
    //Awake is called when game is launched
    private void Awake()
    {
   
    }
    // Start is called before the first frame update
    void Start()
    {
        // Testing out texture creation
        texture = new Texture2D(2, 2, TextureFormat.RGB24, true);
        texture.filterMode = FilterMode.Point;
        sr = gameObject.AddComponent<SpriteRenderer>() as SpriteRenderer;
        // set the pixel values
        texture.SetPixel(0, 0, Color.black);
        texture.SetPixel(1, 0, Color.white);
        texture.SetPixel(0, 1, Color.white);
        texture.SetPixel(1, 1, Color.black);
        
        // Apply all SetPixel calls
        texture.Apply();
        //create a sprite.
        sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector3(0, 0));
        sr.sprite = sprite;
        // connect texture to material of GameObject this script is attached to
        
    }
    //texture to create the initial laborithm initialization
    public void createMesh(int labirynthHeight, int labirynthWidth)
    {
        //rules described at the start of this piece of the file.
        int multiplier =  labirynthWidth<=25 ? 5 :(labirynthWidth<=50 ? 4:(labirynthWidth<=66 ? 3:(labirynthWidth<=100 ? 2:(labirynthWidth<=200? 1:0))));
        texture = new Texture2D(multiplier * labirynthHeight, multiplier * labirynthWidth, TextureFormat.RGB24, true);
        //maybe a workaround.
        Color[] black = new Color[1];
        black[0] = Color.black;
        //setting the default mesh to be black walls and white background.
        texture.SetPixels(black);
        texture.Apply(false);

        

    }
}
