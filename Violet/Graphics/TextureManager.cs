using fNbt;
using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using System.IO;
using Violet.Utility;

namespace Violet.Graphics
{
    /// <summary>
    /// Class that manages all loaded textures
    /// </summary>
    public class TextureManager
    {
        /// <summary>
        /// The instance of the texture manager
        /// </summary>
        public static TextureManager Instance
        {
            get
            {
                return instance;
            }
        }

        private Dictionary<int, int> instances;

        //keeps track of every texture that has ever been loaded within the game session
        private Dictionary<string, int> allFilenameHashes;

        //only has textures that are currently loaded in memory
        private Dictionary<int, string> activeFilenameHashes;

        private Dictionary<int, IVioletTexture> textures;

        private static TextureManager instance = new TextureManager();

        private TextureManager()
        {
            this.instances = new Dictionary<int, int>();
            this.textures = new Dictionary<int, IVioletTexture>();
            this.allFilenameHashes = new Dictionary<string, int>();
            this.activeFilenameHashes = new Dictionary<int, string>();
        }
        /// <summary>
        /// Creates an IndexedTexture from an NBTCompound
        /// </summary>
        /// <param name="root">The NBTCompound to load an IndexedTexture from</param>
        /// <returns></returns>
        private IndexedTexture LoadFromNbtTag(NbtCompound root)
        {
            // this code is all dave/carbine code therefore i wil not look at it!
            NbtTag paletteTag = root.Get("pal");
            IEnumerable<NbtTag> palettes = (paletteTag is NbtList) ? ((NbtList)paletteTag) : ((NbtCompound)paletteTag).Tags;

            uint intValue = (uint)root.Get<NbtInt>("w").IntValue;
            byte[] byteArrayValue = root.Get<NbtByteArray>("img").ByteArrayValue;
            List<int[]> list = new List<int[]>();
            foreach (NbtTag palette in palettes)
            {
                if (palette.TagType == NbtTagType.IntArray)
                {
                    list.Add(((NbtIntArray)palette).IntArrayValue);
                }
            }
            SpriteDefinition spriteDefinition = null;
            Dictionary<int, SpriteDefinition> spriteDefinitions = new Dictionary<int, SpriteDefinition>();

            NbtCompound allSprites = root.Get<NbtCompound>("spr");
            if (allSprites != null)
            {
                foreach (NbtTag potentialSprite in allSprites.Tags)
                {
                    if (potentialSprite is NbtCompound)
                    {
                        NbtCompound spriteCompound = (NbtCompound)potentialSprite;
                        string text = spriteCompound.Name.ToLowerInvariant();

                        NbtIntArray dummyIntArray;
                        int[] coordinatesArray = spriteCompound.TryGet<NbtIntArray>("crd", out dummyIntArray) ? dummyIntArray.IntArrayValue : new int[2];
                        int[] boundsArray = spriteCompound.TryGet<NbtIntArray>("bnd", out dummyIntArray) ? dummyIntArray.IntArrayValue : new int[2];
                        int[] originArray = spriteCompound.TryGet<NbtIntArray>("org", out dummyIntArray) ? dummyIntArray.IntArrayValue : new int[2];
                   
                        byte[] optionsArray = spriteCompound.TryGet<NbtByteArray>("opt", out NbtByteArray nbtByteArray) ? nbtByteArray.ByteArrayValue : new byte[3];
                      
                        IList<NbtTag> speedSet = spriteCompound.Get<NbtList>("spd");
                        int frames = spriteCompound.TryGet<NbtInt>("frm", out NbtInt nbtInt) ? nbtInt.IntValue : 1;

                        // this is only found on tilesets put through VEMC
                        NbtIntArray dataArray = spriteCompound.Get<NbtIntArray>("d");
                        int[] data = (dataArray == null) ? null : dataArray.IntArrayValue;
                       
                        Vector2i coords = new Vector2i(coordinatesArray[0], coordinatesArray[1]);
                        Vector2i bounds = new Vector2i(boundsArray[0], boundsArray[1]);
                        Vector2f origin = new Vector2f(originArray[0], originArray[1]);

                        // options are encoded as arrays
                        // you can guess the values from here but i'll elaborate

                        // 0 - flip the sprite horizontally
                        // 1 - flip the sprite vertically 
                        // 2 - animation mode

                        bool flipX = optionsArray[0] == 1;
                        bool flipY = optionsArray[1] == 1;
                        int mode = optionsArray[2];

                        float[] speeds = (speedSet != null) ? new float[speedSet.Count] : new float[0];
                        for (int i = 0; i < speeds.Length; i++)
                        {
                            NbtTag speedValue = speedSet[i];
                            speeds[i] = speedValue.FloatValue;
                        }
                        SpriteDefinition newSpriteDefinition = new SpriteDefinition(text, coords, bounds, origin, frames, speeds, flipX, flipY, mode, data);
                        if (spriteDefinition == null)
                        {
                            spriteDefinition = newSpriteDefinition;
                        }

                        int key = text.GetHashCode();
                        spriteDefinitions.Add(key, newSpriteDefinition);
                    }
                }
            }
            return new IndexedTexture(intValue, list.ToArray(), byteArrayValue, spriteDefinitions, spriteDefinition);
        }


