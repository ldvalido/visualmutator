﻿namespace VisualMutator.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Controls;

    using CommonUtilityInfrastructure;
    using CommonUtilityInfrastructure.Comparers;
    using CommonUtilityInfrastructure.FunctionalUtils;
    using CommonUtilityInfrastructure.WpfUtils;

    using ICSharpCode.Decompiler;

    using ICSharpCode.ILSpy;
    using Model.Decompilation;
    using Model.Decompilation.CodeDifference;
    using Model.Mutations.MutantsTree;
    using Mono.Cecil;

    using VisualMutator.Model;
    using VisualMutator.Model.Mutations;
    using VisualMutator.ViewModels;

    public class MutantDetailsController : Controller
    {
        private readonly MutantDetailsViewModel _viewModel;

        private readonly ICodeDifferenceCreator _codeDifferenceCreator;


        private readonly CommonServices _commonServices;

 

        private Mutant _currentMutant;
        private MutationTestingSession _session;

        public MutantDetailsController(
            MutantDetailsViewModel viewModel, 
            ICodeDifferenceCreator codeDifferenceCreator,
            CommonServices commonServices)
        {
            _viewModel = viewModel;
            _codeDifferenceCreator = codeDifferenceCreator;
       

            _commonServices = commonServices;

            _viewModel.RegisterPropertyChanged(_=>_.SelectedTabHeader)
                .Where(x=> _currentMutant != null).Subscribe(LoadData);

            _viewModel.RegisterPropertyChanged(_ => _.SelectedLanguage).Subscribe(LoadCode);

        }
        public void LoadDetails(Mutant mutant, MutationTestingSession session)
        {
            _session = session;
            _currentMutant = mutant;

            LoadData(_viewModel.SelectedTabHeader);

        }
        public void LoadData(string header)
        {
            FunctionalExt.Switch(header)
                .Case("Tests", () => LoadTests(_currentMutant))
                .Case("Code", () => LoadCode(_viewModel.SelectedLanguage))
                .ThrowIfNoMatch();   
        }

  

        public async void LoadCode(CodeLanguage selectedLanguage)
        {
            _viewModel.IsCodeLoading = true;
            _viewModel.ClearCode();

            var mutant = _currentMutant;
            var assemblies = _session.OriginalAssemblies;

            if(mutant != null)
            {
                CodeWithDifference diff = await GetCodeWithDifference(selectedLanguage, mutant, new ModulesProvider(assemblies.Select(_=>_.AssemblyDefinition).ToList()));
                if (diff != null)
                {
                    _viewModel.PresentCode(diff);
                    _viewModel.IsCodeLoading = false;
                }
            }
         
           
        }

        private async Task<CodeWithDifference> GetCodeWithDifference(CodeLanguage selectedLanguage, Mutant mutant, ModulesProvider modules)
        {

            return _codeDifferenceCreator.CreateDifferenceListing(selectedLanguage,
                mutant, modules);
        }

        private void LoadTests(Mutant mutant)
        {
            _viewModel.TestNamespaces.Clear();

           

            if (mutant.MutantTestSession.IsComplete)
            {
                _viewModel.TestNamespaces.AddRange(mutant.MutantTestSession.TestNamespaces);
            }
            else
            {
                //_listenerForCurrentMutant = mutant.TestSession.WhenPropertyChanged(_ => _.IsComplete)
                //    .Subscribe(x => _viewModel.TestNamespaces.AddRange(mutant.TestSession.TestNamespaces));
            }
        }


        public MutantDetailsViewModel ViewModel
        {
            get
            {
                return _viewModel;
            }
        }

        public void Clean()
        {
            _currentMutant = null;
           
            _viewModel.IsCodeLoading = false;
            _viewModel.TestNamespaces.Clear();
            _viewModel.SelectedLanguage = CodeLanguage.CSharp;
            _viewModel.ClearCode();

        }
    }
}