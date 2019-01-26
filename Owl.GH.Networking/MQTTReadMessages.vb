Imports System.Net.Mqtt
Imports Grasshopper.Kernel

Public Class MQTTReadMessages
	Inherits GH_Component

	Sub New()
		MyBase.New("MQTT Read", "MQTT Read", "MQTT Read", "Owl", "I/O")
	End Sub

	Public Overrides ReadOnly Property Exposure As GH_Exposure
		Get
			Return GH_Exposure.tertiary
		End Get
	End Property

	Public Overrides ReadOnly Property ComponentGuid As Guid
		Get
			Return New Guid("{0AADA465-B87F-4B61-9320-DE9D7A32E7BC}")
		End Get
	End Property

	Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
		pManager.AddGenericParameter("Buffer", "B", "Message buffer", GH_ParamAccess.item)
	End Sub

	Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
		pManager.AddTextParameter("Topics", "T", "Topics", GH_ParamAccess.list)
		pManager.AddTextParameter("Messages", "M", "Messages", GH_ParamAccess.list)
	End Sub

	Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
		Dim buf As List(Of MqttApplicationMessage) = Nothing
		If Not DA.GetData(0, buf) Then Return

		Dim mess As New List(Of String)
		Dim tops As New List(Of String)

		For i As Integer = 0 To buf.Count - 1
			mess.Add(Text.Encoding.UTF8.GetString(buf(i).Payload))
			tops.Add(buf(i).Topic)
		Next

		buf.Clear()

		DA.SetDataList(0, tops)
		DA.SetDataList(1, mess)
	End Sub

End Class
