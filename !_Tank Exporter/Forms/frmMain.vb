﻿#Region "imports"
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
    Protected Overrides Sub OnClientSizeChanged(e As EventArgs)
        If Not _Started Then Return
        G_Buffer.init()
        draw_scene()
        MyBase.OnClientSizeChanged(e)
    End Sub
#Region "variables"
    Dim pb2_has_focus As Boolean = False
    Dim out_string As New StringBuilder
    Public Background_image_id As Integer
    Private window_state As Integer
    Public Show_lights As Boolean = False
    Public gl_stop As Boolean = False
    Public update_thread As New Thread(AddressOf update_mouse)
    Public path_set As Boolean = False
    Public res_mods_path_set As Boolean = False
    Dim mouse As vec2
    Private mouse_down As Boolean = False
    Public mouse_delta As New Point
    Private mouse_pos As New Point
    Public mouse_find_location As New Point
    Public found_triangle_tv As Integer
    Private TOTAL_TANKS_FOUND As Integer = 0

    Dim delay As Integer = 0
    Dim stepper As Integer = 0

    Public Shared packages(12) As ZipFile
    Public Shared packages_2(12) As ZipFile
    Public Shared packages_HD(12) As ZipFile
    Public Shared packages_HD_2(12) As ZipFile
    Public Shared shared_pkg As Ionic.Zip.ZipFile
    Public Shared shared_sandbox_pkg As Ionic.Zip.ZipFile
    Public shared_contents_build As New Ionic.Zip.ZipFile
    Public gui_pkg As Ionic.Zip.ZipFile
    Public scripts_pkg As Ionic.Zip.ZipFile
    Dim treeviews(10) As TreeView
    Public icons(10) As pngs
    Public view_status_string As String
    Public tank_mini_icons As New ImageList

    Dim time As New Stopwatch
    Dim pick_timer As New Stopwatch
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
    Dim spin_light As Boolean = False
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
            'shared_pkg.Dispose()
            'shared_sandbox_pkg.Dispose()
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
        Dim tab = TC1.SelectedTab
        Dim c = tab.Controls
        Try

            Dim t = DirectCast(c(0), TreeView)
            t.SelectedNode = Nothing
            t.Parent.Focus()
        Catch ex As Exception

        End Try
        If e.KeyCode = Keys.C Then
            If CENTER_SELECTION Then
                CENTER_SELECTION = False
            Else
                CENTER_SELECTION = True
            End If
            pb1.Focus()
        End If
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
        If stop_updating Then draw_scene()

    End Sub

    Private Sub frmMain_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
        If move_mod Then
            move_mod = False
        End If
        If z_move Then
            z_move = False
        End If
        If stop_updating Then draw_scene()

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


    Private Sub frmMain_ResizeEnd(sender As Object, e As EventArgs) Handles Me.ResizeEnd
        'gl_stop = False
        'If season_Buttons_VISIBLE Then
        WINDOW_HEIGHT_DELTA = pb1.Height - OLD_WINDOW_HEIGHT
        relocate_season_Bottons()
        relocate_camobuttons()
        relocate_tankbuttons()
        relocate_texturebuttons()
        'End If
        If Not _Started Then Return
        If stop_updating Then draw_scene()
        w_changing = False
    End Sub

    Private Sub me_size_changed()
        If window_state <> Me.WindowState Then
            If Not Me.WindowState = FormWindowState.Minimized Then
                'gl_stop = True
                'If season_Buttons_VISIBLE Then
                WINDOW_HEIGHT_DELTA = pb1.Height - OLD_WINDOW_HEIGHT
                relocate_season_Bottons()
                relocate_camobuttons()
                relocate_tankbuttons()
                relocate_texturebuttons()
                OLD_WINDOW_HEIGHT = pb1.Height
                'End If
                w_changing = False
                window_state = Me.WindowState
                If stop_updating Then draw_scene()
                Return
                gl_stop = False
            End If
            window_state = Me.WindowState
        End If
        If Not _Started Then Return
        'If season_Buttons_VISIBLE Then
        WINDOW_HEIGHT_DELTA = pb1.Height - OLD_WINDOW_HEIGHT
        relocate_season_Bottons()
        relocate_camobuttons()
        relocate_tankbuttons()
        relocate_texturebuttons()
        OLD_WINDOW_HEIGHT = pb1.Height
        G_Buffer.init()
        'End If
        draw_scene()
    End Sub

    Private Sub make_888_lookup_table()
        lookup(0) = 254
        lookup(1) = 253
        lookup(2) = 252
        lookup(3) = 251
        lookup(4) = 250
        lookup(5) = 249
        lookup(6) = 248
        lookup(7) = 247
        lookup(8) = 246
        lookup(9) = 245
        lookup(10) = 244
        lookup(11) = 243
        lookup(12) = 242
        lookup(13) = 241
        lookup(14) = 240
        lookup(15) = 239
        lookup(16) = 238
        lookup(17) = 237
        lookup(18) = 236
        lookup(19) = 235
        lookup(20) = 234
        lookup(21) = 233
        lookup(22) = 232
        lookup(23) = 231
        lookup(24) = 230
        lookup(25) = 229
        lookup(26) = 228
        lookup(27) = 227
        lookup(28) = 226
        lookup(29) = 225
        lookup(30) = 224
        lookup(31) = 223
        lookup(32) = 222
        lookup(33) = 221
        lookup(34) = 220
        lookup(35) = 219
        lookup(36) = 218
        lookup(37) = 217
        lookup(38) = 216
        lookup(39) = 215
        lookup(40) = 214
        lookup(41) = 213
        lookup(42) = 212
        lookup(43) = 211
        lookup(44) = 210
        lookup(45) = 209
        lookup(46) = 208
        lookup(47) = 207
        lookup(48) = 206
        lookup(49) = 205
        lookup(50) = 204
        lookup(51) = 203
        lookup(52) = 202
        lookup(53) = 201
        lookup(54) = 200
        lookup(55) = 199
        lookup(56) = 198
        lookup(57) = 197
        lookup(58) = 196
        lookup(59) = 195
        lookup(60) = 194
        lookup(61) = 193
        lookup(62) = 192
        lookup(63) = 191
        lookup(64) = 190
        lookup(65) = 189
        lookup(66) = 188
        lookup(67) = 187
        lookup(68) = 186
        lookup(69) = 185
        lookup(70) = 184
        lookup(71) = 183
        lookup(72) = 182
        lookup(73) = 181
        lookup(74) = 180
        lookup(75) = 179
        lookup(76) = 178
        lookup(77) = 177
        lookup(78) = 176
        lookup(79) = 175
        lookup(80) = 174
        lookup(81) = 173
        lookup(82) = 172
        lookup(83) = 171
        lookup(84) = 170
        lookup(85) = 169
        lookup(86) = 168
        lookup(87) = 167
        lookup(88) = 166
        lookup(89) = 165
        lookup(90) = 164
        lookup(91) = 163
        lookup(92) = 162
        lookup(93) = 161
        lookup(94) = 160
        lookup(95) = 159
        lookup(96) = 158
        lookup(97) = 157
        lookup(98) = 156
        lookup(99) = 155
        lookup(100) = 154
        lookup(101) = 153
        lookup(102) = 152
        lookup(103) = 151
        lookup(104) = 150
        lookup(105) = 149
        lookup(106) = 148
        lookup(107) = 147
        lookup(108) = 146
        lookup(109) = 145
        lookup(110) = 144
        lookup(111) = 143
        lookup(112) = 142
        lookup(113) = 141
        lookup(114) = 140
        lookup(115) = 139
        lookup(116) = 138
        lookup(117) = 137
        lookup(118) = 136
        lookup(119) = 135
        lookup(120) = 134
        lookup(121) = 133
        lookup(122) = 132
        lookup(123) = 131
        lookup(124) = 130
        lookup(125) = 129
        lookup(126) = 128
        lookup(127) = 127
        lookup(128) = 126
        lookup(129) = 125
        lookup(130) = 124
        lookup(131) = 123
        lookup(132) = 122
        lookup(133) = 121
        lookup(134) = 120
        lookup(135) = 119
        lookup(136) = 118
        lookup(137) = 117
        lookup(138) = 116
        lookup(139) = 115
        lookup(140) = 114
        lookup(141) = 113
        lookup(142) = 112
        lookup(143) = 111
        lookup(144) = 110
        lookup(145) = 109
        lookup(146) = 108
        lookup(147) = 107
        lookup(148) = 106
        lookup(149) = 105
        lookup(150) = 104
        lookup(151) = 103
        lookup(152) = 102
        lookup(153) = 101
        lookup(154) = 100
        lookup(155) = 99
        lookup(156) = 98
        lookup(157) = 97
        lookup(158) = 96
        lookup(159) = 95
        lookup(160) = 94
        lookup(161) = 93
        lookup(162) = 92
        lookup(163) = 91
        lookup(164) = 90
        lookup(165) = 89
        lookup(166) = 88
        lookup(167) = 87
        lookup(168) = 86
        lookup(169) = 85
        lookup(170) = 84
        lookup(171) = 83
        lookup(172) = 82
        lookup(173) = 81
        lookup(174) = 80
        lookup(175) = 79
        lookup(176) = 78
        lookup(177) = 77
        lookup(178) = 76
        lookup(179) = 75
        lookup(180) = 74
        lookup(181) = 73
        lookup(182) = 72
        lookup(183) = 71
        lookup(184) = 70
        lookup(185) = 69
        lookup(186) = 68
        lookup(187) = 67
        lookup(188) = 66
        lookup(189) = 65
        lookup(190) = 64
        lookup(191) = 63
        lookup(192) = 62
        lookup(193) = 61
        lookup(194) = 60
        lookup(195) = 59
        lookup(196) = 58
        lookup(197) = 57
        lookup(198) = 56
        lookup(199) = 55
        lookup(200) = 54
        lookup(201) = 53
        lookup(202) = 52
        lookup(203) = 51
        lookup(204) = 50
        lookup(205) = 49
        lookup(206) = 48
        lookup(207) = 47
        lookup(208) = 46
        lookup(209) = 45
        lookup(210) = 44
        lookup(211) = 43
        lookup(212) = 42
        lookup(213) = 41
        lookup(214) = 40
        lookup(215) = 39
        lookup(216) = 38
        lookup(217) = 37
        lookup(218) = 36
        lookup(219) = 35
        lookup(220) = 34
        lookup(221) = 33
        lookup(222) = 32
        lookup(223) = 31
        lookup(224) = 30
        lookup(225) = 29
        lookup(226) = 28
        lookup(227) = 27
        lookup(228) = 26
        lookup(229) = 25
        lookup(230) = 24
        lookup(231) = 23
        lookup(232) = 22
        lookup(233) = 21
        lookup(234) = 20
        lookup(235) = 19
        lookup(236) = 18
        lookup(237) = 17
        lookup(238) = 16
        lookup(239) = 15
        lookup(240) = 14
        lookup(241) = 13
        lookup(242) = 12
        lookup(243) = 11
        lookup(244) = 10
        lookup(245) = 9
        lookup(246) = 8
        lookup(247) = 7
        lookup(248) = 6
        lookup(249) = 5
        lookup(250) = 4
        lookup(251) = 3
        lookup(252) = 2
        lookup(253) = 1
        lookup(254) = 0
        lookup(255) = 0

    End Sub

    '############################################################################ form load
    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim nonInvariantCulture As CultureInfo = New CultureInfo("en-US")
        nonInvariantCulture.NumberFormat.NumberDecimalSeparator = "."
        Thread.CurrentThread.CurrentCulture = nonInvariantCulture

        Dim x, z As Single
        x = Cos(l_rot) * (5 * 2)
        z = Sin(l_rot) * (5 * 2)

        position0(0) = x
        position0(1) = 10.0
        position0(2) = z

        tank_label.Text = ""
        SplitContainer1.SplitterDistance = 720
        SplitContainer2.SplitterDistance = SplitContainer2.Height - 160
        Application.DoEvents()
        Me.Width = 1280
        Me.Height = 720
        pb1.Visible = False
        iconbox.Visible = False
        Application.DoEvents()
        pb1.Visible = True
        frmState = Me.WindowState
        info_Label.BringToFront()
        info_Label.Parent = Me
        info_Label.Size = MM.Size
        info_Label.Dock = DockStyle.Top
        MM.Location = New Point(0, 0)
        info_Label.Text = "Welcome... Version: " + Application.ProductVersion
        Me.Text = " Tank Exporter Version: " + Application.ProductVersion
        '====================================================================================================
        start_up_log.AppendLine("------ App Startup ------")
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
        Me.Show()
        PB3.Parent = Me
        PB3.SendToBack()
        PB3.Visible = False
        ToolStripComboBox1.Visible = False
        ToolStripComboBox1.Text = My.Settings.region_selection
        decal_panel.Parent = SplitContainer2.Panel1
        decal_panel.Visible = False
        Application.DoEvents()

        'Check for temp storage folder.. It it exist.. load the API data.. 
        'other wise make the directory and get the API data.
        Temp_Storage = Path.GetTempPath ' this gets the user temp storage folder
        Temp_Storage += "wot_temp"
        If Not System.IO.Directory.Exists(Temp_Storage) Then
            System.IO.Directory.CreateDirectory(Temp_Storage)
        End If
        'fire up OpenGL amd IL
        start_up_log.AppendLine("Starting up OpenGL......")
        Il.ilInit()
        Ilu.iluInit()
        Ilut.ilutInit()
        EnableOpenGL()
        make_shadow_fbo()
        '---------------------------
        'just to convert to .te binary models;
        'load_and_save()

        Dim glstr As String
        glstr = Gl.glGetString(Gl.GL_VENDOR)
        start_up_log.AppendLine("Vendor: " + glstr)

        glstr = Gl.glGetString(Gl.GL_VERSION)
        start_up_log.AppendLine("Driver Version: " + glstr)

        glstr = Gl.glGetString(Gl.GL_SHADING_LANGUAGE_VERSION)
        start_up_log.AppendLine("Shader Version: " + glstr)

        'glstr = Gl.glGetString(Gl.GL_EXTENSIONS).Replace(" ", vbCrLf)
        'start_up_log.AppendLine("Extensions:" + vbCrLf + glstr)
        start_up_log.AppendLine("End OpenGL Information" + vbCrLf)
        start_up_log.AppendLine("OpenGL Startup Complete" + vbCrLf)

        start_up_log.AppendLine("Loading required data..")

        File.WriteAllText(Temp_Storage + "Startup_log.txt", start_up_log.ToString)

        load_type_images() ' get the tank type icons

        Application.DoEvents()
        '====================================================================================================
        _Started = True
        '====================================================================================================
        ' Setup loaction for tank data.. sucks to do it this way but UAC wont allow it any other way.
        TankListTempFolder = Temp_Storage + "\tanklist\"
        decal_path = Temp_Storage + "\decals"

        If Not System.IO.Directory.Exists(TankListTempFolder) Then
            System.IO.Directory.CreateDirectory(TankListTempFolder)
        End If
        If My.Settings.firstRun Then ' check for possible update to tank list.
            My.Settings.firstRun = False
            Dim ts = IO.File.ReadAllText(Application.StartupPath + "\tanks\tanknames.txt")
            File.WriteAllText(TankListTempFolder + "tanknames.txt", ts)
        End If
        '====================================================================================================
        If File.Exists(Temp_Storage + "\game_Path.txt") Then
            My.Settings.game_path = File.ReadAllText(Temp_Storage + "\game_Path.txt")
        Else
            If My.Settings.game_path = "" Then
                MsgBox("Game Location needs to be set.", MsgBoxStyle.Information)
                M_Path.PerformClick()
            End If
        End If
        If File.Exists(Temp_Storage + "\res_mods_path.txt") Then
            My.Settings.res_mods_path = File.ReadAllText(Temp_Storage + "\res_mods_Path.txt")
        Else
            If My.Settings.game_path = "C:\" Then
                MsgBox("res_mods Location needs to be set.", MsgBoxStyle.Information)
                m_res_mods_path.PerformClick()
            End If
        End If
        '====================================================================================================
        Dim testing_controls As Boolean = False
        If Not testing_controls Then

            info_Label.Text = "Loading Data from Packages..."
            Application.DoEvents()
            MM.Enabled = False ' Dont let the user click anything while we are loading data!
            TC1.Enabled = False
            Try

                gui_pkg = New Ionic.Zip.ZipFile(My.Settings.game_path + "\res\packages\gui.pkg")
                start_up_log.AppendLine("Loaded: " + My.Settings.game_path + "\res\packages\gui.pkg")

                scripts_pkg = New Ionic.Zip.ZipFile(My.Settings.game_path + "\res\packages\scripts.pkg")
                start_up_log.AppendLine("Loaded: " + My.Settings.game_path + "\res\packages\scripts.pkg")
                'packages(11) = ZipFile.Read(My.Settings.game_path + "\res\packages\shared_content.pkg")
                'packages(12) = ZipFile.Read(My.Settings.game_path + "\res\packages\shared_content_sandbox.pkg")
                'packages(11) = shared_pkg
                'packages(12) = shared_sandbox_pkg

            Catch ex As Exception
                MsgBox("I was unable to load required pkg files! Path Issue?", MsgBoxStyle.Exclamation, "Error!")
                My.Settings.game_path = ""
                My.Settings.res_mods_path = ""
                My.Settings.Save()
                End
            End Try
            '====================================================================================================
            'MsgBox("I LOADED required pkg files!", MsgBoxStyle.Exclamation, "Error!")
            'Try
            If File.Exists(Temp_Storage + "\shared_contents_build.pkg") Then
                packages(11) = ZipFile.Read(Temp_Storage + "\shared_contents_build.pkg")
                start_up_log.AppendLine("Loaded: " + Temp_Storage + "\shared_contents_build.pkg")
            Else
                '===================================================================================
                start_up_log.AppendLine("Finding all PBS decals in map pak files...")
                info_Label.Text = "finding Decals. This only happens once after install."
                find_pbs_decals()
                start_up_log.AppendLine("Done Finding all PBS decals in map packages.")
                '===================================================================================

                shared_contents_build = New ZipFile(Temp_Storage + "\shared_contents_build.pkg")
                start_up_log.AppendLine("shared_contents_build.pkg does not exist. Building shared_contents_build.pkg")
                start_up_log.AppendLine("Only Entries that contain Vehicle will be read.")
                'add handler for progression call back to display progressbar value
                AddHandler (shared_contents_build.SaveProgress), New EventHandler(Of SaveProgressEventArgs)(AddressOf save_progress)

                info_Label.Text = "Reading all shared content packages. This only needs to be done once."
                Application.DoEvents()
                Application.DoEvents()

                Dim z_path = Temp_Storage + "\zip"
                IO.Directory.CreateDirectory(z_path)
                info_Label.Text = "Reading shared_content-part1.pkg"
                Application.DoEvents()
                IO.Directory.CreateDirectory(decal_path)
                '================================================================================
                'part 1
                PG1.Visible = True
                PG1.Value = 0
                Dim cnt = 0
                Dim arc = ZipFile.Read(My.Settings.game_path + "\res\packages\shared_content-part1.pkg")
                PG1.Maximum = arc.Count
                start_up_log.AppendLine("reading: \res\packages\shared_content-part1.pkg")

                For Each entry In arc
                    PG1.Value = cnt
                    cnt += 1
                    If entry.FileName.ToLower.Contains("vehicle") Then
                        entry.Extract(z_path, ExtractExistingFileAction.OverwriteSilently)
                        Application.DoEvents()
                    End If
                Next
                cnt = 0
                Try
                    info_Label.Text = "getting decals from shared_content-part1.pkg"
                    For Each entry In arc
                        PG1.Value = cnt
                        cnt += 1
                        If entry.FileName.ToLower.Contains("decals_pbs") Then
                            entry.Extract(decal_path, ExtractExistingFileAction.OverwriteSilently)
                            Application.DoEvents()
                        End If
                    Next
                Catch ex As Exception
                End Try
                Try
                    info_Label.Text = "Reading shared_content_hd-part1.pkg"
                    Application.DoEvents()
                    arc = ZipFile.Read(My.Settings.game_path + "\res\packages\shared_content_hd-part1.pkg")
                    PG1.Value = 0
                    PG1.Maximum = arc.Count
                    cnt = 0
                    start_up_log.AppendLine("reading: \res\packages\shared_content_hd-part1.pkg")
                    For Each entry In arc
                        PG1.Value = cnt
                        cnt += 1
                        If entry.FileName.ToLower.Contains("vehicle") Then
                            entry.Extract(z_path, ExtractExistingFileAction.OverwriteSilently)
                            Application.DoEvents()
                        End If
                    Next
                Catch ex As Exception
                    start_up_log.AppendLine("Could not find: \res\packages\shared_content-part1.pkg")
                End Try
                '================================================================================
                'part 2
                info_Label.Text = "Reading shared_content-part2.pkg"
                Application.DoEvents()
                arc = ZipFile.Read(My.Settings.game_path + "\res\packages\shared_content-part2.pkg")
                PG1.Value = 0
                PG1.Maximum = arc.Count
                cnt = 0
                start_up_log.AppendLine("reading: \res\packages\shared_content-part2.pkg")
                For Each entry In arc
                    PG1.Value = cnt
                    cnt += 1
                    If entry.FileName.ToLower.Contains("vehicle") Then
                        entry.Extract(z_path, ExtractExistingFileAction.OverwriteSilently)
                        Application.DoEvents()
                    End If
                Next
                cnt = 0
                Try
                    info_Label.Text = "getting decals from shared_content_hd-part2.pkg"
                    For Each entry In arc
                        PG1.Value = cnt
                        cnt += 1
                        If entry.FileName.ToLower.Contains("decals_pbs") Then
                            entry.Extract(decal_path, ExtractExistingFileAction.OverwriteSilently)
                            Application.DoEvents()
                        End If
                    Next
                Catch ex As Exception
                End Try
                Try
                    info_Label.Text = "Reading shared_content_hd-part2.pkg"
                    Application.DoEvents()
                    arc = ZipFile.Read(My.Settings.game_path + "\res\packages\shared_content_hd-part2.pkg")
                    PG1.Value = 0
                    PG1.Maximum = arc.Count
                    cnt = 0
                    start_up_log.AppendLine("reading: \res\packages\shared_content_hd-part2.pkg")
                    For Each entry In arc
                        PG1.Value = cnt
                        cnt += 1
                        If entry.FileName.ToLower.Contains("vehicle") Then
                            entry.Extract(z_path, ExtractExistingFileAction.OverwriteSilently)
                            Application.DoEvents()
                        End If
                    Next
                Catch ex As Exception
                    start_up_log.AppendLine("Could not find: \res\packages\shared_content-part2.pkg")
                End Try
                '================================================================================
                'part 1
                info_Label.Text = "Reading shared_content_sandbox-part1.pkg"
                Application.DoEvents()
                arc = ZipFile.Read(My.Settings.game_path + "\res\packages\shared_content_sandbox-part1.pkg")
                PG1.Value = 0
                PG1.Maximum = arc.Count
                cnt = 0
                start_up_log.AppendLine("reading: \res\packages\shared_content_sandbox-part1.pkg")
                For Each entry In arc
                    PG1.Value = cnt
                    cnt += 1
                    If entry.FileName.ToLower.Contains("vehicle") Then
                        entry.Extract(z_path, ExtractExistingFileAction.OverwriteSilently)
                        Application.DoEvents()
                    End If
                Next
                Try
                    info_Label.Text = "Reading shared_content_sandbox_hd-part1.pkg"
                    Application.DoEvents()
                    arc = ZipFile.Read(My.Settings.game_path + "\res\packages\shared_content_sandbox_hd-part1.pkg")
                    PG1.Value = 0
                    PG1.Maximum = arc.Count
                    cnt = 0
                    start_up_log.AppendLine("reading: \res\packages\shared_content_sandbox_hd-part1.pkg")
                    For Each entry In arc
                        PG1.Value = cnt
                        cnt += 1
                        If entry.FileName.ToLower.Contains("vehicle") Then
                            entry.Extract(z_path, ExtractExistingFileAction.OverwriteSilently)
                            Application.DoEvents()
                        End If
                    Next
                Catch ex As Exception
                    start_up_log.AppendLine("Could not find: \res\packages\shared_content_sandbox_hd-part1.pkg")
                End Try
                '================================================================================
                'part 2
                info_Label.Text = "Reading shared_content_sandbox-part2.pkg"
                Application.DoEvents()
                arc = ZipFile.Read(My.Settings.game_path + "\res\packages\shared_content_sandbox-part2.pkg")
                PG1.Value = 0
                PG1.Maximum = arc.Count
                cnt = 0
                start_up_log.AppendLine("reading: \res\packages\shared_content_sandbox-part2.pkg")
                For Each entry In arc
                    PG1.Value = cnt
                    cnt += 1
                    If entry.FileName.ToLower.Contains("vehicle") Then
                        entry.Extract(z_path, ExtractExistingFileAction.OverwriteSilently)
                        Application.DoEvents()
                    End If
                Next
                Try
                    info_Label.Text = "Reading shared_content_sandbox_hd-part2.pkg"
                    Application.DoEvents()
                    arc = ZipFile.Read(My.Settings.game_path + "\res\packages\shared_content_sandbox_hd-part2.pkg")
                    PG1.Value = 0
                    PG1.Maximum = arc.Count
                    cnt = 0
                    start_up_log.AppendLine("reading: \res\packages\shared_content_sandbox_hd-part2.pkg")
                    For Each entry In arc
                        PG1.Value = cnt
                        cnt += 1
                        If entry.FileName.ToLower.Contains("vehicle") Then
                            entry.Extract(z_path, ExtractExistingFileAction.OverwriteSilently)
                            Application.DoEvents()
                        End If
                    Next
                Catch ex As Exception
                    start_up_log.AppendLine("Could not find: \res\packages\shared_content_sandbox_hd-part2.pkg")
                End Try
                '================================================================================

                shared_contents_build.AddDirectory(z_path)

                GC.Collect()
                GC.WaitForFullGCComplete()
                shared_contents_build.CompressionLevel = 0 ' no compression
                shared_contents_build.ParallelDeflateThreshold = 0
                info_Label.Text = "Saving " + shared_contents_build.Entries.Count.ToString + " files to shared_contents_build.pkg.. This will take a long time!"
                start_up_log.AppendLine("Saving: " + Temp_Storage + "\shared_contents_build.pkg")
                Application.DoEvents()
                shared_contents_build.Save()
                packages(11) = New ZipFile
                packages(11) = shared_contents_build ' save this in to 11th position
            End If
            'Catch ex As Exception
            '    start_up_log.AppendLine("Something went very wrong creating the shared_contents_build.pkg!")
            'End Try
            screen_totaled_draw_time = 1 ' to stop divide by zero exception
            If Directory.Exists(Temp_Storage + "\zip") Then
                System.IO.Directory.Delete(Temp_Storage + "\zip", True)
            End If
            Application.DoEvents()
            '====================================================================================================
            '====================================================================================================
            tank_label.Parent = iconbox
            tank_label.Text = ""
            tank_label.Location = New Point(5, 10)
            '===================================================================================
            info_Label.Text = "Getting Camo Textures..."
            'load_camo()
            '===================================================================================
            load_customization_files()
            'MsgBox("Past load_customization_files", MsgBoxStyle.Exclamation, "Debug")
            load_season_icons()
            load_tank_buttons()
            start_up_log.AppendLine("Done Creating OpenGL based Buttons.")

            Gl.glFinish()
            '===================================================================================

            If Not File.Exists(Temp_Storage + "\in_shortnames.txt") Then
                start_up_log.AppendLine("Getting DEV API data.")
                get_tank_names()
            Else
                get_tank_info_from_temp_folder()
                start_up_log.AppendLine("Data already read from DEV API.. Loaded it..")
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
            load_tabs()
            Application.DoEvents()
        End If


        TC1.SelectedIndex = 0
        Application.DoEvents()
        make_shaders() 'compile the shaders
        set_shader_variables() ' update uniform addresses
        TC1.SelectedIndex = 0
        'clean up the grabage
        GC.Collect()
        GC.WaitForFullGCComplete()
        Me.KeyPreview = True    'so i catch keyboard before despatching it
        w_changing = False

        'make table used for repacking 888 normal uint32 in converstion
        make_888_lookup_table()
        'load skybox model

        make_xy_grid()
        start_up_log.AppendLine("Done Creating XY Grid Display List.")

        grid_cb.Checked = False
        '===================================================================================
        info_Label.Text = "loading terrain, textures, creating shadow texture, ect..."
        Application.DoEvents()
        load_resources()
        info_Label.Visible = False
        '###################################
        start_up_log.AppendLine("----- Startup Complete -----")
        File.WriteAllText(Temp_Storage + "Startup_log.txt", start_up_log.ToString)
        'show and hide to assign setting
        FrmShadowSettings.Show() ' set the buttns and shadow quality
        FrmShadowSettings.Hide()
        frmLighting.Show()
        frmLighting.Hide()
        frmLightSelection.Show()
        frmLightSelection.Hide()
        '###################################
        MM.Enabled = True
        TC1.Enabled = True
        '###################################
        pick_timer.Start()


        cam_x = 0
        cam_y = 0
        cam_z = 10
        look_point_x = 0
        look_point_y = 0
        look_point_z = 0
        Cam_X_angle = PI * 0.25
        Cam_Y_angle = -PI * 0.25
        view_radius = -10.0
        l_rot = PI * 0.25 + PI * 2


        Startup_Timer.Enabled = True
        Application.DoEvents()
        AddHandler Me.SizeChanged, AddressOf me_size_changed
        window_state = Me.WindowState


    End Sub
    '############################################################################ form load

    Private Sub find_pbs_decals()
        Dim iPath = My.Settings.game_path + "\res\packages\"
        Dim f_info = Directory.GetFiles(iPath)
        Dim maps(100) As String
        Dim cnt As Integer = 0
        'first, lets get a list of all the map files.
        For Each m In f_info
            If Not m.Contains("_hd") And Not m.Contains("vehicles_") Then
                Dim s = Path.GetFileNameWithoutExtension(m)
                Dim ta = s.Split("_")
                If IsNumeric(ta(0)) Then 'If the file name as a number at the start, it's a map file!
                    maps(cnt) = m
                    cnt += 1
                End If
            End If
        Next
        ReDim Preserve maps(cnt - 1)
        'now lets search each map file for decals_pbs
        Dim oPath = Temp_Storage + "\decals\"
        For i = 0 To cnt - 1
            Using z As New ZipFile(maps(i))
                For Each item In z
                    If item.FileName.Contains("decals_pbs") _
                And Not item.FileName.ToLower.Contains("snow") Then
                        item.Extract(oPath, ExtractExistingFileAction.OverwriteSilently)
                    End If
                Next
            End Using
            GC.Collect() 'clean up trash to free memory!
        Next
        load_decal_textures()


    End Sub

    Private Sub load_resources()
        Dim t As New Stopwatch
        info_Label.Text = "loading Environment models"
        start_up_log.AppendLine("loading models..")
        Application.DoEvents()
        load_models()
        Dim tt = t.ElapsedMilliseconds.ToString
        start_up_log.AppendLine("T = " + tt + "ms")
        t.Restart()
        Dim iPath As String = Application.StartupPath + "\resources\models\"
        '==========
        info_Label.Text = "loading Environment textures"
        start_up_log.AppendLine("loading Env textures...")
        Application.DoEvents()
        load_cube_and_cube_map()
        '==========
        gradient_lookup_id = load_png_file(iPath + "borderGradient.png")
        dome_textureId = load_png_file(iPath + "dome.png")
        load_terrain()
        tt = t.ElapsedMilliseconds.ToString
        start_up_log.AppendLine("T = " + tt + "ms")
        t.Restart()
        '==========
        info_Label.Text = "loading Upton control"
        start_up_log.AppendLine("loading Upton....")
        Application.DoEvents()
        upton.load_upton()
        tt = t.ElapsedMilliseconds.ToString
        start_up_log.AppendLine("T = " + tt + "ms")
        t.Restart()
        '==========
        info_Label.Text = "loading Decal textures"
        start_up_log.AppendLine("loading Decal Textures")
        Application.DoEvents()
        load_decal_textures()
        tt = t.ElapsedMilliseconds.ToString
        start_up_log.AppendLine("T = " + tt + "ms")
        t.Restart()
        '==========
        info_Label.Text = "loading Decal layout"
        start_up_log.AppendLine("loading Decal Layout")
        Application.DoEvents()
        load_decal_data()
        tt = t.ElapsedMilliseconds.ToString
        start_up_log.AppendLine("T = " + tt + "ms")
        '==========
        t.Stop()
    End Sub
    Private Sub load_terrain()
        Dim iPath As String = Application.StartupPath + "\resources\models\"
        'terrain_modelId = get_X_model(iPath + "terrain.x") '===========================
        terrain_textureId = load_png_file(iPath + "surface.png")
        terrain_textureNormalId = load_png_file(iPath + "surface_NORM.png")
        terrain_noise_id = load_png_file(iPath + "noise.png")
    End Sub
    Private Sub load_cube_and_cube_map()
        Dim iPath As String = Application.StartupPath + "\resources\cube\cubemap_m00_c0"
        Dim id, iler, w, h As Integer

        Gl.glEnable(Gl.GL_TEXTURE_CUBE_MAP)
        Gl.glGenTextures(1, cube_texture_id)
        Gl.glBindTexture(Gl.GL_TEXTURE_CUBE_MAP, cube_texture_id)

        id = Il.ilGenImage
        Il.ilBindImage(id)

        For i = 0 To 5
            Dim ok = Il.ilLoad(Il.IL_PNG, iPath + i.ToString + ".png")
            iler = Il.ilGetError
            If iler = Il.IL_NO_ERROR Then
                Il.ilConvertImage(Il.IL_RGB, Il.IL_UNSIGNED_BYTE) ' Convert every colour component into unsigned bytes
                w = Il.ilGetInteger(Il.IL_IMAGE_WIDTH)
                h = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT)

                ' Gl.glTexParameteri(Gl.GL_TEXTURE_CUBE_MAP, Gl.GL_GENERATE_MIPMAP, Gl.GL_TRUE)


                Gl.glTexImage2D(Gl.GL_TEXTURE_CUBE_MAP_POSITIVE_X + i, 0, Gl.GL_RGB8, w, h, 0, Gl.GL_RGB, Gl.GL_UNSIGNED_BYTE, Il.ilGetData())
                Glu.gluBuild2DMipmaps(Gl.GL_TEXTURE_CUBE_MAP_POSITIVE_X + i, Gl.GL_RGB8, w, h, Gl.GL_RGB, Gl.GL_UNSIGNED_BYTE, Il.ilGetData())
            Else
                MsgBox("Can't load cube textures!", MsgBoxStyle.Exclamation, "Shit!")
            End If

        Next
        Il.ilBindImage(0)
        Il.ilDeleteImage(id)
        Gl.glTexParameteri(Gl.GL_TEXTURE_CUBE_MAP, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR)
        Gl.glTexParameteri(Gl.GL_TEXTURE_CUBE_MAP, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR_MIPMAP_LINEAR)

        Gl.glTexParameteri(Gl.GL_TEXTURE_CUBE_MAP, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP_TO_EDGE)
        Gl.glTexParameteri(Gl.GL_TEXTURE_CUBE_MAP, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP_TO_EDGE)
        Gl.glTexParameteri(Gl.GL_TEXTURE_CUBE_MAP, Gl.GL_TEXTURE_WRAP_R, Gl.GL_CLAMP_TO_EDGE)

        Gl.glBindTexture(Gl.GL_TEXTURE_CUBE_MAP, 0)
        Gl.glDisable(Gl.GL_TEXTURE_CUBE_MAP)

        Gl.glGenTextures(1, u_brdfLUT)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, u_brdfLUT)

        id = Il.ilGenImage
        Il.ilBindImage(id)

        Il.ilLoad(Il.IL_PNG, Application.StartupPath + "\resources\cube\env_brdf_lut.png")
        iler = Il.ilGetError
        If iler = Il.IL_NO_ERROR Then
            Il.ilConvertImage(Il.IL_RGBA, Il.IL_UNSIGNED_BYTE) ' Convert every colour component into unsigned bytes
            w = Il.ilGetInteger(Il.IL_IMAGE_WIDTH)
            h = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT)

            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR)

            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT)
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA8, w, h, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, Il.ilGetData())
        Else
            MsgBox("Can't load cube textures!", MsgBoxStyle.Exclamation, "Shit!")
        End If
        'clean up
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        id = Il.ilGenImage
        Il.ilBindImage(id)
        'get the cube model
    End Sub
    Private Sub load_models()
        load_binary_models()
    End Sub


    Private Sub load_type_images()
        tank_mini_icons.ColorDepth = ColorDepth.Depth32Bit
        tank_mini_icons.ImageSize = New Size(17, 17)
        Dim sp = Application.StartupPath + "\icons\"

        Dim e = (sp + "heavyTank.png")
        tank_mini_icons.Images.Add(load_type_icon(e))
        tank_mini_icons.Images.Add(load_type_icon(e))

        e = (sp + "mediumTank.png")
        tank_mini_icons.Images.Add(load_type_icon(e))
        tank_mini_icons.Images.Add(load_type_icon(e))

        e = (sp + "lightTank.png")
        tank_mini_icons.Images.Add(load_type_icon(e))
        tank_mini_icons.Images.Add(load_type_icon(e))

        e = (sp + "AT-SPG.png")
        tank_mini_icons.Images.Add(load_type_icon(e))
        tank_mini_icons.Images.Add(load_type_icon(e))

        e = (sp + "SPG.png")
        tank_mini_icons.Images.Add(load_type_icon(e))
        tank_mini_icons.Images.Add(load_type_icon(e))



    End Sub
    Private Function load_type_icon(path As String) As Image
        Dim b = New Bitmap(path)
        Return b

    End Function

    Private Sub save_progress(ByVal sender As Object, ByVal e As SaveProgressEventArgs)
        If e.EventType = Ionic.Zip.ZipProgressEventType.Saving_BeforeWriteEntry Then
            PG1.Visible = True
            PG1.Maximum = e.EntriesTotal
            PG1.Value = e.EntriesSaved + 1
            Application.DoEvents()
        End If
        If e.EventType = ZipProgressEventType.Saving_Completed Then
            PG1.Visible = False
        End If
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
                        Case "italy"
                            index = 10
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
        If MsgBox("This will clean out all temp folder data!!" + vbCrLf + _
                    "Also this will close the application because it can not run with out" + vbCrLf + _
                    "the data." + vbCrLf + _
                    "This only needs to be done if there was an update to the tank data." + vbCrLf + _
                    "It will force a reload of all data and the long delay creating the shared file." + vbCrLf + _
                    "Would you like to continue?", MsgBoxStyle.YesNo, "Warning..") = MsgBoxResult.No Then
            Return
        End If
        Dim f As DirectoryInfo = New DirectoryInfo(Temp_Storage)
        shared_contents_build.Dispose()
        packages(11).Dispose()
        GC.Collect()
        GC.WaitForFullGCComplete()
        If f.Exists Then
            For Each fi In f.GetFiles
                fi.Delete()
            Next
        End If
        Try
            f.Delete()
        Catch ex As Exception
        End Try
        Application.Exit()
    End Sub

    Private Sub draw_background()
        'Gdi.SwapBuffers(pb1_hDC)
        If Not (Wgl.wglMakeCurrent(pb1_hDC, pb1_hRC)) Then
            MessageBox.Show("Unable to make rendering context current")
            End
        End If
        Dim w, h As Integer
        G_Buffer.getsize(w, h)
        ResizeGL(w, h)
        ViewPerspective(w, h)
        ViewOrtho()
        Dim e = Gl.glGetError
        Dim sw! = pb1.Width
        Dim sh! = pb1.Height
        Dim p As New Point(0.0!, 0.0!)
        'Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
        e = Gl.glGetError

        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, Background_image_id)
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
        start_up_log.AppendLine("Creating TreeView :" + st_index.ToString("00"))
        tv = New mytreeview
        tv.Font = font_holder.Font.Clone
        tv.ContextMenuStrip = conMenu
        tv.DrawMode = TreeViewDrawMode.OwnerDrawText
        tv.ImageList = tank_mini_icons
        tv.Dock = DockStyle.Fill
        tv.Nodes.Clear()
        tv.BackColor = Color.DimGray
        tv.ForeColor = Color.Black
        tv.HotTracking = False
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

        'see if there is a exclusionmask in this file.. means its the old SD style tank.
        For Each exclusion In docx.Descendants("exclusionMask")
            exclusionMask_id = -1
            Dim exclu = doc.CreateElement("exclusionMask")
            Dim excluName = doc.CreateElement("name")
            excluName.InnerText = exclusion.Value.ToString.Replace("/", "\")
            If excluName.InnerText.Length > 2 And excluName.InnerText.ToLower.Contains("_cm") Then
                GLOBAL_exclusionMask = 1
                exclu.AppendChild(excluName)
                root_node.AppendChild(exclu)
            End If
        Next



        Dim turret_tiling = doc.CreateElement("turret_tiling")
        Dim found_camo As Boolean = False
        For Each turret0 As XElement In docx.Descendants("turrets0")
            For Each model In turret0.Descendants("undamaged")
                If model.Value.ToLower.ToLower.Contains("turret_") Then

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
                If models.Value.ToString.ToLower.Contains("turret_") Then
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
                store_in_treeview(i, treeviews(i))
                If i > 4 Then
                    store_in_treeview_1(i, treeviews(i))
                End If
                Application.DoEvents()
            Next
            get_tanks_shared()
            'add count to log
            start_up_log.AppendLine("Total Tanks Found:" + TOTAL_TANKS_FOUND.ToString("000"))
            'get_tanks_sandbox()
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
    '================================================================================= Store in treeview
    Private Sub store_in_treeview(ByVal i As Integer, ByRef tn As TreeView)
        AddHandler tn.NodeMouseClick, AddressOf Me.tv_clicked
        AddHandler tn.NodeMouseHover, AddressOf Me.tv_mouse_enter
        AddHandler tn.MouseLeave, AddressOf Me.tv_mouse_leave
        tn.BackColor = Color.DimGray
        tn.CheckBoxes = False
        tn.ItemHeight = 17
        tn.HotTracking = True
        tn.ShowRootLines = False
        tn.ShowLines = False
        tn.Margin = New Padding(0)
        tn.BorderStyle = BorderStyle.None
        Dim ext As String = "-part1.pkg"
        If i < 5 Then
            ext = ".pkg"
        End If
        Dim cnt As Integer = 0
        Dim fpath = My.Settings.game_path + "/res/packages/vehicles_level_" + i.ToString("00") + ext
        If File.Exists(fpath) Then
            packages(i) = Ionic.Zip.ZipFile.Read(fpath)
            start_up_log.AppendLine("Getting Tank data from: " + fpath)
        Else
            start_up_log.AppendLine("Could not find: " + fpath)
            Return
        End If
        Dim fpath_1 = My.Settings.game_path + "\res\packages\vehicles_level_" + i.ToString("00") + "_hd" + ext
        If File.Exists(fpath_1) Then
            packages_HD(i) = Ionic.Zip.ZipFile.Read(fpath_1)
            start_up_log.AppendLine("Getting Tank data from: " + fpath_1)
        Else
            'todo
        End If

        get_tank_info_by_tier(i.ToString)
        ReDim node_list(i).item(tier_list.Length)
        ReDim icons(i).img(tier_list.Length)

        For Each t In tier_list
            Dim n As New TreeNode
            Select Case t.type ' icon types
                Case "Heavy"
                    n.SelectedImageIndex = 0
                    n.StateImageIndex = 0
                    n.ImageIndex = 0
                Case "Medium"
                    n.SelectedImageIndex = 2
                    n.StateImageIndex = 2
                    n.ImageIndex = 2
                Case "Light"
                    n.SelectedImageIndex = 4
                    n.StateImageIndex = 4
                    n.ImageIndex = 4
                Case "Destoryer"
                    n.SelectedImageIndex = 6
                    n.StateImageIndex = 6
                    n.ImageIndex = 6
                Case "Artillary"
                    n.SelectedImageIndex = 8
                    n.StateImageIndex = 8
                    n.ImageIndex = 8

            End Select
            n.Name = t.nation
            n.Text = t.tag
            n.Tag = fpath + ":" + "vehicles/" + get_nation(t.nation) + "/" + t.tag
            node_list(i).item(cnt).name = t.tag
            node_list(i).item(cnt).node = n
            node_list(i).item(cnt).package = packages(i).Name
            icons(i).img(cnt) = get_tank_icon(n.Text).Clone
            If icons(i).img(cnt) IsNot Nothing Then
                node_list(i).item(cnt).icon = icons(i).img(cnt).Clone
                node_list(i).item(cnt).icon.Tag = current_png_path
                cnt += 1
                TOTAL_TANKS_FOUND += 1
            Else
                start_up_log.AppendLine("!!!!! Missing Tank Icon PNG !!!!! :" + current_png_path)
            End If
        Next
        ReDim Preserve node_list(i).item(cnt)

        Application.DoEvents()
        ReDim Preserve icons(i).img(cnt)
        Application.DoEvents()
        tn.Tag = i

    End Sub

    Private Sub store_in_treeview_1(ByVal i As Integer, ByRef tn As TreeView)
        'AddHandler tn.NodeMouseClick, AddressOf Me.tv_clicked
        'AddHandler tn.NodeMouseHover, AddressOf Me.tv_mouse_enter
        'AddHandler tn.MouseLeave, AddressOf Me.tv_mouse_leave
        'tn.BackColor = Color.DimGray
        'tn.CheckBoxes = False
        'tn.ItemHeight = 17
        'tn.HotTracking = True
        'tn.ShowRootLines = False
        'tn.ShowLines = False
        'tn.Margin = New Padding(0)
        'tn.BorderStyle = BorderStyle.None
        Dim cnt As Integer = 0
        Dim fpath = My.Settings.game_path + "/res/packages/vehicles_level_" + i.ToString("00") + "-part2.pkg"
        If File.Exists(fpath) Then
            packages_2(i) = Ionic.Zip.ZipFile.Read(fpath)
            start_up_log.AppendLine("Getting Tank data from: " + fpath)
        Else
            start_up_log.AppendLine("Could not find: " + fpath)
            Return
        End If
        Dim fpath_1 = My.Settings.game_path + "\res\packages\vehicles_level_" + i.ToString("00") + "_hd-part2.pkg"
        If File.Exists(fpath_1) Then
            packages_HD_2(i) = Ionic.Zip.ZipFile.Read(fpath_1)
            start_up_log.AppendLine("Getting Tank data from: " + fpath_1)
        Else
            'todo
        End If

        get_tank_info_by_tier(i.ToString)
        ReDim node_list(i).item(tier_list.Length)
        ReDim icons(i).img(tier_list.Length)

        For Each t In tier_list
            Dim n As New TreeNode
            Select Case t.type ' icon types
                Case "Heavy"
                    n.SelectedImageIndex = 0
                    n.StateImageIndex = 0
                    n.ImageIndex = 0
                Case "Medium"
                    n.SelectedImageIndex = 2
                    n.StateImageIndex = 2
                    n.ImageIndex = 2
                Case "Light"
                    n.SelectedImageIndex = 4
                    n.StateImageIndex = 4
                    n.ImageIndex = 4
                Case "Destoryer"
                    n.SelectedImageIndex = 6
                    n.StateImageIndex = 6
                    n.ImageIndex = 6
                Case "Artillary"
                    n.SelectedImageIndex = 8
                    n.StateImageIndex = 8
                    n.ImageIndex = 8

            End Select
            n.Name = t.nation
            n.Text = t.tag
            n.Tag = fpath + ":" + "vehicles/" + get_nation(t.nation) + "/" + t.tag
            node_list(i).item(cnt).name = t.tag
            node_list(i).item(cnt).node = n
            node_list(i).item(cnt).package = packages_2(i).Name
            icons(i).img(cnt) = get_tank_icon(n.Text).Clone
            If icons(i).img(cnt) IsNot Nothing Then
                node_list(i).item(cnt).icon = icons(i).img(cnt).Clone
                node_list(i).item(cnt).icon.Tag = current_png_path
                cnt += 1
                TOTAL_TANKS_FOUND += 1
            Else
                start_up_log.AppendLine("!!!!! Missing Tank Icon PNG !!!!! :" + current_png_path)
            End If
        Next
        ReDim Preserve node_list(i).item(cnt)

        Application.DoEvents()
        ReDim Preserve icons(i).img(cnt)
        Application.DoEvents()
        tn.Tag = i

    End Sub

    Private Function get_nation(ByVal n As String) As String
        Select Case n
            Case "usa"
                Return "american"
            Case "uk"
                Return "british"
            Case "china"
                Return "chinese"
            Case "czech"
                Return "czech"
            Case "france"
                Return "french"
            Case "germany"
                Return "german"
            Case "japan"
                Return "japan"
            Case "poland"
                Return "poland"
            Case "ussr"
                Return "russian"
            Case "sweden"
                Return "sweden"
            Case "italy"
                Return "italy"
        End Select
        Return "who knows what lurks in the minds of men"
    End Function




    Private Sub get_tanks_shared()
        For Each entry As ZipEntry In packages(11)
            If entry.FileName.Contains("collision_client/Chassis.model") Then
                Dim t_name = entry.FileName
                Dim ta = t_name.Split("/")
                t_name = ""
                For j = 0 To 2
                    t_name += ta(j) + "/"
                Next
                Dim n As New TreeNode
                n.Text = ta(2)
                n.Tag = My.Settings.game_path + "\res\packages\shared_content_build.pkg" + ":" + t_name
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
                    TOTAL_TANKS_FOUND += 1
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
    'Private Sub get_tanks_sandbox()
    '    For Each entry As ZipEntry In packages(11)
    '        If entry.FileName.Contains("normal/lod0/Chassis.model") Then
    '            Dim t_name = entry.FileName
    '            Dim ta = t_name.Split("/")
    '            t_name = ""
    '            For j = 0 To 2
    '                t_name += ta(j) + "/"
    '            Next
    '            Dim n As New TreeNode
    '            n.Text = ta(2)
    '            n.Tag = My.Settings.game_path + "\res\packages\shared_content_sandbox.pkg" + ":" + t_name
    '            'need this to look up actual tanks game name in the
    '            '\res\packages\scripts.pkg\scripts\item_defs\vehicles\***poland***\list.xml
    '            Dim s As String = ""
    '            Dim i As Integer = 0
    '            Select Case ta(1)
    '                Case "american"
    '                    n.Name = "usa"
    '                    s = get_user_name(n.Text)
    '                    i = CInt(get_tier_id(n.Text))
    '                Case "british"
    '                    n.Name = "uk"
    '                    s = get_user_name(n.Text)
    '                    i = CInt(get_tier_id(n.Text))
    '                Case "chinese"
    '                    n.Name = "china"
    '                    s = get_user_name(n.Text)
    '                    i = CInt(get_tier_id(n.Text))
    '                Case "czech"
    '                    n.Name = "czech"
    '                    s = get_user_name(n.Text)
    '                    i = CInt(get_tier_id(n.Text))
    '                Case "french"
    '                    n.Name = "france"
    '                    s = get_user_name(n.Text)
    '                    i = CInt(get_tier_id(n.Text))
    '                Case "german"
    '                    n.Name = "germany"
    '                    s = get_user_name(n.Text)
    '                    i = CInt(get_tier_id(n.Text))
    '                Case "japan"
    '                    n.Name = "japan"
    '                    s = get_user_name(n.Text)
    '                    i = CInt(get_tier_id(n.Text))
    '                Case "poland"
    '                    n.Name = "poland"
    '                    s = get_user_name(n.Text)
    '                    i = CInt(get_tier_id(n.Text))
    '                Case "russian"
    '                    n.Name = "ussr"
    '                    s = get_user_name(n.Text)
    '                    i = CInt(get_tier_id(n.Text))
    '                Case "sweden"
    '                    n.Name = "sweden"
    '                    s = get_user_name(n.Text)
    '                    i = CInt(get_tier_id(n.Text))
    '            End Select
    '            If s.Length > 0 Then ' only save what actually exist
    '                Dim cnt As Integer = node_list(i).item.Length
    '                ReDim Preserve node_list(i).item(cnt)
    '                ReDim Preserve icons(i).img(cnt)
    '                cnt -= 1
    '                node_list(i).item(cnt) = New t_items_
    '                Dim na = n.Text.Split("_")
    '                If na(0).Length = 3 Then
    '                    na(0) += "99"
    '                End If
    '                node_list(i).item(cnt).name = na(0)
    '                node_list(i).item(cnt).node = n
    '                node_list(i).item(cnt).package = My.Settings.game_path + "\res\packages\shared_content_sandbox.pkg"
    '                icons(i).img(cnt) = get_tank_icon(n.Text).Clone
    '                If icons(i).img(cnt) IsNot Nothing Then
    '                    node_list(i).item(cnt).icon = icons(i).img(cnt).Clone
    '                    node_list(i).item(cnt).icon.Tag = current_png_path
    '                    'tn.Nodes.Add(n)
    '                End If
    '            End If
    '        End If
    '    Next

    '    Application.DoEvents()
    'End Sub
    Private Sub get_tank_info_by_tier(ByVal t As String)
        ReDim tier_list(200)
        Dim count As Integer = 0
        Try

            Dim q = From row In TankDataTable _
                        Where row.Field(Of String)("tier") = t _
                Select _
                    un = row.Field(Of String)("shortname"), _
                    tag = row.Field(Of String)("tag"), _
                    nation = row.Field(Of String)("nation"), _
                    type = row.Field(Of String)("type") _
                        Order By nation Descending

            'Dim a = q(0).un.Split(":")
            For Each item In q
                tier_list(count).tag = item.tag
                tier_list(count).username = item.un
                tier_list(count).nation = item.nation
                tier_list(count).tier = t
                tier_list(count).type = item.type
                count += 1
            Next
            ReDim Preserve tier_list(count - 1)

        Catch ex As Exception
            Return
        End Try
        Return


    End Sub
    Private Function get_user_name(ByVal fname As String) As String
        If fname.ToLower.Contains("progetto_m35") Then
            Return "Progetto M35 mod 46"
        End If
        Try

            Dim q = From row In TankDataTable _
                        Where row.Field(Of String)("tag") = fname _
                Select _
                    un = row.Field(Of String)("shortname"), _
                    tier = row.Field(Of String)("tier"), _
                    natiom = row.Field(Of String)("nation") _
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
        If n.Text.ToLower.Contains("progetto_m35") Then
            Return "Progetto M35 mod 46"
        End If

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
            If entry.FileName.Contains(name) And entry.FileName.Contains("/icons/vehicle/") _
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

    '###########################################################################################################################################
    Dim tv As Single
    Private Sub draw_environment()
        '############################################
        G_Buffer.attachColor_And_NormalTexture()
        Dim t = time.ElapsedMilliseconds
        If CSng(t) > 5000 Then
            t = 0.0!
        End If
        If t = 0.0! Then
            time.Restart()
        End If
        tv = CSng(t) / 5000.0!
        'Dome
        '############################################
        'dome
        Dim s As Single = 1.0
        Gl.glFrontFace(Gl.GL_CCW)
        'Gl.glDisable(Gl.GL_DEPTH_TEST)
        Gl.glDisable(Gl.GL_CULL_FACE)
        Gl.glDisable(Gl.GL_BLEND)

        Gl.glUseProgram(shader_list.dome_shader)
        Gl.glUniform1i(dome_colorMap, 0)

        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, dome_textureId)

        Gl.glPushMatrix()
        Gl.glScalef(s, s, s)
        'Gl.glTranslatef(0.0, -4.0, 0.0)
        Gl.glColor3f(1.0, 1.0, 1.0)
        Gl.glDisable(Gl.GL_TEXTURE_2D)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, dome_textureId)
        Gl.glCallList(dome_modelId)
        Gl.glPopMatrix()
        Gl.glEnable(Gl.GL_DEPTH_TEST)
        Gl.glUseProgram(0)

        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '0
        '############################################
        'Terrain
        G_Buffer.attachColor_And_Normal_FOG_Texture()
        Gl.glPushMatrix()
        'Gl.glTranslatef(0.0, -0.06, 0.0)
        'Gl.glRotatef(0.25, -1.0, 0.0, 1.0)
        Gl.glDisable(Gl.GL_CULL_FACE)
        Gl.glFrontFace(Gl.GL_CCW)
        Gl.glEnable(Gl.GL_BLEND)
        Gl.glUseProgram(shader_list.terrainShader_shader)
        Gl.glUniform1i(terrain_textureMap, 0)
        Gl.glUniform1i(terrain_depthMap, 1)
        Gl.glUniform1i(terrain_normalMap, 2)
        Gl.glUniform1i(terrain_gradient, 3)
        Gl.glUniform1i(terrain_noise, 4)
        Gl.glUniform1f(terain_animation, tv)
        Gl.glUniform3f(terrain_camPosition, eyeX, eyeY, eyeZ)

        Gl.glUniformMatrix4fv(terrain_shadowProjection, 1, 0, lightProjection)
        If m_shadows.Checked Then
            Gl.glUniform1i(terrain_use_shadow, 1)
        Else
            Gl.glUniform1i(terrain_use_shadow, 0)
        End If
        Gl.glColor4f(1.0, 1.0, 1.0, 1.0)
        Gl.glEnable(Gl.GL_TEXTURE_2D)
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, terrain_textureId)
        'Gl.glBindTexture(Gl.GL_TEXTURE_2D, decal_textures(5).colorMap_Id)
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, depthBuffer)
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 2)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, terrain_textureNormalId)
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 3)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, gradient_lookup_id)
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 4)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, terrain_noise_id)

        Gl.glCallList(terrain_modelId)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        'Gl.glDisable(Gl.GL_TEXTURE_2D)

        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '4
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 3)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '3
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 2)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '2
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '1
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '0
        Gl.glUseProgram(0)
        Gl.glPopMatrix()
        Gl.glDisable(Gl.GL_BLEND)
        G_Buffer.attachColorTexture()

    End Sub
    '###########################################################################################################################################
    'decals
    Private Sub draw_decals()
        Dim w, h As Integer
        Dim l_array(8) As Single

        l_array(0) = position0(0)
        l_array(1) = position0(1)
        l_array(2) = position0(2)

        l_array(3) = position1(0)
        l_array(4) = position1(1)
        l_array(5) = position1(2)

        l_array(6) = position2(0)
        l_array(7) = position2(1)
        l_array(8) = position2(2)


        G_Buffer.getsize(w, h)
        If current_decal > -1 Then
            G_Buffer.get_depth_buffer(w, h) ' get depth in to gDepth

            Gl.glFrontFace(Gl.GL_CW)
            Gl.glEnable(Gl.GL_CULL_FACE)
            Gl.glDisable(Gl.GL_DEPTH_TEST)
            'Gl.glDisable(Gl.GL_LIGHTING)

            Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_FILL)

            'Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE)
            Gl.glDisable(Gl.GL_CULL_FACE)
            Gl.glDisable(Gl.GL_TEXTURE_2D)

            Gl.glEnable(Gl.GL_BLEND)
            Gl.glBlendEquationSeparate(Gl.GL_FUNC_ADD, Gl.GL_FUNC_ADD)
            Gl.glBlendFuncSeparate(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA, Gl.GL_ONE, Gl.GL_ONE_MINUS_SRC_ALPHA)
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA)


            Gl.glDepthMask(Gl.GL_FALSE)

            Gl.glUseProgram(shader_list.decalsCpass_shader)
            If m_shadows.Checked Then
                Gl.glUniform1i(decalC_use_shadow, 1)
            Else
                Gl.glUniform1i(decalC_use_shadow, 0)
            End If

            Gl.glUniform1i(decalC_depthMap, 0)
            Gl.glUniform1i(decalC_shadowMap, 1)
            Gl.glUniform1i(decalC_gNormalMap, 2)
            Gl.glUniform1i(decalC_fog, 3)
            Gl.glUniform1i(decalC_cube, 4)
            Gl.glUniform1i(decalC_brdf, 5)

            Gl.glUniform1i(decalC_colorMap, 6)
            Gl.glUniform1i(decalC_normalMap, 7)
            Gl.glUniform1i(decalC_GMM, 8)

            Gl.glUniform3f(decalC_camLocation, eyeX, eyeY, eyeZ)
            Gl.glUniform3fv(decalC_lightPosition, 3, l_array)
            Gl.glUniformMatrix4fv(decalC_shadowProj, 1, Gl.GL_FALSE, lightProjection)
            'set up debug values
            Dim v1, v2, v3, v4 As Single
            v1 = CSng(section_a And 1) : v2 = CSng((section_a And 2) >> 1) : v3 = CSng((section_a And 4) >> 2) : v4 = CSng((section_a And 8) >> 3)
            Gl.glUniform4f(decalC_a_group, v1, v2, v3, v4)
            v1 = CSng(section_b And 1) : v2 = CSng((section_b And 2) >> 1) : v3 = CSng((section_b And 4) >> 2) : v4 = CSng((section_b And 8) >> 3)
            Gl.glUniform4f(decalC_b_group, v1, v2, v3, v4)


            Gl.glActiveTexture(Gl.GL_TEXTURE0)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, gDepth)
            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, depthBuffer) 'shadow depth map
            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 2)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, gNormal)
            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 3)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, gFXAA) 'animated ground fog from terrain shader
            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 4)
            Gl.glBindTexture(Gl.GL_TEXTURE_CUBE_MAP, cube_texture_id)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 5)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, u_brdfLUT)


            For i = 0 To decal_matrix_list.Length - 2
                Dim j = decal_order(i)
                With decal_matrix_list(j)
                    .transform()
                    Gl.glUniformMatrix4fv(decalC_decal_matrix, 1, Gl.GL_FALSE, .display_matrix)
                    Gl.glUniform2f(decalC_UVwrap, .u_wrap, .v_wrap)
                    Gl.glUniform1f(decalC_uv_rotate, .uv_rot)
                    Gl.glUniform1f(decalC_alpha, .alpha)
                    Gl.glUniform1f(decalC_level, .level)
                    Gl.glActiveTexture(Gl.GL_TEXTURE0 + 6)
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, decal_matrix_list(j).texture_id)
                    Gl.glActiveTexture(Gl.GL_TEXTURE0 + 7)
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, decal_matrix_list(j).normal_id)
                    Gl.glActiveTexture(Gl.GL_TEXTURE0 + 8)
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, decal_matrix_list(j).gmm_id)

                    Gl.glCallList(decal_draw_box)
                End With
            Next
            Gl.glUseProgram(0)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '8
            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 7)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '7
            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 6)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '6
            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 6)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '5
            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 4)
            Gl.glBindTexture(Gl.GL_TEXTURE_CUBE_MAP, 0) '4
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '4
            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 3)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '3
            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 2)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '2
            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '1
            Gl.glActiveTexture(Gl.GL_TEXTURE0)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '0
            Gl.glDisable(Gl.GL_BLEND)
            Gl.glDepthMask(Gl.GL_TRUE)
            Gl.glEnable(Gl.GL_DEPTH_TEST)

        End If

    End Sub
    '###########################################################################################################################################
    Public Sub draw_scene()
        Application.DoEvents()
        If gl_stop Then Return
        view_status_string = ""
        'End If
        If gBufferFBO = 0 Then
            G_Buffer.init()
        End If
        If m_shadows.Checked And Not frmComponents.Visible Then
            render_depth_to_depth_texture(0)
        End If
        If is_camo_active() Then
            m_edit_camo.Visible = True
        Else
            m_edit_camo.Visible = False
            frmEditCamo.Visible = False
        End If

        If Not (Wgl.wglMakeCurrent(pb1_hDC, pb1_hRC)) Then
            MessageBox.Show("Unable to make rendering context current")
            End
        End If
        'gl_busy = False
        'Return
        Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, gBufferFBO)
        'Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, 0)
        G_Buffer.attachColorTexture()
        For jj = 1 To object_count
            If _group(jj).alphaTest = 0 Then
                _group(jj).alphaRef = 0
            End If
        Next
        Dim color_top() As Byte = {20, 20, 20}
        Dim color_bottom() As Byte = {60, 60, 60}
        Dim position() As Single = {10, 10.0F, 10, 1.0F}

        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)

        Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE)
        Dim er = Gl.glGetError
        Gl.glDepthFunc(Gl.GL_LEQUAL)
        Gl.glFrontFace(Gl.GL_CW)

        Gl.glPolygonOffset(1.0, 1.0)
        Gl.glLineWidth(1)
        Gl.glPointSize(2.0)
        Gl.glClearColor(0.0F, 0.0F, 0.0F, 1.0F)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)

        Gl.glDisable(Gl.GL_BLEND)

        Gl.glEnable(Gl.GL_LIGHTING)
        Gl.glEnable(Gl.GL_CULL_FACE)

        Gl.glEnable(Gl.GL_SMOOTH)
        Gl.glEnable(Gl.GL_NORMALIZE)

        Dim h, w As Integer
        G_Buffer.getsize(w, h)
        ResizeGL(w, h)
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

        Dim l_color() = {0.3!, 0.3!, 0.3!}

        If MODEL_LOADED Then
            If m_show_fbx.Checked Then
                view_status_string = ": FBX View "
            Else
                view_status_string = ": Model View "
            End If
            If m_show_bsp2.Checked Then
                view_status_string = ": BSP2 View "

            End If
        Else
            view_status_string = ": Nothing Loaded "
        End If
        If wire_cb.Checked Then
            view_status_string += ": Solid : "
        Else
            view_status_string += ": Facets : "
        End If
        ViewPerspective(w, h)
        'adjust light2
        position2(0) = -10.0 : position2(1) = 7.5 : position2(2) = -2.4
        set_eyes()
        Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, position0)
        Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_POSITION, position1)
        Gl.glLightfv(Gl.GL_LIGHT2, Gl.GL_POSITION, position2)

        '=============================================================
        If Not grid_cb.Checked Then
            draw_environment()
            draw_decals()
        Else
            Gl.glFrontFace(Gl.GL_CW)
        End If
        '=============================================================
        Gl.glEnable(Gl.GL_LIGHTING)

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
        'cube test
        If m_show_environment.Checked Then
            Gl.glEnable(Gl.GL_LIGHTING)
            Gl.glColor3f(0.75, 0.75, 0.75)

            Gl.glPushMatrix()
            Gl.glUseProgram(shader_list.cube_shader)
            Gl.glScalef(45.0, 45.0, 45.0)
            Gl.glEnable(Gl.GL_TEXTURE_CUBE_MAP)
            Gl.glBindTexture(Gl.GL_TEXTURE_CUBE_MAP, cube_texture_id)
            Gl.glCallList(cube_draw_id)
            Gl.glBindTexture(Gl.GL_TEXTURE_CUBE_MAP, 0)
            Gl.glDisable(Gl.GL_TEXTURE_CUBE_MAP)

            Gl.glUseProgram(0)
            Gl.glPopMatrix()

        End If
        Gl.glColor3f(1.0, 1.0, 1.0)

        '-----------------------------------------------------------------------------
        Gl.glEnable(Gl.GL_DEPTH_TEST)
        Gl.glColor3fv(l_color)
        'Draw Imported FBX if it exists?
        If FBX_LOADED And m_show_fbx.Checked And Not m_show_bsp2.Checked Then
            Gl.glEnable(Gl.GL_TEXTURE_2D)
            If Not wire_cb.Checked Then
                Gl.glEnable(Gl.GL_POLYGON_OFFSET_FILL)
            End If
            If m_load_textures.Checked Then
                Gl.glUseProgram(shader_list.fbx_shader)
                Gl.glUniform1i(fbx_colorMap, 0)
                Gl.glUniform1i(fbx_normalMap, 1)
                Gl.glUniform1i(fbx_specularMap, 2)
                Gl.glUniform1f(fbx_specular, S_level) ' convert to 0.0 to 1.0
                Gl.glUniform1f(fbx_ambient, A_level)
                Gl.glUniform1f(fbx_level, T_level)
                For jj = 1 To fbxgrp.Length - 1
                    Gl.glUniform1i(fbx_texture_count, fbxgrp(jj).texture_count)
                    Gl.glUniform1i(fbx_is_GAmap, fbxgrp(jj).is_GAmap)
                    Gl.glUniform1i(fbx_alphatest, fbxgrp(jj).alphaTest)
                    If m_load_textures.Checked Then
                        Gl.glColor3f(0.5, 0.5, 0.5)
                        Gl.glActiveTexture(Gl.GL_TEXTURE0)
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, fbxgrp(jj).color_Id)
                        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, fbxgrp(jj).normal_Id)
                        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 2)
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, fbxgrp(jj).specular_id)
                    End If
                    Gl.glPushMatrix()
                    Gl.glMultMatrixd(fbxgrp(jj).matrix)
                    Gl.glCallList(fbxgrp(jj).call_list)
                    Gl.glPopMatrix()
                Next
            Else
                For jj = 1 To fbxgrp.Length - 1
                    Gl.glPushMatrix()
                    Gl.glMultMatrixd(fbxgrp(jj).matrix)
                    Gl.glCallList(fbxgrp(jj).call_list)
                    Gl.glPopMatrix()
                Next

            End If
            Gl.glUseProgram(0)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            If Not wire_cb.Checked Then
                Gl.glDisable(Gl.GL_TEXTURE_2D)
                Gl.glDisable(Gl.GL_POLYGON_OFFSET_FILL)
                Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE)
                Gl.glColor3f(0.1, 0.1, 0.1)
                For jj = 1 To fbxgrp.Length - 1
                    Gl.glPushMatrix()
                    Gl.glMultMatrixd(fbxgrp(jj).matrix)
                    Gl.glCallList(fbxgrp(jj).call_list)
                    Gl.glPopMatrix()
                Next
            End If
        End If
        'Dont draw textures?
        If MODEL_LOADED And Not m_load_textures.Checked And Not m_show_fbx.Checked And Not m_show_bsp2.Checked Then
            view_status_string += "Light Only : "
            If Not wire_cb.Checked Then
                Gl.glEnable(Gl.GL_POLYGON_OFFSET_FILL)
            End If
            For jj = 1 To object_count
                If _group(jj).is_carraige Then
                    Gl.glFrontFace(Gl.GL_CW)
                Else
                    Gl.glFrontFace(Gl.GL_CCW)
                End If
                If _group(jj).doubleSided Then
                    Gl.glDisable(Gl.GL_CULL_FACE)
                    Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
                Else
                    Gl.glEnable(Gl.GL_CULL_FACE)
                    Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_FILL)
                End If
                If _object(jj).visible Then
                    Gl.glCallList(_object(jj).main_display_list)
                End If
            Next

            If Not wire_cb.Checked Then
                Gl.glDisable(Gl.GL_POLYGON_OFFSET_FILL)
                Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE)
                Gl.glColor3f(0.1, 0.1, 0.1)
                For jj = 1 To object_count
                    If _group(jj).is_carraige Then
                        Gl.glFrontFace(Gl.GL_CW)
                    Else
                        Gl.glFrontFace(Gl.GL_CCW)
                    End If
                    If _object(jj).visible Then
                        Gl.glCallList(_object(jj).main_display_list)
                    End If
                Next

            End If
        End If
        'draw BSP2?
        If MODEL_LOADED And m_show_bsp2.Checked Then
            Gl.glEnable(Gl.GL_LIGHTING)
            Gl.glEnable(Gl.GL_POLYGON_OFFSET_FILL)
            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
            For jj = 1 To object_count
                If _object(jj).visible Then
                    Gl.glCallList(_group(jj).bsp2_id)
                End If
            Next
            Gl.glDisable(Gl.GL_POLYGON_OFFSET_FILL)
            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE)
            Gl.glColor3f(0.1, 0.1, 0.1)
            For jj = 1 To object_count
                If _object(jj).visible Then
                    Gl.glCallList(_group(jj).bsp2_id)
                End If
            Next
        End If
        'draw BSP2_Tree?
        If MODEL_LOADED And Not m_show_fbx.Checked And m_show_bsp2_tree.Checked Then
            view_status_string += ": BSP2 Tree Visiable : "
            For jj = 1 To object_count
                If _object(jj).visible Then
                    Gl.glCallList(_group(jj).bsp2_tree_id)
                End If
            Next
        End If
        Dim id As Integer = SELECTED_CAMO_BUTTON
        'Draw fully rendered?
        If MODEL_LOADED And m_load_textures.Checked And Not m_show_fbx.Checked And Not m_show_bsp2.Checked And Not m_simple_lighting.Checked Then
            view_status_string += "Textured : "
            Gl.glUseProgram(shader_list.tank_shader)
            Gl.glUniform1i(tank_colorMap, 0)
            Gl.glUniform1i(tank_normalMap, 1)
            Gl.glUniform1i(tank_GMM, 2)
            Gl.glUniform1i(tank_AO, 3)
            Gl.glUniform1i(tank_detailMap, 4)
            Gl.glUniform1i(tank_camo, 5)
            Gl.glUniform1i(tank_cubeMap, 6)
            Gl.glUniform1i(tank_LUT, 7)
            Gl.glUniform1i(tank_shadowMap, 8)

            Gl.glUniformMatrix4fv(tank_lightMatrix, 1, 0, lightProjection)

            Gl.glUniform3f(tank_Camera, eyeX, eyeY, eyeZ)
            Gl.glUniform1f(tank_specular, S_level) ' convert to 0.0 to 1.0
            Gl.glUniform1f(tank_ambient, A_level)
            Gl.glUniform1f(tank_total, T_level)
            If m_shadows.Checked Then
                Gl.glUniform1i(tank_use_shadow, 1)
            Else
                Gl.glUniform1i(tank_use_shadow, 0)
            End If
            'set shader debug mask values
            Dim v1, v2, v3, v4 As Single
            v1 = CSng(section_a And 1) : v2 = CSng((section_a And 2) >> 1) : v3 = CSng((section_a And 4) >> 2) : v4 = CSng((section_a And 8) >> 3)
            Gl.glUniform4f(tank_a_group, v1, v2, v3, v4)
            v1 = CSng(section_b And 1) : v2 = CSng((section_b And 2) >> 1) : v3 = CSng((section_b And 4) >> 2) : v4 = CSng((section_b And 8) >> 3)
            Gl.glUniform4f(tank_b_group, v1, v2, v3, v4)

            If Not wire_cb.Checked Then
                Gl.glEnable(Gl.GL_POLYGON_OFFSET_FILL)
            End If

            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 8)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, depthBuffer)

            For jj = 1 To object_count - track_info.segment_count
                If _group(jj).is_carraige Then
                    Gl.glFrontFace(Gl.GL_CW)
                Else
                    Gl.glFrontFace(Gl.GL_CCW)
                End If
                If _group(jj).doubleSided Or Not _group(jj).metal_textured Then
                    'Gl.glCullFace(Gl.GL_NONE)
                    Gl.glDisable(Gl.GL_CULL_FACE)
                    Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
                Else
                    'Gl.glCullFace(Gl.GL_BACK)
                    Gl.glEnable(Gl.GL_CULL_FACE)
                    Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_FILL)
                End If
                Gl.glUniform1i(tank_is_GAmap, _object(jj).ANM)
                Gl.glUniform1i(tank_alphaRef, _group(jj).alphaRef)
                Gl.glUniform2f(tank_detailTiling, _group(jj).detail_tile.x, _group(jj).detail_tile.y)
                Gl.glUniform1f(tank_detailPower, _group(jj).detail_power)
                Gl.glUniform4f(tank_tile_vec4, _object(jj).camo_tiling.x, _object(jj).camo_tiling.y, _object(jj).camo_tiling.z, _object(jj).camo_tiling.w)
                Gl.glUniform1i(tank_use_camo, _object(jj).use_camo)
                Gl.glUniform1i(tank_exclude_camo, _object(jj).exclude_camo)
                Gl.glUniform1i(tank_use_CM, GLOBAL_exclusionMask)
                Gl.glUniform4f(tank_armorcolor, ARMORCOLOR.x, ARMORCOLOR.y, ARMORCOLOR.z, ARMORCOLOR.w)
                If _object(jj).use_camo > 0 Then

                    Gl.glUniform4f(tank_c0, c0(id).x, c0(id).y, c0(id).z, c0(id).w)
                    Gl.glUniform4f(tank_c1, c1(id).x, c1(id).y, c1(id).z, c1(id).w)
                    Gl.glUniform4f(tank_c2, c2(id).x, c2(id).y, c2(id).z, c2(id).w)
                    Gl.glUniform4f(tank_c3, c3(id).x, c3(id).y, c3(id).z, c3(id).w)
                    Gl.glUniform4f(tank_camo_tiling, bb_tank_tiling(id).x, bb_tank_tiling(id).y, bb_tank_tiling(id).z, bb_tank_tiling(id).w)
                End If


                If _object(jj).visible Then
                    Gl.glActiveTexture(Gl.GL_TEXTURE0)
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).color_Id)
                    Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).normal_Id)
                    Gl.glActiveTexture(Gl.GL_TEXTURE0 + 2)
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).metalGMM_Id)
                    Gl.glActiveTexture(Gl.GL_TEXTURE0 + 3)
                    If GLOBAL_exclusionMask = 1 And Not HD_TANK Then
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, exclusionMask_id)
                    Else
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).ao_id)
                    End If
                    Gl.glActiveTexture(Gl.GL_TEXTURE0 + 4)
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).detail_Id)
                    Gl.glActiveTexture(Gl.GL_TEXTURE0 + 5)
                    If _object(jj).use_camo > 0 Then
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, bb_texture_ids(id))
                    End If
                    Gl.glActiveTexture(Gl.GL_TEXTURE0 + 6)
                    Gl.glBindTexture(Gl.GL_TEXTURE_CUBE_MAP, cube_texture_id)

                    Gl.glActiveTexture(Gl.GL_TEXTURE0 + 7)
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, u_brdfLUT)

                    'Gl.glPushMatrix()
                    'Gl.glMultMatrixd(_object(jj).matrix)
                    Gl.glCallList(_object(jj).main_display_list)
                    'Gl.glPopMatrix()
                End If
            Next
            Gl.glUseProgram(0)

            'clear texture bindings

            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 8)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '8
            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 7)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '7
            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 6)
            Gl.glBindTexture(Gl.GL_TEXTURE_CUBE_MAP, 0) '6
            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 5)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '5
            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 4)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '4
            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 3)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '3
            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 2)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '2
            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '1
            Gl.glActiveTexture(Gl.GL_TEXTURE0)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '0
            If Not wire_cb.Checked Then
                Gl.glDisable(Gl.GL_TEXTURE_2D)
                Gl.glDisable(Gl.GL_LIGHTING)
                Gl.glDisable(Gl.GL_POLYGON_OFFSET_FILL)
                Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE)
                Gl.glColor3f(0.0, 0.0, 0.0)
                For jj = 1 To object_count
                    If _group(jj).is_carraige Then
                        Gl.glFrontFace(Gl.GL_CW)
                    Else
                        Gl.glFrontFace(Gl.GL_CCW)
                    End If
                    If _object(jj).visible Then
                        Gl.glCallList(_object(jj).main_display_list)
                    End If
                Next
            End If
        End If
        Gl.glEnable(Gl.GL_CULL_FACE)
        'simple lighting
        If MODEL_LOADED And m_load_textures.Checked And Not m_show_fbx.Checked And Not m_show_bsp2.Checked And m_simple_lighting.Checked Then
            view_status_string += "Simple Lighting : "
            Gl.glEnable(Gl.GL_TEXTURE_2D)
            Gl.glEnable(Gl.GL_LIGHTING)

            'set lighting
            Dim mcolor(4) As Single
            Dim specReflection(4) As Single
            Dim diffuseLight(4) As Single
            mcolor(0) = A_level : mcolor(1) = A_level : mcolor(2) = A_level : mcolor(3) = 1.0
            specReflection(0) = S_level : specReflection(1) = S_level : specReflection(2) = S_level : specReflection(3) = 1.0
            diffuseLight(0) = T_level : diffuseLight(1) = T_level : diffuseLight(2) = T_level : diffuseLight(3) = 1.0
            Gl.glColor3f(A_level, A_level, A_level)
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT, mcolor)
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, specReflection)
            'Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, diffuseLight)
            'Gl.glColorMaterial(Gl.GL_FRONT, Gl.GL_SPECULAR Or Gl.GL_AMBIENT_AND_DIFFUSE)

            Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_FILL)

            Gl.glMateriali(Gl.GL_FRONT, Gl.GL_SHININESS, CInt(S_level * 128))

            For jj = 1 To object_count - track_info.segment_count
                If _group(jj).is_carraige Then
                    Gl.glFrontFace(Gl.GL_CW)
                Else
                    Gl.glFrontFace(Gl.GL_CCW)
                End If
                If Not wire_cb.Checked Then
                    Gl.glEnable(Gl.GL_POLYGON_OFFSET_FILL)
                End If

                If _object(jj).visible Then
                    Gl.glActiveTexture(Gl.GL_TEXTURE0)
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).color_Id)
                    Gl.glCallList(_object(jj).main_display_list)

                End If
            Next
            If Not wire_cb.Checked Then
                Gl.glDisable(Gl.GL_TEXTURE_2D)
                Gl.glDisable(Gl.GL_LIGHTING)
                Gl.glDisable(Gl.GL_POLYGON_OFFSET_FILL)
                Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE)
                Gl.glColor3f(0.0, 0.0, 0.0)
                For jj = 1 To object_count
                    If _object(jj).visible Then
                        Gl.glCallList(_object(jj).main_display_list)
                    End If
                Next
            End If

        End If

        'Draw Surface Normals?
        If MODEL_LOADED And normal_shader_mode > 0 Then
            Gl.glUseProgram(shader_list.normal_shader)
            Gl.glUniform1i(normal_shader_mode_id, normal_shader_mode)
            If MODEL_LOADED Then
                If normal_shader_mode = 1 Then
                    view_status_string += " Normal View by Face : "
                End If
                If normal_shader_mode = 2 Then
                    view_status_string += " Normal View by Vertex : "
                End If
                If FBX_LOADED And m_show_fbx.Checked Then ' FBX if loaded
                    view_status_string += " Textured : "
                    For jj = 1 To fbxgrp.Length - 1
                        Gl.glPushMatrix()
                        Gl.glMultMatrixd(fbxgrp(jj).matrix)
                        Gl.glCallList(fbxgrp(jj).call_list)
                        Gl.glPopMatrix()
                    Next
                Else
                    For jj = 1 To object_count
                        If _object(jj).visible Then
                            If m_show_bsp2.Checked Then
                                Gl.glCallList(_group(jj).bsp2_id) 'BSP2
                            Else
                                Gl.glCallList(_object(jj).main_display_list) 'Model
                            End If
                        End If
                    Next
                End If
            End If
            Gl.glUseProgram(0)


        End If
        '==========================================
        'Gl.glDisable(Gl.GL_CULL_FACE)
        Gl.glDisable(Gl.GL_LIGHTING)
        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        If MODEL_LOADED And frmTextureViewer.Visible And (frmTextureViewer.m_show_uvs.Checked Or frmTextureViewer.m_uvs_only.Checked) Then
            Gl.glFrontFace(Gl.GL_CW)
            Gl.glEnable(Gl.GL_BLEND)
            Gl.glColor4f(0.8, 0.4, 0.0, 0.8)
            If _group(current_part).is_carraige Then
                Gl.glFrontFace(Gl.GL_CW)
            Else
                Gl.glFrontFace(Gl.GL_CCW)
            End If

            Gl.glBegin(Gl.GL_TRIANGLES)
            Dim v1 = _object(current_part).tris(current_vertex).v1
            Dim v2 = _object(current_part).tris(current_vertex).v2
            Dim v3 = _object(current_part).tris(current_vertex).v3
            Gl.glVertex3f(v1.x, v1.y, v1.z)
            Gl.glVertex3f(v2.x, v2.y, v2.z)
            Gl.glVertex3f(v3.x, v3.y, v3.z)
            Gl.glEnd()
        End If
        Gl.glDisable(Gl.GL_BLEND)
        Gl.glColor3f(0.3, 0.3, 0.3)
        If grid_cb.Checked Then
            Gl.glCallList(grid)
        End If

        'track_test()
        '=================================================================================
        If m_decal.Checked And Not hide_BB_cb.Checked Then
            If current_decal > -1 Then
                Gl.glLineWidth(2.0)
                Gl.glEnable(Gl.GL_LIGHTING)
                Gl.glDisable(Gl.GL_CULL_FACE)
                Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE)
                'Gl.glDisable(Gl.GL_DEPTH_TEST)
                For j = 0 To decal_matrix_list.Length - 2
                    If j = current_decal Then
                        Gl.glColor3f(1.0, 0.0, 0.0)
                    Else
                        Gl.glColor3f(1.0, 1.0, 1.0)
                    End If
                    Gl.glPushMatrix()
                    decal_matrix_list(j).transform()
                    Gl.glMultMatrixf(decal_matrix_list(j).display_matrix)
                    Gl.glCallList(decal_draw_box)
                    Gl.glPopMatrix()
                Next
                Gl.glLineWidth(1.0)

            End If
        End If
        '=================================================================================


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

        Gl.glPopMatrix()
        If MODEL_LOADED Then
            draw_triangle_mouse_texture_window()
        Else
            found_triangle_tv = 0
        End If
        Gl.glFrontFace(Gl.GL_CCW)
        Gl.glEnable(Gl.GL_CULL_FACE)
        Gl.glDisable(Gl.GL_DEPTH_TEST)
        Gl.glDisable(Gl.GL_LIGHTING)
        Gl.glPolygonMode(Gl.GL_BACK, Gl.GL_FILL)
        '===========================================================
        Dim P As New Point(0, 0)
        Gl.glDisable(Gl.GL_CULL_FACE)
        '===========================================================
        '######################################################################### ORTHO MODE
        '######################################################################### ORTHO MODE
        '######################################################################### ORTHO MODE
        ViewOrtho()
        'GoTo fuckit

        'pass one FXAA
        'Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, 0)
        G_Buffer.attachFXAAtexture()
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)

        Gl.glUseProgram(shader_list.FXAA_shader)
        Gl.glUniform1i(FXAA_color, 0)
        Gl.glUniform2f(FXAA_screenSize, CSng(w), CSng(h))



        Gl.glEnable(Gl.GL_TEXTURE_2D)
        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, gColor)
        Gl.glColor3f(1.0, 1.0, 1.0)

        draw_main_rec(P, w, h)

        '===========================================================


        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Gl.glUseProgram(0)
        '===========================================================
        '===========================================================
        '===========================================================
        'final render
        '===========================================================
        '===========================================================
        '===========================================================
