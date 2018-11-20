Public Class ConstructNetwork
	Inherits GH_Component

	Sub New()
		MyBase.New("Deconstruct Network",
				   "DeNetwork",
				   "Deconstruct Owl.Learning Network",
				   "Owl.Learning",
				   "Supervised")
	End Sub

	Public Overrides ReadOnly Property ComponentGuid As Guid
		Get
			Return New Guid("{B72893D6-26A9-482F-A9F2-DCB03CD52BE4}")
		End Get
	End Property

	Protected Overrides ReadOnly Property icon As Bitmap
		Get
			Return My.Resources.Icons_new_21
		End Get
	End Property

	Public Overrides ReadOnly Property Exposure As GH_Exposure
		Get
			Return GH_Exposure.primary
		End Get
	End Property

	Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
		pManager.AddParameter(New Param_OwlNetwork)
	End Sub

	Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
		pManager.AddParameter(New Param_OwlTensorSet, "Weights", "W", "Weights", GH_ParamAccess.item)
		pManager.AddParameter(New Param_OwlTensorSet, "Biases", "B", "Biases", GH_ParamAccess.item)
		pManager.AddIntegerParameter("Activation", "F(x)", "Activation function per layer. 0:Linear 1:Relu 2:Sigmoid 3:Tanh", GH_ParamAccess.list)
	End Sub

	Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
		Dim nn As Network = Nothing
		If Not DA.GetData(0, nn) Then Return

		Dim lay As New List(Of Integer)

		For Each f As NeuronFunctionBase In nn.NeuronFunctions
			Select Case f.GetType
				Case GetType(Linear)
					lay.Add(0)
				Case GetType(Relu)
					lay.Add(1)
				Case GetType(Sigmoid)
					lay.Add(2)
				Case GetType(Tanh)
					lay.Add(3)
			End Select
		Next

		DA.SetData(0, nn.Weights)
		DA.SetData(1, nn.Biases)
		DA.SetDataList(2, lay)
	End Sub
End Class
