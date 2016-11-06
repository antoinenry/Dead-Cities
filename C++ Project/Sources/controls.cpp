#include "controls.h"

void keyboard_callback(unsigned char key, int, int)
{
    switch (key)
    {
    case '+':
        world_scale++;
        break;
    case '-':
        world_scale--;
        break;
    case '8':
        world_scroll+=vec2(0,world_scale);
        break;
    case '2':
        world_scroll+=vec2(0,-world_scale);
        break;
    case '6':
        world_scroll+=vec2(-world_scale,0);
        break;
    case '4':
        world_scroll+=vec2(world_scale,0);
        break;
    }
}

void special_callback(int key, int, int)
{

}

void mouse_callback(int button, int state, int x, int y)
{
    //Vue


    //Actions
    switch (button)
    {
    case GLUT_LEFT_BUTTON:
        if(state == GLUT_UP)
        {
            if(glutGetModifiers() && GLUT_ACTIVE_CTRL)
            {
                select_multiple(x/world_scale,y/world_scale);
            }
            else
                select_tile(x/world_scale,y/world_scale);
        }
        break;
    }
}

void select_tile(unsigned int x, unsigned int y)
{
    unsigned int i, i_max=character.size(), i_selected=i_max, dx=0, dy=0;

    for(i=0; i<i_max; i++)
    {
        if(vec2(x,y)==character[i].getpos())
        {
            character[i].toggle_select(true);
            i_selected = i;

            std::cout<<i<<", "; std::cout<<"selected"<<std::endl;
        }
        if(i_selected== i_max && vec2(x,y)==character[i].getdes())
        {
            std::cout<<"Already a destination"<<std::endl;
            return;
        }
    }

    if(i_selected!=i_max)
    {
        for(i=0; i<i_max; i++)
        {
            if(i!=i_selected)
                character[i].toggle_select(false);
        }
        return;
    }

    for(i=0; i<i_max; i++)
    {
        if(character[i].is_selected())
        {
            character[i].setdes(x+dx,y+dy);
            character[i].setpath(current_map);

            if(dx==2) { dy++; dx=0;}
            else dx++;
        }
    }
}

void select_multiple(unsigned int x, unsigned int y)
{
    unsigned int i, i_max=character.size();

    for(i=0; i<i_max; i++)
    {
        if(vec2(x,y)==character[i].getpos())
        {
            character[i].toggle_select();
            std::cout<<i<<", "; std::cout<<"selected"<<std::endl;
        }
    }
}




