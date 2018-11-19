Imports System.Drawing
Imports Grasshopper.Kernel
Imports Owl.Tensors
Imports OwlGHCommon

Public Class ConstructTensor2D
    Inherits GH_Component

    Sub New()
        MyBase.New("Construct Tensor2D", "Tensor2D", "Construct Owl Tensor2D", "Owl", "Primitive")
    End Sub

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return Utils.GetIcon(Me)
        End Get
    End Property

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("8cb723e5-a79f-48e8-b3fd-bb14cb979cc6")
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddNumberParameter("Values", "V", "Values", GH_ParamAccess.list)
        pManager.AddIntegerParameter("Width", "W", "Width", GH_ParamAccess.item, 1)
        pManager.AddIntegerParameter("Height", "H", "Height", GH_ParamAccess.item, 1)
        Me.Params.Input(0).Optional = True
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_OwlTensor2D)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim t2 As TensorD = Nothing

        Dim l As New List(Of Double)
        Dim w As Integer = 0
        Dim h As Integer = 0
        If Not DA.GetData(1, w) Then Return
        If Not DA.GetData(2, h) Then Return

        If Not DA.GetDataList(0, l) Then
            t2 = New TensorD(w, h)
        Else
            t2 = New TensorD(w, h, l)
        End If

        DA.SetData(0, New TensorD(w, h, l))
    End Sub
End Class
