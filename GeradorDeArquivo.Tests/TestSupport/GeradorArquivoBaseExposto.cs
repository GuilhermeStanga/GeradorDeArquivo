using GeradorTxt;
using System.Globalization;
using System.Text;

namespace GeradorDeArquivo.Tests.TestSupport
{
    internal sealed class GeradorArquivoBaseExposto : GeradorArquivoBase
    {
        public string GerarLinhaTipo00(Empresa empresa)
        {
            var sb = new StringBuilder();
            EscreverTipo00(sb, empresa);
            return sb.ToString();
        }

        public string GerarLinhaTipo01(Documento documento)
        {
            var sb = new StringBuilder();
            EscreverTipo01(sb, documento);
            return sb.ToString();
        }

        public string GerarLinhaTipo02(ItemDocumento item)
        {
            var sb = new StringBuilder();
            EscreverTipo02(sb, item);
            return sb.ToString();
        }

        public string FormatarValor(decimal valor)
        {
            return ToMoney(valor);
        }
    }
}
