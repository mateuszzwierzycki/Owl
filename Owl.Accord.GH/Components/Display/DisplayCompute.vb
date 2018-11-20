Imports System.Drawing
Imports Grasshopper.Kernel
Imports Owl.Core.Tensors
Imports Owl.Core.Structures
Imports Owl.GH.Common

Public Class DisplayCompute
	Inherits ImageComponentBase

	Sub New()
		MyBase.New("Display Compute",
				   "DComp",
				   "Computes the network output",
				   "Owl.Accord",
				   "Display")
	End Sub

	Protected Overrides ReadOnly Property Icon As Bitmap
		Get
			Return My.Resources.icon_10
		End Get
	End Property

	Public Overrides ReadOnly Property Exposure As GH_Exposure
		Get
			Return GH_Exposure.primary
		End Get
	End Property

	Public Overrides ReadOnly Property ComponentGuid As Guid
		Get
			Return New Guid("207bae14-6adb-4e10-b709-d268621a44a2")
		End Get
	End Property

	Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
		pManager.AddParameter(New Param_ActivationNetwork)
		pManager.AddParameter(New Param_OwlTensorSet, "Input", "I", "Input TensorSet", GH_ParamAccess.item)
		pManager.AddIntegerParameter("Shape", "S", "Output Shape", GH_ParamAccess.list)
		Me.Params.Input(2).Optional = True
	End Sub

	Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
		pManager.AddParameter(New Param_OwlTensorSet, "Output", "O", "Output TensorSet", GH_ParamAccess.item)
	End Sub

	Dim tens As Tensor = Nothing

	Protected Overrides Sub BeforeSolveInstance()
		tens = Nothing
	End Sub

	Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
		Dim nn As ActivationNetwork = Nothing
		Dim ins As New GH_OwlTensorSet
		Dim shp As New List(Of Integer)
		Dim noshape As Boolean = True

		If Not DA.GetData(0, nn) Then Return
		If Not DA.GetData(1, ins) Then Return
		If DA.GetDataList(2, shp) Then noshape = False

		Dim outs As New TensorSet

		For i As Integer = 0 To ins.Value.Count - 1 Step 1
			outs.Add(New Tensor(nn.Compute(ins.Value(i).TensorData)))
		Next

		If outs.Count > 0 Then
			If tens Is Nothing Then
				tens = outs(0).Duplicate
				tens.Remap(New Range(0, 1))
				If noshape = False Then
					If Not tens.TryReshape(shp) Then Me.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Incompatible shape")
				End If

				Me.ChangeSize(tens.Width * 2, If(tens.ShapeCount = 1, 1, tens.Height * 2), ImageAttributes.Resolution)
			End If
		End If

		DA.SetData(0, outs)
	End Sub

	Public Overrides Sub DrawBackground(ByRef BackImage As Bitmap)
		BackImage = Owl.Core.Visualization.PlotFactory.DrawGridBackground(10, 10)
	End Sub

	Public Overrides Sub DrawForeground(ByRef ForeImage As Bitmap)
		If tens IsNot Nothing Then ForeImage = Owl.Core.Visualization.PlotFactory.Tensor2DImage(tens, New Range(0, 0))
	End Sub

	Public Overrides Sub OnAttributesCreation(ByRef atts As ImageComponent_Attributes)
		atts.Resizeable = False
		atts.FrameWidth = 50
		atts.FrameHeight = 50
		atts.TileBackground = True
	End Sub
End Class
