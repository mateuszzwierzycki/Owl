Imports System.Drawing

Public Class SortByActivations
    Inherits OwlComponentBase

    Sub New()
		MyBase.New("Highest Activation",
				   "High",
				   "Indicates which dimension of the Tensor has the greatest value.",
				   SubCategoryTensor)
	End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("ebdc911e-76f0-403e-9fa4-7442ed735c4e")
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_30
        End Get
    End Property

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.tertiary
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_OwlTensor)
        pManager.AddIntegerParameter("Dimensions", "D", "Number of dimensions to consider", GH_ParamAccess.item, 3)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddIntegerParameter("Indices", "I", "Highest value dimension index", GH_ParamAccess.list)
        pManager.AddNumberParameter("Values", "V", "Values", GH_ParamAccess.list)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim ts As New Tensor
        Dim ds As Integer = 1
        If Not DA.GetData(0, ts) Then Return
        If Not DA.GetData(1, ds) Then Return

        Dim nli As New List(Of Integer)
        Dim nlv As New List(Of Double)

        Dim vals() As Double = ts.ToArray
        Dim ids(vals.Length - 1) As Integer

        For i As Integer = 0 To vals.Length - 1 Step 1
            ids(i) = i
        Next

        Array.Sort(vals, ids)

        For i As Integer = 0 To ds - 1 Step 1
            If ids.Length - i - 1 < 0 Then Exit For
            nli.Add(ids(ids.Length - i - 1))
            nlv.Add(vals(vals.Length - i - 1))
        Next

        DA.SetDataList(0, nli)
        DA.SetDataList(1, nlv)

    End Sub
End Class
