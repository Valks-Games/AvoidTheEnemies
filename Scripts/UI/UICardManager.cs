namespace AvoidTheEnemies;

public partial class UICardManager : GridContainer
{
    public void AddCards(int totalCards)
    {
        var cardPrefab = GD.Load<PackedScene>("res://Scenes/Prefabs/UI/card.tscn");
        var upgradeTypes = ((CardUpgradeType[])Enum.GetValues(typeof(CardUpgradeType))).ToList();;

        for (int i = 0; i < totalCards; i++)
        {
            var card = cardPrefab.Instantiate<UICard>();
            var upgradeTypeIndex = GD.RandRange(0, upgradeTypes.Count - 1);

            card.CardUpgradeType = upgradeTypes[upgradeTypeIndex];
            card.PreInit(totalCards, Columns);
            AddChild(card);

            // Ensure no duplicate cards
            upgradeTypes.RemoveAt(upgradeTypeIndex);
        }
    }

    public void RemoveCards()
    {
        this.QueueFreeChildren();
    }
}
