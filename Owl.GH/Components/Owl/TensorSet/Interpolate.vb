Public Class Interpolate
    Inherits OwlComponentBase

    Sub New()
        MyBase.New("Interpolate TSet", "Intrp", "Interpolate a TensorSet.", OwlComponentBase.SubCategoryTensorSet)
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("{1E6A57A0-8A14-4C6D-992C-BF8613375E2F}")
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_40
        End Get
    End Property

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.primary
        End Get
    End Property


    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet)
        pManager.AddIntegerParameter("Samples", "S", "Interpolation samples", GH_ParamAccess.item, 10)
        pManager.AddIntegerParameter("Interpolation", "I", "Interpolation type", GH_ParamAccess.item, 0)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)

        Dim ts As New GH_OwlTensorSet
        Dim d As Integer
        Dim it As Integer

        If Not DA.GetData(0, ts) Then Return
        If Not DA.GetData(1, d) Then Return
        If Not DA.GetData(2, it) Then Return

        Dim nvs As New TensorSet

        For i As Integer = 0 To ts.Value.Count - 1 Step 1
            nvs.Add(TensorFactory.Interpolate1D(ts.Value(i), d, it))
        Next

        DA.SetData(0, nvs)

    End Sub



End Class
