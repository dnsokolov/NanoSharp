/*
   Copyright (c) 2019 Sokolov Denis
 */

#include "mcengine.h"

void run_without_temperature(mc_engine *engine, int mc_steps) {
    double tx, ty, tz;
    double old_energy, new_energy;
    int adress;

    for(int s = 0; s < mc_steps; ++s) {
        #ifdef DEBUG
            printf("step %d\n", s);
        #endif
        for(int a = 0; a < engine->num_of_atoms; ++a) {
           if(engine->freezen[a]) continue;
           old_energy = get_energy(a, engine);
           #ifdef DEBUG
            printf("old_energy[%d]: %f\n", a, old_energy);
           #endif
           tx = ((double)rand() / RAND_MAX * 2.0 - 1.0) * engine->max_translation;
           ty = ((double)rand() / RAND_MAX * 2.0 - 1.0) * engine->max_translation;
           tz = ((double)rand() / RAND_MAX * 2.0 - 1.0) * engine->max_translation;
           adress = (a << 1) + a;
           #ifdef DEBUG
              printf("%f %f %f\n", tx, ty, tz);
           #endif
           engine->positions[adress] += tx;
           engine->positions[adress + 1] += ty;
           engine->positions[adress + 2] += tz;
           new_energy = get_energy(a, engine);
           #ifdef DEBUG
            printf("new_energy[%d]: %f\n", a, new_energy);
           #endif
           if(new_energy > old_energy) {
               #ifdef DEBUG
                  printf("Not accepted\n");
               #endif
               engine->positions[adress] -= tx;
               engine->positions[adress + 1] -= ty;
               engine->positions[adress + 2] -= tz;
               engine->energies[a] = old_energy;
           }
           #ifdef DEBUG
               else {
                     printf("Accepted\n");
               }
            #endif
        }
    }
}

void run_without_temperature_only_gupta(mc_engine *engine, int mc_steps) {
    srand(time(NULL));
    double tx, ty, tz;
    double old_energy, new_energy;
    int adress;

    for(int s = 0; s < mc_steps; ++s) {
        #ifdef DEBUG
            printf("step %d\n", s);
        #endif
        for(int a = 0; a < engine->num_of_atoms; ++a) {
           if(engine->freezen[a]) continue;
           old_energy = get_gupta_energy(a, engine);
           #ifdef DEBUG
            printf("old_energy[%d]: %f\n", a, old_energy);
           #endif
           tx = ((double)rand() / RAND_MAX * 2.0 - 1.0) * engine->max_translation;
           ty = ((double)rand() / RAND_MAX * 2.0 - 1.0) * engine->max_translation;
           tz = ((double)rand() / RAND_MAX * 2.0 - 1.0) * engine->max_translation;
           adress = (a << 1) + a;
           #ifdef DEBUG
              printf("%f %f %f\n", tx, ty, tz);
           #endif
           engine->positions[adress] += tx;
           engine->positions[adress + 1] += ty;
           engine->positions[adress + 2] += tz;
           new_energy = get_gupta_energy(a, engine);
           #ifdef DEBUG
            printf("new_energy[%d]: %f\n", a, new_energy);
           #endif
           if(new_energy > old_energy) {
               #ifdef DEBUG
                  printf("Not accepted\n");
               #endif
               engine->positions[adress] -= tx;
               engine->positions[adress + 1] -= ty;
               engine->positions[adress + 2] -= tz;
               engine->energies[a] = old_energy;
           }
           #ifdef DEBUG
               else {
                     printf("Accepted\n");
               }
            #endif
        }
    }
}

