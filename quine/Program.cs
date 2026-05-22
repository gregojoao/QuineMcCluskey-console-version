using System;
using System.Collections.Generic;

namespace quine
{
    class Program
    {
        private static int numVariaveis;

        private static string nomeArquivo = "MapaKarnaugh.txt";

        static void Main(string[] args)
        {
            if (args.Length > 0)
                nomeArquivo = args[0];

            // Carrega todos os Mintermos e Don't Cares do arquivo TXT
            List<Mintermo> ColunaMintermos = CarregarMintermosDoTXT();

            // Executa o Método responsável pelo Quine McCluskey
            QuineMcCluskey(ColunaMintermos);

            Console.ReadLine();
        }

        private static void QuineMcCluskey(List<Mintermo> ColunaMintermos)
        {
            Console.WriteLine("Algoritmo de Quine McCluskey");

            // Cria uma Coluna/lista de Mintermos ordenados por número de 1's
            List<List<Mintermo>> ColunaMintermosAgrupados = CriaMatrizColunasMintermosCheia(ColunaMintermos);

            // Cria uma Matriz Tridimensional de Colunas com Mintermos para comparações
            List<List<List<Coluna>>> MatrizColunasComparacao = CriaMatrizColunasComparacaoVazia(ColunaMintermosAgrupados); /* Aloca memória */

            // Roda o algoritmo de comparações e retorna uma lista de Expressões não simplificadas a serem transportadas para a Tabela de Cobertura
            List<Coluna> ExpressoesNaoSimplificadas = RodaAlgoritmoComparacao(ColunaMintermosAgrupados, MatrizColunasComparacao);

            // Transporta as Expressões não simplificadas para a Tabela de Cobertura e Imprime o resultado na Tela
            TransportaParaTabelaCobertura(ColunaMintermos, ExpressoesNaoSimplificadas);
        }

        private static void ImprimeMintermosAgrupados(List<List<Mintermo>> ColunaMintermosAgrupados)
        {
            // Imprime na Tela os Conjuntos de Mintermos agrupados por quantidade de 1's 

            Console.WriteLine();
            Console.WriteLine("*******************************************************************");
            Console.WriteLine("Conjuntos de Mintermos agrupados por quantidade de 1's:  0 | 1 | 2 ");

            for (int numeroMintermo = 0; numeroMintermo < ColunaMintermosAgrupados.Count; numeroMintermo++)
            {
                Console.WriteLine();

                foreach (var mintermo in ColunaMintermosAgrupados[numeroMintermo])
                {
                    Console.WriteLine("'" + numeroMintermo + "' - " + mintermo.Variaveis + "(" + mintermo.Posicao + ")");
                }

            }

            Console.WriteLine("-------------------------------------------------------------------");
        }

        private static void ImprimeMatrizColunasComparacao(List<List<List<Coluna>>> MatrizColunasComparacao)
        {
            // Imprime na Tela as Colunas de Comparação com seus Mintermos

            string alfabeto = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            Console.WriteLine();
            Console.WriteLine("*******************************************************************");
            Console.WriteLine("Colunas de Comparações por nível na Matriz:  > - > - >");

            for (int i = 0; i < MatrizColunasComparacao.Count; i++)
            {
                for (int j = 0; j < MatrizColunasComparacao[i].Count; j++)
                {

                    for (int k = 0; k < MatrizColunasComparacao[i][j].Count; k++)
                    {
                        Console.WriteLine();
                        Console.Write(alfabeto[i].ToString() + j + " - " + MatrizColunasComparacao[i][j][k].Variaveis);

                        foreach (var mintermo in MatrizColunasComparacao[i][j][k].Mintermos)
                        {
                            Console.Write(" (" + mintermo + ")");
                        }
                    }

                    Console.WriteLine();
                }

                Console.WriteLine("-------------------------------------------------------------------");
            }

        }

