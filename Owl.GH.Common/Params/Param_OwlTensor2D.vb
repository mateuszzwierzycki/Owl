Imports Grasshopper.Kernel

Public Class Param_OwlTensor2D
    Inherits Grasshopper.Kernel.GH_PersistentParam(Of GH_OwlTensor2D)

    Sub New()
        MyBase.New(New GH_InstanceDescription("Tensor2D", "T2", "Owl Tensor 2D", "Owl", "Params"))
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("e001cd2c-d988-4877-825a-340ea89b7871")
        End Get
    End Property

    Protected Overrides Function Prompt_Plural(ByRef values As List(Of GH_OwlTensor2D)) As GH_GetterResult
        Return GH_GetterResult.cancel
    End Function

    Protected Overrides Function Prompt_Singular(ByRef value As GH_OwlTensor2D) As GH_GetterResult
        Return GH_GetterResult.cancel
    End Function

    Protected Overrides ReadOnly Property Icon As System.Drawing.Bitmap
        Get
            Return Utils.GetIcon(Me)
        End Get
    End Property

End Class
