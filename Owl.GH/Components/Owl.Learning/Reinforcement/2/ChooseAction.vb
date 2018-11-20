Public Class ChooseAction
    Inherits GH_Component

    Sub New()
		MyBase.New("Choose Action", "Action", "Choose agent next action", "Owl.Learning", "Reinforcement")
	End Sub

	Public Overrides ReadOnly Property ComponentGuid As Guid
		Get
			Return New Guid("{7B38BC76-BD51-48F2-B122-84FB91079021}")
		End Get
	End Property

	Protected Overrides ReadOnly Property Icon As Bitmap
		Get
			Return My.Resources.Icons_new_14
		End Get
	End Property

	Public Overrides ReadOnly Property Exposure As GH_Exposure
		Get
			Return GH_Exposure.secondary
		End Get
	End Property

	Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_OwlQAgent)
        pManager.AddIntegerParameter("State", "S", "Current state", GH_ParamAccess.item)
        pManager.AddNumberParameter("Epsilon", "E", "Epsilon", GH_ParamAccess.item, 0.1)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_OwlQAgent)
        pManager.AddIntegerParameter("Action", "A", "Chosen action", GH_ParamAccess.item)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim nql As QAgent = Nothing
        If Not DA.GetData(0, nql) Then Return

        Dim state As Integer = -1
        If Not DA.GetData(1, state) Then Return

        Dim eps As Double = 0
        If Not DA.GetData(2, eps) Then Return

        DA.SetData(1, nql.ChooseAction(state, eps))
        DA.SetData(0, nql)
    End Sub

End Class
