using Unity.Entities;


namespace Sample1
{

    public class Version2SystemGroupComponentData : ComponentSystemGroup
    {

    }

    public class Version2World : ICustomBootstrap
    {
        public bool Initialize(string defaultWorldName)
        {
            var world = new World("BattleWorld");
            
            //WorldSystemFilterFlags.
            var systems = DefaultWorldInitialization.GetAllSystems( WorldSystemFilterFlags.Default);
            DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(world, systems);
            //DefaultWorldInitialization.

            World.DefaultGameObjectInjectionWorld = world;
            return true;
        }
    }


}