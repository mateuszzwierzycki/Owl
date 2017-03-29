Imports Owl.Core.Tensors

Namespace Convolutions

    Public Class ImageModifierLayer
        Inherits System.Collections.CollectionBase
        Implements IDisposable
        Implements IEnumerable(Of ImageModifier)

        Sub New()

        End Sub

        Sub New(Other As ImageModifierLayer)
            For i As Integer = 0 To Other.Count - 1 Step 1
                Me.Add(Other(i).Duplicate)
            Next
        End Sub

        Public Function Duplicate() As ImageModifierLayer
            Return New ImageModifierLayer(Me)
        End Function

        Public Sub Add(Value As ImageModifier)
            List.Add(Value)
        End Sub

        Public Sub AddRange(Values As IEnumerable(Of ImageModifier))
            InnerList.AddRange(Values)
        End Sub

        Sub Remove(index As Integer)
            List.RemoveAt(index)
        End Sub

        Default Public Property Item(index As Integer) As ImageModifier
            Get
                Return List.Item(index)
            End Get
            Set(value As ImageModifier)
                List.Item(index) = value
            End Set
        End Property

        ''' <summary>
        ''' Applies all modifiers on copies of the Image
        ''' </summary>
        ''' <param name="Image">Image to modify</param>
        ''' <returns>TensorImageSet storing mutiple different modification of the initial Image</returns>
        Public Function Apply(Image As Tensor, Optional Multithread As Boolean = True) As TensorSet
            Dim ns As New TensorSet

            For i As Integer = 0 To Me.Count - 1 Step 1
                Dim thisdup As Tensor = Image.Duplicate
                ns.Add(thisdup)
            Next

            If Multithread Then
                Parallel.For(0, Me.Count, Sub(index As Integer)
                                              Me(index).Apply(ns(index))
                                          End Sub)
            Else
                For i As Integer = 0 To Me.Count - 1 Step 1
                    Me(i).Apply(ns(i))
                Next
            End If

            Return ns
        End Function

        ''' <summary>
        ''' Applies each modificator to each Image in the ImageSet.
        ''' Number of Images = number of modificators
        ''' OR 
        ''' Number of Images = 1 AND Number of modificators = ANY
        ''' OR
        ''' Number of Images = ANY and Number of modificators = 1
        ''' </summary>
        ''' <param name="ImageSet"></param>
        Public Sub Apply(ByRef ImageSet As TensorSet, Optional Multithread As Boolean = True)
            Dim imgset As TensorSet = ImageSet

            If imgset.Count = 1 And Me.Count > 1 Then
                imgset = Me.Apply(imgset(0))

            ElseIf imgset.Count > 1 And Me.Count = 1 Then

                If Multithread Then
                    Parallel.For(0, imgset.Count, Sub(index As Integer)
                                                      Me(0).Apply(imgset(index))
                                                  End Sub)
                Else
                    For i As Integer = 0 To imgset.Count - 1 Step 1
                        Me(0).Apply(imgset(i))
                    Next
                End If

            ElseIf imgset.Count = Me.Count Then
                If Multithread Then
                    Parallel.For(0, Me.Count, Sub(index As Integer)
                                                  Me(index).Apply(imgset(index))
                                              End Sub)
                Else
                    For i As Integer = 0 To Me.Count - 1 Step 1
                        Me(i).Apply(imgset(i))
                    Next
                End If
            End If

            ImageSet = imgset
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    For i As Integer = 0 To Me.Count - 1 Step 1
                        Me.Clear()
                    Next
                End If
            End If
            disposedValue = True
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
        End Sub
#End Region

        Private Function IEnumerable_GetEnumerator() As IEnumerator(Of ImageModifier) Implements IEnumerable(Of ImageModifier).GetEnumerator
            Throw New NotImplementedException()
        End Function

    End Class

    Public Class ImageModifierLayerEnumerator
        Implements IEnumerator(Of ImageModifier)

        Private m_im As ImageModifierLayer = Nothing
        Dim ind As Integer = 0

        Sub New(Img As ImageModifierLayer)
            m_im = Img
        End Sub

        Public ReadOnly Property Current As ImageModifier Implements IEnumerator(Of ImageModifier).Current
            Get
                Return m_im(ind)
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
            If ind = Me.m_im.Count Then
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
                    m_im = Nothing
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
