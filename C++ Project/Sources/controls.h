#ifndef CONTROLS_H
#define CONTROLS_H

#include "extern.h"

void keyboard_callback(unsigned char key, int, int);
void special_callback(int key, int, int);
void mouse_callback(int button, int state, int x, int y);
void select_tile(unsigned int x, unsigned int y);
void select_multiple(unsigned int x, unsigned int y);

#endif // CONTROLS_H
