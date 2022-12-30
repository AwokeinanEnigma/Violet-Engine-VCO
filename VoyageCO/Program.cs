using MoonSharp.Interpreter;
using SFML.System;
using System;
using System.Collections.Generic;
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
using Violet.Input;
using Violet.Lua;
using Violet.Scenes;
using Violet.Utility;
using static Violet.Engine;

namespace VCO
{
    internal class Program
    {
        private static float _sixty_fps = 1.0f / 60.0f;
        private static float _technically_sixty_fps = 1.0f / 61.0f;

        private static float _deltaTime = 1;
        private static int _frameLoops = 0;
        private static float _maxDeltaTime = 0.25f;
        private static float _accumulator = 0;

        private static Clock frameTimer;

        private static List<string> _funnyWindowNames = new List<string>()
        {
            "Go the distance!",
            "What a voyage!",
            "For blood.",
            "Second flood.",
            "Potential Incarnate",
            "???",
            "Rène Edition",
            "It's Rène, not Renee!",
            "How long did it take to build the Ark?",
        };

        [STAThread]
        private static void Main(string[] args)
        {
            WindowName = $"Voyage: Carpe Omnia - {_funnyWindowNames[new Random().Next(0,_funnyWindowNames.Count)]}";
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

            frameTimer = new Clock();
            frameTimer.Restart();

            const int MAX_FRAMESKIP = 5;

            float time, lastTime;
            time = frameTimer.ElapsedTime.AsSeconds();
            lastTime = time;

            try
            {
                SceneManager.Instance.Push(newScene);

                while (Running)
                {
                    time = frameTimer.ElapsedTime.AsSeconds();
                    _deltaTime = time - lastTime;
                    lastTime = time;

                    if (_deltaTime > _maxDeltaTime)
                    {
                        Violet.Debug.Log($"Passed the threshold for max deltaTime, deltaTime is {_deltaTime}, lastTime is {lastTime}");
                        _deltaTime = _maxDeltaTime;
                    }

                    _accumulator += _deltaTime;
                    _frameLoops = 0;
                    
                    while (_accumulator >= _technically_sixty_fps)
                    {
                        if (_frameLoops >= MAX_FRAMESKIP)
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

                            Violet.Debug.LogWarning($"Resyncing, accumulator is {_accumulator}, and loop count is {_frameLoops}. See comments above this line in Program.cs for more info, Enigma.");
                            _accumulator = 0.0f;
                            break;
                        }
                        
                        Update();
                        Render();

                        _accumulator -= _sixty_fps;

                        _frameLoops++;


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