void run_without_temperature_only_lj(mc_engine *engine, int mc_steps) {
    srand(time(NULL));
    double tx, ty, tz;
    double old_energy, new_energy;
    int adress;

    for(int s = 0; s < mc_steps; ++s) {
        #ifdef DEBUG
            printf("step %d\n", s);
        #endif
        for(int a = 0; a < engine->num_of_atoms; ++a) {
           if(engine->freezen[a]) continue;
           old_energy = get_lj_energy(a, engine);
           #ifdef DEBUG
            printf("old_energy[%d]: %f\n", a, old_energy);
           #endif
           tx = ((double)rand() / RAND_MAX * 2.0 - 1.0) * engine->max_translation;
           ty = ((double)rand() / RAND_MAX * 2.0 - 1.0) * engine->max_translation;
           tz = ((double)rand() / RAND_MAX * 2.0 - 1.0) * engine->max_translation;
           adress = (a << 1) + a;
           #ifdef DEBUG
              printf("%f %f %f\n", tx, ty, tz);
           #endif
           engine->positions[adress] += tx;
           engine->positions[adress + 1] += ty;
           engine->positions[adress + 2] += tz;
           new_energy = get_lj_energy(a, engine);
           #ifdef DEBUG
            printf("new_energy[%d]: %f\n", a, new_energy);
           #endif
           if(new_energy > old_energy) {
               #ifdef DEBUG
                  printf("Not accepted\n");
               #endif
               engine->positions[adress] -= tx;
               engine->positions[adress + 1] -= ty;
               engine->positions[adress + 2] -= tz;
               engine->energies[a] = old_energy;
           }
           #ifdef DEBUG
               else {
                     printf("Accepted\n");
               }
            #endif
        }
    }
}

void run(mc_engine *engine, double temperature, int mc_steps) {
    double tx, ty, tz;
    double old_energy, new_energy, pr;
    int adress;

    for(int s = 0; s < mc_steps; ++s) {
        #ifdef DEBUG
            printf("step %d\n", s);
        #endif
        for(int a = 0; a < engine->num_of_atoms; ++a) {
           if(engine->freezen[a]) continue;
           old_energy = get_energy(a, engine);
           #ifdef DEBUG
            printf("old_energy: %f\n", old_energy);
           #endif
           tx = ((double)rand() / RAND_MAX * 2.0 - 1.0) * engine->max_translation;
           ty = ((double)rand() / RAND_MAX * 2.0 - 1.0) * engine->max_translation;
           tz = ((double)rand() / RAND_MAX * 2.0 - 1.0) * engine->max_translation;
           adress = (a << 1) + a;
           engine->positions[adress] += tx;
           engine->positions[adress + 1] += ty;
           engine->positions[adress + 2] += tz;
           new_energy = get_energy(a, engine);
           #ifdef DEBUG
            printf("new_energy: %f\n", old_energy);
           #endif
           if(new_energy > old_energy) {
               pr = exp(-(new_energy - old_energy)/(kB * temperature));
               if(((double)rand()/RAND_MAX) > pr) {
                   #ifdef DEBUG
                     printf("Not accepted\n");
                   #endif
                   engine->positions[adress] -= tx;
                   engine->positions[adress + 1] -= ty;
                   engine->positions[adress + 2] -= tz;
                   engine->energies[a] = old_energy;
               }
               #ifdef DEBUG
               else {
                     printf("Accepted\n");
               }
               #endif
           }
        }
    }
}

void run_only_gupta(mc_engine *engine, double temperature, int mc_steps) {
    srand(time(NULL));
    double tx, ty, tz;
    double old_energy, new_energy, pr;
    int adress;

    for(int s = 0; s < mc_steps; ++s) {
        #ifdef DEBUG
            printf("step %d\n", s);
        #endif
        for(int a = 0; a < engine->num_of_atoms; ++a) {
           if(engine->freezen[a]) continue;
           old_energy = get_gupta_energy(a, engine);
           #ifdef DEBUG
            printf("old_energy: %f\n", old_energy);
           #endif
           tx = ((double)rand() / RAND_MAX * 2.0 - 1.0) * engine->max_translation;
           ty = ((double)rand() / RAND_MAX * 2.0 - 1.0) * engine->max_translation;
           tz = ((double)rand() / RAND_MAX * 2.0 - 1.0) * engine->max_translation;
           adress = (a << 1) + a;
           engine->positions[adress] += tx;
           engine->positions[adress + 1] += ty;
           engine->positions[adress + 2] += tz;
           new_energy = get_gupta_energy(a, engine);
           #ifdef DEBUG
            printf("new_energy: %f\n", old_energy);
           #endif
           if(new_energy > old_energy) {
               pr = exp(-(new_energy - old_energy)/(kB * temperature));
               if(((double)rand()/RAND_MAX) > pr) {
                   #ifdef DEBUG
                     printf("Not accepted\n");
                   #endif
                   engine->positions[adress] -= tx;
                   engine->positions[adress + 1] -= ty;
                   engine->positions[adress + 2] -= tz;
                   engine->energies[a] = old_energy;
               }
               #ifdef DEBUG
               else {
                     printf("Accepted\n");
               }
               #endif
           }
        }
    }
}

