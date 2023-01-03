using fNbt;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Violet.Collision;
using Violet.Utility;

namespace Violet.Maps
{
    public class MapLoader
    {
        private static string FixPath(string mapFile)
        {
            string path = mapFile;
            if (System.IO.Path.GetExtension(path) == string.Empty)
            {
                path += ".dat";
            }

            return path;
        }

        public static Map Load(string mapFile, string graphicsDirectory)
        {
            string str = MapLoader.FixPath(mapFile);
            if (!File.Exists(str))
            {
                throw new FileNotFoundException("Could not find map file: \"" + str + "\"");
            }

            Map map = new Map();
            NbtCompound rootTag = new NbtFile(str).RootTag;
            long ticks = DateTime.Now.Ticks;
            //Debug.Log("Starting loading");
            LoadHeader(map, rootTag);
            //Debug.Log("Loaded header");





            LoadBGM(map, rootTag);
            //Debug.Log("Loaded BGM");
            LoadSFX(map, rootTag);
            //Debug.Log("Loaded SFX");
            LoadDoors(map, rootTag);
            //Debug.Log("Loaded doors");
            LoadTriggers(map, rootTag);
            //Debug.Log("Loaded triggers");
            LoadNPCs(map, rootTag);
            //Debug.Log("Loaded NPCs");
            LoadNPCPaths(map, rootTag);
            //Debug.Log("Loaded NPC paths");
            LoadNPCAreas(map, rootTag);
            //Debug.Log("Loaded NPC Areas");
            LoadCrowds(map, rootTag);
            //Debug.Log("Loaded crowds");
            LoadSpawns(map, rootTag);
            //Debug.Log("Loaded Loaded spawns");
            LoadCollisions(map, rootTag);
            //  Debug.Log("Loaded collisions");
            LoadTileGroups(map, rootTag);
            // Debug.Log("Loaded groups");
            LoadParallax(map, rootTag);

            Debug.LogDebug($"Loaded map data in {(DateTime.Now.Ticks - ticks) / 10000L}ms");

            return map;
        }

        public static string[] LoadTitle(string mapFile)
        {
            string str = MapLoader.FixPath(mapFile);
            string[] strArray = new string[2];
            if (File.Exists(str))
            {
                Map map = new Map();
                NbtCompound rootTag = new NbtFile(str).RootTag;
                MapLoader.LoadHeader(map, rootTag);
                strArray[0] = map.Head.Title;
                strArray[1] = map.Head.Subtitle;
            }
            else
            {
                strArray[0] = string.Empty;
                strArray[1] = string.Empty;
            }
            return strArray;
        }

        private static void LoadHeader(Map map, NbtCompound mapTag)
        {
            NbtCompound nbtCompound = mapTag.Get<NbtCompound>("head");
            NbtInt nbtInt = nbtCompound.Get<NbtInt>("color");
            Color color = ColorHelper.FromInt(nbtInt.Value);
            NbtInt nbtInt2 = nbtCompound.Get<NbtInt>("nColor");
            Color secondaryColor = color;
            if (nbtInt2 != null)
            {
                secondaryColor = ColorHelper.FromInt(nbtInt2.Value);
            }
            NbtString nbtString = nbtCompound.Get<NbtString>("title");
            NbtString nbtString2 = nbtCompound.Get<NbtString>("subtitle");
            NbtInt nbtInt3 = nbtCompound.Get<NbtInt>("width");
            NbtInt nbtInt4 = nbtCompound.Get<NbtInt>("height");
            NbtList nbtList = nbtCompound.Get<NbtList>("tilesets");
            List<Map.Tileset> list = new List<Map.Tileset>();
            if (nbtList != null)
            {
                foreach (NbtTag nbtTag in nbtList)
                {
                    NbtCompound nbtCompound2 = (NbtCompound)nbtTag;
                    Map.Tileset item = new Map.Tileset
                    {
                        Name = nbtCompound2.Get<NbtString>("ts").Value,
                        FirstId = nbtCompound2.Get<NbtInt>("tid").Value
                    };
                    list.Add(item);
                }
            }

            NbtString nbtString3 = nbtCompound.Get<NbtString>("script");
            string script = (nbtString3 == null) ? null : nbtString3.StringValue;

            NbtString nbtString4 = nbtCompound.Get<NbtString>("bbg");
            string bbg = (nbtString4 == null) ? null : nbtString4.StringValue;

            NbtByte nbtByte = nbtCompound.Get<NbtByte>("shdw");
            bool shadows = nbtByte == null || nbtByte.Value != 0;

            NbtByte nbtByte2 = nbtCompound.Get<NbtByte>("ocn");
            bool ocean = nbtByte2 != null && nbtByte2.Value != 0;

            map.Head = new Map.Header
            {
                PrimaryColor = color,
                SecondaryColor = secondaryColor,
                Title = nbtString.Value,
                Subtitle = nbtString2.Value,
                Width = nbtInt3.Value,
                Height = nbtInt4.Value,
                Tilesets = list,
                Script = script,
                BBG = bbg,
                Shadows = shadows,
                Ocean = ocean
            };
        }

