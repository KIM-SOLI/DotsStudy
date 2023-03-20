using Assets.Scripts.AuthoringAndMono;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ComponentsAndTags
{
    public readonly partial struct GraveyardAspect : IAspect
    {
        public readonly Entity Entity;

        private readonly TransformAspect _transformAspect;

        private readonly RefRO<GraveyardProperties> _graveyardProperties;
        private readonly RefRW<GraveyardRandom> _graveyardRandom;
        private readonly DynamicBuffer<ZombieSpawnPoints> _zombieSpawnPoints;
        private readonly RefRW<ZombieSpawnTimer> _zombieSpawnTimer;

        public int NumberTombstonesToSpawn => _graveyardProperties.ValueRO.NumberTombstonesToSpawn;
        public Entity TombstonePrefab => _graveyardProperties.ValueRO.TombstonePrefab;

        public DynamicBuffer<ZombieSpawnPoints> ZombieSpawnPoints
        {
            get => _zombieSpawnPoints;
        }

        public UniformScaleTransform GetRandomTombstoneTransform()
        {
            return new UniformScaleTransform
            {
                Position = GetRandomPosition(),
                Rotation = GetRandomRotation(),
                Scale = GetRandomScale(0.5f)
            };
        }

        private float3 MinCorner => _transformAspect.Position - HalfDimensions;
        private float3 MaxCorner => _transformAspect.Position + HalfDimensions;

        private float3 HalfDimensions => new float3()
        {
            x = _graveyardProperties.ValueRO.FieldDimenstions.x * 0.5f,
            y = 0f,
            z = _graveyardProperties.ValueRO.FieldDimenstions.y * 0.5f,
        };

        private const float BRAIN_SAFETY_RADIUS_SQ = 100;

        private quaternion GetRandomRotation() =>
            quaternion.RotateY(_graveyardRandom.ValueRW.Value.NextFloat(-0.25f, 0.25f));

        private float GetRandomScale(float min) => _graveyardRandom.ValueRW.Value.NextFloat(min, 1f);

        public float2 GetRandomOffset()
        {
            return _graveyardRandom.ValueRW.Value.NextFloat2();
        }

        public float ZombieSpawnTimer
        {
            get => _zombieSpawnTimer.ValueRO.Value;
            set => _zombieSpawnTimer.ValueRW.Value = value;
        }

        public bool TimeToSpawnZombie => ZombieSpawnTimer <= 0f;

        public float ZombieSpawnRate => _graveyardProperties.ValueRO.ZombieSpawnRate;

        public Entity ZombiePrefab => _graveyardProperties.ValueRO.ZombiePrefab;

        public UniformScaleTransform GetZombieSpawnPoint()
        {
            var position = GetRandomZombieSpawnPoint();
            return new UniformScaleTransform
            {
                Position = position,
                Rotation = quaternion.RotateY(MathHelpers.GetHeading(position, _transformAspect.Position)),
                Scale = 1f
            };
        }

        private float3 GetRandomZombieSpawnPoint()
        {
            return ZombieSpawnPoints[_graveyardRandom.ValueRW.Value.NextInt(ZombieSpawnPoints.Length-1)];
        }

        private float3 GetRandomPosition()
        {
            float3 randomPosition;

            do
            {
                randomPosition = _graveyardRandom.ValueRW.Value.NextFloat3(MinCorner, MaxCorner);
            } while (math.distancesq(_transformAspect.Position, randomPosition) <= BRAIN_SAFETY_RADIUS_SQ);

            return randomPosition;
        }
        public float3 Position => _transformAspect.Position;
    }
}
