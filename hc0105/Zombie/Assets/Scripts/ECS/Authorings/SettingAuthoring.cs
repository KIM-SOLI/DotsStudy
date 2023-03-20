using Unity.Entities;

public class SettingAuthoring : UnityEngine.MonoBehaviour
{
    public int unitCount;
    public UnityEngine.GameObject unitPrefab;

    public int targetCount;
    public UnityEngine.GameObject targetPrefab;

    public class SettingBaker : Baker<SettingAuthoring>
    {
        public override void Bake(SettingAuthoring authoring)
        {
            AddComponent(new SettingComponentData
            {
                targetCount = authoring.targetCount,
                unitCount = authoring.unitCount,
                unitEntity = GetEntity(authoring.unitPrefab),
                targetEntity = GetEntity(authoring.targetPrefab),
            });
        }
    }
}


struct SettingComponentData : IComponentData
{
    public int unitCount;
    public Entity unitEntity;

    public int targetCount;
    public Entity targetEntity;


}