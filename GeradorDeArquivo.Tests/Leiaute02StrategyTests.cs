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
    public class Leiaute02StrategyTests
    {
        [Test]
        public void GerarConteudo_DeveGerarLinhas02E03ComControles()
        {
            var sut = new Leiaute02Strategy();
            var empresas = new List<Empresa>
            {
                new Empresa
                {
                    CNPJ = "12.345.678/0001-90",
                    Nome = "Empresa Leiaute 2",
                    Telefone = "(11) 99999-1111",
                    Documentos = new List<Documento>
                    {
                        new Documento
                        {
                            Modelo = "NFE",
                            Numero = "000123",
                            Valor = 30m,
                            Itens = new List<ItemDocumento>
                            {
                                new ItemDocumento
                                {
                                    NumeroItem = 1,
                                    Descricao = "Item A",
                                    Valor = 10m,
                                    Categorias = new List<CategoriaItem>
                                    {
                                        new CategoriaItem { NumeroCategoria = 1, DescricaoCategoria = "Categoria 1" },
                                        new CategoriaItem { NumeroCategoria = 2, DescricaoCategoria = "Categoria 2" }
                                    }
                                },
                                new ItemDocumento
                                {
                                    NumeroItem = 2,
                                    Descricao = "Item B",
                                    Valor = 20m,
                                    Categorias = new List<CategoriaItem>
                                    {
                                        new CategoriaItem { NumeroCategoria = 3, DescricaoCategoria = "Categoria 3" }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var conteudo = sut.GerarConteudo(empresas);
            var linhas = conteudo.Split(new[] { "\r\n", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);

            CollectionAssert.AreEqual(
                new[]
                {
                    "00|12.345.678/0001-90|Empresa Leiaute 2|(11) 99999-1111",
                    "01|NFE|000123|30.00",
                    "02|1|Item A|10.00",
                    "03|1|Categoria 1",
                    "03|2|Categoria 2",
                    "02|2|Item B|20.00",
                    "03|3|Categoria 3",
                    "09|00|1",
                    "09|01|1",
                    "09|02|2",
                    "09|03|3",
                    "99|12"
                },
                linhas);
        }

        [Test]
        public void Gerar_UsandoBaseV2_DeveGerarTodasAsLinhasNaOrdemEsperada()
        {
            var empresas = JsonRepository.LoadEmpresas(TestPaths.GetSourceJsonV2Path());
            var sut = new Leiaute02Strategy();
            var outputPath = TestPaths.CreateTemporaryOutputFile();

            new ArquivoGerador().Gerar(empresas, outputPath, sut);

            var linhasGeradas = File.ReadAllLines(outputPath);
            var linhasEsperadas = ConstruirLinhasEsperadas(empresas).ToArray();

            CollectionAssert.AreEqual(linhasEsperadas, linhasGeradas);
            Assert.That(linhasGeradas.Count(l => l.StartsWith("00|")), Is.EqualTo(empresas.Count));
            Assert.That(linhasGeradas.Count(l => l.StartsWith("01|")), Is.EqualTo(empresas.Sum(e => e.Documentos.Count)));
            Assert.That(linhasGeradas.Count(l => l.StartsWith("02|")), Is.EqualTo(empresas.Sum(e => e.Documentos.Sum(d => d.Itens.Count))));
            Assert.That(linhasGeradas.Count(l => l.StartsWith("03|")), Is.EqualTo(empresas.Sum(e => e.Documentos.Sum(d => d.Itens.Sum(i => i.Categorias.Count)))));
            Assert.That(linhasGeradas.Last(), Does.StartWith("99|"));
        }

        [Test]
        public void Fbrica_DeveSelecionarLeiauteCorreto()
        {
            Assert.That(LeiauteStrategyFactory.Criar(1), Is.TypeOf<Leiaute01Strategy>());
            Assert.That(LeiauteStrategyFactory.Criar(2), Is.TypeOf<Leiaute02Strategy>());
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
                            "02|{0}|{1}|{2:0.00}",
                            item.NumeroItem,
                            item.Descricao,
                            item.Valor);

                        if (item.Categorias == null)
                            continue;

                        foreach (var categoria in item.Categorias)
                        {
                            qtd03++;
                            yield return string.Format(
                                CultureInfo.InvariantCulture,
                                "03|{0}|{1}",
                                categoria.NumeroCategoria,
                                categoria.DescricaoCategoria);
                        }
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
