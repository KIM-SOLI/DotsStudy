using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyMaker : MonoBehaviour
{
    [SerializeField] EnemySetAuthoring setAuthoring;
    [SerializeField] public List<EnemySetData> data;

    public void Collect()
    {
        data = new List<EnemySetData>();
        foreach (var auth in GetComponentsInChildren<EnemyAuthoring>())
        {
            //Debug.Log("?");
            data.Add(new EnemySetData
            {
                position = auth.transform.position,
                prefab = auth.gameObject,
            }) ;
        }
        setAuthoring.dataSet = data;
    }
}
