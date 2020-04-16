---
title: C# 웹 다운로더 만들기
date: '2020-04-16 16:30:00'
categories: C#/Windows_Forms
comments: true
---

Visual Studio 2017 환경에서 C# Windows Forms 앱으로 Web Downloader를 구현해보자.  

(이 글은 가메출판사의 C# 7.0프로그래밍 실전프로젝트를 통해 공부한 내용을 작성했다.)  

### 1. 새로운 프로젝트 만들기

**파일->새로만들기->프로젝트 (Ctrl + Shift + N)**를 누르고 Visual C# 카테고리의 Windows Forms 앱 프로젝트를 하나 만든다.  

### 2. form1 디자인

![WebDownloader](https://user-images.githubusercontent.com/41281307/79425273-6919b500-7ffc-11ea-8b9e-acd5859873f2.PNG)  
위와 같이 **form1**을 Label, TextBox, Button, CheckBox, ProgressBar, WebClient, FolderBrowerDialog를 통해 만들어주자.

*Label: lblUrl (Text: 주소)  
*TextBox: txtUrl  
*Button: btnDown (Text: 다운로드, Enabled: False)  
*Button: btnFolder (Text: 폴더)  
*CheckBox: cbOpen (Text: 창 열기)  
*ProgressBar: pgbDownload  
*WebClient: webClient  
*FolderBrowerDialog: fdbFile  

작동방식은 아래와 같다.

1. 폴더 버튼 클릭 시, 다운로드 경로를 설정한다.  
2. 다운로드 경로가 설정되면 다운로드 버튼이 활성화된다.  
3. 창 열기에 체크한 후 다운로드 시, 완료되면 다운로드 받은 경로의 탐색기 창이 열린다.  
4. 체크하지 않았다면, 다운로드가 완료되었다고 알림을 띄워준다.  

**webClient가 보이지 않을 시**  
나는 visual studio 2017버전 (.NET Framework 4.6.1)을 사용했는데, webClient가 도구상자에서 보이지 않았다. 구글링을 통해 얻은 방법은 다음과 같다.

1. 도구상자에서 우클릭하여 **항목 선택**을 선택한다.  
2. **.NET Framework 구성 요소**탭에서 **webClient**를 검색하여 추가해준다.  

### 3. form1 코드

```C#
using System.Net;			//webClient
using System.Diagnostics;	//Progress 클래스 사용
.
.
.
    public partial class Form1 : Form
    {

        bool isBusy = false;
        private string filePath = null;
       
        public Form1()
        {
            InitializeComponent();
        }

        private void btnFolder_Click(object sender, EventArgs e)	//버튼 "폴더"를 더블클릭하여 생성
        {
            if (this.fdbFile.ShowDialog() == DialogResult.OK)	//경로가 선택되면
            {
                this.btnDown.Enabled = true;	//다운로드 버튼 활성화
                filePath = this.fdbFile.SelectedPath;
            }
        }

        private void btnDown_Click(object sender, EventArgs e)	//버튼 "다운로드"를 더블클릭하여 생성
        {
            if (isBusy)		//이미 작업이 진행중인 경우
            {
                webClient.CancelAsync();		//작업을 중단
                isBusy = false;
            }
            else
            {
                try
                {
                    var strFileName = this.txtUrl.Text.Split(new char[] { '/' });	// '/'기준으로 입력된 URL을 분리
                    System.Array.Reverse(strFileName);	//배열 순서를 뒤집어서 파일 이름을 얻음
                    Uri uri = new Uri(this.txtUrl.Text);
                    var str = webClient.DownloadString(uri);
                    webClient.DownloadFileAsync(uri, filePath + @"\" + strFileName[0]);
                    isBusy = true;
                }
                catch
                {
                    this.btnDown.Enabled = false;
                    MessageBox.Show("다운로드가 실패 하였습니다.", "에러",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)	//webClient의 이벤트 목록 창에서 [DownloadProgressChanged]를 더블클릭하여 생성
        {
            this.pgbDownload.Value = e.ProgressPercentage;	//Progress 클래스를 이용하여 진행상황을 다운로드 바에 나타냄
        }

        private void webClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)	//webClient의 이벤트 목록 창에서 [DownloadFileCompleted]를 더블클릭하여 생성
        {
            isBusy = false;
            this.btnDown.Enabled = false;	//다운로드 버튼 비활성화
            if (e.Error == null)
            {
                if (this.cbOpen.Checked == false)	//창 열기 미체크 시
                    MessageBox.Show("다운로드가 완료되었습니다.", "알림",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                else	//창 열기 체크 시
                {
                    Process myProcess = new Process();
                    myProcess.StartInfo.FileName = filePath;
                    myProcess.Start();
                }
            }
            else
                MessageBox.Show("다운로드가 실패하였습니다.:" + e.Error.Message.ToString());
        }



    }
```
**webClient 관련 오류**  
webClient를 사용할 때, Form1.Designer.cs 파일에서 this.webClient.AllowReadStreamBuffering과 this.webClient.AllowWriteStreamBuffering를 지원하지 않는다고 오류가 뜬다.   
이는 .NET Framework의 버전이 올라가면서 더 이상 저 두 가지의 API를 지원해주지 않기 때문인데, 저 두 부분을 주석처리 해주면 오류 없이 잘 실행이 된다.  

**C# Windows Forms 초보 유의사항**  
내가 초보이기 때문에 겪었던 오류인데, 위의 코드에서 주석으로 "~를 더블클릭하여 생성"이라는 부분은 그대로 더블클릭해서 생성해주는 것이 좋다.  
직접 타이핑한다면, Form1.Designer.cs 파일에 해당 메서드에 관한 내용이 자동으로 추가가 되지 않기 때문에 실행해도 아무런 일이 일어나지 않는다.  




{% if page.comments %}
<div id="disqus_thread"></div>
<script>
/**
*  RECOMMENDED CONFIGURATION VARIABLES: EDIT AND UNCOMMENT THE SECTION BELOW TO INSERT DYNAMIC VALUES FROM YOUR PLATFORM OR CMS.
*  LEARN WHY DEFINING THESE VARIABLES IS IMPORTANT: https://disqus.com/admin/universalcode/#configuration-variables*/
/*
var disqus_config = function () {
this.page.url = PAGE_URL;  // Replace PAGE_URL with your page's canonical URL variable
this.page.identifier = PAGE_IDENTIFIER; // Replace PAGE_IDENTIFIER with your page's unique identifier variable
};
*/
(function() { // DON'T EDIT BELOW THIS LINE
var d = document, s = d.createElement('script');
s.src = 'https://hwnagto.disqus.com/embed.js';
s.setAttribute('data-timestamp', +new Date());
(d.head || d.body).appendChild(s);
})();
</script>
<noscript>Please enable JavaScript to view the <a href="https://disqus.com/?ref_noscript">comments powered by Disqus.</a></noscript>
{% endif %}