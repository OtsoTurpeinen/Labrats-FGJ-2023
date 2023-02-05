using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using Unity.VisualScripting;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

static class MyExtensions
{
public static void Shuffle<T>(this IList<T> list)
{
    int n = list.Count;
    while (n > 1)
    {
    n--;
    int k = Random.Range(0,n + 1);
    T value = list[k];
    list[k] = list[n];
    list[n] = value;
    }
}
}

public class RatBehaviourScript : MonoBehaviour
{
    public float x; // Grid coordinates
    public float y;

    public float posX; // GameObject locations
    public float posY; 

    public float targetX;
    public float targetY;

    public float direction;

    public float updateDelta = 0.0f;
    private float waitTime = 0.25f;

    public bool moving = false;
    public bool raceFinished = false;
    public int comingFrom = 0;
    public int currentNodeId = -1;

    public int playerId = 0; 

    public int[,] mazeMemory;

    GameObject mazeObject;
    MazeBehaviourScript mazeScript;

    public RatGenetics genetics;

    Animator animator;

    public struct DirectionScan
    {
        public int junctions;
        public int deadend;
        public int distance;
        public int direction;
        public Vector3 firstJunction;
    };

    public struct RouteNode
    {
        public int id;
        public int prevId;
        public Vector3 nodePoint;
        public bool visited;
    }

    private List<RouteNode> routes;
    private int routeNodeId = 0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        updateDelta += Time.deltaTime;

