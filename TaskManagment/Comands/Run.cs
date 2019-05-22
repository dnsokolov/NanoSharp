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
     public class Run : IComand {

         public class ConfigInfo {
             public string Name {set; get;}
             public string Description {set; get;}

             public dynamic Tasks {set; get;}

             public static ConfigInfo LoadFromFile(string file) {
                 StreamReader srr = new StreamReader(file, Encoding.ASCII);
                 StringReader sr = new StringReader(srr.ReadToEnd());
                 srr.Close(); srr.Dispose();
                  var deserializer = new DeserializerBuilder()
                                   .WithNamingConvention(new HyphenatedNamingConvention())
                                   .Build();

                 return deserializer.Deserialize<ConfigInfo>(sr);
             }
         }

         string configfile = "configuration.yaml";
         public string Name {private set; get;} = "run";
         public string Help {private set; get;} = @"running simulation or calculating in current directory.";

         Dictionary<string, ITemplate> templates;
         public Run() {
             templates = new Dictionary<string, ITemplate>();
         }

         public void Add(ITemplate template) {
             templates.Add(template.Name, template);
         }

         public void Delete(string templatename) {
             if(templates.ContainsKey(templatename))
                templates.Remove(templatename);
         }

         public void Start(string[] args) {
             if(File.Exists(configfile)) {
                 string[] destArgs = null;
                 if(args.Length>0) {
                     destArgs = new string[args.Length -1];
                     Array.Copy(args, 1, destArgs, 0, destArgs.Length);
                 } 
                 ConfigInfo config = ConfigInfo.LoadFromFile(configfile);
                 if(templates.ContainsKey(config.Name)) {
                     templates[config.Name].Run(Directory.GetCurrentDirectory(), destArgs);
                 }
                 else {
                     Console.WriteLine($"Not found template '{config.Name}'");
                 }
             }
             else {
                 Console.WriteLine($"Not found configuration file '{configfile}'");
             }
         }
     }
 }