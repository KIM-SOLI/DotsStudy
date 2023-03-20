using Unity.Entities;
using Unity.Mathematics;

namespace Assets.Scripts.AuthoringAndMono
{
    public struct GraveyardRandom : IComponentData
    {
        public Random Value;
    }
}
