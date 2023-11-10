using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private GameObject _tilePrefab;

    [SerializeField] private Canvas _boardGame;
    [SerializeField] private Image _tile;
    [SerializeField] private Button btnLevel1;
    [SerializeField] private Button btnLevel2;
    [SerializeField] private Button btnNewGame;
    private int rows;
    private int columns;
    public GameLevel GameLevel;
    private Sprite bg;
    private Dictionary<string, Sprite> imgSprites = new();
    private Image FirstTile;
    private int countMatching = 0;
    [SerializeField] private Canvas popupWin;
    //private Image SecondTile;

    // Start is called before the first frame update
    void Start()
    {
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
        //_tilePrefab = Resources.Load<GameObject>("Prefabs/TilePrefab");
        InitGame();
    }

    // Update is called once per frame
    void Update()
    {
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
        countMatching = 0;
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
        TilesManager tilesManager = new TilesManager(GameLevel);
        tilesManager.Load();
        Vector3 startPosition = new Vector3(-240, 420, 0);
        var setting = tilesManager.LevelSettings.FirstOrDefault(x => x.Level == GameLevel);
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
                newTile.sprite = bg;
                var txt = newTile.transform.Find("Text")
                    .GetComponent<TextMeshProUGUI>();
                var btn = newTile.transform.Find("Button")
                    .GetComponent<Button>();
                string val = tilesManager.Tiles[row, column];
                btn.onClick.AddListener(delegate { TileClick(newTile, val); });
                //txt.text = val;
                newTile.transform.SetParent(_boardGame.transform, false);
            }
        }
    }

    private void TileClick(Image item, string value)
    {
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
        Thread.Sleep(500);
        if (FirstTile.sprite.name != item.sprite.name)
        {
            FirstTile.sprite = bg;
            item.sprite = bg;
        }
        else
        {
            countMatching += 2;
            Debug.Log(countMatching);
            if (countMatching >= (rows * columns))
            {
                popupWin.gameObject.SetActive(true);
            }
        }
        FirstTile = null;
    }
}
