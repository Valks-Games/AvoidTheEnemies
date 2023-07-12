namespace AvoidTheEnemies;

public partial class Player : CharacterBody2D
{
    public float Speed    { get; set; } = 50;
    public float Friction { get; set; } = 0.1f;

    GTimer timerLasers;

    public override void _Ready()
    {
        timerLasers = new(this, 1000) { Loop = true };
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
