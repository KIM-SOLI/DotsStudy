using Unity.Entities;


namespace Sample1
{

	struct FindTargetComponentData : IComponentData
	{
		//public float3 Position
	} 

	struct TargetEnemyComponentData : IComponentData
	{
		public Entity targetEntity;

	}
}