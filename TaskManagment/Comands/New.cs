/*
   Copyright (c) 2019 Sokolov Denis
   Run class for running simulation
 */
 using System;
 using System.IO;
 using System.Collections.Generic;
 using System.Text;
 using NanoSharp.TaskManagment.Templates;
 using YamlDotNet.Serialization;
 using YamlDotNet.Serialization.NamingConventions;

 namespace NanoSharp.TaskManagment.Comands {
     public class New: IComand {
         public string Name {private set; get;} = "new";
         public string Help {private set; get;} = "creating configuration file ('configuration.yaml'). Comand has required parameter template_name. Example: 'new mc' - create configuration for Monte-Carlo simulation";

         Dictionary<string, ITemplate> templates = new Dictionary<string, ITemplate>();

          public void Add(ITemplate template) {
             templates.Add(template.Name, template);
         }

         public void Delete(string templatename) {
             if(templates.ContainsKey(templatename))
                templates.Remove(templatename);
         }


         public void Start(string[] args) {
             if(args.Length == 0) {
                 Console.WriteLine("Not defined parameter 'teplate_name'");
                 Console.WriteLine(Help);
                 return;
             }

            string[] destArgs = null;
                 if(args.Length>0) {
                     destArgs = new string[args.Length -1];
                     Array.Copy(args, 1, destArgs, 0, destArgs.Length);
                 } 
                 if(templates.ContainsKey(args[0])) {
                     templates[args[0]].MakeConfig(Directory.GetCurrentDirectory(), destArgs);
                 }
                 else {
                     Console.WriteLine($"Not found template '{args[0]}'");
                 }
         }
     }
 }