namespace AvoidTheEnemies;

public enum CardUpgradeType
{
    Speed,
    Health,
    MaxHealth,
    Firerate
}

public partial class UICard : MarginContainer
{
    [Export] public Label CardTitle { get; set; }
    [Export] public VBoxContainer VBox { get; set; }

    public event Action OnPicked;

    const int ANIMATION_DELAY = 50;
    const float ANIMATE_SCALE = 1.05f;
    const float ANIMATION_DURATION = 0.2f;

    GTimer timerAnimationBegin;
    GTimer timerAnimationEnd;
    GTween tweenAnimate;

    PanelContainer panelContainer;

    public CardUpgradeType CardUpgradeType { get; set; }
    int totalCards;
    int columns;

    // I can't think of a better way to do this. At first I thought maybe I could
    // use a constructor but I'm not sure how to make use of a constructor when
    // we have to do GD.Load<PackedScene>("...").Instantiate<UICard>();
    public void PreInit(int totalCards, int columns)
    {
        this.totalCards = totalCards;
        this.columns = columns;
    }

    public override void _Ready()
    {
        panelContainer = GetNodeOrNull<PanelContainer>("PanelContainer");

        // Small delay before the animation begins
        // This is to prevent the animations spazing out went the cursor moves over
        // the cards rapidly
        timerAnimationBegin = new(this, ANIMATION_DELAY);
        timerAnimationBegin.Finished += () =>
        {
            tweenAnimate = new GTween(this);
            tweenAnimate.Create();
            tweenAnimate.Animate("scale", Vector2.One * ANIMATE_SCALE, ANIMATION_DURATION);
        };

        // Small delay before the animation transitions back to original state
        // This is to prevent the animations spazing out went the cursor moves over
        // the cards rapidly
        timerAnimationEnd = new(this, ANIMATION_DELAY);
        timerAnimationEnd.Finished += () =>
        {
            tweenAnimate = new GTween(this);
            tweenAnimate.Create();
            tweenAnimate.Animate("scale", Vector2.One, ANIMATION_DURATION);
        };

        // Update the card size as soon as the card enters the scene
        UpdateSize();

        // Update the card size whenever the game window size changes
        GetTree().Root.SizeChanged += UpdateSize;

        MouseEntered += () =>
        {
            timerAnimationBegin.Start();
            timerAnimationEnd.Stop();
        };

        MouseExited += () =>
        {
            timerAnimationBegin.Stop();
            timerAnimationEnd.Start();
        };

        GuiInput += (inputEvent) =>
        {
            if (inputEvent is InputEventMouseButton mouse)
            {
                if (mouse.IsLeftClickPressed())
                {
                    GetTree().Paused = false;
                    Level.CardManager.RemoveCards();
                    OnPicked?.Invoke();
                }
            }
        };

        var player = Player.Instance;

        OnPicked += () =>
        {
            switch (CardUpgradeType)
            {
                case CardUpgradeType.Speed:
                    player.Speed += 10;
                    break;
                case CardUpgradeType.Health:
                    player.AddHealth(5);
                    break;
                case CardUpgradeType.MaxHealth:
                    player.AddMaxHealth(5);
                    break;
                case CardUpgradeType.Firerate:
                    player.IncreaseFirerate(50);
                    break;
            }
        };

        CardTitle.Text = CardUpgradeType + "";

        var cardUpgradeNote = (UICardUpgradeNote)Prefabs.CardUpgradeNote.Instantiate();

        switch (CardUpgradeType)
        {
            case CardUpgradeType.Speed:
                cardUpgradeNote.SetInfo(
                    type: CardUpgradeType,
                    before: player.Speed,
                    after: player.Speed + 10);
                break;
            case CardUpgradeType.Health:
                cardUpgradeNote.SetInfo(
                    type: CardUpgradeType,
                    before: player.Health,
                    after: player.Health + 5);
                break;
            case CardUpgradeType.MaxHealth:
                cardUpgradeNote.SetInfo(
                    type: CardUpgradeType,
                    before: player.MaxHealth,
                    after: player.MaxHealth + 5);
                break;
            case CardUpgradeType.Firerate:
                cardUpgradeNote.SetInfo(
                    type: CardUpgradeType,
                    before: player.Firerate,
                    after: player.Firerate - 50);
                break;
        }

        VBox.AddChild(cardUpgradeNote);
    }

    async void UpdateSize()
    {
        var windowSize = DisplayServer.WindowGetSize();
        var winFactorX = 4.5f;
        var winFactorY = 1.75f;

        // Most of these calculations were eye balled, none of this looks pretty
        // but it looks 90% decent for a max of 8 cards on the screen. The cards
        // look arguabely scrunched a bit after 5 cards start appearing but what
        // can you do.

        // If you want you can try experimenting with this yourself. Note that there
        // are 2 variables you can factor into these calculations and these are the
        // totalCards and columns.

        // Note that this rows variable is offset by 1. This was done intentionally
        // to help with the calculations below.
        var rows = (totalCards / (columns + 1));
        var rowFactor = rows * 0.2f;

        panelContainer.CustomMinimumSize =
            new Vector2
            (
                x: windowSize.X / winFactorX,
                y: windowSize.Y / (winFactorY * (1 + rowFactor))
            );

        // Need to wait one frame so PivotOffset calculation uses updated Size value
        await GUtils.WaitOneFrame(this);

        // Update pivot offset
        PivotOffset = Size / 2;
    }
}
