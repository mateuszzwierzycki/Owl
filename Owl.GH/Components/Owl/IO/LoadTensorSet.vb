Public Class LoadTensorSet
    Inherits OwlComponentBase

    Sub New()
        MyBase.New("Load TensorSet", "LoadTSet", "Loads the TensorSet from a file.", SubCategoryIO)
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("fb10ed92-2226-4a8b-b996-4081b0760af5")
        End Get
    End Property

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.primary
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_29
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_OwlLoad)
        pManager.AddIntegerParameter("Position", "P", "Tensor from which to start loading", GH_ParamAccess.item, 0)
        pManager.AddIntegerParameter("Count", "C", "Number of Tensors to load, -1 to load all", GH_ParamAccess.item, -1)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim dir As GH_String = Nothing
        If Not DA.GetData(0, dir) Then Return

        Dim pos As Integer
        Dim len As Integer = -1

        If Not DA.GetData(1, pos) Then Return
        If Not DA.GetData(2, len) Then Return

        Try
            If pos >= 0 And len > 0 Then
                Dim ts As New TensorSet(dir.Value, pos, len)
                DA.SetData(0, ts)
            Else
                Dim ts As New TensorSet(dir.Value)
                DA.SetData(0, ts)
            End If

        Catch ex As Exception
            Me.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, ex.Message)
        End Try

    End Sub

End Class
