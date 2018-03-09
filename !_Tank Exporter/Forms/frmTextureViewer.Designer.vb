﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmTextureViewer
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmTextureViewer))
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
        Me.m_alpha_enabled = New System.Windows.Forms.ToolStripButton()
        Me.m_show_uvs = New System.Windows.Forms.ToolStripButton()
        Me.m_uvs_only = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.zoom = New System.Windows.Forms.ToolStripTextBox()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_save_image = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_top_most = New System.Windows.Forms.ToolStripButton()
        Me.SaveFileDialog1 = New System.Windows.Forms.SaveFileDialog()
        Me.ToolStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ToolStrip1
        '
        Me.ToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_alpha_enabled, Me.m_show_uvs, Me.m_uvs_only, Me.ToolStripSeparator1, Me.zoom, Me.ToolStripSeparator2, Me.m_save_image, Me.ToolStripSeparator3, Me.m_top_most})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
        Me.ToolStrip1.Size = New System.Drawing.Size(474, 25)
        Me.ToolStrip1.TabIndex = 0
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'm_alpha_enabled
        '
        Me.m_alpha_enabled.CheckOnClick = True
        Me.m_alpha_enabled.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.m_alpha_enabled.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.m_alpha_enabled.Name = "m_alpha_enabled"
        Me.m_alpha_enabled.Size = New System.Drawing.Size(42, 22)
        Me.m_alpha_enabled.Text = "Alpha"
        '
        'm_show_uvs
        '
        Me.m_show_uvs.CheckOnClick = True
        Me.m_show_uvs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.m_show_uvs.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.m_show_uvs.Name = "m_show_uvs"
        Me.m_show_uvs.Size = New System.Drawing.Size(63, 22)
        Me.m_show_uvs.Text = "Show UVs"
        '
        'm_uvs_only
        '
        Me.m_uvs_only.CheckOnClick = True
        Me.m_uvs_only.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.m_uvs_only.Image = CType(resources.GetObject("m_uvs_only.Image"), System.Drawing.Image)
        Me.m_uvs_only.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.m_uvs_only.Name = "m_uvs_only"
        Me.m_uvs_only.Size = New System.Drawing.Size(59, 22)
        Me.m_uvs_only.Text = "UVs Only"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 25)
        '
        'zoom
        '
        Me.zoom.Name = "zoom"
        Me.zoom.Size = New System.Drawing.Size(100, 25)
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 25)
        '
        'm_save_image
        '
        Me.m_save_image.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.m_save_image.Image = CType(resources.GetObject("m_save_image.Image"), System.Drawing.Image)
        Me.m_save_image.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.m_save_image.Name = "m_save_image"
        Me.m_save_image.Size = New System.Drawing.Size(71, 22)
        Me.m_save_image.Text = "Save Image"
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(6, 25)
        '
        'm_top_most
        '
        Me.m_top_most.CheckOnClick = True
        Me.m_top_most.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.m_top_most.ForeColor = System.Drawing.Color.Red
        Me.m_top_most.Image = CType(resources.GetObject("m_top_most.Image"), System.Drawing.Image)
        Me.m_top_most.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.m_top_most.Name = "m_top_most"
        Me.m_top_most.Size = New System.Drawing.Size(62, 22)
        Me.m_top_most.Text = "Top Most"
        '
        'SaveFileDialog1
        '
        Me.SaveFileDialog1.Filter = "PNG (*.png)|*.png"
        Me.SaveFileDialog1.Title = "Save Png"
        '
        'frmTextureViewer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackgroundImage = Global.Tank_Exporter.My.Resources.Resources.gradiant
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.ClientSize = New System.Drawing.Size(474, 462)
        Me.Controls.Add(Me.ToolStrip1)
        Me.DoubleBuffered = True
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmTextureViewer"
        Me.Text = "Texture Viewer"
        Me.TopMost = True
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ToolStrip1 As System.Windows.Forms.ToolStrip
    Friend WithEvents m_alpha_enabled As System.Windows.Forms.ToolStripButton
    Friend WithEvents zoom As System.Windows.Forms.ToolStripTextBox
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents m_show_uvs As System.Windows.Forms.ToolStripButton
    Friend WithEvents m_top_most As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents m_save_image As System.Windows.Forms.ToolStripButton
    Friend WithEvents SaveFileDialog1 As System.Windows.Forms.SaveFileDialog
    Friend WithEvents m_uvs_only As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
End Class
