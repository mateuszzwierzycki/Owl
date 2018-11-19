<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class LifeSaverControl
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.PushChanges = New System.Windows.Forms.Button()
        Me.SolverEnabled = New System.Windows.Forms.CheckBox()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.LabelBox = New System.Windows.Forms.RichTextBox()
        Me.InputBox = New System.Windows.Forms.RichTextBox()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'PushChanges
        '
        Me.PushChanges.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PushChanges.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.PushChanges.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.PushChanges.Location = New System.Drawing.Point(266, 186)
        Me.PushChanges.Margin = New System.Windows.Forms.Padding(10)
        Me.PushChanges.Name = "PushChanges"
        Me.PushChanges.Padding = New System.Windows.Forms.Padding(1)
        Me.PushChanges.Size = New System.Drawing.Size(236, 39)
        Me.PushChanges.TabIndex = 0
        Me.PushChanges.Text = "Push to Grasshopper"
        Me.PushChanges.UseVisualStyleBackColor = True
        '
        'SolverEnabled
        '
        Me.SolverEnabled.AutoSize = True
        Me.SolverEnabled.Checked = True
        Me.SolverEnabled.CheckState = System.Windows.Forms.CheckState.Checked
        Me.SolverEnabled.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SolverEnabled.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.SolverEnabled.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SolverEnabled.Location = New System.Drawing.Point(10, 186)
        Me.SolverEnabled.Margin = New System.Windows.Forms.Padding(10)
        Me.SolverEnabled.Name = "SolverEnabled"
        Me.SolverEnabled.Padding = New System.Windows.Forms.Padding(1)
        Me.SolverEnabled.Size = New System.Drawing.Size(236, 39)
        Me.SolverEnabled.TabIndex = 1
        Me.SolverEnabled.Text = "Solver Enabled"
        Me.SolverEnabled.UseVisualStyleBackColor = True
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.AutoSize = True
        Me.TableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.LabelBox, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.SolverEnabled, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.InputBox, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.PushChanges, 1, 1)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 2
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 59.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(512, 235)
        Me.TableLayoutPanel1.TabIndex = 2
        '
        'LabelBox
        '
        Me.LabelBox.BackColor = System.Drawing.SystemColors.Control
        Me.LabelBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.LabelBox.DetectUrls = False
        Me.LabelBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LabelBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelBox.Location = New System.Drawing.Point(10, 10)
        Me.LabelBox.Margin = New System.Windows.Forms.Padding(10)
        Me.LabelBox.Name = "LabelBox"
        Me.LabelBox.ReadOnly = True
        Me.LabelBox.Size = New System.Drawing.Size(236, 156)
        Me.LabelBox.TabIndex = 1
        Me.LabelBox.Text = "Add a list of strings to get some text here..."
        '
        'InputBox
        '
        Me.InputBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.InputBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.InputBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.InputBox.Location = New System.Drawing.Point(266, 10)
        Me.InputBox.Margin = New System.Windows.Forms.Padding(10)
        Me.InputBox.Name = "InputBox"
        Me.InputBox.Size = New System.Drawing.Size(236, 156)
        Me.InputBox.TabIndex = 0
        Me.InputBox.Text = ""
        '
        'LifeSaverControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(512, 235)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
        Me.Name = "LifeSaverControl"
        Me.ShowIcon = False
        Me.Text = "LifeSaverControl"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents PushChanges As Windows.Forms.Button
    Friend WithEvents SolverEnabled As Windows.Forms.CheckBox
    Friend WithEvents TableLayoutPanel1 As Windows.Forms.TableLayoutPanel
    Friend WithEvents LabelBox As Windows.Forms.RichTextBox
    Friend WithEvents InputBox As Windows.Forms.RichTextBox
End Class
