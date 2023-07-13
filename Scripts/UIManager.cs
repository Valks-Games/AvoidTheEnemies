namespace AvoidTheEnemies;

public partial class UIManager : CanvasLayer
{
    public void RegisterEvents()
    {
        Level.Player.OnXpChange += xp =>
        {
            Level.XPBar.Value += xp;

            if (Level.XPBar.Value >= 100)
            {
                Level.OnLevelChange?.Invoke();
                Level.XPBar.Value = 0;
                Level.CardManager.AddCards(3);
            }
        };

        Level.Player.OnHealthChange += player =>
        {
            Level.LabelHealth.Text = $"Health: {player.Health}/{player.MaxHealth}";
        };
    }
}
