Imports System.Drawing
Imports Rhino.Geometry
Imports System.IO
Imports Owl.Core.Tensors

Public Class TensorSetDisplay
    Inherits ImageComponentBase

    Sub New()
        MyBase.New("TensorSet Display", "TSView", "Plot a TensorSet", OwlComponentBase.SubCategoryDisplay)
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("8f4e759a-25a0-4e2b-a7aa-e8537322b6a5")
        End Get
    End Property

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.secondary
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_24
        End Get
    End Property

    Private _ts As New TensorSet
    Private _img As Bitmap = Nothing
    Private _highlights As New List(Of Integer)
    Private _intrvl As New Interval

    Property TSet As TensorSet
        Get
            Return _ts
        End Get
        Set(value As TensorSet)
            _ts = value
        End Set
    End Property

    Public Property Highlights As List(Of Integer)
        Get
            Return _highlights
        End Get
        Set(value As List(Of Integer))
            _highlights = value
        End Set
    End Property

    Public Property Intrvl As Interval
        Get
            Return _intrvl
        End Get
        Set(value As Interval)
            _intrvl = value
        End Set
    End Property

    Public Overrides Sub DrawBackground(ByRef BackImage As Bitmap)
        BackImage = Owl.Core.Visualization.PlotFactory.DrawGridBackground(10, 10)
    End Sub

    Public Overrides Sub DrawForeground(ByRef ForeImage As Bitmap)
        If TSet Is Nothing Then Exit Sub
        ForeImage = Owl.Core.Visualization.PlotFactory.TensorSetPlot(TSet, New Range(Intrvl.T0, Intrvl.T1), New Size(ForeImage.Width, ForeImage.Height), 2, Highlights)
    End Sub

    Public Overrides Sub OnAttributesCreation(ByRef atts As ImageComponent_Attributes)
        atts.Resizeable = True
        atts.TileBackground = True
    End Sub

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet, "TensorSet", "TS", "Owl TensorSet", GH_ParamAccess.tree)
        pManager.AddIntervalParameter("Range", "R", "Y Axis range", GH_ParamAccess.item, New Interval(0, 1))
        pManager.AddIntegerParameter("Highlight", "H", "Highlight the Tensors with those indices", GH_ParamAccess.list)
        pManager.AddTextParameter("Directory", "D", "Optional directory for the bitmap", GH_ParamAccess.item)

        pManager.Param(0).Optional = True
        pManager.Param(2).Optional = True
        pManager.Param(3).Optional = True
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)

    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)

        Dim sqi As New GH_Structure(Of GH_OwlTensorSet)
        Dim itv As New Interval(0, 1)
        Dim high As New List(Of Integer)
        Dim mydir As String = Nothing

        Me.Message = ""

        If Not DA.GetDataTree(0, sqi) Then Return
        If Not DA.GetData(1, itv) Then Return
        DA.GetDataList(2, high)
        DA.GetData(3, mydir)

        If sqi.AllData(True).Count < 1 Then
            Return
        End If

        If _ts IsNot Nothing Then _ts.Clear()
        _ts = New TensorSet()
        Dim vsl As List(Of GH_OwlTensorSet) = sqi.FlattenData

        For i As Integer = 0 To vsl.Count - 1 Step 1
            _ts.AddRange(vsl(i).Value)
        Next

        If Not _ts.IsHomogeneous Then Me.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "TensorSet is heterogeneous.")

        TSet = _ts
        Highlights.Clear()
        Highlights.AddRange(high)
        Intrvl = itv

        Me.Refresh()

        If mydir <> "" Then
            If Not Directory.Exists(mydir) Then Directory.CreateDirectory(mydir)
            Dim fn As New String("TensorSet_" & DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fffffff"))
            ImageAttributes.SaveForeground(mydir & "\" & fn & ".png")
        End If

    End Sub


End Class
