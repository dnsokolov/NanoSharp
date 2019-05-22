/*
   Copyright (c) 2019 Sokolov Denis
   XYZFormat class for generating string represenation of Cluster's in xyz format
 */

 using System;
 using System.Text;
 using System.IO;
 using System.Globalization;
 using NanoSharp.Core;

 namespace NanoSharp.Core.ClusterFormats {
     public class XYZFormat: IClusterFormat {

         String[] columns = null;

         private void ParseComment(string comment) {
             StringBuilder sb = new StringBuilder();
             Boolean flag = false;

             foreach(var c in comment) {
                 if(c == '[') {
                     flag = true;
                     continue;
                 }
                 if(c == ']') {
                     break;
                 }

                 if(flag) sb.Append(c);
             }

             String[] splLine = sb.ToString().Split(" \t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
             String[] keyValue;
             
             foreach(var str in splLine) {
                 keyValue = str.Split(":".ToCharArray(), 2, StringSplitOptions.RemoveEmptyEntries);
                 if(keyValue.Length == 2) {
                     string key = keyValue[0];
                     if(key == "columns"){
                         columns = keyValue[1].Split(",".ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                         break;
                     }
                 }
             }
         }
         public String GetStringRepresentation(Cluster cluster) {
             StringBuilder sb = new StringBuilder();
             StringWriter sw = new StringWriter(sb);

             sw.WriteLine(cluster.Atoms.Count);
             if(cluster.ContainsProperty("Comment")) 
             {
                 sw.WriteLine(cluster["Comment"]);
                 ParseComment(cluster["Comment"]);
             }
             else sw.WriteLine();
             
             foreach(Atom atom in cluster.Atoms) {
                 sw.Write(atom.ChemicalName 
                              + " " + atom.Position.X.ToString(CultureInfo.InvariantCulture)
                              + " " + atom.Position.Y.ToString(CultureInfo.InvariantCulture)
                              + " " + atom.Position.Z.ToString(CultureInfo.InvariantCulture));
                 if(columns != null) {
                     foreach(var c in columns) {
                         if(c == "Energy") {
                             sw.Write(" " + atom.Energy.ToString(CultureInfo.InvariantCulture));
                             continue;
                         }
                         sw.Write(" " + atom[c].ToString(CultureInfo.InvariantCulture));
                     }
                 }
                 sw.WriteLine();
             }

             sw.Close(); sw.Dispose();

             return sb.ToString();
         }
     }
 }