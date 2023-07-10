Imports System.Threading
Imports Microsoft.Toolkit.Uwp.Notifications

Public Module ModMain

#Region "主函数"
    Public Sub OnStart()
        ShowNotice("要塞定位", "请扔出一个末影之眼，将准星对准它，然后按下 F3+C。")
Retry1:
        LastClip = GetClipboard()
        Do
            If GetClipboard() <> LastClip Then Exit Do
            Thread.Sleep(500)
        Loop
        Dim loc1 As New McLocationInfo
        Try
            loc1.FromString(GetClipboard())
        Catch
            GoTo Retry1
        End Try
        Dim sh As Integer() = GetPosition(loc1)
        If sh Is Nothing Then
            ShowNotice("要塞定位", "请在距离原点 1728 格内的位置定位要塞。")
            GoTo Retry1
        Else
            ShowNotice("要塞定位", $"要塞坐标：{sh(0)}/{sh(1)}，在不同地点重复上述步骤可以使坐标更精确。")
        End If

Retry2:
        LastClip = GetClipboard()
        Do
            If GetClipboard() <> LastClip Then Exit Do
            Thread.Sleep(500)
        Loop
        Dim loc2 As New McLocationInfo
        Try
            loc2.FromString(GetClipboard())
        Catch
            GoTo Retry2
        End Try
        sh = GetPosition(loc1, loc2)
        If sh Is Nothing Then
            ShowNotice("要塞定位", "请在距离原点 1728 格内的位置定位要塞。")
            GoTo Retry2
        Else
            ShowNotice("要塞定位", $"要塞坐标：{sh(0)}/{sh(1)}。")
        End If
        Process.GetCurrentProcess.Kill()
    End Sub
#End Region

#Region "剪贴板"
    ''' <summary>
    ''' 获取剪贴板内容。
    ''' </summary>
    ''' <returns>剪贴板内容。</returns>
    Public Function GetClipboard() As String
        Dim clipboardText As String = ""

        '检查剪贴板中是否有文本数据
        If Clipboard.ContainsText() Then
            clipboardText = Clipboard.GetText()
        End If

        Return clipboardText
    End Function
#End Region

#Region "通知"
    ''' <summary>
    ''' 发送窗口右下角通知。
    ''' </summary>
    ''' <param name="title">标题。</param>
    ''' <param name="content">内容。</param>
    Public Sub ShowNotice(title As String, content As String)
        Try
            Dim builder As New ToastContentBuilder
            builder.AddText(title)
            builder.AddText(content)
            builder.Show()
        Catch ex As Exception
            MsgBox(content,, title)
        End Try
    End Sub
#End Region

#Region "文本"
    ''' <summary>
    ''' 分割字符串。
    ''' </summary>
    ''' <param name="content"></param>
    ''' <param name="c"></param>
    ''' <returns></returns>
    Public Function Spilt(content As String, c As String) As String()
        Return content.Split(c.ToCharArray.FirstOrDefault)
    End Function
#End Region

End Module
