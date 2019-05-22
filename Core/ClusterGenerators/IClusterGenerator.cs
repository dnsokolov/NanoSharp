/*
   Copyright (c) 2019 Sokolov Denis
   IClusterGenerator interface for generating cluster
 */

 namespace NanoSharp.Core.ClusterGenerators {
     public interface IClusterGenerator {
         Cluster Generate();
     }
 }