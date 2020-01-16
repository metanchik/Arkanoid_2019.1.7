using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelController : MonoBehaviour {
    public int lives = 3;
    private float levelTime;
    private int rowsCount = 15;
    private int columnsCount = 6;
    private int currentBlockCount = 0;

    public Block[][] blocks;
    public Vector3 blockSize;
    public GameObject completeWindow;
    public GameObject failWindow;
    public PlatformController platform;
    public LivesPanelScript livesPanel;
    public FieldController field;

    public int RowsCount => rowsCount;
    public int ColumnsCount => columnsCount;

    public List<BonusController> unfrozenBonuses = new List<BonusController>();
    public List<BallController> balls = new List<BallController>();

    void Awake() {
        blocks = new Block[rowsCount][];
        string config = LevelConfigs.GetLevelConfig(GameController.currentLevelIndex);
        BuildBlocks(config);
        platform = FindObjectOfType<PlatformController>();
        livesPanel = FindObjectOfType<LivesPanelScript>();
        field = FindObjectOfType<FieldController>();
        balls = FindObjectsOfType<BallController>().ToList();
    }

    private void BuildBlocks(string config) {
        string[] rows = config.Split(new char[] {'.'});
        for (int i = 0; i < rowsCount; i++) {
            List<string> cells = rows[i].Split(new char[] {'/'}).ToList();
            cells.RemoveAll(s => s.Equals(""));
            blocks[i] = new Block[columnsCount];
            if (cells.Count == 1) {
                if (!cells[0][0].Equals('-')) {
                    Block mainBlock = GetBlockByConfig(cells[0], i, 0);

                    for (int j = 0; j < columnsCount; j++) {
                        // Block block = new Block(cellColor, lives, i, j);
                        Block block = new Block(mainBlock.color, mainBlock.lives, i, j);
                        blocks[i][j] = block;
                        currentBlockCount += 1;
                    }
                }
            }
            else {
                for (int k = 0; k < cells.Count; k++) {
                    if (!cells[k][0].Equals('-')) {
                        Block block = GetBlockByConfig(cells[k], i, k);

                        blocks[i][k] = block;
                        currentBlockCount += 1;
                    }
                }
            }
        }

        CalculateBlockCenters();
    }

    private Block GetBlockByConfig(string conf, int i, int j) {
        Color cellColor = GetColor(conf[0]);
        int lives = 1;
        Bonus bonus = null;
        if (conf.Length > 1) {
            if (char.IsNumber(conf[1])) {
                lives = int.Parse(conf[1].ToString());
            }
            else {
                bonus = LevelConfigs.GetBonus(conf[1].ToString());
            }

            if (conf.Length == 3) {
                bonus = LevelConfigs.GetBonus(conf[2].ToString());
            }
        }

        Block res = new Block(cellColor, lives, i, j);
        if (bonus != null)
            res.SetBonus(bonus);
        return res;
    }

    private Color GetColor(char colorConfig) {
        Color color;
        switch (colorConfig) {
            case 'r': {
                color = Color.red;
                break;
            }
            case 'g': {
                color = Color.green;
                break;
            }
            case 'b': {
                color = Color.blue;
                break;
            }
            case 'y': {
                color = Color.yellow;
                break;
            }
            case 'c': {
                color = Color.cyan;
                break;
            }
            case 'm': {
                color = Color.magenta;
                break;
            }
            default: {
                color = Color.black;
                break;
            }
        }

        return color;
    }

    public void CalculateBlockCenters() {
        GameObject field = GameObject.Find("Field");
        Vector3 fieldPos = field.transform.position;
        Vector3 fieldSize = field.transform.localScale;
        float leftGrid = fieldPos.x - fieldSize.x / 2 + fieldSize.x / 5;
        float topGrid = fieldPos.y + fieldSize.y / 2 - fieldSize.y / 6;

        float gridWeight = fieldSize.x * 3 / 5;
        float gridHeight = fieldSize.y / 4;

        float blockWidth = gridWeight / columnsCount;
        float blockHeight = gridHeight / rowsCount;

        float firstBlockCenterX = leftGrid + blockWidth / 2;
        float firstBlockCenterY = topGrid - blockHeight / 2;

        blockSize = new Vector3(blockWidth, blockHeight, 1f);

        for (int i = 0; i < rowsCount; i++) {
            for (int j = 0; j < columnsCount; j++) {
                if (blocks[i][j] != null) {
                    float centerX = firstBlockCenterX + blockWidth * j;
                    float centerY = firstBlockCenterY - blockHeight * i;

                    blocks[i][j].BuildBlock(new Vector3(centerX, centerY, -0.1f), blockSize, "Block_" + i + "_" + j);
                }
            }
        }
    }

    public void RemoveBlock(int i, int j) {
        blocks[i][j] = null;
        currentBlockCount -= 1;
        CheckLevelCompleted();
    }

    public void CheckLevelCompleted() {
        if (currentBlockCount == 0) {
            Debug.Log("Complete level!)");
            FindObjectsOfType<BallController>().ToList().ForEach(b => b.speed = 0);
            completeWindow.SetActive(true);
        }
    }

    public void LoseLife(BallController ball) {
        if (balls.Count == 1) {
            lives -= 1;
            platform.SetStartParameters();
            livesPanel.Actualize();
            unfrozenBonuses.ForEach(b => Destroy(b.gameObject));
            unfrozenBonuses.Clear();
            if (lives < 1) {
                failWindow.SetActive(true);
            }
            else
                ball.SetStartBallParams();
        }
        else {
            balls.Remove(ball);
            Destroy(ball.gameObject);
        }
    }

    public void CloneBalls() {
        List<BallController> newBalls = new List<BallController>();
        balls.ForEach(ball => {
            GameObject newBallObj = Instantiate(ball.gameObject);
            BallController newBall = newBallObj.GetComponent<BallController>();
            ball.CloneTo(newBall);
            newBall.SetDeviation(20f);
            newBalls.Add(newBall);
        });
        balls.AddRange(newBalls);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }
}