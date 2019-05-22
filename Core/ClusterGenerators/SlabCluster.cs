/*
   Copyright (c) 2019 Sokolov Denis
   SlabCluster class for generating slab clusters
 */
 using System;

 namespace NanoSharp.Core.ClusterGenerators {
     public class SlabCluster: IClusterGenerator {
         Lattice lattice;
         Single a;
         Single b;
         Single c;

         public Int32 M {set; get;} = 20;

         public SlabCluster(Single a, Single b, Single c, Lattice lattice) {
             this.a = 0.5f * a;
             this.b = 0.5f * b;
             this.c = 0.5f * c;
             this.lattice = lattice;
         }

         public Cluster Generate() {
             Cluster cluster = new Cluster();

             for(Int32 i = -M; i <= M; ++i) {
                 for(Int32 j = -M; j <= M; ++j) {
                     for(Int32 k = -M; k <= M; ++k) {
                         var atoms = lattice.GetAtoms(i, j, k);
                         foreach(var atom in atoms) {
                             if(atom.Position.X >= -a && atom.Position.X <= a &&
                                atom.Position.Y >= -b && atom.Position.Y <= b &&
                                atom.Position.Z >= -c && atom.Position.Z <= c)
                                cluster.Add(atom);
                         }
                     }
                 }
             }

             return cluster;
         }
     }
 }