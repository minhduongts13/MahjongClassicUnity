using JetBrains.Annotations;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;

public class MatchManager : MonoBehaviour
{
    BoardManager board;
    public void SetUp()
    {
        board = GameManager.instance.board;
    }
    public void Match(Tile tile1, Tile tile2)
    {
        if (TopBlock(tile1) || TopBlock(tile2))
        {
            Debug.Log("TOP BLOCKED");
            return;
        }
        if (SideBlock(tile1) || SideBlock(tile2))
        {
            return;
        }
        if (tile1.GetTileType() == tile2.GetTileType())
        {
            tile1.Kill();
            tile2.Kill();
        }
    }

    public bool TopBlock(Tile tile)
    {
        if (tile.layer == board.board.Count - 1)
        {
            return false;
        }
        for (int y = -1; y < 2; y++)
        {
            if (tile.coords.y + y < 0 || tile.coords.y + y >= board.board[0].GetLength(0)) continue;
            for (int x = -1; x < 2; x++)
            {
                if (tile.coords.x + x < 0 || tile.coords.x + x >= board.board[0].GetLength(1)) continue;
                if (board.board[tile.layer + 1][tile.coords.y + y, tile.coords.x + x].used)
                {
                    Debug.Log("x" + (tile.coords.x + x) + "y" + (tile.coords.y + y));
                    // board.board[tile.layer + 1][tile.coords.y + y, tile.coords.x + x].OnChose();
                    return true;
                }
            }
        }
        return false;
    }

    public bool SideBlock(Tile tile)
    {
        int layer = tile.layer;
        bool blockedLeft = false;
        bool blockedRight = false;

        // nếu ở mép thì mặc định không block
        if (tile.coords.x < 2) return false;
        if (tile.coords.x >= board.board[0].GetLength(1) - 2) return false;

        // check bên trái
        for (int i = -1; i < 2; i++)
        {
            if (tile.coords.y + i < 0 || tile.coords.y + i >= board.board[0].GetLength(0)) continue;
            if (board.board[layer][tile.coords.y + i, tile.coords.x - 2].used)
            {
                blockedLeft = true;
                break;
            }
        }

        // check bên phải
        for (int i = -1; i < 2; i++)
        {
            if (tile.coords.y + i < 0 || tile.coords.y + i >= board.board[0].GetLength(0)) continue;
            if (board.board[layer][tile.coords.y + i, tile.coords.x + 2].used)
            {
                blockedRight = true;
                break;
            }
        }

        // chỉ bị block nếu cả 2 bên đều chặn
        return blockedLeft && blockedRight;
    }

}
