Imports Rhino.Geometry

Public Class ConstructOwlNetwork
    Inherits GH_Component

    Sub New()
		MyBase.New("Construct Network",
				   "Network",
				   "Construct Owl.Learning Network",
				   "Owl.Learning",
				   "Supervised")
	End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("{760DCC99-2B78-4B00-806C-FA9C462A0F7A}")
        End Get
    End Property

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.primary
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddIntegerParameter("Layers", "L", "Neurons per layer, first number = Inputs", GH_ParamAccess.list)
        pManager.AddIntegerParameter("Activation", "F(x)", "Activation function per layer. 0:Linear 1:Relu 2:Sigmoid 3:Tanh", GH_ParamAccess.list)
        pManager.AddIntervalParameter("Range", "R", "Initializer range", GH_ParamAccess.item, New Interval(0, 1))
        pManager.AddIntegerParameter("Seed", "S", "Random seed", GH_ParamAccess.item, 123)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_OwlNetwork)
    End Sub

    'Protected Overrides ReadOnly Property Icon As Bitmap
    '    Get
    '        Return My.Resources.icon_07
    '    End Get
    'End Property

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim l As New List(Of Integer)
        Dim f As New List(Of Integer)

        Dim rng As Interval
        Dim seed As Integer = 123

        If Not DA.GetDataList(0, l) Then Return
        If Not DA.GetDataList(1, f) Then Return
        If Not DA.GetData(2, rng) Then Return
        If Not DA.GetData(3, seed) Then Return

        Dim lays As New List(Of Integer)(l)
        lays.RemoveAt(0)

        If f.Count = 1 Then
            For i As Integer = 1 To lays.Count - 1 Step 1
                f.Add(f(0))
            Next
        End If

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

        Dim nn As New Network(fs, l(0), lays, New RandomInitializer(Range.Create(rng.T0, rng.T1), seed))

        DA.SetData(0, nn)
    End Sub
End Class
