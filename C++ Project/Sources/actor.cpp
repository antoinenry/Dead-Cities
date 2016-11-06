#include "actor.h"

Actor::Actor(const int &xpos, const int &ypos)
    :pos(vec2(xpos, ypos)), des(vec2(xpos, ypos)), speed(0.10), selected(0)
{
    path.push_back(vec2(xpos,ypos));
}

Actor::Actor(const int &xpos, const int &ypos, const int& i)
    :pos(vec2(xpos, ypos)), des(vec2(xpos, ypos)), speed(0.10), ref(i), selected(0)
{
    path.push_back(vec2(xpos,ypos));
    R=100+15*(ref%10);
    G=100+10*((1+ref)%15);
    B=50*((2+ref)%5);
}

void Actor::display()
{
    //Position
    glColor3ub(R,G,B);
    glBegin(GL_QUADS);
    glVertex2f(pos.x+0.2, pos.y+0.2); glTexCoord2i(0,0);
    glVertex2f(pos.x+0.8, pos.y+0.2); glTexCoord2i(1,0);
    glVertex2f(pos.x+0.8, pos.y+0.8); glTexCoord2i(1,1);
    glVertex2f(pos.x+0.2, pos.y+0.8); glTexCoord2i(0,1);
    glEnd();

    //Selection
    if(selected)
    {
        glColor3ub(255,255,0);
        glBegin(GL_LINE_LOOP);
        glVertex2f(pos.x+0.1, pos.y+0.1); glVertex2f(pos.x+0.9, pos.y+0.1); glVertex2f(pos.x+0.9, pos.y+0.9); glVertex2f(pos.x+0.1, pos.y+0.9);
        glEnd();
    }

    //Destination
    if(pos!=des)
    {
        glColor3ub(R,G,B);
        glBegin(GL_LINE_LOOP);
        glVertex2f(des.x+0.2, des.y+0.2); glVertex2f(des.x+0.8, des.y+0.2); glVertex2f(des.x+0.8, des.y+0.8); glVertex2f(des.x+0.2, des.y+0.8);
        glEnd();
    }


}

void Actor::move(const float& xmov, const float& ymov)
{
    pos+=vec2(xmov, ymov);
}

void Actor::setpos(const int& xpos, const int& ypos)
{
    pos = vec2(xpos, ypos);
}

vec2 Actor::getpos() const
{
    return pos;
}

void Actor::setdes(const int& xdes, const int& ydes)
{
    des = vec2(xdes, ydes);
}

vec2 Actor::getdes() const
{
    return des;
}

int Actor::getref() const
{
    return ref;
}

void Actor::setspeed(const int s)
{
    speed=s;
}

