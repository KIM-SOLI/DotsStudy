using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemManager : MonoBehaviour
{
    static public SystemManager instance=null;
    public bool isActiveCameraSystem;
    public bool isActiveCannonBallSystem;
    public bool isActiveTankMovementSystem;
    public bool isActiveTurretRotationSystem;
    public bool isActiveTurretShootingSystem;

    public AnimationClip clip1;

    public Mesh quadMesh;
    public Material spriteSheetMaterial;

    private void Awake()
    {
        instance = this;
    }
}
