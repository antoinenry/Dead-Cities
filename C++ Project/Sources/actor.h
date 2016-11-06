#ifndef ACTOR_H
#define ACTOR_H

//Librairies graphiques
#include <GL/glew.h>
#include <GL/glut.h>

#include <iostream>

#include <vector>
#include <time.h>
#include <math.h>

#include "vec2.h"
#include "tilemap.h"

class Actor
{
    public:
        Actor();
        Actor(const int& xpos, const int& ypos);
        Actor(const int& xpos, const int& ypos, const int &i);
        void display();
        void move(const float &xmov, const float &ymov);
        void setpos(const int &xpos, const int &ypos);
        void setdes(const int& xdes, const int& ydes);
        vec2 getpos() const;
        vec2 getdes() const;
        int getref() const;
        int getavspeed() const;
        void setspeed(const int s);
        void movetodes();
        bool setpath(const Tilemap &m);
        bool is_selected();
        bool toggle_select();
        void toggle_select(const bool val);


    private:
        vec2 pos, des;
        std::vector<vec2> path;
        float avspeed, speed;
        int ref;
        bool selected;
        int R,G,B;
};

#endif // ACTOR_H
