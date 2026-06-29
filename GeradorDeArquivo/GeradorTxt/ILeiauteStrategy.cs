using System.Collections.Generic;

namespace GeradorTxt
{
    public interface ILeiauteStrategy
    {
        string Codigo { get; }

        string GerarConteudo(List<Empresa> empresas);
    }
}
