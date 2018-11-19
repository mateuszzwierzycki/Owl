Imports Accord.Neuro
Imports Grasshopper.Kernel
Imports Grasshopper.Kernel.Types

Public Class Param_ActivationNetwork
    Inherits Grasshopper.Kernel.GH_PersistentParam(Of GH_ActivationNetwork)

    Sub New()
        MyBase.New(New GH_InstanceDescription("ActivationNetwork", "AN", "Activation Network", "Owl.Accord", "Params"))
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("{79431F75-3BF4-497E-AAF8-B3336BE177A7}")
        End Get
    End Property

    Protected Overrides Function Prompt_Plural(ByRef values As List(Of GH_ActivationNetwork)) As GH_GetterResult
        Return GH_GetterResult.cancel
    End Function

    Protected Overrides Function Prompt_Singular(ByRef value As GH_ActivationNetwork) As GH_GetterResult
        Return GH_GetterResult.cancel
    End Function

    Protected Overrides ReadOnly Property Icon As System.Drawing.Bitmap
        Get
            Return My.Resources.icon_03
        End Get
    End Property

    Protected Overrides Function PreferredCast(data As Object) As GH_ActivationNetwork

        If TypeOf data Is GH_String Then
            Dim GHC As GH_String = data
            Dim str As New String(GHC.Value)

            If System.IO.File.Exists(str) Then
                Dim nn As ActivationNetwork = ActivationNetwork.Load(str)
                If nn Is Nothing Then Return Nothing
                Return New GH_ActivationNetwork(nn)
            End If
        End If

        Return Nothing

    End Function
End Class
