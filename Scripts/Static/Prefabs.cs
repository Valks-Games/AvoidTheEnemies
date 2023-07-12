namespace AvoidTheEnemies;

public static class Prefabs
{
    public static PackedScene Blob { get; } = Load("blob");
    public static PackedScene Options { get; } = Load("UI/options");

    private static PackedScene Load(string path) =>
        GD.Load<PackedScene>($"res://Scenes/Prefabs/{path}.tscn");
}
