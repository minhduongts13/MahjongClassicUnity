using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[System.Serializable]
public class PopupMapping
{
    public Popup popupType;
    public GameObject popupObject;
}



public class UIManager : MonoBehaviour
{   [SerializeField] private PopupMapping[] popupMappings;

    [SerializeField] private GameObject overlay;
    // [SerializeField] private GameObject progressBar;
    // [SerializeField] private GameObject soundButton;
    // [SerializeField] private GameObject musicButton;
    // [SerializeField] private GameObject vibrateButton;
    
    private Dictionary<Popup, GameObject> popupNodes = new Dictionary<Popup, GameObject>();
    private Dictionary<Popup, BasePopup> popupComponents = new Dictionary<Popup, BasePopup>();
    private static UIManager instance;
    
    public static UIManager Instance => instance;

    void Start()
    {
        instance = this;
        CachePopupNodes();
        this.HideAllPopupsInternal();
    }

  

    private void HideAllPopupsInternal()
    {
        if (overlay != null) overlay.SetActive(false);
        
        foreach (var kvp in popupNodes)
        {
            var node = kvp.Value;
            var type = kvp.Key;
            kvp.Value.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.OutBack);
             
            if (popupComponents.TryGetValue(type, out BasePopup popupComponent) && 
                popupComponent != null && node.activeSelf)
            {
                popupComponent.OnPopupHide();
            }
            node.SetActive(false);
        }
    }
    
    public static void ShowPage(Popup popupType, bool hideOthers = true, int curr = 0)
    {
        if (instance == null)
        {
            Debug.LogWarning("UIManager instance not found!");
            return;
        }

        if (hideOthers)
        {
            HideAllPopups();
        }

        Debug.Log($"Showing popup: {popupType}");

        if (instance.popupNodes.TryGetValue(popupType, out GameObject popup) && popup != null)
        {
            if (instance.overlay != null) instance.overlay.SetActive(true);
            
            popup.SetActive(true);
            popup.transform.localScale = Vector3.zero;   
            popup.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);


            if (instance.popupComponents.TryGetValue(popupType, out BasePopup popupComponent) && 
                popupComponent != null)
            {
                popupComponent.OnPopupShow(curr);
            }
        }
        else
        {
            Debug.LogWarning($"Popup not found: {popupType}");
        }
    }
    public static void ShowPopup(Popup popupType, bool hideOthers = true, int curr = 0)
    {
        if (instance == null)
        {
            Debug.LogWarning("UIManager instance not found!");
            return;
        }

        if (hideOthers)
        {
            HideAllPopups();
        }

        Debug.Log($"Showing popup: {popupType}");

        if (instance.popupNodes.TryGetValue(popupType, out GameObject popup) && popup != null)
        {
            if (instance.overlay != null) instance.overlay.SetActive(true);

            popup.SetActive(true);
            popup.transform.localScale = Vector3.zero;
            popup.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);


            if (instance.popupComponents.TryGetValue(popupType, out BasePopup popupComponent) &&
                popupComponent != null)
            {
                popupComponent.OnPopupShow(curr);
            }
        }
        else
        {
            Debug.LogWarning($"Popup not found: {popupType}");
        }
    }

    public static void HidePopup(Popup popupType)
    {
        if (instance == null)
        {
            Debug.LogWarning("UIManager instance not found!");
            return;
        }

        if (instance.popupNodes.TryGetValue(popupType, out GameObject popup) && 
            popup != null && popup.activeSelf)
        {
            if (instance.popupComponents.TryGetValue(popupType, out BasePopup popupComponent))
            {
                var component = popupComponent;
                
                    popup.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.OutBack)
                    .SetDelay(0.15f)
                    .OnComplete(() => {
                        popup.SetActive(false);
                        if (instance.overlay != null) instance.overlay.SetActive(false);
                        
                        if (component != null)
                        {
                            component.OnPopupHide();
                        }
                    });
            }
        }
    }

    public static void HideAllPopups()
    {
        if (instance == null)
        {
            Debug.LogWarning("UIManager instance not found!");
            return;
        }

        instance.HideAllPopupsInternal();
        Debug.Log("Hidden all popups");
    }

    private void CachePopupNodes()
{
    foreach (var mapping in popupMappings)
    {
        if (mapping.popupObject != null)
        {
            popupNodes[mapping.popupType] = mapping.popupObject;

            BasePopup popupComponent = mapping.popupObject.GetComponent<BasePopup>();
            if (popupComponent != null)
            {
                popupComponents[mapping.popupType] = popupComponent;
                Debug.Log($"✓ Cached popup component: {mapping.popupType}");
            }

            Debug.Log($"✓ Cached popup node: {mapping.popupType}");
        }
        else
        {
            Debug.LogWarning($"Popup object not assigned for: {mapping.popupType}");
        }
    }
}

    void OnDestroy()
    {
        foreach (var component in popupComponents.Values)
        {
            if (component != null)
            {
                component.OnPopupDestroy();
            }
        }
    }

    // public void ToggleSound()
    // {
    //     SoundManager.Instance.soundOn = !SoundManager.Instance.soundOn;
        
    //     string spritePath = SoundManager.Instance.soundOn ? 
    //         "UI/sound-on" : "UI/sound-off";
            
    //     Sprite sprite = Resources.Load<Sprite>(spritePath);
    //     if (soundButton != null && sprite != null)
    //     {
    //         soundButton.GetComponent<Image>().sprite = sprite;
    //     }
    // }

    //     public void ToggleMusic()
    // {
    //     SoundManager.Instance.musicOn = !SoundManager.Instance.musicOn;
        
    //     string spritePath = SoundManager.Instance.musicOn ? 
    //         "UI/music-on" : "UI/music-off";
            
    //     Sprite sprite = Resources.Load<Sprite>(spritePath);
    //     if (musicButton != null && sprite != null)
    //     {
    //         musicButton.GetComponent<Image>().sprite = sprite;
    //     }
    // }

    // public void ToggleVibration()
    // {
    //     SoundManager.Instance.vibrateOn = !SoundManager.Instance.vibrateOn;
        
    //     string spritePath = SoundManager.Instance.vibrateOn ? 
    //         "UI/vibrate-on" : "UI/vibrate-off";
            
    //     Sprite sprite = Resources.Load<Sprite>(spritePath);
    //     if (vibrateButton != null && sprite != null)
    //     {
    //         vibrateButton.GetComponent<Image>().sprite = sprite;
    //     }
    // }
}