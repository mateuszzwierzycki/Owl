Imports Owl.Core.Tensors

Namespace Convolutions

    Public Class ImageNetwork
        Inherits System.Collections.CollectionBase
        Implements IDisposable
        Implements IEnumerable(Of ImageModifierLayer)

        Sub New()

        End Sub

        Sub New(Other As ImageNetwork)
            For i As Integer = 0 To Other.Count - 1 Step 1
                Me.Add(Other(i).Duplicate)
            Next
        End Sub

        Public Function Duplicate() As ImageNetwork
            Return New ImageNetwork(Me)
        End Function

        Public Sub Add(Value As ImageModifierLayer)
            List.Add(Value)
        End Sub

        Public Sub AddRange(Values As IEnumerable(Of ImageModifierLayer))
            InnerList.AddRange(Values)
        End Sub

        Sub Remove(index As Integer)
            List.RemoveAt(index)
        End Sub

        Default Public Property Item(index As Integer) As ImageModifierLayer
            Get
                Return List.Item(index)
            End Get
            Set(value As ImageModifierLayer)
                List(index) = value
            End Set
        End Property

        Public Function Apply(Image As Tensor) As TensorSet
            Dim thisset As New TensorSet()
            thisset.Add(Image.Duplicate)

            For i As Integer = 0 To Me.Count - 1 Step 1
                Me(i).Apply(thisset)
            Next

            Return thisset
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    For i As Integer = 0 To Me.Count - 1 Step 1
                        Me(i).Dispose()
                    Next
                End If
            End If
            disposedValue = True
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
        End Sub
#End Region

        Private Function IEnumerable_GetEnumerator() As IEnumerator(Of ImageModifierLayer) Implements IEnumerable(Of ImageModifierLayer).GetEnumerator
            Throw New NotImplementedException()
        End Function

    End Class

    Public Class ImageNetworkEnumerator
        Implements IEnumerator(Of ImageModifierLayer)

        Private m_n As ImageNetwork = Nothing
        Dim ind As Integer = 0

        Sub New(Net As ImageNetwork)
            m_n = Net
        End Sub

        Public ReadOnly Property Current As ImageModifierLayer Implements IEnumerator(Of ImageModifierLayer).Current
            Get
                Return m_n(ind)
            End Get
        End Property

        Private ReadOnly Property IEnumerator_Current As Object Implements IEnumerator.Current
            Get
                Return Me.Current
            End Get
        End Property

        Public Sub Reset() Implements IEnumerator.Reset
            ind = -1
        End Sub

        Public Function MoveNext() As Boolean Implements IEnumerator.MoveNext
            ind += 1
            If ind = Me.m_n.Count Then
                Return False
            End If
            Return True
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    m_n = Nothing
                End If
            End If
            disposedValue = True
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
        End Sub
#End Region
    End Class

End Namespace
