using System;
using System.Collections.Generic;
using System.IO;
using quine.Domain;

namespace quine.Infrastructure
{
    public class ArquivoTXT
    {
        private readonly string caminhoArquivo;
        private int numeroVariaveis;

        public ArquivoTXT(string caminhoArquivo)
        {
            this.numeroVariaveis = 0;
            if (Path.IsPathRooted(caminhoArquivo))
                this.caminhoArquivo = caminhoArquivo;
            else if (File.Exists(caminhoArquivo))
                this.caminhoArquivo = Path.GetFullPath(caminhoArquivo);
            else
                this.caminhoArquivo = Path.Combine(App_Path(), caminhoArquivo.TrimStart('\\', '/'));
        }

        private string App_Path()
        {
            return AppContext.BaseDirectory;
        }

        public int PegarNumeroVariaveis()
        {
            var numeroMintermos = PegarNumeroMintermos();

            numeroVariaveis = Convert.ToInt32(Math.Log(numeroMintermos, 2));

            return numeroVariaveis;
        }

        private int PegarNumeroMintermos()
        {
            List<int> dontCares = new List<int>();
            List<int> posicoes = new List<int>();

            SepararTermos(LerConteudo(), posicoes, dontCares);

            int ultimoMintermo = -1;

            foreach (int posicao in posicoes)
            {
                if (posicao > ultimoMintermo)
                    ultimoMintermo = posicao;
            }

            foreach (int dontCare in dontCares)
            {
                if (dontCare > ultimoMintermo)
                    ultimoMintermo = dontCare;
            }

            int numeroMintermos = 1;

            while (numeroMintermos <= ultimoMintermo)
                numeroMintermos *= 2;

            return numeroMintermos;
        }

        public List<Mintermo> CarregarMintermos()
        {
            List<int> dontCares = new List<int>();
            List<int> posicoes = new List<int>();

            SepararTermos(LerConteudo(), posicoes, dontCares);

            return PopularMintermos(posicoes, dontCares);
        }

        private string LerConteudo()
        {
            if (!File.Exists(caminhoArquivo))
                throw new FileNotFoundException("Arquivo de mapa de Karnaugh não encontrado.", caminhoArquivo);

            return File.ReadAllText(caminhoArquivo);
        }

        private static void SepararTermos(string conteudo, List<int> posicoes, List<int> dontCares)
        {
            foreach (string termoOriginal in conteudo.Split(new[] { ';', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string termo = termoOriginal.Trim();

                if (termo.Length == 0)
                    continue;

                bool ehDontCare = termo.StartsWith("-");
                string numero = ehDontCare ? termo.Substring(1).Trim() : termo;

                if (!int.TryParse(numero, out int posicao))
                    throw new FormatException("Termo inválido no mapa de Karnaugh: " + termo);

                if (ehDontCare)
                    dontCares.Add(posicao);
                else
                    posicoes.Add(posicao);
            }
        }

        private List<Mintermo> PopularMintermos(List<int> Mintermos, List<int> DontCares)
        {
            List<Mintermo> ResultadoMintermos = new List<Mintermo>();

            var numeroMintermos = PegarNumeroMintermos();

            for (int i = 0; i < numeroMintermos; i++)
            {
                Mintermo newMintermo = new Mintermo();

                newMintermo.Valor = 0;
                newMintermo.Posicao = i;

                foreach (var mintermo in Mintermos)
                {
                    if (i == mintermo)
                    {
                        newMintermo.Valor = 1;
                    }
                }

                foreach (var dontCare in DontCares)
                {
                    if (i == dontCare)
                    {
                        newMintermo.Valor = 2;
                    }
                }

                ResultadoMintermos.Add(newMintermo);
            }

            ColocarExpressoes(ResultadoMintermos, numeroMintermos);

            return ResultadoMintermos;
        }

        private List<Mintermo> ColocarExpressoes(List<Mintermo> Mintermos, Int32 numeroMintermos)
        {
            List<Mintermo> ResultadoMintermos = Mintermos;

            var contador = 0;
            var contadorAux = 0;
            var quantPulos = 0;

            for (int i = 0; i < Math.Log(numeroMintermos, 2); i++)
            {
                for (int j = 0; j < numeroMintermos; j++)
                {
                    if (contador == 0)
                    {
                        quantPulos = 1;
                        contadorAux += 1;

                        if (contadorAux == quantPulos)
                            Mintermos[j].Variaveis = "0";
                        else
                        {
                            Mintermos[j].Variaveis = "1";
                            contadorAux = 0;
                        }

                    }
                    else if (contador == 1)
                    {
                        quantPulos = 2;
                        contadorAux += 1;

                        if (contadorAux <= quantPulos)
                            Mintermos[j].Variaveis = Mintermos[j].Variaveis.PadLeft(contador + 1, '0');
                        else
                        {
                            Mintermos[j].Variaveis = Mintermos[j].Variaveis.PadLeft(contador + 1, '1');

                            if (contadorAux == quantPulos * 2)
                                contadorAux = 0;
                        }

                    }
                    else
                    {
                        quantPulos = Convert.ToInt32(Math.Pow(2, contador));
                        contadorAux += 1;

                        if (Math.Log(numeroMintermos, 2) == contador)
                        {
                            if (contadorAux <= quantPulos / 2)
                                Mintermos[j].Variaveis = Mintermos[j].Variaveis.PadLeft(contador + 1, '0');
                            else
                                Mintermos[j].Variaveis = Mintermos[j].Variaveis.PadLeft(contador + 1, '1');
                        }
                        else
                        {
                            if (contadorAux <= quantPulos)
                                Mintermos[j].Variaveis = Mintermos[j].Variaveis.PadLeft(contador + 1, '0');
                            else
                            {
                                Mintermos[j].Variaveis = Mintermos[j].Variaveis.PadLeft(contador + 1, '1');

                                if (contadorAux == quantPulos * 2)
                                    contadorAux = 0;
                            }
                        }
                    }
                }

                contador += 1;
            }

            return ResultadoMintermos;
        }
    }
}
