Imports Grasshopper.Kernel
Imports Owl.Core.Tensors
Imports Owl.GH.Common

Public Class OwlParamTesterComponent
    Inherits GH_Component

    Public Sub New()
        MyBase.New("OwlParamTester", "Test", "Test", "Testing", "Test")
    End Sub

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_OwlTensor)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim ts As GH_OwlTensorSet = Nothing 'this way we GetData the TensorSet ByRef from the previous component (no casting, no duplicates) 
        'dim ts as TensorSet 'that way the GetData gets the GH_Goo, casts it into a TensorSet and returns a deep copy

        If Not DA.GetData(0, ts) Then Return
        DA.SetData(0, TensorSet.Average(ts.Value))
    End Sub

    Public Overrides ReadOnly Property ComponentGuid() As Guid
        Get
            Return New Guid("a1cc65c2-7ff1-46d4-bed3-7d2cf7b9d647")
        End Get
    End Property
End Class