﻿Imports System.IO

Module modXmodelLoader
    Public Sub load_and_save()
        cube_draw_id = get_X_model(Application.StartupPath + "\resources\models\cube.x")
        terrain_modelId = get_X_model(Application.StartupPath + "\resources\models\terrain.x")
        dome_modelId = get_X_model(Application.StartupPath + "\resources\models\dome.x")
    End Sub
    Public Function load_binary_model(file_ As String)
        Dim f = File.Open(file_, FileMode.Open, FileAccess.Read, FileShare.Read)
        Dim br As New BinaryReader(f)

        Dim v_cnt = br.ReadInt32 * 3 ' 3 per poly
        Dim id = Gl.glGenLists(1)
        Gl.glNewList(id, Gl.GL_COMPILE)

        Gl.glBegin(Gl.GL_TRIANGLES)
        Dim v As vect3
        For i = 0 To v_cnt
            v.x = br.ReadSingle
            v.y = br.ReadSingle
            v.z = br.ReadSingle
            Gl.glNormal3f(v.x, v.y, v.z)
            v.x = br.ReadSingle
            v.y = br.ReadSingle
            Gl.glMultiTexCoord2f(0, v.x, v.y)
            v.x = br.ReadSingle
            v.y = br.ReadSingle
            v.z = br.ReadSingle
            Gl.glVertex3f(v.x, v.y, v.z)
        Next
        Gl.glEnd()
        Gl.glEndList()
        Gl.glFinish()
        f.Close()
        f.Dispose()
        br.Dispose()
        Return id
    End Function


    Public vertices() As vec3
    Public normals() As vec3
    Public uvs() As vec2
    Public indices() As _indice
    Public Structure model_
        Public componet() As componet_
    End Structure
    Public Structure componet_
        Public list_ID As Integer
        Public diffuse As String
        Public diffuse_2 As String
        Public normal As String
        Public diffuse_ID As Integer
        Public diffuse2_ID As Integer
        Public normal_ID As Integer
    End Structure
    Public Structure vec3
        Public x, y, z As Single
    End Structure
    Public Structure _indice
        Public a, b, c As Integer
    End Structure


    Public Function get_X_model(file_ As String) As Integer
        'reads single object directX ASCII file.
        ' IN: path and name of file to load
        ' OUT: Display List ID.
        'At some point this will load multi model files.
        '##################################################
        'there is code in here to save as a binary file!!!!
        'use it to compress the x-files to much smaller sizes.. 1/6th as big and much faster loading
        Dim start_locations(1) As UInteger
        Dim obj_count As Integer = get_start_locations(start_locations, file_)

        Dim foutname = Path.GetFileNameWithoutExtension(file_)

        Dim s As New StreamReader(file_)
        Dim txt As String = ""
        While Not txt.ToLower.Contains("mesh")
            txt = s.ReadLine
        End While
        txt = s.ReadLine ' this should be the number of vertices
        Dim brk = txt.Split(";")
        Dim vertice_count = CInt(brk(0))
        ReDim vertices(vertice_count)
        For i = 0 To vertice_count - 1
            vertices(i) = New vec3
            txt = s.ReadLine
            brk = txt.Split(";")
            vertices(i).x = CSng(brk(0))
            vertices(i).y = CSng(brk(1))
            vertices(i).z = CSng(brk(2))
        Next
        txt = s.ReadLine ' this should be a blank line
        txt = s.ReadLine ' this should be the indice count for the vertices
        brk = txt.Split(";")
        Dim indice_count As Int32 = 0
        indice_count = CInt(brk(0))
        ReDim indices(indice_count)
        For i = 0 To indice_count - 1
            indices(i) = New _indice
            txt = s.ReadLine
            brk = txt.Split(";")
            brk = brk(1).Split(",")
            indices(i).a = CInt(brk(0))
            indices(i).b = CInt(brk(1))
            indices(i).c = CInt(brk(2))
        Next
        ' get normals
        s.Close()
        s = New StreamReader(file_)
        While Not txt.ToLower.Contains("meshnormals")
            txt = s.ReadLine
        End While
        txt = s.ReadLine ' this should be the normal count
        brk = txt.Split(";")
        Dim normal_count As Int32
        normal_count = CInt(brk(0))
        ReDim normals(normal_count)
        For i = 0 To normal_count - 1
            normals(i) = New vec3
            txt = s.ReadLine
            brk = txt.Split(";")
            normals(i).x = CSng(brk(0))
            normals(i).y = CSng(brk(1))
            normals(i).z = CSng(brk(2))
        Next
        s.Close()
        s = New StreamReader(file_)
        While Not txt.ToLower.Contains("meshtexturecoords")
            txt = s.ReadLine
        End While
        txt = s.ReadLine ' this should be the texture coordinate count
        brk = txt.Split(";")
        Dim txt_coord_cnt As Int32
        txt_coord_cnt = CInt(brk(0))
        ReDim uvs(txt_coord_cnt)
        For i = 0 To txt_coord_cnt - 1
            uvs(i) = New vec2
            txt = s.ReadLine
            brk = txt.Split(";")
            uvs(i).x = CSng(brk(0))
            uvs(i).y = CSng(brk(1))
        Next
        'At this point, we have all the data to make the mesh
        'Gen Display List ID.
        Dim a, b, c As Integer
        If True Then

            Dim f = File.Open("c:\" + foutname + ".te", FileMode.OpenOrCreate, FileAccess.Write)
            Dim br As New BinaryWriter(f)
            br.Write(indice_count)
            For i = 0 To indice_count
                a = indices(i).a
                b = indices(i).b
                c = indices(i).c
                br.Write(normals(a).x) : br.Write(normals(a).y) : br.Write(normals(a).z)
                br.Write(uvs(a).x) : br.Write(uvs(a).y)
                br.Write(normals(a).x) : br.Write(normals(a).y) : br.Write(normals(a).z)

                br.Write(normals(b).x) : br.Write(normals(b).y) : br.Write(normals(b).z)
                br.Write(uvs(b).x) : br.Write(uvs(b).y)
                br.Write(normals(b).x) : br.Write(normals(b).y) : br.Write(normals(b).z)

                br.Write(normals(c).x) : br.Write(normals(c).y) : br.Write(normals(c).z)
                br.Write(uvs(c).x) : br.Write(uvs(c).y)
                br.Write(normals(c).x) : br.Write(normals(c).y) : br.Write(normals(c).z)
            Next
            f.Dispose()
            br.Dispose()

        End If

        Dim list_ID = Gl.glGenLists(1)
        Gl.glNewList(list_ID, Gl.GL_COMPILE)
        Gl.glBegin(Gl.GL_TRIANGLES)

        'create all the triangles.
        For i = 0 To indice_count
            a = indices(i).a
            b = indices(i).b
            c = indices(i).c

            Gl.glNormal3f(normals(a).x, normals(a).y, normals(a).z)
            Gl.glTexCoord2f(uvs(a).x, uvs(a).y)
            Gl.glVertex3f(vertices(a).x, vertices(a).y, vertices(a).z)

            Gl.glNormal3f(normals(b).x, normals(b).y, normals(b).z)
            Gl.glTexCoord2f(uvs(b).x, uvs(b).y)
            Gl.glVertex3f(vertices(b).x, vertices(b).y, vertices(b).z)

            Gl.glNormal3f(normals(c).x, normals(c).y, normals(c).z)
            Gl.glTexCoord2f(uvs(c).x, uvs(c).y)
            Gl.glVertex3f(vertices(c).x, vertices(c).y, vertices(c).z)
        Next
        Gl.glEnd()
        Gl.glEndList()
        Return list_ID
    End Function
    Public Function get_start_locations(ByRef loc() As UInteger, ByRef file_ As String)
        Dim m_count As Integer = 0
        Dim c_pos As UInteger = 0
        Dim txt As String = ""
        Dim s As New StreamReader(file_)
        While Not s.EndOfStream
            c_pos = s.BaseStream.Position
            txt = s.ReadLine
            If txt.ToLower.Contains("mesh ") Then
                ReDim Preserve loc(m_count + 1)
                loc(m_count) = c_pos
                m_count += 1
            End If
        End While
        Return m_count
    End Function

End Module
