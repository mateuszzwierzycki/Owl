Imports Grasshopper.GUI
Imports Grasshopper.GUI.Canvas
Imports Grasshopper.Kernel.Attributes

Public Class MQTTConnectAtts
	Inherits GH_ComponentAttributes

	Dim typedowner As MQTTConnectComponent = Nothing

	Public Sub New(owner As MQTTConnectComponent)
		MyBase.New(owner)
		typedowner = owner
	End Sub

	Public Overrides Function RespondToMouseDoubleClick(sender As GH_Canvas, e As GH_CanvasMouseEvent) As GH_ObjectResponse
		typedowner.ConnectForm.Show()

		Return GH_ObjectResponse.Handled
		Return MyBase.RespondToMouseDoubleClick(sender, e)
	End Function

End Class
