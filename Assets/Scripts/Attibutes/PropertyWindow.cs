using UnityEngine;

# if UNITY_EDITOR
using UnityEditor;
[CustomPropertyDrawer(typeof(ArrayRename))]
public class PropertyWindow : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        ArrayRename elements = attribute as ArrayRename;

        var enunNames = elements.enumType.GetEnumNames();

        SerializedProperty transform = property.FindPropertyRelative("transform");

        GUILayout.Space(-21f);
        try
        {
            int index = int.Parse(transform.propertyPath.Split('[', ']')[1]);
            EditorGUILayout.PropertyField(transform, new GUIContent(enunNames[index + 1]));
        }
        catch
        {
            EditorGUILayout.PropertyField(transform, new GUIContent($"Too Many Items. Decrease size"));
        }

    }
}
#endif