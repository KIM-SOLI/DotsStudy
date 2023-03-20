using Unity.Entities;

namespace Assets.Scripts.ComponentsAndTags
{
    public struct CharacterMoveProperties : IComponentData, IEnableableComponent
    {
        public float WalkSpeed;
        public float WalkAmplitude;
        public float WalkFrequency;
    }

    public struct CharacterTimer : IComponentData
    {
        public float Value;
    }

    public struct CharacterHeading : IComponentData
    {
        public float Value;
    }

}