namespace ProjektLS22
{
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
        public static Pos[] DIRECTIONS = new Pos[] { UP, LEFT, DOWN, RIGHT };
    }
}