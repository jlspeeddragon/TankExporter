Imports System.IO
Imports System.Math
Imports System
Imports Tao.OpenGl
Imports Tao.FreeGlut

Module modDeferred
    Public G_Buffer As New GBuffer_
    Public gBufferFBO As Integer
    Public gColor, gFXAA As Integer
    Public gDepth As Integer
    Public rendered_shadow_texture As Integer
    Public Class GBuffer_
        Private attacments() As Integer = {Gl.GL_COLOR_ATTACHMENT0_EXT}
        Public Sub shut_down()
            delete_textures_and_fob_objects()
        End Sub
        Private Sub delete_textures_and_fob_objects()
            Dim e As Integer
            If rendered_shadow_texture > 0 Then
                Gl.glDeleteTextures(1, rendered_shadow_texture)
                e = Gl.glGetError
            End If
            If gColor > 0 Then
                Gl.glDeleteTextures(1, gColor)
                e = Gl.glGetError
            End If
            If gDepth > 0 Then
                Gl.glDeleteRenderbuffersEXT(1, gDepth)
                e = Gl.glGetError
            End If
            If gBufferFBO > 0 Then
                Gl.glDeleteFramebuffersEXT(1, gBufferFBO)
                e = Gl.glGetError
            End If
        End Sub
        Public Sub getsize(ByRef w As Integer, ByRef h As Integer)
            Dim w1, h1 As Integer
            w1 = frmMain.pb1.Width
            h1 = frmMain.pb1.Height
            w = w1 + (w1 Mod 2)
            h = h1 + (h1 Mod 2)
        End Sub
        Private Sub create_textures()
            Dim SCR_WIDTH, SCR_HEIGHT As Integer
            getsize(SCR_WIDTH, SCR_HEIGHT)
            'depth buffer
            Dim e1 = Gl.glGetError

            ' - rendered shadow texture
            Gl.glGenTextures(1, rendered_shadow_texture)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, rendered_shadow_texture)
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA8, SCR_WIDTH, SCR_HEIGHT, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, Nothing)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_GENERATE_MIPMAP, Gl.GL_FALSE)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT)
            ' - Color color buffer
            Gl.glGenTextures(1, gColor)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, gColor)
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA8, SCR_WIDTH, SCR_HEIGHT, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, Nothing)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_GENERATE_MIPMAP, Gl.GL_FALSE)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP_TO_EDGE)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP_TO_EDGE)
            Dim e2 = Gl.glGetError
            ' - Color color buffer
            Gl.glGenTextures(1, gFXAA)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, gFXAA)
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA8, SCR_WIDTH, SCR_HEIGHT, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, Nothing)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_GENERATE_MIPMAP, Gl.GL_FALSE)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP_TO_EDGE)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP_TO_EDGE)
            Dim e3 = Gl.glGetError

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)

        End Sub
        Public Sub attachColorTexture()
            detachFBOtextures()
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, gColor)
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, gColor, 0)
            Gl.glDrawBuffers(1, attacments)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            'Dim er = Gl.glGetError
        End Sub
        Public Sub attachFXAAtexture()
            detachFBOtextures()
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, gFXAA)
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, gFXAA, 0)
            Gl.glDrawBuffers(1, attacments)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            'Dim er = Gl.glGetError
        End Sub
        Public Sub detachFBOtextures()
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, 0, 0)
        End Sub
        Public Function init() As Boolean
            frmMain.update_thread.Suspend()
            Threading.Thread.Sleep(50)
            Dim SCR_WIDTH, SCR_HEIGHT As Integer
            getsize(SCR_WIDTH, SCR_HEIGHT)

            Gl.glBindFramebufferEXT(Gl.GL_DRAW_FRAMEBUFFER_EXT, 0)
            Dim e1 = Gl.glGetError

            delete_textures_and_fob_objects()
            'Create the gBuffer textures
            create_textures()
            Dim e2 = Gl.glGetError

            'Create the FBO
            Gl.glGenFramebuffersEXT(1, gBufferFBO)
            Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, gBufferFBO)
            Dim e3 = Gl.glGetError

            Gl.glGenRenderbuffersEXT(1, gDepth)
            Gl.glBindRenderbufferEXT(Gl.GL_RENDERBUFFER_EXT, gDepth)
            Gl.glRenderbufferStorageEXT(Gl.GL_RENDERBUFFER_EXT, Gl.GL_DEPTH_COMPONENT24, SCR_WIDTH, SCR_HEIGHT)
            Gl.glFramebufferRenderbufferEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_DEPTH_ATTACHMENT_EXT, Gl.GL_RENDERBUFFER_EXT, gDepth)
            Dim e4 = Gl.glGetError


            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, gColor, 0)
            Dim e5 = Gl.glGetError

            'attach draw buffers
            Gl.glDrawBuffers(1, attacments)

            'attach draw buffers
            Dim Status = Gl.glCheckFramebufferStatusEXT(Gl.GL_FRAMEBUFFER_EXT)

            If Status <> Gl.GL_FRAMEBUFFER_COMPLETE_EXT Then
                MsgBox("Failed to create Deferred FBO", MsgBoxStyle.Critical, "Not good!")
                Return False
            End If


            Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, 0)
            frmMain.update_thread.Resume()
            Return True
        End Function


    End Class


End Module
