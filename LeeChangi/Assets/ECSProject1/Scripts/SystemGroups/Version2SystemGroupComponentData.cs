using Sampel1;
using Unity.Entities;
using Unity.Scenes;


namespace Sample1
{

    public class Version2SystemComponentGroup : ComponentSystemGroup
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