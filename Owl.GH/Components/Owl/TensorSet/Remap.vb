Imports Rhino.Geometry

Public Class Remap
    Inherits OwlComponentBase

    Sub New()
        MyBase.New("Remap TSet", "Remap", "Remap TensorSet.", SubCategoryTensorSet)
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("{44300F78-EA02-4626-9D77-108BC17F033B}")
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_47
        End Get
    End Property

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.primary
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet)
        pManager.AddIntervalParameter("Source", "S", "Source domain", GH_ParamAccess.item, New Interval(0, 1))
        pManager.AddIntervalParameter("Target", "T", "Target domain", GH_ParamAccess.item, New Interval(0, 1))
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim ts As New TensorSet
        Dim sourced As Interval
        Dim targetd As Interval

        If Not DA.GetData(0, ts) Then Return
        If Not DA.GetData(1, sourced) Then Return
        If Not DA.GetData(2, targetd) Then Return

        ts.Remap(New Range(sourced.T0, sourced.T1), New Range(targetd.T0, targetd.T1))

        DA.SetData(0, ts)
    End Sub
End Class
