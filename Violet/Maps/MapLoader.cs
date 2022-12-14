using fNbt;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.IO;
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

        public static async Task<Map> Load(string mapFile, string graphicsDirectory)
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
            await LoadBGM(map, rootTag);
            //Debug.Log("Loaded BGM");
            await LoadSFX(map, rootTag);
            //Debug.Log("Loaded SFX");
            await LoadDoors(map, rootTag);
            //Debug.Log("Loaded doors");
            await LoadTriggers(map, rootTag);
            //Debug.Log("Loaded triggers");
            await LoadNPCs(map, rootTag);
            //Debug.Log("Loaded NPCs");
            await LoadNPCPaths(map, rootTag);
            //Debug.Log("Loaded NPC paths");
            await LoadNPCAreas(map, rootTag);
            //Debug.Log("Loaded NPC Areas");
            //await LoadCrowds(map, rootTag);
            //Debug.Log("Loaded crowds");
            await LoadSpawns(map, rootTag);
            //Debug.Log("Loaded loaded spawns");
            await LoadCollisions(map, rootTag);
            //  Debug.Log("Loaded collisions");
            await LoadTileGroups(map, rootTag);
            // Debug.Log("Loaded groups");
            await LoadParallax(map, rootTag);
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

        private static async Task<bool> LoadBGM(Map map, NbtCompound mapTag)
        {
            NbtTag nbtTag = mapTag.Get("audbgm");
            if (!(nbtTag is ICollection<NbtTag>))
            {
                return false;
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
            return true;
        }

        private static async Task<bool> LoadSFX(Map map, NbtCompound mapTag)
        {
            NbtTag nbtTag = mapTag.Get("audsfx");
            if (!(nbtTag is ICollection<NbtTag>))
            {
                return false;
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
            return true;
        }

        private static async Task<bool> LoadDoors(Map map, NbtCompound mapTag)
        {
            NbtTag nbtTag = mapTag.Get("doors");
            if (!(nbtTag is ICollection<NbtTag>))
            {
                return false;
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
            return true;
        }

        private static async Task<bool> LoadTriggers(Map map, NbtCompound mapTag)
        {
            NbtTag nbtTag = mapTag.Get("triggers");
            if (!(nbtTag is ICollection<NbtTag>))
            {
                return false;
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
            return true;
        }

        private static async Task<bool> LoadNPCs(Map map, NbtCompound mapTag)
        {
            NbtTag nbtTag = mapTag.Get("npcs");
            if (!(nbtTag is ICollection<NbtTag>))
            {
                return false;
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
            return true;
        }

        private static async Task<bool> LoadNPCPaths(Map map, NbtCompound mapTag)
        {
            NbtTag nbtTag = mapTag.Get("paths");
            if (!(nbtTag is ICollection<NbtTag>))
            {
                return false;
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
            return true;
        }

        private static async Task<bool> LoadNPCAreas(Map map, NbtCompound mapTag)
        {
            NbtTag areaTag = mapTag.Get("areas");
            if (!(areaTag is ICollection<NbtTag>))
            {
                return false;
            }

            foreach (NbtCompound areaCompound in (IEnumerable<NbtTag>)areaTag)
            {
                Map.Area area = new Map.Area();
                area.Name = areaCompound.Get<NbtString>("name").Value;
                int left = areaCompound.Get<NbtInt>("x").Value;
                int top = areaCompound.Get<NbtInt>("y").Value;
                int width = areaCompound.Get<NbtInt>("w").Value;
                int height = areaCompound.Get<NbtInt>("h").Value;
                area.Rectangle = new IntRect(left, top, width, height);
                map.Areas.Add(area);
            }
            return true;
        }

        private static void LoadCrowds(Map map, NbtCompound mapTag)
        {
        }

        private static async Task<bool> LoadSpawns(Map map, NbtCompound mapTag)
        {
            NbtTag nbtTag = mapTag.Get("spawns");
            if (!(nbtTag is ICollection<NbtTag>))
            {
                return false;
            }

            foreach (NbtCompound nbtCompound in (IEnumerable<NbtTag>)nbtTag)
            {
                Map.EnemySpawn enemySpawn = new Map.EnemySpawn();
                enemySpawn.X = nbtCompound.Get<NbtInt>("x").Value;
                enemySpawn.Y = nbtCompound.Get<NbtInt>("y").Value;
                enemySpawn.Width = nbtCompound.Get<NbtInt>("w").Value;
                enemySpawn.Height = nbtCompound.Get<NbtInt>("h").Value;
                enemySpawn.Enemies = new List<Map.Enemy>();
                NbtList nbtList1 = nbtCompound.Get<NbtList>("enids");
                NbtList nbtList2 = nbtCompound.Get<NbtList>("enfreqs");
                for (int tagIndex = 0; tagIndex < nbtList1.Count; ++tagIndex)
                {
                    NbtString nbtShort = nbtList1.Get<NbtString>(tagIndex);
                    NbtByte nbtByte = nbtList2.Get<NbtByte>(tagIndex);
                    enemySpawn.Enemies.Add(new Map.Enemy()
                    {
                        EnemyName = nbtShort.Value,
                        Chance = nbtByte.ByteValue
                    });
                }
                map.Spawns.Add(enemySpawn);
            }
            return true;
        }

        private static async Task<bool> LoadCollisions(Map map, NbtCompound mapTag)
        {
            NbtTag nbtTag = mapTag.Get("mesh");
            if (!(nbtTag is ICollection<NbtTag>))
            {
                return false;
            }

            foreach (NbtList nbtList in (IEnumerable<NbtTag>)nbtTag)
            {
                List<Vector2f> points = new List<Vector2f>();
                for (int tagIndex = 0; tagIndex < nbtList.Count; tagIndex += 2)
                {
                    int x = nbtList.Get<NbtInt>(tagIndex).Value;
                    int y = nbtList.Get<NbtInt>(tagIndex + 1).Value;
                    points.Add(new Vector2f(x, y));
                }
                Mesh mesh = new Mesh(points);
                map.Mesh.Add(mesh);
            }
            return true;
        }

        private static void LoadTileAnimations(Map map, NbtCompound mapTag)
        {
            NbtTag nbtTag = mapTag.Get("anim");
            if (!(nbtTag is ICollection<NbtTag>))
            {
                return;
            }

            foreach (NbtCompound nbtCompound in (IEnumerable<NbtTag>)nbtTag)
            {
                NbtInt nbtInt = nbtCompound.Get<NbtInt>("tid");
                if (nbtInt != null)
                {
                    Map.TileAnimation tileAnimation = new Map.TileAnimation()
                    {
                        Id = nbtCompound.Get<NbtInt>("id").Value,
                        Frames = nbtCompound.Get<NbtInt>("fc").Value,
                        SkipVert = nbtCompound.Get<NbtInt>("vs").Value,
                        SkipHoriz = nbtCompound.Get<NbtInt>("hs").Value,
                        Speed = nbtCompound.Get<NbtFloat>("fs").Value
                    };
                    map.TileAnimationProperties.Add(nbtInt.Value, tileAnimation);
                }
            }
        }

        private static async Task<bool> LoadTileGroups(Map map, NbtCompound mapTag)
        {
            NbtTag nbtTag1 = mapTag.Get("tiles");
            if (!(nbtTag1 is ICollection<NbtTag>))
            {
                return false;
            }

            foreach (NbtTag nbtTag2 in (IEnumerable<NbtTag>)nbtTag1)
            {
                if (nbtTag2 is NbtCompound)
                {
                    // grab tiles
                    NbtCompound nbtCompound = (NbtCompound)nbtTag2;
                    int num1 = nbtCompound.Get<NbtInt>("depth").Value;
                    int num2 = nbtCompound.Get<NbtInt>("x").Value;
                    int num3 = nbtCompound.Get<NbtInt>("y").Value;
                    int num4 = nbtCompound.Get<NbtInt>("w").Value;

                    byte[] src = nbtCompound.Get<NbtByteArray>("tiles").Value;
                    ushort[] dst = new ushort[src.Length / 2];
                    Buffer.BlockCopy(src, 0, dst, 0, src.Length);
                    Map.Group group = new Map.Group()
                    {
                        Depth = num1,
                        X = num2,
                        Y = num3,
                        Width = num4,
                        Height = dst.Length / 2 / num4,
                        Tiles = dst
                    };
                    map.Groups.Add(group);
                }
            }
            return true;
        }

        private static async Task<bool> LoadParallax(Map map, NbtCompound mapTag)
        {
            NbtTag nbtTag = mapTag.Get("parallax");
            if (!(nbtTag is ICollection<NbtTag>))
            {
                return false;
            }

            foreach (NbtCompound nbtCompound in (IEnumerable<NbtTag>)nbtTag)
            {
                string stringValue = nbtCompound.Get<NbtString>("spr").StringValue;
                float floatValue1 = nbtCompound.Get<NbtFloat>("vx").FloatValue;
                float floatValue2 = nbtCompound.Get<NbtFloat>("vy").FloatValue;
                float floatValue3 = nbtCompound.Get<NbtFloat>("x").FloatValue;
                float floatValue4 = nbtCompound.Get<NbtFloat>("y").FloatValue;
                float floatValue5 = nbtCompound.Get<NbtFloat>("w").FloatValue;
                float floatValue6 = nbtCompound.Get<NbtFloat>("h").FloatValue;
                Map.Parallax parallax = new Map.Parallax()
                {
                    Sprite = stringValue,
                    Vector = new Vector2f(floatValue1, floatValue2),
                    Area = new IntRect((int)floatValue3, (int)floatValue4, (int)floatValue5, (int)floatValue6),
                    Depth = short.MinValue
                };
                map.Parallaxes.Add(parallax);
            }
            return true;
        }
    }
}
