using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(KeyBind))]
public class KeyBindDrawer : PropertyDrawer
{
    [SerializeField] private KeyBind keyBind;
    [SerializeField] private string currentKeyState;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        //var keyProperty = property.FindPropertyRelative("key");     
        var editingKeyProperty = property.FindPropertyRelative("editingKey");

        if (keyBind == null)     
        {
            SetupKeyBind(editingKeyProperty);
        }

        Rect leftRectangle = new Rect(position.x, position.y, position.width / 2, position.height);

        if (editingKeyProperty.boolValue && InTextFieldBounds() || (LeftMousePressed() && InTextFieldBounds()))
        {
            currentKeyState = "...";
            editingKeyProperty.boolValue = true;
        }
        else if (editingKeyProperty.boolValue && ((LeftMousePressed() && !InTextFieldBounds()) || Event.current.keyCode == KeyCode.Escape))
        {
            currentKeyState = "None";
            editingKeyProperty.boolValue = false;
        }
        else if (editingKeyProperty.boolValue && Event.current.keyCode != KeyCode.None)
        {
            currentKeyState = Event.current.keyCode.ToString();
            editingKeyProperty.boolValue = false;
        }

        var pressedKey = EditorGUI.TextField(leftRectangle, new GUIContent("Edit Key"), currentKeyState);
        keyBind.key = pressedKey;
        EditorGUI.TextField(leftRectangle, new GUIContent("Edit Key"), keyBind.key);
        EditorGUI.EndProperty();                                                                         
    }    

    /*
    protected virtual void OnSceneGUI()
    {
        var test = target as LaunchPad;
        var keyBind = test.key;
        //var pressedKey = EditorGUI.TextField(new Rect(0, 0, 5, 5), new GUIContent("Edit Key"), "test");
    }
    */

    private void SetupKeyBind(SerializedProperty editingKeyProperty)
    {
        keyBind = new KeyBind();
        currentKeyState = "None";
        editingKeyProperty.boolValue = false;
        RemoveTextCursor();
    }

    private static void RemoveTextCursor()
    {
        GUI.skin.settings.cursorFlashSpeed = 0;
        GUI.skin.settings.cursorColor = Color.clear;
    }

    private bool LeftMousePressed()
    {
        return Event.current.type == EventType.MouseDown && Event.current.isMouse && Event.current.button == 0;
    }

    private bool InTextFieldBounds()
    {
        return GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition);
    }
}
