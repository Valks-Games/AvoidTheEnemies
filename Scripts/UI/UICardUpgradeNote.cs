namespace AvoidTheEnemies;

public partial class UICardUpgradeNote : Node
{
    [Export] public Label UpgradeType { get; set; }
    [Export] public Label UpgradeBefore { get; set; }
    [Export] public Label UpgradeAfter { get; set; }

    public void SetInfo(object type, object before, object after)
    {
        UpgradeType.Text = type + "";
        UpgradeBefore.Text = before + "";
        UpgradeAfter.Text = after + "";
    }
}
