# Marble Solitaire Solution Explorer

(aka Peg Solitaire)

## Problem

<p>There are 57.7 x 10<sup>19</sup> ways of playing this game and ending up with a 
single piece at the end of the game - a winning board has a single piece in the centre of the board. 
There are only 1,679,072 unique winning board positions after taking account of rotations and  either 
vertically or horizontally. This project explores all the possible board positions that lead to a winning board.
</p>

## Contents of Solution

Uses a Modular structure so any front end could be used, projects are:

1. MarbleSolitaireLib - contains the solver which uses depth first search and in memory memoization 
2. MarbleSolitaireModelLib - model of a generic square based board
3. MarbleSolCommonLib - Contains BitBoard used for bit twiddling amongst other things
4. MarbleSolitaireViewModel - very very dependency light on view technology. Contains an Undo Redo module for stepping ...back and forth through a game
5. MarbleSolitaire - The View - WPF app but could be anything
6. TestMarbleSolitaire - tests some of which may take a couple of minutes to run - see readme.txt in the test project for more details as these can be optinally compiled.

Doesn't cache the winning positions as it is more interesting to see which board positions can be searched instantly from those taking a few seconds.

## Winning Positions

The crux is the output of TestCountUniqueSolutions.   
(select test and click on the output):

Unique number of winning boards listed by pieces count

|Pieces Count  |Number of boards|
|--------------|---------------:|
| 1	|         1|
| 2 |         1|
| 3 |         2|
| 4 |         8|
| 5 |        38|
| 6 |       164|
| 7 |       635|
| 8 |     2,089|
| 9 |     6,174|
|10 |    16,020|
|11 |    35,749|
|12 |    68,326|
|13 |   112,788|
|14 |   162,319|
|15 |   204,992|
|16 |   230,230|
|17 |   230,230|
|18 |   204,992|
|19 |   162,319|
|20 |   112,788|
|21 |    68,326|
|22 |    35,749|
|23 |    16,020|
|24 |     6,174|
|25 |     2,089|
|26 |       635|
|27 |       164|
|28 |        38|
|29 |         8|
|30 |         2|
|31 |         1|
|32 |         1|
|   | ---------|
|   | 1,679,072|



## ViewModel dependency on View
 
Very very minimal - only the DesignerWorkFlow.cs file under the serviceLocator folder takes a dependency on WPF the Presentation framework dll.
This is a simple pass through class which checks if we are in 'design mode' and resolves dependencies as appropriate so can easily be adapted.


## Running Solution

### Requires x64 Platform target

The project MarbleSolitaire needs to target x64 architecture as it needs upward of 8gb ram to fully enumerate all boards

This can be set by right clicking on the project and choosing the x64 platform target from the Build tab.
Additionally Tests require that x64 is set - this can be done under the Test menu - test settings - default processor architecture
select x64 otherwise the tests will fail due to out of memory issues.

### Editing xaml

Editing the view from the MarbleSolitaire project using Expression Blend designer requires 32bit build so switch that platform target back to any Cpu, build 
then switch back to targeting x64 as mentioned above to run the project.

## View

WPF xaml

![alt text](https://github.com/AndrewH2O/MarbleSolitaire/raw/master/MarbleSolitaire/img/marbleSolView.png "Game Explorer")