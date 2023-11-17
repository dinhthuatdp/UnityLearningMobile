using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Drawing;

public class PikachuManager
{
    private int cnt = 0;
    private string[,] matrix;
    int length = 0;
    int Cols = 0;
    int Rows = 0;

    public PikachuManager(TilesManager tilesManager)
    {
        Cols = tilesManager.Columns;
        Rows = tilesManager.Rows;
        cnt = 0;
        length = tilesManager.Tiles.Length;
        //matrix = new string[tilesManager.Rows + 2, tilesManager.Columns + 2];
        matrix = tilesManager.Tiles;
        //for (int i = 0; i < tilesManager.Rows; i++)
        //{
        //    matrix[0, i] = null;
        //}
        //for (int i = 0; i < tilesManager.Columns; i++)
        //{
        //    matrix[i, 0] = null;
        //}
        //for (int i = 1; i <= tilesManager.Rows; i++)
        //{
        //    if (i == 0)
        //    {
        //        continue;
        //    }
        //    for (int j = 1; j <= tilesManager.Columns; j++)
        //    {
        //        if (j == 0)
        //        {
        //            continue;
        //        }
        //        matrix[i, j] = tilesManager.Tiles[i - 1, j - 1];
        //    }
        //}
    }
    // check with line x, from column y1 to y2
    private bool CheckLineX(int y1, int y2, int x)
    {
        // find point have column max and min
        int min = Math.Min(y1, y2);
        int max = Math.Max(y1, y2);
        if (max - min == 1)
        {
            return true;
        }
        // run column
        for (int y = min + 1; y < max; y++)
        {
            if (matrix[x,y] != null)
            { // if see barrier then die
                Debug.Log("CheckLineX die: " + x + "" + y);
                return false;
            }
            Debug.Log("CheckLineX ok: " + x + "" + y);
        }
        // not die -> success
        return true;
    }

    private bool CheckLineY(int x1, int x2, int y)
    {
        int min = Math.Min(x1, x2);
        int max = Math.Max(x1, x2);
        if (max - min == 1)
        {
            return true;
        }
        for (int x = min + 1; x < max; x++)
        {
            if (matrix[x,y] != null)
            {
                Debug.Log("CheckLineY die: " + x + "" + y);
                return false;
            }
            Debug.Log("CheckLineY ok: " + x + "" + y);
        }
        return true;
    }

    // check in rectangle
    public int CheckRectX(Vector2 p1, Vector2 p2)
    {
        if ((int)p1.y == (int)p2.y &&
            ((int)p1.y == Cols - 1 || (int)p1.y == 0))
        {
            return (int)p1.y;
        }
        // find point have y min and max
        Vector2 pMinY = p1, pMaxY = p2;
        if (p1.y > p2.y)
        {
            pMinY = p2;
            pMaxY = p1;
        }
        for (int y = (int)pMinY.y + 1; y < pMaxY.y; y++)
        {
            // check three line
            if (CheckLineX((int)pMinY.y, y, (int)pMinY.x)
                    && CheckLineY((int)pMinY.x, (int)pMaxY.x, y)
                    && CheckLineX(y, (int)pMaxY.y, (int)pMaxY.x))
            {

                Debug.Log("Rect x");
                Debug.Log("(" + pMinY.x + "," + pMinY.y + ") -> ("
                        + pMinY.x + "," + y + ") -> (" + pMaxY.x + "," + y
                        + ") -> (" + pMaxY.x + "," + pMaxY.y + ")");
                // if three line is true return column y
                return y;
            }
        }
        // have a line in three line not true then return -1
        return -1;
    }

    public int checkRectY(Vector2 p1, Vector2 p2)
    {
        if (p1.x == p2.x &&
            ((int)p1.x == Rows - 1 || (int)p1.x == 0))
        {
            return (int)p1.x;
        }
        // find point have y min
        Vector2 pMinX = p1, pMaxX = p2;
        if (p1.x > p2.x)
        {
            pMinX = p2;
            pMaxX = p1;
        }
        // find line and y begin
        for (int x = (int)pMinX.x + 1; x < pMaxX.x; x++)
        {
            if (CheckLineY((int)pMinX.x, x, (int)pMinX.y)
                && CheckLineX((int)pMinX.y, (int)pMaxX.y, x)
                && CheckLineY(x, (int)pMaxX.x, (int)pMaxX.y))
            {

                Debug.Log("Rect y");
                Debug.Log("(" + pMinX.x + "," + pMinX.y + ") -> (" + x
                        + "," + pMinX.y + ") -> (" + x + "," + pMaxX.y
                        + ") -> (" + pMaxX.x + "," + pMaxX.y + ")");
                return x;
            }
        }
        return -1;
    }

