using NUnit.Framework;
using System;
using System.IO;

namespace GeradorDeArquivo.Tests.TestSupport
{
    internal static class TestPaths
    {
        public static string GetRepositoryRoot()
        {
            return Path.GetFullPath(Path.Combine(
                TestContext.CurrentContext.TestDirectory,
                "..",
                "..",
                "..",
                ".."));
        }

        public static string GetSourceJsonPath()
        {
            return Path.Combine(GetRepositoryRoot(), "GeradorDeArquivo", "data", "base-dados.json");
        }

        public static string GetSourceJsonV2Path()
        {
            return Path.Combine(GetRepositoryRoot(), "GeradorDeArquivo", "data", "base-dados-v2.json");
        }

        public static string CreateTemporaryOutputFile()
        {
            var directory = Path.Combine(Path.GetTempPath(), "GeradorDeArquivo.Tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            return Path.Combine(directory, "saida.txt");
        }
    }
}
