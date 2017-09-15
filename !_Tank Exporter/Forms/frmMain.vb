﻿
#Region "imports"
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
Imports Microsoft.VisualBasic.Strings
Imports System.Math
Imports System.Object
Imports System.Threading
Imports System.Data
Imports Tao.DevIl
Imports System.Runtime.InteropServices
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports Ionic.Zip
Imports System.Drawing.Imaging
Imports System.Globalization
#End Region

Public Class frmMain
#Region "variables"
    Public Background_image_id As Integer
    Private window_state As Integer
    Public Show_lights As Boolean = False
    Public gl_stop As Boolean = False
    Public update_thread As New Thread(AddressOf update_mouse)
    Public path_set As Boolean = False
    Public res_mods_path_set As Boolean = False
    Dim mouse As vec2
    Dim move_cam_z, M_DOWN, move_mod, z_move As Boolean

    Public Shared packages(12) As ZipFile
    Public Shared packages_HD(12) As ZipFile
    Public Shared shared_pkg As Ionic.Zip.ZipFile
    Public Shared shared_sandbox_pkg As Ionic.Zip.ZipFile
    Public shared_contents_build As New Ionic.Zip.ZipFile
    Public gui_pkg As Ionic.Zip.ZipFile
    Public scripts_pkg As Ionic.Zip.ZipFile

    Dim treeviews(10) As TreeView
    Public icons(10) As pngs
    Structure pngs
        Public img() As System.Drawing.Bitmap
    End Structure
    Public node_list(10) As t_array
    Public Structure t_array
        Public item() As t_items_
    End Structure
    Public Structure t_items_
        Public name As String
        Public node As TreeNode
        Public package As String
        Public icon As System.Drawing.Bitmap
    End Structure
    Private strings(3000) As String
    Dim TreeView1 As New TreeView
    Dim TreeView2 As New TreeView
    Dim TreeView3 As New TreeView
    Dim TreeView4 As New TreeView
    Dim TreeView5 As New TreeView
    Dim TreeView6 As New TreeView
    Dim TreeView7 As New TreeView
    Dim TreeView8 As New TreeView
    Dim TreeView9 As New TreeView
    Dim TreeView10 As New TreeView

#End Region

    Private Sub frmMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        _Started = False
        Try
            While update_thread.IsAlive
                _Started = False
                update_thread.Abort()
            End While
            DisableOpenGL()
            'delete any data we created
            For i = 1 To 10
                packages(i).Dispose()
            Next
            gui_pkg.Dispose()
            scripts_pkg.Dispose()
            shared_pkg.Dispose()
            shared_sandbox_pkg.Dispose()
        Catch ex As Exception

        End Try
        Try
            For i = 1 To 10
                packages_HD(i).Dispose()
            Next
        Catch ex As Exception

        End Try
        '--------------------------------------------------------
        '--------------------------------------------------------

    End Sub

    Private Sub frmMain_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = 16 Then
            move_mod = True
        End If
        If e.KeyCode = 17 Then
            z_move = True
        End If
        If e.KeyCode = Keys.F3 Then
            If spin_light Then
                spin_light = False
            Else
                spin_light = True
            End If
        End If
        If e.KeyCode = Keys.N Then
            normal_shader_mode += 1
            If normal_shader_mode > 2 Then
                normal_shader_mode = 0
            End If
        End If
        If e.KeyCode = Keys.L Then
            If Show_lights Then
                Show_lights = False
            Else
                Show_lights = True
            End If
        End If
        'If e.KeyCode = Keys.S Then
        '    frmEditFrag.Show()
        '    frmEditFrag.TopMost = True
        'End If
        e.Handled = True
    End Sub

    Private Sub frmMain_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
        If move_mod Then
            move_mod = False
        End If
        If z_move Then
            z_move = False
        End If
    End Sub

    Private Sub frmMain_ResizeBegin(sender As Object, e As EventArgs) Handles Me.ResizeBegin
        'gl_stop = True
        If Not _Started Then Return
        Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE)
        Gl.glDisable(Gl.GL_DEPTH_TEST)
        Gl.glEnable(Gl.GL_TEXTURE_2D)
        Gl.glDisable(Gl.GL_BLEND)
        Gl.glColor4f(1.0, 1.0, 1.0, 1.0)
        OLD_WINDOW_HEIGHT = pb1.Height
        w_changing = True
    End Sub
    Dim w_changing As Boolean = False

    Private Sub frmMain_ResizeEnd(sender As Object, e As EventArgs) Handles Me.ResizeEnd
        'gl_stop = False

        If season_Buttons_VISIBLE Then
            WINDOW_HEIGHT_DELTA = pb1.Height - OLD_WINDOW_HEIGHT
            relocate_season_Bottons()
            relocate_camobuttons()
        End If
        If Not _Started Then Return
        w_changing = False
    End Sub

    Private Sub me_size_changed()
        If window_state <> Me.WindowState Then
            If Not Me.WindowState = FormWindowState.Minimized Then
                'gl_stop = True
                If season_Buttons_VISIBLE Then
                    WINDOW_HEIGHT_DELTA = pb1.Height - OLD_WINDOW_HEIGHT
                    relocate_season_Bottons()
                    relocate_camobuttons()
                    OLD_WINDOW_HEIGHT = pb1.Height
                End If
                w_changing = False
                window_state = Me.WindowState
                Return
                gl_stop = False
            End If
            window_state = Me.WindowState
        End If
        If Not _Started Then Return
        If season_Buttons_VISIBLE Then
            WINDOW_HEIGHT_DELTA = pb1.Height - OLD_WINDOW_HEIGHT
            relocate_season_Bottons()
            relocate_camobuttons()
            OLD_WINDOW_HEIGHT = pb1.Height
        End If
        draw_scene()
    End Sub

 
    '############################################################################ form load
    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim nonInvariantCulture As CultureInfo = New CultureInfo("en-US")
        nonInvariantCulture.NumberFormat.NumberDecimalSeparator = "."
        Thread.CurrentThread.CurrentCulture = nonInvariantCulture
        cam_x = 0
        cam_y = 0
        cam_z = 10
        Cam_X_angle = PI * 0.25
        Cam_Y_angle = -PI * 0.25
        view_radius = -10.0
        tank_label.Text = ""
        Me.Show()
        SplitContainer1.SplitterDistance = 720
        Application.DoEvents()
        Me.Width = 1440
        Me.Height = 800
        pb1.Visible = False
        iconbox.Visible = False
        Application.DoEvents()
        pb1.Visible = True

        '---------------------------
        info_Label.BringToFront()
        info_Label.Parent = Me
        info_Label.Size = MM.Size
        info_Label.Dock = DockStyle.Top
        MM.Location = New Point(0, 0)
        info_Label.Text = "Welcome... Version:" + Application.ProductVersion
        Me.Text = " Tank Exporter Version:" + Application.ProductVersion
        '====================================================================================================
        SplitContainer2.Panel2.Controls.Add(tanklist)
        tanklist.Visible = False
        'tanklist.ScrollBars = ScrollBars.Vertical
        tanklist.Multiline = True
        tanklist.ForeColor = Color.White
        tanklist.BackColor = SplitContainer2.Panel2.BackColor
        tanklist.Dock = DockStyle.Fill
        tanklist.BringToFront()

        'tanklist.BackColor = iconbox.BackColor
        tanklist.Font = TreeView1.Font
        tanklist.SendToBack()
        Application.DoEvents()

        'fire up OpenGL amd IL
        Il.ilInit()
        Ilu.iluInit()
        Ilut.ilutInit()
        EnableOpenGL()
        Application.DoEvents()
        Application.DoEvents()
        Application.DoEvents()
        Application.DoEvents()
        Application.DoEvents()
        '====================================================================================================
        _Started = True
        '====================================================================================================
        'Check for temp storage folder.. It it exist.. load the API data.. 
        'other wise make the directory and get the API data.
        Temp_Storage = Path.GetTempPath ' this gets the user temp storage folder
        Temp_Storage += "wot_temp"
        If Not System.IO.Directory.Exists(Temp_Storage) Then
            System.IO.Directory.CreateDirectory(Temp_Storage)
        End If
        '====================================================================================================

        If My.Settings.game_path = "" Then
            MsgBox("Game Location needs to be set.", MsgBoxStyle.Information)
            M_Path.PerformClick()
        End If
        If My.Settings.game_path = "" Then
            MsgBox("res_mods Location needs to be set.", MsgBoxStyle.Information)
            m_res_mods_path.PerformClick()
        End If
        '====================================================================================================
        info_Label.Text = "Loading Data from Packages..."
        Application.DoEvents()

        Try

            gui_pkg = New Ionic.Zip.ZipFile(My.Settings.game_path + "\res\packages\gui.pkg")
            scripts_pkg = New Ionic.Zip.ZipFile(My.Settings.game_path + "\res\packages\scripts.pkg")
            shared_pkg = ZipFile.Read(My.Settings.game_path + "\res\packages\shared_content.pkg")
            shared_sandbox_pkg = ZipFile.Read(My.Settings.game_path + "\res\packages\shared_content_sandbox.pkg")
            packages(11) = shared_pkg
            'packages(12) = shared_sandbox_pkg

        Catch ex As Exception
            MsgBox("I was unable to load required pkg files!", MsgBoxStyle.Exclamation, "Error!")
            My.Settings.game_path = ""
            My.Settings.res_mods_path = ""
            My.Settings.Save()
            End
        End Try
        '====================================================================================================
        'MsgBox("I LOADED required pkg files!", MsgBoxStyle.Exclamation, "Error!")
        Try
            If File.Exists(Temp_Storage + "\shared_contents_build.pkg") Then
                shared_contents_build = ZipFile.Read(Temp_Storage + "\shared_contents_build.pkg")
                packages(11) = shared_contents_build

            Else
                shared_contents_build = New ZipFile(Temp_Storage + "\shared_contents_build.pkg")

                info_Label.Text = "First time Building and saving shared_contents_build.pkg.. Entry(0)"
                Application.DoEvents()
                Application.DoEvents()
                'packages_HD(11) = ZipFile.Read(My.Settings.game_path + "\res\packages\shared_content_hd.pkg")
                'packages_HD(12) = ZipFile.Read(My.Settings.game_path + "\res\packages\shared_content_sandbox_hd.pkg")
                Dim cnt As Integer = 1
                For Each entry In packages(11)
                    If entry.FileName.ToLower.Contains("vehicles") Then
                        Dim ms As New MemoryStream
                        entry.Extract(ms)
                        ms.Position = 0
                        shared_contents_build.AddEntry(entry.FileName, ms)
                        'GC.Collect()
                        info_Label.Text = "First time Building and saving shared_contents_build.pkg.. Entry(" + cnt.ToString("000") + ")"
                        Application.DoEvents()
                        cnt += 1
                    End If
                    Application.DoEvents()
                Next
                For Each entry In packages(12)
                    If entry.FileName.ToLower.Contains("vehicles") Then
                        Dim ms As New MemoryStream
                        entry.Extract(ms)
                        ms.Position = 0
                        shared_contents_build.AddEntry(entry.FileName, ms)
                        'GC.Collect()
                        info_Label.Text = "First time Building and saving shared_contents_build.pkg.. Entry(" + cnt.ToString("000") + ")"
                        Application.DoEvents()
                        cnt += 1
                    End If
                    Application.DoEvents()
                Next
                shared_contents_build.CompressionLevel = 0 ' no compression
                info_Label.Text = "Saving shared_contents_build.pkg.. This will take a while..."
                Application.DoEvents()
                shared_contents_build.Save()

            End If
        Catch ex As Exception
            log_text.AppendLine("HD Package files not found")
        End Try
        screen_totaled_draw_time = 1 ' to stop divide by zero exception

        Application.DoEvents()
        '====================================================================================================
        m_chassis.ForeColor = Color.DarkGreen
        m_hull.ForeColor = Color.DarkGreen
        m_turret.ForeColor = Color.DarkGreen
        m_gun.ForeColor = Color.DarkGreen
        '====================================================================================================
        tank_label.Parent = iconbox
        tank_label.Text = ""
        tank_label.Location = New Point(5, 10)
        '====================================================================================================
        info_Label.Text = "Getting Camo Textures..."
        'load_camo()
        '====================================================================================================
        load_customization_files()
        'MsgBox("Past load_customization_files", MsgBoxStyle.Exclamation, "Debug")
        load_season_icons()
        Gl.glFinish()
        'removed this load of background image.. 
        'load_back_ground()
        'Gl.glFinish()
        'draw_background()
        'draw_background()
        'draw_background()
        'test_buttons()
        '====================================================================================================

        make_xy_grid()

        If Not File.Exists(Temp_Storage + "\in_shortnames.txt") Then
            get_tank_names()
        Else
            get_tank_info_from_temp_folder()
        End If

        Application.DoEvents()
        set_treeview(TreeView1)
        Application.DoEvents()
        set_treeview(TreeView2)
        Application.DoEvents()
        set_treeview(TreeView3)
        Application.DoEvents()
        set_treeview(TreeView4)
        Application.DoEvents()
        set_treeview(TreeView5)
        Application.DoEvents()
        set_treeview(TreeView6)
        Application.DoEvents()
        set_treeview(TreeView7)
        Application.DoEvents()
        set_treeview(TreeView8)
        Application.DoEvents()
        set_treeview(TreeView9)
        Application.DoEvents()
        set_treeview(TreeView10)
        '-----------------------------
        Application.DoEvents()
        treeviews(1) = TreeView1
        Application.DoEvents()
        treeviews(2) = TreeView2
        Application.DoEvents()
        treeviews(3) = TreeView3
        Application.DoEvents()
        treeviews(4) = TreeView4
        Application.DoEvents()
        treeviews(5) = TreeView5
        Application.DoEvents()
        treeviews(6) = TreeView6
        Application.DoEvents()
        treeviews(7) = TreeView7
        Application.DoEvents()
        treeviews(8) = TreeView8
        Application.DoEvents()
        treeviews(9) = TreeView9
        Application.DoEvents()
        treeviews(10) = TreeView10
        '-----------------------------
        Application.DoEvents()

        'Create a temp folder for app to use.
        '====================================================================================================
        TC1.SelectedIndex = 0
        Application.DoEvents()
        load_tabs()
        Application.DoEvents()
        info_Label.Visible = False
        make_shaders() 'compile the shaders
        set_shader_variables() ' update uniform addresses
        TC1.SelectedIndex = 0
        'clean up the grabage
        GC.Collect()
        GC.WaitForFullGCComplete()
        Me.KeyPreview = True    'so i catch keyboard before despatching it
        w_changing = False
        Startup_Timer.Enabled = True
        Application.DoEvents()
        AddHandler Me.SizeChanged, AddressOf me_size_changed
        window_state = Me.WindowState
    End Sub

    Private Sub load_customization_files()

        For Each entry In scripts_pkg
            If entry.FileName.Contains("customization.xml") Then
                Dim index As Integer = -1
                Dim filename As String = ""
                If entry.FileName.Contains("common") Then
                    Debug.WriteLine("----" + entry.FileName)
                Else
                    Dim ms As New MemoryStream
                    entry.Extract(ms)
                    openXml_stream_2(ms, Path.GetFileNameWithoutExtension(entry.FileName))
                    filename = entry.FileName
                    Debug.WriteLine(entry.FileName)
                    Dim ta = entry.FileName.Split("/")
                    Select Case ta(3).ToLower
                        Case "usa"
                            index = 0
                        Case "uk"
                            index = 1
                        Case "china"
                            index = 2
                        Case "czech"
                            index = 3
                        Case "france"
                            index = 4
                        Case "germany"
                            index = 5
                        Case "japan"
                            index = 6
                        Case "poland"
                            index = 7
                        Case "ussr"
                            index = 8
                        Case "sweden"
                            index = 9
                    End Select
                End If
                build_customization_tables(index, filename)
            End If
        Next

    End Sub

    Private Sub build_customization_tables(ByVal id As Integer, ByVal filename As String)
        If filename.Length < 10 Then Return
        custom_tables(id) = New DataSet

        Dim data_set As New DataSet
        Dim docx = XDocument.Parse(TheXML_String)
        Dim doc As New XmlDocument


        Dim root_node = doc.CreateElement("camouflage")
        'get the armorcolor
        Dim armorcolor = docx.Descendants("armorColor")
        Dim n_armorcolor = doc.CreateElement("armorcolor")
        n_armorcolor.InnerText = armorcolor.Value.ToString
        root_node.AppendChild(n_armorcolor)
        Dim index As Integer = 0
        'get the textures
        For Each el In docx.Descendants("texture")
            Dim tex = doc.CreateElement("texture")
            Dim id_node = doc.CreateElement("id")
            id_node.InnerText = index.ToString
            tex.InnerText = el.Value.ToString
            '---------
            Dim rr = el.Parent
            Dim g_node = doc.CreateElement("camo")
            Dim material = doc.CreateElement("material")
            'material.AppendChild(tex)
            'get kind
            Dim kind_ = rr.Descendants("kind")
            Dim kind = doc.CreateElement("kind")
            kind.InnerText = kind_.Value.ToLower
            'get color
            Dim color_ = rr.Descendants("colors")
            Dim color = doc.CreateElement("colors")
            color.AppendChild(id_node)
            color.AppendChild(kind)
            color.AppendChild(tex)
            For Each c In color_.Elements
                Dim e = doc.CreateElement(c.Name.ToString)
                e.InnerText = c.Value.ToString
                color.AppendChild(e)
            Next
            'material.AppendChild(color)
            Dim tiling = rr.Descendants("tiling")
            Dim cnt As Integer = 0
            For Each tank In tiling.Elements
                Dim name_str = tank.Name.ToString
                'lol wot has added some tanks 2 times to the tiling section
                If color.InnerXml.Contains(name_str) Then
                    name_str += "ERROR" + cnt.ToString("000")
                End If
                Dim tn = doc.CreateElement(name_str)
                tn.InnerText = tank.Value.ToString
                color.AppendChild(tn)
                cnt += 1
            Next
            g_node.AppendChild(color)
            root_node.AppendChild(g_node)

            index += 1
        Next
        doc.AppendChild(root_node)

        Dim fm As New MemoryStream
        doc.Save(fm)
        fm.Position = 0
        data_set.ReadXml(fm)
        custom_tables(id) = data_set.Copy
        fm.Dispose()

    End Sub

    Private Sub load_camo()
        Dim namelist(1000) As String
        Dim cnt As Integer = 0
        For Each ent In shared_sandbox_pkg
            If Path.GetDirectoryName(ent.FileName).Contains("Camouflage") Then
                If Path.GetDirectoryName(ent.FileName).Contains("vehicles") Then
                    namelist(cnt) = ent.FileName
                    cnt += 1
                End If

            End If
        Next
        ReDim Preserve namelist(cnt - 1)
        ReDim camo_images(cnt - 1)
        For i = 0 To cnt - 1
            camo_images(i) = New camo_
            Dim ms = New MemoryStream
            Dim e = shared_sandbox_pkg(namelist(i))
            If e IsNot Nothing Then
                e.Extract(ms)
                camo_images(i).id = get_texture_and_bitmap(ms, namelist(i), camo_images(i).bmp)
                delete_image_start = camo_images(i).id + 1
            End If
        Next
        Dim w = FrmCamo.Width - FrmCamo.ClientSize.Width
        Dim size = Sqrt(cnt - 1)
        FrmCamo.ClientSize = New Size(New System.Drawing.Point(size * 50, size * 50))
        Dim col = 0
        Dim row = 0
        For i = 0 To cnt - 1
            Dim b As New Button
            b.Width = 50
            b.Height = 50
            b.BackgroundImage = camo_images(i).bmp
            b.ImageAlign = ContentAlignment.MiddleCenter
            b.BackgroundImageLayout = ImageLayout.Stretch
            AddHandler b.Click, AddressOf handle_imgbtn_click
            Dim p = New System.Drawing.Point(col * 50, row * 50)
            b.Location = p
            b.Tag = camo_images(i).id
            b.Text = camo_images(i).id.ToString
            b.Font = font_holder.Font
            b.ForeColor = Color.White
            FrmCamo.Controls.Add(b)
            col += 1
            If col > 9 Then
                col = 0
                row += 1
            End If
        Next
    End Sub
    Private Sub handle_imgbtn_click(sender As Object, e As MouseEventArgs)
        Dim b = DirectCast(sender, Button)
        'current_camo_selection = CInt(b.Tag)
        For i = 1 To object_count
            If Not _object(i).name.ToLower.Contains("chassis") Then
                '_object(i).use_camo = current_camo_selection
            End If
        Next
    End Sub
    Private Sub clear_temp_folder()
        If MsgBox("This will close the application.." + vbCrLf + "Would you like to continue?", MsgBoxStyle.YesNo, "Warning..") = MsgBoxResult.No Then
            Return
        End If
        Dim f As DirectoryInfo = New DirectoryInfo(Temp_Storage)
        If f.Exists Then
            For Each fi In f.GetFiles
                fi.Delete()
            Next
        End If
        f.Delete()
        Application.Exit()
    End Sub

    Private Sub draw_background()
        'Gdi.SwapBuffers(pb1_hDC)
        If Not (Wgl.wglMakeCurrent(pb1_hDC, pb1_hRC)) Then
            MessageBox.Show("Unable to make rendering context current")
            End
        End If
        ResizeGL()
        ViewPerspective()
        ViewOrtho()
        Dim e = Gl.glGetError
        Dim sw! = pb1.Width
        Dim sh! = pb1.Height
        Dim p As New Point(0.0!, 0.0!)
        'Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
        e = Gl.glGetError

        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, Background_image_id)
        Dim w, h As Integer
        Gl.glGetTexLevelParameteriv(Gl.GL_TEXTURE_2D, 0, Gl.GL_TEXTURE_WIDTH, w)
        Gl.glGetTexLevelParameteriv(Gl.GL_TEXTURE_2D, 0, Gl.GL_TEXTURE_HEIGHT, h)
        p.X = -((w / 2) - (sw / 2))
        p.Y = (h / 2) - (sh / 2)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, Background_image_id)
        Gl.glBegin(Gl.GL_QUADS)
        '  CW...
        '  1 ------ 2
        '  |        |
        '  |        |
        '  4 ------ 3
        '
        Gl.glTexCoord2f(0.0!, 0.0!)
        Gl.glVertex2f(p.X, p.Y)

        Gl.glTexCoord2f(1.0!, 0.0!)
        Gl.glVertex2f(p.X + w, p.Y)

        Gl.glTexCoord2f(1.0!, 1.0!)
        Gl.glVertex2f(p.X + w, p.Y - h)

        Gl.glTexCoord2f(0.0!, 1.0!)
        Gl.glVertex2f(p.X, p.Y - h)
        Gl.glEnd()
        Gdi.SwapBuffers(pb1_hDC)

        Gl.glBegin(Gl.GL_QUADS)
        '  CW...
        '  1 ------ 2
        '  |        |
        '  |        |
        '  4 ------ 3
        '
        Gl.glTexCoord2f(0.0!, 0.0!)
        Gl.glVertex2f(p.X, p.Y)

        Gl.glTexCoord2f(1.0!, 0.0!)
        Gl.glVertex2f(p.X + w, p.Y)

        Gl.glTexCoord2f(1.0!, 1.0!)
        Gl.glVertex2f(p.X + w, p.Y - h)

        Gl.glTexCoord2f(0.0!, 1.0!)
        Gl.glVertex2f(p.X, p.Y - h)
        Gl.glEnd()
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Gdi.SwapBuffers(pb1_hDC)
        e = Gl.glGetError

    End Sub
    Private Sub load_back_ground()
        Dim rnd As New Random
