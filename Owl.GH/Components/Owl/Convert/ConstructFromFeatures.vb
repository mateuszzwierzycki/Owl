Public Class ConstructFromFeatures
    Inherits OwlComponentBase
    Implements IGH_VariableParameterComponent

    Sub New()
        MyBase.New("Feature TensorSet", "FeatureTS", "Convert multiple types of data into a TensorSet", OwlComponentBase.SubCategoryConvert)
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("5168b0ae-12b9-4b86-83ae-62581c52c059")
        End Get
    End Property

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.secondary
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddGenericParameter("Feature 0", "F0", "Feature 0", GH_ParamAccess.list)
        Me.Params.Input(0).Optional = True
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet)
    End Sub

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_19
        End Get
    End Property

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)

        Dim allgens As New List(Of List(Of Object))
        Dim cnt As Integer = 0

        For i As Integer = 0 To Me.Params.Input.Count - 1 Step 1
            Dim gen As New List(Of Object)
            DA.GetDataList(i, gen)
            If i = 0 Then cnt = gen.Count
            If gen.Count <> cnt And gen.Count > 0 Then
                Me.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Number of features has to be the same for each input")
                Exit Sub
            End If
            allgens.Add(gen)
        Next

        Dim tens As New List(Of List(Of Double))
        Dim tenc As New List(Of Integer)

        For i As Integer = 0 To allgens.Count - 1 Step 1
            Dim thisl As New List(Of Double)

            If allgens(i).Count = 0 Then Continue For

            Select Case allgens(i)(0).GetType
                Case GetType(GH_OwlTensor)
                    Dim typed = TryCast(CObj(allgens(i)), List(Of GH_Number))
                    Dim thislen As Integer = -1

                    For Each obj As GH_OwlTensor In allgens(i)
                        thisl.AddRange(obj.Value)
                        thislen = obj.Value.Length
                    Next
                    tens.Add(thisl)
                    tenc.Add(thislen)
                Case GetType(GH_Number)
                    Dim typed = TryCast(CObj(allgens(i)), List(Of GH_Number))
                    For Each obj As GH_Number In allgens(i)
                        thisl.Add(obj.Value)
                    Next
                    tens.Add(thisl)
                    tenc.Add(1)
                Case GetType(GH_Integer)
                    Dim typed = TryCast(CObj(allgens(i)), List(Of GH_Integer))
                    For Each obj As GH_Integer In allgens(i)
                        thisl.Add(obj.Value)
                    Next
                    tens.Add(thisl)
                    tenc.Add(1)
                Case GetType(GH_Colour)
                    Dim typed = TryCast(CObj(allgens(i)), List(Of GH_Colour))
                    For Each obj As GH_Colour In allgens(i)
                        thisl.Add(obj.Value.R)
                        thisl.Add(obj.Value.G)
                        thisl.Add(obj.Value.B)
                    Next
                    tens.Add(thisl)
                    tenc.Add(3)
                Case GetType(GH_Point)
                    Dim typed = TryCast(CObj(allgens(i)), List(Of GH_Point))
                    For Each obj As GH_Point In allgens(i)
                        thisl.Add(obj.Value.X)
                        thisl.Add(obj.Value.Y)
                        thisl.Add(obj.Value.Z)
                    Next
                    tens.Add(thisl)
                    tenc.Add(3)
                Case GetType(GH_ComplexNumber)
                    Dim typed = TryCast(CObj(allgens(i)), List(Of GH_ComplexNumber))
                    For Each obj As GH_ComplexNumber In allgens(i)
                        thisl.Add(obj.Value.Real)
                        thisl.Add(obj.Value.Imaginary)
                    Next
                    tens.Add(thisl)
                    tenc.Add(2)
                Case GetType(GH_Interval)
                    Dim typed = TryCast(CObj(allgens(i)), List(Of GH_Interval))
                    For Each obj As GH_Interval In allgens(i)
                        thisl.Add(obj.Value.T0)
                        thisl.Add(obj.Value.T1)
                    Next
                    tens.Add(thisl)
                    tenc.Add(2)
                Case GetType(GH_Interval2D)
                    Dim typed = TryCast(CObj(allgens(i)), List(Of GH_Interval2D))
                    For Each obj As GH_Interval2D In allgens(i)
                        thisl.Add(obj.Value.U0)
                        thisl.Add(obj.Value.U1)
                        thisl.Add(obj.Value.V0)
                        thisl.Add(obj.Value.V1)
                    Next
                    tens.Add(thisl)
                    tenc.Add(4)
                Case GetType(GH_Line)
                    Dim typed = TryCast(CObj(allgens(i)), List(Of GH_Line))
                    For Each obj As GH_Line In allgens(i)
                        thisl.Add(obj.Value.From.X)
                        thisl.Add(obj.Value.From.Y)
                        thisl.Add(obj.Value.From.Z)
                        thisl.Add(obj.Value.To.X)
                        thisl.Add(obj.Value.To.Y)
                        thisl.Add(obj.Value.To.Z)
                    Next
                    tens.Add(thisl)
                    tenc.Add(6)
                Case GetType(GH_Plane)
                    Dim typed = TryCast(CObj(allgens(i)), List(Of GH_Plane))
                    For Each obj As GH_Plane In allgens(i)
                        thisl.Add(obj.Value.Origin.X)
                        thisl.Add(obj.Value.Origin.Y)
                        thisl.Add(obj.Value.Origin.Z)
                        thisl.Add(obj.Value.XAxis.X)
                        thisl.Add(obj.Value.XAxis.Y)
                        thisl.Add(obj.Value.XAxis.Z)
                        thisl.Add(obj.Value.YAxis.X)
                        thisl.Add(obj.Value.YAxis.Y)
                        thisl.Add(obj.Value.YAxis.Z)
                    Next
                    tens.Add(thisl)
                    tenc.Add(9)
                Case GetType(GH_Rectangle)
                    Dim typed = TryCast(CObj(allgens(i)), List(Of GH_Rectangle))
                    For Each obj As GH_Rectangle In allgens(i)
                        thisl.Add(obj.Value.Width)
                        thisl.Add(obj.Value.Height)
                    Next
                    tens.Add(thisl)
                    tenc.Add(2)
                Case GetType(GH_Vector)
                    Dim typed = TryCast(CObj(allgens(i)), List(Of GH_Vector))
                    For Each obj As GH_Vector In allgens(i)
                        thisl.Add(obj.Value.X)
                        thisl.Add(obj.Value.Y)
                        thisl.Add(obj.Value.Z)
                    Next
                    tens.Add(thisl)
                    tenc.Add(3)
                Case GetType(GH_String)
                    For Each obj As GH_String In allgens(i)
                        Dim thiscst As String = obj.Value
                        Dim thisdbl As Double = Double.NaN

                        If Double.TryParse(thiscst, thisdbl) Then
                            thisl.Add(thisdbl)
                        End If
                    Next
                    tens.Add(thisl)
                    tenc.Add(1)
            End Select
        Next

        Dim dcount As Integer = 0
        For i As Integer = 0 To tenc.Count - 1 Step 1
            dcount += tenc(i)
        Next

        If dcount = 0 Then Me.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Not enough dimensions.") : Return

        Dim incount As Integer = allgens(0).Count
        Dim ts As New TensorSet

        For i As Integer = 0 To incount - 1 Step 1
            Dim thistens As New Tensor(dcount)
            Dim thistenspos As Integer = 0

            For j As Integer = 0 To tens.Count - 1 Step 1
                Dim thisl As List(Of Double) = tens(j)
                Dim thislen As Integer = tenc(j)
                Dim thispos As Integer = thislen * i

                For k As Integer = 0 To thislen - 1 Step 1
                    thistens(thistenspos + k) = thisl(thispos + k)
                Next

                thistenspos += thislen
            Next

            ts.Add(thistens)
        Next

        DA.SetData(0, ts)
    End Sub

    Public Sub VariableParameterMaintenance() Implements IGH_VariableParameterComponent.VariableParameterMaintenance
        Dim cnt As Integer = 0
        For Each p As IGH_Param In Me.Params.Input
            p.Name = "Feature " & cnt
            p.NickName = "F" & cnt
            p.MutableNickName = False
            cnt += 1
        Next
    End Sub

    Public Function CanInsertParameter(side As GH_ParameterSide, index As Integer) As Boolean Implements IGH_VariableParameterComponent.CanInsertParameter
        If side = GH_ParameterSide.Input Then Return True
        Return False
    End Function

    Public Function CanRemoveParameter(side As GH_ParameterSide, index As Integer) As Boolean Implements IGH_VariableParameterComponent.CanRemoveParameter
        If side = GH_ParameterSide.Input And Me.Params.Input.Count > 1 Then Return True
        Return False
    End Function

    Public Function CreateParameter(side As GH_ParameterSide, index As Integer) As IGH_Param Implements IGH_VariableParameterComponent.CreateParameter
        Dim ng As New Kernel.Parameters.Param_GenericObject()
        ng.Access = GH_ParamAccess.list
        ng.MutableNickName = False
        ng.Optional = True
        Return ng
    End Function

    Public Function DestroyParameter(side As GH_ParameterSide, index As Integer) As Boolean Implements IGH_VariableParameterComponent.DestroyParameter
        Return True
    End Function
End Class
