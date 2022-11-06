namespace VCO.Data
{
    internal class Settings
    {
        public static string Locale
        {
            get => Settings.language;
            set => Settings.language = value;
        }

        public static uint WindowFlavor
        {
            get => Settings.windowFlavor;
            set => Settings.windowFlavor = value;
        }

        public static string WindowStyle => "Data/Graphics/window1.dat";

        public static float EffectsVolume
        {
            get => Settings.sfxVolume;
            set => Settings.sfxVolume = value;
        }

        public static float MusicVolume
        {
            get => Settings.bgmVolume;
            set => Settings.bgmVolume = value;
        }

        public static int TextSpeed
        {
            get => Settings.textSpeed;
            set => Settings.textSpeed = value;
        }

        private static string language = "en_US";

        private static uint windowFlavor = 0U;

        private static float bgmVolume = 0.0f;

        private static float sfxVolume = 0.4f;

        private static int textSpeed = 2;
    }
}