fuckit:
        Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, 0)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)



        'GoTo over
        Gl.glEnable(Gl.GL_TEXTURE_2D)
        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, gFXAA)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, gColor)
        Gl.glColor3f(1.0, 1.0, 1.0)
        draw_main_rec(P, w, h)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Gl.glUseProgram(0)
        'menu
        'draw_menu()

        '######################################################################
        'draw bottom hightlighted area
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
        '######################################################################
        If m_shadow_preview.Checked Then
            show_depth_texture()
        End If
        '######################################################################
        'handle upton if needed
        If m_decal.Checked Then
            upton.draw_upton()
        End If
        '######################################################################
        Dim fps As Integer = 1.0 / (screen_totaled_draw_time * 0.001)
        Dim str = " FPS: ( " + fps.ToString + " )"
        'swat.Stop()
        glutPrint(10, 8 - pb1.Height, str.ToString, 0.0, 1.0, 0.0, 1.0) ' fps string
        glutPrint(10, -20, view_status_string, 0.0, 1.0, 0.0, 1.0) ' view status

        Gl.glDisable(Gl.GL_BLEND)
        Gl.glDisable(Gl.GL_DEPTH_TEST)

        If show_textures_cb.Checked Then
            draw_texture_screen()
            If TANK_TEXTURES_VISIBLE Then
                For i = 0 To texture_buttons.Length - 2
                    texture_buttons(i).draw()
                Next
            End If
        End If

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
        '====================================
        Gdi.SwapBuffers(pb1_hDC)
        '====================================
        'has to be AFTER the buffer swap
        Dim et = pick_timer.ElapsedMilliseconds
        If et > 100 Then 'only do picking so often.. NOT every frame.. its to expensive in render time!
            pick_timer.Restart()
            If frmTextureViewer.Visible Then
                For i = 0 To texture_buttons.Length - 2
                    If current_part = texture_buttons(i).part_ID Then
                        If texture_buttons(i).selected Then
                            frmTextureViewer.draw()
                            If Not (Wgl.wglMakeCurrent(pb1_hDC, pb1_hRC)) Then
                                MessageBox.Show("Unable to make rendering context current")
                                End
                            End If
                            Exit For
                        End If
                    End If
                Next
                If pb2_has_focus Then
                    frmTextureViewer.draw()
                    If Not (Wgl.wglMakeCurrent(pb1_hDC, pb1_hRC)) Then
                        MessageBox.Show("Unable to make rendering context current")
                        End
                    End If
                    ViewOrtho()
                End If
            End If
            '==============================================================================
            If Not STOP_BUTTON_SCAN Then
                Gl.glFrontFace(Gl.GL_CW)
                If season_Buttons_VISIBLE Then
                    draw_season_pick_buttons()
                    mouse_pick_season_button(m_mouse.x, m_mouse.y)
                    'Gdi.SwapBuffers(pb1_hDC)
                End If
                If CAMO_BUTTONS_VISIBLE Then
                    draw_pick_camo_buttons()
                    mouse_pick_camo_button(m_mouse.x, m_mouse.y)
                End If
                If TANKPARTS_VISIBLE Then
                    draw_tankpart_pick()
                    mouse_pick_tankparts(m_mouse.x, m_mouse.y)
                End If
                If TANK_TEXTURES_VISIBLE Then
                    draw_textures_pick()
                    mouse_pick_textures(m_mouse.x, m_mouse.y)
                End If
            End If
            If TANK_TEXTURES_VISIBLE And frmTextureViewer.Visible Then
                draw_tank_pick()
                mouse_pick_tank_vertex(m_mouse.x, m_mouse.y)
                'Gdi.SwapBuffers(pb1_hDC)
            End If
            '====================================
            If m_decal.Checked Then
                upton.pick_upton()
            End If
            '====================================
            'this put the view in perspective!
            If mouse_pick_cb.Checked Then
                mouse_pick_decal()
            End If
            '====================================
        End If
        Gl.glFlush()
        er = Gl.glGetError
        OLD_WINDOW_HEIGHT = pb1.Height
    End Sub
    Private Sub draw_main_rec(ByVal p As Point, ByVal w As Integer, ByVal h As Integer)
        Gl.glBegin(Gl.GL_QUADS)
        'G_Buffer.getsize(w, h)
        '  CW...
        '  1 ------ 2
        '  |        |
        '  |        |
        '  4 ------ 3
        '
        Gl.glTexCoord2f(0.0!, 1.0!)
        Gl.glVertex2f(p.X, p.Y)

        Gl.glTexCoord2f(1.0!, 1.0!)
        Gl.glVertex2f(p.X + w, p.Y)

        Gl.glTexCoord2f(1.0!, 0.0!)
        Gl.glVertex2f(p.X + w, p.Y - h)

        Gl.glTexCoord2f(0.0!, 0.0!)
        Gl.glVertex2f(p.X, p.Y - h)
        Gl.glEnd()


    End Sub
    '###########################################################################################################################################
    Public Sub draw_triangle_mouse_texture_window()
        'If Not pb2.Focused Then Return

        If found_triangle_tv > 0 Then

            Gl.glEnable(Gl.GL_DEPTH_TEST)
            Gl.glEnable(Gl.GL_BLEND)
            Gl.glFrontFace(Gl.GL_CW)
            Gl.glColor4f(1.0, 0.0, 0.0, 0.8)

            Gl.glBegin(Gl.GL_TRIANGLES)
            Dim v1 = _object(current_tank_part).tris(found_triangle_tv).v1
            Dim v2 = _object(current_tank_part).tris(found_triangle_tv).v2
            Dim v3 = _object(current_tank_part).tris(found_triangle_tv).v3
            Gl.glVertex3f(v2.x, v2.y, v2.z)
            Gl.glVertex3f(v1.x, v1.y, v1.z)
            Gl.glVertex3f(v3.x, v3.y, v3.z)
            Gl.glEnd()
            Gl.glDisable(Gl.GL_BLEND)
        End If
    End Sub
    Public Sub track_test()
        'track nurb points
        If MODEL_LOADED And TESTING Then
            If object_count > 6 Then
                Gl.glColor3f(0.9, 0.9, 0.9)
                running = 0.0
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
                Gl.glUniform1i(tank_alphaRef, _group(jj).alphaTest)
                Gl.glUniform2f(tank_detailTiling, _group(jj).detail_tile.x, _group(jj).detail_tile.y)
                Gl.glUniform1f(tank_detailPower, _group(jj).detail_power)
                Gl.glUniform4f(tank_tile_vec4, _object(jj).camo_tiling.x, _object(jj).camo_tiling.y, _object(jj).camo_tiling.z, _object(jj).camo_tiling.w)
                Gl.glUniform1i(tank_use_camo, _object(jj).use_camo)
                Gl.glUniform1i(tank_exclude_camo, 1)
                Gl.glUniform4f(tank_armorcolor, ARMORCOLOR.x, ARMORCOLOR.y, ARMORCOLOR.z, ARMORCOLOR.w)
                Gl.glUniform1i(tank_use_CM, GLOBAL_exclusionMask)

                Gl.glActiveTexture(Gl.GL_TEXTURE0)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).color_Id)
                Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).normal_Id)
                Gl.glActiveTexture(Gl.GL_TEXTURE0 + 2)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).metalGMM_Id)
                Gl.glActiveTexture(Gl.GL_TEXTURE0 + 3)
                If GLOBAL_exclusionMask = 1 And Not HD_TANK Then
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, exclusionMask_id)
                Else
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).ao_id)
                End If
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
                        Gl.glScalef(1.0, 1.0, 1.0)
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
                            Gl.glScalef(1.0, 1.0, 1.0)
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
    Private Sub draw_tank_pick()
        Dim w, h As Integer
        G_Buffer.getsize(w, h)
        ViewPerspective(w, h)
        set_eyes()
        Gl.glClearColor(0.0!, 0.0!, 0.0!, 0.0!)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
        Gl.glDisable(Gl.GL_BLEND)
        Gl.glEnable(Gl.GL_DEPTH_TEST)
        Gl.glDisable(Gl.GL_LIGHTING)
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
        For i = 1 To object_count
            If _object(i).visible Then 'lets not waste time drawing what we wont pick.
                Gl.glCallList(_object(i).vertex_pick_list)
            End If
        Next
    End Sub
    Public Sub mouse_pick_tank_vertex(ByVal x As Integer, ByVal y As Integer)
        'pick function
        Dim viewport(4) As Integer
        Dim pixel() As Byte = {0, 0, 0, 0}
        Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport)
        Gl.glReadPixels(x, viewport(3) - y, 1, 1, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, pixel)
        Dim part = pixel(3)
        Dim index As UInt32 = CUInt(pixel(0))
        If part > 0 Then
            current_part = part - 10
            index = pixel(0) + (pixel(1) * 256) + (pixel(2) * 65536)

            If index > 0 Then
                current_vertex = index
            Else
                current_part = 0
                current_vertex = 0
            End If

        Else
            current_part = 0
            current_vertex = 0
        End If

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

