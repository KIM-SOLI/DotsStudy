using Assets.Scripts.ComponentsAndTags;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.AuthoringAndMono
{
    public class BrainMono : MonoBehaviour
    {
        public float BrainHealth;
    }

    public class BrainBaker : Baker<BrainMono>
    {
        public override void Bake(BrainMono authoring)
        {
            AddComponent<BrainTag>();
            AddComponent(new BrainHealth{ Value = authoring.BrainHealth, Max = authoring.BrainHealth });
            AddBuffer<BrainDamageBufferElement>();
        }
    }
}