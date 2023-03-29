using Sampel1;
using Unity.Entities;


namespace Sample1
{

    public class Version2SystemGroupComponentData : ComponentSystemGroup
    {

    }

    public class Version2Systems : ComponentSystemBase
    {
        public override void Update()
        {
            
        }
    }


    public class Version2World : ICustomBootstrap
    {
        public bool Initialize(string defaultWorldName)
        {
            return false;
        }
    }


}