Public Module LibrarySettings

    Private _printlength As Integer = 10
    Private _stringformat As String = "0.00"

    ''' <summary>
    ''' This value sets how many doubles are printed when calling the Tensor.ToString() function.
    ''' Change it only if you know what you're doing. You have to say "I know what I'm doing" out loud first.
    ''' </summary>
    ''' <returns></returns>
    Public Property TensorPrintLength As Integer
        Get
            Return _printlength
        End Get
        Set(value As Integer)
            _printlength = value
        End Set
    End Property

    ''' <summary>
    ''' This value is the string formatting used internally by the Tensor.ToString() function to print the Doubles.
    ''' Change it only if you know what you're doing. You have to say "I know what I'm doing" out loud first.
    ''' </summary>
    ''' <returns></returns>
    Public Property TensorFormat As String
        Get
            Return _stringformat
        End Get
        Set(value As String)
            _stringformat = value
        End Set
    End Property

End Module
