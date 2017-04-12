Imports System.Drawing
Imports System.Windows.Forms
Imports Grasshopper.GUI

Public MustInherit Class ImageComponentBase
    Inherits OwlComponentBase

    Private _typedatt As ImageComponent_Attributes = m_attributes
    MustOverride Sub DrawBackground(ByRef BackImage As Bitmap)
    MustOverride Sub DrawForeground(ByRef ForeImage As Bitmap)

    Sub New(Name As String, Nickname As String, Description As String, SubCategory As String)
        MyBase.New(Name, Nickname, Description, SubCategory)
    End Sub

    ''' <summary>
    ''' A place to setup the component attributes regarding the image and its properties.
    ''' </summary>
    ''' <param name="atts"></param>
    MustOverride Sub OnAttributesCreation(ByRef atts As ImageComponent_Attributes)

    Public Overrides Sub CreateAttributes()
        m_attributes = New ImageComponent_Attributes(Me)
        ImageAttributes = m_attributes
        OnAttributesCreation(ImageAttributes)
        ImageAttributes.UpdateImages()
    End Sub

    Public Sub Refresh(Optional immediate As Boolean = True)
        ImageAttributes.UpdateImages()
        If immediate Then Me.OnDisplayExpired(True)
        If immediate Then ImageAttributes.ExpireLayout()
    End Sub

    Protected Overrides Sub AppendAdditionalComponentMenuItems(menu As ToolStripDropDown)
        If ImageAttributes.Resizeable Then
            Dim thiswidth As ToolStripMenuItem = Menu_AppendItem(menu, "Width")
            Dim thisheight As ToolStripMenuItem = Menu_AppendItem(menu, "Height")
            Dim thisres As ToolStripMenuItem = Menu_AppendItem(menu, "Resolution")

            Menu_AppendTextItem(thiswidth.DropDown, ImageAttributes.FrameWidth, AddressOf OnWidthKeyDown, AddressOf OnWidthTextChanged, True)
            Menu_AppendTextItem(thisheight.DropDown, ImageAttributes.FrameHeight, AddressOf OnHeightKeyDown, AddressOf OnHeightTextChanged, True)
            Menu_AppendTextItem(thisres.DropDown, ImageAttributes.Resolution, AddressOf OnResolutionKeyDown, AddressOf OnResolutionTextChanged, True)

            Menu_AppendItem(menu, "Lock Size", AddressOf OnLocking, True, ImageAttributes.LockedSize)
            Menu_AppendSeparator(menu)
        End If

        Menu_AppendItem(menu, "Save Image", AddressOf ImageAttributes.SaveForeground, True)

        MyBase.AppendAdditionalComponentMenuItems(menu)
    End Sub

    Private Sub OnLocking()
        ImageAttributes.LockedSize = Not ImageAttributes.LockedSize
    End Sub

    ''' <summary>
    ''' Changes the size and updates the images.
    ''' </summary>
    ''' <param name="NewFrameWidth"></param>
    ''' <param name="NewFrameHeight"></param>
    ''' <param name="Immediate"></param>
    Public Sub ChangeSize(NewFrameWidth As Integer, NewFrameHeight As Integer, Optional Immediate As Boolean = True)
        ImageAttributes.Resize(Math.Min(ImageAttributes.MaxWidth, Math.Max(NewFrameWidth, ImageAttributes.MinWidth)),
        Math.Min(ImageAttributes.MaxHeight, Math.Max(NewFrameHeight, ImageAttributes.MinHeight)), ImageAttributes.Resolution, Immediate)
    End Sub

    Public Sub ChangeResolution(Value As Single, Optional Immediate As Boolean = True)
        Value = Math.Min(30, Math.Max(Value, 1))
        ImageAttributes.Resize(ImageAttributes.FrameWidth, ImageAttributes.FrameHeight, Value, Immediate)
    End Sub

    Private Sub OnWidthTextChanged(sender As GH_MenuTextBox, text As String)
        sender.Text = JustDigits(text)
    End Sub

    Private Sub OnWidthKeyDown(sender As GH_MenuTextBox, e As KeyEventArgs)
        If e.KeyCode = Keys.Enter And sender.Text <> "" Then
            ChangeSize(CInt(sender.Text), ImageAttributes.FrameHeight, ImageAttributes.Resolution)
        End If
    End Sub

    Private Sub OnHeightTextChanged(sender As GH_MenuTextBox, text As String)
        sender.Text = JustDigits(text)
    End Sub

    Private Sub OnHeightKeyDown(sender As GH_MenuTextBox, e As KeyEventArgs)
        If e.KeyCode = Keys.Enter And sender.Text <> "" Then
            ChangeSize(ImageAttributes.FrameWidth, CInt(sender.Text))
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

    Public Property ImageAttributes As ImageComponent_Attributes
        Get
            Return _typedatt
        End Get
        Set(value As ImageComponent_Attributes)
            _typedatt = value
        End Set
    End Property
End Class
