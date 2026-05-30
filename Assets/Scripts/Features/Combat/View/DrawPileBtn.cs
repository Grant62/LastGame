namespace Features.Combat.View
{
    public partial class DrawPileBtn : CardPileBtn
    {
        protected override void RefreshDisplay()
        {
            base.RefreshDisplay();
            Count.text = GetPileCount().ToString();
        }

        protected override int GetPileCount()
        {
            return mModel?.DrawPile.Count ?? 0;
        }
    }
}