        // my code is perfect
#pragma warning disable

        private static void LoadBGM(Map map, NbtCompound mapTag)
        {
            NbtTag nbtTag = mapTag.Get("audbgm");
            if (!(nbtTag is ICollection<NbtTag>))
            {
                return;
            }

            if (nbtTag != null)
            {


                foreach (NbtCompound nbtCompound in (IEnumerable<NbtTag>)nbtTag)
                {
                    map.Music.Add(new Map.BGM()
                    {
                        Loop = nbtCompound.Get<NbtByte>("loop").Value == 1,
                        Flag = nbtCompound.Get<NbtShort>("flag").Value,
                        Name = nbtCompound.Get<NbtString>("bgm").Value,
                        Height = nbtCompound.Get<NbtInt>("h").Value,
                        Width = nbtCompound.Get<NbtInt>("w").Value,
                        X = nbtCompound.Get<NbtInt>("x").Value,
                        Y = nbtCompound.Get<NbtInt>("y").Value
                    });
                }
            }

        }

        private static void LoadSFX(Map map, NbtCompound mapTag)
        {
            NbtTag nbtTag = mapTag.Get("audsfx");
            if (!(nbtTag is ICollection<NbtTag>))
            {
                return;
            }

            foreach (NbtCompound nbtCompound in (IEnumerable<NbtTag>)nbtTag)
            {
                map.SoundEffects.Add(new Map.SFX()
                {
                    Loop = nbtCompound.Get<NbtByte>("loop").Value == 1,
                    Flag = nbtCompound.Get<NbtShort>("flag").Value,
                    Name = nbtCompound.Get<NbtString>("sfx").Value,
                    Height = nbtCompound.Get<NbtInt>("h").Value,
                    Width = nbtCompound.Get<NbtInt>("w").Value,
                    X = nbtCompound.Get<NbtInt>("x").Value,
                    Y = nbtCompound.Get<NbtInt>("y").Value,
                    Interval = nbtCompound.Get<NbtShort>("interval").Value
                });
            }

        }

        private static void LoadDoors(Map map, NbtCompound mapTag)
        {
            NbtTag nbtTag = mapTag.Get("doors");
            if (!(nbtTag is ICollection<NbtTag>))
            {
                return;
            }

            foreach (NbtCompound nbtCompound in (IEnumerable<NbtTag>)nbtTag)
            {
                Map.Portal portal = new Map.Portal();
                portal.X = nbtCompound.Get<NbtInt>("x").Value;
                portal.Y = nbtCompound.Get<NbtInt>("y").Value;
                portal.Width = nbtCompound.Get<NbtInt>("w").Value;
                portal.Height = nbtCompound.Get<NbtInt>("h").Value;
                portal.Xto = nbtCompound.Get<NbtInt>("xto").Value;
                portal.Yto = nbtCompound.Get<NbtInt>("yto").Value;
                portal.Map = nbtCompound.Get<NbtString>(nameof(map)).Value;
                portal.SFX = nbtCompound.Get<NbtInt>("sfx").Value;
                portal.Flag = nbtCompound.Get<NbtShort>("flag").Value;
                NbtByte nbtByte = nbtCompound.Get<NbtByte>("dir");
                portal.DirectionTo = nbtByte != null ? nbtByte.Value : -1;
                map.Portals.Add(portal);
            }

        }

