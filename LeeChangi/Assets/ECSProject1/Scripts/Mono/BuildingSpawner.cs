using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEditor;
using UnityEngine;


public struct BuildingInfo
{
    public Vector3 position;

}


public class BuildingSpawner : MonoBehaviour
{
    public List<GameObject> buildings;
    public void Init()
    {

    }


    public void MakeStream()
    {
        var builder = new BlobBuilder(Unity.Collections.Allocator.Temp);
        //builder.ConstructRoot
        foreach(var building in buildings)
        {
            

        }
        
    }

    public void Spawn()
    {

    }
}


struct BuildingSpawnStream 
{
    public EntityArchetype archiType;

}