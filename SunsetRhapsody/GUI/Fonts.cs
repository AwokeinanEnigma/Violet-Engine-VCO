using System;
using System.IO;
using Violet.GUI;
using fNbt;
using SunsetRhapsody.Data;
using SFML.Graphics;

namespace SunsetRhapsody.GUI
{
	internal class Fonts
	{
        public static FontData Main
        {
            get
            {
                return Fonts.fonts[0];
            }
        }

        public static FontData Title
        {
            get
            {
                return Fonts.fonts[1];
            }
        }

        public static FontData Saturn
        {
            get
            {
                return Fonts.fonts[2];
            }
        }

        private static Font LoadFont(string locale, string fontFile)
        {
            string text = Path.Combine(Paths.TEXT, locale, fontFile);
            if (!File.Exists(text))
            {
                text = Path.Combine(Paths.TEXT, "en_US", fontFile);
            }
            return new Font(text);
        }

        public static void LoadFonts(string locale)
        {
            string text = Path.Combine(Paths.TEXT, locale, "fonts.dat");
            if (!File.Exists(text))
            {
                throw new FileNotFoundException("The fonts file for the " + locale + " locale is missing.");
            }
            NbtFile nbtFile = new NbtFile(text);
            for (int i = 0; i < Fonts.FONT_NAMES.Length; i++)
            {
                NbtCompound nbtCompound = null;
                if (nbtFile.RootTag.TryGet<NbtCompound>(Fonts.FONT_NAMES[i], out nbtCompound))
                {
                    int intValue = nbtCompound.Get<NbtInt>("line").IntValue;
                    uint fontSize = (uint)Math.Max(1, nbtCompound.Get<NbtInt>("size").IntValue);
                    int intValue2 = nbtCompound.Get<NbtInt>("xcomp").IntValue;
                    int intValue3 = nbtCompound.Get<NbtInt>("ycomp").IntValue;
                    string stringValue = nbtCompound.Get<NbtString>("file").StringValue;
                    Font font = Fonts.LoadFont(locale, stringValue);
                    Fonts.fonts[i] = new FontData(font, fontSize, intValue, intValue2, intValue3);
                }
                else
                {
                    Fonts.fonts[i] = new FontData();
                }
            }
        }

        private const string DEFAULT_LOCALE = "en_US";

        private const string FONTS_FILE = "fonts.dat";

        private const string EXTENSION = ".ttf";

        private static readonly string[] FONT_NAMES = new string[]
        {
            "main",
            "title",
            "saturn"
        };

        private static FontData[] fonts = new FontData[Fonts.FONT_NAMES.Length];
    }
}
