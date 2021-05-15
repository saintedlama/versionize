using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Version = NuGet.Versioning.SemanticVersion;

namespace Versionize
{
    public class Projects
    {
        private readonly IEnumerable<Project> _projects;

        private Projects(IEnumerable<Project> projects)
        {
            _projects = projects;
        }

        public bool IsEmpty()
        {
            return _projects.Count() == 0;
        }

        public bool HasInconsistentVersioning()
        {
            var firstProjectVersion = _projects.FirstOrDefault()?.Version;

            if (firstProjectVersion == null)
            {
                return true;
            }

            return _projects.Any(p => !p.Version.Equals(firstProjectVersion));
        }

        public Version Version { get => _projects.First().Version; }

        public static Projects Discover(string workingDirectory)
        {
            var projects = Directory
                .GetFiles(workingDirectory, "*.csproj", SearchOption.AllDirectories)
                .Where(Project.IsVersionable)
                .Select(Project.Create)
                .ToList();

            return new Projects(projects);
        }

        public void WriteVersion(Version nextVersion)
        {
            foreach (var project in _projects)
            {
                project.WriteVersion(nextVersion);
            }
        }

        public IEnumerable<string> GetProjectFiles()
        {
            return _projects.Select(project => project.ProjectFile);
        }
    }
}
