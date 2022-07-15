using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Channels;

public class Card
{
    public readonly Suit suit;
    public readonly Value value;
    public readonly static List<Card> ALL = new List<Card>(Suit.ALL.SelectMany((s, i) => Value.ALL.Select((v, j) => new Card(s, v))));
    public Card(Suit suit, Value value)
    {
        this.suit = suit;
        this.value = value;
    }

    public bool SameSuitButLowerThan(Card compared)
    {
        return SameSuit(compared) && LowerThan(compared);
    }
    public bool SameSuit(Card compared)
    {
        return suit.HasCard(compared);
    }

    public bool LowerThan(Card compared)
    {
        return value.strength < compared.value.strength;
    }
    public Card GetPartner()
    {
        if (value.marriage)
        {
            return new Card(suit, value == Value.KRÁL ? Value.SVRŠEK : Value.KRÁL);
        }
        return null;
    }

    public override bool Equals(Object obj)
    {
        if (obj is Card)
        {
            return (SameSuit((Card)obj) && value == ((Card)obj).value);
        }
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return suit.GetHashCode() ^ value.GetHashCode();
    }
}

public class Suit
{
    public readonly ConsoleColor color;
    public readonly char prefix;
    Suit(ConsoleColor color, char prefix)
    {
        this.color = color;
        this.prefix = prefix;
    }
    public bool HasCard(Card c)
    {
        return c.suit == this;
    }
    public static readonly Suit ŽALUDY = new Suit(ConsoleColor.Yellow, 'ž');
    public static readonly Suit ZELENÝ = new Suit(ConsoleColor.Green, 'z');
    public static readonly Suit ČERVENÝ = new Suit(ConsoleColor.Red, 'č');
    public static readonly Suit KULE = new Suit(ConsoleColor.DarkYellow, 'k');
    public static readonly Suit[] ALL = new Suit[] { ŽALUDY, ZELENÝ, ČERVENÝ, KULE };
}

public class Value
{
    public readonly char symbol;
    public readonly int strength;
    public readonly bool ten;
    public readonly bool marriage;
    Value(char symbol, int strength, bool ten, bool marriage)
    {
        this.symbol = symbol;
        this.strength = strength;
        this.ten = ten;
        this.marriage = marriage;
    }
    public static readonly Value SEDM = new Value('7', 7, false, false);
    public static readonly Value OSM = new Value('8', 8, false, false);
    public static readonly Value DEVĚT = new Value('9', 9, false, false);
    public static readonly Value DESET = new Value('X', 20, true, false);
    public static readonly Value SPODEK = new Value('J', 11, false, false);
    public static readonly Value SVRŠEK = new Value('Q', 12, false, true);
    public static readonly Value KRÁL = new Value('K', 13, false, true);
    public static readonly Value ESO = new Value('A', 30, true, false);
    public static readonly Value[] ALL = new Value[] { SEDM, OSM, DEVĚT, DESET, SPODEK, SVRŠEK, KRÁL, ESO };
}