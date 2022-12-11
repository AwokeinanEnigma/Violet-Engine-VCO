namespace VCO.Actors.Animation
{
    internal enum AnimationType
    {
        INVALID,
        DIRECTION_MASK = 255,
        STANCE_MASK = 65280,
        EAST = 1,
        NORTHEAST,
        NORTH,
        NORTHWEST,
        WEST,
        SOUTHWEST,
        SOUTH,
        SOUTHEAST,
        STAND = 256,
        WALK = 512,
        RUN = 768,
        CROUCH = 1024,
        DEAD = 1280,
        IDLE = 1536,
        TALK = 1792,
        BLINK = 2048,
        CLIMB = 2304,
        SWIM = 2560,
        FLOAT = 2816,
        NAUSEA = 3072
    }
}