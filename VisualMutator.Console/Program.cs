﻿using System.Threading.Tasks;

namespace VisualMutator.Console
{
    class Program
    {
        public Program()
        {
            T2();
            T1();
        }



     //   private static ICollection<TestId> _selectedTests;

        private async Task<object> L1()
        {
            int i = 0;
            return await Task.Run(() =>
            {
                while (i >= 0)
                {
                }
                return new object();
            });
        }
        private async void T2()
        {
            await L1();
        }


        private async void T1()
        {
            await L1();
        }
        private static void Main(string[] args)
        {
            System.Console.WriteLine("Started VisualMutator.Console with params: "+ args.MakeString());

            if (args.Length >= 5)
            {
                var parser = new CommandLineParser();
                parser.ParseFrom(args);
                var connection = new EnvironmentConnection(parser);
                var boot = new ConsoleBootstrapper(connection, parser);
                boot.Initialize().Wait();
            }
            else
            {
                System.Console.WriteLine("Too few parameters.");
            }
        }

       
    }
}
