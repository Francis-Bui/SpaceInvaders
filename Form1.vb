Public Class Form1


    Dim animateCount As Decimal = 0

    'Flags'
    Dim rightWallCheck As Boolean
    Dim leftWallCheck As Boolean
    Dim shiftDown As Boolean
    Dim createClear As Boolean
    Dim touchdown As Boolean
    Dim shipLeft As Boolean = False
    Dim shipRight As Boolean = False
    Dim shooting As Boolean = False
    Dim shootLeftWall As Boolean = False
    Dim shootRightWall As Boolean = False
    Dim alienShot As Boolean = False

    'Alien Constants'
    Dim maxAlien As Integer = 55
    Dim alienXVel As Integer = 5
    Dim alienYVel As Integer = 20
    Dim alienSize As Integer = 30
    Dim yInc As Integer = 35

    'Alien Variables'
    Dim ndx As Integer
    Dim create As Integer
    Dim damaged(maxAlien) As Integer
    Dim row(maxAlien) As Integer
    Dim prevX As Integer
    Dim prevY As Integer
    Dim newX As Integer
    Dim newY As Integer
    Dim mothershipDamage As Integer

    'Ship Constants'
    Dim shipXVel As Integer = 15

    'Ship Variables'
    Dim shipDamage As Integer

    'Laser Constants'
    Dim maxLaser As Integer = 500
    Dim laserYVel As Integer = 30

    'Laser Variables'
    Dim lcnt As Integer
    Dim createShot As Integer
    Dim createAlienShot As Integer
    Dim shootTimer As Integer
    Dim alcnt As Integer

    'Rectangles'
    Dim rAlien(maxAlien) As Rectangle
    Dim rShip As Rectangle
    Dim rLaser(maxLaser) As Rectangle
    Dim rShootAlien As Rectangle
    Dim rAlienLaser(maxLaser) As Rectangle


    Private Sub Form1_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        rShip = New Rectangle(pbCanvas.Width / 2, 900, 40, 30)

        createAlien()

        Timer1.Enabled = True

    End Sub

    Private Sub Timer1_Tick(sender As System.Object, e As System.EventArgs) Handles Timer1.Tick

        moveAlien()

        shipMovement()

        shoot()

        pbCanvas.Refresh()

    End Sub


    Private Sub Timer2_Tick(sender As System.Object, e As System.EventArgs) Handles Timer2.Tick

        animateCount += 1
        shootTimer += 1

        'alien shoot timer'
        If shootTimer Mod 2 = 0 Then
            alienShot = True
        Else
            alienShot = False
        End If

    End Sub

    Private Sub pbCanvas_Paint(sender As System.Object, e As System.Windows.Forms.PaintEventArgs) Handles pbCanvas.Paint

        For ndx As Integer = 0 To maxAlien - 1


            For lcnt As Integer = 0 To createShot - 1

                If rLaser(lcnt).Y > pbCanvas.Top Then
                    'laser to alien collision'
                    If rLaser(lcnt).IntersectsWith(rAlien(ndx)) And damaged(ndx) < 3 Then
                        damaged(ndx) += 1
                        'hide laser'
                        rLaser(lcnt).Y = -200
                        rLaser(lcnt).X = -200
                    End If

                    If rLaser(lcnt).IntersectsWith(rShootAlien) Then
                        mothershipDamage += 1
                        rLaser(lcnt).Y = -200
                        rLaser(lcnt).X = -200
                    End If


                End If

            Next

            'What to draw'
            If row(ndx) = 1 And damaged(ndx) = 0 Then
                animate(My.Resources.InvaderA1, My.Resources.InvaderA2, rAlien(ndx), e)
            ElseIf row(ndx) = 1 And damaged(ndx) = 1 Then
                animate(My.Resources.InvaderRA1, My.Resources.InvaderRA2, rAlien(ndx), e)

            ElseIf row(ndx) = 2 And damaged(ndx) = 0 Then
                animate(My.Resources.InvaderB1, My.Resources.InvaderB2, rAlien(ndx), e)
            ElseIf row(ndx) = 2 And damaged(ndx) = 1 Then
                animate(My.Resources.InvaderRB1, My.Resources.InvaderRB2, rAlien(ndx), e)

            ElseIf row(ndx) = 3 And damaged(ndx) = 0 Then
                animate(My.Resources.InvaderC1, My.Resources.InvaderC2, rAlien(ndx), e)
            ElseIf row(ndx) = 3 And damaged(ndx) = 1 Then
                animate(My.Resources.InvaderRC1, My.Resources.InvaderRC2, rAlien(ndx), e)

            ElseIf row(ndx) = 4 And damaged(ndx) = 0 Then
                animate(My.Resources.InvaderA1, My.Resources.InvaderA2, rAlien(ndx), e)
            ElseIf row(ndx) = 4 And damaged(ndx) = 1 Then
                animate(My.Resources.InvaderRA1, My.Resources.InvaderRA2, rAlien(ndx), e)

            ElseIf row(ndx) = 5 And damaged(ndx) = 0 Then
                animate(My.Resources.InvaderB1, My.Resources.InvaderB2, rAlien(ndx), e)
            ElseIf row(ndx) = 5 And damaged(ndx) = 1 Then
                animate(My.Resources.InvaderRB1, My.Resources.InvaderRB2, rAlien(ndx), e)
            End If

        Next

        'Laser to Ship Collision'
        For alcnt As Integer = 0 To createAlienShot - 1
            If rAlienLaser(alcnt).IntersectsWith(rShip) Then
                shipDamage += 1
            End If
        Next

        'draw ship laser'
        For lcnt As Integer = 0 To createShot - 1
            If rLaser(lcnt).Y > pbCanvas.Top Then
                e.Graphics.FillRectangle(Brushes.White, rLaser(lcnt))
            End If
        Next
        'draw alien laser'
        For alcnt As Integer = 0 To createAlienShot - 1
            If rAlienLaser(alcnt).Y > pbCanvas.Top Then
                e.Graphics.FillRectangle(Brushes.Red, rAlienLaser(alcnt))
            End If
        Next

        'when aliens have reached the ship // ship hp'
        If touchdown = False And shipDamage = 0 Then
            e.Graphics.DrawImage(My.Resources.Ship, rShip)
        ElseIf touchdown = False And shipDamage = 1 Then
            e.Graphics.DrawImage(My.Resources.ShipCrushedLeft, rShip)
        ElseIf touchdown = False And shipDamage = 2 Then
            e.Graphics.DrawImage(My.Resources.ShipCrushedFull, rShip)
        End If

        'draw red invader'
        If mothershipDamage = 0 Then
            e.Graphics.DrawImage(My.Resources.RedInvader, rShootAlien)
        ElseIf mothershipDamage = 1 Then
            animate(My.Resources.RedInvader, My.Resources.RedInvaderWhite, rShootAlien, e)
        End If

    End Sub

    Private Sub shoot()

        If shooting = True And shipDamage < 3 Then

            rLaser(lcnt) = New Rectangle(rShip.X + 20, rShip.Y - 3, 5, 10)

            lcnt += 1

            shooting = False

            createShot += 1
        End If


        For lcnt As Integer = 0 To createShot - 1

            rLaser(lcnt).Y -= laserYVel

        Next

        If alienShot = True And mothershipDamage < 2 Then

            rAlienLaser(alcnt) = New Rectangle(rShootAlien.X + 20, rShootAlien.Y - 3, 15, 30)

            alcnt += 1

            alienShot = False

            createAlienShot += 1
        End If

        For alcnt As Integer = 0 To createAlienShot - 1
            rAlienLaser(alcnt).Y += laserYVel + 30
        Next

    End Sub

    Private Sub createAlien()

        For ndx As Integer = 0 To maxAlien - 1

            create += 1
            'ndx += 1

            prevX = newX
            prevY = newY

            newX = prevX + 35

            'new row'
            If ndx = 11 Or ndx = 22 Or ndx = 33 Or ndx = 44 Or ndx = 55 Then
                newY += yInc
                newX = 30

            End If

            If ndx <= 10 And ndx >= 0 Then
                row(ndx) += 1
            End If

            If ndx <= 21 And ndx >= 11 Then
                row(ndx) += 2
            End If

            If ndx <= 32 And ndx >= 22 Then
                row(ndx) += 3
            End If

            If ndx <= 43 And ndx >= 33 Then
                row(ndx) += 4
            End If

            If ndx <= 54 And ndx >= 44 Then
                row(ndx) += 5
            End If
            rAlien(ndx) = New Rectangle(newX, newY, alienSize, alienSize)

        Next

        rShootAlien = New Rectangle(0, -35, alienSize, alienSize)

    End Sub


    Private Sub moveAlien()

            'shifts aliens down'
        If shiftDown = True Then

            rShootAlien.Y += alienYVel

            For ndx As Integer = 0 To 54

                rAlien(ndx).Y += alienYVel

                shiftDown = False

            Next
        End If

            'changes alien xvel if they hit the border'
        For x As Integer = 0 To 54

            If rightWallCheck = True Then
                rAlien(x).X -= alienXVel
            End If

            If leftWallCheck = True Then
                rAlien(x).X += alienXVel
            End If

        Next

            'Checks if aliens have hit the border'
            For ndx As Integer = 0 To 54

            If rAlien(ndx).X - 35 < pbCanvas.Left Then
                leftWallCheck = True
                rightWallCheck = False
                shiftDown = True
            End If

            If rAlien(ndx).X + alienSize + 35 > pbCanvas.Right Then
                rightWallCheck = True
                leftWallCheck = False
                shiftDown = True
            End If

                If rAlien(ndx).Y + 30 > rShip.Y Then
                    touchdown = True
                End If

        Next

        If rShootAlien.X - 40 < pbCanvas.Left Then
            shootLeftWall = True
            shootRightWall = False
        End If

        If rShootAlien.X + 40 > pbCanvas.Right Then
            shootLeftWall = False
            shootRightWall = True
        End If

        If shootLeftWall = True Then
            rShootAlien.X += alienXVel + 20
            shootRightWall = False
        End If

        If shootRightWall = True Then
            rShootAlien.X -= alienXVel + 20
            shootLeftWall = False
        End If


    End Sub

    Private Sub shipMovement()

        If shipLeft = True And rShip.Left > pbCanvas.Left Then
            rShip.X -= shipXVel
        End If

        If shipRight = True And rShip.Right < pbCanvas.Right Then
            rShip.X += shipXVel
        End If

    End Sub

    Private Sub Form1_KeyDown(sender As System.Object, e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown

        If e.KeyData = Keys.A Then
            shipLeft = True
            shipRight = False
        End If

        If e.KeyData = Keys.D Then
            shipRight = True
            shipLeft = False
        End If

        If e.KeyData = Keys.Space Then
            shooting = True
        End If

    End Sub

    Private Sub Form1_KeyUp(sender As System.Object, e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyUp

        If e.KeyData = Keys.A Then
            shipLeft = False
        End If

        If e.KeyData = Keys.D Then
            shipRight = False
        End If

        If e.KeyData = Keys.Space Then
            shooting = False
        End If

    End Sub

    Private Sub animate(alienOne As System.Drawing.Image, alienTwo As System.Drawing.Image, alienLocation As System.Drawing.Rectangle, e As System.Windows.Forms.PaintEventArgs)

        If animateCount Mod 2 = 0 Then
            e.Graphics.DrawImage(alienOne, alienLocation)
        Else
            e.Graphics.DrawImage(alienTwo, alienLocation)
        End If
    End Sub

End Class
