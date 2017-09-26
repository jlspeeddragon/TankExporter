Imports System.Windows
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Net
Imports System.Text
Imports System.IO
Imports System.Xml
Imports System.Web
Imports Tao.OpenGl
Imports Tao.Platform.Windows
Imports Tao.FreeGlut
Imports Tao.FreeGlut.Glut
Imports Tao.DevIl
Imports Microsoft.VisualBasic.Strings
Imports System.Math
Imports System.Object
Imports System.Threading
Imports System.Data
Imports Skill.FbxSDK
Imports Skill.FbxSDK.IO
Imports SlimDX

Module modFBX
    Public FBX_Texture_path As String
    '    Public Sub import_FBX()
    '        'fbx import sub
    '        If frmMain.DGV_objs.Rows.Count = 0 Then
    '            MsgBox("Select something to export!", MsgBoxStyle.Exclamation, "No Selection...")
    '            Return
    '        End If
    '        Dim id As Integer
    '        Try
    '            id = frmMain.DGV_objs.SelectedCells(0).RowIndex

    '        Catch ex As Exception
    '            MsgBox("Select something to export!", MsgBoxStyle.Exclamation, "No Selection...")
    '            Return
    '        End Try

    '        If id < 0 Then
    '            MsgBox("Select something to export!", MsgBoxStyle.Exclamation, "No Selection...")
    '            Return
    '        End If
    '        Dim j As UInt32 = 0
    '        Dim i As UInt32 = 0

    '        Dim model_name As String = frmMain.DGV_objs(1, id).Value.ToString.Replace(":", "")
    '        frmMain.OpenFileDialog2.FileName = model_name
    '        frmMain.OpenFileDialog2.Filter = "AutoDesk (*.FBX)|*.fbx"
    '        frmMain.OpenFileDialog2.Title = "Import FBX..."
    '        If Not frmMain.OpenFileDialog2.ShowDialog = Forms.DialogResult.OK Then
    '            frmMain.Main_Menu.Enabled = True
    '            frmMain.Timer1.Enabled = True
    '            Return
    '        End If
    '        frmMain.Main_Menu.Enabled = False
    '        frmMain.Timer1.Enabled = False
    '        Dim pManager As FbxSdkManager
    '        Dim scene As FbxScene
    '        pManager = FbxSdkManager.Create
    '        scene = FbxScene.Create(pManager, "My Scene")
    '        Dim fileformat As Integer = Skill.FbxSDK.IO.FileFormat.FbxAscii
    '        'Detect the file format of the file to be imported            
    '        Dim filename = frmMain.OpenFileDialog2.FileName
    '        If Not pManager.IOPluginRegistry.DetectFileFormat(filename, fileformat) Then

    '            ' Unrecognizable file format.
    '            ' Try to fall back to SDK's native file format (an FBX binary file).
    '            fileformat = pManager.IOPluginRegistry.NativeReaderFormat
    '        End If

    '        Dim importOptions = Skill.FbxSDK.IO.FbxStreamOptionsFbxReader.Create(pManager, "")
    '        Dim importer As Skill.FbxSDK.IO.FbxImporter = Skill.FbxSDK.IO.FbxImporter.Create(pManager, "")
    '        importer.FileFormat = fileformat    ' ascii only for now
    '        Dim imp_status As Boolean = importer.Initialize(filename)
    '        If Not imp_status Then
    '            MsgBox("Failed to open " + frmMain.OpenFileDialog2.FileName, MsgBoxStyle.Exclamation, "FBX Load Error...")
    '            pManager.Destroy()
    '            GoTo outofhere
    '        End If
    '        If Not importer.IsFBX Then
    '            MsgBox("Are you sure this is a FBX file? " + vbCrLf + frmMain.OpenFileDialog1.FileName, MsgBoxStyle.Exclamation, "FBX Load Error...")
    '            pManager.Destroy()
    '            GoTo outofhere
    '        End If
    '        importOptions.SetOption(Skill.FbxSDK.IO.FbxStreamOptionsFbx.MATERIAL, True)
    '        importOptions.SetOption(Skill.FbxSDK.IO.FbxStreamOptionsFbx.TEXTURE, True)
    '        importOptions.SetOption(Skill.FbxSDK.IO.FbxStreamOptionsFbx.LINK, True)
    '        importOptions.SetOption(Skill.FbxSDK.IO.FbxStreamOptionsFbx.SHAPE, True)
    '        importOptions.SetOption(Skill.FbxSDK.IO.FbxStreamOptionsFbx.GOBO, True)
    '        importOptions.SetOption(Skill.FbxSDK.IO.FbxStreamOptionsFbx.ANIMATION, True)
    '        importOptions.SetOption(Skill.FbxSDK.IO.FbxStreamOptionsFbx.GLOBAL_SETTINGS, True)

    '        imp_status = importer.Import(scene, importOptions)
    '        importer.Destroy()
    '        Dim rootnode As FbxNode = scene.RootNode
    '        Dim nodecnt As Int32 = rootnode.GetChildCount
    '        Dim childnode As FbxNode
    '        Dim mesh As FbxMesh = Nothing
    '        Dim geo As FbxGeometry = Nothing
    '        For i = 0 To nodecnt
    '            childnode = rootnode.GetChild(i)
    '            Try

    '                mesh = childnode.Mesh
    '                If mesh IsNot Nothing Then
    '                    geo = childnode.NodeAttribute
    '                    Exit For
    '                End If
    '            Catch ex As Exception

    '            End Try

    '        Next
    '        id += 1
    '        Dim polycnt As UInt32 = mesh.PolygonCount
    '        'Dim nIndices() As Int32 = mesh.PolygonVertices
    '        Dim rotation As FbxDouble3 = rootnode.LclRotation.Get
    '        Dim tranX As FbxDouble3 = rootnode.LclRotation.Get
    '        Dim x_cent, y_cent, z_cent As Single
    '        Dim b_max, b_min As FbxDouble3
    '        mesh.ComputeBBox()
    '        b_max = mesh.BBoxMax.Get
    '        b_min = mesh.BBoxMin.Get
    '        x_cent = (b_max.X + b_min.X) / 2
    '        y_cent = (b_max.Y + b_min.Y) / 2
    '        z_cent = (b_max.Z + b_min.Z) / 2
    '        Dim val As Integer = 0
    '        Dim up_axis = scene.GlobalSettings.AxisSystem.GetUpVector(val)
    '        Dim up_y, up_z As Integer
    '        up_y = FbxAxisSystem.UpVector.YAxis
    '        up_z = FbxAxisSystem.UpVector.ZAxis
    '        Dim old_poly_cnt As Long = _group(id).nPrimitives_
    '        Dim old_vertice_cnt As Long = _group(id).nVertices_
    '        Dim nVertices = mesh.Normals.Count
    '        ReDim Preserve _group(id).vertices(1)
    '        ReDim Preserve _group(id).vertices(nVertices)
    '        '------ we use this to resize the vertices array and get the right count of vertices.
    '        For i = 0 To nVertices - 1
    '            _group(id).vertices(i) = New vertice_
    '            _group(id).vertices(i).index_1 = 255
    '        Next
    '        '-----------------------------------------------------------------------------------
    '        ReDim Preserve _group(id).indicies(polycnt)
    '        'Dim layer = mesh.UVLayerCount
    '        Dim uvlayer1 As FbxLayerElementUV = mesh.GetLayer(0).GetUVs
    '        Dim uvlayer2 As FbxLayerElementUV = Nothing
    '        Dim mat_cnt As Integer = geo.Node.GetSrcObjectCount(FbxSurfaceMaterial.ClassId)
    '        Dim material As FbxSurfaceMaterial = geo.Node.GetSrcObject(FbxSurfaceMaterial.ClassId, 0)
    '        Dim property_ As FbxProperty
    '        Dim texture As FbxTexture
    '        'diffuse texture.. color2_name
    '        property_ = material.FindProperty(FbxSurfaceMaterial.SAmbient)
    '        texture = property_.GetSrcObject(FbxTexture.ClassId, 0)
    '        'this might throw an exception so its wrapped. (means there is no 2nd texture.)
    '        Try
    '            _group(id).color2_name = texture.FileName

    '        Catch ex As Exception

    '        End Try
    '        'ambient texture.. color_name
    '        Try
    '            property_ = material.FindProperty(FbxSurfaceMaterial.SDiffuse)

    '        Catch ex As Exception
    '            _group(id).blend_only = False
    '        End Try
    '        texture = property_.GetSrcObject(FbxTexture.ClassId, 0)
    '        _group(id).color_name = texture.FileName
    '        'normal map... normal_name
    '        Try
    '            property_ = material.FindProperty(FbxSurfaceMaterial.SBump)
    '            _group(id).bumped = True
    '        Catch ex As Exception

    '        End Try
    '        texture = property_.GetSrcObject(FbxTexture.ClassId, 0)
    '        Try
    '            _group(id).normal_name = texture.FileName

    '        Catch ex As Exception

    '        End Try
    '        build_textures(id, True)
    '        Try
    '            uvlayer2 = mesh.GetLayer(1).GetUVs
    '            'has_uv2 = True

    '        Catch ex As Exception
    '            '_group(id).multi_textured = False

    '        End Try
    '        If has_uv2 Then
    '            ' uvlayer2 = mesh.GetLayer(1).GetUVs

    '        End If
    '        Dim eNormals As FbxLayerElementNormal = mesh.GetLayer(0).Normals
    '        Dim index_mode = uvlayer1.Reference_Mode
    '        Dim off As UInt32 = _group(id).startVertex_
    '        'ARGH!!! cant figure out the normals!! LOL --> YES I DID!
    '        j = 0
    '        frmMain.pbar.Visible = True
    '        frmMain.pbar.Maximum = polycnt
    '        frmMain.pbar.Minimum = 0
    '        For i = 0 To polycnt - 1
    '            frmMain.pbar.Value = i
    '            Application.DoEvents()

    '            Dim pn1, pn2, pn3 As UInt32
    '            Dim uv1_, uv2_, uv3_ As FbxVector2
    '            Dim uv1_2, uv2_2, uv3_2 As New FbxVector2
    '            Dim vtc1, vtc2, vtc3 As New vertice_
    '            Dim n1, n2, n3 As New FbxVector4
    '            Dim v1 = mesh.GetPolygonVertex(i, 0)
    '            Dim v2 = mesh.GetPolygonVertex(i, 1)
    '            Dim v3 = mesh.GetPolygonVertex(i, 2)
    '            Dim vt1 = mesh.GetControlPointAt(v1)
    '            Dim vt2 = mesh.GetControlPointAt(v2)
    '            Dim vt3 = mesh.GetControlPointAt(v3)
    '            mesh.GetPolygonVertexNormal(i, 0, n1)
    '            mesh.GetPolygonVertexNormal(i, 1, n2)
    '            mesh.GetPolygonVertexNormal(i, 2, n3)


    '            pn1 = frmMain.packnormalFBX(n1)
    '            pn2 = frmMain.packnormalFBX(n2)
    '            pn3 = frmMain.packnormalFBX(n3)
    '            If index_mode = FbxLayerElement.ReferenceMode.Direct Then
    '                uv1_ = uvlayer1.DirectArray(v1)
    '                If frmMain.m_U_save_flip.CheckState Then
    '                    uv1_.X *= -1
    '                End If
    '                If frmMain.m_V_load_flip.CheckState Then
    '                    uv1_.Y *= -1
    '                End If
    '                uv2_ = uvlayer1.DirectArray(v2)
    '                If frmMain.m_U_save_flip.CheckState Then
    '                    uv2_.X *= -1
    '                End If
    '                If frmMain.m_V_load_flip.CheckState Then
    '                    uv2_.Y *= -1
    '                End If
    '                uv3_ = uvlayer1.DirectArray(v3)
    '                If frmMain.m_U_save_flip.CheckState Then
    '                    uv3_.X *= -1
    '                End If
    '                If frmMain.m_V_load_flip.CheckState Then
    '                    uv3_.Y *= -1
    '                End If
    '                If has_uv2 Then
    '                    uv1_2 = uvlayer2.DirectArray(v1)
    '                    If frmMain.m_U_save_flip.CheckState Then
    '                        uv1_2.X *= -1
    '                    End If
    '                    If frmMain.m_V_load_flip.CheckState Then
    '                        uv1_2.Y *= -1
    '                    End If
    '                    uv2_2 = uvlayer2.DirectArray(v2)
    '                    If frmMain.m_U_save_flip.CheckState Then
    '                        uv2_2.X *= -1
    '                    End If
    '                    If frmMain.m_V_load_flip.CheckState Then
    '                        uv2_2.Y *= -1
    '                    End If
    '                    uv3_2 = uvlayer2.DirectArray(v3)
    '                    If frmMain.m_U_save_flip.CheckState Then
    '                        uv3_2.X *= -1
    '                    End If
    '                    If frmMain.m_V_load_flip.CheckState Then
    '                        uv3_2.Y *= -1
    '                    End If
    '                End If
    '            Else
    '                Dim uvp As Integer
    '                uvp = uvlayer1.IndexArray.GetAt(j)
    '                uv1_ = uvlayer1.DirectArray.GetAt(uvp)
    '                If frmMain.m_U_save_flip.CheckState Then
    '                    uv1_.X *= -1
    '                End If
    '                If frmMain.m_V_load_flip.CheckState Then
    '                    uv1_.Y *= -1
    '                End If
    '                uvp = uvlayer1.IndexArray.GetAt(j)
    '                If has_uv2 Then
    '                    uv1_2 = uvlayer2.DirectArray.GetAt(uvp)
    '                    If frmMain.m_U_save_flip.CheckState Then
    '                        uv1_2.X *= -1
    '                    End If
    '                    If frmMain.m_V_load_flip.CheckState Then
    '                        uv1_2.Y *= -1
    '                    End If
    '                End If
    '                j += 1
    '                uvp = uvlayer1.IndexArray.GetAt(j)
    '                uv2_ = uvlayer1.DirectArray.GetAt(uvp)
    '                If frmMain.m_U_save_flip.CheckState Then
    '                    uv2_.X *= -1
    '                End If
    '                If frmMain.m_V_load_flip.CheckState Then
    '                    uv2_.Y *= -1
    '                End If
    '                uvp = uvlayer1.IndexArray.GetAt(j)
    '                If has_uv2 Then
    '                    uv2_2 = uvlayer2.DirectArray.GetAt(uvp)
    '                    If frmMain.m_U_save_flip.CheckState Then
    '                        uv2_2.X *= -1
    '                    End If
    '                    If frmMain.m_V_load_flip.CheckState Then
    '                        uv2_2.Y *= -1
    '                    End If
    '                End If
    '                j += 1
    '                uvp = uvlayer1.IndexArray.GetAt(j)
    '                uv3_ = uvlayer1.DirectArray.GetAt(uvp)
    '                If frmMain.m_U_save_flip.CheckState Then
    '                    uv3_.X *= -1
    '                End If
    '                If frmMain.m_V_load_flip.CheckState Then
    '                    uv3_.Y *= -1
    '                End If
    '                uvp = uvlayer1.IndexArray.GetAt(j)
    '                If has_uv2 Then
    '                    uv3_2 = uvlayer2.DirectArray.GetAt(uvp)
    '                    If frmMain.m_U_save_flip.CheckState Then
    '                        uv3_2.X *= -1
    '                    End If
    '                    If frmMain.m_V_load_flip.CheckState Then
    '                        uv3_2.Y *= -1
    '                    End If
    '                End If

    '                j += 1

    '            End If
    '            vtc1.x = vt1.X
    '            vtc1.y = vt1.Y
    '            vtc1.z = vt1.Z
    '            vtc1.n = pn1
    '            vtc1.u = uv1_.X
    '            vtc1.v = uv1_.Y
    '            If has_uv2 Then
    '                vtc1.u2 = uv1_2.X
    '                vtc1.v2 = uv1_2.Y
    '            End If


    '            vtc2.x = vt2.X
    '            vtc2.y = vt2.Y
    '            vtc2.z = vt2.Z
    '            vtc2.n = pn2
    '            vtc2.u = uv2_.X
    '            vtc2.v = uv2_.Y
    '            If has_uv2 Then
    '                vtc2.u2 = uv2_2.X
    '                vtc2.v2 = uv2_2.Y
    '            End If


    '            vtc3.x = vt3.X
    '            vtc3.y = vt3.Y
    '            vtc3.z = vt3.Z
    '            vtc3.n = pn3
    '            vtc3.u = uv3_.X
    '            vtc3.v = uv3_.Y
    '            If has_uv2 Then
    '                vtc3.u2 = uv3_2.X
    '                vtc3.v2 = uv3_2.Y
    '            End If

    '            _group(id).vertices(v1) = vtc1
    '            _group(id).vertices(v2) = vtc2
    '            _group(id).vertices(v3) = vtc3
    '            _group(id).vertices(v1).index_1 = 0
    '            _group(id).vertices(v2).index_1 = 0
    '            _group(id).vertices(v3).index_1 = 0
    '            _group(id).indicies(i + 1).v1 = v1 + off
    '            _group(id).indicies(i + 1).v2 = v2 + off
    '            _group(id).indicies(i + 1).v3 = v3 + off

    '        Next
    '        frmMain.pbar.Visible = False
    '        For i = 0 To nVertices
    '            Try
    '                If _group(id).vertices(i).index_1 = 255 Then
    '                    ReDim Preserve _group(id).vertices(i + 1)
    '                    _group(id).nVertices_ = i
    '                    Exit For
    '                End If

    '            Catch ex As Exception
    '                _group(id).nVertices_ = i

    '            End Try
    '        Next
    '        'at this point, its a good idea to shift the pointers after our current ID.

    '        Dim delta_v As Integer = old_poly_cnt - polycnt 'calculate size differences.. Could be + or -
    '        Dim delta_i As Integer = old_vertice_cnt - _group(id).nVertices_
    '        If old_poly_cnt <> _group(id).nPrimitives_ Or old_vertice_cnt <> _group(id).nVertices_ Then
    '            If id < object_count Then
    '                For i = id + 1 To object_count
    '                    If _group(id + 1).startVertex_ = 0 Then
    '                        delta_i *= -1
    '                        delta_v *= -1
    '                        _group(i).startVertex_ += delta_i
    '                        _group(i).startIndex_ += (delta_v * 3)
    '                        For j = 1 To _group(i).nPrimitives_
    '                            _group(i).indicies(j).v1 += (delta_i)
    '                            _group(i).indicies(j).v2 += (delta_i)
    '                            _group(i).indicies(j).v3 += (delta_i)
    '                        Next
    '                        delta_i *= -1
    '                        delta_v *= -1
    '                    Else
    '                        _group(i).startVertex_ -= delta_i
    '                        _group(i).startIndex_ -= (delta_v * 3)
    '                        For j = 1 To _group(i).nPrimitives_
    '                            _group(i).indicies(j).v1 -= (delta_i)
    '                            _group(i).indicies(j).v2 -= (delta_i)
    '                            _group(i).indicies(j).v3 -= (delta_i)
    '                        Next

    '                    End If
    '                Next
    '            End If
    '            _group(id).nPrimitives_ = polycnt ' adjust total
    '            object_table.Rows(0).Item("obj_cnt") = polycnt
    '            frmMain.Object_tableBindingsource.DataSource = object_table

    '        End If
    '        If has_uv2 Then 'no need if there isnt a used uv2 set
    '            j = 0
    '            'There are 3 streaming uv2s per poly.. so if delta_v is negitive, we will
    '            'need to enlarge the size of uv2_b by 3 * delta_v.

    '            Try

    '                If delta_v < 0 Then
    '                    j = (uv2_b.Length) + -(delta_v * 3)
    '                    ReDim Preserve uv2_b(j)
    '                End If
    '                j = 0
    '                For i = 1 To object_count
    '                    j += _group(i).nPrimitives_
    '                Next
    '                ReDim Preserve uv2_b(j * 3)
    '                j = 0
    '                For i = 1 To object_count
    '                    For k As UInt32 = 0 To _group(i).nVertices_ - 1
    '                        uv2_b(j).u = _group(i).vertices(k).u2
    '                        uv2_b(j).v = _group(i).vertices(k).v2
    '                        j += 1
    '                    Next
    '                Next
    '                ReDim Preserve uv2_b(j)
    '                uv2 = uv2_b
    '                'Else
    '                'ReDim Preserve uv2(polycnt * 3)
    '                'ReDim Preserve uv2_b(polycnt * 3)
    '                'For i = 0 To polycnt
    '                '    uv2(i) = New uvs
    '                '    uv2_b(i) = New uvs
    '                'Next
    '            Catch ex As Exception
    '                has_uv2 = False
    '            End Try
    '        End If
    '        stop_opengl = True ' so we dont use this _object in the middle of creation
    '        ReDim Preserve _object(id).tris(polycnt)
    '        _object(id).count = polycnt
    '        Dim v As vect3
    '        For jj = id To object_count
    '            With _object(jj)
    '                .count = _group(jj).nPrimitives_
    '                For i = 1 To _group(jj).nPrimitives_
    '                    Dim v1 = _group(jj).indicies(i).v1 - _group(jj).startVertex_
    '                    Dim v2 = _group(jj).indicies(i).v2 - _group(jj).startVertex_
    '                    Dim v3 = _group(jj).indicies(i).v3 - _group(jj).startVertex_
    '                    .tris(i) = New triangle
    '                    .tris(i).uv1 = New uv_
    '                    .tris(i).uv2 = New uv_
    '                    .tris(i).uv3 = New uv_
    '                    .tris(i).uv1_2 = New uv_
    '                    .tris(i).uv2_2 = New uv_
    '                    .tris(i).uv3_2 = New uv_
    '                    .tris(i).v1.x = _group(jj).vertices(v1).x
    '                    .tris(i).v1.y = _group(jj).vertices(v1).y
    '                    .tris(i).v1.z = _group(jj).vertices(v1).z
    '                    .tris(i).uv1.u = _group(jj).vertices(v1).u
    '                    .tris(i).uv1.v = _group(jj).vertices(v1).v
    '                    .tris(i).uv1_2.u = _group(jj).vertices(v1).u2
    '                    .tris(i).uv1_2.v = _group(jj).vertices(v1).v2
    '                    v = frmMain.unpackNormal(_group(jj).vertices(v1).n)
    '                    .tris(i).n1.x = v.x
    '                    .tris(i).n1.y = v.y
    '                    .tris(i).n1.z = v.z


    '                    .tris(i).v2.x = _group(jj).vertices(v2).x
    '                    .tris(i).v2.y = _group(jj).vertices(v2).y
    '                    .tris(i).v2.z = _group(jj).vertices(v2).z
    '                    .tris(i).uv2.u = _group(jj).vertices(v2).u
    '                    .tris(i).uv2.v = _group(jj).vertices(v2).v
    '                    .tris(i).uv2_2.u = _group(jj).vertices(v2).u2
    '                    .tris(i).uv2_2.v = _group(jj).vertices(v2).v2
    '                    v = frmMain.unpackNormal(_group(jj).vertices(v2).n)
    '                    .tris(i).n2.x = v.x
    '                    .tris(i).n2.y = v.y
    '                    .tris(i).n2.z = v.z

    '                    .tris(i).v3.x = _group(jj).vertices(v3).x
    '                    .tris(i).v3.y = _group(jj).vertices(v3).y
    '                    .tris(i).v3.z = _group(jj).vertices(v3).z
    '                    .tris(i).uv3.u = _group(jj).vertices(v3).u
    '                    .tris(i).uv3.v = _group(jj).vertices(v3).v
    '                    .tris(i).uv3_2.u = _group(jj).vertices(v3).u2
    '                    .tris(i).uv3_2.v = _group(jj).vertices(v3).v2
    '                    v = frmMain.unpackNormal(_group(jj).vertices(v3).n)
    '                    .tris(i).n3.x = v.x
    '                    .tris(i).n3.y = v.y
    '                    .tris(i).n3.z = v.z


    '                Next
    '            End With
    '            frmMain.update_list(jj)
    '            _object(jj).find_center()
    '        Next jj
    '        _OBJ_ID = id
    '        pManager.Destroy()
    '        frmMain.create_TBNS(id)
    'outofhere:
    '        frmMain.Main_Menu.Enabled = True
    '        frmMain.Timer1.Enabled = True
    '        stop_opengl = False
    '        frmMain.get_total_verts()
    '        frmMain.draw_scene()
    '    End Sub

    Public Sub export_fbx()
        'export FBX
        Dim rootNode As FbxNode
        Dim id As Integer
        Dim model_name As String = ""
        Dim mat_main As String = ""
        Dim mat_NM As String = ""
        Dim mat_uv2 As String = ""
        Dim fbx_locaction As String = My.Settings.fbx_path
        Dim rp As String = Application.StartupPath
        Dim _date As String = Date.Now
        Dim ar = _date.Split(" ")
        _date = ar(0) + " " + ar(1) + ".0"


        Dim vert_string, normal_string, uv1_string, uv2_string, uv_index, indices_string As New StringBuilder

        'Tried everything so lets do it the hard way
        '--------------------------------------------------------------------------
        Dim m_name As String = "Material"
        Dim s_name As String = "Phong"
        Dim EmissiveColor = New FbxDouble3(0.0, 0.0, 0.0)
        Dim AmbientColor = New FbxDouble3(0.9, 0.9, 0.9)
        Dim SpecularColor = New FbxDouble3(0.7, 0.7, 0.7)
        Dim DiffuseColor As New FbxDouble3(0.8, 0.8, 0.8)
        '--------------------------------------------------------------------------
        Dim pManager As FbxSdkManager
        pManager = FbxSdkManager.Create
        'create the material and texture arrays.

        Dim texture_count = textures.Length
        Dim lMaterials(texture_count) As FbxSurfacePhong
        Dim lTextures(texture_count) As FbxTexture
        Dim lTextures_N(texture_count) As FbxTexture
        'make the materials
        For i = 0 To texture_count - 1
            lMaterials(i) = fbx_create_material(pManager, i) 'Material
            lTextures(i) = fbx_create_texture(pManager, i) 'Color Map
            lTextures_N(i) = fbx_create_texture_N(pManager, i) 'Normal Map
        Next

        'create manager and scene
        Dim scene As FbxScene
        scene = FbxScene.Create(pManager, file_name)
        scene.SceneInfo.Author = "Exported using Coffee_'s Tank Exporter tool"
        scene.SceneInfo.Comment = TANK_NAME

        frmFBX.Label1.Visible = False
        frmFBX.Label2.Visible = True
        Dim node_list() = {FbxNode.Create(pManager, model_name)}
        '--------------------------------------------------------------------------
        rootNode = scene.RootNode
        For id = 1 To object_count
            ReDim Preserve node_list(id + 1)
            frmFBX.Label2.Text = "ID: " + id.ToString + vbCrLf
            'If frmFBX.export_textures.Checked Then
            '    If Not _object(id).visible Then
            '        GoTo we_dont_want_this_one_saved
            '    End If
            'End If
            mat_main = FBX_Texture_path + "\" + Path.GetFileNameWithoutExtension(_group(id).color_name) + ".png"
            mat_NM = FBX_Texture_path + "\" + Path.GetFileNameWithoutExtension(_group(id).normal_name) + ".png"
            mat_uv2 = _group(id).detail_name

            model_name = _group(id).name
            node_list(id) = FbxNode.Create(pManager, model_name)

            frmFBX.Label2.Text = model_name


            'create mesh node
            Dim mymesh = fbx_create_mesh(model_name, id, pManager)


            Dim m As New FbxMatrix
            Dim m_ = _object(id).matrix
            m.SetRow(0, New FbxVector4(m_(0), m_(1), m_(2), m_(3)))
            m.SetRow(1, New FbxVector4(m_(4), m_(5), m_(6), m_(7)))
            m.SetRow(2, New FbxVector4(m_(8), m_(9), m_(10), m_(11)))
            m.SetRow(3, New FbxVector4(m_(12), m_(13), m_(14), m_(15)))
            Dim scale As New SlimDX.Vector3
            Dim rot As New SlimDX.Quaternion
            Dim trans As New SlimDX.Vector3
            Dim Mt As New SlimDX.Matrix
            Mt = load_matrix_decompose(m_, trans, scale, rot)
            Dim r_vector As New FbxVector4(rot.X, rot.Y, rot.Z, rot.W)
            Dim t_vector As New FbxVector4(-trans.X, trans.Y, trans.Z)
            Dim s_vector As New FbxVector4(-scale.X, scale.Y, scale.Z, 0.0)
            If id = object_count Then
                't_vector = New FbxVector4(-trans.X, trans.Y, trans.Z)
            End If
            If model_name.ToLower.Contains("chassis") Then
                s_vector.Z *= -1.0
                s_vector.X *= -1.0
            End If
            If id = object_count Then
                s_vector.Z *= -1.0
                s_vector.X *= -1.0
            End If

            'Need a layercontainer to put the texture in.
            Dim layercontainer As FbxLayerContainer = mymesh
            Dim layerElementTexture As FbxLayerElementTexture = layercontainer.GetLayer(0).DiffuseTextures
            Dim layerElementNTexture As FbxLayerElementTexture = layercontainer.GetLayer(0).BumpTextures
            If layerElementTexture Is Nothing Then
                layerElementTexture = FbxLayerElementTexture.Create(layercontainer, "diffuseMap")
                layercontainer.GetLayer(0).DiffuseTextures = layerElementTexture
                layerElementNTexture = FbxLayerElementTexture.Create(layercontainer, "normalMap")
                layercontainer.GetLayer(0).BumpTextures = layerElementNTexture
            End If
            'not 100% sure about the translucent but it isn't breaking anything.
            layerElementTexture.Blend_Mode = FbxLayerElementTexture.BlendMode.Translucent
            layerElementTexture.Alpha = 1.0
            layerElementTexture.Mapping_Mode = FbxLayerElement.MappingMode.AllSame
            layerElementTexture.Reference_Mode = FbxLayerElement.ReferenceMode.Direct

            layerElementNTexture.Blend_Mode = FbxLayerElementTexture.BlendMode.Translucent
            layerElementNTexture.Alpha = 1.0
            layerElementNTexture.Mapping_Mode = FbxLayerElement.MappingMode.AllSame
            layerElementNTexture.Reference_Mode = FbxLayerElement.ReferenceMode.Direct

            'add the texture from the texture array using the Texture ID for this mesh section
            layerElementTexture.DirectArray.Add(lTextures(_group(id).texture_id))
            layerElementNTexture.DirectArray.Add(lTextures_N(_group(id).texture_id))
            node_list(id).NodeAttribute = mymesh
            node_list(id).SetDefaultR(r_vector)
            node_list(id).SetDefaultT(t_vector)
            node_list(id).SetDefaultS(s_vector)
            If node_list(id).IsValid And frmFBX.export_textures.Checked Then ' useless test but Im leaving it.
                'add the texture from the array using this models texture ID
                node_list(id).AddMaterial(lMaterials(_group(id).texture_id))
                '---------------------------------------
                'If we dont connect this texture to this node, it will never show up!
                node_list(id).ConnectSrcObject(lMaterials(_group(id).texture_id), FbxConnectionType.ConnectionDefault)
            End If
            node_list(id).Shading_Mode = FbxNode.ShadingMode.TextureShading ' not even sure this is needed but what ever.

            rootNode.AddChild(node_list(id))
            rootNode.ConnectSrcObject(node_list(id), FbxConnectionType.ConnectionDefault)