        private static void LoadTriggers(Map map, NbtCompound mapTag)
        {
            NbtTag nbtTag = mapTag.Get("triggers");
            if (!(nbtTag is ICollection<NbtTag>))
            {
                return;
            }

            foreach (NbtCompound nbtCompound in (IEnumerable<NbtTag>)nbtTag)
            {
                Map.Trigger trigger = new Map.Trigger();
                trigger.Flag = nbtCompound.Get<NbtShort>("flag").Value;
                trigger.Script = nbtCompound.Get<NbtString>("scr").Value;
                int x = nbtCompound.Get<NbtInt>("x").Value;
                int y = nbtCompound.Get<NbtInt>("y").Value;
                trigger.Position = new Vector2f(x, y);
                trigger.Points = new List<Vector2f>();
                NbtList nbtList = nbtCompound.Get<NbtList>("coords");
                for (int tagIndex = 0; tagIndex < nbtList.Count; tagIndex += 2)
                {
                    Vector2f vector2f = new Vector2f(((NbtInt)nbtList[tagIndex]).Value, ((NbtInt)nbtList[tagIndex + 1]).Value);
                    trigger.Points.Add(vector2f);
                }
                map.Triggers.Add(trigger);
            }

        }

        private static void LoadNPCs(Map map, NbtCompound mapTag)
        {
            NbtTag nbtTag = mapTag.Get("npcs");
            if (!(nbtTag is ICollection<NbtTag>))
            {
                return;
            }

            foreach (NbtCompound npcCompound in (IEnumerable<NbtTag>)nbtTag)
            {
                Map.NPC npc = new Map.NPC();
                npc.X = npcCompound.Get<NbtInt>("x").Value;
                npc.Y = npcCompound.Get<NbtInt>("y").Value;
                npc.Name = npcCompound.Get<NbtString>("name").Value;
                npc.Direction = npcCompound.Get<NbtByte>("dir").Value;
                npc.Enabled = npcCompound.Get<NbtByte>("en").Value == 1;
                npc.Mode = npcCompound.Get<NbtByte>("mov").Value;

                npcCompound.TryGet<NbtString>("spr", out NbtString sprite);
                npc.Sprite = sprite?.Value;

                npcCompound.TryGet<NbtInt>("w", out NbtInt w);
                npc.Width = w != null ? w.Value : 0;
                npcCompound.TryGet<NbtInt>("h", out NbtInt h);
                npc.Height = h != null ? h.Value : 0;

                npcCompound.TryGet<NbtFloat>("spd", out NbtFloat speed);
                npc.Speed = speed != null ? speed.FloatValue : 1f;

                npcCompound.TryGet<NbtShort>("dly", out NbtShort delay);
                npc.Delay = delay != null ? delay.ShortValue : (short)0;

                npcCompound.TryGet<NbtShort>("dst", out NbtShort distance);
                npc.Distance = distance != null ? distance.ShortValue : (short)20;

                npcCompound.TryGet<NbtString>("cnstr", out NbtString constraint);
                npc.Constraint = constraint != null ? constraint.Value : "";

                npcCompound.TryGet<NbtByte>("shdw", out NbtByte shadow);
                npc.Shadow = shadow == null || shadow.ShortValue != 0;

                npcCompound.TryGet<NbtByte>("cls", out NbtByte solid);
                npc.Solid = solid == null || solid.ShortValue != 0;

                npcCompound.TryGet<NbtByte>("stky", out NbtByte sticky);
                npc.Sticky = sticky == null || sticky.ShortValue != 0
                    ;
                npcCompound.TryGet<NbtInt>("dpth", out NbtInt depth);
                npc.DepthOverride = depth != null ? depth.IntValue : int.MinValue;

                npc.Flag = npcCompound.Get<NbtShort>("flag").Value;

                npc.Text = new List<Map.NPCtext>();
                NbtCompound nbtCompound2 = npcCompound.Get<NbtCompound>("entries");
                if (nbtCompound2 != null)
                {
                    for (int index = 0; index < nbtCompound2.Count; index += 2)
                    {
                        string str = nbtCompound2.Get<NbtString>(string.Format("t{0}", index / 2)).Value;
                        int num = nbtCompound2.Get<NbtShort>(string.Format("f{0}", index / 2)).Value;
                        Map.NPCtext npCtext = new Map.NPCtext()
                        {
                            ID = str,
                            Flag = num
                        };
                        npc.Text.Add(npCtext);
                    }
                }
                npc.TeleText = new List<Map.NPCtext>();
                NbtCompound telepathyCompound = npcCompound.Get<NbtCompound>("tele");
                if (telepathyCompound != null)
                {
                    for (int index = 0; index < telepathyCompound.Count; index += 2)
                    {
                        string str = telepathyCompound.Get<NbtString>(string.Format("t{0}", index / 2)).Value;
                        int num = telepathyCompound.Get<NbtShort>(string.Format("f{0}", index / 2)).Value;
                        Map.NPCtext npcText = new Map.NPCtext()
                        {
                            ID = str,
                            Flag = num
                        };
                        npc.TeleText.Add(npcText);
                    }
                }
                map.NPCs.Add(npc);
            }

        }

