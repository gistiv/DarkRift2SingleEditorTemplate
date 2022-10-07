using System;
using UnityEngine;

namespace Utility.Debugging.Console.Input
{

    public class DebugCommand<TReturn> : DebugCommandBase
    {
        private Func<TReturn> command;

        public DebugCommand(string id, string description, string format, Func<TReturn> command) : base(id, description, format)
        {
            this.command = command;
        }

#pragma warning disable CS0114 // Member hides inherited member; missing override keyword
        public void Invoke(string fullCommand)
#pragma warning restore CS0114 // Member hides inherited member; missing override keyword
        {
            TReturn retVal = command.Invoke();
            base.Invoke(fullCommand, retVal as string);
        }
    }

    public class DebugCommand<T1, TReturn> : DebugCommandBase
    {
        private Func<T1, TReturn> command;

        public DebugCommand(string id, string description, string format, Func<T1, TReturn> command) : base(id, description, format)
        {
            this.command = command;
        }

        public void Invoke(string fullCommand, T1 value)
        {
            TReturn retVal = command.Invoke(value);
            base.Invoke(fullCommand, retVal as string);
        }
    }

    public class DebugCommand<T1, T2, TReturn> : DebugCommandBase
    {
        private Func<T1, T2, TReturn> command;

        public DebugCommand(string id, string description, string format, Func<T1, T2, TReturn> command) : base(id, description, format)
        {
            this.command = command;
        }

        public void Invoke(string fullCommand, T1 value1, T2 value2)
        {
            TReturn retVal = command.Invoke(value1, value2);
            base.Invoke(fullCommand, retVal as string);
        }
    }
}