void run_only_lj(mc_engine *engine, double temperature, int mc_steps) {
    srand(time(NULL));
    double tx, ty, tz;
    double old_energy, new_energy, pr;
    int adress;

    for(int s = 0; s < mc_steps; ++s) {
        #ifdef DEBUG
            printf("step %d\n", s);
        #endif
        for(int a = 0; a < engine->num_of_atoms; ++a) {
           if(engine->freezen[a]) continue;
           old_energy = get_lj_energy(a, engine);
           #ifdef DEBUG
            printf("old_energy: %f\n", old_energy);
           #endif
           tx = ((double)rand() / RAND_MAX * 2.0 - 1.0) * engine->max_translation;
           ty = ((double)rand() / RAND_MAX * 2.0 - 1.0) * engine->max_translation;
           tz = ((double)rand() / RAND_MAX * 2.0 - 1.0) * engine->max_translation;
           adress = (a << 1) + a;
           engine->positions[adress] += tx;
           engine->positions[adress + 1] += ty;
           engine->positions[adress + 2] += tz;
           new_energy = get_lj_energy(a, engine);
           #ifdef DEBUG
            printf("new_energy: %f\n", old_energy);
           #endif
           if(new_energy > old_energy) {
               pr = exp(-(new_energy - old_energy)/(kB * temperature));
               if(((double)rand()/RAND_MAX) > pr) {
                   #ifdef DEBUG
                     printf("Not accepted\n");
                   #endif
                   engine->positions[adress] -= tx;
                   engine->positions[adress + 1] -= ty;
                   engine->positions[adress + 2] -= tz;
                   engine->energies[a] = old_energy;
               }
               #ifdef DEBUG
               else {
                     printf("Accepted\n");
               }
               #endif
           }
        }
    }
}

void print_info(mc_engine *engine) {
    double A, p, r0, q, B, rcut;
    double eps, sigma;
    double *gp = engine->gupta_parameters;
    double *ljp = engine->lj_parameters;
    int gupta_adress;
    int lj_adress;
    int num_of_types = engine->num_of_types;
    
    printf("Monte-Carlo engine (v. 1.0 alpha) (C) Sokolov Denis 2019\n");
    printf("Information about calculating system\n");
    printf("Number of atoms: %d\n", engine->num_of_atoms);
    printf("Number of components: %d\n", num_of_types);
    printf("Max translation: %f\n", engine->max_translation);
    printf("Number of processes (not using yet): %d\n", engine->num_proc);

    printf("\n");
    printf("Informations about potentials\n");
    printf("Gupta potential\n");

    for(int i = 0; i < num_of_types; ++i) {
        for(int j = i; j < num_of_types; ++j) {
            printf("%d --- %d: ", i, j);
            gupta_adress = engine->gupta_adress_matrix[i * num_of_types + j];
            A = gp[gupta_adress];
            p = gp[gupta_adress + 1];
            r0 = gp[gupta_adress + 2];
            q = gp[gupta_adress + 3];
            B = gp[gupta_adress + 4];
            rcut = gp[gupta_adress + 5];
            printf("A = %f p = %f r0 = %f q = %f B = %f rcut = %f\n", A, p, r0, q, B, rcut);
        }
    }

    printf("\n");
    printf("Lennard-Jones potential\n");

    for(int i = 0; i < num_of_types; ++i) {
        for(int j = i; j < num_of_types; ++j) {
            printf("%d --- %d: ", i, j);
            lj_adress = engine->lj_adress_matrix[i * num_of_types + j];
            eps = ljp[lj_adress];
            sigma = ljp[lj_adress + 1];
            rcut = ljp[lj_adress + 2];
            printf("epsilon = %f sigma = %f rcut = %f\n", eps, sigma, rcut);
        }
    }
    printf("\n");
}