        // Check if we have reached beyond 2 seconds.
        // Subtracting two is more accurate over time than resetting to zero.
        if (updateDelta > waitTime)
        {
            updateDelta -= waitTime;

            if (!this.raceFinished)
            {
                RatUpdate();
            }

        }
    }

    public void InitializeRat(float x, float y, MazeBehaviourScript mazeScript, int mazeWidth, int mazeHeight,int playerId, RatGenetics genetics,Color color) {
        this.genetics = new RatGenetics(genetics.current_genes);
        SpriteRenderer spr = GetComponentInChildren<SpriteRenderer>() as SpriteRenderer;
        spr.color = color;
        this.playerId = playerId;
        this.x = x;
        this.y = y;
        this.posX = x;
        this.posY = y;

        this.targetX = x;
        this.targetY = y;
        this.direction = MazeBehaviourScript.DIRECTION_NORTH;
        this.animator = GetComponent<Animator>();

        this.routes = new List<RouteNode>();

        this.mazeMemory = new int[mazeHeight, mazeWidth];
        for (int i = 0; i < mazeHeight; i++)
        {
            for (int j = 0; j < mazeWidth; j++)
            {
                this.mazeMemory[i, j] = 0;
            }
        }

        // this.mazeObject = mazeObject;
        this.mazeScript = mazeScript;

        // First node
        AddNewNode(new Vector3(this.x, this.y, 0.0f), -1);
        
    }

    public void MoveRat(float x, float y) {

        this.x += x;
        this.y += y;

        //-5.0f + x * 1.0f, 4.5f - y * 1.0f

        this.posX = -5.0f + this.x * 1.0f;
        this.posY = 4.5f - this.y * 1.0f;
         

        // Debug.Log("Add x: " + x + ", y: " + y + " =" + (int)this.x + "," + (int)this.y);

        this.gameObject.transform.position = new Vector3(posX, 0.0f, posY); //  this.gameObject.transform.position.z);

    }

    public void RatUpdate() {
        if (this.raceFinished) return;
        if (this.mazeScript == null) return;
        if (!this.mazeScript.started) return;

        // If rat needs to move, it does so but doesnt think while doing so
        RatMovement();

        // Else if the rat is stopped, it thinks about the best approach or routes

        if (this.moving == false)
        {

            RatDecision();

        }

    }


    public void RatMovement() {

        // If rat is not turned to the direction is needs to go, do that first


        // Else move towards the target x and y coordinates

        if ((int)this.targetX != (int)this.x || (int)this.targetY != (int)this.y) {

            this.moving = true;

            int moveDirection = this.mazeScript.getDirection((int)this.x, (int)this.y, (int)this.targetX, (int)this.targetY);

            // Move if we have same direction
            if (this.direction == moveDirection) {
                waitTime = 1.0f / genetics.GetGeneValue(GeneticType.FORWARD_SPEED);

               //  Debug.Log("Moving...");

                Vector3 newMove = this.mazeScript.getMove(moveDirection);

                // Save the direction we are coming from
                this.comingFrom = this.mazeScript.getReverseDirection(moveDirection);

                this.MoveRat(newMove.x, newMove.y);

                // Are we there yet?
                if ((int)this.x == (int)this.targetX && (int)this.y == (int)this.targetY)
                {
                    // Movement is over
                    this.moving = false;

                    // Debug.Log("Moving false!");

                }
                else
                {
                    // Debug.Log("Move diff: " + (int)this.x + " vs " + (int)this.targetX + ", " + (int)this.y + " vs " + (int)this.targetY);
                }

            }
            else {
                // Instant turning for now, for one second delay
                this.animator.SetInteger("direction",moveDirection);
                waitTime = 1.0f / genetics.GetGeneValue(GeneticType.TURN_RATE);
                this.direction = moveDirection;
            }


        }

    }

    public void RatDecision() {
        // 1. Do we see the ending point? If so, go there. Or if we are already there!
        Vector3 finishPos = mazeScript.getFinishPosition();

        // Are we finished?

        if ((int)this.x == (int)finishPos.x && (int)this.y == (int)finishPos.y) {


            this.raceFinished = true;

            Debug.Log("RACE FINISHED!!!");
            GameController.Instance.RatReachedMazeEnd(playerId,gameObject);
            Debug.Log("Informed game controller!!");
        }

        bool canSeeFinish = mazeScript.isStraightLine((int)this.x, (int)this.y, (int)finishPos.x, (int)finishPos.y);

        // Debug.Log("Can see finish: " + canSeeFinish);

        if (canSeeFinish) {
            this.targetX = finishPos.x;
            this.targetY = finishPos.y;
        }
        // 2. If not, can we move forward? If not, backtrace to the previous point of alternative route
        else
        {
            // Update my current location route node
            VisitNode(new Vector3(this.x, this.y, 0.0f));

            // Find new directions
            FindDirection(0);

            // List nodes
            ListNodes();

        }
        
        // 3. If we can move forward or are in crossroads, decide where to go :)

        // 4. Bonus trait-based decision making if needed


    }

    public void FindDirection(int comingFrom)
    {
        DirectionScan winningDirection;

        winningDirection.distance = 999;
        winningDirection.junctions = 0;
        winningDirection.deadend = 0;
        winningDirection.direction = MazeBehaviourScript.DIRECTION_NONE;
        winningDirection.firstJunction = new Vector3(0.0f, 0.0f, 0.0f);

        // Loop through the directions
        List<int> directions = new List<int> {
            1,2,3,4
        };
        directions.Shuffle();
        foreach (int i in directions)
        {
            int junctions = 0;
            int deadend = 0;
            int distance = 0;
            Vector3 closestJunction = new Vector3(-1.0f, -1.0f, 0.0f);

            int direction = i;
            int thisNodeId = currentNodeId;

            if (direction != comingFrom)
            {

                int scanX = (int)this.x;
                int scanY = (int)this.y;
                bool scanMoving = true;
                int scanComingFrom = this.mazeScript.getReverseDirection(direction);

                while (scanMoving == true && distance < 40)
                {

                    Vector3 moveBonus = this.mazeScript.getMove(direction);
                    bool canMove = this.mazeScript.canMove(scanX, scanY, scanX + (int)moveBonus.x, scanY + (int)moveBonus.y);

                    // can we move to this direction?
                    if (canMove)
                    {

                        // Increase distance and move scan point
                        distance++;
                        scanX += (int)moveBonus.x;
                        scanY += (int)moveBonus.y;

                        int ways = 0;
                        bool canGoForward = false;

                        // Get ways from this point
                        for (int j = 1; j < 5; j++)
                        {

                            int scanDirection = j;

                            if (scanDirection != scanComingFrom)
                            {

                                bool scanResult = this.mazeScript.canMove(scanX, scanY, scanDirection);

                                if (scanResult)
                                {
                                    ways++;

                                    // Check the main way
                                    if (scanDirection == direction)
                                    {
                                        canGoForward = true;
                                    }
                                }

                            }

                        }

                        // First junction is saved
                        // Debug.Log("Scan: " + scanX + ", " + scanY + ", Ways: " + ways);

                        if (ways > 1) // more ways then one
                        {
                            junctions++;

                            if (closestJunction.x == -1.0f)
                            {
                                closestJunction = new Vector3(scanX, scanY, 0.0f);
                            }

                        }
                        else if (ways == 1 && !canGoForward) // a turn
                        {
                            junctions++;

                            if (closestJunction.x == -1.0f)
                            {
                                closestJunction = new Vector3(scanX, scanY, 0.0f);
                            }
                        }
                        else if (ways == 0) // Deadend
                        {
                            deadend = 1;
                            scanMoving = false;
                        }

                    } // cant move
                    else
                    {
                        scanMoving = false;
                    }

                } // while move

                bool thisWins = false;

                if (deadend == 0)
                {  // We are not going into deadends... yet

                    if (junctions > winningDirection.junctions)
                    {
                        thisWins = true;
                    }
                    else if (junctions == winningDirection.junctions && distance < winningDirection.distance)
                    {
                        thisWins = true;
                    }
                }

                // Add closest junctions into new nodes

                RouteNode oldNode = getNodeByLocation(closestJunction);
                if (oldNode.id <= 0) {

                    int newId = AddNewNode(closestJunction, thisNodeId);

                    Debug.Log("Add new node: " + newId + ", X: " + closestJunction.x + ", Y: " + closestJunction.y);
                
                }
                else // Or check if old node point has been visited
                {
                    if (oldNode.visited) {
                        thisWins = false;
                    }

                }

                if (thisWins)
                {
                    winningDirection.direction = direction;
                    winningDirection.firstJunction = closestJunction;
                    winningDirection.distance = distance;
                    winningDirection.deadend = deadend;
                    winningDirection.junctions = junctions;

                    Debug.Log("Direction " + direction + " wins for now!");
                    Debug.Log("Distance: " + distance + ", Deadend: " + deadend + ", Junctions: " + junctions);

                }

            }

        } // Direction loop

        // Winner direction and set target
        if (winningDirection.distance < 999 && winningDirection.junctions > 0)
        {
            Debug.Log("Last winner: " + winningDirection.direction);

            this.targetX = winningDirection.firstJunction.x;
            this.targetY = winningDirection.firstJunction.y;

            Debug.Log("Movement to : " + (int)this.targetX + ", " + (int)this.targetY);

        }
        else // Traceback out of here, if there is no routes possible (visited ruled out)
        {
            Debug.Log("Traceback! CurrentId: " + currentNodeId);

            RouteNode thisNode = getNodeById(currentNodeId);
            int prevId = thisNode.prevId;
            RouteNode prevNode = getNodeById(prevId);

            this.targetX = prevNode.nodePoint.x;
            this.targetY = prevNode.nodePoint.y;

            Debug.Log("Traceback to : " + (int)this.targetX + ", " + (int)this.targetY);

        }

    }

    // Route nodes functions
    private int AddNewNode(Vector3 location, int prevId)
    {
        RouteNode newNode = new RouteNode();
        newNode.prevId = prevId;
        routeNodeId++;
        newNode.id = routeNodeId;
        newNode.nodePoint = new Vector3(location.x, location.y, location.z);

        routes.Add(newNode);

        return newNode.id;

    }

    private RouteNode getNodeByLocation(Vector3 location)
    {
        for (int i = 0; i < routes.Count; i++) { 

            if ((int)routes[i].nodePoint.x == (int)location.x && (int)routes[i].nodePoint.y == (int)location.y)
            {

                return routes[i];

            }
        }

        return new RouteNode();
    }

    private void VisitNode(Vector3 location)
    {

        RouteNode thisNode = getNodeByLocation(location);

        if (thisNode.id > 0)
        {
            Debug.Log("CURRENT NODE: " + thisNode.id);

            thisNode.visited = true;
            currentNodeId = thisNode.id;

            UpdateNodePoint(thisNode.id, thisNode);

        }

    }

    private void UpdateNodePoint(int id, RouteNode newRouteNode)
    {
        for (int i = 0; i < routes.Count; i++)
        {

            if ((int)routes[i].id == (int)id)
            {

                routes[i] = newRouteNode;

            }
        }
    }

    private RouteNode getNodeById(int id)
    {
        for (int i = 0; i < routes.Count; i++)
        {
            if ((int)routes[i].id == id)
            {
                return routes[i];
            }
        }
        return new RouteNode();
    }
    private void ListNodes()
    {
        Debug.Log("Listing Nodes:");

        foreach (RouteNode node in this.routes)
        {
            Debug.Log("Id: " + node.id + ", Prev: " + node.prevId + ", Visited: " + node.visited + ", Pos: " + node.nodePoint.x + ", " + node.nodePoint.y);
        }
    }



}
