/*
   Copyright (c) 2019 Sokolov Denis
   MonteCarloEngine class wraper for native mc_engine
 */
 using System;
 using System.Collections.Generic;
 using System.Numerics;
 using System.Runtime.InteropServices;
 using System.Globalization;
 using NanoSharp.Native.MCEngine;

 namespace NanoSharp.Core {
     public class MonteCarloEngine {
         Cluster cluster;
         mc_engine engine;
         double[] positions;
         int[] types;
         int[] freezen;
         double[] energies;

         int[] gupta_adress_matrix;
         int[] lj_adress_matrix;
         int[] gupta_matrix;
         int[] lj_matrix;

         double[] gupta_parameters;
         double[] lj_parameters;

         public readonly int NumGuptaParams = 6;
         public readonly int NumLJParams = 3;

         public readonly int NumPotentialTypes = 2;
         private bool[] potential_flags; // 0 - gupta, 1 - lj 

         public MonteCarloEngine(Cluster cluster, Double max_translation = 1.0) {
             potential_flags = new bool[NumPotentialTypes];
             for(int i = 0; i < potential_flags.Length; ++i) potential_flags[i] = false;

             this.cluster = cluster;
             engine.max_translation = max_translation;

             engine.num_of_atoms = cluster.Atoms.Count;
             engine.num_of_types = cluster.ChemicalComponents.Count;
             positions = new double[engine.num_of_atoms * 3];
             types = new int[engine.num_of_atoms];
             freezen = new int[engine.num_of_atoms];
             energies = new double[engine.num_of_atoms];
             int compsize = engine.num_of_types * engine.num_of_types;

             gupta_adress_matrix = new int[compsize];
             lj_adress_matrix = new int[compsize];

             gupta_matrix = new int[compsize];
             lj_matrix = new int[compsize];

             for(int i =0; i < compsize; ++i) {
                 gupta_matrix[i] = 0;
                 lj_matrix[i] = 0;
             }

             gupta_parameters = new double[(compsize + engine.num_of_types) * 3];
             for(int i = 0; i < gupta_parameters.Length; ++i) gupta_parameters[i] = 0.0;

             lj_parameters = new double[3 * (compsize + engine.num_of_types) / 2];
             for(int i = 0; i < lj_parameters.Length; ++i) lj_parameters[i] = 0.0;

             int adress;

             for(int i = 0; i < engine.num_of_atoms; ++i) {
                 adress = (i << 1) + i;
                 positions[adress] = cluster.Atoms[i].Position.X;
                 positions[adress + 1] = cluster.Atoms[i].Position.Y;
                 positions[adress + 2] = cluster.Atoms[i].Position.Z;
                 types[i] = cluster.ChemicalComponents.IndexOf(cluster.Atoms[i].ChemicalName);
                 energies[i] = cluster.Atoms[i].Energy;
                 freezen[i] = cluster.Atoms[i].Freezen;
             }

             int size = Marshal.SizeOf(positions[0]) * positions.Length;
             engine.positions = Marshal.AllocHGlobal(size);
             Marshal.Copy(positions, 0, engine.positions, positions.Length);

             size = Marshal.SizeOf(types[0]) * types.Length;
             engine.types = Marshal.AllocHGlobal(size);
             Marshal.Copy(types, 0, engine.types, types.Length);

             size = Marshal.SizeOf(freezen[0]) * freezen.Length;
             engine.freezen = Marshal.AllocHGlobal(size);
             Marshal.Copy(freezen, 0, engine.freezen, freezen.Length);

             size = Marshal.SizeOf(energies[0]) * energies.Length;
             engine.energies = Marshal.AllocHGlobal(size);
             Marshal.Copy(energies, 0, engine.energies, energies.Length);

             int last_gupta_num = 0;
             int last_lj_num = 0;

             for(int i = 0; i < engine.num_of_types; ++i ) {
                 for(int j = 0; j < engine.num_of_types; ++j) {
                     if(i <= j) {
                         gupta_adress_matrix[i * engine.num_of_types + j] = last_gupta_num;
                         lj_adress_matrix[i * engine.num_of_types + j] = last_lj_num;
                         last_gupta_num += 6;
                         last_lj_num += 3;
                     }
                     else {
                         gupta_adress_matrix[i * engine.num_of_types + j] =  gupta_adress_matrix[j * engine.num_of_types + i];
                         lj_adress_matrix[i * engine.num_of_types + j] = lj_adress_matrix[j * engine.num_of_types + i];
                     }
                 }
             }

             size = Marshal.SizeOf(gupta_adress_matrix[0]) * gupta_adress_matrix.Length;
             engine.gupta_adress_matrix = Marshal.AllocHGlobal(size);
             Marshal.Copy(gupta_adress_matrix, 0, engine.gupta_adress_matrix, gupta_adress_matrix.Length);

             size = Marshal.SizeOf(lj_adress_matrix[0]) * lj_adress_matrix.Length;
             engine.lj_adress_matrix = Marshal.AllocHGlobal(size);
             Marshal.Copy(lj_adress_matrix, 0, engine.lj_adress_matrix, lj_adress_matrix.Length);

             engine.num_proc = 1;
         }

         private int GetNumPotentialTypesInTheSystem() {
             int counter = 0;
             foreach(var flag in potential_flags) if(flag) ++counter;
             return counter;
         }

         public void Init(bool printinfo = true) {
             int size = Marshal.SizeOf(gupta_adress_matrix[0]) * gupta_adress_matrix.Length;
             engine.gupta_adress_matrix = Marshal.AllocHGlobal(size);
             Marshal.Copy(gupta_adress_matrix, 0, engine.gupta_adress_matrix, gupta_adress_matrix.Length);

             size = Marshal.SizeOf(lj_adress_matrix[0]) * lj_adress_matrix.Length;
             engine.lj_adress_matrix = Marshal.AllocHGlobal(size);
             Marshal.Copy(lj_adress_matrix, 0, engine.lj_adress_matrix, lj_adress_matrix.Length);

             size = Marshal.SizeOf(gupta_matrix[0]) * gupta_matrix.Length;
             engine.gupta_matrix = Marshal.AllocHGlobal(size);
             Marshal.Copy(gupta_matrix, 0, engine.gupta_matrix, gupta_matrix.Length);

             size = Marshal.SizeOf(lj_matrix[0]) * lj_matrix.Length;
             engine.lj_matrix = Marshal.AllocHGlobal(size);
             Marshal.Copy(lj_matrix, 0, engine.lj_matrix, lj_matrix.Length);

             size = Marshal.SizeOf(lj_parameters[0]) * lj_parameters.Length;
             engine.lj_parameters = Marshal.AllocHGlobal(size);
             Marshal.Copy(lj_parameters, 0, engine.lj_parameters, lj_parameters.Length);

             size = Marshal.SizeOf(gupta_parameters[0]) * gupta_parameters.Length;
             engine.gupta_parameters = Marshal.AllocHGlobal(size);
             Marshal.Copy(gupta_parameters, 0, engine.gupta_parameters, gupta_parameters.Length);

             if(printinfo) print_info(ref engine);
         }

         public void AddGuptaParams(string line) {
             String[] splLine = line.Split("<,>: \t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
             String comp1 = splLine[0];
             String comp2 = splLine[1];
             Double r0 = Convert.ToDouble(splLine[2], CultureInfo.InvariantCulture);
             Double A = Convert.ToDouble(splLine[3], CultureInfo.InvariantCulture);
             Double p = Convert.ToDouble(splLine[4], CultureInfo.InvariantCulture);
             Double B = Convert.ToDouble(splLine[5], CultureInfo.InvariantCulture);
             Double q = Convert.ToDouble(splLine[6], CultureInfo.InvariantCulture);
             Double rcut = Convert.ToDouble(splLine[7], CultureInfo.InvariantCulture);
             AddGuptaParams(comp1, comp2, r0, A, p, B, q, rcut);
         }

         public void AddGuptaParams(string comp1, string comp2, double r0, double A, double p, double B, double q, double rcut) {
             int i, j;
             if((i = cluster.ChemicalComponents.IndexOf(comp1)) == -1 || 
                (j = cluster.ChemicalComponents.IndexOf(comp2)) == -1)
                throw new Exception($"Invalid components: '{comp1}' or '{comp2}'");

            potential_flags[0] = true;
            
             if(i == j) gupta_matrix[i * engine.num_of_types + j] = 1;
             else {
                 gupta_matrix[i * engine.num_of_types + j] = 1;
                 gupta_matrix[j * engine.num_of_types + i] = 1;
             }

             gupta_parameters[gupta_adress_matrix[i * engine.num_of_types + j]] = A;
             gupta_parameters[gupta_adress_matrix[i * engine.num_of_types + j] + 1] = p;
             gupta_parameters[gupta_adress_matrix[i * engine.num_of_types + j] + 2] = r0;
             gupta_parameters[gupta_adress_matrix[i * engine.num_of_types + j] + 3] = q;
             gupta_parameters[gupta_adress_matrix[i * engine.num_of_types + j] + 4] = B;
             gupta_parameters[gupta_adress_matrix[i * engine.num_of_types + j] + 5] = rcut;
             
         }

          public void AddLJParams(string line) {
             String[] splLine = line.Split("<,>: \t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
             String comp1 = splLine[0];
             String comp2 = splLine[1];
             Double epsilon = Convert.ToDouble(splLine[2], CultureInfo.InvariantCulture);
             Double sigma = Convert.ToDouble(splLine[3], CultureInfo.InvariantCulture);
             Double rcut = Convert.ToDouble(splLine[4], CultureInfo.InvariantCulture);
             AddLJParams(comp1, comp2, epsilon, sigma, rcut);
         }

         public void AddLJParams(string comp1, string comp2, double epsilon, double sigma, double rcut) {
             int i, j;
             if((i = cluster.ChemicalComponents.IndexOf(comp1)) == -1 || 
                (j = cluster.ChemicalComponents.IndexOf(comp2)) == -1)
                throw new Exception($"Invalid components: '{comp1}' or '{comp2}'");

             potential_flags[1] = true;
            
             if(i == j) lj_matrix[i * engine.num_of_types + j] = 1;
             else {
                 lj_matrix[i * engine.num_of_types + j] = 1;
                 lj_matrix[j * engine.num_of_types + i] = 1;
             }

             lj_parameters[gupta_adress_matrix[i * engine.num_of_types + j]] = epsilon;
             lj_parameters[gupta_adress_matrix[i * engine.num_of_types + j] + 1] = sigma;
             lj_parameters[gupta_adress_matrix[i * engine.num_of_types + j] + 2] = rcut;          
         }

         public void Run(int mc_steps) {
             if(GetNumPotentialTypesInTheSystem()>1) 
                run_without_temperature(ref engine, mc_steps);
             else {
                 if(potential_flags[0]) run_without_temperature_only_gupta(ref engine, mc_steps);
                 if(potential_flags[1]) run_without_temperature_only_lj(ref engine, mc_steps);
             }
             CopyToCluster();
         }

         public void Run(int mc_steps, double temperature) {
             if(GetNumPotentialTypesInTheSystem()>1) {
                 if(Tolerance(temperature, 0)) run_without_temperature(ref engine, mc_steps);
                 else run(ref engine, temperature, mc_steps);
             }
             else {
                 if(Tolerance(temperature,0)) {
                     if(potential_flags[0]) run_without_temperature_only_gupta(ref engine, mc_steps);
                     if(potential_flags[1]) run_without_temperature_only_lj(ref engine, mc_steps);
                 }
                 else {
                     if(potential_flags[0]) run_only_gupta(ref engine, temperature, mc_steps);
                     if(potential_flags[1]) run_only_lj(ref engine, temperature, mc_steps);
                 }
             }
             CopyToCluster();
         }

         public void PrintInfo() {
             print_info(ref engine);
         }
         

         private void CopyToCluster() {
             Marshal.Copy(engine.energies, energies, 0, energies.Length);
             Marshal.Copy(engine.positions, positions, 0, positions.Length);
             for(int i = 0; i < cluster.Atoms.Count; ++i) {
                 cluster.Atoms[i].Energy = energies[i];
                 int adress = (i << 1) + i;
                 cluster.Atoms[i].Position.X = (float)(positions[adress]);
                 cluster.Atoms[i].Position.Y = (float)(positions[adress + 1]);
                 cluster.Atoms[i].Position.Z = (float)(positions[adress + 2]) ;
             }
         }
          
         #if Windows
         [DllImport("mcengine.dll")]
         private static extern void run_without_temperature(ref mc_engine engine, int mc_steps);

         [DllImport("mcengine.dll")]
         private static extern void run_without_temperature_only_gupta(ref mc_engine engine, int mc_steps);

         [DllImport("mcengine.dll")]
         private static extern void run_without_temperature_only_lj(ref mc_engine engine, int mc_steps);

         [DllImport("mcengine.dll")]
         private static extern void run(ref mc_engine engine, double temperature, int mc_steps);

         [DllImport("mcengine.dll")]
         private static extern void run_only_gupta(ref mc_engine engine, double temperature, int mc_steps);

         [DllImport("mcengine.dll")]
         private static extern void run_only_lj(ref mc_engine engine, double temperature, int mc_steps);

         [DllImport("mcengine.dll")]
         private static extern void print_info(ref mc_engine engine);
         #else
         [DllImport("mcengine.so")]
         private static extern void run_without_temperature(ref mc_engine engine, int mc_steps);

         [DllImport("mcengine.so")]
         private static extern void run_without_temperature_only_gupta(ref mc_engine engine, int mc_steps);

         [DllImport("mcengine.so")]
         private static extern void run_without_temperature_only_lj(ref mc_engine engine, int mc_steps);

         [DllImport("mcengine.so")]
         private static extern void run(ref mc_engine engine, double temperature, int mc_steps);

         [DllImport("mcengine.so")]
         private static extern void run_only_gupta(ref mc_engine engine, double temperature, int mc_steps);

         [DllImport("mcengine.so")]
         private static extern void run_only_lj(ref mc_engine engine, double temperature, int mc_steps);

         [DllImport("mcengine.so")]
         private static extern void print_info(ref mc_engine engine);
         #endif

        public static bool Tolerance(double x, double y, double eps = 0.000001) {
                return Math.Abs(x - y) < eps;
        }


     }
 }