using MoonSharp.Interpreter;
using System;
using System.IO;
using System.Reflection;
using VCO.Data;
using VCO.Data.Enemies;
using VCO.Scenes;
using VCO.Scenes.Transitions;
using Violet;
using Violet.Audio;
using Violet.Lua;
using Violet.Scenes;

namespace VCO
{
    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            Engine.Initialize(args);
            AudioManager.Instance.MusicVolume = Settings.MusicVolume;
            AudioManager.Instance.EffectsVolume = Settings.EffectsVolume;
            Scene newScene = new TitleScene();
            EnemyFile.Load();

            // this is totally and utterly fucking worthless
            //UserData.RegisterAssembly(Assembly.GetExecutingAssembly(), true);

            UserData.RegisterType<EventArgs>();
            LuaManager.instance.RegisterAssembly(Assembly.GetExecutingAssembly());


            //   UserData.RegisterType<OverworldScene>(InteropAccessMode.Default);

            //Debug.DumpLogs();
            try
            {
                SceneManager.Instance.Push(newScene);
                while (Engine.Running)
                {
                    Engine.Update();
                }
            }
            catch (Exception value)
            {
                SceneManager.Instance.AbortTransition();
                SceneManager.Instance.Clear();
                SceneManager.Instance.Transition = new IrisTransition(3f);
                SceneManager.Instance.Push(new ErrorScene(value));

                StreamWriter streamWriter = new StreamWriter("error.log", true);
                streamWriter.WriteLine("At {0}:", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss:fff"));
                streamWriter.WriteLine(value);
                streamWriter.WriteLine();
                streamWriter.Close();
            }
        }
    }
}
