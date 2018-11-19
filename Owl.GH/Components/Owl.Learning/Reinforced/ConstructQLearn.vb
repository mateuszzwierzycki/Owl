Public Class ConstructQAgent
    Inherits GH_Component

    Sub New()
		MyBase.New("Construct QAgent", "QAgent", "Construct QAgent", "Owl.Learning", "Reinforced")
	End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("{922A6A78-E97D-4121-A3B4-3799F97F7F93}")
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_OwlTensor(), "QMatrix", "Q", "QMatrix", GH_ParamAccess.item)
        pManager.AddNumberParameter("Alpha", "A", "Alpha, learning rate.", GH_ParamAccess.item, 0.2)
        pManager.AddNumberParameter("Gamma", "G", "Gamma, discount factor.", GH_ParamAccess.item, 0.8)
        pManager.AddIntegerParameter("Seed", "S", "Randomness seed.", GH_ParamAccess.item, 123)
        pManager.AddIntegerParameter("Count", "C", "Randomness count. A value required to reconstruct the pseudo random generator of the QAgent.", GH_ParamAccess.item, 0)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_OwlQAgent)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim qm As New Tensor
        Dim alpha As Double
        Dim gamma As Double
        Dim seed As Integer
        Dim cnt As Integer

        If Not DA.GetData(0, qm) Then Return
        If Not DA.GetData(1, alpha) Then Return
        If Not DA.GetData(2, gamma) Then Return
        If Not DA.GetData(3, seed) Then Return
        If Not DA.GetData(4, cnt) Then Return

        DA.SetData(0, New GH_OwlQAgent(New QAgent(qm, gamma, alpha, seed, cnt)))
    End Sub
End Class
