using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Tile : PooledObject
{
    [SerializeField] List<Image> choseEffect;
    [SerializeField] Image typeImg;
    public Vector2Int coords;
    public int layer;
    private THEME theme;
    private TileType type;
    private List<Action> OnClickCallbacks = new List<Action>();
    void Start()
    {
        setTileType(TileType.None);
    }
    void SetUp()
    {
        AddOnClickCallbacks(() => GameManager.instance.Chose(this));
    }

    public bool LegalChose()
    {
        return true;
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

    public THEME GetTheme()
    {
        return theme;
    }

    public TileType GetTileType()
    {
        return type;
    }
    public void setTileType(TileType ttype)
    {
        type = ttype;
        // typeImg.sprite = AssetsLoader.instance.LoadSprite("theme/2");
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




    }


}
