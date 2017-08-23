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
        scene = FbxScene.Create(pManager, "My Scene")
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
            mat_main = Path.GetDirectoryName(My.Settings.fbx_path) + "\" + Path.GetFileNameWithoutExtension(_group(id).color_name) + ".png"
            mat_NM = Path.GetDirectoryName(My.Settings.fbx_path) + "\" + Path.GetFileNameWithoutExtension(_group(id).normal_name) + ".png"
            mat_uv2 = _group(id).color2_name

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
            End If
            If id = object_count Then
                s_vector.Z *= -1.0
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
        texture.SetFileName(Path.GetDirectoryName(My.Settings.fbx_path) + "\" + Path.GetFileNameWithoutExtension(textures(id).c_name) + ".png") 'Get the Texture path from the list
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
        texture.SetFileName(Path.GetDirectoryName(My.Settings.fbx_path) + "\" + Path.GetFileNameWithoutExtension(textures(id).n_name) + ".png") 'Get the Texture path from the list
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
