using System;
using System.Collections.Generic;
using System.Linq;

public class TilesManager // : MonoBehaviour
{
    public List<string> AllPairs = new() { "img1", "img2", "img3", "img4" };
    public List<string> ResultPair = new();
    public string[,] Tiles;
    public readonly List<LevelModel> LevelSettings = new();

    public readonly int Rows;
    public readonly int Columns;

    public TilesManager(GameLevel gameLevel,
        GameType gameType)
    {
        if (gameType == GameType.Pikachu)
        {
            AllPairs = new() { "p1", "p2", "p3", "p4", "p5" };
        }
        switch (gameLevel)
        {
            case GameLevel.Level2:
                Rows = 5;
                Columns = 4;
                break;
            case GameLevel.Level1:
            default:
                Rows = 4;
                Columns = 3;
                break;
        }
        LevelSettings = new List<LevelModel>
        {
            new LevelModel
            {
                Level = GameLevel.Level1,
                Offset = 20,
                Width = 125,
                Rows = 4,
                Columns = 3
            },
            new LevelModel
            {
                Level = GameLevel.Level2,
                Offset = 10,
                Width = 100,
                Rows = 5,
                Columns = 4
            },
        };
    }

    public List<string> InitPairs()
    {
        int numberOfPair = Rows * Columns / 2;
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

    public string[,] InitTiles()
    {
        Tiles = new string[Rows, Columns];

        int randomIndex = 0;

        for (int row = 0; row < Rows; row++)
        {
            for (int column = 0; column < Columns; column++)
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

public class LevelModel
{
    public GameLevel Level { get; set; }

    public float Width { get; set; }

    public float Offset { get; set; }

    public int Rows { get; set; }

    public int Columns { get; set; }
}

public enum GameLevel
{
    Level1 = 1,
    Level2 = 2
}

public enum GameType
{
    Matching = 1,
    Pikachu = 2
}