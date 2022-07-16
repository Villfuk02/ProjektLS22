using System;
using System.Collections;
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
    public Card(int num)
    {
        suit = Suit.ALL[num >> 3];
        value = Value.ALL[num & 0x07];
    }

    public int GetNum()
    {
        return (suit.index << 3) | value.index;
    }
    public static uint GetMask(IEnumerable<Card> cards)
    {
        uint m = 0;
        foreach (Card c in cards)
        {
            m |= ((uint)1) << c.GetNum();
        }
        return m;
    }
    public static List<Card> GetCards(uint mask)
    {
        List<Card> l = new List<Card>();
        for (int i = 0; i < 32; i++)
        {
            if ((mask & (1 << i)) != 0)
                l.Add(new Card(i));
        }
        return l;
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
    public readonly int index;
    Suit(ConsoleColor color, char prefix, int index)
    {
        this.color = color;
        this.prefix = prefix;
        this.index = index;
    }
    public bool HasCard(Card c)
    {
        return c.suit == this;
    }
    public static readonly Suit ŽALUDY = new Suit(ConsoleColor.Yellow, 'ž', 0);
    public static readonly Suit ZELENÝ = new Suit(ConsoleColor.Green, 'z', 1);
    public static readonly Suit ČERVENÝ = new Suit(ConsoleColor.Red, 'č', 2);
    public static readonly Suit KULE = new Suit(ConsoleColor.DarkYellow, 'k', 3);
    public static readonly Suit[] ALL = new Suit[] { ŽALUDY, ZELENÝ, ČERVENÝ, KULE };
}

public class Value
{
    public readonly char symbol;
    public readonly int strength;
    public readonly bool ten;
    public readonly bool marriage;
    public readonly int index;
    Value(char symbol, int strength, bool ten, bool marriage, int index)
    {
        this.symbol = symbol;
        this.strength = strength;
        this.ten = ten;
        this.marriage = marriage;
        this.index = index;
    }
    public static readonly Value SEDM = new Value('7', 7, false, false, 0);
    public static readonly Value OSM = new Value('8', 8, false, false, 1);
    public static readonly Value DEVĚT = new Value('9', 9, false, false, 2);
    public static readonly Value DESET = new Value('X', 20, true, false, 3);
    public static readonly Value SPODEK = new Value('J', 11, false, false, 4);
    public static readonly Value SVRŠEK = new Value('Q', 12, false, true, 5);
    public static readonly Value KRÁL = new Value('K', 13, false, true, 6);
    public static readonly Value ESO = new Value('A', 30, true, false, 7);
    public static readonly Value[] ALL = new Value[] { SEDM, OSM, DEVĚT, DESET, SPODEK, SVRŠEK, KRÁL, ESO };
}