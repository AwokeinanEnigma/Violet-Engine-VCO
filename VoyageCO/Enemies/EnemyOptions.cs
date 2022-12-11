namespace VCO.Data.Enemies
{
    internal enum EnemyOptions
    {
        None,
        IsBackgroundLayer,
        IsBoss,
        IsGhost = 4,
        IsImmortal = 8,
        IsInsect = 16,
        IsRobot = 32,
        NoChat = 64,
        NoTelepathy = 128,
        SelfDestruct = 256
    }
}