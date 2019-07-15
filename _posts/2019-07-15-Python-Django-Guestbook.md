---
title: Django 05. Django 방명록 앱 만들기
date: '2019-07-15 11:00:00'
categories: Python/Django
comments: true
---
이번 포스트에서는 Django로 방명록 앱을 만들어볼 것이다.

(이 포스트는 [YOUTUBE : LIFE SOFT](https://www.youtube.com/channel/UCqRTjWqD-ZWHj0ZoPSKVWBw) 님이 게시하시는 글을 통해 공부하고 작성한 것이다.)

### 1. 새로운 프로젝트 만들기

**File -> New -> Other (Ctrl + N)**을 누르면 다양한 옵션들이 뜰 것이다.  
**Django**를 검색하면 나오는 **PyDev**의 **Django**를 선택하고, 이름을 정해서 만들어주자.   

### 2. 기본 테이블 생성 및 관리자 계정 (super user) 생성

<code> python manage.py migrate </code>  
**Anaconda Prompt**에서 해당 프로젝트의 디렉토리로 간 뒤 이 코드를 입력해서 **db.sqlite3**라는 기본 테이블을 해당 디렉토리에 생성하자.  
<code> python manage.py createsuperuser  </code>  
또한, 이 코드를 입력해서 관리자 계정을 만들어주자.

### 3. 방명록 앱 만들기 및 세팅

**Anaconda prompt**에서 해당 프로젝트의 디렉토리에 다음 코드를 입력한다.  

<pre><code>python manage.py startapp guestbook </code></pre>
그러면 해당 디렉토리에 메모장 앱에 관련된 디렉토리가 추가된다.  
이 작업 후에  **projname/settings.py**를 조금 수정해야한다.   
또한, **INSTALLED_APPS**라는 항목 안에 **‘guestbook’,**를 추가해주고, **LANGUAGE_CODE**을 **‘ko-kr’**, **TIME_ZONE**을 **‘Asia/Seoul’**로 수정해준다.

### 4. 모델 클래스 정의와 Admin 사이트 설정

**<u>guestbook/models.py</u>**  

```python
from django.db import models
from datetime import datetime

class Guestbook(models.Model):
    idx=models.AutoField(primary_key=True)
    name=models.CharField(null=False,max_length=50)
    email=models.CharField(null=False,max_length=50)
    passwd=models.CharField(null=False,max_length=50)
    content=models.TextField(null=False)
    post_date=models.DateTimeField(default=datetime.now,blank=True)
```

**django**에서 지원하는 **models**의 **Model**이라는 클래스를 상속하는 자식 클래스 **Guestbook**를 만들어준다. 

**<u>guestbook/admin.py</u>**  

```python
from django.contrib import admin
from guestbook.models import Guestbook

class GuestbookAdmin(admin.ModelAdmin):
    list_display = ("name", "email", "passwd", "content")
    
admin.site.register(Guestbook, GuestbookAdmin)
```

**GuestbookAdmin** 클래스를 만들어서 **Guestbook**라는 클래스가 관리자페이지에서 어떻게 보일지 정의해준다.   
**admin.site.register()** 함수를 통해 **Guestbook**와 **GuestbookAdmin**클래스를 관리자 페이지에 등록해준다.  

이제 이렇게 만든 모델 클래스들을 데이터베이스에 반영해준다.   

<pre><code>python manage.py makemigrations
python manage.py migrate</code>
</pre>

### 5.  웹서버 구동과 관리자 페이지

<code>python manage.py runserver localhost:80</code>

이 코드를 **anaconda** **prompt**에 입력해주면 이제 http://localhost 나 http://localhost:80를 통해 접속할 수 있다.  
주소 뒤에 **/admin**을 붙여서 관리자페이지에 접속해서 로그인해준다.  
관리자 페이지에서 **Guestbooks**에 이름, 이메일, 비밀번호, 내용을 간단히 입력해서 몇 명의 방명록을 만들어보자.

**SQLite** **Expert**를 통해 **db.sqlite3** 파일을 확인해보면, **guestbook_guestbook** 테이블에 내용이 추가된 것을 알 수 있다.  

### 6. Url과 페이지 작성

**<u>projname/urls.py</u>**  

```python
from django.contrib import admin
from django.urls import path
from guestbook import views

urlpatterns = [
    path('admin/', admin.site.urls),
    path('',views.list),
    path('write',views.write),
    path('gb_insert',views.insert),
    path('passwd_check',views.passwd_check),
    path('gb_update', views.update),
    path('gb_delete', views.delete),
]
```

이번에는 **url**함수 대신에 **path**함수를 써서 만들었다.  
**url**함수는 정규표현식을 이용해서 비슷한 패턴의 웹 사이트들을 만들어낼 수 있고,  
**path**함수는 입력한 주소와 딱딱 맞아떨어지는 웹 사이트들을 만들어낼 수 있다.

**<u>guestbook/views.py</u>**  

```python
from django.shortcuts import render
from guestbook.models import Guestbook
from django.shortcuts import render_to_response
from django.views.decorators.csrf import csrf_exempt
from django.shortcuts import redirect
from django.db.models import Q
@csrf_exempt
def list(request):
    try: 
        searchkey=request.POST["searchkey"]
    except:
        searchkey="name"
    try:
        search=request.POST["search"]
    except:
        search=""
        
    if searchkey=="name_content":
        gbCount=Guestbook.objects.filter(
            Q(name__contains=search) | Q(content__contains=search)).count()
    elif searchkey=="name":
         gbCount=Guestbook.objects.filter(
            Q(name__contains=search)).count()
    elif searchkey=="content":
         gbCount=Guestbook.objects.filter(
            Q(content__contains=search)).count()
    if searchkey=="name_content":
        gbList=Guestbook.objects.filter(
            Q(name__contains=search) | Q(content__contains=search)).order_by("-idx")
    elif searchkey=="name":
         gbList=Guestbook.objects.filter(
            Q(name__contains=search)).order_by("-idx")
    elif searchkey=="content":
         gbList=Guestbook.objects.filter(
            Q(content__contains=search)).order_by("-idx")
    try:
        msg=request.GET["msg"]
    except:
        msg=""
    if msg == "error":
        msg = "Wrong Passwd"
    
    return render_to_response("list.html", {"gbList":gbList, "gbCount":gbCount, "msg":msg,
                                            "searchkey":searchkey, "search":search})

def write(request):
    return render_to_response("write.html")

@csrf_exempt
def insert(request):
    dto=Guestbook(name=request.POST["name"],
                  email=request.POST["email"],
                  passwd=request.POST["passwd"],
                  content=request.POST["content"])
    dto.save()
    return redirect("/")

@csrf_exempt
def passwd_check(request):
    id = request.POST["idx"]
    pwd = request.POST["passwd"]
    dto=Guestbook.objects.get(idx=id)
    if dto.passwd == pwd:
        return render_to_response("edit.html",{"dto": dto})
    else:
        return redirect("/?msg=error")
    
@csrf_exempt
def update(request):
    id=request.POST["idx"]
    dto=Guestbook(idx=id, name=request.POST["name"],
                  email=request.POST["email"],
                  passwd=request.POST["passwd"],
                  content=request.POST["content"])
    dto.save()
    return redirect("/")

@csrf_exempt
def delete(request):
    id=request.POST["idx"]
    Guestbook.objects.get(idx=id).delete()
    return redirect("/")
```

**list(request)**함수에서 **try**와 **except**함수는 **try** 내부에 있는 코드를 한번 시도해 보고, 오류가 뜨면 **except** 내부의 코드로 대체하는 것이다.

**Guestbook.objects.filter(Q(name__contains=search))**는 name이 포함된 항목을 검색하는 코드다.

**<u>guestbook/templates/list.html</u>**  

```html
{% raw %}<!DOCTYPE html>
<html>
<head>
<meta charset="UTF-8">
<title>Insert title here</title>
</head>
<body>
<h2>방명록</h2>
<form name="form1" method="post">
{% csrf_token %}
<select name="searchkey">
{% if searchkey == "name" %}
	<option value="name" selected>이름</option>
	<option value="content">내용</option>
	<option value="name_content">이름+내용</option>
{% elif searchkey == "content" %}
	<option value="name">이름</option>
	<option value="content" selected>내용</option>
	<option value="name_content">이름+내용</option>
{% else %}
	<option value="name">이름</option>
	<option value="content">내용</option>
	<option value="name_content" selected>이름+내용</option>
{% endif %}
</select>
<input type="text" name="search" value="{{search}}">
<input type="submit" value="조회">
</form>
{{gbCount}}개의 글이 있습니다.
<input type="button" value="글쓰기" onclick="location.href='write'">
<span style="color:red">{{msg}}</span>
{% for row in gbList %}
<form method="post" action="passwd_check">
{% csrf_token %}
<input type="hidden" name="idx" value="{{row.idx}}">
<table border="1" width="600px">
	<tr>
		<td>이름</td>
		<td>{{row.name}}</td>
		<td>날짜</td>
		<td>{{row.post_date|date:"Y-m-d G:i:s"}}</td>
	</tr>
	<tr>
		<td>이메일</td>
		<td colspan="3">{{row.email}}</td>
	</tr>
	<tr>
		<td colspan="4">{{row.content}}</td>
	</tr>
	<tr>
		<td colspn="4">
			비밀번호 <input type="password" name="passwd">
			<input type="submit" value="수정/삭제">
		</td>
	</tr>
</table>
</form>
{% endfor %}
</body>
</html>{% endraw %}
```

첫번째 폼 부분은 검색에 관한 부분인데, if문은 검색 옵션중에 선택된 것을 기본으로 보여주는 역할을 한다.  
아래 폼은 포문으로 검색된 항목들을 이름, 이메일, 비밀번호  순으로 보여준다.

**<u>guestbook/templates/write.html</u>**

```html
{% raw %}<!DOCTYPE html>
<html>
<head>
<meta charset="UTF-8">
<title>Insert title here</title>
<script>
function check(){
	if(document.form1.name.value==""){
		alert("이름을 입력하세요.");
		document.form1.name.focus();
		return;
	}
	if(document.form1.email.value==""){
		alert("이메일을 입력하세요.");
		document.form1.name.focus();
		return;
	}
	if(document.form1.passwd.value==""){
		alert("비밀번호를 입력하세요.");
		document.form1.name.focus();
		return;
	}
	if(document.form1.content.value==""){
		alert("내용을 입력하세요.");
		document.form1.name.focus();
		return;
	}
	document.form1.action="gb_insert";
	document.form1.submit();
}
</script>
</head>
<body>
<h2>방명록 작성</h2>
<form name="form1" id="form1" method="post">
{% csrf_token %}
<table border="1" width="500px">
	<tr>
		<td>이름</td>
		<td><input type="text" name="name" size="40"></td>
	</tr>
	<tr>
		<td>이메일</td>
		<td><input type="text" name="email" size="40"></td>
	</tr>
	<tr>
		<td>비밀번호</td>
		<td><input type="password" name="passwd" size="40"></td>
	</tr>
	<tr align="center">
		<td colspan="2"><textarea rows="5" cols="55" name="content"></textarea></td>
	</tr>
	<tr align="center">
		<td colspan="2"><input type="button" value="확인" onclick="check()">
			<input type="reset" value="취소"></td></tr>
</table>
</form>
</body>
</html>
{% endraw %}
```

글쓰기를 할 때 보여주는 화면이다. 기본적으로, 빈 항목이 있으면 오류메시지를 띄우며 해당 내용을 입력하라고 한다. 취소버튼을 누르면 적었던 내용을 모두 없애준다.

**<u>guestbook/templates/edit.html</u>**

```html
{% raw %}<!DOCTYPE html>
<html>
<head>
<meta charset="UTF-8">
<title>Insert title here</title>
<script>
function gb_update(){
	document.form1.action = "gb_update";
	document.form1.submit();
}
function gb_delete(){
	if(confirm("삭제하시겠습니까?")){
		document.form1.action="gb_delete";
		document.form1.submit();
	}
}
</script>
</head>
<body>
<h2>방명록 수정/삭제</h2>
<form name="form1" id="form1" method="post">
{% csrf_token %}
<table border="1" width="500px">
	<tr>
		<td>이름</td>
		<td><input type="text" name="name" size="40" value="{{dto.name}}"></td>
	</tr>
	<tr>
		<td>이메일</td>
		<td><input type="text" name="email" size="40" value="{{dto.email}}"></td>
	</tr>
	<tr>
		<td>비밀번호</td>
		<td><input type="password" name="passwd" size="40"></td>
	</tr>
	<tr align="center">
		<td colspan="2"><textarea rows="5" cols="55" name="content">{{dto.content}}</textarea></td>
	</tr>
	<tr align="center">
		<td colspan="2">
			<input type="hidden" name="idx" value="{{dto.idx}}">
			<input type="button" value="수정" onclick="gb_update()">
			<input type="button" value="삭제" onclick="gb_delete()">
			<input type="button" value="목록" onclick="location.href='/'">
		</td>
	</tr>
</table>
</form>
</body>
</html>
{% endraw %}
```

수정과 삭제 기능이 있는 페이지이다. 이미 입력되어있는 내용을 기본적으로 불러와서 적어놓는다.   
목록 버튼을 누르면 첫 페이지로 돌아갈 수 있다. **hidden**타입의 **idx**는 수정이나 삭제를 할 때 쓰인다. 





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