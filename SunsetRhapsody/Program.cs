using System;
using System.IO;
using Violet;
using Violet.Audio;
using Violet.Scenes;
using SunsetRhapsody.Data;
using SunsetRhapsody.Data.Enemies;
using SunsetRhapsody.Scenes;
using SunsetRhapsody.Scenes.Transitions;
using Violet.Scenes.Transitions;

namespace SunsetRhapsody
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
