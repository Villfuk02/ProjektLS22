# ZapoctovyProjektLS22

Zápočtový projekt na letní semestr roku 2022.

Zjednodušená varianta Mariáše, konzolová aplikace psaná v C#. Součástí je několik různých botů, proti kterým se dá hrát.

## Uživatelská dokumentace

Aplikace se ovládá klávesnicí. Na konci výstupu jsou vždy možnosti ovládání. Každá možnost má označené jedno písmeno obdélníkem - stisknutím tohoto písmena se možnost zvolí.

### Hlavní menu

První obrazovka, se ktrou se setkáte je hlavní menu. Zde se dá nastavit hra.

Ve třech modrých obdélnících jsou vypsané informace o hráčích. Jejich jména jsou napevno nastavená na *Jarda*, *Franta* a *Karel*. Pod jménem hráče se nachází název **ovladače**. Ovladač hráče se dá změnit a určuje jeho chování.

Možnosti v menu:

- Start - odstartuje hru s aktuálním nastavením.
- Změň ovladač Jardy, Franty nebo Karla - stisknutím příslušné klávesy se změní ovladač příslušného hráče. Možnosti jsou následující:
  - Člověk - hráč je ovládán uživatelem pomocí klávesnice.
  - RandomAI - vždy zahraje náhodou kartu z těch které mu pravidla dovolují.
  - SmartAI - rozhoduje se velmi chytře na základě složitého algoritmu.
  - HybridAI - první 4 kola hraje jako SmartAI, zbylých 6 odsimuluje, a použije strategii, která mu dá nejvyšší šanci na výhru.
  - Sim-10K - simuluje prakticky celou hru, ale vždy prohledává jen 10 tisíc možností.
  - Sim-100K - simuluje prakticky celou hru, ale vždy prohledává jen 100 tisíc možností. Rozhodování může zabrat vteřiny.
  - Sim-1M - simuluje prakticky celou hru, ale vždy prohledává jen 1 milion možností. Rozhodování může zabrat desítky vteřin.
  - Sim-Full - simuluje celou hru. Rozhodování může zabrat minuty.
- Pravidla - zobrazí pravidla. Silně doporučeno novým uživatelům.
- Mód
  - normální - hra probíhá normálně s grafickým zobrazením.
  - simulace X her - proběhne X her bez grafického zobrazení, zobrazují se pouze statistiky. Doporučeno pouze pokud žádný ovladač není *Člověk*.
  
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

Toto je extrémně pomalé, protože kombinací rozdání karet může být vyzkoušeno až 19 399 380, a navíc strom sumulace se extrémně rychle větví. Například první hráč může zahrát libovolnou z 10 karet, druhý třeba jednu z 5 a třetí třeba taky, potom další hrač libovolnou z 9 a tak dále. Takže hra může probíhat ***x*** různými způsoby, kde 10! < ***x*** <(10!)^2.

### Benchmarks
