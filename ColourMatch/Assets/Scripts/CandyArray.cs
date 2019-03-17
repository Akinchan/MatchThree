using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class CandyArray : MonoBehaviour
{
    #region VARIABLES
    private GameObject[,] candies;
    private GameObject candyOne, candyTwo;

    public GameObject this[int _row, int _column]
    {
        get
        {
            try
            {
                return candies[_row, _column];
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        set
        {
            if (candies == null)
                Awake();

            candies[_row, _column] = value;
        }
    }
    #endregion

    #region PUBLIC METHODS
    /// <summary>
    /// Swap two candies
    /// </summary>
    /// <param name="c1"></param>
    /// <param name="c2"></param>
    public void SwapCandiesGO(GameObject c1, GameObject c2)
    {
        candyOne = c1;
        candyTwo = c2;

        Candy c1Candy = c1.GetComponent<Candy>();
        Candy c2Candy = c2.GetComponent<Candy>();

        if (c1Candy != null && c2Candy != null)
        {
            GameObject temp = candies[c1Candy.row, c1Candy.column];
            candies[c1Candy.row, c1Candy.column] = candies[c2Candy.row, c2Candy.column];
            candies[c2Candy.row, c2Candy.column] = temp;

            Candy.SwapCandies(c1Candy, c2Candy);
        }
    }

    /// <summary>
    /// Undo swap
    /// </summary>
    public void UndoSwap()
    {
        SwapCandiesGO(candyOne, candyTwo);
    }

    /// <summary>
    /// Removing the paseed gameobject from the array
    /// </summary>
    /// <param name="go"></param>
    public void Remove(GameObject go)
    {
        Candy _candy = go.GetComponent<Candy>();
        if (_candy != null)
        {
            candies[_candy.row, _candy.column] = null;
        }
    }

    /// <summary>
    /// Colapse or delete the specified column
    /// </summary>
    /// <param name="_columns"></param>
    /// <returns></returns>
    public AlteredCandyInfo Collapse(IEnumerable<int> _columns)
    {
        AlteredCandyInfo alteredCandy = new AlteredCandyInfo();

        foreach (int _col in _columns)
        {
            for (int _row = 0; _row < GameVariables.Rows - 1; _row++)
            {
                if (candies[_row, _col] == null)
                {
                    for (int _row2 = _row + 1; _row2 < GameVariables.Rows; _row2++)
                    {
                        if (candies[_row2, _col] != null)
                        {
                            candies[_row, _col] = candies[_row2, _col];
                            candies[_row2, _col] = null;

                            if (_row2 - _row > alteredCandy.MaxDistance)
                                alteredCandy.MaxDistance = _row2 - _row;

                            candies[_row, _col].GetComponent<Candy>().row = _row;
                            candies[_row, _col].GetComponent<Candy>().column = _col;

                            alteredCandy.AddNewCandies(candies[_row, _col]);
                            break;
                        }
                    }
                }
            }
        }

        return alteredCandy;
    }

    /// <summary>
    /// Get the empty cell [gameObject] in the specific column
    /// </summary>
    /// <param name="_column"></param>
    /// <returns></returns>
    public IEnumerable<CandyInfo> GetEmptyItemsOnColumn(int _column)
    {
        List<CandyInfo> emptyItems = new List<CandyInfo>();
        for (int _row = 0; _row < GameVariables.Rows; _row++)
        {
            if (candies[_row, _column] == null)
            {
                emptyItems.Add(new CandyInfo() { Row = _row, Column = _column });
            }
        }
        return emptyItems;
    }

    /// <summary>
    /// Get for matches with the gameObject (candy)
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public CandyMatchesInfo GetMatches(GameObject go)
    {
        CandyMatchesInfo candyMatchesInfo = new CandyMatchesInfo();
        //Horizontal
        List<GameObject> horizontallyMatches = GetMatchesHorizontally(go).ToList<GameObject>();
        candyMatchesInfo.AddMatchedCandyGO(horizontallyMatches);
        //Vertical
        List<GameObject> verticallyMatches = GetMatchesVertically(go).ToList<GameObject>();
        candyMatchesInfo.AddMatchedCandyGO(verticallyMatches);

        return candyMatchesInfo;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gos"></param>
    /// <returns></returns>
    public IEnumerable<GameObject> GetMatches(IEnumerable<GameObject> gos)
    {
        List<GameObject> matches = new List<GameObject>();
        foreach (GameObject item in gos)
        {
            matches.AddRange(GetMatches(item).GetMatchedCandy);
        }
        return matches.Distinct();
    }
    #endregion

    #region PRIVATE METHODS
    private void Awake()
    {
        candies = new GameObject[GameVariables.Rows, GameVariables.Columns];
    }

    /// <summary>
    /// Get horizontal gameobject list
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    private IEnumerable<GameObject> GetMatchesHorizontally(GameObject go)
    {
        List<GameObject> matches = new List<GameObject>();
        matches.Add(go);

        Candy _candy = go.GetComponent<Candy>();
        if (_candy != null)
        {
            //Search to the left side
            if (_candy.column != 0)
            {
                for (int i = _candy.column - 1; i >= 0; i--)
                {
                    if (candies[_candy.row, i].GetComponent<Candy>().IsOfSameColour(_candy))
                    {
                        matches.Add(candies[_candy.row, i]);
                    }
                    else
                        break;
                }
            }

            //Search to the right side
            if (_candy.column != GameVariables.Columns - 1)
            {
                for (int j = _candy.column + 1; j < GameVariables.Columns; j++)
                {
                    if (candies[_candy.row, j].GetComponent<Candy>().IsOfSameColour(_candy))
                    {
                        matches.Add(candies[_candy.row, j]);
                    }
                    else
                        break;
                }
            }

            if (matches.Count < GameVariables.MinimumMatches)
            {
                matches.Clear();
            }
        }
        return matches.Distinct();
    }

    /// <summary>
    /// Get vertical gameObject list 
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    private IEnumerable<GameObject> GetMatchesVertically(GameObject go)
    {
        List<GameObject> matches = new List<GameObject>();
        matches.Add(go);

        Candy _candy = go.GetComponent<Candy>();
        if (_candy != null)
        {
            //Search to the bottom
            if (_candy.row != 0)
            {
                for (int i = _candy.row - 1; i >= 0; i--)
                {
                    if (candies[i, _candy.column].GetComponent<Candy>().IsOfSameColour(_candy))
                    {
                        matches.Add(candies[i, _candy.column]);
                    }
                    else
                        break;
                }
            }

            //Search to the top
            if (_candy.row != GameVariables.Rows - 1)
            {
                for (int j = _candy.row + 1; j < GameVariables.Rows; j++)
                {
                    if (candies[j, _candy.column].GetComponent<Candy>().IsOfSameColour(_candy))
                    {
                        matches.Add(candies[j, _candy.column]);
                    }
                    else
                        break;
                }
            }
        
            if (matches.Count < GameVariables.MinimumMatches)
            {
                matches.Clear();
            }
        }
        return matches.Distinct();
    }

    /// <summary>
    /// Get the entire row of that gameObject.
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    private IEnumerable<GameObject> GetEntireRow(GameObject go)
    {
        List<GameObject> matches = new List<GameObject>();
        int _row = go.GetComponent<Candy>().row;

        for (int i = 0; i < GameVariables.Columns; i++)
        {
            matches.Add(candies[_row, i]);
        }
        return matches;
    }

    /// <summary>
    /// Get the entire column of that gameObject.
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    private IEnumerable<GameObject> GetEntireColumn(GameObject go)
    {
        List<GameObject> matches = new List<GameObject>();
        int _col = go.GetComponent<Candy>().column;

        for (int i = 0; i < GameVariables.Rows; i++)
        {
            matches.Add(candies[i, _col]);
        }
        return matches;
    }
    #endregion
}
