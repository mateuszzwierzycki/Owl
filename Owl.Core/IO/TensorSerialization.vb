Imports System
Imports System.Globalization
Imports System.IO
Imports Owl.Core.Tensors

Namespace IO

    Public Module TensorSerialization

#Region "Streams"

        ''' <summary>
        ''' Read binary Tensors (using the TBIN format) from any stream.
        ''' </summary>
        ''' <param name="S"></param>
        ''' <returns></returns>
        Public Function ReadTensors(S As Stream) As TensorSet
            Dim ts As New TensorSet

            Using reader As BinaryReader = New BinaryReader(S)
                Do
                    Dim buffer(3) As Byte
                    If reader.Read(buffer, 0, 4) < 4 Then Exit Do 'this reads the tensor bytelength, here it is skipped
                    reader.Read(buffer, 0, 4) 'this reads the first int32, the shapecount

                    Dim shapecount As Integer = BitConverter.ToInt32(buffer, 0)
                    Dim shape As New List(Of Integer)

                    For i As Integer = 0 To shapecount - 1 Step 1
                        reader.Read(buffer, 0, 4)
                        shape.Add(BitConverter.ToInt32(buffer, 0))
                    Next

                    Dim thistens As New Tensor(shape)
                    ReDim buffer(7)

                    For i As Integer = 0 To thistens.Length - 1 Step 1
                        reader.Read(buffer, 0, 8)
                        thistens(i) = BitConverter.ToDouble(buffer, 0)
                    Next

                    ts.Add(thistens)
                Loop
            End Using

            Return ts
        End Function

        ''' <summary>
        ''' Write binary Tensors to any stream usign the TBIN file format.
        ''' </summary>
        ''' <param name="S"></param>
        ''' <param name="Tensors"></param>
        ''' <returns></returns>
        Public Function WriteTensors(S As Stream, Tensors As TensorSet) As Boolean

            For Each tens As Tensor In Tensors
                Dim cnt As Integer = 4 + tens.ShapeCount * 4 + tens.Length * 8
                S.Write(BitConverter.GetBytes(cnt), 0, 4)

                S.Write(BitConverter.GetBytes(tens.ShapeCount), 0, 4)
                For i As Integer = 0 To tens.ShapeCount - 1 Step 1
                    S.Write(BitConverter.GetBytes(tens.ShapeAt(i)), 0, 4)
                Next

                For i As Integer = 0 To tens.Length - 1 Step 1
                    S.Write(BitConverter.GetBytes(tens(i)), 0, 8)
                Next
            Next

            Return True
        End Function

#End Region

        ''' <summary>
        ''' Assuming path "C:\tmp\tensorfile.tbin 30 40" this function returns 30 and 40 as the tuple.. index from inclusive and count.
        ''' </summary>
        ''' <param name="Text"></param>
        ''' <returns></returns>
        Function TryToFindIndices(Text As String, ByRef Final As String) As Tuple(Of Integer, Integer)
            Dim spl() As String = Text.Split(" ")
            Dim emptyint As New Tuple(Of Integer, Integer)(-1, -1)
            If spl.Length <> 3 Then Return emptyint

            Dim fromi As Integer
            Dim timi As Integer


            If Not Integer.TryParse(spl(1), fromi) Then Return emptyint
            If Not Integer.TryParse(spl(2), timi) Then Return emptyint

            Dim fake As Integer
            If Integer.TryParse(spl(0), fake) Then Return emptyint

            Final = spl(0)
            Return New Tuple(Of Integer, Integer)(fromi, timi)
        End Function

        Public Function GetExtension(Format As OwlFileFormat) As String

            Select Case Format
                Case OwlFileFormat.IDX
                    Return ".idx"
                Case OwlFileFormat.TensorImage
                    Return ".timg"
                Case OwlFileFormat.TensorBinary
                    Return ".tbin"
                Case OwlFileFormat.TensorText
                    Return ".ttxt"
            End Select

            Return ""
        End Function

        '''
        Public Enum OwlFileFormat As Integer
            None = -1
            ''' <summary>
            ''' Binary data.
            ''' </summary>
            TensorBinary = 0
            ''' <summary>
            ''' Nicely formatted CSV file.
            ''' </summary>
            TensorText = 1
            'TODO export bitmaps
            TensorImage = 2 'bitmap
            ''' <summary>
            ''' Format used by the MNIST database.
            ''' </summary>
            IDX = 3 'mnist format

            All = Integer.MaxValue
        End Enum

