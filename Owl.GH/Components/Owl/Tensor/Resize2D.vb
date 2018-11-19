Public Class Resize2D
	Inherits GH_Component

	Sub New()
		MyBase.New("Resize", "Resize", "Resize a 2D Tensor", "Owl", "Image")
	End Sub

	Public Overrides ReadOnly Property ComponentGuid As Guid
		Get
			Return New Guid("{091F83C3-C491-4299-AF55-43CBF1E2F7EB}")
		End Get
	End Property

	Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
		pManager.AddParameter(New Param_OwlTensor)
		pManager.AddIntegerParameter("Width", "W", "New width", GH_ParamAccess.item)
		pManager.AddIntegerParameter("Height", "H", "New height", GH_ParamAccess.item)
		pManager.AddIntegerParameter("Interpolation", "I", "Interpolation type", GH_ParamAccess.item, 0)

		Dim pin As Parameters.Param_Integer = Me.Params.Input(3)
		pin.AddNamedValue("Default", 0)
		pin.AddNamedValue("Low", 1)
		pin.AddNamedValue("High", 2)
		pin.AddNamedValue("Bilinear", 3)
		pin.AddNamedValue("Bicubic", 4)
		pin.AddNamedValue("NearestNeighbor", 5)
		pin.AddNamedValue("HighQualityBilinear", 6)
		pin.AddNamedValue("HighQualityBicubic", 7)

	End Sub

	Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
		pManager.AddParameter(New Param_OwlTensor)
	End Sub

	Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
		Dim tens As GH_OwlTensor = Nothing
		If Not DA.GetData(0, tens) Then Return

		Dim w As Integer
		Dim h As Integer
		If Not DA.GetData(1, w) Then Return
		If Not DA.GetData(2, h) Then Return
		If w < 1 Then Me.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Width < 1 !") : Return
		If h < 1 Then Me.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Height < 1 !") : Return

		Dim inter As Integer
		If Not DA.GetData(3, inter) Then Return
		If inter < 0 Or inter > 7 Then Me.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Wrong interpolation value") : Return

		Dim tout As New Tensor(w, h)

		Using nb As Bitmap = New Bitmap(w, h)
			Using bmp As Bitmap = Owl.Core.Images.ImageConverters.ToBitmap(tens.Value)
				Using g As Graphics = Graphics.FromImage(nb)
					g.InterpolationMode = inter
					g.DrawImage(bmp, nb.GetBounds(GraphicsUnit.Pixel))
					tout = Owl.Core.Images.ImageConverters.FromGrayscale(nb)
				End Using
			End Using
		End Using

		DA.SetData(0, tout)
	End Sub

End Class
