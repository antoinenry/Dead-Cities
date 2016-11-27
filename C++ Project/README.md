C++ Version (2015)
==================

This first version of the project was developped on Qt Linux, using OpenGL and GLUT.

![Deadcities](https://github.com/antoinenry/Dead-Cities/blob/master/C++%20Project/screenshot.png)

Main features
-------------

- Static maps (walls and door) with basic map editor (not on this git)
- Randomly generated characters and zombies (male/female, clothes, skin and hair color)
- Minimalist sprite animations (based on the FTL models)
- Mouse control with action menus (walk, run, open/close door, follow)
- Multiple characters control
- Keyboard control (movements only)
- Fonctionnal artificial intelligence for zombies

Project Status
--------------

I've stopped developpement on this version since I've started working with unity. These are the three main reasons of this change:

- I wanted to implement a random city generator, which worked better in my head in a 3D environment, and I didn't want to use 3D modeling.
- Unity seemed like a better way to try out ideas and make adjustments.
- I wanted to work with Unity.

IA
--

This is a feature I find very interesting to develop and that I'm looking forward to work on in the Unity3D version. The zombies have a pretty natural behaviour, wandering around until they see something that might interest them. This can be a human, but also another hostile or an open door. They are able to destroy the doors if they see someone enter a building. The random component gives them a really nice presence, sometime even scary: they can stop in front of the building you're hiding in, hesitate and then go away.
My objective is to transfer this behaviour to my Unity3D project, and enhance it even more. I'm thinking about adding a hearing radius to their detection capabilities.

