Public Class Form4
    Dim backImage As Image
    Dim firstCard As PictureBox = Nothing
    Dim secondCard As PictureBox = Nothing
    Dim isBusy As Boolean = False
    Dim currentPlayer As Integer = 1
    Dim playerScore(2) As Integer
    Dim matchedPairs As Integer = 0
    Const TOTAL_CARDS As Integer = 30
    Const TOTAL_PAIRS As Integer = 15

    Dim cardImages() As String = {
        "",
        "Images\Screenshot 2026-02-08 225741.png",
        "Images\Screenshot 2026-02-08 225734.png",
        "Images\Screenshot 2026-02-08 225726.png",
        "Images\Screenshot 2026-02-08 225715.png",
        "Images\Screenshot 2026-02-08 225708.png",
        "Images\Screenshot 2026-02-08 225654.png",
        "Images\Screenshot 2026-02-08 225645.png",
        "Images\Screenshot 2026-02-08 225630.png",
        "Images\Screenshot 2026-02-08 225622.png",
        "Images\Screenshot 2026-02-08 225614.png",
        "Images\Screenshot 2026-02-08 225607.png",
        "Images\Screenshot 2026-02-09 062330.png",
        "Images\Screenshot 2026-02-09 062339.png",
        "Images\Screenshot 2026-02-09 062517.png",
        "Images\Screenshot 2026-02-09 062528.png"
    }

    Private Sub Form4_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        backImage = Image.FromFile("Images\Screenshot 2026-02-08 221926.png")
        Label1.Hide()
        SetupPairs()
        ShuffleCards()
        ResetBoard()
        Timer1.Interval = 1000
    End Sub

    Sub SetupPairs()
        Dim index As Integer = 1
        For pair As Integer = 1 To TOTAL_PAIRS
            For repeat As Integer = 1 To 2
                CType(Controls("PictureBox" & index), PictureBox).Tag = pair
                index += 1
            Next
        Next
    End Sub

    Sub ShuffleCards()
        Dim rnd As New Random
        For i As Integer = TOTAL_CARDS To 2 Step -1
            Dim j As Integer = rnd.Next(1, i + 1)
            Dim a = CType(Controls("PictureBox" & i), PictureBox)
            Dim b = CType(Controls("PictureBox" & j), PictureBox)
            Dim temp As Integer = CInt(a.Tag)
            a.Tag = b.Tag
            b.Tag = temp
        Next
    End Sub

    Sub ResetBoard()
        matchedPairs = 0
        currentPlayer = 1
        playerScore(1) = 0
        playerScore(2) = 0

        firstCard = Nothing
        secondCard = Nothing
        isBusy = False

        For i As Integer = 1 To TOTAL_CARDS
            Dim pic = CType(Controls("PictureBox" & i), PictureBox)
            pic.Image = backImage
            pic.Visible = True
        Next
    End Sub

    Private Sub Card_Click(sender As Object, e As EventArgs) _
        Handles PictureBox1.Click, PictureBox2.Click, PictureBox3.Click,
        PictureBox4.Click, PictureBox5.Click, PictureBox6.Click,
        PictureBox7.Click, PictureBox8.Click, PictureBox9.Click,
        PictureBox10.Click, PictureBox11.Click, PictureBox12.Click,
        PictureBox13.Click, PictureBox14.Click, PictureBox15.Click,
        PictureBox16.Click, PictureBox17.Click, PictureBox18.Click,
        PictureBox19.Click, PictureBox20.Click, PictureBox21.Click,
        PictureBox22.Click, PictureBox23.Click, PictureBox24.Click,
        PictureBox25.Click, PictureBox26.Click, PictureBox27.Click,
        PictureBox28.Click, PictureBox29.Click, PictureBox30.Click

        If isBusy Then Exit Sub

        Dim card = CType(sender, PictureBox)
        If card Is firstCard OrElse Not card.Visible Then Exit Sub

        card.Image = Image.FromFile(cardImages(card.Tag))

        If firstCard Is Nothing Then
            firstCard = card
        Else
            secondCard = card
            isBusy = True
            Timer1.Start()
        End If
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Timer1.Stop()
        If firstCard.Tag = secondCard.Tag Then
            firstCard.Visible = False
            secondCard.Visible = False
            matchedPairs += 1
            playerScore(currentPlayer) += 1
            currentPlayer = If(currentPlayer = 1, 1, 2)
        Else
            firstCard.Image = backImage
            secondCard.Image = backImage
            currentPlayer = If(currentPlayer = 1, 2, 1)
        End If

        Label3.Text = $"Player: {currentPlayer}"
        Label2.Text = $"        Player 1: {playerScore(1)}                     Player 2: {playerScore(2)        }"
        firstCard = Nothing
        secondCard = Nothing
        isBusy = False
        If matchedPairs = TOTAL_PAIRS Then ShowWinner()
    End Sub

    Sub ShowWinner()
        Label1.Show()
        If (playerScore(1) > playerScore(2)) Then
            Label1.Text = "Player 1 Wins!"
        ElseIf (playerScore(2) > playerScore(1)) Then
            Label1.Text = "Player 2 Wins!"
        Else
            Label1.Text = "It's a Tie!"
        End If
        Timer2.Interval = 3000
        Timer2.Start()
    End Sub
    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
        Timer2.Stop()
        Me.Hide()
        Form3.Show()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        SetupPairs()
        ShuffleCards()
        ResetBoard()
        Me.Hide()
        Form3.Show()
    End Sub
End Class