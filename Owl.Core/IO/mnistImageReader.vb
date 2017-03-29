Imports System.IO
Imports Owl.Images
Imports Owl.Core.Tensors

Namespace IO
    Namespace MNIST

        ''' <summary>
        ''' The tensors created by this class are mapped onto the 0-255 range.
        ''' </summary>
        Public Class mnistImageReader

            Private m_path As String = ""
            Private m_data()(,) As Byte

            Private m_magicNumber As Int32
            Private m_imageCount As Int32
            Private m_rows As Int32
            Private m_cols As Int32

            Sub New(Filepath As String) 'remove the readheader param
                m_path = Filepath
            End Sub

            Public Function Read(From As Integer, [To] As Integer) As TensorSet
                ReadHeader()

                If [To] = -1 Then [To] = m_imageCount - 1

                Dim bmpSize As Integer = m_rows * m_cols

                Dim outs As New TensorSet()

                Using fs As FileStream = New FileStream(m_path, FileMode.Open)
                    Using br As BinaryReader = New BinaryReader(fs)
                        fs.Position = 16 + From * bmpSize

                        For i As Integer = From To [To] Step 1
                            Dim buff(bmpSize - 1) As Byte
                            br.Read(buff, 0, bmpSize)
                            outs.Add(New Tensor(m_cols, m_rows, buff))
                        Next

                    End Using
                End Using

                Return outs
            End Function

            Public Function ReadAll() As TensorSet
                Return Read(0, m_imageCount - 1)
            End Function

            ''' <summary>
            ''' Taken from the file header - call ReadHeader to get this value without reading all the images.
            ''' </summary>
            ''' <returns></returns>
            Public ReadOnly Property Rows As Integer
                Get
                    Return m_rows
                End Get
            End Property

            ''' <summary>
            ''' Taken from the file header - call ReadHeader to get this value without reading all the images.
            ''' </summary>
            ''' <returns></returns>
            Public ReadOnly Property Columns As Integer
                Get
                    Return m_cols
                End Get
            End Property

            Public Sub ReadHeader()
                Using fs As FileStream = New FileStream(m_path, FileMode.Open)
                    Using br As BinaryReader = New BinaryReader(fs)
                        m_magicNumber = SwapEndianness(br.ReadInt32())
                        m_imageCount = SwapEndianness(br.ReadInt32())
                        m_rows = SwapEndianness(br.ReadInt32())
                        m_cols = SwapEndianness(br.ReadInt32())
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
