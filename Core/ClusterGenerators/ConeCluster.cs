/*
   Copyright (c) 2019 Sokolov Denis
   ConeCluster class for generating cone clusters
 */
using System;
using System.Numerics;

 namespace NanoSharp.Core.ClusterGenerators {
     public class ConeCluster : IClusterGenerator {
         Single R;
         Single h;
         Lattice lattice;

         public Int32 M {set;get;} = 20;

         public ConeCluster(Single radius, Single height, Lattice lattice) {
             this.R = radius;
             this.h = height;
             this.lattice = lattice;
         }

         public Cluster Generate() {
             Cluster cluster = new Cluster();

             for(Int32 i = -M; i <= M; ++i) {
                 for(Int32 j = -M; j <= M; ++j) {
                     for(Int32 k = -M; k <= M; ++k) {
                         var atoms = lattice.GetAtoms(i, j, k);
                         foreach(var atom in atoms) {
                               Single r = R - atom.Position.Z * R / h;
                               Vector3 dist = new Vector3(atom.Position.X, atom.Position.Y, 0);
                               if(dist.Length() <= r && atom.Position.Z >= 0f && atom.Position.Z <= h) {
                                   cluster.Add(atom);
                               }
                         }
                     }
                 }
             }

             return cluster;
 
         }
     }
 }