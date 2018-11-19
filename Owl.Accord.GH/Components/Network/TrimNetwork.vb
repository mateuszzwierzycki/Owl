Imports System.Drawing
Imports Grasshopper.Kernel
Imports Owl.Accord.Extensions.Learning

Public Class TrimNetwork
    Inherits GH_Component

    Sub New()
		MyBase.New("Trim Network", "Trim", "Trim the network", "Owl.Accord", "Network")
	End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("{C15E199C-F876-4A19-8008-11823A8B1BBD}")
        End Get
    End Property

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.tertiary
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_18
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_ActivationNetwork)
        pManager.AddIntegerParameter("Trim", "T", "Number of layers to trim", GH_ParamAccess.item, 1)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_ActivationNetwork)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim nn As ActivationNetwork = Nothing
        Dim tr As Integer

        If Not DA.GetData(0, nn) Then Return
        If Not DA.GetData(1, tr) Then Return

        DA.SetData(0, nn.TrimNetwork(tr))
    End Sub

End Class
