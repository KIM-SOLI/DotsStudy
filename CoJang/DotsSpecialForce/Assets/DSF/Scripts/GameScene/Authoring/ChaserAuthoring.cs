using Unity.Entities;
using UnityEngine;

public class ChaserAuthoring : MonoBehaviour
{
}

public struct ChaserTag : IComponentData
{

}

public struct ChaserComponent : IComponentData
{
	public int HP;

}