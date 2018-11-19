Imports System.Drawing
Imports Accord.Neuro
Imports Grasshopper.Kernel
Imports Grasshopper.Kernel.Parameters
Imports Owl

Public Class LoadNetwork
    Inherits GH_Component

    Sub New()
		MyBase.New("Load Network", "Network", "Load Activation Network", "Owl.Accord", "Primitive")
	End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("{E09D05DC-0449-4EE4-B739-ED7917E099CB}")
        End Get
    End Property

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.secondary
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_16
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_FilePath, "File Path", "F", "File path", GH_ParamAccess.item)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_ActivationNetwork)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim str As New String("")
        If Not DA.GetData(0, str) Then Return
        DA.SetData(0, ActivationNetwork.Load(str))
    End Sub
End Class
