using System;
using System.Collections.Generic;
using Violet.Audio;
using VCO.Data;

namespace VCO.Battle.UI
{
	internal class LevelUpJingler : IDisposable
	{
		public LevelUpJingler(CharacterType[] characters, bool useOutro)
		{
			this.useOutro = useOutro;
			this.baseJingle = AudioManager.Instance.Use(Paths.SFX_BATTLE_JINGLES + "jingleBase.wav", AudioType.Stream);
			this.baseJingle.LoopCount = -1;
			if (this.useOutro)
			{
				this.groupOutro = AudioManager.Instance.Use(Paths.SFX_BATTLE + "groupOutro.wav", AudioType.Sound);
			}
			this.characterJingles = new Dictionary<CharacterType, VioletSound>();
			foreach (CharacterType characterType in characters)
			{
				string filename = string.Format("{0}jingle{1}.{2}", Paths.SFX_BATTLE_JINGLES, CharacterNames.GetName(characterType), "wav");
				VioletSound VioletSound = AudioManager.Instance.Use(filename, AudioType.Stream);
				VioletSound.LoopCount = -1;
				this.characterJingles.Add(characterType, VioletSound);
			}
			this.state = LevelUpJingler.State.Stopped;
		}

		~LevelUpJingler()
		{
			this.Dispose(false);
		}

		public void Play()
		{
			if (this.state == LevelUpJingler.State.Stopped)
			{
				this.baseJingle.Play();
				foreach (VioletSound VioletSound in this.characterJingles.Values)
				{
					VioletSound.Volume = 0f;
					VioletSound.Play();
				}
				this.state = LevelUpJingler.State.Playing;
			}
		}

		public void Play(CharacterType character)
		{
			this.Play();
			if (this.characterJingles.ContainsKey(character))
			{
				AudioManager.Instance.FadeIn(this.characterJingles[character], 800U);
				this.state = LevelUpJingler.State.Playing;
			}
		}

		public void End()
		{
			if (this.useOutro)
			{
				foreach (VioletSound sound in this.characterJingles.Values)
				{
					AudioManager.Instance.FadeOut(sound, 400U);
				}
				AudioManager.Instance.FadeOut(this.baseJingle, 400U);
				this.groupOutro.Play();
			}
			else
			{
				foreach (VioletSound sound2 in this.characterJingles.Values)
				{
					AudioManager.Instance.FadeOut(sound2, 3000U);
				}
				AudioManager.Instance.FadeOut(this.baseJingle, 3000U);
			}
			this.state = LevelUpJingler.State.Ending;
		}

		public void Stop()
		{
			this.baseJingle.Stop();
			this.groupOutro.Stop();
			foreach (VioletSound VioletSound in this.characterJingles.Values)
			{
				VioletSound.Stop();
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				AudioManager.Instance.Unuse(this.baseJingle);
				if (this.useOutro)
				{
					AudioManager.Instance.Unuse(this.groupOutro);
				}
				foreach (VioletSound sound in this.characterJingles.Values)
				{
					AudioManager.Instance.Unuse(sound);
				}
				this.disposed = true;
			}
		}

		private const string AUDIO_EXT = "wav";

		private const uint FADE_IN_DURATION = 800U;

		private const uint FADE_OUT_DURATION = 3000U;

		private const uint FADE_OUT_QUICK_DURATION = 400U;

		private bool disposed;

		private VioletSound baseJingle;

		private VioletSound groupOutro;

		private Dictionary<CharacterType, VioletSound> characterJingles;

		private bool useOutro;

		private LevelUpJingler.State state;

		private enum State
		{
			Playing,
			Ending,
			Stopped
		}
	}
}
