using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Unity.Jobs;

public class MazeGenerator : MonoBehaviour
{
    private int multiplier;
    private CreateTexture textureCreator;

    // Start is called when our MazeGenerator object is called.
    void Start()
    {
        textureCreator = gameObject.GetComponent<CreateTexture>();
        int givenHeight = 100;
        int givenWidth = 100;
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
        int choice = 2;
        createStartPosition(maze, ref startHeight, ref startWidth);
        Debug.Log("Start position set: " + startHeight + ", " + startWidth);
        //Step 1. C# passes objects as references, so this should work as intended.
        Cell currentCell = maze.getCell(startHeight, startWidth);
        if(choice == 1)
        {
            recursivelyGenerate(maze, currentCell, startHeight, startWidth);
            Debug.Log("Labirynth has been generated - recursive.");
        }
        else if(choice == 2)
        {
            StartCoroutine(IterativelyGenerate(maze, currentCell, startHeight, startWidth));
            Debug.Log("Labirynth has been generated - iterative.");
        }

       

    }

    //first method. uses recursive Generation - it starts overflowing at 150x150, which is rather small.
    private void recursivelyGenerate(Maze maze, Cell currentCell, int currentHeight, int currentWidth)
    {
        currentCell.markAsVisited();
        //bad practice below, but the easiest implementation.
        int m = textureCreator.geMultiplier();
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
            if (neighbourIndex == 0)
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
                    //textureCreator.removeWall(m, currentHeight, currentWidth, 0, maze.getHeight());
                    //textureCreator.removeWall(m, newHeight, newWidth, 1, maze.getHeight());
                }
                //if it has, update it's status. Move on to the next neighbour.
                else
                {
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
                    //textureCreator.removeWall(m, currentHeight, currentWidth, 1, maze.getHeight());
                    //textureCreator.removeWall(m, newHeight, newWidth, 0, maze.getHeight());
                }
                else
                {
                    currentCell.setNeigbourToFalse(1);
                    continue;
                }

            }
            //moving left
            else if (neighbourIndex == 2)
            {

                chosenCell = maze.getCell(currentHeight, currentWidth - 1);

                newHeight = currentHeight;
                newWidth = currentWidth - 1;
                if (chosenCell.isVisitable())
                {
                    currentCell.removeWall(2);
                    chosenCell.removeWall(3);
                    //textureCreator.removeWall(m, currentHeight, currentWidth, 2, maze.getHeight());
                    //textureCreator.removeWall(m, newHeight, newWidth, 3, maze.getHeight());
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
                    //textureCreator.removeWall(m, currentHeight, currentWidth, 3, maze.getHeight());
                    //textureCreator.removeWall(m, newHeight, newWidth, 2, maze.getHeight());
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
    }
    //generating using a stack. Removes the recursive boundary and can update on the fly.
    private IEnumerator IterativelyGenerate(Maze maze, Cell currentCell, int currentHeight, int currentWidth)
    {
        //for now this solution is the one that requires least thinking but most coding
        //I admit it's very unelegant, but I got stuck thinking how to preserve
        //the widths and simultaneous stacks were easier than stacking arrays
        Stack<Cell> cellsToExpand = new Stack<Cell>();
        Stack<int> heights = new Stack<int>();
        Stack<int> widths = new Stack<int>();
        currentCell.markAsVisited();
        cellsToExpand.Push(currentCell);
        heights.Push(currentHeight);
        widths.Push(currentWidth);
        int m = textureCreator.geMultiplier();
        while (cellsToExpand.Count != 0)
        {
            //now, pop the cell from the stack and do operations on it.
            currentCell = cellsToExpand.Pop();
            currentHeight = heights.Pop();
            currentWidth = widths.Pop();
            //Has Neighbours? If yes, back onto the stack while we do operations on you
            if (currentCell.hasUnvisitedNeighbours())
            {
                cellsToExpand.Push(currentCell);
                heights.Push(currentHeight);
                widths.Push(currentWidth);
                int neighbourIndex = Random.Range(0, 4);
                while (!currentCell.getNeighbourValue(neighbourIndex))
                {
                    neighbourIndex = Random.Range(0, 4);
                }
                Debug.Log("Went with " + neighbourIndex);
                int newHeight;
                int newWidth;
                Cell chosenCell;
                //If we're moving up:
                if (neighbourIndex == 0)
                {
                    chosenCell = maze.getCell(currentHeight - 1, currentWidth);
                    newHeight = currentHeight - 1;
                    newWidth = currentWidth;
                    if (chosenCell.isVisitable())
                    {
                        currentCell.removeWall(0);
                        chosenCell.removeWall(1);
                        textureCreator.removeWall(m, currentHeight, currentWidth, 0, maze.getHeight());
                        textureCreator.removeWall(m, newHeight, newWidth, 1, maze.getHeight());
                        chosenCell.markAsVisited();
                        cellsToExpand.Push(chosenCell);
                        heights.Push(newHeight);
                        widths.Push(newWidth);
                    }
                    //if it isnt visitable, update neighbour's status.
                    else
                    {
                        currentCell.setNeigbourToFalse(0);
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
                        textureCreator.removeWall(m, currentHeight, currentWidth, 1, maze.getHeight());
                        textureCreator.removeWall(m, newHeight, newWidth, 0, maze.getHeight());
                        chosenCell.markAsVisited();
                        cellsToExpand.Push(chosenCell);
                        heights.Push(newHeight);
                        widths.Push(newWidth);
                    }
                    else
                    {
                        currentCell.setNeigbourToFalse(1);
                    }

                }
                //moving left
                else if (neighbourIndex == 2)
                {

                    chosenCell = maze.getCell(currentHeight, currentWidth - 1);

                    newHeight = currentHeight;
                    newWidth = currentWidth - 1;
                    if (chosenCell.isVisitable())
                    {
                        currentCell.removeWall(2);
                        chosenCell.removeWall(3);
                        textureCreator.removeWall(m, currentHeight, currentWidth, 2, maze.getHeight());
                        textureCreator.removeWall(m, newHeight, newWidth, 3, maze.getHeight());
                        chosenCell.markAsVisited();
                        cellsToExpand.Push(chosenCell);
                        heights.Push(newHeight);
                        widths.Push(newWidth);
                    }
                    else
                    {
                        currentCell.setNeigbourToFalse(2);
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
                        textureCreator.removeWall(m, currentHeight, currentWidth, 3, maze.getHeight());
                        textureCreator.removeWall(m, newHeight, newWidth, 2, maze.getHeight());
                        chosenCell.markAsVisited();
                        cellsToExpand.Push(chosenCell);
                        heights.Push(newHeight);
                        widths.Push(newWidth);
                    }
                    else
                    {
                        currentCell.setNeigbourToFalse(3);
                    }
                }

            }
            yield return new WaitForSeconds(0.01f);
        }
       
    }

}
