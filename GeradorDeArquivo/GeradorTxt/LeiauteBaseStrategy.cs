using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace GeradorTxt
{
    public abstract class LeiauteBaseStrategy : ILeiauteStrategy
    {
        private const int QuantidadeTiposControle = 4;

        public abstract string Codigo { get; }

        public string GerarConteudo(List<Empresa> empresas)
        {
            if (empresas == null)
                throw new ArgumentNullException(nameof(empresas));

            var sb = new StringBuilder();
            var contagemTipos = new int[QuantidadeTiposControle];

            foreach (var empresa in empresas)
            {
                if (EscreverTipo00(sb, empresa))
                {
                    contagemTipos[0]++;
                }

                if (empresa.Documentos == null)
                    continue;

                foreach (var documento in empresa.Documentos)
                {
                    if (EscreverTipo01(sb, documento))
                    {
                        contagemTipos[1]++;
                    }

                    if (documento.Itens == null)
                        continue;

                    foreach (var item in documento.Itens)
                    {
                        if (EscreverTipo02(sb, item))
                        {
                            contagemTipos[2]++;
                        }

                        if (item.Categorias == null)
                            continue;

                        foreach (var categoria in item.Categorias)
                        {
                            if (EscreverTipo03(sb, categoria))
                            {
                                contagemTipos[3]++;
                            }
                        }
                    }
                }
            }

            EscreverLinhasControle(sb, contagemTipos);

            return sb.ToString();
        }

        protected string ToMoney(decimal val)
        {
            return val.ToString("0.00", CultureInfo.InvariantCulture);
        }

        protected abstract bool EscreverTipo00(StringBuilder sb, Empresa empresa);

        protected abstract bool EscreverTipo01(StringBuilder sb, Documento documento);

        protected abstract bool EscreverTipo02(StringBuilder sb, ItemDocumento item);

        protected virtual bool EscreverTipo03(StringBuilder sb, CategoriaItem categoria)
        {
            return false;
        }

        private void EscreverLinhasControle(StringBuilder sb, IReadOnlyList<int> contagemTipos)
        {
            var totalLinhasDados = contagemTipos.Sum();
            var totalLinhasArquivo = totalLinhasDados + QuantidadeTiposControle + 1;

            for (var tipo = 0; tipo < QuantidadeTiposControle; tipo++)
            {
                sb.Append("09").Append("|")
                  .Append(tipo.ToString("00", CultureInfo.InvariantCulture)).Append("|")
                  .Append(contagemTipos[tipo].ToString(CultureInfo.InvariantCulture)).AppendLine();
            }

            sb.Append("99").Append("|")
              .Append(totalLinhasArquivo.ToString(CultureInfo.InvariantCulture)).AppendLine();
        }
    }
}
