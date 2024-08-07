﻿#define CG_DEBUG
#define CG_Gizmo      
#define CG_OpenGL      
// #define CG_OpenTK
// #define CG_DirectX      
#define CG_Privado  

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using System;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace gcgcg
{
  public class Mundo : GameWindow
  {
    private static Objeto mundo = null;

    private char rotuloAtual = '?';
    private Objeto objetoSelecionado = null;
    private int qtdObjetos = 0;
    private Poligono polignoEmAndamento = null;
    private int novoPonto = 1;
    private Ponto4D sruPonto = null;

    private readonly float[] _sruEixos =
    [
       -0.5f,  0.0f,  0.0f, /* X- */      0.5f,  0.0f,  0.0f, /* X+ */
       0.0f,  -0.5f,  0.0f, /* Y- */      0.0f,  0.5f,  0.0f, /* Y+ */
       0.0f,  0.0f,  0.0f, /* Z- */      0.0f,  0.0f,  0.0f  /* Z+ */
    ];

    private int _vertexBufferObject_sruEixos;
    private int _vertexArrayObject_sruEixos;

    private int _vertexBufferObject_bbox;
    private int _vertexArrayObject_bbox;

    private Shader _shaderBranca;
    private Shader _shaderVermelha;
    private Shader _shaderVerde;
    private Shader _shaderAzul;
    private Shader _shaderCiano;
    private Shader _shaderMagenta;
    private Shader _shaderAmarela;

    public Mundo(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
      : base(gameWindowSettings, nativeWindowSettings)
    {
      mundo ??= new Objeto(null, ref rotuloAtual); //padrão Singleton
    }

    protected override void OnLoad()
    {
      base.OnLoad();

      Utilitario.Diretivas();
#if CG_DEBUG      
      Console.WriteLine("Tamanho interno da janela de desenho: " + ClientSize.X + "x" + ClientSize.Y);
#endif

      GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

      #region Cores
      _shaderBranca = new Shader("Shaders/shader.vert", "Shaders/shaderBranca.frag");
      _shaderVermelha = new Shader("Shaders/shader.vert", "Shaders/shaderVermelha.frag");
      _shaderVerde = new Shader("Shaders/shader.vert", "Shaders/shaderVerde.frag");
      _shaderAzul = new Shader("Shaders/shader.vert", "Shaders/shaderAzul.frag");
      _shaderCiano = new Shader("Shaders/shader.vert", "Shaders/shaderCiano.frag");
      _shaderMagenta = new Shader("Shaders/shader.vert", "Shaders/shaderMagenta.frag");
      _shaderAmarela = new Shader("Shaders/shader.vert", "Shaders/shaderAmarela.frag");
      #endregion

      #region Eixos: SRU  
      _vertexBufferObject_sruEixos = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_sruEixos);
      GL.BufferData(BufferTarget.ArrayBuffer, _sruEixos.Length * sizeof(float), _sruEixos, BufferUsageHint.StaticDraw);
      _vertexArrayObject_sruEixos = GL.GenVertexArray();
      GL.BindVertexArray(_vertexArrayObject_sruEixos);
      GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
      GL.EnableVertexAttribArray(0);
      #endregion
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
      base.OnRenderFrame(e);

      GL.Clear(ClearBufferMask.ColorBufferBit);

      mundo.Desenhar(new Transformacao4D());

#if CG_Gizmo      
      Gizmo_Sru3D();
      if (objetoSelecionado != null) objetoSelecionado.Bbox().Desenhar();
#endif
      SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
      base.OnUpdateFrame(e);

      #region Teclado
      var estadoTeclado = KeyboardState;
      if (estadoTeclado.IsKeyDown(Keys.Escape))
        Close();
      if (estadoTeclado.IsKeyPressed(Keys.Space))
      {
        if (objetoSelecionado == null)
        {
          objetoSelecionado = mundo;

        }
        objetoSelecionado = mundo.GrafocenaBuscaProximo(objetoSelecionado);
      }
      if (estadoTeclado.IsKeyPressed(Keys.P) && objetoSelecionado != null)
      {
        if (objetoSelecionado.PrimitivaTipo == PrimitiveType.LineLoop) objetoSelecionado.PrimitivaTipo = PrimitiveType.LineStrip;
        else objetoSelecionado.PrimitivaTipo = PrimitiveType.LineLoop;
      }
      if (estadoTeclado.IsKeyPressed(Keys.M) && objetoSelecionado != null)
      {
        objetoSelecionado.MatrizImprimir();
      }
      if (estadoTeclado.IsKeyPressed(Keys.I) && objetoSelecionado != null)
      {
        objetoSelecionado.MatrizAtribuirIdentidade();
      }
      if (estadoTeclado.IsKeyPressed(Keys.Left) && objetoSelecionado != null)
      {
        objetoSelecionado.MatrizTranslacaoXYZ(-0.05, 0, 0);
      }
      if (estadoTeclado.IsKeyPressed(Keys.Right) && objetoSelecionado != null)
      {
        objetoSelecionado.MatrizTranslacaoXYZ(0.05, 0, 0);
      }
      if (estadoTeclado.IsKeyPressed(Keys.Up) && objetoSelecionado != null)
      {
        objetoSelecionado.MatrizTranslacaoXYZ(0, 0.05, 0);
      }
      if (estadoTeclado.IsKeyPressed(Keys.Down) && objetoSelecionado != null)
      {
        objetoSelecionado.MatrizTranslacaoXYZ(0, -0.05, 0);
      }
      if (estadoTeclado.IsKeyPressed(Keys.PageUp) && objetoSelecionado != null)
      {
        objetoSelecionado.MatrizEscalaXYZ(2, 2, 2);
      }
      if (estadoTeclado.IsKeyPressed(Keys.PageDown) && objetoSelecionado != null)
      {
        objetoSelecionado.MatrizEscalaXYZ(0.5, 0.5, 0.5);
      }
      if (estadoTeclado.IsKeyPressed(Keys.Home) && objetoSelecionado != null)
      {
        objetoSelecionado.MatrizEscalaXYZBBox(0.5, 0.5, 0.5);
      }
      if (estadoTeclado.IsKeyPressed(Keys.End) && objetoSelecionado != null)
      {
        objetoSelecionado.MatrizEscalaXYZBBox(2, 2, 2);
      }
      if (estadoTeclado.IsKeyPressed(Keys.D3) && objetoSelecionado != null)
      {
        objetoSelecionado.MatrizRotacaoZBBox(10);
      }
      if (estadoTeclado.IsKeyPressed(Keys.D4) && objetoSelecionado != null)
      {
        objetoSelecionado.MatrizRotacaoZBBox(-10);
      }
      if (estadoTeclado.IsKeyPressed(Keys.Enter))
      {
        objetoSelecionado = polignoEmAndamento;
        qtdObjetos++;
        polignoEmAndamento = null;
        novoPonto = 1;
      }
      if (estadoTeclado.IsKeyPressed(Keys.D) && objetoSelecionado != null)
      {
        objetoSelecionado.PontosLimpar();
        objetoSelecionado = null;
      }
      if (estadoTeclado.IsKeyPressed(Keys.R) && objetoSelecionado != null)
      {
        objetoSelecionado.ShaderObjeto = _shaderVermelha;
      }
      if (estadoTeclado.IsKeyPressed(Keys.G) && objetoSelecionado != null)
      {
        objetoSelecionado.ShaderObjeto = _shaderVerde;
      }
      if (estadoTeclado.IsKeyPressed(Keys.B) && objetoSelecionado != null)
      {
        objetoSelecionado.ShaderObjeto = _shaderAzul;
      }

      #endregion

      #region  Mouse
      int janelaLargura = ClientSize.X;
      int janelaAltura = ClientSize.Y;
      Ponto4D mousePonto = new Ponto4D(MousePosition.X, MousePosition.Y);
      Ponto4D newPTO = Utilitario.NDC_TelaSRU(janelaLargura, janelaAltura, mousePonto);

      if (estadoTeclado.IsKeyDown(Keys.V))
      {
        double menor = Matematica.Distancia(newPTO, objetoSelecionado.PontosId(0));
        int idxToChange = 0;

        for (int j = 1; j < objetoSelecionado.PontosListaTamanho; j++)
        {
          double distancia = Matematica.Distancia(newPTO, objetoSelecionado.PontosId(j));
          if (distancia < menor)
          {
            menor = distancia;
            idxToChange = j;
          }
        }

        objetoSelecionado.PontosAlterar(newPTO, idxToChange);
      }
      if (estadoTeclado.IsKeyPressed(Keys.E))
      {
        double menor = Matematica.Distancia(newPTO, objetoSelecionado.PontosId(0));
        int idxToChange = 0;
        for (int j = 1; j < objetoSelecionado.PontosListaTamanho; j++)
        {
          double distancia = Matematica.Distancia(newPTO, objetoSelecionado.PontosId(j));
          if (distancia < menor)
          {
            menor = distancia;

            idxToChange = j;
          }
        }
        objetoSelecionado.PontoRemover(idxToChange);
      }
      if (MouseState.IsButtonPressed(MouseButton.Left))
      {
        for (int i = 0; i <= qtdObjetos; i++)
        {
          if (objetoSelecionado == null) objetoSelecionado = mundo;

          objetoSelecionado = mundo.GrafocenaBuscaProximo(objetoSelecionado);
          if (objetoSelecionado.Bbox().Dentro(newPTO))
          {
            Ponto4D pontoAux = objetoSelecionado.PontosId(0);
            for (int j = 1; j < objetoSelecionado.PontosListaTamanho; j++)
            {
              if (Matematica.ScanLine(newPTO, pontoAux, objetoSelecionado.PontosId(j)))
              {
                return;
              }
              pontoAux = objetoSelecionado.PontosId(j);
            }
          }
        }
        objetoSelecionado = null;
      }
      if (MouseState.IsButtonDown(MouseButton.Right))
      {
        sruPonto = newPTO;

        if (polignoEmAndamento != null)
        {
          polignoEmAndamento.PontosAlterar(sruPonto, novoPonto);
        }
        else
        {
          List<Ponto4D> pontosPoligono = new List<Ponto4D>();
          pontosPoligono.Add(sruPonto);
          pontosPoligono.Add(sruPonto);
          if (objetoSelecionado != null) polignoEmAndamento = new Poligono(objetoSelecionado, ref rotuloAtual, pontosPoligono);
          else polignoEmAndamento = new Poligono(mundo, ref rotuloAtual, pontosPoligono);
        }
      }
      if (MouseState.IsButtonReleased(MouseButton.Right))
      {
        if (polignoEmAndamento != null)
        {
          polignoEmAndamento.PontosAdicionar(sruPonto);
          novoPonto++;
        }
      }
      #endregion
    }

    protected override void OnResize(ResizeEventArgs e)
    {
      base.OnResize(e);

#if CG_DEBUG      
      Console.WriteLine("Tamanho interno da janela de desenho: " + ClientSize.X + "x" + ClientSize.Y);
#endif
      GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
    }

    protected override void OnUnload()
    {
      mundo.OnUnload();

      GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
      GL.BindVertexArray(0);
      GL.UseProgram(0);

      GL.DeleteBuffer(_vertexBufferObject_sruEixos);
      GL.DeleteVertexArray(_vertexArrayObject_sruEixos);

      GL.DeleteBuffer(_vertexBufferObject_bbox);
      GL.DeleteVertexArray(_vertexArrayObject_bbox);

      GL.DeleteProgram(_shaderBranca.Handle);
      GL.DeleteProgram(_shaderVermelha.Handle);
      GL.DeleteProgram(_shaderVerde.Handle);
      GL.DeleteProgram(_shaderAzul.Handle);
      GL.DeleteProgram(_shaderCiano.Handle);
      GL.DeleteProgram(_shaderMagenta.Handle);
      GL.DeleteProgram(_shaderAmarela.Handle);

      base.OnUnload();
    }

#if CG_Gizmo
    private void Gizmo_Sru3D()
    {
#if CG_OpenGL && !CG_DirectX
      var transform = Matrix4.Identity;
      GL.BindVertexArray(_vertexArrayObject_sruEixos);
      // EixoX
      _shaderVermelha.SetMatrix4("transform", transform);
      _shaderVermelha.Use();
      GL.DrawArrays(PrimitiveType.Lines, 0, 2);
      // EixoY
      _shaderVerde.SetMatrix4("transform", transform);
      _shaderVerde.Use();
      GL.DrawArrays(PrimitiveType.Lines, 2, 2);
      // EixoZ
      _shaderAzul.SetMatrix4("transform", transform);
      _shaderAzul.Use();
      GL.DrawArrays(PrimitiveType.Lines, 4, 2);
#elif CG_DirectX && !CG_OpenGL
      Console.WriteLine(" .. Coloque aqui o seu código em DirectX");
#elif (CG_DirectX && CG_OpenGL) || (!CG_DirectX && !CG_OpenGL)
      Console.WriteLine(" .. ERRO de Render - escolha OpenGL ou DirectX !!");
#endif
    }
#endif    

#if CG_Gizmo
    private void Gizmo_BBox()   //FIXME: não é atualizada com as transformações globais
    {
      if (objetoSelecionado != null)
      {

#if CG_OpenGL && !CG_DirectX

        float[] _bbox =
        {
        (float) objetoSelecionado.Bbox().ObterMenorX, (float) objetoSelecionado.Bbox().ObterMenorY, 0.0f, // A
        (float) objetoSelecionado.Bbox().ObterMaiorX, (float) objetoSelecionado.Bbox().ObterMenorY, 0.0f, // B
        (float) objetoSelecionado.Bbox().ObterMaiorX, (float) objetoSelecionado.Bbox().ObterMaiorY, 0.0f, // C
        (float) objetoSelecionado.Bbox().ObterMenorX, (float) objetoSelecionado.Bbox().ObterMaiorY, 0.0f  // D
      };

        _vertexBufferObject_bbox = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_bbox);
        GL.BufferData(BufferTarget.ArrayBuffer, _bbox.Length * sizeof(float), _bbox, BufferUsageHint.StaticDraw);
        _vertexArrayObject_bbox = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArrayObject_bbox);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        var transform = Matrix4.Identity;
        GL.BindVertexArray(_vertexArrayObject_bbox);
        _shaderAmarela.SetMatrix4("transform", transform);
        _shaderAmarela.Use();
        GL.DrawArrays(PrimitiveType.LineLoop, 0, (_bbox.Length / 3));

#elif CG_DirectX && !CG_OpenGL
      Console.WriteLine(" .. Coloque aqui o seu código em DirectX");
#elif (CG_DirectX && CG_OpenGL) || (!CG_DirectX && !CG_OpenGL)
      Console.WriteLine(" .. ERRO de Render - escolha OpenGL ou DirectX !!");
#endif
      }
    }
#endif    

  }
}
