namespace ProjektLS22
{
    public class Building : ShopItem
    {
        public Building(char symbol, int price, string description) : base(symbol, price, description)
        {

        }

        public override bool CanPlace(Player p, Tile t, World w)
        {
            return t.IsEmpty() && t.color == p.color;
        }

        public virtual void OnDestroyed(Tile t, World w) { }
        public virtual void OnTurn(Player p, Tile t, World w) { }
    }
}