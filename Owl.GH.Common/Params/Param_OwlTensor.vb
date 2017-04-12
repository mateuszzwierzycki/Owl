Imports Grasshopper.Kernel

Public Class Param_OwlTensor
    Inherits Grasshopper.Kernel.GH_PersistentParam(Of GH_OwlTensor)

    Sub New()
        MyBase.New(New GH_InstanceDescription("Tensor", "T", "Owl Tensor", "Owl", "Params"))
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("{3101F182-8420-44D1-AC95-CEBD203DEE4A}")
        End Get
    End Property

    Protected Overrides Function Prompt_Plural(ByRef values As List(Of GH_OwlTensor)) As GH_GetterResult
        Return GH_GetterResult.cancel
    End Function

    Protected Overrides Function Prompt_Singular(ByRef value As GH_OwlTensor) As GH_GetterResult
        Return GH_GetterResult.cancel
    End Function

    Protected Overrides ReadOnly Property Icon As System.Drawing.Bitmap
        Get
            Return My.Resources.icon_01
        End Get
    End Property

End Class
