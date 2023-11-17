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

    private PikachuManager _pikachuManager;
    //private Image SecondTile;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 v1 = new Vector3(-210, 2000, 0);
        Vector3 v2 = new Vector3(2000, 350, 0);
        LineRenderer lineRenderer = _boardGame.gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        lineRenderer.sortingOrder = 1;
        lineRenderer.SetPositions(new Vector3[] { v1, v2 });
        //lineRenderer.transform.SetParent(_boardGame.transform, false);

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


    private void InitPikachu(TilesManager tilesManager)
    {
        _pikachuManager = new PikachuManager(tilesManager);
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
    //private void PikachuTileClick(Image item, string value)
    //{
    //    var txt = item.transform.Find("Text")
    //        .GetComponent<TextMeshProUGUI>();
    //    var arr = txt.text.Split("-").ToList()
    //        .ConvertAll(x => int.Parse(x));
    //    int row = arr[0];
    //    int col = arr[1];

    //    if (FirstTile is null)
    //    {
    //        var outline = item.transform.GetComponent<Outline>();
    //        outline.enabled = true;
    //        FirstTile = item;
    //        FirstPos = new Vector2(col, row);
    //        return;
    //    }
    //    if (FirstTile == item)
    //    {
    //        var outline = item.transform.GetComponent<Outline>();
    //        outline.enabled = false;
    //        FirstTile = null;
    //        FirstPos = null;
    //        return;
    //    }
    //    // Difference image.
    //    if (FirstTile.sprite.name != item.sprite.name)
    //    {
    //        var outline = FirstTile.transform.GetComponent<Outline>();
    //        var outline1 = item.transform.GetComponent<Outline>();
    //        outline.enabled = false;
    //        outline1.enabled = false;
    //        FirstTile = null;
    //        FirstPos = null;
    //        return;
    //    }
    //    // Choose 2 item in 1 column.
    //    if (FirstPos.Value.x == col)
    //    {
    //        if (FirstPos.Value.x == 0 || // Column start.
    //            col == columns - 1 || // Column end.
    //            FirstPos.Value.y + 1 == row || // Next column.
    //            FirstPos.Value.y - 1 == row) // Prev column.
    //        {
    //            FirstTile.gameObject.SetActive(false);
    //            item.gameObject.SetActive(false);
    //            item = null;
    //            FirstTile = null;
    //            FirstPos = null;
    //            return;
    //        }
    //        // Find line top
    //        if (FirstPos.Value.y + 1 > row)
    //        {
    //            TilesManager tilesManager = new TilesManager(GameLevel, GameType);
    //            var setting = tilesManager.LevelSettings.FirstOrDefault(x => x.Level == GameLevel);

    //            var ss = _boardGame.transform.GetComponentsInChildren<Image>().Where(x => x.name == "TempTile(Clone)");
    //            if (ss != null)
    //            {
    //                for (int i = (int)FirstPos.Value.y - 1; i > row; i--)
    //                {
    //                    // txt.text = $"{row}-{column}"
    //                    var nextItem = ss.FirstOrDefault(x => x.transform.Find("Text")
    //                        .GetComponent<TextMeshProUGUI>().text == $"{i}-{col}");
    //                    // If has item in this position
    //                    // 1 line
    //                    if (nextItem != null)
    //                    {
    //                        var outline = item.transform.GetComponent<Outline>();
    //                        var outline1 = FirstTile.transform.GetComponent<Outline>();
    //                        outline.enabled = false;
    //                        outline1.enabled = false;
    //                        FirstTile = null;
    //                        FirstPos = null;
    //                        return;
    //                    }
    //                }
    //                FirstTile.gameObject.SetActive(false);
    //                item.gameObject.SetActive(false);
    //                item = null;
    //                FirstTile = null;
    //                FirstPos = null;
    //            }
    //            return;
    //        }
    //        // Find line bottom
    //        if (FirstPos.Value.y + 1 < row)
    //        {
    //            TilesManager tilesManager = new TilesManager(GameLevel, GameType);
    //            var setting = tilesManager.LevelSettings.FirstOrDefault(x => x.Level == GameLevel);

    //            var ss = _boardGame.transform.GetComponentsInChildren<Image>().Where(x => x.name == "TempTile(Clone)");
    //            if (ss != null)
    //            {
    //                for (int i = (int)FirstPos.Value.y + 1; i < row; i++)
    //                {
    //                    // txt.text = $"{row}-{column}"
    //                    var nextItem = ss.FirstOrDefault(x => x.transform.Find("Text")
    //                        .GetComponent<TextMeshProUGUI>().text == $"{i}-{col}");
    //                    // If has item in this position
    //                    // 1 line
    //                    if (nextItem != null)
    //                    {
    //                        var outline = item.transform.GetComponent<Outline>();
    //                        var outline1 = FirstTile.transform.GetComponent<Outline>();
    //                        outline.enabled = false;
    //                        outline1.enabled = false;
    //                        return;
    //                    }
    //                }
    //                FirstTile.gameObject.SetActive(false);
    //                item.gameObject.SetActive(false);
    //                item = null;
    //                FirstTile = null;
    //                FirstPos = null;
    //            }
    //            return;
    //        }
    //    }
    //    // Choose 2 item in 1 row.
    //    if (FirstPos.Value.y == row)
    //    {
    //        if (FirstPos.Value.y == 0 ||
    //            row == rows - 1 ||
    //            FirstPos.Value.x + 1 == col ||
    //            FirstPos.Value.x - 1 == col)
    //        {
    //            FirstTile.gameObject.SetActive(false);
    //            item.gameObject.SetActive(false);
    //            item = null;
    //            FirstTile = null;
    //            FirstPos = null;
    //            return;
    //        }
    //        if (FirstPos.Value.x + 1 > col)
    //        {
    //            TilesManager tilesManager = new TilesManager(GameLevel, GameType);
    //            var setting = tilesManager.LevelSettings.FirstOrDefault(x => x.Level == GameLevel);

    //            var ss = _boardGame.transform.GetComponentsInChildren<Image>().Where(x => x.name == "TempTile(Clone)");
    //            if (ss != null)
    //            {
    //                for (int i = (int)FirstPos.Value.x - 1; i > col; i--)
    //                {
    //                    // txt.text = $"{row}-{column}"
    //                    var nextItem = ss.FirstOrDefault(x => x.transform.Find("Text")
    //                        .GetComponent<TextMeshProUGUI>().text == $"{i}-{col}");
    //                    // If has item in this position
    //                    // 1 line
    //                    if (nextItem != null)
    //                    {
    //                        FirstTile.gameObject.SetActive(false);
    //                        item.gameObject.SetActive(false);
    //                        item = null;
    //                        FirstTile = null;
    //                        FirstPos = null;
    //                        return;
    //                    }
    //                }
    //                FirstTile.gameObject.SetActive(false);
    //                item.gameObject.SetActive(false);
    //                item = null;
    //                FirstTile = null;
    //                FirstPos = null;
    //            }
    //            return;
    //        }
    //        // Find line bottom
    //        if (FirstPos.Value.x + 1 < col)
    //        {
    //            TilesManager tilesManager = new TilesManager(GameLevel, GameType);
    //            var setting = tilesManager.LevelSettings.FirstOrDefault(x => x.Level == GameLevel);

    //            var ss = _boardGame.transform.GetComponentsInChildren<Image>().Where(x => x.name == "TempTile(Clone)");
    //            if (ss != null)
    //            {
    //                for (int i = (int)FirstPos.Value.x + 1; i < col; i++)
    //                {
    //                    // txt.text = $"{row}-{column}"
    //                    var nextItem = ss.FirstOrDefault(x => x.transform.Find("Text")
    //                        .GetComponent<TextMeshProUGUI>().text == $"{i}-{col}");
    //                    // If has item in this position
    //                    // 1 line
    //                    if (nextItem != null)
    //                    {
    //                        FirstTile.gameObject.SetActive(false);
    //                        item.gameObject.SetActive(false);
    //                        item = null;
    //                        FirstTile = null;
    //                        FirstPos = null;
    //                        return;
    //                    }
    //                }
    //                FirstTile.gameObject.SetActive(false);
    //                item.gameObject.SetActive(false);
    //                item = null;
    //                FirstTile = null;
    //                FirstPos = null;
    //            }
    //            return;
    //        }
    //    }
    //    // 2 item difference columns.
    //    if (FirstPos.Value.x != col)
    //    {
    //        return;
    //    }
    //    // 2 item difference rows.
    //    if (FirstPos.Value.y != row)
    //    {
    //        return;
    //    }
    //}

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
        var ss = _pikachuManager.CheckTwoPoint(FirstPos.Value, new Vector2(row, col));
        Debug.Log(ss?.toString());
        if (ss is null)
        {
            var outline = FirstTile.transform.GetComponent<Outline>();
            var outline1 = item.transform.GetComponent<Outline>();
            outline.enabled = false;
            outline1.enabled = false;
            FirstTile = null;
            FirstPos = null;
            return;
        }
        if (_pikachuManager.CheckWin())
        {
            popupWin.gameObject.SetActive(true);
        }
        FirstTile.gameObject.SetActive(false);
        item.gameObject.SetActive(false);
        item = null;
        FirstTile = null;
        FirstPos = null;
        return;
    }
}