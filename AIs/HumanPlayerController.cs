using System;

namespace ProjektLS22
{
    class HumanPlayerController : PlayerController
    {
        bool choosingPlacement = false;
        public HumanPlayerController()
        {
            isHuman = true;
        }

        public override void GetOptions(Game g)
        {
            Renderer.PRINT.P("UHHH");
        }
    }
}