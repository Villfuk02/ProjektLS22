namespace ProjektLS22
{
    public abstract class ShopItem
    {
        public int price;
        public char symbol;
        public string description;

        public ShopItem(char symbol, int price, string description)
        {
            this.symbol = symbol;
            this.price = price;
            this.description = description;
        }

        public abstract bool CanPlace(Player p, Tile t, World w);

        public virtual void OnPlace(Player p, Tile t, World w) { }

    }
}