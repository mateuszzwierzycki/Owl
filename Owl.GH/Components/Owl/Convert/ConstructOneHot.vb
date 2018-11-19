Public Class ConstructOneHot
    Inherits OwlComponentBase

    Sub New()
        MyBase.New("OneHot TensorSet", "OneHot", "Construct a OneHot TensorSet", OwlComponentBase.SubCategoryConvert)
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("5eb0d346-6b72-4ed5-bb08-c94d0599f5aa")
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddIntegerParameter("Hot", "V", "Hot values", GH_ParamAccess.list)
        pManager.AddIntegerParameter("Length", "L", "Tensor length", GH_ParamAccess.item, 3)
        pManager.AddNumberParameter("Hot Value", "H", "Optional hot value", GH_ParamAccess.item, 1)
        pManager.AddNumberParameter("Cold Value", "C", "Optional cold value", GH_ParamAccess.item, 0)
        Me.Params.Input(2).Optional = True
        Me.Params.Input(3).Optional = True
    End Sub

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_56
        End Get
    End Property

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.secondary
        End Get
    End Property

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim hv As New List(Of Integer)
        Dim len As Integer = 3
        Dim h As Double = 1
        Dim c As Double = 0

        If Not DA.GetDataList(0, hv) Then Return
        If Not DA.GetData(1, len) Then Return
        If Not DA.GetData(2, h) Then Return
        If Not DA.GetData(3, c) Then Return

        DA.SetData(0, TensorFactory.OneHotSet(len, hv, c, h))
    End Sub
End Class
