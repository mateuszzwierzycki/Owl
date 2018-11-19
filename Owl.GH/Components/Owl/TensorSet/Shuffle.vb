Imports System.Drawing

Public Class Shuffle
    Inherits OwlComponentBase

    Sub New()
        MyBase.New("Shuffle TSet", "Shuffle", "Shuffle TensorSet.", SubCategoryTensorSet)
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("{B0E722CE-A207-4235-9443-96FDDF3D3048}")
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_42
        End Get
    End Property

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.primary
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet)
        pManager.AddNumberParameter("Amount", "A", "Amount", GH_ParamAccess.item)
        pManager.AddIntegerParameter("Seed", "S", "Seed", GH_ParamAccess.item)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim ts As New TensorSet
        Dim am As Double
        Dim se As Integer

        If Not DA.GetData(0, ts) Then Return
        If Not DA.GetData(1, am) Then Return
        If Not DA.GetData(2, se) Then Return

        Dim rnd As New Random(se)

        Dim arrv(ts.Count - 1) As Double
        Dim arri(ts.Count - 1) As Double

        For i As Integer = 0 To arrv.Length - 1 Step 1
            arrv(i) = (i / arrv.Length) + ((rnd.NextDouble - 0.5) * am * 2)
            arri(i) = i
        Next

        Array.Sort(arrv, arri)

        Dim ovs As New TensorSet

        For i As Integer = 0 To arri.Length - 1 Step 1
            ovs.Add(ts(arri(i)))
        Next

        DA.SetData(0, ovs)
    End Sub
End Class
