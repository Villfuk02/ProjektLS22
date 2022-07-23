using System;
using System.Collections.Generic;
using static ProjektLS22.Printer;
using static ProjektLS22.Utils;

namespace ProjektLS22
{
    /// <summary>
    /// The game settings menu.
    /// </summary>
    public class GameSetup
    {
        PlayerController.Type[] players = new PlayerController.Type[3] { PlayerController.HUMAN, PlayerController.RANDOM, PlayerController.SMART };
        enum State { Menu, Rules, Finished };
        State state = State.Menu;
        InputHandler menuHandler = new InputHandler();
        InputHandler rulesHandler = new InputHandler();
        /// <summary>
        /// -1 means normal mode, positive integers are the number of games to simulate
        /// </summary>
        public static readonly int[] MODE_OPTIONS = new int[] { -1, 1000, 10_000, 100_000 };
        public int mode_selected = 0;

        public GameSetup()
        {
            menuHandler.RegisterOption('S', () => { state = State.Finished; });
            menuHandler.RegisterParametricOption("JFK", ChangeAI);
            menuHandler.RegisterOption('P', () => { state = State.Rules; });
            menuHandler.RegisterOption('M', ChangeMode);

            rulesHandler.RegisterOption('S', () => { state = State.Finished; });
            rulesHandler.RegisterOption('Z', () => { state = State.Menu; });

            while (state != State.Finished)
            {
                PrintInfo();
                if (state == State.Menu)
                    menuHandler.ProcessInput();
                else if (state == State.Rules)
                    rulesHandler.ProcessInput();
            }
        }

