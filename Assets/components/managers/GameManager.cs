using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Start()
    {

    }
    void Update()
    {

    }
    public void showplay()
    {
        UIManager.ShowPage(Page.PLAY);
    }
    public void showdash()
    {
        UIManager.ShowPage(Page.DASHBOARD);
    }
}
