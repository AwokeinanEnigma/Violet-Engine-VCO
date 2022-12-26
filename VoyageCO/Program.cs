using MoonSharp.Interpreter;
using SFML.System;
using System;
using System.IO;
using System.Reflection;
using VCO.Data;
using VCO.Data.Enemies;
using VCO.Scenes;
using VCO.Scenes.Transitions;
using VCO.Scripts;
using Violet;
using Violet.Audio;
using Violet.Lua;
using Violet.Scenes;
using Violet.Utility;
using static Violet.Engine;

namespace VCO
{
    internal class Program
    {


        [STAThread]
        private static void Main(string[] args)
        {
            WindowName = "Voyage: Carpe Omnia";
            // goes directly to Engine.Initalize
            Initialize();

            AudioManager.Instance.MusicVolume = Settings.MusicVolume;
            AudioManager.Instance.EffectsVolume = Settings.EffectsVolume;
            Scene newScene = new TitleScene();
            EnemyFile.Load();
            
           // Test._Test();

            // this is totally and utterly fucking worthless
            //UserData.RegisterAssembly(Assembly.GetExecutingAssembly(), true);

            UserData.RegisterType<EventArgs>();
            LuaManager.instance.RegisterAssembly(Assembly.GetExecutingAssembly());

            new RufiniActionCatalog();

            //   UserData.RegisterType<OverworldScene>(InteropAccessMode.Default);

            //Debug.DumpLogs();



            try
            {
                SceneManager.Instance.Push(newScene);
                while (Engine.Running)
                {
                        Update();
                        Render();

                }
            }
            catch (Exception value)
            {

                StreamWriter streamWriter = new StreamWriter("error.log", true);
                streamWriter.WriteLine("At {0}:", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss:fff"));
                streamWriter.WriteLine(value);
                streamWriter.WriteLine();
                streamWriter.Close();
                
                SceneManager.Instance.AbortTransition();
                SceneManager.Instance.Clear();
                SceneManager.Instance.Transition = new IrisTransition(3f);
                SceneManager.Instance.Push(new ErrorScene(value));

            }
        }
    }
}
