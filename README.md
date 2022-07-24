# ZapoctovyProjektLS22

Zápočtový projekt na letní semestr roku 2022.

Zjednodušená varianta Mariáše, konzolová aplikace psaná v C#. Součástí je několik různých botů, proti kterým se dá hrát.

## Uživatelská dokumentace

Aplikace se ovládá klávesnicí. Na konci výstupu jsou vždy možnosti ovládání. Každá možnost má označené jedno písmeno obdélníkem - stisknutím tohoto písmena se možnost zvolí.

### Hlavní menu

První obrazovka, se kterou se setkáte je hlavní menu. Zde se dá nastavit hra.

Ve třech modrých obdélnících jsou vypsané informace o hráčích. Jejich jména jsou napevno nastavená na *Jarda*, *Franta* a *Karel*. Pod jménem hráče se nachází název **ovladače**. Ovladač hráče se dá změnit a určuje jeho chování.

Možnosti v menu:

- Start - odstartuje hru s aktuálním nastavením.
- Změň ovladač Jardy, Franty nebo Karla - stisknutím příslušné klávesy se změní ovladač příslušného hráče. Možnosti jsou následující:
  - Člověk - hráč je ovládán uživatelem pomocí klávesnice.
  - RandomAI - vždy zahraje náhodou kartu z těch které mu pravidla dovolují.
  - SmartAI - rozhoduje se velmi chytře na základě složitého algoritmu.
  - HybridAI - první 4 kola hraje jako SmartAI, zbylých 6 odsimuluje, a použije strategii, která mu dá nejvyšší šanci na výhru.
  - Sim-10K - simuluje prakticky celou hru, ale vždy prohledává jen 10 tisíc kombinací rozdání karet.
  - Sim-100K - simuluje prakticky celou hru, ale vždy prohledává jen 100 tisíc kombinací rozdání karet. Rozhodování může zabrat vteřiny.
  - Sim-1M - simuluje prakticky celou hru, ale vždy prohledává jen 1 milion kombinací rozdání karet. Rozhodování může zabrat vteřiny.
  - Sim-Full - simuluje celou hru. Rozhodování může desítky vteřin.
- Pravidla - zobrazí pravidla. Silně doporučeno novým uživatelům.
- Mód
  - normální - hra probíhá normálně s grafickým zobrazením.
  - simulace X her - proběhne X her bez grafického zobrazení, zobrazují se pouze statistiky. Doporučeno pouze pokud žádný ovladač není *Člověk*.
  
### Pravidla

Tato hra je založená na *Mariáši*. Je ale velmi zjednodušená.

Hra je pro tři hráče. V každém kole (sehrávce) je jeden hráč určen jako *aktér*, který je označen *červeně*.
Zbylí dva hráči - *opozice* - hrajou spolu proti aktérovi. Tým, kterým má na konci sehrávky více bodů vyhrává.

#### Průběh kola

Na začátku kola je aktérovi rozdáno 7 + 5 karet a každému hráči opozice 5 + 5.

Aktér se smí dívat jen na prvních sedm svých karet a z nich vybere jednu, kterou ukáže ostatním.
Barva této karty určuje barvu *trumfů* po zbytek sehrávky.
Pokud si aktér nechce vybrat z prvních sedmi karet, může vybrat *z lidu* - vybere kartu náhodně ze zbylých pěti.

Potom si vezme do ruky všechny své karty a dvě z nich odhodí lícem dolů do *talonu*. Se zbylými deseti kartami bude hrát.
Do talonu nesmí být odhozena desítka |X|, eso |A| ani karta, která byla zvolena jako reprezentace trumfů.

Poté následuje sehrání deseti *štychů*, za které jsou přidělovány body.

#### Sehrání štychu

Každý hráč postupně po směru hry odhodí do štychu jednu kartu. První štych *vynáší* aktér - odhazuje první kartu.
První kartu vynášející zvolí libovolně. Každý ze zbylých dvou hráčů se musí řídit následujícími pravidly:

- Přiznat barvu

  Pokud má hráč kartu stejné barvy, jako první karta štychu, musí zahrát kartu této barvy.
  
  - Jinak zahrát trumf
  
    Pokud nemůže hráč přiznat barvu, ale má trumf, musí zahrát trumf.
    Pokud hráč nemá ani trumf, může zahrát libovolnou kartu.
- Přebíjet

  Pokud může hráč přebít nejsilnější kartu štychu, musí jí přebít. Přiznání barvy má ale vyšší prioritu.
  
  Síla karty je určena prvotně podle barvy - nejsilnější jsou *trumfy*, potom barva *první karty štychu*, pak ostatní.
  
  Druhotně podle její hodnoty - seřazeno od nejsilnější po nejslabší:
  Eso |A|, desítka |X|, král |K|, svršek |Q|, spodek |J|, devítka |9|, osma |8|, sedma |7|.

