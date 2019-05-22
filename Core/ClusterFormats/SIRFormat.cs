/*
   Copyright (c) 2019 Sokolov Denis
   SIRFormat class for generating string represenation of Cluster's in sir format
 */

 using System;
 using System.Text;
 using System.IO;
 using System.Globalization;
 using NanoSharp.Core;
 
 namespace NanoSharp.Core.ClusterFormats {
     public class SIRFormat: IClusterFormat {
         public String GetStringRepresentation(Cluster cluster) {
             StringBuilder sb = new StringBuilder();
             StringWriter sw = new StringWriter(sb);

             sw.WriteLine(@"Begin image.ent
Rotate 0.000000 0.000000 0.000000
Scale 1.0");

             foreach(Atom atom in cluster.Atoms) {
                 String color = atom.ContainsProperty("Color") ? atom["Color"] : "0xFBFBFB";
                 Single radius = atom.ContainsProperty("Radius") ? atom["Radius"] : 1.0f;
                 sw.WriteLine(" Sphere " + atom.ChemicalName 
                               + " " + atom.Position.X.ToString(CultureInfo.InvariantCulture) 
                               + " " + atom.Position.Y.ToString(CultureInfo.InvariantCulture) 
                               + " " + atom.Position.Z.ToString(CultureInfo.InvariantCulture) 
                               + " " + color + " " 
                               + radius.ToString(CultureInfo.InvariantCulture));
             }

             sw.WriteLine("End");
             sw.Close(); sw.Dispose();

             return sb.ToString();
         }
     }
 }
