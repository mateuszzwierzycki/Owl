<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ScriptOutput
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
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.Savebut = New System.Windows.Forms.Button()
        Me.CopyBut = New System.Windows.Forms.Button()
        Me.Clearbut = New System.Windows.Forms.Button()
        Me.ClearBox = New System.Windows.Forms.CheckBox()
        Me.ConsoleBox = New System.Windows.Forms.RichTextBox()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 1
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.TableLayoutPanel2, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.ConsoleBox, 0, 0)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 2
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(688, 294)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'TableLayoutPanel2
        '
        Me.TableLayoutPanel2.AutoSize = True
        Me.TableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.TableLayoutPanel2.ColumnCount = 4
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.TableLayoutPanel2.Controls.Add(Me.Savebut, 2, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.CopyBut, 1, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.Clearbut, 1, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.ClearBox, 0, 0)
        Me.TableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel2.Location = New System.Drawing.Point(3, 258)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        Me.TableLayoutPanel2.RowCount = 1
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(682, 33)
        Me.TableLayoutPanel2.TabIndex = 0
        '
        'Savebut
        '
        Me.Savebut.AutoSize = True
        Me.Savebut.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.Savebut.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Savebut.Location = New System.Drawing.Point(515, 5)
        Me.Savebut.Margin = New System.Windows.Forms.Padding(5)
        Me.Savebut.Name = "Savebut"
        Me.Savebut.Size = New System.Drawing.Size(162, 23)
        Me.Savebut.TabIndex = 0
        Me.Savebut.Text = "Save as..."
        Me.Savebut.UseVisualStyleBackColor = True
        '
        'CopyBut
        '
        Me.CopyBut.AutoSize = True
        Me.CopyBut.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.CopyBut.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CopyBut.Location = New System.Drawing.Point(345, 5)
        Me.CopyBut.Margin = New System.Windows.Forms.Padding(5)
        Me.CopyBut.Name = "CopyBut"
        Me.CopyBut.Size = New System.Drawing.Size(160, 23)
        Me.CopyBut.TabIndex = 1
        Me.CopyBut.Text = "Copy"
        Me.CopyBut.UseVisualStyleBackColor = True
        '
        'Clearbut
        '
        Me.Clearbut.AutoSize = True
        Me.Clearbut.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.Clearbut.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Clearbut.Location = New System.Drawing.Point(175, 5)
        Me.Clearbut.Margin = New System.Windows.Forms.Padding(5)
        Me.Clearbut.Name = "Clearbut"
        Me.Clearbut.Size = New System.Drawing.Size(160, 23)
        Me.Clearbut.TabIndex = 2
        Me.Clearbut.Text = "Clear"
        Me.Clearbut.UseVisualStyleBackColor = True
        '
        'ClearBox
        '
        Me.ClearBox.AutoSize = True
        Me.ClearBox.Checked = True
        Me.ClearBox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ClearBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.ClearBox.Location = New System.Drawing.Point(3, 3)
        Me.ClearBox.Name = "ClearBox"
        Me.ClearBox.Size = New System.Drawing.Size(72, 17)
        Me.ClearBox.TabIndex = 4
        Me.ClearBox.Text = "Auto Clear"
        Me.ClearBox.UseVisualStyleBackColor = True
        '
        'ConsoleBox
        '
        Me.ConsoleBox.BackColor = System.Drawing.Color.White
        Me.ConsoleBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.ConsoleBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ConsoleBox.Font = New System.Drawing.Font("Consolas", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ConsoleBox.ForeColor = System.Drawing.Color.Black
        Me.ConsoleBox.Location = New System.Drawing.Point(3, 3)
        Me.ConsoleBox.Name = "ConsoleBox"
        Me.ConsoleBox.ReadOnly = True
        Me.ConsoleBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth
        Me.ConsoleBox.Size = New System.Drawing.Size(682, 249)
        Me.ConsoleBox.TabIndex = 1
        Me.ConsoleBox.TabStop = False
        Me.ConsoleBox.Text = "12314324234"
        Me.ConsoleBox.WordWrap = False
        '
        'ScriptOutput
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(688, 294)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "ScriptOutput"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.Text = "Process Output"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.TableLayoutPanel2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents TableLayoutPanel2 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents Clearbut As System.Windows.Forms.Button
    Friend WithEvents ConsoleBox As System.Windows.Forms.RichTextBox
    Friend WithEvents Savebut As System.Windows.Forms.Button
    Friend WithEvents CopyBut As System.Windows.Forms.Button
    Friend WithEvents ClearBox As System.Windows.Forms.CheckBox
End Class
