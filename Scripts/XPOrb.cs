namespace AvoidTheEnemies;

public partial class XPOrb : AnimatedSprite2D
{
    public override void _Ready()
    {
        GetNode<Area2D>("Area2D").BodyEntered += body =>
        {
            if (body is Player)
            {
                QueueFree();
            }
        };
    }
}