    public int CheckMoreLineX(Vector2 p1, Vector2 p2, int type)
    {
        // find point have y min
        Vector2 pMinY = p1, pMaxY = p2;
        if (p1.y > p2.y)
        {
            pMinY = p2;
            pMaxY = p1;
        }
        // find line and y begin
        int y = (int)pMaxY.y;
        int row = (int)pMinY.x;
        if (type == -1)
        {
            y = (int)pMinY.y;
            row = (int)pMaxY.x;
        }
        // check more
        if (CheckLineX((int)pMinY.y, (int)pMaxY.y, row))
        {
            while (matrix[(int)pMinY.x, y] == null &&
                matrix[(int)pMaxY.x, y] == null)
            {
                if (CheckLineY((int)pMinY.x, (int)pMaxY.x, y))
                {

                    Debug.Log("CheckMoreLineX: TH X " + type);
                    Debug.Log("(" + pMinY.x + "," + pMinY.y + ") -> ("
                            + pMinY.x + "," + y + ") -> (" + pMaxY.x + "," + y
                            + ") -> (" + pMaxY.x + "," + pMaxY.y + ")");
                    return y;
                }
                y += type;
            }
        }
        return -1;
    }

    public int CheckMoreLineY(Vector2 p1, Vector2 p2, int type)
    {
        Vector2 pMinX = p1, pMaxX = p2;
        if (p1.x > p2.x)
        {
            pMinX = p2;
            pMaxX = p1;
        }
        int x = (int)pMaxX.x;
        int col = (int)pMinX.y;
        if (type == -1)
        {
            x = (int)pMinX.x;
            col = (int)pMaxX.y;
        }
        if (CheckLineY((int)pMinX.x, (int)pMaxX.x, col))
        {
            while (matrix[x, (int)pMinX.y] == null &&
                matrix[x, (int)pMaxX.y] == null)
            {
                if (CheckLineX((int)pMinX.y, (int)pMaxX.y, x))
                {
                    Debug.Log("CheckMoreLineY: TH Y " + type);
                    Debug.Log("(" + pMinX.x + "," + pMinX.y + ") -> ("
                            + x + "," + pMinX.y + ") -> (" + x + "," + pMaxX.y
                            + ") -> (" + pMaxX.x + "," + pMaxX.y + ")");
                    return x;
                }
                x += type;
            }
        }
        return -1;
    }



    private void HideItemIfMatch(Vector2 p1, Vector2 p2)
    {
        matrix[(int)p1.x, (int)p1.y] = null;
        matrix[(int)p2.x, (int)p2.y] = null;
        cnt += 2;
    }

    public bool CheckWin()
    {
        return cnt == length;
    }

    public MyLine CheckTwoPoint(Vector2 p1, Vector2 p2)
    {
        //p1 = new Vector2(p1.x + 1, p1.y + 1);
        //p2 = new Vector2(p2.x + 1, p2.y + 1);
        // check line with x
        if (p1.x == p2.x)
        {
            if (CheckLineX((int)p1.y, (int)p2.y, (int)p1.x))
            {
                HideItemIfMatch(p1, p2);
                return new MyLine(p1, p2);
            }
        }
        // check line with y
        if (p1.y == p2.y)
        {
            if (CheckLineY((int)p1.x, (int)p2.x, (int)p1.y))
            {
                HideItemIfMatch(p1, p2);
                return new MyLine(p1, p2);
            }
        }

        int t = -1; // t is column find

        // check in rectangle with x
        if ((t = CheckRectX(p1, p2)) != -1)
        {
            HideItemIfMatch(p1, p2);
            return new MyLine(new Vector2((int)p1.x, t), new Vector2((int)p2.x, t));
        }

        // check in rectangle with y
        if ((t = checkRectY(p1, p2)) != -1)
        {
            HideItemIfMatch(p1, p2);
            return new MyLine(new Vector2(t, p1.y), new Vector2(t, p2.y));
        }
        // check more right
        if ((t = CheckMoreLineX(p1, p2, 1)) != -1)
        {
            HideItemIfMatch(p1, p2);
            return new MyLine(new Vector2(p1.x, t), new Vector2(p2.x, t));
        }
        // check more left
        if ((t = CheckMoreLineX(p1, p2, -1)) != -1)
        {
            HideItemIfMatch(p1, p2);
            return new MyLine(new Vector2(p1.x, t), new Vector2(p2.x, t));
        }
        // check more down
        if ((t = CheckMoreLineY(p1, p2, 1)) != -1)
        {
            HideItemIfMatch(p1, p2);
            return new MyLine(new Vector2(t, p1.y), new Vector2(t, p2.y));
        }
        // check more up
        if ((t = CheckMoreLineY(p1, p2, -1)) != -1)
        {
            HideItemIfMatch(p1, p2);
            return new MyLine(new Vector2(t, p1.y), new Vector2(t, p2.y));
        }
        return null;
    }
}

public class MyLine
{
    public Vector2 p1;
    public Vector2 p2;

    public MyLine(Vector2 p1, Vector2 p2)
    {
        this.p1 = p1;
        this.p2 = p2;
    }

    public String toString()
    {
        string result = "(" + p1.x + "," + p1.y + ") and (" + p2.x + "," + p2.y + ")";
        return result;
    }
}