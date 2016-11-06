#include "display.h"

void display_init(int argc, char** argv)
{
    glutInit(&argc, argv);
    glutInitDisplayMode(GLUT_DOUBLE | GLUT_RGB | GLUT_DEPTH);
    glutInitWindowSize(SCREEN_W, SCREEN_H);
    glutCreateWindow("DeadPixels");


    //Lancement de la boucle d'affichage
    glutDisplayFunc(display_callback);
}

void display_callback()
{
    //Effacement
    //glClearColor(1, 1, 1, 1.0);
    glClear(GL_COLOR_BUFFER_BIT);

    //Configuration 2D
    glDisable(GL_DEPTH_TEST);
    glMatrixMode(GL_PROJECTION);
    glLoadIdentity();
    glOrtho(0, SCREEN_W, SCREEN_H, 0, 0, 1);
    glMatrixMode(GL_MODELVIEW);

    //Affichage de la scène 
    glLoadIdentity();
    glTranslated(world_scroll.x, world_scroll.y, 0.0);
    glScaled(world_scale, world_scale, 1.0);

    current_map.display();
    for(unsigned int i=0, imax=character.size(); i<imax; i++)
        character[i].display();


    //Changement du buffer d'affichage
    glutSwapBuffers();
}
