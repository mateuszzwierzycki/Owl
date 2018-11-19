Public Class OwlDescription
    Inherits Grasshopper.Kernel.GH_AssemblyInfo

    Public Overrides ReadOnly Property AssemblyDescription As String
        Get
            Return "Plug-in for machine-learning oriented data processing."
        End Get
    End Property

    Public Overrides ReadOnly Property AssemblyIcon As Bitmap
        Get
            Return My.Resources.icon_80
        End Get
    End Property

    Public Overrides ReadOnly Property AssemblyLicense As GH_LibraryLicense
        Get
            Return GH_LibraryLicense.free
        End Get
    End Property

    Public Overrides ReadOnly Property AssemblyName As String
        Get
            Return MyBase.AssemblyName
        End Get
    End Property

    Public Overrides ReadOnly Property AssemblyVersion As String
        Get
            Return MyBase.AssemblyVersion
        End Get
    End Property

    Public Overrides ReadOnly Property AuthorContact As String
        Get
            Return "mateuszzwierzycki@gmail.com"
        End Get
    End Property

    Public Overrides ReadOnly Property AuthorName As String
        Get
            Return "Mateusz Zwierzycki"
        End Get
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Plug-in for machine-learning oriented data processing."
        End Get
    End Property

    Public Overrides ReadOnly Property Icon As Bitmap
        Get
            Return My.Resources.icon_80
        End Get
    End Property

    Public Overrides ReadOnly Property License As GH_LibraryLicense
        Get
            Return MyBase.License
        End Get
    End Property

    Public Overrides ReadOnly Property Name As String
        Get
            Return "Owl.GH"
        End Get
    End Property

    Public Overrides ReadOnly Property Version As String
        Get
            Return "1.0.0.0"
        End Get
    End Property

End Class