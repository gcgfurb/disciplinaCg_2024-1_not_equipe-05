//TODO: testar se estes DEFINEs continuam funcionado
#define CG_Gizmo  // debugar gráfico.
#define CG_OpenGL // render OpenGL.
// #define CG_DirectX // render DirectX.
// #define CG_Privado // código do professor.

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
// using OpenTK.Mathematics;

namespace gcgcg
{
  public class Mundo : GameWindow
  {
    private static Objeto mundo = null;
    private char rotuloAtual = '?';
    private Objeto objetoSelecionado = null;
    private int cont = 0;
    private bool first = true;

    private readonly float[] _sruEixos =
    {
           -0.0f,  0.0f,  0.0f, /* X- */      0.5f,  0.0f,  0.0f, /* X+ */
            0.0f,  0.0f,  0.0f, /* Y- */      0.0f,  0.5f,  0.0f, /* Y+ */
            0.0f,  0.0f, -0.5f, /* Z- */      0.0f,  0.0f,  0.5f, /* Z+ */
        };

    private int _vertexBufferObject_sruEixos;
    private int _vertexArrayObject_sruEixos;

    private Shader _shaderVermelha;
    private Shader _shaderVerde;
    private Shader _shaderAzul;

    public Mundo(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
           : base(gameWindowSettings, nativeWindowSettings)
    {
      mundo ??= new Objeto(null, ref rotuloAtual);
    }

    protected override void OnLoad()
    {
      base.OnLoad();

      GL.ClearColor(0.50196f, 0.50196f, 0.50196f, 1.0f);

      #region Eixos: SRU  
      _vertexBufferObject_sruEixos = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_sruEixos);
      GL.BufferData(BufferTarget.ArrayBuffer, _sruEixos.Length * sizeof(float), _sruEixos, BufferUsageHint.StaticDraw);
      _vertexArrayObject_sruEixos = GL.GenVertexArray();
      GL.BindVertexArray(_vertexArrayObject_sruEixos);
      GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
      GL.EnableVertexAttribArray(0);
      _shaderVermelha = new Shader("Shaders/shader.vert", "Shaders/shaderVermelha.frag");
      _shaderVerde = new Shader("Shaders/shader.vert", "Shaders/shaderVerde.frag");
      _shaderAzul = new Shader("Shaders/shader.vert", "Shaders/shaderAzul.frag");
      #endregion

      objetoSelecionado = new Retangulo(mundo, ref rotuloAtual, new Ponto4D(-0.5, -0.5), new Ponto4D(0.5, 0.5))
      {
        PrimitivaTipo = PrimitiveType.Points
      };
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
      base.OnRenderFrame(e);

      GL.Clear(ClearBufferMask.ColorBufferBit);

      // desenha os eixos
#if CG_Gizmo
      Sru3D();
#endif

      mundo.Desenhar();
      SwapBuffers();
    }

    private void AlteraTipoPrimitivo()
    {
      switch (cont)
      {
        case 0:
          if (first)
          {
            first = false;
            objetoSelecionado.PrimitivaTipo = PrimitiveType.Lines;
            cont += 2;
            break;
          }
          else
          {
            objetoSelecionado.PrimitivaTipo = PrimitiveType.Points;
            cont++;
            break;
          }
        case 1:
          objetoSelecionado.PrimitivaTipo = PrimitiveType.Lines;
          cont++;
          break;
        case 2:
          objetoSelecionado.PrimitivaTipo = PrimitiveType.LineLoop;
          cont++;
          break;
        case 3:
          objetoSelecionado.PrimitivaTipo = PrimitiveType.LineStrip;
          cont++;
          break;
        case 4:
          objetoSelecionado.PrimitivaTipo = PrimitiveType.Triangles;
          cont++;
          break;
        case 5:
          objetoSelecionado.PrimitivaTipo = PrimitiveType.TriangleStrip;
          cont++;
          break;
        case 6:
          objetoSelecionado.PrimitivaTipo = PrimitiveType.TriangleFan;
          cont = 0;
          break;
        default:
          break;
      }
    }
    protected override void OnUpdateFrame(FrameEventArgs e)
    {
      base.OnUpdateFrame(e);

      var input = KeyboardState;
      if (input.IsKeyPressed(Keys.Space))
      {
        AlteraTipoPrimitivo();
      }
    }

    protected override void OnResize(ResizeEventArgs e)
    {
      base.OnResize(e);

      GL.Viewport(0, 0, Size.X, Size.Y);
    }

    protected override void OnUnload()
    {
      mundo.OnUnload();

      GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
      GL.BindVertexArray(0);
      GL.UseProgram(0);

      GL.DeleteBuffer(_vertexBufferObject_sruEixos);
      GL.DeleteVertexArray(_vertexArrayObject_sruEixos);

      GL.DeleteProgram(_shaderVermelha.Handle);
      GL.DeleteProgram(_shaderVerde.Handle);
      GL.DeleteProgram(_shaderAzul.Handle);

      base.OnUnload();
    }

#if CG_Gizmo
    private void Sru3D()
    {
#if CG_OpenGL && !CG_DirectX
      GL.BindVertexArray(_vertexArrayObject_sruEixos);
      // EixoX
      _shaderVermelha.Use();
      GL.DrawArrays(PrimitiveType.Lines, 0, 2);
      // EixoY
      _shaderVerde.Use();
      GL.DrawArrays(PrimitiveType.Lines, 2, 2);
      // EixoZ
      _shaderAzul.Use();
      GL.DrawArrays(PrimitiveType.Lines, 4, 2);
#elif CG_DirectX && !CG_OpenGL
      Console.WriteLine(" .. Coloque aqui o seu código em DirectX");
#elif (CG_DirectX && CG_OpenGL) || (!CG_DirectX && !CG_OpenGL)
      Console.WriteLine(" .. ERRO de Render - escolha OpenGL ou DirectX !!");
#endif
    }
#endif

  }
}
