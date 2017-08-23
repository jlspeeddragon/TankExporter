Public Class frmFBX

    Private Sub Start_Export_btn_Click(sender As Object, e As EventArgs) Handles Start_Export_btn.Click
        frmMain.process_tank(False) 'false .. don't save the binary tank file
        modFBX.export_fbx()
        Me.Hide()
    End Sub

    Private Sub frmFBX_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        e.Cancel = True
    End Sub

    Private Sub frmFBX_Load(sender As Object, e As EventArgs) Handles Me.Load
        Label2.Text = ""
    End Sub

    Private Sub Cancel_bnt_Click(sender As Object, e As EventArgs) Handles Cancel_bnt.Click
        Me.Hide()
    End Sub
End Class