#Region "decal transform"

    Private tempX, tempZ As Single
    Private Sub move_xyz()
        If current_decal = -1 Then Return
        Dim x, z As Single
        Dim ms As Single = Sin((view_radius / 80.0!) * (PI / 2.0)) ' distance away changes speed. THIS WORKS WELL!
        Dim speed As Single = 0.2

        If upton.state = 5 Or upton.state = 7 Then
            x = (mouse.x - m_mouse.x) * ms * speed
            z = (mouse.y - m_mouse.y) * ms * speed

            g_decal_translate.x += (x * -Cos(Cam_X_angle)) + (z * -Sin(Cam_X_angle))

            g_decal_translate.z += (z * -Cos(Cam_X_angle)) + (x * Sin(Cam_X_angle))

        End If

        If upton.state = 6 Then
            g_decal_translate.y += -(mouse.y - m_mouse.y) * ms * speed
        End If
        decal_matrix_list(current_decal).set_translate_matrix(0, g_decal_translate)
        mouse.x = m_mouse.x
        mouse.y = m_mouse.y
        If track_decal_cb.Checked Then
            look_point_x = decal_matrix_list(current_decal).translate.x
            look_point_y = decal_matrix_list(current_decal).translate.y
            look_point_z = decal_matrix_list(current_decal).translate.z
        End If
    End Sub
    Private Sub rotate_decal_xy()
        If current_decal = -1 Then Return
        Dim x, z As Single
        If upton.state = 8 Then
            x = -(mouse.y - m_mouse.y) * 0.01
            g_decal_rotate.y += x
            decal_matrix_list(current_decal).set_y_rotation_matrix(x)
        End If
        If upton.state = 9 Then
            z = -(mouse.y - m_mouse.y) * 0.01
            g_decal_rotate.x += z
            decal_matrix_list(current_decal).set_x_rotation_matrix(z)
        End If
        If upton.state = 10 Then
            z = -(mouse.y - m_mouse.y) * 0.01
            g_decal_rotate.z += z
            decal_matrix_list(current_decal).set_z_rotation_matrix(z)
        End If
        'Debug.WriteLine("x " + x.ToString("0.0000") + " :z " + z.ToString("0.00000"))
        mouse.x = m_mouse.x
        mouse.y = m_mouse.y
    End Sub
    Private Sub scale_decal_xyz()
        If current_decal = -1 Then Return
        Dim v As New vect3
        Dim z As Single
        Dim ms As Double = view_radius / 80 ' distance away changes speed. THIS WORKS WELL!
        Dim speed As Single = 0.25

        z = -(mouse.y - m_mouse.y) * ms * speed
        If upton.state = 1 Then
            g_decal_scale.x += z
            If g_decal_scale.x < 0.1 Then g_decal_scale.x = 0.1
            decal_matrix_list(current_decal).set_scale_matrix(g_decal_scale)
        End If
        If upton.state = 2 Then
            g_decal_scale.z += z
            If g_decal_scale.z < 0.1 Then g_decal_scale.z = 0.1
            decal_matrix_list(current_decal).set_scale_matrix(g_decal_scale)
        End If
        If upton.state = 3 Then
            g_decal_scale.y += z
            If g_decal_scale.y < 0.1 Then g_decal_scale.y = 0.1
            decal_matrix_list(current_decal).set_scale_matrix(g_decal_scale)

        End If
        If upton.state = 4 Then
            g_decal_scale.x += z
            g_decal_scale.y += z
            g_decal_scale.z += z
            If g_decal_scale.x < 0.1 Then g_decal_scale.x = 0.1
            If g_decal_scale.y < 0.1 Then g_decal_scale.y = 0.1
            If g_decal_scale.z < 0.1 Then g_decal_scale.z = 0.1
            decal_matrix_list(current_decal).set_scale_matrix(g_decal_scale)
        End If

        mouse.x = m_mouse.x
        mouse.y = m_mouse.y

    End Sub

