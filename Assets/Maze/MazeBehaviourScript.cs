using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MazeBehaviourScript : MonoBehaviour
{

    public int mazeHeight = 16;
    public int mazeWidth = 16;

    public int[,] mazeTile;
    public int[,] mazeWallHorizontal;
    public int[,] mazeWallVertical;

    public static int DIRECTION_NONE = 0;
    public static int DIRECTION_NORTH = 1;
    public static int DIRECTION_EAST = 2;
    public static int DIRECTION_SOUTH = 3;
    public static int DIRECTION_WEST = 4;

    public int startX = 5;
    public int startY = 14;

    public int finishX = 6;
    public int finishY = 0;

    public bool started = false;

    public GameObject wallPrefabHorizontal;
    public GameObject wallPrefabVertical;

    public GameObject startPrefab;
    public GameObject finishPrefab;


    public int mapNum = 3;


    void Start() {
        //InitializeMaze(2);
    }

    public void CleanUpMaze() {
        Transform[] children = GetComponentsInChildren<Transform>();
        foreach (var child in children)
        {
            if (child != transform) {
                Destroy(child.gameObject);
            }
        }
    }
    private string LoadMapFile(string txtFilePath)
    {

        string contents = File.ReadAllText(txtFilePath);
#if UNITY_STANDALONE_LINUX
        List<string> lines = new List<string>(contents.Split("\n"));
#elif UNITY_EDITOR_LINUX
        List<string> lines = new List<string>(contents.Split("\n"));
#else
        List<string> lines = new List<string>(contents.Split("\r\n"));
#endif
        Debug.Log("Lines: " + lines.Count);

        int x = 0;
        int y = 0;

        // Go through the lines, two at the time
        for (int i = 0; i < lines.Count; i+=2)
        {
            string upperLine = lines[i];
            string sideLine = lines[i + 1];

            x = 0;

            // Upper walls
            for (int j = 0; j < upperLine.Length; j+=2)
            {
                string firstChar = upperLine.Substring(j, 1);
                string secondChar = upperLine.Substring(j + 1, 1);

                if (secondChar == "-")
                {
                    // Debug.Log("Add north wall: " + x + ", " + y);
                    this.addWall(x, y, DIRECTION_NORTH);
                }

                x++;
            }

            x = 0;

            for (int j = 0; j < sideLine.Length; j+= 2 )
            {
                string firstChar = sideLine.Substring(j, 1);
                string secondChar = sideLine.Substring(j + 1, 1);

                if (firstChar == "I")
                {
                    // Debug.Log("Add west wall: " + x + ", " + y);
                    this.addWall(x, y, DIRECTION_WEST);
                }

                x++;
            }

            y++;

        }

        return contents;
    }

    private void InitializeScene() {


    }



    public void InitializeMaze(int id) {
        this.started = false;
        // Tile 1 = start, 2 = finish
        this.mazeTile = new int[mazeHeight, mazeWidth]; // Enumerable.Range(this.mazeWidth, this.mazeHeight).ToArray();
        this.mazeWallHorizontal = new int[(mazeHeight + 1),(mazeWidth + 1)];
        this.mazeWallVertical = new int[(mazeHeight + 1),(mazeWidth + 1)];

        // Empty maze and clear walls
        for (int i = 0; i < mazeHeight; i++) {
            for (int j = 0; j < mazeWidth; j++) {
                this.mazeTile[i, j] = 0;
            }
        }

         for (int i = 0; i < (mazeHeight + 1); i++) {
            for (int j = 0; j < (mazeWidth + 1); j++) {
                this.mazeWallHorizontal[i, j] = 0;
                this.mazeWallVertical[i, j] = 0;
            }
        }


        /*
       // Outer walls
       for (int i = 0; i < mazeHeight; i++) {
           this.mazeWallHorizontal[i, 0] = 1;
           this.mazeWallHorizontal[i, (mazeWidth)] = 1;
       }

       for (int j = 0; j < mazeWidth; j++) {
           this.mazeWallVertical[0, j] = 1;
           this.mazeWallVertical[(mazeHeight), j] = 1;
       }



       // For testing, add walls

       for (int j = 1; j < (mazeWidth - 1); j++) {
           this.addWall(j, 0, DIRECTION_SOUTH);
           this.addWall(j, mazeHeight - 1, DIRECTION_NORTH);
       }

       for (int i = 1; i < (mazeHeight - 1); i++) {
           this.addWall(0, i, DIRECTION_EAST);
           this.addWall(mazeWidth - 1, i, DIRECTION_WEST);
       }

       this.removeWall(3, mazeHeight - 1, DIRECTION_NORTH);

       this.addWall(3, mazeHeight - 2, DIRECTION_WEST);
       this.addWall(3, mazeHeight - 2, DIRECTION_EAST);
       this.addWall(3, mazeHeight - 3, DIRECTION_WEST);
       this.addWall(3, mazeHeight - 3, DIRECTION_EAST);
       this.addWall(3, mazeHeight - 4, DIRECTION_WEST);
       this.addWall(3, mazeHeight - 4, DIRECTION_NORTH);

       this.addWall(4, mazeHeight - 4, DIRECTION_NORTH);
       this.addWall(4, mazeHeight - 4, DIRECTION_SOUTH);
       this.addWall(5, mazeHeight - 4, DIRECTION_NORTH);
       this.addWall(5, mazeHeight - 4, DIRECTION_SOUTH);

       this.addWall(5, mazeHeight - 4, DIRECTION_EAST);

       */

        string txtFilePath = string.Format("Assets/Resources/map_{0}.txt",id);
        string mapContent = LoadMapFile(txtFilePath);

        bool resultUp = canMove(5, 4, 5, 3);
        bool resultDown = canMove(5, 4, 5, 5);

        Debug.Log("Result up: " + resultUp);
        Debug.Log("Result down: " + resultDown);


        // bool lineTest = this.isStraightLine(0, 1, 9, 1);
        bool lineTest = this.isStraightLine(1, 0, 1, 9);

        Debug.Log("Line testi: " + lineTest);

        createMazeWalls();
        createMazeStartAndFinish();

        Debug.Log("Map content: " + mapContent);

        if (!started) {
            started = true;
            InitializeScene();
        }

    }

    public void createMazeWalls() {

        for (var i = 0; i < (mazeHeight + 1); i++)
        {
            for (int j = 0 ; j < (mazeWidth + 1); j++) {

            
                if (this.mazeWallVertical[i, j] > 0) {

                    GameObject newWall = Instantiate(wallPrefabVertical, new Vector3(-5.0f + j * 1.0f, 0.0f, 5.0f - i * 1.0f), Quaternion.identity,gameObject.transform);

                }

                if (this.mazeWallHorizontal[i, j] > 0) {

                    GameObject newWall = Instantiate(wallPrefabHorizontal, new Vector3(-5.5f + j * 1.0f, 0.0f, 4.5f - i * 1.0f), Quaternion.identity,gameObject.transform);

                    float yRotation = 90.0f;
                    newWall.transform.eulerAngles = new Vector3(transform.eulerAngles.x, yRotation, transform.eulerAngles.z);

                }

            }
            
        }

    }

    public void createMazeStartAndFinish() {

        Instantiate(startPrefab, new Vector3(-5.5f + this.startX * 1.0f, 0.0f, 4.5f - this.startY * 1.0f), Quaternion.identity,gameObject.transform);

        Instantiate(finishPrefab, new Vector3(-5.5f + this.finishX * 1.0f, 0.0f, 4.5f - this.finishY * 1.0f), Quaternion.identity,gameObject.transform);

    }

    public void addWall(int x1, int y1, int direction) {

       if (direction == DIRECTION_NORTH) {
           // Debug.Log("North add well: " + x1 + ", " + y1);
            this.mazeWallVertical[y1, x1] = 1;
        }
        else if (direction == DIRECTION_SOUTH) {
            this.mazeWallVertical[y1 + 1, x1] = 1;
        }
        else if (direction == DIRECTION_EAST) {
            this.mazeWallHorizontal[y1, x1 + 1] = 1;
        }
        else if (direction == DIRECTION_WEST) {
            this.mazeWallHorizontal[y1, x1] = 1;
        }  
    }

    public void removeWall(int x1, int y1, int direction)
    {

        if (direction == DIRECTION_NORTH)
        {
            this.mazeWallVertical[y1, x1] = 0;
        }
        else if (direction == DIRECTION_SOUTH)
        {
            this.mazeWallVertical[y1 + 1, x1] = 0;
        }
        else if (direction == DIRECTION_EAST)
        {
            this.mazeWallHorizontal[y1, x1 + 1] = 0;
        }
        else if (direction == DIRECTION_WEST)
        {
            this.mazeWallHorizontal[y1, x1] = 0;
        }
    }

    public Vector3 getFinishPosition() {
        return new Vector3(finishX, finishY, 0.0f);
    }

    public Vector3 getMove(int direction) {

        if (direction == DIRECTION_NORTH) {
            return new Vector3(0, -1, 0);
        }
        else if (direction == DIRECTION_EAST) {
            return new Vector3(1, 0, 0);
        }
        else if (direction == DIRECTION_SOUTH) {
            return new Vector3(0, 1, 0);
        }
        else if (direction == DIRECTION_WEST) {
            return new Vector3(-1, 0, 0);
        }

        return new Vector3(0, 0, 0);
    }

    public int getDirection(int x1, int y1, int x2, int y2) {

        if (x1 == x2) {
            if (y1 > y2) {
                return DIRECTION_NORTH;
            }
            else if (y1 < y2) {
                return DIRECTION_SOUTH;
            }
        }
        else if (y1 == y2) {
            if (x1 > x2) {
                return DIRECTION_WEST;
            }
            else if (x1 < x2) {
                return DIRECTION_EAST;
            }
        }

        return DIRECTION_NONE;
    }

    public int getReverseDirection(int direction)
    {
        if (direction == DIRECTION_NORTH)
        {
            return DIRECTION_SOUTH;
        }
        else if (direction == DIRECTION_SOUTH)
        {
            return DIRECTION_NORTH;
        }
        else if (direction == DIRECTION_EAST)
        {
            return DIRECTION_WEST;
        }
        else if (direction == DIRECTION_WEST)
        {
            return DIRECTION_EAST;
        }

        return DIRECTION_NONE;
    }

    // Route finding and utility functions
    public bool canMove(int x1, int y1, int x2, int y2) {

        int direction = getDirection(x1, y1, x2, y2);

        if (direction == DIRECTION_NONE) {
            return false;
        }

        return canMove(x1, y1, direction);
    }

    public bool canMove(int x1, int y1, int direction) {

        bool movePossible = true;
        if (x1 < 0 || x1 >= mazeWidth || y1 < 0 || y1 >= mazeHeight) return false;

        if (direction == DIRECTION_NORTH) {
            movePossible = (this.mazeWallVertical[y1, x1] == 0);
        }
        else if (direction == DIRECTION_SOUTH) {
            movePossible = (this.mazeWallVertical[y1 + 1, x1] == 0);
        }
        else if (direction == DIRECTION_EAST) {
            movePossible = (this.mazeWallHorizontal[y1, x1 + 1] == 0);
        }
        else if (direction == DIRECTION_WEST) {
            movePossible = (this.mazeWallHorizontal[y1, x1] == 0);
        }      

        return movePossible;
    }

    public bool isStraightLine(int x1, int y1, int x2, int y2) {

        int moveX = 0;
        int moveY = 0;
        int distance = 0;
        int direction = DIRECTION_NONE;

        if (x1 == x2 && y1 > y2) {
            direction = DIRECTION_NORTH;
            moveX = 0;
            moveY = -1;
            distance = y1 - y2;
        }
        else if (x1 == x2 && y1 < y2) {
            direction = DIRECTION_SOUTH;
            moveX = 0;
            moveY = 1;
            distance = y2 - y1;
        }
        else if (x1 < x2 && y1 == y2) {
            direction = DIRECTION_EAST;
            moveX = 1;
            moveY = 0;
            distance = x2 - x1;
        }
        else if (x1 > x2 && y1 == y2) {
            direction = DIRECTION_WEST;
            moveX = -1;
            moveY = 0;
            distance = x1 - x2;
        }

        if (direction != DIRECTION_NONE) {

            int posX = x1;
            int posY = y1;

            for (int i = 0; i < distance; i++) {

                Debug.Log("Test move: " + posX + ", " + posY + " => " + direction);

                bool canMove = this.canMove(posX, posY, direction);

                Debug.Log("Move possible: " + canMove);

                if (!canMove) {
                    return false;
                }

                posX += moveX;
                posY += moveY;
            }

            return true;

        }

        return false;
    }

}
