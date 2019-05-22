/*
   Copyright (c) 2019 Sokolov Denis
   IClusterFormat interface for generating string represenation of Cluster's
 */
using System;

 namespace NanoSharp.Core.ClusterFormats {
     public interface IClusterFormat {
         String GetStringRepresentation(Cluster cluster);
     }
 }