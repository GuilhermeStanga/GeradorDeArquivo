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
        public void Gerar_ComListaVazia_DeveGerarSomenteControles()
        {
            var sut = new GeradorArquivoBase();
            var outputPath = TestPaths.CreateTemporaryOutputFile();

            sut.Gerar(new List<Empresa>(), outputPath);

            var linhas = File.ReadAllLines(outputPath);

            CollectionAssert.AreEqual(
                new[]
                {
                    "09|00|0",
                    "09|01|0",
                    "09|02|0",
                    "09|03|0",
                    "99|5"
                },
                linhas);
        }

        [Test]
        public void Gerar_QuandoEmpresaNaoTemDocumentos_DeveGerarTipo00EControles()
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
                    "00|12.345.678/0001-90|Empresa Sem Docs|(11) 99999-1111",
                    "09|00|1",
                    "09|01|0",
                    "09|02|0",
                    "09|03|0",
                    "99|6"
                },
                linhas);
        }

        [Test]
        public void Gerar_QuandoDocumentoNaoTemItens_DeveGerarTipo00ETipo01ComControles()
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
                    "01|NF|000123|10.00",
                    "09|00|1",
                    "09|01|1",
                    "09|02|0",
                    "09|03|0",
                    "99|7"
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
            Assert.That(linhasGeradas.Count(l => l.StartsWith("09|")), Is.EqualTo(4));
            Assert.That(linhasGeradas.Last(), Does.StartWith("99|"));
        }

        private static IEnumerable<string> ConstruirLinhasEsperadas(IEnumerable<Empresa> empresas)
        {
            var qtd00 = 0;
            var qtd01 = 0;
            var qtd02 = 0;
            var qtd03 = 0;

            foreach (var empresa in empresas)
            {
                qtd00++;
                yield return string.Format(
                    CultureInfo.InvariantCulture,
                    "00|{0}|{1}|{2}",
                    empresa.CNPJ,
                    empresa.Nome,
                    empresa.Telefone);

                if (empresa.Documentos == null)
                    continue;

                foreach (var documento in empresa.Documentos)
                {
                    qtd01++;
                    yield return string.Format(
                        CultureInfo.InvariantCulture,
                        "01|{0}|{1}|{2:0.00}",
                        documento.Modelo,
                        documento.Numero,
                        documento.Valor);

                    if (documento.Itens == null)
                        continue;

                    foreach (var item in documento.Itens)
                    {
                        qtd02++;
                        yield return string.Format(
                            CultureInfo.InvariantCulture,
                            "02|{0}|{1:0.00}",
                            item.Descricao,
                            item.Valor);
                    }
                }
            }

            var totalLinhas = qtd00 + qtd01 + qtd02 + qtd03 + 5;

            yield return "09|00|" + qtd00.ToString(CultureInfo.InvariantCulture);
            yield return "09|01|" + qtd01.ToString(CultureInfo.InvariantCulture);
            yield return "09|02|" + qtd02.ToString(CultureInfo.InvariantCulture);
            yield return "09|03|" + qtd03.ToString(CultureInfo.InvariantCulture);
            yield return "99|" + totalLinhas.ToString(CultureInfo.InvariantCulture);
        }
    }
}
