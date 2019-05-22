/*
   Copyright (c) 2019 Sokolov Denis
   ComandManager class for processing comand line parameters
 */

 using System;
 using System.Collections.Generic;
 using NanoSharp.TaskManagment.Comands;

 namespace NanoSharp.TaskManagment {
    public class ComandManager {
        Dictionary<string, IComand> comands;

        public ComandManager() {
          comands = new Dictionary<string, IComand>();
        }

        public void Add(IComand comand) {
          comands.Add(comand.Name, comand);
        }

        public void Delete(string comandName) {
          if(comands.ContainsKey(comandName))
             comands.Remove(comandName);
        }

        public void Start(string[] args) {
          if(comands.ContainsKey(args[0])) {
             string[] destargs = new string[args.Length - 1];
             Array.Copy(args, 1, destargs, 0, destargs.Length);
             comands[args[0]].Start(destargs);
          }
          else {
            Console.WriteLine($"Comand '{args[0]}' not found");
          }
        }

        public void Help() {
          Console.WriteLine("Brief Help");
          foreach(var name in comands.Keys) {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(name + " - ");
            Console.ResetColor();
            Console.WriteLine(comands[name].Help);
          }
          Console.WriteLine();
        }
    }
 }