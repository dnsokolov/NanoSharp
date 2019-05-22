/*
   Copyright (c) 2019 Sokolov Denis
   SIRLoader class for loading atoms from sir files
 */
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Globalization;

namespace NanoSharp.Core.ClusterReaders {
    public class SIRReader: IClusterReader {

        public void Read(String content, Cluster cluster) {
            cluster.Clear();
            StringReader sr = new StringReader(content);

            String line;
            String chemicalName;
            String color;
            Single x;
            Single y;
            Single z;
            Single radius;

            while((line = sr.ReadLine())!=null) {
                if(line.ToLower().Contains("sphere")) {
                    String[] splLine = line.Split(" \t".ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                    chemicalName = splLine[1];
                    x = Convert.ToSingle(splLine[2], CultureInfo.InvariantCulture);
                    y = Convert.ToSingle(splLine[3], CultureInfo.InvariantCulture);
                    z = Convert.ToSingle(splLine[4], CultureInfo.InvariantCulture);
                    color = splLine[5];
                    radius = Convert.ToSingle(splLine[6], CultureInfo.InvariantCulture);
                    Atom atom = new Atom(x, y, z, chemicalName);
                    atom["Color"] = color;
                    atom["Radius"] = radius;
                    cluster.Atoms.Add(atom);
                    if(!cluster.ChemicalComponents.Contains(chemicalName)) {
                        cluster.ChemicalComponents.Add(chemicalName);
                    }
                }
            }

            sr.Close(); sr.Dispose();
        }
    }
}

