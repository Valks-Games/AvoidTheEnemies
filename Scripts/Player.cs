namespace AvoidTheEnemies;

public partial class Player : CharacterBody2D
{
    public static Player Instance { get; private set; }

    public event Action<int> OnXpChange;
    public event Action<Player> OnHealthChange;

    public float Speed    { get; set; } = 50;
    public float Friction { get; set; } = 0.1f;

    GTimer timerLasers;
    int xp;

    public int Firerate { get; private set; } = 1000; // fire every 1 second
    public int MaxHealth { get; private set; } = 3;
    public int Health { get; private set; } = 3;

    public override void _Ready()
    {
        Instance = this;
        timerLasers = new(this, Firerate) { Loop = true };
        timerLasers.Finished += ShootLaser;
        timerLasers.Start();
    }

    public override void _PhysicsProcess(double delta)
    {
        // Velocity is mutiplied by delta for us already
        Velocity += GetMovementInput() * Speed;
        Velocity = Velocity.Lerp(Vector2.Zero, Friction);

        MoveAndSlide();
    }

    public void IncreaseFirerate(int removeMs)
    {
        Firerate -= removeMs;
        Firerate = Mathf.Max(Firerate, 10);

        timerLasers.Start(Firerate);
    }

    public void AddMaxHealth(int maxHealth)
    {
        MaxHealth += maxHealth;
        OnHealthChange?.Invoke(this);
    }

    public void AddHealth(int health)
    {
        Health += health;
        Health = Mathf.Min(Health, MaxHealth);
        OnHealthChange?.Invoke(this);
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        OnHealthChange?.Invoke(this);
    }

    public void AddXP(int xp)
    {
        this.xp += xp;
        OnXpChange?.Invoke(xp);
    }

    void ShootLaser()
    {
        if (Level.Enemies.Count == 0)
            return;

        var laser = (Projectile)Prefabs.LaserBlue.Instantiate();
        laser.Position = Position;
        laser.Target = GetClosestEnemy().Position;
        GetTree().Root.AddChild(laser);
    }

    Blob GetClosestEnemy()
    {
        Blob closestEnemy = null;
        var minDist = Mathf.Inf;

        foreach (var enemy in Level.Enemies.Values)
        {
            var dist = enemy.Position.DistanceTo(Position);

            if (dist < minDist)
            {
                minDist = dist;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }

    public Vector2 GetMovementInput(string prefix = "")
    {
        prefix += !string.IsNullOrWhiteSpace(prefix) ? "_" : "";

        return Input.GetVector(
            $"{prefix}move_left", $"{prefix}move_right",
            $"{prefix}move_up", $"{prefix}move_down");
    }

    public Vector2 GetMovementInputRaw(string prefix = "") => 
        GetMovementInput(prefix).Round();
}
