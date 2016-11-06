#include <iostream>

//Librairies stadards
#include <vector>

//Librairies graphiques
#include <GL/glew.h>
#include <GL/glut.h>

//Classes
#include "actor.h"
#include "tilemap.h"
#include "vec2.h"

//Libs persos
#include "display.h"


extern std::vector<Actor> character;
extern Tilemap current_map;
extern int world_scale;
extern vec2 world_scroll;

extern int GTimer_cpt;