#End Region
#Region "PB1 Mouse"

    Private Sub pb1_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles pb1.MouseDoubleClick

    End Sub

    Private Sub pb1_MouseDown(sender As Object, e As MouseEventArgs) Handles pb1.MouseDown
        'If M_SELECT_COLOR > 0 Then
        '    For i = 0 To button_list.Length - 2
        '        If M_SELECT_COLOR = button_list(i).color Then
        '            CallByName(Menu_Subs, button_list(i).function_, Microsoft.VisualBasic.CallType.Method)
        '        End If
        '    Next
        'End If
        mouse.x = e.X
        mouse.y = e.Y
        If mouse_pick_cb.Checked Then
            If picked_decal > -1 Then
                current_decal = picked_decal
                picked_decal = -1
                decal_matrix_list(current_decal).get_decals_transform_info()
                mouse_pick_cb.Checked = False
                update_decal_order()
                Dim tc As Integer = 0
                For k = 0 To decal_order.Length - 1
                    If decal_order(k) = current_decal Then
                        tc = k
                    End If
                Next
                d_current_line = tc

                Dim sp = d_list_tb.GetFirstCharIndexFromLine(tc) ' get prev line
                d_list_tb.SelectionStart = sp
                d_list_tb.Select(d_list_tb.GetFirstCharIndexOfCurrentLine(), _
                                 d_list_tb.Lines(tc).Length) ' select prev line

            End If
        End If
        If e.Button = Forms.MouseButtons.Right Then
            move_cam_z = True
        End If
        If e.Button = Forms.MouseButtons.Middle Then
            move_mod = True
            M_DOWN = True
        End If
        If e.Button = Forms.MouseButtons.Left Then

            CAMO_BUTTON_DOWN = True
            M_DOWN = True
        End If
    End Sub

    Private Sub pb1_MouseEnter(sender As Object, e As EventArgs) Handles pb1.MouseEnter
        pb1.Focus()
    End Sub
    Private Sub pb1_MouseMove(sender As Object, e As MouseEventArgs) Handles pb1.MouseMove
        m_mouse.x = e.X
        m_mouse.y = e.Y
        If M_DOWN And upton.state > 0 And upton.state < 5 Then
            scale_decal_xyz()
            Return
        End If
        If M_DOWN And upton.state > 4 And upton.state < 8 Then
            move_xyz()
            Return
        End If
        If M_DOWN And upton.state > 7 And upton.state < 11 Then
            rotate_decal_xy()
            Return
        End If
        If upton.state = 102 And M_DOWN Then
            Dim delta As New Point
            delta.X = mouse.x - m_mouse.x
            delta.Y = mouse.y - m_mouse.y
            upton.position.X -= delta.X
            upton.position.Y += delta.Y
            mouse.x = m_mouse.x
            mouse.y = m_mouse.y
            Return
        End If

        If BUTTON_ID > 0 Then
            Return
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
                    If Cam_Y_angle < -PI / 2.0 Then Cam_Y_angle = -PI / 2.0 + 0.001
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
            'Debug.WriteLine(Cam_X_angle.ToString("0.000") + " " + Cam_Y_angle.ToString("0.000"))
            Return
        End If
        If move_cam_z Then
            If e.Y > (mouse.y + dead) Then
                If e.Y - mouse.y > 100 Then t = (10)
            Else : t = CSng(Sin((e.Y - mouse.y) / 100)) * 12
                view_radius += (t * (view_radius * 0.2))    ' zoom is factored in to Cam radius
                If view_radius < -80.0 Then
                    view_radius = -80.0
                End If
                mouse.y = e.Y
            End If
            If e.Y < (mouse.y - dead) Then
                If mouse.y - e.Y > 100 Then t = (10)
            Else : t = CSng(Sin((mouse.y - e.Y) / 100)) * 12
                view_radius -= (t * (view_radius * 0.2))    ' zoom is factored in to Cam radius
                If view_radius > -0.01 Then view_radius = -0.01
                mouse.y = e.Y
            End If
            If view_radius > -0.1 Then view_radius = -0.1
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
        move_mod = False
    End Sub

    Private Sub pb1_MouseWheel(sender As Object, e As MouseEventArgs) Handles pb1.MouseWheel
        If frmTextureViewer.Visible Then
            mouse_delta = New Point(0, 0)
            mouse_pos = New Point(pb2.Width / 2, pb2.Height / 2)
            If e.Delta > 0 Then
                img_scale_up()
            Else
                img_scale_down()
            End If
        End If

    End Sub




    Private Sub pb1_Paint(sender As Object, e As PaintEventArgs) Handles pb1.Paint
        If w_changing Then draw_scene()
    End Sub
