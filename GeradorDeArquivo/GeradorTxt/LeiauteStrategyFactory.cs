using System;

namespace GeradorTxt
{
    public static class LeiauteStrategyFactory
    {
        public static ILeiauteStrategy Criar(int versao)
        {
            switch (versao)
            {
                case 1:
                    return new Leiaute01Strategy();
                case 2:
                    return new Leiaute02Strategy();
                default:
                    throw new NotSupportedException("Versão de leiaute não suportada: " + versao);
            }
        }
    }
}
