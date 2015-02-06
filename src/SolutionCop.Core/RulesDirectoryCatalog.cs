﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SolutionCop.Core
{
    public class RulesDirectoryCatalog
    {
        public IEnumerable<IProjectRule> LoadRules()
        {
            Console.Out.WriteLine("INFO: Scanning for rules...");

            var rules = new List<IProjectRule>();
            var assemblies = new DirectoryInfo(".").GetFiles("*.DLL");
            foreach (var assemblyFile in assemblies)
            {
                var ruleTypes = Assembly.LoadFrom(assemblyFile.FullName).GetExportedTypes().Where(t => t.IsClass && !t.IsAbstract && typeof(IProjectRule).IsAssignableFrom(t));
                foreach (var ruleType in ruleTypes)
                {
                    try
                    {
                        var rule = (IProjectRule)Activator.CreateInstance(ruleType);
                        Console.Out.WriteLine("INFO: Rule {0} found. Description: '{1}'", rule.Id, rule.DisplayName);
                        rules.Add(rule);
                    }
                    catch (Exception)
                    {
                        Console.Out.WriteLine("ERROR: Could not initialize rule from type {0}. Skipping it.", ruleType.FullName);
                    }
                }
            }

            Console.Out.WriteLine("INFO: Scanning for rules finished! Rules found: {0}", rules.Count);
            return rules;
        }
    }
}