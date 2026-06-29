using System.Collections.Generic;

namespace GeradorTxt
{
    /// <summary>
    /// Compatibilidade com o código anterior.
    /// O leiaute 01 agora é implementado como estratégia reutilizável.
    /// </summary>
    public class GeradorArquivoBase : Leiaute01Strategy
    {
        public void Gerar(List<Empresa> empresas, string outputPath)
        {
            new ArquivoGerador().Gerar(empresas, outputPath, this);
        }
    }
}
