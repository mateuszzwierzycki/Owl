Imports Owl.learning

Public Class KMeansClustering
    Inherits OwlComponentBase

    Sub New()
		MyBase.New("KMeans Clustering",
				   "KMeans",
				   "A KMeans clustering component",
				   "Owl.Learning",
				   "Unsupervised")
	End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("2b4dfc74-9768-411f-ada9-98e2d2f723b2")
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_48
        End Get
    End Property

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.primary
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet, "Data", "D", "Data to cluster", GH_ParamAccess.item)
        pManager.AddIntegerParameter("Clusters", "C", "Number of clusters", GH_ParamAccess.item, 3)
        pManager.AddIntegerParameter("Iterations", "I", "Iterations", GH_ParamAccess.item, 1)
        pManager.AddIntegerParameter("Seed", "S", "Randomness seed", GH_ParamAccess.item, 123)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet, "Clusters", "C", "Data split into clusters", GH_ParamAccess.list)
        pManager.AddIntegerParameter("Indices", "I", "Indices", GH_ParamAccess.tree)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim data As New TensorSet
        Dim clustercount As Integer
        Dim iter As Integer
        Dim seed As Integer = 123

        If Not DA.GetData(3, seed) Then Return

        If Not DA.GetData(0, data) Then Return
        If Not data.IsHomogeneous Then
            Me.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "TensorSet is not homogeneous")
            Exit Sub
        End If

        If Not DA.GetData(1, clustercount) Then Return
        If Not DA.GetData(2, iter) Then Return

		Dim nkm As KMeansEngine = Owl.Learning.Clustering.Run(data, clustercount, iter, Double.Epsilon, seed)

		DA.SetDataList(0, nkm.SplitIntoSets)

        Dim dto As New DataTree(Of Integer)
        Dim lll() As List(Of Integer) = nkm.SplitAsIndices

        Dim thisp As GH_Path = DA.ParameterTargetPath(1)
        thisp.AppendElement(DA.ParameterTargetIndex(0))

        For i As Integer = 0 To lll.Length - 1 Step 1
            dto.AddRange(lll(i), thisp.AppendElement(i))
        Next

        DA.SetDataTree(1, dto)
    End Sub
End Class
