using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager instance;
    [SerializeField] Image shuffleEffect;
    public List<Task> tileMoveAnimation = new List<Task>();

    void Awake()
    {
        AnimationManager.instance = this;
    }
    public async void ShuffleEffect()
    {
        shuffleEffect.gameObject.SetActive(true);
        shuffleEffect.transform.localScale = Vector3.zero;
        await shuffleEffect.transform.DOScale(1, 1f).SetEase(Ease.InOutCirc).AsyncWaitForCompletion();
        shuffleEffect.gameObject.SetActive(false);
    }
}
