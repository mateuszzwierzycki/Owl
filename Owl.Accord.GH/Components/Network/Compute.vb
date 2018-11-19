Imports System.Drawing
Imports Accord.Neuro
Imports Accord.Neuro.Learning
Imports Grasshopper.Kernel
Imports Owl.Core.Tensors
Imports Owl.GH.Common

Public Class Compute
    Inherits GH_Component

    Sub New()
		MyBase.New("Compute", "Compute", "Compute the output values for the given input TensorSet", "Owl.Accord", "Network")
	End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("{D46570F2-66E2-484F-A92B-3840B59EF153}")
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_15
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_ActivationNetwork)
        pManager.AddParameter(New Param_OwlTensorSet, "Input", "I", "Input TensorSet", GH_ParamAccess.item)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet, "Output", "O", "Output TensorSet", GH_ParamAccess.item)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim nn As ActivationNetwork = Nothing
        Dim ins As New GH_OwlTensorSet

        If Not DA.GetData(0, nn) Then Return
        If Not DA.GetData(1, ins) Then Return

        Dim outs As New TensorSet

        For i As Integer = 0 To ins.Value.Count - 1 Step 1
            outs.Add(New Tensor(nn.Compute(ins.Value(i).TensorData)))
        Next

        DA.SetData(0, outs)
    End Sub

End Class
