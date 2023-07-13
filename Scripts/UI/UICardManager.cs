namespace AvoidTheEnemies;

public partial class UICardManager : GridContainer
{
    public void AddCards(int totalCards)
    {
        var cardPrefab = GD.Load<PackedScene>("res://Scenes/Prefabs/UI/card.tscn");

        for (int i = 0; i < totalCards; i++)
        {
            var card = cardPrefab.Instantiate<UICard>();
            card.PreInit(totalCards, Columns);
            AddChild(card);
        }
    }

    public void RemoveCards()
    {
        this.QueueFreeChildren();
    }
}
