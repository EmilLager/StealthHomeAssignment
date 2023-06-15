using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(BaseCommand))]
public class CommandPropertyDrawer : PropertyDrawer
{
    private List<string> m_commandTypeStrings;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property) + EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty typeIndexProp = property.FindPropertyRelative("m_concreteTypeIndex");

        if (typeIndexProp == null)
        {
            EditorGUI.PropertyField(position, property, label, true );
            return;
        }

        if (m_commandTypeStrings == null)
        {
            m_commandTypeStrings = BaseCommand.GetAllCommandTypes().Select(type => type.ToString()).ToList();
        }

        Rect typeSelectorPosition = position;
        typeSelectorPosition.height = EditorGUIUtility.singleLineHeight;
        
        if (typeIndexProp.intValue >= 0)
        {
            typeSelectorPosition.width /= 2f;
            
            Rect activatePosition = typeSelectorPosition;
            activatePosition.x += typeSelectorPosition.width;

            if (GUI.Button(activatePosition, "Activate"))
            {
                ((BaseCommand) (property.managedReferenceValue))?.ActivateCommand();
            }
        }

        if (GUI.Button(typeSelectorPosition,
            typeIndexProp.intValue == -1 ? "Select a command type" : m_commandTypeStrings[typeIndexProp.intValue],
            EditorStyles.popup))
        {
            GenericMenu menu = new GenericMenu();
            for (int i = 0; i < m_commandTypeStrings.Count; i++)
            {
                string commandName = m_commandTypeStrings[i];
                int index = i;
                menu.AddItem(new GUIContent(commandName),
                    typeIndexProp.intValue == i,
                    selectionValue =>
                    {
                        typeIndexProp.intValue = index;
                        property.serializedObject.ApplyModifiedProperties();
                    }, commandName);
            }

            menu.ShowAsContext();
        }

        Rect basePosition = position;
        basePosition.y += EditorGUIUtility.singleLineHeight;
        basePosition.height -= EditorGUIUtility.singleLineHeight;

        if (typeIndexProp.intValue != -1)
        {
            EditorGUI.PropertyField(basePosition, property, label, true );
        }
    }
}
