Imports System.Math
Imports System.Threading

Public Class Form2

    '======================
    ' GAME STATE
    '======================
    Dim tankX As Single = 100
    Dim tankY As Single = 200

    Dim tankWidth As Single = 40
    Dim tankHeight As Single = 40

    Dim canMove As Boolean = True

    '======================
    ' TANK DATA
    '======================
    Dim tank1Pos As New PointF(100, 200)
    Dim tank2Pos As New PointF(500, 200)

    Const tankSize As Integer = 40

    Dim tank1Angle As Single = 0
    Dim tank2Angle As Single = 180

    Dim moveSpeed As Single = 4
    Dim rotateSpeed As Single = 4

    '======================
    ' BULLETS
    '======================
    Const bulletSize As Integer = 8
    Dim bulletSpeed As Single = 12

    Dim bullets1 As New List(Of PointF)
    Dim bullets1Vel As New List(Of PointF)
    Dim bullets2 As New List(Of PointF)
    Dim bullets2Vel As New List(Of PointF)

    Dim lastFireTime1 As DateTime = DateTime.MinValue
    Dim lastFireTime2 As DateTime = DateTime.MinValue
    Const fireDelay As Double = 0.3

    '======================
    ' INPUT
    '======================
    Dim pressedKeys As New HashSet(Of Keys)

    '======================
    ' INIT
    '======================
    Public Sub New()
        InitializeComponent()
        Me.DoubleBuffered = True
        Me.KeyPreview = True

        Timer1.Interval = 16 ' ~60 FPS
        Timer1.Start()
    End Sub

    '======================
    ' INPUT HANDLING
    '======================
    Private Sub Form2_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If Not canMove Then Exit Sub

        pressedKeys.Add(e.KeyCode)

        ' Player 1 shoot
        If e.KeyCode = Keys.Space AndAlso (DateTime.Now - lastFireTime1).TotalSeconds >= fireDelay Then
            FireBullet(tank1Pos, tank1Angle, bullets1, bullets1Vel)
            lastFireTime1 = DateTime.Now
        End If

        ' Player 2 shoot
        If e.KeyCode = Keys.Enter AndAlso (DateTime.Now - lastFireTime2).TotalSeconds >= fireDelay Then
            FireBullet(tank2Pos, tank2Angle, bullets2, bullets2Vel)
            lastFireTime2 = DateTime.Now
        End If
    End Sub

    Private Sub Form2_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
        pressedKeys.Remove(e.KeyCode)
    End Sub

    '======================
    ' GAME LOOP
    '======================
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If Not canMove Then Exit Sub

        UpdateTank1()
        UpdateTank2()

        MoveBullets(bullets1, bullets1Vel)
        MoveBullets(bullets2, bullets2Vel)

        CheckBulletCollisions()
        Invalidate()
    End Sub

    '======================
    ' TANK UPDATES
    '======================
    Sub UpdateTank1()
        If pressedKeys.Contains(Keys.A) Then tank1Angle -= rotateSpeed
        If pressedKeys.Contains(Keys.D) Then tank1Angle += rotateSpeed

        If pressedKeys.Contains(Keys.W) Then
            MoveTankForward(tank1Pos, tank1Angle, tank2Pos)
        End If
    End Sub

    Sub UpdateTank2()
        If pressedKeys.Contains(Keys.Left) Then tank2Angle -= rotateSpeed
        If pressedKeys.Contains(Keys.Right) Then tank2Angle += rotateSpeed

        If pressedKeys.Contains(Keys.Up) Then
            MoveTankForward(tank2Pos, tank2Angle, tank1Pos)
        End If
    End Sub

    '======================
    ' MOVEMENT HELPERS
    '======================
    Sub MoveTankForward(ByRef tankPos As PointF, angle As Single, otherTank As PointF)
        Dim nextPos As New PointF(
            tankPos.X + CSng(Cos(ToRadians(angle))) * moveSpeed,
            tankPos.Y + CSng(Sin(ToRadians(angle))) * moveSpeed
        )

        If Not TanksCollide(nextPos, otherTank) Then
            tankPos = nextPos
        End If
    End Sub

    Function TanksCollide(pos1 As PointF, pos2 As PointF) As Boolean
        Dim r1 As New RectangleF(pos1.X, pos1.Y, tankSize, tankSize)
        Dim r2 As New RectangleF(pos2.X, pos2.Y, tankSize, tankSize)
        Return r1.IntersectsWith(r2)
    End Function

    '======================
    ' BULLETS
    '======================
    Sub FireBullet(tankPos As PointF, angle As Single,
                   bulletList As List(Of PointF), velList As List(Of PointF))

        Dim rad As Single = ToRadians(angle)
        Dim offset As Single = tankSize / 2 + bulletSize

        Dim pos As New PointF(
            tankPos.X + tankSize / 2 + CSng(Cos(rad) * offset),
            tankPos.Y + tankSize / 2 + CSng(Sin(rad) * offset)
        )

        Dim vel As New PointF(
            CSng(Cos(rad) * bulletSpeed),
            CSng(Sin(rad) * bulletSpeed)
        )

        bulletList.Add(pos)
        velList.Add(vel)
    End Sub

    Sub MoveBullets(bullets As List(Of PointF), vels As List(Of PointF))
        For i = bullets.Count - 1 To 0 Step -1
            bullets(i) = New PointF(bullets(i).X + vels(i).X, bullets(i).Y + vels(i).Y)

            Dim vx = vels(i).X
            Dim vy = vels(i).Y

            If bullets(i).X <= 0 Or bullets(i).X >= ClientSize.Width Then vx = -vx
            If bullets(i).Y <= 0 Or bullets(i).Y >= ClientSize.Height Then vy = -vy

            vels(i) = New PointF(vx, vy)
        Next
    End Sub

    '======================
    ' COLLISIONS
    '======================
    Sub CheckBulletCollisions()
        CheckHit(bullets1, tank2Pos, "PLAYER 2 HIT!")
        CheckHit(bullets2, tank1Pos, "PLAYER 1 HIT!")
    End Sub

    Sub CheckHit(bullets As List(Of PointF), tankPos As PointF, text As String)
        For Each b In bullets
            Dim bRect As New RectangleF(b.X, b.Y, bulletSize, bulletSize)
            Dim tRect As New RectangleF(tankPos.X, tankPos.Y, tankSize, tankSize)

            If bRect.IntersectsWith(tRect) Then
                EndGame(text)
            End If
        Next
    End Sub

    '======================
    ' RENDERING
    '======================
    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)

        DrawTank(e.Graphics, tank1Pos, tank1Angle, Brushes.Green)
        DrawTank(e.Graphics, tank2Pos, tank2Angle, Brushes.Red)

        For Each b In bullets1
            e.Graphics.FillEllipse(Brushes.Black, b.X, b.Y, bulletSize, bulletSize)
        Next

        For Each b In bullets2
            e.Graphics.FillEllipse(Brushes.Black, b.X, b.Y, bulletSize, bulletSize)
        Next
    End Sub

    Sub DrawTank(g As Graphics, center As PointF, angle As Single, brush As Brush)
        g.TranslateTransform(center.X, center.Y)
        g.RotateTransform(angle)

        g.FillRectangle(
            brush,
            CSng(-tankSize / 2),
            CSng(-tankSize / 2),
            CSng(tankSize),
            CSng(tankSize)
        )
        g.ResetTransform()
    End Sub

    '======================
    ' GAME END
    '======================
    Sub EndGame(text As String)
        canMove = False
        Timer1.Stop()
        MessageBox.Show(text)
        Application.Restart()
    End Sub

    '======================
    ' UTILS
    '======================
    Function ToRadians(deg As Single) As Single
        Return CSng(deg * Math.PI / 180)
    End Function

End Class