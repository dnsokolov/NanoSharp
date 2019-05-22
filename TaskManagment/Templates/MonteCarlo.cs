/*
   Copyright (c) 2019 Sokolov Denis
   MonteCarlo class template for creating config of Monte-Carlo simulation
 */
 using System;
 using System.IO;
 using System.Collections.Generic;
 using System.Globalization;
 using System.Linq;
 using System.Text;
 using NanoSharp.Core;
 using NanoSharp.Core.ClusterReaders;
 using MathNet.Numerics.Statistics;
 using YamlDotNet.Serialization;
 using YamlDotNet.Serialization.NamingConventions;

namespace NanoSharp.TaskManagment.Templates {
    public class MonteCarlo : ITemplate {

        public class MonteCarloTask {
            public string Directory {set; get;}
            public int MicroSteps {set; get;} = 600;
            public int MacroSteps {set; get;} = 100;

            public double StartTemperature {set; get;} = 293.15;
            public double FinishTemperature {set; get;} = 293.15;
            public double StepTemperature {set; get;} = 10.0;

            public double MaxTranslation {set; get;} = 0.25;

            public List<string> Potentials {set; get;}
            public string LoadConfig {set; get;}
            public int NumProcesses {set; get;} = 1;

            public string DumpFile {set; get;} = "dump.xyz";

            public MonteCarloTask() {
                Potentials = new List<string>();
                Potentials.Add("gupt");
            }

            MonteCarloEngine mce;
            Cluster cluster;
            string dumpfile;
            double tstart;
            int mcstart;

            bool ContinueCondition(double T){
                if(StepTemperature > 0) {
                    return T <= FinishTemperature;
                }
                else {
                    return T >= FinishTemperature;
                }
            }

            public void Run() {
                dumpfile = Path.Combine(Directory, DumpFile);
                if(File.Exists(dumpfile)) {
                    cluster = new Cluster();
                    cluster.LoadFromFile(dumpfile, null, new XYZReader(-1));
     
                    tstart = Convert.ToDouble(cluster["temperature"].ToString(), CultureInfo.InvariantCulture);
                    mcstart = Convert.ToInt32(cluster["mcstep"].ToString());
                }
                else {
                    cluster = new Cluster();
                    cluster.LoadFromFile(LoadConfig);
                    tstart = StartTemperature;
                    mcstart = -1;
                }
                cluster.RefreshComment();
                mce = new MonteCarloEngine(cluster, MaxTranslation);
                foreach(var line in Potentials) {
                    if(line.StartsWith("gupt")) {
                        mce.AddGuptaParams(line.Replace("gupt", "").Trim());
                    }
                    if(line.StartsWith("lj")) {
                        mce.AddLJParams(line.Replace("lj", "").Trim());
                    }
                }
                mce.Init();
                StepTemperature = StartTemperature < FinishTemperature ? StepTemperature : -StepTemperature;
                for(double T = tstart; ContinueCondition(T); T += StepTemperature) {
                    for(int step = mcstart + 1; step < MacroSteps; ++step) {
                        Console.WriteLine($"[{Directory}]:\n Launching macrostep {step} for temperature {T.ToString(CultureInfo.InvariantCulture)}");
                        mce.Run(MicroSteps, T);

                        Console.WriteLine($"Saving results for macrostep {step} for temperature {T.ToString(CultureInfo.InvariantCulture)} in {dumpfile}");
                        cluster["temperature"] = T.ToString(CultureInfo.InvariantCulture);
                        cluster["mcstep"] = step.ToString();

                        DescriptiveStatistics stat = new DescriptiveStatistics(cluster.GetEnergies());
                        cluster["energy"] = stat.Mean.ToString(CultureInfo.InvariantCulture);
                        cluster["sigma"] = stat.StandardDeviation.ToString(CultureInfo.InvariantCulture);
                        cluster["max_energy"] = stat.Maximum.ToString(CultureInfo.InvariantCulture);
                        cluster["min_energy"] = stat.Minimum.ToString(CultureInfo.InvariantCulture);

                        Console.WriteLine("Average energy of cluster: " + cluster["energy"] + " eV/atom");
                        Console.WriteLine("Standart deviation of energy: " + cluster["sigma"] + " eV/atom");
                        Console.WriteLine("Max energy: " + cluster["max_energy"] + " eV");
                        Console.WriteLine("Min energy: " + cluster["min_energy"] + " eV");

                        cluster.RefreshComment(false);

                        cluster.DumpToFile(dumpfile);
                        Console.WriteLine();
                    }
                    mcstart = -1;
                }
            }        
        }

        public string Name {private set; get;} = "mc";
        public string Description {private set; get;} = "Template for generating Monte-Carlo config file and running Monte-Carlo simulation";

        public List<MonteCarloTask> Tasks;

        public void MakeConfig(string directory, string[] args) {
            Tasks = new List<MonteCarloTask>();
            var configfiles = Directory.GetFiles(directory, "*.*")
                              .Where(s => s.ToLower().EndsWith(".xyz") || s.EndsWith(".sir"));
            foreach(var c in configfiles) {
                Console.WriteLine("Create config for " + c);
                MonteCarloTask task = new MonteCarloTask();
                task.Directory = Path.GetFileNameWithoutExtension(c);
                task.LoadConfig = c;
                Tasks.Add(task);
            }
            StreamWriter sw = new StreamWriter(Path.Combine(directory,"configuration.yaml"), false, Encoding.ASCII);
            sw.WriteLine(ToString());
            sw.Close(); sw.Dispose();
            Console.WriteLine("Completed...");
        }

        public void Run(string directory, string[] args) {
            StreamReader sr = new StreamReader(Path.Combine(directory,"configuration.yaml"), Encoding.ASCII);
            MonteCarlo mc = Deserialize(sr.ReadToEnd());
            sr.Close(); sr.Dispose();
            string currentdir;
            foreach(var task in mc.Tasks) {
                currentdir = Path.Combine(directory, task.Directory);
                if(!Directory.Exists(currentdir)) {
                    Directory.CreateDirectory(currentdir);
                }
                task.Directory = currentdir;
                task.Run();
            }
        }

        public override string ToString(){
             var serializer = new SerializerBuilder()
                                  .WithNamingConvention(new HyphenatedNamingConvention())
                                  .Build();

             return serializer.Serialize(this);
        }

        public static MonteCarlo Deserialize(string yamlstr)
        {
            StringReader sr = new StringReader(yamlstr);
            var deserializer = new DeserializerBuilder()
                                   .WithNamingConvention(new HyphenatedNamingConvention())
                                   .Build();

            return deserializer.Deserialize<MonteCarlo>(sr);
        }

    }
}