---
title: Django 06. Django 회원가입과 로그인
date: '2019-07-15 14:30:00'
categories: Python/Django
comments: true
---
이번 포스트에서는 Django로 회원가입과 로그인을 구현해볼 것이다.

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

<pre><code>python manage.py startapp member </code></pre>
그러면 해당 디렉토리에 메모장 앱에 관련된 디렉토리가 추가된다.  
이 작업 후에  **projname/settings.py**를 조금 수정해야한다.   
또한, **INSTALLED_APPS**라는 항목 안에 **‘member’,**를 추가해주고, **LANGUAGE_CODE**을 **‘ko-kr’**, **TIME_ZONE**을 **‘Asia/Seoul’**로 수정해준다.

### 4. 모델 클래스 정의와 Admin 사이트 설정

**<u>guestbook/models.py</u>**  

```python
from django import forms
from django.contrib.auth.models import User

class UserForm(forms.ModelForm):
    class Meta:
        model = User
        fields = ["username", "email", "password"]
```

이번에는 모델을 직접 만들지 않고 **ModelForm**이라고 미리 만들어진 모델을 사용한다. **Meta**클래스는 클래스를 만드는 클래스라고 볼 수 있다.  **User** 클래스 또한 이미 정의되어있는 클래스이다. 

이미 있는 모델들을 사용한 것이기 때문에 **어드민 클래스**를 만드는 것과 아래의 코드를 **Anaconda Prompt**에 입력하는 것은 할 필요가 없다.  

<pre><code>python manage.py makemigrations
python manage.py migrate</code>
</pre>

이 코드들을 입력하면 변경사항이 없다고 뜰 것이다.

### 5.  웹서버 구동과 관리자 페이지

<code>python manage.py runserver localhost:80</code>

이 코드를 **Anaconda** **Prompt**에 입력해주면 이제 http://localhost 나 http://localhost:80를 통해 접속할 수 있다.  
주소 뒤에 **/admin**을 붙여서 관리자페이지에 접속해서 로그인해준다.  
관리자 페이지에서 **사용자(들)**에 사용자 이름(Username), 비밀번호를 입력해서 새로운 사용자를 몇 명 추가해보자.

**SQLite** **Expert**를 통해 **db.sqlite3** 파일을 확인해보면, **auth_user** 테이블에 내용이 추가된 것을 알 수 있다.  

### 6. Url과 페이지 작성

**<u>projname/urls.py</u>**  

```python
from django.contrib import admin
from django.urls import path
from member import views
urlpatterns = [
    path('admin/', admin.site.urls),
    
    path('',views.home, name='home'),
    path('join/', views.join, name='join'),
    path('login/', views.login_check, name='login'),
    path('logout/',views.logout, name='logout'),
]

```

**path** 함수의 3번째 인수 **name**은 별칭으로, 후에 템플릿에서 **<code>{% raw %}{% url 'name' %}{% endraw %}</code>**을 사용함으로써 해당 주소를 불러올 수 있다.

**<u>member/forms.py</u>**

```python
from django import forms
from django.contrib.auth.models import User

class UserForm(forms.ModelForm):
    class Meta:
        model = User
        fields = ["username", "email", "password"]

class LoginForm(forms.ModelForm):
    class Meta:
        model = User
        fields = ["username", "password"]
```

회원가입과 로그인할 때 사용할 폼들을 정의해준다.  
**User**이라는 폼에서 해당 **fields**를 사용하겠다는 뜻이다.

**<u>member/views.py</u>**  

```python
from django.shortcuts import render
from member.forms import UserForm, LoginForm
from django.contrib.auth.models import User
from django.contrib.auth import (authenticate, login as django_login, logout as django_logout, )
from django.shortcuts import redirect, render_to_response

def home(request):
    if not request.user.is_authenticated:
        data = {"username" : request.user,
              "is_authenticated" : request.user.is_authenticated}
    else:
        data = {"last_login" : request.user.last_login,
                "username" : request.user.username,
                "password" : request.user.password,
                "is_authenticated" : request.user.is_authenticated}
    return render(request,"index.html",context={"data":data})

def join(request):
    if request.method == "POST":
        form = UserForm(request.POST)
        if form.is_valid():
            new_user = User.objects.create_user(**form.cleaned_data)
            django_login(request, new_user)
            return redirect("/")
        else:
            return render_to_response("index.html", 
                                      {"msg":"failed to sign up..."})
    else:
        form = UserForm()
        return render(request, "join.html", {"form":form})

def logout(request):
    django_logout(request)
    return redirect("/")

def login_check(request):
    if request.method == "POST":
        form = LoginForm(request.POST)
        name = request.POST["username"]
        pwd = request.POST["password"]
        user = authenticate(username=name, password=pwd)
        if user is not None:
            django_login(request, user)
            return redirect("/")
        else:
            return render_to_response("index.html",
                                      {"msg":"Login Failure..."})
    else:
        form = LoginForm()
        return render(request, "login.html", {"form":form})
```

