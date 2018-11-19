Imports System.Drawing
Imports Grasshopper.Kernel
Imports Owl.Tensors

Public Class ConstructTensor2DSet
    Inherits GH_Component

    Sub New()
        MyBase.New("Construct Tensor2DSet", "Tensor2DSet", "Construct Owl Tensor2DSet", "Owl", "Primitive")
    End Sub

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return Utils.GetIcon(Me)
        End Get
    End Property

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("1df1c5ee-ddc5-48f6-be41-2c43bf3e4fdc")
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_OwlTensor2D, "Tensors", "T2", "2D Tensors", GH_ParamAccess.list)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_OwlTensor2DSet)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim l As New List(Of TensorD)
        If Not DA.GetDataList(0, l) Then Return
        DA.SetData(0, New Tensor2DSet(l))
    End Sub
End Class
