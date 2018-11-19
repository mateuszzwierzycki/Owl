Public Class Reshape
    Inherits OwlComponentBase

    Sub New()
        MyBase.New("Reshape", "Reshape", "Reshape a Tensor.", SubCategoryTensor)
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("4d183e73-eaa8-48f9-a618-ed2f36891388")
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return icon_34
        End Get
    End Property

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.primary
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_OwlTensor)
        pManager.AddIntegerParameter("Shape", "S", "Shape", GH_ParamAccess.list, 1)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_OwlTensor)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim ts As Tensor = Nothing
        Dim shape As New List(Of Integer)

        If Not DA.GetData(0, ts) Then Return
        If Not DA.GetDataList(1, shape) Then Return

        If Not ts.TryReshape(shape) Then
            Me.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid shape")
        Else
            DA.SetData(0, ts)
        End If
    End Sub

End Class
