using GeradorDeArquivo.Tests.TestSupport;
using GeradorTxt;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace GeradorDeArquivo.Tests
{
    [TestFixture]
    public class JsonRepositoryTests
    {
        [Test]
        public void LoadEmpresas_DeveCarregarBasePadrao()
        {
            var empresas = JsonRepository.LoadEmpresas(TestPaths.GetSourceJsonPath());

            Assert.That(empresas, Is.Not.Null);
            Assert.That(empresas, Has.Count.EqualTo(20));
            Assert.That(empresas[0].CNPJ, Is.EqualTo("97.164.236/8824-57"));
            Assert.That(empresas[0].Nome, Is.EqualTo("Empresa Ficticia 01 LTDA"));
            Assert.That(empresas[0].Telefone, Is.EqualTo("(27) 93695-3642"));
            Assert.That(empresas[0].Documentos, Is.Not.Null);
            Assert.That(empresas[0].Documentos, Is.Not.Empty);
        }

        [Test]
        public void LoadEmpresas_DeveLancarFileNotFoundQuandoArquivoNaoExiste()
        {
            var caminhoInexistente = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "base-inexistente.json");

            var ex = Assert.Throws<FileNotFoundException>(() => JsonRepository.LoadEmpresas(caminhoInexistente));

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.FileName, Is.EqualTo(caminhoInexistente));
        }

        [Test]
        public void LoadEmpresas_DeveLancarQuandoJsonForInvalido()
        {
            var pasta = Path.Combine(Path.GetTempPath(), "GeradorDeArquivo.Tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(pasta);

            var caminho = Path.Combine(pasta, "json-invalido.json");
            File.WriteAllText(caminho, "isso nao eh json");

            var ex = Assert.Throws<Exception>(() => JsonRepository.LoadEmpresas(caminho));

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Message, Does.Contain("Falha ao desserializar JSON"));
            Assert.That(ex.InnerException, Is.Not.Null);
        }
    }
}
