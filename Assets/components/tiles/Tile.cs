using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;

using UnityEngine;
using UnityEngine.UI;

public class Tile : PooledObject
{
    [SerializeField] List<Image> choseEffect;
    [SerializeField] Image typeImg;
    [SerializeField] Image bgImg;
    [SerializeField] Image overlay;
    [SerializeField] Image hintEff;
    [SerializeField] Image hintRot;
    [SerializeField] Image explodeEff;
    [SerializeField] List<ParticleSystem> explodeParticle;

    [SerializeField] Image hintHighlight;

    [SerializeField] Image shadowImg;
    public Vector2Int coords;
    private int offset = 0;
    public int layer;
    private THEME theme = THEME.Green;
    private int type = 0;
    public int idx;
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
    public async Task Explode(bool left)
    {
        if (explodeEff == null) return;
        if (!left) return;

        // Random index
        System.Random rand = new System.Random();
        int idx = rand.Next(explodeParticle.Count);

        // Bật particle random
        explodeParticle[idx].gameObject.SetActive(true);
        explodeParticle[idx].Play();

        explodeEff.transform.localScale = new Vector3(0, 0);
        explodeEff.gameObject.SetActive(true);
        explodeEff.transform.DOScale(1, 0.2f);

        await explodeEff.DOFade(0.6f, 0.1f).AsyncWaitForCompletion();
        await explodeEff.DOFade(0f, 0.1f).AsyncWaitForCompletion();

        explodeParticle[idx].gameObject.SetActive(false);
        explodeEff.gameObject.SetActive(false);
    }

    public void OnBlocked()
    {
        BoardManager board = GameManager.instance.board;
        if (!GameManager.instance.matchManager.isFree(this))
        {
            ToggleOverlay(true);
            this.Shake();
            foreach (Tile t in GameManager.instance.board.getNeighbour(this))
            {
                t.Shake();
            }
        }

    }
    public void ToggleOverlay(bool on)
    {
        DOTween.Kill(overlay);
        if (on)
        {
            if (overlay.gameObject.activeSelf != true) overlay.DOFade(0, 0.01f);
            overlay.gameObject.SetActive(true);
            overlay.DOFade(0.6f, 0.2f);
        }
        else
        {
            overlay.DOFade(0f, 0.2f).OnComplete(() =>
            {
                overlay.gameObject.SetActive(false);
            });
        }

    }

    public async void Shake()
    {
        RectTransform rt = transform as RectTransform;
        // DOTween.Kill(rt);

        // giữ đúng vị trí gốc
        Vector2 pos = GameManager.instance.board.GetPosFromCoords(this.coords.x, this.coords.y, this.layer) + new Vector2(offset, 0);
        rt.anchoredPosition = pos;

        float strength = 20f;

        Sequence seq = DOTween.Sequence();
        seq.Append(rt.DOAnchorPos(pos + new Vector2(strength, 0f), 0.07f).SetEase(Ease.OutSine));
        seq.Append(rt.DOAnchorPos(pos - new Vector2(strength, 0f), 0.14f).SetEase(Ease.OutSine));
        seq.Append(rt.DOAnchorPos(pos, 0.07f).SetEase(Ease.OutSine));
        await seq.AsyncWaitForCompletion();
        // MoveToRealPos();
    }


    public async Task FadeTile(bool left)
    {
        await Task.WhenAll(
            bgImg.DOFade(0, 0.2f).AsyncWaitForCompletion(),
            typeImg.DOFade(0, 0.2f).AsyncWaitForCompletion(),
            shadowImg.DOFade(0, 0.2f).AsyncWaitForCompletion(),
            Explode(left)
        );
    }

    public void Reset()
    {
        // Kill all tweens on these targets
        bgImg.DOKill();
        typeImg.DOKill();
        shadowImg.DOKill();

        // Set alpha to 1 instantly
        Color c;

        c = bgImg.color;
        c.a = 1f;
        bgImg.color = c;

        c = typeImg.color;
        c.a = 1f;
        typeImg.color = c;

        c = shadowImg.color;
        c.a = 1f;
        shadowImg.color = c;

        TurnOnInput();
    }

    public void OffHint()
    {
        hintEff.gameObject.SetActive(false);
        hintHighlight.gameObject.SetActive(false);
    }

    public async Task Zoom(int delay)
    {
        BlockInput();
        transform.localScale = new Vector3(0, 0);
        await Task.Delay(delay * 200);
        transform.DOScale(1, 0.2f);
        TurnOnInput();
    }
    public void ToggleShadow(bool on)
    {
        shadowImg.gameObject.SetActive(on);
    }
    public void BlockInput()
    {
        UnityEngine.UI.Button button = this.gameObject.GetComponent<UnityEngine.UI.Button>();
        button.interactable = false;
    }

    public void TurnOnInput()
    {
        UnityEngine.UI.Button button = this.gameObject.GetComponent<UnityEngine.UI.Button>();
        button.interactable = true;
    }

    public void MoveToRealPos()
    {
        Vector2 pos = GameManager.instance.board.GetPosFromCoords(this.coords.x, this.coords.y, this.layer);
        RectTransform rt = transform as RectTransform;
        rt.DOAnchorPos(pos, 0.35f).SetEase(Ease.OutSine);
        offset = 0;
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
        this.offset = offset;
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

    public void OnHint()
    {
        hintEff.gameObject.SetActive(true);
        hintHighlight.gameObject.SetActive(true);

        // Xóa tween cũ nếu có
        DOTween.Kill(hintRot.transform);

        // Xoay mãi mãi quanh trục Z
        hintRot.transform
            .DORotate(new Vector3(0, 0, 360f), 2f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
        hintHighlight.DOFade(0.5f, 0.5f)
    .SetLoops(-1, LoopType.Yoyo)
    .SetEase(Ease.Linear);
    }
}