#Region "IDX"

        ''' <summary>
        ''' Load a subset of a TensorSet from a file.
        ''' </summary>
        ''' <param name="Filepath"></param>
        ''' <param name="From"></param>
        ''' <param name="Count"></param>
        ''' <returns></returns>
        Public Function LoadTensorsIDX(Filepath As String, From As Integer, Count As Integer) As TensorSet
            Dim tset As New TensorSet

            Using str As FileStream = New FileStream(Filepath, FileMode.Open)
                Using reader As BinaryReader = New BinaryReader(str)
                    'The magic number Is an integer (MSB first). The first 2 bytes are always 0.
                    'The third byte codes the type of the data
                    '0x08: unsigned Byte 
                    '0x09: signed Byte 
                    '0x0B: Short(2 bytes)
                    '0x0C: Int(4 bytes)
                    '0x0D: float(4 bytes)
                    '0x0E: Double(8 bytes)
                    'The 4 - th byte codes the number of dimensions of the vector/matrix:  1 for vectors, 2 for matrices....

                    'header
                    Dim buffer(3) As Byte
                    If reader.Read(buffer, 0, 4) < 4 Then Return Nothing 'this reads the tensor bytelength, here it is skipped

                    Dim datatype As Type = GetType(Byte)
                    Dim datadim As Byte = buffer(3)
                    Dim databytecount As Byte

                    Select Case buffer(2)
                        Case &H8
                            datatype = GetType(Byte)
                            databytecount = 1
                        Case &H9
                            datatype = GetType(SByte)
                            databytecount = 1
                        Case &HB
                            datatype = GetType(Short)
                            databytecount = 2
                        Case &HC
                            datatype = GetType(Int32)
                            databytecount = 4
                        Case &HD
                            datatype = GetType(Single)
                            databytecount = 4
                        Case &HE
                            datatype = GetType(Double)
                            databytecount = 8
                    End Select

                    Dim dims(datadim - 1) As Int32
                    Dim tensorlength As Long = 1
                    Dim tensorcount As Integer = 0
                    Dim shape As New List(Of Integer)

                    For i As Integer = 0 To datadim - 1 Step 1
                        Dim intbuf(3) As Byte
                        reader.Read(intbuf, 0, 4)
                        Dim asl As List(Of Byte) = intbuf.ToList
                        asl.Reverse()
                        dims(i) = BitConverter.ToInt32(asl.ToArray, 0)

                        If i > 0 Then shape.Add(dims(i))
                        If i > 0 Then tensorlength *= CLng(dims(i))
                    Next
                    tensorcount = Count

                    Dim dataraw(tensorcount * tensorlength * CInt(databytecount) - 1) As Byte

                    'seek to the reading position
                    reader.BaseStream.Position += From * tensorlength * CInt(databytecount)

                    reader.Read(dataraw, 0, dataraw.Length)
                    Array.Reverse(dataraw)

                    Dim data As Array = Array.CreateInstance(datatype, tensorlength * tensorcount)
                    System.Buffer.BlockCopy(dataraw, 0, data, 0, dataraw.Length)
                    Array.Reverse(data)

                    Dim pos As Integer = 0
                    For i As Integer = 0 To tensorcount - 1 Step 1
                        Dim tens As New Tensor(shape)
                        Array.Copy(data, pos, tens.TensorData, 0, tensorlength)
                        tens.TryReshape(shape)
                        pos += tensorlength
                        tset.Add(tens)
                    Next

                End Using
            End Using


            Return tset
        End Function

        ''' <summary>
        ''' Load the TensorSet from a file.
        ''' </summary>
        ''' <param name="Filepath"></param>
        ''' <returns></returns>
        Public Function LoadTensorsIDX(Filepath As String) As TensorSet
            Dim tset As New TensorSet

            Using str As FileStream = New FileStream(Filepath, FileMode.Open)
                Using reader As BinaryReader = New BinaryReader(str)
                    'The magic number Is an integer (MSB first). The first 2 bytes are always 0.
                    'The third byte codes the type of the data
                    '0x08: unsigned Byte 
                    '0x09: signed Byte 
                    '0x0B: Short(2 bytes)
                    '0x0C: Int(4 bytes)
                    '0x0D: float(4 bytes)
                    '0x0E: Double(8 bytes)
                    'The 4 - th byte codes the number of dimensions of the vector/matrix:  1 for vectors, 2 for matrices....

                    'header
                    Dim buffer(3) As Byte
                    If reader.Read(buffer, 0, 4) < 4 Then Return Nothing 'this reads the tensor bytelength, here it is skipped

                    Dim datatype As Type = GetType(Byte)
                    Dim datadim As Byte = buffer(3)
                    Dim databytecount As Byte

                    Select Case buffer(2)
                        Case &H8
                            datatype = GetType(Byte)
                            databytecount = 1
                        Case &H9
                            datatype = GetType(SByte)
                            databytecount = 1
                        Case &HB
                            datatype = GetType(Short)
                            databytecount = 2
                        Case &HC
                            datatype = GetType(Int32)
                            databytecount = 4
                        Case &HD
                            datatype = GetType(Single)
                            databytecount = 4
                        Case &HE
                            datatype = GetType(Double)
                            databytecount = 8
                    End Select

                    Dim dims(datadim - 1) As Int32
                    Dim tensorlength As Long = 1
                    Dim tensorcount As Integer = 0
                    Dim shape As New List(Of Integer)

                    For i As Integer = 0 To datadim - 1 Step 1
                        Dim intbuf(3) As Byte
                        reader.Read(intbuf, 0, 4)
                        Dim asl As List(Of Byte) = intbuf.ToList
                        asl.Reverse()
                        dims(i) = BitConverter.ToInt32(asl.ToArray, 0)

                        If i > 0 Then shape.Add(dims(i))
                        If i > 0 Then tensorlength *= CLng(dims(i))
                    Next
                    tensorcount = dims(0)

                    Dim dataraw(tensorcount * tensorlength * CInt(databytecount) - 1) As Byte
                    reader.Read(dataraw, 0, dataraw.Length)

                    Array.Reverse(dataraw)

                    Dim data As Array = Array.CreateInstance(datatype, tensorlength * tensorcount)
                    System.Buffer.BlockCopy(dataraw, 0, data, 0, dataraw.Length)
                    Array.Reverse(data)

                    Dim pos As Integer = 0
                    For i As Integer = 0 To tensorcount - 1 Step 1
                        Dim tens As New Tensor(shape)
                        Array.Copy(data, pos, tens.TensorData, 0, tensorlength)
                        tens.TryReshape(shape)
                        pos += tensorlength
                        tset.Add(tens)
                    Next

                End Using
            End Using


            Return tset
        End Function

        ''' <summary>
        ''' Read the whole file as one big Tensor. The first dimension of the Tensor indicates the "sample"
        ''' Tensor with Shape = (100,10,10) is 100 samples of 2D 10x10 Tensors.
        ''' </summary>
        ''' <param name="Filepath"></param>
        ''' <returns></returns>
        Public Function LoadTensorIDX(Filepath As String) As Tensor

            Dim ts As Tensor = Nothing

            Using str As FileStream = New FileStream(Filepath, FileMode.Open)
                Using reader As BinaryReader = New BinaryReader(str)
                    'The magic number Is an integer (MSB first). The first 2 bytes are always 0.
                    'The third byte codes the type of the data
                    '0x08: unsigned Byte 
                    '0x09: signed Byte 
                    '0x0B: Short(2 bytes)
                    '0x0C: Int(4 bytes)
                    '0x0D: float(4 bytes)
                    '0x0E: Double(8 bytes)
                    'The 4 - th byte codes the number of dimensions of the vector/matrix:  1 for vectors, 2 for matrices....

                    'header
                    Dim buffer(3) As Byte
                    If reader.Read(buffer, 0, 4) < 4 Then Return Nothing 'this reads the tensor bytelength, here it is skipped

                    Dim datatype As Type = GetType(Byte)
                    Dim datadim As Byte = buffer(3)
                    Dim databytecount As Byte

                    Select Case buffer(2)
                        Case &H8
                            datatype = GetType(Byte)
                            databytecount = 1
                        Case &H9
                            datatype = GetType(SByte)
                            databytecount = 1
                        Case &HB
                            datatype = GetType(Short)
                            databytecount = 2
                        Case &HC
                            datatype = GetType(Int32)
                            databytecount = 4
                        Case &HD
                            datatype = GetType(Single)
                            databytecount = 4
                        Case &HE
                            datatype = GetType(Double)
                            databytecount = 8
                    End Select

                    Dim dims(datadim - 1) As Int32
                    Dim datalength As Long = 1

                    For i As Integer = 0 To datadim - 1 Step 1
                        Dim intbuf(3) As Byte
                        reader.Read(intbuf, 0, 4)
                        Dim asl As List(Of Byte) = intbuf.ToList
                        asl.Reverse()
                        dims(i) = BitConverter.ToInt32(asl.ToArray, 0)
                        datalength *= CLng(dims(i))
                    Next

                    Dim dataraw(datalength * CInt(databytecount) - 1) As Byte
                    reader.Read(dataraw, 0, dataraw.Length)
                    Array.Reverse(dataraw)

                    Dim data As Array = Array.CreateInstance(datatype, datalength)
                    System.Buffer.BlockCopy(dataraw, 0, data, 0, dataraw.Length)
                    Array.Reverse(data)

                    Dim dbl As New List(Of Double)
                    For i As Long = 0 To data.Length - 1 Step 1
                        dbl.Add(CDbl(data(i)))
                    Next

                    ts = New Tensor(dims, dbl)
                End Using
            End Using

            Return ts
        End Function

        ''' <summary>
        ''' Will throw an error if the Tensor value is not within the DataType range. Remap the Tensor beforehand if you're not sure about the range of your data.
        ''' </summary>
        ''' <param name="Tensors"></param>
        ''' <param name="DirectoryName"></param>
        ''' <param name="FileName"></param>
        ''' <param name="DataType"></param>
        ''' <returns></returns>
        Public Function SaveTensorsIDX(Tensors As TensorSet, DirectoryName As String, FileName As String, DataType As Type) As Boolean
            If Not Tensors.IsHomogeneous Then Return False
            If Not Directory.Exists(DirectoryName) Then Directory.CreateDirectory(DirectoryName)

            Using str As FileStream = New FileStream(DirectoryName & "\" & FileName & GetExtension(OwlFileFormat.IDX), FileMode.Create, FileAccess.Write)

                str.WriteByte(&H0)
                str.WriteByte(&H0)

                Dim typebyte As Byte
                Dim databytecount As Byte

                Select Case DataType
                    Case GetType(Byte)
                        typebyte = &H8
                        databytecount = 1
                    Case GetType(SByte)
                        typebyte = &H9
                        databytecount = 1
                    Case GetType(Short)
                        typebyte = &HB
                        databytecount = 2
                    Case GetType(Int32)
                        typebyte = &HC
                        databytecount = 4
                    Case GetType(Single)
                        typebyte = &HD
                        databytecount = 4
                    Case GetType(Double)
                        typebyte = &HE
                        databytecount = 8
                End Select

                str.WriteByte(typebyte)

                str.WriteByte(CByte(Tensors(0).ShapeCount + 1))

                str.Write(BitConverter.GetBytes(Convert.ToInt32(Tensors.Count)).Reverse.ToArray, 0, 4)

                For i As Integer = 0 To Tensors(0).ShapeCount - 1 Step 1
                    str.Write(BitConverter.GetBytes(Convert.ToInt32(Tensors(0).ShapeAt(i))).Reverse.ToArray, 0, 4)
                Next

                Dim nc As Object = Nothing

                Select Case DataType
                    Case GetType(Byte)
                        nc = New Converter(Of Double, Byte)(Function(input As Double) As Byte
                                                                Return CByte(input)
                                                            End Function)
                    Case GetType(SByte)
                        nc = New Converter(Of Double, SByte)(Function(input As Double) As SByte
                                                                 Return CSByte(input)
                                                             End Function)
                    Case GetType(Short)
                        nc = New Converter(Of Double, Short)(Function(input As Double) As Short
                                                                 Return CShort(input)
                                                             End Function)
                    Case GetType(Int32)
                        nc = New Converter(Of Double, Int32)(Function(input As Double) As Int32
                                                                 Return CInt(input)
                                                             End Function)
                    Case GetType(Single)
                        nc = New Converter(Of Double, Single)(Function(input As Double) As Single
                                                                  Return CSng(input)
                                                              End Function)
                    Case GetType(Double)
                        'no conversion 
                End Select

                If DataType = GetType(Double) Then
                    Dim bytes(databytecount * Tensors(0).Length - 1) As Byte

                    For Each tens As Tensor In Tensors
                        Dim typed(tens.Length - 1) As Double
                        tens.TensorData.CopyTo(typed, 0)
                        Array.Reverse(typed)
                        Buffer.BlockCopy(typed, 0, bytes, 0, databytecount * tens.Length)
                        Array.Reverse(bytes)
                        str.Write(bytes, 0, bytes.Length)
                    Next

                Else

                    Dim typed As Array = Array.CreateInstance(DataType, Tensors(0).Length)
                    Dim bytes(databytecount * Tensors(0).Length - 1) As Byte

                    For Each tens As Tensor In Tensors
                        typed = Array.ConvertAll(tens.TensorData, nc)
                        Array.Reverse(typed)
                        Buffer.BlockCopy(typed, 0, bytes, 0, databytecount * tens.Length)
                        Array.Reverse(bytes)
                        str.Write(bytes, 0, bytes.Length)
                    Next

                End If

            End Using

            Return True
        End Function

#End Region

#Region "TTXT"

        Public Sub SaveTensorsText(Tensors As TensorSet, DirectoryName As String, FileName As String, Optional Separator As Char = " ")
            Using str As StreamWriter = New StreamWriter(DirectoryName & "\" & FileName & GetExtension(OwlFileFormat.TensorText), False)

                For i As Integer = 0 To Tensors.Count - 1
                    Dim tens As Tensor = Tensors(i)

                    Dim thisline As String = ""

                    thisline &= (tens.ShapeCount.ToString(CultureInfo.InvariantCulture))
                    thisline &= (Separator)

                    For j As Integer = 0 To tens.ShapeCount - 1 Step 1
                        thisline &= (tens.ShapeAt(j).ToString(CultureInfo.InvariantCulture))
                        thisline &= (Separator)
                    Next

                    For j As Integer = 0 To tens.Length - 1 Step 1
                        thisline &= (tens(j).ToString(CultureInfo.InvariantCulture))
                        If j < tens.Length - 1 Then thisline &= (Separator)
                    Next

                    str.WriteLine(thisline)
                Next

            End Using
        End Sub

        ''' <summary>
        ''' Use "*" to interpret a first non-digit char as the separator.
        ''' </summary>
        ''' <param name="Filepath"></param>
        ''' <param name="Separator"></param>
        ''' <returns></returns>
        Function LoadTensorsText(Filepath As String, Optional Separator As Char = " ") As TensorSet
            Dim thisculture As CultureInfo = My.Application.Culture
            My.Application.ChangeCulture(CultureInfo.InvariantCulture.Name)

            Dim alltext As New List(Of String)

            Using str As StreamReader = New StreamReader(Filepath)
                Do
                    If str.EndOfStream Then Exit Do
                    alltext.Add(str.ReadLine)
                Loop
            End Using

            Dim alltens(alltext.Count - 1) As Tensor

            If Separator = "*" Then
                Dim frs As String = alltext(0)
                For Each c As Char In frs
                    If Not Char.IsDigit(c) Then Separator = c : Exit For
                Next
            End If

            Parallel.For(0, alltext.Count, Sub(index As Integer)
                                               Dim thistens As Tensor = Nothing
                                               If Tensor.TryParse(alltext(index), thistens, Separator) Then
                                                   alltens(index) = thistens
                                               End If
                                           End Sub)

            Dim ts As New TensorSet
            For i As Integer = 0 To alltens.Length - 1 Step 1
                If alltens(i) IsNot Nothing Then ts.Add(alltens(i))
            Next

            My.Application.ChangeCulture(thisculture.Name)
            Return ts
        End Function

        ''' <summary>
        ''' Use "*" to interpret a first non-digit char as the separator.
        ''' </summary>
        ''' <param name="Filepath"></param>
        ''' <param name="Separator"></param>
        ''' <returns></returns>
        Function LoadTensorsText(Filepath As String, From As Integer, Count As Integer, Optional Separator As Char = " ") As TensorSet
            Dim thisculture As CultureInfo = My.Application.Culture
            My.Application.ChangeCulture(CultureInfo.InvariantCulture.Name)

            Dim alltext As New List(Of String)

            Using str As StreamReader = New StreamReader(Filepath)
                Dim cnt As Integer = 0


                Do
                    If str.EndOfStream Then Exit Do
                    Dim thisline As String = str.ReadLine
                    If cnt >= From And cnt < (From + Count) Then
                        alltext.Add(thisline)
                    End If
                    cnt += 1
                Loop

            End Using

            Dim alltens(alltext.Count - 1) As Tensor

            If Separator = "*" Then
                Dim frs As String = alltext(0)
                For Each c As Char In frs
                    If Not Char.IsDigit(c) Then Separator = c : Exit For
                Next
            End If

            Parallel.For(0, alltext.Count, Sub(index As Integer)
                                               Dim thistens As Tensor = Nothing
                                               If Tensor.TryParse(alltext(index), thistens, Separator) Then
                                                   alltens(index) = thistens
                                               End If
                                           End Sub)

            Dim ts As New TensorSet
            For i As Integer = 0 To alltens.Length - 1 Step 1
                If alltens(i) IsNot Nothing Then ts.Add(alltens(i))
            Next

            My.Application.ChangeCulture(thisculture.Name)
            Return ts
        End Function

#End Region

#Region "TBIN"

        ''' <summary>
        ''' For each tensor:
        '''     4 bytes int32 length of the tensor in bytes (4 bytes for shapecount + shapecount * 4 bytes + length * 8)
        '''     4 bytes int32 shapecount
        '''     4*shapecount int32 shape
        '''     8*length 
        ''' </summary>
        ''' <param name="Tensors"></param>
        ''' <param name="DirectoryName"></param>
        ''' <param name="FileName"></param>
        Public Sub SaveTensorsBinary(Tensors As TensorSet, DirectoryName As String, FileName As String)
            If Not Directory.Exists(DirectoryName) Then Directory.CreateDirectory(DirectoryName)

            Using str As FileStream = New FileStream(DirectoryName & "\" & FileName & GetExtension(OwlFileFormat.TensorBinary), FileMode.Create, FileAccess.Write)
                For Each tens As Tensor In Tensors
                    Dim cnt As Integer = 4 + tens.ShapeCount * 4 + tens.Length * 8
                    str.Write(BitConverter.GetBytes(cnt), 0, 4)

                    str.Write(BitConverter.GetBytes(tens.ShapeCount), 0, 4)
                    For i As Integer = 0 To tens.ShapeCount - 1 Step 1
                        str.Write(BitConverter.GetBytes(tens.ShapeAt(i)), 0, 4)
                    Next

                    For i As Integer = 0 To tens.Length - 1 Step 1
                        str.Write(BitConverter.GetBytes(tens(i)), 0, 8)
                    Next
                Next
            End Using
        End Sub

        ''' <summary>
        ''' Counts the number of tensors stored in a file
        ''' </summary>
        ''' <param name="Filepath"></param>
        ''' <returns></returns>
        Function CountTensorsBinary(Filepath As String) As Long
            Dim lng As Long = 0
            Using str As FileStream = New FileStream(Filepath, FileMode.Open)
                Using reader As BinaryReader = New BinaryReader(str)
                    Do
                        Dim buffer(3) As Byte
                        If reader.Read(buffer, 0, 4) < 4 Then Exit Do 'this reads the tensor bytelength, here it is skipped
                        reader.BaseStream.Position += BitConverter.ToInt32(buffer, 0)
                        lng += 1
                    Loop
                End Using
            End Using
            Return lng
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Filepath"></param>
        ''' <param name="FromIndex">Start reading from this tensor</param>
        ''' <param name="Count">Read that many tensors (or less if not enough)</param>
        ''' <returns></returns>
        Function LoadTensorsBinary(Filepath As String, FromIndex As Integer, Count As Integer) As TensorSet
            Dim ts As New TensorSet

            Dim readcount As Integer = 0
            Dim position As Integer = 0

            Using str As FileStream = New FileStream(Filepath, FileMode.Open)
                Using reader As BinaryReader = New BinaryReader(str)
                    Do
                        Dim buffer(3) As Byte
                        If reader.Read(buffer, 0, 4) < 4 Then Exit Do
                        Dim lng As Integer = BitConverter.ToInt32(buffer, 0)

                        If position >= FromIndex Then
                            reader.Read(buffer, 0, 4) 'this reads the first int32, the shapecount

                            Dim shapecount As Integer = BitConverter.ToInt32(buffer, 0)
                            Dim shape As New List(Of Integer)

                            For i As Integer = 0 To shapecount - 1 Step 1
                                reader.Read(buffer, 0, 4)
                                shape.Add(BitConverter.ToInt32(buffer, 0))
                            Next

                            Dim thistens As New Tensor(shape)
                            ReDim buffer(7)

                            For i As Integer = 0 To thistens.Length - 1 Step 1
                                reader.Read(buffer, 0, 8)
                                thistens(i) = BitConverter.ToDouble(buffer, 0)
                            Next

                            ts.Add(thistens)
                            readcount += 1
                        Else
                            str.Position += lng
                        End If

                        position += 1
                        If readcount >= Count Then Exit Do
                    Loop
                End Using
            End Using

            Return ts
        End Function

        Function LoadTensorsBinary(Filepath As String) As TensorSet
            Dim ts As New TensorSet

            Using str As FileStream = New FileStream(Filepath, FileMode.Open)
                Using reader As BinaryReader = New BinaryReader(str)
                    Do
                        Dim buffer(3) As Byte
                        If reader.Read(buffer, 0, 4) < 4 Then Exit Do 'this reads the tensor bytelength, here it is skipped
                        reader.Read(buffer, 0, 4) 'this reads the first int32, the shapecount

                        Dim shapecount As Integer = BitConverter.ToInt32(buffer, 0)
                        Dim shape As New List(Of Integer)

                        For i As Integer = 0 To shapecount - 1 Step 1
                            reader.Read(buffer, 0, 4)
                            shape.Add(BitConverter.ToInt32(buffer, 0))
                        Next

                        Dim thistens As New Tensor(shape)
                        ReDim buffer(7)

                        For i As Integer = 0 To thistens.Length - 1 Step 1
                            reader.Read(buffer, 0, 8)
                            thistens(i) = BitConverter.ToDouble(buffer, 0)
                        Next

                        ts.Add(thistens)
                    Loop
                End Using
            End Using

            Return ts
        End Function

#End Region


    End Module

End Namespace
