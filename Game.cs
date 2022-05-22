using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using ProjektLS22;

public class Game
{
    public int simulate;
    public enum Phase { INIT, CUT, DEAL, STAKES, GAME, SCORE, COLLECT };

    public Phase phase = Phase.INIT;

    public List<Card> deck = new List<Card>();
    public Player[] players = new Player[3];
    public List<Card> talon = new List<Card>();
    public List<Card> trick = new List<Card>();
    public Card trumps;
    public bool zLidu;
    public int activePlayer = -1;
    public int dealer = 0;
    public int step = 0;
    public enum CardShowing { ALL, ACTIVE_HUMAN, HUMAN };
    public CardShowing cardShowing = CardShowing.ACTIVE_HUMAN;
    public bool waitingForPlayer = false;
    public string status = "";
    int wait = 0;

    public int info = 0;
    public Game(PlayerController.Type[] playerTypes, int simulate)
    {
        this.simulate = simulate;
        int humans = 0;
        for (int i = 0; i < 3; i++)
        {
            players[i] = new Player(i, playerTypes[i].GetNew());
            players[i].controller.player = players[i];
            if (playerTypes[i].id == PlayerController.HUMAN.id)
                humans++;
        }
        cardShowing = humans == 0 ? CardShowing.ALL : (humans == 1 ? CardShowing.HUMAN : CardShowing.ACTIVE_HUMAN);
        foreach (Suit s in Suit.ALL)
        {
            foreach (Value v in Value.ALL)
            {
                deck.Add(new Card(s, v));
            }
        }
        dealer = Utils.rand.Next(3);
        deck.Shuffle();
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
            case Phase.STAKES:
                StakesPhase();
                return;
            case Phase.GAME:
                GamePhase();
                return;
            case Phase.SCORE:
                ScorePhase();
                return;
            case Phase.COLLECT:
                CollectPhase();
                break;
            default:
                Step("INVALID STATE", 1000);
                return;
        }
    }

    public void NextStep()
    {
        int lastStep = step;
        Phase lastPhase = phase;
        DoStep();
        if (simulate > 0)
        {
            if (phase == Phase.COLLECT && step == 0)
            {
                simulate--;
                Renderer.PRINT.P($"Zbývá {simulate} her |", 17, false);
                for (int i = 0; i < 3; i++)
                {
                    Renderer.PRINT.P($" {Utils.TranslatePlayer(i)}: ", 9, true).P($"{players[i].score} |", 7, false);
                }
                Renderer.PRINT.NL();
            }
        }
        else if (simulate == 0)
        {
            return;
        }
        if (simulate == -1 || (waitingForPlayer && players[activePlayer].controller.isHuman))
        {
            if (lastPhase != phase || lastStep != step)
            {
                Renderer.RenderState(this);
            }
            if (wait > 0)
                Utils.Wait(wait);
        }
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

    void Step()
    {
        Step(0);
    }

    void Step(Phase nextPhase, int nextStep)
    {
        Step(status, 0, nextPhase, nextStep);
    }


    Player GetPlayer(int num)
    {
        return players[(num + 3) % 3];
    }
    void InitPhase()
    {
        if (step == 0)
            Step("Míchání...", 400);
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
            Step($"{Utils.TranslatePlayer(dealer - 1)} snímá...", 1000);
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
            activePlayer = (dealer + 1) % 3;
            Step($"{Utils.TranslatePlayer(dealer)} rozdává...", 1000);
        }
        else if (step <= 6)
        {
            DealCards(step == 1 ? 7 : 5, GetPlayer(dealer + step).hand);
            Step(400);
        }
        else if (step == 7)
        {
            for (int i = 0; i < 3; i++)
            {
                Utils.SortCards(ref players[i].hand, null, i == activePlayer);
            }
            Step(200);
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

    void StakesPhase()
    {
        switch (step)
        {
            case 0:
                {
                    waitingForPlayer = true;
                    Step($"{Utils.TranslatePlayer(activePlayer)} vybírá trumfy...", 200);
                    break;
                }
            case 1:
                {
                    int choice = players[activePlayer].controller.ChooseTrumps(this);
                    if (choice >= -1 && choice < 7)
                    {
                        if (choice == -1)
                        {
                            trumps = players[activePlayer].hand[Utils.rand.Next(7, 12)];
                            zLidu = true;
                        }
                        else
                        {
                            trumps = players[activePlayer].hand[choice];
                        }
                        Step($"{Utils.TranslatePlayer(activePlayer)} vybral trumfy", 200);
                    }
                    break;
                }
            case 2:
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Utils.SortCards(ref players[i].hand, trumps.suit, false);
                    }
                    Step($"{Utils.TranslatePlayer(activePlayer)} odhazuje do talonu...", 200);
                    break;
                }
            case 3:
                {
                    int choice = players[activePlayer].controller.ChooseTalon(this);
                    if (choice >= 0 && choice < players[activePlayer].hand.Count
                        && Utils.ValidTalon(players[activePlayer].hand, choice, trumps, trick))
                    {
                        Card c = players[activePlayer].hand[choice];
                        players[activePlayer].hand.RemoveAt(choice);
                        talon.Add(c);
                        if (talon.Count == 2)
                            Step($"{Utils.TranslatePlayer(activePlayer)} odhodil do talonu");
                        else
                            Step(Phase.STAKES, 2);
                    }
                    break;
                }
            case 4:
                {
                    Step(Phase.GAME);
                    break;
                }
        }
    }

    void GamePhase()
    {
        switch (step)
        {
            case 0:
                {
                    waitingForPlayer = true;
                    Step($"{Utils.TranslatePlayer(activePlayer)} vynáší...", 200);
                    break;
                }
            case 1:
                {
                    int choice = players[activePlayer].controller.ChooseTalon(this);
                    if (choice >= 0 && choice < players[activePlayer].hand.Count
                        && Utils.ValidPlay(players[activePlayer].hand, choice, trumps, trick))
                    {
                        Card c = players[activePlayer].hand[choice];
                        players[activePlayer].hand.RemoveAt(choice);
                        if (c.value.marriage && players[activePlayer].hand.Exists((Card d) => d.suit == c.suit && d.value.marriage))
                        {
                            players[activePlayer].marriages.Add(c);
                        }
                        trick.Add(c);
                        NextPlayer();
                        if (trick.Count != 3)
                        {
                            Step();
                        }
                        else
                        {
                            waitingForPlayer = false;
                            Step(Phase.GAME, 3);
                        }
                    }
                    break;
                }
            case 2:
                {
                    Step($"Na řadě je {Utils.TranslatePlayer(activePlayer)}...", 200, Phase.GAME, 1);
                    break;
                }
            case 3:
                {
                    int max = 0;
                    int winner = -1;
                    for (int i = 0; i < trick.Count; i++)
                    {
                        Card c = trick[i];
                        int value = (c.suit == trumps.suit ? 200 : (c.suit == trick[0].suit ? 100 : 0)) + c.value.gameStrength;
                        if (value > max)
                        {
                            max = value;
                            winner = (activePlayer + i) % 3;
                        }
                    }
                    info = winner;
                    Step($"{Utils.TranslatePlayer(winner)} bere štych...", 4000);
                    break;
                }
            case 4:
                {
                    players[info].discard.AddRange(trick);
                    trick.Clear();
                    activePlayer = info;
                    if (players[info].hand.Count == 0)
                    {
                        Step(Phase.SCORE);
                    }
                    else
                    {
                        Step(Phase.GAME);
                    }
                    break;
                }
        }
    }

    void NextPlayer()
    {
        activePlayer = (activePlayer + 1) % 3;
    }

    void ScorePhase()
    {
        if (step > 0)
        {
            dealer = (dealer + 1) % 3;
            activePlayer = dealer;
            Step($"{Utils.TranslatePlayer(dealer)} sbírá karty...", Phase.COLLECT);
            return;
        }
        int offense = CountPoints((dealer + 1) % 3);
        int defense = CountPoints(dealer) + CountPoints((dealer + 2) % 3);

        if (offense > defense)
        {
            players[(dealer + 1) % 3].score++;
            Step($"Aktér vyhrál {offense}:{defense}!", 4000);
        }
        else
        {
            players[dealer].score++;
            players[(dealer + 2) % 3].score++;
            Step($"Obrana vyhrála {defense}:{offense}!", 4000);
        }
    }

    int CountPoints(int player)
    {
        int pts = players[player].discard.Sum((Card c) => c.value.ten ? 10 : 0);
        if (player == info)
        {
            pts += 10;
        }
        pts += players[player].marriages.Count * 20;
        return pts;
    }

    void CollectPhase()
    {
        if (step == 0)
        {
            trumps = null;
            zLidu = false;
            List<List<Card>> collect = new List<List<Card>>();
            for (int i = 0; i < 3; i++)
            {
                collect.Add(players[i].discard);
                players[i].marriages.Clear();
            }
            collect.Add(talon);
            collect.Shuffle();
            foreach (List<Card> list in collect)
            {
                deck.AddRange(list);
                list.Clear();
            }
            Step(1000);
        }
        else
        {
            Step(Phase.CUT);
        }
    }
}