namespace ProjektLS22
{

    public class World
    {
        public int size;

        public Tile[,] tiles;

        public World(int size)
        {
            this.size = size;
            tiles = new Tile[size + 2, size + 2];
            for (int x = 0; x < size + 2; x++)
            {
                for (int y = 0; y < size + 2; y++)
                {
                    tiles[x, y] = new Tile(this, new Pos(x, y), x == 0 || x > size || y == 0 || y > size);
                }
            }
        }
    }
    public struct Pos
    {
        public int x;
        public int y;
        public Pos(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public static Pos UP = new Pos(0, -1);
        public static Pos DOWN = new Pos(0, 1);
        public static Pos LEFT = new Pos(-1, 0);
        public static Pos RIGHT = new Pos(1, 0);
    }
}