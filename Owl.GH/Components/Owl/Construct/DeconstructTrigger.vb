Imports Grasshopper.Kernel.Parameters

Public Class DeconstructTrigger
    Inherits OwlComponentBase

    Sub New()
		MyBase.New("Deconstruct Trigger",
				   "DeTrigger",
				   "Deconstruct Trigger",
				   SubCategoryPrimitive)
	End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("{B574D728-BBAD-4039-8FF8-0076D9C27D59}")
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_Trigger)
    End Sub

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_53
        End Get
    End Property

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.tertiary
        End Get
    End Property

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddTextParameter("Nickname", "N", "Nickname", GH_ParamAccess.item)
        pManager.AddTimeParameter("Date", "D", "Trigger date", GH_ParamAccess.item)
        pManager.AddParameter(New Param_Guid, "Source", "S", "Trigger source", GH_ParamAccess.item)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim trig As New Trigger

        If Not DA.GetData(0, trig) Then Return

        DA.SetData(0, trig.Nickname)
        DA.SetData(1, trig.Date)
        DA.SetData(2, trig.Source)

    End Sub
End Class
