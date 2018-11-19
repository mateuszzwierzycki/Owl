Imports System.Drawing
Imports Accord.Neuro.Learning
Imports Grasshopper.Kernel
Imports Owl.Core.Tensors
Imports Owl.GH.Common
Imports Owl.Accord.Extensions.Learning
Imports Grasshopper.Kernel.Data
Imports Owl.Accord.GH.Common
Imports Grasshopper.Kernel.Types

Public Class BackpropThreaded
    Inherits GH_Component

    Sub New()
		MyBase.New("BackpropagationEx",
				   "BackPropEx",
				   "Teach the Network with backpropagation, threaded.",
				   "Owl.Accord",
				   "Backprop")
	End Sub

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.hidden
        End Get
    End Property

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("1aac1e5a-0c1f-42bb-a933-1b5d7575ed3a")
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_14
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_ActivationNetwork)
        pManager.AddParameter(New Param_OwlTensorSet, "Input", "I", "Input TensorSet", GH_ParamAccess.item)
        pManager.AddParameter(New Param_OwlTensorSet, "Output", "O", "Output TensorSet", GH_ParamAccess.item)
        pManager.AddNumberParameter("Learning Rate", "L", "Learning rate", GH_ParamAccess.item, 0.01)
        pManager.AddNumberParameter("Momentum", "M", "Momentum", GH_ParamAccess.item, 0.001)
        pManager.AddIntegerParameter("Epoch", "E", "Tensors per epoch", GH_ParamAccess.item, 10)
        pManager.AddIntegerParameter("Iterations", "N", "Number of iterations", GH_ParamAccess.item, 100)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_ActivationNetwork)
        pManager.AddNumberParameter("Error", "E", "Error", GH_ParamAccess.item)
    End Sub

    Function DoTheComponentStuff(DA As IGH_DataAccess) As Boolean
        If Not DA.GetData(0, nn) Then Return False
        If Not DA.GetData(1, ins) Then Return False
        If Not DA.GetData(2, outs) Then Return False
        If Not DA.GetData(3, rate) Then Return False
        If Not DA.GetData(4, mome) Then Return False
        If Not DA.GetData(5, ep) Then Return False
        If Not DA.GetData(6, iter) Then Return False

        If nn.InputsCount <> ins.Value(0).Count Then Return False
        If nn.Layers(nn.Layers.Count - 1).Neurons.Count <> outs.Value(0).Count Then Return False
        If ins.Value.Count <> outs.Value.Count Then Return False
        Return True
    End Function

    Private nn As ActivationNetwork = Nothing
    Private ins As New GH_OwlTensorSet
    Private outs As New GH_OwlTensorSet
    Private rate As Double
    Private mome As Double
    Private ep As Integer
    Private iter As Integer

    Private bp As BackPropagationLearning = Nothing

    Private lsum As Double

    Sub StartThread()
        bp = New BackPropagationLearning(nn)
        bp.LearningRate = rate
        bp.Momentum = mome
        th = New Threading.Thread(AddressOf Learn)
        th.Start()
    End Sub

    Dim th As System.Threading.Thread = Nothing
    Dim complete As Boolean = False

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)

        If (th Is Nothing) Then
            complete = False
            If Not DoTheComponentStuff(DA) Then Return
            StartThread()
            DA.AbortComponentSolution()
        ElseIf Not complete Then
            DA.AbortComponentSolution()
        ElseIf complete Then
            DA.SetData(0, nn)
            DA.SetData(1, lsum)
            th = Nothing
        End If

    End Sub

    Private m_abortFlag As Boolean = False

    Sub Learn()

        Dim thisvs As TensorSet = ins.Value
        Dim thisout As TensorSet = outs.Value
        Dim ts As New System.Diagnostics.Stopwatch

        Dim cnt As Double = thisvs.Count
        Dim sum As Double = 0

        ts.Start()
        Dim lastms As Long = 0

        For j As Integer = 0 To iter - 1 Step 1

            For i As Integer = 0 To thisvs.Count - 1 Step ep
                sum = bp.RunEpoch(thisvs, thisout, i, Math.Min(i + ep - 1, thisvs.Count - 1))
                If m_abortFlag Then Exit Sub
            Next

            If (ts.ElapsedMilliseconds - lastms) >= 100 Then
                lastms = ts.ElapsedMilliseconds
                InvokeMessage("Iteration: " & j & "/" & iter)
            End If
        Next

        ts.Stop()
        ts = Nothing

        lsum = sum
        complete = True
        InvokeMessage("Done")
        InvokeExpire()
    End Sub

    Sub InvokeMessage(Message As String)
        Me.Message = Message
        Dim exp As Action = AddressOf ExpireDisplay
        If Rhino.RhinoApp.MainApplicationWindow.InvokeRequired Then Rhino.RhinoApp.MainApplicationWindow.Invoke(exp)
    End Sub

    Sub ExpireDisplay()
        Me.OnDisplayExpired(True)
    End Sub

    Sub InvokeExpire()
        Dim exp As Action = AddressOf ExpireComponent
        If Rhino.RhinoApp.MainApplicationWindow.InvokeRequired Then Rhino.RhinoApp.MainApplicationWindow.Invoke(exp)
    End Sub

    Sub ExpireComponent()
        Me.ExpireSolution(True)
    End Sub
