---
title: C# 트레이 메시지 만들기
date: '2020-04-16 14:30:00'
categories: C#/Windows_Forms
comments: true
---

Visual Studio 2017 환경에서 C# Windows Forms 앱으로 Tray Message를 구현해보자.

### 1. 새로운 프로젝트 만들기

**파일->새로만들기->프로젝트 (Ctrl + Shift + N)**를 누르고 Visual C# 카테고리의 Windows Forms 앱 프로젝트를 하나 만든다.  

### 2. form1 만들기

![ex_screenshot](./img/Traymsg_form1.png)  
위와 같이 form1을 버튼과 텍스트박스를 이용해 만들어준다.

*버튼: btnMsg
*텍스트박스: txtMsg

form1에 다음 코드를 추가해주자.

```C#
private void btnMsg_Click(object sender, EventArgs e)
{
    Form2 frm2 = new Form2();
    frm2.MsgText = this.txtMsg.Text;
    frm2.ShowDialog();
}
```
버튼을 클릭 시 form2를 텍스트박스에 입력되어 있는 값을 전달한 후 출력한다.

### 3. form2 만들기

![ex_screenshot](./img/Traymsg_form2.png)  
위와 같이 form2를 Panel과 LinkLabel을 이용해서 만들어준다.  

*Panel: plBack
*LinkLabel: lblResult

form2의 작성코드는 다음과 같다.
```C#
using System.Timers;	//Timer 클래스 사용
.
.
.
public partial class Form2 : Form
{
    private static System.Timers.Timer TimerEvent;	//Timer 개체 생성
    private delegate void OnDelegateHeight(int Flag);	//대리자 선언
    private OnDelegateHeight OnHeight = null;		//대리가 개체 생성

    public string MsgText	//Form1에서 입력받은 문자열을 전달
    {
    	set
    	{
    	this.lblResult.Text = value;
    	}
    }

    public Form2()	//InitializeComponent 호출 전 메시지 창의 위치와 크기를 설정
    {
        int x = Screen.PrimaryScreen.WorkingArea.Width - this.Width - 20;
        int y = Screen.PrimaryScreen.WorkingArea.Height - this.Height;
        DesktopLocation = new Point(x, y);
        this.Size = new Size(170, 0);
        InitializeComponent();
    }

    private void Form2_Load(object sender, EventArgs e)	//폼을 더블클릭하여 생성
    {
        OnHeight = new OnDelegateHeight(MsgView);		//대리자 초기화
        this.Size = new System.Drawing.Size(170, 0);
        this.Location = new System.Drawing.Point(
        Screen.PrimaryScreen.WorkingArea.Width - this.Width - 20,
        Screen.PrimaryScreen.WorkingArea.Height - this.Height);
        TimerEvent = new System.Timers.Timer(2);		//2밀리 초마다 TimerEvent초기화
        TimerEvent.Elapsed += new ElapsedEventHandler(OnPopUp);
        TimerEvent.Start();
    }

    private void MsgView(int Flag)		//폼을 위아래로 이동 혹은 종료하는 메서드
    {
        if (Flag == 0)	//폼을 올린다
        {
            Height++;
            Top--;
        }
        else if (Flag == 1)	//폼을 내린다
        {
            Height--;
            Top++;
        }
        else if (Flag == 2)
        {
            this.Close();
        }
    }

    private void OnPopUp(object sender, ElapsedEventArgs e)	//폼을 올리는 작업을 수행하는 메서드
    {
        if (Height < 120)	//Height가 120보다 작을 시 폼을 올린다
        {
            Invoke(OnHeight, 0);
        }
        else				//Height가 120이상일 시, 폼을 내리는 작업으로 변경
        {
            TimerEvent.Stop();
            TimerEvent.Elapsed -= new ElapsedEventHandler(OnPopUp);
            TimerEvent.Elapsed += new ElapsedEventHandler(OnPopOut);
            TimerEvent.Interval = 3000;		//3초 정지
            TimerEvent.Start();
    	}
        Application.DoEvents();
    }

    private void OnPopOut(object sender, ElapsedEventArgs e)	//폼을 내리는 작업을 수행하는 메서드
    {
        while (Height > 2)	//Height가 2보다 크면 폼을 내린다
        {
            Invoke(OnHeight, 1);
        }
        TimerEvent.Stop();	//Height가 2이하일 시 폼을 종료한다
        Application.DoEvents();
        Invoke(OnHeight, 2);
    }

    private void lblResult_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)	//LinkLabel을 더블클릭하여 생성
    {
        this.Close();			//폼 종료
        TimerEvent.Stop();
    }
}
```

### 4. Delegate, Invoke

**대리자(Delegate)**는 C/C++에서의 함수 포인터와 비슷한 역할이라고 볼 수 있다.
대리자는 이름 그대로, 메소드를 대신해서 호출하는 역할을 수행한다.

```C#
private delegate void OnDelegateHeight(int Flag);	//대리자 선언
private OnDelegateHeight OnHeight = null;		//대리가 개체 생성
.
.
OnHeight = new OnDelegateHeight(MsgView);		//대리자 초기화
.
.
private void MsgView(int Flag)		//폼을 위아래로 이동 혹은 종료하는 메서드
{
    if (Flag == 0)	//폼을 올린다
    {
        Height++;
        Top--;
    }
    else if (Flag == 1)	//폼을 내린다
    {
        Height--;
        Top++;
    }
    else if (Flag == 2)
    {
        this.Close();
    }
}
```
위에 있던 대리자가 사용되었던 부분이다.  
**OnHeight**라는 대리자 개체를 **MsgView**라는 함수를 대리할 수 있도록 초기화해놓았다.  
이제 **OnHeight(0)**과 **MsgView(0)**은 같은 결과를 나타낼 것이다. 하지만, 왜 굳이 이런 식으로 대리자를 사용했을까?  

우선, **Invoke**에 대해 알아보자.
```C#
    private void OnPopOut(object sender, ElapsedEventArgs e)	//폼을 내리는 작업을 수행하는 메서드
    {
        while (Height > 2)	//Height가 2보다 크면 폼을 내린다
        {
            Invoke(OnHeight, 1);
        }
        TimerEvent.Stop();	//Height가 2이하일 시 폼을 종료한다
        Application.DoEvents();
        Invoke(OnHeight, 2);
    }
```
**크로스 스레드 문제** (특정 스레드에서 생성된 Windows Forms 컨트롤을 다른 스레드에서 접근할 때 생기는 문제)를 해결하는 방법 중 하나가 **Invoke**문을 사용하는 것이다.  
**Invoke**를 사용하기 위해서는 메서드를 매개변수로 주어야하고, 대리자는 이를 가능하게 해준다.





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