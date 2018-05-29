﻿Public Class FrmShadowSettings

    Private Sub FrmShadowSettings_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        AddHandler RadioButton1.Click, AddressOf CheckedChanged
        AddHandler RadioButton2.Click, AddressOf CheckedChanged
        AddHandler RadioButton3.Click, AddressOf CheckedChanged
        AddHandler RadioButton4.Click, AddressOf CheckedChanged
        set_buttons()
    End Sub
    Public Sub set_buttons()
        Dim val = CInt(My.Settings.shadow_quality)
        RadioButton1.Checked = False
        RadioButton2.Checked = False
        RadioButton3.Checked = False
        RadioButton4.Checked = False
        Select Case val
            Case 2048
                RadioButton1.Checked = True
            Case 1024
                RadioButton2.Checked = True
            Case 512
                RadioButton3.Checked = True
            Case 256
                RadioButton4.Checked = True
        End Select
    End Sub

    Private Sub CheckedChanged(sender As RadioButton, e As EventArgs) Handles RadioButton1.CheckedChanged
        If sender.Checked = False Then Return
        Dim val = CInt(sender.Tag)
        Select Case val
            Case 2048
                shadowMapSize = 2048
            Case 1024
                shadowMapSize = 1024
            Case 512
                shadowMapSize = 512
            Case 256
                shadowMapSize = 256
        End Select
        My.Settings.shadow_quality = shadowMapSize.ToString
        reset_shadowFbo()
    End Sub

End Class