        /// <summary>
        /// Returns an IndexTexture by name
        /// </summary>
        /// <param name="spriteFile">The path to load the IndexedTexture from</param>
        /// <returns></returns>
        public IndexedTexture Use(string spriteFile)
        {
            // Create hash so we can fetch it later
            int num = Hash.Get(spriteFile);

            // Create IndexTexture variable so we can initialize it later.
            IndexedTexture indexedTexture;

            // To save memory, we're not going to load the texture again. Instead, we'll just return an instance from our dictionary of loaded textures.
            if (!this.textures.ContainsKey(num))
            {
                // We don't need to check if we haven't already added it to our activeFilenameHashes dict, because Unuse() will remove the entry from activeFilenameHashes
                activeFilenameHashes.Add(num, spriteFile);

                // Before adding the texture's sprite to our allFilenameHashes dict, first check if we have already cached it before
                if (!this.allFilenameHashes.ContainsKey(spriteFile))
                {
                    allFilenameHashes.Add(spriteFile, num);
                }


                if (!File.Exists(spriteFile))
                {
                    string message = string.Format("The sprite file \"{0}\" does not exist.", spriteFile);
                    throw new FileNotFoundException(message, spriteFile);
                }
                NbtFile nbtFile = new NbtFile(spriteFile);
                indexedTexture = this.LoadFromNbtTag(nbtFile.RootTag);
                this.instances.Add(num, 1);
                this.textures.Add(num, indexedTexture);
            }
            else
            {
                indexedTexture = (IndexedTexture)this.textures[num];
                Dictionary<int, int> dictionary;
                int key;
                (dictionary = this.instances)[key = num] = dictionary[key] + 1;
            }
            return indexedTexture;
        }

        /// <summary>
        /// Loads a multipart animation by name
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public IndexedTexture[] UseMultipart(string file)
        {
            if (File.Exists(file))
            {
                NbtFile nbtFile = new NbtFile(file);
                NbtCompound rootTag = nbtFile.RootTag;
                int value = rootTag.Get<NbtInt>("f").Value;
                IndexedTexture[] array = new IndexedTexture[value];
                for (int i = 0; i < value; i++)
                {
                    string input = string.Format("{0}-{1}", file, i);
                    int num = Hash.Get(input);
                    IndexedTexture indexedTexture;
                    if (!this.textures.ContainsKey(num))
                    {
                        string tagName = string.Format("img{0}", i);
                        NbtCompound root = rootTag.Get<NbtCompound>(tagName);
                        indexedTexture = this.LoadFromNbtTag(root);
                        this.instances.Add(num, 1);
                        this.textures.Add(num, indexedTexture);
                    }
                    else
                    {
                        indexedTexture = (IndexedTexture)this.textures[num];
                        Dictionary<int, int> dictionary;
                        int key;
                        (dictionary = this.instances)[key = num] = dictionary[key] + 1;
                    }
                    array[i] = indexedTexture;
                }
                return array;
            }
            string message = string.Format("The multipart sprite file \"{0}\" does not exist.", file);
            throw new FileNotFoundException(message, file);
        }

