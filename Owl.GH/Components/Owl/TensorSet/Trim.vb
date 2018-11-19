Imports Rhino.Geometry

Public Class Trim
    Inherits OwlComponentBase

    Sub New()
        MyBase.New("Trim TSet", "TrimTS", "Trim TensorSet values.", SubCategoryTensorSet)
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("{C6A1741A-0D2A-44CE-AC1F-791801BF4BB4}")
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_46
        End Get
    End Property

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.primary
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet)
        pManager.AddIntervalParameter("Range", "R", "Range", GH_ParamAccess.item, New Interval(0, 1))
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim ts As New TensorSet
        Dim pro As New Interval()

        If Not DA.GetData(0, ts) Then Return
        If Not DA.GetData(1, pro) Then Return

        ts.TrimCeiling(pro.Max)
        ts.TrimFloor(pro.Min)

        DA.SetData(0, ts)
    End Sub
End Class
