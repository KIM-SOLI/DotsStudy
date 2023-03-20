using Unity.Entities;

namespace Assets.Scripts.AuthoringAndMono
{
    public struct BrainHealth : IComponentData
    {
        public float Value;
        public float Max;
    }
}