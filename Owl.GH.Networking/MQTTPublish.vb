Imports System.Net.Mqtt
Imports Grasshopper.Kernel

Public Class MQTTPublish
	Inherits GH_Component

	Sub New()
		MyBase.New("MQTT Publish", "MQTT Publish", "MQTT Publish", "Owl", "I/O")
	End Sub

	Public Overrides ReadOnly Property ComponentGuid As Guid
		Get
			Return New Guid("{ACFC813C-4357-42A4-9495-3FABBF2430F3}")
		End Get
	End Property

	Public Overrides ReadOnly Property Exposure As GH_Exposure
		Get
			Return GH_Exposure.tertiary
		End Get
	End Property

	Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
		pManager.AddGenericParameter("Client", "C", "Client", GH_ParamAccess.item)
		pManager.AddTextParameter("Topic", "T", "Topic", GH_ParamAccess.item)
		pManager.AddTextParameter("Message", "M", "Message", GH_ParamAccess.item)
		pManager.AddIntegerParameter("QoS", "Q", "Quality of Service", GH_ParamAccess.item, 0)
		pManager.AddBooleanParameter("Retain", "R", "Retain flag", GH_ParamAccess.item, False)
		pManager.AddBooleanParameter("Send", "S", "Send message", GH_ParamAccess.item, False)
	End Sub

	Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)

	End Sub

	Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
		Dim mqttform As MQTTConnectForm = Nothing
		Dim top As String = ""
		Dim mes As String = ""
		Dim qos As Integer = 0
		Dim ret As Boolean = False
		Dim snd As Boolean = False

		If Not DA.GetData(0, mqttform) Then Return
		If Not DA.GetData(1, top) Then Return
		If Not DA.GetData(2, mes) Then Return
		If Not DA.GetData(3, qos) Then Return
		If Not DA.GetData(4, ret) Then Return
		If Not DA.GetData(5, snd) Then Return

		If Not snd Then Return

		If mqttform.Client Is Nothing Then Return
		mqttform.Client.PublishAsync(New MqttApplicationMessage(top, Text.Encoding.UTF8.GetBytes(mes)), qos, ret)
	End Sub

End Class
