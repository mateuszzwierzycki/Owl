Imports System.Drawing
Imports Rhino.Geometry
Imports System.Windows.Forms
Imports GH_IO.Serialization
Imports Grasshopper.GUI

'TODO implement idisposable
Public Class TensorSetPointCloud
    Inherits OwlComponentBase
    Implements IGH_PreviewObject

    Private pc As New PointCloud
    Private si As Integer = 4
    Private geom As Boolean = False

    Sub New()
        MyBase.New("TensorSet Cloud", "TSCloud", "Plot a TensorSet as a point cloud", OwlComponentBase.SubCategoryDisplay)
        Me.Message = If(OutputGeometry, "Geometry", "Display")
    End Sub

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.secondary
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_23
        End Get
    End Property

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("7a71f58b-0a31-458a-b40c-2b0bc007f817")
        End Get
    End Property

    Public Property OutputGeometry As Boolean
        Get
            Return geom
        End Get
        Set(value As Boolean)
            geom = value
        End Set
    End Property

    Public Property PointSize As Integer
        Get
            Return si
        End Get
        Set(value As Integer)
            si = value
        End Set
    End Property

    Protected Overrides Sub AppendAdditionalComponentMenuItems(menu As ToolStripDropDown)
        Menu_AppendItem(menu, "Output Geometry", AddressOf OutSwitch, True, OutputGeometry)
        Dim thisit As ToolStripMenuItem = Menu_AppendItem(menu, "Point radius")

        Menu_AppendTextItem(thisit.DropDown, PointSize, AddressOf OnSizeKeyDown, AddressOf OnSizeTextChanged, True)

        MyBase.AppendAdditionalComponentMenuItems(menu)
    End Sub

    Private Sub OnSizeTextChanged(sender As GH_MenuTextBox, text As String)
        sender.Text = JustDigitsNoZero(text)
    End Sub

    Private Sub OnSizeKeyDown(sender As GH_MenuTextBox, e As KeyEventArgs)
        If e.KeyCode = Keys.Enter And sender.Text <> "" Then
            SizeSwitch(Integer.Parse(sender.Text))
        End If
    End Sub

    Private Function JustDigitsNoZero(S As String) As String
        Dim ns As String = ""
        Dim cnt As Integer = 0
        For Each c As Char In S
            If cnt = 0 And c = "0" Then Continue For
            If Char.IsDigit(c) Then ns &= c
            cnt += 1
        Next
        Return ns
    End Function

    Private Sub SizeSwitch(NewSize As Integer)
        PointSize = NewSize
        Me.OnPreviewExpired(True)
        Me.Message = If(OutputGeometry, "Geometry", "Display")
        Me.OnDisplayExpired(True)
    End Sub

    Private Sub OutSwitch()
        OutputGeometry = Not OutputGeometry
        Me.ExpireSolution(True)
        Me.Message = If(OutputGeometry, "Geometry", "Display")
        Me.OnDisplayExpired(True)
    End Sub

    Public Overrides Function Read(reader As GH_IReader) As Boolean
        Me.OutputGeometry = reader.GetBoolean("Out")
        Me.PointSize = reader.GetInt32("Size")
        Me.Message = If(OutputGeometry, "Geometry", "Display")
        Me.OnDisplayExpired(True)
        Return MyBase.Read(reader)
    End Function

    Public Overrides Function Write(writer As GH_IWriter) As Boolean
        writer.SetBoolean("Out", OutputGeometry)
        writer.SetInt32("Size", PointSize)
        Return MyBase.Write(writer)
    End Function

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet, "TensorSet", "TS", "Owl TensorSet, up to 6 dimensional Tensors", GH_ParamAccess.item)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddPointParameter("Points", "P", "Points", GH_ParamAccess.list)
        pManager.HideParameter(0)
        pManager.AddColourParameter("Colors", "C", "Colors", GH_ParamAccess.list)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim ts As GH_OwlTensorSet = Nothing
        If DA.Iteration = 0 Then
            pc.Dispose()
            pc = New PointCloud
        End If

        If Not DA.GetData(0, ts) Then Return

        Dim tsc As TensorSet = ts.Value.Duplicate
        Dim thisr As Range = tsc.GetRange

        tsc.Remap(thisr, New Range(0, 255), 3)
        tsc.Remap(thisr, New Range(0, 255), 4)
        tsc.Remap(thisr, New Range(0, 255), 5)

        Dim prec As Integer = pc.Count

        For i As Integer = 0 To tsc.Count - 1 Step 1
            Dim tens As Tensor = tsc(i)

            Dim pt As New Point3d
            Dim col As New Point3d

            For j As Integer = 0 To Math.Min(tens.Length, 6) - 1 Step 1
                Select Case j
                    Case 0
                        pt.X = tens(j)
                    Case 1
                        pt.Y = tens(j)
                    Case 2
                        pt.Z = tens(j)
                    Case 3
                        col.X = tens(j)
                    Case 4
                        col.Y = tens(j)
                    Case 5
                        col.Z = tens(j)
                End Select
            Next

            pc.Add(pt, Color.FromArgb(col.X, col.Y, col.Z))
        Next

        If OutputGeometry Then
            Dim pl As New List(Of Point3d)
            Dim cl As New List(Of Color)

            For i As Integer = prec To pc.Count - 1 Step 1
                pl.Add(pc(i).Location)
                cl.Add(pc(i).Color)
            Next

            DA.SetDataList(0, pl)
            DA.SetDataList(1, cl)
        End If

    End Sub

    Public Overrides ReadOnly Property ClippingBox As BoundingBox
        Get
            If pc Is Nothing Then Return BoundingBox.Empty
            Return pc.GetBoundingBox(False)
        End Get
    End Property

    Public Overrides Sub DrawViewportWires(args As IGH_PreviewArgs)
        If Me.Hidden Or Me.Locked Then Return
        If pc IsNot Nothing Then
            If pc.Count > 0 Then
                args.Display.DrawPointCloud(pc, PointSize)
            End If
        End If
        MyBase.DrawViewportWires(args)
    End Sub

End Class
