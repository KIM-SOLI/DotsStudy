using Assets.Scripts.AuthoringAndMono;
using Assets.Scripts.ComponentsAndTags;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Systems
{
    public partial class CameraControllerSystem : SystemBase
    {
        private EntityManager _entityManager;
        Entity Target;
        private EntityQuery _characterQuery;

        protected override void OnCreate()
        {
            base.OnCreate();

            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _characterQuery = _entityManager.CreateEntityQuery(typeof(CharacterHeading));
        }

        protected override void OnUpdate()
        {
            // 회전 카메라 무빙
            //var brainEntity = SystemAPI.GetSingletonEntity<BrainTag>();
            //var brainScale = SystemAPI.GetComponent<LocalToWorldTransform>(brainEntity).Value.Scale;

            //var cameraSingleton = CameraSingleton.Instance;
            //if (cameraSingleton == null) return;
            //var positionFactor = (float)SystemAPI.Time.ElapsedTime * cameraSingleton.Speed;
            //var height = cameraSingleton.HeightAtScale(brainScale);
            //var radius = cameraSingleton.RadiusAtScale(brainScale);

            //cameraSingleton.transform.position = new Vector3
            //{
            //    x = Mathf.Cos(positionFactor) * radius,
            //    y = height,
            //    z = Mathf.Sin(positionFactor) * radius
            //};
            //cameraSingleton.transform.LookAt(Vector3.zero, Vector3.up);


            if (Target == Entity.Null || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Space))
            {
                var characters = _characterQuery.ToEntityArray(Allocator.Temp);

                if (characters.Length > 0)
                    Target = characters[0];
            }

            if (Target != Entity.Null)
            {
                var cameraTransform = CameraSingleton.Instance.transform;

                LocalToWorld tankTransform = SystemAPI.GetComponent<LocalToWorld>(Target);
                float3 targetPosition = tankTransform.Position;

                //cameraTransform.position = tankTransform.Position - 10.0f * tankTransform.Forward + new float3(0.0f, 5.0f, 0.0f);
                cameraTransform.LookAt(tankTransform.Position, new float3(0.0f, 1.0f, 0.0f));
            }
        }
    }
}