        private static void LoadNPCPaths(Map map, NbtCompound mapTag)
        {
            NbtTag nbtTag = mapTag.Get("paths");
            if (!(nbtTag is ICollection<NbtTag>))
            {
                return;
            }

            foreach (NbtCompound nbtCompound in (IEnumerable<NbtTag>)nbtTag)
            {
                Map.Path path = new Map.Path();
                path.Name = nbtCompound.Get<NbtString>("name").Value;
                List<Vector2f> vector2fList = new List<Vector2f>();
                NbtList coordinates = nbtCompound.Get<NbtList>("coords");

                for (int tagIndex = 0; tagIndex < coordinates.Count; tagIndex += 2)
                {
                    Vector2f vector2f = new Vector2f(((NbtInt)coordinates[tagIndex]).Value, ((NbtInt)coordinates[tagIndex + 1]).Value);
                    vector2fList.Add(vector2f);
                }
                path.Points = vector2fList;
                map.Paths.Add(path);
            }

        }

        private static void LoadNPCAreas(Map map, NbtCompound mapTag)
        {
            // Get the "areas" tag from the map tag
            NbtTag areaTag = mapTag.Get("areas");

            // Check if the tag is not a collection of tags
            if (!(areaTag is ICollection<NbtTag>))
            {
                return;
            }

            // Iterate through each area compound tag in the "areas" tag
            foreach (NbtCompound areaCompound in (IEnumerable<NbtTag>)areaTag)
            {
                // Create a new area object
                Map.Area area = new Map.Area();

                // Set the name of the area
                area.Name = areaCompound.Get<NbtString>("name").Value;

                // Get the position and size of the area
                int left = areaCompound.Get<NbtInt>("x").Value;
                int top = areaCompound.Get<NbtInt>("y").Value;
                int width = areaCompound.Get<NbtInt>("w").Value;
                int height = areaCompound.Get<NbtInt>("h").Value;

                // Set the rectangle of the area
                area.Rectangle = new IntRect(left, top, width, height);

                // Add the area to the map's list of areas
                map.Areas.Add(area);
            }

        }

        private static void LoadCrowds(Map map, NbtCompound mapTag)
        {
        }

        private static void LoadSpawns(Map map, NbtCompound mapTag)
        {
            NbtTag enemySpawnTag = mapTag.Get("spawns");

            // Check that `enemySpawnTag` is a collection of tags
            if (!(enemySpawnTag is ICollection<NbtTag>))
            {
                return;
            }

            // Iterate over each enemy spawn tag
            foreach (NbtCompound enemySpawnCompound in (IEnumerable<NbtTag>)enemySpawnTag)
            {
                // Create a new enemy spawn object
                Map.EnemySpawn enemySpawn = new Map.EnemySpawn();

                // Set the position and size of the enemy spawn
                enemySpawn.X = enemySpawnCompound.Get<NbtInt>("x").Value;
                enemySpawn.Y = enemySpawnCompound.Get<NbtInt>("y").Value;
                enemySpawn.Width = enemySpawnCompound.Get<NbtInt>("w").Value;
                enemySpawn.Height = enemySpawnCompound.Get<NbtInt>("h").Value;

                // Initialize the list of enemies for the enemy spawn
                enemySpawn.Enemies = new List<Map.Enemy>();

                NbtList enemyIds = enemySpawnCompound.Get<NbtList>("enids");
                NbtList enemyFrequencies = enemySpawnCompound.Get<NbtList>("enfreqs");

                // Go through each enemy ID
                for (int tagIndex = 0; tagIndex < enemyIds.Count; ++tagIndex)
                {
                    // Get the name of the enemy
                    NbtString enemyName = enemyIds.Get<NbtString>(tagIndex);

                    // Get the frequency of the enemy
                    NbtByte enemyfrequency = enemyIds.Get<NbtByte>(tagIndex);

                    // Create new enemy with name and frequency
                    enemySpawn.Enemies.Add(new Map.Enemy()
                    {
                        EnemyName = enemyName.Value,
                        Chance = enemyfrequency.ByteValue
                    });
                }
                map.Spawns.Add(enemySpawn);
            }

        }

