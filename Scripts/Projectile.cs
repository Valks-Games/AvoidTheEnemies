namespace AvoidTheEnemies;

public partial class Projectile : Node2D
{
    public Vector2 Target { get; set; }

    GTimer timerDestroy;
    Vector2 dir;

    public override void _Ready()
    {
        timerDestroy = new(this, 5000);
        timerDestroy.Finished += QueueFree;
        timerDestroy.Start();

        dir = (Target - Position).Normalized();
        Rotation = dir.Angle() + Mathf.Pi / 2;

        GetNode<Area2D>("Area2D").BodyEntered += body =>
        {
            if (body is Blob enemy)
            {
                Level.Enemies.Remove(enemy.GetInstanceId());
                enemy.QueueFree();
            }
        };
    }

    public override void _PhysicsProcess(double delta)
    {
        var speed = 10;
        Position += dir * speed;
    }
}
