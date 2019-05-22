/*
   Copyright (c) 2019 Sokolov Denis
   IComand intrface for description comands
 */

 namespace NanoSharp.TaskManagment.Comands {
     public interface IComand {

         string Name {get;}
         string Help {get;}
         void Start(string[] args);
         
     }
 }