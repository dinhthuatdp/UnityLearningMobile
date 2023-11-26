using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Drawing;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.Burst.Intrinsics;

public class PikachuManager1
{
    public PikachuManager1(string[,] arr)
    {
        InitMatrix(arr);
    }

    private string[,] _matrix;
    private void InitMatrix(string[,] arr)
    {
        int rows = arr.GetLength(0);
        int cols = arr.GetLength(1);
        _matrix = new string[rows + 2, cols + 2];
        for (int row = 1; row <= rows; row++)
        {
            for (int col = 1; col <= cols; col++)
            {
                _matrix[row, col] = arr[row - 1, col - 1];
            }
        }
    }

    private void RemoveMatchedPair(Vector2 p1, Vector2 p2)
    {
        _matrix[(int)p1.x, (int)p1.y] = null;
        _matrix[(int)p2.x, (int)p2.y] = null;
    }

    public List<Vector2> FindPath(Vector2 p1, Vector2 p2)
    {
        p1 = p1 + new Vector2(1, 1);
        p2 = p2 + new Vector2(1, 1);
        List<Vector2> result = null;
        if (p1.x == p2.x)
        {
            result = CheckX(p1, p2);
            if (result != null)
            {
                RemoveMatchedPair(p1, p2);
                return result;
            }
        }
        if (p1.y == p2.y)
        {
            result = CheckY(p1, p2);
            if (result != null)
            {
                RemoveMatchedPair(p1, p2);
                return result;
            }
        }
        result = CheckRect(p1, p2);
        if (result != null)
        {
            RemoveMatchedPair(p1, p2);
            return result;
        }
        result = CheckRectExtra(p1, p2);
        if (result != null)
        {
            RemoveMatchedPair(p1, p2);
            return result;
        }
        return null;
    }

    private List<Vector2> CheckX(Vector2 p1, Vector2 p2)
    {
        var maxY = Math.Max(p1.y, p2.y);
        var minY = Math.Min(p1.y, p2.y);
        Vector2 pMinY = p1.y < p2.y ? p1 : p2;
        for (float y = minY + 1; y < maxY; y++)
        {
            if (_matrix[(int)pMinY.x, (int)y] != null)
            {
                return null;
            }
        }
        return new List<Vector2> { p1, p2 };
    }

    private List<Vector2> CheckY(Vector2 p1, Vector2 p2)
    {
        var maxX = Math.Max(p1.x, p2.x);
        var minX = Math.Min(p1.x, p2.x);
        Vector2 pMinX = p1.y < p2.y ? p1 : p2;
        for (float x = minX + 1; x < maxX; x++)
        {
            if (_matrix[(int)x, (int)pMinX.y] != null)
            {
                return null;
            }
        }
        return new List<Vector2> { p1, p2 };
    }