**home**함수는 템플릿**index.html**에 기본적인 데이터를 보내주는 함수인데, 로그인이 되어있지 않다면 **username(AnonymousUser)**와 로그인이 되어있지 않다는 것을 보내준다. 로그인이 되어있다면 **해당 유저가 마지막으로 로그인한 시간**, **username**, **password**와 로그인이 되어있는 상태라는 데이터를 보내준다. 

**join**이나 **login_check**함수는 처음에 사이트로부터 아무 데이터를 받기 전에 Form을 만들어서 사이트에 보내주고, 사이트에 사용자가 데이터를 입력해서 전송하면 회원가입이나 로그인 절차를 진행해주는 함수다.

**<u>member/templates/index.html</u>**  

```html
{% raw %}<!DOCTYPE html>
<html>
<head>
<meta charset="UTF-8">
<title>Insert title here</title>
</head>
<body>
<h2>Home</h2>
{% if data.is_authenticated %}
	<h2>회원 로그인 관련 정보</h2>
	<ul>
		{% for key,value in data.items %}
			<li>{{key}} : {{value}}</li>
		{% endfor %}
	</ul>
	<a href="/logout">로그아웃</a>
{% else %}
	<h2>로그인하세요</h2>
	로그인 상태 : {{data.is_authenticated}}<br>
	사용자 아이디 : {{data.username}}<br>
	<span style="color:red">{{msg}}</span>
	<a href="/login">로그인</a>
	<a href="/join">회원가입</a>
{% endif %}
</body>
</html>{% endraw %}
```

기본 화면에 대한 템플릿이다. 로그인이 되어있다면 해당 유저에 대한 정보들을 나열해준다. (마지막으로 로그인한 시간, username, password, 인증 여부) 그리고 로그아웃 링크도 출력해준다.  
로그인이 되어있지 않다면, 로그인을 하라는 말과 함께 로그인과 회원가입 버튼을 출력해준다.

**<u>member/templates/join.html</u>**

```html
{% raw %}<!DOCTYPE html>
<html>
<head>
<meta charset="UTF-8">
<title>Insert title here</title>
</head>
<body>
<h2>회원가입</h2>
<form method="post">
{% csrf_token %}
<table>
{{form.as_table}}
</table>
<input type="submit" value="회원가입">
</form>
</body>
</html>
{% endraw %}
```

**<code>{% raw %}{{form.as_table}}{% endraw %}</code>**을 통해 **Django**에서 지원해주는 폼을 불러와서 아까 **forms.py**에서 설정했던 **fields**를 불러온다. 

**<u>member/templates/login.html</u>**

```html
{% raw %}<!DOCTYPE html>
<html>
<head>
<meta charset="UTF-8">
<title>Insert title here</title>
</head>
<body>
<h2>로그인</h2>
<form method="post" action="{% url 'login' %}">
{% csrf_token %}
<table>
	<tr>
		<td>아이디</td>
		<td><input id="{{form.username.id_for_label}}" maxlength="15"
			name="{{form.username.html_name}}">
		</td>
	</tr>
	<tr>
		<td>비밀번호</td>
		<td><input id="{{form.password.id_for_label}}" maxlength="120"
			name="{{form.password.html_name}}" type="password">
		</td>
	</tr>
	<tr>
		<td colspan="2" align="center">
			<input type="submit" value="로그인">
			<input type="button" value="취소" onclick="location.href='/';">
		</td>
	</tr>
</table>
</form>
</body>
</html>
{% endraw %}
```

회원가입할 때와 같이 할 수도 있고, 이처럼 조금 더 커스터마이징해서 만들 수도 있다.





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