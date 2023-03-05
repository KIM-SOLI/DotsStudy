using UnityEngine;

class UnitCounterSingleton : MonoBehaviour
{
    public static TMPro.TextMeshProUGUI Instance;

    void Awake()
    {
        Instance = GetComponent<TMPro.TextMeshProUGUI>();
    }
}
