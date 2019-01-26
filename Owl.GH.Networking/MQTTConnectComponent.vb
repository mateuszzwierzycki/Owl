Imports System.Net.Mqtt
Imports GH_IO.Serialization
Imports Grasshopper.Kernel
Imports Rhino

Public Class MQTTConnectComponent
	Inherits GH_Component

	Public WithEvents ConnectForm As New MQTTConnectForm(Me)

	Sub New()
		MyBase.New("MQTT Connect", "MQTT Connect", "MQTT Connect", "Owl", "I/O")
	End Sub

	Public Overrides ReadOnly Property ComponentGuid As Guid
		Get
			Return New Guid("{EE0CCFA4-F255-4397-8D0F-15B8B6E85945}")
		End Get
	End Property

	Public Overrides ReadOnly Property Exposure As GH_Exposure
		Get
			Return GH_Exposure.tertiary
		End Get
	End Property

	Public Overrides Sub AddedToDocument(document As GH_Document)
		MyBase.AddedToDocument(document)
		ConnectForm.Disconnect()
	End Sub

	Public Overrides Sub RemovedFromDocument(document As GH_Document)
		MyBase.RemovedFromDocument(document)
		ConnectForm.Disconnect()
	End Sub

	Public Overrides Sub CreateAttributes()
		Me.Attributes = New MQTTConnectAtts(Me)
	End Sub

	Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)

	End Sub

	Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
		pManager.AddBooleanParameter("Connected", "C", "Connected", GH_ParamAccess.item)
		pManager.AddGenericParameter("Client", "C", "Client", GH_ParamAccess.item)
		pManager.AddGenericParameter("Buffer", "B", "Message buffer", GH_ParamAccess.item)
	End Sub

	Public Overrides Function Read(reader As GH_IReader) As Boolean
		ConnectForm.BoxClientID.Text = reader.GetString("clientid")
		ConnectForm.BoxHost.Text = reader.GetString("host")
		ConnectForm.BoxPort.Value = reader.GetInt32("port")
		ConnectForm.BoxUsername.Text = reader.GetString("username")
		ConnectForm.BoxPassword.Text = reader.GetString("password")
		ConnectForm.BoxReconnect.Value = reader.GetInt32("reconnect")
		ConnectForm.BoxTimeout.Value = reader.GetInt32("timeout")
		ConnectForm.BoxKeepAlive.Value = reader.GetInt32("alive")
		ConnectForm.QOSBox.SelectedIndex = Math.Max(0, reader.GetInt32("qos"))
		ConnectForm.CleanSessionBox.Checked = reader.GetBoolean("clean")

		Dim topcount As Integer = reader.GetInt32("topiccount")

		For i As Integer = 0 To topcount - 1
			ConnectForm.TopicList.Items.Add(reader.GetString("topic_" & i))
		Next

		Return MyBase.Read(reader)
	End Function

	Public Overrides Function Write(writer As GH_IWriter) As Boolean
		writer.SetString("clientid", ConnectForm.BoxClientID.Text)
		writer.SetString("host", ConnectForm.BoxHost.Text)
		writer.SetInt32("port", ConnectForm.BoxPort.Value)
		writer.SetString("username", ConnectForm.BoxUsername.Text)
		writer.SetString("password", ConnectForm.BoxPassword.Text)
		writer.SetInt32("reconnect", ConnectForm.BoxReconnect.Value)
		writer.SetInt32("timeout", ConnectForm.BoxTimeout.Value)
		writer.SetInt32("alive", ConnectForm.BoxKeepAlive.Value)
		writer.SetInt32("qos", Math.Max(0, ConnectForm.QOSBox.SelectedIndex))
		writer.SetBoolean("clean", ConnectForm.CleanSessionBox.Checked)

		writer.SetInt32("topiccount", ConnectForm.TopicList.Items.Count)

		Dim cnt As Integer = 0
		For Each top As String In ConnectForm.TopicList.Items
			writer.SetString("topic_" & cnt, top)
			cnt += 1
		Next

		Return MyBase.Write(writer)
	End Function

	Dim buf As New List(Of MqttApplicationMessage)

	Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
		If ConnectForm.Client Is Nothing Then Return
		DA.SetData(0, ConnectForm.Client.IsConnected)
		DA.SetData(1, ConnectForm)
		DA.SetData(2, buf)
	End Sub

	Public Sub Refresh()
		Dim action As Action = AddressOf Me.ExpireMe
		Grasshopper.Instances.ActiveCanvas.Invoke(action)
	End Sub

	Private Sub ExpireMe()
		Me.ExpireSolution(True)
	End Sub

	Public Sub AddMessage(Message As MqttApplicationMessage)
		buf.Add(Message)
	End Sub

End Class
