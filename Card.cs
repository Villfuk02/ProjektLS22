using System;
using System.Collections.Generic;
using System.Numerics;
using static ProjektLS22.Utils;

public readonly struct Card
{
    public static IEnumerable<Card> ALL { get { for (int i = 0; i < 32; i++) yield return new Card(i); } }
    readonly int num;
    public Suit Suit { get => new Suit(num >> 3); }
    public Value Value { get => new Value(num & 0x7); }
    uint Mask { get => (uint)1 << num; }
    public Pile SameSuitButLower { get => ((Pile)Suit) & (Mask - 1); }
    public Pile SameSuitButHigher { get => ((Pile)Suit) & ~((Mask << 1) - 1); }
    public Card(Suit suit, Value value)
    {
        num = (suit << 3) | value;
    }
    public Card(int num)
    {
        this.num = num;
    }

    public Card GetPartner()
    {
        return new Card(num ^ 0x1);
    }

    public bool SameSuit(Card c)
    {
        return Suit == c.Suit;
    }
    public override bool Equals(object obj)
    {
        if (obj is Card)
            return ((Card)obj).num == num;
        return false;
    }
    public override int GetHashCode()
    {
        return num;
    }
    public static bool operator ==(Card c1, Card c2)
    {
        return c1.num == c2.num;
    }
    public static bool operator !=(Card c1, Card c2)
    {
        return c1.num != c2.num;
    }
    public override string ToString()
    {
        return _FormatCards(new List<Card> { this }, null);
    }
    public static implicit operator Pile(Card c) => c.Mask;
}

public readonly struct Suit
{
    public static IEnumerable<Suit> ALL { get { for (int i = 0; i < 4; i++) yield return new Suit(i); } }
    static readonly ConsoleColor[] colors = new ConsoleColor[] { ConsoleColor.Yellow, ConsoleColor.Green, ConsoleColor.Red, ConsoleColor.DarkYellow };
    static readonly char[] prefixes = new char[] { 'ž', 'z', 'č', 'k' };
    readonly int num;
    public ConsoleColor Color { get => colors[num]; }
    public char Prefix { get => prefixes[num]; }
    uint Mask { get => (uint)0xFF << 8 * num; }
    public Suit(int num)
    {
        this.num = num;
    }
    public override string ToString()
    {
        return Prefix.ToString();
    }
    public static implicit operator int(Suit s) => s.num;
    public static implicit operator Pile(Suit s) => s.Mask;
}

public readonly struct Value
{
    public static IEnumerable<Value> ALL { get { for (int i = 0; i < 8; i++) yield return new Value(i); } }
    static readonly char[] symbols = new char[] { '7', '8', '9', 'J', 'Q', 'K', 'X', 'A' };
    static readonly int[] strengths = new int[] { 7, 8, 9, 11, 12, 13, 20, 30 };
    readonly int num;
    public bool GivesPoints { get => this == TEN || this == ACE; }
    public bool Marriage { get => this == QUEEN || this == KING; }
    public char Symbol { get => symbols[num]; }
    public int Strength { get => strengths[num]; }
    uint Mask { get => (uint)0x01010101 << num; }
    public static readonly Pile GivesPointsMask = TEN.Mask | ACE.Mask;
    public static readonly Pile MarriageMask = QUEEN.Mask | KING.Mask;
    public static readonly Value QUEEN = new Value(4);
    public static readonly Value KING = new Value(5);
    public static readonly Value TEN = new Value(6);
    public static readonly Value ACE = new Value(7);
    public Value(int num)
    {
        this.num = num;
    }
    public override string ToString()
    {
        return Symbol.ToString();
    }
    public static implicit operator int(Value v) => v.num;
    public static implicit operator Pile(Value v) => v.Mask;
}

public readonly struct Pile
{
    public static Pile FULL = new Pile(0xFFFFFFFF);
    readonly uint mask;
    public int Count { get => BitOperations.PopCount(mask); }
    public bool IsEmpty { get => mask == 0; }

    public Pile(uint mask)
    {
        this.mask = mask;
    }
    public Pile(IEnumerable<Card> cards)
    {
        Pile p = new Pile();
        foreach (Card c in cards)
        {
            p = p.With(c);
        }
        this = p;
    }

    public IEnumerable<Card> Enumerate()
    {
        uint m = mask;
        for (int i = 0; i < 32; i++)
        {
            if ((m & 0x1) != 0)
                yield return new Card(i);
            m >>= 1;
        }
    }

    public Pile With(Pile p)
    {
        return mask | p;
    }

    public Pile With(IEnumerable<Card> l)
    {
        return With(new Pile(l));
    }

    public Pile Without(Pile p)
    {
        return mask & ~p;
    }

    public Pile Where(Pile p)
    {
        return mask & p;
    }

    public bool HasAny(Pile p)
    {
        return !Where(p).IsEmpty;
    }

    public override string ToString()
    {
        return _FormatCards(this.Enumerate(), null);
    }

    public static implicit operator Pile(uint mask) => new Pile(mask);
    public static implicit operator uint(Pile p) => p.mask;
}