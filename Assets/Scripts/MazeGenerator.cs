using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class MazeGenerator : MonoBehaviour
{
    // Start is called when our MazeGenerator object is called.
    void Start()
    {
        int givenHeight = 10;
        int givenWidth = 10;
        Maze myMaze = new Maze(givenHeight, givenWidth);
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

    }
    private void recursivelyGenerate(Maze maze, Cell currentCell, int currentHeight, int currentWidth)
    {
        currentCell.markAsVisited();
        int i = 0;
        Debug.Log("Hello hun");
        while (currentCell.hasUnvisitedNeighbours())
        {
            i++;
            //[inclusive, exclusive]
            int neighbourIndex = Random.Range(0, 4);
            //lazy way of finding the index of an unvisited neighbour.
            while (!currentCell.getNeighbour(neighbourIndex))
            {
                Debug.Log(neighbourIndex);
                neighbourIndex = Random.Range(0, 4);
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
                currentCell.removeWall(0);
                Cell newCell = maze.getCell(currentHeight - 1, currentWidth);
                Debug.Log("Hi");
                chosenCell = maze.getCell(currentHeight - 1, currentWidth);
                chosenCell.removeWall(1);
                //creating these values so that currentHeight is preserved when recursive call comes back and other
                //options are explored.
                newHeight = currentHeight - 1;
                newWidth = currentWidth;
            }
            //moving down
            else if (neighbourIndex == 1)
            {
                currentCell.removeWall(1);
                chosenCell = maze.getCell(currentHeight + 1, currentWidth);
                chosenCell.removeWall(0);
                newHeight = currentHeight + 1;
                newWidth = currentWidth;
            }
            //moving left
            else if(neighbourIndex == 2)
            {
                currentCell.removeWall(2);
                chosenCell = maze.getCell(currentHeight, currentWidth -1);
                chosenCell.removeWall(3);
                newHeight = currentHeight;
                newWidth = currentWidth - 1;
            }
            //moving right
            else
            {
                currentCell.removeWall(3);
                chosenCell = maze.getCell(currentHeight, currentWidth + 1);
                chosenCell.removeWall(2);
                newHeight = currentHeight;
                newWidth = currentWidth + 1;
            }
            Debug.Log("Moving into cell at coordinates: "+ newHeight + " " + newWidth);
            recursivelyGenerate(maze, chosenCell, newHeight, newWidth);
        }
        //Labirynth generated! Wohoo!
        Debug.Log("Labirynth has been generated.");
    }
}