        private static void ImprimeExpressoesNaoSimplificadas(List<Coluna> ExpressoesNaoSimplificadas)
        {
            // Imprime na Tela todas as Expressões não simplificadas resultante das comparações

            string alfabeto = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Boolean ehPrimeiro = true;

            Console.WriteLine();
            Console.WriteLine("*******************************************************************");
            Console.WriteLine("Resultado Parcial: Expressões não simplificadas  -> ->");
            Console.WriteLine();

            foreach (var expressao in ExpressoesNaoSimplificadas)
            {
                Console.Write(expressao.Variaveis + " (");
                
                for (int i = 0; i < expressao.Variaveis.Length; i++)
                {
                    if (!ehPrimeiro && expressao.Variaveis[i].ToString() != "_")
                        Console.Write(" + ");

                    if (expressao.Variaveis[i].ToString() != "_")
                    {
                        if (expressao.Variaveis[i].ToString() == "0")
                        {
                            ehPrimeiro = false;
                            Console.Write("!" + alfabeto[i].ToString());
                        }
                        else
                        {
                            ehPrimeiro = false;
                            Console.Write(alfabeto[i].ToString());
                        }
                    }

                    if (i == expressao.Variaveis.Length - 1)
                        Console.Write(")");
                }
                
                ehPrimeiro = true;
                Console.WriteLine();
            }

            ImprimeTextoTransportando();
        }

        private static void ImprimeTextoTransportando()
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Transportando para Tabela de Cobertura:  -> -> / ->");
        }

        private static void ImprimeExpressoesSimplificadas(List<Coluna> ExpressoesSimplificadas)
        {
            // Imprime na Tela todas as Expressões simplificadas resultante do transporte da Tabela de Cobertura

            string alfabeto = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Boolean ehPrimeiro = true;

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Resultado Final: Expressões simplificadas  ->");
            Console.WriteLine();

            foreach (var expressao in ExpressoesSimplificadas)
            {
                if (expressao.Marcado)
                {
                    Console.Write(expressao.Variaveis + " (");

                    for (int i = 0; i < expressao.Variaveis.Length; i++)
                    {
                        if (!ehPrimeiro && expressao.Variaveis[i].ToString() != "_")
                            Console.Write(" + ");

                        if (expressao.Variaveis[i].ToString() != "_")
                        {
                            if (expressao.Variaveis[i].ToString() == "0")
                            {
                                ehPrimeiro = false;
                                Console.Write("!" + alfabeto[i].ToString() );
                            }
                            else
                            {
                                ehPrimeiro = false;
                                Console.Write(alfabeto[i].ToString());
                            }
                        }

                        if (i == expressao.Variaveis.Length - 1)
                            Console.Write(")");
                    }

                    ehPrimeiro = true;
                    Console.WriteLine();
                }
            }

            Console.WriteLine("-------------------------------------------------------------------");
        }

        private static void TransportaParaTabelaCobertura(List<Mintermo> ColunaMintermos, List<Coluna> ExpressoesNaoSimplificadas)
        {
            // Coloca na Tabela de Cobertura todas as Expressões não simplificadas, e então separa aquelas que já são cobertas por outras Expressões

            List<int> TabelaCobertura = new List<int>();
            bool estaCoberto = false;

            foreach (var expressao in ExpressoesNaoSimplificadas)
            {
                foreach (var mintermo in expressao.Mintermos)
                {
                    estaCoberto = TabelaCobertura.Contains(mintermo);

                    if (estaCoberto == false)
                    {
                        if (ColunaMintermos[mintermo].Valor == 1)
                            TabelaCobertura.Add(mintermo);
                    }
                }
            }

            List<int> DontCares = new List<int>();

            bool ehDontCare = true;
            foreach (var expressao in ExpressoesNaoSimplificadas)
            {
                foreach (var mintermo in expressao.Mintermos)
                {
                    ehDontCare = true;
                    foreach (var mintermoCobertura in TabelaCobertura)
                    {
                        if (mintermo == mintermoCobertura)
                            ehDontCare = false;
                    }

                    if (ehDontCare == true)
                    {
                        if (!DontCares.Contains(mintermo))
                            DontCares.Add(mintermo);
                    }
                }

            }

            foreach (var expressao in ExpressoesNaoSimplificadas)
            {
                expressao.Mintermos.RemoveAll(mintermo => DontCares.Contains(mintermo));
            }

            List<Coluna> Conjuntos = new List<Coluna>();

            while (ExpressoesNaoSimplificadas.Count > 0)
            {
                int indiceMaior = 0;

                for (int i = 1; i < ExpressoesNaoSimplificadas.Count; i++)
                {
                    if (ExpressoesNaoSimplificadas[i].Mintermos.Count > ExpressoesNaoSimplificadas[indiceMaior].Mintermos.Count)
                        indiceMaior = i;
                }

                Conjuntos.Add(ExpressoesNaoSimplificadas[indiceMaior]);
                ExpressoesNaoSimplificadas.RemoveAt(indiceMaior);
            }

            ExpressoesNaoSimplificadas = Conjuntos;
            TabelaCobertura.Sort();

            foreach (var expressao in ExpressoesNaoSimplificadas)
            {
                foreach (var mintermo in expressao.Mintermos)
                {
                    if (TabelaCobertura.Contains(mintermo))
                    {
                        expressao.Marcado = true;
                        TabelaCobertura.Remove(mintermo);
                    }
                }
            }

            List<Coluna> ExpressoesSimplificadas = ExpressoesNaoSimplificadas;

            ImprimeExpressoesSimplificadas(ExpressoesSimplificadas);
        }

