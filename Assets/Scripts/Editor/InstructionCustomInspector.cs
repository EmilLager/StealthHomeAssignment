using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Instruction))]
public class InstructionCustomInspector : Editor
{
    private SerializedProperty m_commandsProperty;
    private List<Type> m_commandTypes;
    private Instruction m_instructionTarget;
    
    private void OnEnable()
    {
        m_instructionTarget = (Instruction) target;
        m_commandsProperty = serializedObject.FindProperty("m_commands");
        m_commandTypes = BaseCommand.GetAllCommandTypes();
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        base.OnInspectorGUI();
        serializedObject.ApplyModifiedProperties();

        for (int i = 0; i < m_commandsProperty.arraySize; i++)
        {
            BaseCommand command = (BaseCommand) m_commandsProperty.GetArrayElementAtIndex(i).managedReferenceValue;
            if (command != null && command.TypeIndex >= 0 && command.TypeIndex != m_commandTypes.IndexOf(command.GetType()))
            {
                m_instructionTarget.ChangeCommandType(i, command.TypeIndex, m_commandTypes[command.TypeIndex]);
            }
            else if(command == null)
            {
                m_instructionTarget.ChangeCommandType(i, -1, typeof(BaseCommand));
            }
        }

        if (GUILayout.Button("Activate Commands"))
        {
            m_instructionTarget.ActivateCommands();
        }
    }
}
