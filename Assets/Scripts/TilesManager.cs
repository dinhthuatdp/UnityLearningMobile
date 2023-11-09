using System;
using System.Collections.Generic;
using System.Linq;

public class TilesManager // : MonoBehaviour
{
    public List<int> AllPairs = new() { 1, 2, 3 };
    public List<int> ResultPair = new();
    public int[,] Tiles;

    private readonly int _rows;
    private readonly int _columns;

    public TilesManager(int rows, int columns)
    {
        _rows = rows;
        _columns = columns;
    }

    public List<int> InitPairs()
    {
        int numberOfPair = _rows * _columns / 2;
        if (AllPairs.Count > numberOfPair)
        {
            ResultPair.AddRange(AllPairs.Take(numberOfPair));
        }
        else
        {
            int cntListPair = (numberOfPair / AllPairs.Count);
            for (int i = 0; i < cntListPair; i++)
            {
                ResultPair.AddRange(AllPairs);
            }
            ResultPair.AddRange(AllPairs.Take(numberOfPair % AllPairs.Count));
        }
        ResultPair.AddRange(ResultPair);

        return ResultPair;
    }

    public int[,] InitTiles()
    {
        Tiles = new int[_rows, _columns];

        int randomIndex = 0;

        for (int row = 0; row < _rows; row++)
        {
            for (int column = 0; column < _columns; column++)
            {
                randomIndex = UnityEngine.Random.Range(0, ResultPair.Count);
                Tiles[row, column] = ResultPair[randomIndex];

                Console.Write($"[{row}, {column}] = {ResultPair[randomIndex]}".PadLeft(20));
                ResultPair = ResultPair.Where((v, i) => i != randomIndex).ToList();
            }
        }

        return Tiles;
    }

    public void Load()
    {
        InitPairs();
        InitTiles();
    }
}
