#ifndef TILE_H
#define TILE_H

class Tile
{
public:
    Tile();
    Tile(int xpos, int ypos);

    void setPos(int xpos, int ypos);
    void display();

    int x, y;
    bool lwall, rwall, uwall, dwall;
};

#endif // TILE_H