tryagain:
        Dim rn = CInt(rnd.NextDouble * 44.0)
        If rn < 8 Then
            GoTo tryagain
        End If
        Dim f = gui_pkg("gui/maps/login/back_" + rn.ToString + "_without_sparks.png")
        Dim ms As New MemoryStream
        Try

            If ms IsNot Nothing Then
                f.Extract(ms)
                Background_image_id = get_png_id(ms)
                delete_image_start += 1 ' so this texture is NOT deleted
            End If
        Catch ex As Exception
            GoTo tryagain

        End Try
    End Sub
    Private Sub set_treeview(ByRef tv As TreeView)
        Dim st_index = TC1.SelectedIndex
        Dim st = TC1.SelectedTab

        tv = New mytreeview
        tv.Font = font_holder.Font.Clone
        tv.ContextMenuStrip = conMenu
        tv.DrawMode = TreeViewDrawMode.OwnerDrawText
        tv.Dock = DockStyle.Fill
        tv.Nodes.Clear()
        tv.BackColor = Color.DimGray
        tv.ForeColor = Color.Black
        tv.HotTracking = True
        tv.HideSelection = True
        st.Controls.Add(tv)
        If st_index < 9 Then
            TC1.SelectedIndex = st_index + 1
        End If
        Application.DoEvents()
    End Sub

    Private Sub get_tank_parts_from_xml(ByVal tank As String, ByRef data_set As DataSet)
        'once again the non-standard name calling causes issues
        ' what not use USA for the nation in all paths???? czech, japan, sweeden, poland are ok as is
        Dim turret_names() As String = {"0", "1", "2", "3", "_0", "_1", "_2", "_3"}
        If tank.Contains("american") Then
            tank = tank.Replace("american", "usa")
        End If
        If tank.Contains("british") Then
            tank = tank.Replace("british", "uk")
        End If
        If tank.Contains("chinese") Then
            tank = tank.Replace("chinese", "china")
        End If
        If tank.Contains("french") Then
            tank = tank.Replace("french", "france")
        End If
        If tank.Contains("german") Then
            tank = tank.Replace("german", "germany")
        End If
        If tank.Contains("russian") Then
            tank = tank.Replace("russian", "ussr")
        End If
        ' dont need to change poland, czech, sweden or japan
        'If tank.Contains("czech") Then
        '    tank = tank.Replace("czech", "czech")
        'End If


        Dim f = scripts_pkg("scripts\item_defs\" + tank)
        If f Is Nothing Then
            Return
        End If
        Dim ms As New MemoryStream
        f.Extract(ms)
        openXml_stream(ms, "nation")
        ms.Dispose()
        Dim docx = XDocument.Parse(TheXML_String)
        Dim doc As New XmlDocument
        Dim xmlroot As XmlNode = xDoc.CreateElement(XmlNodeType.Element, "root", "")
        Dim root_node As XmlNode = doc.CreateElement("model")
        doc.AppendChild(root_node)
        'doc.DocumentElement.ParentNode.Value = "<root>" + vbCrLf + "</root>"



        'this is going to be a mess :(
        Dim turret_tiling = doc.CreateElement("turret_tiling")
        Dim found_camo As Boolean = False
        For Each turret0 As XElement In docx.Descendants("turrets0")
            For Each model In turret0.Descendants("undamaged")
                If model.Value.ToLower.ToLower.Contains("turret") Then

                    Dim p = model.Parent.FirstNode
                    Dim pp = p.Parent
                    Dim ppp = pp.Parent

                    For Each n In ppp.Elements
                        If n.LastNode IsNot Nothing Then

                            If n.LastNode.ToString.ToLower.Contains("tiling") And Not n.FirstNode.ToString.ToLower.Contains("gun") Then

                                Dim tile = n.Descendants("tiling")
                                Dim cct = doc.CreateElement("tiling")
                                cct.InnerText = tile.Value.ToString
                                turret_tiling.AppendChild(cct)
                                root_node.AppendChild(turret_tiling)
                                found_camo = True
                            End If

                        End If
                    Next
                    If Not found_camo Then
                        Dim cct = doc.CreateElement("tiling")
                        cct.InnerText = "1.0 1.0 0.0 0.0"
                        turret_tiling.AppendChild(cct)
                        root_node.AppendChild(turret_tiling)
                    End If
                End If

            Next

        Next
        ' root_node.AppendChild(cct)
        Dim turrets = doc.CreateElement("turrets")
        root_node.AppendChild(turrets)

        For Each turret0 As XElement In docx.Descendants("turrets0")
            For Each turret In turret0.Descendants
                'If turret.Name.ToString.ToLower.Contains("turret" + ext) Then
                'Dim turret_name As XElement = turret.FirstNode

                'add turrets name
                Dim r_node = doc.CreateElement("node")
                For Each guns As XElement In turret.Descendants("guns")
                    Dim p = guns.Parent.FirstNode
                    Dim pp = p.Parent
                    'Dim ppp = pp.Parent.FirstNode
                    Dim t_node = pp.FirstNode
                    Dim t_name As String = pp.Name.ToString
                    Dim nd_turret = doc.CreateElement("turret_name")
                    Dim tx_turret = doc.CreateTextNode(t_name)
                    Dim nd_turret_name = doc.CreateElement("turret")

                    nd_turret_name.InnerText = turret.Name.ToString
                    nd_turret.AppendChild(nd_turret_name)

                    Dim gun_name As XElement = guns.FirstNode
                    For Each gun In guns.Descendants("undamaged")

                        Dim nd_gun = doc.CreateElement("gun")
                        r_node.AppendChild(nd_gun)
                        Dim nd_gun_name = doc.CreateElement("gun_name")
                        Dim gg = gun.Parent
                        Dim ggg = gg.Parent
                        Dim g_name = ggg.Name.ToString
                        nd_gun_name.InnerText = g_name
                        nd_gun.AppendChild(nd_gun_name)

                        gun.Value = gun.Value.Replace("/", "\")
                        Dim nd_g = doc.CreateElement("model")
                        nd_g.InnerText = gun.Value
                        nd_gun.AppendChild(nd_g)
                        Dim camo_cnt As Integer = 0
                        For Each camo In gun_name.Descendants("camouflage")
                            Dim nd_c = doc.CreateElement("gun_camouflage")
                            nd_c.InnerText = camo.Value
                            nd_gun.AppendChild(nd_c)
                            camo_cnt += 1
                        Next
                        If camo_cnt = 0 Then
                            Dim nd_c = doc.CreateElement("gun_camouflage")
                            nd_c.InnerText = "1 1 0 0"
                            nd_gun.AppendChild(nd_c)

                        End If
                    Next
                    'nd_turret.LastChild.AppendChild(tx_gun)
                    r_node.AppendChild(nd_turret)
                    turrets.AppendChild(r_node)
                Next
            Next
            'turrets.LastChild.AppendChild(tx_turret)
            'End If
        Next
        For Each turret In docx.Descendants("turrets0")

            Dim tur = turret.Descendants("models")
            For Each models In tur.Descendants("undamaged")
                If models.Value.ToString.ToLower.Contains("turret") Then
                    ' Dim t_e = doc.CreateElement("turret_model")
                    Dim t_n = doc.CreateElement("turret_model")
                    Dim no = doc.GetElementsByTagName("turret_models") ' see if thsi has been created already
                    If no.Count = 0 Then
                        Dim t_n1 = doc.CreateElement("model")
                        t_n1.InnerText = models.Value.ToString
                        t_n.AppendChild(t_n1)
                        root_node.AppendChild(t_n)

                    Else
                        'If they are out of order, the turret_model has already been created.
                        'We need to add to that element other wise it breaks the XML formating
                        With doc.SelectSingleNode("model/turret_model").CreateNavigator().AppendChild()
                            .WriteElementString("model", models.Value.ToString)
                            .WriteEndElement()
                            .Close()
                        End With

                    End If
                End If
            Next
        Next
        'root_node.AppendChild(t_root)

        Dim chassis = docx.Descendants("chassis")
        For Each ch In chassis.Descendants("undamaged")
            Dim c = doc.CreateElement("chassis")
            Dim cn = doc.CreateElement("model")
            cn.InnerText = ch.Value.ToString
            root_node.AppendChild(c)
            c.AppendChild(cn)
        Next
        Dim cnt As Integer = 0

        For Each n As XElement In docx.Descendants("hull")
            For Each h In n.Descendants("undamaged")
                Dim hn = doc.CreateElement("model")
                hn.InnerText = h.Value.ToString
                For Each camo As XElement In n.Descendants("camouflage")
                    cnt += 1
                    Dim hull = doc.CreateElement("hull")
                    root_node.AppendChild(hull)

                    Dim nd = doc.CreateElement("hull_camouflage")
                    nd.InnerText = camo.Value
                    hull.AppendChild(hn)
                    hull.AppendChild(nd)
                Next
                If cnt = 0 Then
                    Dim hull = doc.CreateElement("hull")
                    root_node.AppendChild(hull)
                    Dim nd = doc.CreateElement("hull_camouflage")
                    nd.InnerText = "1.0 1.0 0.0 0.0"
                    hull.AppendChild(hn)
                    hull.AppendChild(nd)

                End If
            Next
        Next
        Try

            Dim track = doc.CreateElement("track_info")
            cnt = 1
            Dim spline_ = docx.Descendants("splineDesc")
            Dim segr = spline_.Descendants("segmentModelRight")
            Dim segl = spline_.Descendants("segmentModelLeft")
            Dim segleftname = doc.CreateElement("left_filename")
            Dim segrightname = doc.CreateElement("right_filename")
            segleftname.InnerText = segl.Value.ToString
            segrightname.InnerText = segr.Value.ToString
            track.AppendChild(segleftname)
            track.AppendChild(segrightname)

            If xDoc.OuterXml.Contains("segment2Model") Then
                Dim segr2 = spline_.Descendants("segment2ModelRight")
                Dim segl2 = spline_.Descendants("segment2ModelLeft")
                Dim segleftname2 = doc.CreateElement("left2_filename")
                Dim segrightname2 = doc.CreateElement("right2_filename")
                segleftname2.InnerText = segl2.Value.ToString
                segrightname2.InnerText = segr2.Value.ToString
                track.AppendChild(segleftname2)
                track.AppendChild(segrightname2)
                cnt = 2


            End If
            'add seg cnt
            Dim seg_cnt = doc.CreateElement("seg_cnt")
            seg_cnt.InnerText = cnt.ToString
            track.AppendChild(seg_cnt)

            'get seglength and seg offsets
            Dim seg_ = docx.Descendants("segmentLength")
            Dim seg_length = doc.CreateElement("segment_length")
            seg_length.InnerText = seg_.Value.ToString
            track.AppendChild(seg_length)
            Dim segoffset = docx.Descendants("segmentOffset")
            Dim seg_off = doc.CreateElement("segmentOffset")
            seg_off.InnerText = segoffset.Value.ToString
            track.AppendChild(seg_off)

            If xDoc.OuterXml.Contains("segment2Offset") Then
                Dim segoffset2 = docx.Descendants("segment2Offset")
                Dim seg_off2 = doc.CreateElement("segment2Offset")
                seg_off2.InnerText = segoffset2.Value.ToString
                track.AppendChild(seg_off2)

            End If

            root_node.AppendChild(track)

        Catch ex As Exception

        End Try

        Dim fm As New MemoryStream
        doc.Save(fm)
        fm.Position = 0
        data_set.ReadXml(fm)
        ms.Dispose()
        fm.Dispose()
    End Sub
    Private Sub load_tabs()
        Try
            For i = 1 To 10
                info_Label.Text = " Creating Nodes by tier (" + i.ToString("00") + ")"
                Application.DoEvents()
                Application.DoEvents()
                store_in_treeview(i, treeviews(i))
            Next
            get_tanks_shared()
            get_tanks_sandbox()
            For i = 1 To 10
                info_Label.Text = "Adding Nodes to TreeView Lists (" + i.ToString("00") + ")"
                Dim l = node_list(i).item.Length - 2
                ReDim Preserve node_list(i).item(l) ' remove last empty item
                ReDim Preserve icons(i).img(l)
                'sort the array
                node_list(i).item = node_list(i).item.OrderByDescending(Function(c) c.node.Name).ToArray

                Application.DoEvents()
                Application.DoEvents()
                TC1.SelectedIndex = i - 1
                Dim tn = treeviews(i)
                For j = 0 To node_list(i).item.Length - 2
                    icons(i).img(j) = node_list(i).item(j).icon
                    tn.Nodes.Add(node_list(i).item(j).node)
                Next
            Next
        Catch ex As Exception
            MsgBox("Something went wrong adding to the Treeviews." + ex.Message, MsgBoxStyle.Exclamation, "Opps!")
        End Try
    End Sub

    Private Sub store_in_treeview(ByVal i As Integer, tn As TreeView)
        AddHandler tn.NodeMouseClick, AddressOf Me.tv_clicked
        AddHandler tn.NodeMouseHover, AddressOf Me.tv_mouse_enter
        AddHandler tn.MouseLeave, AddressOf Me.tv_mouse_leave
        tn.Tag = i

        'make new t_array
        node_list(i) = New t_array

        Application.DoEvents()
        Application.DoEvents()
        tn.FullRowSelect = False
        Application.DoEvents()
        Dim fpath = My.Settings.game_path + "\res\packages\vehicles_level_" + i.ToString("00") + ".pkg"
        packages(i) = Ionic.Zip.ZipFile.Read(fpath)
        Try
            Dim fpath2 = My.Settings.game_path + "\res\packages\vehicles_level_" + i.ToString("00") + "_hd.pkg"
            packages_HD(i) = Ionic.Zip.ZipFile.Read(fpath2)
        Catch ex As Exception
        End Try
        Dim cnt As Integer = 0
        ReDim icons(i).img(100)
        ReDim node_list(i).item(100)
        alltanks.Append("# Tier " + i.ToString("00") + vbCrLf)
        'Get tanks fron tier packages
        For Each entry As ZipEntry In packages(i)
            If entry.FileName.Contains("normal/lod0/Chassis.model") Then
                Dim t_name = entry.FileName
                Dim ta = t_name.Split("/")
                t_name = ""
                For j = 0 To 2
                    t_name += ta(j) + "/"
                Next
                Dim n As New TreeNode
                n.Text = ta(2)
                n.Tag = fpath + ":" + t_name
                'need this to look up actual tanks game name in the
                '\res\packages\scripts.pkg\scripts\item_defs\vehicles\***poland***\list.xml
                Dim s As String = ""
                Select Case ta(1)
                    Case "american"
                        n.Name = "usa"
                        s = get_user_name(n.Text)
                    Case "british"
                        n.Name = "uk"
                        s = get_user_name(n.Text)
                    Case "chinese"
                        n.Name = "china"
                        s = get_user_name(n.Text)
                    Case "czech"
                        n.Name = "czech"
                        s = get_user_name(n.Text)
                    Case "french"
                        n.Name = "france"
                        s = get_user_name(n.Text)
                    Case "german"
                        n.Name = "germany"
                        s = get_user_name(n.Text)
                    Case "japan"
                        n.Name = "japan"
                        s = get_user_name(n.Text)
                    Case "poland"
                        n.Name = "poland"
                        s = get_user_name(n.Text)
                    Case "russian"
                        n.Name = "ussr"
                        s = get_user_name(n.Text)
                    Case "sweden"
                        n.Name = "sweden"
                        s = get_user_name(n.Text)
                End Select
                If s.Length > 0 Then ' only save what actually exist
                    node_list(i).item(cnt) = New t_items_
                    Dim na = n.Text.Split("_")
                    If na(0).Length = 3 Then
                        na(0) += "99"
                    End If
                    node_list(i).item(cnt).name = na(0)
                    node_list(i).item(cnt).node = n
                    node_list(i).item(cnt).package = packages(i).Name
                    icons(i).img(cnt) = get_tank_icon(n.Text).Clone
                    If icons(i).img(cnt) IsNot Nothing Then
                        node_list(i).item(cnt).icon = icons(i).img(cnt).Clone
                        node_list(i).item(cnt).icon.Tag = current_png_path
                        cnt += 1
                    End If
                End If
            End If
        Next
        ReDim Preserve node_list(i).item(cnt)

        Application.DoEvents()
        ReDim Preserve icons(i).img(cnt)
        Application.DoEvents()
    End Sub

    Private Sub get_tanks_shared()
        For Each entry As ZipEntry In shared_pkg
            If entry.FileName.Contains("normal/lod0/Chassis.model") Then
                Dim t_name = entry.FileName
                Dim ta = t_name.Split("/")
                t_name = ""
                For j = 0 To 2
                    t_name += ta(j) + "/"
                Next
                Dim n As New TreeNode
                n.Text = ta(2)
                n.Tag = My.Settings.game_path + "\res\packages\shared_content.pkg" + ":" + t_name
                'need this to look up actual tanks game name in the
                '\res\packages\scripts.pkg\scripts\item_defs\vehicles\***poland***\list.xml
                Dim s As String = ""
                Dim i As Integer = 0
                Select Case ta(1)
                    Case "american"
                        n.Name = "usa"
                        s = get_user_name(n.Text)
                        i = CInt(get_tier_id(n.Text))
                    Case "british"
                        n.Name = "uk"
                        s = get_user_name(n.Text)
                        i = CInt(get_tier_id(n.Text))
                    Case "chinese"
                        n.Name = "china"
                        s = get_user_name(n.Text)
                        i = CInt(get_tier_id(n.Text))
                    Case "czech"
                        n.Name = "czech"
                        s = get_user_name(n.Text)
                        i = CInt(get_tier_id(n.Text))
                    Case "french"
                        n.Name = "france"
                        s = get_user_name(n.Text)
                        i = CInt(get_tier_id(n.Text))
                    Case "german"
                        n.Name = "germany"
                        s = get_user_name(n.Text)
                        i = CInt(get_tier_id(n.Text))
                    Case "japan"
                        n.Name = "japan"
                        s = get_user_name(n.Text)
                        i = CInt(get_tier_id(n.Text))
                    Case "poland"
                        n.Name = "poland"
                        s = get_user_name(n.Text)
                        i = CInt(get_tier_id(n.Text))
                    Case "russian"
                        n.Name = "ussr"
                        s = get_user_name(n.Text)
                        i = CInt(get_tier_id(n.Text))
                    Case "sweden"
                        n.Name = "sweden"
                        s = get_user_name(n.Text)
                        i = CInt(get_tier_id(n.Text))
                End Select
                If s.Length > 0 Then ' only save what actually exist
                    Dim cnt As Integer = node_list(i).item.Length
                    ReDim Preserve node_list(i).item(cnt)
                    ReDim Preserve icons(i).img(cnt)
                    cnt -= 1
                    node_list(i).item(cnt) = New t_items_
                    Dim na = n.Text.Split("_")
                    If na(0).Length = 3 Then
                        na(0) += "99"
                    End If
                    node_list(i).item(cnt).name = na(0)
                    node_list(i).item(cnt).node = n
                    node_list(i).item(cnt).package = My.Settings.game_path + "\res\packages\shared_content.pkg"
                    icons(i).img(cnt) = get_tank_icon(n.Text).Clone
                    If icons(i).img(cnt) IsNot Nothing Then
                        node_list(i).item(cnt).icon = icons(i).img(cnt).Clone
                        node_list(i).item(cnt).icon.Tag = current_png_path
                        'tn.Nodes.Add(n)
                    End If
                End If
            End If
        Next

        Application.DoEvents()
    End Sub
    Private Sub get_tanks_sandbox()
        For Each entry As ZipEntry In shared_sandbox_pkg
            If entry.FileName.Contains("normal/lod0/Chassis.model") Then
                Dim t_name = entry.FileName
                Dim ta = t_name.Split("/")
                t_name = ""
                For j = 0 To 2
                    t_name += ta(j) + "/"
                Next
                Dim n As New TreeNode
                n.Text = ta(2)
                n.Tag = My.Settings.game_path + "\res\packages\shared_content_sandbox.pkg" + ":" + t_name
                'need this to look up actual tanks game name in the
                '\res\packages\scripts.pkg\scripts\item_defs\vehicles\***poland***\list.xml
                Dim s As String = ""
                Dim i As Integer = 0
                Select Case ta(1)
                    Case "american"
                        n.Name = "usa"
                        s = get_user_name(n.Text)
                        i = CInt(get_tier_id(n.Text))
                    Case "british"
                        n.Name = "uk"
                        s = get_user_name(n.Text)
                        i = CInt(get_tier_id(n.Text))
                    Case "chinese"
                        n.Name = "china"
                        s = get_user_name(n.Text)
                        i = CInt(get_tier_id(n.Text))
                    Case "czech"
                        n.Name = "czech"
                        s = get_user_name(n.Text)
                        i = CInt(get_tier_id(n.Text))
                    Case "french"
                        n.Name = "france"
                        s = get_user_name(n.Text)
                        i = CInt(get_tier_id(n.Text))
                    Case "german"
                        n.Name = "germany"
                        s = get_user_name(n.Text)
                        i = CInt(get_tier_id(n.Text))
                    Case "japan"
                        n.Name = "japan"
                        s = get_user_name(n.Text)
                        i = CInt(get_tier_id(n.Text))
                    Case "poland"
                        n.Name = "poland"
                        s = get_user_name(n.Text)
                        i = CInt(get_tier_id(n.Text))
                    Case "russian"
                        n.Name = "ussr"
                        s = get_user_name(n.Text)
                        i = CInt(get_tier_id(n.Text))
                    Case "sweden"
                        n.Name = "sweden"
                        s = get_user_name(n.Text)
                        i = CInt(get_tier_id(n.Text))
                End Select
                If s.Length > 0 Then ' only save what actually exist
                    Dim cnt As Integer = node_list(i).item.Length
                    ReDim Preserve node_list(i).item(cnt)
                    ReDim Preserve icons(i).img(cnt)
                    cnt -= 1
                    node_list(i).item(cnt) = New t_items_
                    Dim na = n.Text.Split("_")
                    If na(0).Length = 3 Then
                        na(0) += "99"
                    End If
                    node_list(i).item(cnt).name = na(0)
                    node_list(i).item(cnt).node = n
                    node_list(i).item(cnt).package = My.Settings.game_path + "\res\packages\shared_content_sandbox.pkg"
                    icons(i).img(cnt) = get_tank_icon(n.Text).Clone
                    If icons(i).img(cnt) IsNot Nothing Then
                        node_list(i).item(cnt).icon = icons(i).img(cnt).Clone
                        node_list(i).item(cnt).icon.Tag = current_png_path
                        'tn.Nodes.Add(n)
                    End If
                End If
            End If
        Next

        Application.DoEvents()
    End Sub
    Private Function get_user_name(ByVal fname As String) As String

        Try

            Dim q = From row In TankDataTable _
                        Where row.Field(Of String)("tag") = fname _
              Select _
                   un = row.Field(Of String)("shortname"), _
                   tier = row.Field(Of String)("tier"), _
                   natiom = row.Field(Of String)("nation")
                   Order By tier Descending

            'Dim a = q(0).un.Split(":")
            If q(0) IsNot Nothing Then
                Return q(0).un

            End If
        Catch ex As Exception
            Return ""
        End Try
        Return ""
    End Function
    Private Function get_tier_id(ByVal fname As String) As String
        Try
            Dim q = From row In TankDataTable _
                        Where row.Field(Of String)("tag") = fname _
              Select _
                   un = row.Field(Of String)("shortname"), _
                   tier = row.Field(Of String)("tier"), _
                   natiom = row.Field(Of String)("nation")
                   Order By tier Descending

            'Dim a = q(0).un.Split(":")
            If q(0) IsNot Nothing Then
                Return q(0).tier
            End If
        Catch ex As Exception
            log_text.Append("Odd Tank: " + fname)
            Return "0"
        End Try
        log_text.AppendLine("Odd Tank: " + fname)
        Return "0"
    End Function

    Private Sub tv_clicked(ByVal sender As Object, ByVal e As TreeNodeMouseClickEventArgs)
        Dim tn = DirectCast(sender, TreeView)
        If e.Button = Forms.MouseButtons.Right Then
            file_name = e.Node.Tag
            iconbox.Visible = True
            iconbox.BackgroundImage = icons(tn.Tag).img(e.Node.Index)
            Dim s = get_shortname(e.Node)
            Dim ar = s.Split(":")
            tank_label.Text = ar(0)
            Application.DoEvents()
            Application.DoEvents()
            Application.DoEvents()
            Return
        End If
        Dim ts = tanklist.Text
        tn.SelectedNode = Nothing
        tn.SelectedNode = e.Node
        If ts.Contains(tn.SelectedNode.Text) Then
            tn.SelectedNode.ForeColor = Color.Black
            ts = Replace(ts, tn.SelectedNode.Text + vbCrLf, "")
        Else
            tn.SelectedNode.ForeColor = Color.White
            ts += tn.SelectedNode.Text + vbCrLf
        End If
        tanklist.Text = ts
        tn.SelectedNode = Nothing
        tn.Parent.Focus()

    End Sub
    Private Sub tv_mouse_enter(ByVal sender As Object, ByVal e As TreeNodeMouseHoverEventArgs)
        Dim tn = DirectCast(sender, TreeView)
        iconbox.Visible = True
        'iconbox.BringToFront()
        tn.Focus()
        iconbox.BackgroundImage = icons(tn.Tag).img(e.Node.Index)
        Dim s = get_shortname(e.Node)
        Dim ar = s.Split(":")
        tank_label.Text = ar(0)
        file_name = e.Node.Tag
        tn.Parent.Focus()
    End Sub
    Private Sub tv_mouse_leave(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim tn = DirectCast(sender, TreeView)
        If conMenu.Visible Then
            Return
        End If
        iconbox.Visible = False
        tn.Parent.Focus()
    End Sub
    Private Function get_shortname(ByVal n As TreeNode) As String
        Dim q = From row In TankDataTable _
            Where row.Field(Of String)("tag") = n.Text _
  Select _
       un = row.Field(Of String)("shortname"), _
       tier = row.Field(Of String)("tier"), _
       natiom = row.Field(Of String)("nation")
       Order By tier Descending

        'Dim a = q(0).un.Split(":")
        If q(0) IsNot Nothing Then
            Return q(0).un
        End If
        Return ""
    End Function

    Private Function get_tank_icon(ByVal name As String) As Bitmap
        For Each entry In gui_pkg
            If entry.FileName.Contains(name) _
            And Not entry.FileName.Contains("small") _
            And Not entry.FileName.Contains("contour") _
            And Not entry.FileName.Contains("unique") _
            And Not entry.FileName.Contains("library") _
                Then
                Dim ms As New MemoryStream
                entry.Extract(ms)
                If ms IsNot Nothing Then
                    'GC.Collect()
                    current_png_path = entry.FileName
                    Return get_png(ms).Clone
                    'bmp.Tag = entry.FileName
                    'Return bmp
                End If
            End If
        Next
        Return Nothing
    End Function

    Dim delay As Integer = 0
    Dim stepper As Integer = 0
    '###########################################################################################################################################
    Public Sub draw_scene()
        Application.DoEvents()
        If gl_stop Then Return
        gl_busy = True
        'End If
        If Not (Wgl.wglMakeCurrent(pb1_hDC, pb1_hRC)) Then
            MessageBox.Show("Unable to make rendering context current")
            End
        End If
        Dim color_top() As Byte = {20, 20, 20}
        Dim color_bottom() As Byte = {60, 60, 60}
        Dim position() As Single = {10, 10.0F, 10, 1.0F}
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
        Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE)
        Dim er = Gl.glGetError
        Gl.glDepthFunc(Gl.GL_LEQUAL)
        Gl.glFrontFace(Gl.GL_CW)
        'Gl.glCullFace(Gl.GL_BACK)
        Gl.glLineWidth(1)
        Gl.glClearColor(0.0F, 0.0F, 0.0F, 1.0F)

        'Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
        Gl.glDisable(Gl.GL_BLEND)

        Gl.glEnable(Gl.GL_LIGHTING)
        Gl.glEnable(Gl.GL_CULL_FACE)

        Gl.glEnable(Gl.GL_SMOOTH)


        ResizeGL()
        Dim v As Point = pb1.Size
        Gl.glDisable(Gl.GL_DEPTH_TEST)
        Gl.glClearColor(0.0F, 0.0F, 0.2353F, 1.0F)
        Gl.glClear(Gl.GL_DEPTH_BUFFER_BIT)

        ViewOrtho()
        Gl.glDisable(Gl.GL_LIGHTING)
        Gl.glDisable(Gl.GL_DEPTH_TEST)

        Gl.glBegin(Gl.GL_QUADS)
        Dim aspect = v.Y / v.X
        Gl.glColor3ubv(color_top)
        Gl.glVertex3f(0.0, -v.Y, 0)

        Gl.glColor3ubv(color_bottom)
        Gl.glVertex3f(0.0, 0.0, 0)
        Gl.glVertex3f(v.X, 0.0, 0)

        Gl.glColor3ubv(color_top)
        Gl.glVertex3f(v.X, -v.Y, 0)
        Gl.glEnd()

        Gl.glFrontFace(Gl.GL_CCW)

        Gl.glDepthFunc(Gl.GL_LEQUAL)
        Dim drawme As Boolean = True

        Gl.glEnable(Gl.GL_NORMALIZE)


        Gl.glEnable(Gl.GL_LIGHTING)

        ViewPerspective()
        set_eyes()
        Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, position0)
        Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_POSITION, position1)
        Gl.glLightfv(Gl.GL_LIGHT2, Gl.GL_POSITION, position2)

        Gl.glPushMatrix()
        Gl.glDisable(Gl.GL_CULL_FACE)
        '-----------------------------------------------------------------------------
        'light positions
        If Show_lights Then
            '0
            Gl.glPushMatrix()
            Gl.glTranslatef(position0(0), position0(1), position0(2))
            Gl.glColor3f(1.0, 0.0, 0.0) 'red
            glutSolidSphere(0.2, 10, 10)
            Gl.glPopMatrix()
            '1
            Gl.glPushMatrix()
            Gl.glTranslatef(position1(0), position1(1), position1(2))
            Gl.glColor3f(0.0, 1.0, 0.0) 'green
            glutSolidSphere(0.2, 10, 10)
            Gl.glPopMatrix()
            '2
            Gl.glPushMatrix()
            Gl.glTranslatef(position2(0), position2(1), position2(2))
            Gl.glColor3f(0.0, 0.0, 1.0) 'blue
            glutSolidSphere(0.2, 10, 10)
            Gl.glPopMatrix()
        End If
        '-----------------------------------------------------------------------------

        Gl.glEnable(Gl.GL_DEPTH_TEST)
        Gl.glColor3f(0.25, 0.25, 0.25)
        If model_loaded And Not m_load_textures.Checked Then
            For jj = 1 To object_count
                If _object(jj).visible Then
                    Gl.glCallList(_object(jj).main_display_list)
                End If
            Next
        End If
        Dim id As Integer = SELECTED_CAMO_BUTTON
        If model_loaded And m_load_textures.Checked Then
            Gl.glUseProgram(shader_list.tank_shader)
            Gl.glUniform1i(tank_colorMap, 0)
            Gl.glUniform1i(tank_normalMap, 1)
            Gl.glUniform1i(tank_GMM, 2)
            Gl.glUniform1i(tank_AO, 3)
            Gl.glUniform1i(tank_detailMap, 4)
            Gl.glUniform1i(tank_camo, 5)
            For jj = 1 To object_count - track_info.segment_count
                Gl.glUniform1i(tank_is_GAmap, _object(jj).ANM)
                Gl.glUniform1i(tank_alphaTest, _group(jj).alphaTest)
                Gl.glUniform2f(tank_detailTiling, _group(jj).detail_tile.x, _group(jj).detail_tile.y)
                Gl.glUniform1f(tank_detailPower, _group(jj).detail_power)
                Gl.glUniform4f(tank_tile_vec4, _object(jj).camo_tiling.x, _object(jj).camo_tiling.y, _object(jj).camo_tiling.z, _object(jj).camo_tiling.w)
                Gl.glUniform1i(tank_use_camo, _object(jj).use_camo)
                Gl.glUniform1i(tank_exclude_camo, _object(jj).exclude_camo)
                If _object(jj).use_camo > 0 Then

                    Gl.glUniform4f(tank_c0, c0(id).x, c0(id).y, c0(id).z, c0(id).w)
                    Gl.glUniform4f(tank_c1, c1(id).x, c1(id).y, c1(id).z, c1(id).w)
                    Gl.glUniform4f(tank_c2, c2(id).x, c2(id).y, c2(id).z, c2(id).w)
                    Gl.glUniform4f(tank_c3, c3(id).x, c3(id).y, c3(id).z, c3(id).w)
                    Gl.glUniform4f(tank_camo_tiling, bb_tank_tiling(id).x, bb_tank_tiling(id).y, bb_tank_tiling(id).z, bb_tank_tiling(id).w)
                    Gl.glUniform4f(tank_armorcolor, ARMORCOLOR.x, ARMORCOLOR.y, ARMORCOLOR.z, 1.0)
                End If


                If _object(jj).visible Then
                    Gl.glActiveTexture(Gl.GL_TEXTURE0)
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).color_Id)
                    Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).normal_Id)
                    Gl.glActiveTexture(Gl.GL_TEXTURE0 + 2)
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).metalGMM_Id)
                    Gl.glActiveTexture(Gl.GL_TEXTURE0 + 3)
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).ao_id)
                    Gl.glActiveTexture(Gl.GL_TEXTURE0 + 4)
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).detail_Id)
                    Gl.glActiveTexture(Gl.GL_TEXTURE0 + 5)
                    If _object(jj).use_camo > 0 Then
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, bb_texture_ids(id))
                    End If

                    Gl.glCallList(_object(jj).main_display_list)
                End If
            Next
            Gl.glUseProgram(0)

            'clear texture bindings
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '4
            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 3)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '3
            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 2)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '2
            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '1
            Gl.glActiveTexture(Gl.GL_TEXTURE0)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '0
        End If

        If normal_shader_mode > 0 Then
            Gl.glUseProgram(shader_list.normal_shader)
            Gl.glUniform1i(normal_shader_mode_id, normal_shader_mode)
            If model_loaded Then
                For jj = 1 To object_count
                    If _object(jj).visible Then
                        Gl.glCallList(_object(jj).main_display_list)
                    End If
                Next
            End If
            Gl.glUseProgram(0)


        End If
        '==========================================
        'track nurb points
        If model_loaded And TESTING Then
            If object_count > 6 Then
                Gl.glColor3f(0.9, 0.9, 0.9)
                catmullrom.draw_spline()
                delay += 1
                If delay > 5 Then
                    stepper += 1
                    delay = 0
                    If stepper > tracks.Length - 2 Then
                        stepper = 0
                    End If
                End If
                stepper = tracks.Length - 1
                For i = 0 To stepper 'tracks.Length - 1
                    Gl.glColor3f(0.9, 0.0, 0.0)
                    'If tracks(i).name.Contains("Track_R") Then
                    '    Gl.glColor3f(0.9, 0.0, 0.0)

                    'End If
                    Gl.glPushMatrix()
                    Gl.glTranslatef(tracks(i).position.X, tracks(i).position.Y, tracks(i).position.Z)
                    'glutSolidSphere(0.04, 10, 10)
                    Gl.glPopMatrix()
                Next
                Gl.glColor3f(0.0, 0.9, 0.9)
                For i = 0 To path_pointer1 - 1
                    Gl.glPushMatrix()
                    Gl.glTranslatef(path_data1(i).pos1.X, path_data1(i).pos1.Y, path_data1(i).pos1.Z)
                    'Glut.glutSolidSphere(0.03, 10, 10)
                    Gl.glPopMatrix()
                    'Gl.glBegin(Gl.GL_LINES)
                    'Gl.glVertex3f(path_data1(i).pos1.X + 0.5, path_data1(i).pos1.Y, path_data1(i).pos1.Z)
                    'Gl.glVertex3f(-path_data1(i).pos1.X - 0.5, path_data1(i).pos1.Y, path_data1(i).pos1.Z)
                    'Gl.glEnd()
                Next
                Gl.glColor3f(0.5, 0.5, 0.5)

                Dim jj = object_count
                If track_info.segment_count = 2 Then
                    jj -= 1
                End If
                Gl.glUseProgram(shader_list.tank_shader)

                Gl.glActiveTexture(Gl.GL_TEXTURE0)
                Gl.glEnable(Gl.GL_TEXTURE_2D)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(object_count).color_Id)
                Gl.glUniform1i(tank_is_GAmap, _object(jj).ANM)
                Gl.glUniform1i(tank_alphaTest, _group(jj).alphaTest)
                Gl.glUniform2f(tank_detailTiling, _group(jj).detail_tile.x, _group(jj).detail_tile.y)
                Gl.glUniform1f(tank_detailPower, _group(jj).detail_power)
                Gl.glUniform4f(tank_tile_vec4, _object(jj).camo_tiling.x, _object(jj).camo_tiling.y, _object(jj).camo_tiling.z, _object(jj).camo_tiling.w)
                Gl.glUniform1i(tank_use_camo, _object(jj).use_camo)
                Gl.glUniform1i(tank_exclude_camo, _object(jj).exclude_camo)

                Gl.glActiveTexture(Gl.GL_TEXTURE0)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).color_Id)
                Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).normal_Id)
                Gl.glActiveTexture(Gl.GL_TEXTURE0 + 2)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).metalGMM_Id)
                Gl.glActiveTexture(Gl.GL_TEXTURE0 + 3)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).ao_id)
                Gl.glActiveTexture(Gl.GL_TEXTURE0 + 4)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).detail_Id)
                Gl.glActiveTexture(Gl.GL_TEXTURE0 + 5)
                For i = 0 To path_pointer1 - 1
                    With path_data1(i)
                        Gl.glPushMatrix()
                        Gl.glTranslatef(.pos1.X, .yc, .zc)
                        'Glut.glutSolidSphere(0.02, 10, 10)
                        Gl.glPopMatrix()

                        Gl.glPushMatrix()
                        Gl.glScalef(-1.0, 1.0, 1.0)
                        Gl.glTranslatef(.pos1.X, .pos1.Y, .pos1.Z)
                        Gl.glRotatef(.angle - 90, 1.0, 0.0, 0.0)
                        Gl.glCallList(_object(jj).main_display_list)
                        Gl.glBegin(Gl.GL_LINES)
                        'Gl.glVertex3f(0.0, 0.0, 0.0)
                        'Gl.glVertex3f(0.0, 0.0, 0.25)
                        'Gl.glVertex3f(.pos1.X, .pos1.Y, .pos1.Z)
                        'Gl.glVertex3f(.pos1.X, .yc, .zc)
                        Gl.glEnd()
                        Gl.glPopMatrix()
                    End With
                Next
                If track_info.segment_count = 2 Then
                    jj += 1
                    For i = 0 To path_pointer2 - 1
                        With path_data2(i)
                            Gl.glPushMatrix()
                            Gl.glTranslatef(.pos1.X, .yc, .zc)
                            'Glut.glutSolidSphere(0.02, 10, 10)
                            Gl.glPopMatrix()

                            Gl.glPushMatrix()
                            Gl.glScalef(-1.0, 1.0, 1.0)
                            Gl.glTranslatef(.pos1.X, .pos1.Y, .pos1.Z)
                            Gl.glRotatef(.angle - 90, 1.0, 0.0, 0.0)
                            Gl.glCallList(_object(jj).main_display_list)
                            Gl.glBegin(Gl.GL_LINES)
                            'Gl.glVertex3f(0.0, 0.0, 0.0)
                            'Gl.glVertex3f(0.0, 0.0, 0.25)
                            'Gl.glVertex3f(.pos1.X, .pos1.Y, .pos1.Z)
                            'Gl.glVertex3f(.pos1.X, .yc, .zc)
                            Gl.glEnd()
                            Gl.glPopMatrix()
                        End With
                    Next

                End If
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
                Gl.glDisable(Gl.GL_TEXTURE_2D)
                Gl.glUseProgram(0)
                'clear texture bindings
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '4
                Gl.glActiveTexture(Gl.GL_TEXTURE0 + 3)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '3
                Gl.glActiveTexture(Gl.GL_TEXTURE0 + 2)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '2
                Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '1
                Gl.glActiveTexture(Gl.GL_TEXTURE0)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '0
            End If
        End If
        '==========================================

        Gl.glColor3f(0.3, 0.3, 0.3)
        Gl.glCallList(grid)


        Gl.glDisable(Gl.GL_LIGHTING)
        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)

        If move_mod Or z_move Then    'draw reference lines to eye center
            Gl.glColor3f(1.0, 1.0, 1.0)
            'Gl.glLineStipple(1, &HF00F)
            'Gl.glEnable(Gl.GL_LINE_STIPPLE)
            Gl.glLineWidth(1)
            Gl.glBegin(Gl.GL_LINES)
            Gl.glVertex3f(U_look_point_x, U_look_point_y + 1000, U_look_point_z)
            Gl.glVertex3f(U_look_point_x, U_look_point_y - 1000, U_look_point_z)

            Gl.glVertex3f(U_look_point_x + 1000, U_look_point_y, U_look_point_z)
            Gl.glVertex3f(U_look_point_x - 1000, U_look_point_y, U_look_point_z)

            Gl.glVertex3f(U_look_point_x, U_look_point_y, U_look_point_z + 1000)
            Gl.glVertex3f(U_look_point_x, U_look_point_y, U_look_point_z - 1000)
            Gl.glEnd()
            'Gl.glLineStipple(1, &HFFFF)
            'Gl.glDisable(Gl.GL_LINE_STIPPLE)
        End If
        Gl.glEnable(Gl.GL_LIGHTING)
        er = Gl.glGetError

        Gl.glPopMatrix()
        ViewOrtho()
        Gl.glDisable(Gl.GL_DEPTH_TEST)
        Gl.glDisable(Gl.GL_LIGHTING)

        'menu
        'draw_menu()

        Dim top As Integer = 20
        If season_Buttons_VISIBLE Then
            top = 177
        End If
        Gl.glEnable(Gl.GL_BLEND)
        Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA)
        Gl.glColor4f(0.3, 0.0, 0.0, 0.6)
        Gl.glBegin(Gl.GL_TRIANGLES)
        Gl.glVertex3f(0.0, -pb1.Height, 0.0)
        Gl.glColor4f(0.3, 0.0, 0.0, 0.4)
        Gl.glVertex3f(0.0, -pb1.Height + top, 0.0)
        Gl.glVertex3f(pb1.Width, -pb1.Height + top, 0.0)

        Gl.glVertex3f(pb1.Width, -pb1.Height + top, 0.0)
        Gl.glColor4f(0.3, 0.0, 0.0, 0.6)
        Gl.glVertex3f(pb1.Width, -pb1.Height, 0.0)
        Gl.glVertex3f(0.0, -pb1.Height, 0.0)
        Gl.glEnd()


        Dim fps As Integer = 1.0 / (screen_totaled_draw_time * 0.001)
        Dim str = " FPS: ( " + fps.ToString + " )"
        'swat.Stop()
        glutPrint(10, 8 - pb1.Height, str.ToString, 0.0, 1.0, 0.0, 1.0)

        Gl.glDisable(Gl.GL_BLEND)

        If season_Buttons_VISIBLE Then
            For i = 0 To season_Buttons.Length - 2
                season_Buttons(i).draw()
            Next
        End If
        If CAMO_BUTTONS_VISIBLE Then
            For i = 0 To camo_Buttons.Length - 2
                camo_Buttons(i).draw()
            Next
        End If

        Gdi.SwapBuffers(pb1_hDC)
        '====================================
        'has to be AFTER the buffer swap
        If Not STOP_BUTTON_SCAN Then

            If season_Buttons_VISIBLE Then
                draw_season_pick_buttons()
                mouse_pick_season_button(m_mouse.x, m_mouse.y)
                'Gdi.SwapBuffers(pb1_hDC)
            End If
            If CAMO_BUTTONS_VISIBLE Then
                draw_pick_camo_buttons()
                mouse_pick_camo_button(m_mouse.x, m_mouse.y)

            End If
        End If
        '====================================
        Gl.glFlush()
        er = Gl.glGetError
        OLD_WINDOW_HEIGHT = pb1.Height
        gl_busy = False
    End Sub
    Public Sub draw_XZ_grid()
        Gl.glDisable(Gl.GL_LIGHTING)
        Gl.glLineWidth(1)
        Gl.glBegin(Gl.GL_LINES)
        Gl.glColor3f(0.3F, 0.3F, 0.3F)
        For z As Single = -100.0F To -1.0F Step 1.0
            Gl.glVertex3f(-100.0F, 0.0F, z)
            Gl.glVertex3f(100.0F, 0.0F, z)
        Next
        For z As Single = 1.0F To 100.0F Step 1.0
            Gl.glVertex3f(-100.0F, 0.0F, z)
            Gl.glVertex3f(100.0F, 0.0F, z)
        Next
        For x As Single = -100.0F To -1.0F Step 1.0
            Gl.glVertex3f(x, 0.0F, 100.0F)
            Gl.glVertex3f(x, 0.0F, -100.0F)
        Next
        For x As Single = 1.0F To 100.0F Step 1.0
            Gl.glVertex3f(x, 0.0F, 100.0F)
            Gl.glVertex3f(x, 0.0F, -100.0F)
        Next
        Gl.glEnd()
        Gl.glLineWidth(1)
        Gl.glBegin(Gl.GL_LINES)
        Gl.glColor3f(0.6F, 0.6F, 0.6F)
        Gl.glVertex3f(1.0F, 0.0F, 0.0F)
        Gl.glVertex3f(-1.0F, 0.0F, 0.0F)
        Gl.glVertex3f(0.0F, 0.0F, 1.0F)
        Gl.glVertex3f(0.0F, 0.0F, -1.0F)
        Gl.glEnd()
        'begin axis markers
        ' red is z+
        ' green is x-
        'blue is z-
        ' yellow x+
        Gl.glLineWidth(1)

        Gl.glBegin(Gl.GL_LINES)
        'z+ red
        Gl.glColor3f(1.0F, 0.0F, 0.0F)
        Gl.glVertex3f(0.0F, 0.0F, 1.0F)
        Gl.glVertex3f(0.0F, 0.0F, 100.0F)
        'z- blue
        Gl.glColor3f(0.0F, 0.0F, 1.0F)
        Gl.glVertex3f(0.0F, 0.0F, -1.0F)
        Gl.glVertex3f(0.0F, 0.0F, -100.0F)
        'x+ yellow
        Gl.glColor3f(1.0F, 1.0F, 0.0F)
        Gl.glVertex3f(1.0F, 0.0F, 0.0F)
        Gl.glVertex3f(100.0F, 0.0F, 0.0F)
        'x- green
        Gl.glColor3f(0.0F, 1.0F, 0.0F)
        Gl.glVertex3f(-1.0F, 0.0F, 0.0F)
        Gl.glVertex3f(-100.0F, 0.0F, 0.0F)
        '---------
        Gl.glEnd()

        Gl.glEnable(Gl.GL_LIGHTING)

    End Sub


    Public Sub set_eyes()

        Dim sin_x, sin_y, cos_x, cos_y As Single
        sin_x = Sin(U_Cam_X_angle + angle_offset)
        cos_x = Cos(U_Cam_X_angle + angle_offset)
        cos_y = Cos(U_Cam_Y_angle)
        sin_y = Sin(U_Cam_Y_angle)
        cam_y = Sin(U_Cam_Y_angle) * view_radius
        cam_x = (sin_x - (1 - cos_y) * sin_x) * view_radius
        cam_z = (cos_x - (1 - cos_y) * cos_x) * view_radius

        Glu.gluLookAt(cam_x + U_look_point_x, cam_y + U_look_point_y, cam_z + U_look_point_z, _
                          U_look_point_x, U_look_point_y, U_look_point_z, 0.0F, 1.0F, 0.0F)

        eyeX = cam_x + U_look_point_x
        eyeY = cam_y + U_look_point_y
        eyeZ = cam_z + U_look_point_z

    End Sub

    Private Sub pb1_MouseDown(sender As Object, e As MouseEventArgs) Handles pb1.MouseDown
        'If M_SELECT_COLOR > 0 Then
        '    For i = 0 To button_list.Length - 2
        '        If M_SELECT_COLOR = button_list(i).color Then
        '            CallByName(Menu_Subs, button_list(i).function_, Microsoft.VisualBasic.CallType.Method)
        '        End If
        '    Next
        'End If
        If e.Button = Forms.MouseButtons.Right Then
            'Timer1.Enabled = False
            move_cam_z = True
            mouse.x = e.X
            mouse.y = e.Y
        End If
        If e.Button = Forms.MouseButtons.Left Then
            mouse.x = e.X
            mouse.y = e.Y
            'If BUTTON_ID > 0 Then
            '    If CAMO_BUTTON_DOWN Then Return
            '    gl_stop = True
            '    If BUTTON_TYPE = 1 Then
            '        'season_Buttons(BUTTON_ID - 100).state = 2
            '        season_Buttons(BUTTON_ID - 100).click()
            '        gl_stop = False
            '        Return
            '    End If
            '    If BUTTON_TYPE = 2 Then
            '        'camo_Buttons(BUTTON_ID - 100).state = 2
            '        camo_Buttons(BUTTON_ID - 100).click()
            '        gl_stop = False
            '        Return

            '    End If
            'End If
            CAMO_BUTTON_DOWN = True
            M_DOWN = True
        End If
    End Sub

    Private Sub pb1_MouseMove(sender As Object, e As MouseEventArgs) Handles pb1.MouseMove
        m_mouse.x = e.X
        m_mouse.y = e.Y

        If BUTTON_ID > 0 Then
            If BUTTON_ID > 0 Then
                Return
            End If
        End If

        'If check_menu_select() Then ' check if we are over a button
        '    Return
        'End If
        Dim dead As Integer = 5
        Dim t As Single
        Dim M_Speed As Single = 0.8
        Dim ms As Single = 0.2F * view_radius ' distance away changes speed.. THIS WORKS WELL!
        If M_DOWN Then
            If e.X > (mouse.x + dead) Then
                If e.X - mouse.x > 100 Then t = (1.0F * M_Speed)
            Else : t = CSng(Sin((e.X - mouse.x) / 100)) * M_Speed
                If Not z_move Then
                    If move_mod Then ' check for modifying flag
                        look_point_x -= ((t * ms) * (Cos(Cam_X_angle)))
                        look_point_z -= ((t * ms) * (-Sin(Cam_X_angle)))
                    Else
                        Cam_X_angle -= t
                    End If
                    If Cam_X_angle > (2 * PI) Then Cam_X_angle -= (2 * PI)
                    mouse.x = e.X
                End If
            End If
            If e.X < (mouse.x - dead) Then
                If mouse.x - e.X > 100 Then t = (M_Speed)
            Else : t = CSng(Sin((mouse.x - e.X) / 100)) * M_Speed
                If Not z_move Then
                    If move_mod Then ' check for modifying flag
                        look_point_x += ((t * ms) * (Cos(Cam_X_angle)))
                        look_point_z += ((t * ms) * (-Sin(Cam_X_angle)))
                    Else
                        Cam_X_angle += t
                    End If
                    If Cam_X_angle < 0 Then Cam_X_angle += (2 * PI)
                    mouse.x = e.X
                End If
            End If
            ' ------- Y moves ----------------------------------
            If e.Y > (mouse.y + dead) Then
                If e.Y - mouse.y > 100 Then t = (M_Speed)
            Else : t = CSng(Sin((e.Y - mouse.y) / 100)) * M_Speed
                If z_move Then
                    look_point_y -= (t * ms)
                Else
                    If move_mod Then ' check for modifying flag
                        look_point_z -= ((t * ms) * (Cos(Cam_X_angle)))
                        look_point_x -= ((t * ms) * (Sin(Cam_X_angle)))
                    Else
                        Cam_Y_angle -= t
                    End If
                    If Cam_Y_angle < -PI Then Cam_Y_angle = -PI
                End If
                mouse.y = e.Y
            End If
            If e.Y < (mouse.y - dead) Then
                If mouse.y - e.Y > 100 Then t = (M_Speed)
            Else : t = CSng(Sin((mouse.y - e.Y) / 100)) * M_Speed
                If z_move Then
                    look_point_y += (t * ms)
                Else
                    If move_mod Then ' check for modifying flag
                        look_point_z += ((t * ms) * (Cos(Cam_X_angle)))
                        look_point_x += ((t * ms) * (Sin(Cam_X_angle)))
                    Else
                        Cam_Y_angle += t
                    End If
                    If Cam_Y_angle > 1.3 Then Cam_Y_angle = 1.3
                End If
                mouse.y = e.Y
            End If
            'draw_scene()
            Return
        End If
        If move_cam_z Then
            If e.Y > (mouse.y + dead) Then
                If e.Y - mouse.y > 100 Then t = (10)
            Else : t = CSng(Sin((e.Y - mouse.y) / 100)) * 12
                view_radius += (t * (view_radius * 0.2))    ' zoom is factored in to Cam radius
                mouse.y = e.Y
            End If
            If e.Y < (mouse.y - dead) Then
                If mouse.y - e.Y > 100 Then t = (10)
            Else : t = CSng(Sin((mouse.y - e.Y) / 100)) * 12
                view_radius -= (t * (view_radius * 0.2))    ' zoom is factored in to Cam radius
                If view_radius > -1.0 Then view_radius = -1.0
                mouse.y = e.Y
            End If
            If view_radius > -0.5 Then view_radius = -0.5
            'draw_scene()
            Return
        End If
        mouse.x = e.X
        mouse.y = e.Y
        'GetOGLPos(e.X, e.Y)
        'draw_scene()
    End Sub

    Private Sub pb1_MouseUp(sender As Object, e As MouseEventArgs) Handles pb1.MouseUp
        M_DOWN = False
        CAMO_BUTTON_DOWN = False
        move_cam_z = False
    End Sub

