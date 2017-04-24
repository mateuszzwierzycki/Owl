Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices
Imports Owl.Core.Structures
Imports Owl.Core.Tensors

Namespace Images

    Public Module ImageConverters

        ''' <summary>
        ''' Returns a grayscale 2D Tensor
        ''' gray = 0.21 * r + 0.72 * g + 0.07 * b
        ''' </summary>
        ''' <param name="Bmp"></param>
        ''' <returns>Normalized Tensor</returns>
        Public Function FromBitmapNormalized(Bmp As Bitmap) As Tensor
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
        Public Function FromBitmap(Bmp As Bitmap) As Tensor
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
        ''' <param name="A"></param>
        ''' <returns></returns>
        Public Function ToGrayscale(A As Tensor) As Bitmap
            Dim bmp As New Bitmap(A.Width, A.Height, Imaging.PixelFormat.Format8bppIndexed)
            Dim pal As Imaging.ColorPalette = bmp.Palette

            For i As Integer = 0 To 255 Step 1
                pal.Entries(i) = Color.FromArgb(i, i, i)
            Next

            bmp.Palette = pal

            Dim rect As Rectangle = New Rectangle(0, 0, A.Width, A.Height)
            Dim bdata As Imaging.BitmapData = bmp.LockBits(rect, Imaging.ImageLockMode.ReadWrite, Imaging.PixelFormat.Format8bppIndexed)

            Dim ptr As IntPtr = bdata.Scan0

            Dim rgb(bdata.Stride * bdata.Height - 1) As Byte

            Dim count As Integer = 0
            Dim stride As Integer = bdata.Stride

            For i As Integer = 0 To bdata.Height - 1 Step 1
                For j As Integer = 0 To bdata.Width - 1 Step 1
                    rgb((i * stride) + j) = CByte(A(count))
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
        ''' <param name="A">Tensor 2D</param>
        ''' <param name="R"></param>
        ''' <returns></returns>
        Public Function ToGrayscale(A As Tensor, R As Range) As Bitmap
            A = A.Duplicate

            A.TrimFloor(R.Minimum)
            A.TrimCeiling(R.Maximum)
            A.Remap(R, New Range(0, 255))

            Dim bmp As New Bitmap(A.Width, A.Height, Imaging.PixelFormat.Format8bppIndexed)
            Dim pal As Imaging.ColorPalette = bmp.Palette

            For i As Integer = 0 To 255 Step 1
                pal.Entries(i) = Color.FromArgb(i, i, i)
            Next

            bmp.Palette = pal

            Dim rect As Rectangle = New Rectangle(0, 0, A.Width, A.Height)
            Dim bdata As Imaging.BitmapData = bmp.LockBits(rect, Imaging.ImageLockMode.ReadWrite, Imaging.PixelFormat.Format8bppIndexed)

            Dim ptr As IntPtr = bdata.Scan0

            Dim rgb(bdata.Stride * bdata.Height - 1) As Byte

            Dim count As Integer = 0
            Dim stride As Integer = bdata.Stride

            For i As Integer = 0 To bdata.Height - 1 Step 1
                For j As Integer = 0 To bdata.Width - 1 Step 1
                    rgb((i * stride) + j) = CByte(A(count))
                    count += 1
                Next
            Next

            Marshal.Copy(rgb, 0, ptr, rgb.Length)

            bmp.UnlockBits(bdata)

            Return bmp
        End Function

    End Module

End Namespace
