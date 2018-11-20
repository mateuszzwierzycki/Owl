Imports System.Drawing
Imports Grasshopper.Kernel
Imports Owl.Core.Tensors
Imports Owl.GH.Common

Public Class WebcamFrameComponent
	Inherits GH_Component

	Sub New()
		MyBase.New("WebCamCapture",
				   "Capture",
				   "Capture a single frame from a webcam",
				   "Owl.Accord",
				   OwlComponentBase.SubCategoryDisplay)
	End Sub

	Public Overrides ReadOnly Property ComponentGuid As Guid
		Get
			Return New Guid("bf279f15-f375-4936-bcd5-c7780c5d94c6")
		End Get
	End Property

	Protected Overrides ReadOnly Property Icon As Bitmap
		Get
			Return My.Resources.Icons_new_08
		End Get
	End Property

	Public Overrides ReadOnly Property Exposure As GH_Exposure
		Get
			Return GH_Exposure.tertiary
		End Get
	End Property

	Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
		pManager.AddBooleanParameter("On", "O", "Turn On/Off the webcam.", GH_ParamAccess.item, False)
		pManager.AddBooleanParameter("Capture", "C", "Capture", GH_ParamAccess.item, False)
		pManager.AddIntegerParameter("Device", "D", "Device index", GH_ParamAccess.item, 0)
		pManager.AddIntegerParameter("Resolution", "R", "Resolution index, -1=highest found", GH_ParamAccess.item, -1)
	End Sub

	Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
		pManager.AddParameter(New Param_OwlTensor)
	End Sub

	Private cap As New Extensions.Video.RunningCapture()
	Private tens As Tensor = Nothing

	Public Overrides Sub RemovedFromDocument(document As GH_Document)
		MyBase.RemovedFromDocument(document)
		If cap IsNot Nothing Then cap.CameraTurnOff()
	End Sub

	Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
		Dim run = False
		Dim doit = False
		Dim dev As Integer = 0
		Dim res As Integer = -1

		If Not DA.GetData(0, run) Then Return
		DA.GetData(1, doit)
		DA.GetData(2, dev)
		DA.GetData(3, res)

		If run Then
			cap.ListDevices()
			cap.CameraTurnOn(cap.Devices(dev), res)
			If doit Then cap.TakePicture()
		Else
			cap.CameraTurnOff()
		End If

		If cap.LastCapture IsNot Nothing Then tens = Owl.Core.Images.FromGrayscale(cap.LastCapture)
		If tens IsNot Nothing Then DA.SetData(0, tens)
	End Sub
End Class

