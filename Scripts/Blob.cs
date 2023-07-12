namespace AvoidTheEnemies;

/*
 * A dampening of 10 and speed of 1000 give nice movement with little to no
 * acceleration
 */
public partial class Blob : RigidBody2D
{
    [Export] public float Speed { get; set; } = 1000;

    public override void _PhysicsProcess(double delta)
    {
        var dir = (Level.Player.Position - Position).Normalized();
        var vel = dir * Speed;

        ApplyCentralForce(vel);
    }
}
