using System;
using System.Collections.Generic;
using System.Data;
using ProjektLS22;

public class Game
{
    public bool active = false;
    public bool auction = false;
    public enum Phase { INIT, CUT, DEAL, CALL_GAME, AUCTION, STAKES, GAME, PAY, COLLECT };

    public Phase phase = Phase.INIT;

    public List<Card> deck = new List<Card>();
    public Player[] players = new Player[GameSetup.PLAYER_AMT];
    public List<Card> talon = new List<Card>();
    public List<Card> trick = new List<Card>();
    public int activePlayer = 0;
    public int dealer = 0;
    public int step = 0;
    public enum CardShowing { ALL, ACTIVE_HUMAN, HUMAN };
    public CardShowing cardShowing = CardShowing.ALL;
    public bool isActive = false;
    public string status = "";
    int wait = 0;

    public int info = 0;
    private Dictionary<Phase, Action[]> steps;
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

        steps = new Dictionary<Phase, Action[]>{
            {Phase.INIT, new Action[]{
                () => Step("Míchání...", 500),
                () => Step(Phase.CUT)
            }},
            {Phase.CUT, new Action[]{
                CutCardsStep,
                () => Step(1000),
                () => Step(400),
                () => Step(400),
                () => Step(400),
                () => Step(400),
                () => Step(Phase.DEAL)
            }},
            {Phase.DEAL, new Action[]{

            }}
        };
    }
    public void NextStep()
    {
        steps[phase][step]();
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

    void CutCardsStep()
    {
        int cut = Utils.rand.Next(2, 30);
        List<Card> n = deck.GetRange(cut, 32 - cut);
        n.AddRange(deck.GetRange(0, cut));
        deck = n;
        info = cut;
        Step($"Hráč {(dealer + 2) % 3 + 1} snímá...", 1000);
    }
}