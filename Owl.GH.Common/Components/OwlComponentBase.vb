Imports System.Drawing
Imports Grasshopper.Kernel

Public MustInherit Class OwlComponentBase
    Inherits GH_Component

    Sub New(Name As String, Nickname As String, Description As String, SubCategory As String)
        MyBase.New(Name, Nickname, Description, "Owl", SubCategory)
    End Sub

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return Utils.GetIcon(Me)
        End Get
    End Property

    Public Const SubCategoryParam As String = "Param"
    Public Const SubCategoryPrimitive As String = "Primitive"
    Public Const SubCategoryConvert As String = "Convert"
    Public Const SubCategoryIO As String = "I/O"
    Public Const SubCategoryTensor As String = "Tensor"
    Public Const SubCategoryTensorSet As String = "TensorSet"
    Public Const SubCategoryDisplay As String = "Display"
    Public Const SubCategoryProbability As String = "Probability"
    Public Const SubCategoryUnsupervised As String = "Unsupervised"


End Class
