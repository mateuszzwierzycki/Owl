Public Class Compute
    Inherits GH_Component

    Sub New()
		MyBase.New("Compute",
				   "Compute",
				   "Compute the output values for the given input TensorSet",
				   "Owl.Learning",
				   "Supervised")
	End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("{2F75DA58-C7C1-4AE7-AC8A-9B7962259F76}")
        End Get
    End Property

    'TODO icon
    'Protected Overrides ReadOnly Property Icon As Bitmap
    '    Get
    '        Return My.Resources.icon_15
    '    End Get
    'End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_OwlNetwork)
        pManager.AddParameter(New Param_OwlTensorSet, "Input", "I", "Input TensorSet", GH_ParamAccess.item)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet, "Output", "O", "Output TensorSet", GH_ParamAccess.item)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim nn As GH_OwlNetwork = Nothing
        Dim ins As New GH_OwlTensorSet

        If Not DA.GetData(0, nn) Then Return
        If Not DA.GetData(1, ins) Then Return

        Dim outs As TensorSet = nn.Value.ComputeOptimized(ins.Value)

        'For i As Integer = 0 To ins.Value.Count - 1 Step 1
        '    outs.Add(nn.Value.Compute(ins.Value(i)))
        'Next

        DA.SetData(0, outs)
    End Sub

End Class
