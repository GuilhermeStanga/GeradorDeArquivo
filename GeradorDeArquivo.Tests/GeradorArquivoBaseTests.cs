using GeradorDeArquivo.Tests.TestSupport;
using GeradorTxt;
using NUnit.Framework;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace GeradorDeArquivo.Tests
{
    [TestFixture]
    public class GeradorArquivoBaseTests
    {
        [Test]
        public void GerarLinhaTipo00_DeveRespeitarLeiaute()
        {
            var sut = new GeradorArquivoBaseExposto();
            var empresa = new Empresa
            {
                CNPJ = "12.345.678/0001-90",
                Nome = "Empresa X",
                Telefone = "(11) 99999-1111"
            };

            var linha = sut.GerarLinhaTipo00(empresa);

            Assert.That(linha, Is.EqualTo("00|12.345.678/0001-90|Empresa X|(11) 99999-1111\r\n"));
        }

        [Test]
        public void GerarLinhaTipo01_DeveFormatarValorComDuasCasas()
        {
            var sut = new GeradorArquivoBaseExposto();
            var documento = new Documento
            {
                Modelo = "NF",
                Numero = "000123",
                Valor = 2185.45m
            };

            var linha = sut.GerarLinhaTipo01(documento);

            Assert.That(linha, Is.EqualTo("01|NF|000123|2185.45\r\n"));
        }

        [Test]
        public void GerarLinhaTipo02_DeveFormatarValorComDuasCasas()
        {
            var sut = new GeradorArquivoBaseExposto();
            var item = new ItemDocumento
            {
                Descricao = "Servico Especial",
                Valor = 17.5m
            };

            var linha = sut.GerarLinhaTipo02(item);

            Assert.That(linha, Is.EqualTo("02|Servico Especial|17.50\r\n"));
        }

        [Test]
        public void Gerar_ComListaVazia_DeveCriarArquivoSemConteudoLogico()
        {
            var sut = new GeradorArquivoBase();
            var outputPath = TestPaths.CreateTemporaryOutputFile();

            sut.Gerar(new List<Empresa>(), outputPath);

            var conteudo = File.ReadAllText(outputPath);

            Assert.That(conteudo, Is.Empty);
        }

        [Test]
        public void Gerar_QuandoEmpresaNaoTemDocumentos_DeveGerarSomenteTipo00()
        {
            var sut = new GeradorArquivoBase();
            var outputPath = TestPaths.CreateTemporaryOutputFile();
            var empresas = new List<Empresa>
            {
                new Empresa
                {
                    CNPJ = "12.345.678/0001-90",
                    Nome = "Empresa Sem Docs",
                    Telefone = "(11) 99999-1111",
                    Documentos = new List<Documento>()
                }
            };

            sut.Gerar(empresas, outputPath);

            var linhas = File.ReadAllLines(outputPath);

            CollectionAssert.AreEqual(
                new[]
                {
                    "00|12.345.678/0001-90|Empresa Sem Docs|(11) 99999-1111"
                },
                linhas);
        }

        [Test]
        public void Gerar_QuandoDocumentoNaoTemItens_DeveGerarTipo00ETipo01()
        {
            var sut = new GeradorArquivoBase();
            var outputPath = TestPaths.CreateTemporaryOutputFile();
            var empresas = new List<Empresa>
            {
                new Empresa
                {
                    CNPJ = "12.345.678/0001-90",
                    Nome = "Empresa Sem Itens",
                    Telefone = "(11) 99999-1111",
                    Documentos = new List<Documento>
                    {
                        new Documento
                        {
                            Modelo = "NF",
                            Numero = "000123",
                            Valor = 10m,
                            Itens = new List<ItemDocumento>()
                        }
                    }
                }
            };

            sut.Gerar(empresas, outputPath);

            var linhas = File.ReadAllLines(outputPath);

            CollectionAssert.AreEqual(
                new[]
                {
                    "00|12.345.678/0001-90|Empresa Sem Itens|(11) 99999-1111",
                    "01|NF|000123|10.00"
                },
                linhas);
        }

        [Test]
        public void Gerar_UsandoBaseCompleta_DeveGerarTodasAsLinhasNaOrdemEsperada()
        {
            var empresas = JsonRepository.LoadEmpresas(TestPaths.GetSourceJsonPath());
            var sut = new GeradorArquivoBase();
            var outputPath = TestPaths.CreateTemporaryOutputFile();

            sut.Gerar(empresas, outputPath);

            var linhasGeradas = File.ReadAllLines(outputPath);
            var linhasEsperadas = ConstruirLinhasEsperadas(empresas).ToArray();

            CollectionAssert.AreEqual(linhasEsperadas, linhasGeradas);
            Assert.That(linhasGeradas.Count(l => l.StartsWith("00|")), Is.EqualTo(empresas.Count));
            Assert.That(linhasGeradas.Count(l => l.StartsWith("01|")), Is.EqualTo(empresas.Sum(e => e.Documentos.Count)));
            Assert.That(linhasGeradas.Count(l => l.StartsWith("02|")), Is.EqualTo(empresas.Sum(e => e.Documentos.Sum(d => d.Itens.Count))));
        }

        private static IEnumerable<string> ConstruirLinhasEsperadas(IEnumerable<Empresa> empresas)
        {
            foreach (var empresa in empresas)
            {
                yield return string.Format(
                    CultureInfo.InvariantCulture,
                    "00|{0}|{1}|{2}",
                    empresa.CNPJ,
                    empresa.Nome,
                    empresa.Telefone);

                foreach (var documento in empresa.Documentos)
                {
                    yield return string.Format(
                        CultureInfo.InvariantCulture,
                        "01|{0}|{1}|{2:0.00}",
                        documento.Modelo,
                        documento.Numero,
                        documento.Valor);

                    foreach (var item in documento.Itens)
                    {
                        yield return string.Format(
                            CultureInfo.InvariantCulture,
                            "02|{0}|{1:0.00}",
                            item.Descricao,
                            item.Valor);
                    }
                }
            }
        }
    }
}
