
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Utility.Debugging.Console.Log
{
    public class DebugConsole : MonoBehaviour
    {
        public bool DisplayConsole = false;

        private Color logColor = Color.white;

        private Color warningColor = Color.yellow;

        private Color errorColor = Color.magenta;

        private Color exceptionColor = Color.red;

        private Color assertColor = Color.red;

        // The entry to be expanded. -1 means no entry.
        private int expandedEntry = -1;

        // The Vector2 for the scroll bar.
        private Vector2 scroll;

        // All the messages.
        private List<DebugMessage> Messages = new List<DebugMessage>();

        private void OnEnable()
        {
            // Subscribe to the log message received event.
            Application.logMessageReceived += OnGetLogMessage;
        }

        private void OnDisable()
        {
            // Unsubscribe to the log message received event.
            Application.logMessageReceived -= OnGetLogMessage;
        }

        private void OnGetLogMessage(string message, string stackTrace, LogType type)
        {
            message = "[" + System.DateTime.Now.Hour + ":" + System.DateTime.Now.Minute + ":" + System.DateTime.Now.Second + "] " + message;

            // Trim the condition and stack trace.
            message = message.Trim();
            stackTrace = stackTrace.Trim();

            // Add the message.
            Messages.Add(new DebugMessage(message, stackTrace, type));
        }


        void OnGUI()
        {
            // Only run if show and enable console is enabled.
            if (DisplayConsole)
            {
                // Begin an area.
                GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height - 25), GUI.skin.box);
                // Begin the scroll area.
                scroll = GUILayout.BeginScrollView(scroll);

                // Loop through every message.
                for (int i = 0; i < Messages.Count; i++)
                {
                    // Create a new label style and base it on the original label.
                    GUIStyle labelStyle = GUI.skin.label;
                    // Create a entry color field.
                    Color entryColor = Color.magenta;
                    // Get the entry color based on the message type.
                    switch (Messages[i].Type)
                    {
                        case LogType.Error:
                            entryColor = errorColor;
                            break;
                        case LogType.Assert:
                            entryColor = assertColor;
                            break;
                        case LogType.Warning:
                            entryColor = warningColor;
                            break;
                        case LogType.Log:
                            entryColor = logColor;
                            break;
                        case LogType.Exception:
                            entryColor = exceptionColor;
                            break;
                    }
                    // Set the text color to the entry color.
                    labelStyle.normal.textColor = entryColor;

                    // Create a button that looks like a label.
                    if (GUILayout.Button(Messages[i].Message, labelStyle))
                    {
                        // When clicked, if the selected entry is the same as this message, close the entry.
                        // Else set the selected entry to thie message entry.
                        if (expandedEntry == i)
                        {
                            expandedEntry = -1;
                        }
                        else
                        {
                            expandedEntry = i;
                        }                            
                    }

                    // If the expanded entry is this message, show the stack trace.
                    if (expandedEntry == i)
                    {
                        // Create a new box style based on the GUI skin box.
                        GUIStyle boxStyle = GUI.skin.box;
                        // Set the text alignment.
                        boxStyle.alignment = TextAnchor.UpperLeft;
                        // Set the text color to the same color as the entry color.
                        boxStyle.normal.textColor = entryColor;
                        // Show it as a box.
                        GUILayout.Box(Messages[i].StackTrace, boxStyle);
                    }
                }

                // End the scroll view.
                GUILayout.EndScrollView();

                // When click, do the export logs function.
                if (GUILayout.Button("Export Logs"))
                {
                    ExportLogs();
                }

                // And lastly end the area.
                GUILayout.EndArea();
            }
        }

        /// <summary>
        /// Exports all logs to Application.dataPath/logs
        /// </summary>
        public void ExportLogs()
        {
            // Create a new string builder.
            StringBuilder sb = new StringBuilder();
            // Loop through all the messages.
            for (int i = 0; i < Messages.Count; i++)
            {
                // Add the message type to the line.
                switch (Messages[i].Type)
                {
                    case LogType.Error:
                        sb.Append("[ERROR] ");
                        break;
                    case LogType.Assert:
                        sb.Append("[ASSERT] ");
                        break;
                    case LogType.Warning:
                        sb.Append("[WARNING] ");
                        break;
                    case LogType.Log:
                        sb.Append("[LOG] ");
                        break;
                    case LogType.Exception:
                        sb.Append("[EXCEPTION] ");
                        break;
                    default:
                        sb.Append("[UNKNOWN TYPE] ");
                        break;
                }

                // Append the condition.
                sb.Append(Messages[i].Message);
                // Start a new line.
                sb.AppendLine();
                // Add the stack trace.
                sb.AppendLine(Messages[i].StackTrace);
                // Add a new empty line.
                sb.AppendLine();
            }

            // Make sure the output directory exists.
            if (!Directory.Exists(Application.dataPath + "/debug/"))
            {
                Directory.CreateDirectory(Application.dataPath + "/debug/");
            }                

            // Create the file name and then the file.
            System.DateTime now = System.DateTime.Now;
            string fileName = string.Format("{0}-{1}-{2} {3}-{4}-{5}.log", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
            File.WriteAllText(Application.dataPath + "/debug/" + fileName, sb.ToString());
        }
    }
}
