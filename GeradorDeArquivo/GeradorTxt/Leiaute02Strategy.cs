using System.Text;

namespace GeradorTxt
{
    public class Leiaute02Strategy : LeiauteBaseStrategy
    {
        public override string Codigo
        {
            get { return "02"; }
        }

        protected override bool EscreverTipo00(StringBuilder sb, Empresa empresa)
        {
            sb.Append("00").Append("|")
              .Append(empresa.CNPJ).Append("|")
              .Append(empresa.Nome).Append("|")
              .Append(empresa.Telefone).AppendLine();
            return true;
        }

        protected override bool EscreverTipo01(StringBuilder sb, Documento documento)
        {
            sb.Append("01").Append("|")
              .Append(documento.Modelo).Append("|")
              .Append(documento.Numero).Append("|")
              .Append(ToMoney(documento.Valor)).AppendLine();
            return true;
        }

        protected override bool EscreverTipo02(StringBuilder sb, ItemDocumento item)
        {
            sb.Append("02").Append("|")
              .Append(item.NumeroItem).Append("|")
              .Append(item.Descricao).Append("|")
              .Append(ToMoney(item.Valor)).AppendLine();
            return true;
        }

        protected override bool EscreverTipo03(StringBuilder sb, CategoriaItem categoria)
        {
            sb.Append("03").Append("|")
              .Append(categoria.NumeroCategoria).Append("|")
              .Append(categoria.DescricaoCategoria).AppendLine();
            return true;
        }
    }
}
