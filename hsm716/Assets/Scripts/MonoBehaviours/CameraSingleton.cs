class CameraSingleton : UnityEngine.MonoBehaviour
{
    public static UnityEngine.Camera Instance;
    public static CameraSingleton cs;
    public  UnityEngine.Mesh quadMesh;
    public  UnityEngine.Material spriteSheetMaterial;

    void Awake()
    {
        Instance = GetComponent<UnityEngine.Camera>();
        cs = this;
    }
}