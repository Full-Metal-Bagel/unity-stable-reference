using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityStableReference.Editor
{
    [CustomPropertyDrawer(typeof(StableWrapper<>), useForChildren: false)]
    public sealed class StableWrapperDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            property = property.GetVisibleChildren().Single();
            return EditorGUI.GetPropertyHeight(property, includeChildren: true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property = property.GetVisibleChildren().Single();
            EditorGUI.PropertyField(position, property, label, includeChildren: true);
        }
    }
}
