using MoonSharp.Interpreter;
using SFML.System;
using System;
using System.Diagnostics;
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
        private static float sixty_fps = 1.0f / 60.0f;
        private static float thirty_fps = 1.0f / 30.0f;
        private static float onehundredtwenty_fps = 1.0f / 120.0f;

        private static float frameTime;
        private static float lastFrameTime;
        private static Stopwatch frameTimer;


        [STAThread]
        private static void Main(string[] args)
        {
            WindowName = "Voyage: Carpe Omnia";
            // goes directly to Engine.Initalize
            Initialize();

            AudioManager.Instance.MusicVolume = Settings.MusicVolume;
            AudioManager.Instance.EffectsVolume = Settings.EffectsVolume;
            Scene newScene = new TitleScene();
            //ArtScene();
            EnemyFile.Load();

            // Test._Test();

            // this is totally and utterly fucking worthless
            //UserData.RegisterAssembly(Assembly.GetExecutingAssembly(), true);

            UserData.RegisterType<EventArgs>();
            LuaManager.instance.RegisterAssembly(Assembly.GetExecutingAssembly());

            new RufiniActionCatalog();

            //   UserData.RegisterType<OverworldScene>(InteropAccessMode.Default);

            //Debug.DumpLogs();



            Clock timer = new Clock();
            timer.Restart();

            const int MAX_FRAMESKIP = 5;

            float time, lastTime;
            time = timer.ElapsedTime.AsSeconds();
            lastTime = time;

            float delta = 1.0f;
            float maxDelta = 0.25f; //0.25f
            float acc = 0;
            float stepTime = sixty_fps;
            int loops;
            float alpha = 0.0f;


            try
            {
                SceneManager.Instance.Push(newScene);

                while (Running)
                {
                    time = timer.ElapsedTime.AsSeconds();
                    delta = time - lastTime;
                    lastTime = time;

                    //m_MainWindow.DispatchEvents();

                    //m_DeltaTime = m_Timer.GetDeltaTime();
                    //m_FPS = 1.0f / m_DeltaTime;

                    if (delta > maxDelta)
                        delta = maxDelta;


                    acc += delta;

                    loops = 0;
                    while (acc >= stepTime)
                    {
                        Update();
                        Render();

                        acc -= stepTime;

                        loops++;

                        if (loops >= MAX_FRAMESKIP)
                        {
                            acc = 0.0f;
                            break;
                        }

                    }

                    /*if (_timeSinceLastUpdate > _timePerFrame)
                                      {
                                          _timeSinceLastUpdate -= _timePerFrame;

                                      }*/
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
