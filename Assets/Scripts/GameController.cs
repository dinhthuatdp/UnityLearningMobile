using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class GameController : MonoBehaviour
{
    private GameObject _tilePrefab;

    [SerializeField] private Canvas _boardGame;
    [SerializeField] private Image _tile;
    [SerializeField] private Button btnLevel1;
    [SerializeField] private Button btnLevel2;
    [SerializeField] private Button btnNewGame;
    [SerializeField] private Button btnGameType;
    private int rows;
    private int columns;
    public GameLevel GameLevel;
    public GameType GameType;
    private Sprite bg;
    private Dictionary<string, Sprite> imgSprites = new();
    private Image FirstTile;
    private int countMatching = 0;
    [SerializeField] private Canvas popupWin;

    private PikachuManager1 _pikachuManager;
    //private Image SecondTile;

    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = _boardGame.gameObject.AddComponent<LineRenderer>();
        //Vector3 v1 = new Vector3(-210, 2000, 0);
        //Vector3 v2 = new Vector3(2000, 350, 0);
        //DrawLine(new Vector3[] { v1, v2 });
        btnGameType.onClick.AddListener(GameTypeClick);

        var outline = _tile.transform.GetComponent<Outline>();
        outline.enabled = false;

        popupWin.gameObject.SetActive(false);
        btnNewGame.onClick.AddListener(delegate
        {
            NewGameClick();
        });
        btnLevel1.onClick.AddListener(delegate
        {
            ButtonLevelClick(GameLevel.Level1);
        });
        btnLevel2.onClick.AddListener(delegate
        {
            ButtonLevelClick(GameLevel.Level2);
        });
        var btn = popupWin.transform.Find("Button")
            .GetComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            popupWin.gameObject.SetActive(false);
        });
        GameLevel = GameLevel.Level1;
        bg = Resources.Load<Sprite>("bg");
        for (int i = 1; i <= 4; i++)
        {
            imgSprites.Add($"img{i}", Resources.Load<Sprite>($"img{i}"));
        }
        for (int i = 1; i <= 5; i++)
        {
            imgSprites.Add($"p{i}", Resources.Load<Sprite>($"p{i}"));
        }
        //_tilePrefab = Resources.Load<GameObject>("Prefabs/TilePrefab");
        InitGame();
    }
    LineRenderer _lineRenderer;
    private void DrawLine(Vector3[] positions)
    {
        _lineRenderer.positionCount = positions.Length;
        _lineRenderer.startColor = Color.red;
        _lineRenderer.endColor = Color.red;
        _lineRenderer.sortingOrder = 1;
        _lineRenderer.startWidth = 9;
        _lineRenderer.endWidth = 9;
        _lineRenderer.SetPositions(positions);
        //lineRenderer.transform.SetParent(_boardGame.transform, false);
    }


    private void InitPikachu(TilesManager tilesManager)
    {
        _pikachuManager = new PikachuManager1(tilesManager.Tiles);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void GameTypeClick()
    {
        var txt = btnGameType.transform.Find("Text")
            .GetComponent<TextMeshProUGUI>();
        if (GameType == GameType.Matching)
        {
            GameLevel = GameLevel.Level2;
            txt.text = "Matching game";
            GameType = GameType.Pikachu;
        }
        else
        {
            GameLevel = GameLevel.Level1;
            txt.text = "Pikachu game";
            GameType = GameType.Matching;
        }
        ClearBoardGame();
        InitGame();
    }

    private void ClearBoardGame()
    {
        for (int i = 0; i < _boardGame.transform.childCount; i++)
        {
            if (_boardGame.transform.GetChild(i).GetComponent<Image>().name == "Background")
            {
                continue;
            }
            Destroy(_boardGame.transform.GetChild(i).gameObject);
        }
    }

    private void NewGameClick()
    {
        ClearBoardGame();
        InitGame();
    }

    private void ButtonLevelClick(GameLevel gameLevel)
    {
        if (gameLevel == GameLevel)
        {
            return;
        }
        GameLevel = gameLevel;
        ClearBoardGame();
        InitGame();
    }

    private void InitGame()
    {
        countMatching = 0;
        FirstTile = null;
        FirstPos = null;
        TilesManager tilesManager = new TilesManager(GameLevel, GameType);
        tilesManager.Load();
        InitPikachu(tilesManager);
        var setting = tilesManager.LevelSettings.FirstOrDefault(x => x.Level == GameLevel);
        Vector3 startPosition = new Vector3(-210, 350, 0);
        if (GameLevel == GameLevel.Level2)
        {
            startPosition = new Vector3(-260, 355, 0);
        }
        rows = setting.Rows;
        columns = setting.Columns;
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                var newTile = Instantiate(_tile);
                newTile.rectTransform.sizeDelta = new Vector2(setting.Width, setting.Width);
                newTile.rectTransform.position = new Vector3(startPosition.x + (setting.Width + setting.Offset + 70) * column,
                    startPosition.y - (setting.Width + setting.Offset + 70) * row,
                    startPosition.z);
                var txt = newTile.transform.Find("Text")
                    .GetComponent<TextMeshProUGUI>();
                var btn = newTile.transform.Find("Button")
                    .GetComponent<Button>();
                string val = tilesManager.Tiles[row, column];
                if (GameType == GameType.Matching)
                {
                    newTile.sprite = bg;
                }
                else
                {
                    newTile.sprite = imgSprites[val];
                }
                btn.onClick.AddListener(delegate { TileClick(newTile, val); });
                txt.text = $"{row}-{column}";// val;
                newTile.transform.SetParent(_boardGame.transform, false);
            }
        }
    }
    private bool isCanClickTile = true;
    private void TileClick(Image item, string value)
    {
        if (GameType == GameType.Pikachu)
        {
            PikachuTileClick(item, value);
            return;
        }
        if (!isCanClickTile) return;

        item.sprite = imgSprites[value];
        if (FirstTile is null)
        {
            FirstTile = item;
            return;
        }
        if (FirstTile == item)
        {
            return;
        }
        // Thread.Sleep(500);
        if (FirstTile.sprite.name != item.sprite.name)
        {

            StartCoroutine(_ActionDelay(0.5f, () => {
                if (FirstTile) FirstTile.sprite = bg;
                item.sprite = bg;

                FirstTile = null;
                isCanClickTile = true;
            }));
        }
        else
        {
            countMatching += 2;
            if (countMatching >= (rows * columns))
            {
                popupWin.gameObject.SetActive(true);
            }

            FirstTile = null;
            isCanClickTile = true;
        }
    }

    private IEnumerator _ActionDelay(float delay, Action callback)
    {
        yield return new WaitForSeconds(delay);
        callback?.Invoke();
    }

    Vector2? FirstPos = null;

    private void PikachuTileClick(Image item, string value)
    {
        var txt = item.transform.Find("Text")
            .GetComponent<TextMeshProUGUI>();
        var arr = txt.text.Split("-").ToList()
            .ConvertAll(x => int.Parse(x));
        int row = arr[0];
        int col = arr[1];

        if (FirstTile is null)
        {
            var outline = item.transform.GetComponent<Outline>();
            outline.enabled = true;
            FirstTile = item;
            FirstPos = new Vector2(row, col);
            return;
        }
        if (FirstTile == item)
        {
            var outline = item.transform.GetComponent<Outline>();
            outline.enabled = false;
            FirstTile = null;
            FirstPos = null;
            return;
        }
        // Difference image.
        if (FirstTile.sprite.name != item.sprite.name)
        {
            var outline = FirstTile.transform.GetComponent<Outline>();
            var outline1 = item.transform.GetComponent<Outline>();
            outline.enabled = false;
            outline1.enabled = false;
            FirstTile = null;
            FirstPos = null;
            return;
        }
        //var ss = _pikachuManager.FindPath((0,0),(4,4));
        var ss = _pikachuManager.FindPath(FirstPos.Value, new Vector2(row, col));
        if (ss != null)
        {
            // -210, 350
            /*
             *  x = -260 + y * 180
             *  y = 355 - x * 180"
             */
            var positions = ss.Select(x => new Vector3(-260 + (x.y - 1) * (180) + 524, 355 - (x.x - 1) * (180) + 919 + (200 - x.x * 50), 0)).ToArray();
            foreach (var p in ss)
            {
                Debug.Log($"{p.x} - {p.y}");
            }
            foreach (var p in positions)
            {
                Debug.Log($"{p.x} - {p.y}");
            }
            DrawLine(positions);
            StartCoroutine(_ActionDelay(0.5f, () => {
                FirstTile.gameObject.SetActive(false);
                item.gameObject.SetActive(false);
                item = null;
                FirstTile = null;
                FirstPos = null;
                countMatching += 2;
                _lineRenderer.positionCount = 0;
            }));
            if (countMatching >= (rows * columns))
            {
                popupWin.gameObject.SetActive(true);
            }
            return;
        }
        var outline3 = item.transform.GetComponent<Outline>();
        var outline4 = FirstTile.transform.GetComponent<Outline>();
        outline3.enabled = false;
        outline4.enabled = false;
        item = null;
        FirstTile = null;
        FirstPos = null;
        return;
    }
}