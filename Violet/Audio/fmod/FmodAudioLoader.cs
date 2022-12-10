// Decompiled with JetBrains decompiler
// Type: Violet.Audio.fmod.FmodAudioLoader
// Assembly: Violet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9929100E-21E2-4663-A88C-1F977D6B46C4
// Assembly location: D:\OddityPrototypes\Violet.dll

using fNbt;
using System;

namespace Violet.Audio.fmod
{
    internal class FmodAudioLoader
    {
        public const string AUDIO_PATH = "Data/Audio/";
        private static FmodAudioLoader instance;

        public static FmodAudioLoader Instance
        {
            get
            {
                if (FmodAudioLoader.instance == null)
                {
                    FmodAudioLoader.instance = new FmodAudioLoader();
                }

                return FmodAudioLoader.instance;
            }
        }

        private FmodAudioLoader()
        {

        }

        public FmodSound LoadSound(
          ref FMOD.System system,
          string name,
          int loopCount,
          float volume)
        {
            string filename = name;
            return new FmodSound(ref system, filename, AudioType.Sound, 0U, 0U, loopCount, volume);
        }
    }
}
