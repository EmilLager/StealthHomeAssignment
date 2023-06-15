using System;
using System.Collections.Generic;
using UnityEngine;

public class Instruction : MonoBehaviour
{
    [SerializeReference] private List<BaseCommand> m_commands;
    
    public void ChangeCommandType(int index, int typeIndex, Type commandType)
    {
        m_commands[index] = Activator.CreateInstance(commandType) as BaseCommand;
        m_commands[index].TypeIndex = typeIndex;
        m_commands[index].Validate(gameObject);
    }

    public async void ActivateCommands()
    {
        foreach (BaseCommand command in m_commands)
        {
            await command.ActivateCommand();
        }
    }

    private void OnValidate()
    {
        foreach (BaseCommand command in m_commands)
        {
            command?.Validate(gameObject);
        }
    }
}
