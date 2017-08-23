Imports System.Text
Imports System.IO
Module modGlobals
    Public FBX_OUT_PATH As String = ""
    Public FBX_OUT_NAME As String = ""
    Public Temp_Storage As String = ""
    Public bad_tanks As New StringBuilder
    Dim chassis_primitive, chassis_visual As MemoryStream
    Dim hull_primitive, hull_visual As MemoryStream
    Dim turret_prmitive, turret_visual As MemoryStream
    Dim gun_primitive, gun_visual As MemoryStream
    Public current_tank_package As Integer

    Public nation_folders() = {"vehicles/american", _
                            "vehicles/russian", _
                            "vehicles/chinese", _
                            "vehicles/french", _
                            "vehicles/british", _
                            "vehicles/german", _
                            "vehicles/japan"}
    Public tier_name() As String = { _
        "vehicles_level_01.pkg", _
        "vehicles_level_02.pkg", _
        "vehicles_level_03.pkg", _
        "vehicles_level_04.pkg", _
        "vehicles_level_05.pkg", _
        "vehicles_level_06.pkg", _
        "vehicles_level_07.pkg", _
        "vehicles_level_08.pkg", _
        "vehicles_level_09.pkg", _
        "vehicles_level_10.pkg"}
    Public tanks() As tank_nation
    Public Structure tank_nation
        Public nation As String
        Public count As Integer
        Public tank_data() As tank_data
    End Structure
    Public Structure tank_data
        Public nation As String
        Public tier As Integer
        Public tankname As String
        Public tankpath As String
        Public hull As String
        Public chassis As String
        Public gun As String
        Public turret As String
        Public texture As String
    End Structure
    Public t_list As New List(Of String)
    Public alltanks As New StringBuilder

    Public warp_shader, bump_shader, sun_shader, corona_shader As Integer
    Public grid As Integer
    Public _Started As Boolean = False
    Public app_path As String
    Public U_Cam_X_angle, U_Cam_Y_angle, Cam_X_angle, Cam_Y_angle As Single
    Public look_point_x, look_point_y, look_point_z As Single
    Public U_look_point_x, U_look_point_y, U_look_point_z As Single
    Public angle_offset, u_View_Radius As Single
    Public view_radius As Single
    Public cam_x, cam_y, cam_z As Single
    Public eyeX, eyeY, eyeZ As Single
    Public screen_avg_counter, screen_avg_draw_time, screen_draw_time, screen_totaled_draw_time As Double
    Public pause As Boolean = False
    Public track_mars As Boolean
    Public frmState As Integer = frmMain.WindowState
    Public gl_busy As Boolean = False
    Public current_png_path As String = ""
    Public Structure vect3Norm
        Public nx As Single
        Public ny As Single
        Public nz As Single
    End Structure
    Public Structure vect3
        Public x, y, z As Single
    End Structure
    Public Structure vec2
        Public x, y As Single
    End Structure
    Public Structure _indice
        Public a, b, c As Integer
    End Structure
End Module
