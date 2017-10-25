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
Imports cttools
Module modFBX
    Public FBX_Texture_path As String
    Public fbxgrp() As _grps
    Public ctz As New cttools.norm_utilities
    Public FBX_LOADED As Boolean = False
    Public m_groups() As mgrp_
    Public Structure mgrp_
        Public list() As Integer
        Public m_type As Integer
        Public cnt As Integer
        Public f_name() As String
        Public package_id() As Integer
        Public changed As Boolean
        Public new_objects As Boolean
    End Structure
    Public Sub remove_loaded_fbx()
        If FBX_LOADED Then
            FBX_LOADED = False
            For ii = 1 To fbxgrp.Length - 2
                Gl.glDeleteTextures(1, fbxgrp(ii).color_Id)
                Gl.glFinish()
                Gl.glDeleteTextures(1, fbxgrp(ii).color_Id)
                Gl.glFinish()
                Gl.glDeleteLists(fbxgrp(ii).call_list, 1)
                Gl.glFinish()
                frmMain.m_show_fbx.Visible = False
                frmMain.m_show_fbx.Checked = False
            Next
            ReDim fbxgrp(0)
            GC.Collect() 'clean up garbage
            GC.WaitForFullGCComplete()
        End If
    End Sub

    Private Sub displayMatrix(m As FbxXMatrix, ByVal name As String)

        Console.WriteLine(name + "------------------------------------------")
        For i = 0 To 3
            For j = 0 To 3
                Console.Write(Round(m.Get(j, i), 6).ToString + vbTab + vbTab)
            Next
            Console.Write(vbCrLf)
        Next

        Console.Write(vbCrLf)

    End Sub



    Public Sub import_FBX()
        'fbx import sub
        Dim j As UInt32 = 0
        Dim i As UInt32 = 0
        Dim start_index As Integer = 0
        Dim start_vertex As Integer = 0
        frmMain.OpenFileDialog1.InitialDirectory = My.Settings.fbx_path
        frmMain.OpenFileDialog1.Filter = "AutoDesk (*.FBX)|*.fbx"
        frmMain.OpenFileDialog1.Title = "Import FBX..."
        If frmMain.OpenFileDialog1.FileName = "OpenFileDialog1" Then
            frmMain.OpenFileDialog1.FileName = ""
        End If
        If Not frmMain.OpenFileDialog1.ShowDialog = Forms.DialogResult.OK Then
            Return
        End If

        My.Settings.fbx_path = Path.GetDirectoryName(frmMain.OpenFileDialog1.FileName)
        frmMain.clean_house()
        remove_loaded_fbx()
        frmMain.info_Label.Visible = True
        frmMain.m_show_bsp2.Checked = False

        Dim pManager As FbxSdkManager
        Dim scene As FbxScene
        pManager = FbxSdkManager.Create
        scene = FbxScene.Create(pManager, "My Scene")
        Dim fileformat As Integer = Skill.FbxSDK.IO.FileFormat.FbxAscii
        'Detect the file format of the file to be imported            
        Dim filename = frmMain.OpenFileDialog1.FileName
        If Not pManager.IOPluginRegistry.DetectFileFormat(filename, fileformat) Then

            ' Unrecognizable file format.
            ' Try to fall back to SDK's native file format (an FBX binary file).
            fileformat = pManager.IOPluginRegistry.NativeReaderFormat
        End If

        Dim importOptions = Skill.FbxSDK.IO.FbxStreamOptionsFbxReader.Create(pManager, "")
        Dim importer As Skill.FbxSDK.IO.FbxImporter = Skill.FbxSDK.IO.FbxImporter.Create(pManager, "")

        importer.FileFormat = fileformat    ' ascii only for now
        Dim imp_status As Boolean = importer.Initialize(filename)
        If Not imp_status Then
            MsgBox("Failed to open " + frmMain.OpenFileDialog1.FileName, MsgBoxStyle.Exclamation, "FBX Load Error...")
            pManager.Destroy()
            GoTo outofhere
        End If
        If Not importer.IsFBX Then
            MsgBox("Are you sure this is a FBX file? " + vbCrLf + frmMain.OpenFileDialog1.FileName, MsgBoxStyle.Exclamation, "FBX Load Error...")
            pManager.Destroy()
            GoTo outofhere
        End If
        importOptions.SetOption(Skill.FbxSDK.IO.FbxStreamOptionsFbx.MATERIAL, True)
        importOptions.SetOption(Skill.FbxSDK.IO.FbxStreamOptionsFbx.TEXTURE, True)
        importOptions.SetOption(Skill.FbxSDK.IO.FbxStreamOptionsFbx.LINK, True)
        importOptions.SetOption(Skill.FbxSDK.IO.FbxStreamOptionsFbx.SHAPE, True)
        importOptions.SetOption(Skill.FbxSDK.IO.FbxStreamOptionsFbx.GOBO, True)
        importOptions.SetOption(Skill.FbxSDK.IO.FbxStreamOptionsFbx.ANIMATION, True)
        importOptions.SetOption(Skill.FbxSDK.IO.FbxStreamOptionsFbx.GLOBAL_SETTINGS, True)

        imp_status = importer.Import(scene, importOptions)

        Dim rootnode As FbxNode = scene.RootNode

        'Dim systemunit = scene.FindProperty("FbxSystemUint")
        Dim p As FbxProperty = rootnode.GetFirstProperty

        Dim sc = rootnode.Scaling.GetValueAsDouble3
        While 1
            Console.WriteLine(p.Name)
            p = rootnode.GetNextProperty(p)
            If Not p.IsValid Then Exit While
        End While

        Dim nodecnt As Int32 = rootnode.GetChildCount
        'make room for the mesh data
        ReDim fbxgrp(nodecnt)
        For i = 1 To nodecnt
            fbxgrp(i) = New _grps
        Next
        Dim cnt As Integer = 0

        Dim childnode As FbxNode
        Dim mesh As FbxMesh = Nothing
        'Dim geo As FbxGeometry = Nothing
        For i = 1 To nodecnt
            childnode = rootnode.GetChild(i - 1)
            Try

                mesh = childnode.Mesh
                If mesh IsNot Nothing Then
                    fbxgrp(i).name = childnode.NameOnly
                    'get transform information -------------------------------------
                    Dim fbx_matrix As New FbxXMatrix
                    fbxgrp(i).rotation = New FbxVector4
                    fbxgrp(i).translation = New FbxVector4
                    fbxgrp(i).scale = New FbxVector4
                    fbxgrp(i).scale.X = 1.0
                    fbxgrp(i).scale.Y = 1.0
                    fbxgrp(i).scale.Z = 1.0
                    'childnode.ComputeLocalState(0, True)
                    Dim inheritType = childnode.InheritType

                    If inheritType = FbxTransformInheritType.Rrs Then

                    End If
                    If inheritType = FbxTransformInheritType.RrSs Then

                    End If
                    If inheritType = FbxTransformInheritType.RSrs Then

                    End If
                    Dim t As New FbxTime
                    Dim GlobalUnitScale = scene.GlobalSettings.FindProperty("UnitScaleFactor", False).GetValueAsDouble
                    'Dim eval As FbxEvaluationInfo
                    'Dim er = childnode .Evaluate(
                    Dim ls = childnode.GetLocalSFromDefaultTake(FbxNode.PivotSet.SourceSet)
                    If ls.X = 1.0 Then
                        ls.X = 0.1
                        ls.Y = 0.1
                        ls.Z = 1.0
                    End If

                    Dim nodeGT = rootnode.GetGlobalFromDefaultTake(FbxNode.PivotSet.DestinationSet)


                    Dim lr = childnode.GetLocalRFromDefaultTake(FbxNode.PivotSet.SourceSet)
                    Dim lt = childnode.GetLocalTFromCurrentTake(t)
                    Dim gr = childnode.Parent.GetLocalRFromCurrentTake(t)
                    'Dim props60 = globalsettings.Find("properties60", False)
                    'Dim Uscale = globalsettings.Item(0)
                    'Dim US = Uscale(0)
                    'Dim SCALE = US.Item(0).GetValueAsDouble
                    'Dim PS As New FbxVector4

                    'Dim m As FbxMatrix = SE
                    Dim scaling = childnode.Scaling.GetValueAsDouble3

                    fbxgrp(i).rotation = childnode.GetGeometricRotation(FbxNode.PivotSet.SourceSet)
                    fbxgrp(i).translation = childnode.GetGeometricTranslation(FbxNode.PivotSet.SourceSet)
                    fbxgrp(i).scale = childnode.GetGeometricScaling(FbxNode.PivotSet.SourceSet)
                    fbx_matrix.SetIdentity()
                    'fbx_matrix.SetTRS(fbxgrp(i).translation, fbxgrp(i).rotation, fbxgrp(i).scale)

                    Dim dr As New FbxVector4
                    Dim dt As New FbxVector4
                    Dim ds As New FbxVector4

                    'childnode.GetLocalState(dt, dr, ds)
                    Dim gm = childnode.GetGlobalFromCurrentTake(t)

                    childnode.GetDefaultR(dr)
                    childnode.GetDefaultS(ds)
                    childnode.GetDefaultT(dt)
                    fbxgrp(i).rotation = childnode.GetGeometricRotation(FbxNode.PivotSet.SourceSet)
                    fbxgrp(i).translation = childnode.GetGeometricTranslation(FbxNode.PivotSet.SourceSet)
                    fbxgrp(i).scale = childnode.GetGeometricScaling(FbxNode.PivotSet.SourceSet)
                    Dim TnR As Double = 0
                    Try
                        TnR = Round(fbxgrp(i).rotation.X, 6) + Round(fbxgrp(i).rotation.Y, 6) + Round(fbxgrp(i).rotation.Z, 6) _
                            + Round(fbxgrp(i).translation.X, 6) + Round(fbxgrp(i).translation.Y, 6) + Round(fbxgrp(i).translation.Z, 6)
                    Catch ex As Exception

                    End Try
                    'dr.X *= 0.017453293
                    'dr.Y *= 0.017453293
                    'dr.Z *= 0.017453293
                    'check to see if the scale is zero for X,Y and Z. If it is. use the LCL values
                    'If fbxgrp(i).scale.X + fbxgrp(i).scale.Y + fbxgrp(i).scale.Z = 0 Then

                    fbx_matrix.SetTRS(lt, lr, ds)
                    fbx_matrix = gm
                    'If TnR = 0.0 Then
                    '    'dr.Y *= -1
                    '    'Dim tn = dt.Z
                    '    'dt.Z = dt.Y
                    '    'dt.Y = tn
                    'Else
                    '    fbx_matrix.SetTRS(fbxgrp(i).translation, fbxgrp(i).rotation * 3.141592654, fbxgrp(i).scale)
                    'End If
                    'Else
                    'End If
                    'fbx_matrix.SetTRS(fbxgrp(i).translation + dt, fbxgrp(i).rotation + dr, fbxgrp(i).scale * ds)
                    'Console.WriteLine("node id: " + i.ToString)
                    'fbx_matrix = CaculateGlobalTransform(childnode)
                    fbx_matrix.Transpose()

                    build_fbx_matrix(i, fbx_matrix)
                    If TnR <> 0.0 Then
                        'Not sure why but the exported fbx matrix is fucked up..
                        'This rounds to closet whole number.
                        For ip = 0 To 11
                            'fbxgrp(i).matrix(ip) = Math.Round(fbxgrp(i).matrix(ip), 6, MidpointRounding.AwayFromZero)
                        Next
                    End If
                    '---------------------------------------------------------------
                    Dim mat_cnt As Integer = mesh.Node.GetSrcObjectCount(FbxSurfaceMaterial.ClassId)
                    Dim material As FbxSurfaceMaterial = mesh.Node.GetSrcObject(FbxSurfaceMaterial.ClassId, 0)
                    Dim property_ As FbxProperty = Nothing
                    Dim texture As FbxTexture
                    'we never read a Ambient texture. Only Diffuse and Bump....
                    Dim uv_scaling, uv_offset As New FbxVector2
                    Try
                        'diffuse texture.. color_name
                        property_ = material.FindProperty(FbxSurfaceMaterial.SDiffuse)
                        texture = property_.GetSrcObject(FbxTexture.ClassId, 0)
                        uv_offset.x = texture.TranslationU
                        uv_offset.y = texture.TranslationV
                        uv_scaling.x = texture.ScaleU
                        uv_scaling.y = texture.ScaleV
                        fbxgrp(i).color_name = texture.FileName
                        fbxgrp(i).color_Id = -1
                        frmMain.info_Label.Text = "Loading Texture: " + texture.FileName
                        Application.DoEvents()
                        fbxgrp(i).color_Id = get_fbx_texture(texture.FileName)
                    Catch ex As Exception
                    End Try

                    Try
                        'normal map... normal_name
                        property_ = material.FindProperty(FbxSurfaceMaterial.SBump)
                        texture = property_.GetSrcObject(FbxTexture.ClassId, 0)
                        fbxgrp(i).normal_name = texture.FileName
                        frmMain.info_Label.Text = "Loading Texture: " + texture.FileName
                        Application.DoEvents()
                        fbxgrp(i).normal_Id = -1
                        fbxgrp(i).normal_Id = get_fbx_texture(texture.FileName)
                        fbxgrp(i).bumped = True
                    Catch ex As Exception

                    End Try

                    'geo = childnode.NodeAttribute
                    '##############################################
                    'get sizes
                    Dim polycnt As UInt32 = mesh.PolygonCount
                    Dim nVertices = mesh.Normals.Count
                    ReDim Preserve fbxgrp(i).vertices(nVertices)
                    ReDim Preserve fbxgrp(i).indicies(polycnt)
                    fbxgrp(i).nPrimitives_ = polycnt
                    fbxgrp(i).nVertices_ = nVertices
                    fbxgrp(i).startIndex_ = start_index : start_index += polycnt * 3
                    fbxgrp(i).startVertex_ = start_vertex : start_vertex += nVertices * 40
                    '###############################################
                    Dim uvlayer1 As FbxLayerElementUV = mesh.GetLayer(0).GetUVs
                    Dim colorLayer1 As FbxLayerElementVertexColor = mesh.GetLayer(0).VertexColors
                    Dim index_mode = uvlayer1.Reference_Mode
                    Dim eNormals As FbxLayerElementNormal = mesh.GetLayer(0).Normals
                    Dim uv2Layer As FbxLayerElementUV = Nothing
                    If mesh.UVLayerCount = 3 Then
                        uv2Layer = mesh.GetLayer(1).GetUVs
                        fbxgrp(i).has_uv2 = 1
                    End If
                    '------ Look for vertexColor information
                    '-----------------------------------------------------------------------------------
                    '3DS Max crashes exporting FBX files that contain a VC channel.
                    'There is no good work around for this I can do in this app.
                    'Therefore, I will read the existing color for the old model and resize it to fit
                    'any change to the models size in vertices and wrtie that data to the .prmititive file.
                    '-----------------------------------------------------------------------------------
                    'Dim colorBuff() As Byte
                    'Dim cpnt As Integer = 0
                    'Dim r, g, b, a As Byte
                    'If colorLayer1 IsNot Nothing Then
                    '    fbxgrp(i).has_color = 1
                    '    ReDim colorBuff(polycnt * 4 * 3)
                    '    For j = 0 To polycnt - 1
                    '        Dim v1 = mesh.GetPolygonVertex(j, 0)
                    '        Dim v2 = mesh.GetPolygonVertex(j, 1)
                    '        Dim v3 = mesh.GetPolygonVertex(j, 2)
                    '        Dim c1 = colorLayer1.DirectArray(v1)
                    '        colorBuff(cpnt + 0) = CByte(c1.Red * 255)
                    '        colorBuff(cpnt + 1) = CByte(c1.Red * 255)
                    '        colorBuff(cpnt + 2) = CByte(c1.Red * 255)
                    '        colorBuff(cpnt + 3) = CByte(c1.Red * 255)
                    '        cpnt += 4
                    '    Next
                    '    ReDim fbxgrp(i).color_data(cpnt - 4)
                    '    ReDim Preserve colorBuff(cpnt - 4)
                    '    colorBuff.CopyTo(fbxgrp(i).color_data, 0) ' copy this to the current fbxgrp
                    'End If

                    '------ we use this to resize the vertices array and get the right count of vertices.
                    '-----------------------------------------------------------------------------------
                    Dim uv1_, uv2_, uv3_ As FbxVector2
                    Dim uv1_2, uv2_2, uv3_2 As New FbxVector2
                    Dim vtc1, vtc2, vtc3 As New vertice_
                    Dim n1, n2, n3 As New FbxVector4
                    Dim k As Integer = 0
                    cnt = 0
                    ' get indices
                    For j = 0 To polycnt - 1
                        fbxgrp(i).indicies(j).v1 = mesh.GetPolygonVertex(j, 0)
                        fbxgrp(i).indicies(j).v2 = mesh.GetPolygonVertex(j, 1)
                        fbxgrp(i).indicies(j).v3 = mesh.GetPolygonVertex(j, 2)
                        If fbxgrp(i).indicies(j).v1 > cnt Then cnt = fbxgrp(i).indicies(j).v1
                        If fbxgrp(i).indicies(j).v2 > cnt Then cnt = fbxgrp(i).indicies(j).v2
                        If fbxgrp(i).indicies(j).v3 > cnt Then cnt = fbxgrp(i).indicies(j).v3
                    Next
                    ReDim fbxgrp(i).vertices(cnt)
                    fbxgrp(i).nVertices_ = cnt + 1
                    For j = 0 To cnt
                        fbxgrp(i).vertices(j) = New vertice_
                        fbxgrp(i).vertices(j).index_1 = 255
                    Next
                    'get mesh verts, normals and uvs
                    For j = 0 To polycnt - 1
                        Application.DoEvents()
                        Dim v1 = mesh.GetPolygonVertex(j, 0)
                        Dim v2 = mesh.GetPolygonVertex(j, 1)
                        Dim v3 = mesh.GetPolygonVertex(j, 2)
                        Dim vt1 = mesh.GetControlPointAt(v1) 'verts
                        Dim vt2 = mesh.GetControlPointAt(v2)
                        Dim vt3 = mesh.GetControlPointAt(v3)
                        mesh.GetPolygonVertexNormal(j, 0, n1) 'normals
                        mesh.GetPolygonVertexNormal(j, 1, n2)
                        mesh.GetPolygonVertexNormal(j, 2, n3)
                        If index_mode = FbxLayerElement.ReferenceMode.Direct Then ' uvs
                            uv1_ = uvlayer1.DirectArray(v1) * uv_scaling + uv_offset
                            uv2_ = uvlayer1.DirectArray(v2) * uv_scaling + uv_offset
                            uv3_ = uvlayer1.DirectArray(v3) * uv_scaling + uv_offset
                            If uv2Layer IsNot Nothing Then
                                uv1_2 = uv2Layer.DirectArray(v1) * uv_scaling + uv_offset
                                uv2_2 = uv2Layer.DirectArray(v2) * uv_scaling + uv_offset
                                uv3_2 = uv2Layer.DirectArray(v3) * uv_scaling + uv_offset

                            End If
                        Else
                            Dim uvp = uvlayer1.IndexArray.GetAt(k)
                            uv1_ = uvlayer1.DirectArray.GetAt(uvp) * uv_scaling + uv_offset
                            uvp = uvlayer1.IndexArray.GetAt(k + 1)
                            uv2_ = uvlayer1.DirectArray.GetAt(uvp) * uv_scaling + uv_offset
                            uvp = uvlayer1.IndexArray.GetAt(k + 2)
                            uv3_ = uvlayer1.DirectArray.GetAt(uvp) * uv_scaling + uv_offset
                            If uv2Layer IsNot Nothing Then
                                uvp = uv2Layer.IndexArray.GetAt(k)
                                uv1_2 = uv2Layer.DirectArray.GetAt(uvp) * uv_scaling + uv_offset
                                uvp = uv2Layer.IndexArray.GetAt(k + 1)
                                uv2_2 = uv2Layer.DirectArray.GetAt(uvp) * uv_scaling + uv_offset
                                uvp = uv2Layer.IndexArray.GetAt(k + 2)
                                uv3_2 = uv2Layer.DirectArray.GetAt(uvp) * uv_scaling + uv_offset
                            End If
                            k += 3
                        End If
                        n1.Normalize()
                        n2.Normalize()
                        n3.Normalize()
                        fbxgrp(i).vertices(v1).x = vt1.X
                        fbxgrp(i).vertices(v1).y = vt1.Y
                        fbxgrp(i).vertices(v1).z = vt1.Z
                        fbxgrp(i).vertices(v1).u = uv1_.X
                        fbxgrp(i).vertices(v1).v = -uv1_.Y
                        fbxgrp(i).vertices(v1).u2 = uv1_2.X
                        fbxgrp(i).vertices(v1).v2 = -uv1_2.Y
                        fbxgrp(i).vertices(v1).nx = n1.X
                        fbxgrp(i).vertices(v1).ny = n1.Y
                        fbxgrp(i).vertices(v1).nz = n1.Z
                        fbxgrp(i).vertices(v1).n = packnormalFBX888(n1)

                        ' these commented out lines are for debuging the packnormalFBX888 method
                        Dim nup = unpackNormal_8_8_8(fbxgrp(i).vertices(v1).n)
                        fbxgrp(i).vertices(v1).nx = nup.nx
                        fbxgrp(i).vertices(v1).ny = nup.ny
                        fbxgrp(i).vertices(v1).nz = nup.nz

                        cnt += 1
                        fbxgrp(i).vertices(v2).x = vt2.X
                        fbxgrp(i).vertices(v2).y = vt2.Y
                        fbxgrp(i).vertices(v2).z = vt2.Z
                        fbxgrp(i).vertices(v2).u = uv2_.X
                        fbxgrp(i).vertices(v2).v = -uv2_.Y
                        fbxgrp(i).vertices(v2).u2 = uv2_2.X
                        fbxgrp(i).vertices(v2).v2 = -uv2_2.Y
                        fbxgrp(i).vertices(v2).nx = n2.X
                        fbxgrp(i).vertices(v2).ny = n2.Y
                        fbxgrp(i).vertices(v2).nz = n2.Z
                        fbxgrp(i).vertices(v2).n = packnormalFBX888(n2)

                        nup = unpackNormal_8_8_8(fbxgrp(i).vertices(v2).n)
                        fbxgrp(i).vertices(v2).nx = nup.nx
                        fbxgrp(i).vertices(v2).ny = nup.ny
                        fbxgrp(i).vertices(v2).nz = nup.nz

                        cnt += 1
                        fbxgrp(i).vertices(v3).x = vt3.X
                        fbxgrp(i).vertices(v3).y = vt3.Y
                        fbxgrp(i).vertices(v3).z = vt3.Z
                        fbxgrp(i).vertices(v3).u = uv3_.X
                        fbxgrp(i).vertices(v3).v = -uv3_.Y
                        fbxgrp(i).vertices(v3).u2 = uv3_2.X
                        fbxgrp(i).vertices(v3).v2 = -uv3_2.Y
                        fbxgrp(i).vertices(v3).nx = n3.X
                        fbxgrp(i).vertices(v3).ny = n3.Y
                        fbxgrp(i).vertices(v3).nz = n3.Z
                        fbxgrp(i).vertices(v3).n = packnormalFBX888(n3)

                        nup = unpackNormal_8_8_8(fbxgrp(i).vertices(v3).n)
                        fbxgrp(i).vertices(v3).nx = nup.nx
                        fbxgrp(i).vertices(v3).ny = nup.ny
                        fbxgrp(i).vertices(v3).nz = nup.nz

                        cnt += 1

                    Next
                    create_TBNS(i)
                End If
            Catch ex As Exception

            End Try

        Next
        'clean up 
        importer.Destroy()
        rootnode.Destroy()
        pManager.Destroy()
        Try
            process_fbx_data()

        Catch ex As Exception

        End Try