        private static List<Mintermo> CarregarMintermosDoTXT()
        {
            ArquivoTXT arquivo = new ArquivoTXT(nomeArquivo);

            List<Mintermo> ColunaMintermos = arquivo.CarregarMintermos();

            numVariaveis = arquivo.PegarNumeroVariaveis();

            return ColunaMintermos;
        }

        private static List<List<Mintermo>> CriaMatrizColunasMintermosCheia(List<Mintermo> ColunaMintermos)
        {
            List<List<Mintermo>> ColunaMintermosAgrupados = new List<List<Mintermo>>();

            // '<=' Porque são feitas colunas para os mintermos que não tem 1's
            for (int i = 0; i <= numVariaveis; i++)
            {
                List<Mintermo> listaColunas = new List<Mintermo>();

                ColunaMintermosAgrupados.Add(listaColunas);
            }

            // Preenche a Matriz de Mintermos de acordo com os 1's
            foreach (var mintermo in ColunaMintermos)
            {
                if (mintermo.Valor == 1 || mintermo.Valor == 2)
                {
                    short contadorUnsMintermo = 0;

                    foreach (char caracter in mintermo.Variaveis)
                    {
                        if (caracter == '1')
                        {
                            contadorUnsMintermo += 1;
                        }
                    }

                    ColunaMintermosAgrupados[contadorUnsMintermo].Add(mintermo);
                }
            }

            ImprimeMintermosAgrupados(ColunaMintermosAgrupados); /* Imprime as Colunas de Mintermos de forma agrupada no Console */

            return ColunaMintermosAgrupados;
        }

