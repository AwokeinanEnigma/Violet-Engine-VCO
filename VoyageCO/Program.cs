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
using static Violet.Engine;

namespace VCO
{
    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            EngineInitializationData initalizationData = new EngineInitializationData()
            {
                base_frame_buffer_scale = 3,
                icon_size = 32,
                target_framerate = 60,
                start_vsync = false,
                start_fullscreen = true,
                required_opengl_version = 2.1m,
                screen_height = 180,
                screen_width = 320,

            };

            // goes directly to Engine.Initalize
            Initialize(args, initalizationData);

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
