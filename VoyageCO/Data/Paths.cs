using System;
using System.IO;

namespace VCO.Data
{
	internal static class Paths
	{
        // this class is a clusterfuck, and by adding onto it i'm making it worse
        // however, that can be handled later!


        public static readonly string RESOURCES = "Data" + Path.DirectorySeparatorChar;

        public static readonly string SFX = Path.Combine(Paths.RESOURCES, "Audio", "SFX") + Path.DirectorySeparatorChar;

		public static readonly string BGM = Path.Combine(Paths.RESOURCES, "Audio", "BGM") + Path.DirectorySeparatorChar;

        public static readonly string AUDIO = Path.Combine(Paths.RESOURCES, "Audio", "") + Path.DirectorySeparatorChar;

        public static readonly string DATA = Path.Combine(Paths.RESOURCES, "Content", "") + Path.DirectorySeparatorChar;

        public static readonly string GRAPHICS = Path.Combine(Paths.RESOURCES, "Graphics", "") + Path.DirectorySeparatorChar;

        /// <summary>
        /// Corresponds to Resources/Data/LUA/
        /// </summary>
        public static readonly string DATA_LUA = DATA + "LUA" + Path.DirectorySeparatorChar;

        /// <summary>
        /// Corresponds to Resources/Data/Enemies
        /// </summary>
        public static readonly string DATA_ENEMIES = DATA + "Enemies" + Path.DirectorySeparatorChar;

        #region  Generic SFX
        /// <summary>
        /// Corresponds to Resources/Audio/SFX/Menu/
        /// </summary>
        public static readonly string SFX_MENU = SFX + "Menu" + Path.DirectorySeparatorChar;

        /// <summary>
        /// Corresponds to Resources/Audio/SFX/Text/
        /// </summary>
        public static readonly string SFX_TEXT = SFX + "Text" + Path.DirectorySeparatorChar;

        /// <summary>
        /// Corresponds to Resources/Audio/SFX/Tiles/
        /// </summary>
        public static readonly string SFX_TILES = SFX + "Tiles" + Path.DirectorySeparatorChar;
        #endregion

        #region Battle SFX

        /// <summary>
        /// Corresponds to Resources/Audio/SFX/Battle/
        /// </summary>
        public static readonly string SFX_BATTLE = SFX + "Battle" + Path.DirectorySeparatorChar;

        /// <summary>
        /// Corresponds to Resources/Audio/SFX/Battle/Combos/
        /// </summary>
        public static readonly string SFX_BATTLE_COMBO = SFX_BATTLE + "Combos" + Path.DirectorySeparatorChar;

        /// <summary>
        /// Corresponds to Resources/Audio/SFX/Battle/AUX/
        /// </summary>
        public static readonly string SFX_BATTLE_AUX = SFX_BATTLE + "AUXSFX" + Path.DirectorySeparatorChar;

        /// <summary>
        /// Corresponds to Resources/Audio/SFX/Battle/Jingles/
        /// </summary>
        public static readonly string SFX_BATTLE_JINGLES = SFX_BATTLE + "Jingles" + Path.DirectorySeparatorChar;
        #endregion

        #region  BGM

        /// <summary>
        /// Corresponds to Resources/Audio/BGM/BattleMusic/
        /// </summary>
        public static readonly string BGM_BATTLE = BGM + "Battle" + Path.DirectorySeparatorChar;

        /// <summary>
        /// Corresponds to Resources/Audio/BGM/Overworld/
        /// </summary>
        public static readonly string BGM_OVERWORLD = BGM + "Overworld" + Path.DirectorySeparatorChar;

        #endregion

        #region Graphics

        /// <summary>
        /// Corresponds to Resources/Graphics/PartyMembers/
        /// </summary>
        public static readonly string GRAPHICS_PARTYMEMBERS = GRAPHICS + "PartyMembers" + Path.DirectorySeparatorChar;

        /// <summary>
        /// Corresponds to Resources/Graphics/Enemies/
        /// </summary>
        public static readonly string GRAPHICS_ENEMIES = GRAPHICS + "Enemies" + Path.DirectorySeparatorChar;

        /// <summary>
        /// Corresponds to Resources/Graphics/NPCs/
        /// </summary>
        public static readonly string GRAPHICS_NPCS = GRAPHICS + "NPCs" + Path.DirectorySeparatorChar;

        /// <summary>
        /// Corresponds to Resources/Graphics/Battle/
        /// </summary>
        public static readonly string GRAPHICS_BATTLE = GRAPHICS + "Battle" + Path.DirectorySeparatorChar;

        /// <summary>
        /// Corresponds to Resources/Graphics/MapTilesets/
        /// </summary>
        public static readonly string GRAPHICS_MAPGRAPHICS = GRAPHICS + "MapTilesets" + Path.DirectorySeparatorChar;

        #endregion

        #region Root Paths
        public static readonly string AUX_GRAPHICS = Path.Combine(Paths.GRAPHICS, "_AUX", "") + Path.DirectorySeparatorChar;

		public static readonly string MAPS = Path.Combine(Paths.RESOURCES, "Maps", "") + Path.DirectorySeparatorChar;

		public static readonly string AUXFILES = Path.Combine(Paths.RESOURCES, "AUXFiles", "") + Path.DirectorySeparatorChar;

		public static readonly string TEXT = Path.Combine(Paths.RESOURCES, "Text", "") + Path.DirectorySeparatorChar;

		public static readonly string BATTLE_SWIRL = Path.Combine(Paths.GRAPHICS, "swirl", "") + Path.DirectorySeparatorChar;
        #endregion
    }
}
