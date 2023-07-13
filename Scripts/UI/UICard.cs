namespace AvoidTheEnemies;

public partial class UICard : MarginContainer
{
    const int ANIMATION_DELAY = 50;
    const float ANIMATE_SCALE = 1.05f;
    const float ANIMATION_DURATION = 0.2f;

    GTimer timerAnimationBegin;
    GTimer timerAnimationEnd;
    GTween tweenAnimate;

    PanelContainer panelContainer;

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
