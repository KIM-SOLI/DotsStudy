using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Assets.Scripts.AuthoringAndMono
{
    [InternalBufferCapacity(10)]
    public struct ZombieSpawnPoints : IBufferElementData
    {
        public static implicit operator float3(ZombieSpawnPoints e) { return e.Value; }

        public static implicit operator ZombieSpawnPoints(float3 e)
        {
            return new ZombieSpawnPoints { Value = e };

        }
        public float3 Value;
    }
}