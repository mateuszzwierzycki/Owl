Imports Rhino.Geometry
Imports System.Windows.Forms
Imports GH_IO.Serialization

Public Class TensorSetPolylines
    Inherits OwlComponentBase
    Implements IGH_PreviewObject

    Private pl As New List(Of Polyline)
    Private geom As Boolean = False

    Sub New()
        MyBase.New("TensorSet Polylines", "TSPoly", "Plot a TensorSet as a set of polylines", OwlComponentBase.SubCategoryDisplay)
        Me.Message = If(OutputGeometry, "Geometry", "Display")
    End Sub

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.secondary
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_25
        End Get
    End Property

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("96189ef9-5923-4be2-8149-f0da3aecb3ce")
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

    Protected Overrides Sub AppendAdditionalComponentMenuItems(menu As ToolStripDropDown)
        Menu_AppendItem(menu, "Output Geometry", AddressOf OutSwitch, True, OutputGeometry)
        MyBase.AppendAdditionalComponentMenuItems(menu)
    End Sub

    Private Sub OutSwitch()
        OutputGeometry = Not OutputGeometry
        Me.ExpireSolution(True)
        Me.Message = If(OutputGeometry, "Geometry", "Display")
        Me.OnDisplayExpired(True)
    End Sub

    Public Overrides Function Read(reader As GH_IReader) As Boolean
        Me.OutputGeometry = reader.GetBoolean("Out")
        Me.Message = If(OutputGeometry, "Geometry", "Display")
        Me.OnDisplayExpired(True)
        Return MyBase.Read(reader)
    End Function

    Public Overrides Function Write(writer As GH_IWriter) As Boolean
        writer.SetBoolean("Out", OutputGeometry)
        Return MyBase.Write(writer)
    End Function

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet, "TensorSet", "TS", "Owl TensorSet, up to 6 dimensional Tensors", GH_ParamAccess.item)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddCurveParameter("Polylines", "P", "Polylines", GH_ParamAccess.list)
        pManager.HideParameter(0)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        If DA.Iteration = 0 Then globb = New BoundingBox

        Dim ts As GH_OwlTensorSet = Nothing
        If DA.Iteration = 0 Then
            pl.Clear()
        End If

        If Not DA.GetData(0, ts) Then Return

        Dim cnt As Integer = pl.Count

        For i As Integer = 0 To ts.Value.Count - 1 Step 1
            Dim tens As Tensor = ts.Value(i)

            Dim pt As New Point3d
            Dim thispl As New Polyline

            For j As Integer = 0 To tens.Length - 1 Step 1
                Select Case j Mod 3
                    Case 0
                        pt.X = tens(j)
                        If j = tens.Length - 1 Then thispl.Add(pt)
                    Case 1
                        pt.Y = tens(j)
                        If j = tens.Length - 1 Then thispl.Add(pt)
                    Case 2
                        pt.Z = tens(j)
                        thispl.Add(pt)
                        pt = Point3d.Origin
                End Select
            Next

            pl.Add(thispl)
            globb.Union(New BoundingBox(thispl))
        Next

        If OutputGeometry Then
            Dim thispl As New List(Of Curve)

            For i As Integer = cnt To pl.Count - 1 Step 1
                thispl.Add(pl(i).ToNurbsCurve)
            Next

            DA.SetDataList(0, thispl)
        End If

    End Sub

    Dim globb As New BoundingBox()

    Public Overrides ReadOnly Property ClippingBox As BoundingBox
        Get
            Return globb
        End Get
    End Property

    Public Overrides Sub DrawViewportWires(args As IGH_PreviewArgs)
        If Me.Hidden Or Me.Locked Then Return
        If pl IsNot Nothing Then
            If pl.Count > 0 Then
                Dim hue As Double = 0
                Dim denom As Double = 1 / pl.Count
                For i As Integer = 0 To pl.Count - 1 Step 1
                    hue = i * denom
                    Dim col As New Rhino.Display.ColorHSL(hue, 1, 0.5)

                    If pl(i).Count = 1 Then
                        args.Display.DrawPoint(pl(i)(0), Rhino.Display.PointStyle.Simple, 5, col)
                    Else
                        args.Display.DrawPolyline(pl(i), col)
                        For Each s As Line In pl(i).GetSegments
                            args.Display.DrawPoint(s.From, Rhino.Display.PointStyle.Simple, 5, col)
                            args.Display.DrawPoint(s.To, Rhino.Display.PointStyle.Simple, 5, col)
                            args.Display.DrawArrowHead(s.To, s.To - s.From, col, 10, 0)
                        Next
                    End If
                Next
            End If
        End If

        MyBase.DrawViewportWires(args)
    End Sub

End Class

