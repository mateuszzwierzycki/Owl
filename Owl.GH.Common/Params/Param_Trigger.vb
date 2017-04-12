Imports Grasshopper.Kernel
Imports Grasshopper.Kernel.Types
Imports Owl.GH.Common

Public Class Param_Trigger
    Inherits Grasshopper.Kernel.GH_PersistentParam(Of GH_Trigger)

    Sub New()
        MyBase.New(New GH_InstanceDescription("Trigger", "T", "Trigger, used to inform downstream components about actions.", "Owl", "Params"))
    End Sub

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.hidden
        End Get
    End Property

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("{900F466E-CCAD-4F43-8D21-761DF95414E5}")
        End Get
    End Property

    Protected Overrides Function Prompt_Plural(ByRef values As List(Of GH_Trigger)) As GH_GetterResult
        Return GH_GetterResult.cancel
    End Function

    Protected Overrides Function Prompt_Singular(ByRef value As GH_Trigger) As GH_GetterResult
        Return GH_GetterResult.cancel
    End Function

    Protected Overrides ReadOnly Property Icon As System.Drawing.Bitmap
        Get
            Return My.Resources.icon_52
        End Get
    End Property

    Protected Overrides Function PreferredCast(data As Object) As GH_Trigger
        Return New GH_Trigger(New Trigger(DateTime.Now, Me.InstanceGuid, data.ToString))
    End Function

End Class
