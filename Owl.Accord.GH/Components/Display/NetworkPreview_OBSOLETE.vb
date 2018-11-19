Imports Rhino.Geometry
Imports Grasshopper.Kernel
Imports System.Drawing
Imports Owl
Imports System.IO
Imports System.Drawing.Imaging
Imports ac = Accord

Public Class NetworkPreview_OBSOLETE
    Inherits GH_Component

    Sub New()
        MyBase.New("Network Preview", "NPreview", "Quick Network preview", "Owl.Accord", "Display")
        Me.MutableNickName = False
    End Sub

    Public Overrides Sub CreateAttributes()
        m_attributes = New NetworkPreview_Attributes(Me)
        att = m_attributes
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("{83F2DCBA-9D11-4927-90C1-5A201BABEDC2}")
        End Get
    End Property

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.hidden
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_20
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddParameter(New Param_ActivationNetwork)
        pManager.AddIntervalParameter("Lines", "L", "Line thickness", GH_ParamAccess.item, New Interval(1, 5))
        pManager.AddIntegerParameter("Points", "P", "Point radius", GH_ParamAccess.item, 8)
        pManager.AddIntegerParameter("Size", "S", "Bitmap size / quality", GH_ParamAccess.item, 800)
        pManager.AddTextParameter("Directory", "D", "Optional directory for the bitmap", GH_ParamAccess.item)
        Me.Params.Input(4).Optional = True
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)

    End Sub

    Dim att As NetworkPreview_Attributes

    Friend m_net As ac.Neuro.Network = Nothing

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)

        'Me.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Repair me")
        'Return

        If DA.Iteration = 0 Then

            Dim thisn As ActivationNetwork = Nothing

            If Not DA.GetData(0, thisn) Then Return

            m_net = thisn

            Dim it As New Interval
            Dim rad As Integer
            Dim q As Integer
            Dim mydir As String = Nothing

            DA.GetData(1, it)
            DA.GetData(2, rad)
            DA.GetData(3, q)
            DA.GetData(4, mydir)

            If mydir Is Nothing Then
                att.CreateImage(it, rad, q)
            Else
                If Not Directory.Exists(mydir) Then Directory.CreateDirectory(mydir)
                Dim fn As New String("Network_" & DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fffffff"))
                att.CreateImage(it, rad, q).Save(mydir & "\" & fn & ".png", ImageFormat.Png)
            End If

        End If

    End Sub
End Class
