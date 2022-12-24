using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;
using Violet.Utility;

namespace Violet.Graphics
{
    public class IndexedTexture : IVioletTexture, IDisposable
    {
        [SuppressUnmanagedCodeSecurity]
        [DllImport("csfml-graphics", CallingConvention = CallingConvention.Cdecl)]
        private unsafe static extern void sfTexture_updateFromPixels(IntPtr texture, byte* pixels, uint width, uint height, uint x, uint y);

        #region Properties
        public Texture Image
        {
            get
            {
                return this.imageTex;
            }
        }

        public Texture Palette
        {
            get
            {
                return this.paletteTex;
            }
        }

        public uint CurrentPalette
        {
            get
            {
                return this.currentPal;
            }
            set
            {
                this.currentPal = Math.Min(this.totalPals, value);
            }
        }

        public float CurrentPaletteFloat
        {
            get
            {
                return (float)this.currentPal / (float)this.totalPals;
            }
        }

        public uint PaletteCount
        {
            get
            {
                return this.totalPals;
            }
        }

        public uint PaletteSize
        {
            get
            {
                return this.palSize;
            }
        }
        #endregion

        private SpriteDefinition defaultDefinition;

        private Dictionary<int, SpriteDefinition> definitions;

        private Texture paletteTex;
        private Texture imageTex;

        private uint currentPal;
        private uint totalPals;
        private uint palSize;

        private bool disposed;


        public unsafe IndexedTexture(uint width, int[][] palettes, byte[] image, Dictionary<int, SpriteDefinition> definitions, SpriteDefinition defaultDefinition)
        {
            this.totalPals = (uint)palettes.Length;
            this.palSize = (uint)palettes[0].Length;
            uint num = (uint)(image.Length / (int)width);

            Color[] totalColors = new Color[this.palSize * this.totalPals];
            for (uint allPalettes = 0; allPalettes < this.totalPals; allPalettes++)
            {
                uint num3 = 0;
                while (num3 < palettes[allPalettes].Length)
                {
                    totalColors[allPalettes * this.palSize + num3] = ColorHelper.FromInt(palettes[allPalettes][num3]);
                    num3++;
                }
            }
            Color[] uncoloredPixels = new Color[width * num];
            uint pixels = 0;
            while (pixels < image.Length)
            {
                uncoloredPixels[pixels].A = byte.MaxValue;
                uncoloredPixels[pixels].R = image[pixels];
                uncoloredPixels[pixels].G = image[pixels];
                uncoloredPixels[pixels].B = image[pixels];
                pixels++;
            }
            this.paletteTex = new Texture(this.palSize, this.totalPals);
            this.imageTex = new Texture(width, num);
            fixed (Color* ptr = totalColors)
            {
                byte* b_pixels = (byte*)ptr;
                IndexedTexture.sfTexture_updateFromPixels(this.paletteTex.CPointer, b_pixels, this.palSize, this.totalPals, 0, 0);
            }
            fixed (Color* ptr2 = uncoloredPixels)
            {
                byte* pixels2 = (byte*)ptr2;
                IndexedTexture.sfTexture_updateFromPixels(this.imageTex.CPointer, pixels2, width, num, 0, 0);
            }
            this.definitions = definitions;
            this.defaultDefinition = defaultDefinition;
        }

        ~IndexedTexture()
        {
            this.Dispose(false);
        }

        public SpriteDefinition GetSpriteDefinition(string name)
        {

            int hashCode = name.GetHashCode();
            return this.GetSpriteDefinition(hashCode);
        }

        public SpriteDefinition GetSpriteDefinition(int hash)
        {
            if (!this.definitions.TryGetValue(hash, out SpriteDefinition result))
            {
                result = null;
            }
            return result;
        }

        public ICollection<SpriteDefinition> GetSpriteDefinitions()
        {
            return this.definitions.Values;
        }

        public SpriteDefinition GetDefaultSpriteDefinition()
        {
            return this.defaultDefinition;
        }

        public FullColorTexture ToFullColorTexture()
        {
            uint x = this.imageTex.Size.X;
            uint y = this.imageTex.Size.Y;
            Image image = new Image(x, y);
            Image indexedImage = this.imageTex.CopyToImage();
            Image paletteImage = this.paletteTex.CopyToImage();

            Parallel.For(0, y, num =>
            {
                for (uint num2 = 0U; num2 < x; num2 += 1U)
                {
                    uint x2 = (uint)(indexedImage.GetPixel(num2, (uint)num).R / 255.0 * this.palSize);
                    Color pixel = paletteImage.GetPixel(x2, this.currentPal);
                    image.SetPixel(num2, (uint)num, pixel);
                }
            });
            image.SaveToFile("img.png");
            indexedImage.SaveToFile("indImg.png");
            paletteImage.SaveToFile("palImg.png");
            return new FullColorTexture(image);
        }

        public virtual void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed && disposing)
            {
                this.imageTex.Dispose();
                this.paletteTex.Dispose();
            }
            this.disposed = true;
        }
    }
}
