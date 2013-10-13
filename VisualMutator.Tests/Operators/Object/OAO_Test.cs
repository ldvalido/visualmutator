﻿namespace VisualMutator.Tests.Operators
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Cci.MutableCodeModel;
    using Model;
    using Model.Decompilation;
    using Model.Decompilation.CodeDifference;
    using Model.Mutations.MutantsTree;
    using NUnit.Framework;
    using OperatorsObject.Operators;
    using OperatorsStandard;
    using Util;
    using log4net.Appender;
    using log4net.Config;
    using log4net.Layout;

    [TestFixture]
    public class OAO_Test
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            BasicConfigurator.Configure(
                new ConsoleAppender
                    {
                        Layout = new SimpleLayout()
                    });
        }

        #endregion

     String _assemblyPath = @"D:\PLIKI\Dropbox\++Inzynierka\VisualMutator\Projekty do testów\dsa-96133\Dsa\Dsa\bin\Debug\Dsa.dll";
        [Test]
        public void MutationSuccess()
        {
            const string code =
                @"using System;
namespace Ns
{
    public class Test
    {
        public void Execute()
        {
            Method1(""string1"", ""string2"", 0, 5f, 1);
        }
        public bool Method1(string s, string s2, int a, float f, int b)
        {
            return true;
        }
    }
}";
       //     new Conditional().;
        //    MutationTests.DebugTraverse(code);
           
            
            List<Mutant> mutants;
            ModulesProvider original;
            CodeDifferenceCreator diff;
            MutationTests.RunMutationsFromFile(_assemblyPath, new OAO_ArgumentOrderChange(), out mutants, out original, out diff);

         

            foreach (Mutant mutant in mutants)
            {
                CodeWithDifference codeWithDifference = diff.CreateDifferenceListing(CodeLanguage.CSharp, mutant,
                                                                                     original);
                Console.WriteLine(codeWithDifference.Code);
             //   Assert.AreEqual(codeWithDifference.LineChanges.Count, 2);
            }

            mutants.Count.ShouldEqual(1);
        }
    }
}