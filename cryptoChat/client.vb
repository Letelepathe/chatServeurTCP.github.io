Imports System.Text
Imports System.Net
Imports System.Net.Sockets
Imports System.Threading
Imports System.Threading.Tasks
Public Class client
    Dim monclient As TcpClient

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Button1.Text = "Log In" Then
            monclient = New TcpClient()
            monclient.Connect(IPAddress.Parse(TextBox3.Text), CInt(TextBox4.Text))
            Dim context As TaskScheduler = TaskScheduler.FromCurrentSynchronizationContext
            Task.Run(Sub() liremessage(context, monclient.GetStream()))
            Button1.Text = "Log Out"
        Else
            monclient.Close()
            Button1.Text = "Log In"
        End If

    End Sub
    Private Sub liremessage(ByVal context As TaskScheduler, ByVal stream As NetworkStream)
        Try
            Dim buffer(4096) As Byte
            While (True)
                Dim lu As Integer = stream.Read(buffer, 0, buffer.Length)
                If lu > 0 Then
                    Dim message As String = Encoding.UTF8.GetString(buffer, 0, lu)
                    Task.Factory.StartNew(Sub() nouveaumessage(message, False), CancellationToken.None, TaskCreationOptions.None, context)
                Else
                    Task.Factory.StartNew(Sub() nouveaumessage("le serveur est stopper", True), CancellationToken.None, TaskCreationOptions.None, context)
                End If
            End While
        Catch ex As Exception

        End Try

    End Sub
    Private Sub nouveaumessage(ByVal message As String, ByVal fermer As Boolean)
        TextBox1.AppendText(message + vbNewLine)
        If fermer Then
            Button1.Text = "Log Out"
            monclient.Close()
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If Button1.Text = "Log Out" Then
            Dim buffer() As Byte = Encoding.UTF8.GetBytes(TextBox2.Text)
            monclient.GetStream().Write(buffer, 0, buffer.Length)
            TextBox2.Text = ""


        End If
    End Sub
End Class