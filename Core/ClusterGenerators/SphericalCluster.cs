/*
   Copyright (c) 2019 Sokolov Denis
   SphericalGenerator class for generating spherical cluster
 */
 using System;

 namespace NanoSharp.Core.ClusterGenerators {
     public class SphericalCluster: IClusterGenerator {
         Lattice lattice;
         Single radiusInternal;
         Single radiusExternal;

         public Int32 M {private set; get;} = 20;


         public SphericalCluster(Single radiusInternal, Single radiusExternal, Lattice lattice) {
             this.lattice = lattice;
             this.radiusInternal = radiusInternal;
             this.radiusExternal = radiusExternal;
         }

         public Cluster Generate() {
             Cluster cluster = new Cluster();

             for(Int32 i = -M ; i <= M; ++i) {
                 for(Int32 j = -M; j <= M; ++j) {
                     for(Int32 k = -M; k <= M; ++k) {
                         var atoms = lattice.GetAtoms(i, j, k);
                         foreach(var a in atoms) {
                             if(a.Position.Length() >= radiusInternal && a.Position.Length() <= radiusExternal)
                               cluster.Add(a);
                         }
                     }
                 }
             }
             return cluster;
         }
     } 
 }