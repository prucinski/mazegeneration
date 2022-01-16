using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//simple class containing a maze object and it's utility functions.
public class Maze
{
    private int height;
    private int width;
    private Cell[,] cells;
    public Maze(int newHeight, int newWidth)
    {
        height = newHeight;
        width = newWidth;
        //creating a 2D array of cells inside the maze.
        cells = new Cell[height, width];
        //initializing them to be non-null.
        for(int i = 0; i < height; i++)
        {
            for(int j = 0; j < width; j++)
            {
                cells[i,j] = new Cell();
            }
        }
        setVisitedNeighbours();
    }
    public int getHeight()
    {
        return height;
    }
    public int getWidth()
    {
        return width;
    }
    public Cell getCell(int h, int w)
    {
        return cells[h, w];
    }
    //setting the borderds of the maze.
    public void setVisitedNeighbours()
    {
        for (int i = 0; i < height; i++)
        {
            //setting WEST to FALSE
            cells[i, 0].setNeigbourToFalse(2);
            //setting EAST to FALSE
            cells[i, width - 1].setNeigbourToFalse(3);
        }
        for (int j = 0; j < width; j++)
        {
            //setting NORTH to FALSE
            cells[0, j].setNeigbourToFalse(0);
            //setting SOUTH to FALSE
            cells[height - 1, j].setNeigbourToFalse(1);
        }
    }

}