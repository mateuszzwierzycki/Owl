Imports System.Drawing
Imports Accord.Neuro
Imports Grasshopper.Kernel
Imports Grasshopper.Kernel.Parameters
Imports System.IO
Imports Owl.GH.Common
Imports Owl

Public Class SaveNetwork
    Inherits GH_Component

    Sub New()
		MyBase.New("Save Network", "Network", "Save Activation Network", "Owl.Accord", "Primitive")
	End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("{59E546B4-00D7-436D-85C5-CF6DFD12C1D4}")
        End Get
    End Property

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.secondary
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_17
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_ActivationNetwork)
        pManager.AddTextParameter("File Path", "F", "File path", GH_ParamAccess.item)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_Trigger)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim nn As ActivationNetwork = Nothing
        Dim fp As String = ""

        If Not DA.GetData(0, nn) Then Return
        If Not DA.GetData(1, fp) Then Return

        Dim dir As String = Path.GetDirectoryName(fp)

        If Not Directory.Exists(dir) Then Directory.CreateDirectory(dir)

        nn.Save(fp)

        DA.SetData(0, New Trigger(DateTime.Now, Me.InstanceGuid, "Save File"))
    End Sub
End Class
