using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class MazeGenerator : MonoBehaviour
{
    private CreateTexture textureCreator;
    // Start is called when our MazeGenerator object is called.
    void Start()
    {
        textureCreator = gameObject.GetComponent<CreateTexture>();
        int givenHeight = 150;
        int givenWidth = 150;
        Maze myMaze = new Maze(givenHeight, givenWidth);
        textureCreator.createMesh(givenHeight, givenWidth);
        Debug.Log("Empty maze created.");
        generateMaze(myMaze);
    }

    //for now, it can start anywhere inside of the maze. I might change it later.
    //Pass by reference.
    private void createStartPosition(Maze maze, ref int firstIndex, ref int secondIndex)
    {
        firstIndex = Random.Range(0, maze.getHeight());
        secondIndex = Random.Range(0, maze.getWidth());
    }
    public void generateMaze(Maze maze)
    {
        int startHeight = 0, startWidth = 0;
        createStartPosition(maze, ref startHeight, ref startWidth);
        Debug.Log("Start position set: " + startHeight + ", " + startWidth);
        //Step 1. C# passes objects as references, so this should work as intended.
        Cell currentCell = maze.getCell(startHeight, startWidth);
        recursivelyGenerate(maze, currentCell, startHeight, startWidth);
        Debug.Log("Labirynth has been generated.");

    }

    //first method. uses recursive Generation - it starts overflowing at 150x150, which is rather small.
    private void recursivelyGenerate(Maze maze, Cell currentCell, int currentHeight, int currentWidth)
    {
        currentCell.markAsVisited();
        while (currentCell.hasUnvisitedNeighbours())
        {

            //[inclusive, exclusive]
            int neighbourIndex = Random.Range(0, 4);
            //Debug.Log("Exploring given cells unvisited neighbours: looking at cell: " + neighbourIndex);
            //lazy way of finding the index of an unvisited neighbour.
            while (!currentCell.getNeighbourValue(neighbourIndex))
            {
                neighbourIndex = Random.Range(0, 4);
                //Debug.Log("Oops - this cell is unaccessible. Looking at cell: " + neighbourIndex);
            }
            //Now, the hard and tedious part. Removing walls between the cells.
            //Probably there's a more elegant solution to this than what I'm doing here,
            //but I came up with this all by myself so I'm pretty proud.
            int newHeight;
            int newWidth;
            Cell chosenCell;
            //If we're moving up:
            if(neighbourIndex == 0)
            {
                chosenCell = maze.getCell(currentHeight - 1, currentWidth);
                //creating these values so that currentHeight is preserved when recursive call comes back and other
                //options are explored.
                newHeight = currentHeight - 1;
                newWidth = currentWidth;
                //Checking whether the chosen cell has been visited at some other point in the recursion
                if (chosenCell.isVisitable())
                {
                    currentCell.removeWall(0);
                    chosenCell.removeWall(1);
                }
                //if it has, update it's status. Move on to the next neighbour.
                else{
                    currentCell.setNeigbourToFalse(0);
                    continue;
                }

            }
            //moving down
            else if (neighbourIndex == 1)
            {
                chosenCell = maze.getCell(currentHeight + 1, currentWidth);
                newHeight = currentHeight + 1;
                newWidth = currentWidth;
                if (chosenCell.isVisitable())
                {
                    currentCell.removeWall(1);
                    chosenCell.removeWall(0);
                }
                else
                {
                    currentCell.setNeigbourToFalse(1);
                    continue;
                }

            }
            //moving left
            else if(neighbourIndex == 2)
            {

                chosenCell = maze.getCell(currentHeight, currentWidth -1);

                newHeight = currentHeight;
                newWidth = currentWidth - 1;
                if (chosenCell.isVisitable())
                {
                    currentCell.removeWall(2);
                    chosenCell.removeWall(3);
                }
                else
                {
                    currentCell.setNeigbourToFalse(2);
                    continue;
                }
            }
            //moving right
            else
            {
                chosenCell = maze.getCell(currentHeight, currentWidth + 1);
                newHeight = currentHeight;
                newWidth = currentWidth + 1;
                if (chosenCell.isVisitable())
                {
                    currentCell.removeWall(3);
                    chosenCell.removeWall(2);
                }
                else
                {
                    currentCell.setNeigbourToFalse(3);
                    continue;
                }
            }
            //Debug.Log("Moving into cell at coordinates: "+ newHeight + " " + newWidth);
            recursivelyGenerate(maze, chosenCell, newHeight, newWidth);
        }
        //Debug.Log("Cell at coordinates: " + currentHeight + " " + currentWidth + "has no unvisited neighbours left. Going back up.");
        //Labirynth generated! Wohoo!
        
    }
}
