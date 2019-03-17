using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class GameManager : MonoBehaviour
{
    #region SINGLETON
    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
            instance = this;
    }
    #endregion

    #region VARIABLES
    public GameObject[] candyPrefabs;
    public CandyArray candyArray;

    private Vector2 bottomRight = new Vector2(-2.37f, -4.27f);
    private Vector2 candySize = new Vector2(0.7f, 0.7f);
    private Vector2[] spawnPosition;

    private GameState gameState;
    private GameObject hitGO = null;    

    private IEnumerator checkPotentialMatchesCoroutine;
    private List<GameObject> potentialMatches;
    #endregion

    #region PUBLIC METHODS
    /// <summary>
    /// Initialize candies in the scene
    /// </summary>
    public void InitCandies()
    {
        if(candyArray!=null)
        {
            DestroyAllCandy();
        }
        candyArray = new CandyArray();
        spawnPosition = new Vector2[GameVariables.Columns];

        for (int row = 0; row < GameVariables.Rows; row++)
        {
            for (int col = 0; col < GameVariables.Columns; col++)
            {
                GameObject newCandy = GetRandomCandy();
                 //Checking, previous two horizontal candies are of same colour.
                while(col >= 2 && candyArray[row,col - 1].GetComponent<Candy>().IsOfSameColour(newCandy.GetComponent<Candy>())
                    && candyArray[row, col - 2].GetComponent<Candy>().IsOfSameColour(newCandy.GetComponent<Candy>()))
                {
                    newCandy = GetRandomCandy();
                }

                //Checking, previous two vertical candies are of same colour.
                while (row >= 2 && candyArray[row - 1, col].GetComponent<Candy>().IsOfSameColour(newCandy.GetComponent<Candy>())
                    && candyArray[row - 2, col].GetComponent<Candy>().IsOfSameColour(newCandy.GetComponent<Candy>()))
                {
                    newCandy = GetRandomCandy();
                }

                CreateNewCandy(row, col, newCandy);
            }
        }
        SetSpawnPosition();
    }
    #endregion

    #region PRIVATE METHODS
    private void Start()
    {
        gameState = GameState.None;
        InitCandies();
        StartCheckForPotentialMatches();
    }

    /// <summary>
    /// Delete or destroy tall the candies present in the scene.
    /// </summary>
    private void DestroyAllCandy()
    {
        for (int row = 0; row < GameVariables.Rows; row++)
        {
            for (int col = 0; col < GameVariables.Columns; col++)
            {
                Destroy(candyArray[row, col]);
            }
        }
    }

    /// <summary>
    /// Create a new candy and place it into the candyArray
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <param name="candy"></param>
    private void CreateNewCandy(int row, int col, GameObject candy)
    {
        Vector2 offSetVector = new Vector2(col * candySize.x, row * candySize.y);
        GameObject newCandy = Instantiate(candy, bottomRight + offSetVector, Quaternion.identity);
        newCandy.GetComponent<Candy>().Initialize(row, col, newCandy.GetComponent<Candy>().candyColour);
        candyArray[row, col] = newCandy;
    }

    /// <summary>
    /// Set spawn positon for new candies.
    /// </summary>
    private void SetSpawnPosition()
    {
        for (int col = 0; col < GameVariables.Columns; col++)
        {
            Vector2 offSetVector = new Vector2(col * candySize.x, GameVariables.Rows * candySize.y);
            spawnPosition[col] = bottomRight + offSetVector;
        }
    }

    /// <summary>
    /// Function returns a random candy gameobject.
    /// </summary>
    /// <returns></returns>
    private GameObject GetRandomCandy()
    {
        return candyPrefabs[UnityEngine.Random.Range(0, candyPrefabs.Length)];
    }

    private IEnumerator CheckPotentialMatches()
    {
        yield return new WaitForSeconds(GameVariables.WaitBeforematchCheck);
        potentialMatches = MatchChecker.GetPotentialMatches(candyArray).ToList<GameObject>();
    }

    private void StartCheckForPotentialMatches()
    {
        StopCheckPotentialMatches();
        checkPotentialMatchesCoroutine = CheckPotentialMatches();
        StartCoroutine(checkPotentialMatchesCoroutine);
    }

    private void StopCheckPotentialMatches()
    {
        if(checkPotentialMatchesCoroutine!=null)
        {
            StopCoroutine(checkPotentialMatchesCoroutine);
        }
    }

    private void FixSortingLayer(GameObject hitGO1,GameObject hitGO2)
    {
        SpriteRenderer sp1 = hitGO1.GetComponent<SpriteRenderer>();
        SpriteRenderer sp2 = hitGO2.GetComponent<SpriteRenderer>();

        if(sp1 != null && sp2 != null)
        {
            if(sp1.sortingOrder <= sp2.sortingOrder)
            {
                sp1.sortingOrder = 1;
                sp2.sortingOrder = 0;
            }
        }
    }

    /// <summary>
    /// Function resposible of creating new candies after the matches of the candies of same colour
    /// </summary>
    /// <param name="missingCandies"></param>
    /// <returns></returns>
    private AlteredCandyInfo CreateNewCandyinSpecificColumn(IEnumerable<int> missingCandies)
    {
        AlteredCandyInfo newCandyInfo = new AlteredCandyInfo();

        foreach (int col in missingCandies)
        {
            var candyInfos = candyArray.GetEmptyItemsOnColumn(col);

            foreach (var item in candyInfos)
            {
                GameObject tempGO = GetRandomCandy();
                if(tempGO!=null)
                {
                    GameObject newCandy = Instantiate(tempGO, spawnPosition[col], Quaternion.identity);
                    newCandy.GetComponent<Candy>().Initialize(item.Row, item.Column, newCandy.GetComponent<Candy>().candyColour);

                    if(GameVariables.Rows - item.Row > newCandyInfo.MaxDistance)
                    {
                        newCandyInfo.MaxDistance = GameVariables.Rows - item.Row;
                    }

                    candyArray[item.Row, item.Column] = newCandy;
                    newCandyInfo.AddNewCandies(newCandy);
                }
            }
        }
        return newCandyInfo;
    }
    
    private void MoveCandy(IEnumerable<GameObject> moveGO, int distance)
    {
        foreach (var item in moveGO)
        {
            Vector3 _position = bottomRight + new Vector2(item.GetComponent<Candy>().column * candySize.x, item.GetComponent<Candy>().row * candySize.y);
            item.transform.position = _position;
        }
    }

    private IEnumerator FindMatchesAndCollapse(RaycastHit2D rayCast)
    {
        GameObject go = rayCast.collider.gameObject;
        candyArray.SwapCandiesGO(hitGO, go);

        Vector3 tempVector = hitGO.transform.position;
        hitGO.transform.position = rayCast.transform.position;
        go.transform.position = tempVector;
        yield return new WaitForSeconds(1f);

        var hitGOMatchesInfo = candyArray.GetMatches(hitGO);
        var goMatchesInfo = candyArray.GetMatches(go);

        IEnumerable<GameObject> totalMatches = hitGOMatchesInfo.GetMatchedCandy.Union(goMatchesInfo.GetMatchedCandy).Distinct();
        int tempCount = totalMatches.Count();
        if (tempCount >= GameVariables.MinimumMatches)
        {
            Vector3 _tempVector = hitGO.transform.position;
            hitGO.transform.position = go.transform.position;
            go.transform.localPosition = _tempVector;
            //yield return new WaitForSeconds(1f);

            candyArray.UndoSwap();

            Candy hitGOCache = null;
            int timeRun = 1;
            
            while (totalMatches.Count() >= GameVariables.MinimumMatches)
            {
                foreach (var item in totalMatches)
                {
                    candyArray.Remove(item);
                    RemoveFromScene(item);
                }

                var column = totalMatches.Select(_go => _go.GetComponent<Candy>().column).Distinct();
                var collapsedInfo = candyArray.Collapse(column);
                var newCandyInfo = CreateNewCandyinSpecificColumn(column);
                int maxDistance = Mathf.Max(collapsedInfo.MaxDistance, newCandyInfo.MaxDistance);
                MoveCandy(newCandyInfo.GetNewCandies(), maxDistance);
                MoveCandy(collapsedInfo.GetNewCandies(), maxDistance);

                //yield return new WaitForSeconds(1f * maxDistance);

                totalMatches = candyArray.GetMatches(collapsedInfo.GetNewCandies()).Union(candyArray.GetMatches(newCandyInfo.GetNewCandies())).Distinct();
                timeRun++;
            }

            gameState = GameState.None;
            StartCheckForPotentialMatches();
        }        
    }

    private void RemoveFromScene(GameObject item)
    {
        Destroy(item);
    }

    private void Update()
    {
        if(gameState == GameState.None)
        {
            if(Input.GetMouseButtonDown(0))
            {
                var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition),Vector2.zero);
                if(hit.collider != null)
                {
                    hitGO = hit.collider.gameObject;
                    gameState = GameState.SelectionStarted;
                }
            }
        }
        else if(gameState == GameState.SelectionStarted)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null && hitGO != hit.collider.gameObject)
                {
                    StopCheckPotentialMatches();
                    if(!MatchChecker.AreNeighbors(hitGO.GetComponent<Candy>(),hit.collider.gameObject.GetComponent<Candy>()))
                    {
                        gameState = GameState.None;
                    }
                    else
                    {
                        FixSortingLayer(hitGO, hit.collider.gameObject);
                        StartCoroutine(FindMatchesAndCollapse(hit));
                    }
                }
            }
        }
    }
    #endregion
}
 /// <summary>
 /// A class manages the destroyed candy or to create a new candy after destruction(or say deletion)
 /// </summary>