we_dont_want_this_one_saved:
        Next 'Id
        scene.SetCurrentTake("Show all faces")



        'time to save... not sure im even close to having what i need to save but fuck it!
        Dim exporter As Skill.FbxSDK.IO.FbxExporter = FbxExporter.Create(pManager, "")
        If Not exporter.Initialize(frmMain.SaveFileDialog1.FileName) Then
            MsgBox("fbx unable to initialize exporter!", MsgBoxStyle.Exclamation, "FBX Error..")
            GoTo outahere
        End If
        Dim version As Version = Skill.FbxSDK.IO.FbxIO.CurrentVersion
        Console.Write(String.Format("FBX version number for this FBX SDK is {0}.{1}.{2}", _
                          version.Major, version.Minor, version.Revision))
        If frmFBX.export_as_binary_cb.Checked Then
            exporter.FileFormat = IO.FileFormat.FbxBinary
        Else
            exporter.FileFormat = IO.FileFormat.FbxAscii
        End If

        Dim exportOptions As Skill.FbxSDK.IO.FbxStreamOptionsFbxWriter _
                = Skill.FbxSDK.IO.FbxStreamOptionsFbxWriter.Create(pManager, "")
        If pManager.IOPluginRegistry.WriterIsFBX(IO.FileFormat.FbxAscii) Then

            ' Export options determine what kind of data is to be imported.
            ' The default (except for the option eEXPORT_TEXTURE_AS_EMBEDDED)
            ' is true, but here we set the options explictly.
            exportOptions.SetOption(Skill.FbxSDK.IO.FbxStreamOptionsFbx.MATERIAL, True)
            exportOptions.SetOption(Skill.FbxSDK.IO.FbxStreamOptionsFbx.TEXTURE, True)
            exportOptions.SetOption(Skill.FbxSDK.IO.FbxStreamOptionsFbx.EMBEDDED, False)
            exportOptions.SetOption(Skill.FbxSDK.IO.FbxStreamOptionsFbx.LINK, True)
            exportOptions.SetOption(Skill.FbxSDK.IO.FbxStreamOptionsFbx.SHAPE, False)
            exportOptions.SetOption(Skill.FbxSDK.IO.FbxStreamOptionsFbx.GOBO, False)
            exportOptions.SetOption(Skill.FbxSDK.IO.FbxStreamOptionsFbx.ANIMATION, False)
            exportOptions.SetOption(Skill.FbxSDK.IO.FbxStreamOptionsFbx.GLOBAL_SETTINGS, False)
            exportOptions.SetOption(Skill.FbxSDK.IO.FbxStreamOptionsFbx.MEDIA, False)
        End If
        Dim status = exporter.Export(scene, exportOptions)
        exporter.Destroy()
        pManager.Destroy()
        'textureAmbientLayer.Destroy()
        'textureDiffuseLayer.Destroy()
