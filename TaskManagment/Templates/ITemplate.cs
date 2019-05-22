/*
   Copyright (c) 2019 Sokolov Denis
   ITemplate intrface for description templates
 */

 namespace NanoSharp.TaskManagment.Templates {
     public interface ITemplate {
         string Name {get;}
         string Description {get;}      
         void MakeConfig(string directory, string[] args);   
         void Run(string directory, string[] args);
     }
 }