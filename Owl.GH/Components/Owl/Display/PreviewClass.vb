Imports Rhino.Geometry
Imports System.Drawing

Public Class PreviewClass
    Inherits OwlComponentBase
    Implements IGH_PreviewObject

    Sub New()
        MyBase.New("Preview Classes", "CPreview", "Quick Class preview", SubCategoryDisplay)
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("e70c5f8a-03e3-428a-a610-3ce91a500e0d")
        End Get
    End Property

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.secondary
        End Get
    End Property


    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_21
        End Get
    End Property


    Protected Overrides Sub RegisterInputParams(pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddGeometryParameter("Geometry", "G", "Geometry to use for classification display", GH_ParamAccess.tree)
        pManager.AddTextParameter("Name", "N", "Class name", GH_ParamAccess.list)
        pManager.AddPointParameter("Position", "P", "Text position", GH_ParamAccess.item, New Point3d(0.1, 0.1, 0.1))
        pManager.AddIntegerParameter("Size", "S", "Text size", GH_ParamAccess.item, 12)
        pManager.AddIntegerParameter("Seed", "S", "Random color seed", GH_ParamAccess.item, 123)

        For i As Integer = 0 To Me.Params.Input.Count - 1 Step 1
            Me.Params.Input(i).Optional = True
            pManager.HideParameter(i)
        Next

    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)

    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        boxwires.Clear()
        boxcolors.Clear()
        boxnames.Clear()
        boxpos.Clear()

        Dim ght As New GH_Structure(Of IGH_GeometricGoo)
        Dim nam As New List(Of String)
        Dim pos As New Point3d
        Dim siz As Integer
        Dim see As Integer = 123

        If Not DA.GetData(2, pos) Then Return
        If Not DA.GetData(3, siz) Then Return

        textsiz = siz
        DA.GetData(4, see)

        If Not DA.GetDataTree(0, ght) Then Return
        DA.GetDataList(1, nam)

        If nam.Count <> ght.Branches.Count Then _
            Me.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Geometry branch count <> Names count")

        For Each d As IGH_GeometricGoo In ght.AllData(True)
            bb.Union(d.GetBoundingBox(Transform.Identity))
        Next


        Dim dict As New SortedList(Of String, Color)

        For i As Integer = 0 To nam.Count - 1 Step 1
            If Not dict.ContainsKey(nam(i)) Then
                Dim rnd As New Random(StringBytesum(nam(i)) + see)

                dict.Add(nam(i), New Rhino.Display.ColorHSL(rnd.NextDouble, rnd.Next(5, 11) / 10, rnd.Next(4, 7) / 10))
            End If
            boxcolors.Add(dict(nam(i)))
        Next

        For i As Integer = 0 To ght.Paths.Count - 1 Step 1
            Dim thisbb As BoundingBox = BoundingBox.Empty

            For j As Integer = 0 To ght.Branch(ght.Path(i)).Count - 1 Step 1
                Dim thisitem As IGH_GeometricGoo = ght.Branch(ght.Path(i)).Item(j)
                thisbb.Union(thisitem.GetBoundingBox(Transform.Identity))
            Next

            Dim thisw As Double = thisbb.Corner(0, 0, 0).DistanceTo(thisbb.Corner(1, 0, 0))
            Dim thisd As Double = thisbb.Corner(0, 0, 0).DistanceTo(thisbb.Corner(0, 1, 0))
            Dim thish As Double = thisbb.Corner(0, 0, 0).DistanceTo(thisbb.Corner(0, 0, 1))

            thisbb.Inflate(0.05 * thisw, 0.05 * thisd, 0.05 * thish)

            boxpos.Add(thisbb.PointAt(pos.X, pos.Y, pos.Z))
            boxwires.Add(New List(Of Line)(thisbb.GetEdges))
        Next

        boxnames.AddRange(nam)
    End Sub

    Dim bb As BoundingBox = BoundingBox.Empty

    Dim boxwires As New List(Of List(Of Line))
    Dim boxcolors As New List(Of Color)
    Dim boxnames As New List(Of String)
    Dim textsiz As Integer = 12
    Dim boxpos As New List(Of Point3d)

    Private Function GetBytes(str As String) As Byte()
        Dim bytes As Byte() = New Byte(str.Length * 2 - 1) {}
        System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length)
        Return bytes
    End Function

    Private Function StringBytesum(str As String) As Integer
        Dim b() As Byte = GetBytes(str)

        Dim sum As Integer = 0

        For i As Integer = 0 To b.Length - 1 Step 1
            sum += b(i)
        Next

        Return sum
    End Function

    Public Overrides Sub DrawViewportWires(args As IGH_PreviewArgs)
        MyBase.DrawViewportWires(args)

        If (Not Hidden) And (Not Me.Locked) And (boxnames.Count = boxcolors.Count) And (boxnames.Count = boxpos.Count) Then
            For i As Integer = 0 To boxwires.Count - 1 Step 1
                args.Display.DrawLines(boxwires(i), boxcolors(i))
            Next

            For i As Integer = 0 To boxnames.Count - 1 Step 1
                args.Display.Draw2dText(boxnames(i), boxcolors(i), boxpos(i), True, textsiz)
            Next
        End If

    End Sub

    Public Overrides ReadOnly Property ClippingBox As BoundingBox
        Get
            Return bb
        End Get
    End Property

End Class
