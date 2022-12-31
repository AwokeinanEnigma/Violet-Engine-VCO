using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Violet.Scenes;
using Violet.Scenes.Transitions;

namespace Violet
{
    //based on https://github.com/NoelFB/Foster/blob/master/Framework/Logging/Log.cs
    //with some minor changes
    public static class Debug
    {
        private static LogLevel Verbosity = LogLevel.Debug;

        private static List<string> log = new List<string>();

        public enum LogLevel
        {
            System,
            Assert,
            Error,
            Warning,
            Info,
            Debug,
            Lua,
            Engine,
            Trace
        }

        private static Dictionary<LogLevel, ConsoleColor> logColors = new Dictionary<LogLevel, ConsoleColor>
        {
            [LogLevel.System] = ConsoleColor.White,
            [LogLevel.Assert] = ConsoleColor.DarkRed,
            [LogLevel.Error] = ConsoleColor.Red,
            [LogLevel.Warning] = ConsoleColor.Yellow,
            [LogLevel.Info] = ConsoleColor.White,
            [LogLevel.Debug] = ConsoleColor.Gray,
            [LogLevel.Lua] = ConsoleColor.Magenta,
            [LogLevel.Engine] = ConsoleColor.Green,
            [LogLevel.Trace] = ConsoleColor.Cyan,
        };

        /// <summary>
        /// Generic logging function. Just logs as system.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="callerFilePath">Ignore this.</param>
        /// <param name="callerLineNumber">Ignore this.</param>
        public static void Log(
        object message,
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
        {
            LogInternal(LogLevel.System, message, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Generic logging function. Just logs as system.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="callerFilePath">Ignore this.</param>
        /// <param name="callerLineNumber">Ignore this.</param>
        internal static void LogEngine(
        object message,
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
        {
            LogInternal(LogLevel.Engine, message, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Used to stop the game if a condition is false. Sends a message to the console and then throws an error.
        /// </summary>
        /// <param name="condition">If this condition is false, the game will go into an error scene.</param>
        /// <param name="message">The message to display if the condition is false. Is "Assertion failed." by default.</param>
        /// <param name="callerFilePath">Ignore this.</param>
        /// <param name="callerLineNumber">Ignore this.</param>
        public static void LogAssertion(
        bool condition,
        string message = "Assertion failed.",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
        {
            if (condition == false)
            {
                LogInternal(LogLevel.Assert, message, callerFilePath, callerLineNumber);
                SceneManager.Instance.AbortTransition();
                SceneManager.Instance.Clear();
                SceneManager.Instance.Transition = new InstantTransition();
                SceneManager.Instance.Push(new ErrorScene(new Exception("Assertion failed!"))); //throw new Exception("Assertion failed!");
            }

        }

        /// <summary>
        /// Used to send error messages to the console.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="callerFilePath">Ignore this.</param>
        /// <param name="callerLineNumber">Ignore this.</param>
        public static void LogError(
        object message,
        bool throwException,
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0
        )
        {
            LogInternal(LogLevel.Error, message, callerFilePath, callerLineNumber);
            if (throwException)
            {
                SceneManager.Instance.AbortTransition();
                SceneManager.Instance.Clear();
                SceneManager.Instance.Transition = new InstantTransition();
                SceneManager.Instance.Push(new ErrorScene(new Exception(message.ToString()))); //throw new Exception("Assertion failed!");
            }
        }

        /// <summary>
        /// Used to send warning messages to the console.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="callerFilePath">Ignore this.</param>
        /// <param name="callerLineNumber">Ignore this.</param>
        public static void LogWarning(
        object message,
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
        {
            LogInternal(LogLevel.Warning, message, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Used to send info messages to the console.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="callerFilePath">Ignore this.</param>
        /// <param name="callerLineNumber">Ignore this.</param>
        public static void LogInfo(
        object message,
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
        {
            LogInternal(LogLevel.Info, message, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Used to send Lua info messages to the console.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="callerFilePath">Ignore this.</param>
        /// <param name="callerLineNumber">Ignore this.</param>
        public static void LogLua(
        object message,
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
        {
            LogInternal(LogLevel.Lua, message, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Used to send debug messages to the console.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="callerFilePath">Ignore this.</param>
        /// <param name="callerLineNumber">Ignore this.</param>
        public static void LogDebug(
        object message,
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
        {
            LogInternal(LogLevel.Debug, message, callerFilePath, callerLineNumber);
        }


        public static void SetVerbosity(
        LogLevel level,
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
        {
            LogInternal(LogLevel.System, $"Current log level is {Verbosity}, setting it to {level}.", callerFilePath, callerLineNumber);
            Verbosity = level;
        }

        private static void LogInternal(LogLevel logLevel, object message, string callerFilePath, int callerLineNumber)
        {
            if (Verbosity < logLevel)
            {
                return;
            }

            string callsite = $"{Path.GetFileName(callerFilePath)}:{callerLineNumber}";
            string dateTimeNow = DateTime.Now.ToString("HH:mm:ss");

            Console.ForegroundColor = logColors[logLevel];
            Console.WriteLine($"{logLevel}, {dateTimeNow}, {callsite}>>> {message}");
            Console.ResetColor();
            log.Add($"{dateTimeNow} [{logLevel}] {callsite}>>> {message}");

        }

        public static void DumpLogs()
        {
            StreamWriter streamWriter = new StreamWriter("Data/Logs/logs.log");
            log.ForEach(x => streamWriter.WriteLine(x));
            streamWriter.Close();
        }

        public static void Initialize()
        {
            SetVerbosity(LogLevel.Engine);
        }
    }
}
