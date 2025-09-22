using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
//hẹ hẹ
[System.Serializable]
public class PopupMapping
{
    public Popup popupType;
    public GameObject popupObject;
}
[System.Serializable]
public class PageMapping
{
    public Page popupType;
    public GameObject popupObject;
}


public class UIManager : MonoBehaviour
{

    [SerializeField] private PageMapping[] pageMappings;
    [SerializeField] private PopupMapping[] popupMappings;

    [SerializeField] private GameObject overlay;
    // [SerializeField] private GameObject progressBar;
    // [SerializeField] private GameObject soundButton;
    // [SerializeField] private GameObject musicButton;
    // [SerializeField] private GameObject vibrateButton;

    private Dictionary<Popup, GameObject> popupNodes = new Dictionary<Popup, GameObject>();
    private Dictionary<Popup, BasePopup> popupComponents = new Dictionary<Popup, BasePopup>();
    private Dictionary<Page, GameObject> pageNodes = new Dictionary<Page, GameObject>();
    private Dictionary<Page, BasePage> pageComponents = new Dictionary<Page, BasePage>();
    private static UIManager instance;

    public static UIManager Instance => instance;

    void Start()
    {
        instance = this;
        CachePopupNodes();
        CachePageNodes();
        this.HideAllPopupsInternal();
    }
    //hẹ hẹ



    private void HideAllPopupsInternal()
    {
        if (overlay != null) overlay.SetActive(false);

        foreach (var kvp in popupNodes)
        {
            var node = kvp.Value;
            var type = kvp.Key;
            //   kvp.Value.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.OutBack);

            if (popupComponents.TryGetValue(type, out BasePopup popupComponent) &&
                popupComponent != null && node.activeSelf)
            {
                popupComponent.OnPopupHide();
            }
            node.SetActive(false);
        }
    }
    private void HideAllPages()
    {
        if (overlay != null) overlay.SetActive(false);
        foreach (var kvp in pageNodes)
        {
            var node = kvp.Value;
            var type = kvp.Key;
            //  kvp.Value.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.OutBack);

            if (pageComponents.TryGetValue(type, out BasePage popupComponent) &&
                popupComponent != null && node.activeSelf)
            {
                popupComponent.OnPageHide();
            }
            Debug.Log("hi " + node);
            node.SetActive(false);
        }
    }
    public static void ShowPage(Page popupType, bool hideOthers = true, int curr = 0)
    {
        if (instance == null)
        {
            Debug.LogWarning("UIManager hêhhe");
            return;
        }

        if (hideOthers)
        {
            HideAllPopups();
        }
        instance.HideAllPages();
        Debug.Log($"shopw {popupType}");

        if (instance.pageNodes.TryGetValue(popupType, out GameObject popup) && popup != null)
        {
            // if (instance.overlay != null) instance.overlay.SetActive(true);

            popup.SetActive(true);
            // popup.transform.localScale = Vector3.zero;
            // popup.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);


            if (instance.pageComponents.TryGetValue(popupType, out BasePage popupComponent) &&
                popupComponent != null)
            {
                popupComponent.OnPageShow(curr);
            }
        }
        else
        {
            Debug.LogWarning($"not found: {popupType}");
        }
    }
    public static void ShowPopup(Popup popupType, bool hideOthers = true, int curr = 0)
    {
        if (instance == null)
        {
            Debug.LogWarning("UIManager heùhuie");
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
    public static void HidePage(Page page)
    {
        if (instance == null)
        {
            Debug.LogWarning("hẹhe");
            return;
        }

        if (instance.pageNodes.TryGetValue(page, out GameObject popup) &&
            popup != null && popup.activeSelf)
        {
            if (instance.pageComponents.TryGetValue(page, out BasePage popupComponent))
            {
                var component = popupComponent;

                popup.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.OutBack)
                .SetDelay(0.15f)
                .OnComplete(() =>
                {
                    popup.SetActive(false);
                    if (instance.overlay != null) instance.overlay.SetActive(false);

                    if (component != null)
                    {
                        component.OnPageHide();
                    }
                });
            }
        }
    }
    public static void HidePopup(Popup popupType)
    {
        if (instance == null)
        {
            Debug.LogWarning("UIManager ihẹhee");
            return;
        }

        if (instance.popupNodes.TryGetValue(popupType, out GameObject popup) &&
            popup != null && popup.activeSelf)
        {
            if (instance.popupComponents.TryGetValue(popupType, out BasePopup popupComponent))
            {
                var component = popupComponent;

                popup.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
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
                    Debug.Log($" Cached popup component: {mapping.popupType}");
                }

                Debug.Log($"Cached popup node: {mapping.popupType}");
            }
            else
            {
                Debug.LogWarning($"Popup {mapping.popupType}");
            }
        }
    }
    private void CachePageNodes()
    {
        foreach (var mapping in pageMappings)
        {
            if (mapping.popupObject != null)
            {
                pageNodes[mapping.popupType] = mapping.popupObject;

                BasePage popupComponent = mapping.popupObject.GetComponent<BasePage>();
                if (popupComponent != null)
                {
                    pageComponents[mapping.popupType] = popupComponent;
                    Debug.Log($" Cached page component: {mapping.popupType}");
                }

                Debug.Log($" Cached page node: {mapping.popupType}");
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
    public void showDaily()
    {
        UIManager.ShowPopup(Popup.DAILY);
    }
    public void hideDaily()
    {
        UIManager.HidePopup(Popup.DAILY);
    }
    public static void showtask(Popup popupType, string name)
    {
        if (instance == null)
        {
            Debug.LogWarning("UIManager heùhuie");
            return;
        }

        if (instance.popupNodes.TryGetValue(popupType, out GameObject popup) && popup != null)
        {
            if (instance.overlay != null) instance.overlay.SetActive(true);

            popup.SetActive(true);
            popup.transform.localScale = Vector3.zero;
            popup.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack)
            .SetDelay(2f).OnComplete(() =>
            {
                popup.SetActive(false);
            });


            if (instance.popupComponents.TryGetValue(popupType, out BasePopup popupComponent) &&
                popupComponent != null)
            {
                popupComponent.OnPopupShow();
            }
        }
        else
        {
            Debug.LogWarning($"Popup not found: {popupType}");
        }
    }
    public static void showWin()
    {
        Debug.LogWarning($"Popup not found: igfịì");

        UIManager.ShowPopup(Popup.WIN);

    }
    
    public void showSettings()
    {
        UIManager.ShowPopup(Popup.SETTINGS);
    }
}