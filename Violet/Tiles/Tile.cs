using SFML.System;

namespace Violet.Tiles
{
    public struct Tile
    {
        public Tile(uint tileID, Vector2f position, bool flipHoriz, bool flipVert, bool flipDiag, ushort animId)
        {
            this.ID = tileID;
            this.Position = position;
            this.FlipHorizontal = flipHoriz;
            this.FlipVertical = flipVert;
            this.FlipDiagonal = flipDiag;
            this.AnimationId = animId;
        }

        public const uint SIZE = 8U;

        public readonly uint ID;

        public readonly Vector2f Position;

        public readonly bool FlipHorizontal;

        public readonly bool FlipVertical;

        public readonly bool FlipDiagonal;

        public readonly ushort AnimationId;
    }
}
