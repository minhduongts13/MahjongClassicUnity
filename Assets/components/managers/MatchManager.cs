using System;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using JetBrains.Annotations;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchManager : MonoBehaviour
{
    BoardManager board;
    [SerializeField] private Image blockNoti;
    public void SetUp()
    {
        board = GameManager.instance.board;
    }

    private Sequence blockSeq;

    public void ShowBlock(Tile tile, bool top = true)
    {
        if (blockSeq != null && blockSeq.IsActive()) blockSeq.Kill();
        Vector2 startPos = board.GetPosFromCoords(tile.coords.x, tile.coords.y, tile.layer) + new Vector2(0, 50);
        blockNoti.rectTransform.anchoredPosition = startPos;

        blockNoti.gameObject.SetActive(true);
        blockNoti.color = new Color(blockNoti.color.r, blockNoti.color.g, blockNoti.color.b, 1);

        // Reset alpha cho Image & TMP con
        foreach (var g in blockNoti.GetComponentsInChildren<TMP_Text>(true))
        {
            Color c = g.color;
            c.a = 1;
            g.color = c;

            g.text = top ? "Locked by above tiles" : "Locked by left and right";
        }

        blockSeq = DOTween.Sequence();
        blockSeq.Append(blockNoti.rectTransform.DOAnchorPos(startPos + new Vector2(0, 100), 1.5f).SetEase(Ease.OutCubic));
        blockSeq.Join(blockNoti.DOFade(0, 2f));

        foreach (var g in blockNoti.GetComponentsInChildren<Graphic>(true))
        {
            blockSeq.Join(g.DOFade(0, 2f));
        }

        blockSeq.OnComplete(() => blockNoti.gameObject.SetActive(false));
    }


    public async Task Match(Tile tile1, Tile tile2, bool move = true)
    {
        if (board.shuffling) return;

        if (TopBlock(tile1) || TopBlock(tile2))
        {
            Debug.Log("TOP BLOCKED");
            GameManager.instance.pointManager.OnNoMatch();
            //GameManager.instance.combo.ResetCombo();

            return;
        }
        if (SideBlock(tile1) || SideBlock(tile2))
        {
            GameManager.instance.pointManager.OnNoMatch();
            //GameManager.instance.combo.ResetCombo();

            return;
        }
        if (tile1.GetTileType() == tile2.GetTileType())
        {
            // tile1.Kill();
            GameManager.instance.UnChose();
            // tile2.Kill();
            addMove(tile1, tile2);
            tile1.setTileType(0);
            tile2.setTileType(0);
            board.remainTile -= 2;
            Debug.Log(board.remainTile);

            UnlockNeighbour(tile1);
            UnlockNeighbour(tile2);

            tile1.isDraggableNow = false;
            tile2.isDraggableNow = false;
            tile1.OffHint();
            tile2.OffHint();
            GameManager.instance.hinting = false;

            tile1.OnUnChose(move);
            tile2.OnUnChose(move);
            GameManager.instance.pointManager.OnMatchPoint();
            GameManager.instance.pointManager.OnChangeMatches();
            GameManager.instance.combo.addCombo();

            if (board.remainTile == 0)
            {
                await MoveMatching(tile1, tile2);
                UIManager.showWin();
                GameManager.instance.combo.ResetCombo();

                // GameManager.instance.AdvanceLevel();
                // GameManager.instance.Reload();
                // GameManager.instance.AdvanceLevel();
                // await GameManager.instance.Reload();
            }
            else
            {
                await MoveMatching(tile1, tile2);
            }
        }
    }


    private void UnlockNeighbour(Tile tile)
    {
        foreach (Tile t in board.getNeighbour(tile))
        {
            if (isFree(t)) t.ToggleOverlay(false);
        }
        foreach (Tile t in board.getUnder(tile))
        {
            if (isFree(t)) t.ToggleOverlay(false);
        }
    }

    public Vector2 GetMidPoint(Vector2 pos1, Vector2 pos2)
    {
        return (pos1 + pos2) / 2f;
    }
    public async Task MoveMatching(Tile tile1, Tile tile2)
    {
        tile1.BlockInput();
        tile2.BlockInput();
        RectTransform rt1 = tile1.transform as RectTransform;
        RectTransform rt2 = tile2.transform as RectTransform;
        Vector2 mid = GetMidPoint(rt1.anchoredPosition, rt2.anchoredPosition);
        bool left = tile1.coords.x < tile2.coords.x;
        if (left)
        {
            tile1.transform.SetAsLastSibling();
            tile2.transform.SetAsLastSibling();

        }
        else
        {
            tile2.transform.SetAsLastSibling();
            tile1.transform.SetAsLastSibling();
        }
        await Task.WhenAll(TileMoveMatching(tile1, mid, left), TileMoveMatching(tile2, mid, !left));
        if (tile1.GetTileType() == 0) tile1.Kill();
        if (tile2.GetTileType() == 0) tile2.Kill();

    }

    public async Task TileMoveMatching(Tile tile, Vector2 mid, bool left)
    {
        RectTransform rt = tile.transform as RectTransform;
        // DOTween.Kill(rt);
        Sequence seq = DOTween.Sequence();
        await seq.Append(rt.DOAnchorPos(mid + new Vector2(left ? -300 : 300, 0), 0.35f))
        .Append(rt.DOAnchorPos(mid + new Vector2(left ? -125 / 2 : 125 / 2, 0), 0.3f).SetEase(Ease.OutFlash)).AsyncWaitForCompletion();
        await tile.FadeTile(left);
    }

    public bool isFree(Tile tile)
    {
        if (TopBlock(tile)) return false;
        if (SideBlock(tile)) return false;
        return true;
    }

    public bool BlockLeft(Tile tile)
    {
        int layer = tile.layer;
        if (tile.coords.x < 2) return false;
        for (int i = -1; i < 2; i++)
        {
            if (tile.coords.y + i < 0 || tile.coords.y + i >= board.board[0].GetLength(0)) continue;
            if (board.board[layer][tile.coords.y + i, tile.coords.x - 2].GetTileType() != 0)
            {
                return true;
            }
        }
        return false;
    }

    private void addMove(Tile tile1, Tile tile2)
    {
        // Lưu: ( (pos1, pos2), (type1, type2) )

        Vector3 pos1 = new Vector3(tile1.coords.x, tile1.coords.y, tile1.layer);
        Vector3 pos2 = new Vector3(tile2.coords.x, tile2.coords.y, tile2.layer);
        var move = Tuple.Create(
            Tuple.Create(pos1, pos2),
            Tuple.Create(tile1.GetTileType(), tile2.GetTileType())
        );
        Debug.Log(move);

        GameManager.instance.moves.Push(move);
    }

    public bool BlockRight(Tile tile)
    {
        int layer = tile.layer;
        if (tile.coords.x >= board.board[0].GetLength(1) - 2) return false;
        for (int i = -1; i < 2; i++)
        {
            if (tile.coords.y + i < 0 || tile.coords.y + i >= board.board[0].GetLength(0)) continue;
            if (board.board[layer][tile.coords.y + i, tile.coords.x + 2].GetTileType() != 0)
            {
                return true;
            }
        }
        return false;
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
                if (board.board[tile.layer + 1][tile.coords.y + y, tile.coords.x + x].GetTileType() != 0)
                {
                    // Debug.Log("x" + (tile.coords.x + x) + "y" + (tile.coords.y + y));
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
            if (board.board[layer][tile.coords.y + i, tile.coords.x - 2].GetTileType() != 0)
            {
                blockedLeft = true;
                break;
            }
        }

        // check bên phải
        for (int i = -1; i < 2; i++)
        {
            if (tile.coords.y + i < 0 || tile.coords.y + i >= board.board[0].GetLength(0)) continue;
            if (board.board[layer][tile.coords.y + i, tile.coords.x + 2].GetTileType() != 0)
            {
                blockedRight = true;
                break;
            }
        }

        // chỉ bị block nếu cả 2 bên đều chặn
        return blockedLeft && blockedRight;
    }

}
