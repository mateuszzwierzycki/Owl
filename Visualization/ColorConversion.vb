Imports System.Drawing

Namespace Visualization

    ''' <summary>
    ''' Source: https://www.programmingalgorithms.com/algorithm/hsl-to-rgb?lang=VB.Net
    ''' </summary>
    Module ColorConversion
        Public Function HSLToRGB(hsl As ColorHSL) As Color
            Dim r As Byte = 0
            Dim g As Byte = 0
            Dim b As Byte = 0

            If hsl.S = 0 Then
                r = CByte(Math.Truncate(hsl.L * 255))
                g = CByte(Math.Truncate(hsl.L * 255))
                b = CByte(Math.Truncate(hsl.L * 255))
            Else
                Dim v1 As Single, v2 As Single
                Dim hue As Single = CSng(hsl.H) / 360

                v2 = If((hsl.L < 0.5), (hsl.L * (1 + hsl.S)), ((hsl.L + hsl.S) - (hsl.L * hsl.S)))
                v1 = 2 * hsl.L - v2

                r = CByte(Math.Truncate(255 * HueToRGB(v1, v2, hue + (1.0F / 3))))
                g = CByte(Math.Truncate(255 * HueToRGB(v1, v2, hue)))
                b = CByte(Math.Truncate(255 * HueToRGB(v1, v2, hue - (1.0F / 3))))
            End If

            Return Color.FromArgb(hsl.A * 255, r, g, b)
        End Function

        Private Function HueToRGB(v1 As Single, v2 As Single, vH As Single) As Single
            If vH < 0 Then
                vH += 1
            End If

            If vH > 1 Then
                vH -= 1
            End If

            If (6 * vH) < 1 Then
                Return (v1 + (v2 - v1) * 6 * vH)
            End If

            If (2 * vH) < 1 Then
                Return v2
            End If

            If (3 * vH) < 2 Then
                Return (v1 + (v2 - v1) * ((2.0F / 3) - vH) * 6)
            End If

            Return v1
        End Function

    End Module

End Namespace
