using DG.Tweening;
using UnityEngine;

public class Sakura : MonoBehaviour
{
    void Start()
    {

        Rotate();
    }

    public void Rotate()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DORotate(new Vector3(0, 0, 360), 12f, RotateMode.FastBeyond360).SetEase(Ease.InQuad).SetRelative(true));
        seq.Append(transform.DORotate(new Vector3(0, 0, -360), 12f, RotateMode.FastBeyond360).SetEase(Ease.InQuad).SetRelative(true));
        seq.SetLoops(-1);
    }
    public void RotateLeft()
    {
        this.transform.DORotate(
            new Vector3(0, 0, this.transform.eulerAngles.z - 360),
            12.0f,
            RotateMode.FastBeyond360
        ).SetEase(Ease.InQuad);
        // .OnComplete(() => RotateRight());
    }
}