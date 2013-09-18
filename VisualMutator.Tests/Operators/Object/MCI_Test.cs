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
    public class MCI_Test
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

     
        [Test]
        public void MutationSuccess()
        {
            const string code =
                @"using System;
namespace Ns
{
    public class Test
    {
        Test a = new Test();
        Test b = new Test();
        public void Method1(bool b1, bool b2)
        {
           a.Method1(true, true);
        }
    }
}";
       //     new Conditional().;
            Common.DebugTraverse(code);
           
            
            List<Mutant> mutants;
            ModulesProvider original;
            CodeDifferenceCreator diff;
            Common.RunMutations(code, new MCI_MemberCallFromAnotherInheritedClass(), out mutants, out original, out diff);

         

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