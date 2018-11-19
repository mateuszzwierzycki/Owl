Public Class TensorCrop
    Inherits OwlComponentBase

    Sub New()
        MyBase.New("Tensor Crop", "Crop", "Tensor Crop.", SubCategoryTensor)
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("ff5d7cfd-3484-41df-9ec7-9b81dfe849da")
        End Get
    End Property

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.tertiary
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_55
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_OwlTensor)
        pManager.AddIntegerParameter("X", "X", "Crop rectangle location X", GH_ParamAccess.item)
        pManager.AddIntegerParameter("Y", "Y", "Crop rectangle location Y", GH_ParamAccess.item)
        pManager.AddIntegerParameter("W", "W", "Crop rectangle width", GH_ParamAccess.item)
        pManager.AddIntegerParameter("H", "H", "Crop rectangle height", GH_ParamAccess.item)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_OwlTensor)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim tens As New GH_OwlTensor
        If Not DA.GetData(0, tens) Then Return
        Dim x As Integer : If Not DA.GetData(1, x) Then Return
        Dim y As Integer : If Not DA.GetData(2, y) Then Return
        Dim w As Integer : If Not DA.GetData(3, w) Then Return
        Dim h As Integer : If Not DA.GetData(4, h) Then Return
        DA.SetData(0, Owl.Core.Images.Crop(tens.Value, x, y, w, h))
    End Sub
End Class