#End Region

#Region "PB2 Mouse"

    Private Sub pb2_MouseDown(sender As Object, e As MouseEventArgs) Handles pb2.MouseDown
        mouse_down = True
        mouse_delta = e.Location

    End Sub

    Private Sub pb2_MouseEnter(sender As Object, e As EventArgs) Handles pb2.MouseEnter
        pb2.Focus()
        pb2_has_focus = True
    End Sub

    Private Sub pb2_MouseLeave(sender As Object, e As EventArgs) Handles pb2.MouseLeave
        pb2_has_focus = False
    End Sub

    Private Sub pb2_MouseMove(sender As Object, e As MouseEventArgs) Handles pb2.MouseMove
        mouse_find_location = e.Location
        If mouse_down Then
            Dim p As New Point
            p = e.Location - mouse_delta
            rect_location += p
            mouse_delta = e.Location
            frmTextureViewer.draw()
            Return
        End If
    End Sub

    Private Sub pb2_MouseUp(sender As Object, e As MouseEventArgs) Handles pb2.MouseUp
        mouse_down = False
    End Sub

    Private Sub pb2_MouseWheel(sender As Object, e As MouseEventArgs) Handles pb2.MouseWheel
        mouse_pos = e.Location
        mouse_delta = e.Location

        If e.Delta > 0 Then
            img_scale_up()
        Else
            img_scale_down()
        End If
    End Sub
    Public Sub img_scale_up()
        If Zoom_Factor >= 4.0 Then
            Zoom_Factor = 4.0
            Return 'to big and the t_bmp creation will hammer memory.
        End If
        Dim amt As Single = 0.125
        Zoom_Factor += amt
        Dim z = (Zoom_Factor / 1.0) * 100.0
        frmTextureViewer.zoom.Text = "Zoom:" + vbCrLf + z.ToString("000") + "%"
        Application.DoEvents()
        'this bit of math zooms the texture around the mouses center during the resize.
        'old_w and old_h is the original size of the image in width and height
        'mouse_pos is current mouse position in the window.

        Dim offset As New Point
        Dim old_size_w, old_size_h As Double
        old_size_w = (old_w * (Zoom_Factor - amt))
        old_size_h = (old_h * (Zoom_Factor - amt))

        offset = rect_location - (mouse_pos)

        rect_size.X = Zoom_Factor * old_w
        rect_size.Y = Zoom_Factor * old_h

        Dim delta_x As Double = CDbl(offset.X / old_size_w)
        Dim delta_y As Double = CDbl(offset.Y / old_size_h)

        Dim x_offset = delta_x * (rect_size.X - old_size_w)
        Dim y_offset = delta_y * (rect_size.Y - old_size_h)
        Try

            rect_location.X += CInt(x_offset)
            rect_location.Y += CInt(y_offset)

        Catch ex As Exception

        End Try
        frmTextureViewer.draw()
    End Sub
    Public Sub img_scale_down()
        If Zoom_Factor <= 0.25 Then
            Zoom_Factor = 0.25
            Return
        End If
        Dim amt As Single = 0.125
        Zoom_Factor -= amt
        Dim z = (Zoom_Factor / 1.0) * 100.0
        frmTextureViewer.zoom.Text = "Zoom:" + vbCrLf + z.ToString("000") + "%"
        Application.DoEvents()

        'this bit of math zooms the texture around the mouses center during the resize.
        'old_w and old_h is the original size of the image in width and height
        'mouse_pos is current mouse position in the window.

        Dim offset As New Point
        Dim old_size_w, old_size_h As Double

        old_size_w = (old_w * (Zoom_Factor - amt))
        old_size_h = (old_h * (Zoom_Factor - amt))

        offset = rect_location - (mouse_pos)

        rect_size.X = Zoom_Factor * old_w
        rect_size.Y = Zoom_Factor * old_h

        Dim delta_x As Double = CDbl(offset.X / (rect_size.X + (rect_size.X - old_size_w)))
        Dim delta_y As Double = CDbl(offset.Y / (rect_size.Y + (rect_size.Y - old_size_h)))

        Dim x_offset = delta_x * (rect_size.X - old_size_w)
        Dim y_offset = delta_y * (rect_size.Y - old_size_h)
        Try

            rect_location.X += -CInt(x_offset)
            rect_location.Y += -CInt(y_offset)

        Catch ex As Exception

        End Try

        frmTextureViewer.draw()
    End Sub
