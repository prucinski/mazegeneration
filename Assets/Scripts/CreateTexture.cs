using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//The logic will be:
//Graphically, cells will be represented as 5x5 pixel blocks at minimum - multipliers of those
//for bigger mazes.
//I'll even it out for the smaller resolution - let's assume 1000 pixels.
//So for mazes up to 20x20 - cells are 50x50 pixel blocks
//So for mazes up to 40x40 - cells are 25x25 pixel blocks
//for mazes up to 50x50 - cells are 20x20 pixel blocks
//for mazes up to 66x66 - cells are 15x15 pixel blocks
//For mazes up to 100x100 - cells are 10x10 pixel blocks
//for mazes up to 200x200 - cells are 5x5 pixels.
//mazes up to 250x250 will be slighly scaled down.
//This is the basic 5x5 cell layout, on which the code is based:
//W - wall, S - space.
//W W W W W
//W S S S W
//W S S S W
//W S S S W
//W W W W W
//Wall removal will work by exchanging a corridor of W to a corridor of S.
public class CreateTexture : MonoBehaviour
{
    private Texture2D texture;
    private Sprite sprite;
    private SpriteRenderer sr;
    private int multiplier;
    private Color[] colorWhite;
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
        //resetting local scale
        sr.transform.localScale = new Vector3(1f, 1f);
        int makeSelection = Mathf.Max(labirynthHeight, labirynthWidth);
        //rules described at the start of this piece of the file. For now no other pattern
        multiplier = makeSelection <= 20 ? 10 : (makeSelection <= 40 ? 5 : (makeSelection <= 50 ? 4 : (makeSelection <= 66 ? 3 :(makeSelection<= 100 ? 2: 1))));
        Debug.Log(geMultiplier());
        //in pixels
        int meshHeight = multiplier * labirynthHeight * 5;
        int meshWidth = multiplier * labirynthWidth * 5;

