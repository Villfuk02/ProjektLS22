using System;
public class Card
{
    public readonly Suit suit;
    public readonly Value value;
    public Card(Suit suit, Value value)
    {
        this.suit = suit;
        this.value = value;
    }

    public override bool Equals(Object obj)
    {
        if (obj is Card)
        {
            return (this.suit == ((Card)obj).suit && this.value == ((Card)obj).value);
        }
        return base.Equals(obj);
    }
}

public class Suit
{
    public readonly ConsoleColor color;
    public readonly string name;
    Suit(ConsoleColor color, string name)
    {
        this.color = color;
        this.name = name;
    }
    public static readonly Suit ŽALUDY = new Suit(ConsoleColor.Yellow, "žaludy");
    public static readonly Suit ZELENÝ = new Suit(ConsoleColor.Green, "zelený");
    public static readonly Suit ČERVENÝ = new Suit(ConsoleColor.Red, "červený");
    public static readonly Suit KULE = new Suit(ConsoleColor.DarkYellow, "kule");
    public static readonly Suit[] ALL = new Suit[] { ŽALUDY, ZELENÝ, ČERVENÝ, KULE };
}

public class Value
{
    public readonly char symbol;
    public readonly int gameStrength;
    public readonly int standardStrength;
    public readonly bool ten;
    public readonly bool marriage;
    Value(char symbol, int gameStrength, int standardStrength, bool ten, bool marriage)
    {
        this.symbol = symbol;
        this.gameStrength = gameStrength;
        this.standardStrength = standardStrength;
        this.ten = ten;
    }
    public static readonly Value SEDM = new Value('7', 7, 7, false, false);
    public static readonly Value OSM = new Value('8', 8, 8, false, false);
    public static readonly Value DEVĚT = new Value('9', 9, 9, false, false);
    public static readonly Value DESET = new Value('X', 15, 10, true, false);
    public static readonly Value SPODEK = new Value('J', 11, 11, false, false);
    public static readonly Value SVRŠEK = new Value('Q', 12, 12, false, true);
    public static readonly Value KRÁL = new Value('K', 13, 13, false, true);
    public static readonly Value ESO = new Value('A', 20, 20, true, false);
    public static readonly Value[] ALL = new Value[] { SEDM, OSM, DEVĚT, DESET, SPODEK, SVRŠEK, KRÁL, ESO };
}