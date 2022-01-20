using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Unity.Jobs;

public class MazeGenerator : MonoBehaviour
{
    private int multiplier;
    private bool hasVerticalBias;
    private float animationSpeed;
    private CreateTexture textureCreator;
    // Start was called when our MazeGenerator object is called.
    //Now UI is present, so that is unnecessary.
    void Start()
    {
        //generateMaze(myMaze);
    }

    //for now, it can start anywhere inside of the maze. I might change it later.
    //Pass by reference.
    private void createStartPosition(Maze maze, ref int firstIndex, ref int secondIndex)
    {
        firstIndex = Random.Range(0, maze.getHeight());
        secondIndex = Random.Range(0, maze.getWidth());
    }
    //keeping it as an int as I might add more generation methods later.
    public void generateMaze(int choice, int givenWidth = 10, int givenHeight = 10, int anim = 3, bool biased = false)
    {
        textureCreator = gameObject.GetComponent<CreateTexture>();
        Maze maze = new Maze(givenHeight, givenWidth);
        textureCreator.createMesh(givenHeight, givenWidth);
        Debug.Log("Empty maze created.");
        //animation speed will be a global variable. Easiest for recursive calls, albeit not the best practice.
        animationSpeed = anim == 3 ? 0.001f : (anim == 2 ? 0.01f : 0.1f);
        hasVerticalBias = biased;
        int startHeight = 0, startWidth = 0;
        createStartPosition(maze, ref startHeight, ref startWidth);
        Debug.Log("Start position set: " + startHeight + ", " + startWidth);
        //Step 1. C# passes objects as references, so this should work as intended.
        Cell currentCell = maze.getCell(startHeight, startWidth);
        if(choice == 1)
        {
            StartCoroutine(recursivelyGenerate(maze, currentCell, startHeight, startWidth));
        }
        else if(choice == 2)
        {
            StartCoroutine(IterativelyGenerate(maze, currentCell, startHeight, startWidth));
        }

        
    }

    //first method. uses recursive Generation - it starts overflowing at 150x150, which is rather small.
    private IEnumerator recursivelyGenerate(Maze maze, Cell currentCell, int currentHeight, int currentWidth)
    {
        currentCell.markAsVisited();
        //bad practice (increases coupling) below, but the easiest implementation.
        int m = textureCreator.geMultiplier();
        while (currentCell.hasUnvisitedNeighbours())
        {
            int neighbourIndex;
            if (hasVerticalBias)
            {
                //75% chance it will follow the bias.
                //[inclusive, exclusive]
                int randChance = Random.Range(0, 4);
                neighbourIndex = randChance != 0 ? Random.Range(0, 2) : Random.Range(0, 4);
            }
            else
            {
                neighbourIndex = Random.Range(0, 4);
            }
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
                    textureCreator.removeWall(m, currentHeight, currentWidth, 0, maze.getHeight());
                    textureCreator.removeWall(m, newHeight, newWidth, 1, maze.getHeight());
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
                    textureCreator.removeWall(m, currentHeight, currentWidth, 1, maze.getHeight());
                    textureCreator.removeWall(m, newHeight, newWidth, 0, maze.getHeight());
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
                    textureCreator.removeWall(m, currentHeight, currentWidth, 2, maze.getHeight());
                    textureCreator.removeWall(m, newHeight, newWidth, 3, maze.getHeight());
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
                    textureCreator.removeWall(m, currentHeight, currentWidth, 3, maze.getHeight());
                    textureCreator.removeWall(m, newHeight, newWidth, 2, maze.getHeight());
                }
                else
                {
                    currentCell.setNeigbourToFalse(3);
                    continue;
                }
            }
            //Debug.Log("Moving into cell at coordinates: "+ newHeight + " " + newWidth);
            yield return new WaitForSeconds(animationSpeed);
            yield return recursivelyGenerate(maze, chosenCell, newHeight, newWidth);
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
        //I was planning to have it allocate a stack of a very large size,
        //since Stack<T> is array based - so we don't want to be resizing it during
        //runtime. However, despite documentation saying that EnsureCapacity() is 
        //baseline for Generics, I am unable to call it. Could you please give me 
        //some feedback on what I could be doing wrong?

        //cellsToExpand.EnsureCapacity(maze.getWidth()*maze.getHeight());
        //heights.EnsureCapacity(maze.getWidth()*maze.getHeight());
        //widths.EnsureCapacity(maze.getWidth()*maze.getHeight())
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
                int neighbourIndex;
                if (hasVerticalBias)
                {
                    //75% chance it will follow the bias.
                    int randChance = Random.Range(0, 4);
                    neighbourIndex = randChance != 0 ? Random.Range(0, 2) : Random.Range(0, 4);
                }
                else
                {
                    neighbourIndex = Random.Range(0, 4);
                }
                while (!currentCell.getNeighbourValue(neighbourIndex))
                {
                    neighbourIndex = Random.Range(0, 4);
                }
                //Debug.Log("Went with " + neighbourIndex);
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
            yield return new WaitForSeconds(animationSpeed);
        }
    }

}
