Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices
Imports Owl.Core.Tensors
Imports Owl.Core.Structures
Imports System.IO

Namespace Images
    Public Module ImageOperators

        ''' <summary>
        ''' A(i) * B(i)
        ''' </summary>
        ''' <param name="A">2D Tensor</param>
        ''' <param name="B">2D Tensor</param>
        ''' <returns></returns>
        Public Function ImageMultiply(A As Tensor, B As Tensor) As Tensor
            If A.Width <> B.Width OrElse A.Height <> B.Height Then Return Nothing

            Dim ni As New Tensor(A)

            For i As Integer = 0 To B.Length - 1 Step 1
                ni(i) *= B(i)
            Next

            Return ni
        End Function

        ''' <summary>
        ''' Computes average value of all the Tensor values.
        ''' </summary>
        ''' <param name="A">2D Tensor</param>
        ''' <returns></returns>
        Public Function ImageAverage(A As Tensor) As Double
            Dim sum As Double = 0

            For i As Integer = 0 To A.Length - 1 Step 1
                sum += A(i)
            Next

            sum /= A.Length
            Return sum
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Bmp"></param>
        ''' <returns>Normalized Tensor</returns>
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
            Dim denom As Double = 1 / 255

            For i As Integer = 0 To b.Length - 1 Step 1
                gray(i) = 0.21 * r(i) + 0.72 * g(i) + 0.07 * b(i) * denom
            Next

            Return New Tensor(Bmp.Width, Bmp.Height, gray)
        End Function

        ''' <summary>
        ''' Tensor will be trimmed to stay in R range, then remapped to 0 to 255.
        ''' </summary>
        ''' <param name="A">Tensor 2D</param>
        ''' <param name="R"></param>
        ''' <returns></returns>
        Public Function ToBitmap(A As Tensor, R As Range) As Bitmap
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