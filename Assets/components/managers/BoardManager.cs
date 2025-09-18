using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    private List<int[,]> mockUpLevel;
    public List<Tile[,]> board;
    public int remainTile;

    void LevelMock()
    {
        mockUpLevel = new List<int[,]>();

        int[,] layer1 =
        {
            { 1,0,20, 0, 0, 1 },
            {0,0, 0, 0, 0, 0 },
            {0,1, 0, 0, 20, 0 },
            { 0, 0, 0, 0 ,0,0},
        };
        mockUpLevel.Add(layer1);

        int[,] layer2 =
        {
            { 0,0,1, 0, 0, 1 },
            {1,0, 0, 0, 0, 0 },
            {0,0, 0, 2, 0, 0 },
            { 0, 0, 0, 0 ,0,2},
        };
        mockUpLevel.Add(layer2);
    }

    public void SetUp()
    {
        remainTile = 0;
        Debug.Log("aaa");
        // LevelMock();
        // mockUpLevel = GameManager.instance.currentLevel.layers;
        Debug.Log("llalaal");
        LevelLoader.PrintLevelGridData(GameManager.instance.currentLevel);
        mockUpLevel = LevelLoader.instance.getArray(GameManager.instance.currentLevel);
        Debug.Log("MOCK" + mockUpLevel);
        board = new List<Tile[,]>();

        for (int i = 0; i < mockUpLevel.Count; i++)
        {
            int[,] levelData = mockUpLevel[i];
            int rows = levelData.GetLength(0);
            int cols = levelData.GetLength(1);

            Tile[,] tileGrid = new Tile[rows, cols];

            for (int c = 0; c < cols; c++)
            {
                for (int r = 0; r < rows; r++)
                {

                    // Tạo tile từ prefab
                    Tile t = GameManager.instance.tilePool.GetFirstItem();
                    t.Reset();
                    t.transform.SetAsLastSibling();
                    // t.transform.SetSiblingIndex((c - c % 2) * rows + (r - r % 2) * cols + i * rows * cols);
                    tileGrid[r, c] = t;
                    t.layer = i;
                    t.coords = new Vector2Int(c, r);
                    // if (levelData[r, c] == 0)
                    // {
                    //     t.Kill();
                    // }
                }
            }


            board.Add(tileGrid);

        }
        for (int i = 0; i < mockUpLevel.Count; i++)
        {

            int rows = mockUpLevel[i].GetLength(0);
            int cols = mockUpLevel[i].GetLength(1);
            int[,] levelData = mockUpLevel[i];

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (board[i][r, c] != null)
                        (board[i][r, c].transform as RectTransform).anchoredPosition = GetPosFromCoords(c, r, i);
                    if (levelData[r, c] == 0)
                    {
                        board[i][r, c].Kill();
                        // board[i][r, c].gameObject.SetActive(false);

                    }
                    else
                    {

                        board[i][r, c].setTileType(levelData[r, c]);
                        board[i][r, c].Zoom(i);
                        remainTile++;
                    }
                }
            }
        }



    }

    public Vector2 GetPosFromCoords(int x, int y, int layerIndex)
    {
        int rows = board[layerIndex].GetLength(0);
        int cols = board[layerIndex].GetLength(1);

        float tileWidth = 125f / 2;
        float tileHeight = 155f / 2;

        // Calculate total grid size
        float totalWidth = cols * tileWidth;
        float totalHeight = rows * tileHeight;

        // Offset from center of grid
        float originX = -totalWidth / 2f + tileWidth / 2f;
        float originY = totalHeight / 2f - tileHeight / 2f;

        float posX = originX + (x * tileWidth);
        float posY = originY - (y * tileHeight);

        // Apply layer offset (up and left)
        float layerOffsetX = -tileWidth * 0.2f * layerIndex;  // shift left
        float layerOffsetY = tileHeight * 0.2f * layerIndex;  // shift up

        posX += layerOffsetX;
        posY += layerOffsetY;

        return new Vector2(posX, posY);
    }

}
