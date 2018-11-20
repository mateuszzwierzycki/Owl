Imports System.Drawing
Imports Accord.Video.DirectShow
Imports Grasshopper
Imports Grasshopper.Kernel
Imports Grasshopper.Kernel.Data
Imports Owl.GH.Common

Public Class ListWebcams
	Inherits GH_Component

	Sub New()
		MyBase.New("List Devices",
				   "Devices",
				   "Image capturing devices",
				   "Owl.Accord",
				   OwlComponentBase.SubCategoryDisplay)
	End Sub

	Protected Overrides ReadOnly Property Icon As Bitmap
		Get
			Return My.Resources.Icons_new_09
		End Get
	End Property

	Public Overrides ReadOnly Property ComponentGuid As Guid
		Get
			Return New Guid("7fcb8a88-9697-48d1-a774-4414db26fd04")
		End Get
	End Property

	Public Overrides ReadOnly Property Exposure As GH_Exposure
		Get
			Return GH_Exposure.tertiary
		End Get
	End Property

	Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)

	End Sub

	Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
		pManager.AddTextParameter("Devices", "D", "List of devices", GH_ParamAccess.list)
		pManager.AddTextParameter("Modes", "M", "Camera modes", GH_ParamAccess.tree)
	End Sub

	Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
		Dim videoDevices = New FilterInfoCollection(FilterCategory.VideoInputDevice)

		Dim desc As New List(Of String)
		Dim douts As New DataTree(Of String)


		Dim cnt As Integer = 0
		For Each dev As FilterInfo In videoDevices
			Dim thisdev = New VideoCaptureDevice(dev.MonikerString, Imaging.PixelFormat.Format24bppRgb)

			For i As Integer = 0 To thisdev.VideoCapabilities.Length - 1 Step 1
				Dim thiscap = thisdev.VideoCapabilities(i)
				douts.Add(thiscap.FrameSize.ToString, New GH_Path(cnt))
			Next

			desc.Add(dev.ToString)
			cnt += 1
		Next

		DA.SetDataList(0, desc)
		DA.SetDataTree(1, douts)
	End Sub
End Class
