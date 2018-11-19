Imports Rhino.Geometry
Imports System.Drawing
Imports System.IO
Imports Owl.Core.Visualization
Imports Owl.GH.Common
Imports Grasshopper.Kernel
Imports Owl.Accord.GH.Common
Imports Grasshopper.Kernel.Data

Public Class NetPreview
	Inherits ImageComponentBase

	Sub New()
		MyBase.New("Network Preview",
				   "2DPreview",
				   "2D network preview",
				   "Owl.Accord",
				   SubCategoryDisplay)
	End Sub

	Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("0d6ab38a-5d23-4661-8200-74a1be3d5dd7")
        End Get
    End Property

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.tertiary
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_20
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_ActivationNetwork, "Network", "N", "Activation Network", GH_ParamAccess.tree)
        pManager.AddIntervalParameter("Lines", "L", "Line thickness", GH_ParamAccess.item, New Interval(1, 5))
        pManager.AddNumberParameter("Radius", "R", "Point radius", GH_ParamAccess.item, 5)
        pManager.AddTextParameter("Directory", "D", "Optional directory for the bitmap", GH_ParamAccess.item)
        Me.Params.Input(3).Optional = True
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)

    End Sub

    Private _drawnet As ActivationNetwork = Nothing
    Dim rad As Double = 5
    Dim itv As New Interval(1, 5)

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim sqi As New GH_Structure(Of GH_ActivationNetwork)
        Dim mydir As String = ""
        Me.Message = ""

        If Not DA.GetDataTree(0, sqi) Then Return
        If Not DA.GetData(1, itv) Then Return
        If Not DA.GetData(2, rad) Then Return
        DA.GetData(3, mydir)

        If sqi.AllData(True).Count < 1 Then
            Return
        End If

        Dim flat As List(Of GH_ActivationNetwork) = sqi.FlattenData

        Dim thisnet As ActivationNetwork = flat(0).Value
        _drawnet = thisnet
        Me.Refresh()

        If mydir <> "" Then
            If Not Directory.Exists(mydir) Then Directory.CreateDirectory(mydir)

            Dim fn As New String("Network_" & DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fffffff"))
            Dim bmp As Bitmap = Owl.Accord.Extensions.Visualization.NetworkImage(thisnet, New Core.Structures.Range(itv.Min, itv.Max), rad, Me.ImageAttributes.ImageWidth)
            bmp.Save(mydir & "\" & fn & ".png")
            bmp.Dispose()

        End If
    End Sub

    Public Overrides Sub DrawBackground(ByRef BackImage As Bitmap)
        BackImage = PlotFactory.DrawGridBackground(10, 10)
    End Sub

    Public Overrides Sub DrawForeground(ByRef ForeImage As Bitmap)
        If _drawnet Is Nothing Then Return
        ForeImage = Owl.Accord.Extensions.Visualization.NetworkImage(_drawnet, New Core.Structures.Range(itv.Min, itv.Max), rad, New Size(Me.ImageAttributes.ImageWidth, Me.ImageAttributes.ImageHeight))
    End Sub

    Public Overrides Sub OnAttributesCreation(ByRef atts As ImageComponent_Attributes)
        atts.TileBackground = True
        atts.FrameWidth = 250
        atts.FrameHeight = 250
        atts.Resolution = 3
        atts.LockedSize = False
        atts.Resizeable = True
    End Sub

End Class
