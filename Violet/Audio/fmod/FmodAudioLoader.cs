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
          float volume,
          AudioType type)
        {
            string filename = name;
            return new FmodSound(ref system, filename, type, 0U, 0U, loopCount, volume);
        }
    }
}
