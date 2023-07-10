Public Module ModMinecraft

#Region "声明"
    ''' <summary>
    ''' 上一次剪贴板中的内容。
    ''' </summary>
    Public LastClip As String = ""
#End Region

#Region "Minecraft"
    '/execute in minecraft:overworld run tp @s -17.97 -60.00 -7.00 191.40 -2.10
    '0 固定，/execute
    '1 固定，in
    '2 维度，如果包含 overworld 则有效，否则不判断
    '3 固定，run
    '4 固定，tp
    '5 固定，@s
    '6 X 坐标
    '7 Y 坐标
    '8 Z 坐标
    '9 水平角度，若大于 180，则将其减去 360 使用
    '10 俯仰角度
    Public Class McLocationInfo
        ''' <summary>
        ''' 维度。
        ''' </summary>
        Public Property Dimention As String
        Public Property X As Single
        Public Property Z As Single
        ''' <summary>
        ''' 水平角，保证该值介于 -180 与 180 之间。
        ''' </summary>
        Public Property Horizontal As Single
        '''' <summary>
        '''' 俯仰角，介于 -90 和 90 之间。
        '''' </summary>
        'Public Property Pitch As Single
        ''' <summary>
        ''' 从 F3+C 复制的坐标信息导入。
        ''' </summary>
        Public Sub FromString(content As String)
            Try
                If Not content.StartsWith("/execute in ") Then Throw New ArgumentException("不是有效的 /execute 指令", "content")
                Dim info As String() = Spilt(content, " ")
                Dimention = info(2)
                X = Val(info(6))
                'Y = Val(info(7))
                Z = Val(info(8))
                Dim oriHor As Single = Val(info(9))
                Do Until oriHor <= 180 AndAlso oriHor >= -180
                    If oriHor > 180 Then
                        oriHor -= 360
                    ElseIf oriHor < -180 Then
                        oriHor += 360
                    Else
                        Exit Do
                    End If
                Loop
                Horizontal = oriHor
            Catch ex As Exception
                Throw New Exception("导入坐标信息失败", ex)
            End Try
        End Sub
    End Class
#End Region

#Region "要塞"
#If False Then
    一个眼睛（最终还是没有解出这个二元二次……现在这个过程就留在这里存档了）
    z^2+x^2=1728^2, x-posX=-tan(a)(z-posZ)
    x=-tan(a)(z-posZ)
    z^2+[-tan(a)(z-posZ)]^2=1728^2
    z^2+tan(a)^2*(z-posZ)^2=1728^2
    z^2+tan(a)^2*(z^2-2z*posZ+posZ^2)=1728^2
    z^2 + tan(a)^2*z^2 - tan(a)^2*2z*posZ + tan(a)^2*posZ^2 = 1728^2
    [tan(a)^2+1]z^2 - 2tan(a)^2*posZ*z + [tan(a)^2*posZ^2-1728^2] = 0
    a=tan(a)^2+1, b=-2tan(a)^2*posZ, c=tan(a)^2*posZ^2-1728^2
    delta=[2tan(a)^2*posZ]^2-4[tan(a)^2 + 1][tan(a)^2*posZ^2 - 1728^2]
         =4tan(a)^4*posZ^2 - 4[tan(a)^4*posZ^2 - tan(a)^2*1728^2 + tan(a)^2*posZ^2 - 1728^2
         =4tan(a)^4*posZ^2 - 4tan(a)^4*posZ^2 - 4tan(a)^2*1728^2 + 4tan(a)^2*posZ^2 - 4*1728^2
         =-4tan(a)^2*1728^2 + 4tan(a)^2*posZ^2 - 4*1728^2
         =4[-tan(a)^2*1728^2+tan(a)^2*posZ^2-1728^2]
    z=(-b+-sqrt(delta))/2a
     =(2tan(a)^2*posZ +- sqrt{4[-tan(a)^2*1728^2+tan(a)^2*posZ^2-1728^2]})/2(tan(a)^2+1)
     ={2tan(a)^2*posZ +- 2sqrt[-tan(a)^2*1728^2+tan(a)^2*posZ^2-1728^2]}/2(tan(a)^2+1)
#End If
    Public Function GetPosition(loc1 As McLocationInfo, Optional loc2 As McLocationInfo = Nothing) As Integer()
        If Not loc1.Dimention.Contains("overworld") OrElse Not loc2?.Dimention.Contains("overworld") Then
            Throw New Exception("不在有效的维度。")
        End If
        If loc2 Is Nothing Then
            '一个眼睛
            '烦死了，微软数学它根本解不出，更别说我这个七年级学历了，直接借鉴 Shrans/Stronghold-Locator 的计算结果
            '如有侵权请联系 mailto:youzi5201086@163.com
            If Math.Sqrt((loc1.X ^ 2) + (loc1.Z ^ 2)) > 1728 Then Return Nothing
            Dim k As Double = -Math.Tan(Math.PI / 180 * loc1.Horizontal)
            Dim delta As Double = Math.Sqrt(Math.Abs(
                (1728 ^ 2) - ((loc1.Z * k) ^ 2) + ((1728 ^ 2) * (k ^ 2)) + (2 * loc1.X * loc1.Z * k) - (loc1.X ^ 2)))
            Dim divisor = (k ^ 2) + 1
            Dim x1 As Double = (-k * delta - (loc1.Z * k) + loc1.X) / divisor
            Dim x2 As Double = (k * delta - (loc1.Z * k) + loc1.X) / divisor
            Dim z1 As Double = ((loc1.Z * (k ^ 2)) - delta - (loc1.X * k)) / divisor
            Dim z2 As Double = ((loc1.Z * (k ^ 2)) + delta - (loc1.X * k)) / divisor
            If 0 < loc1.Horizontal AndAlso loc1.Horizontal < 90 Then
                If x1 < loc1.X AndAlso z1 > loc1.Z Then
                    Return {x1, z1}
                Else
                    Return {x2, z2}
                End If
            ElseIf 90 < loc1.Horizontal AndAlso loc1.Horizontal < 180 Then
                If x1 < loc1.X AndAlso z1 < loc1.Z Then
                    Return {x1, z1}
                Else
                    Return {x2, z2}
                End If
            ElseIf -90 < loc1.Horizontal AndAlso loc1.Horizontal < 0 Then
                If x1 > loc1.X AndAlso z1 > loc1.Z Then
                    Return {x1, z1}
                Else
                    Return {x2, z2}
                End If
            ElseIf -180 < loc1.Horizontal AndAlso loc1.Horizontal < -90 Then
                If x1 > loc1.X AndAlso z1 < loc1.Z Then
                    Return {x1, z1}
                Else
                    Return {x2, z2}
                End If
            ElseIf Math.Abs(loc1.Horizontal) = 90 Then
                If x1 > loc1.X AndAlso loc1.X > x2 Then
                    Return {x1, z1}
                Else
                    Return {x2, z2}
                End If
            Else
                Return Nothing
            End If
        Else
            '两个眼睛
            Dim k1 As Double = -Math.Tan(Math.PI / 180 * loc1.Horizontal)
            Dim k2 As Double = -Math.Tan(Math.PI / 180 * loc2.Horizontal)
            Dim x As Integer = -((loc1.Z * k1 * k2) - (loc2.Z * k1 * k2) - (loc1.X * k2) + (loc2.X * k1 / (k2 - k1)))
            Dim z As Integer = -(((loc1.Z * k1) - (loc2.Z * k2) - loc1.X + loc2.X) / (k2 - k1))
            Return {x, z}
        End If
    End Function
#End Region

End Module
