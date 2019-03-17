using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candy : MonoBehaviour
{
    #region VARIABLES
    public CandyColour candyColour;
    public int row;
    public int column;
    #endregion

    #region PUBLIC METHODS
    /// <summary>
    /// Checks the two candies are of same colour or not.
    /// </summary>
    /// <param name="otherCandy"></param>
    /// <returns></returns>
    public bool IsOfSameColour(Candy otherCandy)
    {
        return candyColour.CompareTo(otherCandy.candyColour) == 0;
    }

    /// <summary>
    /// Initialize candy properties
    /// </summary>
    /// <param name="_row"></param>
    /// <param name="_col"></param>
    /// <param name="_candyColour"></param>
    public void Initialize(int _row, int _col, CandyColour _candyColour)
    {
        row = _row;
        column = _col;
        candyColour = _candyColour;
    }

    /// <summary>
    /// Swap candies
    /// </summary>
    /// <param name="_candyOne"></param>
    /// <param name="_candyTwo"></param>
    public static void SwapCandies(Candy _candyOne, Candy _candyTwo)
    {
        //For Rows
        int temp = _candyOne.row;
        _candyOne.row = _candyTwo.row;
        _candyTwo.row = temp;

        //For Column
        temp = _candyOne.column;
        _candyOne.column = _candyTwo.column;
        _candyTwo.column = temp;

    }
    #endregion
}
