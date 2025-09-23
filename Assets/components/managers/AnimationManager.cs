using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager instance;
    public List<Task> tileMoveAnimation = new List<Task>();

    void Awake()
    {
        instance = this;
    }
}
