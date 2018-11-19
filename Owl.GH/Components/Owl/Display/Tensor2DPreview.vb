Imports System.IO
Imports Rhino.Geometry

Public Class Tensor2dPreview
    Inherits Owl.GH.Common.ImageComponentBase

    Sub New()
        MyBase.New("Tensor2D Preview", "2DPreview", "Tensor2D preview", OwlComponentBase.SubCategoryDisplay)
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("f2ddab22-e772-41d5-8d02-3fe94679d5b3")
        End Get
    End Property

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.primary
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_22
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_OwlTensor, "Tensor", "T", "Tensor 2D or 3D", GH_ParamAccess.tree)
        pManager.AddIntervalParameter("Range", "R", "Range", GH_ParamAccess.item, New Interval(0, 0))
        pManager.AddTextParameter("Directory", "D", "Optional directory for the bitmap", GH_ParamAccess.item)
        pManager.AddBooleanParameter("SaveAll", "S", "Save all, display the first one", GH_ParamAccess.item, False)
        Me.Params.Input(1).Optional = True
        Me.Params.Input(2).Optional = True
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)

    End Sub

    Private _drawtens As Tensor = Nothing
    Private _range As Range = New Range(0, 0)

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim sqi As New GH_Structure(Of GH_OwlTensor)

        Dim all As Boolean = False
        Dim mydir As String = ""
        Dim rng As Interval = Nothing

        Me.Message = ""

        If Not DA.GetDataTree(0, sqi) Then Return

        DA.GetData(1, rng)
        Dim asr As New Range(rng.T0, rng.T1)

        DA.GetData(2, mydir)
        DA.GetData(3, all)

        If sqi.AllData(True).Count < 1 Then
            Return
        End If

        Dim flat As List(Of GH_OwlTensor) = sqi.FlattenData

        If flat(0).Value.GetShape.Count < 2 Then
            Me.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The first Tensor is not 2D")
        End If

        Dim thistens As Tensor = flat(0).Value '.Duplicate

        _drawtens = thistens
        _range = asr
        Me.Refresh()

        If mydir <> "" Then
            If Not Directory.Exists(mydir) Then Directory.CreateDirectory(mydir)

            For i As Integer = 0 To flat.Count - 1 Step 1
                Dim loctens As Tensor = flat(i).Value.Duplicate
                Dim fn As New String("Tensor_" & i & "_" & DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fffffff"))
                Dim bmp As Bitmap = Nothing

                If loctens IsNot Nothing Then
                    If loctens.ShapeCount < 3 Then
                        If _range.Length = 0 Then
                            bmp = Tensor2DImage(loctens)
                        Else
                            bmp = Tensor2DImage(loctens, _range)
                        End If
                    Else
                        If _range.Length = 0 Then
                            bmp = Tensor3DImage(loctens)
                        Else
                            bmp = Tensor3DImage(loctens, _range)
                        End If
                    End If
                End If

                bmp.Save(mydir & "\" & fn & ".png")
                bmp.Dispose()
                If Not all Then Exit For
            Next

        End If
    End Sub

    Public Overrides Sub DrawBackground(ByRef BackImage As Bitmap)
        If _drawtens Is Nothing Then
            BackImage = PlotFactory.DrawGridBackground(10, 10)
        Else
            BackImage = Nothing
        End If
    End Sub

    Public Overrides Sub DrawForeground(ByRef ForeImage As Bitmap)
        If _drawtens IsNot Nothing Then
            If _drawtens.ShapeCount < 3 Then
                If _range.Length = 0 Then
                    ForeImage = Tensor2DImage(_drawtens)
                Else
                    ForeImage = Tensor2DImage(_drawtens, _range)
                End If
            Else
                If _range.Length = 0 Then
                    ForeImage = Tensor3DImage(_drawtens)
                Else
                    ForeImage = Tensor3DImage(_drawtens, _range)
                End If
            End If
        End If
    End Sub

    Public Overrides Sub OnAttributesCreation(ByRef atts As ImageComponent_Attributes)
        atts.TileBackground = True
    End Sub

End Class
