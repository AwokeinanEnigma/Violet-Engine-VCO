using fNbt;
using System.Collections.Generic;
using VCO.Battle;

namespace VCO.Data.Enemies
{
    internal class EnemyData
    {
        // (get) Token: 0x06000086 RID: 134 RVA: 0x0000530A File Offset: 0x0000350Af
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
        // (get) Token: 0x0600008A RID: 138 RVA: 0x0000532A File Offset: 0x0000352A
        public int Experience => this.experience;
        // (get) Token: 0x0600008D RID: 141 RVA: 0x00005342 File Offset: 0x00003542
        public EnemyOptions Options => this.options;
        // (get) Token: 0x0600008E RID: 142 RVA: 0x0000534A File Offset: 0x0000354A
        public EnemyImmunities Immunities => this.immunities;
        // (get) Token: 0x0600008F RID: 143 RVA: 0x00005352 File Offset: 0x00003552
        public int Level => this.level;
        // (get) Token: 0x06000090 RID: 144 RVA: 0x0000535A File Offset: 0x0000355A
        public int HP => this.hp;
        // (get) Token: 0x06000091 RID: 145 RVA: 0x00005362 File Offset: 0x00003562
        public int PP => this.pp;
        // (get) Token: 0x06000092 RID: 146 RVA: 0x0000536A File Offset: 0x0000356A
        public int Offense => this.offense;
        // (get) Token: 0x06000093 RID: 147 RVA: 0x00005372 File Offset: 0x00003572
        public int Defense => this.defense;
        // (get) Token: 0x06000094 RID: 148 RVA: 0x0000537A File Offset: 0x0000357A
        public int Speed => this.speed;
        // (get) Token: 0x06000095 RID: 149 RVA: 0x00005382 File Offset: 0x00003582
        public int Guts => this.guts;
        // (get) Token: 0x06000096 RID: 150 RVA: 0x0000538A File Offset: 0x0000358A
        public int IQ => this.iq;
        // (get) Token: 0x06000097 RID: 151 RVA: 0x00005392 File Offset: 0x00003592
        public int Luck => this.luck;
        // (get) Token: 0x06000098 RID: 152 RVA: 0x0000539A File Offset: 0x0000359A
        public float ModifierElectric => this.modElectric;
        // (get) Token: 0x06000099 RID: 153 RVA: 0x000053A2 File Offset: 0x000035A2
        public float ModifierExplosive => this.modExplosive;
        // (get) Token: 0x0600009A RID: 154 RVA: 0x000053AA File Offset: 0x000035AA
        public float ModifierFire => this.modFire;
        // (get) Token: 0x0600009B RID: 155 RVA: 0x000053B2 File Offset: 0x000035B2
        public float ModifierIce => this.modIce;
        // (get) Token: 0x0600009C RID: 156 RVA: 0x000053BA File Offset: 0x000035BA
        public float ModifierNausea => this.modNausea;
        // (get) Token: 0x0600009D RID: 157 RVA: 0x000053C2 File Offset: 0x000035C2
        public float ModifierPhysical => this.modPhysical;
        // (get) Token: 0x0600009E RID: 158 RVA: 0x000053CA File Offset: 0x000035CA
        public float ModifierPoison => this.modPoison;
        // (get) Token: 0x0600009F RID: 159 RVA: 0x000053D2 File Offset: 0x000035D2
        public float ModifierWater => this.modWater;


        public Dictionary<string, string> strings;
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

    }
}