        public FullColorTexture UseUnprocessed(string file)
        {
            int hashCode = file.GetHashCode();
            FullColorTexture fullColorTexture;
            if (!this.textures.ContainsKey(hashCode))
            {
                Image image = new Image(file);
                fullColorTexture = new FullColorTexture(image);
                this.instances.Add(hashCode, 1);
                this.textures.Add(hashCode, fullColorTexture);
            }
            else
            {
                fullColorTexture = (FullColorTexture)this.textures[hashCode];
                Dictionary<int, int> dictionary;
                int key;
                (dictionary = this.instances)[key = hashCode] = dictionary[key] + 1;
            }
            return fullColorTexture;
        }

        public FullColorTexture UseFramebuffer()
        {
            int hashCode = Engine.Frame.GetHashCode();
            
            RenderStates states = new RenderStates(BlendMode.Alpha, Transform.Identity, Engine.FrameBuffer.Texture, null);
            VertexArray vertexArray = new VertexArray(PrimitiveType.Quads, 4U);
            vertexArray[0U] = new Vertex(new Vector2f(0f, 0f), new Vector2f(0f, Engine.SCREEN_HEIGHT));
            vertexArray[1U] = new Vertex(new Vector2f(Engine.SCREEN_WIDTH, 0f), Engine.SCREEN_SIZE);
            vertexArray[2U] = new Vertex(Engine.SCREEN_SIZE, new Vector2f(Engine.SCREEN_WIDTH, 0f));
            vertexArray[3U] = new Vertex(new Vector2f(0f, Engine.SCREEN_HEIGHT), new Vector2f(0f, 0f));

            RenderTexture renderTexture = new RenderTexture(Engine.SCREEN_WIDTH, Engine.SCREEN_HEIGHT);
            renderTexture.Clear(Color.Black);
            renderTexture.Draw(vertexArray, states);
            Texture tex = new Texture(renderTexture.Texture);
            renderTexture.Dispose();
            vertexArray.Dispose();
            FullColorTexture fullColorTexture = new FullColorTexture(tex);
            this.instances.Add(hashCode, 1);
            this.textures.Add(hashCode, fullColorTexture);
            return fullColorTexture;
        }

        public void Unuse(ICollection<IVioletTexture> textures)
        {
            if (textures != null)
            {
                foreach (IVioletTexture texture in textures)
                {
                    this.Unuse(texture);
                }
            }
        }

        public void Unuse(IVioletTexture texture)
        {
            foreach (KeyValuePair<int, IVioletTexture> keyValuePair in this.textures)
            {
                int key = keyValuePair.Key;
                IVioletTexture value = keyValuePair.Value;
                if (value == texture)
                {
                    activeFilenameHashes.Remove(key);
                    Dictionary<int, int> dictionary;
                    int key2;
                    (dictionary = this.instances)[key2 = key] = dictionary[key2] - 1;
                    break;
                }
            }
        }

        public void Purge()
        {
            List<int> list = new List<int>();
            foreach (KeyValuePair<int, IVioletTexture> keyValuePair in this.textures)
            {
                int key = keyValuePair.Key;
                IVioletTexture value = keyValuePair.Value;
                if (value != null && this.instances[key] <= 0)
                {
                    list.Add(key);
                }
            }
            foreach (int key2 in list)
            {
                this.textures[key2].Dispose();
                this.textures[key2] = null;
                this.instances.Remove(key2);
                this.textures.Remove(key2);
            }
        }

        public void DumpEveryLoadedTexture()
        {
            List<string> textures = new List<string>();
            foreach (KeyValuePair<string, int> keyntex in this.allFilenameHashes)
            {
                textures.Add($"name == '{ keyntex.Key}' :: hash == '{keyntex.Value}'");
            }

            StreamWriter streamWriter = new StreamWriter("Data/Logs/all_loaded_textures.log");
            textures.ForEach(x => streamWriter.WriteLine(x));
            streamWriter.Close();
        }

        public void DumpLoadedTextures()
        {
            List<string> textures = new List<string>();
            foreach (KeyValuePair<int, string> keyntex in this.activeFilenameHashes)
            {
                textures.Add($"name == '{ keyntex.Value}' :: hash == '{keyntex.Key}'");
            }

            StreamWriter streamWriter = new StreamWriter("Data/Logs/loaded_textures.log");
            textures.ForEach(x => streamWriter.WriteLine(x));
            streamWriter.Close();
        }
    }
}
