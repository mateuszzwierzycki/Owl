Imports System.Drawing
Imports Rhino.Geometry

Public Class GetBounds
    Inherits OwlComponentBase

    Sub New()
        MyBase.New("Bounds TSet", "Bounds", "Get TensorSet bounds.", SubCategoryTensorSet)
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("{0DE7183A-7482-49AD-9007-68D062A127EA}")
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_39
        End Get
    End Property

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.primary
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet)
        pManager.AddIntegerParameter("Dimension", "D", "Dimension, -1 for all dimensions", GH_ParamAccess.item, -1)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddIntervalParameter("Bounds", "B", "Bounds", GH_ParamAccess.item)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim ts As New GH_OwlTensorSet
        Dim d As Integer

        If Not DA.GetData(0, ts) Then Return
        If Not DA.GetData(1, d) Then Return

        Dim rng As Range = If(d = -1, ts.Value.GetRange, ts.Value.GetRange(d))
        DA.SetData(0, New Interval(rng.From, rng.To))
    End Sub

End Class
