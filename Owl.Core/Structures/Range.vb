Namespace Structures

    Public Structure Range

        Dim m_from As Double
        Dim m_to As Double

        Sub New(From As Double, [To] As Double)
            m_from = From
            m_to = [To]
        End Sub

        Public Function IsEmpty() As Boolean
            Return Me = Empty()
        End Function

        Public Shared Function Empty() As Range
            Return New Range(0, 0)
        End Function

        Public Shared Function Create(From As Double, [To] As Double) As Range
            Return New Range(From, [To])
        End Function

        Public Property From As Double
            Get
                Return m_from
            End Get
            Set(value As Double)
                m_from = value
            End Set
        End Property

        Public Property [To] As Double
            Get
                Return m_to
            End Get
            Set(value As Double)
                m_to = value
            End Set
        End Property

        Public Function ValueAtParameter(Parameter As Double) As Double
            Return (([To] - From) * Parameter) + From
        End Function

        Public Function ParameterAtValue(Value As Double) As Double
            Return (Value - From) / ([To] - From)
        End Function

        Public ReadOnly Property Length As Double
            Get
                Return Maximum - Minimum
            End Get
        End Property

        Public ReadOnly Property Minimum As Double
            Get
                Return Math.Min(From, [To])
            End Get
        End Property

        Public ReadOnly Property Maximum As Double
            Get
                Return Math.Max(From, [To])
            End Get
        End Property

        Public Shared Operator =(A As Range, B As Range) As Boolean
            If A.From = B.From And A.To = B.To Then Return True
            Return False
        End Operator

        Public Shared Operator <>(A As Range, B As Range) As Boolean
            Return Not A = B
        End Operator

        Public Overrides Function ToString() As String
            Return "Range {" & Me.From.ToString("0.0") & "-" & Me.To.ToString("0.0") & "}"
        End Function

    End Structure

End Namespace
