Imports System.Drawing
Imports ac = Accord
Imports Accord.Neuro.Learning
Imports Grasshopper.Kernel
Imports Owl.Core.Tensors
Imports Owl.GH.Common
Imports Owl.Accord.Extensions.Learning
Imports Owl.Accord.Extensions.Owl.Accord.Extensions

Public Class EvaluateNetwork
    Inherits GH_Component

    Sub New()
        MyBase.New("Evaluate Network",
                   "EvalNet",
                   "Evaluate the network for error",
                   "Owl.Accord",
                   "Network")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("{DD60666F-CFEC-4166-8317-2B2772E67EBB}")
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.eval_net
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_ActivationNetwork)
        pManager.AddParameter(New Param_OwlTensorSet, "Input", "I", "Input TensorSet", GH_ParamAccess.item)
        pManager.AddParameter(New Param_OwlTensorSet, "Output", "O", "Output TensorSet", GH_ParamAccess.item)
        pManager.AddIntegerParameter("Epoch", "E", "Tensors per epoch", GH_ParamAccess.item, 10)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddNumberParameter("Average error", "E", "Error", GH_ParamAccess.item)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim nn As ActivationNetwork = Nothing
        Dim ins As New GH_OwlTensorSet
        Dim outs As New GH_OwlTensorSet
        Dim ep As Integer

        If Not DA.GetData(0, nn) Then Return
        If Not DA.GetData(1, ins) Then Return
        If Not DA.GetData(2, outs) Then Return
        If Not DA.GetData(3, ep) Then Return

        Dim ane As New ActivationNetworkEvaluation(nn)
        DA.SetData(0, Evaluate(ins.Value, outs.Value, ep, ane))
    End Sub

    Function Evaluate(Ins As TensorSet, Outs As TensorSet, PerEpoch As Integer, BP As ActivationNetworkEvaluation) As Double
        Dim cnt As Integer = Ins.Count
        Dim sum As Double = 0

        For i As Integer = 0 To Ins.Count - 1 Step PerEpoch
            sum += BP.RunEpoch(Ins, Outs, i, Math.Min(i + PerEpoch - 1, Ins.Count - 1))
        Next

        Return sum / cnt
    End Function

End Class
