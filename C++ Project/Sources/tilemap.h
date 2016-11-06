#ifndef TILEMAP_H
#define TILEMAP_H

#include "tile.h"
#include "vec2.h"

#include <assert.h>
#include <vector>
#include <fstream>
#include <iostream>

//Librairies graphiques
#include <GL/glew.h>
#include <GL/glut.h>

class Tilemap
{
    public:
        Tilemap();
        Tilemap(const unsigned int& width, const unsigned int& height);

        Tile& operator[](vec2 pos);

        unsigned int geth() const;
        unsigned int getw() const;
        void seth(unsigned int val);
        void setw(unsigned int val);
        void setTile(const unsigned int& xpos, const unsigned int& ypos, const int &left, const int &right, const int &up, const int &down);
        void setTile(const vec2& pos, const int &left, const int &right, const int &up, const int &down);
        Tile getTile(const unsigned int& xpos, const unsigned int& ypos) const;
        Tile getTile(const vec2& pos) const;

        bool isInTilemap(const vec2& pos) const;
        void display();

        void saveDPM(const std::string &fichier);
        void loadDPM(const std::string& fichier);
    private:
        unsigned int h, w;
        std::vector<Tile> T;
};

#endif // Tilemap_H
