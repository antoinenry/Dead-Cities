C++ Version (2015)
==================

This second version of the project was made on Windows with Unity3D and C#.

![Deadcities](https://github.com/antoinenry/Dead-Cities/blob/master/Unity3D%20Project/screenshot.png)

Main features
-------------

- 3D
- Random city generation
- Character mouse control and UI
- Pathfinding
- Dynamic expanding world
- Basic camera works

Project Status
--------------

I'm currently working on this version. I've been concentrating my effort mainly on the world generation, which is one of the reasons I switched to Unity. The exterior (building and streets) works fine but I'm still working on generating the inside of each building. 

World Generation
----------------

This is the main feature I've been developping in this version. I've started drawing doodles of city maps, and then tried to recreate my process in code: first trace the main streets, then smaller streets, then fill the gap with various buildings. The result is fonctionnal, but lacks a little variety. The program generates the city block-by-block and adds new blocks each time a character gets close to the world limits. My plan is to find a few other generation methods for other types of blocks (plazza, parking, mall...), and pick a different method each time I generate a new block. 
