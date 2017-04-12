Imports System.Drawing
Imports System.Windows.Forms
Imports GH_IO.Serialization
Imports Grasshopper.GUI
Imports Grasshopper.Kernel

Public MustInherit Class ImageComponentBase
    Inherits OwlComponentBase

    Private _w As Single = 100
    Private _h As Single = 100
    Private _resolution As Single = 1

    Public MustOverride Function ProvideBackground() As Bitmap
    Public MustOverride Function ProvideForeground() As Bitmap

    Private _backimage As Bitmap = Nothing
    Private _foreimage As Bitmap = Nothing

    Private _minw As Single = 50
    Private _minh As Single = 50

    Private _maxw As Single = 800
    Private _maxh As Single = 800

    Private _resizeable As Boolean = True
    Private _lockedsize As Boolean = False

    Private _tilebackground As Boolean = False

    Sub New(Name As String, Nickname As String, Description As String, SubCategory As String, Optional SizeLocked As Boolean = False)
        MyBase.New(Name, Nickname, Description, SubCategory)
        Resizeable = Not SizeLocked
    End Sub

    Public Overrides Sub CreateAttributes()
        m_attributes = New ImageComponent_Attributes(Me)
    End Sub

    Protected Overrides Sub AppendAdditionalComponentMenuItems(menu As ToolStripDropDown)

        If Resizeable Then
            Dim thiswidth As ToolStripMenuItem = Menu_AppendItem(menu, "Width")
            Dim thisheight As ToolStripMenuItem = Menu_AppendItem(menu, "Height")
            Dim thisres As ToolStripMenuItem = Menu_AppendItem(menu, "Resolution")

            Menu_AppendTextItem(thiswidth.DropDown, FrameWidth, AddressOf OnWidthKeyDown, AddressOf OnWidthTextChanged, True)
            Menu_AppendTextItem(thisheight.DropDown, FrameHeight, AddressOf OnHeightKeyDown, AddressOf OnHeightTextChanged, True)
            Menu_AppendTextItem(thisres.DropDown, Resolution, AddressOf OnResolutionKeyDown, AddressOf OnResolutionTextChanged, True)

            Menu_AppendItem(menu, "Lock Size", AddressOf OnLocking, True, Me.LockedSize)
            Menu_AppendSeparator(menu)
        End If

        Menu_AppendItem(menu, "Save Image", AddressOf SaveForeground, True)

        MyBase.AppendAdditionalComponentMenuItems(menu)
    End Sub

    Public Sub SaveForeground()
        Dim save As New System.Windows.Forms.SaveFileDialog()

        save.Filter = "PNG Files (*.png)|*.png"

        If save.ShowDialog = System.Windows.Forms.DialogResult.OK Then
            Foreground.Save(save.FileName)
        End If
    End Sub

    Private Sub OnLocking()
        Me.LockedSize = Not Me.LockedSize
    End Sub

    ''' <summary>
    ''' Changes the width with respect to the min and max values. Updates the images.
    ''' </summary>
    ''' <param name="Value"></param>
    Public Sub ChangeWidth(Value As Integer, Optional Immediate As Boolean = True)
        Me.FrameWidth = Math.Min(MaxWidth, Math.Max(Value, MinWidth))
        UpdateImages()
        If Immediate Then Me.Attributes.ExpireLayout()
        If Immediate Then Me.OnDisplayExpired(True)
    End Sub

    ''' <summary>
    ''' Changes the size and updates the images.
    ''' </summary>
    ''' <param name="Width"></param>
    ''' <param name="Height"></param>
    ''' <param name="Immediate"></param>
    Public Sub ChangeSize(Width As Integer, Height As Integer, Optional Immediate As Boolean = True)
        Me.FrameWidth = Math.Min(MaxWidth, Math.Max(Width, MinWidth))
        Me.FrameHeight = Math.Min(MaxHeight, Math.Max(Height, MinHeight))
        UpdateImages()
        If Immediate Then Me.Attributes.ExpireLayout()
        If Immediate Then Me.OnDisplayExpired(True)
    End Sub

    ''' <summary>
    ''' Changes the heigth with respect to the min and max values. Updates the images.
    ''' </summary>
    ''' <param name="Value"></param>
    Public Sub ChangeHeight(Value As Integer, Optional Immediate As Boolean = True)
        Me.FrameHeight = Math.Min(MaxHeight, Math.Max(Value, MinHeight))
        UpdateImages()
        If Immediate Then Me.Attributes.ExpireLayout()
        If Immediate Then Me.OnDisplayExpired(True)
    End Sub

    Public Sub ChangeResolution(Value As Single, Optional Immediate As Boolean = True)
        Me.Resolution = Math.Min(30, Math.Max(Value, 1))
        UpdateImages()
        If Immediate Then Me.Attributes.ExpireLayout()
        If Immediate Then Me.OnDisplayExpired(True)
    End Sub

    Private Sub OnWidthTextChanged(sender As GH_MenuTextBox, text As String)
        sender.Text = JustDigits(text)
    End Sub

    Private Sub OnWidthKeyDown(sender As GH_MenuTextBox, e As KeyEventArgs)
        If e.KeyCode = Keys.Enter And sender.Text <> "" Then
            ChangeWidth(CInt(sender.Text))
        End If
    End Sub

    Private Sub OnHeightTextChanged(sender As GH_MenuTextBox, text As String)
        sender.Text = JustDigits(text)
    End Sub

    Private Sub OnHeightKeyDown(sender As GH_MenuTextBox, e As KeyEventArgs)
        If e.KeyCode = Keys.Enter And sender.Text <> "" Then
            ChangeHeight(CInt(sender.Text))
        End If
    End Sub

    Private Sub OnResolutionTextChanged(sender As GH_MenuTextBox, text As String)
        sender.Text = JustDigits(text)
    End Sub

    Private Sub OnResolutionKeyDown(sender As GH_MenuTextBox, e As KeyEventArgs)
        If e.KeyCode = Keys.Enter And sender.Text <> "" Then
            ChangeResolution(CInt(sender.Text))
        End If
    End Sub

    Private Function JustDigits(S As String) As String
        Dim ns As String = ""
        For Each c As Char In S
            If Char.IsDigit(c) Then ns &= c
        Next
        Return ns
    End Function

    Public Overrides Function Write(writer As GH_IWriter) As Boolean
        writer.SetSingle("W", FrameWidth)
        writer.SetSingle("H", FrameHeight)
        writer.SetSingle("R", Resolution)
        writer.SetSingle("MaxW", MaxWidth)
        writer.SetSingle("MaxH", MaxHeight)
        writer.SetSingle("MinW", MinWidth)
        writer.SetSingle("MinH", MinHeight)

        writer.SetBoolean("Lock", LockedSize)
        Return MyBase.Write(writer)
    End Function

    Public Overrides Function Read(reader As GH_IReader) As Boolean
        FrameWidth = reader.GetSingle("W")
        FrameHeight = reader.GetSingle("H")
        Resolution = reader.GetSingle("R")
        LockedSize = reader.GetBoolean("Lock")

        MinWidth = reader.GetSingle("MinW")
        MinHeight = reader.GetSingle("MinH")

        MaxWidth = reader.GetSingle("MaxW")
        MaxHeight = reader.GetSingle("MaxH")

        UpdateImages()
        Return MyBase.Read(reader)
    End Function

    Public Sub UpdateImages()
        Me.Background = ProvideBackground()
        Me.Foreground = ProvideForeground()
    End Sub

    Public Property Background As Bitmap
        Get
            Return _backimage
        End Get
        Set(value As Bitmap)
            _backimage = value
        End Set
    End Property

    Public Property Foreground As Bitmap
        Get
            Return _foreimage
        End Get
        Set(value As Bitmap)
            _foreimage = value
        End Set
    End Property

    Public ReadOnly Property ImageWidth As Integer
        Get
            Return _w * Resolution
        End Get
    End Property

    Public ReadOnly Property ImageHeight As Integer
        Get
            Return _h * Resolution
        End Get
    End Property

    ''' <summary>
    ''' Frame width in the canvas units
    ''' </summary>
    ''' <returns></returns>
    Public Property FrameWidth As Single
        Get
            Return _w
        End Get
        Set(value As Single)
            _w = Math.Max(MinWidth, Math.Min(MaxWidth, value))
        End Set
    End Property

    ''' <summary>
    ''' Frame height in the canvas units
    ''' </summary>
    ''' <returns></returns>
    Public Property FrameHeight As Single
        Get
            Return _h
        End Get
        Set(value As Single)
            _h = Math.Max(MinHeight, Math.Min(MaxHeight, value))
        End Set
    End Property

    Public Property Resolution As Single
        Get
            Return _resolution
        End Get
        Set(value As Single)
            _resolution = value
        End Set
    End Property

    Public Property MinWidth As Single
        Get
            Return _minw
        End Get
        Set(value As Single)
            _minw = value
        End Set
    End Property

    ''' <summary>
    ''' Typically 20 * Parameters.Count
    ''' </summary>
    ''' <returns></returns>
    Public Property MinHeight As Single
        Get
            Return _minh
        End Get
        Set(value As Single)
            _minh = value
        End Set
    End Property

    ''' <summary>
    ''' Generally resizeable.
    ''' </summary>
    ''' <returns></returns>
    Public Property Resizeable As Boolean
        Get
            Return _resizeable
        End Get
        Set(value As Boolean)
            _resizeable = value
        End Set
    End Property

    ''' <summary>
    ''' Locked by the user.
    ''' </summary>
    ''' <returns></returns>
    Public Property LockedSize As Boolean
        Get
            Return _lockedsize
        End Get
        Set(value As Boolean)
            _lockedsize = value
        End Set
    End Property

    Public Property MaxWidth As Single
        Get
            Return _maxw
        End Get
        Set(value As Single)
            _maxw = value
        End Set
    End Property

    Public Property MaxHeight As Single
        Get
            Return _maxh
        End Get
        Set(value As Single)
            _maxh = value
        End Set
    End Property

    Public Property TileBackground As Boolean
        Get
            Return _tilebackground
        End Get
        Set(value As Boolean)
            _tilebackground = value
        End Set
    End Property
End Class
