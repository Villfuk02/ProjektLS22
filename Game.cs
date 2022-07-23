using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ProjektLS22;
using static ProjektLS22.Printer;
using static ProjektLS22.Utils;

public class Game
{
    public int simulate;
    Stopwatch sw = new Stopwatch();
    TimeSpan writeStep = TimeSpan.FromSeconds(1);
    TimeSpan nextWrite = TimeSpan.Zero;
    TimeSpan lastWrite = TimeSpan.Zero;
    public enum Phase { INIT, CUT, DEAL, BEGIN, GAME, SCORE, COLLECT };

    public Phase phase = Phase.INIT;

    public List<Card> deck = new List<Card>();
    public Player[] players = new Player[3];
    public List<Card> talon = new List<Card>();
    public List<Card> trick = new List<Card>();
    public Card? trumps;
    public bool fromPeople;
    public int activePlayer = -1;
    public int dealer = 0;
    public int step = 0;
    public enum CardShowing { ALL, HUMAN, ACTIVE_HUMAN };
    public CardShowing cardShowing = CardShowing.ACTIVE_HUMAN;
    public bool waitingForPlayer = false;
    public string status = "";
    int wait = 0;

    bool fast = false;

    public int info = 0;
    public Game(PlayerController.Type[] playerTypes, int simulate)
    {
        sw.Start();
        nextWrite += writeStep;
        this.simulate = simulate;
        int humans = 0;
        for (int i = 0; i < 3; i++)
        {
            players[i] = new Player(i, playerTypes[i]);
            if (playerTypes[i].id == PlayerController.HUMAN.id)
                humans++;
        }
        cardShowing = humans == 0 ? CardShowing.ALL : (humans == 1 ? CardShowing.HUMAN : CardShowing.ACTIVE_HUMAN);
        deck = new List<Card>(Card.ALL);
        dealer = _rand.Next(3);
        deck._Shuffle();
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
            case Phase.BEGIN:
                BeginPhase();
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
                if (simulate == 0 || sw.Elapsed >= nextWrite)
                {
                    if (sw.Elapsed - lastWrite > 2 * writeStep)
                    {
                        nextWrite = sw.Elapsed + writeStep;
                        _printer.F(ConsoleColor.Yellow);
                    }
                    else
                    {
                        nextWrite += writeStep;
                        _printer.W();
                    }
                    _printer.P($" {sw.Elapsed.ToString(@"hh\:mm\:ss")} | ").P($"Zbývá {simulate} | ", 14, false);
                    for (int i = 0; i < 3; i++)
                    {
                        _printer.P(_playerNames[i], 1, true).P(':').P($" a {players[i].offense_wins},", 9, false).P($" o {players[i].defense_wins},", 9, false).P($" C {players[i].offense_wins + players[i].defense_wins} | ", 11, false);
                    }
                    _printer.NL().R();
                    lastWrite = sw.Elapsed;
                }
                if (simulate % 100 == 0)
                {
                    deck._Shuffle();
                }
            }
        }
        else if (simulate == 0)
        {
            sw.Stop();
            return;
        }
        if (simulate == -1 || (waitingForPlayer && players[activePlayer].controller.IsHuman))
        {
            if (lastPhase != phase || lastStep != step)
            {
                Renderer.RenderState(this);
            }
            if (wait > 0 && !fast)
                _Wait(wait);
        }
    }

    void Step(string status, int wait, Phase nextPhase, int nextStep)
    {
        phase = nextPhase;
        step = nextStep;
        this.status = status;
        this.wait = wait;
    }
    void Step(string status, Phase nextPhase)
    {
        Step(status, 0, nextPhase, 0);
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
            foreach (Player p in players)
            {
                p.controller.NewRound(dealer);
            }
            int cut = _rand.Next(2, 30);
            List<Card> n = deck.GetRange(cut, 32 - cut);
            n.AddRange(deck.GetRange(0, cut));
            deck = n;
            info = cut;
            Step($"{_playerNames[_PPlus(dealer, 2)]} snímá...", 1000);
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
            activePlayer = _PPlus(dealer, 1);
            Step($"{_playerNames[dealer]} rozdává...", 1000);
        }
        else if (step <= 6)
        {
            Player p = players[_PPlus(dealer, step)];
            DealCards(step == 1 ? 7 : 5, step == 4 ? p.discard : p.hand);
            Step(400);
        }
        else if (step == 7)
        {
            for (int i = 0; i < 3; i++)
            {
                _SortCards(ref players[i].hand, null);
            }
            Step(200);
        }
        else
        {
            Step(Phase.BEGIN);
        }
    }

    void DealCards(int num, List<Card> destination)
    {
        destination.AddRange(deck.GetRange(0, num));
        deck.RemoveRange(0, num);
    }

    void BeginPhase()
    {
        switch (step)
        {
            case 0:
                {
                    waitingForPlayer = true;
                    Step($"{_playerNames[activePlayer]} vybírá trumfy...", 200);
                    break;
                }
            case 1:
                {
                    Card? choice = players[activePlayer].controller.ChooseTrumps();
                    if (!choice.HasValue || players[activePlayer].hand.Contains(choice.Value))
                    {
                        players[activePlayer].hand.AddRange(players[activePlayer].discard);
                        players[activePlayer].discard.Clear();
                        if (choice.HasValue)
                        {
                            trumps = choice.Value;
                        }
                        else
                        {
                            trumps = players[activePlayer].hand[_rand.Next(7, 12)];
                            fromPeople = true;
                        }
                        for (int i = 0; i < 3; i++)
                        {
                            _SortCards(ref players[i].hand, trumps.Value.Suit);
                        }
                        Step($"{_playerNames[activePlayer]} vybral trumfy", 200);
                    }
                    break;
                }
            case 2:
                {
                    Step($"{_playerNames[activePlayer]} odhazuje do talonu...", 200);
                    break;
                }
            case 3:
                {
                    Card choice = players[activePlayer].controller.ChooseTalon(trumps.Value, new Pile(talon));
                    if (players[activePlayer].hand.Contains(choice) && _ValidTalon(choice, trumps.Value))
                    {
                        players[activePlayer].hand.Remove(choice);
                        talon.Add(choice);
                        if (talon.Count == 2)
                            Step($"{_playerNames[activePlayer]} odhodil do talonu");
                        else
                            Step(Phase.BEGIN, 2);
                    }
                    break;
                }
            case 4:
                {
                    foreach (Player p in players)
                    {
                        p.controller.FirstTrickStart(trumps.Value, fromPeople, activePlayer, p.index == activePlayer ? new Pile(talon) : null);
                    }
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
                    Step($"{_playerNames[activePlayer]} vynáší...", 200);
                    break;
                }
            case 1:
                {
                    Card choice = players[activePlayer].controller.ChoosePlay(trick, trumps.Value);
                    if (players[activePlayer].hand.Contains(choice) && _ValidPlay(new Pile(players[activePlayer].hand), choice, trumps.Value.Suit, trick.ToArray(), trick.Count))
                    {
                        players[activePlayer].hand.Remove(choice);
                        bool marriage = false;
                        if (choice.Value.Marriage && players[activePlayer].hand.Exists(d => choice.SameSuit(d) && d.Value.Marriage))
                        {
                            marriage = true;
                            players[activePlayer].marriages.Add(choice);
                        }
                        trick.Add(choice);
                        foreach (Player p in players)
                        {
                            p.controller.PlaysCard(activePlayer, choice, trick, trumps.Value, marriage);
                        }
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
                    Step($"Na řadě je {_playerNames[activePlayer]}...", 200, Phase.GAME, 1);
                    break;
                }
            case 3:
                {
                    int winner = _PPlus(activePlayer, _TrickWinner(trick.ToArray(), trumps.Value.Suit));
                    foreach (Player p in players)
                    {
                        p.controller.TakesTrick(winner, trick);
                    }
                    info = winner;
                    Step($"{_playerNames[winner]} bere štych...", 4000);
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
        activePlayer = _PPlus(activePlayer, 1);
    }

    void ScorePhase()
    {
        if (step > 0)
        {
            dealer = _PPlus(dealer, 1);
            activePlayer = dealer;
            Step($"{_playerNames[dealer]} sbírá karty...", Phase.COLLECT);
            return;
        }
        int offense = CountPoints(_PPlus(dealer, 1));
        int defense = CountPoints(dealer) + CountPoints(_PPlus(dealer, 2));

        if (offense > defense)
        {
            players[_PPlus(dealer, 1)].offense_wins++;
            Step($"Aktér vyhrál {offense}:{defense}!", 4000);
        }
        else
        {
            players[dealer].defense_wins++;
            players[_PPlus(dealer, 2)].defense_wins++;
            Step($"Opozice vyhrála {defense}:{offense}!", 4000);
        }
    }

    int CountPoints(int player)
    {
        int pts = players[player].discard.Count(c => c.Value.GivesPoints) * 10;
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
            fromPeople = false;
            List<List<Card>> collect = new List<List<Card>>();
            for (int i = 0; i < 3; i++)
            {
                collect.Add(players[i].discard);
                players[i].marriages.Clear();
            }
            collect.Add(talon);
            collect._Shuffle();
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