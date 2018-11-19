Imports Grasshopper.Kernel
Imports Owl.GH.Common

Public Class Tensor2DMesh
    Inherits GH_Component

    Public Sub New()
		MyBase.New("Preview T2", "PT2", "Preview 2D Tensor as a mesh", "Owl", "Image")
	End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("{9D0A308C-A095-4BDB-8044-8123D6DF8EBF}")
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_OwlTensor)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddMeshParameter("Mesh", "M", "Mesh", GH_ParamAccess.item)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim tens As GH_OwlTensor = Nothing
        If Not DA.GetData(0, tens) Then Return
        DA.SetData(0, ConvertTensorToMesh(tens.Value))
    End Sub

End Class
