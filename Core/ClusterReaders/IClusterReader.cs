/*
   Copyright (c) 2019 Sokolov Denis
   IFileLoader interface for loading atoms from files
 */
 using System;
 using System.Text;

 namespace NanoSharp.Core.ClusterReaders {
     public interface IClusterReader {
         void Read(String content, Cluster cluster);
     }
 }