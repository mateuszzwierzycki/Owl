Imports Grasshopper.Kernel
Imports Owl.Core.Tensors

Public Class Param_OwlTensorSet
    Inherits Grasshopper.Kernel.GH_PersistentParam(Of GH_OwlTensorSet)

    Public Shared Nick As String = "TS"

    Sub New()
        MyBase.New(New GH_InstanceDescription("TensorSet", Nick, "Owl TensorSet", "Owl", "Params"))
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("{E7D0C937-2A79-4DDF-BA05-25BE37B1F05C}")
        End Get
    End Property

    Protected Overrides Function Prompt_Plural(ByRef values As List(Of GH_OwlTensorSet)) As GH_GetterResult
        Return GH_GetterResult.cancel
    End Function

    Protected Overrides Function Prompt_Singular(ByRef value As GH_OwlTensorSet) As GH_GetterResult
        Return GH_GetterResult.cancel
    End Function

    Protected Overrides ReadOnly Property Icon As System.Drawing.Bitmap
        Get
            Return My.Resources.icon_02
        End Get
    End Property

    Protected Overrides Function PreferredCast(data As Object) As GH_OwlTensorSet

        If TypeOf data Is GH_OwlTensor Then
            Dim d As GH_OwlTensor = data
            Dim v As Tensor = d.Value.Duplicate

            Dim ts As New TensorSet()
            ts.Add(v)

            Return New GH_OwlTensorSet(ts)
        End If

        Return Nothing

    End Function

End Class
