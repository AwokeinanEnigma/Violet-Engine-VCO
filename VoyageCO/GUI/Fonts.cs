using fNbt;
using SFML.Graphics;
using System;
using System.IO;
using Violet.GUI;

namespace VCO.GUI
{
    internal class Fonts
    {
        // (get) Token: 0x06000135 RID: 309 RVA: 0x0000876D File Offset: 0x0000696D
        public static FontData Main => Fonts.fonts[0];
        // (get) Token: 0x06000136 RID: 310 RVA: 0x00008776 File Offset: 0x00006976
        public static FontData Title => Fonts.fonts[1];
        // (get) Token: 0x06000137 RID: 311 RVA: 0x0000877F File Offset: 0x0000697F
        public static FontData Saturn => Fonts.fonts[2];

        public static readonly string DATA = "Data" + Path.DirectorySeparatorChar;
        public static readonly string TEXT = Path.Combine(DATA, "Text", "") + Path.DirectorySeparatorChar;

        private static Font LoadFont(string locale, string fontFile)
        {
            string text = Path.Combine(TEXT, locale, fontFile);
            if (!File.Exists(text))
            {
                text = Path.Combine(TEXT, "en_US", fontFile);
            }
            return new Font(text);
        }
        public static void LoadFonts(string locale)
        {
            string text = Path.Combine(TEXT, locale, "fonts.dat"); if (!File.Exists(text))
            {
                throw new FileNotFoundException("The fonts file for the " + locale + " locale is missing.");
            }
            NbtFile nbtFile = new NbtFile(text);
            for (int i = 0; i < Fonts.FONT_NAMES.Length; i++)
            {
                if (nbtFile.RootTag.TryGet<NbtCompound>(Fonts.FONT_NAMES[i], out NbtCompound nbtCompound))
                {
                    //Console.WriteLine($"font name is {Fonts.FONTS_FILE[i]}");
                    int intValue = nbtCompound.Get<NbtInt>("line").IntValue;
                    uint fontSize = (uint)Math.Max(1, nbtCompound.Get<NbtInt>("size").IntValue);
                    int intValue2 = nbtCompound.Get<NbtInt>("xcomp").IntValue;
                    int intValue3 = nbtCompound.Get<NbtInt>("ycomp").IntValue;
                    string stringValue = nbtCompound.Get<NbtString>("file").StringValue;
                    Font font = Fonts.LoadFont(locale, stringValue);
                    //  Console.WriteLine($"font name is {Fonts.FONTS_FILE[i]} height iz {fontSize}");
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
        private static readonly FontData[] fonts = new FontData[Fonts.FONT_NAMES.Length];
    }
}
