Imports Rhino.Geometry

Public Class TensorizeCurve_OBSOLETE
    Inherits OwlComponentBase

    Sub New()
        MyBase.New("Curve Tensor", "CrvTensor", "Convert curvature information from a curve into a Tensor", OwlComponentBase.SubCategoryConvert)
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("{2BB355E9-2125-470C-B765-A347C1B17A6D}")
        End Get
    End Property

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.hidden
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddCurveParameter("Curves", "C", "Curves", GH_ParamAccess.item)
        pManager.AddIntegerParameter("Count", "N", "Parameter count", GH_ParamAccess.item, 10)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_OwlTensor)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)

        Dim Crv As Curve = Nothing
        Dim Divide As Integer = 0

        If Not DA.GetData(0, Crv) Then Return
        If Not DA.GetData(1, Divide) Then Return

        Dim params() As Double = Nothing

        Dim denom As Double = 1 / Math.PI

        If Crv.IsPeriodic Then
            params = Crv.DivideByCount(Divide + 1, True)
        Else
            params = Crv.DivideByCount(Divide, True)
        End If

        Dim angs((Divide * 2) - 1) As Double

        Dim denom2pi As Double = 1 / (Math.PI * 2)
        Dim denompi As Double = 1 / Math.PI

        For i As Integer = 0 To Divide - 1 Step 1
            Dim thisv As Vector3d = Crv.TangentAt(params(i))
            Dim thatv As Vector3d = Crv.TangentAt(params((i + 1 + Divide) Mod Divide))

            Dim thisc As Vector3d = Crv.CurvatureAt(params(i))
            Dim thatc As Vector3d = Crv.CurvatureAt(params((i + 1 + Divide) Mod Divide))

            angs(i) = Vector3d.VectorAngle(thisv, thatv) * denompi
            angs(i + Divide) = Vector3d.VectorAngle(thisc, thatc) * denompi

        Next

        DA.SetData(0, New Tensor(angs))

    End Sub



End Class
