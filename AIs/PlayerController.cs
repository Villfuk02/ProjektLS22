namespace ProjektLS22
{
    public class PlayerController
    {
        public enum Type { Human };
        public static PlayerController GetNew(Type type)
        {
            switch (type)
            {
                case Type.Human: return new HumanPlayerController();
                default: return new HumanPlayerController();
            }
        }
    }
}