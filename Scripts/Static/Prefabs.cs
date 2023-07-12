namespace AvoidTheEnemies;

public static class Prefabs
{
    public static PackedScene XP_Orb { get; } = Load("xp_orb");
    public static PackedScene LaserBlue { get; } = Load("laser_blue");
    public static PackedScene Blob { get; } = Load("blob");
    public static PackedScene Options { get; } = Load("UI/options");

    private static PackedScene Load(string path) =>
        GD.Load<PackedScene>($"res://Scenes/Prefabs/{path}.tscn");
}
