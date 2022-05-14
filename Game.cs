using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using ProjektLS22;

public class Game
{
    public bool active = false;
    public bool auction = false;
    public enum Phase { INIT, CUT, DEAL, STAKES, GAME, PAY, COLLECT };

    public Phase phase = Phase.INIT;

    public List<Card> deck = new List<Card>();
    public Player[] players = new Player[GameSetup.PLAYER_AMT];
    public List<Card> talon = new List<Card>();
    public List<Card> trick = new List<Card>();
    public int activePlayer = -1;
    public int dealer = 0;
    public int step = 0;
    public enum CardShowing { ALL, ACTIVE_HUMAN, HUMAN };
    public CardShowing cardShowing = CardShowing.ACTIVE_HUMAN;
    public bool waitingForPlayer = false;
    public string status = "";
    int wait = 0;

    public int info = 0;
    public Game(PlayerController.Type[] playerTypes)
    {
        int humans = 0;
        for (int i = 0; i < GameSetup.PLAYER_AMT; i++)
        {
            players[i] = new Player(i, playerTypes[i].GetNew());
            if (playerTypes[i].id == PlayerController.HUMAN.id)
                humans++;
        }
        //cardShowing = humans == 0 ? CardShowing.ALL : (humans == 1 ? CardShowing.HUMAN : CardShowing.ACTIVE_HUMAN);
        foreach (Suit s in Suit.ALL)
        {
            foreach (Value v in Value.ALL)
            {
                deck.Add(new Card(s, v));
            }
        }
        dealer = Utils.rand.Next(GameSetup.PLAYER_AMT);
        deck.Shuffle();
        active = true;
    }

    public void DoStep()
    {
        switch (phase)
        {
            case Phase.INIT:
                InitPhase();
                return;
            case Phase.CUT:
                CutPhase();
                return;
            case Phase.DEAL:
                DealPhase();
                return;

            default:
                Step("INVALID STATE", 1000);
                return;
        }
    }

    public void NextStep()
    {
        DoStep();
        Renderer.RenderState(this);
        if (wait > 0)
            Utils.Wait(wait);
    }

    void Step(string status, int wait, Phase nextPhase, int nextStep)
    {
        phase = nextPhase;
        step = nextStep;
        this.status = status;
        this.wait = wait;
    }
    void Step(string status, int wait, Phase nextPhase)
    {
        Step(status, wait, nextPhase, 0);
    }
    void Step(string status, Phase nextPhase)
    {
        Step(status, 0, nextPhase);
    }
    void Step(string status, int wait)
    {
        Step(status, wait, phase, step + 1);
    }
    void Step(string status)
    {
        Step(status, 0);
    }
    void Step(Phase nextPhase)
    {
        Step(status, nextPhase);
    }
    void Step(int wait)
    {
        Step(status, wait);
    }


    Player GetPlayer(int num)
    {
        return players[(num + GameSetup.PLAYER_AMT) % GameSetup.PLAYER_AMT];
    }
    void InitPhase()
    {
        if (step == 0)
            Step("Míchání...", 500);
        else
            Step(Phase.CUT);
    }

    void CutPhase()
    {
        if (step == 0)
        {
            int cut = Utils.rand.Next(2, 30);
            List<Card> n = deck.GetRange(cut, 32 - cut);
            n.AddRange(deck.GetRange(0, cut));
            deck = n;
            info = cut;
            Step($"Hráč {Utils.TranslatePlayerNum(dealer - 1)} snímá...", 1000);
        }
        else if (step == 1)
        {
            Step(1000);
        }
        else if (step <= 5)
        {
            Step(400);
        }
        else
        {
            Step(Phase.DEAL);
        }
    }

    void DealPhase()
    {
        if (step == 0)
        {
            activePlayer = (dealer + 1) % GameSetup.PLAYER_AMT;
            Step($"Hráč {Utils.TranslatePlayerNum(dealer)} rozdává...", 1000);
        }
        else if (step <= 6)
        {
            DealCards(step == 1 ? 7 : 5, GetPlayer(dealer + step).hand);
            Step(400);
        }
        else if (step == 7)
        {
            for (int i = 0; i < GameSetup.PLAYER_AMT; i++)
            {
                Utils.SortCards(ref players[i].hand, null, i == activePlayer);
            }
            Step(400);
        }
        else
        {
            Step(Phase.STAKES);
        }
    }

    void DealCards(int num, List<Card> destination)
    {
        destination.AddRange(deck.GetRange(0, num));
        deck.RemoveRange(0, num);
    }
}