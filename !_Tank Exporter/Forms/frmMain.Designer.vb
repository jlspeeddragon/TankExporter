﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.Startup_Timer = New System.Windows.Forms.Timer(Me.components)
        Me.MM = New System.Windows.Forms.MenuStrip()
        Me.m_file = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_load_file = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_save = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator7 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_Import_FBX = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_remove_fbx = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_write_primitive = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator9 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_edit_visual = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator8 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_open_temp_folder = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_region = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator11 = New System.Windows.Forms.ToolStripSeparator()
        Me.M_Path = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_res_mods_path = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_clear_temp_folder_data = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_reload_api_data = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_shadows = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_select_light = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_shadowQuality = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_simple_lighting = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_show_log = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator12 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_edit_shaders = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_Shader_Debug = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_show_environment = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_shadow_preview = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator10 = New System.Windows.Forms.ToolStripSeparator()
        Me.M_Exit = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_show_model_info = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_show_fbx = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_show_bsp2 = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_show_bsp2_tree = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_export_tank_list = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_clear_selected_tanks = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_pick_camo = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_load_textures = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_lighting = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_help = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripComboBox1 = New System.Windows.Forms.ToolStripComboBox()
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.SplitContainer3 = New System.Windows.Forms.SplitContainer()
        Me.show_textures_cb = New System.Windows.Forms.CheckBox()
        Me.wire_cb = New System.Windows.Forms.CheckBox()
        Me.grid_cb = New System.Windows.Forms.CheckBox()
        Me.gun_cb = New System.Windows.Forms.CheckBox()
        Me.turret_cb = New System.Windows.Forms.CheckBox()
        Me.hull_cb = New System.Windows.Forms.CheckBox()
        Me.chassis_cb = New System.Windows.Forms.CheckBox()
        Me.PB3 = New System.Windows.Forms.PictureBox()
        Me.pb1 = New System.Windows.Forms.PictureBox()
        Me.font_holder = New System.Windows.Forms.Label()
        Me.PG1 = New System.Windows.Forms.ProgressBar()
        Me.pb2 = New System.Windows.Forms.Panel()
        Me.info_Label = New System.Windows.Forms.Label()
        Me.SplitContainer2 = New System.Windows.Forms.SplitContainer()
        Me.TC1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.TabPage3 = New System.Windows.Forms.TabPage()
        Me.TabPage4 = New System.Windows.Forms.TabPage()
        Me.TabPage5 = New System.Windows.Forms.TabPage()
        Me.TabPage6 = New System.Windows.Forms.TabPage()
        Me.TabPage7 = New System.Windows.Forms.TabPage()
        Me.TabPage8 = New System.Windows.Forms.TabPage()
        Me.TabPage9 = New System.Windows.Forms.TabPage()
        Me.TabPage10 = New System.Windows.Forms.TabPage()
        Me.tank_label = New System.Windows.Forms.Label()
        Me.iconbox = New System.Windows.Forms.PictureBox()
        Me.conMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.m_load = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_export_fbx = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator6 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_create_and_extract = New System.Windows.Forms.ToolStripMenuItem()
        Me.SaveFileDialog1 = New System.Windows.Forms.SaveFileDialog()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.m_decal = New System.Windows.Forms.ToolStripMenuItem()
        Me.MM.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.SplitContainer3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer3.Panel1.SuspendLayout()
        Me.SplitContainer3.Panel2.SuspendLayout()
        Me.SplitContainer3.SuspendLayout()
        CType(Me.PB3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pb1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer2.Panel1.SuspendLayout()
        Me.SplitContainer2.Panel2.SuspendLayout()
        Me.SplitContainer2.SuspendLayout()
        Me.TC1.SuspendLayout()
        CType(Me.iconbox, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.conMenu.SuspendLayout()
        Me.SuspendLayout()
        '
        'Startup_Timer
        '
        Me.Startup_Timer.Interval = 500
        '
        'MM
        '
        Me.MM.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_file, Me.m_show_model_info, Me.m_show_fbx, Me.m_show_bsp2, Me.m_show_bsp2_tree, Me.m_export_tank_list, Me.m_clear_selected_tanks, Me.m_pick_camo, Me.m_load_textures, Me.m_lighting, Me.m_decal, Me.m_help, Me.ToolStripComboBox1})
        Me.MM.Location = New System.Drawing.Point(0, 0)
        Me.MM.Name = "MM"
        Me.MM.Size = New System.Drawing.Size(968, 27)
        Me.MM.TabIndex = 1
        '
        'm_file
        '
        Me.m_file.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_load_file, Me.m_save, Me.ToolStripSeparator7, Me.m_Import_FBX, Me.m_remove_fbx, Me.ToolStripSeparator1, Me.m_write_primitive, Me.ToolStripSeparator9, Me.m_edit_visual, Me.ToolStripSeparator8, Me.m_open_temp_folder, Me.ToolStripSeparator4, Me.m_region, Me.ToolStripSeparator11, Me.M_Path, Me.m_res_mods_path, Me.ToolStripSeparator2, Me.m_clear_temp_folder_data, Me.m_reload_api_data, Me.ToolStripSeparator3, Me.m_shadows, Me.m_select_light, Me.m_shadowQuality, Me.m_simple_lighting, Me.ToolStripSeparator5, Me.m_show_log, Me.ToolStripSeparator12, Me.m_edit_shaders, Me.m_Shader_Debug, Me.m_show_environment, Me.m_shadow_preview, Me.ToolStripSeparator10, Me.M_Exit})
        Me.m_file.Name = "m_file"
        Me.m_file.Size = New System.Drawing.Size(37, 23)
        Me.m_file.Text = "&File"
        '
        'm_load_file
        '
        Me.m_load_file.Name = "m_load_file"
        Me.m_load_file.Size = New System.Drawing.Size(222, 22)
        Me.m_load_file.Text = "Load"
        '
        'm_save
        '
        Me.m_save.Name = "m_save"
        Me.m_save.Size = New System.Drawing.Size(222, 22)
        Me.m_save.Text = "Save"
        '
        'ToolStripSeparator7
        '
        Me.ToolStripSeparator7.Name = "ToolStripSeparator7"
        Me.ToolStripSeparator7.Size = New System.Drawing.Size(219, 6)
        '
        'm_Import_FBX
        '
        Me.m_Import_FBX.Name = "m_Import_FBX"
        Me.m_Import_FBX.Size = New System.Drawing.Size(222, 22)
        Me.m_Import_FBX.Text = "Import FBX"
        '
        'm_remove_fbx
        '
        Me.m_remove_fbx.Name = "m_remove_fbx"
        Me.m_remove_fbx.Size = New System.Drawing.Size(222, 22)
        Me.m_remove_fbx.Text = "Remove Models"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(219, 6)
        '
        'm_write_primitive
        '
        Me.m_write_primitive.Enabled = False
        Me.m_write_primitive.Name = "m_write_primitive"
        Me.m_write_primitive.Size = New System.Drawing.Size(222, 22)
        Me.m_write_primitive.Text = "Write Primitive"
        '
        'ToolStripSeparator9
        '
        Me.ToolStripSeparator9.Name = "ToolStripSeparator9"
        Me.ToolStripSeparator9.Size = New System.Drawing.Size(219, 6)
        '
        'm_edit_visual
        '
        Me.m_edit_visual.Name = "m_edit_visual"
        Me.m_edit_visual.Size = New System.Drawing.Size(222, 22)
        Me.m_edit_visual.Text = "Show Visual Files"
        '
        'ToolStripSeparator8
        '
        Me.ToolStripSeparator8.Name = "ToolStripSeparator8"
        Me.ToolStripSeparator8.Size = New System.Drawing.Size(219, 6)
        '
        'm_open_temp_folder
        '
        Me.m_open_temp_folder.Name = "m_open_temp_folder"
        Me.m_open_temp_folder.Size = New System.Drawing.Size(222, 22)
        Me.m_open_temp_folder.Text = "Open Tank Folder"
        '
        'ToolStripSeparator4
        '
        Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
        Me.ToolStripSeparator4.Size = New System.Drawing.Size(219, 6)
        '
        'm_region
        '
        Me.m_region.Name = "m_region"
        Me.m_region.Size = New System.Drawing.Size(222, 22)
        Me.m_region.Text = "Set Region"
        Me.m_region.Visible = False
        '
        'ToolStripSeparator11
        '
        Me.ToolStripSeparator11.Name = "ToolStripSeparator11"
        Me.ToolStripSeparator11.Size = New System.Drawing.Size(219, 6)
        Me.ToolStripSeparator11.Visible = False
        '
        'M_Path
        '
        Me.M_Path.Name = "M_Path"
        Me.M_Path.Size = New System.Drawing.Size(222, 22)
        Me.M_Path.Text = "Path to Game folder"
        '
        'm_res_mods_path
        '
        Me.m_res_mods_path.Name = "m_res_mods_path"
        Me.m_res_mods_path.Size = New System.Drawing.Size(222, 22)
        Me.m_res_mods_path.Text = "Path to res_mods "
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(219, 6)
        '
        'm_clear_temp_folder_data
        '
        Me.m_clear_temp_folder_data.Name = "m_clear_temp_folder_data"
        Me.m_clear_temp_folder_data.Size = New System.Drawing.Size(222, 22)
        Me.m_clear_temp_folder_data.Text = "Clear Temp Folder"
        '
        'm_reload_api_data
        '
        Me.m_reload_api_data.Name = "m_reload_api_data"
        Me.m_reload_api_data.Size = New System.Drawing.Size(222, 22)
        Me.m_reload_api_data.Text = "Reload WoT API data"
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(219, 6)
        '
        'm_shadows
        '
        Me.m_shadows.CheckOnClick = True
        Me.m_shadows.Name = "m_shadows"
        Me.m_shadows.Size = New System.Drawing.Size(222, 22)
        Me.m_shadows.Text = "Shadows"
        '
        'm_select_light
        '
        Me.m_select_light.Name = "m_select_light"
        Me.m_select_light.Size = New System.Drawing.Size(222, 22)
        Me.m_select_light.Text = "Set Shadow Light"
        '
        'm_shadowQuality
        '
        Me.m_shadowQuality.Name = "m_shadowQuality"
        Me.m_shadowQuality.Size = New System.Drawing.Size(222, 22)
        Me.m_shadowQuality.Text = "Set Shadow Quality"
        '
        'm_simple_lighting
        '
        Me.m_simple_lighting.Checked = Global.Tank_Exporter.My.MySettings.Default.m_simple_lighting
        Me.m_simple_lighting.CheckOnClick = True
        Me.m_simple_lighting.Name = "m_simple_lighting"
        Me.m_simple_lighting.Size = New System.Drawing.Size(222, 22)
        Me.m_simple_lighting.Text = "Simple Lighting"
        '
        'ToolStripSeparator5
        '
        Me.ToolStripSeparator5.Name = "ToolStripSeparator5"
        Me.ToolStripSeparator5.Size = New System.Drawing.Size(219, 6)
        '
        'm_show_log
        '
        Me.m_show_log.Name = "m_show_log"
        Me.m_show_log.Size = New System.Drawing.Size(222, 22)
        Me.m_show_log.Text = "Show Log File"
        '
        'ToolStripSeparator12
        '
        Me.ToolStripSeparator12.Name = "ToolStripSeparator12"
        Me.ToolStripSeparator12.Size = New System.Drawing.Size(219, 6)
        '
        'm_edit_shaders
        '
        Me.m_edit_shaders.Name = "m_edit_shaders"
        Me.m_edit_shaders.Size = New System.Drawing.Size(222, 22)
        Me.m_edit_shaders.Text = "Edit Shaders"
        '
        'm_Shader_Debug
        '
        Me.m_Shader_Debug.Name = "m_Shader_Debug"
        Me.m_Shader_Debug.Size = New System.Drawing.Size(222, 22)
        Me.m_Shader_Debug.Text = "Tank Shader Debug Settings"
        '
        'm_show_environment
        '
        Me.m_show_environment.CheckOnClick = True
        Me.m_show_environment.Name = "m_show_environment"
        Me.m_show_environment.Size = New System.Drawing.Size(222, 22)
        Me.m_show_environment.Text = "Show Environment"
        '
        'm_shadow_preview
        '
        Me.m_shadow_preview.CheckOnClick = True
        Me.m_shadow_preview.Name = "m_shadow_preview"
        Me.m_shadow_preview.Size = New System.Drawing.Size(222, 22)
        Me.m_shadow_preview.Text = "Shadow Preview"
        '
        'ToolStripSeparator10
        '
        Me.ToolStripSeparator10.Name = "ToolStripSeparator10"
        Me.ToolStripSeparator10.Size = New System.Drawing.Size(219, 6)
        '
        'M_Exit
        '
        Me.M_Exit.Name = "M_Exit"
        Me.M_Exit.Size = New System.Drawing.Size(222, 22)
        Me.M_Exit.Text = "Exit"
        '
        'm_show_model_info
        '
        Me.m_show_model_info.Name = "m_show_model_info"
        Me.m_show_model_info.Size = New System.Drawing.Size(77, 23)
        Me.m_show_model_info.Text = "Model Info"
        '
        'm_show_fbx
        '
        Me.m_show_fbx.CheckOnClick = True
        Me.m_show_fbx.Name = "m_show_fbx"
        Me.m_show_fbx.Size = New System.Drawing.Size(71, 23)
        Me.m_show_fbx.Text = "Show FBX"
        Me.m_show_fbx.Visible = False
        '
        'm_show_bsp2
        '
        Me.m_show_bsp2.CheckOnClick = True
        Me.m_show_bsp2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.m_show_bsp2.Name = "m_show_bsp2"
        Me.m_show_bsp2.Size = New System.Drawing.Size(45, 23)
        Me.m_show_bsp2.Text = "BSP2"
        Me.m_show_bsp2.Visible = False
        '
        'm_show_bsp2_tree
        '
        Me.m_show_bsp2_tree.CheckOnClick = True
        Me.m_show_bsp2_tree.Name = "m_show_bsp2_tree"
        Me.m_show_bsp2_tree.Size = New System.Drawing.Size(71, 23)
        Me.m_show_bsp2_tree.Text = "BSP2 Tree"
        Me.m_show_bsp2_tree.Visible = False
        '
        'm_export_tank_list
        '
        Me.m_export_tank_list.Name = "m_export_tank_list"
        Me.m_export_tank_list.Size = New System.Drawing.Size(102, 23)
        Me.m_export_tank_list.Text = "Export Tank List"
        Me.m_export_tank_list.Visible = False
        '
        'm_clear_selected_tanks
        '
        Me.m_clear_selected_tanks.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.m_clear_selected_tanks.Name = "m_clear_selected_tanks"
        Me.m_clear_selected_tanks.Size = New System.Drawing.Size(127, 23)
        Me.m_clear_selected_tanks.Text = "Clear Selected Tanks"
        '
        'm_pick_camo
        '
        Me.m_pick_camo.Enabled = False
        Me.m_pick_camo.Name = "m_pick_camo"
        Me.m_pick_camo.Size = New System.Drawing.Size(84, 23)
        Me.m_pick_camo.Text = "Camouflage"
        '
        'm_load_textures
        '
        Me.m_load_textures.Checked = True
        Me.m_load_textures.CheckOnClick = True
        Me.m_load_textures.CheckState = System.Windows.Forms.CheckState.Checked
        Me.m_load_textures.ForeColor = System.Drawing.Color.Red
        Me.m_load_textures.Name = "m_load_textures"
        Me.m_load_textures.Size = New System.Drawing.Size(95, 23)
        Me.m_load_textures.Text = "Show Textures"
        '
        'm_lighting
        '
        Me.m_lighting.Name = "m_lighting"
        Me.m_lighting.Size = New System.Drawing.Size(63, 23)
        Me.m_lighting.Text = "Lighting"
        '
        'm_help
        '
        Me.m_help.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.m_help.Image = Global.Tank_Exporter.My.Resources.Resources.question
        Me.m_help.Name = "m_help"
        Me.m_help.Size = New System.Drawing.Size(28, 23)
        Me.m_help.Text = "Help"
        '
        'ToolStripComboBox1
        '
        Me.ToolStripComboBox1.Items.AddRange(New Object() {"NA", "EU", "RU", "ASIA"})
        Me.ToolStripComboBox1.Name = "ToolStripComboBox1"
        Me.ToolStripComboBox1.Size = New System.Drawing.Size(121, 23)
        Me.ToolStripComboBox1.Text = Global.Tank_Exporter.My.MySettings.Default.region_selection
        Me.ToolStripComboBox1.Visible = False
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
        Me.SplitContainer1.IsSplitterFixed = True
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 27)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.SplitContainer1.Panel1.Controls.Add(Me.SplitContainer3)
        Me.SplitContainer1.Panel1.Controls.Add(Me.info_Label)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer))
        Me.SplitContainer1.Panel2.Controls.Add(Me.SplitContainer2)
        Me.SplitContainer1.Size = New System.Drawing.Size(968, 506)
        Me.SplitContainer1.SplitterDistance = 915
        Me.SplitContainer1.SplitterWidth = 1
        Me.SplitContainer1.TabIndex = 2
        '
        'SplitContainer3
        '
        Me.SplitContainer3.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.SplitContainer3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.SplitContainer3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SplitContainer3.IsSplitterFixed = True
        Me.SplitContainer3.Location = New System.Drawing.Point(0, 23)
        Me.SplitContainer3.Name = "SplitContainer3"
        '
        'SplitContainer3.Panel1
        '
        Me.SplitContainer3.Panel1.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer))
        Me.SplitContainer3.Panel1.Controls.Add(Me.show_textures_cb)
        Me.SplitContainer3.Panel1.Controls.Add(Me.wire_cb)
        Me.SplitContainer3.Panel1.Controls.Add(Me.grid_cb)
        Me.SplitContainer3.Panel1.Controls.Add(Me.gun_cb)
        Me.SplitContainer3.Panel1.Controls.Add(Me.turret_cb)
        Me.SplitContainer3.Panel1.Controls.Add(Me.hull_cb)
        Me.SplitContainer3.Panel1.Controls.Add(Me.chassis_cb)
        '
        'SplitContainer3.Panel2
        '
        Me.SplitContainer3.Panel2.BackgroundImage = Global.Tank_Exporter.My.Resources.Resources.gradiant
        Me.SplitContainer3.Panel2.Controls.Add(Me.PB3)
        Me.SplitContainer3.Panel2.Controls.Add(Me.pb1)
        Me.SplitContainer3.Panel2.Controls.Add(Me.font_holder)
        Me.SplitContainer3.Panel2.Controls.Add(Me.PG1)
        Me.SplitContainer3.Panel2.Controls.Add(Me.pb2)
        Me.SplitContainer3.Size = New System.Drawing.Size(915, 483)
        Me.SplitContainer3.SplitterDistance = 56
        Me.SplitContainer3.SplitterWidth = 1
        Me.SplitContainer3.TabIndex = 3
        '
        'show_textures_cb
        '
        Me.show_textures_cb.Appearance = System.Windows.Forms.Appearance.Button
        Me.show_textures_cb.BackColor = System.Drawing.Color.Gray
        Me.show_textures_cb.BackgroundImage = Global.Tank_Exporter.My.Resources.Resources.images_off
        Me.show_textures_cb.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.show_textures_cb.FlatAppearance.BorderSize = 2
        Me.show_textures_cb.FlatAppearance.CheckedBackColor = System.Drawing.Color.Sienna
        Me.show_textures_cb.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.show_textures_cb.ForeColor = System.Drawing.Color.White
        Me.show_textures_cb.Location = New System.Drawing.Point(2, 310)
        Me.show_textures_cb.Name = "show_textures_cb"
        Me.show_textures_cb.Size = New System.Drawing.Size(48, 48)
        Me.show_textures_cb.TabIndex = 8
        Me.show_textures_cb.UseVisualStyleBackColor = False
        '
        'wire_cb
        '
        Me.wire_cb.Appearance = System.Windows.Forms.Appearance.Button
        Me.wire_cb.BackColor = System.Drawing.Color.Gray
        Me.wire_cb.BackgroundImage = Global.Tank_Exporter.My.Resources.Resources.box_solid
        Me.wire_cb.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.wire_cb.Checked = True
        Me.wire_cb.CheckState = System.Windows.Forms.CheckState.Checked
        Me.wire_cb.FlatAppearance.BorderSize = 2
        Me.wire_cb.FlatAppearance.CheckedBackColor = System.Drawing.Color.Sienna
        Me.wire_cb.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.wire_cb.ForeColor = System.Drawing.Color.White
        Me.wire_cb.Location = New System.Drawing.Point(2, 258)
        Me.wire_cb.Name = "wire_cb"
        Me.wire_cb.Size = New System.Drawing.Size(48, 48)
        Me.wire_cb.TabIndex = 7
        Me.wire_cb.UseVisualStyleBackColor = False
        '
        'grid_cb
        '
        Me.grid_cb.Appearance = System.Windows.Forms.Appearance.Button
        Me.grid_cb.BackColor = System.Drawing.Color.Gray
        Me.grid_cb.BackgroundImage = Global.Tank_Exporter.My.Resources.Resources.grid
        Me.grid_cb.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.grid_cb.Checked = True
        Me.grid_cb.CheckState = System.Windows.Forms.CheckState.Checked
        Me.grid_cb.FlatAppearance.BorderSize = 2
        Me.grid_cb.FlatAppearance.CheckedBackColor = System.Drawing.Color.Sienna
        Me.grid_cb.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.grid_cb.ForeColor = System.Drawing.Color.White
        Me.grid_cb.Location = New System.Drawing.Point(2, 207)
        Me.grid_cb.Name = "grid_cb"
        Me.grid_cb.Size = New System.Drawing.Size(48, 48)
        Me.grid_cb.TabIndex = 6
        Me.grid_cb.UseVisualStyleBackColor = False
        '
        'gun_cb
        '
        Me.gun_cb.Appearance = System.Windows.Forms.Appearance.Button
        Me.gun_cb.BackColor = System.Drawing.Color.Gray
        Me.gun_cb.BackgroundImage = Global.Tank_Exporter.My.Resources.Resources.gun
        Me.gun_cb.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.gun_cb.Checked = True
        Me.gun_cb.CheckState = System.Windows.Forms.CheckState.Checked
        Me.gun_cb.FlatAppearance.BorderSize = 2
        Me.gun_cb.FlatAppearance.CheckedBackColor = System.Drawing.Color.Sienna
        Me.gun_cb.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.gun_cb.ForeColor = System.Drawing.Color.White
        Me.gun_cb.Location = New System.Drawing.Point(2, 156)
        Me.gun_cb.Name = "gun_cb"
        Me.gun_cb.Size = New System.Drawing.Size(48, 48)
        Me.gun_cb.TabIndex = 5
        Me.gun_cb.UseVisualStyleBackColor = False
        '
        'turret_cb
        '
        Me.turret_cb.Appearance = System.Windows.Forms.Appearance.Button
        Me.turret_cb.BackColor = System.Drawing.Color.Gray
        Me.turret_cb.BackgroundImage = Global.Tank_Exporter.My.Resources.Resources.tower
        Me.turret_cb.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.turret_cb.Checked = True
        Me.turret_cb.CheckState = System.Windows.Forms.CheckState.Checked
        Me.turret_cb.FlatAppearance.BorderSize = 2
        Me.turret_cb.FlatAppearance.CheckedBackColor = System.Drawing.Color.Sienna
        Me.turret_cb.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.turret_cb.ForeColor = System.Drawing.Color.White
        Me.turret_cb.Location = New System.Drawing.Point(2, 105)
        Me.turret_cb.Name = "turret_cb"
        Me.turret_cb.Size = New System.Drawing.Size(48, 48)
        Me.turret_cb.TabIndex = 4
        Me.turret_cb.UseVisualStyleBackColor = False
        '
        'hull_cb
        '
        Me.hull_cb.Appearance = System.Windows.Forms.Appearance.Button
        Me.hull_cb.BackColor = System.Drawing.Color.Gray
        Me.hull_cb.BackgroundImage = Global.Tank_Exporter.My.Resources.Resources.hull
        Me.hull_cb.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.hull_cb.Checked = True
        Me.hull_cb.CheckState = System.Windows.Forms.CheckState.Checked
        Me.hull_cb.FlatAppearance.BorderSize = 2
        Me.hull_cb.FlatAppearance.CheckedBackColor = System.Drawing.Color.Sienna
        Me.hull_cb.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.hull_cb.ForeColor = System.Drawing.Color.White
        Me.hull_cb.Location = New System.Drawing.Point(2, 54)
        Me.hull_cb.Name = "hull_cb"
        Me.hull_cb.Size = New System.Drawing.Size(48, 48)
        Me.hull_cb.TabIndex = 3
        Me.hull_cb.UseVisualStyleBackColor = False
        '
        'chassis_cb
        '
        Me.chassis_cb.Appearance = System.Windows.Forms.Appearance.Button
        Me.chassis_cb.BackColor = System.Drawing.Color.Gray
        Me.chassis_cb.BackgroundImage = Global.Tank_Exporter.My.Resources.Resources.chassis
        Me.chassis_cb.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.chassis_cb.Checked = True
        Me.chassis_cb.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chassis_cb.FlatAppearance.BorderSize = 2
        Me.chassis_cb.FlatAppearance.CheckedBackColor = System.Drawing.Color.Sienna
        Me.chassis_cb.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.chassis_cb.ForeColor = System.Drawing.Color.White
        Me.chassis_cb.Location = New System.Drawing.Point(2, 3)
        Me.chassis_cb.Name = "chassis_cb"
        Me.chassis_cb.Size = New System.Drawing.Size(48, 48)
        Me.chassis_cb.TabIndex = 2
        Me.chassis_cb.UseVisualStyleBackColor = False
        '
        'PB3
        '
        Me.PB3.Location = New System.Drawing.Point(251, 267)
        Me.PB3.Name = "PB3"
        Me.PB3.Size = New System.Drawing.Size(100, 50)
        Me.PB3.TabIndex = 4
        Me.PB3.TabStop = False
        Me.PB3.Visible = False
        '
        'pb1
        '
        Me.pb1.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.pb1.BackgroundImage = Global.Tank_Exporter.My.Resources.Resources.gradiant
        Me.pb1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.pb1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pb1.Location = New System.Drawing.Point(0, 23)
        Me.pb1.Name = "pb1"
        Me.pb1.Size = New System.Drawing.Size(854, 456)
        Me.pb1.TabIndex = 0
        Me.pb1.TabStop = False
        '
        'font_holder
        '
        Me.font_holder.AutoSize = True
        Me.font_holder.Font = New System.Drawing.Font("Lucida Console", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.font_holder.ForeColor = System.Drawing.Color.White
        Me.font_holder.Location = New System.Drawing.Point(108, 405)
        Me.font_holder.Name = "font_holder"
        Me.font_holder.Size = New System.Drawing.Size(111, 13)
        Me.font_holder.TabIndex = 1
        Me.font_holder.Text = "For font only"
        Me.font_holder.Visible = False
        '
        'PG1
        '
        Me.PG1.Dock = System.Windows.Forms.DockStyle.Top
        Me.PG1.Location = New System.Drawing.Point(0, 0)
        Me.PG1.Name = "PG1"
        Me.PG1.Size = New System.Drawing.Size(854, 23)
        Me.PG1.TabIndex = 2
        Me.PG1.Visible = False
        '
        'pb2
        '
        Me.pb2.Location = New System.Drawing.Point(18, 125)
        Me.pb2.Name = "pb2"
        Me.pb2.Size = New System.Drawing.Size(200, 100)
        Me.pb2.TabIndex = 3
        Me.pb2.Visible = False
        '
        'info_Label
        '
        Me.info_Label.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer))
        Me.info_Label.Dock = System.Windows.Forms.DockStyle.Top
        Me.info_Label.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.info_Label.ForeColor = System.Drawing.Color.Silver
        Me.info_Label.Location = New System.Drawing.Point(0, 0)
        Me.info_Label.Name = "info_Label"
        Me.info_Label.Size = New System.Drawing.Size(915, 23)
        Me.info_Label.TabIndex = 1
        Me.info_Label.Text = "Label1"
        '
        'SplitContainer2
        '
        Me.SplitContainer2.BackColor = System.Drawing.SystemColors.Menu
        Me.SplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
        Me.SplitContainer2.IsSplitterFixed = True
        Me.SplitContainer2.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer2.Name = "SplitContainer2"
        Me.SplitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer2.Panel1
        '
        Me.SplitContainer2.Panel1.Controls.Add(Me.TC1)
        '
        'SplitContainer2.Panel2
        '
        Me.SplitContainer2.Panel2.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer))
        Me.SplitContainer2.Panel2.Controls.Add(Me.tank_label)
        Me.SplitContainer2.Panel2.Controls.Add(Me.iconbox)
        Me.SplitContainer2.Size = New System.Drawing.Size(52, 506)
        Me.SplitContainer2.SplitterDistance = 480
        Me.SplitContainer2.SplitterWidth = 1
        Me.SplitContainer2.TabIndex = 1
        '
        'TC1
        '
        Me.TC1.Controls.Add(Me.TabPage1)
        Me.TC1.Controls.Add(Me.TabPage2)
        Me.TC1.Controls.Add(Me.TabPage3)
        Me.TC1.Controls.Add(Me.TabPage4)
        Me.TC1.Controls.Add(Me.TabPage5)
        Me.TC1.Controls.Add(Me.TabPage6)
        Me.TC1.Controls.Add(Me.TabPage7)
        Me.TC1.Controls.Add(Me.TabPage8)
        Me.TC1.Controls.Add(Me.TabPage9)
        Me.TC1.Controls.Add(Me.TabPage10)
        Me.TC1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TC1.ItemSize = New System.Drawing.Size(24, 21)
        Me.TC1.Location = New System.Drawing.Point(0, 0)
        Me.TC1.Name = "TC1"
        Me.TC1.SelectedIndex = 0
        Me.TC1.Size = New System.Drawing.Size(52, 480)
        Me.TC1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed
        Me.TC1.TabIndex = 0
        '
        'TabPage1
        '
        Me.TabPage1.BackColor = System.Drawing.Color.DimGray
        Me.TabPage1.Location = New System.Drawing.Point(4, 25)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(44, 451)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "1"
        '
        'TabPage2
        '
        Me.TabPage2.BackColor = System.Drawing.Color.DimGray
        Me.TabPage2.Location = New System.Drawing.Point(4, 25)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(44, 451)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "2"
        '
        'TabPage3
        '
        Me.TabPage3.BackColor = System.Drawing.Color.DimGray
        Me.TabPage3.Location = New System.Drawing.Point(4, 25)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage3.Size = New System.Drawing.Size(44, 451)
        Me.TabPage3.TabIndex = 2
        Me.TabPage3.Text = "3"
        '
        'TabPage4
        '
        Me.TabPage4.BackColor = System.Drawing.Color.DimGray
        Me.TabPage4.Location = New System.Drawing.Point(4, 25)
        Me.TabPage4.Name = "TabPage4"
        Me.TabPage4.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage4.Size = New System.Drawing.Size(44, 451)
        Me.TabPage4.TabIndex = 3
        Me.TabPage4.Text = "4"
        '
        'TabPage5
        '
        Me.TabPage5.BackColor = System.Drawing.Color.DimGray
        Me.TabPage5.Location = New System.Drawing.Point(4, 25)
        Me.TabPage5.Name = "TabPage5"
        Me.TabPage5.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage5.Size = New System.Drawing.Size(44, 451)
        Me.TabPage5.TabIndex = 4
        Me.TabPage5.Text = "5"
        '
        'TabPage6
        '
        Me.TabPage6.BackColor = System.Drawing.Color.DimGray
        Me.TabPage6.Location = New System.Drawing.Point(4, 25)
        Me.TabPage6.Name = "TabPage6"
        Me.TabPage6.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage6.Size = New System.Drawing.Size(44, 451)
        Me.TabPage6.TabIndex = 5
        Me.TabPage6.Text = "6"
        '
        'TabPage7
        '
        Me.TabPage7.BackColor = System.Drawing.Color.DimGray
        Me.TabPage7.Location = New System.Drawing.Point(4, 25)
        Me.TabPage7.Name = "TabPage7"
        Me.TabPage7.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage7.Size = New System.Drawing.Size(44, 451)
        Me.TabPage7.TabIndex = 6
        Me.TabPage7.Text = "7"
        '
        'TabPage8
        '
        Me.TabPage8.BackColor = System.Drawing.Color.DimGray
        Me.TabPage8.Location = New System.Drawing.Point(4, 25)
        Me.TabPage8.Name = "TabPage8"
        Me.TabPage8.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage8.Size = New System.Drawing.Size(44, 451)
        Me.TabPage8.TabIndex = 7
        Me.TabPage8.Text = "8"
        '
        'TabPage9
        '
        Me.TabPage9.BackColor = System.Drawing.Color.DimGray
        Me.TabPage9.Location = New System.Drawing.Point(4, 25)
        Me.TabPage9.Name = "TabPage9"
        Me.TabPage9.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage9.Size = New System.Drawing.Size(44, 451)
        Me.TabPage9.TabIndex = 8
        Me.TabPage9.Text = "9"
        '
        'TabPage10
        '
        Me.TabPage10.BackColor = System.Drawing.Color.DimGray
        Me.TabPage10.Location = New System.Drawing.Point(4, 25)
        Me.TabPage10.Name = "TabPage10"
        Me.TabPage10.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage10.Size = New System.Drawing.Size(44, 451)
        Me.TabPage10.TabIndex = 9
        Me.TabPage10.Text = "10"
        '
        'tank_label
        '
        Me.tank_label.AutoSize = True
        Me.tank_label.Dock = System.Windows.Forms.DockStyle.Left
        Me.tank_label.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tank_label.ForeColor = System.Drawing.Color.White
        Me.tank_label.Location = New System.Drawing.Point(0, 0)
        Me.tank_label.Name = "tank_label"
        Me.tank_label.Size = New System.Drawing.Size(49, 16)
        Me.tank_label.TabIndex = 3
        Me.tank_label.Text = "Label1"
        '
        'iconbox
        '
        Me.iconbox.BackColor = System.Drawing.Color.Transparent
        Me.iconbox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.iconbox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.iconbox.Location = New System.Drawing.Point(0, 0)
        Me.iconbox.Name = "iconbox"
        Me.iconbox.Size = New System.Drawing.Size(52, 25)
        Me.iconbox.TabIndex = 2
        Me.iconbox.TabStop = False
        '
        'conMenu
        '
        Me.conMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_load, Me.m_export_fbx, Me.ToolStripSeparator6, Me.m_create_and_extract})
        Me.conMenu.Name = "conMenu"
        Me.conMenu.Size = New System.Drawing.Size(211, 76)
        '
        'm_load
        '
        Me.m_load.Name = "m_load"
        Me.m_load.Size = New System.Drawing.Size(210, 22)
        Me.m_load.Text = "Load This.."
        '
        'm_export_fbx
        '
        Me.m_export_fbx.Name = "m_export_fbx"
        Me.m_export_fbx.Size = New System.Drawing.Size(210, 22)
        Me.m_export_fbx.Text = "Export FBX"
        '
        'ToolStripSeparator6
        '
        Me.ToolStripSeparator6.Name = "ToolStripSeparator6"
        Me.ToolStripSeparator6.Size = New System.Drawing.Size(207, 6)
        '
        'm_create_and_extract
        '
        Me.m_create_and_extract.Name = "m_create_and_extract"
        Me.m_create_and_extract.Size = New System.Drawing.Size(210, 22)
        Me.m_create_and_extract.Text = "Extract to res_mods folder"
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'm_decal
        '
        Me.m_decal.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.m_decal.CheckOnClick = True
        Me.m_decal.Name = "m_decal"
        Me.m_decal.Size = New System.Drawing.Size(75, 23)
        Me.m_decal.Text = "Decal Tool"
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Black
        Me.ClientSize = New System.Drawing.Size(968, 533)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.MM)
        Me.DoubleBuffered = True
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MM
        Me.Name = "frmMain"
        Me.Text = "Tank Exporter"
        Me.MM.ResumeLayout(False)
        Me.MM.PerformLayout()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.SplitContainer3.Panel1.ResumeLayout(False)
        Me.SplitContainer3.Panel2.ResumeLayout(False)
        Me.SplitContainer3.Panel2.PerformLayout()
        CType(Me.SplitContainer3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer3.ResumeLayout(False)
        CType(Me.PB3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pb1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer2.Panel1.ResumeLayout(False)
        Me.SplitContainer2.Panel2.ResumeLayout(False)
        Me.SplitContainer2.Panel2.PerformLayout()
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer2.ResumeLayout(False)
        Me.TC1.ResumeLayout(False)
        CType(Me.iconbox, System.ComponentModel.ISupportInitialize).EndInit()
        Me.conMenu.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents pb1 As System.Windows.Forms.PictureBox
    Friend WithEvents Startup_Timer As System.Windows.Forms.Timer
    Friend WithEvents MM As System.Windows.Forms.MenuStrip
    Friend WithEvents m_file As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents M_Exit As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents M_Path As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents FolderBrowserDialog1 As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents SplitContainer2 As System.Windows.Forms.SplitContainer
    Friend WithEvents TC1 As System.Windows.Forms.TabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage3 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage4 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage5 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage6 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage7 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage8 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage9 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage10 As System.Windows.Forms.TabPage
    Friend WithEvents iconbox As System.Windows.Forms.PictureBox
    Friend WithEvents info_Label As System.Windows.Forms.Label
    Friend WithEvents conMenu As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents m_load As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents font_holder As System.Windows.Forms.Label
    Friend WithEvents m_clear_temp_folder_data As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_reload_api_data As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_export_tank_list As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_clear_selected_tanks As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_load_file As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_save As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents tank_label As System.Windows.Forms.Label
    Friend WithEvents m_open_temp_folder As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator4 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents SaveFileDialog1 As System.Windows.Forms.SaveFileDialog
    Friend WithEvents m_export_fbx As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_load_textures As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator5 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents m_show_log As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_res_mods_path As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_pick_camo As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_edit_shaders As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_lighting As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator6 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents m_create_and_extract As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_help As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents PG1 As System.Windows.Forms.ProgressBar
    Friend WithEvents ToolStripSeparator7 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents m_Import_FBX As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents m_remove_fbx As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator8 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents m_show_fbx As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator9 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents m_show_bsp2 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SplitContainer3 As System.Windows.Forms.SplitContainer
    Friend WithEvents chassis_cb As System.Windows.Forms.CheckBox
    Friend WithEvents hull_cb As System.Windows.Forms.CheckBox
    Friend WithEvents turret_cb As System.Windows.Forms.CheckBox
    Friend WithEvents gun_cb As System.Windows.Forms.CheckBox
    Friend WithEvents grid_cb As System.Windows.Forms.CheckBox
    Friend WithEvents wire_cb As System.Windows.Forms.CheckBox
    Friend WithEvents m_show_bsp2_tree As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents show_textures_cb As System.Windows.Forms.CheckBox
    Friend WithEvents pb2 As System.Windows.Forms.Panel
    Friend WithEvents m_edit_visual As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_write_primitive As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_show_model_info As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_simple_lighting As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator10 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents m_region As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator11 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripComboBox1 As System.Windows.Forms.ToolStripComboBox
    Friend WithEvents ToolStripSeparator12 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents m_Shader_Debug As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_show_environment As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents PB3 As System.Windows.Forms.PictureBox
    Friend WithEvents m_shadow_preview As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_shadows As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_shadowQuality As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_select_light As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_decal As System.Windows.Forms.ToolStripMenuItem

End Class
