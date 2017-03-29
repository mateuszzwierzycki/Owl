Imports Owl.Core.Tensors

Namespace Clustering

    Public Module SimplifiedKMeans

        Public Function Run(TS As TensorSet, ClusterCount As Integer, Iterations As Integer, Optional Energy As Double = 0, Optional RndSeed As Integer = 123) As KMeansEngine
            If ClusterCount > TS.Count Then Return Nothing

            Dim nk As New KMeansEngine(TS)

            Dim rnd As New Random(RndSeed)
            Dim hs As New HashSet(Of Integer)
            While hs.Count < ClusterCount
                hs.Add(rnd.Next(TS.Count))
            End While

            For Each index As Integer In hs
                nk.AddSeed(TS(index))
            Next

            For i As Integer = 0 To Iterations - 1 Step 1
                If nk.RunOnce() < Energy Then Exit For
            Next

            Return nk
        End Function

    End Module

    Public Class KMeansEngine

        Private _vset As New TensorSet
        Private _seeds As New TensorSet
        Private _seedMultiply As New List(Of Double)
        Private _dimMultiply() As Double = Nothing
        Private _map As New List(Of Integer)

        Private _furthestIndex As Integer = -1
        Private _furthestDistance As Double = 0

        Public Sub New(Tensors As IEnumerable(Of Tensor))
            InternalSet.AddRange(Tensors)

            For i As Integer = 0 To Tensors.Count - 1 Step 1
                Map.Add(-1)
            Next

            ReDim _dimMultiply(InternalSet(0).Length - 1)

            For i As Integer = 0 To _dimMultiply.Length - 1
                _dimMultiply(i) = 1
            Next
        End Sub

        Public Function GetMap() As Integer()
            Return Map.ToArray
        End Function

        Private Property Map As List(Of Integer)
            Get
                Return _map
            End Get
            Set(value As List(Of Integer))
                _map = value
            End Set
        End Property

        Public Property DimensionWeights As Double()
            Get
                Return _dimMultiply
            End Get
            Set(value As Double())
                _dimMultiply = value
            End Set
        End Property

        Public Property SeedWeights As List(Of Double)
            Get
                Return _seedMultiply
            End Get
            Set(value As List(Of Double))
                _seedMultiply = value
            End Set
        End Property

        Public Sub RemoveSeed(Index As Integer)
            _seeds.RemoveAt(Index)
            SeedWeights.RemoveAt(Index)
        End Sub

        Public Function AddSeed(Seed As Tensor) As Boolean
            If _seeds.Count > 0 Then
                If _seeds(0).Length <> Seed.Length Then Return False
            End If

            SeedWeights.Add(1)
            _seeds.Add(Seed)
            Return True
        End Function

        ''' <summary>
        ''' Returns a DUPLICATE of the Seeds as a TensorSet.
        ''' </summary>
        ''' <returns></returns>
        Public Function GetSeeds() As TensorSet
            Return _seeds.Duplicate
        End Function

        Public ReadOnly Property FurthestCluster As Integer
            Get
                Return Map(FurthestIndex)
            End Get
        End Property

        Public ReadOnly Property FurthestIndex As Integer
            Get
                Return _furthestIndex
            End Get
        End Property

        Public ReadOnly Property FurtherDistance As Double
            Get
                Return _furthestDistance
            End Get
        End Property

        Private Property InternalSet As TensorSet
            Get
                Return _vset
            End Get
            Set(value As TensorSet)
                _vset = value
            End Set
        End Property

        ''' <summary>
        ''' Splits Tensors into TensorSet clusters.
        ''' </summary>
        ''' <returns></returns>
        Public Function SplitIntoSets() As List(Of TensorSet)
            Dim nl As New List(Of TensorSet)

            For i As Integer = 0 To _seeds.Count - 1 Step 1
                nl.Add(New TensorSet)
            Next

            For i As Integer = 0 To InternalSet.Count - 1 Step 1
                If Map(i) <> -1 Then nl(Map(i)).Add(InternalSet(i))
            Next

            Return nl
        End Function

        ''' <summary>
        ''' Returns an array of lists with Tensor indices assigned to each of the seeds.
        ''' </summary>
        ''' <returns></returns>
        Public Function SplitAsIndices() As List(Of Integer)()
            Dim those(_seeds.Count - 1) As List(Of Integer)

            For i As Integer = 0 To _seeds.Count - 1 Step 1
                those(i) = New List(Of Integer)
            Next

            For i As Integer = 0 To Map.Count - 1 Step 1
                If Map(i) <> -1 Then those(Map(i)).Add(i)
            Next

            Return those
        End Function

        ''' <summary>
        ''' Returns total distance by which the seeds were moved. 
        ''' </summary>
        ''' <returns></returns>
        Public Function RunOnce() As Double
            Dim ClosestSeed(InternalSet.Count - 1) As Integer
            Dim SeedScore(_seeds.Count - 1) As Integer
            Dim NextSeeds As New TensorSet(_seeds.Count, _seeds(0).Length)

            _furthestDistance = -1
            _furthestIndex = -1

            Dim SeedW(SeedWeights.Count - 1) As Double

            For i As Integer = 0 To SeedWeights.Count - 1 Step 1
                SeedW(i) = 1 / SeedWeights(i)
            Next

            Parallel.For(0, InternalSet.Count, Sub(Index As Integer)
                                                   Dim Idx As Integer = -1
                                                   Dim Dist As Double = Double.MaxValue

                                                   For i As Integer = 0 To _seeds.Count - 1 Step 1
                                                       Dim ThisDist As Double = 0

                                                       For j As Integer = 0 To DimensionWeights.Length - 1 Step 1
                                                           ThisDist += ((_seeds(i)(j) - InternalSet(Index)(j)) ^ 2) * DimensionWeights(j)
                                                       Next

                                                       ThisDist *= SeedW(i)

                                                       If ThisDist < Dist Then
                                                           Dist = ThisDist
                                                           Idx = i
                                                       End If
                                                   Next

                                                   Dim mydist As Double = InternalSet(Index).DistanceTo(_seeds(Idx))

                                                   If mydist > FurtherDistance Then
                                                       _furthestDistance = mydist
                                                       _furthestIndex = Index
                                                   End If

                                                   ClosestSeed(Index) = Idx
                                                   SeedScore(Idx) += 1
                                               End Sub)

            For i As Integer = 0 To ClosestSeed.Length - 1 Step 1
                NextSeeds(ClosestSeed(i)) += InternalSet(i)
            Next

            For i As Integer = 0 To NextSeeds.Count - 1 Step 1
                If SeedScore(i) > 0 Then
                    NextSeeds(i) /= SeedScore(i)
                Else
                    NextSeeds(i) = _seeds(i)
                End If
            Next

            Dim energy As Double = 0

            For i As Integer = 0 To _seeds.Count - 1 Step 1
                energy += _seeds(i).DistanceTo(NextSeeds(i))
            Next

            _seeds = NextSeeds
            _map = ClosestSeed.ToList

            Return energy
        End Function

    End Class
End Namespace
