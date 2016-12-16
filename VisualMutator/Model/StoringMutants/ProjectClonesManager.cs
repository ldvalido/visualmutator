﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using UsefulTools.ExtensionMethods;
using UsefulTools.FileSystem;
using UsefulTools.Paths;
using VisualMutator.Infrastructure;

namespace VisualMutator.Model.StoringMutants
{
    public interface IProjectClonesManager : IDisposable
    {
        ProjectFilesClone CreateClone(string name);
        Task<ProjectFilesClone> CreateCloneAsync(string name);
    }

    public class ProjectClonesManager : IProjectClonesManager
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IHostEnviromentConnection _hostEnviroment;
        private readonly IFileSystem _fs;
        private readonly ProjectFilesClone _mainClone;
        private readonly FilesManager _filesManager;

        public ProjectClonesManager(
            IHostEnviromentConnection hostEnviroment,
            FilesManager filesManager,
            IFileSystem fs)
        {
            _hostEnviroment = hostEnviroment;
            _filesManager = filesManager;
            _fs = fs;

            List<FilePathAbsolute> originalProjectFiles = _hostEnviroment.GetProjectAssemblyPaths().ToList();
            IEnumerable<FilePathAbsolute> referencedFiles = GetReferencedAssemblyPaths(originalProjectFiles).Select(s => s.ToFilePathAbs());

            FilePathAbsolute tmp = CreateTmpDir("VisualMutator-MainClone-");
            _mainClone = _filesManager.CreateProjectClone(referencedFiles, originalProjectFiles, tmp).Result;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            _mainClone.Dispose();
        }

        public ProjectFilesClone CreateClone(string name)
        {
            return CreateCloneAsync(name).Result;
        }

        public async Task<ProjectFilesClone> CreateCloneAsync(string name)
        {
            FilePathAbsolute tmp = CreateTmpDir("VisualMutator-" + name + "-");
            ProjectFilesClone clone = await _filesManager.CreateProjectClone(_mainClone.Referenced, _mainClone.Assemblies, tmp);
            clone.IsIncomplete |= _mainClone.IsIncomplete;
            return clone;
        }

        private FilePathAbsolute CreateTmpDir(string s)
        {
            string tmpDirectoryPath = Path.Combine(_hostEnviroment.GetTempPath(), s+Path.GetRandomFileName());
            _fs.Directory.CreateDirectory(tmpDirectoryPath);
            return new FilePathAbsolute(tmpDirectoryPath);
        }

        private IEnumerable<string> GetReferencedAssemblyPaths(IList<FilePathAbsolute> projects)
        {
            var list = new HashSet<string>(projects.AsStrings());
            foreach (var binDir in projects.Select(p => p.ParentDirectoryPath))
            {
                var files = Directory.EnumerateFiles(binDir.Path, "*.*", SearchOption.AllDirectories)
                        .Where(s => s.EndsWith(".dll") || s.EndsWith(".pdb"))
                        .Where(p => !projects.Contains(p.ToFilePathAbs()));
                list.AddRange(files);
            }
            return list;
        }
    }
}