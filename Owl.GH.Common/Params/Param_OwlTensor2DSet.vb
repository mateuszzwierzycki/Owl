Imports Grasshopper.Kernel
Imports Owl.Tensors

Public Class Param_OwlTensor2DSet
    Inherits Grasshopper.Kernel.GH_PersistentParam(Of GH_OwlTensor2dSet)

    Sub New()
        MyBase.New(New GH_InstanceDescription("Tensor2DSet", "T2S", "Owl Tensor2DSet", "Owl", "Params"))
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("a43d902c-0cdf-446c-a880-1d69b4a6be8f")
        End Get
    End Property

    Protected Overrides Function Prompt_Plural(ByRef values As List(Of GH_OwlTensor2DSet)) As GH_GetterResult
        Return GH_GetterResult.cancel
    End Function

    Protected Overrides Function Prompt_Singular(ByRef value As GH_OwlTensor2DSet) As GH_GetterResult
        Return GH_GetterResult.cancel
    End Function

    Protected Overrides ReadOnly Property Icon As System.Drawing.Bitmap
        Get
            Return Utils.GetIcon(Me)
        End Get
    End Property

    Protected Overrides Function PreferredCast(data As Object) As GH_OwlTensor2DSet

        If TypeOf data Is GH_OwlTensor2D Then
            Dim d As GH_OwlTensor2D = data
            Dim v As TensorD = d.Value.Duplicate

            Dim vs As New Tensor2DSet()
            vs.Add(v)

            Return New GH_OwlTensor2DSet(vs)
        End If

        Return Nothing

    End Function

End Class