#End Region


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
        If stop_updating And update Then update_screen()

        Return update
    End Function
    Dim l_rot As Single
    Dim l_timer As New Stopwatch
    Public Sub update_mouse()
        Dim sun_angle As Single = 0.0
        Dim sun_radius As Single = 5.0
        'This will run for the duration that Tank Exporter is open.
        'Its in a closed loop
        screen_totaled_draw_time = 10.0
        Dim swat As New Stopwatch
        Dim x, z As Single
        Dim s As Single = 2.0
        l_timer.Restart()
        While _Started
            need_update()
            angle_offset = 0

            Application.DoEvents()
            If Not gl_busy And Not Me.WindowState = FormWindowState.Minimized Then

                If spin_light And l_timer.ElapsedMilliseconds > 32 Then
                    l_timer.Restart()
                    l_rot += 0.015
                    If l_rot > 2 * PI Then
                        l_rot -= (2 * PI)
                    End If
                    sun_angle = l_rot
                    x = Cos(l_rot) * (sun_radius * s)
                    z = Sin(l_rot) * (sun_radius * s)

                    position0(0) = x
                    position0(1) = 10.0
                    position0(2) = z

                End If


                If Not w_changing And Not stop_updating Then
                    update_screen()
                End If
                screen_draw_time = CDbl(swat.ElapsedMilliseconds)
                swat.Reset()
                swat.Start()
                If screen_avg_counter > 10 Then
                    screen_totaled_draw_time = screen_avg_draw_time / screen_avg_counter
                    screen_avg_counter = 0.0
                    screen_avg_draw_time = 0.0
                Else
                    screen_avg_counter += 1.0
                    screen_avg_draw_time += screen_draw_time
                End If
            End If

            Thread.Sleep(14)
        End While
        'Thread.CurrentThread.Abort()
    End Sub
    Private Delegate Sub update_screen_delegate()
    Private Sub update_screen()
        gl_busy = True
        Try
            If Me.InvokeRequired Then
                Me.Invoke(New update_screen_delegate(AddressOf update_screen))
            Else
                draw_scene()
            End If
        Catch ex As Exception

        End Try
        gl_busy = False
    End Sub
    Private Sub Startup_Timer_Tick(sender As Object, e As EventArgs) Handles Startup_Timer.Tick
        Startup_Timer.Enabled = False
        update_thread.IsBackground = True
        update_thread.Name = "mouse updater"
        update_thread.Priority = ThreadPriority.Lowest
        update_thread.Start()
    End Sub
