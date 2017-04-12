Imports System.Drawing

Friend Module Utils

    Friend Function GetIcon(comp As Grasshopper.Kernel.GH_ActiveObject) As Bitmap
        Dim s As String = comp.NickName
        Dim b As New Bitmap(24, 24)

        Using g As Graphics = Graphics.FromImage(b)
            g.TextRenderingHint = Text.TextRenderingHint.AntiAlias
            g.DrawString(s, New Font(FontFamily.GenericSansSerif, 6), Brushes.Black, New RectangleF(-1, -1, 26, 26))
        End Using

        Return b

    End Function

End Module
