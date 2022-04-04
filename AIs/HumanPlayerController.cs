using System;

namespace ProjektLS22
{
    class HumanPlayerController : PlayerController
    {
        bool choosingPlacement = false;
        Pos cursorPosition = new Pos();
        public HumanPlayerController()
        {

        }

        public override Player.Action GetIntent(World w, Player p)
        {
            choosingPlacement = true;
            cursorPosition = p.basePos;
            Renderer.PRINT.P(" ");
            Renderer.PaintCursor(w, cursorPosition);
            while (choosingPlacement)
            {
                ConsoleKeyInfo k = Console.ReadKey(true);
                Renderer.UnpaintCursor(w, cursorPosition);
                switch (k.Key)
                {
                    case ConsoleKey.Spacebar:
                        choosingPlacement = false;
                        break;
                    case ConsoleKey.UpArrow:
                        cursorPosition += Pos.UP;
                        break;
                    case ConsoleKey.RightArrow:
                        cursorPosition += Pos.RIGHT;
                        break;
                    case ConsoleKey.DownArrow:
                        cursorPosition += Pos.DOWN;
                        break;
                    case ConsoleKey.LeftArrow:
                        cursorPosition += Pos.LEFT;
                        break;
                }
                cursorPosition.LimitToWorld(w.size);
                //Renderer.RenderState(w, cursorPosition);
                Renderer.PRINT.CL(1).P($"{cursorPosition.x} {cursorPosition.y}");
                Renderer.PaintCursor(w, cursorPosition);
            }
            Console.ReadLine();
            Renderer.PRINT.CL(2);
            return new Player.Action(' ', new Pos());
        }
    }
}