outofhere:
        frmMain.info_Label.Text = "Creating Display Lists"
        Application.DoEvents()
        For i = 1 To nodecnt
            Dim id = Gl.glGenLists(1)
            Gl.glNewList(id, Gl.GL_COMPILE)
            fbxgrp(i).call_list = id
            make_fbx_display_lists(fbxgrp(i).nPrimitives_, i)
            Gl.glEndList()
        Next
        FBX_LOADED = True
        frmMain.info_Label.Visible = False
        frmMain.m_show_fbx.Checked = True
        If MODEL_LOADED Then
            frmMain.m_show_fbx.Visible = True
        End If
    End Sub

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
        Dim dfr = New FbxVector4(0.0, 0.0, 0.0, 0.0)
        Dim dfs = New FbxVector4(1.0, 1.0, 1.0, 0.0)
        Dim dft As New FbxVector4(0.0, 0.0, 0.0, 1.0)
        rootNode.SetDefaultR(dfr)
        rootNode.SetDefaultS(dfs)
        rootNode.SetDefaultT(dft)
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

            model_name = _group(id).name.Replace("/", "\")
            model_name = model_name.Replace(":", "~")
            node_list(id) = FbxNode.Create(pManager, model_name)

            frmFBX.Label2.Text = model_name


            'create mesh node
            Dim mymesh = fbx_create_mesh(model_name, id, pManager)


            'Dim m As New FbxXMatrix
            Dim m_ = _object(id).matrix
            'setFbxMatrix(m_, m)
            Dim scale As New SlimDX.Vector3
            Dim rot As New SlimDX.Quaternion
            Dim trans As New SlimDX.Vector3
            Dim Mt As New SlimDX.Matrix
            Mt = load_matrix_decompose(m_, trans, scale, rot)
            'Dim quat As New FbxQuaternion(rot.X, rot.Y, rot.Z)
            'Dim vd = quat.DecomposeSphericalXYZ


            Dim r_vector As New FbxVector4(rot.X, 0.0, rot.Z, rot.W)
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
            'fm.SetTQS(t_vector, r_vector, s_vector)
            'Dim t_post, r_post, s_p
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
            Dim dr, ds, dt As New FbxVector4
            dr.Set(0, 0, 0, 0)
            ds.Set(1, 1, 1, 1)
            dt.Set(0, 0, 0, 1)
            'node_list(id).SetDefaultR(dr)
            'node_list(id).SetDefaultT(ds)
            'node_list(id).SetDefaultS(dt)
            node_list(id).SetDefaultR(r_vector)
            node_list(id).SetDefaultT(t_vector)
            node_list(id).SetDefaultS(s_vector)

            'node_list(id).SetGeometricRotation(FbxNode.PivotSet.SourceSet, r_vector)
            'node_list(id).SetGeometricTranslation(FbxNode.PivotSet.SourceSet, t_vector)
            'node_list(id).SetGeometricScaling(FbxNode.PivotSet.SourceSet, s_vector)

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
#Region "Import helpers"
    Private Sub process_fbx_data()
        get_component_index() 'build indexing table
    End Sub

    Private Sub get_component_index()
        Dim ct, ht, tt, gt As Integer
        Dim d_len As Integer
        Dim c_cnt, h_cnt, t_cnt, g_cnt As Integer
        Dim odd_model As Boolean
        '---------------------------------------------------------------------------------------------------
        'find out if we have a wrongly named model in the FBX
        odd_model = False
        For i = 1 To fbxgrp.Length - 1
            If Not odd_model Then
                If fbxgrp(i).name.ToLower.Contains("chassis") Or _
                    fbxgrp(i).name.ToLower.Contains("hull") Or _
                    fbxgrp(i).name.ToLower.Contains("turret") Or _
                    fbxgrp(i).name.ToLower.Contains("gun") Then
                Else
                    odd_model = True
                End If
            End If
        Next

        '---------------------------------------------------------------------------------------------------
        'sort out how many are of what type in the fbx
        'we need to do this if parts have been added

        'now we create our index table
        Dim c As Integer = 1
        ReDim m_groups(4) ' there are 4 types... chassis, hull, turret and gun
        m_groups(1) = New mgrp_
        m_groups(2) = New mgrp_
        m_groups(3) = New mgrp_
        m_groups(4) = New mgrp_
        Dim ar() As String
        For i = 1 To fbxgrp.Length - 1
            If fbxgrp(i).name.ToLower.Contains("chassis") Then
                ReDim Preserve m_groups(1).list(ct)
                ReDim Preserve m_groups(1).f_name(ct)
                ReDim Preserve m_groups(1).package_id(ct)
                m_groups(1).cnt = ct + 1
                m_groups(1).list(ct) = i
                m_groups(1).m_type = 1
                ar = fbxgrp(i).name.Split("~")
                m_groups(1).f_name(ct) = ar(0)
                If ar.Length > 1 Then
                    m_groups(1).package_id(ct) = CInt(ar(1))
                Else
                    m_groups(1).package_id(ct) = -1
                End If
                ct += 1
            End If
            If fbxgrp(i).name.ToLower.Contains("hull") Then
                ReDim Preserve m_groups(2).list(ht)
                ReDim Preserve m_groups(2).f_name(ht)
                ReDim Preserve m_groups(2).package_id(ht)
                m_groups(2).cnt = ht + 1
                m_groups(2).list(ht) = i
                m_groups(2).m_type = 2
                ar = fbxgrp(i).name.Split("~")
                m_groups(2).f_name(ht) = ar(0)
                If ar.Length > 1 Then
                    m_groups(2).package_id(ht) = CInt(ar(1))
                Else
                    m_groups(2).package_id(ht) = -1
                End If
                ht += 1
            End If
            If fbxgrp(i).name.ToLower.Contains("turret") Then
                ReDim Preserve m_groups(3).list(tt)
                ReDim Preserve m_groups(3).f_name(tt)
                ReDim Preserve m_groups(3).package_id(tt)
                m_groups(3).cnt = tt + 1
                m_groups(3).list(tt) = i
                m_groups(3).m_type = 3
                ar = fbxgrp(i).name.Split("~")
                m_groups(3).f_name(tt) = ar(0)
                If ar.Length > 1 Then
                    m_groups(3).package_id(tt) = CInt(ar(1))
                Else
                    m_groups(3).package_id(tt) = -1
                End If
                tt += 1
            End If
            If fbxgrp(i).name.ToLower.Contains("gun") Then
                ReDim Preserve m_groups(4).list(gt)
                ReDim Preserve m_groups(4).f_name(gt)
                ReDim Preserve m_groups(4).package_id(gt)
                m_groups(4).cnt = gt + 1
                m_groups(4).list(gt) = i
                m_groups(4).m_type = 4
                ar = fbxgrp(i).name.Split("~")
                m_groups(4).f_name(gt) = ar(0)
                If ar.Length > 1 Then
                    m_groups(4).package_id(gt) = CInt(ar(1))
                Else
                    m_groups(4).package_id(gt) = -1
                End If
                gt += 1
            End If
        Next

        '---------------------------------------------------------------------------------------------------
        'now we will load the model from the package files
        For i = 1 To 4
            file_name = m_groups(i).f_name(0).Replace(".primitives_processed", ".model") 'assuming (0) has the correct name.
            file_name = file_name.Replace(".primitives", ".model")
            current_tank_package = m_groups(i).package_id(0)
            Dim success = build_primitive_data(True)
        Next
        '---------------------------------------------------------------------------------------------------
        'sort out how many are of what type in the existing model
        For i = 1 To object_count
            If _group(i).name.ToLower.Contains("chassis") Then
                c_cnt += 1
            End If
            If _group(i).name.ToLower.Contains("hull") Then
                h_cnt += 1
            End If
            If _group(i).name.ToLower.Contains("turret") Then
                t_cnt += 1
            End If
            If _group(i).name.ToLower.Contains("gun") Then
                g_cnt += 1
            End If
        Next
        '---------------------------------------------------------------------------------------------------
        Dim t_fbx, t_mdl As Integer
        t_fbx = ct + ht + tt + gt
        t_mdl = c_cnt + h_cnt + t_cnt + g_cnt
        'if t_fbx = t_mdl than we have the same componet counts.
        'Check of one of them has been modified.
        Dim flg, CB, HB, TB, GB As Boolean
        Dim c_new, h_new, t_new, g_new As Boolean
        CB = False : HB = False : TB = False : GB = False ' these default to false but set them anyway
        c_new = False : h_new = False : t_new = False : g_new = False
        If t_fbx <> t_mdl Then
            If c_cnt <> ct Then 'something added?
                CB = True
                c_new = True
            End If
            If h_cnt <> ht Then 'something added?
                HB = True
                h_new = True
            End If
            If t_cnt <> tt Then 'something added?
                TB = True
                t_new = True
            End If
            If g_cnt <> gt Then 'something added?
                GB = True
                g_new = True
            End If
        Else
            For i = 1 To object_count
                flg = False
                If _group(i).nVertices_ <> fbxgrp(i).nVertices_ Then 'something removed or added?
                    flg = True : GoTo whichOne
                End If
                For j As UInt32 = 0 To _group(i).nVertices_ - 1
                    If _group(i).vertices(j).x <> fbxgrp(i).vertices(j).x Then
                        flg = True
                        GoTo whichOne
                    End If
                    If _group(i).vertices(j).y <> fbxgrp(i).vertices(j).y Then
                        flg = True
                        GoTo whichOne
                    End If
                    If _group(i).vertices(j).z <> fbxgrp(i).vertices(j).z Then
                        flg = True
                        GoTo whichOne
                    End If

                Next
whichone:
                If flg Then ' if true than either the count is different or the vertices are changed
                    If _group(i).name.ToLower.Contains("chassis") Then
                        CB = True
                    End If
                    If _group(i).name.ToLower.Contains("hull") Then
                        HB = True
                    End If
                    If _group(i).name.ToLower.Contains("turret") Then
                        TB = True
                    End If
                    If _group(i).name.ToLower.Contains("gun") Then
                        GB = True
                    End If
                End If
            Next

        End If
        For i = 1 To fbxgrp.Length - 1
            If Not fbxgrp(i).name.Contains("vehicles\") Then
                fbxgrp(i).is_new_model = True
            End If
        Next
        'need to find out if there is a dangling model that was imported.
        'one that was not assigned via name to a group
        If odd_model Then
            MsgBox("It appears you have added a model that is not assigned to a group." + vbCrLf + _
                    "Make sure you renamed the model you created to include a group name.." + vbCrLf + _
                    "The name should include one of these : Chassis, Hull, Turret or Gun." + vbCrLf + _
                    "I CAN NOT add a new group to a tank model. I can Only add new items to a group." + vbCrLf + _
                    "You will not beable to save this model!", MsgBoxStyle.Exclamation, "Import Issue")
            frmMain.m_write_primitive.Enabled = False
        Else
            frmMain.m_write_primitive.Enabled = True
        End If
        'We give the user the opertunity to extract the model. We need some where to write any changed data too.
        ar = file_name.Replace("/", "\").Split("\")
        Dim fn = ar(0) + "\" + ar(1) + "\" + ar(2)
        Dim dp = My.Settings.res_mods_path + "\" + fn
        frmWritePrimitive.SAVE_NAME = dp
        If Not Directory.Exists(dp) Then
            If MsgBox("It appears You have not extracted data for this model." + vbCrLf + _
                      "There is no place to save this new Model." + vbCrLf + _
                       "Would you like to extract the data from the .PKG files?", MsgBoxStyle.YesNo, "Extract?") = MsgBoxResult.Yes Then
                file_name = "1:dummy:" + Path.GetFileNameWithoutExtension(dp.Replace("/", "\"))
                frmMain.m_create_and_extract.PerformClick()
            End If

        End If
        'set which group has new models or changed data
        frmWritePrimitive.Visible = True
        frmWritePrimitive.cew_cb.Checked = False '= CB
        frmWritePrimitive.cew_cb.Enabled = False
        m_groups(1).changed = False ' = CB
        m_groups(1).new_objects = c_new

        frmWritePrimitive.hew_cb.Checked = HB
        m_groups(2).changed = HB
        m_groups(2).new_objects = h_new

        frmWritePrimitive.tew_cb.Checked = TB
        m_groups(3).changed = TB
        m_groups(3).new_objects = t_new

        frmWritePrimitive.gew_cb.Checked = False ' = GB
        frmWritePrimitive.gew_cb.Enabled = False
        m_groups(4).changed = False '= GB
        m_groups(4).new_objects = g_new

        frmWritePrimitive.Visible = False
        MODEL_LOADED = True
    End Sub

    Public Sub make_fbx_display_lists(ByVal cnt As Integer, ByVal jj As Integer)
        Gl.glBegin(Gl.GL_TRIANGLES)
        'trans_vertex(jj)
        For z As UInt32 = 0 To (cnt) - 1
            make_triangle(jj, fbxgrp(jj).indicies(z).v1)
            make_triangle(jj, fbxgrp(jj).indicies(z).v2)
            make_triangle(jj, fbxgrp(jj).indicies(z).v3)
        Next
        Gl.glEnd()
    End Sub

    Private Sub make_triangle(ByVal jj As Integer, ByVal i As Integer)
        Gl.glNormal3f(fbxgrp(jj).vertices(i).nx, fbxgrp(jj).vertices(i).ny, fbxgrp(jj).vertices(i).nz)
        Gl.glMultiTexCoord3f(1, fbxgrp(jj).vertices(i).tx, fbxgrp(jj).vertices(i).ty, fbxgrp(jj).vertices(i).tz)
        Gl.glMultiTexCoord3f(2, fbxgrp(jj).vertices(i).bnx, fbxgrp(jj).vertices(i).bny, fbxgrp(jj).vertices(i).bnz)
        Gl.glTexCoord2f(-fbxgrp(jj).vertices(i).u, fbxgrp(jj).vertices(i).v)
        Gl.glVertex3f(fbxgrp(jj).vertices(i).x, fbxgrp(jj).vertices(i).y, fbxgrp(jj).vertices(i).z)

    End Sub

    Private Sub setFbxMatrix(ByRef m_() As Double, ByRef fb As FbxXMatrix)
        Dim m As New SlimDX.Matrix
        For i = 1 To 3
            For j = 1 To 3
                m.Item(j, i) = m_((i * 4) + j)
            Next
        Next
        Dim r As SlimDX.Quaternion
        Dim t, s As SlimDX.Vector3
        m.Decompose(s, r, t)
        Dim vs As New FbxVector4(s.X, s.Y, s.Z, 1.0)
        Dim vt As New FbxVector4(t.X, t.X, t.Z, 1.0)
        Dim vr As New FbxQuaternion(r.X, r.Y, r.Z, 0.0)
        fb.SetTQS(vt, vr, vs)

    End Sub


#Region "TBN Creation functions"

    Public Sub create_TBNS(ByVal id As UInt32)
        Dim cnt = fbxgrp(id).nPrimitives_
        Dim p1, p2, p3 As UInt32
        For i As UInt32 = 0 To cnt - 1
            p1 = fbxgrp(id).indicies(i).v1
            p2 = fbxgrp(id).indicies(i).v2
            p3 = fbxgrp(id).indicies(i).v3
            Dim tan, bn As vect3
            Dim v1, v2, v3 As vect3
            Dim u1, u2, u3 As vect3
            v1.x = fbxgrp(id).vertices(p1).x
            v1.y = fbxgrp(id).vertices(p1).y
            v1.z = fbxgrp(id).vertices(p1).z
            v2.x = fbxgrp(id).vertices(p2).x
            v2.y = fbxgrp(id).vertices(p2).y
            v2.z = fbxgrp(id).vertices(p2).z
            v3.x = fbxgrp(id).vertices(p3).x
            v3.y = fbxgrp(id).vertices(p3).y
            v3.z = fbxgrp(id).vertices(p3).z
            '
            u1.x = fbxgrp(id).vertices(p1).u
            u1.y = fbxgrp(id).vertices(p1).v
            u2.x = fbxgrp(id).vertices(p2).u
            u2.y = fbxgrp(id).vertices(p2).v
            u3.x = fbxgrp(id).vertices(p3).u
            u3.y = fbxgrp(id).vertices(p3).v
            ComputeTangentBasis(v1, v2, v3, u1, u2, u3, tan, bn) ' calculate tan and biTan

            save_tbn(id, tan, bn, p1) ' puts xyz values in vertex
            save_tbn(id, tan, bn, p2)
            save_tbn(id, tan, bn, p3)

            fbxgrp(id).vertices(p1).t = packnormalFBX888(toFBXv(tan)) 'packs and puts the uint value in to the vertex
            fbxgrp(id).vertices(p1).bn = packnormalFBX888(toFBXv(bn))
            fbxgrp(id).vertices(p2).t = packnormalFBX888(toFBXv(tan))
            fbxgrp(id).vertices(p2).bn = packnormalFBX888(toFBXv(bn))
            fbxgrp(id).vertices(p3).t = packnormalFBX888(toFBXv(tan))
            fbxgrp(id).vertices(p3).bn = packnormalFBX888(toFBXv(bn))
        Next
        Return
    End Sub

    Private Sub save_tbn(id As Integer, tan As vect3, bn As vect3, i As Integer)
        fbxgrp(id).vertices(i).tx = tan.x
        fbxgrp(id).vertices(i).ty = tan.y
        fbxgrp(id).vertices(i).tz = tan.z
        fbxgrp(id).vertices(i).bnx = bn.x
        fbxgrp(id).vertices(i).bny = bn.y
        fbxgrp(id).vertices(i).bnz = bn.z

    End Sub
    Public Function toFBXv(ByVal inv As vect3) As FbxVector4
        Dim v As New FbxVector4
        v.X = inv.x
        v.Y = inv.y
        v.Z = inv.z
        Return v
    End Function

    Private Sub ComputeTangentBasis( _
      ByVal p1 As vect3, ByVal p2 As vect3, ByVal p3 As vect3, _
      ByVal UV1 As vect3, ByVal UV2 As vect3, ByVal UV3 As vect3, _
      ByRef tangent As vect3, ByRef bitangent As vect3)

        Dim Edge1 As vect3 = subvect3(p2, p1)
        Dim Edge2 As vect3 = subvect3(p3, p1)
        Dim Edge1uv As vect3 = subvect2(UV2, UV1)
        Dim Edge2uv As vect3 = subvect2(UV3, UV1)

        Dim cp As Single = Edge1uv.y * Edge2uv.x - Edge1uv.x * Edge2uv.y

        If cp <> 0.0F Then
            Dim mul As Single = 1.0F / cp
            tangent = mulvect3(addvect3(mulvect3(Edge1, -Edge2uv.y), mulvect3(Edge2, Edge1uv.y)), mul)
            bitangent = mulvect3(addvect3(mulvect3(Edge1, -Edge2uv.x), mulvect3(Edge2, Edge1uv.x)), mul)

            tangent = normalize(tangent)
            bitangent = normalize(bitangent)
        End If

    End Sub

    Private Function normalize(ByVal normal As vect3) As vect3
        Dim len As Single = Sqrt((normal.x * normal.x) + (normal.y * normal.y) + (normal.z * normal.z))

        ' avoid division by 0
        If len = 0.0F Then len = 1.0F
        Dim v As vect3
        ' reduce to unit size
        v.x = (normal.x / len)
        v.y = (normal.y / len)
        v.z = (normal.z / len)

        Return v
    End Function
    Private Function mulvect3(ByVal v1 As vect3, ByVal v As Single) As vect3
        v1.x *= v
        v1.y *= v
        v1.z *= v
        Return v1
    End Function
    Private Function addvect3(ByVal v1 As vect3, ByVal v2 As vect3) As vect3
        v1.x += v2.x
        v1.y += v2.y
        v1.z += v2.z
        Return v1
    End Function
    Private Function subvect3(ByVal v1 As vect3, ByVal v2 As vect3) As vect3
        v1.x -= v2.x
        v1.y -= v2.y
        v1.z -= v2.z
        Return v1
    End Function
    Private Function subvect2(ByVal v1 As vect3, ByVal v2 As vect3) As vect3
        v1.x -= v2.x
        v1.y -= v2.y
        Return v1
    End Function
#End Region

#End Region

#Region "Export Helpers"

    Private Sub build_fbx_matrix(ByVal idx As Integer, ByVal fm As FbxXMatrix)
        ReDim fbxgrp(idx).matrix(15)
        For i = 0 To 15
            fbxgrp(idx).matrix(i) = CSng(fm.Item((i >> 2 And &H3), (i And &H3)))
        Next

    End Sub

    Private Function s_to_int(ByRef n As Single) As Int32
        Dim i As Int32
        i = lookup(((n + 1.0) * 0.5) * 254)
        Return i
    End Function

    Public Function packnormalFBX_old(ByVal n As FbxVector4) As UInt32
        'ctz is my special C++ function to pack the vector into a Uint32
        'ctz.init_x(n.X * -1.0)
        ctz.init_x(n.X)
        ctz.init_y(n.Y)
        ctz.init_z(n.Z)
        Return ctz.pack(1)
    End Function

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
        lMaterial = FbxSurfacePhong.Create(pManager, m_name + "" + id.ToString("000"))
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
        'Dim texture = FbxTexture.Create(pManager, "DiffuseMap" + ":" + id.ToString("000"))
        Dim texture = FbxTexture.Create(pManager, FBX_Texture_path + "\" + Path.GetFileNameWithoutExtension(textures(id).c_name) + ".png")
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
        'Dim texture = FbxTexture.Create(pManager, "NormalMap" + ":" + id.ToString("000"))
        Dim texture = FbxTexture.Create(pManager, FBX_Texture_path + "\" + Path.GetFileNameWithoutExtension(textures(id).n_name) + ".png")
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

    Private Function load_matrix_decompose(data() As Double, ByRef trans As SlimDX.Vector3, ByRef scale As SlimDX.Vector3, ByRef rot As SlimDX.Quaternion) As SlimDX.Matrix
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
        round_error(rot.X)
        round_error(rot.Y)
        round_error(rot.Z)
        round_error(rot.W)
        Return m_
    End Function

    Private Sub round_error(ByRef val As Single)
        val = Round(val, 6, MidpointRounding.AwayFromZero)
    End Sub

    Public Function fbx_create_mesh(model_name As String, id As Integer, pManager As FbxSdkManager) As FbxMesh
        Dim myMesh As FbxMesh
        myMesh = FbxMesh.Create(pManager, model_name)
        Dim cnt = _group(id).nPrimitives_
        Dim off As UInt32
        Dim v As vect3Norm
        Dim v4 As New FbxVector4
        Dim I As Integer
        off = _group(id).startVertex_

        '--------------------------------------------------------------------------
        '--------------------------------------------------------------------------
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

        '--------------------------------------------------------------------------
        '--------------------------------------------------------------------------
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
        '--------------------------------------------------------------------------
        '3DS Max crashes exporting FBX files that have a CV channel.
        'This is a horrible and long running bug.
        'There is no way to solve this with my app so VC will be grabbed from the original at import time.
        '--------------------------------------------------------------------------
        'Dim colorLayer1 As FbxLayerElementVertexColor = Nothing
        'If _group(id).has_color = 1 Then
        '    colorLayer1 = FbxLayerElementVertexColor.Create(myMesh, "VertexColor")
        '    colorLayer1.Name = "VertexColor"
        '    colorLayer1.Mapping_Mode = FbxLayerElement.MappingMode.ByControlPoint
        '    colorLayer1.Reference_Mode = FbxLayerElement.ReferenceMode.Direct
        '    Dim color As New FbxColor
        '    For I = 0 To myMesh.ControlPointsCount - 1
        '        color.Red = CDbl(_group(id).vertices(I).r / 255)
        '        color.Green = CDbl(_group(id).vertices(I).r / 255)
        '        color.Blue = CDbl(_group(id).vertices(I).r / 255)
        '        color.Alpha = CDbl(_group(id).vertices(I).r / 255)
        '        colorLayer1.DirectArray.Add(color)
        '    Next

        'End If
        'layer.VertexColors = colorLayer1
        '--------------------------------------------------------------------------
        '--------------------------------------------------------------------------
        Dim v_2 As New FbxVector2
        Dim UV2Layer As FbxLayerElementUV = Nothing
        If _group(id).has_uv2 = 1 Then

            UV2Layer = FbxLayerElementUV.Create(myMesh, "UV2")
            UV2Layer.Mapping_Mode = FbxLayerElement.MappingMode.ByControlPoint
            UV2Layer.Reference_Mode = FbxLayerElement.ReferenceMode.Direct
            layer.SetUVs(UV2Layer, FbxLayerElement.LayerElementType.AmbientTextures)
            For I = 0 To myMesh.ControlPointsCount - 1
                If frmFBX.flip_u.Checked Then
                    v_2.X = _group(id).vertices(I).u2 * -1
                Else
                    v_2.X = _group(id).vertices(I).u2
                End If

                If frmFBX.flip_v.Checked Then
                    v_2.Y = _group(id).vertices(I).v2 * -1
                Else
                    v_2.Y = _group(id).vertices(I).v2
                End If
                UV2Layer.DirectArray.Add(v_2)

            Next
            UV2Layer.IndexArray.Count = _group(id).nPrimitives_
        End If
        '--------------------------------------------------------------------------
        '--------------------------------------------------------------------------
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


        '--------------------------------------------------------------------------
        '--------------------------------------------------------------------------

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
            UVDiffuseLayer.IndexArray.SetAt(pos, j)
            If _group(id).has_uv2 = 1 Then
                UV2Layer.IndexArray.SetAt(pos, j)
            End If
            j += 1
            pos = _group(id).indicies(n).v2 - off
            myMesh.AddPolygon(pos)
            UVDiffuseLayer.IndexArray.SetAt(pos, j)
            If _group(id).has_uv2 = 1 Then
                UV2Layer.IndexArray.SetAt(pos, j)
            End If
            j += 1
            pos = _group(id).indicies(n).v3 - off
            myMesh.AddPolygon(pos)
            UVDiffuseLayer.IndexArray.SetAt(pos, j)
            If _group(id).has_uv2 = 1 Then
                UV2Layer.IndexArray.SetAt(pos, j)
            End If
            n += 1
            myMesh.EndPolygon()

        Next
        Return myMesh
    End Function

    Public Function packnormalFBX888(ByVal n As FbxVector4) As UInt32
        'This took an entire night to get working correctly
        Try
            'n.X = -0.715007 ' debug testing shit
            'n.X = -0.5
            'n.Y = 0.0
            'n.Z = 1.0
            n.Normalize()
            n.X = Round(n.X, 4)
            n.Y = Round(n.Y, 4)
            n.Z = Round(n.Z, 4)
            Dim nx, ny, nz As Int32

            nx = s_to_int(-n.X)
            ny = s_to_int(-n.Y)
            nz = s_to_int(-n.Z)

            'nx = Convert.ToSByte(Round(n.X * 127))
            'ny = Convert.ToSByte(Round(n.Y * 127))
            'nz = Convert.ToSByte(Round(n.Z * 127))

            Dim nu = CLng(nz << 16)
            Dim nm = CLng(ny << 8)
            Dim nb = CInt(nx)
            Dim ru = Convert.ToUInt32((nu And &HFF0000) + (nm And &HFF00) + (nb And &HFF))
            Return ru
        Catch ex As Exception

        End Try
        Return New Int32
    End Function
    Public Function packnormalFBX888_writePrimitive(ByVal n As FbxVector4) As UInt32
        'This took an entire night to get working correctly
        Try
            'n.X = -0.715007 ' debug testing shit
            'n.X = -0.5
            'n.Y = 0.0
            'n.Z = 1.0
            n.Normalize()
            n.X = Round(n.X, 4)
            n.Y = Round(n.Y, 4)
            n.Z = Round(n.Z, 4)
            Dim nx, ny, nz As Int32

            nx = s_to_int(-n.X)
            ny = s_to_int(-n.Y)
            nz = s_to_int(-n.Z)

            'nx = Convert.ToSByte(Round(n.X * 127))
            'ny = Convert.ToSByte(Round(n.Y * 127))
            'nz = Convert.ToSByte(Round(n.Z * 127))

            Dim nu = CLng(nz << 16)
            Dim nm = CLng(ny << 8)
            Dim nb = CInt(nx)
            Dim ru = Convert.ToUInt32((nu And &HFF0000) + (nm And &HFF00) + (nb And &HFF))
            Return ru
        Catch ex As Exception

        End Try
        Return New Int32
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
        p.nx = -(p.nx / len)
        p.ny = -(p.ny / len)
        p.nz = -(p.nz / len)
        'Console.WriteLine(p.x.ToString("0.000000") + " " + p.y.ToString("0.000000") + " " + p.z.ToString("0.000000"))
        Return p
    End Function
    Public lookup(255) As Byte

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

#End Region

End Module
