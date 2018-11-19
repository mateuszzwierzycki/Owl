Public Class Threshold
	Inherits GH_Component

	Sub New()
		MyBase.New("Threshold",
				   "T",
				   "Apply threshold filter on the Tensor",
				   "Owl",
				   "Image")
	End Sub

	Public Overrides ReadOnly Property ComponentGuid As Guid
		Get
			Return New Guid("{7C0A7B47-4BBF-4E8B-A2FE-0A2C0707AAB4}")
		End Get
	End Property

	Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
		pManager.AddParameter(New Param_OwlTensor)
		pManager.AddNumberParameter("Level", "L", "Threshold level", GH_ParamAccess.item, -1)
		pManager.AddNumberParameter("Tolerance", "T", "Threshold tolerance", GH_ParamAccess.item, 0.1)
	End Sub

	Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
		pManager.AddParameter(New Param_OwlTensor)
	End Sub

	Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
		Dim tens As Tensor = Nothing
		Dim level As Double = 0
		Dim toler As Double = 0
		Dim aver As Double = 0

		If Not DA.GetData(0, tens) Then Return
		If Not DA.GetData(1, level) Then Return
		If Not DA.GetData(2, toler) Then Return

		Dim sourcerange As Range = tens.GetRange
		tens.Remap(Range.Create(0, 1))

		If level = -1 Then
			aver = tens.Average
		Else
			aver = level
		End If

		tens.Remap(Range.Create(aver - toler, aver + toler), Range.Create(0, 1))

		tens.TrimCeiling(1)
		tens.TrimFloor(0)

		DA.SetData(0, New GH_OwlTensor(tens))
	End Sub




End Class