outahere:
        frmFBX.Label1.Visible = True
        frmFBX.Label2.Visible = False

    End Sub
    Public Function fbx_create_material(pManager As FbxSdkManager, id As Integer) As FbxSurfacePhong
        Dim lMaterial As FbxSurfacePhong
        Dim m_name As String = "Material"
        Dim s_name As String = "Phong"
        'need colors defined
        Dim EmissiveColor = New FbxDouble3(0.0, 0.0, 0.0)
        Dim AmbientColor = New FbxDouble3(0.9, 0.9, 0.9)
        Dim SpecularColor = New FbxDouble3(0.7, 0.7, 0.7)
        Dim DiffuseColor As New FbxDouble3(0.8, 0.8, 0.8)
        'Need a name for this material
        lMaterial = FbxSurfacePhong.Create(pManager, m_name + ":" + id.ToString("000"))
        lMaterial.EmissiveColor = EmissiveColor
        lMaterial.AmbientColor = AmbientColor
        lMaterial.DiffuseColor = DiffuseColor
        lMaterial.SpecularColor = SpecularColor
        lMaterial.SpecularFactor = 0.3
        lMaterial.TransparencyFactor = 0.0
        lMaterial.Shininess = 60.0
        lMaterial.ShadingModel = s_name
        Return lMaterial
    End Function

    Public Function fbx_create_texture(pManager As FbxSdkManager, id As Integer) As FbxTexture
        'need a name for this texture
        Dim texture = FbxTexture.Create(pManager, "DiffuseMap" + ":" + id.ToString("000"))
        ' Set texture properties.
        texture.SetFileName(FBX_Texture_path + "\" + Path.GetFileNameWithoutExtension(textures(id).c_name) + ".png") 'Get the Texture path from the list
        texture.TextureUseType = FbxTexture.TextureUse.Standard
        texture.Mapping = FbxTexture.MappingType.Uv
        texture.MaterialUseType = FbxTexture.MaterialUse.Model
        texture.SwapUV = False
        texture.SetTranslation(0.0, 0.0)
        texture.SetScale(1.0, 1.0)
        texture.SetRotation(0.0, 0.0)
        Return texture
    End Function
    Public Function fbx_create_texture_N(pManager As FbxSdkManager, id As Integer) As FbxTexture
        'need a name for this texture
        Dim texture = FbxTexture.Create(pManager, "NormalMap" + ":" + id.ToString("000"))
        ' Set texture properties.
        texture.SetFileName(FBX_Texture_path + "\" + Path.GetFileNameWithoutExtension(textures(id).n_name) + ".png") 'Get the Texture path from the list
        texture.TextureUseType = FbxTexture.TextureUse.BumpNormalMap
        texture.Mapping = FbxTexture.MappingType.Uv
        texture.MaterialUseType = FbxTexture.MaterialUse.Model
        texture.SwapUV = False
        texture.SetTranslation(0.0, 0.0)
        texture.SetScale(1.0, 1.0)
        texture.SetRotation(0.0, 0.0)
        Return texture
    End Function

    Private Function load_matrix_decompose(data() As Single, ByRef trans As SlimDX.Vector3, ByRef scale As SlimDX.Vector3, ByRef rot As SlimDX.Quaternion) As SlimDX.Matrix
        Dim m_ As New SlimDX.Matrix
        For i = 0 To 3
            For k = 0 To 3
                m_(i, k) = data((i * 4) + k)
            Next
        Next
        'm_(0, 0) *= -1.0
        'm_(2, 0) *= -1.0
        'm_(2, 0) *= -1.0
        'm_(2, 2) *= -1.0
        m_.Decompose(scale, rot, trans)
        Return m_
    End Function
    Public Function fbx_create_mesh(model_name As String, id As Integer, pManager As FbxSdkManager) As FbxMesh
        Dim myMesh As FbxMesh
        myMesh = FbxMesh.Create(pManager, model_name)
        Dim cnt = _group(id).nPrimitives_
        Dim off As UInt32
        Dim v As vect3Norm
        Dim v4 As New FbxVector4
        Dim I As Integer
        off = _group(id).startVertex_

        'first we load all the vertices for the _group data
        myMesh.InitControlPoints(_group(id).nVertices_) ' size of array
        'add in the vertices (or control points as its called in FBX)
        Dim cp_array(myMesh.ControlPointsCount - 1) As FbxVector4
        For I = 0 To myMesh.ControlPointsCount - 1
            cp_array(I) = New FbxVector4
            cp_array(I).X = _group(id).vertices(I).x
            cp_array(I).Y = _group(id).vertices(I).y
            cp_array(I).Z = _group(id).vertices(I).z
        Next
        myMesh.ControlPoints = cp_array ' push it in to the mesh object
        'create or get the layer 0
        Dim layer As FbxLayer = myMesh.GetLayer(0)
        If layer Is Nothing Then
            myMesh.CreateLayer()
            layer = myMesh.GetLayer(0)
        End If

        'normals.. seems to be working ok
        Dim layerElementNormal = FbxLayerElementNormal.Create(myMesh, "Normals")
        layerElementNormal.Mapping_Mode = FbxLayerElement.MappingMode.ByPolygonVertex
        layerElementNormal.Reference_Mode = FbxLayerElement.ReferenceMode.Direct
        'time to assign the normals to each control point.

        For I = 1 To _group(id).nPrimitives_
            Dim v1 = _group(id).indicies(I).v1
            Dim v2 = _group(id).indicies(I).v2
            Dim v3 = _group(id).indicies(I).v3
            v = unpackNormal(_group(id).vertices(v1 - off).n, _group(id).BPVT_mode)
            v4.X = -v.nx
            v4.Y = v.ny
            v4.Z = v.nz
            layerElementNormal.DirectArray.Add(v4)

            v = unpackNormal(_group(id).vertices(v2 - off).n, _group(id).BPVT_mode)
            v4.X = -v.nx
            v4.Y = v.ny
            v4.Z = v.nz
            layerElementNormal.DirectArray.Add(v4)

            v = unpackNormal(_group(id).vertices(v3 - off).n, _group(id).BPVT_mode)
            v4.X = -v.nx
            v4.Y = v.ny
            v4.Z = v.nz
            layerElementNormal.DirectArray.Add(v4)
        Next

        layer.Normals = layerElementNormal

        '--------------------------------------------------------------------------
        Dim v_2 As New FbxVector2
        ' Create UV for Bump channel
        'Dim UVBumpLayer As FbxLayerElementUV = FbxLayerElementUV.Create(myMesh, "BumpUV")
        'UVBumpLayer.Mapping_Mode = FbxLayerElement.MappingMode.ByControlPoint
        'UVBumpLayer.Reference_Mode = FbxLayerElement.ReferenceMode.Direct
        'layer.SetUVs(UVBumpLayer, FbxLayerElement.LayerElementType.BumpTextures)
        'For I = 0 To myMesh.ControlPointsCount - 1
        '    If frmFBX.flip_u.Checked Then
        '        v_2.X = _group(id).vertices(I).u * -1
        '    Else
        '        v_2.X = _group(id).vertices(I).u
        '    End If

        '    If Not frmFBX.flip_v.Checked Then
        '        v_2.Y = _group(id).vertices(I).v * -1
        '    Else
        '        v_2.Y = _group(id).vertices(I).v
        '    End If
        '    UVBumpLayer.DirectArray.Add(v_2)
        '    'If fbx_cancel Then
        '    '    Return myMesh
        '    'End If
        'Next

        ' Create UV for Diffuse channel
        Dim UVDiffuseLayer As FbxLayerElementUV = FbxLayerElementUV.Create(myMesh, "DiffuseUV")
        UVDiffuseLayer.Mapping_Mode = FbxLayerElement.MappingMode.ByControlPoint
        UVDiffuseLayer.Reference_Mode = FbxLayerElement.ReferenceMode.Direct
        layer.SetUVs(UVDiffuseLayer, FbxLayerElement.LayerElementType.DiffuseTextures)
        For I = 0 To myMesh.ControlPointsCount - 1
            If frmFBX.flip_u.Checked Then
                v_2.X = _group(id).vertices(I).u * -1
            Else
                v_2.X = _group(id).vertices(I).u
            End If

            If Not frmFBX.flip_v.Checked Then
                v_2.Y = _group(id).vertices(I).v * -1
            Else
                v_2.Y = _group(id).vertices(I).v
            End If
            UVDiffuseLayer.DirectArray.Add(v_2)
            'If fbx_cancel Then
            '    Return myMesh
            'End If
        Next



        'Now we have set the UVs as eINDEX_TO_DIRECT reference and in eBY_POLYGON_VERTEX  mapping mode
        'we must update the size of the index array.
        UVDiffuseLayer.IndexArray.Count = _group(id).nPrimitives_
        'in the same way with Textures, but we are in eBY_POLYGON,
        'we should have N polygons (1 for each faces of the object)
        Dim pos As UInt32 = 0
        Dim n As UInt32 = 1
        Dim j As UInt32 = 0
        For I = 0 To _group(id).nPrimitives_ - 1
            myMesh.BeginPolygon(-1, -1, -1, False)

            j = 0
            pos = _group(id).indicies(n).v1 - off
            myMesh.AddPolygon(pos)
            'UVBumpLayer.IndexArray.SetAt(pos, j)
            UVDiffuseLayer.IndexArray.SetAt(pos, j)
            j += 1
            pos = _group(id).indicies(n).v2 - off
            myMesh.AddPolygon(pos)
            'UVBumpLayer.IndexArray.SetAt(pos, j)
            UVDiffuseLayer.IndexArray.SetAt(pos, j)
            j += 1
            pos = _group(id).indicies(n).v3 - off
            myMesh.AddPolygon(pos)
            'UVBumpLayer.IndexArray.SetAt(pos, j)
            UVDiffuseLayer.IndexArray.SetAt(pos, j)
            n += 1
            myMesh.EndPolygon()
            'frmMain.pbar.Value = I

        Next
        Return myMesh
    End Function
    Private Function unpackNormal_8_8_8(ByVal packed As UInt32) As vect3Norm
        'Console.WriteLine(packed.ToString("x"))
        Dim pkz, pky, pkx As Int32
        pkx = CLng(packed) And &HFF Xor 127
        pky = CLng(packed >> 8) And &HFF Xor 127
        pkz = CLng(packed >> 16) And &HFF Xor 127

        Dim x As Single = (pkx)
        Dim y As Single = (pky)
        Dim z As Single = (pkz)

        Dim p As New vect3Norm
        If x > 127 Then
            x = -128 + (x - 128)
        End If
        If y > 127 Then
            y = -128 + (y - 128)
        End If
        If z > 127 Then
            z = -128 + (z - 128)
        End If
        p.nx = CSng(x) / 127
        p.ny = CSng(y) / 127
        p.nz = CSng(z) / 127
        Dim len As Single = Sqrt((p.nx ^ 2) + (p.ny ^ 2) + (p.nz ^ 2))

        'avoid division by 0
        If len = 0.0F Then len = 1.0F
        'len = 1.0
        'reduce to unit size
        p.nx = (p.nx / len)
        p.ny = -(p.ny / len)
        p.nz = -(p.nz / len)
        'Console.WriteLine(p.x.ToString("0.000000") + " " + p.y.ToString("0.000000") + " " + p.z.ToString("0.000000"))
        Return p
    End Function
    Private Function unpackNormal(ByVal packed As UInt32, type As Boolean) As vect3Norm
        If type Then
            Return unpackNormal_8_8_8(packed)
        End If
        Dim pkz, pky, pkx As Int32
        pkz = packed And &HFFC00000
        pky = packed And &H4FF800
        pkx = packed And &H7FF

        Dim z As Int32 = pkz >> 22
        Dim y As Int32 = (pky << 10L) >> 21
        Dim x As Int32 = (pkx << 21L) >> 21
        Dim p As New vect3Norm

        p.nx = CSng(x) / 1023.0!
        p.ny = CSng(y) / 1023.0!
        p.nz = CSng(z) / 511.0!
        Dim len As Single = Sqrt((p.nx ^ 2) + (p.ny ^ 2) + (p.nz ^ 2))

        'avoid division by 0
        If len = 0.0F Then len = 1.0F

        'reduce to unit size
        p.nx = (p.nx / len)
        p.ny = (p.ny / len)
        p.nz = (p.nz / len)
        Return p
    End Function

End Module
