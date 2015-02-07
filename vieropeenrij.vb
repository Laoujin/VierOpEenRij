
Public Class VierOpEenRij
    Private Const SPELER_1 As Short = 1
    Private Const SPELER_2 As Short = 2

    Private Const GEWONNEN_AANTAL As Short = 4
    Private Const GEWONNEN_FALSE As Boolean = False
    Private Const GEWONNEN_TRUE As Boolean = True

    Private Const BORD_OPEN As Short = 0
    Private Const BORD_VOL As Short = 1

    Private Const ZET_GELDIG As Short = 0
    Private Const ZET_ONGELDIG As Short = -1
    Private Const ZET_BORDVOL As Short = -2
    Private Const ZET_EINDE As Short = -3

    Private Const TEXT_SPELER1 As String = "Speler: Blauw"
    Private Const TEXT_SPELER2 As String = "Speler: Rood"

    Private mBord As Short(,)   ' Het speelbord RIJEN en KOLOMMEN groot. 0 voor "open" 1-2 voor volle plaatsen
    Private mGewonnen As Boolean
    Private mAanzet As Short    ' 1-2 specifieert welke speler aan de beurt is
    ' Vraag me trouwens niet meer waarvoor de "m" weeral staat

    Private RIJEN As Short
    Private KOLOMMEN As Short

    Public Sub New(ByVal vRij As Short, ByVal vKolom As Short)
        RIJEN = vRij
        KOLOMMEN = vKolom
        mBord = New Short(RIJEN, KOLOMMEN) {}

        ' Initialiseren van het veld
    End Sub

    Public Sub NieuwSpel(ByVal picArray As PictureBox(,), ByVal picSpelDrop As PictureBox())
        Dim i, j As Integer
        For i = 0 To KOLOMMEN - 1
            picSpelDrop(i).Visible = True
            For j = 0 To RIJEN - 1
                mBord(j, i) = BORD_OPEN
                picArray(j, i).Visible = True
                picArray(j, i).Image = Nothing
            Next
        Next
        mAanzet = SPELER_1 'mss randomizen
        mGewonnen = GEWONNEN_FALSE

        ' Het veld zichtbaar maken op de form
        ' mBord "leeg" maken
    End Sub

    Public Function GeldigeZet(ByVal DeKolom As Integer, ByRef lbl As String) As Integer
        ' Controleert of er nog een schijf kan worden toegevoegd bij "DeKolom"
        ' Controleert meteen ook of het bord niet vol staat

        Dim i, j As Integer
        If Gewonnen = GEWONNEN_TRUE Then
            'lbl = "Einde Spel"
            Return ZET_EINDE
        ElseIf mBord(0, DeKolom) = BORD_OPEN Then
            Return ZET_GELDIG
        Else
            For i = 0 To RIJEN - 1
                For j = 0 To KOLOMMEN - 1
                    If mBord(i, j) = BORD_OPEN Then
                        'lbl = "Ongeldige Zet!"
                        Return ZET_ONGELDIG
                    End If
                Next
            Next
            'lbl = "Bord vol"
            mGewonnen = True
            Return ZET_BORDVOL
        End If
    End Function

    Public Function Speelzet(ByVal DeKolom As Integer, ByVal picSpel As PictureBox(,), ByVal imgSpeler As PictureBox, ByRef lbl As String) As Integer
        ' Roept GeldigeZet aan om te controleren of de zet geldig is

        Dim i, Geldig As Integer
        Geldig = GeldigeZet(DeKolom, lbl)
        If Geldig = ZET_GELDIG Then
            For i = RIJEN - 1 To 0 Step -1
                If mBord(i, DeKolom) = BORD_OPEN Then
                    mBord(i, DeKolom) = AanZet  ' Toekenen van het "speler-nummer" op die locatie op het bord
                    picSpel(i, DeKolom).Image = imgSpeler.Image ' Teken de schijf
                    mGewonnen = isGewonnen(i, DeKolom, lbl) ' Controleer of het spel gewonnen is

                    If mGewonnen = True Then
                        lbl = "Vier op een rij!"
                        Return ZET_EINDE
                    Else
                        ' Verandertd de huidige speler
                        If mAanzet = SPELER_1 Then
                            'lbl = TEXT_SPELER2
                            mAanzet = SPELER_2
                        Else
                            'lbl = TEXT_SPELER1
                            mAanzet = SPELER_1
                        End If
                        Return i
                    End If
                End If
            Next
        End If
    End Function

    Private Function isGewonnen(ByVal Rij As Integer, ByVal Kolom As Integer, ByRef lbl As String) As Short
        ' Controleer rij
        Dim Aantal, i As Integer
        Dim maxkol, maxrij As Integer

        For i = 0 To KOLOMMEN - 1
            If mBord(Rij, i) = AanZet Then
                Aantal += 1
            Else
                Aantal = 0
            End If
            If Aantal = GEWONNEN_AANTAL Then mGewonnen = True
        Next

        ' Controleer kolom
        Aantal = 0
        For i = 0 To RIJEN - 1
            If mBord(i, Kolom) = AanZet Then
                Aantal += 1
            Else
                Aantal = 0
            End If
            If Aantal = GEWONNEN_AANTAL Then mGewonnen = True
        Next

        'van links boven naar rechts onder
        Aantal = 0
        maxkol = Kolom
        maxrij = Rij
        'bepaal rechts onder
        While maxkol < KOLOMMEN - 1 And maxrij < RIJEN - 1
            maxkol += 1
            maxrij += 1
        End While
        'van rechtsonder naar links boven
        While maxkol >= 0 And maxrij >= 0 And Aantal < GEWONNEN_AANTAL
            If mBord(maxrij, maxkol) = AanZet Then
                Aantal += 1
            Else
                Aantal = 0
            End If
            maxkol -= 1
            maxrij -= 1
        End While
        If Aantal = GEWONNEN_AANTAL Then mGewonnen = True
        'van linksonder naar rechts boven
        Aantal = 0
        maxkol = Kolom
        maxrij = Rij
        'bepaal links onder
        While maxkol > 0 And maxrij < RIJEN - 1
            maxkol -= 1
            maxrij += 1
        End While
        While maxkol <= KOLOMMEN - 1 And maxrij >= 0 And Aantal < GEWONNEN_AANTAL
            If mBord(maxrij, maxkol) = AanZet Then
                Aantal += 1
            Else
                Aantal = 0
            End If
            maxkol += 1
            maxrij -= 1
        End While
        If Aantal = GEWONNEN_AANTAL Then mGewonnen = True

        If Gewonnen = True Then
            lbl = "Einde spel"
            Return True
        Else
            Return False
        End If
    End Function

    Public Function AssignKleur(ByVal Kleur1 As PictureBox, ByVal Kleur2 As PictureBox) As PictureBox
        ' Return de kleur van de huidige speler
        If AanZet = SPELER_1 Then Return Kleur1 Else Return Kleur2
    End Function

    Public Function GeldigeZetBool(ByVal DeKolom As Integer, ByRef lbl As String) As Boolean
        ' GeldigeZet returnt een nummer, GeldigeZetBool t of f
        ' Dit wordt gebruikt voor de "mouseover effecten" op de form

        Dim GeldigeZet As Short
        GeldigeZet = Me.GeldigeZet(DeKolom, lbl)

        If GeldigeZet = ZET_GELDIG Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Property Gewonnen() As Boolean
        Get
            Return mGewonnen
        End Get
        Set(ByVal Value As Boolean)
            mGewonnen = Value
        End Set
    End Property

    Public ReadOnly Property AanZet() As Short
        Get
            Return mAanzet
        End Get
    End Property
End Class