bool Actor::setpath(const Tilemap &m)
{
    unsigned int x,y, nextx, nexty;
    unsigned int counter = 0, counter_max=50, bugcpt=0;
    bool reach=false, notfoundyet=true;
    unsigned int h=m.geth(), w=m.getw();
    std::vector<unsigned int> coeffmap(w*h, counter_max+1);

    srand(time(NULL));

    coeffmap[des.x+w*des.y] = 0;

    while(!reach && counter<counter_max)
    {
        counter++;

        for(y=0; y<h; y++)
        {
            for(x=0; x<w; x++)
            {
                if(coeffmap[x+y*w]==counter-1)
                {
                    //Up
                    if( y>0 && m.getTile(x,y).uwall == 0 && m.getTile(x,y-1).dwall == 0 )
                    {
                        if (coeffmap[x+(y-1)*w]>counter) coeffmap[x+(y-1)*w] = counter;
                        if(path[0].x==x && path[0].y==y-1) reach=true;
                    }
                    //Down
                    if( y<h-1 && m.getTile(x,y).dwall == 0 && m.getTile(x,y+1).uwall == 0 )
                    {
                        if (coeffmap[x+(y+1)*w]>counter) coeffmap[x+(y+1)*w] = counter;
                        if(path[0].x==x && path[0].y==y+1) reach=true;
                    }
                    //Left
                    if( x>0 && m.getTile(x,y).lwall == 0 && m.getTile(x-1,y).rwall == 0 )
                    {
                        if (coeffmap[x-1+y*w]>counter) coeffmap[x-1+y*w] = counter;
                        if(path[0].x==x-1 && path[0].y==y) reach=true;
                    }
                    //Right
                    if( x<w-1 && m.getTile(x,y).rwall == 0 && m.getTile(x+1,y).lwall == 0 )
                    {
                        if (coeffmap[x+1+y*w]>counter) coeffmap[x+1+y*w] = counter;
                        if(path[0].x==x+1 && path[0].y==y) reach=true;
                    }
                }
            }
        }
        if(reach) break;
    }

    if(!reach) {std::cout<<std::endl<<" PATH NOT FOUND "; return 0; }

//    for(y=0; y<h; y++)
//    {
//        for(x=0; x<w; x++)
//        {
//            if(coeffmap[x+y*w]<=counter_max) std::cout<<coeffmap[x+y*w]<<" ";
//            else std::cout<<"- ";
//        }
//        std::cout<<std::endl;
//    }
//    std::cout<<std::endl;

    x=path[0].x; y=path[0].y;
    path.resize(1, vec2(x,y));

    nextx=x; nexty=y;
    while(counter!=0 && bugcpt<counter_max)
    {
        notfoundyet=true;

        //Up
        if(y>0)
        {
            if(coeffmap[x+(y-1)*w]==counter-1 && m.getTile(x,y).uwall==0 && m.getTile(x,y-1).dwall==0)
            {
                nextx=x; nexty=y-1;
                notfoundyet = false;
            }
        }
        //Down
        if(y<h-1)
        {
            if(coeffmap[x+(y+1)*w]==counter-1 && m.getTile(x,y).dwall==0 && m.getTile(x,y+1).uwall==0
                    && (notfoundyet || rand()%2))
            {
                nextx=x; nexty=y+1;
                notfoundyet = false;
            }
        }
        //Left
        if(x>0)
        {
            if(coeffmap[x-1+y*w]==counter-1 && m.getTile(x,y).lwall==0 && m.getTile(x-1,y).rwall==0
                    && (notfoundyet || rand()%2))
            {
                nextx=x-1; nexty=y;
                notfoundyet = false;
            }
        }
        //Right
        if(x<w-1)
        {
            if(coeffmap[x+1+y*w]==counter-1 && m.getTile(x,y).rwall==0 && m.getTile(x+1,y).lwall==0
                    && (notfoundyet || rand()%2))
            {
                nextx=x+1; nexty=y;
                notfoundyet = false;
            }
        }

        path.push_back(vec2(nextx, nexty));

        x=nextx; y=nexty;
        counter--;
        bugcpt++;
    }

    if(bugcpt==counter_max) {std::cout<<std::endl<<" PATH NOT FOUND "; return 0; }

    return 1;
}

void Actor::movetodes()
{
    if(pos!=des)
    {
        if(pos.CompRel(path[0], speed))
        {
            //std::cout<<std::endl<<path[0].x<<","<<path[0].y<<" ERASED. ";
            pos=path[0];
            path.erase(path.begin());
            //std::cout<<"NEXT IS "<<path[0].x<<","<<path[0].y<<std::endl;
        }

        if(path[0].x>pos.x) pos.x+=speed;
        else if(path[0].x<pos.x) pos.x-=speed;

        if(path[0].y>pos.y) pos.y+=speed;
        else if(path[0].y<pos.y) pos.y-=speed;

        //std::cout<<pos.x<<","<<pos.y<<" - ";
    }
}

bool Actor::is_selected()
{
    return selected;
}

bool Actor::toggle_select()
{
    selected=!selected;
    return selected;
}

void Actor::toggle_select(const bool val)
{
    selected=val;
}











