using System.Collections.Generic;

namespace quine.Domain
{
    public class Coluna
    {
        public bool Marcado;

        public string Variaveis;

        public List<int> Mintermos;
        
        public Coluna()
        {
            Mintermos = new List<int>();
            
            Marcado = false;
        }
    }
}