Pokud hráč zahraje krále |K| (nabo svrška |Q|) a má svrška |Q| (nebo krále |K|) stejné barvy v ruce, tak zahrál tzv *hlášku*.
Za hlášku jsou bonusové body a oznamuje se veřejně všem hráčům.

Poté, co odehraje třetí hráč, tak hráč, který zahrál nejsilnější kartu vyhrává štych a *vynáší* příští štych.
Po odehrání všech deseti štychů se sehrávka vyhodnotí.

#### Vyhodnocení

Každý hráč získá:

- *10 bodů* za každé eso |A| a každou desítku |X| v štyších, které vyhrál.
- *20 bodů* za každou hlášku.
- *10 bodů* za vyhrání posledního štychu.

Kolo *vyhrává* ten tým, který nasbíral více bodů. Maximálně lze dohromady získat 170 bodů.

Nakonec se seberou všecnhy karty, nemíchají se, jen se sejmou na náhodném místě v balíčku.
V příštím kole bude aktér hráč po směru hry od momentálního aktéra.)
  
### Hra, normální mód

V horní části se zobrazuje status hry a případně zvolené trumfy.

Pod ním se většinou zobrazuje momentální štych. Každá karta je umístěná směrem k hráči, který ji zahrál.

Níže jsou jména hráčů a u každého příslušný počet výher.

Pod jmény se zobrazují karty, které mají hráči v ruce. Modré obdélníky představují karty, na které uživatel nevidí. Vedle každé ruky se případně ukazuje, které hlášky tento hráč zahrál a kolik vyhrál štychů.

Pod těmito kartami se můžou ukazovat písmena. Ty určují která karta bude vybrána stisknutím které klávesy.

### Hra, mód simulace

Na každé řádce jsou informace z daného okamžiku průběhu simulace oddělené |:

- Čas od zahájení  
- Počet her zbývajících do konce simulace
- Výsledky jednotlivých hráčů označené jejich iniciály
  - a - počet výher jako aktér
  - o - počet výher v opozici
  - C - celkový počet výher

## Programátorská dokumentace

Většina věcí by měla být zřejmá z kódu a komentářů v něm. Zde jsou jen některé obecnější myšlenky, které se hodí přečíst předem.

### SmartAI

Sleduje stav hry a předpovídá které karty můžou a nemůžou ostatní hráči mít. Podle této předpovědi ohodnotí každou kartu, kterou může zahrát, podle toho jak moc si myslí, že by bylo dobré ji zahrát. Pak zahraje tu s nejlepším hodnocením.

při hodnocení karet počítá pravděpodobnosti výskytu různých úkazů. Všechny tyto pravděpodobnosti jsou pouze odhady a často mohou vyjít záporně nebo větší než 1. Pro účely ohodnocení karty jsou ale dostatečné.

### SimAI

Podle informací o tom, které karty můžou ostatní hráči mít, vygeneruje všechny kombinace, jak by mohly karty být rozdané. Poté vybere několik až všecny tyto permutace a u každé minmaxem odsimuluje, jestli může vyhrát po zahrátí každé z karet. Karta, která má nejvíc potenciálních výher je vybrána. V případě remízy se rozhodne jako SmartAI.

Toto je extrémně pomalé, protože kombinací rozdání karet může být vyzkoušeno až 19 399 380, a navíc strom sumulace se extrémně rychle větví. Například první hráč může zahrát libovolnou z 10 karet, druhý třeba jednu z 5 a třetí třeba taky, potom další hrač libovolnou z 9 a tak dále. Takže hra může probíhat ***x*** různými způsoby, kde 10! < ***x*** < (10!)^3.

Většinou SimAI stejně dojde k tomu, že prohraje ať zahraje cokoli nebo že vyhraje ať zahraje cokoli, takže se často rozhodnutí určuje podle SmartAI.V opačném případě často volí možnost, kterou by SmartAI nezvolilo.

### Testy

Každé AI jsem otestoval odehráním 10 000 her proti dvěma SmartAI. Některé testy jsem pustil vícekrát a výsledky zprůměroval:

| AI       | Výher | Čas     | Počet testů |
|----------|-------|---------|-------------|
| RandomAI | 3783  | 1s      | 10          |
| SmartAI  | 5013  | 1s      | 10          |
| HybridAI | 4990  | 1:07    | 10          |
| Sim-10K  | 5004  | 6:35    | 5           |
| Sim-100K | 4994  | 15:45   | 3           |
| Sim-1M   | 5017  | 44:47   | 1           |
| Sim-Full | 5035  | 3:04:39 | 1           |

Výsledky nabádají k tvzení, že všechny AI (kromě RandomAI) jsou stejně dobré. U většiny testů byla odchylka +- 100 výher.
