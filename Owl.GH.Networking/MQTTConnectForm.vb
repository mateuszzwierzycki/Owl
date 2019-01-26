Imports System.Net.Mqtt
Imports System.Reactive.Linq
Imports System.Windows.Forms
Imports Grasshopper.Kernel

Public Class MQTTConnectForm

	Private _owner As MQTTConnectComponent = Nothing
	Public WithEvents Client As IMqttClient = Nothing
	Public WithEvents Observer As New MQTTObserver(Me)
	Dim topics As New HashSet(Of String)

	Public Event MessageReceived()

	Public Sub New(Owner As MQTTConnectComponent)
		' This call is required by the designer.
		InitializeComponent()

		' Add any initialization after the InitializeComponent() call.
		_owner = Owner
	End Sub

	Private Sub ButtonConnect_Click(sender As Object, e As EventArgs) Handles ButtonConnect.Click
		Connect()
	End Sub

	Private Sub ButtonDisconnect_Click(sender As Object, e As EventArgs) Handles ButtonDisconnect.Click
		Disconnect()
	End Sub

	Async Sub Disconnect()
		If Client Is Nothing Then Return
		OutputText.Text = "Disconnecting..."
		Await Client.DisconnectAsync()
		OutputText.Text = "Disconnected"
		_owner.Refresh()
	End Sub

	Async Sub Connect()
		OutputText.Text = "Connecting..."

		topics.Clear()

		Dim configuration As New MqttConfiguration()
		configuration.Port = BoxPort.Value
		configuration.ConnectionTimeoutSecs = BoxTimeout.Value
		configuration.KeepAliveSecs = BoxKeepAlive.Value

		Client = Await MqttClient.CreateAsync(BoxHost.Text, configuration)

		If BoxClientID.Text <> "" Then
			Dim credentials As New MqttClientCredentials(BoxClientID.Text, BoxUsername.Text, BoxPassword.Text)
			Dim sess As SessionState = Await Client.ConnectAsync(credentials, , CleanSessionBox.Checked)
		Else
			Dim sess As SessionState = Await Client.ConnectAsync()
		End If

		Dim stream As IObservable(Of MqttApplicationMessage) = Client.MessageStream

		For Each top As String In TopicList.Items
			topics.Add(top)
		Next

		Dim qos As Integer = Math.Max(0, QOSBox.SelectedIndex)

		For Each top As String In topics
			Client.SubscribeAsync(top, qos).Wait()
		Next

		stream.Where(AddressOf TopicCheck).Subscribe(Observer)

		If Client.IsConnected Then
			OutputText.Text = "Connected to " & BoxHost.Text
		Else
			OutputText.Text = "Disconnected"
		End If

		_owner.Refresh()
	End Sub

	Private Function TopicCheck(arg As MqttApplicationMessage) As Boolean
		If topics.Contains(arg.Topic) Then Return True
		Return False
	End Function

	Private Sub FormIsClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
		If e.CloseReason = CloseReason.UserClosing Then
			e.Cancel = True
			Me.Hide()
		End If
	End Sub

	Private Sub TopicInput_TextChanged(sender As Object, e As KeyEventArgs) Handles TopicInput.KeyDown
		If e.KeyCode = Keys.Enter Then
			TopicList.Items.Add(TopicInput.Text)
			TopicInput.Text = ""
		End If
	End Sub

	Private Sub TopicList_SelectedIndexChanged(sender As Object, e As KeyEventArgs) Handles TopicList.KeyDown
		If e.KeyCode = Keys.Delete Then
			If TopicList.SelectedIndex <> -1 Then
				TopicList.Items.RemoveAt(TopicList.SelectedIndex)
			End If
		End If
	End Sub

	Public Sub OnMessage(Message As MqttApplicationMessage)
		Me._owner.AddMessage(Message)
	End Sub

	Private Sub MQTTConnectForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

	End Sub

	Private Sub BoxClientID_TextChanged(sender As Object, e As EventArgs) Handles BoxClientID.TextChanged
		If BoxClientID.Text = "" Then
			Me._owner.NickName = "MQTT Connect"
		Else
			Me._owner.NickName = BoxClientID.Text
		End If

		Me._owner.OnDisplayExpired(True)
	End Sub
End Class

Public Class MQTTObserver
	Implements IObserver(Of MqttApplicationMessage)

	Public _owner As MQTTConnectForm = Nothing

	Public Sub New(Owner As MQTTConnectForm)
		_owner = Owner
	End Sub

	Public Sub OnNext(value As MqttApplicationMessage) Implements IObserver(Of MqttApplicationMessage).OnNext
		_owner.OnMessage(value)
	End Sub

	Public Sub OnError([error] As Exception) Implements IObserver(Of MqttApplicationMessage).OnError
		Debug.WriteLine([error].Message)
	End Sub

	Public Sub OnCompleted() Implements IObserver(Of MqttApplicationMessage).OnCompleted
		Debug.WriteLine("On Completed")
	End Sub
End Class