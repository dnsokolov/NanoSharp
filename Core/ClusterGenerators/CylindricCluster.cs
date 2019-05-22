/*
   Copyright (c) 2019 Sokolov Denis
   CylindricCluster class for generating cylindric clusters
 */
 using System;
 using System.Numerics;

 namespace NanoSharp.Core.ClusterGenerators {
     public class CylindricCluster : IClusterGenerator {
         Single R;
         Single h;
         Lattice lattice;

         public Int32 M {set;get;} = 20;

         public CylindricCluster(Single radius, Single height, Lattice lattice) {
             this.R = radius;
             this.h = 0.5f * height;
             this.lattice = lattice;
         }

         public Cluster Generate() {
             Cluster cluster = new Cluster();

             for(Int32 i = -M; i <= M; ++i) {
                 for(Int32 j = -M; j <= M; ++j) {
                     for(Int32 k = -M; k <= M; ++k) {
                         var atoms = lattice.GetAtoms(i, j, k);
                         foreach(var atom in atoms) {
                                Vector3 dist = new Vector3(atom.Position.X, atom.Position.Y, 0);
                                if(dist.Length() <= R && atom.Position.Z >= -h && atom.Position.Z <=h)
                                cluster.Add(atom);
                         }
                     }
                 }
             }

             return cluster;
 
         }
     }
 }