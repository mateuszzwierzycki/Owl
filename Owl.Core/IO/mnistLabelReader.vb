Imports System.IO

Namespace IO
    Namespace MNIST
        Public Class mnistLabelReader

            Private m_path As String = ""
            Private m_data()(,) As Byte

            Private m_magicNumber As Int32
            Private m_labelcount As Int32

            Sub New(Filepath As String)
                m_path = Filepath
            End Sub

            Public Function Read(From As Integer, [To] As Integer) As List(Of Byte)
                ReadHeader()

                If [To] = -1 Then [To] = m_labelcount - 1

                Dim outs As New List(Of Byte)

                Using fs As FileStream = New FileStream(m_path, FileMode.Open)
                    Using br As BinaryReader = New BinaryReader(fs)
                        fs.Position = 8 + From

                        Dim buff([To] - From) As Byte
                        br.Read(buff, 0, [To] - From)
                        outs.AddRange(buff)

                    End Using
                End Using

                Return outs
            End Function

            Public Function ReadAll() As List(Of Byte)
                Return Read(0, m_labelcount - 1)
            End Function

            Public Sub ReadHeader()
                Using fs As FileStream = New FileStream(m_path, FileMode.Open)
                    Using br As BinaryReader = New BinaryReader(fs)
                        m_magicNumber = SwapEndianness(br.ReadInt32())
                        m_labelcount = SwapEndianness(br.ReadInt32())
                    End Using
                End Using
            End Sub

            Private Function SwapEndianness(value As Int32) As Int32
                Dim b1 = (value >> 0) And &HFF
                Dim b2 = (value >> 8) And &HFF
                Dim b3 = (value >> 16) And &HFF
                Dim b4 = (value >> 24) And &HFF

                Return b1 << 24 Or b2 << 16 Or b3 << 8 Or b4 << 0
            End Function

        End Class

    End Namespace
End Namespace
