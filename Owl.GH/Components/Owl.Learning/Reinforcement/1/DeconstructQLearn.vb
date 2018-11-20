Public Class DeconstructQAgent
    Inherits GH_Component

    Sub New()
		MyBase.New("Deconstruct QAgent", "DeQAgent", "Deconstruct QAgent", "Owl.Learning", "Reinforcement")
	End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("{33D6505D-C355-4409-83A4-E9015FE691FB}")
        End Get
    End Property

	Protected Overrides ReadOnly Property Icon As Bitmap
		Get
			Return My.Resources.Icons_new_15
		End Get
	End Property

	Public Overrides ReadOnly Property Exposure As GH_Exposure
		Get
			Return GH_Exposure.primary
		End Get
	End Property

	Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_OwlQAgent)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_OwlTensor(), "QMatrix", "Q", "QMatrix", GH_ParamAccess.item)
        pManager.AddNumberParameter("Alpha", "A", "Alpha, learning rate.", GH_ParamAccess.item)
        pManager.AddNumberParameter("Gamma", "G", "Gamma, discount factor.", GH_ParamAccess.item)
        pManager.AddIntegerParameter("Seed", "S", "Randomness seed.", GH_ParamAccess.item)
        pManager.AddIntegerParameter("Count", "C", "Randomness count. A value required to reconstruct the pseudo random generator of the QAgent.", GH_ParamAccess.item)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim qa As QAgent = Nothing
        If Not DA.GetData(0, qa) Then Return

        DA.SetData(0, qa.QMatrix)
        DA.SetData(1, qa.Alpha)
        DA.SetData(2, qa.Gamma)
        DA.SetData(3, qa.RndSeed)
        DA.SetData(4, qa.RndCount)
    End Sub
End Class
