using System;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private GameObject _tilePrefab;

    [SerializeField] private Canvas _boardGame;
    [SerializeField] private Image _tile;
    public int Rows;
    public int Columns;

    // Start is called before the first frame update
    void Start()
    {
        _tilePrefab = Resources.Load<GameObject>("Prefabs/TilePrefab");
        InitGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitGame()
    {
        TilesManager tilesManager = new TilesManager(Rows, Columns);
        tilesManager.Load();
        Debug.Log(tilesManager.Tiles.Length);
        for (int row = 0; row < Rows; row++)
        {
            for (int column = 0; column < Columns; column++)
            {
                var newTile = Instantiate(_tile);
                newTile.rectTransform.sizeDelta = new Vector2(300, 300);
                newTile.rectTransform.position = new Vector3(220 + 310 * column, -220 - 310 * row, 0);
                newTile.transform.SetParent(_boardGame.transform, false);
            }
        }
    }
}
