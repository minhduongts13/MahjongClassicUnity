using UnityEngine;

public class PooledObject : MonoBehaviour
{
    public bool used = false;
    public void Kill()
    {
        used = false;
        gameObject.SetActive(false);
    }
}
