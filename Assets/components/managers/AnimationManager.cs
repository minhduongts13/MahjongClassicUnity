using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager instance;
    [SerializeField] Image shuffleEffect;
    [SerializeField] List<ParticleSystem> shuffleParticles;
    public List<Task> tileMoveAnimation = new List<Task>();

    void Awake()
    {
        AnimationManager.instance = this;
    }
    public async void ShuffleEffect()
    {
        // Đảm bảo có CanvasGroup
        var cg = shuffleEffect.GetComponent<CanvasGroup>();
        if (cg == null) cg = shuffleEffect.gameObject.AddComponent<CanvasGroup>();

        shuffleEffect.gameObject.SetActive(true);
        shuffleEffect.transform.localScale = Vector3.zero;
        cg.alpha = 0.75f;

        cg.DOFade(0.5f, 0.45f);
        foreach (ParticleSystem shuffleParticle in shuffleParticles) shuffleParticle.Play();
        await shuffleEffect.transform
            .DOScale(0.8f, 0.5f)
            .SetEase(Ease.InOutSine)
            .AsyncWaitForCompletion();

        cg.DOFade(0f, 0.2f);

        await shuffleEffect.transform
            .DOScale(1.2f, 0.2f)
            .SetEase(Ease.OutSine)
            .AsyncWaitForCompletion();
        // shuffleParticle.gameObject.SetActive(false);

        shuffleEffect.gameObject.SetActive(false);
    }

}
