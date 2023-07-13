namespace AvoidTheEnemies;

public partial class XPOrb : AnimatedSprite2D
{
    public event Action<Player> OnPickup;

    public int Value { get; set; }

    public override void _Ready()
    {
        GetNode<Area2D>("Area2D").BodyEntered += body =>
        {
            if (body is Player player)
            {
                OnPickup?.Invoke(player);

                // Destroy this xp orb
                QueueFree();
            }
        };
    }
}
