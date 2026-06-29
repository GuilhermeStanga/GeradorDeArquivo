using System.Text;

namespace GeradorTxt
{
    public class Leiaute01Strategy : LeiauteBaseStrategy
    {
        public override string Codigo
        {
            get { return "01"; }
        }

        protected override void EscreverTipo00(StringBuilder sb, Empresa empresa)
        {
            sb.Append("00").Append("|")
              .Append(empresa.CNPJ).Append("|")
              .Append(empresa.Nome).Append("|")
              .Append(empresa.Telefone).AppendLine();
        }

        protected override void EscreverTipo01(StringBuilder sb, Documento documento)
        {
            sb.Append("01").Append("|")
              .Append(documento.Modelo).Append("|")
              .Append(documento.Numero).Append("|")
              .Append(ToMoney(documento.Valor)).AppendLine();
        }

        protected override void EscreverTipo02(StringBuilder sb, ItemDocumento item)
        {
            sb.Append("02").Append("|")
              .Append(item.Descricao).Append("|")
              .Append(ToMoney(item.Valor)).AppendLine();
        }
    }
}
