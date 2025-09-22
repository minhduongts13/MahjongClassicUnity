using System.Diagnostics;
using UnityEngine;

public class Languages : BasePopup
{
    public GameObject content;
    public void chooseLanguage(GameObject L)
    {

        var check = getCheck(L);
        foreach (Transform child in content.transform)
        {
            if (child.gameObject != L)
            {
                var otherCheck = getCheck(child.gameObject);
                otherCheck.SetActive(false);
            }
        }
        check.SetActive(true);
    }

    public GameObject getCheck(GameObject L)
    {
        var checkbox = L.transform.GetChild(2);
        return checkbox.transform.GetChild(0).gameObject;
    }

    
}