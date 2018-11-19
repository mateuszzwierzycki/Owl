Imports System.Drawing

Public Class ClassesSet
    Inherits OwlComponentBase

    Sub New()
        MyBase.New("Classes", "Classes", "Generate classification TensorSet.", SubCategoryTensorSet)
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("7c3b8a67-35a1-4db0-b663-8c470c4b0e9c")
        End Get
    End Property

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.hidden
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddIntegerParameter("Values", "V", "Values", GH_ParamAccess.list)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim nl As New List(Of Integer)
        If Not DA.GetDataList(0, nl) Then Return

        Dim ts As New TensorSet()

        For i As Integer = 0 To nl.Count - 1 Step 1
            For j As Integer = 0 To nl(i) - 1 Step 1
                Dim thisv As New Tensor(nl.Count)
                thisv(i) = 1
                ts.Add(thisv)
            Next
        Next

        DA.SetData(0, ts)
    End Sub

End Class