        private static void LoadCollisions(Map map, NbtCompound mapTag)
        {
            // Check if the mapTag has a "mesh" property
            NbtTag meshTag = mapTag.Get("mesh");

            // Return if the meshTag is not a collection
            if (!(meshTag is ICollection<NbtTag>))
            {
                return;
            }

            // Iterate through each mesh list in the collection
            foreach (NbtList meshList in (IEnumerable<NbtTag>)meshTag)
            {
                // Create a list of points
                List<Vector2f> points = new List<Vector2f>();

                // Iterate through every two tags in the mesh list
                for (int tagIndex = 0; tagIndex < meshList.Count; tagIndex += 2)
                {
                    // Get the x and y values as integers
                    int x = meshList.Get<NbtInt>(tagIndex).Value;
                    int y = meshList.Get<NbtInt>(tagIndex + 1).Value;

                    // Add a new Vector2f with the x and y values to the points list
                    points.Add(new Vector2f(x, y));
                }
                // Create new mesh with points
                Mesh mesh = new Mesh(points);
                map.Mesh.Add(mesh);
            }
        }

        private static void LoadTileGroups(Map map, NbtCompound mapTag)
        {
            // Check if the mapTag has a "tiles" property
            NbtTag tilesTag = mapTag.Get("tiles");

            // Return if the tilesTag is not a collection
            if (!(tilesTag is ICollection<NbtTag>))
            {
                return;
            }

            // Iterate through each tile compound in the tilesTag collection
            foreach (NbtTag tileTag in (IEnumerable<NbtTag>)tilesTag)
            {
                // Check if the tileTag is a compound
                if (tileTag is NbtCompound tileCompound)
                {
                    // Get the values from the tile compound
                    int depth = tileCompound.Get<NbtInt>("depth").Value;
                    int x = tileCompound.Get<NbtInt>("x").Value;
                    int y = tileCompound.Get<NbtInt>("y").Value;
                    int width = tileCompound.Get<NbtInt>("w").Value;

                    // Get the tiles as a byte array and convert it to a ushort array
                    byte[] src = tileCompound.Get<NbtByteArray>("tiles").Value;
                    ushort[] dst = new ushort[src.Length / 2];
                    Buffer.BlockCopy(src, 0, dst, 0, src.Length);

                    // Create a new Group object with the values
                    Map.Group group = new Map.Group()
                    {
                        Depth = depth,
                        X = x,
                        Y = y,
                        Width = width,
                        Height = dst.Length / 2 / width,
                        Tiles = dst
                    };

                    // Add the Group object to the map's Groups list
                    map.Groups.Add(group);
                }
            }
        }

        private static void LoadParallax(Map map, NbtCompound mapTag)
        {

            // Check if the mapTag has a "parallax" property
            NbtTag parallaxTag = mapTag.Get("parallax");

            // Return if the parallaxTag is not a collection
            if (!(parallaxTag is ICollection<NbtTag>))
            {
                return;
            }

            // Iterate through each parallax compound in the parallaxTag collection
            foreach (NbtCompound parallaxCompound in (IEnumerable<NbtTag>)parallaxTag)
            {
                // Get the values from the parallax compound
                string sprite = parallaxCompound.Get<NbtString>("spr").StringValue;
                int shader = parallaxCompound.Get<NbtInt>("shdrmde").IntValue;

                float vectorX = parallaxCompound.Get<NbtFloat>("vx").FloatValue;
                float vectorY = parallaxCompound.Get<NbtFloat>("vy").FloatValue;

                float x = parallaxCompound.Get<NbtFloat>("x").FloatValue;
                float y = parallaxCompound.Get<NbtFloat>("y").FloatValue;

                float width = parallaxCompound.Get<NbtFloat>("w").FloatValue;
                float height = parallaxCompound.Get<NbtFloat>("h").FloatValue;

                // Create a new Parallax object with the values
                Map.Parallax parallax = new Map.Parallax()
                {
                    Sprite = sprite,
                    Vector = new Vector2f(vectorX, vectorY),
                    Area = new IntRect((int)x, (int)y, (int)width, (int)height),
                    Depth = short.MinValue,
                    ShaderMode = shader,
                };
                map.Parallaxes.Add(parallax);
            }
            // Add the Parallax object to the map's Parallaxes list
        }

    }
}
