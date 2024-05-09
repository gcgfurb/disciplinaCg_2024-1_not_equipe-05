#define CG_Debug

using System;
using System.Runtime;
using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
  internal class SrPalito : Objeto
  {
    public Ponto4D Start { get; set; }
    public Ponto4D End { get; set; }

    private double angulo;
    private double raio;
    private double dislocaX;

    public SrPalito(Objeto _paiRef, ref char _rotulo) : base(_paiRef, ref _rotulo)
    {
      PrimitivaTipo = PrimitiveType.Lines;
      PrimitivaTamanho = 1;
      Start = new Ponto4D(0, 0);
      End = new Ponto4D(0.35, 0.35);
      dislocaX = 0;
      angulo = 45;
      raio = Matematica.distancia(Start, End);

      base.PontosAdicionar(Start);
      base.PontosAdicionar(End);
      base.ObjetoAtualizar();
    }

    private void Atualizar()
    {
      End = Matematica.GerarPtosCirculo(angulo, raio);
      Start.X = dislocaX;
      End.X += dislocaX;
      base.PontosAlterar(Start, 0);
      base.PontosAlterar(End, 1);
      base.ObjetoAtualizar();
    }

    public void Movimentar(double value)
    {
      dislocaX += value;
      Atualizar();
    }

    public void MudarTamanho(double value)
    {
      raio += value;
      Atualizar();
    }

    public void Girar(double value)
    {
      angulo += value;
      Atualizar();
    }

    // public void GiraVoltaCompleta(double value)
    // {
    //   for (int i = 0; i < 72; i++)
    //   {
    //     angulo += value;
    //     Atualizar();
    //   }
    // }


#if CG_Debug
    public override string ToString()
    {
      string retorno;
      retorno = "__ Objeto SrPalito _ Tipo: " + PrimitivaTipo + " _ Tamanho: " + PrimitivaTamanho + "\n";
      retorno += ImprimeToString();
      return (retorno);
    }
#endif

  }
}
