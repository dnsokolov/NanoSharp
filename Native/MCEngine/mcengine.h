/*
   Copyright (c) 2019 Sokolov Denis
   Native mc_engine struct for calculating process in Cluster
 */
#ifndef MCENGINE_H
#define MCENGINE_H
#include <stdlib.h>
#include <stdio.h>
#include <time.h>
#include <math.h>

#define NUM_GUPTA_PARAMS 6
#define NUM_LJ_PARAMS 3
#define kB 8.6173303E-5

#ifdef _WIN32
#define NANOSHARP __declspec(dllexport)
#else
#define NANOSHARP
#endif


typedef struct _mc_engine {

    double max_translation;    //max translation in x, y, z directions         

    int num_of_atoms;          //num of atoms in the system
    int num_of_types;          //num of types of atoms
    double *positions;         //positions of atoms (size 3 * num_of_atoms)
    int *types;                //types of atoms(size num_of_atoms)
    int *freezen;              //freezen atoms
    double *energies;          //energies of atoms

    int *gupta_adress_matrix;
    int *lj_adress_matrix;

    int *gupta_matrix;         //matrix describes gupta bonds between types of atoms (size num_of_types * num_of_types)
    double *gupta_parameters;  //gupta parameters potential (0 - A, 1 - p, 2 - r0, 3 - q, 4 - B, 5 - rcut)

    int *lj_matrix;            //matrix describes lj bonds between types of atoms (size num_of_types * num_of_types)
    double *lj_parameters;     //lennard-jones parameters potential (0 - epsilon, 1 - sigma, 2 - rcut)

    int num_proc;              //number of parallel processes (not using yet)
} mc_engine;


//get distance between atom i and j in mc_engine
static inline double get_dist(int i, int j, mc_engine *engine) {
    int istart = (i << 1) + i; //equalent 2i + i = 3i
    int jstart = (j << 1) + j; //equalent 2j + j = 3j
    double *pos = engine->positions;

    double dx = pos[istart] - pos[jstart];
    double dy = pos[istart + 1] - pos[jstart + 1];
    double dz = pos[istart + 2] - pos[jstart + 2];

    return sqrt(dx * dx + dy * dy + dz * dz);
}

//get energy atom a in mc_engine
static inline double get_gupta_energy(int a, mc_engine *engine) {
    double repulsive = 0.0;
    double attractive = 0.0;
    double dist, d, rcut;
    int a_type = engine->types[a];
    int i_type;
    int num_of_types = engine->num_of_types;
    int adress;
    int gupta_adress;
    double *gp = engine->gupta_parameters;

    for(int i = 0; i < engine->num_of_atoms; ++i) {
        if(i == a) continue;
        i_type = engine->types[i];
        adress = i_type * num_of_types + a_type;  
        if(engine->gupta_matrix[adress] == 0) continue;
        gupta_adress = engine->gupta_adress_matrix[adress];
        dist = get_dist(i, a, engine);
        rcut = gp[gupta_adress + 5];
        if((rcut > 0.0 && dist <= rcut) || rcut <= 0.0) {
            d = dist/gp[gupta_adress + 2] - 1.0;
            repulsive += gp[gupta_adress] * exp(-gp[gupta_adress + 1] * d );
            attractive += gp[gupta_adress + 4] * gp[gupta_adress + 4] * exp(-2.0 * gp[gupta_adress + 3] * d);
        }
    }

    engine->energies[a] = repulsive - sqrt(attractive);
    return engine->energies[a];
}

static inline double get_lj_energy(int a, mc_engine *engine) {
    double e = 0.0;
    double dist, d, rcut;
    int a_type = engine->types[a];
    int i_type;
    int num_of_types = engine->num_of_types;
    int adress;
    int lj_adress;
    double *ljp = engine->lj_parameters;

    for(int i = 0; i < engine->num_of_atoms; ++i) {
        if(i == a) continue;
        i_type = engine->types[i];
        adress = i_type * num_of_types + a_type;
        if(engine->lj_matrix[adress] == 0) continue;
        lj_adress = engine->lj_adress_matrix[adress];
        dist = get_dist(i, a, engine);
        rcut = ljp[ lj_adress + 3];
        if((rcut > 0.0 && dist <= rcut) || rcut <= 0.0) {
            d = ljp[lj_adress + 1] / dist;
            e += ljp[lj_adress] * (pow(d, 12) - pow(d,6));
        }
    }

    engine->energies[a] = e;
    return engine->energies[a];
}

static inline double get_energy(int a, mc_engine *engine) {
    double E = 0.0;
    if(engine->lj_matrix != NULL) E += get_lj_energy(a, engine);
    if(engine->gupta_matrix != NULL) E += get_gupta_energy(a, engine);
    return E;
}

NANOSHARP void run_without_temperature(mc_engine *, int);
NANOSHARP void run_without_temperature_only_gupta(mc_engine *, int);
NANOSHARP void run_without_temperature_only_lj(mc_engine *, int);
NANOSHARP void run(mc_engine *, double, int);
NANOSHARP void run_only_gupta(mc_engine *, double, int);
NANOSHARP void run_only_lj(mc_engine *, double, int);
NANOSHARP void print_info(mc_engine *);

#endif