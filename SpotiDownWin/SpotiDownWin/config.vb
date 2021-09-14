Public Class config
    Public Property metadata As New config_metadata
    Public Property prefernces As New config_preferences
    Public Property ffmpegpath As String = ""
    Public Property oauth As New config_oauth
End Class

Public Class config_metadata
    Public Property lyrics As Boolean = True
    Public Property artwork As Boolean = True
End Class

Public Class config_preferences
    Public Property quality As Integer = 3
    Public Property downloadpath As String = "SpotiDown\"
    Public Property filename As String = "{artist}\{title}"
End Class

Public Class config_oauth
    Public Property id As String = "bd6035158be648b695451636f2e311f2"
    Public Property secret As String = "0dd522b08dc646559e5fc7ef092fa9d7"
End Class