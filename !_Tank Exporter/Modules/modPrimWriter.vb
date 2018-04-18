
Imports System.IO
Imports System.Windows
Imports System.Runtime.InteropServices

Module modPrimWriter
    Public OBJECT_WAS_INSERTED As Boolean
    Dim br As BinaryWriter
    Dim pnter As Integer
    Dim b As Byte = &H0
    Dim indi_cnt As Integer
    Dim obj_cnt As Integer
    Dim total_indices As Integer
    Dim bsp2_mat_size As Integer
    Dim bsp2_size As Integer
    Dim idx_size As Integer
    Dim uv2_cnt As Integer
    Dim total_verts As Integer

    Dim l As Integer
    Dim padding As Integer
    Public Sub write_primitives(ByVal ID As Integer)
        Dim tsa() As Char
        Dim dummy As UInt32 = 0
        Dim i, j As UInt32
        For i = 1 To object_count
            'create_TBNS(i) ' not sure we need to do this again
        Next
        Dim r As FileStream = Nothing
        obj_cnt = m_groups(ID).cnt
        try

            r = New FileStream(My.Settings.res_mods_path + "\" + m_groups(ID).f_name(0), FileMode.Create, FileAccess.Write)
        Catch e As Exception
            MsgBox("I could not open """ + My.Settings.res_mods_path + "\" + m_groups(ID).f_name(0) + """!" + vbCrLf + _
                    "The Root folder is there but there are no  .primitive_processed files." + vbCrLf _
                    + " Did you delete them?", MsgBoxStyle.Exclamation, "Can find folder!")
            Return
        End Try

        br = New BinaryWriter(r)
        Dim mh As UInt32 = &H42A14E65
        br.Write(mh) ' write magic number
        Dim indi_size, vert_size As UInt32
        '-------------------------------------------------------------
        Dim p = r.Position
        write_list_data(ID) ' write indices list and indexing table
        indi_size = r.Position - p ' get section size
        indi_size -= padding ' l is the padding amount written
        '-------------------------------------------------------------

        p = r.Position
        write_vertex_data(ID) 'write out vertices and UV2s if they exist
        vert_size = r.Position - p ' get section size
        vert_size -= padding ' l is the padding amount written
        '-------------------------------------------------------------

        'write colored vertice data if the model has them.
        Dim has_color As Boolean = False ' flag for wrting table at the end of the .primitive
        Dim color_data_size As Integer
        pnter = m_groups(ID).list(0)
        If _group(pnter).has_color = 1 Then
            has_color = True ' yes.. make it true
            Dim c_buff(_group(pnter).color_data.Length - 1) As Byte
            _group(pnter).color_data.CopyTo(c_buff, 0)
            Dim resize = total_verts * 4 ' there are 4 bytes for every single vertex
            ReDim Preserve c_buff(resize)
            p = r.Position
            br.Write(c_buff, 0, c_buff.Length)
            color_data_size = r.Position - p
            l = (br.BaseStream.Position) Mod 4L
            Console.Write("color_pnt" + vbTab + "base {0} , mod-4 {1} " + vbCrLf, br.BaseStream.Position, (br.BaseStream.Position) Mod 4L)
            If l > 0 Then
                For i = 1 To 4 - l
                    br.Write(b)
                    color_data_size += 1
                Next
            End If

        End If
        '-------------------------------------------------------------

        'write_BSP2(ID)
        '-------------------------------------------------------------

        Dim header_length As UInt32 = 68 + 64

        Dim offset As UInt32 = 0
        '##### Write table containing the sizes and names of the sections in the file
        '-------------------------------------------------------------
        'write indices table entry
        tsa = "indices".ToArray

        br.Write(indi_size) ' size of data
        br.Write(dummy) : br.Write(dummy) : br.Write(dummy) : br.Write(dummy) ' fill data
        br.Write(Convert.ToUInt32(tsa.Length)) ' string length
        br.Write(tsa) ' string
        offset += tsa.Length + 24 ' adjust table start by this entry length
        l = (br.BaseStream.Position) Mod 4L
        Console.Write("ind_pnt" + vbTab + "base {0} , mod-4 {1} " + vbCrLf, br.BaseStream.Position, (br.BaseStream.Position) Mod 4L)
        If l > 0 Then ' pad to int aligmenment
            For i = 1 To 4 - l
                br.Write(b)
            Next
            offset += i - 1
        End If
        '-------------------------------------------------------------
        'write vertice table entry
        Dim tvn As String = ""
        tsa = "vertices".ToArray ' convert to char array
        br.Write(vert_size)
        br.Write(dummy) : br.Write(dummy) : br.Write(dummy) : br.Write(dummy) ' padding
        br.Write(Convert.ToUInt32(tsa.Length))
        br.Write(tsa)
        offset += tsa.Length + 24
        l = (br.BaseStream.Position) Mod 4L
        Console.Write("vts_pnt" + vbTab + "base {0} , mod-4 {1} " + vbCrLf, br.BaseStream.Position, (br.BaseStream.Position) Mod 4L)
        If l > 0 Then
            For i = 1 To 4 - l
                br.Write(b)
            Next
            offset += i - 1
        End If
        '-------------------------------------------------------------
        GoTo no_UV2EVER
        If save_has_uv2 Then
            'write uv2 table entry
            tsa = "uv2".ToArray
            br.Write(uv2_cnt * 8UI)
            br.Write(dummy) : br.Write(dummy) : br.Write(dummy) : br.Write(dummy)
            br.Write(Convert.ToUInt32(tsa.Length))
            br.Write(tsa)
            offset += tsa.Length + 24
            l = (br.BaseStream.Position) Mod 4L
            Console.Write("uv2_pnt" + vbTab + "base {0} , mod-4 {1} " + vbCrLf, br.BaseStream.Position, (br.BaseStream.Position) Mod 4L)
            If l > 0 Then
                For i = 1 To 4 - l
                    br.Write(b)
                Next
                offset += i - 1
            End If
        End If
no_UV2EVER:
        '-------------------------------------------------------------
        If has_color Then
            'write uv2 table entry
            tsa = "colour".ToArray
            br.Write(color_data_size)
            br.Write(dummy) : br.Write(dummy) : br.Write(dummy) : br.Write(dummy)
            br.Write(Convert.ToUInt32(tsa.Length))
            br.Write(tsa)
            offset += tsa.Length + 24
            l = (br.BaseStream.Position) Mod 4L
            Console.Write("color_pnt" + vbTab + "base {0} , mod-4 {1} " + vbCrLf, br.BaseStream.Position, (br.BaseStream.Position) Mod 4L)
            If l > 0 Then
                For i = 1 To 4 - l
                    br.Write(b)
                Next
                offset += i - 1
            End If
        End If

        '-------------------------------------------------------------
        'write BSP2 if it exists
        GoTo noBSP_anymore
        For i = 1 To obj_cnt
            pnter = m_groups(ID).list(i - 1)

            If _group(pnter).bsp2_id > 0 Then

                tsa = "bsp2".ToArray
                br.Write(bsp2_size)
                br.Write(dummy) : br.Write(dummy) : br.Write(dummy) : br.Write(dummy)
                br.Write(Convert.ToUInt32(tsa.Length))
                br.Write(tsa)
                offset += tsa.Length + 24
                l = (br.BaseStream.Position) Mod 4L
                Console.Write("bsp2_pnt" + vbTab + "base {0} , mod-4 {1} " + vbCrLf, br.BaseStream.Position, (br.BaseStream.Position) Mod 4L)
                If l > 0 Then
                    For j = 1 To 4 - l
                        br.Write(b)
                    Next
                    offset += j - 1
                End If
                '--- bsp2_materials
                tsa = "bsp2_materials".ToArray
                br.Write(bsp2_mat_size)
                br.Write(dummy) : br.Write(dummy) : br.Write(dummy) : br.Write(dummy)
                br.Write(Convert.ToUInt32(tsa.Length))
                br.Write(tsa)
                offset += tsa.Length + 24
                l = (br.BaseStream.Position) Mod 4L
                Console.Write("bsp2_m_pnt" + vbTab + "base {0} , mod-4 {1} " + vbCrLf + "---end ---" + vbCrLf, br.BaseStream.Position, (br.BaseStream.Position) Mod 4L)
                If l > 0 Then
                    For j = 1 To 4 - l
                        br.Write(b)
                    Next
                    offset += j - 1
                End If
                Exit For
            End If
        Next
noBSP_anymore:
        br.Write(offset)
        'thats all folks !!
        br.Close()
        r.Close()
        r.Dispose()
        r = Nothing
        Dim f = XML_Strings(m_groups(ID).m_type)
        f = f.Replace(vbCr, "")
        Dim pos As Integer = 0
        OBJECT_WAS_INSERTED = m_groups(ID).new_objects

        If OBJECT_WAS_INSERTED Then


            '------------------------
          
            Dim inst_start As Integer = 0
            Dim pgrp As Integer = 0
            For pos = 0 To m_groups(ID).cnt
                'Dim s1 As String = "<PG_ID>" + pos.ToString + "</PG_ID>"
                'Dim s2 As String = "<PG_ID>" + Convert.ToString(pos + 1) + "</PG_ID>"
                'f = f.Replace(s1, s2)
                If f.Contains("<PG_ID>" + pos.ToString + "</PG_ID>") Then
                    inst_start = InStr(f, "<PG_ID>" + pos.ToString + "</PG_ID>")
                    pgrp += 1
                End If
            Next
            'pos = f.IndexOf("<node>", 2)
            'pos = f.IndexOf("<node>", pos + 10)
            'f = f.Insert(pos, ns)

            pos = 0
            Dim rp As String = Application.StartupPath
            rp += "\Templates\templateColorOnly.txt"
            Dim templateColorOnly As String = File.ReadAllText(Application.StartupPath + "\Templates\templateColorOnly.txt")
            Dim templateNormal As String = File.ReadAllText(Application.StartupPath + "\Templates\templateNormal.txt")
            Dim templateNormalSpec As String = File.ReadAllText(Application.StartupPath + "\Templates\templateNormalSpec.txt")
            Dim first, last As Integer
            first = m_groups(ID).existingCount
            last = m_groups(ID).cnt - first
            For item_num = first To last
                Dim primObj As String = ""
                Dim fbx_id As Integer = m_groups(ID).list(item_num) 'get id for this new item
                Dim new_name = fbxgrp(fbx_id).name ' get objects name

                'default.....
                primObj = templateColorOnly

                'normalMap.....
                If fbxgrp(fbx_id).normal_name IsNot Nothing Then
                    primObj = templateNormal ' bump shader
                End If

                'specular.....
                If fbxgrp(fbx_id).normal_name IsNot Nothing And fbxgrp(fbx_id).specular_name IsNot Nothing Then
                    primObj = templateNormalSpec  ' bump shader
                End If

                'check for legit texture assignments
                If fbxgrp(fbx_id).normal_name Is Nothing And fbxgrp(fbx_id).specular_name IsNot Nothing Then
                    MsgBox("You have a specularMap but no normalMap for " + new_name + "..." + vbCrLf + _
                            "Defaulting to colorOnly shader...", MsgBoxStyle.Exclamation, "Texture Mapping Issue...")
                End If

                primObj = primObj.Replace("<PG_ID>0</PG_ID>", "<PG_ID>" + pgrp.ToString + "</PG_ID>") ' update primitive grp id
                pgrp += 1 ' add one for each new item
                primObj = primObj.Replace("Kustom_mat", new_name) ' update indentity name

                Try ' this will change shortly
                    Dim new_s As String = fbxgrp(fbx_id).normal_name
                    primObj = primObj.Replace("NORMAL_NAME", new_s)
                Catch ex As Exception
                End Try
                Try
                    Dim new_s As String = fbxgrp(fbx_id).color_name
                    primObj = primObj.Replace("COLOR_NAME", new_s) ' update diffuse texture name
                Catch ex As Exception
                End Try
                Try
                    Dim new_s As String = fbxgrp(fbx_id).specular_name
                    primObj = primObj.Replace("SPECULAR_NAME", new_s) ' update diffuse texture name
                Catch ex As Exception
                End Try
                Try

                    'Dim new_s As String = _group(1).color2_name
                    'primObj = primObj.Replace("UV_NAME", new_s)
                Catch ex As Exception
                End Try
                pos = f.IndexOf("<groupOrigin>", inst_start)
                inst_start = pos
                f = f.Insert(pos, primObj)
            Next


        End If
        f = f.Replace("  ", "")
        'f = f.Replace(vbCrLf, "")
        'f = f.Replace(vbCr, "")
        'f = f.Replace(vbCr, "")
        f = f.Replace(vbCrLf, vbLf)
        f = f.Replace(vbTab, "")
        For i = 0 To 100
            f = f.Replace("<primitiveGroup>" + vbLf + "<PG_ID>" + i.ToString + "</PG_ID>", "<primitiveGroup>" + i.ToString)
        Next

        f = f.Replace("SceneRoot", "Scene Root")
        Dim fn As String = m_groups(ID).f_name(0)
        fn = fn.Replace(".primitives", ".visual")
        File.WriteAllText(My.Settings.res_mods_path + "\" + fn, f)

    End Sub

    Private Sub write_vertex_data(ByVal id As Integer)
        Dim j As Integer
        Dim h() = "                         ".ToArray
        If stride = 40 Then
            Dim h1() = "xyznuvtb".ToArray
            h = h1
        Else
            Dim h1() = "xyznuviiiwwtb".ToArray
            h = h1
        End If
        h = "BPVTxyznuvtb".ToArray
        Dim h2 = "set3/xyznuvtbpc".ToArray
        ReDim Preserve h(67)
        ReDim Preserve h2(63)
        br.Write(h)
        br.Write(h2)
        'Dim total_verts As UInt32
        Dim obj_cnt = m_groups(id).cnt
        Dim pnter As Integer
        total_verts = 0

        For i = 1 To obj_cnt
            pnter = m_groups(id).list(i - 1)
            total_verts += fbxgrp(pnter).nVertices_
        Next
        Dim idx_size As Integer = 2 '############## this will need to change If the total indice count is > FFFF (65535)
        Dim indi_cnt As Integer = 0
        For i = 1 To obj_cnt
            pnter = m_groups(id).list(i - 1)
            indi_cnt += (fbxgrp(pnter).nPrimitives_ * 3)
        Next
        Dim parent = m_groups(id).list(0)
        br.Write(total_verts)
        For i = 1 To obj_cnt
            pnter = m_groups(id).list(i - 1)
            j = fbxgrp(pnter).nVertices_ - 1
            For k = 0 To j
                'r.Close()
                'Return
                If fbxgrp(pnter).is_new_model Then
                    Dim v As New vect3
                    v.x = fbxgrp(pnter).vertices(k).x
                    v.y = fbxgrp(pnter).vertices(k).y
                    v.z = fbxgrp(pnter).vertices(k).z
                    v = transform(v, fbxgrp(pnter).matrix)
                    v.x -= fbxgrp(parent).matrix(12)
                    v.y -= fbxgrp(parent).matrix(13)
                    v.z -= fbxgrp(parent).matrix(14)
                    'sucks but we have to transform N, T and Bt
                    ' N --------------------------------------------------
                    Dim n As vect3
                    n.x = fbxgrp(pnter).vertices(k).nx
                    n.y = fbxgrp(pnter).vertices(k).ny
                    n.z = fbxgrp(pnter).vertices(k).nz

                    n = rotate_transform(n, fbxgrp(pnter).matrix)
                    fbxgrp(pnter).vertices(k).n = packnormalFBX888_writePrimitive(toFBXv(n))
                    ' T --------------------------------------------------
                    n.x = fbxgrp(pnter).vertices(k).tx
                    n.y = fbxgrp(pnter).vertices(k).ty
                    n.z = fbxgrp(pnter).vertices(k).tz

                    n = rotate_transform(n, fbxgrp(pnter).matrix)
                    'fbxgrp(pnter).vertices(k).t = packnormalFBX888(toFBXv(n))
                    ' Tb --------------------------------------------------
                    n.x = fbxgrp(pnter).vertices(k).bnx
                    n.y = fbxgrp(pnter).vertices(k).bny
                    n.z = fbxgrp(pnter).vertices(k).bnz

                    n = rotate_transform(n, fbxgrp(pnter).matrix)
                    'fbxgrp(pnter).vertices(k).bn = packnormalFBX888(toFBXv(n))


                    br.Write(v.x)
                    br.Write(v.y)
                    br.Write(v.z)

                Else
                    br.Write(fbxgrp(pnter).vertices(k).x)
                    br.Write(fbxgrp(pnter).vertices(k).y)
                    br.Write(fbxgrp(pnter).vertices(k).z)
                End If
                br.Write(fbxgrp(pnter).vertices(k).n)
                br.Write(fbxgrp(pnter).vertices(k).u)
                br.Write(fbxgrp(pnter).vertices(k).v)
                If stride = 37 Then
                    br.Write(fbxgrp(pnter).vertices(k).index_1)
                    br.Write(fbxgrp(pnter).vertices(k).index_2)
                    br.Write(fbxgrp(pnter).vertices(k).index_3)
                    br.Write(fbxgrp(pnter).vertices(k).weight_1)
                    br.Write(fbxgrp(pnter).vertices(k).weight_2)
                End If
                br.Write(fbxgrp(pnter).vertices(k).t)
                br.Write(fbxgrp(pnter).vertices(k).bn)
            Next
        Next
        Dim l As Long = (br.BaseStream.Position) Mod 4L
        padding = l
        Console.Write("vt raw" + vbTab + "base {0} , mod-4 {1} " + vbCrLf, br.BaseStream.Position, (br.BaseStream.Position) Mod 4L)
        If l > 0 Then
            For i = 1 To 4 - l
                br.Write(b)
            Next
        End If
        save_has_uv2 = False
        For i = 1 To obj_cnt
            pnter = m_groups(id).list(i - 1)
            If fbxgrp(pnter).has_uv2 = 1 Then
                ' (4+4)*j
                uv2_cnt = fbxgrp(pnter).nPrimitives_ - 1  ' this had better = total_verts or we are screwed!
                For j = 0 To uv2_cnt
                    br.Write(fbxgrp(pnter).vertices(j).u2)
                    br.Write(fbxgrp(pnter).vertices(j).v2)
                Next
                save_has_uv2 = True
                l = (br.BaseStream.Position) Mod 4L
                padding += l
                Console.Write("uv2 raw" + vbTab + "base {0} , mod-4 {1} " + vbCrLf, br.BaseStream.Position, (br.BaseStream.Position) Mod 4L)
                If l > 0 Then
                    For j = 1 To 4 - l
                        br.Write(b)
                    Next
                End If
            End If
        Next
    End Sub
    Private Structure xyznuvtb_
        Dim x, y, z As Single
        Dim n As UInt32
        Dim u, v As Single
        Dim t, b As UInt32
    End Structure
    Private Sub write_list_data(ByVal id As Integer)
        Dim xyz As New xyznuvtb_
        Dim len_vertex As UInt32 = Marshal.SizeOf(xyz)
        total_indices = 0
        For i = 1 To obj_cnt
            pnter = m_groups(id).list(i - 1)
            total_indices += (fbxgrp(pnter).nPrimitives_ * 3)
        Next
        Dim h2() = "list".ToArray
        ind_scale = 2
        If total_indices > &HFFFF Then
            ind_scale = 4
            h2 = "list32".ToArray
        End If
        ReDim Preserve h2(63)
        br.Write(h2)
        br.Write(total_indices)
        br.Write(Convert.ToUInt32(obj_cnt)) 'write how many objects there are in this model
        Dim off As UInt32 = 0
        For i = 1 To obj_cnt
            Dim cnt = 0
            pnter = m_groups(id).list(i - 1)
            For j As UInt32 = 0 To fbxgrp(pnter).nPrimitives_ - 1
                'note: my routine uses other rotation
                If fbxgrp(pnter).is_new_model Then
                    If ind_scale = 2 Then
                        If frmWritePrimitive.flipWindingOrder_cb.Checked Then
                            br.Write(Convert.ToUInt16(fbxgrp(pnter).indicies(j).v1 + off))
                            br.Write(Convert.ToUInt16(fbxgrp(pnter).indicies(j).v2 + off))
                            br.Write(Convert.ToUInt16(fbxgrp(pnter).indicies(j).v3 + off))
                        Else
                            br.Write(Convert.ToUInt16(fbxgrp(pnter).indicies(j).v2 + off))
                            br.Write(Convert.ToUInt16(fbxgrp(pnter).indicies(j).v1 + off))
                            br.Write(Convert.ToUInt16(fbxgrp(pnter).indicies(j).v3 + off))

                        End If
                        If fbxgrp(pnter).indicies(j).v1 + off > cnt Then cnt = fbxgrp(pnter).indicies(j).v1
                        If fbxgrp(pnter).indicies(j).v2 + off > cnt Then cnt = fbxgrp(pnter).indicies(j).v2
                        If fbxgrp(pnter).indicies(j).v3 + off > cnt Then cnt = fbxgrp(pnter).indicies(j).v3
                    Else
                        If frmWritePrimitive.flipWindingOrder_cb.Checked Then
                            br.Write(fbxgrp(pnter).indicies(j).v1 + off)
                            br.Write(fbxgrp(pnter).indicies(j).v2 + off)
                            br.Write(fbxgrp(pnter).indicies(j).v3 + off)
                        Else
                            br.Write(fbxgrp(pnter).indicies(j).v2 + off)
                            br.Write(fbxgrp(pnter).indicies(j).v1 + off)
                            br.Write(fbxgrp(pnter).indicies(j).v3 + off)

                        End If
                        If fbxgrp(pnter).indicies(j).v1 + off > cnt Then cnt = fbxgrp(pnter).indicies(j).v1
                        If fbxgrp(pnter).indicies(j).v2 + off > cnt Then cnt = fbxgrp(pnter).indicies(j).v2
                        If fbxgrp(pnter).indicies(j).v3 + off > cnt Then cnt = fbxgrp(pnter).indicies(j).v3
                    End If
                Else
                    If ind_scale = 2 Then
                        If frmWritePrimitive.flipWindingOrder_cb.Checked Then
                            br.Write(Convert.ToUInt16(fbxgrp(pnter).indicies(j).v2 + off))
                            br.Write(Convert.ToUInt16(fbxgrp(pnter).indicies(j).v1 + off))
                            br.Write(Convert.ToUInt16(fbxgrp(pnter).indicies(j).v3 + off))
                        Else
                            br.Write(Convert.ToUInt16(fbxgrp(pnter).indicies(j).v1 + off))
                            br.Write(Convert.ToUInt16(fbxgrp(pnter).indicies(j).v2 + off))
                            br.Write(Convert.ToUInt16(fbxgrp(pnter).indicies(j).v3 + off))
                        End If
                        If fbxgrp(pnter).indicies(j).v1 + off > cnt Then cnt = fbxgrp(pnter).indicies(j).v1
                        If fbxgrp(pnter).indicies(j).v2 + off > cnt Then cnt = fbxgrp(pnter).indicies(j).v2
                        If fbxgrp(pnter).indicies(j).v3 + off > cnt Then cnt = fbxgrp(pnter).indicies(j).v3
                    Else
                        If frmWritePrimitive.flipWindingOrder_cb.Checked Then
                            br.Write(fbxgrp(pnter).indicies(j).v2 + off)
                            br.Write(fbxgrp(pnter).indicies(j).v1 + off)
                            br.Write(fbxgrp(pnter).indicies(j).v3 + off)
                        Else
                            br.Write(fbxgrp(pnter).indicies(j).v1 + off)
                            br.Write(fbxgrp(pnter).indicies(j).v2 + off)
                            br.Write(fbxgrp(pnter).indicies(j).v3 + off)
                        End If
                        If fbxgrp(pnter).indicies(j).v1 + off > cnt Then cnt = fbxgrp(pnter).indicies(j).v1
                        If fbxgrp(pnter).indicies(j).v2 + off > cnt Then cnt = fbxgrp(pnter).indicies(j).v2
                        If fbxgrp(pnter).indicies(j).v3 + off > cnt Then cnt = fbxgrp(pnter).indicies(j).v3
                    End If
                End If
            Next
            off += fbxgrp(pnter).nVertices_
        Next
        Dim s_index, s_vertex As UInt32
        For i = 1 To obj_cnt
            pnter = m_groups(id).list(i - 1)
            Dim pnter2 = pnter
            If i > 1 Then
                pnter2 = m_groups(id).list(i - 2) 'we have to do it this way because added items wont be in order
                s_index += fbxgrp(pnter2).nPrimitives_ * 3
                s_vertex += fbxgrp(pnter2).nVertices_
            End If
            pnter = m_groups(id).list(i - 1)
            br.Write(s_index)
            br.Write(fbxgrp(pnter).nPrimitives_)
            br.Write(s_vertex)
            br.Write(fbxgrp(pnter).nVertices_)
        Next
        l = (br.BaseStream.Position) Mod 4L
        padding = l
        Console.Write("indices" + vbTab + "base {0} , mod-4 {1} " + vbCrLf, br.BaseStream.Position, (br.BaseStream.Position) Mod 4L)
        If l > 0 Then
            For i = 1 To 4 - l
                br.Write(b)
            Next
        End If

    End Sub

    Private Sub write_BSP2(ByVal id As Integer)
        'write BSP2 data if it exists
        Dim p = br.BaseStream.Position
        For i = 1 To obj_cnt
            pnter = m_groups(id).list(i - 1)
            If _group(pnter).bsp2_id > 0 Then
                'bsp2_size = _group(pnter).bsp2_data.Length - 2
                br.Write(_group(pnter).bsp2_data, 0, _group(pnter).bsp2_data.Length - 2)
                l = (br.BaseStream.Position) Mod 4L
                Console.Write("bsp2" + vbTab + "base {0} , mod-4 {1} " + vbCrLf, br.BaseStream.Position, (br.BaseStream.Position) Mod 4L)
                bsp2_size = br.BaseStream.Position - p ' get size
                If l > 0 Then
                    For j = 1 To 4 - l
                        br.Write(b)
                    Next
                End If
                p = br.BaseStream.Position
                'bsp2_mat_size = _group(pnter).bsp2_material_data.Length - 2
                br.Write(_group(pnter).bsp2_material_data, 0, _group(pnter).bsp2_material_data.Length - 2)
                bsp2_mat_size = br.BaseStream.Position - p ' get size
                l = (br.BaseStream.Position) Mod 4L
                Console.Write("bsp2_m" + vbTab + "base {0} , mod-4 {1} " + vbCrLf, br.BaseStream.Position, (br.BaseStream.Position) Mod 4L)
                If l > 0 Then
                    For j = 1 To 4 - l
                        br.Write(b)
                    Next
                End If
                Exit For
            End If
        Next


    End Sub
 
End Module
