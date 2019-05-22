/*
   Copyright (c) 2019 Sokolov Denis
   XYZLoader class for loading atoms from xyz files
 */
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Globalization;

 namespace NanoSharp.Core.ClusterReaders {
     public class XYZReader : IClusterReader {
         Int32 readNumFrame; //read frame number readNumFrame (if < 0 then numeration start with end)
         List<string> frameContent;
         String[] columns;

         public XYZReader(int readNumFrame = 0) {
             this.readNumFrame = readNumFrame;
             frameContent = new List<string>();
             columns = null;
         }

         public Int32 NumFrames {private set; get;} = 0;

         private void ParseComment(string comment, Cluster cluster) {
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
                         continue;
                     }
                     cluster[key] = keyValue[1];
                 }
             }
         }

         private void ReadContent(String content, Cluster cluster) {
             cluster.Clear();
             StringReader sr = new StringReader(content);
             sr.ReadLine();
             cluster["Comment"] = sr.ReadLine();
             ParseComment(cluster["Comment"], cluster);

             String line;
             String chemicalName;
             Single x;
             Single y;
             Single z;

             while((line = sr.ReadLine())!=null) {
                 String[] splLine = line.Split(" \t".ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                 if(splLine.Length < 4) break; 
                 chemicalName = splLine[0];
                 x = Convert.ToSingle(splLine[1], CultureInfo.InvariantCulture);
                 y = Convert.ToSingle(splLine[2], CultureInfo.InvariantCulture);
                 z = Convert.ToSingle(splLine[3], CultureInfo.InvariantCulture);
                 Atom atom = new Atom( x, y, z, chemicalName);
                 
                 if(columns != null) {
                    try {
                       for(int i = 0; i < columns.Length; ++i) {
                           if(columns[i] == "Energy") {
                               atom.Energy = Convert.ToDouble(splLine[i+4], CultureInfo.InvariantCulture);
                               continue;
                           }
                           atom[columns[i]] = splLine[i + 4];
                        }
                    }
                    catch {}
                 }


                 cluster.Atoms.Add(atom);
                 if(!cluster.ChemicalComponents.Contains(chemicalName)) {
                     cluster.ChemicalComponents.Add(chemicalName);
                 }
             }
             sr.Close(); sr.Dispose();
         }

         public void ReadFrame(Int32 numFrame, String content, Cluster cluster) {
             StringReader sr = new StringReader(content);
             String numAtoms = sr.ReadLine();
             StringBuilder sb = new StringBuilder();
             StringWriter sw = new StringWriter(sb);
             String line;
             String[] splLine;
             sw.WriteLine(numAtoms);
             sw.WriteLine(sr.ReadLine());
             frameContent.Clear();

             while((line = sr.ReadLine()) != null) {
                 splLine = line.Split(" \t".ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                 if(splLine.Length == 1 && line == numAtoms) {
                     frameContent.Add(sb.ToString());
                     sb.Clear();
                 }
                 sw.WriteLine(line);
             }

             frameContent.Add(sb.ToString());

             NumFrames = frameContent.Count;
             sr.Close(); sw.Close(); sr.Dispose(); sw.Dispose();

             if(numFrame >= 0) ReadContent(frameContent[numFrame], cluster);
             else ReadContent(frameContent[frameContent.Count + numFrame], cluster);
         }

         public void Read(String content, Cluster cluster) {
             ReadFrame(readNumFrame, content, cluster);
         }
     }
 }