'      (_)
'     <___>
'      | |_______
'      | |`-._`-.)
'      | |`-._`-(__________
'      | |    `-.| :|    |:)
'      | | _ _ _ | :|    |(__________
'      | |-------| :|    |:| _.-'_.-'|
'      | |       |`:|    |:|'_.-'_.-'|
'      | |_______|--      -|'_.-'    |
'      | |- - - -|   BONK  |' _ _ _ _|
'      | |     _.|__      _|---------|
'      | | _.-'_.|-:|    |:|         |
'      | |'_.-'_.|':|    |:|_________|
'      | |~~~~~~~| :|    |:|- - - - -|
'      | |       | :|    |:|`-._     |
'      | |       '~~~~~~~~~|`-._`-._ |
'      | |                 |`-._`-._`|
'      | |                 '~~~~~~~~~~
'      | |
'jams jams bonk her magistrates seacrate cervix
'To lower the imports so its more readable when compiling



Imports System.IO
Imports System.Net

Public Class main
    Dim drag As Boolean
    Dim mousex As Integer
    Dim mousey As Integer
    Dim ticker_results As String()
    Dim ticker_list As String()
    Dim algo As String = "cryptonight"
    Dim coin As String = "etn"
    Dim fullcoin As String = "monero"
    Dim pool_info As String()
    Dim pool_uptime As New Process()
    Dim cpum As New Process()
    Dim cpum_state As Boolean = False
    Dim xmr_stak_proc_state As Boolean = False
    Dim xmr_stak_proc As New Process()
    Dim cpu0 As Single = "0"
    Dim cpu1 As Single = "0"
    Dim cpu2 As Single = "0"
    Dim cpu3 As Single = "0"
    'THE 'ARRAY' STARTS AT 1 SUE ME
    Dim coindex As String = "coin2"
    Dim pool_uptime_state As Boolean = False



    Dim version As String = "1.0.0"
    Private Sub green_Click(sender As Object, e As EventArgs) Handles green.Click
        If Me.WindowState = FormWindowState.Maximized Then
            Me.WindowState = FormWindowState.Normal
        ElseIf Me.WindowState = FormWindowState.Normal Then
            Me.WindowState = FormWindowState.Maximized
        End If
    End Sub
    Private Sub ETN_earned_Nanopool()
        Using client = New WebClient()
            Dim nanopool_etn_earned = client.DownloadString("https://api.nanopool.org/v1/etn/balance/" & textbox_wallet_address.Text)
            nanopool_etn_earned = nanopool_etn_earned.Replace("{""status"":true,""data"":", "")
            nanopool_etn_earned = nanopool_etn_earned.Replace("}", "")
            lbl_coin_Earned.Text = coin.ToUpper & " Earned: " & nanopool_etn_earned
        End Using
    End Sub

    Private Sub ETN_earned_Spacepools()
        'foranotherday
    End Sub
    Delegate Sub PoolUptimeBoxDelg(cpu_probe As String)
    Public pool_uptime_delegate As PoolUptimeBoxDelg = New PoolUptimeBoxDelg(AddressOf pool_uptime_probe)
    Private Sub kill_pool_uptime()
        If pool_uptime_state = True Then
            pool_uptime.Kill()
            RemoveHandler pool_uptime.ErrorDataReceived, AddressOf pool_uptime_OutputDataReceived
            RemoveHandler pool_uptime.OutputDataReceived, AddressOf pool_uptime_OutputDataReceived
            pool_uptime.CancelErrorRead()
            pool_uptime.CancelOutputRead()
            droplist_pool.Enabled = True
            pool_uptime_state = False
        End If
    End Sub
    Public Sub pool_uptime_probe(cpu_probe As String)
        If cpu_probe.Contains("CPU #") Then
            lbl_pool_status.Text = "Pool Status: Up and running!"
            Call kill_pool_uptime()
        ElseIf cpu_probe.Contains("failed") Then
            lbl_pool_status.Text = "Pool Status: Pool is down/incorrect port"
            Call kill_pool_uptime()
        ElseIf cpu_probe.Contains("pc2_login_decode") Then
            Call kill_pool_uptime()
            lbl_pool_status.Text = "Pool Status: Pool Online, Invalid wallet address"
            droplist_pool.Enabled = True
        ElseIf cpu_probe.Contains("resolve host") Then
            lbl_pool_status.Text = "Pool Status: Pool down/Invalid pool address"
            Call kill_pool_uptime()
        End If
        'rpc2_login_decode


    End Sub
    Public Sub pool_uptime_OutputDataReceived(ByVal sender As Object, ByVal e As DataReceivedEventArgs)
        If Me.InvokeRequired = True Then
            Me.Invoke(pool_uptime_delegate, e.Data)
        Else
            pool_uptime_probe(e.Data)
        End If

    End Sub
    Private Sub check_pool_uptime()
        pool_uptime.StartInfo.FileName = ".\backend\cpuminer.exe"
        pool_uptime.StartInfo.Arguments = ("-a " & algo & " -o stratum+tcp://" & pool_info(0) & ":" & textbox_port.Text & " -u " & textbox_wallet_address.Text & " -p x -t 1" & "pause")
        pool_uptime.StartInfo.WorkingDirectory = ""
        pool_uptime.StartInfo.RedirectStandardError = True
        pool_uptime.StartInfo.RedirectStandardOutput = True
        pool_uptime.EnableRaisingEvents = True
        pool_uptime.StartInfo.UseShellExecute = False
        pool_uptime.StartInfo.CreateNoWindow = True
        Application.DoEvents()
        AddHandler pool_uptime.ErrorDataReceived, AddressOf pool_uptime_OutputDataReceived
        AddHandler pool_uptime.OutputDataReceived, AddressOf pool_uptime_OutputDataReceived
        pool_uptime.Start()
        pool_uptime.BeginErrorReadLine()
        pool_uptime.BeginOutputReadLine()
        pool_uptime_state = True
    End Sub
    Private Sub load_pool_list()
        'DOWNLOAD POOL LIST AND ASSOCIATED FILES
        'load pool list
        droplist_pool.Items.Clear()

        Using listclient = New WebClient()
            listclient.DownloadFile("https://parthk.co.uk/" & coin & "-poolbase/pool.list", ".\hotassets\" & coin & "_pool.list")
            Dim pool_list_file() As String = IO.File.ReadAllLines(".\hotassets\" & coin & "_pool.list")
            listclient.Dispose()
        End Using

        For Each pool_url_loop As String In File.ReadLines(".\hotassets\" & coin & "_pool.list")
            Dim pool_array As String() = Split(pool_url_loop, ",")
            droplist_pool.Items.Add(pool_array(0))
        Next

        droplist_pool.Items.Add("Custom pool")
        droplist_pool.SelectedItem = droplist_pool.Items(0)
    End Sub
    Private Sub offline_load_pool_list()
        Dim pool_list As String = (".\hotassets\" & coin & "_pool.list")
        If System.IO.File.Exists(pool_list) = False Then
            Call load_pool_list()
        Else
            'load list without downloading excess files
            droplist_pool.Items.Clear()
            For Each pool_url_loop As String In File.ReadLines(".\hotassets\" & coin & "_pool.list")

                Dim pool_array As String() = Split(pool_url_loop, ",")
                droplist_pool.Items.Add(pool_array(0))
                If pool_array(0) = droplist_pool.SelectedItem() Then
                    pool_info = pool_array
                    Exit For
                End If
            Next
            droplist_pool.Items.Add("Custom pool")
            droplist_pool.SelectedItem = droplist_pool.Items(0)
        End If
    End Sub
    Private Sub ez_ticker()
        System.Diagnostics.Process.Start("ez_ticker.exe")
        ticker_results = File.ReadAllLines("ticker_results.dat")
        ticker_list = File.ReadAllLines("ticker_list.dat")
        'update widgets without dynamically renaming controls
        widget_1_title.Text = ticker_list(0) & " Price (CMC)"
        widget_1_price.Text = ticker_results(0) & " $"
        widget_2_title.Text = ticker_list(1) & " Price (CMC)"
        widget_2_price.Text = ticker_results(1) & " $"
        widget_3_title.Text = ticker_list(2) & " Price (CMC)"
        widget_3_price.Text = ticker_results(2) & " $"
        widget_4_title.Text = ticker_list(3) & " Price (CMC)"
        widget_4_price.Text = ticker_results(3) & " $"


        Dim support_api As String = "0"
        If droplist_pool.Text = "" Then
        Else
            support_api = droplist_pool.SelectedItem()
        End If
        If support_api.Contains("nanopool") And support_api.Contains("etn") And textbox_wallet_address.Text.Contains("etn") Then
            Call ETN_earned_Nanopool()
        Else
            lbl_coin_Earned.Text = coin.ToUpper & " Earned: Not supported by this pool"
        End If
    End Sub
    Delegate Sub xmr_stak_boxDelg(xmr_stak_text As String)
    Public xmr_stak_delegate As xmr_stak_boxDelg = New xmr_stak_boxDelg(AddressOf xmr_stak_output)
    Public Sub xmr_stak_output(xmr_stak_text As String)
        If xmr_stak_text.Contains("Totals:") Then
            'Totals:    106.4   (na)   (na) H/s
            'Totals:     96.6   99.1   (na) H/s
            Dim xmr_stak_text_processed As String = xmr_stak_text.Replace("Totals:    ", "")
            xmr_stak_text_processed.Replace(" H/s", "")
            Dim xmr_stak_text_split As String() = Split(xmr_stak_text_processed, " ")
            For Each bits As String In xmr_stak_text_split
                'i really tried to do it a better way, but all if not statements or for loops that check for spaces decide to only SOMETIMES work, this one i have guaranteed to work 30 times in a row.
                If bits.Contains("1") Or bits.Contains("2") Or bits.Contains("3") Or bits.Contains("4") Or bits.Contains("5") Or bits.Contains("6") Or bits.Contains("7") Or bits.Contains("8") Or bits.Contains("9") Or bits.Contains("0") Then
                    lbl_hashrate.Text = "Hashrate: " & bits & " H/s"
                    tab_2_label.Text = "Mine - " & bits & " H/s"
                    Exit For
                End If
            Next
        ElseIf xmr_stak_text.Contains("invalid address used for login") Then

            Call kill_xmr_stak()
            lbl_status.Text = "Status: Stopped, Invalid wallet address"
            button_start_miner.Enabled = True
            button_stop_miner.Enabled = False
        ElseIf xmr_stak_text.Contains("No such host is known") Then

            Call kill_xmr_stak()
            lbl_pool_status.Text = "Pool Status: Pool down/Invalid pool address"
            button_start_miner.Enabled = True
            button_stop_miner.Enabled = False

        End If
        mining_output.Text += xmr_stak_text & Environment.NewLine
        mining_output.SelectionStart = mining_output.Text.Length
        mining_output.ScrollToCaret()
        mining_output.Text = mining_output.Text.Replace("[0m", "")
    End Sub
    Public Sub xmr_stak_proc_OutputDataReceived(ByVal sender As Object, ByVal e As DataReceivedEventArgs)
        If Me.InvokeRequired = True Then
            Me.Invoke(xmr_stak_delegate, e.Data)
        Else
            xmr_stak_output(e.Data)
        End If
    End Sub

    Public Sub kill_xmr_stak()
        If xmr_stak_proc_state = True Then
            xmr_stak_proc.Kill()
            RemoveHandler xmr_stak_proc.ErrorDataReceived, AddressOf xmr_stak_proc_OutputDataReceived
            RemoveHandler xmr_stak_proc.OutputDataReceived, AddressOf xmr_stak_proc_OutputDataReceived
            xmr_stak_proc.CancelErrorRead()
            xmr_stak_proc.CancelOutputRead()
            xmr_stak_proc_state = False
            lbl_status.Text = "Status: Stopped"
        End If
    End Sub
    Private Sub xmr_stak()
        'create New config file
        If System.IO.File.Exists("config.txt") = False Then
            System.IO.File.Create("config.txt").Dispose()
        Else
            System.IO.File.Delete("config.txt")
            System.IO.File.Create("config.txt").Dispose()
        End If
        System.IO.File.Copy(".\backend\config-template.txt", ".\backend\config.txt", True)
        Dim stak_config_open As String = My.Computer.FileSystem.ReadAllText(".\backend\config.txt").Replace("algorithm", fullcoin)
        stak_config_open = stak_config_open.Replace("pooladdress", pool_info(0) & ":" & textbox_port.Text)
        stak_config_open = stak_config_open.Replace("walletaddress", textbox_wallet_address.Text)
        My.Computer.FileSystem.WriteAllText(".\backend\config.txt", stak_config_open, False)
        If droplist_donation.SelectedItem = droplist_donation.Items(0) Then
            xmr_stak_proc.StartInfo.FileName = ".\backend\xmr-stak"
        Else
            xmr_stak_proc.StartInfo.FileName = ".\backend\xmr-stak-no-donation"
        End If
        Dim args As String = " "
        If droplist_cpuorgpu.SelectedItem = droplist_cpuorgpu.Items(0) Then
            args = " --noAMD --noNVIDIA"
        ElseIf droplist_cpuorgpu.SelectedItem = droplist_cpuorgpu.Items(1) And droplist_vendor.SelectedItem = droplist_vendor.Items(1) Then
            args = " --noCPU --noNVIDIA"
        ElseIf droplist_cpuorgpu.SelectedItem = droplist_cpuorgpu.Items(1) And droplist_vendor.SelectedItem = droplist_vendor.Items(0) Then
            args = " --noCPU --noAMD"
        ElseIf droplist_cpuorgpu.SelectedItem = droplist_cpuorgpu.Items(1) And droplist_vendor.SelectedItem = droplist_vendor.Items(2) Then
            args = " --noCPU"
        ElseIf droplist_cpuorgpu.SelectedItem = droplist_cpuorgpu.Items(2) And droplist_vendor.SelectedItem = droplist_vendor.Items(1) Then
            args = " --noNVIDIA"
        ElseIf droplist_cpuorgpu.SelectedItem = droplist_cpuorgpu.Items(2) And droplist_vendor.SelectedItem = droplist_vendor.Items(2) Then
            args = " "
        End If
        xmr_stak_proc.StartInfo.Arguments = ("--noUAC" & args)
        xmr_stak_proc.StartInfo.WorkingDirectory = ".\backend\"
        xmr_stak_proc.StartInfo.RedirectStandardError = True
        xmr_stak_proc.StartInfo.RedirectStandardOutput = True
        xmr_stak_proc.EnableRaisingEvents = True
        xmr_stak_proc.StartInfo.UseShellExecute = False
        xmr_stak_proc.StartInfo.CreateNoWindow = True
        Application.DoEvents()
        AddHandler xmr_stak_proc.ErrorDataReceived, AddressOf xmr_stak_proc_OutputDataReceived
        AddHandler xmr_stak_proc.OutputDataReceived, AddressOf xmr_stak_proc_OutputDataReceived
        xmr_stak_proc.Start()
        xmr_stak_proc.BeginErrorReadLine()
        xmr_stak_proc.BeginOutputReadLine()
        xmr_stak_proc_state = True
    End Sub
    Delegate Sub MinerOutputBoxDelg(Text As String)
    Public myDelegate As MinerOutputBoxDelg = New MinerOutputBoxDelg(AddressOf MinerOutput)
    Public Sub MinerOutput(Text As String)
        If Text.Contains("CPU #") Then
            'Totals:    106.4   (na)   (na) H/s
            'Totals:     96.6   99.1   (na) H/s
            Dim cpuminer_multi_processed As String = Text
            'cpuminer_multi_processed.Replace("CPU # 1", "")
            'cpuminer_multi_processed.Replace("CPU # 2", "")
            'cpuminer_multi_processed.Replace("CPU# 3", "")
            'cpuminer_multi_processed.Replace(" H/s", "")
            'MessageBox.Show(cpuminer_multi_processed)
            Dim cpuminer_multi_text_split As String() = Split(cpuminer_multi_processed, " ")

            If cpuminer_multi_text_split(3).Contains("0") Then
                cpu0 = cpuminer_multi_text_split(4)
            End If
            If cpuminer_multi_text_split(3).Contains("1") Then
                cpu1 = cpuminer_multi_text_split(4)
            End If
            If cpuminer_multi_text_split(3).Contains("2") Then
                cpu2 = cpuminer_multi_text_split(4)
            End If
            If cpuminer_multi_text_split(3).Contains("3") Then
                cpu3 = cpuminer_multi_text_split(4)

            End If
            Dim totalhash As Single = cpu0 + cpu1 + cpu2 + cpu3
            lbl_hashrate.Text = "Hashrate: " & totalhash & " H/s"
            tab_2_label.Text = "Mine - " & totalhash & " H/s"
        ElseIf Text.Contains("pc2_login_decode") Then

            Call kill_cpuminer_multi()
            lbl_status.Text = "Status: Stopped, Invalid wallet address"
            button_start_miner.Enabled = True
            button_stop_miner.Enabled = False
        ElseIf Text.Contains("resolve host") Then

            Call kill_cpuminer_multi()
            lbl_pool_status.Text = "Pool Status: Pool down/Invalid pool address"
            lbl_status.Text = "Status: Stopped, check pool status"
            button_start_miner.Enabled = True
            button_stop_miner.Enabled = False
        End If
        mining_output.Text += Text & Environment.NewLine
        mining_output.SelectionStart = mining_output.Text.Length
        mining_output.ScrollToCaret()
        mining_output.Text = mining_output.Text.Replace("[0m", "")
    End Sub
    Public Sub cpum_OutputDataReceived(ByVal sender As Object, ByVal e As DataReceivedEventArgs)
        If Me.InvokeRequired = True Then
            Me.Invoke(myDelegate, e.Data)
        Else
            MinerOutput(e.Data)
        End If
    End Sub
    Public Sub kill_cpuminer_multi()
        If cpum_state = True Then
            cpum.Kill()
            RemoveHandler cpum.ErrorDataReceived, AddressOf cpum_OutputDataReceived
            RemoveHandler cpum.OutputDataReceived, AddressOf cpum_OutputDataReceived
            cpum.CancelErrorRead()
            cpum.CancelOutputRead()
            cpum_state = False
            lbl_status.Text = "Status: Stopped"
        End If
    End Sub
    Private Sub cpuminer_multi()
        cpum.StartInfo.FileName = ".\backend\cpuminer"
        cpum.StartInfo.Arguments = ("-a " & algo & " -o stratum+tcp://" & pool_info(0) & ":" & textbox_port.Text & " -u " & textbox_wallet_address.Text & " -p x -t " & textbox_threadcount.Text & "pause")
        cpum.StartInfo.WorkingDirectory = ".\backend\"
        cpum.StartInfo.RedirectStandardError = True
        cpum.StartInfo.RedirectStandardOutput = True
        cpum.EnableRaisingEvents = True
        cpum.StartInfo.UseShellExecute = False
        cpum.StartInfo.CreateNoWindow = True
        Application.DoEvents()
        AddHandler cpum.ErrorDataReceived, AddressOf cpum_OutputDataReceived
        AddHandler cpum.OutputDataReceived, AddressOf cpum_OutputDataReceived
        cpum.Start()
        cpum.BeginErrorReadLine()
        cpum.BeginOutputReadLine()
        cpum_state = True
    End Sub
    Private Sub main_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim comp_width As Integer = Screen.PrimaryScreen.Bounds.Width
        Dim comp_height As Integer = Screen.PrimaryScreen.Bounds.Height
        If comp_width < "1600" And comp_height < "900" Then
            'compatibility mode
            button_googleforms_report.Visible = False
            button_googleforms_list.Visible = False
        End If
        Call ez_update()
        droplist_background_colour.SelectedItem = droplist_background_colour.Items(0)
        droplist_donation.SelectedItem = droplist_donation.Items(0)
        'hide close tabs for future use
        tab_1_close.Visible = False
        tab_2_close.Visible = False
        tab_3_close.Visible = False
        tab_4_close.Visible = False
        f_1.BringToFront()
        Dim INFOfiles As String = ".\hotassets"
        For Each delINFOfiles In Directory.GetFiles(INFOfiles, "*.*", SearchOption.TopDirectoryOnly)
            File.Delete(delINFOfiles)
        Next
        Call ez_ticker()
        Call load_pool_list()
        droplist_cpuorgpu.SelectedItem = droplist_cpuorgpu.Items(0)
        button_stop_miner.Enabled = False
        droplist_vendor.SelectedItem = droplist_vendor.Items(0)


        If System.IO.File.Exists("auto.mcf") = True Then
            Dim config_contents_load As String() = File.ReadAllLines("auto.mcf")
            textbox_wallet_address.Text = config_contents_load(0)
            droplist_pool.SelectedItem = config_contents_load(1)
            textbox_custom_pool.Text = config_contents_load(2)
            textbox_port.Text = config_contents_load(3)
            textbox_threadcount.Text = config_contents_load(4)
            droplist_cpuorgpu.SelectedIndex = config_contents_load(5)
            droplist_vendor.SelectedIndex = config_contents_load(6)
            droplist_donation.SelectedIndex = config_contents_load(7)
            coin = config_contents_load(8)
            checkbox_load_config_on_startup.Checked = config_contents_load(9)
            checkbox_automatic_updates.Checked = config_contents_load(10)
            droplist_background_colour.SelectedItem = config_contents_load(11)
            If coin = "coin1" Then
                coin_2_Click(sender, e)
            End If
            If coin = "coin2" Then
                coin_2_Click(sender, e)
            End If
            If coin = "coin3" Then
                coin_2_Click(sender, e)
            End If

        End If
        Dim dateselect As String = f_1_richtextbox_changelog.Lines(0)
        f_1_richtextbox_changelog.Select(f_1_richtextbox_changelog.GetFirstCharIndexFromLine(0), dateselect.Length)
        f_1_richtextbox_changelog.SelectionColor = Color.DodgerBlue
    End Sub

    Private Sub main_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseDown
        drag = True
        mousex = Windows.Forms.Cursor.Position.X - Me.Left
        mousey = Windows.Forms.Cursor.Position.Y - Me.Top
    End Sub
    Private Sub main_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseMove
        If drag Then
            Me.Top = Windows.Forms.Cursor.Position.Y - mousey
            Me.Left = Windows.Forms.Cursor.Position.X - mousex
        End If
    End Sub
    Private Sub main_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseUp
        drag = False
    End Sub

    Private Sub ez_caller_Tick(sender As Object, e As EventArgs) Handles ez_caller.Tick
        Call ez_ticker()
    End Sub

    Private Sub tab_1_label_Click(sender As Object, e As EventArgs) Handles tab_1_label.Click
        tab_2.BackgroundImage = System.Drawing.Image.FromFile(".\graphics\tabs_nf.png")
        tab_3.BackgroundImage = System.Drawing.Image.FromFile(".\graphics\tabs_nf.png")
        tab_1.BackgroundImage = System.Drawing.Image.FromFile(".\graphics\tabs.png")
        tab_4.BackgroundImage = System.Drawing.Image.FromFile(".\graphics\tabs_nf.png")
        f_1.BringToFront()
    End Sub

    Private Sub tab_1_Click(sender As Object, e As EventArgs) Handles tab_1.Click
        tab_2.BackgroundImage = System.Drawing.Image.FromFile(".\graphics\tabs_nf.png")
        tab_3.BackgroundImage = System.Drawing.Image.FromFile(".\graphics\tabs_nf.png")
        tab_1.BackgroundImage = System.Drawing.Image.FromFile(".\graphics\tabs.png")
        tab_4.BackgroundImage = System.Drawing.Image.FromFile(".\graphics\tabs_nf.png")
        f_1.BringToFront()
    End Sub

    Private Sub tab_2_label_Click(sender As Object, e As EventArgs) Handles tab_2_label.Click
        tab_1.BackgroundImage = System.Drawing.Image.FromFile(".\graphics\tabs_nf.png")
        tab_3.BackgroundImage = System.Drawing.Image.FromFile(".\graphics\tabs_nf.png")
        tab_2.BackgroundImage = System.Drawing.Image.FromFile(".\graphics\tabs.png")
        tab_4.BackgroundImage = System.Drawing.Image.FromFile(".\graphics\tabs_nf.png")
        f_2.BringToFront()
    End Sub

    Private Sub tab_2_Click(sender As Object, e As EventArgs) Handles tab_2.Click
        tab_1.BackgroundImage = System.Drawing.Image.FromFile(".\graphics\tabs_nf.png")
        tab_3.BackgroundImage = System.Drawing.Image.FromFile(".\graphics\tabs_nf.png")
        tab_2.BackgroundImage = System.Drawing.Image.FromFile(".\graphics\tabs.png")
        tab_4.BackgroundImage = System.Drawing.Image.FromFile(".\graphics\tabs_nf.png")
        f_2.BringToFront()
    End Sub
    Private Sub tab_3_label_Click(sender As Object, e As EventArgs) Handles tab_3_label.Click
        tab_1.BackgroundImage = System.Drawing.Image.FromFile(".\graphics\tabs_nf.png")
        tab_2.BackgroundImage = System.Drawing.Image.FromFile(".\graphics\tabs_nf.png")
        tab_3.BackgroundImage = System.Drawing.Image.FromFile(".\graphics\tabs.png")
        tab_4.BackgroundImage = System.Drawing.Image.FromFile(".\graphics\tabs_nf.png")
        f_3.BringToFront()
    End Sub

    Private Sub tab_3_Click(sender As Object, e As EventArgs) Handles tab_3.Click
        tab_1.BackgroundImage = System.Drawing.Image.FromFile(".\graphics\tabs_nf.png")
        tab_2.BackgroundImage = System.Drawing.Image.FromFile(".\graphics\tabs_nf.png")
        tab_3.BackgroundImage = System.Drawing.Image.FromFile(".\graphics\tabs.png")
        tab_4.BackgroundImage = System.Drawing.Image.FromFile(".\graphics\tabs_nf.png")
        f_3.BringToFront()
    End Sub
    Private Sub tab_4_label_Click(sender As Object, e As EventArgs) Handles tab_4_label.Click
        tab_1.BackgroundImage = System.Drawing.Image.FromFile(".\graphics\tabs_nf.png")
        tab_2.BackgroundImage = System.Drawing.Image.FromFile(".\graphics\tabs_nf.png")
        tab_3.BackgroundImage = System.Drawing.Image.FromFile(".\graphics\tabs_nf.png")
        tab_4.BackgroundImage = System.Drawing.Image.FromFile(".\graphics\tabs.png")
        'f_4.BringToFront()
    End Sub

    Private Sub tab_4_Click(sender As Object, e As EventArgs) Handles tab_4.Click
        tab_1.BackgroundImage = System.Drawing.Image.FromFile(".\graphics\tabs_nf.png")
        tab_2.BackgroundImage = System.Drawing.Image.FromFile(".\graphics\tabs_nf.png")
        tab_3.BackgroundImage = System.Drawing.Image.FromFile(".\graphics\tabs_nf.png")
        tab_4.BackgroundImage = System.Drawing.Image.FromFile(".\graphics\tabs.png")
        'f_4.BringToFront()
    End Sub

    Private Sub button_start_miner_Click(sender As Object, e As EventArgs) Handles button_start_miner.Click
        If Not textbox_wallet_address.Text = "" Then
            If textbox_custom_pool.Enabled = True Then
                pool_info(0) = textbox_custom_pool.Text
                lbl_pool_address.Text = "Pool Address: " & pool_info(0)
                lbl_pool_status.Text = "Pool Status: Checking"
            End If
            Call xmr_stak()
            button_start_miner.Enabled = False
            button_stop_miner.Enabled = True
            lbl_status.Text = "Status: Mining"
        Else
            lbl_status.Text = "Status: Stopped, Invalid wallet address"
        End If
    End Sub

    Private Sub coin_1_Click(sender As Object, e As EventArgs) Handles coin_1.Click
        algo = "cryptonight"
        coin_1.BackgroundImage = Image.FromFile(".\graphics\xmrc.png")
        coin_2.BackgroundImage = Image.FromFile(".\graphics\etnuc.png")
        coin_3.BackgroundImage = Image.FromFile(".\graphics\aeonuc.png")
        coin = "xmr"
        fullcoin = "monero"
        Call offline_load_pool_list()
        coindex = "coin1"
    End Sub

    Private Sub coin_2_Click(sender As Object, e As EventArgs) Handles coin_2.Click
        algo = "cryptonight"
        coin_1.BackgroundImage = Image.FromFile(".\graphics\xmruc.png")
        coin_2.BackgroundImage = Image.FromFile(".\graphics\etnc.png")
        coin_3.BackgroundImage = Image.FromFile(".\graphics\aeonuc.png")
        coin = "etn"
        fullcoin = "monero"
        Call offline_load_pool_list()
        coindex = "coin2"
    End Sub

    Private Sub coin_3_Click(sender As Object, e As EventArgs) Handles coin_3.Click
        algo = "cryptolight"
        coin_1.BackgroundImage = Image.FromFile(".\graphics\xmruc.png")
        coin_2.BackgroundImage = Image.FromFile(".\graphics\etnuc.png")
        coin_3.BackgroundImage = Image.FromFile(".\graphics\aeonc.png")
        coin = "aeon"
        fullcoin = "aeon"
        Call offline_load_pool_list()
        coindex = "coin3"
    End Sub

    Private Sub droplist_pool_SelectedIndexChanged(sender As Object, e As EventArgs) Handles droplist_pool.SelectedIndexChanged
        If droplist_pool.SelectedItem = "Custom pool" Then
            textbox_custom_pool.Enabled = True
            textbox_custom_pool.Clear()
            lbl_pool_status.Text = "Pool Status: Start mining to update"
            lbl_pool_address.Text = "Pool Address: Start mining to update"
            lbl_pool_fee.Text = "Fee: Custom pool not supported"
            lbl_pool_payout.Text = "Minimum Payout: Custom pool not supported"
        Else
            textbox_custom_pool.Enabled = False
            For Each pool_url_loop As String In File.ReadLines(".\hotassets\" & coin & "_pool.list")

                Dim pool_array As String() = Split(pool_url_loop, ",")
                If pool_array(0) = droplist_pool.SelectedItem() Then
                    pool_info = pool_array
                    Exit For
                End If
            Next
            lbl_pool_address.Text = "Address: " & pool_info(0)
            lbl_pool_fee.Text = "Fee: " & pool_info(2) & "%"
            lbl_pool_payout.Text = "Minimum Payout: " & pool_info(3) & " " & coin
            If pool_info(1) = "default" Then
                textbox_port.Text = "5555"
            Else
                textbox_port.Text = pool_info(1)
            End If

            If textbox_wallet_address.Text = "" Then
            Else
                droplist_pool.Enabled = False
                lbl_pool_status.Text = "Pool Status: Checking"
                Call check_pool_uptime()

            End If
            Dim support_api As String = "0"
            If droplist_pool.Text = "" Then
            Else
                support_api = droplist_pool.SelectedItem()
            End If
            If support_api.Contains("nanopool") And support_api.Contains("etn") And textbox_wallet_address.Text.Contains("etn") Then
                Call ETN_earned_Nanopool()
            Else
            End If
        End If

    End Sub

    Private Sub button_stop_miner_Click(sender As Object, e As EventArgs) Handles button_stop_miner.Click
        Call kill_cpuminer_multi()
        Call kill_xmr_stak()
        button_start_miner.Enabled = True
        button_stop_miner.Enabled = False
    End Sub

    Private Sub button_clear_wallet_Click(sender As Object, e As EventArgs) Handles button_clear_wallet.Click
        textbox_wallet_address.Clear()
    End Sub

    Private Sub droplist_cpuorgpu_SelectedIndexChanged(sender As Object, e As EventArgs) Handles droplist_cpuorgpu.SelectedIndexChanged
        If droplist_cpuorgpu.SelectedItem = droplist_cpuorgpu.Items(0) Then
            droplist_vendor.Enabled = False
        Else
            droplist_vendor.Enabled = True
        End If
    End Sub

    Private Sub button_save_config_Click(sender As Object, e As EventArgs) Handles button_save_config.Click
        dialog_save_config.Filter = "Miner Configuration Files (*.mcf*)|*.mcf"
        If dialog_save_config.ShowDialog = Windows.Forms.DialogResult.OK _
        Then
            Dim config_contents_save As String = textbox_wallet_address.Text & vbNewLine & droplist_pool.SelectedItem & vbNewLine & textbox_custom_pool.Text & vbNewLine & textbox_port.Text & vbNewLine & textbox_threadcount.Text & vbNewLine & droplist_cpuorgpu.SelectedIndex & vbNewLine & droplist_vendor.SelectedIndex & vbNewLine & droplist_donation.SelectedIndex & vbNewLine & coindex & vbNewLine & checkbox_load_config_on_startup.CheckState & vbNewLine & checkbox_automatic_updates.CheckState & vbNewLine & droplist_background_colour.SelectedItem
            My.Computer.FileSystem.WriteAllText(dialog_save_config.FileName, config_contents_save, True)
        End If
    End Sub

    Private Sub button_load_config_Click(sender As Object, e As EventArgs) Handles button_load_config.Click
        dialog_config_open.Filter = "Miner Configuration Files (*.mcf*)|*.mcf"
        If dialog_config_open.ShowDialog = Windows.Forms.DialogResult.OK _
        Then
            Dim config_contents_load As String() = File.ReadAllLines(dialog_config_open.FileName)
            textbox_wallet_address.Text = config_contents_load(0)
            droplist_pool.SelectedItem = config_contents_load(1)
            textbox_custom_pool.Text = config_contents_load(2)
            textbox_port.Text = config_contents_load(3)
            textbox_threadcount.Text = config_contents_load(4)
            droplist_cpuorgpu.SelectedIndex = config_contents_load(5)
            droplist_vendor.SelectedIndex = config_contents_load(6)
            droplist_donation.SelectedIndex = config_contents_load(7)
            coin = config_contents_load(8)
            checkbox_load_config_on_startup.Checked = config_contents_load(9)
            checkbox_automatic_updates.Checked = config_contents_load(10)
            droplist_background_colour.SelectedItem = config_contents_load(11)
            If coin = "coin1" Then
                coin_2_Click(sender, e)
            End If
            If coin = "coin2" Then
                coin_2_Click(sender, e)
            End If
            If coin = "coin3" Then
                coin_2_Click(sender, e)
            End If
        End If
    End Sub

    Private Sub checkbox_load_config_on_startup_CheckedChanged(sender As Object, e As EventArgs) Handles checkbox_load_config_on_startup.CheckedChanged
        If checkbox_load_config_on_startup.Checked = True Then
            Dim config_contents_save As String = textbox_wallet_address.Text & vbNewLine & droplist_pool.SelectedItem & vbNewLine & textbox_custom_pool.Text & vbNewLine & textbox_port.Text & vbNewLine & textbox_threadcount.Text & vbNewLine & droplist_cpuorgpu.SelectedIndex & vbNewLine & droplist_vendor.SelectedIndex & vbNewLine & droplist_donation.SelectedIndex & vbNewLine & coindex & vbNewLine & checkbox_load_config_on_startup.CheckState & vbNewLine & checkbox_automatic_updates.CheckState & vbNewLine & droplist_background_colour.SelectedItem
            My.Computer.FileSystem.WriteAllText("auto.mcf", config_contents_save, True)
        ElseIf checkbox_load_config_on_startup.Checked = False AndAlso System.IO.File.Exists("auto.mcf") = True Then
            System.IO.File.Delete("config.txt")
        End If
    End Sub
    Private Sub ez_update()
        If checkbox_automatic_updates.Checked = True Then
            Using client = New WebClient()
                Dim versionnumber As String = client.DownloadString("https://parthk.co.uk/version.version")
                If Not versionnumber = version Then
                    Using listclient = New WebClient()
                        listclient.DownloadFile("https://parthk.co.uk/ez-updater.exe", ".\updates\ez_updater.exe")
                        listclient.Dispose()
                    End Using
                    Process.Start("ez_updater.exe")
                    Call kill_cpuminer_multi()
                    Call kill_pool_uptime()
                    Call kill_xmr_stak()
                    Me.Close()
                End If

            End Using
        End If
    End Sub

    Private Sub red_Click(sender As Object, e As EventArgs) Handles red.Click
        Call kill_cpuminer_multi()
        Call kill_pool_uptime()
        Call kill_xmr_stak()
        Me.Close()
    End Sub

    Private Sub droplist_background_colour_SelectedIndexChanged(sender As Object, e As EventArgs) Handles droplist_background_colour.SelectedIndexChanged
        If droplist_background_colour.SelectedItem = "Blue" Then
            Me.BackColor = Color.Navy
        ElseIf droplist_background_colour.SelectedItem = "Red" Then
            Me.BackColor = Color.Crimson
        ElseIf droplist_background_colour.SelectedItem = "Yellow" Then
            Me.BackColor = Color.Gold
        ElseIf droplist_background_colour.SelectedItem = "Green" Then
            Me.BackColor = Color.ForestGreen
        End If


    End Sub

    Private Sub yellow_Click(sender As Object, e As EventArgs) Handles yellow.Click
        Me.WindowState = FormWindowState.Minimized
    End Sub

    Private Sub button_googleforms_list_Click(sender As Object, e As EventArgs) Handles button_googleforms_list.Click
        Process.Start("https://docs.google.com/forms/d/e/1FAIpQLSdI61jsP261Fw8V0iawT5cmO-OL5OMDGJ5rKyLRcLq0nZaEWQ/viewform?usp=sf_link")
    End Sub

    Private Sub button_googleforms_report_Click(sender As Object, e As EventArgs) Handles button_googleforms_report.Click
        Process.Start("https://docs.google.com/forms/d/e/1FAIpQLScP9LeYOPkAIj2jjbGmW_kV7ELVYIrgXP81O3ERgrmI2EDunw/viewform?usp=sf_link")

    End Sub
End Class