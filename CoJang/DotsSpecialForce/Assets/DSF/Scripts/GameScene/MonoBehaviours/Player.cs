using UnityEngine;

class Player : MonoBehaviour
{
    public static GameObject Instance;

    private void Awake()
    {
        Instance = gameObject;
    }
}
