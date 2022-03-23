using System;

namespace ProjektLS22
{
    class HumanPlayerController : PlayerController
    {
        public HumanPlayerController()
        {

        }

        public override Player.Action GetIntent(World w)
        {
            Console.ReadLine();
            Renderer.PRINT.CL(2);
            return new Player.Action(' ', new Pos());
        }
    }
}