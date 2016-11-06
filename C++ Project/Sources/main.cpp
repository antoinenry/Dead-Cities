#include "extern.h"
#include "controls.h"

#define CALLBACK_FREQ 25

Tilemap current_map;
std::vector<Actor> character;
int world_scale=50;
vec2 world_scroll(0,0);

int GTimer_cpt=0;


static void timer_callback(int)
{
    glutTimerFunc(CALLBACK_FREQ, timer_callback, 0);

    //Rafraichissement des données de scène
    //Personnages
    for(unsigned int i=0, imax=character.size(); i<imax; i++)
    {
        character[i].movetodes();
    }

    GTimer_cpt++;
    //reactualisation de l'affichage
    glutPostRedisplay();
}

int main(int argc, char** argv)
{
    current_map.loadDPM("testmap.dpm");

    character.push_back(Actor(5,5,0));
    character.push_back(Actor(10,5,1));
    character.push_back(Actor(9,9,2));
    character.push_back(Actor(0,8,3));
    character.push_back(Actor(10,12,4));
    character.push_back(Actor(13,13,5));

    display_init(argc, argv);

    //Fonction de gestion du clavier et souris
    glutKeyboardFunc(keyboard_callback);
    glutMouseFunc(mouse_callback);

//    //Fonction des touches speciales du clavier
    glutSpecialFunc(special_callback);

    //Fonction d'appel d'affichage en chaine
    glutTimerFunc(CALLBACK_FREQ, timer_callback, 0);

    //Initialisation des fonctions OpenGL
    glewInit();

    //Lancement de la boucle principale
    glutMainLoop();


    return 0;

}

