using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelGenerator : MonoBehaviour
{
    public GameObject initialPiece;

    [SerializeField] public GameObject[] lowerPieces;
    [SerializeField] public GameObject[] upperPieces;

    //Later there will be 3 types of pieces for upper abd lower
    // for which probably states are needed

    #region Lower Piece variables
    private GameObject currentLowerPieceObj;

    private GameObject nextLowerPiecePrefab;

    private int currentLowerPieceIndex;
    private int nextLowerPieceIndex;

    private float nextLowerPieceXPosOffset;
    #endregion

    #region Upper Piece Variables
    private GameObject nextUpperPiecePrefab;

    private int currentUpperPieceIndex;
    private int nextUpperPieceIndex;

    private float nextUpperPieceYPosOffset;
    #endregion

    List<List<int>> arrangement2DList = new List<List<int>>();
    int currentPathEndIndexHeight;
    int lastPathEndIndexHeight;


    List<GameObject> createdObj = new List<GameObject>();
    Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
        arrangement2DList.Add(new List<int>());
        arrangement2DList.Add(new List<int>());


        lowerPieces = Resources.LoadAll<GameObject>("LowerLevelPieces");
        upperPieces = Resources.LoadAll<GameObject>("UpperLevelPieces");

        currentLowerPieceObj = initialPiece;
        //IterateGeneratePathMap();
    }



    private void Start()
    {

        AssignNextLowerPiecePrefab();

        for (int i = 0; i < 20 ; i++)
        {
            #region Lower Piece
            //Instantiate the next lower piece
            var lowerPieceObj = Instantiate(lowerPieces[nextLowerPieceIndex], transform.position, Quaternion.identity);

            //Find Transforms for entry node of the next piece and exit node of current piece
            Transform currentLowerPieceExitNodeTransform = currentLowerPieceObj.GetComponentInChildren<ExitNode>().transform;
            Transform nextLowerPieceEntryNodeTransform = lowerPieceObj.GetComponentInChildren<EntryNode>().transform;

            //Find distances between the nodes and the centre of the pieces
            float distanceCurrentLowerToNode = currentLowerPieceExitNodeTransform.position.x - currentLowerPieceObj.transform.position.x;
            float distanceNextLowerToNode = nextLowerPiecePrefab.transform.position.x - nextLowerPieceEntryNodeTransform.position.x;

            //Find distance of both combined
            nextLowerPieceXPosOffset = distanceCurrentLowerToNode + distanceNextLowerToNode;

            //Place the position of the block
            lowerPieceObj.transform.position = new Vector3
                (
                currentLowerPieceObj.transform.position.x + nextLowerPieceXPosOffset,
                currentLowerPieceObj.transform.position.y,
                0
                );


            #endregion

            //generate dangers
            AssignNextLowerPiecePrefab(lowerPieceObj);
            #region Upper Piece

            AssignNextUpperPiecePrefab();

            //Instantiate the next lower piece
            var upperPieceObj = Instantiate(upperPieces[nextUpperPieceIndex], transform.position, Quaternion.identity);

            //Find Transforms for entry node of the next piece and exit node of current piece
            Transform currentLowerPieceVerticalNodeTransform = lowerPieceObj.GetComponentInChildren<VerticalNode>().transform;
            Transform nextUpperPieceVerticalNodeTransform = upperPieceObj.GetComponentInChildren<VerticalNode>().transform;

            //Find distances between the nodes and the centre of the pieces
            float distanceCurrentLowerToVerticalNode = currentLowerPieceVerticalNodeTransform.position.y - lowerPieceObj.transform.position.y;
            float distanceNextUpperToNode = nextUpperPiecePrefab.transform.position.y - nextUpperPieceVerticalNodeTransform.position.y;

            //Find distance of both combined
            nextUpperPieceYPosOffset = distanceCurrentLowerToVerticalNode + distanceNextUpperToNode;

            //Place the position of the block
            upperPieceObj.transform.position = new Vector3
                (
                lowerPieceObj.transform.position.x,
                lowerPieceObj.transform.position.y + nextUpperPieceYPosOffset,
                0
                );


            AssignNextUpperPiecePrefab(upperPieceObj);

            #endregion

            //IterateGeneratePathMap();

            


            createdObj.Add(lowerPieceObj);
            createdObj.Add(upperPieceObj);

            foreach (var piece in createdObj)
            {
                if (piece.transform.position.x < mainCamera.transform.position.x - mainCamera.pixelWidth / 2 - 200)
                {
                    createdObj.RemoveAt(createdObj.FindIndex(x => x == piece));
                }
            }
            //check for pieces that need to be destructed (from behind)
            //update Path Map (destroy last, create new)
            //destroy them
        }

        //Debug.Log(arrangement2DList);

        //Debug.Log("Upper = " + String.Join("",
        //     new List<int>(arrangement2DList[0])
        //     .ConvertAll(i => i.ToString())
        //     .ToArray()));

        //Debug.Log("Lower = " + String.Join("",
        //    new List<int>(arrangement2DList[1])
        //    .ConvertAll(i => i.ToString())
        //    .ToArray()));

    }

    private void Update()
    {
        
    }

    private void IterateGeneratePathMap()
    {
        if (currentLowerPieceObj = initialPiece)
        {
            arrangement2DList[1].Add(2);
            currentPathEndIndexHeight = 1;
            lastPathEndIndexHeight = currentPathEndIndexHeight;
        }
        else
        {
            int direction = UnityEngine.Random.Range(0, 2);
            Debug.Log(direction);
            //direction 0 = upper
            //direction 1 = lower


            //add 2(following) to the last item

            // if last is lower but we are going up then generate lower and upper
            if (direction == 0 && arrangement2DList[1][arrangement2DList[0].Count] == 2)
            {
                arrangement2DList[0].Add(2); // assign 2 to  upper
                arrangement2DList[1].Add(1); // assign 1 to  lower
                currentPathEndIndexHeight = 0;
            }

            // If opposite then do the opposite
            else if (direction == 1 && arrangement2DList[0][arrangement2DList[0].Count] == 2)
            {
                arrangement2DList[0].Add(1); // assign 1 to Upper
                arrangement2DList[1].Add(2); // assign 2 to Lower
                currentPathEndIndexHeight = 1;
            }

            else
            {
                arrangement2DList[direction].Add(2);
                currentPathEndIndexHeight = direction;
            }
            //replace previous 2s with 1s
            int indexOfLastPathEnd = arrangement2DList[lastPathEndIndexHeight].FindAll(x => x == 2).Min();
            arrangement2DList[lastPathEndIndexHeight][indexOfLastPathEnd] = 1;

            lastPathEndIndexHeight = currentPathEndIndexHeight;

        }

    }

    private void ReturnTypeOfPieceRequired()
    {

    }

    #region Lower Piece Functions
    private void AssignCurrentLowerPiece(GameObject currentLowerPieceInput)
    {
        currentLowerPieceObj = currentLowerPieceInput;
    }

    private void AssignNextLowerPiecePrefab()
    {
        do
        {
            nextLowerPieceIndex = UnityEngine.Random.Range(0, lowerPieces.Length);
        }
        while (currentLowerPieceIndex == nextLowerPieceIndex);

        nextLowerPiecePrefab = lowerPieces[nextLowerPieceIndex];
        currentLowerPieceIndex = nextLowerPieceIndex;
    }

    private void AssignNextLowerPiecePrefab(GameObject currentLowerPieceInput)
    {
        do
        {
            nextLowerPieceIndex = UnityEngine.Random.Range(0, lowerPieces.Length);
        }
        while (currentLowerPieceIndex == nextLowerPieceIndex);

        AssignCurrentLowerPiece(currentLowerPieceInput);

        nextLowerPiecePrefab = lowerPieces[nextLowerPieceIndex];
        currentLowerPieceIndex = nextLowerPieceIndex;
    }
    #endregion

    #region Upper Piece Functions
    private void AssignCurrentUpperPiece()
    {
    }

    private void AssignNextUpperPiecePrefab()
    {
        do
        {
            nextUpperPieceIndex = UnityEngine.Random.Range(0, upperPieces.Length);
        }
        while (currentUpperPieceIndex == nextUpperPieceIndex);

        nextUpperPiecePrefab = upperPieces[nextUpperPieceIndex];

        currentUpperPieceIndex = nextUpperPieceIndex;

    }

    private void AssignNextUpperPiecePrefab(GameObject currentUpperPieceInput)
    {
        do
        {
            nextUpperPieceIndex = UnityEngine.Random.Range(0, upperPieces.Length);

        }
        while (currentUpperPieceIndex != nextUpperPieceIndex);


        nextUpperPiecePrefab = upperPieces[nextUpperPieceIndex];

        currentUpperPieceIndex = nextUpperPieceIndex;


    }
    #endregion
}
