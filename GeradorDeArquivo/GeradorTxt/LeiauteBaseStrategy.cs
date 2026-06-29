using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace GeradorTxt
{
    public abstract class LeiauteBaseStrategy : ILeiauteStrategy
    {
        public abstract string Codigo { get; }

        public string GerarConteudo(List<Empresa> empresas)
        {
            var sb = new StringBuilder();

            foreach (var empresa in empresas)
            {
                EscreverTipo00(sb, empresa);

                if (empresa.Documentos == null)
                    continue;

                foreach (var documento in empresa.Documentos)
                {
                    EscreverTipo01(sb, documento);

                    if (documento.Itens == null)
                        continue;

                    foreach (var item in documento.Itens)
                    {
                        EscreverTipo02(sb, item);
                    }
                }
            }

            return sb.ToString();
        }

        protected string ToMoney(decimal val)
        {
            return val.ToString("0.00", CultureInfo.InvariantCulture);
        }

        protected abstract void EscreverTipo00(StringBuilder sb, Empresa empresa);

        protected abstract void EscreverTipo01(StringBuilder sb, Documento documento);

        protected abstract void EscreverTipo02(StringBuilder sb, ItemDocumento item);
    }
}