    private List<Vector2> CheckRect(Vector2 p1, Vector2 p2)
    {
        Vector2 pMinX, pMaxX;
        Vector2 pMinY, pMaxY;
        if (p1.x < p2.x)
        {
            pMinX = p1;
            pMaxX = p2;
        }
        else
        {
            pMinX = p2;
            pMaxX = p1;
        }
        if (p1.y < p2.y)
        {
            pMinY = p1;
            pMaxY = p2;
        }
        else
        {
            pMinY = p2;
            pMaxY = p1;
        }
        if (pMinY == pMinX && pMaxX == pMaxY)
        {
            var check1 = CheckX(pMinX, new Vector2(pMinX.x, pMaxY.y + 1));
            var check2 = CheckY(new Vector2(pMinX.x - 1, pMaxY.y), pMaxY);
            if (check1 != null &&
                check2 != null)
            {
                /*---->
                 *    |
                 *    |
                 *    v
                 */
                return new List<Vector2>
                {
                    p1, new Vector2(pMinX.x, pMaxY.y), p2
                };
            }
            check1 = CheckY(pMinX, new Vector2(pMaxX.x + 1, pMinY.y));
            check2 = CheckX(new Vector2(pMaxX.x, pMinY.y - 1), pMaxY);
            if (check1 != null &&
                check2 != null)
            {
                /* |
                 * |
                 * v_____>
                 * 
                 */
                return new List<Vector2>
                {
                    p1, new Vector2(pMaxX.x, pMinX.y), p2
                };
            }
        }
        if (pMinX == pMaxY && pMaxX == pMinY)
        {
            var check1 = CheckX(pMinX, new Vector2(pMinX.x, pMinY.y - 1));
            var check2 = CheckY(new Vector2(pMinX.x - 1, pMinY.y), pMaxX);
            if (check1 != null &&
                check2 != null)
            {
                /* <________
                 * |
                 * |
                 * |
                 * v
                 */
                return new List<Vector2>
                {
                    p1, new Vector2(pMinX.x, pMinY.y), p2
                };
            }
            check1 = CheckY(pMinX, new Vector2(pMaxX.x + 1, pMaxY.y));
            check2 = CheckX(new Vector2(pMaxX.x, pMaxY.y + 1), pMaxX);
            if (check1 != null &&
                check2 != null)
            {
                /*      |
                 *      | 
                 *      |
                 *      v
                 * <-----
                 */
                return new List<Vector2>
                {
                    p1, new Vector2(pMaxX.x, pMaxY.y), p2
                };
            }
        }
        if (pMinY.x < pMaxY.x)
        {
            /*---->
             *    |
             *    | 
             *    v
             *    ----->
             */
            for (float y = pMinY.y + 1; y < pMaxY.y; y++)
            {
                var check1 = CheckX(pMinX, new Vector2(pMinX.x, y + 1));
                var check2 = CheckY(new Vector2(pMinX.x - 1, y), new Vector2(pMaxX.x + 1, y));
                var check3 = CheckX(new Vector2(pMaxX.x, y - 1), new Vector2(pMaxX.x, pMaxY.y));
                if (check1 != null &&
                    check2 != null &&
                    check3 != null)
                {
                    return new List<Vector2>
                    {
                        pMinX, new Vector2(pMinX.x, y), new Vector2(pMaxX.x, y), pMaxX
                    };
                }

            }

            /* |
             * |
             * |
             * v
             *  ------->
             *         |
             *         |  
             *         |
             *         v
             */
            for (float x = pMinX.x + 1; x < pMaxX.x; x++)
            {
                var check1 = CheckY(pMinX, new Vector2(x + 1, pMinX.y));
                var check2 = CheckX(new Vector2(x, pMinY.y - 1), new Vector2(x, pMaxY.y + 1));
                var check3 = CheckY(new Vector2(x - 1, pMaxX.y), pMaxY);
                if (check1 != null &&
                    check2 != null &&
                    check3 != null)
                {
                    return new List<Vector2>
                    {
                        pMinX, new Vector2(x, pMinY.y), new Vector2(x, pMaxY.y), pMaxX
                    };
                }
            }
        }
        if (pMinY.x > pMaxY.x)
        {
            /*
             *     <------
             *     |
             *     |
             *     | 
             *     v
             *<---- 
             */
            for (float y = pMaxY.y - 1; y > pMinY.y; y--)
            {
                var check1 = CheckX(pMinX, new Vector2(pMinX.x, y - 1));
                var check2 = CheckY(new Vector2(pMinX.x - 1, y), new Vector2(pMaxX.x + 1, y));
                var check3 = CheckX(new Vector2(pMaxX.x, y - 1), pMaxX);
                if (check1 != null &&
                    check2 != null &&
                    check3 != null)
                {
                    return new List<Vector2>
                    {
                        pMinX, new Vector2(pMinX.x, y), new Vector2(pMaxX.x, y), pMaxX
                    };
                }
            }
            /*
             *       | 
             *       |
             *       |
             *       v
             * <-----
             * |
             * |
             * |
             * v
             */
            for (float x = pMinX.x + 1; x < pMaxX.x; x++)
            {
                var check1 = CheckY(pMinX, new Vector2(x + 1, pMaxY.y));
                var check2 = CheckX(new Vector2(x, pMaxY.y + 1), new Vector2(x, pMinY.y - 1));
                var check3 = CheckY(new Vector2(x - 1, pMinY.y), pMinY);
                if (check1 != null &&
                    check2 != null &&
                    check3 != null)
                {
                    return new List<Vector2>
                    {
                        pMinX, new Vector2(x, pMaxY.y), new Vector2(x, pMinY.y), pMaxX
                    };
                }
            }
        }
        return null;
    }

