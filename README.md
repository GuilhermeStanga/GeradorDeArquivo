# Gerador de Arquivo

## Descrição

O **Gerador de Arquivo** é uma aplicação desenvolvida em **.NET** que processa uma base de dados e gera arquivos de saída em formatos específicos (leiautes). A aplicação utiliza padrões de projeto como **Strategy** para suportar múltiplos leiautes, tornando o código modular e extensível.

Atualmente, a aplicação suporta dois leiautes principais, com validações e linhas de controle para garantir a integridade dos dados gerados.

---

## Tecnologias Utilizadas

- **.NET Framework/Core**: Plataforma principal para desenvolvimento da aplicação.
- **C#**: Linguagem de programação utilizada.
- **Padrão Strategy**: Implementado para gerenciar diferentes leiautes de forma modular.
- **System.Text**: Biblioteca para manipulação de strings e geração de arquivos.
- **System.Globalization**: Utilizada para formatação de valores monetários e culturais.

---

## Estrutura do Projeto

- **GeradorTxt**: Contém as classes principais, incluindo as estratégias para geração de leiautes.
- **LeiauteBaseStrategy**: Classe base que define a estrutura comum para os leiautes.
- **Leiaute01Strategy e Leiaute02Strategy**: Implementações específicas para os leiautes 01 e 02.
- **Data**: Pasta onde a base de dados é armazenada.
- **Out**: Pasta onde os arquivos gerados são salvos.

---
