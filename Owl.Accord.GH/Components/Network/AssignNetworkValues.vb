Imports System.Drawing
Imports Grasshopper.Kernel
Imports Owl.GH.Common

Public Class AssignNetworkValues
	Inherits GH_Component

	Sub New()
		MyBase.New("Assign Values", "SetNet", "Assing network weights and biases directly.", "Owl.Accord", "Network")
	End Sub

	Public Overrides ReadOnly Property ComponentGuid As Guid
		Get
			Return New Guid("dc9eb4f8-aca4-4072-8a44-6c0c27b7c757")
		End Get
	End Property

	Protected Overrides ReadOnly Property Icon As Bitmap
		Get
			Return My.Resources.Icons_new_10
		End Get
	End Property

	Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
		pManager.AddParameter(New Param_ActivationNetwork)
		pManager.AddParameter(New Param_OwlTensorSet, "Weights", "W", "Weight values", GH_ParamAccess.item)
		pManager.AddParameter(New Param_OwlTensorSet, "Thresholds", "T", "Threshold values", GH_ParamAccess.item)

		Me.Params.Input(1).Optional = True
		Me.Params.Input(2).Optional = True
	End Sub

	Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
		pManager.AddParameter(New Param_ActivationNetwork)
	End Sub

	Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim nn As ActivationNetwork = Nothing
        Dim wei As New GH_OwlTensorSet
        Dim tre As New GH_OwlTensorSet

        If Not DA.GetData(0, nn) Then Return
        If DA.GetData(1, wei) Then Owl.Accord.Extensions.Learning.SetWeights(nn, wei.Value)
        If DA.GetData(2, tre) Then Owl.Accord.Extensions.Learning.SetThresholds(nn, tre.Value)

        DA.SetData(0, nn)
    End Sub

End Class
