<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
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
        Me.pb1 = New System.Windows.Forms.PictureBox()
        Me.Startup_Timer = New System.Windows.Forms.Timer(Me.components)
        Me.MM = New System.Windows.Forms.MenuStrip()
        Me.m_file = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_load_file = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_save = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_open_temp_folder = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator()
        Me.M_Path = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_clear_temp_folder_data = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_reload_api_data = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.M_Exit = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_export_tank_list = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_clear_selected_tanks = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_chassis = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_hull = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_turret = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_gun = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_load_textures = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_edit_shaders = New System.Windows.Forms.ToolStripMenuItem()
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.font_holder = New System.Windows.Forms.Label()
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
        Me.SaveFileDialog1 = New System.Windows.Forms.SaveFileDialog()
        CType(Me.pb1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.MM.SuspendLayout()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.SplitContainer2.Panel1.SuspendLayout()
        Me.SplitContainer2.Panel2.SuspendLayout()
        Me.SplitContainer2.SuspendLayout()
        Me.TC1.SuspendLayout()
        CType(Me.iconbox, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.conMenu.SuspendLayout()
        Me.SuspendLayout()
        '
        'pb1
        '
        Me.pb1.BackColor = System.Drawing.Color.Black
        Me.pb1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.pb1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pb1.Location = New System.Drawing.Point(0, 0)
        Me.pb1.Name = "pb1"
        Me.pb1.Size = New System.Drawing.Size(442, 509)
        Me.pb1.TabIndex = 0
        Me.pb1.TabStop = False
        '
        'Startup_Timer
        '
        Me.Startup_Timer.Interval = 500
        '
        'MM
        '
        Me.MM.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_file, Me.m_export_tank_list, Me.m_clear_selected_tanks, Me.m_chassis, Me.m_hull, Me.m_turret, Me.m_gun, Me.m_load_textures, Me.m_edit_shaders})
        Me.MM.Location = New System.Drawing.Point(0, 0)
        Me.MM.Name = "MM"
        Me.MM.Size = New System.Drawing.Size(689, 24)
        Me.MM.TabIndex = 1
        '
        'm_file
        '
        Me.m_file.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_load_file, Me.m_save, Me.ToolStripSeparator1, Me.m_open_temp_folder, Me.ToolStripSeparator4, Me.M_Path, Me.ToolStripSeparator2, Me.m_clear_temp_folder_data, Me.m_reload_api_data, Me.ToolStripSeparator3, Me.M_Exit})
        Me.m_file.Name = "m_file"
        Me.m_file.Size = New System.Drawing.Size(37, 20)
        Me.m_file.Text = "&File"
        '
        'm_load_file
        '
        Me.m_load_file.Name = "m_load_file"
        Me.m_load_file.Size = New System.Drawing.Size(185, 22)
        Me.m_load_file.Text = "Load"
        '
        'm_save
        '
        Me.m_save.Name = "m_save"
        Me.m_save.Size = New System.Drawing.Size(185, 22)
        Me.m_save.Text = "Save"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(182, 6)
        '
        'm_open_temp_folder
        '
        Me.m_open_temp_folder.Name = "m_open_temp_folder"
        Me.m_open_temp_folder.Size = New System.Drawing.Size(185, 22)
        Me.m_open_temp_folder.Text = "Open Tank Folder"
        '
        'ToolStripSeparator4
        '
        Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
        Me.ToolStripSeparator4.Size = New System.Drawing.Size(182, 6)
        '
        'M_Path
        '
        Me.M_Path.Name = "M_Path"
        Me.M_Path.Size = New System.Drawing.Size(185, 22)
        Me.M_Path.Text = "Path"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(182, 6)
        '
        'm_clear_temp_folder_data
        '
        Me.m_clear_temp_folder_data.Name = "m_clear_temp_folder_data"
        Me.m_clear_temp_folder_data.Size = New System.Drawing.Size(185, 22)
        Me.m_clear_temp_folder_data.Text = "Clear Temp Folder"
        '
        'm_reload_api_data
        '
        Me.m_reload_api_data.Name = "m_reload_api_data"
        Me.m_reload_api_data.Size = New System.Drawing.Size(185, 22)
        Me.m_reload_api_data.Text = "Reload WoT API data"
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(182, 6)
        '
        'M_Exit
        '
        Me.M_Exit.Name = "M_Exit"
        Me.M_Exit.Size = New System.Drawing.Size(185, 22)
        Me.M_Exit.Text = "Exit"
        '
        'm_export_tank_list
        '
        Me.m_export_tank_list.Name = "m_export_tank_list"
        Me.m_export_tank_list.Size = New System.Drawing.Size(102, 20)
        Me.m_export_tank_list.Text = "Export Tank List"
        '
        'm_clear_selected_tanks
        '
        Me.m_clear_selected_tanks.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.m_clear_selected_tanks.Name = "m_clear_selected_tanks"
        Me.m_clear_selected_tanks.Size = New System.Drawing.Size(127, 20)
        Me.m_clear_selected_tanks.Text = "Clear Selected Tanks"
        '
        'm_chassis
        '
        Me.m_chassis.Checked = True
        Me.m_chassis.CheckOnClick = True
        Me.m_chassis.CheckState = System.Windows.Forms.CheckState.Checked
        Me.m_chassis.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.m_chassis.Font = New System.Drawing.Font("Lucida Console", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.m_chassis.ForeColor = System.Drawing.Color.Green
        Me.m_chassis.Name = "m_chassis"
        Me.m_chassis.Size = New System.Drawing.Size(31, 20)
        Me.m_chassis.Text = "C"
        '
        'm_hull
        '
        Me.m_hull.Checked = True
        Me.m_hull.CheckOnClick = True
        Me.m_hull.CheckState = System.Windows.Forms.CheckState.Checked
        Me.m_hull.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.m_hull.Font = New System.Drawing.Font("Lucida Console", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.m_hull.ForeColor = System.Drawing.Color.Green
        Me.m_hull.Name = "m_hull"
        Me.m_hull.Size = New System.Drawing.Size(31, 20)
        Me.m_hull.Text = "H"
        '
        'm_turret
        '
        Me.m_turret.BackColor = System.Drawing.Color.Black
        Me.m_turret.Checked = True
        Me.m_turret.CheckOnClick = True
        Me.m_turret.CheckState = System.Windows.Forms.CheckState.Checked
        Me.m_turret.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.m_turret.Font = New System.Drawing.Font("Lucida Console", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.m_turret.ForeColor = System.Drawing.Color.Green
        Me.m_turret.Name = "m_turret"
        Me.m_turret.Size = New System.Drawing.Size(31, 20)
        Me.m_turret.Text = "T"
        '
        'm_gun
        '
        Me.m_gun.Checked = True
        Me.m_gun.CheckOnClick = True
        Me.m_gun.CheckState = System.Windows.Forms.CheckState.Checked
        Me.m_gun.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.m_gun.Font = New System.Drawing.Font("Lucida Console", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.m_gun.ForeColor = System.Drawing.Color.Green
        Me.m_gun.Name = "m_gun"
        Me.m_gun.Size = New System.Drawing.Size(31, 20)
        Me.m_gun.Text = "G"
        '
        'm_load_textures
        '
        Me.m_load_textures.Checked = True
        Me.m_load_textures.CheckOnClick = True
        Me.m_load_textures.CheckState = System.Windows.Forms.CheckState.Checked
        Me.m_load_textures.ForeColor = System.Drawing.Color.Red
        Me.m_load_textures.Name = "m_load_textures"
        Me.m_load_textures.Size = New System.Drawing.Size(92, 20)
        Me.m_load_textures.Text = "Load Textures"
        '
        'm_edit_shaders
        '
        Me.m_edit_shaders.Name = "m_edit_shaders"
        Me.m_edit_shaders.Size = New System.Drawing.Size(83, 20)
        Me.m_edit_shaders.Text = "Edit Shaders"
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
        Me.SplitContainer1.IsSplitterFixed = True
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 24)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.SplitContainer1.Panel1.Controls.Add(Me.font_holder)
        Me.SplitContainer1.Panel1.Controls.Add(Me.info_Label)
        Me.SplitContainer1.Panel1.Controls.Add(Me.pb1)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer))
        Me.SplitContainer1.Panel2.Controls.Add(Me.SplitContainer2)
        Me.SplitContainer1.Size = New System.Drawing.Size(689, 509)
        Me.SplitContainer1.SplitterDistance = 442
        Me.SplitContainer1.SplitterWidth = 1
        Me.SplitContainer1.TabIndex = 2
        '
        'font_holder
        '
        Me.font_holder.AutoSize = True
        Me.font_holder.Font = New System.Drawing.Font("Candara", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.font_holder.ForeColor = System.Drawing.Color.White
        Me.font_holder.Location = New System.Drawing.Point(41, 456)
        Me.font_holder.Name = "font_holder"
        Me.font_holder.Size = New System.Drawing.Size(86, 18)
        Me.font_holder.TabIndex = 1
        Me.font_holder.Text = "For font only"
        Me.font_holder.Visible = False
        '
        'info_Label
        '
        Me.info_Label.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.info_Label.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.info_Label.ForeColor = System.Drawing.Color.Silver
        Me.info_Label.Location = New System.Drawing.Point(135, 165)
        Me.info_Label.Name = "info_Label"
        Me.info_Label.Size = New System.Drawing.Size(212, 23)
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
        Me.SplitContainer2.Size = New System.Drawing.Size(246, 509)
        Me.SplitContainer2.SplitterDistance = 358
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
        Me.TC1.Size = New System.Drawing.Size(246, 358)
        Me.TC1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed
        Me.TC1.TabIndex = 0
        '
        'TabPage1
        '
        Me.TabPage1.Location = New System.Drawing.Point(4, 25)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(238, 329)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "1"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'TabPage2
        '
        Me.TabPage2.Location = New System.Drawing.Point(4, 25)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(235, 329)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "2"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'TabPage3
        '
        Me.TabPage3.Location = New System.Drawing.Point(4, 25)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage3.Size = New System.Drawing.Size(235, 329)
        Me.TabPage3.TabIndex = 2
        Me.TabPage3.Text = "3"
        Me.TabPage3.UseVisualStyleBackColor = True
        '
        'TabPage4
        '
        Me.TabPage4.Location = New System.Drawing.Point(4, 25)
        Me.TabPage4.Name = "TabPage4"
        Me.TabPage4.Size = New System.Drawing.Size(235, 329)
        Me.TabPage4.TabIndex = 3
        Me.TabPage4.Text = "4"
        Me.TabPage4.UseVisualStyleBackColor = True
        '
        'TabPage5
        '
        Me.TabPage5.Location = New System.Drawing.Point(4, 25)
        Me.TabPage5.Name = "TabPage5"
        Me.TabPage5.Size = New System.Drawing.Size(235, 329)
        Me.TabPage5.TabIndex = 4
        Me.TabPage5.Text = "5"
        Me.TabPage5.UseVisualStyleBackColor = True
        '
        'TabPage6
        '
        Me.TabPage6.Location = New System.Drawing.Point(4, 25)
        Me.TabPage6.Name = "TabPage6"
        Me.TabPage6.Size = New System.Drawing.Size(235, 329)
        Me.TabPage6.TabIndex = 5
        Me.TabPage6.Text = "6"
        Me.TabPage6.UseVisualStyleBackColor = True
        '
        'TabPage7
        '
        Me.TabPage7.Location = New System.Drawing.Point(4, 25)
        Me.TabPage7.Name = "TabPage7"
        Me.TabPage7.Size = New System.Drawing.Size(235, 329)
        Me.TabPage7.TabIndex = 6
        Me.TabPage7.Text = "7"
        Me.TabPage7.UseVisualStyleBackColor = True
        '
        'TabPage8
        '
        Me.TabPage8.Location = New System.Drawing.Point(4, 25)
        Me.TabPage8.Name = "TabPage8"
        Me.TabPage8.Size = New System.Drawing.Size(235, 329)
        Me.TabPage8.TabIndex = 7
        Me.TabPage8.Text = "8"
        Me.TabPage8.UseVisualStyleBackColor = True
        '
        'TabPage9
        '
        Me.TabPage9.Location = New System.Drawing.Point(4, 25)
        Me.TabPage9.Name = "TabPage9"
        Me.TabPage9.Size = New System.Drawing.Size(235, 329)
        Me.TabPage9.TabIndex = 8
        Me.TabPage9.Text = "9"
        Me.TabPage9.UseVisualStyleBackColor = True
        '
        'TabPage10
        '
        Me.TabPage10.Location = New System.Drawing.Point(4, 25)
        Me.TabPage10.Name = "TabPage10"
        Me.TabPage10.Size = New System.Drawing.Size(235, 329)
        Me.TabPage10.TabIndex = 9
        Me.TabPage10.Text = "10"
        Me.TabPage10.UseVisualStyleBackColor = True
        '
        'tank_label
        '
        Me.tank_label.AutoSize = True
        Me.tank_label.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tank_label.ForeColor = System.Drawing.Color.White
        Me.tank_label.Location = New System.Drawing.Point(4, 3)
        Me.tank_label.Name = "tank_label"
        Me.tank_label.Size = New System.Drawing.Size(49, 16)
        Me.tank_label.TabIndex = 3
        Me.tank_label.Text = "Label1"
        '
        'iconbox
        '
        Me.iconbox.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.iconbox.BackColor = System.Drawing.Color.Transparent
        Me.iconbox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.iconbox.Location = New System.Drawing.Point(3, -2)
        Me.iconbox.Name = "iconbox"
        Me.iconbox.Size = New System.Drawing.Size(243, 152)
        Me.iconbox.TabIndex = 2
        Me.iconbox.TabStop = False
        '
        'conMenu
        '
        Me.conMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_load, Me.m_export_fbx})
        Me.conMenu.Name = "conMenu"
        Me.conMenu.Size = New System.Drawing.Size(132, 48)
        '
        'm_load
        '
        Me.m_load.Name = "m_load"
        Me.m_load.Size = New System.Drawing.Size(131, 22)
        Me.m_load.Text = "Load This.."
        '
        'm_export_fbx
        '
        Me.m_export_fbx.Name = "m_export_fbx"
        Me.m_export_fbx.Size = New System.Drawing.Size(131, 22)
        Me.m_export_fbx.Text = "Export FBX"
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Black
        Me.ClientSize = New System.Drawing.Size(689, 533)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.MM)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MM
        Me.Name = "frmMain"
        Me.Text = "Tank Exporter"
        CType(Me.pb1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.MM.ResumeLayout(False)
        Me.MM.PerformLayout()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel1.PerformLayout()
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.ResumeLayout(False)
        Me.SplitContainer2.Panel1.ResumeLayout(False)
        Me.SplitContainer2.Panel2.ResumeLayout(False)
        Me.SplitContainer2.Panel2.PerformLayout()
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
    Friend WithEvents m_chassis As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_hull As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_turret As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_gun As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tank_label As System.Windows.Forms.Label
    Friend WithEvents m_open_temp_folder As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator4 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents SaveFileDialog1 As System.Windows.Forms.SaveFileDialog
    Friend WithEvents m_export_fbx As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_load_textures As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_edit_shaders As System.Windows.Forms.ToolStripMenuItem

End Class