#End Region

    Public Sub clean_house()
        frmModelInfo.Close() ' close so it resets on load
        frmTextureViewer.Hide() ' hide.. so we dont kill settings
        frmEditVisual.Close() ' close so it resets on load

        'reset data params
        MODEL_LOADED = False
        FBX_LOADED = False
        m_pick_camo.Enabled = False
        LAST_SEASON = 10
        season_Buttons_VISIBLE = False
        CAMO_BUTTONS_VISIBLE = False
        TANKPARTS_VISIBLE = False
        TANK_TEXTURES_VISIBLE = False

        show_textures_cb.Checked = False
        m_write_primitive.Enabled = False
        m_show_fbx.Checked = False
        m_show_bsp2.Checked = False

        GLOBAL_exclusionMask = 0
        exclusionMask_sd = -1
        HD_TANK = True
        turret_count = 0
        hull_count = 0
        XML_Strings(1) = ""
        XML_Strings(2) = ""
        XML_Strings(3) = ""
        XML_Strings(4) = ""
        log_text.Clear()
        If Not bb_texture_list(0) = "" Then
            For i = 0 To bb_texture_list.Length - 1
                Gl.glDeleteTextures(1, bb_texture_ids(i))
                Gl.glFinish()
                Gl.glDeleteTextures(1, bb_camo_texture_ids(i))
                Gl.glFinish()
            Next
        End If
        ReDim textures(0)

        '-------------------------------------------------------
        If object_count > 0 Then
            For i = 1 To object_count
                Gl.glDeleteLists(_object(i).main_display_list, 1)
                Gl.glFinish()
                Gl.glDeleteLists(_object(i).vertex_pick_list, 1)
                Gl.glFinish()
                Gl.glDeleteLists(_object(i).uv2_display_list, 1)
                Gl.glFinish()
                Gl.glDeleteLists(_group(i).bsp2_id, 1)
                Gl.glFinish()
                Gl.glDeleteLists(_group(i).bsp2_tree_id, 1)
                Gl.glFinish()
                '-------
                Gl.glDeleteTextures(1, _group(i).color_Id)
                Gl.glFinish()
                Gl.glDeleteTextures(1, _group(i).normal_Id)
                Gl.glFinish()
                Gl.glDeleteTextures(1, _group(i).detail_Id)
                Gl.glFinish()
                Gl.glDeleteTextures(1, _group(i).ao_id)
                Gl.glFinish()
                Gl.glDeleteTextures(1, _group(i).metalGMM_Id)
                Gl.glFinish()
            Next
        End If
        object_count = 0
        ReDim _group(0)
        _group(0) = New _grps
        ReDim _object(0)
        _object(0) = New obj
        object_count = 0
    End Sub

    Private Function validate_path(ByVal name As String)
        Dim ent = packages(current_tank_package)(name)
        If ent IsNot Nothing Then
            Return name
        End If
        ent = packages(11)(name)
        If ent IsNot Nothing Then
            Return name
        End If
        ent = packages_2(current_tank_package)(name)
        If ent IsNot Nothing Then
            Return name
        End If
        Return ""
    End Function

    '##################################################################################
    Public Sub process_tank(ByVal save_tank As Boolean)
        'need to set these before loading anyhing
        clean_house()
        '===================================
        log_text.Append(" ======== Model Load Start =========" + vbCrLf)
        Dim ar = file_name.Split(":")
        file_name = ar(2)
        Me.Text = "File: " + file_name

        Dim ts = ar(1)
        ar = ts.Split("/")
        For i = 0 To ar.Length - 1
            If ar(i).ToLower.Contains("level_") Then
                ts = ar(i)
                Exit For
            End If
        Next
        ar = ts.Split("_")
        ts = ar(2)
        If ts.Contains("-part") Then
            ar = ts.Split("-")
        Else
            ar = ts.Split(".")

        End If
        Dim fd As String = "lod0"
        Try
            current_tank_package = CInt(ar(0))

        Catch ex As Exception
            Try
                If ts.ToLower.Contains("tanks") Then
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
        '########################################################
        'get the tank info from scripts package
        ar = file_name.Split("/")
        Dim xml_file = ar(0) + "\" + ar(1) + "\" + ar(2) + ".xml"
        Dim t As New DataSet
        get_tank_parts_from_xml(xml_file, t)
        If t.Tables.Count = 0 Then
            Return
        End If
        '-----------------------------------
        'see if this is the old style tanks
        If GLOBAL_exclusionMask = 1 Then
            Dim et = t.Tables("exclusionMask")
            Dim eq = From row In et.AsEnumerable _
                        Select _
                        na = row.Field(Of String)("name")
            exclusionMask_name = eq(0)
            Dim en = packages(current_tank_package)(exclusionMask_name)
            Dim ms As New MemoryStream
            If en Is Nothing Then
                en = packages(11)(exclusionMask_name)
                If en Is Nothing Then
                    en = shared_sandbox_pkg(exclusionMask_name)
                End If
            End If
            If en IsNot Nothing Then
                en.Extract(ms)
                exclusionMask_id = get_texture(ms, exclusionMask_name)
            Else
                log_text.AppendLine("unable to locate : " + exclusionMask_name)
            End If
            et.Dispose()
        End If
        '-------------------------------------------------------
        'Return
        'get take part paths from table
        Dim turrets(10) As String
        Dim guns(10) As String
        Dim hulls(10) As String
        Dim chassis(10) As String
        ReDim hull_tile(10)
        ReDim gun_tile(10)
        ReDim turret_tile(10)
        Dim cnt As Integer = 0

        Dim tbl = t.Tables("gun")
        Dim q = From row In tbl.AsEnumerable _
                Select _
                g_name = row.Field(Of String)("gun_name"), _
                model = row.Field(Of String)("model"), _
                tile = row.Field(Of String)("gun_camouflage") Distinct
        cnt = 0
        '-------------------------------------------------------
        'guns
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
        '-------------------------------------------------------
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
        '-------------------------------------
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
        '-------------------------------------------------------
        'setup treeview and its nodes
        Dim selected_turret, selected_gun As Integer
        If Not save_tank Then
            frmComponents.tv_guns.Nodes.Clear()
            frmComponents.tv_turrets.Nodes.Clear()

            Dim cn As Integer = 0
            For i = 0 To guns.Length - 2
                If validate_path(guns(i)) = guns(i) Then
                    Dim n = New TreeNode

                    n.Text = Path.GetFileNameWithoutExtension(guns(i))
                    n.Tag = i
                    'tv_guns.Nodes.Add(n)
                    frmComponents.tv_guns.Nodes.Add(n)
                    cn += 1
                End If
            Next
            'frmComponents.tv_guns.Nodes.Add(tv_guns)
            frmComponents.tv_guns.SelectedNode = frmComponents.tv_guns.Nodes(cn - 1)
            frmComponents.tv_guns.SelectedNode.Checked = True
            selected_gun = cn
            '-------------------------------------------------------
            cn = 0
            For i = 0 To turrets.Length - 2
                If validate_path(turrets(i)) = turrets(i) Then
                    Dim n = New TreeNode
                    n.Text = Path.GetFileNameWithoutExtension(turrets(i))
                    n.Tag = i
                    frmComponents.tv_turrets.Nodes.Add(n)
                    cn += 1
                End If
            Next
            frmComponents.tv_turrets.SelectedNode = frmComponents.tv_turrets.Nodes(cn - 1)
            frmComponents.tv_turrets.SelectedNode.Checked = True
            selected_turret = cn
            '-------------------------------------------------------
            If frmFBX.Visible Then ' if fbx export form is visble, place the components form next to it
                Dim l = frmFBX.Location
                l.X -= frmComponents.Width
                frmComponents.Location = l
            Else
                frmComponents.Location = Me.Location + New Point(200, 200)
            End If
            frmComponents.ShowDialog(Me)
            If frmFBX.Visible Then
                frmFBX.Location = Me.Location
            End If
        End If
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
        '-------------------------------------------------------
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
        '-------------------------------------------------------
        'Array.Sort(guns)
        'Array.Sort(turrets)
        'Array.Sort(hulls)
        'Array.Sort(chassis)
        'more hacks to deal with turret names
        Dim turret_name As String
        Try
            turret_name = turrets(turrets.Length - 3)
        Catch ex1 As Exception
            Try
                turret_name = turrets(turrets.Length - 2)
            Catch ex2 As Exception
                turret_name = turrets(turrets.Length - 1)
            End Try

        End Try
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
        Dim nation_string As String = ""
        Select Case ar(1)
            Case "american"
                CURRENT_DATA_SET = 0
                nation_string = "usa"
            Case "british"
                CURRENT_DATA_SET = 1
                nation_string = "uk"
            Case "chinese"
                CURRENT_DATA_SET = 2
                nation_string = "china"
            Case "czech"
                CURRENT_DATA_SET = 3
                nation_string = "czech"
            Case "french"
                CURRENT_DATA_SET = 4
                nation_string = "france"
            Case "german"
                CURRENT_DATA_SET = 5
                nation_string = "germany"
            Case "japan"
                CURRENT_DATA_SET = 6
                nation_string = "japan"
            Case "poland"
                CURRENT_DATA_SET = 7
                nation_string = "poland"
            Case "russian"
                CURRENT_DATA_SET = 8
                nation_string = "ussr"
            Case "sweden"
                CURRENT_DATA_SET = 9
                nation_string = "sweden"
            Case "italy"
                CURRENT_DATA_SET = 10
                nation_string = "italy"
        End Select
        TANK_NAME = "vehicles\" + ar(1) + "\" + ar(2) + ":" + current_tank_package.ToString
        '===================================
        Dim d = custom_tables(CURRENT_DATA_SET).Copy
        '===================================

        Dim tt = d.Tables("camouflage")
        Dim qq = From row In tt.AsEnumerable
        Select _
        armorC = row.Field(Of String)("armorcolor")
        ARMORCOLOR = get_vect4(qq(0))
        tt.Dispose()

        'clear tank variables
        gun_trans = New vect3
        gun_trans2 = New vect3
        turret_trans = New vect3
        hull_trans = New vect3
        gun_location = New vect3
        turret_location = New vect3
        'make sure visiblity check boxes are checked
        If Not chassis_cb.Checked Then
            chassis_cb.Checked = True
        End If
        If Not hull_cb.Checked Then
            hull_cb.Checked = True
        End If
        If Not turret_cb.Checked Then
            turret_cb.Checked = True
        End If
        If Not gun_cb.Checked Then
            gun_cb.Checked = True
        End If
        '-------------------------------------------------------
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
            If track_info.segment_offset2 > track_info.segment_offset1 Then
                Dim t_seg = track_info.segment_offset1
                track_info.segment_offset1 = track_info.segment_offset2
                track_info.segment_offset2 = t_seg
            End If
            running = 0
            path_pointer1 = 0
            track_length = 0
            For i = 0 To tracks.Length - 1
                catmullrom.CatmullRomSpline_get_length(i)
            Next
            Dim lenS = running / track_info.segment_length
            If Z_Flipped Then
                lenS -= 1.0
            Else
                lenS -= 1.0

            End If
            segment_length_adjusted = running / (Floor(lenS))
            Dim refact = track_info.segment_length / segment_length_adjusted
            track_info.segment_offset1 /= refact
            track_info.segment_offset2 /= refact
            Dim half = track_info.segment_offset2
            '========= segment 1 =========
            ReDim path_data1(CInt(Floor(lenS)) + 3)
            If Z_Flipped Then
                running = 0 + track_info.segment_offset1 + half
            Else
                running = 0 + track_info.segment_offset1 + half
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
            '========= segment 2 =========
            If track_info.segment_count = 2 Then
                ReDim path_data2(CInt(Floor(lenS)) + 3)
                track_length = 0
                If Z_Flipped Then
                    running = 0 + track_info.segment_offset2 + half
                Else
                    running = 0 + track_info.segment_offset2 + half
                End If
                path_pointer2 = 0
                For i = 0 To tracks.Length - 1
                    catmullrom.GetCatmullRomSpline2(i)
                Next
                ReDim Preserve path_data2(path_pointer2)
                get_tread_rotations2()
            End If

        End If
        '================================= end testing
        file_name = chassis_name
        build_primitive_data(False) ' -- chassis
        If stop_updating Then draw_scene()

        file_name = hull_name
        build_primitive_data(True) ' -- chassis
        If stop_updating Then draw_scene()

        If save_tank Then

            file_name = turret_name
            build_primitive_data(True) ' -- chassis

            For gn = guns.Length - 2 To 0 Step -1
                file_name = guns(gn)
                gun_name = file_name
                If build_primitive_data(True) Then ' -- chassis
                    If stop_updating Then draw_scene()
                    Exit For
                End If

            Next
        Else
            file_name = turrets(frmComponents.tv_turrets.SelectedNode.Tag)
            build_primitive_data(True) ' -- turret
            If stop_updating Then draw_scene()

            file_name = guns(frmComponents.tv_guns.SelectedNode.Tag)
            build_primitive_data(True) ' -- gun
            If stop_updating Then draw_scene()

        End If


        If TESTING Then
            file_name = track_info.left_path1
            build_primitive_data(True) ' -- chassis
            If stop_updating Then draw_scene()

            If track_info.segment_count = 2 Then
                file_name = track_info.left_path2
                build_primitive_data(True) ' -- chassis
                If stop_updating Then draw_scene()
            End If

        End If


        MODEL_LOADED = True
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
        '####################################
        'All the tank parts are loaded so
        'lets create the color picking lists.
        'This should speed up color picking a lot.
        Dim r, b, g, a As Byte
        For i = 1 To object_count
            Dim cpl = Gl.glGenLists(1)
            _object(i).vertex_pick_list = cpl
            Gl.glNewList(cpl, Gl.GL_COMPILE)
            a = i + 10
            Gl.glEnable(Gl.GL_CULL_FACE)
            If _group(i).is_carraige Then
                Gl.glFrontFace(Gl.GL_CW)
            Else
                Gl.glFrontFace(Gl.GL_CCW)
            End If
            If _object(i).visible Then
                Gl.glBegin(Gl.GL_TRIANGLES)
                For k As UInt32 = 1 To _object(i).count
                    Dim v1 = _object(i).tris(k).v1
                    Dim v2 = _object(i).tris(k).v2
                    Dim v3 = _object(i).tris(k).v3
                    r = k And &HFF
                    g = (k And &HFF00) >> 8
                    b = (k And &HFF0000) >> 16
                    Gl.glColor4ub(r, g, b, a)
                    Gl.glVertex3f(v1.x, v1.y, v1.z)
                    Gl.glVertex3f(v2.x, v2.y, v2.z)
                    Gl.glVertex3f(v3.x, v3.y, v3.z)
                Next
                Gl.glEnd()
            End If
            Gl.glEndList()
        Next
        log_text.Append(" ======== Model Load Complete =========" + vbCrLf)

        If save_tank Then

            Dim rot_limit_l, rot_limit_r As Single
            Dim gun_limit_u, gun_limit_d As Single
            rot_limit_l = -400.0
            rot_limit_r = 400.0

            Dim ent = scripts_pkg("scripts\item_defs\vehicles\" + nation_string + "\" + ar(2) + ".xml")
            Dim ms As New MemoryStream
            ent.Extract(ms)
            openXml_stream(ms, "")
            ms.Dispose()
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

            Dim fo = File.Open(TankListTempFolder + ar(2) + ".tank", FileMode.OpenOrCreate)
            Dim fw As New BinaryWriter(fo)
            'version changes
            'ver 1 

            Dim version As Integer = 1
            Dim rotation_limit As Single = 0.0
            'ver 1
            Dim s1 = "File format: 1 INT32 as version, INT32 as chassis and hull vertex count, INT32 as turret vertex count, INT32 as Gun vertex Count."
            Dim s2 = "3 Floats turret pivot center XYZ, " + _
                    "2 Floats rotation limits L&R,"
            Dim s3 = "3 Floats gun pivot point XYZ , 2 Floats gun limits U&D, " + _
                    "6 Floats as list of vertices:Each being (position XYZ Normal XYZ), "
            Dim s4 = "9 Floats for future use."
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
        t.Dispose()
        tbl.Dispose()
        GC.Collect()
        If FBX_LOADED Then
            m_show_fbx.Visible = True
            m_show_fbx.Checked = False
        End If
        m_pick_camo.Enabled = True
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



    Public Sub extract_selections()
        If Not My.Settings.res_mods_path.ToLower.Contains("res_mods") Then
            If MsgBox("You need to set the path to the res_mods folder!" + vbCrLf + _
                    "Set it Now and continue?" + vbCrLf + _
                    "It should be something like this:" + vbCrLf + _
                    "C:\Games\World_of_Tanks\res_mods\0.9.20.0", MsgBoxStyle.YesNo, "Opps..") = MsgBoxResult.Yes Then
                m_res_mods_path.PerformClick()
                If Not My.Settings.res_mods_path.ToLower.Contains("res_mods") Then
                    Return
                End If
            Else
                Return
            End If
        End If
        Dim all_lods As Boolean = False
        Dim models As Boolean = frmExtract.no_models.Checked
        If frmExtract.all_lods_rb.Checked Then
            all_lods = True
        Else
            all_lods = False
        End If
        Dim p As String = ""
        TC1.Enabled = False
        Dim ar = file_name.Split(":")
        If frmExtract.m_customization.Checked Then ' export customization?

            Try ' catch any exception thrown

                Dim nar = ar(2).Split("/")
                Dim nation = nar(1)
                Select Case nation
                    Case "russian"
                        nation = "ussr"
                    Case "british"
                        nation = "uk"
                    Case "american"
                        nation = "usa"
                    Case "french"
                        nation = "france"
                    Case "german"
                        nation = "germany"
                    Case "chinese"
                        nation = "china"
                End Select
                Dim cust_path As String = "scripts\item_defs\vehicles\" + nation + "\customization.xml"
                Dim c_ent = scripts_pkg(cust_path)
                If c_ent IsNot Nothing Then
                    c_ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                End If
            Catch ex As Exception

            End Try
        End If

        For i = 1 To packages.Length - 2
            For Each ent In packages(i)
                If ent.FileName.Contains(ar(2)) Then
                    If Not ent.FileName.Contains("collision_client") Then
                        If Not ent.FileName.Contains("crash") Then
                            If Not models Then
                                Select Case all_lods
                                    Case True
                                        Select Case frmExtract.ext_chassis.Checked
                                            Case True
                                                If ent.FileName.ToLower.Contains("chassis") Then
                                                    ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                End If
                                        End Select
                                        Select Case frmExtract.ext_hull.Checked
                                            Case True
                                                If ent.FileName.ToLower.Contains("hull") Then
                                                    ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                End If
                                        End Select
                                        Select Case frmExtract.ext_turret.Checked
                                            Case True
                                                If ent.FileName.ToLower.Contains("turret") Then
                                                    ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                End If
                                        End Select
                                        Select Case frmExtract.ext_gun.Checked
                                            Case True
                                                If ent.FileName.ToLower.Contains("gun") Then
                                                    ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                End If
                                        End Select
                                    Case False
                                        If ent.FileName.ToLower.Contains("lod0") Then
                                            Select Case frmExtract.ext_chassis.Checked
                                                Case True
                                                    If ent.FileName.ToLower.Contains("chassis") Then
                                                        ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                    End If
                                            End Select
                                            Select Case frmExtract.ext_hull.Checked
                                                Case True
                                                    If ent.FileName.ToLower.Contains("hull") Then
                                                        ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                    End If
                                            End Select
                                            Select Case frmExtract.ext_turret.Checked
                                                Case True
                                                    If ent.FileName.ToLower.Contains("turret") Then
                                                        ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                    End If
                                            End Select
                                            Select Case frmExtract.ext_gun.Checked
                                                Case True
                                                    If ent.FileName.ToLower.Contains("gun") Then
                                                        ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                    End If
                                            End Select
                                        End If
                                End Select
                            End If 'if model
                            Select Case ent.FileName.Contains("dds")
                                Case True
                                    Select Case frmExtract.ext_chassis.Checked
                                        Case True
                                            If ent.FileName.ToLower.Contains("chassis") Then
                                                ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                            End If
                                    End Select
                                    Select Case frmExtract.ext_hull.Checked
                                        Case True
                                            If ent.FileName.ToLower.Contains("hull") Then
                                                ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                            End If
                                    End Select
                                    Select Case frmExtract.ext_turret.Checked
                                        Case True
                                            If ent.FileName.ToLower.Contains("turret") Then
                                                ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                            End If
                                    End Select
                                    Select Case frmExtract.ext_gun.Checked
                                        Case True
                                            If ent.FileName.ToLower.Contains("gun") Then
                                                ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                            End If
                                    End Select
                            End Select
                        End If ' crash
                    End If ' collision_client
                End If ' filename match
            Next ' next entry
        Next 'next package
        ' now check package_2 data
        Dim start_from As Integer = 5 ' This might change and its used in 2 places
        For i = start_from To 10
            For Each ent In packages_2(i)
                If ent.FileName.Contains(ar(2)) Then
                    If Not ent.FileName.Contains("collision_client") Then
                        If Not ent.FileName.Contains("crash") Then
                            If Not models Then
                                Select Case all_lods
                                    Case True
                                        Select Case frmExtract.ext_chassis.Checked
                                            Case True
                                                If ent.FileName.ToLower.Contains("chassis") Then
                                                    ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                End If
                                        End Select
                                        Select Case frmExtract.ext_hull.Checked
                                            Case True
                                                If ent.FileName.ToLower.Contains("hull") Then
                                                    ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                End If
                                        End Select
                                        Select Case frmExtract.ext_turret.Checked
                                            Case True
                                                If ent.FileName.ToLower.Contains("turret") Then
                                                    ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                End If
                                        End Select
                                        Select Case frmExtract.ext_gun.Checked
                                            Case True
                                                If ent.FileName.ToLower.Contains("gun") Then
                                                    ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                End If
                                        End Select
                                    Case False
                                        If ent.FileName.ToLower.Contains("lod0") Then
                                            Select Case frmExtract.ext_chassis.Checked
                                                Case True
                                                    If ent.FileName.ToLower.Contains("chassis") Then
                                                        ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                    End If
                                            End Select
                                            Select Case frmExtract.ext_hull.Checked
                                                Case True
                                                    If ent.FileName.ToLower.Contains("hull") Then
                                                        ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                    End If
                                            End Select
                                            Select Case frmExtract.ext_turret.Checked
                                                Case True
                                                    If ent.FileName.ToLower.Contains("turret") Then
                                                        ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                    End If
                                            End Select
                                            Select Case frmExtract.ext_gun.Checked
                                                Case True
                                                    If ent.FileName.ToLower.Contains("gun") Then
                                                        ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                    End If
                                            End Select
                                        End If
                                End Select
                            End If 'if model
                            Select Case ent.FileName.Contains("dds")
                                Case True
                                    Select Case frmExtract.ext_chassis.Checked
                                        Case True
                                            If ent.FileName.ToLower.Contains("chassis") Then
                                                ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                            End If
                                    End Select
                                    Select Case frmExtract.ext_hull.Checked
                                        Case True
                                            If ent.FileName.ToLower.Contains("hull") Then
                                                ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                            End If
                                    End Select
                                    Select Case frmExtract.ext_turret.Checked
                                        Case True
                                            If ent.FileName.ToLower.Contains("turret") Then
                                                ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                            End If
                                    End Select
                                    Select Case frmExtract.ext_gun.Checked
                                        Case True
                                            If ent.FileName.ToLower.Contains("gun") Then
                                                ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                            End If
                                    End Select
                            End Select
                        End If ' crash
                    End If ' collision_client
                End If ' filename match
            Next ' next entry
        Next 'next package
        ' now check package_hd data
        For i = 1 To packages_HD.Length - 2
            If packages_HD(i) IsNot Nothing Then

                For Each ent In packages_HD(i)
                    If ent.FileName.Contains(ar(2)) Then
                        If Not ent.FileName.Contains("collision_client") Then
                            If Not ent.FileName.Contains("crash") Then
                                If Not models Then
                                    Select Case all_lods
                                        Case True
                                            Select Case frmExtract.ext_chassis.Checked
                                                Case True
                                                    If ent.FileName.ToLower.Contains("chassis") Then
                                                        ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                    End If
                                            End Select
                                            Select Case frmExtract.ext_hull.Checked
                                                Case True
                                                    If ent.FileName.ToLower.Contains("hull") Then
                                                        ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                    End If
                                            End Select
                                            Select Case frmExtract.ext_turret.Checked
                                                Case True
                                                    If ent.FileName.ToLower.Contains("turret") Then
                                                        ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                    End If
                                            End Select
                                            Select Case frmExtract.ext_gun.Checked
                                                Case True
                                                    If ent.FileName.ToLower.Contains("gun") Then
                                                        ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                    End If
                                            End Select
                                        Case False
                                            If ent.FileName.ToLower.Contains("lod0") Then
                                                Select Case frmExtract.ext_chassis.Checked
                                                    Case True
                                                        If ent.FileName.ToLower.Contains("chassis") Then
                                                            ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                        End If
                                                End Select
                                                Select Case frmExtract.ext_hull.Checked
                                                    Case True
                                                        If ent.FileName.ToLower.Contains("hull") Then
                                                            ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                        End If
                                                End Select
                                                Select Case frmExtract.ext_turret.Checked
                                                    Case True
                                                        If ent.FileName.ToLower.Contains("turret") Then
                                                            ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                        End If
                                                End Select
                                                Select Case frmExtract.ext_gun.Checked
                                                    Case True
                                                        If ent.FileName.ToLower.Contains("gun") Then
                                                            ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                        End If
                                                End Select
                                            End If
                                    End Select
                                End If 'if model
                                Select Case ent.FileName.Contains("dds")
                                    Case True
                                        Select Case frmExtract.ext_chassis.Checked
                                            Case True
                                                If ent.FileName.ToLower.Contains("chassis") Then
                                                    ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                    p = ent.FileName
                                                End If
                                        End Select
                                        Select Case frmExtract.ext_hull.Checked
                                            Case True
                                                If ent.FileName.ToLower.Contains("hull") Then
                                                    ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                    p = ent.FileName
                                                End If
                                        End Select
                                        Select Case frmExtract.ext_turret.Checked
                                            Case True
                                                If ent.FileName.ToLower.Contains("turret") Then
                                                    ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                    p = ent.FileName
                                                End If
                                        End Select
                                        Select Case frmExtract.ext_gun.Checked
                                            Case True
                                                If ent.FileName.ToLower.Contains("gun") Then
                                                    ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                    p = ent.FileName
                                                End If
                                        End Select
                                End Select
                            End If ' crash
                        End If ' collision_client
                    End If ' filename match
                Next ' next entry
            End If 'isnot nothing
        Next 'next package
        'now check package_hd_2 data
        For i = start_from To packages_HD.Length - 2
            If packages_HD_2(i) IsNot Nothing Then

                For Each ent In packages_HD(i)
                    If ent.FileName.Contains(ar(2)) Then
                        If Not ent.FileName.Contains("collision_client") Then
                            If Not ent.FileName.Contains("crash") Then
                                If Not models Then
                                    Select Case all_lods
                                        Case True
                                            Select Case frmExtract.ext_chassis.Checked
                                                Case True
                                                    If ent.FileName.ToLower.Contains("chassis") Then
                                                        ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                    End If
                                            End Select
                                            Select Case frmExtract.ext_hull.Checked
                                                Case True
                                                    If ent.FileName.ToLower.Contains("hull") Then
                                                        ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                    End If
                                            End Select
                                            Select Case frmExtract.ext_turret.Checked
                                                Case True
                                                    If ent.FileName.ToLower.Contains("turret") Then
                                                        ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                    End If
                                            End Select
                                            Select Case frmExtract.ext_gun.Checked
                                                Case True
                                                    If ent.FileName.ToLower.Contains("gun") Then
                                                        ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                    End If
                                            End Select
                                        Case False
                                            If ent.FileName.ToLower.Contains("lod0") Then
                                                Select Case frmExtract.ext_chassis.Checked
                                                    Case True
                                                        If ent.FileName.ToLower.Contains("chassis") Then
                                                            ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                        End If
                                                End Select
                                                Select Case frmExtract.ext_hull.Checked
                                                    Case True
                                                        If ent.FileName.ToLower.Contains("hull") Then
                                                            ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                        End If
                                                End Select
                                                Select Case frmExtract.ext_turret.Checked
                                                    Case True
                                                        If ent.FileName.ToLower.Contains("turret") Then
                                                            ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                        End If
                                                End Select
                                                Select Case frmExtract.ext_gun.Checked
                                                    Case True
                                                        If ent.FileName.ToLower.Contains("gun") Then
                                                            ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                        End If
                                                End Select
                                            End If
                                    End Select
                                End If 'if model
                                Select Case ent.FileName.Contains("dds")
                                    Case True
                                        Select Case frmExtract.ext_chassis.Checked
                                            Case True
                                                If ent.FileName.ToLower.Contains("chassis") Then
                                                    ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                    p = ent.FileName
                                                End If
                                        End Select
                                        Select Case frmExtract.ext_hull.Checked
                                            Case True
                                                If ent.FileName.ToLower.Contains("hull") Then
                                                    ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                    p = ent.FileName
                                                End If
                                        End Select
                                        Select Case frmExtract.ext_turret.Checked
                                            Case True
                                                If ent.FileName.ToLower.Contains("turret") Then
                                                    ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                    p = ent.FileName
                                                End If
                                        End Select
                                        Select Case frmExtract.ext_gun.Checked
                                            Case True
                                                If ent.FileName.ToLower.Contains("gun") Then
                                                    ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                    p = ent.FileName
                                                End If
                                        End Select
                                End Select
                            End If ' crash
                        End If ' collision_client
                    End If ' filename match
                Next ' next entry
            End If 'isnot nothing
        Next 'next package
        If frmExtract.create_work_area_cb.Checked Then
            p = My.Settings.res_mods_path + "\" + Path.GetDirectoryName(p)
            Dim wap = p + "\Work Area"
            Il.ilDisable(Il.IL_FILE_OVERWRITE) ' dont allow devil to overwrite existing PNGS.. Preserver the users work!
            If Not Directory.Exists(wap) Then
                Directory.CreateDirectory(wap)
                Dim di = Directory.GetFiles(p)
                Dim id As Integer = 0
                For Each img In di
                    If img.ToLower.Contains("_am_hd.dds") Or img.ToLower.Contains("_ao_hd.dds") Then
                        Dim tp = Path.GetDirectoryName(img)
                        Dim t_tn = Path.GetFileNameWithoutExtension(img)
                        Dim out_path As String = tp + "\Work Area\" + t_tn + ".png"
                        id = Il.ilGenImage()
                        Il.ilBindImage(id)
                        Ilu.iluLoadImage(img)
                        Il.ilSave(Il.IL_PNG, out_path)
                        Il.ilBindImage(0)
                        Il.ilDeleteImage(id)
                    End If

                Next
            End If
        End If
        TC1.Enabled = True
    End Sub

#Region "menu_button_functions"
    Private Sub m_load_Click(sender As Object, e As EventArgs) Handles m_load.Click
        TC1.Enabled = False
        current_tank_name = file_name
        short_tank_name = tank_label.Text
        process_tank(False) 'false .. don't save the binary tank file
        m_ExportExtract.Enabled = True
        TC1.Enabled = True
    End Sub

    Private Sub m_clear_temp_folder_data_Click(sender As Object, e As EventArgs) Handles m_clear_temp_folder_data.Click
        clear_temp_folder()
    End Sub

    Private Sub m_reload_api_data_Click(sender As Object, e As EventArgs) Handles m_reload_api_data.Click
        info_Label.Visible = True
        get_tank_names()
        info_Label.Visible = False
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
        TC1.Enabled = False
        Dim f As DirectoryInfo = New DirectoryInfo(Application.StartupPath + "\tanks\")
        If f.Exists Then
            For Each fi In f.GetFiles
                If Not fi.Name.Contains(".txt") Then
                    fi.Delete()
                End If
            Next
        End If
        IGNORE_TEXTURES = True
        show_textures_cb.Checked = False
        Application.DoEvents()
        MM.Enabled = False
        Dim show_text_State = m_load_textures.Checked
        m_load_textures.Checked = False
        Application.DoEvents()
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
                MODEL_LOADED = False
            End If
        Next
        MODEL_LOADED = True
        TC1.Enabled = True
        IGNORE_TEXTURES = False
        MM.Enabled = True
        m_load_textures.Checked = show_text_State
    End Sub

    Private Sub m_create_and_extract_Click(sender As Object, e As EventArgs)
        frmExtract.ShowDialog(Me)
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
        Dim in_s = IO.File.ReadAllText(TankListTempFolder + "tanknames.txt")
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
        m_export_tank_list.Visible = True
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

        File.WriteAllText(TankListTempFolder + "tanknames.txt", out_string.ToString)
    End Sub

    Private Sub m_open_temp_folder_Click(sender As Object, e As EventArgs) Handles m_open_temp_folder.Click
        Dim f As DirectoryInfo = New DirectoryInfo(Temp_Storage)
        If f.Exists Then
            Process.Start("explorer.exe", TankListTempFolder)
        End If

    End Sub

    Private Sub m_export_fbx_Click(sender As Object, e As EventArgs)

        SaveFileDialog1.Filter = "AutoDesk (*.FBX)|*.fbx"
        SaveFileDialog1.Title = "Export FBX..."
        SaveFileDialog1.InitialDirectory = My.Settings.fbx_path
        SaveFileDialog1.FileName = tank_label.Text.Replace("\/", "_") + ".fbx"

        If SaveFileDialog1.ShowDialog = Forms.DialogResult.OK Then
            remove_loaded_fbx()
            clean_house()
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
        File.WriteAllText(t, log_text.ToString + vbCrLf + start_up_log.ToString)

        System.Diagnostics.Process.Start("notepad.exe", t)
    End Sub

    Private Sub m_res_mods_path_Click(sender As Object, e As EventArgs) Handles m_res_mods_path.Click
        FolderBrowserDialog1.SelectedPath = My.Settings.res_mods_path
        If FolderBrowserDialog1.ShowDialog = Forms.DialogResult.OK Then
            My.Settings.res_mods_path = FolderBrowserDialog1.SelectedPath
            'If Not File.Exists(My.Settings.res_mods_path) Then
            '    MsgBox("Incorrect Path.", MsgBoxStyle.Information)
            '    m_res_mods_path.PerformClick()
            '    Return
            'End If
            res_mods_path_set = True
            My.Settings.res_mods_path = FolderBrowserDialog1.SelectedPath
            File.WriteAllText(Temp_Storage + "\res_mods_Path.txt", My.Settings.res_mods_path)
            My.Settings.Save()
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
            File.WriteAllText(Temp_Storage + "\game_Path.txt", My.Settings.game_path)
            My.Settings.Save()
            Return
        End If
    End Sub

    Private Sub m_pick_camo_Click(sender As Object, e As EventArgs) Handles m_pick_camo.Click
        setup_camo_selection()
    End Sub

    Private Sub m_edit_shaders_Click(sender As Object, e As EventArgs) Handles m_edit_shaders.Click
        frmShaderEditor.Show()
    End Sub

    Private Sub M_Exit_Click(sender As Object, e As EventArgs) Handles M_Exit.Click
        Me.Close()
    End Sub

    Private Sub m_lighting_Click(sender As Object, e As EventArgs) Handles m_lighting.Click
        If Not frmLighting.Visible Then
            frmLighting.Show()
        Else
            frmLighting.Hide()
        End If
    End Sub

    Private Sub m_help_Click(sender As Object, e As EventArgs) Handles m_help.Click
        Process.Start(Application.StartupPath + "\html\MainPage.html")
    End Sub

    Private Sub m_show_bsp2_CheckedChanged(sender As Object, e As EventArgs) Handles m_show_bsp2.CheckedChanged
        If m_show_bsp2.Checked Then
            m_show_bsp2.ForeColor = Color.Red
        Else
            m_show_bsp2.ForeColor = Color.Black
        End If
    End Sub

    Private Sub m_Import_FBX_Click(sender As Object, e As EventArgs) Handles m_Import_FBX.Click
        MM.Enabled = False
        TC1.Enabled = False
        import_FBX()
        MM.Enabled = True
        TC1.Enabled = True
    End Sub

    Private Sub m_show_fbx_CheckedChanged(sender As Object, e As EventArgs) Handles m_show_fbx.CheckedChanged
        If m_show_fbx.Checked Then
            m_show_fbx.Text = "Show Model"
        Else
            m_show_fbx.Text = "Show FBX"
        End If
        If stop_updating Then draw_scene()
    End Sub

    Private Sub m_remove_fbx_Click(sender As Object, e As EventArgs) Handles m_remove_fbx.Click
        m_ExportExtract.Enabled = False
        clean_house()
        remove_loaded_fbx()
        If stop_updating Then draw_scene()

    End Sub

    Private Sub chassis_cb_CheckedChanged(sender As Object, e As EventArgs) Handles chassis_cb.CheckedChanged
        If Not _Started Then Return
        For i = 1 To object_count
            If _object(i).name.ToLower.Contains("chassis") Then
                _object(i).visible = chassis_cb.Checked
            End If
        Next
        If stop_updating Then draw_scene()
    End Sub

    Private Sub hull_cb_CheckedChanged(sender As Object, e As EventArgs) Handles hull_cb.CheckedChanged
        If Not _Started Then Return
        For i = 1 To object_count
            If _object(i).name.ToLower.Contains("hull") Then
                _object(i).visible = hull_cb.Checked
            End If
        Next
        If stop_updating Then draw_scene()
    End Sub

    Private Sub turret_cb_CheckedChanged(sender As Object, e As EventArgs) Handles turret_cb.CheckedChanged
        If Not _Started Then Return
        For i = 1 To object_count
            If _object(i).name.ToLower.Contains("turret") Then
                _object(i).visible = turret_cb.Checked
            End If
        Next
        If stop_updating Then draw_scene()
    End Sub

    Private Sub gun_cb_CheckedChanged(sender As Object, e As EventArgs) Handles gun_cb.CheckedChanged
        If Not _Started Then Return
        For i = 1 To object_count
            If _object(i).name.ToLower.Contains("gun") Then
                _object(i).visible = gun_cb.Checked
            End If
        Next
        If stop_updating Then draw_scene()

    End Sub

    Private Sub grid_cb_CheckStateChanged(sender As Object, e As EventArgs) Handles grid_cb.CheckStateChanged
        If grid_cb.Checked Then
            grid_cb.BackgroundImage = My.Resources.grid
        Else
            grid_cb.BackgroundImage = My.Resources.grid_dark
        End If
        If stop_updating Then draw_scene()
    End Sub

    Private Sub wire_cb_CheckedChanged(sender As Object, e As EventArgs) Handles wire_cb.CheckedChanged
        If wire_cb.Checked Then
            wire_cb.BackgroundImage = My.Resources.box_solid
        Else
            wire_cb.BackgroundImage = My.Resources.box_wire
        End If
        If stop_updating Then draw_scene()
    End Sub

    Private Sub m_show_bsp2_tree_CheckedChanged(sender As Object, e As EventArgs) Handles m_show_bsp2_tree.CheckedChanged
        If m_show_bsp2_tree.Checked Then
            m_show_bsp2_tree.ForeColor = Color.Red
        Else
            m_show_bsp2_tree.ForeColor = Color.Black
        End If
    End Sub

    Private Sub show_textures_cb_CheckedChanged(sender As Object, e As EventArgs) Handles show_textures_cb.CheckedChanged
        'make sure camo crap is not visible
        If season_Buttons_VISIBLE Then
            'CAMO_BUTTONS_VISIBLE = False
            'season_Buttons_VISIBLE = False
        End If
        '---------------------------------
        If show_textures_cb.Checked Then
            reset_tank_buttons()
            'STOP_BUTTON_SCAN = False
            TANKPARTS_VISIBLE = True
        Else
            'If Not season_Buttons_VISIBLE Then
            '    STOP_BUTTON_SCAN = True
            'End If
            TANKPARTS_VISIBLE = False
            TANK_TEXTURE_ID = 0
            TANK_TEXTURES_VISIBLE = False
        End If

        If stop_updating Then draw_scene()

    End Sub
#End Region


    Private Sub m_edit_visual_Click(sender As Object, e As EventArgs) Handles m_edit_visual.Click
        frmEditVisual.Show()
    End Sub

    Private Sub m_write_primitive_Click(sender As Object, e As EventArgs) Handles m_write_primitive.Click
        If FBX_LOADED Then
            frmWritePrimitive.ShowDialog(Me)
        End If
    End Sub

    Private Sub m_show_model_info_Click(sender As Object, e As EventArgs) Handles m_show_model_info.Click
        frmModelInfo.Show()
    End Sub

    Private Sub m_region_Click(sender As Object, e As EventArgs) Handles m_region.Click
        ToolStripComboBox1.Visible = True
    End Sub

    Private Sub ToolStripComboBox1_TextChanged(sender As Object, e As EventArgs) Handles ToolStripComboBox1.TextChanged
        If Not _Started Then Return ' dont want to cause a trigger here!
        API_REGION = ToolStripComboBox1.Text
        My.Settings.region_selection = API_REGION
        ToolStripComboBox1.Visible = False
        MsgBox("You will need to clear the temp folder (under menu)" + vbCrLf + _
                "and restart Tank Exporter." + vbCrLf + _
                "This had to be done to reload data for your region!", MsgBoxStyle.Exclamation, "Warning!")

        My.Settings.Save()

    End Sub


    Private Sub m_Shader_Debug_Click(sender As Object, e As EventArgs) Handles m_Shader_Debug.Click
        frmShaderDebugSettings.Show()

    End Sub

    Private Sub frmMain_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        If stop_updating Then draw_scene()
    End Sub

    Private Sub m_show_environment_CheckedChanged(sender As Object, e As EventArgs) Handles m_show_environment.CheckedChanged
        If stop_updating Then draw_scene()
    End Sub

    Private Sub m_shadow_preview_Click(sender As Object, e As EventArgs) Handles m_shadow_preview.Click
        If stop_updating Then draw_scene()

    End Sub

    Private Sub m_terrain_Click(sender As Object, e As EventArgs)
        If stop_updating Then draw_scene()
    End Sub

    Private Sub m_shadows_Click(sender As Object, e As EventArgs) Handles m_shadows.Click
        If stop_updating Then draw_scene()
    End Sub

    Private Sub m_shadowQuality_Click(sender As Object, e As EventArgs) Handles m_shadowQuality.Click
        FrmShadowSettings.Show()
    End Sub

    Private Sub m_select_light_Click(sender As Object, e As EventArgs) Handles m_select_light.Click
        frmLightSelection.Show()
    End Sub

    Private Sub m_decal_Click(sender As Object, e As EventArgs) Handles m_decal.Click
        If m_decal.Checked Then
            m_decal.ForeColor = Color.Red
            upton.position = New Point(pb1.Width - 150, -210)
            upton.state = 0
            decal_panel.Dock = DockStyle.Fill
            decal_panel.Visible = True
            decal_panel.BringToFront()
            TC1.Visible = False
            'make_test_decal(0)
        Else
            m_decal.ForeColor = Color.Black
            decal_panel.Visible = False
            TC1.Visible = True
        End If
        gl_busy = False
    End Sub

    Private Sub m_new_Click(sender As Object, e As EventArgs) Handles m_new.Click
        add_decal()
    End Sub

    Public d_sel_Len As Integer
    Public d_current_line As Integer
    Dim d_line_count As Integer
    Private Sub d_list_tb_Click(sender As Object, e As EventArgs) Handles d_list_tb.Click
        d_list_tb.SelectionLength = 0
        Dim t = d_list_tb.Text.Split(":")
        d_line_count = t.Length - 2
        If d_line_count = -1 Then Return
        Dim a = d_list_tb.GetLineFromCharIndex(d_list_tb.GetFirstCharIndexOfCurrentLine())
        d_list_tb.Select(d_list_tb.GetFirstCharIndexOfCurrentLine(), d_list_tb.Lines(a).Length)
        If d_list_tb.SelectedText.Length < 4 Then
            Return
            d_sel_Len = 0
            d_current_line = 0
        Else
            Dim d = d_list_tb.SelectedText.Split(":")
            current_decal = CInt(d(1))
            decal_matrix_list(current_decal).get_decals_transform_info()
            d_sel_Len = d_list_tb.Lines(a).Length
            d_current_line = a
            decal_matrix_list(current_decal).get_decals_transform_info()
        End If

    End Sub

    Private Sub d_move_up_Click(sender As Object, e As EventArgs) Handles d_move_up.Click
        If d_current_line = 0 Then
            Return
        End If
        If d_sel_Len > 0 Then
            Dim prev_line = d_current_line - 1
            Dim sel_tex_current = d_list_tb.SelectedText ' save current text
            If sel_tex_current = "" Then Return
            Dim sp = d_list_tb.GetFirstCharIndexFromLine(prev_line) ' get prev line
            Application.DoEvents()
            d_list_tb.SelectionStart = sp
            d_list_tb.Select(d_list_tb.GetFirstCharIndexOfCurrentLine(), _
                             d_list_tb.Lines(prev_line).Length) ' select prev line
            Application.DoEvents()
            Dim pre_text = d_list_tb.SelectedText ' save prev line text
            If pre_text = "" Then Return
            Application.DoEvents()
            d_list_tb.SelectedText = sel_tex_current ' replace current line text
            Application.DoEvents()

            d_list_tb.SelectionStart = sp + sel_tex_current.Length + 2
            d_current_line = d_list_tb.GetLineFromCharIndex(d_list_tb.GetFirstCharIndexOfCurrentLine())

            d_list_tb.Select(d_list_tb.GetFirstCharIndexOfCurrentLine(), _
                             d_list_tb.Lines(d_current_line).Length) ' reselect current line
            Application.DoEvents()
            d_list_tb.SelectedText = pre_text ' replace it with prev lines text
            d_list_tb.Select(d_list_tb.GetFirstCharIndexOfCurrentLine(), _
                     d_list_tb.Lines(d_current_line).Length) ' reselect current line
            Application.DoEvents()
            update_decal_order()
            d_list_tb.SelectionStart = sp
            d_list_tb.Select(d_list_tb.GetFirstCharIndexOfCurrentLine(), _
                       d_list_tb.Lines(prev_line).Length) ' select prev line
            d_current_line = prev_line
            decal_matrix_list(current_decal).get_decals_transform_info()
        End If
    End Sub

    Private Sub d_move_down_Click(sender As Object, e As EventArgs) Handles d_move_down.Click
        If d_current_line = d_line_count Then
            Return
        End If
        If d_sel_Len > 0 Then
            Dim next_line = d_current_line + 1
            Dim sel_tex_current = d_list_tb.SelectedText ' save current text
            If sel_tex_current = "" Then Return
            Dim sp = d_list_tb.GetFirstCharIndexFromLine(next_line) ' get prev line
            Application.DoEvents()
            d_list_tb.SelectionStart = sp
            d_list_tb.Select(d_list_tb.GetFirstCharIndexOfCurrentLine(), _
                             d_list_tb.Lines(next_line).Length) ' select prev line
            Application.DoEvents()
            Dim next_text = d_list_tb.SelectedText ' save prev line text
            If next_text = "" Then Return
            Application.DoEvents()
            d_list_tb.SelectedText = sel_tex_current ' replace current line text
            Application.DoEvents()

            d_list_tb.SelectionStart = sp - 2
            d_current_line = d_list_tb.GetLineFromCharIndex(d_list_tb.GetFirstCharIndexOfCurrentLine())

            d_list_tb.Select(d_list_tb.GetFirstCharIndexOfCurrentLine(), _
                             d_list_tb.Lines(d_current_line).Length) ' reselect current line
            Application.DoEvents()
            d_list_tb.SelectedText = next_text ' replace it with prev lines text
            d_list_tb.Select(d_list_tb.GetFirstCharIndexOfCurrentLine(), _
                     d_list_tb.Lines(d_current_line).Length) ' reselect current line
            Application.DoEvents()
            update_decal_order()
            d_list_tb.SelectionStart = sp
            d_list_tb.Select(d_list_tb.GetFirstCharIndexOfCurrentLine(), _
                             d_list_tb.Lines(next_line).Length) ' select prev line
            d_current_line = next_line
            decal_matrix_list(current_decal).get_decals_transform_info()
        End If
    End Sub

    Private Sub d_list_tb_KeyPress(sender As Object, e As KeyPressEventArgs) Handles d_list_tb.KeyPress
        e.Handled = True
    End Sub

    Private Sub decal_alpha_slider_Scroll(sender As Object, e As EventArgs) Handles decal_alpha_slider.Scroll
        If current_decal = -1 Then Return
        decal_matrix_list(current_decal).alpha = CSng(decal_alpha_slider.Value / 100)
    End Sub
    Private Sub decal_level_slider_Scroll(sender As Object, e As EventArgs) Handles decal_level_slider.Scroll
        If current_decal = -1 Then Return
        decal_matrix_list(current_decal).level = CSng(decal_level_slider.Value / 100)

    End Sub
    Dim t_list As TextBox

    Private Sub m_sel_texture_Click(sender As Object, e As EventArgs) Handles m_sel_texture.Click
        If current_decal < 0 Then Return
        If t_list Is Nothing Then ' create text box and fill it with all the texture names if it hasn't been created already.
            t_list = New TextBox
            t_list.Multiline = True
            t_list.Parent = decal_panel
            t_list.Width = d_list_tb.Width
            t_list.Height = d_list_tb.Height
            t_list.Location = d_list_tb.Location
            t_list.Anchor = AnchorStyles.Bottom Or AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            t_list.ForeColor = d_list_tb.ForeColor
            t_list.BackColor = d_list_tb.BackColor
            t_list.Font = d_list_tb.Font
            t_list.ScrollBars = ScrollBars.Vertical
            For j = 0 To decal_textures.Length - 1
                t_list.Text += decal_textures(j).colorMap_name + " :" + j.ToString + vbCrLf
            Next
            AddHandler t_list.Click, AddressOf handle_t_click
        End If
        t_list.BringToFront()
        d_list_tb.SendToBack()
        current_decal_lable.Text = current_decal.ToString
    End Sub
    Private Sub handle_t_click(sender As TextBox, e As EventArgs)
        If current_decal < 0 Then Return
        t_list.SelectionLength = 0
        Dim t = t_list.Text.Split(":")
        d_line_count = t.Length - 2
        Dim a = t_list.GetLineFromCharIndex(t_list.GetFirstCharIndexOfCurrentLine())
        t_list.Select(t_list.GetFirstCharIndexOfCurrentLine(), t_list.Lines(a).Length)
        If t_list.SelectedText.Length < 4 Then
            Return
            d_sel_Len = 0
            d_current_line = 0
        Else
            Dim d = t_list.SelectedText.Split(":")
            Dim id = CInt(d(1))
            load_this_Decal(id)
            decal_matrix_list(current_decal).decal_texture = decal_textures(id).colorMap_name
            decal_matrix_list(current_decal).texture_id = decal_textures(id).colorMap_Id
            decal_matrix_list(current_decal).normal_id = decal_textures(id).normalMap_Id
            decal_matrix_list(current_decal).gmm_id = decal_textures(id).gmmMap_id
            d_texture_name.Text = decal_matrix_list(current_decal).decal_texture
            t_list.SendToBack()
            d_list_tb.BringToFront()
            d_move_up.BringToFront()
            d_move_down.BringToFront()
        End If
    End Sub

    Private Sub m_delete_Click(sender As Object, e As EventArgs) Handles m_delete.Click
        If decal_matrix_list.Length < 2 Then Return
        Dim t_l(decal_matrix_list.Length - 2) As decal_matrix_list_
        Dim ta = d_list_tb.Text.Split(vbLf)
        Dim ts As String = ""
        For Each s In ta
            Dim ti = s.Split(":")
            If ti.Length > 1 Then ' dont mess with empty lines
                Dim tii = ti(1).Split(vbCr)
                Dim rv = CInt(tii(0))
                If rv <> current_decal Then
                    If rv > current_decal Then ' If this ones larger it needs decremented.
                        rv -= 1
                    End If
                    ts += ti(0) + ":" + rv.ToString + vbCrLf 're-assemble the line.
                End If
            End If
        Next
        d_list_tb.Text = ts
        Application.DoEvents()
        Dim cnt As Integer = 0
        For i = 0 To decal_matrix_list.Length - 2
            If i <> current_decal Then
                t_l(cnt) = decal_matrix_list(i)
                cnt += 1
            End If
        Next
        ReDim Preserve decal_matrix_list(decal_matrix_list.Length - 2)
        For i = 0 To decal_matrix_list.Length - 2
            decal_matrix_list(i) = t_l(i)
            'd_list_tb.Text += "Decal ID :" + i.ToString + vbCrLf
        Next
        'current_decal -= 1
        'If current_decal = -1 And decal_matrix_list.Length > 1 Then
        '    current_decal = 0
        'End If
        If current_decal > -1 Then
            update_decal_order()
            Dim new_line As Integer = 0
            For i = 0 To decal_order.Length - 1
                If decal_order(i) = current_decal Then
                    new_line = i
                    Exit For
                End If
            Next
            Dim sp = d_list_tb.GetFirstCharIndexFromLine(new_line) ' get line
            d_list_tb.SelectionStart = sp
            Try
                d_list_tb.Select(d_list_tb.GetFirstCharIndexOfCurrentLine(), _
                                     d_list_tb.Lines(new_line).Length) ' select prev line
                decal_matrix_list(new_line).get_decals_transform_info()

            Catch ex As Exception
                Return
            End Try
            d_current_line = new_line
            d_sel_Len = d_list_tb.Lines(new_line).Length
        End If
    End Sub


    Private Sub Uwrap_SelectedItemChanged(sender As Object, e As EventArgs) Handles Uwrap.SelectedItemChanged
        If current_decal = -1 Then Return
        decal_matrix_list(current_decal).u_wrap = CSng(Uwrap.SelectedItem)
        decal_matrix_list(current_decal).u_wrap_index = Uwrap.SelectedIndex
    End Sub

    Private Sub Vwrap_SelectedItemChanged(sender As Object, e As EventArgs) Handles Vwrap.SelectedItemChanged
        If current_decal = -1 Then Return
        decal_matrix_list(current_decal).v_wrap = CSng(Vwrap.SelectedItem)
        decal_matrix_list(current_decal).v_wrap_index = Vwrap.SelectedIndex

    End Sub

 
    Private Sub save_decal_btn_Click(sender As Object, e As EventArgs) Handles save_decal_btn.Click
        If current_decal = -1 Then Return
        If MsgBox("Are you sure?", MsgBoxStyle.YesNo, "For real?") = MsgBoxResult.Yes Then
            save_decal_data()
        End If
    End Sub

    Private Sub load_decal_btn_Click(sender As Object, e As EventArgs) Handles load_decal_btn.Click
        If MsgBox("Are you sure?", MsgBoxStyle.YesNo, "For real?") = MsgBoxResult.Yes Then
            load_decal_data()
        End If
    End Sub

    Private Sub uv_rotate_direction(sender As Object, e As EventArgs) Handles uv_rotate.SelectedItemChanged
        If current_decal = -1 Then Return
        decal_matrix_list(current_decal).uv_rot = CSng(uv_rotate.SelectedItem) * 0.017453293
        decal_matrix_list(current_decal).uv_rot_index = uv_rotate.SelectedIndex

    End Sub

    Private Sub copy_Decal_btn_Click(sender As Object, e As EventArgs) Handles copy_Decal_btn.Click
        If decal_order.Length < 1 Then
            Return
        End If
        copy_decal()
    End Sub

    Private Sub m_reload_textures_Click(sender As Object, e As EventArgs) Handles m_reload_textures.Click
        If Not MODEL_LOADED Then
            Return
        End If
        MODEL_LOADED = False ' stop drawing the tank
        ' delete texture so we dont waste video memory!
        For i = 0 To textures.Length - 1
            Gl.glDeleteTextures(1, textures(i).c_id)
            Gl.glDeleteTextures(1, textures(i).n_id)
            If textures(i).ao_id > -1 Then
                Gl.glDeleteTextures(1, textures(i).ao_id)
            End If
            Gl.glDeleteTextures(1, textures(i).gmm_id)
            If textures(i).detail_id > -1 Then
                Gl.glDeleteTextures(1, textures(i).detail_id)
            End If
        Next
        ReDim textures(0) ' resize so it can be rebuild
        For i = 1 To _group.Length - 1
            build_textures(i) 'get the textures for this model. If its in the cache, use them.
        Next
        MODEL_LOADED = True ' enable drawing the tank
        log_text.AppendLine("---- Reloading Textures -----")
    End Sub

    Private Sub m_extract_Click(sender As Object, e As EventArgs) Handles m_extract.Click
        file_name = current_tank_name
        frmExtract.ShowDialog(Me)

    End Sub

    Private Sub m_export_to_fbx_Click(sender As Object, e As EventArgs) Handles m_export_to_fbx.Click
        SaveFileDialog1.Filter = "AutoDesk (*.FBX)|*.fbx"
        SaveFileDialog1.Title = "Export FBX..."
        SaveFileDialog1.InitialDirectory = My.Settings.fbx_path

        SaveFileDialog1.FileName = short_tank_name.Replace("\/", "_") + ".fbx"

        If SaveFileDialog1.ShowDialog = Forms.DialogResult.OK Then
            My.Settings.fbx_path = SaveFileDialog1.FileName

            'remove_loaded_fbx()
            'clean_house()
        Else
            Return
        End If
        'ReDim textures(0)
        frmFBX.ShowDialog(Me)

    End Sub

    Private Sub m_edit_camo_Click(sender As Object, e As EventArgs) Handles m_edit_camo.Click
        frmEditCamo.Visible = True
    End Sub
End Class
