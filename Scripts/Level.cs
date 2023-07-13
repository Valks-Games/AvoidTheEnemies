namespace AvoidTheEnemies;

public partial class Level : Node
{
    public static Player Player { get; private set; }
    public static Dictionary<ulong, Blob> Enemies { get; } = new();
    public static ProgressBar XPBar { get; private set; }
    public static UICardManager CardManager { get; private set; }
    public static Label LabelHealth { get; private set; }
    public static UIManager UIManager { get; private set; }

    // This is the first time I have ever used a static Action. I did not add the
    // event keyword because then I would not have been able to invoke it from
    // UIManager. I am not sure if this is the right way to do things.
    public static Action OnLevelChange;

    public event Action OnDifficultyIncrease;
    public event Action OnSpawnEnemies;

    const int MAX_ENEMIES_ON_SCREEN = 1000;
    const int SPAWNER_INTERVAL = 1000;
    const int DIFFICULTY_INCREASE_INTERVAL = 60000;
    const int ENEMY_SPAWN_DISTANCE_FROM_PLAYER = 2500;
    const int ENEMY_RANDOM_SPAWN_DISTANCE_FROM_PLAYER = 500;

    // The number of enemies to add to the world spawns when the difficulty increases
    int enemiesToAddForDifficultyIncrease = 1;
    int maxEnemiesToAddForDifficultyIncrease = 10;

    // Timers
    GTimer timerSpawnInterval;
    GTimer timerDifficultyIncrease;

    // Nodes
    Node nodeEnemies;

    // Trackers
    int enemiesToSpawn = 1;
    int maxEnemiesForDifficulty = 20;

    public override void _Ready()
    {
        UIManager = GetNode<UIManager>("CanvasLayer");
        LabelHealth = GetNode<Label>("CanvasLayer/Health");
        Player = GetNode<Player>("Player");
        nodeEnemies = GetNode("Enemies");
        XPBar = GetNode<ProgressBar>("CanvasLayer/ProgressBar");
        CardManager = GetNode<UICardManager>("CanvasLayer/MarginContainer/GridContainer");

        timerSpawnInterval = new(this, SPAWNER_INTERVAL) { Loop = true };
        timerSpawnInterval.Finished += SpawnEnemies;
        timerSpawnInterval.Start();

        timerDifficultyIncrease = new(this, DIFFICULTY_INCREASE_INTERVAL) { Loop = true };
        timerDifficultyIncrease.Finished += IncreaseDifficulty;
        timerDifficultyIncrease.Start();

        UIManager.RegisterEvents();

        OnLevelChange += () =>
        {
            GetTree().Paused = true;
        };
    }

    void IncreaseDifficulty()
    {
        maxEnemiesForDifficulty += maxEnemiesToAddForDifficultyIncrease;
        maxEnemiesForDifficulty = Mathf.Min(maxEnemiesForDifficulty, MAX_ENEMIES_ON_SCREEN);
        enemiesToSpawn += enemiesToAddForDifficultyIncrease;
        
        OnDifficultyIncrease?.Invoke();
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            if (Enemies.Count >= maxEnemiesForDifficulty)
                break;

            var blob = (Blob)Prefabs.Blob.Instantiate();
            Enemies.Add(blob.GetInstanceId(), blob);

            var dist = ENEMY_SPAWN_DISTANCE_FROM_PLAYER + GD.RandRange(0, ENEMY_RANDOM_SPAWN_DISTANCE_FROM_PLAYER);
            var theta = (float)GD.RandRange(0, Mathf.Pi * 2);
            var dir = new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));

            blob.Position = Player.Position + dir * dist;
            nodeEnemies.AddChild(blob);
        }

        OnSpawnEnemies?.Invoke();
    }
}
