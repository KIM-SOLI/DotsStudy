using Assets.Scripts.ComponentsAndTags;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.AuthoringAndMono
{
    public class CharacterAuthoring : MonoBehaviour
    {
        public float WalkSpeed;
        public float WalkAmplitude;
        public float WalkFrequency;
    }

    public class CharacterBaker : Baker<CharacterAuthoring>
    {
        public override void Bake(CharacterAuthoring authoring)
        {
            AddComponent(new CharacterMoveProperties
            {
                WalkSpeed = authoring.WalkSpeed,
                WalkAmplitude = authoring.WalkAmplitude,
                WalkFrequency = authoring.WalkFrequency
            });
            AddComponent<CharacterTimer>();
            AddComponent<CharacterHeading>();
        }
    }
}