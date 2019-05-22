/*
   Copyright (c) 2019 Sokolov Denis
   Atom class for description of Atoms
 */

using System;
using System.Collections.Generic;
using System.Numerics;

namespace NanoSharp.Core {
    public class Atom {

        public Vector3 Position;

        public String ChemicalName;

        public Double Energy;

        public Int32 Freezen;

        public Dictionary<String, dynamic> Properties;

        public dynamic this[String property] {
            set {
                if(Properties.ContainsKey(property)) Properties[property] = value;
                else Properties.Add(property, value);
            }

            get {
                return Properties[property];
            }
        }

        public bool ContainsProperty(String property) {
            return Properties.ContainsKey(property);
        }

        public Atom() {
            Position = Vector3.Zero;
            ChemicalName = "";
            Properties = new Dictionary<String, dynamic>();
            Energy = 0.0f;
            Freezen = 0;
        }

        public Atom(Single x, Single y, Single z, String chemicalName) {
            Position = new Vector3(x, y, z);
            ChemicalName = chemicalName;
            Properties = new Dictionary<String, dynamic>();
            Energy = 0.0f;
            Freezen = 0;
        }

        public Atom(String chemicalName) {
            Position = Vector3.Zero;
            ChemicalName = chemicalName;
            Properties = new Dictionary<String, dynamic>();
            Energy = 0.0f;
            Freezen = 0;
        }

        public Atom(Vector3 position, String chemicalName) {
            Position = position;
            ChemicalName = chemicalName;
            Properties = new Dictionary<String, dynamic>();
            Energy = 0.0f;
            Freezen = 0;
        }

        public Atom(Atom atom) {
            Position = atom.Position;
            ChemicalName = atom.ChemicalName;
            Energy = atom.Energy;
            Properties = new Dictionary<string, dynamic>(atom.Properties);
            Freezen = atom.Freezen;
        }

        public void Rotate(Vector3 axis, Single angle) {
            Quaternion q = Quaternion.CreateFromAxisAngle(axis, angle);
            Quaternion pos = new Quaternion(Position, 0);
            pos = q * pos * Quaternion.Conjugate(q);
            Position.X = pos.X;
            Position.Y = pos.Y;
            Position.Z = pos.Z;
        }

        public Single Distance(Atom atom) {
            return Vector3.Distance(Position, atom.Position);
        }

        public override String ToString() {
            return ChemicalName + " " + Position + " " + Energy + " " + Freezen;
        }
    }
}