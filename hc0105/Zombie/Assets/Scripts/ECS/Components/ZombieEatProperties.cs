using Unity.Entities;

namespace Assets.Scripts.ComponentsAndTags
{
    public struct ZombieEatProperties : IComponentData, IEnableableComponent
    {
        public float EatDamagePerSecond;
        public float EatAmplitude;
        public float EatFrequency;
    }
}