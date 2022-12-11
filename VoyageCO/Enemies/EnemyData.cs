using fNbt;
using System.Collections.Generic;
using VCO.Battle;

namespace VCO.Data.Enemies
{
    internal class EnemyData
    {
        #region Properties
        public string QualifiedName => this.qualifiedName;
        public string AIName => this.aiName;
        public string GetStringQualifiedName(string stringName)
        {
            if (!this.strings.TryGetValue(stringName, out string empty))
            {
                empty = string.Empty;
            }
            return empty;
        }

        public string BackgroundName => this.bbgName;

        public string MusicName => this.bgmName;

        public string SpriteName => this.spriteName;
        public int Experience => this.experience;
        public EnemyOptions Options => this.options;
        public EnemyImmunities Immunities => this.immunities;
        public int Level => this.level;
        public int HP => this.hp;
        public int PP => this.pp;
        public int Offense => this.offense;
        public int Defense => this.defense;
        public int Speed => this.speed;
        public int Guts => this.guts;
        public int IQ => this.iq;
        public int Luck => this.luck;
        public float ModifierElectric => this.modElectric;
        public float ModifierExplosive => this.modExplosive;
        public float ModifierFire => this.modFire;
        public float ModifierIce => this.modIce;
        public float ModifierNausea => this.modNausea;
        public float ModifierPhysical => this.modPhysical;
        public float ModifierPoison => this.modPoison;
        public float ModifierWater => this.modWater;
        #endregion

        public Dictionary<string, string> strings;
        #region Fields
        public string Article;
        private string qualifiedName;
        private string bbgName;
        private string bgmName;
        private string spriteName;
        private readonly Dictionary<string, object> aiProperties;
        private int experience;

        private string aiName;
        private EnemyOptions options;
        private EnemyImmunities immunities;
        private int level;
        private int hp;
        private int pp;
        private int offense;
        private int defense;
        private int speed;
        private int guts;
        private int iq;
        private int luck;
        private float modElectric;
        private float modExplosive;
        private float modFire;
        private float modIce;
        private float modNausea;
        private float modPhysical;
        private float modPoison;
        private float modWater;
#endregion

        public EnemyData(NbtCompound tag)
        {
            this.strings = new Dictionary<string, string>();
            this.aiProperties = new Dictionary<string, object>();
            this.LoadBaseAttributes(tag);
            if (tag.TryGet<NbtCompound>("stats", out NbtCompound tag2))
            {
                this.LoadStats(tag2);
            }
            if (tag.TryGet<NbtCompound>("modifiers", out NbtCompound tag3))
            {
                this.LoadModifiers(tag3);
            }
            if (tag.TryGet<NbtCompound>("str", out NbtCompound stringsTag))
            {
                this.LoadStrings(stringsTag);
            }
        }

        public string MoverName;
        public string OverworldSprite;
        private void LoadBaseAttributes(NbtCompound tag)
        {
            if (tag.TryGet("overworldspr", out NbtTag nbtTag))
            {
                this.OverworldSprite = nbtTag.StringValue;
            }
            if (tag.TryGet("exp", out nbtTag))
            {
                this.experience = nbtTag.IntValue;
            }
            if (tag.TryGet("bbg", out nbtTag))
            {
                this.bbgName = nbtTag.StringValue;
            }
            if (tag.TryGet("bgm", out nbtTag))
            {
                this.bgmName = nbtTag.StringValue;
            }
            if (tag.TryGet("spr", out nbtTag))
            {
                this.spriteName = nbtTag.StringValue;
            }
            if (tag.TryGet("opt", out nbtTag))
            {
                this.options = (EnemyOptions)nbtTag.IntValue;
            }
            if (tag.TryGet("imm", out nbtTag))
            {
                this.immunities = (EnemyImmunities)nbtTag.IntValue;
            }

            if (tag.TryGet("ainame", out nbtTag))
            {
                aiName = nbtTag.StringValue;
            }

            if (tag.TryGet("movername", out nbtTag))
            {
                MoverName = nbtTag.StringValue;
            }
        }
        private void LoadStats(NbtCompound tag)
        {
            if (tag.TryGet("hp", out NbtTag nbtTag))
            {
                this.hp = nbtTag.IntValue;
            }
            if (tag.TryGet("pp", out nbtTag))
            {
                this.pp = nbtTag.IntValue;
            }
            if (tag.TryGet("lvl", out nbtTag))
            {
                this.level = nbtTag.IntValue;
            }
            if (tag.TryGet("off", out nbtTag))
            {
                this.offense = nbtTag.IntValue;
            }
            if (tag.TryGet("def", out nbtTag))
            {
                this.defense = nbtTag.IntValue;
            }
            if (tag.TryGet("spd", out nbtTag))
            {
                this.speed = nbtTag.IntValue;
            }
            if (tag.TryGet("gut", out nbtTag))
            {
                this.guts = nbtTag.IntValue;
            }
            if (tag.TryGet("iq", out nbtTag))
            {
                this.iq = nbtTag.IntValue;
            }
            if (tag.TryGet("lck", out nbtTag))
            {
                this.luck = nbtTag.IntValue;
            }
        }
        private void LoadModifiers(NbtCompound tag)
        {
            if (tag.TryGet("elec", out NbtTag nbtTag))
            {
                this.modElectric = nbtTag.FloatValue;
            }
            if (tag.TryGet("expl", out nbtTag))
            {
                this.modExplosive = nbtTag.FloatValue;
            }
            if (tag.TryGet("fire", out nbtTag))
            {
                this.modFire = nbtTag.FloatValue;
            }
            if (tag.TryGet("ice", out nbtTag))
            {
                this.modIce = nbtTag.FloatValue;
            }
            if (tag.TryGet("naus", out nbtTag))
            {
                this.modNausea = nbtTag.FloatValue;
            }
            if (tag.TryGet("phys", out nbtTag))
            {
                this.modPhysical = nbtTag.FloatValue;
            }
            if (tag.TryGet("pois", out nbtTag))
            {
                this.modPoison = nbtTag.FloatValue;
            }
            if (tag.TryGet("wet", out nbtTag))
            {
                this.modWater = nbtTag.FloatValue;
            }
        }

        public string PlayerFriendlyName;

        private void LoadStrings(NbtCompound stringsTag)
        {
            Article = stringsTag.Get("article").StringValue + " ";
            qualifiedName = stringsTag.Get("name").StringValue;
            PlayerFriendlyName = qualifiedName + " ";

            foreach (NbtTag nbtTag in stringsTag)
            {
                if (nbtTag is NbtString)
                {

                    this.strings.Add(nbtTag.Name, nbtTag.StringValue);
                }
            }
        }

        private object GetPropertyObject(NbtTag propertyTag)
        {
            object result = null;
            if (propertyTag is NbtByte || propertyTag is NbtShort || propertyTag is NbtInt)
            {
                result = propertyTag.IntValue;
            }
            else if (propertyTag is NbtFloat || propertyTag is NbtDouble)
            {
                result = propertyTag.FloatValue;
            }
            else if (propertyTag is NbtString)
            {
                result = propertyTag.StringValue;
            }

            return result;
        }

        public StatSet GetStatSet()
        {
            return new StatSet
            {
                Level = this.level,
                HP = this.hp,
                MaxHP = this.hp,
                PP = this.pp,
                MaxPP = this.pp,
                Offense = this.offense,
                Defense = this.defense,
                Speed = this.speed,
                Guts = this.guts,
                IQ = this.iq,
                Luck = this.luck
            };
        }



    }
}
