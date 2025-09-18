using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Tile : PooledObject
{
    [SerializeField] List<Image> choseEffect;
    [SerializeField] Image typeImg;
    [SerializeField] Image bgImg;

    [SerializeField] Image shadowImg;
    public Vector2Int coords;
    public int layer;
    private THEME theme = THEME.Green;
    private int type = 0;
    private List<Action> OnClickCallbacks = new List<Action>();
    void Start()
    {
        // SetTheme(THEME.Green);
        // setTileType(40);
    }
    void SetUp()
    {
        AddOnClickCallbacks(() => GameManager.instance.Chose(this));
    }

    public async Task FadeTile()
    {
        await Task.WhenAll(
            bgImg.DOFade(0, 0.2f).AsyncWaitForCompletion(),
            typeImg.DOFade(0, 0.2f).AsyncWaitForCompletion(),
            shadowImg.DOFade(0, 0.2f).AsyncWaitForCompletion()
        );
    }

    public void Reset()
    {
        bgImg.DOFade(1, 0.01f);
        typeImg.DOFade(1, 0.01f);
        shadowImg.DOFade(1, 0.01f);
        TurnOnInput();
    }

    public async void Zoom(int delay)
    {
        transform.localScale = new Vector3(0, 0);
        await Task.Delay(delay * 200);
        transform.DOScale(1, 0.2f);
    }

    public void BlockInput()
    {
        Button button = this.gameObject.GetComponent<Button>();
        button.interactable = false;
    }

    public void TurnOnInput()
    {
        Button button = this.gameObject.GetComponent<Button>();
        button.interactable = true;
    }

    public void MoveToRealPos()
    {
        Vector2 pos = GameManager.instance.board.GetPosFromCoords(this.coords.x, this.coords.y, this.layer);
        RectTransform rt = transform as RectTransform;
        rt.DOAnchorPos(pos, 0.35f).SetEase(Ease.OutSine);
    }


    public void AddOnClickCallbacks(Action callback)
    {

        OnClickCallbacks.Add(callback);
    }
    public void EmitOnclick()
    {
        if (OnClickCallbacks.Count == 0) SetUp();
        foreach (Action cb in OnClickCallbacks)
        {
            cb();
        }
    }

    public void OnChose()
    {

        foreach (Image c in choseEffect)
        {
            DOTween.Kill(c);
            c.gameObject.SetActive(true);

            c.DOFade(1f, 0.5f);
        }

    }

    public void MoveOffset(bool left, bool move)
    {
        if (!move) return;
        int offset = left ? -25 : 25;
        RectTransform rt = transform as RectTransform;
        rt.DOAnchorPos(rt.anchoredPosition + new Vector2(offset, 0), 0.35f).SetEase(Ease.OutSine);
    }


    public THEME GetTheme()
    {
        return theme;
    }

    public int GetTileType()
    {
        return type;
    }
    public void setTileType(int ttype)
    {
        type = ttype;
        if (ttype == 0) return;
        string PATH = global.GetSprite(type, theme);
        typeImg.sprite = AssetsLoader.instance.LoadSprite(PATH);
    }

    public void SetTheme(THEME th)
    {
        theme = th;
        string PATH = global.GetSprite(type, theme);
        typeImg.sprite = AssetsLoader.instance.LoadSprite(PATH);
    }

    public void OnUnChose()
    {
        foreach (Image c in choseEffect)
        {
            DOTween.Kill(c);
            c.DOFade(0, 0.5f).OnComplete(() =>
            {
                c.gameObject.SetActive(false);
            });
        }

        MoveToRealPos();

    }


}
