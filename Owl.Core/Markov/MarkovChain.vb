Namespace Probability
    Public Class MarkovChain(Of T As {IComparable})

        Dim m_dict As New SortedList(Of T, Integer)
        Dim m_revDict As New SortedList(Of Integer, T)

        Dim m_sums() As Integer
        Dim m_Xsums() As Integer
        Dim m_xsum As Integer

        Dim m_prob(,) As Integer
        Dim m_rnd As Random = Nothing

        Sub New(Dictionary As IEnumerable(Of T), Seed As Integer)
            Dim hs As New HashSet(Of T)

            For i As Integer = 0 To Dictionary.Count - 1 Step 1
                hs.Add(Dictionary(i))
            Next

            For i As Integer = 0 To hs.Count - 1 Step 1
                m_dict.Add(hs(i), i)
                m_revDict.Add(i, hs(i))
            Next

            ReDim m_prob(m_dict.Count - 1, m_dict.Count - 1)
            ReDim m_sums(m_dict.Count - 1)
            ReDim m_Xsums(m_dict.Count - 1)
            m_rnd = New Random(Seed)
        End Sub

        Sub AddChain(Chain As IEnumerable(Of T))

            For i As Integer = 1 To Chain.Count - 1 Step 1
                Dim thisk As T = Chain(i - 1)
                Dim thisv As T = Chain(i)
                m_prob(m_dict(thisk), m_dict(thisv)) += 1
                m_sums(m_dict(thisk)) += 1
                m_Xsums(m_dict(thisv)) += 1
                m_xsum += 1
            Next

        End Sub

        Public Function GetNext(This As T, ByRef Choosen As T) As Boolean

            Dim sum As Integer = m_sums(m_dict(This))
            If sum = 0 Then Return False
            Dim value As Double = m_rnd.NextDouble * sum

            Dim adding As Integer = 0
            Dim thisi As Integer = m_dict(This)

            Dim found As Boolean = False

            For i As Integer = 0 To m_prob.GetUpperBound(1) Step 1
                Dim thisprob As Integer = m_prob(thisi, i)
                adding += thisprob
                If adding >= value Then
                    Choosen = m_revDict(i)
                    found = True
                    Exit For
                End If
            Next

            Return found

        End Function

        Public Function GetSeries(Start As T, Count As Integer, Optional PreventDeadEnds As Boolean = True) As List(Of T)
            Dim ser As New List(Of T)

            ser.Add(Start)

            For i As Integer = 1 To Count - 1 Step 1
                Dim thisnext As T
                If Me.GetNext(ser(i - 1), thisnext) Then
                    ser.Add(thisnext)
                Else
                    If PreventDeadEnds Then
                        ser.Add(ChooseRandom())
                    Else
                        Exit For
                    End If
                End If
            Next

            Return ser
        End Function

        Public Function ChooseRandom() As T
            Dim value As Double = m_rnd.NextDouble * m_xsum
            Dim adding As Integer = 0

            For i As Integer = 0 To m_Xsums.Length - 1 Step 1
                Dim thisprob As Integer = m_Xsums(i)
                adding += thisprob
                If adding >= value Then
                    Return m_revDict(i)
                End If
            Next

            Return Nothing
        End Function

        Public ReadOnly Property Dictionary As SortedList(Of T, Integer)
            Get
                Return m_dict
            End Get
        End Property

        Public ReadOnly Property Probabilities As Integer(,)
            Get
                Return m_prob
            End Get
        End Property

    End Class
End Namespace