        //textures are confusing when it comes to indexing. Or perhaps it's my system that's confusing?
        texture = new Texture2D(meshWidth, meshHeight, TextureFormat.RGB24, false);
        texture.filterMode = FilterMode.Point;
        //fill with white
        for (int i = 0; i < meshHeight; i++)
        {
            for (int j = 0; j < meshWidth; j++)
            {
                texture.SetPixel(j, i, Color.white);
            }
        }
        //make walls
        for (int i = 0; i < meshHeight; i++)
        {
            for (int j = 0; j < meshWidth; j++)
            {
                //Is it divisible by cell width? If yes, set wall on given height to white.
                if (j % (5 * multiplier) == 0)
                {
                    //depending on the multipliers, set walls to black.
                    for (int k = 0; k < multiplier; k++)
                    {
                        texture.SetPixel(j + k, i, Color.black);
                        texture.SetPixel(j + multiplier * 4 + k, i, Color.black);
                    }
                }
                //Set wall on given width to white if on border of a cell.
                if (i % (5 * multiplier) == 0)
                {
                    //depending on the multipliers, set walls to black.
                    for (int k = 0; k < multiplier; k++)
                    {
                        texture.SetPixel(j, i + k, Color.black);
                        texture.SetPixel(j, i + multiplier * 4 + k, Color.black);
                    }
                }
            }

        }
        //Setup for removeWall() function, at the same time:
        //Setup for SetPixels() function, that is supposedly better optimized than calling SetPixel() repeatedly
        //it doesnt matter what size of it is, it just need to be a minimum of the wall we're removing - at best 10x30 pixel blocks.
        colorWhite = new Color[300];
        for (int i = 0; i < colorWhite.Length; i++)
        {
            colorWhite[i] = Color.white;
        }
        texture.Apply(false);
        sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector3(0, 0));
        sr.sprite = sprite;
        //making sure the labirynth fits on screen.
        if (makeSelection > 200)
        {
            sr.transform.localScale = new Vector3(0.75f, 0.75f);
        }
        //center the maze on screen
        sr.transform.position = new Vector3(-sr.bounds.size.x / 2, -sr.bounds.size.y / 2, -0.5f);
        
    }
 
    public void removeWall(int multiplier, int cellHeight, int cellWidth, int direction, int mazeHeight)
    {
        //textures start in bottom left corner. My array starts at top left corner.
        //Therefore need to subtract the height
        cellHeight = mazeHeight - cellHeight - 1;
        //Remove NORTH wall
        if (direction == 0)
        {
            //Setting a block of white
            texture.SetPixels(cellWidth * 5 * multiplier + multiplier, cellHeight * 5 * multiplier + multiplier * 4, 3 * multiplier, multiplier, colorWhite);
        }

        //Remove SOUTH wall
        else if (direction == 1)
        {
            texture.SetPixels(cellWidth * 5 * multiplier + multiplier, cellHeight * 5 * multiplier, 3 * multiplier, multiplier, colorWhite);
        }
        //remove WEST
        else if (direction == 2)
        {
            texture.SetPixels(cellWidth * 5 * multiplier, cellHeight * 5 * multiplier + multiplier,multiplier, 3*multiplier, colorWhite);
        }
        //remove EAST
        else if (direction == 3)
        {
            texture.SetPixels(cellWidth * 5 * multiplier + multiplier*4, cellHeight * 5 * multiplier + multiplier, multiplier, 3 * multiplier, colorWhite);
        }
        texture.Apply(false);
    }
    //THIS IS LEGACY CODE.
    //I would have throw it out however I think it does a better way of explaining what removeWall does, as this thing here
    //is doing so step by step.
    public void removeWallLegacy(int multiplier, int cellHeight, int cellWidth, int direction, int mazeHeight)
    {


        //textures start in bottom left corner. My array starts at top left corner.
        //Therefore need to subtract the height
        cellHeight = mazeHeight - cellHeight - 1;
        //Remove NORTH wall
        if (direction == 0)
        {
            //go through the pixels on the north border, at a given height
            for (int i = cellWidth * 5 * multiplier + multiplier; i < cellWidth * 5 * multiplier + multiplier * 4; i++)
            {
                //go through width on the height
                for (int j = 0; j < multiplier; j++)
                {
                    //textures start in bottom left corner. My array starts at top left corner.
                    //Therefore need to subtract the height 
                    texture.SetPixel(i, cellHeight * 5 * multiplier + multiplier * 4 + j, Color.white);
                }
            }
        }

        //Remove SOUTH wall
        else if (direction == 1)
        {
            //go through the pixels on the north border, at a given height
            for (int i = cellWidth * 5 * multiplier + multiplier; i < cellWidth * 5 * multiplier + multiplier * 4; i++)
            {
                //go through width on the height
                for (int j = 0; j < multiplier; j++)
                {

                    texture.SetPixel(i, cellHeight * 5 * multiplier + j, Color.white);
                }
            }
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
        }
        else if (direction == 3)
        {
            //go through the pixels on the north border, at a given height
            for (int i = cellHeight * 5 * multiplier + multiplier; i < cellHeight * 5 * multiplier + multiplier * 4; i++)
            {
                //go through width on the height
                for (int j = 0; j < multiplier; j++)
                {

                    texture.SetPixel(cellWidth * 5 * multiplier + multiplier * 4 + j, i, Color.white);
                }
            }

        }
        texture.Apply(false);

    }
    //BONUS function - save a maze as a PNG so that you can print it later!
    //via https://answers.unity.com/questions/1331297/how-to-save-a-texture2d-into-a-png.html
    public void saveImage()
    {
        byte[] bytes = texture.EncodeToPNG();
        var directoryPath = Application.dataPath + "/SavedMazes/";
        if (!System.IO.Directory.Exists(directoryPath))
        {
            System.IO.Directory.CreateDirectory(directoryPath);
        }
        System.IO.File.WriteAllBytes(directoryPath + "Maze" + System.DateTimeOffset.Now.ToUnixTimeSeconds() + ".png", bytes);
        Debug.Log("Created picture at " + directoryPath + ". ");
    }

    public int geMultiplier()
    {
        return multiplier;
    }
}
