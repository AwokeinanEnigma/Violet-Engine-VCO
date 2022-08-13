using System;
using System.IO;
using Violet;
using Violet.Audio;
using Violet.Scenes;
using SunsetRhapsody.Data;
using SunsetRhapsody.Data.Enemies;
using SunsetRhapsody.Scenes;

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
                StreamWriter streamWriter = new StreamWriter("error.log", true);
                streamWriter.WriteLine("At {0}:", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss:fff"));
                streamWriter.WriteLine(value);
                streamWriter.WriteLine();
                streamWriter.Close();
            }
        }
    }
}
