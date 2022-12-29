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
        private static float technically_sixty_fps = 1.0f / 61.0f;


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

            UserData.RegisterType<EventArgs>();
            LuaManager.instance.RegisterAssembly(Assembly.GetExecutingAssembly());

            new RufiniActionCatalog();



            Clock timer = new Clock();
            timer.Restart();

            const int MAX_FRAMESKIP = 5;

            float time, lastTime;
            time = timer.ElapsedTime.AsSeconds();
            lastTime = time;

            float delta = 1.0f;
            float maxDelta = 0.25f; //0.25f
            float acc = 0;
            float stepTime = technically_sixty_fps;
            int loops;


            try
            {
                SceneManager.Instance.Push(newScene);

                while (Running)
                {
                    time = timer.ElapsedTime.AsSeconds();
                    delta = time - lastTime;
                    lastTime = time;

                    if (delta > maxDelta)
                    {
                        Violet.Debug.Log($", deltaTime is {delta}, lastTime is {lastTime}");
                        delta = maxDelta;
                    }

                    acc += delta;
                    loops = 0;
                    
                    while (acc >= technically_sixty_fps)
                    {
                        if (loops >= MAX_FRAMESKIP)
                        {
                            /*
                             * Here's possible causes as to why this would be triggered:
                             *
                             * The user tabbed in and back out of the game. SFML gets weird when you lose focus.
                             * The game is taking a frame or two to load something
                             * An error has occurred
                             * The user's computer cannot keep up with the game
                             * 
                            */

                            Violet.Debug.LogWarning($"Resyncing, accumulator is {acc}, and loop count is {loops}. See comments above this line in Program.cs for more info, Enigma.");
                            acc = 0.0f;
                            break;
                        }
                        
                        Update();
                        Render();

                        acc -= sixty_fps;

                        loops++;


                    }
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
