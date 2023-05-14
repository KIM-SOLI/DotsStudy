using Sample1;
using UnityEditor;
using UnityEngine;



namespace Unity.Scenes
{

    [CustomPropertyDrawer(typeof(IGetBakedSystem))]
    public class TypeReferencePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //base.OnGUI(position, property, label);
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }
    }

}