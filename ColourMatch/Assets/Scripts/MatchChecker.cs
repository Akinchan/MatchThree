using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchChecker
{
    public static bool AreNeighbors(Candy _candyOne, Candy _candyTwo)
    {
        return (_candyOne.column == _candyTwo.column || _candyOne.row == _candyTwo.row) && Mathf.Abs(_candyOne.column - _candyTwo.column) <= 1
            && Mathf.Abs(_candyOne.row - _candyTwo.row) <= 1;
    }

    /// <summary>
    /// Get the list of matched candy(GameObject) by passing the candy array
    /// </summary>
    /// <param name="candyArray"></param>
    /// <returns></returns>
    public static IEnumerable<GameObject> GetPotentialMatches(CandyArray candyArray)
    {
        List<List<GameObject>> matches = new List<List<GameObject>>();
        for (int row = 0; row < GameVariables.Rows; row++)
        {
            for (int col = 0; col < GameVariables.Columns; col++)
            {
                //HORIZONTAL
                List<GameObject> match1 = CheckHorizontallyOne(row, col, candyArray);
                List<GameObject> match2 = CheckHorizontallyTwo(row, col, candyArray);
                List<GameObject> match3 = CheckHorizontallyThree(row, col, candyArray);
                //VERTICAL
                List<GameObject> match4 = CheckVerticallyOne(row, col, candyArray);
                List<GameObject> match5 = CheckVerticallyTwo(row, col, candyArray);
                List<GameObject> match6 = CheckVerticallyThree(row, col, candyArray);

                if (match1 != null) matches.Add(match1);
                if (match2 != null) matches.Add(match2);
                if (match3 != null) matches.Add(match3);
                if (match4 != null) matches.Add(match4);
                if (match5 != null) matches.Add(match5);
                if (match6 != null) matches.Add(match6);

                if((matches.Count>=3) || (row >= GameVariables.Rows/2 && matches.Count > 0 && matches.Count <= 2))
                {
                    return matches[Random.Range(0,matches.Count - 1)];
                }
            }
        }
        return null;
    }

    #region HORIZONTAL CHECKERS
    /// <summary>
    /// Check horizontal matches
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <param name="candyArray"></param>
    /// <returns></returns>
    public static List<GameObject> CheckHorizontallyOne(int row, int col, CandyArray candyArray)
    {
        /*Example
         0 0 0 0 0
         0 @ @ 0 0
         @ 0 0 0 0
         0 0 0 0 0
         */

        if(col <= GameVariables.Columns - 2)
        {
            if(candyArray[row, col].GetComponent<Candy>().IsOfSameColour(candyArray[row,col + 1].GetComponent<Candy>()))
            {
                if(row >= 1 && col >= 1)
                {
                    if(candyArray[row, col].GetComponent<Candy>().IsOfSameColour(candyArray[row - 1, col - 1].GetComponent<Candy>()))
                    {
                        return new List<GameObject>
                        {
                            candyArray[row,col],
                            candyArray[row,col + 1],
                            candyArray[row - 1,col - 1]
                        };

                        if (row <= GameVariables.Rows - 2 && col >= 1)
                        {
                            if(candyArray[row,col].GetComponent<Candy>().IsOfSameColour(candyArray[row + 1, col - 1].GetComponent<Candy>()))
                            {
                                return new List<GameObject>
                                {
                                    candyArray[row,col],
                                    candyArray[row,col + 1],
                                    candyArray[row + 1,col - 1]
                                };
                            }
                        }
                        /*Example
                         0 0 0 0 0
                         0 @ @ 0 0
                         @ 0 0 0 0
                         0 0 0 0 0
                         */
                    }
                }
            }
        }
        return null;
    }

    public static List<GameObject> CheckHorizontallyTwo(int row, int col, CandyArray candyArray)
    {
        /*Example
         0 0 0 0 0
         0 @ @ 0 0
         0 0 0 @ 0
         */

        if(col <= GameVariables.Columns - 3)
        {
            if(candyArray[row,col].GetComponent<Candy>().IsOfSameColour(candyArray[row, col + 1].GetComponent<Candy>()))
            {
                if(row >= 1 && col <= GameVariables.Columns-3)
                {
                    if(candyArray[row, col].GetComponent<Candy>().IsOfSameColour(candyArray[row - 1, col + 2].GetComponent<Candy>()))
                    {
                        return new List<GameObject>
                        {
                            candyArray[row, col],
                            candyArray[row, col + 1],
                            candyArray[row - 1, col + 2]
                        };

                        if(row<=GameVariables.Rows - 2 && col <= GameVariables.Columns - 3)
                        {
                            if(candyArray[row, col].GetComponent<Candy>().IsOfSameColour(candyArray[row + 1, col + 2].GetComponent<Candy>()))
                            {
                                return new List<GameObject>
                                {
                                    candyArray[row, col],
                                    candyArray[row, col + 1],
                                    candyArray[row + 1, col + 2]
                                };
                            }
                        }

                        /*Example
                         0 0 0 @ 0
                         0 @ @ 0 0
                         0 0 0 0 0
                         */
                    }
                }
            }
        }

        return null;
    }

    public static List<GameObject> CheckHorizontallyThree(int row, int col, CandyArray candyArray)
    {
        /*Example
         0 0 0 0 0
         0 @ @ 0 @
         0 0 0 0 0
         0 0 0 0 0
         */

        if (col <= GameVariables.Columns - 4)
        {
            if (candyArray[row, col].GetComponent<Candy>().IsOfSameColour(candyArray[row, col + 1].GetComponent<Candy>())
                && candyArray[row, col].GetComponent<Candy>().IsOfSameColour(candyArray[row, col + 3].GetComponent<Candy>()))
            {
                return new List<GameObject>
                {
                    candyArray[row, col],
                    candyArray[row, col + 1],
                    candyArray[row, col + 3]
                };
            }
        }

        /*Example
         0 0 0 0 0
         0 @ 0 @ @
         0 0 0 0 0
         0 0 0 0 0
         */
        if (col >= 2 && col <= GameVariables.Columns - 2)
        {
            if (candyArray[row, col].GetComponent<Candy>().IsOfSameColour(candyArray[row, col + 1].GetComponent<Candy>())
                && candyArray[row, col].GetComponent<Candy>().IsOfSameColour(candyArray[row, col - 2].GetComponent<Candy>()))
            {
                return new List<GameObject>
                {
                    candyArray[row, col],
                    candyArray[row, col + 1],
                    candyArray[row, col - 2]
                };
            }
        }
        return null;
    }
    #endregion

    #region VERTICAL CHECKERS
    public static List<GameObject> CheckVerticallyOne(int row, int col, CandyArray candyArray)
    {
        /*Example
         0 0 0 0 0
         0 @ 0 0 0
         0 @ 0 0 0
         @ 0 0 0 0
         */
         if(row <= GameVariables.Rows - 2)
        {
            if(candyArray[row,col].GetComponent<Candy>().IsOfSameColour(candyArray[row + 1, col].GetComponent<Candy>()))
            {
                if(col >= 1 && row >= 1)
                {
                    if(candyArray[row, col].GetComponent<Candy>().IsOfSameColour(candyArray[row - 1, col - 1].GetComponent<Candy>()))
                    {
                        return new List<GameObject>
                        {
                            candyArray[row,col],
                            candyArray[row + 1,col],
                            candyArray[row - 1,col - 1]
                        };

                        /*Example
                         0 0 0 0 0
                         0 @ 0 0 0
                         0 @ 0 0 0
                         0 0 @ 0 0
                         */
                        if (col <= GameVariables.Columns - 2 && row >= 1)
                        {
                            if (candyArray[row, col].GetComponent<Candy>().IsOfSameColour(candyArray[row - 1, col + 1].GetComponent<Candy>()))
                            {
                                return new List<GameObject>
                                {
                                    candyArray[row, col],
                                    candyArray[row + 1, col],
                                    candyArray[row - 1, col + 1]
                                };
                            }
                        }
                    }
                }
            }
        }
        return null;
    }

    public static List<GameObject> CheckVerticallyTwo(int row, int col, CandyArray candyArray)
    {
        /*Example
         0 0 0 0 0
         @ 0 0 0 0
         0 @ 0 0 0
         0 @ 0 0 0
         */

        if(row <= GameVariables.Rows - 3)
        {
            if (candyArray[row, col].GetComponent<Candy>().IsOfSameColour(candyArray[row + 1, col].GetComponent<Candy>()))
            {
                if(col >= 1)
                {
                    if (candyArray[row, col].GetComponent<Candy>().IsOfSameColour(candyArray[row + 2, col - 1].GetComponent<Candy>()))
                    {
                        return new List<GameObject>
                        {
                            candyArray[row, col],
                            candyArray[row + 1, col],
                            candyArray[row + 2, col - 1]
                        };

                        /*Example
                         0 0 0 0 0
                         0 0 @ 0 0
                         0 @ 0 0 0
                         0 @ 0 0 0
                         */
                        if(col <= GameVariables.Columns - 2)
                        {
                            if (candyArray[row, col].GetComponent<Candy>().IsOfSameColour(candyArray[row + 2, col + 1].GetComponent<Candy>()))
                            {
                                return new List<GameObject>
                                {
                                    candyArray[row, col],
                                    candyArray[row + 1, col],
                                    candyArray[row + 2, col + 1]
                                };
                            }
                        }
                    }
                }
            }
        }
        return null;
    }

    public static List<GameObject> CheckVerticallyThree(int row, int col, CandyArray candyArray)
    {
        /*Example
         0 @ 0 0 0
         0 0 0 0 0
         0 @ 0 0 0
         0 @ 0 0 0
         */
        if(row <= GameVariables.Rows - 4)
        {
            if(candyArray[row,col].GetComponent<Candy>().IsOfSameColour(candyArray[row + 1, col].GetComponent<Candy>())
                && candyArray[row, col].GetComponent<Candy>().IsOfSameColour(candyArray[row + 3, col].GetComponent<Candy>()))
            {
                return new List<GameObject>
                {
                    candyArray[row,col],
                    candyArray[row + 1,col],
                    candyArray[row + 3,col]
                };
            }
        }
        /*Example
         0 @ 0 0 0
         0 @ 0 0 0
         0 0 0 0 0
         0 @ 0 0 0
         */

        if(row >= 2 && row <= GameVariables.Rows - 2)
        {
            if (candyArray[row, col].GetComponent<Candy>().IsOfSameColour(candyArray[row + 1, col].GetComponent<Candy>())
                && candyArray[row, col].GetComponent<Candy>().IsOfSameColour(candyArray[row - 2, col].GetComponent<Candy>()))
            {
                return new List<GameObject>
                {
                    candyArray[row, col],
                    candyArray[row + 1, col],
                    candyArray[row - 2, col]
                };
            }
        }
        return null;
    }
    #endregion
}
