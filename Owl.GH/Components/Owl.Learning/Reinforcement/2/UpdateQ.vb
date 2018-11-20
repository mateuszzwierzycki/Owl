Public Class UpdateQ
    Inherits GH_Component

    Sub New()
		MyBase.New("UpdateQ", "UpdateQ", "Update QAgent values", "Owl.Learning", "Reinforcement")
	End Sub

	Public Overrides ReadOnly Property ComponentGuid As Guid
		Get
			Return New Guid("{3FA90223-7846-413B-9E87-36B3B0258908}")
		End Get
	End Property

	Protected Overrides ReadOnly Property Icon As Bitmap
		Get
			Return My.Resources.Icons_new_17
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
        pManager.AddIntegerParameter("Action", "A", "Performed action", GH_ParamAccess.item)
        pManager.AddIntegerParameter("Next", "N", "Next state", GH_ParamAccess.item)
        pManager.AddNumberParameter("Reward", "R", "S|A Reward", GH_ParamAccess.item)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_OwlQAgent)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim nql As QAgent = Nothing
        If Not DA.GetData(0, nql) Then Return

        Dim state As Integer
        Dim action As Integer
        Dim nstate As Integer
        Dim reward As Double

        If Not DA.GetData(1, state) Then Return
        If Not DA.GetData(2, action) Then Return
        If Not DA.GetData(3, nstate) Then Return
        If Not DA.GetData(4, reward) Then Return

        nql.UpdateQ(state, action, nstate, reward)

        DA.SetData(0, nql)
    End Sub
End Class
