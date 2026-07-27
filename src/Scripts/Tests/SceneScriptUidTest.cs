using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace EchoForest.Tests;

/// <summary>
/// Verifies that Godot scenes reference C# scripts by the UID generated in
/// each script's <c>.cs.uid</c> sidecar file.
/// </summary>
[TestFixture]
public class SceneScriptUidTest
{
    private static readonly Regex ScriptResourcePattern = new(
        """\[ext_resource\s+type="Script"(?<attributes>[^\]]*)\]""",
        RegexOptions.Compiled);

    private static readonly Regex AttributePattern = new(
        """(?<name>uid|path)="(?<value>[^"]+)""",
        RegexOptions.Compiled);

    [Test]
    public void CSharpScriptResources_UseMatchingGeneratedUids()
    {
        var repositoryRoot = FindRepositoryRoot();
        var scenesDirectory = Path.Combine(repositoryRoot, "src", "Scenes");
        var failures = new List<string>();

        foreach (var scenePath in Directory.GetFiles(scenesDirectory, "*.tscn", SearchOption.AllDirectories))
        {
            var sceneContents = File.ReadAllText(scenePath);
            foreach (Match resourceMatch in ScriptResourcePattern.Matches(sceneContents))
            {
                var attributes = ParseAttributes(resourceMatch.Groups["attributes"].Value);
                if (!attributes.TryGetValue("path", out var resourcePath) || !resourcePath.EndsWith(".cs", StringComparison.Ordinal))
                    continue;

                var sceneName = Path.GetRelativePath(repositoryRoot, scenePath);
                var scriptPath = Path.Combine(repositoryRoot, resourcePath["res://".Length..].Replace('/', Path.DirectorySeparatorChar));
                var uidPath = scriptPath + ".uid";

                if (!attributes.TryGetValue("uid", out var sceneUid))
                {
                    failures.Add($"{sceneName} is missing uid for {resourcePath}.");
                    continue;
                }

                if (!File.Exists(uidPath))
                {
                    failures.Add($"{sceneName} references {resourcePath}, but {Path.GetRelativePath(repositoryRoot, uidPath)} is missing.");
                    continue;
                }

                var generatedUid = File.ReadAllText(uidPath).Trim();
                if (!string.Equals(sceneUid, generatedUid, StringComparison.Ordinal))
                    failures.Add($"{sceneName} references {resourcePath} with {sceneUid}, but {Path.GetRelativePath(repositoryRoot, uidPath)} contains {generatedUid}.");
            }
        }

        Assert.That(failures, Is.Empty, string.Join(Environment.NewLine, failures));
    }

    private static Dictionary<string, string> ParseAttributes(string attributes)
    {
        var values = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (Match attributeMatch in AttributePattern.Matches(attributes))
            values.Add(attributeMatch.Groups["name"].Value, attributeMatch.Groups["value"].Value);

        return values;
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "EchoForest.sln")))
                return directory.FullName;

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate the repository root from the NUnit test directory.");
    }
}