#Region "screen refresh"
    Public Function need_update() As Boolean
        'This updates the display if the mouse has changed the view angles, locations or distance.
        Dim update As Boolean = False

        If look_point_x <> U_look_point_x Then
            U_look_point_x = look_point_x
            update = True
        End If
        If look_point_y <> U_look_point_y Then
            U_look_point_y = look_point_y
            update = True
        End If
        If look_point_z <> U_look_point_z Then
            U_look_point_z = look_point_z
            update = True
        End If
        If Cam_X_angle <> U_Cam_X_angle Then
            U_Cam_X_angle = Cam_X_angle
            update = True
        End If
        If Cam_Y_angle <> U_Cam_Y_angle Then
            U_Cam_Y_angle = Cam_Y_angle
            update = True
        End If
        If view_radius <> u_View_Radius Then
            u_View_Radius = view_radius
            update = True
        End If

        Return update
    End Function
    Dim spin_light As Boolean = False
    Public Sub update_mouse()
        Dim l_rot As Single
        Dim sun_angle As Single = 0
        Dim sun_radius As Single = 5
        'This will run for the duration that Terra! is open.
        'Its in a closed loop
        screen_totaled_draw_time = 10.0
        Dim swat As New Stopwatch
        While _Started
            angle_offset = 0
            If need_update() Then
                'If we need to update the screen, lets calculate draw times.
            End If
            '	Application.DoEvents()
            If Not gl_busy And Not Me.WindowState = FormWindowState.Minimized Then

                If spin_light Then
                    Dim x, z As Single
                    l_rot += 0.01
                    If l_rot > 2 * PI Then
                        l_rot -= (2 * PI)
                    End If
                    If sun_radius > 0 Then
                        'sun_radius *= -1.0
                    End If
                    Dim s As Single = 2.0
                    sun_angle = l_rot
                    x = Cos(l_rot) * (sun_radius * s)
                    z = Sin(l_rot) * (sun_radius * s)
                    position0(0) = x
                    ' position0(1) = sun_radius * s * 0.75
                    position0(1) = 2.5

                    position0(2) = z

                End If


                If Not w_changing Then
                    update_screen()
                End If
                screen_draw_time = CInt(swat.ElapsedMilliseconds)
                Dim freq = Stopwatch.Frequency
                'screen_draw_time = screen_draw_time / freq
                'screen_draw_time *= 0.001
                swat.Reset()
                swat.Start()
                If screen_avg_counter > 15 Then
                    screen_totaled_draw_time = screen_avg_draw_time / screen_avg_counter
                    screen_avg_counter = 0
                    screen_avg_draw_time = 0
                Else
                    If screen_draw_time < 1 Then
                        'screen_draw_time = 5
                    End If
                    screen_avg_counter += 1
                    screen_avg_draw_time += screen_draw_time
                End If
            End If

            Thread.Sleep(10)
        End While
        'Thread.CurrentThread.Abort()
    End Sub
    Private Delegate Sub update_screen_delegate()
    Private Sub update_screen()
        Try
            If Me.InvokeRequired Then
                Me.Invoke(New update_screen_delegate(AddressOf update_screen))
            Else
                draw_scene()
            End If
        Catch ex As Exception

        End Try
    End Sub
    Private Sub Startup_Timer_Tick(sender As Object, e As EventArgs) Handles Startup_Timer.Tick
        Startup_Timer.Enabled = False
        update_thread.IsBackground = True
        update_thread.Name = "mouse updater"
        update_thread.Priority = ThreadPriority.Normal
        update_thread.Start()
    End Sub
