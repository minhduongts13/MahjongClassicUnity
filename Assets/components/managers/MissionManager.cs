using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static List<Missiondata> missiondatas = new List<Missiondata>();
    public static void addProgress(string name, int count)
    {
        foreach (var Missiondata in missiondatas)
        {
            if (Missiondata.name == name)
            {
                Missiondata.misionCount -= count;
                UIManager.showtask(Popup.TASK,name);
            }
        }
    }
    public static void checkTask()
    {

    }
}
