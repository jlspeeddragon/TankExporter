<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmWritePrimitive
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmWritePrimitive))
        Me.Label1 = New System.Windows.Forms.Label()
        Me.cew_cb = New System.Windows.Forms.CheckBox()
        Me.hew_cb = New System.Windows.Forms.CheckBox()
        Me.tew_cb = New System.Windows.Forms.CheckBox()
        Me.gew_cb = New System.Windows.Forms.CheckBox()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.SaveFileDialog1 = New System.Windows.Forms.SaveFileDialog()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(84, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Modified Groups"
        '
        'cew_cb
        '
        Me.cew_cb.AutoSize = True
        Me.cew_cb.Enabled = False
        Me.cew_cb.Location = New System.Drawing.Point(15, 34)
        Me.cew_cb.Name = "cew_cb"
        Me.cew_cb.Size = New System.Drawing.Size(62, 17)
        Me.cew_cb.TabIndex = 1
        Me.cew_cb.Text = "Chassis"
        Me.cew_cb.UseVisualStyleBackColor = True
        '
        'hew_cb
        '
        Me.hew_cb.AutoSize = True
        Me.hew_cb.Location = New System.Drawing.Point(15, 57)
        Me.hew_cb.Name = "hew_cb"
        Me.hew_cb.Size = New System.Drawing.Size(44, 17)
        Me.hew_cb.TabIndex = 2
        Me.hew_cb.Text = "Hull"
        Me.hew_cb.UseVisualStyleBackColor = True
        '
        'tew_cb
        '
        Me.tew_cb.AutoSize = True
        Me.tew_cb.Location = New System.Drawing.Point(15, 80)
        Me.tew_cb.Name = "tew_cb"
        Me.tew_cb.Size = New System.Drawing.Size(54, 17)
        Me.tew_cb.TabIndex = 3
        Me.tew_cb.Text = "Turret"
        Me.tew_cb.UseVisualStyleBackColor = True
        '
        'gew_cb
        '
        Me.gew_cb.AutoSize = True
        Me.gew_cb.Enabled = False
        Me.gew_cb.Location = New System.Drawing.Point(15, 103)
        Me.gew_cb.Name = "gew_cb"
        Me.gew_cb.Size = New System.Drawing.Size(46, 17)
        Me.gew_cb.TabIndex = 4
        Me.gew_cb.Text = "Gun"
        Me.gew_cb.UseVisualStyleBackColor = True
        '
        'Button1
        '
        Me.Button1.ForeColor = System.Drawing.Color.Black
        Me.Button1.Location = New System.Drawing.Point(15, 127)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 5
        Me.Button1.Text = "Write File(s)"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'frmWritePrimitive
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(109, 175)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.gew_cb)
        Me.Controls.Add(Me.tew_cb)
        Me.Controls.Add(Me.hew_cb)
        Me.Controls.Add(Me.cew_cb)
        Me.Controls.Add(Me.Label1)
        Me.ForeColor = System.Drawing.Color.Gainsboro
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmWritePrimitive"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Primitive Writer"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents cew_cb As System.Windows.Forms.CheckBox
    Friend WithEvents hew_cb As System.Windows.Forms.CheckBox
    Friend WithEvents tew_cb As System.Windows.Forms.CheckBox
    Friend WithEvents gew_cb As System.Windows.Forms.CheckBox
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents SaveFileDialog1 As System.Windows.Forms.SaveFileDialog
End Class
