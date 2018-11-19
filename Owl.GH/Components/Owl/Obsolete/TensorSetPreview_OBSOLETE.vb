Imports Rhino.Geometry
Imports System.IO
Imports Owl.Core.Tensors

Public Class TensorSetPreview_OBSOLETE
    Inherits GH_Component

    Sub New()
        MyBase.New("TensorSet Preview", "TPreview", "Quick TensorSet preview", "Owl", "Display")
    End Sub

    Public Overrides Sub CreateAttributes()
        m_attributes = New TensorSetPreview_Attributes(Me)
        att = m_attributes
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("{F4B58E51-059C-4F89-AE5C-59EF7AD69C16}")
        End Get
    End Property

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.hidden
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet, "TensorSet", "V", "Owl TensorSet", GH_ParamAccess.tree)
        pManager.AddIntervalParameter("Range", "R", "Y Axis range", GH_ParamAccess.item, New Interval(0, 1))
        pManager.AddIntegerParameter("Highlight", "H", "Highlight the Tensors with those indices", GH_ParamAccess.list)
        pManager.AddIntegerParameter("Size", "S", "Bitmap size / quality", GH_ParamAccess.item, 800)
        pManager.AddTextParameter("Directory", "D", "Optional directory for the bitmap", GH_ParamAccess.item)

        pManager.Param(0).Optional = True
        pManager.Param(2).Optional = True
        pManager.Param(4).Optional = True
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)

    End Sub

    Dim att As TensorSetPreview_Attributes
    Private m_vs As New TensorSet

    Friend Property VecSet As TensorSet
        Get
            Return m_vs
        End Get
        Set(value As TensorSet)
            m_vs = value
        End Set
    End Property


    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        'Me.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Repair me")
        'Return

        Dim sqi As New GH_Structure(Of GH_OwlTensorSet)
        Dim itv As New Interval(0, 1)
        Dim high As New List(Of Integer)
        Dim q As Integer
        Dim mydir As String = Nothing

        Me.Message = ""

        If Not DA.GetDataTree(0, sqi) Then Return
        If Not DA.GetData(1, itv) Then Return
        DA.GetDataList(2, high)
        DA.GetData(3, q)
        DA.GetData(4, mydir)

        If sqi.AllData(True).Count < 1 Then
            Return
        End If

        If m_vs IsNot Nothing Then m_vs.Clear()
        m_vs = New TensorSet()
        Dim vsl As List(Of GH_OwlTensorSet) = sqi.FlattenData

        For i As Integer = 0 To vsl.Count - 1 Step 1
            m_vs.AddRange(vsl(i).Value)
        Next

        If Not m_vs.IsHomogeneous Then Me.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "TensorSet is heterogeneous.")

        If mydir Is Nothing Then
            att.CreateImage(New Range(itv.T0, itv.T1), high, q)
        Else
            If Not Directory.Exists(mydir) Then Directory.CreateDirectory(mydir)
            Dim fn As New String("TensorSet_" & DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fffffff"))
            att.CreateImage(New Range(itv.T0, itv.T1), high, q).Save(mydir & "\" & fn & ".bmp")
        End If

    End Sub
End Class
