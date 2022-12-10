using MoonSharp.Interpreter;
using SFML.System;
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
using Violet.Utility;
using static Violet.Engine;

namespace VCO
{
    internal class Program
    {


        [STAThread]
        private static void Main(string[] args)
        {
            /*
            EngineInitializationData initalizationData = new EngineInitializationData()
            {
                base_frame_buffer_scale = ini["enginedata"]["base_frame_buffer_scale"].ToUInt(),
                icon_size = ini["enginedata"]["icon_size"].ToUInt(),
                target_framerate = ini["enginedata"]["target_framerate"].ToUInt(),
                start_vsync = ini["enginedata"]["start_vsync"].ToBool(),
                start_fullscreen = ini["enginedata"]["start_fullscreen"].ToBool(),
                required_opengl_version = ini["enginedata"]["required_opengl_version"].ToUInt(),
                screen_height = ini["enginedata"]["screen_height"].ToUInt(),
                screen_width = ini["enginedata"]["screen_width"].ToUInt(),

            };*/

            // goes directly to Engine.Initalize
            Initialize();

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
                        Update();
                        Render();

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
