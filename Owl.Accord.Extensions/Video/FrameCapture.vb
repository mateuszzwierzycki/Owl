Imports System.Drawing
Imports Accord.Video
Imports Accord.Video.DirectShow

Namespace Video

    ''' <summary>
    ''' TODO this class
    ''' </summary>
    Public Class RunningCapture

        Private _devs As FilterInfoCollection = Nothing
        Private _image As Bitmap = Nothing
        Private _dev As VideoCaptureDevice = Nothing

        Sub New()
            Devices = ListDevices()
        End Sub

        Public Property Devices As FilterInfoCollection
            Get
                Return _devs
            End Get
            Set(value As FilterInfoCollection)
                _devs = value
            End Set
        End Property

        Public Property LastCapture As Bitmap
            Get
                If _image Is Nothing Then Return Nothing
                SyncLock _image
                    Return _image
                End SyncLock
            End Get
            Set(value As Bitmap)
                If _image Is Nothing Then
                    _image = value
                Else
                    SyncLock _image
                        _image = value
                    End SyncLock
                End If
            End Set
        End Property

        ''' <summary>
        ''' Run after you connect a new device.
        ''' </summary>
        ''' <returns></returns>
        Public Function ListDevices() As FilterInfoCollection
            Dim videoDevices = New FilterInfoCollection(FilterCategory.VideoInputDevice)
            _devs = videoDevices
            Return videoDevices
        End Function

        ''' <summary>
        ''' You can get the list of all available devices from the Devices property.
        ''' </summary>
        ''' <param name="SelectedDevice"></param>
        Public Function CameraTurnOn(SelectedDevice As FilterInfo, Optional CameraResolution As Integer = -1) As Boolean
            If _dev IsNot Nothing Then

                Return False 'its already running z
            End If

            Dim ok As Boolean = True
            _dev = New VideoCaptureDevice(SelectedDevice.MonikerString, Imaging.PixelFormat.Format24bppRgb)

            If CameraResolution = -1 Then
                Dim maxi As Integer = -1
                Dim maxw As Integer = 0

                For i As Integer = 0 To _dev.VideoCapabilities.Count - 1 Step 1
                    If _dev.VideoCapabilities(i).FrameSize.Width > maxw Then
                        maxw = _dev.VideoCapabilities(i).FrameSize.Width
                        maxi = i
                    End If
                Next

                _dev.VideoResolution = _dev.VideoCapabilities(maxi)
            Else
                If CameraResolution > _dev.VideoCapabilities.Length - 1 Then
                    'do nothing, will use the default one
                    ok = False
                Else
                    _dev.VideoResolution = _dev.VideoCapabilities(CameraResolution)
                End If
            End If

            AddHandler _dev.NewFrame, AddressOf GetFrame
            _dev.Start()
            Return ok
        End Function

        Public Sub CameraTurnOff()
            capture = False
            If _dev Is Nothing Then Return
            _dev.Stop()
            _dev.WaitForStop()
            RemoveHandler _dev.NewFrame, AddressOf GetFrame
            _dev = Nothing
        End Sub

        Public Sub TakePicture()
            If _dev Is Nothing Then Return
            capture = True

            Do
                If Not capture Then Exit Do
                Threading.Thread.Sleep(20)
            Loop

        End Sub

        Public capture As Boolean = False

        Private Sub GetFrame(sender As Object, e As NewFrameEventArgs)
            If capture = False Then Return
            If Me._image IsNot Nothing Then Me.LastCapture.Dispose()
            Me.LastCapture = New Bitmap(e.Frame)
            capture = False
        End Sub

    End Class

    ''' <summary>
    ''' A simple class which can list cameras and capture single frames from them.
    ''' </summary>
    Public Class FrameCapture

        Private _devs As FilterInfoCollection = Nothing
        Private _image As Bitmap = Nothing
        Private _st As New Stopwatch()

        Sub New()
            Devices = ListDevices()
        End Sub

        Public Property Devices As FilterInfoCollection
            Get
                Return _devs
            End Get
            Set(value As FilterInfoCollection)
                _devs = value
            End Set
        End Property

        Public Property LastCapture As Bitmap
            Get
                Return _image
            End Get
            Set(value As Bitmap)
                _image = value
            End Set
        End Property

        Public Property TimeWatch As Stopwatch
            Get
                Return _st
            End Get
            Set(value As Stopwatch)
                _st = value
            End Set
        End Property

        ''' <summary>
        ''' Run after you connect a new device.
        ''' </summary>
        ''' <returns></returns>
        Public Function ListDevices() As FilterInfoCollection
            Dim videoDevices = New FilterInfoCollection(FilterCategory.VideoInputDevice)
            Return videoDevices
        End Function

        Public Function CaptureOne(SelectedDevice As FilterInfo, Optional Timeout As Integer = 1000, Optional CameraResolution As Integer = -1) As Boolean

            Dim thisdevice As VideoCaptureDevice = New VideoCaptureDevice(SelectedDevice.MonikerString, Imaging.PixelFormat.Format24bppRgb)

            Dim maxi As Integer = -1
            Dim maxw As Integer = 0

            If CameraResolution = -1 Then

                For i As Integer = 0 To thisdevice.VideoCapabilities.Count - 1 Step 1
                    If thisdevice.VideoCapabilities(i).FrameSize.Width > maxw Then
                        maxw = thisdevice.VideoCapabilities(i).FrameSize.Width
                        maxi = i
                    End If
                Next

                thisdevice.VideoResolution = thisdevice.VideoCapabilities(maxi)

            Else

                If CameraResolution > thisdevice.VideoCapabilities.Length - 1 Then
                    thisdevice.VideoResolution = thisdevice.VideoCapabilities(0)
                Else
                    thisdevice.VideoResolution = thisdevice.VideoCapabilities(CameraResolution)
                End If

            End If

            AddHandler thisdevice.NewFrame, AddressOf GetFrame
            collected = False

            TimeWatch.Reset()
            thisdevice.Start()
            TimeWatch.Start()

            While Not collected
                If TimeWatch.ElapsedMilliseconds > Timeout Then
                    thisdevice.Stop()
                    TimeWatch.Stop()
                    TimeWatch.Reset()
                    Return False
                End If
            End While

            thisdevice.Stop()
            thisdevice.WaitForStop()
            TimeWatch.Stop()

            Return True
        End Function

        Private collected As Boolean = False

        Private Sub GetFrame(sender As Object, e As NewFrameEventArgs)
            If Me._image IsNot Nothing Then Me.LastCapture.Dispose()
            Me.LastCapture = New Bitmap(e.Frame)
            collected = True
        End Sub

    End Class

End Namespace

