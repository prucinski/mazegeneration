//Simple class describing the basic building block of the maze: a cell.
public class Cell
{
    //bolean describing which direction of the cell has a wall.
    //A cell starts off closed off.
    //in this order:         [north,south,west, east];
    private bool[] hasWall = {true, true, true, true};

    //this is irrelevant for only the cell, but it matters when it becomes a building block of a maze.
    //Directions work the same way as above. Description expanded in Maze and MazeGenerator.
    private bool[] neighbours = {true, true, true, true};
    private bool wasVisited = false;
    public Cell()
    {

    }
    //function to determine whether a cell is visitable by an algorythm.
    public bool isVisitable()
    {
        return !wasVisited;
    }
    //function to remove a wall from a cell.
    public void removeWall(int wallIndex)
    {
        hasWall[wallIndex] = false;
    }
    public void markAsVisited()
    {
        wasVisited = true;
    }
    public void setNeigbourToFalse(int neighbourIndex)
    {
        neighbours[neighbourIndex] = false;
    }
    //function used in recursive calls for expanding neighbouring nodes.
    public bool hasUnvisitedNeighbours()
    {
        if(neighbours[0] || neighbours[1] || neighbours[2] || neighbours[3])
        {
            return true;
        }
        return false;
    }
    public bool getNeighbour(int neighbourIndex)
    {
        return neighbours[neighbourIndex];
    }
}
