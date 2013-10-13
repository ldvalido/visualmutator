﻿namespace VisualMutator.Tests.Operators
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using CommonUtilityInfrastructure.Paths;
    using Model;
    using Model.Decompilation;
    using Model.Decompilation.CodeDifference;
    using Model.Mutations;
    using Model.Mutations.MutantsTree;
    using Model.Mutations.Operators;
    using Model.Mutations.Types;
    using NUnit.Framework;
    using OperatorsStandard;
    using Util;
    using log4net.Appender;
    using log4net.Config;
    using log4net.Layout;

    [TestFixture]
    public class TestRelationalOperatorReplacement
    {
        private String _dsaTestsPath = @"C:\PLIKI\Dropbox\++Inzynierka\VisualMutator\Projekty do testów\dsa-96133\Dsa\Dsa.Test\bin\Debug\Dsa.Test.dll";

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



        

        [Test]
        public void MutationFullOnFloat()
        {
            const string code =
                @"using System;
namespace Ns
{
    public class Test
    {
        public bool Method1(float a, float b)
        {
            bool result = true;
            result &= a > b;
            result &= a < b;
            result &= a <= b;
            result &= a >= b;
            result &= a == b;
            result &= a != b;
            return result;
        }
    }
}";

            List<Mutant> mutants;
            ModulesProvider original;
            CodeDifferenceCreator diff;
            MutationTests.RunMutations(code, new ROR_RelationalOperatorReplacement(), out mutants, out original, out diff);

            Assert.AreEqual(mutants.Count, 42);

            foreach (Mutant mutant in mutants)
            {
                CodeWithDifference codeWithDifference = diff.CreateDifferenceListing(CodeLanguage.CSharp, mutant,
                                                                                     original);
                Console.WriteLine(codeWithDifference.Code);
                Assert.AreEqual(codeWithDifference.LineChanges.Count, 2);
            }
        }

        [Test]
        public void MutationPartialOnObject()
        {
            const string code =
                @"using System;
namespace Ns
{
    public class Test
    {
        public bool Method1(object a, object b)
        {
            bool result = true;
            result &= a == b;
            result &= a != b;
            return result;
        }
    }
}";

            List<Mutant> mutants;
            ModulesProvider original;
            CodeDifferenceCreator diff;
            MutationTests.RunMutations(code, new ROR_RelationalOperatorReplacement(), out mutants, out original, out diff);


            foreach (Mutant mutant in mutants)
            {
                CodeWithDifference codeWithDifference = diff.CreateDifferenceListing(CodeLanguage.CSharp, mutant,
                                                                                     original);
                Console.WriteLine(codeWithDifference.Code);
                codeWithDifference.LineChanges.Count.ShouldEqual(2);
            }
            mutants.Count(m => m.MutationTarget.Variant.Signature == "Equality").ShouldEqual(1);
            mutants.Count(m => m.MutationTarget.Variant.Signature == "NotEquality").ShouldEqual(1);
            mutants.Count(m => m.MutationTarget.Variant.Signature == "True").ShouldEqual(2);
            mutants.Count(m => m.MutationTarget.Variant.Signature == "False").ShouldEqual(2);
            mutants.Count.ShouldEqual(6);
        }

        [Test]
        public void MutationSuccess()
        {
            const string code =
                @"using System;
namespace Ns
{
    public class Test
    {
        public bool Method1(int a, int b)
        {
            bool result = true;
            result &= a > b;
            result &= a < b;
            result &= a <= b;
            result &= a >= b;
            result &= a == b;
            result &= a != b;
            return result;
        }
    }
}";

            List<Mutant> mutants;
            ModulesProvider original;
            CodeDifferenceCreator diff;
            MutationTests.RunMutations(code, new ROR_RelationalOperatorReplacement(), out mutants, out original, out diff);

            Assert.AreEqual(mutants.Count, 42);

            foreach (Mutant mutant in mutants)
            {
                CodeWithDifference codeWithDifference = diff.CreateDifferenceListing(CodeLanguage.CSharp, mutant,
                                                                                     original);
                Console.WriteLine(codeWithDifference.Code);
                codeWithDifference.LineChanges.Count.ShouldEqual(2);
            }
        }
        [Test]
        public void Mutant_Groups_Are_Formed_with_correct_number_of_mutants()
        {
            var oper = new ROR_RelationalOperatorReplacement();
            ///////
            var cci = new CommonCompilerInfra();
            var utils = new OperatorUtils(cci);
            var container = new MutantsContainer(cci, utils);
            var visualizer = new CodeVisualizer(cci);
            var cache = new MutantsCache(container);
            List<AssemblyNode> assemblyNodes = new List<AssemblyNode>
            {
                new AssemblyNode("", cci.AppendFromFile(MutationTests.DsaPath))
                {
                    AssemblyPath = new FilePathAbsolute(MutationTests.DsaPath)
                },
            };
            var original = new ModulesProvider(cci.Modules);
            cache.setDisabled(disableCache: false);
            var diff = new CodeDifferenceCreator(cache, visualizer);
            container.DebugConfig = true;
            List<MutantGroup> groups = MutationTests.CreateMutantsLight(oper, container, assemblyNodes, cache, 500).ToList();

            var groupsBad = groups
                .Where(g => g.Mutants.Select(m => m.ToString()).Distinct().Count() != g.Mutants.Count()).ToList();
            int y = groupsBad.Count;
            
            groupsBad.Count.ShouldEqual(0);
        }
        [Test]
        public void ROR_Success()
        {
            var oper = new ROR_RelationalOperatorReplacement();
            ///////
            var cci = new CommonCompilerInfra();
            var utils = new OperatorUtils(cci);
            var container = new MutantsContainer(cci, utils);
            var visualizer = new CodeVisualizer(cci);
            var cache = new MutantsCache(container);
            List<AssemblyNode> assemblyNodes = new List<AssemblyNode>
            {
                new AssemblyNode("", cci.AppendFromFile(MutationTests.DsaPath))
                {
                    AssemblyPath = new FilePathAbsolute(MutationTests.DsaPath)
                },
            };
            var original = new ModulesProvider(cci.Modules);
            cache.setDisabled(disableCache: false);
            var diff = new CodeDifferenceCreator(cache, visualizer);
            container.DebugConfig = true;
            List<MutantGroup> groups = MutationTests.CreateMutantsLight(oper, container, assemblyNodes, cache, 500).ToList();
            var mutants = groups.SelectMany(g=>g.Mutants).ToList();

            foreach (Mutant mutant in mutants)
            {
                CodeWithDifference codeWithDifference = diff.CreateDifferenceListing(CodeLanguage.CSharp, mutant,
                                                                   original);
                Console.WriteLine(codeWithDifference.Code);
                if (codeWithDifference.LineChanges.Count > 4 || codeWithDifference.LineChanges.Count == 0)
                {
                    Assert.Fail();
                }
            }
            mutants.Count.ShouldEqual(1);
        }
    }
}