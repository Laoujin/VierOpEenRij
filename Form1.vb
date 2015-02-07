Imports System.Threading
Imports System.Net.Sockets
Imports System.Net
Imports System.IO

Public Class frmGame
    Inherits System.Windows.Forms.Form

    Delegate Sub SetTextCallback(ByVal [text] As String)


    Private Enum PStatus
        INACTIVE
        SERVER
        CLIENT
    End Enum

    Private Structure Speler
        Dim Id As Integer
        Dim Naam As String
        Dim Win As Integer
        Dim Loss As Integer
        Dim Draw As Integer
    End Structure

    Private Const RIJEN As Short = 6
    Private Const KOLOMMEN As Short = 7

    Private Const GAME_PORT As Integer = 9001
    Private Const GAME_END As String = "#STOP"
    Private Const GAME_START As String = "#START"
    Private Const GAME_REMISE As String = "#DRAW"
    Private Const CONNECTION_CLOSE As String = "#CLOSE"

    Private picSpel As PictureBox(,)
    Private picSpelDrop As PictureBox()
    Private Spel As New VierOpEenRij(RIJEN, KOLOMMEN)

    Private PlayerName As String
    Private PlayerStatus As PStatus
    Private PlayerNumber As Integer

    Private sThread As Thread
    Private sSocket As Socket
    Private sStream As NetworkStream

    Private uWriter As BinaryWriter
    Friend WithEvents MenuItem2 As System.Windows.Forms.MenuItem
    Private uReader As BinaryReader

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call
        Control.CheckForIllegalCrossThreadCalls = False
    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents pic1 As System.Windows.Forms.PictureBox
    Friend WithEvents MainMenu1 As System.Windows.Forms.MainMenu
    Friend WithEvents MenuItem1 As System.Windows.Forms.MenuItem
    Friend WithEvents MenuItem3 As System.Windows.Forms.MenuItem
    Friend WithEvents picSchijfBlauw As System.Windows.Forms.PictureBox
    Friend WithEvents picSchijfRood As System.Windows.Forms.PictureBox
    Friend WithEvents imgDrop As System.Windows.Forms.PictureBox
    Friend WithEvents pcbNot As System.Windows.Forms.PictureBox
    Friend WithEvents txtIP As System.Windows.Forms.TextBox
    Friend WithEvents txtNaam As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents cmdConnecteer As System.Windows.Forms.Button
    Friend WithEvents lblInvisible As System.Windows.Forms.TextBox
    Friend WithEvents pnlChat As System.Windows.Forms.GroupBox
    Friend WithEvents cmdSend As System.Windows.Forms.Button
    Friend WithEvents txtSend As System.Windows.Forms.TextBox
    Friend WithEvents txtDisplay As System.Windows.Forms.TextBox
    Friend WithEvents mnuAfsluiten As System.Windows.Forms.MenuItem
    Friend WithEvents mnuMultiplayer As System.Windows.Forms.MenuItem
    Friend WithEvents MenuItem6 As System.Windows.Forms.MenuItem
    Friend WithEvents pnlServer As System.Windows.Forms.GroupBox
    Friend WithEvents pnlConnecteer As System.Windows.Forms.GroupBox
    Friend WithEvents pnlInfo As System.Windows.Forms.GroupBox
    Friend WithEvents lblInfo As System.Windows.Forms.Label
    Friend WithEvents mnuStartServer As System.Windows.Forms.MenuItem
    Friend WithEvents mnuConnecteer As System.Windows.Forms.MenuItem
    Friend WithEvents mnuDisconnecteer As System.Windows.Forms.MenuItem
    Friend WithEvents txtServer As System.Windows.Forms.TextBox
    Friend WithEvents cmdStart As System.Windows.Forms.Button
    Friend WithEvents pnlGame As System.Windows.Forms.GroupBox
    Friend WithEvents pnlGameStatus As System.Windows.Forms.GroupBox
    Friend WithEvents txtGame As System.Windows.Forms.TextBox
    Friend WithEvents lblWarning As System.Windows.Forms.Label
    Friend WithEvents TimerWarningMsg As System.Windows.Forms.Timer
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmGame))
        Me.pic1 = New System.Windows.Forms.PictureBox
        Me.MainMenu1 = New System.Windows.Forms.MainMenu(Me.components)
        Me.MenuItem1 = New System.Windows.Forms.MenuItem
        Me.mnuMultiplayer = New System.Windows.Forms.MenuItem
        Me.mnuStartServer = New System.Windows.Forms.MenuItem
        Me.mnuConnecteer = New System.Windows.Forms.MenuItem
        Me.MenuItem6 = New System.Windows.Forms.MenuItem
        Me.mnuDisconnecteer = New System.Windows.Forms.MenuItem
        Me.MenuItem3 = New System.Windows.Forms.MenuItem
        Me.mnuAfsluiten = New System.Windows.Forms.MenuItem
        Me.picSchijfBlauw = New System.Windows.Forms.PictureBox
        Me.picSchijfRood = New System.Windows.Forms.PictureBox
        Me.imgDrop = New System.Windows.Forms.PictureBox
        Me.pcbNot = New System.Windows.Forms.PictureBox
        Me.pnlConnecteer = New System.Windows.Forms.GroupBox
        Me.cmdConnecteer = New System.Windows.Forms.Button
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.txtNaam = New System.Windows.Forms.TextBox
        Me.txtIP = New System.Windows.Forms.TextBox
        Me.lblInvisible = New System.Windows.Forms.TextBox
        Me.pnlChat = New System.Windows.Forms.GroupBox
        Me.cmdSend = New System.Windows.Forms.Button
        Me.txtSend = New System.Windows.Forms.TextBox
        Me.txtDisplay = New System.Windows.Forms.TextBox
        Me.pnlServer = New System.Windows.Forms.GroupBox
        Me.txtServer = New System.Windows.Forms.TextBox
        Me.pnlInfo = New System.Windows.Forms.GroupBox
        Me.lblInfo = New System.Windows.Forms.Label
        Me.pnlGame = New System.Windows.Forms.GroupBox
        Me.cmdStart = New System.Windows.Forms.Button
        Me.pnlGameStatus = New System.Windows.Forms.GroupBox
        Me.txtGame = New System.Windows.Forms.TextBox
        Me.lblWarning = New System.Windows.Forms.Label
        Me.TimerWarningMsg = New System.Windows.Forms.Timer(Me.components)
        Me.MenuItem2 = New System.Windows.Forms.MenuItem
        CType(Me.pic1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picSchijfBlauw, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picSchijfRood, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.imgDrop, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pcbNot, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlConnecteer.SuspendLayout()
        Me.pnlChat.SuspendLayout()
        Me.pnlServer.SuspendLayout()
        Me.pnlInfo.SuspendLayout()
        Me.pnlGame.SuspendLayout()
        Me.pnlGameStatus.SuspendLayout()
        Me.SuspendLayout()
        '
        'pic1
        '
        Me.pic1.Image = CType(resources.GetObject("pic1.Image"), System.Drawing.Image)
        Me.pic1.Location = New System.Drawing.Point(8, 0)
        Me.pic1.Name = "pic1"
        Me.pic1.Size = New System.Drawing.Size(75, 75)
        Me.pic1.TabIndex = 0
        Me.pic1.TabStop = False
        Me.pic1.Visible = False
        '
        'MainMenu1
        '
        Me.MainMenu1.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.MenuItem1})
        '
        'MenuItem1
        '
        Me.MenuItem1.Index = 0
        Me.MenuItem1.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuMultiplayer, Me.MenuItem3, Me.mnuAfsluiten})
        Me.MenuItem1.Text = "Bestand"
        '
        'mnuMultiplayer
        '
        Me.mnuMultiplayer.Index = 0
        Me.mnuMultiplayer.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.MenuItem2, Me.mnuStartServer, Me.mnuConnecteer, Me.MenuItem6, Me.mnuDisconnecteer})
        Me.mnuMultiplayer.Text = "&Multiplayer"
        '
        'mnuStartServer
        '
        Me.mnuStartServer.Index = 0
        Me.mnuStartServer.Shortcut = System.Windows.Forms.Shortcut.CtrlS
        Me.mnuStartServer.Text = "Start Server"
        '
        'mnuConnecteer
        '
        Me.mnuConnecteer.Index = 1
        Me.mnuConnecteer.Shortcut = System.Windows.Forms.Shortcut.CtrlC
        Me.mnuConnecteer.Text = "Connecteer"
        '
        'MenuItem6
        '
        Me.MenuItem6.Index = 2
        Me.MenuItem6.Text = "-"
        '
        'mnuDisconnecteer
        '
        Me.mnuDisconnecteer.Enabled = False
        Me.mnuDisconnecteer.Index = 3
        Me.mnuDisconnecteer.Shortcut = System.Windows.Forms.Shortcut.CtrlD
        Me.mnuDisconnecteer.Text = "Disconnecteer"
        '
        'MenuItem3
        '
        Me.MenuItem3.Index = 1
        Me.MenuItem3.Text = "-"
        '
        'mnuAfsluiten
        '
        Me.mnuAfsluiten.Index = 2
        Me.mnuAfsluiten.Shortcut = System.Windows.Forms.Shortcut.CtrlA
        Me.mnuAfsluiten.Text = "&Afsluiten"
        '
        'picSchijfBlauw
        '
        Me.picSchijfBlauw.BackColor = System.Drawing.SystemColors.Control
        Me.picSchijfBlauw.Image = CType(resources.GetObject("picSchijfBlauw.Image"), System.Drawing.Image)
        Me.picSchijfBlauw.Location = New System.Drawing.Point(272, 0)
        Me.picSchijfBlauw.Name = "picSchijfBlauw"
        Me.picSchijfBlauw.Size = New System.Drawing.Size(75, 75)
        Me.picSchijfBlauw.TabIndex = 2
        Me.picSchijfBlauw.TabStop = False
        Me.picSchijfBlauw.Visible = False
        '
        'picSchijfRood
        '
        Me.picSchijfRood.BackColor = System.Drawing.SystemColors.Control
        Me.picSchijfRood.Image = CType(resources.GetObject("picSchijfRood.Image"), System.Drawing.Image)
        Me.picSchijfRood.Location = New System.Drawing.Point(360, 0)
        Me.picSchijfRood.Name = "picSchijfRood"
        Me.picSchijfRood.Size = New System.Drawing.Size(75, 75)
        Me.picSchijfRood.TabIndex = 3
        Me.picSchijfRood.TabStop = False
        Me.picSchijfRood.Visible = False
        '
        'imgDrop
        '
        Me.imgDrop.Image = CType(resources.GetObject("imgDrop.Image"), System.Drawing.Image)
        Me.imgDrop.Location = New System.Drawing.Point(96, 0)
        Me.imgDrop.Name = "imgDrop"
        Me.imgDrop.Size = New System.Drawing.Size(75, 75)
        Me.imgDrop.TabIndex = 10
        Me.imgDrop.TabStop = False
        Me.imgDrop.Visible = False
        '
        'pcbNot
        '
        Me.pcbNot.Image = CType(resources.GetObject("pcbNot.Image"), System.Drawing.Image)
        Me.pcbNot.Location = New System.Drawing.Point(59, 81)
        Me.pcbNot.Name = "pcbNot"
        Me.pcbNot.Size = New System.Drawing.Size(75, 75)
        Me.pcbNot.TabIndex = 11
        Me.pcbNot.TabStop = False
        Me.pcbNot.Visible = False
        '
        'pnlConnecteer
        '
        Me.pnlConnecteer.Controls.Add(Me.cmdConnecteer)
        Me.pnlConnecteer.Controls.Add(Me.Label2)
        Me.pnlConnecteer.Controls.Add(Me.Label1)
        Me.pnlConnecteer.Controls.Add(Me.txtNaam)
        Me.pnlConnecteer.Controls.Add(Me.txtIP)
        Me.pnlConnecteer.Location = New System.Drawing.Point(1070, 12)
        Me.pnlConnecteer.Name = "pnlConnecteer"
        Me.pnlConnecteer.Size = New System.Drawing.Size(520, 120)
        Me.pnlConnecteer.TabIndex = 12
        Me.pnlConnecteer.TabStop = False
        Me.pnlConnecteer.Text = "Connecteer"
        '
        'cmdConnecteer
        '
        Me.cmdConnecteer.Location = New System.Drawing.Point(144, 88)
        Me.cmdConnecteer.Name = "cmdConnecteer"
        Me.cmdConnecteer.Size = New System.Drawing.Size(184, 24)
        Me.cmdConnecteer.TabIndex = 2
        Me.cmdConnecteer.Text = "&Connecteer"
        '
        'Label2
        '
        Me.Label2.Location = New System.Drawing.Point(64, 56)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(136, 24)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "IP Host"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(64, 24)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(136, 16)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Naam"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'txtNaam
        '
        Me.txtNaam.Location = New System.Drawing.Point(208, 24)
        Me.txtNaam.MaxLength = 15
        Me.txtNaam.Name = "txtNaam"
        Me.txtNaam.Size = New System.Drawing.Size(232, 23)
        Me.txtNaam.TabIndex = 0
        Me.txtNaam.Text = "client"
        '
        'txtIP
        '
        Me.txtIP.Location = New System.Drawing.Point(208, 56)
        Me.txtIP.Name = "txtIP"
        Me.txtIP.Size = New System.Drawing.Size(232, 23)
        Me.txtIP.TabIndex = 1
        Me.txtIP.Text = "localhost"
        '
        'lblInvisible
        '
        Me.lblInvisible.Location = New System.Drawing.Point(408, 8)
        Me.lblInvisible.Name = "lblInvisible"
        Me.lblInvisible.Size = New System.Drawing.Size(104, 23)
        Me.lblInvisible.TabIndex = 16
        Me.lblInvisible.Text = "TextBox1"
        Me.lblInvisible.Visible = False
        '
        'pnlChat
        '
        Me.pnlChat.Controls.Add(Me.cmdSend)
        Me.pnlChat.Controls.Add(Me.txtSend)
        Me.pnlChat.Controls.Add(Me.txtDisplay)
        Me.pnlChat.Location = New System.Drawing.Point(544, 136)
        Me.pnlChat.Name = "pnlChat"
        Me.pnlChat.Size = New System.Drawing.Size(520, 224)
        Me.pnlChat.TabIndex = 17
        Me.pnlChat.TabStop = False
        Me.pnlChat.Text = "Chat Window"
        '
        'cmdSend
        '
        Me.cmdSend.Location = New System.Drawing.Point(432, 192)
        Me.cmdSend.Name = "cmdSend"
        Me.cmdSend.Size = New System.Drawing.Size(72, 24)
        Me.cmdSend.TabIndex = 12
        Me.cmdSend.Text = "Send"
        '
        'txtSend
        '
        Me.txtSend.Location = New System.Drawing.Point(8, 192)
        Me.txtSend.Name = "txtSend"
        Me.txtSend.Size = New System.Drawing.Size(416, 23)
        Me.txtSend.TabIndex = 11
        '
        'txtDisplay
        '
        Me.txtDisplay.Location = New System.Drawing.Point(8, 18)
        Me.txtDisplay.Multiline = True
        Me.txtDisplay.Name = "txtDisplay"
        Me.txtDisplay.ReadOnly = True
        Me.txtDisplay.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtDisplay.Size = New System.Drawing.Size(496, 168)
        Me.txtDisplay.TabIndex = 13
        Me.txtDisplay.TabStop = False
        '
        'pnlServer
        '
        Me.pnlServer.Controls.Add(Me.txtServer)
        Me.pnlServer.Location = New System.Drawing.Point(544, 360)
        Me.pnlServer.Name = "pnlServer"
        Me.pnlServer.Size = New System.Drawing.Size(520, 176)
        Me.pnlServer.TabIndex = 18
        Me.pnlServer.TabStop = False
        Me.pnlServer.Text = "Server Status"
        '
        'txtServer
        '
        Me.txtServer.Location = New System.Drawing.Point(8, 26)
        Me.txtServer.Multiline = True
        Me.txtServer.Name = "txtServer"
        Me.txtServer.ReadOnly = True
        Me.txtServer.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtServer.Size = New System.Drawing.Size(496, 144)
        Me.txtServer.TabIndex = 13
        Me.txtServer.TabStop = False
        '
        'pnlInfo
        '
        Me.pnlInfo.Controls.Add(Me.lblInfo)
        Me.pnlInfo.Location = New System.Drawing.Point(544, 12)
        Me.pnlInfo.Name = "pnlInfo"
        Me.pnlInfo.Size = New System.Drawing.Size(520, 120)
        Me.pnlInfo.TabIndex = 19
        Me.pnlInfo.TabStop = False
        Me.pnlInfo.Text = "Info"
        '
        'lblInfo
        '
        Me.lblInfo.Location = New System.Drawing.Point(16, 24)
        Me.lblInfo.Name = "lblInfo"
        Me.lblInfo.Size = New System.Drawing.Size(488, 88)
        Me.lblInfo.TabIndex = 0
        Me.lblInfo.Text = "Start een server of connecteer via IP met een geactiveerde server om te spelen."
        '
        'pnlGame
        '
        Me.pnlGame.Controls.Add(Me.cmdStart)
        Me.pnlGame.Location = New System.Drawing.Point(1070, 136)
        Me.pnlGame.Name = "pnlGame"
        Me.pnlGame.Size = New System.Drawing.Size(520, 120)
        Me.pnlGame.TabIndex = 20
        Me.pnlGame.TabStop = False
        Me.pnlGame.Text = "Spel Opties"
        Me.pnlGame.Visible = False
        '
        'cmdStart
        '
        Me.cmdStart.Location = New System.Drawing.Point(16, 32)
        Me.cmdStart.Name = "cmdStart"
        Me.cmdStart.Size = New System.Drawing.Size(144, 32)
        Me.cmdStart.TabIndex = 0
        Me.cmdStart.Text = "&Start"
        '
        'pnlGameStatus
        '
        Me.pnlGameStatus.Controls.Add(Me.txtGame)
        Me.pnlGameStatus.Location = New System.Drawing.Point(544, 360)
        Me.pnlGameStatus.Name = "pnlGameStatus"
        Me.pnlGameStatus.Size = New System.Drawing.Size(520, 176)
        Me.pnlGameStatus.TabIndex = 21
        Me.pnlGameStatus.TabStop = False
        Me.pnlGameStatus.Text = "Game Status"
        Me.pnlGameStatus.Visible = False
        '
        'txtGame
        '
        Me.txtGame.Location = New System.Drawing.Point(12, 24)
        Me.txtGame.Multiline = True
        Me.txtGame.Name = "txtGame"
        Me.txtGame.ReadOnly = True
        Me.txtGame.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtGame.Size = New System.Drawing.Size(496, 144)
        Me.txtGame.TabIndex = 15
        Me.txtGame.TabStop = False
        Me.txtGame.Visible = False
        '
        'lblWarning
        '
        Me.lblWarning.Font = New System.Drawing.Font("Verdana", 21.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblWarning.ForeColor = System.Drawing.Color.Red
        Me.lblWarning.Location = New System.Drawing.Point(0, 104)
        Me.lblWarning.Name = "lblWarning"
        Me.lblWarning.Size = New System.Drawing.Size(544, 64)
        Me.lblWarning.TabIndex = 22
        Me.lblWarning.Text = "WarningMessage"
        Me.lblWarning.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.lblWarning.Visible = False
        '
        'TimerWarningMsg
        '
        Me.TimerWarningMsg.Interval = 5000
        '
        'MenuItem2
        '
        Me.MenuItem2.Index = 0
        Me.MenuItem2.Text = "Start spel"
        '
        'frmGame
        '
        Me.AcceptButton = Me.cmdSend
        Me.AutoScaleBaseSize = New System.Drawing.Size(7, 16)
        Me.ClientSize = New System.Drawing.Size(1530, 547)
        Me.Controls.Add(Me.lblWarning)
        Me.Controls.Add(Me.pnlConnecteer)
        Me.Controls.Add(Me.pnlServer)
        Me.Controls.Add(Me.pnlChat)
        Me.Controls.Add(Me.lblInvisible)
        Me.Controls.Add(Me.pnlGame)
        Me.Controls.Add(Me.pcbNot)
        Me.Controls.Add(Me.imgDrop)
        Me.Controls.Add(Me.picSchijfRood)
        Me.Controls.Add(Me.picSchijfBlauw)
        Me.Controls.Add(Me.pic1)
        Me.Controls.Add(Me.pnlInfo)
        Me.Controls.Add(Me.pnlGameStatus)
        Me.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.Menu = Me.MainMenu1
        Me.Name = "frmGame"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Vier op een rij"
        CType(Me.pic1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picSchijfBlauw, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picSchijfRood, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.imgDrop, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pcbNot, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlConnecteer.ResumeLayout(False)
        Me.pnlConnecteer.PerformLayout()
        Me.pnlChat.ResumeLayout(False)
        Me.pnlChat.PerformLayout()
        Me.pnlServer.ResumeLayout(False)
        Me.pnlServer.PerformLayout()
        Me.pnlInfo.ResumeLayout(False)
        Me.pnlGame.ResumeLayout(False)
        Me.pnlGameStatus.ResumeLayout(False)
        Me.pnlGameStatus.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

    Private Sub frmGame_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim picNieuw As New PictureBox
        Dim picDrop As New PictureBox
        Dim i, j As Integer

        picSpel = New PictureBox(RIJEN, KOLOMMEN) {}
        picSpelDrop = New PictureBox(KOLOMMEN) {}
        For i = 0 To KOLOMMEN - 1
            ' picDrop zijn de pictureboxen die gebruikt worden voor de mouseover effecten
            picDrop = New PictureBox
            Me.Controls.Add(picDrop)
            picDrop.Name = "kol" & i
            picDrop.Size = New System.Drawing.Size(75, 75)
            picDrop.Location = New System.Drawing.Point(10 + i * picDrop.Width, 10)
            picDrop.BackgroundImage = imgDrop.Image
            picDrop.Visible = False
            picSpelDrop(i) = picDrop

            ' Toekennen van events voor Click, MouseLeave en MouseEnter
            AddHandler picDrop.Click, AddressOf imgDrop_Click
            AddHandler picDrop.MouseLeave, AddressOf imgDrop_MouseLeave
            AddHandler picDrop.MouseEnter, AddressOf imgDrop_MouseEnter

            ' Toevoegen van het eigenlijk speelveld
            For j = 0 To RIJEN - 1
                picNieuw = New PictureBox
                Me.Controls.Add(picNieuw)
                picNieuw.Size = New System.Drawing.Size(75, 75)
                picNieuw.Location = New System.Drawing.Point(10 + i * picNieuw.Height, 85 + j * picNieuw.Height)
                picNieuw.BackgroundImage = pic1.Image
                picNieuw.Visible = False
                picSpel(j, i) = picNieuw
            Next
        Next

        ' Geen verbinding met een server
        PlayerStatus = PStatus.INACTIVE
        Spel.NieuwSpel(picSpel, picSpelDrop)
    End Sub

    Private Sub mnuAfsluiten_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuAfsluiten.Click
        Application.Exit()
    End Sub

    Private Sub imgDrop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles imgDrop.Click
        If PlayerStatus = PStatus.INACTIVE Then

            ' Je kan enkel spelen als je aan de beurt bent
        ElseIf Spel.AanZet = PlayerNumber Then
            ' De naam van de form waarop geklikt werd
            Dim imgClicked As String = CType(sender, System.Windows.Forms.PictureBox).Name
            Dim bds As String = ""
            Dim i, zet As Integer

            For i = 0 To KOLOMMEN - 1
                If imgClicked = "kol" & i Then
                    Dim SchijfKleur As PictureBox
                    SchijfKleur = Spel.AssignKleur(picSchijfBlauw, picSchijfRood)
                    zet = Spel.Speelzet(i, picSpel, SchijfKleur, bds)   ' Zet een schijf op kolom i voor speler SchijfKleur
                    sender.image = Nothing

                    If zet >= 0 Or zet = -3 Then
                        uWriter.Write("#" & i)  ' Sturen van de zet naar de tegenstander
                    End If

                    If Not bds = "" Then
                        uWriter.Write(bds)  ' Sturen van "belangrijke" boodschappen (zoals "einde spel", etc)
                        AddText(bds, "txtDisplay")
                    End If
                End If
            Next
        End If
    End Sub

    Private Sub imgDrop_MouseLeave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles imgDrop.MouseLeave
        ' De schijf-foto wegdoen
        sender.image = Nothing
    End Sub

    Private Sub imgDrop_MouseEnter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles imgDrop.MouseEnter
        Dim SchijfKleur As PictureBox
        Dim bds As String = ""

        If Spel.Gewonnen = False And Spel.GeldigeZetBool(CInt(CStr(sender.name).Substring(3)), bds) = True And Spel.AanZet = PlayerNumber Then
            ' Teken schijf (in juiste kleur)
            ' Enkel wanneer het spel niet over is, de huidige zet geldig is en de huidige speler aan de beurt is
            SchijfKleur = Spel.AssignKleur(picSchijfBlauw, picSchijfRood)
            sender.image = SchijfKleur.Image
        Else
            ' Teken kruis
            sender.image = pcbNot.Image
        End If
    End Sub

    Private Sub mnuStartServer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuStartServer.Click
        ' Start Server
        If PlayerStatus = PStatus.INACTIVE Then
            PlayerName = InputBox("Geef je naam:", "Vier op een rij")
            If PlayerName = "" Then PlayerName = "Speler"
            pnlInfo.Visible = False

            SwitchStatus()

            ' Start thread op de Sub StartServer
            sThread = New Thread(AddressOf StartServer)
            sThread.Start()
        Else
            MsgBox("Disconnecteer eerst!")
        End If
    End Sub

    Private Sub mnuConnecteer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuConnecteer.Click
        ' Connecteer
        If PlayerStatus = PStatus.INACTIVE Then
            pnlConnecteer.Visible = True
            pnlInfo.Visible = False
            SwitchStatus()
        Else
            MsgBox("Disconnecteer eerst!")
        End If
    End Sub

    Private Sub mnuDisconnecteer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuDisconnecteer.Click
        If PlayerStatus = PStatus.INACTIVE Then
            MsgBox("Er is geen actieve verbinding")
        Else
            PlayerStatus = PStatus.INACTIVE
        End If
    End Sub

    Private Sub SwitchStatus()
        mnuConnecteer.Enabled = Not mnuConnecteer.Enabled
        mnuDisconnecteer.Enabled = Not mnuDisconnecteer.Enabled
        mnuStartServer.Enabled = Not mnuStartServer.Enabled
    End Sub

    Private Sub cmdConnecteer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdConnecteer.Click
        If Len(txtNaam.Text) = 0 Then PlayerName = "Speler" Else PlayerName = txtNaam.Text

        sThread = New Thread(AddressOf StartClient)
        sThread.Start()
    End Sub

    Private Sub cmdSend_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSend.Click
        ' Versturen van data over het netwerk
        ' /nick naam: veranderen naam (zie hier juist dat enkel de "server" zijn naam kan veranderen)
        ' /start: starten van een nieuw spel
        ' Alle andere text wordt gewoon doorgestuurd.
        ' Zetten kunnen niet "gefaked" worden doordat er "speler> " aan alle boodschappen wordt voorafgegeaan
        If PlayerStatus = PStatus.INACTIVE Then
            AddText("Er is geen actieve verbinding", "txtDisplay")
        Else
            Dim cM As String = txtSend.Text
            Try
                If (Not txtSend.Text = "") Then
                    If cM.Substring(0, 1) = "/" Then
                        Dim Cmd, Param As String

                        If cM.IndexOf(" ") > 0 Then
                            Cmd = cM.Substring(0, cM.IndexOf(" "))
                            Param = cM.Substring(cM.IndexOf(" ") + 1)
                        Else
                            Cmd = cM
                            Param = ""
                        End If

                        Select Case Cmd
                            Case "/nick"
                                uWriter.Write(PlayerName & " has changed names to " & Param)
                                PlayerName = Param
                        End Select
                    Else
                        uWriter.Write(PlayerName & "> " & cM)
                        AddText(PlayerName & "> " & cM, "txtDisplay")
                    End If
                End If
            Catch exception As SocketException
                AddText("Error sending text!", "txtDisplay")
            End Try
        End If
        txtSend.Clear()
    End Sub

    Private Sub txtNaam_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtNaam.GotFocus
        txtNaam.SelectAll()
    End Sub

    Private Sub txtIP_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtIP.GotFocus
        txtIP.SelectAll()
    End Sub

    Private Sub SetTextGame(ByVal [text] As String)
        ' InvokeRequired required compares the thread ID of the
        ' calling thread to the thread ID of the creating thread.
        ' If these threads are different, it returns true.
        If Me.txtDisplay.InvokeRequired Then
            Dim d As New SetTextCallback(AddressOf SetTextGame)
            Me.Invoke(d, New Object() {[text]})
        Else
            Me.txtGame.Text &= [text]
        End If
    End Sub

    Private Sub SetTextDisplay(ByVal [text] As String)
        ' InvokeRequired required compares the thread ID of the
        ' calling thread to the thread ID of the creating thread.
        ' If these threads are different, it returns true.
        If Me.txtDisplay.InvokeRequired Then
            Dim d As New SetTextCallback(AddressOf SetTextDisplay)
            Me.Invoke(d, New Object() {[text]})
        Else
            Me.txtDisplay.Text &= [text]
        End If
    End Sub

    Private Sub SetTextServer(ByVal [text] As String)
        ' InvokeRequired required compares the thread ID of the
        ' calling thread to the thread ID of the creating thread.
        ' If these threads are different, it returns true.
        If Me.txtDisplay.InvokeRequired Then
            Dim d As New SetTextCallback(AddressOf SetTextServer)
            Me.Invoke(d, New Object() {[text]})
        Else
            Me.txtServer.Text &= [text]
        End If
    End Sub

    Private Sub StartServer()
        ' De server code: wordt beheerd door een aparte thread (net als de client sub)

        Dim sListener As TcpListener
        Try
            sListener = New TcpListener(IPAddress.Parse("127.0.0.1"), GAME_PORT)
            sListener.Start()

            PlayerStatus = PStatus.SERVER
            While PlayerStatus = PStatus.SERVER
                SetTextGame("")
                SetTextServer("")
                AddText("Waiting for connection...", "txtServer")
                sSocket = sListener.AcceptSocket()

                ' Eenmaal de verbinding tot stand is, stream en bwriter/reader setten
                sStream = New NetworkStream(sSocket)
                uWriter = New BinaryWriter(sStream)
                uReader = New BinaryReader(sStream)

                AddText("Welkom op de fantastische vier op een rij! :)", "txtDisplay")
                AddText("Connection Established", "txtServer")
                uWriter.Write("$Connection Established")
                uWriter.Write("$" & PlayerName & " joined the game")

                'pnlGame.Visible = True
                Dim cMessage As String = ""
                Try
                    Do
                        ' Text beginnend met een # is een zet van de tegenstander
                        ' of een van de voorgedifinieerde keywords (zoals #STOP of #START)
                        cMessage = uReader.ReadString()
                        If cMessage.Substring(0, 1) = "#" Then
                            Select Case cMessage
                                Case GAME_END
                                Case GAME_START
                                Case GAME_REMISE
                                Case Else
                                    OpponentMove(cMessage.Substring(1))
                            End Select
                        ElseIf cMessage.Substring(0, 1) = "$" Then
                            AddText(cMessage.Substring(1), "txtServer")
                        Else
                            ' Anders gewoon toevoegen aan de displaybox
                            AddText(cMessage, "txtDisplay")
                        End If
                    Loop While (cMessage <> CONNECTION_CLOSE AndAlso sSocket.Connected AndAlso Not PlayerStatus = PStatus.INACTIVE)
                Catch inputOutputException As IOException
                    MessageBox.Show("Client application closing (Server)" & vbCrLf & inputOutputException.Message)
                Finally
                    AddText("User terminated connection", "txtServer")

                    uWriter.Close()
                    uReader.Close()
                    sStream.Close()
                    sSocket.Close()
                End Try
            End While
        Catch inputOutputException As IOException
            MessageBox.Show("Server application closing (Server)" & vbCrLf & inputOutputException.Message)
        Finally
            uWriter.Close()
            uReader.Close()
            sStream.Close()
            sSocket.Close()
        End Try
    End Sub

    Private Sub StartClient()
        ' Code voor de client

        Dim sClient As TcpClient
        Try
            SetTextGame("")
            SetTextServer("")
            AddText("Attempting connection...", "txtServer")

            ' Gaat hier automatish met de localhost verbinden
            ' s = server, u = user
            sClient = New TcpClient
            sClient.Connect(txtIP.Text, GAME_PORT)
            sStream = sClient.GetStream()
            uWriter = New BinaryWriter(sStream)
            uReader = New BinaryReader(sStream)

            AddText("Welkom op de fantastische vier op een rij! :)", "txtDisplay")
            uWriter.Write("$" & PlayerName & " joined the game")

            ' Status van het spel: enum gedifinieert bovenaan
            PlayerStatus = PStatus.CLIENT

            'pnlConnecteer.Visible = False
            'pnlGame.Visible = True
            Dim cMessage As String = ""
            Try
                Do
                    cMessage = uReader.ReadString
                    If cMessage.Substring(0, 1) = "#" Then
                        Select Case cMessage
                            Case GAME_START ' Bij het toekomen van #start wordt een nieuw spel gestart
                                Spel.NieuwSpel(picSpel, picSpelDrop)
                                PlayerNumber = 2
                                AddText("Nieuw spel gestart!", "txtServer")
                            Case GAME_END '= give up
                            Case GAME_REMISE
                            Case Else
                                ' Anders is het een zet van de tegenstander
                                OpponentMove(cMessage.Substring(1))
                        End Select
                    ElseIf cMessage.Substring(0, 1) = "$" Then
                        AddText(cMessage.Substring(1), "txtServer")
                    Else
                        ' En nog anders gewoon tekst door de tegenstander gestuurd
                        AddText(cMessage, "txtDisplay")
                    End If
                Loop While cMessage <> CONNECTION_CLOSE AndAlso Not PlayerStatus = PStatus.INACTIVE
            Catch inputOutputException As IOException
                MessageBox.Show("Client application closing (Client)" & vbCrLf & inputOutputException.Message)
            Finally
                AddText("Closing connection...", "txtServer")

                uWriter.Close()
                uReader.Close()
                sStream.Close()
                sClient.Close()
            End Try
        Catch inputOutputException As Exception
            MessageBox.Show("Client application closing 2 (Client)" & vbCrLf & inputOutputException.Message)
        Finally
            uWriter.Close()
            uReader.Close()
            sStream.Close()
            'If Not sClient Is Nothing Then sClient.Close()
        End Try
    End Sub

    Public Sub AddText(ByVal t As String, ByRef txt As String)
        ' Toevoegen van tekst aan txtDisplay + naar onder scrollen zodat de laatste entries zichtbaar zijn
        Dim lTxt As String = vbCrLf & t

        If txt = "txtServer" Then
            SetTextServer(lTxt)
        ElseIf txt = "txtGame" Then
            SetTextGame(lTxt)
        Else

            SetTextDisplay(lTxt)
        End If

        'txt.SelectionStart = txt.Text.Length
        'txt.ScrollToCaret()
    End Sub

    Private Sub OpponentMove(ByVal Kolom As Integer)
        ' Zet van de tegenstander registreren

        Dim SchijfKleur As PictureBox
        Dim bds As String = ""

        SchijfKleur = Spel.AssignKleur(picSchijfBlauw, picSchijfRood)
        Spel.Speelzet(Kolom, picSpel, SchijfKleur, bds)
    End Sub

    Private Sub cmdStart_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdStart.Click
        Dim SpelerStart As Random = New Random

        If PlayerStatus = PStatus.SERVER Then
            pnlServer.Visible = False
            pnlGameStatus.Visible = True

            Spel.NieuwSpel(picSpel, picSpelDrop)
            PlayerNumber = 1 'SpelerStart.Next(1, 2)
            uWriter.Write(String.Concat(GAME_START, PlayerNumber.ToString()))
        End If
    End Sub

    Private Sub TimerWarningMsg_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TimerWarningMsg.Tick
        lblWarning.Visible = False
    End Sub

    Private Sub MenuItem2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MenuItem2.Click
        Dim SpelerStart As Random = New Random

        If PlayerStatus = PStatus.SERVER Then
            pnlServer.Visible = False
            pnlGameStatus.Visible = True

            Spel.NieuwSpel(picSpel, picSpelDrop)
            PlayerNumber = 1 'SpelerStart.Next(1, 2)
            Dim lStr As String = GAME_START & PlayerNumber.ToString()
            uWriter.Write(lStr) 'lStr
        End If
    End Sub
End Class
