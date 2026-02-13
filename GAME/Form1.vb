Public Class Form1
    Dim p1Up, p1Down, p1Left, p1Right As Boolean
    Dim p2Up, p2Down, p2Left, p2Right As Boolean
    Dim countdown As Integer = 3
    Dim speed As Integer = 5
    Dim enemySpeed As Integer = 6
    Dim canMove As Boolean = True
    Dim walls As New List(Of PictureBox)
    Dim deathZones As New List(Of PictureBox)

    Dim Cat1GoingDown = True
    Dim Cat2GoingDown = True
    Dim Cat3GoingDown = False
    Dim Cat4GoingDown = False
    Dim BigCatGoingDown = True
    Dim MT1GoingRight = True
    Dim MT2GoingRight = False
    Dim MT3GoingRight = True
    Dim MT4GoingRight = True
    Dim MT5GoingRight = False
    Dim MT6GoingRight = True
    Dim p1Start As New Point(448, 465)
    Dim p2Start As New Point(448, 531)

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Timer1.Interval = 20
        Timer1.Enabled = True
        canMove = False
        Timer1.Enabled = False
        Timer2.Interval = 1000
        AddHandler Timer2.Tick, AddressOf Countdown_Tick
        Timer2.Start()
        Label1.Text = countdown.ToString()
        Label1.Visible = True

        walls.AddRange({
            PictureBox1, PictureBox2, PictureBox3, PictureBox4,
            PictureBox6, PictureBox7, PictureBox8, PictureBox9, PictureBox10
        })
        deathZones.AddRange({Cat1, Cat2, Cat3, BigCat, MT1, MT2, MT3, MT4, MT5, MT6})
        ResetPlayers()
    End Sub

    Private Sub Countdown_Tick(sender As Object, e As EventArgs)
        countdown -= 1
        If countdown > 0 Then
            Label1.Text = countdown.ToString()
        Else
            Timer2.Stop()
            Label1.Text = "GO!"
            canMove = True
            Timer1.Enabled = True
            Timer3.Interval = 1000
            AddHandler Timer3.Tick, Sub(s, args)
                                        Label1.Visible = False
                                        Timer3.Stop()
                                    End Sub
            Timer3.Start()
        End If
    End Sub

    Private Sub ResetPlayers()
        Panel1.Location = p1Start
        Panel4.Location = p2Start

        p1Up = False : p1Down = False : p1Left = False : p1Right = False
        p2Up = False : p2Down = False : p2Left = False : p2Right = False
    End Sub

    Private Sub ResetGame()
        Timer1.Enabled = False
        Timer2.Stop()
        Timer3.Stop()
        canMove = False
        countdown = 3
        Label1.Text = countdown.ToString()
        Label1.Visible = True
        ResetPlayers()

        Cat1GoingDown = True
        Cat2GoingDown = True
        Cat3GoingDown = False
        Cat4GoingDown = False
        BigCatGoingDown = True
        MT1GoingRight = True
        MT2GoingRight = False
        MT3GoingRight = True
        MT4GoingRight = True
        MT5GoingRight = False
        MT6GoingRight = True
        Cat1.Top = 240
        Cat2.Top = 240
        Cat3.Top = 693
        Cat4.Top = 693
        BigCat.Top = 240
        MT1.Left = 1379
        MT2.Left = 1503
        MT3.Left = 1379
        MT4.Left = 1379
        MT5.Left = 1503
        MT6.Left = 1379

        Timer2.Start()
    End Sub

    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If canMove = False Then Exit Sub

        If e.KeyCode = Keys.W Then p1Up = True
        If e.KeyCode = Keys.S Then p1Down = True
        If e.KeyCode = Keys.A Then p1Left = True
        If e.KeyCode = Keys.D Then p1Right = True

        If e.KeyCode = Keys.Up Then p2Up = True
        If e.KeyCode = Keys.Down Then p2Down = True
        If e.KeyCode = Keys.Left Then p2Left = True
        If e.KeyCode = Keys.Right Then p2Right = True
    End Sub

    Private Sub Form1_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
        If e.KeyCode = Keys.W Then p1Up = False
        If e.KeyCode = Keys.S Then p1Down = False
        If e.KeyCode = Keys.A Then p1Left = False
        If e.KeyCode = Keys.D Then p1Right = False

        If e.KeyCode = Keys.Up Then p2Up = False
        If e.KeyCode = Keys.Down Then p2Down = False
        If e.KeyCode = Keys.Left Then p2Left = False
        If e.KeyCode = Keys.Right Then p2Right = False
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If canMove = False Then Exit Sub
        MoveVertical(Cat1, 240, 693, Cat1GoingDown)
        MoveVertical(Cat2, 240, 693, Cat2GoingDown)
        MoveVertical(Cat3, 240, 693, Cat3GoingDown)
        MoveVertical(Cat4, 240, 693, Cat4GoingDown)
        MoveVertical(BigCat, 240, 581, BigCatGoingDown)
        MoveHorizontal(MT1, 1379, 1503, MT1GoingRight)
        MoveHorizontal(MT2, 1379, 1503, MT2GoingRight)
        MoveHorizontal(MT3, 1379, 1503, MT3GoingRight)
        MoveHorizontal(MT5, 1379, 1503, MT5GoingRight)
        MoveHorizontal(MT4, 1379, 1503, MT4GoingRight)
        MoveHorizontal(MT6, 1379, 1503, MT6GoingRight)

        If p1Up Then MovePlayer(Panel1, 0, -speed)
        If p1Down Then MovePlayer(Panel1, 0, speed)
        If p1Left Then MovePlayer(Panel1, -speed, 0)
        If p1Right Then MovePlayer(Panel1, speed, 0)

        If p2Up Then MovePlayer(Panel4, 0, -speed)
        If p2Down Then MovePlayer(Panel4, 0, speed)
        If p2Left Then MovePlayer(Panel4, -speed, 0)
        If p2Right Then MovePlayer(Panel4, speed, 0)

        CheckDeath(Panel1)
        CheckDeath(Panel4)

        CheckWin(Panel1)
        CheckWin(Panel4)
    End Sub

    Private Sub MovePlayer(p As Panel, dx As Integer, dy As Integer)
        p.Left += dx
        p.Top += dy
        If CollidesWithWalls(p) Then
            p.Left -= dx
            p.Top -= dy
        End If
    End Sub

    Private Function CollidesWithWalls(p As Control) As Boolean
        For Each w In walls
            If p.Bounds.IntersectsWith(w.Bounds) Then Return True
        Next
        Return False
    End Function

    Private Sub MoveVertical(enemy As PictureBox, minY As Integer, maxY As Integer, ByRef down As Boolean)
        If down Then
            enemy.Top += enemySpeed
            If enemy.Top >= maxY Then down = False
        Else
            enemy.Top -= enemySpeed
            If enemy.Top <= minY Then down = True
        End If
    End Sub
    Private Sub MoveHorizontal(enemy As PictureBox, minX As Integer, maxX As Integer, ByRef right As Boolean)
        If right Then
            enemy.Left += enemySpeed
            If enemy.Left >= maxX Then right = False
        Else
            enemy.Left -= enemySpeed
            If enemy.Left <= minX Then right = True
        End If
    End Sub

    Private Sub CheckDeath(p As Panel)
        For Each d In deathZones
            If p.Bounds.IntersectsWith(d.Bounds) Then
                If p Is Panel1 Then
                    p.Location = p1Start
                ElseIf p Is Panel4 Then
                    p.Location = p2Start
                End If
                p1Up = False : p1Down = False : p1Left = False : p1Right = False
                p2Up = False : p2Down = False : p2Left = False : p2Right = False

                Exit Sub
            End If
        Next
    End Sub
    Private Sub CheckWin(p As Panel)
        If p.Bounds.IntersectsWith(Finish.Bounds) Then
            Timer1.Enabled = False
            canMove = False
            If p Is Panel1 Then
                MessageBox.Show("Player 1 wins!")
            ElseIf p Is Panel4 Then
                MessageBox.Show("Player 2 wins!")
            End If
            Me.Hide()
            Form3.Show()
            ResetGame()
        End If
    End Sub
End Class