public class AlteredCandyInfo
{
    private List<GameObject> newCandies;
    private int maxDistance;

    public int MaxDistance { get => maxDistance; set => maxDistance = value; }

    public AlteredCandyInfo()
    {
        newCandies = new List<GameObject>();
    }

    public IEnumerable<GameObject> GetNewCandies()
    {
        return newCandies;
    }

    public void AddNewCandies(GameObject go)
    {
        if (!newCandies.Contains(go))
        {
            newCandies.Add(go);
        }
    }
}

/// <summary>
/// Class holds the row and column information of a candy
/// </summary>
public class CandyInfo
{
    private int row, column;

    public int Row { get => row; set => row = value; }
    public int Column { get => column; set => column = value; }
}

/// <summary>
/// Class contains the information about the candy matches.
/// </summary>
public class CandyMatchesInfo
{
    private List<GameObject> matches;

    public CandyMatchesInfo()
    {
        matches = new List<GameObject>();
    }

    public void AddMatchedCandyGO(GameObject go)
    {
        if (!matches.Contains(go))
            matches.Add(go);
    }

    public void AddMatchedCandyGO(List<GameObject> gos)
    {
        foreach (GameObject item in gos)
        {
            AddMatchedCandyGO(item);
        }
    }

    public IEnumerable<GameObject> GetMatchedCandy
    {
        get
        {
            return matches.Distinct();
        }
    }
}
