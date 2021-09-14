Imports System.IO
Imports System.Reflection

Class Application
    Public Sub New()
        AddHandler AppDomain.CurrentDomain.AssemblyResolve, AddressOf OnResolveAssembly
    End Sub

    Private Shared Function OnResolveAssembly(sender As Object, args As ResolveEventArgs) As Assembly
        Try
            Dim parentAssembly = Assembly.GetExecutingAssembly()
            Dim finalname = args.Name.Substring(0, args.Name.IndexOf(","c)) & ".dll"
            Dim ResourcesList = parentAssembly.GetManifestResourceNames()
            Dim OurResourceName As String = Nothing
            For i As Integer = 0 To ResourcesList.Count - 1
                Dim name = ResourcesList(i)
                If name.EndsWith(finalname) Then
                    OurResourceName = name
                    Exit For
                End If
            Next

            If Not String.IsNullOrWhiteSpace(OurResourceName) Then
                Using stream As Stream = parentAssembly.GetManifestResourceStream(OurResourceName)
                    Dim block As Byte() = New Byte(stream.Length - 1) {}
                    stream.Read(block, 0, block.Length)
                    Return Assembly.Load(block)
                End Using
            Else
                Return Nothing
            End If
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
End Class
