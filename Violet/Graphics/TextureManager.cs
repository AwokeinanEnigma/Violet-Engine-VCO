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
                return TextureManager.instance;
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
            NbtTag nbtTag = root.Get("pal");
            IEnumerable<NbtTag> enumerable = (nbtTag is NbtList) ? ((NbtList)nbtTag) : ((NbtCompound)nbtTag).Tags;
            uint intValue = (uint)root.Get<NbtInt>("w").IntValue;
            byte[] byteArrayValue = root.Get<NbtByteArray>("img").ByteArrayValue;
            List<int[]> list = new List<int[]>();
            foreach (NbtTag nbtTag2 in enumerable)
            {
                if (nbtTag2.TagType == NbtTagType.IntArray)
                {
                    list.Add(((NbtIntArray)nbtTag2).IntArrayValue);
                }
            }
            SpriteDefinition spriteDefinition = null;
            Dictionary<int, SpriteDefinition> dictionary = new Dictionary<int, SpriteDefinition>();
            NbtCompound nbtCompound = root.Get<NbtCompound>("spr");
            if (nbtCompound != null)
            {
                foreach (NbtTag nbtTag3 in nbtCompound.Tags)
                {
                    if (nbtTag3 is NbtCompound)
                    {
                        NbtCompound nbtCompound2 = (NbtCompound)nbtTag3;
                        string text = nbtCompound2.Name.ToLowerInvariant();
                        int[] array = nbtCompound2.TryGet<NbtIntArray>("crd", out NbtIntArray nbtIntArray) ? nbtIntArray.IntArrayValue : new int[2];
                        int[] array2 = nbtCompound2.TryGet<NbtIntArray>("bnd", out nbtIntArray) ? nbtIntArray.IntArrayValue : new int[2];
                        int[] org = nbtCompound2.TryGet<NbtIntArray>("org", out nbtIntArray) ? nbtIntArray.IntArrayValue : new int[2];
                        byte[] opt = nbtCompound2.TryGet<NbtByteArray>("opt", out NbtByteArray nbtByteArray) ? nbtByteArray.ByteArrayValue : new byte[3];
                        IList<NbtTag> spd = nbtCompound2.Get<NbtList>("spd");
                        int frames = nbtCompound2.TryGet<NbtInt>("frm", out NbtInt nbtInt) ? nbtInt.IntValue : 1;
                        NbtIntArray nbtIntArray2 = nbtCompound2.Get<NbtIntArray>("d");
                        int[] data = (nbtIntArray2 == null) ? null : nbtIntArray2.IntArrayValue;
                        Vector2i coords = new Vector2i(array[0], array[1]);
                        Vector2i bounds = new Vector2i(array2[0], array2[1]);
                        Vector2f origin = new Vector2f(org[0], org[1]);
                        bool flipX = opt[0] == 1;
                        bool flipY = opt[1] == 1;
                        int mode = opt[2];
                        float[] array5 = (spd != null) ? new float[spd.Count] : new float[0];
                        for (int i = 0; i < array5.Length; i++)
                        {
                            NbtTag nbtTag4 = spd[i];
                            array5[i] = nbtTag4.FloatValue;
                        }
                        SpriteDefinition spriteDefinition2 = new SpriteDefinition(text, coords, bounds, origin, frames, array5, flipX, flipY, mode, data);
                        if (spriteDefinition == null)
                        {
                            spriteDefinition = spriteDefinition2;
                        }

                        int key = text.GetHashCode();
                        dictionary.Add(key, spriteDefinition2);
                    }
                }
            }
            return new IndexedTexture(intValue, list.ToArray(), byteArrayValue, dictionary, spriteDefinition);
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
            vertexArray[0U] = new Vertex(new Vector2f(0f, 0f), new Vector2f(0f, 180f));
            vertexArray[1U] = new Vertex(new Vector2f(320f, 0f), new Vector2f(320f, 180f));
            vertexArray[2U] = new Vertex(new Vector2f(320f, 180f), new Vector2f(320f, 0f));
            vertexArray[3U] = new Vertex(new Vector2f(0f, 180f), new Vector2f(0f, 0f));
            RenderTexture renderTexture = new RenderTexture(320U, 180U);
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
            Debug.LogI($"textures before is {textures.Count} ");
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
            Debug.LogI($"textures length is {textures.Count} ");
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