    private List<Vector2> CheckRectExtra(Vector2 p1, Vector2 p2)
    {
        int rows = _matrix.GetLength(0);
        int cols = _matrix.GetLength(1);
        if (p1.y == p2.y)
        {
            if (p1.y == 1)
            {
                return new List<Vector2>
                {
                    p1, new Vector2(p1.x, p1.y - 1), new Vector2(p2.x, p2.y - 1), p2
                };
            }
            if (p1.y == cols - 1)
            {
                return new List<Vector2>
            {
                p1, new Vector2(p1.x, p1.y + 1), new Vector2(p2.x, p2.y + 1), p2
            };
            }
        }
        if (p1.x == p2.x)
        {
            if (p1.x == 1)
            {
                return new List<Vector2>
                {
                    p1, new Vector2(p1.x - 1, p1.y), new Vector2(p2.x - 1, p2.y), p2
                };
            }
            if (p1.x == rows - 1)
            {
                return new List<Vector2>
                {
                    p1, new Vector2(p1.x + 1, p1.y), new Vector2(p2.x + 1, p2.y), p2
                };
            }
        }
        Vector2 pMinX, pMaxX;
        Vector2 pMinY, pMaxY;
        if (p1.x < p2.x)
        {
            pMinX = p1;
            pMaxX = p2;
        }
        else
        {
            pMinX = p2;
            pMaxX = p1;
        }
        if (p1.y < p2.y)
        {
            pMinY = p1;
            pMaxY = p2;
        }
        else
        {
            pMinY = p2;
            pMaxY = p1;
        }
        //if (pMaxY == pMinX &&
        //    pMinY == pMaxX)
        {
            //var checkX1 = CheckX(pMaxY, new Vector2(pMinX.x, 0));
            //var checkX2 = CheckX(new Vector2(pMaxX.x, 0), pMinY);
            //if (checkX1 != null &&
            //    checkX2 != null)
            //{
            //    return new List<Vector2>
            //    {
            //        pMaxY, new Vector2(pMinX.x, 1), new Vector2(pMaxX.x, 1), pMinY
            //    };
            //}
            // Check left
            for (int y = 1; y < pMinY.y; y++)
            {
                /*
                 * <----pMaxX, pMinX
                 * |
                 * |
                 * v
                 * ---------->pMinY, pMaxX
                 */
                var check1 = CheckX(pMinX, new Vector2(pMinX.x, y - 1));
                var check2 = CheckY(new Vector2(pMinX.x - 1, y), new Vector2(pMaxX.x + 1, y));
                var check3 = CheckX(new Vector2(pMaxX.x, y - 1), pMaxX);
                if (check1 != null &&
                    check2 != null &&
                    check3 != null)
                {
                    return new List<Vector2>
                    {
                        pMaxY, new Vector2(pMinX.x, y), new Vector2(pMaxX.x, y), pMinY
                    };
                }
            }
            // Check right
            for (float y = pMaxY.y + 1; y < cols; y++)
            {
                var check1 = CheckX(pMaxY, new Vector2(pMinX.x, y + 1));
                var check2 = CheckY(new Vector2(pMinX.x - 1, y), new Vector2(pMaxX.x + 1, y));
                var check3 = CheckX(new Vector2(pMaxX.x, y - 1), pMaxX);
                if (check1 != null &&
                    check2 != null &&
                    check3 != null)
                {
                    return new List<Vector2>
                    {
                        pMaxY, new Vector2(pMinX.x, y), new Vector2(pMaxX.x, y), pMinY
                    };
                }

            }
            // Check top
            for (int x = 1; x < pMinX.x; x++)
            {
                /*
                 * 
                 * -----^
                 * |    |
                 * |    |
                 * |    pMinX, pMaxY
                 * |
                 * v
                 * pMaxX, pMinY
                 */
                var check1 = CheckY(pMinX, new Vector2(x - 1, pMinX.y));
                var check2 = CheckX(new Vector2( x, pMinX.y - 1), new Vector2(x, pMaxX.y - 1));
                var check3 = CheckY(new Vector2(x - 1, pMinY.y), pMinY);
                if (check1 != null &&
                    check2 != null &&
                    check3 != null)
                {
                    return new List<Vector2>
                    {
                        pMinX, new Vector2(x, pMaxY.y), new Vector2(x, pMinY.y), pMaxX
                    };
                }
            }
            // Check bottom
            for (float x = pMaxX.x + 1; x < rows; x++)
            {
                /*
                 *             pMinX, pMaxY
                 *                  |
                 *                  |
                 * pMaxX, pMinY     |
                 * ^                |
                 * |                |
                 * |                v
                 * <-----------------
                 */
                var check1 = CheckY(pMaxY, new Vector2(x + 1, pMaxY.y));
                var check2 = CheckX(new Vector2(x, pMaxY.y + 1), new Vector2(x, pMinY.y - 1));
                var check3 = CheckY(new Vector2(x + 1, pMinY.y), pMinY);
                if (check1 != null &&
                    check2 != null &&
                    check3 != null)
                {
                    return new List<Vector2>
                    {
                        pMaxY, new Vector2(x, pMinY.y), new Vector2(x, pMinY.y), pMinY
                    };
                }
            }
        }
        if (pMinX == pMinY &&
            pMaxX == pMaxY)
        {

        }

        return null;
    }
}