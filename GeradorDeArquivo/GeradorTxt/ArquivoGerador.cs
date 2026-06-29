using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GeradorTxt
{
    public sealed class ArquivoGerador
    {
        public void Gerar(List<Empresa> empresas, string outputPath, ILeiauteStrategy strategy)
        {
            var conteudo = strategy.GerarConteudo(empresas);
            File.WriteAllText(outputPath, conteudo, Encoding.UTF8);
        }
    }
}
