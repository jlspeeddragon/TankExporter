Imports System
Imports System.Text
Imports System.String
Imports System.IO
Imports Tao.OpenGl
Imports Tao.Platform.Windows
Module shader_loader
    Public shader_list As New shader_list_
    Public Class shader_list_
        Public normal_shader As Integer
        Public mixer_shader As Integer
        Public tank_shader As Integer
        Public fbx_shader As Integer
    End Class

#Region "variables"
    Public shaders As shaders__
    Public Structure shaders__
        Public shader() As shaders_
        Public Function f(ByVal name As String) As Integer
            For Each s In shader
                If s.shader_name = name Then
                    Return s.shader_id
                End If
            Next
            Return 0
        End Function
    End Structure
    Public Structure shaders_
        Public fragment As String
        Public vertex As String
        Public geo As String
        Public shader_name As String
        Public shader_id As Integer
        Public has_geo As Boolean
        Public Sub set_call_id(ByVal id As Integer)
            Try
                CallByName(shader_list, Me.shader_name, CallType.Set, Me.shader_id)
            Catch ex As Exception
                MsgBox("missing member from shader_list:" + Me.shader_name, MsgBoxStyle.Exclamation, "Oops!")
                End
            End Try
        End Sub
    End Structure

#End Region


    Public Sub make_shaders()
        'I'm tierd of all the work every time I add a shader.
        'So... Im going to automate the process.. Hey.. its a computer for fucks sake!
        Dim f_list() As String = IO.Directory.GetFiles(Application.StartupPath + "\shaders\", "*fragment.glsl")
        Dim v_list() As String = IO.Directory.GetFiles(Application.StartupPath + "\shaders\", "*vertex.glsl")
        Dim g_list() As String = IO.Directory.GetFiles(Application.StartupPath + "\shaders\", "*geo.glsl")
        Array.Sort(f_list)
        Array.Sort(v_list)
        Array.Sort(g_list)
        ReDim shaders.shader(f_list.Length - 1)
        With shaders

            For i = 0 To f_list.Length - 1
                .shader(i) = New shaders_
                With .shader(i)
                    Dim fn As String = Path.GetFileNameWithoutExtension(f_list(i))
                    Dim ar = fn.Split("_")
                    .shader_name = ar(0) + "_shader"
                    .fragment = f_list(i)
                    .vertex = v_list(i)
                    .geo = ""
                    For Each g In g_list ' find matching geo if there is one.. usually there wont be
                        If g.Contains(ar(0)) Then
                            .geo = g
                            .has_geo = True ' found a matching geo so we need to set this true
                        End If
                    Next
                    .shader_id = -1
                    .set_call_id(-1)
                End With
            Next

        End With
        Dim fs As String
        Dim vs As String
        Dim gs As String

        For i = 0 To shaders.shader.Length - 1
            With shaders.shader(i)
                vs = .vertex
                fs = .fragment
                gs = .geo
                Dim id = assemble_shader(vs, gs, fs, .shader_id, .shader_name, .has_geo)
                .set_call_id(id)
                .shader_id = id

                'Debug.WriteLine(.shader_name + "  Id:" + .shader_id.ToString)
            End With
        Next

    End Sub
    Public Function assemble_shader(v As String, g As String, f As String, ByRef shader As Integer, ByRef name As String, ByRef has_geo As Boolean) As Integer
        Dim vs(1) As String
        Dim gs(1) As String
        Dim fs(1) As String
        Dim vertexObject As Integer
        Dim geoObject As Integer
        Dim fragmentObject As Integer
        Dim status_code As Integer
        Dim info As New StringBuilder
        info.Length = 8192
        Dim info_l As Integer

        If shader > 0 Then
            Gl.glUseProgram(0)
            Gl.glDeleteProgram(shader)
            Gl.glGetProgramiv(shader, Gl.GL_DELETE_STATUS, status_code)
            Gl.glFinish()
        End If

        Dim e = Gl.glGetError
        If e <> 0 Then
            Dim s = Glu.gluErrorString(e)
            Dim ms As String = System.Reflection.MethodBase.GetCurrentMethod().Name
            'MsgBox("Function: " + ms + vbCrLf + "Error! " + s, MsgBoxStyle.Exclamation, "OpenGL Issue")
        End If
        'have a hard time with files remaining open.. hope this fixes it! (yep.. it did)
        Using vs_s As New StreamReader(v)
            vs(0) = vs_s.ReadToEnd
            vs_s.Close()
            vs_s.Dispose()
        End Using
        Using fs_s As New StreamReader(f)
            fs(0) = fs_s.ReadToEnd
            fs_s.Close()
            fs_s.Dispose()
        End Using
        If has_geo Then
            Using gs_s As New StreamReader(g)
                gs(0) = gs_s.ReadToEnd
                gs_s.Close()
                gs_s.Dispose()
            End Using
        End If


        vertexObject = Gl.glCreateShader(Gl.GL_VERTEX_SHADER)
        fragmentObject = Gl.glCreateShader(Gl.GL_FRAGMENT_SHADER)
        '--------------------------------------------------------------------
        shader = Gl.glCreateProgram()

        ' Compile vertex shader
        Gl.glShaderSource(vertexObject, 1, vs, vs(0).Length)
        Gl.glCompileShader(vertexObject)
        Gl.glGetShaderInfoLog(vertexObject, 8192, info_l, info)
        Gl.glGetShaderiv(vertexObject, Gl.GL_COMPILE_STATUS, status_code)
        If Not status_code = Gl.GL_TRUE Then
            Gl.glDeleteShader(vertexObject)
            gl_error(name + "_vertex didn't compile!" + vbCrLf + info.ToString)
            'Return
        End If

        e = Gl.glGetError
        If e <> 0 Then
            Dim s = Glu.gluErrorString(e)
            Dim ms As String = System.Reflection.MethodBase.GetCurrentMethod().Name
            MsgBox("Function: " + ms + vbCrLf + "Error! " + s, MsgBoxStyle.Exclamation, "OpenGL Issue")
        End If

        If has_geo Then
            'geo
            geoObject = Gl.glCreateShader(Gl.GL_GEOMETRY_SHADER_EXT)
            Gl.glShaderSource(geoObject, 1, gs, gs(0).Length)
            Gl.glCompileShader(geoObject)
            Gl.glGetShaderInfoLog(geoObject, 8192, info_l, info)
            Gl.glGetShaderiv(geoObject, Gl.GL_COMPILE_STATUS, status_code)
            If Not status_code = Gl.GL_TRUE Then
                Gl.glDeleteShader(geoObject)
                gl_error(name + "_geo didn't compile!" + vbCrLf + info.ToString)
                'Return
            End If
            e = Gl.glGetError
            If e <> 0 Then
                Dim s = Glu.gluErrorString(e)
                Dim ms As String = System.Reflection.MethodBase.GetCurrentMethod().Name
                MsgBox("Function: " + ms + vbCrLf + "Error! " + s, MsgBoxStyle.Exclamation, "OpenGL Issue")
            End If
            If name.Contains("raytrace") Then

                Gl.glProgramParameteriEXT(shader, Gl.GL_GEOMETRY_INPUT_TYPE_EXT, Gl.GL_TRIANGLES)
                Gl.glProgramParameteriEXT(shader, Gl.GL_GEOMETRY_OUTPUT_TYPE_EXT, Gl.GL_LINE_STRIP)
                Gl.glProgramParameteriEXT(shader, Gl.GL_GEOMETRY_VERTICES_OUT_EXT, 6)
            End If
            If name.Contains("normal") Then
                Gl.glProgramParameteriEXT(shader, Gl.GL_GEOMETRY_INPUT_TYPE_EXT, Gl.GL_TRIANGLES)
                Gl.glProgramParameteriEXT(shader, Gl.GL_GEOMETRY_OUTPUT_TYPE_EXT, Gl.GL_LINE_STRIP)
                Gl.glProgramParameteriEXT(shader, Gl.GL_GEOMETRY_VERTICES_OUT_EXT, 18)
            End If

            e = Gl.glGetError
            If e <> 0 Then
                Dim s = Glu.gluErrorString(e)
                Dim ms As String = System.Reflection.MethodBase.GetCurrentMethod().Name
                MsgBox("Function: " + ms + vbCrLf + "Error! " + s, MsgBoxStyle.Exclamation, "OpenGL Issue")
            End If

        End If

        ' Compile fragment shader

        Gl.glShaderSource(fragmentObject, 1, fs, fs(0).Length)
        Gl.glCompileShader(fragmentObject)
        Gl.glGetShaderInfoLog(fragmentObject, 8192, info_l, info)
        Gl.glGetShaderiv(fragmentObject, Gl.GL_COMPILE_STATUS, status_code)

        If Not status_code = Gl.GL_TRUE Then
            Gl.glDeleteShader(fragmentObject)
            gl_error(name + "_fragment didn't compile!" + vbCrLf + info.ToString)
            'Return
        End If
        e = Gl.glGetError
        If e <> 0 Then
            Dim s = Glu.gluErrorString(e)
            Dim ms As String = System.Reflection.MethodBase.GetCurrentMethod().Name
            MsgBox("Function: " + ms + vbCrLf + "Error! " + s, MsgBoxStyle.Exclamation, "OpenGL Issue")
        End If

        'attach shader objects
        Gl.glAttachShader(shader, fragmentObject)
        If has_geo Then
            Gl.glAttachShader(shader, geoObject)
        End If
        Gl.glAttachShader(shader, vertexObject)

        'link program
        Gl.glLinkProgram(shader)

        ' detach shader objects
        Gl.glDetachShader(shader, fragmentObject)
        If has_geo Then
            Gl.glDetachShader(shader, geoObject)
        End If
        Gl.glDetachShader(shader, vertexObject)

        e = Gl.glGetError
        If e <> 0 Then
            Dim s = Glu.gluErrorString(e)
            Dim ms As String = System.Reflection.MethodBase.GetCurrentMethod().Name
            MsgBox("Function: " + ms + vbCrLf + "Error! " + s, MsgBoxStyle.Exclamation, "OpenGL Issue")
        End If

        Gl.glGetShaderiv(shader, Gl.GL_LINK_STATUS, status_code)

        If Not status_code = Gl.GL_TRUE Then
            Gl.glDeleteProgram(shader)
            gl_error(name + " did not link!" + vbCrLf + info.ToString)
            'Return
        End If

        'delete shader objects
        Gl.glDeleteShader(fragmentObject)
        Gl.glGetShaderiv(fragmentObject, Gl.GL_DELETE_STATUS, status_code)
        If has_geo Then
            Gl.glDeleteShader(geoObject)
            Gl.glGetShaderiv(geoObject, Gl.GL_DELETE_STATUS, status_code)
        End If
        Gl.glDeleteShader(vertexObject)
        Gl.glGetShaderiv(vertexObject, Gl.GL_DELETE_STATUS, status_code)
        e = Gl.glGetError
        If e <> 0 Then
            'aways throws a error after deletion even though the status shows them as deleted.. ????
            Dim s = Glu.gluErrorString(e)
            Dim ms As String = System.Reflection.MethodBase.GetCurrentMethod().Name
            'MsgBox("Function: " + ms + vbCrLf + "Error! " + s, MsgBoxStyle.Exclamation, "OpenGL Issue")
        End If
        vs(0) = Nothing
        fs(0) = Nothing
        If has_geo Then
            gs(0) = Nothing
        End If
        GC.Collect()
        GC.WaitForFullGCComplete()

        Return shader
    End Function


    Public Sub gl_error(s As String)
        s = s.Replace(vbLf, vbCrLf)
        s.Replace("0(", vbCrLf + "(")
        frmShaderError.Show()
        frmShaderError.er_tb.Text += s
    End Sub

    '==============================================================================================================
    'tank_shader
    Public tank_colorMap, tank_normalMap, tank_is_GAmap, tank_alphaTest, tank_viewPosition As Integer
    Public tank_GMM, tank_AO, tank_detailMap, tank_detailTiling, tank_detailPower As Integer
    Public tank_camo, tank_use_camo, tank_tile_vec4 As Integer
    Public tank_c0, tank_c1, tank_c2, tank_c3, tank_armorcolor, tank_camo_tiling, tank_exclude_camo As Integer
    Public tank_use_CM As Integer
    Public tank_ambient, tank_specular, tank_total As Integer

    Private Sub set_tank_shader_variables()
        tank_colorMap = Gl.glGetUniformLocation(shader_list.tank_shader, "colorMap")
        tank_normalMap = Gl.glGetUniformLocation(shader_list.tank_shader, "normalMap")
        tank_GMM = Gl.glGetUniformLocation(shader_list.tank_shader, "gmmMap")
        tank_AO = Gl.glGetUniformLocation(shader_list.tank_shader, "aoMap")
        tank_detailMap = Gl.glGetUniformLocation(shader_list.tank_shader, "detailMap")
        tank_camo = Gl.glGetUniformLocation(shader_list.tank_shader, "camoMap")
        tank_is_GAmap = Gl.glGetUniformLocation(shader_list.tank_shader, "is_GAmap")
        tank_alphaTest = Gl.glGetUniformLocation(shader_list.tank_shader, "alphaTest")
        tank_detailTiling = Gl.glGetUniformLocation(shader_list.tank_shader, "detailTiling")
        tank_detailPower = Gl.glGetUniformLocation(shader_list.tank_shader, "detailPower")
        tank_use_camo = Gl.glGetUniformLocation(shader_list.tank_shader, "use_camo")
        tank_tile_vec4 = Gl.glGetUniformLocation(shader_list.tank_shader, "tile_vec4")
        tank_c0 = Gl.glGetUniformLocation(shader_list.tank_shader, "c0")
        tank_c1 = Gl.glGetUniformLocation(shader_list.tank_shader, "c1")
        tank_c2 = Gl.glGetUniformLocation(shader_list.tank_shader, "c2")
        tank_c3 = Gl.glGetUniformLocation(shader_list.tank_shader, "c3")
        tank_armorcolor = Gl.glGetUniformLocation(shader_list.tank_shader, "armorcolor")
        tank_camo_tiling = Gl.glGetUniformLocation(shader_list.tank_shader, "camo_tiling")
        tank_exclude_camo = Gl.glGetUniformLocation(shader_list.tank_shader, "exclude_camo")
        tank_use_CM = Gl.glGetUniformLocation(shader_list.tank_shader, "use_CM")
        tank_ambient = Gl.glGetUniformLocation(shader_list.tank_shader, "A_level")
        tank_specular = Gl.glGetUniformLocation(shader_list.tank_shader, "S_level")
        tank_total = Gl.glGetUniformLocation(shader_list.tank_shader, "T_level")

    End Sub

    '==============================================================================================================

    Public fbx_ambient, fbx_specular, fbx_level As Integer
    Public fbx_colorMap, fbx_specularMap, fbx_normalMap, fbx_is_GAmap As Integer
    Public fbx_texture_count, fbx_alphatest As Integer
    Private Sub set_fbx_shader_variables()
        fbx_ambient = Gl.glGetUniformLocation(shader_list.fbx_shader, "A_level")
        fbx_specular = Gl.glGetUniformLocation(shader_list.fbx_shader, "S_level")
        fbx_level = Gl.glGetUniformLocation(shader_list.fbx_shader, "T_level")
        fbx_colorMap = Gl.glGetUniformLocation(shader_list.fbx_shader, "colorMap")
        fbx_normalMap = Gl.glGetUniformLocation(shader_list.fbx_shader, "normalMap")
        fbx_specularMap = Gl.glGetUniformLocation(shader_list.fbx_shader, "specularMap")
        fbx_texture_count = Gl.glGetUniformLocation(shader_list.fbx_shader, "t_cnt")
        fbx_is_GAmap = Gl.glGetUniformLocation(shader_list.fbx_shader, "is_GAmap")
        fbx_alphatest = Gl.glGetUniformLocation(shader_list.fbx_shader, "alphaTest")
    End Sub
    '==============================================================================================================
    Public normal_shader_mode As Integer
    Public normal_shader_mode_id As Integer
    Private Sub set_normal_shader_variables()
        normal_shader_mode_id = Gl.glGetUniformLocation(shader_list.normal_shader, "mode")
    End Sub
    '==============================================================================================================
    Public mix_camoMap, mix_c0, mix_c1, mix_c2, mix_c3, mix_armorColor
    Private Sub set_mixer_shader_variables()
        mix_camoMap = Gl.glGetUniformLocation(shader_list.mixer_shader, "camoMap")
        mix_c0 = Gl.glGetUniformLocation(shader_list.mixer_shader, "c0")
        mix_c1 = Gl.glGetUniformLocation(shader_list.mixer_shader, "c1")
        mix_c2 = Gl.glGetUniformLocation(shader_list.mixer_shader, "c2")
        mix_c3 = Gl.glGetUniformLocation(shader_list.mixer_shader, "c3")
        mix_armorColor = Gl.glGetUniformLocation(shader_list.mixer_shader, "armorColor")

    End Sub

    '==============================================================================================================

    Public Sub set_shader_variables()
        set_tank_shader_variables()
        set_normal_shader_variables()
        set_mixer_shader_variables()
        set_fbx_shader_variables()
        Return

    End Sub
    '==============================================================================================================

End Module
