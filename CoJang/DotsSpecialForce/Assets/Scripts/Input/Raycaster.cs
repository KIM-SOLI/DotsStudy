using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Raycaster : MonoBehaviour
{
    public struct RayInfo
    {
        public Vector3 startPosition;
        public Vector3 endPosition;
        public Vector3 direction;
        public float maxDistance;

        public bool isHit;
    }

    public static RaycastHit Hit { get; private set; }

    public static RayInfo LastRayInfo { get; private set; }

    public const float MaxDistance = 200.0f;

    public static bool ShootRay()
    {
        return ShootRay(InputSystem.MousePosition);
    }

    public static bool ShootRay(Vector2 screenPos)
    {
        if (Camera.main != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(screenPos);

            bool ishit = Physics.Raycast(ray, out RaycastHit hit, MaxDistance);
            Hit = hit;

            LastRayInfo = new RayInfo
            {
                startPosition = ray.origin,
                endPosition = ray.origin + ray.direction * MaxDistance,
                direction = ray.direction,
                maxDistance = MaxDistance,

                isHit = ishit,
            };

            return ishit;
        }
        else
        {
            Debug.LogError("Main Camera is Null! [MousePickSystem, OnMouseClick]");

            LastRayInfo = new RayInfo();
            Hit = new RaycastHit();

            return false;
        }
    }
}
