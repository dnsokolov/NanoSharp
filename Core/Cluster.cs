/*
   Copyright (c) 2019 Sokolov Denis
   Cluster class for storaging, manipulating and processing group of atoms 
 */

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Numerics;
using NanoSharp.Core.ClusterReaders;
using NanoSharp.Core.ClusterFormats;

namespace NanoSharp.Core {
    public class Cluster {
        public List<Atom> Atoms;
        public List<String> ChemicalComponents;
        public Dictionary<String, dynamic> Properties;

        public Dictionary<String, IClusterReader> Readers;
        public Dictionary<String, IClusterFormat> Formats;

        public dynamic this[String property] {
            set {
                if(Properties.ContainsKey(property)) Properties[property] = value;
                else Properties.Add(property, value);
            }

            get {
                return Properties[property];
            }
        }

        public Boolean ContainsProperty(String property) {
            return Properties.ContainsKey(property);
        }

        public List<double> GetEnergies(){
            List<double> energies = new List<double>();
            foreach(var a in Atoms) {
                energies.Add(a.Energy);
            }
            return energies;
        }

        public Int32 NumOfAtomsComponent(string component) {
            Int32 counter = 0;
            for(Int32 i = 0; i < Atoms.Count; ++i) 
            if(Atoms[i].ChemicalName == component) ++counter;
            return counter;
        }

        public void Freeze() {
            for(Int32 i = 0; i < Atoms.Count; ++i) Atoms[i].Freezen = 1;
        }

        public void UnFreeze() {
            for(Int32 i = 0; i < Atoms.Count; ++i) Atoms[i].Freezen = 0;
        }

        public void Add(Atom atom) {
            if(!ChemicalComponents.Contains(atom.ChemicalName)) ChemicalComponents.Add(atom.ChemicalName);
            Atoms.Add(atom);
        }

        public void Clear() {
            Atoms.Clear();
            ChemicalComponents.Clear();
            Properties.Clear();
        }

        public Cluster() {
            Atoms = new List<Atom>();
            ChemicalComponents = new List<String>();
            Properties = new Dictionary<String, dynamic>();

            Readers = new Dictionary<String, IClusterReader>();
            Readers.Add(".xyz", new XYZReader());
            Readers.Add(".sir", new SIRReader());

            Formats = new Dictionary<String, IClusterFormat>();
            Formats.Add(".xyz", new XYZFormat());
            Formats.Add(".sir", new SIRFormat());
        }

        public Cluster(Cluster cluster) {
            Atoms = new List<Atom>();
            ChemicalComponents = new List<String>(cluster.ChemicalComponents);
            Properties = new Dictionary<String, dynamic>(cluster.Properties);

            Readers = new Dictionary<String, IClusterReader>();
            Readers.Add(".xyz", new XYZReader());
            Readers.Add(".sir", new SIRReader());

            Formats = new Dictionary<String, IClusterFormat>();
            Formats.Add(".xyz", new XYZFormat());
            Formats.Add(".sir", new SIRFormat());

            foreach(var atom in cluster.Atoms) {
                Atoms.Add(new Atom(atom));
            }
        }

        public void Rotate(Vector3 axis, Single angle) {
            foreach(var atom in Atoms) {
                atom.Rotate(axis, angle);
            }
        }

        private StringBuilder columns = new StringBuilder();

        public void AppendColumn(string column) {
            if(columns.ToString().Length == 0) columns.Append("columns:");
            columns.Append(column + ",");
        }

        public void RefreshComment(bool appendEnregyColumn = true) {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            if(appendEnregyColumn) AppendColumn("Energy");

            sw.Write("[");
            foreach(var key in Properties.Keys) {
                if(key != "Comment")
                   sw.Write(key + ":" + Properties[key] + " ");
            }
            sw.Write(columns.ToString().Trim(','));
            sw.Write("]");

            sw.Close(); sw.Dispose();

           if(ContainsProperty("Comment")) {
               StringBuilder props = new StringBuilder();
               Boolean flag = false;
               foreach(var c in this["Comment"]) {
                   if(c == '[') {
                       props.Append(c);
                       flag = true;
                       continue;
                   }
                   if( c == ']') {
                       props.Append(c);
                       break;
                   }               
                   if(flag) props.Append(c);
               }
               this["Comment"] = this["Comment"].ToString().Replace(props.ToString(), sb.ToString());
            }
            else {
               this["Comment"] = sb.ToString();
            }
        } 

        public void LoadFromFile(String filename, 
                                 Encoding encoding = null, 
                                 IClusterReader reader = null) 
        {
            if(encoding == null) encoding = Encoding.ASCII;
             StreamReader sr = new StreamReader(filename, encoding);
             String content = sr.ReadToEnd();
             reader = reader == null ? Readers[Path.GetExtension(filename).ToLower()] : reader;
             reader.Read(content, this);
             sr.Close(); sr.Dispose();
        }

        public void SaveToFile(String filename,
                               IClusterFormat format = null,
                               Encoding encoding = null
                              )
        {
            if(encoding == null) encoding = Encoding.ASCII;
            format = format == null ? Formats[Path.GetExtension(filename)] : format;

            StreamWriter sw = new StreamWriter(filename, false, encoding);
            sw.WriteLine(ToString(format));
            sw.Close(); sw.Dispose();
        }

        public void DumpToFile(String filename, 
                            IClusterFormat format = null,
                            Encoding encoding = null) 
        {
            if(encoding == null) encoding = Encoding.ASCII;
            format = format == null ? Formats[Path.GetExtension(filename)] : format;

            StreamWriter sw = new StreamWriter(filename, true, encoding);
            sw.Write(ToString(format));
            sw.Close(); sw.Dispose();
        }

        public String ToString(IClusterFormat format) {
            return format.GetStringRepresentation(this);
        }
    }
}
