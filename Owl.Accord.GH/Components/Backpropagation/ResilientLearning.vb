Imports System.Drawing
Imports Accord.Neuro
Imports Accord.Neuro.Learning
Imports Grasshopper.Kernel
Imports Owl.Core.Tensors
Imports Owl.GHCommon
Imports Owl.AccordExtensions

Public Class Resilient
    Inherits GH_Component

    Sub New()
        MyBase.New("Resilient", "Resilient", "Teach the Network with resilient backpropagation", "Owl", "Accord.Learning")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("48c0c1bc-5e45-4c50-8081-474156d2b70a")
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return Utils.GetIcon(Me)
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_ActivationNetwork)
        pManager.AddParameter(New Param_OwlTensorSet, "Input", "I", "Input TensorSet", GH_ParamAccess.item)
        pManager.AddParameter(New Param_OwlTensorSet, "Output", "O", "Output TensorSet", GH_ParamAccess.item)
        pManager.AddNumberParameter("Increase", "I", "Increase rate", GH_ParamAccess.item, 1.2)
        pManager.AddNumberParameter("Decrease", "D", "Decrease rate", GH_ParamAccess.item, 0.5)
        pManager.AddIntegerParameter("Epoch", "E", "Tensors per epoch", GH_ParamAccess.item, 10)
        pManager.AddIntegerParameter("Iterations", "N", "Number of iterations", GH_ParamAccess.item, 100)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_ActivationNetwork)
        pManager.AddNumberParameter("Error", "E", "Error", GH_ParamAccess.item)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim nn As ActivationNetwork = Nothing
        Dim ins As New GH_OwlTensorSet
        Dim outs As New GH_OwlTensorSet
        Dim decr As Double
        Dim incr As Double
        Dim ep As Integer
        Dim iter As Integer

        If Not DA.GetData(0, nn) Then Return
        If Not DA.GetData(1, ins) Then Return
        If Not DA.GetData(2, outs) Then Return

        If Not DA.GetData(3, incr) Then Return
        If Not DA.GetData(4, decr) Then Return

        If Not DA.GetData(5, ep) Then Return
        If Not DA.GetData(6, iter) Then Return

        Dim bp As New ResilientBackpropagationLearning(nn)

        DA.SetData(1, Learn(ins.Value, outs.Value, ep, bp, iter))

        DA.SetData(0, nn)
    End Sub

    Function Learn(Ins As TensorSet, Outs As TensorSet, PerEpoch As Integer, BP As ResilientBackpropagationLearning, Times As Integer) As Double
        Dim cnt As Double = Ins.Count
        Dim sum As Double = 0

        For j As Integer = 0 To Times - 1 Step 1
            For i As Integer = 0 To Ins.Count - 1 Step PerEpoch
                sum = BP.RunEpoch(Ins, Outs, i, Math.Min(i + PerEpoch - 1, Ins.Count - 1))
            Next
        Next

        Return sum
    End Function

End Class
