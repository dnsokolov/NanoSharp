/*
   Copyright (c) 2019 Sokolov Denis
   Lattice class for description of atomic lattice
 */
 using System;
 using System.Numerics;
 using System.Collections.Generic;

 namespace NanoSharp.Core.ClusterGenerators {
     public class Lattice {
         public Vector3 a;
         public Vector3 b;
         public Vector3 c;

         public List<Atom> basis;

         public Lattice(Vector3 a, Vector3 b, Vector3 c) {
             this.a = a;
             this.b = b;
             this.c = c;
             basis = new List<Atom>();
         }

         public Lattice(Single a, Single b, Single c) {
             this.a = a * Vector3.UnitX;
             this.b = b * Vector3.UnitY;
             this.c = c * Vector3.UnitZ;
             basis = new List<Atom>();
         }

         public void AddBasisAtom(Atom atom) {
             atom.Position = atom.Position.X * a + atom.Position.Y * b + atom.Position.Z * c;
             basis.Add(atom);
         }

         public List<Atom> GetAtoms(Int32 m, Int32 n, Int32 k) {
             List<Atom> atoms = new List<Atom>();
             foreach(Atom atom in basis) {
                 Atom atm = new Atom(atom.Position + m * a + 
                                                     n * b + 
                                                     k * c, 
                                   atom.ChemicalName);
                 atoms.Add(atm);
             }
             return atoms;
         }

         public static Lattice SC(Single a) {
            return new Lattice(a, a, a);
         }

         public static Lattice FCC(Single a) {
             return new Lattice(0.5f * a * (Vector3.UnitY + Vector3.UnitZ),
                                0.5f * a * (Vector3.UnitZ + Vector3.UnitX),
                                0.5f * a * (Vector3.UnitX + Vector3.UnitY));
         }

         public static Lattice HCP(Single a) {
             return new Lattice(0.5f * a * (Vector3.UnitY + Vector3.UnitZ - Vector3.UnitX),
                                0.5f * a * (Vector3.UnitZ + Vector3.UnitX - Vector3.UnitY),
                                0.5f * a * (Vector3.UnitX + Vector3.UnitY - Vector3.UnitZ));
         }
     }
 }