﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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

        public enum LogLevel {
            System,
            Assert,
            Error,
            Warning,
            Info,
            Debug,
            Trace
        }

        private static Dictionary<LogLevel, ConsoleColor> logColors = new Dictionary<LogLevel, ConsoleColor>
        {
            [LogLevel.System] = ConsoleColor.White,
            [LogLevel.Assert] = ConsoleColor.DarkRed,
            [LogLevel.Error] = ConsoleColor.Red,
            [LogLevel.Warning] = ConsoleColor.Yellow,
            [LogLevel.Info] = ConsoleColor.White,
            [LogLevel.Debug] = ConsoleColor.DarkBlue,
            [LogLevel.Trace] = ConsoleColor.Cyan,
        };

        /// <summary>
        /// Used to send system messages to the console.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="callerFilePath">Ignore this.</param>
        /// <param name="callerLineNumber">Ignore this.</param>
        public static void LSystem(
        string message,
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
        {
            LogInternal(LogLevel.System, message, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Used to stop the game if a condition is false.
        /// </summary>
        /// <param name="condition">If this condition is false, the game will go into an error scene.</param>
        /// <param name="message">The message to display if the condition is false. Is "Assertion failed." by default.</param>
        /// <param name="callerFilePath">Ignore this.</param>
        /// <param name="callerLineNumber">Ignore this.</param>
        public static void LAssert(
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
        public static void LError(
        string message,
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
        {
            LogInternal(LogLevel.Error, message, callerFilePath, callerLineNumber);
            SceneManager.Instance.AbortTransition();
            SceneManager.Instance.Clear();
            SceneManager.Instance.Transition = new InstantTransition();
            SceneManager.Instance.Push(new ErrorScene(new Exception(message))); //throw new Exception("Assertion failed!");
        }

        /// <summary>
        /// Used to send warning messages to the console.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="callerFilePath">Ignore this.</param>
        /// <param name="callerLineNumber">Ignore this.</param>
        public static void LWarning(
        string message,
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
        public static void LInfo(
        string message,
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
        {
            LogInternal(LogLevel.Info, message, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Used to send debug messages to the console.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="callerFilePath">Ignore this.</param>
        /// <param name="callerLineNumber">Ignore this.</param>
        public static void LDebug(
        string message,
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

        private static void LogInternal(LogLevel logLevel, string message, string callerFilePath, int callerLineNumber)
        {
            if (Verbosity < logLevel)
            {
                return;
            }

            var callsite = $"{Path.GetFileName(callerFilePath)}:{callerLineNumber}";
            string dateTimeNow = DateTime.Now.ToString("HH:mm:ss");

            Console.ForegroundColor = logColors[logLevel];
            Console.WriteLine($"{logLevel}, {dateTimeNow}, {callsite}>>> {message}");
            Console.ResetColor();
            log.Add($"{dateTimeNow} [{logLevel}] {callsite} {message}");

        }

        public static void DumpLogs() {
            StreamWriter streamWriter = new StreamWriter("logs.log");
            log.ForEach(x => streamWriter.WriteLine(x));
            streamWriter.Close();
        }

        public static void Initialize()
        {
        }
    }
}
