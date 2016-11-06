#include "tile.h"

Tile::Tile()
    :x(0), y(0), lwall(0), rwall(0), uwall(0), dwall(0)
{}

Tile::Tile(int xpos, int ypos)
    :x(xpos), y(ypos), lwall(0), rwall(0), uwall(0), dwall(0)
{}

void Tile::setPos(int xpos, int ypos)
{
    x=xpos;
    y=ypos;
}
