Imports Grasshopper.Kernel

Public Class LifeSaver
    Inherits GH_Component

    Friend lsc As LifeSaverControl = Nothing

    Sub New()
        MyBase.New("LifeSaver", "LS", "Owl", "Owl", "Owl")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("{8BA28BF9-7552-4BE4-A92E-FC9C479292FE}")
        End Get
    End Property

    Public Overrides Sub CreateAttributes()
        m_attributes = New LifeSaver_Attributes(Me)
    End Sub

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddTextParameter("Names", "N", "Optional names", GH_ParamAccess.list)
        Me.Params.Input(0).Optional = True
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddGenericParameter("Data", "D", "Data from the control", GH_ParamAccess.list)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim s As New List(Of String)
        If lsc Is Nothing Then Return

        DA.GetDataList(0, s)

        SyncLock lsc.m_instring
            lsc.m_instring = ""
            For Each str As String In s
                lsc.m_instring &= str & vbCrLf
            Next
        End SyncLock

        SyncLock lsc.m_string
            DA.SetDataList(0, lsc.m_string.Split(vbCrLf))
        End SyncLock
    End Sub
End Class
