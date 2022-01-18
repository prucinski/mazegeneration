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
    private int multiplier;
    //Awake is called when game is launched
    private void Awake()
    {
        sr = gameObject.AddComponent<SpriteRenderer>() as SpriteRenderer;
    }
    // Start is called before the first frame update
    void Start()
    {  
    }
    //texture to create the initial laborithm initialization
    public void createMesh(int labirynthHeight, int labirynthWidth)
    {
        //rules described at the start of this piece of the file. For now no other pattern
        multiplier =  labirynthWidth<=25 ? 5 :(labirynthWidth<=50 ? 4:(labirynthWidth<=66 ? 3:(labirynthWidth<=100 ? 2:(labirynthWidth<=200? 1:1))));
        Debug.Log(geMultiplier());
        //in pixels
        int meshHeight = multiplier * labirynthHeight*5;
        int meshWidth = multiplier * labirynthWidth*5;

        //textures are confusing when it comes to indexing. Or perhaps it's my system that's confusing?
        texture = new Texture2D(meshWidth, meshHeight, TextureFormat.RGB24, true);
        texture.filterMode = FilterMode.Point;
        //fill with white
        for (int i = 0; i <meshHeight; i++)
        {
            for(int j = 0; j < meshWidth; j++)
            {
                texture.SetPixel(j, i, Color.white);
            }
        }
        //make walls
        for(int i = 0; i<meshHeight; i++)
        {
            for(int j = 0; j<meshWidth; j++)
            {
                //Is it divisible by cell width? If yes, set wall on given height to white.
                if (j % (5*multiplier) == 0)
                {
                    //depending on the multipliers, set walls to black.
                    for(int k = 0; k<multiplier; k++)
                    {
                        texture.SetPixel(j+k, i, Color.black);
                        texture.SetPixel(j + multiplier * 4 + k, i, Color.black);
                    }  
                }
                //Set wall on given width to white if on border of a cell.
                if (i % (5 * multiplier) == 0)
                {
                    //depending on the multipliers, set walls to black.
                    for (int k = 0; k < multiplier; k++)
                    {
                        texture.SetPixel(j, i+k, Color.black);
                        texture.SetPixel(j, i+multiplier * 4 + k, Color.black);
                    }
                }
            }

        }
        texture.Apply(false);
        sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector3(0, 0));
        sr.sprite = sprite;
        //center the maze on screen
        sr.transform.position = new Vector3(-sr.bounds.size.x / 2, -sr.bounds.size.y / 2);
    }
    //seems like there's no quick methods
    private void setPixelsHelper()
    {

    }
    public void removeWall(int multiplier, int cellHeight, int cellWidth, int direction, int mazeHeight)
    {
        //textures start in bottom left corner. My array starts at top left corner.
        //Therefore need to subtract the height
        cellHeight = mazeHeight - cellHeight -1;
        //Remove NORTH wall
        if(direction == 0)
        {
            //go through the pixels on the north border, at a given height
            for (int i = cellWidth * 5 * multiplier + multiplier; i < cellWidth * 5 * multiplier + multiplier * 4; i++)
            {
                //go through width on the height
                for (int j = 0; j < multiplier; j++)
                {
                    //textures start in bottom left corner. My array starts at top left corner.
                    //Therefore need to subtract the height 
                    texture.SetPixel(i, cellHeight * 5 * multiplier + multiplier*4 + j, Color.white);
                }
            }
            texture.Apply(false);
            sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector3(0, 0));
            sr.sprite = sprite;
        }

        //Remove SOUTH wall
        else if (direction == 1)
        {
            //go through the pixels on the north border, at a given height
            for (int i = cellWidth *5* multiplier + multiplier; i<cellWidth*5*multiplier +multiplier*4; i++)
            {
                //go through width on the height
                for (int j = 0; j < multiplier; j++)
                {
                    
                    texture.SetPixel(i, cellHeight*5*multiplier + j, Color.white);
                }
            }
            texture.Apply(false);
            sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector3(0, 0));
            sr.sprite = sprite;
        }
        //remove WEST
        else if (direction == 2)
        {
            //go through the pixels on the north border, at a given height
            for (int i = cellHeight * 5 * multiplier + multiplier; i < cellHeight * 5 * multiplier + multiplier * 4; i++)
            {
                //go through width on the height
                for (int j = 0; j < multiplier; j++)
                {

                    texture.SetPixel(cellWidth * 5 * multiplier + j, i, Color.white);
                }
            }
            texture.Apply(false);
            sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector3(0, 0));
            sr.sprite = sprite;
        }
        else if (direction == 3)
        {
            //go through the pixels on the north border, at a given height
            for (int i = cellHeight * 5 * multiplier + multiplier; i < cellHeight * 5 * multiplier + multiplier * 4; i++)
            {
                //go through width on the height
                for (int j = 0; j < multiplier; j++)
                {

                    texture.SetPixel(cellWidth * 5 * multiplier +multiplier*4 + j, i, Color.white);
                }
            }

        }
        texture.Apply(false);
        sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector3(0, 0));
        sr.sprite = sprite;

    }
    public void finishRender()
    {

    }

    public int geMultiplier()
    {
        return multiplier;
    }
}
