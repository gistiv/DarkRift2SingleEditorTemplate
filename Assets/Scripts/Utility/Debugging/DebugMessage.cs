using UnityEngine;

namespace Utility.Debugging
{
    /// <summary>
    /// A struct to hold all the info about a debug message.
    /// </summary>
    struct DebugMessage
    {
        /// <summary> The condition text. </summary>
        public string Message { get; set; }
        /// <summary> The stack trace. </summary>
        public string StackTrace { get; set; }
        /// <summary> The type of the log. </summary>
        public LogType Type { get; set; }

        public DebugMessage(string message, string stackTrace, LogType type)
        {
            Message = message;
            StackTrace = stackTrace;
            Type = type;
        }
    }


}
