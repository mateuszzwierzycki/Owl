Public Class ConstructQMatrix
    Inherits GH_Component

    Sub New()
		MyBase.New("Construct QMatrix", "QMatrix", "Construct QMatrix", "Owl.Learning", "Reinforcement")
	End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("{55907EA0-0A01-41E4-B4D8-9FD9082F1D5E}")
        End Get
    End Property

	Protected Overrides ReadOnly Property Icon As Bitmap
		Get
			Return My.Resources.Icons_new_18
		End Get
	End Property

	Public Overrides ReadOnly Property Exposure As GH_Exposure
		Get
			Return GH_Exposure.primary
		End Get
	End Property

	Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddNumberParameter("Actions", "A", "Each list has to have the same amount of actions.", GH_ParamAccess.tree)
        pManager.AddIntegerParameter("Pattern", "P", "Pattern", GH_ParamAccess.list)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_OwlTensor, "QMatrix", "Q", "QMatrix", GH_ParamAccess.item)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim dt As New GH_Structure(Of GH_Number)
        Dim pat As New List(Of Integer)

        If Not DA.GetDataTree(0, dt) Then Return
        If Not DA.GetDataList(1, pat) Then Return

        'check if the right count of actions
        For i As Integer = 0 To dt.Branches.Count - 2
            If dt.Branch(i).Count <> dt.Branch(i + 1).Count Then
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Uneven number of actions")
                Return
            End If
        Next

        Dim tensout As New Tensor(dt.Branch(0).Count, pat.Count)

        For i As Integer = 0 To pat.Count - 1
            Dim thisbranch As List(Of GH_Number) = dt.Branch(pat(i))
            For j As Integer = 0 To thisbranch.Count - 1
                tensout.ValueAt(i, j) = thisbranch(j).Value
            Next
        Next

        DA.SetData(0, tensout)
    End Sub
End Class
