<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MQTTConnectForm
	Inherits System.Windows.Forms.Form

	'Form overrides dispose to clean up the component list.
	<System.Diagnostics.DebuggerNonUserCode()>
	Protected Overrides Sub Dispose(ByVal disposing As Boolean)
		Try
			If disposing AndAlso components IsNot Nothing Then
				components.Dispose()
			End If
		Finally
			MyBase.Dispose(disposing)
		End Try
	End Sub

	'Required by the Windows Form Designer
	Private components As System.ComponentModel.IContainer

	'NOTE: The following procedure is required by the Windows Form Designer
	'It can be modified using the Windows Form Designer.  
	'Do not modify it using the code editor.
	<System.Diagnostics.DebuggerStepThrough()>
	Private Sub InitializeComponent()
		Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
		Me.Label9 = New System.Windows.Forms.Label()
		Me.TopicInput = New System.Windows.Forms.TextBox()
		Me.BoxKeepAlive = New System.Windows.Forms.NumericUpDown()
		Me.BoxReconnect = New System.Windows.Forms.NumericUpDown()
		Me.BoxTimeout = New System.Windows.Forms.NumericUpDown()
		Me.Label7 = New System.Windows.Forms.Label()
		Me.Label8 = New System.Windows.Forms.Label()
		Me.Label6 = New System.Windows.Forms.Label()
		Me.BoxPassword = New System.Windows.Forms.TextBox()
		Me.BoxUsername = New System.Windows.Forms.TextBox()
		Me.BoxHost = New System.Windows.Forms.TextBox()
		Me.Label1 = New System.Windows.Forms.Label()
		Me.Label2 = New System.Windows.Forms.Label()
		Me.BoxClientID = New System.Windows.Forms.TextBox()
		Me.Label4 = New System.Windows.Forms.Label()
		Me.Label5 = New System.Windows.Forms.Label()
		Me.Label3 = New System.Windows.Forms.Label()
		Me.BoxPort = New System.Windows.Forms.NumericUpDown()
		Me.ButtonConnect = New System.Windows.Forms.Button()
		Me.ButtonDisconnect = New System.Windows.Forms.Button()
		Me.TopicList = New System.Windows.Forms.ListBox()
		Me.CleanSessionBox = New System.Windows.Forms.CheckBox()
		Me.QOSBox = New System.Windows.Forms.ListBox()
		Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
		Me.OutputText = New System.Windows.Forms.Label()
		Me.TableLayoutPanel1.SuspendLayout()
		CType(Me.BoxKeepAlive, System.ComponentModel.ISupportInitialize).BeginInit()
		CType(Me.BoxReconnect, System.ComponentModel.ISupportInitialize).BeginInit()
		CType(Me.BoxTimeout, System.ComponentModel.ISupportInitialize).BeginInit()
		CType(Me.BoxPort, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.TableLayoutPanel2.SuspendLayout()
		Me.SuspendLayout()
		'
		'TableLayoutPanel1
		'
		Me.TableLayoutPanel1.ColumnCount = 2
		Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
		Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 66.66667!))
		Me.TableLayoutPanel1.Controls.Add(Me.Label9, 0, 8)
		Me.TableLayoutPanel1.Controls.Add(Me.TopicInput, 0, 10)
		Me.TableLayoutPanel1.Controls.Add(Me.BoxKeepAlive, 1, 7)
		Me.TableLayoutPanel1.Controls.Add(Me.BoxReconnect, 1, 5)
		Me.TableLayoutPanel1.Controls.Add(Me.BoxTimeout, 1, 6)
		Me.TableLayoutPanel1.Controls.Add(Me.Label7, 0, 7)
		Me.TableLayoutPanel1.Controls.Add(Me.Label8, 0, 6)
		Me.TableLayoutPanel1.Controls.Add(Me.Label6, 0, 5)
		Me.TableLayoutPanel1.Controls.Add(Me.BoxPassword, 1, 4)
		Me.TableLayoutPanel1.Controls.Add(Me.BoxUsername, 1, 3)
		Me.TableLayoutPanel1.Controls.Add(Me.BoxHost, 1, 1)
		Me.TableLayoutPanel1.Controls.Add(Me.Label1, 0, 0)
		Me.TableLayoutPanel1.Controls.Add(Me.Label2, 0, 1)
		Me.TableLayoutPanel1.Controls.Add(Me.BoxClientID, 1, 0)
		Me.TableLayoutPanel1.Controls.Add(Me.Label4, 0, 4)
		Me.TableLayoutPanel1.Controls.Add(Me.Label5, 0, 2)
		Me.TableLayoutPanel1.Controls.Add(Me.Label3, 0, 3)
		Me.TableLayoutPanel1.Controls.Add(Me.BoxPort, 1, 2)
		Me.TableLayoutPanel1.Controls.Add(Me.ButtonConnect, 1, 11)
		Me.TableLayoutPanel1.Controls.Add(Me.ButtonDisconnect, 0, 11)
		Me.TableLayoutPanel1.Controls.Add(Me.TopicList, 1, 10)
		Me.TableLayoutPanel1.Controls.Add(Me.CleanSessionBox, 1, 9)
		Me.TableLayoutPanel1.Controls.Add(Me.QOSBox, 1, 8)
		Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
		Me.TableLayoutPanel1.Location = New System.Drawing.Point(3, 3)
		Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
		Me.TableLayoutPanel1.RowCount = 12
		Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.666667!))
		Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.666667!))
		Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.666667!))
		Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.666667!))
		Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.666667!))
		Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.666667!))
		Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.666667!))
		Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.666667!))
		Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 13.33333!))
		Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.666667!))
		Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20.0!))
		Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.666667!))
		Me.TableLayoutPanel1.Size = New System.Drawing.Size(425, 560)
		Me.TableLayoutPanel1.TabIndex = 0
		'
		'Label9
		'
		Me.Label9.AutoSize = True
		Me.Label9.Dock = System.Windows.Forms.DockStyle.Fill
		Me.Label9.Location = New System.Drawing.Point(3, 296)
		Me.Label9.Name = "Label9"
		Me.Label9.Size = New System.Drawing.Size(135, 74)
		Me.Label9.TabIndex = 18
		Me.Label9.Text = "QoS"
		Me.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'TopicInput
		'
		Me.TopicInput.Dock = System.Windows.Forms.DockStyle.Fill
		Me.TopicInput.Location = New System.Drawing.Point(3, 410)
		Me.TopicInput.Name = "TopicInput"
		Me.TopicInput.Size = New System.Drawing.Size(135, 20)
		Me.TopicInput.TabIndex = 9
		'
		'BoxKeepAlive
		'
		Me.BoxKeepAlive.Dock = System.Windows.Forms.DockStyle.Fill
		Me.BoxKeepAlive.Location = New System.Drawing.Point(144, 262)
		Me.BoxKeepAlive.Maximum = New Decimal(New Integer() {100000, 0, 0, 0})
		Me.BoxKeepAlive.Name = "BoxKeepAlive"
		Me.BoxKeepAlive.Size = New System.Drawing.Size(278, 20)
		Me.BoxKeepAlive.TabIndex = 7
		Me.BoxKeepAlive.Value = New Decimal(New Integer() {10, 0, 0, 0})
		'
		'BoxReconnect
		'
		Me.BoxReconnect.Dock = System.Windows.Forms.DockStyle.Fill
		Me.BoxReconnect.Location = New System.Drawing.Point(144, 188)
		Me.BoxReconnect.Maximum = New Decimal(New Integer() {100000, 0, 0, 0})
		Me.BoxReconnect.Name = "BoxReconnect"
		Me.BoxReconnect.Size = New System.Drawing.Size(278, 20)
		Me.BoxReconnect.TabIndex = 5
		Me.BoxReconnect.Value = New Decimal(New Integer() {1000, 0, 0, 0})
		'
		'BoxTimeout
		'
		Me.BoxTimeout.Dock = System.Windows.Forms.DockStyle.Fill
		Me.BoxTimeout.Location = New System.Drawing.Point(144, 225)
		Me.BoxTimeout.Maximum = New Decimal(New Integer() {1000, 0, 0, 0})
		Me.BoxTimeout.Name = "BoxTimeout"
		Me.BoxTimeout.Size = New System.Drawing.Size(278, 20)
		Me.BoxTimeout.TabIndex = 6
		Me.BoxTimeout.Value = New Decimal(New Integer() {30, 0, 0, 0})
		'
		'Label7
		'
		Me.Label7.AutoSize = True
		Me.Label7.Dock = System.Windows.Forms.DockStyle.Fill
		Me.Label7.Location = New System.Drawing.Point(3, 259)
		Me.Label7.Name = "Label7"
		Me.Label7.Size = New System.Drawing.Size(135, 37)
		Me.Label7.TabIndex = 16
		Me.Label7.Text = "Keep Alive [s]"
		Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'Label8
		'
		Me.Label8.AutoSize = True
		Me.Label8.Dock = System.Windows.Forms.DockStyle.Fill
		Me.Label8.Location = New System.Drawing.Point(3, 222)
		Me.Label8.Name = "Label8"
		Me.Label8.Size = New System.Drawing.Size(135, 37)
		Me.Label8.TabIndex = 15
		Me.Label8.Text = "Connection Timeout [s]"
		Me.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'Label6
		'
		Me.Label6.AutoSize = True
		Me.Label6.Dock = System.Windows.Forms.DockStyle.Fill
		Me.Label6.Location = New System.Drawing.Point(3, 185)
		Me.Label6.Name = "Label6"
		Me.Label6.Size = New System.Drawing.Size(135, 37)
		Me.Label6.TabIndex = 13
		Me.Label6.Text = "Reconnect Period [ms]"
		Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'BoxPassword
		'
		Me.BoxPassword.Dock = System.Windows.Forms.DockStyle.Fill
		Me.BoxPassword.Location = New System.Drawing.Point(144, 151)
		Me.BoxPassword.Name = "BoxPassword"
		Me.BoxPassword.Size = New System.Drawing.Size(278, 20)
		Me.BoxPassword.TabIndex = 4
		'
		'BoxUsername
		'
		Me.BoxUsername.Dock = System.Windows.Forms.DockStyle.Fill
		Me.BoxUsername.Location = New System.Drawing.Point(144, 114)
		Me.BoxUsername.Name = "BoxUsername"
		Me.BoxUsername.Size = New System.Drawing.Size(278, 20)
		Me.BoxUsername.TabIndex = 3
		'
		'BoxHost
		'
		Me.BoxHost.Dock = System.Windows.Forms.DockStyle.Fill
		Me.BoxHost.Location = New System.Drawing.Point(144, 40)
		Me.BoxHost.Name = "BoxHost"
		Me.BoxHost.Size = New System.Drawing.Size(278, 20)
		Me.BoxHost.TabIndex = 1
		Me.BoxHost.Text = "broker.hivemq.com"
		'
		'Label1
		'
		Me.Label1.AutoSize = True
		Me.Label1.Dock = System.Windows.Forms.DockStyle.Fill
		Me.Label1.Location = New System.Drawing.Point(3, 0)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(135, 37)
		Me.Label1.TabIndex = 0
		Me.Label1.Text = "Client ID"
		Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'Label2
		'
		Me.Label2.AutoSize = True
		Me.Label2.Dock = System.Windows.Forms.DockStyle.Fill
		Me.Label2.Location = New System.Drawing.Point(3, 37)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New System.Drawing.Size(135, 37)
		Me.Label2.TabIndex = 1
		Me.Label2.Text = "Host"
		Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'BoxClientID
		'
		Me.BoxClientID.Dock = System.Windows.Forms.DockStyle.Fill
		Me.BoxClientID.Location = New System.Drawing.Point(144, 3)
		Me.BoxClientID.Name = "BoxClientID"
		Me.BoxClientID.Size = New System.Drawing.Size(278, 20)
		Me.BoxClientID.TabIndex = 0
		'
		'Label4
		'
		Me.Label4.AutoSize = True
		Me.Label4.Dock = System.Windows.Forms.DockStyle.Fill
		Me.Label4.Location = New System.Drawing.Point(3, 148)
		Me.Label4.Name = "Label4"
		Me.Label4.Size = New System.Drawing.Size(135, 37)
		Me.Label4.TabIndex = 3
		Me.Label4.Text = "Password"
		Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'Label5
		'
		Me.Label5.AutoSize = True
		Me.Label5.Dock = System.Windows.Forms.DockStyle.Fill
		Me.Label5.Location = New System.Drawing.Point(3, 74)
		Me.Label5.Name = "Label5"
		Me.Label5.Size = New System.Drawing.Size(135, 37)
		Me.Label5.TabIndex = 4
		Me.Label5.Text = "Port"
		Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'Label3
		'
		Me.Label3.AutoSize = True
		Me.Label3.Dock = System.Windows.Forms.DockStyle.Fill
		Me.Label3.Location = New System.Drawing.Point(3, 111)
		Me.Label3.Name = "Label3"
		Me.Label3.Size = New System.Drawing.Size(135, 37)
		Me.Label3.TabIndex = 2
		Me.Label3.Text = "Username"
		Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'BoxPort
		'
		Me.BoxPort.Dock = System.Windows.Forms.DockStyle.Fill
		Me.BoxPort.Location = New System.Drawing.Point(144, 77)
		Me.BoxPort.Maximum = New Decimal(New Integer() {65535, 0, 0, 0})
		Me.BoxPort.Name = "BoxPort"
		Me.BoxPort.Size = New System.Drawing.Size(278, 20)
		Me.BoxPort.TabIndex = 2
		Me.BoxPort.Value = New Decimal(New Integer() {1883, 0, 0, 0})
		'
		'ButtonConnect
		'
		Me.ButtonConnect.Dock = System.Windows.Forms.DockStyle.Fill
		Me.ButtonConnect.Location = New System.Drawing.Point(144, 522)
		Me.ButtonConnect.Name = "ButtonConnect"
		Me.ButtonConnect.Size = New System.Drawing.Size(278, 35)
		Me.ButtonConnect.TabIndex = 11
		Me.ButtonConnect.Text = "Connect"
		Me.ButtonConnect.UseVisualStyleBackColor = True
		'
		'ButtonDisconnect
		'
		Me.ButtonDisconnect.Dock = System.Windows.Forms.DockStyle.Fill
		Me.ButtonDisconnect.Location = New System.Drawing.Point(3, 522)
		Me.ButtonDisconnect.Name = "ButtonDisconnect"
		Me.ButtonDisconnect.Size = New System.Drawing.Size(135, 35)
		Me.ButtonDisconnect.TabIndex = 12
		Me.ButtonDisconnect.Text = "Disconnect"
		Me.ButtonDisconnect.UseVisualStyleBackColor = True
		'
		'TopicList
		'
		Me.TopicList.Dock = System.Windows.Forms.DockStyle.Fill
		Me.TopicList.FormattingEnabled = True
		Me.TopicList.Location = New System.Drawing.Point(144, 410)
		Me.TopicList.Name = "TopicList"
		Me.TopicList.ScrollAlwaysVisible = True
		Me.TopicList.Size = New System.Drawing.Size(278, 106)
		Me.TopicList.TabIndex = 10
		'
		'CleanSessionBox
		'
		Me.CleanSessionBox.AutoSize = True
		Me.CleanSessionBox.Location = New System.Drawing.Point(144, 373)
		Me.CleanSessionBox.Name = "CleanSessionBox"
		Me.CleanSessionBox.Size = New System.Drawing.Size(93, 17)
		Me.CleanSessionBox.TabIndex = 20
		Me.CleanSessionBox.Text = "Clean Session"
		Me.CleanSessionBox.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.CleanSessionBox.UseVisualStyleBackColor = True
		'
		'QOSBox
		'
		Me.QOSBox.Dock = System.Windows.Forms.DockStyle.Fill
		Me.QOSBox.FormattingEnabled = True
		Me.QOSBox.Items.AddRange(New Object() {"At Most Once (0)", "At Least Once (1)", "Exactly Once (2)"})
		Me.QOSBox.Location = New System.Drawing.Point(144, 299)
		Me.QOSBox.Name = "QOSBox"
		Me.QOSBox.Size = New System.Drawing.Size(278, 68)
		Me.QOSBox.TabIndex = 21
		'
		'TableLayoutPanel2
		'
		Me.TableLayoutPanel2.ColumnCount = 1
		Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
		Me.TableLayoutPanel2.Controls.Add(Me.OutputText, 0, 1)
		Me.TableLayoutPanel2.Controls.Add(Me.TableLayoutPanel1, 0, 0)
		Me.TableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill
		Me.TableLayoutPanel2.Location = New System.Drawing.Point(0, 0)
		Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
		Me.TableLayoutPanel2.RowCount = 2
		Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 90.0!))
		Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.0!))
		Me.TableLayoutPanel2.Size = New System.Drawing.Size(431, 629)
		Me.TableLayoutPanel2.TabIndex = 1
		'
		'OutputText
		'
		Me.OutputText.AutoSize = True
		Me.OutputText.Dock = System.Windows.Forms.DockStyle.Fill
		Me.OutputText.Location = New System.Drawing.Point(3, 566)
		Me.OutputText.Name = "OutputText"
		Me.OutputText.Size = New System.Drawing.Size(425, 63)
		Me.OutputText.TabIndex = 17
		Me.OutputText.Text = "Press Connect..."
		Me.OutputText.TextAlign = System.Drawing.ContentAlignment.TopCenter
		'
		'MQTTConnectForm
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(431, 629)
		Me.Controls.Add(Me.TableLayoutPanel2)
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
		Me.KeyPreview = True
		Me.Name = "MQTTConnectForm"
		Me.ShowIcon = False
		Me.ShowInTaskbar = False
		Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
		Me.Text = "MQTT_Connect"
		Me.TopMost = True
		Me.TableLayoutPanel1.ResumeLayout(False)
		Me.TableLayoutPanel1.PerformLayout()
		CType(Me.BoxKeepAlive, System.ComponentModel.ISupportInitialize).EndInit()
		CType(Me.BoxReconnect, System.ComponentModel.ISupportInitialize).EndInit()
		CType(Me.BoxTimeout, System.ComponentModel.ISupportInitialize).EndInit()
		CType(Me.BoxPort, System.ComponentModel.ISupportInitialize).EndInit()
		Me.TableLayoutPanel2.ResumeLayout(False)
		Me.TableLayoutPanel2.PerformLayout()
		Me.ResumeLayout(False)

	End Sub

	Friend WithEvents TableLayoutPanel1 As Windows.Forms.TableLayoutPanel
	Friend WithEvents BoxKeepAlive As Windows.Forms.NumericUpDown
	Friend WithEvents BoxReconnect As Windows.Forms.NumericUpDown
	Friend WithEvents Label7 As Windows.Forms.Label
	Friend WithEvents Label8 As Windows.Forms.Label
	Friend WithEvents Label6 As Windows.Forms.Label
	Friend WithEvents BoxPassword As Windows.Forms.TextBox
	Friend WithEvents BoxUsername As Windows.Forms.TextBox
	Friend WithEvents BoxHost As Windows.Forms.TextBox
	Friend WithEvents Label1 As Windows.Forms.Label
	Friend WithEvents Label2 As Windows.Forms.Label
	Friend WithEvents BoxClientID As Windows.Forms.TextBox
	Friend WithEvents Label4 As Windows.Forms.Label
	Friend WithEvents Label5 As Windows.Forms.Label
	Friend WithEvents Label3 As Windows.Forms.Label
	Friend WithEvents BoxPort As Windows.Forms.NumericUpDown
	Friend WithEvents BoxTimeout As Windows.Forms.NumericUpDown
	Friend WithEvents TopicInput As Windows.Forms.TextBox
	Friend WithEvents TopicList As Windows.Forms.ListBox
	Friend WithEvents TableLayoutPanel2 As Windows.Forms.TableLayoutPanel
	Friend WithEvents OutputText As Windows.Forms.Label
	Friend WithEvents Label9 As Windows.Forms.Label
	Friend WithEvents CleanSessionBox As Windows.Forms.CheckBox
	Friend WithEvents ButtonConnect As Windows.Forms.Button
	Friend WithEvents ButtonDisconnect As Windows.Forms.Button
	Friend WithEvents QOSBox As Windows.Forms.ListBox
End Class
