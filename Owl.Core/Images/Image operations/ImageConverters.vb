Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices
Imports Owl.Core.Structures
Imports Owl.Core.Tensors

Namespace Images

    Public Module ImageConverters

        ''' <summary>
        ''' Will convert a Tensor with 3d shape into a multi-channel bitmap. 
        ''' Tensor shape (Height, Width, Channel) is assumed.
        ''' Supports 1, 3 and 4 channels. (Grayscale, RGB, ARGB)
        ''' </summary>
        ''' <param name="Tens"></param>
        ''' <returns></returns>
        Public Function ToBitmap(Tens As Tensor) As Bitmap
            Dim bmp As Bitmap = Nothing

            If Tens.ShapeCount = 1 Then
                Return ToGrayscale(Tens)
            ElseIf Tens.ShapeCount = 2 Then
                Return ToGrayscale(Tens)
            ElseIf Tens.ShapeCount = 3 Then
                If Tens.ShapeAt(2) = 3 Then
                    bmp = New Bitmap(Tens.Width, Tens.Height, PixelFormat.Format24bppRgb)
                ElseIf Tens.ShapeAt(2) = 4 Then
                    bmp = New Bitmap(Tens.Width, Tens.Height, PixelFormat.Format32bppArgb)
                End If
            End If

            Dim rect As Rectangle = New Rectangle(0, 0, bmp.Width, bmp.Height)
            Dim bdata As Imaging.BitmapData = bmp.LockBits(rect, Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat)
            Dim ptr As IntPtr = bdata.Scan0
            Dim stride As Integer = bdata.Stride
            Dim widBytes As Integer = bmp.Width * Tens.ShapeAt(2)

            Dim rgb(bdata.Stride * bdata.Height - 1) As Byte

            For i As Integer = 0 To Tens.Height - 1
                For j As Integer = 0 To widBytes - 1
                    rgb(i * stride + j) = CByte(Tens.TensorData(i * widBytes + j))
                Next
            Next

            Marshal.Copy(rgb, 0, ptr, rgb.Length)
            bmp.UnlockBits(bdata)
            Return bmp
        End Function

        ''' <summary>
        ''' Currently only the 32bppArgb and 24bppRgb formats.
        ''' </summary>
        ''' <param name="Bmp"></param>
        ''' <param name="Format"></param>
        ''' <returns></returns>
        Public Function FromBitmap(Bmp As Bitmap, Format As Imaging.PixelFormat) As Tensor

            Dim str As Integer = 0
            Dim chan As Integer = -1

            Dim b As Bitmap = Bmp
            Dim bd As Drawing.Imaging.BitmapData = Nothing

            Select Case Format
                Case System.Drawing.Imaging.PixelFormat.Format24bppRgb
                    chan = 3
                    bd = b.LockBits(New Rectangle(0, 0, b.Width, b.Height), Drawing.Imaging.ImageLockMode.ReadWrite, Imaging.PixelFormat.Format24bppRgb)
                Case System.Drawing.Imaging.PixelFormat.Format32bppArgb
                    chan = 4
                    bd = b.LockBits(New Rectangle(0, 0, b.Width, b.Height), Drawing.Imaging.ImageLockMode.ReadWrite, Imaging.PixelFormat.Format32bppArgb)
                Case Else
                    Return Nothing
            End Select

            Dim bytes(bd.Stride * bd.Height - 1) As Byte

            str = bd.Stride
            Marshal.Copy(bd.Scan0, bytes, 0, bd.Stride * bd.Height)
            b.UnlockBits(bd)

            Dim clean(b.Width * chan * b.Height - 1) As Byte

            For i As Integer = 0 To b.Height - 1 Step 1
                Buffer.BlockCopy(bytes, str * i, clean, b.Width * chan * i, b.Width * chan)
            Next

            Dim dbl(clean.Length - 1) As Double

            Array.Copy(clean, dbl, clean.Length)

            Dim tens As New Tensor(New List(Of Integer)({b.Height, b.Width, chan}))
            tens.TensorData = dbl

            Return tens
        End Function

        ''' <summary>
        ''' Returns a grayscale 2D Tensor
        ''' gray = (0.21 * r + 0.72 * g + 0.07 * b) / 255 
        ''' </summary>
        ''' <param name="Bmp"></param>
        ''' <returns>Normalized Tensor</returns>
        Public Function FromGrayscaleNormalized(Bmp As Bitmap) As Tensor
            Dim rect As Rectangle = New Rectangle(0, 0, Bmp.Width, Bmp.Height)
            Dim bdata As BitmapData = Bmp.LockBits(rect, ImageLockMode.ReadWrite, Imaging.PixelFormat.Format24bppRgb)

            Dim ptr As IntPtr = bdata.Scan0
            Dim bytes As Integer = bdata.Stride * Bmp.Height

            Dim rgb(bytes - 1) As Byte
            Marshal.Copy(ptr, rgb, 0, bytes)

            Dim r((Bmp.Width * Bmp.Height) - 1) As Byte
            Dim g((Bmp.Width * Bmp.Height) - 1) As Byte
            Dim b((Bmp.Width * Bmp.Height) - 1) As Byte

            Dim count As Integer = 0
            Dim stride As Integer = bdata.Stride

            For i As Integer = 0 To bdata.Height - 1 Step 1
                For j As Integer = 0 To bdata.Width - 1 Step 1
                    b(count) = CByte(rgb((i * stride) + (j * 3) + 0))
                    g(count) = CByte(rgb((i * stride) + (j * 3) + 1))
                    r(count) = CByte(rgb((i * stride) + (j * 3) + 2))
                    count += 1
                Next
            Next

            Dim gray((Bmp.Width * Bmp.Height) - 1) As Double
            Dim denom As Double = 1 / 255

            For i As Integer = 0 To b.Length - 1 Step 1
                gray(i) = 0.21 * r(i) + 0.72 * g(i) + 0.07 * b(i) * denom
            Next

            Bmp.UnlockBits(bdata)

            Dim tensout As New Tensor(New Integer() {Bmp.Height, Bmp.Width}) With {
                .TensorData = gray
            }

            Return tensout
        End Function

        ''' <summary>
        ''' Returns a grayscale 2D Tensor
        ''' gray = 0.21 * r + 0.72 * g + 0.07 * b
        ''' </summary>
        ''' <param name="Bmp"></param>
        Public Function FromGrayscale(Bmp As Bitmap) As Tensor
            Dim rect As Rectangle = New Rectangle(0, 0, Bmp.Width, Bmp.Height)
            Dim bdata As BitmapData = Bmp.LockBits(rect, ImageLockMode.ReadWrite, Imaging.PixelFormat.Format24bppRgb)

            Dim ptr As IntPtr = bdata.Scan0
            Dim bytes As Integer = bdata.Stride * Bmp.Height

            Dim rgb(bytes - 1) As Byte
            Marshal.Copy(ptr, rgb, 0, bytes)

            Dim r((Bmp.Width * Bmp.Height) - 1) As Byte
            Dim g((Bmp.Width * Bmp.Height) - 1) As Byte
            Dim b((Bmp.Width * Bmp.Height) - 1) As Byte

            Dim count As Integer = 0
            Dim stride As Integer = bdata.Stride

            For i As Integer = 0 To bdata.Height - 1 Step 1
                For j As Integer = 0 To bdata.Width - 1 Step 1
                    b(count) = CByte(rgb((i * stride) + (j * 3) + 0))
                    g(count) = CByte(rgb((i * stride) + (j * 3) + 1))
                    r(count) = CByte(rgb((i * stride) + (j * 3) + 2))
                    count += 1
                Next
            Next

            Dim gray((Bmp.Width * Bmp.Height) - 1) As Double

            For i As Integer = 0 To b.Length - 1 Step 1
                gray(i) = 0.21 * r(i) + 0.72 * g(i) + 0.07 * b(i)
            Next

            Bmp.UnlockBits(bdata)

            Dim tensout As New Tensor(New Integer() {Bmp.Height, Bmp.Width}) With {
                .TensorData = gray
            }

            Return tensout
        End Function

        ''' <summary>
        ''' Direct Tensor to grayscale Bitmap conversion... 0-255 range implied.
        ''' </summary>
        ''' <param name="Tens"></param>
        ''' <returns></returns>
        Public Function ToGrayscale(Tens As Tensor) As Bitmap
            Dim bmp As Bitmap = Nothing

            If Tens.ShapeCount = 1 Then
                bmp = New Bitmap(Tens.Length, 1, Imaging.PixelFormat.Format8bppIndexed)
            Else
                bmp = New Bitmap(Tens.Width, Tens.Height, Imaging.PixelFormat.Format8bppIndexed)
            End If

            Dim pal As Imaging.ColorPalette = bmp.Palette

            For i As Integer = 0 To 255 Step 1
                pal.Entries(i) = Color.FromArgb(i, i, i)
            Next

            bmp.Palette = pal

            Dim rect As Rectangle = New Rectangle(0, 0, bmp.Width, bmp.Height)
            Dim bdata As Imaging.BitmapData = bmp.LockBits(rect, Imaging.ImageLockMode.ReadWrite, Imaging.PixelFormat.Format8bppIndexed)

            Dim ptr As IntPtr = bdata.Scan0

            Dim rgb(bdata.Stride * bdata.Height - 1) As Byte

            Dim count As Integer = 0
            Dim stride As Integer = bdata.Stride

            For i As Integer = 0 To bdata.Height - 1 Step 1
                For j As Integer = 0 To bdata.Width - 1 Step 1
                    rgb((i * stride) + j) = CByte(Tens(count))
                    count += 1
                Next
            Next

            Marshal.Copy(rgb, 0, ptr, rgb.Length)

            bmp.UnlockBits(bdata)

            Return bmp
        End Function

        ''' <summary>
        ''' Tensor will be trimmed to stay in R range, then remapped to 0 to 255.
        ''' </summary>
        ''' <param name="Tens">Tensor 2D</param>
        ''' <param name="R">Threshold range</param>
        ''' <returns></returns>
        Public Function ToGrayscale(Tens As Tensor, R As Range) As Bitmap
            Dim bmp As Bitmap = Nothing

            If Tens.ShapeCount = 1 Then
                bmp = New Bitmap(Tens.Length, 1, Imaging.PixelFormat.Format8bppIndexed)
            Else
                bmp = New Bitmap(Tens.Width, Tens.Height, Imaging.PixelFormat.Format8bppIndexed)
            End If

            Tens = Tens.Duplicate

            Tens.TrimFloor(R.Minimum)
            Tens.TrimCeiling(R.Maximum)
            Tens.Remap(R, New Range(0, 255))
            Dim pal As Imaging.ColorPalette = bmp.Palette

            For i As Integer = 0 To 255 Step 1
                pal.Entries(i) = Color.FromArgb(i, i, i)
            Next

            bmp.Palette = pal

            Dim rect As Rectangle = New Rectangle(0, 0, bmp.Width, bmp.Height)
            Dim bdata As Imaging.BitmapData = bmp.LockBits(rect, Imaging.ImageLockMode.ReadWrite, Imaging.PixelFormat.Format8bppIndexed)

            Dim ptr As IntPtr = bdata.Scan0

            Dim rgb(bdata.Stride * bdata.Height - 1) As Byte

            Dim count As Integer = 0
            Dim stride As Integer = bdata.Stride

            For i As Integer = 0 To bdata.Height - 1 Step 1
                For j As Integer = 0 To bdata.Width - 1 Step 1
                    rgb((i * stride) + j) = CByte(Tens(count))
                    count += 1
                Next
            Next

            Marshal.Copy(rgb, 0, ptr, rgb.Length)

            bmp.UnlockBits(bdata)

            Return bmp
        End Function

    End Module

End Namespace
