#define CG_Debug

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;

namespace gcgcg
{
  internal class Spline : Objeto
  {
    private double _inc = 0.1;

    public Spline(Objeto _paiRef, ref char _rotulo) : base(_paiRef, ref _rotulo)
    {
      PrimitivaTipo = PrimitiveType.LineStrip;
      PrimitivaTamanho = 1;
      Atualizar();
    }

    public void Atualizar()
    {
      base.pontosLista = [];
      Ponto4D A = paiRef.PontosId(0);
      Ponto4D C = paiRef.PontosId(1);
      Ponto4D D = paiRef.PontosId(2);
      Ponto4D B = paiRef.PontosId(3);

      base.PontosAdicionar(A);
      for (double i = 0.1; i < 1; i += _inc) {
        double PontoAPontoCx = A.X + (C.X - A.X) * i;
        double PontoAPontoCy = A.Y + (C.Y - A.Y) * i;
        double PontoCPontoDx = C.X + (D.X - C.X) * i;
        double PontoCPontoDy = C.Y + (D.Y - C.Y) * i;
        double PontoDPontoBx = D.X + (B.X - D.X) * i;
        double PontoDPontoBy = D.Y + (B.Y - D.Y) * i;

        double R1x = PontoAPontoCx + (PontoCPontoDx - PontoAPontoCx) * i;
        double R1y = PontoAPontoCy + (PontoCPontoDy - PontoAPontoCy) * i;

        double R2x = PontoCPontoDx + (PontoDPontoBx - PontoCPontoDx) * i;
        double R2y = PontoCPontoDy + (PontoDPontoBy - PontoCPontoDy) * i;

        double Rx = R1x + (R2x - R1x) * i;
        double Ry = R1y + (R2y - R1y) * i;
        base.PontosAdicionar(new Ponto4D(Rx, Ry));
      }
      base.PontosAdicionar(B);

      base.ObjetoAtualizar();
    }

    public void SplineQtdPto(double inc) {
      _inc = inc;
      Atualizar();
    }

#if CG_Debug
    public override string ToString()
    {
      string retorno;
      retorno = "__ Objeto Poligono _ Tipo: " + PrimitivaTipo + " _ Tamanho: " + PrimitivaTamanho + "\n";
      retorno += base.ImprimeToString();
      return (retorno);
    }
#endif

  }
}
