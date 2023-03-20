using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

namespace Sampel1
{

    [CustomEditor(typeof(EnemyMaker))]
    public class EnemyMakerEditor : Editor
    {
        // Start is called before the first frame update
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Collect"))
            {
                //(target as EnemyMaker).Collect();
            }
        }
    }

}