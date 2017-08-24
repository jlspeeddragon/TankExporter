
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
    Public gl_stop As Boolean = False
    Dim update_thread As New Thread(AddressOf update_mouse)
    Public path_set As Boolean = False
    Dim mouse As vec2
    Dim move_cam_z, M_DOWN, move_mod, z_move As Boolean
    Public Shared packages(10) As Ionic.Zip.ZipFile
    Public Shared packages_HD(10) As Ionic.Zip.ZipFile
    Public Shared shared_pkg As Ionic.Zip.ZipFile
    Public Shared shared_sandbox_pkg As Ionic.Zip.ZipFile
    Dim treeviews(10) As TreeView
    Dim gui_pkg As Ionic.Zip.ZipFile
    Dim scripts_pkg As Ionic.Zip.ZipFile
    Public icons(10) As pngs
    Structure pngs
        Public img() As System.Drawing.Bitmap
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

    Public usa_ds As New DataSet
    Public uk_ds As New DataSet
    Public china_ds As New DataSet
    Public czech_ds As New DataSet
    Public france_ds As New DataSet
    Public germany_ds As New DataSet
    Public japan_ds As New DataSet
    Public poland_ds As New DataSet
    Public ussr_ds As New DataSet
    Public sweden_ds As New DataSet

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
        'If e.KeyCode = Keys.S Then
        '    frmEditFrag.Show()
        '    frmEditFrag.TopMost = True
        'End If

    End Sub

    Private Sub frmMain_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
        If move_mod Then
            move_mod = False
        End If
        If z_move Then
            z_move = False
        End If
    End Sub



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
        Me.KeyPreview = True    'so i catch keyboard before despatching it
        tank_label.Text = ""
        Me.Show()
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
        '---------------------------
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
        '=========================
        _Started = True
        If My.Settings.game_path = "" Then
            MsgBox("Game Location needs to be set.", MsgBoxStyle.Information)
            M_Path.PerformClick()
        End If
        info_Label.Text = "Loading Data from Packages..."
        Application.DoEvents()
        Try

            gui_pkg = New Ionic.Zip.ZipFile(My.Settings.game_path + "\res\packages\gui.pkg")
            scripts_pkg = New Ionic.Zip.ZipFile(My.Settings.game_path + "\res\packages\scripts.pkg")
            shared_pkg = ZipFile.Read(My.Settings.game_path + "\res\packages\shared_content.pkg")
            shared_sandbox_pkg = ZipFile.Read(My.Settings.game_path + "\res\packages\shared_content_sandbox.pkg")

            load_back_ground()
        Catch ex As Exception
            log_text.AppendLine("Package files not found")
        End Try
        Application.DoEvents()
        '=========================
        m_chassis.ForeColor = Color.DarkGreen
        m_hull.ForeColor = Color.DarkGreen
        m_turret.ForeColor = Color.DarkGreen
        m_gun.ForeColor = Color.DarkGreen
        '=========================
        tank_label.Parent = iconbox
        tank_label.Text = ""
        tank_label.Location = New Point(5, 10)
        '=========================

        make_xy_grid()
        'create_GL_menu()

        'Check for temp storage folder.. It it exist.. load the API data.. 
        'other wise make the directory and get the API data.
        Temp_Storage = Path.GetTempPath ' this gets the user temp storage folder
        Temp_Storage += "wot_temp"
        If Not System.IO.Directory.Exists(Temp_Storage) Then
            System.IO.Directory.CreateDirectory(Temp_Storage)
        End If

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
        '------------------------------------------------------------------------
        '------------------------------------------------------------------------
        TC1.SelectedIndex = 0
        Application.DoEvents()
        load_tabs()
        Application.DoEvents()
        info_Label.Visible = False
        pb1.Visible = True
        make_shaders() 'compile the shaders
        set_shader_variables() ' update uniform addresses
        Startup_Timer.Enabled = True
        TC1.SelectedIndex = 0
        Application.DoEvents()
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
                pb1.BackgroundImage = get_png(ms)
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
        Dim root_node As XmlNode = doc.CreateElement("models")
        doc.AppendChild(root_node)
        'doc.DocumentElement.ParentNode.Value = "<root>" + vbCrLf + "</root>"
        For Each n As XElement In docx.Descendants("undamaged")
            n.Value = n.Value.Replace("/", "\")
            Dim nd = doc.CreateElement("undamaged")
            Dim tx = doc.CreateTextNode(n.Value)
            doc.DocumentElement.AppendChild(nd)
            doc.DocumentElement.LastChild.AppendChild(tx)
        Next


        Dim fm As New MemoryStream
        doc.Save(fm)
        fm.Position = 0
        data_set.ReadXml(fm)
        ms.Dispose()
        fm.Dispose()
    End Sub
    Private Sub load_tabs()
        For i = 1 To 10
            store_in_treeview(i, treeviews(i))
        Next
    End Sub
    Private Sub store_in_treeview(ByVal i As Integer, tn As TreeView)
        TC1.SelectedIndex = i - 1
        Application.DoEvents()
        info_Label.Text = "Adding Tanks to TreeView Lists (" + i.ToString("00") + ")"
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
        AddHandler tn.NodeMouseClick, AddressOf Me.tv_clicked
        AddHandler tn.NodeMouseHover, AddressOf Me.tv_mouse_enter
        'AddHandler tn.MouseEnter, Function(sender, e) tv_mouse_enter(New TreeView, New MouseEventArgs)
        AddHandler tn.MouseLeave, AddressOf Me.tv_mouse_leave
        tn.Tag = i
        Dim cnt As Integer = 0
        ReDim icons(i).img(100)
        alltanks.Append("# Tier " + i.ToString("00") + vbCrLf)
        For Each entry As ZipEntry In packages(i)
            If entry.FileName.Contains("normal/lod2/Chassis.model") Then
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
                        s = get_user_name(usa_ds, n.Text)
                    Case "british"
                        n.Name = "uk"
                        s = get_user_name(uk_ds, n.Text)
                    Case "chinese"
                        n.Name = "china"
                        s = get_user_name(china_ds, n.Text)
                    Case "czech"
                        n.Name = "czech"
                        s = get_user_name(czech_ds, n.Text)
                    Case "french"
                        n.Name = "france"
                        s = get_user_name(france_ds, n.Text)
                    Case "german"
                        n.Name = "germany"
                        s = get_user_name(germany_ds, n.Text)
                    Case "japan"
                        n.Name = "japan"
                        s = get_user_name(japan_ds, n.Text)
                    Case "poland"
                        n.Name = "poland"
                        s = get_user_name(poland_ds, n.Text)
                    Case "russian"
                        n.Name = "ussr"
                        s = get_user_name(ussr_ds, n.Text)
                    Case "sweden"
                        n.Name = "sweden"
                        s = get_user_name(sweden_ds, n.Text)
                End Select
                If s.Length > 0 Then ' only save what actually exist
                    icons(i).img(cnt) = get_tank_icon(n.Text).Clone
                    If icons(i).img(cnt) IsNot Nothing Then
                        icons(i).img(cnt).Tag = current_png_path
                        tn.Nodes.Add(n)
                        cnt += 1
                    End If
                End If
            End If
        Next
        Application.DoEvents()
        ReDim Preserve icons(i).img(cnt)
        Application.DoEvents()
    End Sub

    Private Function get_user_name(ByRef ds As DataSet, ByVal fname As String) As String

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

    End Sub
    Private Sub tv_mouse_enter(ByVal sender As Object, ByVal e As TreeNodeMouseHoverEventArgs)
        iconbox.Visible = True
        'iconbox.BringToFront()
        Dim tn = DirectCast(sender, TreeView)
        tn.Focus()
        iconbox.BackgroundImage = icons(tn.Tag).img(e.Node.Index)
        Dim s = get_shortname(e.Node)
        Dim ar = s.Split(":")
        tank_label.Text = ar(0)
        file_name = e.Node.Tag
    End Sub
    Private Sub tv_mouse_leave()
        If conMenu.Visible Then
            Return
        End If
        iconbox.Visible = False
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

    Public Function get_png(ByVal ms As MemoryStream) As Bitmap
        'Dim s As String = ""
        's = Gl.glGetError
        Dim image_id As Integer = -1
        'Dim app_local As String = Application.StartupPath.ToString

        Dim texID As UInt32
        Dim textIn(ms.Length) As Byte
        ms.Position = 0
        ms.Read(textIn, 0, ms.Length)
        texID = Ilu.iluGenImage() ' /* Generation of one image name */
        Il.ilBindImage(texID) '; /* Binding of image name */
        Dim success = Il.ilGetError
        Il.ilLoadL(Il.IL_PNG, textIn, textIn.Length)
        success = Il.ilGetError
        If success = Il.IL_NO_ERROR Then
            'Ilu.iluFlipImage()
            'Ilu.iluMirror()
            Dim width As Integer = Il.ilGetInteger(Il.IL_IMAGE_WIDTH)
            Dim height As Integer = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT)

            ' Create the bitmap.
            Dim Bitmapi = New System.Drawing.Bitmap(width, height, PixelFormat.Format32bppArgb)
            Dim rect As Rectangle = New Rectangle(0, 0, width, height)

            ' Store the DevIL image data into the bitmap.
            Dim bitmapData As BitmapData = Bitmapi.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb)

            Il.ilConvertImage(Il.IL_BGRA, Il.IL_UNSIGNED_BYTE)
            Il.ilCopyPixels(0, 0, 0, width, height, 1, Il.IL_BGRA, Il.IL_UNSIGNED_BYTE, bitmapData.Scan0)
            Bitmapi.UnlockBits(bitmapData)

            'If your image contains alpha channel you can replace IL_RGB with IL_RGBA */
            'If make_id Then

            '    Gl.glGenTextures(1, image_id)
            '    Gl.glEnable(Gl.GL_TEXTURE_2D)
            '    Gl.glBindTexture(Gl.GL_TEXTURE_2D, image_id)
            '    Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR)
            '    Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST_MIPMAP_LINEAR)
            '    Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_GENERATE_MIPMAP, Gl.GL_TRUE)

            '    Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT)
            '    Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT)

            '    Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Il.ilGetInteger(Il.IL_IMAGE_BPP), Il.ilGetInteger(Il.IL_IMAGE_WIDTH), _
            '                    Il.ilGetInteger(Il.IL_IMAGE_HEIGHT), 0, Il.ilGetInteger(Il.IL_IMAGE_FORMAT), Gl.GL_UNSIGNED_BYTE, _
            '                    Il.ilGetData()) '  Texture specification 
            '    Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            '    Il.ilBindImage(0)
            '    'ilu.iludeleteimage(texID)
            '    ReDim Preserve map_texture_ids(index + 1)
            '    map_texture_ids(index) = image_id
            'End If

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Il.ilBindImage(0)
            Ilu.iluDeleteImage(texID)
            'GC.Collect()
            Return Bitmapi
        Else
            MsgBox("can't find thumb image of tank", MsgBoxStyle.Critical, "oops")
        End If
        Return Nothing
    End Function

    '###########################################################################################################################################
    Public Sub draw_scene()
        If gl_stop Then Return
        gl_busy = True
        Dim color_top() As Byte = {20, 20, 20}
        Dim color_bottom() As Byte = {60, 60, 60}
        Dim position() As Single = {10, 10.0F, 10, 1.0F}
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
        Dim er = Gl.glGetError
        Gl.glDepthFunc(Gl.GL_LEQUAL)
        Gl.glFrontFace(Gl.GL_CW)
        'Gl.glCullFace(Gl.GL_BACK)
        Gl.glLineWidth(1)
        Gl.glClearColor(0.0F, 0.0F, 0.0F, 1.0F)

        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
        Gl.glDisable(Gl.GL_BLEND)
        Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE)

        Gl.glEnable(Gl.GL_LIGHTING)
        Gl.glEnable(Gl.GL_CULL_FACE)

        Gl.glEnable(Gl.GL_SMOOTH)
        Gl.glMateriali(Gl.GL_FRONT, Gl.GL_SHININESS, 60)

        Dim diffuseLight() As Single = {0.65, 0.65, 0.65, 1.0F}
        Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_DIFFUSE, diffuseLight)

        Dim specular() As Single = {0.8F, 0.8F, 0.8F, 1.0F}
        Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_SPECULAR, specular)

        Dim ambient() As Single = {0.2F, 0.2F, 0.2F, 1.0F}
        Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_AMBIENT, ambient)


        ResizeGL()
        Dim v As Point = pb1.Size
        Gl.glDisable(Gl.GL_DEPTH_TEST)
        Gl.glClearColor(0.0F, 0.0F, 0.2353F, 1.0F)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)

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
        Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, position)
        Gl.glEnable(Gl.GL_LIGHTING)
        ViewPerspective()
        set_eyes()
        Gl.glPushMatrix()
        Gl.glDisable(Gl.GL_CULL_FACE)
        '-----------------------------------------------------------------------------
        Gl.glEnable(Gl.GL_DEPTH_TEST)
        Gl.glColor3f(0.6, 0.6, 0.6)
        If model_loaded And Not m_load_textures.Checked Then
            For jj = 1 To object_count
                If _object(jj).visible Then
                    Gl.glCallList(_object(jj).main_display_list)
                End If
            Next
        End If

        If model_loaded And m_load_textures.Checked Then
            Gl.glUseProgram(shader_list.tank_shader)
            Gl.glUniform1i(tank_colorMap, 0)
            Gl.glUniform1i(tank_normalMap, 1)
            Gl.glUniform1i(tank_GMM, 2)
            Gl.glUniform1i(tank_AO, 3)
            Gl.glUniform3f(tank_viewPosition, eyeX, eyeY, eyeZ)
            For jj = 1 To object_count
                Gl.glUniform1i(tank_is_GAmap, _object(jj).ANM)
                Gl.glUniform1i(tank_alphaTest, _group(jj).alphaTest)
                If _object(jj).visible Then
                    Gl.glActiveTexture(Gl.GL_TEXTURE0)
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).color_Id)
                    Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).normal_Id)
                    Gl.glActiveTexture(Gl.GL_TEXTURE0 + 2)
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).metal_Id)
                    Gl.glActiveTexture(Gl.GL_TEXTURE0 + 3)
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).ao_id)
                    Gl.glCallList(_object(jj).main_display_list)
                End If
            Next
            Gl.glUseProgram(0)

            'clear texture bindings
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '3
            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 2)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '2
            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '1
            Gl.glActiveTexture(Gl.GL_TEXTURE0)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '0
        End If


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
        Gl.glPushMatrix()
        Gl.glTranslatef(0.0, 0.0F, -0.01F)
        Gl.glDisable(Gl.GL_DEPTH_TEST)
        Gl.glDisable(Gl.GL_LIGHTING)

        'menu
        'draw_menu()



        Gl.glEnable(Gl.GL_BLEND)
        Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA)
        Gl.glColor4f(0.3, 0.0, 0.0, 0.6)
        Gl.glBegin(Gl.GL_TRIANGLES)
        Gl.glVertex3f(0.0, -pb1.Height, 0.0)
        Gl.glVertex3f(0.0, -pb1.Height + 20, 0.0)
        Gl.glVertex3f(pb1.Width, -pb1.Height + 20, 0.0)

        Gl.glVertex3f(pb1.Width, -pb1.Height + 20, 0.0)
        Gl.glVertex3f(pb1.Width, -pb1.Height, 0.0)
        Gl.glVertex3f(0.0, -pb1.Height, 0.0)
        Gl.glEnd()

        Dim fps As Integer = 1.0 / (screen_totaled_draw_time * 0.001)
        Dim str = " FPS: ( " + fps.ToString + " )"
        'swat.Stop()
        glutPrint(10, 8 - pb1.Height, str.ToString, 0.0, 1.0, 0.0, 1.0)

        Gl.glDisable(Gl.GL_BLEND)
        Gl.glEnable(Gl.GL_LIGHTING)

        Gl.glPopMatrix()
        Gdi.SwapBuffers(pb1_hDC)
        Gl.glFlush()
        er = Gl.glGetError
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
            M_DOWN = True
            mouse.x = e.X
            mouse.y = e.Y
        End If
    End Sub

    Private Sub pb1_MouseMove(sender As Object, e As MouseEventArgs) Handles pb1.MouseMove
        m_mouse.x = e.X
        m_mouse.y = e.Y
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
    Public Sub update_mouse()
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
            If Not gl_busy Then
                update_screen()
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

            Thread.Sleep(1)
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


    Private Sub M_Path_Click(sender As Object, e As EventArgs) Handles M_Path.Click
        If FolderBrowserDialog1.ShowDialog = Forms.DialogResult.OK Then
            My.Settings.game_path = FolderBrowserDialog1.SelectedPath
            If Not File.Exists(My.Settings.game_path + "\WOTLauncher.exe") Then
                MsgBox("Incorrect Path.", MsgBoxStyle.Information)
                M_Path.PerformClick()
                Return
            End If
            path_set = True
            My.Settings.game_path = FolderBrowserDialog1.SelectedPath
            Return
        End If
    End Sub


    Private Sub frmMain_ResizeBegin(sender As Object, e As EventArgs) Handles Me.ResizeBegin
        gl_stop = True
    End Sub

    Private Sub frmMain_ResizeEnd(sender As Object, e As EventArgs) Handles Me.ResizeEnd
        gl_stop = False
    End Sub

    Private Sub m_load_Click(sender As Object, e As EventArgs) Handles m_load.Click
        process_tank(False) 'false .. don't save the binary tank file
    End Sub

    Public Sub process_tank(ByVal save_tank As Boolean)
        ReDim textures(0)

        For i = 1 To 50
            Gl.glDeleteTextures(1, i)
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
        current_tank_package = CInt(ar(0))
        ar = file_name.Split("/")
        Dim xml_file = ar(0) + "\" + ar(1) + "\" + ar(2) + ".xml"
        Dim t As New DataSet
        get_tank_parts_from_xml(xml_file, t)
        If t.Tables.Count = 0 Then
            Return
        End If
        '-----------------------------------
        'get take part paths from table
        Dim turrets(10) As String
        Dim guns(10) As String
        Dim hulls(10) As String
        Dim chassis(10) As String
        Dim cnt As Integer = 0
        Dim tbl = t.Tables("undamaged")
        Dim q = From row In tbl.AsEnumerable _
                Select r0 = row.Field(Of String)("undamaged_Text")
        cnt = 0
        '----- turrets
        For Each r0 In q
            If r0.ToLower.Contains("turret") Then
                turrets(cnt) = r0
                cnt += 1
            End If
        Next
        If cnt = 0 Then
            bad_tanks.AppendLine(file_name)
            Return
        End If
        ReDim Preserve turrets(cnt)
        cnt = 0
        '----------------------------------
        '----- chassis
        For Each r0 In q
            If r0.ToLower.Contains("chassis") Then
                chassis(cnt) = r0
                cnt += 1
            End If
        Next
        If cnt = 0 Then
            bad_tanks.AppendLine(file_name)
            Return
        End If
        ReDim Preserve chassis(cnt)
        cnt = 0
        '----------------------------------
        '----- guns
        For Each r0 In q
            Dim gn = Path.GetFileNameWithoutExtension(r0)
            If gn.ToLower.Contains("gun_") Then
                guns(cnt) = r0
                cnt += 1
            End If
        Next
        If cnt = 0 Then
            bad_tanks.AppendLine(file_name)
            Return
        End If
        ReDim Preserve guns(cnt)
        cnt = 0
        '----------------------------------
        '----- hull
        For Each r0 In q
            If r0.ToLower.Contains("hull") Then
                hulls(cnt) = r0
                cnt += 1
            End If
        Next
        If cnt = 0 Then
            bad_tanks.AppendLine(file_name)
            Return
        End If
        ReDim Preserve hulls(cnt)
        cnt = 0
        '----------------------------------
        Array.Sort(guns)
        Array.Sort(turrets)
        Array.Sort(hulls)
        Array.Sort(chassis)
        Dim turret_name = turrets(turrets.Length - 1)
        Dim hull_name = hulls(hulls.Length - 1)
        Dim chassis_name = chassis(chassis.Length - 1)
        Dim gun_name As String = ""
        If guns.Length = 10 Then
            gun_name = guns(guns.Length - 2)
        Else
            gun_name = guns(guns.Length - 1)

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
        model_loaded = False
        chassis_texture = ""
        hull_texture = ""
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
        file_name = chassis_name
        build_primitive_data(False) ' -- chassis

        file_name = hull_name
        build_primitive_data(True) ' -- chassis

        file_name = turret_name
        build_primitive_data(True) ' -- chassis

        file_name = gun_name
        build_primitive_data(True) ' -- chassis

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
        If save_tank Then
            Dim s As String = ""
            Select Case ar(1)
                Case "american"
                    s = "usa"
                Case "british"
                    s = "uk"
                Case "chinese"
                    s = "china"
                Case "czech"
                    s = "czech"
                Case "french"
                    s = "france"
                Case "german"
                    s = "germany"
                Case "japan"
                    s = "japan"
                Case "poland"
                    s = "poland"
                Case "russian"
                    s = "ussr"
                Case "sweden"
                    s = "sweden"
            End Select

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
    End Sub
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

    Private Sub clear_node_selection(ByRef n As TreeNode)
        If n.ForeColor = Color.White Then
            n.ForeColor = Color.Black
        End If
    End Sub

    Private Sub M_Exit_Click(sender As Object, e As EventArgs) Handles M_Exit.Click
        Me.Close()
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
                _object(i).visible = m_chassis.Checked
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

    Private Sub m_edit_shaders_Click(sender As Object, e As EventArgs) Handles m_edit_shaders.Click
        frmEditFrag.Show()
    End Sub

    Private Sub m_load_textures_CheckedChanged(sender As Object, e As EventArgs) Handles m_load_textures.CheckedChanged
        If m_load_textures.Checked Then
            m_load_textures.ForeColor = Color.Red
        Else
            m_load_textures.ForeColor = Color.Black
        End If
    End Sub


End Class
