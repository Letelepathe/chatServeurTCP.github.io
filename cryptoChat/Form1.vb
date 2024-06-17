Imports System.Net
Imports System.Net.Sockets
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Text
Public Class Form1
    Dim monserver As TcpListener
    Dim myclient As List(Of TcpClient)
    Dim client As Integer = 0
    Dim Client1 As client
    Friend sp = CreateObject("sapi.spvoice")

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Try
            If Button2.Text = "Start" Then
                monserver = New TcpListener(IPAddress.Parse("127.0.0.1"), CInt(TextBox3.Text))
                monserver.Start()
                myclient = New List(Of TcpClient)
                Dim context As TaskScheduler = TaskScheduler.FromCurrentSynchronizationContext
                Button2.Text = "Stop"
                Task.Factory.StartNew(Sub() accept(context), CancellationToken.None, TaskCreationOptions.LongRunning)
            Else
                monserver.Stop()
                Button2.Text = "Start"
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString)
        End Try
     
    End Sub
    Public Sub accept(ByVal context As TaskScheduler)
        Try
            While (True)
                Dim newclient As TcpClient = monserver.AcceptTcpClient
                myclient.Add(newclient)
                client += 1
                Task.Run(Sub() readclient(newclient.GetStream(), "client" & client.ToString, context))

            End While
        Catch ex As Exception

        End Try
    End Sub
    Private Sub readclient(ByVal stream As NetworkStream, ByVal nameclient As String, ByVal context As TaskScheduler)
        Try
            Dim buffer(4096) As Byte
            While (True)
                Dim read As Integer = stream.Read(buffer, 0, buffer.Length)
                If read > 0 Then
                    Dim message As String = Encoding.UTF8.GetString(buffer, 0, read)
                    ' messagereceive(nameclient + ": " + message)
                    Task.Factory.StartNew(Sub() messagereceive(nameclient + ":" + message), CancellationToken.None, TaskCreationOptions.None, context)

                Else
                    Task.Factory.StartNew(Sub() messagereceive(nameclient + ":disconnect"), CancellationToken.None, TaskCreationOptions.None, context)
                    Dim sp = CreateObject("sapi.spvoice")



                    'messagereceive(nameclient + ":disconnect")
                    Exit Sub
                End If
            End While

        Catch ex As Exception

        End Try
    End Sub
    Private Sub messagereceive(ByVal themassage)
        TextBox1.AppendText(themassage + vbNewLine)
        '  sp.speak(TextBox1.Text)
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If (myclient.Count > 0) And (TextBox2.Text IsNot "") Then
            Dim theMessage As String = TextBox2.Text
            Dim buffer() As Byte = Encoding.UTF8.GetBytes(theMessage)
            For Each client As TcpClient In myclient
                client.GetStream().Write(buffer, 0, buffer.Length)
            Next
        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Client1 = New client
        Client1.Show()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class
