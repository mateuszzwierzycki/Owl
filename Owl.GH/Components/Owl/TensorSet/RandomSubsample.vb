Public Class RandomSubsample
    Inherits OwlComponentBase

    Sub New()
        MyBase.New("Subsample TSet", "Subsample", "Subsample TensorSet.", SubCategoryTensorSet)
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("{0C27AD0E-B27D-4232-BA69-109CFF464B32}")
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_43
        End Get
    End Property

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.primary
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet)
        pManager.AddNumberParameter("Amount", "A", "Amount", GH_ParamAccess.item, 0.5)
        pManager.AddIntegerParameter("Seed", "S", "Seed", GH_ParamAccess.item, 123)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim ts As New TensorSet
        Dim am As Double
        Dim se As Integer

        If Not DA.GetData(0, ts) Then Return
        If Not DA.GetData(1, am) Then Return
        If Not DA.GetData(2, se) Then Return

        Dim rnd As New Random(se)

        Dim ovs As New TensorSet

        For i As Integer = 0 To ts.Count - 1 Step 1
            If rnd.NextDouble + am >= 1 Then
                ovs.Add(ts(i))
            End If
        Next

        DA.SetData(0, ovs)
    End Sub
End Class
