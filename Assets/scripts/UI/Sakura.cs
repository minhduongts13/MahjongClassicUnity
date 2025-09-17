using DG.Tweening;
using UnityEngine;

public class Sakura : MonoBehaviour
{
    void Start()
    {
        RotateRight();
    }

    public void RotateRight()
    {
        this.transform.DORotate(
            new Vector3(0, 0, this.transform.eulerAngles.z + 360),
            10.0f,
            RotateMode.FastBeyond360
        ).SetEase(Ease.InQuad)
         .OnComplete(() => RotateLeft());
    }
    public void RotateLeft()
    {
        this.transform.DORotate(
            new Vector3(0, 0, this.transform.eulerAngles.z - 360),
            10.0f, 
            RotateMode.FastBeyond360
        ).SetEase(Ease.InQuad)
         .OnComplete(() => RotateRight());
    }
}