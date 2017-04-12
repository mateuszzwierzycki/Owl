Public Class Trigger

    Private m_datetime As DateTime = DateTime.Now
    Private m_source As Guid = Guid.Empty
    Private m_nick As String = "Trigger"

    Sub New()

    End Sub

    Sub New(D As Date, Source As Guid, Nickname As String)
        m_datetime = D
        m_source = Source
        m_nick = Nickname
    End Sub

    Public Property [Date] As Date
        Get
            Return m_datetime
        End Get
        Set(value As Date)
            m_datetime = value
        End Set
    End Property

    Public Property Source As Guid
        Get
            Return m_source
        End Get
        Set(value As Guid)
            m_source = value
        End Set
    End Property

    Public Property Nickname As String
        Get
            Return m_nick
        End Get
        Set(value As String)
            m_nick = value
        End Set
    End Property

    Public Overrides Function ToString() As String
        Dim gs As String = Me.Source.ToString
        gs = gs.Substring(gs.Length - 6, 6)
        Return "Trigger {" & Me.Nickname & " " & gs & " " & Me.Date.ToLongTimeString & "}"
    End Function

    Public Function Duplicate() As Trigger
        Return New Trigger(Me.Date, Me.Source, Me.Nickname)
    End Function


End Class
