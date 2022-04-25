using System;
using System.Collections.Generic;
using ProjektLS22;

public class Game
{
    public bool active = false;
    public bool auction = false;
    public enum Phase { CUT, DEAL, CALL_GAME, AUCTION_1, AUCTION_2, STAKES, GAME, PAY, COLLECT };

    public Phase phase = Phase.CUT;

    public List<Card> deck = new List<Card>();
    public Player[] players = new Player[GameSetup.PLAYER_AMT];
    public int activePlayer = 0;
    public Game(PlayerController.Type[] playerTypes)
    {
        for (int i = 0; i < GameSetup.PLAYER_AMT; i++)
        {
            players[i] = new Player(i, playerTypes[i].GetNew());
        }
        foreach (Suit s in Suit.ALL)
        {
            foreach (Value v in Value.ALL)
            {
                deck.Add(new Card(s, v));
            }
        }
        deck.Shuffle();
        active = true;
    }

    public void NextStep()
    {
        Console.ReadKey(true);
        deck.Shuffle();
    }
}