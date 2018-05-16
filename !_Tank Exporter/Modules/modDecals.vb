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



Module modDecals
    Public decal_matrix_list() As decal_matrix_list_
    Public Structure decal_matrix_list_
        Public x_angle As Single
        Public y_angle As Single
        Public scale As vect3
        Public translate As vect3
        Public rotation As vect3
        Public u_wrap As Single
        Public v_wrap As Single
        Public decal_data() As vertex_data
        Public texture_id As Integer
        Public normal_id As Integer
        Public gmm_id As Integer
        Public display_id As Integer
        Public decal_texture As String
        Public decal_normal As String
        Public decal_gmm As String
        Public matrix() As Single

        Public display_matrix() As Single
        Public y_rotate_matrix() As Single
        Public x_rotate_matrix() As Single
        Public scale_matrix() As Single
        Public traslate_matrix() As Single

        Public good As Boolean
        Public offset As vect4
        Public priority As Integer
        Public influence As Integer
        Public texture_matrix() As Single
        Public lbl As vect3
        Public lbr As vect3
        Public ltl As vect3
        Public ltr As vect3
        Public rbl As vect3
        Public rbr As vect3
        Public rtl As vect3
        Public rtr As vect3
        Public BB() As vect3
        Public visible As Boolean
        Public flags As UInteger
        Public cull_method As Integer
        Public pi2 As Single
        Public Sub load_indenity()
            pi2 = PI * 2.0
            g_decal_scale = scale
            If Sqrt(scale.x ^ 2 + scale.y ^ 2 + scale.z ^ 2) = 0.0 Then
                g_decal_scale.x = 1.0
                g_decal_scale.y = 1.0
                g_decal_scale.z = 1.0

            End If
            g_decal_translate = translate
            g_decal_rotate = rotation
            matrix = { _
                1.0, 0.0, 0.0, 0.0, _
                0.0, 1.0, 0.0, 0.0, _
                0.0, 0.0, 1.0, 0.0, _
                0.0, 0.0, 0.0, 1.0}
            display_matrix = { _
                1.0, 0.0, 0.0, 0.0, _
                0.0, 1.0, 0.0, 0.0, _
                0.0, 0.0, 1.0, 0.0, _
                0.0, 0.0, 0.0, 1.0}
            y_rotate_matrix = { _
                1.0, 0.0, 0.0, 0.0, _
                0.0, 1.0, 0.0, 0.0, _
                0.0, 0.0, 1.0, 0.0, _
                0.0, 0.0, 0.0, 1.0}
            x_rotate_matrix = { _
                1.0, 0.0, 0.0, 0.0, _
                0.0, 1.0, 0.0, 0.0, _
                0.0, 0.0, 1.0, 0.0, _
                0.0, 0.0, 0.0, 1.0}
            scale_matrix = { _
                1.0, 0.0, 0.0, 0.0, _
                0.0, 1.0, 0.0, 0.0, _
                0.0, 0.0, 1.0, 0.0, _
                0.0, 0.0, 0.0, 1.0}
            traslate_matrix = { _
                1.0, 0.0, 0.0, 0.0, _
                0.0, 1.0, 0.0, 0.0, _
                0.0, 0.0, 1.0, 0.0, _
                0.0, 0.0, 0.0, 1.0}
        End Sub
        Public Sub transform()
            Me.rotation = g_decal_rotate
            Gl.glPushMatrix()
            Gl.glLoadIdentity()
            Gl.glMultMatrixf(traslate_matrix)
            Gl.glMultMatrixf(y_rotate_matrix)
            Gl.glMultMatrixf(x_rotate_matrix)
            Gl.glMultMatrixf(scale_matrix)
            Gl.glGetFloatv(Gl.GL_MODELVIEW_MATRIX, display_matrix)
            Gl.glPopMatrix()
        End Sub

        Public Function set_y_rotation_matrix(x As Single)
            rotation = g_decal_rotate
            x_angle += x
            If x_angle > pi2 Then
                x_angle -= pi2
            End If
            If x_angle < -pi2 Then
                x_angle += pi2
            End If
            Dim s = Sin(x)
            Dim c = Cos(x)
            y_rotate_matrix = { _
              c, 0.0, -s, 0.0, _
              0.0, 1.0, 0.0, 0.0, _
              s, 0.0, c, 0.0, _
              0.0, 0.0, 0.0, 1.0}
            Return matrix
        End Function
        Public Function set_x_rotation_matrix(y As Single)
            y_angle += y
            If y_angle > pi2 Then
                y_angle -= pi2
            End If
            If y_angle < -pi2 Then
                y_angle += pi2
            End If
            Dim s = Sin(y)
            Dim c = Cos(y)
            x_rotate_matrix = { _
                1.0, 0.0, 0.0, 0.0, _
                0.0, c, s, 0.0, _
                0.0, -s, c, 0.0, _
                0.0, 0.0, 0.0, 1.0}
            'Dim matrix() As Single = { _
            '  c, -s, 0.0, 0.0, _
            '  s, c, 0.0, 0.0, _
            '  0.0, 0.0, 1.0, 0.0, _
            '  0.0, 0.0, 0.0, 1.0}
            Return matrix
        End Function
        Public Function set_scale_matrix(s As vect3)
            scale = s
            scale_matrix = { _
                s.x, 0.0, 0.0, 0.0, _
                0.0, s.y, 0.0, 0.0, _
                0.0, 0.0, s.z, 0.0, _
                0.0, 0.0, 0.0, 1.0}

            Return matrix
        End Function

        Public Sub set_translate_matrix(id As Integer, v As vect3)
            translate = g_decal_translate
            decal_matrix_list(id).traslate_matrix(12) = v.x
            decal_matrix_list(id).traslate_matrix(13) = v.y
            decal_matrix_list(id).traslate_matrix(14) = v.z
        End Sub
    End Structure
    Public Structure vertex_data
        Public x As Single
        Public y As Single
        Public z As Single
        Public u As Single
        Public v As Single
        Public nx As Single
        Public ny As Single
        Public nz As Single
        Public map As Integer
        Public t As vect3
        Public bt As vect3
    End Structure

    Public Sub make_test_decal(id As Integer)

        If decal_matrix_list Is Nothing Then
            ReDim Preserve decal_matrix_list(id + 1)
        End If
        If decal_matrix_list.Length - 1 > id Then
            decal_matrix_list(id) = New decal_matrix_list_
        End If
        With decal_matrix_list(id)
            .load_indenity()
            Gl.glDeleteLists(.display_id, 1)
            .display_id = Gl.glGenLists(1)
            Gl.glNewList(.display_id, Gl.GL_COMPILE)

            get_box_corners(0, 0.5) ' creates coordinates

            Gl.glBegin(Gl.GL_QUADS)
            make_decal_box(id) ' draws the box
            Gl.glEnd()
            Gl.glEndList()
        End With
    End Sub


    Private Sub make_decal_box(ByVal decal As Integer)
        With decal_matrix_list(decal)
            '1 right
            Gl.glNormal3f(1.0, 0.0, 0.0)
            Gl.glVertex3f(.lbr.x, .lbr.y, .lbr.z)
            Gl.glVertex3f(.ltr.x, .ltr.y, .ltr.z)
            Gl.glVertex3f(.rtr.x, .rtr.y, .rtr.z)
            Gl.glVertex3f(.rbr.x, .rbr.y, .rbr.z)
            '2 back
            Gl.glNormal3f(0.0, 0.0, -1.0)
            Gl.glVertex3f(.lbl.x, .lbl.y, .lbl.z)
            Gl.glVertex3f(.ltl.x, .ltl.y, .ltl.z)
            Gl.glVertex3f(.ltr.x, .ltr.y, .ltr.z)
            Gl.glVertex3f(.lbr.x, .lbr.y, .lbr.z)
            '3 left
            Gl.glNormal3f(-1.0, 0.0, 0.0)
            Gl.glVertex3f(.rbl.x, .rbl.y, .rbl.z)
            Gl.glVertex3f(.rtl.x, .rtl.y, .rtl.z)
            Gl.glVertex3f(.ltl.x, .ltl.y, .ltl.z)
            Gl.glVertex3f(.lbl.x, .lbl.y, .lbl.z)
            '4 front
            Gl.glNormal3f(0.0, 0.0, 1.0)
            Gl.glVertex3f(.rbr.x, .rbr.y, .rbr.z)
            Gl.glVertex3f(.rtr.x, .rtr.y, .rtr.z)
            Gl.glVertex3f(.rtl.x, .rtl.y, .rtl.z)
            Gl.glVertex3f(.rbl.x, .rbl.y, .rbl.z)
            '5 top
            Gl.glNormal3f(0.0, 1.0, 0.0)
            Gl.glVertex3f(.rtr.x, .rtr.y, .rtr.z)
            Gl.glVertex3f(.ltr.x, .ltr.y, .ltr.z)
            Gl.glVertex3f(.ltl.x, .ltl.y, .ltl.z)
            Gl.glVertex3f(.rtl.x, .rtl.y, .rtl.z)
            '6 bottom
            Gl.glNormal3f(0.0, -1.0, 0.0)
            Gl.glVertex3f(.rbl.x, .rbl.y, .rbl.z)
            Gl.glVertex3f(.lbl.x, .lbl.y, .lbl.z)
            Gl.glVertex3f(.lbr.x, .lbr.y, .lbr.z)
            Gl.glVertex3f(.rbr.x, .rbr.y, .rbr.z)


        End With

    End Sub

    Private Sub get_box_corners(ByVal decal As Integer, ByVal z_scale As Single)
        With decal_matrix_list(decal)
            ReDim .BB(8)
            ' left side -----------
            .lbl.x = -0.5 'left bottom left
            .lbl.y = -0.5
            .lbl.z = -z_scale
            .BB(0) = .lbl
            '
            .lbr.x = 0.5 ' left bottom right
            .lbr.y = -0.5
            .lbr.z = -z_scale
            .BB(1) = .lbr
            '
            .ltl.x = -0.5 'left top left
            .ltl.y = 0.5
            .ltl.z = -z_scale
            .BB(2) = .ltl
            '
            .ltr.x = 0.5 ' left top right
            .ltr.y = 0.5
            .ltr.z = -z_scale
            .BB(3) = .ltr
            ' right side ----------
            .rbl.x = -0.5 ' right bottom left
            .rbl.y = -0.5
            .rbl.z = z_scale
            .BB(4) = .rbl
            '
            .rbr.x = 0.5 ' right bottom right
            .rbr.y = -0.5
            .rbr.z = z_scale
            .BB(5) = .rbr
            '
            .rtl.x = -0.5 ' right top left
            .rtl.y = 0.5
            .rtl.z = z_scale
            .BB(6) = .rtl
            '
            .rtr.x = 0.5 ' right top right
            .rtr.y = 0.5
            .rtr.z = z_scale
            .BB(7) = .rtr

        End With
        For i = 0 To 7
            decal_matrix_list(decal).BB(i) = translate_to(decal_matrix_list(decal).BB(i), decal_matrix_list(decal).matrix)
        Next
    End Sub
    Public Function rotate_only(ByVal v As vect3, ByVal m() As Single) As vect3
        Dim vo As vect3
        vo.x = (m(0) * v.x) + (m(4) * v.y) + (m(8) * v.z)
        vo.y = (m(1) * v.x) + (m(5) * v.y) + (m(9) * v.z)
        vo.z = (m(2) * v.x) + (m(6) * v.y) + (m(10) * v.z)

        Return vo

    End Function
    Public Function translate_to(ByVal v As vect3, ByVal m() As Single) As vect3
        Dim vo As vect3
        vo.x = (m(0) * v.x) + (m(4) * v.y) + (m(8) * v.z)
        vo.y = (m(1) * v.x) + (m(5) * v.y) + (m(9) * v.z)
        vo.z = (m(2) * v.x) + (m(6) * v.y) + (m(10) * v.z)

        vo.x += m(12)
        vo.y += m(13)
        vo.z += m(14)
        Return vo

    End Function
    Public Function translate_only(ByVal v As vect3, ByVal m() As Single) As vect3
        Dim vo As vect3
        vo.x += m(12)
        vo.y += m(13)
        vo.z += m(14)
        Return vo

    End Function
    Private Function transform(ByRef m() As Single, ByVal v As vertex_data, ByRef scale As Single, ByRef k As Integer) As vertex_data
        Dim vo As vertex_data
        v.x *= scale
        v.y *= scale
        vo.x = (m(0) * v.x) + (m(4) * v.y) + (m(8) * v.z)
        vo.y = (m(1) * v.x) + (m(5) * v.y) + (m(9) * v.z)
        vo.z = (m(2) * v.x) + (m(6) * v.y) + (m(10) * v.z)

        vo.u = v.u
        vo.v = v.v * -1.0

        vo.x += m(12)
        vo.y += m(13)
        vo.z += m(14)

        Return vo
    End Function
    Private Function rotate_decal_view(ByVal m() As Single) As vect3
        Dim vo As vect3
        Dim v As vect3
        v.x = 0.0
        v.y = 1.0
        v.z = 0.0
        vo.x = (m(0) * v.x) + (m(4) * v.y) + (m(8) * v.z)
        vo.y = (m(1) * v.x) + (m(5) * v.y) + (m(9) * v.z)
        vo.z = (m(2) * v.x) + (m(6) * v.y) + (m(10) * v.z)
        Dim l = Sqrt((vo.x ^ 2) + (vo.y ^ 2) + (vo.z ^ 2))
        If l = 0.0 Then l = 1.0
        vo.x /= l
        vo.y /= l
        vo.z /= l

        Return vo
    End Function

End Module
