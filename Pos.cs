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
        public static Pos[] DIRECTIONS = new Pos[] { UP, RIGHT, DOWN, LEFT };
        public static Pos[] NEIGHBORS = new Pos[] { UP, UP + RIGHT, RIGHT, RIGHT + DOWN, DOWN, DOWN + LEFT, LEFT, LEFT + UP };

        public static Pos operator +(Pos a, Pos b)
        {
            return new Pos(a.x + b.x, a.y + b.y);
        }
        public static Pos operator -(Pos a, Pos b)
        {
            return new Pos(a.x - b.x, a.y - b.y);
        }
        public static Pos operator *(int a, Pos p)
        {
            return new Pos(a * p.x, a * p.y);
        }

        public void LimitToWorld(int size)
        {
            x = (x + size - 1) % size + 1;
            y = (y + size - 1) % size + 1;
        }
    }
}