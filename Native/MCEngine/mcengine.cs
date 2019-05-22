/*
   Copyright (c) 2019 Sokolov Denis
   C# Wraper for native mc_engine
 */
 using System;
 using System.Runtime.InteropServices;

 namespace NanoSharp.Native.MCEngine {
     public struct mc_engine {
         public double max_translation;

         public int num_of_atoms;
         public int num_of_types;
         public IntPtr positions;
         public IntPtr types;
         public IntPtr freezen;
         public IntPtr energies;

         public IntPtr gupta_adress_matrix;
         public IntPtr lj_adress_matrix;

         public IntPtr gupta_matrix;
         public IntPtr gupta_parameters;

         public IntPtr lj_matrix;
         public IntPtr lj_parameters;

         public int num_proc;
     }
 }