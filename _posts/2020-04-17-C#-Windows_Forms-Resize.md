---
title: C# 윈폼 인터페이스 크기 조절
date: '2020-04-17 17:30:00'
categories: C#/Windows_Forms
comments: true
---

Visual Studio 2017 환경에서 C# Windows Forms 앱의 인터페이스 크기를 조절해보자.

### 1. 결과화면

![winapp_practice_5](https://user-images.githubusercontent.com/41281307/79549089-7359b300-80d1-11ea-8b69-de51f3ffa1bf.png)

디자인한 cs파일과 실제로 실행했을 때의 창 크기가 달라서 이런 식으로 오른쪽이 남게 되는 상황이다.

![winapp_practice_1](https://user-images.githubusercontent.com/41281307/79548610-a9e2fe00-80d0-11ea-8c96-4f6157f90611.png)

크기를 잘 조절해서 정렬한다면 이 화면처럼 만들 수 있다.

Resize 메소드를 만들어서 정렬했다.

**폼 컨트롤**

*등록월: lbMon / txtMon  
*등록일: lbDay / txtDay  
*거래처명: lbtNm / txtNm  
*거래처코드: lbtCode / txtCode  
*비고: lbNote / txtNote  
*계획등록일: lbRegDay / txtDay1 / lbDash / txtDay2  
*검색구분: lbSrh / txtSrh  
*거래처명(콤보박스): cbxNm  
*수정: btnEdit  
*등록: btnReg  
*입력초기화: btnInit  
*품목추가: btnAppend  
*품목삭제: btnDel  
*검색: btnSrh  
*선택삭제: btnSelDel  
*DataGridView: dgvRcs   
*SplitContainer 有

**Anchor**속성은 모두 Top, Left다.

### 2. Resize

```C#
.
.
.
private void Resize(object sender, EventArgs e)
{

    lbDay.Left = Convert.ToInt32(dgvRcs.Right * 0.25);
    lbtNm.Left = Convert.ToInt32(dgvRcs.Right * 0.5);
    lbtCode.Left = Convert.ToInt32(dgvRcs.Right * 0.75);

    txtMon.Left = txtNote.Left = lbMon.Right + 5;
    txtDay.Left = lbDay.Right + 5;
    txtNm.Left = lbtNm.Right + 5;
    txtCode.Left = lbtCode.Right + 5;

    txtMon.Width = lbDay.Left - txtMon.Left - 5;
    txtDay.Width = lbtNm.Left - txtDay.Left - 5;
    txtNm.Width = lbtCode.Left - txtNm.Left - 5;
    txtCode.Width = dgvRcs.Right - txtCode.Left;
    txtNote.Width = lbtCode.Left - txtNote.Left - 5;

    btnInit.Left = btnDel.Left = btnSelDel.Left = txtCode.Right - btnInit.Width;
    btnReg.Left = btnAdd.Left = btnSrh.Left = btnInit.Left - btnReg.Width - 5;
    btnEdit.Left = btnReg.Left - btnEdit.Width - 5;

    txtSrh.Left = lbtCode.Left;
    txtSrh.Width = btnSrh.Left - txtSrh.Left - 5;
    cbxNm.Left = txtSrh.Left - cbxNm.Width - 5;
    lbSrh.Left = cbxNm.Left - lbSrh.Width - 5;
    txtDay2.Width = txtDay1.Width = txtSrh.Width;
    txtDay2.Left = lbSrh.Left - txtDay2.Width - 5;
    lbDash.Left = txtDay2.Left - lbDash.Width - 5;
    txtDay1.Left = lbDash.Left - txtDay1.Width - 5;
    lbRegDay.Left = txtDay1.Left - lbRegDay.Width - 5;

}
.
.
.
```

작동방식은 아래와 같다. 

1. 우선, 기본적으로 자동으로 Column 너비를 조정해주는 DataGridView기준으로 너비가 정해져있는 Label 종류의 좌측 자리부터 잡아준다.

   

2. 그 후, TextBox들의 좌측 자리와 너비를 잡아준다.   
   TextBox들의 좌측 기준은 "해당하는 Label의 우측 끝 + 5"를 기준으로 잡았다.  
   그리고, TextBox들의 너비는 "우측 Label의 좌측 값 - 자신의 좌측값 - 5"로 정해주어서 우측 Label과의 5 간격을 유지해준다.

   

3. 같은 방법으로 버튼과 폼 컨트롤들을 정렬해준다.






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