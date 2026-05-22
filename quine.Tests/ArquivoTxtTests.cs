using quine.Infrastructure;

namespace quine.Tests;

public class ArquivoTxtTests
{
    [Fact]
    public void CarregarMintermos_ReadsMintermsAndDontCares()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "TestData", "MapaKarnaugh.txt");

        ArquivoTXT arquivo = new ArquivoTXT(path);
        var mintermos = arquivo.CarregarMintermos();

        Assert.Equal(5, arquivo.PegarNumeroVariaveis());
        Assert.Equal(32, mintermos.Count);
        Assert.Equal(1, mintermos[0].Valor);
        Assert.Equal(1, mintermos[16].Valor);
        Assert.Equal(2, mintermos[17].Valor);
        Assert.Equal(0, mintermos[18].Valor);
        Assert.Equal(2, mintermos[29].Valor);
    }

    [Fact]
    public void CarregarMintermos_AcceptsContentWithoutTrailingSemicolon()
    {
        string path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.txt");

        try
        {
            File.WriteAllText(path, "0;1;-7");

            ArquivoTXT arquivo = new ArquivoTXT(path);
            var mintermos = arquivo.CarregarMintermos();

            Assert.Equal(3, arquivo.PegarNumeroVariaveis());
            Assert.Equal(8, mintermos.Count);
            Assert.Equal(1, mintermos[0].Valor);
            Assert.Equal(1, mintermos[1].Valor);
            Assert.Equal(2, mintermos[7].Valor);
        }
        finally
        {
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}