#End Region

#Region "menu_button_functions"
    Private Sub m_load_Click(sender As Object, e As EventArgs) Handles m_load.Click
        process_tank(False) 'false .. don't save the binary tank file
    End Sub

    Private Sub m_clear_temp_folder_data_Click(sender As Object, e As EventArgs) Handles m_clear_temp_folder_data.Click
        clear_temp_folder()
    End Sub

    Private Sub m_reload_api_data_Click(sender As Object, e As EventArgs) Handles m_reload_api_data.Click
        get_tank_names()
    End Sub

    Private Sub m_export_tank_list_Click(sender As Object, e As EventArgs) Handles m_export_tank_list.Click
        If MsgBox("This can take a while and" + vbCrLf + _
                  "will delete all previous tank files!" + vbCrLf + _
                  "Are you sure?", MsgBoxStyle.YesNo, "Warning!") = MsgBoxResult.No Then
            Return
        End If

        If tanklist.Text = "" Then
            Return
        End If
        Dim f As DirectoryInfo = New DirectoryInfo(Application.StartupPath + "\tanks\")
        If f.Exists Then
            For Each fi In f.GetFiles
                If Not fi.Name.Contains(".txt") Then
                    fi.Delete()
                End If
            Next
        End If

        Dim tank As String = ""
        Dim ta = tanklist.Text.Split(vbCr)
        For i = 0 To ta.Length - 2
            tank = ta(i)
            tank = tank.Replace(vbLf, "")
            file_name = ""
            '1
            For Each n As TreeNode In TreeView1.Nodes
                If n.Text = tank Then
                    file_name = n.Tag
                    GoTo make_this_tank
                End If
            Next

            '2
            For Each n As TreeNode In TreeView2.Nodes
                If n.Text = tank Then
                    file_name = n.Tag
                    GoTo make_this_tank
                End If
            Next

            '3
            For Each n As TreeNode In TreeView3.Nodes
                If n.Text = tank Then
                    file_name = n.Tag
                    GoTo make_this_tank
                End If
            Next

            '4
            For Each n As TreeNode In TreeView4.Nodes
                If n.Text = tank Then
                    file_name = n.Tag
                    GoTo make_this_tank
                End If
            Next

            '5
            For Each n As TreeNode In TreeView5.Nodes
                If n.Text = tank Then
                    file_name = n.Tag
                    GoTo make_this_tank
                End If
            Next

            '6
            For Each n As TreeNode In TreeView6.Nodes
                If n.Text = tank Then
                    file_name = n.Tag
                    GoTo make_this_tank
                End If
            Next

            '7
            For Each n As TreeNode In TreeView7.Nodes
                If n.Text = tank Then
                    file_name = n.Tag
                    GoTo make_this_tank
                End If
            Next

            '8
            For Each n As TreeNode In TreeView8.Nodes
                If n.Text = tank Then
                    file_name = n.Tag
                    GoTo make_this_tank
                End If
            Next

            '9
            For Each n As TreeNode In TreeView9.Nodes
                If n.Text = tank Then
                    file_name = n.Tag
                    GoTo make_this_tank
                End If
            Next

            '10
            For Each n As TreeNode In TreeView10.Nodes
                If n.Text = tank Then
                    file_name = n.Tag
                    GoTo make_this_tank
                End If
            Next

make_this_tank:
            If file_name = "" Then
                log_text.AppendLine("Tank not found:" + tank)
            Else
                process_tank(True) ' true means save the binary tank file
                model_loaded = False
            End If
        Next
        model_loaded = True


    End Sub

    Private Sub m_clear_selected_tanks_Click(sender As Object, e As EventArgs) Handles m_clear_selected_tanks.Click
        If MsgBox("Are you sure?" + vbCrLf + "This can NOT be undone!", MsgBoxStyle.YesNo, "Warning!") = MsgBoxResult.No Then
            Return
        End If
        tanklist.Text = ""
        For Each n As TreeNode In TreeView1.Nodes
            clear_node_selection(n)
        Next
        '2
        For Each n As TreeNode In TreeView2.Nodes
            clear_node_selection(n)
        Next

        '3
        For Each n As TreeNode In TreeView3.Nodes
            clear_node_selection(n)
        Next

        '4
        For Each n As TreeNode In TreeView4.Nodes
            clear_node_selection(n)
        Next

        '5
        For Each n As TreeNode In TreeView5.Nodes
            clear_node_selection(n)
        Next

        '6
        For Each n As TreeNode In TreeView6.Nodes
            clear_node_selection(n)
        Next

        '7
        For Each n As TreeNode In TreeView7.Nodes
            clear_node_selection(n)
        Next

        '8
        For Each n As TreeNode In TreeView8.Nodes
            clear_node_selection(n)
        Next

        '9
        For Each n As TreeNode In TreeView9.Nodes
            clear_node_selection(n)
        Next

        '10
        For Each n As TreeNode In TreeView10.Nodes
            clear_node_selection(n)
        Next

    End Sub

    Private Sub m_load_file_Click(sender As Object, e As EventArgs) Handles m_load_file.Click
        out_string.Length = 0
        Dim in_s = IO.File.ReadAllText(Application.StartupPath + "\tanks\tanknames.txt")
        If in_s = "" Then
            Return
        End If
        tanklist.Text = ""
        file_name = ""
        Dim tank As String = ""
        '1
        For Each n As TreeNode In TreeView1.Nodes
            If in_s.Contains(n.Text + ":") Then
                tanklist.Text += (n.Text) + vbCrLf
                n.ForeColor = Color.White
            Else
                n.ForeColor = Color.Black
            End If
        Next

        '2
        For Each n As TreeNode In TreeView2.Nodes
            If in_s.Contains(n.Text + ":") Then
                tanklist.Text += (n.Text) + vbCrLf
                n.ForeColor = Color.White
            Else
                n.ForeColor = Color.Black
            End If
        Next

        '3
        For Each n As TreeNode In TreeView3.Nodes
            If in_s.Contains(n.Text + ":") Then
                tanklist.Text += (n.Text) + vbCrLf
                n.ForeColor = Color.White
            Else
                n.ForeColor = Color.Black
            End If
        Next

        '4
        For Each n As TreeNode In TreeView4.Nodes
            If in_s.Contains(n.Text + ":") Then
                tanklist.Text += (n.Text) + vbCrLf
                n.ForeColor = Color.White
            Else
                n.ForeColor = Color.Black
            End If
        Next

        '5
        For Each n As TreeNode In TreeView5.Nodes
            If in_s.Contains(n.Text + ":") Then
                tanklist.Text += (n.Text) + vbCrLf
                n.ForeColor = Color.White
            Else
                n.ForeColor = Color.Black
            End If
        Next

        '6
        For Each n As TreeNode In TreeView6.Nodes
            If in_s.Contains(n.Text + ":") Then
                tanklist.Text += (n.Text) + vbCrLf
                n.ForeColor = Color.White
            Else
                n.ForeColor = Color.Black
            End If
        Next

        '7
        For Each n As TreeNode In TreeView7.Nodes
            If in_s.Contains(n.Text + ":") Then
                tanklist.Text += (n.Text) + vbCrLf
                n.ForeColor = Color.White
            Else
                n.ForeColor = Color.Black
            End If
        Next

        '8
        For Each n As TreeNode In TreeView8.Nodes
            If in_s.Contains(n.Text + ":") Then
                tanklist.Text += (n.Text) + vbCrLf
                n.ForeColor = Color.White
            Else
                n.ForeColor = Color.Black
            End If
        Next

        '9
        For Each n As TreeNode In TreeView9.Nodes
            If in_s.Contains(n.Text + ":") Then
                tanklist.Text += (n.Text) + vbCrLf
                n.ForeColor = Color.White
            Else
                n.ForeColor = Color.Black
            End If
        Next

        '10
        For Each n As TreeNode In TreeView10.Nodes
            If in_s.Contains(n.Text + ":") Then
                tanklist.Text += (n.Text) + vbCrLf
                n.ForeColor = Color.White
            Else
                n.ForeColor = Color.Black
            End If
        Next

        Application.DoEvents()
    End Sub

    Private Sub m_save_Click(sender As Object, e As EventArgs) Handles m_save.Click
        out_string.Length = 0
        If tanklist.Text = "" Then
            Return
        End If
        Dim tank As String = ""
        file_name = ""
        Dim ta = tanklist.Text.Split(vbCr)

        For i = 0 To ta.Length - 2
            tank = ta(i)
            tank = tank.Replace(vbLf, "")
            '1
            For Each n As TreeNode In TreeView1.Nodes
                If n.Text = tank Then
                    get_tank_xml_data(n)
                    out_string.Append(icons(1).img(n.Index).Tag.ToString + vbCrLf)
                End If
            Next

            '2
            For Each n As TreeNode In TreeView2.Nodes
                If n.Text = tank Then
                    get_tank_xml_data(n)
                    out_string.Append(icons(2).img(n.Index).Tag.ToString + vbCrLf)
                End If
            Next

            '3
            For Each n As TreeNode In TreeView3.Nodes
                If n.Text = tank Then
                    get_tank_xml_data(n)
                    out_string.Append(icons(3).img(n.Index).Tag.ToString + vbCrLf)
                End If
            Next

            '4
            For Each n As TreeNode In TreeView4.Nodes
                If n.Text = tank Then
                    get_tank_xml_data(n)
                    out_string.Append(icons(4).img(n.Index).Tag.ToString + vbCrLf)
                End If
            Next

            '5
            For Each n As TreeNode In TreeView5.Nodes
                If n.Text = tank Then
                    get_tank_xml_data(n)
                    out_string.Append(icons(5).img(n.Index).Tag.ToString + vbCrLf)
                End If
            Next

            '6
            For Each n As TreeNode In TreeView6.Nodes
                If n.Text = tank Then
                    get_tank_xml_data(n)
                    out_string.Append(icons(6).img(n.Index).Tag.ToString + vbCrLf)
                End If
            Next

            '7
            For Each n As TreeNode In TreeView7.Nodes
                If n.Text = tank Then
                    get_tank_xml_data(n)
                    out_string.Append(icons(7).img(n.Index).Tag.ToString + vbCrLf)
                End If
            Next

            '8
            For Each n As TreeNode In TreeView8.Nodes
                If n.Text = tank Then
                    get_tank_xml_data(n)
                    out_string.Append(icons(8).img(n.Index).Tag.ToString + vbCrLf)
                End If
            Next

            '9
            For Each n As TreeNode In TreeView9.Nodes
                If n.Text = tank Then
                    get_tank_xml_data(n)
                    out_string.Append(icons(9).img(n.Index).Tag.ToString + vbCrLf)
                End If
            Next

            '10
            For Each n As TreeNode In TreeView10.Nodes
                If n.Text = tank Then
                    get_tank_xml_data(n)
                    out_string.Append(icons(10).img(n.Index).Tag.ToString + vbCrLf)
                End If
            Next

        Next
        File.WriteAllText(Application.StartupPath + "\tanks\tanknames.txt", out_string.ToString)
    End Sub

    Private Sub m_chassis_CheckedChanged(sender As Object, e As EventArgs) Handles m_chassis.CheckedChanged
        If Not model_loaded Then Return
        If m_chassis.Checked Then
            m_chassis.ForeColor = Color.DarkGreen
        Else
            m_chassis.ForeColor = Color.Black
        End If
        For i = 1 To object_count
            If _object(i).name.ToLower.Contains("chassis") Then
                If i < 3 Then
                    _object(i).visible = m_chassis.Checked
                End If
            End If
        Next
    End Sub

    Private Sub m_hull_CheckedChanged(sender As Object, e As EventArgs) Handles m_hull.CheckedChanged
        If Not model_loaded Then Return
        If m_hull.Checked Then
            m_hull.ForeColor = Color.DarkGreen
        Else
            m_hull.ForeColor = Color.Black
        End If
        For i = 1 To object_count
            If _object(i).name.ToLower.Contains("hull_") Then
                _object(i).visible = m_hull.Checked
            End If
        Next

    End Sub

    Private Sub m_turret_CheckedChanged(sender As Object, e As EventArgs) Handles m_turret.CheckedChanged
        If Not model_loaded Then Return
        If m_turret.Checked Then
            m_turret.ForeColor = Color.DarkGreen
        Else
            m_turret.ForeColor = Color.Black
        End If
        For i = 1 To object_count
            If _object(i).name.ToLower.Contains("turret_") Then
                _object(i).visible = m_turret.Checked
            End If
        Next

    End Sub

    Private Sub m_gun_CheckedChanged(sender As Object, e As EventArgs) Handles m_gun.CheckedChanged
        If Not model_loaded Then Return
        If m_gun.Checked Then
            m_gun.ForeColor = Color.DarkGreen
        Else
            m_gun.ForeColor = Color.Black
        End If
        For i = 1 To object_count
            If _object(i).name.ToLower.Contains("gun_") Then
                _object(i).visible = m_gun.Checked
            End If
        Next

    End Sub

    Private Sub m_open_temp_folder_Click(sender As Object, e As EventArgs) Handles m_open_temp_folder.Click
        Dim f As DirectoryInfo = New DirectoryInfo(Temp_Storage)
        If f.Exists Then
            Process.Start("explorer.exe", Application.StartupPath + "\tanks\")
        End If

    End Sub

    Private Sub m_export_fbx_Click(sender As Object, e As EventArgs) Handles m_export_fbx.Click

        SaveFileDialog1.Filter = "AutoDesk (*.FBX)|*.fbx"
        SaveFileDialog1.Title = "Export FBX..."
        SaveFileDialog1.InitialDirectory = My.Settings.fbx_path
        SaveFileDialog1.FileName = tank_label.Text.Replace("\/", "_") + ".fbx"

        If SaveFileDialog1.ShowDialog = Forms.DialogResult.OK Then
            My.Settings.fbx_path = SaveFileDialog1.FileName
        Else
            Return
        End If
        ReDim textures(0)
        frmFBX.ShowDialog(Me)


    End Sub

    Private Sub m_load_textures_CheckedChanged(sender As Object, e As EventArgs) Handles m_load_textures.CheckedChanged
        If m_load_textures.Checked Then
            m_load_textures.ForeColor = Color.Red
        Else
            m_load_textures.ForeColor = Color.Black
        End If
    End Sub

    Private Sub m_show_log_Click(sender As Object, e As EventArgs) Handles m_show_log.Click
        Dim t As String = Temp_Storage + "\log_text.txt"
        File.WriteAllText(t, log_text.ToString)

        System.Diagnostics.Process.Start("notepad.exe", t)
    End Sub

    Private Sub m_res_mods_path_Click(sender As Object, e As EventArgs) Handles m_res_mods_path.Click
        If FolderBrowserDialog1.ShowDialog = Forms.DialogResult.OK Then
            My.Settings.res_mods_path = FolderBrowserDialog1.SelectedPath
            If Not File.Exists(My.Settings.res_mods_path) Then
                MsgBox("Incorrect Path.", MsgBoxStyle.Information)
                m_res_mods_path.PerformClick()
                Return
            End If
            res_mods_path_set = True
            My.Settings.res_mods_path = FolderBrowserDialog1.SelectedPath
            Return
        End If
    End Sub

    Private Sub M_Path_Click(sender As Object, e As EventArgs) Handles M_Path.Click
        If FolderBrowserDialog1.ShowDialog = Forms.DialogResult.OK Then
            My.Settings.game_path = FolderBrowserDialog1.SelectedPath
            If Not File.Exists(My.Settings.game_path + "\paths.xml") Then
                MsgBox("Incorrect Path.", MsgBoxStyle.Information)
                M_Path.PerformClick()
                Return
            End If
            path_set = True
            My.Settings.game_path = FolderBrowserDialog1.SelectedPath
            Return
        End If
    End Sub

    Private Sub m_pick_camo_Click(sender As Object, e As EventArgs) Handles m_pick_camo.Click
        setup_camo_selection()
    End Sub

    Private Sub m_edit_shaders_Click(sender As Object, e As EventArgs) Handles m_edit_shaders.Click
        frmEditFrag.Show()
    End Sub

    Private Sub M_Exit_Click(sender As Object, e As EventArgs) Handles M_Exit.Click
        Me.Close()
    End Sub

#End Region

    Public Sub process_tank(ByVal save_tank As Boolean)
        If Not bb_texture_list(0) = "" Then
            For i = 0 To bb_texture_list.Length - 1
                Gl.glDeleteTextures(1, bb_texture_ids(i))
                Gl.glFinish()
                Gl.glDeleteTextures(1, bb_camo_texture_ids(i))
                Gl.glFinish()
            Next
        End If
        model_loaded = False
        '===================================
        m_pick_camo.Enabled = False
        LAST_SEASON = 10
        season_Buttons_VISIBLE = False
        CAMO_BUTTONS_VISIBLE = False
        ReDim textures(0)

        For i = 1 To 100
            Gl.glDeleteTextures(1, delete_image_start + 1) 'start one past camo ids
            Gl.glFinish()
        Next
        Dim ar = file_name.Split(":")
        file_name = ar(2)
        Dim ts = ar(1)
        ar = ts.Split("\")
        For i = 0 To ar.Length - 1
            If ar(i).ToLower.Contains("level_") Then
                ts = ar(i)
                Exit For
            End If
        Next
        ar = ts.Split("_")
        ts = ar(2)
        ar = ts.Split(".")
        Dim fd As String = "lod0"
        Try
            current_tank_package = CInt(ar(0))

        Catch ex As Exception
            Try
                If ts.ToLower.Contains("\shared") Then
                    current_tank_package = 11
                End If
                If ts.ToLower.Contains("\shared_sandbox") Then
                    current_tank_package = 11
                End If
            Catch eex As Exception
                MsgBox("Unable to find package file!", MsgBoxStyle.Exclamation, "Well shit...")
                Return
            End Try

        End Try
        ar = file_name.Split("/")
        Dim xml_file = ar(0) + "\" + ar(1) + "\" + ar(2) + ".xml"
        Dim t As New DataSet
        get_tank_parts_from_xml(xml_file, t)
        If t.Tables.Count = 0 Then
            Return
        End If
        TANK_NAME = ar(2)
        '-----------------------------------
        'Return
        'get take part paths from table
        Dim turrets(10) As String
        Dim guns(10) As String
        Dim hulls(10) As String
        Dim chassis(10) As String
        ReDim hull_tile(10)
        ReDim gun_tile(10)
        ReDim turret_tile(GLUT_BITMAP_HELVETICA_10)
        Dim cnt As Integer = 0

        Dim tbl = t.Tables("gun")
        Dim q = From row In tbl.AsEnumerable _
                Select _
                g_name = row.Field(Of String)("gun_name"), _
                model = row.Field(Of String)("model"), _
                tile = row.Field(Of String)("gun_camouflage")
        cnt = 0
        '----- guns
        For Each thing In q
            Dim gn = thing.model
            guns(cnt) = gn
            gun_tile(cnt) = New vect4
            If thing.tile IsNot Nothing Then

                Dim n = thing.tile.Split(" ")
                gun_tile(cnt).x = CSng(n(0))
                gun_tile(cnt).y = CSng(n(1))
                gun_tile(cnt).z = CSng(n(2))
                gun_tile(cnt).w = CSng(n(3))
                cnt += 1
            Else
                gun_tile(cnt).x = 1.0
                gun_tile(cnt).y = 1.0
                gun_tile(cnt).z = 0.0
                gun_tile(cnt).w = 0.0
                cnt += 1

            End If
        Next
        If cnt = 0 Then
            bad_tanks.AppendLine(file_name)
            Return
        End If
        ReDim Preserve guns(cnt)
        ReDim Preserve gun_tile(cnt)
        cnt = 0
        '----------------------------------
        '----- turret tiling
        Try
            tbl = t.Tables("turret_tiling")

            Dim q25 = From row In tbl.AsEnumerable _
                      Select _
                      tile = row.Field(Of String)("tiling")

            For Each thing In q25
                Dim n = thing.Split(" ")
                turret_tile(cnt).x = CSng(n(0))
                turret_tile(cnt).y = CSng(n(1))
                turret_tile(cnt).z = CSng(n(2))
                turret_tile(cnt).w = CSng(n(3))
                cnt += 1
            Next
            ReDim Preserve turret_tile(cnt)
            cnt = 0
        Catch ex As Exception
            tbl = t.Tables("tiling")

            Dim q25 = From row In tbl.AsEnumerable _
                      Select _
                      tile = row.Field(Of String)("tiling_Text")

            For Each thing In q25
                Dim n = thing.Split(" ")
                turret_tile(cnt).x = CSng(n(0))
                turret_tile(cnt).y = CSng(n(1))
                turret_tile(cnt).z = CSng(n(2))
                turret_tile(cnt).w = CSng(n(3))
                cnt += 1
            Next
            ReDim Preserve turret_tile(cnt)
            cnt = 0

        End Try

        '----------------------------------
        '----- turrets
        tbl = t.Tables("turret_model")
        If tbl Is Nothing Then

        Else

            Dim q1 = From row In tbl.AsEnumerable _
                Select _
                turret = row.Field(Of String)("model")

            For Each r0 In q1
                turrets(cnt) = r0
                cnt += 1
            Next
            If cnt = 0 Then
                bad_tanks.AppendLine(file_name)
                Return
            End If
            ReDim Preserve turrets(cnt)
        End If
        cnt = 0
        '----------------------------------
        '----- chassis

        tbl = t.Tables("chassis")
        Dim q2 = From row In tbl.AsEnumerable _
          Select _
          chass = row.Field(Of String)("model")
        For Each thing In q2

            chassis(cnt) = thing
            cnt += 1
        Next
        If cnt = 0 Then
            bad_tanks.AppendLine(file_name)
            Return
        End If
        ReDim Preserve chassis(cnt)
        cnt = 0
        '----------------------------------
        '----- hull
        tbl = t.Tables("hull")
        Dim q3 = From row In tbl.AsEnumerable
             Select _
             model = row.Field(Of String)("model"), _
             tile = row.Field(Of String)("hull_camouflage")

        For Each thing In q3
            hulls(cnt) = thing.model
            hull_tile(cnt) = New vect4
            Dim n = thing.tile.Split(" ")
            hull_tile(cnt).x = CSng(n(0))
            hull_tile(cnt).y = CSng(n(1))
            hull_tile(cnt).z = CSng(n(2))
            hull_tile(cnt).w = CSng(n(3))
            cnt += 1
        Next
        If cnt = 0 Then
            bad_tanks.AppendLine(file_name)
            Return
        End If
        ReDim Preserve hulls(cnt)
        ReDim Preserve hull_tile(cnt)
        cnt = 0
        '----------------------------------
        'Array.Sort(guns)
        'Array.Sort(turrets)
        'Array.Sort(hulls)
        'Array.Sort(chassis)
        turret_count = 0
        hull_count = 0
        Dim turret_name = turrets(turrets.Length - 2)
        turret_tiling = turret_tile(turrets.Length - 2)
        Dim hull_name = hulls(hulls.Length - 2)
        hull_tiling = hull_tile(hulls.Length - 2)
        Dim chassis_name = chassis(chassis.Length - 2)
        Dim gun_name As String = ""
        Dim ti, tj As New vect4
        If guns.Length = 10 Then
            gun_name = guns(guns.Length - 2)
            ti = gun_tile(guns.Length - 2)
            tj = ti
            tj.w = ti.z
            tj.z = ti.w
            gun_tiling = tj
        Else
            gun_name = guns(guns.Length - 2)
            ti = gun_tile(guns.Length - 2)
            tj = ti
            tj.w = ti.z
            tj.z = ti.w
            gun_tiling = tj
        End If
        '========================================

        'reset data params
        '---------------------
        If object_count > 0 Then
            For i = 1 To object_count
                Gl.glDeleteLists(_object(i).main_display_list, 1)
            Next
        End If
        ReDim _object(0)
        _object(0) = New obj
        object_count = 0
        'clear tank variables
        gun_trans = New vect3
        gun_trans2 = New vect3
        turret_trans = New vect3
        hull_trans = New vect3
        gun_location = New vect3
        turret_location = New vect3
        'make sure visiblity check boxes are checked
        If Not m_chassis.Checked Then
            m_chassis.PerformClick()
            m_chassis.ForeColor = Color.DarkGreen
        End If
        If Not m_hull.Checked Then
            m_hull.PerformClick()
            m_hull.ForeColor = Color.DarkGreen
        End If
        If Not m_turret.Checked Then
            m_turret.PerformClick()
            m_turret.ForeColor = Color.DarkGreen
        End If
        If Not m_gun.Checked Then
            m_gun.PerformClick()
            m_gun.ForeColor = Color.DarkGreen
        End If
        '---------------------
        If TESTING Then

            'test stuff to grab track stuff
            tbl = t.Tables("track_info")
            Dim tq = From row In tbl.AsEnumerable
                     Select _
                     seg_cnt = row.Field(Of String)("seg_cnt")


            If tq(0).Contains("1") Then
                track_info.segment_count = 1
                Dim t1q = From row In tbl.AsEnumerable
                          Select _
                          trp = row.Field(Of String)("right_filename"), _
                          tlp = row.Field(Of String)("left_filename"), _
                          seglength = row.Field(Of String)("segment_length"), _
                          seg_off = row.Field(Of String)("segmentOffset")
                For Each tr In t1q
                    track_info.left_path1 = tr.tlp
                    track_info.right_path1 = tr.trp
                    track_info.segment_length = tr.seglength
                    track_info.segment_offset1 = tr.seg_off
                Next

            Else
                track_info.segment_count = 2
                Dim t1q = From row In tbl.AsEnumerable
                          Select _
                          trp = row.Field(Of String)("right_filename"), _
                          tlp = row.Field(Of String)("left_filename"), _
                          seglength = row.Field(Of String)("segment_length"), _
                          seg_off = row.Field(Of String)("segmentOffset"), _
                          trp2 = row.Field(Of String)("right2_filename"), _
                          tlp2 = row.Field(Of String)("left2_filename"), _
                          seg_off2 = row.Field(Of String)("segment2Offset")
                For Each tr In t1q
                    track_info.left_path1 = tr.tlp
                    track_info.right_path1 = tr.trp
                    track_info.left_path2 = tr.tlp2
                    track_info.right_path2 = tr.trp2
                    track_info.segment_length = tr.seglength
                    track_info.segment_offset1 = tr.seg_off
                    track_info.segment_offset2 = tr.seg_off2
                Next

            End If



            Dim tra() = chassis_name.Split("/")
            Dim track_path = Path.GetDirectoryName(track_info.left_path1) + "\right.track"
            Dim tent = packages(current_tank_package)(track_path)
            Dim t_data As New DataSet
            If tent IsNot Nothing Then
                Dim ms As New MemoryStream
                tent.Extract(ms)
                openXml_stream(ms, "right.track")
                get_track_section()

            End If
            running = 0
            path_pointer1 = 0
            track_length = 0
            For i = 0 To tracks.Length - 1
                catmullrom.CatmullRomSpline_get_length(i)
            Next
            Dim lenS = running / track_info.segment_length
            If Z_Flipped Then
                lenS += 0
            Else
                lenS += 0

            End If
            segment_length_adjusted = running / (Floor(lenS))
            ReDim path_data1(CInt(Floor(lenS)) + 3)
            If Z_Flipped Then
                running = 0 - segment_length_adjusted
            Else
                running = 0 - track_info.segment_offset1 '  segment_length_adjusted
            End If
            GC.Collect()
            GC.WaitForFullGCComplete()
            path_pointer1 = 0
            track_length = 0
            For i = 0 To tracks.Length - 1
                catmullrom.GetCatmullRomSpline1(i)
            Next
            ReDim Preserve path_data1(path_pointer1)
            get_tread_rotations1()
            If track_info.segment_count = 2 Then
                ReDim path_data2(CInt(Floor(lenS)) + 3)
                track_length = 0
                If Z_Flipped Then
                    running = 0
                Else
                    running = 0
                End If
                path_pointer2 = 0
                For i = 0 To tracks.Length - 1
                    catmullrom.GetCatmullRomSpline2(i)
                Next
                ReDim Preserve path_data2(path_pointer2)
                get_tread_rotations2()
            End If

        End If
        file_name = chassis_name
        build_primitive_data(False) ' -- chassis

        file_name = hull_name
        build_primitive_data(True) ' -- chassis

        file_name = turret_name
        build_primitive_data(True) ' -- chassis

        file_name = gun_name
        build_primitive_data(True) ' -- chassis

        file_name = track_info.left_path1
        build_primitive_data(True) ' -- chassis
        If TESTING Then

            If track_info.segment_count = 2 Then
                file_name = track_info.left_path2
                build_primitive_data(True) ' -- chassis
            End If

        End If


        model_loaded = True
        part_counts = New part_counts_
        For i = 1 To object_count
            If _object(i).name.ToLower.Contains("chassis") Then
                part_counts.chassis_cnt += _object(i).count
            End If
            If _object(i).name.ToLower.Contains("hull") Then
                part_counts.hull_cnt += _object(i).count
            End If
            If _object(i).name.ToLower.Contains("turret") Then
                part_counts.turret_cnt += _object(i).count
            End If
            If _object(i).name.ToLower.Contains("gun") Then
                part_counts.gun_cnt += _object(i).count
            End If

        Next
        'get data_set camo set we will use
        'and string to scripts package
        Dim s As String = ""
        Select Case ar(1)
            Case "american"
                CURRENT_DATA_SET = 0
                s = "usa"
            Case "british"
                CURRENT_DATA_SET = 1
                s = "uk"
            Case "chinese"
                CURRENT_DATA_SET = 2
                s = "china"
            Case "czech"
                CURRENT_DATA_SET = 3
                s = "czech"
            Case "french"
                CURRENT_DATA_SET = 4
                s = "france"
            Case "german"
                CURRENT_DATA_SET = 5
                s = "germany"
            Case "japan"
                CURRENT_DATA_SET = 6
                s = "japan"
            Case "poland"
                CURRENT_DATA_SET = 7
                s = "poland"
            Case "russian"
                CURRENT_DATA_SET = 8
                s = "ussr"
            Case "sweden"
                CURRENT_DATA_SET = 9
                s = "sweden"
        End Select

        If save_tank Then

            Dim rot_limit_l, rot_limit_r As Single
            Dim gun_limit_u, gun_limit_d As Single
            rot_limit_l = -400.0
            rot_limit_r = 400.0

            Dim ent = scripts_pkg("scripts\item_defs\vehicles\" + s + "\" + ar(2) + ".xml")
            Dim ms As New MemoryStream
            ent.Extract(ms)
            openXml_stream(ms, "")
            Dim docx = XDocument.Parse(TheXML_String)
            Dim xmlroot As XmlNode = xDoc.CreateElement(XmlNodeType.Element, "root", "")
            'doc.DocumentElement.ParentNode.Value = "<root>" + vbCrLf + "</root>"
            For Each n As XElement In docx.Descendants("turretYawLimits")
                n.Value = n.Value.Replace("/", "\")
                Dim ar2 = n.Value.Split(" ")
                rot_limit_l = ar2(0)
                rot_limit_r = ar2(1)
                Exit For
            Next

            For Each n As XElement In docx.Descendants("minPitch")
                n.Value = n.Value.Replace("/", "\")
                Dim ar2 = n.Value.Split(" ")
                gun_limit_u = -ar2(1)
                Exit For
            Next
            For Each n As XElement In docx.Descendants("maxPitch")
                n.Value = n.Value.Replace("/", "\")
                Dim ar2 = n.Value.Split(" ")
                gun_limit_d = -ar2(1)
                Exit For
            Next


            If rot_limit_l = -180 Then rot_limit_l = -400
            If rot_limit_r = 180 Then rot_limit_r = 400

            Dim fo = File.Open(Application.StartupPath + "\tanks\" + ar(2) + ".tank", FileMode.OpenOrCreate)
            Dim fw As New BinaryWriter(fo)
            'version changes
            'ver 1 

            Dim version As Integer = 1
            Dim rotation_limit As Single = 0.0
            'ver 1
            'Dim s1 = "File format: INT as version, INT as chassis and hull vertex count, INT as turret and gun vertex count"
            'Dim s2 = "Floats turret pivet XY," + _
            '        "floats rot_limits L&R , " + _
            '        "floats as list of vertices: position XYZ Normal XYZ"
            'ver 2
            Dim s1 = "File format: INT as version, INT as chassis and hull vertex count, INT as turret vertex count, INT as Gun vertex Count"
            Dim s2 = "Floats turret pivet XYZ," + _
                    "floats rot_limits L&R,"
            Dim s3 = "floats gun pivit XYZ , floats gun limits U&D, " + _
                    "floats as list of vertices:Each being (position XYZ Normal XYZ),"
            Dim s4 = "floats (9) for future use."
            fw.Write(s1)
            fw.Write(s2)
            fw.Write(s3)
            fw.Write(s4)
            fw.Write(version)

            fw.Write(part_counts.chassis_cnt + part_counts.hull_cnt)
            fw.Write(part_counts.turret_cnt)
            fw.Write(part_counts.gun_cnt)
            'turret info
            fw.Write(turret_location.x)
            fw.Write(turret_location.y)
            fw.Write(turret_location.z)
            fw.Write(rot_limit_l)
            fw.Write(rot_limit_r)
            'gun info
            fw.Write(gun_location.x)
            fw.Write(gun_location.y)
            fw.Write(gun_location.z)
            fw.Write(gun_limit_u)
            fw.Write(gun_limit_d)
            'extra vects
            '1
            fw.Write(1.0!)
            fw.Write(1.0!)
            fw.Write(1.0!)
            '2
            fw.Write(1.0!)
            fw.Write(1.0!)
            fw.Write(1.0!)
            '3
            fw.Write(1.0!)
            fw.Write(1.0!)
            fw.Write(1.0!)

            For i = 1 To object_count
                If _object(i).name.ToLower.Contains("chassis") Then
                    write_vertex_data(_object(i), fw)
                End If
                If _object(i).name.ToLower.Contains("hull") Then
                    write_vertex_data(_object(i), fw)
                End If
                If _object(i).name.ToLower.Contains("turret") Then
                    write_vertex_data(_object(i), fw)
                End If
                If _object(i).name.ToLower.Contains("gun") Then
                    write_vertex_data(_object(i), fw)
                End If

            Next

            fo.Close()
        End If
        m_pick_camo.Enabled = True
    End Sub
    Private Sub get_track_section()

        Dim docx = XDocument.Parse(TheXML_String.Replace("matrix", "position"))
        Dim doc As New XmlDocument
        Dim xmlroot As XmlNode = xDoc.CreateElement(XmlNodeType.Element, "root", "")
        Dim root_node As XmlNode = doc.CreateElement("model")
        doc.AppendChild(root_node)

        For Each node In docx.Descendants("node")
            Dim node_ = doc.CreateElement("node")
            Dim name = doc.CreateElement("name")
            Dim matrix = doc.CreateElement("matrix")
            For Each n In node.Descendants("name")
                name.InnerText = n.Value.ToString
                node_.AppendChild(name)
            Next
            For Each mat In node.Descendants("position")
                matrix.InnerText = mat.Value.ToString.Replace("position", "")
                node_.AppendChild(matrix)
            Next
            root_node.AppendChild(node_)
        Next


        Dim fm As New MemoryStream
        doc.Save(fm)
        fm.Position = 0
        Dim data_set As New DataSet
        data_set.ReadXml(fm)

        Dim t = data_set.Tables("node")
        Dim q = From row In t.AsEnumerable
                Select _
                Name = row.Field(Of String)("name"), _
           Matrix = row.Field(Of String)("matrix")
        ' id = row.Field(Of String)("id"), _

        ReDim tracks(q.Count - 1)

        Dim cnt As Integer = 0
        For Each trk In q
            tracks(cnt) = New track_
            ReDim tracks(cnt).matrix(15)
            tracks(cnt).name = trk.Name
            ' tracks(cnt).id = trk.id
            Dim ar = trk.Matrix.Split(" ")
            Dim j As Integer = 0
            If ar.Length > 3 Then
                Dim mm(15) As Single
                For Each m In ar

                    If CSng(m) > 1.0 Or CSng(m) < -1.0 Then
                        mm(j) = CSng(m) * 0.01
                    Else
                        mm(j) = CSng(m)
                    End If
                    j += 1
                    If j = 16 Then Exit For
                Next
                tracks(cnt).position.X = mm(3)
                tracks(cnt).position.Y = mm(7)
                tracks(cnt).position.Z = -mm(11)

            Else
                j = 0
                Dim mm(15) As Single
                For Each m In ar

                    mm(j) = CSng(m)
                    j += 1
                    If j = 3 Then Exit For
                Next
                tracks(cnt).position.X = mm(0)
                tracks(cnt).position.Y = mm(1)
                tracks(cnt).position.Z = -mm(2)

            End If
            cnt += 1
        Next
        'check if we need to flip the Z on this track.
        If tracks(0).position = tracks(tracks.Length - 1).position Then
            ReDim Preserve tracks(tracks.Length - 2)
        End If
        Dim vv = tracks(0).position

        Z_Flipped = False
        If vv.Z < 0 Then
            For i = 0 To tracks.Length - 1
                tracks(i).position.Z *= -1.0
            Next
            Z_Flipped = True
        End If
        If vv.Y < 0 Then
            For i = 0 To tracks.Length - 1
                tracks(i).position.Z *= -1.0
            Next
        End If
        data_set.Dispose()

        fm.Dispose()


    End Sub

    Private Sub get_tread_rotations1()
        For i = 0 To path_pointer1 - 1
            path_data1(check_pos(i + 1, path_pointer1)).angle = 0
            path_data1(check_pos(i + 1, path_pointer1)).zc = 0
            path_data1(check_pos(i + 1, path_pointer1)).yc = 0
        Next
        For i = -2 To path_pointer1 - 1
            path_data1(check_pos(i + 1, path_pointer1)).angle = 0
            path_data1(check_pos(i + 1, path_pointer1)).zc = 0
            path_data1(check_pos(i + 1, path_pointer1)).yc = 0
            get_angle_and_center(i, path_data1, path_pointer1)
        Next

    End Sub
    Private Sub get_tread_rotations2()
        For i = 0 To path_pointer2 - 1
            path_data2(check_pos(i + 1, path_pointer2)).angle = 0
            path_data2(check_pos(i + 1, path_pointer2)).zc = 0
            path_data2(check_pos(i + 1, path_pointer2)).yc = 0
        Next
        For i = -1 To path_pointer2 - 1
            path_data2(check_pos(i + 1, path_pointer2)).angle = 0
            path_data2(check_pos(i + 1, path_pointer2)).zc = 0
            path_data2(check_pos(i + 1, path_pointer2)).yc = 0
            get_angle_and_center(i, path_data2, path_pointer2)
        Next

    End Sub
    Private Sub get_angle_and_center(ByVal pos As Integer, ByRef path_data() As path_data_, ByVal path_pointer As Integer)
        Dim yc, zc As Single
        Dim y1, z1, y2, z2, y3, z3 As Single
        Dim direction As Integer
        Dim cnt As Integer
        Dim z__ As Decimal

        Dim p1, p2, p3 As SlimDX.Vector3

        p1 = path_data(check_pos(pos, path_pointer)).pos1
        p2 = path_data(check_pos(pos + 1, path_pointer)).pos1
        p3 = path_data(check_pos(pos + 2, path_pointer)).pos1
        'gotta flip y and z for this old algo to work
        Dim rf As Integer = 3
        y1 = Round(p1.Y, rf)
        z1 = Round(p1.Z, rf)
        y2 = Round(p2.Y, rf)
        z2 = Round(p2.Z, rf)
        y3 = Round(p3.Y, rf)
        z3 = Round(p3.Z, rf)
        Dim s As Single = 0.5D * ((y2 - y3) * (y1 - y3) - (z2 - z3) * (z3 - z1))
        Dim sUnder As Single = (y1 - y2) * (z3 - z1) - (z2 - z1) * (y1 - y3)
        If sUnder <> 0 Then

            s /= sUnder

            yc = Round(0.5D * (y1 + y2) + s * (z2 - z1), 3) ' center y coordinate
            zc = Round(0.5D * (z1 + z2) + s * (y1 - y2), 3)  ' center y coordinate
        End If
        Dim radius As Single = CSng(Round((Sqrt(((y3 - yc) * (y3 - yc)) + ((z3 - zc) * (z3 - zc)))), 5))

        z__ = (y2 - y1) * (z3 - z2)
        z__ -= (z2 - z1) * (y3 - y2)
        If z__ < 0 Then
            cnt -= 1
        Else
            If z__ > 0 Then
                cnt += 1
            End If
        End If
        If z__ = 0 Then
            direction = 0
        End If
        If cnt > 0 Then
            direction = 3
        Else
            direction = 2
        End If
        Dim agl = Round(mAtan2(z2 - z1, y2 - y1), 6)
        If zc = 0 And yc = 0 Then
            path_data(check_pos(pos + 1, path_pointer)).angle = agl * 57.29577
            path_data(check_pos(pos + 1, path_pointer)).zc = z2
            path_data(check_pos(pos + 1, path_pointer)).yc = y2 + 0.25
            Return
            'End If
        End If
        Dim dyr1 As Single = CSng(Round(y1 - yc, 6))
        Dim dy2 As Single = CSng(Round(y3 - yc, 6))
        Dim dzr1 As Single = CSng(Round(z1 - zc, 6))
        Dim dz2 As Single = CSng(Round(z2 - zc, 6))
        Dim dy1 As Single = CSng(Round(y1 - yc, 6))
        Dim dz1 As Single = CSng(Round(z2 - zc, 6))

        Dim r1 = Sqrt((dyr1 * dyr1) + (dzr1 * dzr1))

        Dim sa = Round(mAtan2(z1 - z2, y1 - y2), 6)
        Dim ea = Round(mAtan2(z2 - z3, y2 - y3), 6)
        Dim angle = Round(mAtan2(z2 - zc, y2 - yc), 6)
        If direction = 3 Then
            angle += PI / 2.0
        Else
            angle -= PI / 2.0
        End If

        path_data(check_pos(pos + 1, path_pointer)).angle = angle * 57.29577
        path_data(check_pos(pos + 1, path_pointer)).zc = zc
        path_data(check_pos(pos + 1, path_pointer)).yc = yc
        If direction = 0 Then
            Stop
        End If
        Return



    End Sub
    Private Function check_pos(ByVal p As Integer, ByVal path_pointer As Integer)
        If p > path_pointer - 1 Then
            p -= (path_pointer - 1)
        End If
        If p < 0 Then
            p += (path_pointer - 1)
        End If
        Return p
    End Function
    Private Function mAtan2(ByVal y As Single, ByVal x As Single) As Single
        Dim theta As Single
        theta = CSng(Atan2(y, x))
        If theta < 0 Then
            theta += CSng((PI * 2))
        End If
        Return theta
    End Function
    Private Sub write_vertex_data(ByVal o As obj, ByVal fw As BinaryWriter)
        For i As Integer = 1 To o.count
            '1
            fw.Write(o.tris(i).v1.x)
            fw.Write(o.tris(i).v1.y)
            fw.Write(o.tris(i).v1.z)
            fw.Write(o.tris(i).n1.x)
            fw.Write(o.tris(i).n1.y)
            fw.Write(o.tris(i).n1.z)

            '2
            fw.Write(o.tris(i).v2.x)
            fw.Write(o.tris(i).v2.y)
            fw.Write(o.tris(i).v2.z)
            fw.Write(o.tris(i).n2.x)
            fw.Write(o.tris(i).n2.y)
            fw.Write(o.tris(i).n2.z)

            '3
            fw.Write(o.tris(i).v3.x)
            fw.Write(o.tris(i).v3.y)
            fw.Write(o.tris(i).v3.z)
            fw.Write(o.tris(i).n3.x)
            fw.Write(o.tris(i).n3.y)
            fw.Write(o.tris(i).n3.z)

        Next
    End Sub


    Private Sub clear_node_selection(ByRef n As TreeNode)
        If n.ForeColor = Color.White Then
            n.ForeColor = Color.Black
        End If
    End Sub


    Private Sub set_node_white(ByRef n As TreeNode)
        n.ForeColor = Color.White
    End Sub
    Private Sub set_node_black(ByRef n As TreeNode)
        n.ForeColor = Color.Black
    End Sub

    Private Sub get_tank_xml_data(ByVal n As TreeNode)
        Dim q = From row In TankDataTable _
            Where row.Field(Of String)("tag") = n.Text _
  Select _
       un = row.Field(Of String)("shortname"), _
       tier = row.Field(Of String)("tier"), _
       natiom = row.Field(Of String)("nation"), _
       Type = row.Field(Of String)("type")
       Order By tier Descending

        'Dim a = q(0).un.Split(":")
        If q(0) IsNot Nothing Then
            out_string.Append(n.Text + ":" + q(0).un + ":" + q(0).natiom + ":" + q(0).tier + ":" + q(0).Type + ":")


        End If

    End Sub
    Dim out_string As New StringBuilder


    Private Sub pb1_Paint(sender As Object, e As PaintEventArgs) Handles pb1.Paint
        If w_changing Then draw_scene()
    End Sub
End Class
