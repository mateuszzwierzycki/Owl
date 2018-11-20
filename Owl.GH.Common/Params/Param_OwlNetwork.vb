Imports Grasshopper.Kernel

Public Class Param_OwlNetwork
    Inherits Grasshopper.Kernel.GH_PersistentParam(Of GH_OwlNetwork)

    Sub New()
        MyBase.New(New GH_InstanceDescription("Owl.Learning.Network", "N", "Owl.Learning.Network", "Owl.Learning", "Params"))
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("{9F28C691-9AA6-4CBB-9551-DF3B296ADB43}")
        End Get
    End Property

    Protected Overrides Function Prompt_Plural(ByRef values As List(Of GH_OwlNetwork)) As GH_GetterResult
        Return GH_GetterResult.cancel
    End Function

    Protected Overrides Function Prompt_Singular(ByRef value As GH_OwlNetwork) As GH_GetterResult
        Return GH_GetterResult.cancel
    End Function

	Protected Overrides ReadOnly Property Icon As System.Drawing.Bitmap
		Get
			Return My.Resources.Icons_new_12
		End Get
	End Property

End Class
