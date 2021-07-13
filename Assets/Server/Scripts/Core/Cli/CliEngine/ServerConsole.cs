//-----------------------------------------------------------------
// File:         ServerConsole.cs
// Description:  The console which handle admin commands
// Module:       Network.Server
// Author:       Thomas Hervé
// Date:         14/05/2021
//-----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using UnityEngine;

namespace MonsterWorld.Unity.Network.Server.Cli
{
    public class ServerConsole : IConsole
    {
        #region commands
        Dictionary<string, Command> commands = new Dictionary<string, Command>();

        public void AddCommand(Command command)
        {
            foreach (string name in command.Alias)
            {
                commands.Add(name, command);
            }
        }

        public void Run()
        {
            // Init
            Console.Clear();

            // Instantiate custom logger
            new MyLogHandler();
            LogTypesAuthorized = new LogType[] { LogType.Log, LogType.Warning, LogType.Error };

            while (true)
            {
                PrintEntry("~$");
                try
                {
                    string input = Console.ReadLine();
                    if(input == "")
                    {
                        continue;
                    }
                    string[] args = input.Split(' ');
                    Command c;
                    if(commands.TryGetValue(args[0], out c))
                    {
                        string[] sub_args = new string[args.Length-1];
                        Array.Copy(args, 1, sub_args, 0, args.Length - 1);
                        c.Execute(args);
                    } else
                    {
                        Print("Unknown command");
                    }
                } catch
                {
                    Print("Wrong input");
                }
            }
        }

        public static void Print(string value)
        {
            Console.SetCursorPosition(0, Console.WindowHeight);
            Console.Write(value);
            string end = "";
            for (int i = value.Length; i < Console.WindowWidth; i++)
            {
                end += " ";
            }
            Console.Write(end);
        }

        private static void PrintEntry(string value)
        {
            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            Console.Write(value);
            string end = "";
            for (int i = value.Length; i < Console.WindowWidth; i++)
            {
                end += " ";
            }
            Console.Write(end);
            Console.SetCursorPosition(value.Length, Console.WindowHeight - 1);
        }
        #endregion

        #region logs
        public static LogType[] LogTypesAuthorized { get; set; }

        private class MyLogHandler : ILogHandler
        {
            LogArray logArray = new LogArray();
            private FileStream m_FileStream;
            private StreamWriter m_StreamWriter;

            public MyLogHandler()
            {
                // Save in a file
                string filePath = Application.persistentDataPath + "/Log.txt";
                m_FileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                m_StreamWriter = new StreamWriter(m_FileStream);

                // Replace the default debug log handler
                Debug.unityLogger.logHandler = this;
            }

            public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
            {
                if (LogTypesAuthorized.Contains(logType))
                {
                    // Write in file
                    m_StreamWriter.WriteLine(String.Format(format, args));
                    m_StreamWriter.Flush();

                    // Write in console
                    string message = String.Format(format, args);
                    logArray.Print(message, logType);
                }
            }

            public void LogException(Exception exception, UnityEngine.Object context)
            {
                Debug.unityLogger.LogError("Error", exception);
            }
        }

        private class LogArray
        {
            private int MARGIN_BOTTOM = 3;
            private List<string> logs = new List<string>();

            public void Print(string message, LogType logType)
            {
                logs.Add($"[{logType}] {message}");
                if(logs.Count > Console.WindowHeight - MARGIN_BOTTOM)
                {
                    logs.RemoveAt(0);
                }
                for(int i = 0; i < logs.Count; i++)
                {
                    PrintLog(logs.ElementAt(i), i);
                }
            }

            private void PrintLog(string value, int position)
            {
                Console.SetCursorPosition(0, position);
                Console.Write(value);
                string end = "";
                for (int i = value.Length; i < Console.WindowWidth; i++)
                {
                    end += " ";
                }
                Console.Write(end);
            }
        }
        #endregion
    }
}