        private static List<Coluna> RodaAlgoritmoComparacao(List<List<Mintermo>> ColunaMintermosAgrupados, List<List<List<Coluna>>> MatrizColunasComparacao)
        {
            var numeroConjuntos = ColunaMintermosAgrupados.Count - 1;

            for (int i = 0; i < ColunaMintermosAgrupados.Count - 1; i++)
            {
                for (int j = 0; j < numeroConjuntos; j++)
                {
                    List<Coluna> ListaConjuntos = new List<Coluna>();
                    MatrizColunasComparacao[i].Add(ListaConjuntos);
                }

                numeroConjuntos -= 1;
            }

            // For que preenche a primeira Coluna da Matriz, a partir das Colunas de 1's
            for (int i = 0; i < ColunaMintermosAgrupados.Count; i++)
            {
                if (i + 1 < ColunaMintermosAgrupados.Count)
                {
                    foreach (Mintermo mintermo in ColunaMintermosAgrupados[i])
                    {
                        foreach (Mintermo mintermoAux in ColunaMintermosAgrupados[i + 1])
                        {
                            string variaveisAux = "";
                            short contador = 0;

                            for (int j = 0; j < numVariaveis; j++)
                            {
                                var caracter = mintermo.Variaveis.Substring(j, 1);
                                var caracterAux = mintermoAux.Variaveis.Substring(j, 1);

                                if (caracter == caracterAux)
                                {
                                    variaveisAux += caracter;
                                }
                                else
                                {
                                    variaveisAux += "_";
                                    contador += 1;
                                }
                            }

                            if (contador == 1)
                            {
                                Coluna coluna = new Coluna();
                                coluna.Variaveis = variaveisAux;

                                coluna.Mintermos.Add(mintermo.Posicao);
                                coluna.Mintermos.Add(mintermoAux.Posicao);

                                MatrizColunasComparacao[0][i].Add(coluna);
                            }
                        }
                    }

                }
            }

            var numeroDiferencas = 1;

            for (int i = 0; i < MatrizColunasComparacao.Count - 1; i++) // For mais externo, de acordo com o número de colunas..
            {
                for (int j = 0; j < MatrizColunasComparacao[i].Count; j++)
                {
                    if (j + 1 < MatrizColunasComparacao[i].Count)
                    {
                        for (int k = 0; k < MatrizColunasComparacao[i][j].Count; k++)
                        {
                            var ListaMintermos = MatrizColunasComparacao[i][j][k];

                            for (int h = 0; h < MatrizColunasComparacao[i][j + 1].Count; h++)
                            {
                                var ColunaMintermosAux = MatrizColunasComparacao[i][j + 1][h];

                                string variaveisAux = "";
                                short contador = 0;

                                for (int quantCaracteres = 0; quantCaracteres < numVariaveis; quantCaracteres++)
                                {
                                    var caracter = ListaMintermos.Variaveis.Substring(quantCaracteres, 1);
                                    var caracterAux = ColunaMintermosAux.Variaveis.Substring(quantCaracteres, 1);

                                    if (caracter == caracterAux)
                                    {
                                        variaveisAux += caracter;
                                    }
                                    else
                                    {
                                        variaveisAux += "_";
                                        contador += 1;
                                    }
                                }

                                if (contador == numeroDiferencas)
                                {
                                    ListaMintermos.Marcado = true;
                                    ColunaMintermosAux.Marcado = true;

                                    Coluna coluna = new Coluna();
                                    coluna.Variaveis = variaveisAux;

                                    var naoTem = false;

                                    foreach (var item in MatrizColunasComparacao[i + 1][j])
                                    {
                                        if (variaveisAux == item.Variaveis)
                                        {
                                            naoTem = true;
                                        }
                                    }

                                    if (!naoTem)
                                    {
                                        foreach (var mintermo in ListaMintermos.Mintermos)
                                        {
                                            coluna.Mintermos.Add(mintermo);
                                        }

                                        foreach (var mintermoAux in ColunaMintermosAux.Mintermos)
                                        {
                                            coluna.Mintermos.Add(mintermoAux);
                                        }

                                        MatrizColunasComparacao[i + 1][j].Add(coluna);
                                    }
                                }
                            }
                        }
                    }
                }
            }


            MatrizColunasComparacao = DeletaColunasVazias(MatrizColunasComparacao);

            ImprimeMatrizColunasComparacao(MatrizColunasComparacao);


            List<Coluna> ExpressoesNaoSimplificadas = new List<Coluna>();

            foreach (var listas in MatrizColunasComparacao)
            {
                foreach (var mintermos in listas)
                {
                    foreach (var item in mintermos)
                    {
                        if (!item.Marcado)
                        {
                            ExpressoesNaoSimplificadas.Add(item);
                        }
                    }
                }
            }

            ImprimeExpressoesNaoSimplificadas(ExpressoesNaoSimplificadas);

            return ExpressoesNaoSimplificadas;
        }

        private static List<List<List<Coluna>>> CriaMatrizColunasComparacaoVazia(List<List<Mintermo>> ColunaMintermosAgrupados)
        {
            List<List<List<Coluna>>> MatrizColunasComparacao = new List<List<List<Coluna>>>();

            for (int i = 0; i < ColunaMintermosAgrupados.Count - 1; i++)
            {
                List<List<Coluna>> listaColunas = new List<List<Coluna>>();

                MatrizColunasComparacao.Add(listaColunas);
            }

            return MatrizColunasComparacao;
        }

        private static List<List<List<Coluna>>> DeletaColunasVazias(List<List<List<Coluna>>> MatrizColunasComparacao)
        {
            for (int i = MatrizColunasComparacao.Count - 1; i >= 0; i--)
            {
                for (int j = MatrizColunasComparacao[i].Count - 1; j >= 0; j--)
                {
                    MatrizColunasComparacao[i][j].RemoveAll(coluna => coluna.Mintermos.Count == 0);

                    if (MatrizColunasComparacao[i][j].Count == 0)
                        MatrizColunasComparacao[i].RemoveAt(j);
                }

                if (MatrizColunasComparacao[i].Count == 0)
                    MatrizColunasComparacao.RemoveAt(i);
            }

            return MatrizColunasComparacao;
        }
    }
}
