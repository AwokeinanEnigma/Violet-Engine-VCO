﻿using fNbt;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.IO;
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
            MapLoader.LoadHeader(map, rootTag);
            MapLoader.LoadBGM(map, rootTag);
            MapLoader.LoadSFX(map, rootTag);
            MapLoader.LoadDoors(map, rootTag);
            MapLoader.LoadTriggers(map, rootTag);
            MapLoader.LoadNPCs(map, rootTag);
            MapLoader.LoadNPCPaths(map, rootTag);
            MapLoader.LoadNPCAreas(map, rootTag);
            MapLoader.LoadCrowds(map, rootTag);
            MapLoader.LoadSpawns(map, rootTag);
            MapLoader.LoadCollisions(map, rootTag);
            MapLoader.LoadTileGroups(map, rootTag);
            MapLoader.LoadParallax(map, rootTag);
            Debug.LDebug($"Loaded map data in {(DateTime.Now.Ticks - ticks) / 10000L}ms");
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

            foreach (NbtCompound nbtCompound1 in (IEnumerable<NbtTag>)nbtTag)
            {
                Map.NPC npc = new Map.NPC();
                npc.X = nbtCompound1.Get<NbtInt>("x").Value;
                npc.Y = nbtCompound1.Get<NbtInt>("y").Value;
                npc.Name = nbtCompound1.Get<NbtString>("name").Value;
                npc.Direction = nbtCompound1.Get<NbtByte>("dir").Value;
                npc.Enabled = nbtCompound1.Get<NbtByte>("en").Value == 1;
                npc.Mode = nbtCompound1.Get<NbtByte>("mov").Value;
                nbtCompound1.TryGet<NbtString>("spr", out NbtString result1);
                npc.Sprite = result1?.Value;
                nbtCompound1.TryGet<NbtInt>("w", out NbtInt result2);
                npc.Width = result2 != null ? result2.Value : 0;
                nbtCompound1.TryGet<NbtInt>("h", out NbtInt result3);
                npc.Height = result3 != null ? result3.Value : 0;
                nbtCompound1.TryGet<NbtFloat>("spd", out NbtFloat result4);
                npc.Speed = result4 != null ? result4.FloatValue : 1f;
                nbtCompound1.TryGet<NbtShort>("dly", out NbtShort result5);
                npc.Delay = result5 != null ? result5.ShortValue : (short)0;
                nbtCompound1.TryGet<NbtShort>("dst", out NbtShort result6);
                npc.Distance = result6 != null ? result6.ShortValue : (short)20;
                nbtCompound1.TryGet<NbtString>("cnstr", out NbtString result7);
                npc.Constraint = result7 != null ? result7.Value : "";
                nbtCompound1.TryGet<NbtByte>("shdw", out NbtByte result8);
                npc.Shadow = result8 == null || result8.ShortValue != 0;
                nbtCompound1.TryGet<NbtByte>("cls", out NbtByte result9);
                npc.Solid = result9 == null || result9.ShortValue != 0;
                nbtCompound1.TryGet<NbtByte>("stky", out NbtByte result10);
                npc.Sticky = result10 == null || result10.ShortValue != 0;
                nbtCompound1.TryGet<NbtInt>("dpth", out NbtInt result11);
                npc.DepthOverride = result11 != null ? result11.IntValue : int.MinValue;
                npc.Flag = nbtCompound1.Get<NbtShort>("flag").Value;
                npc.Text = new List<Map.NPCtext>();
                NbtCompound nbtCompound2 = nbtCompound1.Get<NbtCompound>("entries");
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
                NbtCompound nbtCompound3 = nbtCompound1.Get<NbtCompound>("tele");
                if (nbtCompound3 != null)
                {
                    for (int index = 0; index < nbtCompound3.Count; index += 2)
                    {
                        string str = nbtCompound3.Get<NbtString>(string.Format("t{0}", index / 2)).Value;
                        int num = nbtCompound3.Get<NbtShort>(string.Format("f{0}", index / 2)).Value;
                        Map.NPCtext npCtext = new Map.NPCtext()
                        {
                            ID = str,
                            Flag = num
                        };
                        npc.TeleText.Add(npCtext);
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
                NbtList nbtList = nbtCompound.Get<NbtList>("coords");
                for (int tagIndex = 0; tagIndex < nbtList.Count; tagIndex += 2)
                {
                    Vector2f vector2f = new Vector2f(((NbtInt)nbtList[tagIndex]).Value, ((NbtInt)nbtList[tagIndex + 1]).Value);
                    vector2fList.Add(vector2f);
                }
                path.Points = vector2fList;
                map.Paths.Add(path);
            }
        }

        private static void LoadNPCAreas(Map map, NbtCompound mapTag)
        {
            NbtTag nbtTag = mapTag.Get("areas");
            if (!(nbtTag is ICollection<NbtTag>))
            {
                return;
            }

            foreach (NbtCompound nbtCompound in (IEnumerable<NbtTag>)nbtTag)
            {
                Map.Area area = new Map.Area();
                area.Name = nbtCompound.Get<NbtString>("name").Value;
                int left = nbtCompound.Get<NbtInt>("x").Value;
                int top = nbtCompound.Get<NbtInt>("y").Value;
                int width = nbtCompound.Get<NbtInt>("w").Value;
                int height = nbtCompound.Get<NbtInt>("h").Value;
                area.Rectangle = new IntRect(left, top, width, height);
                map.Areas.Add(area);
            }
        }

        private static void LoadCrowds(Map map, NbtCompound mapTag)
        {
        }

        private static void LoadSpawns(Map map, NbtCompound mapTag)
        {
            NbtTag nbtTag = mapTag.Get("spawns");
            if (!(nbtTag is ICollection<NbtTag>))
            {
                return;
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
        }

        private static void LoadCollisions(Map map, NbtCompound mapTag)
        {
            NbtTag nbtTag = mapTag.Get("mesh");
            if (!(nbtTag is ICollection<NbtTag>))
            {
                return;
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

        private static void LoadTileGroups(Map map, NbtCompound mapTag)
        {
            NbtTag nbtTag1 = mapTag.Get("tiles");
            if (!(nbtTag1 is ICollection<NbtTag>))
            {
                return;
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
        }

        private static void LoadParallax(Map map, NbtCompound mapTag)
        {
            NbtTag nbtTag = mapTag.Get("parallax");
            if (!(nbtTag is ICollection<NbtTag>))
            {
                return;
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
        }
    }
}
