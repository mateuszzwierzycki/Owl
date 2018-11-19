Imports System.Drawing
Imports Accord.Neuro
Imports Grasshopper.Kernel
Imports Owl

Public Class ConstructNetwork
    Inherits GH_Component

    Sub New()
        MyBase.New("Construct Network", "Network", "Construct Owl.Accord Network", "Owl.Accord", "Primitive")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("{E4FF1B57-7D1F-4472-A2FF-A674D8EC1412}")
        End Get
    End Property

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.quarternary
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddIntegerParameter("Layers", "L", "Neurons per layer, first number = Inputs", GH_ParamAccess.list)
        pManager.AddIntegerParameter("Activation", "F(x)", "Activation function", GH_ParamAccess.item, 0)

        Dim pin As Parameters.Param_Integer = Me.Params.Input(1)
        pin.AddNamedValue("Sigmoid", 0)
        pin.AddNamedValue("Bipolar Sigmoid", 1)
        pin.AddNamedValue("Threshold", 2)

        pManager.AddNumberParameter("Alpha", "A", "Alpha value, required with sigmoids", GH_ParamAccess.item, 2)
        pManager.AddIntegerParameter("Seed", "S", "Random seed", GH_ParamAccess.item, 123)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_ActivationNetwork)
    End Sub

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_07
        End Get
    End Property

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim l As New List(Of Integer)
        Dim f As Integer = 0
        Dim a As Double = 0
        Dim r As Integer = 123

        If Not DA.GetDataList(0, l) Then Return
        If Not DA.GetData(1, f) Then Return
        If Not DA.GetData(2, a) Then Return
        If Not DA.GetData(3, r) Then Return

        Dim neurons(l.Count - 2) As Integer

        For i As Integer = 1 To l.Count - 1 Step 1
            neurons(i - 1) = l(i)
        Next

        Dim nn As ActivationNetwork = Nothing

        Select Case f
            Case 0
                nn = New ActivationNetwork(New SigmoidFunction(a), l(0), neurons)
            Case 1
                nn = New ActivationNetwork(New BipolarSigmoidFunction(a), l(0), neurons)
            Case 2
                nn = New ActivationNetwork(New ThresholdFunction, l(0), neurons)
        End Select

        Dim rnd As New Random(r)

        For i As Integer = 0 To nn.Layers.Count - 1 Step 1
            For j As Integer = 0 To nn.Layers(i).Neurons.Count - 1 Step 1
                Dim actn As ActivationNeuron = nn.Layers(i).Neurons(j)
                actn.Threshold = rnd.NextDouble
                For k As Integer = 0 To nn.Layers(i).Neurons(j).Weights.Count - 1 Step 1
                    nn.Layers(i).Neurons(j).Weights(k) = rnd.NextDouble
                Next
            Next
        Next

        DA.SetData(0, nn)
    End Sub
End Class
