Imports System.Drawing
Imports Grasshopper.Kernel
Imports Owl.Core.Tensors
Imports Owl.GH.Common
Imports Grasshopper.Kernel.Data
Imports ac = Accord

Public Class TSNE
    Inherits GH_Component

    Sub New()
		MyBase.New("t-SNE",
				   "t-SNE",
				   "Laurens van der Maaten's dimensionality reduction method.",
				   "Owl.Accord",
				   "Unsupervised")
	End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("864519f5-0de2-4cc1-8353-e70224d086d4")
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_50
        End Get
    End Property

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.secondary
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet, "Data", "D", "Data to cluster", GH_ParamAccess.item)
        pManager.AddNumberParameter("Perplexity", "P", "Perplexity", GH_ParamAccess.item, 1.5)
        pManager.AddNumberParameter("Theta", "T", "Theta", GH_ParamAccess.item, 0.5)
        pManager.AddIntegerParameter("Outputs", "O", "Number of output dimensions", GH_ParamAccess.item, 1)
        pManager.AddIntegerParameter("Seed", "S", "Randomness Seed", GH_ParamAccess.item, 123)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet, "Mapped", "M", "Data after dimensionality reduction", GH_ParamAccess.item)
    End Sub

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)

        Dim data As New TensorSet
        Dim perp As Double
        Dim thet As Double
        Dim outs As Integer
        Dim seed As Integer

        If Not DA.GetData(0, data) Then Return
        If Not data.IsHomogeneous Then
            Me.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "TensorSet is not homogeneous")
            Exit Sub
        End If

        If Not DA.GetData(1, perp) Then Return
        If Not DA.GetData(2, thet) Then Return
        If Not DA.GetData(3, outs) Then Return
        If Not DA.GetData(4, seed) Then Return


        ac.Math.Random.Generator.Seed = seed

        outs = System.Math.Max(1, outs)

        Dim nn As New ac.MachineLearning.Clustering.TSNE()

        nn.NumberOfInputs = data(0).Length
        nn.NumberOfOutputs = outs
        nn.Perplexity = perp
        nn.Theta = thet
        Dim res()() As Double = nn.Transform(data.AsArrayArray)

        Dim nto As New TensorSet
        For Each r As Double() In res
            nto.Add(New Tensor(r))
        Next

        DA.SetData(0, nto)
    End Sub
End Class

Public Class TSNEthread
    Inherits OwlMultiThreadedBase

    Sub New()
        MyBase.New("t-SNE Ex", "t-SNE", "Laurens van der Maaten's dimensionality reduction method.", "Owl.Accord", "Unsupervised")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("34493e98-1e71-4c51-bc04-8d463011a298")
        End Get
    End Property

    Protected Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_51
        End Get
    End Property

    Public Overrides ReadOnly Property Exposure As GH_Exposure
        Get
            Return GH_Exposure.secondary
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet, "Data", "D", "Data to cluster", GH_ParamAccess.item)
        pManager.AddNumberParameter("Perplexity", "P", "Perplexity", GH_ParamAccess.item, 1.5)
        pManager.AddNumberParameter("Theta", "T", "Theta", GH_ParamAccess.item, 0.5)
        pManager.AddIntegerParameter("Outputs", "O", "Number of output dimensions", GH_ParamAccess.item, 1)
        pManager.AddIntegerParameter("Seed", "S", "Randomness Seed", GH_ParamAccess.item, 123)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
        pManager.AddParameter(New Param_OwlTensorSet, "Mapped", "M", "Data after dimensionality reduction", GH_ParamAccess.tree)
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
                Dim nn As New GH_Structure(Of GH_OwlTensorSet)

                For Each tsk As TensorSetTask In MyTasks
                    Dim tdbl()() As Double = tsk.Result(0)

                    Dim nto As New TensorSet
                    For Each r As Double() In tdbl
                        nto.Add(New Tensor(r))
                    Next
                    nn.Append(New GH_OwlTensorSet(nto), tsk.DataPath)
                Next

                DA.SetDataTree(0, nn)
            End If
            Exit Sub
        End If

        If Not ThreadIsAlive() Then

            Dim data As New TensorSet
            Dim perp As Double
            Dim thet As Double
            Dim outs As Integer
            Dim seed As Integer

            If Not DA.GetData(0, data) Then Return
            If Not data.IsHomogeneous Then
                Me.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "TensorSet is not homogeneous")
                MyTasks.Clear()
                Exit Sub
            End If

            If Not DA.GetData(1, perp) Then Return
            If Not DA.GetData(2, thet) Then Return
            If Not DA.GetData(3, outs) Then Return
            If Not DA.GetData(4, seed) Then Return

            outs = System.Math.Max(1, outs)

            Dim par As New List(Of Object)
            par.Add(outs)
            par.Add(perp)
            par.Add(thet)
            par.Add(seed)

            Dim thisp As GH_Path = DA.ParameterTargetPath(0)
            thisp = thisp.AppendElement(DA.ParameterTargetIndex(0))

            Me.AllSetUp = True

            Dim nt As New TensorSetTask(data, par, AddressOf ComponentTensorAction, thisp)
            Me.MyTasks.Add(nt)
            Exit Sub
        End If

    End Sub

    Public Overrides Function ComponentTensorAction(TS As TensorSet, Params As List(Of Object)) As List(Of Object)
        ac.Math.Random.Generator.Seed = Params(3)
        Dim nn As New ac.MachineLearning.Clustering.TSNE()

        nn.NumberOfInputs = TS(0).Length
        nn.NumberOfOutputs = Params(0)
        nn.Perplexity = Params(1)
        nn.Theta = Params(2)
        Dim res()() As Double = nn.Transform(TS.AsArrayArray)

        Return New List(Of Object)({res})
    End Function
End Class
