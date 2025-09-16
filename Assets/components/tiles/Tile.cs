using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [SerializeField] List<Image> choseEffect;
    public Vector2 coords;
    private THEME theme;
    private TileType type;
    private List<Action> OnClickCallbacks = new List<Action>();
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

    public void OnUnChose()
    {
        foreach (Image c in choseEffect)
        {
            c.DOFade(0, 0.5f).OnComplete(() =>
            {
                c.gameObject.SetActive(false);
            });
        }




    }


}
