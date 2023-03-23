using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

class SoldierAuthoring : UnityEngine.MonoBehaviour
{
    
    class SoldierBaker : Baker<SoldierAuthoring>
    {
        public override void Bake(SoldierAuthoring authoring)
        {
            var graph = PlayableGraph.Create();
            var playableoutput = AnimationPlayableOutput.Create(graph, "Animation", GetComponent<Animator>());
            var mixerPlayable = AnimationMixerPlayable.Create(graph, 1);
            AddComponent(new MoveToTarget());
            AddComponent(new Soldier()
            {
                graph = graph,
                mixerPlayable = mixerPlayable
            });
        }   

    }
}

public struct Soldier : IComponentData
{
    public PlayableGraph graph;
    public AnimationMixerPlayable mixerPlayable;
}
