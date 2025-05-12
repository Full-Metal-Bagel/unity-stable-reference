using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityStableReference.Editor
{
    [CustomPropertyDrawer(typeof(StableReference<>), useForChildren: false)]
    public sealed class StableReferenceDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var wrapperProperty = property.FindPropertyRelative(StableReference<int>.WrapperPropertyName);
            return EditorGUI.GetPropertyHeight(wrapperProperty, includeChildren: true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var wrapperProperty = property.FindPropertyRelative(StableReference<int>.WrapperPropertyName);
            var referenceBaseType = property.GetObject().GetType().GetGenericArguments()[0];

            var stableTypes = TypeCache.GetTypesDerivedFrom(typeof(StableWrapper<>));
            var buttonColor = GUI.backgroundColor;
            var buttonName = wrapperProperty.managedReferenceValue == null ? "" : wrapperProperty
                .managedReferenceValue
                .GetType()
                .GetInterfaces()
                .Single(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IStableWrapper<>))
                .GetGenericArguments()[0].FullName;
            var dropdownClicked = false;

            var dropdownPosition = position;
            dropdownPosition.height = EditorGUIUtility.singleLineHeight;
            dropdownPosition.width = position.width - 120;
            dropdownPosition.x = position.x + 120;

            {
                var color = GUI.backgroundColor;
                GUI.backgroundColor = buttonColor;
                dropdownClicked = EditorGUI.DropdownButton(dropdownPosition, new GUIContent(buttonName), FocusType.Passive);
                GUI.backgroundColor = color;
            }

            if (dropdownClicked)
            {
                var types = stableTypes
                    .Where(type => !type.IsAbstract)
                    .SelectMany(type => type.GetInterfaces().Select(@interface => (type, @interface)))
                    .Where(t => t.@interface.IsGenericType && t.@interface.GetGenericTypeDefinition() == typeof(IStableWrapper<>))
                    .Select(t => (t.type, referenceType: t.@interface.GetGenericArguments()[0]))
                    .Where(t => referenceBaseType.IsAssignableFrom(t.referenceType))
                    .OrderBy(t => t.referenceType.FullName)
                ;

                var menu = new GenericMenu();
                foreach (var (wrapperType, referenceType) in types)
                {
                    menu.AddItem(
                        content: new GUIContent(referenceType.FullName),
                        on: true,
                        func: () =>
                        {
                            property.serializedObject.Update();
                            wrapperProperty.managedReferenceValue = Activator.CreateInstance(wrapperType);
                            property.serializedObject.ApplyModifiedProperties();
                        });
                }

                var popup = GenericMenuPopup.Get(menu, "");
                popup.showSearch = true;
                popup.showTooltip = false;
                popup.resizeToContent = true;
                popup.Show(dropdownPosition.position);
            }
            EditorGUI.PropertyField(position, wrapperProperty, label, includeChildren: true);
        }
    }
}
