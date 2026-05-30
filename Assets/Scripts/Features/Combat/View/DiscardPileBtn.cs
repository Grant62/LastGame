namespace Features.Combat.View
{
    public partial class DiscardPileBtn : CardPileBtn
    {
        protected override void RefreshDisplay()
        {
            base.RefreshDisplay();
            Count.text = GetPileCount().ToString();
        }

        protected override int GetPileCount()
        {
            return mModel?.DiscardPile.Count ?? 0;
        }
    }
}