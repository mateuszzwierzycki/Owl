Imports Grasshopper.Kernel.Parameters

Public Class LoadBitmap
    Inherits OwlComponentBase

    Sub New()
        MyBase.New("Load Bitmap", "LoadBmp", "Loads a bitmap as a Tensor.", SubCategoryIO)
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("8b69e7ec-9c5a-4739-8695-c9c913db72b8")
        End Get
    End Property

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.primary
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Grasshopper.Kernel.Parameters.Param_FilePath, "File Path", "F", "Bitmap to load", GH_ParamAccess.item)
        pManager.AddIntegerParameter("Format", "I", "PixelFormat", GH_ParamAccess.item, 0)

        Dim pi As Param_Integer = Me.Params.Input(1)
        pi.AddNamedValue("Grayscale", 0)
        pi.AddNamedValue("Grayscale Normalized", 1)
        pi.AddNamedValue("RGB", 2)
        pi.AddNamedValue("ARGB", 3)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_OwlTensor)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim dir As String = ""
        If Not DA.GetData(0, dir) Then Return

        Dim pic As Integer = 0
        If Not DA.GetData(1, pic) Then Return

        Dim pth As New Parameters.Param_FilePath

        Try
            Dim bmp As New Bitmap(dir)

            Dim tens As Tensor = Nothing

            Select Case pic
                Case 0
                    tens = FromGrayscale(bmp)
                Case 1
                    tens = FromGrayscaleNormalized(bmp)
                Case 2
                    tens = FromBitmap(bmp, Imaging.PixelFormat.Format24bppRgb)
                Case 3
                    tens = FromBitmap(bmp, Imaging.PixelFormat.Format32bppArgb)
            End Select

            bmp.Dispose()
            DA.SetData(0, tens)
        Catch ex As Exception
            Me.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, ex.Message)
        End Try
    End Sub

End Class
