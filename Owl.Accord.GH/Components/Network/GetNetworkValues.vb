Imports System.Drawing
Imports Grasshopper.Kernel
Imports Owl.GH.Common

Public Class GetNetworkValues
	Inherits GH_Component

	Sub New()
		MyBase.New("Extract Values", "GetNet", "Get network weights and biases as Tensors", "Owl.Accord", "Network")
	End Sub

	Public Overrides ReadOnly Property ComponentGuid As Guid
		Get
			Return New Guid("da40f227-d005-4ca3-a684-80639027a995")
		End Get
	End Property

	Protected Overrides ReadOnly Property Icon As Bitmap
		Get
			Return My.Resources.Icons_new_11
		End Get
	End Property

	Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
		pManager.AddParameter(New Param_ActivationNetwork)
	End Sub

	Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
		pManager.AddParameter(New Param_OwlTensorSet, "Weights", "W", "Weight values", GH_ParamAccess.item)
		pManager.AddParameter(New Param_OwlTensorSet, "Thresholds", "T", "Threshold values", GH_ParamAccess.item)
	End Sub

	Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim nn As ActivationNetwork = Nothing
        If Not DA.GetData(0, nn) Then Return

        DA.SetData(0, Owl.Accord.Extensions.Learning.ExtractWeights(nn))
        DA.SetData(1, Owl.Accord.Extensions.Learning.ExtractThresholds(nn))
    End Sub

End Class