        void PrintInfo()
        {
            if (state == State.Rules)
            {
                _printer.CLR().R().PF("*  - |Pravidla| -  *").NL().NL();
                _printer.PF("Tato hra je založená na *Mariáši*. Je ale velmi zjednodušená.").NL();
                _printer.PF("Hra je pro tři hráče. V každém kole (sehrávce) je jeden hráč určen jako *aktér*, který je označen ").F(ConsoleColor.Red).P("červeně").R().P(".").NL();
                _printer.PF("Zbylí dva hráči - *opozice* - hrajou spolu proti aktérovi. Tým, kterým má na konci sehrávky více bodů vyhrává.").NL();
                _printer.NL().PF("  *|Průběh kola|*").NL().NL();
                _printer.PF("Na začátku kola je aktérovi rozdáno 7 + 5 karet a každému hráči opozice 5 + 5.").NL().NL();
                _printer.PF("Aktér se smí dívat jen na prvních sedm svých karet a z nich vybere jednu, kterou ukáže ostatním.").NL();
                _printer.PF("Barva této karty určuje barvu *trumfů* po zbytek sehrávky.").NL();
                _printer.PF("Pokud si aktér nechce vybrat z prvních sedmi karet, může vybrat *z lidu* - vybere kartu náhodně ze zbylých pěti.").NL().NL();
                _printer.PF("Potom si vezme do ruky všechny své karty a dvě z nich odhodí lícem dolů do *talonu*. Se zbylými deseti kartami bude hrát.").NL();
                _printer.PF("Do talonu nesmí být odhozena desítka |X|, eso |A| ani karta, která byla zvolena jako reprezentace trumfů.").NL();
                _printer.PF("Poté následuje sehrání deseti *štychů*, za které jsou přidělovány body.").NL();
                _printer.NL().PF("  *|Sehrání štychu|*").NL().NL();
                _printer.PF("Každý hráč postupně po směru hry odhodí do štychu jednu kartu. První štych *vynáší* aktér - odhazje první kartu.").NL();
                _printer.PF("První kartu vynášející zvolí libovolně. Každý ze zbylých dvou hráčů se musí řídit následujícími pravidly:").NL();
                _printer.PF(" - *Přiznat barvu*").NL();
                _printer.PF("   Pokud má hráč kartu stejné barvy, jako první karta štychu, musí zahrát kartu této barvy.").NL();
                _printer.PF("   - *Jinak zahrát trumf*").NL();
                _printer.PF("     Pokud nemůže hráč přiznat barvu, ale má trumf, musí zahrát trumf.").NL();
                _printer.PF("     Pokud hráč nemá ani trumf, může zahrát libovolnou kartu.").NL();
                _printer.PF(" - *Přebíjet*").NL();
                _printer.PF("   Pokud může hráč přebít nejsilnější kartu štychu, musí jí přebít. Přiznání barvy má ale vyšší prioritu.").NL();
                _printer.PF("   Síla karty je určena prvotně podle barvy - nejsilnější jsou *trumfy*, potom barva *první karty štychu*, pak ostatní.").NL();
                _printer.PF("   Druhotně podle její hodnoty - seřazeno od nejsilnější po nejslabší:").NL();
                _printer.PF("   Eso |A|, desítka |X|, král |K|, svršek |Q|, spodek |J|, devítka |9|, osma |8|, sedma |7|.").NL().NL();
                _printer.PF("Pokud hráč zahraje krále |K| (nabo svrška |Q|) a má svrška |Q| (nebo krále |K|) stejné barvy v ruce, tak zahrál tzv *hlášku*.").NL();
                _printer.PF("Za hlášku jsou bonusové body a oznamuje se veřejně všem hráčům.").NL().NL();
                _printer.PF("Poté, co odehraje třetí hráč, tak hráč, který zahrál nejsilnější kartu vyhrává štych a *vynáší* příští štych.").NL();
                _printer.PF("Po odehrání všech deseti štychů se sehrávka vyhodnotí.").NL();
                _printer.NL().PF("  *|Vyhodnocení|*").NL().NL();
                _printer.PF("Každý hráč získá:").NL();
                _printer.PF("  *10 bodů* za každé eso |A| a každou desítku |X| v štyších, které vyhrál.").NL();
                _printer.PF("  *20 bodů* za každou hlášku.").NL();
                _printer.PF("  *10 bodů* za vyhrání posledního štychu.").NL();
                _printer.PF("Kolo *vyhrává* ten tým, který nasbíral více bodů. Maximálně lze dohromady získat 190 bodů.").NL().NL();
                _printer.PF("Nakonec se seberou všecnhy karty, nemíchají se, jen se sejmou na náhodném místě v balíčku.").NL();
                _printer.PF("V příštím kole bude aktér hráč po směru hry od momentálního aktéra.");
            }
            else
            {
                _printer.CLR().R().G().P("Nastavení hry:").NL().NL();
                for (int i = 0; i < 3; i++)
                {
                    _printer.W().P(" ").B(ConsoleColor.Blue).P(" ").P(_playerNames[i], 7, true).B().P("  ");
                }
                _printer.NL();
                for (int i = 0; i < 3; i++)
                {
                    _printer.W().P(" ").B(ConsoleColor.Blue).P(players[i].label, 8, true).B().P("  ");
                }
            }
            _printer.NL().NL();
            switch (state)
            {
                case State.Menu:
                    _printer.G().P("| ").H("Start | Změň ovladač ").H("Jardy, ").H("Franty nebo ").H("Karla | ").H("Pravidla | ").H("Mód: ");
                    if (MODE_OPTIONS[mode_selected] == -1)
                    {
                        _printer.P("normální |");
                    }
                    else
                    {
                        _printer.P($"simulace {MODE_OPTIONS[mode_selected]} her |");
                    }
                    break;
                case State.Rules:
                    _printer.G().P("| ").H("Start | ").H("Zpět do menu |");
                    break;
            }
            _printer.NL();
        }

        void ChangeAI(int player)
        {
            int a = (players[player].id + 1) % PlayerController.TYPES.Count;
            players[player] = PlayerController.TYPES[a];
        }

        void ChangeMode()
        {
            mode_selected = (mode_selected + 1) % MODE_OPTIONS.Length;
        }

        public Game NewGame()
        {
            _printer.CLR().R();
            return new Game(players, MODE_OPTIONS[mode_selected]);
        }
    }
}