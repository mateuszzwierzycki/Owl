Public Class ConstructOwlNetwork_Direct
	Inherits GH_Component

	Sub New()
		MyBase.New("Construct Network Ex",
				   "NetworkEx",
				   "Construct Owl.Learning Network from the atomic data." + vbCrLf +
				   "Use this component when importing already trained models from other frameworks.",
				   "Owl.Learning",
				   "Supervised")
	End Sub

	Public Overrides ReadOnly Property ComponentGuid As Guid
		Get
			Return New Guid("{29977F8E-492F-431D-B308-2A45AFF3B3F8}")
		End Get
	End Property

	Protected Overrides ReadOnly Property Icon As Bitmap
		Get
			Return My.Resources.Icons_new_22
		End Get
	End Property

	Public Overrides ReadOnly Property Exposure As GH_Exposure
		Get
			Return GH_Exposure.primary
		End Get
	End Property

	Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
		pManager.AddParameter(New Param_OwlTensorSet, "Weights", "W", "Weights", GH_ParamAccess.item)
		pManager.AddParameter(New Param_OwlTensorSet, "Biases", "B", "Biases", GH_ParamAccess.item)
		pManager.AddIntegerParameter("Activation", "F(x)", "Activation function per layer. 0:Linear 1:Relu 2:Sigmoid 3:Tanh", GH_ParamAccess.list)
	End Sub

	Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
		pManager.AddParameter(New Param_OwlNetwork)
	End Sub

	Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
		Dim w As New TensorSet
		Dim b As New TensorSet
		Dim f As New List(Of Integer)

		If Not DA.GetData(0, w) Then Return
		If Not DA.GetData(1, b) Then Return
		If Not DA.GetDataList(2, f) Then Return

		Dim fs As New List(Of NeuronFunctionBase)

		For Each val As Integer In f
			Select Case val
				Case 0
					fs.Add(New Linear)
				Case 1
					fs.Add(New Relu)
				Case 2
					fs.Add(New Sigmoid)
				Case 3
					fs.Add(New Tanh)
			End Select
		Next

		Dim nn As New Network(w, b, fs)
		DA.SetData(0, New GH_OwlNetwork(nn))
	End Sub
End Class
