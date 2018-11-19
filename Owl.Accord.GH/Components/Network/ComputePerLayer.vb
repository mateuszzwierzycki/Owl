Imports System.Drawing
Imports Accord.Neuro
Imports Accord.Neuro.Learning
Imports Grasshopper.Kernel
Imports Owl.Accord.Extensions.Learning
Imports Owl.Core.Tensors
Imports Owl.GH.Common

Public Class ComputePerLayers
    Inherits GH_Component

    Sub New()
		MyBase.New("Layer Compute",
				   "ComputeL",
				   "Compute the output values for each layer, given the input Tensor",
				   "Owl.Accord",
				   "Network")
	End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("100c4351-5e07-409c-b147-fc0c29ae54f7")
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_57
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_ActivationNetwork)
        pManager.AddParameter(New Param_OwlTensor, "Input", "I", "Input Tensor", GH_ParamAccess.item)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_OwlTensor, "Outputs", "O", "Layer Tensors", GH_ParamAccess.list)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim nn As ActivationNetwork = Nothing
        Dim ins As New GH_OwlTensor

        If Not DA.GetData(0, nn) Then Return
        If Not DA.GetData(1, ins) Then Return

        DA.SetDataList(0, nn.ComputePerLayer(ins.Value))
    End Sub

End Class
