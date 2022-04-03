Imports System.Drawing
Imports ac = Accord
Imports Accord.Neuro.Learning
Imports Grasshopper.Kernel
Imports Owl.Core.Tensors
Imports Owl.GH.Common
Imports Owl.Accord.Extensions.Learning

Public Class BackpropLearning
    Inherits GH_Component

    Sub New()
		MyBase.New("Backpropagation",
				   "BackProp",
				   "Teach the Network with backpropagation",
				   "Owl.Accord",
				   "Backprop")
	End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("{C46B6642-DB8C-409E-BE27-798A6C07DD5A}")
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_13
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_ActivationNetwork)
        pManager.AddParameter(New Param_OwlTensorSet, "Input", "I", "Input TensorSet", GH_ParamAccess.item)
        pManager.AddParameter(New Param_OwlTensorSet, "Output", "O", "Output TensorSet", GH_ParamAccess.item)
        pManager.AddNumberParameter("Learning Rate", "L", "Learning rate", GH_ParamAccess.item, 0.01)
        pManager.AddNumberParameter("Momentum", "M", "Momentum", GH_ParamAccess.item, 0.001)
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
        Dim rate As Double
        Dim mome As Double
        Dim ep As Integer
        Dim iter As Integer

        If Not DA.GetData(0, nn) Then Return
        If Not DA.GetData(1, ins) Then Return
        If Not DA.GetData(2, outs) Then Return
        If Not DA.GetData(3, rate) Then Return
        If Not DA.GetData(4, mome) Then Return
        If Not DA.GetData(5, ep) Then Return
        If Not DA.GetData(6, iter) Then Return

        Dim bp As New BackPropagationLearning(nn) With {
            .LearningRate = rate,
            .Momentum = mome
        }

        DA.SetData(0, nn)
        DA.SetData(1, Learn(ins.Value, outs.Value, ep, bp, iter))
    End Sub

    Function Learn(Ins As TensorSet, Outs As TensorSet, PerEpoch As Integer, BP As BackPropagationLearning, Times As Integer) As Double
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
