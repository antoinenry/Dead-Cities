#include "tilemap.h"

Tilemap::Tilemap()
    :h(1), w(1)
{}

Tilemap::Tilemap(const unsigned int& width, const unsigned int& height)
    :h(height), w(width)
{
    Tile empty_tile;

    for(unsigned int iy=0; iy<h; iy++)
    {
        for(unsigned int ix=0; ix<w; ix++)
        {
            empty_tile.setPos(ix, iy);
            T.push_back(empty_tile);
        }
    }
}

Tile& Tilemap::operator[](vec2 pos)
{
    assert(pos.x<w && pos.x>=0 && pos.y<h && pos.y>=0);
    unsigned int x = pos.x, y = pos.y;
    return T[y*w+x];
}

unsigned int Tilemap::geth()const
{
    return h;
}

unsigned int Tilemap::getw() const
{
    return w;
}

void Tilemap::seth(unsigned int val)
{
    Tile empty_tile;
    for(unsigned int iy=h; iy<val; iy++)
    {
        for(unsigned int ix=0; ix<w; ix++)
        {
            empty_tile.setPos(ix, iy);
            T.push_back(empty_tile);
        }
    }
    h=val;
}

void Tilemap::setw(unsigned int val)
{
    Tile empty_tile;
    std::vector<Tile> temp;
    for(unsigned int iy=0; iy<h; iy++)
    {
        for(unsigned int ix=0; ix<w; ix++)
        {
            temp.push_back(T[ix+iy*w]);
        }
        for(unsigned int ix=w; ix<val; ix++)
        {
            empty_tile.setPos(ix, iy);
            temp.push_back(empty_tile);
        }
    }
    T=temp;
    w=val;
}

void Tilemap::setTile(const unsigned int& xpos, const unsigned int& ypos, const int& left, const int& right, const int& up, const int& down)
{
    if(left==0 || left==1) T[xpos+ypos*w].lwall=left;
    else if(left==-1) T[xpos+ypos*w].lwall=!T[xpos+ypos*w].lwall;

    if(right==0 || right==1) T[xpos+ypos*w].rwall=right;
    else if(right==-1) T[xpos+ypos*w].rwall=!T[xpos+ypos*w].rwall;

    if(up==0 || up==1) T[xpos+ypos*w].uwall=up;
    else if(up==-1) T[xpos+ypos*w].uwall=!T[xpos+ypos*w].uwall;

    if(down==0 || down==1) T[xpos+ypos*w].dwall=down;
    else if(down==-1) T[xpos+ypos*w].dwall=!T[xpos+ypos*w].dwall;
}

void Tilemap::setTile(const vec2& pos, const int& left, const int& right, const int& up, const int& down)
{
    setTile(pos.x, pos.y, left, right, up, down);
}

Tile Tilemap::getTile(const unsigned int& xpos, const unsigned int& ypos) const
{
    return T[xpos+ypos*w];
}

Tile Tilemap::getTile(const vec2& pos) const
{
    return T[pos.x+pos.y*w];
}

bool Tilemap::isInTilemap(const vec2 &pos) const
{
    if(pos.x<0 || pos.x>=w || pos.y<0 || pos.y>=h)
        return false;

    return true;
}

void Tilemap::display()
{
    glBegin(GL_LINES);

    //Grille
    glColor3ub(55, 55, 55);
    for(unsigned int iy=0; iy<=h; iy++)
    {
        glVertex2i(0, iy); glVertex2i(w,iy);
    }
    for(unsigned int ix=0; ix<=w; ix++)
    {
        glVertex2i(ix, 0); glVertex2i(ix,h);
    }

    //Murs
    glColor3ub(255, 255, 255);
    for(unsigned int iy=0; iy<h; iy++)
    {
        for(unsigned int ix=0; ix<w; ix++)
        {
            if(getTile(ix,iy).uwall){
            glVertex2i(ix, iy); glVertex2i(ix+1,iy);}

            if(getTile(ix,iy).dwall){
            glVertex2i(ix, iy+1); glVertex2i(ix+1,iy+1);}

            if(getTile(ix,iy).lwall){
            glVertex2i(ix, iy); glVertex2i(ix,iy+1);}

            if(getTile(ix,iy).rwall){
            glVertex2i(ix+1, iy); glVertex2i(ix+1,iy+1);}
        }
    }
    glEnd();
}

void Tilemap::saveDPM(const std::string& fichier)
{
    const char* chfichier = fichier.c_str();

    std::ofstream offlux(chfichier);

    offlux<<w<<" "<<h<<std::endl;

    for(unsigned int iy=0; iy<h; iy++)
    {
        for(unsigned int ix=0; ix<w; ix++)
        {
            offlux<<getTile(ix,iy).lwall<<getTile(ix,iy).rwall<<getTile(ix,iy).uwall<<getTile(ix,iy).dwall<<" ";
        }
        offlux<<std::endl;
    }
    offlux.close();
}

void Tilemap::loadDPM(const std::string& fichier)
{
    const char* chfichier = fichier.c_str();
    int l, r, u, d, n;

    std::ifstream ifflux(chfichier);

    ifflux>>w; ifflux>>h;
    T.resize(w*h);
    std::cout<<w<<" "<<h<<std::endl;

    for(unsigned int iy=0; iy<h; iy++)
    {
        for(unsigned int ix=0; ix<w; ix++)
        {
            ifflux>>n;
            l=n/1000;
            r=(n/100)%10;
            u=((n/10)%100)%10;
            d=((n%1000)%100)%10;

            setTile(ix, iy, l, r, u, d);
            std::cout<<l<<r<<u<<d<<" ";
        }
        std::cout<<std::endl;
    }

    ifflux.close();
}




