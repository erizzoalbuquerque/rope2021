using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCommandBase
{
    string _commandId;
    string _commandDescription;
    string _commandFormat;

    public string CommandId { get { return _commandId; } }
    public string CommandDescription { get { return _commandDescription; } }
    public string CommandFormat { get { return _commandFormat; } }

    public DebugCommandBase(string id, string description, string format)
    {
        _commandId = id;
        _commandDescription = description;
        _commandFormat = format;
    }
}

public class DebugCommand : DebugCommandBase
{
    private Action _command;
    
    public DebugCommand(string id, string description, string format, Action command) : base (id, description, format)
    {
        this._command = command;
    }
    
    public void Invoke()
    {
        _command.Invoke();
    }
}

public class DebugCommand<T1> : DebugCommandBase
{
    private Action<T1> _command;

    public DebugCommand(string id, string description, string format, Action<T1> command) : base(id, description, format)
    {
        this._command = command;
    }

    public void Invoke(T1 value)
    {
        _command.Invoke(value);
    }
}