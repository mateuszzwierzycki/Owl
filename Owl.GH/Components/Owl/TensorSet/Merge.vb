Public Class Merge
    Inherits OwlComponentBase

    Sub New()
        MyBase.New("Merge TSet", "MergeTS", "Merge multiple TensorSets into one.", SubCategoryTensorSet)
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("{29FB0036-BD37-40E0-8BAB-E2176931B31B}")
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_54
        End Get
    End Property

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.primary
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet, "TensorSets", Param_OwlTensorSet.Nick, "TensorSets", GH_ParamAccess.list)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim l As New List(Of TensorSet)

        If Not DA.GetDataList(0, l) Then Return

        Dim ts As New TensorSet

        For i As Integer = 0 To l.Count - 1 Step 1
            ts.AddRange(l(i))
        Next

        DA.SetData(0, ts)
    End Sub
End Class