End Class


Public Class BackOwl
    Inherits OwlMultiThreadedBase

    Sub New()
        MyBase.New("BackpropagationEx", "BackEx", "Backpropagation, threaded.", "Owl.Accord", "Learning")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("fd79b5aa-2504-4e9e-a220-5bf30bf53e0e")
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_14
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_ActivationNetwork)
        pManager.AddParameter(New Param_OwlTensorSet, "Input", "I", "Input TensorSet", GH_ParamAccess.item)
        pManager.AddParameter(New Param_OwlTensorSet, "Output", "O", "Output TensorSet", GH_ParamAccess.item)
        pManager.AddNumberParameter("Learning Rate", "L", "Learning rate", GH_ParamAccess.item, 0.01)
        pManager.AddNumberParameter("Momentum", "M", "Momentum", GH_ParamAccess.item, 0.001)
        pManager.AddIntegerParameter("Iterations", "N", "Number of iterations", GH_ParamAccess.item, 100)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_ActivationNetwork)
        pManager.AddNumberParameter("Error", "E", "Error", GH_ParamAccess.item)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        If Me.AbortFlag Then
            Me.AbortFlag = False
            Me.Message = ""
            Exit Sub
        End If

        If ThreadComplete Then
            ThreadComplete = False
            If DA.Iteration = 0 Then
                If _st Is Nothing Then _st = New System.Diagnostics.Stopwatch
                _cnt = 0
                Dim nn As New GH_Structure(Of GH_ActivationNetwork)
                Dim ne As New GH_Structure(Of GH_Number)

                For Each tsk As TensorSetTask In MyTasks
                    nn.Append(New GH_ActivationNetwork(tsk.Result(0)), tsk.DataPath)
                    Dim num As New GH_Number
                    num.Value = tsk.Result(1)
                    ne.Append(num, tsk.DataPath)
                Next

                DA.SetDataTree(0, nn)
                DA.SetDataTree(1, ne)
            End If
            Exit Sub
        End If

        If Not ThreadIsAlive() Then

            Dim nn As ActivationNetwork = Nothing
            Dim ins As New GH_OwlTensorSet
            Dim outs As New GH_OwlTensorSet
            Dim rate As Double
            Dim mome As Double
            Dim iter As Integer

            If Not DA.GetData(0, nn) Then Return
            If Not DA.GetData(1, ins.Value) Then Return
            If Not DA.GetData(2, outs.Value) Then Return
            If Not DA.GetData(3, rate) Then Return
            If Not DA.GetData(4, mome) Then Return
            If Not DA.GetData(5, iter) Then Return

            Dim par As New List(Of Object)
            par.Add(nn)     '0
            par.Add(ins)    '1
            par.Add(outs)   '2
            par.Add(rate)   '3 
            par.Add(mome)   '4
            par.Add(iter)   '5

            Me.AllSetUp = True
            Dim thisp As GH_Path = DA.ParameterTargetPath(0)
            thisp = thisp.AppendElement(DA.ParameterTargetIndex(0))

            Dim nt As New TensorSetTask(ins.Value, par, AddressOf ComponentTensorAction, thisp)
            Me.MyTasks.Add(nt)
            Exit Sub
        End If

    End Sub

    Private _cnt As Integer = 0
    Private _st As New System.Diagnostics.Stopwatch()

    Public Overrides Function ComponentTensorAction(TS As TensorSet, Params As List(Of Object)) As List(Of Object)
        If Not _st.IsRunning Then _st.Start()
        Dim bp As New BackPropagationLearning(Params(0))
        bp.LearningRate = Params(3)
        bp.Momentum = Params(4)
        Dim iter As Integer = Params(5)

        Dim its As GH_OwlTensorSet = Params(1)
        Dim ots As GH_OwlTensorSet = Params(2)

        Dim err As Double = 0

        For i As Integer = 0 To iter - 1 Step 1
            err = bp.RunEpoch(its.Value.AsArrayArray, ots.Value.AsArrayArray)

            If _st.ElapsedMilliseconds > 500 Then
                Threading.Interlocked.Add(_cnt, 1)
                MyBase.InvokePercent(_cnt)
                _st.Restart()
            End If

        Next

        Dim outs As New List(Of Object)
        outs.Add(Params(0))
        outs.Add(err)

        _st.Stop()
        Return outs
    End Function

End Class
