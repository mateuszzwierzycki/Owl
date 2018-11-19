Imports Rhino.Geometry

Public Class AddNoise
    Inherits OwlComponentBase

    Sub New()
        MyBase.New("Add Noise", "Noise", "Add Noise to a TensorSet", SubCategoryTensorSet)
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("{42BC89B6-9FB9-4CAF-B112-78DABADEACD0}")
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_38
        End Get
    End Property

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.primary
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet)
        pManager.AddNumberParameter("Amplitude", "A", "Noise amplitude", GH_ParamAccess.item, 0.1)
        pManager.AddIntegerParameter("Seed", "S", "Noise seed", GH_ParamAccess.item, 123)
        pManager.AddIntervalParameter("Constrain", "C", "Constrain the TensorSet to this interval", GH_ParamAccess.item, New Interval(0, 1))
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim ts As New TensorSet
        Dim pro As New Interval()
        Dim amp As Double
        Dim sed As Integer

        If Not DA.GetData(0, ts) Then Return
        If Not DA.GetData(1, amp) Then Return
        If Not DA.GetData(2, sed) Then Return
        If Not DA.GetData(3, pro) Then Return

        ts.AddNoise(amp, sed)
        ts.TrimCeiling(pro.Max)
        ts.TrimFloor(pro.Min)

        DA.SetData(0, ts)
    End Sub
End Class
