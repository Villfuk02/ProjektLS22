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
    public int activePlayer = 0;
    public int dealer = 0;
    public int step = 0;
    public bool onlyHuman = false;
    string status = "";
    int wait = 0;
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
        onlyHuman = humans != 1;
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
                ()=>Step("Míchání...", 500),
                () => Step(Phase.CUT)
                }
            },
            {Phase.CUT, new Action[]{
                CutCardsStep,
                ()=>Step("", 300),
                ()=>Step("", 300),
                ()=>Step("", 300),
                ()=>Step(Phase.DEAL)
                }
            }
        };
    }
    public void NextStep()
    {
        steps[phase][step]();
        Renderer.RenderState(this, status);
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

    void CutCardsStep()
    {
        int cut = Utils.rand.Next(2, 30);
        List<Card> n = deck.GetRange(cut, 32 - cut);
        n.AddRange(deck.GetRange(0, cut));
        deck = n;
        Step($"Hráč {(dealer + 2) % 3 + 1} snímá...", 1000);
    }
}